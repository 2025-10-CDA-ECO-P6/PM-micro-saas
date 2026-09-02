# ADR-010 — Suppression d'espace (soft-delete + purge + saga)

> **Annotation 2026-06-12** — Renommage suite à ADR-018 (généralisation Espace) : `CampaignDeleted` → `SpaceDeleted`, `Campaign.Delete()` → `Space.Delete()`. Précision de périmètre : la suppression utilisateur (soft-delete + corbeille J+30) s'applique aux espaces **supprimables par le MJ** — types `CAMPAIGN` et `ONE_SHOT`. L'espace `PERSONAL` est le conteneur par défaut, **non supprimable par l'utilisateur** ; il est purgé uniquement à la suppression du compte (`UserDeleted`, inconditionnel — voir ADR-011/012).

- **Statut** : Accepté
- **Date** : 2026-06-09

---

## Contexte

Le propriétaire d'un espace de type `CAMPAIGN` ou `ONE_SHOT` a le droit de supprimer cet espace. Cependant, le modèle de domaine `Space` ne propose que la méthode `Archive()` — aucune méthode `Delete()` n'existe. Aucun événement `SpaceDeleted` n'est défini, et il n'y a pas de stratégie de cascade pour effacer ou dissocier les données rattachées (documents, dossiers, memberships, sessions, guest accesses).

Cette lacune impacte plusieurs couches :
- **Domaine** : l'invariant « le propriétaire peut supprimer son espace » est inexprimable.
- **RGPD** : l'utilisateur a le droit à l'effacement de ses données ; la suppression d'espace est un cas majeur (cf. ADR-007 pseudonymisation et ADR-009 FK `ownerId`).
- **Cascade** : les données possédées par l'espace doivent être traitées de manière cohérente lors de sa suppression.

La distinction entre `Archive()` (mettre de côté, réversible) et `Delete()` (suppression, éventuellement irréversible) doit être clarifiée avant l'entrée en construction.

> **Périmètre de suppression par l'utilisateur** : seuls les espaces de types `CAMPAIGN` et `ONE_SHOT` sont supprimables par le MJ. L'espace `PERSONAL` est le conteneur par défaut de l'utilisateur — il ne peut pas être supprimé par l'utilisateur (de même qu'on ne supprime pas sa bibliothèque personnelle). Il est purgé uniquement à la suppression du **compte** via l'événement `UserDeleted` (inconditionnel — voir ADR-011/012). Les deux chemins sont distincts et ne se substituent pas l'un à l'autre.

---

## Décision

**Soft-delete + purge à 30 jours + saga `SpaceDeleted`.**

### 1. Soft-delete — introduction d'une corbeille

Un espace supprimé est marqué comme supprimé (statut ou timestamp `deleted_at`) **sans effacement physique immédiat**. L'espace reste en base et est logiquement invisible aux utilisateurs (les requêtes par défaut l'excluent). Un utilisateur peut **récupérer l'espace dans une fenêtre de rétention de 30 jours** après la suppression.

**Distinction claire** :
- `Archive()` : l'espace est mis de côté (statut `Archived`). Il reste pleinement retrievable par l'interface d'archivage. Réversible (peut être restauré).
- `Delete()` : l'espace passe en corbeille (statut `Deleted` ou timestamp `deleted_at` défini). Il n'est pas visible aux utilisateurs standard. Récupérable pendant 30 jours, puis **purge physique**.

Ce mécanisme s'applique aux espaces supprimables par l'utilisateur (`CAMPAIGN`, `ONE_SHOT`). L'espace `PERSONAL` ne passe jamais par ce chemin ; sa suppression physique est déclenchée par `UserDeleted` (ADR-011/012).

### 2. Purge physique à 30 jours

Passé 30 jours en état supprimé, l'espace et toutes ses données rattachées sont **physiquement supprimés** (hard-delete). Un **job de purge** exécuté régulièrement (quotidien ou hebdomadaire) identifie les espaces avec `deleted_at ≤ now() - 30 days` et les supprime ainsi que leurs données dépendantes.

### 3. Saga `SpaceDeleted` — orchestration de la cascade

La suppression d'un espace émet un événement `SpaceDeleted(spaceId, ownerId)`. Une **saga** gère la propagation de la suppression à travers les agrégats et contextes concernés :

- **Content Library** : suppression de tous les documents et dossiers rattachés à l'espace.
- **Session Conduct** : suppression de toutes les sessions rattachées à l'espace.
- **Space Management** : suppression de toutes les memberships (records dans `space_memberships`, `membership_characters`, etc.) et de tous les guest accesses à l'espace, puis suppression de l'espace lui-même.

La saga garantit la **cohérence transactionnelle** de la cascade : soit tous les composants sont supprimés, soit la suppression est entièrement annulée (compensating transactions en cas d'erreur).

---

## Alternatives considérées

### Hard-delete immédiat (rejeté)

Supprimer physiquement l'espace dès que l'utilisateur déclenche la suppression. Avantages : simplicité, pas de purgeur de fond, libération immédiate de l'espace disque. **Inconvénients décisifs** :
- **Perte de données irréversible** : aucune fenêtre de récupération en cas de suppression accidentelle.
- **Tension avec l'intégrité référentielle** : la FK `SPACE.ownerId → USER` (ADR-009) empêche le hard-delete de la ligne `users` tant qu'un espace la référence. Hard-deleter l'espace immédiatement crée un risque asynchrone et une gestion complexe des contraintes.
- **Conflit RGPD** : si le propriétaire supprime son compte ET l'espace, la suppression du compte ne peut pas purger la ligne `users` tant qu'un espace non-supprimé la référence. L'immédiateté du hard-delete amplifie ce problème.
- **Pas aligné avec la posture de pseudonymisation** (ADR-007) : la conception adopte un modèle de **non-destruction initiale** (pseudonymisation/anonymisation au lieu de hard-delete) ; le hard-delete d'espace serait une exception contraire au modèle.

**Conclusion** : hard-delete immédiat est incompatible avec la stratégie RGPD et l'intégrité référentielle retenue.

### Archivage seul — pas de méthode `Delete()` (rejeté)

Utiliser uniquement `Archive()` et ne pas implémenter `Delete()`. Les espaces archivés peuvent être restaurés ultérieurement. **Inconvénients décisifs** :
- **Pas de suppression réelle** : le propriétaire demande à supprimer son espace ; l'archivage ne satisfait pas ce droit. Archiver ≠ supprimer.
- **Confusion sémantique** : archiver un espace (« je mets cette campagne en pause ») est distinct de le supprimer (« je supprime définitivement cet espace »). Les utiliser de manière interchangeable crée une mauvaise expérience utilisateur.
- **Pas de compliance RGPD directe** : un utilisateur ayant droit à l'effacement de ses données attend que la suppression élimine ou anonymise les données ; l'archivage n'offre pas cette garantie.

**Conclusion** : l'archivage seul ne suffit pas et crée une confusion métier.

### Soft-delete uniquement, pas de purge (rejeté)

Mettre les données en corbeille indéfiniment sans jamais les purger physiquement. **Inconvénients décisifs** :
- **Croissance infinie de la base de données** : les données supprimées restent éternellement en base, consommant du stockage sans limite.
- **Complexité de conformité** : l'obligation RGPD de droit à l'oubli implique une **suppression physique effective**, pas un marquage éternel.

**Conclusion** : la purge à 30 jours est nécessaire pour la conformité et la santé opérationnelle.

---

## Conséquences

### Modèle de domaine

- La décision implique que le modèle de domaine doit exposer une méthode `Space.Delete()` qui émet l'événement `SpaceDeleted`. Cette méthode est applicable aux types `CAMPAIGN` et `ONE_SHOT` ; elle n'est pas exposée pour le type `PERSONAL`.
- Elle nécessite l'ajout d'un attribut `deleted_at : DateTime?` (nullable) ou d'un statut `Status` avec valeur `Deleted` au modèle `Space`. Les requêtes de lecture par défaut filtreront `WHERE deleted_at IS NULL` (ou statut ≠ `Deleted`).
- Dissociation claire entre `Archive()` et `Delete()` dans les contrats et la documentation du domaine.
- **L'implémentation complète est prévue en J2** (hors périmètre du lot P0.5).

### Schéma et MLD

- La table `spaces` (et les tables rattachées) porte un champ `deleted_at TIMESTAMP NULL` ou une colonne de statut.
- Les index sur les requêtes d'énumération (ex. `spaces WHERE owner_id = ? AND deleted_at IS NULL`) doivent être correctement positionnés pour la performance.
- Un **job de purge** est programmé (ex. toutes les 24 h, ou quotidiennement) pour identifier et supprimer les espaces avec `deleted_at ≤ now() - INTERVAL '30 days'`.

### Saga et événements

- L'événement `SpaceDeleted(spaceId, ownerId, deletedAt)` est défini. Il est émis par la suppression volontaire d'un espace `CAMPAIGN` ou `ONE_SHOT` par son propriétaire. Il est distinct de la purge de l'espace `PERSONAL`, qui est déclenchée par `UserDeleted` (ADR-011/012).
- Une **saga orchestratrice** écoute cet événement et propage la suppression en cascade sur les agrégats dépendants (documents, dossiers, memberships, guest accesses, sessions, personnages).
- Les compensating transactions définissent le comportement en cas d'erreur partielle (ex. si la suppression d'une membership échoue, la saga doit annuler les suppressions antérieures).

### Interaction avec d'autres décisions

- **ADR-007 (Pseudonymisation RGPD)** : la suppression d'un espace est un cas d'usage majeur du droit à l'effacement. La saga garantit que toutes les données associées à l'espace et au propriétaire sont traitées de manière cohérente (suppression ou pseudonymisation selon le contexte).
- **ADR-009 (FK `ownerId`)** : la suppression d'un espace ne supprime pas le compte utilisateur du propriétaire. Les deux cycles de vie sont distincts : un espace peut être supprimé sans que le compte soit supprimé, et réciproquement. La FK `spaces.owner_id` reste valide pendant la fenêtre de soft-delete (30 j) et est supprimée avec la ligne `spaces` lors de la purge physique. Le mécanisme et l'ordre topologique complet de la purge sont définis dans **[ADR-011](ADR-011-cascade-integrite-referentielle.md)**.
- **ADR-011/012 (cascade `UserDeleted`)** : la purge de l'espace `PERSONAL` est un sous-cas de la suppression de compte, géré par la saga `UserDeleted`. Elle n'est pas couverte par le présent ADR, qui ne traite que la suppression volontaire d'un espace par son propriétaire.
- **Politique de rétention** : la corbeille 30 jours pour les espaces supprimables s'aligne avec la fenêtre de rétention générale retenue pour les données supprimées.

### Observation d'un utilisateur

- **Suppression accidentelle** : un utilisateur peut **restaurer** son espace depuis la corbeille pendant 30 jours après la suppression (interface à déterminer). Passé ce délai, l'espace est irrécupérable.
- **Confiance** : la fenêtre de 30 jours offre un point d'équilibre entre sécurité (l'utilisateur ne perd pas définitivement ses données immédiatement) et conformité (la purge effective honore le droit à l'oubli).

### Notification de suppression (MVP)

**Décision produit — validée au MVP** : aucune notification email n'est envoyée au propriétaire au moment de la suppression. La confirmation UI à l'action de suppression tient lieu d'information — c'est suffisant pour signaler à l'utilisateur que le changement a eu lieu. La fenêtre de rétention de 30 jours et la possibilité de restauration restent inchangées, mais ne sont pas relancées par email.

**Justification** : simplifier le MVP en évitant une dépendance de la saga vers un service d'email broker. La rétroaction UI est immédiate et fiable. L'utilisateur peut explorer sa corbeille et procéder à une restauration s'il le souhaite, sans attendre une notification asynchrone.

**Réévaluation post-MVP** : ce choix peut être revisité si les métriques d'utilisation ou les retours utilisateur indiquent qu'une notification proactive améliore significativement la compréhension de la fenêtre de 30 jours ou réduit les tickets de support (« Comment récupérer mon espace supprimé ? »). À évaluer lors d'une planification post-MVP.

---

## Points à trancher en J2

- **Interface de restauration** : comment l'utilisateur accède-t-il à sa corbeille et restaure-t-il un espace ? (Un onglet spécifique ? Une endpoint API dédiée ?)
- **Détail de la cascade** : **Statué dans [ADR-011](ADR-011-cascade-integrite-referentielle.md)** — mécanisme saga applicative, toutes FK en `ON DELETE RESTRICT`, séquences de déliaison et ordre topologique de DELETE complets (matrice 38 FK, deux cycles traités, `source_document_id` cross-espace, préséance des sagas `SpaceDeleted` et `UserAnonymized`).
- **Mécanisme du job de purge** : **Statué dans ADR-011** — Hosted Service .NET, claim exclusif par espace, transaction unique par espace, idempotence garantie.

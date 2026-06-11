# ADR-010 — Suppression de campagne (soft-delete + purge + saga)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : C-09, C-10 (connexe)

---

## Contexte

Le propriétaire d'une campagne a le droit de supprimer sa campagne. Cependant, le modèle de domaine `Campaign` ne propose que la méthode `Archive()` — aucune méthode `Delete()` n'existe. Aucun événement `CampaignDeleted` n'est défini, et il n'y a pas de stratégie de cascade pour effacer ou dissocier les données rattachées (documents, dossiers, memberships, sessions, guest accesses).

Cette lacune impacte plusieurs couches :
- **Domaine** : l'invariant « le propriétaire peut supprimer sa campagne » est inexprimable.
- **RGPD** : l'utilisateur a le droit à l'effacement de ses données ; la suppression de campagne est un cas majeur (cf. ADR-007 pseudonymisation et ADR-009 FK `ownerId`).
- **Cascade** : les données possédées par la campagne doivent être traitées de manière cohérente lors de sa suppression.

La distinction entre `Archive()` (mettre de côté, réversible) et `Delete()` (suppression, éventuellement irréversible) doit être clarifiée avant la Vague 1.

---

## Décision

**Soft-delete + purge à 30 jours + saga `CampaignDeleted`.**

### 1. Soft-delete — introduction d'une corbeille

Une campagne supprimée est marquée comme supprimée (statut ou timestamp `deleted_at`) **sans effacement physique immédiat**. La campagne reste en base et est logiquement invisible aux utilisateurs (les requêtes par défaut l'excluent). Un utilisateur peut **récupérer la campagne dans une fenêtre de rétention de 30 jours** après la suppression.

**Distinction claire** :
- `Archive()` : la campagne est mise de côté (statut `Archived`). Elle reste pleinement retrievable par l'interface d'archivage. Réversible (peut être restaurée).
- `Delete()` : la campagne passe en corbeille (statut `Deleted` ou timestamp `deleted_at` défini). Elle n'est pas visible aux utilisateurs standard. Récupérable pendant 30 jours, puis **purge physique**.

### 2. Purge physique à 30 jours

Passé 30 jours en état supprimé, la campagne et toutes ses données rattachées sont **physiquement supprimées** (hard-delete). Un **job de purge** exécuté régulièrement (quotidien ou hebdomadaire) identifie les campagnes avec `deleted_at ≤ now() - 30 days` et les supprime ainsi que leurs données dépendantes.

### 3. Saga `CampaignDeleted` — orchestration de la cascade

La suppression d'une campagne émet un événement `CampaignDeleted(campaignId, ownerId)`. Une **saga** gère la propagation de la suppression à travers les agrégats et contextes concernés :

- **Content Library** : suppression de tous les documents et dossiers rattachés à la campagne.
- **Session Conduct** : suppression de toutes les sessions rattachées à la campagne.
- **Campaign Management** : suppression de toutes les memberships (records dans `campaign_memberships`, `membership_characters`, etc.) et de tous les guest accesses à la campagne, puis suppression de la campagne elle-même.

La saga garantit la **cohérence transactionnelle** de la cascade : soit tous les composants sont supprimés, soit la suppression est entièrement annulée (compensating transactions en cas d'erreur).

---

## Alternatives considérées

### Hard-delete immédiat (rejeté)

Supprimer physiquement la campagne dès que l'utilisateur déclenche la suppression. Avantages : simplicité, pas de purgeur de fond, libération immédiate de l'espace disque. **Inconvénients décisifs** :
- **Perte de données irréversible** : aucune fenêtre de récupération en cas de suppression accidentelle.
- **Tension avec l'intégrité référentielle** : la FK `CAMPAIGN.ownerId → USER` (ADR-009) empêche le hard-delete de la ligne `users` tant qu'une campagne la référence. Hard-deleter la campagne immédiatement crée un risque asynchrone et une gestion complexe des contraintes.
- **Conflit RGPD** : si le propriétaire supprime son compte AND la campagne, la suppression du compte ne peut pas purger la ligne `users` tant qu'une campagne non-supprimée la référence. L'immédiateté du hard-delete amplifie ce problème.
- **Pas aligné avec la posture de pseudonymisation** (ADR-007) : la conception adopte un modèle de **non-destruction initiale** (pseudonymisation/anonymisation au lieu de hard-delete) ; le hard-delete de campagne serait une exception contraire au modèle.

**Conclusion** : hard-delete immédiat est incompatible avec la stratégie RGPD et l'intégrité référentielle retenue.

### Archivage seul — pas de méthode `Delete()` (rejeté)

Utiliser uniquement `Archive()` et ne pas implémenter `Delete()`. Les campagnes archivées peuvent être restaurées ultérieurement. **Inconvénients décisifs** :
- **Pas de suppression réelle** : le propriétaire demande à supprimer sa campagne ; l'archivage ne satisfait pas ce droit. Archiver ≠ supprimer.
- **Confusion sémantique** : archiver une campagne (« je pause cette campagne ») est distinct de la supprimer (« je supprime définitivement cette campagne »). Les utiliser de manière interchangeable crée une mauvaise expérience utilisateur.
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

- La décision implique que le modèle de domaine doit exposer une méthode `Campaign.Delete()` qui émet l'événement `CampaignDeleted`.
- Elle nécessite l'ajout d'un attribut `deleted_at : DateTime?` (nullable) ou d'un statut `Status` avec valeur `Deleted` au modèle `Campaign`. Les requêtes de lecture par défaut filtreront `WHERE deleted_at IS NULL` (ou statut ≠ `Deleted`).
- Dissociation claire entre `Archive()` et `Delete()` dans les contrats et la documentation du domaine.
- **L'implémentation complète est prévue en Vague 1** (hors périmètre du lot P0.5).

### Schéma et MLD

- La table `campaigns` (et les tables rattachées) porte un champ `deleted_at TIMESTAMP NULL` ou une colonne de statut.
- Les index sur les requêtes d'énumération (ex. `campaigns WHERE owner_id = ? AND deleted_at IS NULL`) doivent être correctement positionnés pour la performance.
- Un **job de purge** est programmé (ex. toutes les 24 h, ou quotidiennement) pour identifier et supprimer les campagnes avec `deleted_at ≤ now() - INTERVAL '30 days'`.

### Saga et événements

- L'événement `CampaignDeleted(campaignId, ownerId, deletedAt)` est défini.
- Une **saga orchestratrice** écoute cet événement et propage la suppression en cascade sur les agrégats dépendants (documents, dossiers, memberships, guest accesses, sessions, personnages).
- Les compensating transactions définissent le comportement en cas d'erreur partielle (ex. si la suppression d'une membership échoue, la saga doit annuler les suppressions antérieures).

### Interaction avec d'autres décisions

- **ADR-007 (Pseudonymisation RGPD)** : la suppression d'une campagne est un cas d'usage majeur du droit à l'effacement. La saga garantit que toutes les données associées à la campagne et au propriétaire sont traitées de manière cohérente (suppression ou pseudonymisation selon le contexte).
- **ADR-009 (FK `ownerId`)** : la suppression de campagne ne supprime pas le compte utilisateur du propriétaire. Les deux cycles de vie sont distincts : une campagne peut être supprimée sans que le compte soit supprimé, et réciproquement. La FK `campaigns.owner_id` reste valide pendant la fenêtre de soft-delete (30 j) et est supprimée avec la ligne `campaigns` lors de la purge physique. Le mécanisme et l'ordre topologique complet de la purge sont définis dans **[ADR-011](ADR-011-cascade-integrite-referentielle.md)**.
- **Politique de rétention** : la corbeille 30 jours pour les campagnes s'aligne avec la fenêtre de rétention générale pour les données supprimées, cohérente avec les findings C-09 et C-10.

### Observation d'un utilisateur

- **Suppression accidentelle** : un utilisateur peut **restaurer** sa campagne depuis la corbeille pendant 30 jours après la suppression (interface à déterminer). Passé ce délai, la campagne est irrécupérable.
- **Confiance** : la fenêtre de 30 jours offre un point d'équilibre entre sécurité (l'utilisateur ne perd pas définitivement ses données immédiatement) et conformité (la purge effective honore le droit à l'oubli).

---

## Points à trancher en Vague 1

- **Interface de restauration** : comment l'utilisateur accède-t-il à sa corbeille et restaure-t-il une campagne ? (Un onglet spécifique ? Une endpoint API dédiée ?)
- **Notification lors de la suppression** : envoyer un email au propriétaire pour confirmer la suppression et lui signaler la fenêtre de 30 jours de récupération ?
- **Détail de la cascade** : **Statué dans [ADR-011](ADR-011-cascade-integrite-referentielle.md)** — mécanisme saga applicative, toutes FK en `ON DELETE RESTRICT`, séquences de déliaison et ordre topologique de DELETE complets (matrice ~30 FK, deux cycles traités, `source_document_id` cross-campagne, préséance des sagas `CampaignDeleted` et `UserAnonymized`).
- **Mécanisme du job de purge** : **Statué dans ADR-011** — Hosted Service .NET, claim exclusif par campagne, transaction unique par campagne, idempotence garantie.

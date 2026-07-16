# ADR-011 — Cascade & intégrité référentielle (sagas `SpaceDeleted` et `UserAnonymized`)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session de cadrage P1)
- **Findings liés** : finding STRUCTURAL (plan 20260609-conception-mvp, graphe de cascade non spécifié), C-09, C-04, F-08, B-05

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03, audit conception pure 2026-06.)*

> *Annotation du 2026-06-12 — deux natures distinctes : (1) renommage `Campaign→Space` (à confirmer à l'entrée en build, ADR-018) ; (2) règle nouvelle : purge inconditionnelle de l'espace `PERSONAL` à `UserDeleted` sous invariant de reprise (claim/idempotence/reclaim — voir §Exception PERSONAL dans `UserAnonymized` et §Saga-événements).*

---

## Contexte

ADR-010 a acté le soft-delete + purge J+30 et introduit la saga `SpaceDeleted`, mais n'a pas spécifié le mécanisme de cascade ni l'ordre d'opérations. Le graphe de FK du MLD actuel compte **38 FK** (35 FK initiales + 3 FK **ajoutées nettes** à l'occasion de cette décision — voir §5 ci-dessous pour le détail ajoutées vs. reprécisées) dont plusieurs forment des cycles ou traversent les frontières de bounded contexts.

Trois questions restaient ouvertes au terme d'ADR-010 :

1. **Mécanisme** : cascade SQL déclarative (`ON DELETE CASCADE`) ou saga applicative ?
2. **Cycles** : le graphe contient deux cycles référentiels. Comment les casser sans violer les contraintes `NOT NULL` ?
3. **Préséance** : la saga `UserAnonymized` (effacement de compte) et la saga `SpaceDeleted` (purge J+30) peuvent concerner les mêmes lignes. Laquelle prime ?

Par ailleurs, l'ADR-009 affirmait que `spaces.owner_id` était « la seule FK cross-module » — affirmation corrigée ici (17 FK cross-module recensées au total — 16 pures « Cross » + 1 mixte « Intra+Cross » —, voir §5 pour le détail de cette reprécision).

---

## Décision

### 1. Mécanisme de cascade : saga applicative, toutes les FK en `ON DELETE RESTRICT`

Toutes les clés étrangères sont déclarées `ON DELETE RESTRICT`. Le SGBD ne cascade jamais : il constitue un filet de sécurité qui refuse toute suppression d'une ligne encore référencée. La progression des suppressions est entièrement pilotée par la saga applicative, dans l'ordre topologique décrit à la section Conséquences.

**Motif du rejet de `ON DELETE CASCADE` SQL** : la cascade SQL s'exécute dans l'ordre d'évaluation du SGBD, sans visibilité sur les cycles et sans possibilité d'intercaler des étapes de déliaison. Sur un graphe aussi dense (38 FK, deux cycles, FK cross-module), la cascade SQL produirait soit une erreur de cycle, soit des suppressions silencieuses dans un ordre non auditable. La saga applicative rend le séquençage explicite, testable et auditable.

### 2. Règle générale de déliaison (absorbeur de cycles futurs)

La règle suivante s'applique à tout cycle détecté dans le graphe, présent ou futur :

> **On ne NULL-e que les arêtes de cycle portées par une FK nullable. Les FK NOT NULL ne sont jamais déliées : elles imposent l'ordre de DELETE (enfant avant parent).**

Cette règle est stable : un futur cycle peut être absorbé sans rouvrir cet ADR, à condition d'identifier de quel côté se trouve la FK nullable.

### 3. Deux cycles dans le graphe — traitement

**Cycle 1 — `documents` ⇄ `guest_accesses`**

- `documents.guest_access_id` (nullable) → `guest_accesses.id`
- `guest_accesses.character_id` (nullable) → `documents.id`

Rompu en passe 1 en NULL-ant les deux colonnes nullable sur le périmètre de l'espace purgé.

**Cycle 2 — `documents` ⇄ `folders`**

- `documents.folder_id` (**NOT NULL**) → `folders.id`
- `folders.default_template_document_id` (nullable) → `documents.id`

La colonne NOT NULL ne peut pas être déliée. On rompt le cycle par le côté nullable : `folders.default_template_document_id := NULL` en passe 1. Ensuite l'ordre de DELETE est `documents` avant `folders` (enfant avant parent).

### 4. `source_document_id` cross-espace

Un document peut être instancié depuis un document source appartenant à un autre espace. La relation `documents.source_document_id` est nullable. À la purge d'un espace E :

- Les `documents.source_document_id` **sortants** (documents de E qui pointent vers un autre espace) sont NULL-és en passe 1 avec les autres nullable.
- Les `documents.source_document_id` **entrants depuis d'autres espaces** (documents hors E qui pointent vers un document de E) sont également NULL-és en passe 1, avant que les documents de E soient supprimés. La provenance est perdue ; le document instancié (copie) survit dans son contexte d'origine.

### 5. FK ajoutées ou précisées au recensement

À l'occasion de l'analyse du graphe, **cinq FK sont touchées par le recensement**, de deux natures distinctes qu'il convient de ne pas confondre :

- **3 FK ajoutées (net)** : elles n'existaient pas parmi les 35 FK initiales. Leur ajout porte le total du graphe de cascade de 35 à **38**.
- **2 FK pré-existantes reprécisées** : elles faisaient déjà partie des 35 FK initiales et sont **déjà comptées** parmi les **17 FK cross-module** de la matrice (16 pures « Cross » + 1 mixte « Intra+Cross ») ; elles n'étaient simplement pas annotées comme cross-module dans le MLD avant cette décision. Leur reprécision corrige cette annotation MLD, sans créer de delta dans le décompte cross-module : ces deux FK sont déjà comprises dans les dix-sept, et n'augmente pas le total de 38, ces FK y étant déjà comptées depuis l'origine.

**FK ajoutées (3) — net-nouvelles, portent le total de 35 à 38 :**

- `documents.document_type_id` → `document_types.id` (nullable, intra-module CL→CL) — content-library.md
- `folders.default_document_type_id` → `document_types.id` (nullable, intra-module CL→CL) — content-library.md
- `membership_characters.(space_id, user_id)` → `space_memberships.(space_id, user_id)` (composite NOT NULL, intra-module SM→SM) — space-management.md

Ces trois FK sont intra-module : leur ajout est sans effet sur le décompte cross-module. Elles sont la seule source de la variation du total du graphe de cascade (35 → **38 FK**).

**FK pré-existantes reprécisées (2) — cross-module, sans effet sur le total :**

- `session_view_configs.space_id` → `spaces.id` (NOT NULL)
- `document_types.space_id` → `spaces.id` (NOT NULL, pour les types *custom* uniquement ; les types *système* sont globaux, `space_id` NULL)

Ces deux FK portent la racine du graphe de cascade vers `spaces` et existaient déjà parmi les 35 FK initiales : elles ne sont pas ajoutées au graphe, seulement **reprécisées** dans le MLD (elles n'y étaient pas annotées comme cross-module). Ces deux FK sont **déjà comptées** parmi les **17 FK cross-module** de la matrice ci-dessous (chacune y porte l'annotation `Cross`) : la reprécision corrige uniquement l'annotation du MLD, qui était en retard sur la matrice — sans créer de delta dans le décompte cross-module, et **sans augmenter le total de 38**, ces deux FK y étant déjà comptées depuis l'origine.

**Bilan** : 3 FK ajoutées (net, → total 38) + 2 FK reprécisées (déjà comptées parmi les 17 FK cross-module, correction d'annotation MLD sans effet sur ce sous-total) = les cinq FK touchées par le recensement. Le graphe de cascade total atteint **38 FK** (35 initiales + 3 ajoutées ; les 2 reprécisées n'y contribuent pas, elles y étaient déjà comptées) — compte exact, vérifiable depuis la matrice ci-dessous, qui recense **17 FK cross-module** (16 pures « Cross » + 1 mixte « Intra+Cross » : `documents.source_document_id`).

---

## Alternatives considérées

**`ON DELETE CASCADE` SQL déclaratif**
Rejeté. Voir §1. Non auditable sur un graphe avec cycles et FK cross-module.

**`ON DELETE SET NULL` câblé en base pour les nullable**
Rejeté. Rendrait le comportement implicite et difficile à tester. La déliaison doit rester dans la saga pour conserver la traçabilité des opérations (logs, idempotence, tests unitaires).

**Suppression unitaire d'entité sans mini-saga**
Rejeté. Toute suppression d'une entité encore référencée (ex. un personnage lié à un membership) doit passer par une mini-saga de déliaison avant le DELETE, par cohérence avec le mécanisme retenu.

---

## Conséquences

### Domaine

- Le domaine émet `SpaceDeleted(spaceId, ownerId, deletedAt)` au moment du soft-delete (ADR-010) ; la saga de purge se déclenche sur le critère `deleted_at ≤ now() - 30 jours`.
- La suppression de toute entité encore référencée passe par une mini-saga de déliaison ; aucun `SET NULL` ou cascade n'est configuré en base.
- `Space.Delete()` n'implique pas de hard-delete immédiat ; le hard-delete physique est exclusivement le fait du job de purge J+30.

### Schéma et MLD

**Matrice des FK (38 FK, toutes `ON DELETE RESTRICT`)**

| Table.colonne | Cible | Nullable | Intra / Cross | Rôle | Action passe 1 |
|---|---|---|---|---|---|
| `spaces.owner_id` | `users.id` | NOT NULL | Cross (I&A) | Propriétaire | — (UserAnonymized, pas purge) |
| `folders.space_id` | `spaces.id` | NOT NULL | Intra (SM) | Racine purge | — (DELETE passe 2) |
| `documents.space_id` | `spaces.id` | NOT NULL | Intra (CL) | Racine purge | — (DELETE passe 2) |
| `sessions.space_id` | `spaces.id` | NOT NULL | Intra (SC) | Racine purge | — (DELETE passe 2) |
| `invitations.space_id` | `spaces.id` | NOT NULL | Intra (SM) | Racine purge | — (DELETE passe 2) |
| `guest_accesses.space_id` | `spaces.id` | NOT NULL | Intra (SM) | Racine purge | — (DELETE passe 2) |
| `space_memberships.space_id` | `spaces.id` | NOT NULL | Intra (SM) | Racine purge | — (DELETE passe 2) |
| `session_view_configs.space_id` | `spaces.id` | NOT NULL | Cross (SC→SM) | Racine purge | — (DELETE passe 2) |
| `document_types.space_id` | `spaces.id` | NOT NULL | Cross (CL→SM) | Racine purge, types custom | — (DELETE passe 2) |
| `folders.parent_folder_id` | `folders.id` | nullable | Intra (CL) | Auto-référence arborescence | NULL en passe 1 |
| `folders.default_template_document_id` | `documents.id` | nullable | Intra (CL) | Cycle 2 | **NULL en passe 1** |
| `folders.default_document_type_id` | `document_types.id` | nullable | Intra (CL) | Type par défaut du dossier | — (DELETE passe 2 : folders avant document_types) |
| `documents.folder_id` | `folders.id` | NOT NULL | Intra (CL) | Cycle 2, enfant | — (DELETE passe 2 : docs avant folders) |
| `documents.document_type_id` | `document_types.id` | nullable | Intra (CL) | Type du document | — (DELETE passe 2 : documents avant document_types) |
| `documents.source_document_id` | `documents.id` | nullable | Intra+Cross (CL) | Instanciation/héritage | NULL en passe 1 (intra + entrants cross-espace) |
| `documents.character_id` | `documents.id` | nullable | Intra (CL) | LIVE_NOTE → player_character | NULL en passe 1 |
| `documents.guest_access_id` | `guest_accesses.id` | nullable | Cross (CL→SM) | Cycle 1 | **NULL en passe 1** |
| `document_blocks.document_id` | `documents.id` | NOT NULL | Intra (CL) | Contenu structuré | — (DELETE passe 2) |
| `document_links.source_document_id` | `documents.id` | NOT NULL | Intra (CL) | Lien sortant | — (DELETE passe 2) |
| `document_links.target_document_id` | `documents.id` | NOT NULL | Intra (CL) | Lien entrant | — (DELETE passe 2) |
| `document_tags.document_id` | `documents.id` | NOT NULL | Intra (CL) | Tag | — (DELETE passe 2) |
| `membership_characters.space_id` | `spaces.id` | NOT NULL | Intra (SM) | Liaison membership-personnage | — (DELETE passe 2) |
| `membership_characters.character_id` | `documents.id` | NOT NULL | Cross (SM→CL) | Personnage joueur | — (DELETE passe 2) |
| `membership_characters.(space_id, user_id)` | `space_memberships.(space_id, user_id)` | NOT NULL (composite) | Intra (SM) | Intégrité membership-personnage | — (DELETE passe 2 : membership_characters avant space_memberships) |
| `guest_accesses.character_id` | `documents.id` | nullable | Cross (SM→CL) | Cycle 1 | **NULL en passe 1** |
| `guest_accesses.session_id` | `sessions.id` | nullable | Cross (SM→SC) | Accès invité → session | NULL en passe 1 |
| `invitations.session_id` | `sessions.id` | nullable | Cross (SM→SC) | Invitation → session | NULL en passe 1 |
| `sessions.scenario_id` | `documents.id` | nullable | Cross (SC→CL) | Scénario lié | NULL en passe 1 |
| `session_pinned_documents.session_id` | `sessions.id` | NOT NULL | Intra (SC) | Document épinglé | — (DELETE passe 2) |
| `session_pinned_documents.document_id` | `documents.id` | NOT NULL | Cross (SC→CL) | Document épinglé | — (DELETE passe 2) |
| `session_live_notes.session_id` | `sessions.id` | NOT NULL | Intra (SC) | Note live | — (DELETE passe 2) |
| `session_live_notes.document_id` | `documents.id` | NOT NULL | Cross (SC→CL) | Note live → document | — (DELETE passe 2) |
| `session_view_folders.folder_id` | `folders.id` | NOT NULL | Cross (SC→CL) | Vue dossier | — (DELETE passe 2) |
| `session_view_folders.session_view_config_id` | `session_view_configs.id` | NOT NULL | Intra (SC) | Vue config | — (DELETE passe 2) |
| `space_memberships.user_id` | `users.id` | NOT NULL | Cross (SM→I&A) | Membre | — (UserAnonymized, pas purge) |
| `folders.created_by_id` | `users.id` | NOT NULL | Cross (CL→I&A) | Créateur | — (UserAnonymized, pas purge) |
| `documents.created_by_id` | `users.id` | NOT NULL | Cross (CL→I&A) | Créateur | — (UserAnonymized, pas purge) |
| `sessions.created_by_id` | `users.id` | NOT NULL | Cross (SC→I&A) | Créateur | — (UserAnonymized, pas purge) |

*Légende modules : SM = Space Management, CL = Content Library, SC = Session Conduct, I&A = Identity & Access.*

**Invariant de visibilité du soft-delete** : la visibilité des données d'un espace en corbeille est portée par jointure sur `spaces.deleted_at IS NOT NULL`. Aucune table enfant ne porte son propre flag `deleted_at`. Cet invariant s'applique à **tous les chemins de lecture** : requêtes d'énumération (P4, P5), lectures directes par ID (type `GET /documents/{id}`), queries SignalR, et projections CQRS. Une lecture par ID qui ne joint pas ou ne filtre pas `spaces.deleted_at` exposerait les données d'un espace en corbeille.

### Séquences de saga

#### Saga `SpaceDeleted` — purge J+30

**Déclencheur** : Hosted Service .NET sélectionne `spaces WHERE deleted_at ≤ now() - 30 jours AND purge_claimed_at IS NULL`. Un claim exclusif (`purge_claimed_at := now()`) est posé **avant** d'entrer dans la transaction de purge. Un espace claimé ne peut plus être restauré (le claim est irréversible).

**Idempotence** : la déliaison est idempotente (NULL-er une colonne déjà NULL = no-op). En cas de crash partiel, la reprise reprend depuis le début de la transaction ; les passes de déliaison sont rejouables sans effet de bord.

**Expiration du claim** : un claim posé (`purge_claimed_at IS NOT NULL`) est considéré **expiré** si la purge n'est pas terminée dans un délai maximal (à définir en B1 — ex. 1 heure). Un espace dont le claim a expiré sans purge terminée redevient **reclaimable** : le Hosted Service doit sélectionner également les espaces avec `purge_claimed_at ≤ now() - <seuil>` sans confirmation de purge terminée. Sans cette mécanique d'expiration, un crash survenant entre le claim et l'ouverture de la transaction laisserait l'espace définitivement bloqué, avec des données conservées au-delà du délai légal (manquement potentiel à l'Art. 17 RGPD). Le claim reste exclusif vis-à-vis des claims concurrents et de la restauration — il n'est pas définitivement bloquant en cas de crash.

**Transaction** : une transaction unique par espace couvre les deux passes. Si la transaction échoue, aucune ligne n'est supprimée.

---

**Passe 1 — déliaison** (NULL des FK nullable, dans n'importe quel ordre)

Sur le périmètre de l'espace E (filtre `space_id = E` ou jointure transitoire) :

1. `UPDATE documents SET guest_access_id = NULL WHERE space_id = E` — casse cycle 1 côté documents
2. `UPDATE guest_accesses SET character_id = NULL WHERE space_id = E` — casse cycle 1 côté guest_accesses
3. `UPDATE folders SET default_template_document_id = NULL WHERE space_id = E` — casse cycle 2
4. `UPDATE documents SET source_document_id = NULL WHERE space_id = E` — intra (auto-référence)
5. `UPDATE documents SET source_document_id = NULL WHERE source_document_id IN (SELECT id FROM documents WHERE space_id = E)` — entrants cross-espace (documents d'autres espaces pointant vers un document de E)
6. `UPDATE documents SET character_id = NULL WHERE space_id = E` — LIVE_NOTE → player_character
7. `UPDATE sessions SET scenario_id = NULL WHERE space_id = E`
8. `UPDATE guest_accesses SET session_id = NULL WHERE space_id = E`
9. `UPDATE invitations SET session_id = NULL WHERE space_id = E`
10. `UPDATE folders SET parent_folder_id = NULL WHERE space_id = E` — aplatit l'arborescence pour DELETE

---

**Passe 2 — DELETE topologique** (feuilles avant racines)

```
document_links            (source_document_id NOT NULL, target_document_id NOT NULL)
document_tags             (document_id NOT NULL)
document_blocks           (document_id NOT NULL)
session_view_folders      (folder_id NOT NULL, session_view_config_id NOT NULL)
session_view_configs      (space_id NOT NULL)
session_pinned_documents  (session_id NOT NULL, document_id NOT NULL)
session_live_notes        (session_id NOT NULL, document_id NOT NULL)
sessions                  (space_id NOT NULL)
membership_characters     (space_id NOT NULL, character_id NOT NULL)  ← avant space_memberships (FK composite)
space_memberships         (space_id NOT NULL, user_id NOT NULL)
invitations               (space_id NOT NULL)
guest_accesses            (space_id NOT NULL)
documents                 (space_id NOT NULL)  ← avant folders ET avant document_types (documents.document_type_id nullable → pas de blocage, mais l'ordre garantit que document_types existe au DELETE des documents)
folders                   (space_id NOT NULL)  ← enfants avant parents (par profondeur DESC), puis racines ; après documents (documents.folder_id NOT NULL)
document_types            (space_id NOT NULL, custom uniquement)  ← après documents ET folders (folders.default_document_type_id et documents.document_type_id les référencent, nullable) ; avant spaces
spaces                    (racine)
```

**Justification de la position de `document_types`** : `folders.default_document_type_id` et `documents.document_type_id` sont des FK nullable vers `document_types`. Comme elles sont nullable, aucune passe 1 de déliaison n'est nécessaire — la passe 1 ne NULL-e que ce qui est requis pour casser des cycles ou satisfaire des NOT NULL bloquants. En revanche, `document_types` doit être supprimé **après** `documents` et `folders` (qui le référencent), car le SGBD est en `ON DELETE RESTRICT` : tenter de supprimer un `document_types` encore référencé par un `documents` ou `folders` existant déclencherait une erreur RESTRICT. L'ordre `documents → folders → document_types` suffit à éviter ce blocage. Les `document_types` système (`space_id IS NULL`) ne sont pas touchés.

---

#### Saga `UserAnonymized` — effacement de compte (espace vivant)

**Filtre préalable** : s'abstient sur les espaces en corbeille (`deleted_at IS NOT NULL`). Seules les lignes appartenant à des espaces actifs ou archivés sont traitées.

**Ordre impératif** (conformément à ADR-007) :

1. Réécriture `asp_net_users.password_hash` + `security_stamp` **avant** `users.Anonymize()`.
2. `users.email := 'deleted-{id}@haversack.invalid'`, `display_name := '[Compte supprimé]'`, `status := 'DELETED'`. L'`id` est conservé.
3. Les FK vers `users.id` (`spaces.owner_id`, `space_memberships.user_id`, `folders.created_by_id`, `documents.created_by_id`, `sessions.created_by_id`) restent **intactes** : l'identifiant survit anonymisé, les contraintes NOT NULL sont satisfaites.
4. Les `guest_accesses` d'espaces actifs (espace vivant) : `display_name` n'est pas anonymisé ici — les données d'invité sur un espace vivant relèvent de **B1.6** (base légale, critère de déclenchement et sort des données invité sur demande individuelle, voir *Points à trancher*). À la **purge d'espace** (J+30), les `guest_accesses` sont hard-deletés en passe 2 (le `display_name` est supprimé avec la ligne — couvert par la saga `SpaceDeleted`, pas par `UserAnonymized`).
5. (Option Art. 17) DELETE des documents sélectionnés selon la politique B1.4, via déliaison-puis-delete. Le périmètre de visibilité supprimable est défini en **B1.4** (hors de cet ADR).
6. **Purge des logs de corrélation** : purge ou anonymisation des entrées de logs applicatifs et d'infrastructure corrélant `UserId` ↔ email d'origine (IP de connexion, logs d'authentification, logs d'audit). Cette étape doit être exécutée dans la fenêtre de traitement Art. 12§3. Politique détaillée dans **ADR-012 §5**.

**MJ propriétaire effaçant son compte sur un espace vivant** : l'espace reste possédé par l'`id` anonymisé (`owner_id NOT NULL` satisfait). Le transfert de propriété forcé est post-MVP (renvoyé à B1.6/UC-11).

**Exception — espace `PERSONAL` à `UserDeleted`** : l'espace `PERSONAL` du propriétaire qui s'efface est **purgé inconditionnellement** (hard-delete immédiat, sans attendre J+30). Il n'est **pas** conservé sous identité anonymisée — ce comportement déroge à la règle générale « espace conservé sous `id` anonymisé » car l'unique membre est le propriétaire lui-même. La saga `SpaceDeleted` s'applique intégralement à l'espace `PERSONAL` purgé (mêmes passes 1 et 2).

**Mécanisme additionnel — consommation du jeu matérialisé et relocation** : la purge du contenu PERSONAL consomme le **jeu de sélection matérialisé** capturé à `deletion_requested_at` (ADR-012 §7) comme source de périmètre — les passes 1 et 2 de `SpaceDeleted` ne recalculent pas depuis le `space_id` ou la `visibility` courants du document. Un **step de relocation obligatoire** s'exécute **avant** la purge physique : tout document marqué `conserve` dans le jeu matérialisé mais physiquement situé dans l'espace PERSONAL au moment de l'exécution est **réaffecté hors de cet espace** (réaffectation d'appartenance + réadressage des FK scopées à l'espace), prévenant ainsi une purge silencieuse d'un document devant être conservé. Mécanisme complet et justification en ADR-012 §7.

La politique de sélection de la catégorie hard-delete PERSONAL est définie dans **ADR-012**.

La purge immédiate de l'espace `PERSONAL` déclenchée par `UserDeleted` s'exécute **sous le même invariant de claim / idempotence / reprise** que la purge J+30 (le déclencheur change — `UserDeleted` au lieu du critère temporel — la garantie de reprise après crash ne change pas). Le sélecteur du Hosted Service inclut les espaces `PERSONAL` en attente de purge sur `UserDeleted`, sans la condition `deleted_at ≤ now()-30j` ; un claim posé est reclaimable à l'identique. Sans cet invariant, un crash en cours de purge personnelle laisserait un résidu au-delà du délai Art. 17. Le câblage exact (handler `UserDeleted` vs Hosted Service mutualisé, flag de sélection) est renvoyé à B1.

---

### Matrice de préséance des deux sagas

| Situation | `UserAnonymized` | `SpaceDeleted` (purge) |
|---|---|---|
| Espace actif ou archivé (`deleted_at IS NULL`) | S'exécute normalement | Non déclenchée |
| Espace en corbeille (`deleted_at IS NOT NULL`, purge non claimée) | **S'abstient** sur les lignes de cet espace | Non encore déclenchée (< J+30 ou job pas passé) |
| Espace claimé pour purge (`purge_claimed_at IS NOT NULL`) | **S'abstient** — claim irréversible, purge prime | **Prime** — suppression terminale |
| Purge terminée | Sans objet | Sans objet |
| **Espace `PERSONAL` à `UserDeleted`** | **Déclenche la purge terminale inconditionnellement** — pas d'abstention au prétexte d'un membre actif (l'unique membre est le propriétaire qui s'efface) | **Prime** — déclenchée immédiatement par `UserDeleted`, sans attendre J+30 |

La règle générale : sur toute ligne appartenant à un espace dont `deleted_at IS NOT NULL`, `UserAnonymized` ne touche rien. `SpaceDeleted` purge est terminale et prime sur toute opération concurrente. **Exception espace `PERSONAL`** : `UserDeleted` déclenche directement la purge terminale de l'espace `PERSONAL` — la saga `SpaceDeleted` est invoquée immédiatement, sans passage par la corbeille J+30.

### Saga-événements

- L'événement `SpaceDeleted` est émis au moment du soft-delete (ADR-010) ; la saga de purge physique est déclenchée par le Hosted Service sur critère temporel, pas par un second événement. **Exception espace `PERSONAL`** : l'événement `UserDeleted` déclenche directement la purge physique de l'espace `PERSONAL` sans passage par le soft-delete J+30.
- Le claim de purge (`purge_claimed_at`) est posé hors transaction (avant l'ouverture de la transaction de purge) pour garantir l'exclusivité.
- La transaction unique par espace couvre l'intégralité des deux passes. Sur base unique partagée (MVP monolithe), il n'y a pas de compensating transactions : en cas d'échec, la transaction est rollbackée entièrement et l'espace reste en corbeille. Les compensating transactions mentionnées dans ADR-010 sont réservées à l'architecture multi-service future.

### Contenu LIVE_NOTE et médias externalisés

**Décision MVP : aucun blob/média externalisé**

Le contenu textuel des documents (y compris les LIVE_NOTE) est stocké dans `document_blocks` en base de données **en tant que contenu inline** dans la colonne `jsonb` `content`, et est couvert par la purge passe 2 (`DELETE document_blocks WHERE document_id IN ...`). **Au MVP, aucun blob ou média (image, fichier, carte) n'est externalisé** : tout contenu média associé à un document est stocké inline dans `document_blocks.content`, supprimé en passe 2 avec la ligne parente. Aucun blob orphelin possible tant que le contenu reste inline.

**Debt dormante (B1.9) — câblage média externalisé**

Si des médias ou blobs associés à des documents sont externalisés dans une version future (chaîne média B1.9 — stockage objet S3 ou équivalent, uploads futurs : cartes, portraits PNJ), leur purge doit être **câblée à la saga `SpaceDeleted`** : un step supplémentaire après la passe 2 SQL doit déclencher la suppression des objets dans le store externe. Ce câblage s'applique aux deux chemins de purge : la purge J+30 ordinaire et la purge immédiate de l'espace `PERSONAL` déclenchée par `UserDeleted`. Sans ce câblage sur l'un ou l'autre chemin, les blobs orphelins persistent indéfiniment après la purge des métadonnées.

> **Condition non réalisée au MVP — à réactiver (B1.9) si une chaîne média externalisée est introduite.** L'exigence de câblage de purge aux deux chemins (`SpaceDeleted` J+30 + purge PERSONAL immédiate) demeure explicite dans cet ADR et doit être implémentée dès l'introduction d'un store externe. Voir **B1.9** pour la définition de la chaîne média.

### Interaction avec les autres ADR

**ADR-009 (FK `ownerId`)** : la FK `spaces.owner_id → users.id` est NOT NULL et n'est pas déliée lors de la purge. Elle est supprimée avec la ligne `spaces` en fin de passe 2. L'affirmation « seule FK cross-module » dans ADR-009 est corrigée : 17 FK cross-module sont recensées (voir matrice ci-dessus). ADR-009 reste un précédent de gouvernance valide ; la stratégie `ON DELETE` et le mécanisme de cascade sont définis dans le présent ADR.

**ADR-010 (soft-delete + purge + saga)** : cet ADR complète ADR-010 en spécifiant le mécanisme et l'ordre d'opérations. Les compensating transactions mentionnées dans ADR-010 s'appliquent uniquement à l'extraction future multi-service ; sur le MVP monolithe à base unique, la transaction de base de données joue ce rôle.

**ADR-007 (RGPD pseudonymisation)** : `UserAnonymized` respecte l'ordre impératif défini dans ADR-007 (réécriture ASP.NET Identity avant anonymisation). L'option Art. 17 d'effacement étendu délègue la définition du périmètre supprimable à B1.4.

**ADR-018 (généralisation Campaign→Space)** : la racine de purge est désormais `space_id`. L'espace `PERSONAL` (type `SpaceType.PERSONAL`) est soumis à la même saga `SpaceDeleted` que tout autre espace, avec la règle supplémentaire de purge inconditionnelle à `UserDeleted`. La politique de sélection de la catégorie hard-delete PERSONAL est définie dans ADR-012.

---

## Points à trancher

Les éléments suivants sont explicitement renvoyés à la Vague 1 (lot B1.x) :

- **B1.4 — Périmètre RGPD de l'effacement étendu (option Art. 17)** : **Statué dans [ADR-012](ADR-012-rgpd-effacement-compte.md).** Politique de sélection des documents supprimables (`PLAYER_PRIVATE` et non partagés) et des documents conservés (partagés / `GM_ONLY` sous intérêt légitime Art. 17§3(e)), règle anti-résidu F-08, purge des logs de corrélation UUID↔email, et obligations Art. 12§3 (délai 1 mois, horodatage, notification).
- **B1.6 — Données invité sur espace vivant** : **Statué dans [ADR-013](ADR-013-rgpd-donnees-invites.md).** Base légale retenue pour `guest_accesses.display_name` (intérêt légitime), information Art. 13 au formulaire invité, rétention autonome à 90 jours après `expires_at`, posture mineurs (service non destiné aux enfants, attestation 16+), posture sous-traitant F-13 et DPA, UC-11 (espace conservé sous `id` anonymisé au MVP, transfert de propriété forcé post-MVP — exception : un espace `PERSONAL` n'est pas conservé — purgé inconditionnellement à `UserDeleted`, voir §Exception PERSONAL dans `UserAnonymized`).
- **B1.9 — Chaîne média** : définir si des blobs/médias sont externalisés et câbler leur suppression à la saga `SpaceDeleted` (voir section *Contenu LIVE_NOTE et médias externalisés* ci-dessus).
- **Art. 20 — Portabilité** : périmètre de la portabilité des données utilisateur et espace. Post-MVP.
- **Seuil d'expiration du claim** : valeur concrète du timeout de `purge_claimed_at` (ex. 1 heure) à fixer en B1 lors de l'implémentation du Hosted Service.

---

## Compléments post-revue

*(Section réservée aux clarifications post-implémentation — vide à la date de l'ADR.)*

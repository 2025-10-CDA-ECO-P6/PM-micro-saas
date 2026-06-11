# ADR-011 — Cascade & intégrité référentielle (sagas `CampaignDeleted` et `UserAnonymized`)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session de cadrage P1)
- **Findings liés** : finding STRUCTURAL (plan 20260609-conception-mvp, graphe de cascade non spécifié), C-09, C-04, F-08, B-05

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03, audit conception pure 2026-06.)*

---

## Contexte

ADR-010 a acté le soft-delete + purge J+30 et introduit la saga `CampaignDeleted`, mais n'a pas spécifié le mécanisme de cascade ni l'ordre d'opérations. Le graphe de FK du MLD actuel compte **38 FK** (35 FK initiales + 3 ajoutées à l'occasion de cette décision — voir section Schéma/MLD ci-dessous) dont plusieurs forment des cycles ou traversent les frontières de bounded contexts.

Trois questions restaient ouvertes au terme d'ADR-010 :

1. **Mécanisme** : cascade SQL déclarative (`ON DELETE CASCADE`) ou saga applicative ?
2. **Cycles** : le graphe contient deux cycles référentiels. Comment les casser sans violer les contraintes `NOT NULL` ?
3. **Préséance** : la saga `UserAnonymized` (effacement de compte) et la saga `CampaignDeleted` (purge J+30) peuvent concerner les mêmes lignes. Laquelle prime ?

Par ailleurs, l'ADR-009 affirmait que `campaigns.owner_id` était « la seule FK cross-module » — affirmation corrigée ici (19 FK cross-module recensées au total).

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

Rompu en passe 1 en NULL-ant les deux colonnes nullable sur le périmètre de la campagne purgée.

**Cycle 2 — `documents` ⇄ `folders`**

- `documents.folder_id` (**NOT NULL**) → `folders.id`
- `folders.default_template_document_id` (nullable) → `documents.id`

La colonne NOT NULL ne peut pas être déliée. On rompt le cycle par le côté nullable : `folders.default_template_document_id := NULL` en passe 1. Ensuite l'ordre de DELETE est `documents` avant `folders` (enfant avant parent).

### 4. `source_document_id` cross-campagne

Un document peut être instancié depuis un document source appartenant à une autre campagne. La relation `documents.source_document_id` est nullable. À la purge d'une campagne C :

- Les `documents.source_document_id` **sortants** (documents de C qui pointent vers une autre campagne) sont NULL-és en passe 1 avec les autres nullable.
- Les `documents.source_document_id` **entrants depuis d'autres campagnes** (documents hors C qui pointent vers un document de C) sont également NULL-és en passe 1, avant que les documents de C soient supprimés. La provenance est perdue ; le document instancié (copie) survit dans son contexte d'origine.

### 5. FK ajoutées ou précisées au recensement

À l'occasion de l'analyse du graphe, cinq FK non annotées ou incomplètes dans le MLD sont identifiées comme FK physiques réelles :

**FK cross-module (2) — portent la racine du graphe de cascade vers `campaigns` :**

- `session_view_configs.campaign_id` → `campaigns.id` (NOT NULL)
- `document_types.campaign_id` → `campaigns.id` (NOT NULL, pour les types *custom* uniquement ; les types *système* sont globaux, `campaign_id` NULL)

Ces deux FK cross-module font passer le décompte cross-module de **17 à 19** (ADR-009 en recensait 17 ; ces deux ajouts portent le total à 19, cohérent avec le compte de la matrice ci-dessous).

**FK intra-module (3) — sans effet sur le décompte cross-module :**

- `documents.document_type_id` → `document_types.id` (nullable, intra-module CL→CL) — content-library.md
- `folders.default_document_type_id` → `document_types.id` (nullable, intra-module CL→CL) — content-library.md
- `membership_characters.(campaign_id, user_id)` → `campaign_memberships.(campaign_id, user_id)` (composite NOT NULL, intra-module CM→CM) — campaign-management.md

Ces trois FK intra-module ne modifient pas le décompte cross-module (reste 19). Le graphe de cascade total atteint **38 FK** (compte exact, vérifiable depuis la matrice).

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

- Le domaine émet `CampaignDeleted(campaignId, ownerId, deletedAt)` au moment du soft-delete (ADR-010) ; la saga de purge se déclenche sur le critère `deleted_at ≤ now() - 30 jours`.
- La suppression de toute entité encore référencée passe par une mini-saga de déliaison ; aucun `SET NULL` ou cascade n'est configuré en base.
- `Campaign.Delete()` n'implique pas de hard-delete immédiat ; le hard-delete physique est exclusivement le fait du job de purge J+30.

### Schéma et MLD

**Matrice des FK (38 FK, toutes `ON DELETE RESTRICT`)**

| Table.colonne | Cible | Nullable | Intra / Cross | Rôle | Action passe 1 |
|---|---|---|---|---|---|
| `campaigns.owner_id` | `users.id` | NOT NULL | Cross (I&A) | Propriétaire | — (UserAnonymized, pas purge) |
| `folders.campaign_id` | `campaigns.id` | NOT NULL | Intra (CM) | Racine purge | — (DELETE passe 2) |
| `documents.campaign_id` | `campaigns.id` | NOT NULL | Intra (CL) | Racine purge | — (DELETE passe 2) |
| `sessions.campaign_id` | `campaigns.id` | NOT NULL | Intra (SC) | Racine purge | — (DELETE passe 2) |
| `invitations.campaign_id` | `campaigns.id` | NOT NULL | Intra (CM) | Racine purge | — (DELETE passe 2) |
| `guest_accesses.campaign_id` | `campaigns.id` | NOT NULL | Intra (CM) | Racine purge | — (DELETE passe 2) |
| `campaign_memberships.campaign_id` | `campaigns.id` | NOT NULL | Intra (CM) | Racine purge | — (DELETE passe 2) |
| `session_view_configs.campaign_id` | `campaigns.id` | NOT NULL | Cross (SC→CM) | Racine purge | — (DELETE passe 2) |
| `document_types.campaign_id` | `campaigns.id` | NOT NULL | Cross (CL→CM) | Racine purge, types custom | — (DELETE passe 2) |
| `folders.parent_folder_id` | `folders.id` | nullable | Intra (CL) | Auto-référence arborescence | NULL en passe 1 |
| `folders.default_template_document_id` | `documents.id` | nullable | Intra (CL) | Cycle 2 | **NULL en passe 1** |
| `folders.default_document_type_id` | `document_types.id` | nullable | Intra (CL) | Type par défaut du dossier | — (DELETE passe 2 : folders avant document_types) |
| `documents.folder_id` | `folders.id` | NOT NULL | Intra (CL) | Cycle 2, enfant | — (DELETE passe 2 : docs avant folders) |
| `documents.document_type_id` | `document_types.id` | nullable | Intra (CL) | Type du document | — (DELETE passe 2 : documents avant document_types) |
| `documents.source_document_id` | `documents.id` | nullable | Intra+Cross (CL) | Instanciation/héritage | NULL en passe 1 (intra + entrants cross-campagne) |
| `documents.character_id` | `documents.id` | nullable | Intra (CL) | LIVE_NOTE → player_character | NULL en passe 1 |
| `documents.guest_access_id` | `guest_accesses.id` | nullable | Cross (CL→CM) | Cycle 1 | **NULL en passe 1** |
| `document_blocks.document_id` | `documents.id` | NOT NULL | Intra (CL) | Contenu structuré | — (DELETE passe 2) |
| `document_links.source_document_id` | `documents.id` | NOT NULL | Intra (CL) | Lien sortant | — (DELETE passe 2) |
| `document_links.target_document_id` | `documents.id` | NOT NULL | Intra (CL) | Lien entrant | — (DELETE passe 2) |
| `document_tags.document_id` | `documents.id` | NOT NULL | Intra (CL) | Tag | — (DELETE passe 2) |
| `membership_characters.campaign_id` | `campaigns.id` | NOT NULL | Intra (CM) | Liaison membership-personnage | — (DELETE passe 2) |
| `membership_characters.character_id` | `documents.id` | NOT NULL | Cross (CM→CL) | Personnage joueur | — (DELETE passe 2) |
| `membership_characters.(campaign_id, user_id)` | `campaign_memberships.(campaign_id, user_id)` | NOT NULL (composite) | Intra (CM) | Intégrité membership-personnage | — (DELETE passe 2 : membership_characters avant campaign_memberships) |
| `guest_accesses.character_id` | `documents.id` | nullable | Cross (CM→CL) | Cycle 1 | **NULL en passe 1** |
| `guest_accesses.session_id` | `sessions.id` | nullable | Cross (CM→SC) | Accès invité → session | NULL en passe 1 |
| `invitations.session_id` | `sessions.id` | nullable | Cross (CM→SC) | Invitation → session | NULL en passe 1 |
| `sessions.scenario_id` | `documents.id` | nullable | Cross (SC→CL) | Scénario lié | NULL en passe 1 |
| `session_pinned_documents.session_id` | `sessions.id` | NOT NULL | Intra (SC) | Document épinglé | — (DELETE passe 2) |
| `session_pinned_documents.document_id` | `documents.id` | NOT NULL | Cross (SC→CL) | Document épinglé | — (DELETE passe 2) |
| `session_live_notes.session_id` | `sessions.id` | NOT NULL | Intra (SC) | Note live | — (DELETE passe 2) |
| `session_live_notes.document_id` | `documents.id` | NOT NULL | Cross (SC→CL) | Note live → document | — (DELETE passe 2) |
| `session_view_folders.folder_id` | `folders.id` | NOT NULL | Cross (SC→CL) | Vue dossier | — (DELETE passe 2) |
| `session_view_folders.session_view_config_id` | `session_view_configs.id` | NOT NULL | Intra (SC) | Vue config | — (DELETE passe 2) |
| `campaign_memberships.user_id` | `users.id` | NOT NULL | Cross (CM→I&A) | Membre | — (UserAnonymized, pas purge) |
| `folders.created_by_id` | `users.id` | NOT NULL | Cross (CL→I&A) | Créateur | — (UserAnonymized, pas purge) |
| `documents.created_by_id` | `users.id` | NOT NULL | Cross (CL→I&A) | Créateur | — (UserAnonymized, pas purge) |
| `sessions.created_by_id` | `users.id` | NOT NULL | Cross (SC→I&A) | Créateur | — (UserAnonymized, pas purge) |

*Légende modules : CM = Campaign Management, CL = Content Library, SC = Session Conduct, I&A = Identity & Access.*

**Invariant de visibilité du soft-delete** : la visibilité des données d'une campagne en corbeille est portée par jointure sur `campaigns.deleted_at IS NOT NULL`. Aucune table enfant ne porte son propre flag `deleted_at`. Cet invariant s'applique à **tous les chemins de lecture** : requêtes d'énumération (P4, P5), lectures directes par ID (type `GET /documents/{id}`), queries SignalR, et projections CQRS. Une lecture par ID qui ne joint pas ou ne filtre pas `campaigns.deleted_at` exposerait les données d'une campagne en corbeille.

### Séquences de saga

#### Saga `CampaignDeleted` — purge J+30

**Déclencheur** : Hosted Service .NET sélectionne `campaigns WHERE deleted_at ≤ now() - 30 jours AND purge_claimed_at IS NULL`. Un claim exclusif (`purge_claimed_at := now()`) est posé **avant** d'entrer dans la transaction de purge. Une campagne claimée ne peut plus être restaurée (le claim est irréversible).

**Idempotence** : la déliaison est idempotente (NULL-er une colonne déjà NULL = no-op). En cas de crash partiel, la reprise reprend depuis le début de la transaction ; les passes de déliaison sont rejouables sans effet de bord.

**Expiration du claim** : un claim posé (`purge_claimed_at IS NOT NULL`) est considéré **expiré** si la purge n'est pas terminée dans un délai maximal (à définir en B1 — ex. 1 heure). Une campagne dont le claim a expiré sans purge terminée redevient **reclaimable** : le Hosted Service doit sélectionner également les campagnes avec `purge_claimed_at ≤ now() - <seuil>` sans confirmation de purge terminée. Sans cette mécanique d'expiration, un crash survenant entre le claim et l'ouverture de la transaction laisserait la campagne définitivement bloquée, avec des données conservées au-delà du délai légal (manquement potentiel à l'Art. 17 RGPD). Le claim reste exclusif vis-à-vis des claims concurrents et de la restauration — il n'est pas définitivement bloquant en cas de crash.

**Transaction** : une transaction unique par campagne couvre les deux passes. Si la transaction échoue, aucune ligne n'est supprimée.

---

**Passe 1 — déliaison** (NULL des FK nullable, dans n'importe quel ordre)

Sur le périmètre de la campagne C (filtre `campaign_id = C` ou jointure transitoire) :

1. `UPDATE documents SET guest_access_id = NULL WHERE campaign_id = C` — casse cycle 1 côté documents
2. `UPDATE guest_accesses SET character_id = NULL WHERE campaign_id = C` — casse cycle 1 côté guest_accesses
3. `UPDATE folders SET default_template_document_id = NULL WHERE campaign_id = C` — casse cycle 2
4. `UPDATE documents SET source_document_id = NULL WHERE campaign_id = C` — intra (auto-référence)
5. `UPDATE documents SET source_document_id = NULL WHERE source_document_id IN (SELECT id FROM documents WHERE campaign_id = C)` — entrants cross-campagne (documents d'autres campagnes pointant vers un document de C)
6. `UPDATE documents SET character_id = NULL WHERE campaign_id = C` — LIVE_NOTE → player_character
7. `UPDATE sessions SET scenario_id = NULL WHERE campaign_id = C`
8. `UPDATE guest_accesses SET session_id = NULL WHERE campaign_id = C`
9. `UPDATE invitations SET session_id = NULL WHERE campaign_id = C`
10. `UPDATE folders SET parent_folder_id = NULL WHERE campaign_id = C` — aplatit l'arborescence pour DELETE

---

**Passe 2 — DELETE topologique** (feuilles avant racines)

```
document_links            (source_document_id NOT NULL, target_document_id NOT NULL)
document_tags             (document_id NOT NULL)
document_blocks           (document_id NOT NULL)
session_view_folders      (folder_id NOT NULL, session_view_config_id NOT NULL)
session_view_configs      (campaign_id NOT NULL)
session_pinned_documents  (session_id NOT NULL, document_id NOT NULL)
session_live_notes        (session_id NOT NULL, document_id NOT NULL)
sessions                  (campaign_id NOT NULL)
membership_characters     (campaign_id NOT NULL, character_id NOT NULL)  ← avant campaign_memberships (FK composite)
campaign_memberships      (campaign_id NOT NULL, user_id NOT NULL)
invitations               (campaign_id NOT NULL)
guest_accesses            (campaign_id NOT NULL)
documents                 (campaign_id NOT NULL)  ← avant folders ET avant document_types (documents.document_type_id nullable → pas de blocage, mais l'ordre garantit que document_types existe au DELETE des documents)
folders                   (campaign_id NOT NULL)  ← enfants avant parents (par profondeur DESC), puis racines ; après documents (documents.folder_id NOT NULL)
document_types            (campaign_id NOT NULL, custom uniquement)  ← après documents ET folders (folders.default_document_type_id et documents.document_type_id les référencent, nullable) ; avant campaigns
campaigns                 (racine)
```

**Justification de la position de `document_types`** : `folders.default_document_type_id` et `documents.document_type_id` sont des FK nullable vers `document_types`. Comme elles sont nullable, aucune passe 1 de déliaison n'est nécessaire — la passe 1 ne NULL-e que ce qui est requis pour casser des cycles ou satisfaire des NOT NULL bloquants. En revanche, `document_types` doit être supprimé **après** `documents` et `folders` (qui le référencent), car le SGBD est en `ON DELETE RESTRICT` : tenter de supprimer un `document_types` encore référencé par un `documents` ou `folders` existant déclencherait une erreur RESTRICT. L'ordre `documents → folders → document_types` suffit à éviter ce blocage. Les `document_types` système (`campaign_id IS NULL`) ne sont pas touchés.

---

#### Saga `UserAnonymized` — effacement de compte (campagne vivante)

**Filtre préalable** : s'abstient sur les campagnes en corbeille (`deleted_at IS NOT NULL`). Seules les lignes appartenant à des campagnes actives ou archivées sont traitées.

**Ordre impératif** (conformément à ADR-007) :

1. Réécriture `asp_net_users.password_hash` + `security_stamp` **avant** `users.Anonymize()`.
2. `users.email := 'deleted-{id}@haversack.invalid'`, `display_name := '[Compte supprimé]'`, `status := 'DELETED'`. L'`id` est conservé.
3. Les FK vers `users.id` (`campaigns.owner_id`, `campaign_memberships.user_id`, `folders.created_by_id`, `documents.created_by_id`, `sessions.created_by_id`) restent **intactes** : l'identifiant survit anonymisé, les contraintes NOT NULL sont satisfaites.
4. Les `guest_accesses` de campagnes actives (campagne vivante) : `display_name` n'est pas anonymisé ici — les données d'invité sur une campagne vivante relèvent de **B1.6** (base légale, critère de déclenchement et sort des données invité sur demande individuelle, voir *Points à trancher*). À la **purge de campagne** (J+30), les `guest_accesses` sont hard-deletés en passe 2 (le `display_name` est supprimé avec la ligne — couvert par la saga `CampaignDeleted`, pas par `UserAnonymized`).
5. (Option Art. 17) DELETE des documents sélectionnés selon la politique B1.4, via déliaison-puis-delete. Le périmètre de visibilité supprimable est défini en **B1.4** (hors de cet ADR).
6. **Purge des logs de corrélation** : purge ou anonymisation des entrées de logs applicatifs et d'infrastructure corrélant `UserId` ↔ email d'origine (IP de connexion, logs d'authentification, logs d'audit). Cette étape doit être exécutée dans la fenêtre de traitement Art. 12§3. Politique détaillée dans **ADR-012 §5**.

**MJ propriétaire effaçant son compte sur une campagne vivante** : la campagne reste possédée par l'`id` anonymisé (`owner_id NOT NULL` satisfait). Le transfert de propriété forcé est post-MVP (renvoyé à B1.6/UC-11).

---

### Matrice de préséance des deux sagas

| Situation | `UserAnonymized` | `CampaignDeleted` (purge) |
|---|---|---|
| Campagne active ou archivée (`deleted_at IS NULL`) | S'exécute normalement | Non déclenchée |
| Campagne en corbeille (`deleted_at IS NOT NULL`, purge non claimée) | **S'abstient** sur les lignes de cette campagne | Non encore déclenchée (< J+30 ou job pas passé) |
| Campagne claimée pour purge (`purge_claimed_at IS NOT NULL`) | **S'abstient** — claim irréversible, purge prime | **Prime** — suppression terminale |
| Purge terminée | Sans objet | Sans objet |

La règle : sur toute ligne appartenant à une campagne dont `deleted_at IS NOT NULL`, `UserAnonymized` ne touche rien. `CampaignDeleted` purge est terminale et prime sur toute opération concurrente.

### Saga-événements

- L'événement `CampaignDeleted` est émis au moment du soft-delete (ADR-010) ; la saga de purge physique est déclenchée par le Hosted Service sur critère temporel, pas par un second événement.
- Le claim de purge (`purge_claimed_at`) est posé hors transaction (avant l'ouverture de la transaction de purge) pour garantir l'exclusivité.
- La transaction unique par campagne couvre l'intégralité des deux passes. Sur base unique partagée (MVP monolithe), il n'y a pas de compensating transactions : en cas d'échec, la transaction est rollbackée entièrement et la campagne reste en corbeille. Les compensating transactions mentionnées dans ADR-010 sont réservées à l'architecture multi-service future.

### Contenu LIVE_NOTE et médias externalisés

Le contenu textuel des documents (y compris les LIVE_NOTE) est stocké dans `document_blocks` en base de données et est couvert par la purge passe 2 (`DELETE document_blocks WHERE document_id IN ...`). Si des médias ou blobs associés à des documents sont externalisés (chaîne média B1.9 — stockage objet S3 ou équivalent), leur purge doit être **câblée à la saga `CampaignDeleted`** : un step supplémentaire après la passe 2 SQL doit déclencher la suppression des objets dans le store externe. Sans ce câblage, les blobs orphelins persistent indéfiniment après la purge des métadonnées. Voir **B1.9** pour la définition de la chaîne média.

### Interaction avec les autres ADR

**ADR-009 (FK `ownerId`)** : la FK `campaigns.owner_id → users.id` est NOT NULL et n'est pas déliée lors de la purge. Elle est supprimée avec la ligne `campaigns` en fin de passe 2. L'affirmation « seule FK cross-module » dans ADR-009 est corrigée : 19 FK cross-module sont recensées (voir matrice ci-dessus). ADR-009 reste un précédent de gouvernance valide ; la stratégie `ON DELETE` et le mécanisme de cascade sont définis dans le présent ADR.

**ADR-010 (soft-delete + purge + saga)** : cet ADR complète ADR-010 en spécifiant le mécanisme et l'ordre d'opérations. Les compensating transactions mentionnées dans ADR-010 s'appliquent uniquement à l'extraction future multi-service ; sur le MVP monolithe à base unique, la transaction de base de données joue ce rôle.

**ADR-007 (RGPD pseudonymisation)** : `UserAnonymized` respecte l'ordre impératif défini dans ADR-007 (réécriture ASP.NET Identity avant anonymisation). L'option Art. 17 d'effacement étendu délègue la définition du périmètre supprimable à B1.4.

---

## Points à trancher

Les éléments suivants sont explicitement renvoyés à la Vague 1 (Build-out B1.x) :

- **B1.4 — Périmètre RGPD de l'effacement étendu (option Art. 17)** : **Statué dans [ADR-012](ADR-012-rgpd-effacement-compte.md).** Politique de sélection des documents supprimables (`PLAYER_PRIVATE` et non partagés) et des documents conservés (partagés / `GM_ONLY` sous intérêt légitime Art. 17§3(e)), règle anti-résidu F-08, purge des logs de corrélation UUID↔email, et obligations Art. 12§3 (délai 1 mois, horodatage, notification).
- **B1.6 — Données invité sur campagne vivante** : **Statué dans [ADR-013](ADR-013-rgpd-donnees-invites.md).** Base légale retenue pour `guest_accesses.display_name` (intérêt légitime), information Art. 13 au formulaire invité, rétention autonome à 90 jours après `expires_at`, posture mineurs (service non destiné aux enfants, attestation 16+), posture sous-traitant F-13 et DPA, UC-11 (campagne conservée sous `id` anonymisé au MVP, transfert de propriété forcé post-MVP).
- **B1.9 — Chaîne média** : définir si des blobs/médias sont externalisés et câbler leur suppression à la saga `CampaignDeleted` (voir section *Contenu LIVE_NOTE et médias externalisés* ci-dessus).
- **Art. 20 — Portabilité** : périmètre de la portabilité des données utilisateur et campagne. Post-MVP.
- **Seuil d'expiration du claim** : valeur concrète du timeout de `purge_claimed_at` (ex. 1 heure) à fixer en B1 lors de l'implémentation du Hosted Service.

---

## Compléments post-revue

*(Section réservée aux clarifications post-implémentation — vide à la date de l'ADR.)*

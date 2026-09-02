# Note de mapping EF Core — cluster données & persistance

- **Statut** : Spec pré-build — à confirmer à l'entrée en build (même statut que les ADR sources).
- **Sources** : [ADR-008](../decisions/ADR-008-structure-solution.md) (l.48, Conséquences) ; [ADR-002](../decisions/ADR-002-tout-est-document-gouvernance.md) (l.22, 24, 46, 58) ; [ADR-011](../decisions/ADR-011-cascade-integrite-referentielle.md) (l.30-32, 130-131, 156) ; [ADR-014](../decisions/ADR-014-modele-autorisation-api.md) (l.98-99) ; [content-library.md](../../conception/domain/content-library.md) ; [diagrams/classes/content-library.md](../../conception/domain/diagrams/classes/content-library.md) ; [diagrams/mld/content-library.md](../../conception/domain/diagrams/mld/content-library.md).
- **Périmètre** : les 5 éléments listés par ADR-008:46 comme livrable de mapping EF Core, complétés fidèlement par les décisions de gouvernance ADR-002 et les invariants de cascade/lecture ADR-011/ADR-014 qui portent directement sur le mapping physique.

Cette note ne tranche rien qui ne soit pas déjà fixé par un ADR. Ce qui n'est pas fixé est marqué `[À TRANCHER — ticket]`.

---

## 1. Converters d'IDs typés

**Décision reportée** (ADR-002:20) : un seul identifiant physique, `DocumentId`. Les value objects distincts `ScenarioId`, `SceneId`, `CharacterId` sont supprimés — ils ne correspondaient à aucune entité distincte au sens DDD.

Des **alias sémantiques de type** (ex. `CharacterRef = DocumentId`) sont autorisés pour la lisibilité du code applicatif, **sans créer de type EF Core distinct** : un seul converter de valeur (`ValueConverter<DocumentId, Guid>` ou équivalent) couvre toutes les colonnes `uuid` qui portent conceptuellement un `DocumentId`, y compris celles nommées `character_id` ou `source_document_id` en base.

**Conséquence de mapping** : pas de converter dédié par alias sémantique — un converter unique pour `DocumentId`, réutilisé partout où l'alias s'applique. Les autres IDs typés du cluster (`FolderId`, `DocumentTypeId`, `DocumentBlockId`) suivent le même principe : un converter par identifiant physiquement distinct, pas par alias de lisibilité.

`[À TRANCHER — ticket]` : la validation runtime associée aux alias sémantiques (ex. « le `DocumentId` associé comme personnage doit référencer un `Document` de type `player_character` », ADR-002:54) est un invariant de domaine, pas un mécanisme EF Core — son point d'implémentation (constructeur de VO, méthode de domaine, ou interceptor EF) reste ouvert.

---

## 2. Owned types

**Décision reportée** (ADR-002:22, 46) : `Document.SetProperties()` n'accepte qu'un `DocumentProperties` validé contre le `propertiesSchema` du type. `DocumentProperties` est le value object qui gouverne l'écriture de la colonne `properties` — il est mappé en **owned type** EF Core sur `Document`, propriétaire exclusif de la colonne `documents.properties`.

Contrat détaillé (VO + schémas par type) : voir `document-properties-schemas.md` (companion de cette note pour ce même champ).

`DocumentLink` est également un value object dans l'agrégat `Document` (diagrams/classes/content-library.md:41-45) — owned type porté par la table `document_links`, pas par une colonne jsonb.

`[À TRANCHER — ticket]` : mapping owned-type in-table (colonne unique) vs owned-collection (table séparée) pour `DocumentLink` — le MLD (diagrams/mld/content-library.md:96-112) documente déjà `document_links` comme table séparée avec PK composite `(source_document_id, target_document_id)`, ce qui fixe de fait un owned-collection en table séparée ; aucun ADR ne le nomme explicitement comme choix EF Core.

---

## 3. `jsonb` sur `documents.properties`

**Décision reportée** (ADR-002, ADR-008:46) : `HasColumnType("jsonb")` sur `documents.properties`. Le MLD confirme le type physique : `properties jsonb, nullable` (diagrams/mld/content-library.md:53).

Cette colonne est exclusivement écrite via `Document.SetProperties()` (ADR-002:22) — aucune écriture JSON directe. Voir `document-properties-schemas.md` pour le contrat de validation complet.

---

## 4. Discriminant de bloc

**Modélisation domaine trouvée** (companion `content-library.md` + `diagrams/classes/content-library.md`) : le discriminant de bloc est porté par l'énumération `BlockType`, avec quatre valeurs modélisées : `TEXT`, `TABLE`, `IMAGE`, `DIVIDER` (content-library.md:104 ; diagrams/classes/content-library.md:72-78).

Le MLD confirme la colonne physique : `document_blocks.type varchar(20) NOT NULL`, valeurs `TEXT` / `TABLE` / `IMAGE` / `DIVIDER` (diagrams/mld/content-library.md:88). La colonne `document_blocks.content` est `jsonb NOT NULL` — « contenu sérialisé selon le type » (diagrams/mld/content-library.md:89).

**Conséquence de mapping** : `document_blocks.type` est mappé comme colonne discriminante simple (`varchar` + converter d'énumération `BlockType`), pas comme discriminant TPH EF Core au sens strict — `DocumentBlock` reste une entité unique dont le champ `content` est un `jsonb` non typé fortement, sans hiérarchie de classes C# par valeur de `BlockType`.

`[À TRANCHER — ticket]` : le domaine modélise les *valeurs* du discriminant (`BlockType`) mais ne fixe pas la forme structurée de `content` par valeur (ex. schéma du `content` d'un bloc `TABLE` vs `TEXT`) — cette forme n'est déclarée nulle part dans le companion domaine. Rester `jsonb` libre par décision explicite (ADR-002:58, `DocumentBlock.content` libre, distinct de `properties`) : ce point n'est donc pas un trou mais une décision déjà actée — le mapping EF Core traite `content` comme `jsonb` sans validation de schéma, quel que soit `BlockType`.

---

## 5. Query filters globaux

**Décision reportée** (ADR-014:96-97, prédicats P2/P3 du contexte de lecture unifié) :

| # | Filtre | Portée | Table |
|---|---|---|---|
| P2 | `spaces.deleted_at IS NULL` ET `spaces.purge_claimed_at IS NULL` | Indépendant de l'appelant | `spaces` |
| P3 | `documents.is_deleted = false` | Indépendant de l'appelant | `documents` |

Ces deux prédicats sont « structurels et indépendants de l'appelant » et portés par des **query filters EF Core globaux** — renvoi explicite ADR-014 → **B5.1** pour la spécification technique complète du câblage (le renvoi B5.1 n'est pas résolu dans ADR-014 elle-même).

**Cas d'exception au filtre** : l'endpoint de restauration d'espace (réservé à `OWNER`) est le seul chemin autorisé à traverser P2 — il vérifie `deleted_at IS NOT NULL` explicitement (ADR-014:102). Un query filter global doit donc être contournable sur ce chemin précis (`IgnoreQueryFilters()` ciblé ou requête dédiée hors filtre), pas désactivé globalement.

`[À TRANCHER — ticket]` : le câblage technique précis des query filters (expression `HasQueryFilter`, navigation vers `spaces` depuis les entités qui ne portent pas directement `space_id` en filtre, gestion de `IgnoreQueryFilters` sur le chemin de restauration) est renvoyé à **B5.1** — non résolu par ADR-014.

---

## 6. Colonnes promues nullable indexées — `documents.character_id` / `documents.guest_access_id`

**Décision reportée** (ADR-002:56) : les champs `characterId` et `guestAccessId` des `LIVE_NOTE`, initialement envisagés dans `documents.properties` (jsonb), sont **promus en colonnes nullable indexées de premier niveau** sur `documents`. Ce changement débloque l'invariant d'autorisation d'ADR-007.

**Confirmation MLD** (diagrams/mld/content-library.md:63-64, 74-75) :
- `documents.character_id uuid, nullable` — FK physique réelle intra-module vers `documents.id` (type `player_character`).
- `documents.guest_access_id uuid, nullable` — FK physique réelle cross-module vers `guest_accesses.id` (Space Management).
- Index B-tree partiel `(character_id) WHERE character_id IS NOT NULL` — qualifié « préalable authz OBLIGATOIRE » pour la résolution transitive des documents `PLAYER_PRIVATE`.
- Index B-tree partiel `(guest_access_id) WHERE guest_access_id IS NOT NULL` — même justification côté auteur invité.

**Confirmation matrice FK** (ADR-011:134, 144) : ces deux colonnes portent le rôle « Cycle 1 » du graphe de cascade (`documents ⇄ guest_accesses`) — NULL-ées en passe 1 de la saga `SpaceDeleted`.

**Conséquence de mapping** : ces deux colonnes sont mappées comme propriétés scalaires nullable de premier niveau sur l'entité `Document`, avec les deux index partiels ci-dessus déclarés en configuration EF Core (`HasIndex(...).HasFilter(...)`), pas comme faisant partie de l'owned type `DocumentProperties`.

---

## 7. Toutes FK `ON DELETE RESTRICT`

**Décision reportée** (ADR-011:28-30) : toutes les clés étrangères du graphe (38 FK au total, matrice complète ADR-011 §Schéma et MLD) sont déclarées `ON DELETE RESTRICT`. Le SGBD ne cascade jamais — la progression des suppressions est intégralement pilotée par la saga applicative (`SpaceDeleted`, `UserAnonymized`), jamais par une cascade SQL déclarative.

**Conséquence de mapping** : chaque relation EF Core (`HasOne(...).WithMany(...)`) déclare explicitement `.OnDelete(DeleteBehavior.Restrict)` — ce comportement doit être fixé explicitement à la configuration, car le comportement par défaut d'EF Core pour une FK NOT NULL est `Cascade`, non `Restrict`.

---

## 8. Invariant soft-delete par jointure

**Décision reportée** (ADR-011:154) : la visibilité des données d'un espace en corbeille est portée par jointure sur `spaces.deleted_at IS NOT NULL`. **Aucune table enfant ne porte son propre flag `deleted_at`** — l'invariant s'applique à *tous* les chemins de lecture : requêtes d'énumération, lectures directes par ID, queries SignalR, projections CQRS.

**Conséquence de mapping** : ce n'est pas un query filter portable sur chaque table enfant indépendamment — il exige soit (a) une navigation obligatoire vers `spaces` incluse dans le query filter global de chaque entité enfant qui doit hériter de la visibilité de son espace (`folders`, `documents`, `sessions`, etc.), soit (b) une vérification applicative systématique hors EF Core sur les chemins qui ne passent pas par le filtre global (lecture directe par ID notamment, citée explicitement par ADR-011:154 comme chemin à risque).

`[À TRANCHER — ticket]` : le choix entre (a) query filter EF Core avec navigation implicite vers `spaces` et (b) vérification applicative systématique n'est pas arbitré par ADR-011 — c'est un point de configuration du câblage B5.1 (même renvoi que §5 ci-dessus).

---

## Synthèse des trous nommés

| # | Point ouvert | Ticket |
|---|---|---|
| 1 | Point d'implémentation de la validation runtime des alias sémantiques d'ID | À TRANCHER |
| 2 | Owned-type in-table vs owned-collection pour `DocumentLink` (choix EF Core non nommé explicitement, bien que le MLD fixe déjà la table séparée) | À TRANCHER |
| 4 | Forme structurée de `content` par valeur de `BlockType` (non modélisée — mais `jsonb` libre est un choix déjà acté, pas un trou de mapping) | Décision actée, forme non modélisée |
| 5 | Câblage technique des query filters P2/P3 et du contournement sur le chemin de restauration | Renvoyé à B5.1 |
| 8 | Mécanisme exact de propagation de l'invariant soft-delete par jointure aux tables enfant | Renvoyé à B5.1 |

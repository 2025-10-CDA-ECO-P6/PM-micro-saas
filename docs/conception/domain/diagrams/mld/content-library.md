# Content Library — Modèle Logique de Données (MLD)

> **Nature : vue physique assumée** — quatrième vue de la suite de modélisation du domaine (prose → classes → MCD → MLD). Les types SQL et index sont la fonction de ce document ; il reste dans la couche conception par arbitrage T-08 (2026-06-10), sans nommer de produit d'infrastructure.

## Table `folders`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `space_id` | `uuid` | NOT NULL | FK physique réelle → `spaces.id` (Space Management) — exception assumée inter-module, voir note ci-dessous |
| `parent_folder_id` | `uuid` | FK → `folders.id`, nullable | Self-référence pour l'arborescence |
| `name` | `varchar(200)` | NOT NULL | |
| `is_system` | `bool` | NOT NULL, DEFAULT `false` | Créé automatiquement à `SpaceCreated` — informatif uniquement, non restrictif |
| `is_virtual` | `bool` | NOT NULL, DEFAULT `false` | Dossier "Non classés" — invisible dans la navigation, non supprimable, un seul par campagne |
| `default_document_type_id` | `uuid` | FK → `document_types.id`, nullable | |
| `default_template_document_id` | `uuid` | FK → `documents.id`, nullable | Template utilisé pour initialiser les nouveaux documents créés dans ce dossier |
| `order` | `int` | NOT NULL DEFAULT 0 | Ordre d'affichage dans le dossier parent |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `created_by_id` | `uuid` | NOT NULL | FK physique réelle → `users.id` (Identity & Access) — exception assumée inter-module, voir note ci-dessous |

---

## Table `document_types`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `slug` | `varchar(50)` | NOT NULL | `scenario`, `scene`, `npc`, `location`, `note`, `player_character` — voir contraintes d'unicité ci-dessous |
| `name` | `varchar(100)` | NOT NULL | Nom affiché |
| `properties_schema` | `jsonb` | nullable | Schéma JSON des propriétés structurées |
| `is_system` | `bool` | NOT NULL, DEFAULT `false` | Types built-in non modifiables |
| `space_id` | `uuid` | nullable | `null` pour types système, `space_id` pour types custom |
| `created_at` | `timestamptz` | NOT NULL | |

**Contraintes d'unicité sur `slug`** (C-16 — décision C2 : unicité conditionnelle, pas globale) :
- `UNIQUE (slug) WHERE space_id IS NULL` — types système uniques globalement.
- `UNIQUE (space_id, slug) WHERE space_id IS NOT NULL` — types custom uniques par campagne (permet à deux campagnes différentes de définir un type avec le même slug).

**Types seedés** : `scenario`, `scene`, `npc`, `location`, `note`, `player_character`, `live_note`, `reveal`

---

## Table `documents`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `space_id` | `uuid` | NOT NULL | FK physique réelle → `spaces.id` (Space Management) — exception assumée inter-module, voir note ci-dessous |
| `folder_id` | `uuid` | FK → `folders.id`, NOT NULL | |
| `title` | `varchar(500)` | NOT NULL | |
| `document_type_id` | `uuid` | FK → `document_types.id`, nullable | |
| `properties` | `jsonb` | nullable | Propriétés structurées selon le type |
| `visibility` | `varchar(20)` | NOT NULL, DEFAULT `'GM_ONLY'` | `PUBLIC` / `GM_ONLY` / `PLAYER_PRIVATE` |
| `slug` | `varchar(300)` | NOT NULL | |
| `is_reusable` | `bool` | NOT NULL, DEFAULT `false` | |
| `source_document_id` | `uuid` | FK → `documents.id`, nullable | Si instancié depuis un template |
| `is_deleted` | `bool` | NOT NULL, DEFAULT `false` | Soft-delete |
| `deleted_at` | `timestamptz` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `created_by_id` | `uuid` | NOT NULL | FK physique réelle → `users.id` (Identity & Access) — exception assumée inter-module, voir note ci-dessous |
| `character_id` | `uuid` | nullable | FK physique réelle → `documents.id` (Content Library, type `player_character`) — exception assumée intra-module ; personnage associé d'un LIVE_NOTE (promu depuis `properties`, ADR-002) |
| `guest_access_id` | `uuid` | nullable | FK physique réelle → `guest_accesses.id` (Space Management) — exception assumée inter-module ; auteur invité d'un LIVE_NOTE quand `created_by_id` est null (promu depuis `properties`, ADR-002) |

**Index**

| Index | Type | Condition | Justification |
|---|---|---|---|
| `UNIQUE (space_id, document_type_id, slug) NULLS NOT DISTINCT` | B-tree unique | — | Unicité du slug par campagne et type (contrainte métier). `NULLS NOT DISTINCT` (option SQL non universelle, à défaut contrainte équivalente côté application) : deux documents sans type (`document_type_id IS NULL`) dans la même campagne avec le même slug sont rejetés — sans cette option, le SGBD traiterait les NULL comme distincts et laisserait passer des doublons. |
| `(folder_id) WHERE is_deleted = false` | B-tree partiel | `is_deleted = false` | Navigation de l'arborescence d'un dossier — exclut les soft-deletes. Source C-02. |
| `(space_id, document_type_id) WHERE is_deleted = false` | B-tree partiel | `is_deleted = false` | Lookups par campagne + type (ex. lister les `player_character` d'une campagne pour la validation runtime ADR-002). Source C-02. |
| `GIN trigramme (title) WHERE is_deleted = false` | GIN trigramme | `is_deleted = false` | Recherche par sous-chaîne (« contient ») sur le titre. ⚠️ Requiert l'activation de l'extension de recherche trigramme du SGBD. Sources C-01/CR-5/H-05. **Note** : indexe le seul `title` — reste « recherche titre seul » au sens d'ADR-002. La recherche full-text sur le contenu (`tsvector`) est post-MVP assumée (non indexée ici). |
| `(character_id) WHERE character_id IS NOT NULL` | B-tree partiel | `character_id IS NOT NULL` | **Préalable authz OBLIGATOIRE** — résolution des documents `PLAYER_PRIVATE` associés à un personnage. Cet index sert l'invariant métier n°6 (Content Library, prose domaine) : un document `PLAYER_PRIVATE` ne peut être lu que via son auteur, y compris quand cette identité est portée par le chemin de résolution `characterId` → résolution transitive `membership_characters`. |
| `(guest_access_id) WHERE guest_access_id IS NOT NULL` | B-tree partiel | `guest_access_id IS NOT NULL` | **Préalable authz OBLIGATOIRE** — résolution des documents `PLAYER_PRIVATE` rattachés à un invité. Cet index sert l'invariant métier n°6 (Content Library, prose domaine) : un document `PLAYER_PRIVATE` ne peut être lu que par son auteur invité, y compris quand cette identité est portée par le chemin de résolution `guestAccessId`. |

> **Post-MVP assumé** : index GIN `tsvector` full-text sur `title`/`content`/`properties` — non inclus MVP.

---

## Table `document_blocks`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `document_id` | `uuid` | FK → `documents.id`, NOT NULL | |
| `order` | `int` | NOT NULL | Position dans le document |
| `type` | `varchar(20)` | NOT NULL | `TEXT` / `TABLE` / `IMAGE` / `DIVIDER` |
| `content` | `jsonb` | NOT NULL | Contenu sérialisé selon le type |
| `is_locked` | `bool` | NOT NULL, DEFAULT `false` | Modélisé, non activé MVP |

**Index** : `(document_id, order)`

---

## Table `document_links`

Références ordonnées entre documents. Représente la relation "ce document en référence un autre"
(liste de scènes d'un scénario, PNJ d'une scène, etc.).

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `source_document_id` | `uuid` | PK, FK → `documents.id`, NOT NULL | Document source |
| `target_document_id` | `uuid` | PK, FK → `documents.id`, NOT NULL | Document cible |
| `order` | `int` | NOT NULL | Position dans la liste des références du source |

**PK** : `(source_document_id, target_document_id)`
**Index** : `(target_document_id)` — pour le calcul des backlinks

> **Note** : les backlinks sont calculés à la lecture via une requête sur `target_document_id`.
> Aucune table de backlinks n'est nécessaire.

---

## Table `document_tags`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `document_id` | `uuid` | PK, FK → `documents.id`, NOT NULL | |
| `tag` | `varchar(50)` | PK, NOT NULL | Valeur normalisée |

**PK** : `(document_id, tag)`
**Index** : `(tag)` — pour le filtrage par tag

---

## Note — FK inter-modules

Certaines colonnes de `documents` sont des **clés étrangères physiques réelles** vers d'autres modules — exception assumée du monolithe modulaire (ADR-009). Deux natures à distinguer :

- **Intra-module** : `character_id` → `documents.id` (même module Content Library, type `player_character`). Auto-FK dans le même bounded context.
- **Cross-module** : `space_id` (dans `folders` et `documents`), `created_by_id`, et `guest_access_id` traversent une frontière de bounded context vers la table propriétaire de l'autre module.

L'isolation des contextes est tenue au niveau du code (contrats, namespaces), pas par l'absence de FK. À l'extraction éventuelle d'un contexte en service dédié, ces FK deviendront des projections par events. *(ADR-009)*

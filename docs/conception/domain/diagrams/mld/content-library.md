# Content Library — Modèle Logique de Données (MLD)

## Table `folders`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | NOT NULL | Référence logique Campaign Management (pas de FK physique inter-module) |
| `parent_folder_id` | `uuid` | FK → `folders.id`, nullable | Self-référence pour l'arborescence |
| `name` | `varchar(200)` | NOT NULL | |
| `is_system` | `bool` | NOT NULL, DEFAULT `false` | Créé automatiquement à `CampaignCreated` — informatif uniquement, non restrictif |
| `is_virtual` | `bool` | NOT NULL, DEFAULT `false` | Dossier "Non classés" — invisible dans la navigation, non supprimable, un seul par campagne |
| `default_document_type_id` | `uuid` | FK → `document_types.id`, nullable | |
| `default_template_document_id` | `uuid` | FK → `documents.id`, nullable | Template utilisé pour initialiser les nouveaux documents créés dans ce dossier |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `created_by_id` | `uuid` | NOT NULL | Référence logique vers `users.id` |

---

## Table `document_types`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `slug` | `varchar(50)` | UNIQUE, NOT NULL | `scenario`, `scene`, `npc`, `location`, `note`, `player_character` |
| `name` | `varchar(100)` | NOT NULL | Nom affiché |
| `properties_schema` | `jsonb` | nullable | Schéma JSON des propriétés structurées |
| `is_system` | `bool` | NOT NULL, DEFAULT `false` | Types built-in non modifiables |
| `campaign_id` | `uuid` | nullable | `null` pour types système, `campaign_id` pour types custom |
| `created_at` | `timestamptz` | NOT NULL | |

**Types seedés** : `scenario`, `scene`, `npc`, `location`, `note`, `player_character`, `live_note`, `reveal`

---

## Table `documents`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | NOT NULL | Référence logique Campaign Management |
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
| `created_by_id` | `uuid` | NOT NULL | Référence logique vers `users.id` |

**Index** : `UNIQUE (campaign_id, document_type_id, slug)` — unicité du slug par campagne et type

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

## Note sur les FK inter-modules

Les colonnes `campaign_id` (dans `folders` et `documents`) et `created_by_id` sont des
références logiques sans FK physique, conformément au principe d'isolation entre modules
du monolithe modulaire. La cohérence est garantie par la couche application.

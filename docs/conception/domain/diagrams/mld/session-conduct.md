# Session Conduct — Modèle Logique de Données (MLD)

> **Nature : vue physique assumée** — quatrième vue de la suite de modélisation du domaine (prose → classes → MCD → MLD). Les types SQL et index sont la fonction de ce document ; il reste dans la couche conception par arbitrage T-08 (2026-06-10), sans nommer de produit d'infrastructure.

## Table `sessions`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `space_id` | `uuid` | NOT NULL | FK physique réelle → `spaces.id` (Space Management) — exception assumée inter-module, voir note ci-dessous |
| `title` | `varchar(300)` | NOT NULL | |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'LIVE'` | `LIVE` / `CLOSED` / `ARCHIVED` |
| `scenario_id` | `uuid` | nullable | FK physique réelle → `documents.id` (Content Library) — exception assumée inter-module, voir note ci-dessous |
| `summary` | `text` | nullable | Éditable en CLOSED |
| `started_at` | `timestamptz` | NOT NULL | |
| `closed_at` | `timestamptz` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `created_by_id` | `uuid` | NOT NULL | FK physique réelle → `users.id` (Identity & Access) — exception assumée inter-module, voir note ci-dessous |

**Contrainte partielle** : `UNIQUE (space_id) WHERE status = 'LIVE'` — une seule session au statut LIVE par campagne. *(C-14, décision B1)*

---

## Table `session_pinned_documents`

Documents épinglés par le MJ pendant la session.

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_id` | `uuid` | PK, FK → `sessions.id`, NOT NULL | |
| `document_id` | `uuid` | PK, NOT NULL | FK physique réelle → `documents.id` (Content Library) — exception assumée inter-module, voir note ci-dessous |

**PK** : `(session_id, document_id)`

---

## Table `session_live_notes`

Notes de session — références vers des Documents de type LIVE_NOTE dans Content Library.

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_id` | `uuid` | PK, FK → `sessions.id`, NOT NULL | |
| `document_id` | `uuid` | PK, NOT NULL | FK physique réelle → `documents.id` (Content Library, type LIVE_NOTE) — exception assumée inter-module, voir note ci-dessous |

**PK** : `(session_id, document_id)`

> Les données de la note (contenu, visibility, characterId, guestAccessId) sont dans
> la table `documents` et `document_blocks` de Content Library.
> `character_id` et `guest_access_id` sont des **colonnes de premier niveau** sur `documents` (promues depuis `properties`, ADR-002).

---

## Table `session_view_configs`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `space_id` | `uuid` | UNIQUE, NOT NULL | Un seul config par campagne |
| `updated_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (space_id)`

---

## Table `session_view_folders`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_view_config_id` | `uuid` | PK, FK → `session_view_configs.id`, NOT NULL | |
| `folder_id` | `uuid` | PK, NOT NULL | FK physique réelle → `folders.id` (Content Library) — exception assumée inter-module, voir note ci-dessous |
| `order` | `int` | NOT NULL | Ordre d'affichage |

**PK** : `(session_view_config_id, folder_id)`

---

## Note — FK inter-modules

Les colonnes traversant une frontière de bounded context (`space_id`, `scenario_id`, `document_id`, `created_by_id`, `folder_id`) sont des **clés étrangères physiques réelles** vers la table propriétaire de l'autre module. C'est une **exception assumée** du monolithe modulaire à base de données unique partagée : l'isolation des contextes est tenue au niveau du code (contrats, namespaces), pas par l'absence de FK. À l'extraction éventuelle d'un contexte en service dédié, ces FK deviendront des projections par events. *(ADR-009)*

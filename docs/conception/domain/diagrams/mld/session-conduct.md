# Session Conduct — Modèle Logique de Données (MLD)

## Table `sessions`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | NOT NULL | Référence logique Campaign Management |
| `title` | `varchar(300)` | NOT NULL | |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'LIVE'` | `LIVE` / `CLOSED` / `ARCHIVED` |
| `scenario_id` | `uuid` | nullable | Référence logique Content Library |
| `summary` | `text` | nullable | Éditable en CLOSED |
| `started_at` | `timestamptz` | NOT NULL | |
| `closed_at` | `timestamptz` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `created_by_id` | `uuid` | NOT NULL | Référence logique vers `users.id` |

---

## Table `session_pinned_documents`

Documents épinglés par le MJ pendant la session.

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_id` | `uuid` | PK, FK → `sessions.id`, NOT NULL | |
| `document_id` | `uuid` | PK, NOT NULL | Référence logique Content Library |

**PK** : `(session_id, document_id)`

---

## Table `session_live_notes`

Notes de session — références vers des Documents de type LIVE_NOTE dans Content Library.

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_id` | `uuid` | PK, FK → `sessions.id`, NOT NULL | |
| `document_id` | `uuid` | PK, NOT NULL | Référence logique Content Library — Document de type LIVE_NOTE |

**PK** : `(session_id, document_id)`

> Les données de la note (contenu, visibility, characterId, guestAccessId) sont dans
> la table `documents` et `document_blocks` de Content Library.
> `guestAccessId` et `characterId` sont dans `documents.properties` (jsonb).

---

## Table `session_view_configs`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | UNIQUE, NOT NULL | Un seul config par campagne |
| `updated_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (campaign_id)`

---

## Table `session_view_folders`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `session_view_config_id` | `uuid` | PK, FK → `session_view_configs.id`, NOT NULL | |
| `folder_id` | `uuid` | PK, NOT NULL | Référence logique Content Library |
| `order` | `int` | NOT NULL | Ordre d'affichage |

**PK** : `(session_view_config_id, folder_id)`

---

## Note sur les FK inter-modules

Toutes les colonnes référençant d'autres modules (`campaign_id`, `scenario_id`, `document_id`, `created_by_id`, `guest_access_id`, `character_id`, `folder_id`) sont des références logiques sans FK physique, conformément au principe d'isolation entre modules du monolithe modulaire.

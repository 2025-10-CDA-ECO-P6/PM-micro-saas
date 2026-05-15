# Campaign Management — Modèle Logique de Données (MLD)

## Table `campaigns`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `owner_id` | `uuid` | FK → `users.id`, NOT NULL | Propriétaire technique |
| `name` | `varchar(200)` | NOT NULL | |
| `slug` | `varchar(100)` | NOT NULL | |
| `type` | `varchar(20)` | NOT NULL | `CAMPAIGN` / `ONE_SHOT` |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `ACTIVE` / `ARCHIVED` / `FROZEN` |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (owner_id, slug)`

---

## Table `campaign_memberships`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `campaign_id` | `uuid` | PK, FK → `campaigns.id`, NOT NULL | |
| `user_id` | `uuid` | PK, FK → `users.id`, NOT NULL | |
| `role` | `varchar(10)` | NOT NULL | `OWNER` / `GM` / `PLAYER` |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `PENDING` / `ACTIVE` / `REMOVED` |
| `joined_at` | `timestamptz` | NOT NULL | |

**PK** : `(campaign_id, user_id)`

---

## Table `membership_characters`

Table de liaison entre un membership et les personnages associés (IDs vers Content Library).

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `campaign_id` | `uuid` | PK, FK → `campaigns.id`, NOT NULL | |
| `user_id` | `uuid` | PK, FK → `campaign_memberships.user_id`, NOT NULL | |
| `character_id` | `uuid` | PK, NOT NULL | Référence vers Content Library (pas de FK physique inter-module) |

**PK** : `(campaign_id, user_id, character_id)`

---

## Table `invitations`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | FK → `campaigns.id`, NOT NULL | |
| `token` | `uuid` | UNIQUE, NOT NULL | Utilisé dans l'URL |
| `type` | `varchar(10)` | NOT NULL | `LINK` / `EMAIL` |
| `scope` | `varchar(20)` | NOT NULL | `CAMPAIGN` / `SESSION` |
| `session_id` | `uuid` | nullable | Référence vers Session Conduct (pas de FK physique) |
| `expires_at` | `timestamptz` | nullable | |
| `max_uses` | `int` | nullable | |
| `used_count` | `int` | NOT NULL, DEFAULT `0` | |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `ACTIVE` / `REVOKED` / `EXPIRED` |
| `created_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (token)`

---

## Table `guest_accesses`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `campaign_id` | `uuid` | FK → `campaigns.id`, NOT NULL | |
| `scope` | `varchar(20)` | NOT NULL | `SESSION` / `CAMPAIGN` |
| `session_id` | `uuid` | nullable | Référence vers Session Conduct (pas de FK physique) |
| `token` | `uuid` | UNIQUE, NOT NULL | Utilisé dans l'URL |
| `display_name` | `varchar(100)` | NOT NULL | |
| `character_id` | `uuid` | nullable | Référence vers Content Library (pas de FK physique) |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `ACTIVE` / `EXPIRED` / `REVOKED` / `CONVERTED` |
| `expires_at` | `timestamptz` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (token)`

> **Note sur les FK inter-modules** : les colonnes `session_id`, `character_id` (dans `guest_accesses`)
> et `character_id` (dans `membership_characters`) sont des références logiques sans FK physique,
> conformément au principe d'isolation entre modules du monolithe modulaire.
> La cohérence est garantie par la couche application.

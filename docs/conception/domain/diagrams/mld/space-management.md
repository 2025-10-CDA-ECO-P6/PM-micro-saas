# Space Management — Modèle Logique de Données (MLD)

> **Nature : vue physique assumée** — quatrième vue de la suite de modélisation du domaine (prose → classes → MCD → MLD). Les types SQL et index sont la fonction de ce document ; il reste dans la couche conception par arbitrage T-08 (2026-06-10), sans nommer de produit d'infrastructure.

## Table `spaces`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `owner_id` | `uuid` | FK → `users.id`, NOT NULL | Propriétaire technique — FK physique réelle inter-module (I&A), exception assumée *(ADR-009)* |
| `name` | `varchar(200)` | NOT NULL | |
| `slug` | `varchar(100)` | NOT NULL | |
| `type` | `varchar(20)` | NOT NULL | `CAMPAIGN` / `ONE_SHOT` |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `ACTIVE` / `ARCHIVED` / `FROZEN` |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |
| `deleted_at` | `timestamptz` | NULL | Soft-delete + corbeille 30 j (ADR-010) ; purge physique = job ultérieur. Stratégie de cascade définie : voir [ADR-011](../../../../architecture/decisions/ADR-011-cascade-integrite-referentielle.md). |
| `purge_claimed_at` | `timestamptz` | NULL | Claim de purge exclusif posé par le Hosted Service avant l'ouverture de la transaction de purge. Une campagne claimée ne peut plus être restaurée. Claim expirant après un seuil configurable (campagne reclaimable en cas de crash) — voir [ADR-011](../../../../architecture/decisions/ADR-011-cascade-integrite-referentielle.md). |

**Index** : `UNIQUE (owner_id, slug)`

---

## Table `space_memberships`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `space_id` | `uuid` | PK, FK → `spaces.id`, NOT NULL | |
| `user_id` | `uuid` | PK, FK → `users.id`, NOT NULL | |
| `role` | `varchar(10)` | NOT NULL | `OWNER` / `GM` / `PLAYER` |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `PENDING` / `ACTIVE` / `REMOVED` |
| `joined_at` | `timestamptz` | NOT NULL | |

**PK** : `(space_id, user_id)`

---

## Table `membership_characters`

Table de liaison entre un membership et les personnages associés (IDs vers Content Library).

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `space_id` | `uuid` | PK, FK → `spaces.id`, NOT NULL | |
| `user_id` | `uuid` | PK, NOT NULL | |
| `character_id` | `uuid` | PK, NOT NULL | FK physique réelle → `documents.id` (Content Library, type `player_character`) — exception assumée inter-module, voir note ci-dessous |

**PK** : `(space_id, user_id, character_id)`
**FK composite** : `(space_id, user_id) → space_memberships(space_id, user_id)` — intra-module (la PK de `space_memberships` est composite ; référencer `user_id` seul serait invalide).

---

## Table `invitations`

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `space_id` | `uuid` | FK → `spaces.id`, NOT NULL | |
| `token` | `uuid` | UNIQUE, NOT NULL | Utilisé dans l'URL |
| `type` | `varchar(10)` | NOT NULL | `LINK` / `EMAIL` |
| `scope` | `varchar(20)` | NOT NULL | `CAMPAIGN` / `SESSION` |
| `session_id` | `uuid` | nullable | FK physique réelle → `sessions.id` (Session Conduct) — exception assumée inter-module, voir note ci-dessous |
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
| `space_id` | `uuid` | FK → `spaces.id`, NOT NULL | |
| `scope` | `varchar(20)` | NOT NULL | `SESSION` / `CAMPAIGN` |
| `session_id` | `uuid` | nullable | FK physique réelle → `sessions.id` (Session Conduct) — exception assumée inter-module, voir note ci-dessous |
| `token` | `uuid` | UNIQUE, NOT NULL | Utilisé dans l'URL |
| `display_name` | `varchar(100)` | NOT NULL | |
| `character_id` | `uuid` | nullable | FK physique réelle → `documents.id` (Content Library, type `player_character`) — exception assumée inter-module, voir note ci-dessous |
| `status` | `varchar(20)` | NOT NULL, DEFAULT `'ACTIVE'` | `ACTIVE` / `EXPIRED` / `REVOKED` / `CONVERTED` |
| `expires_at` | `timestamptz` | nullable | |
| `created_at` | `timestamptz` | NOT NULL | |

**Index** : `UNIQUE (token)`

> **Note — FK inter-modules** : les colonnes traversant une frontière de bounded context (`owner_id` → I&A, `space_memberships.user_id` → I&A, `membership_characters.character_id` → Content Library, `invitations.session_id` et `guest_accesses.session_id` → Session Conduct, `guest_accesses.character_id` → Content Library) sont des **clés étrangères physiques réelles** vers la table propriétaire de l'autre module. C'est une **exception assumée** du monolithe modulaire à base de données unique partagée : l'isolation des contextes est tenue au niveau du code (contrats, namespaces), pas par l'absence de FK. À l'extraction éventuelle d'un contexte en service dédié, ces FK deviendront des projections par events. *(ADR-009)*

---

## Table `scenario_library_entries` *(post-MVP)*

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | |
| `owner_id` | `uuid` | FK → `users.id`, NOT NULL | Compte propriétaire de l'entrée — FK physique réelle inter-module (I&A), exception assumée *(ADR-009)* |
| `document_id` | `uuid` | FK → `documents.id`, NOT NULL | Document promu depuis Content Library — FK physique réelle inter-module, exception assumée *(ADR-009)* |
| `promoted_at` | `timestamptz` | NOT NULL | Horodatage de la promotion |

**Index** : `UNIQUE (owner_id, document_id)`

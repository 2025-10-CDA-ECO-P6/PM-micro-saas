# Identity & Access — Modèle Logique de Données (MLD)

## Table `users` (domaine I&A)

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | Partagé avec `asp_net_users.id` |
| `email` | `varchar(256)` | UNIQUE, NOT NULL | Adresse email normalisée |
| `display_name` | `varchar(100)` | NOT NULL | Nom d'affichage |
| `status` | `varchar(20)` | NOT NULL, DEFAULT 'ACTIVE' | ACTIVE / SUSPENDED / DELETED |
| `tier` | `varchar(10)` | NOT NULL, DEFAULT 'FREE' | FREE / PRO |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

## Table `asp_net_users` (infrastructure ASP.NET Identity — référence)

Gérée par ASP.NET Identity. Non modifiée par le domaine I&A.
Partage le même `id` (UUID) que la table `users`.

| Colonne | Description |
|---|---|
| `id` | UUID partagé avec `users.id` |
| `user_name` | Identifiant technique (= email) |
| `normalized_email` | Email normalisé pour les recherches |
| `password_hash` | Hash bcrypt géré par Identity |
| `security_stamp` | Token de révocation |
| … | Autres colonnes Identity standard |

## Cohérence entre les deux tables

- Les deux tables partagent le même `id`.
- `users` est créée par le domaine immédiatement après `asp_net_users`.
- La suppression logique se fait via `users.status = 'DELETED'` — `asp_net_users` peut être anonymisée en parallèle.
- Aucune FK explicite entre les deux tables (modules séparés) — la cohérence est garantie par la couche application.

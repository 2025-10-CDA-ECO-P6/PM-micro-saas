# Identity & Access — Modèle Logique de Données (MLD)

> **Nature : vue physique assumée** — quatrième vue de la suite de modélisation du domaine (prose → classes → MCD → MLD). Les types SQL et index sont la fonction de ce document ; il reste dans la couche conception par arbitrage T-08 (2026-06-10), sans nommer de produit d'infrastructure.

## Table `users` (domaine I&A)

| Colonne | Type SQL | Contraintes | Description |
|---|---|---|---|
| `id` | `uuid` | PK, NOT NULL | Partagé avec la table du composant d'identité |
| `email` | `varchar(256)` | UNIQUE, NOT NULL | Adresse email normalisée |
| `display_name` | `varchar(100)` | NOT NULL | Nom d'affichage |
| `status` | `varchar(20)` | NOT NULL, DEFAULT 'ACTIVE' | ACTIVE / SUSPENDED / DELETED |
| `tier` | `varchar(10)` | NOT NULL, DEFAULT 'FREE' | FREE / PRO |
| `created_at` | `timestamptz` | NOT NULL | |
| `updated_at` | `timestamptz` | NOT NULL | |

## Table du référentiel d'identité (infrastructure — composant d'identité, référence)

Gérée par le composant d'identité. Non modifiée par le domaine I&A.
Partage le même `id` (UUID) que la table `users`.

| Colonne | Description |
|---|---|
| `id` | UUID partagé avec `users.id` |
| `user_name` | Identifiant technique (= email) |
| `normalized_email` | Email normalisé pour les recherches |
| `password_hash` | Empreinte de mot de passe gérée par le composant d'identité |
| `security_stamp` | Token de révocation |
| … | Autres colonnes standard du composant d'identité |

## Cohérence entre les deux tables

- Les deux tables partagent le même `id`.
- `users` est créée par le domaine immédiatement après la table du composant d'identité.
- La suppression logique se fait via `users.status = 'DELETED'` — la table du composant d'identité peut être anonymisée en parallèle.
- Aucune FK explicite entre les deux tables — la cohérence est garantie par la couche application. Cette absence de FK est **intentionnelle et légitime** : `users` (domaine I&A) et la table du composant d'identité (infrastructure) sont deux couches au sein du même bounded context Identity & Access, pas deux bounded contexts distincts. Il ne s'agit pas d'une frontière inter-module : la règle « toutes FK réelles » (ADR-009) ne s'applique pas ici. L'isolation est de nature technique (séparation domaine/infra), non DDD.

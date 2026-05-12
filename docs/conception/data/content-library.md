# MCD — Content Library

Contexte central du contenu éditorial. Pattern structurant : chaque entité métier
(`NPC`, `PLAYER_CHARACTER`, `SCENARIO`, `SCENE`) possède un `DOCUMENT` pour son
contenu variable, et porte ses propres métadonnées dans une table séparée.

## 4.1 Document et blocs

```mermaid
erDiagram
    DOCUMENT {
        uuid id PK
        uuid campaignId "ref Campaign Management"
        enum type "NOTE|NPC|CHARACTER|SCENARIO|SCENE|LOCATION|CUSTOM"
        string customType "obligatoire si type = CUSTOM, null sinon"
        string title
        string slug "unique par (campaignId, type) — affichage uniquement, jamais dans les URLs"
        enum visibility "PRIVATE | PLAYER_PRIVATE | SHARED | PUBLIC"
        uuid ownerCharacterId "ref PLAYER_CHARACTER, nullable — requis si PLAYER_PRIVATE"
        uuid templateId FK "-> DOCUMENT_TEMPLATE, nullable"
        uuid folderId FK "-> FOLDER, nullable — null = non classe"
        int appliedTemplateVersion "nullable — version template appliquee, compare a DOCUMENT_TEMPLATE.version"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    DOCUMENT_BLOCK {
        uuid id PK
        uuid documentId FK "-> DOCUMENT"
        enum kind "TEXT|FIELD|STAT_BAR|RELATION|LIST|ITEM|CHECKLIST|IMAGE"
        string label "nullable, nom du champ affiche"
        int order "position dans le document"
        jsonb value "structure selon kind — voir table ci-dessous"
        boolean isPrivate "true = visible MJ uniquement"
        boolean isLocked "true = non modifiable joueur (modele, inactif MVP)"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
    }
    DOCUMENT_TEMPLATE {
        uuid id PK
        string name
        enum documentType "meme valeurs que DOCUMENT.type"
        uuid gameSystemId "ref Campaign Management, nullable"
        enum scope "BUILTIN | CAMPAIGN | USER"
        uuid ownerId "ref Identity, null si BUILTIN"
        uuid campaignId "ref Campaign Management, null si USER ou BUILTIN"
        int version "commence a 1, incremente a chaque modification du schema"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    BLOCK_SCHEMA {
        uuid id PK
        uuid templateId FK "-> DOCUMENT_TEMPLATE"
        enum kind "meme valeurs que DOCUMENT_BLOCK.kind"
        string label
        boolean required "indicateur UI — n'invalide pas un document incomplet"
        jsonb defaultValue "nullable, meme structure que DOCUMENT_BLOCK.value"
        int order
    }
    TAG {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        string label "unique par campaignId (insensible casse)"
        string color "nullable, hex ou nom"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    DOCUMENT_TAG {
        uuid documentId FK "-> DOCUMENT — PK composite (documentId, tagId)"
        uuid tagId FK "-> TAG — PK composite (documentId, tagId)"
    }
    DOCUMENT ||--o{ DOCUMENT_BLOCK : "compose de (zero ou plusieurs)"
    DOCUMENT ||--o{ DOCUMENT_TAG : "tague par"
    TAG ||--o{ DOCUMENT_TAG : "applique a"
    DOCUMENT }o--o| DOCUMENT_TEMPLATE : "cree depuis (snapshot)"
    DOCUMENT_TEMPLATE ||--o{ BLOCK_SCHEMA : "definit"
    FOLDER ||--o{ DOCUMENT : "contient"
    FOLDER }o--o| DOCUMENT_TEMPLATE : "template par defaut"
```

## 4.1b Table FOLDER

```mermaid
erDiagram
    FOLDER {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        string name
        string slug "unique par campaignId"
        uuid defaultTemplateId "ref DOCUMENT_TEMPLATE, nullable"
        boolean isSystem "true = PNJ, Personnages joueurs, Scenarios, Notes"
        int order "position dans la navigation de campagne"
        datetime createdAt
        datetime updatedAt
        uuid createdById FK
        uuid updatedById FK
        boolean isDeleted
        datetime deletedAt
        uuid deletedById FK
    }
```

**Dossiers système créés à l'initialisation de chaque campagne :**

| name | isSystem | defaultTemplateId |
|---|---|---|
| PNJ | true | → template "Fiche PNJ générique" |
| Personnages joueurs | true | → template "Fiche personnage générique" |
| Scénarios | true | null |
| Notes | true | null |

- Dossiers système : `isSystem = true`, non supprimables, renommables.
- `slug` : unique par `campaignId`, généré à la création, non modifiable après.
- `order` : géré par `FolderOrderService` (application service).
- Index recommandés : `FOLDER(campaignId)`, `FOLDER(campaignId, slug)` unique, `FOLDER(campaignId, isSystem)`.

### Notes sur les Tags

- `TAG.label` : contrainte unique sur `(campaignId, LOWER(label))` pour insensibilité à la casse.
- `DOCUMENT_TAG` : PK composite `(documentId, tagId)` — pas de colonne `id`.
- Suppression d'un TAG (`isDeleted = true`) : supprimer physiquement toutes les lignes `DOCUMENT_TAG` correspondantes.
  Ce nettoyage est déclenché par le domain event `TagDeleted` via un handler synchrone.
- Index recommandés : `TAG(campaignId)`, `DOCUMENT_TAG(documentId)`, `DOCUMENT_TAG(tagId)`.

### Structure de DOCUMENT_BLOCK.value selon kind

| kind | Structure JSON |
|---|---|
| `TEXT` | `{ "content": "string" }` |
| `FIELD` | `{ "value": "string" }` |
| `STAT_BAR` | `{ "current": 14, "max": 20 }` |
| `RELATION` | `{ "targetId": "uuid", "targetType": "NPC" }` |
| `LIST` | `{ "items": ["string", "string"] }` |
| `ITEM` | `{ "name": "string", "quantity": 1, "properties": { "weight": "2kg" } }` |
| `CHECKLIST` | `{ "items": [{ "label": "string", "checked": false }] }` |
| `IMAGE` | `{ "url": "string", "caption": "string" }` |

**Convention de migration** : toute modification de la structure JSON d'un `BlockKind` doit être
rétrocompatible ou accompagnée d'une migration de données JSONB.

## 4.2 Entités métier

```mermaid
erDiagram
    NPC {
        uuid id PK
        uuid campaignId "ref Campaign Management"
        uuid documentId FK "-> DOCUMENT, unique"
        string name "denormalise depuis DOCUMENT.title — sync via DocumentTitleUpdated (synchrone)"
        enum status "ALIVE | DEAD | MISSING | UNKNOWN"
        uuid linkedCharacterId "ref PLAYER_CHARACTER, nullable"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    PLAYER_CHARACTER {
        uuid id PK
        uuid campaignId "ref Campaign Management"
        uuid documentId FK "-> DOCUMENT, unique"
        string name "denormalise depuis DOCUMENT.title — sync via DocumentTitleUpdated (synchrone)"
        uuid ownerId "ref Identity, nullable = en attente ou personnage MJ"
        uuid linkedNpcId "ref NPC, nullable"
        enum status "ACTIVE | RETIRED | DEAD"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    SCENARIO {
        uuid id PK
        uuid campaignId "ref Campaign Management"
        uuid documentId FK "-> DOCUMENT, unique"
        string title "denormalise depuis DOCUMENT.title — sync via DocumentTitleUpdated (synchrone)"
        string slug "unique par campaignId"
        int order "position dans la campagne — sync avec ordre dossier Scenarios par defaut"
        enum status "DRAFT | READY | PLAYED | ARCHIVED"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    SCENE {
        uuid id PK
        uuid scenarioId FK "-> SCENARIO"
        uuid documentId FK "-> DOCUMENT, unique"
        string title "denormalise depuis DOCUMENT.title — sync via DocumentTitleUpdated (synchrone)"
        int order "position dans le scenario"
        enum status "PENDING | PLAYED | SKIPPED"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
    }
    SCENE_NPC {
        uuid sceneId FK "-> SCENE"
        uuid npcId "ref NPC"
    }
    NPC ||--|| DOCUMENT : "possede"
    PLAYER_CHARACTER ||--|| DOCUMENT : "possede"
    SCENARIO ||--|| DOCUMENT : "possede"
    SCENARIO ||--o{ SCENE : "ordonne"
    SCENE ||--|| DOCUMENT : "possede"
    SCENE ||--o{ SCENE_NPC : "lie a"
```

### Notes Content Library

- `NPC.documentId` et `PLAYER_CHARACTER.documentId` ont une contrainte unique —
  un document ne peut appartenir qu'a une seule entite metier.
- **Co-création obligatoire** : NPC, PLAYER_CHARACTER, SCENARIO et SCENE sont toujours
  créés via leur factory domaine, jamais par création directe d'un DOCUMENT.
- `NPC.name`, `PLAYER_CHARACTER.name`, `SCENARIO.title`, `SCENE.title` sont des champs
  dénormalisés depuis `DOCUMENT.title`. Mis à jour via le domain event `DocumentTitleUpdated`
  dispatchés synchrone in-process dans la même transaction.
- `NPC.linkedCharacterId` et `PLAYER_CHARACTER.linkedNpcId` sont des références
  sans FK en base — association narrative, cycle de vie indépendant.
- Soft delete `NPC` ou `PLAYER_CHARACTER` : mettre `isDeleted = true` sur l'entité
  ET sur son `DOCUMENT` associé dans la même transaction.
- Soft delete `SCENARIO` : mettre `isDeleted = true` sur le scénario ET sur son `DOCUMENT`,
  puis supprimer physiquement les `SCENE` et mettre `isDeleted = true` sur leurs `DOCUMENT`.
- Suppression d'un `DOCUMENT` (soft delete) : supprimer physiquement tous ses `DOCUMENT_BLOCK`.
- `SCENE_NPC` : si un NPC est soft-deleted, supprimer physiquement les lignes correspondantes.
- `DOCUMENT_TEMPLATE` avec `scope = BUILTIN` : pas de modification possible,
  protégé applicativement. `version` commence à 1.
- Contraintes de scope sur `DOCUMENT_TEMPLATE` :
  - `BUILTIN` → `ownerId IS NULL AND campaignId IS NULL`
  - `CAMPAIGN` → `ownerId IS NOT NULL AND campaignId IS NOT NULL`
  - `USER` → `ownerId IS NOT NULL AND campaignId IS NULL`
- **Backlinks** : la requête `SELECT documentId FROM DOCUMENT_BLOCK WHERE kind = 'RELATION'
  AND value->>'targetId' = :targetId` filtre `DOCUMENT.isDeleted = false` via jointure.
  Les backlinks pointant vers des documents supprimés ne sont pas affichés.
- Index recommandés : `DOCUMENT(campaignId, type)`, `DOCUMENT(campaignId, type, slug)` unique,
  `DOCUMENT_BLOCK(documentId, order)`, `NPC(campaignId)`, `PLAYER_CHARACTER(campaignId)`,
  `PLAYER_CHARACTER(ownerId)`, `SCENARIO(campaignId)`, `SCENARIO(campaignId, order)`,
  `SCENE(scenarioId, order)`.
- Index GIN recommandé sur `DOCUMENT_BLOCK.value` pour les requêtes dans le JSON.
- Index GIN recommandé sur `DOCUMENT_BLOCK.value->>'targetId'` pour les backlinks.
- Index GIN recommandé sur `DOCUMENT.title` et `DOCUMENT_BLOCK.value` pour la recherche FTS
  (PostgreSQL `tsvector`, configuration linguistique : `french`).

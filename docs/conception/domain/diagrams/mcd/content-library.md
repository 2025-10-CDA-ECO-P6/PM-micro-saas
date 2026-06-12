# Content Library — Modèle Conceptuel de Données (MCD)

```mermaid
erDiagram
    FOLDER {
        identifiant id PK
        identifiant space_id "ref Space Management"
        identifiant parent_folder_id FK "nullable — self-ref"
        texte name
        booléen is_system
        booléen is_virtual
        identifiant default_document_type_id FK "nullable"
        identifiant default_template_document_id FK "nullable — ref documents.id"
        horodatage created_at
        horodatage updated_at
        identifiant created_by_id "ref users.id"
    }

    DOCUMENT_TYPE {
        identifiant id PK
        texte slug "unique — ex: scenario, scene, npc"
        texte name
        structure properties_schema "nullable"
        booléen is_system
        identifiant space_id "nullable — null pour types système"
        horodatage created_at
    }

    DOCUMENT {
        identifiant id PK
        identifiant space_id "ref Space Management"
        identifiant folder_id FK
        texte title
        identifiant document_type_id FK "nullable"
        structure properties "nullable"
        texte visibility "PUBLIC | GM_ONLY | PLAYER_PRIVATE"
        texte slug
        booléen is_reusable
        identifiant source_document_id FK "nullable — self-ref"
        booléen is_deleted
        horodatage deleted_at "nullable"
        horodatage created_at
        horodatage updated_at
        identifiant created_by_id "ref users.id"
    }

    DOCUMENT_BLOCK {
        identifiant id PK
        identifiant document_id FK
        entier order
        texte type "TEXT | TABLE | IMAGE | DIVIDER"
        structure content
        booléen is_locked
    }

    DOCUMENT_LINK {
        identifiant source_document_id PK,FK
        identifiant target_document_id PK,FK
        entier order
    }

    DOCUMENT_TAG {
        identifiant document_id PK,FK
        texte tag PK
    }

    FOLDER ||--o{ DOCUMENT : "contient"
    FOLDER ||--o{ FOLDER : "parent de"
    DOCUMENT_TYPE ||--o{ DOCUMENT : "type de"
    DOCUMENT_TYPE ||--o{ FOLDER : "type par défaut de"
    DOCUMENT ||--o{ DOCUMENT_BLOCK : "composé de"
    DOCUMENT ||--o{ DOCUMENT_LINK : "référence"
    DOCUMENT ||--o{ DOCUMENT_TAG : "taggé par"
    DOCUMENT ||--o{ DOCUMENT : "instancié depuis (source)"
```

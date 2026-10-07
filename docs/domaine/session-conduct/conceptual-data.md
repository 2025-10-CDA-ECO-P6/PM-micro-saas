# Session Conduct — Modèle Conceptuel de Données (MCD)

```mermaid
erDiagram
    SESSION {
        identifiant id PK
        identifiant space_id "ref Space Management"
        texte title
        texte status "LIVE | CLOSED | ARCHIVED"
        identifiant scenario_id "nullable — ref Content Library"
        texte summary "nullable"
        horodatage started_at
        horodatage closed_at "nullable"
        horodatage created_at
        horodatage updated_at
        identifiant created_by_id "ref users.id"
    }

    SESSION_PINNED_DOCUMENT {
        identifiant session_id PK,FK
        identifiant document_id PK "ref Content Library"
    }

    SESSION_LIVE_NOTE {
        identifiant session_id PK,FK
        identifiant document_id PK "ref Content Library — type LIVE_NOTE"
    }

    SESSION_VIEW_CONFIG {
        identifiant id PK
        identifiant space_id "UNIQUE — ref Space Management"
        horodatage updated_at
    }

    SESSION_VIEW_FOLDER {
        identifiant session_view_config_id PK,FK
        identifiant folder_id PK "ref Content Library"
        entier order
    }

    SESSION ||--o{ SESSION_PINNED_DOCUMENT : "épingle"
    SESSION ||--o{ SESSION_LIVE_NOTE : "référence"
    SESSION_VIEW_CONFIG ||--o{ SESSION_VIEW_FOLDER : "configure"
```

# Space Management — Modèle Conceptuel de Données (MCD)

```mermaid
erDiagram
    SPACE {
        identifiant id PK
        identifiant owner_id FK "ref users.id"
        texte name
        texte slug "unique par owner"
        texte type "CAMPAIGN | ONE_SHOT | PERSONAL"
        texte status "ACTIVE | ARCHIVED | FROZEN"
        horodatage created_at
        horodatage updated_at
    }

    SPACE_MEMBERSHIP {
        identifiant space_id PK,FK
        identifiant user_id PK,FK "ref users.id"
        texte role "OWNER | GM | PLAYER"
        texte status "PENDING | ACTIVE | REMOVED"
        horodatage joined_at
    }

    MEMBERSHIP_CHARACTER {
        identifiant space_id PK,FK
        identifiant user_id PK,FK
        identifiant character_id PK "ref Content Library"
    }

    INVITATION {
        identifiant id PK
        identifiant space_id FK
        texte token "UUID unique"
        texte type "LINK | EMAIL"
        texte scope "CAMPAIGN | SESSION"
        identifiant session_id "nullable, ref Session Conduct"
        horodatage expires_at "nullable"
        entier max_uses "nullable"
        entier used_count
        texte status "ACTIVE | REVOKED | EXPIRED"
        horodatage created_at
    }

    GUEST_ACCESS {
        identifiant id PK
        identifiant space_id FK
        texte scope "SESSION | CAMPAIGN"
        identifiant session_id "nullable, ref Session Conduct"
        texte token "UUID unique"
        texte display_name
        identifiant character_id "nullable, ref Content Library"
        texte status "ACTIVE | EXPIRED | REVOKED | CONVERTED"
        horodatage expires_at "nullable"
        horodatage created_at
    }

    %% post-MVP
    SCENARIO_LIBRARY_ENTRY {
        identifiant id PK
        identifiant owner_id FK "ref users.id"
        identifiant document_id FK "ref documents.id (Content Library)"
        horodatage promoted_at
    }

    SPACE ||--o{ SPACE_MEMBERSHIP : "a des membres"
    SPACE ||--o{ INVITATION : "a des invitations"
    SPACE ||--o{ GUEST_ACCESS : "a des accès invités"
    SPACE_MEMBERSHIP ||--o{ MEMBERSHIP_CHARACTER : "associé à des personnages"
    SCENARIO_LIBRARY_ENTRY }o--|| USER : "appartient à"
    SCENARIO_LIBRARY_ENTRY }o--|| DOCUMENT : "promeut"
```

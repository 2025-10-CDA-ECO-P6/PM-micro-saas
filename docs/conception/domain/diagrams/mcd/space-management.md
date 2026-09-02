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
        texte scope "SPACE | SESSION"
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
        texte scope "SESSION | SPACE"
        identifiant session_id "nullable, ref Session Conduct"
        texte token "UUID unique"
        texte display_name
        identifiant character_id "nullable, ref Content Library"
        texte status "ACTIVE | EXPIRED | REVOKED | CONVERTED"
        horodatage expires_at "nullable"
        horodatage created_at
    }

    SPACE ||--o{ SPACE_MEMBERSHIP : "a des membres"
    SPACE ||--o{ INVITATION : "a des invitations"
    SPACE ||--o{ GUEST_ACCESS : "a des accès invités"
    SPACE_MEMBERSHIP ||--o{ MEMBERSHIP_CHARACTER : "associé à des personnages"
```

> **Pas d'agrégat de pont `ScenarioLibrary`/`ScenarioLibraryEntry`** : la réutilisabilité est portée par `Document.isReusable` (Content Library) ; la bibliothèque personnelle du MJ est une vue filtrée de son espace `PERSONAL` (`type = PERSONAL` ∧ `isReusable = true`) — voir space-management.md § Scénario réutilisable (invariant 12, retiré) et ADR-018.

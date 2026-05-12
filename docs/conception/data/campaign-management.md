# MCD — Campaign Management

Contexte organisationnel. Gère le cycle de vie des campagnes, des membres,
des invitations, des accès invités et des autorisations de contenu.

```mermaid
erDiagram
    CAMPAIGN {
        uuid id PK
        uuid ownerId FK "-> USER"
        string name
        string slug "unique par ownerId"
        string description
        uuid gameSystemId FK "-> GAME_SYSTEM, nullable"
        enum status "DRAFT | ACTIVE | PAUSED | ARCHIVED"
        datetime createdAt
        datetime updatedAt
        uuid createdById FK
        uuid updatedById FK
        boolean isDeleted
        datetime deletedAt
        uuid deletedById FK
    }
    CAMPAIGN_MEMBERSHIP {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        uuid userId FK "-> USER"
        enum role "OWNER | PLAYER"
        enum status "PENDING | ACTIVE | REMOVED"
        datetime joinedAt
        datetime createdAt
        datetime updatedAt
        uuid createdById FK
        uuid updatedById FK
    }
    INVITATION {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        string token "unique, genere aleatoirement"
        enum type "LINK | EMAIL"
        datetime expiresAt "nullable = pas d'expiration"
        int maxUses "nullable = illimite"
        int useCount "default 0"
        enum status "ACTIVE | EXPIRED | REVOKED"
        datetime createdAt
        datetime updatedAt
        uuid createdById FK
        uuid updatedById FK
    }
    GUEST_ACCESS {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        uuid invitationId FK "-> INVITATION"
        string displayName "pseudo choisi par l'invite"
        string accessToken "valide en infra uniquement"
        uuid characterId "ref Content Library, nullable"
        datetime expiresAt "nullable"
        enum status "ACTIVE | EXPIRED"
        datetime createdAt
    }
    GAME_SYSTEM {
        uuid id PK
        string name
        string slug
        string description
        boolean isBuiltIn "true = fourni par l'app, non modifiable"
        uuid ownerId "ref Identity, null si BUILTIN"
        datetime createdAt
        datetime updatedAt
        uuid createdById FK
        uuid updatedById FK
        boolean isDeleted
        datetime deletedAt
        uuid deletedById FK
    }
    CONTENT_ACCESS_RULE {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        enum resourceType "DOCUMENT | LIVE_NOTE | SESSION_SUMMARY"
        uuid resourceId "ref ressource partageable"
        uuid grantedById FK "-> USER"
        enum targetType "ALL | MEMBER | CHARACTER"
        uuid targetId "nullable si targetType = ALL"
        datetime createdAt
    }
    CAMPAIGN ||--|{ CAMPAIGN_MEMBERSHIP : "contient"
    CAMPAIGN ||--o{ INVITATION : "genere"
    CAMPAIGN ||--o{ GUEST_ACCESS : "accueille"
    CAMPAIGN ||--o{ CONTENT_ACCESS_RULE : "definit les acces"
    CAMPAIGN }o--o| GAME_SYSTEM : "utilise"
    CAMPAIGN_MEMBERSHIP }|--|| USER : "lie a"
    INVITATION ||--o{ GUEST_ACCESS : "cree un"
```

### Notes Campaign Management

- `CAMPAIGN` a exactement un membre avec `role = OWNER` — invariant garanti applicativement.
- `CAMPAIGN_MEMBERSHIP.userId` est toujours renseigné — les accès sans compte utilisent `GUEST_ACCESS`.
- `INVITATION.status` passe automatiquement à `EXPIRED` quand `useCount >= maxUses`
  ou quand `expiresAt` est dépassé — logique applicative.
- `CONTENT_ACCESS_RULE` est immuable : pas de `updatedAt`, pas de soft delete.
  La révocation est une suppression physique.
- `CONTENT_ACCESS_RULE.resourceId` est une référence cross-context sans FK en base.
- `CONTENT_ACCESS_RULE` : index unique sur `(resourceType, resourceId, targetType, targetId)` avec
  `NULLS NOT DISTINCT` pour gérer le cas `targetType = ALL` (targetId = NULL).
- `GAME_SYSTEM.ownerId` : null pour les systèmes BUILTIN, non-null pour les systèmes custom (scope privé MVP).
- Index recommandés : `CAMPAIGN(ownerId)`, `CAMPAIGN_MEMBERSHIP(campaignId, userId)`,
  `INVITATION(token)` unique, `CONTENT_ACCESS_RULE(campaignId, documentId)`.

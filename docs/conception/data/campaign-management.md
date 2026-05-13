# MCD — Campaign Management

Contexte organisationnel. Gère le cycle de vie des campagnes, des membres,
des invitations, des accès invités et des autorisations de contenu.

```mermaid
erDiagram
    CAMPAIGN {
        uuid id PK
        uuid ownerId "ref Identity"
        string name
        string slug "unique par ownerId"
        string description
        uuid gameSystemId "ref GAME_SYSTEM, nullable"
        enum type "CAMPAIGN | ONE_SHOT"
        enum status "DRAFT | ACTIVE | PAUSED | ARCHIVED"
        boolean autoArchiveEnabled "default true — si false, désactive l'archivage auto des ONE_SHOT"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    CAMPAIGN_MEMBERSHIP {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        uuid userId "ref Identity"
        enum role "OWNER | PLAYER"
        enum status "PENDING | ACTIVE | REMOVED"
        datetime joinedAt
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
    }
    INVITATION {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        enum scope "CAMPAIGN | SESSION"
        uuid sessionId "ref Session Conduct, nullable — requis si scope = SESSION"
        string token "unique, genere aleatoirement"
        enum type "LINK | EMAIL"
        datetime expiresAt "nullable = pas d'expiration"
        int maxUses "nullable = illimite"
        int useCount "default 0"
        enum status "ACTIVE | EXPIRED | REVOKED"
        datetime createdAt
        datetime updatedAt
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
    }
    GUEST_ACCESS {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        uuid invitationId FK "-> INVITATION"
        enum scope "CAMPAIGN | SESSION"
        uuid sessionId "ref Session Conduct, nullable"
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
        uuid createdById "ref Identity"
        uuid updatedById "ref Identity"
        boolean isDeleted
        datetime deletedAt
        uuid deletedById "ref Identity"
    }
    CONTENT_ACCESS_RULE {
        uuid id PK
        uuid campaignId FK "-> CAMPAIGN"
        enum resourceType "DOCUMENT | LIVE_NOTE"
        uuid resourceId "ref ressource partageable"
        uuid grantedByUserId "ref Identity, nullable"
        uuid grantedByGuestAccessId "ref GuestAccess, nullable"
        enum targetType "ALL | MEMBER | CHARACTER | SESSION"
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

- `CAMPAIGN.type` : `CAMPAIGN` pour une campagne longue, `ONE_SHOT` pour une partie unique. Même modèle de données, parcours utilisateur différent.
- Archivage automatique `ONE_SHOT` : une campagne `ONE_SHOT` passe à `ARCHIVED` 48h après la fin de sa dernière session si `autoArchiveEnabled = true` et si le MJ n'a pas rouvert la campagne. Déclenché par un scheduler applicatif — `Campaign.AutoArchive()` en domaine.
- `CAMPAIGN` a exactement un membre avec `role = OWNER` — invariant garanti applicativement.
- `CAMPAIGN_MEMBERSHIP.userId` est toujours renseigné — les accès sans compte utilisent `GUEST_ACCESS`.
- `INVITATION.status` passe automatiquement à `EXPIRED` quand `useCount >= maxUses`
  ou quand `expiresAt` est dépassé — logique applicative.
- `INVITATION.scope = SESSION` impose `sessionId` et produit un `GUEST_ACCESS` limité à cette session.
- `INVITATION.scope = CAMPAIGN` impose `sessionId IS NULL`.
- `CONTENT_ACCESS_RULE` est immuable : pas de `updatedAt`, pas de soft delete.
  La révocation est une suppression physique.
- `CONTENT_ACCESS_RULE.resourceId` est une référence cross-context sans FK en base.
- `CONTENT_ACCESS_RULE` renseigne exactement un auteur de partage :
  `grantedByUserId` pour un MJ ou joueur authentifié, `grantedByGuestAccessId` pour un invité.
- `CONTENT_ACCESS_RULE` : index unique sur `(resourceType, resourceId, targetType, targetId)` avec
  `NULLS NOT DISTINCT` pour gérer le cas `targetType = ALL` (targetId = NULL).
- `GAME_SYSTEM.ownerId` : null pour les systèmes BUILTIN, non-null pour les systèmes custom (scope privé MVP).
- Index recommandés : `CAMPAIGN(ownerId)`, `CAMPAIGN_MEMBERSHIP(campaignId, userId)`,
  `INVITATION(token)` unique, `INVITATION(scope, sessionId)`,
  `GUEST_ACCESS(campaignId, sessionId)`, `CONTENT_ACCESS_RULE(campaignId, resourceType, resourceId)`.

# Diagramme de classes — Campaign Management

Contexte organisationnel. `Campaign` est l'agrégat racine central — il encapsule
ses membres (`CampaignMembership`) et ses invitations (`Invitation`) dont le cycle
de vie lui est entièrement lié.

`GuestAccess` est une entité avec repository distincte de `CampaignMembership` :
un invité sans compte n'est pas un membre au sens plein — c'est un accès temporaire
créé depuis une invitation, sans identité persistante dans le système.
Du point de vue de la visibilité, un GuestAccess actif est traité comme un joueur ordinaire
sur le périmètre de son personnage : PUBLIC, SHARED et PLAYER_PRIVATE si le `characterId`
correspond.

`AccessPolicy` est un service domaine, pas un agrégat. Il persiste des `ContentAccessRule`
immuables. Sa méthode `CanAccess` accepte un `RequesterId` (Shared Kernel) pour gérer
uniformément authentifiés et invités.

`GameSystem` est un agrégat léger indépendant — point d'extension futur pour les règles
de jeu. Référencé optionnellement par une campagne.

```mermaid
classDiagram
    direction TB
    class Campaign {
        <<aggregate root>>
        +id: CampaignId
        +ownerId: UserId
        +name: String
        +slug: Slug
        +description: String?
        +gameSystemId: GameSystemId?
        +status: CampaignStatus
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class CampaignMembership {
        <<entity>>
        +id: MembershipId
        +campaignId: CampaignId
        +userId: UserId
        +role: MemberRole
        +status: MembershipStatus
        +joinedAt: DateTime?
        +audit: AuditInfo
    }
    class Invitation {
        <<entity>>
        +id: InvitationId
        +campaignId: CampaignId
        +token: String
        +type: InvitationType
        +expiresAt: DateTime?
        +maxUses: Int?
        +useCount: Int
        +status: InvitationStatus
        +audit: AuditInfo
    }
    class GuestAccess {
        <<entity — repository>>
        +id: GuestAccessId
        +campaignId: CampaignId
        +displayName: String
        +accessToken: String
        +characterId: CharacterId?
        +expiresAt: DateTime?
        +status: GuestAccessStatus
        +createdAt: DateTime
    }
    class GameSystem {
        <<aggregate root>>
        +id: GameSystemId
        +name: String
        +slug: Slug
        +description: String?
        +isBuiltIn: Boolean
        +ownerId: UserId?
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class ContentAccessRule {
        <<entity — AccessPolicy>>
        +id: AccessRuleId
        +campaignId: CampaignId
        +resource: ShareableResourceRef
        +grantedById: UserId
        +target: AccessTarget
        +createdAt: DateTime
    }
    class AccessPolicy {
        <<domain service>>
        +CanAccess(resource, requester: RequesterId) bool
    }
    class AccessTarget {
        <<value object sealed>>
    }
    class AllMembersTarget {
        <<value object>>
    }
    class SpecificMemberTarget {
        <<value object>>
        +userId: UserId
    }
    class SpecificCharacterTarget {
        <<value object>>
        +characterId: CharacterId
    }
    class CampaignStatus {
        <<enumeration>>
        DRAFT
        ACTIVE
        PAUSED
        ARCHIVED
    }
    class MemberRole {
        <<enumeration>>
        OWNER
        PLAYER
    }
    class MembershipStatus {
        <<enumeration>>
        PENDING
        ACTIVE
        REMOVED
    }
    class InvitationType {
        <<enumeration>>
        LINK
        EMAIL
    }
    class InvitationStatus {
        <<enumeration>>
        ACTIVE
        EXPIRED
        REVOKED
    }
    class GuestAccessStatus {
        <<enumeration>>
        ACTIVE
        EXPIRED
    }
    Campaign "1" *-- "0..*" CampaignMembership
    Campaign "1" *-- "0..*" Invitation
    Campaign --> CampaignStatus
    Campaign ..> GameSystem : gameSystemId
    CampaignMembership --> MemberRole
    CampaignMembership --> MembershipStatus
    Invitation --> InvitationType
    Invitation --> InvitationStatus
    Invitation ..> GuestAccess : crée un GuestAccess à l'usage
    GuestAccess --> GuestAccessStatus
    GuestAccess ..> Campaign : campaignId
    AccessPolicy --> ContentAccessRule : persiste
    ContentAccessRule *-- AccessTarget
    ContentAccessRule --> Campaign
    AccessTarget <|-- AllMembersTarget
    AccessTarget <|-- SpecificMemberTarget
    AccessTarget <|-- SpecificCharacterTarget
    note for CampaignMembership "userId toujours renseigné\nLes invités sans compte\nutilisent GuestAccess"
    note for ContentAccessRule "Géré par AccessPolicy\nservice domaine\nRévocation = suppression physique\nid: AccessRuleId (dans Shared Kernel)"
    note for ContentAccessRule "resource = Document, LiveNote ou SessionSummary\nref cross-context par Id"
    note for AccessTarget "Hiérarchie scellée — même principe que BlockValue\nAllMembersTarget : authentifiés membres + GuestAccess actifs\nIndex unique : (resource, sous-type, targetId?) NULLS NOT DISTINCT"
    note for AccessPolicy "CanAccess accepte un RequesterId\n(AuthenticatedRequesterId ou GuestRequesterId)\njamais un UserId nu"
```

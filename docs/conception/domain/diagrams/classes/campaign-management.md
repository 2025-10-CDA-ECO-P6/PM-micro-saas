# Campaign Management — Diagramme de classes

```mermaid
classDiagram
    class Campaign {
        +CampaignId id
        +UserId ownerId
        +string name
        +Slug slug
        +CampaignType type
        +CampaignStatus status
        +List~CampaignMembership~ memberships
        +List~Invitation~ invitations
        +DateTime createdAt
        +DateTime updatedAt
        +Create(ownerId, name, type)$ Campaign
        +AddMember(userId, role) MemberJoined
        +RemoveMember(userId) MemberRemoved
        +CreateInvitation(type, scope, options) Invitation
        +RevokeInvitation(invitationId) void
        +AssociateCharacter(userId, characterId) void
        +Archive() CampaignArchived
        +Freeze() CampaignFrozen
        +Unfreeze() CampaignUnfrozen
    }

    class CampaignMembership {
        +UserId userId
        +MemberRole role
        +MembershipStatus status
        +List~CharacterId~ characterIds
        +DateTime joinedAt
    }

    class Invitation {
        +InvitationId id
        +string token
        +InvitationType type
        +InvitationScope scope
        +SessionId sessionId
        +DateTime expiresAt
        +int maxUses
        +int usedCount
        +InvitationStatus status
        +DateTime createdAt
        +Use() void
        +Revoke() void
    }

    class GuestAccess {
        +GuestAccessId id
        +CampaignId campaignId
        +GuestAccessScope scope
        +SessionId sessionId
        +string token
        +string displayName
        +CharacterId characterId
        +GuestAccessStatus status
        +DateTime expiresAt
        +DateTime createdAt
        +Create(campaignId, scope, sessionId)$ GuestAccess
        +SetDisplayName(name) void
        +AssociateCharacter(characterId) void
        +Expire() GuestAccessExpired
        +Revoke() GuestAccessRevoked
        +Convert(userId) GuestAccessConverted
    }

    class CampaignType {
        <<enumeration>>
        CAMPAIGN
        ONE_SHOT
    }

    class CampaignStatus {
        <<enumeration>>
        ACTIVE
        ARCHIVED
        FROZEN
    }

    class MemberRole {
        <<enumeration>>
        OWNER
        GM
        PLAYER
    }

    class MembershipStatus {
        <<enumeration>>
        PENDING
        ACTIVE
        REMOVED
    }

    class InvitationScope {
        <<enumeration>>
        CAMPAIGN
        SESSION
    }

    class GuestAccessStatus {
        <<enumeration>>
        ACTIVE
        EXPIRED
        REVOKED
        CONVERTED
    }

    Campaign "1" *-- "1..*" CampaignMembership : memberships
    Campaign "1" *-- "0..*" Invitation : invitations
    Campaign --> CampaignType
    Campaign --> CampaignStatus
    CampaignMembership --> MemberRole
    CampaignMembership --> MembershipStatus
    Invitation --> InvitationScope
    GuestAccess --> GuestAccessStatus
    GuestAccess --> CampaignId
```

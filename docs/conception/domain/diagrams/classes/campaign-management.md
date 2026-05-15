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
        +Create(ownerId, name, type)$ CampaignCreated
        +AddMember(userId, role) MemberJoined
        +RemoveMember(userId) MemberRemoved
        +CreateInvitation(type, scope, options) InvitationCreated
        +RevokeInvitation(invitationId) InvitationRevoked
        +AssociateCharacter(userId, characterId) CharacterAssociated
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
        +Activate() MemberActivated
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
        +Create(campaignId, scope, sessionId)$ GuestAccessCreated
        +SetDisplayName(name) void
        +AssociateCharacter(characterId) void
        +Expire() GuestAccessExpired
        +Revoke() GuestAccessRevoked
        +Convert(userId) GuestAccessConvertedToMember
    }

    class CampaignCreated {
        <<domainEvent>>
        +CampaignId campaignId
        +UserId ownerId
        +CampaignType type
    }

    class MemberJoined {
        <<domainEvent>>
        +CampaignId campaignId
        +UserId userId
        +MemberRole role
    }

    class MemberActivated {
        <<domainEvent>>
        +CampaignId campaignId
        +UserId userId
    }

    class MemberRemoved {
        <<domainEvent>>
        +CampaignId campaignId
        +UserId userId
    }

    class InvitationCreated {
        <<domainEvent>>
        +CampaignId campaignId
        +InvitationId invitationId
        +string token
    }

    class InvitationRevoked {
        <<domainEvent>>
        +CampaignId campaignId
        +InvitationId invitationId
    }

    class CharacterAssociated {
        <<domainEvent>>
        +CampaignId campaignId
        +UserId userId
        +CharacterId characterId
    }

    class CampaignArchived {
        <<domainEvent>>
        +CampaignId campaignId
    }

    class CampaignFrozen {
        <<domainEvent>>
        +CampaignId campaignId
    }

    class CampaignUnfrozen {
        <<domainEvent>>
        +CampaignId campaignId
    }

    class GuestAccessCreated {
        <<domainEvent>>
        +GuestAccessId guestAccessId
        +CampaignId campaignId
        +string token
    }

    class GuestAccessExpired {
        <<domainEvent>>
        +GuestAccessId guestAccessId
    }

    class GuestAccessRevoked {
        <<domainEvent>>
        +GuestAccessId guestAccessId
    }

    class GuestAccessConvertedToMember {
        <<domainEvent>>
        +GuestAccessId guestAccessId
        +UserId userId
        +CampaignId campaignId
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

    Campaign ..> CampaignCreated : produces
    Campaign ..> MemberJoined : produces
    Campaign ..> MemberRemoved : produces
    Campaign ..> InvitationCreated : produces
    Campaign ..> InvitationRevoked : produces
    Campaign ..> CharacterAssociated : produces
    Campaign ..> CampaignArchived : produces
    Campaign ..> CampaignFrozen : produces
    Campaign ..> CampaignUnfrozen : produces
    CampaignMembership ..> MemberActivated : produces
    GuestAccess ..> GuestAccessCreated : produces
    GuestAccess ..> GuestAccessExpired : produces
    GuestAccess ..> GuestAccessRevoked : produces
    GuestAccess ..> GuestAccessConvertedToMember : produces
```

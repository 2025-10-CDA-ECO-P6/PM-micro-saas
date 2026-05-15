# Identity & Access — Diagramme de classes

```mermaid
classDiagram
    class User {
        +UserId id
        +Email email
        +string displayName
        +AccountStatus status
        +AccountTier tier
        +DateTime createdAt
        +DateTime updatedAt
        +Register(email, displayName)$ User
        +UpdateDisplayName(name) DisplayNameUpdated
        +ChangeTier(tier) AccountTierChanged
        +Delete() UserDeleted
        +Anonymize() UserAnonymized
        +Suspend() AccountSuspended
    }

    class AccountStatus {
        <<enumeration>>
        ACTIVE
        SUSPENDED
        DELETED
    }

    class AccountTier {
        <<enumeration>>
        FREE
        PRO
    }

    class UserRegistered {
        +userId: UserId
        +email: Email
        +occurredAt: DateTime
    }

    class AccountTierChanged {
        +userId: UserId
        +previousTier: AccountTier
        +newTier: AccountTier
        +occurredAt: DateTime
    }

    class UserDeleted {
        +userId: UserId
        +occurredAt: DateTime
    }

    class UserAnonymized {
        +userId: UserId
        +occurredAt: DateTime
    }

    class DisplayNameUpdated {
        +userId: UserId
        +newDisplayName: string
        +occurredAt: DateTime
    }

    class AccountSuspended {
        +userId: UserId
        +occurredAt: DateTime
    }

    User --> AccountStatus
    User --> AccountTier
    User ..> UserRegistered : produces
    User ..> DisplayNameUpdated : produces
    User ..> AccountTierChanged : produces
    User ..> AccountSuspended : produces
    User ..> UserDeleted : produces
    User ..> UserAnonymized : produces

    note for User "Shadow entity — partage son id\navec AspNetUsers (ASP.NET Identity)"
```

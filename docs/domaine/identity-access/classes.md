# Identity & Access — Diagramme de classes

## Diagramme

```mermaid
classDiagram
    class User {
        +identifiant id
        +Email email
        +booléen emailVerified
        +texte displayName
        +AccountStatus status
        +AccountTier tier
        +date createdAt
        +date updatedAt
        +Register(email, displayName)$
        +VerifyEmail()
        +UpdateDisplayName(name)
        +ChangeEmail(newEmail)
        +LinkFederatedIdentity(provider, externalId)
        +SetInitialPassword()
        +ChangeTier(tier)
        +Delete()
        +Anonymize()
        +Suspend()
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
        <<domainEvent>>
        +identifiant userId
        +Email email
    }

    class AccountTierChanged {
        <<domainEvent>>
        +identifiant userId
        +AccountTier previousTier
        +AccountTier newTier
    }

    class UserDeleted {
        <<domainEvent>>
        +identifiant userId
    }

    class UserAnonymized {
        <<domainEvent>>
        +identifiant userId
    }

    class DisplayNameUpdated {
        <<domainEvent>>
        +identifiant userId
        +texte newDisplayName
    }

    class EmailVerified {
        <<domainEvent>>
        +identifiant userId
    }

    class EmailChangeRequested {
        <<domainEvent>>
        +identifiant userId
        +Email newEmail
    }

    class FederatedIdentityLinked {
        <<domainEvent>>
        +identifiant userId
        +texte provider
        +texte externalId
    }

    class InitialPasswordSet {
        <<domainEvent>>
        +identifiant userId
    }

    class AccountSuspended {
        <<domainEvent>>
        +identifiant userId
    }

    User --> AccountStatus
    User --> AccountTier
    User ..> UserRegistered : produces
    User ..> EmailVerified : produces
    User ..> DisplayNameUpdated : produces
    User ..> EmailChangeRequested : produces
    User ..> FederatedIdentityLinked : produces
    User ..> InitialPasswordSet : produces
    User ..> AccountTierChanged : produces
    User ..> AccountSuspended : produces
    User ..> UserDeleted : produces
    User ..> UserAnonymized : produces
```

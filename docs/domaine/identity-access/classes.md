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
        +horodatage occurredAt
    }

    class AccountTierChanged {
        <<domainEvent>>
        +identifiant userId
        +AccountTier previousTier
        +AccountTier newTier
        +horodatage occurredAt
    }

    class UserDeleted {
        <<domainEvent>>
        +identifiant userId
        +horodatage occurredAt
    }

    class UserAnonymized {
        <<domainEvent>>
        +identifiant userId
        +horodatage occurredAt
    }

    class DisplayNameUpdated {
        <<domainEvent>>
        +identifiant userId
        +texte newDisplayName
        +horodatage occurredAt
    }

    class EmailVerified {
        <<domainEvent>>
        +identifiant userId
        +horodatage occurredAt
    }

    class EmailChangeRequested {
        <<domainEvent>>
        +identifiant userId
        +Email newEmail
        +horodatage occurredAt
    }

    class FederatedIdentityLinked {
        <<domainEvent>>
        +identifiant userId
        +texte provider
        +texte externalId
        +horodatage occurredAt
    }

    class InitialPasswordSet {
        <<domainEvent>>
        +identifiant userId
        +horodatage occurredAt
    }

    class AccountSuspended {
        <<domainEvent>>
        +identifiant userId
        +horodatage occurredAt
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

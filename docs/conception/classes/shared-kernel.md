# Diagramme de classes — Shared Kernel — RequesterId

`RequesterId` est un type union scellé du Shared Kernel permettant à `AccessPolicy`
de traiter uniformément les utilisateurs authentifiés et les invités sans compte.
Ce type résout le problème d'autorisation des GuestAccess qui n'ont pas de UserId.

```mermaid
classDiagram
    direction TB
    class RequesterId {
        <<value object sealed>>
    }
    class AuthenticatedRequesterId {
        <<value object>>
        +userId: UserId
        +characterId: CharacterId?
    }
    class GuestRequesterId {
        <<value object>>
        +guestAccessId: GuestAccessId
        +characterId: CharacterId?
    }
    RequesterId <|-- AuthenticatedRequesterId
    RequesterId <|-- GuestRequesterId
    note for RequesterId "Utilisé par AccessPolicy.CanAccess\nSeul type passé à l'autorisation\nJamais de UserId nu dans AccessPolicy"
    note for AuthenticatedRequesterId "characterId = personnage courant dans la campagne\nUn même user peut avoir plusieurs personnages"
    note for GuestRequesterId "characterId permet la résolution\nde SpecificCharacterTarget et PLAYER_PRIVATE\nMême sans UserId"
```

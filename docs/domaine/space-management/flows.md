# Space Management — Diagrammes de flux

## Flux 1 — Créer un espace

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SM as Space Management
    participant CL as Content Library

    MJ->>App: Créer un espace (name, type)
    App->>App: Vérifier quota FREE (≤ 3 espaces actifs CAMPAIGN/ONE_SHOT) — sans objet pour type = PERSONAL, hors quota
    alt Quota dépassé
        App-->>MJ: Bloqué — upgrade requis
    else Quota OK
        App->>SM: Space.Create(ownerId, name, type, description?, gameSystem?)
        SM-->>App: SpaceCreated event
        alt type = CAMPAIGN ou ONE_SHOT
            App->>CL: Créer les dossiers système (Personnages, Joueurs, Scénarios, Notes) + dossier virtuel (Non classés)
        else type = PERSONAL
            App->>CL: Créer uniquement le dossier virtuel (Non classés)
        end
        App-->>MJ: Espace créé
    end
```

## Flux 2 — Inviter un joueur (via lien)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SM as Space Management

    MJ->>App: Générer un lien d'invitation (scope, options)
    App->>SM: Space.CreateInvitation(LINK, scope, expiresAt?, maxUses?)
    SM-->>App: Invitation créée (token)
    App-->>MJ: Lien d'invitation (URL avec token)
    MJ->>MJ: Partage le lien (Discord, email, etc.)
```

## Flux 3 — Rejoindre via invitation — utilisateur connecté

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant SM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>SM: Valider invitation (token)
    alt Invitation invalide / expirée
        SM-->>App: Erreur
        App-->>Joueur: "Ce lien n'est pas utilisable"
    else Invitation valide
        SM-->>App: SpaceId + scope
        App->>SM: Space.AddMember(userId, PLAYER)
        SM-->>App: MemberJoined event
        App-->>Joueur: Accès à l'espace
    end
```

## Flux 4 — Rejoindre en tant qu'invité (sans compte)

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant SM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>SM: Valider invitation (token)
    alt Invitation invalide / expirée
        App-->>Joueur: "Ce lien n'est pas utilisable"
    else Invitation valide
        App-->>Joueur: Page de saisie du nom d'affichage
        Joueur->>App: Saisir displayName
        App->>SM: GuestAccess.Create(spaceId, scope, sessionId?)
        SM-->>App: GuestAccessCreated (token)
        App->>SM: GuestAccess.SetDisplayName(displayName)
        App-->>Joueur: Accès en tant qu'invité
    end
```

## Flux 5 — Gel des espaces excédentaires (downgrade PRO → FREE)

```mermaid
sequenceDiagram
    participant Billing as Billing (infrastructure)
    participant App as Application Layer
    participant IA as Identity & Access
    participant SM as Space Management

    Billing->>App: Résiliation abonnement Pro (userId)
    App->>IA: User.ChangeTier(FREE)
    IA-->>App: AccountTierChanged(userId, FREE)
    App->>SM: GetActiveSpaces(userId, types = CAMPAIGN/ONE_SHOT)
    SM-->>App: Liste des espaces actifs CAMPAIGN/ONE_SHOT (triée par date de création) — PERSONAL exclu
    loop Pour chaque espace excédentaire (rang > 3)
        App->>SM: Space.Freeze()
        SM-->>App: SpaceFrozen event
    end
    App-->>Billing: Confirmé
```

## Flux 6 — Conversion GuestAccess → SpaceMembership

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant IA as Identity & Access
    participant SM as Space Management

    Note over Joueur: Joueur invité qui crée un compte
    Joueur->>App: Créer un compte (email, displayName, password, guestToken)
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered
    App->>SM: ConvertGuestAccessToMembership(userId, guestToken)
    SM->>SM: GuestAccess.Convert(userId)
    SM->>SM: Space.AddMember(userId, PLAYER)
    SM-->>App: GuestAccessConvertedToMember + MemberJoined
    App-->>Joueur: Compte créé — membre permanent de l'espace
```

## Flux 7 — Associer un personnage à un membre

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SM as Space Management

    MJ->>App: Associer personnage (userId, characterId)
    App->>SM: Space.AssociateCharacter(userId, characterId)
    SM->>SM: Ajouter characterId dans SpaceMembership.characterIds
    SM-->>App: CharacterAssociated event
    App-->>MJ: Personnage associé
```

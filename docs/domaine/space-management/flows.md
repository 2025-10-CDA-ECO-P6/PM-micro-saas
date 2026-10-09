# Space Management — Diagrammes de flux

## Flux 1 — Créer un espace

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SM as Space Management
    participant CL as Content Library
    participant SC as Session Conduct

    Note over MJ,CL: L'espace PERSONAL n'est pas créé ici : il naît à UserRegistered (invariant 14),<br/>avec le seul dossier virtuel (Non classés)
    MJ->>App: Créer un espace (name, type = CAMPAIGN ou ONE_SHOT)
    App->>App: Vérifier quota FREE (≤ 3 espaces actifs CAMPAIGN/ONE_SHOT)
    alt Quota dépassé
        App-->>MJ: Bloqué — upgrade requis
    else Quota OK
        App->>SM: Space.Create(ownerId, name, type, description?, gameSystem?)
        SM-->>App: SpaceCreated event
        App->>CL: Créer les dossiers système (Personnages, Joueurs, Scénarios, Notes) + dossier virtuel (Non classés)
        App->>SC: SessionViewConfig.Create(spaceId, dossiers système) — flux 1 de Session Conduct
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
    alt Invitation invalide / expirée / espace archivé
        SM-->>App: Erreur
        App-->>Joueur: "Ce lien n'est pas utilisable"
    else Invitation valide
        SM-->>App: SpaceId + scope
        App->>SM: Space.AddMember(userId, PLAYER)
        SM-->>App: MemberJoined event
        App->>SM: SpaceMembership.Activate()
        SM-->>App: MemberActivated event
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
    alt Invitation invalide / expirée / espace archivé
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
        App->>App: Notifier le MJ (SpaceFrozen)
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
    SM->>SM: GuestAccess.Convert(userId) — status CONVERTED
    SM->>SM: Space.AddMember(userId, PLAYER) — SpaceMembership en PENDING
    SM-->>App: GuestAccessConvertedToMember + MemberJoined
    App-->>Joueur: Compte créé — accès invité conservé, adhésion en attente du MJ
    Note over SM: Activate() par le MJ, hors de ce flux (invariant 8, RB-09-16)
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

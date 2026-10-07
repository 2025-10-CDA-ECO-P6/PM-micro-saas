# Space Management — Diagrammes de flux

## 1. Créer un espace

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SM as Space Management
    participant CL as Content Library

    MJ->>App: Créer un espace (name, type)
    App->>App: Vérifier quota FREE (≤ 3 espaces actifs)
    alt Quota dépassé
        App-->>MJ: Bloqué — upgrade requis
    else Quota OK
        App->>SM: Space.Create(ownerId, name, type)
        SM-->>App: SpaceCreated event
        App->>CL: Créer les dossiers système (Personnages, Joueurs, Scénarios, Notes)
        App-->>MJ: Espace créé
    end
```

## 2. Inviter un joueur (via lien)

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

## 3. Rejoindre via invitation — utilisateur connecté

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant SM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>SM: Valider invitation (token)
    alt Invitation invalide / expirée
        SM-->>App: Erreur
        App-->>Joueur: "Ce lien n'est plus actif"
    else Invitation valide
        SM-->>App: SpaceId + scope
        App->>SM: Space.AddMember(userId, PLAYER)
        SM-->>App: MemberJoined event
        App-->>Joueur: Accès à l'espace
    end
```

## 4. Rejoindre en tant qu'invité (sans compte)

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant SM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>SM: Valider invitation (token)
    alt Invitation invalide / expirée
        App-->>Joueur: "Ce lien n'est plus actif"
    else Invitation valide
        App-->>Joueur: Page de saisie du nom d'affichage
        Joueur->>App: Saisir displayName
        App->>SM: GuestAccess.Create(spaceId, scope, sessionId?)
        SM-->>App: GuestAccessCreated (token)
        App->>SM: GuestAccess.SetDisplayName(displayName)
        App-->>Joueur: Accès en tant qu'invité
    end
```

## 5. Gel des espaces excédentaires (downgrade PRO → FREE)

```mermaid
sequenceDiagram
    participant Billing as Billing (infrastructure)
    participant App as Application Layer
    participant IA as Identity & Access
    participant SM as Space Management

    Billing->>App: Résiliation abonnement Pro (userId)
    App->>IA: User.ChangeTier(FREE)
    IA-->>App: AccountTierChanged(userId, FREE)
    App->>SM: GetActiveSpaces(userId)
    SM-->>App: Liste des espaces actifs (triées par date de création)
    loop Pour chaque espace excédentaire (rang > 3)
        App->>SM: Space.Freeze()
        SM-->>App: SpaceFrozen event
    end
    App-->>Billing: Confirmé
```

## 6. Conversion GuestAccess → SpaceMembership

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

## 7. Associer un personnage à un membre

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

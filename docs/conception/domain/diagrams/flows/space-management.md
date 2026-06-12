# Space Management — Diagrammes de flux

## 1. Créer une campagne

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CM as Space Management
    participant CL as Content Library

    MJ->>App: Créer une campagne (name, type)
    App->>App: Vérifier quota FREE (≤ 3 campagnes actives)
    alt Quota dépassé
        App-->>MJ: Bloqué — upgrade requis
    else Quota OK
        App->>CM: Space.Create(ownerId, name, type)
        CM-->>App: SpaceCreated event
        App->>CL: Créer les dossiers système (Personnages, Joueurs, Scénarios, Notes)
        App-->>MJ: Campagne créée
    end
```

## 2. Inviter un joueur (via lien)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CM as Space Management

    MJ->>App: Générer un lien d'invitation (scope, options)
    App->>CM: Space.CreateInvitation(LINK, scope, expiresAt?, maxUses?)
    CM-->>App: Invitation créée (token)
    App-->>MJ: Lien d'invitation (URL avec token)
    MJ->>MJ: Partage le lien (Discord, email, etc.)
```

## 3. Rejoindre via invitation — utilisateur connecté

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant CM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>CM: Valider invitation (token)
    alt Invitation invalide / expirée
        CM-->>App: Erreur
        App-->>Joueur: "Ce lien n'est plus actif"
    else Invitation valide
        CM-->>App: SpaceId + scope
        App->>CM: Space.AddMember(userId, PLAYER)
        CM-->>App: MemberJoined event
        App-->>Joueur: Accès à la campagne
    end
```

## 4. Rejoindre en tant qu'invité (sans compte)

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant CM as Space Management

    Joueur->>App: Clic sur lien d'invitation (token)
    App->>CM: Valider invitation (token)
    alt Invitation invalide / expirée
        App-->>Joueur: "Ce lien n'est plus actif"
    else Invitation valide
        App-->>Joueur: Page de saisie du nom d'affichage
        Joueur->>App: Saisir displayName
        App->>CM: GuestAccess.Create(campaignId, scope, sessionId?)
        CM-->>App: GuestAccessCreated (token)
        App->>CM: GuestAccess.SetDisplayName(displayName)
        App-->>Joueur: Accès en tant qu'invité
    end
```

## 5. Gel des campagnes excédentaires (downgrade PRO → FREE)

```mermaid
sequenceDiagram
    participant Billing as Billing (infrastructure)
    participant App as Application Layer
    participant IA as Identity & Access
    participant CM as Space Management

    Billing->>App: Résiliation abonnement Pro (userId)
    App->>IA: User.ChangeTier(FREE)
    IA-->>App: AccountTierChanged(userId, FREE)
    App->>CM: GetActiveCampaigns(userId)
    CM-->>App: Liste des campagnes actives (triées par date de création)
    loop Pour chaque campagne excédentaire (rang > 3)
        App->>CM: Space.Freeze()
        CM-->>App: SpaceFrozen event
    end
    App-->>Billing: Confirmé
```

## 6. Conversion GuestAccess → SpaceMembership

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant IA as Identity & Access
    participant CM as Space Management

    Note over Joueur: Joueur invité qui crée un compte
    Joueur->>App: Créer un compte (email, displayName, password, guestToken)
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered
    App->>CM: ConvertGuestAccessToMembership(userId, guestToken)
    CM->>CM: GuestAccess.Convert(userId)
    CM->>CM: Space.AddMember(userId, PLAYER)
    CM-->>App: GuestAccessConvertedToMember + MemberJoined
    App-->>Joueur: Compte créé — membre permanent de la campagne
```

## 7. Associer un personnage à un membre

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CM as Space Management

    MJ->>App: Associer personnage (userId, characterId)
    App->>CM: Space.AssociateCharacter(userId, characterId)
    CM->>CM: Ajouter characterId dans SpaceMembership.characterIds
    CM-->>App: CharacterAssociated event
    App-->>MJ: Personnage associé
```

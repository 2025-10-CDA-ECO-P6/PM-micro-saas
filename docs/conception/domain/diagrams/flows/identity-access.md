# Identity & Access — Diagrammes de flux

## 1. Inscription depuis le mode local

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant IA as I&A Domain
    participant Identity as Infrastructure d'identité

    MJ->>App: Créer un compte (email, displayName, password)
    App->>Identity: CreateIdentityUser(id, email, password)
    Identity-->>App: OK
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered event
    App->>App: Migrer les données locales (stockage local → cloud)
    Note over App: Orchestration applicative :<br/>importe espaces, documents,<br/>dossiers depuis l'export local
    App-->>MJ: Compte créé, données migrées
```

## 2. Connexion

> Durées et révocation définies dans [ADR-015](../../../../architecture/decisions/ADR-015-securite-authentification-mvp.md) §3 : access token ≤ 15 min, refresh token ≤ 7 j absolu avec rotation et détection de réutilisation (denylist JTI). La révocation de la famille de tokens est déclenchée par `AccountSuspended` et `UserAnonymized` via `ITokenDenylist.RevokeFamilyAsync`.

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant Identity as Infrastructure d'identité
    participant Denylist as Token Denylist (JTI)

    Utilisateur->>App: Connexion (email, password)
    App->>Identity: SignIn(email, password)
    alt Identifiants valides et compte ACTIVE
        Identity-->>App: JWT (≤ 15 min) + refresh token (≤ 7 j, rotatif)
        App-->>Utilisateur: Authentifié
    else Identifiants invalides
        Identity-->>App: Échec
        App-->>Utilisateur: Erreur générique (pas de détail)
    else Compte SUSPENDED ou DELETED
        App-->>Utilisateur: Accès refusé
    end
    Note over App,Denylist: AccountSuspended / UserAnonymized → RevokeFamilyAsync(userId)<br/>invalide tous les refresh tokens actifs (denylist JTI)
```

## 3. Réinitialisation du mot de passe

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant Identity as Infrastructure d'identité
    participant Email as Service Email

    Utilisateur->>App: Demande reset (email)
    App->>Identity: GeneratePasswordResetToken(email)
    Identity-->>App: Token temporaire
    App->>Email: Envoyer lien de réinitialisation
    Email-->>Utilisateur: Email avec lien
    Utilisateur->>App: Nouveau mot de passe (token + password)
    App->>Identity: ResetPassword(token, newPassword)
    Identity-->>App: OK
    App-->>Utilisateur: Mot de passe mis à jour
```

## 4. Suppression de compte (RGPD)

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant IA as I&A Domain
    participant SM as Space Management

    Utilisateur->>App: Supprimer mon compte
    App->>SM: GetActiveSpacesWithMembers(userId)
    alt Espaces actifs avec membres
        SM-->>App: Liste des espaces bloquants
        App-->>Utilisateur: Bloqué — gérer les espaces d'abord
    else Aucun espace bloquant
        App->>IA: User.Delete()
        IA-->>App: UserDeleted event
        App->>IA: User.Anonymize()
        IA-->>App: UserAnonymized event
        App->>SM: Réagir à UserDeleted (anonymiser member data)
        App-->>Utilisateur: Compte supprimé, déconnecté
    end
```

## 5. Changement de tier (upgrade ou résiliation)

```mermaid
sequenceDiagram
    participant Billing as Billing Webhook (infrastructure)
    participant App as Application Layer
    participant IA as I&A Domain
    participant SM as Space Management

    Billing->>App: TierChanged(userId, newTier)
    App->>IA: User.ChangeTier(newTier)
    IA-->>App: AccountTierChanged event
    App->>SM: Réagir à AccountTierChanged
    alt Downgrade FREE → PRO (résiliation)
        SM->>SM: Geler les espaces excédentaires (> 3)
        SM-->>App: OK
    else Upgrade FREE → PRO
        SM->>SM: Débloquer les quotas
        SM-->>App: OK
    end
    App-->>Billing: Confirmé
```

## 6. Conversion GuestAccess → User

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant IA as I&A Domain
    participant SM as Space Management

    Note over Joueur: Joueur invité sans compte<br/>veut créer un compte
    Joueur->>App: Créer un compte (email, displayName, password, guestToken)
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered event
    App->>SM: ConvertGuestAccessToMembership(userId, guestToken)
    SM->>SM: GuestAccess → SpaceMembership
    SM-->>App: OK
    App-->>Joueur: Compte créé, membre permanent de l'espace
```

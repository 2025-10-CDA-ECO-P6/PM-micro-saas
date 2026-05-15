# Identity & Access — Diagrammes de flux

## 1. Inscription depuis le mode local

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant IA as I&A Domain
    participant Identity as ASP.NET Identity

    MJ->>App: Créer un compte (email, displayName, password)
    App->>Identity: CreateIdentityUser(id, email, password)
    Identity-->>App: OK
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered event
    App->>App: Migrer les données locales (IndexedDB → cloud)
    Note over App: Orchestration applicative :<br/>importe campagnes, documents,<br/>dossiers depuis l'export local
    App-->>MJ: Compte créé, données migrées
```

## 2. Connexion

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant Identity as ASP.NET Identity

    Utilisateur->>App: Connexion (email, password)
    App->>Identity: SignIn(email, password)
    alt Identifiants valides et compte ACTIVE
        Identity-->>App: JWT + refresh token
        App-->>Utilisateur: Authentifié
    else Identifiants invalides
        Identity-->>App: Échec
        App-->>Utilisateur: Erreur générique (pas de détail)
    else Compte SUSPENDED ou DELETED
        App-->>Utilisateur: Accès refusé
    end
```

## 3. Réinitialisation du mot de passe

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant Identity as ASP.NET Identity
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
    participant CM as Campaign Management

    Utilisateur->>App: Supprimer mon compte
    App->>CM: GetActiveCampaignsWithMembers(userId)
    alt Campagnes actives avec membres
        CM-->>App: Liste des campagnes bloquantes
        App-->>Utilisateur: Bloqué — gérer les campagnes d'abord
    else Aucune campagne bloquante
        App->>IA: User.Delete()
        IA-->>App: UserDeleted event
        App->>IA: User.Anonymize()
        IA-->>App: UserAnonymized event
        App->>CM: Réagir à UserDeleted (anonymiser member data)
        App-->>Utilisateur: Compte supprimé, déconnecté
    end
```

## 5. Changement de tier (upgrade ou résiliation)

```mermaid
sequenceDiagram
    participant Billing as Billing Webhook (infrastructure)
    participant App as Application Layer
    participant IA as I&A Domain
    participant CM as Campaign Management

    Billing->>App: TierChanged(userId, newTier)
    App->>IA: User.ChangeTier(newTier)
    IA-->>App: AccountTierChanged event
    App->>CM: Réagir à AccountTierChanged
    alt Downgrade FREE → PRO (résiliation)
        CM->>CM: Geler les campagnes excédentaires (> 3)
        CM-->>App: OK
    else Upgrade FREE → PRO
        CM->>CM: Débloquer les quotas
        CM-->>App: OK
    end
    App-->>Billing: Confirmé
```

## 6. Conversion GuestAccess → User

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant IA as I&A Domain
    participant CM as Campaign Management

    Note over Joueur: Joueur invité sans compte<br/>veut créer un compte
    Joueur->>App: Créer un compte (email, displayName, password, guestToken)
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered event
    App->>CM: ConvertGuestAccessToMembership(userId, guestToken)
    CM->>CM: GuestAccess → CampaignMembership
    CM-->>App: OK
    App-->>Joueur: Compte créé, membre permanent de la campagne
```

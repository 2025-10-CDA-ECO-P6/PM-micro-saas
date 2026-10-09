# Identity & Access — Diagrammes de flux

## Flux 1 — Inscription depuis le mode local

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant IA as I&A Domain
    participant Identity as Infrastructure d'identité
    participant SM as Space Management

    MJ->>App: Créer un compte (email, displayName, password)
    App->>Identity: CreateIdentityUser(id, email, password)
    Identity-->>App: OK
    App->>IA: User.Register(userId, email, displayName)
    IA-->>App: UserRegistered event
    App->>SM: Space.Create(ownerId, type = PERSONAL)
    SM-->>App: SpaceCreated event
    opt Données locales détectées
        App-->>MJ: Gate de reconnaissance (données locales détectées)
        MJ->>App: Confirme la migration
        App->>App: Migrer les données locales (stockage local → cloud)
        Note over App: Orchestration applicative :<br/>importe espaces, documents, dossiers<br/>et historique de session (sessions terminées,<br/>notes de session, épinglés, résumés)
    end
    App-->>MJ: Compte créé
```

## Flux 2 — Connexion

> Durées de vie et révocation des jetons : voir [ADR-015](../../decisions/ADR-015-securite-authentification-mvp.md) § 3 et la [vue transverse de sécurité](../../architecture/transverse.md). La suspension et l'anonymisation du compte révoquent tous les jetons de renouvellement actifs de l'utilisateur.

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant Identity as Infrastructure d'identité
    participant IA as I&A Domain

    Utilisateur->>App: Connexion (email, password)
    App->>Identity: SignIn(email, password)
    App->>IA: Lire User.status (invariant 2 : ACTIVE requis)
    IA-->>App: status
    alt Identifiants valides et compte ACTIVE
        Identity-->>App: Jeton d'accès + jeton de renouvellement
        App-->>Utilisateur: Authentifié
    else Identifiants invalides
        Identity-->>App: Échec
        App-->>Utilisateur: Erreur générique (pas de détail)
    else Compte SUSPENDED ou DELETED
        App-->>Utilisateur: Accès refusé
    end
    Note over App,Identity: AccountSuspended / UserAnonymized →<br/>révocation de tous les jetons de renouvellement actifs de l'utilisateur
```

## Flux 3 — Réinitialisation du mot de passe

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

## Flux 4 — Suppression de compte (RGPD)

```mermaid
sequenceDiagram
    actor Utilisateur
    participant App as Application Layer
    participant IA as I&A Domain
    participant SM as Space Management
    participant CL as Content Library

    Utilisateur->>App: Supprimer mon compte
    break Adresse de messagerie non validée
        App-->>Utilisateur: Refusé — valider l'adresse d'abord
    end
    App->>SM: GetBlockingSpaces(userId)
    Note over SM: Espaces CAMPAIGN/ONE_SHOT vivants (hors corbeille) avec au moins<br/>un SpaceMembership actif autre que le propriétaire.<br/>PERSONAL n'est jamais bloquant (mono-membre, invariant 3 I&A).
    alt Espaces CAMPAIGN/ONE_SHOT bloquants
        SM-->>App: Liste des espaces bloquants
        App-->>Utilisateur: Bloqué — gérer les espaces d'abord
    else Aucun espace bloquant
        App-->>Utilisateur: Conséquences de la suppression
        Utilisateur->>App: Confirme la suppression
        App->>IA: User.Delete()
        IA-->>App: UserDeleted event
        App->>IA: User.Anonymize()
        IA-->>App: UserAnonymized event
        App->>SM: Réagir à UserDeleted — routage par type d'espace :<br/>PERSONAL → hard-delete (saga SpaceDeleted),<br/>CAMPAIGN/ONE_SHOT restants → anonymiser member data
        App->>CL: Réagir à UserDeleted — hard-delete des documents PLAYER_PRIVATE créés par l'utilisateur
        App-->>Utilisateur: Compte supprimé, déconnecté
    end
```

## Flux 5 — Changement de tier (upgrade ou résiliation)

```mermaid
sequenceDiagram
    participant Billing as Billing Webhook (infrastructure)
    participant App as Application Layer
    participant IA as I&A Domain
    participant SM as Space Management

    Billing->>App: TierChanged(userId, newTier)
    App->>IA: User.ChangeTier(newTier)
    IA-->>App: AccountTierChanged event
    Note over App,SM: Réaction à AccountTierChanged orchestrée par l'Application,<br/>détail dans le flux 5 de Space Management
    alt Downgrade PRO → FREE (résiliation)
        App->>SM: GetActiveSpaces(userId, types = CAMPAIGN/ONE_SHOT)
        SM-->>App: Espaces actifs triés par date de création (PERSONAL exclu)
        loop Pour chaque espace excédentaire (rang > 3)
            App->>SM: Space.Freeze()
            SM-->>App: SpaceFrozen event
            App->>App: Notifier le MJ (SpaceFrozen)
        end
    else Upgrade FREE → PRO
        loop Pour chaque espace FROZEN du propriétaire (dans la limite du quota cible)
            App->>SM: Space.Unfreeze()
            SM-->>App: SpaceUnfrozen event
        end
    end
    App-->>Billing: Confirmé
```

## Flux 6 — Conversion GuestAccess → User

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
    SM->>SM: GuestAccess → CONVERTED, SpaceMembership en PENDING
    SM-->>App: OK
    App-->>Joueur: Compte créé — accès invité conservé, adhésion en attente du MJ
```

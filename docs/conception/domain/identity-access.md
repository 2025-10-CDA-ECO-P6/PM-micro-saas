# Identity & Access

> **Responsabilité** : gérer les comptes utilisateurs authentifiés.
> Création, connexion, mise à jour du profil, tier de compte, suppression RGPD.
>
> Ce contexte ne sait pas ce qu'est une campagne, un MJ, un joueur ou une session.
> Il sait seulement qu'un `User` existe, a une identité, un statut et un tier.

---

## Ce que ce contexte fait

- Créer un compte utilisateur.
- Authentifier un utilisateur (en délégant à ASP.NET Identity pour la gestion du mot de passe).
- Maintenir le profil : nom d'affichage.
- Gérer le tier de compte (FREE, PRO).
- Supprimer un compte et anonymiser les données nominatives (RGPD).
- Suspendre un compte.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Rôles MJ / Joueur | Campaign Management — le rôle est contextuel par campagne |
| GuestAccess (joueurs sans compte) | Campaign Management |
| Migration des données locales → cloud | Couche application (orchestration cross-contextes) |
| Gestion des membres de campagne | Campaign Management |
| Hashs de mots de passe, tokens JWT, refresh tokens | ASP.NET Identity (infrastructure) |
| Permissions sur les documents ou les sessions | Content Library / Session Conduct |

---

## Agrégat unique : `User`

### Champs

| Champ | Type | Description |
|---|---|---|
| `id` | `UserId` | Identifiant unique, partagé avec `AspNetUsers.Id` (infrastructure) |
| `email` | `Email` | Adresse email unique dans le système |
| `displayName` | `string` | Nom d'affichage choisi par l'utilisateur |
| `status` | `AccountStatus` | État du compte (ACTIVE, SUSPENDED, DELETED) |
| `tier` | `AccountTier` | Niveau d'abonnement (FREE, PRO) |
| `createdAt` | `DateTime` | Date de création |
| `updatedAt` | `DateTime` | Date de dernière modification |

### Méthodes

| Méthode | Événement produit | Description |
|---|---|---|
| `Register(email, displayName)` | `UserRegistered` | Crée un nouveau compte. Tier initial = FREE. |
| `UpdateDisplayName(name)` | — | Met à jour le nom d'affichage. |
| `ChangeTier(tier)` | `AccountTierChanged` | Change le tier (FREE → PRO ou PRO → FREE). |
| `Delete()` | `UserDeleted` | Marque le compte pour suppression. Déclenche l'anonymisation. |
| `Anonymize()` | `UserAnonymized` | Remplace les données nominatives par `[Compte supprimé]`. |
| `Suspend()` | — | Désactive temporairement le compte. |

### Enums

**`AccountStatus`**
| Valeur | Description |
|---|---|
| `ACTIVE` | Compte actif, connexion autorisée |
| `SUSPENDED` | Compte suspendu, connexion bloquée |
| `DELETED` | Compte supprimé, connexion bloquée, données anonymisées |

**`AccountTier`**
| Valeur | Limites |
|---|---|
| `FREE` | 3 campagnes cloud max, 4 joueurs par session, 500 Mo |
| `PRO` | Campagnes illimitées, joueurs illimités, 5 Go+ |

> **Note** : le mode local (sans compte) n'est pas un tier — il n'y a pas de `User` en mode local.
> Les limites du mode local (~50-100 Mo, 1 device) sont des règles frontend, pas des invariants de domaine.

---

## Invariants métier

1. `email` est unique dans le système.
2. Un `User` avec `status = DELETED` ou `status = SUSPENDED` ne peut pas s'authentifier.
3. `User.Delete()` est bloqué si l'utilisateur possède des campagnes avec des membres actifs (vérification applicative — Campaign Management est interrogé via un contrat applicatif avant l'exécution de la commande).
4. `displayName` ne peut pas être vide ni dépasser 100 caractères.
5. `tier = FREE` est le tier initial à la création.
6. La suppression est irréversible. L'anonymisation remplace `email` et `displayName` par des valeurs neutres. Le `UserId` est conservé comme référence morte dans les autres contextes.

---

## Règles métier

1. L'email est unique dans le système.
2. Le mot de passe est géré par ASP.NET Identity — `User` ne le connaît pas.
3. `User` ne porte aucun rôle métier global. Tout utilisateur authentifié peut créer une campagne.
4. La transition `PRO → FREE` (résiliation) est déclenchée par un webhook de facturation (infrastructure) via une commande applicative. `User.ChangeTier(FREE)` publie `AccountTierChanged`. Campaign Management écoute cet événement et applique ses propres règles (gel des campagnes excédentaires).
5. Après suppression RGPD, les `LiveNote` avec `visibility = PLAYER_PRIVATE` restent attachées au `CharacterId` dans Session Conduct — l'utilisateur supprimé perd l'accès, mais les notes restent pour préserver la continuité de campagne.
6. Les contenus créés (documents, notes) restent attachés à la campagne sous identité anonymisée — ils appartiennent à la campagne, pas à l'individu.

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `UserRegistered` | `User.Register()` | Application (email de bienvenue), Campaign Management (si conversion depuis GuestAccess) |
| `AccountTierChanged` | `User.ChangeTier()` | Campaign Management (quotas campagnes/joueurs), Application (notification) |
| `UserDeleted` | `User.Delete()` | Campaign Management (anonymisation des member data), Content Library si nécessaire |
| `UserAnonymized` | `User.Anonymize()` | Interne I&A — déclenché après confirmation de `UserDeleted` |

---

## Intégration avec les autres contextes

### Ce que I&A publie

- `UserId` comme identifiant de référence — les autres contextes l'utilisent sans importer l'entité `User`.
- Les événements listés ci-dessus via le bus d'événements (synchrone dans la même transaction pour le MVP).

### Ce que les autres contextes font avec `UserId`

| Contexte | Usage |
|---|---|
| Campaign Management | `Campaign.ownerId: UserId` (FK réelle acceptée — ADR-12), `CampaignMembership.userId: UserId` |
| Content Library | `AuditInfo.createdById: UserId` |
| Session Conduct | `LiveNote.createdById: UserId` |

### Ce que I&A ne consomme pas

I&A ne dépend d'aucun autre bounded context. Il ne réagit à aucun événement externe.

---

## Note sur la conversion GuestAccess → User

Quand un joueur invité (sans compte) crée un compte, le flow applicatif est :

1. I&A crée le `User` → publie `UserRegistered`.
2. L'application (couche application) appelle la commande `ConvertGuestAccessToMembership(userId, guestToken)` dans Campaign Management.
3. Campaign Management convertit le `GuestAccess` en `CampaignMembership` lié au nouveau `UserId`.

Ce flow est **applicatif**, pas domaine. I&A ne connaît pas `GuestAccess`. Campaign Management ne crée pas de User.

---

## Note sur ASP.NET Identity

ASP.NET Identity gère une table `AspNetUsers` en infrastructure. L'entité domaine `User` dans I&A est une **shadow entity** : les deux partagent le même `id` (UUID), mais ont des responsabilités séparées.

| Responsabilité | Géré par |
|---|---|
| Hash du mot de passe | `AspNetUsers` (Identity) |
| Génération / validation JWT | Identity infrastructure |
| Refresh tokens | Identity infrastructure |
| Claims techniques | Identity infrastructure |
| Email, displayName, status, tier | `User` (domaine I&A) |

---

## Diagrammes

→ [Classes](diagrams/classes/identity-access.md)
→ [MCD](diagrams/mcd/identity-access.md)
→ [MLD](diagrams/mld/identity-access.md)
→ [Flux](diagrams/flows/identity-access.md)

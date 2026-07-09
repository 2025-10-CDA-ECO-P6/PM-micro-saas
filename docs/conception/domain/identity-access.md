# Identity & Access

> **Responsabilité** : gérer les comptes utilisateurs authentifiés.
> Création, connexion, mise à jour du profil, tier de compte, suppression RGPD.
>
> Ce contexte ne sait pas ce qu'est un espace, un MJ, un joueur ou une session.
> Il sait seulement qu'un `User` existe, a une identité, un statut et un tier.

---

## Ce que ce contexte fait

- Créer un compte utilisateur.
- Authentifier un utilisateur (en délégant à l'infrastructure d'identité pour la gestion du mot de passe).
- Maintenir le profil : nom d'affichage.
- Gérer le tier de compte (FREE, PRO).
- Supprimer un compte et anonymiser les données nominatives (RGPD).
- Suspendre un compte.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Rôles MJ / Joueur | Space Management — le rôle est contextuel par espace |
| GuestAccess (joueurs sans compte) | Space Management |
| Migration des données locales → cloud | Couche application (orchestration cross-contextes) |
| Gestion des membres d'espace | Space Management |
| Secrets et jetons d'authentification | Infrastructure d'identité |
| Permissions sur les documents ou les sessions | Content Library / Session Conduct |

---

## Agrégat unique : `User`

### Champs

| Champ | Type | Description |
|---|---|---|
| `id` | `UserId` | Identifiant unique, partagé avec le référentiel d'identité de l'infrastructure |
| `email` | `Email` | Adresse email unique dans le système |
| `emailVerified` | `bool` | Indique si l'adresse de messagerie a été confirmée par l'utilisateur. `false` à la création, `true` après vérification. |
| `displayName` | `string` | Nom d'affichage choisi par l'utilisateur |
| `status` | `AccountStatus` | État du compte (ACTIVE, SUSPENDED, DELETED) |
| `tier` | `AccountTier` | Niveau d'abonnement (FREE, PRO) |
| `createdAt` | `DateTime` | Date de création |
| `updatedAt` | `DateTime` | Date de dernière modification |

### Méthodes

| Méthode | Événement produit | Description |
|---|---|---|
| `Register(email, displayName)` | `UserRegistered` | Crée un nouveau compte. Tier initial = FREE. `emailVerified = false`. |
| `VerifyEmail()` | `EmailVerified` | Marque l'adresse de messagerie comme vérifiée. `emailVerified` passe à `true`. |
| `UpdateDisplayName(name)` | `DisplayNameUpdated` | Met à jour le nom d'affichage. |
| `ChangeEmail(newEmail)` | `EmailChangeRequested` | Demande un changement d'adresse de messagerie. Exige `emailVerified = true`. Repasse `emailVerified` à `false` jusqu'à confirmation de la nouvelle adresse. |
| `LinkFederatedIdentity(provider, externalId)` | `FederatedIdentityLinked` | Associe une identité fédérée (SSO) au compte. Exige `emailVerified = true`. |
| `ChangeTier(tier)` | `AccountTierChanged` | Change le tier (FREE → PRO ou PRO → FREE). |
| `Delete()` | `UserDeleted` | Marque le compte pour suppression. Exige `emailVerified = true`. Déclenche l'anonymisation. |
| `Anonymize()` | `UserAnonymized` | Remplace les données nominatives par `[Compte supprimé]`. |
| `Suspend()` | `AccountSuspended` | Désactive temporairement le compte. |

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
| `FREE` | 3 espaces `CAMPAIGN`/`ONE_SHOT` max (l'espace `PERSONAL` par défaut n'est pas décompté), 4 joueurs par session, 500 Mo |
| `PRO` | Espaces illimités, joueurs illimités, 5 Go+ |

> **Note** : le mode local (sans compte) n'est pas un tier — il n'y a pas de `User` en mode local.
> Les limites du mode local (~50-100 Mo, 1 device) sont des règles frontend, pas des invariants de domaine.
> La propriété d'un espace (y compris `SpaceType.PERSONAL`) naît à la création de compte. En mode local, le conteneur par défaut **est** l'espace `PERSONAL` — typé, sans `ownerId` — et non un conteneur type-agnostique ; il est lié à l'espace `PERSONAL` cloud à la création de compte / migration (cohérent invariant 14, régi côté Space Management, ADR-017/ADR-018).

---

## Invariants métier

1. `email` est unique dans le système.
2. Un `User` avec `status = DELETED` ou `status = SUSPENDED` ne peut pas s'authentifier.
3. `User.Delete()` n'est **pas** bloqué par la possession d'espaces. À l'événement `UserDeleted`, chaque espace possédé est traité selon son type :
   — `PERSONAL` : **hard-delete inconditionnel** (via la saga `SpaceDeleted`), sous l'invariant de claim/reprise J+30 (ADR-018 l.248, ADR-011) ; il ne survit **jamais** sous identité anonymisée.
   — `CAMPAIGN` / `ONE_SHOT` : **conservation sous identité anonymisée** (`owner_id → utilisateur anonymisé`, contenu `PUBLIC`/`GM_ONLY` conservé — ADR-013 §6), plus hard-delete des documents `PLAYER_PRIVATE` créés par l'utilisateur (F-08 / RB-4).
   La seule pré-condition de `User.Delete()` demeure `emailVerified = true` (invariant 7).

   > **Résolu (ADR-018, W2)** : l'ancienne lecture de l'invariant 3 (pré-condition bloquante sur « espaces avec membres actifs ») bloquait littéralement toute suppression de compte, un espace `PERSONAL` étant mono-membre par construction (son propriétaire, toujours actif). Le point est fermé : l'invariant 3 n'est plus une pré-condition mais un routage de cascade différencié par type d'espace, tranché ci-dessus.
4. `displayName` ne peut pas être vide ni dépasser 100 caractères.
5. `tier = FREE` est le tier initial à la création.
6. La suppression est irréversible. L'anonymisation remplace `email` et `displayName` par des valeurs neutres. Le `UserId` est conservé comme référence morte dans les autres contextes.
7. L'effacement RGPD (`User.Delete()`) et toutes les opérations sensibles — modification d'adresse de messagerie (`ChangeEmail()`), liaison d'une identité fédérée (`LinkFederatedIdentity()`) — **exigent `emailVerified = true`**. Ces opérations sont bloquées si `emailVerified = false`. Cette exigence garantit que l'adresse de messagerie est bien contrôlée par l'utilisateur avant qu'une action irréversible ou à fort impact soit appliquée sur le compte (aligné RB-10-05 et NFR-CONF-03).

---

## Règles métier

1. Le mot de passe est délégué à l'infrastructure d'identité — `User` ne le connaît pas.
2. `User` ne porte aucun rôle métier global. Tout utilisateur authentifié peut créer un espace.
3. La transition `PRO → FREE` (résiliation) est déclenchée par une notification du système de facturation (infrastructure) via une commande applicative. `User.ChangeTier(FREE)` publie `AccountTierChanged`. Space Management écoute cet événement et applique ses propres règles (gel des espaces excédentaires).
4. **Règle F-08 — Effacement effectif des documents privés à la suppression de compte** (RGPD, article 17 — droit à l'effacement) : après suppression d'un compte, deux populations de documents privés sont supprimées **physiquement** (pas via `SoftDelete` — la suppression logique réversible ne constitue pas un effacement au sens légal) : (a) les documents avec `visibility = PLAYER_PRIVATE` créés par cet utilisateur, et (b) les documents avec `visibility = PLAYER_PRIVATE` créés par cet utilisateur et rattachés aux personnages incarnés par l'utilisateur dans l'ensemble des espaces vivants où il était membre. Cette extension couvre le risque de résidu : un personnage pouvant être réassocié ultérieurement à un autre joueur, une note privée résiduelle rattachée à ce personnage serait exposée au nouveau propriétaire. Les contenus partagés (`PUBLIC`, `GM_ONLY`) sont conservés sous intérêt légitime pour assurer la continuité d'espace. La mise en œuvre de cette obligation est arbitrée par ADR-012.
5. Les contenus créés (documents, notes) restent attachés à l'**espace**, pas à l'individu directement. L'**espace personnel** (`SpaceType.PERSONAL`), lui, appartient à l'individu (`ownerId`) : la chaîne de possession est `contenu → Space → ownerId`. Pour les espaces partagés, les contenus partagés (`PUBLIC`, `GM_ONLY`) sont conservés sous identité anonymisée pour assurer la continuité d'espace. Le contenu d'un espace `PERSONAL` n'a pas de tiers ; son effacement suit l'invariant 3 (hard-delete inconditionnel de l'espace `PERSONAL` à `UserDeleted`, via la saga `SpaceDeleted` — ADR-018/ADR-011).

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `UserRegistered` | `User.Register()` | Application (email de bienvenue), Space Management (création de l'espace `PERSONAL` du compte — invariant 14 ; conversion depuis GuestAccess le cas échéant) |
| `EmailVerified` | `User.VerifyEmail()` | Interne I&A — aucun autre contexte consommateur ; débloque en interne les opérations sensibles conditionnées par `emailVerified` (invariant 7) |
| `DisplayNameUpdated` | `User.UpdateDisplayName()` | Space Management, Session Conduct (affichage du nom dans les vues joueur et membre) |
| `EmailChangeRequested` | `User.ChangeEmail()` | Application (envoi de la confirmation à la nouvelle adresse, préalable à sa prise en compte) |
| `FederatedIdentityLinked` | `User.LinkFederatedIdentity()` | Interne I&A — aucun autre contexte consommateur |
| `AccountTierChanged` | `User.ChangeTier()` | Space Management (quotas espaces/joueurs), Application (notification) |
| `AccountSuspended` | `User.Suspend()` | Application (toutes les sessions actives de l'utilisateur sont immédiatement révoquées — ADR-015) |
| `UserDeleted` | `User.Delete()` | Space Management (routage cascade par type d'espace, invariant 3 : hard-delete de l'espace `PERSONAL` via la saga `SpaceDeleted`, anonymisation `ownerId` pour `CAMPAIGN`/`ONE_SHOT`), Content Library (hard-delete des documents `PLAYER_PRIVATE` créés par l'utilisateur — F-08/RB-4) |
| `UserAnonymized` | `User.Anonymize()` | Interne I&A — déclenché après confirmation de `UserDeleted` ; déclenche la révocation de toutes les sessions actives de l'utilisateur (ADR-015) |

---

## Intégration avec les autres contextes

### Ce que I&A publie

- `UserId` comme identifiant de référence — les autres contextes l'utilisent sans importer l'entité `User`.
- Les événements listés ci-dessus, publiés et consommés de manière synchrone dans la même transaction pour le MVP.

### Ce que les autres contextes font avec `UserId`

| Contexte | Usage |
|---|---|
| Space Management | `Space.ownerId: UserId` (référence directe acceptée — [ADR-009](../../architecture/decisions/ADR-009-fk-campaign-owner.md)), `SpaceMembership.userId: UserId` |
| Content Library | `AuditInfo.createdById: UserId` |
| Session Conduct | `Document (type LIVE_NOTE).AuditInfo.createdById: UserId` |

### Ce que I&A ne consomme pas

I&A ne dépend d'aucun autre bounded context. Il ne réagit à aucun événement externe.

---

## Note sur la conversion GuestAccess → User

Quand un joueur invité (sans compte) crée un compte, le flow applicatif est :

1. I&A crée le `User` → publie `UserRegistered`.
2. L'application (couche application) appelle la commande `ConvertGuestAccessToMembership(userId, guestToken)` dans Space Management.
3. Space Management convertit le `GuestAccess` en `SpaceMembership` lié au nouveau `UserId`.

Ce flow est **applicatif**, pas domaine. I&A ne connaît pas `GuestAccess`. Space Management ne crée pas de User.

---

## Note sur la délégation d'identité

L'infrastructure d'identité gère son propre référentiel de secrets et de jetons. L'entité domaine `User` dans I&A est une **shadow entity** : l'entité domaine et le référentiel d'infrastructure partagent le même identifiant, mais ont des responsabilités séparées.

| Responsabilité | Géré par |
|---|---|
| Secrets d'authentification (mots de passe) | Infrastructure d'identité |
| Génération et validation des jetons | Infrastructure d'identité |
| Jetons de renouvellement | Infrastructure d'identité |
| Métadonnées techniques d'authentification | Infrastructure d'identité |
| Email, displayName, status, tier | `User` (domaine I&A) |

---

## Diagrammes

→ [Classes](diagrams/classes/identity-access.md)
→ [MCD](diagrams/mcd/identity-access.md)
→ [MLD](diagrams/mld/identity-access.md)
→ [Flux](diagrams/flows/identity-access.md)

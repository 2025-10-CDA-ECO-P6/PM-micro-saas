# Domain Model


## Table des matières

1. [Vision du domaine](#1-vision-du-domaine)
2. [Ubiquitous Language](#2-ubiquitous-language)
3. [Bounded Contexts et Context Map](#3-bounded-contexts-et-context-map)
4. [Shared Kernel — Core](#4-shared-kernel--core)
5. [Bounded Context — Identity & Access](#5-bounded-context--identity--access)
6. [Bounded Context — Campaign Management](#6-bounded-context--campaign-management)
7. [Bounded Context — Content Library](#7-bounded-context--content-library)
8. [Bounded Context — Session Conduct](#8-bounded-context--session-conduct)
9. [Domain Events — vue globale](#9-domain-events--vue-globale)
10. [Décisions d'architecture domaine](#10-décisions-darchitecture-domaine)
11. [Hors périmètre MVP — points d'extension documentés](#11-hors-périmètre-mvp--points-dextension-documentés)

---

## 1. Vision du domaine

Haversack est un outil d'assistance au Maître du Jeu (MJ) pour la préparation
et la conduite de campagnes de jeu de rôle.

Le domaine résout un problème central : **la dispersion des informations**.
Un MJ gère simultanément des scénarios, des PNJ, des notes privées, des fiches
personnages, des sessions en cours sur des outils non spécialisés et non connectés.
Haversack centralise ces informations dans un modèle structuré, modulaire et extensible.

### Utilisateurs du domaine

| Acteur | Description |
|---|---|
| **MJ (Maître du Jeu)** | Utilisateur principal. Crée et administre les campagnes, prépare et conduit les sessions. |
| **Joueur** | Utilisateur secondaire. Accède à sa fiche personnage et aux informations partagées par le MJ. |
| **Joueur invité** | Accès temporaire sans compte. Rejoint via token d'invitation. Traité comme un joueur à part entière du point de vue de la visibilité du contenu — la distinction est uniquement technique (pas d'identité persistante). |

---

## 2. Ubiquitous Language

Vocabulaire partagé du domaine. Ces termes sont utilisés tels quels dans le code,
la documentation et les conversations.

| Terme | Définition |
|---|---|
| **Campagne** | Espace organisationnel regroupant scénarios, PNJ, personnages, sessions et notes d'une aventure JDR. |
| **MJ** | Maître du Jeu. Propriétaire de la campagne, seul à pouvoir modifier le contenu et gérer les accès. |
| **Joueur** | Membre authentifié d'une campagne, associé à un ou plusieurs personnages joueurs. |
| **Joueur invité** | Accès temporaire sans compte persistant. Représenté par un GuestAccess, pas un User. Traité comme un joueur ordinaire pour l'accès au contenu. |
| **Membre** | Toute personne ayant accès à une campagne : joueur authentifié (CampaignMembership) ou joueur invité actif (GuestAccess). Un contenu PUBLIC est accessible à tous les membres, y compris les invités. |
| **Scénario** | Structure narrative préparée par le MJ, composée de scènes ordonnées. |
| **Scène** | Unité narrative d'un scénario. Peut être liée à des PNJ. |
| **PNJ** | Personnage Non-Joueur. Entité narrative créée et gérée par le MJ. |
| **Personnage joueur** | Fiche d'un personnage appartenant à un joueur. Créée par le MJ ou le joueur. Peut exister sans joueur associé (en attente d'association ou personnage joué par le MJ). |
| **Document** | Unité de contenu modulaire. Tout contenu éditorial est un Document typé composé de blocs. |
| **Bloc** | Unité atomique de contenu dans un Document. Chaque bloc a un type et une valeur fortement typée. |
| **Template** | Schéma de blocs définissant la structure attendue d'un Document. Snapshot à la création — non lié après. |
| **Tag** | Étiquette créée au niveau de la campagne. Réutilisable sur n'importe quel Document de la campagne. Permet le filtrage et la navigation transversale. |
| **Dossier** | Conteneur organisationnel créé par le MJ pour regrouper ses documents librement. Quatre dossiers système existent dans chaque campagne (PNJ, Personnages joueurs, Scénarios, Notes). Le MJ peut créer des dossiers personnalisés. |
| **Backlink** | Référence inverse — liste des documents qui pointent vers un document donné via un bloc RELATION. Calculé à la lecture depuis l'index, sans table de liaison dédiée. Un backlink pointant vers un document supprimé n'est pas affiché. |
| **Session** | Instance d'une partie jouée. Liée optionnellement à un scénario. |
| **Note live** | Note prise pendant une session. Peut être créée par le MJ ou par un joueur. Liée automatiquement à la session en cours. |
| **Résumé** | Compte-rendu d'une session clôturée. Peut être partagé aux joueurs. |
| **Visibilité** | Niveau d'accès d'un contenu : PRIVATE (MJ uniquement), PLAYER_PRIVATE (joueur créateur uniquement — MJ exclu), SHARED (membres ciblés via AccessPolicy), PUBLIC (tous les membres, y compris les invités actifs). |
| **RequesterId** | Identité du demandeur d'accès à un contenu. Type union scellé : soit un UserId (utilisateur authentifié) soit un GuestRequesterId (invité avec GuestAccessId et CharacterId optionnel). Utilisé par AccessPolicy. |
| **AccessPolicy** | Service domaine gérant les autorisations d'accès aux contenus d'une campagne. |
| **GuestAccess** | Accès temporaire dans une campagne. N'est pas un User — pas d'identité persistante. |
| **Invitation** | Token généré par le MJ permettant à un joueur de rejoindre une campagne. |
| **Système de jeu** | Référentiel de règles d'un JDR (D&D 5e, Call of Cthulhu, etc.). Point d'extension futur. |
| **Slug** | Identifiant lisible généré depuis un titre. Utilisé pour l'affichage. Les URLs utilisent l'Id UUID — le slug n'est jamais dans les routes. |
| **Audit** | Traçabilité des créations et modifications : qui, quand. |
| **Soft delete** | Suppression logique — l'entité est marquée supprimée mais reste en base pour l'intégrité référentielle. |

---

## 3. Bounded Contexts et Context Map

### Découpage en Bounded Contexts

| Contexte | Responsabilité | Agrégats racines | Entités avec repository |
|---|---|---|---|
| **Core** (Shared Kernel) | Primitives, Id typés, abstractions | — | — |
| **Identity & Access** | Utilisateurs authentifiés, rôles globaux | `User` | — |
| **Campaign Management** | Campagnes, membres, invitations, systèmes de jeu, accès aux contenus | `Campaign`, `GameSystem` | `GuestAccess` |
| **Content Library** | Tout le contenu éditorial | `Document`, `Scenario`, `DocumentTemplate`, `Folder`, `Tag` | `NPC`, `PlayerCharacter` |
| **Session Conduct** | Préparation, conduite et clôture des sessions | `Session` | — |

### Context Map

```
Core (Shared Kernel)
  └── consommé par tous les contextes
      └── fournit : Id typés, AuditInfo, SoftDelete, abstractions, RequesterId

Identity & Access  [Upstream]
  └── fournit UserId à tous les autres contextes

Campaign Management
  ├── consomme Identity (UserId)
  ├── fournit CampaignId à Content Library et Session Conduct
  └── héberge AccessPolicy — service domaine de visibilité des contenus

Content Library
  ├── consomme Campaign Management (CampaignId)
  ├── consomme Identity (UserId)
  └── fournit DocumentId, NpcId, CharacterId, ScenarioId, TagId à Session Conduct

Session Conduct
  ├── consomme Campaign Management (CampaignId)
  ├── consomme Content Library (DocumentId, CharacterId, ScenarioId)
  └── ne référence les entités des autres contextes que par leurs Id typés
```

### Règle de dépendance entre contextes

> Un bounded context ne peut jamais importer une entité domaine d'un autre contexte.
> Il ne consomme que les **Id typés** et les **primitives** du Shared Kernel.
> La résolution d'un Id vers une entité complète est faite par la couche Application
> via une query cross-context.

---

## 4. Shared Kernel — Core

Contenu stable partagé par tous les contextes. Aucune règle métier. Aucune dépendance externe.

### Id typés

Chaque agrégat et entité avec repository possède son propre type d'identifiant.
Un `UserId` ne peut jamais être passé là où un `CampaignId` est attendu — erreur de compilation.

```
UserId          — Identity & Access
CampaignId      — Campaign Management
GameSystemId    — Campaign Management
MembershipId    — Campaign Management
InvitationId    — Campaign Management
GuestAccessId   — Campaign Management
AccessRuleId    — Campaign Management (ContentAccessRule)
DocumentId      — Content Library
BlockId         — Content Library
NpcId           — Content Library
CharacterId     — Content Library
ScenarioId      — Content Library
SceneId         — Content Library
TemplateId      — Content Library
FolderId        — Content Library
TagId           — Content Library
SessionId       — Session Conduct
LiveNoteId      — Session Conduct
SummaryId       — Session Conduct
```

### Value Objects fondations

#### AuditInfo

Traçabilité des créations et modifications. Embarqué par toutes les entités.

```
AuditInfo
├── createdAt   : DateTime
├── updatedAt   : DateTime
├── createdById : UserId
└── updatedById : UserId?
```

**Comportement** : `updatedAt` et `updatedById` sont mis à jour à chaque modification.
La logique est centralisée dans ce value object.

#### SoftDelete

Suppression logique. Embarqué uniquement par les entités qui le supportent.

```
SoftDelete
├── isDeleted   : Boolean
├── deletedAt   : DateTime?
└── deletedById : UserId?
```

**Règle** : `isDeleted = true` exclut l'entité de toutes les requêtes par défaut.
Elle reste en base pour l'intégrité référentielle et les audits.

#### Email

```
Email
└── value : String    — validé format RFC 5322
```

**Invariant** : un Email invalide ne peut pas être instancié.

#### Slug

```
Slug
└── value : String    — lowercase, tirets, alphanumérique uniquement
```

**Comportement** : généré depuis un titre. Unicité vérifiée dans le contexte applicatif.
Les Slugs sont utilisés pour l'affichage et la recherche — les URLs utilisent les Id UUID.

#### PinnedItem

Value object représentant un document épinglé à une session avec son ordre et sa date.

```
PinnedItem
├── documentId : DocumentId
├── order      : Int
└── pinnedAt   : DateTime
```

### RequesterId — type union pour l'autorisation

`RequesterId` est une hiérarchie de types scellés permettant à `AccessPolicy`
de traiter uniformément les utilisateurs authentifiés et les invités.

```
RequesterId  (abstract, sealed)
├── AuthenticatedRequesterId
│   └── userId : UserId
│
└── GuestRequesterId
    ├── guestAccessId : GuestAccessId
    └── characterId   : CharacterId?    — personnage associé par le MJ, peut être null
```

**Usage** : `AccessPolicy.CanAccess(documentId, requester: RequesterId) → bool`.
Voir la section Campaign Management pour les règles de résolution.

### Enumerations partagées

```
Visibility
├── PRIVATE        — visible uniquement par le MJ propriétaire de la campagne
├── PLAYER_PRIVATE — visible uniquement par le joueur créateur (createdById) — MJ exclu
│                    jamais accessible à un GuestRequesterId
├── SHARED         — visible par les membres ciblés via AccessPolicy
└── PUBLIC         — visible par tous les membres de la campagne (joueurs authentifiés ET invités actifs)
```

**Règle de résolution d'accès** (appliquée par `AccessPolicy.CanAccess`) :

1. `PRIVATE` → accès accordé au MJ (`AuthenticatedRequesterId` avec `userId = campaign.ownerId`) uniquement.
2. `PLAYER_PRIVATE` → accès accordé uniquement au `AuthenticatedRequesterId` dont `userId = document.createdById`.
   Jamais accordé à un `GuestRequesterId`. Le MJ n'a **pas** accès non plus.
3. `PUBLIC` → accès accordé à tout `RequesterId` valide (authentifié ou invité actif).
4. `SHARED` → accès accordé si `AccessPolicy` contient une `ContentAccessRule` correspondant au demandeur :
   - `AllMembersTarget` → accordé à tout `AuthenticatedRequesterId` membre de la campagne, ET à tout `GuestRequesterId` avec un `GuestAccess.status = ACTIVE`.
   - `SpecificMemberTarget(userId)` → accordé uniquement si `requester` est `AuthenticatedRequesterId` avec `userId` correspondant.
   - `SpecificCharacterTarget(characterId)` → accordé si `requester.characterId = characterId` (fonctionne pour les deux types de RequesterId).

**Invariant** : une `ContentAccessRule` ne peut être créée que pour un document `SHARED`.

### Abstractions d'infrastructure

```
IAggregateRoot<TId>    — interface marqueur pour les racines d'agrégats
IEntity<TId>           — interface marqueur pour les entités avec repository
IDomainEvent           — interface marqueur pour les événements domaine
IRepository<T, TId>   — interface générique de base
IUnitOfWork            — abstraction transactionnelle
```

---

## 5. Bounded Context — Identity & Access

### Responsabilité

Gestion des utilisateurs authentifiés et de leurs rôles globaux.
Isolé pour permettre l'ajout futur d'OAuth ou SSO sans impacter les autres contextes.

> **Note importante** : Les joueurs invités (GUEST) ne sont **pas** des `User`.
> Ils sont représentés par un `GuestAccess` dans Campaign Management.
> `UserRole` ne contient que `GM` et `PLAYER`.

### Note d'implémentation

En .NET, ce contexte utilise ASP.NET Core Identity en infrastructure.
L'entité domaine `User` est totalement indépendante de `IdentityUser`.
Le mapping est géré par un `UserMapper` dans la couche Infrastructure.

---

### Agrégat : `User`

```
User
├── id          : UserId
├── email       : Email
├── displayName : String
├── role        : UserRole
├── status      : UserStatus
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Enumerations

```
UserRole
├── GM       — Maître du Jeu, peut créer des campagnes
└── PLAYER   — Joueur, accède aux campagnes via invitation

UserStatus
├── ACTIVE      — compte actif
├── SUSPENDED   — compte suspendu temporairement
└── DELETED     — soft delete, compte désactivé définitivement
```

#### Invariants et règles métier

- L'email est unique dans le système.
- Un utilisateur `DELETED` n'est jamais supprimé physiquement.
- Un `GM` peut créer et administrer plusieurs campagnes.
- Un `PLAYER` ne peut pas créer de campagne.
- Un utilisateur peut demander la suppression de son compte (RGPD) : statut → `DELETED`, données personnelles anonymisées. Les contenus de campagne (notes, personnages) ne sont pas supprimés — leur `createdById` est conservé pour l'intégrité.

#### Domain Events

```
UserRegistered    { userId, email, role, occurredAt }
UserRoleChanged   { userId, oldRole, newRole, occurredAt }
UserDeactivated   { userId, occurredAt }
```

---

## 6. Bounded Context — Campaign Management

### Responsabilité

Organisation et cycle de vie des campagnes. Gestion des membres, invitations,
accès invités et systèmes de jeu. Héberge `AccessPolicy` — le service domaine
de visibilité des contenus.

---

### Agrégat : `Campaign`

```
Campaign
├── id            : CampaignId
├── ownerId       : UserId
├── name          : String
├── slug          : Slug               — unique par propriétaire
├── description   : String?
├── gameSystemId  : GameSystemId?      — optionnel, point d'extension futur
├── status        : CampaignStatus
├── memberships   : CampaignMembership[]    — entités enfants
├── invitations   : Invitation[]            — entités enfants
├── audit         : AuditInfo
└── softDelete    : SoftDelete
```

#### Entité enfant : `CampaignMembership`

```
CampaignMembership
├── id           : MembershipId
├── campaignId   : CampaignId
├── userId       : UserId              — toujours renseigné (User authentifié)
├── role         : MemberRole
├── status       : MembershipStatus
├── joinedAt     : DateTime?
└── audit        : AuditInfo
```

> Les accès invités sans compte sont gérés par `GuestAccess`, pas par `CampaignMembership`.

#### Entité enfant : `Invitation`

```
Invitation
├── id          : InvitationId
├── campaignId  : CampaignId
├── token       : String               — identifiant unique généré
├── type        : InvitationType
├── expiresAt   : DateTime?            — null = pas d'expiration
├── maxUses     : Int?                 — null = illimité
├── useCount    : Int
├── status      : InvitationStatus
└── audit       : AuditInfo
```

#### Enumerations Campaign

```
CampaignStatus
├── DRAFT     — en cours de création
├── ACTIVE    — campagne en cours
├── PAUSED    — campagne en pause
└── ARCHIVED  — campagne terminée, lecture seule

MemberRole
├── OWNER    — MJ propriétaire, droits complets
└── PLAYER   — joueur membre, droits limités

MembershipStatus
├── PENDING   — invitation envoyée, pas encore acceptée
├── ACTIVE    — membre actif
└── REMOVED   — membre retiré

InvitationType
├── LINK    — lien partageable
└── EMAIL   — invitation nominative

InvitationStatus
├── ACTIVE    — utilisable
├── EXPIRED   — expirée (date ou maxUses atteint)
└── REVOKED   — révoquée par le MJ
```

#### Invariants et règles métier

- Une campagne a **exactement un** membre `OWNER` — invariant garanti par l'agrégat.
- L'`OWNER` ne peut pas être retiré — seul un transfert de propriété est possible.
- `useCount >= maxUses` → invitation passe automatiquement à `EXPIRED`.
- Une invitation `EXPIRED` ou `REVOKED` ne peut plus être utilisée.
- Une campagne `ARCHIVED` est en lecture seule.
- Le slug est unique par propriétaire (`ownerId + slug`).

#### Domain Events

```
CampaignCreated           { campaignId, ownerId, name, occurredAt }
CampaignStatusChanged     { campaignId, oldStatus, newStatus, occurredAt }
CampaignOwnerTransferred  { campaignId, oldOwnerId, newOwnerId, occurredAt }
MemberJoined              { campaignId, userId, role, occurredAt }
MemberRemoved             { campaignId, userId, occurredAt }
InvitationCreated         { invitationId, campaignId, type, occurredAt }
InvitationRevoked         { invitationId, campaignId, occurredAt }
```

---

### Entité : `GuestAccess`

Représente un accès temporaire à une campagne sans compte utilisateur persistant.
Ce n'est **pas** un `User` — c'est une session d'accès limitée dans le temps.
Du point de vue de la visibilité du contenu, un GuestAccess actif est traité
comme un joueur ordinaire (accès aux documents PUBLIC et aux documents SHARED qui lui sont ciblés).

```
GuestAccess
├── id           : GuestAccessId
├── campaignId   : CampaignId
├── displayName  : String              — pseudo choisi par l'invité
├── accessToken  : String              — token d'accès, validé en infrastructure
├── characterId  : CharacterId?        — personnage associé par le MJ
├── expiresAt    : DateTime?
├── status       : GuestAccessStatus
└── createdAt    : DateTime
```

```
GuestAccessStatus
├── ACTIVE    — accès valide
└── EXPIRED   — accès expiré ou révoqué
```

#### Invariants et règles métier

- Un `GuestAccess` est créé depuis une `Invitation` active.
- La validation du token est une responsabilité de la couche Infrastructure — le domaine ne connaît que le statut.
- Un `GuestAccess` expiré ne donne aucun accès.
- Un `GuestAccess` n'a pas de `SoftDelete` — suppression physique à expiration.
- Pour `AccessPolicy`, un `GuestAccess` actif est représenté par un `GuestRequesterId(guestAccessId, characterId?)`.

#### Domain Events

```
GuestAccessCreated           { guestAccessId, campaignId, occurredAt }
GuestAccessExpired           { guestAccessId, campaignId, occurredAt }
GuestAccessCharacterAssigned { guestAccessId, campaignId, characterId, occurredAt }
```

---

### Agrégat : `GameSystem`

Point d'extension futur pour les règles système et les templates par jeu.

```
GameSystem
├── id          : GameSystemId
├── name        : String
├── slug        : Slug
├── description : String?
├── isBuiltIn   : Boolean
├── ownerId     : UserId?      — null si BUILTIN, sinon créateur du système custom
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Invariants et règles métier

- `isBuiltIn = true` → non modifiable, non supprimable.
  **Cet invariant est protégé dans l'entité domaine `GameSystem`**, pas dans un service applicatif.
  `GameSystem.Update()` et `GameSystem.Delete()` lèvent une `DomainException` si `isBuiltIn = true`.
- Un système custom (`ownerId` non null) est visible uniquement par son créateur — non partageable entre GM dans le MVP.

---

### Service domaine : `AccessPolicy`

Gère les autorisations d'accès aux contenus de Content Library pour les membres
d'une campagne. Ce n'est pas un agrégat — c'est un service domaine avec persistance
légère via des entités `ContentAccessRule`.

```
ContentAccessRule               — entité persistée par AccessPolicy
├── id            : AccessRuleId
├── campaignId    : CampaignId
├── documentId    : DocumentId  — ref cross-context par Id uniquement
├── grantedById   : UserId
├── target        : AccessTarget
└── createdAt     : DateTime    — règle immuable, pas de AuditInfo complet
```

```
AccessTarget  (hiérarchie de value objects scellés)
├── AllMembersTarget
│   — cible tous les membres authentifiés ET tous les GuestAccess actifs de la campagne
│
├── SpecificMemberTarget
│   └── userId : UserId        — cible un membre authentifié précis
│
└── SpecificCharacterTarget
    └── characterId : CharacterId  — cible le joueur (ou invité) associé à ce personnage
```

> **Note de typage** : `AccessTarget` est une hiérarchie de types scellés,
> pas un objet avec un discriminant enum et un champ `targetId` optionnel.
> Même principe que `BlockValue` et `RequesterId`.

#### Méthode exposée

```
AccessPolicy.CanAccess(documentId: DocumentId, requester: RequesterId) → bool
```

**Algorithme de résolution** :

```
1. Charger Document.visibility
2. Si PRIVATE    → vrai ssi requester est AuthenticatedRequesterId
                   ET userId == campaign.ownerId
3. Si PLAYER_PRIVATE → vrai ssi requester est AuthenticatedRequesterId
                       ET userId == document.createdById
4. Si PUBLIC     → vrai pour tout RequesterId valide (y compris GuestRequesterId)
5. Si SHARED     → charger les ContentAccessRule pour ce documentId, puis :
   - AllMembersTarget       : vrai si requester est authentifié membre actif
                              OU GuestRequesterId avec GuestAccess.status = ACTIVE
   - SpecificMemberTarget   : vrai si requester est AuthenticatedRequesterId
                              ET userId correspond
   - SpecificCharacterTarget: vrai si requester.characterId correspond
                              (AuthenticatedRequesterId ou GuestRequesterId)
```

#### Invariants et règles métier

- Seul l'`OWNER` de la campagne peut créer ou révoquer une `ContentAccessRule`.
- Une `ContentAccessRule` ne peut être créée que pour un document `SHARED`.
- Une règle est unique par `(documentId, target)` — index unique sur `(documentId, targetType, targetId)` en base, avec `NULLS NOT DISTINCT` pour `targetId = NULL` (cas `AllMembersTarget`).
- La révocation est une suppression physique — une règle révoquée n'existe plus.

#### Domain Events

```
ContentAccessGranted  { ruleId, campaignId, documentId, target, occurredAt }
ContentAccessRevoked  { ruleId, campaignId, documentId, occurredAt }
```

---

## 7. Bounded Context — Content Library

### Responsabilité

Tout le contenu éditorial de la campagne. Le modèle central est `Document` —
une unité de contenu modulaire composée de `DocumentBlock` fortement typés.

`NPC` et `PlayerCharacter` sont des **entités avec repository** (pas des agrégats racines)
— elles ont leur propre Id et leur propre cycle de vie, mais pas d'entités enfants
à protéger transactionnellement.

---

### Agrégat : `Document`

```
Document
├── id          : DocumentId
├── campaignId  : CampaignId
├── type        : DocumentType
├── customType  : String?           — renseigné si type = CUSTOM
├── title       : String
├── slug        : Slug              — unique par (campaignId, type) — usage affichage uniquement
├── visibility  : Visibility
├── templateId  : TemplateId?
├── folderId    : FolderId?         — null = document non classé
├── appliedTemplateVersion : Int?   — null = pas de template ou sync jamais effectuée
├── tagIds      : TagId[]           — références aux Tags de la campagne
├── blocks      : DocumentBlock[]   — entités enfants, cycle de vie lié (zéro ou plusieurs)
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

```
DocumentType
├── NOTE        — note libre du MJ
├── NPC         — fiche PNJ
├── CHARACTER   — fiche personnage joueur
├── SCENARIO    — document narratif d'un scénario
├── SCENE       — document d'une scène
├── LOCATION    — fiche de lieu
└── CUSTOM      — type défini librement, précisé dans customType
```

#### Entité enfant : `DocumentBlock`

```
DocumentBlock
├── id          : BlockId
├── documentId  : DocumentId
├── kind        : BlockKind
├── label       : String?
├── order       : Int
├── value       : BlockValue        — value object fortement typé selon kind
├── isPrivate   : Boolean
├── isLocked    : Boolean           — champ verrouillé par le MJ (joueur ne peut pas modifier)
│                                     défaut : false — modélisé, non activé dans le MVP
└── audit       : AuditInfo         — pas de SoftDelete, suppression physique
```

#### Hiérarchie `BlockValue`

`BlockValue` est une hiérarchie de value objects scellés. Chaque sous-type
correspond exactement à un `BlockKind` — aucun champ optionnel superflu.

```
BlockValue  (abstract)
├── TextBlockValue
│   └── content : String
│
├── FieldBlockValue
│   └── value : String
│
├── StatBarBlockValue
│   ├── current : Int
│   └── max     : Int
│
├── RelationBlockValue
│   ├── targetId   : DocumentId
│   └── targetType : DocumentType
│
├── ListBlockValue
│   └── items : String[]
│
├── ItemBlockValue
│   ├── name       : String
│   ├── quantity   : Int
│   └── properties : Map<String, String>    — clé/valeur libre
│
├── ChecklistBlockValue
│   └── items : ChecklistItem[]
│       ├── label   : String
│       └── checked : Boolean
│
└── ImageBlockValue
    ├── url     : String
    └── caption : String?
```

```
BlockKind
├── TEXT
├── FIELD
├── STAT_BAR
├── RELATION
├── LIST
├── ITEM
├── CHECKLIST
└── IMAGE
```

#### Invariants et règles métier

- Un `Document` peut avoir zéro ou plusieurs blocs.
- L'ordre des blocs est géré par l'agrégat `Document`.
- Un bloc `RELATION` valide que le `targetId` appartient à la même campagne.
- Un bloc `isPrivate = true` n'est jamais exposé aux joueurs, même si le Document est `SHARED`.
- Un bloc `isLocked = true` ne peut pas être modifié par le joueur `ownerId` du personnage associé.
  Seul le MJ peut modifier un bloc verrouillé. Non activé dans le MVP — valeur par défaut `false`.
- Soft delete `Document` → suppression physique de tous ses blocs.
- Un `Document` créé depuis un template est un snapshot indépendant — le template peut changer sans affecter le document.
- `customType` est obligatoire si `type = CUSTOM`, null sinon — invariant garanti par le constructeur.
- Le `slug` est unique par `(campaignId, type)`.
- La visibilité par défaut d'un `Document` associé à un `PlayerCharacter` sans `ownerId` est `PRIVATE`. Le MJ la change explicitement pour la partager.

**Co-création obligatoire** : un `Document` avec `type ∈ {NPC, CHARACTER, SCENARIO, SCENE}`
**ne doit être créé que via la factory de l'entité correspondante** (voir AD-21).
La création directe d'un Document de ces types sans son enveloppe métier est interdite.

**Backlinks** : un bloc `RELATION` dont le `targetId` pointe vers un document soft-deleted
n'est pas affiché dans les backlinks. La requête de backlinks filtre `DOCUMENT.isDeleted = false`.

#### Domain Events

```
DocumentCreated           { documentId, campaignId, type, createdById, occurredAt }
DocumentTitleUpdated      { documentId, oldTitle, newTitle, occurredAt }
DocumentVisibilityChanged { documentId, oldVisibility, newVisibility, occurredAt }
DocumentMovedToFolder     { documentId, oldFolderId, newFolderId, occurredAt }
DocumentDeleted           { documentId, campaignId, occurredAt }
BlockAdded                { documentId, blockId, kind, occurredAt }
BlockUpdated              { documentId, blockId, occurredAt }
BlockRemoved              { documentId, blockId, occurredAt }
BlockReordered            { documentId, occurredAt }
```

---

### Entité : `Tag`

Tag de campagne réutilisable. Créé par le MJ au niveau de la campagne.
Assigné à n'importe quel Document de la même campagne.

```
Tag
├── id         : TagId
├── campaignId : CampaignId
├── label      : String           — unique par campaignId (insensible à la casse)
├── color      : String?          — hex ou nom de couleur
├── audit      : AuditInfo
└── softDelete : SoftDelete
```

#### Invariants et règles métier

- Le `label` est unique par campagne (insensible à la casse).
- Supprimer un Tag retire automatiquement sa référence de tous les Documents de la campagne (via event `TagDeleted`).
- Un Tag n'appartient qu'à une campagne — non partageable entre campagnes.

#### Domain Events

```
TagCreated  { tagId, campaignId, label, occurredAt }
TagUpdated  { tagId, campaignId, occurredAt }
TagDeleted  { tagId, campaignId, occurredAt }
```

---

### Entité : `NPC`

Entité de premier niveau avec repository. Pas d'entités enfants — pas d'agrégat racine.
Porte les règles métier propres au PNJ. Le contenu est dans son `Document` associé.

```
NPC
├── id                  : NpcId
├── campaignId          : CampaignId
├── documentId          : DocumentId
├── name                : String           — dénormalisé, synchronisé via DocumentTitleUpdated
├── status              : NpcStatus
├── linkedCharacterId   : CharacterId?     — association narrative optionnelle
├── audit               : AuditInfo
└── softDelete          : SoftDelete
```

```
NpcStatus
├── ALIVE
├── DEAD
├── MISSING
└── UNKNOWN
```

#### Invariants et règles métier

- `name` est synchronisé avec `Document.title` via le handler de `DocumentTitleUpdated`.
  Ce dispatch est **synchrone in-process** dans le monolithe MVP — cohérence garantie dans la même transaction.
- Un NPC `DEAD` reste consultable.
- `linkedCharacterId` est une association narrative — la suppression du `PlayerCharacter` ne supprime pas le NPC.
- Soft delete NPC → soft delete de son Document associé (même transaction).
- Les blocs `isPrivate = true` du Document ne sont jamais exposés aux joueurs.

#### Domain Events

```
NpcCreated                { npcId, campaignId, documentId, occurredAt }
NpcStatusChanged          { npcId, oldStatus, newStatus, occurredAt }
NpcLinkedToCharacter      { npcId, characterId, occurredAt }
NpcUnlinkedFromCharacter  { npcId, occurredAt }
```

---

### Entité : `PlayerCharacter`

Entité de premier niveau avec repository. Pas d'entités enfants — pas d'agrégat racine.
Porte les règles d'appartenance et de permissions. Le contenu est dans son `Document` associé.

```
PlayerCharacter
├── id              : CharacterId
├── campaignId      : CampaignId
├── documentId      : DocumentId
├── name            : String           — dénormalisé, synchronisé via DocumentTitleUpdated
├── ownerId         : UserId?          — null = créé par le MJ, en attente d'association
│                                        ou personnage joué par le MJ (joueur absent)
├── linkedNpcId     : NpcId?           — association narrative optionnelle
├── status          : CharacterStatus
├── audit           : AuditInfo
└── softDelete      : SoftDelete
```

```
CharacterStatus
├── ACTIVE
├── RETIRED
└── DEAD
```

#### Invariants et règles métier

- Seul le `ownerId` ou le MJ peut modifier les blocs du Document associé.
- Les blocs `isPrivate = true` sont visibles uniquement par le MJ.
- `ownerId = null` → Document avec visibilité `PRIVATE` par défaut.
  Le MJ peut la modifier explicitement (SHARED ou PUBLIC) pour partager la fiche avec le groupe
  avant qu'un joueur soit associé (ex. : session avec un PJ temporaire ou joueur absent).
  L'association d'un `ownerId` ne change **pas automatiquement** la visibilité — c'est une action explicite du MJ.
- `linkedNpcId` est une association narrative — la suppression du NPC ne supprime pas le personnage.
- Soft delete PlayerCharacter → soft delete de son Document associé (même transaction).

#### Domain Events

```
CharacterCreated          { characterId, campaignId, documentId, occurredAt }
CharacterOwnerAssigned    { characterId, ownerId, occurredAt }
CharacterStatusChanged    { characterId, oldStatus, newStatus, occurredAt }
CharacterLinkedToNpc      { characterId, npcId, occurredAt }
CharacterUnlinkedFromNpc  { characterId, occurredAt }
```

---

### Agrégat : `Scenario`

Gère la structure ordonnée des scènes et le statut de progression.
Le contenu narratif global est dans son Document associé.

```
Scenario
├── id          : ScenarioId
├── campaignId  : CampaignId
├── documentId  : DocumentId
├── title       : String           — dénormalisé, synchronisé via DocumentTitleUpdated
├── slug        : Slug             — unique par campagne
├── order       : Int              — position du scénario dans la campagne, géré par ScenarioOrderService
│                                    synchronisé par défaut avec l'ordre d'affichage dans le dossier "Scénarios"
├── status      : ScenarioStatus
├── scenes      : Scene[]          — entités enfants, cycle de vie lié (zéro ou plusieurs)
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

```
ScenarioStatus
├── DRAFT
├── READY
├── PLAYED
└── ARCHIVED
```

#### Entité enfant : `Scene`

```
Scene
├── id              : SceneId
├── scenarioId      : ScenarioId
├── documentId      : DocumentId
├── title           : String       — dénormalisé, synchronisé via DocumentTitleUpdated
├── order           : Int          — position dans le scénario, géré par Scenario
├── status          : SceneStatus
├── linkedNpcIds    : NpcId[]      — références légères
└── audit           : AuditInfo    — pas de SoftDelete, suppression en cascade
```

```
SceneStatus
├── PENDING
├── PLAYED
└── SKIPPED
```

#### Invariants et règles métier

- Un `Scenario` peut contenir **zéro, une ou plusieurs** scènes.
- Une `Scene` ne peut pas exister sans son `Scenario` parent.
- L'ordre des scènes est une responsabilité de l'agrégat `Scenario`.
- `Scenario.order` représente la position narrative dans la campagne. Par défaut synchronisé
  avec l'ordre d'affichage dans le dossier système "Scénarios". Le MJ peut les dissocier
  (point d'extension post-MVP : `Scenario.followsFolderOrder: Boolean = true`).
- Un `Scenario` `ARCHIVED` est en lecture seule.
- Soft delete `Scenario` → suppression physique des `Scene` et soft delete de leurs Documents.
- `linkedNpcIds` : si un NPC est soft-deleted, sa référence est retirée sans supprimer la scène.

#### Domain Events

```
ScenarioCreated       { scenarioId, campaignId, documentId, occurredAt }
ScenarioStatusChanged { scenarioId, oldStatus, newStatus, occurredAt }
ScenarioReordered     { campaignId, occurredAt }
SceneAdded            { scenarioId, sceneId, documentId, occurredAt }
SceneRemoved          { scenarioId, sceneId, occurredAt }
SceneStatusChanged    { sceneId, scenarioId, oldStatus, newStatus, occurredAt }
SceneReordered        { scenarioId, occurredAt }
SceneNpcLinked        { sceneId, npcId, occurredAt }
SceneNpcUnlinked      { sceneId, npcId, occurredAt }
```

---

### Agrégat : `DocumentTemplate`

Schéma de blocs définissant la structure attendue pour un type de Document.

```
DocumentTemplate
├── id              : TemplateId
├── name            : String
├── documentType    : DocumentType
├── gameSystemId    : GameSystemId?
├── scope           : TemplateScope
├── ownerId         : UserId?          — null si BUILTIN
├── campaignId      : CampaignId?      — null si scope USER ou BUILTIN
├── version         : Int              — incrémenté à chaque modification du schéma de blocs
│                                        commence à 1, jamais décrémenté
├── schema          : BlockSchema[]
├── audit           : AuditInfo
└── softDelete      : SoftDelete
```

```
TemplateScope
├── BUILTIN    — fourni par l'application, non modifiable
├── CAMPAIGN   — défini pour une campagne spécifique
└── USER       — défini par un utilisateur, portable entre ses campagnes
```

#### Combinaisons valides scope / ownerId / campaignId

| scope    | ownerId    | campaignId  |
|---|---|---|
| BUILTIN  | null       | null        |
| CAMPAIGN | non-null   | non-null    |
| USER     | non-null   | null        |

**Invariant** : toute autre combinaison est rejetée par le constructeur.

#### Incrément de version

`DocumentTemplate.version` est incrémenté à chaque appel à `AddBlock()`, `RemoveBlock()`,
`ReorderBlocks()` ou `UpdateBlockSchema()`. `Document.appliedTemplateVersion` est comparé
à `template.version` pour détecter qu'une synchronisation est disponible (UC-18).

#### Value Object : `BlockSchema`

```
BlockSchema
├── kind         : BlockKind
├── label        : String
├── required     : Boolean
└── defaultValue : BlockValue?    — instance concrète du sous-type correspondant
```

**Note sur `required`** : un bloc `required = true` dans le schéma est un indicateur
pour l'UI (champ mis en avant). La validation applicative lors de UC-18 ajoute ces blocs
s'ils sont absents — elle ne bloque pas la sauvegarde d'un document incomplet.

#### Invariants et règles métier

- `BUILTIN` non modifiable, non supprimable.
- `CAMPAIGN` visible uniquement par les membres de la campagne.
- `USER` portable entre les campagnes de son propriétaire.
- Un Document créé depuis un template est un snapshot indépendant.

#### Domain Events

```
TemplateCreated          { templateId, scope, occurredAt }
TemplateSchemaUpdated    { templateId, newVersion, occurredAt }
TemplateAppliedToDocument { templateId, documentId, occurredAt }
```

---

### Agrégat : `Folder`

Conteneur organisationnel créé par le MJ pour regrouper ses documents librement.
Chaque campagne démarre avec 4 dossiers système créés automatiquement.

```
Folder
├── id                : FolderId
├── campaignId        : CampaignId
├── name              : String
├── slug              : Slug              — unique par campaignId
├── defaultTemplateId : TemplateId?       — template appliqué à la création d'un doc dans ce dossier
├── isSystem          : Boolean           — true = dossier créé par le système, non supprimable
├── order             : Int               — position dans la navigation
├── audit             : AuditInfo
└── softDelete        : SoftDelete
```

**Dossiers système créés à l'initialisation de la campagne :**

| Nom | isSystem | Template par défaut |
|---|---|---|
| PNJ | true | "Fiche PNJ générique" |
| Personnages joueurs | true | "Fiche personnage générique" |
| Scénarios | true | — |
| Notes | true | — |

#### Invariants et règles métier

- Un dossier système (`isSystem = true`) ne peut pas être supprimé.
- Tous les dossiers (y compris système) peuvent être renommés.
- Le `slug` est régénéré depuis le `name` à la création, jamais modifié après.
- Supprimer un dossier non-système nécessite de traiter ses documents (déplacer ou déclasser).
- Changer le `defaultTemplateId` n'affecte jamais les documents existants dans le dossier.
- L'ordre des dossiers est géré par `FolderOrderService` (application service — voir AD-20).

#### Références entre documents — backlinks

Les références entre documents existent via `RelationBlockValue { targetId: DocumentId, targetType: DocumentType }`.
Un document peut pointer vers n'importe quel autre document de la campagne via un bloc `RELATION`.

Les **backlinks** (documents qui pointent *vers* un document donné) sont résolus
via une requête sur `DOCUMENT_BLOCK` filtrée par `kind = RELATION`, `value->>'targetId' = ?`
**et `DOCUMENT.isDeleted = false`**. Un backlink pointant vers un document supprimé n'est pas affiché.

---

## 8. Bounded Context — Session Conduct

### Responsabilité

Préparation, conduite en temps réel et clôture des sessions de jeu.
Consomme les autres contextes par Id uniquement.

---

### Agrégat : `Session`

```
Session
├── id                : SessionId
├── campaignId        : CampaignId
├── scenarioId        : ScenarioId?      — null = session libre
├── title             : String
├── slug              : Slug
├── scheduledAt       : DateTime?
├── startedAt         : DateTime?
├── endedAt           : DateTime?
├── status            : SessionStatus
├── participantIds    : CharacterId[]    — références légères
├── selectedNpcIds    : NpcId[]          — PNJ sélectionnés pour la session
│                                          déduits automatiquement des scènes du scénario
│                                          modifiables manuellement par le MJ
├── pinnedItems       : PinnedItem[]     — value object avec ordre et date
├── liveNotes         : LiveNote[]       — entités enfants
├── summary           : SessionSummary? — entité enfant unique
├── audit             : AuditInfo
└── softDelete        : SoftDelete
```

```
SessionStatus
├── PLANNED
├── LIVE
├── CLOSED
└── ARCHIVED
```

#### Entité enfant : `LiveNote`

Note prise pendant ou après une session. Peut être créée par le MJ ou par un joueur.

```
LiveNote
├── id                : LiveNoteId
├── sessionId         : SessionId
├── content           : String
├── authorId          : UserId          — MJ ou joueur authentifié
├── authorRole        : LiveNoteAuthorRole  — MJ ou PLAYER (détermine les règles de visibilité par défaut)
├── visibility        : Visibility      — PRIVATE (MJ), PLAYER_PRIVATE (joueur), ou SHARED/PUBLIC
│                                         jamais null — défaut selon authorRole
├── linkedDocumentId  : DocumentId?
└── audit             : AuditInfo       — pas de SoftDelete
```

```
LiveNoteAuthorRole
├── GM      — note créée par le MJ
└── PLAYER  — note créée par un joueur
```

**Visibilité par défaut selon authorRole** :
- `GM` → `PRIVATE` (note privée MJ par défaut, peut être partagée)
- `PLAYER` → `PLAYER_PRIVATE` (note personnelle joueur par défaut, invisible au MJ)

**Contraintes de visibilité par authorRole** :
- Une `LiveNote` avec `authorRole = GM` ne peut pas avoir `visibility = PLAYER_PRIVATE`.
- Une `LiveNote` avec `authorRole = PLAYER` ne peut pas avoir `visibility = PRIVATE`.

#### Entité enfant : `SessionSummary`

```
SessionSummary
├── id          : SummaryId
├── sessionId   : SessionId
├── content     : String
├── visibility  : Visibility
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Invariants et règles métier

- **Une seule session `LIVE` par campagne** à un instant donné — invariant fort.
- Transitions autorisées uniquement : `PLANNED → LIVE → CLOSED → ARCHIVED`.
- Aucun retour en arrière sur les transitions de statut.
- Les `LiveNote` de type MJ sont créées avec `visibility = PRIVATE` par défaut.
- Les `LiveNote` de type joueur sont créées avec `visibility = PLAYER_PRIVATE` par défaut.
- Un joueur ne peut créer des LiveNotes que pendant une session LIVE (pas PLANNED, pas a posteriori sur CLOSED).
  Le MJ peut créer des LiveNotes sur une session LIVE ou CLOSED (ajout rétroactif).
- Un `SessionSummary` `PRIVATE` est visible uniquement par le MJ.
- `participantIds`, `selectedNpcIds` et `pinnedItems` sont des références légères —
  si une entité référencée est supprimée, la référence est retirée.
- Soft delete `Session` → suppression physique des `LiveNote`, soft delete du `SessionSummary`.

**Permissions d'édition par statut** :

| Statut     | Métadonnées | Ajout LiveNote MJ | Ajout LiveNote Joueur | Édition SessionSummary | Épinglage |
|------------|-------------|-------------------|-----------------------|------------------------|-----------|
| `PLANNED`  | Oui         | Non               | Non                   | Non                    | Oui       |
| `LIVE`     | Oui         | Oui               | Oui                   | Non                    | Oui       |
| `CLOSED`   | Non         | Oui (rétro)       | Non                   | Oui                    | Non       |
| `ARCHIVED` | Non         | Non               | Non                   | Non — lecture seule    | Non       |

> Une session `CLOSED` reste éditable pour les notes MJ rétroactives et le résumé.
> Les joueurs ne peuvent plus créer de LiveNotes sur une session CLOSED.
> L'état `ARCHIVED` est le seul état véritablement immuable.

**Déduction automatique des `selectedNpcIds`** :
Quand un `scenarioId` est associé à une session, `SessionNpcSelector` (application service)
calcule la liste initiale depuis l'union des `linkedNpcIds` de toutes les scènes du scénario.
Le MJ peut ensuite ajouter ou retirer des NpcId manuellement.

#### Domain Events

```
SessionPlanned                  { sessionId, campaignId, scheduledAt, occurredAt }
SessionStarted                  { sessionId, campaignId, startedAt, occurredAt }
SessionClosed                   { sessionId, campaignId, endedAt, occurredAt }
SessionArchived                 { sessionId, campaignId, occurredAt }
SessionNpcSelected              { sessionId, npcId, occurredAt }
SessionNpcDeselected            { sessionId, npcId, occurredAt }
LiveNoteAdded                   { sessionId, liveNoteId, authorId, authorRole, occurredAt }
LiveNoteAddedPostSession        { sessionId, liveNoteId, authorId, occurredAt }
LiveNoteRemoved                 { sessionId, liveNoteId, occurredAt }
LiveNoteVisibilityChanged       { sessionId, liveNoteId, oldVisibility, newVisibility, occurredAt }
DocumentPinnedToSession         { sessionId, documentId, occurredAt }
DocumentUnpinnedFromSession     { sessionId, documentId, occurredAt }
SessionSummaryCreated           { sessionId, summaryId, visibility, occurredAt }
SessionSummaryUpdated           { sessionId, summaryId, occurredAt }
SessionSummaryVisibilityChanged { sessionId, summaryId, oldVisibility, newVisibility, occurredAt }
```

---

## 9. Domain Events — vue globale

### Identity & Access
```
UserRegistered, UserRoleChanged, UserDeactivated
```

### Campaign Management
```
CampaignCreated, CampaignStatusChanged, CampaignOwnerTransferred
MemberJoined, MemberRemoved
InvitationCreated, InvitationRevoked
GuestAccessCreated, GuestAccessExpired, GuestAccessCharacterAssigned
ContentAccessGranted, ContentAccessRevoked
```

### Content Library
```
DocumentCreated, DocumentTitleUpdated, DocumentVisibilityChanged
DocumentMovedToFolder, DocumentDeleted
BlockAdded, BlockUpdated, BlockRemoved, BlockReordered
TagCreated, TagUpdated, TagDeleted
TemplateCreated, TemplateSchemaUpdated, TemplateAppliedToDocument
FolderCreated, FolderRenamed, FolderDeleted
NpcCreated, NpcStatusChanged, NpcLinkedToCharacter, NpcUnlinkedFromCharacter
CharacterCreated, CharacterOwnerAssigned, CharacterStatusChanged
CharacterLinkedToNpc, CharacterUnlinkedFromNpc
ScenarioCreated, ScenarioStatusChanged, ScenarioReordered
SceneAdded, SceneRemoved, SceneStatusChanged, SceneReordered
SceneNpcLinked, SceneNpcUnlinked
```

### Session Conduct
```
SessionPlanned, SessionStarted, SessionClosed, SessionArchived
SessionNpcSelected, SessionNpcDeselected
LiveNoteAdded, LiveNoteAddedPostSession, LiveNoteRemoved, LiveNoteVisibilityChanged
DocumentPinnedToSession, DocumentUnpinnedFromSession
SessionSummaryCreated, SessionSummaryUpdated, SessionSummaryVisibilityChanged
```

---

## 10. Décisions d'architecture domaine

### AD-01 — Séparation entités domaine / modèles infrastructure
**Décision** : Les entités domaine ne dépendent d'aucun framework.
Un mapper dédié assure la conversion en couche Infrastructure.
**Raison** : Domaine agnostique, tests unitaires facilités, évolution de stack sans impact domaine.

### AD-02 — Shared Kernel pour les primitives partagées
**Décision** : Id typés, AuditInfo, SoftDelete, Email, Slug, Visibility, PinnedItem, RequesterId
et abstractions vivent dans `Core`.
**Raison** : Éviter la duplication sans créer de couplage entre contextes.

### AD-03 — Id typés — jamais de UUID nu dans le domaine
**Décision** : Chaque agrégat et entité avec repository possède son propre type d'Id.
**Raison** : Sécurité de type à la compilation.

### AD-04 — SoftDelete séparé de AuditInfo
**Décision** : `SoftDelete` est un value object distinct de `AuditInfo`.
**Raison** : Séparation des préoccupations — audit ≠ suppression.

### AD-05 — BlockValue comme hiérarchie de value objects scellés
**Décision** : `BlockValue` est une hiérarchie de sous-types, un par `BlockKind`.
**Raison** : Cohérence interne des value objects. Pas de champs optionnels sans sens selon le type.

### AD-06 — NPC et PlayerCharacter — entités avec repository, pas agrégats racines
**Décision** : `NPC` et `PlayerCharacter` ont leur propre Id et leur propre repository
mais ne sont pas des racines d'agrégat au sens DDD strict.
**Raison** : Ils n'ont pas d'entités enfants à protéger transactionnellement. Leurs invariants
sont locaux à l'entité elle-même.

### AD-07 — Lien NPC ↔ PlayerCharacter optionnel et non structurant
**Décision** : Association narrative optionnelle dans les deux sens.
**Raison** : Permet la future promotion sans migration. Cycle de vie indépendant.

### AD-08 — Séparation User authentifié et accès invité
**Décision** : Les joueurs invités sans compte sont représentés par `GuestAccess`
dans Campaign Management. `UserRole` ne contient que `GM` et `PLAYER`.
**Raison** : Un `User` représente une identité persistante. Un invité est un accès temporaire.
Les unifier créerait des champs conditionnellement valides.

### AD-09 — AccessPolicy comme service domaine
**Décision** : La visibilité des contenus est gérée par un service domaine `AccessPolicy`
qui persiste des entités légères `ContentAccessRule`.
**Raison** : `ContentAccessRule` est immuable — créée ou révoquée, jamais modifiée.

### AD-10 — DocumentTemplate — combinaisons scope/ownerId/campaignId protégées
**Décision** : Le constructeur de `DocumentTemplate` rejette toute combinaison invalide.
**Raison** : Éviter les états incohérents non détectables à l'exécution.

### AD-11 — Ordre des scénarios persisté via `Scenario.order`
**Décision** : `Scenario` porte un champ `order: Int`. Le réordonnancement est orchestré
par `ScenarioOrderService`.
**Raison** : Sans champ de persistance, l'ordre ne peut pas être sauvegardé.

### AD-12 — PinnedItem value object pour les documents épinglés
**Décision** : Les documents épinglés à une session sont représentés par `pinnedItems: PinnedItem[]`.
**Raison** : Capture l'ordre et la date d'épinglage — sémantique explicite dans le domaine.

### AD-13 — LiveNote avec visibilité contrôlée et auteur typé
**Décision** : `LiveNote` porte un champ `visibility` et un `authorRole` (GM ou PLAYER).
La visibilité par défaut dépend du rôle : `PRIVATE` pour le MJ, `PLAYER_PRIVATE` pour le joueur.
**Raison** : Le MJ et les joueurs ont des intentions différentes pour leurs notes de session.
La valeur par défaut garantit qu'aucune note n'est accidentellement exposée.

### AD-14 — Références légères entre contextes — Id uniquement
**Décision** : Un contexte ne référence jamais une entité d'un autre contexte,
uniquement son Id typé.
**Raison** : Découplage strict. Chaque contexte évolue indépendamment.

### AD-15 — Synchronisation des champs dénormalisés via domain event synchrone
**Décision** : `NPC.name`, `PlayerCharacter.name`, `Scenario.title` et `Scene.title`
sont dénormalisés depuis `Document.title`. Leur synchronisation est assurée
par un handler synchrone in-process qui réagit à `DocumentTitleUpdated` **dans la même transaction**.
**Raison** : Le dispatch synchrone dans le monolithe MVP élimine la fenêtre d'incohérence.
Les handlers sont enregistrés via un médiateur in-process (ex. MediatR). Si l'architecture
évolue vers un bus asynchrone, ce comportement doit être explicitement documenté dans un ADR de migration.

### AD-16 — Dossiers utilisateur — structure libre, non imposée
**Décision** : La structure du contenu d'une campagne est définie par le MJ via des `Folder`.
Le système fournit 4 dossiers système non suppressibles à l'initialisation.
**Raison** : Chaque MJ a une organisation différente selon son système de jeu et son style.

### AD-17 — Synchronisation template — action manuelle, jamais automatique
**Décision** : Modifier un template ne propage jamais automatiquement les changements
aux documents existants. `ApplyTemplateToDocument` est une action explicite du MJ.
**Raison** : Un MJ qui a rempli ses fiches ne doit pas voir son travail écrasé.
`Document.appliedTemplateVersion` comparé à `DocumentTemplate.version` détecte la disponibilité d'une sync.

### AD-18 — RequesterId — type union pour l'autorisation GuestAccess
**Décision** : `AccessPolicy.CanAccess` accepte un `RequesterId` (type union scellé : `AuthenticatedRequesterId` ou `GuestRequesterId`) au lieu d'un `UserId` seul.
**Raison** : Un GuestAccess n'a pas de UserId. La signature précédente rendait l'autorisation des invités impossible sans contournement. Le type union permet à AccessPolicy de traiter les deux cas dans une seule méthode avec des règles explicites pour chaque type de demandeur.
**Alternatives écartées** : surcharge de méthode — fragmente la logique d'autorisation et crée un risque d'oubli. Conversion invité → UserId fictif — crée une identité fantôme non traçable.

### AD-19 — Tags comme entités de campagne dans Content Library
**Décision** : `Tag` est une entité légère avec `TagId`, appartenant à une campagne, gérée dans Content Library. `Document.tagIds: TagId[]` référence ces entités. La suppression d'un Tag déclenche le retrait de sa référence de tous les Documents via `TagDeleted` event.
**Raison** : Des tags réutilisables au niveau campagne nécessitent une identité propre pour pouvoir être renommés ou supprimés globalement. Un value object embarqué dans chaque Document rendrait le renommage bulk impossible.
**Alternatives écartées** : Tag comme value object dans le Shared Kernel — trop couplé, pas de cycle de vie propre. Tag comme entité dans Campaign Management — les tags sont une préoccupation du contenu, pas de l'organisation de campagne.

### AD-20 — Dispatch synchrone des domain events dans le monolithe MVP
**Décision** : Les domain events (notamment `DocumentTitleUpdated`, `TagDeleted`) sont dispatchés synchrones in-process dans la même transaction, via un médiateur in-process (MediatR ou équivalent). Aucun bus de messages externe dans le MVP.
**Raison** : Dans un monolithe modulaire, le dispatch synchrone élimine la fenêtre d'incohérence entre l'émission de l'event et son traitement. La complexité d'un bus asynchrone n'est pas justifiée pour le MVP.
**Conséquences** : si un handler échoue, la transaction entière est annulée — comportement correct pour les invariants de cohérence. Si l'architecture évolue vers des services distribués, les handlers asynchrones doivent être documentés dans un ADR de migration dédié.

### AD-21 — Pattern factory pour la co-création Document + entité métier
**Décision** : Créer un NPC, PlayerCharacter, Scenario ou Scene implique toujours la co-création atomique d'un Document et de son enveloppe métier. Cette co-création est encapsulée dans une méthode factory statique sur chaque entité.
```
NPC.Create(campaignId, name, folderId?, templateId?) → (NPC, Document)
Scenario.Create(campaignId, title, folderId?) → (Scenario, Document)
```
La couche Application persiste les deux entités dans la même `IUnitOfWork`. Créer un `Document` avec `type = NPC|CHARACTER|SCENARIO|SCENE` directement (sans l'enveloppe) est interdit et doit lever une `DomainException`.
**Raison** : Sans factory, un Document de type NPC sans entité NPC correspondante est un état incohérent non détectable à la compilation. La factory est le seul chemin de création valide.

### AD-22 — Backlinks orphelins — non affichés, jamais d'erreur
**Décision** : La requête de backlinks filtre `DOCUMENT.isDeleted = false`. Un bloc RELATION pointant vers un document supprimé ne génère ni erreur ni backlink dans la liste. Il reste en base (suppression physique du bloc non implémentée pour les RELATION orphelins dans le MVP).
**Raison** : Afficher un backlink cassé génère une mauvaise UX sans valeur. Supprimer automatiquement les blocs RELATION à la suppression d'un document cible nécessite une cascade cross-agrégats coûteuse. Le filtrage à la lecture est la solution la plus simple et la plus sûre.
**Alternatives écartées** : cascade de suppression sur les blocs RELATION orphelins — couplage cross-contextes excessif, performance dégradée pour les documents très référencés. Marquage "lien cassé" visible — UX dégradée pour un cas rare.

### AD-23 — Stratégie URL — Id UUID, slug pour l'affichage uniquement
**Décision** : Les URLs de l'application utilisent les Id UUID comme identifiants primaires. Les slugs ne sont jamais dans les routes. Pattern : `/campaigns/{campaignId}/documents/{documentId}`.
**Raison** : Le slug est unique par `(campaignId, type)` — l'inclure dans l'URL nécessiterait d'y inclure aussi le type, ou d'accepter des collisions cross-type. Les Id UUID sont non-ambigus, stables et ne nécessitent aucune logique de déduplication dans le routage. Les slugs restent utiles pour la recherche et l'affichage textuel.
**Alternatives écartées** : URLs basées sur le slug avec type dans le chemin (`/campaigns/{slug}/npcs/{npcSlug}`) — fragmente les URLs par type, complique les liens directs et les bookmarks.

### AD-24 — Visibilité par défaut des personnages sans association joueur
**Décision** : Un `PlayerCharacter` créé avec `ownerId = null` a un Document avec `visibility = PRIVATE` par défaut. Le MJ peut explicitement changer la visibilité pour partager la fiche avec le groupe avant l'association d'un joueur. L'association d'un `ownerId` ne change pas automatiquement la visibilité.
**Raison** : La visibilité est une décision du MJ, pas une conséquence automatique de l'association. Un MJ peut jouer un personnage temporairement sans vouloir l'exposer, ou au contraire partager la fiche à l'avance. L'automatisme créerait des expositions accidentelles.

---

## 11. Hors périmètre MVP — points d'extension documentés

| Fonctionnalité | Point d'extension prévu | Contexte |
|---|---|---|
| Règles système de jeu | `GameSystem` prêt à recevoir un `RuleSet` | Campaign Management |
| Templates par système | `DocumentTemplate.gameSystemId` déjà modélisé | Content Library |
| Promotion NPC → PJ | Liens `linkedCharacterId` / `linkedNpcId` déjà présents | Content Library |
| Versioning des documents | Architecture compatible avec un historique de blocs | Content Library |
| OAuth / SSO | Contexte Identity isolé, remplacement sans impact | Identity & Access |
| Templates communautaires | `TemplateScope.USER` déjà modélisé | Content Library |
| AccessPolicy en contexte autonome | `ContentAccessRule` extractible si la complexité l'exige | Campaign Management |
| Temps réel (WebSocket) | `SessionStatus.LIVE` + domain events — base pour un bus événementiel | Session Conduct |
| SessionSummary deux versions | Modèle actuel : un seul résumé par session. Multi-version = post-MVP | Session Conduct |
| Types de document extensibles | `DocumentType.CUSTOM` + `customType: String` déjà modélisé | Content Library |
| Verrouillage de champs | `DocumentBlock.isLocked` modélisé, non activé dans le MVP | Content Library |
| Factions | Représentées via `Document(CUSTOM, "FACTION")` si nécessaire | Content Library |
| UserProjection locale | Si extraction de Campaign Management en service : ajouter projection via events | Campaign Management |
| Dossiers imbriqués (sous-dossiers) | `Folder.parentFolderId?` — non activé MVP, un seul niveau de dossiers | Content Library |
| Synchronisation template automatique | `Document.appliedTemplateVersion` modélisé — propagation auto non activée | Content Library |
| Dissociation ordre scénario / ordre dossier | `Scenario.followsFolderOrder: Boolean = true` — non activé MVP | Content Library |
| GameSystem custom partagé entre GM | `GameSystem.ownerId` = null pour les built-in, scope private en MVP | Campaign Management |

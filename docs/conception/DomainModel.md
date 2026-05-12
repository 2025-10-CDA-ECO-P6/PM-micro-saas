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
| **Joueur invité** | Accès temporaire sans compte. Rejoint via token d'invitation, sans identité persistante. |

---

## 2. Ubiquitous Language

Vocabulaire partagé du domaine. Ces termes sont utilisés tels quels dans le code,
la documentation et les conversations.

| Terme | Définition |
|---|---|
| **Campagne** | Espace organisationnel regroupant scénarios, PNJ, personnages, sessions et notes d'une aventure JDR. |
| **MJ** | Maître du Jeu. Propriétaire de la campagne, seul à pouvoir modifier le contenu et gérer les accès. |
| **Joueur** | Membre authentifié d'une campagne, associé à un ou plusieurs personnages joueurs. |
| **Joueur invité** | Accès temporaire sans compte persistant. Représenté par un GuestAccess, pas un User. |
| **Scénario** | Structure narrative préparée par le MJ, composée de scènes ordonnées. |
| **Scène** | Unité narrative d'un scénario. Peut être liée à des PNJ. |
| **PNJ** | Personnage Non-Joueur. Entité narrative créée et gérée par le MJ. |
| **Personnage joueur** | Fiche d'un personnage appartenant à un joueur. Créée par le MJ ou le joueur. |
| **Document** | Unité de contenu modulaire. Tout contenu éditorial est un Document typé composé de blocs. |
| **Bloc** | Unité atomique de contenu dans un Document. Chaque bloc a un type et une valeur fortement typée. |
| **Template** | Schéma de blocs définissant la structure attendue d'un Document. Snapshot à la création — non lié après. |
| **Session** | Instance d'une partie jouée. Liée optionnellement à un scénario. |
| **Note live** | Note prise par le MJ pendant une session. Liée automatiquement à la session en cours. |
| **Résumé** | Compte-rendu d'une session clôturée. Peut être partagé aux joueurs. |
| **Visibilité** | Niveau d'accès d'un contenu : PRIVATE (MJ uniquement), SHARED (membres ciblés), PUBLIC (tous). |
| **AccessPolicy** | Service domaine gérant les autorisations d'accès aux contenus d'une campagne. |
| **GuestAccess** | Accès temporaire dans une campagne. N'est pas un User — pas d'identité persistante. |
| **Invitation** | Token généré par le MJ permettant à un joueur de rejoindre une campagne. |
| **Système de jeu** | Référentiel de règles d'un JDR (D&D 5e, Call of Cthulhu, etc.). Point d'extension futur. |
| **Slug** | Identifiant lisible généré depuis un titre. Utilisé dans les URLs. Unique dans son contexte. |
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
| **Content Library** | Tout le contenu éditorial | `Document`, `Scenario`, `DocumentTemplate` | `NPC`, `PlayerCharacter` |
| **Session Conduct** | Préparation, conduite et clôture des sessions | `Session` | — |

### Context Map

```
Core (Shared Kernel)
  └── consommé par tous les contextes
      └── fournit : Id typés, AuditInfo, SoftDelete, abstractions

Identity & Access  [Upstream]
  └── fournit UserId à tous les autres contextes

Campaign Management
  ├── consomme Identity (UserId)
  ├── fournit CampaignId à Content Library et Session Conduct
  └── héberge AccessPolicy — service domaine de visibilité des contenus

Content Library
  ├── consomme Campaign Management (CampaignId)
  ├── consomme Identity (UserId)
  └── fournit DocumentId, NpcId, CharacterId, ScenarioId à Session Conduct

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
DocumentId      — Content Library
BlockId         — Content Library
NpcId           — Content Library
CharacterId     — Content Library
ScenarioId      — Content Library
SceneId         — Content Library
TemplateId      — Content Library
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

#### Tag

```
Tag
├── label : String
└── color : String?    — hex ou nom de couleur
```

#### PinnedItem

Value object représentant un document épinglé à une session avec son ordre et sa date.

```
PinnedItem
├── documentId : DocumentId
├── order      : Int
└── pinnedAt   : DateTime
```

### Enumerations partagées

```
Visibility
├── PRIVATE        — visible uniquement par le MJ propriétaire de la campagne
├── PLAYER_PRIVATE — visible uniquement par le joueur créateur (createdById) — MJ exclu
├── SHARED         — visible par les membres ciblés via AccessPolicy
└── PUBLIC         — visible par tous les membres de la campagne
```

**Règle de résolution d'accès** (appliquée par `AccessPolicy.CanAccess`) :

1. `PRIVATE` → accès accordé au MJ uniquement.
2. `PLAYER_PRIVATE` → accès accordé au `createdById` du document uniquement.
   Le MJ n'a **pas** accès, même en tant que propriétaire de la campagne.
3. `PUBLIC` → accès accordé à tous les membres sans consulter les règles.
4. `SHARED` → accès accordé si et seulement si `AccessPolicy` contient une
   `ContentAccessRule` correspondant au demandeur.

**Invariant** : une `ContentAccessRule` ne peut être créée que pour un document `SHARED`.
Toute tentative de créer une règle sur un document `PRIVATE`, `PLAYER_PRIVATE` ou `PUBLIC`
est rejetée par `AccessPolicy`.

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
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Invariants et règles métier

- `isBuiltIn = true` → non modifiable, non supprimable.
  **Cet invariant est protégé dans l'entité domaine `GameSystem`**, pas dans un service applicatif.
  `GameSystem.Update()` et `GameSystem.Delete()` lèvent une `DomainException` si `isBuiltIn = true`.
- Un système custom appartient à l'utilisateur identifié par `audit.createdById`.

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
└── createdAt     : DateTime    — pas de AuditInfo complet, règle immuable
```

```
AccessTarget  (hiérarchie de value objects scellés)
├── AllMembersTarget
│   — cible tous les membres de la campagne
│
├── SpecificMemberTarget
│   └── userId : UserId        — cible un membre authentifié précis
│
└── SpecificCharacterTarget
    └── characterId : CharacterId  — cible le joueur associé à ce personnage
```

> **Note de typage** : `AccessTarget` est une hiérarchie de types scellés,
> pas un objet avec un discriminant enum et un champ `targetId` optionnel.
> Chaque sous-type ne contient que les données pertinentes — aucun champ
> conditionnellement invalide. Même principe que `BlockValue`.

#### Invariants et règles métier

- Seul l'`OWNER` de la campagne peut créer ou révoquer une `ContentAccessRule`.
- Une `ContentAccessRule` ne peut être créée que pour un document `SHARED`.
  Créer une règle sur un document `PRIVATE`, `PLAYER_PRIVATE` ou `PUBLIC` est rejeté.
- Une règle est unique par `(documentId, target)` — pas de doublons.
  Unicité sur `(documentId, targetType, targetId)` en base.
- La révocation est une suppression physique — une règle révoquée n'existe plus.

**Méthode exposée** :
```
AccessPolicy.CanAccess(documentId: DocumentId, requesterId: UserId, characterId: CharacterId?) → bool
```

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
├── slug        : Slug              — unique par (campaignId, type)
├── visibility  : Visibility
├── templateId  : TemplateId?
├── tags        : Tag[]
├── blocks      : DocumentBlock[]   — entités enfants, cycle de vie lié
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

- L'ordre des blocs est géré par l'agrégat `Document`.
- Un bloc `RELATION` valide que le `targetId` appartient à la même campagne.
- Un bloc `isPrivate = true` n'est jamais exposé aux joueurs, même si le Document est `SHARED`.
- Un bloc `isLocked = true` ne peut pas être modifié par le joueur `ownerId` du personnage associé.
  Seul le MJ peut modifier un bloc verrouillé. Non activé dans le MVP — valeur par défaut `false`.
- Soft delete `Document` → suppression physique de tous ses blocs.
- Un `Document` créé depuis un template est un snapshot indépendant — le template peut changer sans affecter le document.
- `customType` est obligatoire si `type = CUSTOM`, null sinon — invariant garanti par le constructeur.
- Le `slug` est unique par `(campaignId, type)` — deux documents de types différents peuvent avoir le même slug.

**Relations narratives** : toute relation entre entités sans règle métier propre (NPC→NPC,
NPC→lieu, NPC→faction) est exprimée via un `DocumentBlock` de type `RELATION` dans le
`Document` source. Pas d'entité dédiée pour ces liens dans le MVP.

#### Domain Events

```
DocumentCreated           { documentId, campaignId, type, createdById, occurredAt }
DocumentTitleUpdated      { documentId, oldTitle, newTitle, occurredAt }
DocumentVisibilityChanged { documentId, oldVisibility, newVisibility, occurredAt }
DocumentDeleted           { documentId, campaignId, occurredAt }
BlockAdded                { documentId, blockId, kind, occurredAt }
BlockUpdated              { documentId, blockId, occurredAt }
BlockRemoved              { documentId, blockId, occurredAt }
BlockReordered            { documentId, occurredAt }
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
- Un NPC `DEAD` reste consultable.
- `linkedCharacterId` est une association narrative — la suppression du `PlayerCharacter` ne supprime pas le NPC.
- Soft delete NPC → soft delete de son Document associé.
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
- `ownerId = null` → personnage en attente d'association joueur.
- `linkedNpcId` est une association narrative — la suppression du NPC ne supprime pas le personnage.
- Soft delete PlayerCharacter → soft delete de son Document associé.

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
- L'ordre des scénarios dans une campagne est géré par `ScenarioOrderService` (domain service).
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

#### Value Object : `BlockSchema`

```
BlockSchema
├── kind         : BlockKind
├── label        : String
├── required     : Boolean
└── defaultValue : BlockValue?    — instance concrète du sous-type correspondant
```

#### Invariants et règles métier

- `BUILTIN` non modifiable, non supprimable.
- `CAMPAIGN` visible uniquement par les membres de la campagne.
- `USER` portable entre les campagnes de son propriétaire.
- Un Document créé depuis un template est un snapshot indépendant.

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

```
LiveNote
├── id                : LiveNoteId
├── sessionId         : SessionId
├── content           : String
├── authorId          : UserId
├── visibility        : Visibility      — PRIVATE par défaut
├── linkedDocumentId  : DocumentId?
└── audit             : AuditInfo       — pas de SoftDelete
```

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
- Les `LiveNote` sont créées avec `visibility = PRIVATE` par défaut.
- Un `SessionSummary` `PRIVATE` est visible uniquement par le MJ.
- `participantIds`, `selectedNpcIds` et `pinnedItems` sont des références légères —
  si une entité référencée est supprimée, la référence est retirée.
- Soft delete `Session` → suppression physique des `LiveNote`, soft delete du `SessionSummary`.

**Permissions d'édition par statut** :

| Statut     | Métadonnées | Ajout LiveNote | Édition SessionSummary | Épinglage |
|------------|-------------|----------------|------------------------|-----------|
| `PLANNED`  | Oui         | Non            | Non                    | Oui       |
| `LIVE`     | Oui         | Oui            | Non                    | Oui       |
| `CLOSED`   | Non         | Oui (rétro)    | Oui                    | Non       |
| `ARCHIVED` | Non         | Non            | Non — lecture seule    | Non       |

> Une session `CLOSED` reste éditable pour les notes et le résumé.
> "Rouvrir" une session signifie modifier son contenu textuel — pas changer son statut.
> L'état `ARCHIVED` est le seul état véritablement immuable.

**Déduction automatique des `selectedNpcIds`** :
Quand un `scenarioId` est associé à une session, `SessionNpcSelector` (domain service)
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
LiveNoteAdded                   { sessionId, liveNoteId, authorId, occurredAt }
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
DocumentCreated, DocumentTitleUpdated, DocumentVisibilityChanged, DocumentDeleted
BlockAdded, BlockUpdated, BlockRemoved, BlockReordered
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
**Décision** : Id typés, AuditInfo, SoftDelete, Email, Slug, Tag, Visibility, PinnedItem
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
**Raison** : Un agrégat racine se justifie quand il protège des invariants sur une collection
d'entités enfants. NPC et PlayerCharacter n'ont pas d'entités enfants — leur contenu vit
dans leur Document associé. Ils ont uniquement des métadonnées légères (statut, liens narratifs)
qui ne nécessitent pas de frontière transactionnelle propre.

### AD-07 — Lien NPC ↔ PlayerCharacter optionnel et non structurant
**Décision** : Association narrative optionnelle dans les deux sens.
**Raison** : Permet la future promotion sans migration. Cycle de vie indépendant.

### AD-08 — Séparation User authentifié et accès invité
**Décision** : Les joueurs invités sans compte sont représentés par `GuestAccess`
dans Campaign Management. `UserRole` ne contient que `GM` et `PLAYER`.
**Raison** : Un `User` dans Identity & Access représente une identité persistante
avec email et authentification. Un joueur invité n'a ni l'un ni l'autre — c'est
un accès temporaire à une campagne, pas une identité système. Les deux concepts
ont des cycles de vie, des règles et des responsabilités radicalement différents.
Les unifier créerait des champs conditionnellement valides et des règles métier ambiguës.

### AD-09 — AccessPolicy comme service domaine
**Décision** : La visibilité des contenus est gérée par un service domaine `AccessPolicy`
qui persiste des entités légères `ContentAccessRule`.
**Raison** : Une règle d'accès n'a pas d'entités enfants et pas d'invariants transactionnels
complexes — un agrégat racine serait surdimensionné. `ContentAccessRule` est une entité
simple immuable : elle est créée ou révoquée (suppression physique), jamais modifiée.
Ce pattern reflète la nature binaire d'une autorisation d'accès.

### AD-10 — DocumentTemplate — combinaisons scope/ownerId/campaignId protégées
**Décision** : Le constructeur de `DocumentTemplate` rejette toute combinaison invalide.
**Raison** : Éviter les états incohérents non détectables à l'exécution.

### AD-11 — Ordre des scénarios persisté via `Scenario.order`
**Décision** : `Scenario` porte un champ `order: Int` représentant sa position dans la campagne.
Le réordonnancement est orchestré par `ScenarioOrderService` (domain service dans Content Library)
qui garantit l'unicité des positions sur une campagne donnée.
**Raison** : Sans champ de persistance, l'ordre ne peut pas être sauvegardé. Confier l'ordre
à la couche Application sans support de persistance était une décision incomplète.
`ScenarioOrderService` reçoit la liste ordonnée d'Ids depuis la couche Application et met
à jour les champs `order` de chaque `Scenario` concerné.
**Changement** : remplace l'ancienne décision "ordre géré par la couche Application sans champ dédié".

### AD-12 — PinnedItem value object pour les documents épinglés
**Décision** : Les documents épinglés à une session sont représentés par `pinnedItems: PinnedItem[]`,
un value object portant `documentId`, `order` et `pinnedAt`.
**Raison** : Un simple tableau d'Id ne capture pas l'intention complète. Le MJ épingle
un document dans un certain ordre et à un instant précis — ces informations sont
pertinentes pour l'affichage de la vue session. Le value object `PinnedItem` rend
cette sémantique explicite dans le domaine.

### AD-13 — LiveNote avec visibilité contrôlée
**Décision** : `LiveNote` porte un champ `visibility` avec `PRIVATE` comme valeur par défaut.
**Raison** : Pendant une session, le MJ peut décider de partager une note live aux joueurs
en temps réel — par exemple révéler un indice ou une information narrative. Sans visibilité
sur la note elle-même, ce cas d'usage ne peut pas être exprimé dans le domaine.
La valeur par défaut `PRIVATE` garantit qu'aucune note n'est accidentellement exposée.

### AD-14 — Références légères entre contextes — Id uniquement
**Décision** : Un contexte ne référence jamais une entité d'un autre contexte,
uniquement son Id typé.
**Raison** : Découplage strict. Chaque contexte évolue indépendamment.

### AD-15 — Synchronisation des champs dénormalisés via domain event
**Décision** : `NPC.name`, `PlayerCharacter.name`, `Scenario.title` et `Scene.title`
sont des champs dénormalisés depuis `Document.title`. Leur synchronisation est assurée
par un handler qui réagit à l'event `DocumentTitleUpdated` émis par `Document`.
**Raison** : Ces champs existent pour permettre des requêtes de liste efficaces
(afficher les NPC d'une campagne sans charger tous leurs Documents).
La dénormalisation est un choix explicite de performance — le mécanisme de synchronisation
doit être tout aussi explicite pour éviter les incohérences silencieuses.

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
| Factions | Représentées via `Document(CUSTOM, "FACTION")` si nécessaire — pas d'entité dédiée | Content Library |
| UserProjection locale | Si extraction de Campaign Management en service : ajouter projection via events | Campaign Management |
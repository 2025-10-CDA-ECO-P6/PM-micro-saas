# Domain Model Document


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

Haversack est un outil d'assistance au Maître du Jeu (MJ) pour la préparation et la conduite de campagnes de jeu de rôle.

Le domaine résout un problème central : **la dispersion des informations**. Un MJ gère simultanément des scénarios, des PNJ, des notes privées, des fiches personnages, des sessions en cours — sur des outils non spécialisés et non connectés. Haversack centralise ces informations dans un modèle structuré, modulaire et extensible.

### Utilisateurs du domaine

| Acteur | Description |
|---|---|
| **MJ (Maître du Jeu)** | Utilisateur principal. Crée et administre les campagnes, prépare et conduit les sessions. |
| **Joueur** | Utilisateur secondaire. Accède à sa fiche personnage et aux informations partagées par le MJ. |
| **Joueur invité** | Accès temporaire sans compte. Rejoint via token d'invitation. |

---

## 2. Ubiquitous Language

Vocabulaire partagé du domaine. Ces termes sont utilisés tels quels dans le code, la documentation et les conversations.

| Terme | Définition |
|---|---|
| **Campagne** | Espace organisationnel regroupant l'ensemble des ressources d'une aventure JDR : scénarios, PNJ, personnages, sessions, notes. |
| **MJ** | Maître du Jeu. Propriétaire de la campagne, seul à pouvoir modifier le contenu et gérer les accès. |
| **Joueur** | Membre d'une campagne associé à un ou plusieurs personnages joueurs. |
| **Scénario** | Structure narrative préparée par le MJ, composée de scènes ordonnées. |
| **Scène** | Unité narrative d'un scénario. Peut être liée à des PNJ. |
| **PNJ** | Personnage Non-Joueur. Entité narrative créée et gérée par le MJ. |
| **Personnage joueur** | Fiche d'un personnage appartenant à un joueur. Créée par le MJ ou le joueur. |
| **Document** | Unité de contenu modulaire. Tout contenu éditorial est un Document typé composé de blocs. |
| **Bloc** | Unité atomique de contenu dans un Document. Chaque bloc a un type et une valeur flexible. |
| **Template** | Schéma de blocs définissant la structure d'un type de Document. Non lié au Document après création. |
| **Session** | Instance d'une partie jouée. Liée optionnellement à un scénario. |
| **Note live** | Note prise par le MJ pendant une session. Liée automatiquement à la session en cours. |
| **Résumé** | Compte-rendu d'une session clôturée. Peut être partagé aux joueurs. |
| **Visibilité** | Niveau d'accès d'un contenu : PRIVATE (MJ uniquement), SHARED (membres ciblés), PUBLIC (tous les membres). |
| **ShareGrant** | Autorisation explicite donnée par le MJ pour qu'un contenu soit accessible à un ou plusieurs membres. |
| **Invitation** | Token d'accès généré par le MJ pour permettre à un joueur de rejoindre une campagne. |
| **Système de jeu** | Référentiel de règles d'un jeu de rôle (D&D 5e, Call of Cthulhu, etc.). Point d'extension futur. |
| **Slug** | Identifiant lisible généré depuis le titre. Utilisé dans les URLs. |
| **Audit** | Traçabilité des créations et modifications : qui, quand. |
| **Soft delete** | Suppression logique — l'entité est marquée supprimée mais reste en base pour l'intégrité référentielle. |

---

## 3. Bounded Contexts et Context Map

### Découpage en Bounded Contexts

| Contexte | Responsabilité | Agrégats racines |
|---|---|---|
| **Core** (Shared Kernel) | Primitives, Id typés, abstractions partagées | — |
| **Identity & Access** | Utilisateurs, authentification, rôles globaux | `User` |
| **Campaign Management** | Campagnes, membres, invitations, systèmes de jeu, partage | `Campaign`, `GameSystem`, `ShareGrant` |
| **Content Library** | Tout le contenu éditorial : documents, PNJ, personnages, scénarios | `Document`, `NPC`, `PlayerCharacter`, `Scenario`, `DocumentTemplate` |
| **Session Conduct** | Préparation, conduite et clôture des sessions | `Session` |

### Context Map

```
Core (Shared Kernel)
  └── consommé par tous les contextes
      └── fournit : Id typés, AuditInfo, SoftDelete, abstractions

Identity & Access
  └── Upstream de tous les autres contextes
      └── fournit : UserId, UserRole (via Shared Kernel)

Campaign Management
  ├── consomme Identity (UserId)
  ├── fournit CampaignId à Content Library et Session Conduct
  └── héberge ShareGrant — service transverse de visibilité

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
> Il ne peut consommer que les **Id typés** et les **primitives** du Shared Kernel.  
> La résolution d'un Id vers une entité complète est faite par la couche Application via une query cross-context.

---

## 4. Shared Kernel — Core

Contenu stable partagé par tous les contextes. Aucune règle métier. Aucune dépendance externe.

### Id typés

Chaque agrégat possède son propre type d'identifiant. Un `UserId` ne peut jamais être passé là où un `CampaignId` est attendu — erreur de compilation.

```
UserId          — wrapping UUID — Identity & Access
CampaignId      — wrapping UUID — Campaign Management
GameSystemId    — wrapping UUID — Campaign Management
MembershipId    — wrapping UUID — Campaign Management
InvitationId    — wrapping UUID — Campaign Management
ShareGrantId    — wrapping UUID — Campaign Management
DocumentId      — wrapping UUID — Content Library
BlockId         — wrapping UUID — Content Library
NpcId           — wrapping UUID — Content Library
CharacterId     — wrapping UUID — Content Library
ScenarioId      — wrapping UUID — Content Library
SceneId         — wrapping UUID — Content Library
TemplateId      — wrapping UUID — Content Library
SessionId       — wrapping UUID — Session Conduct
LiveNoteId      — wrapping UUID — Session Conduct
SummaryId       — wrapping UUID — Session Conduct
```

### Value Objects fondations

#### AuditInfo
Traçabilité des créations et modifications. Embarqué par toutes les entités et agrégats racines.

```
AuditInfo
├── createdAt   : DateTime   — date de création
├── updatedAt   : DateTime   — date de dernière modification
├── createdById : UserId     — auteur de la création
└── updatedById : UserId?    — auteur de la dernière modification (null si jamais modifié)
```

**Comportement** : `updatedAt` et `updatedById` sont mis à jour à chaque modification de l'entité. La logique est centralisée dans ce value object.

#### SoftDelete
Suppression logique. Embarqué uniquement par les entités qui supportent le soft delete.

```
SoftDelete
├── isDeleted   : Boolean    — true si supprimé logiquement
├── deletedAt   : DateTime?  — null si non supprimé
└── deletedById : UserId?    — null si non supprimé
```

**Règle** : une entité avec `isDeleted = true` est exclue de toutes les requêtes par défaut. Elle reste en base pour l'intégrité référentielle et les audits.

#### Email
```
Email
└── value : String   — validé format RFC 5322
```
**Invariant** : un Email invalide ne peut pas être instancié.

#### Slug
```
Slug
└── value : String   — lowercase, tirets, alphanumérique uniquement
```
**Comportement** : généré automatiquement depuis un titre. Unique dans son contexte (par campagne, par utilisateur selon l'entité).

#### Tag
```
Tag
├── label : String   — libellé du tag
└── color : String?  — couleur optionnelle (hex ou nom)
```

### Enumerations partagées

```
Visibility
├── PRIVATE   — visible uniquement par le MJ
├── SHARED    — visible par les membres ciblés via ShareGrant
└── PUBLIC    — visible par tous les membres de la campagne
```

### Abstractions d'infrastructure

```
IAggregateRoot<TId>   — interface marqueur pour les racines d'agrégats
IDomainEvent          — interface marqueur pour les événements domaine
IRepository<T, TId>  — interface générique de base pour les repositories
IUnitOfWork           — abstraction transactionnelle
```

---

## 5. Bounded Context — Identity & Access

### Responsabilité
Gestion des utilisateurs, authentification et rôles globaux. Isolé volontairement pour permettre l'ajout futur d'OAuth, SSO ou tout autre mécanisme d'authentification sans impacter les autres contextes.

---

### Agrégat : `User`

**Racine d'agrégat.** Entité centrale du contexte.

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
├── PLAYER   — Joueur, accède aux campagnes via invitation
└── GUEST    — Invité temporaire, accès sans compte via token

UserStatus
├── ACTIVE      — compte actif
├── SUSPENDED   — compte suspendu temporairement
└── DELETED     — soft delete, compte désactivé définitivement
```

#### Invariants et règles métier

- L'email est unique dans le système.
- Un `GUEST` n'a pas de mot de passe — il accède uniquement via token d'invitation.
- Un utilisateur `DELETED` n'est jamais supprimé physiquement — soft delete obligatoire pour l'intégrité référentielle.
- Le rôle `GM` est global — un GM peut créer et administrer plusieurs campagnes.
- Un `PLAYER` ne peut pas créer de campagne.

#### Domain Events

```
UserRegistered      { userId, email, role, occurredAt }
UserRoleChanged     { userId, oldRole, newRole, occurredAt }
UserDeactivated     { userId, occurredAt }
```

---

## 6. Bounded Context — Campaign Management

### Responsabilité
Organisation et cycle de vie des campagnes. Gestion des membres et des invitations. Référentiel des systèmes de jeu. Service de partage et de visibilité des contenus.

---

### Agrégat : `Campaign`

**Racine d'agrégat.** Contient `CampaignMembership` et `Invitation` comme entités enfants — leur cycle de vie est lié à la campagne.

```
Campaign
├── id            : CampaignId
├── ownerId       : UserId
├── name          : String
├── slug          : Slug              — unique par propriétaire
├── description   : String?
├── gameSystemId  : GameSystemId?     — optionnel, point d'extension futur
├── status        : CampaignStatus
├── memberships   : CampaignMembership[]
├── invitations   : Invitation[]
├── audit         : AuditInfo
└── softDelete    : SoftDelete
```

#### Entité enfant : `CampaignMembership`

```
CampaignMembership
├── id           : MembershipId
├── campaignId   : CampaignId
├── userId       : UserId?         — null si GUEST sans compte
├── guestToken   : String?         — token d'accès pour les GUEST
├── role         : MemberRole
├── status       : MembershipStatus
├── joinedAt     : DateTime?
└── audit        : AuditInfo
```

```
MemberRole
├── OWNER    — MJ propriétaire, droits complets
├── PLAYER   — joueur membre, droits limités
└── GUEST    — invité temporaire, lecture seule

MembershipStatus
├── PENDING   — invitation envoyée, pas encore acceptée
├── ACTIVE    — membre actif
└── REMOVED   — membre retiré de la campagne
```

#### Entité enfant : `Invitation`

```
Invitation
├── id          : InvitationId
├── campaignId  : CampaignId
├── token       : String           — unique, généré aléatoirement
├── type        : InvitationType
├── expiresAt   : DateTime?        — null = pas d'expiration
├── maxUses     : Int?             — null = illimité
├── useCount    : Int              — nombre d'utilisations actuelles
├── status      : InvitationStatus
└── audit       : AuditInfo
```

```
InvitationType
├── LINK    — lien partageable
└── EMAIL   — invitation par email

InvitationStatus
├── ACTIVE    — utilisable
├── EXPIRED   — expirée (date ou maxUses atteint)
└── REVOKED   — révoquée manuellement par le MJ
```

#### Enumerations Campaign

```
CampaignStatus
├── DRAFT     — en cours de création
├── ACTIVE    — campagne en cours
├── PAUSED    — campagne en pause
└── ARCHIVED  — campagne terminée, lecture seule
```

#### Invariants et règles métier

- Une campagne a **exactement un** membre avec le rôle `OWNER` — invariant garanti par l'agrégat.
- L'`OWNER` ne peut pas être retiré — seul un transfert de propriété est possible.
- `useCount >= maxUses` → l'invitation passe automatiquement à `EXPIRED`.
- Une invitation `EXPIRED` ou `REVOKED` ne peut plus être utilisée.
- Un `GUEST` a accès en lecture seule aux contenus explicitement partagés.
- Le slug est unique par propriétaire (`ownerId` + `slug`).
- Une campagne `ARCHIVED` est en lecture seule — aucune modification possible.

#### Domain Events

```
CampaignCreated             { campaignId, ownerId, name, occurredAt }
CampaignStatusChanged       { campaignId, oldStatus, newStatus, occurredAt }
CampaignOwnerTransferred    { campaignId, oldOwnerId, newOwnerId, occurredAt }
MemberJoined                { campaignId, userId, role, occurredAt }
MemberRemoved               { campaignId, userId, occurredAt }
InvitationCreated           { invitationId, campaignId, type, occurredAt }
InvitationRevoked           { invitationId, campaignId, occurredAt }
```

---

### Agrégat : `GameSystem`

Référentiel des systèmes de jeu. Point d'extension futur pour les règles système et les templates par jeu.

```
GameSystem
├── id          : GameSystemId
├── name        : String
├── slug        : Slug
├── description : String?
├── isBuiltIn   : Boolean      — fourni par l'app (true) ou créé par l'utilisateur (false)
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Invariants et règles métier

- Un `GameSystem` avec `isBuiltIn = true` ne peut pas être modifié ni supprimé.
- Un système custom appartient à l'utilisateur identifié par `audit.createdById`.

---

### Agrégat : `ShareGrant`

Service de visibilité transverse. Gère les autorisations d'accès aux contenus du Content Library pour les membres d'une campagne.

```
ShareGrant
├── id          : ShareGrantId
├── campaignId  : CampaignId
├── contentRef  : ContentRef       — référence polymorphe au contenu partagé
├── grantedById : UserId
├── target      : ShareTarget      — à qui s'applique le partage
└── audit       : AuditInfo        — pas de softDelete : la révocation est définitive
```

#### Value Object : `ContentRef`

Référence polymorphe vers un contenu du Content Library. N'importe quel `Document` peut être référencé.

```
ContentRef
├── documentId   : DocumentId
└── documentType : DocumentType    — hint de type pour le rendu
```

#### Value Object : `ShareTarget`

```
ShareTarget
├── type      : TargetType
└── targetId  : UserId? | CharacterId?   — null si type = ALL

TargetType
├── ALL         — tous les membres de la campagne
├── MEMBER      — un membre spécifique (par UserId)
└── CHARACTER   — associé à un personnage spécifique (par CharacterId)
```

#### Invariants et règles métier

- Seul le MJ (`OWNER` de la campagne) peut créer ou supprimer un `ShareGrant`.
- Un contenu `PRIVATE` sans `ShareGrant` n'est **jamais** visible par les joueurs.
- La révocation d'un `ShareGrant` est une suppression physique — pas de soft delete.
- Un `ShareGrant` est unique par `(contentRef, target)` — pas de doublons.

#### Domain Events

```
ContentShared     { shareGrantId, campaignId, contentRef, target, occurredAt }
ContentUnshared   { shareGrantId, campaignId, contentRef, occurredAt }
```

---

## 7. Bounded Context — Content Library

### Responsabilité
Tout le contenu éditorial de la campagne. Le modèle central est `Document` — une unité de contenu modulaire composée de `DocumentBlock`. Les entités `NPC`, `PlayerCharacter`, `Scenario` et `Scene` sont des **enveloppes relationnelles** qui portent les règles métier propres à chaque type, et délèguent leur contenu flexible à un `Document` associé.

---

### Agrégat : `Document`

**Racine d'agrégat.** Unité de contenu modulaire. Tout contenu éditorial est un `Document` typé.

```
Document
├── id          : DocumentId
├── campaignId  : CampaignId
├── type        : DocumentType
├── title       : String
├── slug        : Slug              — unique par campagne
├── visibility  : Visibility
├── templateId  : TemplateId?       — null si créé sans template
├── tags        : Tag[]
├── blocks      : DocumentBlock[]   — entités enfants, cycle de vie lié
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

```
DocumentType
├── NOTE        — note libre du MJ
├── NPC         — fiche de personnage non-joueur
├── CHARACTER   — fiche de personnage joueur
├── SCENARIO    — document narratif d'un scénario
├── SCENE       — document d'une scène
├── LOCATION    — fiche de lieu
└── CUSTOM      — type défini par un template utilisateur
```

#### Entité enfant : `DocumentBlock`

Unité atomique de contenu dans un Document. Cycle de vie entièrement lié au Document parent.

```
DocumentBlock
├── id          : BlockId
├── documentId  : DocumentId
├── kind        : BlockKind
├── label       : String?       — nom du champ affiché à l'utilisateur
├── order       : Int           — position dans le document
├── value       : BlockValue    — value object polymorphe selon kind
├── isPrivate   : Boolean       — true = visible MJ uniquement
└── audit       : AuditInfo     — pas de softDelete, suppression physique
```

```
BlockKind et structure de BlockValue associée

TEXT        { content: String }
            — texte libre enrichi (markdown ou rich text)

FIELD       { value: String }
            — champ label + valeur scalaire (ex: "Alignement: Chaotique neutre")

STAT_BAR    { current: Int, max: Int }
            — ressource avec valeur courante et maximum (ex: PV: 14/20)

RELATION    { targetId: DocumentId, targetType: DocumentType }
            — lien vers un autre Document de la même campagne

LIST        { items: String[] }
            — liste ordonnée d'éléments textuels

ITEM        { name: String, quantity: Int, properties: Map<String, Any> }
            — objet d'inventaire avec propriétés flexibles

CHECKLIST   { items: { label: String, checked: Boolean }[] }
            — liste de tâches ou de scènes avec état coché

IMAGE       { url: String, caption: String? }
            — image avec légende optionnelle
```

#### Invariants et règles métier

- L'ordre des blocs est une responsabilité de l'agrégat `Document` — cohérence garantie.
- Un bloc de type `RELATION` valide que le `targetId` existe dans la même campagne au moment de la création.
- Un bloc avec `isPrivate = true` n'est **jamais** exposé aux joueurs, même si le `Document` parent est `SHARED` ou `PUBLIC`.
- La suppression d'un `Document` (soft delete) entraîne la suppression physique de tous ses `DocumentBlock`.
- Un `Document` créé depuis un `DocumentTemplate` n'est **pas lié** au template après création — c'est un snapshot, pas un héritage.

#### Domain Events

```
DocumentCreated             { documentId, campaignId, type, createdById, occurredAt }
DocumentVisibilityChanged   { documentId, oldVisibility, newVisibility, occurredAt }
DocumentDeleted             { documentId, campaignId, occurredAt }
BlockAdded                  { documentId, blockId, kind, occurredAt }
BlockUpdated                { documentId, blockId, occurredAt }
BlockRemoved                { documentId, blockId, occurredAt }
BlockReordered              { documentId, occurredAt }
```

---

### Agrégat : `NPC`

Enveloppe relationnelle du Personnage Non-Joueur. Porte les règles métier propres au PNJ. Le contenu (description, stats, motivations, inventaire, notes) est dans le `Document` associé.

```
NPC
├── id                  : NpcId
├── campaignId          : CampaignId
├── documentId          : DocumentId       — contenu flexible associé
├── name                : String           — dénormalisé depuis Document.title
├── status              : NpcStatus
├── linkedCharacterId   : CharacterId?     — association future NPC → Personnage joueur
├── audit               : AuditInfo
└── softDelete          : SoftDelete
```

```
NpcStatus
├── ALIVE    — PNJ en vie
├── DEAD     — PNJ mort
├── MISSING  — PNJ disparu ou localisation inconnue
└── UNKNOWN  — statut non défini
```

#### Invariants et règles métier

- `name` est dénormalisé depuis `Document.title` pour les requêtes de liste. Il est synchronisé via le domain event `DocumentTitleChanged` (évolution future) ou mis à jour explicitement.
- Un NPC `DEAD` reste **consultable** — le MJ peut avoir besoin de relire la fiche d'un PNJ mort.
- `linkedCharacterId` est une **association narrative**, pas une dépendance de cycle de vie — la suppression d'un `PlayerCharacter` ne supprime pas le NPC.
- Le soft delete d'un `NPC` entraîne le soft delete de son `Document` associé.
- Les blocs `isPrivate = true` du `Document` associé ne sont **jamais** exposés aux joueurs.

#### Domain Events

```
NpcCreated              { npcId, campaignId, documentId, occurredAt }
NpcStatusChanged        { npcId, oldStatus, newStatus, occurredAt }
NpcLinkedToCharacter    { npcId, characterId, occurredAt }
NpcUnlinkedFromCharacter { npcId, occurredAt }
```

---

### Agrégat : `PlayerCharacter`

Enveloppe relationnelle du Personnage Joueur. Porte les règles d'appartenance et de permissions. Le contenu (stats, inventaire, notes) est dans le `Document` associé.

```
PlayerCharacter
├── id              : CharacterId
├── campaignId      : CampaignId
├── documentId      : DocumentId       — contenu flexible associé
├── name            : String           — dénormalisé depuis Document.title
├── ownerId         : UserId?          — null = créé par le MJ, en attente d'association
├── linkedNpcId     : NpcId?           — association narrative avec un PNJ
├── status          : CharacterStatus
├── audit           : AuditInfo
└── softDelete      : SoftDelete
```

```
CharacterStatus
├── ACTIVE   — personnage actif en campagne
├── RETIRED  — personnage retraité, toujours en vie
└── DEAD     — personnage mort
```

#### Invariants et règles métier

- Seul le `ownerId` (joueur propriétaire) **ou** le MJ (`OWNER` de la campagne) peut modifier les blocs du `Document` associé.
- Les blocs `isPrivate = true` du `Document` associé sont visibles **uniquement** par le MJ.
- Un `PlayerCharacter` sans `ownerId` est **en attente d'association** — le MJ l'a créé à l'avance.
- `linkedNpcId` est une **association narrative** — la suppression d'un NPC ne supprime pas le `PlayerCharacter`.
- Le soft delete d'un `PlayerCharacter` entraîne le soft delete de son `Document` associé.

#### Domain Events

```
CharacterCreated            { characterId, campaignId, documentId, occurredAt }
CharacterOwnerAssigned      { characterId, ownerId, occurredAt }
CharacterStatusChanged      { characterId, oldStatus, newStatus, occurredAt }
CharacterLinkedToNpc        { characterId, npcId, occurredAt }
CharacterUnlinkedFromNpc    { characterId, occurredAt }
```

---

### Agrégat : `Scenario`

Enveloppe relationnelle du Scénario. Gère la structure ordonnée des scènes et le statut de progression. Le contenu narratif global est dans le `Document` associé.

```
Scenario
├── id          : ScenarioId
├── campaignId  : CampaignId
├── documentId  : DocumentId       — contenu narratif global
├── title       : String           — dénormalisé depuis Document.title
├── slug        : Slug             — unique par campagne
├── status      : ScenarioStatus
├── order       : Int?             — position dans la campagne (optionnel)
├── scenes      : Scene[]          — entités enfants, cycle de vie lié
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

```
ScenarioStatus
├── DRAFT     — en cours de préparation
├── READY     — prêt à jouer
├── PLAYED    — déjà joué
└── ARCHIVED  — archivé
```

#### Entité enfant : `Scene`

```
Scene
├── id              : SceneId
├── scenarioId      : ScenarioId
├── documentId      : DocumentId       — contenu de la scène
├── title           : String           — dénormalisé depuis Document.title
├── order           : Int              — position dans le scénario
├── status          : SceneStatus
├── linkedNpcIds    : NpcId[]          — références légères, pas de cycle de vie lié
└── audit           : AuditInfo        — pas de softDelete, suppression en cascade
```

```
SceneStatus
├── PENDING  — pas encore jouée
├── PLAYED   — scène jouée
└── SKIPPED  — scène passée volontairement
```

#### Invariants et règles métier

- Une `Scene` ne peut pas exister sans son `Scenario` parent — suppression en cascade.
- L'ordre des `Scene` est géré par l'agrégat `Scenario` — c'est lui qui garantit la cohérence de la numérotation.
- Un `Scenario` `ARCHIVED` est en lecture seule.
- Le soft delete d'un `Scenario` entraîne la suppression physique de ses `Scene` et le soft delete de leurs `Document` associés.
- `linkedNpcIds` sont des **références légères** — si un NPC est soft-deleted, sa référence est retirée de la liste sans supprimer la scène.

#### Domain Events

```
ScenarioCreated         { scenarioId, campaignId, documentId, occurredAt }
ScenarioStatusChanged   { scenarioId, oldStatus, newStatus, occurredAt }
SceneAdded              { scenarioId, sceneId, documentId, occurredAt }
SceneRemoved            { scenarioId, sceneId, occurredAt }
SceneStatusChanged      { sceneId, scenarioId, oldStatus, newStatus, occurredAt }
SceneReordered          { scenarioId, occurredAt }
SceneNpcLinked          { sceneId, npcId, occurredAt }
SceneNpcUnlinked        { sceneId, npcId, occurredAt }
```

---

### Agrégat : `DocumentTemplate`

Schéma de blocs définissant la structure attendue pour un type de `Document`. Permet la création de templates génériques ou liés à un système de jeu.

```
DocumentTemplate
├── id              : TemplateId
├── name            : String
├── documentType    : DocumentType
├── gameSystemId    : GameSystemId?    — null = générique tous systèmes
├── scope           : TemplateScope
├── ownerId         : UserId?          — null si BUILTIN
├── campaignId      : CampaignId?      — null si scope USER ou BUILTIN
├── schema          : BlockSchema[]    — définition des blocs attendus
├── audit           : AuditInfo
└── softDelete      : SoftDelete
```

```
TemplateScope
├── BUILTIN    — fourni par l'application, non modifiable
├── CAMPAIGN   — défini pour une campagne spécifique
└── USER       — défini par un utilisateur, portable entre campagnes
```

#### Value Object : `BlockSchema`

```
BlockSchema
├── kind          : BlockKind
├── label         : String
├── required      : Boolean
└── defaultValue  : BlockValue?    — valeur pré-remplie optionnelle
```

#### Invariants et règles métier

- Un template `BUILTIN` ne peut pas être modifié ni supprimé.
- Un template `CAMPAIGN` est visible uniquement par les membres de la campagne concernée.
- Un template `USER` appartient à l'utilisateur identifié par `ownerId` et est portable entre ses campagnes.
- Un `Document` créé depuis un template est un **snapshot indépendant** — modifier le template ne modifie pas les documents déjà créés.

---

## 8. Bounded Context — Session Conduct

### Responsabilité
Préparation, conduite en temps réel et clôture des sessions de jeu. Consomme des références vers les contenus du Content Library sans en posséder les entités.

---

### Agrégat : `Session`

**Racine d'agrégat.** Contient `LiveNote` et `SessionSummary` comme entités enfants.

```
Session
├── id                  : SessionId
├── campaignId          : CampaignId
├── scenarioId          : ScenarioId?          — null = session libre sans scénario
├── title               : String
├── slug                : Slug
├── scheduledAt         : DateTime?
├── startedAt           : DateTime?
├── endedAt             : DateTime?
├── status              : SessionStatus
├── participantIds      : CharacterId[]         — références, pas de cycle de vie lié
├── pinnedDocumentIds   : DocumentId[]          — références, pas de cycle de vie lié
├── liveNotes           : LiveNote[]            — entités enfants
├── summary             : SessionSummary?       — entité enfant unique
├── audit               : AuditInfo
└── softDelete          : SoftDelete
```

```
SessionStatus
├── PLANNED   — session programmée, pas encore démarrée
├── LIVE      — session en cours
├── CLOSED    — session clôturée
└── ARCHIVED  — session archivée
```

#### Entité enfant : `LiveNote`

Note prise pendant la session. Liée automatiquement à la session en cours à sa création.

```
LiveNote
├── id                  : LiveNoteId
├── sessionId           : SessionId
├── content             : String
├── authorId            : UserId
├── linkedDocumentId    : DocumentId?   — lien optionnel vers un élément du Content Library
└── audit               : AuditInfo     — pas de softDelete, suppression physique
```

#### Entité enfant : `SessionSummary`

Compte-rendu de la session. Unique par session. Créé à la clôture.

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

- **Une seule session `LIVE` par campagne** à un instant donné — invariant fort garanti par l'agrégat.
- Transitions de statut autorisées : `PLANNED → LIVE → CLOSED → ARCHIVED`.
- Aucun retour en arrière sur les transitions (une session `CLOSED` ne peut pas repasser à `LIVE`).
- Les `LiveNote` sont automatiquement liées à la session en cours à leur création.
- Un `SessionSummary` avec `visibility = PRIVATE` est visible **uniquement** par le MJ.
- `participantIds` et `pinnedDocumentIds` sont des **références légères** — si un `PlayerCharacter` ou un `Document` est supprimé, la référence est retirée sans supprimer la session.
- Le soft delete d'une `Session` entraîne la suppression physique de ses `LiveNote` et le soft delete de son `SessionSummary`.

#### Domain Events

```
SessionPlanned                  { sessionId, campaignId, scheduledAt, occurredAt }
SessionStarted                  { sessionId, campaignId, startedAt, occurredAt }
SessionClosed                   { sessionId, campaignId, endedAt, occurredAt }
SessionArchived                 { sessionId, campaignId, occurredAt }
LiveNoteAdded                   { sessionId, liveNoteId, authorId, occurredAt }
LiveNoteRemoved                 { sessionId, liveNoteId, occurredAt }
DocumentPinnedToSession         { sessionId, documentId, occurredAt }
DocumentUnpinnedFromSession     { sessionId, documentId, occurredAt }
SessionSummaryCreated           { sessionId, summaryId, visibility, occurredAt }
SessionSummaryVisibilityChanged { sessionId, summaryId, oldVisibility, newVisibility, occurredAt }
```

---

## 9. Domain Events — vue globale

Récapitulatif de tous les domain events par contexte émetteur.

### Identity & Access
```
UserRegistered, UserRoleChanged, UserDeactivated
```

### Campaign Management
```
CampaignCreated, CampaignStatusChanged, CampaignOwnerTransferred
MemberJoined, MemberRemoved
InvitationCreated, InvitationRevoked
ContentShared, ContentUnshared
```

### Content Library
```
DocumentCreated, DocumentVisibilityChanged, DocumentDeleted
BlockAdded, BlockUpdated, BlockRemoved, BlockReordered
NpcCreated, NpcStatusChanged, NpcLinkedToCharacter, NpcUnlinkedFromCharacter
CharacterCreated, CharacterOwnerAssigned, CharacterStatusChanged
CharacterLinkedToNpc, CharacterUnlinkedFromNpc
ScenarioCreated, ScenarioStatusChanged
SceneAdded, SceneRemoved, SceneStatusChanged, SceneReordered
SceneNpcLinked, SceneNpcUnlinked
```

### Session Conduct
```
SessionPlanned, SessionStarted, SessionClosed, SessionArchived
LiveNoteAdded, LiveNoteRemoved
DocumentPinnedToSession, DocumentUnpinnedFromSession
SessionSummaryCreated, SessionSummaryVisibilityChanged
```

---

## 10. Décisions d'architecture domaine

### AD-01 — Séparation entités domaine / modèles infrastructure
**Décision** : Les entités domaine ne dépendent d'aucun framework (EF Core, Identity, etc.). Un mapper dédié assure la conversion entre entité domaine et modèle de persistance dans la couche Infrastructure.  
**Raison** : Garder le domaine agnostique de toute implémentation technique. Facilite les tests unitaires et l'évolution de la stack.

### AD-02 — Shared Kernel pour les primitives partagées
**Décision** : Les Id typés, `AuditInfo`, `SoftDelete`, `Email`, `Slug`, `Tag`, `Visibility` et les abstractions d'infrastructure vivent dans un module `Core` consommé par tous les contextes.  
**Raison** : Éviter la duplication sans créer de couplage entre contextes. Le Shared Kernel est stable et sans règle métier.

### AD-03 — Id typés — jamais de UUID nu dans le domaine
**Décision** : Chaque agrégat possède son propre type d'identifiant wrappant un UUID.  
**Raison** : Sécurité de type à la compilation. Impossible de passer un `CampaignId` là où un `DocumentId` est attendu.

### AD-04 — SoftDelete séparé de AuditInfo
**Décision** : `SoftDelete` est un value object distinct de `AuditInfo`.  
**Raison** : Séparation des préoccupations. L'audit trace les modifications, le soft delete trace la suppression. Certaines entités ont l'un sans l'autre.

### AD-05 — Modèle Document modulaire avec DocumentBlock
**Décision** : Tout contenu éditorial variable (stats, inventaire, descriptions, notes) est stocké dans des `DocumentBlock` typés avec une `BlockValue` polymorphe.  
**Raison** : Extensibilité maximale — ajouter un nouveau type de bloc ou supporter un nouveau système de jeu ne nécessite pas de migration de schéma.

### AD-06 — NPC et PlayerCharacter sont deux agrégats distincts
**Décision** : `NPC` et `PlayerCharacter` sont des agrégats séparés avec leurs propres règles métier.  
**Raison** : Leurs règles divergent trop — permissions, visibilité des blocs privés, ownership. Une entité unifiée créerait des champs nullables et des règles conditionnelles.

### AD-07 — Lien NPC ↔ PlayerCharacter optionnel
**Décision** : `NPC.linkedCharacterId` et `PlayerCharacter.linkedNpcId` sont des associations narratives optionnelles, pas des dépendances de cycle de vie.  
**Raison** : Permet la future promotion NPC → Personnage joueur sans migration. La suppression de l'un ne supprime pas l'autre.

### AD-08 — Références légères entre contextes
**Décision** : Un contexte ne référence jamais une entité d'un autre contexte — uniquement son Id typé.  
**Raison** : Découplage strict entre contextes. Chaque contexte peut évoluer indépendamment.

### AD-09 — DocumentTemplate snapshot, pas héritage
**Décision** : Un Document créé depuis un template est une copie indépendante — modifier le template ne modifie pas les documents existants.  
**Raison** : Prévisibilité. Le MJ ne doit pas voir ses fiches se modifier suite à un changement de template.

### AD-10 — Une seule session LIVE par campagne
**Décision** : L'invariant "une seule session LIVE par campagne" est garanti par l'agrégat Session via un domain service.  
**Raison** : Cohérence métier — deux sessions simultanées dans une même campagne n'ont pas de sens.

---

## 11. Hors périmètre MVP — points d'extension documentés

| Fonctionnalité | Point d'extension prévu | Contexte |
|---|---|---|
| Règles système de jeu | `GameSystem` — agrégat léger prêt à recevoir un `RuleSet` | Campaign Management |
| Templates par système | `DocumentTemplate.gameSystemId` — lien vers `GameSystem` déjà modélisé | Content Library |
| Promotion NPC → PJ | `NPC.linkedCharacterId` / `PlayerCharacter.linkedNpcId` — liens optionnels déjà présents | Content Library |
| Versioning des documents | `DocumentBlock` sans soft delete — architecture compatible avec un historique de blocs | Content Library |
| OAuth / SSO | Contexte Identity isolé — remplacement du mécanisme d'auth sans impact sur les autres contextes | Identity & Access |
| Partage communautaire de templates | `DocumentTemplate.scope = USER` — déjà modélisé, expose via API future | Content Library |
| ShareGrant en contexte autonome | `ShareGrant` extractible de Campaign Management en contexte dédié si la complexité l'exige | Campaign Management |
| Temps réel (WebSocket) | `SessionStatus.LIVE` + domain events — base pour un bus d'événements temps réel | Session Conduct |
# Shared Kernel — Core

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
AccessRuleId    — Campaign Management
ShareableResourceId — Campaign Management (ContentAccessRule, wrapper logique)
DocumentId      — Content Library
BlockId         — Content Library
DocumentTypeId  — Content Library
CharacterId     — Content Library
ScenarioId      — Content Library
SceneId         — Content Library
TemplateId      — Content Library
FolderId        — Content Library
TagId           — Content Library
SessionId       — Session Conduct
LiveNoteId      — Session Conduct
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
│   ├── userId      : UserId
│   └── characterId : CharacterId?    — personnage courant dans cette campagne, peut être null
│
└── GuestRequesterId
    ├── guestAccessId : GuestAccessId
    └── characterId   : CharacterId?    — personnage associé par le MJ, peut être null
```

**Usage** : `AccessPolicy.CanAccess(resource: ShareableResourceRef, requester: RequesterId) → bool`.
Voir la section Campaign Management pour les règles de résolution.

Un utilisateur authentifié peut posséder plusieurs personnages dans une même campagne.
`AuthenticatedRequesterId.characterId` représente donc le personnage **actif pour la requête**
et non une propriété globale du compte. La couche Application construit le `RequesterId`
depuis le contexte d'accès : campagne, session, personnage sélectionné ou personnage associé
au contenu consulté.

### ShareableResourceRef — référence de ressource partageable

`AccessPolicy` ne cible pas uniquement les documents. Une même règle d'accès peut
s'appliquer à tout contenu exposable aux joueurs dans le MVP : document ou note live.

Les récapitulatifs post-session sont modélisés comme des Documents de campagne si le MJ
en crée un. Il n'existe pas d'entité dédiée de résumé dans le domaine MVP.

```
ShareableResourceRef
├── resourceType : ShareableResourceType
└── resourceId   : UUID typé selon resourceType

ShareableResourceType
├── DOCUMENT
└── LIVE_NOTE
```

La couche Application résout cette référence vers le contexte propriétaire de la ressource
pour lire sa campagne, sa visibilité et son éventuel `ownerCharacterId`.

### Enumerations partagées

```
Visibility
├── PRIVATE        — visible uniquement par le MJ propriétaire de la campagne
├── PLAYER_PRIVATE — visible uniquement via le Personnage joueur propriétaire — MJ exclu
├── SHARED         — visible par les membres ciblés via AccessPolicy
└── PUBLIC         — visible par tous les membres de la campagne (joueurs authentifiés ET invités actifs)
```

**Règle de résolution d'accès** (appliquée par `AccessPolicy.CanAccess`) :

1. `PRIVATE` → accès accordé au MJ (`AuthenticatedRequesterId` avec `userId = campaign.ownerId`) uniquement.
2. `PLAYER_PRIVATE` → accès accordé uniquement si `requester.characterId = resource.ownerCharacterId`.
   Fonctionne pour un joueur authentifié et pour un GuestAccess actif. Le MJ n'a **pas** accès non plus.
   Les fiches `PlayerCharacter` sont une exception métier explicite : elles restent consultables par le MJ
   même si le joueur associé y accède via son `CharacterId`.
3. `PUBLIC` → accès accordé à tout `RequesterId` valide dans le scope de la campagne ou de la session
   (membre authentifié ou invité actif).
4. `SHARED` → accès accordé si `AccessPolicy` contient une `ContentAccessRule` correspondant au demandeur :
   - `AllMembersTarget` → accordé à tout `AuthenticatedRequesterId` membre de la campagne,
     ET à tout `GuestRequesterId` actif dont le scope couvre toute la campagne.
   - `SpecificMemberTarget(userId)` → accordé uniquement si `requester` est `AuthenticatedRequesterId` avec `userId` correspondant.
   - `SpecificCharacterTarget(characterId)` → accordé si `requester.characterId = characterId` (fonctionne pour les deux types de RequesterId).
   - `SessionParticipantsTarget(sessionId)` → accordé aux participants de la session ciblée, y compris les invités actifs créés depuis un lien de session.

**Invariant** : une `ContentAccessRule` ne peut être créée que pour une ressource `SHARED`.

### Abstractions d'infrastructure

```
IAggregateRoot<TId>    — interface marqueur pour les racines d'agrégats
IEntity<TId>           — interface marqueur pour les entités avec repository
IDomainEvent           — interface marqueur pour les événements domaine
IRepository<T, TId>   — interface générique de base
IUnitOfWork            — abstraction transactionnelle
```

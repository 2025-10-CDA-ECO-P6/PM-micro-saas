# Bounded Context — Campaign Management

### Responsabilité

Organisation et cycle de vie des campagnes. Gestion des membres, invitations,
accès invités et systèmes de jeu. Héberge `AccessPolicy` — le service domaine
de visibilité des contenus.

---

### Agrégat : `Campaign`

```
Campaign
├── id                   : CampaignId
├── ownerId              : UserId
├── name                 : String
├── slug                 : Slug               — unique par propriétaire
├── description          : String?
├── gameSystemId         : GameSystemId?      — optionnel, point d'extension futur
├── type                 : CampaignType
├── status               : CampaignStatus
├── autoArchiveEnabled   : Boolean            — true par défaut ; si false, désactive l'archivage automatique des one-shots
├── memberships          : CampaignMembership[]    — entités enfants
├── invitations          : Invitation[]            — entités enfants
├── audit                : AuditInfo
└── softDelete           : SoftDelete
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
├── scope       : InvitationScope
├── sessionId   : SessionId?           — renseigné uniquement si scope = SESSION
├── token       : String               — identifiant unique généré
├── type        : InvitationType       — mode de distribution
├── expiresAt   : DateTime?            — null = pas d'expiration
├── maxUses     : Int?                 — null = illimité
├── useCount    : Int
├── status      : InvitationStatus
└── audit       : AuditInfo
```

#### Enumerations Campaign

```
CampaignType
├── CAMPAIGN   — campagne longue, plusieurs sessions, groupe stable
└── ONE_SHOT   — partie unique, archivage automatique post-session

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

InvitationScope
├── CAMPAIGN — accès durable à la campagne, pour les membres permanents ou futurs membres
└── SESSION  — accès temporaire à une session précise, pour les joueurs invités sans compte

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
- `scope = CAMPAIGN` → `sessionId = null`.
- `scope = SESSION` → `sessionId` obligatoire. L'invitation ne donne accès qu'au périmètre
  de la session ciblée et expire au plus tard après la fenêtre de grâce post-session.
- Une campagne `ARCHIVED` est en lecture seule.
- Le slug est unique par propriétaire (`ownerId + slug`).
- Une campagne `ONE_SHOT` passe automatiquement à `ARCHIVED` 48h après la fin de sa dernière session, si `autoArchiveEnabled = true` et si le MJ n'a pas ouvert la campagne pendant ce délai. Cette règle est évaluée par un scheduler applicatif qui appelle `Campaign.AutoArchive()` — le domaine expose la méthode et l'invariant, le déclenchement est infrastucturel.
- Le MJ peut rouvrir une campagne `ARCHIVED` (statut → `ACTIVE`) et désactiver l'archivage automatique à tout moment (`autoArchiveEnabled = false`).
- `autoArchiveEnabled` n'a d'effet que pour `type = ONE_SHOT` — ignoré pour `type = CAMPAIGN`.

#### Domain Events

```
CampaignCreated           { campaignId, ownerId, type, name, occurredAt }
CampaignStatusChanged     { campaignId, oldStatus, newStatus, occurredAt }
CampaignOwnerTransferred  { campaignId, oldOwnerId, newOwnerId, occurredAt }
MemberJoined              { campaignId, userId, role, occurredAt }
MemberRemoved             { campaignId, userId, occurredAt }
InvitationCreated         { invitationId, campaignId, scope, type, occurredAt }
InvitationRevoked         { invitationId, campaignId, occurredAt }
```

---

### Entité : `GuestAccess`

Représente un accès temporaire à une campagne sans compte utilisateur persistant.
Ce n'est **pas** un `User` — c'est un droit d'accès sécurisé, limité dans le temps,
vers une campagne et, idéalement, vers un `PlayerCharacter`.

Du point de vue fonctionnel, un invité peut faire ce qu'un joueur avec compte peut faire
sur le périmètre de son personnage : consulter sa fiche, prendre des notes personnelles,
voir les contenus PUBLIC ou SHARED qui le ciblent. La persistance des données joueur
est portée par le `CharacterId`, pas par le `GuestAccessId`.

```
GuestAccess
├── id           : GuestAccessId
├── campaignId   : CampaignId
├── invitationId : InvitationId
├── scope        : InvitationScope
├── sessionId    : SessionId?          — renseigné si accès invité limité à une session
├── displayName  : String              — pseudo choisi par l'invité
├── accessToken  : String              — token d'accès, validé en infrastructure
├── characterId  : CharacterId?        — personnage associé par le MJ ; recommandé pour l'accès joueur
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

- Un `GuestAccess` est créé depuis une `Invitation` active et conserve son `invitationId`
  pour tracer le chemin d'accès utilisé.
- Un `GuestAccess(scope = SESSION)` est limité à la session ciblée. Il peut consulter et créer
  les mêmes ressources qu'un joueur authentifié dans ce périmètre, mais ne devient pas membre
  permanent de la campagne.
- Un `GuestAccess(scope = CAMPAIGN)` est un accès invité durable jusqu'à expiration ou révocation,
  sans compte utilisateur.
- La validation du token est une responsabilité de la couche Infrastructure — le domaine ne connaît que le statut.
- Un `GuestAccess` expiré ne donne aucun accès.
- Un `GuestAccess` n'a pas de `SoftDelete` — suppression physique à expiration.
- Pour `AccessPolicy`, un `GuestAccess` actif est représenté par un `GuestRequesterId(guestAccessId, characterId?)`.
- Un `GuestAccess` sans `characterId` peut accéder aux ressources `PUBLIC`, `SHARED` via
  `AllMembersTarget`, et `SHARED` via `SessionParticipantsTarget` si le scope de session correspond.
  Il ne peut pas accéder aux ressources `PLAYER_PRIVATE`.
- Les notes personnelles créées par un invité sont liées au `CharacterId`; elles restent récupérables lors d'une session suivante si le MJ recrée ou prolonge un accès invité vers le même personnage.

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

Gère les autorisations d'accès aux ressources partageables d'une campagne
(`Document`, `LiveNote`). Ce n'est pas un agrégat — c'est un service
domaine avec persistance légère via des entités `ContentAccessRule`.

```
ContentAccessRule               — entité persistée par AccessPolicy
├── id            : AccessRuleId
├── campaignId    : CampaignId
├── resource      : ShareableResourceRef  — ref cross-context par Id uniquement
├── grantedBy     : RequesterId           — MJ, joueur authentifié ou invité auteur de la règle
├── target        : AccessTarget
└── createdAt     : DateTime    — règle immuable, pas de AuditInfo complet
```

```
AccessTarget  (hiérarchie de value objects scellés)
├── AllMembersTarget
│   — cible tous les membres authentifiés ET les GuestAccess actifs de scope CAMPAIGN
│
├── SpecificMemberTarget
│   └── userId : UserId        — cible un membre authentifié précis
│
├── SpecificCharacterTarget
│   └── characterId : CharacterId  — cible le joueur (ou invité) associé à ce personnage
│
└── SessionParticipantsTarget
    └── sessionId : SessionId      — cible les participants de la session courante
```

> **Note de typage** : `AccessTarget` est une hiérarchie de types scellés,
> pas un objet avec un discriminant enum et un champ `targetId` optionnel.
> Même principe que `BlockValue` et `RequesterId`.

#### Méthode exposée

```
AccessPolicy.CanAccess(resource: ShareableResourceRef, requester: RequesterId) → bool
```

**Algorithme de résolution** :

```
1. Charger la ressource ciblée (Document ou LiveNote)
   pour connaître campaignId, visibility et ownerCharacterId éventuel.
2. Si PRIVATE    → vrai ssi requester est AuthenticatedRequesterId
                   ET userId == campaign.ownerId
3. Si PLAYER_PRIVATE → vrai ssi requester.characterId == resource.ownerCharacterId
4. Si PUBLIC     → vrai pour tout RequesterId valide dont le scope couvre la ressource
                   (membre de campagne ou GuestAccess actif dans le périmètre autorisé)
5. Si SHARED     → charger les ContentAccessRule pour cette resource, puis :
   - AllMembersTarget       : vrai si requester est authentifié membre actif
                              OU GuestRequesterId actif avec scope = CAMPAIGN
   - SpecificMemberTarget   : vrai si requester est AuthenticatedRequesterId
                              ET userId correspond
   - SpecificCharacterTarget: vrai si requester.characterId correspond
                              (AuthenticatedRequesterId ou GuestRequesterId)
   - SessionParticipantsTarget: vrai si le requester est participant de la session ciblée
                                OU GuestRequesterId actif avec scope = SESSION et sessionId correspondant
```

#### Invariants et règles métier

- L'`OWNER` de la campagne peut créer ou révoquer une `ContentAccessRule` sur les documents
  et notes MJ de sa campagne.
- L'auteur joueur d'une `LiveNote(authorRole = PLAYER)` peut créer ou révoquer les règles
  de partage de sa propre note personnelle, sans obtenir de droits sur les autres ressources
  de campagne.
- Une `ContentAccessRule` ne peut être créée que pour une ressource `SHARED`.
- Une règle est unique par `(resource, target)` — index unique sur `(resourceType, resourceId, targetType, targetId)` en base, avec `NULLS NOT DISTINCT` pour `targetId = NULL` (cas `AllMembersTarget`).
- La révocation est une suppression physique — une règle révoquée n'existe plus.

#### Domain Events

```
ContentAccessGranted  { ruleId, campaignId, resource, target, occurredAt }
ContentAccessRevoked  { ruleId, campaignId, resource, occurredAt }
```

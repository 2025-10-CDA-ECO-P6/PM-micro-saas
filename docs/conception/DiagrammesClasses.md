# Diagrammes de classes

## Légende des relations

| Notation | Signification |
|---|---|
| `*--` | Composition — cycle de vie lié, l'enfant ne vit pas sans le parent |
| `-->` | Association — dépendance directionnelle |
| `..>` | Référence légère — Id uniquement, cycle de vie indépendant |
| `<|--` | Héritage / spécialisation |

---

## 1. Identity & Access

Contexte le plus simple et le plus stable. Un seul agrégat racine : `User`.
Il représente uniquement les utilisateurs authentifiés avec une identité persistante.
Les joueurs invités sans compte ne sont pas des `User` — ils vivent dans Campaign Management
sous la forme d'un `GuestAccess`. `UserRole` est intentionnellement limité à deux valeurs :
un rôle global ne dit pas ce qu'un utilisateur peut faire dans une campagne précise,
cela relève de `MemberRole` dans Campaign Management.

```mermaid
classDiagram
    direction TB
    class User {
        <<aggregate root>>
        +id: UserId
        +email: Email
        +displayName: String
        +role: UserRole
        +status: UserStatus
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class Email {
        <<value object>>
        +value: String
    }
    class UserRole {
        <<enumeration>>
        GM
        PLAYER
    }
    class UserStatus {
        <<enumeration>>
        ACTIVE
        SUSPENDED
        DELETED
    }
    User *-- Email
    User --> UserRole
    User --> UserStatus
    note for UserRole "Deux valeurs uniquement\nGM = peut créer des campagnes\nPLAYER = accède via invitation\nLes invités sans compte sont\nreprésentés par GuestAccess\ndans Campaign Management"
```

---

## 2. Campaign Management

Contexte organisationnel. `Campaign` est l'agrégat racine central — il encapsule
ses membres (`CampaignMembership`) et ses invitations (`Invitation`) dont le cycle
de vie lui est entièrement lié.

`GuestAccess` est une entité avec repository distincte de `CampaignMembership` :
un invité sans compte n'est pas un membre au sens plein — c'est un accès temporaire
créé depuis une invitation, sans identité persistante dans le système.

`AccessPolicy` est un service domaine, pas un agrégat. Il persiste des `ContentAccessRule`
immuables qui définissent qui peut voir quel contenu. Une règle est créée ou révoquée
(suppression physique) — jamais modifiée. `ContentAccessRule.documentId` est une référence
cross-context vers Content Library par Id uniquement, conformément à la règle AD-14.

`GameSystem` est un agrégat léger indépendant — point d'extension futur pour les règles
de jeu. Référencé optionnellement par une campagne.

```mermaid
classDiagram
    direction TB
    class Campaign {
        <<aggregate root>>
        +id: CampaignId
        +ownerId: UserId
        +name: String
        +slug: Slug
        +description: String?
        +gameSystemId: GameSystemId?
        +status: CampaignStatus
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class CampaignMembership {
        <<entity>>
        +id: MembershipId
        +campaignId: CampaignId
        +userId: UserId
        +role: MemberRole
        +status: MembershipStatus
        +joinedAt: DateTime?
        +audit: AuditInfo
    }
    class Invitation {
        <<entity>>
        +id: InvitationId
        +campaignId: CampaignId
        +token: String
        +type: InvitationType
        +expiresAt: DateTime?
        +maxUses: Int?
        +useCount: Int
        +status: InvitationStatus
        +audit: AuditInfo
    }
    class GuestAccess {
        <<entity — repository>>
        +id: GuestAccessId
        +campaignId: CampaignId
        +displayName: String
        +accessToken: String
        +characterId: CharacterId?
        +expiresAt: DateTime?
        +status: GuestAccessStatus
        +createdAt: DateTime
    }
    class GameSystem {
        <<aggregate root>>
        +id: GameSystemId
        +name: String
        +slug: Slug
        +description: String?
        +isBuiltIn: Boolean
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class ContentAccessRule {
        <<entity — AccessPolicy>>
        +id: AccessRuleId
        +campaignId: CampaignId
        +documentId: DocumentId
        +grantedById: UserId
        +target: AccessTarget
        +createdAt: DateTime
    }
    class AccessTarget {
        <<value object sealed>>
    }
    class AllMembersTarget {
        <<value object>>
    }
    class SpecificMemberTarget {
        <<value object>>
        +userId: UserId
    }
    class SpecificCharacterTarget {
        <<value object>>
        +characterId: CharacterId
    }
    class CampaignStatus {
        <<enumeration>>
        DRAFT
        ACTIVE
        PAUSED
        ARCHIVED
    }
    class MemberRole {
        <<enumeration>>
        OWNER
        PLAYER
    }
    class MembershipStatus {
        <<enumeration>>
        PENDING
        ACTIVE
        REMOVED
    }
    class InvitationType {
        <<enumeration>>
        LINK
        EMAIL
    }
    class InvitationStatus {
        <<enumeration>>
        ACTIVE
        EXPIRED
        REVOKED
    }
    class GuestAccessStatus {
        <<enumeration>>
        ACTIVE
        EXPIRED
    }
    Campaign "1" *-- "0..*" CampaignMembership
    Campaign "1" *-- "0..*" Invitation
    Campaign --> CampaignStatus
    Campaign ..> GameSystem : gameSystemId
    CampaignMembership --> MemberRole
    CampaignMembership --> MembershipStatus
    Invitation --> InvitationType
    Invitation --> InvitationStatus
    Invitation ..> GuestAccess : crée un GuestAccess à l'usage
    GuestAccess --> GuestAccessStatus
    GuestAccess ..> Campaign : campaignId
    ContentAccessRule *-- AccessTarget
    ContentAccessRule --> Campaign
    AccessTarget <|-- AllMembersTarget
    AccessTarget <|-- SpecificMemberTarget
    AccessTarget <|-- SpecificCharacterTarget
    note for CampaignMembership "userId toujours renseigné\nLes invités sans compte\nutilisent GuestAccess"
    note for ContentAccessRule "Géré par AccessPolicy\nservice domaine\nRévocation = suppression physique"
    note for ContentAccessRule "documentId = ref cross-context\nvers Content Library par Id"
    note for AccessTarget "Hiérarchie scellée — le sous-type EST le discriminant\nPas d'AccessTargetType enum\nIndex unique : (documentId, sous-type, targetId?)"
```

---

## 3. Content Library — Document et blocs

`Document` est l'agrégat racine du contenu modulaire. Tout contenu éditorial dans
Haversack est un `Document` typé composé de `DocumentBlock`.

Chaque `DocumentBlock` porte une `BlockValue` fortement typée — une hiérarchie
de value objects scellés, un sous-type concret par `BlockKind`. Ce design évite
les champs optionnels sans sens : un `StatBarBlockValue` n'a que `current` et `max`,
un `TextBlockValue` n'a que `content`. Aucune ambiguïté sur ce qui est valide.

Le champ `customType: String?` sur `Document` permet d'étendre les types de document
sans modifier l'énumération — `DocumentType.CUSTOM` avec un libellé libre couvre
les besoins non prévus et les futurs types système de jeu.

```mermaid
classDiagram
    direction TB
    class Document {
        <<aggregate root>>
        +id: DocumentId
        +campaignId: CampaignId
        +type: DocumentType
        +customType: String?
        +title: String
        +slug: Slug
        +visibility: Visibility
        +templateId: TemplateId?
        +tags: Tag[]
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class DocumentBlock {
        <<entity>>
        +id: BlockId
        +documentId: DocumentId
        +kind: BlockKind
        +label: String?
        +order: Int
        +value: BlockValue
        +isPrivate: Boolean
        +isLocked: Boolean
        +audit: AuditInfo
    }
    class BlockValue {
        <<value object abstract>>
    }
    class TextBlockValue {
        <<value object>>
        +content: String
    }
    class FieldBlockValue {
        <<value object>>
        +value: String
    }
    class StatBarBlockValue {
        <<value object>>
        +current: Int
        +max: Int
    }
    class RelationBlockValue {
        <<value object>>
        +targetId: DocumentId
        +targetType: DocumentType
    }
    class ListBlockValue {
        <<value object>>
        +items: String[]
    }
    class ItemBlockValue {
        <<value object>>
        +name: String
        +quantity: Int
        +properties: Map
    }
    class ChecklistBlockValue {
        <<value object>>
        +items: ChecklistItem[]
    }
    class ImageBlockValue {
        <<value object>>
        +url: String
        +caption: String?
    }
    class Visibility {
        <<enumeration — Shared Kernel>>
        PRIVATE
        PLAYER_PRIVATE
        SHARED
        PUBLIC
    }
    class DocumentType {
        <<enumeration>>
        NOTE
        NPC
        CHARACTER
        SCENARIO
        SCENE
        LOCATION
        CUSTOM
    }
    class BlockKind {
        <<enumeration>>
        TEXT
        FIELD
        STAT_BAR
        RELATION
        LIST
        ITEM
        CHECKLIST
        IMAGE
    }
    Document "1" *-- "0..*" DocumentBlock
    Document --> DocumentType
    Document --> Visibility
    DocumentBlock *-- BlockValue
    DocumentBlock --> BlockKind
    BlockValue <|-- TextBlockValue
    BlockValue <|-- FieldBlockValue
    BlockValue <|-- StatBarBlockValue
    BlockValue <|-- RelationBlockValue
    BlockValue <|-- ListBlockValue
    BlockValue <|-- ItemBlockValue
    BlockValue <|-- ChecklistBlockValue
    BlockValue <|-- ImageBlockValue
    note for Document "customType obligatoire\nsi type = CUSTOM\nnull sinon"
    note for DocumentBlock "isPrivate = true → jamais exposé aux joueurs\nmême si Document est SHARED\nisLocked = true → non modifiable par joueur\n(modélisé, inactif en MVP)"
    note for Visibility "PRIVATE = MJ uniquement\nPLAYER_PRIVATE = joueur créateur uniquement (MJ exclu)\nSHARED = membres ciblés via ContentAccessRule\nPUBLIC = tous les membres"
    note for BlockValue "Un sous-type par BlockKind\nAucun champ optionnel superflu"
```

---

## 4. Content Library — Enveloppes métier

Le pattern structurant de Content Library : chaque concept métier (`NPC`, `PlayerCharacter`,
`Scenario`, `Scene`) possède un `Document` pour son contenu variable, et porte
ses propres règles métier dans une enveloppe séparée.

`NPC` et `PlayerCharacter` sont des **entités avec repository**, pas des agrégats racines.
Ils ont leur propre Id et leur propre cycle de vie, mais pas d'entités enfants à protéger
transactionnellement. Leurs seuls invariants propres sont leur statut et leurs liens narratifs.

`Scenario` est un agrégat racine car il protège l'ordre et la cohérence de sa collection
de `Scene`. Une scène ne peut pas exister sans son scénario parent.

Le lien `NPC ↔ PlayerCharacter` est une **association narrative bidirectionnelle optionnelle**.
Les deux entités ont des cycles de vie indépendants — la suppression de l'un ne supprime
pas l'autre. Ce lien prépare la future promotion NPC → Personnage joueur sans migration.

`DocumentTemplate` définit un schéma de blocs attendus pour un type de document.
Un document créé depuis un template est un snapshot indépendant — modifier le template
ne modifie pas les documents déjà créés.

```mermaid
classDiagram
    direction TB
    class NPC {
        <<entity — repository>>
        +id: NpcId
        +campaignId: CampaignId
        +documentId: DocumentId
        +name: String
        +status: NpcStatus
        +linkedCharacterId: CharacterId?
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class PlayerCharacter {
        <<entity — repository>>
        +id: CharacterId
        +campaignId: CampaignId
        +documentId: DocumentId
        +name: String
        +ownerId: UserId?
        +linkedNpcId: NpcId?
        +status: CharacterStatus
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class Scenario {
        <<aggregate root>>
        +id: ScenarioId
        +campaignId: CampaignId
        +documentId: DocumentId
        +title: String
        +slug: Slug
        +order: Int
        +status: ScenarioStatus
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class Scene {
        <<entity>>
        +id: SceneId
        +scenarioId: ScenarioId
        +documentId: DocumentId
        +title: String
        +order: Int
        +status: SceneStatus
        +linkedNpcIds: NpcId[]
        +audit: AuditInfo
    }
    class DocumentTemplate {
        <<aggregate root>>
        +id: TemplateId
        +name: String
        +documentType: DocumentType
        +gameSystemId: GameSystemId?
        +scope: TemplateScope
        +ownerId: UserId?
        +campaignId: CampaignId?
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class BlockSchema {
        <<value object>>
        +kind: BlockKind
        +label: String
        +required: Boolean
        +defaultValue: BlockValue?
    }
    class NpcStatus {
        <<enumeration>>
        ALIVE
        DEAD
        MISSING
        UNKNOWN
    }
    class CharacterStatus {
        <<enumeration>>
        ACTIVE
        RETIRED
        DEAD
    }
    class ScenarioStatus {
        <<enumeration>>
        DRAFT
        READY
        PLAYED
        ARCHIVED
    }
    class SceneStatus {
        <<enumeration>>
        PENDING
        PLAYED
        SKIPPED
    }
    class TemplateScope {
        <<enumeration>>
        BUILTIN
        CAMPAIGN
        USER
    }
    NPC --> NpcStatus
    NPC --> Document : documentId
    NPC ..> PlayerCharacter : linkedCharacterId
    PlayerCharacter --> CharacterStatus
    PlayerCharacter --> Document : documentId
    PlayerCharacter ..> NPC : linkedNpcId
    Scenario "1" *-- "0..*" Scene
    Scenario --> ScenarioStatus
    Scenario --> Document : documentId
    Scene --> SceneStatus
    Scene --> Document : documentId
    Scene ..> NPC : linkedNpcIds
    DocumentTemplate "1" *-- "0..*" BlockSchema
    DocumentTemplate --> TemplateScope
    Document ..> DocumentTemplate : snapshot à la création
    note for NPC "Entité avec repository\nPas d'agrégat racine\nPas d'entités enfants"
    note for PlayerCharacter "ownerId null = en attente\nd'association joueur"
    note for DocumentTemplate "Combinaisons valides :\nBUILTIN → ownerId null, campaignId null\nCAMPAIGN → ownerId non-null, campaignId non-null\nUSER → ownerId non-null, campaignId null"
    note for Scenario "order : Int géré par ScenarioOrderService\nservice domaine — persiste l'ordre dans Scenario\nCardinality 0..* : scénario sans scènes autorisé"
    note for Scene "Cycle de vie lié à Scenario\nSuppression en cascade\nL'ordre est une responsabilité\nde l'agrégat Scenario\npas de la Scene elle-même"
```

---

## 5. Session Conduct

Contexte opérationnel. `Session` est le seul agrégat racine — il encapsule les notes
prises en temps réel (`LiveNote`) et le compte-rendu final (`SessionSummary`).

Tous les liens vers les autres contextes sont des **références légères par Id** :
`campaignId`, `scenarioId`, `participantIds`, `pinnedItems.documentId`. Session Conduct
ne possède aucune entité des autres contextes — il les consomme par Id conformément
à la règle d'isolation des bounded contexts.

`PinnedItem` est un value object qui enrichit la simple liste d'Ids : il capture
l'ordre d'affichage et la date d'épinglage, ce qui permet au MJ de réordonner
ses éléments de référence pendant la session.

`LiveNote.visibility` permet au MJ de partager une note en direct aux joueurs
pendant la session. La valeur par défaut `PRIVATE` garantit qu'aucune note
n'est exposée accidentellement.

L'invariant le plus fort de ce contexte : **une seule session `LIVE` par campagne
à la fois**. Les transitions de statut sont unidirectionnelles :
`PLANNED → LIVE → CLOSED → ARCHIVED`.

```mermaid
classDiagram
    direction TB
    class Session {
        <<aggregate root>>
        +id: SessionId
        +campaignId: CampaignId
        +scenarioId: ScenarioId?
        +title: String
        +slug: Slug
        +scheduledAt: DateTime?
        +startedAt: DateTime?
        +endedAt: DateTime?
        +status: SessionStatus
        +participantIds: CharacterId[]
        +selectedNpcIds: NpcId[]
        +pinnedItems: PinnedItem[]
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class PinnedItem {
        <<value object>>
        +documentId: DocumentId
        +order: Int
        +pinnedAt: DateTime
    }
    class LiveNote {
        <<entity>>
        +id: LiveNoteId
        +sessionId: SessionId
        +content: String
        +authorId: UserId
        +visibility: Visibility
        +linkedDocumentId: DocumentId?
        +audit: AuditInfo
    }
    class SessionSummary {
        <<entity>>
        +id: SummaryId
        +sessionId: SessionId
        +content: String
        +visibility: Visibility
        +audit: AuditInfo
        +softDelete: SoftDelete
    }
    class SessionStatus {
        <<enumeration>>
        PLANNED
        LIVE
        CLOSED
        ARCHIVED
    }
    Session "1" *-- "0..*" PinnedItem
    Session "1" *-- "0..*" LiveNote
    Session "1" *-- "0..1" SessionSummary
    Session --> SessionStatus
    note for Session "Références cross-context par Id uniquement\n─────────────────────────────────────\ncampaignId → Campaign Management\nscenarioId → Content Library\nparticipantIds → Content Library\nselectedNpcIds → Content Library (NpcId[])\npinnedItems.documentId → Content Library\n─────────────────────────────────────\nselectedNpcIds : auto-déduit depuis scènes du scénario\npar SessionNpcSelector (domain service)\nSurcharge manuelle possible par le MJ\n─────────────────────────────────────\nInvariant : une seule session LIVE\npar campagne à la fois\nTransitions : PLANNED→LIVE→CLOSED→ARCHIVED\nCLOSED = contenu éditable, ARCHIVED = lecture seule"
    note for LiveNote "visibility = PRIVATE par défaut\nPeut être partagée aux joueurs\npendant la session (UC-06)"
    note for SessionSummary "PRIVATE = MJ uniquement\nSHARED = joueurs ciblés"
    note for PinnedItem "Remplace pinnedDocumentIds[]\nCapture ordre et date d'épinglage"
```

---

## 6. Context Map

Vue globale des dépendances entre bounded contexts. Les flèches indiquent
la direction de consommation — il n'y a pas de dépendance circulaire.

Le **Shared Kernel** est la fondation commune consommée par tous les contextes.
Il contient uniquement des primitives stables sans règle métier.

**Identity & Access** est le contexte upstream — il fournit `UserId` à tous les autres.
Aucun contexte ne remonte d'informations vers Identity.

**Campaign Management** fournit `CampaignId` aux deux contextes en aval. Il héberge
aussi `AccessPolicy` dont la `ContentAccessRule` référence des `DocumentId` de Content Library
par Id uniquement — la seule dépendance cross-context dans les données persistées.

**Content Library** fournit ses Id (`DocumentId`, `CharacterId`, `ScenarioId`) à Session Conduct
qui les consomme comme références légères.

```mermaid
graph TB
    subgraph CORE[Shared Kernel]
        IDS[Id types]
        PRIM[AuditInfo SoftDelete Email Slug Tag Visibility PinnedItem]
        ABS[IAggregateRoot IEntity IDomainEvent IRepository IUnitOfWork]
    end
    subgraph IA[Identity and Access]
        U[User]
        UR[UserRole GM PLAYER]
    end
    subgraph CM[Campaign Management]
        CA[Campaign]
        MB[CampaignMembership]
        INV[Invitation]
        GA[GuestAccess]
        GS[GameSystem]
        AP[AccessPolicy]
        CR[ContentAccessRule]
    end
    subgraph CL[Content Library]
        DOC[Document et DocumentBlock]
        NPC[NPC]
        PC[PlayerCharacter]
        SCE[Scenario et Scene]
        TPL[DocumentTemplate]
    end
    subgraph SE[Session Conduct]
        SS[Session]
        LN[LiveNote]
        SUM[SessionSummary]
        PI[PinnedItem]
    end
    CORE -.->|primitives et Id types| IA
    CORE -.->|primitives et Id types| CM
    CORE -.->|primitives et Id types| CL
    CORE -.->|primitives et Id types| SE
    IA -->|UserId et UserRole| CM
    IA -->|UserId| CL
    IA -->|UserId| SE
    CM -->|CampaignId| CL
    CM -->|CampaignId| SE
    CR -.->|DocumentId ref cross-context| DOC
    CL -->|DocumentId CharacterId ScenarioId| SE
    CA --- MB
    CA --- INV
    INV -.->|cree| GA
    AP --- CR
    CA -.->|ref| GS
```
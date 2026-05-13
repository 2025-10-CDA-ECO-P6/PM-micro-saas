# Bounded Context — Content Library

### Responsabilité

Tout le contenu éditorial de la campagne et de la bibliothèque personnelle du MJ.
Le modèle central est `Document` — une unité de contenu modulaire composée de `DocumentBlock`
fortement typés.

`PlayerCharacter` est le seul profil spécialisé maintenu en tant qu'entité distincte :
`CharacterId` est un pivot d'accès dans `AccessPolicy` et `GuestRequesterId`. Les métadonnées
de profil (Nom, Joueur, Description) transitent par le `DocumentType` BUILTIN "Personnage joueur"
via les `properties` du Document associé.

Les fiches PNJ, lieux, objets et factions sont des `Document(role = STANDARD)` ordinaires,
enrichis optionnellement d'un `DocumentType`.

---

### Agrégat : `Document`

```
Document
├── id                     : DocumentId
├── campaignId             : CampaignId?          — null si document en bibliothèque (scénario source)
├── role                   : DocumentRole          — rôle structurel dans le domaine
├── title                  : String
├── slug                   : Slug                  — unique par (campaignId, role) ; non contraint si campaignId = null
├── visibility             : Visibility            — toujours PRIVATE si campaignId = null
├── ownerCharacterId       : CharacterId?           — renseigné si visibility = PLAYER_PRIVATE
├── templateId             : TemplateId?
├── documentTypeId         : DocumentTypeId?        — type optionnel (PNJ, Lieu, Objet…) ; null = document libre
├── properties             : JSONB?               — valeurs des propriétés du DocumentType ; null si pas de type
├── folderId               : FolderId?            — null = non classé ; toujours null si campaignId = null
├── tagIds                 : TagId[]              — toujours vide si campaignId = null
├── blocks                 : DocumentBlock[]
├── audit                  : AuditInfo
└── softDelete             : SoftDelete
```

```
DocumentRole
├── STANDARD    — document libre, note ou contenu éditorial sans rôle structurant (défaut)
├── SCENARIO    — document associé à un agrégat Scenario
└── SCENE       — document associé à une entité Scene
```

#### Entité enfant : `DocumentBlock`

```
DocumentBlock
├── id          : BlockId
├── documentId  : DocumentId
├── kind        : BlockKind
├── label       : String?
├── order       : Int
├── value       : BlockValue
├── isPrivate   : Boolean
└── audit       : AuditInfo        — pas de SoftDelete, suppression physique
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
│   └── targetId : DocumentId
│
├── ListBlockValue
│   └── items : String[]
│
├── ItemBlockValue
│   ├── name       : String
│   ├── quantity   : Int
│   └── properties : Map<String, String>
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
- Un bloc `RELATION` valide que le `targetId` appartient à la même campagne si `campaignId` non null.
  Pour les documents en bibliothèque (`campaignId = null`), cette validation est déléguée à la couche applicative.
- Un bloc `isPrivate = true` n'est jamais exposé aux joueurs, même si le Document est `SHARED`.
- Soft delete `Document` → suppression physique de tous ses blocs.
- Un `Document` créé depuis un template est un snapshot indépendant.
- `ownerCharacterId` est obligatoire si `visibility = PLAYER_PRIVATE`, null sinon.
- La visibilité par défaut d'un `Document` associé à un `PlayerCharacter` sans accès joueur est `PRIVATE`.
  La fiche personnage elle-même suit les règles de `PlayerCharacter` : le MJ y a toujours accès,
  et le joueur associé y accède via son `CharacterId`.
- `documentTypeId` non null implique `properties` non null (peut être `{}`).
- Les champs `documentTypeId` et `properties` sont au coeur du système documentaire MVP :
  ils permettent des documents libres, typés, filtrables et extensibles sans créer une entité
  spécialisée pour chaque catégorie métier.

**Documents en bibliothèque** (`campaignId = null`) :
- Un document en bibliothèque est toujours rattaché à un agrégat `Scenario(isTemplate = true)` — jamais créé de façon indépendante.
- `visibility = PRIVATE` immuable — `AccessPolicy` ne s'applique pas à ces documents.
- `folderId = null` et `tagIds = []` — les dossiers et tags sont des concepts de campagne.
- Le scope d'accès est déduit depuis le `Scenario.ownerId` parent.

**Co-création obligatoire** :
- `Document(role = SCENARIO)` uniquement via la factory de l'agrégat `Scenario`.
- `Document(role = SCENE)` uniquement via la factory de l'entité `Scene`.
- `Document` associé à un `PlayerCharacter` uniquement via la factory de `PlayerCharacter`, qui assigne simultanément `documentTypeId = [Personnage joueur BUILTIN]`. Un document typé "Personnage joueur" sans `CharacterId` associé n'est pas un `PlayerCharacter` du point de vue du domaine.

**Backlinks** : un bloc `RELATION` dont le `targetId` pointe vers un document soft-deleted
n'est pas affiché dans les backlinks. La requête de backlinks filtre `DOCUMENT.isDeleted = false`.

#### Domain Events

```
DocumentCreated           { documentId, campaignId?, role, createdById, occurredAt }
DocumentTitleUpdated      { documentId, oldTitle, newTitle, occurredAt }
DocumentVisibilityChanged { documentId, oldVisibility, newVisibility, occurredAt }
DocumentMovedToFolder     { documentId, oldFolderId, newFolderId, occurredAt }
DocumentDeleted           { documentId, campaignId?, occurredAt }
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
├── color      : String?
├── audit      : AuditInfo
└── softDelete : SoftDelete
```

#### Invariants et règles métier

- Le `label` est unique par campagne (insensible à la casse).
- Supprimer un Tag est un soft delete et retire automatiquement sa référence de tous les Documents
  de la campagne (via event `TagDeleted`).
- Un Tag n'appartient qu'à une campagne — non partageable entre campagnes.

#### Domain Events

```
TagCreated  { tagId, campaignId, label, occurredAt }
TagUpdated  { tagId, campaignId, occurredAt }
TagDeleted  { tagId, campaignId, occurredAt }
```

---

### Profil spécialisé : `PlayerCharacter`

Vue spécialisée d'un `Document(role = STANDARD, documentTypeId = [Personnage joueur BUILTIN])`.
Le personnage est le point d'ancrage des droits joueur dans la campagne : un compte authentifié
ou un `GuestAccess` peut être associé à son `CharacterId`. Les données privées joueur sont liées
au personnage, pas au compte ni au guest temporaire.

`PlayerCharacter` est le seul profil spécialisé maintenu en entité distincte parce que
`CharacterId` est consommé par `AccessPolicy` et `GuestRequesterId` dans le Shared Kernel.

```
PlayerCharacter
├── id               : CharacterId
├── campaignId       : CampaignId
├── documentId       : DocumentId
├── name             : String           — dénormalisé, synchronisé via DocumentTitleUpdated
├── ownerId          : UserId?          — compte joueur associé, null si invité seul ou en attente
├── linkedDocumentId : DocumentId?      — association narrative optionnelle (typiquement un document PNJ)
├── status           : CharacterStatus
├── audit            : AuditInfo
└── softDelete       : SoftDelete
```

```
CharacterStatus
├── ACTIVE
├── RETIRED
└── DEAD
```

#### Invariants et règles métier

- `PlayerCharacter` appartient toujours à une campagne — pas de PlayerCharacter en bibliothèque.
- Le MJ peut toujours consulter la fiche personnage, car elle fait partie du matériel de campagne.
- Le `ownerId` ou un `GuestAccess` actif associé au `CharacterId` peut consulter et modifier
  les zones autorisées de sa fiche. Les permissions fines d'édition sont gérées par l'application
  sur les blocs et propriétés exposés au joueur.
- Les blocs `isPrivate = true` sont visibles uniquement par le MJ.
- `ownerId = null` → Document avec visibilité `PRIVATE` par défaut. Le MJ peut la modifier
  explicitement. L'association d'un `ownerId` ne change **pas automatiquement** la visibilité.
- Un même utilisateur peut posséder plusieurs `PlayerCharacter` dans une campagne.
  Le personnage courant est porté par `RequesterId.characterId` pendant l'accès.
- Les contenus `PLAYER_PRIVATE` liés à ce personnage utilisent `ownerCharacterId = characterId`.
  Ils restent récupérables par un futur `GuestAccess` ou compte associé au même personnage.
- `linkedDocumentId` est une association narrative — la suppression du document lié ne supprime pas le personnage.
- Soft delete PlayerCharacter → soft delete de son Document associé (même transaction).

#### Domain Events

```
CharacterCreated              { characterId, campaignId, documentId, occurredAt }
CharacterOwnerAssigned        { characterId, ownerId, occurredAt }
CharacterStatusChanged        { characterId, oldStatus, newStatus, occurredAt }
CharacterLinkedToDocument     { characterId, documentId, occurredAt }
CharacterUnlinkedFromDocument { characterId, occurredAt }
```

---

### Agrégat : `Scenario`

Gère la structure ordonnée des scènes et le statut de progression.
Le contenu narratif global est dans son Document associé.

Un scénario peut vivre en **bibliothèque personnelle** du MJ (scénario source réutilisable,
`isTemplate = true`) ou en tant qu'**instance** dans une campagne ou un one-shot.

```
Scenario
├── id               : ScenarioId
├── campaignId       : CampaignId?     — null si scénario en bibliothèque
├── ownerId          : UserId?         — renseigné si scénario en bibliothèque, null si dans campagne
├── documentId       : DocumentId
├── title            : String          — dénormalisé, synchronisé via DocumentTitleUpdated
├── slug             : Slug            — unique par campagne ; unique par propriétaire en bibliothèque
├── order            : Int             — position dans la campagne (ignoré si isTemplate = true)
├── status           : ScenarioStatus
├── isTemplate       : Boolean         — true = source réutilisable en bibliothèque
├── sourceScenarioId : ScenarioId?     — null si source ou indépendant, renseigné si instance copiée
├── scenes           : Scene[]
├── audit            : AuditInfo
└── softDelete       : SoftDelete
```

**Invariants de cohérence bibliothèque / campagne :**
- `isTemplate = true` → `campaignId = null` ET `ownerId` non null.
- `isTemplate = false` ET `campaignId = null` → impossible (une instance appartient toujours à une campagne).
- `sourceScenarioId` non null → `isTemplate = false` (une instance ne peut pas devenir source).
- Un scénario source ne peut être modifié que par son `ownerId` — jamais depuis une instance.

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
├── id                : SceneId
├── scenarioId        : ScenarioId
├── documentId        : DocumentId
├── title             : String         — dénormalisé, synchronisé via DocumentTitleUpdated
├── order             : Int            — position dans le scénario, géré par Scenario
├── status            : SceneStatus
├── linkedDocumentIds : DocumentId[]   — références légères (PNJ, lieux, objets liés à la scène)
└── audit             : AuditInfo     — pas de SoftDelete, suppression en cascade
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
- `linkedDocumentIds` : si un document lié est soft-deleted, sa référence est retirée sans supprimer la scène.
- La création d'une instance est une **copie profonde** : Document de scénario, scènes,
  documents de scène, blocs et documents liés nécessaires au scénario (PNJ, lieux, objets,
  notes de préparation) sont dupliqués dans la campagne cible.
  L'instance est indépendante du source après la copie — toute modification va dans l'instance.
- Un `Scenario(isTemplate = true)` ne peut être rattaché à aucune `Session` — seules les instances
  de campagne (`isTemplate = false`) peuvent l'être.
- L'historique des runs est reconstruit depuis les instances (`sourceScenarioId`), les campagnes
  one-shot et les sessions associées. Il n'existe pas d'entité `ScenarioRun` dédiée dans le MVP.

#### Domain Events

```
ScenarioCreated          { scenarioId, campaignId?, documentId, occurredAt }
ScenarioStatusChanged    { scenarioId, oldStatus, newStatus, occurredAt }
ScenarioReordered        { campaignId, occurredAt }
ScenarioMarkedAsTemplate { scenarioId, ownerId, occurredAt }
ScenarioInstanceCreated  { instanceId, sourceScenarioId, campaignId, occurredAt }
SceneAdded               { scenarioId, sceneId, documentId, occurredAt }
SceneRemoved             { scenarioId, sceneId, occurredAt }
SceneStatusChanged       { sceneId, scenarioId, oldStatus, newStatus, occurredAt }
SceneReordered           { scenarioId, occurredAt }
SceneDocumentLinked      { sceneId, documentId, occurredAt }
SceneDocumentUnlinked    { sceneId, documentId, occurredAt }
```

---

### Agrégat : `DocumentType`

Définit un schéma de propriétés structurées associé optionnellement à un `Document`.
Indépendant du `DocumentTemplate` (qui initialise les blocs) : un document peut avoir
un template sans type, un type sans template, les deux, ou aucun.

Types BUILTIN fournis par l'application, utilisables sans configuration :

| Nom | Propriétés |
|---|---|
| PNJ | Nom, Rôle, Affiliation, Description, Secret |
| Personnage joueur | Nom, Joueur, Description |
| Lieu | Nom, Type, Description |
| Objet | Nom, Rareté, Propriétaire |
| Faction | Nom, Alignement, Chef, Description |

```
DocumentType
├── id         : DocumentTypeId
├── name       : String
├── slug       : Slug               — unique par (scope, ownerId?, campaignId?)
├── scope      : DocumentTypeScope
├── ownerId    : UserId?            — null si BUILTIN
├── campaignId : CampaignId?        — null si scope USER ou BUILTIN
├── properties : PropertySchema[]
├── audit      : AuditInfo
└── softDelete : SoftDelete
```

```
DocumentTypeScope
├── BUILTIN    — fourni par l'application, non modifiable
├── CAMPAIGN   — défini pour une campagne spécifique
└── USER       — défini par un utilisateur, portable entre ses campagnes
```

```
PropertySchema
├── key       : String              — identifiant technique (stable, utilisé dans JSONB)
├── label     : String              — libellé affiché à l'utilisateur
├── valueType : PropertyValueType
└── required  : Boolean
```

```
PropertyValueType
├── TEXT
├── NUMBER
└── BOOLEAN
```

#### Combinaisons valides scope / ownerId / campaignId

| scope    | ownerId  | campaignId |
|---|---|---|
| BUILTIN  | null     | null       |
| CAMPAIGN | non-null | non-null   |
| USER     | non-null | null       |

**Invariant** : toute autre combinaison est rejetée par le constructeur.

#### Invariants et règles métier

- `BUILTIN` non modifiable, non supprimable.
- `CAMPAIGN` visible uniquement par les membres de la campagne.
- `USER` portable entre les campagnes de son propriétaire.
- Le `slug` est unique dans son scope : parmi les BUILTIN globaux, parmi les types d'une même campagne, parmi les types d'un même propriétaire USER.
- Supprimer un `DocumentType` custom ne supprime pas les Documents qui l'utilisent :
  leur `documentTypeId` passe à null et leurs `properties` sont conservées en lecture seule.
- La validation des `properties` d'un Document contre le schéma de son `DocumentType` est
  assurée par la couche application — le domaine n'invalide pas un document dont les propriétés
  ne correspondent plus au schéma (le type a pu évoluer après coup).

#### Domain Events

```
DocumentTypeCreated  { documentTypeId, scope, occurredAt }
DocumentTypeUpdated  { documentTypeId, occurredAt }
DocumentTypeDeleted  { documentTypeId, occurredAt }
```

---

### Agrégat : `DocumentTemplate`

Schéma de blocs définissant la structure initiale d'un `Document` à sa création.

```
DocumentTemplate
├── id           : TemplateId
├── name         : String
├── documentRole : DocumentRole?       — null = aucune restriction de rôle
├── gameSystemId : GameSystemId?
├── scope        : TemplateScope
├── ownerId      : UserId?             — null si BUILTIN
├── campaignId   : CampaignId?         — null si scope USER ou BUILTIN
├── schema       : BlockSchema[]
├── audit        : AuditInfo
└── softDelete   : SoftDelete
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
└── defaultValue : BlockValue?
```

**Note sur `required`** : indicateur pour l'UI et pour la création initiale depuis template.
Il ne bloque pas la sauvegarde d'un document incomplet.

#### Invariants et règles métier

- `BUILTIN` non modifiable, non supprimable.
- `CAMPAIGN` visible uniquement par les membres de la campagne.
- `USER` portable entre les campagnes de son propriétaire.
- Un Document créé depuis un template est un snapshot indépendant.
- Modifier un template n'a aucun effet automatique sur les Documents déjà créés.
  La synchronisation de template est hors MVP.

#### Domain Events

```
TemplateCreated           { templateId, scope, occurredAt }
TemplateUpdated           { templateId, occurredAt }
```

---

### Agrégat : `Folder`

Conteneur organisationnel créé par le MJ pour regrouper ses documents librement.
Chaque campagne démarre avec 4 dossiers système créés automatiquement.

```
Folder
├── id                    : FolderId
├── campaignId            : CampaignId
├── name                  : String
├── slug                  : Slug                  — unique par campaignId
├── defaultTemplateId     : TemplateId?           — template appliqué à la création d'un doc dans ce dossier
├── defaultDocumentTypeId : DocumentTypeId?        — DocumentType appliqué par défaut à la création
│                                                    applicable aux documents STANDARD uniquement
├── isSystem              : Boolean               — true = créé par le système, non supprimable
├── order                 : Int                   — position dans la navigation
├── audit                 : AuditInfo
└── softDelete            : SoftDelete
```

**Dossiers système créés à l'initialisation de la campagne :**

| Nom | isSystem | Type par défaut |
|---|---|---|
| Personnages | true | PNJ (BUILTIN) |
| Joueurs | true | Personnage joueur (BUILTIN) |
| Scénarios | true | — |
| Notes | true | — |

#### Invariants et règles métier

- Un dossier système (`isSystem = true`) ne peut pas être supprimé.
- Tous les dossiers (y compris système) peuvent être renommés.
- Le `slug` est régénéré depuis le `name` à la création, jamais modifié après.
- Supprimer un dossier non-système nécessite de traiter ses documents (déplacer ou déclasser).
- Changer `defaultTemplateId` ou `defaultDocumentTypeId` n'affecte jamais les documents existants.
- `defaultDocumentTypeId` s'applique uniquement aux Documents `STANDARD` — le dossier "Scénarios"
  ne peut pas imposer un type de document : ses documents sont créés via l'agrégat `Scenario`.
- L'ordre des dossiers est géré par `FolderOrderService` (application service).

#### Domain Events

```
FolderCreated  { folderId, campaignId, name, isSystem, occurredAt }
FolderRenamed  { folderId, campaignId, oldName, newName, occurredAt }
FolderDeleted  { folderId, campaignId, occurredAt }
```

#### Références entre documents — backlinks

Les références entre documents existent via `RelationBlockValue { targetId: DocumentId }`.
Un document peut pointer vers n'importe quel autre document de la même campagne via un bloc `RELATION`.

Les **backlinks** sont résolus via une requête sur `DOCUMENT_BLOCK` filtrée par
`kind = RELATION`, `value->>'targetId' = ?` **et `DOCUMENT.isDeleted = false`**.

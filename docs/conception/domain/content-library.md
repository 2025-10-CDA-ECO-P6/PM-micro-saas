# Bounded Context — Content Library

### Responsabilité

Tout le contenu éditorial de la campagne. Le modèle central est `Document` —
une unité de contenu modulaire composée de `DocumentBlock` fortement typés.

`NPC` et `PlayerCharacter` sont des **profils spécialisés de Document**.
Leur contenu flexible vit dans le `Document`; leur table dédiée porte uniquement
les métadonnées métier nécessaires aux listes, recherches et règles simples
(statut, propriétaire, lien narratif). Ils sont chargés via repository pour des raisons
pratiques, mais le modèle source de contenu reste le `Document`.

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
├── ownerCharacterId : CharacterId?   — renseigné si visibility = PLAYER_PRIVATE
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
- `ownerCharacterId` est obligatoire si `visibility = PLAYER_PRIVATE`, null sinon.
- La visibilité par défaut d'un `Document` associé à un `PlayerCharacter` sans accès joueur est `PRIVATE`. Le MJ la change explicitement pour la partager.

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
- Supprimer un Tag est un soft delete et retire automatiquement sa référence de tous les Documents de la campagne (via event `TagDeleted`).
- Un Tag n'appartient qu'à une campagne — non partageable entre campagnes.

#### Domain Events

```
TagCreated  { tagId, campaignId, label, occurredAt }
TagUpdated  { tagId, campaignId, occurredAt }
TagDeleted  { tagId, campaignId, occurredAt }
```

---

### Profil spécialisé : `NPC`

Vue spécialisée d'un `Document(type = NPC)`. Selon le système de jeu, un PNJ peut être
une simple description ou une fiche complète ; le modèle ne fige donc pas sa structure
dans l'entité `NPC`. Cette table porte seulement les métadonnées transverses utiles
au domaine et aux requêtes.

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

### Profil spécialisé : `PlayerCharacter`

Vue spécialisée d'un `Document(type = CHARACTER)`. Le personnage est le point d'ancrage
des droits joueur dans la campagne : un compte authentifié ou un `GuestAccess` peut être
associé à son `CharacterId`. Les données privées joueur sont liées au personnage,
pas au compte ni au guest temporaire.

```
PlayerCharacter
├── id              : CharacterId
├── campaignId      : CampaignId
├── documentId      : DocumentId
├── name            : String           — dénormalisé, synchronisé via DocumentTitleUpdated
├── ownerId         : UserId?          — compte joueur associé, null si invité seul ou en attente
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

- Seul le `ownerId`, un `GuestAccess` actif associé au `CharacterId`, ou le MJ peut modifier les blocs autorisés du Document associé.
- Les blocs `isPrivate = true` sont visibles uniquement par le MJ.
- `ownerId = null` → Document avec visibilité `PRIVATE` par défaut.
  Le MJ peut la modifier explicitement (SHARED ou PUBLIC) pour partager la fiche avec le groupe
  avant qu'un compte joueur soit associé (ex. : session avec invité sans compte ou joueur absent).
  L'association d'un `ownerId` ne change **pas automatiquement** la visibilité — c'est une action explicite du MJ.
- Les contenus `PLAYER_PRIVATE` liés à ce personnage utilisent `ownerCharacterId = characterId`.
  Ils restent donc récupérables par un futur `GuestAccess` ou compte authentifié associé au même personnage.
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

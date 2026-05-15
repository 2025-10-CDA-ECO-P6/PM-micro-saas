# Content Library

> **Responsabilité** : gérer tout le contenu durable d'une campagne.
> Documents, dossiers, types de documents. Ce contexte est le référentiel de tout
> ce que le MJ prépare avant et entre les séances.
>
> Il ne gère pas le déroulement de session (→ Session Conduct),
> ni les droits d'accès à la campagne (→ Campaign Management).

---

## Principe fondamental — Tout est Document

Haversack repose sur un **modèle de document unifié et récursif**.
Il n'existe pas de type "Scénario" ou "Scène" séparé du système documentaire.

Un Document est composé de deux couches :
1. **Blocs** (`DocumentBlock[]`) — le contenu libre : texte, tableau, image…
2. **Références** (`DocumentLink[]`) — des liens ordonnés vers d'autres Documents

Ce modèle permet au MJ de travailler comme il le souhaite :
- Écrire tout un scénario dans un seul document sans aucune scène.
- Structurer un scénario en scènes (documents SCENE référencés dans `linkedDocuments`).
- Utiliser un scénario comme une scène depuis un autre document.
- Créer un PNJ et le référencer depuis plusieurs scènes.

**La hiérarchie Scénario → Scènes → PNJ n'est qu'un cas d'usage parmi d'autres.**
Le système n'impose aucune structure — il la rend possible.

---

## Ce que ce contexte fait

- Créer, modifier, supprimer des documents et des dossiers.
- Gérer les types de documents (système + personnalisés futurs).
- Gérer la visibilité des documents (PUBLIC, GM_ONLY, PLAYER_PRIVATE).
- Gérer les références entre documents (DocumentLink).
- Créer les dossiers système à la création d'une campagne.
- Gérer la réutilisabilité des documents (templates et instances).
- Exposer les données pour la recherche full-text (PostgreSQL FTS).

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Droits d'accès à la campagne, membres | Campaign Management |
| Déroulement de session, notes de session, partage en temps réel | Session Conduct |
| Comptes utilisateurs | Identity & Access |
| Moteur de recherche (infrastructure FTS) | Application / Infrastructure |

---

## Agrégats

### Document (agrégat principal)

Représente n'importe quel contenu : note, PNJ, lieu, objet, scénario, scène,
personnage joueur, règle maison, faction… Le `documentTypeId` optionnel spécialise
le document sans contraindre sa structure libre.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentId` | |
| `campaignId` | `CampaignId` | |
| `folderId` | `FolderId` | Dossier parent |
| `title` | `string` | |
| `documentTypeId` | `DocumentTypeId?` | Optionnel — active les propriétés structurées |
| `properties` | `JSON?` | Propriétés structurées selon le type (prévu dès MVP pour éviter une migration future) |
| `blocks` | `DocumentBlock[]` | Contenu libre en blocs ordonnés |
| `linkedDocuments` | `DocumentLink[]` | Références ordonnées vers d'autres Documents |
| `visibility` | `Visibility` | `PUBLIC` \| `GM_ONLY` \| `PLAYER_PRIVATE` |
| `tags` | `Tag[]` | |
| `slug` | `Slug` | Unique par `(campaignId, documentTypeId)` |
| `isReusable` | `bool` | Ce document peut servir de template |
| `sourceDocumentId` | `DocumentId?` | Renseigné si instancié depuis un template |
| `AuditInfo` | | `createdAt`, `updatedAt`, `createdById: UserId` |
| `SoftDelete` | | `isDeleted`, `deletedAt` |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(campaignId, folderId, title, typeId?)` | `DocumentCreated` | |
| `UpdateContent(blocks)` | — | Met à jour les blocs |
| `LinkDocument(targetId, order)` | `DocumentLinked` | Ajoute une référence vers un autre document |
| `UnlinkDocument(targetId)` | `DocumentUnlinked` | Retire une référence |
| `Share()` | `DocumentVisibilityChanged` | Passe `visibility` à `PUBLIC` — permanent |
| `Unshare()` | `DocumentVisibilityChanged` | Repasse `visibility` à `GM_ONLY` |
| `Delete()` | `DocumentDeleted` | Soft-delete |
| `Instantiate(campaignId, folderId)` | `DocumentInstantiated` | Crée une copie indépendante depuis un template |

---

### DocumentBlock (entité dans Document)

Unité atomique de contenu. L'ensemble des blocs d'un document forme son corps.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentBlockId` | |
| `order` | `int` | Position dans le document |
| `type` | `BlockType` | `TEXT` \| `TABLE` \| `IMAGE` \| `DIVIDER` … |
| `content` | `JSON` | Contenu sérialisé selon le type de bloc |
| `isLocked` | `bool` | Modélisé, valeur `false` par défaut, non activé MVP |

---

### DocumentLink (value object dans Document)

Référence ordonnée d'un Document vers un autre.
C'est ce qui permet à un scénario de lister ses scènes, à une scène de lister ses PNJ,
ou à n'importe quel document de pointer vers n'importe quel autre.

| Champ | Type | Description |
|---|---|---|
| `targetDocumentId` | `DocumentId` | Document cible |
| `order` | `int` | Position dans la liste des références |

> **Note** : les backlinks (documents qui pointent vers un document donné) ne sont pas
> stockés — ils sont calculés à la lecture via une requête sur `DocumentLink.targetDocumentId`.

---

### Folder (agrégat)

Conteneur organisationnel. Structure l'arborescence du contenu dans une campagne.

| Champ | Type | Description |
|---|---|---|
| `id` | `FolderId` | |
| `campaignId` | `CampaignId` | |
| `parentFolderId` | `FolderId?` | `null` = niveau racine |
| `name` | `string` | |
| `isSystem` | `bool` | Créé automatiquement à `CampaignCreated` — simple point de départ, renommable et supprimable |
| `isVirtual` | `bool` | `true` pour le dossier "Non classés" — créé automatiquement par campagne, invisible dans la navigation MJ, non supprimable. Garantit que `folderId` reste non-nullable sur `Document`. |
| `defaultDocumentTypeId` | `DocumentTypeId?` | Type proposé par défaut pour les nouveaux docs dans ce dossier |
| `defaultTemplateDocumentId` | `DocumentId?` | Document réutilisable (`isReusable = true`) utilisé pour initialiser le contenu des nouveaux documents créés dans ce dossier. Optionnel. |
| `AuditInfo` | | |

**Dossiers système créés à `CampaignCreated`**

| Nom | Rôle par défaut |
|---|---|
| Personnages | Documents PLAYER_CHARACTER |
| Joueurs | Documents joueurs partagés |
| Scénarios | Documents SCENARIO |
| Notes | Documents NOTE |
| (Non classés) | Dossier virtuel invisible. Reçoit les documents sans dossier explicite. `isVirtual = true`, `isSystem = true`. |

> Ces noms sont système-agnostiques et ne sont que le point de départ.
> Le MJ peut renommer, réorganiser ou supprimer ces dossiers à sa guise.
> `isSystem = true` est purement informatif — il indique l'origine automatique, pas une contrainte.

---

### DocumentType (entité de référence)

Types de documents disponibles dans la campagne. Les types système sont seedés en base.
Les types personnalisés (Could Have) permettront au MJ de définir ses propres structures.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentTypeId` | |
| `slug` | `string` | Identifiant technique unique (`scenario`, `scene`, `npc`, `location`, `note`, `player_character`, `live_note`, `reveal`) |
| `name` | `string` | Nom affiché |
| `propertiesSchema` | `JSON?` | Schéma des propriétés structurées (prévu pour les types custom futurs) |
| `isSystem` | `bool` | Type built-in non modifiable |
| `campaignId` | `CampaignId?` | `null` pour les types système, renseigné pour les types custom |

> **Types système built-in** : `SCENARIO`, `SCENE`, `NPC`, `LOCATION`, `NOTE`, `PLAYER_CHARACTER`, `LIVE_NOTE`, `REVEAL`
>
> **Décision de conception** : table de référence plutôt qu'enum — prépare les types
> personnalisés sans migration future.

**Schéma `properties` du type `live_note`**

| Propriété | Type | Description |
|---|---|---|
| `characterId` | `string?` | Personnage associé (notes PLAYER_PRIVATE joueur) |
| `guestAccessId` | `string?` | Auteur invité sans compte (quand `createdById` est null) |

---

## Cas d'usage illustrés

### Scénario avec scènes structurées

```
Document (SCENARIO) "La Crypte Maudite"
  blocks: [intro du scénario]
  linkedDocuments:
    → Document (SCENE) "Entrée de la crypte"  [order: 1]
    → Document (SCENE) "La salle du trône"    [order: 2]
    → Document (SCENE) "Le boss final"         [order: 3]

Document (SCENE) "Entrée de la crypte"
  blocks: [description de la scène]
  linkedDocuments:
    → Document (NPC) "Le gardien zombie"      [order: 1]
    → Document (LOCATION) "Carte de l'entrée" [order: 2]
```

### Scénario sans scènes (tout en blocs)

```
Document (SCENARIO) "One-shot express"
  blocks: [tout le contenu écrit directement]
  linkedDocuments: []
```

### Scénario utilisé comme scène

```
Document (SCENARIO) "Campagne épique — Acte 1"
  blocks: [intro de l'acte]
  linkedDocuments:
    → Document (SCENARIO) "Chapitre 1 — La forêt"  [order: 1]
    → Document (SCENARIO) "Chapitre 2 — La ville"  [order: 2]
```

### Scène avec révélations joueurs

```
Document (SCENE) "Entrée de la crypte"
  visibility: GM_ONLY
  blocks: [description privée, notes MJ, secrets]
  linkedDocuments:
    → Document (REVEAL) "Indice : inscription sur la porte"  [order: 1]  visibility: GM_ONLY → PUBLIC (via UC-08)
    → Document (REVEAL) "Carte partielle de la crypte"       [order: 2]  visibility: GM_ONLY → PUBLIC (via UC-08)
    → Document (NPC)    "Le gardien zombie"                  [order: 3]  visibility: GM_ONLY
```

**Règle** : le document de scène reste `GM_ONLY`. Les documents `REVEAL` sont créés et liés automatiquement via un bouton "Ajouter une révélation" dans l'éditeur de scène. Leur visibilité passe à `PUBLIC` uniquement lors de la révélation explicite par le MJ pendant la session (→ UC-08). Avant révélation, ils sont `GM_ONLY`.

---

## Invariants métier

1. Un `Document` appartient toujours à exactement un `Folder`. Si le dossier d'un document est supprimé, le document est automatiquement déplacé vers le dossier virtuel "Non classés" de la campagne.
2. Un `Folder` appartient toujours à exactement une `Campaign`.
3. Les dossiers système (`isSystem = true`) sont le point de départ d'une campagne. Le MJ peut les renommer ou les supprimer librement — `isSystem` est informatif, pas restrictif.
4. Un `Folder` avec `isVirtual = true` n'est pas supprimable et n'est pas affiché dans la navigation. Il en existe exactement un par campagne. Il reçoit tout document dont le dossier explicite a été supprimé.
5. Le `slug` d'un Document est unique par `(campaignId, documentTypeId)`.
6. Un Document `PLAYER_PRIVATE` n'est lisible que par son `createdById` et par les membres `OWNER` et `GM`.
7. La suppression d'un `Folder` déplace ses Documents vers le dossier virtuel "Non classés" de la campagne, ou vers un autre dossier choisi par le MJ au moment de la suppression. Aucun Document n'est supprimé implicitement par la suppression de son dossier.
8. Un Document avec `isReusable = false` ne peut pas être instancié.
9. Une instance (`sourceDocumentId` renseigné) est totalement indépendante de son source après création.
10. La visibilité `PUBLIC` est permanente jusqu'à `Unshare()` explicite — un document partagé reste accessible aux joueurs entre les sessions.

---

## Règles métier

1. `visibility = PUBLIC` : le document est visible par tous les membres de la campagne et les GuestAccess actifs.
2. `visibility = GM_ONLY` : visible uniquement par les membres `OWNER` et `GM`.
3. `visibility = PLAYER_PRIVATE` : visible uniquement par le `createdById` et les membres `OWNER` et `GM`. **Exception** : pour les Documents de type `LIVE_NOTE` avec `visibility = PLAYER_PRIVATE`, le document n'est lisible que par son auteur (`createdById` ou `properties.guestAccessId`) — les membres `OWNER` et `GM` n'y ont pas accès (RB-06-25). Cette règle est encodée dans `Document.CanBeReadBy(userId, memberRole, documentType)` et non dans un service applicatif.
4. Partager un document (`Share()`) change sa visibilité de façon permanente. Ce n'est pas un partage temporaire de session — le joueur peut y accéder entre les séances.
5. L'instanciation d'un document réutilisable crée une copie profonde (blocs + liens + propriétés). Les modifications ultérieures du source n'affectent pas les instances.
6. Les backlinks ne sont pas stockés — ils sont calculés en lecture par une requête sur `document_links.target_document_id`.
7. Les types système (`isSystem = true`) ne peuvent pas être modifiés ni supprimés.
8. Un document `REVEAL` lié à une scène est créé avec `visibility = GM_ONLY` par défaut. Il passe à `PUBLIC` uniquement via une action explicite du MJ en session (→ UC-08 — partage d'information). Ce passage est permanent jusqu'à `Unshare()`.
9. Si un `Folder.defaultTemplateDocumentId` est défini, tout nouveau `Document` créé dans ce dossier est initialisé en appelant `Document.Instantiate()` sur le template. Si le template est supprimé, le champ passe à `null` — les documents existants ne sont pas affectés.

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `DocumentCreated` | `Document.Create()` | Session Conduct (si créé à la volée en session) |
| `DocumentVisibilityChanged` | `Document.Share()` / `Unshare()` | Session Conduct (mise à jour de la vue joueur en temps réel) |
| `DocumentDeleted` | `Document.Delete()` | Session Conduct (retirer des documents épinglés si actif en session) |
| `DocumentLinked` | `Document.LinkDocument()` | — (backlinks calculés en lecture) |
| `DocumentUnlinked` | `Document.UnlinkDocument()` | — |
| `DocumentInstantiated` | `Document.Instantiate()` | Campaign Management si besoin |
| `FolderCreated` | `Folder.Create()` | — |
| `FolderDeleted` | `Folder.Delete()` | Session Conduct (retirer les documents du dossier supprimé si actifs en session) |

---

## Intégration avec les autres contextes

### Ce que Content Library reçoit

| Événement | Source | Action |
|---|---|---|
| `CampaignCreated` | Campaign Management | Créer les 5 dossiers système (dont le dossier virtuel "Non classés") |
| `CampaignArchived` | Campaign Management | Passer tous les documents en lecture seule (soft-lock) |

### Ce que Content Library publie

- `DocumentId`, `FolderId` comme identifiants de référence.
- Les événements listés ci-dessus.

### Utilisation par les autres contextes

| Contexte | Usage |
|---|---|
| Session Conduct | `DocumentId` pour les documents sélectionnés, `DocumentLink` pour construire la vue session, `visibility` pour filtrer ce que les joueurs voient |
| Campaign Management | `DocumentId` comme `characterId` dans `CampaignMembership` (référence logique sans FK physique) |

---

## Concepts en attente d'arbitrage

### ScenarioLibrary (UC-13)

UC-13 et US-13 introduisent une `ScenarioLibrary` appartenant au compte MJ, cross-campagne.
Ce concept n'est pas encore modélisé dans ce contexte.

**Deux options :**

**Option A — Agrégat `ScenarioLibrary` dans Content Library**
La `ScenarioLibrary` est un agrégat léger avec une liste de références de scénarios marqués comme réutilisables (`isReusable = true`). L'instanciation reste dans `Document.Instantiate()`. Avantage : minimal, ne crée pas un nouveau contexte.

**Option B — Extension de Campaign Management** *(recommandé)*
La `ScenarioLibrary` vit au niveau du compte, pas de la campagne — elle dépasse les responsabilités de Content Library qui est toujours campagne-scoped. Un micro-agrégat `ScenarioLibraryEntry(userId, documentId, promotedAt)` dans Campaign Management est plus cohérent : Campaign Management gère déjà les ressources au niveau du compte MJ (quotas, ownerId).

> Décision : **Option B retenue**. Implémentation dans Campaign Management avant UC-13.

---

## Diagrammes

→ [Classes](diagrams/classes/content-library.md)
→ [MCD](diagrams/mcd/content-library.md)
→ [MLD](diagrams/mld/content-library.md)
→ [Flux](diagrams/flows/content-library.md)

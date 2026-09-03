# Content Library

> **Responsabilité** : gérer tout le contenu durable d'un espace.
> Documents, dossiers, types de documents. Ce contexte est le référentiel de tout
> ce que le MJ prépare avant et entre les séances.
>
> Il ne gère pas le déroulement de session (→ Session Conduct),
> ni les droits d'accès à l'espace (→ Space Management).

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
- Créer les dossiers système à la création d'un espace.
- Gérer la réutilisabilité des documents (templates et instances).
- Exposer les données nécessaires à la recherche plein texte.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Droits d'accès à l'espace, membres | Space Management |
| Déroulement de session, notes de session, partage en temps réel | Session Conduct |
| Comptes utilisateurs | Identity & Access |
| Moteur de recherche | Application / Infrastructure |

---

## Agrégats

### Document (agrégat principal)

Représente n'importe quel contenu : note, PNJ, lieu, objet, scénario, scène,
personnage joueur, règle maison, faction… Le `documentTypeId` optionnel spécialise
le document sans contraindre sa structure libre.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentId` | |
| `spaceId` | `SpaceId` | |
| `folderId` | `FolderId` | Dossier parent |
| `title` | `string` | |
| `documentTypeId` | `DocumentTypeId?` | Optionnel — active les propriétés structurées |
| `properties` | `structure?` | Propriétés structurées selon le type (prévu dès MVP pour éviter une migration future) |
| `blocks` | `DocumentBlock[]` | Contenu libre en blocs ordonnés |
| `linkedDocuments` | `DocumentLink[]` | Références ordonnées vers d'autres Documents |
| `visibility` | `Visibility` | `PUBLIC` \| `GM_ONLY` \| `PLAYER_PRIVATE` |
| `tags` | `Tag[]` | |
| `slug` | `Slug` | Unique par `(spaceId, documentTypeId)` |
| `isReusable` | `bool` | Ce document peut servir de template |
| `sourceDocumentId` | `DocumentId?` | Renseigné si instancié depuis un template |
| `characterId` | `DocumentId?` | Référence intra-module vers un Document de type `player_character` — personnage associé d'un LIVE_NOTE (promu depuis `properties`, ADR-002) |
| `guestAccessId` | `GuestAccessId?` | Référence cross-module vers les accès invités (Space Management) — auteur invité d'un LIVE_NOTE quand `createdById` est null (promu depuis `properties`, ADR-002) |
| `AuditInfo` | | `createdAt`, `updatedAt`, `createdById: UserId` |
| `SoftDelete` | | `isDeleted`, `deletedAt` |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(spaceId, folderId, title, typeId?)` | `DocumentCreated` | |
| `UpdateContent(blocks)` | — | Met à jour les blocs |
| `LinkDocument(targetId, order)` | `DocumentLinked` | Ajoute une référence vers un autre document |
| `UnlinkDocument(targetId)` | `DocumentUnlinked` | Retire une référence |
| `Share()` | `DocumentVisibilityChanged` | Passe `visibility` à `PUBLIC` — permanent |
| `Unshare()` | `DocumentVisibilityChanged` | Repasse `visibility` à `GM_ONLY` |
| `Delete()` | `DocumentDeleted` | Soft-delete |
| `Instantiate(spaceId, folderId)` | `DocumentInstantiated` | Crée une copie indépendante depuis un template |

---

### DocumentBlock (entité dans Document)

Unité atomique de contenu. L'ensemble des blocs d'un document forme son corps.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentBlockId` | |
| `order` | `int` | Position dans le document |
| `type` | `BlockType` | `TEXT` \| `TABLE` \| `IMAGE` \| `DIVIDER` … |
| `content` | `structure` | Contenu structuré selon le type de bloc |
| `isLocked` | `bool` | Modélisé, valeur `false` par défaut, non activé MVP |

**Format de `content` — tranché** (décision d'entrée en build du 2026-09-03). Le contenu d'un bloc est un **arbre de nœuds typés**, jamais une chaîne de balisage. Un nœud porte son type et, selon ce type, ses enfants ou sa valeur textuelle. L'interface rend cet arbre en construisant ses éléments ; elle n'injecte aucune chaîne de balisage.

**Pourquoi ce point relevait de la conception et non de la sécurité.** La spécification de sanitisation présupposait une liste de balises HTML autorisées — donc un contenu balisé — que rien ici n'établissait. La « liste blanche positive » du plancher de sanitisation porte désormais sur l'**énumération fermée des types de nœuds** que ce modèle définit, ce qui réduit la surface d'injection aux seules feuilles textuelles.

*Voir [`specs/sanitisation-csp.md`](../../architecture/specs/sanitisation-csp.md) pour le plancher, inchangé.*

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
> stockés — ils sont calculés à la lecture via `DocumentLink.targetDocumentId`.

---

### Folder (agrégat)

Conteneur organisationnel. Structure l'arborescence du contenu dans un espace.

| Champ | Type | Description |
|---|---|---|
| `id` | `FolderId` | |
| `spaceId` | `SpaceId` | |
| `parentFolderId` | `FolderId?` | `null` = niveau racine |
| `name` | `string` | |
| `isSystem` | `bool` | Créé automatiquement à `SpaceCreated` — simple point de départ, renommable et supprimable |
| `isVirtual` | `bool` | `true` pour le dossier "Non classés" — créé automatiquement par espace, invisible dans la navigation MJ, non supprimable. Garantit que `folderId` reste non-nullable sur `Document`. |
| `defaultDocumentTypeId` | `DocumentTypeId?` | Type proposé par défaut pour les nouveaux docs dans ce dossier |
| `defaultTemplateDocumentId` | `DocumentId?` | Document réutilisable (`isReusable = true`) utilisé pour initialiser le contenu des nouveaux documents créés dans ce dossier. Optionnel. |
| `order` | `int` | Ordre d'affichage dans le dossier parent. `0` par défaut. |
| `AuditInfo` | | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(spaceId, name, parentId?)` | `FolderCreated` | Crée un dossier. `parentId` optionnel — `null` = niveau racine. |
| `Rename(name)` | — | Renomme le dossier. |
| `Delete()` | `FolderDeleted` | Supprime le dossier. Ses documents sont déplacés vers le dossier virtuel « Non classés » ou vers un autre dossier choisi par le MJ (invariant 7). |

**Dossiers créés à `SpaceCreated`**

La création des dossiers est conditionnelle au type d'espace.

**Pour un espace `CAMPAIGN` ou `ONE_SHOT`** — 4 dossiers système nommés + 1 dossier virtuel :

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

**Pour un espace `PERSONAL`** — uniquement le dossier virtuel :

| Nom | Rôle par défaut |
|---|---|
| (Non classés) | Dossier virtuel invisible. Reçoit les documents sans dossier explicite. `isVirtual = true`, `isSystem = true`. |

> L'espace personnel démarre sans dossiers nommés : les concepts « Joueurs » et « Personnages » n'ont pas de sens dans un espace mono-membre. Le propriétaire organise son espace avec ses propres dossiers (organisation libre). L'invariant 4 — un dossier virtuel par espace — reste valide pour tous les types.

---

### DocumentType (entité de référence)

Types de documents disponibles dans l'espace. Les types système sont fournis avec l'application
et ne peuvent pas être modifiés. Les types personnalisés (Could Have) permettront au MJ de définir ses propres structures.

| Champ | Type | Description |
|---|---|---|
| `id` | `DocumentTypeId` | |
| `slug` | `string` | Identifiant technique unique (`scenario`, `scene`, `npc`, `location`, `note`, `player_character`, `live_note`, `reveal`) |
| `name` | `string` | Nom affiché |
| `propertiesSchema` | `structure?` | Schéma des propriétés structurées (prévu pour les types custom futurs) |
| `isSystem` | `bool` | Type built-in non modifiable |
| `spaceId` | `SpaceId?` | `null` pour les types système, renseigné pour les types custom |

> **Types système built-in** : `SCENARIO`, `SCENE`, `NPC`, `LOCATION`, `NOTE`, `PLAYER_CHARACTER`, `LIVE_NOTE`, `REVEAL`
>
> **Décision de conception** : entité de référence plutôt que liste fermée — permet les types
> personnalisés sans remettre en cause le modèle.

**Propriétés structurées du type `live_note`**

> Les champs domaine `characterId` et `guestAccessId` du Document sont promus au niveau des champs de premier niveau (ADR-002) — ils ne font plus partie des propriétés structurées.

*Aucune propriété structurée supplémentaire spécifique au type `live_note` n'est définie pour le MVP.*

---

**Propriétés structurées du type `scenario`**

Le statut d'un scénario (brouillon / prêt / joué / archivé, évoqué par US-03-06) se loge dans
les **propriétés structurées (`properties`) du type `SCENARIO`**. Il n'y a pas de nouveau champ
`status` sur `Document` ni de nouvelle entité — le modèle générique le permet déjà.

| Propriété structurée | Type | Valeurs | Description |
|---|---|---|---|
| `scenarioStatus` | `enum` | `DRAFT` \| `READY` \| `PLAYED` \| `ARCHIVED` | État éditorial du scénario, géré par le MJ. Distinct du `status` d'espace (Space Management) et du `status` de session (Session Conduct). |

> `scenarioStatus` est porté par `Document.properties` et est invisible au modèle générique `Document`.
> Il n'est accessible qu'en présence d'un `documentTypeId = SCENARIO`.
> La valeur par défaut à la création est `DRAFT`.

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

1. Un `Document` appartient toujours à exactement un `Folder`. Si le dossier d'un document est supprimé, le document est automatiquement déplacé vers le dossier virtuel "Non classés" de l'espace.
2. Un `Folder` appartient toujours à exactement un `Space`.
3. Les dossiers système (`isSystem = true`) sont le point de départ d'un espace. Le MJ peut les renommer ou les supprimer librement — `isSystem` est informatif, pas restrictif.
4. Un `Folder` avec `isVirtual = true` n'est pas supprimable et n'est pas affiché dans la navigation. Il en existe exactement un par espace. Il reçoit tout document dont le dossier explicite a été supprimé.
5. Le `slug` d'un Document est unique par `(spaceId, documentTypeId)`.
6. Un Document `PLAYER_PRIVATE` n'est lisible que par son auteur (`createdById` pour un membre, ou `guestAccessId` pour un invité). Les membres `OWNER` et `GM` n'y ont aucun accès — ni lecture directe, ni énumération, ni métadonnées. Cette règle s'applique aussi à la résolution transitive : un Document `PLAYER_PRIVATE` peut être retrouvé via son association à un personnage (`characterId`) ou un accès invité (`guestAccessId`), mais seul son auteur, via ces mêmes chemins de résolution, est autorisé à le lire. Le rôle d'un personnage ou la simple association à une LIVE_NOTE n'ouvre aucune lecture — l'invariant de confidentialité reste absolu, quel que soit le chemin d'accès (direct, via personnage, ou via invité).
7. La suppression d'un `Folder` déplace ses Documents vers le dossier virtuel "Non classés" de l'espace, ou vers un autre dossier choisi par le MJ au moment de la suppression. Aucun Document n'est supprimé implicitement par la suppression de son dossier.
8. Un Document avec `isReusable = false` ne peut pas être instancié.
9. Une instance (`sourceDocumentId` renseigné) est totalement indépendante de son source après création.
10. La visibilité `PUBLIC` est permanente jusqu'à `Unshare()` explicite — un document partagé reste accessible aux joueurs entre les sessions.

---

## Règles métier

1. `visibility = PUBLIC` : le document est visible par tous les membres de l'espace et les GuestAccess actifs.
2. `visibility = GM_ONLY` : visible uniquement par les membres `OWNER` et `GM`.
3. `visibility = PLAYER_PRIVATE` : lisible uniquement par l'auteur du document — `createdById` pour un membre, `guestAccessId` pour un invité. Les membres `OWNER` et `GM` n'y ont aucun accès, quel que soit le type de document. Cette règle est encodée dans `Document.CanBeReadBy(userId, memberRole, documentType)` et non dans un service applicatif. La suppression en cascade (purge d'espace, ADR-011) n'est pas affectée — la confidentialité porte sur la lecture, pas sur la suppression administrative.
4. Partager un document (`Share()`) change sa visibilité de façon permanente. Ce n'est pas un partage temporaire de session — le joueur peut y accéder entre les séances.
5. L'instanciation d'un document réutilisable crée une copie profonde (blocs + liens + propriétés). Les modifications ultérieures du source n'affectent pas les instances. L'instance est créée avec `isReusable = false` : elle n'est pas elle-même marquée réutilisable à sa création. La réutilisabilité reste une propriété du document source dans l'espace `PERSONAL` du MJ (la bibliothèque personnelle est la vue filtrée `isReusable = true` de cet espace) ; une instance vivant dans un espace `CAMPAIGN`/`ONE_SHOT` n'a pas vocation à apparaître dans cette bibliothèque. Rien n'empêche un MJ de marquer explicitement une instance comme réutilisable par la suite, au même titre que tout document.
6. Les backlinks ne sont pas stockés — ils sont calculés à la lecture via `DocumentLink.targetDocumentId`.
7. Les types système (`isSystem = true`) ne peuvent pas être modifiés ni supprimés.
8. Un document `REVEAL` lié à une scène est créé avec `visibility = GM_ONLY` par défaut. Il passe à `PUBLIC` uniquement via une action explicite du MJ en session (→ UC-08 — partage d'information). Ce passage est permanent jusqu'à `Unshare()`.
9. Si un `Folder.defaultTemplateDocumentId` est défini, tout nouveau `Document` créé dans ce dossier est initialisé en appelant `Document.Instantiate()` sur le template. Si le template est supprimé, le champ passe à `null` — les documents existants ne sont pas affectés.
10. **Effacement physique sous obligation RGPD** : les Documents avec `visibility = PLAYER_PRIVATE` qui tombent sous une obligation d'effacement légal (suppression de compte utilisateur, fin définitive d'un `GuestAccess` non converti) sont supprimés **physiquement**, et non via le mécanisme `SoftDelete`. La suppression logique réversible ne constitue pas un effacement au sens de l'article 17 du RGPD — elle ne fait que masquer le contenu. Pour ces populations, la suppression est définitive et non réversible. Les Documents avec `visibility = PUBLIC` ou `GM_ONLY` ne sont pas concernés : ils restent attachés à l'espace sous intérêt légitime (continuité d'espace).
11. Un document `SCENE` peut être lié depuis plusieurs scénarios (cardinalité n↔n via `DocumentLink`, aucune contrainte d'unicité sur `targetDocumentId`) — le même mécanisme qu'un PNJ partagé entre plusieurs scènes. « Ajouter une scène » crée toujours une scène fraîche possédée (create+link) ; le partage multi-parent n'est atteignable que via « lier un document existant » (SCENE est un type cible éligible de ce chemin). Retirer une scène d'un scénario supprime le lien, pas le document — la scène survit pour les autres scénarios qui la référencent ; pas de suppression en cascade. Le backlink calculé « référencée par N scénarios » est surfacé pour signaler le partage. Une scène partagée au niveau template n'affecte pas les instances (`Document.Instantiate`, UC-13) — chaque instance reste indépendante.

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `DocumentCreated` | `Document.Create()` | Session Conduct (si créé à la volée en session) |
| `DocumentVisibilityChanged` | `Document.Share()` / `Unshare()` | Session Conduct (mise à jour de la vue joueur en temps réel) |
| `DocumentDeleted` | `Document.Delete()` | Session Conduct (retirer des documents épinglés si actif en session) |
| `DocumentLinked` | `Document.LinkDocument()` | — (backlinks calculés en lecture) |
| `DocumentUnlinked` | `Document.UnlinkDocument()` | — |
| `DocumentInstantiated` | `Document.Instantiate()` | Space Management si besoin |
| `FolderCreated` | `Folder.Create()` | — |
| `FolderDeleted` | `Folder.Delete()` | Session Conduct (retirer les documents du dossier supprimé si actifs en session) |

---

## Intégration avec les autres contextes

### Ce que Content Library reçoit

| Événement | Source | Action |
|---|---|---|
| `SpaceCreated` | Space Management | Pour `CAMPAIGN`/`ONE_SHOT` : créer les 4 dossiers système nommés (Personnages, Joueurs, Scénarios, Notes) + le dossier virtuel (Non classés). Pour `PERSONAL` : créer uniquement le dossier virtuel (Non classés). |
| `SpaceArchived` | Space Management | Passer tous les documents en lecture seule (soft-lock) |
| `SpaceUnarchived` | Space Management | Repasser tous les documents en écriture (lever le soft-lock) |

### Ce que Content Library publie

- `DocumentId`, `FolderId` comme identifiants de référence.
- Les événements listés ci-dessus.

### Utilisation par les autres contextes

| Contexte | Usage |
|---|---|
| Session Conduct | `DocumentId` pour les documents sélectionnés, `DocumentLink` pour construire la vue session, `visibility` pour filtrer ce que les joueurs voient |
| Space Management | Le personnage rattaché à un membre d'espace référence un Document de type `player_character` (référence directe inter-contextes — exception assumée, motivée par ADR-009) |

---

## Note sur la ScenarioLibrary (UC-13)

La réutilisabilité est portée par `Document.isReusable` + `Document.Instantiate` (Content Library) ;
la bibliothèque personnelle est une vue de l'espace `PERSONAL` (Space Management). Il n'existe pas
d'agrégat de pont `ScenarioLibrary`/`ScenarioLibraryEntry` (subsumé, ADR-018).

---

## Diagrammes

→ [Classes](diagrams/classes/content-library.md)
→ [MCD](diagrams/mcd/content-library.md)
→ [MLD](diagrams/mld/content-library.md)
→ [Flux](diagrams/flows/content-library.md)

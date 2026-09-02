# Session Conduct

> **Responsabilité** : gérer le déroulement d'une session de jeu.
> Cycle de vie de la session, tableau de bord MJ configurable, documents épinglés,
> notes de session, accès joueurs en temps réel.
>
> Ce contexte **consomme** ce que Content Library prépare.
> Il ne possède aucun contenu — il l'expose et l'orchestre pendant la séance.

---

## Ce que ce contexte fait

- Créer et piloter le cycle de vie d'une session (LIVE → CLOSED → ARCHIVED).
- Gérer la configuration du tableau de bord session par espace (SessionViewConfig).
- Gérer les documents épinglés pendant une session.
- Gérer les notes de session (Documents LIVE_NOTE) prises pendant et après la session.
- Autoriser l'accès des joueurs (authentifiés ou invités) à la vue session.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Contenu des documents, blocs, scénarios | Content Library |
| Droits d'accès espace, memberships, GuestAccess | Space Management |
| Comptes utilisateurs | Identity & Access |
| Partage permanent d'un document (visibility) | Content Library (`Document.Share()`) |
| Layout visuel des colonnes de la vue session | UI — pas une préoccupation du domaine |

---

## Machine d'états de Session

```
LIVE ──► CLOSED ──► ARCHIVED
```

| Statut | Description |
|---|---|
| `LIVE` | Session en cours — tableau de bord actif, partage temps réel |
| `CLOSED` | Session terminée — contenu éditable (résumé, notes de session rétroactives) |
| `ARCHIVED` | Lecture seule complète — aucune modification possible |

> Une session est créée directement en état `LIVE`. Il n'y a pas d'état `PLANNED` dans le MVP.
> Rouvrir une session CLOSED signifie éditer son contenu, pas changer son statut.

---

## Agrégats

### Session

> Une session n'existe que dans un **espace partagé** (`CAMPAIGN` ou `ONE_SHOT`).
> Un espace `PERSONAL` (mono-membre, sans joueurs) n'a pas de session — le partage temps réel,
> la vue joueur et les notes joueur supposent un groupe de jeu.

| Champ | Type | Description |
|---|---|---|
| `id` | `SessionId` | |
| `spaceId` | `SpaceId` | Espace partagé auquel appartient la session |
| `title` | `string` | |
| `status` | `SessionStatus` | `LIVE` \| `CLOSED` \| `ARCHIVED` |
| `scenarioId` | `DocumentId?` | Scénario joué — nullable (session improvisée possible) |
| `pinnedDocumentIds` | `DocumentId[]` | Documents épinglés pendant la séance |
| `sessionNoteIds` | `DocumentId[]` | Références vers les Documents LIVE_NOTE (notes de session) rattachés à cette session |
| `summary` | `string?` | Résumé — éditable en état CLOSED |
| `startedAt` | `DateTime` | |
| `closedAt` | `DateTime?` | |
| `AuditInfo` | | `createdAt`, `updatedAt`, `createdById: UserId` |

**Méthodes**

| Méthode | Événement produit | Condition |
|---|---|---|
| `Start(spaceId, title, scenarioId?)` | `SessionStarted` | — |
| `Close()` | `SessionClosed` | status = LIVE |
| `Archive()` | `SessionArchived` | status = CLOSED |
| `PinDocument(docId)` | `DocumentPinned` | status = LIVE ou CLOSED |
| `UnpinDocument(docId)` | `DocumentUnpinned` | status = LIVE ou CLOSED |
| `AttachNote(documentId)` | — | Rattache un Document LIVE_NOTE (note de session) à la session. status = LIVE ou CLOSED |
| `UpdateSummary(text)` | — | status = CLOSED |

---

### SessionViewConfig (agrégat)

Configuration du tableau de bord session au niveau de l'espace.
Définit quels dossiers le MJ met en avant dans sa vue session.
Un seul `SessionViewConfig` par espace.

> `SessionViewConfig` n'existe que pour les **espaces partagés** (`CAMPAIGN` ou `ONE_SHOT`).
> Un espace `PERSONAL` n'a pas de `SessionViewConfig` : il n'y a ni joueurs, ni vue session joueur.

Créé automatiquement à `SpaceCreated` (espaces partagés uniquement) avec les dossiers système de l'espace comme point de départ. Le MJ peut ensuite ajouter, retirer ou réordonner librement.

| Champ | Type | Description |
|---|---|---|
| `id` | `SessionViewConfigId` | |
| `spaceId` | `SpaceId` | Unique — un seul config par espace partagé |
| `focusedFolders` | `SessionViewFolder[]` | Dossiers mis en avant dans la vue session |
| `updatedAt` | `DateTime` | |

**Méthodes**

| Méthode | Description |
|---|---|
| `AddFolder(folderId, order)` | Ajoute un dossier aux panneaux de la vue session |
| `RemoveFolder(folderId)` | Retire un dossier des panneaux |
| `ReorderFolders(orderedFolderIds)` | Réordonne les dossiers |

---

### SessionViewFolder (value object dans SessionViewConfig)

| Champ | Type | Description |
|---|---|---|
| `folderId` | `FolderId` | Référence vers Content Library |
| `order` | `int` | Priorité d'affichage |

> Le rendu visuel (colonnes, onglets, accordéon…) est une décision UI.
> Le domaine expose uniquement une liste ordonnée de dossiers.

---

### Notes de session — Documents de type LIVE_NOTE

Les notes de session sont des **Documents** de Content Library avec `documentTypeId = LIVE_NOTE`.
La session référence leurs IDs dans `sessionNoteIds`.

Créer une note de session = deux opérations applicatives :
1. `Document.Create()` dans Content Library (type LIVE_NOTE, folder = "Notes" de l'espace)
2. `Session.AttachNote(documentId)`

Les métadonnées spécifiques aux notes de session sont portées par deux **champs de premier niveau** du Document, promus depuis les propriétés structurées (ADR-002) :

| Champ | Type | Description |
|---|---|---|
| `characterId` | `DocumentId?` | Personnage associé — pour les notes PLAYER_PRIVATE joueur |
| `guestAccessId` | `GuestAccessId?` | Auteur invité sans compte — quand l'auteur n'est pas un utilisateur enregistré |

> Ce modèle permet à une note de session de référencer d'autres documents via `linkedDocuments`
> (lier un PNJ, une scène, un lieu à la note) sans aucune modélisation supplémentaire.
> Voir Règles métier n°8 (suppression physique des notes privées à la suppression de compte).

---

## Vue session — philosophie

La vue session est un **tableau de bord configurable**. Le MJ choisit quels dossiers il met en avant selon ses besoins : certains veulent leurs PNJ, d'autres leurs lieux, d'autres les deux. Le système ne présuppose aucune organisation.

Ce modèle est cohérent avec les principes de Content Library :
- Les dossiers sont libres et renommables.
- Les types de documents sont optionnels.
- La structure d'un espace appartient au MJ, pas à l'application.

La `SessionViewConfig` est la traduction domaine de cette philosophie : elle mémorise les préférences du MJ sans imposer de structure.

---

## Invariants métier

1. La machine d'états est unidirectionnelle : LIVE → CLOSED → ARCHIVED.
2. Une session ARCHIVED refuse toute modification.
3. `AttachNote` et `UpdateSummary` sont autorisés en état CLOSED.
4. Tout Document avec `visibility = PLAYER_PRIVATE` n'est lisible que par son auteur : le membre identifié par `createdById`, ou l'invité identifié par `guestAccessId`. Les membres `OWNER` et `GM` n'y ont aucun accès, quel que soit le type de document — y compris les Documents de type `LIVE_NOTE`. Le lien vers le personnage (`characterId`) est un **lien d'affichage et de routage** : il permet d'associer la note à un personnage dans l'interface. Il ne confère aucune propriété sur la note et ne fait pas hériter la note à un futur joueur incarnant le même personnage. Un joueur ultérieur jouant le même personnage ne peut pas accéder aux notes `PLAYER_PRIVATE` d'un ancien joueur — même si le `characterId` est identique.
5. Il existe exactement un `SessionViewConfig` par espace partagé (`CAMPAIGN` ou `ONE_SHOT`). Un espace `PERSONAL` n'en a pas.
6. Un `SessionViewFolder` référence un dossier qui appartient au même espace.
7. `Session.summary` n'est modifiable que si `status = CLOSED`. Il peut être null. Il représente un résumé narratif libre rédigé par le MJ après la séance — distinct des notes de session (`sessionNoteIds`) qui sont des Documents LIVE_NOTE.
8. Il ne peut y avoir qu'une seule session au statut `LIVE` par espace. La création d'une nouvelle session en `LIVE` est bloquée si une session `LIVE` existe déjà pour l'espace. *(C-14)*

---

## Règles métier

1. Démarrer une session (`Start()`) est réservé aux membres `OWNER` ou `GM`.
2. Un joueur accède à la session via son `SpaceMembership` ou un `GuestAccess` actif — la validation est faite en couche application.
3. Un joueur ne voit que les documents avec `visibility = PUBLIC` et ses propres documents avec `visibility = PLAYER_PRIVATE` (dont les Documents de type `LIVE_NOTE`). Le MJ ne voit pas les documents `PLAYER_PRIVATE` dont il n'est pas l'auteur.
4. Épingler un document (`PinDocument`) n'en change pas la visibilité — c'est une organisation locale à la session.
5. Partager un document avec les joueurs (`Document.Share()`) est une opération Content Library déclenchée depuis la couche application — Session Conduct ne possède pas cette opération.
6. À la clôture de session (`Close()`), la couche application notifie Space Management pour déclencher le countdown d'expiration des `GuestAccess SESSION`.
7. La création à la volée (UC-07) crée un Document dans Content Library via la couche application. Le rattachement à la session dépend du type créé :
   - **Document durable** (note, PNJ, lieu, faction, personnage joueur, objet, contenu libre) : en séance LIVE, ce Document est **épinglé par défaut** dans la session (`Session.PinDocument()`) ; le MJ peut le désépingler ensuite (`Session.UnpinDocument()`, cf. règle 4). En séance CLOSED, l'épinglage n'est pas automatique — il reste optionnel, à la main du MJ.
   - **`LIVE_NOTE`** (note de session créée à la volée) : elle est **rattachée à la session via `Session.AttachNote()`** (`sessionNoteIds`) — jamais épinglée, quel que soit le statut de la session (cf. RB-07-04).
8. **Règle F-08 — Effacement effectif des documents privés sous obligation RGPD** (RGPD, article 17 — droit à l'effacement) : les Documents `PLAYER_PRIVATE` sous obligation d'effacement sont supprimés **physiquement** — pas via le mécanisme `SoftDelete` (suppression logique réversible). Pour ces populations, « supprimé » signifie « effacé », pas « masqué ». Cette portée n'est **pas restreinte au type `LIVE_NOTE`** : l'obligation d'effacement porte sur la confidentialité (`visibility = PLAYER_PRIVATE`), pas sur le type de document (cohérent avec content-library.md invariant 10 et identity-access.md règle métier n°4). Session Conduct manipule directement la population `LIVE_NOTE` via `sessionNoteIds` ; les autres types `PLAYER_PRIVATE` relèvent de Content Library. Deux déclencheurs :
   - **Suppression de compte utilisateur** : deux populations de Documents `PLAYER_PRIVATE` sont effacées : (a) ceux créés par cet utilisateur (`createdById`), et (b) ceux créés par cet utilisateur et rattachés aux personnages incarnés par l'utilisateur dans les espaces où il était membre. Cette extension couvre le risque de résidu : un personnage pouvant être réassocié à un autre joueur, un document `PLAYER_PRIVATE` résiduel serait exposé au nouveau propriétaire.
   - **Fin définitive d'un `GuestAccess` non converti** : les Documents `PLAYER_PRIVATE` créés par cet invité (`guestAccessId`) sont effacés physiquement — uniquement les siens.
   Les contenus partagés (`PUBLIC`, `GM_ONLY`) sont conservés sous intérêt légitime pour assurer la continuité de l'espace. La mise en œuvre de cette obligation est arbitrée par ADR-012 et ADR-013.

   > **Portée actée** : voir identity-access.md § Règles métier n°4 — UC-10 porte désormais cette même portée large, arbitrage rendu.

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `SessionStarted` | `Session.Start()` | Application (notification membres) |
| `SessionClosed` | `Session.Close()` | Space Management (expiration GuestAccess SESSION) |
| `SessionArchived` | `Session.Archive()` | — |
| `DocumentPinned` | `Session.PinDocument()` | Application (épinglage automatique UC-07, UC-08) |
| `DocumentUnpinned` | `Session.UnpinDocument()` | — |

---

## Intégration avec les autres contextes

### Ce que Session Conduct reçoit

| Événement / Requête | Source | Action |
|---|---|---|
| `SpaceCreated` | Space Management | Créer le `SessionViewConfig` avec les dossiers système (espaces partagés uniquement — `CAMPAIGN` ou `ONE_SHOT`) |
| `GuestAccessCreated` | Space Management | Autoriser l'entrée du joueur invité |
| `GuestAccessRevoked` | Space Management | Couper l'accès de l'invité si une session est en cours |
| `GuestAccessExpired` | Space Management | Couper l'accès de l'invité si une session est en cours |
| `MemberJoined` | Space Management | — |
| `MemberActivated` | Space Management | Autoriser l'accès du membre en session |
| `MemberRemoved` | Space Management | Invalider l'accès si session en cours |
| `DisplayNameUpdated` | Identity & Access | Mettre à jour l'affichage du nom dans la vue joueur |
| `DocumentDeleted` | Content Library | Retirer de `pinnedDocumentIds` si présent |
| `FolderDeleted` | Content Library | Retirer les documents du dossier supprimé de `pinnedDocumentIds` |
| `DocumentVisibilityChanged` | Content Library | Mettre à jour la vue joueur en temps réel |
| `SpaceArchived` | Space Management | *(aucune transition — convention actée ci-dessous)* |
| `SpaceUnarchived` | Space Management | *(aucune transition — même convention, ci-dessous)* |

> **Convention actée — pas de transition sur `SpaceArchived` ni `SpaceUnarchived`** : une session `CLOSED` reste `CLOSED` quel que soit le statut d'archivage de son espace, à l'aller comme au retour. Trois motifs :
> 1. L'invariant 1 rend la machine d'états de `Session` unidirectionnelle et pilotée par ses seules commandes (`LIVE → CLOSED → ARCHIVED`). Y injecter une transition déclenchée par un événement d'un autre bounded context romprait cet invariant, pour un gain nul.
> 2. L'archivage d'un espace est réversible (voir space-management.md), alors que `ARCHIVED` est ici un état terminal en lecture seule complète (invariant 2). Faire transiter la session sur un événement réversible créerait une asymétrie : l'espace peut redevenir actif, la session resterait figée sans retour possible.
> 3. Puisque `SpaceArchived` ne produit déjà aucune transition, il n'y a rien à défaire au désarchivage : `SpaceUnarchived` reste sans effet sur `Session`, pour le même motif que celui qui le rend sans effet à l'aller.
>
> État `LIVE` : déjà couvert en amont — la garde posée sur `Space.Archive()` (space-management.md) refuse l'archivage tant qu'une session `LIVE` subsiste sur l'espace. Seul l'état `CLOSED` était concerné par cette convention.

### Ce que Session Conduct publie

- `SessionClosed` → Space Management

### Utilisation par les autres contextes

Session Conduct ne publie pas d'identifiants consommés par d'autres contextes. Il est le contexte terminal de la chaîne.

---

## Diagrammes

→ [Classes](diagrams/classes/session-conduct.md)
→ [MCD](diagrams/mcd/session-conduct.md)
→ [MLD](diagrams/mld/session-conduct.md)
→ [Flux](diagrams/flows/session-conduct.md)

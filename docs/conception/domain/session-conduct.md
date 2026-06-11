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
- Gérer la configuration du tableau de bord session par campagne (SessionViewConfig).
- Gérer les documents épinglés pendant une session.
- Gérer les notes de session (Documents LIVE_NOTE) prises pendant et après la session.
- Autoriser l'accès des joueurs (authentifiés ou invités) à la vue session.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Contenu des documents, blocs, scénarios | Content Library |
| Droits d'accès campagne, memberships, GuestAccess | Campaign Management |
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

| Champ | Type | Description |
|---|---|---|
| `id` | `SessionId` | |
| `campaignId` | `CampaignId` | |
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
| `Start(campaignId, title, scenarioId?)` | `SessionStarted` | — |
| `Close()` | `SessionClosed` | status = LIVE |
| `Archive()` | `SessionArchived` | status = CLOSED |
| `PinDocument(docId)` | `DocumentPinned` | status = LIVE ou CLOSED |
| `UnpinDocument(docId)` | `DocumentUnpinned` | status = LIVE ou CLOSED |
| `AttachNote(documentId)` | — | Rattache un Document LIVE_NOTE (note de session) à la session. status = LIVE ou CLOSED |
| `UpdateSummary(text)` | — | status = CLOSED |

---

### SessionViewConfig (agrégat)

Configuration du tableau de bord session au niveau de la campagne.
Définit quels dossiers le MJ met en avant dans sa vue session.
Un seul `SessionViewConfig` par campagne.

Créé automatiquement à `CampaignCreated` avec les dossiers système de la campagne comme point de départ. Le MJ peut ensuite ajouter, retirer ou réordonner librement.

| Champ | Type | Description |
|---|---|---|
| `id` | `SessionViewConfigId` | |
| `campaignId` | `CampaignId` | Unique — un seul config par campagne |
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
1. `Document.Create()` dans Content Library (type LIVE_NOTE, folder = "Notes" de la campagne)
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
- La structure d'une campagne appartient au MJ, pas à l'application.

La `SessionViewConfig` est la traduction domaine de cette philosophie : elle mémorise les préférences du MJ sans imposer de structure.

---

## Invariants métier

1. La machine d'états est unidirectionnelle : LIVE → CLOSED → ARCHIVED.
2. Une session ARCHIVED refuse toute modification.
3. `AttachNote` et `UpdateSummary` sont autorisés en état CLOSED.
4. Tout Document avec `visibility = PLAYER_PRIVATE` n'est lisible que par son auteur (`createdById` ou `guestAccessId`). Les membres `OWNER` et `GM` n'y ont aucun accès, quel que soit le type de document — y compris les Documents de type `LIVE_NOTE`.
5. Il existe exactement un `SessionViewConfig` par campagne.
6. Un `SessionViewFolder` référence un dossier qui appartient à la même campagne.
7. `Session.summary` n'est modifiable que si `status = CLOSED`. Il peut être null. Il représente un résumé narratif libre rédigé par le MJ après la séance — distinct des notes de session (`sessionNoteIds`) qui sont des Documents LIVE_NOTE.
8. Il ne peut y avoir qu'une seule session au statut `LIVE` par campagne. La création d'une nouvelle session en `LIVE` est bloquée si une session `LIVE` existe déjà pour la campagne. *(C-14)*

---

## Règles métier

1. Démarrer une session (`Start()`) est réservé aux membres `OWNER` ou `GM`.
2. Un joueur accède à la session via son `CampaignMembership` ou un `GuestAccess` actif — la validation est faite en couche application.
3. Un joueur ne voit que les documents avec `visibility = PUBLIC` et ses propres documents avec `visibility = PLAYER_PRIVATE` (dont les Documents de type `LIVE_NOTE`). Le MJ ne voit pas les documents `PLAYER_PRIVATE` dont il n'est pas l'auteur.
4. Épingler un document (`PinDocument`) n'en change pas la visibilité — c'est une organisation locale à la session.
5. Partager un document avec les joueurs (`Document.Share()`) est une opération Content Library déclenchée depuis la couche application — Session Conduct ne possède pas cette opération.
6. À la clôture de session (`Close()`), la couche application notifie Campaign Management pour déclencher le countdown d'expiration des `GuestAccess SESSION`.
7. La création à la volée (UC-07) crée un Document dans Content Library via la couche application, puis le résultat est épinglé dans la session.
8. **Règle F-08 — Suppression physique des notes privées à la suppression de compte** (RGPD, article 17 — droit à l'effacement) : à la suppression d'un compte utilisateur, deux populations de Documents de type `LIVE_NOTE` avec `visibility = PLAYER_PRIVATE` sont supprimées physiquement : (a) ceux créés par cet utilisateur, et (b) ceux créés par cet utilisateur et rattachés aux personnages incarnés par l'utilisateur dans l'ensemble des campagnes vivantes où il était membre. Cette extension couvre le risque de résidu : un personnage pouvant être réassocié ultérieurement à un autre joueur, une note de session privée résiduelle rattachée à ce personnage serait exposée au nouveau propriétaire. Les contenus partagés (`PUBLIC`, `GM_ONLY`) sont conservés sous intérêt légitime pour assurer la continuité de campagne. La mise en œuvre de cette obligation est arbitrée par ADR-012.

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `SessionStarted` | `Session.Start()` | Application (notification membres) |
| `SessionClosed` | `Session.Close()` | Campaign Management (expiration GuestAccess SESSION) |
| `SessionArchived` | `Session.Archive()` | — |
| `DocumentPinned` | `Session.PinDocument()` | Application (épinglage automatique UC-07, UC-08) |
| `DocumentUnpinned` | `Session.UnpinDocument()` | — |

---

## Intégration avec les autres contextes

### Ce que Session Conduct reçoit

| Événement / Requête | Source | Action |
|---|---|---|
| `CampaignCreated` | Campaign Management | Créer le `SessionViewConfig` avec les dossiers système |
| `GuestAccessCreated` | Campaign Management | Autoriser l'entrée du joueur invité |
| `GuestAccessRevoked` | Campaign Management | Couper l'accès de l'invité si une session est en cours |
| `GuestAccessExpired` | Campaign Management | Couper l'accès de l'invité si une session est en cours |
| `MemberJoined` | Campaign Management | — |
| `MemberActivated` | Campaign Management | Autoriser l'accès du membre en session |
| `MemberRemoved` | Campaign Management | Invalider l'accès si session en cours |
| `DisplayNameUpdated` | Identity & Access | Mettre à jour l'affichage du nom dans la vue joueur |
| `DocumentDeleted` | Content Library | Retirer de `pinnedDocumentIds` si présent |
| `FolderDeleted` | Content Library | Retirer les documents du dossier supprimé de `pinnedDocumentIds` |
| `DocumentVisibilityChanged` | Content Library | Mettre à jour la vue joueur en temps réel |

### Ce que Session Conduct publie

- `SessionClosed` → Campaign Management

### Utilisation par les autres contextes

Session Conduct ne publie pas d'identifiants consommés par d'autres contextes. Il est le contexte terminal de la chaîne.

---

## Diagrammes

→ [Classes](diagrams/classes/session-conduct.md)
→ [MCD](diagrams/mcd/session-conduct.md)
→ [MLD](diagrams/mld/session-conduct.md)
→ [Flux](diagrams/flows/session-conduct.md)

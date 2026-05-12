# Bounded Context — Session Conduct

### Responsabilité

Préparation, conduite en temps réel et clôture des sessions de jeu.
Consomme les autres contextes par Id uniquement.

---

### Agrégat : `Session`

```
Session
├── id                : SessionId
├── campaignId        : CampaignId
├── scenarioId        : ScenarioId?      — null = session libre
├── title             : String
├── slug              : Slug
├── scheduledAt       : DateTime?
├── startedAt         : DateTime?
├── endedAt           : DateTime?
├── status            : SessionStatus
├── participantIds    : CharacterId[]    — références légères
├── selectedNpcIds    : NpcId[]          — PNJ sélectionnés pour la session
│                                          déduits automatiquement des scènes du scénario
│                                          modifiables manuellement par le MJ
├── pinnedItems       : PinnedItem[]     — value object avec ordre et date
├── liveNotes         : LiveNote[]       — entités enfants
├── summary           : SessionSummary? — entité enfant unique
├── audit             : AuditInfo
└── softDelete        : SoftDelete
```

```
SessionStatus
├── PLANNED
├── LIVE
├── CLOSED
└── ARCHIVED
```

#### Entité enfant : `LiveNote`

Note prise pendant ou après une session. Peut être créée par le MJ ou par un joueur.

```
LiveNote
├── id                : LiveNoteId
├── sessionId         : SessionId
├── content           : String
├── authorUserId      : UserId?         — renseigné si auteur authentifié
├── authorGuestAccessId : GuestAccessId? — renseigné si auteur invité
├── ownerCharacterId  : CharacterId?    — obligatoire pour PLAYER_PRIVATE
├── authorRole        : LiveNoteAuthorRole  — MJ ou PLAYER (détermine les règles de visibilité par défaut)
├── visibility        : Visibility      — PRIVATE (MJ), PLAYER_PRIVATE (joueur), ou SHARED/PUBLIC
│                                         jamais null — défaut selon authorRole
├── linkedDocumentId  : DocumentId?
└── audit             : AuditInfo       — pas de SoftDelete
```

```
LiveNoteAuthorRole
├── GM      — note créée par le MJ
└── PLAYER  — note créée par un joueur
```

**Visibilité par défaut selon authorRole** :
- `GM` → `PRIVATE` (note privée MJ par défaut, peut être partagée)
- `PLAYER` → `PLAYER_PRIVATE` (note personnelle joueur par défaut, invisible au MJ)

**Contraintes de visibilité par authorRole** :
- Une `LiveNote` avec `authorRole = GM` ne peut pas avoir `visibility = PLAYER_PRIVATE`.
- Une `LiveNote` avec `authorRole = PLAYER` ne peut pas avoir `visibility = PRIVATE`.
- Une `LiveNote` `PLAYER_PRIVATE` a toujours un `ownerCharacterId`.
- Si la note est créée par un invité, `authorGuestAccessId` trace l'accès utilisé,
  mais le droit de récupération futur repose sur `ownerCharacterId`.

#### Entité enfant : `SessionSummary`

```
SessionSummary
├── id          : SummaryId
├── sessionId   : SessionId
├── content     : String
├── visibility  : Visibility
├── ownerCharacterId : CharacterId? — null sauf extension future de résumé privé joueur
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Invariants et règles métier

- **Une seule session `LIVE` par campagne** à un instant donné — invariant fort.
- Transitions autorisées uniquement : `PLANNED → LIVE → CLOSED → ARCHIVED`.
- Aucun retour en arrière sur les transitions de statut.
- Les `LiveNote` de type MJ sont créées avec `visibility = PRIVATE` par défaut.
- Les `LiveNote` de type joueur sont créées avec `visibility = PLAYER_PRIVATE` par défaut
  et `ownerCharacterId = requester.characterId`.
- Un joueur ne peut créer des LiveNotes que pendant une session LIVE (pas PLANNED, pas a posteriori sur CLOSED).
  Le MJ peut créer des LiveNotes sur une session LIVE ou CLOSED (ajout rétroactif).
- Un `SessionSummary` `PRIVATE` est visible uniquement par le MJ.
- Les changements de visibilité de `Document`, `LiveNote` et `SessionSummary` s'appuient
  sur `AccessPolicy` et `ContentAccessRule`; il n'existe pas de mécanisme de partage parallèle.
- `participantIds`, `selectedNpcIds` et `pinnedItems` sont des références légères —
  si une entité référencée est supprimée, la référence est retirée.
- Soft delete `Session` → suppression physique des `LiveNote`, soft delete du `SessionSummary`.

**Permissions d'édition par statut** :

| Statut     | Métadonnées | Ajout LiveNote MJ | Ajout LiveNote Joueur | Édition SessionSummary | Épinglage |
|------------|-------------|-------------------|-----------------------|------------------------|-----------|
| `PLANNED`  | Oui         | Non               | Non                   | Non                    | Oui       |
| `LIVE`     | Oui         | Oui               | Oui                   | Non                    | Oui       |
| `CLOSED`   | Non         | Oui (rétro)       | Non                   | Oui                    | Non       |
| `ARCHIVED` | Non         | Non               | Non                   | Non — lecture seule    | Non       |

> Une session `CLOSED` reste éditable pour les notes MJ rétroactives et le résumé.
> Les joueurs ne peuvent plus créer de LiveNotes sur une session CLOSED.
> L'état `ARCHIVED` est le seul état véritablement immuable.

**Déduction automatique des `selectedNpcIds`** :
Quand un `scenarioId` est associé à une session, `SessionNpcSelector` (application service)
calcule la liste initiale depuis l'union des `linkedNpcIds` de toutes les scènes du scénario.
Le MJ peut ensuite ajouter ou retirer des NpcId manuellement.

#### Domain Events

```
SessionPlanned                  { sessionId, campaignId, scheduledAt, occurredAt }
SessionStarted                  { sessionId, campaignId, startedAt, occurredAt }
SessionClosed                   { sessionId, campaignId, endedAt, occurredAt }
SessionArchived                 { sessionId, campaignId, occurredAt }
SessionNpcSelected              { sessionId, npcId, occurredAt }
SessionNpcDeselected            { sessionId, npcId, occurredAt }
LiveNoteAdded                   { sessionId, liveNoteId, authorUserId?, authorGuestAccessId?, ownerCharacterId?, authorRole, occurredAt }
LiveNoteAddedPostSession        { sessionId, liveNoteId, authorUserId?, occurredAt }
LiveNoteRemoved                 { sessionId, liveNoteId, occurredAt }
LiveNoteVisibilityChanged       { sessionId, liveNoteId, oldVisibility, newVisibility, occurredAt }
DocumentPinnedToSession         { sessionId, documentId, occurredAt }
DocumentUnpinnedFromSession     { sessionId, documentId, occurredAt }
SessionSummaryCreated           { sessionId, summaryId, visibility, occurredAt }
SessionSummaryUpdated           { sessionId, summaryId, occurredAt }
SessionSummaryVisibilityChanged { sessionId, summaryId, oldVisibility, newVisibility, occurredAt }
```

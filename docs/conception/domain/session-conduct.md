# Bounded Context — Session Conduct

### Responsabilité

Préparation et conduite en temps réel des sessions de jeu.
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
├── selectedDocumentIds : DocumentId[]   — documents sélectionnés pour la session (PNJ, lieux…)
│                                          déduits automatiquement des linkedDocumentIds des scènes
│                                          modifiables manuellement par le MJ
├── pinnedItems       : PinnedItem[]     — value object avec ordre et date
├── liveNotes         : LiveNote[]       — entités enfants
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
├── id                  : LiveNoteId
├── sessionId           : SessionId
├── content             : String
├── authorUserId        : UserId?          — renseigné si auteur authentifié
├── authorGuestAccessId : GuestAccessId?   — renseigné si auteur invité
├── ownerCharacterId    : CharacterId?     — obligatoire pour PLAYER_PRIVATE
├── authorRole          : LiveNoteAuthorRole  — MJ ou PLAYER (détermine les règles de visibilité par défaut)
├── visibility          : Visibility       — PRIVATE (MJ), PLAYER_PRIVATE (joueur), ou SHARED/PUBLIC
│                                           jamais null — défaut selon authorRole
├── linkedDocumentId    : DocumentId?
└── audit               : AuditInfo        — pas de SoftDelete
```

```
LiveNoteAuthorRole
├── GM      — note créée par le MJ
└── PLAYER  — note créée par un joueur
```

**Visibilité par défaut selon authorRole** :
- `GM` → `PRIVATE` (note privée MJ par défaut, peut être partagée)
- `PLAYER` → `PLAYER_PRIVATE` (note personnelle joueur par défaut, invisible au MJ,
  partageable ensuite si le joueur décide de la montrer)

**Contraintes de visibilité par authorRole** :
- Une `LiveNote` avec `authorRole = GM` ne peut pas avoir `visibility = PLAYER_PRIVATE`.
- Une `LiveNote` avec `authorRole = PLAYER` ne peut pas avoir `visibility = PRIVATE`.
- Une `LiveNote` `PLAYER_PRIVATE` a toujours un `ownerCharacterId`.
- Si la note est créée par un invité, `authorGuestAccessId` trace l'accès utilisé,
  mais le droit de récupération futur repose sur `ownerCharacterId`.
- Pour une note invitée, `AuditInfo.createdById` est renseigné avec un auteur technique
  système/MJ ; `authorGuestAccessId` reste la vérité métier de l'auteur réel.

#### Invariants et règles métier

- **Une seule session `LIVE` par campagne** à un instant donné — invariant fort.
- Transitions autorisées uniquement : `PLANNED → LIVE → CLOSED → ARCHIVED`.
- Aucun retour en arrière sur les transitions de statut.
- Les `LiveNote` de type MJ sont créées avec `visibility = PRIVATE` par défaut.
- Les `LiveNote` de type joueur sont créées avec `visibility = PLAYER_PRIVATE` par défaut
  et `ownerCharacterId = requester.characterId`.
- Un joueur peut partager une LiveNote personnelle, comme il montrerait une note papier
  à la table. Le partage passe par `AccessPolicy` et `ContentAccessRule`.
- Un joueur ne peut créer des LiveNotes que pendant une session LIVE (pas PLANNED, pas a posteriori sur CLOSED).
  Le MJ peut créer des LiveNotes sur une session LIVE ou CLOSED (ajout rétroactif).
- Les récapitulatifs post-session sont des `Document(role = STANDARD)` de campagne,
  typés ou rangés selon l'organisation du MJ. Il n'existe pas d'entité dédiée de résumé dans le MVP.
- Les changements de visibilité de `Document` et `LiveNote` s'appuient
  sur `AccessPolicy` et `ContentAccessRule`; il n'existe pas de mécanisme de partage parallèle.
- `participantIds`, `selectedDocumentIds` et `pinnedItems` sont des références légères —
  si une entité référencée est supprimée, la référence est retirée.
- `scenarioId` doit référencer un `Scenario(isTemplate = false)` — un scénario source de bibliothèque
  ne peut pas être directement attaché à une session ; seule une instance de campagne le peut.
- Soft delete `Session` → suppression physique des `LiveNote`.

**Permissions d'édition par statut** :

| Statut     | Métadonnées | Ajout LiveNote MJ | Ajout LiveNote Joueur | Épinglage |
|------------|-------------|-------------------|-----------------------|-----------|
| `PLANNED`  | Oui         | Non               | Non                   | Oui       |
| `LIVE`     | Oui         | Oui               | Oui                   | Oui       |
| `CLOSED`   | Non         | Oui (rétro)       | Non                   | Non       |
| `ARCHIVED` | Non         | Non               | Non                   | Non       |

> Une session `CLOSED` reste éditable pour les notes MJ rétroactives.
> Les joueurs ne peuvent plus créer de LiveNotes sur une session CLOSED.
> L'état `ARCHIVED` est le seul état véritablement immuable.

**Déduction automatique des `selectedDocumentIds`** :
Quand un `scenarioId` est associé à une session, `SessionDocumentSelector` (application service)
calcule la liste initiale depuis l'union des `linkedDocumentIds` de toutes les scènes du scénario.
Le MJ peut ensuite ajouter ou retirer des DocumentId manuellement.

#### Domain Events

```
SessionPlanned                  { sessionId, campaignId, scheduledAt, occurredAt }
SessionStarted                  { sessionId, campaignId, startedAt, occurredAt }
SessionClosed                   { sessionId, campaignId, endedAt, occurredAt }
SessionArchived                 { sessionId, campaignId, occurredAt }
SessionDocumentSelected         { sessionId, documentId, occurredAt }
SessionDocumentDeselected       { sessionId, documentId, occurredAt }
LiveNoteAdded                   { sessionId, liveNoteId, authorUserId?, authorGuestAccessId?, ownerCharacterId?, authorRole, occurredAt }
LiveNoteAddedPostSession        { sessionId, liveNoteId, authorUserId?, occurredAt }
LiveNoteRemoved                 { sessionId, liveNoteId, occurredAt }
LiveNoteVisibilityChanged       { sessionId, liveNoteId, oldVisibility, newVisibility, occurredAt }
DocumentPinnedToSession         { sessionId, documentId, occurredAt }
DocumentUnpinnedFromSession     { sessionId, documentId, occurredAt }
```

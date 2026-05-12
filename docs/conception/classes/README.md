# Classes

Diagrammes de classes UML par bounded context, avec légende des relations.

## Légende des relations

| Notation | Signification |
|---|---|
| `*--` | Composition — cycle de vie lié, l'enfant ne vit pas sans le parent |
| `-->` | Association — dépendance directionnelle |
| `..>` | Référence légère — Id uniquement, cycle de vie indépendant |
| `<--` | Héritage / spécialisation |

## Fichiers

- [identity-access.md](identity-access.md) — Agrégat User et ses value objects
- [shared-kernel.md](shared-kernel.md) — Hiérarchie RequesterId (AuthenticatedRequesterId / GuestRequesterId)
- [campaign-management.md](campaign-management.md) — Campaign, CampaignMembership, Invitation, GuestAccess, GameSystem, AccessPolicy
- [content-library.md](content-library.md) — Document/blocs/tags, NPC, PlayerCharacter, Scenario, DocumentTemplate, Folder
- [session-conduct.md](session-conduct.md) — Session, LiveNote, SessionSummary, PinnedItem
- [context-map.md](context-map.md) — Vue globale des dépendances entre bounded contexts

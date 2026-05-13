# MCD — Récapitulatifs

## 6. Récapitulatif des tables

| Table | Contexte | Type | Soft delete |
|---|---|---|---|
| `USER` | Identity & Access | Aggregate root | Oui |
| `CAMPAIGN` | Campaign Management | Aggregate root | Oui |
| `CAMPAIGN_MEMBERSHIP` | Campaign Management | Entité enfant | Non |
| `INVITATION` | Campaign Management | Entité enfant | Non |
| `GUEST_ACCESS` | Campaign Management | Entité | Non |
| `GAME_SYSTEM` | Campaign Management | Aggregate root | Oui |
| `CONTENT_ACCESS_RULE` | Campaign Management | Entité (AccessPolicy) | Non — suppression physique |
| `DOCUMENT` | Content Library | Aggregate root | Oui |
| `DOCUMENT_BLOCK` | Content Library | Entité enfant | Non — suppression physique |
| `DOCUMENT_TEMPLATE` | Content Library | Aggregate root | Oui |
| `BLOCK_SCHEMA` | Content Library | Entité enfant | Non — suppression physique |
| `DOCUMENT_TYPE` | Content Library | Aggregate root | Oui |
| `PROPERTY_SCHEMA` | Content Library | Entité enfant | Non — suppression physique |
| `FOLDER` | Content Library | Aggregate root | Oui |
| `TAG` | Content Library | Entité (repository) | Oui |
| `DOCUMENT_TAG` | Content Library | Table de liaison | Non — suppression physique |
| `PLAYER_CHARACTER` | Content Library | Entité (repository) | Oui |
| `SCENARIO` | Content Library | Aggregate root | Oui |
| `SCENE` | Content Library | Entité enfant | Non — suppression en cascade |
| `SCENE_DOCUMENT` | Content Library | Table de liaison | Non — suppression physique |
| `SESSION` | Session Conduct | Aggregate root | Oui |
| `SESSION_PARTICIPANT` | Session Conduct | Table de liaison (PK composite) | Non — suppression physique |
| `SESSION_DOCUMENT` | Session Conduct | Table de liaison (PK composite) | Non — suppression physique |
| `PINNED_ITEM` | Session Conduct | Entité enfant (VO) | Non — suppression physique |
| `LIVE_NOTE` | Session Conduct | Entité enfant | Non — suppression physique |

---

## 7. Références cross-context — récapitulatif

Ces colonnes contiennent des UUID vers des entités d'un autre bounded context.
**Aucune FK en base** sur ces colonnes — la cohérence est garantie applicativement.

| Table | Colonne | Cible |
|---|---|---|
| `CAMPAIGN` | `ownerId` | `USER.id` (Identity) |
| `GUEST_ACCESS` | `characterId` | `PLAYER_CHARACTER.id` (Content Library) |
| `CONTENT_ACCESS_RULE` | `resourceId` | `DOCUMENT.id` ou `LIVE_NOTE.id` selon `resourceType` |
| `CONTENT_ACCESS_RULE` | `grantedByUserId` | `USER.id` si partage par utilisateur authentifié |
| `CONTENT_ACCESS_RULE` | `grantedByGuestAccessId` | `GUEST_ACCESS.id` si partage par invité |
| `INVITATION` | `sessionId` | `SESSION.id` si `scope = SESSION` |
| `GUEST_ACCESS` | `sessionId` | `SESSION.id` si `scope = SESSION` |
| `CONTENT_ACCESS_RULE` | `targetId` | `USER.id`, `PLAYER_CHARACTER.id` ou `SESSION.id` selon `targetType` |
| `DOCUMENT` | `campaignId` | `CAMPAIGN.id` (Campaign Management) — nullable si bibliothèque |
| `DOCUMENT` | `ownerCharacterId` | `PLAYER_CHARACTER.id` (même contexte) |
| `DOCUMENT_TEMPLATE` | `gameSystemId` | `GAME_SYSTEM.id` (Campaign Management) |
| `DOCUMENT_TYPE` | `campaignId` | `CAMPAIGN.id` (Campaign Management) — nullable selon scope |
| `FOLDER` | `campaignId` | `CAMPAIGN.id` (Campaign Management) |
| `TAG` | `campaignId` | `CAMPAIGN.id` (Campaign Management) |
| `PLAYER_CHARACTER` | `campaignId` | `CAMPAIGN.id` (Campaign Management) |
| `PLAYER_CHARACTER` | `ownerId` | `USER.id` (Identity) |
| `PLAYER_CHARACTER` | `linkedDocumentId` | `DOCUMENT.id` (même contexte, ref narrative) |
| `SCENARIO` | `campaignId` | `CAMPAIGN.id` (Campaign Management) — nullable si bibliothèque |
| `SCENARIO` | `ownerId` | `USER.id` (Identity) — non null si bibliothèque |
| `SCENARIO` | `sourceScenarioId` | `SCENARIO.id` (même contexte) |
| `SESSION` | `campaignId` | `CAMPAIGN.id` (Campaign Management) |
| `SESSION` | `scenarioId` | `SCENARIO.id` (Content Library) |
| `SESSION_PARTICIPANT` | `characterId` | `PLAYER_CHARACTER.id` (Content Library) |
| `SESSION_DOCUMENT` | `documentId` | `DOCUMENT.id` (Content Library) |
| `PINNED_ITEM` | `documentId` | `DOCUMENT.id` (Content Library) |
| `LIVE_NOTE` | `authorUserId` | `USER.id` (Identity) |
| `LIVE_NOTE` | `authorGuestAccessId` | `GUEST_ACCESS.id` (Campaign Management) |
| `LIVE_NOTE` | `ownerCharacterId` | `PLAYER_CHARACTER.id` (Content Library) |
| `LIVE_NOTE` | `linkedDocumentId` | `DOCUMENT.id` (Content Library) |

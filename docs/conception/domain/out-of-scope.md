# Hors périmètre MVP — points d'extension documentés

| Fonctionnalité | Point d'extension prévu | Contexte |
|---|---|---|
| Règles système de jeu | `GameSystem` prêt à recevoir un `RuleSet` | Campaign Management |
| Templates par système | `DocumentTemplate.gameSystemId` déjà modélisé | Content Library |
| Promotion NPC → PJ | Liens `linkedCharacterId` / `linkedNpcId` déjà présents | Content Library |
| Versioning des documents | Architecture compatible avec un historique de blocs | Content Library |
| OAuth / SSO | Contexte Identity isolé, remplacement sans impact | Identity & Access |
| Templates communautaires | `TemplateScope.USER` déjà modélisé | Content Library |
| AccessPolicy en contexte autonome | `ContentAccessRule` extractible si la complexité l'exige | Campaign Management |
| Temps réel (WebSocket) | `SessionStatus.LIVE` + domain events — base pour un bus événementiel | Session Conduct |
| SessionSummary deux versions | Modèle actuel : un seul résumé par session. Multi-version = post-MVP | Session Conduct |
| Types de document extensibles | `DocumentType.CUSTOM` + `customType: String` déjà modélisé | Content Library |
| Verrouillage de champs | `DocumentBlock.isLocked` modélisé, non activé dans le MVP | Content Library |
| Factions | Représentées via `Document(CUSTOM, "FACTION")` si nécessaire | Content Library |
| UserProjection locale | Si extraction de Campaign Management en service : ajouter projection via events | Campaign Management |
| Dossiers imbriqués (sous-dossiers) | `Folder.parentFolderId?` — non activé MVP, un seul niveau de dossiers | Content Library |
| Synchronisation template automatique | `Document.appliedTemplateVersion` modélisé — propagation auto non activée | Content Library |
| Dissociation ordre scénario / ordre dossier | `Scenario.followsFolderOrder: Boolean = true` — non activé MVP | Content Library |
| GameSystem custom partagé entre GM | `GameSystem.ownerId` = null pour les built-in, scope private en MVP | Campaign Management |

# Hors périmètre MVP — points d'extension documentés

| Fonctionnalité | Point d'extension prévu | Contexte |
|---|---|---|
| Règles système de jeu | `GameSystem` prêt à recevoir un `RuleSet` | Campaign Management |
| Templates par système | `DocumentTemplate.gameSystemId` déjà modélisé | Content Library |
| Promotion de document PNJ en Personnage joueur | `PlayerCharacter.linkedDocumentId` déjà présent ; factory à invoquer explicitement | Content Library |
| Versioning des documents | Architecture compatible avec un historique de blocs | Content Library |
| OAuth / SSO | Contexte Identity isolé, remplacement sans impact | Identity & Access |
| Templates communautaires | `TemplateScope.USER` déjà modélisé | Content Library |
| AccessPolicy en contexte autonome | `ContentAccessRule` extractible si la complexité l'exige | Campaign Management |
| Temps réel (WebSocket) | `SessionStatus.LIVE` + domain events — base pour un bus événementiel | Session Conduct |
| Résumé de session dédié | MVP couvert par un Document standard de campagne ; une entité spécialisée pourra être ajoutée si le workflow devient central | Session Conduct / Content Library |
| Relations typées entre documents | Fondation via `DocumentType.properties` et `RelationBlockValue`; ajout d'un type de propriété référence si le besoin est validé | Content Library |
| Verrouillage de champs | Non modélisé en MVP ; à ajouter quand l'édition joueur de fiches devient réellement fine | Content Library |
| Dossiers imbriqués (sous-dossiers) | `Folder.parentFolderId?` — non activé MVP, un seul niveau de dossiers | Content Library |
| Synchronisation template automatique | Les templates MVP sont des snapshots à la création ; pas de version/sync persistée | Content Library |
| Dissociation ordre scénario / ordre dossier | `Scenario.followsFolderOrder: Boolean = true` — non activé MVP | Content Library |
| GameSystem custom partagé entre MJ | `GameSystem.ownerId` = null pour les built-in, scope privé en MVP | Campaign Management |
| Moteur de règles léger | Fondation dans `DocumentType` et relations typées — voir UC-HORS-MVP | Content Library |

# ADR-002 — « Tout est Document » strict et gouvernance des identifiants et de `properties`

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : CR-4, A-03, B-02, B-04, C-03, C-04, C-06

---

## Contexte

Le pattern central « tout contenu éditorial est un `Document` composé de `DocumentBlock` » est retenu dans la conception. Cependant, l'audit a relevé trois problèmes non résolus qui rendaient ce pattern incohérent en pratique.

**Identifiants contradictoires.** Les diagrammes de classes utilisaient des types forts `ScenarioId`, `SceneId`, `CharacterId`, alors que la prose et d'autres parties de la conception utilisaient `DocumentId` pour les mêmes entités. Ces trois types d'ID ne correspondaient à aucune entité distincte — ils étaient des artefacts résiduels d'une modélisation antérieure à « tout est Document ».

**Champ `properties` non gouverné.** Le champ `properties` (JSON libre) des `Document` ne faisait l'objet d'aucune validation. Or ce champ porte des données à enjeu : la visibilité des notes (`PLAYER_PRIVATE`, `SHARED`) qui a des implications RGPD, et des métadonnées métier propres à chaque type de document. Un EAV (Entity–Attribute–Value) non gouverné dans ce contexte est un risque d'intégrité.

---

## Décision

**Un seul identifiant `DocumentId`.** Les value objects `ScenarioId`, `SceneId`, `CharacterId` sont supprimés. Des alias sémantiques de type (par exemple `CharacterRef = DocumentId`) sont autorisés pour la lisibilité dans le code, sans créer de type distinct au sens DDD.

**Un value object `DocumentProperties` valide `properties` à l'écriture.** La méthode `Document.SetProperties()` accepte uniquement un `DocumentProperties` validé contre le `propertiesSchema` du type de document concerné. Les écritures directes dans le JSON sans passer par ce VO sont interdites.

**Pour le MVP, les types de document système ont un schéma figé seedé en base.** Pas de `properties` libre non validé au démarrage : chaque type de document système (Scénario, Personnage, Note, etc.) dispose d'un schéma déclaré qui contraint les valeurs acceptables.

---

## Alternatives considérées

**Réintroduire des entités de premier ordre (`Scenario`, `Character` comme agrégats avec leurs propres IDs distincts de `DocumentId`).**
Écartée. Cette option revient sur la décision « tout est Document » adoptée après réflexion lors d'une itération précédente. Elle ajoute des tables, des migrations, des relations supplémentaires, et de la complexité de mapping EF Core sans apporter de bénéfice fonctionnel au MVP.

**Conserver « tout est Document » strict mais sans value object de validation pour le MVP.**
Écartée. Elle laisse l'EAV non gouverné sur des données à enjeu RGPD (visibilité des notes) et sur des invariants métier. Le risque d'incohérence silencieuse est trop élevé pour être différé.

---

## Conséquences

- Les diagrammes de classes du Core et les trois fichiers où `CharacterId`, `ScenarioId`, `SceneId` subsistent doivent être nettoyés et alignés sur `DocumentId` (Vague 1, finding A-03, B-04, C-06).
- L'EAV libre devient un EAV gouverné : toute écriture dans `properties` passe par `Document.SetProperties()` avec validation contre le schéma.
- La recherche full-text sur `properties` n'est pas indexée au MVP — la recherche reste titre-seul (finding CR-5). Cette limite est assumée.
- La migration future vers des relations typées (post-MVP, si le besoin émerge) est sécurisée par la présence d'un schéma déclaré par type.
- La conception du VO `DocumentProperties` et des schémas seedés est un livrable de la phase de cadrage (Vague 2, finding C-03).

---

## Compléments post-revue (2026-06-09)

Suite à la revue adversariale (revue Vague 0, artefact purgé du corpus — historique git), cette décision est complétée comme suit, sans changer sa direction.

- **Changement de signature d'agrégat.** La suppression de `CharacterId` n'est pas un nettoyage cosmétique : elle modifie la signature de `CampaignMembership.characterIds`, de `GuestAccess.characterId`, de la méthode `AssociateCharacter`, des événements associés et de l'invariant RB-11-18. Les diagrammes `classes/core.md`, `classes/campaign-management.md` et la prose `campaign-management.md` sont également affectés — environ 6 emplacements distincts, pas 3.

- **Validation runtime ajoutée.** Puisque le type fort disparaît, une validation runtime est requise : le `DocumentId` associé comme personnage doit référencer un `Document` de type `player_character`. Cette vérification est à intégrer dans l'invariant de domaine de `CampaignMembership`.

- **Promotion en colonnes.** Les champs `characterId` et `guestAccessId` des `LIVE_NOTE` passent de `documents.properties` (jsonb) à des colonnes nullable indexées de premier niveau sur la table `documents`. Ce changement débloque l'invariant d'autorisation décrit dans ADR-007 et résout le finding C-04.

- **`DocumentBlock.content` : libre par décision.** Le champ `content` est du contenu éditeur, sans enjeu d'intégrité référentielle. Ce choix est explicite et distinct de `properties` (gouverné par `DocumentProperties`).

- **`propertiesSchema` figé.** Le schéma figé s'applique aux types système non modifiés. La validation est permissive par défaut pour les types custom ou modifiés — choix délibéré pour ne pas bloquer l'extensibilité.

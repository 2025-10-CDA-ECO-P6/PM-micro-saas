# UC-13 — Utiliser un scénario réutilisable (one-shot)

> **Should Have — hors première livraison (post-MVP)** — Ce cas d'utilisation, ainsi que le parcours express one-shot qu'il conditionne (UC-02 §A1), sont reportés après la première livraison. Voir vision §5bis. En MVP, le one-shot se crée via le parcours campagne nominal (UC-02, scénario nominal, `type = ONE_SHOT`).

> **Note de subsomption (ADR-018)** — L'espace personnel (`SpaceType.PERSONAL`) subsume le rôle de la `ScenarioLibrary` telle qu'elle était modélisée dans le domaine Space Management. Un scénario réutilisable est désormais un `Document` de l'espace personnel du MJ avec `isReusable = true`. L'entité `ScenarioLibraryEntry` (pont compte↔espace) devient largement superflue dans ce modèle : la bibliothèque personnelle **est** l'espace personnel lui-même. Voir ADR-018 §ScenarioLibrary, UC-13, UC-F07.

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre à un MJ de créer un scénario une fois et de le rejouer avec des groupes de joueurs différents, en conservant le contenu original intact et en traçant les variantes entre les runs.

## Contexte

Sonia (31 ans, animatrice) organise des one-shots exclusivement : 3 à 5 parties par mois, avec des joueurs différents à chaque fois. Elle a un catalogue de 15 scénarios qu'elle rejoue. Son unité de travail n'est pas la campagne — c'est le scénario. Elle ne veut pas "créer une campagne pour chaque run" ; elle veut "lancer ce scénario ce soir".

Le scénario réutilisable répond aussi à Antoine (3 campagnes simultanées), qui veut des structures réutilisables entre espaces, et à tout MJ qui anime le même module pour plusieurs groupes.

**Bibliothèque personnelle** : les scénarios réutilisables sont des `Document` de l'**espace personnel** du MJ (`SpaceType.PERSONAL`), marqués `isReusable = true`. La bibliothèque personnelle n'est pas une entité à part — c'est l'espace personnel lui-même. Elle est visible depuis le tableau de bord et accessible depuis le parcours "Lancer un one-shot" (→ UC-02 A1).

## Déclencheur

Le MJ veut lancer un scénario existant avec un nouveau groupe, ou veut marquer un scénario comme réutilisable dans son catalogue.

## Préconditions

- Le MJ utilise l'application en mode local ou avec un compte cloud.
- L'espace personnel du MJ existe — il est créé automatiquement à la création du compte. En mode local sans compte, les scénarios réutilisables (`isReusable = true`) peuvent exister dans le conteneur local, mais la synchronisation inter-espaces n'est pas disponible.
- Un scénario source existe dans l'espace personnel du MJ avec `isReusable = true`, ou un scénario existant dans un espace peut être marqué réutilisable et transféré.

## Scénario nominal — Rejouer un scénario depuis la bibliothèque

1. Le MJ accède à sa bibliothèque (tableau de bord → "Mes scénarios"), qui reflète les documents de son espace personnel marqués `isReusable = true`.
2. Il sélectionne un scénario et choisit "Rejouer".
3. L'application propose deux contextes :
   - **One-shot ce soir** : crée un espace one-shot minimal et lance directement la session.
   - **Ajouter à une campagne** : instancie le scénario dans un espace de type `CAMPAIGN` existant.
4. L'application crée une **instance** du scénario via `Document.Instantiate(spaceId, folderId)` :
   - Le contenu source (documents, PNJ, structure) est copié dans l'espace cible (one-shot ou campagne).
   - L'instance vit dans l'espace de jeu partagé (espace cible).
   - Le scénario source reste dans l'espace personnel du MJ, intact et non modifié.
5. Le MJ adapte les détails de l'instance si nécessaire (noms, variantes).
6. Il lance la session depuis cette instance.
7. Après la session, les notes et modifications de l'instance n'affectent pas le scénario source dans l'espace personnel.

## Scénarios alternatifs

### A1 — Marquer un scénario comme réutilisable

1. Le MJ a un scénario existant dans un espace (campagne ou one-shot).
2. Il choisit "Marquer comme réutilisable" : le scénario est déplacé ou copié vers son espace personnel et marqué `isReusable = true`.
3. Le scénario apparaît dans sa bibliothèque personnelle (vue filtrée sur l'espace personnel, `isReusable = true`).
4. Il peut désormais créer des instances depuis ce scénario sans toucher à l'original dans l'espace personnel.

> **Note** : l'ancienne formulation — « promotion en entrée d'index `ScenarioLibraryEntry` » — est remplacée par ce mécanisme. Le déplacement ou la copie vers l'espace personnel, avec `isReusable = true`, remplit le même rôle sans entité de pont intermédiaire.

### A2 — Consulter l'historique des runs

1. Le MJ sélectionne un scénario dans sa bibliothèque personnelle.
2. Il voit la liste des instances passées avec la date, le groupe de joueurs, et les notes MJ de chaque run.
3. Il peut consulter une instance passée en lecture seule.

### A3 — Espace one-shot léger

1. Le MJ veut lancer un scénario sans créer un espace complet de type campagne.
2. Il crée un espace de type `ONE_SHOT` : nom minimal, durée prévue (une session), pas de continuité.
3. L'instance du scénario est créée dans cet espace one-shot via `Document.Instantiate`.
4. Après la session, le MJ peut archiver manuellement l'espace one-shot, comme il le ferait pour une campagne (→ UC-02).

## Exceptions

### E1 — Copie profonde échouée

Si la duplication du scénario source vers une instance échoue (erreur système, timeout), l'opération est annulée dans sa totalité. Aucune instance partielle n'est créée. Le MJ reçoit un message d'erreur et peut réessayer.

### E2 — Scénario source supprimé après création d'instances

Les instances existantes restent valides et complètes — elles sont indépendantes du source depuis leur création. La suppression du source dans l'espace personnel n'affecte pas les instances déjà créées dans les espaces cibles.

### E3 — Espace cible archivé

Si le MJ tente d'instancier un scénario dans un espace archivé, l'opération est refusée. Le système propose de choisir un espace actif ou de créer un one-shot.

## Postconditions

- Une instance du scénario existe dans l'espace cible et porte ses propres notes et modifications.
- Le scénario source reste intact dans l'espace personnel du MJ.
- L'historique des runs est accessible depuis la bibliothèque personnelle.

## Données manipulées

### Scénario source (espace personnel)

- `Document` de l'espace personnel du MJ (`SpaceType.PERSONAL`)
- `isReusable = true`
- Titre
- Propriétaire (`ownerId` de l'espace personnel → compte MJ)
- Statut : actif, archivé
- Scènes et documents associés (structure narrative)

> **Note de migration** : dans le modèle antérieur à ADR-018, le scénario source était représenté comme une entrée `ScenarioLibraryEntry` dans la `ScenarioLibrary` (Space Management). Cette entité de pont est désormais largement superflue — l'espace personnel joue ce rôle sans couche d'indirection supplémentaire.

### Instance

- `Document` créé par `Document.Instantiate(spaceId, folderId)` dans l'espace cible
- `SpaceId` de l'espace cible (type `CAMPAIGN` ou `ONE_SHOT`)
- Copie complète du contenu source au moment de l'instanciation
- Notes et modifications propres à ce run
- Aucun lien vivant vers le source après création

### Entrée d'historique (run)

- Date de la session
- Identifiant de l'instance
- Notes MJ du run

---

## Décision d'architecture — Statut source vs instance

**Décision** : une instance est une **copie indépendante** du scénario source au moment de son instanciation (`Document.Instantiate`). Le scénario source (dans l'espace personnel) et l'instance (dans l'espace cible) n'ont aucun lien vivant après la création de l'instance.

**Justification** : aligne avec le principe de zéro friction et de prévisibilité pour Sonia. Elle modifie son run sans risquer d'altérer le source. Le source reste sa "version propre" du scénario, intacte pour les prochains groupes. La cohérence entre le source et les instances n'est pas gérée automatiquement — c'est le MJ qui décide de promouvoir à nouveau s'il améliore le source.

**Conséquence** : pas de mécanisme de diff ou de synchronisation source → instance dans le MVP. Cette feature (UC-13 stories exclues) est Could Have post-MVP.

---

## Règles métier

- Un scénario source ne peut pas être modifié depuis une instance (les modifications vont dans l'instance, pas dans le source).
- Une instance est toujours liée à un espace cible (type `CAMPAIGN` ou `ONE_SHOT`) — elle n'existe pas "libre" en dehors d'un espace.
- Le catalogue de scénarios réutilisables est propre au MJ (espace personnel mono-membre — pas de partage entre MJ dans le MVP).
- La création d'une instance est une copie profonde : le scénario, ses scènes, ses documents liés et les documents typés PNJ/lieu/objet nécessaires sont dupliqués dans l'espace cible.

## Critères d'acceptation

- Un MJ peut marquer un scénario comme réutilisable (le déplacer ou le copier vers son espace personnel avec `isReusable = true`).
- Il peut créer une instance de ce scénario dans un espace cible sans modifier le source dans l'espace personnel.
- L'historique des instances (runs passés) est visible depuis la bibliothèque personnelle.
- *(post-MVP — conditionné au parcours express UC-02 §A1)* Un espace one-shot peut être créé en moins de 30 secondes via le point d'entrée dédié (nom + scénario depuis la bibliothèque).
- Modifier une instance n'affecte pas le scénario source dans l'espace personnel.

## Questions à valider en interview

- Sonia trace-t-elle vraiment les variantes entre runs, ou se contente-t-elle de relancer sans notes ?
- Le "catalogue de scénarios" est-il mieux organisé dans une vue dédiée de l'espace personnel ou au niveau du dashboard ?
- Faut-il un mécanisme de "diff" entre instance et source pour savoir ce qui a été changé ?

# UC-13 — Utiliser un scénario réutilisable (one-shot)

> **Should Have — hors première livraison (post-MVP)** — Ce cas d'utilisation, ainsi que le parcours express one-shot qu'il conditionne (UC-02 §A1), sont reportés après la première livraison. Voir vision §5bis. En MVP, le one-shot se crée via le parcours campagne nominal (UC-02, scénario nominal, `type = ONE_SHOT`).

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre à un MJ de créer un scénario une fois et de le rejouer avec des groupes de joueurs différents, en conservant le contenu original intact et en traçant les variantes entre les runs.

## Contexte

Sonia (31 ans, animatrice) organise des one-shots exclusivement : 3 à 5 parties par mois, avec des joueurs différents à chaque fois. Elle a un catalogue de 15 scénarios qu'elle rejoue. Son unité de travail n'est pas la campagne — c'est le scénario. Elle ne veut pas "créer une campagne pour chaque run" ; elle veut "lancer ce scénario ce soir".

Le scénario réutilisable répond aussi à Antoine (3 campagnes simultanées), qui veut des structures réutilisables entre campagnes, et à tout MJ qui anime le même module pour plusieurs groupes.

**Bibliothèque personnelle** : les scénarios réutilisables existent au niveau du compte MJ,
pas à l'intérieur d'une campagne. La bibliothèque est visible depuis le tableau de bord
et accessible depuis le parcours "Lancer un one-shot" (→ UC-02 A1).

## Déclencheur

Le MJ veut lancer un scénario existant avec un nouveau groupe, ou veut marquer un scénario comme réutilisable dans son catalogue.

## Préconditions

- Le MJ utilise l'application en mode local ou avec un compte cloud.
- La bibliothèque de scénarios réutilisables (`ScenarioLibrary`) est rattachée au compte MJ — elle n'est disponible qu'avec un compte cloud. En mode local sans compte, cette fonctionnalité n'est pas accessible.
- Un scénario existe dans une campagne ou en bibliothèque.

## Scénario nominal — Rejouer un scénario depuis la bibliothèque

1. Le MJ accède à sa bibliothèque (tableau de bord → "Mes scénarios").
2. Il sélectionne un scénario et choisit "Rejouer".
3. L'application propose deux contextes :
   - **One-shot ce soir** : crée un espace one-shot minimal et lance directement la session.
   - **Ajouter à une campagne** : instancie le scénario dans une campagne existante.
4. L'application crée une **instance** du scénario :
   - Le contenu source (documents, PNJ, structure) est copié dans la nouvelle instance.
   - Le scénario source reste intact et non modifié.
5. Le MJ adapte les détails de l'instance si nécessaire (noms, variantes).
6. Il lance la session depuis cette instance.
7. Après la session, les notes et modifications de l'instance n'affectent pas le scénario source.

## Scénarios alternatifs

### A1 — Marquer un scénario comme template réutilisable

1. Le MJ a un scénario existant dans une campagne.
2. Il choisit "Marquer comme réutilisable".
3. Le scénario apparaît dans son catalogue de templates personnels.
4. Il peut désormais créer des instances depuis ce scénario sans toucher à l'original.

### A2 — Consulter l'historique des runs

1. Le MJ sélectionne un scénario dans son catalogue.
2. Il voit la liste des instances passées avec la date, le groupe de joueurs, et les notes MJ de chaque run.
3. Il peut consulter une instance passée en lecture seule.

### A3 — Campagne one-shot légère

1. Le MJ veut lancer un scénario sans créer une campagne complète.
2. Il crée une "campagne one-shot" : nom minimal, durée prévue (une session), pas de continuité.
3. L'instance du scénario est liée à cette campagne one-shot.
4. Après la session, la campagne one-shot est archivée automatiquement.

## Exceptions

### E1 — Copie profonde échouée

Si la duplication du scénario source vers une instance échoue (erreur système, timeout), l'opération est annulée dans sa totalité. Aucune instance partielle n'est créée. Le MJ reçoit un message d'erreur et peut réessayer.

### E2 — Scénario source supprimé après création d'instances

Les instances existantes restent valides et complètes — elles sont indépendantes du source depuis leur création. La suppression du source n'affecte pas les instances.

### E3 — Campagne cible archivée

Si le MJ tente d'instancier un scénario dans une campagne archivée, l'opération est refusée. Le système propose de choisir une campagne active ou de créer un one-shot.

## Postconditions

- Une instance du scénario existe et porte ses propres notes et modifications.
- Le scénario source reste intact.
- L'historique des runs est accessible depuis le catalogue.

## Données manipulées

### Scénario source (ScenarioLibrary)

- Identifiant unique
- Titre
- Propriétaire (compte MJ)
- Statut : actif, archivé
- Scènes et documents associés (structure narrative)

### Instance

- Identifiant unique
- Référence au scénario source (lecture seule)
- Contexte d'exécution : campagne ou one-shot
- Copie complète du contenu source au moment de l'instanciation
- Notes et modifications propres à ce run

### Entrée d'historique (run)

- Date de la session
- Identifiant de l'instance
- Notes MJ du run

---

## Décision d'architecture — Statut source vs instance

**Décision** : une instance est une **copie indépendante** du scénario source au moment de son instanciation. Le scénario source et l'instance n'ont aucun lien vivant après la création de l'instance.

**Justification** : aligne avec le principe de zéro friction et de prévisibilité pour Sonia. Elle modifie son run sans risquer d'altérer le source. Le source reste sa "version propre" du scénario, intacte pour les prochains groupes. La cohérence entre le source et les instances n'est pas gérée automatiquement — c'est le MJ qui décide de promouvoir à nouveau s'il améliore le source.

**Conséquence** : pas de mécanisme de diff ou de synchronisation source → instance dans le MVP. Cette feature (UC-13 stories exclues) est Could Have post-MVP.

---

## Règles métier

- Un scénario source ne peut pas être modifié depuis une instance (les modifications vont dans l'instance, pas dans le source).
- Une instance est toujours liée à un contexte (campagne ou one-shot) — elle n'existe pas "libre".
- Le catalogue de scénarios réutilisables est propre au MJ (pas de partage entre MJ dans le MVP).
- La création d'une instance est une copie profonde : le scénario, ses scènes, ses documents liés
  et les documents typés PNJ/lieu/objet nécessaires sont dupliqués dans le contexte cible.

## Critères d'acceptation

- Un MJ peut marquer un scénario comme réutilisable.
- Il peut créer une instance de ce scénario sans modifier le source.
- L'historique des instances (runs passés) est visible depuis le catalogue.
- *(post-MVP — conditionné au parcours express UC-02 §A1)* Une campagne one-shot peut être créée en moins de 30 secondes via le point d'entrée dédié (nom + scénario depuis la bibliothèque).
- Modifier une instance n'affecte pas le scénario source.

## Questions à valider en interview

- Sonia trace-t-elle vraiment les variantes entre runs, ou se contente-t-elle de relancer sans notes ?
- Le "catalogue de scénarios" est-il mieux rangé dans une campagne dédiée ("Ma bibliothèque") ou au niveau du dashboard ?
- Faut-il un mécanisme de "diff" entre instance et source pour savoir ce qui a été changé ?

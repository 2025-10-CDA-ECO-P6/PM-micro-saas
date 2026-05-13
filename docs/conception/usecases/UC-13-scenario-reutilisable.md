# UC-13 — Utiliser un scénario réutilisable (one-shot)

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

- Le MJ a un compte (local ou cloud).
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

## Postconditions

- Une instance du scénario existe et porte ses propres notes et modifications.
- Le scénario source reste intact.
- L'historique des runs est accessible depuis le catalogue.

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
- Une campagne "one-shot" peut être créée en moins de 30 secondes (nom + scénario).
- Modifier une instance n'affecte pas le scénario source.

## Questions à valider en interview

- Sonia trace-t-elle vraiment les variantes entre runs, ou se contente-t-elle de relancer sans notes ?
- Le "catalogue de scénarios" est-il mieux rangé dans une campagne dédiée ("Ma bibliothèque") ou au niveau du dashboard ?
- Faut-il un mécanisme de "diff" entre instance et source pour savoir ce qui a été changé ?

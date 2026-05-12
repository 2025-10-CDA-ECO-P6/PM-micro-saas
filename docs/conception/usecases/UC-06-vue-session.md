# UC-06 — Utiliser la vue session

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si le MJ partage des informations pendant la session.

## Objectif

Permettre au MJ d'accéder rapidement aux informations importantes pendant une partie.

## Contexte

Pendant une session, le MJ doit maintenir le rythme. Il peut devoir retrouver un PNJ, une note, une scène, une fiche personnage ou une information de campagne. Si l'information est trop difficile à trouver, la partie ralentit.

## Besoin utilisateur

Le MJ veut piloter sa session avec une interface simple et rapide, sans devoir fouiller dans tous ses documents.

## Déclencheur

La partie commence.

## Préconditions

- Une campagne existe.
- Une session existe ou peut être lancée rapidement.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il sélectionne une session prévue.
3. Il clique sur "Lancer la session".
4. Le système affiche une vue session simplifiée.
5. La vue session présente :
   - scénario actif ;
   - scènes prévues ;
   - PNJ importants ;
   - personnages joueurs ;
   - notes utiles ;
   - zone de note rapide ;
   - barre de recherche.

6. Le MJ consulte les éléments nécessaires pendant la partie.
7. Le MJ ajoute des notes rapides au fil de la session.
8. Le MJ marque éventuellement certaines scènes comme jouées.
9. Le MJ peut partager une information aux joueurs.
10. En fin de partie, le MJ sauvegarde ou clôture la session.

## Scénarios alternatifs

### A1 — Lancement sans session préparée

Le MJ lance une session rapide directement depuis une campagne.

### A2 — Création d'un élément en session

Le MJ crée rapidement un PNJ ou une note pendant la session.

### A3 — Recherche d'un élément non prévu

Le MJ utilise la recherche pour ouvrir un élément qui n'avait pas été préparé pour la session.

### A4 — Partage en direct

Le MJ rend une note ou une information visible aux joueurs pendant la partie.

## Exceptions

### E1 — Problème de sauvegarde

Si une note ne peut pas être sauvegardée, le système avertit le MJ et conserve localement le contenu si possible.

### E2 — Accès interdit

Un utilisateur non MJ tente d'accéder à la vue session MJ. Le système refuse l'accès.

## Postconditions

- Les notes prises pendant la session sont sauvegardées.
- Les éléments créés sont liés à la campagne.
- Les scènes marquées comme jouées conservent leur statut.
- La session peut être clôturée ou reprise plus tard.

## Données manipulées

- Session
- Scénario
- Scène
- PNJ
- Personnage joueur
- Note
- Information partagée

## Règles métier

- La vue session est réservée au MJ.
- Les notes rapides créées depuis la vue session sont automatiquement liées à la session en cours.
- Les éléments privés restent invisibles pour les joueurs.
- La vue session doit prioriser la rapidité d'accès à l'information plutôt que l'édition avancée.
- Les PNJ affichés dans la vue session correspondent à `Session.selectedNpcIds` : liste auto-déduite depuis les scènes du scénario actif, modifiable manuellement par le MJ (voir UC-05 A3).
- Les notes PLAYER_PRIVATE d'un joueur sont invisibles au MJ ; seul le joueur créateur peut les consulter.

## Critères d'acceptation

- Le MJ peut lancer une vue session.
- Le MJ peut consulter le scénario actif.
- Le MJ peut consulter les PNJ de `selectedNpcIds`.
- Le MJ peut consulter les fiches personnages.
- Le MJ peut créer une note rapide liée automatiquement à la session.
- Le MJ peut rechercher une information.
- Les notes créées en session sont sauvegardées.
- Les notes PLAYER_PRIVATE ne sont pas visibles dans la vue MJ.

## Questions à valider en interview

- Quelles informations les MJ cherchent-ils le plus souvent pendant une session ?
- Qu'est-ce qui casse le rythme d'une partie ?
- Les MJ ont-ils besoin d'un mode session séparé du mode préparation ?
- Quelle interface serait trop lourde en pleine partie ?

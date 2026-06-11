# UC-14 — Rechercher rapidement une information

## Acteur principal

MJ

## Acteurs secondaires

Joueur, dans une version limitée de la recherche.

## Objectif

Permettre au MJ de retrouver rapidement une information dans sa campagne.

## Contexte

Même avec une bonne organisation, certaines informations peuvent être difficiles à retrouver pendant une session. La recherche est une fonctionnalité importante pour éviter les interruptions et maintenir le rythme de jeu.

**Persona central : Nadia** (MJ casual, sessions espacées). Après plusieurs semaines d'absence, elle ne se souvient plus où est rangée une information. La recherche est son point d'entrée principal dans le contenu. Émilie en bénéficie aussi pendant la session (retrouver un PNJ en 2 secondes), Thomas moins (son organisation lui suffit).

## Besoin utilisateur

Nadia veut retrouver une information après une longue absence sans se souvenir de son emplacement dans les dossiers. Émilie veut retrouver un PNJ ou une note en quelques secondes pendant la session. La recherche couvre les deux cas : retrouvabilité post-absence et accès rapide en session.

## Déclencheur

Le MJ cherche une information pendant la préparation ou pendant une session.

## Préconditions

- Une campagne existe.
- La campagne contient des données recherchables.
- La recherche porte sur le contenu de la campagne tel qu'il est créé et organisé (documents, dossiers, types, tags — UC-04, UC-05).

## Scénario nominal

1. Le MJ utilise la barre de recherche.
2. Il saisit un mot-clé. La recherche MVP porte sur le **titre des documents uniquement** — aucune indexation du contenu des blocs.
3. Le système affiche les résultats correspondants, filtrés à la campagne active.
4. Les résultats sont regroupés par type de document :
   - documents sans type (libres) ;
   - PNJ, Lieux, Objets, Factions (documents typés) ;
   - scénarios ;
   - notes de session.

5. Le MJ sélectionne un résultat.
6. Le système ouvre l'élément correspondant.
7. Si le MJ est en vue session, l'élément s'ouvre sans casser le contexte de session autant que possible.

## Scénarios alternatifs

### A1 — Aucun résultat

Le système affiche un état vide avec une suggestion de création ou de modification de recherche.

### A2 — Résultats nombreux

Le système permet de filtrer par type de contenu.

### A3 — Recherche par tag *(hors MVP — Could Have)*

Le MJ filtre les résultats par tag ou catégorie.

### A4 — Recherche joueur

Le joueur utilise une recherche limitée aux informations visibles pour lui.

## Exceptions

### E1 — Accès non autorisé

Un joueur tente de rechercher une note privée. Le système ne retourne pas ce résultat.

### E2 — Erreur d'indexation

Si la recherche échoue, le système affiche un message d'erreur et propose une navigation manuelle.

## Postconditions

- Le MJ accède à l'information recherchée.
- Les règles de visibilité sont respectées.

## Données manipulées

### Résultat de recherche

- Identifiant
- Type de contenu
- Titre
- Extrait *(post-MVP — non indexé pour la recherche MVP, affiché uniquement pour aider l'identification)*
- Campagne associée
- Visibilité

## Règles métier

- La recherche du MJ inclut les éléments privés et partagés de sa campagne.
- La recherche joueur n'inclut que les éléments visibles par ce joueur.
- Les résultats doivent être limités au contexte de la campagne active.

## Critères d'acceptation

- Le MJ peut rechercher une information dans une campagne.
- Les résultats sont classés par type.
- Le MJ peut ouvrir un résultat.
- Les notes privées ne sont pas visibles dans la recherche joueur.
- La recherche fonctionne depuis la vue session.

## Questions à valider en interview

- Les MJ utilisent-ils beaucoup la recherche dans leurs outils actuels ?
- Cherchent-ils plutôt par nom, tag, date, type ou contenu ?
- Quelles informations doivent être accessibles en moins de quelques secondes ?
- Une recherche globale est-elle plus utile qu'une navigation structurée ?

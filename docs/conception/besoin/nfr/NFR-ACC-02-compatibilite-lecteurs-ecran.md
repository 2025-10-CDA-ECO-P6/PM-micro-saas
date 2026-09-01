# NFR-ACC-02 — Compatibilité avec les outils de lecture d'écran

Famille **Accessibilité** · [Index de conception](../../README.md)

---

## Énoncé normatif

Le contenu et les actions du produit sont lisibles et utilisables par les personnes
malvoyantes utilisant leurs outils habituels de lecture d'écran. Les éléments interactifs
sont identifiables, les zones de contenu sont structurées de façon intelligible, et les
changements d'état (notification, partage en cours, résultat de recherche) sont annoncés.

---

## Raison d'être

Haversack se positionne comme agnostique au système de jeu (vision §2.2) et sans friction
d'entrée (vision §5). Cette promesse d'ouverture perdrait sa cohérence si une partie de
l'audience potentielle — les personnes malvoyantes qui utilisent un lecteur d'écran — se
retrouvait silencieusement exclue. L'inclusion n'est pas une option de confort : elle conditionne
la crédibilité du positionnement.

Les personnes malvoyantes utilisent des outils de lecture d'écran qui traduisent le contenu
affiché en retour vocal ou en braille. Pour cela, ces outils s'appuient sur la navigation
au clavier (NFR-ACC-01) et sur la structure intelligible du contenu : les éléments interactifs
doivent être identifiables par leur nature, les zones de contenu doivent avoir une organisation
lisible par ces outils, et les changements d'état dynamiques — une notification qui apparaît,
un résultat de recherche qui se charge, un document qui est partagé aux joueurs — doivent
être signalés sans action de l'utilisateur.

La vue session (UC-06) est le point le plus exigeant de ce produit pour cette exigence.
C'est une interface dynamique, avec des panneaux configurables, des notes qui s'affichent
en temps réel, des états qui changent (document épinglé, visibilité de note basculée,
joueur qui consulte un document partagé). Chacun de ces changements d'état doit être
perceptible par un utilisateur de lecteur d'écran sans qu'il ait à explorer activement
l'interface pour le découvrir.

**[Lucas](../persona/persona-03-lucas.md)** illustre le cas joueur : il accède via un lien
de session, sans compte. Un joueur malvoyant qui suit ce parcours doit pouvoir saisir son
nom d'affichage, accéder à la vue joueur et consulter les documents partagés sans aide
extérieure. Si ce parcours est bloqué, le différenciant « accès joueur sans compte » (vision §5)
ne bénéficie qu'à une partie de l'audience potentielle.

**[Émilie](../persona/persona-02-emilie.md)** crée des fiches PNJ à la volée en session.
Pour un MJ malvoyant, cette action doit être accomplissable : l'élément de création doit
être identifiable par son rôle, la zone de saisie doit être accessible, et le retour
confirmant que le document a bien été créé et épinglé doit être perceptible sans voir l'écran.

Le référentiel d'accessibilité des contenus web reconnu internationalement (référence :
WCAG 2.1, niveau AA) documente les critères mesurables qui orientent l'implémentation de
cette exigence, sans s'y substituer.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ensemble des éléments interactifs du produit : boutons, zones de saisie, liens, menus,
  panneaux configurables de la vue session — leur nature et leur rôle doivent être lisibles
  par les outils de lecture d'écran habituels.
- La structure des zones de contenu : les panneaux de dossiers, les listes de documents, le
  panneau des documents épinglés, la zone de notes de session — leur organisation doit être
  intelligible de façon séquentielle.
- Les changements d'état dynamiques : apparition d'une notification, résultat de recherche
  chargé, basculement de visibilité d'une note, confirmation d'épinglage, document partagé
  aux joueurs — ces changements doivent être annoncés sans action de l'utilisateur.
- Le parcours joueur : accès via lien de session, saisie d'un nom, consultation des documents
  partagés, prise de notes personnelles.
- Le parcours MJ complet depuis l'ouverture de l'application jusqu'à la clôture d'une session.

**Ce que cette exigence ne couvre pas :**

- La configuration des outils de lecture d'écran eux-mêmes — ce sont des outils tiers
  dont la configuration relève de l'utilisateur.
- La compatibilité avec des outils de lecture d'écran anciens ou non maintenus — l'exigence
  porte sur les outils habituels en usage actuel.
- Les contenus créés par le MJ dans ses documents (leur structure interne relève de l'auteur,
  non du produit).
- Les interfaces tiers accessibles depuis des liens partagés (documents exportés, liens externes).

---

## Critères d'acceptation produit

**Parcours MJ — création d'une campagne et lancement de session :**

Un MJ utilisant un lecteur d'écran peut créer une campagne, saisir un titre de session et
lancer la vue session sans aide extérieure. Chaque étape est franchissable depuis le seul
retour fourni par l'outil de lecture d'écran.

**Parcours MJ — navigation en vue session :**

Un MJ utilisant un lecteur d'écran peut identifier les panneaux de dossiers configurés,
naviguer dans les documents d'un panneau, ouvrir un document en consultation et revenir
à la vue session — sans avoir à voir l'écran pour comprendre dans quelle zone il se trouve.

**Changements d'état annoncés sans action :**

Lorsqu'une note de session est créée, lorsque la visibilité d'un document bascule vers
visible par les joueurs, lorsqu'un résultat de recherche se charge, et lorsqu'une notification
non bloquante apparaît (perte de connexion, espace de stockage sous pression), le changement
est annoncé par le produit sans que l'utilisateur ait à déplacer son focus pour le découvrir.

**Parcours joueur — accès et consultation :**

Un joueur malvoyant utilisant un lecteur d'écran peut cliquer sur un lien de session, saisir
son nom d'affichage, accéder à la liste des documents partagés par le MJ et lire le contenu
de ces documents — sans aide extérieure.

**Éléments interactifs identifiables par leur rôle :**

Depuis le retour d'un lecteur d'écran, chaque bouton, zone de saisie et lien du produit est
distinguable : son rôle (bouton, champ de saisie, liste) et son libellé (l'action qu'il
déclenche ou l'information qu'il contient) sont perceptibles sans avoir à voir l'élément.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (phases 2-3, changements d'état, notifications) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| UC-09 — Accès session joueur | [../usecases/UC-09-acces-session-joueur.md](../usecases/UC-09-acces-session-joueur.md) |
| UC-14 — Rechercher rapidement une information | [../usecases/UC-14-recherche.md](../usecases/UC-14-recherche.md) |
| US-06-01 — Lancer une session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-03 — Naviguer dans les dossiers pendant la session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-04 — Prendre des notes de session MJ (notification E1) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-07 — Accéder à la vue joueur pendant une session LIVE | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-14-03 — Rechercher depuis la vue session | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| NFR-ACC-01 — Utilisabilité sans dispositif de pointage (navigation clavier, prérequis) | [NFR-ACC-01-utilisabilite-sans-dispositif-de-pointage.md](NFR-ACC-01-utilisabilite-sans-dispositif-de-pointage.md) |

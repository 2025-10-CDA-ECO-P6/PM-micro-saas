# NFR-ACC-03 — Lisibilité en conditions de faible éclairage

Famille **Accessibilité** · [Index de conception](../README.md)

---

## Énoncé normatif

Les contrastes entre le texte et l'arrière-plan sont suffisants pour que le contenu de
l'application soit lisible dans les conditions d'éclairage typiques d'une soirée de jeu de
rôle autour d'une table (lumière d'ambiance, pièce partiellement éclairée).

---

## Raison d'être

La vue session (UC-06) est utilisée pendant une partie de jeu de rôle, typiquement en
soirée, dans un appartement ou une salle éclairée à la lumière d'ambiance. Ce contexte
est distinct de celui d'un bureau ou d'une salle de réunion : l'éclairage est souvent
tamisé, parfois en partie indirecte, et l'écran du MJ coexiste avec des bougies, des
lampes de table ou des lumières de plafond réglées pour créer une atmosphère.

Ce contexte réel d'usage définit le niveau de contraste minimum que le produit doit
garantir. Un produit dont le texte devient difficile à lire dans ces conditions rate
son contexte d'usage central — c'est précisément le moment où le MJ en a le plus besoin.

**[Nadia](../persona/persona-04-nadia.md)** joue une session par mois, souvent le soir
dans un appartement après avoir couché ses enfants. Sa fenêtre de préparation est courte,
et pendant la session elle doit retrouver des informations rapidement. Si la lisibilité
de l'écran la force à rapprocher son visage ou à augmenter la luminosité de l'écran —
ce qui perturbe l'ambiance de la table —, l'outil crée une friction là où il devait en
retirer.

**[Émilie](../persona/persona-02-emilie.md)** joue des systèmes à forte dimension
narrative (Ironsworn, Blades in the Dark, Fate) où l'atmosphère de la table est une
composante du jeu à part entière. Elle est attentive à ne pas interrompre le flux narratif.
Un écran trop lumineux ou un contraste insuffisant qui l'oblige à manipuler les réglages
de son appareil en pleine scène est une interruption de même nature que chercher dans
cinq fichiers.

**[Antoine](../persona/persona-05-antoine.md)** mène plusieurs campagnes en parallèle,
avec des soirées fréquentes. La lisibilité en faible éclairage est pour lui une condition
de confort durable, pas une tolérance ponctuelle.

Le référentiel d'accessibilité des contenus web reconnu internationalement (référence :
WCAG 2.1, niveau AA) documente des critères mesurables de contraste textuel qui orientent
l'implémentation de cette exigence. Ces critères constituent une référence de calibrage
pour les décisions d'implémentation, sans se substituer au besoin formulé ici.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- Le texte des documents consultés depuis la vue session : titres, contenu des blocs,
  propriétés affichées en vue condensée dans les panneaux de dossiers.
- Les notes de session créées et affichées pendant une session LIVE.
- Les résultats de recherche affichés dans le panneau latéral (UC-14).
- Les notifications et messages non bloquants affichés pendant la session (état de
  synchronisation, avertissements d'espace de stockage).
- Les libellés des éléments interactifs de la vue session : boutons, indicateurs d'état,
  étiquettes de panneaux.
- La vue joueur : documents partagés, notes personnelles.

**Ce que cette exigence ne couvre pas :**

- Les conditions d'éclairage extrêmes (obscurité totale, lumière de plein soleil direct
  sur l'écran) — ces conditions sortent du contexte d'usage documenté et sont de la
  responsabilité du matériel de l'utilisateur.
- La gestion de la luminosité et des modes d'affichage de l'appareil (mode sombre,
  mode nuit) — leur activation relève de la configuration de l'appareil par l'utilisateur.
  Cette exigence garantit la lisibilité sans dépendre de ces modes.
- Le contenu créé par le MJ à l'intérieur des documents (choix de mise en forme du contenu
  par l'auteur, non par le produit).
- Les interfaces tiers accessibles depuis le produit (liens externes, exports).

---

## Critères d'acceptation produit

**Lisibilité du contenu de session sans ajustement de l'appareil :**

Dans une pièce éclairée à la lumière d'ambiance du soir (lampe de table, plafond tamisé),
un utilisateur avec une acuité visuelle normale peut lire le texte des documents, des notes
de session et des résultats de recherche dans la vue session sans ressentir le besoin de
modifier la luminosité de son écran ni de se rapprocher de l'appareil.

**Lisibilité des notifications et indicateurs d'état :**

Les messages non bloquants qui apparaissent pendant la session (notification de synchronisation
en attente, avertissement de stockage local) sont lisibles dans les mêmes conditions
d'éclairage, sans que l'utilisateur ait à les chercher visuellement.

**Lisibilité des libellés d'interface :**

Les étiquettes des panneaux, les libellés des boutons et les indicateurs d'état de la vue
session sont lisibles dans les mêmes conditions, y compris les éléments en mode condensé
(vue résumée d'un document dans un panneau de dossier).

**Lisibilité de la vue joueur :**

La vue joueur, utilisée depuis l'appareil personnel du joueur dans les mêmes conditions
d'éclairage de soirée, présente un niveau de contraste suffisant pour que les documents
partagés soient lisibles sans configuration préalable.

**Absence de surbrillance pénalisante :**

Aucun élément de l'interface, dans son état par défaut, ne crée un halo lumineux ou une
surbrillance qui dégrade la lisibilité du texte environnant dans un environnement peu éclairé.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (contexte d'usage, phase 2 — affichage des panneaux) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| UC-14 — Rechercher rapidement une information (résultats affichés en session) | [../usecases/UC-14-recherche.md](../usecases/UC-14-recherche.md) |
| UC-09 — Accès session joueur (vue joueur) | [../usecases/UC-09-acces-session-joueur.md](../usecases/UC-09-acces-session-joueur.md) |
| US-06-03 — Naviguer dans les dossiers pendant la session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-04 — Prendre des notes de session MJ (notification E1) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-07 — Accéder à la vue joueur pendant une session LIVE | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-14-03 — Rechercher depuis la vue session | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |

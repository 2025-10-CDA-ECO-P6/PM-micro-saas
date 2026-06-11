# NFR-ACC-04 — Tailles de texte adaptées à la lecture rapide en session

Famille **Accessibilité** · [Index de conception](../README.md)

---

## Énoncé normatif

La taille des éléments textuels dans la vue session est suffisante pour une lecture rapide
à distance normale de l'écran, sans zoom ni configuration préalable.

---

## Raison d'être

Le MJ utilise la vue session (UC-06) dans des conditions de lecture spécifiques : il est
assis à une table, à une distance normale de son écran, et lit ses notes pendant qu'une
conversation se déroule à voix haute autour de lui. Il n'a pas le temps de se pencher vers
l'écran, de zoomer ou de réajuster l'affichage. Une lecture qui exige un effort visuel —
si peu que ce soit — interrompt le contact visuel avec les joueurs et signale à la table
que le MJ est distrait par son outil.

Ce contexte de lecture rapide à distance est distinct de la lecture posée et approfondie
que l'on pratique lors de la préparation. En session, le MJ cherche un nom, vérifie une
valeur, lit les premières lignes d'une note. La taille du texte détermine si ce geste peut
être accompli sans effort perceptible par la table, ou s'il crée une rupture dans le
déroulement narratif.

**[Thomas](../persona/persona-01-thomas.md)** jonglait entre des onglets avant Haversack.
La valeur de la vue session pour lui réside précisément dans l'élimination de ce temps
perdu à chercher et à se repositionner. Si le texte de la vue session est trop petit et
l'oblige à se rapprocher de l'écran, le gain est annulé — non pas par la recherche, mais
par la lecture elle-même.

**[Nadia](../persona/persona-04-nadia.md)** joue dans des conditions souvent sous-optimales
(fin de soirée, fatigue). Un texte dont la taille impose un effort visuel supplémentaire
dans ces conditions dégrade l'expérience de session au moment où elle en a le plus besoin.

**[Sonia](../persona/persona-07-sonia.md)** anime des parties en festival, parfois avec
un grand écran partagé visible de plusieurs participants à distance variable. La taille
du texte affiché dans la vue session doit permettre une lecture rapide depuis la place du
MJ à la table — pas depuis le fond de la salle, mais depuis la position naturelle de
travail.

Cette exigence s'inscrit dans les conditions réelles d'usage du produit : éclairage faible
autour d'une table de jeu, lecture rapide en pleine session, parfois sur un grand écran
partagé ou un petit écran mobile. Le référentiel d'accessibilité des contenus web reconnu
internationalement (référence : WCAG 2.1, niveau AA) fournit des critères orientant les
décisions d'implémentation relatives aux dimensions des éléments textuels. Ces critères
restent une référence d'implémentation, non un critère d'acceptation produit.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- Les titres et le contenu des documents affichés dans les panneaux de la vue session
  (vue condensée dans les panneaux de dossiers, vue complète ouverte depuis un panneau).
- Les notes de session affichées dans la zone de notes pendant une session LIVE.
- Les résultats de recherche affichés dans le panneau latéral depuis la vue session (UC-14).
- Les libellés d'interface visibles pendant la session : étiquettes de panneaux, indicateurs,
  noms de documents dans le panneau des documents épinglés.
- La vue joueur, pour les documents partagés et les notes personnelles consultées pendant
  la session.

**Ce que cette exigence ne couvre pas :**

- La phase de préparation (hors session LIVE) — le contexte de lecture y est différent :
  posé, prolongé, à une distance d'écran plus variable.
- Les écrans exceptionnellement éloignés ou les configurations grand écran à plusieurs mètres
  de distance — l'exigence porte sur la distance normale de l'utilisateur à sa session de
  travail, pas sur une projection en salle.
- Les paramètres de zoom ou d'accessibilité de l'appareil — leur activation relève de la
  configuration de l'utilisateur. Cette exigence garantit une taille lisible sans dépendre
  de ces paramètres.
- Le contenu créé à l'intérieur des documents par le MJ (taille des blocs de texte définie
  par l'auteur, non par le produit).

---

## Critères d'acceptation produit

**Lecture des titres de documents sans effort :**

Dans la vue session, un MJ assis à une distance normale de son écran peut lire le titre
d'un document affiché dans un panneau de dossier d'un seul coup d'œil, sans incliner la
tête ni se rapprocher de l'appareil.

**Lecture des notes de session pendant la conversation :**

Un MJ peut lire les premières lignes d'une note de session pendant qu'une conversation
se déroule autour de lui, sans interrompre le contact visuel avec la table pour s'approcher
de l'écran.

**Lecture des résultats de recherche en session :**

Les titres des résultats affichés dans le panneau latéral de recherche (UC-14) sont lisibles
à distance normale de l'écran, sans que le MJ ait à zoomer ou à ouvrir chaque résultat
pour identifier lequel correspond à sa recherche.

**Lecture dans la vue joueur :**

Les documents partagés et les notes personnelles sont lisibles depuis la position habituelle
du joueur à la table — assis face à son propre appareil, à distance normale.

**Aucune configuration préalable requise :**

La taille du texte dans la vue session est suffisante à l'ouverture de l'application,
sans que le MJ ait à ajuster les paramètres de l'application ou de son appareil avant
de lancer une session.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (contexte, phase 2 — panneaux configurés, notes de session) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| UC-14 — Rechercher rapidement une information (résultats en vue session) | [../usecases/UC-14-recherche.md](../usecases/UC-14-recherche.md) |
| UC-09 — Accès session joueur (vue joueur) | [../usecases/UC-09-acces-session-joueur.md](../usecases/UC-09-acces-session-joueur.md) |
| US-06-03 — Naviguer dans les dossiers pendant la session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-04 — Prendre des notes de session MJ | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-07 — Accéder à la vue joueur pendant une session LIVE | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-14-03 — Rechercher depuis la vue session | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |

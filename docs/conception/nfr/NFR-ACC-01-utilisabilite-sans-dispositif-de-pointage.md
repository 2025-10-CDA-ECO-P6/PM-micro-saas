# NFR-ACC-01 — Utilisabilité sans dispositif de pointage

Famille **Accessibilité** · [Index de conception](../README.md)

---

## Énoncé normatif

Toutes les fonctionnalités du produit sont accessibles en utilisant uniquement un clavier ou
un dispositif de navigation équivalent. Aucune action critique (créer, consulter, partager,
rechercher) ne nécessite l'usage d'une souris ou d'un écran tactile.

---

## Raison d'être

La vue session est l'interface à la plus forte contrainte temporelle du produit (UC-06,
contexte). Le MJ l'utilise pendant qu'une conversation réelle se déroule autour de lui. Dans
ce contexte, il arrive qu'une main soit occupée — prenant une note sur papier, indiquant un
élément sur un plan physique, tenant une fiche — et que la navigation au clavier soit la
seule option disponible à l'instant voulu.

Au-delà de la session, l'accessibilité clavier représente une condition d'inclusion fondamentale :
certains utilisateurs ne peuvent pas utiliser une souris ou un écran tactile de façon permanente,
non ponctuelle. Pour ces utilisateurs, l'exigence n'est pas un confort mais la condition d'accès
au produit.

**[Thomas](../persona/persona-01-thomas.md)** utilise Haversack comme couche de vue session
et de partage par-dessus son organisation existante. Un parcours clavier complet lui permettrait
de naviguer entre ses dossiers configurés, d'épingler un document et de lancer une recherche
sans jamais quitter sa position à table ni saisir la souris. La valeur qu'il cherche — un accès
rapide pendant la partie — ne se matérialise que si chaque action de navigation est atteignable
sans geste supplémentaire.

**[Nadia](../persona/persona-04-nadia.md)** accède à Haversack avec des sessions espacées,
souvent dans des conditions peu optimales. Un outil navigable entièrement au clavier réduit
la charge de manipulation de l'interface, libérant son attention vers le contenu.

L'accessibilité clavier conditionne également la compatibilité avec les technologies
d'assistance — en particulier les outils de lecture d'écran, dont l'usage repose sur une
navigation séquentielle au clavier. Cette interdépendance est couverte distinctement par
NFR-ACC-02, mais elle confirme que NFR-ACC-01 est une exigence structurante qui conditionne
la réalisation d'autres exigences d'accessibilité.

Le référentiel d'accessibilité des contenus web reconnu internationalement (référence :
WCAG 2.1, niveau AA) documente les critères mesurables associés à la navigation au clavier.
Il oriente l'implémentation sans se substituer au besoin formulé ici.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ensemble des actions critiques documentées dans les use cases du MVP : créer une campagne
  (UC-02), structurer un scénario (UC-03), gérer des documents (UC-04), organiser par dossiers
  (UC-05), lancer et piloter une session (UC-06), créer à la volée (UC-07), partager une
  information (UC-08), accéder à la session en tant que joueur (UC-09), gérer les membres
  (UC-11), rejoindre une campagne (UC-12), et rechercher une information (UC-14).
- La navigation entre les panneaux de la vue session, l'épinglage d'un document, la prise de
  notes de session, le basculement de visibilité d'une note.
- Les actions joueur : accès à la vue session, consultation des documents partagés, prise de
  notes personnelles.
- Le parcours d'accès au produit : ouverture de l'application, création d'un espace de jeu,
  saisie d'un titre de session.

**Ce que cette exigence ne couvre pas :**

- Les interactions avec des services ou outils tiers (partage de lien vers une autre application,
  ouverture d'un export dans un lecteur externe).
- La configuration matérielle de l'appareil de l'utilisateur (disposition du clavier, pilotes,
  paramètres d'accessibilité du système d'exploitation).
- Les parcours explicitement hors MVP (UC-HORS-MVP).

---

## Critères d'acceptation produit

**Parcours complet MJ — vue session au clavier :**

Un MJ ayant ouvert la vue session peut, en utilisant uniquement le clavier : passer d'un
panneau de dossier au suivant, ouvrir un document pour le consulter, épingler ce document
dans le panneau des documents épinglés, prendre une note de session, basculer la visibilité
de cette note vers visible par les joueurs, et lancer une recherche — sans jamais avoir à
saisir une souris ou toucher l'écran.

**Parcours complet MJ — préparation au clavier :**

Un MJ peut créer une campagne, saisir un titre de session et accéder à la vue session
entièrement au clavier, depuis l'ouverture de l'application.

**Parcours complet joueur — accès et consultation au clavier :**

Un joueur accédant via un lien de session peut saisir son nom d'affichage, accéder à la
vue joueur, naviguer parmi les documents partagés et créer une note personnelle — entièrement
au clavier.

**Focus visible en permanence :**

À tout moment, l'élément ayant le focus au clavier est visuellement identifiable pour
l'utilisateur. Aucune étape de navigation ne laisse le focus dans un état invisible ou ambigu.

**Aucun blocage dans les parcours documentés :**

Un utilisateur naviguant exclusivement au clavier peut accomplir l'ensemble des parcours
utilisateur documentés (user journeys MVP) sans rencontrer d'action inatteignable ou de
composante interactive sur laquelle le clavier est sans effet.

**Ordre de navigation cohérent :**

L'ordre dans lequel le clavier passe d'un élément interactif au suivant suit la logique
visuelle de la page — du plus général au plus spécifique, sans sauts incohérents entre des
zones sans rapport.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (phases 2-3, navigation MJ) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| UC-14 — Rechercher rapidement une information | [../usecases/UC-14-recherche.md](../usecases/UC-14-recherche.md) |
| UC-09 — Accès session joueur | [../usecases/UC-09-acces-session-joueur.md](../usecases/UC-09-acces-session-joueur.md) |
| US-06-03 — Naviguer dans les dossiers pendant la session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-04 — Prendre des notes de session MJ | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-06-05 — Épingler un document pendant la session | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| US-14-01 — Rechercher un document par titre | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| US-14-03 — Rechercher depuis la vue session | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |

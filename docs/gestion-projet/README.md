# Gestion de projet — Haversack

> **Nature du document.** Ce dossier rassemble le pilotage du projet : cadrage, planification,
> organisation, risques et conventions de travail. Chaque document consolide et renvoie vers sa
> source de conception ; en cas de divergence de détail, la source citée fait foi.

---

## Documents du dossier

| Document | Contenu |
|---|---|
| [note-cadrage-projet.md](note-cadrage-projet.md) | Note de cadrage / charte de projet — raison d'être, objectifs, périmètre, parties prenantes, jalons, gouvernance |
| [guide-conventions-et-dod.md](guide-conventions-et-dod.md) | Guide de conventions de développement et Definition of Done |
| [roadmap-entree-build.md](roadmap-entree-build.md) | Roadmap d'entrée en build — ordonnancement technique du build |
| [guide-lecture-par-fonctionnalite.md](guide-lecture-par-fonctionnalite.md) | Guide d'entrée par fonctionnalité — renvois vers le besoin, les règles métier, la maquette, les décisions d'architecture, la spécification technique et les cas de recette de chaque fonctionnalité |
| [plan-de-travail.md](plan-de-travail.md) | Plan de travail — décomposition en épiques et en tâches du périmètre de build, avec dépendances et périmètre d'écriture |
| [methode-de-ticket.md](methode-de-ticket.md) | Méthode de ticket — modèle de ticket, condition d'entrée avant prise et cycle de vie jusqu'à clôture |
| [roadmap-produit.md](roadmap-produit.md) | Roadmap produit — trajectoire de valeur fonctionnelle |
| [registre-risques.md](registre-risques.md) | Registre d'évaluation des risques (méthode ISO 31000) |
| [decisions-en-attente.md](decisions-en-attente.md) | Index des décisions en attente, regroupées par décideur et ordonnées par ce qu'elles bloquent dans la séquence J0 → J1 → J2 → J3 → lancement |

## Deux roadmaps distinctes

Ce dossier porte deux documents nommés « roadmap », à ne pas confondre :

- [roadmap-entree-build.md](roadmap-entree-build.md) ordonnance la séquence technique du build (jalons J0 → J3, points de décision go/no-go), à destination de l'équipe de build et du responsable produit.
- [roadmap-produit.md](roadmap-produit.md) décrit la trajectoire de valeur du produit, palier après palier, à destination d'une communication externe (investisseurs, partenaires, parties prenantes non techniques) ; elle ne porte pas de calendrier d'exécution.

## Deux artefacts d'ordonnancement, à des grains distincts

Ce dossier porte désormais deux artefacts qu'un lecteur peut confondre, tous deux issus de la séquence de jalons — à ne pas confondre non plus avec les deux roadmaps ci-dessus :

- [roadmap-entree-build.md](roadmap-entree-build.md) ordonnance les **jalons** eux-mêmes : leur contenu, leurs préalables bloquants, leurs critères de sortie, les points de décision qui les séparent.
- [plan-de-travail.md](plan-de-travail.md) décompose **à l'intérieur** de ces jalons, en épiques et en tâches.

Les critères de sortie d'un jalon ne se trouvent donc pas dans le plan de travail — ils appartiennent à `roadmap-entree-build.md` et au cahier de stratégie de test et de recette ; le plan de travail ne les redéfinit pas et y renvoie.

## Guide de conventions — vocation de sortie du repo documentaire

[guide-conventions-et-dod.md](guide-conventions-et-dod.md) a vocation à alimenter le dépôt de code à l'ouverture du développement : il rassemble les conventions et la Definition of Done applicables dès l'entrée en build, avant que ce dépôt ne devienne lui-même le point de référence courant.

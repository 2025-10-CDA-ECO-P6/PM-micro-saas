# Haversack

> Outil d'aide à la préparation et à la conduite de parties de jeu de rôle.

**État au 2026-10-07 :** ce dépôt contient la conception complète du produit et l'outillage C# qui la vérifie. Aucune ligne de code produit n'existe encore. Voir [État du projet et travail en cours](#12-état-du-projet-et-travail-en-cours).

---

## Sommaire

1. [En bref](#1-en-bref)
2. [Reprendre le projet](#2-reprendre-le-projet)
3. [Carte de la documentation](#3-carte-de-la-documentation)
4. [Où trouver quoi](#4-où-trouver-quoi)
5. [Le produit](#5-le-produit)
6. [Périmètre de la première livraison](#6-périmètre-de-la-première-livraison)
7. [Concepts et vocabulaire](#7-concepts-et-vocabulaire)
8. [Sans compte, puis avec un compte](#8-sans-compte-puis-avec-un-compte)
9. [Architecture technique](#9-architecture-technique)
10. [Règles du corpus](#10-règles-du-corpus)
11. [Vérifier le corpus](#11-vérifier-le-corpus)
12. [État du projet et travail en cours](#12-état-du-projet-et-travail-en-cours)
13. [Jalonnement](#13-jalonnement)

---

## 1. En bref

- **Quoi** : un outil pour le Maître du Jeu (MJ). Il prépare son contenu narratif, pilote ses sessions et partage sélectivement des informations à ses joueurs. Ce n'est ni une table virtuelle, ni un éditeur générique.
- **Pour qui** : le MJ d'abord, les joueurs ensuite.
- **Comment** : une application web Angular, un backend ASP.NET Core et PostgreSQL. L'application est utilisable sans compte, en local dans le navigateur, puis avec un compte cloud.
- **Où en est-on** : la conception est écrite (besoin, domaine, interface, architecture, sécurité, test, plan de travail). Le développement commence au jalon J0. Un backlog d'alignement du corpus est ouvert depuis le 2026-10-07.

---

## 2. Reprendre le projet

Choisissez le parcours qui correspond à ce que vous venez faire.

### Découvrir le produit

1. Ce README, sections 5 à 8.
2. [vision-produit.md](docs/conception/besoin/vision/vision-produit.md) : le problème, les personas, le positionnement.
3. [moscow.md](docs/conception/besoin/vision/moscow.md) : ce qui est dans la première livraison et ce qui n'y est pas.
4. [use-cases.md](docs/conception/besoin/usecases/use-cases.md) : la vue d'ensemble des capacités.

### Écrire la première ligne de code (J0)

1. [docs/README.md](docs/README.md) : la carte du corpus.
2. [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md) : les jalons, les préalables bloquants et les critères de sortie.
3. [guide-conventions-et-dod.md](docs/gestion-projet/guide-conventions-et-dod.md) : les conventions de code et la Definition of Done.
4. [structure-projets.md](docs/architecture/structure-projets.md) et [ADR-008](docs/architecture/decisions/ADR-008-structure-solution.md) : la structure des projets .NET.
5. [guide-lecture-par-fonctionnalite.md](docs/gestion-projet/guide-lecture-par-fonctionnalite.md) : pour une capacité donnée, le besoin, les règles, la maquette, les décisions et la spécification.
6. [plan-de-travail.md](docs/gestion-projet/plan-de-travail.md) : les tâches TB-nnn, leurs dépendances et leur périmètre.
7. [methode-de-ticket.md](docs/gestion-projet/methode-de-ticket.md) : le modèle de ticket et son cycle de vie.

### Modifier la conception

1. [Règles du corpus](#10-règles-du-corpus) : l'ordre d'autorité, qui dit quel document corriger en cas d'écart.
2. [ADR-019](docs/architecture/decisions/ADR-019-conventions-de-tenue-du-corpus.md) : les conventions de tenue (identifiants, gabarits, marqueurs, renvois…).
3. [Vérifier le corpus](#11-vérifier-le-corpus) : les contrôles à lancer avant de commiter.
4. [docs/project/backlog/](docs/project/backlog/README.md) : les écarts déjà connus, pour ne pas les redécouvrir.

### Piloter le projet

1. [note-cadrage-projet.md](docs/gestion-projet/note-cadrage-projet.md) : les objectifs et les enjeux.
2. [roadmap-produit.md](docs/gestion-projet/roadmap-produit.md) : la trajectoire de valeur.
3. [decisions-en-attente.md](docs/gestion-projet/decisions-en-attente.md) : les questions ouvertes.
4. [registre-risques.md](docs/gestion-projet/registre-risques.md) : les risques et leur traitement.

---

## 3. Carte de la documentation

Chaque dossier a son propre index, qui fait foi sur son contenu. Ce tableau n'en donne que l'entrée.

### Besoin — [docs/conception/besoin/](docs/conception/besoin/README.md)

| Document | Rôle |
|---|---|
| [vision/vision-produit.md](docs/conception/besoin/vision/vision-produit.md) | Le problème, les hypothèses, le positionnement, le modèle de monétisation |
| [vision/moscow.md](docs/conception/besoin/vision/moscow.md) | La seule autorité sur les priorités (Must, Should, Could, Won't) |
| [persona/](docs/conception/besoin/persona/README.md) | Les 7 personas de référence |
| [usecases/](docs/conception/besoin/usecases/README.md) | Les 15 use cases UC-NN, source de vérité du besoin fonctionnel, et les exclusions ([UC-HORS-MVP](docs/conception/besoin/usecases/UC-HORS-MVP.md)) |
| [user-stories/](docs/conception/besoin/user-stories/README.md) | Les épiques US-UC-NN : stories, règles métier RB-NN-NN et critères Gherkin |
| [user-journeys/](docs/conception/besoin/user-journeys/README.md) | Le parcours de chaque use case |
| [parcours/](docs/conception/besoin/parcours/README.md) | Les parcours transverses d'un persona à travers plusieurs use cases |
| [nfr/](docs/conception/besoin/nfr/README.md) | Les 20 exigences non fonctionnelles NFR-XXX-NN |
| [INTERVIEW_GUIDE.md](docs/conception/INTERVIEW_GUIDE.md) | Le guide des entretiens terrain |

### Domaine — [docs/conception/domain/](docs/conception/domain/README.md)

| Document | Rôle |
|---|---|
| [identity-access.md](docs/conception/domain/identity-access.md), [space-management.md](docs/conception/domain/space-management.md), [content-library.md](docs/conception/domain/content-library.md), [session-conduct.md](docs/conception/domain/session-conduct.md) | Les quatre contextes métier : agrégats, invariants, événements |
| [core.md](docs/conception/domain/core.md) | Le noyau partagé |
| [diagrams/](docs/conception/domain/README.md) | Diagrammes de classes, de flux, MCD et MLD de chaque contexte |
| [glossaire.md](docs/conception/glossaire.md) | La référence des noms de termes |

### Interface — [docs/conception/interface/](docs/conception/interface/README.md)

| Document | Rôle |
|---|---|
| [zoning.md](docs/conception/interface/zoning.md) | L'ossature de navigation, l'inventaire des écrans (S4) et les arbitrages AR-NN |
| [conventions-wireframe.md](docs/conception/interface/conventions-wireframe.md) | Les conventions des maquettes et le vocabulaire affiché |
| [gabarit-ecran.md](docs/conception/interface/gabarit-ecran.md) | Le gabarit d'une fiche écran |
| [wireframes/](docs/conception/interface/wireframes/README.md) | Les fiches écran et les maquettes basse fidélité |
| [reflexion-ux-mvp.md](docs/conception/interface/reflexion-ux-mvp.md) | Le raisonnement UX derrière ces choix |

### Architecture — [docs/architecture/](docs/architecture/README.md)

| Document | Rôle |
|---|---|
| [decisions/](docs/architecture/decisions/README.md) | Le registre des décisions, ADR-001 à ADR-019 |
| [stack.md](docs/architecture/stack.md) | La pile technique et ses raisons |
| [structure-projets.md](docs/architecture/structure-projets.md) | La structure de la solution .NET et l'ordre de construction |
| [ddd-fondations.md](docs/architecture/ddd-fondations.md), [architecture-detaillee.md](docs/architecture/architecture-detaillee.md) | Les fondations DDD et les préoccupations transverses |
| `specs/` | Huit spécifications pré-build : [contrat OpenAPI](docs/architecture/specs/contrat-openapi.md), [mapping EF Core](docs/architecture/specs/mapping-ef-core.md), [schémas de propriétés](docs/architecture/specs/document-properties-schemas.md), [sanitisation et CSP](docs/architecture/specs/sanitisation-csp.md), [télémétrie](docs/architecture/specs/telemetrie.md), [repli temps réel](docs/architecture/specs/repli-temps-reel.md), [configuration de sécurité de la migration](docs/architecture/specs/config-securite-migration.md), [requête d'effacement](docs/architecture/specs/requete-effacement-non-partage.md) |

### Sécurité, test, déploiement — [docs/securite/](docs/securite/README.md), [docs/test/](docs/test/README.md)

| Document | Rôle |
|---|---|
| [dossier-securite.md](docs/securite/dossier-securite.md) | L'analyse de menaces et les contre-mesures |
| `securite/conformite/` | Les cadrages RGPD et les projets de documents publics ([CGU](docs/securite/conformite/cgu.md), [politique de confidentialité](docs/securite/conformite/politique-confidentialite.md), [feuille de route juridique](docs/securite/conformite/roadmap-juridique.md)…), en attente de validation par un juriste |
| [cahier-strategie-test-et-recette.md](docs/test/cahier-strategie-test-et-recette.md) | La stratégie de test et les cas de recette CR-UCNN-KK |
| [deploiement/](docs/deploiement/README.md), [ecoconception/](docs/ecoconception/README.md) | Des dossiers réservés, encore vides |

### Gestion de projet — [docs/gestion-projet/](docs/gestion-projet/README.md)

| Document | Rôle |
|---|---|
| [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md) | Les jalons J0 à J3 et les points de décision |
| [plan-de-travail.md](docs/gestion-projet/plan-de-travail.md) | Les tâches TB-nnn, la source de vérité du reste à faire |
| [methode-de-ticket.md](docs/gestion-projet/methode-de-ticket.md) | Le modèle de ticket et son cycle de vie |
| [guide-conventions-et-dod.md](docs/gestion-projet/guide-conventions-et-dod.md) | Les conventions de code et de corpus, et la Definition of Done |
| [guide-lecture-par-fonctionnalite.md](docs/gestion-projet/guide-lecture-par-fonctionnalite.md) | L'entrée par fonctionnalité |
| [decisions-en-attente.md](docs/gestion-projet/decisions-en-attente.md) | L'index des questions ouvertes |
| [registre-risques.md](docs/gestion-projet/registre-risques.md), [note-cadrage-projet.md](docs/gestion-projet/note-cadrage-projet.md), [roadmap-produit.md](docs/gestion-projet/roadmap-produit.md), [revue-produit-plan-de-travail.md](docs/gestion-projet/revue-produit-plan-de-travail.md) | Les risques, le cadrage, la trajectoire produit et la revue du plan |

### Documents de référence — [docs/context/](docs/context/README.md)

Ces documents sont lisibles seuls et remis tels quels à un tiers : [cahier des charges](docs/context/cahier-des-charges.md), [cahier de spécifications techniques](docs/context/cahier-specifications-techniques.md), [dossier d'architecture](docs/context/dossier-architecture.md), [dossier de conception détaillée](docs/context/dossier-conception-detaillee.md), [note business](docs/context/note-business-gtm.md). Ils consolident le corpus et ne font jamais foi : en cas d'écart, la source détaillée l'emporte.

### Backlog d'alignement — [docs/project/backlog/](docs/project/backlog/README.md)

Les 92 entrées ouvertes le 2026-10-07 : les réponses du porteur du projet à reporter dans les sources, et les écarts à aligner, document par document.

### Outillage — `tools/`

| Projet | Rôle |
|---|---|
| `tools/VerifCorpus` | Contrôle les liens, ancres et orphelins du corpus |
| `tools/VerifMailleAgregat` | Contrôle la maille du plan de travail |
| `tools/VerifCouvertureRecette` | Contrôle la couverture des cas de recette |
| `tools/CreerIssues`, `tools/CreerProjet` | Reportent le plan de travail en issues et en projet GitHub (workflow `.github/workflows/recalcul-suivi-projet.yml`) |
| `tools/Tests`, `tools/Harnais` | Les tests de caractérisation de l'outillage et leurs doublures |

---

## 4. Où trouver quoi

| Vous cherchez… | Le document qui fait foi |
|---|---|
| La priorité d'une capacité | [moscow.md](docs/conception/besoin/vision/moscow.md) |
| Ce que doit faire une capacité | Son use case dans [usecases/](docs/conception/besoin/usecases/README.md) |
| Une règle métier RB-NN-NN et ses critères d'acceptation | Son épique dans [user-stories/](docs/conception/besoin/user-stories/README.md) |
| Le sens d'un terme | [glossaire.md](docs/conception/glossaire.md) |
| Un invariant, un agrégat, un événement | Le contexte concerné dans [domain/](docs/conception/domain/README.md) |
| Un écran, sa navigation | [zoning.md](docs/conception/interface/zoning.md) et [wireframes/](docs/conception/interface/wireframes/README.md) |
| Une décision et ses alternatives écartées | [decisions/](docs/architecture/decisions/README.md) |
| Un détail technique pré-build | [architecture/](docs/architecture/README.md), dossier `specs/` |
| Une tâche à prendre | [plan-de-travail.md](docs/gestion-projet/plan-de-travail.md) |
| Une question encore ouverte | [decisions-en-attente.md](docs/gestion-projet/decisions-en-attente.md) et [backlog/](docs/project/backlog/README.md) |
| Tout ce qui touche une fonctionnalité, en un renvoi | [guide-lecture-par-fonctionnalite.md](docs/gestion-projet/guide-lecture-par-fonctionnalite.md) |

---

## 5. Le produit

### Le problème qu'il résout

Un Maître du Jeu (MJ) actif gère simultanément une quantité considérable d'informations : scénarios préparés, PNJ, notes secrètes, informations à partager avec les joueurs, suivi narratif, lore. Cette information est aujourd'hui dispersée sur des outils qui n'ont pas été conçus pour cet usage : fichiers texte ou Word (difficiles à parcourir en session), Notion ou Google Docs (pas pensés pour le JDR), Discord (informations perdues dans le défilement), papier (non cherchable), ou des tables virtuelles comme Roll20 et Foundry (centrées sur le combat, pas sur la narration).

Cette fragmentation casse le rythme de jeu, alourdit la charge mentale de préparation et provoque des oublis en pleine partie.

### Pour qui

Le MJ est l'utilisateur principal : c'est lui qui prépare, choisit les outils et porte la charge organisationnelle. Les joueurs sont des utilisateurs secondaires, qui accèdent aux informations que le MJ choisit de partager. Le détail des profils de référence est porté par les [personas](docs/conception/besoin/persona/README.md).

### Ce qui le distingue

Haversack n'est ni une table virtuelle (Roll20, Foundry) ni un éditeur générique (Notion, Obsidian). C'est un outil pensé spécifiquement pour la préparation narrative et le pilotage de session, avec plusieurs partis pris :

- **Friction d'entrée nulle** — aucun compte requis pour commencer, et les données créées en local restent possédées par l'utilisateur : elles peuvent être exportées dans un format ouvert et consultées hors de l'application.
- **Une vue de session dédiée** — un tableau de bord de conduite en temps réel, plutôt qu'un simple espace de stockage de documents.
- **Agnostique au système de jeu** — aucune mécanique de règles ou de combat n'est imposée ; le contenu se structure librement, quel que soit le système utilisé à table.
- **Partage maîtrisé** — le MJ choisit ce qui est visible aux joueurs, document par document, et garde ses notes de préparation privées par défaut.
- **Accès joueur sans compte** — un lien et un nom d'affichage suffisent pour rejoindre une session.
- **Organisation libre** — le contenu est structuré par le MJ, pas imposé par l'application.

Le raisonnement complet et le positionnement concurrentiel sont dans [vision-produit.md](docs/conception/besoin/vision/vision-produit.md).

---

## 6. Périmètre de la première livraison

La première livraison couvre un parcours cohérent de bout en bout : démarrer sans compte, créer et structurer du contenu dans un espace personnel ou un espace de jeu, organiser ce contenu en dossiers, piloter une session avec création de contenu à la volée, partager sélectivement aux joueurs, laisser un joueur rejoindre sans créer de compte, et migrer vers un compte cloud lorsque le besoin de partage ou de sauvegarde apparaît. L'export d'un espace dans un format ouvert et lisible en dehors de l'application fait également partie de ce périmètre.

Le one-shot (session unique) est utilisable dès cette première livraison ; son parcours de lancement dédié et sa bibliothèque de scénarios réutilisables entre espaces restent hors périmètre initial.

Sont explicitement exclus de cette première livraison, par choix de positionnement plutôt que par oubli : une table virtuelle visuelle, un moteur de règles ou de gestion de combat, l'intelligence artificielle générative, une application desktop avec synchronisation hors ligne, les relations typées entre documents, et les templates communautaires.

Le détail du périmètre et les priorités de chaque capacité sont la propriété exclusive de [moscow.md](docs/conception/besoin/vision/moscow.md) : ce README n'en recopie ni le compte ni le classement. Des arbitrages de périmètre rendus le 2026-10-07 attendent encore d'être reportés dans les sources ; ils sont listés dans le [backlog](docs/project/backlog/README.md).

---

## 7. Concepts et vocabulaire

Ce vocabulaire est nécessaire pour lire n'importe quel autre document du corpus. Chaque concept est défini dans le [glossaire](docs/conception/glossaire.md) et modélisé dans le [domaine](docs/conception/domain/README.md) ; ce qui suit n'en est qu'une introduction.

**Espace** — le conteneur de tout le contenu et, le cas échéant, des membres d'une table de jeu. Un espace a toujours exactement un propriétaire. Il existe trois types d'espace :
- **personnel** — mono-membre, créé automatiquement pour chaque utilisateur, disponible dès le mode local. C'est la zone d'atterrissage par défaut du MJ qui note une idée sans avoir encore de table.
- **campagne** — plusieurs sessions, groupe stable, continuité narrative.
- **one-shot** — une session, sans continuité attendue.

Dans la première livraison, campagne et one-shot ne se distinguent par aucun comportement : c'est le même modèle, avec deux étiquettes.

**Document** — l'unité de contenu unique du système. Tout ce que le MJ crée — un PNJ, un lieu, un scénario, une note, un objet, une fiche de personnage joueur — est un document, composé de blocs de contenu libres. Un type de document est une spécialisation optionnelle qui ajoute des propriétés structurées sans jamais contraindre la structure de base. Un document appartient toujours à un dossier, dans un espace ; il peut référencer d'autres documents.

**Dossier** — l'organisation libre du contenu à l'intérieur d'un espace, renommée et restructurée librement par le MJ.

**Visibilité** — chaque document porte un niveau de visibilité qui détermine qui peut le lire : visible par tous les membres de l'espace, réservé au MJ, ou strictement privé à son auteur. C'est ce mécanisme qui porte le partage sélectif aux joueurs.

**Session** — une séance de jeu, avec un cycle de vie qui va de la session en cours à la session close puis archivée. Pendant qu'elle est en cours, la vue session offre au MJ un tableau de bord de pilotage : contenu épinglé, prise de notes rapide, accès au contenu préparé.

**Membre** — la participation active d'un utilisateur authentifié à un espace, avec un rôle (propriétaire, co-MJ ou joueur).

**Accès invité** — l'accès d'un joueur sans compte à un espace ou à une session, via un lien partagé par le MJ. Limité dans le temps, il peut être converti en accès permanent (un membre) si le joueur crée un compte par la suite.

---

## 8. Sans compte, puis avec un compte

C'est un trait structurant du produit, qui explique une bonne partie de l'architecture technique.

**Mode local, sans compte.** Un MJ ouvre l'application et commence à créer du contenu immédiatement — dans son espace personnel ou dans un espace de jeu qu'il crée. Aucune inscription n'est demandée. Les données sont stockées uniquement dans le navigateur. Ce mode couvre toute la préparation et le pilotage de session côté MJ, mais pas la collaboration : sans compte, il n'existe ni partage aux joueurs, ni synchronisation entre appareils. Le détail de ce mode et de ses limites est dans [UC-01](docs/conception/besoin/usecases/UC-01-mode-local-sans-compte.md).

**Migration vers un compte.** La création de compte n'est jamais imposée au démarrage — elle intervient quand un besoin concret apparaît : partager avec ses joueurs, sécuriser ses données, ou y accéder depuis un autre appareil. Si des données locales existent, l'application les présente explicitement et exige une confirmation avant de les importer, pour éviter de s'approprier par accident les données d'un tiers sur un poste partagé. La migration se fait espace par espace, chacun important intégralement ou pas du tout. Le détail est dans [UC-10](docs/conception/besoin/usecases/UC-10-compte-cloud.md).

**Une fois le compte créé**, le produit distingue un palier gratuit (partage et synchronisation activés, avec un nombre d'espaces et un volume de stockage plafonnés) et un palier payant (mêmes fonctionnalités, sans plafond). Les valeurs exactes vivent dans [vision-produit.md](docs/conception/besoin/vision/vision-produit.md).

---

## 9. Architecture technique

### Pile technologique

| Couche | Technologie |
|---|---|
| Landing page | Angular (SSR / prerender statique) |
| Application web | Angular (SPA) |
| Backend | ASP.NET Core (.NET / C#) |
| Base de données | PostgreSQL |
| ORM | Entity Framework Core |
| Authentification | ASP.NET Identity |

Le raisonnement derrière chaque choix est détaillé dans [stack.md](docs/architecture/stack.md).

### Style d'architecture

Clean Architecture avec inversion des dépendances : le domaine définit les interfaces, l'infrastructure les implémente, la présentation dépend du domaine et de l'application — jamais l'inverse.

Le domaine est modélisé en Domain-Driven Design, en quatre contextes métier plus un noyau partagé :

- **Identity & Access** — comptes authentifiés, tiers d'abonnement, suppression de compte
- **Space Management** — espaces, membres, invitations, accès invités
- **Content Library** — documents, dossiers, types de document
- **Session Conduct** — sessions, tableau de bord de pilotage, notes de session

Les contextes ne dépendent pas les uns des autres ; leurs échanges passent par des identifiants, des événements de domaine ou des contrats applicatifs.

### Structure de la solution

Dans la première livraison, le Domaine et l'Application sont chacun un projet .NET unique : les quatre contextes y sont des frontières logiques (des namespaces), vérifiées par un test d'architecture automatisé. Un contexte peut être extrait en projet séparé si un besoin réel l'exige. L'infrastructure et la présentation sont multi-projets dès le départ. L'état exact de ce qui est tranché est dans [structure-projets.md](docs/architecture/structure-projets.md), qui fait foi ; la décision d'origine est [ADR-008](docs/architecture/decisions/ADR-008-structure-solution.md).

### Le mode local côté client

Le mode local est une couche de persistance TypeScript avec des validations minimales : il n'exécute pas le domaine C# complet. Le domaine serveur reste la source de vérité unique : tout ce que le mode local accepte, le serveur doit pouvoir le revalider. L'ordre de construction est détaillé dans [structure-projets.md](docs/architecture/structure-projets.md).

### Clients futurs

La Clean Architecture permet d'ajouter d'autres clients sans toucher au domaine : PWA installable, wrapper desktop, packaging mobile, client .NET natif. Ils sont hors de la première livraison et documentés comme trajectoire dans [stack.md](docs/architecture/stack.md).

---

## 10. Règles du corpus

**L'ordre d'autorité dit quel document corriger en cas d'écart.** Dans `docs/conception/`, l'artefact le plus en amont fait foi :

**personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire**

- les **use cases** sont la source de vérité du besoin fonctionnel ;
- le **domaine** modélise la résolution du besoin, il ne le dicte pas : s'il contredit un use case, c'est le domaine qu'on corrige ;
- le **glossaire** fait référence pour les noms de termes, pas pour le fond ;
- les documents de consolidation et de pilotage (`docs/context/`, `docs/gestion-projet/`, ce README) ne font jamais foi.

Les autres conventions de tenue sont tranchées par [ADR-019](docs/architecture/decisions/ADR-019-conventions-de-tenue-du-corpus.md) et résumées dans le [guide de conventions, § 8.8](docs/gestion-projet/guide-conventions-et-dod.md) : identifiants, gabarits, renvois, marqueurs, registre des décisions, reste à faire, langue, règles métier, glossaire et flux. Le texte est en français, et tout ce qui touche au code est en anglais.

---

## 11. Vérifier le corpus

Avant de commiter une modification de `docs/`, lancez depuis la racine :

```
dotnet test tools/Tests/CaracterisationOutils.csproj
dotnet run --project tools/VerifCorpus
dotnet run --project tools/VerifMailleAgregat
dotnet run --project tools/VerifCouvertureRecette
```

Les trois premiers doivent passer (code 0). `VerifCouvertureRecette` sort en code 1 tant que des écarts de recette assumés subsistent : il les liste, et seul un écart nouveau est une régression. Le sens de chaque sortie est dans le [guide de conventions, § 8.8](docs/gestion-projet/guide-conventions-et-dod.md). Ces contrôles ne tournent pas encore en CI.

---

## 12. État du projet et travail en cours

**Le code produit n'existe pas encore.** Le dépôt contient la conception et l'outillage qui la vérifie.

Ce qui est ouvert :

- **Le chantier de mise en conformité** ([cadrage](docs/workstreams/conformite-corpus/CADRAGE.md), décidé par l'ADR-020 à l'ADR-022). Il réorganise le corpus en un dossier par couche et applique le backlog d'alignement, en 23 tâches. Il n'est pas commencé.
- **Le backlog d'alignement** ([docs/project/backlog/](docs/project/backlog/README.md)). Le 2026-10-07, une relecture complète du corpus a relevé 118 contradictions et 75 incertitudes. Les arbitrages du porteur du projet et les écarts à corriger forment 92 entrées, à reporter dans les sources puis à verser dans le plan de travail.
- **Les questions ouvertes** ([decisions-en-attente.md](docs/gestion-projet/decisions-en-attente.md)).
- **Les décisions pré-implémentation**, à confirmer à l'entrée en build : la liste est dans [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md).
- **L'identité visuelle** (logo, charte graphique), non définie.
- **Les entretiens terrain**, pas encore menés : les hypothèses de la vision ne sont pas validées par le terrain ([INTERVIEW_GUIDE.md](docs/conception/INTERVIEW_GUIDE.md)).

La sélection des modules du corpus lue par l'outillage d'assistance est dans [docs/corpus.md](docs/corpus.md).

---

## 13. Jalonnement

La construction suit quatre jalons, entrecoupés de deux points de décision qu'aucune CI ne vérifie :

```
J0 (socle)  →  J1 (local-only)  →  [DÉCISION MARCHÉ]  →  J2 (cloud + migration)  →  [VALIDATION JURIDIQUE EU]  →  J3 (partage + temps réel)
```

- **J0 — socle** : confirmation des décisions d'architecture pré-implémentation, squelette du Domaine et de l'Application, test d'architecture en intégration continue.
- **J1 — local-only** : le mode local (projection TypeScript/IndexedDB, interface Angular minimale), sans aucune brique cloud.
- **Décision marché** : conditionne l'engagement de la construction plus lourde du cloud et du temps réel, sur la base de la télémétrie d'usage observée en J1.
- **J2 — cloud et migration** : persistance serveur, authentification, migration des données locales vers le cloud, mécanismes de conformité liés à la suppression de compte.
- **Validation juridique EU** : préalable au lancement commercial en Europe ; elle bloque le lancement, pas la construction.
- **J3 — partage et temps réel** : diffusion en temps réel vers les joueurs et canal d'accès invité.

Le détail de chaque jalon — contenu, préalables bloquants, critères de sortie — est dans [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md), qui fait foi.

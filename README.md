# Haversack

> Outil d'aide à la préparation et à la conduite de parties de jeu de rôle.

Ce dépôt contient la conception complète du produit. Aucune ligne de code n'existe encore — voir [État du projet](#état-du-projet).

---

## Le produit

### Le problème qu'il résout

Un Maître du Jeu (MJ) actif gère simultanément une quantité considérable d'informations : scénarios préparés, PNJ, notes secrètes, informations à partager avec les joueurs, suivi narratif, lore. Cette information est aujourd'hui dispersée sur des outils qui n'ont pas été conçus pour cet usage : fichiers texte ou Word (difficiles à parcourir en session), Notion ou Google Docs (pas pensés pour le JDR), Discord (informations perdues dans le défilement), papier (non cherchable), ou des tables virtuelles comme Roll20 et Foundry (centrées sur le combat, pas sur la narration).

Cette fragmentation casse le rythme de jeu, alourdit la charge mentale de préparation et provoque des oublis en pleine partie.

### Pour qui

Le MJ est l'utilisateur principal : c'est lui qui prépare, choisit les outils et porte la charge organisationnelle. Les joueurs sont des utilisateurs secondaires, qui accèdent aux informations que le MJ choisit de partager. Le détail des profils de référence (MJ aux besoins variés, joueur type) est porté par les personas — voir [docs/conception/besoin/persona/](docs/conception/besoin/persona/README.md).

### Ce qui le distingue

Haversack n'est ni une table virtuelle (Roll20, Foundry) ni un éditeur générique (Notion, Obsidian). C'est un outil pensé spécifiquement pour la préparation narrative et le pilotage de session, avec plusieurs partis pris qui le distinguent de ces alternatives :

- **Friction d'entrée nulle** — aucun compte requis pour commencer, et les données créées en local restent possédées par l'utilisateur : elles peuvent être exportées dans un format ouvert et consultées hors de l'application.
- **Une vue de session dédiée** — un tableau de bord de conduite en temps réel, plutôt qu'un simple espace de stockage de documents.
- **Agnostique au système de jeu** — aucune mécanique de règles ou de combat n'est imposée ; le contenu se structure librement, quel que soit le système utilisé à table.
- **Partage maîtrisé** — le MJ choisit ce qui est visible aux joueurs, document par document, et garde ses notes de préparation privées par défaut.
- **Accès joueur sans compte** — un lien et un nom d'affichage suffisent pour rejoindre une session.
- **Organisation libre** — le contenu est structuré par le MJ, pas imposé par l'application.

Le raisonnement complet et le positionnement concurrentiel détaillé sont dans [vision-produit.md](docs/conception/besoin/vision/vision-produit.md).

---

## Périmètre de la première livraison

La première livraison couvre un parcours cohérent de bout en bout : démarrer sans compte, créer et structurer du contenu dans un espace personnel ou un espace de jeu, organiser ce contenu en dossiers, piloter une session avec création de contenu à la volée, partager sélectivement aux joueurs, laisser un joueur rejoindre sans créer de compte, et migrer vers un compte cloud lorsque le besoin de partage ou de sauvegarde apparaît. L'export d'un espace dans un format ouvert et lisible en dehors de l'application fait également partie de ce périmètre minimal.

Le one-shot (session unique) est utilisable dès cette première livraison sous la forme d'une campagne à session unique ; son parcours de lancement dédié et sa bibliothèque de scénarios réutilisables entre espaces restent hors périmètre initial.

Sont explicitement exclus de cette première livraison, par choix de positionnement plutôt que par oubli : une table virtuelle visuelle, un moteur de règles ou de gestion de combat, l'intelligence artificielle générative, une application desktop avec synchronisation hors ligne, les relations typées entre documents, et les templates communautaires.

Le détail complet du périmètre et les priorités relatives de chaque capacité sont la propriété exclusive de [moscow.md](docs/conception/besoin/vision/moscow.md) — ce document n'en recopie ni le compte ni le classement, qui évoluent indépendamment de ce fichier. Le détail fonctionnel de chaque capacité est dans [usecases/](docs/conception/besoin/usecases/README.md), avec une vue d'ensemble dans [use-cases.md](docs/conception/besoin/usecases/use-cases.md).

---

## Concepts et vocabulaire

Le vocabulaire suivant est nécessaire pour lire n'importe quel autre document du corpus. Chaque concept est défini avec précision dans le [glossaire](docs/conception/glossaire.md) et modélisé dans le [domaine](docs/conception/domain/README.md) ; ce qui suit n'en est qu'une introduction.

**Espace** — le conteneur de tout le contenu et, le cas échéant, des membres d'une table de jeu. Un espace a toujours exactement un propriétaire. Il existe trois types d'espace :
- **personnel** — mono-membre, créé automatiquement pour chaque utilisateur, disponible dès le mode local. Il reçoit tout contenu créé sans espace de jeu explicite : c'est la zone d'atterrissage par défaut du MJ qui note une idée sans avoir encore de table.
- **campagne** — plusieurs sessions, groupe stable, continuité narrative.
- **one-shot** — une session, sans continuité attendue.

Dans la première livraison, campagne et one-shot ne se distinguent par aucun comportement : c'est le même modèle, avec deux étiquettes. Leur différenciation (parcours d'entrée dédié pour le one-shot) est un horizon produit, pas un engagement de cette livraison.

**Document** — l'unité de contenu unique du système. Tout ce que le MJ crée — un PNJ, un lieu, un scénario, une note, un objet, une fiche de personnage joueur — est un document, composé de blocs de contenu libres. Un type de document est une spécialisation optionnelle qui ajoute des propriétés structurées sans jamais contraindre la structure de base. Un document appartient toujours à un dossier, dans un espace ; il peut référencer d'autres documents.

**Dossier** — l'organisation libre du contenu à l'intérieur d'un espace, renommée et restructurée librement par le MJ.

**Visibilité** — chaque document porte un niveau de visibilité qui détermine qui peut le lire : visible par tous les membres de l'espace, réservé au MJ, ou strictement privé à son auteur. C'est ce mécanisme qui porte le partage sélectif aux joueurs.

**Session** — une séance de jeu, avec un cycle de vie qui va de la session en cours à la session close puis archivée. Pendant qu'elle est en cours, la vue session offre au MJ un tableau de bord de pilotage : contenu épinglé, prise de notes rapide, accès au contenu préparé.

**Membre** — la participation active d'un utilisateur authentifié à un espace, avec un rôle (propriétaire, co-MJ, ou joueur).

**Accès invité** — l'accès d'un joueur sans compte à un espace ou à une session, via un lien partagé par le MJ. Limité dans le temps, il peut être converti en accès permanent (un membre) si le joueur crée un compte par la suite.

---

## Utilisation sans compte, puis avec un compte

C'est un trait structurant du produit, qui explique une bonne partie de l'architecture technique décrite plus bas.

**Mode local, sans compte.** Un MJ ouvre l'application et commence à créer du contenu immédiatement — dans son espace personnel ou dans un espace de jeu qu'il crée. Aucune inscription n'est demandée. Les données sont stockées uniquement dans le navigateur. Ce mode couvre toute la préparation et le pilotage de session côté MJ, mais pas la collaboration : sans compte, il n'existe ni partage aux joueurs, ni synchronisation entre appareils. Le détail de ce mode, y compris ses limites (capacité de stockage du navigateur, absence de garantie de conservation, absence de chiffrement au repos) est dans [UC-01](docs/conception/besoin/usecases/UC-01-mode-local-sans-compte.md).

**Migration vers un compte.** La création de compte n'est jamais imposée au démarrage — elle intervient quand un besoin concret apparaît : le MJ veut partager avec ses joueurs, sécuriser ses données, ou y accéder depuis un autre appareil. Si des données locales existent, l'application les présente explicitement (espaces détectés, volume, historique de session) et exige une confirmation avant de les importer — un mécanisme de protection contre l'appropriation accidentelle de données d'un tiers sur un poste partagé. La migration se fait espace par espace, chacun important intégralement ou pas du tout. Le détail du parcours est dans [UC-10](docs/conception/besoin/usecases/UC-10-compte-cloud.md).

**Une fois le compte créé**, le produit distingue un palier gratuit (partage aux joueurs et synchronisation cloud activés, avec un nombre d'espaces et un volume de stockage plafonnés) et un palier payant (mêmes fonctionnalités, sans plafond). Les valeurs exactes de ces paliers sont volatiles et ne sont pas reprises ici : elles vivent dans [vision-produit.md, section sur le modèle de monétisation](docs/conception/besoin/vision/vision-produit.md).

---

## Architecture technique

### Pile technologique

| Couche | Technologie |
|---|---|
| Landing page | Angular (SSR / prerender statique) |
| Application web | Angular (SPA) |
| Backend | ASP.NET Core (.NET / C#) |
| Base de données | PostgreSQL |
| ORM | Entity Framework Core |
| Authentification | ASP.NET Identity |

Le raisonnement derrière chaque choix, ses points forts et ses points faibles assumés, sont détaillés dans [stack.md](docs/architecture/stack.md).

### Style d'architecture

Clean Architecture avec inversion des dépendances : le domaine définit les interfaces (persistance, notifications…), l'infrastructure les implémente, la présentation dépend du domaine et de l'application — jamais l'inverse. Ce principe s'applique quelle que soit la granularité physique des projets.

Le domaine est modélisé en Domain-Driven Design, organisé en quatre contextes métier plus un noyau partagé :

- **Identity & Access** — comptes authentifiés, tiers d'abonnement, suppression de compte
- **Space Management** — espaces, membres, invitations, accès invités
- **Content Library** — documents, dossiers, types de document
- **Session Conduct** — sessions, tableau de bord de pilotage, notes de session

Les contextes ne dépendent pas les uns des autres ; leurs échanges passent par des identifiants, des événements de domaine ou des contrats applicatifs.

### Structure de la solution

Dans la première livraison, le Domaine et l'Application sont chacun un projet .NET unique — les quatre contextes métier y sont des frontières logiques (des namespaces), pas des projets séparés. Cette granularité est un choix délibéré, cohérent avec une équipe restreinte et un produit qui valide encore ses hypothèses avant d'investir dans une isolation complète : un contexte peut être extrait en projet séparé à tout moment si un besoin réel l'exige (montée en équipe, dépendances incompatibles, volume de build). La frontière entre contextes est vérifiée à chaque intégration par un test d'architecture automatisé, plutôt que par la seule discipline de revue.

```
Haversack.Domain            (namespaces : IdentityAccess, SpaceManagement, ContentLibrary, SessionConduct, SharedKernel)
Haversack.Application
Haversack.Infrastructure.Persistence
Haversack.Infrastructure.Notifications
Haversack.Presentation.Api
Haversack.Presentation.Landing
```

L'infrastructure et la présentation, à l'inverse du Domaine et de l'Application, restent multi-projets dès le départ : ce sont des préoccupations techniques distinctes (persistance, notifications temps réel, API, landing page), pas un découpage du domaine.

Le détail complet — nommage, responsabilités, promotion ultérieure d'un contexte en projet séparé — est dans [structure-projets.md](docs/architecture/structure-projets.md), qui fait foi sur ce sujet ; la décision d'origine est tracée dans [ADR-008](docs/architecture/decisions/ADR-008-structure-solution.md).

### Le mode local côté client

Le mode local (navigateur, sans compte) est une couche de persistance TypeScript avec des validations minimales — il n'exécute pas le domaine C# complet. Le domaine serveur reste la source de vérité unique : tout ce que le mode local accepte, le serveur doit pouvoir le revalider, jamais l'inverse. La construction suit un ordre déterminé — domaine et application C# d'abord, puis persistance, puis la projection TypeScript locale, puis l'interface Angular — détaillé dans [structure-projets.md](docs/architecture/structure-projets.md).

### Clients futurs

Au-delà de l'application web, la Clean Architecture permet d'ajouter des projets de présentation supplémentaires sans toucher au domaine. Ceux-ci sont hors de la première livraison mais documentés comme trajectoire possible dans [stack.md](docs/architecture/stack.md) : PWA installable, wrapper desktop natif, packaging mobile via une webview, ou un client .NET natif multi-plateforme.

---

## État du projet

**Le code n'existe pas encore.** Ce dépôt ne contient que la conception du produit : besoin, domaine, architecture, interface. La conception est complète.

Ce qui reste explicitement non défini à ce stade :

- **L'identité visuelle** — logo, slogan et charte graphique ne sont pas définis.
- **Un ensemble de décisions d'architecture portent une réserve** : leur raisonnement et leurs alternatives écartées restent la référence, mais chacune doit être confirmée au démarrage du développement. La liste précise de ces décisions est dans [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md).

Le zoning d'interface — ossature de navigation, choix de structure des écrans, châssis applicatif — et un ensemble de fiches de wireframe basse-fidélité couvrant les surfaces de la première livraison existent dans [docs/conception/interface/](docs/conception/interface/README.md).

---

## Par où commencer

Pour un développeur qui reprend ce projet en vue d'écrire la première ligne de code, l'ordre de lecture conseillé est :

1. **[docs/README.md](docs/README.md)** — la carte du corpus documentaire et la distinction entre documents de référence autoportants et corpus de traçabilité détaillé.
2. **[roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md)** — la séquence de construction : jalons, préalables bloquants, critères de sortie, et les conditions pour passer d'un jalon au suivant.
3. **[guide-conventions-et-dod.md](docs/gestion-projet/guide-conventions-et-dod.md)** — les conventions de code dérivées du corpus de conception et la Definition of Done ; le point d'entrée pour comprendre les critères de complétude d'une tâche sans devoir naviguer tout le corpus.
4. **[structure-projets.md](docs/architecture/structure-projets.md)** et **[ADR-008](docs/architecture/decisions/ADR-008-structure-solution.md)** — la structure concrète des projets .NET, à lire avant d'échafauder la solution.

---

## Organisation du corpus et hiérarchie d'autorité

Le corpus documentaire distingue deux paliers. Le dossier [`docs/context/`](docs/context/README.md) rassemble des documents de référence autoportants — lisibles seuls, exportables hors de ce dépôt, remis tels quels à un tiers. Tous les autres dossiers (`architecture/`, `conception/`, `gestion-projet/`, `securite/`, `test/`…) portent le corpus de traçabilité détaillé dont ces documents de référence sont la consolidation : c'est là que vivent les décisions, leur justification et leur détail. Chaque dossier est propriétaire de son propre index — c'est pourquoi ce README n'en recopie pas le contenu, il y renvoie.

À l'intérieur du corpus de conception spécifiquement ([`docs/conception/`](docs/conception/README.md)), un ordre d'autorité explicite s'applique en cas de conflit entre deux artefacts : **personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire**. L'artefact le plus en amont de cet ordre fait foi. Trois conséquences directes en découlent :

- les **use cases** sont la source de vérité du besoin fonctionnel — tout le reste en dérive et s'y conforme ;
- le **domaine** modélise la résolution du besoin, il ne le dicte pas — s'il contredit un use case, c'est le domaine qui est à corriger ;
- le **glossaire** n'a aucune autorité de fond — c'est un outil de nommage dérivé. Une entrée de glossaire qui contredit un use case est l'entrée à corriger, jamais l'inverse.

Un repreneur qui ignore cet ordre risque de corriger le mauvais document face à une divergence apparente entre deux artefacts.

---

## Jalonnement

La construction suit un axe de quatre jalons, entrecoupé de deux points de décision qui ne sont vérifiables par aucune CI :

```
J0 (socle)  →  J1 (local-only)  →  [DÉCISION MARCHÉ]  →  J2 (cloud + migration)  →  [VALIDATION JURIDIQUE EU]  →  J3 (partage + temps réel)
```

- **J0 — socle** : confirmation des décisions d'architecture pré-implémentation, squelette du Domaine et de l'Application, test d'architecture en intégration continue.
- **J1 — local-only** : le mode local (projection TypeScript/IndexedDB, interface Angular minimale), sans aucune brique cloud.
- **Décision marché** : conditionne l'engagement de la construction plus lourde du cloud et du temps réel, sur la base de la télémétrie d'usage observée en J1.
- **J2 — cloud et migration** : persistance serveur, authentification, migration des données locales vers le cloud, mécanismes de conformité liés à la suppression de compte.
- **Validation juridique EU** : préalable au lancement commercial en Europe — elle bloque le lancement, pas la construction, et peut être instruite en parallèle de J2 et J3.
- **J3 — partage et temps réel** : diffusion en temps réel vers les joueurs et canal d'accès invité.

Le détail de chaque jalon et de chaque point de décision — contenu précis, préalables bloquants, critères de sortie factuels — est dans [roadmap-entree-build.md](docs/gestion-projet/roadmap-entree-build.md), qui fait foi sur ce sujet.

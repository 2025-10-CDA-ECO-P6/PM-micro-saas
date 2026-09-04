# Plan de travail — Haversack

| Champ | Valeur |
|---|---|
| Statut | Décomposition en tranches du périmètre J0-J1, en épiques seules pour J2-J3 |
| Audience | la ou les personnes qui prennent en charge l'implémentation — **une à trois**, seules ou en petit collectif |
| Sources | `docs/gestion-projet/roadmap-entree-build.md`, `docs/gestion-projet/guide-conventions-et-dod.md`, `docs/architecture/structure-projets.md`, `docs/conception/besoin/vision/moscow.md`, les fiches `usecases/UC-NN`, `user-stories/US-UC-NN`, `docs/conception/domain/*.md`, `docs/architecture/decisions/ADR-NNN`, `docs/architecture/specs/*.md`, `docs/conception/interface/wireframes/**`, `docs/test/cahier-strategie-test-et-recette.md` |

---

## Bandeau de cadrage — ce que ce document fait et ne fait pas

Ce document **décompose** un ordonnancement déjà arrêté. Il ne redéfinit :
- **ni le périmètre MoSCoW** ([`vision/moscow.md`](../conception/besoin/vision/moscow.md) — seule autorité du corpus pour attribuer une priorité Must/Should/Could/Won't) ;
- **ni le phasage J0-J3** ([`roadmap-entree-build.md §1-2`](roadmap-entree-build.md)) ;
- **ni les critères de sortie de jalon** ([`roadmap-entree-build.md §3`](roadmap-entree-build.md), [`cahier-strategie-test-et-recette.md §9`](../test/cahier-strategie-test-et-recette.md)) ;
- **ni l'ordre des user stories à l'intérieur d'une epic** (section « Ordre de livraison recommandé » de chaque fichier `US-UC-NN-*.md`).

**En cas de conflit entre ce document et une source citée** (roadmap, ADR, use case, user story, spécification), **la source citée fait foi**. Ce document est un artefact de décomposition, pas une nouvelle autorité de corpus.

Ce que ce document **fait** : il prend la séquence de jalons J0-J3 et, pour J0 et J1, la décompose jusqu'à la tranche — but observable, dépendances techniques, périmètre d'écriture, conflits calculés, taille, critères d'acceptation par renvoi. Pour J2 et J3, il s'arrête au niveau de l'épique (raison en §7). Il regroupe les tranches ouvrables simultanément en lots parallélisables, calculés depuis les périmètres d'écriture déclarés — jamais affirmés.

**Périmètre engagé, non redéfini.** Ce plan engage les user stories que ses tranches citent — ni plus, ni moins ; ce périmètre se lit sur les tranches elles-mêmes, pas sur un décompte figé dans ce bandeau. Une user story du corpus J1 qu'aucune tranche ne cite reste hors périmètre engagé ; son statut se lit au grain du use case dont elle dérive — au paragraphe `Critère de sortie` du use case concerné dans [`moscow.md`](../conception/besoin/vision/moscow.md), seule autorité du corpus pour cette priorité, et aux critères de sortie factuels du jalon « Local-only » ([`roadmap-entree-build.md §3.2`](roadmap-entree-build.md)) — et à la plage `CR-` que ce use case couvre dans [`cahier-strategie-test-et-recette.md §9`](../test/cahier-strategie-test-et-recette.md), jamais à une valeur MoSCoW ou à un critère de sortie recopiés dans ce document. Le critère fonde l'appartenance au périmètre local ; la plage `CR-` l'énumère et porte déjà ses colonnes de verdict — c'est à qui clôture J1 (`TB-094`) de trancher, cas par cas, si une plage relève du périmètre local.

**Hors périmètre** : durée, effectif, affectation nominative — aucun de ces éléments n'est un fait dérivable du corpus de conception, qui ne porte ni charge ni ordonnancement temporel propre.

---

## 1. Comment ce plan se lit

Cinq règles opposables, quel que soit l'effectif qui exécute ce plan :

**(a) Aucune tranche ne porte de « qui » ni de « quand ».** Une tranche est définie par son but observable, ses dépendances techniques et son périmètre d'écriture — jamais par une charge, une durée ou un nom de personne. Ce plan reste valide qu'il soit exécuté par une personne seule ou un petit collectif.

**(b) L'ordre d'une tranche est celui de ses dépendances techniques, rien d'autre.** Le champ `Dépend de` liste uniquement les tranches dont le résultat est un préalable technique — jamais une hypothèse sur qui ferait la tranche, sur l'effectif disponible ou sur une commodité d'organisation.

**(b bis) Ce que la dépendance n'ordonne pas, l'epic le départage.** Deux tranches qui partagent un module sans dépendre l'une de l'autre doivent être sérialisées, mais aucune raison technique ne dit dans quel ordre. Leur ordre de sérialisation suit alors la section « Ordre de livraison recommandé » de l'epic qui les porte, **citée en renvoi sur la ligne concernée**. Cette règle ne classe rien par la valeur : elle nomme la source qui départage ce que la dépendance laisse indifférent.

**Dispositif de départage entre deux tranches que la dépendance ne départage pas.** Deux situations y entrent, dans les deux cas parce qu'aucune dépendance technique ne les départage : deux tranches **en conflit** sans dépendance mutuelle, qu'il faut sérialiser sans qu'aucune raison technique ne dise dans quel ordre ; et deux tranches **sans conflit**, simultanément ouvrables, qu'aucune dépendance ne préfère à l'autre — il n'y a alors rien à sérialiser, mais une tranche à prendre en premier. Dans les deux cas, ce qui décide est le même :

1. d'abord la section « Ordre de livraison recommandé » de l'épique, là où un use case la porte — règle (b bis) ;
2. à défaut, le choix libre, déclaré comme tel : aucune source du corpus ne départage plus. Ce n'est pas un critère de choix : c'est l'absence d'un critère, nommée comme telle.

Ce dispositif s'applique au **couple de tranches comparé**, jamais au lot (§9) — un même lot peut regrouper des tranches de plusieurs épiques (§1), et rien n'y loge un renvoi d'ordre.

**(c) `En conflit avec` est calculé depuis `Périmètre d'écriture`, jamais affirmé.** Deux tranches sont en conflit si elles déclarent au moins un module commun ; le module partagé est nommé en clair sur la ligne. Un recouvrement que la maille ne sait pas décider porte la mention `recouvrement à confirmer à J0` avec sa raison — il n'est jamais affirmé sans preuve.

**(d) La largeur d'un lot est son nombre de tranches, lisible sur place.** Aucun décompte n'est écrit dans ce document. Compter les lignes d'un lot dit sa largeur ; l'écrire ailleurs créerait un nombre qui dérive au premier ajout.

**Règle de lecture pour un effectif variable** : une personne de plus qu'il n'y a de tranches ouvrables dans un lot attend la levée d'une dépendance ; elle ne redécoupe pas le lot. Le découpage est fixé par ce document, pas par l'effectif du jour. **La largeur réellement offerte par le graphe est une propriété mesurable de ce document, pas une promesse** — elle se remesure en relançant le calcul des lots (§9) après toute modification.

**Deux natures de tranche, et c'est ce qui rend chacune clôturable.** Le corpus attache ses critères à deux grains distincts, et les confondre produit des tranches qu'on ne peut pas fermer :

| | Périmètre d'écriture | Critères d'acceptation | Ne cite jamais |
|---|---|---|---|
| **Tranche de comportement** | un ou plusieurs agrégats, ou un module transverse nommé | règles métier `RB-nn-kk`, invariants du modèle de domaine, spécifications | un scénario Gherkin, un cas de recette `CR-` |
| **Tranche de surface** | une ou plusieurs fiches d'écran | scénarios Gherkin nommés, sous-plage de cas de recette, fiche de wireframe, arbitrages de zoning | — |

Une règle métier se prouve sur l'agrégat ; un scénario Gherkin décrit un comportement **observable**, donc atteignable seulement à travers un écran. Une tranche de comportement qui citerait un scénario Gherkin serait faisable mais non clôturable. **Ce contrôle est mécanique, et sa forme est définie ici pour être opposable** : aucune tranche sans fiche d'écran à son périmètre ne cite de **scénario Gherkin** ni de **cas de recette**.

- Un **scénario Gherkin** est cité sous l'une de ces deux formes exactement : `US-NN-KK §"<titre du scénario>"`, ou `US-NN-KK, ses scénarios nommés` (`leurs scénarios nommés` au pluriel).
- Un **cas de recette** est cité par un identifiant `CR-UCNN-KK`, seul ou en plage.
- Un renvoi vers une **section** d'une fiche de user story — de la forme `` `user-stories/US-UC-NN-....md § US-NN-KK § <section>` `` — n'est ni l'un ni l'autre : il désigne un passage du document, pas un comportement à prouver. Une tranche de comportement peut donc le citer.

**La verticalité se ferme à l'épique.** Une tranche de surface et les tranches de comportement dont elle dépend forment ensemble le chemin complet — domaine, application, persistance, écran. C'est l'épique qui se ferme, pas la tranche isolée.

**Épique et lot parallélisable sont deux notions distinctes.** Une **épique** est un regroupement de travail qui concourt au même but fonctionnel ou technique. Un **lot parallélisable** est un ensemble de tranches **ouvrables simultanément aujourd'hui**, calculé depuis les dépendances closes et l'absence de conflit de périmètre. Une même épique peut répartir ses tranches sur plusieurs lots ; un même lot peut regrouper des tranches de plusieurs épiques.

**Deux natures d'épique.** Là où un use case du corpus porte le regroupement, l'épique **est** l'epic existante du corpus — son identifiant `US-UC-01` à `US-UC-14`, portée par le fichier `docs/conception/besoin/user-stories/US-UC-NN-*.md`. Aucun identifiant neuf n'est créé pour ce cas. Là où aucun use case ne porte le regroupement, l'épique porte un identifiant `EP-nn` propre à ce document.

---

## 2. Maille de désignation du périmètre d'écriture

Le champ `Périmètre d'écriture` de chaque tranche est une liste de **modules**, séparés par `;`. Un module est exactement l'une de ces natures, et rien d'autre :

- un **agrégat du modèle de domaine**, désigné par son nom, parmi ceux que [`docs/conception/domain/`](../conception/domain/README.md) nomme en sections propres — `## Agrégats` dans trois fichiers de contexte, `## Agrégat unique : User` dans le quatrième. Les entités et value objects internes à un agrégat (`### X (entité dans Y)`, `### X (value object dans Y)`) n'en sont pas : **un agrégat est la frontière transactionnelle du modèle**, donc l'unité d'écriture ;
- une **fiche d'écran**, par son slug, parmi celles de [`docs/conception/interface/wireframes/`](../conception/interface/wireframes/README.md) ;
- un **module transverse nommé** : le noyau partagé `SharedKernel` ([`structure-projets.md §4`](../architecture/structure-projets.md)), `Haversack.Infrastructure.Notifications`, un projet .NET désigné par son rôle tant que son nom n'est pas tranché ([`structure-projets.md §3 § Convention de désignation`](../architecture/structure-projets.md)), ou — **sur la seule tranche qui les crée** — l'un des quatre namespaces de bounded context `IdentityAccess`, `SpaceManagement`, `ContentLibrary`, `SessionConduct` ;
- un **module de la couche cliente**, parmi les six que le corpus nomme par leur préoccupation — jamais par un répertoire, dont l'arborescence reste `[À TRANCHER — J0]` ;
- un **module d'outillage**, désigné par son rôle selon la même convention que les projets .NET dont le nom n'est pas tranché : `le projet de test d'architecture`, `le projet de test de bout en bout`, `la configuration de la solution`, `la configuration d'intégration continue`.

**Les six modules de la couche cliente et leur ancrage.** [`structure-projets.md §7`](../architecture/structure-projets.md) renvoie l'emplacement de cette couche à l'ouverture de J0 et n'en nomme aucun répertoire. Ce document ne tranche pas cette arborescence : il désigne les modules par la **préoccupation** que le corpus leur donne déjà ailleurs.

| Module | Où le corpus le nomme |
|---|---|
| `service d'accès au store` | [`roadmap-entree-build.md Annexe A`](roadmap-entree-build.md), ligne J1, code `P6` — « Services Angular IndexedDB (wrappers) » |
| `châssis` | [`zoning.md §S7 — Châssis`](../conception/interface/zoning.md) |
| `bandeaux transversaux` | [`roadmap-entree-build.md Annexe A`](roadmap-entree-build.md), code `P6` — « bandeaux durabilité/confidentialité » |
| `sanitisation` | [`roadmap-entree-build.md Annexe A`](roadmap-entree-build.md), code `P6` — « `DomSanitizer` » ; [`specs/sanitisation-csp.md`](../architecture/specs/sanitisation-csp.md) |
| `politique de sécurité de contenu` | [`roadmap-entree-build.md Annexe A`](roadmap-entree-build.md), code `P6` — « CSP complète » ; [`specs/sanitisation-csp.md`](../architecture/specs/sanitisation-csp.md) |
| `projection d'export` | [`ADR-017 § Conséquences`](../architecture/decisions/ADR-017-modele-indexeddb-local.md) — « Couture de projection filtrée, sans reformatage structurel » |

*Le bandeau de durabilité et le bandeau de confidentialité partagent un seul module, comme le corpus les nomme d'un seul trait. Ils sont donc déclarés en conflit et sérialisés ; leur épique n'étant pas portée par un use case, le premier point du dispositif de départage (§1) ne s'y applique pas, et c'est son second point qui s'applique : l'ordre est laissé à qui les prend.*

**L'agrégat couvre ses deux moitiés.** Déclarer `Space` couvre l'agrégat côté domaine **et** sa persistance côté client : le store local `spaces` **est** l'agrégat `Space` persisté, pas un second objet. Les douze object stores nommés par [`ADR-017 §1.1`](../architecture/decisions/ADR-017-modele-indexeddb-local.md) se rattachent chacun à exactement un agrégat, et ce rattachement se lit sur le tableau de cette section — il n'est pas recopié depuis l'ADR, il en est dérivé :

| Agrégat | Object stores qu'il couvre |
|---|---|
| `Space` | `spaces` |
| `Folder` | `folders` |
| `Document` | `documents`, `document_blocks`, `document_links`, `document_tags` |
| `DocumentType` | `document_types` |
| `Session` | `sessions`, `session_pinned_documents`, `session_live_notes` |
| `SessionViewConfig` | `session_view_configs`, `session_view_folders` |

*Deux rattachements sont une lecture et non un calcul* : `session_pinned_documents` et `session_live_notes` sont des stores de jonction entre `Session` et `Document` ; `ADR-017 §1.1` les décrit comme des « références » portées par la session, ce que ce tableau suit. À confirmer à J0.

**Aucun namespace de bounded context n'apparaît comme module**, sauf sur la seule tranche qui les crée (TB-003) : un namespace est un tiroir, pas une unité d'écriture. Deux agrégats du même namespace — `Document` et `Folder` dans `ContentLibrary` — sont deux modules distincts, et deux tranches qui les écrivent ne sont pas en conflit.

**Aucun chemin de fichier n'apparaît.** [`structure-projets.md §3`](../architecture/structure-projets.md) écrit que « le contenu de chacun des cinq namespaces — fichiers, classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document », et marque ces lignes `[À TRANCHER — J0]`. Un plan qui énumérerait des fichiers trancherait à la place du build. **La maille par agrégat ne dépend pas de cette décision** : un agrégat est nommé et stable indépendamment de sa future disposition en fichiers.

**Le point aveugle de cette maille, et sa parade — tranchée.** Une maille par agrégat ne voit pas les **points de composition** — enregistrement des cas d'usage de la couche Application, table de routage, déclaration du schéma du store — qui n'appartiennent à aucun agrégat. Deux tranches déclarées sans conflit s'y rencontreraient, et l'erreur cesserait d'aller dans le sens sûr.

La parade est tranchée et vit à sa source : [`structure-projets.md §3 § Points de composition : un par agrégat`](../architecture/structure-projets.md) porte ces points **par agrégat** plutôt qu'en fichier commun. Côté client, la même propriété découle du contrat d'accès au store, un service par agrégat ([`structure-projets.md §7`](../architecture/structure-projets.md)). Il ne subsiste donc aucun fichier écrit par toutes les tranches, et la garantie de non-recouvrement que ce document énonce est vraie.

**Deux marqueurs, à ne pas confondre.**

- **`HORS MAILLE`** — la tranche ne vise **aucun module de code** : confirmer un ADR, retenir un outil, arrêter une convention, renseigner une colonne de verdict. Ce n'est pas un manque du corpus, c'est la maille qui ne parle que de code. Une tranche `HORS MAILLE` n'a pas de conflit d'écriture par construction, et **satisfait la cinquième condition d'entrée** ([`methode-de-ticket.md §2`](methode-de-ticket.md)) au même titre qu'un module nommé.
- **`TROU`** — la tranche vise un module de code que le corpus ne nomme pas ; le marqueur reste défini pour le cas où une tranche future en aurait besoin.

Le marqueur `TROU` s'applique de la même façon aux **critères d'acceptation** : il y signale une absence réelle du corpus source ; le marqueur reste défini pour le cas où une tranche future en aurait besoin sur ce champ. Le périmètre d'écriture et les critères d'acceptation sont deux conditions d'entrée distinctes : nommer un module ne fournit pas un critère.

---

## 3. Échelle de taille

La taille d'une tranche mesure sa **surface de vérification** — combien de choses distinctes il faudra prouver, et à combien d'endroits — jamais sa **difficulté**. Une tranche `L` n'est pas une tranche dure ; c'est une tranche dont la preuve de complétude touche plusieurs modules ou s'appuie sur de nombreux renvois.

**Cette échelle est un signal de découpage, rien d'autre : elle ne sert ni à remplir une itération, ni à ordonner, ni à estimer.** Une tranche cotée `M` peut à elle seule porter la clôture de tout un jalon ; lire une capacité sur cette échelle la rangerait dans le même seau qu'une tranche à trois renvois, ce qu'elle n'est pas. Ce que la taille dit de la tranche ne dit rien de sa place dans une itération — ce sujet s'inscrit ailleurs.

| Taille | Critère |
|---|---|
| **S** | un seul module au périmètre, et au plus 2 renvois de critère d'acceptation |
| **M** | un seul module et 3 à 6 renvois ; ou deux modules et au plus 2 renvois |
| **L** | plus de deux modules, ou plus de 6 renvois, ou deux modules avec plus de 2 renvois |

**Un agrégat et sa persistance locale comptent pour un seul module** (§2) : le store local d'un agrégat n'est pas un second module. Sans cette lecture, presque toute tranche verticale vaudrait `L` et l'échelle cesserait de discriminer.

**Une tranche `L` porte, sur la même ligne que sa taille, la raison pour laquelle elle n'est pas scindée** — sauf lorsque son périmètre ne nomme aucun module (marqueurs `HORS MAILLE` et `TROU`, §2), auquel cas la taille se lit sur le seul décompte de renvois.

**Unité de comptage d'un renvoi.** Un renvoi est un **segment du champ `Critères d'acceptation` séparé par un point-virgule**. Deux règles à l'intérieur d'un segment :
- **une plage explicite** (`CR-UC07-01 → CR-UC07-09`, `RB-06-18 → RB-06-21`) compte pour **son étendue** — jamais pour un ;
- **tout autre segment** compte pour **un**, quel que soit le nombre d'éléments qu'il énumère.

Citer `RB-02-01 → RB-02-20` engage réellement vingt preuves distinctes à la clôture, pas une — c'est exactement ce que l'échelle mesure.

**Une plage citée ne doit englober aucun cas retiré.** Les sous-plages de cas de recette de ce document sont **dérivées de la colonne `Source`** du cahier de recette, qui nomme pour chaque cas la user story dont il dérive : une plage ainsi dérivée ne peut pas englober un cas retiré, contrairement à une plage numérique fermée à la main.


---

## 4. Vue des épiques

Colonne `Dépend de` : épiques précédentes dont au moins une tranche de l'épique courante dépend techniquement. Colonne `Tranches` : identifiants rattachés à cette épique dans les sections J0/J1 ; `—` pour une épique J2/J3 (niveau épique seul, §7-8).

### J0

| Épique | But | Jalon | Dépend de | Tranches |
|---|---|---|---|---|
| EP-01 — Confirmation des décisions pré-implémentation | chaque ADR pré-implémentation ou mixte reçoit sa validation de décideur, datée | J0 | — | TB-001 |
| EP-02 — Échafaudage de la solution .NET | les projets .NET existent, s'assemblent, et portent les abstractions communes du Domaine | J0 | EP-01 | TB-002, TB-003, TB-004, TB-005 |
| EP-03 — Contrôle d'architecture en intégration continue | toute violation de frontière entre bounded contexts est détectée et bloquante, sans intervention humaine | J0 | EP-02 | TB-006, TB-007, TB-008, TB-009, TB-095 |
| EP-04 — Contrat d'autorisation en couche Application | le contrat d'autorisation applicative est déclaré, puis câblé aux contrats Application avant l'ouverture de J1 | J0-J1 | EP-02 | TB-010 |
| EP-05 — Conventions outillées et socle d'ingénierie | les conventions de format, de style et de commit/branche sont arrêtées et opposables en revue | J0 | EP-02 | TB-011, TB-012 |

### J1

| Épique | But | Jalon | Dépend de | Tranches |
|---|---|---|---|---|
| EP-04 — Contrat d'autorisation en couche Application | le contrat d'autorisation applicative est déclaré, puis câblé aux contrats Application avant l'ouverture de J1 | J0-J1 | EP-02 | TB-080 |
| EP-29 — Couture cliente et socle du store local | un projet Angular existe et son build réussit, le contrat du service d'accès au store est déclaré et satisfait par une doublure, et le store local s'ouvre, s'indexe et se versionne | J1 | EP-02 | TB-056, TB-057, TB-104 |
| EP-10 — Châssis applicatif transverse | indicateurs, bandeaux, accès compte et accessibilité transversale sont présents sur toute surface MJ | J1 | EP-29 | TB-058 |
| US-UC-01 — Mode local sans compte | le MJ commence sans compte depuis l'écran d'accueil, retrouve ses données au retour — ou en est informé si elles ont disparu — et voit les fonctionnalités cloud inaccessibles sans compte | J1 | EP-10, EP-08, US-UC-02, EP-29 | TB-079, TB-101, TB-102, TB-103 |
| US-UC-02 — Créer un espace de jeu | nommer et instancier un espace partagé, avec la propriété automatique de l'espace personnel | J1 | EP-29, EP-10 | TB-059, TB-060, TB-061, TB-062 |
| US-UC-05 — Organiser par dossiers | chaque espace dispose de son arborescence de dossiers par défaut, système-agnostique | J1 | US-UC-02 | TB-063, TB-064 |
| US-UC-04 — Gérer les documents d'un espace | un document se crée, se retrouve et se modifie, avec une visibilité privée par défaut | J1 | US-UC-05 | TB-065, TB-066, TB-067, TB-068 |
| US-UC-03 — Structurer un scénario | un scénario se crée avec son titre seul et contient zéro à N scènes ordonnées | J1 | US-UC-04 | TB-069, TB-070, TB-099, TB-100 |
| US-UC-06 — Utiliser la vue session | conduire une partie hors ligne depuis un hub unique — scènes, notes, contenu mis en avant | J1 | US-UC-02, US-UC-04 | TB-071, TB-072, TB-073, TB-074, TB-075 |
| US-UC-07 — Créer un élément à la volée | improviser un document en cours de partie, sans rupture de contexte | J1 | US-UC-06, US-UC-04 | TB-076, TB-077 |
| US-UC-14 — Rechercher et filtrer l'information | un MJ retrouve un document de son espace par titre, depuis la préparation et depuis la vue session — où les documents liés à la session active remontent en tête des résultats | J1 | EP-29, US-UC-05, US-UC-04, US-UC-06 | TB-078, TB-096, TB-097, TB-098 |
| EP-30 — Validations minimales du mode local | le mode local refuse un titre vide et une structure de blocs invalide, sans porter aucune règle que le domaine serveur n'a pas | J1 | EP-29, US-UC-04 | TB-081 |
| EP-08 — Durabilité et sécurité du mode local | `navigator.storage.persist()` est traité comme état de première classe ; aucun contenu ne peut déclencher l'exécution de code | J1 | EP-29, EP-10 | TB-082, TB-083, TB-084, TB-085, TB-086 |
| EP-09 — Export d'espace, version minimale | un export au format `schemaVersion` se produit depuis le store local et est structurellement rejouable | J1 | EP-29, US-UC-06 | TB-087, TB-088, TB-089, TB-090, TB-091 |
| EP-11 — Instrumentation de validation du MVP | l'application constate, de façon anonyme, l'activation de chacun des piliers | J1 | EP-04, US-UC-06, EP-10 | TB-092, TB-093 |
| EP-12 — Recette et clôture du jalon J1 | chaque cas de recette du périmètre local porte un verdict | J1 | l'ensemble des épiques J1 ci-dessus | TB-094 |

### J2 — niveau épique seul (raison en §7)

| Épique | But | Jalon | Dépend de | Plage de tâches |
|---|---|---|---|---|
| EP-13 — Promotion de schéma `LIVE_NOTE` | `characterId` et `guestAccessId` quittent `documents.properties` pour des colonnes nullable indexées de premier niveau | J2 | EP-12 | — |
| EP-14 — Mapping EF Core et migrations SQL | Le modèle de domaine est persisté en PostgreSQL, avec converters d'IDs typés et FK `ON DELETE RESTRICT` | J2 | EP-13, EP-02 | — |
| EP-15 — Query filters globaux d'autorisation | Les prédicats de suppression logique sont portés par des query filters EF Core globaux, actifs sur tous les chemins de lecture | J2 | EP-13, EP-14 | — |
| EP-16 — Autorisation API câblée | Le pipeline behavior REST appelle le même `IResourceAccessPolicy.CanAccess(...)` que le futur filtre SignalR | J2 | EP-04, EP-13 | — |
| EP-17 — Authentification et rotation de clé | Politique de mot de passe, rate limiting hybride, cycle de vie des tokens et rotation de clé sont en place | J2 | — | — |
| EP-18 — Reprise de compte par preuve fédérée, conditionnelle | traiter la collision entre une inscription fédérée et une coquille de compte pas encore confirmée | J2 | EP-17 | — |
| EP-19 — Handler d'import et migration | Un export J1 se migre de bout en bout : dry-run, rapport de rejets, import transactionnel par espace, idempotence | J2 | EP-14, EP-16, EP-09 | — |
| EP-20 — Suppression d'espace et sagas RGPD | La saga `SpaceDeleted` et la saga `UserAnonymized` s'exécutent sans erreur de cycle FK | J2 | EP-14 | — |
| EP-21 — Données invité sur espace vivant | `guest_accesses.display_name` reste hors purge sur un espace vivant | J2 | EP-20 | — |
| EP-22 — Contrat OpenAPI des endpoints d'authentification | Les codes 400/401/403/429 sont spécifiés, la non-révélation d'existence est appliquée | J2 | EP-17 | — |
| EP-23 — Tests de sécurité et IDOR | Un test IDOR de référence et des tests brute-force/replay/rotation couvrent le périmètre auth | J2 | EP-15, EP-16, EP-17 | — |
| US-UC-10 — Créer un compte cloud et migrer | faire basculer un usage local vers le cloud, sans perte, en débloquant le partage | J2 | EP-17, EP-19 | — |


### J3 — niveau épique seul (raison en §8)

| Épique | But | Jalon | Dépend de | Plage de tâches |
|---|---|---|---|---|
| EP-24 — Runtime SignalR | Groupes, révocation en session, reconnexion ; transport forcé en SSE tant que la diffusion reste unidirectionnelle | J3 | EP-16 | — |
| EP-25 — Authentification du canal invité | Le token `GuestAccess` transite hors query-string ; déconnexion forcée du hub sur révocation/expiration | J3 | EP-24, EP-17 | — |
| EP-26 — Anti-fuite par visibilité | Chaque événement poussé est filtré selon `PUBLIC`/`PLAYER_PRIVATE`/`GM_ONLY` | J3 | EP-24, EP-25 | — |
| EP-27 — Repli SSE vers polling adaptatif | Un mécanisme de repli existe, piloté par un seuil de coût par session concurrente | J3 | EP-24 | — |
| US-UC-08 — Partager une information aux joueurs | diffuser un contenu choisi vers les joueurs, consultable sans qu'ils s'inscrivent | J3 | EP-24, EP-26 | — |
| US-UC-09 — Accès joueur sans compte | entrer dans une session par un simple lien, sans inscription préalable ni identifiant durable | J3 | EP-24, EP-26 | — |
| US-UC-12 — Consulter son espace en tant que joueur | offrir au joueur membre une surface de consultation propre, distincte de celle du MJ | J3 | US-UC-09 | — |
| US-UC-11 — Gérer les membres et invitations | distinguer l'invitation durable d'un membre du lien ponctuel d'une session | J3 | EP-16, EP-24 | — |
| EP-28 — Recette du périmètre J3 | Chaque cas de recette des UC-08, 09, 11, 12 porte un verdict, ainsi que le résiduel joueur/partage d'UC-06 (`CR-UC06-19 → CR-UC06-24`) | J3 | US-UC-08, US-UC-09, US-UC-11, US-UC-12 | — |


---

## 5. J0 — épiques et tranches

### EP-01 — Confirmation des décisions pré-implémentation

##### TB-001 — Confirmer les ADR de nature pré-implémentation ou mixte

| Champ | Valeur |
|---|---|
| But | chaque ADR de nature pré-implémentation ou mixte porte, dans son en-tête, une ligne de confirmation datée et signée d'un décideur |
| Épique | EP-01 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | HORS MAILLE — nature : acte de confirmation documentaire sur les ADR eux-mêmes — ne vise aucun module de code |
| En conflit avec | — |
| Taille | M — 0 modules, 3 renvois |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 1 ; `cahier-strategie-test-et-recette.md §9`, ligne « Socle », puce 1 ; la liste des ADR concernés se lit sur `architecture/decisions/README.md § Index`, colonne « Nature » (valeurs « pré-implémentation » et « mixte ») — non recopiée ici, pour ne pas dupliquer un décompte que ce registre possède et fait évoluer |
| Code de renvoi | — |

### EP-02 — Échafaudage de la solution .NET

##### TB-002 — Créer la solution et ses projets

| Champ | Valeur |
|---|---|
| But | `dotnet build` de la solution réussit sur une solution vide de logique métier |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-001 |
| Périmètre d'écriture | le projet Domaine unique ; le projet Application unique ; Haversack.Infrastructure.Notifications ; les autres projets d'Infrastructure et de Présentation, dont le nombre et les noms relèvent de J0 |
| En conflit avec | TB-010 — module partagé : le projet Application unique ; TB-080 — module partagé : le projet Application unique ; TB-092 — module partagé : le projet Application unique |
| Taille | L — les projets sont échafaudés en un seul geste solidaire (une solution .NET unique) ; les scinder romprait l'unité de la structure décrite par `structure-projets.md`. Cette tâche demeure le **premier livrable de structure** (`ADR-008 §Compléments post-revue`, non amendé) |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 2 |
| Code de renvoi | — |

##### TB-003 — Poser les namespaces du Domaine

| Champ | Valeur |
|---|---|
| But | le projet Domaine unique contient les cinq sous-namespaces et rien d'autre |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | IdentityAccess ; SpaceManagement ; ContentLibrary ; SessionConduct ; SharedKernel |
| En conflit avec | TB-004 — module partagé : SharedKernel ; TB-005 — module partagé : SharedKernel |
| Taille | L — les namespaces sont posés en un seul geste de structuration du Domaine ; leur contenu interne reste `[À TRANCHER — J0]`, ce qui interdit de scinder cette tâche plus finement aujourd'hui |
| Critères d'acceptation | `structure-projets.md § Domaine et Application : projets uniques` ; `guide-conventions-et-dod.md §2 § Organisation par namespaces par bounded context` |
| Code de renvoi | — |
| Borne du critère | le critère se limite à l'existence des cinq répertoires, et c'est une borne assumée, non un manque : `structure-projets.md` marque les cinq lignes `[À TRANCHER — J0]` et écrit que « le contenu de chacun des cinq namespaces — fichiers, classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document ». Cette tranche se clôt sur l'existence des répertoires. |
| Note de maille | cette tâche est la seule du plan à déclarer des **namespaces** et non des agrégats : elle crée les tiroirs, pas leur contenu. Les agrégats qu'ils accueilleront sont déclarés par les tranches de J1 (§2). |

##### TB-004 — Implémenter les abstractions du `SharedKernel`

| Champ | Valeur |
|---|---|
| But | `Entity`, `AggregateRoot`, `DomainEvent`, `AuditInfo`, `SoftDelete`, `IRepository<T>` et les exceptions métier existent et compilent |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-003 |
| Périmètre d'écriture | SharedKernel |
| En conflit avec | TB-003 — module partagé : SharedKernel ; TB-005 — module partagé : SharedKernel |
| Taille | M — un module, 5 renvois |
| Critères d'acceptation | `structure-projets.md §4 — Nommage du noyau partagé` ; `conception/domain/core.md § Abstractions DDD` ; `conception/domain/core.md § Value objects primitifs` ; `conception/domain/core.md § Primitives de traçabilité` ; `guide-conventions-et-dod.md §2` |
| Code de renvoi | — |

##### TB-005 — Implémenter les IDs typés du Core

| Champ | Valeur |
|---|---|
| But | aucune signature métier n'accepte un `Guid` nu ; une inversion de deux identifiants ne compile pas |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-004 |
| Périmètre d'écriture | SharedKernel |
| En conflit avec | TB-003 — module partagé : SharedKernel ; TB-004 — module partagé : SharedKernel |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `conception/domain/core.md § IDs typés` ; `guide-conventions-et-dod.md §2` ; `architecture/specs/mapping-ef-core.md §1` |
| Code de renvoi | — |

### EP-03 — Contrôle d'architecture en intégration continue

##### TB-006 — Écrire le test d'architecture des frontières de namespace (règle 2)

| Champ | Valeur |
|---|---|
| But | le test échoue si un type d'un bounded context dépend d'un type d'un autre bounded context sans autorisation, et passe sinon — quelle que soit la forme de la dépendance |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | TB-003 |
| Périmètre d'écriture | le projet de test d'architecture |
| En conflit avec | TB-009 — module partagé : le projet de test d'architecture ; recouvrement à confirmer à J0 — TB-009 écrit dans le même projet de test d'architecture, non nommé par le corpus, donc non déclarable comme module au sens de la maille (§2). Recouvrement à confirmer à J0. |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `guide-conventions-et-dod.md §6 § Dérivable du corpus`, règle 2 — « chaque contexte respecte les frontières logiques définies par les namespaces » ; `guide-conventions-et-dod.md §6 § Non couvert par le corpus`, puce « Règle 2 — NetArchTest » — le contrôle porte sur le graphe des types de l'assemblage compilé, non sur le texte source ; `cahier-strategie-test-et-recette.md §3.4 — Test d'architecture (CI)` |
| Code de renvoi | — |

##### TB-007 — Retenir les outils du contrôle d'architecture

| Champ | Valeur |
|---|---|
| But | chaque règle du contrôle d'architecture est rattachée au mécanisme qui la vérifie, et le rattachement est tracé |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | HORS MAILLE — nature : acte de décision — retenir un outil et tracer le choix ne vise aucun module de code |
| En conflit avec | — |
| Taille | M — 0 modules, 3 renvois |
| Critères d'acceptation | `guide-conventions-et-dod.md §6 § Non couvert par le corpus`, puce « Tranché » — la règle 2 relève de NetArchTest, les règles 1 et 3 d'un contrôle du graphe des références de projet ; `structure-projets.md §6`, mention « Outillage tranché » — chaque règle est rattachée au mécanisme qui la vérifie, avec la raison de ce rattachement ; `guide-conventions-et-dod.md §6 § Non couvert par le corpus`, paragraphe « Ce que chacun ne fait pas » — les limites de chaque mécanisme sont écrites, et le rattachement n'est réputé tracé que si elles le sont |
| Code de renvoi | — |

##### TB-008 — Câbler les deux contrôles d'architecture dans le pipeline d'intégration continue

| Champ | Valeur |
|---|---|
| But | les deux contrôles d'architecture s'exécutent à chaque commit et bloquent en cas d'échec |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | TB-006, TB-007, TB-095 |
| Périmètre d'écriture | la configuration d'intégration continue |
| En conflit avec | TB-095 — module partagé : la configuration d'intégration continue |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 3 — « Le test d'archi CI existe, **tourne en pipeline** » ; `guide-conventions-et-dod.md §6 § Non couvert par le corpus`, puce « Tranché » — les contrôles s'exécutent sur GitHub Actions, déclenchés à chaque commit — l'hébergement de l'application reste hors de cette décision |
| Code de renvoi | — |

##### TB-009 — Définition exhaustive du test d'architecture (`B3.2`)

| Champ | Valeur |
|---|---|
| But | la liste des handlers scopés/non-scopés et la couverture `ITokenValidator`/`ITokenDenylist` sont énumérées et le test les vérifie |
| Épique | EP-03 |
| Jalon | J0 (rattachement d'annexe) — **contenu dépendant de J2** |
| Dépend de | TB-006 |
| Périmètre d'écriture | le projet de test d'architecture |
| En conflit avec | TB-006 — module partagé : le projet de test d'architecture ; recouvrement à confirmer à J0 — TB-006 écrit dans le même projet de test d'architecture, non nommé par le corpus. Recouvrement à confirmer à J0. |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `guide-conventions-et-dod.md §6` : « `[À TRANCHER — B3.2]` : la définition exhaustive du test d'architecture […] est une dette déjà nommée par le corpus sous ce code de renvoi. Elle n'est pas résolue ici. » ; `ADR-014-modele-autorisation-api.md § Points à trancher` ; `ADR-015-securite-authentification-mvp.md § Points à trancher` ; `roadmap-entree-build.md Annexe A` |
| Code de renvoi | B3.2 |

##### TB-095 — Contrôler le graphe des références de projet (règles 1 et 3)

| Champ | Valeur |
|---|---|
| But | le contrôle échoue avant la compilation si un projet du Domaine référence l'Infrastructure ou la Présentation, ou si l'Application ou l'Infrastructure référence autre chose que le Domaine |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | la configuration d'intégration continue |
| En conflit avec | TB-008 — module partagé : la configuration d'intégration continue |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `guide-conventions-et-dod.md §6 § Dérivable du corpus`, règles 1 et 3 — le Domaine n'importe aucune assembly d'Infrastructure ni de Présentation, l'Application et l'Infrastructure ne dépendent que du Domaine ; `guide-conventions-et-dod.md §6 § Non couvert par le corpus`, puce « Règles 1 et 3 » — le contrôle porte sur le graphe des fichiers de projet, lisible sans ambiguïté de syntaxe et exécutable avant la compilation ; `structure-projets.md §2` — sens des dépendances de la Clean Architecture |
| Code de renvoi | — |
| Pourquoi une tranche distincte de TB-006 | les deux contrôles n'ont ni le même but observable, ni le même module d'écriture, ni le même moment d'exécution — celui-ci échoue avant la compilation, celui de TB-006 après. Les réunir produirait une tranche que l'on ne peut pas clore sur un seul critère. |

### EP-04 — Contrat d'autorisation en couche Application

##### TB-010 — Déclarer `IResourceAccessPolicy` en couche Application

| Champ | Valeur |
|---|---|
| But | l'interface est déclarée dans le projet Application unique et compile, sans implémentation ni câblage |
| Épique | EP-04 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | le projet Application unique |
| En conflit avec | TB-002 — module partagé : le projet Application unique ; TB-080 — module partagé : le projet Application unique ; TB-092 — module partagé : le projet Application unique |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 4 ; `cahier-strategie-test-et-recette.md §9`, ligne « Socle », puce 4 ; `ADR-007-rgpd-autorisation-api.md § Conséquences` ; `ADR-014-modele-autorisation-api.md § Décision` |
| Code de renvoi | — |

### EP-05 — Conventions outillées et socle d'ingénierie

##### TB-011 — Arrêter les conventions de format et de style outillées

| Champ | Valeur |
|---|---|
| But | un fichier de configuration de style existe et le build applique ses règles |
| Épique | EP-05 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | la configuration de la solution |
| En conflit avec | — |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `guide-conventions-et-dod.md §2 § Non couvert par le corpus`, puce « Tranché » — `.editorconfig` à la racine de la solution et analyzers Roslyn du SDK, appliqués par le build ; `guide-conventions-et-dod.md §3 § Non couvert par le corpus`, puce « Tranché » — ESLint avec le préréglage `angular-eslint` et Prettier, appliqués par le build ; `guide-conventions-et-dod.md §2` et `§3`, puces « Version » — la version en support à long terme en cours à l'ouverture du build, règle et non numéro |
| Code de renvoi | — |
| Borne du critère | cette tranche se clôt sur la **moitié C#** de ses critères — `.editorconfig` et analyzers Roslyn, réellement appliqués par le build : `roadmap-entree-build.md §3.1 § Critères de sortie factuels` n'exige, pour clore J0, ni lint ni build TypeScript parmi ses quatre critères. La moitié TypeScript/Angular — ESLint `angular-eslint` et Prettier réellement **appliqués par le build** — relève de `TB-104`, où le projet Angular est construit (`roadmap-entree-build.md §3.2 § Contenu`) : cette tranche ne peut pas appliquer un lint à un projet qui n'existe qu'en J1. C'est une borne assumée, pas un manque. La règle de version LTS (`guide-conventions-et-dod.md §2` et `§3`, puces « Version ») reste en revanche une politique déclarable dès J0, indépendante de l'existence du projet — elle n'est pas amputée par cette borne. |

##### TB-012 — Arrêter la convention de commit et de branche

| Champ | Valeur |
|---|---|
| But | une convention est écrite et opposable en revue |
| Épique | EP-05 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | HORS MAILLE — nature : acte de décision — arrêter une convention ne vise aucun module de code |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `guide-conventions-et-dod.md §5`, règle de message — Conventional Commits ratifié, forme `type(portée): sujet` ; `guide-conventions-et-dod.md §5`, règle de branche — `<type>/<TB-nnn>-<slug>`, l'identifiant de tranche rendant le lien branche ↔ tranche ↔ ticket retrouvable sans qu'aucun numéro ne soit recopié dans ce plan |
| Code de renvoi | — |

---

## 6. J1 — épiques et tranches

**Préalables bloquants d'entrée en J1**, nommés par [`roadmap-entree-build.md §3.2 § Préalable bloquant rattaché`](roadmap-entree-build.md) (renvoi seul) : J0 stabilisé ; `navigator.storage.persist()` (G-08) levé ; invariant d'autorisation API câblé dans les contrats Application.

### EP-04 — Contrat d'autorisation en couche Application

##### TB-080 — Câbler l'invariant d'autorisation dans les contrats Application

| Champ | Valeur |
|---|---|
| But | les contrats de la couche Application portent `IResourceAccessPolicy` ; aucun chemin de lecture ne le contourne |
| Épique | EP-04 |
| Jalon | J1 |
| Dépend de | TB-010 |
| Périmètre d'écriture | le projet Application unique |
| En conflit avec | TB-002 — module partagé : le projet Application unique ; TB-010 — module partagé : le projet Application unique ; TB-092 — module partagé : le projet Application unique |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.2` — « Le contrat `IResourceAccessPolicy` défini en J0 doit être intégré aux contrats Application "avant le début du jalon J1" » (`ADR-007-rgpd-autorisation-api.md § Conséquences`) ; `roadmap-entree-build.md §5.2` |
| Code de renvoi | — |
| Préalable bloquant | cette tâche est un **préalable bloquant à l'ouverture de J1** nommé par `roadmap-entree-build.md §3.2 § Préalable bloquant rattaché`. |

### EP-29 — Couture cliente et socle du store local

##### TB-104 — Échafauder le projet Angular (SPA + landing SSR/prerender)

| Champ | Valeur |
|---|---|
| But | un projet Angular existe et son build réussit, portant les deux surfaces — SPA interactive, landing SSR/prerender — sans encore aucune logique métier |
| Épique | EP-29 |
| Jalon | J1 |
| Dépend de | — |
| Périmètre d'écriture | TROU — l'échafaudage du projet Angular ne correspond à aucune des cinq natures de module de `§2` (ni agrégat, ni fiche d'écran, ni projet .NET, ni préoccupation de couche cliente déjà nommée, ni rôle d'outillage de l'énumération fermée) ; son emplacement est marqué `[À TRANCHER — J0]` par `structure-projets.md §7`, qui ne le rattache à aucune de ses sections |
| En conflit avec | recouvrement à confirmer à J0 — le périmètre `TROU` ne nomme aucun module (§2) : aucun recouvrement avec une autre tranche n'est décidable, ni affirmable ni infirmable, tant que `structure-projets.md §7` n'aura pas nommé l'emplacement de la couche cliente. Recouvrement à confirmer à J0. |
| Taille | M — 0 modules, 4 renvois |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Contenu` — « Domaine/Application (C#), projection TS/IndexedDB et Angular minimal » ; `structure-projets.md §7 § Frontend : Angular SPA + Angular SSR/prerender` — « un seul écosystème, partagé par deux surfaces » (SPA interactive, landing SSR/prerender) ; `guide-conventions-et-dod.md §3 § Dérivable du corpus` — « Écosystème Angular unique, partagé landing + application » ; `guide-conventions-et-dod.md §3 § Non couvert par le corpus`, puce « Tranché » — ESLint avec le préréglage `angular-eslint` et Prettier, réellement appliqués par le build de ce projet (borne renvoyée par `TB-011`) |
| Code de renvoi | — |
| Trou balisé | l'emplacement des répertoires (couche Angular du mode local et des deux surfaces frontend) est marqué `[À TRANCHER — J0]` par `structure-projets.md §7`, dernier paragraphe — ce document ne tranche pas cette arborescence. Suivi : `decisions-en-attente.md § Groupe 1.a`. |

##### TB-056 — Déclarer le contrat du service d'accès au store local et sa doublure

| Champ | Valeur |
|---|---|
| But | les composants de la couche cliente n'accèdent jamais au store directement ; le contrat du service est déclaré et une doublure sans règle métier le satisfait, ce qui ouvre la construction de la couche cliente sans attendre l'implémentation du domaine |
| Épique | EP-29 |
| Jalon | J1 |
| Dépend de | TB-005 |
| Périmètre d'écriture | service d'accès au store |
| En conflit avec | TB-082 — module partagé : service d'accès au store |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `ADR-001 § Compléments post-revue (2026-09-03)`, puce « Ordre C#-first » — C#-first désigne l'autorité du contrat, non l'ordre de construction de la couche cliente ; `roadmap-entree-build.md Annexe A`, ligne J1 — « Services Angular IndexedDB (wrappers, `DomSanitizer`, bandeaux durabilité/confidentialité, CSP complète) », code `P6` ; `structure-projets.md §7 § Accès au store : un service par agrégat` — un service d'accès par agrégat persisté, aucun service de façade partagé ; `guide-conventions-et-dod.md §3`, puce « Une règle de style opposable » — aucun composant n'importe directement l'API du store hors de ces services |
| Code de renvoi | P6 |
| Ce que la doublure ne peut pas porter | aucune règle métier. `ADR-001 § Alternatives considérées` écarte la réimplémentation du domaine en TypeScript, et cette révision ne la rouvre pas : une doublure rend des jeux de données, les seules validations autorisées côté client sont celles que `structure-projets.md §7` énumère (TB-094). |

##### TB-057 — Ouvrir le store local : object stores, clés, index et versionnement

| Champ | Valeur |
|---|---|
| But | le store local expose les object stores nommés avec leurs clés et index, se crée à la première ouverture, et une montée de version s'applique sans perte |
| Épique | EP-29 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | Space ; Folder ; Document ; DocumentType ; Session ; SessionViewConfig |
| En conflit avec | TB-059 — module partagé : Space ; TB-063 — module partagé : Folder ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document, DocumentType ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-071 — module partagé : Session ; TB-072 — module partagé : SessionViewConfig ; TB-073 — module partagé : Document, Session ; TB-076 — module partagé : Document, Session ; TB-081 — module partagé : Document |
| Taille | L — l'ouverture de la base porte sur tous les agrégats persistés à la fois — `onupgradeneeded` s'applique à la base entière, pas store par store ; la scinder produirait des tâches qui se réécriraient l'une l'autre |
| Critères d'acceptation | `ADR-017-modele-indexeddb-local.md §1.1 § Object stores` (le tableau nomme les stores et leurs clés) ; `ADR-017-modele-indexeddb-local.md §1.3 § Indexes locaux` ; `ADR-017-modele-indexeddb-local.md §1.4 § Versionnement du store et stratégie d'upgrade` ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 1 ; NFR-PERF-04 ; NFR-OFF-03 |
| Code de renvoi | M1, L1 |
| Point d'attention | RB-01-15 — « aucun élément donnant accès à des données protégées (jeton d'accès, secret, identifiant de connexion à un compte) n'est conservé dans le navigateur » — s'applique aux agrégats déclarés ici. |

### EP-10 — Châssis applicatif transverse

##### TB-058 — Poser le châssis applicatif transverse

| Champ | Valeur |
|---|---|
| But | indicateurs, bandeaux, accès compte et accessibilité transversale sont présents sur toute surface MJ |
| Épique | EP-10 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | châssis |
| En conflit avec | TB-078 — module partagé : châssis ; TB-093 — module partagé : châssis ; TB-097 — module partagé : châssis ; TB-103 — module partagé : châssis |
| Taille | L — surface de vérification dépassant le plafond `M` par le décompte de renvois (§3) |
| Critères d'acceptation | `zoning.md §S7 — Châssis` ; `zoning.md §S6 AR-19` (règle de densité) ; NFR-ACC-01 → NFR-ACC-04 ; `cahier-strategie-test-et-recette.md §3.6 — Axe transverse Accessibilité` |
| Code de renvoi | — |
| Effet de la révision du 2026-09-03 | le châssis ne dépend plus de l'implémentation du domaine ni du store réel — seulement du contrat (TB-056). C'est ce qui rend les surfaces ouvrables tôt. |

### US-UC-01 — Mode local sans compte

##### TB-079 — Écran d'accueil non authentifié

| Champ | Valeur |
|---|---|
| But | le MJ choisit de commencer sans compte et reçoit un message informatif au premier démarrage |
| Épique | US-UC-01 |
| Jalon | J1 |
| Dépend de | TB-058 |
| Périmètre d'écriture | fiche `accueil` |
| En conflit avec | TB-101 — module partagé : fiche `accueil` |
| Taille | M — un module, 5 renvois |
| Critères d'acceptation | `wireframes/sv1-transversaux/accueil/accueil.md § Zones et hiérarchie` ; US-01-01, ses scénarios nommés ; CR-UC01-01 → CR-UC01-03 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

##### TB-101 — Données locales introuvables, distinction première visite / perte

| Champ | Valeur |
|---|---|
| But | le MJ qui revient et dont les données ont disparu reçoit un message distinguant ce cas d'une première visite, et une action pour repartir |
| Épique | US-UC-01 |
| Jalon | J1 |
| Dépend de | TB-079, TB-083 |
| Périmètre d'écriture | fiche `accueil` |
| En conflit avec | TB-079 — module partagé : fiche `accueil` |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | `wireframes/sv1-transversaux/accueil/accueil.md § États` ; RB-01-12 ; US-01-06, ses scénarios nommés ; CR-UC01-12 → CR-UC01-14 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Point d'attention | la dépendance à `TB-083` tient au cas « stockage plein » (`CR-UC01-14`) : la surface de l'alerte est portée ici, mais son comportement relève de `NFR-OFF-03`, que `TB-083` cite déjà. |
| Trou balisé | `wireframes/sv1-transversaux/accueil/accueil.md § États` ne porte que trois états — « état vide » (première visite, aucune donnée locale ni compte détecté), « état chargé » (`[SOUS-SPÉCIFIÉ — UC-01 §Questions à valider en interview]` sur `UC-01 A2`), « état erreur » (réseau) — et aucun n'est l'état « données disparues » que `US-01-06` exige de distinguer d'une première visite. Le trou est inscrit, pas comblé : la définition de cet état relève de `docs/conception/interface/wireframes/`, non de ce plan. |

##### TB-102 — Persistance après fermeture du navigateur

| Champ | Valeur |
|---|---|
| But | le MJ qui rouvre l'application après avoir fermé le navigateur retrouve ses espaces et leurs contenus, intacts |
| Épique | US-UC-01 |
| Jalon | J1 |
| Dépend de | TB-060, TB-057 |
| Périmètre d'écriture | fiche `tableau-de-bord` |
| En conflit avec | TB-060 — module partagé : fiche `tableau-de-bord` |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `wireframes/sv2-entree-espace/tableau-de-bord/tableau-de-bord.md § États` ; RB-01-04 ; US-01-02, ses scénarios nommés ; CR-UC01-05 |
| Code de renvoi | — |
| Borne du critère | cette tranche se clôt sur la persistance observée (`CR-UC01-05`) — le MJ retrouve ses données. La seconde moitié de `RB-01-04` (demande de garantie de conservation permanente, bandeau si refus) est portée par `TB-083`, qui cite déjà `RB-01-04 (révisée)`. |

##### TB-103 — Fonctions cloud visibles mais désactivées en mode local

| Champ | Valeur |
|---|---|
| But | en mode local, chaque fonction cloud est visible, désactivée, et accompagnée d'une invite de conversion ; aucune n'est masquée |
| Épique | US-UC-01 |
| Jalon | J1 |
| Dépend de | TB-058 |
| Périmètre d'écriture | châssis |
| En conflit avec | TB-058 — module partagé : châssis ; TB-093 — module partagé : châssis ; TB-078 — module partagé : châssis ; TB-097 — module partagé : châssis |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | RB-01-06 → RB-01-08 ; `zoning.md §S7 § Châssis mode local` ; `zoning.md § AR-13` ; `user-stories/US-UC-01-mode-local-sans-compte.md § US-01-04 § Notes de conception` |
| Code de renvoi | — |
| Note de portée | `US-01-05` (création de compte avec migration) relève de J2 par sa dépendance à `UC-10` : cette tranche construit l'invite de conversion, pas ce vers quoi elle mène — hors périmètre de cette tranche. |
| Ordre de sérialisation | cette tranche dépend de `TB-058`, ce qui départage leur couple. Avec `TB-093`, `TB-078` et `TB-097`, aucune dépendance ne les départage, et aucune des épiques en présence (`US-UC-01`, `EP-11`, `US-UC-14`) ne porte à la fois cette tranche et l'une des trois autres dans un `Ordre de livraison recommandé` commun — le premier point du dispositif (§1, règle (b bis)) ne s'applique à aucun de ces trois couples. C'est le second point qui s'applique aux trois : l'ordre est laissé à qui les prend. |

### US-UC-02 — Créer un espace de jeu

##### TB-059 — Agrégat `Space` : types, espace personnel, invariants

| Champ | Valeur |
|---|---|
| But | un espace se crée avec un nom, porte son type `PERSONAL`/`CAMPAIGN`/`ONE_SHOT`, et l'espace personnel est provisionné sans geste utilisateur |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-057 |
| Périmètre d'écriture | Space |
| En conflit avec | TB-057 — module partagé : Space |
| Taille | L — un seul agrégat, mais la plage RB-02-01 → RB-02-20 engage vingt preuves distinctes, indivisibles de l'agrégat `Space` |
| Critères d'acceptation | RB-02-01 → RB-02-20 (`user-stories/US-UC-02-creer-espace-jeu.md § Règles métier`) ; `conception/domain/space-management.md § Invariants métier` |
| Code de renvoi | — |

##### TB-060 — Tableau de bord des espaces

| Champ | Valeur |
|---|---|
| But | le MJ accède à la liste des espaces dont il est propriétaire ou membre, l'espace personnel distinct et hors quota |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-059, TB-058 |
| Périmètre d'écriture | fiche `tableau-de-bord` |
| En conflit avec | TB-102 — module partagé : fiche `tableau-de-bord` |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | `wireframes/sv2-entree-espace/tableau-de-bord/tableau-de-bord.md § Zones et hiérarchie` ; `zoning.md §S6 AR-17`, `AR-05`, `AR-22` ; US-02-00, ses scénarios nommés ; CR-UC02-01 → CR-UC02-03 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

##### TB-061 — Écran de création d'espace

| Champ | Valeur |
|---|---|
| But | le MJ crée un espace `CAMPAIGN` ou `ONE_SHOT` nommé, sans être jamais bloqué par le quota en mode local ; sans nom, le refus est explicite |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-060 |
| Périmètre d'écriture | fiche `creation-espace` |
| En conflit avec | — |
| Taille | L — surface de vérification dépassant le plafond `M` par le décompte de renvois (§3) |
| Critères d'acceptation | `wireframes/sv2-entree-espace/creation-espace/creation-espace.md` ; US-02-01, ses scénarios nommés ; US-02-03 §"Le MJ en mode local n'est jamais bloqué par un quota d'espaces" ; CR-UC02-04 → CR-UC02-08 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Note de portée | `CR-UC02-09`, `CR-UC02-10` et `CR-UC02-12` (US-02-03, blocage et déblocage du quota) relèvent du plan payant et du compte cloud — J2. Cette tranche cite déjà le scénario « le MJ en mode local n'est jamais bloqué par un quota d'espaces » ; les scénarios de blocage effectif du quota gratuit n'y sont pas engagés. |
| Cas dérivé | `CR-UC02-13` ne porte pas de user story en source (« UC-02 fiche §Exceptions E2, dérivé, pas de Gherkin ») : son rattachement à cette tranche est une lecture de sa source, non un calcul. |

##### TB-062 — Hub de travail d'un espace et déclinaison personnelle

| Champ | Valeur |
|---|---|
| But | le MJ accède au hub de travail d'un espace `CAMPAIGN`/`ONE_SHOT`, et à la déclinaison sans vue session ni partage de l'espace `PERSONAL` |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-060 |
| Périmètre d'écriture | fiche `vue-campagne` ; fiche `vue-espace-personnel` |
| En conflit avec | — |
| Taille | L — deux fiches d'écran et plus de deux renvois ; la déclinaison `PERSONAL` de la surface Préparation n'étant « pas une surface-UC distincte » (`wireframes/README.md § Organisation du répertoire`), les deux fiches sont livrées ensemble |
| Critères d'acceptation | `wireframes/sv3-preparation/vue-campagne/vue-campagne.md` ; `wireframes/sv4-espace-personnel/vue-espace-personnel/vue-espace-personnel.md` (« sans vue session, sans partage, sans gestion de membres ») ; `zoning.md §S6 AR-14`, `AR-15` ; US-01-09, ses scénarios nommés ; CR-UC01-17 → CR-UC01-19 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

### US-UC-05 — Organiser par dossiers

##### TB-063 — Agrégat `Folder` et arborescence par défaut

| Champ | Valeur |
|---|---|
| But | chaque espace naît avec son gabarit de dossiers ; « Non classés » est présent et protégé ; les dossiers système sont renommables et supprimables |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-059 |
| Périmètre d'écriture | Folder |
| En conflit avec | TB-057 — module partagé : Folder |
| Taille | L — un seul agrégat, mais la plage RB-05-01 → RB-05-12 engage douze preuves distinctes, indivisibles de l'agrégat `Folder` |
| Critères d'acceptation | RB-05-01 → RB-05-12 ; `cahier-strategie-test-et-recette.md §3.1` — « `isSystem` est informatif : les dossiers système sont renommables et supprimables » |
| Code de renvoi | — |

##### TB-064 — Navigation par dossiers

| Champ | Valeur |
|---|---|
| But | le MJ navigue l'arborescence d'un espace et accède à la création d'un document dans le dossier courant |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-063, TB-062 |
| Périmètre d'écriture | fiche `navigation-dossiers` |
| En conflit avec | — |
| Taille | L — un seul module, mais neuf cas de recette et quatre stories rattachés : la surface de vérification dépasse le plafond `M` |
| Critères d'acceptation | `wireframes/sv3-preparation/navigation-dossiers/navigation-dossiers.md` ; `zoning.md §S6 AR-11`, `AR-16`, `AR-20` ; US-05-01, US-05-02, US-05-04 et US-05-05, leurs scénarios nommés ; CR-UC05-01 → CR-UC05-04 et CR-UC05-08 → CR-UC05-12 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Note de portée | `US-05-03` (associer un template par défaut à un dossier) relève de la couche riche de dossiers, distincte de la base que porte cette tranche — [`moscow.md § UC-05 riche — Dossiers et types de document élaborés (Should Have)`](../conception/besoin/vision/moscow.md), `§ Critère de sortie` : « Le MJ peut associer un type (PNJ, Lieu, Objet…) à un dossier ou un document pour enrichir ses entrées sans contraindre la structure. » Leur verdict (`CR-UC05-05 → CR-UC05-07`) n'est pas engagé par cette tranche. |

### US-UC-04 — Gérer les documents d'un espace

##### TB-065 — Agrégat `Document` : blocs, liens, tags, suppression logique

| Champ | Valeur |
|---|---|
| But | un document se crée avec un titre seul, porte des blocs libres, se déplace entre dossiers, se supprime logiquement, se lie et se tague |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-063 |
| Périmètre d'écriture | Document |
| En conflit avec | TB-057 — module partagé : Document ; TB-066 — module partagé : Document ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-073 — module partagé : Document ; TB-076 — module partagé : Document ; TB-081 — module partagé : Document |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | RB-04-01 → RB-04-05 ; `conception/domain/content-library.md § Invariants métier` |
| Code de renvoi | — |

##### TB-066 — `DocumentType` et value object `DocumentProperties`

| Champ | Valeur |
|---|---|
| But | un document peut rester sans type ; un document typé conserve son corps libre en blocs ; `SetProperties` refuse une valeur non conforme au `propertiesSchema` |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-065 |
| Périmètre d'écriture | DocumentType ; Document |
| En conflit avec | TB-057 — module partagé : Document, DocumentType ; TB-065 — module partagé : Document ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-073 — module partagé : Document ; TB-076 — module partagé : Document ; TB-081 — module partagé : Document |
| Taille | L — deux agrégats — le catalogue de types s'écrit indépendamment de toute instance de document — et plus de deux renvois |
| Critères d'acceptation | RB-04-04, RB-04-05 ; `architecture/specs/document-properties-schemas.md §1 § Contrat du value object DocumentProperties` (décision reportée d'ADR-002) ; `cahier-strategie-test-et-recette.md §3.1` ; `document-properties-schemas.md §1` — `SetProperties()` est refusé sur un document sans type : `properties` n'existe pas sans schéma contre quoi valider ; `document-properties-schemas.md § scene, npc, location, note, player_character, reveal` — `propertiesSchema` vide au MVP pour ces six types système, absence actée sur le précédent de `live_note` |
| Code de renvoi | — |

##### TB-067 — Invariant de visibilité `Document.CanBeReadBy`

| Champ | Valeur |
|---|---|
| But | un document `PLAYER_PRIVATE` est inaccessible même au MJ `OWNER`/`GM`, sans exception, y compris pour les `LIVE_NOTE` ; un document créé est privé par défaut |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-065 |
| Périmètre d'écriture | Document |
| En conflit avec | TB-057 — module partagé : Document ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document ; TB-069 — module partagé : Document ; TB-073 — module partagé : Document ; TB-076 — module partagé : Document ; TB-081 — module partagé : Document |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | RB-04-01, RB-06-11, RB-06-25, RB-06-26 ; `cahier-strategie-test-et-recette.md §2 § Invariant transverse : privé par défaut` ; `cahier-strategie-test-et-recette.md §3.1`, puces 2-3 |
| Code de renvoi | — |

##### TB-068 — Éditeur de document

| Champ | Valeur |
|---|---|
| But | le MJ compose un document complet — champ de type facultatif, contenu structuré ou libre, portée de lecture, renvois vers d'autres documents, backlinks |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-065, TB-066, TB-067, TB-064 |
| Périmètre d'écriture | fiche `editeur-document` |
| En conflit avec | — |
| Taille | L — un seul module, mais dix-neuf cas de recette et sept stories rattachés (US-03-05 n'y apporte qu'un seul scénario, les backlinks) : la surface de vérification dépasse le plafond `M` |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-document/editeur-document.md` ; `zoning.md §S6 AR-11` (placement des backlinks figé) ; US-04-01, US-04-02, US-04-03, US-04-04, US-04-05 et US-04-06, leurs scénarios nommés ; CR-UC04-01 → CR-UC04-18 (dérivés de la colonne `Source` du cahier) ; US-03-05 §"Voir les backlinks depuis la fiche d'un PNJ" ; CR-UC03-16 (dérivé de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Cas dérivés | `CR-UC04-19`, `CR-UC04-20` et `CR-UC04-21` ne portent pas de user story en source (exceptions de fiche de use case et `NFR-CONF-01`) : leur rattachement à cette tranche est une lecture de leur source, non un calcul. |

### US-UC-03 — Structurer un scénario

##### TB-069 — Le scénario comme `Document` : scènes ordonnées, révélations, statut

| Champ | Valeur |
|---|---|
| But | un scénario se crée avec son titre seul, contient zéro à N scènes ordonnées, et sépare contenu privé et contenu partageable |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-065, TB-066 |
| Périmètre d'écriture | Document |
| En conflit avec | TB-057 — module partagé : Document ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document ; TB-067 — module partagé : Document ; TB-073 — module partagé : Document ; TB-076 — module partagé : Document ; TB-081 — module partagé : Document |
| Taille | L — un seul agrégat, mais la plage RB-03-01 → RB-03-13 engage treize preuves distinctes, indivisibles du scénario comme document |
| Critères d'acceptation | RB-03-01 → RB-03-13 ; `conception/domain/content-library.md § Cas d'usage illustrés` |
| Code de renvoi | — |
| Ordre de sérialisation | cette tranche et TB-065 écrivent le même agrégat `Document` ; TB-069 dépend de TB-065 (et de TB-066), l'ordre est donc fixé par la dépendance. |

##### TB-070 — Éditeur de scénario

| Champ | Valeur |
|---|---|
| But | le MJ construit un scénario avec ses scènes liées et ses documents associés, cas spécialisé de l'éditeur de document |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-069, TB-068 |
| Périmètre d'écriture | fiche `editeur-scenario` |
| En conflit avec | TB-099 — module partagé : fiche `editeur-scenario` ; TB-100 — module partagé : fiche `editeur-scenario` |
| Taille | L — un seul module, mais treize cas de recette et cinq stories rattachés : la surface de vérification dépasse le plafond `M` |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-scenario/editeur-scenario.md` ; US-03-01, US-03-02, US-03-03, US-03-04 et US-03-07, leurs scénarios nommés ; CR-UC03-01 → CR-UC03-12 et CR-UC03-20 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Trou balisé | `cahier-strategie-test-et-recette.md §12 PO-09` — exception E2 de la fiche UC-03 (perte de connexion / erreur de sauvegarde) non dérivable d'un Gherkin ni d'une RB ferme, non recettée. |
| Cas dérivé | `CR-UC03-21` ne porte pas de user story en source (« UC-03 fiche §Exceptions E1, dérivé, pas de Gherkin ») : son geste (formulaire de création de scénario, titre manquant refusé) est situé sur cette fiche ; son rattachement est une lecture de sa source, non un calcul. |

##### TB-099 — Lier des documents existants à un scénario ou une scène

| Champ | Valeur |
|---|---|
| But | le MJ lie un document existant de l'espace à son scénario ou à une scène, et voit les backlinks depuis la fiche du document lié |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-065, TB-070 |
| Périmètre d'écriture | fiche `editeur-scenario` |
| En conflit avec | TB-070 — module partagé : fiche `editeur-scenario` ; TB-100 — module partagé : fiche `editeur-scenario` |
| Taille | M — un module, 5 renvois |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-scenario/editeur-scenario.md` ; US-03-05, ses scénarios nommés ; CR-UC03-13 → CR-UC03-15 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Ordre de sérialisation | cette tranche et `TB-100` écrivent la même fiche `editeur-scenario` sans dépendre l'une de l'autre. `US-UC-03` porte les deux dans son `Ordre de livraison recommandé`, qui place `US-03-06` (item 5) **avant** `US-03-05` (item 6) — `TB-100` est donc sérialisée avant `TB-099` (§1, règle (b bis)), malgré le sens de la numérotation des tickets. |

##### TB-100 — Gérer le statut d'un scénario

| Champ | Valeur |
|---|---|
| But | le MJ attribue un statut à son scénario (brouillon, prêt, joué, archivé) et filtre ses scénarios par statut |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-069, TB-070 |
| Périmètre d'écriture | fiche `editeur-scenario` |
| En conflit avec | TB-070 — module partagé : fiche `editeur-scenario` ; TB-099 — module partagé : fiche `editeur-scenario` |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-scenario/editeur-scenario.md` ; RB-03-10, RB-03-11 ; US-03-06, ses scénarios nommés ; CR-UC03-17 → CR-UC03-19 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

### US-UC-06 — Utiliser la vue session

##### TB-071 — Agrégat `Session` et machine d'états `LIVE → CLOSED → ARCHIVED`

| Champ | Valeur |
|---|---|
| But | chaque transition est irréversible et `ARCHIVED` refuse toute modification ; une seule session `LIVE` par espace |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-059, TB-066 |
| Périmètre d'écriture | Session |
| En conflit avec | TB-057 — module partagé : Session ; TB-073 — module partagé : Session ; TB-076 — module partagé : Session |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | RB-06-18, RB-06-20, RB-06-21 ; `cahier-strategie-test-et-recette.md §3.1`, puce 1 ; `conception/domain/session-conduct.md § Machine d'états de Session`, `§ Invariants métier` |
| Code de renvoi | — |

##### TB-072 — `SessionViewConfig` et dossiers mis en avant

| Champ | Valeur |
|---|---|
| But | le MJ configure ses panneaux hors session, les modifie en direct, et la configuration persiste d'une session à l'autre |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-071 |
| Périmètre d'écriture | SessionViewConfig |
| En conflit avec | TB-057 — module partagé : SessionViewConfig |
| Taille | S |
| Critères d'acceptation | `conception/domain/session-conduct.md § SessionViewConfig (agrégat)` ; `zoning.md §S6 AR-18` |
| Code de renvoi | — |
| Point d'attention | `ADR-016-serialisation-locale-migration.md §1.2 § Exclusions explicites` exclut `session_view_configs` et `session_view_folders` du périmètre migré (« préférence d'affichage, recréée à l'import comme à la création d'espace ») — la persistance locale visée ici et l'exclusion du payload de migration ne se contredisent pas. |

##### TB-073 — Notes de session (`LIVE_NOTE`) et épinglage

| Champ | Valeur |
|---|---|
| But | une note de session MJ naît `GM_ONLY`, une note joueur naît `PLAYER_PRIVATE` et reste hors d'atteinte du MJ ; épingler et partager sont deux gestes indépendants |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-067, TB-071 |
| Périmètre d'écriture | Session ; Document |
| En conflit avec | TB-057 — module partagé : Document, Session ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-071 — module partagé : Session ; TB-076 — module partagé : Document, Session ; TB-081 — module partagé : Document |
| Taille | L — deux agrégats — le type `LIVE_NOTE` étant un `Document` né dans une `Session` — et plus de deux renvois ; le geste métier est indivisible entre les deux |
| Critères d'acceptation | RB-06-11, RB-06-25, RB-06-26, RB-08-08b, RB-08-08c ; `cahier-strategie-test-et-recette.md Annexe A § Visibilité vs épinglage` ; `zoning.md §S6 AR-12` |
| Code de renvoi | — |

##### TB-074 — Vue session MJ, trois modes

| Champ | Valeur |
|---|---|
| But | le MJ pilote une session — accès aux scènes, notes et contenu épinglé, prise de note rapide — dans chacun des trois modes configuration / `LIVE` / consultation `CLOSED` |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-071, TB-072, TB-073, TB-058 |
| Périmètre d'écriture | fiche `vue-session-mj` |
| En conflit avec | TB-098 — module partagé : fiche `vue-session-mj` |
| Taille | L — un seul module déclaré, mais huit stories et vingt-et-un cas de recette s'y rattachent : la surface de vérification dépasse manifestement le plafond `M`. Cette tranche reste candidate à un redécoupage par mode au moment où l'équipe de build affine le grain — ce document ne tranche pas cette coupe |
| Critères d'acceptation | `wireframes/sv-session/vue-session-mj/vue-session-mj.md` (modes configuration / LIVE / consultation CLOSED) ; `zoning.md §S6 AR-02`, `AR-04`, `AR-09` ; US-06-01, US-06-02, US-06-03, US-06-04, US-06-05, US-06-06, US-06-09 et US-06-10, leurs scénarios nommés ; NFR-PERF-01, NFR-PERF-02, NFR-PERF-03 ; CR-UC06-01 → CR-UC06-18 et CR-UC06-25 → CR-UC06-27 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Note de portée | la vue joueur de la session (US-06-07, US-06-08) relève de J3 — hors périmètre de cette tranche : `moscow.md § UC-06 § Critère de sortie` borne ce jalon au mode local, « sans partage joueurs » ; leur verdict (CR-UC06-19 → CR-UC06-24) est porté par EP-28. |
| Cas dérivés | `CR-UC06-28` et `CR-UC06-29` ne portent pas de user story en source : leur rattachement est une lecture de leur source, non un calcul. |

##### TB-075 — Paramètres de campagne

| Champ | Valeur |
|---|---|
| But | le MJ configure les dossiers mis en avant en vue session et leur ordre, depuis l'écran paramètres |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-072, TB-062 |
| Périmètre d'écriture | fiche `parametres-campagne` |
| En conflit avec | TB-091 — module partagé : fiche `parametres-campagne` |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `wireframes/sv3-preparation/parametres-campagne/parametres-campagne.md` ; `zoning.md §S6 AR-18` (configuration logée en surface session, paramètres = renvoi), `AR-22` (désarchivage) ; RB-02-22, RB-02-23 ; CR-UC01-06 (dérivé de la colonne `Source` du cahier) ; US-01-04, ses scénarios nommés |
| Code de renvoi | — |
| Note de portée | la génération de lien d'invitation également logée sur cette fiche relève de J3 (US-UC-11, US-UC-08) — hors périmètre de cette tranche. |
| Note de portée | `US-01-05` (création de compte avec migration, `CR-UC01-08 → CR-UC01-11`) relève de J2 par sa dépendance à `UC-10` — hors périmètre de cette tranche. `CR-UC01-09` y situe littéralement son geste : « Il accède aux paramètres et choisit de créer un compte ». |

### US-UC-07 — Créer un élément à la volée

##### TB-076 — Création à la volée depuis la session

| Champ | Valeur |
|---|---|
| But | depuis une session `LIVE`, un document se crée sans quitter le contexte ; refus si titre vide ; impossible en `ARCHIVED` |
| Épique | US-UC-07 |
| Jalon | J1 |
| Dépend de | TB-071, TB-065 |
| Périmètre d'écriture | Session ; Document |
| En conflit avec | TB-057 — module partagé : Document, Session ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-071 — module partagé : Session ; TB-073 — module partagé : Document, Session ; TB-081 — module partagé : Document |
| Taille | L — deux agrégats et plus de deux renvois : le geste de création à la volée traverse `Session` et `Document` par construction |
| Critères d'acceptation | RB-07-01 → RB-07-08 ; NFR-PERF-03 |
| Code de renvoi | — |

##### TB-077 — Panneau de création rapide

| Champ | Valeur |
|---|---|
| But | le MJ produit un document improvisé en quelques secondes, sans quitter l'écran de session |
| Épique | US-UC-07 |
| Jalon | J1 |
| Dépend de | TB-076, TB-074 |
| Périmètre d'écriture | fiche `panneau-creation-rapide` |
| En conflit avec | — |
| Taille | L — un seul module, mais huit cas de recette et deux stories rattachés : la surface de vérification dépasse le plafond `M` |
| Critères d'acceptation | `wireframes/sv-session/panneau-creation-rapide/panneau-creation-rapide.md` (« sur-couche sans navigation propre ») ; US-07-01 et US-07-02, leurs scénarios nommés ; NFR-PERF-03 ; CR-UC07-01 → CR-UC07-08 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Cas dérivé | `CR-UC07-09` ne porte pas de user story en source (« RB-07-01..04, dérivé, pas de Gherkin spécifique PJ ») : son rattachement est une lecture de sa source. |

### US-UC-14 — Rechercher et filtrer l'information

##### TB-078 — Recherche en préparation

| Champ | Valeur |
|---|---|
| But | le MJ retrouve un document de son espace par titre, en moins de cinq secondes, depuis la préparation |
| Épique | US-UC-14 |
| Jalon | J1 |
| Dépend de | TB-057, TB-064 |
| Périmètre d'écriture | fiche `recherche-preparation` ; châssis |
| En conflit avec | TB-058 — module partagé : châssis ; TB-093 — module partagé : châssis ; TB-096 — module partagé : fiche `recherche-preparation` ; TB-097 — module partagé : châssis ; TB-103 — module partagé : châssis |
| Taille | L — deux modules (fiche `recherche-preparation` et `châssis`) et une surface de vérification dépassant le plafond `M` par le décompte de renvois (§3) ; la recherche est un composant de châssis dont le rendu est défini au niveau du wireframe de chaque surface (`zoning.md §S7`) — les deux moitiés ne se scindent pas |
| Critères d'acceptation | `wireframes/sv3-preparation/recherche-preparation/recherche-preparation.md` (« titre seul au MVP ») ; US-14-01, ses scénarios nommés ; NFR-PERF-04 ; CR-UC14-01 → CR-UC14-05 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Ordre de sérialisation | cette tranche partage `châssis` avec `TB-058` et `TB-093` sans dépendre de l'une ni de l'autre (`Dépend de` : `TB-057`, `TB-064` — aucun des deux ne s'y trouve). Ni `EP-10` (épique de `TB-058`) ni `EP-11` (épique de `TB-093`) n'est porté par un use case : le premier point du dispositif de départage (§1, règle (b bis)) ne s'applique à aucun des deux couples — `US-UC-14-recherche.md § Ordre de livraison recommandé` ordonne les stories de cette épique, pas une tranche étrangère, et ne départage donc pas davantage. C'est le second point qui s'applique aux deux couples : l'ordre est laissé à qui les prend. |
| Cas dérivé | `CR-UC14-13` ne porte pas de user story en source (« UC-14 fiche §Exceptions E2, dérivé, pas de Gherkin ») : son rattachement à cette tranche est une lecture de sa source, non un calcul. |

##### TB-096 — Filtrer les résultats de recherche par type, en préparation

| Champ | Valeur |
|---|---|
| But | le MJ restreint les résultats de recherche à un type de document, et retire le filtre sans perdre sa recherche |
| Épique | US-UC-14 |
| Jalon | J1 |
| Dépend de | TB-078, TB-066 |
| Périmètre d'écriture | fiche `recherche-preparation` |
| En conflit avec | TB-078 — module partagé : fiche `recherche-preparation` ; recouvrement à confirmer à J0 — la maille ne tranche pas si le filtre par type relève du composant de châssis (`zoning.md §S7`) ou du seul rendu de surface `recherche-preparation` ; aucune section ne le dit. Recouvrement à confirmer à J0. |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | `wireframes/sv3-preparation/recherche-preparation/recherche-preparation.md` ; `usecases/UC-14-recherche.md § A2 — Résultats nombreux` ; US-14-02, ses scénarios nommés ; CR-UC14-06 → CR-UC14-08 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

##### TB-097 — Pondération de la session active dans la recherche

| Champ | Valeur |
|---|---|
| But | depuis la vue session, les documents liés à la session active remontent en tête des résultats de recherche, sous les mêmes règles de visibilité qu'en préparation |
| Épique | US-UC-14 |
| Jalon | J1 |
| Dépend de | TB-072, TB-073, TB-078, TB-067 |
| Périmètre d'écriture | châssis |
| En conflit avec | TB-058 — module partagé : châssis ; TB-093 — module partagé : châssis ; TB-078 — module partagé : châssis ; TB-103 — module partagé : châssis |
| Taille | M — un module, 6 renvois |
| Critères d'acceptation | RB-14-07 → RB-14-10 ; `zoning.md §S7 § Recherche — composant de châssis omniprésent` ; `zoning.md § AR-11` |
| Code de renvoi | — |
| Ordre de sérialisation | cette tranche dépend de `TB-078`, ce qui départage leur couple. Avec `TB-058` et `TB-093`, en revanche, aucune dépendance ne les départage : ni `EP-10` (épique de `TB-058`) ni `EP-11` (épique de `TB-093`) n'est porté par un use case, et `US-UC-14-recherche.md § Ordre de livraison recommandé` ne nomme ni l'une ni l'autre tranche — le premier point du dispositif de départage (§1, règle (b bis)) ne s'applique à aucun des deux couples. C'est le second point qui s'applique : l'ordre est laissé à qui les prend. |

##### TB-098 — Panneau latéral de résultats de recherche en session

| Champ | Valeur |
|---|---|
| But | le MJ recherche depuis la vue session sans la quitter ; les résultats s'ouvrent dans un panneau latéral qu'il peut fermer sans perte de contexte |
| Épique | US-UC-14 |
| Jalon | J1 |
| Dépend de | TB-074, TB-097 |
| Périmètre d'écriture | fiche `vue-session-mj` |
| En conflit avec | TB-074 — module partagé : fiche `vue-session-mj` |
| Taille | L — la coupe comportement/surface est déjà faite (`TB-097`) ; ce qui reste ici est indivisible : l'ouverture, le rendu et la fermeture du panneau latéral appartiennent à la même fiche d'écran, et la plage de renvois dépasse le plafond `M` (§3) |
| Critères d'acceptation | `wireframes/sv-session/vue-session-mj/vue-session-mj.md` (zones « BARRE DE RECHERCHE », « PANNEAU LATÉRAL DE RÉSULTATS DE RECHERCHE ») ; `zoning.md § AR-11` ; US-14-03, ses scénarios nommés ; CR-UC14-09 → CR-UC14-12 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |

### EP-30 — Validations minimales du mode local

##### TB-081 — Validations TypeScript minimales du mode local

| Champ | Valeur |
|---|---|
| But | un titre vide et une structure de blocs invalide sont refusés côté client ; aucune validation côté client ne refuse ce que le domaine serveur accepte |
| Épique | EP-30 |
| Jalon | J1 |
| Dépend de | TB-056, TB-065 |
| Périmètre d'écriture | Document |
| En conflit avec | TB-057 — module partagé : Document ; TB-065 — module partagé : Document ; TB-066 — module partagé : Document ; TB-067 — module partagé : Document ; TB-069 — module partagé : Document ; TB-073 — module partagé : Document ; TB-076 — module partagé : Document |
| Taille | S |
| Critères d'acceptation | `structure-projets.md §7 § Le mode local : persistance navigateur + validations minimales` — les trois validations autorisées y sont énumérées littéralement (titre non vide, structure de blocs valide, aucune validation métier riche) ; `ADR-001 § Compléments post-revue (2026-09-03)`, puce d'invariant requalifiée — l'invariant se lit au sens des règles : aucune règle côté client que le domaine serveur ne porte pas |
| Code de renvoi | — |
| Borne du critère | cette tranche se clôt sur les **trois validations nommées**, pas sur l'invariant lui-même : `ADR-016 § Conséquences` qualifie celui-ci de « discipline de conception dirigée, pas une propriété vérifiée par outillage » — aucun test cross-langage TypeScript/C# ne l'assure, et le filet est la revalidation à l'import. C'est une borne assumée par le corpus, non un manque. |
| Conséquence opposable | une validation côté client qui refuse ce que le domaine serveur accepte est un **défaut à retirer du client**, non une protection supplémentaire (`ADR-001 § Compléments post-revue (2026-09-03)`). |

### EP-08 — Durabilité et sécurité du mode local

##### TB-082 — Traiter `navigator.storage.persist()` (`G-08`) comme état de première classe

| Champ | Valeur |
|---|---|
| But | à l'entrée en mode local, la demande de persistance est émise, son résultat est lu, et l'état (accordé / refusé-best-effort) est disponible à l'interface |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-057 |
| Périmètre d'écriture | service d'accès au store |
| En conflit avec | TB-056 — module partagé : service d'accès au store |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 2 ; `ADR-017-modele-indexeddb-local.md §3` ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 2 |
| Code de renvoi | — |
| Préalable bloquant | `G-08` est un **préalable bloquant à lever avant J1** (`ADR-001 § Compléments post-revue (2026-06-09)`, puce « G-08 »). |

##### TB-083 — Bandeau de durabilité

| Champ | Valeur |
|---|---|
| But | quand la conservation permanente n'est pas garantie, un bandeau non bloquant l'indique et propose la création d'un compte |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-082, TB-058 |
| Périmètre d'écriture | bandeaux transversaux |
| En conflit avec | TB-084 — module partagé : bandeaux transversaux |
| Taille | M — un module, 5 renvois |
| Critères d'acceptation | RB-01-04 (révisée) ; `usecases/UC-01-mode-local-sans-compte.md § Mode local (sans compte)` — « Bandeau de durabilité » ; `user-stories/US-UC-01-mode-local-sans-compte.md § US-01-03 § Révision apportée` ; NFR-OFF-02, NFR-OFF-03 ; `zoning.md §S7` |
| Code de renvoi | — |

##### TB-084 — Bandeau de confidentialité

| Champ | Valeur |
|---|---|
| But | en mode local, un bandeau distinct signale que les données ne sont pas chiffrées au repos, sans bloquer l'usage |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-058 |
| Périmètre d'écriture | bandeaux transversaux |
| En conflit avec | TB-083 — module partagé : bandeaux transversaux |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | RB-01-14 ; `usecases/UC-01-mode-local-sans-compte.md § Mode local (sans compte)` — « Bandeau de confidentialité » ; NFR-CONF-04 ; `user-stories/US-UC-01-mode-local-sans-compte.md § US-01-03 § Révision apportée` |
| Code de renvoi | — |

##### TB-085 — Sanitisation côté client et politique CSP

| Champ | Valeur |
|---|---|
| But | aucun contenu affiché ne peut déclencher l'exécution de code ; la politique de sécurité de contenu est servie et observable |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | sanitisation ; politique de sécurité de contenu |
| En conflit avec | TB-086 — module partagé : politique de sécurité de contenu ; recouvrement à confirmer à J0 — TB-086 porte sur la même configuration de politique de sécurité de contenu, non nommable dans la maille. Recouvrement à confirmer à J0. |
| Taille | L — surface de vérification dépassant le plafond `M` par le décompte de renvois (§3) |
| Critères d'acceptation | `guide-conventions-et-dod.md §7 § Conventions de sécurité de code` — critères d'acceptation non négociables énumérés (liste blanche positive identique serveur/client, `<script>` et attributs `on*` interdits sans exception, ordre valider-puis-sanitiser avant toute écriture, posture `default-src 'self'` / `script-src 'self'` / `connect-src 'self'`) ; `specs/sanitisation-csp.md §1`, `§2`, `§4` ; `usecases/UC-01-mode-local-sans-compte.md § Critères d'acceptation` — « Aucun contenu, importé ou saisi, ne peut déclencher l'exécution de code lors de son affichage » ; `guide-conventions-et-dod.md §7`, puce « bibliothèque de sanitisation » — `DomSanitizer` d'Angular, complété de DOMPurify sur le chemin d'import JSON ; `guide-conventions-et-dod.md §7`, puce « directives » — `default-src`, `script-src` et `connect-src` en `'self'`, plus `object-src 'none'`, `base-uri 'self'`, `form-action 'self'`, `frame-ancestors 'none'`, sans nonce ; `conception/domain/content-library.md § DocumentBlock`, mention « Format de `content` — tranché » — le contenu d'un bloc est un arbre de nœuds typés, jamais une chaîne de balisage — la liste blanche positive porte donc sur l'énumération fermée des types de nœuds |
| Code de renvoi | B1.2, P6 |

##### TB-086 — Vérifier l'absence d'appel réseau dans le périmètre mode local

| Champ | Valeur |
|---|---|
| But | aucun appel vers l'API Haversack n'est émis en mode local, et cette absence est observable |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-085 |
| Périmètre d'écriture | politique de sécurité de contenu |
| En conflit avec | TB-085 — module partagé : politique de sécurité de contenu ; recouvrement à confirmer à J0 — TB-085 porte sur la même configuration, non nommable dans la maille. Recouvrement à confirmer à J0. |
| Taille | M — un module, 3 renvois |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 4 — « Aucun appel réseau vers l'API Haversack n'existe dans le périmètre mode local livré (observable via la CSP `connect-src 'self'`) » ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 4 ; NFR-CONF-02 |
| Code de renvoi | — |

### EP-09 — Export d'espace, version minimale

##### TB-087 — Enveloppe de payload versionnée

| Champ | Valeur |
|---|---|
| But | un export produit une enveloppe portant `schemaVersion`, `exportedAt`, `appVersion`, `spaces` ; un payload sans `schemaVersion` est rejeté comme malformé |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-057 |
| Périmètre d'écriture | projection d'export |
| En conflit avec | TB-088 — module partagé : projection d'export ; TB-089 — module partagé : projection d'export |
| Taille | S |
| Critères d'acceptation | `ADR-016-serialisation-locale-migration.md §1.1 § Enveloppe du payload` (structure littérale) ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 3 |
| Code de renvoi | — |

##### TB-088 — Fonction de projection `store local → payload`

| Champ | Valeur |
|---|---|
| But | la projection produit exactement le périmètre sérialisé, en distinguant champs gouvernés et champs libres |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-087, TB-071, TB-072 |
| Périmètre d'écriture | projection d'export |
| En conflit avec | TB-087 — module partagé : projection d'export ; TB-089 — module partagé : projection d'export |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `ADR-016-serialisation-locale-migration.md §1.2 § Périmètre sérialisé` ; `§1.3 § Champs gouvernés et champs libres` ; `§1.4 § Format comme contrat versionné stable` ; `ADR-017-modele-indexeddb-local.md § Conséquences § Couture de projection filtrée, sans reformatage structurel` (le filtre écarte les sessions en statut `LIVE`, `session_view_configs` et `session_view_folders`) |
| Code de renvoi | — |

##### TB-089 — Dry-run structurel de validation du payload

| Champ | Valeur |
|---|---|
| But | un export produit est validable structurellement sans serveur cloud actif |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-087, TB-088 |
| Périmètre d'écriture | projection d'export |
| En conflit avec | TB-087 — module partagé : projection d'export ; TB-088 — module partagé : projection d'export |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 3 — « […] est structurellement rejouable (dry-run de validation possible côté contrat, sans nécessiter de serveur cloud actif pour cette vérification structurelle) » ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 3 |
| Code de renvoi | — |

##### TB-090 — Test de bout en bout multi-versions du store local (`P7`, part J1)

| Champ | Valeur |
|---|---|
| But | la fonction de projection produit un payload valide depuis chaque version supportée du store local |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-057, TB-088, TB-089 |
| Périmètre d'écriture | le projet de test de bout en bout |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md Annexe A`, ligne J1 — « e2e multi-versions IndexedDB (fonction de projection store→payload) », code `P7` ; `cahier-strategie-test-et-recette.md §11`, ligne UC-01, colonne « Tests nommés » |
| Code de renvoi | P7 |

##### TB-091 — Déclencher l'export depuis les paramètres

| Champ | Valeur |
|---|---|
| But | depuis les paramètres, le MJ obtient un fichier d'export de son espace ou de son espace personnel, en mode local comme avec un compte |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-088, TB-075 |
| Périmètre d'écriture | fiche `parametres-campagne` |
| En conflit avec | TB-075 — module partagé : fiche `parametres-campagne` |
| Taille | M — un module, 5 renvois |
| Critères d'acceptation | US-01-07, ses scénarios nommés ; `usecases/UC-01-mode-local-sans-compte.md § A4a` et `§ Critères d'acceptation` (« Must Have / version minimale […] promu ») ; `vision/moscow.md § Export d'espace § Critère de sortie` ; CR-UC01-15 → CR-UC01-16 (dérivés de la colonne `Source` du cahier) |
| Code de renvoi | — |
| Point d'attention | appliquer la priorité définie par `vision/moscow.md § Export d'espace`, seule autorité MoSCoW du corpus : Must Have. |
| Ordre de sérialisation | cette tranche et TB-075 écrivent la même fiche `parametres-campagne` ; TB-091 dépend de TB-075, l'ordre est donc fixé par la dépendance. |

### EP-11 — Instrumentation de validation du MVP

##### TB-092 — Instrumenter les piliers d'activation dès le mode local

| Champ | Valeur |
|---|---|
| But | trois compteurs anonymes, sans lecture du contenu créé, attestent l'usage réel de chacun des piliers du MVP |
| Épique | EP-11 |
| Jalon | J1 |
| Dépend de | TB-080, TB-074 |
| Périmètre d'écriture | le projet Application unique |
| En conflit avec | TB-002 — module partagé : le projet Application unique ; TB-010 — module partagé : le projet Application unique ; TB-080 — module partagé : le projet Application unique |
| Taille | M — un module, 4 renvois |
| Critères d'acceptation | `vision/moscow.md § Instrumentation de validation du MVP`, pilier 1 — activation préparation : un espace créé et au moins trois documents, ou un geste structurant (un dossier créé, ou un document déplacé hors de « Non classés ») ; `vision/moscow.md § Instrumentation de validation du MVP`, pilier 2 — activation vue session : au moins une action du MJ pendant la session ; `vision/moscow.md § Instrumentation de validation du MVP`, mention « Comment cette mesure sort du navigateur » — compteurs écrits dans le store local, transmis à la création de compte, aucun appel sortant en mode local ; `vision/vision-produit.md §2.3`, encadré « Limite structurelle de l'instrument » — le biais en faveur des convertis est nommé et doit être rappelé à toute lecture des résultats |
| Code de renvoi | — |
| Préalable, non dette | le pilier « activation partage » que `moscow.md § Instrumentation de validation du MVP` nomme suppose un partage aux joueurs, livré en J3 — deux jalons après le point de décision qu'il alimente. Point remonté, non résolu par ce plan. |

##### TB-093 — Capture de contact non bloquante en mode local

| Champ | Valeur |
|---|---|
| But | le MJ peut laisser une adresse de contact ; le refus n'a aucune conséquence sur l'accès ni le fonctionnement |
| Épique | EP-11 |
| Jalon | J1 |
| Dépend de | TB-058 |
| Périmètre d'écriture | châssis |
| En conflit avec | TB-058 — module partagé : châssis ; TB-078 — module partagé : châssis ; TB-097 — module partagé : châssis ; TB-103 — module partagé : châssis |
| Taille | S |
| Critères d'acceptation | `vision/moscow.md § Instrumentation de validation du MVP`, mention « Mécanisme — tranché » — sollicitation unique, affichée après que le MJ a atteint l'activation préparation, un refus ou une absence de réponse valant refus définitif — la proposition ne revient jamais ; `vision/moscow.md § Instrumentation de validation du MVP § Capture de contact non bloquante` — la proposition reste refusable sans conséquence sur l'accès ni le fonctionnement |
| Code de renvoi | — |

### EP-12 — Recette et clôture du jalon J1

##### TB-094 — Exécuter le cahier de recette du périmètre J1

| Champ | Valeur |
|---|---|
| But | chaque cas de recette du périmètre local porte un verdict |
| Épique | EP-12 |
| Jalon | J1 |
| Dépend de | TB-061, TB-077, TB-081, TB-084, TB-086, TB-090, TB-091, TB-092, TB-093, TB-096, TB-098, TB-099, TB-100, TB-101, TB-102, TB-103 |
| Périmètre d'écriture | HORS MAILLE — nature : renseignement de la colonne Verdict d'un document de recette — ne vise aucun module de code |
| En conflit avec | — |
| Taille | M — 0 modules, 3 renvois |
| Critères d'acceptation | `cahier-strategie-test-et-recette.md §8 — Critères d'entrée/sortie globaux & Definition of Done de test` ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only » (4 critères) ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels` (4 critères) |
| Code de renvoi | — |
| Point d'attention | `cahier-strategie-test-et-recette.md §13 § Contrôle d'intégrité des citations` impose, à chaque jalon, de vérifier que chaque ligne `CR-` cite un scénario Gherkin ou une RB réel et à jour dans le corpus source. |

---

## 7. J2 — épiques seulement

La décomposition en tâches de J2 n'est pas produite ici. Deux raisons cumulatives : le contenu réel des tâches J2 dépend de ce que J0 aura tranché sur les arborescences internes des projets .NET et namespaces (marqueurs `[À TRANCHER — J0]`, §2) — un plan qui découperait aujourd'hui des tâches à l'intérieur de ces arborescences trancherait à la place du build ; et l'entrée même en J2 est conditionnée par `[DÉCISION MARCHÉ]` (`roadmap-entree-build.md §3.3`), un point de décision `NON-VERIFIABLE-IN-BUILD` non acquis à ce jour. Descendre à la tâche pour un jalon dont l'entrée n'est pas encore actée serait du travail à refaire. Le détail des épiques J2 figure en §4 ; leur contenu, non recopié ici, est renvoyé à `roadmap-entree-build.md §3.4` et `Annexe A`.


## 8. J3 — épiques seulement

Même raisonnement qu'en §7 : J3 dépend structurellement de J2 (`roadmap-entree-build.md §3.6` — « le canal SignalR filtre par le même `IResourceAccessPolicy` que REST »), lui-même non ouvert. Le détail des épiques J3 figure en §4 ; leur contenu est renvoyé à `roadmap-entree-build.md §3.6` et `Annexe A`.


---

## 9. Lots parallélisables

Le champ `En conflit avec` de chaque tranche (§5-6) dit ce qui **ne peut pas** être parallèle ; il ne dit pas ce qui peut l'être — cette section le fait, en forme positive. Un lot regroupe des tranches dont les dépendances sont toutes closes et dont les périmètres d'écriture sont deux à deux disjoints.

**Convention de cette section** : seuls les paliers ouvrant au moins deux tranches simultanément sont nommés `LOT-nn` — un palier qui n'ouvre qu'une seule tranche n'ajoute rien à ce que le champ `Dépend de` de cette tranche porte déjà.

**Ces lots sont calculés, non écrits à la main** : ils se recalculent depuis les champs `Dépend de` et `Périmètre d'écriture` des tranches, et doivent être recalculés après toute modification de l'un ou de l'autre.

**Recalculé le 2026-09-04** (huit tranches neuves — `TB-096` à `TB-103` — et les conflits/dépendances qu'elles introduisent). Le recalcul corrige au passage un défaut topologique préexistant, indépendant de ces huit tranches : l'ancien `LOT-10` regroupait `TB-067`, `TB-071` et `TB-078` comme si les trois ouvraient au même palier — mais `TB-071` dépend de `TB-066` (qui ne clôt qu'au palier de `TB-067`) et `TB-078` dépend de `TB-064` (même palier). Ni l'un ni l'autre n'est donc prêt en même temps que `TB-067`, qui ne dépend que de `TB-065`, close un palier plus tôt. Vérifié champ par champ contre `1aecfcf` avant d'écrire ce constat.

**Convention appliquée aux paliers scindés par conflit** — la formulation ci-dessus ne tranchait pas le cas d'un palier ouvrant plusieurs tranches dont certaines se conflictent entre elles : la scission produit alors un groupe de plusieurs tranches et un ou plusieurs groupes d'une seule. **Ce dépôt retient que c'est le palier, pas le groupe, qui porte la propriété d'exemption** : un palier qui ouvre au moins deux tranches simultanément voit tous ses groupes nommés `LOT-nn`, y compris un groupe réduit à une seule tranche par un conflit — parce que ce groupe porte alors une information que le seul champ `Dépend de` de la tranche ne porte pas : qu'elle ouvre au même palier qu'un autre lot, dont un conflit l'exclut spécifiquement. Seul un palier qui, dans son ensemble, n'ouvre qu'**une** tranche reste hors `LOT-nn`, conformément à la justification donnée (« n'ajoute rien à ce que le champ `Dépend de` de cette tranche porte déjà »).

**L'appartenance à un lot se calcule depuis les dépendances et les périmètres déclarés ; elle ne présume ni du jalon, ni de la Definition of ready, ni — sur un périmètre `TROU` — d'une disjonction prouvée. Trois choses distinctes**, et `TB-104` est la première tranche du plan à les séparer toutes les trois.

**Le jalon** : un lot peut mélanger J0 et J1 si le graphe le permet — c'est le cas de `LOT-01` depuis l'ajout de `TB-104` (J1) aux côtés de `TB-001`, `TB-007`, `TB-012` (J0) ; aucune phrase de cette section n'exige l'homogénéité de jalon.

**La Definition of ready** (`methode-de-ticket.md §2`) : une tranche `TROU` peut ouvrir dans le même lot que des tranches prêtes sans que cela referme son trou — à la différence de `HORS MAILLE`, qui « ne signale aucun manque » et n'empêche « ni d'être prête, ni d'être close » (`methode-de-ticket.md §1 § À distinguer du marqueur HORS MAILLE`), le marqueur `TROU` maintient le ticket de `TB-104` à l'état `Ouvert, non prêt` (`methode-de-ticket.md §1 § Ce qu'un ticket fait du marqueur TROU`) tant que J0 n'a pas nommé l'emplacement de la couche Angular.

**La disjonction** : `§2` n'accorde l'absence de conflit d'écriture « par construction » qu'au seul marqueur `HORS MAILLE` — `TB-001`, `TB-007` et `TB-012` en bénéficient. `TB-104` non : son périmètre `TROU` ne nomme aucun module, donc aucun recouvrement avec une autre tranche n'est décidable, et `§1 (c)` l'interdit d'affirmer sans preuve. Sa présence dans `LOT-01` repose uniquement sur l'absence de dépendance déclarée (`Dépend de : —`), jamais sur une disjonction prouvée — voir la colonne `Modules touchés` de ce lot, qui le dit explicitement.

##### LOT-01

| Champ | Valeur |
|---|---|
| Ouvrable après | — (point de départ) |
| Tranches | TB-001, TB-007, TB-012, TB-104 |
| Modules touchés, deux à deux disjoints | TB-001 : HORS MAILLE ; TB-007 : HORS MAILLE ; TB-012 : HORS MAILLE — ces trois sont disjointes par construction (§2). TB-104 : TROU — recouvrement non décidable, ni affirmé ni infirmé (§1 (c)) ; sa présence ici repose sur l'absence de dépendance déclarée, pas sur une disjonction prouvée. |
| Ferme quand | TB-001, TB-007, TB-012, TB-104 sont closes |
| Note | `TB-104` est en J1, les trois autres en J0 — ce lot mélange deux jalons, ce que cette section permet (voir chapeau). Son ticket reste `Ouvert, non prêt` indépendamment de sa présence ici : l'appartenance à un lot n'est ni une readiness, ni — pour cette tranche — une disjonction prouvée. |

##### LOT-02

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-002 |
| Tranches | TB-003, TB-010, TB-011, TB-095 |
| Modules touchés, deux à deux disjoints | TB-003 : IdentityAccess, SpaceManagement, ContentLibrary, SessionConduct, SharedKernel ; TB-010 : le projet Application unique ; TB-011 : la configuration de la solution ; TB-095 : la configuration d'intégration continue |
| Ferme quand | TB-003, TB-010, TB-011, TB-095 sont closes |

##### LOT-03

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-003, TB-010 |
| Tranches | TB-004, TB-006, TB-080 |
| Modules touchés, deux à deux disjoints | TB-004 : SharedKernel ; TB-006 : le projet de test d'architecture ; TB-080 : le projet Application unique |
| Ferme quand | TB-004, TB-006, TB-080 sont closes |

##### LOT-04

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-004, TB-006, TB-007, TB-095 |
| Tranches | TB-005, TB-008, TB-009 |
| Modules touchés, deux à deux disjoints | TB-005 : SharedKernel ; TB-008 : la configuration d'intégration continue ; TB-009 : le projet de test d'architecture |
| Ferme quand | TB-005, TB-008, TB-009 sont closes |

##### LOT-05

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-056 |
| Tranches | TB-057, TB-058, TB-085 |
| Modules touchés, deux à deux disjoints | TB-057 : Space, Folder, Document, DocumentType, Session, SessionViewConfig ; TB-058 : châssis ; TB-085 : sanitisation, politique de sécurité de contenu |
| Ferme quand | TB-057, TB-058, TB-085 sont closes |

##### LOT-06

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-057, TB-058, TB-085 |
| Tranches | TB-059, TB-079, TB-082, TB-084, TB-086, TB-087, TB-093 |
| Modules touchés, deux à deux disjoints | TB-059 : Space ; TB-079 : fiche `accueil` ; TB-082 : service d'accès au store ; TB-084 : bandeaux transversaux ; TB-086 : politique de sécurité de contenu ; TB-087 : projection d'export ; TB-093 : châssis |
| Ferme quand | TB-059, TB-079, TB-082, TB-084, TB-086, TB-087, TB-093 sont closes |

##### LOT-07

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-058 |
| Tranches | TB-103 |
| Modules touchés, deux à deux disjoints | TB-103 : châssis |
| Ferme quand | TB-103 est close |
| Note | même palier que `LOT-06` — exclue de ce lot par son conflit de module `châssis` avec `TB-093`. |

##### LOT-08

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-058, TB-059, TB-082 |
| Tranches | TB-060, TB-063, TB-083 |
| Modules touchés, deux à deux disjoints | TB-060 : fiche `tableau-de-bord` ; TB-063 : Folder ; TB-083 : bandeaux transversaux |
| Ferme quand | TB-060, TB-063, TB-083 sont closes |

##### LOT-09

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-057, TB-060, TB-063, TB-079, TB-083 |
| Tranches | TB-061, TB-062, TB-065, TB-101, TB-102 |
| Modules touchés, deux à deux disjoints | TB-061 : fiche `creation-espace` ; TB-062 : fiche `vue-campagne`, fiche `vue-espace-personnel` ; TB-065 : Document ; TB-101 : fiche `accueil` ; TB-102 : fiche `tableau-de-bord` |
| Ferme quand | TB-061, TB-062, TB-065, TB-101, TB-102 sont closes |

##### LOT-10

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-062, TB-063, TB-065 |
| Tranches | TB-064, TB-066 |
| Modules touchés, deux à deux disjoints | TB-064 : fiche `navigation-dossiers` ; TB-066 : DocumentType, Document |
| Ferme quand | TB-064, TB-066 sont closes |

##### LOT-11

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-065 |
| Tranches | TB-067 |
| Modules touchés, deux à deux disjoints | TB-067 : Document |
| Ferme quand | TB-067 est close |
| Note | même palier que `LOT-10` — exclue par son conflit de module `Document` avec `TB-066`. |

##### LOT-12

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-056, TB-065 |
| Tranches | TB-081 |
| Modules touchés, deux à deux disjoints | TB-081 : Document |
| Ferme quand | TB-081 est close |
| Note | même palier que `LOT-10` et `LOT-11` — exclue par son conflit de module `Document` avec `TB-066` et `TB-067`. |

##### LOT-13

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-057, TB-059, TB-064, TB-065, TB-066, TB-067 |
| Tranches | TB-068, TB-069, TB-071, TB-078 |
| Modules touchés, deux à deux disjoints | TB-068 : fiche `editeur-document` ; TB-069 : Document ; TB-071 : Session ; TB-078 : fiche `recherche-preparation`, châssis |
| Ferme quand | TB-068, TB-069, TB-071, TB-078 sont closes |

##### LOT-14

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-066, TB-067, TB-068, TB-069, TB-071, TB-078 |
| Tranches | TB-070, TB-072, TB-073, TB-096 |
| Modules touchés, deux à deux disjoints | TB-070 : fiche `editeur-scenario` ; TB-072 : SessionViewConfig ; TB-073 : Session, Document ; TB-096 : fiche `recherche-preparation` |
| Ferme quand | TB-070, TB-072, TB-073, TB-096 sont closes |

##### LOT-15

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-065, TB-071 |
| Tranches | TB-076 |
| Modules touchés, deux à deux disjoints | TB-076 : Session, Document |
| Ferme quand | TB-076 est close |
| Note | même palier que `LOT-14` — exclue par son conflit de modules `Session`/`Document` avec `TB-071`, `TB-069`, `TB-073`. |

##### LOT-16

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-058, TB-062, TB-065, TB-067, TB-070, TB-071, TB-072, TB-073, TB-078, TB-087 |
| Tranches | TB-074, TB-075, TB-088, TB-097, TB-099 |
| Modules touchés, deux à deux disjoints | TB-074 : fiche `vue-session-mj` ; TB-075 : fiche `parametres-campagne` ; TB-088 : projection d'export ; TB-097 : châssis ; TB-099 : fiche `editeur-scenario` |
| Ferme quand | TB-074, TB-075, TB-088, TB-097, TB-099 sont closes |

##### LOT-17

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-069, TB-070 |
| Tranches | TB-100 |
| Modules touchés, deux à deux disjoints | TB-100 : fiche `editeur-scenario` |
| Ferme quand | TB-100 est close |
| Note | même palier que `LOT-16` — exclue par son conflit de module `fiche editeur-scenario` avec `TB-099`. |

##### LOT-18

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-074, TB-075, TB-076, TB-080, TB-087, TB-088, TB-097 |
| Tranches | TB-077, TB-089, TB-091, TB-092, TB-098 |
| Modules touchés, deux à deux disjoints | TB-077 : fiche `panneau-creation-rapide` ; TB-089 : projection d'export ; TB-091 : fiche `parametres-campagne` ; TB-092 : le projet Application unique ; TB-098 : fiche `vue-session-mj` |
| Ferme quand | TB-077, TB-089, TB-091, TB-092, TB-098 sont closes |

**Paliers n'ouvrant qu'une seule tranche** (voir le champ `Dépend de` de la tranche elle-même) : TB-002, TB-056, TB-090, TB-094.

---

## 10. Ce que ce plan n'ordonne pas

**Les deux points de décision non vérifiables en intégration continue.** `[DÉCISION MARCHÉ]` (renvoi seul : `roadmap-entree-build.md §3.3`) et `[VALIDATION JURIDIQUE EU]` (renvoi seul : `roadmap-entree-build.md §3.5`) ne sont pas des jalons de build : aucun test, aucun linter ne peut les trancher. Ce plan ne les décompose pas en tâches — il en hérite seulement les effets déjà tracés dans `roadmap-entree-build.md §3.4 § Dépendance d'entrée` (entrée en J2 conditionnée, cumulativement, par J1 stable et `[DÉCISION MARCHÉ] = go`) et `§3.5` (le lancement EU, pas le build de J3, est bloqué par `[VALIDATION JURIDIQUE EU]`).

**Les points `[À TRANCHER]`.** Ce document en signale un sous-ensemble, un par un, au fil des tâches où ils mordent directement (sur les fiches des tranches qui les closent, §5-6) — il n'en tient pas un registre séparé ni n'en recopie le décompte : le corpus les porte déjà (`guide-conventions-et-dod.md § Convention de balisage`, `structure-projets.md §3`, `architecture/specs/*.md`).

**Le post-MVP.** `UC-13` (scénario réutilisable) et `UC-15` (gel d'espaces au downgrade de tier) sont classés hors MoSCoW du MVP par `vision/moscow.md § UC-13` (paragraphe « hors première livraison ») et `§ UC-15 — Classement arbitré`, seule autorité du corpus pour cette classification. Ce plan ne leur ouvre aucune tâche ni épique décomposée, cohérent avec `moscow.md § Dépendances § Convention actée`, qui les exclut par construction de son propre diagramme.

**Trois points qui doivent rester ouverts, chacun sur un axe distinct — ce plan ne prend position sur aucune de leurs issues.**

- **Identité graphique, et le design system codé qui en dépend** (axe conception d'interface) — `roadmap-entree-build.md § Annexe B` fait de l'identité graphique un préalable hors jalon, porté par `docs/conception/interface/**`, non modifié par ce document. Tant que ce préalable n'est pas produit, aucune tranche de ce plan ne porte le design system **codé** (composants Angular, Storybook, CSS vars) : une tranche qui le viserait échouerait les quatrième et cinquième conditions d'entrée de [`methode-de-ticket.md §2`](methode-de-ticket.md) — ni critère d'acceptation tranchant, ni module nommable dans la maille (§2). Suivi de ce préalable : [`decisions-en-attente.md § Groupe 6`](decisions-en-attente.md).
- **Seuil chiffré de l'hypothèse `H1`** (axe métrique produit) — `vision/vision-produit.md §2.3` en est propriétaire ; `vision/moscow.md § Instrumentation de validation du MVP` y renvoie sans le fixer.
- **L'entrée `J-12` du registre des risques** (axe juridique) — `registre-risques.md §3.3`, méthode de vérification d'identité des invités non retenue ; le registre signale lui-même que la frontière entre « lacune réelle » et « posture existante » n'est pas tranchée par ce registre et attend un statut en comité des risques.


---

## 11. Identifiants retirés

Aucun identifiant de ce plan n'est renuméroté ni réattribué. Les tranches de J1 issues du redécoupage portent des identifiants **neufs** ; les identifiants qu'elles absorbent sont **retirés** et pointent ici vers ce qui les reprend. Un identifiant retiré n'est jamais réemployé pour désigner un autre périmètre.

Cette trace vit dans un tableau et **non dans une fiche de tranche** : une fiche produirait un ticket pour un travail qui n'existe plus.

| Identifiant retiré | Repris par |
|---|---|
| TB-013 | TB-059 |
| TB-014 | TB-063 |
| TB-015 | TB-065 |
| TB-016 | TB-066 |
| TB-017 | TB-067 |
| TB-018 | TB-069 |
| TB-019 | TB-071 |
| TB-020 | TB-072 |
| TB-021 | TB-073 |
| TB-022 | TB-076 |
| TB-023 | TB-080 |
| TB-024 | — dissoute : chaque tranche traverse la couche Application pour son propre geste ; l'invariant qu'elle portait est une clause de Definition of Done (`guide-conventions-et-dod.md §1 § Architecture`), non un lot de travail |
| TB-025 | TB-057 |
| TB-026 | TB-057 |
| TB-027 | TB-057 |
| TB-028 | TB-081 |
| TB-029 | TB-056 |
| TB-030 | TB-057 |
| TB-031 | TB-082 |
| TB-032 | TB-083 |
| TB-033 | TB-084 |
| TB-034 | TB-085 |
| TB-035 | TB-086 |
| TB-036 | TB-087 |
| TB-037 | TB-088 |
| TB-038 | TB-091 |
| TB-039 | TB-089 |
| TB-040 | TB-090 |
| TB-041 | TB-058 |
| TB-042 | TB-079 |
| TB-043 | TB-060 |
| TB-044 | TB-061 |
| TB-045 | TB-062 |
| TB-046 | TB-064 |
| TB-047 | TB-068 |
| TB-048 | TB-070 |
| TB-049 | TB-078 |
| TB-050 | TB-075 |
| TB-051 | TB-074 |
| TB-052 | TB-077 |
| TB-053 | TB-092 |
| TB-054 | TB-093 |
| TB-055 | TB-094 |

Deux épiques sont également retirées :

| Épique retirée | Devenue |
|---|---|
| EP-06 | — dissoute : « chaque geste métier passe par un cas d'usage Application unique » est un invariant d'architecture, pas un regroupement de travail. Sa seule tranche survivante, le câblage de l'invariant d'autorisation, rejoint EP-04 dont elle est la suite directe |
| EP-07 | — dissoute : les object stores, index et validations locales sont la moitié cliente de chaque tranche. Ce qui reste indivisible — l'ouverture et le versionnement de la base, qui portent sur la base entière — rejoint EP-29 |

Les identifiants de lot de toute version antérieure de la §9 sont retirés à chaque recalcul : les lots sont recalculés depuis les périmètres et les dépendances de ce document, et renumérotés à partir de `LOT-01`. **Ce nombre n'est délibérément pas fixé ici** — il varie à chaque recalcul, et l'écrire figerait un décompte que la §9 elle-même contredirait au premier ajout de tranche. Se référer à la §9 pour le compte courant.

---

## 12. Renvois

- [`docs/gestion-projet/roadmap-entree-build.md`](roadmap-entree-build.md) — séquence J0-J3, critères de sortie par jalon, ordre C#-first, réconciliation de nomenclature, rattachement des codes de renvoi.
- [`docs/gestion-projet/guide-conventions-et-dod.md`](guide-conventions-et-dod.md) — conventions de code, gates d'intégration continue, Definition of Done, conventions de maintenance du corpus documentaire (§8, appliquées par ce document).
- [`docs/architecture/structure-projets.md`](../architecture/structure-projets.md) — granularité des projets .NET, nommage du noyau partagé, périmètre du mode local TypeScript.
- [`docs/conception/besoin/vision/moscow.md`](../conception/besoin/vision/moscow.md) — priorisation MoSCoW, graphe de dépendances entre use cases.
- [`docs/test/cahier-strategie-test-et-recette.md`](../test/cahier-strategie-test-et-recette.md) — critères de sortie par jalon, Definition of Done de test, cas de recette.
- [`docs/conception/interface/wireframes/README.md`](../conception/interface/wireframes/README.md) — table de couverture UC → fiche(s), éléments différés.
- [`docs/conception/interface/zoning.md`](../conception/interface/zoning.md) — arbitrages figés (§S6), châssis.
- Use cases, user stories et journeys : `docs/conception/besoin/usecases/UC-NN-*.md`, `docs/conception/besoin/user-stories/US-UC-NN-*.md`.
- ADR cités : [ADR-001](../architecture/decisions/ADR-001-execution-domaine-mode-local.md), [ADR-002](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md), [ADR-004](../architecture/decisions/ADR-004-transport-temps-reel.md), [ADR-006](../architecture/decisions/ADR-006-perimetre-mvp.md), [ADR-007](../architecture/decisions/ADR-007-rgpd-autorisation-api.md), [ADR-008](../architecture/decisions/ADR-008-structure-solution.md), [ADR-011](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md), [ADR-014](../architecture/decisions/ADR-014-modele-autorisation-api.md), [ADR-015](../architecture/decisions/ADR-015-securite-authentification-mvp.md), [ADR-016](../architecture/decisions/ADR-016-serialisation-locale-migration.md), [ADR-017](../architecture/decisions/ADR-017-modele-indexeddb-local.md), [ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md).
- Spécifications citées : `docs/architecture/specs/document-properties-schemas.md`, `docs/architecture/specs/sanitisation-csp.md`, `docs/architecture/specs/telemetrie.md`, `docs/architecture/specs/mapping-ef-core.md`, `docs/architecture/specs/contrat-openapi.md`, `docs/architecture/specs/repli-temps-reel.md`, `docs/architecture/specs/config-securite-migration.md`, `docs/architecture/specs/requete-effacement-non-partage.md`.
- [`docs/gestion-projet/registre-risques.md`](registre-risques.md) — entrée `J-12`.
- [`docs/conception/besoin/vision/vision-produit.md`](../conception/besoin/vision/vision-produit.md) — hypothèse `H1`.
- [`docs/deploiement/README.md`](../deploiement/README.md) — absence de document de déploiement, hébergeur non tranché.

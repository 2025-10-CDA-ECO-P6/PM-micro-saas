# Plan de travail — Haversack

| Champ | Valeur |
|---|---|
| Statut | Décomposition en tâches du périmètre J0-J1, en épiques seules pour J2-J3 |
| Audience | la ou les personnes qui prennent en charge l'implémentation — seules ou en petit collectif |
| Sources | `docs/gestion-projet/roadmap-entree-build.md`, `docs/gestion-projet/guide-conventions-et-dod.md`, `docs/architecture/structure-projets.md`, `docs/conception/besoin/vision/moscow.md`, les fiches `usecases/UC-NN`, `user-stories/US-UC-NN`, `docs/architecture/decisions/ADR-NNN`, `docs/architecture/specs/*.md`, `docs/conception/interface/wireframes/**`, `docs/test/cahier-strategie-test-et-recette.md` |

---

## Bandeau de cadrage — ce que ce document fait et ne fait pas

Ce document **décompose** un ordonnancement déjà arrêté. Il ne redéfinit :
- **ni le périmètre MoSCoW** ([`vision/moscow.md`](../conception/besoin/vision/moscow.md) — seule autorité du corpus pour attribuer une priorité Must/Should/Could/Won't) ;
- **ni le phasage J0-J3** ([`roadmap-entree-build.md` §1-2](roadmap-entree-build.md)) ;
- **ni les critères de sortie de jalon** ([`roadmap-entree-build.md` §3](roadmap-entree-build.md), [`cahier-strategie-test-et-recette.md` §9](../test/cahier-strategie-test-et-recette.md)) ;
- **ni l'ordre des user stories à l'intérieur d'une epic** (section « Ordre de livraison recommandé » de chaque fichier `US-UC-NN-*.md`).

**En cas de conflit entre ce document et une source citée** (roadmap, ADR, use case, user story, spécification), **la source citée fait foi**. Ce document est un artefact de décomposition, pas une nouvelle autorité de corpus — même posture que [`roadmap-entree-build.md` § Bandeau de cadrage](roadmap-entree-build.md) vis-à-vis des ADR.

Ce que ce document **fait** : il prend la séquence de jalons J0-J3 et, pour J0 et J1, la décompose jusqu'à la tâche — but observable, dépendances techniques, périmètre d'écriture, conflits calculés, taille, critères d'acceptation par renvoi. Pour J2 et J3, il s'arrête au niveau de l'épique (raison en §7). Il regroupe les tâches ouvrables simultanément en lots parallélisables, calculés depuis les périmètres d'écriture déclarés — jamais affirmés.

**Hors périmètre** : durée, effectif, affectation nominative — aucun de ces éléments n'est un fait dérivable du corpus de conception, qui ne porte ni charge ni ordonnancement temporel propre. La méthode de vérification par renvoi (guillemets, citation, chaîne distinctive) suit [`guide-conventions-et-dod.md` §8](guide-conventions-et-dod.md).

---

## 1. Comment ce plan se lit

Quatre règles opposables, quel que soit l'effectif qui exécute ce plan :

**(a) Aucune tâche ne porte de « qui » ni de « quand ».** Une tâche est définie par son but observable, ses dépendances techniques et son périmètre d'écriture — jamais par une charge, une durée ou un nom de personne. Ce plan reste valide qu'il soit exécuté par une personne seule ou une équipe.

**(b) L'ordre d'une tâche est celui de ses dépendances techniques, rien d'autre.** Le champ `Dépend de` d'une tâche liste uniquement les tâches dont le résultat est un préalable technique — jamais une hypothèse sur qui ferait la tâche, sur l'effectif disponible ou sur une commodité d'organisation.

**(c) `En conflit avec` est calculé depuis `Périmètre d'écriture`, jamais affirmé.** Deux tâches sont en conflit si elles déclarent au moins un module commun dans leur périmètre d'écriture ; le module partagé est nommé en clair sur la ligne. Un conflit sans module partagé porte sa justification en clair sur la même ligne — il n'est jamais affirmé sans preuve.

**(d) La largeur d'un lot est son nombre de tâches, lisible sur place.** Aucun décompte n'est écrit dans ce document — ni « les N tâches de tel lot », ni « les M tâches ouvrables ». Compter les lignes d'un lot dit sa largeur ; l'écrire ailleurs créerait un nombre qui dérive au premier ajout.

**Règle de lecture pour un effectif variable** : une personne de plus qu'il n'y a de tâches ouvrables dans un lot attend la levée d'une dépendance ; elle ne redécoupe pas le lot. Le découpage des tâches est fixé par ce document, pas par l'effectif du jour.

**Épique et lot parallélisable sont deux notions distinctes.** Une **épique** est un regroupement de travail — un ensemble de tâches qui concourent au même but fonctionnel ou technique, que ce but soit porté par un use case du corpus ou non. Un **lot parallélisable** est un ensemble de tâches **ouvrables simultanément aujourd'hui**, calculé depuis les dépendances déjà closes et l'absence de conflit de périmètre — sans rapport avec le découpage en épiques. Une même épique peut répartir ses tâches sur plusieurs lots successifs ; un même lot peut regrouper des tâches de plusieurs épiques.

**Deux natures d'épique.** Là où un use case du corpus porte le regroupement, l'épique **est** l'epic existante du corpus — son identifiant `US-UC-01` à `US-UC-14`, portée par le fichier `docs/conception/besoin/user-stories/US-UC-NN-*.md` (ces fichiers sont littéralement intitulés « # Epic — … » et leur [README](../conception/besoin/user-stories/README.md) leur assigne déjà ce rôle). Aucun identifiant neuf n'est créé pour ce cas. Là où aucun use case ne porte le regroupement — l'échafaudage de J0, les trois Must Have sans use case identifiés par [`moscow.md` § Vue d'ensemble](../conception/besoin/vision/moscow.md) (espace personnel, export d'espace, instrumentation de validation du MVP), et les lots transverses de J1 qui servent plusieurs epics sans en porter aucune (contrats Application, socle du store local, châssis applicatif) — l'épique porte un identifiant `EP-nn`, séquentiel sur deux chiffres, propre à ce document. Une épique `EP-nn` porte un champ `Sert` nommant la ou les epics `US-UC-NN` qu'elle sert, ou `—` si elle ne sert aucune epic en particulier (échafaudage, conventions).

---

## 2. Maille de désignation du périmètre d'écriture

Le champ `Périmètre d'écriture` de chaque tâche est une liste de **modules**, séparés par `;`. Un module est exactement l'une de ces natures, et rien d'autre :

- un des projets .NET nommés par [`structure-projets.md` §3](../architecture/structure-projets.md) : `Haversack.Domain`, `Haversack.Application`, `Haversack.Infrastructure.Persistence`, `Haversack.Infrastructure.Notifications`, `Haversack.Presentation.Api`, `Haversack.Presentation.Landing` ;
- un des namespaces de bounded context du Domaine : `Haversack.Domain/IdentityAccess`, `/SpaceManagement`, `/ContentLibrary`, `/SessionConduct` ;
- le noyau partagé `Haversack.Domain/SharedKernel` ([`structure-projets.md` §4](../architecture/structure-projets.md)) ;
- un object store local nommé par [ADR-017 §1.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md) — `spaces`, `folders`, `documents`, `document_blocks`, `document_links`, `document_tags`, `document_types` ;
- une fiche d'écran, par son slug, parmi les vingt de [`docs/conception/interface/wireframes/`](../conception/interface/wireframes/README.md).

**Aucun chemin de fichier n'apparaît sous ces niveaux.** `structure-projets.md §3` écrit littéralement que « le contenu de chacun des cinq namespaces — fichiers, classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document », et marque ces cinq lignes `[À TRANCHER — J0]`. Un plan qui énumérerait des fichiers trancherait à la place du build.

**Coût de cette maille — écrit en clair, pas seulement mesuré.** Cette maille est **conservatrice** : elle sépare proprement deux bounded contexts ou deux couches (un namespace du Domaine ne peut pas être confondu avec `Haversack.Application`), mais elle **ne sait pas séparer deux tâches à l'intérieur d'un même module** — par exemple deux agrégats distincts du même namespace `ContentLibrary`. Ces deux tâches sont déclarées en conflit et sérialisées par ce document, alors que le code réel, une fois écrit, autoriserait peut-être leur écriture simultanée sur des fichiers distincts. **C'est un coût de délai, jamais un risque de collision — l'erreur va dans le sens sûr.**

**Le plancher de cette maille est le namespace ou le projet, tant que son arborescence interne reste `[À TRANCHER — J0]`.** Les namespaces du Domaine et les projets .NET portent tous ce marqueur pour leur contenu interne ([`structure-projets.md` §3](../architecture/structure-projets.md)). Dès que J0 tranche cette arborescence, la valeur du champ `Périmètre d'écriture` des tâches concernées pourra être affinée à un grain plus fin (par exemple : deux agrégats du même namespace, une fois leurs fichiers nommés, cesseraient d'être déclarés en conflit) — sans qu'aucun autre champ de la tâche (but, dépendances, acceptation) n'ait à changer.

**Cas particulier — le store racine des espaces.** Le store IndexedDB racine est désigné ici par son rôle (« store local `spaces` »), pas par un nom de variable figé. [ADR-017 §1.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md) nomme la table `spaces`, mais [`roadmap-entree-build.md` Annexe B](roadmap-entree-build.md) en fait un point de vigilance de build : ce nom est un résidu de rédaction antérieur au renommage `campaigns → spaces` d'[ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md), à confirmer cohérent au moment de l'implémentation — ce plan emploie donc `spaces` en le désignant par son rôle documenté, pas comme une valeur figée à l'abri de toute révision.

**Ce que cette maille ne peut pas nommer.** Plusieurs tâches de la couche Angular/TypeScript du mode local (services d'accès au store, bandeaux transversaux, sanitisation, châssis applicatif) ne correspondent à aucune des cinq natures ci-dessus : ce ne sont ni des projets .NET, ni des namespaces du Domaine, ni des object stores, ni des fiches d'écran de wireframe — elles vivent dans une couche dont le corpus ne nomme aucun répertoire ([`structure-projets.md` §7](../architecture/structure-projets.md) en décrit le périmètre fonctionnel, pas l'emplacement). Leur champ `Périmètre d'écriture` porte `TROU — non nommable dans la maille` avec la nature précise du manque ; ce n'est pas un oubli de ce document, c'est une absence mesurée du corpus source.

---

## 3. Échelle de taille

La taille d'une tâche mesure sa **surface de vérification** — combien de choses distinctes il faudra prouver, et à combien d'endroits — jamais sa **difficulté**. Une tâche `L` n'est pas une tâche dure ; c'est une tâche dont la preuve de complétude touche plusieurs modules ou s'appuie sur de nombreux renvois.

| Taille | Critère |
|---|---|
| **S** | un seul module au périmètre, et au plus 2 renvois de critère d'acceptation |
| **M** | un seul module et 3 à 6 renvois ; ou deux modules et au plus 2 renvois |
| **L** | plus de deux modules, ou plus de 6 renvois, ou deux modules avec plus de 2 renvois (la surface combinée dépasse le plafond `M` dans les deux cas) |

**Une tâche `L` porte, sur la même ligne que sa taille, la raison pour laquelle elle n'est pas scindée** — sauf lorsque son périmètre est `TROU` (non nommable), auquel cas la taille se lit sur le seul décompte de renvois, faute de module à compter (le seuil `M` d'un seul module — 3 à 6 — et le seuil `L` — plus de 6 — s'appliquent alors tels quels).

**Unité de comptage d'un renvoi — définie ici pour être recalculable par quiconque relit ce document.** Un renvoi est un **segment du champ `Critères d'acceptation` séparé par un point-virgule** — c'est la ponctuation que ce document emploie systématiquement pour juxtaposer des citations distinctes. Deux règles de comptage à l'intérieur d'un segment :
- **une plage explicite** (`CR-UC07-01 → CR-UC07-09`, `RB-06-18 → RB-06-21`, notation `→` ou `..`) compte pour **son étendue** — le nombre d'éléments qu'elle couvre — jamais pour un ;
- **tout autre segment** — qu'il porte un identifiant nu, plusieurs identifiants séparés par une virgule, un renvoi `fichier § section`, un ou plusieurs titres de scénario entre guillemets, ou une incise (« , puce 1 ») — compte pour **un**, quel que soit le nombre d'éléments qu'il énumère en son sein.

Un renvoi à une plage de règles métier ou de cas de recette n'est pas cosmétique : citer `RB-02-01 → RB-02-20` engage réellement vingt preuves distinctes à la clôture de la tâche, pas une — c'est exactement ce que l'échelle mesure (surface de vérification, pas difficulté). Une tâche qui cite une telle plage bascule donc mécaniquement en `L`, même à un seul module.

---

## 4. Vue des épiques

Colonne `Dépend de` : épiques précédentes dont au moins une tâche de l'épique courante dépend techniquement — jamais un critère de sortie de jalon (renvoyé, non recopié). Colonne `Plage de tâches` : identifiants `TB-nnn` rattachés à cette épique dans les sections J0/J1 ; `—` pour une épique J2/J3 (niveau épique seul, §7-8).

### J0

| Épique | But | Jalon | Dépend de | Plage de tâches |
|---|---|---|---|---|
| EP-01 — Confirmation des décisions pré-implémentation | chaque ADR pré-implémentation ou mixte reçoit sa validation de décideur, datée | J0 | — | TB-001 |
| EP-02 — Échafaudage de la solution .NET | les projets .NET existent, s'assemblent, et portent les abstractions communes du Domaine | J0 | EP-01 | TB-002 → TB-005 |
| EP-03 — Gate d'architecture en intégration continue | toute violation de frontière entre bounded contexts est détectée et bloquante, sans intervention humaine | J0 | EP-02 | TB-006 → TB-009 |
| EP-04 — Contrat d'autorisation en couche Application | le contrat d'autorisation applicative existe sous forme d'interface, prêt à être implémenté ultérieurement | J0 | EP-02 | TB-010 |
| EP-05 — Conventions outillées et socle d'ingénierie | Les conventions de format, de style et de commit/branche sont arrêtées et opposables en revue | J0 | EP-02 | TB-011 → TB-012 |

### J1

| Épique | But | Jalon | Dépend de | Plage de tâches |
|---|---|---|---|---|
| US-UC-02 — Créer un espace de jeu | nommer et instancier un espace partagé, avec la propriété automatique de l'espace personnel | J1 | EP-02 | TB-013, TB-043, TB-044, TB-045 |
| US-UC-05 — Organiser par dossiers | Chaque espace dispose de son arborescence de dossiers par défaut, système-agnostique | J1 | US-UC-02 | TB-014, TB-046 |
| US-UC-04 — Gérer les documents d'un espace | Un document se crée, se retrouve et se modifie, avec une visibilité privée par défaut | J1 | US-UC-05 | TB-015 → TB-017, TB-047 |
| US-UC-03 — Structurer un scénario | Un scénario se crée avec son titre seul et contient zéro à N scènes ordonnées | J1 | US-UC-04 | TB-018, TB-048 |
| US-UC-06 — Utiliser la vue session | conduire une partie hors ligne depuis un hub unique — scènes, notes, contenu mis en avant | J1 | US-UC-02, US-UC-04 | TB-019 → TB-021, TB-030, TB-050, TB-051 |
| US-UC-07 — Créer un élément à la volée | improviser un document en cours de partie, sans rupture de contexte | J1 | US-UC-06, US-UC-04 | TB-022, TB-052 |
| EP-06 — Contrats Application du périmètre local | Chaque geste métier passe par un cas d'usage Application unique ; l'invariant d'autorisation y est câblé | J1 | EP-04, US-UC-02 → US-UC-07 | TB-023, TB-024 |
| EP-07 — Socle technique du store local IndexedDB | Le store local expose les object stores et index nommés, se crée à la première ouverture et s'accède par service dédié | J1 | EP-06 | TB-025 → TB-029 |
| US-UC-01 — Mode local sans compte | Le MJ choisit de commencer sans compte, depuis l'écran d'accueil non authentifié | J1 | EP-10 | TB-042 |
| EP-08 — Durabilité et sécurité du mode local | `navigator.storage.persist()` est traité comme état de première classe ; aucun contenu ne peut déclencher l'exécution de code | J1 | EP-07 | TB-031 → TB-035 |
| EP-09 — Export d'espace, version minimale | Un export au format `schemaVersion` se produit depuis le store local et est structurellement rejouable | J1 | EP-07, US-UC-06 | TB-036 → TB-040 |
| EP-10 — Châssis applicatif transverse | Indicateurs, bandeaux, accès compte et accessibilité transversale sont présents sur toute surface MJ | J1 | EP-07, EP-08 | TB-041 |
| US-UC-14 — Rechercher et filtrer l'information | Un MJ retrouve un document de son espace par titre, depuis la préparation | J1 | EP-07, US-UC-05 | TB-049 |
| EP-11 — Instrumentation de validation du MVP | L'application constate, de façon anonyme, l'activation de chacun des piliers | J1 | EP-06, US-UC-06, EP-10 | TB-053, TB-054 |
| EP-12 — Recette et clôture du jalon J1 | Chaque cas de recette du périmètre local porte un verdict | J1 | l'ensemble des épiques J1 ci-dessus | TB-055 |

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
| EP-28 — Recette du périmètre J3 | Chaque cas de recette des UC-08, 09, 11, 12 porte un verdict | J3 | US-UC-08, US-UC-09, US-UC-11, US-UC-12 | — |

---

## 5. J0 — épiques et tâches

### EP-01 — Confirmation des décisions pré-implémentation

##### TB-001 — Confirmer les ADR de nature pré-implémentation ou mixte

| Champ | Valeur |
|---|---|
| But | chaque ADR de nature pré-implémentation ou mixte porte, dans son en-tête, une ligne de confirmation datée et signée d'un décideur |
| Épique | EP-01 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | TROU — nature : acte de confirmation documentaire sur les ADR eux-mêmes, hors maille de désignation (ne vise aucun projet, namespace, store ni écran) |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 1 ; `cahier-strategie-test-et-recette.md §9`, ligne « Socle », puce 1 ; la liste des ADR concernés se lit sur `architecture/decisions/README.md § Index`, colonne « Nature » (valeurs « pré-implémentation » et « mixte ») — non recopiée ici, pour ne pas dupliquer un décompte que ce registre possède et fait évoluer |
| Code de renvoi | — |

### EP-02 — Échafaudage de la solution .NET

##### TB-002 — Créer la solution et les projets nommés

| Champ | Valeur |
|---|---|
| But | `dotnet build` de la solution réussit sur une solution vide de logique métier |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-001 |
| Périmètre d'écriture | Haversack.Domain ; Haversack.Application ; Haversack.Infrastructure.Persistence ; Haversack.Infrastructure.Notifications ; Haversack.Presentation.Api ; Haversack.Presentation.Landing |
| En conflit avec | TB-010 — module partagé : Haversack.Application ; TB-022 — module partagé : Haversack.Application ; TB-023 — module partagé : Haversack.Application ; TB-024 — module partagé : Haversack.Application ; TB-053 — module partagé : Haversack.Application |
| Taille | L — les projets sont échafaudés en un seul geste solidaire (une solution .NET unique) ; les scinder romprait l'unité de la structure décrite par `structure-projets.md` |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 2 |
| Code de renvoi | — |

##### TB-003 — Poser les namespaces du Domaine

| Champ | Valeur |
|---|---|
| But | `Haversack.Domain` contient les cinq sous-namespaces et rien d'autre |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | Haversack.Domain/IdentityAccess ; Haversack.Domain/SpaceManagement ; Haversack.Domain/ContentLibrary ; Haversack.Domain/SessionConduct ; Haversack.Domain/SharedKernel |
| En conflit avec | TB-004 — module partagé : Haversack.Domain/SharedKernel ; TB-005 — module partagé : Haversack.Domain/SharedKernel ; TB-013 — module partagé : Haversack.Domain/SpaceManagement ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-019 — module partagé : Haversack.Domain/SessionConduct ; TB-020 — module partagé : Haversack.Domain/SessionConduct ; TB-021 — module partagé : Haversack.Domain/ContentLibrary, Haversack.Domain/SessionConduct ; TB-022 — module partagé : Haversack.Domain/SessionConduct |
| Taille | L — les namespaces sont posés en un seul geste de structuration du Domaine ; leur contenu interne reste `[À TRANCHER — J0]` (voir Acceptation), ce qui interdit de scinder cette tâche plus finement aujourd'hui |
| Critères d'acceptation | `structure-projets.md § Domaine et Application : projets uniques` ; `guide-conventions-et-dod.md §2 § Organisation par namespaces par bounded context`. **`TROU`** — nature : *contenu non spécifié* — `structure-projets.md` marque les cinq lignes `[À TRANCHER — J0]` et écrit que « le contenu de chacun des cinq namespaces — fichiers, classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document » ; aucun critère d'acceptation n'existe au-delà de l'existence des cinq répertoires |
| Code de renvoi | — |

##### TB-004 — Implémenter les abstractions du `SharedKernel`

| Champ | Valeur |
|---|---|
| But | `Entity`, `AggregateRoot`, `DomainEvent`, `AuditInfo`, `SoftDelete`, `IRepository<T>` et les exceptions métier existent et compilent |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-003 |
| Périmètre d'écriture | Haversack.Domain/SharedKernel |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/SharedKernel ; TB-005 — module partagé : Haversack.Domain/SharedKernel |
| Taille | M |
| Critères d'acceptation | `structure-projets.md §4 — Nommage du noyau partagé` ; `conception/domain/core.md § Abstractions DDD` ; `conception/domain/core.md § Value objects primitifs` ; `conception/domain/core.md § Primitives de traçabilité` ; `guide-conventions-et-dod.md §2` |
| Code de renvoi | — |

##### TB-005 — Implémenter les IDs typés du Core

| Champ | Valeur |
|---|---|
| But | aucune signature métier n'accepte un `Guid` nu ; une inversion de deux identifiants ne compile pas |
| Épique | EP-02 |
| Jalon | J0 |
| Dépend de | TB-004 |
| Périmètre d'écriture | Haversack.Domain/SharedKernel |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/SharedKernel ; TB-004 — module partagé : Haversack.Domain/SharedKernel |
| Taille | M |
| Critères d'acceptation | `conception/domain/core.md § IDs typés` ; `guide-conventions-et-dod.md §2` ; `architecture/specs/mapping-ef-core.md §1` |
| Code de renvoi | — |

### EP-03 — Gate d'architecture en intégration continue

##### TB-006 — Écrire le test d'architecture (règles initiales)

| Champ | Valeur |
|---|---|
| But | le test échoue si une référence traverse une frontière de bounded context non autorisée, et passe sinon |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | TB-003 |
| Périmètre d'écriture | TROU — nature : projet de test d'architecture non nommé par le corpus (mesuré : 0 occurrence de `.Tests`, `UnitTests`, « projet de test » dans les fichiers versionnés du dépôt) |
| En conflit avec | TB-009 — recouvrement à confirmer à J0 : les deux tâches écrivent dans le même projet de test d'architecture, non nommé par le corpus, donc non déclarable comme module au sens de la maille (§2) |
| Taille | M |
| Critères d'acceptation | `guide-conventions-et-dod.md §6 § Dérivable du corpus` (les règles y sont énumérées littéralement) ; `structure-projets.md §6` ; `cahier-strategie-test-et-recette.md §3.4 — Test d'architecture (CI)` |
| Code de renvoi | — |

##### TB-007 — Choisir l'outil du test d'architecture

| Champ | Valeur |
|---|---|
| But | un outil est retenu et la décision est tracée |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | TROU — nature : destination de la décision non fixée par le corpus |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `guide-conventions-et-dod.md §6 § Non couvert par le corpus` : « `[À TRANCHER — J0]` : l'outil exact du test d'architecture. La source présente deux options de façon alternative, sans trancher entre elles — NetArchTest ou une convention de namespace vérifiée par script. » Aucun critère d'acceptation n'existe pour choisir entre les deux |
| Code de renvoi | — |

##### TB-008 — Câbler le gate dans le pipeline d'intégration continue

| Champ | Valeur |
|---|---|
| But | le test d'architecture s'exécute à chaque commit et bloque en cas d'échec |
| Épique | EP-03 |
| Jalon | J0 |
| Dépend de | TB-006, TB-007 |
| Périmètre d'écriture | TROU — nature : configuration de pipeline, emplacement et hébergeur non fixés par le corpus |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.1 § Critères de sortie factuels`, puce 3 — « Le test d'archi CI existe, **tourne en pipeline** ». **`TROU`** — nature : *le critère de sortie présuppose une infrastructure que le corpus ne décrit nulle part* — `docs/deploiement/README.md` déclare qu'« aucun document de déploiement n'est produit à ce jour » et que l'hébergeur est `[non tranché]` ; aucun ADR, spécification ni critère d'acceptation n'existe pour la plateforme de CI, ses environnements ou son déclencheur |
| Code de renvoi | — |

##### TB-009 — Définition exhaustive du test d'architecture (`B3.2`)

| Champ | Valeur |
|---|---|
| But | la liste des handlers scopés/non-scopés et la couverture `ITokenValidator`/`ITokenDenylist` sont énumérées et le test les vérifie |
| Épique | EP-03 |
| Jalon | J0 (rattachement d'annexe) — **contenu dépendant de J2** (les handlers et les contrats de token n'existent qu'en J2 ; tension documentée, non résolue ici) |
| Dépend de | TB-006 |
| Périmètre d'écriture | TROU — nature : même projet de test d'architecture que TB-006, non nommé |
| En conflit avec | TB-006 — recouvrement à confirmer à J0 : les deux tâches écrivent dans le même projet de test d'architecture, non nommé par le corpus |
| Taille | M |
| Critères d'acceptation | `guide-conventions-et-dod.md §6` : « `[À TRANCHER — B3.2]` : la définition exhaustive du test d'architecture […] est une dette déjà nommée par le corpus sous ce code de renvoi. Elle n'est pas résolue ici. » ; `ADR-014-modele-autorisation-api.md § Points à trancher` ; `ADR-015-securite-authentification-mvp.md § Points à trancher` ; `roadmap-entree-build.md Annexe A` |
| Code de renvoi | B3.2 |

### EP-04 — Contrat d'autorisation en couche Application

##### TB-010 — Déclarer `IResourceAccessPolicy` en couche Application

| Champ | Valeur |
|---|---|
| But | l'interface est déclarée dans `Haversack.Application` et compile, sans implémentation ni câblage |
| Épique | EP-04 |
| Jalon | J0 |
| Dépend de | TB-002 |
| Périmètre d'écriture | Haversack.Application |
| En conflit avec | TB-002 — module partagé : Haversack.Application ; TB-022 — module partagé : Haversack.Application ; TB-023 — module partagé : Haversack.Application ; TB-024 — module partagé : Haversack.Application ; TB-053 — module partagé : Haversack.Application |
| Taille | M |
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
| Périmètre d'écriture | TROU — nature : configuration à la racine de la solution et configuration front, emplacement non nommé par le corpus |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | **`TROU`** — nature : *deux `[À TRANCHER — J0]` explicites* — `guide-conventions-et-dod.md §2 § Non couvert par le corpus` (règles de format et de style fines, `.editorconfig`, analyzers Roslyn, conventions de casse détaillées) et `guide-conventions-et-dod.md §3 § Non couvert par le corpus` (configuration ESLint/Prettier, ruleset exact, versions Angular/Node). Aucun critère d'acceptation n'existe |
| Code de renvoi | — |

##### TB-012 — Arrêter la convention de commit et de branche

| Champ | Valeur |
|---|---|
| But | une convention est écrite et opposable en revue |
| Épique | EP-05 |
| Jalon | J0 |
| Dépend de | — |
| Périmètre d'écriture | TROU — nature : point intégralement laissé ouvert par le corpus, aucune destination fixée |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | **`TROU`** — nature : *point intégralement laissé ouvert* — `guide-conventions-et-dod.md §5` : « Le corpus de conception ne définit aucun format de message de commit ni de convention de nommage de branche. Ce point est intégralement laissé à l'équipe de build […] un candidat courant est Conventional Commits, à ratifier — ce guide ne le tranche pas. » |
| Code de renvoi | — |

**Recouvrement d'écriture à l'intérieur de J0** : `Haversack.Domain/SharedKernel` est touché par TB-003, TB-004 et TB-005 ; `Haversack.Application` par TB-002 et TB-010 ; le projet de test d'architecture, non nommé, par TB-006 et TB-009 (recouvrement à confirmer à J0). Aucune autre paire de tâches J0 ne partage de périmètre.

---

## 6. J1 — épiques et tâches

**Préalables bloquants d'entrée en J1**, nommés par `roadmap-entree-build.md §3.2 § Préalable bloquant rattaché` (renvoi seul, non recopié en détail) : J0 stabilisé ; `navigator.storage.persist()` (G-08) levé ; invariant d'autorisation API câblé dans les contrats Application.

### US-UC-02 — Créer un espace de jeu

##### TB-013 — Agrégat `Space` (`PERSONAL` / `CAMPAIGN` / `ONE_SHOT`)

| Champ | Valeur |
|---|---|
| But | un espace se crée avec un nom, porte son type, et l'espace `PERSONAL` est provisionné sans geste utilisateur |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-003, TB-004, TB-005 |
| Périmètre d'écriture | Haversack.Domain/SpaceManagement |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/SpaceManagement |
| Taille | L — un seul module, mais la plage RB-02-01 → RB-02-20 et la plage CR-UC02-01 → CR-UC02-13 portent à elles seules trente-trois preuves distinctes, indivisibles de l'agrégat Space |
| Critères d'acceptation | RB-02-01 → RB-02-20 (`user-stories/US-UC-02-creer-espace-jeu.md § Règles métier`) ; US-02-00 §"Le MJ en mode local dispose d'un espace personnel comme conteneur par défaut", §"L'espace personnel n'apparaît pas dans le quota FREE" ; US-02-01 §"Le MJ crée une campagne avec le nom uniquement", §"Le MJ tente de créer une campagne sans renseigner de nom" ; `conception/domain/space-management.md § Invariants métier` ; CR-UC02-01 → CR-UC02-13 |
| Code de renvoi | — |

##### TB-043 — Tableau de bord des espaces

| Champ | Valeur |
|---|---|
| But | le MJ accède à la liste des espaces dont il est propriétaire ou membre, avec l'espace personnel distinct et hors quota |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-013, TB-041 |
| Périmètre d'écriture | fiche `tableau-de-bord` |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `wireframes/sv2-entree-espace/tableau-de-bord/tableau-de-bord.md § Zones et hiérarchie` (carte espace personnel en tête, badge « hors quota ») ; `zoning.md §S6 AR-17`, `AR-05`, `AR-22` ; RB-02-22, RB-02-23 (désarchivage) ; US-02-00 §"L'espace personnel n'apparaît pas dans le quota FREE" |
| Code de renvoi | — |

##### TB-044 — Écran de création d'espace

| Champ | Valeur |
|---|---|
| But | le MJ crée un espace `CAMPAIGN` ou `ONE_SHOT` nommé, sans être jamais bloqué par le quota en mode local |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-043 |
| Périmètre d'écriture | fiche `creation-espace` |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `wireframes/sv2-entree-espace/creation-espace/creation-espace.md` ; US-02-01 (scénarios de création nommée) ; US-02-03 §"Le MJ en mode local n'est jamais bloqué par un quota d'espaces" |
| Code de renvoi | — |

##### TB-045 — Vue campagne et vue espace personnel

| Champ | Valeur |
|---|---|
| But | le MJ accède au hub de travail d'un espace `CAMPAIGN`/`ONE_SHOT`, et à la déclinaison sans vue session ni partage de l'espace `PERSONAL` |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-043 |
| Périmètre d'écriture | fiche `vue-campagne` ; fiche `vue-espace-personnel` |
| En conflit avec | — |
| Taille | L — deux fiches d'écran et 4 renvois (le plafond `M` à deux modules est 2 renvois) ; la déclinaison PERSONAL de la surface Préparation n'étant « pas une surface-UC distincte » (`wireframes/README.md § Organisation du répertoire` ; `zoning.md §S6 AR-01/14/15/16/17`), les deux fiches sont livrées ensemble |
| Critères d'acceptation | `wireframes/sv3-preparation/vue-campagne/vue-campagne.md` ; `wireframes/sv4-espace-personnel/vue-espace-personnel/vue-espace-personnel.md` (« sans vue session, sans partage, sans gestion de membres ») ; `zoning.md §S6 AR-14`, `AR-15` ; US-01-09 §"Le MJ retrouve le contenu capturé" |
| Code de renvoi | — |

### US-UC-05 — Organiser par dossiers

##### TB-014 — Agrégat `Folder` et arborescence par défaut

| Champ | Valeur |
|---|---|
| But | chaque espace naît avec son gabarit de dossiers ; « Non classés » est présent et protégé ; les dossiers système sont renommables et supprimables |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-013 |
| Périmètre d'écriture | Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-021 — module partagé : Haversack.Domain/ContentLibrary |
| Taille | L — un seul module ; les plages RB-05-01 → RB-05-12 et CR-UC05-01 → CR-UC05-12 portent l'essentiel de la surface de vérification, indivisible de l'agrégat Folder |
| Critères d'acceptation | RB-05-01 → RB-05-12 ; US-05-01 §"Le MJ crée un dossier avec un nom valide", §"Le MJ tente de créer un dossier avec un nom vide" ; US-05-02 §"Le MJ renomme un dossier système" ; US-05-04 §"Le MJ supprime un dossier non vide — option Non classés", §"Le MJ supprime un dossier système" ; `cahier-strategie-test-et-recette.md §3.1` — « `isSystem` est informatif : les dossiers système sont renommables et supprimables ; le dossier virtuel « Non classés » est protégé » ; `zoning.md §S6 AR-20`, `AR-16` ; CR-UC05-01 → CR-UC05-12 |
| Code de renvoi | — |

##### TB-046 — Navigation par dossiers

| Champ | Valeur |
|---|---|
| But | le MJ navigue l'arborescence d'un espace et accède à la création d'un document dans le dossier courant |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-014, TB-045 |
| Périmètre d'écriture | fiche `navigation-dossiers` |
| En conflit avec | — |
| Taille | L — un seul module ; la plage CR-UC05-01 → CR-UC05-12 porte l'essentiel de la surface de vérification de cet écran |
| Critères d'acceptation | `wireframes/sv3-preparation/navigation-dossiers/navigation-dossiers.md` ; `zoning.md §S6 AR-11`, `AR-16`, `AR-20` ; US-05-01, US-05-02, US-05-04, US-05-05 (scénarios nommés) ; CR-UC05-01 → CR-UC05-12 |
| Code de renvoi | — |

### US-UC-04 — Gérer les documents d'un espace

##### TB-015 — Agrégat `Document`, `DocumentBlock`, `DocumentLink`, tags

| Champ | Valeur |
|---|---|
| But | un document se crée avec un titre seul, porte des blocs libres, se déplace entre dossiers, se supprime logiquement, se lie et se tague |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-014 |
| Périmètre d'écriture | Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-021 — module partagé : Haversack.Domain/ContentLibrary |
| Taille | L — un seul module ; la plage CR-UC04-01 → CR-UC04-21 porte à elle seule vingt-et-une preuves, indivisibles de l'agrégat Document |
| Critères d'acceptation | RB-04-01 → RB-04-05 ; US-04-01 §"Le MJ supprime un document", §"Le MJ déplace un document vers un autre dossier" ; US-04-02 §"Le MJ crée une note sans renseigner aucun champ", §"La note rapide est placée dans le dossier "Notes" par défaut" ; US-04-04 §"Le MJ saisit une variante de casse d'un tag existant" ; US-04-05 §"Le MJ supprime un lien sans supprimer le document cible" ; `conception/domain/content-library.md § Invariants métier` ; CR-UC04-01 → CR-UC04-21 |
| Code de renvoi | — |

##### TB-016 — `DocumentType` et value object `DocumentProperties`

| Champ | Valeur |
|---|---|
| But | un document peut rester sans type ; un document typé conserve son corps libre en blocs ; `SetProperties` refuse une valeur non conforme au `propertiesSchema` |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-015 |
| Périmètre d'écriture | Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-021 — module partagé : Haversack.Domain/ContentLibrary |
| Taille | M — un seul module, 3 renvois |
| Critères d'acceptation | RB-04-04, RB-04-05 ; `architecture/specs/document-properties-schemas.md §1 § Contrat du value object DocumentProperties` (décision reportée d'ADR-002) ; `cahier-strategie-test-et-recette.md §3.1`. **`TROU` partiel** — nature : *champs non modélisés* — `document-properties-schemas.md § 3` et `§ Synthèse des trous nommés` marquent `[À TRANCHER — modélisation domaine]` les champs concrets de `propertiesSchema` non encore modélisés par le domaine ; ADR-002 ne fixe pas ces champs, ils ne sont pas inventés ici |
| Code de renvoi | — |

##### TB-017 — Invariant de visibilité `Document.CanBeReadBy`

| Champ | Valeur |
|---|---|
| But | un document `PLAYER_PRIVATE` est inaccessible même au MJ `OWNER`/`GM`, sans exception, y compris pour les `LIVE_NOTE` ; un document créé est privé par défaut |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-015 |
| Périmètre d'écriture | Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-021 — module partagé : Haversack.Domain/ContentLibrary |
| Taille | M |
| Critères d'acceptation | RB-04-01, RB-06-11, RB-06-25, RB-06-26 ; `cahier-strategie-test-et-recette.md §2 § Invariant transverse : privé par défaut` ; `cahier-strategie-test-et-recette.md §3.1`, puces 2-3 ; US-04-03 §"Un joueur ne voit pas un document privé" |
| Code de renvoi | — |

##### TB-047 — Éditeur de document

| Champ | Valeur |
|---|---|
| But | le MJ compose un document complet — champ de type facultatif, contenu structuré ou libre, portée de lecture, renvois vers d'autres documents |
| Épique | US-UC-04 |
| Jalon | J1 |
| Dépend de | TB-015, TB-016, TB-017, TB-046 |
| Périmètre d'écriture | fiche `editeur-document` |
| En conflit avec | — |
| Taille | L — un seul module ; la plage CR-UC04-01 → CR-UC04-21 porte l'essentiel de la surface de vérification de cet écran |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-document/editeur-document.md` ; `zoning.md §S6 AR-11` (placement des backlinks figé) ; US-04-01, US-04-02, US-04-03, US-04-04, US-04-05, US-04-06 (scénarios nommés) ; CR-UC04-01 → CR-UC04-21 |
| Code de renvoi | — |

### US-UC-03 — Structurer un scénario

##### TB-018 — Agrégat `Scenario` sous la forme `Document` (scènes, révélations, statut)

| Champ | Valeur |
|---|---|
| But | un scénario se crée avec son titre seul, contient zéro à N scènes ordonnées, et sépare contenu privé et contenu partageable |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-015, TB-016 |
| Périmètre d'écriture | Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-021 — module partagé : Haversack.Domain/ContentLibrary |
| Taille | L — un seul module ; les plages RB-03-01 → RB-03-13 et CR-UC03-01 → CR-UC03-21 portent l'essentiel de la surface de vérification de l'agrégat Scenario |
| Critères d'acceptation | RB-03-01 → RB-03-13 ; US-03-01 §"Le titre est le seul champ obligatoire", §"Un scénario improvisé minimal est valide (A4)" ; US-03-02 §"Les scènes respectent l'ordre défini", §"Un scénario peut contenir zéro scène" ; US-03-03 §"Écrire un scénario monobloc sans scènes" ; US-03-04 §"Le contenu privé et le contenu partageable sont bien séparés" ; `conception/domain/content-library.md § Cas d'usage illustrés` ; CR-UC03-01 → CR-UC03-21 |
| Code de renvoi | — |
| Trou balisé | `cahier-strategie-test-et-recette.md §12 PO-09` — exception E2 de la fiche UC-03 (perte de connexion / erreur de sauvegarde) non dérivable d'un Gherkin ni d'une RB ferme, non recettée |

##### TB-048 — Éditeur de scénario

| Champ | Valeur |
|---|---|
| But | le MJ construit un scénario avec ses scènes liées et ses documents associés, cas spécialisé de l'éditeur de document |
| Épique | US-UC-03 |
| Jalon | J1 |
| Dépend de | TB-018, TB-047 |
| Périmètre d'écriture | fiche `editeur-scenario` |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `wireframes/sv3-preparation/editeur-scenario/editeur-scenario.md` ; US-03-02 §"Ajouter une scène à un scénario existant", §"Modifier le titre et le contenu d'une scène", §"Supprimer une scène du scénario" ; US-03-04 §"Le MJ ajoute une révélation depuis l'éditeur de scène" ; US-03-07 §"Créer un document à la volée depuis l'éditeur de scénario" |
| Code de renvoi | — |

### US-UC-06 — Utiliser la vue session

##### TB-019 — Agrégat `Session` et machine d'états `LIVE → CLOSED → ARCHIVED`

| Champ | Valeur |
|---|---|
| But | chaque transition est irréversible et `ARCHIVED` refuse toute modification ; une seule session `LIVE` par espace |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-013, TB-016 |
| Périmètre d'écriture | Haversack.Domain/SessionConduct |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/SessionConduct ; TB-020 — module partagé : Haversack.Domain/SessionConduct ; TB-021 — module partagé : Haversack.Domain/SessionConduct ; TB-022 — module partagé : Haversack.Domain/SessionConduct |
| Taille | L — un seul module ; la plage CR-UC06-01 → CR-UC06-29 porte vingt-neuf preuves à elle seule, indivisibles de la machine d'états de Session |
| Critères d'acceptation | RB-06-18, RB-06-20, RB-06-21 ; `cahier-strategie-test-et-recette.md §3.1`, puce 1 ; `conception/domain/session-conduct.md § Machine d'états de Session`, `§ Invariants métier` ; US-06-01 §"Lancement refusé si titre vide" ; US-06-06 §"Le MJ archive une session CLOSED" ; US-07-01 §"Création impossible en ARCHIVED (E2)" ; CR-UC06-01 → CR-UC06-29 |
| Code de renvoi | — |

##### TB-020 — `SessionViewConfig` et `SessionViewFolder`

| Champ | Valeur |
|---|---|
| But | le MJ configure ses panneaux hors session, les modifie en direct, et la configuration persiste d'une session à l'autre |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-019 |
| Périmètre d'écriture | Haversack.Domain/SessionConduct |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/SessionConduct ; TB-019 — module partagé : Haversack.Domain/SessionConduct ; TB-021 — module partagé : Haversack.Domain/SessionConduct ; TB-022 — module partagé : Haversack.Domain/SessionConduct |
| Taille | M |
| Critères d'acceptation | US-06-02 §"Le MJ configure ses panneaux sans lancer de session", §"Le MJ modifie les panneaux en direct pendant une session LIVE", §"La config persiste entre deux sessions" ; `conception/domain/session-conduct.md § SessionViewConfig (agrégat)` ; `zoning.md §S6 AR-18` |
| Code de renvoi | — |
| Point d'attention | `ADR-016-serialisation-locale-migration.md §1.2 § Exclusions explicites` exclut `session_view_configs` et `session_view_folders` du périmètre migré (« préférence d'affichage, recréée à l'import comme à la création d'espace ») — la persistance locale visée par cette tâche et l'exclusion du payload de migration ne se contredisent pas ; ce point est distinct de la contradiction ADR-016/ADR-017 portée par TB-030 |

##### TB-021 — Notes de session (`LIVE_NOTE`) et épinglage

| Champ | Valeur |
|---|---|
| But | une note de session MJ naît `GM_ONLY`, une note joueur naît `PLAYER_PRIVATE` et reste hors d'atteinte du MJ ; épingler et partager sont deux gestes indépendants |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-017, TB-019 |
| Périmètre d'écriture | Haversack.Domain/SessionConduct ; Haversack.Domain/ContentLibrary |
| En conflit avec | TB-003 — module partagé : Haversack.Domain/ContentLibrary, Haversack.Domain/SessionConduct ; TB-014 — module partagé : Haversack.Domain/ContentLibrary ; TB-015 — module partagé : Haversack.Domain/ContentLibrary ; TB-016 — module partagé : Haversack.Domain/ContentLibrary ; TB-017 — module partagé : Haversack.Domain/ContentLibrary ; TB-018 — module partagé : Haversack.Domain/ContentLibrary ; TB-019 — module partagé : Haversack.Domain/SessionConduct ; TB-020 — module partagé : Haversack.Domain/SessionConduct ; TB-022 — module partagé : Haversack.Domain/SessionConduct |
| Taille | L — deux modules (ContentLibrary et SessionConduct, le type `LIVE_NOTE` étant un `Document`) et 4 renvois ; le geste métier (une note née dans une session) est indivisible entre les deux bounded contexts qu'il traverse |
| Critères d'acceptation | RB-06-11, RB-06-25, RB-06-26, RB-08-08b, RB-08-08c ; US-06-04 §"Le MJ crée une note de session privé MJ", §"Le MJ rend une note de session publique" ; US-06-05 §"Le MJ épingle un document", §"Épinglage automatique à la création à la volée" ; `cahier-strategie-test-et-recette.md Annexe A § Visibilité vs épinglage` ; `zoning.md §S6 AR-12` |
| Code de renvoi | — |

##### TB-030 — Stores de session dans le store local

| Champ | Valeur |
|---|---|
| But | les sessions terminées, les épinglages et les références de notes de session survivent à la fermeture du navigateur en mode local |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-019, TB-020, TB-021, TB-025 |
| Périmètre d'écriture | TROU — nature : aucun object store n'est nommé pour les entités de session, du fait de la contradiction ci-dessous |
| En conflit avec | TB-037 — recouvrement à confirmer à J0 : la fonction de projection store→payload (TB-037) consomme le résultat de cette tâche ; le périmètre exact de l'une comme de l'autre reste indéterminable tant que D-b-01 n'est pas tranchée |
| Taille | S |
| Critères d'acceptation | **`TROU`** — nature : *contradiction entre deux ADR sources, non arbitrée*. `ADR-017-modele-indexeddb-local.md §1.1` exclut du store local « toute entité de session » (`sessions`, `session_view_configs`, `session_pinned_documents`, `session_live_notes`, `session_view_folders`), en se déclarant « exclusions identiques à ADR-016 ». `ADR-016-serialisation-locale-migration.md §1.2`, sous une annotation de correction, qualifie cette prémisse de « factuellement fausse » et inverse l'exclusion : seules restent exclues les sessions `LIVE`, `session_view_configs` et `session_view_folders` — `sessions` (CLOSED/ARCHIVED), `session_pinned_documents` et `session_live_notes` entrent au contraire dans le périmètre sérialisé. Le critère de sortie de J1 (`roadmap-entree-build.md §3.2`, puce 1) n'énumère que la liste d'ADR-017 : une livraison conforme à ce critère ne persisterait aucune session en local, ce que US-06-09 §"Le MJ reprend une session après interruption" et NFR-OFF-02 exigent par ailleurs. Aucun critère d'acceptation n'existe pour cette tâche tant que la contradiction n'est pas tranchée |
| Code de renvoi | — |

##### TB-050 — Paramètres de campagne

| Champ | Valeur |
|---|---|
| But | le MJ configure les dossiers mis en avant en vue session et leur ordre, depuis l'écran paramètres |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-020, TB-045 |
| Périmètre d'écriture | fiche `parametres-campagne` |
| En conflit avec | TB-038 — module partagé : fiche `parametres-campagne` |
| Taille | M |
| Critères d'acceptation | `wireframes/sv3-preparation/parametres-campagne/parametres-campagne.md` ; `zoning.md §S6 AR-18` (configuration logée en surface session, paramètres = renvoi), `AR-22` (désarchivage) ; RB-02-22, RB-02-23 |
| Code de renvoi | — |
| Note de portée | la génération de lien d'invitation également logée sur cette fiche relève de J3 (US-UC-11, US-UC-08) — hors périmètre de cette tâche |

##### TB-051 — Vue session MJ (trois modes)

| Champ | Valeur |
|---|---|
| But | le MJ pilote une session — accès aux scènes, notes et contenu épinglé, prise de note rapide — dans chacun des trois modes configuration / LIVE / consultation `CLOSED` |
| Épique | US-UC-06 |
| Jalon | J1 |
| Dépend de | TB-019, TB-020, TB-021, TB-030, TB-041 |
| Périmètre d'écriture | fiche `vue-session-mj` |
| En conflit avec | — |
| Taille | L — un seul module déclaré (une fiche d'écran unique), mais 10 US, 30 RB et 29 CR distincts s'y rattachent : la surface de vérification dépasse manifestement le plafond `M` par le seul décompte de renvois. Cette tâche est signalée comme candidate à un redécoupage par mode (configuration / conduite LIVE / consultation CLOSED) au moment où l'équipe de build affine le grain — ce document ne tranche pas cette coupe, qui touche à la fois le contenu de l'écran et non son identifiant de module |
| Critères d'acceptation | `wireframes/sv-session/vue-session-mj/vue-session-mj.md` (modes configuration / LIVE / consultation CLOSED) ; `zoning.md §S6 AR-02`, `AR-04`, `AR-09` ; US-06-01, US-06-02, US-06-03, US-06-04, US-06-05, US-06-06, US-06-09, US-06-10 (scénarios nommés) ; NFR-PERF-01, NFR-PERF-02, NFR-PERF-03 ; CR-UC06-01 → CR-UC06-29 (hors cas joueur) |
| Code de renvoi | — |

### US-UC-07 — Créer un élément à la volée

##### TB-022 — Création à la volée depuis la session

| Champ | Valeur |
|---|---|
| But | depuis une session `LIVE`, un document se crée sans quitter le contexte ; refus si titre vide ; impossible en `ARCHIVED` |
| Épique | US-UC-07 |
| Jalon | J1 |
| Dépend de | TB-019, TB-015 |
| Périmètre d'écriture | Haversack.Application ; Haversack.Domain/SessionConduct |
| En conflit avec | TB-002 — module partagé : Haversack.Application ; TB-003 — module partagé : Haversack.Domain/SessionConduct ; TB-010 — module partagé : Haversack.Application ; TB-019 — module partagé : Haversack.Domain/SessionConduct ; TB-020 — module partagé : Haversack.Domain/SessionConduct ; TB-021 — module partagé : Haversack.Domain/SessionConduct ; TB-023 — module partagé : Haversack.Application ; TB-024 — module partagé : Haversack.Application ; TB-053 — module partagé : Haversack.Application |
| Taille | L — deux modules et 4 renvois : la surface combinée dépasse le plafond `M` réservé à deux modules (§3) ; le geste de création à la volée traverse Application et SessionConduct par construction |
| Critères d'acceptation | RB-07-01 → RB-07-08 ; les scénarios de US-07-01 ; CR-UC07-01 → CR-UC07-09 ; NFR-PERF-03 |
| Code de renvoi | — |

##### TB-052 — Panneau de création rapide

| Champ | Valeur |
|---|---|
| But | le MJ produit un document improvisé en quelques secondes, sans quitter l'écran de session |
| Épique | US-UC-07 |
| Jalon | J1 |
| Dépend de | TB-022, TB-051 |
| Périmètre d'écriture | fiche `panneau-creation-rapide` |
| En conflit avec | — |
| Taille | L — un seul module ; la plage CR-UC07-01 → CR-UC07-09 porte l'essentiel de la surface de vérification de cet écran |
| Critères d'acceptation | `wireframes/sv-session/panneau-creation-rapide/panneau-creation-rapide.md` (« sur-couche sans navigation propre ») ; les scénarios de US-07-01 ; US-07-02 §"Le MJ ajoute un PNJ rétroactivement en CLOSED (A5)" ; NFR-PERF-03 ; CR-UC07-01 → CR-UC07-09 |
| Code de renvoi | — |

### EP-06 — Contrats Application du périmètre local

##### TB-023 — Câbler l'invariant d'autorisation dans les contrats Application

| Champ | Valeur |
|---|---|
| But | les contrats de la couche Application portent `IResourceAccessPolicy` ; aucun chemin de lecture ne le contourne |
| Épique | EP-06 |
| Jalon | J1 |
| Dépend de | TB-010 — **préalable bloquant à l'ouverture de J1** |
| Périmètre d'écriture | Haversack.Application |
| En conflit avec | TB-002 — module partagé : Haversack.Application ; TB-010 — module partagé : Haversack.Application ; TB-022 — module partagé : Haversack.Application ; TB-024 — module partagé : Haversack.Application ; TB-053 — module partagé : Haversack.Application |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.2` — « Le contrat `IResourceAccessPolicy` défini en J0 doit être intégré aux contrats Application "avant le début du jalon J1" » (`ADR-007-rgpd-autorisation-api.md § Conséquences`) ; `roadmap-entree-build.md §5.2` |
| Code de renvoi | — |

##### TB-024 — Cas d'usage Application du périmètre local

| Champ | Valeur |
|---|---|
| But | créer un espace, un dossier, un document, une session, changer une visibilité — chaque geste passe par un cas d'usage Application unique, consommé aussi bien par le mode local que par le futur serveur |
| Épique | EP-06 |
| Jalon | J1 |
| Dépend de | TB-013, TB-014, TB-015, TB-016, TB-017, TB-018, TB-019, TB-020, TB-021, TB-022, TB-023 |
| Périmètre d'écriture | Haversack.Application |
| En conflit avec | TB-002 — module partagé : Haversack.Application ; TB-010 — module partagé : Haversack.Application ; TB-022 — module partagé : Haversack.Application ; TB-023 — module partagé : Haversack.Application ; TB-053 — module partagé : Haversack.Application |
| Taille | M |
| Critères d'acceptation | `structure-projets.md §2` (sens des dépendances) ; `guide-conventions-et-dod.md §1 § Architecture` ; l'ensemble des RB des UC-02, UC-03, UC-04, UC-05, UC-06, UC-07 |
| Code de renvoi | — |

### EP-07 — Socle technique du store local IndexedDB

##### TB-025 — Object stores et clés primaires

| Champ | Valeur |
|---|---|
| But | le store local expose les object stores nommés, avec leurs clés primaires, et se crée à la première ouverture |
| Épique | EP-07 |
| Jalon | J1 |
| Dépend de | TB-024 |
| Périmètre d'écriture | store local `spaces` ; store local `folders` ; store local `documents` ; store local `document_blocks` ; store local `document_links` ; store local `document_tags` ; store local `document_types` |
| En conflit avec | TB-026 — module partagé : store local `documents`, `folders`, `document_types`, `document_blocks`, `document_links`, `document_tags` ; TB-027 — module partagé : l'ensemble des mêmes object stores ; TB-028 — module partagé : store local `documents`, `document_blocks` |
| Taille | L — modules déclarés en nombre égal aux object stores créés en un seul geste d'initialisation du store local, `ADR-017 §1.1` les présentant comme un tableau unique (le compte se lit sur la ligne `Périmètre d'écriture` ci-dessus) |
| Critères d'acceptation | `ADR-017-modele-indexeddb-local.md §1.1 § Object stores` (tableau nommant `spaces`, `folders`, `documents`, `document_blocks`, `document_links`, `document_tags`, `document_types` avec leurs clés) ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 1 |
| Code de renvoi | M1, L1 (`roadmap-entree-build.md §5.3`) |

##### TB-026 — Index locaux

| Champ | Valeur |
|---|---|
| But | lister le contenu d'un espace, naviguer un dossier, lire un document et chercher par titre s'exécutent sur index, sans balayage complet |
| Épique | EP-07 |
| Jalon | J1 |
| Dépend de | TB-025 |
| Périmètre d'écriture | store local `documents` ; store local `folders` ; store local `document_types` ; store local `document_blocks` ; store local `document_links` ; store local `document_tags` |
| En conflit avec | TB-025 — module partagé : store local `documents`, `folders`, `document_types`, `document_blocks`, `document_links`, `document_tags` ; TB-027 — module partagé : les mêmes object stores ; TB-028 — module partagé : store local `documents`, `document_blocks` |
| Taille | L — modules déclarés en nombre égal aux object stores couverts par les index (`by_space`, `by_folder`, `by_document`, `by_title`, `ADR-017 §1.3` — le compte se lit sur la ligne `Périmètre d'écriture` ci-dessus) |
| Critères d'acceptation | `ADR-017-modele-indexeddb-local.md §1.3 § Indexes locaux` (tableau `by_space` / `by_folder` / `by_document` / `by_title`, avec la note distinguant `source_id` de `document_id`) ; NFR-PERF-04 |
| Code de renvoi | — |

##### TB-027 — Versionnement du store et stratégie d'upgrade

| Champ | Valeur |
|---|---|
| But | une montée de version du schéma local s'applique sans perte de données existantes |
| Épique | EP-07 |
| Jalon | J1 |
| Dépend de | TB-025 |
| Périmètre d'écriture | store local `spaces` ; store local `folders` ; store local `documents` ; store local `document_blocks` ; store local `document_links` ; store local `document_tags` ; store local `document_types` |
| En conflit avec | TB-025 — module partagé : l'ensemble des mêmes object stores ; TB-026 — module partagé : store local `documents`, `folders`, `document_types`, `document_blocks`, `document_links`, `document_tags` ; TB-028 — module partagé : store local `documents`, `document_blocks` |
| Taille | L — modules déclarés en nombre égal aux object stores (le versionnement `onupgradeneeded` d'IndexedDB porte sur la base entière, donc sur l'ensemble des object stores — le compte se lit sur la ligne `Périmètre d'écriture` ci-dessus) |
| Critères d'acceptation | `ADR-017-modele-indexeddb-local.md §1.4 § Versionnement du store et stratégie d'upgrade` ; NFR-OFF-03 (aucune perte silencieuse) |
| Code de renvoi | — |

##### TB-028 — Validations TypeScript minimales

| Champ | Valeur |
|---|---|
| But | un titre vide et une structure de blocs invalide sont refusés côté local ; aucune validation locale n'est plus stricte que la validation serveur |
| Épique | EP-07 |
| Jalon | J1 |
| Dépend de | TB-025 |
| Périmètre d'écriture | store local `documents` ; store local `document_blocks` |
| En conflit avec | TB-025 — module partagé : store local `documents`, `document_blocks` ; TB-026 — module partagé : store local `documents`, `document_blocks` ; TB-027 — module partagé : store local `documents`, `document_blocks` |
| Taille | L — deux modules et 3 renvois : la surface combinée dépasse le plafond `M` réservé à deux modules (§3) |
| Critères d'acceptation | `structure-projets.md §7 § Le mode local : persistance navigateur + validations minimales` (les validations y sont énumérées) ; invariant `validation locale ⊆ validation serveur` (`ADR-001-execution-domaine-mode-local.md § Compléments post-revue` ; `guide-conventions-et-dod.md §3`). **`TROU`** — nature : *invariant non outillable* — `roadmap-entree-build.md §3.2` le qualifie de « discipline de conception, non garantie outillée (pas de test cross-langage TS/C#) » ; aucun critère d'acceptation vérifiable n'existe pour l'invariant lui-même, seulement pour les validations nommées |
| Code de renvoi | — |

##### TB-029 — Services Angular d'accès au store (`P6`)

| Champ | Valeur |
|---|---|
| But | les composants Angular n'accèdent jamais à IndexedDB directement ; toutes les lectures/écritures locales passent par un service |
| Épique | EP-07 |
| Jalon | J1 |
| Dépend de | TB-025, TB-026, TB-027, TB-028 |
| Périmètre d'écriture | TROU — nature : composant de service Angular transverse — ni projet .NET, ni namespace du Domaine, ni object store, ni fiche d'écran de wireframe ; le corpus ne nomme aucun répertoire pour la couche Angular (`structure-projets.md §7` en décrit le périmètre fonctionnel, pas l'emplacement) |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md Annexe A`, ligne J1 — « Services Angular IndexedDB (wrappers, `DomSanitizer`, bandeaux durabilité/confidentialité, CSP complète) », code `P6`, source `ADR-017-modele-indexeddb-local.md § Points à trancher` |
| Code de renvoi | P6 |

### US-UC-01 — Mode local sans compte

##### TB-042 — Écran Accueil non authentifié

| Champ | Valeur |
|---|---|
| But | le MJ choisit de commencer sans compte et reçoit un message informatif au premier démarrage |
| Épique | US-UC-01 |
| Jalon | J1 |
| Dépend de | TB-041 |
| Périmètre d'écriture | fiche `accueil` |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `wireframes/sv1-transversaux/accueil/accueil.md § Zones et hiérarchie` ; US-01-01 §"Le MJ choisit de commencer sans compte", §"Le MJ reçoit un message informatif au premier démarrage" |
| Code de renvoi | — |

### EP-08 — Durabilité et sécurité du mode local

##### TB-031 — `navigator.storage.persist()` (`G-08`) traité comme état de première classe

| Champ | Valeur |
|---|---|
| But | à l'entrée en mode local, la demande de persistance est émise, son résultat est lu, et l'état (accordé / refusé-best-effort) est disponible à l'interface |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-025 — **préalable bloquant à lever avant J1** |
| Périmètre d'écriture | TROU — nature : appel d'API navigateur au niveau de la base entière, ni un object store nommé, ni un écran |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 2 ; `ADR-017-modele-indexeddb-local.md §3` ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 2 |
| Code de renvoi | — |

##### TB-032 — Bandeau de durabilité

| Champ | Valeur |
|---|---|
| But | quand la conservation permanente n'est pas garantie, un bandeau non bloquant l'indique et propose la création d'un compte |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-031 |
| Périmètre d'écriture | TROU — nature : composant Angular transverse, présent sur toute surface MJ, sans fiche d'écran ni store propre |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | RB-01-04 (révisée) ; `usecases/UC-01-mode-local-sans-compte.md § Mode local (sans compte)` — « Bandeau de durabilité » ; `user-stories/US-UC-01-mode-local-sans-compte.md § US-01-03 § Révision apportée` (le bandeau est distribué dans US-01-02) ; NFR-OFF-02, NFR-OFF-03 ; `zoning.md §S7` |
| Code de renvoi | — |

##### TB-033 — Bandeau de confidentialité

| Champ | Valeur |
|---|---|
| But | en mode local, un bandeau distinct signale que les données ne sont pas chiffrées au repos, sans bloquer l'usage |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-029 |
| Périmètre d'écriture | TROU — nature : composant Angular transverse, distinct du bandeau de durabilité mais de même nature non nommable |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | RB-01-14 ; `usecases/UC-01-mode-local-sans-compte.md § Mode local (sans compte)` — « Bandeau de confidentialité » ; NFR-CONF-04 ; `user-stories/US-UC-01-mode-local-sans-compte.md § US-01-03 § Révision apportée` (« systématique en mode local […] câblé à l'invite UC-10, non bloquant ») |
| Code de renvoi | — |

##### TB-034 — Sanitisation côté client et politique CSP

| Champ | Valeur |
|---|---|
| But | aucun contenu affiché ne peut déclencher l'exécution de code ; la CSP est servie et observable |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-029 |
| Périmètre d'écriture | TROU — nature : configuration de la page hôte et code de sanitisation Angular, ni un projet .NET, ni un écran |
| En conflit avec | TB-035 — recouvrement à confirmer à J0 : les deux tâches portent sur la même configuration CSP, non nommable dans la maille |
| Taille | M |
| Critères d'acceptation | `guide-conventions-et-dod.md §7 § Conventions de sécurité de code` — critères d'acceptation non négociables énumérés (liste blanche positive identique serveur/client ; `<script>` et attributs `on*` interdits sans exception ; ordre valider-puis-sanitiser avant toute écriture ; posture CSP `default-src 'self'` / `script-src 'self'` / `connect-src 'self'`) ; `specs/sanitisation-csp.md §1`, `§2`, `§4` ; `usecases/UC-01-mode-local-sans-compte.md § Critères d'acceptation` — « Aucun contenu, importé ou saisi, ne peut déclencher l'exécution de code lors de son affichage ». **`TROU` partiel** — nature : *dette balisée* — `guide-conventions-et-dod.md §7 § Non couvert par le corpus` : « `[À TRANCHER — B1.2 / P6]` : la liste exhaustive des balises HTML autorisées, la bibliothèque de sanitisation exacte, et les directives CSP complètes » |
| Code de renvoi | B1.2, P6 |

##### TB-035 — Vérifier l'absence d'appel réseau dans le périmètre mode local

| Champ | Valeur |
|---|---|
| But | aucun appel vers l'API Haversack n'est émis en mode local, et cette absence est observable |
| Épique | EP-08 |
| Jalon | J1 |
| Dépend de | TB-034 |
| Périmètre d'écriture | TROU — nature : configuration CSP et test, même terrain que TB-034 |
| En conflit avec | TB-034 — recouvrement à confirmer à J0 : même configuration CSP, non nommable dans la maille |
| Taille | M |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 4 — « Aucun appel réseau vers l'API Haversack n'existe dans le périmètre mode local livré (observable via la CSP `connect-src 'self'`) » ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 4 ; NFR-CONF-02 |
| Code de renvoi | — |

### EP-09 — Export d'espace, version minimale

##### TB-036 — Enveloppe de payload versionnée

| Champ | Valeur |
|---|---|
| But | un export produit une enveloppe portant `schemaVersion`, `exportedAt`, `appVersion`, `spaces` ; un payload sans `schemaVersion` est rejeté comme malformé |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-025 |
| Périmètre d'écriture | TROU — nature : fonction TypeScript d'assemblage du payload, distincte des object stores qu'elle lit — ni store nommé, ni écran |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `ADR-016-serialisation-locale-migration.md §1.1 § Enveloppe du payload` (structure littérale) ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 3 |
| Code de renvoi | — |

##### TB-037 — Fonction de projection `store local → payload`

| Champ | Valeur |
|---|---|
| But | la projection produit exactement le périmètre sérialisé, en distinguant champs gouvernés et champs libres |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-036, TB-030 |
| Périmètre d'écriture | TROU — nature : fonction de projection transverse aux object stores, non nommable |
| En conflit avec | TB-030 — recouvrement à confirmer à J0 : le périmètre exact des deux tâches reste indéterminable tant que la contradiction D-b-01 (TB-030) n'est pas tranchée |
| Taille | M |
| Critères d'acceptation | `ADR-016-serialisation-locale-migration.md §1.2 § Périmètre sérialisé` (les 10 entrées listées) ; `§1.3 § Champs gouvernés et champs libres` ; `§1.4 § Format comme contrat versionné stable` (la couture de projection est nommée comme livrable) ; `ADR-017-modele-indexeddb-local.md §1.2` (« la fonction store local → payload reste quasi-identitaire »). **Dépendance bloquée** : la « quasi-identité » énoncée par ADR-017 §1.2 est fausse tant que TB-030 n'est pas tranchée — 3 des 10 collections du périmètre sérialisé (`sessions`, `session_pinned_documents`, `session_live_notes`) n'existent pas dans la liste d'object stores d'ADR-017 §1.1 |
| Code de renvoi | — |

##### TB-038 — Déclencher l'export depuis les paramètres

| Champ | Valeur |
|---|---|
| But | depuis les paramètres, le MJ obtient un fichier d'export de son espace ou de son espace personnel, en mode local comme avec un compte |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-037, TB-047 |
| Périmètre d'écriture | fiche `parametres-campagne` |
| En conflit avec | TB-050 — module partagé : fiche `parametres-campagne` |
| Taille | M |
| Critères d'acceptation | US-01-07 §"Le MJ exporte sa campagne depuis les paramètres (mode local)", §"Le MJ exporte sa campagne depuis les paramètres (compte cloud)" ; `usecases/UC-01-mode-local-sans-compte.md § A4a` et `§ Critères d'acceptation` (« Must Have / version minimale […] promu ») ; `vision/moscow.md § Export d'espace § Critère de sortie` ; `wireframes/README.md` (correction UI, « export MVP ») |
| Code de renvoi | — |
| Écart de priorité signalé | la story `US-UC-01-mode-local-sans-compte.md § US-01-07` porte toujours la mention « Should Have » en prose et en tableau de métadonnées ; `vision/moscow.md § Export d'espace` — seule autorité MoSCoW du corpus — l'a promue Must Have par décision produit ultérieure, tout comme `usecases/UC-01-mode-local-sans-compte.md § Critères d'acceptation`. Cette tâche suit la priorité de `moscow.md` (Must Have) ; l'US porte une mention non résorbée, à corriger dans la source, pas ici |

##### TB-039 — Dry-run structurel de validation du payload

| Champ | Valeur |
|---|---|
| But | un export produit est validable structurellement sans serveur cloud actif |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-036, TB-037 |
| Périmètre d'écriture | TROU — nature : fonction de validation structurelle, non nommable |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md §3.2 § Critères de sortie factuels`, puce 3 — « […] est structurellement rejouable (dry-run de validation possible côté contrat, sans nécessiter de serveur cloud actif pour cette vérification structurelle) » ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only », puce 3 |
| Code de renvoi | — |

##### TB-040 — Test e2e multi-versions IndexedDB (`P7`, part J1)

| Champ | Valeur |
|---|---|
| But | la fonction de projection produit un payload valide depuis chaque version supportée du store local |
| Épique | EP-09 |
| Jalon | J1 |
| Dépend de | TB-027, TB-037, TB-039 |
| Périmètre d'écriture | TROU — nature : projet de test e2e, non nommé par le corpus |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | `roadmap-entree-build.md Annexe A`, ligne J1 — « e2e multi-versions IndexedDB (fonction de projection store→payload) », code `P7`, sources `ADR-016-serialisation-locale-migration.md § Points à trancher` et `ADR-017-modele-indexeddb-local.md § Points à trancher` ; `cahier-strategie-test-et-recette.md §11`, ligne UC-01, colonne « Tests nommés » |
| Code de renvoi | P7 |

### EP-10 — Châssis applicatif transverse

##### TB-041 — Châssis applicatif

| Champ | Valeur |
|---|---|
| But | indicateurs, bandeaux, accès compte et accessibilité transversale sont présents sur toute surface MJ |
| Épique | EP-10 |
| Jalon | J1 |
| Dépend de | TB-029, TB-032, TB-033 |
| Périmètre d'écriture | TROU — nature : composant Angular racine, hôte des bandeaux et indicateurs transversaux — aucune fiche d'écran ne le porte, `zoning.md §S7` le décrit sans lui donner de nom de module |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `zoning.md §S7 — Châssis` ; `zoning.md §S6 AR-19` (règle de densité) ; NFR-ACC-01 → NFR-ACC-04 ; `cahier-strategie-test-et-recette.md §3.6 — Axe transverse Accessibilité` |
| Code de renvoi | — |

### US-UC-14 — Rechercher et filtrer l'information

##### TB-049 — Recherche en préparation

| Champ | Valeur |
|---|---|
| But | le MJ retrouve un document de son espace par titre, en moins de cinq secondes, depuis la préparation |
| Épique | US-UC-14 |
| Jalon | J1 |
| Dépend de | TB-026, TB-046 |
| Périmètre d'écriture | fiche `recherche-preparation` |
| En conflit avec | — |
| Taille | L — un seul module ; la plage CR-UC14-01 → CR-UC14-13 porte l'essentiel de la surface de vérification de cet écran |
| Critères d'acceptation | `wireframes/sv3-preparation/recherche-preparation/recherche-preparation.md` (« titre seul au MVP ; résultats regroupés par type ») ; US-14-01 §"Recherche par titre avec résultat (nominal)", §"Recherche partielle insensible a la casse", §"Aucun resultat (A1)" ; NFR-PERF-04 ; CR-UC14-01 → CR-UC14-13 |
| Code de renvoi | — |

### EP-11 — Instrumentation de validation du MVP

##### TB-053 — Instrumenter les piliers d'activation dès le mode local

| Champ | Valeur |
|---|---|
| But | trois compteurs anonymes, sans lecture du contenu créé, attestent l'usage réel de chacun des piliers du MVP |
| Épique | EP-11 |
| Jalon | J1 |
| Dépend de | TB-024, TB-051 |
| Périmètre d'écriture | Haversack.Application |
| En conflit avec | TB-002 — module partagé : Haversack.Application ; TB-010 — module partagé : Haversack.Application ; TB-022 — module partagé : Haversack.Application ; TB-023 — module partagé : Haversack.Application ; TB-024 — module partagé : Haversack.Application |
| Taille | S |
| Critères d'acceptation | **`TROU`** — nature : *unité fonctionnelle Must Have sans aucun critère d'acceptation, dont la spec se déclare inapplicable*. Aucune UC, aucune US, aucun Gherkin, aucune RB, aucun CR ne porte l'instrumentation (mesuré : 0 occurrence de « instrumentation\|télémétrie\|analytics » dans `usecases/`, `user-stories/`, `user-journeys/`) ; `architecture/specs/telemetrie.md`, bandeau « Statut » : « cadre à compléter — non implémentable en l'état, les trous nommés ci-dessous bloquent le passage en développement » — 15 marqueurs `[À TRANCHER]` y sont mesurés, dont le seuil N du pilier préparation, l'opérationnalisation de « usage réel constaté », l'outil analytics, la technique d'anonymisation et le mécanisme de capture email |
| Code de renvoi | — |

##### TB-054 — Capture de contact non bloquante en mode local

| Champ | Valeur |
|---|---|
| But | le MJ peut laisser une adresse de contact ; le refus n'a aucune conséquence sur l'accès ni le fonctionnement |
| Épique | EP-11 |
| Jalon | J1 |
| Dépend de | TB-041 |
| Périmètre d'écriture | TROU — nature : composant Angular transverse, sans fiche d'écran ni store propre |
| En conflit avec | — |
| Taille | S |
| Critères d'acceptation | **`TROU`** — nature : *mécanisme non défini* — `architecture/specs/telemetrie.md §4.2 § Ce qui reste ouvert` : « Mécanisme de capture email non bloquante : point de sollicitation dans le parcours, comportement si le MJ refuse ou ignore, traitement de la donnée collectée. `[À TRANCHER — ticket]` ». Seule l'exigence de résultat est actée (`vision/moscow.md § Instrumentation de validation du MVP § Capture de contact non bloquante`) |
| Code de renvoi | — |

### EP-12 — Recette et clôture du jalon J1

##### TB-055 — Exécuter le cahier de recette du périmètre J1

| Champ | Valeur |
|---|---|
| But | chaque cas de recette du périmètre local porte un verdict |
| Épique | EP-12 |
| Jalon | J1 |
| Dépend de | l'ensemble des tâches TB-013 → TB-054 ci-dessus |
| Périmètre d'écriture | TROU — nature : colonne Verdict d'un document de recette, hors maille de désignation (ne vise aucun module de code) |
| En conflit avec | — |
| Taille | M |
| Critères d'acceptation | `cahier-strategie-test-et-recette.md §8 — Critères d'entrée/sortie globaux & Definition of Done de test` ; `cahier-strategie-test-et-recette.md §9`, ligne « Local-only » (4 critères) ; `roadmap-entree-build.md §3.2 § Critères de sortie factuels` (4 critères) |
| Code de renvoi | — |
| Point d'attention | `cahier-strategie-test-et-recette.md §13 § Contrôle d'intégrité des citations` impose, à chaque jalon, de vérifier que chaque ligne `CR-` cite un scénario Gherkin ou une RB réel et à jour dans le corpus source — un contrôle mesuré en défaut sur une partie des citations portant sur les UC de J2/J3 (hors périmètre de recette J1) |

**Recouvrements d'écriture en J1** (le calcul de parallélisation en dépend — les identifiants énumérés disent la largeur du recouvrement, non répétée en nombre) :
- `Haversack.Domain/ContentLibrary` — TB-014, TB-015, TB-016, TB-017, TB-018, TB-021.
- `Haversack.Domain/SessionConduct` — TB-019, TB-020, TB-021, TB-022.
- `Haversack.Application` — TB-022, TB-023, TB-024, TB-053.
- Les object stores du mode local — TB-025, TB-026, TB-027, TB-028, avec un recouvrement variable par store — le point de contention le plus fort du jalon mesuré par module.
- La fiche `parametres-campagne` — TB-038, TB-050.
- Les fiches `vue-campagne` et `vue-espace-personnel` — TB-045.
- Recouvrement non décidable (TROU, `recouvrement à confirmer à J0`) : TB-030 ↔ TB-037 (fonction de projection dépendant du store de session non tranché) ; TB-034 ↔ TB-035 (même configuration CSP).

---

## 7. J2 — épiques seulement

La décomposition en tâches de J2 n'est pas produite ici. Deux raisons cumulatives : le contenu réel des tâches J2 dépend de ce que J0 aura tranché sur les arborescences internes des projets .NET et namespaces (marqueurs `[À TRANCHER — J0]`, §2) — un plan qui découperait aujourd'hui des tâches à l'intérieur de ces arborescences trancherait à la place du build ; et l'entrée même en J2 est conditionnée par `[DÉCISION MARCHÉ]` (`roadmap-entree-build.md §3.3`), un point de décision `NON-VERIFIABLE-IN-BUILD` non acquis à ce jour. Descendre à la tâche pour un jalon dont l'entrée n'est pas encore actée serait du travail à refaire. Le détail des épiques J2 figure en §4 ; leur contenu, non recopié ici, est renvoyé à `roadmap-entree-build.md §3.4` et `Annexe A`.

## 8. J3 — épiques seulement

Même raisonnement qu'en §7 : J3 dépend structurellement de J2 (`roadmap-entree-build.md §3.6` — « le canal SignalR filtre par le même `IResourceAccessPolicy` que REST »), lui-même non ouvert. Le détail des épiques J3 figure en §4 ; leur contenu est renvoyé à `roadmap-entree-build.md §3.6` et `Annexe A`.

---

## 9. Lots parallélisables

Le champ `En conflit avec` de chaque tâche (§5-6) dit ce qui **ne peut pas** être parallèle ; il ne dit pas ce qui peut l'être — cette section le fait, en forme positive. Un lot regroupe des tâches dont les dépendances sont toutes closes et dont les périmètres d'écriture sont deux à deux disjoints (aucun module partagé, aucun recouvrement signalé « à confirmer à J0 »). **Convention de cette section** : seuls les lots ouvrant au moins deux tâches simultanément sont nommés `LOT-nn` — un palier de dépendance qui n'ouvre qu'une seule tâche n'ajoute aucune information à ce que le champ `Dépend de` de cette tâche porte déjà, et n'est pas répété ici sous forme de lot.

##### LOT-01

| Champ | Valeur |
|---|---|
| Ouvrable après | — (point de départ) |
| Tâches | TB-001, TB-007, TB-012 |
| Modules touchés, deux à deux disjoints | TB-001 : TROU (non-code) ; TB-007 : TROU (non-code) ; TB-012 : TROU (non-code) — trois cibles distinctes et sans rapport (confirmation ADR, choix d'outil de test d'architecture, convention de commit) |
| Ferme quand | TB-001, TB-007 et TB-012 sont clos |

##### LOT-02

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-002 |
| Tâches | TB-003, TB-010, TB-011 |
| Modules touchés, deux à deux disjoints | TB-003 : les namespaces du Domaine ; TB-010 : Haversack.Application ; TB-011 : TROU (configuration de style) |
| Ferme quand | TB-003, TB-010 et TB-011 sont clos |

##### LOT-03

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-003 (pour TB-004 et TB-006), TB-010 (pour TB-023) |
| Tâches | TB-004, TB-006, TB-023 |
| Modules touchés, deux à deux disjoints | TB-004 : Haversack.Domain/SharedKernel ; TB-006 : TROU (projet de test d'architecture) ; TB-023 : Haversack.Application |
| Ferme quand | TB-004, TB-006 et TB-023 sont clos |

##### LOT-04

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-004 (pour TB-005), TB-006 et TB-007 (pour TB-008), TB-006 (pour TB-009) |
| Tâches | TB-005, TB-008, TB-009 |
| Modules touchés, deux à deux disjoints | TB-005 : Haversack.Domain/SharedKernel ; TB-008 : TROU (configuration de pipeline) ; TB-009 : TROU (projet de test d'architecture — recouvrement avec TB-006 signalé en §5, déjà clos à ce point) |
| Ferme quand | TB-005, TB-008 et TB-009 sont clos — **fin de J0** |

##### LOT-05

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-015 et TB-016 (pour TB-018), TB-013 et TB-016 (pour TB-019) |
| Tâches | TB-018, TB-019 |
| Modules touchés, deux à deux disjoints | TB-018 : Haversack.Domain/ContentLibrary ; TB-019 : Haversack.Domain/SessionConduct — deux bounded contexts distincts |
| Ferme quand | TB-018 et TB-019 sont clos |

##### LOT-06

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-025 (pour TB-026, TB-031, TB-036), TB-019/TB-020/TB-021/TB-025 (pour TB-030) |
| Tâches | TB-026, TB-030, TB-031, TB-036 |
| Modules touchés, deux à deux disjoints | TB-026 : store local `documents`, `folders`, `document_types`, `document_blocks`, `document_links`, `document_tags` ; TB-030 : TROU (session, D-b-01) ; TB-031 : TROU (appel navigateur) ; TB-036 : TROU (enveloppe de payload) — aucun recouvrement signalé entre ces quatre cibles |
| Ferme quand | TB-026, TB-030, TB-031 et TB-036 sont clos |

##### LOT-07

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-025 → TB-028 (pour TB-029), TB-031 (pour TB-032), TB-030 et TB-036 (pour TB-037) |
| Tâches | TB-029, TB-032, TB-037 |
| Modules touchés, deux à deux disjoints | TB-029 : TROU (services d'accès au store) ; TB-032 : TROU (bandeau de durabilité) ; TB-037 : TROU (fonction de projection) — trois cibles distinctes, non signalées en recouvrement entre elles |
| Ferme quand | TB-029, TB-032 et TB-037 sont clos |

##### LOT-08

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-029 (pour TB-033 et TB-034), TB-036 et TB-037 (pour TB-039) |
| Tâches | TB-033, TB-034, TB-039 |
| Modules touchés, deux à deux disjoints | TB-033 : TROU (bandeau de confidentialité) ; TB-034 : TROU (sanitisation/CSP) ; TB-039 : TROU (dry-run de validation) — trois cibles distinctes |
| Ferme quand | TB-033, TB-034 et TB-039 sont clos |

##### LOT-09

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-034 (pour TB-035), TB-027/TB-037/TB-039 (pour TB-040), TB-029/TB-032/TB-033 (pour TB-041) |
| Tâches | TB-035, TB-040, TB-041 |
| Modules touchés, deux à deux disjoints | TB-035 : TROU (vérification réseau/CSP) ; TB-040 : TROU (test e2e multi-versions) ; TB-041 : TROU (châssis applicatif) — trois cibles distinctes |
| Ferme quand | TB-035, TB-040 et TB-041 sont clos |

##### LOT-10

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-041 (pour TB-042 et TB-054), TB-013 et TB-041 (pour TB-043), TB-019/TB-020/TB-021/TB-030/TB-041 (pour TB-051) |
| Tâches | TB-042, TB-043, TB-051, TB-054 |
| Modules touchés, deux à deux disjoints | TB-042 : fiche `accueil` ; TB-043 : fiche `tableau-de-bord` ; TB-051 : fiche `vue-session-mj` ; TB-054 : TROU (capture de contact) — quatre cibles distinctes |
| Ferme quand | TB-042, TB-043, TB-051 et TB-054 sont clos |

##### LOT-11

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-043 (pour TB-044 et TB-045), TB-022 et TB-051 (pour TB-052), TB-024 et TB-051 (pour TB-053) |
| Tâches | TB-044, TB-045, TB-052, TB-053 |
| Modules touchés, deux à deux disjoints | TB-044 : fiche `creation-espace` ; TB-045 : fiche `vue-campagne`, fiche `vue-espace-personnel` ; TB-052 : fiche `panneau-creation-rapide` ; TB-053 : Haversack.Application — quatre cibles distinctes, aucun module partagé |
| Ferme quand | TB-044, TB-045, TB-052 et TB-053 sont clos |

##### LOT-12

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-014 et TB-045 (pour TB-046), TB-020 et TB-045 (pour TB-050) |
| Tâches | TB-046, TB-050 |
| Modules touchés, deux à deux disjoints | TB-046 : fiche `navigation-dossiers` ; TB-050 : fiche `parametres-campagne` |
| Ferme quand | TB-046 et TB-050 sont clos |

##### LOT-13

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-015/TB-016/TB-017/TB-046 (pour TB-047), TB-026 et TB-046 (pour TB-049) |
| Tâches | TB-047, TB-049 |
| Modules touchés, deux à deux disjoints | TB-047 : fiche `editeur-document` ; TB-049 : fiche `recherche-preparation` |
| Ferme quand | TB-047 et TB-049 sont clos |

##### LOT-14

| Champ | Valeur |
|---|---|
| Ouvrable après | TB-037 et TB-047 (pour TB-038), TB-018 et TB-047 (pour TB-048) |
| Tâches | TB-038, TB-048 |
| Modules touchés, deux à deux disjoints | TB-038 : fiche `parametres-campagne` ; TB-048 : fiche `editeur-scenario` — le partage de module de TB-038 est avec TB-050 (LOT-12, déjà clos à ce point), pas avec TB-048 |
| Ferme quand | TB-038 et TB-048 sont clos — **fin de J1**, ouvre TB-055 |

**Étapes non regroupées en lot** (une seule tâche s'ouvre à ce palier de dépendance — voir le champ `Dépend de` de la tâche elle-même) : TB-002, TB-013, TB-014, TB-015, TB-020, TB-021, TB-022, TB-024, TB-025, TB-055.

---

## 10. Ce que ce plan n'ordonne pas

**Les deux points de décision non vérifiables en intégration continue.** `[DÉCISION MARCHÉ]` (renvoi seul : `roadmap-entree-build.md §3.3`) et `[VALIDATION JURIDIQUE EU]` (renvoi seul : `roadmap-entree-build.md §3.5`) ne sont pas des jalons de build : aucun test, aucun linter ne peut les trancher. Ce plan ne les décompose pas en tâches — il en hérite seulement les effets déjà tracés dans `roadmap-entree-build.md §3.4 § Dépendance d'entrée` (entrée en J2 conditionnée, cumulativement, par J1 stable et `[DÉCISION MARCHÉ] = go`) et `§3.5` (le lancement EU, pas le build de J3, est bloqué par `[VALIDATION JURIDIQUE EU]`).

**Les points `[À TRANCHER]`.** Ce document en signale un sous-ensemble, un par un, au fil des tâches où ils mordent directement (marqueurs `TROU` et `TROU partiel`, §5-6) — il n'en tient pas un registre séparé ni n'en recopie le décompte : le corpus les porte déjà (`guide-conventions-et-dod.md § Convention de balisage`, `structure-projets.md §3`, `architecture/specs/*.md`).

**Le post-MVP.** `UC-13` (scénario réutilisable) et `UC-15` (gel d'espaces au downgrade de tier) sont classés hors MoSCoW du MVP par `vision/moscow.md § UC-13` (paragraphe « hors première livraison ») et `§ UC-15 — Classement arbitré`, seule autorité du corpus pour cette classification. Ce plan ne leur ouvre aucune tâche ni épique décomposée, cohérent avec `moscow.md § Dépendances § Convention actée`, qui les exclut par construction de son propre diagramme.

**Trois points qui doivent rester ouverts, chacun sur un axe distinct — ce plan ne prend position sur aucune de leurs issues.**

- **Identité graphique** (axe conception d'interface) — `roadmap-entree-build.md § Annexe B` en fait un préalable hors jalon, porté par `docs/conception/interface/**`, non modifié par ce document.
- **Seuil chiffré de l'hypothèse `H1`** (axe métrique produit) — `vision/vision-produit.md §2.3` en est propriétaire ; `vision/moscow.md § Instrumentation de validation du MVP` y renvoie sans le fixer.
- **L'entrée `J-12` du registre des risques** (axe juridique) — `registre-risques.md §3.3`, méthode de vérification d'identité des invités non retenue ; le registre signale lui-même que la frontière entre « lacune réelle » et « posture existante » n'est pas tranchée par ce registre et attend un statut en comité des risques.

---

## 11. Renvois

- [`docs/gestion-projet/roadmap-entree-build.md`](roadmap-entree-build.md) — séquence J0-J3, critères de sortie par jalon, ordre C#-first, réconciliation de nomenclature, rattachement des codes de renvoi.
- [`docs/gestion-projet/guide-conventions-et-dod.md`](guide-conventions-et-dod.md) — conventions de code, gates d'intégration continue, Definition of Done, conventions de maintenance du corpus documentaire (§8, appliquées par ce document).
- [`docs/architecture/structure-projets.md`](../architecture/structure-projets.md) — granularité des projets .NET, nommage du noyau partagé, périmètre du mode local TypeScript.
- [`docs/conception/besoin/vision/moscow.md`](../conception/besoin/vision/moscow.md) — priorisation MoSCoW, graphe de dépendances entre use cases.
- [`docs/test/cahier-strategie-test-et-recette.md`](../test/cahier-strategie-test-et-recette.md) — critères de sortie par jalon, Definition of Done de test, cas de recette.
- [`docs/conception/interface/wireframes/README.md`](../conception/interface/wireframes/README.md) — table de couverture UC → fiche(s), éléments différés.
- [`docs/conception/interface/zoning.md`](../conception/interface/zoning.md) — arbitrages figés `AR-01` à `AR-22`, châssis.
- Use cases, user stories et journeys : `docs/conception/besoin/usecases/UC-NN-*.md`, `docs/conception/besoin/user-stories/US-UC-NN-*.md`.
- ADR cités : [ADR-001](../architecture/decisions/ADR-001-execution-domaine-mode-local.md), [ADR-002](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md), [ADR-004](../architecture/decisions/ADR-004-transport-temps-reel.md), [ADR-006](../architecture/decisions/ADR-006-perimetre-mvp.md), [ADR-007](../architecture/decisions/ADR-007-rgpd-autorisation-api.md), [ADR-008](../architecture/decisions/ADR-008-structure-solution.md), [ADR-011](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md), [ADR-014](../architecture/decisions/ADR-014-modele-autorisation-api.md), [ADR-015](../architecture/decisions/ADR-015-securite-authentification-mvp.md), [ADR-016](../architecture/decisions/ADR-016-serialisation-locale-migration.md), [ADR-017](../architecture/decisions/ADR-017-modele-indexeddb-local.md), [ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md).
- Spécifications citées : `docs/architecture/specs/document-properties-schemas.md`, `docs/architecture/specs/sanitisation-csp.md`, `docs/architecture/specs/telemetrie.md`, `docs/architecture/specs/mapping-ef-core.md`, `docs/architecture/specs/contrat-openapi.md`, `docs/architecture/specs/repli-temps-reel.md`, `docs/architecture/specs/config-securite-migration.md`, `docs/architecture/specs/requete-effacement-non-partage.md`.
- [`docs/gestion-projet/registre-risques.md`](registre-risques.md) — entrée `J-12`.
- [`docs/conception/besoin/vision/vision-produit.md`](../conception/besoin/vision/vision-produit.md) — hypothèse `H1`.
- [`docs/deploiement/README.md`](../deploiement/README.md) — absence de document de déploiement, hébergeur non tranché.


# Cahier de spécifications techniques — Haversack

| Champ | Valeur |
|---|---|
| Version | 1.0 |
| Date | 2026-07-16 |
| Statut | Version de référence — consolidation du corpus |
| Périmètre | MVP — trajectoire technique actée pour UC-01 à UC-12, UC-14 ; post-MVP signalé où il apparaît |
| Audiences | équipe de build, architecte, opérateur (arbitrage des points non tranchés) |
| Sources | `docs/architecture/decisions/**` (registre ADR), `docs/architecture/*.md` (fondations, structure, stack), `docs/architecture/specs/**` (spécifications pré-build), `docs/conception/domain/**` (modèle de domaine), `docs/context/cahier-des-charges.md` §7/§8 |

> Document dérivé du corpus, zéro décision technique neuve.

---

## 0. Objet & cadre d'autorité

> **Section provisoire.** Les choix décrits dans ce cahier s'appuient sur des décisions d'architecture (ADR) de nature pré-implémentation : elles cadrent une trajectoire technique retenue à ce stade de conception, mais n'ont pas encore été confrontées à la construction effective du MVP. Les éléments consolidés ci-après sont donc provisoires et susceptibles d'ajustement à l'entrée en build. Ce statut n'affaiblit pas la portée normative de chaque décision citée pour la phase courante — il en situe seulement la maturité.

### Ce que ce document fait

Ce cahier consolide, en un registre technique unique, le corpus déjà écrit — décisions d'architecture (ADR), spécifications pré-build, fiches d'exigences non-fonctionnelles, modèle de domaine — en sections miroir du cahier des charges fonctionnel. Il rend le **QUOI technique** (bornes, invariants, contrats, structures de données) vérifiable en un seul endroit, sans en déplacer l'autorité.

### Ce que ce document ne fait pas

Il ne crée aucune décision technique neuve. Il n'invente aucun seuil chiffré absent du corpus. Il ne recopie pas le contenu intégral des spécifications sources — il synthétise et renvoie. Il ne reproduit pas la vue transverse par préoccupation de `docs/architecture/architecture-detaillee.md`, ni la conception détaillée en avant (le « comment ») portée par le dossier de conception détaillée. `architecture-detaillee.md` organise le même corpus ADR par préoccupation technique ; ce cahier le met en miroir du cahier des charges. Aucun des deux ne cite l'autre comme autorité — en cas de divergence, l'ADR source fait foi — et toute évolution d'un ADR se répercute dans les deux. Il ne tranche aucun point ouvert du corpus : un point non tranché est reporté tel quel et consolidé au registre des points ouverts (§11).

### Clause d'autorité

Ce document ne porte aucune autorité propre ; il consolide et renvoie. En cas de divergence entre ce cahier et un ADR, une spécification, une fiche NFR ou le modèle de domaine, **la source fait foi**.

### Ordre d'autorité du corpus

L'ordre suivant n'est jamais renversé par ce document :

**personas → vision → use cases (source de vérité) → user stories / NFR → domaine → glossaire.**

Toute référence ajoutée par ce document pointe vers le haut de cette hiérarchie, ou vers un pair de même niveau normatif (ADR, spec pré-build) déjà accepté dans le registre. Ce cahier ne devient à aucun moment une source que le corpus amont citerait en retour.

### Frontière avec le SDD (règle opérationnelle)

Ce cahier énonce **ce qui doit être vrai** — bornes, invariants, contrats, planchers, structure de données, périmètres. Le dossier de conception détaillée (SDD, `docs/context/dossier-conception-detaillee.md`) énonce **comment le flux se déroule** — séquences, algorithmes, règles ordonnées, machines d'états déroulées pas à pas.

Cas-test à citer pour distinguer les deux registres : « toutes les FK sont `RESTRICT` et la suppression est pilotée par une saga » relève de ce cahier (QUOI) ; « la saga NULL-e les cycles en passe 1 puis DELETE les feuilles en passe 2 » relève du SDD (COMMENT). Ce cahier ne franchit jamais cette frontière — il cite la source du COMMENT en renvoi, il ne l'héberge pas.

### Convention de report des points ouverts

Tout point non tranché du corpus est reporté tel quel sous la forme `[À TRANCHER — <axe/référence>]`, jamais comblé par une valeur ou une hypothèse. Ces points sont consolidés au registre des points ouverts (§11).

---

## 1. Principes d'architecture

Cette section consolide les principes fondateurs qui cadrent l'ensemble des choix techniques du MVP : la place du mode local, la structure documentaire du contenu, et l'organisation en Clean Architecture / monolithe modulaire.

### Mode local : pilier non négociable

Travailler sans compte, avec persistance directement dans le navigateur, constitue un pilier non négociable du produit : le mode local. Techniquement, il se limite à une couche de persistance côté navigateur assortie de règles de validation minimales — création, modification, suppression, quelques contrôles élémentaires comme un titre non vide ou une structure de blocs valide — **sans dupliquer le domaine métier complet**. La **source de vérité unique** reste le domaine métier exécuté côté serveur : toute donnée locale qui rejoint le cloud y transite comme un **import revalidé**, jamais comme une copie acceptée sur confiance.

L'invariant transverse qui gouverne cette frontière est : **validation locale ⊆ validation serveur**. Le mode local peut être plus permissif en lecture, il ne doit jamais accepter en écriture un état que le serveur rejettera à l'import. L'ordre de construction acté est C#-first : le domaine serveur est échafaudé en premier, le modèle local en est une projection dérivée.

### « Tout est document »

Un principe unique gouverne le contenu éditorial : **« tout est document »**. Scénario, PNJ, lieu, note ou fiche partagent le même identifiant (`DocumentId`) et la même structure — des blocs de contenu, reliés par des références ordonnées vers d'autres documents. Aucun de ces usages ne constitue un type séparé au sens d'un identifiant propre : « Scénario », « Scène » ou « Personnage » désignent des emplois du même document, pas des entités distinctes.

L'écriture des propriétés structurées d'un document — dont sa visibilité — passe par un value object dédié qui les valide contre le schéma déclaré pour le type concerné ; un champ jusque-là libre de toute contrainte se retrouve ainsi gouverné.

### Clean Architecture, inversion des dépendances, monolithe modulaire

L'inversion des dépendances de la **Clean Architecture** structure la solution : les contrats sont définis par le domaine métier, l'infrastructure les implémente, et la présentation en dépend — jamais l'inverse. Cette règle vaut quelle que soit la granularité physique retenue pour les projets.

Le découpage retenu est celui d'un **monolithe modulaire**, autour de **quatre frontières logiques** — Identity & Access, Space Management, Content Library, Session Conduct. Pour le MVP, ces frontières vivent à l'intérieur d'une solution unique, sous forme de namespaces et de contrats internes plutôt que d'assemblies séparées par contexte ; une extraction en composants indépendants reste ouverte si un besoin réel s'en fait sentir. Les préoccupations techniques transverses, elles — persistance, notifications temps réel, présentation —, sont isolées en composants distincts dès le démarrage. La coexistence des frontières logiques dans un même assembly a une conséquence directe : leur étanchéité n'est pas garantie par le compilateur (voir §3, test d'architecture CI).

### Tableau de renvoi — §1

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-001 — Exécution du domaine en mode local | pré-implémentation | Accepté | Mode local = persistance + validations minimales ; domaine serveur = source de vérité ; invariant validation locale ⊆ serveur ; ordre C#-first |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | Identifiant unique `DocumentId` ; `DocumentProperties` gouverne l'écriture des propriétés structurées |
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | Clean Architecture, monolithe modulaire, 4 frontières logiques, multi-projets sur infrastructure/présentation |
| `docs/architecture/ddd-fondations.md` | fondations (conception) | — | Justification DDD, langage ubiquitaire, frontières explicites, règles métier dans le domaine |
| `docs/architecture/architecture-detaillee.md` §1 | consolidation transverse | — | Renvoi vers le Dossier d'Architecture Technique pour la vue par bounded context |
| CdC §7.1 | synthèse produit | — | Formulation d'origine des trois principes ci-dessus |

---

## 2. Stack & choix technologiques

Cette section nomme les technologies retenues pour le MVP et les mécanismes structurants associés — la nomination des technologies est pertinente à ce niveau du cahier, à la différence des sections centrées sur le besoin fonctionnel.

### Front : mono-écosystème Angular

**Angular** est l'unique écosystème front, aussi bien pour l'application principale (SPA) que pour la landing page — un choix qui évite à une équipe réduite de maintenir deux pipelines et deux jeux de composants distincts. La landing page en tire une déclinaison en rendu statique préconstruit (SSG/prerender), servie depuis un CDN sans surcharge serveur, pour répondre à ses besoins de référencement. Blazor WASM a été évalué et écarté : il aurait pu partager le domaine C# côté navigateur, mais au prix d'un écosystème de composants moins riche et d'un poids de runtime pénalisant au premier chargement — la conséquence sur le mode local (TypeScript minimal, pas de domaine partagé) est assumée.

### Backend et persistance

Le backend est construit en **ASP.NET Core / C#**, avec **PostgreSQL** comme base de données (recherche plein texte native, JSONB pour les blocs de contenu, UUID), **Entity Framework Core** comme ORM, et **ASP.NET Identity** pour l'authentification.

### Structure de la solution

La solution est structurée en six projets : `SharedKernel`, `Domain`, `Application`, `Infrastructure.Persistence`, `Infrastructure.Notifications`, `Api` — complétés par `Presentation.Landing` pour la landing Angular. Domaine et Application restent des projets uniques pour le MVP (les quatre frontières logiques y sont des namespaces, pas des assemblies séparées) ; la granularité multi-projets est conservée sur l'infrastructure et la présentation, qui reflètent des préoccupations techniques distinctes du découpage DDD. Le détail des responsabilités et de la structure interne de chaque projet est renvoyé à `docs/architecture/structure-projets.md`.

### Temps réel : SignalR

Le partage d'informations en temps réel du MJ vers les joueurs s'appuie sur **SignalR**, natif à ASP.NET Core, avec **repli automatique** (WebSocket → Server-Sent Events → long-polling) selon les capacités du réseau. Le module est isolé dans `Infrastructure.Notifications` dès le démarrage. La visibilité de la ressource concernée conditionne systématiquement ce qui est poussé : aucune information diffusée ne contourne les règles de partage fixées par le MJ, la même politique d'autorisation s'appliquant identiquement au canal REST et au canal SignalR (voir §3).

### Frontière d'exécution local / cloud

Le domaine C# serveur est la vérité unique de l'exécution métier. Le mode local persiste en TypeScript/IndexedDB avec des validations minimales, sans réimplémentation du domaine. Ce partage de responsabilité découle directement d'ADR-001 et du rejet de Blazor WASM (ADR-003) : deux implémentations métier en langages différents auraient produit une divergence structurelle inévitable.

### Brique sécurité nommée

L'algorithme de hachage des mots de passe retenu est **Argon2id**, avec un repli acceptable vers **bcrypt** (cost factor ≥ 12) si Argon2id n'est pas intégrable dans la contrainte de dépendances du MVP. La signature des jetons d'authentification est **asymétrique** (RS256 ou ES256), l'algorithme `alg: none` étant explicitement interdit à la validation. Les valeurs de configuration associées (durées de vie des jetons, seuils de rate limiting) sont consolidées en §5 — cette section ne nomme que la brique technique.

### Tableau de renvoi — §2

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-003 — Stack front : mono-écosystème Angular | pré-implémentation | Accepté | Angular SPA + Angular SSR/prerender landing ; Blazor WASM et Next.js écartés |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | SignalR natif, repli automatique, isolation `Infrastructure.Notifications` |
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | 6 projets, domaine/application uniques, infrastructure/présentation multi-projets |
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Argon2id (repli bcrypt), signature asymétrique RS256/ES256, `alg: none` interdit |
| `docs/architecture/stack.md` | sélection technologique | — | Vue d'ensemble de la stack, alternatives évaluées par couche |
| `docs/architecture/structure-projets.md` | synthèse structurelle | — | Détail complet des 6 projets, granularité, nommage `SharedKernel` |
| CdC §7.2 | synthèse produit | — | Formulation d'origine de la stack |

---

## 3. Contraintes techniques

Cette section consolide les contraintes structurantes qui s'imposent à l'implémentation, au-delà du choix de stack : ce que le compilateur ne garantit pas, l'invariant de validation croisée local/serveur, l'intégrité référentielle, l'hébergement temps réel et les bornes du mode local.

### Frontières de contexte non garanties par le compilateur

Les quatre frontières logiques (§1) coexistant dans un même assembly, leur respect n'est **pas garanti par le compilateur**. Cette limite est compensée par un **test d'architecture automatisé exécuté en CI**, qui remplace la discipline de revue de code jugée insuffisante en contexte d'équipe restreinte. Ce test est un livrable attendu avant la première mise en production ; le détail de son câblage (bibliothèque, règles précises) relève du dossier de conception détaillée.

### Invariant transverse de validation

L'invariant « validation locale ⊆ validation serveur » (§1) s'applique à l'ensemble du plan technique, pas seulement au mode local : toute règle acceptée en écriture côté navigateur doit rester acceptable par le domaine serveur à la revalidation. L'ordre de build C#-first en est la conséquence directe.

### Intégrité référentielle : FK `RESTRICT`, suppression par saga

Toutes les clés étrangères du graphe de données sont déclarées **`ON DELETE RESTRICT`** — le système de gestion de base de données ne cascade jamais. La progression des suppressions est pilotée intégralement par une **saga applicative** : une transaction unique par espace supprimé, avec garantie d'idempotence et de reprise après un arrêt en cours d'exécution. Ce cahier énonce le QUOI (RESTRICT partout, saga responsable de la cascade, transaction par espace, idempotence) ; le séquençage pas à pas de la saga (ordre des passes de déliaison, ordre topologique des suppressions) relève du dossier de conception détaillée et n'est pas reproduit ici.

Le passage d'un espace en purge s'appuie sur un **claim exclusif**, posé avant l'ouverture de la transaction, qui interdit toute restauration concurrente une fois posé. Le délai au-delà duquel ce claim est considéré expiré — rendant l'espace de nouveau éligible à une tentative de purge après un arrêt en cours d'exécution — reste `[À TRANCHER — timeout d'expiration du claim purge_claimed_at de la saga de suppression, config-securite-migration.md / ADR-011 B1]`.

### Contrainte d'hébergement temps réel

Le choix de SignalR (§2) impose des connexions persistantes et des sessions collantes (« sticky sessions ») côté hébergement. La configuration retenue est sobre : repli forcé vers Server-Sent Events tant que la communication reste unidirectionnelle (MJ → joueurs), fermeture de la connexion en fin de session active, et heartbeat allongé (les événements sont rares en session de jeu de rôle). Un mécanisme de repli vers un polling adaptatif est prévu au-delà d'un seuil de coût par session concurrente — ce seuil, sa métrique et sa cadence de repli restent `[À TRANCHER — spec repli-temps-reel.md §3]`, non chiffrés dans le corpus à ce stade.

### Mode local : plafond et posture migration-only

Le mode local est plafonné à **trois espaces de type campagne ou one-shot actifs** ; ce plafond est vérifié avant toute écriture dans le store local. Sa posture de synchronisation est **migration-only** : aucune synchronisation continue entre le navigateur et le cloud n'existe au MVP — le seul passage de données vers le serveur est une migration ponctuelle, déclenchée explicitement par l'utilisateur, traitée intégralement ou pas du tout par espace.

### Stockage : quotas et paliers

Le palier local persiste les données exclusivement dans le navigateur, sans limite de volume propre au produit au-delà de celle de l'appareil. Les paliers gratuit et Pro disposent chacun d'un quota de stockage propre, le palier Pro offrant un volume sensiblement supérieur ; l'espace personnel n'est, dans les deux cas, jamais décompté du quota du palier gratuit. Les valeurs chiffrées de ces quotas restent en renvoi vers le cahier des charges (`docs/context/cahier-des-charges.md` §12.4) — elles ne sont pas recopiées ici.

### Tableau de renvoi — §3

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | Test d'architecture CI comme garde-fou des frontières logiques |
| `docs/architecture/structure-projets.md` §6 | synthèse structurelle | — | Détail du test d'architecture, règles à mettre en place |
| ADR-001 — Exécution du domaine en mode local | pré-implémentation | Accepté | Invariant validation locale ⊆ serveur, ordre C#-first |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Toutes FK `RESTRICT`, sagas `SpaceDeleted`/`UserAnonymized`, transaction par espace, idempotence |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Configuration sobre (SSE forcé, fermeture fin de session, heartbeat allongé) |
| `docs/architecture/specs/repli-temps-reel.md` §2-3 | spec pré-build | cadre à compléter | Mécanisme de repli vers polling adaptatif ; seuil, métrique, cadence non chiffrés — `[À TRANCHER]` |
| ADR-017 — Modèle IndexedDB local | mixte (dominante pré-implémentation) | Accepté | Plafond de 3 espaces, posture migration-only, absence de synchronisation continue |
| CdC §7.5 (+ renvoi §12.4) | synthèse produit | — | Quotas de stockage par palier, valeurs chiffrées en renvoi |

---

## 4. Modèle de données & domaine

Cette section consolide l'organisation du domaine métier — quatre contextes bornés adossés à un noyau partagé, principe « tout est document », ontologie de l'espace — sans reproduire le détail porté par le modèle de domaine lui-même, auquel elle renvoie systématiquement.

### « Tout est Document » : identifiant unique et structure

Il n'existe plus de type `Scénario`, `Scène` ou `Personnage` distinct au sens d'un identifiant propre : un seul identifiant, `DocumentId`, couvre l'ensemble. Un document est composé de blocs de contenu et de références ordonnées vers d'autres documents — une structure récursive qui permet au MJ de rédiger un scénario monobloc, de le structurer en scènes liées, ou de réutiliser un document comme scène dans un autre contexte, sans qu'aucune organisation ne soit imposée par le système.

### `DocumentProperties` et schémas par type

Les propriétés structurées d'un document (dont sa visibilité) sont gouvernées par un value object dédié, `DocumentProperties`, dont l'écriture passe exclusivement par une méthode de domaine validée contre le schéma déclaré (`propertiesSchema`) du type de document concerné. Aucune écriture directe dans le champ libre sous-jacent n'est permise.

Huit types de document système sont seedés pour le MVP : scénario, scène, PNJ, lieu, note, personnage joueur, note live, révélation. Parmi ces huit, le schéma structuré du type **scénario** (un statut éditorial à quatre valeurs) et l'absence de schéma structuré supplémentaire pour le type **note live** (ses deux champs distinctifs sont promus en colonnes de premier niveau, hors du périmètre du VO) sont dérivés du modèle de domaine. Les schémas structurés des six autres types système restent `[À TRANCHER — modélisation domaine, spec document-properties-schemas.md §5]` : ce cahier ne les invente pas.

### Ontologie de l'espace (`Space`)

L'agrégat central du modèle est l'**espace** (`Space`), frontière de cohérence pour les membres, les invitations et le rattachement du contenu. La campagne, le one-shot et l'espace personnel ne sont pas des concepts distincts mais des **spécialisations d'un seul agrégat**, distinguées par un type (`SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`) : les différences de comportement entre ces trois formes sont comportementales, pas structurelles. L'espace personnel est créé par défaut à la création du compte, mono-membre par construction, et n'est jamais décompté du quota du palier gratuit.

### Intégrité référentielle : matrice des FK cross-module

Le graphe de cascade compte **38 clés étrangères**, toutes déclarées `ON DELETE RESTRICT` (§3). Sur ce total, la matrice de cascade recense **17 FK cross-module** (16 « Cross » pures + 1 mixte « Intra+Cross », `documents.source_document_id`).

`[À TRANCHER — réconcilier le décompte de FK cross-module entre ADR-009 (19 FK cross-module annoncées, en renvoi vers ADR-011) et ADR-011 (17 FK cross-module recensées dans sa propre matrice)]`. Ce cahier reporte la divergence telle qu'elle apparaît dans le corpus, sans arbitrer laquelle des deux valeurs est exacte.

### Core (noyau partagé)

Le noyau partagé fournit aux quatre contextes bornés les identifiants typés (`UserId`, `SpaceId`, `GuestAccessId`, `SessionId`, `DocumentId`, `FolderId`), les value objects transverses (`Email`, `Slug`, `Tag`), les primitives de traçabilité (`AuditInfo`, `SoftDelete`) et l'énumération partagée de visibilité (`Visibility ∈ {PUBLIC, GM_ONLY, PLAYER_PRIVATE}`). Il ne contient jamais d'entité dotée d'un cycle de vie propre.

Une exception RGPD s'applique à `SoftDelete` : les contenus strictement privés à un joueur qui tombent sous une obligation légale d'effacement (fin de compte, fin d'accès invité sans conversion) sont supprimés **physiquement**, jamais simplement masqués — le masquage réversible est réservé au cycle de vie normal du contenu.

### Les quatre contextes bornés — responsabilité en un paragraphe, renvoi pour le détail

- **Identity & Access** gère les comptes utilisateurs authentifiés — identité, palier d'abonnement, suppression RGPD — sans connaître les notions de MJ, joueur, espace ou session. Détail : `docs/conception/domain/identity-access.md`.
- **Space Management** gère les espaces, leurs membres, leurs invitations et les accès invités — il décide qui entre dans quel espace et selon quel niveau d'accès, sans gérer le contenu ni le déroulement d'une session. Détail : `docs/conception/domain/space-management.md`.
- **Content Library** gère le contenu durable d'un espace — documents, dossiers, types de documents — comme référentiel de ce que le MJ prépare avant et entre les séances, sans porter les droits d'accès ni le déroulement d'une session. Détail : `docs/conception/domain/content-library.md`.
- **Session Conduct** gère le déroulement d'une session — cycle de vie, tableau de bord, documents épinglés, notes de session — sans posséder de contenu propre : il consomme ce que la bibliothèque de contenu a préparé. Détail : `docs/conception/domain/session-conduct.md`.

Les contextes bornés ne dépendent jamais directement les uns des autres : leurs échanges passent par des identifiants typés, des événements de domaine ou des contrats applicatifs. Les diagrammes associés à chaque contexte (MCD, MLD, classes, flux) sont disponibles dans `docs/conception/domain/diagrams/` et ne sont pas reproduits ici.

### Tableau de renvoi — §4

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | `DocumentId` unique, `DocumentProperties`, régime de validation par type |
| `docs/architecture/specs/document-properties-schemas.md` | spec pré-build | — | 8 types système seedés ; schémas dérivés (scenario, live_note) et non modélisés (6 autres types) |
| ADR-018 — Généralisation Campaign en Space | conception | Accepté | `SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`, espace personnel mono-membre, hors quota |
| ADR-009 — FK `spaces.ownerId → users` | conception | Accepté | FK cross-module fondatrice ; décompte 19 FK cross-module annoncé (renvoi ADR-011) |
| ADR-010 — Suppression d'espace (soft-delete + purge + saga) | conception | Accepté | Distinction `Archive()` / `Delete()`, corbeille 30 jours, événement `SpaceDeleted` |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Matrice des 38 FK, 17 FK cross-module recensées, deux sagas, deux cycles traités |
| `docs/architecture/specs/mapping-ef-core.md` | spec pré-build | — | Converters d'IDs typés, owned types, `jsonb`, query filters, colonnes promues, FK `RESTRICT` |
| `docs/conception/domain/core.md` | domaine (conception) | — | Contenu du noyau partagé, règles de gouvernance d'admission, exception RGPD `SoftDelete` |
| CdC §8 | synthèse produit | — | Formulation d'origine du modèle de données et domaine |

---

## 5. Sécurité & autorisation

Cette section consolide les planchers de sécurité opposables du MVP — politique de mot de passe, cycle de vie des jetons, rate limiting, liaison des fournisseurs d'identité externes, invariant d'autorisation, principe de non-révélation d'existence, plancher de sanitisation — sans en reproduire le déroulement algorithmique, renvoyé au dossier de conception détaillée. Aucune borne listée ci-après n'est adoucie par rapport à sa source ; là où la source renvoie une valeur exacte à l'implémentation, ce cahier reporte le renvoi tel quel plutôt que d'en inventer une.

### Politique de mot de passe et hachage

La longueur minimale du mot de passe est fixée à **12 caractères** ; les règles de complexité additionnelle (chiffres, symboles, casse) sont **désactivées explicitement** dans le code, pas laissées à une valeur par défaut. L'algorithme de hachage retenu est **Argon2id**, avec un repli acceptable vers **bcrypt (cost ≥ 12)** si Argon2id n'est pas intégrable dans la contrainte de dépendances du MVP. Les paramètres numériques exacts d'Argon2id (mémoire, itérations, parallélisme) restent `[À TRANCHER — B1.5]` — les valeurs illustratives figurant en annexe de la source ne font pas autorité. La vérification de compromission du mot de passe (breach-check HaveIBeenPwned, protocole k-anonymity) est une dette nommée, reportée post-MVP.

Un compte créé et authentifié uniquement via un fournisseur fédéré peut définir un mot de passe complémentaire (mesure préventive anti-verrouillage, pas une capacité de récupération) : cette définition est une opération sensible exigeant `emailVerified = true` et une ré-authentification récente et complète auprès du fournisseur d'identité fédéré, la seule session applicative Haversack étant insuffisante comme preuve d'identité. La fraîcheur maximale tolérée de cette ré-authentification et son mécanisme de déclenchement restent `[À TRANCHER — B1.5]`.

### Cycle de vie des jetons

| Jeton | Durée | Prolongation |
|---|---|---|
| Access token (JWT) | ≤ 15 min | Aucune — renouvellement par refresh uniquement |
| Refresh token | ≤ 7 jours, borne ABSOLUE | Aucune prolongation glissante (refresh glissant = dette post-MVP) |

La signature JWT est **asymétrique** (RS256 ou ES256) ; l'algorithme `alg: none` est interdit explicitement à la validation, un token ainsi signé devant échouer par exception et non produire un résultat valide. La clé de signature n'est jamais committée ni exposée en variable d'environnement en clair — elle est gérée par un secret manager ou un magasin de certificats, avec rotation sans redéploiement applicatif.

Chaque refresh token appartient à une famille ; la rotation intervient à chaque usage, et la détection d'un JTI déjà en denylist déclenche la révocation de la famille entière (probable réutilisation post-vol). La denylist JTI est externe et partagée (jamais mono-instance en implémentation de référence, cohérent avec la topologie scale-out à sticky sessions), purgée périodiquement de ses entrées expirées. Elle doit être alimentée sur les événements domaine `AccountSuspended` et `UserAnonymized` — pour ce dernier, la révocation des tokens actifs est une étape de la saga elle-même, pas une opération asynchrone indépendante. [Le pipeline exact de rotation et le câblage de la denylist relèvent du dossier de conception détaillée.]

### Rate limiting

Le rate limiting est **hybride, par-IP ET par-compte**, à seuils indépendants et cumulatifs — jamais l'une des deux dimensions seule. Il couvre la connexion, le rafraîchissement de jeton, la validation de jeton d'accès invité, et la demande comme la confirmation de réinitialisation de mot de passe (réponse `429` au-delà du seuil). Une borne supérieure de sécurité est obligatoire pour chaque seuil — une valeur arbitrairement élevée neutraliserait la protection. Les valeurs exactes des seuils (par-IP, par-compte, borne supérieure) restent `[À TRANCHER — B1.5]`.

Le token de réinitialisation de mot de passe est à usage unique, invalidant tous les tokens de réinitialisation précédemment émis pour l'utilisateur concerné à chaque nouvelle demande, avec un TTL borné dur ≤ 15 min dont la valeur exacte dans la plage 10-15 min reste `[À TRANCHER — B1.5]`.

### Fournisseurs d'identité externes (OAuth)

Les fournisseurs retenus pour le MVP sont **Google et Discord**, chacun satisfaisant deux conditions cumulatives : un claim d'email vérifié jugé fiable, et une adresse email canonique (non un alias de relais). La fiabilité de ce claim est une dépendance externe non vérifiable par la validation continue Haversack, quel que soit le fournisseur ; le gate applicatif qui l'encadre reste, lui, testable par un fournisseur simulé.

La liaison d'un compte OAuth à un compte préexistant est **interdite si l'email de ce compte n'est pas prouvé vérifié**, avec rejet silencieux (aucun message ne révèle l'existence du compte). Le cas d'un compte préexistant non vérifié portant la même adresse email qu'une preuve OAuth fraîche est résolu par une reprise de la coquille (bascule de son statut de vérification, neutralisation obligatoire du mot de passe préexistant, liaison à l'identité fédérée) plutôt que par la création d'un doublon — un doublon sur la même adresse n'étant pas implémentable au regard de l'unicité de l'email dans le système. Cette résolution reste `[À TRANCHER — à ratifier opérateur]` ; le sort du contenu éventuel déjà rattaché à la coquille reprise est une facette distincte, renvoyée au dossier juridique. L'ajout futur d'un fournisseur à email de relais non canonique (ex. masquage d'adresse) est une dette nommée : il imposerait de revisiter la règle de liaison par email, non déclenchée par le périmètre MVP (Google et Discord, tous deux à email canonique).

### Invariant d'autorisation

Toute lecture d'une ressource d'un espace compose, **en ET**, une vérification d'**appartenance** (l'appelant est membre actif de l'espace, ou dispose d'un accès invité actif dont le périmètre couvre la ressource) et une vérification de **visibilité** portée par le domaine (document public, réservé au MJ, ou strictement privé à son auteur — cette dernière catégorie exclut le MJ sans exception de type de document). Ces deux vérifications sont composées en un **mécanisme unique centralisé**, appliqué identiquement aux appels d'API et à la diffusion en temps réel — jamais deux implémentations distinctes susceptibles de diverger. Deux autres vérifications, structurelles et indépendantes de l'appelant (espace ni supprimé ni en cours de purge ; document non marqué supprimé), s'y ajoutent sans en faire partie. [Le pipeline d'évaluation et son câblage exact relèvent du dossier de conception détaillée.]

### Non-révélation d'existence

Un principe transverse, énoncé une seule fois ici : l'accès à un document non partagé, une tentative de reconnexion après suppression de compte, et une liaison OAuth échouée ne révèlent jamais l'existence ou l'état de la ressource ou du compte visés. Un résidu assumé à ce principe est conservé par choix produit : le message d'inscription confirmant qu'une adresse email est déjà associée à un compte, mitigé par le rate limiting hybride ci-dessus — décision opérateur tracée, pas un oubli.

### Plancher de sanitisation et CSP

La sanitisation du contenu est une **liste blanche positive des deux côtés** (serveur et client) : les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont interdits et supprimés **sans exception**, quels que soient la bibliothèque ou la liste exacte de balises retenus — ce plancher est un critère d'acceptation non négociable. La politique CSP retenue restreint par défaut à l'origine de l'application (`default-src 'self'`), interdit tout script inline (`script-src 'self'`), et restreint les connexions sortantes à l'origine ou à l'API (`connect-src 'self'`) — cette dernière directive rend observable l'absence d'envoi réseau en mode local. Les listes blanches exhaustives de balises et les directives CSP complètes restent `[À TRANCHER — B1.2 côté serveur / P6 côté client]`. À l'import d'un fichier local comme au parcours de migration, l'ordre est impératif : validation structurelle **puis** sanitisation, l'une et l'autre **avant** toute écriture (détail du parcours de migration en §8).

### Risques résiduels assumés

Le stockage local IndexedDB n'est pas chiffré at-rest — aucun secret utilisateur n'existe en mode sans-compte à partir duquel dériver une clé ; cette limitation est assumée et communiquée par un bandeau de confidentialité distinct du bandeau de durabilité (cohérent NFR-CONF-04). L'énumération de comptes évoquée ci-dessus est tracée comme risque accepté. Aucun blob média externalisé n'est prévu au MVP.

### Tableau de renvoi — §5

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Politique mot de passe, Argon2id/bcrypt, cycle de vie des jetons, rotation/denylist, rate limiting, OAuth Google/Discord, reclaim-in-place, résidu CWE-204 |
| ADR-014 — Modèle d'autorisation API | conception | Accepté | Invariant d'autorisation composé (appartenance + visibilité), mécanisme unique REST/temps réel, non-révélation d'existence |
| ADR-007 — RGPD et modèle d'autorisation API | conception | Accepté | Invariant d'autorisation fondateur, formalisé par ADR-014 ; reclassement des findings bloquants MVP formalisés par ADR-015 |
| `docs/architecture/specs/config-securite-migration.md` | spec pré-build | cadre à compléter | Registre des seuils (longueur mdp, Argon2id, durées de jetons, rate limiting, TTL reset) — bornes fixées vs valeurs renvoyées à B1.5 |
| `docs/architecture/specs/contrat-openapi.md` | spec pré-build | cadre à compléter | Principe de non-révélation d'existence, cas 403 tranchés, 403 vs 404 renvoyé B1.10 |
| `docs/architecture/specs/sanitisation-csp.md` | spec pré-build | cadre à compléter | Plancher de sanitisation liste blanche, posture CSP, ordre validation puis sanitisation |
| `docs/conception/besoin/nfr/NFR-CONF-01` à `04` | NFR produit | — | Séparation notes privées/vue joueurs, isolation mode local, suppression effective, transparence des risques — renvoi croisé §10 |
| `docs/conception/domain/core.md` | domaine (conception) | — | Énumération `Visibility`, primitives de traçabilité mobilisées par l'invariant d'autorisation |
| CdC §7.3 | synthèse produit | — | Formulation d'origine de la sécurité & authentification |

---

## 6. Conformité RGPD technique

Cette section consolide le régime technique de conformité RGPD acté pour le MVP — effacement de compte, données des joueurs invités, cascade de suppression, instrumentation respectueuse — et signale, sans les requalifier, les axes que le corpus renvoie explicitement à une validation juridique préalable au lancement.

### Effacement de compte (droit à l'oubli, Art. 17)

L'effacement d'un compte procède par **anonymisation par réécriture** : l'email et le nom d'affichage sont remplacés par des valeurs neutres, l'identifiant technique de l'utilisateur est conservé pour la continuité des espaces partagés auxquels il appartenait. Le sort du contenu créé par l'utilisateur se répartit en trois catégories : les documents strictement privés à l'auteur (et les notes liées à ses personnages) sont **supprimés physiquement**, sans exception ; les documents non partagés au sens d'un critère opérationnel à trois conditions cumulatives (absence d'épinglage, absence de référencement depuis un document d'un autre utilisateur, absence d'instanciation par un autre document) sont également **supprimés physiquement**, sauf s'ils sont réservés au MJ, auquel cas ils rejoignent la catégorie suivante ; les documents partagés ou réservés au MJ sont **conservés** sous l'identifiant anonymisé, au titre de l'intérêt légitime de continuité pour les membres tiers d'un espace vivant. Le contenu de l'espace personnel de l'utilisateur, lui, est **supprimé intégralement et sans condition**, aucun tiers n'y ayant d'intérêt légitime de continuité. [Le séquençage exact de déliaison puis suppression, et l'algorithme de la saga, relèvent du dossier de conception détaillée.]

Le critère de partage ci-dessus, comme l'appartenance d'espace des documents, est évalué à l'instant de la **demande d'effacement**, non à l'instant de l'exécution effective du traitement — un jeu de sélection est matérialisé à cet instant et consommé sans recalcul par les traitements de suppression, ce qui referme un risque de décalage entre l'état constaté à la demande et l'état au moment de l'exécution. [Le mécanisme technique de matérialisation et de relocation relève du dossier de conception détaillée.]

Toute demande d'effacement est traitée dans un délai légal d'**1 mois**, extensible à **2 mois** sous réserve d'en informer la personne dans le premier mois.

### Données des joueurs invités

Le nom d'affichage saisi par un joueur invité est traité sur la base de l'**intérêt légitime** (Art. 6), avec une information affichée **au moment même de la saisie**, avant l'entrée en session — l'invité ne s'inscrivant pas, cette information ne peut pas être reportée à une page de compte. Les accès invités expirés sont purgés **90 jours après leur expiration**, indépendamment du cycle de vie de l'espace qui les porte (un espace partagé actif de longue durée ne justifie pas la conservation indéfinie d'accès expirés). Les métadonnées techniques de connexion associées (IP, horodatages) sont conservées au maximum **30 jours** après la fin de l'accès. La posture retenue sur les mineurs est celle d'un service non destiné aux enfants, avec attestation déclarative d'un âge minimal de 16 ans, sans mécanisme de vérification d'âge ni de consentement parental.

### Cascade et intégrité à la suppression

La suppression d'un espace procède par corbeille puis purge physique différée de 30 jours. La suppression de compte déclenche la purge inconditionnelle et immédiate du contenu de l'espace personnel, dans la même fenêtre de traitement légale que l'anonymisation du compte. [Le détail des deux traitements de suppression — ordre topologique, garanties transactionnelles — relève du dossier de conception détaillée et n'est pas reproduit ici ; voir §3, §4.]

### Instrumentation respectueuse

L'instrumentation de mesure d'usage du MVP est conçue pour mesurer l'occurrence d'un usage sans capter le contenu narratif créé ou partagé par le MJ — cohérent avec l'isolation des données en mode local (renvoi §9).

### Export et possession des données

L'export d'un espace réutilise le même format de sérialisation versionné que la migration locale vers le cloud, garantissant que les données exportées restent lisibles et réutilisables hors de l'application (renvoi §8).

### Axes soumis à validation juridique préalable au lancement (non requalifiés ici)

Cinq axes, consolidés dans un cadrage dédié destiné à une revue juridique, restent des **questions ouvertes de qualification légale** — aucun n'est vérifiable par une validation continue applicative, chacun exige un avis externe :

1. Qualification d'un service « destiné aux enfants » (Art. 8) et suffisance de l'attestation déclarative 16+, pour le compte utilisateur comme pour l'accès invité.
2. Qualification sous-traitant / responsable de traitement (Art. 28) pour les contenus créés par le MJ décrivant des tiers identifiables, et périmètre exact du contrat de sous-traitance envisagé — y compris son extension au contenu de l'espace personnel.
3. Mise en balance formelle de l'intérêt légitime, non conduite par un juriste, pour la conservation des documents partagés d'un compte supprimé et pour la base légale du nom d'affichage invité.
4. Suffisance, au regard de l'Art. 17, de la suppression inconditionnelle du contenu de l'espace personnel, y compris les deux points subsidiaires sur l'instant de référence en cas de déplacement de document entre espace personnel et espace partagé.
5. Sort du contenu éventuellement déjà rattaché à un compte non vérifié repris par une preuve d'identité fédérée fraîche (reclaim-in-place, §5).

Chacun de ces cinq axes reste `[À TRANCHER — FLAG JURISTE]` ; ce cahier ne les requalifie pas en décisions et ne préjuge d'aucune issue.

### Tableau de renvoi — §6

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-012 — RGPD : effacement de compte | conception | Accepté | Procédure d'anonymisation, catégorisation du contenu (physique/non partagé/conservé/personnel), délai Art. 12§3, mécanisme de figement à la demande |
| ADR-013 — RGPD : données des joueurs invités | conception | Accepté | Base légale du nom d'affichage, information Art. 13, rétention 90/30 jours, posture mineurs |
| ADR-007 — RGPD et modèle d'autorisation API | conception | Accepté | Principe fondateur de l'anonymisation par réécriture ; formalisé par ADR-012 et ADR-013 |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Sagas `SpaceDeleted` / `UserAnonymized` consommées par la politique RGPD ci-dessus |
| ADR-010 — Suppression d'espace | conception | Accepté | Corbeille + purge J+30 pour les espaces supprimables par le MJ, raffiné par ADR-011 pour le mécanisme complet |
| `docs/architecture/specs/requete-effacement-non-partage.md` | spec pré-build, illustrative non normative | cadre à compléter | Critère opérationnel « document non partagé », instant de référence, trou nommé sur les références nullable entrantes |
| `docs/securite/conformite/cadrage-validation-pre-lancement-eu.md` | dossier juridique | à valider juriste | Les 5 axes de validation légale préalable au lancement EU, consolidés depuis ADR-012/013/018/015 |
| CdC §7.4 | synthèse produit | — | Formulation d'origine de la conformité RGPD |

---

## 7. Contrats d'API

Cette section consolide les contrats observables du canal d'accès aux ressources — décision d'accès, sémantique des codes HTTP, interfaces d'authentification, contrat de propriétés de document — sans reproduire les schémas de requête/réponse endpoint par endpoint, qui restent l'objet d'un contrat non encore stabilisé par le corpus.

### Décision d'accès en un point

La décision d'autorisation d'accès à une ressource est rendue par un service unique, retournant une valeur discriminée autorisé/refusé, consommé identiquement par le canal REST et par la diffusion en temps réel (renvoi §5). [Le pipeline d'évaluation exact relève du dossier de conception détaillée.]

### Sémantique des codes HTTP

Les scénarios suivants constituent des cas déjà tranchés par le corpus, tous en **403** — un refus légitime connu opposé à un appelant déjà reconnu dans l'espace ou la session : un invité hors du périmètre de sa session, un joueur ou un invité face à un document réservé au MJ (y compris épinglé en session), un invité face au document strictement privé d'un autre membre, un invité réassocié au même personnage face à une note privée d'un compte supprimé. Le choix entre **403 et 404** pour un appelant non-membre accédant à une ressource d'un autre espace reste ouvert par le corpus lui-même — une lecture dérivée du principe de non-révélation (404, cohérent avec l'absence d'appartenance de l'appelant) est proposée sans être tranchée : `[À TRANCHER — B1.10]`.

Un espace en corbeille ou en purge, ou un document marqué supprimé, ne constituent pas un cas d'erreur distinct au niveau du contrat : ils sont rendus invisibles en amont de toute résolution applicative, sans code ou corps de réponse dédié. Une requête de liens entrants (« quels documents pointent vers celui-ci ? ») exclut silencieusement, sans code d'erreur ni indicateur de filtrage partiel, tout document source non lisible par l'appelant. Les cinq points d'entrée d'authentification (connexion, rafraîchissement de jeton, validation de jeton d'accès invité, demande et confirmation de réinitialisation de mot de passe) renvoient **429** au-delà du seuil de rate limiting (renvoi §5). Une liaison OAuth rejetée pour cause de compte préexistant non vérifié est rejetée silencieusement, sans code distinctif permettant à l'appelant d'en déduire la cause.

Restent `[À TRANCHER — B1.10]`, sans proposition dérivée dans le corpus : les schémas de requête et de réponse exacts par point d'entrée, l'attribution fine des codes 400 et 401, la liste littérale et exhaustive des chemins REST (le corpus désigne des ressources et des scénarios, jamais des chemins arrêtés), le schéma exact du corps de réponse 429, le code HTTP exact du rejet silencieux de liaison OAuth, et le format de l'annotation de sécurité portée par le contrat OpenAPI.

### Interfaces applicatives observables d'authentification

Les garanties de sécurité liées aux jetons sont portées par des contrats applicatifs observables plutôt que par une configuration d'infrastructure opaque : validation de jeton, gestion de la denylist, signature, et exigence de vérification email avant opération sensible sont chacune un point de passage nommé. [Les signatures normatives complètes relèvent de l'annexe pré-implémentation de la source et du dossier de conception détaillée.]

### Contrat de propriétés de document

Toute écriture des propriétés structurées d'un document — dont sa visibilité — passe exclusivement par une méthode de domaine validée contre le schéma déclaré du type de document concerné ; aucune écriture directe dans le champ sous-jacent n'est permise. Deux régimes de validation coexistent : un schéma figé pour les types système non modifiés, une validation permissive par défaut pour les types personnalisés ou modifiés — choix délibéré pour ne pas bloquer l'extensibilité. Parmi les huit types système du MVP, seul le schéma du type scénario (un statut éditorial à quatre valeurs) et l'absence de schéma structuré du type note live (ses champs distinctifs étant promus en colonnes de premier niveau) sont dérivés du modèle de domaine ; les schémas des six autres types système restent `[À TRANCHER — modélisation domaine]`. Le comportement de cette méthode d'écriture en l'absence de type déclaré sur un document reste également `[À TRANCHER — modélisation domaine]`.

### Contrat de payload versionné

Le contrat d'échange local↔cloud (enveloppe `schemaVersion` / `exportedAt` / `appVersion` / `spaces[]`) est détaillé en §8 ; il n'est mentionné ici que comme le pendant, côté migration, de la validation des propriétés ci-dessus — les deux mécanismes empruntent le même chemin de revalidation par le domaine.

### Tableau de renvoi — §7

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-014 — Modèle d'autorisation API | conception | Accepté | Service de décision d'accès unique, cas IDOR de référence tranchés en 403, renvoi B1.10 pour 403/404 non-membre |
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Interfaces observables d'authentification, périmètre du rate limiting 429 |
| ADR-016 — Sérialisation locale et contrat de migration | mixte, dominante pré-implémentation | Accepté | Contrat de payload versionné (détail §8) |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | Méthode de domaine comme point de passage obligatoire, deux régimes de validation |
| `docs/architecture/specs/contrat-openapi.md` | spec pré-build | cadre à compléter | Sémantique 403 déjà tranchée, exclusion des backlinks, 429 sur endpoints d'authentification, points renvoyés B1.10 |
| `docs/architecture/specs/document-properties-schemas.md` | spec pré-build | cadre à compléter | Contrat `DocumentProperties`, deux régimes de validation, schémas dérivés vs à trancher des huit types système |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Mécanisme de décision d'accès partagé entre REST et diffusion temps réel |

---

## 8. Migration & sérialisation

Cette section consolide le contrat de données et le contrat d'échec de la frontière local↔cloud — format de sérialisation, frontière de confiance, parcours d'échec transactionnel, modèle de stockage local — sans reproduire l'algorithme d'import topologique, renvoyé au dossier de conception détaillée.

### Format et périmètre sérialisé

Le passage de données entre le mode local et le cloud repose sur **un seul format JSON versionné**, utilisé aussi bien pour l'export local que pour le payload de migration. L'enveloppe porte un `schemaVersion` entier obligatoire — un payload sans cette mention, ou dont la version est inconnue ou antérieure au seuil de compatibilité maintenu, est rejeté proprement. Le périmètre sérialisé couvre l'agrégat document et ses dépendances directes ainsi que l'historique des sessions terminées ; sont explicitement exclus : les sessions en cours, les préférences d'affichage de session (recréées à l'import), les comptes et appartenances (absents du mode local), et tout indicateur de cycle de vie gouverné par le serveur. L'espace personnel est inclus dans le périmètre sérialisé au même titre que les espaces partagés, et rattaché au compte créé au moment de l'import.

Deux catégories de champs se distinguent à l'import : les **champs gouvernés** (propriétés structurées, visibilité, identifiants de slug, statut de session) sont revalidés par le domaine ; le **champ libre** (contenu des blocs de document) est transporté sans revalidation sémantique mais reste soumis à sanitisation (renvoi §5).

### Frontière de confiance

Le serveur n'honore **aucun champ d'autorité** contenu dans le payload : les identifiants sont régénérés à l'import, la propriété et l'auteur sont assignés au compte authentifié qui déclenche la migration, les horodatages sont réémis côté serveur, et les indicateurs de cycle de vie serveur ne sont jamais transportés. La migration n'est pas conçue comme une preuve d'appartenance des données — le mode local n'ayant pas de compte, il n'existe rien à prouver — mais comme une **assignation de propriété** au moment de l'import.

### Ordre impératif à l'import

L'ordre est impératif et non négociable : **validation structurelle d'abord**, **sanitisation du contenu ensuite**, l'une et l'autre **avant** toute écriture. Un fichier importé n'est jamais écrit tel quel. [Le détail algorithmique de la résolution des références internes du payload et de l'ordre topologique de création relève du dossier de conception détaillée.]

### Parcours d'échec transactionnel

La granularité de la transaction d'import est **l'espace** : ni le payload entier, ni le document individuel. Le serveur effectue un contrôle de pré-validation sur chaque espace du payload, produit un rapport de rejets codifié par raison, importe séparément chaque espace valide, et laisse intactes les données locales correspondant aux espaces rejetés jusqu'à confirmation explicite. L'opération de migration est associée à un identifiant de lot dont le format est validé et qui garantit l'**idempotence**, scopée par utilisateur authentifié : un espace déjà importé dans le cadre d'un même lot n'est pas réimporté.

### Gate de confirmation anti-appropriation

Avant tout import, l'utilisateur se voit présenter les espaces détectés dans son navigateur (titres, volume estimé, historique de session, espace personnel inclus) et doit fournir une **confirmation explicite**. Ce gate est un **contrôle applicatif côté serveur** — la requête d'import porte un indicateur de consentement explicite, et le serveur rejette l'import si cet indicateur est absent, indépendamment de ce que le client a présenté. Il borne le risque d'appropriation accidentelle sur un poste partagé ; il ne constitue pas une preuve cryptographique d'appartenance.

### Modèle local IndexedDB

Le store local n'est pas un miroir relationnel des tables serveur : il est **enraciné sur l'agrégat de l'espace** (aggregate-rooted), organisation cohérente avec la posture CRUD du mode local et qui rend la fonction de projection vers le payload quasi-identitaire. Les index locaux se limitent strictement aux chemins de lecture qu'exige le mode local — navigation par espace, par dossier, par document, recherche par titre — sans index spéculatif. La version interne du store IndexedDB est **distincte du `schemaVersion` du payload** : les deux contrats évoluent indépendamment, la fonction de projection absorbant les évolutions internes du store sans affecter le contrat de payload côté serveur. Le mode local est plafonné à trois espaces actifs, vérifié avant toute écriture.

### Points ouverts de configuration

Le seuil de `schemaVersion` minimale maintenue côté serveur et l'horizon de rétention du registre des lots de migration restent `[À TRANCHER — P7]`.

### Tableau de renvoi — §8

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-016 — Sérialisation locale et contrat de migration | mixte, dominante pré-implémentation | Accepté | Format versionné, frontière de confiance, parcours d'échec transactionnel par espace, gate de confirmation |
| ADR-017 — Modèle IndexedDB local | mixte, dominante pré-implémentation | Accepté | Structure aggregate-rooted, index restreints, versionnement du store distinct du payload, plafond de 3 espaces |
| ADR-001 — Exécution du domaine en mode local | pré-implémentation | Accepté | Fondation — invariant validation locale ⊆ serveur, migration comme import revalidé |
| ADR-018 — Généralisation de `Campaign` en `Space` | conception | Accepté | Inclusion de l'espace `PERSONAL` dans le périmètre sérialisé et la transactionnalité par espace |
| `docs/architecture/specs/sanitisation-csp.md` §2 | spec pré-build | cadre à compléter | Ordre validation puis sanitisation à l'import JSON local |
| `docs/architecture/specs/config-securite-migration.md` | spec pré-build | cadre à compléter | Seuil `schemaVersion` et rétention du lot de migration renvoyés P7 |
| `docs/architecture/structure-projets.md` §7 | synthèse structurelle | — | Structure de solution mobilisée par le handler d'import |
| CdC §7.2, §7.4 | synthèse produit | — | Formulation d'origine de la migration et de l'export |

---

## 9. Observabilité & télémétrie

Cette section consolide l'instrumentation compensatoire de validation du MVP et le suivi de coût du canal temps réel — deux mécanismes dont le corpus fixe l'intention et le principe sans en arrêter les valeurs.

### Instrumentation d'activation par pilier

Le MVP livrant en un seul bloc (préparation, vue de session, cloud, partage) plutôt qu'en temps de livraison séparés, une instrumentation compensatoire mesure l'occurrence d'usage sur trois piliers, chacun composite (une seule création ne suffit pas à qualifier l'activation) : activation préparation (espace créé + N documents créés), activation vue de session (session ouverte + usage réel constaté), activation partage (document partagé + au moins un joueur l'ayant ouvert). Cette instrumentation mesure une **occurrence**, jamais le **contenu narratif** produit ou partagé par le MJ — cohérent avec l'isolation des données en mode local (renvoi §5/§6).

Une capture d'email **non bloquante**, accompagnée d'un dispositif d'analytics qualifié d'« anonyme RGPD », s'applique **dès le mode local** — avant même la création d'un compte cloud.

### Suivi de coût et repli du canal temps réel

Un seuil de coût par session concurrente constitue le critère de réversibilité déclenchant le repli du canal temps réel vers un polling adaptatif ; ce seuil est un **livrable de configuration exploitable en production**, pas une mention documentaire sans effet opérationnel. En amont de tout repli, la configuration retenue est déjà sobre : transport forcé en flux à sens unique tant que la communication reste MJ→joueurs, fermeture de connexion en fin de session active, fréquence de battement de vie allongée (cohérent avec la rareté des événements en session de jeu de rôle).

### Points ouverts

Les points suivants restent `[À TRANCHER — telemetrie.md §6 / repli-temps-reel.md §5]`, non comblés par ce cahier : la valeur du seuil N du pilier préparation, l'opérationnalisation du critère « usage réel constaté » du pilier vue de session, l'outil analytics retenu, le schéma d'événements formalisé (noms techniques, propriétés), la technique d'anonymisation RGPD appliquée à ces événements, le mécanisme de capture email non bloquante, la définition même de la métrique de coût par session concurrente, la valeur du seuil de repli, la cadence cible du polling adaptatif, et le mécanisme de détection du dépassement de seuil.

Deux maillons ne sont, par nature, pas vérifiables par une validation continue applicative : le comportement de coût et de repli réel sous charge (dépend de conditions d'hébergement en production) et le comportement de persistance/éviction du stockage navigateur (renvoi §8) — ces maillons sont nommés pour éviter une fausse confiance dans la couverture des vérifications automatisées, pas pour signaler un manque à corriger.

### Tableau de renvoi — §9

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| `docs/architecture/specs/telemetrie.md` | spec pré-build | cadre à compléter | Trois piliers d'activation composites, principe occurrence sans contenu narratif, six points ouverts |
| `docs/architecture/specs/repli-temps-reel.md` §4-5 | spec pré-build | cadre à compléter | Seuil de coût comme livrable de configuration, configuration sobre actée, quatre points ouverts |
| ADR-006 — Périmètre MVP | décision produit, fusionnée en vision | Accepté | Origine de l'instrumentation compensatoire par pilier et de la capture email/analytics anonyme |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Origine du seuil de coût par session concurrente et de la configuration sobre |
| `docs/conception/besoin/nfr/NFR-CONF-02` | NFR produit | — | Principe d'isolation des données en mode local, contrainte sur les mesures d'usage |
| CdC §7.4 | synthèse produit | — | Formulation d'origine de l'instrumentation respectueuse |

---

## 10. Exigences non-fonctionnelles (dérivées)

Les vingt fiches NFR du corpus de conception (`docs/conception/besoin/nfr/`) portent les identifiants stables et sont la **source unique** des critères d'expérience. Cette section ne les recopie pas : elle référence chaque identifiant et ne consolide que ce qu'une source technique fixe effectivement au-delà du critère produit — le reste reste renvoyé, jamais reformulé.

### Performance perçue

| ID NFR | Exigence technique dérivée | Seuil technique |
|---|---|---|
| NFR-PERF-01 | Fluidité de la navigation en session | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-02 | Réactivité à la première interaction | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-03 | Continuité sans interruption lors des actions en session | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-04 | Résultats de recherche immédiats — recherche titre-only au MVP (renvoi §4, UC-14) | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |

Les quatre seuils chiffrés sont ouverts par conception — le corpus ne fixe aucune métrique de latence ou de délai.

### Hors connexion

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-OFF-01 | Périmètre offline intégral en mode local (préparation, vue de session, recherche locale) |
| NFR-OFF-02 | Durabilité best-effort des données locales entre sessions — retrouvées intactes après fermeture du navigateur ou redémarrage de l'appareil ; `navigator.storage.persist()` comme mécanisme de durabilité. Volet alerte délégué à NFR-OFF-03 |
| NFR-OFF-03 | Aucune perte silencieuse — avertissement non bloquant préalable et action de sécurisation proposée dès qu'une perte devient possible (garantie de conservation non obtenue, stockage saturé) ; bandeau distinct du bandeau de confidentialité (NFR-CONF-04), à ne pas fusionner |
| NFR-OFF-04 | Fonctionnement partiel en cas de perte réseau passagère en mode cloud, en session (renvoi ADR-017 §2(c) pour le régime des trois postures local/migration/cloud) |
| NFR-OFF-05 | Continuité d'édition en préparation cloud hors session lors d'une perte de réseau — conservation locale et synchronisation automatique au retour de connexion, sans action du MJ ; distinct de NFR-OFF-04 (session active) |

### Confidentialité

Le corps technique de CONF-01 à CONF-04 est porté par les §5 et §6 ci-dessus ; cette entrée est un renvoi croisé, pas un dédoublement.

### Accessibilité

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-ACC-01 | Parcours intégralement exécutables au clavier, focus visuellement identifiable en permanence |
| NFR-ACC-02 | Contenu, actions et changements d'état exploitables et annoncés par les lecteurs d'écran usuels |
| NFR-ACC-03 | Contraste suffisant en conditions de faible éclairage |
| NFR-ACC-04 | Tailles de texte adaptées à la lecture rapide en session |

Le référentiel WCAG 2.1 niveau AA fournit un cadre de mesure pour ces quatre exigences ; son statut de critère opposable et son calibrage exact restent `[À TRANCHER]`.

### Internationalisation

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-I18N-01 | Interface entièrement en français au MVP |
| NFR-I18N-02 | Externalisation des chaînes et disposition tolérante à la longueur des libellés — évolutivité, pas un engagement de calendrier |
| NFR-I18N-03 | Traitement Unicode de bout en bout pour le contenu créé par le MJ, indépendant de la langue de l'interface |

### Bornes MVP à répercuter comme décisions de périmètre

Les bornes suivantes sont des **décisions de périmètre assumées**, pas des travaux différés : synchronisation multi-appareils exclue du MVP, recherche titre-only mono-espace, partage par document (pas par joueur), multilinguisme non planifié pour cette version, disposition droite-à-gauche et texte vertical hors MVP.

### Tableau de renvoi — §10

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| `docs/conception/besoin/nfr/README.md` | NFR produit, index | — | Cinq familles NFR, périmètre MVP assumé par famille |
| `docs/conception/besoin/nfr/NFR-PERF-01` à `04` | NFR produit | — | Performance perçue, seuils chiffrés non fixés |
| `docs/conception/besoin/nfr/NFR-OFF-01` à `05` | NFR produit | — | Fonctionnement hors connexion, deux bandeaux distincts, continuité d'édition en préparation cloud (NFR-OFF-05) |
| `docs/conception/besoin/nfr/NFR-CONF-01` à `04` | NFR produit | — | Confidentialité — corps technique en §5/§6 |
| `docs/conception/besoin/nfr/NFR-ACC-01` à `04` | NFR produit | — | Accessibilité, référentiel WCAG 2.1 AA |
| `docs/conception/besoin/nfr/NFR-I18N-01` à `03` | NFR produit | — | Internationalisation, périmètre français au MVP |
| CdC §6 | synthèse produit | — | Reformulation des vingt exigences non fonctionnelles en expérience vécue |

---

## 11. Points ouverts & confirmations attendues à l'entrée en build

Le corpus technique consolidé dans ce cahier est de nature pré-implémentation : les points listés ci-dessous sont à confirmer ou à trancher à l'entrée en construction, section par section. Aucun n'est comblé par ce cahier — chacun est reporté tel qu'il apparaît dans sa source.

### §3 — Contraintes techniques

- Timeout d'expiration du claim `purge_claimed_at` de la saga de suppression — `[À TRANCHER — config-securite-migration.md / ADR-011 B1]`.

### §4 — Modèle de données & domaine

- Contradiction non arbitrée sur le décompte de FK cross-module : 19 FK (ADR-009, en renvoi vers ADR-011) contre 17 FK recensées dans la matrice propre d'ADR-011.
- Schémas de `propertiesSchema` des six types de document système restant à modéliser (scène, PNJ, lieu, note, personnage joueur, révélation) — `[À TRANCHER — modélisation domaine, document-properties-schemas.md §5]`.

### §5 — Sécurité & autorisation

- Paramètres exacts d'Argon2id (mémoire, itérations, parallélisme) — `[À TRANCHER — B1.5]`.
- Valeurs exactes des seuils de rate limiting (par-IP, par-compte, borne supérieure) — `[À TRANCHER — B1.5]`.
- Valeur exacte du TTL du token de réinitialisation dans la plage 10-15 min — `[À TRANCHER — B1.5]`.
- Fraîcheur maximale et déclenchement de la ré-authentification IdP pour la définition d'un premier mot de passe sur compte fédéré — `[À TRANCHER — B1.5]`.
- Résolution reclaim-in-place du cas email OAuth = compte préexistant non vérifié — `[À TRANCHER — à ratifier opérateur]`.
- Ajout futur d'un fournisseur OAuth à email de relais non canonique — dette nommée, non déclenchée par le périmètre MVP.
- Listes blanches exhaustives de balises de sanitisation (serveur et client) et directives CSP complètes — `[À TRANCHER — B1.2 serveur / P6 client]`.

### §6 — Conformité RGPD technique

- Critère opérationnel exact de « document non partagé » : la requête précise reste à écrire à l'implémentation.
- Seuil d'invocation de l'extension du délai de traitement à 2 mois — à documenter dans la procédure de support.
- Qualification juridique de l'instant de référence figé en cas de déplacement d'un document entre espace personnel et espace partagé, dans les deux sens — non arbitrée par le mécanisme technique de figement.
- Trou nommé sur les références nullable entrantes non couvertes par le critère de sélection (colonnes portées par d'autres documents ou par des accès invités pointant vers un document sélectionné pour suppression).
- Les 5 axes du dossier juridique — `[À TRANCHER — FLAG JURISTE]` : qualification Art. 8 (mineurs), qualification sous-traitant Art. 28 et périmètre du contrat associé (y compris l'espace personnel), mise en balance de l'intérêt légitime (documents conservés + nom d'affichage invité), suffisance Art. 17 du hard-delete inconditionnel de l'espace personnel, sort du contenu d'une coquille reprise par reclaim-in-place.
- Activation de l'espace personnel comme zone d'atterrissage par défaut et redéfinition de l'hypothèse H1 associée — gate opérateur explicitement requis par ADR-018, non tranché dans ce cahier.

### §7 — Contrats d'API

- Choix entre 403 et 404 pour un appelant non-membre accédant à une ressource d'un autre espace — `[À TRANCHER — B1.10]`.
- Schémas de requête et de réponse exacts par point d'entrée — `[À TRANCHER — B1.10]`.
- Attribution fine des codes 400 et 401 par point d'entrée — `[À TRANCHER — B1.10]`.
- Liste littérale et exhaustive des chemins REST — `[À TRANCHER — B1.10]`.
- Schéma exact du corps de réponse 429 — `[À TRANCHER — B1.10]`.
- Code HTTP exact du rejet silencieux de liaison OAuth — `[À TRANCHER — B1.10]`.
- Format de l'annotation de sécurité portée par le contrat OpenAPI — `[À TRANCHER — B1.10]`.
- Comportement de la méthode d'écriture des propriétés en l'absence de type de document déclaré — `[À TRANCHER — modélisation domaine]`.

### §8 — Migration & sérialisation

- Seuil de `schemaVersion` minimale maintenue côté serveur — `[À TRANCHER — P7]`.
- Horizon de rétention du registre des identifiants de lot de migration — `[À TRANCHER — P7]`.
- Migration ascendante du payload (compatibilité des versions de `schemaVersion` antérieures) — dette nommée, post-MVP.

### §9 — Observabilité & télémétrie

- Valeur du seuil N du pilier préparation — `[À TRANCHER — telemetrie.md §6]`.
- Opérationnalisation du critère « usage réel constaté » du pilier vue de session — `[À TRANCHER — telemetrie.md §6]`.
- Outil analytics retenu, schéma d'événements formalisé, technique d'anonymisation RGPD, mécanisme de capture email non bloquante — `[À TRANCHER — telemetrie.md §6]`.
- Définition de la métrique de coût, valeur du seuil de repli, cadence cible du polling adaptatif, mécanisme de détection du dépassement — `[À TRANCHER — repli-temps-reel.md §5]`.

### §10 — Exigences non-fonctionnelles

- Les quatre seuils chiffrés de performance perçue (PERF-01 à 04) — `[À TRANCHER — seuil de performance non chiffré dans le corpus]`.
- Statut du référentiel WCAG 2.1 AA comme critère opposable et son calibrage exact — `[À TRANCHER]`.

---

## 12. Traçabilité source → section

Cette matrice prouve que chaque spec pré-build, chaque section technique du cahier des charges, et chaque décision d'architecture du registre trouve sa ou ses sections d'accueil dans ce cahier. Les ADR précurseurs raffinés par une décision ultérieure sont signalés comme tels — la relation de subsomption est portée, pas aplatie.

### Specs pré-build

| Source | Type | Section(s) du cahier | Nature/Statut |
|---|---|---|---|
| `docs/architecture/specs/config-securite-migration.md` | spec | §3, §4, §5, §8 | cadre à compléter |
| `docs/architecture/specs/contrat-openapi.md` | spec | §5, §7 | cadre à compléter |
| `docs/architecture/specs/document-properties-schemas.md` | spec | §4, §7 | cadre à compléter |
| `docs/architecture/specs/mapping-ef-core.md` | spec | §4 | cadre à compléter — empreinte technique de configuration EF Core, portée par renvoi seulement |
| `docs/architecture/specs/repli-temps-reel.md` | spec | §3, §9 | cadre à compléter |
| `docs/architecture/specs/requete-effacement-non-partage.md` | spec, illustrative non normative | §6 | cadre à compléter |
| `docs/architecture/specs/sanitisation-csp.md` | spec | §5, §8 | cadre à compléter |
| `docs/architecture/specs/telemetrie.md` | spec | §9 | cadre à compléter |

### Cahier des charges

| Source | Type | Section(s) du cahier | Nature/Statut |
|---|---|---|---|
| CdC §6 — Exigences non-fonctionnelles | §CdC | §10 | synthèse produit |
| CdC §7 — Contraintes techniques, RGPD & sécurité | §CdC | §1, §2, §3, §5, §6 | synthèse produit, section provisoire |
| CdC §8 — Modèle de données & domaine | §CdC | §4 | synthèse produit |

### Décisions d'architecture (18 ADR)

| ADR | Section(s) du cahier | Nature/Statut |
|---|---|---|
| ADR-001 — Exécution du domaine en mode local | §1, §3, §8 | pré-implémentation, Accepté |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | §1, §4, §7 | conception, Accepté |
| ADR-003 — Stack front : mono-écosystème Angular | §2 | pré-implémentation, Accepté |
| ADR-004 — Transport temps réel : SignalR | §2, §3, §9 | pré-implémentation, Accepté |
| ADR-005 — Modèle de monétisation | §3 (renvoi seul — quotas) | décision produit, fusionnée en vision — empreinte technique faible, traitée en renvoi |
| ADR-006 — Périmètre MVP | §9 (renvoi seul — origine de l'instrumentation) | décision produit, fusionnée en vision — empreinte technique faible, traitée en renvoi |
| ADR-007 — RGPD et modèle d'autorisation API | §5, §6 | conception, Accepté — **précurseur**, formalisé par ADR-012 et ADR-013 (RGPD, §6) ainsi que par ADR-014 et ADR-015 (modèle d'autorisation et sécurité authentification, §5) ; relation de subsomption portée, non aplatie |
| ADR-008 — Structure physique de la solution | §1, §2, §3 | pré-implémentation, Accepté |
| ADR-009 — FK `spaces.ownerId → users` | §4 | conception, Accepté — **précurseur**, raffiné par ADR-011 (matrice de cascade complète) et ADR-018 (renommage `Space`) ; relation de subsomption portée, non aplatie |
| ADR-010 — Suppression d'espace | §4, §6 | conception, Accepté — **précurseur**, raffiné par ADR-011 (mécanisme de cascade complet) et ADR-018 (renommage `Space`) ; relation de subsomption portée, non aplatie |
| ADR-011 — Cascade & intégrité référentielle | §3, §4, §6 | pré-implémentation, Accepté |
| ADR-012 — RGPD : effacement de compte | §6 | conception, Accepté |
| ADR-013 — RGPD : données des joueurs invités | §6 | conception, Accepté |
| ADR-014 — Modèle d'autorisation API | §5, §7 | conception, Accepté |
| ADR-015 — Sécurité authentification MVP | §2, §5, §7 | pré-implémentation, Accepté |
| ADR-016 — Sérialisation locale et contrat de migration | §5, §7, §8 | mixte, dominante pré-implémentation, Accepté |
| ADR-017 — Modèle IndexedDB local | §3, §5, §8 | mixte, dominante pré-implémentation, Accepté |
| ADR-018 — Généralisation de `Campaign` en `Space` | §4, §6, §11 | conception, Accepté |

*Fin de la présente version. Ce cahier consolide §0 à §12 du corpus technique. Toute évolution ultérieure du corpus (nouvelle décision, levée d'un point `[À TRANCHER]`, ADR post-MVP) est répercutée dans ce cahier par une passe dédiée, jamais anticipée ici.*

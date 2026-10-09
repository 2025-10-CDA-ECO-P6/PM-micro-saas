# Cahier des spécifications techniques

| Champ | Valeur |
|---|---|
| Version | 1.3 |
| Date | 2026-10-09 |
| Statut | Version de référence — consolidation du corpus |
| Porteur | SOUS-SPÉCIFIÉ |
| Validation | SOUS-SPÉCIFIÉ |
| Périmètre | MVP — trajectoire technique actée pour UC-01 à UC-12, UC-14 ; post-MVP signalé où il apparaît |
| Audiences | équipe de build, architecte, responsable produit (arbitrage des points non tranchés) |
| Sources | `docs/decisions/**` (registre ADR), `docs/architecture/*.md` (fondations, structure, stack), `docs/architecture/specs/**` (spécifications pré-build), `docs/domaine/**` (modèle de domaine), `docs/reference/cahier-des-charges.md` §9/§10 |

### Historique des révisions

| Version | Date | Auteur | Nature de la révision |
|---|---|---|---|
| 1.0 | 2026-07-16 | SOUS-SPÉCIFIÉ | SOUS-SPÉCIFIÉ |
| 1.1 | 2026-10-09 | Pierre-Marie Marchio | Correction du fond, sans restructuration : renvois, ordre d'autorité, marqueurs et sources des points ouverts, points déjà décidés retirés de la § 11, tableaux de renvoi, statuts des specs et § 12 alignés sur leurs sources |
| 1.2 | 2026-10-09 | Pierre-Marie Marchio | Correction du fond, sans restructuration : « miroir du cahier des charges » reformulé en correspondance avec ses sections techniques, sans en suivre l'ordre ; renvoi de NFR-PERF-04 vers une section qui ne parle pas de recherche ramené à UC-14 ; contenu de la conception détaillée aligné sur ce qu'elle porte (séquences, règles de gestion consolidées, machines d'états) |
| 1.3 | 2026-10-09 | Pierre-Marie Marchio | Restructuration sans changement de fond : encadré « Lire ce document », six parties, sections renumérotées de 1 à 13 (objet devenu § 1, anciens § 1 à 4 devenus § 2 à 5, migration remontée en § 6 après les données, anciens § 5, 7 et 6 devenus § 7, 8 et 9 dans l'ordre sécurité, contrats d'API, conformité, anciens § 9 à 12 devenus § 10 à 13), sommaire limité aux parties et aux sections, sous-sections des points ouverts remises dans l'ordre des sections, renvois internes et renvois entrants réparés |

> **Lire ce document**
>
> - **À qui** : à l'équipe de build, à l'architecte et au responsable produit, ce dernier pour arbitrer les points non tranchés.
> - **À quoi** : à vérifier en un seul endroit le QUOI technique du MVP — bornes, invariants, contrats, structures de données —, consolidé depuis les décisions d'architecture (ADR), les spécifications pré-build, les fiches d'exigences non fonctionnelles et le modèle de domaine. Le comment est dans la [conception détaillée](conception-detaillee.md).
> - **Comment le lire** : chaque partie répond à une question, lisez celle qui est la vôtre. Les sections 2 à 11 se terminent par un tableau de renvoi qui nomme leurs sources et ce que chacune porte. Un point non tranché est marqué là où il apparaît et regroupé en §12 ; la §13 donne, pour chaque source, les sections qui la consolident.
> - **Ce qui fait foi** : jamais ce document ([ADR-019, § 1](../decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité)). Document dérivé du corpus, zéro décision technique neuve. En cas d'écart avec une décision d'architecture, une spécification, une fiche NFR ou le modèle de domaine, la source fait foi et c'est ce cahier qu'on corrige.

**Sommaire**

- [Partie 1 — De quoi parle ce cahier, et qui fait foi ?](#partie-1--de-quoi-parle-ce-cahier-et-qui-fait-foi-)
  - [1. Objet et cadre d'autorité](#1-objet-et-cadre-dautorité)
- [Partie 2 — Sur quoi la solution repose-t-elle ?](#partie-2--sur-quoi-la-solution-repose-t-elle-)
  - [2. Principes](#2-principes)
  - [3. Stack](#3-stack)
  - [4. Contraintes](#4-contraintes)
- [Partie 3 — Quelles données le produit détient-il, et comment passent-elles au cloud ?](#partie-3--quelles-données-le-produit-détient-il-et-comment-passent-elles-au-cloud-)
  - [5. Données](#5-données)
  - [6. Migration](#6-migration)
- [Partie 4 — Qui accède à quoi, et que devient une donnée personnelle ?](#partie-4--qui-accède-à-quoi-et-que-devient-une-donnée-personnelle-)
  - [7. Sécurité](#7-sécurité)
  - [8. Contrats d'API](#8-contrats-dapi)
  - [9. Conformité](#9-conformité)
- [Partie 5 — Que mesure-t-on, et quelle expérience doit-on garantir ?](#partie-5--que-mesure-t-on-et-quelle-expérience-doit-on-garantir-)
  - [10. Observabilité](#10-observabilité)
  - [11. Exigences non fonctionnelles](#11-exigences-non-fonctionnelles)
- [Partie 6 — Que reste-t-il à trancher, et d'où vient chaque exigence ?](#partie-6--que-reste-t-il-à-trancher-et-doù-vient-chaque-exigence-)
  - [12. Points ouverts](#12-points-ouverts)
  - [13. Traçabilité, source vers section](#13-traçabilité-source-vers-section)

## Partie 1 — De quoi parle ce cahier, et qui fait foi ?

Cette partie dit ce que le cahier consolide, ce qu'il laisse à la conception détaillée et quelles sources font foi sur lui.

## 1. Objet et cadre d'autorité

> **Section provisoire.** Les choix décrits dans ce cahier s'appuient sur des décisions d'architecture (ADR) de nature pré-implémentation : elles cadrent une trajectoire technique retenue à ce stade de conception, mais n'ont pas encore été confrontées à la construction effective du MVP. Les éléments consolidés ci-après sont donc provisoires et susceptibles d'ajustement à l'entrée en build. Ce statut n'affaiblit pas la portée normative de chaque décision citée pour la phase courante — il en situe seulement la maturité.

### Ce que ce document fait

Ce cahier consolide, en un registre technique unique, le corpus déjà écrit — décisions d'architecture (ADR), spécifications pré-build, fiches d'exigences non-fonctionnelles, modèle de domaine — en sections qui reprennent chacune, quand il en existe une, la section technique correspondante du cahier des charges fonctionnel — le tableau de renvoi de la section la nomme —, sans en suivre l'ordre. Il rend le **QUOI technique** (bornes, invariants, contrats, structures de données) vérifiable en un seul endroit, sans en déplacer l'autorité.

### Ce que ce document ne fait pas

Il ne crée aucune décision technique neuve. Il n'invente aucun seuil chiffré absent du corpus. Il ne recopie pas le contenu intégral des spécifications sources — il synthétise et renvoie. Il ne reproduit pas la vue transverse par préoccupation de `docs/architecture/transverse.md`, ni la conception détaillée en avant (le « comment ») portée par le dossier de conception détaillée. `transverse.md` organise le même corpus ADR par préoccupation technique ; ce cahier le met en correspondance avec les sections techniques du cahier des charges. Aucun des deux ne cite l'autre comme autorité — en cas de divergence, l'ADR source fait foi — et toute évolution d'un ADR se répercute dans les deux. Il ne tranche aucun point ouvert du corpus : un point non tranché est reporté tel quel et consolidé au registre des points ouverts (§12).

### Clause d'autorité

Ce document ne porte aucune autorité propre ; il consolide et renvoie. En cas de divergence entre ce cahier et un ADR, une spécification, une fiche NFR ou le modèle de domaine, **la source fait foi**.

### Ordre d'autorité du corpus

L'ordre suivant n'est jamais renversé par ce document :

**personas → vision produit → use cases (source de vérité) → user journeys / user stories / NFR → domaine → glossaire.**

Toute référence ajoutée par ce document pointe vers le haut de cette hiérarchie, ou vers un pair de même niveau normatif (ADR, spec pré-build) déjà accepté dans le registre. Ce cahier ne devient à aucun moment une source que le corpus amont citerait en retour.

### Frontière avec le SDD (règle opérationnelle)

Ce cahier énonce **ce qui doit être vrai** — bornes, invariants, contrats, planchers, structure de données, périmètres. Le dossier de conception détaillée (SDD, `docs/reference/conception-detaillee.md`) énonce **comment le flux se déroule** — séquences, règles de gestion consolidées, machines d'états.

Cas-test à citer pour distinguer les deux registres : « toutes les FK sont `RESTRICT` et la suppression est pilotée par une saga » relève de ce cahier (QUOI) ; « la saga NULL-e les cycles en passe 1 puis DELETE les feuilles en passe 2 » relève du SDD (COMMENT). Ce cahier ne franchit jamais cette frontière — il cite la source du COMMENT en renvoi, il ne l'héberge pas.

### Convention de report des points ouverts

Tout point non tranché du corpus est reporté tel quel sous la forme `[À TRANCHER — <axe/référence>]`, ou sous le marqueur que sa source écrit (`[À VALIDER — JURISTE]` pour une qualification juridique), jamais comblé par une valeur ou une hypothèse. Ces points sont consolidés au registre des points ouverts (§12).

---

## Partie 2 — Sur quoi la solution repose-t-elle ?

Cette partie pose les principes fondateurs, les technologies retenues et les contraintes qui s'imposent à l'implémentation.

## 2. Principes

Cette section consolide les principes fondateurs qui cadrent l'ensemble des choix techniques du MVP : la place du mode local, la structure documentaire du contenu, et l'organisation en Clean Architecture / monolithe modulaire.

### Mode local : pilier non négociable

Travailler sans compte, avec persistance directement dans le navigateur, constitue un pilier non négociable du produit : le mode local. Techniquement, il se limite à une couche de persistance côté navigateur assortie de règles de validation minimales — création, modification, suppression, quelques contrôles élémentaires comme un titre non vide ou une structure de blocs valide — **sans dupliquer le domaine métier complet**. La **source de vérité unique** reste le domaine métier exécuté côté serveur : toute donnée locale qui rejoint le cloud y transite comme un **import revalidé**, jamais comme une copie acceptée sur confiance.

L'invariant transverse qui gouverne cette frontière est : **validation locale ⊆ validation serveur**, **au sens des règles** — aucune règle de validation côté client que le domaine serveur ne porte pas. Le mode local peut donc accepter un état que le serveur refusera ; le filet est la revalidation par les value objects à l'import, avec rapport de rejets. À l'inverse, une validation côté client qui refuse ce que le domaine serveur accepte est un défaut à retirer du client. **C#-first désigne l'autorité du contrat**, non l'ordre de construction de la couche cliente : celle-ci se construit contre le contrat du service d'accès au store local, sans attendre l'implémentation du domaine ([ADR-001, § Compléments post-revue (2026-09-03)](../decisions/ADR-001-execution-domaine-mode-local.md)).

### « Tout est document »

Un principe unique gouverne le contenu éditorial : **« tout est document »**. Scénario, PNJ, lieu, note ou fiche partagent le même identifiant (`DocumentId`) et la même structure — des blocs de contenu, reliés par des références ordonnées vers d'autres documents. Aucun de ces usages ne constitue un type séparé au sens d'un identifiant propre : « Scénario », « Scène » ou « Personnage » désignent des emplois du même document, pas des entités distinctes.

L'écriture des propriétés structurées d'un document — dont sa visibilité — passe par un value object dédié qui les valide contre le schéma déclaré pour le type concerné ; un champ jusque-là libre de toute contrainte se retrouve ainsi gouverné.

### Clean Architecture, inversion des dépendances, monolithe modulaire

L'inversion des dépendances de la **Clean Architecture** structure la solution : les contrats sont définis par le domaine métier, l'infrastructure les implémente, et la présentation en dépend — jamais l'inverse. Cette règle vaut quelle que soit la granularité physique retenue pour les projets.

Le découpage retenu est celui d'un **monolithe modulaire**, autour de **quatre frontières logiques** — Identity & Access, Space Management, Content Library, Session Conduct. Pour le MVP, ces frontières vivent à l'intérieur d'une solution unique, sous forme de namespaces et de contrats internes plutôt que d'assemblies séparées par contexte ; une extraction en composants indépendants reste ouverte si un besoin réel s'en fait sentir. Les préoccupations techniques transverses, elles — persistance, notifications temps réel, présentation —, sont isolées en composants distincts dès le démarrage. La coexistence des frontières logiques dans un même assembly a une conséquence directe : leur étanchéité n'est pas garantie par le compilateur (voir §4, test d'architecture CI).

### Tableau de renvoi — §2

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-001 — Exécution du domaine en mode local | conception | Accepté | Mode local = persistance + validations minimales ; domaine serveur = source de vérité ; invariant validation locale ⊆ serveur ; ordre C#-first |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | Identifiant unique `DocumentId` ; `DocumentProperties` gouverne l'écriture des propriétés structurées |
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | Clean Architecture, monolithe modulaire, 4 frontières logiques, multi-projets sur infrastructure/présentation |
| `docs/architecture/fondations.md` | fondations (conception) | — | Justification DDD, langage ubiquitaire, frontières explicites, règles métier dans le domaine |
| `docs/architecture/transverse.md` §1 | consolidation transverse | — | Renvoi vers le Dossier d'architecture pour la vue par bounded context |
| CdC §9.1 | synthèse produit | — | Formulation d'origine des trois principes ci-dessus |

---

## 3. Stack

Cette section nomme les technologies retenues pour le MVP et les mécanismes structurants associés — la nomination des technologies est pertinente à ce niveau du cahier, à la différence des sections centrées sur le besoin fonctionnel.

### Front : mono-écosystème Angular

**Angular** est l'unique écosystème front, aussi bien pour l'application principale (SPA) que pour la landing page — un choix qui évite à une équipe réduite de maintenir deux pipelines et deux jeux de composants distincts. La landing page en tire une déclinaison en rendu statique préconstruit (SSG/prerender), servie depuis un CDN sans surcharge serveur, pour répondre à ses besoins de référencement. Blazor WASM a été évalué et écarté : il aurait pu partager le domaine C# côté navigateur, mais au prix d'un écosystème de composants moins riche et d'un poids de runtime pénalisant au premier chargement — la conséquence sur le mode local (TypeScript minimal, pas de domaine partagé) est assumée.

### Backend et persistance

Le backend est construit en **ASP.NET Core / C#**, avec **PostgreSQL** comme base de données (recherche plein texte native, JSONB pour les blocs de contenu, UUID), **Entity Framework Core** comme ORM, et **ASP.NET Identity** pour l'authentification.

### Structure de la solution

La solution est structurée en six projets .NET/Angular, nommés et détaillés dans `docs/architecture/structure-projets.md` (source de vérité). Domaine et Application restent des projets uniques pour le MVP (les quatre bounded contexts DDD y sont des namespaces, pas des assemblies séparées — `SharedKernel` est lui aussi un namespace interne au projet Domaine, pas un projet à part) ; la granularité multi-projets est conservée sur l'infrastructure et la présentation, qui reflètent des préoccupations techniques distinctes du découpage DDD. Le détail des responsabilités et de la structure interne de chaque projet est renvoyé à `docs/architecture/structure-projets.md`.

### Temps réel : SignalR

Le partage d'informations en temps réel du MJ vers les joueurs s'appuie sur **SignalR**, natif à ASP.NET Core, avec **repli automatique** (WebSocket → Server-Sent Events → long-polling) selon les capacités du réseau. Le module est isolé dans `Haversack.Infrastructure.Notifications` dès le démarrage. La visibilité de la ressource concernée conditionne systématiquement ce qui est poussé : aucune information diffusée ne contourne les règles de partage fixées par le MJ, la même politique d'autorisation s'appliquant identiquement au canal REST et au canal SignalR (voir §7 et §8).

### Frontière d'exécution local / cloud

Le domaine C# serveur est la vérité unique de l'exécution métier. Le mode local persiste en TypeScript/IndexedDB avec des validations minimales, sans réimplémentation du domaine. Ce partage de responsabilité découle directement d'ADR-001 et du rejet de Blazor WASM (ADR-003) : deux implémentations métier en langages différents auraient produit une divergence structurelle inévitable.

### Brique sécurité nommée

L'algorithme de hachage des mots de passe retenu est **Argon2id**, avec un repli acceptable vers **bcrypt** (cost factor ≥ 12) si Argon2id n'est pas intégrable dans la contrainte de dépendances du MVP. La signature des jetons d'authentification est **asymétrique** (RS256 ou ES256), l'algorithme `alg: none` étant explicitement interdit à la validation. Les valeurs de configuration associées (durées de vie des jetons, seuils de rate limiting) sont consolidées en §7 — cette section ne nomme que la brique technique.

### Tableau de renvoi — §3

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-003 — Stack front : mono-écosystème Angular | pré-implémentation | Accepté | Angular SPA + Angular SSR/prerender landing ; Blazor WASM et Next.js écartés |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | SignalR natif, repli automatique, isolation `Haversack.Infrastructure.Notifications` |
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | 6 projets, domaine/application uniques, infrastructure/présentation multi-projets |
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Argon2id (repli bcrypt), signature asymétrique RS256/ES256, `alg: none` interdit |
| `docs/architecture/stack.md` | sélection technologique | — | Vue d'ensemble de la stack, alternatives évaluées par couche |
| `docs/architecture/structure-projets.md` | synthèse structurelle | — | Détail complet des 6 projets, granularité, nommage `SharedKernel` |
| CdC §9.2 | synthèse produit | — | Formulation d'origine de la stack |

---

## 4. Contraintes

Cette section consolide les contraintes structurantes qui s'imposent à l'implémentation, au-delà du choix de stack : ce que le compilateur ne garantit pas, l'invariant de validation croisée local/serveur, l'intégrité référentielle, l'hébergement temps réel et les bornes du mode local.

### Frontières de contexte non garanties par le compilateur

Les quatre frontières logiques (§2) coexistant dans un même assembly, leur respect n'est **pas garanti par le compilateur**. Cette limite est compensée par un **test d'architecture automatisé exécuté en CI**, qui remplace la discipline de revue de code jugée insuffisante en contexte d'équipe restreinte. Ce test est un livrable attendu avant la première mise en production ; le détail de son câblage (bibliothèque, règles précises) relève du dossier de conception détaillée.

### Invariant transverse de validation

L'invariant « validation locale ⊆ validation serveur » (§2) s'applique à l'ensemble du plan technique, pas seulement au mode local : **aucune règle de validation côté navigateur que le domaine serveur ne porte pas**. Il ne garantit pas que toute donnée acceptée côté navigateur soit acceptable à la revalidation — le mode local est volontairement plus pauvre, et le rapport de rejets à l'import est le filet prévu pour l'écart. C#-first en est la conséquence au sens de l'autorité du contrat, non de l'ordre de construction de la couche cliente ([ADR-001, § Compléments post-revue (2026-09-03)](../decisions/ADR-001-execution-domaine-mode-local.md)).

### Intégrité référentielle : FK `RESTRICT`, suppression par saga

Toutes les clés étrangères du graphe de données sont déclarées **`ON DELETE RESTRICT`** — le système de gestion de base de données ne cascade jamais. La progression des suppressions est pilotée intégralement par une **saga applicative** : une transaction unique par espace supprimé, avec garantie d'idempotence et de reprise après un arrêt en cours d'exécution. Ce cahier énonce le QUOI (RESTRICT partout, saga responsable de la cascade, transaction par espace, idempotence) ; le séquençage pas à pas de la saga (ordre des passes de déliaison, ordre topologique des suppressions) relève du dossier de conception détaillée et n'est pas reproduit ici.

Le passage d'un espace en purge s'appuie sur un **claim exclusif**, posé avant l'ouverture de la transaction, qui interdit toute restauration concurrente une fois posé. Le délai au-delà duquel ce claim est considéré expiré — rendant l'espace de nouveau éligible à une tentative de purge après un arrêt en cours d'exécution — reste `[À TRANCHER — timeout d'expiration du claim purge_claimed_at de la saga de suppression, config-securite-migration.md / ADR-011 B1]`.

### Contrainte d'hébergement temps réel

Le choix de SignalR (§3) impose des connexions persistantes et des sessions collantes (« sticky sessions ») côté hébergement. La configuration retenue est sobre : repli forcé vers Server-Sent Events tant que la communication reste unidirectionnelle (MJ → joueurs), fermeture de la connexion en fin de session active, et heartbeat allongé (les événements sont rares en session de jeu de rôle). Un mécanisme de repli vers un polling adaptatif est prévu au-delà d'un seuil de coût par session concurrente — ce seuil, sa métrique et sa cadence de repli restent `[À TRANCHER — spec repli-temps-reel.md §5]`, non chiffrés dans le corpus à ce stade.

### Mode local : capacité de stockage et posture migration-only

Le mode local ne porte aucun plafond de création d'espace : sa seule contrainte est la capacité de stockage du navigateur (UC-01 § Règles métier). Le plafond de trois espaces porte exclusivement sur la synchronisation cloud d'un compte gratuit (RB-02-10), espace personnel exclu du décompte. Sa posture de synchronisation est **migration-only** : aucune synchronisation continue entre le navigateur et le cloud n'existe au MVP — le seul passage de données vers le serveur est une migration ponctuelle, déclenchée explicitement par l'utilisateur, traitée intégralement ou pas du tout par espace.

### Stockage : quotas et paliers

Le palier local persiste les données exclusivement dans le navigateur, sans limite de volume propre au produit au-delà de celle de l'appareil. Les paliers gratuit et Pro disposent chacun d'un quota de stockage propre, le palier Pro offrant un volume sensiblement supérieur ; l'espace personnel n'est, dans les deux cas, jamais décompté du quota du palier gratuit. Les valeurs chiffrées de ces quotas restent en renvoi vers le cahier des charges (`docs/reference/cahier-des-charges.md`, annexe « Valeurs de référence chiffrées ») — elles ne sont pas recopiées ici.

### Tableau de renvoi — §4

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-008 — Structure physique de la solution | pré-implémentation | Accepté | Test d'architecture CI comme garde-fou des frontières logiques |
| `docs/architecture/structure-projets.md` §6 | synthèse structurelle | — | Détail du test d'architecture, règles à mettre en place |
| ADR-001 — Exécution du domaine en mode local | conception | Accepté | Invariant validation locale ⊆ serveur, ordre C#-first |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Toutes FK `RESTRICT`, sagas `SpaceDeleted`/`UserAnonymized`, transaction par espace, idempotence |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Configuration sobre (SSE forcé, fermeture fin de session, heartbeat allongé) |
| `docs/architecture/specs/repli-temps-reel.md` §2-3 | spec pré-build | cadre à compléter | Mécanisme de repli vers polling adaptatif ; seuil, métrique, cadence non chiffrés — `[À TRANCHER]` |
| `docs/architecture/specs/config-securite-migration.md` | spec pré-build | à confirmer à l'entrée en build | Timeout d'expiration du claim `purge_claimed_at` : mécanisme tranché, valeur renvoyée à B1 — `[À TRANCHER — B1]` |
| ADR-017 — Modèle IndexedDB local | mixte (dominante pré-implémentation) | Accepté | Absence de plafond de création en mode local, posture migration-only, absence de synchronisation continue |
| ADR-005 — Modèle de monétisation | décision produit, fusionnée en vision | Accepté | Quotas de stockage par palier (Pro : stockage étendu) — renvoi seul, empreinte technique faible |
| CdC §9.5 (+ renvoi annexe « Valeurs de référence chiffrées ») | synthèse produit | — | Quotas de stockage par palier, valeurs chiffrées en renvoi |

---

## Partie 3 — Quelles données le produit détient-il, et comment passent-elles au cloud ?

Cette partie décrit l'organisation du domaine et le contrat qui gouverne le passage des données du mode local au cloud.

## 5. Données

Cette section consolide l'organisation du domaine métier — quatre contextes bornés adossés à un noyau partagé, principe « tout est document », ontologie de l'espace — sans reproduire le détail porté par le modèle de domaine lui-même, auquel elle renvoie systématiquement.

### « Tout est Document » : identifiant unique et structure

Il n'existe plus de type `Scénario`, `Scène` ou `Personnage` distinct au sens d'un identifiant propre : un seul identifiant, `DocumentId`, couvre l'ensemble. Un document est composé de blocs de contenu et de références ordonnées vers d'autres documents — une structure récursive qui permet au MJ de rédiger un scénario monobloc, de le structurer en scènes liées, ou de réutiliser un document comme scène dans un autre contexte, sans qu'aucune organisation ne soit imposée par le système.

### `DocumentProperties` et schémas par type

Les propriétés structurées d'un document (dont sa visibilité) sont gouvernées par un value object dédié, `DocumentProperties`, dont l'écriture passe exclusivement par une méthode de domaine validée contre le schéma déclaré (`propertiesSchema`) du type de document concerné. Aucune écriture directe dans le champ libre sous-jacent n'est permise.

Huit types de document système sont seedés pour le MVP : scénario, scène, PNJ, lieu, note, personnage joueur, note live, révélation. Parmi ces huit, le schéma structuré du type **scénario** (un statut éditorial à quatre valeurs) et l'absence de schéma structuré supplémentaire pour le type **note live** (ses deux champs distinctifs sont promus en colonnes de premier niveau, hors du périmètre du VO) sont dérivés du modèle de domaine. Les schémas structurés des six autres types système sont **vides au MVP** — DÉCIDÉ 2026-09-03 ([document-properties-schemas.md, § 5](../architecture/specs/document-properties-schemas.md)) : leur contenu vit dans les blocs du document.

### Ontologie de l'espace (`Space`)

L'agrégat central du modèle est l'**espace** (`Space`), frontière de cohérence pour les membres, les invitations et le rattachement du contenu. La campagne, le one-shot et l'espace personnel ne sont pas des concepts distincts mais des **spécialisations d'un seul agrégat**, distinguées par un type (`SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`) : les différences de comportement entre ces trois formes sont comportementales, pas structurelles. L'espace personnel est créé par défaut à la création du compte, mono-membre par construction, et n'est jamais décompté du quota du palier gratuit. Il est aussi la zone d'atterrissage par défaut d'un document créé sans espace explicite : comportement opérationnel du MVP, DÉCIDÉ 2026-06-12 ([matrice de priorités, § Espace personnel — conteneur par défaut et capture-first](../besoin/vision/perimetre/matrice-de-priorites.md#espace-personnel--conteneur-par-défaut-et-capture-first-must-have--adr-018)).

### Intégrité référentielle : matrice des FK cross-module

Le graphe de cascade compte **38 clés étrangères**, toutes déclarées `ON DELETE RESTRICT` (§4). Sur ce total, la matrice de cascade recense **17 FK cross-module** (16 « Cross » pures + 1 mixte « Intra+Cross », `documents.source_document_id`).

Le décompte est de **17** : [ADR-009](../decisions/ADR-009-fk-campaign-owner.md) et [ADR-011](../decisions/ADR-011-cascade-integrite-referentielle.md) concordent. ADR-009 annonçait 19 avant d'être corrigé le 2026-09-02 (commit `f4214d1`) ; la matrice d'ADR-011 recense 17 lignes « Cross » ou « Intra+Cross ».

### Core (noyau partagé)

Le noyau partagé fournit aux quatre contextes bornés les identifiants typés (`UserId`, `SpaceId`, `GuestAccessId`, `SessionId`, `DocumentId`, `FolderId`), les value objects transverses (`Email`, `Slug`, `Tag`), les primitives de traçabilité (`AuditInfo`, `SoftDelete`) et l'énumération partagée de visibilité (`Visibility ∈ {PUBLIC, GM_ONLY, PLAYER_PRIVATE}`). Il ne contient jamais d'entité dotée d'un cycle de vie propre.

Une exception RGPD s'applique à `SoftDelete` : les contenus strictement privés à un joueur qui tombent sous une obligation légale d'effacement (fin de compte, fin d'accès invité sans conversion) sont supprimés **physiquement**, jamais simplement masqués — le masquage réversible est réservé au cycle de vie normal du contenu.

### Les quatre contextes bornés — responsabilité en un paragraphe, renvoi pour le détail

- **Identity & Access** gère les comptes utilisateurs authentifiés — identité, palier d'abonnement, suppression RGPD — sans connaître les notions de MJ, joueur, espace ou session. Détail : `docs/domaine/identity-access/context.md`.
- **Space Management** gère les espaces, leurs membres, leurs invitations et les accès invités — il décide qui entre dans quel espace et selon quel niveau d'accès, sans gérer le contenu ni le déroulement d'une session. Détail : `docs/domaine/space-management/context.md`.
- **Content Library** gère le contenu durable d'un espace — documents, dossiers, types de documents — comme référentiel de ce que le MJ prépare avant et entre les séances, sans porter les droits d'accès ni le déroulement d'une session. Détail : `docs/domaine/content-library/context.md`.
- **Session Conduct** gère le déroulement d'une session — cycle de vie, tableau de bord, documents épinglés, notes de session — sans posséder de contenu propre : il consomme ce que la bibliothèque de contenu a préparé. Détail : `docs/domaine/session-conduct/context.md`.

Les contextes bornés ne dépendent jamais directement les uns des autres : leurs échanges passent par des identifiants typés, des événements de domaine ou des contrats applicatifs. Les diagrammes associés à chaque contexte (MCD, MLD, classes, flux) sont disponibles dans les dossiers de contexte de `docs/domaine/` et ne sont pas reproduits ici.

### Tableau de renvoi — §5

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | `DocumentId` unique, `DocumentProperties`, régime de validation par type |
| `docs/architecture/specs/document-properties-schemas.md` | spec pré-build | à confirmer à l'entrée en build | 8 types système seedés ; schémas dérivés (scenario, live_note) ; `propertiesSchema` vide au MVP pour les 6 autres types |
| ADR-018 — Généralisation Campaign en Space | mixte (dominante pré-implémentation) | Accepté | `SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`, espace personnel mono-membre, hors quota |
| ADR-009 — FK `spaces.ownerId → users` | conception | Accepté | FK cross-module fondatrice ; décompte 17 FK cross-module (renvoi ADR-011) |
| ADR-010 — Suppression d'espace (soft-delete + purge + saga) | conception | Accepté | Distinction `Archive()` / `Delete()`, corbeille 30 jours, événement `SpaceDeleted` |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Matrice des 38 FK, 17 FK cross-module recensées, deux sagas, deux cycles traités |
| `docs/architecture/specs/mapping-ef-core.md` | spec pré-build | à confirmer à l'entrée en build | Converters d'IDs typés, owned types, `jsonb`, query filters, colonnes promues, FK `RESTRICT` |
| `docs/domaine/core/kernel.md` | domaine (conception) | — | Contenu du noyau partagé, règles de gouvernance d'admission, exception RGPD `SoftDelete` |
| CdC §10 | synthèse produit | — | Formulation d'origine du modèle de données et domaine |

---

## 6. Migration

Cette section consolide le contrat de données et le contrat d'échec de la frontière local↔cloud — format de sérialisation, frontière de confiance, parcours d'échec transactionnel, modèle de stockage local — sans reproduire l'algorithme d'import topologique, renvoyé au dossier de conception détaillée.

### Format et périmètre sérialisé

Le passage de données entre le mode local et le cloud repose sur **un seul format JSON versionné**, utilisé aussi bien pour l'export local que pour le payload de migration. L'enveloppe porte un `schemaVersion` entier obligatoire — un payload sans cette mention, ou dont la version est inconnue ou antérieure au seuil de compatibilité maintenu, est rejeté proprement. Le périmètre sérialisé couvre l'agrégat document et ses dépendances directes ainsi que l'historique des sessions terminées ; sont explicitement exclus : les sessions en cours, les préférences d'affichage de session (recréées à l'import), les comptes et appartenances (absents du mode local), et tout indicateur de cycle de vie gouverné par le serveur. L'espace personnel est inclus dans le périmètre sérialisé au même titre que les espaces partagés, et rattaché au compte créé au moment de l'import.

Deux catégories de champs se distinguent à l'import : les **champs gouvernés** (propriétés structurées, visibilité, identifiants de slug, statut de session) sont revalidés par le domaine ; le **champ libre** (contenu des blocs de document) est transporté sans revalidation sémantique mais reste soumis à sanitisation (renvoi §7).

### Frontière de confiance

Le serveur n'honore **aucun champ d'autorité** contenu dans le payload : les identifiants sont régénérés à l'import, la propriété et l'auteur sont assignés au compte authentifié qui déclenche la migration, les horodatages sont réémis côté serveur, et les indicateurs de cycle de vie serveur ne sont jamais transportés. La migration n'est pas conçue comme une preuve d'appartenance des données — le mode local n'ayant pas de compte, il n'existe rien à prouver — mais comme une **assignation de propriété** au moment de l'import.

### Ordre impératif à l'import

L'ordre est impératif et non négociable : **validation structurelle d'abord**, **sanitisation du contenu ensuite**, l'une et l'autre **avant** toute écriture. Un fichier importé n'est jamais écrit tel quel. [Le détail algorithmique de la résolution des références internes du payload et de l'ordre topologique de création relève du dossier de conception détaillée.]

### Parcours d'échec transactionnel

La granularité de la transaction d'import est **l'espace** : ni le payload entier, ni le document individuel. Le serveur effectue un contrôle de pré-validation sur chaque espace du payload, produit un rapport de rejets codifié par raison, importe séparément chaque espace valide, et laisse intactes les données locales correspondant aux espaces rejetés jusqu'à confirmation explicite. L'opération de migration est associée à un identifiant de lot dont le format est validé et qui garantit l'**idempotence**, scopée par utilisateur authentifié : un espace déjà importé dans le cadre d'un même lot n'est pas réimporté.

### Gate de confirmation anti-appropriation

Avant tout import, l'utilisateur se voit présenter les espaces détectés dans son navigateur (titres, volume estimé, historique de session, espace personnel inclus) et doit fournir une **confirmation explicite**. Ce gate est un **contrôle applicatif côté serveur** — la requête d'import porte un indicateur de consentement explicite, et le serveur rejette l'import si cet indicateur est absent, indépendamment de ce que le client a présenté. Il borne le risque d'appropriation accidentelle sur un poste partagé ; il ne constitue pas une preuve cryptographique d'appartenance.

### Modèle local IndexedDB

Le store local n'est pas un miroir relationnel des tables serveur : il est **enraciné sur l'agrégat de l'espace** (aggregate-rooted), organisation cohérente avec la posture CRUD du mode local et qui limite la fonction de projection vers le payload à un filtre nommé, sans reformatage structurel complexe. Les index locaux se limitent strictement aux chemins de lecture qu'exige le mode local — navigation par espace, par dossier, par document, recherche par titre — sans index spéculatif. La version interne du store IndexedDB est **distincte du `schemaVersion` du payload** : les deux contrats évoluent indépendamment, la fonction de projection absorbant les évolutions internes du store sans affecter le contrat de payload côté serveur.

### Points ouverts de configuration

Le seuil de `schemaVersion` minimale maintenue côté serveur et l'horizon de rétention du registre des lots de migration restent `[À TRANCHER — P7]`.

### Tableau de renvoi — §6

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-016 — Sérialisation locale et contrat de migration | mixte, dominante pré-implémentation | Accepté | Format versionné, frontière de confiance, parcours d'échec transactionnel par espace, gate de confirmation |
| ADR-017 — Modèle IndexedDB local | mixte, dominante pré-implémentation | Accepté | Structure aggregate-rooted, index restreints, versionnement du store distinct du payload, absence de plafond de création en mode local |
| ADR-001 — Exécution du domaine en mode local | conception | Accepté | Fondation — invariant validation locale ⊆ serveur, migration comme import revalidé |
| ADR-018 — Généralisation de `Campaign` en `Space` | mixte (dominante pré-implémentation) | Accepté | Inclusion de l'espace `PERSONAL` dans le périmètre sérialisé et la transactionnalité par espace |
| `docs/architecture/specs/sanitisation-csp.md` §2 | spec pré-build | à confirmer à l'entrée en build | Ordre validation puis sanitisation à l'import JSON local |
| `docs/architecture/specs/config-securite-migration.md` | spec pré-build | à confirmer à l'entrée en build | Seuil `schemaVersion` et rétention du lot de migration renvoyés P7 |
| `docs/architecture/structure-projets.md` §7 | synthèse structurelle | — | Structure de solution mobilisée par le handler d'import |
| CdC §9.2, §9.4 | synthèse produit | — | Formulation d'origine de la migration et de l'export |

---

## Partie 4 — Qui accède à quoi, et que devient une donnée personnelle ?

Cette partie consolide les planchers de sécurité, les contrats observables d'accès aux ressources et le régime technique de conformité au RGPD.

## 7. Sécurité

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

La liaison d'un compte OAuth à un compte préexistant est **interdite si l'email de ce compte n'est pas prouvé vérifié**, avec rejet silencieux (aucun message ne révèle l'existence du compte). Le cas d'un compte préexistant non vérifié portant la même adresse email qu'une preuve OAuth fraîche est résolu par une reprise de la coquille (bascule de son statut de vérification, neutralisation obligatoire du mot de passe préexistant, liaison à l'identité fédérée) plutôt que par la création d'un doublon — un doublon sur la même adresse n'étant pas implémentable au regard de l'unicité de l'email dans le système. Cette résolution reste `[À TRANCHER — PRODUIT]` ; le sort du contenu éventuel déjà rattaché à la coquille reprise est une facette distincte, renvoyée au dossier juridique. L'ajout futur d'un fournisseur à email de relais non canonique (ex. masquage d'adresse) est une dette nommée : il imposerait de revisiter la règle de liaison par email, non déclenchée par le périmètre MVP (Google et Discord, tous deux à email canonique).

### Invariant d'autorisation

Toute lecture d'une ressource d'un espace compose, **en ET**, une vérification d'**appartenance** (l'appelant est membre actif de l'espace, ou dispose d'un accès invité actif dont le périmètre couvre la ressource) et une vérification de **visibilité** portée par le domaine (document public, réservé au MJ, ou strictement privé à son auteur — cette dernière catégorie exclut le MJ sans exception de type de document). Ces deux vérifications sont composées en un **mécanisme unique centralisé**, appliqué identiquement aux appels d'API et à la diffusion en temps réel — jamais deux implémentations distinctes susceptibles de diverger. Deux autres vérifications, structurelles et indépendantes de l'appelant (espace ni supprimé ni en cours de purge ; document non marqué supprimé), s'y ajoutent sans en faire partie. [Le pipeline d'évaluation et son câblage exact relèvent du dossier de conception détaillée.]

### Non-révélation d'existence

Un principe transverse, énoncé une seule fois ici : l'accès à un document non partagé, une tentative de reconnexion après suppression de compte, et une liaison OAuth échouée ne révèlent jamais l'existence ou l'état de la ressource ou du compte visés. Un résidu assumé à ce principe est conservé par choix produit : le message d'inscription confirmant qu'une adresse email est déjà associée à un compte, mitigé par le rate limiting hybride ci-dessus — décision produit tracée, pas un oubli.

### Plancher de sanitisation et CSP

La sanitisation du contenu est une **liste blanche positive des deux côtés** (serveur et client) : les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont interdits et supprimés **sans exception**, quels que soient la bibliothèque ou la liste exacte de balises retenus — ce plancher est un critère d'acceptation non négociable. La politique CSP retenue restreint par défaut à l'origine de l'application (`default-src 'self'`), interdit tout script inline (`script-src 'self'`), et restreint les connexions sortantes à l'origine ou à l'API (`connect-src 'self'`) — cette dernière directive rend observable l'absence d'envoi réseau en mode local. Seule reste ouverte la bibliothèque de sanitisation exacte côté serveur, `[À TRANCHER — B1.2]` ([sanitisation-csp.md, § 5](../architecture/specs/sanitisation-csp.md)) ; les listes blanches (énumération fermée des types de nœuds), la bibliothèque côté client et les directives CSP complètes sont tranchées. À l'import d'un fichier local comme au parcours de migration, l'ordre est impératif : validation structurelle **puis** sanitisation, l'une et l'autre **avant** toute écriture (détail du parcours de migration en §6).

### Risques résiduels assumés

Le stockage local IndexedDB n'est pas chiffré at-rest — aucun secret utilisateur n'existe en mode sans-compte à partir duquel dériver une clé ; cette limitation est assumée et communiquée par un bandeau de confidentialité distinct du bandeau de durabilité (cohérent NFR-CONF-04). L'énumération de comptes évoquée ci-dessus est tracée comme risque accepté. Aucun blob média externalisé n'est prévu au MVP.

### Tableau de renvoi — §7

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Politique mot de passe, Argon2id/bcrypt, cycle de vie des jetons, rotation/denylist, rate limiting, OAuth Google/Discord, reclaim-in-place, résidu CWE-204 |
| ADR-014 — Modèle d'autorisation API | conception | Accepté | Invariant d'autorisation composé (appartenance + visibilité), mécanisme unique REST/temps réel, non-révélation d'existence |
| ADR-007 — RGPD et modèle d'autorisation API | conception | Accepté | Invariant d'autorisation fondateur, formalisé par ADR-014 ; reclassement des constats bloquants MVP formalisés par ADR-015 |
| ADR-016 — Sérialisation locale et contrat de migration | mixte, dominante pré-implémentation | Accepté | Plancher de sanitisation côté serveur par liste blanche positive, appliqué aussi à l'import (§2.4) |
| ADR-017 — Modèle IndexedDB local | mixte, dominante pré-implémentation | Accepté | Sécurité du mode local : sanitisation côté client, CSP, ordre validation puis sanitisation à l'import de fichier, non-chiffrement at-rest assumé et bandeau de confidentialité |
| `docs/architecture/specs/config-securite-migration.md` | spec pré-build | à confirmer à l'entrée en build | Registre des seuils (longueur mdp, Argon2id, durées de jetons, rate limiting, TTL reset) — bornes fixées vs valeurs renvoyées à B1.5 |
| `docs/architecture/specs/contrat-openapi.md` | spec pré-build | à confirmer à l'entrée en build | Principe de non-révélation d'existence, cas 403 tranchés, 403 vs 404 renvoyé B1.10 |
| `docs/architecture/specs/sanitisation-csp.md` | spec pré-build | à confirmer à l'entrée en build | Plancher de sanitisation liste blanche, posture CSP, ordre validation puis sanitisation |
| `docs/besoin/nfr/NFR-CONF-01` à `04` | NFR produit | — | Séparation notes privées/vue joueurs, isolation mode local, suppression effective, transparence des risques — renvoi croisé §11 |
| `docs/domaine/core/kernel.md` | domaine (conception) | — | Énumération `Visibility`, primitives de traçabilité mobilisées par l'invariant d'autorisation |
| CdC §9.3 | synthèse produit | — | Formulation d'origine de la sécurité & authentification |

---

## 8. Contrats d'API

Cette section consolide les contrats observables du canal d'accès aux ressources — décision d'accès, sémantique des codes HTTP, interfaces d'authentification, contrat de propriétés de document — sans reproduire les schémas de requête/réponse endpoint par endpoint, qui restent l'objet d'un contrat non encore stabilisé par le corpus.

### Décision d'accès en un point

La décision d'autorisation d'accès à une ressource est rendue par un service unique, retournant une valeur discriminée autorisé/refusé, consommé identiquement par le canal REST et par la diffusion en temps réel (renvoi §7). [Le pipeline d'évaluation exact relève du dossier de conception détaillée.]

### Sémantique des codes HTTP

Les scénarios suivants constituent des cas déjà tranchés par le corpus, tous en **403** — un refus légitime connu opposé à un appelant déjà reconnu dans l'espace ou la session : un invité hors du périmètre de sa session, un joueur ou un invité face à un document réservé au MJ (y compris épinglé en session), un invité face au document strictement privé d'un autre membre, un invité réassocié au même personnage face à une note privée d'un compte supprimé. Le choix entre **403 et 404** pour un appelant non-membre accédant à une ressource d'un autre espace reste ouvert par le corpus lui-même — une lecture dérivée du principe de non-révélation (404, cohérent avec l'absence d'appartenance de l'appelant) est proposée sans être tranchée : `[À TRANCHER — B1.10]`.

Un espace en corbeille ou en purge, ou un document marqué supprimé, ne constituent pas un cas d'erreur distinct au niveau du contrat : ils sont rendus invisibles en amont de toute résolution applicative, sans code ou corps de réponse dédié. Une requête de liens entrants (« quels documents pointent vers celui-ci ? ») exclut silencieusement, sans code d'erreur ni indicateur de filtrage partiel, tout document source non lisible par l'appelant. Les cinq points d'entrée d'authentification (connexion, rafraîchissement de jeton, validation de jeton d'accès invité, demande et confirmation de réinitialisation de mot de passe) renvoient **429** au-delà du seuil de rate limiting (renvoi §7). Une liaison OAuth rejetée pour cause de compte préexistant non vérifié est rejetée silencieusement, sans code distinctif permettant à l'appelant d'en déduire la cause.

Restent `[À TRANCHER — B1.10]`, sans proposition dérivée dans le corpus : les schémas de requête et de réponse exacts par point d'entrée, l'attribution fine des codes 400 et 401, la liste littérale et exhaustive des chemins REST (le corpus désigne des ressources et des scénarios, jamais des chemins arrêtés), le schéma exact du corps de réponse 429, le code HTTP exact du rejet silencieux de liaison OAuth, et le format de l'annotation de sécurité portée par le contrat OpenAPI.

### Interfaces applicatives observables d'authentification

Les garanties de sécurité liées aux jetons sont portées par des contrats applicatifs observables plutôt que par une configuration d'infrastructure opaque : validation de jeton, gestion de la denylist, signature, et exigence de vérification email avant opération sensible sont chacune un point de passage nommé. [Les signatures normatives complètes relèvent de l'annexe pré-implémentation de la source et du dossier de conception détaillée.]

### Contrat de propriétés de document

Toute écriture des propriétés structurées d'un document — dont sa visibilité — passe exclusivement par une méthode de domaine validée contre le schéma déclaré du type de document concerné ; aucune écriture directe dans le champ sous-jacent n'est permise. Deux régimes de validation coexistent : un schéma figé pour les types système non modifiés, une validation permissive par défaut pour les types personnalisés ou modifiés — choix délibéré pour ne pas bloquer l'extensibilité. Parmi les huit types système du MVP, seul le schéma du type scénario (un statut éditorial à quatre valeurs) et l'absence de schéma structuré du type note live (ses champs distinctifs étant promus en colonnes de premier niveau) sont dérivés du modèle de domaine ; les schémas des six autres types système sont vides au MVP (DÉCIDÉ 2026-09-03, [document-properties-schemas.md, § 5](../architecture/specs/document-properties-schemas.md)). En l'absence de type déclaré sur un document, cette méthode d'écriture est refusée (DÉCIDÉ 2026-09-03, [document-properties-schemas.md, § 1](../architecture/specs/document-properties-schemas.md)).

### Contrat de payload versionné

Le contrat d'échange local↔cloud (enveloppe `schemaVersion` / `exportedAt` / `appVersion` / `spaces[]`) est détaillé en §6 ; il n'est mentionné ici que comme le pendant, côté migration, de la validation des propriétés ci-dessus — les deux mécanismes empruntent le même chemin de revalidation par le domaine.

### Tableau de renvoi — §8

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-014 — Modèle d'autorisation API | conception | Accepté | Service de décision d'accès unique, cas IDOR de référence tranchés en 403, renvoi B1.10 pour 403/404 non-membre |
| ADR-015 — Sécurité authentification MVP | pré-implémentation | Accepté | Interfaces observables d'authentification, périmètre du rate limiting 429 |
| ADR-016 — Sérialisation locale et contrat de migration | mixte, dominante pré-implémentation | Accepté | Contrat de payload versionné (détail §6) |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | conception | Accepté | Méthode de domaine comme point de passage obligatoire, deux régimes de validation |
| `docs/architecture/specs/contrat-openapi.md` | spec pré-build | à confirmer à l'entrée en build | Sémantique 403 déjà tranchée, exclusion des backlinks, 429 sur endpoints d'authentification, points renvoyés B1.10 |
| `docs/architecture/specs/document-properties-schemas.md` | spec pré-build | à confirmer à l'entrée en build | Contrat `DocumentProperties`, deux régimes de validation, schémas dérivés vs à trancher des huit types système |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Mécanisme de décision d'accès partagé entre REST et diffusion temps réel |

---

## 9. Conformité

Cette section consolide le régime technique de conformité RGPD acté pour le MVP — effacement de compte, données des joueurs invités, cascade de suppression, instrumentation respectueuse — et signale, sans les requalifier, les axes que le corpus renvoie explicitement à une validation juridique préalable au lancement.

### Effacement de compte (droit à l'oubli, Art. 17)

L'effacement d'un compte procède par **anonymisation par réécriture** : l'email et le nom d'affichage sont remplacés par des valeurs neutres, l'identifiant technique de l'utilisateur est conservé pour la continuité des espaces partagés auxquels il appartenait. Le sort du contenu créé par l'utilisateur se répartit en trois catégories : les documents strictement privés à l'auteur (et les notes liées à ses personnages) sont **supprimés physiquement**, sans exception ; les documents non partagés au sens d'un critère opérationnel à trois conditions cumulatives (absence d'épinglage, absence de référencement depuis un document d'un autre utilisateur, absence d'instanciation par un autre document) sont également **supprimés physiquement**, sauf s'ils sont réservés au MJ, auquel cas ils rejoignent la catégorie suivante ; les documents partagés ou réservés au MJ sont **conservés** sous l'identifiant anonymisé, au titre de l'intérêt légitime de continuité pour les membres tiers d'un espace vivant. Le contenu de l'espace personnel de l'utilisateur, lui, est **supprimé intégralement et sans condition**, aucun tiers n'y ayant d'intérêt légitime de continuité. [Le séquençage exact de déliaison puis suppression, et l'algorithme de la saga, relèvent du dossier de conception détaillée.]

Le critère de partage ci-dessus, comme l'appartenance d'espace des documents, est évalué à l'instant de la **demande d'effacement**, non à l'instant de l'exécution effective du traitement — un jeu de sélection est matérialisé à cet instant et consommé sans recalcul par les traitements de suppression, ce qui referme un risque de décalage entre l'état constaté à la demande et l'état au moment de l'exécution. [Le mécanisme technique de matérialisation et de relocation relève du dossier de conception détaillée.]

Toute demande d'effacement est traitée dans un délai légal d'**1 mois**, extensible à **2 mois** sous réserve d'en informer la personne dans le premier mois.

### Données des joueurs invités

Le nom d'affichage saisi par un joueur invité est traité sur la base de l'**intérêt légitime** (Art. 6), avec une information affichée **au moment même de la saisie**, avant l'entrée en session — l'invité ne s'inscrivant pas, cette information ne peut pas être reportée à une page de compte. Les accès invités expirés sont purgés **90 jours après leur expiration**, indépendamment du cycle de vie de l'espace qui les porte (un espace partagé actif de longue durée ne justifie pas la conservation indéfinie d'accès expirés). Les métadonnées techniques de connexion associées (IP, horodatages) sont conservées au maximum **30 jours** après la fin de l'accès. La posture retenue sur les mineurs est celle d'un service non destiné aux enfants, avec attestation déclarative d'un âge minimal de 16 ans, sans mécanisme de vérification d'âge ni de consentement parental.

### Cascade et intégrité à la suppression

La suppression d'un espace procède par corbeille puis purge physique différée de 30 jours. La suppression de compte déclenche la purge inconditionnelle et immédiate du contenu de l'espace personnel, dans la même fenêtre de traitement légale que l'anonymisation du compte. [Le détail des deux traitements de suppression — ordre topologique, garanties transactionnelles — relève du dossier de conception détaillée et n'est pas reproduit ici ; voir §4, §5.]

### Instrumentation respectueuse

L'instrumentation de mesure d'usage du MVP est conçue pour mesurer l'occurrence d'un usage sans capter le contenu narratif créé ou partagé par le MJ — cohérent avec l'isolation des données en mode local (renvoi §10).

### Export et possession des données

L'export d'un espace réutilise le même format de sérialisation versionné que la migration locale vers le cloud, garantissant que les données exportées restent lisibles et réutilisables hors de l'application (renvoi §6).

### Axes soumis à validation juridique préalable au lancement (non requalifiés ici)

Cinq axes, consolidés dans un cadrage dédié destiné à une revue juridique, restent des **questions ouvertes de qualification légale** — aucun n'est vérifiable par une validation continue applicative, chacun exige un avis externe :

1. Qualification d'un service « destiné aux enfants » (Art. 8) et suffisance de l'attestation déclarative 16+, pour le compte utilisateur comme pour l'accès invité.
2. Qualification sous-traitant / responsable de traitement (Art. 28) pour les contenus créés par le MJ décrivant des tiers identifiables, et périmètre exact du contrat de sous-traitance envisagé — y compris son extension au contenu de l'espace personnel.
3. Mise en balance formelle de l'intérêt légitime, non conduite par un juriste, pour la conservation des documents partagés d'un compte supprimé et pour la base légale du nom d'affichage invité.
4. Suffisance, au regard de l'Art. 17, de la suppression inconditionnelle du contenu de l'espace personnel, y compris les deux points subsidiaires sur l'instant de référence en cas de déplacement de document entre espace personnel et espace partagé.
5. Sort du contenu éventuellement déjà rattaché à un compte non vérifié repris par une preuve d'identité fédérée fraîche (reclaim-in-place, §7).

Chacun de ces cinq axes reste `[À VALIDER — JURISTE]` ; ce cahier ne les requalifie pas en décisions et ne préjuge d'aucune issue.

### Tableau de renvoi — §9

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| ADR-012 — RGPD : effacement de compte | conception | Accepté | Procédure d'anonymisation, catégorisation du contenu (physique/non partagé/conservé/personnel), délai Art. 12§3, mécanisme de figement à la demande |
| ADR-013 — RGPD : données des joueurs invités | conception | Accepté | Base légale du nom d'affichage, information Art. 13, rétention 90/30 jours, posture mineurs |
| ADR-007 — RGPD et modèle d'autorisation API | conception | Accepté | Principe fondateur de l'anonymisation par réécriture ; formalisé par ADR-012 et ADR-013 |
| ADR-011 — Cascade & intégrité référentielle | pré-implémentation | Accepté | Sagas `SpaceDeleted` / `UserAnonymized` consommées par la politique RGPD ci-dessus |
| ADR-010 — Suppression d'espace | conception | Accepté | Corbeille + purge J+30 pour les espaces supprimables par le MJ, raffiné par ADR-011 pour le mécanisme complet |
| ADR-018 — Généralisation de `Campaign` en `Space` | mixte (dominante pré-implémentation) | Accepté | Purge inconditionnelle du contenu de l'espace personnel à la suppression de compte ; périmètre du contrat de sous-traitance étendu à l'espace personnel |
| `docs/architecture/specs/requete-effacement-non-partage.md` | spec pré-build, illustrative non normative | — | Critère opérationnel « document non partagé », instant de référence, trou nommé sur les références nullable entrantes |
| `docs/assurance/conformite/cadres/cadrage-validation-pre-lancement-eu.md` | dossier juridique | à valider juriste | Les 5 axes de validation légale préalable au lancement EU, consolidés depuis ADR-012/013/018/015 |
| CdC §9.4 | synthèse produit | — | Formulation d'origine de la conformité RGPD |

---

## Partie 5 — Que mesure-t-on, et quelle expérience doit-on garantir ?

Cette partie rassemble ce que le MVP doit mesurer et les exigences non fonctionnelles, rattachées à leurs identifiants.

## 10. Observabilité

Cette section consolide l'instrumentation compensatoire de validation du MVP et le suivi de coût du canal temps réel — deux mécanismes dont le corpus fixe l'intention et le principe sans en arrêter les valeurs.

### Instrumentation d'activation par pilier

Le MVP livrant en un seul bloc (préparation, vue de session, cloud, partage) plutôt qu'en temps de livraison séparés, une instrumentation compensatoire mesure l'occurrence d'usage sur trois piliers, chacun composite (une seule création ne suffit pas à qualifier l'activation) : activation préparation (espace créé + N documents créés), activation vue de session (session ouverte + usage réel constaté), activation partage (document partagé + au moins un joueur l'ayant ouvert). Cette instrumentation mesure une **occurrence**, jamais le **contenu narratif** produit ou partagé par le MJ — cohérent avec l'isolation des données en mode local (renvoi §7/§9). Le seuil N de l'activation préparation est de trois documents, et l'« usage réel constaté » de l'activation vue de session est au moins une action du MJ pendant la session — DÉCIDÉ 2026-09-03 ([matrice de priorités, § Instrumentation de validation du MVP](../besoin/vision/perimetre/matrice-de-priorites.md#instrumentation-de-validation-du-mvp)).

Une capture d'email **non bloquante**, accompagnée d'un dispositif d'analytics qualifié d'« anonyme RGPD », s'applique **dès le mode local** — avant même la création d'un compte cloud. Le transport de la mesure est tranché : les compteurs sont écrits dans le store local et ne quittent le navigateur qu'à la création d'un compte, ce qui rend l'outil analytics externe sans objet en mode local ; la capture d'email est proposée une seule fois, après l'activation préparation, et un refus ou une absence de réponse vaut refus définitif — DÉCIDÉ 2026-09-03 ([matrice de priorités, § Instrumentation de validation du MVP](../besoin/vision/perimetre/matrice-de-priorites.md#instrumentation-de-validation-du-mvp)).

### Suivi de coût et repli du canal temps réel

Un seuil de coût par session concurrente constitue le critère de réversibilité déclenchant le repli du canal temps réel vers un polling adaptatif ; ce seuil est un **livrable de configuration exploitable en production**, pas une mention documentaire sans effet opérationnel. En amont de tout repli, la configuration retenue est déjà sobre : transport forcé en flux à sens unique tant que la communication reste MJ→joueurs, fermeture de connexion en fin de session active, fréquence de battement de vie allongée (cohérent avec la rareté des événements en session de jeu de rôle).

### Points ouverts

Les points suivants restent `[À TRANCHER — telemetrie.md §6 / repli-temps-reel.md §5]`, non comblés par ce cahier : le schéma d'événements formalisé (noms techniques, propriétés), la définition même de la métrique de coût par session concurrente, la valeur du seuil de repli, la cadence cible du polling adaptatif, et le mécanisme de détection du dépassement de seuil.

Deux maillons ne sont, par nature, pas vérifiables par une validation continue applicative : le comportement de coût et de repli réel sous charge (dépend de conditions d'hébergement en production) et le comportement de persistance/éviction du stockage navigateur (renvoi §6) — ces maillons sont nommés pour éviter une fausse confiance dans la couverture des vérifications automatisées, pas pour signaler un manque à corriger.

### Tableau de renvoi — §10

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| `docs/architecture/specs/telemetrie.md` | spec pré-build | cinq points sur six tranchés (2026-09-03) | Trois piliers d'activation composites, principe occurrence sans contenu narratif, cinq des six points tranchés (2026-09-03), seul le schéma d'événements ouvert |
| `docs/architecture/specs/repli-temps-reel.md` §1-2, §4-5 | spec pré-build | cadre à compléter | Seuil de coût comme livrable de configuration, configuration sobre actée, quatre points ouverts |
| ADR-006 — Périmètre MVP | décision produit, fusionnée en vision | Accepté | Origine de l'instrumentation compensatoire par pilier et de la capture email/analytics anonyme |
| ADR-004 — Transport temps réel : SignalR | pré-implémentation | Accepté | Origine du seuil de coût par session concurrente et de la configuration sobre |
| `docs/besoin/nfr/NFR-CONF-02` | NFR produit | — | Principe d'isolation des données en mode local, contrainte sur les mesures d'usage |
| CdC §9.4 | synthèse produit | — | Formulation d'origine de l'instrumentation respectueuse |

---

## 11. Exigences non fonctionnelles

Les vingt fiches NFR du corpus de conception (`docs/besoin/nfr/`) portent les identifiants stables et sont la **source unique** des critères d'expérience. Cette section ne les recopie pas : elle référence chaque identifiant et ne consolide que ce qu'une source technique fixe effectivement au-delà du critère produit — le reste reste renvoyé, jamais reformulé.

### Performance perçue

| ID NFR | Exigence technique dérivée | Seuil technique |
|---|---|---|
| NFR-PERF-01 | Fluidité de la navigation en session | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-02 | Réactivité à la première interaction | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-03 | Continuité sans interruption lors des actions en session | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |
| NFR-PERF-04 | Résultats de recherche immédiats — recherche titre-only au MVP (renvoi UC-14) | `[À TRANCHER — seuil de performance non chiffré dans le corpus]` |

Les quatre seuils chiffrés sont ouverts par conception — le corpus ne fixe aucune métrique de latence ou de délai ([stratégie de test, § 12](../assurance/strategie-de-test.md#12-registre-des-points-ouverts), PO-01).

### Hors connexion

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-OFF-01 | Périmètre offline intégral en mode local (préparation, vue de session, recherche locale) |
| NFR-OFF-02 | Durabilité best-effort des données locales entre sessions — retrouvées intactes après fermeture du navigateur ou redémarrage de l'appareil ; `navigator.storage.persist()` comme mécanisme de durabilité. Volet alerte délégué à NFR-OFF-03 |
| NFR-OFF-03 | Aucune perte silencieuse — avertissement non bloquant préalable et action de sécurisation proposée dès qu'une perte devient possible (garantie de conservation non obtenue, stockage saturé) ; bandeau distinct du bandeau de confidentialité (NFR-CONF-04), à ne pas fusionner |
| NFR-OFF-04 | Fonctionnement partiel en cas de perte réseau passagère en mode cloud, en session (renvoi ADR-017 §2(c) pour le régime des trois postures local/migration/cloud) |
| NFR-OFF-05 | Continuité d'édition en préparation cloud hors session lors d'une perte de réseau — conservation locale et synchronisation automatique au retour de connexion, sans action du MJ ; distinct de NFR-OFF-04 (session active) |

### Confidentialité

Le corps technique de CONF-01 à CONF-04 est porté par les §7 et §9 ci-dessus ; cette entrée est un renvoi croisé, pas un dédoublement.

### Accessibilité

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-ACC-01 | Parcours intégralement exécutables au clavier, focus visuellement identifiable en permanence |
| NFR-ACC-02 | Contenu, actions et changements d'état exploitables et annoncés par les lecteurs d'écran usuels |
| NFR-ACC-03 | Contraste suffisant en conditions de faible éclairage |
| NFR-ACC-04 | Tailles de texte adaptées à la lecture rapide en session |

Le référentiel WCAG 2.1 niveau AA fournit un cadre de mesure pour ces quatre exigences, sans s'y substituer : il oriente l'implémentation et n'est pas converti en critère de recette produit ([NFR, § Accessibilité](../besoin/nfr/README.md#4-accessibilité) ; [stratégie de test, § 3.6](../assurance/strategie-de-test.md)).

### Internationalisation

| ID NFR | Exigence technique dérivée |
|---|---|
| NFR-I18N-01 | Interface entièrement en français au MVP |
| NFR-I18N-02 | Externalisation des chaînes et disposition tolérante à la longueur des libellés — évolutivité, pas un engagement de calendrier |
| NFR-I18N-03 | Traitement Unicode de bout en bout pour le contenu créé par le MJ, indépendant de la langue de l'interface |

### Bornes MVP à répercuter comme décisions de périmètre

Les bornes suivantes sont des **décisions de périmètre assumées**, pas des travaux différés : synchronisation multi-appareils exclue du MVP, recherche titre-only mono-espace, partage par document (pas par joueur), multilinguisme non planifié pour cette version, disposition droite-à-gauche et texte vertical hors MVP.

### Tableau de renvoi — §11

| Source | Nature | Statut | Ce qu'elle porte |
|---|---|---|---|
| `docs/besoin/nfr/README.md` | NFR produit, index | — | Cinq familles NFR, périmètre MVP assumé par famille |
| `docs/besoin/nfr/NFR-PERF-01` à `04` | NFR produit | — | Performance perçue, seuils chiffrés non fixés |
| `docs/besoin/nfr/NFR-OFF-01` à `05` | NFR produit | — | Fonctionnement hors connexion, deux bandeaux distincts, continuité d'édition en préparation cloud (NFR-OFF-05) |
| `docs/besoin/nfr/NFR-CONF-01` à `04` | NFR produit | — | Confidentialité — corps technique en §7/§9 |
| `docs/besoin/nfr/NFR-ACC-01` à `04` | NFR produit | — | Accessibilité, référentiel WCAG 2.1 AA |
| `docs/besoin/nfr/NFR-I18N-01` à `03` | NFR produit | — | Internationalisation, périmètre français au MVP |
| CdC §8 | synthèse produit | — | Reformulation des vingt exigences non fonctionnelles en expérience vécue |

---

## Partie 6 — Que reste-t-il à trancher, et d'où vient chaque exigence ?

Cette partie regroupe, section par section, les points encore à trancher, puis relie chaque source aux sections qui la consolident.

## 12. Points ouverts

Le corpus technique consolidé dans ce cahier est de nature pré-implémentation : les points listés ci-dessous sont à confirmer ou à trancher à l'entrée en construction, section par section. Aucun n'est comblé par ce cahier — chacun est reporté tel qu'il apparaît dans sa source.

### §4 — Contraintes

- Timeout d'expiration du claim `purge_claimed_at` de la saga de suppression — `[À TRANCHER — config-securite-migration.md / ADR-011 B1]`.

### §5 — Données

Aucun point ouvert dans cette section.

### §6 — Migration

- Seuil de `schemaVersion` minimale maintenue côté serveur — `[À TRANCHER — P7]`.
- Horizon de rétention du registre des identifiants de lot de migration — `[À TRANCHER — P7]`.
- Migration ascendante du payload (compatibilité des versions de `schemaVersion` antérieures) — dette nommée, post-MVP ([ADR-016, § 1.4](../decisions/ADR-016-serialisation-locale-migration.md) et § Dettes nommées (non silencieuses)).

### §7 — Sécurité

- Paramètres exacts d'Argon2id (mémoire, itérations, parallélisme) — `[À TRANCHER — B1.5]`.
- Valeurs exactes des seuils de rate limiting (par-IP, par-compte, borne supérieure) — `[À TRANCHER — B1.5]`.
- Valeur exacte du TTL du token de réinitialisation dans la plage 10-15 min — `[À TRANCHER — B1.5]`.
- Fraîcheur maximale et déclenchement de la ré-authentification IdP pour la définition d'un premier mot de passe sur compte fédéré — `[À TRANCHER — B1.5]`.
- Résolution reclaim-in-place du cas email OAuth = compte préexistant non vérifié — `[À TRANCHER — PRODUIT]`.
- Ajout futur d'un fournisseur OAuth à email de relais non canonique — dette nommée, non déclenchée par le périmètre MVP ([ADR-015, § Points à trancher](../decisions/ADR-015-securite-authentification-mvp.md#points-à-trancher)).
- Bibliothèque de sanitisation exacte côté serveur — `[À TRANCHER — B1.2]` ([sanitisation-csp.md, § 5](../architecture/specs/sanitisation-csp.md)) ; le reste du point (listes blanches, bibliothèque côté client, directives CSP complètes) est tranché.

### §8 — Contrats d'API

- Choix entre 403 et 404 pour un appelant non-membre accédant à une ressource d'un autre espace — `[À TRANCHER — B1.10]`.
- Schémas de requête et de réponse exacts par point d'entrée — `[À TRANCHER — B1.10]`.
- Attribution fine des codes 400 et 401 par point d'entrée — `[À TRANCHER — B1.10]`.
- Liste littérale et exhaustive des chemins REST — `[À TRANCHER — B1.10]`.
- Schéma exact du corps de réponse 429 — `[À TRANCHER — B1.10]`.
- Code HTTP exact du rejet silencieux de liaison OAuth — `[À TRANCHER — B1.10]`.
- Format de l'annotation de sécurité portée par le contrat OpenAPI — `[À TRANCHER — B1.10]`.

### §9 — Conformité

- Critère opérationnel exact de « document non partagé » : la requête précise reste à écrire à l'implémentation — `[À TRANCHER — ADR-012, § Points à trancher]` ([ADR-012](../decisions/ADR-012-rgpd-effacement-compte.md#points-à-trancher)).
- Seuil d'invocation de l'extension du délai de traitement à 2 mois — à documenter dans la procédure de support — `[À TRANCHER — ADR-012, § Points à trancher]` ([ADR-012](../decisions/ADR-012-rgpd-effacement-compte.md#points-à-trancher)).
- Qualification juridique de l'instant de référence figé en cas de déplacement d'un document entre espace personnel et espace partagé, dans les deux sens — non arbitrée par le mécanisme technique de figement — `[À VALIDER — JURISTE]` ([ADR-012, § 7](../decisions/ADR-012-rgpd-effacement-compte.md)).
- Trou nommé sur les références nullable entrantes non couvertes par le critère de sélection (colonnes portées par d'autres documents ou par des accès invités pointant vers un document sélectionné pour suppression) — `[À TRANCHER — ticket]` ([requete-effacement-non-partage.md, § 5](../architecture/specs/requete-effacement-non-partage.md)).
- Les 5 axes du [cadrage de validation juridique pré-lancement EU](../assurance/conformite/cadres/cadrage-validation-pre-lancement-eu.md) — `[À VALIDER — JURISTE]` : qualification Art. 8 (mineurs), qualification sous-traitant Art. 28 et périmètre du contrat associé (y compris l'espace personnel), mise en balance de l'intérêt légitime (documents conservés + nom d'affichage invité), suffisance Art. 17 du hard-delete inconditionnel de l'espace personnel, sort du contenu d'une coquille reprise par reclaim-in-place.

### §10 — Observabilité

- Schéma d'événements formalisé (noms exacts, typage) — `[À TRANCHER — telemetrie.md §6]`.
- Définition de la métrique de coût, valeur du seuil de repli, cadence cible du polling adaptatif, mécanisme de détection du dépassement — `[À TRANCHER — repli-temps-reel.md §5]`.

### §11 — Exigences non fonctionnelles

- Les quatre seuils chiffrés de performance perçue (PERF-01 à 04) — `[À TRANCHER — seuil de performance non chiffré dans le corpus]` ([stratégie de test, § 12](../assurance/strategie-de-test.md#12-registre-des-points-ouverts), PO-01).

---

## 13. Traçabilité, source vers section

Cette matrice prouve que chaque spec pré-build, chaque section technique du cahier des charges, et chaque décision d'architecture du registre (ADR-001 à ADR-018) trouve sa ou ses sections d'accueil dans ce cahier ; les ADR-019 à ADR-023, de nature méthode, règlent la tenue du corpus et n'y sont pas consolidées. Les ADR précurseurs raffinés par une décision ultérieure sont signalés comme tels — la relation de subsomption est portée, pas aplatie.

### Specs pré-build

| Source | Type | Section | Nature/Statut |
|---|---|---|---|
| `docs/architecture/specs/config-securite-migration.md` | spec | §4, §5, §6, §7 | à confirmer à l'entrée en build |
| `docs/architecture/specs/contrat-openapi.md` | spec | §7, §8 | à confirmer à l'entrée en build |
| `docs/architecture/specs/document-properties-schemas.md` | spec | §5, §8 | à confirmer à l'entrée en build |
| `docs/architecture/specs/mapping-ef-core.md` | spec | §5 | à confirmer à l'entrée en build — empreinte technique de configuration EF Core, portée par renvoi seulement |
| `docs/architecture/specs/repli-temps-reel.md` | spec | §4, §10 | cadre à compléter |
| `docs/architecture/specs/requete-effacement-non-partage.md` | spec, illustrative non normative | §9 | — |
| `docs/architecture/specs/sanitisation-csp.md` | spec | §6, §7 | à confirmer à l'entrée en build |
| `docs/architecture/specs/telemetrie.md` | spec | §10 | cinq points sur six tranchés (2026-09-03) |

### Cahier des charges

| Source | Type | Section | Nature/Statut |
|---|---|---|---|
| CdC §8 — Exigences non fonctionnelles | §CdC | §11 | synthèse produit |
| CdC §9 — Contraintes | §CdC | §2, §3, §4, §6, §7, §9, §10 | synthèse produit, section provisoire |
| CdC §10 — Modèle de données et domaine | §CdC | §5 | synthèse produit |

### Décisions d'architecture (ADR-001 à ADR-018)

| ADR | Section | Nature/Statut |
|---|---|---|
| ADR-001 — Exécution du domaine en mode local | §2, §4, §6 | conception, Accepté |
| ADR-002 — « Tout est Document » et gouvernance de `properties` | §2, §5, §8 | conception, Accepté |
| ADR-003 — Stack front : mono-écosystème Angular | §3 | pré-implémentation, Accepté |
| ADR-004 — Transport temps réel : SignalR | §3, §4, §8, §10 | pré-implémentation, Accepté |
| ADR-005 — Modèle de monétisation | §4 (renvoi seul — quotas) | décision produit, fusionnée en vision, Accepté — empreinte technique faible, traitée en renvoi |
| ADR-006 — Périmètre MVP | §10 (renvoi seul — origine de l'instrumentation) | décision produit, fusionnée en vision, Accepté — empreinte technique faible, traitée en renvoi |
| ADR-007 — RGPD et modèle d'autorisation API | §7, §9 | conception, Accepté — **précurseur**, formalisé par ADR-012 et ADR-013 (RGPD, §9) ainsi que par ADR-014 et ADR-015 (modèle d'autorisation et sécurité authentification, §7) ; relation de subsomption portée, non aplatie |
| ADR-008 — Structure physique de la solution | §2, §3, §4 | pré-implémentation, Accepté |
| ADR-009 — FK `spaces.ownerId → users` | §5 | conception, Accepté — **précurseur**, raffiné par ADR-011 (matrice de cascade complète) et ADR-018 (renommage `Space`) ; relation de subsomption portée, non aplatie |
| ADR-010 — Suppression d'espace | §5, §9 | conception, Accepté — **précurseur**, raffiné par ADR-011 (mécanisme de cascade complet) et ADR-018 (renommage `Space`) ; relation de subsomption portée, non aplatie |
| ADR-011 — Cascade & intégrité référentielle | §4, §5, §9 | pré-implémentation, Accepté |
| ADR-012 — RGPD : effacement de compte | §9 | conception, Accepté |
| ADR-013 — RGPD : données des joueurs invités | §9 | conception, Accepté |
| ADR-014 — Modèle d'autorisation API | §7, §8 | conception, Accepté |
| ADR-015 — Sécurité authentification MVP | §3, §7, §8 | pré-implémentation, Accepté |
| ADR-016 — Sérialisation locale et contrat de migration | §6, §7, §8 | mixte, dominante pré-implémentation, Accepté |
| ADR-017 — Modèle IndexedDB local | §4, §6, §7 | mixte, dominante pré-implémentation, Accepté |
| ADR-018 — Généralisation de `Campaign` en `Space` | §5, §6, §9 | mixte (dominante pré-implémentation), Accepté |

*Fin de la présente version. Ce cahier consolide §1 à §13 du corpus technique. Toute évolution ultérieure du corpus (nouvelle décision, levée d'un point `[À TRANCHER]`, ADR post-MVP) est répercutée dans ce cahier lors de sa prochaine mise à jour, jamais anticipée ici.*

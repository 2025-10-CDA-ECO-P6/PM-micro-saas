# Haversack

Haversack est une application web pour le **Maître du Jeu** (MJ) de jeu de rôle : elle l'aide à préparer ses parties, à les conduire, et à partager avec ses joueurs ce qu'il choisit de leur montrer.

Ce dépôt contient **la conception complète du produit** (dans `docs/`, en français) et **l'outillage qui vérifie cette conception** (dans `tools/`, en C#). Il ne contient pas encore de code de l'application : le développement commence au jalon J0, décrit plus bas.

**Sommaire**

- [En deux minutes](#en-deux-minutes)
- [Ce qui reste à faire](#ce-qui-reste-à-faire)
- [Votre première heure sur le projet](#votre-première-heure-sur-le-projet)
- [Ensuite, selon votre rôle](#ensuite-selon-votre-rôle)
- [Je cherche…](#je-cherche)
- [Comment la documentation est rangée](#comment-la-documentation-est-rangée)
- [Les mots de la méthode](#les-mots-de-la-méthode)
- [Vérifier la documentation](#vérifier-la-documentation)
- [Contribuer](#contribuer)

---

## En deux minutes

**Ce que fait le produit.**

- Le MJ ouvre l'application et commence à écrire tout de suite, **sans créer de compte** : ses données restent dans son navigateur.
- Il range ses scénarios, personnages non joueurs, lieux et notes en dossiers, dans des **espaces** : un espace personnel, puis un espace par campagne ou par one-shot.
- Pendant la partie, une **vue session** lui met sous la main ce qu'il a préparé et lui permet d'improviser du contenu sans quitter l'écran.
- Pour partager avec ses joueurs ou sauvegarder en ligne, il crée un compte. Les **joueurs rejoignent par un lien**, sans compte.
- Haversack ne gère ni les règles, ni les dés, ni les combats : il s'adresse à tous les systèmes de jeu.

Ces choix viennent de la [vision produit, § 2](docs/besoin/vision/vision.md#2-positionnement), qui fait foi. Les mots du produit (espace, document, session, accès invité…) sont définis dans le [glossaire](docs/glossaire.md).

**La pile technique prévue.** Une API en .NET (C#), une application web Angular, une base PostgreSQL pour le mode en ligne, et un stockage dans le navigateur pour le mode sans compte : voir la [stack technologique](docs/architecture/stack.md).

**Où en est le projet (au 2026-10-09).**

- La conception est terminée et vérifiée : besoin, domaine, interface, décisions d'architecture, sécurité, plan de test. Les contrôles automatiques passent (voir [Vérifier la documentation](#vérifier-la-documentation)).
- Le développement n'a pas commencé. Il suit quatre jalons : **J0** socle, **J1** mode local sans compte, **J2** compte en ligne et migration, **J3** partage avec les joueurs et temps réel. Entre J1 et J2, une **décision de marché** dit si l'on continue ([roadmap de build, § 2](docs/projet/roadmap-de-build.md#2-vue-de-synthèse)).
- Les hypothèses que le produit doit démontrer n'ont pas encore été confrontées au terrain : aucun entretien avec des MJ n'a eu lieu ([guide d'entretien, § 8](docs/besoin/decouverte/guide-d-entretien.md#8-résultats)).

**Qui décide.** Le porteur du projet valide les arbitrages de périmètre et les décisions de passage (go/no-go) : voir la [note de cadrage, § 6](docs/projet/note-de-cadrage.md#6-organisation-et-gouvernance).

---

## Ce qui reste à faire

Les tâches vivent dans le [plan de travail](docs/projet/plan-de-travail.md), et les questions encore ouvertes dans les [décisions en attente](docs/projet/decisions-en-attente.md). Voici l'essentiel, dans l'ordre.

### 1. Lever les décisions qui bloquent le démarrage

Les [décisions en attente](docs/projet/decisions-en-attente.md) déclarent bloquantes avant J0 :

- au **groupe 1** (équipe de build) : 1.a, l'arborescence interne des couches .NET et TypeScript ; 1.b, le nom du projet de test d'architecture ; 1.c, le découpage du module `châssis` ; et les autres décisions de structure listées au même groupe ;
- au **groupe 2** (responsable produit) : 2.a, le critère d'acceptation de l'instrumentation de validation.

### 2. Démarrer le développement (jalon J0)

Ce que vous pouvez prendre lundi, d'après le premier lot du plan ([§ 9](docs/projet/plan-de-travail.md#9-lots-parallélisables)) :

- **`TB-001`** : confirmer, une par une, les décisions d'architecture prises avant l'implémentation. Rien ne la bloque.
- **`TB-007`** : retenir les outils du contrôle d'architecture. Rien ne la bloque.
- **`TB-012`** (convention de commit et de branche) : la convention est déjà tranchée dans le [guide de conventions, § 5](docs/projet/conventions-dod.md#5-domaine-de-convention--conventions-de-commit-et-de-branche) ; il reste à constater ses critères et à clore la tranche.
- **`TB-104`** (projet Angular) : elle est dans ce lot mais appartient à J1, et son ticket n'est pas encore prêt.

La [vue des épiques](docs/projet/plan-de-travail.md#4-vue-des-épiques) dit, pour chaque jalon, ce qui doit être vrai à la fin. La [méthode de ticket](docs/projet/methode-de-ticket.md) dit quand une tâche est prête et comment la mener jusqu'au bout.

### 3. Valider le produit auprès de vrais MJ

Deux tranches du plan attendent des entretiens, et ne doivent pas être écrites sans eux :

- `TB-106` : mener les entretiens du [guide d'entretien](docs/besoin/decouverte/guide-d-entretien.md) et en écrire les comptes rendus ;
- `TB-105` : écrire le modèle de fonctionnement et le guide de contenu, avec le porteur du projet.

### 4. Avant le lancement dans l'Union européenne

Une validation juridique sur cinq axes (mineurs, sous-traitance, intérêt légitime, effacement, reprise de compte) bloque le lancement commercial en Europe, pas le développement : voir la [roadmap de build, § 3.5](docs/projet/roadmap-de-build.md#35-validation-juridique-eu--validation-juridique-pré-lancement) et le [cadrage de validation juridique](docs/assurance/conformite/cadres/cadrage-validation-pre-lancement-eu.md).

### 5. Entretenir la documentation

`TB-107` liste les reprises de fond de la documentation encore à faire : quelques alignements entre documents, deux maquettes à refaire. Les risques du projet sont suivis dans le [registre des risques](docs/projet/registre-des-risques.md).

---

## Votre première heure sur le projet

Quel que soit votre rôle, lisez dans cet ordre :

1. **La [vision produit](docs/besoin/vision/vision.md), § 1 et 2** : le problème du MJ et les choix assumés. Ils expliquent presque tous les choix suivants.
2. **La [matrice de priorités](docs/besoin/vision/perimetre/matrice-de-priorites.md)**, en survol : ce qui est dans la première version et ce qui n'y est pas. C'est la seule autorité sur le périmètre.
3. **Le [glossaire](docs/glossaire.md)**, en survol : les mots du produit ont un sens précis, et tous les documents les emploient ainsi.
4. **La [roadmap de build, § 2](docs/projet/roadmap-de-build.md#2-vue-de-synthèse)** : les jalons et ce qui les sépare.
5. **Le [guide de lecture par fonctionnalité](docs/projet/guide-de-lecture.md)** : choisissez une fonctionnalité, par exemple la vue session, et suivez-la de son besoin jusqu'à sa maquette, ses décisions et sa spécification. C'est la meilleure façon de comprendre comment les documents s'articulent.

---

## Ensuite, selon votre rôle

### Vous allez développer

1. [Stack technologique](docs/architecture/stack.md) : la pile et les raisons de chaque choix.
2. [Structure des projets](docs/architecture/structure-projets.md) : la solution .NET, ses projets et l'ordre de construction.
3. [Modèle du domaine](docs/domaine/README.md) : un noyau partagé (`core/`) et quatre contextes métier, chacun avec ses entités, ses règles et ses événements. Si le vocabulaire du DDD vous est nouveau, lisez d'abord les [fondations architecturales](docs/architecture/fondations.md).
4. [Guide de conventions et définition de fini](docs/projet/conventions-dod.md) : les conventions de code, de commit et de branche, et ce qui fait qu'une tâche est terminée.
5. [Registre des décisions](docs/decisions/README.md) : les décisions d'architecture (ADR) et les options écartées, à ouvrir dès qu'une contrainte surprend.
6. Le plan de travail par ses sections utiles, pas en entier : la [vue des épiques](docs/projet/plan-de-travail.md#4-vue-des-épiques), puis la fiche de la tranche que vous prenez ; et la [méthode de ticket](docs/projet/methode-de-ticket.md).

### Vous pilotez le produit

1. [Personas](docs/besoin/decouverte/personas/README.md) : pour qui l'on conçoit.
2. [Cas d'utilisation](docs/besoin/cas-d-utilisation/README.md) : ce que le produit doit faire, capacité par capacité ; c'est la source de vérité du besoin. Leurs [diagrammes](docs/besoin/cas-d-utilisation/diagrammes.md) en donnent la vue d'ensemble.
3. [Parcours utilisateur](docs/besoin/parcours-utilisateur/README.md) : le vécu de chaque persona, étape par étape.
4. [Zonage de l'interface](docs/interface/zonage.md) et [fiches d'écran](docs/interface/ecrans/README.md) : la navigation et le contenu de chaque écran.
5. [Roadmap produit](docs/projet/roadmap-produit.md), [note de cadrage](docs/projet/note-de-cadrage.md) et [décisions en attente](docs/projet/decisions-en-attente.md) : la trajectoire, le cadre du projet, et ce qu'il reste à trancher.

### Vous préparez la recette

1. [User stories](docs/besoin/stories/README.md) : leurs scénarios Gherkin sont les seuls critères d'acceptation.
2. [Exigences non fonctionnelles](docs/besoin/nfr/README.md) : performance, confidentialité, accessibilité, fonctionnement hors ligne.
3. [Stratégie de test et cahier de recette](docs/assurance/strategie-de-test.md) : les niveaux de test et les cas de recette par cas d'utilisation.
4. [Traçabilité](docs/tracabilite.md) : pour chaque cas d'utilisation, ses stories, ses règles, ses cas de test.
5. [Dossier de sécurité](docs/assurance/securite/dossier-securite.md) : les menaces et contre-mesures à éprouver.

---

## Je cherche…

| Je cherche… | Je lis… |
|---|---|
| ce qui est dans la première version | la [matrice de priorités](docs/besoin/vision/perimetre/matrice-de-priorites.md) |
| comment une fonctionnalité doit marcher | son [cas d'utilisation](docs/besoin/cas-d-utilisation/README.md), puis sa [user story](docs/besoin/stories/README.md) ; le [guide de lecture](docs/projet/guide-de-lecture.md) donne tous les renvois d'une fonctionnalité |
| le sens d'un mot | le [glossaire](docs/glossaire.md) |
| un écran | le [zonage](docs/interface/zonage.md) et les [fiches d'écran](docs/interface/ecrans/README.md) |
| le modèle de données, les règles métier | le [domaine](docs/domaine/README.md), un dossier par contexte ; les événements sont dans le [catalogue d'événements](docs/domaine/evenements.md) |
| pourquoi tel choix technique | le [registre des décisions](docs/decisions/README.md) |
| les données personnelles et le RGPD | le [registre des traitements](docs/assurance/securite/registre-des-traitements.md) et l'[analyse d'impact](docs/assurance/securite/analyse-d-impact.md) |
| la prochaine tâche | le [plan de travail](docs/projet/plan-de-travail.md#4-vue-des-épiques) |
| une question pas encore tranchée | les [décisions en attente](docs/projet/decisions-en-attente.md) |
| une vue d'ensemble sans lire tout le corpus | les [documents de référence](docs/reference/README.md) : cahier des charges, dossier d'architecture, conception détaillée |

---

## Comment la documentation est rangée

Tout est dans `docs/`, un dossier par couche ; chaque dossier a son propre index. La [carte de `docs/`](docs/README.md) les décrit en une ligne chacun.

| Dossier | On y trouve |
|---|---|
| [`besoin/`](docs/besoin/README.md) | la vision, les personas, les cas d'utilisation, les stories, les parcours, les exigences non fonctionnelles |
| [`domaine/`](docs/domaine/README.md) | le modèle métier, par contexte : entités, règles, événements, données |
| [`interface/`](docs/interface/README.md) | le zonage, les conventions d'écran, les fiches et maquettes d'écran |
| [`decisions/`](docs/decisions/README.md) | les décisions d'architecture (ADR) |
| [`architecture/`](docs/architecture/README.md) | la pile, la structure de la solution, les spécifications techniques |
| [`assurance/`](docs/assurance/README.md) | la sécurité, la stratégie de test, la conformité |
| [`reference/`](docs/reference/README.md) | des documents de synthèse, lisibles seuls |
| [`projet/`](docs/projet/README.md) | la conduite du projet : roadmaps, plan de travail, risques, conventions |

**Qui fait foi.** Quand deux documents se contredisent, l'un fait foi et l'autre se corrige. Sur le fond, le besoin passe en premier : les cas d'utilisation font foi sur ce que le produit doit faire, le domaine les sert, et les documents de synthèse (`reference/`, `projet/`) ne font jamais foi. L'ordre exact est décidé par l'[ADR-019, § 1](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité) et repris dans les [conventions du corpus](docs/conventions.md).

---

## Les mots de la méthode

Les documents de conduite du projet emploient quelques termes propres :

- une **tranche** (`TB-nnn`) est une tâche du plan de travail ; une **épique** (`EP-nn`) regroupe des tranches ; un **lot** (`LOT-nn`) regroupe des tranches qu'on peut mener en parallèle ([plan de travail, § 1 et § 9](docs/projet/plan-de-travail.md#1-règles-de-lecture)) ;
- **HORS MAILLE** marque une tranche qui ne touche aucun code (une décision, une convention) ; **TROU** marque une tranche dont le corpus ne nomme pas encore ce qu'elle touchera ([plan de travail, § 2](docs/projet/plan-de-travail.md#2-natures-du-périmètre-décriture)) ;
- dans les documents, **DÉCIDÉ** (avec une date) clôt un point, **À TRANCHER** et **SOUS-SPÉCIFIÉ** signalent un point ouvert ([registre des marqueurs](docs/conventions.md#registre-des-marqueurs)).

---

## Vérifier la documentation

La documentation est vérifiée par des outils C#, lancés à chaque modification par la CI ([controles-corpus.yml](.github/workflows/controles-corpus.yml)). Pour les lancer vous-même, il faut le SDK .NET 10. Depuis la racine du dépôt, lancez chaque commande telle quelle, sans tube ni redirection, pour lire son vrai code de sortie :

```
dotnet test tools/Tests/CaracterisationOutils.csproj
dotnet run --project tools/VerifCorpus
dotnet run --project tools/VerifMailleAgregat
dotnet run --project tools/VerifMarqueursIdentifiants
dotnet run --project tools/VerifCouvertureRecette
```

| Outil | Ce qu'il vérifie | Résultat attendu |
|---|---|---|
| `tools/Tests` | l'outillage lui-même (tests de caractérisation) | tous les tests passent |
| `VerifCorpus` | liens, ancres, documents orphelins, renvois « § » vers une section | 0 partout, code 0 |
| `VerifMailleAgregat` | la cohérence du plan de travail avec le domaine | code 0 |
| `VerifMarqueursIdentifiants` | les marqueurs de statut et les identifiants définis deux fois | 0 écart nouveau, code 0 |
| `VerifCouvertureRecette` | la couverture des cas de recette par le plan | code 1, voir ci-dessous |

`VerifCouvertureRecette` sort toujours en code 1, parce que quelques écarts connus sont assumés. Ce n'est pas une erreur : comparez sa liste à [`ecarts-attendus.txt`](tools/VerifCouvertureRecette/ecarts-attendus.txt) ; seul un écart absent de ce fichier est une régression. De même, chaque outil range ses écarts acceptés dans un fichier de référence à côté de lui. Le sens détaillé de chaque sortie est dans le [guide de conventions, § 8.8](docs/projet/conventions-dod.md#88-conventions-de-tenue-du-corpus).

`tools/CreerIssues` et `tools/CreerProjet` ne vérifient rien : ils reportent le plan de travail en issues et en tableau de projet GitHub, par le workflow [recalcul-suivi-projet.yml](.github/workflows/recalcul-suivi-projet.yml). Au 2026-10-09, ce workflow est désactivé ; il se réactive par `gh workflow enable recalcul-suivi-projet.yml`.

---

## Contribuer

- **Un fait, un seul endroit.** Chaque information a un document propriétaire ; ailleurs, on y renvoie par un lien, on ne la recopie pas. Les [conventions du corpus](docs/conventions.md) disent qui possède quoi.
- **Corriger à la source d'abord.** Si un document contredit celui qui fait foi, on le corrige ; si c'est la source qui est fausse, on la corrige puis on aligne ceux qui la citent.
- **Langue.** La documentation est en français ; le code, ses identifiants et ses messages sont en anglais ([ADR-019, § 9](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#9-langue)).
- **Commits et branches.** Conventional Commits, et une branche par tranche : voir le [guide de conventions, § 5](docs/projet/conventions-dod.md#5-domaine-de-convention--conventions-de-commit-et-de-branche).
- **Avant de commiter** une modification de `docs/`, lancez les contrôles de [Vérifier la documentation](#vérifier-la-documentation).

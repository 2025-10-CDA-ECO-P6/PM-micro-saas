# Haversack — Documentation

Outil d'assistance au Maître du Jeu pour la préparation et la conduite de parties de jeu de rôle. Cette page est le point d'entrée de la documentation : elle propose un parcours de lecture, dit où trouver chaque sujet et décrit les dossiers. Pour une présentation du projet, voir le [README du dépôt](../README.md).

**Sommaire**

- [Votre première heure sur le projet](#votre-première-heure-sur-le-projet)
- [Ensuite, selon votre rôle](#ensuite-selon-votre-rôle)
- [Je cherche…](#je-cherche)
- [Comment ce corpus est organisé](#comment-ce-corpus-est-organisé)
- [Les dossiers](#les-dossiers)
- [Les documents d'entrée](#les-documents-dentrée)
- [Les mots de la méthode](#les-mots-de-la-méthode)

---

## Votre première heure sur le projet

Quel que soit votre rôle, lisez dans cet ordre :

1. **La [vision produit](besoin/vision/vision.md), § 1 et 2** : le problème du MJ et les choix assumés. Ils expliquent presque tous les choix suivants.
2. **La [matrice de priorités](besoin/vision/perimetre/matrice-de-priorites.md)**, en survol : ce qui est dans la première version et ce qui n'y est pas. C'est la seule autorité sur le périmètre.
3. **Le [glossaire](glossaire.md)**, en survol : les mots du produit ont un sens précis, et tous les documents les emploient ainsi.
4. **La [roadmap de build, § 2](projet/roadmap-de-build.md#2-vue-de-synthèse)** : les jalons et ce qui les sépare.
5. **Le [guide de lecture par fonctionnalité](projet/guide-de-lecture.md)** : choisissez une fonctionnalité, par exemple la vue session, et suivez-la de son besoin jusqu'à sa maquette, ses décisions et sa spécification. C'est la meilleure façon de comprendre comment les documents s'articulent.

Ce qui reste à faire vit dans le [plan de travail](projet/plan-de-travail.md#4-vue-des-épiques) ; les questions encore ouvertes, dans les [décisions en attente](projet/decisions-en-attente.md) ; les risques, dans le [registre des risques](projet/registre-des-risques.md). Le porteur du projet valide les arbitrages de périmètre et les décisions de passage : voir la [note de cadrage, § 6](projet/note-de-cadrage.md#6-organisation-et-gouvernance).

---

## Ensuite, selon votre rôle

### Vous allez développer

1. [Stack technologique](architecture/stack.md) : la pile et les raisons de chaque choix.
2. [Structure des projets](architecture/structure-projets.md) : la solution .NET, ses projets et l'ordre de construction.
3. [Modèle du domaine](domaine/README.md) : un noyau partagé (`core/`) et quatre contextes métier, chacun avec ses entités, ses règles et ses événements. Si le vocabulaire du DDD vous est nouveau, lisez d'abord les [fondations architecturales](architecture/fondations.md).
4. [Guide de conventions et définition de fini](projet/conventions-dod.md) : les conventions de code, de commit et de branche, et ce qui fait qu'une tâche est terminée.
5. [Registre des décisions](decisions/README.md) : les décisions d'architecture (ADR) et les options écartées, à ouvrir dès qu'une contrainte surprend.
6. Le plan de travail par ses sections utiles, pas en entier : la [vue des épiques](projet/plan-de-travail.md#4-vue-des-épiques), puis la fiche de la tranche que vous prenez ; et la [méthode de ticket](projet/methode-de-ticket.md), qui dit quand une tâche est prête et comment la mener jusqu'au bout.

### Vous pilotez le produit

1. [Personas](besoin/decouverte/personas/README.md) : pour qui l'on conçoit.
2. [Cas d'utilisation](besoin/cas-d-utilisation/README.md) : ce que le produit doit faire, capacité par capacité ; c'est la source de vérité du besoin. Leurs [diagrammes](besoin/cas-d-utilisation/diagrammes.md) en donnent la vue d'ensemble.
3. [Parcours utilisateur](besoin/parcours-utilisateur/README.md) : le vécu de chaque persona, étape par étape.
4. [Zonage de l'interface](interface/zonage.md) et [fiches d'écran](interface/ecrans/README.md) : la navigation et le contenu de chaque écran.
5. [Roadmap produit](projet/roadmap-produit.md), [note de cadrage](projet/note-de-cadrage.md) et [décisions en attente](projet/decisions-en-attente.md) : la trajectoire, le cadre du projet, et ce qu'il reste à trancher.
6. [Guide d'entretien](besoin/decouverte/guide-d-entretien.md) : comment confronter les hypothèses du produit à de vrais MJ.

### Vous préparez la recette

1. [User stories](besoin/stories/README.md) : leurs scénarios Gherkin sont les seuls critères d'acceptation.
2. [Exigences non fonctionnelles](besoin/nfr/README.md) : performance, confidentialité, accessibilité, fonctionnement hors ligne.
3. [Stratégie de test et cahier de recette](assurance/strategie-de-test.md) : les niveaux de test et les cas de recette par cas d'utilisation.
4. [Traçabilité](tracabilite.md) : pour chaque cas d'utilisation, ses stories, ses règles, ses cas de test.
5. [Dossier de sécurité](assurance/securite/dossier-securite.md) : les menaces et contre-mesures à éprouver.

---

## Je cherche…

| Je cherche… | Je lis… |
|---|---|
| ce qui est dans la première version | la [matrice de priorités](besoin/vision/perimetre/matrice-de-priorites.md) |
| comment une fonctionnalité doit marcher | son [cas d'utilisation](besoin/cas-d-utilisation/README.md), puis sa [user story](besoin/stories/README.md) ; le [guide de lecture](projet/guide-de-lecture.md) donne tous les renvois d'une fonctionnalité |
| le sens d'un mot | le [glossaire](glossaire.md) |
| un écran | le [zonage](interface/zonage.md) et les [fiches d'écran](interface/ecrans/README.md) |
| le modèle de données, les règles métier | le [domaine](domaine/README.md), un dossier par contexte ; les événements sont dans le [catalogue d'événements](domaine/evenements.md) |
| pourquoi tel choix technique | le [registre des décisions](decisions/README.md) |
| les données personnelles et le RGPD | le [registre des traitements](assurance/securite/registre-des-traitements.md) et l'[analyse d'impact](assurance/securite/analyse-d-impact.md) |
| ce qu'il faut avant un lancement en Europe | la [roadmap de build, § 3.5](projet/roadmap-de-build.md#35-validation-juridique-eu--validation-juridique-pré-lancement) et le [cadrage de validation juridique](assurance/conformite/cadres/cadrage-validation-pre-lancement-eu.md) |
| la prochaine tâche | le [plan de travail](projet/plan-de-travail.md#4-vue-des-épiques) |
| une question pas encore tranchée | les [décisions en attente](projet/decisions-en-attente.md) |
| une vue d'ensemble sans lire tout le corpus | les [documents de référence](reference/README.md) : cahier des charges, dossier d'architecture, conception détaillée |

---

## Comment ce corpus est organisé

Le corpus se range en un dossier par couche, du besoin vers sa réalisation. Deux dossiers consolident le reste sans jamais faire foi : [`reference/`](reference/README.md) rassemble les documents **de référence** — autoportants, lisibles seuls, exportables hors de ce dépôt — et [`projet/`](projet/README.md) porte le pilotage. Les autres dossiers portent le **corpus de traçabilité détaillé** : c'est là que vivent les décisions, leur justification et leur détail.

Chaque dossier ci-dessous est propriétaire de son propre index ; ce document n'en recopie pas le contenu.

**Qui fait foi.** Quand deux documents se contredisent, l'un fait foi et l'autre se corrige. Sur le fond, le besoin passe en premier : les cas d'utilisation font foi sur ce que le produit doit faire, le domaine les sert, et les documents de synthèse (`reference/`, `projet/`) ne font jamais foi. L'ordre exact est décidé par l'[ADR-019, § 1](decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité) et repris dans les [conventions du corpus](conventions.md).

---

## Les dossiers

| Dossier | Rôle | Documents clés |
|---|---|---|
| [`besoin/`](besoin/README.md) | Le besoin produit : personas, vision et priorités, use cases, user stories, user journeys, parcours de bout en bout, exigences non fonctionnelles | [vision](besoin/vision/vision.md), [matrice de priorités](besoin/vision/perimetre/matrice-de-priorites.md), [cas d'utilisation](besoin/cas-d-utilisation/README.md) |
| [`domaine/`](domaine/README.md) | Le modèle de domaine DDD : un dossier par contexte métier et un noyau partagé, avec règles, agrégats, événements, diagrammes et modèles de données | [noyau partagé](domaine/core/kernel.md), [catalogue d'événements](domaine/evenements.md) |
| [`interface/`](interface/README.md) | La conception de l'interface : zonage et navigation, conventions, fiches d'écran et maquettes basse fidélité | [zonage](interface/zonage.md), [fiches d'écran](interface/ecrans/README.md), [conventions d'interface](interface/conventions.md) |
| [`decisions/`](decisions/README.md) | Le registre unique des décisions d'architecture (ADR) | [ADR-008, structure de la solution](decisions/ADR-008-structure-solution.md), [ADR-019, tenue du corpus](decisions/ADR-019-conventions-de-tenue-du-corpus.md) |
| [`architecture/`](architecture/README.md) | La documentation technique : fondations DDD, stack, structure des projets, préoccupations transverses et spécifications pré-build ; porte aussi le dossier [`deploiement/`](architecture/deploiement/README.md), réservé | [stack](architecture/stack.md), [structure des projets](architecture/structure-projets.md), [fondations](architecture/fondations.md) |
| [`assurance/`](assurance/README.md) | La stratégie de test et de recette, la [sécurité technique](assurance/securite/README.md), les cadrages de conformité (RGPD) en attente de validation juridique, et le dossier [`ecoconception/`](assurance/ecoconception/README.md), réservé | [stratégie de test](assurance/strategie-de-test.md), [dossier de sécurité](assurance/securite/dossier-securite.md), [registre des traitements](assurance/securite/registre-des-traitements.md) |
| [`reference/`](reference/README.md) | Les documents de référence autoportants, consolidations du corpus détaillé | [cahier des charges](reference/cahier-des-charges.md), [dossier d'architecture](reference/dossier-architecture.md), [conception détaillée](reference/conception-detaillee.md) |
| [`projet/`](projet/README.md) | Le pilotage : cadrage, roadmaps, plan de travail, méthode de ticket, conventions et définition de fini, risques, décisions en attente, revues | [roadmap de build](projet/roadmap-de-build.md), [plan de travail](projet/plan-de-travail.md), [conventions et définition de fini](projet/conventions-dod.md) |

---

## Les documents d'entrée

| Document | Rôle |
|---|---|
| [conventions.md](conventions.md) | Les conventions de tenue du corpus : ordre d'autorité, registre des identifiants, registre des marqueurs, renvois |
| [glossaire.md](glossaire.md) | La référence de forme des termes du corpus ; il fixe le nom des termes, pas leur sens |
| [corpus.md](corpus.md) | La sélection des modules du corpus lue par l'outillage d'assistance (`.claude/`) |
| [journal-du-corpus.md](journal-du-corpus.md) | Le journal des changements normatifs du corpus, alimenté par l'outillage |
| [tracabilite.md](tracabilite.md) | La matrice des use cases vers leurs stories, règles, cas de test, tâches, écrans et décisions, générée par l'outillage |

Le catalogue des événements du domaine, lui aussi alimenté par l'outillage, est dans [domaine/evenements.md](domaine/evenements.md).

---

## Les mots de la méthode

Les documents de conduite du projet emploient quelques termes propres :

- une **tranche** (`TB-nnn`) est une tâche du plan de travail ; une **épique** (`EP-nn`) regroupe des tranches ; un **lot** (`LOT-nn`) regroupe des tranches qu'on peut mener en parallèle ([plan de travail, § 1 et § 9](projet/plan-de-travail.md#1-règles-de-lecture)) ;
- **HORS MAILLE** marque une tranche qui ne touche aucun code (une décision, une convention) ; **TROU** marque une tranche dont le corpus ne nomme pas encore ce qu'elle touchera ([plan de travail, § 2](projet/plan-de-travail.md#2-natures-du-périmètre-décriture)) ;
- dans les documents, **DÉCIDÉ** (avec une date) clôt un point, **À TRANCHER** et **SOUS-SPÉCIFIÉ** signalent un point ouvert ([registre des marqueurs](conventions.md#registre-des-marqueurs)).

Les contrôles automatiques du corpus et les règles de contribution sont décrits dans [`CONTRIBUTING.md`](../CONTRIBUTING.md).

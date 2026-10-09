# Haversack

Haversack est un outil d'assistance au Maître du Jeu (MJ) pour la préparation et la conduite de parties de jeu de rôle ([docs/README.md](docs/README.md)). Ce dépôt porte sa conception, en français, et l'outillage C# qui la vérifie.

**Sommaire**

- [Ce qu'est Haversack](#ce-quest-haversack)
- [Où en est le projet](#où-en-est-le-projet)
- [Comment la documentation est organisée](#comment-la-documentation-est-organisée)
- [Reprendre le projet : par où commencer](#reprendre-le-projet--par-où-commencer)
- [Comment vérifier](#comment-vérifier)
- [Où trouver le reste à faire et les questions ouvertes](#où-trouver-le-reste-à-faire-et-les-questions-ouvertes)
- [Comment contribuer](#comment-contribuer)

---

## Ce qu'est Haversack

Un outil pour le MJ, pas une table virtuelle ni un éditeur générique : il prépare son contenu narratif, pilote ses sessions et choisit ce qu'il partage avec ses joueurs. Le problème, les personas, le positionnement et le raisonnement concurrentiel sont dans la [vision produit](docs/besoin/vision/vision.md).

Pour entrer dans le produit par ses termes, lisez le [glossaire](docs/glossaire.md), qui fixe le nom de chaque terme. Pour un trait structurant, l'usage sans compte puis avec un compte, lisez [UC-01](docs/besoin/cas-d-utilisation/UC-01-mode-local-sans-compte.md) et [UC-10](docs/besoin/cas-d-utilisation/UC-10-compte-cloud.md). Pour la pile technique et ses raisons, lisez [stack.md](docs/architecture/stack.md).

---

## Où en est le projet

Le projet est en phase de conception : la [note de cadrage](docs/projet/note-de-cadrage.md) en porte le statut. Le dépôt ne contient aucun code produit ; il contient le corpus de conception (`docs/`) et l'outillage qui le vérifie (`tools/`).

Les hypothèses que le MVP doit démontrer ne sont pas encore validées sur le terrain : aucun entretien n'a eu lieu ([guide d'entretien, § 8. Résultats](docs/besoin/decouverte/guide-d-entretien.md#8-résultats)).

Le build s'ouvre au jalon J0, point de départ de la séquence J0 → J3 : voir la [vue de synthèse de la roadmap de build](docs/projet/roadmap-de-build.md#2-vue-de-synthèse), puis le [détail par jalon](docs/projet/roadmap-de-build.md#3-détail-par-jalon) pour les préalables bloquants et les critères de sortie. Le [plan de travail](docs/projet/plan-de-travail.md) décompose cette séquence en tâches ; son en-tête dit jusqu'où.

L'histoire des changements du corpus se lit dans le [journal du corpus](docs/journal-du-corpus.md) et dans git, pas ici.

---

## Comment la documentation est organisée

Le corpus est dans `docs/`, un dossier par couche. La [carte de `docs/`](docs/README.md) donne une ligne par dossier ; chaque dossier a son propre index, qui fait foi sur son contenu.

Les couches vont du besoin vers sa réalisation : le [besoin](docs/besoin/README.md), le [domaine](docs/domaine/README.md), l'[interface](docs/interface/README.md), les [décisions](docs/decisions/README.md), l'[architecture](docs/architecture/README.md) et l'[assurance](docs/assurance/README.md) (sécurité, test, conformité). Deux dossiers consolident sans jamais faire foi : les [documents de référence](docs/reference/README.md), lisibles seuls, et la [gestion de projet](docs/projet/README.md).

Quand deux documents divergent, celui qui est listé le premier dans l'ordre d'autorité fait foi, et c'est l'autre qu'on corrige : l'ordre est dans les [conventions du corpus](docs/conventions.md#ordre-dautorité-entre-artefacts), sa décision dans l'[ADR-019, § 1](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité). En particulier, les use cases sont la source de vérité du besoin ; le glossaire fait référence pour les noms, pas pour le fond.

Le choix de cette arborescence est l'[ADR-020](docs/decisions/ADR-020-le-corpus-est-reorganise-selon-l-arborescence-un-dossier-par.md).

---

## Reprendre le projet : par où commencer

Chaque parcours se lit dans l'ordre.

### Écrire la première ligne de code (J0)

Pour le développeur ou la développeuse qui reprend le build.

1. [Vision produit](docs/besoin/vision/vision.md) : ce que le produit vise et ce qu'il refuse d'être, avant tout détail technique.
2. [Roadmap de build](docs/projet/roadmap-de-build.md) : les jalons, leurs préalables bloquants et leurs critères de sortie ; c'est par là que le build s'ouvre.
3. [Guide de conventions et définition de fini](docs/projet/conventions-dod.md) : les conventions de code, de commit et de branche, et ce qui rend une tâche terminée.
4. [Stack technologique](docs/architecture/stack.md) : la pile retenue et les raisons de chaque choix.
5. [Structure des projets](docs/architecture/structure-projets.md) : la structure de la solution .NET et l'ordre de construction.
6. [Registre des décisions](docs/decisions/README.md) : les arbitrages d'architecture et leurs alternatives écartées, à ouvrir quand une contrainte surprend.
7. [Plan de travail](docs/projet/plan-de-travail.md) : les tâches, leurs dépendances et leur périmètre d'écriture ; c'est là qu'on prend la prochaine.
8. [Guide de lecture par fonctionnalité](docs/projet/guide-de-lecture.md) : pour une fonctionnalité donnée, le chemin vers son besoin, sa maquette, ses décisions et sa spécification.

### Produit

Pour qui pilote le périmètre et les arbitrages.

1. [Vision produit](docs/besoin/vision/vision.md) : le problème, le positionnement et les choix assumés.
2. [Personas](docs/besoin/decouverte/personas/README.md) : pour qui l'on conçoit, et les profils qui servent à valider chaque choix.
3. [Matrice de priorités](docs/besoin/vision/perimetre/matrice-de-priorites.md) : la seule autorité sur ce qui est dans la première livraison et ce qui n'y est pas.
4. [Cas d'utilisation](docs/besoin/cas-d-utilisation/README.md) : la source de vérité du besoin fonctionnel, capacité par capacité ; leurs [diagrammes](docs/besoin/cas-d-utilisation/diagrammes.md) en donnent la vue d'ensemble.
5. [Parcours utilisateur](docs/besoin/parcours-utilisateur/README.md) : le vécu de chaque persona, étape par étape.
6. [Zonage de l'interface](docs/interface/zonage.md) : la navigation et l'inventaire des écrans qui en découlent.
7. [Roadmap produit](docs/projet/roadmap-produit.md) : la trajectoire de valeur, palier après palier.
8. [Décisions en attente](docs/projet/decisions-en-attente.md) : ce que le produit doit encore trancher, par décideur.

### QA

Pour qui construit la recette.

1. [Matrice de priorités](docs/besoin/vision/perimetre/matrice-de-priorites.md) : ce qui est à recetter dans la première livraison.
2. [Cas d'utilisation](docs/besoin/cas-d-utilisation/README.md) : ce que chaque capacité doit faire.
3. [User stories](docs/besoin/stories/README.md) : les scénarios Gherkin, qui sont l'unique porteur des critères d'acceptation, et les règles métier.
4. [Exigences non fonctionnelles](docs/besoin/nfr/README.md) : ce que l'utilisateur doit pouvoir ressentir et vérifier, hors fonctionnalités.
5. [Stratégie de test et cahier de recette](docs/assurance/strategie-de-test.md) : les niveaux de test et les cas de recette par use case.
6. [Traçabilité](docs/tracabilite.md) : pour chaque use case, ses stories, ses règles, ses cas de test et ses écrans.
7. [Dossier de sécurité](docs/assurance/securite/dossier-securite.md) : les menaces et contre-mesures à éprouver.
8. [Guide de lecture par fonctionnalité](docs/projet/guide-de-lecture.md) : tout ce qui touche une fonctionnalité, en un renvoi.

---

## Comment vérifier

Prérequis : le SDK .NET 10 (les projets de `tools/` ciblent `net10.0` ; la [CI](.github/workflows/controles-corpus.yml) installe `10.0.x`).

Depuis la racine du dépôt, lancez les commandes telles quelles, sans tube ni redirection, pour observer le vrai code de sortie :

```
dotnet test tools/Tests/CaracterisationOutils.csproj
dotnet run --project tools/VerifCorpus
dotnet run --project tools/VerifMailleAgregat
dotnet run --project tools/VerifMarqueursIdentifiants
dotnet run --project tools/VerifCouvertureRecette
```

- `tools/Tests` : les tests de caractérisation de l'outillage (golden masters), doublures dans `tools/Harnais`.
- `tools/VerifCorpus` : liens, ancres, orphelins et renvois « § ».
- `tools/VerifMailleAgregat` : la maille du plan de travail.
- `tools/VerifMarqueursIdentifiants` : les marqueurs hors de la liste fermée et les identifiants, comparés aux listes de référence versionnées à côté de l'outil.
- `tools/VerifCouvertureRecette` : la couverture des cas de recette par le plan. Il sort en code 1 par conception tant que des écarts sont assumés : comparez sa liste à `tools/VerifCouvertureRecette/ecarts-attendus.txt`, seul un écart nouveau est une régression.

Le sens de chaque sortie est dans le [guide de conventions, § 8.8](docs/projet/conventions-dod.md#88-conventions-de-tenue-du-corpus). La CI les rejoue sur toute modification de `docs/` ou de `tools/` : voir [controles-corpus.yml](.github/workflows/controles-corpus.yml).

`tools/CreerIssues` et `tools/CreerProjet` ne vérifient rien : ils reportent le plan de travail en issues et en projet GitHub, via [recalcul-suivi-projet.yml](.github/workflows/recalcul-suivi-projet.yml).

---

## Où trouver le reste à faire et les questions ouvertes

- Le reste à faire est dans le [plan de travail](docs/projet/plan-de-travail.md) (tâches TB-nnn), source de vérité des tâches ([ADR-019, § 7](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#7-reste-à-faire)). Le modèle de ticket et son cycle de vie sont dans la [méthode de ticket](docs/projet/methode-de-ticket.md).
- Les questions ouvertes sont dans [decisions-en-attente.md](docs/projet/decisions-en-attente.md), index unique, regroupé par décideur.
- Les risques sont dans le [registre des risques](docs/projet/registre-des-risques.md).

---

## Comment contribuer

- Les conventions de tenue du corpus (identifiants, gabarits, renvois, marqueurs, registre des décisions, langue…) sont dans le document des [conventions du corpus](docs/conventions.md) ; leur décision est l'[ADR-019](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md) et leur résumé le [guide de conventions, § 8.8](docs/projet/conventions-dod.md#88-conventions-de-tenue-du-corpus).
- Un fait a un seul propriétaire : on y renvoie par un lien relatif, on ne le recopie pas.
- La documentation est en français ; tout ce qui touche au code est en anglais ([ADR-019, § 9](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#9-langue)).
- Les messages de commit et les noms de branche suivent le [guide de conventions, § 5](docs/projet/conventions-dod.md#5-domaine-de-convention--conventions-de-commit-et-de-branche).
- Avant de commiter une modification de `docs/`, lancez les contrôles de la section [Comment vérifier](#comment-vérifier).

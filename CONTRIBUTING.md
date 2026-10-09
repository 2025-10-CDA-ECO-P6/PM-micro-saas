# Contribuer à Haversack

Ce dépôt contient la conception du produit, en français, et l'outillage C# qui la contrôle. Chaque règle ci-dessous renvoie à sa source ; en cas de doute, la source fait foi.

## Les règles

- **Un fait vit à un seul endroit.** Chaque information a un document propriétaire ; ailleurs, on y renvoie par un lien, on ne la recopie pas. Qui possède quoi : les [conventions du corpus](docs/conventions.md).
- **On corrige à la source.** Quand deux documents se contredisent, celui qui fait foi est désigné par l'[ordre d'autorité](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité) : on corrige l'autre. Si c'est la source qui est fausse, on la corrige d'abord, puis on aligne ceux qui la citent.
- **Langue** : voir l'[ADR-019, § 9](docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md#9-langue).
- **Commits et branches** : voir le [guide de conventions, § 5](docs/projet/conventions-dod.md#5-domaine-de-convention--conventions-de-commit-et-de-branche).

## Avant de commiter

Les contrôles tournent en intégration continue ([`controles-corpus.yml`](.github/workflows/controles-corpus.yml)) à chaque modification de `docs/` ou de `tools/`. Lancez-les aussi en local avant tout commit qui touche `docs/`. Il faut le [SDK .NET 10][dotnet], en version 10.0.300 au moins (fixée par `global.json`).

Depuis la racine du dépôt, lancez chaque commande telle quelle, sans tube ni redirection, pour lire son vrai code de sortie :

```bash
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

`VerifCouvertureRecette` sort toujours en code 1, parce que quelques écarts connus sont assumés. Ce n'est pas une erreur : comparez sa liste à [`ecarts-attendus.txt`](tools/VerifCouvertureRecette/ecarts-attendus.txt) ; seul un écart absent de ce fichier est une régression. De même, chaque outil range ses écarts acceptés dans un fichier de référence à côté de lui.

Le sens détaillé de chaque sortie est dans le [guide de conventions, § 8.8](docs/projet/conventions-dod.md#88-conventions-de-tenue-du-corpus).

`tools/CreerIssues` et `tools/CreerProjet` ne vérifient rien : ils reportent le plan de travail en issues et en tableau de projet GitHub, par le workflow [`recalcul-suivi-projet.yml`](.github/workflows/recalcul-suivi-projet.yml).

Les outils de `tools/` passent un à un en programmes C# d'un seul fichier, sans projet : leur forme suit la [convention des outils en fichier unique](tools/FICHIER-UNIQUE.md).

## Prévisualiser le site de documentation

La documentation est aussi publiée sous forme de site, construit avec MkDocs. Les fichiers qui le configurent, dont `docs-site/requirements.txt`, arrivent avec le site. Une fois présents, depuis la racine du dépôt :

```bash
pip install -r docs-site/requirements.txt
mkdocs serve
```

[dotnet]: https://dotnet.microsoft.com/download

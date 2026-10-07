# SPEC — deplacements

## Contrat

### Pourquoi

Toute opération suivante s'adresse aux chemins définitifs (ADR-021). La sélection et les déplacements forment un même commit (ADR-022, condition 2).

### État actuel

L'outillage code des chemins en dur : `tools/VerifMailleAgregat/Program.cs:19`, `tools/VerifMailleAgregat/ModeleDomaine.cs:23`, `tools/VerifCouvertureRecette/Program.cs:17`, `tools/CreerIssues/Chemins.cs:12`, `tools/CreerIssues/WireframeSlugs.cs:5`. Les golden masters portent sur le corpus réel (`tools/Tests/VerifCorpusTests.cs:36`).

### Ce qui doit être vrai à la fin

- Chaque ligne de la table a été appliquée par `git mv`, puis `conform.sh relink` a recalculé les liens et les chemins entre backticks.
- `docs/corpus.md` est réécrit avec `corpus.sh write-selection` à partir de `selection-cible.txt`.
- L'outillage C# et le workflow pointent vers les nouveaux chemins.
- Les règles `.claude/rules/corpus-*.md` sont régénérées.
- La différence des golden masters est exactement la substitution des chemins (ADR-022, condition 4).

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- `git mv` uniquement, pour que l'historique suive.
- Aucun titre de section ne change : les ancres restent valides.
- Chercher les anciens chemins aussi dans `*.cs`, `*.yml` et `.claude/rules/`.

### Hors contrat

- Le contenu des documents ne change pas (pas de gabarit, pas de correction de fond).
- `docs/project/backlog/` ne bouge pas.

### À lire avant de commencer

- `docs/workstreams/conformite-corpus/table-deplacement.tsv`
- `docs/workstreams/conformite-corpus/reference.md`
- ADR-022
- l'en-tête de `.claude/skills/onboard/conform.sh`

## Lots

- Lot 1 — fichiers: docs/**, tools/** — vérités: 1, 2, 3, 4
- Lot 2 — fichiers: .github/workflows/recalcul-suivi-projet.yml, README.md — vérités: 3

## Périmètre

- `docs/**`
- `README.md`
- `tools/**`
- `.github/workflows/recalcul-suivi-projet.yml`

## Revue

Le diff seul, puis le contrat : rien d'autre qu'un déplacement ?

## Vérités

1. Quand on lance VerifCorpus, le nombre de liens relatifs est celui de la référence, avec 0 lien mort, 0 ancre morte et 0 orphelin.
2. Quand on lance les tests de caractérisation, ils passent tous, et le diff des golden masters ne contient que des chemins remplacés selon la table.
3. Quand on cherche un ancien chemin de la table avec `git grep`, aucune occurrence ne subsiste hors de `docs/workstreams/` et des entrées datées du backlog.
4. Quand on lance `conform.sh verify <commit de départ>` avec les chemins réécrits déclarés, il sort en code 0.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `git grep -n -F -f <colonne ancien>` : aucune occurrence hors exceptions déclarées.

## Notes de dev



## Notes de revue




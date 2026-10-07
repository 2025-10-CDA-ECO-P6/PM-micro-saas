# SPEC — solde-du-backlog

## Contrat

### Pourquoi

Le plan de travail possède le reste à faire, et deux dossiers projet/ et project/ ne doivent pas coexister (ADR-022, condition 5).

### État actuel

Le backlog est indexé à `docs/project/backlog/README.md:1`. Le système code son emplacement en dur (`.claude/hooks/lib/corpus.sh:750`).

### Ce qui doit être vrai à la fin

- Chaque entrée « ouvert » devient une TB du plan, numérotée par la règle des identifiants, avec `origin: docs/project/backlog/<n> @ <commit>`.
- `docs/project/` est retiré par `git rm`.
- Les renvois vers le backlog sont redirigés vers le plan, ou vers la référence git du commit qui précède le retrait.
- Le défaut amont (chemin codé en dur) est consigné pour condor.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).

### Hors contrat

- L'application des entrées, déjà faite.

### À lire avant de commencer

- `docs/project/backlog/README.md`
- le plan de travail
- ADR-019 (§ Reste à faire)

## Périmètre

- `docs/project/**`
- `docs/projet/**`
- `docs/README.md`
- `README.md`

## Revue

Le contrat : rien de perdu, rien en double ?

- general-purpose

## Vérités

1. Quand on liste `docs/project/`, le dossier n'existe pas.
2. Quand on cherche `docs/project/backlog` dans le corpus, on ne le trouve que dans des références git datées.
3. Quand on lance VerifMailleAgregat, les nouvelles TB sont reconnues et le code est 0.
4. Quand on lance `conform.sh verify`, il sort en code 0 : chaque entrée « ouvert » se retrouve dans le plan.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.

## Notes de dev



## Notes de revue




# SPEC — doublons

## Contrat

### Pourquoi

Le glossaire fait référence pour les noms de termes, et un renommage est propagé partout (ADR-019, § Glossaire).

### État actuel

Le plan calculé relève 11 termes rejetés : « scenariolibrary » dans l'ADR-018 et la conception détaillée, « visible par les joueurs » dans NFR-CONF-01 et le zoning, « note privée mj » dans la stratégie de test (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`).

### Ce qui doit être vrai à la fin

- Chaque terme rejeté est remplacé par le terme du glossaire.
- Chaque définition présente à deux endroits n'en garde qu'un, avec `origin: <ancien chemin>:<ligne> @ <commit>`, et l'autre garde un renvoi.
- Deux versions qui diffèrent ne sont pas repliées : elles deviennent une question pour l'opérateur.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).

### Hors contrat

- Les gabarits et le backlog.

### À lire avant de commencer

- `docs/glossaire.md`
- les lignes F3 du plan calculé

## Périmètre

- `docs/**`

## Revue

Le diff : un seul propriétaire par définition, aucun fond changé ?

- general-purpose

## Vérités

1. Quand on cherche chaque terme rejeté dans le corpus, on ne le trouve plus qu'au glossaire, comme alias.
2. Quand on lance `conform.sh verify`, il sort en code 0, et chaque ligne manquante est déclarée comme repliée avec renvoi.
3. Quand on lance les quatre contrôles, ils donnent les compteurs de la référence.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.

## Notes de dev



## Notes de revue




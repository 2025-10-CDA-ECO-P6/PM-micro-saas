# SPEC — documents-generes

## Contrat

### Pourquoi

Les modules conventions, event-catalogue, traceability et corpus-changelog sont sélectionnés comme attendus (`docs/corpus.md`), et le socle exige conventions (ADR-019).

### État actuel

Les règles du corpus sont tranchées dans `docs/architecture/decisions/ADR-019-conventions-de-tenue-du-corpus.md:21` et résumées au guide de conventions (`docs/gestion-projet/guide-conventions-et-dod.md:298`). Aucun de ces quatre documents n'existe.

### Ce qui doit être vrai à la fin

- `docs/conventions.md` est écrit avec `/write conventions` à partir de l'ADR-019, avec ses régions générées (ordre d'autorité, registres d'identifiants et de marqueurs).
- Le catalogue d'événements et la traçabilité sont produits par `corpus.sh generate events|trace`.
- Le journal du corpus existe et `/write` l'alimente.
- Le § 8.8 du guide de conventions renvoie à `docs/conventions.md`.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Ne jamais éditer à la main une région générée.

### Hors contrat

- La mise en CI des contrôles.

### À lire avant de commencer

- ADR-019
- les recettes `conventions`, `event-catalogue`, `traceability`, `corpus-changelog` sous `.claude/corpus/modules/`

## Lots

- Lot 1 — fichiers: docs/domaine/**, docs/**/tracabilite* — vérités: 3
- Lot 2 — fichiers: docs/**/journal*, docs/projet/**, docs/conventions.md — vérités: 1, 2, 4

## Périmètre

- `docs/conventions.md`
- `docs/domaine/**`
- `docs/**/tracabilite*`
- `docs/**/journal*`
- `docs/projet/**`

## Revue

Le contrat : les documents générés reflètent-ils les sources, sans valeur inventée ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check conventions docs/conventions.md`, il sort `CLEAN`.
2. Quand on relance `corpus.sh render`, le diff est vide.
3. Quand on compare le catalogue d'événements aux tables d'événements des contextes, chaque événement y figure une fois.
4. Quand on lance VerifCorpus, on obtient 0 orphelin.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `bash .claude/skills/write/write.sh check <module> <fichier>` pour chacun des quatre modules

## Notes de dev



## Notes de revue




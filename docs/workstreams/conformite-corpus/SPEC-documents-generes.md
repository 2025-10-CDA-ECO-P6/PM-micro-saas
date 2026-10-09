# SPEC — documents-generes

## Contrat

### Pourquoi

Les modules conventions, event-catalogue, traceability et corpus-changelog sont sélectionnés comme attendus (`docs/corpus.md`), et le socle exige conventions (ADR-019).

### État actuel

Les règles du corpus sont tranchées dans `docs/decisions/ADR-019-conventions-de-tenue-du-corpus.md:21` et résumées au guide de conventions (`docs/projet/conventions-dod.md:298`). Aucun de ces quatre documents n'existe.

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
- `docs/corpus.md` (chemins des quatre modules, déclarés « - » jusqu'ici) et `docs/README.md` (index qui rend les nouveaux documents joignables) : ajoutés par l'orchestrateur, sur délégation de l'opérateur

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

- `docs/corpus.md` donne leur chemin aux quatre modules : `docs/conventions.md`, `docs/domaine/evenements.md`, `docs/tracabilite.md` et `docs/journal-du-corpus.md`.
- Le catalogue d'événements et la traçabilité sont produits par `corpus_generate` (events, trace). Le catalogue compte 39 événements, chacun une fois, soit exactement ceux des tables des contextes (vérité 3).
- `docs/conventions.md` est écrit à partir de l'ADR-019, et ses régions générées (ordre d'autorité, registres d'identifiants et de marqueurs) sont remplies par `corpus_render`. Un second rendu ne change rien (vérité 2).
- Le journal du corpus porte une entrée par document ajouté, écrite par `write.sh`.
- Le § 8.8 du guide de conventions renvoie à `docs/conventions.md`, et l'index `docs/README.md` rend les quatre documents joignables (0 orphelin).
- Contrôle `template` sur une racine minimale (les quatre documents et la sélection) : 0 écart, en 7 secondes. Les `write.sh check` de l'agent relançaient le contrôle global, de 11 minutes, pour finalement le sauter (« SKIPPED template — its report is capped »). Ils ont été arrêtés, avec les boucles d'attente que l'agent y avait accrochées : défaut de l'outillage, à reporter.
- `conform-verify-complet.sh 368e8c4 --declared docs/workstreams/conformite-corpus/declares-documents-generes.txt` : code 0.

## Notes de revue

Revue « les documents générés reflètent-ils les sources, sans valeur inventée ? », 2026-10-09 : six constats.

- `deferred` (outillage) : le registre des marqueurs généré liste `[RÉSOLU — <source>]` et omet la liste fermée de l'ADR-019 § 5 ; l'ordre d'autorité généré place des documents de pilotage avant les personas. Une région générée ne se corrige pas à la main, et l'opérateur a exclu tout chantier d'outillage. La prose de `conventions.md` dit que l'ADR-019 prévaut.
- `fix` : « le glossaire vient en dernier » est situé dans la chaîne du besoin, et « selon cet ordre » est retiré de l'attribution au § 11.
- `fix` : trois `SOUS-SPÉCIFIÉ` deviennent des renvois vers des contenus que le corpus porte déjà (ordre d'autorité, § Sens de dépendance ; guide de conventions, § 8.1 à 8.4 ; index de `docs/`). **Justification** (décision de l'orchestrateur, par délégation de l'opérateur) : la décision 12 réserve ce marqueur à un contenu écrit nulle part.
- `fix` : le guide de conventions dit « reprises en partie » et nomme les axes repris.
- `deferred` : la table du § 8.8 du guide de conventions nomme encore `context/` et `gestion-projet/`.

Répartition : 3 `fix`, 2 `deferred`.

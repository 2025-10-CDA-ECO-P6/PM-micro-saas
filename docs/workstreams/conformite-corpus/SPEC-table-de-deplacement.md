# SPEC — table-de-deplacement

## Contrat

### Pourquoi

Le plan calculé par `conform.sh` ne peut pas servir d'oracle : il envoie un README sur un autre, aplatit les fiches écran et ne déplace pas le domaine (ADR-022, condition 1 ; étude `etudes/01-revue-architecte-chemin.md`).

### État actuel

Le plan calculé est conservé dans `plan-conformite-calcule-2026-10-07.txt` : 148 déplacements et 31 cas écartés. Les fichiers du domaine sont rangés par type de diagramme (`docs/conception/domain/diagrams/mld/content-library.md:1`). Les fiches écran sont rangées par surface (`docs/conception/interface/wireframes/README.md:17`).

### Ce qui doit être vrai à la fin

- Une table `ancien<TAB>nouveau`, au format lu par `conform.sh relink`, couvre chaque fichier suivi de `docs/`.
- Le domaine est regroupé par contexte : `docs/domaine/<contexte>/` contient contexte, classes, flux, MCD et MLD.
- Les fiches écran gardent leur niveau surface.
- La méthode, les plans, les registres et les revues vont sous `docs/projet/`, conformément à l'ADR-020 et non aux valeurs par défaut du catalogue (`docs/pilotage/`).
- Une section de justification liste chaque écart avec `conform.sh plan --map`.
- La sélection cible (`docs/corpus.md`) est rédigée à côté, et non appliquée.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Les noms de fichiers suivent la langue du projet (ADR-020).
- Les identifiants dans les noms (UC-NN, ADR-NNN…) ne changent jamais.

### Hors contrat

- Aucun fichier n'est déplacé.
- `docs/corpus.md` n'est pas modifié.

### À lire avant de commencer

- ADR-020 et ADR-022
- `docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt`
- `.claude/corpus/labels` (les `path.*`)
- `bash .claude/skills/onboard/conform.sh plan --map`

## Périmètre

- `docs/workstreams/conformite-corpus/table-deplacement.tsv`
- `docs/workstreams/conformite-corpus/selection-cible.txt`

## Revue

Le contrat et l'ADR-020 : la table réalise-t-elle l'arborescence décidée ?

## Vérités

1. Quand on compare la colonne « ancien » à `git ls-files docs` (hors `docs/workstreams/` et `docs/project/`), aucun fichier ne manque.
2. Quand on trie la colonne « nouveau », aucune valeur n'apparaît deux fois, et aucune n'existe déjà dans le dépôt.
3. Quand on passe `selection-cible.txt` à `bash .claude/hooks/lib/corpus.sh resolve` dans une copie de travail, la résolution sort en code 0.
4. Quand on lit la justification, chaque ligne de `conform.sh plan --map` qui diffère de la table y est expliquée.

## Vérification

- un script `python3` de contrôle de la table (couverture, unicité, non-existence), joint à la tâche et rejouable
- `bash .claude/hooks/lib/corpus.sh resolve` sur la sélection cible

## Notes de dev



## Notes de revue




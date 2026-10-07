# SPEC — reprise-du-suivi-github

## Contrat

### Pourquoi

Le suivi est suspendu pendant le chantier, puis réactivé, et les issues sont mises à jour (ADR-022, condition 6).

### État actuel

Le workflow a été désactivé par la tâche « Référence de départ ». CreerIssues régénère le corps d'une issue quand il diverge (`.github/workflows/recalcul-suivi-projet.yml:24`).

### Ce qui doit être vrai à la fin

- Le workflow est réactivé.
- Une exécution en mode à blanc puis `--appliquer` a mis à jour les corps des issues.
- Les TB nouvelles du solde du backlog ont leur issue.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Lancer d'abord le mode à blanc et relire son compte rendu ; `--appliquer` publie sur GitHub, ce qui est une action visible de l'extérieur, à confirmer par l'opérateur.

### Hors contrat

- La modification du contenu des TB.

### À lire avant de commencer

- `docs/workstreams/conformite-corpus/reference.md`
- `tools/CreerIssues/`

## Périmètre

- `.github/workflows/recalcul-suivi-projet.yml`
- `docs/workstreams/conformite-corpus/reference.md`

## Revue

Le contrat : le suivi reflète-t-il le plan déplacé ?

## Vérités

1. Quand on ferme puis rouvre une issue de test, le workflow s'exécute.
2. Quand on ouvre une issue de TB, son lien vers le plan pointe vers le nouveau chemin et se résout.
3. Quand on relance CreerIssues en mode à blanc, il ne propose plus aucune modification de corps.

## Vérification

- `dotnet run --project tools/CreerIssues` (mode à blanc) : aucune divergence
- l'onglet Actions : une exécution réussie

## Notes de dev



## Notes de revue




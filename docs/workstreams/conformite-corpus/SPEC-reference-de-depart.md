# SPEC — reference-de-depart

## Contrat

### Pourquoi

Toutes les tâches suivantes se comparent à un état figé (ADR-022, condition 3). Le suivi GitHub est suspendu pendant le chantier (ADR-022, condition 6).

### État actuel

Aujourd'hui : 1 864 liens relatifs et 258 liens à ancre (sortie de `dotnet run --project tools/VerifCorpus` le 2026-10-07), 65 tests. Le workflow se déclenche sur les événements d'issues (`.github/workflows/recalcul-suivi-projet.yml:18`).

### Ce qui doit être vrai à la fin

- Un fichier `reference.md` dans le dossier du chantier consigne le commit de départ, la sortie complète des quatre contrôles et le nombre de liens relatifs.
- Le workflow `recalcul-suivi-projet` est désactivé, de façon réversible, et le fichier de référence indique comment le réactiver.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Désactiver le workflow sans supprimer son fichier.

### Hors contrat

- Aucun document du corpus n'est modifié.
- Les issues GitHub ne sont pas touchées.

### À lire avant de commencer

- `docs/workstreams/conformite-corpus/CADRAGE.md`
- `docs/architecture/decisions/ADR-022-le-chantier-de-conformite-s-execute-sous-six-conditions.md`
- `.github/workflows/recalcul-suivi-projet.yml`

## Périmètre

- `docs/workstreams/conformite-corpus/reference.md`
- `.github/workflows/recalcul-suivi-projet.yml`

## Revue

Le contrat seul : la référence est-elle rejouable par un tiers ?

## Vérités

1. Quand on lit `reference.md`, on y trouve un hash de commit qui existe dans l'historique, et le compteur de liens relatifs qu'imprime VerifCorpus à ce commit.
2. Quand on rejoue les quatre contrôles sur le commit de départ, leur sortie est identique à celle consignée.
3. Quand on ferme ou rouvre une issue, aucune exécution du workflow `recalcul-suivi-projet` n'apparaît dans l'onglet Actions.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `git show <commit de départ> --stat` : le commit existe.

## Notes de dev



## Notes de revue




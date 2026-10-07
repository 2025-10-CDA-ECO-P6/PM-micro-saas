# Référence de départ du chantier conformite-corpus

Cet état figé sert de point de comparaison à toutes les tâches suivantes (ADR-022, condition 3). Le suivi GitHub est suspendu pendant le chantier (ADR-022, condition 6) : voir [ADR-022](../../decisions/ADR-022-le-chantier-de-conformite-s-execute-sous-six-conditions.md).

## Commit de départ

`282e2b6b`, sur `chantier/conformite-corpus`, relevé le 2026-10-07 avec un arbre de travail propre :

```text
282e2b666219fcb0d83e323a7306c376488d275d docs(conformite-corpus): exécute le chantier sur une branche dédiée, fusionnée en un commit
```

**Liens relatifs : 1 897**, ce qu'imprime VerifCorpus à ce commit. Les 1 864 de la spec avaient été comptés avant les commits de cadrage du chantier.

## Rejouer

Depuis la racine du dépôt, sur un arbre propre :

```sh
git switch --detach 282e2b6b
dotnet test tools/Tests/CaracterisationOutils.csproj
dotnet run --project tools/VerifCorpus
dotnet run --project tools/VerifMailleAgregat
dotnet run --project tools/VerifCouvertureRecette
```

Ces commandes sont les quatre contrôles du [guide de conventions, § 8.8](../../projet/conventions-dod.md#88-conventions-de-tenue-du-corpus). Seules deux parties de la sortie varient d'un poste ou d'une exécution à l'autre : les chemins absolus de la compilation et la durée des tests (`Duration`). Tout le reste doit être identique.

## Sorties consignées

### `dotnet test tools/Tests/CaracterisationOutils.csproj` — code 0

Sortie complète, telle qu'imprimée sur le poste de relevé. Les lignes de compilation (`->`) peuvent apparaître dans un autre ordre, car les projets compilent en parallèle.

```text
  Determining projects to restore...
  All projects are up-to-date for restore.
  GardeIdentiteGh -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/Harnais/GardeIdentiteGh/bin/Debug/net10.0/GardeIdentiteGh.dll
  FauxGh -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/Harnais/FauxGh/bin/Debug/net10.0/gh.dll
  VerifCorpus -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/VerifCorpus/bin/Debug/net10.0/VerifCorpus.dll
  VerifMailleAgregat -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/VerifMailleAgregat/bin/Debug/net10.0/VerifMailleAgregat.dll
  VerifCouvertureRecette -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/VerifCouvertureRecette/bin/Debug/net10.0/VerifCouvertureRecette.dll
  CreerProjet -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/CreerProjet/bin/Debug/net10.0/CreerProjet.dll
  CreerIssues -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/CreerIssues/bin/Debug/net10.0/CreerIssues.dll
  CaracterisationOutils -> /home/pierre-marie/code/Perso/Projects/Haversack/tools/Tests/bin/Debug/net10.0/CaracterisationOutils.dll
Test run for /home/pierre-marie/code/Perso/Projects/Haversack/tools/Tests/bin/Debug/net10.0/CaracterisationOutils.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    65, Skipped:     0, Total:    65, Duration: 2 s - CaracterisationOutils.dll (net10.0)
```

### `dotnet run --project tools/VerifCorpus` — code 0

```text
fichiers markdown             : 333  (sous docs/ : 332)
liens relatifs                : 1897
liens portant une ancre       : 258
liens à cible inexistante     : 0
liens à ancre inexistante     : 0
fichiers orphelins            : 0
blocs de code déséquilibrés   : 0
```

### `dotnet run --project tools/VerifMailleAgregat` — code 0

```text
agrégats extraits du modèle de domaine (8) :
    Document             ← content-library.md
    DocumentType         ← content-library.md
    Folder               ← content-library.md
    GuestAccess          ← space-management.md
    Session              ← session-conduct.md
    SessionViewConfig    ← session-conduct.md
    Space                ← space-management.md
    User                 ← identity-access.md
fiches d'écran indexées      : 20
object stores indexés        : 32

tâches du plan                        : 61
  périmètre entièrement résolu        : 55
  périmètre HORS MAILLE (exclu)       : 4
  périmètre TROU (non nommable)       : 1
  périmètre en attente de J0 (rôle)   : 1
  item ne se résolvant pas la maille  : 0
    [EN ATTENTE J0] TB-002 -> ["les autres projets d'Infrastructure et de Présentation, dont le nombre et les noms relèvent de J0"]

écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0

identifiants à la fois fiche vivante et retirés (intersection)     : 0
renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : 0
```

### `dotnet run --project tools/VerifCouvertureRecette` — code 1

```text
cahier de recette (§10) — cas de recette réels extraits : 257

classement des absences face au plan de travail :
  RETIRÉ (le cahier le marque mort, ne pèse pas)  : 2
    [RETIRÉ] CR-UC01-04 -> *(retiré)* — aucun plafond de création n'existe en mode local (décision produit, `RB-01-03` retirée) ; la seule contrainte du mode local est la capacité de stockage du navigateur, couverte par CR-UC01-14
    [RETIRÉ] CR-UC02-11 -> *(retiré)* — aucun plafond de création n'existe en mode local (décision produit, `RB-01-03` et `RB-02-11` retirées)
  CITÉ                                           : 134
  ABSENT — JALON ULTÉRIEUR (J2/J3, légitime)     : 85
  ABSENT — HORS MVP (use case hors §4, légitime) : 16
  SYNTHÈSE (second niveau, cite d'autres CR-)    : 18
  ABSENT — TROU (ni cité, ni légitime)           : 2
    [TROU] CR-UC01-07 (US-UC-01, jalon J1) -> cité par aucune tranche du plan
        Source (cahier) : US-01-04 §"Le MJ comprend ce qui est accessible sans compte"
    [TROU] CR-UC01-20 (US-UC-01, jalon J1) -> cité par aucune tranche du plan
        Source (cahier) : RB-01-15 (dérivé, pas de Gherkin)

limite assumée : le jalon ci-dessus est dérivé au grain du use case (§4), faute d'un
champ de jalon au grain de la story dans le plan — un trou dont la Source nomme une
story dépendant elle-même d'un use case de jalon ultérieur demande une adjudication
humaine, pas un classement automatique supplémentaire.

cahier §13 — contrôle d'intégrité des citations : sources déclarées retirées : 2
    [SOURCE RETIRÉE] CR-UC01-04 -> US-01-01 §RB-01-03 (retirée)
    [SOURCE RETIRÉE] CR-UC02-11 -> US-01-01 §RB-01-03 (retirée)
```

Ce code 1 est voulu : il signale des écarts assumés. La liste de référence contient deux `TROU` (CR-UC01-07, CR-UC01-20) et deux `SOURCE RETIRÉE` (CR-UC01-04, CR-UC02-11). Seul un écart qui n'est pas dans cette liste est une régression.

## Suspension du workflow `recalcul-suivi-projet`

Le workflow est désactivé côté GitHub. Son fichier, `.github/workflows/recalcul-suivi-projet.yml`, n'est pas modifié. Il se déclenche sur des événements d'issues, et GitHub lit alors le fichier de la branche par défaut : modifier le fichier sur la branche du chantier ne l'aurait donc pas arrêté.

- Désactivation, le 2026-10-07 : `gh workflow disable recalcul-suivi-projet.yml`
- Constat : `gh workflow list --all` affiche `Recalcul du suivi de projet	disabled_manually	350014262`. Le compteur était alors de 9 exécutions (`gh workflow view recalcul-suivi-projet.yml`).
- **Réactivation**, par la tâche `reprise-du-suivi-github` : `gh workflow enable recalcul-suivi-projet.yml`, puis `gh workflow list --all` doit afficher `active`.

Tant que le workflow est suspendu, ce compteur de 9 ne doit pas augmenter quand une issue est fermée ou rouverte.

## Réépinglages du commit de départ

Après chaque tâche « backlog », le commit de départ de `conform-verify` passe au commit de cette tâche (ADR-022, condition 3), car c'est elle qui change le fond. Un commit ne peut pas contenir son propre hash : la tâche suivante inscrit donc le hash de la précédente.

| Après la tâche | Commit de départ | Hash |
|---|---|---|
| `backlog-besoin` | le commit « docs(conformite-corpus): applique le backlog de la couche besoin » | `e99aa88` |
| `backlog-domaine` | le commit « docs(conformite-corpus): applique le backlog de la couche domaine » | `dca4540` |
| `backlog-interface` | le commit « docs(conformite-corpus): applique le backlog de la couche interface » | inscrit par la tâche `gabarits-decisions-architecture-assurance` |

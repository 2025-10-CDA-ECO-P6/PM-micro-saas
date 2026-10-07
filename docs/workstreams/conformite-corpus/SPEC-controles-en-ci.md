# SPEC — controles-en-ci

## Contrat

### Pourquoi

Les quatre contrôles sont lancés en CI sur toute modification de `docs/` (ADR-019, § Vérification) ; les marqueurs forment une liste fermée et un identifiant n'a qu'une origine (ADR-019).

### État actuel

Le seul workflow existant lance CreerIssues et CreerProjet (`.github/workflows/recalcul-suivi-projet.yml:155`). VerifCorpus ne contrôle que les ancres `#`, pas les renvois « § » (`tools/VerifCorpus/VerificateurCorpus.cs:9`).

### Ce qui doit être vrai à la fin

- Un workflow déclenché sur les modifications de `docs/**` et `tools/**` lance les quatre contrôles et échoue sur une régression par rapport à la liste attendue de VerifCouvertureRecette.
- Un contrôle signale tout marqueur hors de la liste fermée de l'ADR-019.
- Un contrôle signale tout identifiant défini deux fois.
- VerifCorpus contrôle aussi les renvois « § section ».
- Chaque nouveau contrôle a ses tests de caractérisation, dont un défaut injecté qu'il doit trouver.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Valider chaque nouveau contrôle contre un défaut injecté ET contre un cas légitime voisin (faux positif).

### Hors contrat

- Corriger les findings existants : ils sont listés, puis traités par une tâche de suivi.

### À lire avant de commencer

- ADR-019
- `tools/VerifCorpus/VerificateurCorpus.cs`
- `tools/Tests/`

## Lots

- Lot 1 — fichiers: .github/workflows/**, tools/** — vérités: 1, 2, 3, 4, 5

## Périmètre

- `.github/workflows/**`
- `tools/**`

## Revue

Le contrat seul : chaque contrôle peut-il échouer ?

## Vérités

1. Quand une branche modifie `docs/` et casse un lien, le workflow échoue et nomme le lien.
2. Quand un document porte « RÉSOLU » comme marqueur de statut, le contrôle des marqueurs le signale, avec fichier et ligne.
3. Quand deux documents définissent le même RB-NN-NN, le contrôle des identifiants le signale, avec les deux emplacements.
4. Quand un renvoi « § » vise une section absente, VerifCorpus le signale.
5. Quand on lance les tests de caractérisation, ils passent tous, nouveaux tests compris.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- une exécution du workflow sur une branche de test, avec un défaut injecté

## Notes de dev



## Notes de revue




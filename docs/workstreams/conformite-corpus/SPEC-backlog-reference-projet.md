# SPEC — backlog-reference-projet

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 18 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

### Ce qui doit être vrai à la fin

- Chaque entrée appliquée a modifié exactement ce qu'elle décrit, dans les documents qui font foi selon l'ordre d'autorité.
- Chaque entrée appliquée passe au statut « fait », avec le hash du commit.
- Une entrée qui ne peut pas être appliquée telle quelle reste « ouvert » avec la raison, et devient une question pour l'opérateur.
- Les documents aval qui citaient l'ancien texte sont alignés (`write.sh consumers`).
- `reference.md` porte le nouveau commit de départ.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Appliquer la règle de l'ADR-019 : la correction se fait d'abord à la source (ordre d'autorité), puis dans les consommateurs.
- Après chaque réécriture, chercher la chaîne citée dans tout le corpus (guide de conventions, § 8.1).
- Une entrée qui touche une autre couche est appliquée en entier ici, et le rapport le dit.

### Hors contrat

- Les gabarits, déjà faits.
- Les entrées des autres couches.
- Toute décision nouvelle : une contradiction nouvelle est remontée à l'opérateur, pas tranchée.

### À lire avant de commencer

- les 18 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Lots

- Lot 1 — fichiers: docs/reference/**, docs/projet/** — vérités: 2, 3
- Lot 2 — fichiers: docs/project/backlog/**, docs/README.md, README.md, docs/workstreams/conformite-corpus/reference.md — vérités: 1, 4

## Périmètre

- `docs/reference/**`
- `docs/projet/**`
- `docs/README.md`
- `README.md`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`

## Revue

Le contrat et les entrées : chaque modification correspond-elle à une entrée, et une seule ?

## Vérités

1. Quand on ouvre chaque entrée de la couche, son statut est « fait » avec un hash, ou « ouvert » avec une raison.
2. Quand on cherche le texte qu'une entrée « fait » demandait de retirer, on ne le trouve plus dans la couche ni dans ses consommateurs.
3. Quand on lance les quatre contrôles, ils donnent les compteurs de la référence, à l'exception des écarts que les entrées appliquées expliquent et que le rapport liste.
4. Quand on lit `reference.md`, le commit de départ est celui de cette tâche.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- pour chaque entrée « fait » : `git grep` de la chaîne retirée, sans résultat

## Notes de dev



## Notes de revue




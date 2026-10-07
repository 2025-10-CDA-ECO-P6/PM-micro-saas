# Reprise du chantier conformite-corpus

État au 2026-10-07, à la fin de la tâche 7. Ce document sert à reprendre le chantier dans une nouvelle session, où un orchestrateur exécute toutes les tâches restantes l'une après l'autre.

## Pour reprendre

```sh
git switch chantier/conformite-corpus
claude -n conformite-corpus-reprise
```

Prompt de départ :

> Reprends le chantier conformite-corpus en orchestrateur, à la tâche 8 (`backlog-domaine`). Lis d'abord `docs/workstreams/conformite-corpus/reprise/REPRISE.md`, puis `TACHES.md`. Une tâche, un commit, sur `chantier/conformite-corpus`.

## Où on en est

| # | Tâche | Commit | État |
|---|---|---|---|
| 1 | reference-de-depart | `97102e3` | close |
| 2 | table-de-deplacement | `d736d01` | close |
| 3 | deplacements | `8580b16` | close |
| 4 | identifiants | `939821c` | close |
| 5 | gabarits-besoin | `5071d48` | close |
| 6 | backlog-besoin | `e99aa88` | close |
| 7 | gabarits-domaine | `e1a54a0` | close |
| 8 à 24 | — | — | à faire, dans l'ordre de [TACHES.md](../TACHES.md) |

- **Commit de départ de `conform-verify`** : `e99aa88`. Il sera réépinglé sur le commit de la tâche 8, la prochaine tâche backlog. Voir la table de [reference.md](../reference.md).
- **Workflow GitHub `recalcul-suivi-projet`** : désactivé par `gh workflow disable` ; son fichier n'est pas modifié. La tâche 23 le réactive.

## Décisions de l'opérateur

Ces décisions valent pour le reste du chantier.

- **Exécution** : tout le chantier se fait dans une seule session orchestratrice, et non une session par tâche comme le disait le cadrage.
- **Outillage** : l'opérateur n'est pas satisfait de l'outillage `.claude/` (close, gardes, contrôles) et le refera dans un cadrage séparé. Pendant le chantier, on ne s'y applique que des contournements minimaux, et on n'ouvre aucun chantier d'outillage.
- **Gabarits** :
  - l'élision « Étant donné qu'un… » est gardée ; le contrôle `template` la refuse à tort, et 85 étapes restent signalées pour cette seule raison ;
  - un élément requis par le gabarit, dont le contenu n'existe nulle part, reçoit le seul marqueur `SOUS-SPÉCIFIÉ` ;
  - un qualificatif exigé par la recette et posé par jugement sur un texte intact (par exemple nominal ou limite) est accepté ;
  - les conversions de forme sont déclarées à `verify`.
- **Domaine** : les fichiers de chaque contexte gardent les noms reconnus par l'outillage (`context.md`, `flows.md`…). Voir la règle R10 de la table.
- **Rangement** : `securite/` et `ecoconception/` vont sous `docs/assurance/`, et `deploiement/` sous `docs/architecture/`. `docs/conception/README.md` devient `docs/reference/ordre-d-autorite.md`.
- **Identifiants** : RB-09-18 à RB-09-22 sont définies dans US-UC-09, avec le texte d'UC-09 ; UC-09 y renvoie.
- **Produit** :
  - au MVP, seul l'OWNER gère les membres (UC-11), et le droit du GM vaut à l'activation du rôle, hors MVP ;
  - le message unique d'un lien inutilisable est « Ce lien n'est pas utilisable ».
- **Hash d'un commit de backlog** : un commit ne peut pas porter son propre hash. Les entrées « fait » désignent donc le commit de leur tâche, et la tâche suivante inscrit son hash dans la table de réépinglage de [reference.md](../reference.md). L'opérateur peut encore remettre cette règle en cause.

## La méthode qui marche

### Pour chaque tâche

1. `bash .claude/skills/frame/frame.sh task <tâche>`, puis `frame.sh perimeter <globs> --workstream conformite-corpus`. Les globs sous `docs/workstreams/` sont refusés, car exemptés.
2. `bash .claude/skills/brief/brief.sh dev > <scratchpad>/dev-tN.txt`, puis le transmettre aux agents par son chemin.
3. Faire exécuter la tâche par des agents `dev`, en parallèle sur des fichiers disjoints quand elle est grosse.
4. Vérifier soi-même : `verify`, puis les quatre contrôles. `dotnet test` se lance seul, sans tube ni `&&` : sinon il ne compte pas pour `close.sh`.
5. Revue : `brief.sh review --diff HEAD`, transmise à un agent `general-purpose`. Au-delà d'environ 150 Ko, passer le diff au grain du mot (`git diff --word-diff=plain`, lignes changées seulement).
6. Trier les constats en `fix`, `intent`, `deferred`, rejeté ou optionnel, et les consigner dans les notes de dev et de revue de la spec.
7. Commiter avec `git commit --only <fichiers>`, sans trailer Co-Authored-By, puis `bash .claude/skills/close/close.sh --task --base <commit précédent>` (code 0 attendu).

### Le parallèle

Les worktrees d'isolation (`isolation: worktree`) partent de `main` et non de la branche du chantier : ils sont inutilisables ici. Créez vos propres worktrees détachés dans le scratchpad :

```sh
git worktree add --detach <scratchpad>/wt-NX <commit>
```

`.claude/` est ignoré par git : il est donc absent des worktrees, et les agents lisent les recettes par chemin absolu dans le dépôt principal. Rassemblez les diffs avec `git apply --3way` ; il exige un index propre, donc lancez d'abord `git add -A docs`, puis `git reset` après la fusion. Retirez les worktrees à la fin.

### Les contrôles lents

- Le contrôle `template` global prend environ 11 minutes et n'imprime que 30 écarts par catégorie. `write.sh check` le relance à chaque appel : ne l'utilisez pas fichier par fichier. Lancez la copie sans coupe :

  ```sh
  bash docs/workstreams/conformite-corpus/reprise/generer-outils-complets.sh <scratchpad>
  bash <scratchpad>/corpus-checks-complet.sh template --root "$PWD" > <scratchpad>/template.txt
  ```

  Un agent se vérifie plus vite sur une racine minimale : `corpus-checks.sh template --root <dossier ne contenant que ses fichiers, plus docs/corpus.md>`.
- `conform.sh verify` n'imprime que 20 lignes perdues. La copie `conform-verify-complet.sh <commit> --declared <fichier>` les imprime toutes.

### Les consignes types

[consignes-gabarits.txt](consignes-gabarits.txt) et [consignes-backlog.txt](consignes-backlog.txt) sont les consignes communes données aux agents. Les décisions 11 à 15 de la première portent celles de l'opérateur. Avant de les réutiliser, mettez à jour le commit de base et les chemins du scratchpad.

## Pièges rencontrés

- Le premier lancement d'un agent sur une grosse tâche backlog atteint la limite de 80 tours : relancez-le par `SendMessage`, avec la consigne de garder 5 tours pour son rapport.
- Les agents posent parfois une valeur de fond au détour d'un champ du gabarit (une priorité MoSCoW, un remède) : la revue les attrape. Cherchez en priorité « texte de fond modifié, perdu ou inventé ».
- `relink` fait pointer un lien vers un dossier sur son README. Quand ce README a été renommé (par exemple `conception/README.md` devenu `ordre-d-autorite.md`), le lien est juste techniquement mais faux sur le fond.
- Un `pkill -f <motif>` tue le shell qui le lance si sa propre ligne contient le motif.
- Les entrées du backlog citent les anciens chemins et numéros de ligne : retrouvez chaque endroit par son texte, et son chemin par la table de déplacement.

## Restes connus

- **Tâche 8 (backlog-domaine)** : `docs/domaine/content-library/context.md` (règles 2 et 3, `CanBeReadBy`) contredit l'exception de RB-12-01, qui rend la fiche du personnage associé lisible par son joueur (entrée 0024). Le domaine est en aval : c'est lui qui s'aligne.
- **Ambigus non tranchés** : les « notes partagées » du membre (RB-11-13, RB-11-21, UC-11).
- **À reprendre sur le fond** : les « Transitions inter-UC » des UJ-04 à 06, à migrer vers les parcours bout-en-bout ; les opportunités des UJ-08 à 14, à rapprocher de leurs frictions.
- **Tâche 15 (doublons)** : RB-06-26 double RB-06-25.
- **Bloqués** : la tâche 20 (comptes rendus d'entretien) attend un entretien réel. La tâche 19 demande un entretien avec l'opérateur. La tâche 23 publie sur GitHub et demande sa confirmation.
- **Défauts de l'outillage à reporter dans son futur cadrage** :
  - le contrôle `template` est global, lent, coupé à 30 lignes, et refuse l'élision ;
  - `verify` est coupé à 20 lignes ;
  - le motif de comptage des tests était sensible à la casse (corrigé dans `state/`) ;
  - les angles de revue des specs étaient en prose (une puce `- general-purpose` a été ajoutée) ;
  - les worktrees d'isolation partent de `main` ;
  - `conform.sh plan` ignore les chemins de la sélection (`security-dossier`) ;
  - `write.sh consumers` n'existe pas.

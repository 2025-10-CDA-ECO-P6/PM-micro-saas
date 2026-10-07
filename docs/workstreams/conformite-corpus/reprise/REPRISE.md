# Reprise du chantier conformite-corpus

État au 2026-10-07, à la fin de la tâche 14. Ce document sert à reprendre le chantier dans une nouvelle session, où un orchestrateur exécute toutes les tâches restantes l'une après l'autre.

## Pour reprendre

```sh
git switch chantier/conformite-corpus
claude -n conformite-corpus-reprise
```

Prompt de départ :

> Reprends le chantier conformite-corpus en orchestrateur, à la tâche 15 (`doublons`). Lis d'abord `docs/workstreams/conformite-corpus/reprise/REPRISE.md`, puis `TACHES.md`. Une tâche, un commit, sur `chantier/conformite-corpus`.

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
| 8 | backlog-domaine | `dca4540` | close |
| 9 | gabarits-interface | `f23dd91` | close |
| 10 | backlog-interface | `4ce7faa` | close |
| 11 | gabarits-decisions-architecture-assurance | `110c0b8` | close |
| 12 | backlog-decisions-architecture-assurance | `3037a00` | close |
| 13 | gabarits-reference-projet | `be6c45b` | close |
| 14 | backlog-reference-projet | `775a1db` | close |
| 15 à 18, 21 à 24 | — | — | à faire, dans l'ordre de [TACHES.md](../TACHES.md) |
| 19, 20 | — | — | **reportées** hors du chantier (décision de l'opérateur, voir plus bas) |

- **Commit de départ de `conform-verify`** : `775a1db` (tâche 14). La tâche 15 inscrit ce hash dans la ligne `backlog-reference-projet` de la table de réépinglage. Voir la table de [reference.md](../reference.md).
- **Workflow GitHub `recalcul-suivi-projet`** : désactivé par `gh workflow disable` ; son fichier n'est pas modifié. La tâche 23 le réactive.

## Décisions de l'opérateur

Ces décisions valent pour le reste du chantier.

- **Autonomie, 2026-10-07** : « si il y a un choix à faire je te laisse le prendre mais il doit être justifié ». Chaque choix pris par l'orchestrateur est écrit, avec sa justification, dans les notes de dev de la spec de sa tâche.
- **Périmètre** : une entrée qui touche une autre couche s'applique en entier, et ses consommateurs s'alignent. Le périmètre de la spec est élargi aux seuls fichiers qu'elle nomme, et l'élargissement est écrit dans la spec (tâches 8 et 10).
- **Domaine, 2026-10-07** : la garde de session `LIVE` avant `Archive()` passe par un contrat applicatif ; l'exception de RB-12-01 est limitée aux fiches `GM_ONLY` ou `PUBLIC` ; `Folder` porte un rôle système `NOTES`.

- **Exécution** : tout le chantier se fait dans une seule session orchestratrice, et non une session par tâche comme le disait le cadrage.
- **Entretiens, 2026-10-07** : les tâches 19 (`write-operating-model-et-content-guide`) et 20 (`write-interview-reports`) sont reportées hors du chantier, car aucun entretien n'aura lieu. Elles ne sont pas émulées par des agents IA : des entretiens simulés produiraient des verbatims et des validations d'hypothèses inventés, et un modèle de fonctionnement écrit sans l'opérateur inventerait ses choix. Les modules `interview-reports`, `operating-model` et `content-guide` restent déclarés « attendus, non écrits » (`-`) dans la sélection. Au moment de la tâche 21 (solde du backlog), ces deux tâches deviennent des TB du plan de travail. La tâche 24 liste alors 21 tâches commitées, et non 23, en nommant les deux tâches reportées.
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

- **Différés des tâches 11 à 14** :
  - **à la tâche 15 (doublons)** : les critères de sortie par jalon, portés à la fois par la roadmap de build, le §9 de la stratégie de test et la section « Jalons » de la note de cadrage ; « campagne par campagne » (parcours-01, parcours-02, RB-01-11) ;
  - **à la conversion des marqueurs** : des `RÉSOLU`, `TRANCHÉ` et « (résolu) » préexistants (PO-04 et PO-06 de la stratégie de test, US-UC-01, D-01 du registre, titres du §9 du zonage), plus le marqueur du reclaim-in-place (« à ratifier produit » contre « PRODUIT ») ;
  - **écarts de gabarit assumés** (le contrôle les signale ; justification dans les notes de dev des specs 11 et 13) : le §9 de la stratégie de test, le champ Sources d'UC-15, les fiches TB du plan (tableaux lus par l'outillage), les 7 colonnes du registre, les nœuds de décision de la roadmap, la Version de la note business ;
  - divers : les sous-titres « ### §N — … » du §11 du cahier des spécifications ; « ordre de build C#-first » (cadrage juridique, cahier des spécifications) ; l'outil d'intégrité des citations Gherkin que décrit le §13 de la stratégie de test, à prévoir à la tâche 22.
- **Différés des tâches 8 à 10**, à reprendre au solde du backlog (tâche 21) ou à remonter à l'opérateur :
  - `space-management/context.md`, consommateur de `CharacterAssociated` : « Session Conduct (accès aux notes PLAYER_PRIVATE du personnage) » contredit l'invariant 6 de Content Library ;
  - « Non classés » : UC-02 le dit non renommable, AR-20 et les fiches le disent renommable ; UC-05 parle encore d'une « vue dédiée », close par le zonage ;
  - UC-06 (épinglage automatique sans distinguer la note de session) et parcours-02, en retard sur RB-06-17 et RB-07-04 ;
  - les six contradictions entre fiches d'écran listées dans l'entrée 0002 ; ce que voit un joueur d'un espace ARCHIVED (0018, SOUS-SPÉCIFIÉ) ;
  - les maquettes `acces-lien-saisie-nom.dc.html` et `parametres-campagne.dc.html` divergent de leurs fiches, qui prévalent ;
  - les anciens noms de fichiers cités en prose (`zoning.md`, `conventions-wireframe.md`, `reflexion-ux-mvp.md`…, une centaine d'occurrences) ; `decisions-en-attente.md:146` cite l'ancien titre de S10 ; « Vue de l'espace personnel » dans `cahier-des-charges.md` ;
  - les surfaces SV1 à SV5 sont déclarées dans le README des écrans, et non dans le zonage comme le veut la recette ; trois marqueurs hors liste dans le zonage ; des titres de `raisonnement-ux.md` portent un compte.
- **Ambigus non tranchés** : les « notes partagées » du membre (RB-11-13, RB-11-21, UC-11).
- **À reprendre sur le fond** : les « Transitions inter-UC » des UJ-04 à 06, à migrer vers les parcours bout-en-bout ; les opportunités des UJ-08 à 14, à rapprocher de leurs frictions.
- **Tâche 15 (doublons)** : RB-06-26 double RB-06-25.
- **Reportées** : les tâches 19 et 20 (voir les décisions). **À confirmer** : la tâche 23 publie sur GitHub et demande l'accord de l'opérateur.
- **Défauts de l'outillage à reporter dans son futur cadrage** :
  - le module `screen-sheets` n'a pas de lecteur dans le contrôle `template`, et sa recette ignore la section « Mode local » que déclare le gabarit d'écran du projet ;
  - les libellés de l'outillage donnent `[RÉSOLU — source]`, alors que l'ADR-019 veut `DÉCIDÉ` avec date et renvoi ;
  - le contrôle `template` n'accepte qu'un numéro « N. » devant un titre, si bien que les repères S1 à S10 du zonage vivent dans une phrase du préambule ;
  - le contrôle refuse `SOUS-SPÉCIFIÉ` dans un champ à format imposé (Date, Version) et attend des champs « **Libellé** : » là où l'outillage du projet lit des tableaux (fiches TB) ;
  - `VueDesEpiques` (VerifCouvertureRecette) borne sa lecture sur un titre « ## 5. » : tout changement de niveau de titre du plan déplace silencieusement son périmètre ;
  - le contrôle `template` est global, lent, coupé à 30 lignes, et refuse l'élision ;
  - `verify` est coupé à 20 lignes ;
  - le motif de comptage des tests était sensible à la casse (corrigé dans `state/`) ;
  - les angles de revue des specs étaient en prose (une puce `- general-purpose` a été ajoutée) ;
  - les worktrees d'isolation partent de `main` ;
  - `conform.sh plan` ignore les chemins de la sélection (`security-dossier`) ;
  - `write.sh consumers` n'existe pas.

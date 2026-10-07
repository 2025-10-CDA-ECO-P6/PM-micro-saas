# Tâches — Mise en conformité du corpus

Ordre imposé par l'ADR-021 et l'ADR-022 : chaque tâche est un commit, exécuté dans sa propre session (`claude -n conformite-corpus-<tâche>`).

## 1. reference-de-depart

Référence de départ.

**Figé si** : un commit contient `docs/workstreams/conformite-corpus/reference.md` avec le commit de départ et les compteurs des quatre contrôles, et le workflow `recalcul-suivi-projet` ne se déclenche plus.

Spec : [SPEC-reference-de-depart.md](SPEC-reference-de-depart.md)

## 2. table-de-deplacement

Table de déplacement.

**Figé si** : le fichier `table-deplacement.tsv` du chantier associe une cible à chaque fichier suivi de `docs/` (hors `docs/workstreams/` et `docs/project/`), sans deux sources vers une même cible ni cible déjà existante, et chaque écart avec `conform.sh plan --map` est justifié.

Spec : [SPEC-table-de-deplacement.md](SPEC-table-de-deplacement.md)

## 3. deplacements

Déplacements (F1).

**Figé si** : le corpus est à l'arborescence de l'ADR-020, la sélection pointe vers les chemins cibles, et les quatre contrôles donnent les compteurs de la référence (même nombre de liens relatifs), dans un seul commit.

Spec : [SPEC-deplacements.md](SPEC-deplacements.md)

## 4. identifiants

Identifiants (F4).

**Figé si** : aucun identifiant n'est défini deux fois dans le corpus, et le second porteur de RB-05-01 a un identifiant neuf, avec ses citations redirigées.

Spec : [SPEC-identifiants.md](SPEC-identifiants.md)

## 5. gabarits-besoin

Gabarits — la couche besoin (F2).

**Figé si** : chaque document de la couche besoin passe `bash .claude/skills/write/write.sh check <module> <fichier>` sans finding de gabarit, et `conform.sh verify` sort en code 0 avec les seuls titres renommés et mots-clés traduits déclarés.

Spec : [SPEC-gabarits-besoin.md](SPEC-gabarits-besoin.md)

## 6. backlog-besoin

Backlog — la couche besoin.

**Figé si** : les entrées 0011, 0016, 0017, 0019, 0020, 0021, 0022, 0023, 0024, 0025, 0026, 0028, 0029, 0030, 0031, 0032, 0033, 0034, 0035, 0036, 0037, 0038, 0050, 0052, 0053, 0054, 0055, 0056, 0057, 0058, 0059, 0080, 0081, 0082, 0083, 0084 de `docs/project/backlog/` sont appliquées à leurs documents sources et passées au statut « fait » ; le commit de départ de `conform-verify` est réépinglé sur ce commit.

Spec : [SPEC-backlog-besoin.md](SPEC-backlog-besoin.md)

## 7. gabarits-domaine

Gabarits — la couche domaine (F2).

**Figé si** : chaque document de la couche domaine passe `bash .claude/skills/write/write.sh check <module> <fichier>` sans finding de gabarit, et `conform.sh verify` sort en code 0 avec les seuls titres renommés et mots-clés traduits déclarés.

Spec : [SPEC-gabarits-domaine.md](SPEC-gabarits-domaine.md)

## 8. backlog-domaine

Backlog — la couche domaine.

**Figé si** : les entrées 0007, 0010, 0027, 0039, 0040, 0041, 0042, 0043, 0044, 0048, 0060, 0061, 0085, 0086, 0087 de `docs/project/backlog/` sont appliquées à leurs documents sources et passées au statut « fait » ; le commit de départ de `conform-verify` est réépinglé sur ce commit.

Spec : [SPEC-backlog-domaine.md](SPEC-backlog-domaine.md)

## 9. gabarits-interface

Gabarits — la couche interface (F2).

**Figé si** : chaque document de la couche interface passe `bash .claude/skills/write/write.sh check <module> <fichier>` sans finding de gabarit, et `conform.sh verify` sort en code 0 avec les seuls titres renommés et mots-clés traduits déclarés.

Spec : [SPEC-gabarits-interface.md](SPEC-gabarits-interface.md)

## 10. backlog-interface

Backlog — la couche interface.

**Figé si** : les entrées 0001, 0002, 0013, 0018, 0045, 0046, 0047, 0062, 0063, 0064, 0065, 0088, 0089 de `docs/project/backlog/` sont appliquées à leurs documents sources et passées au statut « fait » ; le commit de départ de `conform-verify` est réépinglé sur ce commit.

Spec : [SPEC-backlog-interface.md](SPEC-backlog-interface.md)

## 11. gabarits-decisions-architecture-assurance

Gabarits — les couches décisions, architecture et assurance (F2).

**Figé si** : chaque document de les couches décisions, architecture et assurance passe `bash .claude/skills/write/write.sh check <module> <fichier>` sans finding de gabarit, et `conform.sh verify` sort en code 0 avec les seuls titres renommés et mots-clés traduits déclarés.

Spec : [SPEC-gabarits-decisions-architecture-assurance.md](SPEC-gabarits-decisions-architecture-assurance.md)

## 12. backlog-decisions-architecture-assurance

Backlog — les couches décisions, architecture et assurance.

**Figé si** : les entrées 0003, 0008, 0009, 0014, 0015, 0049, 0066, 0067, 0068, 0069 de `docs/project/backlog/` sont appliquées à leurs documents sources et passées au statut « fait » ; le commit de départ de `conform-verify` est réépinglé sur ce commit.

Spec : [SPEC-backlog-decisions-architecture-assurance.md](SPEC-backlog-decisions-architecture-assurance.md)

## 13. gabarits-reference-projet

Gabarits — les couches référence et projet (F2).

**Figé si** : chaque document de les couches référence et projet passe `bash .claude/skills/write/write.sh check <module> <fichier>` sans finding de gabarit, et `conform.sh verify` sort en code 0 avec les seuls titres renommés et mots-clés traduits déclarés.

Spec : [SPEC-gabarits-reference-projet.md](SPEC-gabarits-reference-projet.md)

## 14. backlog-reference-projet

Backlog — les couches référence et projet.

**Figé si** : les entrées 0004, 0005, 0006, 0012, 0051, 0070, 0071, 0072, 0073, 0074, 0075, 0076, 0077, 0078, 0079, 0090, 0091, 0092 de `docs/project/backlog/` sont appliquées à leurs documents sources et passées au statut « fait » ; le commit de départ de `conform-verify` est réépinglé sur ce commit.

Spec : [SPEC-backlog-reference-projet.md](SPEC-backlog-reference-projet.md)

## 15. doublons

Doublons (F3).

**Figé si** : les 11 emplois de termes rejetés par le glossaire sont remplacés par le terme retenu, et chaque définition dupliquée n'a plus qu'un propriétaire, l'autre lieu gardant un renvoi d'une ligne.

Spec : [SPEC-doublons.md](SPEC-doublons.md)

## 16. documents-generes

Documents générés et conventions (F5).

**Figé si** : `docs/conventions.md`, le catalogue d'événements, la matrice de traçabilité et le journal du corpus existent et passent leurs contrôles, et les registres d'identifiants et de marqueurs sont générés par `corpus.sh render`.

Spec : [SPEC-documents-generes.md](SPEC-documents-generes.md)

## 17. write-processing-register-et-dpia

Registre des traitements et AIPD.

**Figé si** : chaque module de la tâche a au moins une instance qui passe `write.sh check`, et son index existe.

Spec : [SPEC-write-processing-register-et-dpia.md](SPEC-write-processing-register-et-dpia.md)

## 18. write-release-plan

Plan de livraison.

**Figé si** : chaque module de la tâche a au moins une instance qui passe `write.sh check`, et son index existe.

Spec : [SPEC-write-release-plan.md](SPEC-write-release-plan.md)

## 19. write-operating-model-et-content-guide

Modèle de fonctionnement et guide de contenu.

**Figé si** : chaque module de la tâche a au moins une instance qui passe `write.sh check`, et son index existe.

Spec : [SPEC-write-operating-model-et-content-guide.md](SPEC-write-operating-model-et-content-guide.md)

## 20. write-interview-reports

Comptes rendus d'entretien.

**Figé si** : au moins un compte rendu d'entretien réel est écrit et passe `write.sh check interview-reports`. Tant qu'aucun entretien n'a eu lieu, la tâche reste bloquée.

Spec : [SPEC-write-interview-reports.md](SPEC-write-interview-reports.md)

## 21. solde-du-backlog

Solde du backlog.

**Figé si** : `docs/project/` n'existe plus, chaque entrée encore « ouvert » est devenue une tâche TB du plan de travail avec un renvoi vers son origine, et les règles `.claude/rules/corpus-backlog.md` et l'ADR-019 (§ Reste à faire) ne citent plus `docs/project/backlog/`.

Spec : [SPEC-solde-du-backlog.md](SPEC-solde-du-backlog.md)

## 22. controles-en-ci

Contrôles en CI.

**Figé si** : un workflow GitHub lance les quatre contrôles sur toute modification de `docs/`, et deux nouveaux contrôles signalent les marqueurs hors liste fermée et les identifiants définis deux fois.

Spec : [SPEC-controles-en-ci.md](SPEC-controles-en-ci.md)

## 23. reprise-du-suivi-github

Reprise du suivi GitHub.

**Figé si** : le workflow `recalcul-suivi-projet` est réactivé, une exécution a régénéré les issues, et chaque issue ouverte pointe vers le nouveau chemin du plan de travail.

Spec : [SPEC-reprise-du-suivi-github.md](SPEC-reprise-du-suivi-github.md)

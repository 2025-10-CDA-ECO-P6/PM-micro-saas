# ADR-022 — Le chantier de conformité s'exécute sous six conditions

- **Statut** : Accepté
- **Date** : 2026-10-07
- **Nature** : méthode

## Contexte

La revue de l'architecte du 2026-10-07 a validé le chemin des ADR-020 et ADR-021, à condition de corriger trois points : une vérification sans perte impossible si gabarits et substance sont mêlés, un plan calculé qui ne peut pas servir d'oracle, et une arborescence cible sous-spécifiée.

## Décision

Le chantier de conformité s'exécute sous six conditions. (1) Les déplacements suivent une table ancien → nouveau écrite et relue avant tout déplacement, et non les emplacements par défaut du catalogue. Elle regroupe le domaine par contexte, garde le niveau surface des fiches écran et place conventions-dod et roadmap-produit sous docs/projet/ comme le veut l'ADR-020. (2) La réécriture de la sélection et les déplacements forment un même commit. (3) Pour chaque couche, les gabarits et les entrées du backlog sont deux commits distincts. Le commit de départ de conform-verify est réépinglé après chaque commit de backlog, car seul ce dernier change la substance. (4) Après les déplacements, la différence des golden masters doit être exactement la substitution des chemins. (5) Le backlog docs/project/backlog/ est consommé par le chantier : les entrées encore ouvertes à la fin deviennent des TB du plan de travail, puis docs/project/ est retiré. (6) Le workflow recalcul-suivi-projet est suspendu pendant le chantier, puis réactivé à la fin, et les issues GitHub sont mises à jour.

**Raison.** Revue de l'architecte du 2026-10-07. conform-verify n'excuse que les titres renommés, les chemins réécrits et les replis (conform-verify.sh:19-22) : un commit qui mêle gabarits et changements de substance le ferait échouer, ou le réduirait à rien. Le plan calculé par conform.sh envoie un README sur un autre (docs/conception/besoin/user-stories/README.md vers docs/securite/conformite/), aplatit les fiches écran et ne déplace pas le domaine. Le catalogue range deux documents dans docs/pilotage/ (labels:992,1030), contre l'ADR-020.

## Alternatives considérées

- Un commit unique par couche (gabarits et backlog ensemble) : écarté, car la vérification sans perte deviendrait impossible.
- Suivre les emplacements par défaut du catalogue : écarté, car ils contredisent l'ADR-020 et produisent des collisions.
- Laisser le workflow tourner pendant le chantier : écarté par l'opérateur, pour ne pas réécrire les issues à chaque étape.

## Conséquences

Ce qui devient vrai : chaque commit du chantier est vérifiable de l'extérieur, et aucun contenu ne part au mauvais endroit.

Ce qui coûte : une table de déplacements écrite à la main, deux fois plus de commits par couche, et une suspension du suivi GitHub pendant le chantier.

Dette : les défauts de conform.sh (collision des README, aplatissement des fiches écran, domaine non reconnu) sont à remonter à condor.

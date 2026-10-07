# 0009 — Reconstruire VerifCorpus et vérifier que la DLL suit la source

- **Id** : 0009
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-decisions-architecture-assurance (chantier conformite-corpus)
- **Appliqué dans** : aucun fichier modifié (vérification).
- **Conclusion** : La DLL suit la source. `git log -- tools/VerifCorpus` ne donne qu'un commit, 81b30eb (2026-09-03 22:40) : la source n'a reçu aucun commit ensuite. Le commit 427dfd7 (WIP, 2026-10-06) ne touche aucun fichier sous `tools/` (`git show --stat 427dfd7`, 0 ligne `tools/`). La DLL du 2026-09-04 est donc postérieure à la dernière modification de la source. La preuve est cet argument chronologique. La comparaison de sorties faite ensuite (`dotnet build tools/VerifCorpus` dans le worktree, 0 avertissement, 0 erreur ; sortie identique à celle de la DLL du dépôt principal) oppose deux DLL reconstruites le 2026-10-07 : elle confirme que la source compile et se comporte de même, mais ne dit rien de la DLL du 2026-09-04, écrasée par la reconstruction.

Reconstruire VerifCorpus et vérifier que la DLL suit la source. La DLL précompilée date du 2026-09-04 ; la source a reçu des commits ensuite (dont le WIP 427dfd7).

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

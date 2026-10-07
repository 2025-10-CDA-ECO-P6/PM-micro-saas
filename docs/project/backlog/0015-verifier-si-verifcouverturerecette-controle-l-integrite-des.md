# 0015 — Vérifier si VerifCouvertureRecette contrôle l'intégrité des citations Gherkin

- **Id** : 0015
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-decisions-architecture-assurance (chantier conformite-corpus)
- **Appliqué dans** : docs/projet/conventions-dod.md (ligne de la commande `VerifCouvertureRecette`).
- **Conclusion** : Non : l'outil ne contrôle pas l'intégrité des citations Gherkin. `tools/VerifCouvertureRecette/VerificateurCouvertureRecette.cs` ne fait que deux choses : classer chaque cas de recette face aux identifiants `CR-` cités par le plan de travail (`Classificateur.cs`, `CitationsPlan.cs`), et lister les cas dont la colonne Source contient « retirée » (`[SOURCE RETIRÉE]`). La colonne Source n'est lue que pour y chercher `CR-`, `US-` ou « retirée » ; aucun titre de scénario n'est comparé à `docs/besoin/stories/`. Le contrôle décrit par `docs/assurance/strategie-de-test.md` § 13 n'a donc pas d'outil. La ligne de `docs/projet/conventions-dod.md` qui annonçait « l'intégrité de leurs citations » décrivait faussement l'outil : corrigée. § 13 ne nomme pas l'outil et n'est pas modifié.

Vérifier si VerifCouvertureRecette contrôle l'intégrité des citations Gherkin. Le cahier de recette (cahier-strategie-test-et-recette.md:835) décrit ce contrôle par jalon ; le périmètre déclaré de l'outil est la couverture par le plan et les RB retirées.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

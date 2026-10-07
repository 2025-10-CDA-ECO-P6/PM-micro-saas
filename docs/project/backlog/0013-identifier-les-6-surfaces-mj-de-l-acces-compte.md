# 0013 — Identifier les « 6 surfaces MJ » de l'accès compte

- **Id** : 0013
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : `docs/interface/raisonnement-ux.md`

Identifier les « 6 surfaces MJ » de l'accès compte. reflexion-ux-mvp.md:305 en annonce six ; gabarit-ecran.md:176 en liste cinq.

**Conclusion.** Il n'existe pas de « 6 surfaces MJ ». Deux comptes distincts coexistent, aucun n'est six. [gabarit-ecran.md](../../interface/gabarit-ecran.md) (§ Mode local) liste cinq surfaces au sens de l'ossature (tableau de bord, vue campagne / espace de travail, vue session MJ, vue de préparation, espace personnel), ce qui concorde avec l'ossature de [zonage.md §2](../../interface/zonage.md#ossature-de-navigation) : le décompte du gabarit est juste et reste. Le « 6 » de [raisonnement-ux.md](../../interface/raisonnement-ux.md) (Lentille 1 b et ligne 2 du tableau synthétique) désignait les écrans qui reçoivent l'accès compte (châssis S7) ; la passe de re-audit qui l'a posé (commit d55a759, journal D6 de AUDIT.md) en énumère dix : `tableau-de-bord`, `creation-espace`, `vue-campagne`, `navigation-dossiers`, `recherche-preparation`, `editeur-document`, `editeur-scenario`, `parametres-campagne`, `vue-espace-personnel`, `vue-session-mj` (`panneau-creation-rapide`, sur-couche de la vue session, n'en porte pas). Corrigé à la source (le raisonnement UX) : « 10 écrans MJ », liste donnée. Le « 6 surfaces » de [ecrans/index.html](../../interface/ecrans/index.html) est un autre compte, juste (six groupes SV) ; « 6 surfaces documentaires » en §3 de raisonnement-ux.md est aussi un autre compte (six noms, dont la surface joueur, le profil et le gate de migration : pas des surfaces MJ seules), non touché.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

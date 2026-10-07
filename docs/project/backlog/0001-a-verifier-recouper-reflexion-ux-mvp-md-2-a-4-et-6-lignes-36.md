# 0001 — Recouper reflexion-ux-mvp §2-§4 et §6 avec les fiches écran

- **Id** : 0001
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : `docs/interface/raisonnement-ux.md`. Alignement induit, hors de la plage vérifiée : §5 Famille C, fournisseurs d'identité, passé en DÉCIDÉ 2026-07-15 (ADR-015, enregistré au zonage §9 : Google et Discord)

À vérifier : recouper reflexion-ux-mvp.md §2 à §4 et §6 (lignes 36-139 et 188-211) avec les fiches écran ; cette partie n'a pas été lue ligne à ligne lors de l'onboarding du 2026-10-07

**Conclusion de la vérification.** §2, §3, §4 et §6 recoupés avec les fiches d'écran (`vue-espace-personnel`, `tableau-de-bord`, `vue-campagne`, `parametres-campagne`, `editeur-document`, `vue-session-mj`), avec [zonage.md](../../interface/zonage.md) et avec les personas. Cinq écarts, tous corrigés dans [raisonnement-ux.md](../../interface/raisonnement-ux.md) (le raisonnement UX n'introduit aucune règle ; la source la plus à jour l'emporte) :

- §2 déclarait la présentation de l'espace personnel « ouverte » au zonage ; [zonage.md §9](../../interface/zonage.md#9-trous-de-corpus-et-points-dinterview) la donne résolue (mix assumé), comme le dit la fiche `vue-espace-personnel`. Corrigé aux deux endroits, et le renvoi « confirmées par UC-02 §interview » vise désormais UC-02 §Contexte (la section interview ne confirme aucune contrainte).
- §3 citait « pas de panneau de réglages séparé » comme citation de US-06-02 ; le texte de la story est « Pas de panneau paramètres séparé ». Citation rétablie à l'identique.
- §4 disait des backlinks qu'ils se replient par défaut ; la fiche `editeur-document` les classe hors MVP, zone non affichée, placement figé conservé (AR-11). Précision ajoutée.
- §6 décrivait Rémi comme « joueur distant, coordinateur » ; [persona-06](../../besoin/decouverte/personas/persona-06-remi.md) est un MJ papier, probablement hors cible. Corrigé.
- §9 (ligne « Famille C zoning §S9 ») disait la présentation de l'espace personnel non résolue ; alignée sur le zonage (conséquence du premier écart).

Le reste de §2 à §4 et §6 concorde avec les fiches : reprise de session doublement câblée (`tableau-de-bord`, `vue-campagne`), renvoi sans configuration propre dans `parametres-campagne` (AR-18), export au second plan et archivage replié (`parametres-campagne`), règle d'éviction (`vue-session-mj`).

**Raison.** Trou de lecture de l'analyse d'onboarding, classé B par l'opérateur

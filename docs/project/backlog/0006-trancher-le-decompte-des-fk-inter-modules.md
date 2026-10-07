# 0006 — Trancher le décompte des FK inter-modules : 19 ou 17

- **Id** : 0006
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-reference-projet (chantier conformite-corpus)
- **Appliqué dans** : `docs/reference/cahier-specifications-techniques.md` (§ 4 et § 11)
- **Conclusion** : Le décompte est 17, il n'y a plus d'écart. ADR-011 recense 17 lignes « Cross » dans sa matrice (16 pures et 1 mixte `documents.source_document_id`, 38 FK au total) ; ADR-009 disait 19 dans le texte, corrigé en 17 par le commit `f4214d1` (2026-09-02), et dit 17 à `ADR-009:44,54`. Le « 19 » n'existe plus dans aucune source : le CdS le reportait depuis une version périmée. Le marqueur À TRANCHER du § 4 et la puce du § 11 sont retirés, le décompte 17 est écrit au § 4 et dans le tableau de renvoi.

Trancher le décompte des FK inter-modules : 19 ou 17. Écart signalé à docs/context/cahier-specifications-techniques.md:549, non recoupé avec ADR-009 et ADR-011.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

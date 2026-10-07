# 0007 — Vérifier l'unicité de la session LIVE pour un espace PERSONAL

- **Id** : 0007
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/project/backlog/0007-verifier-l-unicite-de-la-session-live-pour-un-espace-persona.md` (conclusion de la vérification, ci-dessous)

Vérifier l'unicité de la session LIVE pour un espace PERSONAL. docs/architecture/ddd-fondations.md:33 pose une seule session LIVE par espace ; le cas de l'espace personnel (mono-membre, sans invités) n'a pas été vérifié dans le domaine.

**Conclusion de la vérification.** L'unicité de la session `LIVE` tient pour un espace `PERSONAL` : cet espace n'a aucune session (« Une session n'existe que dans un espace partagé (`CAMPAIGN` ou `ONE_SHOT`) », [Session Conduct, § Agrégats](../../domaine/session-conduct/context.md#session)), donc la règle « une seule session `LIVE` par espace » (invariant 8 de Session Conduct ; [fondations](../../architecture/fondations.md)) est vérifiée sans objet pour ce type — zéro session, jamais deux. Le même contexte pose l'absence de `SessionViewConfig` pour `PERSONAL` (invariant 5) et Space Management classe les sessions « non applicable » pour ce type ([One-shot — spécificités](../../domaine/space-management/context.md)). Observation, sans décision : l'absence de session est posée en prose ; aucun invariant de Session Conduct ne nomme de garde qui fasse refuser `Session.Start()` pour un espace `PERSONAL`.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

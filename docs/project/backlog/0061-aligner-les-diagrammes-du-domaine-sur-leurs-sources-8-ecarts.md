# 0061 — Aligner les diagrammes du domaine sur leurs sources (8 écarts)

- **Id** : 0061
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/core/classes.md`, `docs/domaine/space-management/classes.md`, `docs/domaine/identity-access/logical-data.md`, `docs/domaine/content-library/conceptual-data.md`, `docs/domaine/identity-access/flows.md`, `docs/domaine/content-library/flows.md`, `docs/domaine/session-conduct/flows.md`, `docs/domaine/space-management/flows.md`

- [domain-classes:contradicted-1 — stale, core.md] Ajouter la classe GuestAccessId au diagramme de classes du Core
- [domain-classes:contradicted-2 — stale, space-management.md] Ajouter Unarchive() à Space et la classe SpaceUnarchived produite par Space dans le diagramme de classes SM
- [domain-mld:contradicted-2 — gap, identity-access.md] Compléter la note du MLD I&A avec folders.created_by_id, documents.created_by_id et sessions.created_by_id
- [domain-mcd:contradicted-1 — stale, content-library.md] Remplacer « unique » sur DOCUMENT_TYPE.slug dans le MCD par « unique par espace, global pour les types système (C2) »
- [domain-flows:contradicted-1 — stale, identity-access.md] Corriger le libellé en « Downgrade PRO → FREE (résiliation) »
- [domain-flows:contradicted-2 — stale, content-library.md] Ajouter aux trois flux une branche PERSONAL qui ne crée que le dossier virtuel
- [domain-flows:contradicted-3 — stale, session-conduct.md] Conditionner SessionViewConfig.Create au type CAMPAIGN/ONE_SHOT dans le flux SC, dans la même retouche que 0043
- [domain-flows:contradicted-5 — stale, space-management.md] Restreindre le contrôle de quota et la boucle de gel des flux SM aux espaces CAMPAIGN/ONE_SHOT

**Raison.** Contradictions triées stale/gap, tri validé en bloc par l'opérateur le 2026-10-07, regroupées par document à corriger

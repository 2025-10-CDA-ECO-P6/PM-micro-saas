# 0087 — Amender l'invariant 4 de Content Library

- **Id** : 0087
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/content-library/context.md`, `docs/domaine/content-library/logical-data.md`, `docs/besoin/cas-d-utilisation/UC-05-organiser-dossiers.md`, `docs/besoin/stories/US-UC-05-organiser-contenu-dossiers.md`, `docs/besoin/parcours-utilisateur/UJ-UC-05-organiser-contenu-dossiers.md`, `docs/glossaire.md`, `docs/interface/zonage.md`
- **Note** : la source (UC-05 A4) et ses consommateurs (US-UC-05, UJ-UC-05, glossaire, zonage AR-16) sont alignés sur la décision de l'opérateur du 2026-10-07 : « Non classés » est affiché dès qu'il n'est pas vide.

Amender l'invariant 4 de Content Library : Non classés est visible dès qu'il n'est pas vide. Il est aussi toujours visible dans un espace PERSONAL sans autre dossier, comme le dit zoning.md:486 ; sinon, les notes repliées par 0042 seraient introuvables (content-library.md:286, :143).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Toujours invisible, en retirant la précision du zoning

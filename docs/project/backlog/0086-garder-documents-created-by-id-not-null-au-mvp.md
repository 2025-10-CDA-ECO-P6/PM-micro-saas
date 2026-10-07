# 0086 — Garder documents.created_by_id NOT NULL au MVP

- **Id** : 0086
- **Date** : 2026-10-07
- **Statut** : ouvert

Garder documents.created_by_id NOT NULL au MVP. Plus aucun document d'invité n'existe au MVP (0081) : marquer guestAccessId post-MVP et retirer « quand createdById est null » (content-library.md:77, diagrams/mld/content-library.md:62-64). La question reviendra avec les notes joueur.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Nullable avec contrainte « exactement un auteur »

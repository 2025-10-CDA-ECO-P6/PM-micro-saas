# 0085 — Retirer Identity & Access des consommateurs d'InvitationRevoked

- **Id** : 0085
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/space-management/context.md` (table des événements domaine, `InvitationRevoked`)

Retirer Identity & Access des consommateurs d'InvitationRevoked. L'invalidation reste interne à Space Management (space-management.md:279, :61) ; I&A ne consomme toujours aucun événement externe (identity-access.md:148).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- I&A réagit à InvitationRevoked

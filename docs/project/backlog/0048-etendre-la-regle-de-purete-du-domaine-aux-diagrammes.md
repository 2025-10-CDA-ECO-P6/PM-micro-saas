# 0048 — Étendre la règle de pureté du domaine aux diagrammes

- **Id** : 0048
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/README.md` (§ Sens de dépendance besoin ↔ décision — pureté du domaine), `docs/domaine/identity-access/flows.md` (flux 2), `docs/domaine/content-library/flows.md` (flux 8)

Étendre la règle de pureté du domaine aux diagrammes. JWT, ITokenDenylist et le SQL quittent docs/conception/domain/diagrams/ pour les specs d'architecture, que les diagrammes citent (domain/README.md:56-58, flows/identity-access.md:24, :36, flows/content-library.md:132).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Pureté limitée à la prose, diagrammes techniques permis

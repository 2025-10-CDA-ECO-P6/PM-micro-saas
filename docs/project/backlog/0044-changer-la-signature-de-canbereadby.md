# 0044 — Changer la signature de CanBeReadBy : le lecteur est un membre (UserId et rôle) ou un invité (GuestAccessId)

- **Id** : 0044
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/content-library/context.md`, `docs/domaine/content-library/classes.md`, `docs/decisions/ADR-014-modele-autorisation-api.md`, `docs/assurance/strategie-de-test.md`
- **Note** : appliquée avec l'alignement de consommateur de l'entrée 0024 (déjà « fait »). Décision de l'opérateur du 2026-10-07 : l'exception de RB-12-01 ne vaut que pour les fiches `GM_ONLY` ou `PUBLIC`, `PLAYER_PRIVATE` reste auteur seul sans exception (règles métier 2 et 3 de `content-library/context.md`). La signature de `CanBeReadBy` ne dit pas comment le domaine connaît le personnage associé au lecteur : SOUS-SPÉCIFIÉ dans la règle métier 2.

Changer la signature de CanBeReadBy : le lecteur est un membre (UserId et rôle) ou un invité (GuestAccessId). Mettre à jour la règle 3 (content-library.md:300) et le diagramme de classes de Content Library.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Une méthode dédiée CanBeReadByGuest

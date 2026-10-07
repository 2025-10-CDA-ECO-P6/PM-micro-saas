# 0049 — Corriger PO-05 : le délai d'effacement de compte est de 1 mois, extensible à 2 (ADR-012:147)

- **Id** : 0049
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-decisions-architecture-assurance (chantier conformite-corpus)
- **Appliqué dans** : docs/assurance/strategie-de-test.md (§ 12, PO-05) ; docs/projet/decisions-en-attente.md (ligne citant PO-05, hors couche, alignée)

Corriger PO-05 : le délai d'effacement de compte est de 1 mois, extensible à 2 (ADR-012:147). Retirer « J+30 », qui est le délai de purge d'un espace supprimé (ADR-012:16), du critère PO-05 (cahier-strategie-test-et-recette.md:815).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- J+30, PO-05 visant la purge d'espace

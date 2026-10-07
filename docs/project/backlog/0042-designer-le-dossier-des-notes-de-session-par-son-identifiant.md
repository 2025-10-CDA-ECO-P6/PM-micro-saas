# 0042 — Désigner le dossier des notes de session par son identifiant, avec repli sur Non classés

- **Id** : 0042
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/content-library/context.md`, `docs/domaine/content-library/classes.md`, `docs/domaine/content-library/conceptual-data.md`, `docs/domaine/content-library/logical-data.md`, `docs/domaine/content-library/flows.md`, `docs/domaine/session-conduct/context.md`, `docs/domaine/session-conduct/flows.md` ; `docs/interface/zonage.md`
- **Note** : l'entrée disait « porté par l'identifiant ». Décision de l'opérateur du 2026-10-07 : `Folder` porte un rôle système `systemRole` (valeur `NOTES`), fixé à la création et indépendant du nom ; il est posé dans la règle métier 12 de Content Library, créée par cette entrée. Consommateur aligné : `docs/interface/zonage.md` (décision sur les dossiers par défaut).

Désigner le dossier des notes de session par son identifiant, avec repli sur Non classés. Le rôle système « notes » est porté par l'identifiant, pas par le nom, donc un renommage est sans effet ; si le dossier a été supprimé, la LIVE_NOTE va dans Non classés (session-conduct.md:131, content-library.md:285).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Recréer automatiquement le dossier Notes
- Dossier cible choisi dans le SessionViewConfig

# 0045 — Avant la saisie du nom, l'accès par lien ne montre que les métadonnées du lien

- **Id** : 0045
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : `docs/interface/zonage.md` (S4, ligne « Accès par lien + saisie du nom d'affichage »), `docs/interface/conventions.md` (Registre 2, « zone de contenu partagé »), `docs/besoin/cas-d-utilisation/UC-09-acces-session-joueur.md` (scénario nominal A §4), `docs/besoin/parcours-utilisateur/bout-en-bout/parcours-03-lucas.md`, `docs/interface/ecrans/sv5-joueur/acces-lien-saisie-nom/acces-lien-saisie-nom.md` (zone d'information du lien, affordance « prendre connaissance du lien », Sources), `docs/interface/conventions.md` (Registre 2, « zone d'information du lien »)
- **Note** : reformulée selon le flux 4 de `docs/domaine/space-management/flows.md` ; la décision de l'opérateur prime sur UC-09, amendé avec son consommateur parcours-03. La fiche `acces-lien-saisie-nom.md` et les deux lignes de `conventions.md` (« zone de contenu partagé », « zone d'information du lien ») citent UC-09 scénario A §4, amendé, comme source première ; le flux 4 du domaine reste cité en second. L'information RGPD, que UC-09 §4 range parmi ce qui est affiché avant la saisie du nom, l'est bien : la fiche la porte dans sa zone dédiée, visible avant la saisie, et la définition du terme « zone d'information du lien » ne la reprend donc pas.

Avant la saisie du nom, l'accès par lien ne montre que les métadonnées du lien. Nom de l'espace ou de la session, MJ, information RGPD ; aucun document avant GuestAccess.Create. Reformuler zoning.md:246 selon le flux space-management:73-75.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Créer le GuestAccess dès l'ouverture du lien

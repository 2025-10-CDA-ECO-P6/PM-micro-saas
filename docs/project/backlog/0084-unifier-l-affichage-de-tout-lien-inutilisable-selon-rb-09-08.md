# 0084 — Unifier l'affichage de tout lien inutilisable selon RB-09-08

- **Id** : 0084
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-besoin (chantier conformite-corpus)
- **Appliqué dans** : `docs/besoin/cas-d-utilisation/UC-09-acces-session-joueur.md`, `docs/besoin/parcours-utilisateur/UJ-UC-09-acces-session-joueur.md`, `docs/besoin/stories/US-UC-09-acces-session-joueur.md`, `docs/besoin/stories/US-UC-11-gerer-membres-espace-partage.md`, `docs/besoin/parcours-utilisateur/bout-en-bout/parcours-03-lucas.md`, `docs/interface/ecrans/sv5-joueur/erreur-acces/erreur-acces.md`, `docs/assurance/strategie-de-test.md`, `docs/reference/cahier-des-charges.md` ; `docs/domaine/session-conduct/flows.md` ; `docs/domaine/space-management/flows.md` ; `docs/interface/ecrans/sv5-joueur/erreur-acces/erreur-acces.dc.html` ; `docs/besoin/parcours-utilisateur/UJ-UC-11-gerer-membres-espace-partage.md`
- **Note** : décision de l'opérateur : le message unique d'un lien inutilisable est « Ce lien n'est pas utilisable » (il ne suppose plus que le lien a été actif).

Unifier l'affichage de tout lien inutilisable selon RB-09-08. Un seul affichage pour un lien expiré, révoqué, invalide ou malformé, qui ne révèle pas si le lien a existé. Aligner UC-09 A3 (:71-75), E1 (:79-81) et UJ-UC-09:56-57.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Deux affichages distincts, en retirant l'unification de RB-09-08

# 0081 — Sortir du MVP toutes les notes de session du joueur (Could Have)

- **Id** : 0081
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-besoin (chantier conformite-corpus)
- **Appliqué dans** : `docs/besoin/cas-d-utilisation/UC-06-vue-session.md`, `docs/besoin/cas-d-utilisation/UC-12-consulter-espace-joueur.md`, `docs/besoin/cas-d-utilisation/UC-09-acces-session-joueur.md`, `docs/besoin/cas-d-utilisation/UC-08-partager-information.md`, `docs/besoin/cas-d-utilisation/UC-11-gerer-membres-espace-partage.md`, `docs/besoin/stories/US-UC-09-acces-session-joueur.md`, `docs/besoin/stories/US-UC-11-gerer-membres-espace-partage.md`, `docs/besoin/vision/perimetre/matrice-de-priorites.md`, `docs/reference/cahier-des-charges.md` ; `docs/besoin/stories/US-UC-06-vue-session.md`, `docs/besoin/stories/US-UC-10-compte-cloud.md`, `docs/besoin/stories/US-UC-12-consulter-espace-joueur.md`, `docs/besoin/parcours-utilisateur/UJ-UC-06-vue-session.md`, `docs/besoin/parcours-utilisateur/UJ-UC-12-consulter-espace-joueur.md`, `docs/besoin/parcours-utilisateur/bout-en-bout/parcours-01-thomas.md`, `docs/besoin/nfr/NFR-CONF-01-separation-notes-privees-vue-joueurs.md`, `docs/besoin/nfr/NFR-ACC-01-utilisabilite-sans-dispositif-de-pointage.md`
- **Note** : l'entrée 0036 (recherche des notes `PLAYER_PRIVATE`) devient sans objet au MVP. Les consommateurs en stories, parcours et NFR sont alignés dans le même commit : US-UC-06, US-UC-09, US-UC-10, US-UC-11, US-UC-12, UJ-UC-06, UJ-UC-09, UJ-UC-12, parcours-01, parcours-03, NFR-CONF-01, NFR-ACC-01. Les règles RGPD d'effacement des notes `PLAYER_PRIVATE` (RB-09-18, RB-09-19, RB-10-17, RB-10-18) restent en vigueur.

Sortir du MVP toutes les notes de session du joueur (Could Have). Retirer de UC-06 la prise de notes joueur en LIVE (UC-06:101, :255, :289) et aligner UC-12:72, UC-09 RB-09-14 et moscow.md:462. L'entrée 0036 (recherche des notes PLAYER_PRIVATE) devient sans objet au MVP.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Note joueur en LIVE en Must, seule la persistance d'une session à l'autre en Could

# 0005 — Vérifier si le CdC §12.1 est toujours à jour (épinglage, migration)

- **Id** : 0005
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-reference-projet (chantier conformite-corpus)
- **Appliqué dans** : `docs/reference/cahier-des-charges.md` (annexe « Points à arbitrer » ; § 5, règles de gestion et déroulé d'UC-07)
- **Conclusion** : Les deux points sont réconciliés en amont, l'annexe était périmée. Épinglage : UC-07 (`docs/besoin/cas-d-utilisation/UC-07*.md`, déroulé 6) et Session Conduct règle 7 (`docs/domaine/session-conduct/context.md`) disent la même chose, épinglé par défaut en session LIVE et désépinglable, jamais épinglée pour une `LIVE_NOTE` ; réconcilié par le commit `6ce3e66` (2026-07-15). Migration : UC-01 (étape 4) et UC-10 (étape 6, règles métier) disent « espace par espace, tout-ou-rien par espace », comme le glossaire (`docs/glossaire.md:81`). Le CdC est corrigé : point 1 et point 2 retirés de l'annexe, renvois « à réconcilier » retirés des règles de gestion et du déroulé d'UC-07. Restes hors couche : « campagne par campagne » dans `parcours-01-thomas.md:210,279` et `parcours-02-emilie.md:224`, « campagne » dans RB-01-11 de `US-UC-01` (lignes 261, 293).

Vérifier si le CdC §12.1 est toujours à jour (épinglage, migration). Le cahier des charges liste encore comme non réconciliés l'épinglage de session (UC-07 vs Session Conduct règle 7) et le vocabulaire de migration ; une réconciliation en amont depuis le 2026-07-02 n'a pas été vérifiée.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

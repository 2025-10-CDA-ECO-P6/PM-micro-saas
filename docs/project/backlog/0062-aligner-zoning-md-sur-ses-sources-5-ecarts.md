# 0062 — Aligner zoning.md sur ses sources (5 écarts)

- **Id** : 0062
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : `docs/interface/zonage.md` (S4 : lignes « Panneau de création rapide à la volée » et « Paramètres de campagne » ; S6 : AR-10 ; S9 : ligne « Sémantique `ARCHIVED PERSONAL` »), `docs/domaine/space-management/context.md`, `classes.md`, `conceptual-data.md`, `logical-data.md`, `flows.md`, `docs/reference/conception-detaillee.md`
- **Note** : Les écarts 1 et 5 sont un seul changement (ligne S4 du panneau de création rapide). L'écart 2 est le même changement que l'item 10 de l'entrée 0065 (invariant 14 → 15) : fait ici, le statut de 0065 n'est pas modifié. Dans la ligne S9, seule la première occurrence (« `space-management.md` invariant 14 tranchent : un espace `PERSONAL` est toujours `ACTIVE` ») devient « invariant 15 » ; la seconde (« `ARCHIVED` contredirait l'invariant 14 ») cite l'invariant 14 voulu, tel que l'invariant 15 du domaine le cite lui-même. Écart 4 : sources amont UC-02 scénario nominal §3 et « Données manipulées », US-UC-02 RB-02-01 et RB-02-04. Les champs sont nommés `description` / `gameSystem` (MLD : `description`, `game_system`, `text`, NULL) ; la signature devient `Create(ownerId, name, type, description?, gameSystem?)` (décision de l'orchestrateur sur délégation de l'opérateur), propagée à `flows.md` et à `docs/reference/conception-detaillee.md` (diagramme du flux de création). Le tableau de S4 « Paramètres de campagne » citait aussi `SpaceMembership` pour le lien permanent : aligné sur l'écart 3. Même écart : la ligne S4 « Affordance de génération de lien d'invitation » et la parenthèse « logé dans la vue session ou la vue campagne » de la raison d'être d'AR-10 sont alignées sur le texte d'AR-10 (lien ponctuel dans la vue session, lien permanent dans les paramètres de campagne).

- [interface-zoning:contradicted-1 — stale] Restreindre l'épinglage automatique de zoning.md:238 aux documents durables, la note de session étant rattachée sans épinglage
- [interface-zoning:contradicted-3 — stale] Remplacer « invariant 14 » par « invariant 15 » dans zoning.md:696
- [interface-zoning:contradicted-4 — gap] Préciser dans AR-10 qu'un lien permanent utilisé par un anonyme donne un GuestAccess SPACE, l'étiquette SpaceMembership ne valant que pour un utilisateur connecté
- [interface-zoning:contradicted-5 — stale] Ajouter description et système de jeu optionnels à l'agrégat Space (et au MLD spaces)
- [screen-sheets:contradicted-11 — stale] Qualifier la ligne S4 de zoning.md:238 : épinglé automatiquement en LIVE seulement, jamais pour une note de session

**Raison.** Contradictions triées stale/gap, tri validé en bloc par l'opérateur le 2026-10-07, regroupées par document à corriger

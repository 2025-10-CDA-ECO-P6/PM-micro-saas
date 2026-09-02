# ADR-003 — Stack front : mono-écosystème Angular

- **Statut** : Accepté
- **Date** : 2026-06-09

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03.)*

---

## Contexte

La conception prévoyait initialement deux écosystèmes front distincts : Next.js pour la landing page (SSR/SEO) et Angular pour l'application principale (SPA). Cette dualité constitue une dette structurelle pour une équipe solo : deux pipelines de build, deux ensembles de dépendances, deux styles de composants à maintenir.

Par ailleurs, Blazor WASM n'avait jamais été évalué formellement alors que le backend est .NET/C#. Son évaluation était pertinente au regard de la question de l'exécution du domaine en mode local (ADR-001).

---

## Décision

**Angular pour l'application (SPA) et Angular SSR/prerender pour la landing page.** Un seul écosystème front, partage de la charte graphique et des composants entre les deux surfaces.

**Blazor WASM est écarté.** Next.js pour la landing est écarté.

---

## Alternatives considérées

**Blazor WASM (domaine C# potentiellement partagé navigateur/serveur).**
Écarté. Blazor WASM aurait pu dissoudre partiellement le problème du mode local décrit dans ADR-001, en permettant l'exécution du domaine C# dans le navigateur. Cette option a été évaluée. Les raisons de l'écarter : l'écosystème de composants UI est moins riche que l'écosystème JavaScript/TypeScript ; le poids du runtime WASM au premier chargement est une contrainte pour l'expérience utilisateur ; le vivier de recrutement front est plus large côté JS/TS. La conséquence sur ADR-001 (mode local en TypeScript minimal) est assumée.

**Maintenir Next.js comme second écosystème front pour la landing.**
Écarté. Double front pour une équipe solo, avec des coûts de maintenance disproportionnés au stade MVP.

---

## Conséquences

- Un seul écosystème front à maintenir : Angular pour les deux surfaces.
- La mise en place du rendu SSR/prerender pour la landing Angular représente un effort de configuration (frameworks, routes statiques, hydratation).
- Le mode local (ADR-001) reste à implémenter en TypeScript côté navigateur. Angular ne partage pas le domaine C# — cette conséquence du rejet de Blazor est explicitement assumée.
- La dette d'un double écosystème front dès J0 est résolue par cette décision.

---

## Compléments post-revue (2026-06-09)

Suite à une revue critique postérieure à cette décision, celle-ci est complétée comme suit, sans changer sa direction.

- **La landing MVP utilise SSG / prerender statique**, déployable sur CDN, pas SSR dynamique. Le SSR dynamique est réservé à une éventuelle évolution future si du contenu dynamique est nécessaire.

- **Budget de performance Angular (I-04).** La landing hérite du runtime Angular ; le budget de performance de l'application (I-04) s'applique donc également à la landing comme critère de qualité.

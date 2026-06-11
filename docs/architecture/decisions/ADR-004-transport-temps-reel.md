# ADR-004 — Transport temps réel : SignalR

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : CR-8, G-02, H-02, H-06, I-01, E-03

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03, audit conception pure 2026-06.)*

---

## Contexte

Le partage temps réel du MJ vers les joueurs est une fonctionnalité promise dans la conception (UC-08/09) et présentée comme un différenciateur central du produit. L'audit a relevé (finding CR-8) qu'aucune technologie de transport n'avait été choisie, alors que ce choix conditionne l'architecture d'infrastructure (connexions persistantes, sticky sessions), le modèle d'autorisation du canal invité (finding H-06), et le coût d'exploitation (finding I-01, E-03).

---

## Décision

**SignalR dès le départ.** SignalR est natif à ASP.NET Core, gère automatiquement les fallbacks (WebSocket → Server-Sent Events → long-polling), et est isolé dans le module `Infrastructure.Notifications`.

---

## Alternatives considérées

**Polling court ou SSE au MVP.**
Cette option était moins coûteuse en infrastructure (pas de connexions persistantes, pas de sticky sessions) et aurait été adéquate pour un besoin de latence de quelques secondes, tolérable dans le contexte d'une session de jeu de rôle. Elle a été présentée comme option viable lors de l'audit. L'opérateur a préféré une expérience push native.

**Pas de temps réel (différé post-MVP).**
Non viable : le partage temps réel MJ→joueurs est une fonctionnalité centrale (UC-08/09) identifiée comme Must dans le périmètre MVP (ADR-006).

---

## Conséquences

- Coût d'infrastructure temps réel dès le MVP : connexions persistantes, sticky sessions à prévoir côté hébergement. Ce coût est à chiffrer explicitement, en particulier l'impact sur le tier gratuit d'hébergement (Vague 2).
- L'authentification du canal invité sans compte (token `GuestAccess` porté sur le canal SignalR) est à concevoir, avec propagation des révocations et expirations en cours de session active (Vague 2, finding H-06).
- L'empreinte des sessions longues (sessions de jeu de plusieurs heures) sur les connexions persistantes est un point de surveillance opérationnelle (finding I-01).
- Le module `Infrastructure.Notifications` est isolé dès J0 (structure décidée dans ADR-008).

---

## Compléments post-revue (2026-06-09)

Suite à la revue adversariale (revue Vague 0, artefact purgé du corpus — historique git), cette décision est complétée comme suit, sans changer sa direction.

- **Configuration sobre obligatoire.** Le transport est forcé en SSE tant que la communication reste unidirectionnelle (MJ→joueurs). La connexion est fermée sur fin de session LIVE. Le heartbeat/keep-alive est allongé (événements rares en session de jeu). Un seuil de coût par session concurrente est à définir comme critère de réversibilité déclenchant le repli vers polling adaptatif — ce seuil est un livrable de configuration, pas un commentaire de documentation.

- **Sécurité du canal invité.** Le token `GuestAccess` ne doit pas être transmis en query-string : préférer un cookie court-lived ou un échange de token avant la négociation WebSocket. Chaque événement poussé est filtré selon la visibilité de la ressource (`PUBLIC`, `PLAYER_PRIVATE`, `GM_ONLY`) — pas de diffusion par groupe campagne indifférenciée. Une déconnexion forcée du hub est déclenchée sur `GuestAccessRevoked` et `GuestAccessExpired`. L'invariant d'autorisation défini dans ADR-007 s'applique intégralement au canal SignalR.

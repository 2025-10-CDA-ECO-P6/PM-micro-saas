# Spec — Seuil de coût session concurrente → repli SSE → polling adaptatif

> **Nature** : spec pré-build (approche cadre fidèle). Ce document reporte fidèlement ce qui est acté dans le corpus de décisions et nomme, sans les combler, les points laissés ouverts. Aucune valeur numérique, aucune formule de coût non explicitement actée dans le corpus n'est introduite ici. Les extraits de configuration ci-dessous sont **illustratifs et non normatifs** — ils balisent la forme du livrable de configuration attendu, pas son contenu.
>
> **Source normative** : [ADR-004 — Transport temps réel : SignalR](../decisions/ADR-004-transport-temps-reel.md), notamment les lignes 36 et 47 (« Compléments post-revue »).
>
> **Lecteur visé** : l'équipe qui configurera le module `Infrastructure.Notifications` (ADR-004:20, ADR-008:24) à l'entrée en build, et l'exploitant qui suivra le coût d'hébergement en production une fois le mécanisme en place.
>
> **Statut** : cadre à compléter — non implémentable en l'état, les trous nommés ci-dessous bloquent le passage en développement.

---

## 1. Pourquoi ce mécanisme existe

[ADR-004](../decisions/ADR-004-transport-temps-reel.md) acte SignalR comme transport temps réel dès le MVP, avec fallback natif WebSocket → SSE → long-polling. Cette décision a une conséquence de coût assumée mais non chiffrée :

> « Coût d'infrastructure temps réel dès le MVP : connexions persistantes, sticky sessions à prévoir côté hébergement. Ce coût est à chiffrer explicitement, en particulier l'impact sur le tier gratuit d'hébergement (Vague 2). » (ADR-004, section Conséquences, ligne 36)

**Les données de coût d'hébergement sont absentes du corpus.** Aucun chiffrage n'existe à date dans les documents accessibles ; ADR-004:36 renvoie lui-même ce chiffrage à un travail ultérieur (« Vague 2 »). Cette spec ne peut donc pas fixer de métrique de coût ni de seuil — elle ne fait que cadrer la forme du mécanisme de repli attendu.

La revue adversariale post-ADR ajoute, dans le même mouvement, un mécanisme de repli explicitement requis comme livrable de configuration :

> « Un seuil de coût par session concurrente est à définir comme critère de réversibilité déclenchant le repli vers polling adaptatif — ce seuil est un livrable de configuration, pas un commentaire de documentation. » (ADR-004:47)

Deux éléments à reporter fidèlement, sans les affaiblir :

- Le seuil est un **critère de réversibilité** — c'est-à-dire le signal qui déclenche un changement de mode de transport en cours d'exploitation, pas une simple observation.
- Le seuil est un **livrable de configuration** — une valeur exploitable par le système en production (fichier de config, variable d'environnement, feature flag), et non une mention documentaire sans effet opérationnel.

---

## 2. Contexte de configuration sobre déjà acté

ADR-004:47 fixe, indépendamment du seuil lui-même, un contexte de configuration qui s'applique dès l'entrée en build et que ce mécanisme de repli doit respecter :

> « Configuration sobre obligatoire. Le transport est forcé en SSE tant que la communication reste unidirectionnelle (MJ→joueurs). La connexion est fermée sur fin de session LIVE. Le heartbeat/keep-alive est allongé (événements rares en session de jeu). » (ADR-004:47)

À reporter tel quel :

| Élément acté | Portée |
|---|---|
| **SSE forcé** | Tant que le canal reste unidirectionnel MJ→joueurs (pas de retour joueur→MJ sur ce canal) |
| **Connexion fermée en fin de session LIVE** | La connexion persistante n'est pas maintenue au-delà de la session de jeu active |
| **Heartbeat/keep-alive allongé** | Justifié par la rareté des événements en session de jeu de rôle |
| **Sticky sessions + connexions persistantes** | Contrainte d'hébergement associée, mentionnée en Conséquences (ADR-004:36) comme point à prévoir côté infrastructure |

Ce contexte n'est pas remis en cause par le mécanisme de repli — il constitue le point de départ (le transport « normal » avant repli) sur lequel le mécanisme de repli vers polling adaptatif vient s'ajouter en cas de dépassement du seuil de coût.

---

## 3. Spec du mécanisme de repli — déclencheur, métrique, cadence, seuil

| Composant | Ce qui est acté | Ce qui est ouvert |
|---|---|---|
| **Déclencheur** | Dépassement du seuil de coût par session concurrente (ADR-004:47) | Mécanisme de détection du dépassement (évaluation continue ? périodique ? par palier d'hébergement ?) `[À TRANCHER — ticket]` |
| **Métrique de coût** | Doit exister « par session concurrente » (ADR-004:47) ; son chiffrage renvoie à un travail non fait (ADR-004:36) | **Définition de la métrique elle-même** (coût de connexions persistantes ? de sticky sessions ? formule composite ?) — absente du corpus. `[À TRANCHER — ticket]` |
| **Seuil** | Doit être un critère de réversibilité et un livrable de configuration, pas un commentaire (ADR-004:47) | **Valeur du seuil** — non fixée. `[À TRANCHER — ticket]` (ADR-004:47) |
| **Cadence du polling adaptatif** | Le repli cible « polling adaptatif » (ADR-004:47), nommé en alternative écartée comme option de latence « de quelques secondes, tolérable » (ADR-004:27) | **Cadence cible précise** du polling adaptatif — non fixée au-delà de cette mention qualitative. `[À TRANCHER — ticket]` |

### 3.1 Forme illustrative du livrable de configuration

L'extrait ci-dessous est **illustratif et non normatif** — il montre la forme attendue (une configuration exploitable, conforme à l'exigence « livrable de configuration, pas un commentaire » d'ADR-004:47), pas les valeurs, qui restent toutes `[À TRANCHER — ticket]` :

```yaml
# illustratif — non normatif — aucune valeur ci-dessous n'est actée
RealtimeFallback:
  CostMetric: "[À TRANCHER — ticket]"          # définition de la métrique de coût
  CostThresholdPerConcurrentSession: "[À TRANCHER — ticket]"  # seuil de réversibilité
  AdaptivePollingCadence: "[À TRANCHER — ticket]"             # cadence cible
```

---

## 4. Maillon non-vérifiable-en-CI

Le comportement de coût et de repli réel sous charge **n'est pas reproductible en CI**. La métrique de coût par session concurrente dépend de conditions d'hébergement en production (connexions persistantes réellement ouvertes, sticky sessions réellement réparties, facturation réelle du fournisseur) qu'un pipeline d'intégration continue ne reproduit pas.

**Conséquence à nommer explicitement** : le seuil défini en §3 est, par nature, un livrable de configuration dont la pertinence **ne peut être prouvée qu'en exploitation** — un test automatisé peut vérifier que le mécanisme de repli se déclenche correctement *pour une valeur de métrique simulée*, mais ne peut pas valider que le seuil choisi correspond effectivement au bon point de bascule coût/expérience en conditions réelles d'hébergement. Cette spec ne prétend pas couvrir ce maillon — il reste un point de surveillance opérationnelle une fois le mécanisme livré, cohérent avec le point de vigilance déjà acté sur l'empreinte des sessions longues (ADR-004, Conséquences, ligne 38 : « L'empreinte des sessions longues […] sur les connexions persistantes est un point de surveillance opérationnelle »).

---

## 5. Récapitulatif des points à trancher

| # | Point ouvert | Référence corpus |
|---|---|---|
| 1 | Valeur du seuil de coût par session concurrente | ADR-004:47 |
| 2 | Définition de la métrique de coût | ADR-004:36 (chiffrage absent du corpus, renvoyé à Vague 2) |
| 3 | Cadence cible du polling adaptatif | ADR-004:47 (cible qualitative seulement, ADR-004:27) |
| 4 | Mécanisme de détection du dépassement de seuil | Absent du corpus |

Ces quatre points bloquent le passage de cette spec à un ticket de développement. Aucun n'est tranché dans ce document.

---

## 6. Traçabilité

| Artefact | Nature du lien |
|---|---|
| [ADR-004 — Transport temps réel : SignalR](../decisions/ADR-004-transport-temps-reel.md) | Source normative du seuil de réversibilité, du contexte de configuration sobre, et du renvoi de chiffrage (lignes 36, 47) |
| [ADR-008 — Structure solution](../decisions/ADR-008-structure-solution.md) | Isolation du module `Infrastructure.Notifications`, périmètre d'implémentation du mécanisme de repli |
| [ADR-007 — RGPD et autorisation API](../decisions/ADR-007-rgpd-autorisation-api.md) | Invariant d'autorisation qui s'applique intégralement au canal SignalR (ADR-004:49) — contexte adjacent, hors périmètre du seuil traité ici |

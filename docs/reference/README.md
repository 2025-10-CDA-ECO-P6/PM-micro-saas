# Documents de référence — Haversack

> **Nature du document.** Ce dossier rassemble les documents autoportants de premier rang du
> projet : chacun se lit seul, sans avoir à ouvrir le reste du corpus, et peut être remis à un
> tiers (prestataire, investisseur, dossier de due diligence) indépendamment des autres. Ils
> consolident et mettent en forme le travail détaillé porté par [`../architecture/`](../architecture/README.md),
> [`../besoin/`](../besoin/README.md), [`../domaine/`](../domaine/README.md) et [`../interface/`](../interface/README.md), sans en devenir l'autorité : en cas
> d'écart, ils ne font jamais foi et c'est eux qu'on corrige
> ([ADR-019, § 1](../decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité)).

---

## Les quatre documents maîtres

Quatre documents consolident chacun un angle distinct du projet, sur le modèle quoi
fonctionnel / quoi technique / carte du système / comment. Chacun le déclare dans ses propres
métadonnées ou son objet :

| Document | Angle |
|---|---|
| [cahier-des-charges.md](cahier-des-charges.md) | Le **quoi fonctionnel** — document officiel de référence du périmètre produit du MVP |
| [cahier-specifications-techniques.md](cahier-specifications-techniques.md) | Le **quoi technique** — registre normatif d'exigences (contraintes, contrats, structures de données), consolidé en correspondance avec les sections techniques du cahier des charges |
| [dossier-architecture.md](dossier-architecture.md) | La **carte du système** — vues C4 (contexte, conteneurs, composants, déploiement) et choix de structure macro |
| [conception-detaillee.md](conception-detaillee.md) | Le **comment** — relations et dépendances entre décisions, séquences, règles consolidées |

Ces quatre documents sont des consolidations : aucun ne tranche de décision propre, chacun
renvoie à sa source d'origine (ADR, spécification, modèle de domaine) qui fait foi en cas de
divergence.

## Autre document du dossier

- [note-business.md](note-business.md) — cadrage du coût d'infrastructure, du dimensionnement de marché et de la stratégie go-to-market, à destination du responsable produit.

---

## Corpus détaillé sous-jacent

Ces quatre documents consolident un travail plus détaillé, tracé dans deux dossiers distincts :

- [../architecture/](../architecture/README.md) — décisions d'architecture (ADR), spécifications techniques pré-build, structure des projets.
- [../besoin/](../besoin/README.md), [../domaine/](../domaine/README.md), [../interface/](../interface/README.md) — vision produit, personas, use cases, user stories, user journeys, NFR, domaine DDD.

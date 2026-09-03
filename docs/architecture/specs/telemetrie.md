# Spec — Instrumentation par pilier, capture email, analytics RGPD

> **Nature** : spec pré-build (approche de report fidèle au corpus). Ce document reporte fidèlement ce qui est acté dans le corpus de décisions et nomme, sans les combler, les points laissés ouverts. Aucune valeur numérique, aucun nom d'outil, aucun mécanisme non explicitement acté dans le corpus n'est introduit ici.
>
> **Source normative** : [ADR-006 — Périmètre MVP](../decisions/ADR-006-perimetre-mvp.md), section « Compléments post-revue » (lignes 55-61). Contrainte RGPD de cohérence : [`cahier-des-charges.md` §7.4](../../context/cahier-des-charges.md#74-conformité-rgpd--protection-des-données) (ligne 808) et [NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md).
>
> **Lecteur visé** : l'équipe de build qui opérationnalisera l'instrumentation en entrée des jalons J1/J2/J3 (ADR-006:44), et le porteur produit, dont les décisions du 2026-09-03 ont levé les points qui bloquaient l'implémentation.
>
> **Statut** : les six points qui bloquaient le passage en développement sont tranchés (2026-09-03). Cette spec, fidèle à sa nature de report, **ne les tranche pas ici** — elle renvoie vers les documents qui font autorité : [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) pour les seuils, le transport et la capture de contact, [`vision-produit.md §2.3`](../../conception/besoin/vision/vision-produit.md) pour la limite de l'instrument. Reste ouvert le seul schéma d'événements formalisé, qui se dérive des décisions ci-dessus au moment de l'implémentation.

---

## 1. Pourquoi cette instrumentation existe

[ADR-006](../decisions/ADR-006-perimetre-mvp.md) acte un MVP unique, sans découpage en deux temps de livraison (préparation vs vue session). Cette décision a une conséquence assumée par le porteur produit : *« L'apprentissage produit n'est pas isolé : en cas d'adoption faible du MVP, il ne sera pas possible de distinguer l'échec de la préparation de celui de la vue session. »* (ADR-006, section Conséquences, ligne 42).

Pour compenser cette perte de signal, la revue adversariale post-ADR ajoute une exigence dans le périmètre MVP (ADR-006:55) :

> « Puisque le MVP unique ne sépare pas les hypothèses d'apprentissage, une télémétrie séparée par pilier est ajoutée dans le périmètre MVP […]. Cette instrumentation récupère l'essentiel de l'apprentissage que la décision de MVP unique assume de perdre. » (ADR-006:55,59)

Cette instrumentation n'est donc pas un ajout optionnel : elle est le mécanisme compensatoire explicitement chargé de reconstituer, après coup, ce que le découpage en deux temps de livraison aurait offert nativement.

---

## 2. Les trois piliers d'activation (verbatim ADR-006:57-59)

Le corpus définit exactement trois piliers, chacun avec son critère d'activation composite :

| Pilier | Critère d'activation (verbatim ADR-006) |
|---|---|
| **Préparation** | « Activation préparation : campagne créée + N documents créés. » (ADR-006:57) |
| **Vue session** | « Activation vue session : session ouverte + usage réel constaté. » (ADR-006:58) |
| **Partage** | « Activation partage : document partagé + au moins 1 joueur l'ayant ouvert. » (ADR-006:59) |

Chaque pilier est composite : la seule création (campagne créée / session ouverte / document partagé) ne suffit pas à qualifier l'activation. Un second événement, propre au pilier, doit survenir pour que le pilier soit considéré comme « activé ». C'est cette composition à deux temps que la spec d'instrumentation ci-dessous doit capturer événement par événement.

---

## 3. Spec d'instrumentation — pilier × événement × point de capture × propriétés

Le tableau ci-dessous décompose chaque pilier en événements discrets. Les colonnes « Événement technique », « Propriétés » et, pour le pilier vue session, l'opérationnalisation même du second critère, sont des trous — ils ne sont fixés nulle part dans le corpus actuel.

### 3.1 Pilier Préparation

| Étape du critère | Point de capture | Événement (nom technique) | Propriétés |
|---|---|---|---|
| Campagne créée | Point de sortie de la création de campagne (couche Application) | `[À TRANCHER — ticket]` | `space_id` (probable, non acté) — reste à confirmer |
| N documents créés | Point de sortie de la création de document, cumulé par campagne | `[À TRANCHER — ticket]` | `space_id`, compteur de documents — reste à confirmer |
| Seuil N | — | — | **Tranché (2026-09-03)** : N = **3 documents**. Geste structurant de l'espace personnel : un dossier créé, ou un document déplacé hors de « Non classés ». Renvoi seul — [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |

### 3.2 Pilier Vue session

| Étape du critère | Point de capture | Événement (nom technique) | Propriétés |
|---|---|---|---|
| Session ouverte | Point d'entrée en session (couche Application ou Presentation) | `[À TRANCHER — ticket]` | `session_id`, `space_id` — reste à confirmer |
| Usage réel constaté | — | point d'action en session | **Tranché (2026-09-03)** : au moins une action du MJ pendant la session — une note de session créée, un document épinglé, ou une scène naviguée. Une durée est explicitement écartée. Renvoi seul — [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |

### 3.3 Pilier Partage

| Étape du critère | Point de capture | Événement (nom technique) | Propriétés |
|---|---|---|---|
| Document partagé | Point de sortie de l'action de partage (couche Application) | `[À TRANCHER — ticket]` | `document_id`, `space_id`, visibilité de la ressource (cf. ADR-004:47) — reste à confirmer |
| ≥ 1 joueur l'ayant ouvert | Point d'ouverture du document partagé côté joueur/invité | `[À TRANCHER — ticket]` | `document_id`, identifiant de session invité (sans donnée identifiante au-delà de ce que permet NFR-CONF-02) — reste à confirmer |

### 3.4 Ce que la spec ne fixe pas

- **Le transport de la mesure est tranché (2026-09-03)** et rend la question de l'outil externe sans objet en mode local : les compteurs sont écrits dans le store local et **ne quittent le navigateur qu'à la création d'un compte**. Aucun appel sortant n'a lieu en mode local, ce qui préserve la directive `connect-src 'self'` et la garantie observable qu'elle porte. Renvoi seul — [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md).
- **La limite de ce choix est nommée** : les MJ qui ne créent jamais de compte ne transmettent jamais leurs compteurs, ce qui biaise H1 et rend H5 partiellement circulaire. Renvoi seul — [`vision-produit.md §2.3`](../../conception/besoin/vision/vision-produit.md).
- **La liste concrète des événements/propriétés** au sens d'un schéma formalisé (noms exacts, typage) reste à écrire — elle **se dérive** des trois critères d'activation ci-dessus et du transport tranché, sans nouvelle décision produit. `[À TRANCHER — ticket]`

---

## 4. Capture email non bloquante + analytics anonyme RGPD (périmètre MVP, dès le mode local)

ADR-006 ajoute, dans le même paragraphe de compléments post-revue, une exigence distincte des trois piliers d'activation :

> « Capture email non bloquante + analytics anonyme RGPD dès le mode local (aggravé par le MVP unique local-first) — dans le périmètre MVP. » (ADR-006:61)

Deux éléments à retenir fidèlement :

- **Champ d'application** : « dès le mode local » — cette exigence s'applique donc avant même la création d'un compte cloud, dans le contexte du MVP unique où le mode local et le mode cloud ne sont plus séparés en deux temps de livraison (ADR-006:22-26).
- **Deux volets distincts** : une capture d'email qui doit être « non bloquante » (le MJ n'est pas empêché de progresser s'il ne fournit pas d'email), et un dispositif d'analytics qualifié d'« anonyme RGPD ».

### 4.1 Ce qui est fixé

Rien au-delà du champ d'application et de la qualification (« non bloquante », « anonyme RGPD ») cité ci-dessus. Le corpus ne décrit ni le mécanisme de capture email, ni le moment précis de sollicitation, ni le traitement de l'email capturé.

### 4.2 Ce qui reste ouvert

- **Mécanisme de capture email non bloquante — tranché (2026-09-03)** : sollicitation unique, affichée **après** que le MJ a atteint l'activation préparation ; un refus ou une absence de réponse vaut refus définitif ; l'adresse part avec les compteurs à la création de compte et reste effaçable sur demande. Renvoi seul — [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md).
- **Traitement des données — tranché (2026-09-03)** par le transport : rien ne sort du navigateur avant la création d'un compte, moment où la donnée entre dans le périmètre de traitement du compte. Renvoi seul — [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md).

---

## 5. Contrainte RGPD de cohérence à reporter (déjà actée ailleurs)

Cette instrumentation n'est pas conçue en dehors du cadre de confidentialité déjà acté pour Haversack. Le cahier des charges est explicite sur ce point de cohérence :

> « L'instrumentation de validation du MVP — activation préparation, activation vue de session, activation partage […] — est conçue pour mesurer l'occurrence d'un usage sans capter le contenu narratif créé ou partagé par le MJ, cohérent avec l'isolation des données en mode local (NFR-CONF-02). » (`cahier-des-charges.md`:808)

[NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md) confirme ce même principe côté portée de l'exigence : *« Les mesures d'usage anonymes éventuelles : si de telles mesures sont mises en place, elles font l'objet d'un traitement distinct, sans lien avec le contenu local du MJ. »* (NFR-CONF-02, §Portée et hors-portée, ligne 39).

**Principe à reporter tel quel, non renégociable dans cette spec** : chaque événement décrit en §3 mesure une **occurrence** (un fait binaire ou un comptage — campagne créée, document créé, session ouverte, document ouvert) et ne capture à aucun moment le **contenu narratif** produit ou partagé par le MJ (texte des documents, notes, contenu de session). Toute implémentation qui ferait transiter du contenu narratif dans un événement de télémétrie contredirait NFR-CONF-02 et le principe déjà acté au cahier des charges.

Ce principe **contraint** la résolution des trous du §3 et du §4 — en particulier le choix des propriétés d'événement — sans les résoudre : le détail des propriétés d'événement reste `[À TRANCHER — ticket]` et devra être choisi de façon compatible avec cette contrainte. Le transport, lui, est tranché et la sert : rien ne quitte le navigateur avant la création d'un compte.

---

## 6. Récapitulatif — ce qui est tranché, ce qui reste

| # | Point | État | Référence |
|---|---|---|---|
| 1 | Valeur de N (pilier préparation) | **tranché** — N = 3 documents ; geste structurant nommé | ADR-006:57 ; [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |
| 2 | Opérationnalisation de « usage réel constaté » | **tranché** — au moins une action du MJ en session | ADR-006:58 ; [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |
| 3 | Outil analytics | **sans objet en mode local** — le transport tranché n'émet aucun appel sortant avant la création de compte | ADR-006:61 ; [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |
| 4 | Liste concrète des événements/propriétés (schéma formalisé) | ouvert — **se dérive** des cinq autres, sans décision produit supplémentaire | ADR-006:55-61 (absent) |
| 5 | Technique de traitement des données | **tranché** — rien ne quitte le navigateur avant la création de compte | ADR-006:61 ; [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |
| 6 | Mécanisme de capture de contact | **tranché** — sollicitation unique après activation, refus définitif | ADR-006:61 ; [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) |

Cinq de ces six points sont tranchés le 2026-09-03 — seuils, opérationnalisation de l'usage réel, transport de la mesure en lieu et place d'un outil externe, technique de traitement, et mécanisme de capture de contact. **Aucun n'est tranché *dans* ce document**, fidèlement à sa nature de report : ils le sont dans [`moscow.md § Instrumentation de validation du MVP`](../../conception/besoin/vision/moscow.md) et [`vision-produit.md §2.3`](../../conception/besoin/vision/vision-produit.md), vers lesquels les sections ci-dessus renvoient. Le sixième — le schéma d'événements formalisé — se dérive des cinq autres au moment de l'implémentation et ne demande aucune décision produit supplémentaire.

---

## 7. Traçabilité

| Artefact | Nature du lien |
|---|---|
| [ADR-006 — Périmètre MVP](../decisions/ADR-006-perimetre-mvp.md) | Source normative des trois piliers et de l'exigence email/analytics (lignes 55-61) |
| [`cahier-des-charges.md` §7.4](../../context/cahier-des-charges.md#74-conformité-rgpd--protection-des-données) | Reformulation synthèse du principe « occurrence sans contenu narratif » (ligne 808) |
| [NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md) | Exigence d'isolation des données en mode local ; place explicitement les « mesures d'usage anonymes » hors de sa propre portée, comme traitement distinct |

# Spec — Instrumentation par pilier, capture email, analytics RGPD

> **Nature** : spec pré-build (approche de report fidèle au corpus). Ce document reporte fidèlement ce qui est acté dans le corpus de décisions et nomme, sans les combler, les points laissés ouverts. Aucune valeur numérique, aucun nom d'outil, aucun mécanisme non explicitement acté dans le corpus n'est introduit ici.
>
> **Source normative** : [ADR-006 — Périmètre MVP](../decisions/ADR-006-perimetre-mvp.md), section « Compléments post-revue » (lignes 55-61). Contrainte RGPD de cohérence : [`cahier-des-charges.md` §7.4](../../context/cahier-des-charges.md#74-conformité-rgpd--protection-des-données) (ligne 808) et [NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md).
>
> **Lecteur visé** : l'équipe de build qui opérationnalisera l'instrumentation en entrée des jalons J1/J2/J3 (ADR-006:44), et le porteur produit qui devra trancher les points `[À TRANCHER]` avant que cette instrumentation puisse être implémentée.
>
> **Statut** : cadre à compléter — non implémentable en l'état, les trous nommés ci-dessous bloquent le passage en développement.

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
| Seuil N | — | — | **`[À TRANCHER — ticket]` : valeur de N non fixée (ADR-006:57 ne donne aucun chiffre)** |

### 3.2 Pilier Vue session

| Étape du critère | Point de capture | Événement (nom technique) | Propriétés |
|---|---|---|---|
| Session ouverte | Point d'entrée en session (couche Application ou Presentation) | `[À TRANCHER — ticket]` | `session_id`, `space_id` — reste à confirmer |
| Usage réel constaté | — | `[À TRANCHER — ticket]` | **`[À TRANCHER — ticket]` : « usage réel constaté » n'est pas opérationnalisé dans le corpus (ADR-006:58 pose le critère sans le définir en signal observable — durée minimale ? action MJ pendant la session ? interaction joueur ? aucun de ces choix n'est tranché)** |

### 3.3 Pilier Partage

| Étape du critère | Point de capture | Événement (nom technique) | Propriétés |
|---|---|---|---|
| Document partagé | Point de sortie de l'action de partage (couche Application) | `[À TRANCHER — ticket]` | `document_id`, `space_id`, visibilité de la ressource (cf. ADR-004:47) — reste à confirmer |
| ≥ 1 joueur l'ayant ouvert | Point d'ouverture du document partagé côté joueur/invité | `[À TRANCHER — ticket]` | `document_id`, identifiant de session invité (sans donnée identifiante au-delà de ce que permet NFR-CONF-02) — reste à confirmer |

### 3.4 Ce que la spec ne fixe pas

- **L'outil analytics** support de la collecte n'est pas nommé dans le corpus (ADR-006:61 mentionne « analytics anonyme RGPD » sans désigner de solution). `[À TRANCHER — ticket]`
- **La liste concrète des événements/propriétés** au sens d'un schéma d'événements formalisé (noms exacts, typage des propriétés) n'existe pas au-delà des trois critères d'activation ci-dessus. `[À TRANCHER — ticket]`
- **La technique d'anonymisation RGPD** appliquée à ces événements (pseudonymisation, agrégation, durée de rétention) n'est pas définie. `[À TRANCHER — ticket]`

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

- **Mécanisme de capture email non bloquante** : point de sollicitation dans le parcours, comportement si le MJ refuse ou ignore, traitement de la donnée collectée. `[À TRANCHER — ticket]` (ADR-006:61)
- **Outil et technique d'anonymisation RGPD** pour l'analytics — même trou que §3.4, seule l'exigence de résultat (« anonyme RGPD ») est actée, pas le moyen. `[À TRANCHER — ticket]` (ADR-006:61)

---

## 5. Contrainte RGPD de cohérence à reporter (déjà actée ailleurs)

Cette instrumentation n'est pas conçue en dehors du cadre de confidentialité déjà acté pour Haversack. Le cahier des charges est explicite sur ce point de cohérence :

> « L'instrumentation de validation du MVP — activation préparation, activation vue de session, activation partage […] — est conçue pour mesurer l'occurrence d'un usage sans capter le contenu narratif créé ou partagé par le MJ, cohérent avec l'isolation des données en mode local (NFR-CONF-02). » (`cahier-des-charges.md`:808)

[NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md) confirme ce même principe côté portée de l'exigence : *« Les mesures d'usage anonymes éventuelles : si de telles mesures sont mises en place, elles font l'objet d'un traitement distinct, sans lien avec le contenu local du MJ. »* (NFR-CONF-02, §Portée et hors-portée, ligne 39).

**Principe à reporter tel quel, non renégociable dans cette spec** : chaque événement décrit en §3 mesure une **occurrence** (un fait binaire ou un comptage — campagne créée, document créé, session ouverte, document ouvert) et ne capture à aucun moment le **contenu narratif** produit ou partagé par le MJ (texte des documents, notes, contenu de session). Toute implémentation qui ferait transiter du contenu narratif dans un événement de télémétrie contredirait NFR-CONF-02 et le principe déjà acté au cahier des charges.

Ce principe **contraint** la résolution des trous du §3 et du §4 — en particulier le choix des propriétés d'événement — sans les résoudre : le nom de l'outil analytics, la technique d'anonymisation et le détail des propriétés restent `[À TRANCHER — ticket]`, mais devront être choisis de façon compatible avec cette contrainte.

---

## 6. Récapitulatif des points à trancher

| # | Point ouvert | Référence corpus |
|---|---|---|
| 1 | Valeur de N (pilier préparation) | ADR-006:57 |
| 2 | Opérationnalisation de « usage réel constaté » (pilier vue session) | ADR-006:58 |
| 3 | Outil analytics | ADR-006:61 |
| 4 | Liste concrète des événements/propriétés (schéma formalisé) | ADR-006:55-61 (absent) |
| 5 | Technique d'anonymisation RGPD | ADR-006:61 |
| 6 | Mécanisme de capture email non bloquante | ADR-006:61 |

Ces six points bloquent le passage de cette spec à un ticket de développement. Aucun n'est tranché dans ce document.

---

## 7. Traçabilité

| Artefact | Nature du lien |
|---|---|
| [ADR-006 — Périmètre MVP](../decisions/ADR-006-perimetre-mvp.md) | Source normative des trois piliers et de l'exigence email/analytics (lignes 55-61) |
| [`cahier-des-charges.md` §7.4](../../context/cahier-des-charges.md#74-conformité-rgpd--protection-des-données) | Reformulation synthèse du principe « occurrence sans contenu narratif » (ligne 808) |
| [NFR-CONF-02](../../conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md) | Exigence d'isolation des données en mode local ; place explicitement les « mesures d'usage anonymes » hors de sa propre portée, comme traitement distinct |

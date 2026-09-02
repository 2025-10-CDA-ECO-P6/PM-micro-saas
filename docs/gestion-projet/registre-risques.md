# Registre d'évaluation des risques — Haversack

> **Statut** : v1 — évaluation baseline, à valider en comité des risques.
> **Référentiel méthodologique** : ISO 31000 (management du risque — principes et lignes directrices).
> **Audience** : pilotage de projet et présentation à un tiers (investisseur, auditeur, délégué à la protection des données), équipe de build comme référence de suivi.
> **Date de production** : 2026-07-16.

---

## Ce que ce document fait et ne fait pas

Ce document **consolide et évalue** des risques déjà identifiés dans le corpus de conception du projet (décisions d'architecture, spécifications techniques, cahier des charges, roadmap d'entrée en build, cadrage juridique). Il n'en **invente aucun** : chaque entrée renvoie à sa source d'origine, qui fait foi en cas de désaccord de lecture avec la formulation retenue ici. Ce registre est un artefact de synthèse transverse ; il ne constitue pas une nouvelle autorité de conception et ne renverse l'ordre d'autorité d'aucune décision citée.

Ce document **ne se substitue pas** à un futur dossier RGPD consolidé ni à une éventuelle analyse d'impact relative à la protection des données. Ces derniers, s'ils sont produits, examineront chaque traitement de données en détail ; ce registre garde délibérément la **vue risque** — probabilité, impact, traitement, criticité, statut — transverse aux quatre axes (technique, produit, juridique, délai-coût), sans prétendre à l'exhaustivité d'une analyse d'impact dédiée.

Ce document ne tranche aucun seuil non fixé par le corpus source. Les valeurs différées à l'implémentation par le corpus source sont reprises telles que documentées (le cas échéant encore `[À TRANCHER]` dans leur décision d'origine, voir par exemple T-03), mais ce registre leur applique systématiquement une **cotation typée** (probabilité/impact pour une entrée `risque`, classe de conséquence pour les autres types) et le **statut de validation** « brouillon expert — à valider en comité des risques » (§1.5) — aucune entrée de ce registre ne reste au marqueur `[À AFFINER]` d'une version antérieure. Il ne redéfinit ni le périmètre de priorisation fonctionnelle ni le phasage macro du projet, tous deux renvoyés à leurs artefacts de référence respectifs (roadmap d'entrée en build et cahier des charges — voir Annexe A).

**En cas de conflit entre ce registre et une source citée, la source citée fait foi.**

**Réserve de méthode** (détail en Annexe B) : une partie des risques de l'axe délai-coût est tracée principalement au document qui les cite plutôt que par réouverture systématique de chaque décision source ; de même, la provenance de certains points juridiques n'a pas été recoupée à une source primaire au-delà de la mention documentée dans le calendrier juridique. Ces traçabilités sont légitimes pour un exercice de consolidation mais sont de force de preuve moindre qu'une citation directe à la décision d'origine — elles doivent être rouvertes à la source avant toute décision engageante (arbitrage financier, ratification opérateur, engagement contractuel).

---

## 1. Méthodologie

### 1.0 Jalons de la trajectoire de build

Le vocabulaire de jalons utilisé comme valeur d'Échéance sur la quasi-totalité des entrées de ce registre (« socle », « local-only », « cloud + migration », « partage temps réel », « post-MVP ») est nommé et séquencé ci-dessous, pour qu'un tiers sans accès au dépôt puisse reconstituer l'ordre sans ouvrir un autre document. Séquence et contenu dérivés de la roadmap d'entrée en build (voir Annexe A) — nommage par contenu, sans date ni numérotation de phase.

```
socle
  │
local-only
  │
[gate marché]
  │
cloud + migration
  │
[gate juriste EU]
  │
partage temps réel
```

- **Socle** : confirmation datée des décisions d'architecture pré-implémentation, squelette du domaine applicatif, test d'architecture en intégration continue.
- **Local-only** : projection du domaine dans le mode de fonctionnement local (stockage navigateur, interface minimale), sans aucune brique cloud.
- **[Gate marché]** : décision d'opérateur go/no-go sur la télémétrie du jalon local, avant d'engager le build lourd cloud et temps réel — gate humain, non vérifiable en build.
- **Cloud + migration** : mapping vers le stockage serveur, authentification, migration des données locales, cascade d'effacement RGPD.
- **[Gate juriste EU]** : validation par avis juriste sur les axes RGPD, condition du lancement commercial dans l'Union européenne — gate humain, non vérifiable en build ; ne bloque pas le build du jalon suivant, qui peut être construit en parallèle de l'instruction juridique.
- **Partage temps réel** : diffusion en temps réel, accès joueur invité, protection contre la fuite de contenu entre espaces.
- **Post-MVP** : fonctionnalités et dettes explicitement reportées après la première livraison, hors séquence des jalons ci-dessus.

### 1.1 Typologie des entrées

Chaque entrée du registre porte un **type**, qui détermine la nature de sa cotation. Confondre les types reviendrait à coter arbitrairement ce que le corpus ne permet pas de coter :

| Type | Nature | Cotation |
|---|---|---|
| **Risque** | Aléa incertain | Probabilité × Impact → criticité inhérente et résiduelle. Seul type porté sur la matrice de criticité et le classement des risques prioritaires. |
| **Hypothèse** | Pari de validation produit, à confirmer par la mesure | Aucune probabilité cotée. Porte une classe de conséquence si elle n'est pas confirmée, ainsi que le dispositif de suivi (instrumentation, entretiens). Vue dédiée, hors matrice. |
| **Gate / décision** | Point de contrôle go/no-go | Hors échelle. Porte une classe de conséquence et le dispositif de décision. Vue dédiée, hors matrice. |
| **Dette / arbitrage accepté** | Choix délibérément assumé par l'opérateur ou l'équipe de conception | La probabilité n'est pas pertinente (le choix est acté, pas aléatoire) : seul l'impact conditionnel, le risque résiduel et la condition de réactivation sont cotés. Hors matrice des risques actifs. |
| **Question ouverte** | Point de conformité ou de viabilité non tranché, non maîtrisable en interne (dépend d'un avis juriste ou d'une donnée de marché) | Aucune probabilité inventée. Porte l'impact sous forme de **classe de conséquence factuelle** et le traitement — c'est-à-dire le dispositif qui doit lever le point. Vue dédiée. |

Cette typologie est appliquée strictement à chacune des 69 entrées du registre (section 3). Pour les entrées non typées `risque`, aucune probabilité n'est cotée même lorsque le corpus source portait initialement une valeur chiffrée : seule la classe de conséquence et le dispositif de traitement sont retenus, conformément au principe de zéro cotation inventée.

### 1.2 Échelles de cotation

**Probabilité** (horizon : de l'entrée en build au lancement commercial dans l'Union européenne) :

| Niveau | Valeur |
|---|---|
| Faible | 1 |
| Modérée | 2 |
| Élevée | 3 |
| Quasi-certaine | 4 |

**Impact** :

| Niveau | Valeur |
|---|---|
| Mineur | 1 |
| Modéré | 2 |
| Majeur | 3 |
| Critique | 4 |

**Criticité** = Probabilité × Impact :

| Bande de criticité | Score | 
|---|---|
| Faible | 1 à 2 |
| Moyenne | 3 à 4 |
| Élevée | 6 à 9 |
| Très élevée | 12 à 16 |

**Scores atteignables** : les échelles de probabilité et d'impact étant des entiers 1 à 4, seuls les produits {1, 2, 3, 4, 6, 8, 9, 12, 16} sont arithmétiquement atteignables. Les scores 5, 7, 10, 11, 13, 14 et 15 sont inatteignables par construction — leur absence de toute grille ou matrice de ce registre n'est donc pas un signal, c'est une conséquence de l'échelle à 4 niveaux.

**Gabarit de la matrice 4×4** (grille de méthode, vide — distincte de la heat-map peuplée des risques réels en §2.1) :

| Probabilité \ Impact | Mineur (1) | Modéré (2) | Majeur (3) | Critique (4) |
|---|---|---|---|---|
| **Quasi-certaine (4)** | 4 | 8 | 12 | 16 |
| **Élevée (3)** | 3 | 6 | 9 | 12 |
| **Modérée (2)** | 2 | 4 | 6 | 8 |
| **Faible (1)** | 1 | 2 | 3 | 4 |

### 1.3 Rubrique d'ancrage d'impact par dimension

Un même niveau d'impact ("Majeur", par exemple) recouvre des réalités différentes selon la dimension touchée. La rubrique suivante fixe des descripteurs de référence par dimension, pour garantir la reproductibilité de la cotation d'une revue à l'autre :

| Niveau | Sécurité & données personnelles | Conformité RGPD | Valeur produit | Coût & délai |
|---|---|---|---|---|
| **Mineur** | Faiblesse théorique, sans exposition réaliste | Écart de forme, corrigeable sans effet sur les personnes concernées | Gêne contournable | Retard localisé, effort marginal |
| **Modéré** | Exposition limitée (cas de bord, poste partagé) | Non-conformité de forme, corrigeable avant le lancement | Fonctionnalité secondaire affaiblie | Dette ou retard notable, sans blocage de jalon |
| **Majeur** | Fuite de données personnelles plausible, ou prise de contrôle de compte | Non-conformité substantielle (base légale, droits des personnes) exposant à une sanction, mais corrigeable | Blocage d'un pilier fonctionnel ou d'un jalon | Blocage d'un jalon, surcoût significatif |
| **Critique** | Compromission systémique des données | Non-conformité bloquant le lancement dans l'Union européenne, ou sanction majeure | Invalidation de la proposition de valeur du produit | Remise en cause de la viabilité du projet |

Chaque entrée de type `risque` porte sa **dimension d'impact dominante** — la dimension qui a déterminé le niveau retenu. Cette information permet de filtrer, par exemple, les seuls risques à dominante de conformité RGPD.

**Plafond observé** : aucun risque de nature technique ou délai-coût n'atteint le niveau "Critique" dans cette évaluation — ce niveau est réservé aux conséquences d'invalidation de la proposition de valeur produit ou de blocage du lancement/de la viabilité du projet. Les risques de prise de contrôle de compte et les risques liés à l'obligation d'effacement (article 17 du RGPD) plafonnent à "Majeur". Ce plafonnement est intentionnel et cohérent avec la rubrique d'ancrage ci-dessus.

### 1.4 Convention inhérent / résiduel

Chaque entrée de type `risque` porte **deux valeurs de criticité** :

- **Criticité inhérente** (état réel actuel) : Probabilité × Impact **sans déduire l'effet des mitigations non encore effectivement en place**. Une mitigation "conçue mais non certifiée" (par exemple une saga d'effacement, une sanitisation anti-injection, un test d'architecture en intégration continue, un invariant de validation non outillé) **n'abaisse pas** la criticité inhérente : celle-ci reste au niveau réel d'aujourd'hui.
- **Criticité résiduelle cible** : criticité obtenue après application effective du traitement planifié. Chaque entrée précise si la mitigation est *en place*, *conçue mais non certifiée*, ou *à définir*.

### 1.5 Statut de validation, tendance, confiance

- **Statut de validation** : toutes les entrées de cette version portent le statut **« brouillon expert — à valider en comité des risques »**. Aucune entrée n'a fait l'objet d'une ratification formelle à la date de production.
- **Tendance** : toutes les entrées portent la valeur **« nouveau (baseline v1) »** — cette colonne existe pour être peuplée aux revues suivantes ; aucune tendance n'est fabriquée à ce stade faute d'historique.
- **Confiance** : deux valeurs possibles. **Ferme** (la cotation est directement dérivable du corpus, sans dépendance à un jugement externe) ou **sous réserve — à confirmer par [rôle]** (la cotation dépend d'un jugement que le projet ne maîtrise pas seul : avis juriste, réaction du marché, comportement d'un fournisseur d'identité tiers, coût d'hébergement réel, comportement d'un navigateur). Pour les entrées de type `question ouverte`, la confiance "sous réserve" est la norme : l'impact y est une classe de conséquence, non une probabilité.
- **`[NON-VÉRIFIABLE-EN-BUILD]`** : tag distinct du champ Confiance, jamais fusionné dans sa valeur. Porté sur toute entrée dont la résolution dépend d'un jugement humain externe (décision d'investissement, avis juriste, comportement d'un tiers) et qu'aucun test automatisé, aucune revue de code, aucune exécution de la solution ne peut donc trancher. Affiché sous ce rendu uniforme sur toutes les entrées concernées (T-03, T-25, T-26, P-01, J-01, axe juridique — voir §3.3).

### 1.6 Appétence au risque et seuil d'escalade

L'appétence au risque proposée pour ce projet, cohérente avec un produit en phase d'entrée en construction (MVP à hypothèses de valeur non confirmées) : tolérance élevée aux risques de dette technique et de délai contenus sous la bande "Élevée" tant qu'ils ne bloquent pas un jalon ; tolérance faible aux risques de sécurité et de conformité RGPD dès la bande "Élevée" ; tolérance nulle à tout risque atteignant "Critique".

| Bande de criticité | Seuil d'escalade proposé | Destinataire |
|---|---|---|
| Faible | Suivi de routine, revue à la prochaine échéance de jalon | Propriétaire de l'entrée |
| Moyenne | Suivi actif, mention au point d'avancement de jalon | Propriétaire de l'entrée + direction de projet |
| Élevée | Escalade immédiate, plan de traitement daté requis avant la clôture du jalon concerné | Direction de projet ; DPO si dimension dominante = conformité RGPD |
| Très élevée | Escalade immédiate à l'opérateur, blocage de facto du jalon tant que le traitement n'est pas engagé | Opérateur |

Ce cadre d'appétence et ces seuils sont une proposition méthodologique de cette version, à valider par le comité des risques (voir section 4).

### 1.7 Légende des colonnes du registre détaillé (section 3)

Pour une entrée `risque` : identifiant, intitulé, type, description, probabilité, impact et dimension dominante, criticité inhérente, criticité résiduelle, traitement, mitigation, propriétaire, échéance, statut de validation, tendance, confiance, source, recoupements (le cas échéant).

Pour une entrée `hypothèse` / `gate-décision` / `question ouverte` / `dette-arbitrage accepté` : identifiant, intitulé, type, description, classe de conséquence, dispositif / traitement, propriétaire, échéance, statut de validation, confiance, source, recoupements (le cas échéant).

Le tag `[NON-VÉRIFIABLE-EN-BUILD]` (§1.5), lorsqu'il s'applique, est affiché distinctement du champ Confiance — jamais comme valeur de ce champ.

---

## 2. Synthèse exécutive

### 2.1 Matrice de criticité inhérente (risques uniquement)

Seules les 32 entrées de type `risque` figurent sur cette matrice. Les 37 autres entrées (hypothèses, gates, dettes, questions ouvertes) sont présentées dans leurs vues dédiées ci-dessous.

| Probabilité \ Impact | Mineur | Modéré | Majeur | Critique |
|---|---|---|---|---|
| **Quasi-certaine** | — | D-01 | T-01, D-02 | — |
| **Élevée** | — | T-14 | T-02, T-05, T-07, T-16, P-08, P-14, D-03 | — |
| **Modérée** | T-06, T-29 | T-03, T-12, T-15, T-19, T-25, D-04, D-05 | T-04, T-10 | — |
| **Faible** | T-09 | T-08, T-11, T-13, T-17, T-28, D-08 | T-23, T-24, T-26 | — |

Aucune entrée n'atteint la colonne "Critique" (voir plafond observé, §1.3). Ce plafond ne vaut que pour les entrées de type `risque` portées par cette matrice : le registre porte par ailleurs un unique impact de niveau critique, celui de l'hypothèse centrale H2 (P-03) — hors matrice par construction, puisqu'une `hypothèse` n'est pas cotée en probabilité (§1.1). Voir la vue Hypothèses (§2.3).

### 2.2 Top risques (criticité inhérente Élevée ou Très élevée)

Table exhaustive : les 13 entrées de type `risque` dont la criticité inhérente est de bande Élevée (score 6 à 9) ou Très élevée (score 12 à 16) — voir §1.2 pour les bandes. Règle d'inclusion uniforme : même score de criticité inhérente → même rang (ex. T-04, T-10 et T-14 partagent le score 6 et le rang 11). La colonne Dimension dominante reprend le descripteur porté par chaque entrée en section 3, et permet le filtrage par dimension annoncé en §1.3 (par exemple, ne retenir que les entrées à dominante « Conformité RGPD »).

| Rang | Identifiant | Intitulé | Dimension dominante | Criticité inhérente | Criticité résiduelle cible |
|---|---|---|---|---|---|
| 1 | T-01 | `navigator.storage.persist()` non résolu avant le socle local | Valeur produit | Très élevée | Moyenne |
| 1 | D-02 | Stabilisation du socle et confirmation des décisions d'architecture pré-implémentation | Coût & délai | Très élevée | Moyenne |
| 3 | T-02 | Promotion de colonnes préalable au cloud (identifiants de personnage / d'accès invité) | Sécurité & données personnelles | Élevée | Faible |
| 3 | T-05 | Câblage de l'autorisation, test d'architecture et frontières logiques de contexte | Sécurité & données personnelles | Élevée | Moyenne |
| 3 | T-07 | Cascade d'effacement RGPD : complexité du graphe et reprise après incident | Conformité RGPD | Élevée | Moyenne |
| 3 | T-16 | Filtres de requête (soft-delete par jointure) non arbitrés | Conformité RGPD | Élevée | Moyenne |
| 3 | P-08 | Instrumentation d'activation non opérationnalisable en l'état | Valeur produit | Élevée | Moyenne |
| 3 | P-14 | Dépendance à la cohorte pilote non traitée | Valeur produit | Élevée | Moyenne |
| 3 | D-03 | Dépendance cumulative socle local stable et gate marché favorable | Coût & délai | Élevée | Moyenne |
| 10 | D-01 | Double référent de nomenclature de jalon (résolu) et ancrages non littéraux (résiduel) | Coût & délai | Élevée | Moyenne |
| 11 | T-04 | Reclaim-in-place non ratifié `[À RATIFIER OPÉRATEUR]` | Sécurité & données personnelles | Élevée | Moyenne |
| 11 | T-10 | Valeurs de seuils de limitation de débit non fixées | Sécurité & données personnelles | Élevée | Moyenne |
| 11 | T-14 | Schéma de propriétés des types de document système non modélisé | Coût & délai | Élevée | Élevée (aucune mitigation actée) |

T-14 n'est pas un cas isolé : 15 autres entrées de type `risque` partagent la même caractéristique — une criticité résiduelle qui reste au niveau de l'inhérente faute de mitigation actée abaissant le score (T-03, T-06, T-08, T-09, T-11, T-12, T-13, T-15, T-17, T-19, T-24, T-25, T-28, T-29, D-08) —, soit 16 entrées sur les 32 que compte la matrice des risques (§2.1). Aucune de ces 15 autres n'atteint la bande Élevée ou Très élevée (elles restent en bande Faible ou Moyenne), ce qui explique qu'elles ne figurent pas dans la table ci-dessus ; seule T-14 cumule bande Élevée et résiduelle non abaissée.

### 2.3 Vue Hypothèses de validation

Cinq hypothèses de validation produit (H1 à H5), aucune n'est cotée en probabilité — voir typologie §1.1. Toutes portent l'échéance "gate marché" et le propriétaire "responsable produit".

| Identifiant | Hypothèse | Seuil et délai | Classe de conséquence si infirmée |
|---|---|---|---|
| P-02 | H1 — activation (préparation) | ≥ 60 % de la cohorte, 14 jours. *Nuance (décision opérateur du 2026-07-09) : le contenu d'un espace personnel sans campagne ne compte que **partiellement** dans cette activation, s'il traduit un **geste structurant** ; seuil exact `[À TRANCHER — métrique produit]`.* | Activation de base compromise |
| P-03 | **H2 — vue de session (hypothèse centrale)** | ≥ 50 % à l'activation, répétabilité ≥ 50 % sur deux sessions ou plus, 30 et 60 jours | **Invalidation de la proposition de valeur du produit dans son ensemble** — seule classe de conséquence de niveau "critique" de ce registre |
| P-04 | H3 — fluidité du partage | ≥ 40 %, 60 jours | Affaiblissement du vecteur de croissance et de différenciation |
| P-05 | H4 — accès joueur sans compte | ≥ 70 %, 60 jours | Un frein à l'entrée du joueur invité compromet l'adoption du groupe entier |
| P-06 | H5 — conversion local vers compte | ≥ 10 %, 90 jours | Limite l'échelle de conversion, sans remettre en cause la valeur en usage local |

### 2.4 Vue Gates (points de décision go/no-go)

| Identifiant | Gate | Classe de conséquence | Vérifiabilité |
|---|---|---|---|
| P-01 | Gate marché — avant le build lourd (cloud, migration, partage temps réel) | Engage le capital du build lourd ; un no-go arrête la phase cloud, pas le socle local | `[NON-VÉRIFIABLE-EN-BUILD]` — décision d'opérateur sur interprétation de télémétrie |
| J-01 | Gate juriste EU — validation pré-lancement sur 5 axes RGPD | Bloque le lancement commercial dans l'Union européenne, pas le build technique | `[NON-VÉRIFIABLE-EN-BUILD]` — dépend d'un avis juriste externe |

Le gate juriste EU est un **indicateur agrégé** des questions ouvertes J-02 à J-05 et de la facette RGPD du risque de reprise de compte fédérée (voir T-04) — il ne s'additionne pas à ces entrées dans une synthèse de criticité, il les enveloppe.

### 2.5 Points à ratifier, dettes majeures et questions ouvertes à lacune réelle

| Catégorie | Identifiant | Point |
|---|---|---|
| À ratifier par l'opérateur | T-04 | Reclaim-in-place (identité fédérée reprenant une coquille de compte non vérifiée) |
| Résolu, résiduel à confirmer | D-01 | Double référent de nomenclature de jalon — **résolu** (axe canonique J0-J3 ratifié par l'opérateur, 2026-09-01) ; ancrage inféré `P0.5` résiduel, à confirmer |
| Dette majeure (résiduel non abaissé) | D-07 | Compression du calendrier de build, charge concentrée en amont |
| Dette majeure (résiduel non abaissé) | P-07 | Apprentissage produit non isolé, couplé à P-08 |
| Question ouverte à lacune réelle | J-15 | Catégories particulières de données (article 9 RGPD) incidentes, non évaluées |
| Question ouverte à lacune réelle | J-16 | Procédure de notification de violation de données non actée |
| Question ouverte à lacune réelle | J-12 | Méthode de vérification d'identité des invités non retenue |

Ces trois questions ouvertes juridiques se distinguent des quatorze autres de l'axe juridique par l'absence de toute mesure actée dans le corpus, même provisoire — les autres portent au moins une posture ou un squelette en attente de confirmation.

`[À TRANCHER — COMITÉ DES RISQUES]` Cette frontière est un jugement de degré, pas une ligne nette, pour **J-12** : son entrée (§3.3) porte « un principe de proportionnalité posé », structurellement comparable aux postures jugées suffisantes ailleurs sur l'axe juridique (par exemple J-04, « des bases légales par défaut sont posées »). **J-15** et **J-16**, eux, ne portent clairement aucune posture. Ce registre ne tranche pas si J-12 relève de la « lacune réelle » ou de la « posture existante » — la question est signalée ici pour statuer en comité des risques, sans reclassement de J-12 par ce registre.

---

## 3. Registre détaillé par axe

Chaque entrée porte un identifiant propre à ce registre (`T-`, `P-`, `J-`, `D-`), conservé depuis la version précédente du registre.

### 3.1 Axe technique (T-01 à T-29)

**T-01 — `navigator.storage.persist()` non résolu avant le socle local**
*Type : risque.*
- **Description** : l'appel à l'API de persistance du stockage navigateur est un préalable bloquant confirmé pour l'entrée dans le socle local. Sans garantie de stockage persistant, une perte de données dès le premier contact utilisateur est possible.
- **Probabilité** : Quasi-certaine — le préalable n'est pas encore levé et se présente par construction à chaque entrée en mode local.
- **Impact** : Majeur — dimension dominante : valeur produit (perte de données au premier contact si le stockage n'est pas garanti persistant).
- **Criticité inhérente** : Très élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : le résultat de l'appel (accordé ou refusé en best-effort) est traité comme un état de première classe, avec un bandeau de durabilité affiché si le stockage n'est pas garanti.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle, avant l'entrée dans le jalon local.
- **Statut de validation** : brouillon expert — à valider en comité des risques.
- **Tendance** : nouveau (baseline v1).
- **Confiance** : sous réserve — à confirmer par le comportement d'octroi du navigateur.
- **Source** : décision d'architecture sur l'exécution du domaine en mode local ; décision d'architecture sur le modèle IndexedDB local ; roadmap d'entrée en build.

**T-02 — Promotion de colonnes préalable au cloud (identifiants de personnage et d'accès invité)**
*Type : risque.*
- **Description** : deux champs actuellement portés en propriétés libres doivent être promus en colonnes indexées de premier niveau avant le jalon cloud, ce préalable schéma débloquant l'invariant d'autorisation.
- **Probabilité** : Élevée — préalable schéma décidé mais non encore implémenté.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles (débloque l'invariant d'autorisation nécessaire au cloud).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : promotion des champs vers des colonnes nullable indexées de premier niveau, déjà actée en conception.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la gouvernance documentaire ; roadmap d'entrée en build.

**T-03 — Seuil de repli et coût d'hébergement temps réel non fixés**
*Type : risque.*
- **Description** : le principe de repli du transport temps réel (basculement vers un mode dégradé en cas de dépassement de seuil) est acté, mais sa cadence et son seuil de déclenchement restent ouverts, avec un risque de dérive de coût d'hébergement si cela n'est pas fixé avant la mise en production.
- **Probabilité** : Modérée — principe acté, paramètres non fixés.
- **Impact** : Modéré — dimension dominante : coût & délai.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : aucune valeur actée à ce stade ; le principe de repli lui-même est acté, sa cadence et son seuil restent `[À TRANCHER]`.
- **Propriétaire** : Direction de projet / Architecte.
- **Échéance** : bloquant pour la clôture opérationnelle du jalon partage temps réel, non bloquant pour son entrée.
- **Confiance** : sous réserve — à confirmer par le coût d'hébergement réel et le comportement du fournisseur d'infrastructure. `[NON-VÉRIFIABLE-EN-BUILD]`
- **Source** : spécification du repli temps réel ; décision d'architecture sur le transport temps réel ; roadmap d'entrée en build.

**T-04 — Reclaim-in-place non ratifié (identité fédérée reprenant une coquille de compte non vérifiée)** `[À RATIFIER OPÉRATEUR]`
*Type : risque.*
- **Description** : lorsqu'un utilisateur s'authentifie via un fournisseur d'identité externe avec un email correspondant à un compte préexistant non vérifié, une opération de reprise de cette coquille est proposée mais non encore ratifiée par l'opérateur. Une résolution mal bornée serait apparentée à une prise de contrôle de compte anticipée.
- **Probabilité** : Modérée — la résolution proposée n'est pas encore ratifiée.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : gate applicatif proposé avec bascule de l'indicateur d'email vérifié, neutralisation du justificatif préexistant, liaison fédérée ; des alternatives ont été écartées et documentées (refus non-silencieux, email synthétique, suppression de la coquille).
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration, si la résolution proposée est retenue.
- **Confiance** : sous réserve — à confirmer par ratification opérateur.
- **Source** : décision d'architecture sur la sécurité de l'authentification ; roadmap d'entrée en build.
- **Recoupements** : cette entrée porte trois faces distinctes, nommées ici plutôt que dupliquées en trois risques — la résolution technique elle-même (ci-dessus), le sort du contenu de la coquille non vérifiée évincée par la reprise, qui relève du périmètre juridique et est enveloppé par le gate juriste EU (J-01), et son statut de point à ratifier par l'opérateur dans le pilotage de l'entrée en build.

**T-05 — Câblage de l'autorisation, test d'architecture et frontières logiques de contexte**
*Type : risque.*
- **Description** : l'autorisation doit être centralisée dans un contrat applicatif unique, appelé identiquement par le pipeline REST et par le filtre de diffusion temps réel. Un test d'architecture en intégration continue est prévu comme garde-fou, remplaçant une discipline de revue de code jugée insuffisante ; sa définition exhaustive (couverture complète des gestionnaires de requêtes et des jetons) reste renvoyée à une passe ultérieure. Une alternative d'isolation physique par module a été écartée au profit de frontières logiques.
- **Probabilité** : Élevée — câblage préalable, test d'architecture à définir exhaustivement (→ B3.2).
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles (un gestionnaire de requête oublié constituerait une référence directe non protégée à un objet).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : centralisation de l'autorisation dans un contrat applicatif unique ; test d'architecture en intégration continue prévu comme livrable du socle.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle, puis local-only pour le câblage ; définition exhaustive du test d'architecture renvoyée à une passe ultérieure.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le modèle d'autorisation API ; décision d'architecture sur l'autorisation RGPD ; décision d'architecture sur la structure de la solution ; roadmap d'entrée en build.

**T-06 — Résidu de nommage de l'espace de stockage local**
*Type : risque.*
- **Description** : le renommage de la notion de campagne vers celle d'espace a été propagé sur le corpus de conception fonctionnelle (domaine, glossaire, cas d'usage), mais la dénomination technique de l'espace de stockage local n'est pas confirmée cohérente.
- **Probabilité** : Modérée — résidu de nommage présent dans la documentation technique.
- **Impact** : Mineur — dimension dominante : coût & délai.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : point de vigilance signalé pour l'implémentation du service concerné ; pas un arbitrage différent, un résidu de rédaction à corriger.
- **Propriétaire** : Lead technique.
- **Échéance** : local-only.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le modèle IndexedDB local ; décision d'architecture sur la généralisation de l'espace ; roadmap d'entrée en build.

**T-07 — Cascade d'effacement RGPD (article 17) : complexité du graphe et reprise après incident**
*Type : risque.*
- **Description** : la cascade d'effacement (38 clés étrangères, deux cycles de relations, 17 relations inter-modules) est portée par une saga applicative explicite, idempotente, avec un claim exclusif et un mécanisme de reprise en cas d'expiration du claim sans purge terminée. Le seuil d'expiration du claim reste ouvert, de même que le traitement des références nullable entrantes non couvertes par le critère de partage, avec un risque de blocage de la saga si ce dernier point n'est pas résolu.
- **Probabilité** : Élevée — mécanisme conçu mais non certifié en exécution réelle, deux points ouverts.
- **Impact** : Majeur — dimension dominante : conformité RGPD (un manquement sur cette cascade expose à un manquement de l'obligation d'effacement).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : saga applicative explicite et idempotente ; la certification de cette saga fait partie du critère de sortie du jalon cloud + migration.
- **Propriétaire** : Architecte / Lead technique pour le mécanisme ; délégué à la protection des données pour la face juridique (voir recoupement).
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la cascade d'intégrité référentielle ; spécification de configuration de sécurité et de migration ; spécification de la requête d'effacement non partagé ; spécification du mapping EF Core.
- **Recoupements** : voir J-05 (qualification juridique du hard-delete inconditionnel de l'espace personnel) — face légale du même mécanisme ; l'une implémente, l'autre qualifie.

**T-08 — Seuil minimal de version de schéma serveur non fixé**
*Type : risque.*
- **Description** : le seuil minimal de version de schéma accepté côté serveur n'est pas fixé.
- **Probabilité** : Faible — rejet propre déjà acté, seule la valeur seuil est ouverte.
- **Impact** : Modéré — dimension dominante : coût & délai.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : rejet propre avec code d'erreur dédié acté en cas de version non supportée.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sérialisation locale et la migration ; spécification de configuration de sécurité et de migration.

**T-09 — Horizon de rétention de l'identifiant de lot de migration non fixé**
*Type : risque.*
- **Description** : la durée de conservation de l'identifiant de lot utilisé pour l'idempotence de migration n'est pas fixée.
- **Probabilité** : Faible — idempotence scopée par utilisateur déjà actée, indépendante de l'horizon de rétention.
- **Impact** : Mineur — dimension dominante : coût & délai.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Accepter.
- **Mitigation** : idempotence scopée par utilisateur actée, indépendamment de l'horizon retenu.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sérialisation locale et la migration.

**T-10 — Valeurs de seuils de limitation de débit non fixées**
*Type : risque.*
- **Description** : les valeurs précises des seuils de limitation de débit (protection contre la force brute sur l'authentification) ne sont pas fixées.
- **Probabilité** : Modérée — principe et borne supérieure actés, valeurs précises ouvertes.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles (exposition à une attaque par force brute si les seuils tardent à être fixés).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : principe hybride (limitation par adresse IP et par compte) et borne supérieure actés.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sécurité de l'authentification ; spécification de configuration de sécurité et de migration.

**T-11 — Durée de vie du jeton de réinitialisation et paramètres de hachage non fixés**
*Type : risque.*
- **Description** : les valeurs précises de durée de vie du jeton de réinitialisation de mot de passe et de coût de hachage ne sont pas fixées, bien que des bornes dures soient déjà actées.
- **Probabilité** : Faible — bornes dures actées (jeton ≤ 15 minutes, coût de hachage ≥ 12).
- **Impact** : Modéré — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : bornes dures actées.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : spécification de configuration de sécurité et de migration ; décision d'architecture sur la sécurité de l'authentification.

**T-12 — Fraîcheur de ré-authentification du fournisseur d'identité non fixée**
*Type : risque.*
- **Description** : la fraîcheur maximale acceptée pour une ré-authentification auprès du fournisseur d'identité fédéré n'est pas fixée, apparentée à une fenêtre d'authentification expirée mal contrôlée.
- **Probabilité** : Modérée — exigence actée en principe, fraîcheur non fixée.
- **Impact** : Modéré — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : exigence de ré-authentification actée dans son principe.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : sous réserve — à confirmer par le comportement du fournisseur d'identité.
- **Source** : décision d'architecture sur la sécurité de l'authentification.

**T-13 — Choix du code de réponse pour un utilisateur non-membre accédant à un autre espace**
*Type : risque.*
- **Description** : le choix entre deux codes de réponse HTTP distincts pour un accès non autorisé n'est pas fixé, bien que le principe de non-révélation de l'existence de l'espace soit acté.
- **Probabilité** : Faible — principe acté, code proposé.
- **Impact** : Modéré — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : principe de non-révélation de l'existence d'un espace acté, avec une proposition de code de réponse.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : spécification du contrat OpenAPI ; décision d'architecture sur le modèle d'autorisation API.

**T-14 — Schéma de propriétés des types de document système non modélisé**
*Type : risque.*
- **Description** : le schéma de propriétés des types de document système n'est pas modélisé, ce qui bloque l'amorçage de ces types tant que le point n'est pas résolu.
- **Probabilité** : Élevée — aucune mitigation actée, bloque l'amorçage des types système.
- **Impact** : Modéré — dimension dominante : coût & délai.
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Élevée — aucune mitigation actée à ce stade.
- **Traitement** : Réduire.
- **Mitigation** : aucune actée à ce stade.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle, modélisation de domaine.
- **Confiance** : ferme.
- **Source** : spécification des schémas de propriétés de document.

**T-15 — Comportement du réglage de propriétés sans type de document non fixé**
*Type : risque.*
- **Description** : le comportement de l'opération de réglage de propriétés lorsqu'aucun type de document n'est défini n'est pas fixé — enjeu de gouvernance des propriétés et, potentiellement, de visibilité au sens RGPD.
- **Probabilité** : Modérée — aucune mitigation, comportement de bord sans type non fixé.
- **Impact** : Modéré — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : aucune actée à ce stade.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle, modélisation de domaine.
- **Confiance** : ferme.
- **Source** : spécification des schémas de propriétés de document.

**T-16 — Filtres de requête (soft-delete par jointure) non arbitrés**
*Type : risque.*
- **Description** : deux options de câblage technique pour empêcher la lecture d'un document via son identifiant lorsque son espace est en corbeille restent non tranchées, bien que l'invariant de visibilité soit acté dans son principe.
- **Probabilité** : Élevée — invariant acté, deux options de câblage non tranchées.
- **Impact** : Majeur — dimension dominante : conformité RGPD (risque de fuite par lecture d'un document alors que son espace est en corbeille).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : invariant de visibilité acté dans son principe ; deux options de câblage technique restent non tranchées.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : spécification du mapping EF Core ; décision d'architecture sur la cascade d'intégrité référentielle.

**T-17 — Modélisation du lien de document et validation de l'alias non nommées**
*Type : risque.*
- **Description** : la modélisation technique du lien inter-documents et la validation à l'exécution de son alias ne sont pas encore nommées, bien que le modèle logique de données fixe déjà une table séparée pour ce lien.
- **Probabilité** : Faible — le modèle logique de données fixe déjà une table séparée.
- **Impact** : Modéré — dimension dominante : coût & délai.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : le modèle logique de données fixe déjà une table séparée pour ce lien.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : spécification du mapping EF Core ; décision d'architecture sur la gouvernance documentaire.

**T-18 — Chaîne média externalisée et purge des blobs orphelins**
*Type : dette / arbitrage accepté.*
- **Description** : le MVP conserve tout contenu média en ligne (inline) ; le câblage de purge des blobs orphelins pour une chaîne média externalisée est différé à une passe ultérieure.
- **Impact conditionnel** : Majeur — dimension dominante : conformité RGPD — si un média externalisé est introduit sans que la purge soit câblée.
- **Résiduel** : Faible tant que le MVP reste sans média externalisé.
- **Traitement** : Accepter, avec déclencheur de réactivation explicite.
- **Condition de réactivation** : introduction d'un média externalisé au périmètre.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la cascade d'intégrité référentielle.

**T-19 — Invariant « validation locale incluse dans la validation serveur » non outillé**
*Type : risque.*
- **Description** : la discipline de conception garantissant que toute validation effectuée localement est un sous-ensemble de la validation serveur n'est pas garantie par un test croisé automatisé entre les deux langages du projet.
- **Probabilité** : Modérée — discipline non garantie par un test croisé.
- **Impact** : Modéré — dimension dominante : coût & délai (divergence entre validation locale et serveur).
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : revalidation par les objets-valeur serveur à l'import, en filet de sécurité.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sérialisation locale et la migration ; décision d'architecture sur le modèle IndexedDB local ; roadmap d'entrée en build.

**T-20 — Migration ascendante du format d'export reportée**
*Type : dette / arbitrage accepté.*
- **Description** : la capacité de migrer un export ancien vers un format plus récent est reportée après le MVP.
- **Impact conditionnel** : Modéré — dimension dominante : coût & délai.
- **Résiduel** : Faible — rejet propre en garde en cas d'incompatibilité de version.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — dette nommée post-MVP.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sérialisation locale et la migration.

**T-21 — Chiffrement au repos non implémenté sur le stockage local**
*Type : dette / arbitrage accepté.*
- **Description** : le contenu stocké localement n'est pas chiffré au repos, avec une exposition possible en cas d'inspection via les outils de développement du navigateur sur un poste partagé.
- **Impact conditionnel** : Modéré — dimension dominante : sécurité & données personnelles.
- **Résiduel** : Moyenne — un bandeau de confidentialité informe l'utilisateur.
- **Traitement** : Accepter, avec réduction partielle par le bandeau de confidentialité.
- **Condition de réactivation** : n/a — dette nommée post-MVP, information au délégué à la protection des données.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le modèle IndexedDB local.

**T-22 — Dettes d'authentification post-MVP**
*Type : dette / arbitrage accepté.*
- **Description** : deux fonctionnalités de sécurité complémentaires (vérification de fuite de mot de passe, renouvellement glissant de session) sont reportées après le MVP.
- **Impact conditionnel** : Mineur — dimension dominante : sécurité & données personnelles.
- **Résiduel** : Faible — des bornes de repli sont déjà en place en l'absence de ces deux fonctionnalités.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — dettes nommées post-MVP.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sécurité de l'authentification.

**T-23 — Liste de révocation de jeton en mémoire, incompatible avec le passage à plusieurs instances**
*Type : risque.*
- **Description** : la liste de révocation des jetons est tenue en mémoire ; un accès résiduel après révocation resterait possible en cas de déploiement sur plusieurs instances non anticipé.
- **Probabilité** : Faible — mono-instance au MVP, magasin externe imposé dès le passage à plusieurs instances, avec un test d'architecture prévu pour détecter une dérive.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : magasin externe imposé dès que le déploiement dépasse une seule instance, avec une annotation de test d'architecture prévue pour détecter une dérive.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration, réactivation à l'introduction d'un déploiement multi-instance.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sécurité de l'authentification.

**T-24 — Sanitisation anti-injection conçue mais non certifiée**
*Type : risque.*
- **Description** : la protection contre l'injection de script persistante repose sur une liste blanche de balises, une politique de sécurité de contenu et une validation avant sanitisation — les trois actées dans leur principe mais non certifiées jusqu'à l'implémentation.
- **Probabilité** : Faible — mesures actées en principe, non certifiées.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles (apparenté à une injection de script persistante en cas de défaillance).
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : liste blanche de balises, politique de sécurité de contenu, validation avant sanitisation.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle, local-only ; choix de bibliothèque et de balises restant ouverts.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le modèle IndexedDB local ; spécification de sanitisation et de politique de sécurité de contenu.

**T-25 — Migration du stockage local multi-versions non prouvable en intégration continue**
*Type : risque.*
- **Description** : un contrat de format est testable en intégration continue, mais la validation de bout en bout sur plusieurs navigateurs est reportée à une passe de test dédiée, hors périmètre du build lui-même.
- **Probabilité** : Modérée — contrat testable, validation multi-navigateurs reportée.
- **Impact** : Modéré — dimension dominante : coût & délai.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire, complété par un transfert de la validation de bout en bout hors du build.
- **Mitigation** : contrat de format testable en intégration continue.
- **Propriétaire** : Lead technique.
- **Échéance** : local-only pour le contrat, post-MVP pour la validation de bout en bout.
- **Confiance** : sous réserve — à confirmer par le comportement multi-navigateurs. `[NON-VÉRIFIABLE-EN-BUILD]` pour le volet plateforme.
- **Source** : décision d'architecture sur la sérialisation locale et la migration ; décision d'architecture sur le modèle IndexedDB local.

**T-26 — Fiabilité du signal d'email vérifié pour les fournisseurs d'identité à email de relais**
*Type : risque.*
- **Description** : un email de relais (masquant l'adresse réelle) traité à tort comme vérifié exposerait à une prise de contrôle de compte. Un contrôle simulé et une règle de confiance par fournisseur sont actés ; la dette liée aux emails de relais est nommée et non déclenchée au périmètre du MVP.
- **Probabilité** : Faible — contrôle simulé et règle de confiance actés, dette relais non déclenchée.
- **Impact** : Majeur — dimension dominante : sécurité & données personnelles.
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire, complété par une acceptation de la dette relais nommée hors MVP.
- **Mitigation** : contrôle simulé et règle de confiance par fournisseur actés.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : sous réserve — à confirmer par le comportement du fournisseur d'identité. `[NON-VÉRIFIABLE-EN-BUILD]`
- **Source** : décision d'architecture sur la sécurité de l'authentification.

**T-27 — Sémantique de visibilité sur l'espace personnel non tranchée**
*Type : dette / arbitrage accepté.*
- **Description** : la sémantique fine de visibilité (visible par le seul maître du jeu, ou par le seul joueur concerné) sur l'espace personnel n'est pas tranchée ; l'effet de bord "visible par le seul propriétaire" est noté comme comportement par défaut acceptable en l'état.
- **Impact conditionnel** : Mineur — dimension dominante : sécurité & données personnelles.
- **Résiduel** : Faible.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a.
- **Propriétaire** : Responsable produit / Architecte.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la généralisation de l'espace personnel.

**T-28 — Instanciation cross-espace depuis un espace personnel d'autrui : purge non précisée**
*Type : risque.*
- **Description** : le traitement de purge en cas d'instanciation cross-espace depuis un espace personnel d'autrui n'est pas précisé, avec un risque de perte de la provenance d'un document en cas de purge mal séquencée. Le cas est cependant déjà couvert par le principe général de la saga de purge.
- **Probabilité** : Faible — cas cross-espace couvert par le principe de la saga.
- **Impact** : Modéré — dimension dominante : conformité RGPD.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : le traitement cross-espace de ce cas est déjà acté dans le principe général de la saga de purge.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la généralisation de l'espace personnel ; décision d'architecture sur la cascade d'intégrité référentielle.

**T-29 — Interface de restauration de la corbeille d'un espace non statuée**
*Type : risque.*
- **Description** : ni la fenêtre de rétention de 30 jours ni la possibilité de restauration elle-même ne sont remises en cause ; seul le point d'accès (un onglet dédié, un point d'API spécifique) reste ouvert — enjeu de conception de l'expérience utilisateur.
- **Probabilité** : Modérée — point d'accès ouvert, aucune mitigation actée.
- **Impact** : Mineur — dimension dominante : valeur produit.
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : aucune actée à ce stade.
- **Propriétaire** : Responsable produit.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la suppression d'espace.

---

### 3.2 Axe produit (P-01 à P-14)

**P-01 — Gate marché : décision go/no-go non vérifiable en build**
*Type : gate / décision.*
- **Description** : un gate de validation marché doit être posé avant d'engager le build lourd cloud et temps réel. Il engage le capital d'investissement de cette phase ; ses seuils de décision ne sont pas fixés.
- **Classe de conséquence** : engage le capital du build lourd cloud et temps réel ; un no-go arrête l'entrée dans cette phase, pas le socle local déjà livré.
- **Dispositif de décision** : gate go/no-go sur télémétrie interprétée par l'opérateur ; seuils de décision non fixés.
- **Propriétaire** : Direction de projet, en appui du responsable produit.
- **Échéance** : après le jalon local, avant le jalon cloud + migration.
- **Confiance** : sous réserve — dépend d'une interprétation opérateur de la télémétrie, non automatisable par un critère de build. `[NON-VÉRIFIABLE-EN-BUILD]`
- **Source** : roadmap d'entrée en build ; décision d'architecture sur le périmètre du MVP.
- **Recoupements** : dépend de P-08 (instrumentation) et de P-14 (cohorte pilote) ; voir aussi D-03 (même dépendance lue sous l'angle délai-coût).

**P-02 — Hypothèse H1 (activation préparation) non confirmée**
*Type : hypothèse.*
- **Description** : seuil de 60 % ou plus de la cohorte pilote atteignant l'activation de préparation, sur un délai de 14 jours.
- **Classe de conséquence si infirmée** : activation de base compromise.
- **Dispositif de suivi** : activation instrumentée avec un critère de falsifiabilité explicite — un seuil non atteint impose un constat, jamais une réinterprétation a posteriori.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : sous réserve — dépend de l'adoption réelle par la cohorte.
- **Source** : cahier des charges — hypothèses de validation.

**P-03 — Hypothèse H2 (vue de session), hypothèse centrale du produit, non confirmée**
*Type : hypothèse.*
- **Description** : seuil de 50 % ou plus à l'activation, avec un signal de répétabilité de 50 % ou plus sur deux sessions ou davantage, sur des délais de 30 et 60 jours. Le cahier des charges qualifie explicitement cette hypothèse de centrale.
- **Classe de conséquence si infirmée** : invalidation de la proposition de valeur du produit dans son ensemble, pas seulement d'un pilier — seule classe de conséquence de niveau "critique" de ce registre.
- **Dispositif de suivi** : activation instrumentée, complétée par des entretiens sur les motifs de non-adoption en cas d'échec du seuil.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : l'impact est ferme (thèse produit explicitement qualifiée par le cahier des charges) ; l'issue de l'hypothèse elle-même est sous réserve de la mesure.
- **Source** : cahier des charges — hypothèses de validation.

**P-04 — Hypothèse H3 (fluidité du partage) non confirmée**
*Type : hypothèse.*
- **Description** : seuil de 40 % ou plus, sur un délai de 60 jours.
- **Classe de conséquence si infirmée** : affaiblit le vecteur de croissance et de différenciation du produit.
- **Dispositif de suivi** : activation du partage instrumentée, complétée par des entretiens.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : sous réserve.
- **Source** : cahier des charges — hypothèses de validation.

**P-05 — Hypothèse H4 (accès joueur sans compte) non confirmée**
*Type : hypothèse.*
- **Description** : seuil de 70 % ou plus, sur un délai de 60 jours.
- **Classe de conséquence si infirmée** : un frein à l'entrée du joueur invité compromet l'adoption du groupe entier autour de la table.
- **Dispositif de suivi** : taux de consultation instrumenté, complété par des entretiens sur les motifs d'abandon.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : sous réserve.
- **Source** : cahier des charges — hypothèses de validation ; décision d'architecture sur le périmètre du MVP.

**P-06 — Hypothèse H5 (conversion local vers compte) non confirmée**
*Type : hypothèse.*
- **Description** : seuil de 10 % ou plus, sur un délai de 90 jours — seul volet de conversion testé par le MVP.
- **Classe de conséquence si infirmée** : limite l'échelle de conversion (croissance), sans remettre en cause la valeur en usage local.
- **Dispositif de suivi** : mesure d'usage, complétée par des entretiens.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : sous réserve.
- **Source** : cahier des charges — hypothèses de validation ; décision d'architecture sur le modèle de monétisation.

**P-07 — Apprentissage produit non isolé (MVP unique, sans test A/B)**
*Type : dette / arbitrage accepté.*
- **Description** : le MVP unique n'isole pas expérimentalement l'apprentissage par pilier — un choix de conception délibéré, assumé par l'opérateur, et non un aléa à réduire. Un échec d'une hypothèse (H1 à H4) ne peut donc pas être attribué avec certitude à un pilier précis du produit.
- **Impact conditionnel** : Majeur (modéré à élevé) — dimension dominante : valeur produit.
- **Résiduel** : Élevé, non abaissable en l'état — la mitigation envisagée (télémétrie isolant la mesure par pilier) est précisément ce que P-08 documente comme non implémentable en l'état.
- **Traitement** : Accepter, avec une réduction partielle attendue via P-08.
- **Condition de réactivation** : n/a — risque assumé au moment de l'arbitrage du périmètre du MVP.
- **Propriétaire** : Responsable produit.
- **Échéance** : gate marché.
- **Confiance** : ferme — le choix est assumé, sa conséquence sur l'attribution des causes est certaine par construction, sans dépendance à un jugement externe.
- **Source** : décision d'architecture sur le périmètre du MVP ; spécification de télémétrie.
- **Recoupements** : couplé directement à P-08 — la mitigation prévue pour ce risque est précisément ce que P-08 documente comme non implémentable en l'état. Les deux entrées ne se neutralisent pas : l'une cause un besoin de mitigation que l'autre empêche de satisfaire.

**P-08 — Instrumentation d'activation non opérationnalisable en l'état (télémétrie par pilier)**
*Type : risque.*
- **Description** : sans cette instrumentation, les hypothèses H1 à H4 et le gate marché reposent sur une mesure incomplète ou absente. Une contrainte RGPD est posée (mesure d'occurrence sans contenu narratif), mais celle-ci contraint la conception de l'instrumentation sans fournir le cadre technique nécessaire à sa mise en œuvre : six points bloquants restent ouverts dans la spécification.
- **Probabilité** : Élevée — non implémentable en l'état, six points bloquants.
- **Impact** : Majeur — dimension dominante : valeur produit.
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne — trancher les six points ouverts est un travail interne maîtrisable, qui débloque également P-07.
- **Traitement** : Réduire.
- **Mitigation** : contrainte RGPD posée (mesure d'occurrence sans contenu narratif) ; cadre technique à compléter.
- **Propriétaire** : Responsable produit pour les critères, Architecte / Lead technique pour l'implémentation.
- **Échéance** : local-only.
- **Confiance** : ferme.
- **Source** : spécification de télémétrie ; décision d'architecture sur le périmètre du MVP.
- **Recoupements** : voir P-07 (relation directe décrite ci-dessus) ; noté sur l'axe produit car son impact porte avant tout sur la validation des hypothèses produit, pas sur une fonctionnalité livrable en tant que telle.

**P-09 — Conversion payante (offre payante) non testée par le MVP**
*Type : dette / arbitrage accepté.*
- **Description** : la conversion vers l'offre payante n'est pas testée par le MVP, par choix de portée.
- **Impact conditionnel** : Modéré — dimension dominante : coût & délai (signal de viabilité économique différé).
- **Résiduel** : Moyenne.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — hors périmètre de test du MVP par choix de portée.
- **Propriétaire** : Responsable produit.
- **Échéance** : post-MVP.
- **Confiance** : sous réserve.
- **Source** : décision d'architecture sur le modèle de monétisation.

**P-10 — Dimensionnement de marché et stratégie de mise sur le marché non instruits**
*Type : question ouverte.*
- **Description** : le dimensionnement du marché adressable et le revenu moyen par utilisateur, ainsi que la stratégie de mise sur le marché, ne sont pas instruits à l'entrée en build.
- **Classe de conséquence** : le modèle économique et la décision d'investissement post-gate risquent d'être mal calibrés sans cette instruction.
- **Traitement** : instruire avant tout engagement lourd — positionné après le gate marché.
- **Propriétaire** : Responsable produit ; direction de projet pour le volet mise sur le marché.
- **Échéance** : après le gate marché.
- **Confiance** : sous réserve — dépend de données de marché externes.
- **Source** : décision d'architecture sur le modèle de monétisation ; décision d'architecture sur le périmètre du MVP.

**P-11 — Prix de l'offre payante, coût infrastructure et seuil de rentabilité non validés**
*Type : question ouverte.*
- **Description** : le prix de l'offre payante, le coût infrastructure par utilisateur et le seuil de rentabilité ne sont pas validés ; les chiffres sont qualifiés de volatiles par la source.
- **Classe de conséquence** : la viabilité économique du modèle reste à confirmer.
- **Traitement** : produire une note de coût infrastructure ; valider après mesure du coût réel.
- **Propriétaire** : Responsable produit ; Architecte pour le volet coût infrastructure.
- **Échéance** : cloud + migration, à la mesure du coût réel.
- **Confiance** : sous réserve.
- **Source** : décision d'architecture sur le modèle de monétisation ; cahier des charges.

**P-12 — Scénario réutilisable en une fois reporté hors première livraison**
*Type : dette / arbitrage accepté.*
- **Description** : le scénario jouable en une seule fois est reporté hors de la première livraison ; le profil d'usage concerné reste servi par le parcours de campagne classique.
- **Impact conditionnel** : Mineur — dimension dominante : valeur produit.
- **Résiduel** : Faible.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — arbitrage de périmètre assumé.
- **Propriétaire** : Responsable produit.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le périmètre du MVP ; cahier des charges.

**P-13 — Types de documents élaborés rétrogradés en priorité secondaire**
*Type : dette / arbitrage accepté.*
- **Description** : les types de documents élaborés sont rétrogradés en priorité secondaire, avec un contrepoids par le reclassement d'un autre cas d'usage (organisation en dossiers) en priorité supérieure.
- **Impact conditionnel** : Mineur — dimension dominante : valeur produit.
- **Résiduel** : Faible.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — arbitrage de périmètre assumé.
- **Propriétaire** : Responsable produit.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le périmètre du MVP ; cahier des charges.

**P-14 — Dépendance à la cohorte pilote non traitée**
*Type : risque.*
- **Description** : le recrutement des premiers utilisateurs (cohorte pilote d'utilisateurs réels) n'est pas traité, alors qu'il constitue un prérequis de fait pour la validation de toutes les hypothèses produit.
- **Probabilité** : Élevée — recrutement non traité, prérequis ouvert sans plan.
- **Impact** : Majeur — dimension dominante : valeur produit (précondition de mesure de toutes les hypothèses et intrant du gate marché).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne — établir un plan de recrutement en amont.
- **Traitement** : Réduire.
- **Mitigation** : aucune actée à ce stade.
- **Propriétaire** : Responsable produit, direction de projet.
- **Échéance** : en amont du gate marché.
- **Confiance** : sous réserve — dépend du recrutement effectif.
- **Source** : cahier des charges — hypothèses de validation.
- **Recoupements** : précondition dure des cinq hypothèses de validation (P-02 à P-06) — sans cohorte recrutée, aucune hypothèse n'est mesurable, y compris l'hypothèse centrale (P-03), quel que soit l'état de son instrumentation propre.

---

### 3.3 Axe juridique (J-01 à J-18)

Toutes les questions ouvertes de cet axe (J-02 à J-18) portent la mention « à valider juriste », le tag `[NON-VÉRIFIABLE-EN-BUILD]`, et dépendent d'un avis juriste externe. Cette caractéristique commune n'est pas répétée à chaque entrée.

**J-01 — Gate juriste EU : validation pré-lancement sur 5 axes**
*Type : gate / décision.*
- **Description** : validation juridique pré-lancement portant sur cinq axes RGPD, gate humain externe au build technique.
- **Classe de conséquence** : bloque le lancement commercial dans l'Union européenne, pas le build technique.
- **Dispositif de décision** : avis juriste sur les cinq axes, consolidation actée, séquencement proposé, instruction parallélisable au build technique, critère de sortie binaire par axe.
- **Propriétaire** : Direction de projet, en appui du conseil juridique et du délégué à la protection des données.
- **Échéance** : avant le lancement commercial dans l'Union européenne.
- **Confiance** : sous réserve — dépend d'un avis juriste externe sur les cinq axes. `[NON-VÉRIFIABLE-EN-BUILD]` Ce gate est un **indicateur agrégé** des axes J-02, J-03, J-04, J-05 et de la facette RGPD de T-04 — il ne s'additionne pas à ces entrées, il les enveloppe.
- **Source** : roadmap d'entrée en build ; cadrage juridique de validation pré-lancement EU.

**J-02 — Qualification « service destiné aux mineurs » (article 8 du RGPD)**
*Type : question ouverte.*
- **Description** : si le service est qualifié de destiné aux mineurs, une simple attestation d'âge de 16 ans ou plus pourrait être insuffisante et nécessiter une vérification d'âge ou un consentement parental.
- **Classe de conséquence** : nécessité potentielle d'une vérification d'âge ou d'un consentement parental si la qualification est retenue.
- **Traitement** : transférer à la qualification juriste ; une attestation d'âge est déjà actée comme mesure provisoire, sans vérification active.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : gate juriste EU (axe 1).
- **Source** : cadrage juridique de validation pré-lancement EU ; décision d'architecture sur l'effacement de compte ; décision d'architecture sur les données des invités.

**J-03 — Périmètre du contrat de sous-traitance, y compris l'espace personnel (article 28 du RGPD)**
*Type : question ouverte.*
- **Description** : la qualification sous-traitant ou responsable de traitement, notamment pour l'espace personnel, n'est pas tranchée ; une posture de sous-traitant est retenue par défaut avec un squelette de contrat déjà produit.
- **Classe de conséquence** : une qualification erronée constitue une non-conformité structurelle ; conditionne directement le périmètre du contrat de sous-traitance (voir J-08).
- **Traitement** : transférer à la qualification juriste.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : gate juriste EU (axe 2).
- **Source** : décision d'architecture sur les données des invités ; décision d'architecture sur la généralisation de l'espace personnel ; cadrage juridique de validation pré-lancement EU.

**J-04 — Mise en balance de l'intérêt légitime (conservation post-effacement, base légale du nom affiché invité)**
*Type : question ouverte.*
- **Description** : la mise en balance de l'intérêt légitime pour la conservation post-effacement de documents partagés et pour la base légale du nom affiché des invités n'a pas été produite ; des bases légales par défaut sont posées avec un motif esquissé.
- **Classe de conséquence** : la base légale de conservation ou du nom affiché serait invalidée si la mise en balance s'avère défaillante.
- **Traitement** : transférer — test de mise en balance à conduire.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : gate juriste EU (axe 3).
- **Source** : cadrage juridique de validation pré-lancement EU ; décision d'architecture sur l'effacement de compte ; décision d'architecture sur les données des invités.

**J-05 — Hard-delete inconditionnel de l'espace personnel, y compris l'instant de référence (article 17 du RGPD)**
*Type : question ouverte.*
- **Description** : la purge inconditionnelle et le figement de l'instant de référence sont actés dans leur principe technique, mais la qualification juridique elle-même de ce mécanisme au regard de l'obligation d'effacement n'est pas tranchée.
- **Classe de conséquence** : suffisance du mécanisme d'effacement au regard de l'article 17 à confirmer.
- **Traitement** : transférer — la posture technique est conçue, elle ne doit pas être sur-cotée en l'absence de validation juridique.
- **Propriétaire** : Conseil juridique / délégué à la protection des données ; Architecte pour l'instant de référence.
- **Échéance** : gate juriste EU (axe 4).
- **Source** : cadrage juridique de validation pré-lancement EU ; décision d'architecture sur l'effacement de compte ; décision d'architecture sur la généralisation de l'espace personnel.
- **Recoupements** : voir T-07 (mécanisme technique de la cascade d'effacement portant le même sujet) — face juridique, l'une qualifie, l'autre implémente. Décision réversible : ne préjuge pas irréversiblement du mécanisme technique sous-jacent.

**J-06 — Base légale des invités, bloquante avant le lancement EU**
*Type : question ouverte.*
- **Description** : la base légale retenue pour le traitement des données des invités (intérêt légitime) est proposée, sous réserve de confirmation par le gate juriste EU. Ce point est bloquant avant tout lancement dans l'Union européenne, pas nécessairement avant le début technique du jalon cloud.
- **Classe de conséquence** : bloque le lancement dans l'Union européenne ; base proposée standard, confirmable.
- **Traitement** : transférer — validation attendue lors du premier jalon du calendrier juridique.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : avant le lancement dans l'Union européenne.
- **Source** : roadmap d'entrée en build ; roadmap juridique ; décision d'architecture sur l'autorisation RGPD ; décision d'architecture sur les données des invités.

**J-07 — Qualification du canal de notification pour un compte anonymisé (article 12 §3 du RGPD)**
*Type : question ouverte.*
- **Description** : le canal de notification approprié pour un compte anonymisé n'est pas qualifié juridiquement, bien qu'un enregistrement à finalité limitée et une exception à la purge soient conçus.
- **Classe de conséquence** : portée limitée, cas de bord étroit.
- **Traitement** : transférer à la qualification juriste.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : gate juriste EU.
- **Source** : décision d'architecture sur l'effacement de compte ; procédure des droits des invités.

**J-08 — Contrat de sous-traitance non rédigé (article 28 du RGPD)**
*Type : question ouverte.*
- **Description** : le contrat de sous-traitance n'est pas rédigé ; un squelette de contrat est déjà produit. Ce point dépend de J-03 (périmètre du sous-traitant).
- **Classe de conséquence** : l'absence de ce document contractuel requis bloque le lancement du traitement des contenus partagés.
- **Traitement** : réduire (rédiger depuis le squelette existant), puis transférer pour validation.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : jalon dédié du calendrier juridique, avant le lancement dans l'Union européenne.
- **Source** : roadmap juridique ; squelette de contrat de sous-traitance ; décision d'architecture sur les données des invités.

**J-09 — Politique de confidentialité non produite**
*Type : question ouverte.*
- **Description** : la politique de confidentialité n'est pas produite ; un contenu minimal est esquissé dans les décisions d'architecture concernées.
- **Classe de conséquence** : document obligatoire au lancement dans l'Union européenne, son absence constitue une non-conformité.
- **Traitement** : réduire (produire le document), puis transférer pour validation.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, en appui du responsable produit.
- **Échéance** : jalon dédié du calendrier juridique, avant le lancement dans l'Union européenne.
- **Source** : roadmap juridique ; décision d'architecture sur l'effacement de compte ; décision d'architecture sur les données des invités.

**J-10 — Mentions d'information aux points de collecte non mises en œuvre (articles 13 et 14 du RGPD)**
*Type : question ouverte.*
- **Description** : les mentions d'information obligatoires aux points de collecte ne sont pas mises en œuvre ; un contenu minimal est esquissé, dépendant de la qualification de J-02.
- **Classe de conséquence** : mentions obligatoires au point de collecte, leur absence constitue une non-conformité.
- **Traitement** : réduire (implémenter dans l'interface), puis transférer pour validation.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, en appui du responsable produit.
- **Échéance** : jalon dédié du calendrier juridique, avant le lancement dans l'Union européenne.
- **Source** : roadmap juridique ; décision d'architecture sur les données des invités.

**J-11 — Applicabilité du registre des traitements non tranchée (article 30 du RGPD)**
*Type : question ouverte.*
- **Description** : l'applicabilité d'un registre des traitements formel au projet n'est pas tranchée.
- **Classe de conséquence** : registre administratif ; la matière est déjà cartographiée par ailleurs dans le corpus.
- **Traitement** : transférer (trancher l'applicabilité), puis réduire si applicable.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : jalon dédié du calendrier juridique, avant le lancement dans l'Union européenne.
- **Source** : roadmap juridique.

**J-12 — Méthode de vérification d'identité des invités et cas d'identité non corroborée (article 12 §6 du RGPD)**
*Type : question ouverte — lacune réelle.*
- **Description** : aucune méthode de vérification d'identité des invités exerçant un droit n'est retenue à ce stade ; un principe de proportionnalité est posé mais sans méthode concrète, ce qui constitue un blocage procédural en l'état.
- **Classe de conséquence** : risque de divulgation ou d'effacement au profit d'un usurpateur d'identité — face sécurité de cette question.
- **Traitement** : réduire (définir une méthode proportionnée) — qualification conjointe juriste et sécurité.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, Architecte / Lead technique.
- **Échéance** : avant la mise en opération de la procédure des droits des invités, avant le lancement dans l'Union européenne.
- **Source** : procédure des droits des invités.

**J-13 — Critères d'extension du délai de réponse non qualifiés (article 12 §3 du RGPD, 2 mois)**
*Type : question ouverte.*
- **Description** : les critères permettant d'étendre le délai de réponse aux demandes des personnes concernées ne sont pas qualifiés ; le délai de principe et le mécanisme d'extension sont posés dans leur structure.
- **Classe de conséquence** : raffinement procédural, non-conformité mineure en l'absence de qualification.
- **Traitement** : transférer à la qualification juriste.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : avant le lancement dans l'Union européenne.
- **Source** : procédure des droits des invités ; décision d'architecture sur l'effacement de compte.

**J-14 — Portabilité et limitation des invités non qualifiées (articles 20 et 18 du RGPD)**
*Type : question ouverte.*
- **Description** : l'applicabilité des droits à la portabilité et à la limitation du traitement pour les invités n'est pas qualifiée ; la portabilité est renvoyée après le MVP.
- **Classe de conséquence** : portée étroite, déclenchée uniquement si une demande de ce type se présente.
- **Traitement** : accepter sous réserve, qualifier si une demande se présente.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : post-MVP, à qualifier au cas par cas.
- **Source** : procédure des droits des invités ; décision d'architecture sur l'effacement de compte.

**J-15 — Catégories particulières de données incidentes dans le contenu narratif, non évaluées (article 9 du RGPD)**
*Type : question ouverte — lacune réelle.*
- **Description** : la présence possible de catégories particulières de données personnelles dans le contenu narratif généré par les utilisateurs n'a pas été évaluée en conception ; aucune détection automatique n'existe.
- **Classe de conséquence** : traitement incident de données sensibles sans base légale ni évaluation — exposition non mitigée la plus forte de l'axe juridique.
- **Traitement** : réduire (faire évaluer le point et décider), puis transférer — ce point ne doit pas être accepté par défaut.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, en appui du responsable produit et de l'architecte.
- **Échéance** : gate juriste EU, avant le lancement dans l'Union européenne.
- **Source** : squelette de contrat de sous-traitance.

**J-16 — Procédure de notification de violation de données non actée (articles 28 §3(f), 33 et 34 du RGPD)**
*Type : question ouverte — lacune réelle.*
- **Description** : aucune procédure de notification de violation de données n'est actée à ce stade.
- **Classe de conséquence** : la notification à l'autorité de contrôle sous 72 heures est obligatoire ; l'absence de procédure est grave si une violation survient.
- **Traitement** : réduire (définir la procédure), puis transférer ; éviter toute mise en production dans l'Union européenne sans procédure actée.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, Architecte / Lead technique.
- **Échéance** : avant le lancement dans l'Union européenne.
- **Source** : squelette de contrat de sous-traitance.

**J-17 — Rétention des sauvegardes pré-anonymisation à confirmer (article 5 §1(e) du RGPD)**
*Type : question ouverte.*
- **Description** : une durée de rétention de 30 jours est posée par défaut pour les sauvegardes antérieures à une anonymisation, avec interdiction actée de restauration non conforme ; le point reste à confirmer.
- **Classe de conséquence** : portée étroite, posture déjà conçue.
- **Traitement** : transférer, ou accepter sous réserve.
- **Propriétaire** : Conseil juridique / délégué à la protection des données, Architecte pour la stratégie de sauvegarde.
- **Échéance** : jalon cloud + migration, avant le lancement dans l'Union européenne.
- **Source** : décision d'architecture sur l'effacement de compte.

**J-18 — Base légale des journaux techniques et métadonnées non arbitrée**
*Type : question ouverte.*
- **Description** : la base légale des journaux techniques et métadonnées (adresse IP, horodatages) n'est pas arbitrée ; la base légale et la durée de conservation de 30 jours après expiration sont actées, mais la mise en balance elle-même n'a pas été produite.
- **Classe de conséquence** : base sécurité et débogage répandue et généralement défendable, à confirmer par mise en balance.
- **Traitement** : transférer — mise en balance à conduire.
- **Propriétaire** : Conseil juridique / délégué à la protection des données.
- **Échéance** : gate juriste EU.
- **Source** : roadmap juridique ; décision d'architecture sur les données des invités.

---

### 3.4 Axe délai-coût (D-01 à D-08)

**D-01 — Double référent de nomenclature de jalon (résolu) et ancrages non littéraux (résiduel)**
*Type : risque.*
- **Description** : le corpus portait deux usages distincts d'un même symbole de jalon selon la décision d'architecture consultée — l'une l'assimilait au jalon local, l'autre à une préoccupation cloud. **Ce point est résolu** : l'axe canonique J0-J3, nommé par contenu, est ratifié par l'opérateur (2026-09-01, `roadmap-entree-build.md` §5) et la décision d'architecture source a été clarifiée en conséquence. Le point résiduel, distinct et non couvert par cette ratification : certains rattachements entre codes de renvoi d'ADR et jalons de build restent des **ancrages inférés plutôt que documentés littéralement** — notamment le rapprochement du lot `P0.5` avec la phase de conception (CdC §11), qui n'établit qu'une antériorité déduite, pas un ancrage littéral, et reste à confirmer explicitement dans l'artefact de planification dédié.
- **Probabilité** : Quasi-certaine — incohérence de nomenclature déjà présente dans le corpus *(cotation portée à la prochaine revue du comité des risques : le facteur dominant — le double référent — est désormais résolu ; seul l'ancrage inféré résiduel subsiste)*.
- **Impact** : Modéré — dimension dominante : coût & délai (erreur d'ordonnancement si l'ancrage inféré est traité comme confirmé sans vérification).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : axe canonique ratifié pour la nomenclature de jalon ; l'ancrage inféré `P0.5` reste à confirmer explicitement dans l'artefact de planification dédié avant d'être traité comme acquis.
- **Propriétaire** : Direction de projet.
- **Échéance** : socle.
- **Confiance** : ferme sur la nomenclature de jalon (ratifiée) ; sous réserve sur l'ancrage inféré `P0.5`.
- **Source** : roadmap d'entrée en build ; cahier des charges §11.
- **Recoupements** : la roadmap documente la ratification directement ; ce registre le reprend comme point de pilotage sans y ajouter d'élément.

**D-02 — Stabilisation du socle et confirmation des décisions d'architecture pré-implémentation**
*Type : risque.*
- **Description** : un ensemble de décisions d'architecture pré-implémentation doit faire l'objet d'une confirmation datée à l'entrée en build ; ce critère de sortie du socle n'est pas encore levé, avec un effet de blocage en cascade sur le jalon local.
- **Probabilité** : Quasi-certaine — préalable bloquant du socle non levé (confirmation datée des décisions d'architecture, squelette de solution, test d'architecture en intégration continue).
- **Impact** : Majeur — dimension dominante : coût & délai (blocage en cascade du jalon local).
- **Criticité inhérente** : Très élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : critère de sortie factuel posé — confirmation datée de chaque décision d'architecture concernée avant l'entrée dans le jalon suivant.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : socle.
- **Confiance** : ferme.
- **Source** : roadmap d'entrée en build ; décision d'architecture sur le périmètre du MVP.

**D-03 — Dépendance cumulative : socle local stable et gate marché favorable**
*Type : risque.*
- **Description** : l'entrée dans le jalon cloud dépend de deux conditions cumulatives, non alternatives — la stabilité du jalon local et une décision favorable du gate marché.
- **Probabilité** : Élevée — dépendance cumulative non levée.
- **Impact** : Majeur — dimension dominante : coût & délai (engager le build lourd sans le gate serait une erreur coûteuse).
- **Criticité inhérente** : Élevée.
- **Criticité résiduelle cible** : Moyenne.
- **Traitement** : Réduire.
- **Mitigation** : le gate est posé dans son principe (voir P-01).
- **Propriétaire** : Direction de projet.
- **Échéance** : entre le gate marché et le jalon cloud + migration.
- **Confiance** : ferme. `[NON-VÉRIFIABLE-EN-BUILD]` sur le volet gate, celui-ci étant humain.
- **Source** : roadmap d'entrée en build ; décision d'architecture sur le périmètre du MVP.
- **Recoupements** : même contrainte que P-01 (gate marché), lue ici sous l'angle de la condition d'entrée dans le jalon suivant plutôt que sous l'angle de la décision elle-même.

**D-04 — Code de renvoi à cheval entre le jalon local et le jalon cloud**
*Type : risque.*
- **Description** : un code de renvoi du planning de build couvre à la fois du code serveur et la validation du contrat local, sans être assignable à un seul jalon.
- **Probabilité** : Modérée — code à cheval entre deux jalons.
- **Impact** : Modéré — dimension dominante : coût & délai (suivi de planning, pas de risque technique ajouté).
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : rattachement double explicité par la roadmap d'entrée en build.
- **Propriétaire** : Direction de projet.
- **Échéance** : frontière entre le jalon local et le jalon cloud + migration.
- **Confiance** : ferme.
- **Source** : roadmap d'entrée en build.
- **Recoupements** : renvoie, sur l'axe technique, à T-05 (câblage de l'autorisation) et T-19 / T-25 (invariants et migration multi-versions) — la double assignation calendaire ne crée pas de risque technique supplémentaire, elle en complique seulement le suivi de planning.

**D-05 — Code de renvoi à cheval entre le jalon cloud et le jalon partage temps réel**
*Type : risque.*
- **Description** : un code de renvoi du planning de build couvre à la fois le runtime temps réel introduit en amont et sa consolidation lors du jalon suivant.
- **Probabilité** : Modérée — code à cheval entre deux jalons.
- **Impact** : Modéré — dimension dominante : coût & délai (suivi de planning).
- **Criticité inhérente** : Moyenne.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Réduire.
- **Mitigation** : recoupement signalé explicitement par la roadmap d'entrée en build.
- **Propriétaire** : Direction de projet.
- **Échéance** : frontière entre le jalon cloud + migration et le jalon partage temps réel.
- **Confiance** : ferme.
- **Source** : roadmap d'entrée en build.
- **Recoupements** : même famille que D-04 (codes à cheval entre jalons).

**D-06 — Récupération de compte après perte du fournisseur d'identité, hors MVP**
*Type : dette / arbitrage accepté.*
- **Description** : le cas de récupération de compte après perte du fournisseur d'identité fédéré n'est pas couvert par le MVP ; un mot de passe complémentaire reste possible tant que le fournisseur d'identité subsiste comme option d'authentification.
- **Impact conditionnel** : Modéré — dimension dominante : coût & délai.
- **Résiduel** : Faible.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — dette nommée hors MVP.
- **Propriétaire** : Architecte / Lead technique.
- **Échéance** : post-MVP.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur la sécurité de l'authentification.

**D-07 — Compression du calendrier de build (jalons enchaînés sans découplage produit)**
*Type : dette / arbitrage accepté.*
- **Description** : les jalons de build sont enchaînés sans découplage produit entre eux, ce qui concentre la charge de travail en amont sur une équipe restreinte — un choix assumé par l'opérateur au moment de l'arbitrage du périmètre du MVP.
- **Impact conditionnel** : Majeur — dimension dominante : coût & délai (charge concentrée en amont sur une équipe restreinte).
- **Résiduel** : Élevé, non réductible sans redécoupler la séquence (option refusée) ; le gate marché borne le capital engagé en aval, pas la charge de travail en amont.
- **Traitement** : Accepter.
- **Condition de réactivation** : n/a — compression actée.
- **Propriétaire** : Direction de projet.
- **Échéance** : en amont du jalon cloud + migration.
- **Confiance** : ferme.
- **Source** : décision d'architecture sur le périmètre du MVP ; roadmap d'entrée en build.
- **Recoupements** : contrepartie assumée de l'absence d'isolement de l'apprentissage produit (voir P-07) ; le gate marché (P-01) arbitre l'engagement du build lourd qui suit cette compression.

**D-08 — Assets d'identité visuelle, préalable hors jalon de build**
*Type : risque.*
- **Description** : les assets d'identité visuelle (logo, charte graphique) sont un préalable hors jalon de build technique, requis pour le lancement mais sans bloquer le build lui-même.
- **Probabilité** : Faible — préalable hors jalon, piste de production séparée.
- **Impact** : Modéré — dimension dominante : coût & délai (assets requis pour le lancement, sans bloquer le build technique).
- **Criticité inhérente** : Faible.
- **Criticité résiduelle cible** : Faible.
- **Traitement** : Transférer.
- **Mitigation** : signalé pour visibilité de trajectoire ; le détail relève d'un travail de conception visuelle distinct, non couvert par ce registre.
- **Propriétaire** : Responsable produit.
- **Échéance** : hors jalon de build, avant le lancement.
- **Confiance** : sous réserve.
- **Source** : roadmap d'entrée en build.

---

## 4. Gouvernance du registre

**Rôle du comité des risques** : instance de revue périodique chargée de valider ou d'amender chaque cotation portée par ce registre, de statuer sur les points marqués `[À RATIFIER OPÉRATEUR]`, et de suivre l'évolution de la criticité résiduelle entre deux revues.

**Fréquence de revue proposée** : une revue complète à chaque franchissement de gate (gate marché, gate juriste EU) et à l'entrée de chaque jalon de build ; une revue allégée (statut des entrées "Élevée" et "Très élevée" uniquement) à cadence mensuelle entre deux jalons.

**Cycle de mise à jour** : toute nouvelle information issue du corpus de conception (décision d'architecture amendée, spécification complétée) qui modifie une cotation doit être répercutée dans ce registre à l'occasion de la prochaine revue, jamais rétroactivement sans trace de la modification. La colonne "tendance" (section 1.5) est destinée à porter cette évolution aux revues suivantes.

**Seuil d'escalade** : voir section 1.6.

**Appétence au risque** : voir section 1.6. Cette appétence est une proposition méthodologique de cette version, soumise à validation du comité des risques comme l'ensemble des cotations du registre.

**Source de vérité unique** : la section 3 (registre détaillé par axe) est la source de vérité unique des cotations de ce registre (probabilité, impact, criticité, classe de conséquence, statut, confiance). Les vues de synthèse de la section 2 (matrice §2.1, top risques §2.2, vue Hypothèses §2.3, vue Gates §2.4, points à ratifier et dettes §2.5) ainsi que les seuils d'escalade (§1.6) sont des **vues dérivées, à régénérer intégralement depuis la section 3 à chaque revue** — jamais à éditer ponctuellement de manière isolée. Une édition ponctuelle d'une vue de synthèse sans réouverture de la section 3 est la cause type d'une désynchronisation entre la matrice et le registre détaillé.

---

## 5. Relations et dépendances

Cette section porte les relations entre entrées qui touchent plusieurs axes ou se conditionnent mutuellement. Chaque entrée reste inscrite une seule fois dans la section 3, sous l'axe où elle est classée ; ce qui suit ne duplique aucune entrée.

- **P-07 ↔ P-08** — le MVP unique n'isole pas l'apprentissage produit par pilier (P-07, risque assumé par l'opérateur), et la télémétrie censée compenser cet effet en mesurant chaque pilier séparément n'est pas opérationnalisable en l'état documenté (P-08). La mitigation prévue pour l'un est précisément ce que l'autre empêche de fonctionner : une dépendance de mitigation non satisfaite, pas un doublon.
- **T-04 (reclaim-in-place)** porte trois faces distinctes sur trois axes : la résolution de sécurité elle-même (axe technique), le sort RGPD du contenu de la coquille reprise (enveloppé par le gate juriste EU, J-01), et son statut de point à ratifier par l'opérateur (gouvernance de build). Les trois faces sont nommées dans l'entrée T-04 ; elles ne constituent pas trois risques séparés.
- **T-07 ↔ J-05** — la complexité du mécanisme de cascade d'effacement et la qualification juridique du hard-delete inconditionnel de l'espace personnel portent sur le même sujet (article 17 du RGPD) sous deux angles distincts : l'un implémente, l'autre qualifie. Une clarification sur l'un peut modifier les contraintes de l'autre.
- **D-01 (nomenclature)** est la face délai-coût d'une incohérence documentée directement par la roadmap d'entrée en build — ce registre n'ajoute rien à ce constat, il le reprend comme point de pilotage.
- **P-01 (gate marché) ↔ D-03** — la dépendance cumulative du jalon cloud (socle local stable et gate marché favorable) est la même contrainte lue depuis l'angle produit (la décision elle-même) et depuis l'angle délai-coût (la condition d'entrée dans le jalon suivant).
- **D-04 / D-05 (codes à cheval)** renvoient, sur l'axe technique, aux mêmes réalités que celles couvertes par T-05 (câblage de l'autorisation) et T-19 / T-25 (invariants et migration multi-versions) : la double assignation calendaire documentée par la roadmap ne crée pas de risque technique supplémentaire, elle en complique seulement le suivi de planning.
- **P-14 → P-02 / P-03 / P-04 / P-05 / P-06** — la dépendance à la cohorte pilote est une précondition dure de la mesure des cinq hypothèses produit : le cahier des charges pose que les cinq hypothèses se démontrent auprès d'une cohorte pilote d'utilisateurs réels recrutés. Sans cohorte recrutée, aucune des hypothèses H1 à H5 n'est mesurable, qu'il s'agisse de l'hypothèse centrale (P-03, impact critique) ou des autres, quel que soit l'état de son instrumentation propre.
- **Chaîne de dépendance dominante de l'axe produit** : P-14 (cohorte) → P-08 (instrumentation) → P-02 à P-06 (hypothèses) → P-01 (gate marché). P-14 et P-08 sont les deux préconditions maîtrisables en interne à traiter en priorité.

---

## 6. Annexe A — Références documentaires nommées

Le corps de ce registre cite les sources ci-dessous par leur identifiant ou leur nom, sans chemin de fichier. Chaque source citée fait foi sur son propre contenu en cas de désaccord de lecture avec la formulation retenue dans ce registre.

**Décisions d'architecture** :
- ADR-001 — Exécution du domaine en mode local
- ADR-002 — Gouvernance documentaire (modèle « tout est document »)
- ADR-004 — Transport temps réel
- ADR-005 — Modèle de monétisation
- ADR-006 — Périmètre du MVP
- ADR-007 — RGPD et autorisation API
- ADR-008 — Structure de la solution
- ADR-010 — Suppression d'espace (campagne)
- ADR-011 — Cascade d'intégrité référentielle
- ADR-012 — RGPD, effacement de compte
- ADR-013 — RGPD, données des invités
- ADR-014 — Modèle d'autorisation API
- ADR-015 — Sécurité de l'authentification (MVP)
- ADR-016 — Sérialisation locale et migration
- ADR-017 — Modèle IndexedDB local
- ADR-018 — Espace personnel, généralisation de la notion d'espace

**Spécifications techniques** :
- Spécification — Repli du temps réel (SSE vers polling adaptatif)
- Spécification — Télémétrie
- Spécification — Configuration de sécurité et de migration
- Spécification — Schémas des propriétés de document
- Spécification — Mapping EF Core
- Spécification — Requête d'effacement non partagé
- Spécification — Sanitisation et politique de sécurité de contenu
- Spécification — Contrat OpenAPI

**Artefacts juridiques** :
- Cadrage juridique — Validation pré-lancement dans l'Union européenne (5 axes RGPD)
- Roadmap juridique — Calendrier de mise en conformité RGPD
- Squelette de contrat de sous-traitance
- Procédure des droits des invités

**Artefacts de planification** :
- Roadmap d'entrée en build — séquence de jalons, gates, préalables bloquants, réconciliation de nomenclature
- Cahier des charges — périmètre fonctionnel, hypothèses de validation (H1 à H5), phasage, modèle de prix

---

## 7. Annexe B — Réserve de méthode

Une partie des risques de l'axe délai-coût est tracée principalement au document qui les cite — la roadmap d'entrée en build, qui elle-même cite les décisions d'architecture sources — plutôt que par réouverture systématique de chaque décision primaire. Cette traçabilité "au citant" est légitime pour un exercice de consolidation mais est de force de preuve moindre qu'une citation directe à la décision d'origine.

De même, la provenance de certains points juridiques n'a pas été recoupée à une source primaire au-delà de la mention documentée dans le calendrier juridique.

Ces traçabilités doivent être rouvertes à la source avant toute décision engageante : arbitrage financier, ratification opérateur, engagement contractuel.

**Statut de cette version** : v1 — baseline d'évaluation. Toutes les cotations portées par ce registre sont au statut "brouillon expert — à valider en comité des risques" (voir section 1.5) et doivent être confirmées avant tout usage comme fondement d'une décision engageante.

---

## 8. Glossaire

**Termes métier**

- **Maître du jeu (MJ)** : joueur qui anime la partie et dispose de droits de visibilité et d'édition étendus sur le contenu de campagne.
- **Espace (Space)** : conteneur générique de contenu de jeu, généralisant la notion antérieure de campagne ; inclut notamment l'espace personnel d'un utilisateur.
- **Campagne** : partie de jeu de rôle continue, réunissant un maître du jeu et des joueurs sur la durée.
- **One-shot (scénario réutilisable en une fois)** : partie de jeu de rôle jouée en une seule séance.
- **Joueur invité** : joueur accédant à un espace sans détenir de compte utilisateur complet.
- **Reclaim-in-place** : opération consistant à faire reprendre par un utilisateur, via une preuve d'identité fédérée, un compte préexistant non vérifié portant la même adresse email.

**Acronymes**

- **MVP** : produit minimum viable (Minimum Viable Product), périmètre minimal livré pour valider les hypothèses produit.
- **ADR** : décision d'architecture tracée (Architecture Decision Record).
- **EU** : Union européenne.
- **RGPD** : Règlement général sur la protection des données.
- **DPA** : contrat de sous-traitance de données personnelles (Data Processing Agreement).
- **DPIA** : analyse d'impact relative à la protection des données (Data Protection Impact Assessment).
- **DPO** : délégué à la protection des données (Data Protection Officer).
- **IDOR** : référence directe non protégée à un objet (Insecure Direct Object Reference), catégorie de vulnérabilité d'autorisation.
- **XSS** : injection de script persistante ou réfléchie (Cross-Site Scripting), catégorie de vulnérabilité web.
- **CSP** : politique de sécurité de contenu (Content Security Policy), mécanisme de restriction des sources de contenu exécutable d'une page web.
- **IdP** : fournisseur d'identité (Identity Provider), tiers assurant l'authentification fédérée.
- **JWT** : jeton web signé (JSON Web Token), format de jeton d'authentification.
- **CWE** : catégorie de faiblesse logicielle référencée (Common Weakness Enumeration).
- **ISO 31000** : norme internationale de référence pour le management du risque.
- **SSE** : flux d'événements envoyés par le serveur (Server-Sent Events), mode de transport temps réel unidirectionnel.
- **EF Core** : infrastructure de mapping objet-relationnel (Entity Framework Core) utilisée côté serveur.
- **IndexedDB** : mécanisme de stockage structuré côté navigateur, utilisé pour le mode local.
- **Article 5, 8, 9, 12, 13, 14, 17, 18, 20, 28, 30, 33, 34 (RGPD)** : articles du règlement cités par les questions ouvertes de l'axe juridique — relatifs respectivement aux principes de traitement, aux mineurs, aux catégories particulières de données, aux droits des personnes, à l'information des personnes, au droit à l'effacement, au droit à la limitation, au droit à la portabilité, à la sous-traitance, au registre des traitements et à la notification de violation.

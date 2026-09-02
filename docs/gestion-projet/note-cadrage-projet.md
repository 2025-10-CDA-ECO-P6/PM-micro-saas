# Note de cadrage — Projet Haversack

| Champ | Valeur |
|---|---|
| **Objet du document** | Note de cadrage / charte de projet — document de gouvernance de tête, consolidant sans réinvention le corpus de conception, de planification et de business existant |
| **Version** | 1.0 |
| **Date** | 2026-07-16 |
| **Porteur** | Pierre-Marie Marchio |
| **Statut** | Conception — périmètre produit stabilisé, volet architecture technique à confirmer à l'entrée en build, plusieurs points de gouvernance et de données de gestion encore ouverts (voir [section 10](#10-éléments-à-compléter)) |
| **Audience** | Lecteur externe (investisseur, comité, partenaire) et porteur du projet |

Ce document ne tranche aucune décision nouvelle et n'introduit aucune donnée non présente dans le corpus. En cas de divergence de détail avec l'un des documents cités en [section 11](#11-documents-de-référence), la source citée fait foi.

---

## Sommaire

1. [Raison d'être & résumé exécutif](#1-raison-dêtre--résumé-exécutif)
2. [Objectifs & enjeux](#2-objectifs--enjeux)
3. [Périmètre](#3-périmètre)
4. [Parties prenantes & rôles](#4-parties-prenantes--rôles)
5. [Jalons](#5-jalons)
6. [Organisation & gouvernance](#6-organisation--gouvernance)
7. [Hypothèses de validation](#7-hypothèses-de-validation)
8. [Critères de succès](#8-critères-de-succès)
9. [Points de décision go/no-go](#9-points-de-décision-gono-go)
10. [Éléments à compléter](#10-éléments-à-compléter)
11. [Documents de référence](#11-documents-de-référence)

---

## 1. Raison d'être & résumé exécutif

Un Maître du Jeu (MJ) actif gère un volume d'information important — scénarios, historique de session, personnages non-joueurs, notes secrètes, informations partagées aux joueurs, lore — aujourd'hui dispersé sur des supports hétérogènes non conçus pour cet usage : fichiers texte, outils génériques (Notion, Obsidian), Discord, papier, PDF, tables virtuelles (Roll20, Foundry). Cette fragmentation a un coût direct en session : temps perdu à rechercher une information, rythme de jeu interrompu, charge mentale de préparation élevée.

Haversack se positionne sur un créneau non couvert par l'offre existante : un outil dédié au MJ, entre les outils génériques — flexibles mais sans logique de jeu de rôle — et les tables virtuelles — riches sur le plan mécanique mais centrées sur le combat plutôt que la narration.

Le produit se distingue par cinq éléments : une friction d'entrée nulle (aucun compte requis pour commencer) ; une vue de session dédiée au pilotage temps réel pendant la partie ; un agnosticisme total au système de jeu pratiqué ; un partage contrôlé, document par document, préservant les notes de préparation privées ; et un accès joueur sans compte. À ces éléments s'ajoute une possession effective des données par l'utilisateur — la capacité d'exporter l'intégralité d'un espace dans un format ouvert, en mode local comme avec un compte.

## 2. Objectifs & enjeux

**Objectif principal du MVP.** Permettre à un MJ de préparer une campagne, de structurer ses scénarios, de centraliser ses notes et d'accéder rapidement aux informations importantes en session — sans friction d'onboarding et sans compte obligatoire pour commencer.

**Enjeu stratégique — deux hypothèses portées en parallèle.** Le MVP valide simultanément une hypothèse produit et une hypothèse de monétisation :
- **Hypothèse produit** : un MJ acceptera de payer pour un outil qui réduit la friction de préparation et de pilotage de ses sessions, même en l'absence de fonctionnalités avancées (intelligence artificielle, table virtuelle, moteur de règles).
- **Hypothèse de monétisation** : un modèle en trois paliers (local → compte gratuit → payant) permet une adoption initiale sans résistance, suivie d'une conversion naturelle vers le cloud lorsque le besoin de partage ou de sauvegarde se manifeste.

**Enjeu business — modèle à trois paliers.** Le modèle de monétisation est non-agressif : la valeur est perçue avant tout engagement, et le passage à un palier supérieur est déclenché par un besoin concret (partage aux joueurs, dépassement de quota), jamais par une contrainte imposée en amont.

| Palier | Compte requis | Fonctionnalités | Déclencheur d'upgrade |
|---|---|---|---|
| Local | Aucun | Préparation complète, vue de session, création à la volée | — |
| Gratuit | Email et mot de passe | Synchronisation cloud, partage aux joueurs, accès multi-appareil | Intention de partager ou de sécuriser ses données |
| Pro | Abonnement (environ 7 €/mois) | Ensemble des fonctionnalités du palier gratuit, sans limite | Dépassement du quota d'espaces du palier gratuit |

**Segments de marché identifiés.** Trois segments sont dérivés du corpus : le MJ numériquement outillé (personas Thomas, Émilie), le MJ one-shot / convention (persona Sonia — non servi par la première livraison, le contexte one-shot complet étant hors périmètre), et le MJ résistant au numérique (persona Rémi — n'adopte que si la friction est nulle et la valeur immédiate).

## 3. Périmètre

Le périmètre du MVP est livré **en un seul bloc** : la brique cloud (compte, synchronisation) n'est pas différable, le partage aux joueurs — exigence Must Have — dépendant structurellement d'un compte utilisateur.

| Catégorie | Contenu |
|---|---|
| **Must Have** | UC-01 à UC-10 (dont UC-05 base — dossiers libres), espace personnel (conteneur par défaut, capture immédiate), instrumentation de validation du MVP, export d'espace (version minimale — promu de Should Have à Must Have le 2026-06-25) |
| **Should Have** | UC-05 riche (types de document élaborés), UC-11 (gestion des membres), UC-12 (vue joueur), UC-14 (recherche par mot-clé), UC-13 (scénario réutilisable — hors première livraison) |
| **Could Have** | Types de document personnalisés, réimport de fichier de sauvegarde (post-MVP), notes personnelles joueur persistantes |
| **Won't Have (cette version)** | Inventaire personnage, clôture formelle de session, lore en use case distinct, relations typées entre documents, moteur de règles, assistant IA, table visuelle, application desktop avec synchronisation CRDT, templates communautaires |

**Axe de périmètre produit.** Un espace prend l'une de trois formes (`SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`) : la campagne est livrée en MVP ; le one-shot complet (parcours express, scénarios réutilisables) est hors première livraison, un one-shot restant néanmoins possible en MVP sous forme d'une campagne à session unique ; l'espace personnel est opérationnel dès le MVP, y compris comme zone d'atterrissage par défaut pour tout document créé sans espace explicite.

Le détail complet des arbitrages de périmètre — critères de sortie, risques, dépendances par use case — fait foi dans `docs/conception/besoin/vision/moscow.md`. Cette section n'en constitue qu'une synthèse.

## 4. Parties prenantes & rôles

| Partie prenante | Rôle |
|---|---|
| **Maître du Jeu (MJ)** | Utilisateur principal — seul habilité à créer du contenu, administrer un espace et partager une information avec les joueurs. Compte optionnel. Porte l'intégralité de la charge de préparation et est le décideur d'achat. |
| **Joueur** | Accède à une campagne sur invitation du MJ. Compte optionnel — accès possible en mode invité via un lien de session. |
| **Joueur invité** | Accès temporaire par lien, sans compte, limité à la durée de la session (fenêtre de grâce de 24h). Peut convertir son accès en accès membre permanent à tout moment. |
| **Porteur du projet** | Pierre-Marie Marchio — validateur, décideur des arbitrages de périmètre et des points de décision go/no-go. |
| **Équipe de build** | Solo ou équipe restreinte ; le contexte solo est pris en compte dans le dispositif qualité (le test d'architecture en intégration continue remplace la discipline de revue de code). |
| **Juriste / conformité EU** | Instance de la validation juridique préalable au lancement commercial en Europe (voir [section 9](#9-points-de-décision-gono-go)). |
| **Partenaires techniques** | Fournisseurs d'identité externes retenus pour le MVP : Google et Discord. |
| **Audiences externes** | Investisseurs, partenaires, parties prenantes non techniques. |

**Personas de référence.** Sept personas illustrent la diversité des profils pris en compte en conception : **Thomas** (MJ préparateur, utilisateur avancé d'Obsidian, résistant au changement d'outil) ; **Émilie** (MJ improvisatrice, capture en séance) ; **Lucas** (joueur sans compte, réfractaire aux nouveaux outils) ; **Nadia** (MJ occasionnelle, sessions espacées) ; **Antoine** (MJ avancé, multi-groupe et multi-système) ; **Rémi** (MJ débutant, résistant idéologique au numérique) ; **Sonia** (MJ one-shot, groupes changeants).

Un organigramme nominal et une matrice RACI détaillée ne sont pas portés par le corpus de conception à ce stade — voir [section 10](#10-éléments-à-compléter).

## 5. Jalons

Le corpus porte deux repères de jalonnement distincts, complémentaires et non interchangeables : un axe technique d'entrée en build, et une trajectoire de valeur produit. Les deux sont reportés ici sans qu'aucun ne soit choisi silencieusement au détriment de l'autre.

### 5.1 Axe technique d'entrée en build (J0 → J3)

| Jalon | Contenu |
|---|---|
| **J0 — Socle** | Confirmation des décisions d'architecture pré-implémentation, squelette Domain/Application, test d'architecture en intégration continue, contrat d'autorisation applicatif. |
| **J1 — Local-only** | Domaine/Application → projection locale → interface minimale ; export versionné produit et rejouable ; aucune brique cloud. |
| **J2 — Cloud + migration** | Persistance cloud, authentification, handler d'import/migration, cascade de suppression conforme au RGPD. |
| **J3 — Partage + temps réel** | Diffusion temps réel, canal d'accès invité, dispositif anti-fuite par visibilité. |

L'ordre interne privilégie le socle métier avant toute projection technique (domaine et application d'abord, projection locale puis interface ensuite).

### 5.2 Paliers de valeur produit

En miroir de l'axe technique, la trajectoire produit se décompose en paliers de valeur : le **socle** (livré en un bloc unique et indivisible) → un **jalon de validation** (décision go/no-go) → l'**approfondissement du cœur** (renforcement de l'existant) → des **extensions déclenchées par signal** (priorité conditionnée à un usage réel observé) → une **vision long terme** (horizon produit).

### 5.3 Points de décision intercalés

Deux points de décision s'intercalent dans la séquence technique — détaillés en [section 9](#9-points-de-décision-gono-go) :
- la **DÉCISION MARCHÉ**, positionnée entre J1 et J2 ;
- la **VALIDATION JURIDIQUE EU**, positionnée avant le lancement commercial (elle n'ordonnance pas le build de J3, instructible en parallèle).

### 5.4 Point résolu — double référent du symbole « J1 »

Le corpus portait une incohérence apparente : le symbole « J1 » avait deux référents distincts selon la source. La décision d'architecture actant le périmètre du MVP nomme J1 le jalon **local-only** (le jalon décrit en 5.1 ci-dessus). Une autre décision d'architecture, portant sur la conformité RGPD et le modèle d'autorisation, mentionnait dans sa rédaction d'origine un « jalon J1 » assorti d'un repère de jalon numéroté distinct (nomenclature aujourd'hui retirée du corpus), associé à une préoccupation **cloud**. **Ratifié en décision produit le 2026-09-01** ([`roadmap-entree-build.md` §5](roadmap-entree-build.md)) : J1 = local-only, conformément à l'usage le plus densément documenté dans le corpus ; l'ADR source a été clarifié en conséquence (son invariant est intégré aux contrats Application avant J1, son application effective relevant de J2).

### 5.5 Absence de dates calendaires

Le corpus qualifie explicitement ce phasage d'**indicatif et non figé** : ni l'axe technique ni la trajectoire de valeur ne portent de date calendaire ferme. Le calendrier d'exécution est renvoyé à la [section 10](#10-éléments-à-compléter).

## 6. Organisation & gouvernance

**Instances de décision.** Deux points de décision go/no-go structurent la gouvernance de la trajectoire (détail en [section 9](#9-points-de-décision-gono-go)) : la DÉCISION MARCHÉ (décision produit sur la télémétrie du jalon J1) et la VALIDATION JURIDIQUE EU (avis d'un juriste sur cinq axes RGPD).

**Point de décision produit.** Le jalon de validation (5.2) constitue un go/no-go conditionnant l'engagement des paliers de valeur suivants : un résultat positif ouvre l'approfondissement du cœur, un résultat partiel ou négatif oriente vers la consolidation.

**Autorité de validation.** Le porteur du projet valide les arbitrages de périmètre et de point de décision. Le volet d'architecture technique, acté en conception, est re-confirmé formellement à l'entrée en build.

**Corps de décisions d'architecture.** Dix-huit décisions d'architecture (ADR) sont actées en conception pour le MVP, indexées dans le corpus de conception. Leur confirmation formelle à l'entrée en build (jalon J0) reste un acte à venir, portant sur les huit ADR de nature pré-implémentation ou mixte (structure de solution, stack front, transport temps réel, cascade RGPD, sécurité de l'authentification, sérialisation locale/migration, modèle IndexedDB, généralisation de l'espace) — voir `docs/gestion-projet/roadmap-entree-build.md` §3.1.

**Principaux risques.** Le corpus porte, sans qu'un registre dédié ne soit ici dupliqué, quatre risques de gouvernance déjà identifiés : l'hypothèse H2 comme risque produit existentiel (si la valeur de la vue de session n'est pas confirmée, la proposition de valeur globale est invalidée) ; le segment one-shot/convention délibérément non servi en première livraison, avec condition de retour tracée ; la VALIDATION JURIDIQUE EU, bloquante pour le lancement commercial en Europe ; et l'économie du produit non chiffrée à ce stade (valeurs volatiles). Le registre complet — quatre axes, matrice de criticité, vue par hypothèse et par point de décision — fait foi dans `docs/gestion-projet/registre-risques.md`.

**Dispositif qualité.** Le contexte d'équipe solo ou restreinte est pris en compte explicitement : un test d'architecture en intégration continue est érigé en garde-fou structurel, remplaçant la discipline de revue de code jugée insuffisante dans ce contexte.

**Mécanisme de gouvernance des points ouverts.** Les points non tranchés du corpus sont portés par des marqueurs explicites (`[À TRANCHER]`, `[À RATIFIER — produit]`) plutôt que résolus par défaut ou masqués.

**Règle d'autorité documentaire.** Le corpus de conception fait foi sur tout document dérivé (dont le cahier des charges) ; les artefacts de planification (roadmaps) ordonnancent un périmètre déjà arrêté, sans détenir d'autorité de corpus propre. En cas de conflit de lecture, la source citée prime.

**Instances formelles de pilotage.** Un comité de pilotage, une cadence de revue formalisée et une gouvernance de release ne sont que partiellement ou pas portés par le corpus à ce stade — voir [section 10](#10-éléments-à-compléter).

## 7. Hypothèses de validation

Le MVP doit démontrer cinq hypothèses (H1 à H5) auprès d'une cohorte pilote d'utilisateurs réels (early adopters recrutés). Chaque hypothèse est assortie d'un seuil chiffré et d'un délai d'observation, constituant un critère de décision objectif — un seuil non atteint impose un constat explicite (réussite partielle, échec de pilier, hypothèse invalidée), jamais une réinterprétation a posteriori.

**H2 est l'hypothèse centrale du produit** : elle porte sur la valeur réelle de la vue de session en pleine partie ; si elle n'est pas confirmée, la proposition de valeur du produit dans son ensemble est invalidée. H1 à H4 forment les piliers croisés par la DÉCISION MARCHÉ (voir [section 9](#9-points-de-décision-gono-go)). H5, distincte des quatre premières, porte spécifiquement sur la conversion et alimente le lien avec le modèle économique.

| # | Hypothèse | Seuil chiffré | Délai |
|---|---|---|---|
| **H1** | Un MJ crée un espace structuré et retrouve ses informations sans friction d'onboarding. | ≥ 60 % de la cohorte pilote atteignent l'activation préparation. *Nuance (décision produit du 2026-07-09) : le contenu d'un espace personnel sans campagne n'est comptabilisé que **partiellement** s'il traduit un geste structurant ; seuil exact `[À TRANCHER — métrique produit]`.* | 14 jours après le premier usage |
| **H2 (centrale)** | La vue de session apporte une valeur réelle pendant une partie. | ≥ 50 % des MJ ayant atteint l'activation préparation atteignent l'activation vue session ; répétabilité si ≥ 50 % l'utilisent sur ≥ 2 sessions. | 30 jours (première activation) / 60 jours (répétabilité) |
| **H3** | Le partage aux joueurs est perçu comme plus fluide que les solutions actuelles. | ≥ 40 % des MJ ayant animé une session avec joueurs atteignent l'activation partage ; ≥ 3 MJ sur 5 interrogés confirment en entretien. | 60 jours |
| **H4** | L'accès joueur sans compte n'est pas un frein à l'adoption du groupe entier. | ≥ 70 % des sessions partagées comptent au moins un joueur ayant consulté le contenu ; moins de 2 joueurs sur 10 interrogés rapportent avoir renoncé. | 60 jours |
| **H5** | La conversion du mode local vers un compte cloud se produit naturellement. | ≥ 10 % des MJ actifs en local créent un compte, le déclencheur constaté étant une intention de partage ou de sauvegarde. | 90 jours |

Seuils et délais indiqués au 2026-07-02 ; valeurs volatiles, ajustables avant le lancement de l'observation, jamais pendant. Point de référence unique et daté : cahier des charges §12.4.

## 8. Critères de succès

**Critères de décision produit.** Les seuils et délais des cinq hypothèses (section 7) constituent les critères de décision produit, chacun assorti d'un instrument de constat dédié.

**Principe de falsifiabilité.** Un seuil non atteint impose systématiquement un constat explicite — réussite partielle, échec de pilier, ou hypothèse invalidée — jamais une réinterprétation a posteriori du critère une fois l'observation engagée.

**Instruments de constat (Must Have).** Trois mesures d'activation anonymes — activation préparation, activation vue session, activation partage — complétées par une mesure d'usage anonyme en mode local et des entretiens utilisateurs, sans jamais capter de contenu narratif ni de donnée nominative.

**Critères de sortie techniques par jalon (exemples).** J0 : test d'architecture en intégration continue au vert. J1 : export versionné produit et rejouable, aucun appel réseau en mode local. J2 : migration d'un jeu de données J1 réussie de bout en bout, filtres de cloisonnement actifs sur tous les chemins de lecture. J3 : test anti-fuite en temps réel au vert.

**Cibles de friction chiffrées.** Accès à l'espace de travail en moins de deux minutes ; création d'un élément à la volée en moins de dix secondes ; résultat de recherche en moins de cinq secondes.

Les critères produit (hypothèses de validation, section 7) et les critères techniques (jalons de build, section 5) restent de nature distincte : les premiers conditionnent une décision d'investissement produit, les seconds une progression technique vérifiable en intégration continue ou par revue.

## 9. Points de décision go/no-go

| Point de décision | Nature | Ce qu'il conditionne | Critère de décision |
|---|---|---|---|
| **DÉCISION MARCHÉ** | Décision produit, non automatisable | Positionnée entre J1 et J2 ; conditionne l'engagement du build cloud + temps réel (J2/J3) | Télémétrie du jalon J1 (activations des trois piliers) croisée avec les hypothèses H1 à H4. Les seuils de décision de la décision marché elle-même restent `[À TRANCHER]`. |
| **VALIDATION JURIDIQUE EU** | Avis juridique, non automatisable | Bloque le **lancement commercial en Europe**, pas le build technique de J3 (instructible en parallèle) | Statut binaire sur cinq axes RGPD : qualification service destiné aux mineurs, posture sous-traitance / DPA, mise en balance de l'intérêt légitime et conservation, droit à l'effacement de l'espace personnel, facette RGPD de la ré-appropriation de compte en place. |
| **Jalon de validation produit** | Décision produit sur les hypothèses H1-H5 | Conditionne l'engagement de l'approfondissement du cœur produit et des paliers suivants | Résultat positif, partiel ou négatif sur les cinq hypothèses de validation (section 7) |
| **Gates techniques cumulatifs** | Vérifiables en intégration continue ou par revue | Préalables bloquants entre jalons de build successifs | J0 stabilisé avant J1 ; stockage persistant et invariant d'autorisation câblés avant J1 ; J1 stable **et** DÉCISION MARCHÉ = go avant J2 ; J2 achevé avant J3 |

La DÉCISION MARCHÉ et la VALIDATION JURIDIQUE EU se distinguent structurellement des gates techniques : l'une dépend d'une télémétrie interprétée par le porteur du projet, l'autre d'un avis juriste, et ni l'une ni l'autre ne peuvent être tranchées par un test automatisé, un linter ou un build.

## 10. Éléments à compléter

Les éléments suivants sont absents du corpus de conception, de planification ou de business actuel et ne sont donc pas renseignés dans cette note :

- `[À COMPLÉTER — produit]` Budget et coûts d'infrastructure, seuil de rentabilité (break-even).
- `[À COMPLÉTER — produit]` Dimensionnement de marché (TAM/SAM) et ARPU cible.
- `[À COMPLÉTER — produit]` Équipe nominale, affectation, organigramme.
- `[À COMPLÉTER — produit]` Matrice RACI détaillée.
- `[À COMPLÉTER — produit]` Dates fermes et calendrier d'exécution.
- `[À COMPLÉTER — produit]` Plan de canaux et tactiques d'acquisition (go-to-market).
- `[À COMPLÉTER — produit]` Périmètre géographique de lancement (l'hypothèse « francophone puis élargissement UE » n'est pas tranchée par le corpus).
- `[À COMPLÉTER — produit]` Procédure et calendrier de saisine juridique pour la VALIDATION JURIDIQUE EU.
- `[À COMPLÉTER — produit]` Seuils de décision chiffrés de la DÉCISION MARCHÉ.

## 11. Documents de référence

**Vision**
- `../conception/besoin/vision/vision-produit.md` — positionnement produit, hypothèses de validation, modèle de monétisation.
- `../conception/besoin/vision/moscow.md` — priorisation détaillée du périmètre (Must/Should/Could/Won't Have).

**Cahier des charges**
- `../context/cahier-des-charges.md` — document officiel de référence pour la construction du MVP, consolidant le corpus de conception.

**Planning**
- `roadmap-entree-build.md` — séquence technique d'entrée en build (jalons J0-J3, points de décision, préalables bloquants).
- `roadmap-produit.md` — trajectoire de valeur produit (paliers, jalon de validation, extensions par signal).
- `registre-risques.md` — registre de risques consolidé (quatre axes technique/produit/juridique/délai-coût, matrice de criticité, vue par hypothèse H1-H5, vue par point de décision).

**Business**
- `../context/note-business-gtm.md` — méthode de calcul du coût d'infrastructure, du dimensionnement de marché et de la stratégie go-to-market.

Ces renvois pointent vers les sources de conception, de planification et de business qui font foi ; cette note n'en constitue qu'une consolidation de gouvernance, sans autorité de corpus propre en cas de divergence de détail.

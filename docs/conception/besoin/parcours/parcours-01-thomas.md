# Parcours bout-en-bout — Thomas, du mode local à la migration cloud

> Objet : vérifier les coutures inter-UC sur le parcours principal de Thomas.
> Vocabulaire : [glossaire Haversack 2026-06-10](../../glossaire.md) (strict).
> Aucun nom de technologie, aucun ADR cité comme source d'autorité.
> Persona : [Thomas — MJ préparateur](../persona/persona-01-thomas.md)
> Index parcours : [README](README.md)

---

## Présentation du parcours

Thomas (persona-01) est un MJ expérimenté, méfiant vis-à-vis du lock-in. Il découvre Haversack, veut évaluer l'outil avant de s'engager, et finit par migrer vers un compte cloud pour pouvoir partager avec ses joueurs.

Le parcours couvre six étapes enchaînées. Pour chaque étape : ce que Thomas fait et obtient (comportements observables clés), l'état laissé et présupposé, et la **couture** vers l'étape suivante — vérification explicite que l'état laissé par N coïncide avec l'état présupposé par N+1.

Les déroulés pas à pas sont portés par les User Journeys cités dans chaque étape.

---

## Étape 1 — Démarrage en mode local sans compte

**UC porteur :** [UC-01](../usecases/UC-01-mode-local-sans-compte.md) | **US porteur :** [US-UC-01](../user-stories/US-UC-01-mode-local-sans-compte.md)

Déroulé détaillé : [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md)

### Ce que Thomas fait

Thomas ouvre l'application. L'écran d'accueil lui présente deux options équivalentes : « Commencer sans compte » et « Créer un compte / Se connecter ». Il choisit la première.

### Ce que Thomas obtient (comportements observables clés)

- Aucun formulaire d'inscription, aucun email demandé.
- Deux bandeaux non bloquants apparaissent : un bandeau de durabilité (conditionnel — si le navigateur ne garantit pas la conservation permanente des données) et un bandeau de confidentialité (systématique — données lisibles par toute personne ayant accès à ce navigateur).
- Les fonctionnalités de partage (UC-08) et d'accès joueur (UC-09) sont **visibles mais désactivées**, avec un appel à l'action vers la création de compte (RB-01-06, RB-01-07).
- Maximum 3 campagnes créables en mode local (RB-01-03).

### État laissé par l'étape 1

- Thomas est en **mode local** : aucun `User` instancié, aucune donnée envoyée au serveur.
- Une session navigateur locale existe, avec un identifiant opaque.
- Les données sont durables entre fermetures du navigateur (RB-01-04).

### Couture vers l'étape 2

L'étape 2 (structuration du contenu) présuppose qu'une `Campagne` peut être créée depuis le mode local. UC-02 confirme : « Le MJ peut créer un espace de jeu en mode local (sans compte) ou depuis un compte cloud. Les préconditions sont identiques dans les deux cas. » **Couture continue.**

---

## Étape 2 — Structuration du contenu : espace de jeu, scénario, documents, dossiers

**UC porteurs :** [UC-02](../usecases/UC-02-creer-espace-jeu.md), [UC-03](../usecases/UC-03-structurer-scenario.md), [UC-04](../usecases/UC-04-gerer-documents-campagne.md), [UC-05](../usecases/UC-05-organiser-dossiers.md)

Déroulés détaillés : [UJ-UC-02](../user-journeys/UJ-UC-02-creer-espace-jeu.md) · [UJ-UC-03](../user-journeys/UJ-UC-03-structurer-scenario.md) · [UJ-UC-04](../user-journeys/UJ-UC-04-gerer-documents-campagne.md) · [UJ-UC-05](../user-journeys/UJ-UC-05-organiser-contenu-dossiers.md)

### Ce que Thomas fait

Thomas crée sa campagne (UC-02), adapte les dossiers à son système de jeu (UC-05), crée des documents — fiches de PNJ, lieux, notes de préparation — avec `visibility = GM_ONLY` par défaut (UC-04), lie des documents entre eux via des `DocumentLink`, et structure un scénario avec ses scènes (UC-03).

### Ce que Thomas obtient (comportements observables clés)

- Une campagne avec une arborescence de dossiers personnalisée, persistée localement.
- Des `Document` créés, organisés, liés entre eux, tous privés par défaut.
- Un scénario structuré, exploitable en vue session.
- Tout le contenu est accessible via la recherche ([UC-14](../usecases/UC-14-recherche.md)).

### État laissé par l'étape 2

- Une `Campagne` existe en mode local, avec son identifiant, ses `Folder`, ses `Document` et ses `DocumentLink`.
- La `SessionViewConfig` de la campagne existe (créée automatiquement à `CampaignCreated`), mais ses `focusedFolders` ne sont pas encore configurés — la configuration de la vue session peut se faire avant ou pendant une session (UC-06 A2).
- Thomas est toujours en mode local, sans `User`.

### Couture vers l'étape 3

UC-06 précondition : « Une campagne existe et le MJ y a accès. » UC-06 confirme aussi : « Le MJ est authentifié **ou** en mode local sans compte. La vue session MJ est disponible dans les deux cas. » **Couture continue.**

---

## Étape 3 — Conduite d'une session avec la vue session

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) (avec [UC-07](../usecases/UC-07-creation-volee-session.md) si création à la volée)

Déroulés détaillés : [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md) · [UJ-UC-07](../user-journeys/UJ-UC-07-creation-volee-session.md)

### Ce que Thomas fait

Thomas lance une session depuis la vue campagne. La `Session` est créée directement en `LIVE`. Il navigue dans ses dossiers, consulte ses fiches, épingle des documents dans `pinnedDocumentIds`, crée des notes de session (`LIVE_NOTE`) avec `visibility = GM_ONLY` par défaut, et peut en basculer certaines en `PUBLIC`. Si un PNJ inattendu émerge, il le crée à la volée (UC-07). En fin de session, Thomas termine la session : la `Session` passe en `CLOSED`, il peut ajouter un résumé (`summary`) ou archiver.

### Ce que Thomas obtient (comportements observables clés)

- Vue session MJ fonctionnelle **en mode local** — panneaux configurables, notes de session, épingles, recherche globale.
- Notes de session créées, référencées par la session (`sessionNoteIds`).
- Cycle de vie `Session` : `LIVE → CLOSED → ARCHIVED`.
- En mode local : pas de vue joueur, pas de partage temps réel — ces fonctionnalités présupposent un compte (UC-06 précondition + RB-01-06).

### Point de friction identifié dans le corpus

La vue session MJ est disponible en mode local. Cependant, dans le scénario de Thomas, la valeur réelle qu'il cherche (partager avec ses joueurs en session) est **bloquée en mode local**. La vue session locale n'est utile à Thomas que pour piloter sa propre préparation — pas pour le partage. Cette friction est explicitement représentée dans [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md) (étape « Tenter de partager avec les joueurs »).

### État laissé par l'étape 3

- Une `Session` existe dans la campagne, en état `LIVE` ou `CLOSED`.
- Des `LIVE_NOTE` ont été créées, rattachées à la session.
- Thomas est toujours en mode local.

### Couture vers l'étape 4

UC-08 précondition : « Des joueurs ou des accès invités peuvent accéder à la campagne ou à la session. » Cette précondition n'est **pas encore satisfaite** à cette étape du parcours — Thomas est en mode local, aucun `GuestAccess` ni `SpaceMembership` ne peut exister.

**Point de friction de parcours** : pour que UC-08 soit accessible, Thomas doit avoir un compte (RB-01-06). La question est donc : **à quel moment Thomas crée-t-il son compte pour pouvoir partager ?**

Deux positionnements sont possibles selon le corpus :
- **Parcours A** : Thomas crée son compte avant la première session avec joueurs (UC-10 avant étape 3 si l'objectif du MJ inclut les joueurs dès le départ). Ce parcours est couvert par UC-10 déclencheur 1 : « Le MJ tente de partager une information avec ses joueurs (UC-08) depuis le mode local. »
- **Parcours B** : Thomas fait une session solo en mode local d'abord (étape 3), puis crée son compte (UC-10) avant d'inviter ses joueurs (UC-08 et UC-09).

Le corpus ne tranche pas explicitement l'ordre préférentiel entre ces deux parcours. [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md) (flux fonctionnel) positionne la migration **après** une tentative de partage — ce qui valide le Parcours B. **Le présent document retient le Parcours B comme parcours décrit**, cohérent avec l'intention de démarrage en mode local sans friction de Thomas.

La couture étape 3 → étape 4 est donc : Thomas tente de partager, constate que la fonctionnalité est désactivée, décide de créer un compte. La création de compte (UC-10) est le prérequis pour UC-08. **Couture conditionnelle — le parcours UC-10 s'intercale avant UC-08. Cette intercalation n'est pas modélisée comme étape explicite du parcours dans les UC existants — zone à documenter, non rupture.**

---

## Étape 4 — Partage d'information aux joueurs en cours de session

**UC porteur :** [UC-08](../usecases/UC-08-partager-information.md) | **US porteur :** [US-UC-08](../user-stories/US-UC-08-partager-information.md)

Déroulé détaillé : [UJ-UC-08](../user-journeys/UJ-UC-08-partager-information.md)

**Précondition résolue :** Thomas a créé son compte (UC-10) avant cette étape — voir couture étape 3 → 4 ci-dessus.

### Ce que Thomas fait

Depuis la vue session ou depuis la bibliothèque de contenu, Thomas ouvre un document et clique « Partager ». La `visibility` du document passe de `GM_ONLY` à `PUBLIC`. Ce changement est **durable** : le document reste visible entre les sessions jusqu'à retrait explicite. Depuis la vue session, le document partagé est automatiquement ajouté à `pinnedDocumentIds`. Thomas peut retirer le partage à tout moment (`Unshare()`).

### Ce que Thomas obtient (comportements observables clés)

- Le document est visible par les joueurs ayant accès à la campagne ou à la session.
- Les notes privées du MJ restent protégées.
- Le partage est durable entre sessions sauf retrait explicite.
- En mode compte actif : le partage est temps réel (les joueurs voient immédiatement le document).

### Limite du corpus (UC-08, règle métier)

Le partage sélectif par joueur ou personnage **n'est pas dans le MVP** (UC-08 règle métier). Tout document partagé est visible par tous les joueurs autorisés.

### État laissé par l'étape 4

- Au moins un `Document` avec `visibility = PUBLIC` existe dans la campagne.
- Thomas a un compte actif (`AccountTier = FREE` au minimum).
- La campagne existe en mode cloud synchronisé (migration effectuée lors de UC-10).

### Couture vers l'étape 5

UC-09 précondition : « Le MJ a un compte (au minimum gratuit) » et « Le MJ a généré un lien de session depuis UC-11 ou UC-08. » Thomas a maintenant un compte gratuit. **Couture continue à condition que Thomas génère un lien de session** — la génération du lien est dans UC-09 scénario nominal (Thomas génère le lien) et dans UC-08 (partage qui produit des `GuestAccess`). Les deux UC convergent sur cette nécessité. **Couture continue.**

---

## Étape 5 — Accès d'un joueur sans compte à la session

**UC porteur :** [UC-09](../usecases/UC-09-acces-session-joueur.md) | **US porteur :** [US-UC-09](../user-stories/US-UC-09-acces-session-joueur.md)

Déroulé détaillé : [UJ-UC-09](../user-journeys/UJ-UC-09-acces-session-joueur.md)

### Ce que Thomas fait

Thomas génère un lien de session ponctuel depuis la vue session ou le panneau membres (UC-09 scénario nominal, UC-11). Il partage ce lien. Le joueur clique sur le lien.

### Ce que le joueur obtient (comportements observables clés)

- Page d'accueil immédiate avec les informations partagées par Thomas pour cette session.
- Saisie d'un **nom d'affichage uniquement** (pas d'email, pas de mot de passe).
- Un `GuestAccess` de portée `SESSION` est créé : `status = ACTIVE`.
- Le joueur accède à la **vue session joueur** : documents `PUBLIC`, documents épinglés, zone de notes de session personnelles.
- Le joueur peut créer des notes de session avec `visibility = PLAYER_PRIVATE`, liées à son personnage associé si renseigné.
- L'accès expire à la fermeture de la session + 24 heures (fenêtre de grâce).

### Règle de limite : compte FREE

Thomas a un compte `FREE` : maximum **4 joueurs par session** (UC-09 règle métier, UC-01 modèle d'accès). Au-delà, le compte `PRO` est nécessaire.

### État laissé par l'étape 5

- Un ou plusieurs `GuestAccess` (`status = ACTIVE`, portée `SESSION`) existent dans la campagne.
- Des `LIVE_NOTE` avec `visibility = PLAYER_PRIVATE` peuvent avoir été créées par les joueurs.
- La session est toujours `LIVE` ou vient de passer en `CLOSED`.
- Thomas est sur un compte `FREE`.

### Couture vers l'étape 6

UC-10 (migration vers un compte cloud) est déjà réalisé dans le parcours décrit — Thomas a créé son compte avant l'étape 4. Cette étape 6 couvre donc une situation différente : la migration complète des données locales **qui n'auraient pas encore été migrées** (si Thomas a créé son compte en urgence pour partager une information, certaines campagnes locales peuvent être restées en attente de migration — voir UC-10 E5 : état « migration en attente »). Alternativement, si Thomas a un compte mais n'a pas encore migré toutes ses campagnes locales, le gate de reconnaissance peut être présenté à tout moment depuis son espace de travail.

**Cette étape est donc couverte comme finalisation de la migration, pas comme première création de compte.** Couture : Thomas accède à son espace de travail, y trouve ses campagnes importées et ses campagnes locales en attente. **Couture continue.**

---

## Étape 6 — Migration vers un compte cloud avec gate de reconnaissance

**UC porteur :** [UC-10](../usecases/UC-10-compte-cloud.md) | **US porteur :** [US-UC-10](../user-stories/US-UC-10-compte-cloud.md)

Déroulé détaillé : [UJ-UC-10](../user-journeys/UJ-UC-10-compte-cloud.md)

### Ce que Thomas fait

Thomas crée son compte (si pas encore fait) ou reprend une migration en attente. Le déclencheur principal dans ce parcours : Thomas tente de partager une information avec ses joueurs depuis le mode local — la fonctionnalité est désactivée, il clique sur le CTA « Créer un compte ».

Il renseigne email, nom d'affichage, mot de passe. Le compte est créé (tier `FREE`).

L'application présente le **gate de reconnaissance** : les campagnes locales détectées (titres, volume estimé, date de création) sont listées. Thomas doit confirmer explicitement avant que la migration commence. Ce gate protège contre l'appropriation accidentelle de données d'un tiers.

Après confirmation, les campagnes sont importées **campagne par campagne, tout-ou-rien par campagne**. En cas d'échec d'une campagne, les données locales correspondantes sont conservées intégralement. Un rapport de rejets est présenté.

Thomas retrouve son espace de travail intact pour les campagnes importées, maintenant synchronisé.

### Ce que Thomas obtient (comportements observables clés)

- Compte actif (`AccountStatus = ACTIVE`, `AccountTier = FREE`).
- Campagnes migrées accessibles en cloud (3 maximum en `FREE`), avec tout leur historique de session (sessions terminées, notes de session, documents épinglés, résumés) migré et consultable.
- Une session en cours (`LIVE`) détectée est signalée par le gate : elle doit être clôturée avant migration (RB-10-04, RB-10-09).
- La configuration de la vue session (`SessionViewConfig`) est recréée après migration — les panneaux configurés en local ne sont pas repris.
- Partage et accès joueur désormais disponibles.
- Données locales conservées pour les campagnes rejetées.

### État final

- Thomas est `User` authentifié.
- Ses campagnes sont synchronisées en cloud.
- Il peut partager, inviter des joueurs, accéder depuis d'autres appareils.

---

## Coutures vérifiées / Ruptures constatées

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Mode local → création de campagne | 1 → 2 | **Continue** | UC-02 précondition = application accessible en mode local ou cloud. Correspondance confirmée. |
| C2 | Campagne locale → lancement de session | 2 → 3 | **Continue** | UC-06 précondition = campagne existante + MJ authentifié ou en mode local. Les deux chemins sont explicitement couverts. |
| C3 | Session locale → partage joueurs | 3 → 4 | **Continue (documentée)** | La précondition UC-08 (joueurs/`GuestAccess` accessibles) n'est pas satisfaite en mode local. UC-10 doit s'intercaler. Le parcours « session solo mode local → création de compte → session avec joueurs » est désormais formulé dans UC-10 (Contexte) et dans UJ-UC-01 (flux fonctionnel : historique de session retrouvé, enchaînement vers l'invitation des joueurs) et UJ-UC-10 (section « Après la migration — Continuité vers le partage »). La continuité de l'historique de session à la migration est garantie par UC-10 (RB-10-04, RB-10-09). |
| C4 | Compte actif → partage d'un document | 4 (après UC-10) | **Continue** | UC-08 précondition = campagne existante + joueurs/`GuestAccess` accessibles. Satisfaite dès qu'un `GuestAccess` ou `SpaceMembership` existe. Le lien de session est généré par Thomas (UC-09 nominal). |
| C5 | Document partagé → accès joueur sans compte | 4 → 5 | **Continue** | UC-09 précondition = compte MJ `FREE` minimum + lien généré. Satisfait après UC-10. Le `GuestAccess` créé à l'arrivée du joueur consomme le document `PUBLIC`. Correspondance vérifiée. |
| C6 | Accès invité → migration cloud | 5 → 6 | **Continue** | La migration (UC-10) est positionnée avant l'étape 4 dans ce parcours. L'étape 6 couvre la finalisation pour les campagnes locales en attente (UC-10 E5). Mécanisme de reprise de migration documenté dans UC-10. |
| C7 | Limite `FREE` : partage avec joueurs | 4 + 5 | **Continue** | UC-01 et UC-09 convergent sur la règle : compte `FREE` = 4 joueurs max par session. Cohérence entre RB-01-06 (US-UC-01), UC-09 règle métier et UC-01 modèle d'accès. |
| C8 | `SessionViewConfig` créée à `CampaignCreated` | 2 → 3 | **Continue** | Le glossaire confirme : une `SessionViewConfig` est créée automatiquement à `CampaignCreated`. La vue session peut donc s'ouvrir même sans configuration préalable des panneaux (message d'invitation à configurer à la première session). |
| C9 | Notes personnelles joueur (`PLAYER_PRIVATE`) en mode local | 3 (mode local) | **Continue (limitation formulée)** | UC-06 précondition : « La vue joueur (partage, accès invité) nécessite un compte. » En mode local, les joueurs n'ont pas de vue session. Les notes de session `PLAYER_PRIVATE` d'un joueur présupposent un `GuestAccess` actif ou un `SpaceMembership`, ce qui est impossible en mode local. La limitation « en mode local pur, aucun joueur ne peut prendre de notes de session » est désormais énoncée explicitement dans UC-01 (fonctionnalités indisponibles) et UC-06 (précondition + règle métier « Mode local et vue joueur »). |

---

## Ruptures nouvelles (non listées dans l'audit CP-01 à CP-22)

### RUPTURE-N1 — Absence de modélisation du parcours « session solo mode local → création de compte → session avec joueurs »

**Localisation :** Couture C3 (étape 3 → 4). Non répertoriée dans l'audit.

**Constat :** La chronologie du parcours Thomas implique nécessairement un moment où le MJ bascule du mode local (session solo) vers le mode cloud (session avec joueurs). Ce moment est représenté comme déclencheur dans UC-10 (« Le MJ tente de partager une information avec ses joueurs depuis le mode local ») mais n'est pas modélisé comme étape de parcours dans aucun UC, UJ ou US. UJ-UC-01 le couvre partiellement (la tentative de partage comme point de conversion), mais le parcours post-migration — retour en session après migration, avec les mêmes campagnes maintenant synchronisées — est absent de UJ-UC-01 et non couvert par UJ-UC-10.

**Impact :** Un MJ qui a conduit une session en mode local et veut ensuite inviter ses joueurs dans cette même campagne ne dispose pas d'une documentation de parcours pour ce cas. La migration transfère les campagnes, mais le comportement de la session `CLOSED` ou `LIVE` après migration (les `LIVE_NOTE` créées en mode local sont-elles visibles en cloud ? les `pinnedDocumentIds` sont-ils migrés ?) n'est pas spécifié dans les UC existants.

**Consigne :** Ne pas résoudre ici. À ouvrir comme zone de spécification dans UC-10 et/ou UJ-UC-10.

**Statut : refermée (2026-06-10)** — le parcours est désormais formulé dans UC-10 (Contexte), UJ-UC-01 (flux fonctionnel : historique de session retrouvé, enchaînement vers l'invitation des joueurs) et UJ-UC-10 (section « Après la migration — Continuité vers le partage »). Le comportement de l'historique de session après migration est spécifié dans UC-10 : sessions terminées, notes de session et documents épinglés migrés et consultables ; une session LIVE doit être clôturée avant migration.

---

### RUPTURE-N2 — Précondition UC-09 citant UC-11 comme source du lien de session, UC-11 hors périmètre de ce parcours

**Localisation :** UC-09 précondition : « Le MJ a généré un lien de session depuis UC-11 ou UC-08. »

**Constat :** UC-11 (Gérer les membres d'une campagne) est cité comme source du lien de session dans UC-09. Ce parcours ne passe pas par UC-11 explicitement — Thomas génère un lien ponctuel directement depuis la vue session (UC-09 scénario nominal). La relation UC-09 ↔ UC-11 pour la génération du lien est mentionnée mais pas détaillée dans le parcours Thomas. UC-11 est un UC à part entière (gestion des membres permanents de la campagne), distinct du lien ponctuel `GuestAccess` couvert par UC-09. La frontière entre les deux n'est pas formalisée dans le parcours.

**Impact :** La génération du lien de session est présupposée par UC-09 mais son origine (UC-11 vs. action inline dans la vue session) n'est pas documentée dans UC-06 ni UC-08. Un lecteur ne peut pas déterminer, depuis UC-06 ou UC-08 seuls, comment Thomas génère concrètement le lien qu'il partage à ses joueurs.

**Consigne :** À documenter dans UC-06 (relation avec la génération de lien de session) ou dans UC-09 (clarifier que le lien ponctuel peut être généré inline sans passer par UC-11).

**Statut : refermée (2026-06-10)** — l'origine du lien de session est documentée : UC-06 énonce que le MJ génère le lien de session ponctuel depuis la vue session (règle métier + relation vers UC-09) ; la précondition d'UC-09 est alignée sur son scénario nominal ; le lien de campagne permanent relève d'UC-11.

---

### RUPTURE-N3 — État des `LIVE_NOTE` et `pinnedDocumentIds` après migration locale→cloud non spécifié

**Localisation :** UC-10 E5 (migration en attente), UC-01 règle métier (migration campagne par campagne).

**Constat :** UC-10 spécifie que la migration est « tout-ou-rien par campagne » et que les campagnes importées sont accessibles. Il ne précise pas si les `Session` créées en mode local (avec leurs `LIVE_NOTE`, `pinnedDocumentIds`, `summary`) sont migrées ou abandonnées. Le glossaire définit `Session` comme un agrégat de `Session Conduct` — son sort lors d'une migration locale→cloud n'est pas couvert par UC-10 ni par UC-01.

**Impact :** Si Thomas a conduit une session en mode local, ses notes de session et épingles peuvent être perdues à la migration. Ce point est structurant pour la promesse de confiance/possession des données (persona Thomas, douleur principale).

**Consigne :** À spécifier dans UC-10 (sort des `Session` et `LIVE_NOTE` lors de la migration) ou en règle métier complémentaire.

**Statut : refermée (2026-06-10)** — UC-10 spécifie désormais le sort de l'historique de session à la migration : tout l'historique (sessions terminées, notes de session, documents épinglés, résumés) migre avec sa campagne ; rien n'est perdu ; une session en cours (LIVE) doit être clôturée avant migration et le gate de reconnaissance le signale ; la configuration de la vue session est recréée. Règles portées par UC-10 (règle métier dédiée) et US-UC-10 (RB-10-04, RB-10-09).

---

## Points de friction du parcours

- **Mode local et partage** : la valeur principale que Thomas cherche (partager avec ses joueurs) est désactivée en mode local. La vue session locale sert uniquement à la préparation solo. Le point de conversion vers la création de compte est la tentative de partage — documentée dans [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md).

- **Intercalation de UC-10 avant UC-08** : le parcours impose une création de compte (UC-10) entre la session locale (étape 3) et le partage (étape 4). Cette intercalation n'est pas modélisée comme étape explicite dans les UC existants. Elle est couverte comme zone à documenter dans la couture C3.

- **Limite FREE et joueurs** : Thomas est limité à 4 joueurs par session en compte `FREE`. Ce plafond est cohérent entre UC-01, UC-09 et les règles métier, mais il peut devenir un point de friction pour son groupe de 6 joueurs — la mise à niveau vers `PRO` est le chemin attendu.

- **`SessionViewConfig` non reprise à la migration** : après migration locale→cloud, les panneaux de la vue session configurés en local ne sont pas repris. Thomas devra reconfigurer sa vue session en cloud (documenté dans UC-10 comportements observables).

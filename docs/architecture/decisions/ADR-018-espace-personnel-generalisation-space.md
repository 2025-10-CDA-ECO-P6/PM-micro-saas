# ADR-018 — Contenu personnel de premier ordre : généralisation de `Campaign` en `Space`

- **Statut** : Accepté — décision de conception pré-implémentation, à confirmer à l'entrée en build
- **Date** : 2026-06-12
- **Décideur** : opérateur (validation explicite, session d'exploration de conception)
- **Décisions liées** : ADR-011 (cascade `CampaignDeleted` / `UserAnonymized`), ADR-012 (RGPD effacement de compte), ADR-013 (données invités), ADR-017 §1.1 (modèle IndexedDB, store campaign-rooté)

---

## Périmètre de cet ADR

Cet ADR acte la généralisation de l'agrégat `Campaign` en `Space` pour permettre au contenu documentaire d'exister de manière indépendante de toute campagne, en introduisant `SpaceType.PERSONAL` comme conteneur de premier ordre appartenant au MJ.

**Dans le périmètre** :
- Motivation et analyse d'impact du besoin de contenu personnel.
- Sélection et justification de la voie retenue (Voie 3 — `Space`), avec les deux alternatives écartées.
- Analyse d'impact invariant par invariant sur le corpus de conception existant.
- Impact sur les sagas RGPD (ADR-011, ADR-012) et le mode local (ADR-017).
- Recommandations MoSCoW (propriété vs activation vs bibliothèque de réutilisation).
- Description de la chaîne d'artefacts de remédiation à produire dans un Build-out séparé.

**Hors périmètre de cet ADR** :
- Réécriture effective du corpus de domaine (renommage `Campaign` → `Space` dans les quatre fichiers domaine, le glossaire et les diagrammes) — **propriété de la session de remédiation Build-out `+conception`**.
- Modification des use cases existants (UC-01, UC-02, UC-13).
- Implémentation du store IndexedDB réécrit — renvoyé à ADR-016/017 au moment de la réécriture.
- Résolution des incohérences de corpus listées en fin de document.

---

## Contexte

### Le besoin passé sous couvert

Le processus créatif du MJ praticien produit fréquemment des idées — lieu, PNJ, scénario, objet, règle maison — **sans lien avec une campagne en cours et sans intention d'en créer une**. Un MJ peut noter une idée de donjon à trois heures du matin, esquisser un PNJ récurrent, ou préparer un scénario de convention des semaines avant que la logistique du groupe soit réglée.

Ce besoin est **central** — il a été identifié comme passé sous couvert lors de la conception du corpus. L'enjeu est de reconnaître le contenu personnel comme un **contenu de premier ordre, appartenant au MJ, indépendant de toute campagne, potentiellement permanent**.

La campagne est *une façon d'organiser et d'utiliser une partie du contenu* — elle n'est pas un prérequis de création. Cette reformulation inverse l'ontologie actuelle du corpus : aujourd'hui, la campagne est la racine et le contenu lui est rattaché ; la décision actée ici fait de l'**espace** la racine, dont la campagne est une spécialisation.

### État du corpus au moment de la décision

L'agrégat `Campaign` du domaine Campaign Management porte aujourd'hui `type: CampaignType ∈ {CAMPAIGN, ONE_SHOT}` (campaign-management.md l.46). Le type `ONE_SHOT` est déjà une spécialisation comportementale d'une `Campaign` dont les différences sont portées par `type`, pas par la structure (campaign-management.md — section « One-shot — spécificités »). Ce précédent est le point de départ de la généralisation.

Tous les Documents du domaine Content Library portent un `campaignId: CampaignId` non-nullable (content-library.md l.64). La règle métier 5 d'Identity & Access affirme : « les contenus créés (documents, notes) restent attachés à la campagne sous identité anonymisée — ils appartiennent à la campagne, pas à l'individu » (identity-access.md l.100). Trois tensions caractérisent l'état actuel face au besoin identifié :

1. Tout document exige une campagne existante à sa création — il n'existe pas de zone d'atterrissage naturelle pour le contenu pré-campagne.
2. La ScenarioLibrary (campaign-management.md l.154-183, statut post-MVP) tente de répondre partiellement au besoin de réutilisation inter-campagnes mais introduit une entité de pont (`ScenarioLibraryEntry`) et ne couvre que les scénarios, pas l'ensemble des types de documents.
3. L'UC-F07 (UC-HORS-MVP.md l.39-47) reconnaît le besoin de « créer sans campagne » et le qualifie de palliatif via une « campagne-atelier » — signal que l'ontologie actuelle résiste à ce besoin.

---

## Décision

### Voie retenue — Voie 3 : généralisation de `Campaign` en `Space`

**L'agrégat `Campaign` est généralisé en `Space` (Espace).** Un `Space` porte `SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`. La campagne et le one-shot deviennent des spécialisations d'un espace, au même titre que l'espace personnel.

**Propriétés structurelles** :
- L'agrégat `Space` reprend tous les champs de l'agrégat `Campaign` existant (`ownerId`, `name`, `slug`, `type` — renommé depuis `CampaignType`, `status`, `memberships`, `invitations`, etc.).
- `SpaceType` remplace `CampaignType` et ajoute la valeur `PERSONAL`.
- Un espace de type `PERSONAL` est **créé par défaut** au moment de la création du compte utilisateur. Il est possédé par l'`ownerId` existant (même FK `owner_id → users.id` documentée dans ADR-009).
- `Document.spaceId` (renommé depuis `campaignId`) reste **non-nullable** — l'espace personnel est le conteneur par défaut pour tout document créé sans espace explicite.
- L'espace personnel est **mono-membre** : le propriétaire est son seul membre. La question de la gestion des membres (`memberships`) d'un espace `PERSONAL` est résolue par la règle : un espace `PERSONAL` ne peut avoir qu'un membre (`OWNER` = le propriétaire). À préciser à la modélisation.

**Renommage ubiquitaire** : toute occurrence de `Campaign`, `CampaignId`, `CampaignType`, `CampaignCreated`, `CampaignDeleted`, etc. est renommée en `Space`, `SpaceId`, `SpaceType`, `SpaceCreated`, `SpaceDeleted`, etc. Ce renommage s'applique au domaine (quatre fichiers), au glossaire, aux diagrammes, aux use cases concernés, et à la chaîne RGPD (ADR-011/012 sont annotés au moment de la réécriture). **Ce renommage est le chantier de la session de remédiation — le présent ADR le décrit, il ne l'exécute pas.**

---

## Alternatives considérées

### Voie 1 — Racine sur l'Utilisateur (`ownerId` de premier ordre sur `Document`, `campaignId` nullable)

Cette voie aurait rendu le champ `Document.campaignId` nullable et fait de `Document.ownerId: UserId` une propriété de premier ordre.

**Rejetée pour flaw STRUCTURAL — trois motifs indépendants, chacun suffisant :**

**(a) Violation de la frontière de contexte Identity & Access.** La règle métier 5 d'Identity & Access affirme explicitement : « les contenus créés restent attachés à la campagne sous identité anonymisée — ils appartiennent à la campagne, pas à l'individu » (identity-access.md l.100). Ce n'est pas seulement une règle métier : c'est la définition de la frontière entre Identity & Access et Content Library. I&A déclare elle-même : « Ce contexte ne sait pas ce qu'est une campagne, un MJ, un joueur ou une session. » (identity-access.md l.6-7). Faire porter à `Document` une propriété `ownerId: UserId` de premier ordre crée un couplage entre Content Library et Identity & Access — précisément le couplage que la frontière de contexte interdit.

**(b) Résidu post-effacement RGPD — manquement dur.** Un document avec `campaignId = NULL` n'est atteint par **aucune** des deux sagas définies dans ADR-011. La saga `CampaignDeleted` filtre sur `campaign_id` (ADR-011 — passe 1 et passe 2, toutes les opérations portent le filtre `WHERE campaign_id = C`). La saga `UserAnonymized` ne touche que les documents `PLAYER_PRIVATE` liés à un `membership_characters` sur des campagnes vivantes (ADR-011 — saga `UserAnonymized`, §3 ADR-012). Un document `campaignId = NULL` survivrait aux deux sagas : résidu post-Art. 17 caractérisé. Cette voie exigerait une troisième saga entièrement nouvelle, racine `owner_id`, doublant la surface de test de la machinerie RGPD.

**(c) Incompatibilité avec le mode local.** ADR-017 §1.1 acte explicitement : les object stores `users`, `campaign_memberships`, `membership_characters` sont **exclus** du store IndexedDB local — « ces exclusions ne sont pas des omissions de simplification — elles délimitent ce que le mode local UC-01 contient » (ADR-017 §1.1). En mode local, un `User` n'existe pas (identity-access.md l.77 : « le mode local n'est pas un tier — il n'y a pas de `User` en mode local »). Un `Document.ownerId: UserId` de premier ordre n'a pas de valeur assignable en mode local : la propriété naît à la création de compte, pas avant. Le mode local deviendrait une exception structurelle nécessitant un traitement spécial au lieu de s'inscrire naturellement dans le modèle.

**Format vision §5bis** :
- *Décision* : racine sur l'utilisateur — rejetée.
- *Raison d'être* : intuition de simplification (contenu directement possédé).
- *Alternatives considérées* : `campaignId` nullable + `ownerId` de premier ordre.
- *Condition de retour* : n/a — flaw structural sur trois axes indépendants.

---

### Voie 2 — Type `PERSONAL` sur l'agrégat `Campaign` non renommé

Cette voie aurait ajouté `CampaignType.PERSONAL` à l'agrégat `Campaign` existant sans renommage. Elle est un repli viable.

**Tenants** : coût de mise en œuvre minimal (pattern déjà établi avec `ONE_SHOT`), tous les invariants de domaine actuels préservés sans réécriture, machinerie RGPD (ADR-011/012) réutilisée verbatim, mode local résolu sans modification du store.

**Aboutissants** : dette sémantique significative. L'utilisateur dont le contenu personnel est stocké dans « une campagne nommée Personnel » vit exactement l'ontologie que le besoin dénonce : *« je dois créer une campagne pour ça »*. La correction technique est masquée derrière un terme qui réintroduit le biais campagne-centré. Par ailleurs, la ScenarioLibrary (campaign-management.md l.154-183) reste une entité de pont artificielle — elle crée un couplage `ScenarioLibraryEntry ↔ ownerId` qui ne trouve pas de résolution naturelle dans ce modèle.

**Format vision §5bis** :
- *Décision* : `PERSONAL` sur `Campaign` non renommée — repli viable, non retenu.
- *Raison d'être* : coût minimal, invariants préservés.
- *Alternatives considérées* : voie retenue (Voie 3).
- *Condition de retour* : revenir vers la Voie 3 dès que la ScenarioLibrary entre en jeu (condition de retour déjà active au moment de cette décision).

---

## Conséquences

### Analyse d'impact — invariant par invariant sous Voie 3

#### Content Library — invariant 1 : « Un `Document` appartient à exactement un `Folder` »

**Préservé sans modification.** La relation `Document → Folder` est intra-module (Content Library) et ne dépend pas du type de `Space` parent.

#### Content Library — invariant 2 : « Un `Folder` appartient à exactement une `Campaign` »

**Réécrit** : « Un `Folder` appartient à exactement un `Space` ». Le sens de l'invariant — unicité et non-nullabilité du conteneur d'appartenance — est intégralement conservé. Seul le terme change.

#### Content Library — invariant 4 : dossier virtuel « Non classés » par campagne, garantit `folderId` non-nullable sur `Document`

**Réécrit** : « Un dossier virtuel « Non classés » est créé par espace à la création de l'espace ». Le sens est conservé : le dossier virtuel garantit que `folderId` reste non-nullable sur `Document`, quelle que soit la nature de l'espace (CAMPAIGN, ONE_SHOT, PERSONAL). Ce pattern était déjà un précédent de conception — un défaut système qui préserve la contrainte non-nullable — il est transposé d'un cran au-dessus (de la campagne à l'espace) sans en changer la nature.

#### Identity & Access — règle métier 5 : « les contenus appartiennent à la campagne, pas à l'individu »

**Préservé dans son esprit.** La chaîne de possession est désormais : `Document → Space → ownerId`. Le contenu appartient à l'**espace**, et l'espace est possédé par l'`ownerId` (comme `Campaign` aujourd'hui via ADR-009). L'interdiction de couplage direct Content Library → I&A reste intacte : un document ne porte pas d'`ownerId` ; il porte un `spaceId`.

La règle est à reformuler ainsi dans le domaine I&A : « les contenus appartiennent à l'espace, pas à l'individu. L'espace personnel appartient à l'individu. »

#### Visibilité — enum `Visibility` (`PUBLIC`, `GM_ONLY`, `PLAYER_PRIVATE`) dans Core

**Préservé sans modification de l'enum.** La résolution de la visibilité se fait contre les membres d'un espace. Un espace `PERSONAL` étant mono-membre (le propriétaire), `GM_ONLY` et `PLAYER_PRIVATE` ont un effet de bord de facto « visible du propriétaire uniquement ». La sémantique exacte de `PLAYER_PRIVATE` sur un espace `PERSONAL` (est-ce un état légalement valide ? un état à restreindre à l'interface ?) est à préciser à la modélisation. Ce point n'affecte pas la définition de l'enum Core, qui reste partagé et stable.

#### Dossiers — portée

**Réécrit** : les dossiers passent de campagne-scopés à espace-scopés. Un espace personnel dispose de sa propre arborescence de dossiers, créée à la création de l'espace (dossiers système + dossier virtuel « Non classés »). Cette arborescence est un bénéfice direct : la structuration du contenu personnel est gratuite, elle reprend le même mécanisme.

#### Quotas — stockage

Les quotas actuels sont définis en Mo par tier (identity-access.md — `AccountTier`). Cette définition est par stockage, non par nombre de campagnes. L'introduction d'un espace personnel s'intègre naturellement dans cette logique : le stockage de l'espace personnel compte dans le quota Mo du compte, sans qu'une règle additionnelle soit nécessaire.

#### Cascade RGPD — ADR-011

**Réécrit sur `space` : toute la machinerie est préservée, seul le terme racine change.**

La racine `campaign_id` dans la matrice des 38 FK (ADR-011) devient `space_id`. Les deux passages de la saga `CampaignDeleted` (renommée `SpaceDeleted`) — passe 1 de déliaison, passe 2 DELETE topologique — restent valides sur un graphe renommé. L'invariant de visibilité du soft-delete (jointure sur `spaces.deleted_at IS NOT NULL`) s'applique de façon identique. L'espace `PERSONAL` est une racine purgeable comme toute autre : si un utilisateur supprime son compte, l'espace personnel est purgé via la saga `SpaceDeleted`.

**Point d'attention** : la saga `UserAnonymized` (ADR-011) filtre sur les campagnes avec `deleted_at IS NULL` pour s'abstraire des campagnes en corbeille. Cette logique s'applique identiquement aux espaces — `UserAnonymized` s'abstient sur tout espace dont `deleted_at IS NOT NULL`.

#### Effacement de compte — ADR-012

**Une catégorie est ajoutée au tableau §2 d'ADR-012.** Le tableau des données du compte et de leur sort à l'effacement doit inclure la catégorie suivante :

| Catégorie | Données | Sort à l'effacement | Base légale si conservée |
|---|---|---|---|
| Contenu de l'espace `PERSONAL` | Tous les `Document`, `Folder`, `DocumentBlock`, `DocumentLink`, `DocumentTag`, `DocumentType` custom rattachés à l'espace personnel de l'utilisateur | **Hard-delete inconditionnel** déclenché par l'événement `UserDeleted`, via la saga `SpaceDeleted` sur l'espace personnel | Aucune — aucun tiers, aucun intérêt légitime de continuité Art. 17§3(e) |

**La sélection §3(a/b/c) d'ADR-012 n'est pas simplifiée.** Elle reste applicable pour le contenu des espaces partagés (CAMPAIGN, ONE_SHOT avec membres). L'espace `PERSONAL` bénéficie d'un traitement distinct car son contenu est par construction sans tiers.

**Vigilance — surcharge explicite du défaut UC-11/ADR-013 §6.** ADR-013 §6 et ADR-011 décrivent le comportement MVP pour un MJ propriétaire anonymisant son compte : la campagne est conservée sous l'`id` anonymisé (`owner_id NOT NULL` satisfait). Ce comportement s'applique aux espaces `CAMPAIGN` et `ONE_SHOT` avec membres. **Un espace `PERSONAL` ne doit PAS survivre sous identité anonymisée.** La purge de l'espace personnel est inconditionnelle à `UserDeleted`, contrairement aux espaces partagés. Ce point est une surcharge explicite du défaut — il doit être documenté dans la saga lors de la réécriture.

#### Mode local — ADR-017 §1.1

En mode local, il n'y a pas de `User` (identity-access.md l.77, ADR-017 §1.1 exclusions). La propriété d'un espace (`ownerId`) n'existe donc qu'après la création d'un compte. En mode local, l'espace personnel est un **simple conteneur** : le store IndexedDB ignore `type` et `ownerId` pour les mêmes raisons qu'il ignore aujourd'hui les entités d'identité. Le store local reste aggregate-rooted sur l'objet `Space` (renommé depuis `campaigns`), sans changement de structure logique.

Ce point est à documenter dans la révision d'ADR-016/017 au moment de la réécriture du domaine.

#### ScenarioLibrary, UC-13, UC-F07

**Ces trois éléments sont subsumés par la décision.**

Un scénario réutilisable, dans le nouveau modèle, est un `Document` appartenant à l'espace `PERSONAL` du MJ avec `isReusable = true`. L'instanciation dans un espace `CAMPAIGN` ou `ONE_SHOT` s'effectue via `Document.Instantiate(spaceId, folderId)` — méthode déjà présente dans le domaine Content Library (content-library.md l.92 ; signature post-renommage, aujourd'hui `Document.Instantiate(campaignId, folderId)`). Le pont `ScenarioLibraryEntry` (promotion → index) devient largement superflu : la bibliothèque personnelle est l'espace personnel lui-même.

UC-F07 (« créer un scénario directement dans la bibliothèque sans campagne », UC-HORS-MVP.md l.39-47), qualifié de palliatif dans le corpus, devient le **cas nominal** : créer un document dans l'espace personnel est le flux de base.

La relation à UC-13 (Sonia — « unité = scénario », bibliothèque niveau compte, UC-13-scenario-reutilisable.md) est à expliciter dans la révision d'UC-13 : UC-13 A1 (marquer un scénario comme template) devient « déplacer ou copier vers l'espace personnel » ; UC-13 scénario nominal (rejouer depuis la bibliothèque) devient « instancier depuis l'espace personnel ».

---

## Recommandations MoSCoW

### Propriété / schéma — ACTÉ MAINTENANT (Must Have schéma anticipé)

L'ontologie `Space + SpaceType.PERSONAL + espace personnel par défaut` est actée comme schéma anticipé, au même titre que `documentTypeId`, la valeur `EMAIL` de `InvitationType`, `FROZEN` dans `CampaignStatus`, ou la `ScenarioLibrary` modélisée en post-MVP (moscow.md — doctrine du schéma anticipé).

**Motif** : la migration inverse — re-router la propriété sur la table `Document` après que `Document.campaignId` serait devenu une convention établie, réconcilier les `slug` uniques par `(spaceId, documentTypeId)` — est la plus coûteuse des migrations futures possibles. L'acte de nommage précoce (`Space` plutôt que `Campaign`) évite cette migration.

### Geste capture-first — espace personnel comme zone d'atterrissage par défaut — RECOMMANDÉ EN MVP

**Recommandation** : rendre l'espace personnel opérationnel en MVP — notamment comme zone d'atterrissage par défaut pour les documents créés sans espace explicite, y compris depuis le mode local.

**Caveat — DÉCISION PRODUIT à confirmer par l'opérateur au gate** : cette recommandation redéfinit H1 (vision-produit.md §2.3 — « activation préparation » doit-elle compter le contenu de l'espace personnel ?). Elle change également l'onboarding UC-01/UC-02 : le MJ en mode local peut créer du contenu sans créer de campagne. Ce changement de posture est substantiel et doit être validé explicitement avant d'être inscrit dans les user stories.

### Bibliothèque de réutilisation inter-espaces — POST-MVP

La fonctionnalité de promotion, catalogue, et instanciation inter-espaces reste post-MVP, alignée sur UC-13 et déclenchée par la condition de retour existante (vision-produit.md §5bis — condition de retour « one-shot »). L'espace personnel fournit l'infrastructure nécessaire ; l'interface de bibliothèque (navigation, recherche, instanciation en un geste) est un investissement d'UX post-MVP.

---

## Chaîne d'artefacts à construire ensuite

La remédiation effective du corpus relève d'un **Build-out `+conception` séparé**, coordonné avec la session de remédiation en cours. Le présent ADR la décrit ; il ne l'exécute pas.

**Persona** : pas de nouveau persona. Ajout d'une facette aux personas Sonia (persona-07) et Antoine (persona-05) : « le contenu m'appartient, la campagne en organise une partie ». Thomas (persona-01) porte l'argument de possession autonome.

**Vision** : ajout d'une trace d'arbitrage §5bis pour la décision « Space / espace personnel ».

**Use cases** : évolution d'UC-13 (bibliothèque = espace personnel, instanciation depuis `Document.Instantiate`) et création éventuelle d'un UC-Fxx « Contenu personnel rattaché au compte » au voisinage d'UC-F07.

**Domaine** : renommage de `Campaign` → `Space` dans les quatre fichiers domaine (campaign-management.md, content-library.md, identity-access.md, core.md si applicable) + glossaire + diagrammes. Marquage « modélisé, non implémenté » pour le comportement de bibliothèque de réutilisation post-MVP.

---

## Points à trancher

- **Sémantique de `Visibility` sur un espace `PERSONAL`** : `GM_ONLY` et `PLAYER_PRIVATE` n'ont pas de sens naturel sur un espace mono-membre. Valider à la modélisation si ces valeurs sont interdites à l'interface (validation applicative) ou simplement équivalentes à « visible du propriétaire uniquement ».
- **Instanciation dans un espace `PERSONAL` vs espace partagé** : `Document.Instantiate(spaceId, folderId)` crée une copie indépendante. Préciser si une instance dans un espace partagé peut pointer vers un document source dans un espace personnel d'un autre utilisateur — et si oui, quel est le comportement à la purge de l'espace source (ADR-011 §4 — `source_document_id` cross-espace).
- **Activation de l'espace personnel en mode local** : l'espace personnel comme zone d'atterrissage par défaut en mode local (sans `ownerId` assigné) est une recommandation — sa validation explicite est requise par l'opérateur au gate UC-01/UC-02.
- **Qualification juridique du contenu personnel au regard de l'Art. 17** : voir section « Conformité conçue, non certifiée ».

---

## Incohérences signalées — hors périmètre, pour la remédiation

Ces incohérences de corpus sont signalées pour la session de remédiation. **Elles ne sont pas corrigées dans cet ADR.**

- `ScenarioLibraryEntry` porte un champ nommé `ownerId` dans campaign-management.md l.174 mais `userId` dans le glossaire l.204 — incohérence de nommage à résoudre lors de la réécriture du domaine.
- L'attribution de la `ScenarioLibrary` à Campaign Management est à réexaminer : dans le nouveau paradigme, la bibliothèque personnelle est l'espace `PERSONAL` lui-même, dont la responsabilité relève davantage d'un nouveau contexte ou du domaine Content Library réécrit.

---

## Conformité conçue, non certifiée

Les choix documentés dans cet ADR sont cohérents avec le RGPD tel que lu et interprété lors de la conception. Deux points requièrent une **validation juridique avant tout lancement EU** :

1. **Qualification du contenu « personnel de premier ordre » au regard de l'Art. 17** : la thèse hard-delete inconditionnel à `UserDeleted` pour le contenu de l'espace personnel repose sur l'absence de tiers — donc l'absence d'intérêt légitime Art. 17§3(e). Cette qualification est solide en principe mais doit être confirmée. Points subsidiaires : (a) l'instant de référence (`deletion_requested_at`, cohérent avec ADR-012 §3(b)) s'applique-t-il au contenu personnel avec la même rigueur qu'au contenu partagé ? (b) si un document est déplacé d'un espace personnel vers un espace partagé après une demande d'effacement, la sélection est-elle figée à `deletion_requested_at` ou suivie dynamiquement ?

2. **Posture sous-traitant Art. 28 — périmètre du DPA** : ADR-013 §5 qualifie Haversack de sous-traitant pour les contenus créés par les MJ décrivant des tiers identifiables. Le DPA envisagé couvre les « contenus en campagne ». Un contenu personnel dans l'espace `PERSONAL` décrivant un tiers identifiable (PNJ inspiré d'une personne réelle, note personnelle sur un joueur) sort-il du périmètre DPA tel que formulé ? La posture sous-traitant doit être vérifiée pour les deux périmètres d'espace (partagé et personnel).

---

## Compléments post-revue

**Exécution du renommage (plan `20260612-reecriture-domaine-space`, vagues W1–W8 — 2026-06-12).** La décision (Voie 3) a été propagée sur l'ensemble du corpus : domaine (5 bounded contexts), glossaire, 14 use cases, user-stories, user-journeys, parcours, vision / moscow / personas / NFR, ADR RGPD-autorisation-migration (ADR-009/010/011/012/013/014/016/017) et diagrammes. Le fichier `campaign-management.md` (domaine + 4 diagrammes) est renommé `space-management.md`. L'incohérence `ScenarioLibraryEntry` `ownerId`/`userId` est résolue (alignée sur `ownerId`).

Décisions tranchées en cours de propagation (réversibles, à confirmer à l'entrée en build) :
- quota FREE = espaces `CAMPAIGN`/`ONE_SHOT` (l'espace `PERSONAL` par défaut **n'est pas décompté**) ;
- un espace `PERSONAL` est créé avec le **seul dossier virtuel « Non classés »** (les 4 dossiers système nommés restent réservés aux espaces partagés) ;
- purge **inconditionnelle** de l'espace `PERSONAL` à `UserDeleted`, **sous le même invariant de claim/reprise** que la purge J+30 (garantie Art.17, ADR-011).

> **Note de lecture** : les références de ce document à `campaign-management.md` et aux numéros de ligne reflètent l'état du corpus **au moment de la décision (2026-06-12)** ; elles ne sont pas réactualisées post-renommage. Se reporter aux fichiers `space-management.md` et au domaine courant pour l'état effectif. La session de zoning (`interface/**`, suspendue) réalignera ses propres références à sa reprise.

**Points de validation juridique toujours ouverts** (avant lancement EU) : qualification Art.17 du hard-delete inconditionnel du contenu personnel ; périmètre DPA Art.28 pour un contenu personnel décrivant des tiers. Non tranchés ici — flaggés pour un juriste.

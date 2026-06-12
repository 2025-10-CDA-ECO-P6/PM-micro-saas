# Parcours bout-en-bout — Antoine, MJ multi-groupe multi-système

> Objet : vérifier les coutures inter-UC sur le parcours propre à Antoine.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict).
> Aucun nom de technologie, aucun ADR cité comme source d'autorité.
> Renvoi : [README](README.md) · [Fiche persona](../persona/persona-05-antoine.md)

---

## Présentation du parcours

Antoine mène trois campagnes simultanées avec trois groupes différents. Chaque système — D&D 5e, Call of Cthulhu, Blades in the Dark — a sa propre logique de préparation, ses propres entités centrales, son propre vocabulaire documentaire. Aujourd'hui, il utilise trois outils différents. Sa douleur : la fragmentation.

Si Haversack lui dit qu'il est agnostique au système, il va tester ça immédiatement. La question directe qu'il pose à l'outil : les dossiers par défaut peuvent-ils être supprimés ou renommés ? Peut-on vraiment adapter la structure par campagne ?

Ce parcours couvre trois temps : la mise en place initiale de trois espaces distincts avec des structures adaptées à chaque système, la conduite de sessions multi-campagnes, et l'invitation de ses joueurs réguliers. Il ne couvre pas les scénarios réutilisables — UC-13 est explicitement hors première livraison (vision-produit §5bis, arbitrage du 2026-06-10) et ne fait pas partie du périmètre MVP.

Le parcours suppose qu'Antoine crée directement un compte dès le départ — il n'est pas dans un profil d'évaluation méfiant. Il a trois groupes de joueurs, un compte cloud est le prérequis naturel.

---

## Étape 1 — Création de compte et accès au tableau de bord

**UC porteur :** [UC-10](../usecases/UC-10-compte-cloud.md) | **UJ porteur :** [UJ-UC-10](../user-journeys/UJ-UC-10-compte-cloud.md)

### Ce qu'Antoine cherche à faire

Antoine découvre Haversack. Il n'a pas de données locales préexistantes — il s'inscrit directement.

### Comportements observables

- Antoine accède à la page d'inscription.
- Il renseigne email, nom d'affichage, mot de passe. Son compte est créé (tier `FREE`).
- Aucune donnée locale n'existe — le gate de reconnaissance n'est pas présenté (UC-10 scénario nominal « Inscription sans données locales »).
- Il est redirigé vers l'écran de création de campagne.
- Avec un compte `FREE` : 3 campagnes cloud maximum. Antoine a besoin de trois campagnes simultanées — il atteint immédiatement la limite du tier `FREE`. Pour trois campagnes actives en cloud, un compte `PRO` est nécessaire (glossaire §2, `AccountTier`).

### État laissé

- Antoine est `User` authentifié, `AccountTier = FREE` (limite à 3 campagnes cloud).
- Aucune `Campagne` n'existe encore.

### Couture vers l'étape 2

UC-02 précondition : « L'application est accessible (mode local ou compte cloud). » Satisfait — Antoine est authentifié.

**Couture continue.** Note : la limite de 3 campagnes `FREE` couvre exactement ses trois campagnes en cours. Si Antoine voulait une quatrième, il devrait passer en `PRO`. Ce point est un signal, pas une rupture — la limite est documentée et cohérente.

---

## Étape 2 — Création de trois espaces de campagne distincts

**UC porteur :** [UC-02](../usecases/UC-02-creer-espace-jeu.md) | **UJ porteur :** [UJ-UC-02](../user-journeys/UJ-UC-02-creer-espace-jeu.md) (si disponible)

### Ce qu'Antoine cherche à faire

Créer ses trois campagnes. Chacune sera configurée différemment à l'étape suivante — pour l'instant, il crée les conteneurs.

### Comportements observables

- Antoine accède à son tableau de bord.
- Il crée successivement trois campagnes en renseignant uniquement le nom (champ obligatoire) et le système de jeu (optionnel) :
  - « Les Profondeurs de Barovia » (D&D 5e)
  - « La Maison Dorchester » (Call of Cthulhu 7e)
  - « Les Fils de l'Anguille » (Blades in the Dark)
- Pour chaque campagne, quatre dossiers système sont créés automatiquement : Personnages, Joueurs, Scénarios, Notes.
- Une `SessionViewConfig` est créée automatiquement pour chaque campagne.

### État laissé

- Trois `Campagne` existent, toutes `ACTIVE`.
- Chaque campagne a ses quatre dossiers système et son dossier virtuel « Non classés ».
- Le compte `FREE` atteint sa limite de 3 campagnes cloud.

### Couture vers l'étape 3

UC-05 précondition : « Une campagne existe. Le MJ est propriétaire de la campagne. » Satisfait pour les trois campagnes.

**Couture continue.**

---

## Étape 3 — Adaptation de la structure documentaire par système

**UC porteur :** [UC-05](../usecases/UC-05-organiser-dossiers.md) | **UJ porteur :** [UJ-UC-05](../user-journeys/UJ-UC-05-organiser-contenu-dossiers.md)

### Ce qu'Antoine cherche à faire

C'est le test direct de l'agnosticisme système. Antoine vérifie que chaque campagne peut avoir une structure documentaire adaptée à sa logique propre. La première question de la fiche persona : les dossiers par défaut peuvent-ils être supprimés ou renommés ?

### Comportements observables — Campagne D&D 5e

Les dossiers système par défaut correspondent approximativement à la logique D&D (personnages, scénarios, notes). Antoine les adapte légèrement :
- Il renomme « Personnages » en « PNJ ».
- Il crée des dossiers « Lieux », « Factions », « Artefacts ».
- Il réordonne selon sa priorité.

### Comportements observables — Campagne Call of Cthulhu

L'organisation naturelle de CoC est par scénario puis par acte. La structure documentaire par défaut est trop générique :
- Antoine supprime « Joueurs » (ses joueurs ont des comptes — la gestion est dans UC-11).
- Il renomme « Scénarios » en « Arcs ».
- Il crée des dossiers « Suspects », « Indices », « Lieux du crime ».
- Les dossiers système supprimés : UC-05 E2/E3 — si le dossier contient des documents, le système propose de les déplacer ou de les laisser dans « Non classés ». Les dossiers système sont supprimables librement (`isSystem` est informatif, non restrictif — UC-05 règle métier).

### Comportements observables — Campagne Blades in the Dark

Les factions et les clocks sont les entités centrales de Blades. Les dossiers par défaut ne correspondent pas :
- Antoine supprime « Personnages » et « Scénarios ».
- Il renomme « Joueurs » en « Scoundrels ».
- Il crée des dossiers « Factions », « Turf », « Clocks », « Rumeurs ».
- Il renomme « Notes » en « Flashbacks ».

> Les types de document personnalisés (ex. : un type « Faction » avec des propriétés propres à Blades) relèvent de Could Have post-MVP — ils ne sont pas disponibles dans la première livraison. Antoine modélise ses factions comme des `Document` libres dans le dossier « Factions » (glossaire §4, `DocumentType` optionnel). La valeur existe ; elle est moins différenciante qu'avec les types personnalisés.

### État laissé

- Trois campagnes avec des structures de dossiers distincts et adaptés.
- Les dossiers renommés apparaissent avec leur nouveau nom dans la `SessionViewConfig` (UJ-UC-05 §Transitions inter-UC — Vers UC-06).

### Couture vers l'étape 4

UC-04 précondition : « Une campagne existe. Le MJ a accès à la campagne. » Satisfait pour les trois campagnes.

**Couture continue.**

---

## Étape 4 — Constitution du contenu de préparation

**UC porteurs :** [UC-04](../usecases/UC-04-gerer-documents-campagne.md), [UC-03](../usecases/UC-03-structurer-scenario.md) | **UJ porteurs :** [UJ-UC-04](../user-journeys/UJ-UC-04-gerer-documents-campagne.md), [UJ-UC-03](../user-journeys/UJ-UC-03-structurer-scenario.md) (si disponible)

### Ce qu'Antoine cherche à faire

Remplir ses campagnes avec son contenu. Il a déjà du contenu dans ses outils précédents — il le recrée ou le ressaisit ici. Il veut des fiches de PNJ, des scénarios, des notes.

### Comportements observables

- Antoine crée des `Document` dans chaque campagne, organisés dans leurs dossiers respectifs.
- Il peut créer un `Document` typé `NPC` pour ses PNJ, `SCENARIO` pour ses arcs narratifs, `NOTE` pour ses notes libres.
- Il peut lier des documents entre eux (`DocumentLink`) — ex. : un arc Call of Cthulhu référençant ses scènes, ses suspects et ses lieux.
- Chaque `Document` est créé avec `visibility = GM_ONLY` par défaut.
- Pour la campagne D&D, il peut structurer un scénario (UC-03) : `Document` de type `SCENARIO` liant des `Document` de type `SCENE`, chaque scène référençant des PNJ et des lieux.

### État laissé

- Trois campagnes avec un corpus de `Document` constitué.
- Des `DocumentLink` organisent la structure narrative de chaque campagne.
- Tout le contenu est `GM_ONLY` — rien n'est encore partagé aux joueurs.

### Couture vers l'étape 5

UC-06 précondition : « Une campagne existe et le MJ y a accès. Le MJ est authentifié ou en mode local sans compte. » Antoine est authentifié. Satisfait.

**Couture continue.**

---

## Étape 5 — Configuration de la vue session par campagne

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) | **UJ porteur :** [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md)

### Ce qu'Antoine cherche à faire

Antoine veut que sa vue session soit immédiatement opérationnelle à l'ouverture — les bons panneaux dans le bon ordre. Pour D&D, il veut ses PNJ et ses lieux en premier. Pour Blades, ses factions et ses clocks. Pour Call of Cthulhu, ses suspects et ses indices.

### Comportements observables

- Antoine configure la `SessionViewConfig` de chaque campagne depuis les paramètres de campagne — hors session, avant la première séance.
- Pour chaque campagne, il sélectionne les dossiers qui apparaissent dans les panneaux de la vue session et les ordonne.
- La configuration est mémorisée par campagne (UJ-UC-06 §Opportunités UX : « configuration de la vue session persistée entre deux sessions »).
- Les dossiers renommés à l'étape 3 apparaissent avec leur nouveau nom dans les panneaux.

### État laissé

- La `SessionViewConfig` de chaque campagne est configurée avec les `focusedFolders` voulus.
- Cette configuration est persistée et sera retrouvée à l'identique à chaque session.

### Couture vers l'étape 6

UC-06 précondition lancement de session : satisfait. La `SessionViewConfig` configurée garantit une ouverture sans friction (UJ-UC-06 §Scénario Thomas — profil le plus proche d'Antoine pour la préparation).

**Couture continue.**

---

## Étape 6 — Conduite d'une session multi-campagnes

**UC porteurs :** [UC-06](../usecases/UC-06-vue-session.md), [UC-07](../usecases/UC-07-creation-volee-session.md) | **UJ porteurs :** [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md), [UJ-UC-07](../user-journeys/UJ-UC-07-creation-volee-session.md)

### Ce qu'Antoine cherche à faire

Antoine mène 2 à 3 sessions par semaine. Il passe d'une campagne à l'autre. Pour chaque session, il veut un accès rapide à sa campagne et une vue session immédiatement opérationnelle.

### Comportements observables

- Antoine accède à son tableau de bord. Ses trois campagnes sont visibles — le tableau de bord est multi-campagnes (UC-02 postcondition : après création, le MJ retrouve l'ensemble des espaces de jeu dont il est propriétaire, jusqu'à la limite de son AccountTier, et peut passer de l'un à l'autre).
- Il ouvre la campagne du soir, lance une session.
- La vue session s'ouvre avec ses panneaux configurés — les dossiers Factions, Clocks et Scoundrels pour Blades in the Dark, par exemple.
- Il consulte ses fiches, épingle des documents dans `pinnedDocumentIds`.
- Si une faction inattendue émerge en session, il la crée à la volée (UC-07) : panneau de création rapide, titre seul, type `NOTE` ou document libre dans le dossier « Factions ». Le document est automatiquement épinglé.
- Un document créé à la volée est placé dans le dossier d'accueil résolu selon le type (UJ-UC-07 §Points de conversion : « Document placé dans le dossier correct dès la création »).

### État laissé

- Une `Session` par campagne, cycle de vie indépendant.
- Plusieurs `Document` créés à la volée, liés à leur campagne respective.
- Des `LIVE_NOTE` créées pendant les sessions.

### Couture vers l'étape 7

UC-11 précondition : « Une campagne existe. Le MJ est propriétaire de la campagne. L'adhésion d'un membre permanent présuppose que le joueur dispose d'un compte ou en crée un au fil du parcours d'invitation (UC-10). » Satisfait — Antoine est propriétaire de ses trois campagnes.

**Couture continue.**

---

## Étape 7 — Invitation des joueurs réguliers et gestion des membres

**UC porteur :** [UC-11](../usecases/UC-11-gerer-membres-campagne.md) | **UJ porteur :** [UJ-UC-11](../user-journeys/UJ-UC-11-gerer-membres-campagne.md)

### Ce qu'Antoine cherche à faire

Antoine a des groupes stables pour chaque campagne. Il veut inviter ses joueurs réguliers comme membres permanents — ils accèdent à l'historique de la campagne entre les sessions et trouvent leurs personnages directement.

### Comportements observables

- Pour chaque campagne, Antoine accède à la section « Membres ».
- Il génère un lien d'invitation de périmètre `CAMPAIGN` (accès durable).
- Il peut configurer une date d'expiration ou un nombre maximum d'utilisations.
- Il partage le lien à ses joueurs.
- Quand un joueur utilise le lien avec un compte existant, un `SpaceMembership` est créé en statut `ACTIVE` (UC-09 — octroi d'accès, scénario nominal côté joueur ; UC-11 — validation côté MJ). La vue cohérente que le joueur obtient ensuite relève d'UC-12 (vue joueur).
- Quand un joueur sans compte utilise le lien, il crée son compte dans le même parcours (UC-10 A3, UC-09 A2) — son `GuestAccess` est converti en `SpaceMembership`.
- Antoine associe chaque joueur membre à son personnage (`SpaceMembership` + personnage associé, UC-11 A1).
- Il peut révoquer une invitation active (UC-11 A2) ou retirer un membre (UC-11 A3) sans perdre ses données.

### État laissé

- Des `SpaceMembership` actifs existent dans chaque campagne, associés à des personnages.
- Les joueurs ont accès aux contenus `PUBLIC` de leur campagne et à leur propre fiche de personnage.

### Couture vers l'étape 8

UC-08 précondition : « Le MJ est authentifié et propriétaire de la campagne. Des joueurs ou des `GuestAccess` peuvent accéder à la campagne. » Satisfait — des membres existent dans chaque campagne.

**Couture continue.**

---

## Étape 8 — Session avec joueurs : partage d'informations en direct

**UC porteurs :** [UC-06](../usecases/UC-06-vue-session.md), [UC-08](../usecases/UC-08-partager-information.md), [UC-09](../usecases/UC-09-acces-session-joueur.md)

### Ce qu'Antoine cherche à faire

Pendant la session, Antoine veut partager des informations aux joueurs au bon moment — une carte, un document de lore, une révélation. Il contrôle ce qui est visible.

### Comportements observables

- Antoine lance une session depuis sa campagne. Les joueurs membres y ont accès via leur compte.
- Pour les joueurs occasionnels ou les sessions ponctuelles, Antoine génère un lien de session ponctuel (`GuestAccess`, portée `SESSION`) depuis la vue session (UC-06 règle métier « Lien de session ponctuel »).
- Depuis la vue session, il ouvre un document et clique « Partager ». La `visibility` passe de `GM_ONLY` à `PUBLIC` — changement durable au-delà de la session (UC-08).
- Les joueurs voient immédiatement le document dans leur vue session joueur.
- Antoine peut retirer le partage à tout moment (`Unshare()`).
- Limite compte `FREE` : 4 joueurs par session. Si Antoine a plus de 4 joueurs pour une session, un compte `PRO` est nécessaire.

### État laissé

- Des `Document` avec `visibility = PUBLIC` existent dans les campagnes.
- Les joueurs membres ont accès à l'historique des sessions et au lore partagé entre les sessions.

---

## Points de friction propres à Antoine

Ces points sont fondés sur le corpus (fiche persona, UJ-UC-05, UJ-UC-07) — aucun n'est inventé.

1. **Dashboard multi-campagnes** : la fiche persona pose directement la question « Y a-t-il un dashboard multi-campagnes ou faut-il naviguer campagne par campagne ? » Le comportement du tableau de bord multi-campagnes n'est pas spécifié au niveau de détail d'un UC dédié. UC-02 couvre la création de l'espace et la redirection vers son tableau de bord, mais la navigation entre plusieurs campagnes simultanées relève de l'interface générale — zone peu documentée dans les UC existants.

2. **Types personnalisés absents du MVP** : Antoine veut créer un type « Faction » avec les propriétés propres à Blades in the Dark. Les types personnalisés sont Could Have post-MVP (moscow.md §Could Have). En première livraison, il modélise ses factions comme des documents libres — valeur réelle, mais moins différenciante.

3. **Dossiers par défaut imposés au premier regard** : la fiche persona indique qu'Antoine va tester l'agnosticisme système immédiatement. Si les dossiers système sont renommables et supprimables (ce qu'ils sont — UC-05 règle métier), la friction disparaît. Si ce n'était pas le cas, Antoine abandonnerait.

4. **Associer un template à un dossier sans UC-13** : UJ-UC-05 §Transitions inter-UC (Vers UC-04) précise que la disponibilité d'un modèle par défaut pour un dossier dépend de UC-13 (Should Have — post-MVP). En l'absence de UC-13, la liste des modèles disponibles est vide. Antoine ne peut pas associer de template à ses dossiers dans la première livraison — friction documentée dans UJ-UC-05 §Points de friction.

5. **Limite de 4 joueurs par session (FREE)** : Antoine a potentiellement plus de 4 joueurs dans un groupe. La limite FREE est atteinte pour ses groupes les plus larges — déclencheur naturel de l'upgrade vers PRO.

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Inscription → création de campagnes | 1 → 2 | **Continue** | UC-02 précondition = application accessible. Antoine est authentifié. Satisfait. |
| C2 | Campagnes créées → adaptation des dossiers | 2 → 3 | **Continue** | UC-05 précondition = campagne existante + MJ propriétaire. Satisfait pour les trois campagnes. |
| C3 | Dossiers adaptés → création du contenu | 3 → 4 | **Continue** | UC-04 précondition = campagne existante + MJ y ayant accès. Satisfait. Les dossiers renommés sont immédiatement utilisables. |
| C4 | Contenu créé → configuration vue session | 4 → 5 | **Continue** | UC-06 (SessionViewConfig) précondition = campagne existante. Satisfait. Les dossiers renommés apparaissent avec leur nouveau nom (UJ-UC-05 §Vers UC-06). |
| C5 | Vue session configurée → conduite de session | 5 → 6 | **Continue** | UC-06 lancement : campagne existante + MJ authentifié. La SessionViewConfig configurée est retrouvée à l'identique. |
| C6 | Conduite de session → invitation membres | 6 → 7 | **Continue** | UC-11 précondition = campagne existante + MJ propriétaire + joueurs disposant d'un compte (ou en créant un via UC-09/UC-10). Satisfait. L'octroi (`SpaceMembership`) relève d'UC-09 (joueur) et UC-11 (MJ) ; la vue joueur post-accès relève d'UC-12. |
| C7 | Membres invités → partage en session | 7 → 8 | **Continue** | UC-08 précondition = MJ authentifié + joueurs/`GuestAccess` accessibles. `SpaceMembership` actifs existent. Satisfait. |
| C8 | Templates de dossier indisponibles (UC-13 hors MVP) | 3 | **Limite assumée** | UJ-UC-05 §Transitions inter-UC trace explicitement la dépendance : la liste des modèles pour un dossier dépend de UC-13 (Should Have — post-MVP, hors première livraison) ; en son absence, l'association de modèle n'est pas opérante et un message explicatif est prévu si aucun modèle n'est disponible (UJ-UC-05, points de vigilance). Localisé : UJ-UC-05 §Transitions inter-UC — Vers UC-04, et UC-05 scénario nominal « Associer un template à un dossier ». Antoine ne peut pas associer de template à ses dossiers Factions, Suspects ou Clocks dans la première livraison — limite de périmètre tracée, pas un trou de spécification. |
| C9 | Dashboard multi-campagnes | 2, 6 | **Continue (couture)** | UC-02 postcondition : après création d'une campagne, depuis le tableau de bord, le MJ retrouve l'ensemble des espaces de jeu dont il est propriétaire ou membre, jusqu'à la limite de son AccountTier. Étape 2 : création des trois campagnes. Étape 6 : Antoine accède à son tableau de bord et retrouve ses trois campagnes, puis navigue entre elles pour conduire ses sessions. La navigation multi-espaces est continue — pas de rupture de parcours. |

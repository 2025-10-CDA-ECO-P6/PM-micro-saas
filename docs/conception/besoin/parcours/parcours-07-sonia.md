# Parcours bout-en-bout — Sonia, la MJ one-shot (hors cible MVP)

> Objet : vérifier la frontière du périmètre produit sur le parcours de Sonia — ce que le MVP
> peut lui offrir sous la forme d'une campagne à session unique, la friction que cela représente
> pour sa pratique réelle, et la limite tracée avec sa condition de retour exacte.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict).
> Index des parcours → [README](README.md)
> Fiche persona → [../persona/persona-07-sonia.md](../persona/persona-07-sonia.md)

---

## Présentation

Sonia, 31 ans, animatrice en centre de loisirs, mène 3 à 5 parties par mois — exclusivement des
one-shots et courts métrages. Alien RPG, Call of Cthulhu, Ironsworn. Elle dispose d'une quinzaine
de scénarios dans son catalogue, qu'elle rejoue et adapte selon le groupe et l'occasion. Pas de
continuité narrative, pas de groupe stable, pas d'historique commun.

Sa logique de travail est celle d'une script : des indices, des atmosphères, des PNJ qui
n'apparaissent qu'une seule fois. Ce qui lui importe dans un outil : retrouver ses scénarios,
les adapter rapidement, partager des documents avec des joueurs qu'elle ne reverra peut-être
jamais.

Ce parcours est documenté bien que Sonia soit hors cible MVP. Sa valeur est précisément de
mesurer la friction que le périmètre actuel représente pour elle, et de localiser la limite tracée
avec sa condition de retour telle que le corpus l'énonce. La frontière du périmètre se vérifie ici.

---

## Étape 1 — Ouvrir l'application et créer un espace de jeu

**UC porteurs :** UC-01, UC-02 | **UJ porteurs :** UJ-UC-01, UJ-UC-02

### Ce que Sonia fait

Sonia ouvre l'application. Elle voit l'écran d'accueil avec deux options. Elle choisit
« Commencer sans compte » — sa pratique en convention ou dans son association locale implique
des joueurs qui changent à chaque fois ; elle ne veut pas les forcer à s'inscrire. Elle accède
à l'écran de création de campagne.

### Le premier point de friction

La fiche persona le formule sans détour : « Si elle ouvre Haversack, elle verra "créer une
campagne" et aura l'impression que l'outil ne parle pas d'elle. Créer une campagne vide pour
chaque one-shot serait absurde de son point de vue. »

La vision produit §2.2 reconnaît ce point explicitement : « Un one-shot reste possible en MVP
sous la forme d'une campagne à session unique, sans le parcours express ni catalogue réutilisable »
(section « Contexte one-shot — arbitrage du 2026-06-10 »). Le corpus acte donc que Sonia peut
techniquement créer un espace de jeu — mais dans un conteneur nommé et pensé pour la campagne,
pas pour son flux de travail.

### Ce que Sonia obtient à la création (comportements observables)

- Un espace de jeu créé avec quatre dossiers système par défaut : « Personnages », « Joueurs »,
  « Scénarios », « Notes » (UC-02, glossaire §3 `Folder`). Elle peut les renommer ou les
  supprimer librement.
- Le `SpaceType` est `CAMPAIGN` par défaut — le glossaire définit `ONE_SHOT` comme valeur
  possible de `SpaceType`, mais son point d'entrée dédié dans l'interface (« deux points
  d'entrée distincts » — UC-02 §Contexte) fait partie du périmètre post-MVP.
- Aucune donnée envoyée au serveur. Fonctionnalités de partage désactivées en mode local.

### État laissé par l'étape 1

- Une `Campagne` existe en mode local avec une `SessionViewConfig` créée automatiquement
  à `CampaignCreated` (glossaire §5 `SessionViewConfig`).
- Sonia est en mode local, sans `User`.

### Couture vers l'étape 2

UC-04 et UC-05 préconditions : campagne existante, MJ en mode local ou authentifié. UC-02
confirme l'identité des préconditions dans les deux modes. **Couture continue.**

---

## Étape 2 — Préparer le scénario du one-shot

**UC porteurs :** UC-03, UC-04, UC-05 | **UJ porteurs :** UJ-UC-03, UJ-UC-04, UJ-UC-05

### Ce que Sonia fait

Sonia crée les documents de son scénario — indices, PNJ, atmosphères, une carte ou une lettre
in-game. Le modèle de blocs libres lui permet de saisir ce contenu sans structure imposée. Elle
peut organiser ses documents dans les dossiers de l'espace de jeu, les renommer à sa façon,
créer des liens entre eux (`DocumentLink`).

### Comportements observables

- `Document` créés avec `visibility = GM_ONLY` par défaut.
- Organisation libre par dossiers renommables (UC-05 base, Must Have).
- Liens entre documents navigables (glossaire §4 `DocumentLink`).
- Contenu persisté dans le navigateur.

### La friction spécifique de Sonia à cette étape

Sonia anime la même partie avec des groupes différents. Son flux de travail réel implique de
réutiliser un scénario existant : l'adapter, le relancer, sans dupliquer tout le contenu. UC-13
(Utiliser un scénario réutilisable) couvre ce besoin — `Document` avec `isReusable = true`,
instanciation indépendante du source (glossaire §4 `Document réutilisable` et `Instance`).

UC-13 est explicitement hors première livraison. Dans le MVP, Sonia doit manuellement recréer
ou dupliquer son scénario à chaque nouvelle table — ou maintenir une campagne de référence et
copier le contenu à la main. Aucun mécanisme d'instanciation de document réutilisable n'est
disponible (UC-13 — scénario réutilisable, Should Have post-MVP ; voir glossaire `ScenarioLibrary`).

### État laissé par l'étape 2

- La `Campagne` contient les `Document` et `Folder` du scénario.
- Sonia est toujours en mode local.

### Couture vers l'étape 3

UC-06 précondition : campagne existante + MJ authentifié ou en mode local. **Couture continue.**

---

## Étape 3 — Lancer la session et utiliser la vue session

**UC porteur :** UC-06 | **UJ porteur :** UJ-UC-06

### Ce que Sonia fait

Elle clique « Lancer une session ». La `Session` est créée directement en `LIVE`. La vue session
MJ s'ouvre. Elle navigue dans ses documents, épingle ce dont elle a besoin (`pinnedDocumentIds`),
prend des notes de session (`LIVE_NOTE` avec `visibility = GM_ONLY` par défaut).

En fin de partie, elle termine la session (`LIVE → CLOSED`). Elle peut ajouter un résumé
(`summary`).

### Comportements observables

- Vue session MJ disponible en mode local — tableau de bord configurable, panneaux dossiers,
  notes de session privées MJ, épingles, barre de recherche (UC-14 Should Have).
- Cycle de vie `Session` : `LIVE → CLOSED → ARCHIVED`.
- En mode local : aucun joueur ne peut accéder à la vue joueur, aucun `GuestAccess` ne peut
  être créé, aucune note de session de joueur ne peut exister (UC-01 règles métier, UC-06
  précondition).

### La friction « 30 secondes chrono »

La vision produit §2.2 décrit le parcours express one-shot comme : « lancer en moins de
30 secondes depuis une bibliothèque de scénarios réutilisables, sans configuration de campagne ».
Ce parcours est hors MVP.

Dans le MVP, Sonia doit créer un espace de jeu, y structurer son contenu, puis lancer une
session — un flux conçu pour un MJ de campagne qui configure une fois et utilise sur la durée.
Pour Sonia qui répète ce cycle 3 à 5 fois par mois avec des groupes différents, chaque one-shot
rouvre le problème de configuration. La vision produit identifie cette friction et l'énonce
comme raison du report : le parcours express dépend du catalogue de scénarios réutilisables
(UC-13), une capacité entière qui ne conditionne pas la validation du cœur produit.

### État laissé par l'étape 3

- Une `Session` en `CLOSED` ou `ARCHIVED` existe dans la campagne.
- Des `LIVE_NOTE` avec `visibility = GM_ONLY` ont pu être créées.
- Sonia est toujours en mode local.

### Couture vers l'étape 4

UC-08 précondition : « Des joueurs ou des accès invités peuvent accéder à la campagne ou à la
session. » Cette précondition n'est pas satisfaite en mode local. Sonia doit créer un compte
pour partager des informations avec ses joueurs. **Couture conditionnelle — UC-10 s'intercale.**

---

## Étape 4 — Créer un compte pour pouvoir partager avec les joueurs

**UC porteur :** UC-10 | **UJ porteur :** UJ-UC-10

### Ce que Sonia fait

Sonia tente de partager une lettre ou une carte in-game avec ses joueurs. Le bouton est visible
mais désactivé (UC-01 RB-01-06). Le CTA lui propose de créer un compte. Elle saisit email, nom
d'affichage, mot de passe. Le compte est créé en tier `FREE`.

Le gate de reconnaissance lui présente les données locales détectées (titre de la campagne,
volume, date). Elle confirme. La campagne est importée dans le cloud.

### Comportements observables

- Compte `User` créé (`AccountStatus = ACTIVE`, `AccountTier = FREE`).
- Campagne importée en cloud, accessible sur plusieurs appareils.
- Fonctionnalités de partage activées.
- Maximum 3 campagnes en cloud, 4 joueurs par session (`AccountTier = FREE`).

### La friction du modèle de membres pour Sonia

La fiche persona pose la question directement : « Comment gérer des joueurs qui changent à
chaque session ? Le modèle de membres suppose une liste stable. »

Dans le MVP, ce problème est résolu par l'accès joueur sans compte (UC-09) : le joueur reçoit
un lien de session, saisit un nom d'affichage, accède. Le `GuestAccess` de portée `SESSION`
expire à la fermeture de la session + 24 heures. Sonia n'a pas à gérer une liste de membres.
**Ce point est couvert par le MVP.**

En revanche, la limite est sur le volume : compte `FREE` = 4 joueurs par session (glossaire §2
`AccountTier`). Si Sonia anime une table de convention à 6 joueurs, elle dépasse ce seuil.
Le compte `PRO` est le prérequis.

### État laissé par l'étape 4

- Sonia a un compte actif.
- La campagne est synchronisée en cloud.

### Couture vers l'étape 5

UC-08 précondition maintenant satisfaite : campagne existante + joueurs ou accès invités
accessibles. **Couture continue.**

---

## Étape 5 — Partager des informations et accès joueur sans compte

**UC porteurs :** UC-08, UC-09 | **UJ porteurs :** UJ-UC-08, UJ-UC-09

### Ce que Sonia fait

Depuis la vue session, Sonia ouvre la lettre ou la carte in-game et clique « Partager ». Le
document passe de `visibility = GM_ONLY` à `visibility = PUBLIC` (UC-08 scénario nominal).
Elle génère un lien de session depuis la vue session (UC-09 précondition, UC-06 règle métier).
Elle partage le lien via n'importe quel canal.

Chaque joueur clique sur le lien, saisit un nom d'affichage (`displayName`), accède à la vue
session joueur — documents `PUBLIC` visibles en temps réel.

À la fermeture de session, les `GuestAccess` de portée `SESSION` expirent après 24 heures
(glossaire §3 `GuestAccess`). Sonia peut relancer le cycle au one-shot suivant.

### Comportements observables

- Partage de documents par le MJ : granularité document par document, tout-ou-tous les joueurs
  autorisés (pas de partage sélectif par joueur — arbitrage du 2026-06-10, vision §5bis).
- Accès joueur sans compte : lien + nom d'affichage, pas d'inscription, pas d'email.
- Le `GuestAccess` expire — pas de liste de membres à maintenir entre les parties.
- Limite `FREE` : 4 joueurs par session.

---

## L'arrêt — Ce que Sonia ne peut pas faire dans le MVP

Sonia peut animer un one-shot dans le MVP sous la forme d'une campagne à session unique. Le
corpus l'acte explicitement (vision §2.2 : « Un one-shot reste possible en MVP sous la forme
d'une campagne à session unique »).

Ce qu'elle ne peut pas faire :

1. **Lancer en moins de 30 secondes depuis son catalogue.** Elle doit créer ou reconfigurer
   un espace de jeu à chaque partie. Le parcours express (vision §2.2, §4.1 `Parcours principaux`)
   est hors première livraison.

2. **Réutiliser un scénario de son catalogue sans ressaisie.** UC-13 (Utiliser un scénario
   réutilisable) est explicitement hors première livraison (moscow.md §UC-13 §Arbitrage du
   2026-06-10). Sans instanciation, Sonia duplique manuellement.

3. **Créer un scénario directement dans une bibliothèque sans passer par une campagne.**
   UC-F07 (Créer un scénario directement dans la bibliothèque) est Could Have non MVP
   (UC-HORS-MVP §UC-F07).

---

## Limite tracée et condition de retour

### Limite tracée

**Sonia est structurellement non servie par la première livraison.** La moscow.md §UC-13
l'énonce sans ambiguïté : « Sonia (one-shots exclusivement, 15 scénarios en catalogue) reste
structurellement non servie par la première livraison. »

La vision produit §2.2 §Contexte one-shot précise : « Le one-shot complet — parcours express,
scénarios réutilisables, UC-13 — est explicitement hors première livraison. L'ambition "deux
contextes de premier ordre" reste l'horizon produit. »

### Condition de retour exacte (texte du corpus)

La condition de retour est énoncée dans deux artefacts :

**Vision produit §5bis** (section « Contexte one-shot — arbitrage du 2026-06-10 ») :
> « Condition de retour : si les entretiens ou l'usage révèlent que le profil one-shot exclusif
> (Sonia — conventions, groupes changeants, catalogue de scénarios) est une part significative
> des utilisateurs réels ou un levier d'adoption, UC-13 est réexaminé en priorité lors d'une
> version ultérieure. »

**Moscow.md §UC-13** (arbitrage du 2026-06-10) :
> « Si les entretiens ou l'usage révèlent que le profil one-shot est une part significative des
> utilisateurs réels ou un levier d'adoption, UC-13 est réexaminé en priorité lors d'une version
> ultérieure. »

Les deux sources convergent. La condition de retour est un signal d'usage ou d'entretien,
pas un événement calendaire — c'est une observation à faire sur la cohorte pilote réelle.

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Démarrage sans compte → création de l'espace de jeu | Entrée → étape 1 | Continue | UC-01 et UC-02 : préconditions identiques en mode local et mode cloud. Friction perceptuelle documentée dans la fiche persona (« créer une campagne » ne parle pas d'elle). |
| C2 | Espace de jeu → préparation du scénario | Étape 1 → étape 2 | Continue | UC-03, UC-04, UC-05 base : préconditions satisfaites dès qu'une campagne existe en mode local. |
| C3 | Scénario préparé → lancement de session | Étape 2 → étape 3 | Continue | UC-06 précondition : campagne existante + MJ authentifié ou en mode local. Explicitement couvert. |
| C4 | Session locale → partage avec les joueurs | Étape 3 → étape 4 | Continue (documentée) | UC-01 règles métier : partage désactivé en mode local. UC-10 s'intercale. La transition est documentée dans UJ-UC-01 (flux fonctionnel, point de conversion « tentative de partage »). |
| C5 | Compte actif → partage et accès joueur sans compte | Étape 4 → étape 5 | Continue | UC-08 + UC-09 préconditions satisfaites dès qu'un compte `FREE` existe. Le `GuestAccess` de portée `SESSION` couvre les joueurs changeants sans liste de membres. |
| C6 | One-shot suivant → réutilisation du scénario | Post-étape 5 | Limite assumée | UC-13 hors première livraison (moscow.md §UC-13, vision §5bis). Sonia doit recréer ou dupliquer manuellement. C'est le noyau de sa non-servitude structurelle. Condition de retour tracée dans la vision §5bis et la moscow.md. |
| C7 | Création d'un scénario directement sans campagne | Hors périmètre MVP | Limite assumée | UC-F07 Could Have non MVP (UC-HORS-MVP §UC-F07). Contournement documenté dans UC-HORS-MVP : « campagne "Atelier scénarios" comme conteneur de travail en attendant ». |
| C8 | Table de convention > 4 joueurs | Étape 5 | Zone muette | La limite `FREE` à 4 joueurs par session est tracée (glossaire §2 `AccountTier`, UC-09 règles métier). Aucun UC ni UJ ne documente le parcours de Sonia face à cette limite (passage au compte `PRO` ou refus d'accès au 5e joueur). |

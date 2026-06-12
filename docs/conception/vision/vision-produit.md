# Vision produit — Haversack

> Document de référence pour le positionnement produit, les choix assumés
> et la définition des utilisateurs cibles.
>
> Use cases détaillés → [usecases/README.md](../usecases/README.md)
> Priorisation MoSCoW → [moscow.md](moscow.md)

---

## 1. Problème utilisateur

### 1.1 Le contexte actuel du Maître du Jeu

Un Maître du Jeu (MJ) actif gère simultanément une quantité d'information considérable :
scénarios préparés, historique des sessions passées, fiches de personnages non-joueurs,
notes secrètes, informations partagées avec les joueurs, suivi des conséquences narratives,
lore, lieux, factions, objets importants.

Aujourd'hui, cette information est typiquement dispersée sur plusieurs supports en parallèle :

| Support | Usage courant | Problème principal |
|---|---|---|
| Fichiers texte / Word | Scénarios, notes longues | Difficile à parcourir en session |
| Notion / Google Docs | Organisation de campagne | Pas conçu pour le JDR |
| Discord | Communication joueurs, partage d'infos | Informations perdues dans le scroll |
| Papier | Notes rapides, fiches PNJ | Non cherchable, risque de perte |
| PDF | Règles, modules achetés | Lecture seule, non intégrable |
| Roll20 / Foundry | Table virtuelle | Centré sur le combat, pas la narration |

Cette fragmentation a des conséquences directes pendant la partie :
- le MJ perd du temps à chercher une information pendant la session ;
- le rythme de jeu est cassé ;
- certaines informations sont oubliées ou introuvables ;
- la charge mentale de préparation est élevée.

### 1.2 Le besoin non adressé

Les outils existants tombent dans deux catégories :

**Outils génériques** (Notion, Obsidian, Google Docs) : flexibles mais sans logique JDR.
Le MJ doit construire sa propre organisation from scratch, et elle n'est pas pensée pour
un accès rapide pendant la session.

**Outils de table virtuelle** (Roll20, Foundry VTT) : riches mais centrés sur la mécanique
de jeu (combat, cartes, jets de dés). La narration, la préparation et la gestion documentaire
sont des fonctions secondaires.

**Le créneau de Haversack** : l'outil d'assistance au MJ centré sur la préparation narrative
et le pilotage de session. Pas une table virtuelle. Pas un éditeur de texte générique.
Un outil pensé pour les problèmes du MJ.

---

## 2. Positionnement du MVP

### 2.1 Objectif principal

> Permettre au MJ de préparer une campagne, structurer ses scénarios, centraliser
> ses notes et accéder rapidement aux informations importantes pendant une session —
> sans friction d'onboarding, sans compte obligatoire pour commencer.

Le MVP valide deux hypothèses en parallèle :

1. **Hypothèse produit** : un MJ paiera pour un outil qui réduit la friction de préparation
   et de pilotage, même sans fonctionnalités avancées (IA, table virtuelle, gestion de règles).
2. **Hypothèse de monétisation** : le modèle local-gratuit-Pro permet une adoption sans résistance
   initiale et une conversion naturelle vers le cloud lorsque le besoin de partage apparaît.

### 2.2 Choix produit assumés

**Le MJ est l'utilisateur principal.** Ce choix est délibéré et non négociable pour le MVP.

Justification :
- Le MJ porte l'intégralité de la charge de préparation.
- Le MJ choisit les outils utilisés par le groupe — il est le décideur d'achat.
- Le MJ a la douleur utilisateur la plus forte et la plus documentable.
- Une adoption réussie côté MJ entraîne mécaniquement l'adoption côté joueurs.

**Aucun compte obligatoire pour commencer.** Un MJ peut ouvrir l'application et créer sa
première campagne sans s'inscrire. Les données sont stockées localement dans le navigateur.
Le compte n'arrive que lorsque le MJ veut partager avec ses joueurs ou sauvegarder dans le cloud.
Cette décision réduit la friction d'onboarding pour les profils les plus résistants (Nadia, Rémi)
et structure le modèle de monétisation — voir section 3.

**Les joueurs accèdent sans compte.** Un joueur clique sur un lien de session, saisit un nom
d'affichage, et accède immédiatement aux informations partagées. Aucune inscription n'est
demandée. Un compte joueur reste possible pour conserver un accès persistant entre sessions.

**Pas de moteur de règles.** Haversack est agnostique au système de jeu. Il ne gère
pas les jets de dés, les combats ou les mécaniques. Cette décision réduit la complexité
et élargit la cible (D&D, Call of Cthulhu, Fate, Blades in the Dark, systèmes narratifs…).

**Le contenu appartient à l'espace — la campagne est un type d'espace, pas un prérequis.**
Le MJ peut créer et organiser du contenu (PNJ, lieu, scénario, règle maison, objet) sans
qu'une campagne soit en cours ou même planifiée. Un **espace personnel** est disponible
par défaut, y compris dès le mode local, pour accueillir ce contenu.
La campagne est une façon d'organiser et d'utiliser une partie de ce contenu — pas sa condition
d'existence. Ce choix est cohérent avec « Système de document générique » (tout est Document)
et avec le différenciant « friction d'entrée nulle » : le MJ capture une idée immédiatement,
sans créer de structure d'abord. *(ADR-018 — Voie 3 Space+PERSONAL, acté 2026-06-12.)*

**Deux contextes de jeu de premier ordre — horizon produit.** Haversack reconnaît deux modes d'utilisation
distincts qui ne partagent pas les mêmes besoins :

- **Campagne** : plusieurs sessions, groupe stable, continuité narrative.
  Configuration initiale, gestion des membres, historique des sessions.
  **Livré en MVP.**

- **One-shot** : une seule session, joueurs potentiellement différents, aucune continuité.
  Parcours express (lancer en moins de 30 secondes depuis une bibliothèque de scénarios réutilisables, sans configuration de campagne)
  — **hors première livraison**, trace d'arbitrage en section 5bis.
  Un one-shot reste possible en MVP sous la forme d'une campagne à session unique (sans le parcours express).
  Le contexte complet (parcours express + UC-13) dépend des scénarios réutilisables et arrive après validation du cœur produit.

Le one-shot n'est pas une campagne dégradée. C'est un contexte de jeu à part entière,
qui disposera de son propre point d'entrée dans l'application. Techniquement, les deux reposent sur
le même modèle de données — ce détail est invisible pour l'utilisateur.

**Système de document générique.** Plutôt que des formulaires figés par type de contenu,
Haversack utilise un modèle de blocs libres. Un document peut représenter n'importe quoi :
un PNJ, un lieu, une faction, une règle maison, un objet important. Le MJ structure son contenu
comme il le souhaite. Les types de document (PNJ, Lieu, Objet…) sont une couche optionnelle
qui ajoute des propriétés structurées sans retirer la liberté d'édition.

**Organisation libre par dossiers.** Le MJ organise son contenu dans des dossiers qu'il
crée et nomme lui-même. Quatre dossiers sont générés automatiquement à la création
de la campagne ("Personnages", "Joueurs", "Scénarios", "Notes") — point de départ neutre,
pas une cage. Le MJ peut renommer, réorganiser ou supprimer ces dossiers librement.
Un MJ de Blades in the Dark peut renommer "Personnages" en "Factions", changer le type
de document par défaut, ou repartir d'une ardoise vide.

**Vue session configurable.** Le tableau de bord de session n'impose aucune structure fixe. Le MJ choisit quels dossiers il met en avant dans sa vue — certains veulent leurs PNJ au premier plan, d'autres leurs lieux ou leurs scènes. Cette configuration est mémorisée par campagne. La cohérence avec le système de dossiers libres est totale : ce que le MJ organise dans sa bibliothèque, il peut l'exposer directement dans sa vue session.

**Relations entre documents.** N'importe quel document peut référencer un autre.
Les backlinks (documents pointant vers un document donné) sont consultables depuis la fiche cible.
La campagne devient un réseau d'informations navigable. Les relations typées entre documents
(ce PNJ appartient à cette faction, cet objet est porté par ce personnage) sont une évolution
post-MVP qui s'appuie sur les types de document — voir [UC-HORS-MVP](../usecases/UC-HORS-MVP.md).

### 2.3 Ce que le MVP doit démontrer

Pour valider le concept et justifier une suite, le MVP doit démontrer cinq hypothèses auprès des utilisateurs réels. Chaque hypothèse reçoit un seuil chiffré et un délai d'observation — ces seuils sont calibrés pour une cohorte pilote restreinte (early adopters recrutés), révisables avant le lancement de l'observation, et constituent un gate GO/NO-GO objectif à l'issue de la période.

**Important** : un seuil non atteint impose un constat explicite (réussite partielle, échec du pilier, ou hypothèse invalidée) — jamais une réinterprétation. Chaque hypothèse reste falsifiable : si le seuil n'est pas atteint dans le délai, l'hypothèse n'est pas démontrée.

**Les instruments de constat** sont exclusivement ceux déjà inscrits au périmètre Must Have (section « Instrumentation de validation du MVP » de `moscow.md`) : activation préparation, activation vue session, activation partage, mesure d'usage anonyme en mode local, et entretiens utilisateurs.

| # | Hypothèse | Seuil chiffré | Délai | Instrument de constat |
|---|---|---|---|---|
| **H1** | Un MJ peut créer un espace structuré et y retrouver ses informations sans friction d'onboarding (pas de compte obligatoire au démarrage). | ≥ 60 % des MJ de la cohorte pilote qui ouvrent l'application atteignent **activation préparation** (espace actif — campagne ou espace personnel — avec premiers documents créés). *(instrument redéfini ADR-018 : l'espace personnel compte au même titre qu'une campagne créée — décision 2026-06-12.)* | 14 jours après le premier usage. | Activation préparation (mesure d'usage anonyme). |
| **H2** | La vue session apporte une valeur réelle pendant une partie — réduction du temps de recherche, accès au contenu préparé, création à la volée. | ≥ 50 % des MJ ayant atteint activation préparation atteignent **activation vue session** (session ouverte ET réellement utilisée en partie). Signal de valeur confirmé : ≥ 50 % d'entre eux l'utilisent sur 2 sessions ou plus. | 30 jours pour la première activation vue session. 60 jours pour le signal de répétabilité (2 sessions+). | Activation vue session (mesure d'usage anonyme) + entretiens pour raison de non-adoption. |
| **H3** | Le partage d'informations aux joueurs est plus fluide que les solutions actuelles (Discord, Google Docs, papier). | ≥ 40 % des MJ ayant animé une session avec joueurs atteignent **activation partage** (document partagé + au moins un joueur l'a consulté). La perception « plus fluide » est confirmée en entretien : ≥ 3 MJ sur 5 interrogés rapportent une fluidité supérieure aux solutions actuelles. | 60 jours pour activation partage. Entretiens parallèles ou récapitulatifs. | Activation partage (mesure d'usage anonyme). Entretiens utilisateurs (guide existant). |
| **H4** | L'accès joueur sans compte n'est pas un frein à l'adoption du groupe entier. | Sur les sessions où un partage a eu lieu, ≥ 70 % comptent au moins un joueur ayant effectivement consulté le contenu partagé. Les abandons à l'entrée (joueurs ne consultant pas) se vérifient en entretien — moins de 2 joueurs sur 10 rapportent avoir renoncé à l'étape d'entrée. | 60 jours. | Activation partage (mesure d'usage anonyme : taux de consultation côté joueur). Entretiens pour identifier les motifs d'abandon. |
| **H5** | La conversion du mode local vers un compte cloud se produit naturellement quand le besoin de partage ou de sauvegarde apparaît. | ≥ 10 % des MJ actifs en mode local (activation préparation atteinte) créent un compte. Le déclencheur est constaté : intention de partager ou sauvegarde cloud (pas migration forcée). | 90 jours. | Mesure d'usage anonyme (passage du mode local au compte, constaté à la création du compte). Entretiens pour identifier le déclencheur explicite. |

**Calibrage des seuils** : ces seuils reflètent les attentes pour une cohorte d'early adopters recrutés. Chaque seuil peut être ajusté avant le lancement de l'observation (jamais pendant) en fonction du profil réel de la cohorte ou de changements de périmètre produit.

> **Conception d'interface — Support de démonstration de H2**
> 
> La démonstration de l'hypothèse H2 s'appuie sur une conception documentée de l'interface de la vue session. Les wireframes basse-fidélité, dérivés du parcours utilisateur (UJ-UC-06) et des critères d'acceptation des user stories, constituent un livrable au périmètre de la conception. Ils seront produits en session dédiée avec l'opérateur en fin de complétude de la conception, avant le jalon final (décision du 2026-06-10 — arbitrage T-09, audit conception pure 2026-06).

### 2.4 Fonctionnalités hors périmètre MVP

Ces éléments sont explicitement exclus — leur absence est un choix, pas un oubli.

| Fonctionnalité | Raison d'exclusion |
|---|---|
| Table virtuelle visuelle | Concurrence directe Roll20/Foundry — hors positionnement |
| Gestion des combats / jets de dés | Moteur de règles — hors périmètre assumé |
| Intelligence artificielle générative | Risque de dénaturer le positionnement MVP |
| Templates de système de jeu | Complexité de contenu élevée — post-adoption |
| Relations typées entre documents | Post-MVP, nécessite l'adoption des types de document |
| Application desktop / sync offline (CRDT) | Complexité architecturale disproportionnée |
| Templates et marketplace communautaires | Écosystème à construire après validation du core |
| Inventaire personnage | Compète avec D&D Beyond sans pouvoir l'égaler |

---

## 3. Modèle de monétisation

Le modèle est conçu pour être non-agressif : la valeur est réelle avant tout engagement,
l'upgrade est une décision rationnelle déclenchée par un besoin concret.

| Tier | Compte | Fonctionnalités | Limite |
|---|---|---|---|
| **Local** | Aucun | Préparation complète, vue session, création à la volée | Stockage navigateur (~50-100 Mo), pas de partage joueurs, 1 device |
| **Gratuit** | Email + mot de passe | Cloud sync, partage joueurs, accès multi-device | 3 campagnes en cloud, 4 joueurs par session, 500 Mo |
| **Pro** | Abonnement (~7 €/mois) | Tout le gratuit + illimité | Campagnes illimitées, 5 Go+ |

**Déclencheurs naturels d'upgrade :**
- Local → Gratuit : le MJ veut partager une information avec ses joueurs (UC-08), ou il a peur
  de perdre ses données locales (bandeau de rappel non-intrusif dans l'app).
- Gratuit → Pro : le MJ a plus de 3 campagnes actives.

**Ce qui ne force pas l'upgrade :**
- Le mode local ne présente aucun watermark ni limitation visible des fonctionnalités de préparation.
- Les fonctionnalités de partage sont visibles depuis le mode local mais déclenchent une invite
  à créer un compte — elles n'affichent pas d'erreur mystérieuse.

**Référence** : modèle proche d'Obsidian (local gratuit, sync payant). La différence est que
Haversack intègre le partage joueurs dans le tier gratuit, ce qui aligne l'adoption MJ et joueurs.

---

## 4. Acteurs

### 4.1 Maître du Jeu — MJ

**Profil** : utilisateur créant et administrant une ou plusieurs campagnes.
Compte optionnel — peut utiliser l'application en mode local sans s'inscrire.
Seul rôle pouvant créer du contenu, administrer la campagne et partager des informations.

**Personas de référence** : Thomas, Émilie, Nadia, Antoine, Rémi, Sonia
→ [docs/conception/persona/](../persona/)

**Douleurs principales** :
- Temps perdu à chercher une information en pleine partie.
- Charge mentale de préparation élevée.
- Informations dispersées sur des supports incompatibles.
- Difficulté à partager sélectivement des informations aux joueurs.
- Friction à l'adoption d'un nouvel outil (obligation de compte, configuration initiale).

**Objectifs dans l'application** :
- Commencer à préparer immédiatement, sans inscription.
- Structurer ses scénarios et son contenu à sa façon, quel que soit le système de jeu.
- Réduire la charge mentale pendant la session via un accès rapide au contenu préparé.
- Créer des éléments à la volée en session sans interrompre la partie.
- Partager certaines informations aux joueurs sans exposer ses notes privées.

**Parcours principaux** :

```
Mode local (sans compte)
  ├── Campagne → Préparation (scénarios, notes, dossiers) → Vue session
  │                                                        └── Création à la volée
  └── One-shot → Sélection scénario (bibliothèque) → Vue session directe
                   (post-MVP — voir 5bis)

(optionnel) Création de compte → Cloud sync → Partage joueurs → Membres permanents
```

### 4.2 Joueur

**Profil** : utilisateur accédant à une campagne via invitation du MJ.
Compte optionnel — peut accéder en mode invité via un lien de session.

**Persona de référence** : Lucas
→ [persona-03-lucas.md](../persona/persona-03-lucas.md)

**Douleurs principales** :
- Friction à l'adoption d'un nouvel outil (obligation de compte, configuration).
- Informations de campagne dispersées entre Discord, papier et D&D Beyond.

**Objectifs dans l'application** :
- Rejoindre une session avec un minimum de friction — idéalement zéro inscription.
- Accéder en temps réel aux informations partagées par le MJ pendant la partie.
- (Optionnel, avec compte) Retrouver l'historique des sessions entre les parties.

**Parcours principal** :

```
Lien de session reçu → Saisie d'un nom d'affichage → Accès aux informations partagées
                                                     └── (optionnel) Création de compte
                                                           → Membre permanent de la campagne
```

### 4.3 Joueur invité — mode session ponctuelle

**Profil** : joueur rejoignant via un lien temporaire, sans création de compte.
Accès limité à la durée de la session et au périmètre défini par le MJ.
Typique pour les one-shots, les conventions, les groupes changeants.

**Objectifs** :
- Rejoindre rapidement sans aucune configuration.
- Consulter les informations partagées pour cette session uniquement.

**Contraintes** :
- L'accès expire à la fin de la session (fenêtre de grâce de 24h).
- Pas d'accès à l'historique des sessions précédentes.
- Peut créer un compte à tout moment pour convertir son accès en membre permanent (UC-09 —
  octroi d'accès, UC-11 — gestion côté MJ), sans perdre les données de session déjà consultées.
  Une fois membre, il accède à une vue cohérente de la campagne (UC-12 — vue joueur).

---

## 5. Positionnement concurrentiel

| Outil | Forces | Limites vs Haversack |
|---|---|---|
| Notion | Très flexible, markdown riche | Non spécialisé JDR, pas de vue session, pas de partage sélectif |
| Obsidian | Graphe de liens, markdown local | Pas de collaboration, pas de vue session, pas de partage joueurs |
| Roll20 | Table virtuelle complète, dés, cartes | Centré mécanique, pas conçu pour la narration |
| Foundry VTT | Très puissant, extensible | Complexité élevée, centré sur le combat |
| WorldAnvil | Worldbuilding riche | Centré lore, pas de vue session, courbe d'apprentissage élevée |
| Google Docs + Discord | Simple, connu | Aucune logique JDR, information dispersée |

**Différenciants de Haversack :**

- **Friction d'entrée nulle** — aucun compte pour commencer, données locales immédiates et toujours possédées par l'utilisateur (pas de compte requis pour conserver ses données en local). La possession est actionnable : le MJ peut exporter l'ensemble de son espace dans un format ouvert et le consulter hors de l'application.
- **Vue session dédiée** — le seul outil centré sur le pilotage de session en temps réel.
- **Agnostique au système de jeu** — fonctionne pour D&D, Call of Cthulhu, Fate, Blades,
  systèmes maison, systèmes narratifs sans imposer une structure.
- **Partage fluide** — le MJ contrôle ce qui est visible aux joueurs, document par document, et garde ses notes de préparation privées. L'ambition long terme est la granularité par joueur ou personnage, mais le MVP valide d'abord que le partage au groupe suffit à offrir une expérience plus fluide que les solutions actuelles.
- **Accès joueur sans compte** — un lien, un nom, c'est tout.
- **Organisation libre** — le MJ structure son espace à sa façon, pas à la façon de l'app.

---

## 5bis. Traces d'arbitrage vision ↔ périmètre MVP

### Granularité du partage — arbitrage du 2026-06-10

**Décision** : le MVP livre un **partage par document** — le MJ contrôle la visibilité document par document (tout-ou-tous), et aucune information de préparation privée n'est jamais exposée aux joueurs.

**Raison d'être produit** : valider d'abord que le partage au groupe (sans secret intra-groupe) est plus fluide que les solutions actuelles et justifie l'adoption avant d'investir dans la granularité fine. La granularité fine ajoute par ailleurs une charge cognitive au MJ en pleine session et complique le contrôle de qui voit quoi — un coût injustifié tant que le flux de partage n'est pas validé.

**Alternatives considérées** : livrer la granularité par joueur ou personnage dès le MVP — écartée, l'ambition est conservée comme horizon produit post-MVP.

**Condition de retour** : si l'usage ou les entretiens avec les MJ révèlent que le partage à tout le groupe bloque des tables réelles (révélations destinées à un seul joueur, gestion des secrets entre joueurs trop rigide), la granularité par joueur ou personnage sera réexaminée.

### Possession des données — arbitrage du 2026-06-10

**Décision** : l'export d'espace est rehaussé en Should Have — le MJ peut exporter l'ensemble de son espace dans un format ouvert et le consulter hors de l'application.

**Raison d'être produit** : le différenciant n°1 de la vision est la possession des données. Cette possession ne peut être qu'une affirmation sans une capacité concrète et actionnable. L'export matérialise la promesse de possession et répond aux douleurs de confiance (Thomas, Rémi) et au besoin de filet de sécurité du mode local.

**Alternatives considérées** : reporter l'export en Could Have — rejeté car laisserait le différenciant « possession » sans matérialisation concrète.

**Condition de retour** : aucune. C'est une promotion de priorité, non un report conditionné.

### Contexte one-shot — arbitrage du 2026-06-10

**Décision** : le MVP cible d'abord les campagnes. Le one-shot complet — parcours express, scénarios réutilisables, UC-13 — est explicitement hors première livraison. L'ambition « deux contextes de premier ordre » reste l'horizon produit.

**Raison d'être produit** : la validation du cœur du produit (préparation, vue session, partage, monétisation) passe par le contexte campagne, qui porte les personas principaux (Thomas, Émilie, Lucas, Antoine) et structure l'hypothèse de monétisation (membres stables, partage durable). Le one-shot complet dépend du catalogue de scénarios réutilisables — UC-13, une capacité entière qui est une construction post-MVP et ne conditionne pas la validation initiale. Un one-shot reste possible en MVP sous la forme d'une campagne à session unique, sans parcours express ni catalogue réutilisable.

**Alternatives considérées** : rehausser UC-13 dans la première livraison pour livrer les deux contextes dès le MVP — écartée. L'arbitrage privilégie la solidité du contexte campagne et maintient l'ambition deux-contextes pour la suite, avec trace explicite de la condition de retour.

**Condition de retour** : si les entretiens ou l'usage révèlent que le profil one-shot exclusif (Sonia — conventions, groupes changeants, catalogue de scénarios) est une part significative des utilisateurs réels ou un levier d'adoption, UC-13 est réexaminé en priorité de la vague suivante.

---

### Espace personnel & généralisation Campaign→Space — arbitrage du 2026-06-12

**Décision** : Voie 3 retenue — l'agrégat `Campaign` est généralisé en `Space` ; `SpaceType ∈ {CAMPAIGN, ONE_SHOT, PERSONAL}`. Un espace de type `PERSONAL` est créé par défaut à la création du compte (et disponible comme simple conteneur en mode local). L'espace personnel est opérationnel dès le MVP, y compris comme zone d'atterrissage par défaut pour les documents créés sans espace explicite. La campagne et le one-shot deviennent des spécialisations d'un espace — le contenu appartient à l'espace, pas à la campagne.

**Raison d'être produit** : le processus créatif du MJ produit du contenu (lieu, PNJ, scénario, règle maison) sans lien avec une campagne en cours. Ce contenu est de premier ordre — il appartient au MJ, pas à une campagne. Reconnaître l'espace personnel comme conteneur par défaut supprime la friction « je dois créer une campagne pour noter une idée » et est directement cohérent avec le différenciant « friction d'entrée nulle » et avec le principe « Tout est Document ». Cette décision change la définition de l'instrument H1 : « activation préparation » compte désormais le contenu créé dans l'espace personnel au même titre qu'une campagne créée.

**Alternatives considérées** :
- *Voie 1 — racine sur l'Utilisateur (`Document.ownerId` de premier ordre, `campaignId` nullable)* — **rejetée, flaw STRUCTURAL sur trois axes indépendants** : violation de la frontière de contexte Identity & Access (ADR), résidu post-effacement RGPD (aucune saga ADR-011 n'atteint un document `campaignId = NULL`), incompatibilité mode local (pas de `User` en mode local, ADR-017 §1.1).
- *Voie 2 — `CampaignType.PERSONAL` sans renommage* — repli viable, non retenu : dette sémantique significative (le MJ stocke son contenu dans « une campagne nommée Personnel », réintroduit le biais campagne-centré). La `ScenarioLibrary` reste un pont artificiel sans résolution naturelle.

**Condition de retour** : si un use case démontre une nature distincte du contenu personnel — comportement de partage, gestion du cycle de vie, ou contrainte de visibilité — incompatible avec une spécialisation de l'agrégat `Space`, la séparation en agrégat dédié est réexaminée.

---

## 6. Architecture documentaire — vision d'évolution

Le système de document de Haversack est conçu pour évoluer progressivement sans rupture :

```
Maintenant — Document libre (blocs)
  └── + Type optionnel → propriétés structurées (Nom, Rôle, PV…)

Post-MVP — Relations typées entre documents
  └── Ce PNJ appartient à cette Faction
  └── Cet Objet est porté par ce Personnage

Long terme — Règles légères sur les propriétés
  └── Calculateur contextuel basé sur les propriétés déclarées
  └── Émulation partielle de système de jeu (jamais un moteur complet)
```

Le schéma de données `Document` prévoit `documentTypeId` et `properties` (structure de données structurée)
dès le MVP pour ne pas nécessiter de migration majeure lors de l'activation des relations.

Détail → [UC-HORS-MVP — UC-F06](../usecases/UC-HORS-MVP.md)

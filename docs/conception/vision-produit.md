# Vision produit — Haversack

> Document de référence pour le positionnement produit, les choix assumés
> et la définition des utilisateurs cibles.

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
> ses notes et accéder rapidement aux informations importantes pendant une session.

Le MVP valide une hypothèse centrale : **un MJ paiera pour un outil qui réduit
la friction de préparation et de pilotage**, même sans fonctionnalités avancées
(IA, table virtuelle, gestion de règles).

### 2.2 Choix produit assumés

**Le MJ est l'utilisateur principal.** Ce choix est délibéré et non négociable pour le MVP.

Justification économique et produit :
- Le MJ porte l'intégralité de la charge de préparation.
- Le MJ choisit les outils utilisés par le groupe — il est le décideur d'achat.
- Le MJ a la douleur utilisateur la plus forte et la plus documentable.
- Le MJ représente le profil le plus susceptible de payer pour un outil SaaS dédié.
- Une adoption réussie côté MJ entraîne mécaniquement l'adoption côté joueurs.

**Les joueurs sont des utilisateurs secondaires.** Leur expérience doit être simple
et sans friction (notamment sans obligation de compte), mais le produit ne cherche pas
à devenir une plateforme joueur dans son MVP.

**Pas de moteur de règles.** Haversack est agnostique au système de jeu. Il ne gère
pas les jets de dés, les combats ou les mécaniques. Cette décision réduit la complexité
et élargit la cible (MJ de D&D, Call of Cthulhu, Fate, Pathfinder, etc.).

**Modèle Document flexible.** Plutôt que de créer des formulaires figés pour chaque
type de contenu, Haversack utilise un modèle de blocs inspiré de Notion. Cela permet
de s'adapter à n'importe quel système de jeu sans migration de schéma.

**Organisation libre par dossiers.** Le MJ organise son contenu dans des dossiers qu'il
crée et nomme lui-même. Quatre dossiers système (PNJ, Personnages joueurs, Scénarios, Notes)
sont générés automatiquement à la création de la campagne — point de départ, pas une cage.
Le MJ peut créer autant de dossiers personnalisés que nécessaire (Lieux, Factions, Artefacts...).

**Références entre documents.** N'importe quel document peut référencer un autre via
un bloc RELATION. Les backlinks (documents pointant vers un document donné) sont consultables
depuis la fiche cible — la campagne devient un réseau d'informations navigable.

### 2.3 Ce que le MVP doit démontrer

Pour valider le concept et justifier une suite, le MVP doit prouver que :

1. Un MJ peut créer une campagne structurée et y retrouver ses informations.
2. La vue session apporte une valeur réelle pendant une partie (réduction du temps de recherche).
3. Le partage d'informations aux joueurs est plus fluide que les solutions actuelles.
4. L'absence de compte obligatoire pour les joueurs n'est pas un frein à l'adoption.

### 2.4 Fonctionnalités hors périmètre MVP

Ces éléments sont explicitement exclus — leur absence est un choix, pas un oubli.

| Fonctionnalité | Raison d'exclusion |
|---|---|
| Table virtuelle visuelle | Concurrence directe Roll20/Foundry — hors positionnement |
| Cartes interactives | Complexité technique élevée, non core |
| Gestion automatisée des combats | Moteur de règles — hors périmètre |
| Audio / vidéo | Infrastructure coûteuse, hors positionnement |
| Intelligence artificielle générative | Risque de dénaturer le positionnement MVP |
| Marketplace / plugins | Écosystème à construire après validation du core |
| Support complet de plusieurs systèmes de jeu | Templates configurables — post-MVP |
| Collaboration temps réel complexe (CRDT) | Complexité technique disproportionnée pour le MVP |
| SessionSummary multi-versions | Modélisé, non activé en MVP |
| Verrouillage de champs (`isLocked`) | Modélisé, non activé en MVP |
| Factions comme entité dédiée | Couvert par Document CUSTOM en MVP |
| UserProjection locale dans Campaign Management | Nécessaire uniquement lors de l'extraction en service |

---

## 3. Acteurs

### 3.1 Maître du Jeu — MJ

**Profil** : utilisateur créant et administrant une ou plusieurs campagnes.
Compte obligatoire. Seul rôle pouvant créer du contenu et administrer la campagne.

**Douleurs principales** :
- Temps perdu à chercher une information en pleine partie.
- Charge mentale de préparation élevée.
- Informations dispersées sur des supports incompatibles.
- Difficulté à partager sélectivement des informations aux joueurs.

**Objectifs dans l'application** :
- Préparer une campagne plus efficacement.
- Structurer ses scénarios et ses PNJ.
- Centraliser les informations importantes dans un seul espace.
- Réduire la charge mentale pendant la session.
- Partager certaines informations aux joueurs sans exposer ses notes privées.
- Créer à la volée pendant la session sans casser le rythme.

**Parcours principal** :
```
Inscription → Création campagne → Préparation scénarios/PNJ/notes
           → Préparation session → Vue session → Clôture session
```

### 3.2 Joueur

**Profil** : utilisateur accédant à une campagne via invitation du MJ.
Compte facultatif — peut accéder via GuestAccess (lien temporaire).

**Douleurs principales** :
- Difficulté à retrouver sa fiche personnage pendant la partie.
- Informations de campagne dispersées sur Discord ou papier.
- Friction à l'adoption d'un nouvel outil (obligation de compte, configuration).

**Objectifs dans l'application** :
- Accéder facilement à sa fiche personnage et son inventaire.
- Retrouver les informations partagées par le MJ.
- Prendre des notes personnelles sans qu'elles soient visibles par le MJ.
- Participer à une session avec un minimum de configuration.

**Parcours principal** :
```
Réception lien → Rejoindre campagne (avec ou sans compte)
              → Consulter fiche personnage → Notes personnelles
```

### 3.3 Joueur invité sans compte — GuestAccess

**Profil** : joueur rejoignant via un lien temporaire, sans création de compte.
Accès limité en durée et en périmètre. Peut créer un compte ultérieurement
pour convertir son accès en CampaignMembership persistant.

**Objectifs** :
- Rejoindre rapidement une session avec un minimum de friction.
- Être associé à un personnage par le MJ.
- Accéder aux informations partagées pour la session.

**Contraintes** :
- Pas de compte persistant — ses données `PLAYER_PRIVATE` restent inaccessibles après expiration.
- Accès limité au périmètre défini par le MJ.
- Peut upgrader en compte joueur à tout moment (UC-00 A3).

---

## 4. Positionnement concurrentiel

| Outil | Forces | Limites vs Haversack |
|---|---|---|
| Notion | Très flexible, markdown riche | Non spécialisé JDR, pas de vue session, pas de partage sélectif |
| Obsidian | Graphe de liens, markdown local | Pas de collaboration, pas de vue session, pas de partage joueurs |
| Roll20 | Table virtuelle complète, dés, cartes | Centré mécanique, pas conçu pour la narration |
| Foundry VTT | Très puissant, extensible | Complexité élevée, centré sur le combat |
| WorldAnvil | Worldbuilding riche | Centré lore, pas de vue session, courbe d'apprentissage élevée |
| Google Docs + Discord | Simple, connu | Aucune logique JDR, information dispersée |

**Positionnement différenciant de Haversack** :
- Seul outil centré sur le **pilotage de session** (vue session MJ).
- Modèle de contenu **agnostique au système de jeu**.
- Partage **sélectif et granulaire** d'informations aux joueurs.
- Accès joueur **sans friction** (pas de compte obligatoire).
- **Organisation libre** du contenu via dossiers personnalisables — le MJ structure sa campagne à sa façon.
- **Références entre documents** avec navigation par backlinks — la campagne devient un graphe d'informations.

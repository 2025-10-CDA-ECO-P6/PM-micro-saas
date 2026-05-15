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

**Deux contextes de jeu de premier ordre.** Haversack reconnaît deux modes d'utilisation
distincts qui ne partagent pas les mêmes besoins :

- **Campagne** : plusieurs sessions, groupe stable, continuité narrative.
  Configuration initiale, gestion des membres, historique des sessions.

- **One-shot** : une seule session, joueurs potentiellement différents, aucune continuité.
  Parcours express — un MJ peut lancer un one-shot en moins de 30 secondes depuis sa
  bibliothèque de scénarios. Aucune configuration de campagne n'est demandée.

Le one-shot n'est pas une campagne dégradée. C'est un contexte de jeu à part entière,
avec son propre point d'entrée dans l'application. Techniquement, les deux reposent sur
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

Pour valider le concept et justifier une suite, le MVP doit prouver que :

1. Un MJ peut créer une campagne structurée et y retrouver ses informations sans friction
   d'onboarding (pas de compte obligatoire au démarrage).
2. La vue session apporte une valeur réelle pendant une partie — réduction du temps de
   recherche, accès au contenu préparé, création à la volée.
3. Le partage d'informations aux joueurs est plus fluide que les solutions actuelles
   (Discord, Google Docs, papier).
4. L'accès joueur sans compte n'est pas un frein à l'adoption du groupe entier.
5. La conversion du mode local vers un compte payant se produit naturellement quand
   le besoin de partage ou de cloud apparaît.

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
| **Gratuit** | Email + mot de passe | Cloud sync, partage joueurs, accès multi-device | 3 campagnes en cloud, 500 Mo |
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
- Peut créer un compte à tout moment pour convertir son accès en membre permanent (UC-12),
  sans perdre les données de session déjà consultées.

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

- **Zéro friction au démarrage** — aucun compte pour commencer, données locales immédiates.
- **Vue session dédiée** — le seul outil centré sur le pilotage de session en temps réel.
- **Agnostique au système de jeu** — fonctionne pour D&D, Call of Cthulhu, Fate, Blades,
  systèmes maison, systèmes narratifs sans imposer une structure.
- **Partage sélectif** — le MJ révèle exactement ce qu'il veut, quand il le veut.
- **Accès joueur sans compte** — un lien, un nom, c'est tout.
- **Organisation libre** — le MJ structure sa campagne à sa façon, pas à la façon de l'app.

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

Le schéma de données `Document` prévoit `documentTypeId` et `properties` (JSON structuré)
dès le MVP pour ne pas nécessiter de migration majeure lors de l'activation des relations.

Détail → [UC-HORS-MVP — UC-F06](../usecases/UC-HORS-MVP.md)

# Roadmap produit — Haversack

## Trajectoire de valeur, jalon de validation et paliers d'évolution du produit

| | |
|---|---|
| **Version du document** | 1.0 |
| **Statut** | Version de référence — diffusion externe restreinte |
| **Date** | 2026-07-16 |
| **Émetteur** | Équipe produit — Haversack |
| **Audience** | Communication externe : investisseurs, partenaires, parties prenantes non techniques |
| **Confidentialité** | Confidentiel — diffusion restreinte |

Cette roadmap présente les paliers de valeur du produit Haversack, dans l'ordre où ils sont
livrés aux utilisateurs — maîtres du jeu (MJ) et joueurs. Elle répond à une question simple :
*qu'est-ce que le produit apporte, palier après palier ?*

Cette roadmap décrit une trajectoire de valeur, pas un calendrier : le séquencement temporel
opérationnel relève d'un support de planification dédié. De même, les mentions de modèle
économique (tiers, seuils) sont sujettes à révision et ne constituent pas un engagement
tarifaire figé.

Ce document couvre la trajectoire de valeur du produit ; le dimensionnement de marché et le
modèle d'affaires détaillé sont traités dans un document dédié.

### Historique de révision

| Version | Date | Auteur | Nature de la révision |
|---|---|---|---|
| 1.0 | 2026-07-16 | Équipe produit | Première émission |

---

## Sommaire

- [Résumé exécutif](#résumé-exécutif)
- [Vue synoptique de la trajectoire](#vue-synoptique-de-la-trajectoire)
- [1. Proposition de valeur](#1-proposition-de-valeur)
- [2. Le socle](#2-le-socle)
- [3. Le jalon de validation](#3-le-jalon-de-validation)
- [4. Approfondissement du cœur](#4-approfondissement-du-cœur)
- [5. Extensions déclenchées par signal](#5-extensions-déclenchées-par-signal)
- [6. Vision long terme](#6-vision-long-terme)
- [Annexe A — Correspondance avec le périmètre de conception](#annexe-a--correspondance-avec-le-périmètre-de-conception)
- [Annexe B — Glossaire](#annexe-b--glossaire)

---

<a id="résumé-exécutif"></a>
## Résumé exécutif

Un maître du jeu (MJ) actif gère une quantité d'information considérable — scénarios, historique
de session, personnages non joueurs, notes, lore — aujourd'hui dispersée sur des supports qui
n'ont pas été conçus pour cet usage (fichiers texte, outils génériques, messageries, papier).
Cette fragmentation coûte du temps de recherche en pleine partie, casse le rythme de jeu et
alourdit la charge de préparation. Les outils existants ne comblent pas ce manque : les outils
génériques manquent de logique de jeu de rôle, les outils de table virtuelle sont centrés sur la
mécanique (combat, dés) plutôt que sur la narration et la préparation.

Haversack se positionne comme l'outil d'assistance au MJ centré sur deux moments : la
préparation d'une partie et le pilotage en direct pendant qu'elle se joue — agnostique au
système de jeu utilisé. Ses différenciants : une friction d'entrée nulle (aucune inscription
pour commencer), une vue dédiée au pilotage de session, l'agnosticisme au système de jeu, un
partage fluide et maîtrisé avec les joueurs, un accès joueur sans compte, et une possession
actionnable des données (export à tout moment, indépendamment de l'application).

La trajectoire produit se décompose en quatre paliers de valeur. **Le socle**, livré comme un
bloc unique et indivisible, couvre la préparation, le pilotage de session, le partage aux
joueurs et la possession des données — le minimum cohérent pour tester l'hypothèse centrale du
produit. **L'approfondissement du cœur**, engagé après validation, renforce ce qui existe déjà
(contenu plus riche, collaboration de groupe stable, recherche à l'échelle) sans ouvrir de
nouveaux usages. **Les extensions déclenchées par signal** regroupent des capacités à la valeur
reconnue mais dont la mise en priorité dépend de signaux mesurés en usage réel plutôt que d'une
décision prise à l'avance — chacune est associée à une condition de retour explicite. **La
vision long terme** décrit l'horizon du produit au-delà de cette trajectoire immédiate, avec ses
opportunités et ses risques de positionnement assumés.

Entre le socle et les paliers suivants s'intercale un **jalon de validation** : un point de
décision go/no-go explicite, fondé sur cinq questions observées en usage réel (engagement en
préparation, valeur de la vue de session, fluidité du partage, absence de friction pour les
joueurs invités, conversion naturelle vers un compte). Ce jalon est directement lié au modèle
économique du produit — une progression en trois paliers, d'un usage local sans compte à un
compte gratuit puis payant — dont la cinquième question teste précisément la mécanique de
conversion. Un résultat positif ouvre la voie à l'approfondissement du cœur ; un résultat partiel
ou négatif sur un pilier oriente la priorité vers la consolidation avant l'élargissement.

---

<a id="vue-synoptique-de-la-trajectoire"></a>
## Vue synoptique de la trajectoire

| Palier | Valeur livrée (résumé) | Bénéficiaire principal | Nature |
|---|---|---|---|
| Le socle | Préparer une partie sans inscription, piloter une session en direct, partager sélectivement avec les joueurs, posséder et exporter ses données. | MJ et joueur | Engagement actuel |
| Approfondissement du cœur | Contenu plus riche sans rigidité, collaboration de groupe stable dans le temps, recherche de l'information à l'échelle. | MJ et joueur | Post-validation |
| Extensions déclenchées par signal | Contexte one-shot complet, granularité fine du partage, portabilité entrante, modèle de contenu personnalisable, expérience joueur persistante. | MJ et joueur | Conditionnel — par signal mesuré |
| Vision long terme | Préparation adaptée au système de jeu, relations et règles légères entre documents, bibliothèque de réutilisation, assistant d'aide au MJ, ouverture communautaire. | MJ | Horizon |

---

<a id="1-proposition-de-valeur"></a>
## 1. Proposition de valeur

### Le problème du maître du jeu

Un maître du jeu actif gère simultanément une quantité d'information considérable : scénarios
préparés, historique de ce qui s'est passé en session, fiches de personnages non joueurs, notes
secrètes, informations à partager avec les joueurs, lore, lieux, objets importants.

Cette information est aujourd'hui dispersée sur des supports qui n'ont pas été conçus pour cet
usage : fichiers texte, outils de prise de notes génériques, messageries, papier, PDF. Cette
fragmentation coûte du temps de recherche pendant la partie, casse le rythme de jeu, et alourdit
la charge mentale de préparation en amont.

Les outils existants ne comblent pas ce manque. Les outils génériques (type prise de notes ou
documents partagés) sont flexibles mais sans logique de jeu de rôle : le MJ doit construire sa
propre organisation, et elle n'est pas pensée pour un accès rapide en session. Les outils de
table virtuelle sont riches mais centrés sur la mécanique de jeu — combat, cartes, jets de dés —
la préparation narrative et la gestion documentaire y sont secondaires.

### Le créneau de Haversack

Haversack se positionne comme l'outil d'assistance au MJ centré sur deux moments : la
préparation d'une partie, et le pilotage en direct pendant qu'elle se joue. Ni une table
virtuelle, ni un éditeur de texte générique — un outil pensé pour les problèmes concrets du MJ,
et volontairement agnostique au système de jeu utilisé (les mécaniques de règles, combats et
jets de dés ne font pas partie du produit).

### Ce qui différencie Haversack

- **Friction d'entrée nulle** — aucune inscription n'est nécessaire pour commencer à préparer.
  Les données restent disponibles localement, sans dépendre d'un compte.
- **Une vue dédiée au pilotage de session** — un espace de travail pensé spécifiquement pour
  l'usage pendant la partie, pas une adaptation d'un outil de prise de notes générique.
- **Agnosticisme au système de jeu** — le produit fonctionne pour n'importe quel système de jeu
  de rôle, sans imposer une structure ou un vocabulaire de règles.
- **Partage fluide et maîtrisé** — le MJ choisit précisément ce qu'il rend visible à ses joueurs,
  sans exposer ses notes de préparation privées.
- **Accès joueur sans friction** — un joueur rejoint une session sans créer de compte.
- **Possession actionnable des données** — ce que le MJ produit lui appartient et reste
  consultable indépendamment de l'application, pas seulement en principe mais de façon concrète.

---

<a id="2-le-socle"></a>
## 2. Le socle

Le socle est la première livraison du produit. Il est conçu et livré comme **un bloc unique et
indivisible** : il n'y a pas de version intermédiaire entre « rien » et le socle complet, parce
que c'est l'ensemble de ce bloc qui permet de tester l'hypothèse centrale du produit — pas une
partie de celui-ci prise isolément.

### Ce que le socle apporte au maître du jeu

- **Commencer sans engagement** : le MJ ouvre l'application et prépare sa première campagne ou
  son premier one-shot sans créer de compte. Ses données restent disponibles localement entre
  ses sessions de travail.
- **Un espace de capture immédiat** : le MJ peut noter une idée — un lieu, un personnage
  non joueur, une esquisse de scénario — sans avoir besoin qu'une campagne existe déjà pour
  l'accueillir. Un espace personnel, propre au MJ, existe par défaut dès le premier usage et
  reçoit ce contenu.
- **Structurer un scénario et organiser son contenu librement** : le MJ construit ses scénarios
  sous forme de scènes liées, et range ses documents dans des dossiers qu'il nomme lui-même —
  sans structure imposée par un système de jeu particulier.
- **Piloter une session en direct** : une vue dédiée regroupe l'accès aux scènes, aux notes et au
  contenu préparé pendant que la partie se joue — c'est le cœur différenciant du produit.
- **Créer dans l'instant, sans casser le rythme** : pendant la session, le MJ peut créer un
  nouvel élément (un personnage non joueur improvisé, un lieu qui émerge en jeu) en quelques
  secondes, sans quitter sa vue de pilotage.
- **Partager sélectivement avec ses joueurs** : le MJ décide, document par document, ce qui
  devient visible à ses joueurs, en gardant ses notes de préparation privées.
- **Posséder concrètement ses données** : le MJ peut, à tout moment, exporter l'intégralité de
  son espace dans un format ouvert et lisible hors de l'application — que ce soit en usage local
  ou avec un compte. Cette capacité matérialise la promesse de possession des données : ce n'est
  pas une déclaration d'intention, c'est un geste que le MJ peut faire.

### Ce que le socle apporte au joueur

- **Rejoindre sans inscription** : un joueur qui reçoit un lien de session saisit simplement un
  nom d'affichage et accède immédiatement aux informations partagées par son MJ, en temps réel.
- **Aucune configuration préalable** : pas de compte à créer pour participer à une session, pas
  de courbe d'apprentissage avant de pouvoir suivre la partie.

### Le compte, une évolution naturelle plutôt qu'une porte d'entrée

Le compte et la synchronisation dans le cloud font partie du socle — ils sont nécessaires pour
que le partage aux joueurs fonctionne réellement (la diffusion en temps réel vers un groupe
suppose un service en ligne). Mais l'usage local reste pleinement fonctionnel pour la
préparation : le compte est une étape que le MJ franchit lorsqu'il veut partager avec son groupe
ou sécuriser ses données, jamais un préalable imposé pour commencer à utiliser le produit.

### Pourquoi ce périmètre, pas un autre

Le socle teste une hypothèse produit et une hypothèse de modèle économique en même temps :
qu'un MJ retire une valeur suffisante d'un outil qui réduit la friction de préparation et de
pilotage — même sans intelligence artificielle, sans table virtuelle, sans gestion de règles —
et que le passage du local gratuit vers un compte se fait naturellement, porté par le besoin de
partager plutôt qu'imposé. Une mesure d'usage anonyme, qui ne capte aucun contenu narratif, est
intégrée dès cette première livraison pour permettre de constater, pilier par pilier, ce qui
fonctionne et ce qui ne fonctionne pas.

---

<a id="3-le-jalon-de-validation"></a>
## 3. Le jalon de validation

Avant d'engager les paliers suivants, le socle doit démontrer sa valeur auprès d'utilisateurs
réels. Ce jalon fonctionne comme un point de décision explicite : il détermine si, et comment,
la suite est engagée — pas une formalité de suivi.

### Ce qui est observé

Cinq questions structurent l'observation, chacune associée à une mesure et à une fenêtre
d'observation après le début d'usage :

- **La préparation engage-t-elle réellement le MJ ?** Au-delà de l'ouverture de l'application,
  le MJ crée-t-il effectivement un espace de travail structuré et y installe-t-il ses premières
  informations — sans qu'un compte lui ait été imposé pour démarrer ?
- **La vue de pilotage de session apporte-t-elle une valeur pendant la partie ?** Le MJ
  utilise-t-il réellement cette vue en jeu — pas seulement à l'ouverture — et y revient-il d'une
  session à l'autre ?
- **Le partage aux joueurs est-il plus fluide que les solutions actuelles ?** Le MJ partage-t-il
  effectivement des informations, et les MJ interrogés confirment-ils en entretien une fluidité
  supérieure à ce qu'ils utilisaient auparavant ?
- **L'absence de compte joueur est-elle vraiment sans friction pour le groupe ?** Les joueurs
  invités consultent-ils effectivement ce qui leur est partagé, ou l'étape d'entrée reste-t-elle
  un point d'abandon ?
- **La conversion vers un compte se produit-elle naturellement ?** Les MJ actifs en usage local
  franchissent-ils l'étape du compte de leur propre initiative, poussés par un besoin concret de
  partage ou de sauvegarde — plutôt que par une contrainte du produit ?

Chaque question est associée à un seuil de réussite clair et à une fenêtre d'observation
allant de deux semaines (premier engagement en préparation) à trois mois (conversion vers un
compte). Un seuil non atteint donne lieu à un constat explicite — réussite partielle, échec
d'un des trois piliers, ou hypothèse non démontrée — jamais à une réinterprétation a posteriori.
Ces seuils sont calibrés pour une cohorte pilote restreinte et peuvent être ajustés avant le
lancement de l'observation, jamais pendant.

### Le lien avec le modèle économique

L'usage du produit suit une progression en trois paliers : un usage local sans compte, un
compte gratuit qui débloque la synchronisation et le partage aux joueurs dans une limite
raisonnable d'espaces actifs, et un palier payant pour un usage sans limite. La cinquième
question du jalon — la conversion naturelle vers le compte — est directement le test de cette
progression : elle vérifie que le passage d'un palier à l'autre se produit parce que le besoin
apparaît, pas parce que le produit bloque l'accès à la valeur.

### Ce que ce jalon décide

C'est un point de passage — go/no-go — avant d'investir dans les paliers suivants. Un résultat
positif ouvre la voie à l'approfondissement du cœur produit. Un résultat partiel ou négatif sur
un pilier donné oriente la priorité : consolider ce qui ne fonctionne pas avant d'élargir le
périmètre.

---

<a id="4-approfondissement-du-cœur"></a>
## 4. Approfondissement du cœur

Une fois le socle validé, ce palier renforce ce qui existe déjà plutôt que d'ouvrir de nouveaux
usages. Il se présente comme un ensemble cohérent de renforcements, pas comme une succession de
livraisons séparées.

### Une préparation plus riche, sans rigidité

Le socle organise le contenu en dossiers libres, sans structure imposée. Ce palier ajoute une
couche optionnelle : des types de contenu prédéfinis (un personnage non joueur, un lieu, un
objet…) que le MJ peut associer à ses documents pour les enrichir — sans jamais devenir une
contrainte. Un MJ qui préfère ses dossiers libres continue de travailler exactement comme avant.
Cette couche prépare aussi, en arrière-plan, les évolutions futures autour des relations entre
documents (voir palier « Vision long terme »).

### Une collaboration de groupe qui dure dans le temps

Le partage aux joueurs fonctionne déjà dans le socle — ce palier ne le conditionne pas, il
l'enrichit. Le MJ peut désormais inviter des membres permanents dans son espace, avec une
invitation durable et la possibilité de révoquer un accès, plutôt que de ne gérer que des accès
ponctuels par lien de session. En miroir, le joueur qui a rejoint un espace de façon durable
dispose d'une vue cohérente et unifiée de ce qui lui est accessible — sa fiche, les documents qui
le concernent, l'historique partagé — plutôt que de simplement consulter ce qui lui a été envoyé
au fil de l'eau.

### Retrouver l'information à mesure que le contenu s'accumule

Dès que l'espace de travail accumule plusieurs semaines de contenu, retrouver une information
précise devient un besoin de survie en session. Ce palier introduit une recherche par mot-clé
qui permet au MJ de retrouver rapidement n'importe quel document de son espace, y compris depuis
la vue de pilotage de session. Un raffinement ultérieur — filtrer par étiquette plutôt que par
mot-clé — reste un horizon distinct, évalué en fonction des usages réels une fois la recherche de
base en place.

---

<a id="5-extensions-déclenchées-par-signal"></a>
## 5. Extensions déclenchées par signal

Ce palier regroupe des capacités dont la valeur est reconnue, mais dont la priorité dépend de
signaux mesurés plutôt que d'une décision prise à l'avance. Chacune est présentée avec la
condition concrète qui déclencherait sa mise en priorité — une manière honnête de dire « pas
maintenant, mais voici ce qu'on surveille », plutôt que de la présenter comme un doute vague.

### Le contexte one-shot complet

Le socle traite une partie unique comme une campagne à session unique — cela fonctionne, mais
sans le parcours dédié. Une expérience complète pour le MJ qui organise des parties ponctuelles
(conventions, groupes changeants, catalogue de scénarios prêts à l'emploi) suppose une
bibliothèque de scénarios réutilisables, avec une instanciation en un geste depuis l'espace
personnel du MJ — l'infrastructure de cet espace personnel existe déjà dans le socle, seule
l'interface de bibliothèque reste à construire.

**Condition de retour** : ce palier est réexaminé en priorité si l'usage réel ou les entretiens
utilisateurs montrent que le profil de MJ organisant exclusivement des parties ponctuelles
représente une part significative des utilisateurs, ou un levier d'adoption identifiable.

### Une granularité de partage plus fine

Le socle partage l'information au niveau du groupe entier, document par document. Une évolution
envisagée est de permettre au MJ de cibler un partage vers un joueur ou un personnage précis —
utile pour une révélation destinée à une seule personne, ou pour gérer des secrets entre joueurs
au sein d'un même groupe.

**Condition de retour** : cette granularité fine est réexaminée si l'usage révèle que le partage
au groupe entier bloque des situations de jeu réelles.

### La portabilité entrante

Le socle permet déjà d'exporter l'intégralité d'un espace vers un format ouvert. La capacité
symétrique — réimporter un fichier de sauvegarde pour reconstituer un espace sans tout ressaisir
— reste un palier distinct.

**Condition de retour** : cette capacité est réexaminée si la migration manuelle depuis un autre
outil est identifiée comme un frein principal à l'adoption chez les utilisateurs déjà équipés
d'un système de prise de notes existant.

### Un modèle de contenu personnalisable

Au-delà des types de document prédéfinis (voir palier « Approfondissement du cœur »), le MJ
pourrait définir ses propres types de contenu, avec des propriétés sur mesure adaptées à son
système de jeu.

**Condition de retour** : cette capacité est réexaminée si les types prédéfinis se révèlent
insuffisants après un usage réel prolongé.

### Une expérience joueur qui persiste dans le temps

Le joueur pourrait prendre des notes personnelles liées à une session, qui persistent d'une
partie à l'autre — une valeur qui n'apparaît que pour des groupes stables jouant sur la durée,
absente pour un usage ponctuel.

**Condition de retour** : cette capacité est réexaminée si les groupes stables jouant sur la
durée représentent une part identifiable des utilisateurs et si le besoin de conserver des
notes d'une partie à l'autre est exprimé en usage réel.

---

<a id="6-vision-long-terme"></a>
## 6. Vision long terme

Ce palier décrit l'horizon du produit au-delà de la validation et de l'approfondissement du
cœur. Chaque capacité est présentée avec la valeur qu'elle apporterait et le risque de
positionnement qu'elle porte — la vision long terme de Haversack reste un outil d'organisation
et de pilotage narratif, pas un moteur de règles ni un assistant génératif, et chaque extension
de ce palier doit composer avec cette tension.

### Une préparation adaptée au système de jeu utilisé

Le produit pourrait proposer des modèles adaptés à un système de jeu donné, ajustant
automatiquement certains champs (fiche de personnage, structure de scénario, ressources). Cela
réduirait la configuration manuelle et faciliterait l'adoption. Le risque associé est une
dérive de complexité de contenu qui rapprocherait le produit d'un moteur de règles complet — ce
qui n'est pas le positionnement recherché.

### Des relations et des règles légères entre documents

Une fois les types de contenu adoptés (voir « Approfondissement du cœur »), une première
évolution permettrait de relier des documents entre eux de façon structurée et navigable — un
personnage appartient à une faction, un objet est porté par un personnage — avec une navigation
bidirectionnelle. Une évolution plus lointaine ajouterait un calcul léger fondé sur les
propriétés déclarées des documents liés — jamais un moteur de règles complet, un outil d'aide
qui reste au service du MJ.

**Un choix d'architecture pris dès le socle** : pour que cette évolution ne nécessite pas de
reconstruction majeure des données existantes le jour où elle serait engagée, le socle est déjà
conçu pour accueillir un type de document et des propriétés structurées en arrière-plan — une
décision de solidité technique prise en amont, précisément pour que cette trajectoire reste
ouverte sans coût de migration lourd.

### Une bibliothèque de réutilisation entre espaces

L'espace personnel du MJ, disponible dès le socle, pourrait devenir le point de départ d'une
bibliothèque : promouvoir un contenu vers un catalogue partageable, puis l'instancier en un
geste dans un autre espace. Cette capacité prolonge directement le contexte one-shot complet
(voir « Extensions déclenchées par signal ») et s'appuie sur une infrastructure déjà en place.

### Un assistant pour aider le MJ à préparer

Une aide fondée sur l'intelligence artificielle pourrait générer des idées, structurer un
scénario, esquisser un personnage non joueur, ou résumer une session. La valeur perçue en serait
forte. Le risque de positionnement l'est tout autant : le produit doit d'abord faire la preuve
de sa valeur comme outil d'organisation avant d'introduire une brique d'intelligence
artificielle qui pourrait en brouiller la lecture. Cette capacité reste une extension explicite,
pas le cœur du produit.

### Une ouverture communautaire

Les utilisateurs pourraient partager entre eux des modèles de campagne, de fiches ou d'aides de
jeu — un effet de réseau qui enrichirait le produit sans que tout soit développé en interne. Le
risque porte sur la modération et la qualité variable d'un catalogue ouvert, ce qui en fait un
horizon lointain plutôt qu'un chantier immédiat.

### Des horizons explicitement écartés du cœur du produit

Certaines directions, bien qu'envisagées, ne font pas partie de la trajectoire du produit et
sont écartées par choix de positionnement plutôt que par oubli :

- une table de jeu visuelle (cartes, pions, positions) placerait Haversack en concurrence
  frontale avec les outils de table virtuelle existants, sur un terrain qui n'est pas le sien ;
- une application de bureau avec synchronisation complète hors ligne entre plusieurs appareils
  représente une complexité technique disproportionnée par rapport au besoin réel — l'usage
  local sans compte du socle répond déjà à l'essentiel du besoin de démarrage sans engagement.

### La robustesse du modèle économique dans le temps

Le passage d'un palier payant à un palier gratuit (par exemple à la fin d'un abonnement) est
géré de façon réversible et sans perte de données : les espaces excédentaires par rapport à la
limite du palier gratuit deviennent temporairement inaccessibles en écriture, sans jamais être
supprimés, et redeviennent pleinement actifs dès que le palier payant est repris. Cette capacité
de cycle de vie accompagne la trajectoire de monétisation du produit sur la durée.

---

<a id="annexe-a--correspondance-avec-le-périmètre-de-conception"></a>
## Annexe A — Correspondance avec le périmètre de conception

Cette annexe trace chaque capacité mentionnée dans cette roadmap vers sa référence de conception
et sa catégorie de priorisation. C'est le seul endroit de ce document où les codes de use case et
le vocabulaire de priorisation interne apparaissent — la roadmap elle-même reste tournée vers la
valeur, pas vers l'outillage de conception.

### Le socle

| Capacité (roadmap) | Référence conception | Catégorie |
|---|---|---|
| Mode local sans compte | UC-01 | Must Have |
| Créer et configurer un espace (campagne ou one-shot) | UC-02 | Must Have |
| Structurer un scénario | UC-03 | Must Have |
| Gérer les documents d'un espace | UC-04 | Must Have |
| Organiser le contenu en dossiers libres | UC-05 (base) | Must Have |
| Vue de pilotage de session | UC-06 | Must Have |
| Création à la volée en session | UC-07 | Must Have |
| Partage d'information aux joueurs (par document, au groupe) | UC-08 | Must Have |
| Accès joueur sans compte | UC-09 | Must Have |
| Compte et synchronisation cloud | UC-10 | Must Have |
| Espace personnel — propriété et capture sans friction | Décision ADR-018 (propriété/capture uniquement — hors interface de bibliothèque) | Must Have |
| Quota du palier gratuit sur les espaces de type campagne/one-shot (espace personnel non décompté) | moscow.md — section « Quota FREE » (règle de monétisation) | Must Have |
| Export d'espace, version minimale | moscow.md — section « Export d'espace » | Must Have (promu de Should Have le 2026-06-25) |
| Instrumentation de validation (mesure anonyme, sans contenu narratif) | moscow.md — section « Instrumentation de validation du MVP » | Must Have |

### Le jalon de validation

| Élément (roadmap) | Référence conception | Nature |
|---|---|---|
| Cinq hypothèses de validation (H1 à H5) | vision-produit.md §2.3 | Point de décision go/no-go, non un use case |
| Progression local → compte gratuit → palier payant | vision-produit.md §3 | Modèle économique, valeurs volatiles |

### Approfondissement du cœur

| Capacité (roadmap) | Référence conception | Catégorie |
|---|---|---|
| Types de document prédéfinis (couche riche) | UC-05 (riche) | Should Have |
| Membres permanents et gestion des accès | UC-11 | Should Have |
| Vue joueur unifiée post-accès | UC-12 | Should Have |
| Recherche par mot-clé | UC-14 | Should Have (le filtrage par étiquette est un raffinement ultérieur, hors de ce palier) |

### Extensions déclenchées par signal

| Capacité (roadmap) | Référence conception | Catégorie / condition de retour |
|---|---|---|
| Contexte one-shot complet (scénario réutilisable, parcours express) | UC-13 | Should Have, explicitement hors première livraison — arbitrage du 2026-06-10 |
| Granularité de partage par joueur ou personnage | vision-produit.md §5bis — arbitrage « Granularité du partage » | Horizon vision, condition de retour du 2026-06-10 |
| Portabilité entrante (réimport de fichier de sauvegarde) | moscow.md — section « Réimport de fichier de sauvegarde » | Could Have |
| Types de document personnalisés | moscow.md — section « Types de document personnalisés » | Could Have |
| Notes personnelles joueur | moscow.md — section « Notes personnelles joueur » | Could Have |

### Vision long terme

| Capacité (roadmap) | Référence conception | Catégorie |
|---|---|---|
| Préparation adaptée au système de jeu (templates de système) | UC-F01 | Vision long terme |
| Relations typées entre documents (couche 1) | UC-F06 — couche 1 | Won't Have en MVP, vision long terme |
| Règles légères sur les propriétés (couche 2) | UC-F06 — couche 2 | Won't Have en MVP, vision long terme |
| Prérequis technique gravé dès le socle (`documentTypeId` / propriétés structurées) | vision-produit.md §6, moscow.md — Won't Have « Relations entre documents typés » | Choix d'architecture MVP |
| Bibliothèque de réutilisation inter-espaces (catalogue, promotion, instanciation) | UC-F07 (largement subsumé par ADR-018) — interface de bibliothèque | Could Have résiduel, post-MVP |
| Assistant IA pour le MJ | UC-F02 | Won't Have en MVP, vision long terme |
| Ouverture communautaire (templates communautaires) | UC-F05 | Won't Have en MVP, vision long terme |
| Table visuelle légère — écarté du cœur | UC-F03 | Won't Have |
| Application desktop avec synchronisation complète hors ligne — écarté du cœur | UC-F04 | Won't Have |
| Gel et dégel réversible des espaces au changement de palier | UC-15 | Post-MVP (spécifiés) — hors catégorisation MoSCoW du MVP, classement arbitré (voir `moscow.md` §UC-15) |

---

<a id="annexe-b--glossaire"></a>
## Annexe B — Glossaire

Définitions courtes et neutres des termes métier employés dans ce document, à l'attention d'un
lecteur non familier du jeu de rôle.

| Terme | Définition |
|---|---|
| **Jeu de rôle** | Activité de loisir collaborative où un groupe de joueurs incarne des personnages dans une histoire dirigée par un maître du jeu, selon les règles d'un système de jeu donné. |
| **Maître du jeu (MJ)** | La personne qui prépare et anime la partie : elle crée le contenu (scénario, personnages non joueurs, lieux), dirige la narration et décide de ce qui est partagé aux autres joueurs. |
| **Joueur** | Un participant qui rejoint une partie animée par un MJ, incarne un personnage et reçoit les informations que le MJ choisit de partager. |
| **Campagne** | Un contexte de jeu qui se déroule sur plusieurs sessions, avec un groupe stable et une continuité narrative d'une session à l'autre. |
| **One-shot** | Un contexte de jeu qui se déroule en une seule session, potentiellement avec des joueurs différents à chaque fois, sans continuité narrative attendue. |
| **Session** | Une séance de jeu — l'unité de temps pendant laquelle le groupe joue effectivement. |
| **Espace** | Le conteneur numérique qui regroupe et organise le contenu produit par le maître du jeu. Un espace prend l'une de trois formes — une campagne, un one-shot, ou l'espace personnel du MJ. Le contenu appartient à l'espace. |
| **Espace personnel** | La forme d'espace créée par défaut pour chaque MJ, disponible dès le premier usage (y compris en mode local), qui accueille le contenu créé sans qu'une campagne ou un one-shot soit nécessaire. |
| **Mode local** | Un usage du produit sans création de compte, où les données restent disponibles sur l'appareil utilisé. |
| **Vue de pilotage de session** | L'espace de travail dédié utilisé par le MJ pendant qu'une session se joue, regroupant l'accès aux scènes, aux notes et au contenu préparé. |
| **Socle** | Le premier palier de la trajectoire produit, livré comme un bloc unique et indivisible. |
| **Palier** | Une étape de la trajectoire de valeur du produit, regroupant un ensemble cohérent de capacités livrées ou envisagées. |
| **Scénario** | Le contenu narratif préparé par le MJ pour une session ou une campagne, structuré en scènes. |

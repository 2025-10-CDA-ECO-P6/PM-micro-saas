# Use Cases : Assistant MJ pour jeu de rôle

# 1. Contexte produit

Le projet vise à répondre à une problématique fréquente chez les maîtres du jeu : la dispersion des informations et des outils utilisés pour préparer et animer une campagne de jeu de rôle.

Aujourd’hui, un MJ peut utiliser plusieurs supports en parallèle : documents texte, feuilles papier, Discord, Google Docs, Notion, fichiers PDF, tableurs, notes personnelles ou outils de table virtuelle. Cette fragmentation entraîne une perte de temps, une charge mentale importante et une difficulté à retrouver rapidement l’information pertinente pendant une session.

Le produit proposé est un outil d’assistance au MJ, centré sur la préparation et le pilotage de session. Il ne cherche pas, dans son MVP, à remplacer une table virtuelle comme Roll20 ou Foundry VTT. Le cœur du produit est l’organisation narrative et opérationnelle du MJ.

---

# 2. Positionnement du MVP

## 2.1 Objectif principal

Permettre au MJ de préparer une campagne, structurer ses scénarios, centraliser ses notes et accéder rapidement aux informations importantes pendant une session.

## 2.2 Choix produit assumé

Le produit est centré sur le MJ comme utilisateur principal.

Ce choix est justifié par le fait que le MJ :

- porte la charge de préparation ;
- centralise les informations de campagne ;
- choisit généralement les outils utilisés par le groupe ;
- a la douleur utilisateur la plus forte ;
- représente l’utilisateur le plus susceptible de payer pour un outil SaaS dédié.

Les joueurs sont des utilisateurs secondaires. Leur expérience doit rester simple et fluide, mais le produit n’a pas vocation, dans son MVP, à devenir une plateforme complète centrée sur les joueurs.

## 2.3 Hors périmètre MVP

Les fonctionnalités suivantes sont exclues du MVP :

- table virtuelle visuelle ;
- cartes interactives ;
- gestion automatisée des combats ;
- moteur de règles avancé ;
- audio/vidéo ;
- marketplace ;
- système de plugins ;
- intelligence artificielle générative ;
- support complet de plusieurs systèmes de jeu ;
- collaboration temps réel complexe.

Ces éléments peuvent être envisagés dans une vision produit long terme, mais ils ne doivent pas déstabiliser le périmètre du MVP.

---

# 3. Acteurs

## 3.1 Maître du jeu — MJ

Le MJ est l’utilisateur principal du produit. Il crée et administre les campagnes, prépare les scénarios, crée les PNJ, prend des notes, gère les informations privées ou partagées, consulte les fiches personnages et pilote la session.

### Objectifs principaux

- Préparer une campagne plus efficacement.
- Structurer ses scénarios.
- Centraliser les informations importantes.
- Réduire la charge mentale pendant la session.
- Partager certaines informations aux joueurs sans exposer ses notes privées.

## 3.2 Joueur

Le joueur est un utilisateur secondaire. Il accède à sa fiche personnage, à son inventaire, à ses notes personnelles et aux informations partagées par le MJ.

### Objectifs principaux

- Accéder facilement à sa fiche personnage.
- Retrouver les informations partagées par le MJ.
- Gérer son inventaire et ses notes.
- Participer à une session sans configuration lourde.

## 3.3 Joueur invité sans compte

Le joueur invité rejoint une campagne ou une session via un lien ou un code, sans création de compte obligatoire.

### Objectifs principaux

- Rejoindre rapidement une session.
- Être associé à un personnage.
- Accéder aux informations nécessaires avec un minimum de friction.

---

# 4. Synthèse des use cases MVP

| ID    | Use Case                                  | Acteur principal | Priorité MVP |
| ----- | ----------------------------------------- | ---------------: | -----------: |
| UC-01 | Créer et configurer une campagne          |               MJ |     Critique |
| UC-02 | Structurer un scénario                    |               MJ |     Critique |
| UC-03 | Créer et gérer des PNJ                    |               MJ |  Très élevée |
| UC-04 | Gérer les notes MJ                        |               MJ |     Critique |
| UC-05 | Préparer une session de jeu               |               MJ |       Élevée |
| UC-06 | Utiliser la vue session                   |               MJ |     Critique |
| UC-07 | Créer et gérer une fiche personnage       |      MJ / Joueur |       Élevée |
| UC-08 | Gérer l’inventaire d’un personnage        |      Joueur / MJ |      Moyenne |
| UC-09 | Partager une information aux joueurs      |               MJ |       Élevée |
| UC-10 | Rejoindre une campagne ou une session     |           Joueur |       Élevée |
| UC-11 | Rechercher rapidement une information     |               MJ |       Élevée |
| UC-12 | Clôturer une session et préparer la suite |               MJ |      Moyenne |

---

# 5. Use Cases détaillés

---

# UC-01 — Créer et configurer une campagne

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Créer un espace centralisé permettant au MJ d’organiser une campagne de jeu de rôle.

## Contexte

Une campagne de jeu de rôle regroupe de nombreuses informations : scénarios, sessions, PNJ, personnages joueurs, notes, documents et éléments de lore. Sans espace centralisé, le MJ risque de disperser ses informations sur plusieurs supports.

## Besoin utilisateur

Le MJ veut disposer d’un espace unique pour organiser une campagne sans mélanger ses informations avec d’autres campagnes ou d’autres projets.

## Déclencheur

Le MJ souhaite commencer une nouvelle campagne ou migrer une campagne existante dans l’outil.

## Préconditions

- Le MJ dispose d’un compte utilisateur.
- Le MJ est authentifié.

## Scénario nominal

1. Le MJ accède à son tableau de bord.
2. Il clique sur l’action “Créer une campagne”.
3. Le système affiche un formulaire de création.
4. Le MJ renseigne les informations principales :
   - nom de la campagne ;
   - description courte ;
   - système de jeu utilisé ;
   - ambiance ou genre ;
   - statut initial de la campagne.

5. Le MJ valide la création.
6. Le système crée la campagne.
7. Le système redirige le MJ vers le tableau de bord de la campagne.
8. Le tableau de bord affiche les sections principales : scénarios, sessions, PNJ, joueurs, notes et informations partagées.

## Scénarios alternatifs

### A1 — Création en mode brouillon

Le MJ commence à remplir le formulaire mais ne dispose pas encore de toutes les informations. Il peut sauvegarder la campagne comme brouillon.

### A2 — Campagne sans système de jeu défini

Le MJ ne choisit aucun système de jeu. Le système crée alors la campagne en mode générique.

### A3 — Annulation de la création

Le MJ quitte le formulaire sans valider. Aucune campagne n’est créée, sauf si un brouillon automatique est prévu.

## Exceptions

### E1 — Nom de campagne manquant

Le système empêche la validation et indique que le nom est obligatoire.

### E2 — Erreur de création

Si une erreur survient lors de la sauvegarde, le système affiche un message d’erreur et conserve les données saisies.

## Postconditions

- Une campagne est créée.
- La campagne est associée au MJ propriétaire.
- Le MJ peut commencer à ajouter des scénarios, PNJ, notes et personnages.

## Données manipulées

### Campagne

- Identifiant unique
- Nom
- Description
- Système de jeu
- Statut
- Propriétaire
- Date de création

## Règles métier

- Une campagne appartient à un MJ propriétaire.
- Un MJ peut posséder plusieurs campagnes.
- Le nom de campagne est obligatoire.
- Le système de jeu est facultatif dans le MVP.
- Une campagne peut être archivée sans être supprimée définitivement.

## Critères d’acceptation

- Le MJ peut créer une campagne depuis son tableau de bord.
- Une campagne créée apparaît dans la liste des campagnes du MJ.
- Le MJ accède à un tableau de bord dédié à la campagne après création.
- Une campagne ne peut pas être créée sans nom.

## Questions à valider en interview

- Comment les MJ organisent-ils leurs campagnes ?
- Font-ils une séparation claire entre campagne, scénario et session ?
- Ont-ils plusieurs campagnes en parallèle ?
- Le choix du système de jeu est-il important dès la création ?

---

# UC-02 — Structurer un scénario

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Permettre au MJ de préparer et structurer un scénario exploitable pendant une session.

## Contexte

Le scénario est un élément central de la préparation du MJ. Il peut contenir des scènes, des objectifs narratifs, des PNJ, des lieux, des indices, des objets et des notes privées. Si ces informations sont mal organisées, le MJ peut perdre du temps ou oublier des éléments importants pendant la partie.

## Besoin utilisateur

Le MJ veut préparer un scénario de manière structurée tout en gardant une certaine liberté d’improvisation.

## Déclencheur

Le MJ prépare une future session ou un arc narratif.

## Préconditions

- Une campagne existe.
- Le MJ a les droits d’administration sur la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section “Scénarios”.
3. Il clique sur “Créer un scénario”.
4. Le système affiche un éditeur de scénario.
5. Le MJ renseigne les informations générales :
   - titre ;
   - résumé ;
   - contexte ;
   - objectif narratif ;
   - statut du scénario.

6. Le MJ ajoute une ou plusieurs scènes.
7. Pour chaque scène, le MJ peut renseigner :
   - titre ;
   - description ;
   - objectif ;
   - informations à révéler aux joueurs ;
   - notes privées ;
   - PNJ liés ;
   - lieux ou objets liés.

8. Le MJ sauvegarde le scénario.
9. Le scénario devient disponible dans la campagne et peut être utilisé dans une session.

## Scénarios alternatifs

### A1 — Scénario libre sans découpage en scènes

Le MJ peut créer un scénario sous forme de note structurée sans utiliser le découpage en scènes.

### A2 — Ajout d’éléments liés existants

Le MJ lie au scénario des PNJ, notes ou personnages déjà existants dans la campagne.

### A3 — Création d’un élément depuis le scénario

Le MJ crée un nouveau PNJ ou une nouvelle note directement depuis l’éditeur de scénario.

### A4 — Scénario improvisé

Le MJ crée un scénario minimal pendant ou juste avant une session, avec uniquement un titre et quelques notes.

## Exceptions

### E1 — Titre manquant

Le système empêche la création si le scénario n’a pas de titre.

### E2 — Perte de connexion ou erreur de sauvegarde

Le système conserve les données saisies localement si possible et affiche un message d’erreur.

## Postconditions

- Le scénario est sauvegardé dans la campagne.
- Le scénario peut être consulté, modifié ou lié à une session.
- Les éléments liés au scénario sont accessibles depuis celui-ci.

## Données manipulées

### Scénario

- Identifiant unique
- Titre
- Résumé
- Contexte
- Objectif narratif
- Statut
- Campagne associée
- Scènes
- Éléments liés

### Scène

- Identifiant unique
- Titre
- Description
- Objectif
- Ordre d’affichage
- Notes
- Informations partageables
- PNJ liés
- Lieux liés
- Objets liés

## Règles métier

- Un scénario appartient à une campagne.
- Un scénario peut contenir zéro, une ou plusieurs scènes.
- Une scène peut être liée à plusieurs PNJ.
- Les notes privées d’un scénario ne sont visibles que par le MJ.
- Un scénario peut être dans l’un des statuts suivants : brouillon, prêt, joué, archivé.

## Critères d’acceptation

- Le MJ peut créer un scénario dans une campagne.
- Le MJ peut ajouter, modifier et supprimer des scènes.
- Le MJ peut lier un PNJ à une scène.
- Le MJ peut sauvegarder un scénario comme brouillon.
- Le scénario peut être retrouvé depuis la campagne.

## Questions à valider en interview

- Les MJ structurent-ils leurs scénarios en scènes, actes, lieux ou événements ?
- Ont-ils besoin d’un éditeur très libre ou très guidé ?
- Quelles informations doivent absolument apparaître dans un scénario ?
- À quel moment préparent-ils leurs scénarios ?
- Utilisent-ils des templates ou des structures récurrentes ?

---

# UC-03 — Créer et gérer des PNJ

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Centraliser les informations des personnages non-joueurs afin de les retrouver rapidement pendant la préparation ou la session.

## Contexte

Les PNJ sont nombreux dans une campagne. Le MJ peut devoir retenir leur nom, leur rôle, leur personnalité, leurs motivations, leurs relations avec les joueurs et leurs secrets. Ces informations sont souvent dispersées dans les notes du MJ.

## Besoin utilisateur

Le MJ veut éviter d’oublier des informations importantes sur ses PNJ et pouvoir les consulter rapidement.

## Déclencheur

Le MJ prépare un scénario, enrichit sa campagne ou crée un PNJ improvisé pendant une session.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section “PNJ”.
3. Il clique sur “Créer un PNJ”.
4. Le système affiche une fiche PNJ.
5. Le MJ renseigne les informations principales :
   - nom ;
   - rôle ;
   - description ;
   - personnalité ;
   - motivation ;
   - statut ;
   - informations connues des joueurs ;
   - notes privées MJ.

6. Le MJ ajoute éventuellement des relations avec d’autres entités :
   - personnages joueurs ;
   - autres PNJ ;
   - lieux ;
   - scénarios ;
   - factions.

7. Le MJ sauvegarde la fiche PNJ.
8. Le PNJ devient accessible dans la campagne, la recherche et les scénarios liés.

## Scénarios alternatifs

### A1 — Création rapide

Le MJ crée un PNJ avec seulement un nom et une note courte, par exemple pendant une session.

### A2 — Transformation d’une note en PNJ

Le MJ transforme une note existante en fiche PNJ.

### A3 — Ajout d’un PNJ depuis un scénario

Le MJ crée un PNJ directement depuis une scène de scénario.

### A4 — Archivage d’un PNJ

Le MJ archive un PNJ qui n’est plus utilisé sans le supprimer définitivement.

## Exceptions

### E1 — Nom manquant

Le système empêche la création d’un PNJ sans nom ou génère un nom temporaire selon le comportement choisi.

### E2 — Suppression accidentelle

Le système demande confirmation avant suppression définitive ou privilégie l’archivage.

## Postconditions

- Le PNJ est enregistré dans la campagne.
- Le PNJ peut être retrouvé par recherche.
- Le PNJ peut être lié à un scénario, une scène ou une note.

## Données manipulées

### PNJ

- Identifiant unique
- Nom
- Alias
- Rôle
- Description
- Personnalité
- Motivation
- Statut
- Informations publiques
- Notes privées
- Relations
- Tags
- Campagne associée

## Règles métier

- Un PNJ appartient à une campagne.
- Un PNJ peut être lié à plusieurs scénarios ou scènes.
- Les notes privées d’un PNJ ne sont visibles que par le MJ.
- Les informations publiques peuvent être partagées aux joueurs si le MJ le décide.

## Critères d’acceptation

- Le MJ peut créer une fiche PNJ.
- Le MJ peut modifier une fiche PNJ.
- Le MJ peut retrouver un PNJ via la recherche.
- Le MJ peut lier un PNJ à un scénario ou une scène.
- Le MJ peut distinguer les informations privées des informations partageables.

## Questions à valider en interview

- Combien de PNJ les MJ suivent-ils en moyenne ?
- Quelles informations sont réellement utiles sur une fiche PNJ ?
- Les MJ ont-ils besoin de relations entre PNJ et personnages ?
- Ont-ils besoin de fiches très détaillées ou de fiches rapides ?
- Créent-ils souvent des PNJ improvisés ?

---

# UC-04 — Gérer les notes MJ

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, uniquement si certaines notes sont partagées.

## Objectif

Permettre au MJ de créer, organiser, retrouver et éventuellement partager ses notes de campagne.

## Contexte

La prise de notes est un besoin central pour un MJ. Les notes peuvent concerner des idées, des événements, des secrets, des décisions de joueurs, des éléments de lore ou des rappels pour une prochaine session.

## Besoin utilisateur

Le MJ veut centraliser ses notes et les relier au contexte approprié pour éviter de les perdre ou de les oublier.

## Déclencheur

Le MJ prépare une campagne, rédige un scénario, improvise une idée ou prend une note pendant une session.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section “Notes”.
3. Il clique sur “Créer une note”.
4. Le système affiche un éditeur de note.
5. Le MJ renseigne :
   - titre ;
   - contenu ;
   - type de note ;
   - visibilité ;
   - tags ;
   - éléments liés.

6. Par défaut, la note est privée.
7. Le MJ peut lier la note à un scénario, une scène, un PNJ, un personnage ou une session.
8. Le MJ sauvegarde la note.
9. La note devient accessible depuis la liste des notes, la recherche et les entités liées.

## Scénarios alternatifs

### A1 — Note rapide

Le MJ crée une note rapide sans titre. Le système génère un titre temporaire à partir de la date ou du début du contenu.

### A2 — Note partagée

Le MJ définit la note comme visible par tous les joueurs ou par certains joueurs uniquement.

### A3 — Changement de visibilité

Le MJ transforme une note privée en note partagée ou inversement.

### A4 — Note prise pendant une session

Le MJ crée une note depuis la vue session. Elle est automatiquement liée à la session en cours.

## Exceptions

### E1 — Contenu vide

Le système peut empêcher la création d’une note vide ou autoriser une note vide comme brouillon.

### E2 — Accès joueur non autorisé

Un joueur tente d’accéder à une note privée. Le système refuse l’accès.

## Postconditions

- La note est enregistrée.
- La note est liée à la campagne.
- La note respecte sa configuration de visibilité.

## Données manipulées

### Note

- Identifiant unique
- Titre
- Contenu
- Type
- Visibilité
- Tags
- Entités liées
- Auteur
- Date de création
- Date de modification

## Règles métier

- Toute note est privée par défaut.
- Seul le MJ peut partager une note.
- Une note peut être liée à plusieurs entités.
- Un joueur ne peut consulter et voire que les notes qui lui sont explicitement accessibles.

## Critères d’acceptation

- Le MJ peut créer une note privée.
- Le MJ peut créer une note partagée.
- Le MJ peut modifier la visibilité d’une note.
- Une note privée n’est pas visible par les joueurs.
- Une note peut être retrouvée via la recherche.

## Questions à valider en interview

- Comment les MJ prennent-ils leurs notes aujourd’hui ?
- Les notes sont-elles plutôt longues, courtes, structurées ou libres ?
- Les MJ ont-ils besoin de partager certaines notes ?
- Comment gèrent-ils les secrets et les informations connues des joueurs ?
- Quelles notes doivent être retrouvées rapidement en session ?

---

# UC-05 — Préparer une session de jeu

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si le MJ associe des participants à la session.

## Objectif

Préparer une session précise en sélectionnant les éléments de campagne utiles pour cette partie.

## Contexte

Une campagne peut contenir beaucoup d’informations. Pour une session donnée, le MJ a besoin d’un espace focalisé sur ce qui sera utilisé : scénario prévu, scènes importantes, PNJ présents, personnages joueurs, notes critiques.

## Besoin utilisateur

Le MJ veut préparer une session sans devoir parcourir toute sa campagne au moment de jouer.

## Déclencheur

Une partie est prévue à une date donnée.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.
- Des éléments de campagne peuvent déjà exister, mais ce n’est pas obligatoire.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section “Sessions”.
3. Il clique sur “Créer une session”.
4. Il renseigne :
   - titre ;
   - date prévue ;
   - scénario associé ;
   - résumé d’intention.

5. Il sélectionne les éléments utiles :
   - scènes prévues ;
   - PNJ importants ;
   - notes nécessaires ;
   - personnages participants.

6. Le système prépare une vue session à partir de ces éléments.
7. Le MJ sauvegarde la session.

## Scénarios alternatifs

### A1 — Session sans scénario

Le MJ crée une session libre, sans scénario associé.

### A2 — Session improvisée

Le MJ crée une session au dernier moment avec très peu d’informations.

### A3 — Ajout d’éléments après création

Le MJ complète la session progressivement avant la partie.

### A4 — Duplication d’une session précédente

Le MJ duplique une session passée comme base de préparation.

## Exceptions

### E1 — Session sans titre

Le système demande un titre ou génère un titre automatique basé sur la date.

### E2 — Scénario supprimé

Si le scénario lié est supprimé ou archivé, le système informe le MJ et conserve la session sans scénario actif.

## Postconditions

- Une session est créée.
- La session est liée à une campagne.
- La session peut être lancée en mode session.

## Données manipulées

### Session

- Identifiant unique
- Titre
- Date prévue
- Statut
- Scénario associé
- Participants
- PNJ sélectionnés
- Notes sélectionnées
- Résumé d’intention

## Règles métier

- Une session appartient à une campagne.
- Une session peut être liée à zéro ou un scénario principal.
- Une session peut avoir plusieurs participants.
- Une session peut passer par les statuts : prévue, en cours, terminée, archivée.

## Critères d’acceptation

- Le MJ peut créer une session depuis une campagne.
- Le MJ peut associer un scénario à une session.
- Le MJ peut sélectionner des PNJ et notes utiles.
- Une session créée peut être lancée en vue session.

## Questions à valider en interview

- Les MJ préparent-ils session par session ?
- Ont-ils besoin d’une checklist avant partie ?
- Préfèrent-ils une préparation très structurée ou libre ?
- Quels éléments veulent-ils avoir sous les yeux au moment de lancer une partie ?

---

# UC-06 — Utiliser la vue session

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si le MJ partage des informations pendant la session.

## Objectif

Permettre au MJ d’accéder rapidement aux informations importantes pendant une partie.

## Contexte

Pendant une session, le MJ doit maintenir le rythme. Il peut devoir retrouver un PNJ, une note, une scène, une fiche personnage ou une information de campagne. Si l’information est trop difficile à trouver, la partie ralentit.

## Besoin utilisateur

Le MJ veut piloter sa session avec une interface simple et rapide, sans devoir fouiller dans tous ses documents.

## Déclencheur

La partie commence.

## Préconditions

- Une campagne existe.
- Une session existe ou peut être lancée rapidement.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il sélectionne une session prévue.
3. Il clique sur “Lancer la session”.
4. Le système affiche une vue session simplifiée.
5. La vue session présente :
   - scénario actif ;
   - scènes prévues ;
   - PNJ importants ;
   - personnages joueurs ;
   - notes utiles ;
   - zone de note rapide ;
   - barre de recherche.

6. Le MJ consulte les éléments nécessaires pendant la partie.
7. Le MJ ajoute des notes rapides au fil de la session.
8. Le MJ marque éventuellement certaines scènes comme jouées.
9. Le MJ peut partager une information aux joueurs.
10. En fin de partie, le MJ sauvegarde ou clôture la session.

## Scénarios alternatifs

### A1 — Lancement sans session préparée

Le MJ lance une session rapide directement depuis une campagne.

### A2 — Création d’un élément en session

Le MJ crée rapidement un PNJ ou une note pendant la session.

### A3 — Recherche d’un élément non prévu

Le MJ utilise la recherche pour ouvrir un élément qui n’avait pas été préparé pour la session.

### A4 — Partage en direct

Le MJ rend une note ou une information visible aux joueurs pendant la partie.

## Exceptions

### E1 — Problème de sauvegarde

Si une note ne peut pas être sauvegardée, le système avertit le MJ et conserve localement le contenu si possible.

### E2 — Accès interdit

Un utilisateur non MJ tente d’accéder à la vue session MJ. Le système refuse l’accès.

## Postconditions

- Les notes prises pendant la session sont sauvegardées.
- Les éléments créés sont liés à la campagne.
- Les scènes marquées comme jouées conservent leur statut.
- La session peut être clôturée ou reprise plus tard.

## Données manipulées

- Session
- Scénario
- Scène
- PNJ
- Personnage joueur
- Note
- Information partagée

## Règles métier

- La vue session est réservée au MJ.
- Les notes rapides créées depuis la vue session sont automatiquement liées à la session en cours.
- Les éléments privés restent invisibles pour les joueurs.
- La vue session doit prioriser la rapidité d’accès à l’information plutôt que l’édition avancée.

## Critères d’acceptation

- Le MJ peut lancer une vue session.
- Le MJ peut consulter le scénario actif.
- Le MJ peut consulter les PNJ liés.
- Le MJ peut consulter les fiches personnages.
- Le MJ peut créer une note rapide.
- Le MJ peut rechercher une information.
- Les notes créées en session sont sauvegardées.

## Questions à valider en interview

- Quelles informations les MJ cherchent-ils le plus souvent pendant une session ?
- Qu’est-ce qui casse le rythme d’une partie ?
- Les MJ ont-ils besoin d’un mode session séparé du mode préparation ?
- Quelle interface serait trop lourde en pleine partie ?

---

# UC-07 — Créer et gérer une fiche personnage

## Acteurs principaux

MJ et joueur

## Acteurs secondaires

Aucun.

## Objectif

Centraliser les fiches personnages pour permettre au joueur et au MJ d’y accéder facilement.

## Contexte

Les fiches personnages sont parfois conservées sur papier, en PDF, en image ou dans des documents séparés. Elles peuvent être oubliées, perdues ou difficiles à consulter rapidement par le MJ.

## Besoin utilisateur

Le joueur veut accéder facilement à sa fiche. Le MJ veut pouvoir consulter rapidement les informations utiles des personnages pendant la session.

## Déclencheur

Un joueur rejoint une campagne ou un personnage doit être créé.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal côté MJ

1. Le MJ ouvre une campagne.
2. Il accède à la section “Personnages”.
3. Il clique sur “Créer un personnage”.
4. Il renseigne :
   - nom du personnage ;
   - joueur associé ;
   - description ;
   - caractéristiques principales ;
   - notes MJ éventuelles.

5. Le MJ sauvegarde la fiche.
6. La fiche devient consultable depuis la campagne et la vue session.

## Scénario nominal côté joueur

1. Le joueur accède à son espace campagne ou à un lien fourni par le MJ.
2. Il ouvre sa fiche personnage.
3. Il consulte les informations.
4. Il modifie les champs autorisés :
   - notes personnelles ;
   - inventaire ;
   - informations libres selon permissions.

5. Les modifications sont sauvegardées.

## Scénarios alternatifs

### A1 — Personnage sans joueur associé

Le MJ crée une fiche personnage avant que le joueur ne rejoigne.

### A2 — Joueur invité sans compte

Le joueur accède à sa fiche via un lien temporaire ou une session.

### A3 — Fiche générique

Le personnage est créé avec une structure générique non dépendante d’un système de jeu précis.

### A4 — Verrouillage de certains champs

Le MJ verrouille des champs que le joueur ne peut pas modifier.

## Exceptions

### E1 — Accès non autorisé

Un joueur tente d’accéder à la fiche d’un autre joueur sans autorisation. Le système refuse l’accès.

### E2 — Modification interdite

Le joueur tente de modifier un champ verrouillé. Le système bloque la modification.

## Postconditions

- Une fiche personnage existe dans la campagne.
- Le MJ peut la consulter.
- Le joueur associé peut y accéder selon les permissions.

## Données manipulées

### Personnage

- Identifiant unique
- Nom
- Description
- Joueur associé
- Caractéristiques génériques
- Ressources simples
- Inventaire
- Notes joueur
- Notes privées MJ
- Campagne associée

## Règles métier

- Un personnage appartient à une campagne.
- Un personnage peut être associé à zéro ou un joueur.
- Le MJ peut consulter toutes les fiches personnages de sa campagne.
- Un joueur ne peut consulter que les fiches auxquelles il a accès.
- Certains champs peuvent être modifiables uniquement par le MJ.

## Critères d’acceptation

- Le MJ peut créer une fiche personnage.
- Le MJ peut associer une fiche à un joueur.
- Le joueur peut consulter sa fiche.
- Le joueur peut modifier les champs autorisés.
- Le MJ peut consulter la fiche depuis la vue session.

## Questions à valider en interview

- Les MJ ont-ils besoin de consulter les fiches joueurs pendant la partie ?
- Les joueurs accepteraient-ils d’utiliser une fiche dans l’outil ?
- Une fiche générique suffit-elle pour un MVP ?
- Quelles informations sont indispensables sur une fiche ?

---

# UC-08 — Gérer l’inventaire d’un personnage

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre au joueur de suivre les objets de son personnage et au MJ de les consulter si nécessaire.

## Contexte

L’inventaire est une information fréquente en jeu de rôle. Il peut être oublié ou mal synchronisé entre le joueur et le MJ, surtout quand la campagne dure longtemps.

## Besoin utilisateur

Le joueur veut suivre ses objets facilement. Le MJ veut pouvoir vérifier rapidement l’inventaire d’un personnage.

## Déclencheur

Le personnage obtient, perd, utilise ou modifie un objet.

## Préconditions

- Une fiche personnage existe.
- Le joueur a accès à la fiche ou le MJ administre la campagne.

## Scénario nominal

1. Le joueur ouvre sa fiche personnage.
2. Il accède à la section “Inventaire”.
3. Il ajoute un objet avec :
   - nom ;
   - quantité ;
   - description courte ;
   - note éventuelle.

4. Il sauvegarde l’objet.
5. L’objet apparaît dans son inventaire.
6. Le MJ peut consulter l’inventaire depuis la fiche du personnage.

## Scénarios alternatifs

### A1 — Modification d’un objet

Le joueur modifie la quantité ou la description d’un objet.

### A2 — Suppression d’un objet

Le joueur retire un objet de son inventaire.

### A3 — Ajout par le MJ

Le MJ ajoute directement un objet dans l’inventaire d’un personnage.

### A4 — Objet secret

Le MJ crée un objet non visible par le joueur. Cette option est plutôt hors MVP ou à traiter simplement.

## Exceptions

### E1 — Quantité invalide

Le système refuse une quantité négative si la quantité est renseignée.

### E2 — Accès interdit

Un joueur tente de modifier l’inventaire d’un autre joueur. Le système refuse.

## Postconditions

- L’inventaire du personnage est mis à jour.
- Les modifications sont visibles par les utilisateurs autorisés.

## Données manipulées

### Objet d’inventaire

- Identifiant unique
- Nom
- Quantité
- Description
- Note
- Personnage associé

## Règles métier

- Un objet d’inventaire appartient à un personnage.
- Le joueur peut modifier son propre inventaire si le MJ l’autorise.
- Le MJ peut consulter et modifier les inventaires des personnages de sa campagne.

## Critères d’acceptation

- Le joueur peut ajouter un objet à son inventaire.
- Le joueur peut modifier un objet existant.
- Le joueur peut supprimer un objet.
- Le MJ peut consulter l’inventaire depuis la fiche personnage.

## Questions à valider en interview

- L’inventaire est-il souvent source de friction ?
- Les MJ veulent-ils valider les modifications d’inventaire ?
- Les joueurs gèrent-ils leur inventaire sérieusement ?
- L’inventaire est-il indispensable au MVP ?

---

# UC-09 — Partager une information aux joueurs

## Acteur principal

MJ

## Acteurs secondaires

Joueurs

## Objectif

Permettre au MJ de partager certaines informations avec les joueurs tout en conservant ses notes privées.

## Contexte

Dans une campagne, certaines informations doivent être transmises aux joueurs : résumés, indices, documents, lore, rappels ou notes communes. D’autres informations doivent rester secrètes.

## Besoin utilisateur

Le MJ veut contrôler précisément ce qui est visible ou non par les joueurs.

## Déclencheur

Le MJ souhaite transmettre une information au groupe ou à certains joueurs.

## Préconditions

- Une campagne existe.
- Des joueurs ou personnages sont associés à la campagne.
- Le contenu à partager existe ou est créé par le MJ.

## Scénario nominal

1. Le MJ ouvre une note, un document ou une information de campagne.
2. Il choisit l’action “Partager”.
3. Le système affiche les options de visibilité.
4. Le MJ choisit la cible :
   - tous les joueurs ;
   - certains joueurs ;
   - certains personnages ;
   - session actuelle.

5. Le MJ valide le partage.
6. Le système rend l’information accessible aux utilisateurs concernés.
7. Les joueurs voient l’information dans leur espace.

## Scénarios alternatifs

### A1 — Retirer le partage

Le MJ rend à nouveau une information privée.

### A2 — Partage à un seul joueur

Le MJ partage une information uniquement à un joueur précis.

### A3 — Partage depuis la vue session

Le MJ partage une information pendant une partie.

### A4 — Partage d’un résumé de session

Le MJ partage un résumé après la clôture d’une session.

## Exceptions

### E1 — Aucun destinataire sélectionné

Le système empêche la validation si aucune cible n’est sélectionnée.

### E2 — Joueur supprimé ou non disponible

Si un joueur n’est plus associé à la campagne, il n’apparaît plus comme cible de partage.

## Postconditions

- L’information est visible par les joueurs ciblés.
- Les notes privées restent protégées.
- Le MJ peut modifier la visibilité ultérieurement.

## Données manipulées

### Information partagée

- Identifiant du contenu
- Type de contenu
- Visibilité
- Cibles
- Date de partage
- Auteur du partage

## Règles métier

- Tout contenu est privé par défaut.
- Seul le MJ peut partager une information de campagne.
- Un joueur ne peut consulter que les informations explicitement partagées avec lui.
- Le MJ peut retirer un partage.

## Critères d’acceptation

- Le MJ peut partager une note avec tous les joueurs.
- Le MJ peut partager une note avec un joueur spécifique.
- Le MJ peut retirer un partage.
- Un joueur ne voit pas les notes privées.
- Les joueurs voient les informations partagées dans leur espace.

## Questions à valider en interview

- Les MJ partagent-ils souvent des documents ou notes aux joueurs ?
- Ont-ils besoin d’un partage par joueur ou seulement par groupe ?
- Ont-ils déjà eu des problèmes de spoilers ou d’informations révélées trop tôt ?
- Quels types d’informations sont généralement partagés ?

---

# UC-10 — Rejoindre une campagne ou une session

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre à un joueur de rejoindre facilement une campagne ou une session, avec ou sans compte.

## Contexte

Le produit étant choisi par le MJ, l’expérience joueur doit être peu contraignante. Une obligation de création de compte peut devenir un frein à l’adoption.

## Besoin utilisateur

Le joueur veut accéder rapidement à la campagne, à sa fiche et aux informations utiles sans configuration complexe.

## Déclencheur

Le MJ invite un joueur à rejoindre une campagne ou une session.

## Préconditions

- Une campagne existe.
- Le MJ a généré un lien ou un code d’invitation.

## Scénario nominal sans compte

1. Le MJ génère un lien d’invitation.
2. Le joueur ouvre le lien.
3. Le système affiche une page de rejoindre.
4. Le joueur saisit un pseudo.
5. Le joueur rejoint la campagne ou la session comme invité.
6. Le MJ peut associer le joueur à un personnage existant.
7. Le joueur accède à sa fiche et aux informations partagées.

## Scénario nominal avec compte

1. Le joueur ouvre le lien d’invitation.
2. Il se connecte ou crée un compte.
3. Il rejoint la campagne.
4. Le MJ l’associe à un personnage.
5. L’accès est conservé durablement sur son compte.

## Scénarios alternatifs

### A1 — Lien expiré

Le joueur ouvre un lien expiré. Le système affiche un message d’erreur et demande un nouveau lien.

### A2 — Joueur en attente de validation

Le joueur demande à rejoindre la campagne, mais le MJ doit valider son accès.

### A3 — Accès direct à une session

Le joueur rejoint uniquement la session en cours, sans accès complet à la campagne.

## Exceptions

### E1 — Campagne introuvable

Le lien ne correspond à aucune campagne active.

### E2 — Accès refusé par le MJ

Le MJ refuse ou retire l’accès du joueur.

## Postconditions

- Le joueur est associé à la campagne ou à la session.
- Le joueur peut accéder aux informations autorisées.
- Le MJ peut gérer son association à un personnage.

## Données manipulées

### Invitation

- Identifiant unique
- Code ou token
- Campagne associée
- Date d’expiration
- Type d’accès
- Statut

### Participant

- Pseudo
- Compte éventuel
- Rôle
- Personnage associé

## Règles métier

- Le MJ contrôle les invitations.
- Un joueur invité sans compte a un accès limité ou temporaire.
- Un compte joueur permet un accès persistant.
- Le joueur ne voit que les informations partagées avec lui.

## Critères d’acceptation

- Le MJ peut générer un lien d’invitation.
- Un joueur peut rejoindre avec un pseudo sans compte.
- Le MJ peut associer un joueur à un personnage.
- Un joueur invité peut consulter sa fiche et les notes partagées.
- Un lien expiré ne permet pas l’accès.

## Questions à valider en interview

- La création de compte est-elle un frein pour les joueurs ?
- Les MJ préfèrent-ils inviter par lien, code ou email ?
- Les joueurs doivent-ils accéder à toute la campagne ou seulement à la session ?
- Le MJ veut-il valider les entrées manuellement ?

---

# UC-11 — Rechercher rapidement une information

## Acteur principal

MJ

## Acteurs secondaires

Joueur, dans une version limitée de la recherche.

## Objectif

Permettre au MJ de retrouver rapidement une information dans sa campagne.

## Contexte

Même avec une bonne organisation, certaines informations peuvent être difficiles à retrouver pendant une session. La recherche est une fonctionnalité importante pour éviter les interruptions et maintenir le rythme de jeu.

## Besoin utilisateur

Le MJ veut trouver rapidement un PNJ, une note, une scène, un personnage ou un élément de campagne.

## Déclencheur

Le MJ cherche une information pendant la préparation ou pendant une session.

## Préconditions

- Une campagne existe.
- La campagne contient des données recherchables.

## Scénario nominal

1. Le MJ utilise la barre de recherche.
2. Il saisit un mot-clé.
3. Le système affiche les résultats correspondants.
4. Les résultats sont regroupés par type :
   - PNJ ;
   - scénarios ;
   - scènes ;
   - notes ;
   - personnages ;
   - objets.

5. Le MJ sélectionne un résultat.
6. Le système ouvre l’élément correspondant.
7. Si le MJ est en vue session, l’élément s’ouvre sans casser le contexte de session autant que possible.

## Scénarios alternatifs

### A1 — Aucun résultat

Le système affiche un état vide avec une suggestion de création ou de modification de recherche.

### A2 — Résultats nombreux

Le système permet de filtrer par type d’entité.

### A3 — Recherche par tag

Le MJ filtre les résultats par tag ou catégorie.

### A4 — Recherche joueur

Le joueur utilise une recherche limitée aux informations visibles pour lui.

## Exceptions

### E1 — Accès non autorisé

Un joueur tente de rechercher une note privée. Le système ne retourne pas ce résultat.

### E2 — Erreur d’indexation

Si la recherche échoue, le système affiche un message d’erreur et propose une navigation manuelle.

## Postconditions

- Le MJ accède à l’information recherchée.
- Les règles de visibilité sont respectées.

## Données manipulées

### Résultat de recherche

- Identifiant
- Type d’entité
- Titre
- Extrait
- Campagne associée
- Visibilité

## Règles métier

- La recherche du MJ inclut les éléments privés et partagés de sa campagne.
- La recherche joueur n’inclut que les éléments visibles par ce joueur.
- Les résultats doivent être limités au contexte de la campagne active.

## Critères d’acceptation

- Le MJ peut rechercher une information dans une campagne.
- Les résultats sont classés par type.
- Le MJ peut ouvrir un résultat.
- Les notes privées ne sont pas visibles dans la recherche joueur.
- La recherche fonctionne depuis la vue session.

## Questions à valider en interview

- Les MJ utilisent-ils beaucoup la recherche dans leurs outils actuels ?
- Cherchent-ils plutôt par nom, tag, date, type ou contenu ?
- Quelles informations doivent être accessibles en moins de quelques secondes ?
- Une recherche globale est-elle plus utile qu’une navigation structurée ?

---

# UC-12 — Clôturer une session et préparer la suite

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si un résumé est partagé.

## Objectif

Conserver une trace de la session jouée et préparer la continuité narrative.

## Contexte

Après une session, le MJ doit souvent se souvenir des décisions prises, des PNJ rencontrés, des objets obtenus, des conséquences narratives et des éléments à préparer pour la prochaine partie.

## Besoin utilisateur

Le MJ veut éviter de perdre les informations importantes entre deux sessions.

## Déclencheur

Une session se termine.

## Préconditions

- Une session existe.
- Le MJ est dans la campagne concernée.

## Scénario nominal

1. Le MJ termine une session depuis la vue session.
2. Le système propose une étape de clôture.
3. Le MJ rédige un résumé de session.
4. Il ajoute les éléments importants :
   - décisions des joueurs ;
   - PNJ rencontrés ;
   - objets obtenus ;
   - conséquences ;
   - pistes pour la suite.

5. Le MJ choisit si le résumé reste privé ou devient partagé.
6. Le MJ valide la clôture.
7. La session passe au statut “terminée”.
8. Le résumé est sauvegardé dans la campagne.

## Scénarios alternatifs

### A1 — Clôture sans résumé

Le MJ termine la session sans rédiger de résumé.

### A2 — Résumé ajouté plus tard

Le MJ revient sur une session terminée pour compléter le résumé.

### A3 — Version privée et version partagée

Le MJ garde une version privée détaillée et partage une version simplifiée aux joueurs.

### A4 — Création automatique de notes de suivi

Certaines informations du résumé peuvent être transformées en notes ou tâches de préparation. Cette fonctionnalité est plutôt hors MVP.

## Exceptions

### E1 — Clôture accidentelle

Le MJ peut rouvrir une session terminée ou modifier son résumé.

### E2 — Erreur de sauvegarde

Le système avertit le MJ et conserve le contenu saisi si possible.

## Postconditions

- La session est marquée comme terminée.
- Un résumé peut être consulté dans l’historique de campagne.
- Le résumé peut être partagé aux joueurs selon la visibilité choisie.

## Données manipulées

### Résumé de session

- Identifiant unique
- Session associée
- Contenu
- Éléments liés
- Visibilité
- Date de création
- Date de modification

## Règles métier

- Une session terminée reste consultable.
- Le MJ peut modifier un résumé après clôture.
- Le résumé est privé par défaut.
- Le MJ choisit explicitement ce qui est partagé.

## Critères d’acceptation

- Le MJ peut clôturer une session.
- Le MJ peut rédiger un résumé.
- Le résumé est sauvegardé.
- Le MJ peut choisir de partager ou non le résumé.
- Une session terminée apparaît dans l’historique.

## Questions à valider en interview

- Les MJ font-ils des résumés après session ?
- Les résumés sont-ils destinés au MJ, aux joueurs ou aux deux ?
- Quelles informations sont importantes à conserver ?
- Les MJ veulent-ils préparer la prochaine session à partir du résumé ?

---

# 6. Use cases hors MVP pour la vision produit

Les use cases suivants ne sont pas prévus dans le MVP, mais peuvent guider l’architecture et la vision long terme.

## UC-F01 — Utiliser des templates de système de jeu

Le MJ choisit un système de jeu ou un template. Le produit adapte certains champs : fiche personnage, fiche PNJ, ressources, structure de scénario.

### Intérêt

- Améliore l’adoption.
- Réduit la configuration manuelle.
- Rend l’outil plus adapté aux usages réels.

### Risque

- Forte complexité car chaque système de jeu a ses propres règles.
- Risque de transformer le projet en moteur de règles.

### Position recommandée

Prévoir une fiche générique dans le MVP, puis ajouter des templates configurables plus tard.

---

## UC-F02 — Assistant IA pour le MJ

Le MJ utilise une aide IA pour générer des idées, structurer un scénario, créer un PNJ, résumer une session ou proposer des pistes d’improvisation.

### Intérêt

- Forte valeur perçue.
- Cohérent avec le besoin d’aide à la préparation.
- Peut différencier le produit à moyen terme.

### Risque

- Peut brouiller le positionnement du MVP.
- Risque de construire un produit “IA” au lieu d’un outil d’organisation.

### Position recommandée

L’IA doit rester une extension de l’assistant MJ, pas le cœur du MVP.

---

## UC-F03 — Table visuelle légère

Le produit propose une table visuelle pour représenter des cartes, positions ou scènes.

### Intérêt

- Utile pour les sessions en ligne.
- Peut compléter l’expérience de session.

### Risque

- Concurrence directe avec Roll20 et Foundry.
- Complexité technique élevée.
- Peut détourner le produit de son cœur : l’aide au MJ.

### Position recommandée

À considérer uniquement après validation du socle MJ.

---

## UC-F04 — Version desktop ou local-first

Le MJ utilise l’application en local, potentiellement via une version desktop, avec synchronisation optionnelle.

### Intérêt

- Répond au besoin d’usage hors ligne.
- Pertinent pour les sessions physiques.
- Peut rassurer les utilisateurs sur la possession de leurs données.

### Risque

- Complexifie l’architecture.
- Nécessite une stratégie de synchronisation.

### Position recommandée

Ne pas développer dans le MVP, mais éviter une architecture trop dépendante du cloud si cette piste est importante à long terme.

---

## UC-F05 — Templates communautaires

Les utilisateurs peuvent créer, partager ou installer des templates de campagne, fiches ou aides de jeu.

### Intérêt

- Crée un effet communautaire.
- Peut enrichir le produit sans tout développer en interne.
- Peut soutenir une stratégie freemium ou premium.

### Risque

- Modération.
- Qualité variable.
- Complexité de gestion d’un catalogue.

### Position recommandée

Vision long terme uniquement.

---

# 7. Priorisation fonctionnelle

## Must Have — MVP strict

- Création de compte MJ.
- Création de campagne.
- Création de scénario.
- Création de PNJ.
- Notes privées.
- Notes partagées.
- Vue session.
- Recherche dans une campagne.

## Should Have — MVP étendu

- Fiches personnages simples.
- Inventaire simple.
- Invitation joueur par lien.
- Accès joueur sans compte.
- Résumé de session.

## Could Have — Après MVP

- Tags avancés.
- Templates génériques.
- Partage par joueur spécifique.
- Journal de campagne.
- Export PDF.

## Won’t Have — Hors MVP

- Table virtuelle.
- Cartes interactives.
- IA générative.
- Plugins.
- Marketplace.
- Audio/vidéo.
- Moteur de règles complet.

---

# 8. Écrans principaux à wireframer

## Écran 1 — Tableau de bord MJ

Objectif : permettre au MJ de voir ses campagnes et d’en créer une nouvelle.

Contenu :

- liste des campagnes ;
- accès rapide à la dernière campagne ;
- bouton de création ;
- statuts des campagnes.

## Écran 2 — Tableau de bord campagne

Objectif : centraliser les grandes sections de la campagne.

Contenu :

- scénarios ;
- sessions ;
- PNJ ;
- personnages ;
- notes ;
- informations partagées.

## Écran 3 — Éditeur de scénario

Objectif : rédiger et structurer un scénario.

Contenu :

- résumé ;
- scènes ;
- notes privées ;
- éléments liés.

## Écran 4 — Fiche PNJ

Objectif : consulter et modifier un PNJ.

Contenu :

- identité ;
- rôle ;
- motivation ;
- relations ;
- notes privées ;
- informations partageables.

## Écran 5 — Notes

Objectif : gérer les notes privées ou partagées.

Contenu :

- liste de notes ;
- filtres ;
- éditeur ;
- visibilité ;
- éléments liés.

## Écran 6 — Vue session

Objectif : piloter la partie avec accès rapide aux informations.

Contenu :

- scénario actif ;
- scènes ;
- PNJ importants ;
- fiches joueurs ;
- notes rapides ;
- recherche.

## Écran 7 — Vue joueur

Objectif : permettre au joueur d’accéder aux informations utiles.

Contenu :

- fiche personnage ;
- inventaire ;
- notes personnelles ;
- notes partagées.

## Écran 8 — Invitation joueur

Objectif : permettre l’accès à une campagne ou session.

Contenu :

- lien ou code ;
- pseudo ;
- association à un personnage ;
- accès sans compte ou avec compte.

---

# 10. Conclusion

Le MVP doit rester centré sur un problème principal : aider le MJ à préparer et conduire ses sessions avec moins de friction et moins de dispersion d’informations.

La valeur principale du produit ne repose pas sur une table virtuelle, un moteur de règles ou une intelligence artificielle, mais sur la capacité à centraliser, structurer et rendre accessible l’information utile au bon moment.

# Haversack — Diagrammes de cas d'utilisation

> Ces diagrammes couvrent le périmètre fonctionnel MVP défini dans les Use Cases.
> Organisés du plus synthétique au plus détaillé.
>
> **Note sur les acteurs** : `MJ` et `Joueur` sont des **rôles contextuels au sein d'une campagne**,
> pas des types d'utilisateurs globaux. Un même utilisateur peut être MJ d'une campagne et joueur
> dans une autre. Tout utilisateur authentifié peut créer une campagne et endosser le rôle MJ.
> `Joueur invité` est un cas technique distinct : accès temporaire sans compte (`GuestAccess`).

---

## Légende

| Notation | Signification |
|---|---|
| Rectangle `[ ]` | Acteur |
| Ovale `([ ])` | Cas d'utilisation |
| `-->` | Association acteur vers cas d'utilisation |
| `-.->` `include` | Inclusion — le cas inclus est toujours exécuté |
| `-.->` `extend` | Extension — le cas étendu est exécuté sous condition |
| `-- hérite -->` | Généralisation entre acteurs |

---

## 1. Diagramme simplifié

Vue d'entrée. Utile pour une présentation ou pour saisir le périmètre
en un coup d'oeil avant de plonger dans les détails.

```mermaid
flowchart LR
    MJ["MJ"]
    Player["Joueur"]
    Guest["Joueur invite"]
    Guest -- herite --> Player
    subgraph SYS["Assistant MJ"]
        UC01(["Creer une campagne"])
        UC_PREP_CONTENT(["Preparer le contenu MJ"])
        UC17(["Organiser le contenu"])
        UC05(["Preparer une session"])
        UC06(["Utiliser la vue session"])
        UC09(["Partager des informations"])
        UC10(["Rejoindre une session"])
        UC07(["Consulter et modifier sa fiche"])
        UC08(["Gerer son inventaire"])
    end
    MJ --> UC01
    MJ --> UC_PREP_CONTENT
    MJ --> UC17
    MJ --> UC05
    MJ --> UC06
    MJ --> UC09
    MJ --> UC07
    MJ --> UC08
    Player --> UC10
    Player --> UC07
    Player --> UC08
    Player --> UC09
    Guest --> UC10
    Guest --> UC07
    UC05 -.->|"include"| UC_PREP_CONTENT
    UC06 -.->|"include"| UC05
    UC08 -.->|"extend"| UC07
```

---

## 2. Vue globale

Périmètre fonctionnel complet — tous les acteurs, tous les cas d'utilisation,
toutes les relations. Référence complète pour le dossier de conception.

```mermaid
flowchart LR
    MJ["MJ"]
    Player["Joueur"]
    Guest["Joueur invite"]
    Guest -- herite --> Player
    subgraph CAMP["Campagne"]
        UC_CREATE(["Creer une campagne"])
        UC_MANAGE(["Gerer une campagne"])
        UC_INVITE(["Inviter des joueurs"])
    end
    subgraph PREP["Preparation MJ"]
        UC_SCENARIO(["Structurer un scenario"])
        UC_NPC(["Gerer les PNJ"])
        UC_NOTES(["Gerer les notes MJ"])
        UC_LINK(["Lier des elements de campagne"])
        UC_FOLDERS(["Organiser en dossiers"])
        UC_TPL_SYNC(["Synchroniser avec le template"])
    end
    subgraph SESSION["Session"]
        UC_PREP_SESSION(["Preparer une session"])
        UC_RUN_SESSION(["Utiliser la vue session"])
        UC_SEARCH(["Rechercher une information"])
        UC_CLOSE(["Cloturer une session"])
    end
    subgraph JOUEURS["Joueurs"]
        UC_JOIN(["Rejoindre une campagne ou session"])
        UC_VIEW_CHAR(["Consulter sa fiche personnage"])
        UC_EDIT_CHAR(["Modifier sa fiche personnage"])
        UC_INVENTORY(["Gerer son inventaire"])
        UC_SHARED(["Consulter les informations partagees"])
    end
    subgraph PARTAGE["Partage"]
        UC_SHARE(["Partager une information"])
        UC_VISIBILITY(["Definir la visibilite"])
    end
    MJ --> UC_CREATE
    MJ --> UC_MANAGE
    MJ --> UC_INVITE
    MJ --> UC_SCENARIO
    MJ --> UC_NPC
    MJ --> UC_NOTES
    MJ --> UC_FOLDERS
    MJ --> UC_TPL_SYNC
    MJ --> UC_PREP_SESSION
    MJ --> UC_RUN_SESSION
    MJ --> UC_SEARCH
    MJ --> UC_CLOSE
    MJ --> UC_SHARE
    MJ --> UC_VIEW_CHAR
    MJ --> UC_EDIT_CHAR
    MJ --> UC_INVENTORY
    Player --> UC_JOIN
    Player --> UC_VIEW_CHAR
    Player --> UC_EDIT_CHAR
    Player --> UC_INVENTORY
    Player --> UC_SHARED
    Guest --> UC_JOIN
    Guest --> UC_VIEW_CHAR
    Guest --> UC_SHARED
    UC_SCENARIO -.->|"include"| UC_LINK
    UC_NPC -.->|"include"| UC_LINK
    UC_NOTES -.->|"include"| UC_LINK
    UC_PREP_SESSION -.->|"include"| UC_SCENARIO
    UC_PREP_SESSION -.->|"include"| UC_NPC
    UC_PREP_SESSION -.->|"include"| UC_NOTES
    UC_RUN_SESSION -.->|"include"| UC_PREP_SESSION
    UC_SHARE -.->|"include"| UC_VISIBILITY
    UC_SEARCH -.->|"extend"| UC_RUN_SESSION
    UC_CLOSE -.->|"extend"| UC_RUN_SESSION
    UC_SHARE -.->|"extend"| UC_NOTES
    UC_INVENTORY -.->|"extend"| UC_EDIT_CHAR
```

---

## 3. Vue centree MJ

Focus sur le coeur du produit : aider le MJ a preparer, organiser
et conduire ses sessions. L'acteur Joueur n'apparait pas ici.

```mermaid
flowchart LR
    MJ["MJ"]
    subgraph ORGANISER["Organiser la campagne"]
        UC_CREATE(["Creer une campagne"])
        UC_CONFIG(["Configurer la campagne"])
        UC_INVITE(["Inviter des joueurs"])
        UC_ASSIGN(["Associer les joueurs aux personnages"])
    end
    subgraph CONTENU["Preparer le contenu"]
        UC_SCENARIO(["Structurer un scenario"])
        UC_SCENE(["Creer des scenes"])
        UC_NPC(["Gerer les PNJ"])
        UC_NOTES(["Gerer les notes MJ"])
        UC_LINK(["Lier les elements de campagne"])
        UC_FOLDERS(["Organiser le contenu en dossiers"])
        UC_TPL_SYNC(["Synchroniser avec le template"])
    end
    subgraph CONDUIRE["Conduire la session"]
        UC_PREP(["Preparer une session"])
        UC_START(["Lancer la vue session"])
        UC_VIEW_CONTENT(["Consulter les elements utiles"])
        UC_SEARCH(["Rechercher une information"])
        UC_LIVE_NOTE(["Prendre une note live"])
        UC_CREATE_LIVE(["Creer un element a la volee"])
        UC_CLOSE(["Cloturer la session"])
    end
    subgraph PARTAGER["Partager"]
        UC_SHARE(["Partager une information"])
        UC_VISIBILITY(["Definir la visibilite"])
    end
    MJ --> UC_CREATE
    MJ --> UC_CONFIG
    MJ --> UC_INVITE
    MJ --> UC_ASSIGN
    MJ --> UC_SCENARIO
    MJ --> UC_NPC
    MJ --> UC_NOTES
    MJ --> UC_FOLDERS
    MJ --> UC_TPL_SYNC
    MJ --> UC_PREP
    MJ --> UC_START
    MJ --> UC_SEARCH
    MJ --> UC_LIVE_NOTE
    MJ --> UC_CREATE_LIVE
    MJ --> UC_CLOSE
    MJ --> UC_SHARE
    UC_SCENARIO -.->|"include"| UC_SCENE
    UC_SCENARIO -.->|"include"| UC_LINK
    UC_NPC -.->|"include"| UC_LINK
    UC_NOTES -.->|"include"| UC_LINK
    UC_PREP -.->|"include"| UC_SCENARIO
    UC_PREP -.->|"include"| UC_NPC
    UC_PREP -.->|"include"| UC_NOTES
    UC_START -.->|"include"| UC_PREP
    UC_START -.->|"include"| UC_VIEW_CONTENT
    UC_SHARE -.->|"include"| UC_VISIBILITY
    UC_SEARCH -.->|"extend"| UC_START
    UC_LIVE_NOTE -.->|"extend"| UC_START
    UC_CREATE_LIVE -.->|"extend"| UC_START
    UC_CLOSE -.->|"extend"| UC_START
    UC_SHARE -.->|"extend"| UC_NOTES
```

---

## 4. Vue centree joueur

Fonctionnalites cote joueur. Logique simple et secondaire par rapport au MJ.
Le MJ apparait uniquement pour l'association joueur-personnage qu'il controle.

```mermaid
flowchart LR
    Player["Joueur"]
    Guest["Joueur invite"]
    MJ["MJ"]
    Guest -- herite --> Player
    subgraph ACCES["Acces"]
        UC_JOIN(["Rejoindre une campagne ou session"])
        UC_TEMP(["Saisir un pseudo temporaire"])
        UC_ASSIGN(["Etre associe a un personnage"])
    end
    subgraph FICHE["Fiche personnage"]
        UC_VIEW(["Consulter sa fiche"])
        UC_EDIT(["Modifier sa fiche"])
        UC_INVENTORY(["Gerer son inventaire"])
        UC_NOTES(["Gerer ses notes personnelles"])
    end
    subgraph INFO["Informations"]
        UC_SHARED(["Consulter les informations partagees"])
    end
    Player --> UC_JOIN
    Player --> UC_VIEW
    Player --> UC_EDIT
    Player --> UC_INVENTORY
    Player --> UC_NOTES
    Player --> UC_SHARED
    Guest --> UC_JOIN
    Guest --> UC_TEMP
    Guest --> UC_VIEW
    Guest --> UC_SHARED
    MJ --> UC_ASSIGN
    UC_JOIN -.->|"include"| UC_ASSIGN
    UC_TEMP -.->|"extend"| UC_JOIN
    UC_INVENTORY -.->|"extend"| UC_EDIT
    UC_NOTES -.->|"extend"| UC_VIEW
```

---

## 5. Cycle preparation - session - cloture

Detaille la proposition de valeur centrale du produit :
preparer en amont, conduire efficacement, cloturer proprement.
Chaque package correspond a une phase temporelle distincte.

```mermaid
flowchart TB
    MJ["MJ"]
    subgraph AVANT["Avant la session"]
        UC_SCENARIO(["Structurer un scenario"])
        UC_SCENES(["Creer des scenes"])
        UC_SELECT_NPC(["Selectionner les PNJ utiles"])
        UC_SELECT_NOTES(["Selectionner les notes utiles"])
        UC_PREP(["Preparer une session"])
    end
    subgraph PENDANT["Pendant la session"]
        UC_RUN(["Utiliser la vue session"])
        UC_VIEW_SCENARIO(["Consulter le scenario actif"])
        UC_VIEW_NPC(["Consulter les PNJ"])
        UC_VIEW_PLAYERS(["Consulter les fiches joueurs"])
        UC_SEARCH(["Rechercher une information"])
        UC_LIVE_NOTE(["Prendre une note live"])
        UC_CREATE_LIVE(["Creer un element a la volee"])
    end
    subgraph APRES["Apres la session"]
        UC_CLOSE(["Cloturer la session"])
        UC_SUMMARY(["Rediger un resume"])
        UC_SHARE_SUMMARY(["Partager le resume"])
    end
    MJ --> UC_PREP
    MJ --> UC_RUN
    MJ --> UC_SEARCH
    MJ --> UC_LIVE_NOTE
    MJ --> UC_CREATE_LIVE
    MJ --> UC_CLOSE
    UC_PREP -.->|"include"| UC_SCENARIO
    UC_SCENARIO -.->|"include"| UC_SCENES
    UC_PREP -.->|"include"| UC_SELECT_NPC
    UC_PREP -.->|"include"| UC_SELECT_NOTES
    UC_RUN -.->|"include"| UC_PREP
    UC_RUN -.->|"include"| UC_VIEW_SCENARIO
    UC_RUN -.->|"include"| UC_VIEW_NPC
    UC_RUN -.->|"include"| UC_VIEW_PLAYERS
    UC_SEARCH -.->|"extend"| UC_RUN
    UC_LIVE_NOTE -.->|"extend"| UC_RUN
    UC_CREATE_LIVE -.->|"extend"| UC_RUN
    UC_CLOSE -.->|"extend"| UC_RUN
    UC_CLOSE -.->|"include"| UC_SUMMARY
    UC_SHARE_SUMMARY -.->|"extend"| UC_SUMMARY
```

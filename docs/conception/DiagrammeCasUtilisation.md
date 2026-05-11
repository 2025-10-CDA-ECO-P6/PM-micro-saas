# Diagramme de cas d’utilisation — Assistant MJ

> Version compatible README Markdown / GitHub avec Mermaid.
>
> Note : Mermaid ne propose pas de vrai diagramme de cas d’utilisation UML natif.  
> Le diagramme ci-dessous est donc une représentation lisible en `flowchart`, compatible README.
> Pour un diagramme UML strict, il vaut mieux générer une image PlantUML en PNG/SVG et l’intégrer dans le README.

## Diagramme principal

```mermaid
flowchart LR
    %% Acteurs
    MJ(("MJ"))
    Joueur(("Joueur"))
    Invite(("Joueur invité"))

    %% Frontière système
    subgraph App["Assistant MJ"]
        direction TB

        subgraph Acces["Accès"]
            UC_AUTH["S'authentifier"]
            UC_REGISTER["Créer un compte"]
            UC10["Rejoindre une campagne ou une session"]
            UC_ASSIGN["Être associé à un personnage"]
        end

        subgraph Campagne["Gestion de campagne"]
            UC01["Créer et configurer une campagne"]
            UC_DASH["Consulter le tableau de bord campagne"]
            UC_INVITE["Inviter des joueurs"]
        end

        subgraph Preparation["Préparation MJ"]
            UC02["Structurer un scénario"]
            UC03["Créer et gérer des PNJ"]
            UC04["Gérer les notes MJ"]
            UC05["Préparer une session de jeu"]
        end

        subgraph Session["Conduite de session"]
            UC06["Utiliser la vue session"]
            UC11["Rechercher rapidement une information"]
            UC12["Clôturer une session et préparer la suite"]
        end

        subgraph EspaceJoueur["Espace joueur"]
            UC07_VIEW["Consulter sa fiche personnage"]
            UC07_EDIT["Modifier sa fiche personnage"]
            UC08["Gérer l'inventaire"]
            UC_PLAYER_NOTES["Gérer ses notes personnelles"]
            UC_SHARED_VIEW["Consulter les informations partagées"]
        end

        subgraph Partage["Partage d'information"]
            UC09["Partager une information aux joueurs"]
            UC_VISIBILITY["Définir la visibilité d'une information"]
        end
    end

    %% Associations acteurs
    MJ --- UC_AUTH
    MJ --- UC_REGISTER
    MJ --- UC01
    MJ --- UC_DASH
    MJ --- UC_INVITE
    MJ --- UC02
    MJ --- UC03
    MJ --- UC04
    MJ --- UC05
    MJ --- UC06
    MJ --- UC11
    MJ --- UC12
    MJ --- UC07_VIEW
    MJ --- UC07_EDIT
    MJ --- UC08
    MJ --- UC09
    MJ --- UC_ASSIGN

    Joueur --- UC_AUTH
    Joueur --- UC_REGISTER
    Joueur --- UC10
    Joueur --- UC07_VIEW
    Joueur --- UC07_EDIT
    Joueur --- UC08
    Joueur --- UC_PLAYER_NOTES
    Joueur --- UC_SHARED_VIEW

    Invite --- UC10
    Invite --- UC07_VIEW
    Invite --- UC_SHARED_VIEW

    %% Relations fonctionnelles proches UML
    UC01 -. "<<include>>" .-> UC_AUTH
    UC_DASH -. "<<include>>" .-> UC_AUTH
    UC_INVITE -. "<<include>>" .-> UC_AUTH

    UC02 -. "<<include>>" .-> UC_DASH
    UC03 -. "<<include>>" .-> UC_DASH
    UC04 -. "<<include>>" .-> UC_DASH
    UC05 -. "<<include>>" .-> UC_DASH

    UC06 -. "<<include>>" .-> UC05
    UC06 -. "<<include>>" .-> UC11

    UC09 -. "<<include>>" .-> UC_VISIBILITY
    UC10 -. "<<include>>" .-> UC_ASSIGN

    UC_REGISTER -. "<<extend>>" .-> UC_AUTH
    UC09 -. "<<extend>>" .-> UC04
    UC12 -. "<<extend>>" .-> UC06
    UC08 -. "<<extend>>" .-> UC07_EDIT
    UC_PLAYER_NOTES -. "<<extend>>" .-> UC07_VIEW
```

## Version simplifiée

```mermaid
flowchart LR
    MJ(("MJ"))
    Joueur(("Joueur"))
    Invite(("Joueur invité"))

    subgraph App["Assistant MJ"]
        UC01["Créer une campagne"]
        UC02["Structurer un scénario"]
        UC03["Gérer les PNJ"]
        UC04["Gérer les notes"]
        UC05["Préparer une session"]
        UC06["Utiliser la vue session"]
        UC11["Rechercher une information"]
        UC09["Partager une information"]
        UC12["Clôturer une session"]
        UC10["Rejoindre une session"]
        UC07["Consulter / modifier sa fiche personnage"]
        UC08["Gérer son inventaire"]
    end

    MJ --- UC01
    MJ --- UC02
    MJ --- UC03
    MJ --- UC04
    MJ --- UC05
    MJ --- UC06
    MJ --- UC11
    MJ --- UC09
    MJ --- UC12
    MJ --- UC07
    MJ --- UC08

    Joueur --- UC10
    Joueur --- UC07
    Joueur --- UC08
    Joueur --- UC09

    Invite --- UC10
    Invite --- UC07

    UC06 -. "<<include>>" .-> UC05
    UC06 -. "<<include>>" .-> UC11
    UC09 -. "<<extend>>" .-> UC04
    UC12 -. "<<extend>>" .-> UC06
    UC08 -. "<<extend>>" .-> UC07
```

## Pourquoi PlantUML ne s’affiche pas toujours ?

Un bloc comme celui-ci :

```plantuml
@startuml
actor MJ
@enduml
```

ne s’affiche généralement pas directement dans un README GitHub classique.

Pour utiliser PlantUML, il faut soit :

- générer une image `.png` ou `.svg` depuis le fichier PlantUML ;
- intégrer cette image dans le README ;
- ou utiliser un outil/documentation qui supporte PlantUML, comme certains pipelines GitLab, MkDocs avec plugin, Asciidoctor, Kroki, etc.

## Recommandation

Pour le README du projet, utiliser la version Mermaid ci-dessus.

Pour le dossier de conception ou la soutenance, générer une image propre depuis PlantUML afin d’avoir un vrai diagramme UML plus conventionnel.
# Haversack — Diagrammes de cas d'utilisation

> Périmètre fonctionnel MVP défini dans [README.md](README.md).
> Organisés du plus synthétique au plus détaillé.
>
> **Note sur les acteurs** : `MJ` et `Joueur` sont des rôles contextuels liés à un espace de jeu,
> pas des types d'utilisateurs globaux. Un même utilisateur peut être MJ d'une campagne et joueur
> dans une autre. Le `MJ` peut utiliser l'application sans compte (mode local, UC-01).
> `JoueurInvite` est un joueur sans compte — accès temporaire via lien de session (UC-09).

---

## Légende

| Notation | Signification |
|---|---|
| Rectangle `[ ]` | Acteur |
| Ovale `([ ])` | Cas d'utilisation |
| `-->` | Association acteur → cas d'utilisation |
| `-.->` `include` | Inclusion — le cas inclus est toujours exécuté |
| `-.->` `extend` | Extension — le cas étendu s'exécute sous condition |
| `-- hérite -->` | Généralisation entre acteurs |

---

## 1. Vue simplifiée

Vue d'ensemble. Utile pour une présentation ou pour saisir le périmètre en un coup d'œil.

```mermaid
flowchart LR
    MJ["MJ"]
    Joueur["Joueur"]
    JInvite["Joueur invite"]
    JInvite -- herite --> Joueur

    subgraph SYS["Haversack — perimetre MVP"]
        UC01(["Demarrer sans compte\nmode local"])
        UC02(["Creer un espace\ncampagne ou one-shot"])
        UC_PREP(["Preparer le contenu\nscenarios · notes · dossiers"])
        UC06(["Vue session"])
        UC07(["Creer a la volee"])
        UC08(["Partager une information"])
        UC09(["Acceder a la session\nsans compte"])
        UC12(["Rejoindre\nune campagne"])
    end

    MJ --> UC01
    MJ --> UC02
    MJ --> UC_PREP
    MJ --> UC06
    MJ --> UC07
    MJ --> UC08
    Joueur --> UC09
    Joueur --> UC12
    JInvite --> UC09

    UC07 -.->|"extend"| UC06
    UC08 -.->|"extend"| UC06
```

---

## 2. Vue globale

Périmètre fonctionnel complet — tous les acteurs, tous les cas d'utilisation, toutes les relations clés.
Référence complète pour le dossier de conception.

```mermaid
flowchart LR
    MJ["MJ"]
    Joueur["Joueur"]
    JInvite["Joueur invite"]
    JInvite -- herite --> Joueur

    subgraph ACCES["Acces et compte"]
        UC01(["UC-01\nMode local"])
        UC10(["UC-10\nCompte cloud"])
    end
    subgraph ESPACES["Espaces de jeu"]
        UC02(["UC-02\nCampagne ou one-shot"])
        UC13(["UC-13\nBibliotheque de scenarios"])
    end
    subgraph PILIER1["Pilier 1 — Preparation documentaire"]
        UC03(["UC-03\nStructurer scenario"])
        UC04(["UC-04\nDocuments et notes"])
        UC05(["UC-05\nDossiers et types"])
        UC14(["UC-14\nRechercher"])
    end
    subgraph PILIER2["Pilier 2 — Vue session"]
        UC06(["UC-06\nVue session"])
        UC07(["UC-07\nCreer a la volee"])
        UC08(["UC-08\nPartager"])
        UC09(["UC-09\nAcces joueur"])
    end
    subgraph MEMBRES["Membres"]
        UC11(["UC-11\nGerer membres"])
        UC12(["UC-12\nRejoindre campagne"])
    end

    MJ --> UC01
    MJ --> UC02
    MJ --> UC03
    MJ --> UC04
    MJ --> UC05
    MJ --> UC13
    MJ --> UC06
    MJ --> UC07
    MJ --> UC08
    MJ --> UC11
    MJ --> UC14
    Joueur --> UC09
    Joueur --> UC12
    JInvite --> UC09

    UC10 -.->|"extend"| UC01
    UC13 -.->|"extend"| UC02
    UC07 -.->|"extend"| UC06
    UC08 -.->|"extend"| UC06
    UC14 -.->|"extend"| UC06
    UC11 -.->|"include"| UC10
    UC12 -.->|"include"| UC10
```

---

## 3. Vue centrée MJ

Focus sur le cœur du produit. Détaille tous les cas d'utilisation du MJ et leurs relations.

```mermaid
flowchart LR
    MJ["MJ"]

    subgraph DEMARRAGE["Demarrage"]
        UC01(["UC-01\nMode local"])
        UC10(["UC-10\nCompte cloud"])
    end
    subgraph ESPACES["Espaces de jeu"]
        UC02(["UC-02\nCampagne ou one-shot"])
        UC13(["UC-13\nBibliotheque de scenarios"])
    end
    subgraph PILIER1["Pilier 1 — Preparation documentaire"]
        UC03(["UC-03\nStructurer scenario"])
        UC04(["UC-04\nDocuments et notes"])
        UC05(["UC-05\nDossiers et types"])
    end
    subgraph PILIER2["Pilier 2 — Vue session"]
        UC06(["UC-06\nVue session"])
        UC07(["UC-07\nCreer a la volee"])
        UC08(["UC-08\nPartager"])
        UC14(["UC-14\nRechercher"])
    end
    subgraph GESTION["Gestion des acces"]
        UC11(["UC-11\nGerer membres"])
    end

    MJ --> UC01
    MJ --> UC02
    MJ --> UC03
    MJ --> UC04
    MJ --> UC05
    MJ --> UC13
    MJ --> UC06
    MJ --> UC07
    MJ --> UC08
    MJ --> UC11
    MJ --> UC14

    UC10 -.->|"extend"| UC01
    UC13 -.->|"extend"| UC02
    UC11 -.->|"include"| UC10
    UC07 -.->|"extend"| UC06
    UC08 -.->|"extend"| UC06
    UC14 -.->|"extend"| UC06
```

---

## 4. Vue centrée joueur

Expérience côté joueur. Simple par conception — le joueur est un consommateur de contenu,
pas un producteur.

```mermaid
flowchart LR
    Joueur["Joueur"]
    JInvite["Joueur invite"]
    JInvite -- herite --> Joueur

    subgraph ACCES["Acces"]
        UC09(["UC-09\nAcceder sans compte\nvia lien de session"])
        UC10(["UC-10\nCreer un compte"])
        UC12(["UC-12\nRejoindre campagne\nmembre permanent"])
    end
    subgraph CONTENU["Contenu accessible"]
        INFO(["Informations partagees\npar le MJ en temps reel"])
        HIST(["Historique des sessions\net documents partages"])
    end

    JInvite --> UC09
    Joueur --> UC09
    Joueur --> UC12

    UC10 -.->|"extend"| UC09
    UC12 -.->|"include"| UC10
    INFO -.->|"include"| UC09
    HIST -.->|"extend"| UC12
```

---

## 5. Parcours campagne vs one-shot

Compare les deux contextes de jeu. Ils convergent vers la même vue session
mais n'ont pas le même point d'entrée ni le même cycle de vie.

```mermaid
flowchart LR
    MJ["MJ"]
    Joueur["Joueur"]
    JInvite["Joueur invite"]
    JInvite -- herite --> Joueur

    subgraph CAMP["Campagne continue"]
        C1(["Creer une campagne"])
        C2(["Preparer le contenu\nscenarios · notes · dossiers"])
        C3(["Inviter des membres\npermanents"])
    end
    subgraph ONESHOT["One-shot express"]
        O1(["Bibliotheque\nde scenarios"])
        O2(["Lancer un one-shot\nen une action"])
        O3(["Lien de session\ntemporaire"])
    end
    subgraph COMMUN["Vue session — commun aux deux contextes"]
        S1(["UC-06 Vue session"])
        S2(["UC-07 Creer a la volee"])
        S3(["UC-08 Partager"])
        S4(["UC-14 Rechercher"])
        S2 -.->|"extend"| S1
        S3 -.->|"extend"| S1
        S4 -.->|"extend"| S1
    end
    subgraph JOUEURS["Acces joueurs"]
        UC09(["UC-09\nAcceder sans compte"])
        UC12(["UC-12\nRejoindre campagne"])
    end

    MJ --> C1
    C1 --> C2
    C3 -.->|"extend"| C1
    C2 --> S1
    MJ --> O1
    O1 --> O2
    O3 -.->|"extend"| O2
    O2 --> S1
    S3 --> UC09
    C3 --> UC12
    Joueur --> UC12
    JInvite --> UC09
```

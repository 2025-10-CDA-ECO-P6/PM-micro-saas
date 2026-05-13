# Haversack

> Outil d'aide à la préparation et à la conduite de parties de jeu de rôle

---

## Présentation du service

**Nom :** Haversack

**Logo :** _(à créer)_

**Slogan :** _(à définir)_

---

## Charte graphique

> _À définir : palette de couleurs (3-5), typographies, style d'illustrations, ambiance visuelle._

---

## Expression des besoins

### Problème

Un Maître du Jeu (MJ) actif gère simultanément une quantité considérable d'informations : scénarios, PNJ, notes secrètes, informations à partager aux joueurs, suivi narratif, lore. Ces informations sont aujourd'hui dispersées sur plusieurs outils non spécialisés :

| Support | Problème principal |
|---|---|
| Fichiers texte / Word | Difficile à parcourir pendant la session |
| Notion / Google Docs | Pas conçu pour le JDR |
| Discord | Informations perdues dans le scroll |
| Papier | Non cherchable, risque de perte |
| Roll20 / Foundry | Centré sur le combat, pas la narration |

Cette fragmentation casse le rythme de jeu, génère une charge mentale élevée et provoque des oublis en session.

### Public cible

**Utilisateur principal :** le Maître du Jeu — c'est lui qui prépare la campagne, choisit les outils et porte la charge organisationnelle.

**Utilisateurs secondaires :** les joueurs, qui accèdent aux informations partagées par le MJ pendant la session.

### Fonctionnalité principale

**La vue session** — un tableau de bord de conduite qui regroupe en un seul endroit tout ce dont le MJ a besoin pendant la partie : scénario actif, notes, accès au contenu préparé, prise de notes rapide, partage en temps réel. Objectif : ne plus quitter l'application pour retrouver une information.

### Fonctionnalités MVP

- Démarrage sans compte — données stockées localement dans le navigateur
- Création de campagnes et de one-shots (parcours distincts)
- Structuration de scénarios et organisation libre par dossiers
- Gestion des notes MJ (privées / partagées)
- Vue session dédiée avec création à la volée
- Partage d'informations aux joueurs (visibilité configurable)
- Recherche dans la campagne depuis la vue session
- Accès joueur sans compte (lien temporaire)
- Compte cloud optionnel : sync multi-device, partage joueurs, membres permanents

### Hors périmètre MVP

Table virtuelle visuelle, moteur de règles, gestion de combat, IA générative, desktop natif, templates communautaires.

> Détail des choix → [vision produit](docs/conception/vision/vision-produit.md) · [MoSCoW complet](docs/conception/vision/moscow.md)

### Zoning / wireframes

> _À créer : esquisses des vues principales (tableau de bord MJ, vue session, fiche campagne)._

---

## Stack technique

| Couche | Technologie |
|---|---|
| **Landing page** | Next.js (React, SSR/SSG) |
| **Application web** | Angular (SPA) |
| **Backend** | .NET / ASP.NET Core (C#) |
| **Base de données** | PostgreSQL |
| **ORM** | Entity Framework Core |
| **Authentification** | ASP.NET Identity |

**Architecture :** Clean Architecture, monolithe modulaire, Domain-Driven Design (DDD), 4 Bounded Contexts.

**Structure de la solution :**

```
Haversack.SharedKernel
Haversack.Domain
Haversack.Application
Haversack.Infrastructure.Persistence
Haversack.Infrastructure.Notifications
Haversack.Api
```

**Clients de présentation futurs** (post-MVP) :

| Client | Technologie | Effort |
|---|---|---|
| Mobile / Desktop installable | PWA (`@angular/pwa`) | Quasi nul |
| Desktop natif Win/Linux/macOS | Tauri (wrape Angular) | Faible |
| iOS / Android stores | Capacitor (wrape Angular) | Modéré |
| .NET natif multi-plateforme | MAUI (projet séparé, UI propre) | Élevé |

> [Détail des choix techniques, justifications, points forts/faibles et évolutions](docs/conception/stack.md)
> [Structure complète de la solution et clients futurs](docs/architecture/06-structure-projets.md)

---

## Diagramme de cas d'utilisation

> Vue simplifiée — [voir le détail complet](docs/conception/usecases/use-cases.md)

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

## MCD — Modèle Conceptuel de Données

> Vue globale — [voir le détail par bounded context](docs/conception/data/README.md)

```mermaid
erDiagram
    USER ||--o{ CAMPAIGN : "possede (GM)"
    USER ||--o{ CAMPAIGN_MEMBERSHIP : "membre de"
    CAMPAIGN ||--|{ CAMPAIGN_MEMBERSHIP : "contient"
    CAMPAIGN ||--o{ INVITATION : "genere"
    CAMPAIGN ||--o{ FOLDER : "contient"
    FOLDER ||--o{ DOCUMENT : "contient"
    DOCUMENT ||--o{ DOCUMENT_BLOCK : "compose de"
    DOCUMENT }o--|| DOCUMENT_TYPE : "type optionnel"
    CAMPAIGN ||--o{ SESSION : "contient"
    SESSION ||--o{ LIVE_NOTE : "genere"
    SESSION ||--o{ SESSION_PARTICIPANT : "reunit"
    CAMPAIGN ||--o{ GUEST_ACCESS : "genere"
```

---

## Documentation

- [Conception complète](docs/conception/README.md)
- [Architecture](docs/architecture/ArchitectureIndex.md)
- [Use cases détaillés](docs/conception/usecases/README.md)

# Haversack

> Outil d'aide à la préparation et à la conduite de parties de jeu de rôle

---

## Présentation du service

**Nom :** Haversack

**Logo :** _(à créer)_

**Slogan :** _(à définir)_

---

## Charte graphique

> _À définir_

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

> Détail des choix → [vision produit](docs/conception/besoin/vision/vision-produit.md) · [MoSCoW complet](docs/conception/besoin/vision/moscow.md)

### Zoning / wireframes

Le zoning d'interface MVP (arbitrages AR-01..17, inventaire des 20 écrans, châssis, exclusions) et les 20 wireframes basse-fidélité couvrant l'ensemble des surfaces MVP sont produits.

- Zoning complet : [`docs/conception/interface/zoning.md`](docs/conception/interface/zoning.md)
- Index des fiches et table de couverture UC→écran : [`docs/conception/interface/wireframes/`](docs/conception/interface/wireframes/)

---

## Stack technique

| Couche | Technologie |
|---|---|
| **Landing page** | Angular (SSR / prerender statique) |
| **Application web** | Angular (SPA) |
| **Backend** | .NET / ASP.NET Core (C#) |
| **Base de données** | PostgreSQL |
| **ORM** | Entity Framework Core |
| **Authentification** | ASP.NET Identity |

**Architecture :** Clean Architecture, monolithe modulaire, Domain-Driven Design (DDD), 4 Bounded Contexts.

**Structure de la solution :**

```
Haversack.Domain.Kernel
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

> [Détail des choix techniques, justifications, points forts/faibles et évolutions](docs/architecture/stack.md)

---

## Diagramme de cas d'utilisation

> Vue simplifiée — [voir le détail complet](docs/conception/besoin/usecases/use-cases.md)


---

## Documentation

| Dossier | Contenu |
|---|---|
| [docs/context/](docs/context/README.md) | Documents de référence de premier rang : cahier des charges, spécifications techniques, dossier d'architecture, dossier de conception détaillée, note business |
| [docs/gestion-projet/](docs/gestion-projet/README.md) | Cadrage, planification, organisation, risques, conventions de travail |
| [docs/securite/](docs/securite/README.md) | Sécurité technique et conformité légale (RGPD) |
| [docs/test/](docs/test/README.md) | Stratégie de test et de recette |
| [docs/deploiement/](docs/deploiement/README.md) | Environnements, pipelines et procédures de déploiement (dossier réservé) |
| [docs/ecoconception/](docs/ecoconception/README.md) | Démarche d'écoconception (dossier réservé, sujet non engagé) |
| [docs/conception/](docs/conception/README.md) | Vision produit, personas, use cases, user stories, user journeys, domaine DDD |
| [docs/architecture/](docs/architecture/README.md) | Décisions d'architecture (ADR), structure des projets |

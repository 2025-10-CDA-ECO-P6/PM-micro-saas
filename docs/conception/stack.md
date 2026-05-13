# Stack technique — Haversack

## Vue d'ensemble

| Couche | Technologie |
|---|---|
| Landing page | Next.js (React, SSR/SSG) |
| Application web | Angular (SPA) |
| Backend | ASP.NET Core (.NET / C#) |
| Base de données | PostgreSQL |
| ORM | Entity Framework Core |
| Authentification | ASP.NET Identity |
| Architecture | Monolithe modulaire, DDD, Clean Architecture |

**Structure de la solution :**
```
Haversack.SharedKernel
Haversack.Domain
Haversack.Application
Haversack.Infrastructure.Persistence
Haversack.Infrastructure.Notifications
Haversack.Api
```
> [Détail complet de la structure des projets](../architecture/06-structure-projets.md)

---

## Landing page — Next.js

### Pourquoi

Next.js a été retenu parce que la landing page a des besoins opposés à ceux de l'application : SEO, performance au premier chargement, peu d'interactivité. Un SPA Angular serait inadapté à ce cas.

### Points forts

- SSR et SSG natifs — pages indexables, chargement rapide
- Ecosystem React mature pour les composants marketing (animations, sections, formulaires)
- Déploiement simple sur Vercel ou autre plateforme edge
- Peut accueillir un blog, un changelog ou une documentation publique sans changement d'outil

### Points faibles

- Deux frontends à maintenir (Next.js + Angular) — dette organisationnelle dès le départ
- Stack React distincte de l'app Angular — pas de partage de composants possible entre les deux

### Ce qu'elle permet dans le futur

- Ajouter un blog de contenu (SEO JDR) pour l'acquisition organique
- Pages de documentation publique ou de présentation des fonctionnalités
- A/B testing de landing avec des outils comme Vercel Analytics

### Limitations

- Totalement découplé de l'app — toute navigation entre la landing et l'app passe par un lien externe
- Duplication possible de certains éléments visuels (couleurs, composants) si la charte graphique évolue

---

## Application web — Angular (SPA)

### Pourquoi

Haversack est une application avec beaucoup d'état, de vues imbriquées, de formulaires complexes et de navigation entre entités (campagne → scénario → scène → PNJ). Angular est conçu pour ce type d'application : structure imposée, injection de dépendances native, RxJS pour la gestion des flux asynchrones.

### Points forts

- Framework opinioné — l'organisation du code est standardisée, pas de décisions à prendre sur l'architecture frontend
- TypeScript en natif, strict par défaut
- DI intégrée — cohérente avec la culture .NET du projet
- Bon support des formulaires réactifs (fiches personnages, éditeurs de contenu)
- `@angular/pwa` : transform l'app en PWA installable mobile/desktop en une commande

### Points faibles

- Bundle initial plus lourd que Vue ou React — compensé par le fait qu'il n'y a pas de besoin SEO sur l'app
- Verbosité — plus de boilerplate que Vue pour les composants simples

### Ce qu'elle permet dans le futur

- **PWA** : installation sur mobile et desktop sans passer par les stores, avec support offline partiel
- **Tauri** : wrapper natif Windows/Linux/macOS (5–10 MB) autour du build Angular existant, sans réécrire l'UI
- **Capacitor** : packaging iOS/Android si le PWA ne suffit pas pour les stores
- L'Angular app reste inchangée dans tous ces cas — c'est la couche présentation web qui est réutilisée

### Limitations

- Pas de SSR sur l'app (non nécessaire — aucun contenu à indexer)
- Si un jour on veut des projets clients mobiles natifs (MAUI par exemple), l'Angular app ne peut pas être réutilisée — mais c'est un choix documenté : les futurs clients natifs sont des projets de présentation séparés dans la Clean Architecture, consommant la même API

---

## Backend — ASP.NET Core (.NET / C#)

### Pourquoi

ASP.NET Core est un choix naturel pour une architecture DDD + Clean Architecture : typage fort, support natif des value objects, records, sealed classes, bonne intégration EF Core. La plateforme est mature et performante.

### Points forts

- Performances HTTP parmi les meilleures (Kestrel, minimal APIs)
- C# adapté au DDD : records immuables, pattern matching, sealed hierarchies
- Clean Architecture bien supportée par l'écosystème .NET
- Outillage solide : migrations EF Core, ASP.NET Identity, OpenAPI auto-généré

### Points faibles

- Déploiement plus lourd qu'un serveur Node.js (runtime .NET à installer)
- Moins "startup-friendly" en termes d'hébergement low-cost (pas de free tier sur Railway pour .NET, par exemple)

### Ce qu'elle permet dans le futur

- **Clients multiples** : la Clean Architecture permet d'ajouter des projets de présentation séparés (MAUI mobile, desktop, CLI) qui consomment la même couche Application sans modifier le domaine
- **Extraction de modules** : chaque Bounded Context peut être extrait en service indépendant si la charge le justifie
- **gRPC ou GraphQL** : l'API REST peut être complétée ou remplacée par d'autres protocoles sans toucher au domaine

### Limitations

- Tous les Bounded Contexts partagent le même process — une régression dans un module peut affecter les autres (limitation du monolithe modulaire, pas de .NET spécifiquement)
- Le scaling horizontal s'applique à l'ensemble du monolithe

---

## Base de données — PostgreSQL

### Pourquoi

PostgreSQL supporte nativement la recherche plein texte (FTS), le JSONB pour les blocs de contenu (`DOCUMENT_BLOCK.value`), et les UUID. 

### Points forts

- FTS intégré — pas besoin d'Elasticsearch pour le MVP
- JSONB — stockage flexible des structures de blocs sans multiplier les tables
- Fiable, open source, hébergeable partout (Railway, Supabase, Render, auto-hébergé)

### Points faibles

- Nécessite un serveur dédié (contrairement à SQLite pour le dev)
- Pas de scaling horizontal natif (sharding manuel ou passage à CockroachDB si nécessaire)

### Ce qu'elle permet dans le futur

- FTS avancé avec `tsvector` et ranking par pertinence
- Triggers pour des projections légères ou des audits
- Extensions (pg_vector pour de l'IA sémantique si besoin)

### Limitations

- Base de données unique pour tous les Bounded Contexts — cohérent avec le monolithe modulaire, mais complique une extraction future en microservices (chaque service devrait avoir sa propre base)
- Pas de separation physique entre contextes — la discipline applicative doit compenser (pas de jointures cross-context en base)

---

## Architecture globale — Clean Architecture + Monolithe modulaire + DDD

### Pourquoi

Pour un Micro-SaaS avec un seul développeur en phase MVP, un monolithe modulaire est le bon compromis : déploiement simple, pas de latence réseau entre modules, outillage standard. La Clean Architecture garantit que `Domain` et `Application` sont agnostiques de tout client et de toute technologie d'infrastructure. Le DDD garantit que la modularité est réelle (isolation stricte des Bounded Contexts) et non cosmétique.

### Ce qu'elle permet dans le futur

**Clients de présentation supplémentaires** — `Domain` et `Application` ne changeant pas, chaque nouveau client est un projet de présentation indépendant :

| Client | Technologie | Réutilise Angular | Effort |
|---|---|---|---|
| Mobile / Desktop installable | PWA (`@angular/pwa`) | Oui (identique) | Quasi nul |
| Desktop natif Win/Linux/macOS | Tauri | Oui (webview) | Faible |
| iOS / Android stores | Capacitor | Oui (webview) | Modéré |
| .NET natif multi-plateforme | MAUI (projet `Presentation.Maui`) | Non | Élevé |

**Extraction de contextes** — l'isolation au niveau code est la précondition à extraire un Bounded Context en microservice indépendant sans réécriture majeure.

**Remplacement d'infrastructure** — changer de provider email, migrer vers Elasticsearch, ajouter un cache Redis : aucune modification du domaine ou de l'application.

> Détail complet : [06-structure-projets.md](../architecture/06-structure-projets.md)

### Limitations

- Déploiement atomique — impossible de déployer un module seul
- Un bug critique dans un module nécessite un redéploiement complet
- La discipline d'isolation cross-context repose sur les conventions, pas sur des barrières réseau

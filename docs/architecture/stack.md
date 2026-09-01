# Stack technique — Haversack

> **Note de couches documentaires** : document de sélection technologique (couche décision). Déplacé de `docs/conception/` vers `docs/architecture/` le 2026-06-10 (finding CP-02, décision opérateur). Voir [ADR-003](decisions/ADR-003-stack-front.md) pour la trace de la décision landing + application.

## Vue d'ensemble

| Couche | Technologie |
|---|---|
| Landing page | Angular (SSR/prerender) |
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
> [Détail complet de la structure des projets](structure-projets.md)

---

## Landing page — Angular (SSR/prerender)

### Pourquoi

La landing page partage un écosystème unique avec l'application : Angular pour les deux surfaces. Cette décision élimine une dette structurelle (deux frontends, deux pipelines, deux styles de composants) disproportionnée pour une équipe solo. L'équipe réutilise la même charte graphique, les mêmes composants, et gère une seule stack JavaScript/TypeScript. Le prerender statique offre les mêmes bénéfices SEO qu'une approche React/Next.js.

### Points forts

- Un seul écosystème front à maintenir — réduction de la complexité et des dépendances
- Réutilisation de la charte graphique et des composants entre landing et application
- Prerender statique (SSG) : pages indexables, chargement rapide, déploiement sur CDN sans surcharge serveur
- Pas de double pipeline de build ni de friction liée à deux styles de développement
- Infrastructure de présentation partagée — évolution cohérente et unifiée

### Points faibles

- Effort de configuration du SSR/prerender : routes statiques, hydratation Angular, garantir la cohérence entre build statique et navigation client
- Bundle Angular hérité par la landing — budget de performance I-04 s'applique également à la landing
- Moins flexible qu'une approche multi-écosystème si des besoins radicalement différents émergent (mais scenario non retenu au MVP)

### Ce qu'elle permet dans le futur

- Ajouter du contenu dynamique via SSR (pas SSG) si des éléments de la landing doivent être mis à jour sans rebuild statique
- Blog de contenu ou documentation intégrés partageant composants et styles avec l'app
- Extensibilité sans changement d'outillage ou d'équipe de compétences

### Limitations

- Landing et application partagent le même runtime — toute régression Angular affecte les deux surfaces
- SSG MVP limite le contenu dynamique — évolution future nécessiterait passage au SSR avec ses coûts serveur
- Configuration initiale du prerender plus complexe que Next.js clé en main

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

PostgreSQL supporte nativement la recherche plein texte (FTS), le JSONB pour les blocs de contenu (`DOCUMENT_BLOCK.content`), et les UUID. 

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

> Détail complet : [structure-projets.md](structure-projets.md)

### Limitations

- Déploiement atomique — impossible de déployer un module seul
- Un bug critique dans un module nécessite un redéploiement complet
- La discipline d'isolation cross-context repose sur les conventions, pas sur des barrières réseau

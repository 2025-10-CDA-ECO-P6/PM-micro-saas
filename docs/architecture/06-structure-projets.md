# Structure des projets — Haversack

## Organisation de la solution

```
Haversack.sln
│
├── Haversack.SharedKernel
├── Haversack.Domain
├── Haversack.Application
├── Haversack.Infrastructure.Persistence
├── Haversack.Infrastructure.Notifications
└── Haversack.Api
```

---

## Détail par projet

### Haversack.SharedKernel

Contient les types partagés entre tous les Bounded Contexts. Aucune logique métier.

```
SharedKernel/
├── Ids/                  ← Id typés (CampaignId, NpcId, etc.)
├── ValueObjects/         ← Email, Slug, Tag, Visibility, AuditInfo, etc.
├── Abstractions/         ← interfaces génériques (IAggregateRoot, IDomainEvent, etc.)
└── SoftDelete/           ← ISoftDeletable, colonnes isDeleted/deletedAt/deletedById
```

Dépendances sortantes : aucune.

---

### Haversack.Domain

Contient les agrégats, entités, value objects, events et interfaces de repositories pour les quatre Bounded Contexts. Les contextes sont isolés par namespace — aucun import d'entité domaine entre contextes.

```
Domain/
├── IdentityAccess/
│   ├── User.cs
│   ├── Events/
│   └── Repositories/     ← IUserRepository (interface uniquement)
├── CampaignManagement/
│   ├── Campaign.cs
│   ├── CampaignMembership.cs
│   ├── GuestAccess.cs
│   ├── Events/
│   └── Repositories/
├── ContentLibrary/
│   ├── Document.cs
│   ├── DocumentBlock.cs
│   ├── Npc.cs
│   ├── PlayerCharacter.cs
│   ├── Scenario.cs
│   ├── Scene.cs
│   ├── Folder.cs
│   ├── Events/
│   └── Repositories/
└── SessionConduct/
    ├── Session.cs
    ├── LiveNote.cs
    ├── SessionSummary.cs
    ├── Events/
    └── Repositories/
```

Dépendances sortantes : `SharedKernel` uniquement.

**Règle d'isolation** : un namespace de contexte ne peut pas importer les entités domaine d'un autre contexte. La communication inter-contextes passe par les domain events ou par les Id typés (référence sans FK en base).

---

### Haversack.Application

Contient les commandes, queries (CQRS), handlers et services applicatifs pour les quatre Bounded Contexts. Orchestre le domaine sans en contenir la logique.

```
Application/
├── IdentityAccess/
│   ├── Commands/
│   ├── Queries/
│   └── Services/
├── CampaignManagement/
│   ├── Commands/
│   ├── Queries/
│   └── Services/
├── ContentLibrary/
│   ├── Commands/
│   ├── Queries/
│   └── Services/
└── SessionConduct/
    ├── Commands/
    ├── Queries/
    └── Services/
```

Dépendances sortantes : `Domain`, `SharedKernel`.

---

### Haversack.Infrastructure.Persistence

Implémentation de la persistance : EF Core, DbContext, migrations, implémentations de repositories.

```
Persistence/
├── AppDbContext.cs           ← DbContext unique pour le monolithe
├── Migrations/               ← migrations EF Core centralisées ici
├── Configurations/           ← IEntityTypeConfiguration<T> par entité
│   ├── CampaignConfiguration.cs
│   ├── NpcConfiguration.cs
│   └── ...
└── Repositories/             ← implémentation des IXxxRepository du Domain
    ├── CampaignRepository.cs
    └── ...
```

**Pourquoi un seul DbContext ?** Les migrations EF Core réparties sur plusieurs DbContexts dans un même projet posent des problèmes d'ordre d'exécution et de gestion des références croisées. Un DbContext unique simplifie les migrations. L'isolation entre contextes est garantie par la couche applicative, pas par des DbContexts séparés.

Dépendances sortantes : `Domain`, `Application` (interfaces), `SharedKernel`.

---

### Haversack.Infrastructure.Notifications

Implémente l'envoi de notifications — pour l'instant uniquement les emails d'invitation.

```
Notifications/
├── Email/
│   ├── SmtpEmailSender.cs        ← implémente IEmailInvitationSender (défini en Application)
│   └── Templates/
└── ...
```

Dépendances sortantes : `Application` (interfaces), `SharedKernel`.

**Évolutions futures** : notifications push, webhooks, SMS — chaque canal peut être ajouté ici sans toucher aux autres projets.

---

### Haversack.Api

Point d'entrée HTTP. Configure le DI container, expose les endpoints REST, gère l'authentification et les middlewares.

```
Api/
├── Program.cs
├── Controllers/
│   ├── CampaignController.cs
│   ├── SessionController.cs
│   └── ...
├── Middleware/
└── DependencyInjection/      ← registration des services par couche
```

Dépendances sortantes : tous les projets (c'est le point de composition).

---

## Graphe de dépendances

```
Api
 ├── Application
 │    └── Domain
 │         └── SharedKernel
 ├── Infrastructure.Persistence
 │    ├── Application
 │    └── Domain
 └── Infrastructure.Notifications
      └── Application
```

La règle fondamentale : `Domain` et `SharedKernel` ne dépendent de personne. `Application` dépend uniquement de `Domain`. L'Infrastructure dépend de `Application` (pour implémenter ses interfaces) — jamais l'inverse.

---

## Évolutions futures

### Nouveaux clients de présentation

La Clean Architecture rend `Domain` et `Application` totalement agnostiques du client. `Haversack.Api` est simplement le premier client — d'autres peuvent être ajoutés en parallèle, consommant la même couche `Application` sans la modifier.

#### PWA — progression naturelle, coût quasi nul

`@angular/pwa` ajoute un Service Worker au build Angular existant. Le résultat est installable sur mobile et desktop depuis le navigateur, avec un cache partiel hors-ligne. C'est la première étape logique post-MVP : pas de nouveau projet, pas de nouveau déploiement, juste une configuration Angular.

**Limite** : pas d'accès aux APIs systèmes natives (Bluetooth, fichiers système profond, notifications OS). Suffisant pour un outil de table.

#### Tauri — desktop natif Windows / Linux / macOS

Tauri wrape le build Angular dans une fenêtre native sans embarquer Chromium. Le binaire résultant fait 5–10 MB (vs ~150 MB pour Electron). Sous le capot, Tauri utilise le webview système (Edge sur Windows, WebKit sur macOS/Linux).

Structure dans la solution :

```
Haversack.sln
└── Presentation.Desktop/   ← projet Tauri (Rust + config)
    ├── src-tauri/
    │   ├── Cargo.toml
    │   └── main.rs         ← commandes Tauri si accès natif nécessaire
    └── (build Angular servi localement)
```

Le projet Tauri consomme `Haversack.Api` en HTTP — rien ne change dans les couches inférieures. Les commandes Tauri (Rust) peuvent exposer des fonctionnalités OS (notifications, fichiers locaux) que l'Angular invoque via `@tauri-apps/api`.

**Quand le faire** : si les utilisateurs expriment un besoin d'application installée sans passer par un navigateur, ou si une fonctionnalité nécessite un accès système non disponible en PWA.

#### Capacitor — iOS et Android

Capacitor (Ionic) wrape le même build Angular dans un conteneur iOS/Android natif. Le projet s'ajoute à côté de l'Angular app sans la modifier.

```
Presentation.Mobile/
├── android/
├── ios/
└── capacitor.config.ts
```

Capacitor expose des plugins natifs (caméra, notifications push, stockage) que l'Angular invoque via une API unifiée. Le build Angular est identique — Capacitor ajoute une couche de liaison native.

**Quand le faire** : si la PWA ne suffit pas pour les stores Apple/Google, ou si un accès caméra (photo de fiche papier) ou notifications push sont nécessaires.

#### MAUI — application .NET native (iOS, Android, Windows, macOS)

MAUI est une option distincte des précédentes : il ne réutilise pas l'Angular app. Il s'agit d'un projet de présentation .NET avec sa propre UI (XAML ou Blazor Hybrid), qui consomme directement la couche `Application` en référence de projet ou via l'API.

```
Haversack.sln
└── Presentation.Maui/      ← projet .NET MAUI
    ├── MauiProgram.cs
    ├── Pages/
    └── ViewModels/         ← consomme Application.* directement (si même solution)
```

**Avantage** : accès complet aux API natives, UI 100% native, un seul runtime .NET pour tout.

**Inconvénient** : l'UI doit être entièrement réécrite — aucune réutilisation du code Angular. Le maintenabilité est plus lourde si les deux frontends (Angular + MAUI) doivent rester fonctionnellement équivalents.

**Quand le faire** : si une expérience vraiment native (gestures iOS, widgets Android) est requise et que Capacitor ne suffit pas. À réserver à une phase de maturité produit, pas avant validation du marché.

#### Landing page — Next.js (déjà décidé)

Projet séparé, déployé indépendamment. Ne consomme pas `Haversack.Api` — uniquement le contenu marketing. Peut être étendu à un blog, changelog ou documentation publique.

---

### Synthèse des clients de présentation

| Client | Technologie | Réutilise Angular | Réutilise Domain/Application | Quand |
|---|---|---|---|---|
| Web app | Angular SPA | — | via Api | MVP |
| Landing page | Next.js | Non | Non | MVP |
| Mobile/Desktop installable | PWA | Oui (identique) | via Api | Post-MVP rapide |
| Desktop natif | Tauri | Oui (webview) | via Api | Si besoin app installée |
| Mobile stores | Capacitor | Oui (webview) | via Api | Si stores nécessaires |
| .NET natif multi-plateforme | MAUI | Non | Direct ou via Api | Maturité produit |

---

### Extraction d'un Bounded Context en service indépendant

Si un contexte (par exemple `SessionConduct`) doit devenir un service indépendant, les namespaces correspondent déjà à des frontières identifiées. Le travail consiste à :

1. Extraire les namespaces `Domain.SessionConduct` et `Application.SessionConduct` dans une nouvelle solution
2. Créer un `Infrastructure.Persistence` dédié avec sa propre base de données
3. Remplacer les appels directs (in-process) par des appels HTTP ou des messages
4. Introduire les projections locales pour les données cross-context (ex : `UserProjection` dans Campaign Management — voir ADR-12)

La discipline d'isolation au niveau code (pas d'imports cross-context) est la précondition à cette extraction sans réécriture majeure.

---

### Infrastructure supplémentaire

| Projet | Déclencheur |
|---|---|
| `Infrastructure.Search` | FTS PostgreSQL insuffisant — passage à Elasticsearch/Typesense |
| `Infrastructure.Storage` | Fichiers attachés (images, PDF) — S3 ou stockage objet |
| `Infrastructure.Cache` | Redis pour sessions, listes fréquentes ou rate limiting |
| `Infrastructure.Messaging` | Message broker si extraction de contextes en services |

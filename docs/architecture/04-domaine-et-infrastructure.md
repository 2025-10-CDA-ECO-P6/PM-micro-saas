# 04 — Domaine et infrastructure

> La séparation des couches, comment EF Core et ASP.NET Identity s'intègrent
> sans polluer le domaine, et le rôle des value objects AuditInfo et SoftDelete.

---

## Le principe de séparation

Le domaine ne dépend d'aucun framework, aucune librairie tierce, aucune technologie
de persistance. Il exprime uniquement les règles métier dans un code pur.

```
Domain Layer
├── Entities         (Campaign, Session, Document...)
├── Value Objects    (AuditInfo, Email, BlockValue...)
├── Domain Events    (SessionStarted, DocumentTitleUpdated...)
└── Repository Interfaces (ICampaignRepository...)
        ↑ ne dépend de rien d'externe

Application Layer
├── Use Cases / Command Handlers
├── Query Handlers
└── DTOs
        ↑ dépend du Domain Layer

Infrastructure Layer
├── Persistence Models   (CampaignPersistenceModel...)
├── EF Core DbContext
├── Repository Implementations
└── Mappers (Domain ↔ Persistence)
        ↑ dépend du Domain Layer et de EF Core
```

---

## Pourquoi cette séparation pour .NET / EF Core

EF Core impose ses propres contraintes : propriétés de navigation, attributs `[Key]`,
héritage via `TPH` ou `TPT`, `OwnsOne` pour les owned entities.
Si les entités domaine portent ces attributs ou héritent de classes EF Core,
elles deviennent couplées à l'infrastructure.

Avec la séparation :
- Les entités domaine peuvent être testées en isolation, sans base de données
- La technologie de persistance peut changer sans toucher au domaine
- La logique métier est lisible sans connaissance d'EF Core

---

## Le cas ASP.NET Identity

ASP.NET Core Identity fournit `IdentityUser<TKey>` avec sa propre structure
de table (`AspNetUsers`, claims, tokens...).
L'entité domaine `User` ne connaît pas `IdentityUser`.

En pratique, la couche Infrastructure maintient deux objets :

```csharp
// Infrastructure uniquement
public class UserPersistenceModel : IdentityUser<Guid> {
    public string DisplayName { get; set; }
    // champs supplémentaires
}

// Domaine pur — aucune dépendance externe
public class User : IAggregateRoot<UserId> {
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public string DisplayName { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public AuditInfo Audit { get; private set; }
    public SoftDelete? SoftDelete { get; private set; }
}

// Infrastructure — conversion bidirectionnelle
public class UserMapper {
    public User ToDomain(UserPersistenceModel model) { ... }
    public UserPersistenceModel ToPersistence(User user) { ... }
}
```

Le domaine voit un `User` propre. Identity gère l'authentification.
Les deux peuvent évoluer indépendamment.

---

## AuditInfo — pourquoi comme Value Object

Option naïve : ajouter `createdAt`, `updatedAt`, `createdById`, `updatedById`
sur chaque entité séparément.

Problèmes : duplication du code, oublis possibles sur une nouvelle entité,
logique "mettre à jour updatedAt" dispersée dans tous les services.

Avec le value object `AuditInfo` :

```csharp
// Domaine
public class Campaign : IAggregateRoot<CampaignId> {
    public AuditInfo Audit { get; private set; }
    // ...
}

// Infrastructure — EF Core owned entity
modelBuilder.Entity<CampaignPersistenceModel>()
    .OwnsOne(c => c.Audit, audit => {
        audit.Property(a => a.CreatedAt).HasColumnName("created_at");
        audit.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        audit.Property(a => a.CreatedById).HasColumnName("created_by_id");
        audit.Property(a => a.UpdatedById).HasColumnName("updated_by_id");
    });
```

Les colonnes `created_at`, `updated_at`... sont dans la même table que l'entité.
Pas de jointure, pas de table séparée. La logique de mise à jour est dans `AuditInfo`,
une fois, testée une fois.

---

## SoftDelete — pourquoi séparé de AuditInfo

`AuditInfo` et `SoftDelete` sont deux préoccupations distinctes :
- `AuditInfo` trace qui a créé et modifié une entité — présent sur toutes les entités
- `SoftDelete` gère la suppression logique — présent uniquement sur les entités qui le supportent

Certaines entités sont supprimées physiquement (`LiveNote`, `DocumentBlock`,
`SCENE_NPC`...) et ne portent pas `SoftDelete`. Les mélanger dans un seul
value object créerait des champs inutilisés sur ces entités.

```csharp
// Entité avec soft delete
public class Campaign : IAggregateRoot<CampaignId> {
    public AuditInfo Audit { get; private set; }
    public SoftDelete? SoftDelete { get; private set; }
}

// Entité sans soft delete — suppression physique
public class DocumentBlock : IEntity<BlockId> {
    public AuditInfo Audit { get; private set; }
    public bool IsLocked { get; private set; } = false;  // modélisé, inactif MVP
    // pas de SoftDelete
}
```

Les requêtes de filtre global EF Core s'appliquent sur `SoftDelete.IsDeleted`
uniquement sur les entités qui portent ce value object.

---

## Domain et Application sont agnostiques du client

`Domain` et `Application` ne savent pas s'ils sont appelés depuis une requête HTTP, une commande MAUI, un job en arrière-plan ou un test unitaire. C'est une propriété structurelle de la Clean Architecture, pas une bonne pratique optionnelle.

```
[Angular SPA]  [Next.js]  [MAUI]  [Tauri]  [Capacitor]  [CLI]  [Tests]
      ↓             ↓        ↓        ↓           ↓         ↓       ↓
  [Haversack.Api]       [Presentation.Maui]  [Presentation.Desktop]
          ↓                      ↓                     ↓
              ───────── Haversack.Application ──────────
                               ↓
                        Haversack.Domain
```

Chaque client est un projet de présentation séparé. Il peut :
- consommer `Haversack.Api` via HTTP (Angular, Tauri, Capacitor, Next.js)
- ou référencer directement `Haversack.Application` dans la même solution (MAUI, CLI, tests)

Dans les deux cas, `Domain` et `Application` ne changent pas. Les handlers, règles métier, invariants et domain events sont identiques quel que soit le client qui les invoque.

**Conséquence pratique** : ajouter un client mobile (Capacitor) ou desktop (Tauri) après le MVP est un travail de présentation pur — pas de régression possible dans le domaine. Voir [06-structure-projets.md](06-structure-projets.md) pour le détail par technologie.

---

## Flux d'une modification typique

Voici le flux complet d'un use case, de l'API au domaine et retour :

```
HTTP Request
    → Controller (présentation)
    → Command (Application Layer)
    → CommandHandler
        → IRepository.FindById(id)          ← interface domaine
            → Repository (Infrastructure)
                → EF Core query
                → PersistenceModel
                → Mapper.ToDomain()
                → Entité domaine
        → entité.Méthode()                  ← règle métier dans le domaine
        → IRepository.Save(entité)
            → Repository (Infrastructure)
                → Mapper.ToPersistence()
                → EF Core SaveChanges()
        → Publish DomainEvents              ← effets de bord découplés
    → Response DTO
```

Le domaine n'apparaît qu'au milieu. Il ne sait rien de HTTP, EF Core ou ASP.NET.
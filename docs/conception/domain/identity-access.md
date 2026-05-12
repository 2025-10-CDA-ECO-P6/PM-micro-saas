# Bounded Context — Identity & Access

### Responsabilité

Gestion des utilisateurs authentifiés.
Isolé pour permettre l'ajout futur d'OAuth ou SSO sans impacter les autres contextes.

> **Note importante** : Les joueurs invités ne sont **pas** des `User`.
> Ils sont représentés par un `GuestAccess` dans Campaign Management.
> `User` ne porte aucun rôle global — MJ et Joueur sont des rôles contextuels
> portés par `CampaignMembership.role` dans Campaign Management.

### Note d'implémentation

En .NET, ce contexte utilise ASP.NET Core Identity en infrastructure.
L'entité domaine `User` est totalement indépendante de `IdentityUser`.
Le mapping est géré par un `UserMapper` dans la couche Infrastructure.

---

### Agrégat : `User`

```
User
├── id          : UserId
├── email       : Email
├── displayName : String
├── status      : UserStatus
├── audit       : AuditInfo
└── softDelete  : SoftDelete
```

#### Enumeration

```
UserStatus
├── ACTIVE      — compte actif
├── SUSPENDED   — compte suspendu temporairement
└── DELETED     — soft delete, compte désactivé définitivement
```

#### Invariants et règles métier

- L'email est unique dans le système.
- Un utilisateur `DELETED` n'est jamais supprimé physiquement.
- Tout utilisateur authentifié peut créer une campagne — il en devient automatiquement le MJ (`MemberRole.OWNER`) via `CampaignMembership`.
- Un utilisateur peut demander la suppression de son compte (RGPD) : statut → `DELETED`, données personnelles anonymisées. Les contenus de campagne (notes, personnages) ne sont pas supprimés — leur `createdById` est conservé pour l'intégrité.

#### Domain Events

```
UserRegistered    { userId, email, occurredAt }
UserDeactivated   { userId, occurredAt }
```

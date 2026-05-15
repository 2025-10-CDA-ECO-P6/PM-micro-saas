# Core — Shared Kernel

> Le Core est le noyau partagé entre tous les bounded contexts.
> Il contient uniquement des abstractions techniques, des IDs typés et des value objects stables.
> Il ne contient aucune logique métier spécifique à un contexte, et aucune entité.

---

## Rôle

Le Core permet aux bounded contexts de parler le même langage technique sans se coupler entre eux.
Un bounded context qui utilise `UserId` ou `Email` ne dépend pas du module Identity & Access —
il dépend du Core, qui est une dépendance stable et agnostique.

---

## Ce qui est dans le Core

### Abstractions DDD

| Classe | Rôle |
|---|---|
| `AggregateRoot` | Classe de base pour tous les agrégats. Porte la collection d'événements domaine non publiés. |
| `Entity` | Classe de base pour les entités (identité par ID). |
| `ValueObject` | Classe de base pour les value objects (identité par valeur). |
| `DomainEvent` | Classe de base pour les événements domaine. |

### IDs typés

Tous les identifiants sont des types forts (wrappers sur `Guid`) pour éviter les confusions entre IDs de natures différentes.

| Type | Utilisé dans |
|---|---|
| `UserId` | Identity & Access, Campaign Management, Content Library, Session Conduct |
| `CampaignId` | Campaign Management, Content Library, Session Conduct |
| `SessionId` | Session Conduct, Campaign Management |
| `DocumentId` | Content Library, Session Conduct |
| `FolderId` | Content Library |

### Value objects primitifs

| VO | Validation | Utilisé dans |
|---|---|---|
| `Email` | Format RFC 5321, normalisé en minuscules | Identity & Access, Campaign Management (invitations) |
| `Slug` | Alphanumérique + tirets, minuscules, 3–100 caractères | Content Library, Campaign Management |
| `Tag` | Chaîne non vide, max 50 caractères, normalisée | Content Library, Session Conduct |

### Primitives de traçabilité

| Concept | Champs | Utilisé dans |
|---|---|---|
| `AuditInfo` | `createdAt: DateTime`, `updatedAt: DateTime`, `createdById: UserId` | Tous les contextes |
| `SoftDelete` | `isDeleted: bool`, `deletedAt: DateTime?` | Content Library, Campaign Management |

### Enums partagés

| Enum | Valeurs | Utilisé dans |
|---|---|---|
| `Visibility` | `PUBLIC`, `GM_ONLY`, `PLAYER_PRIVATE` | Content Library (documents), Session Conduct (notes de session) |

---

## Ce qui n'est PAS dans le Core

| Concept | Appartient à | Raison |
|---|---|---|
| `AccessPolicy`, `GuestAccess` | Campaign Management | Spécifique à l'accès campagne |
| `MemberRole`, `CampaignMembership` | Campaign Management | Rôle contextuel par campagne |
| `DocumentType`, `DocumentBlock` | Content Library | Structure de contenu spécifique |
| Machine d'états de Session | Session Conduct | Logique LIVE→CLOSED→ARCHIVED |
| Documents de type `LIVE_NOTE`, documents épinglés de session | Session Conduct | Concepts de session uniquement |
| `Invitation` | Campaign Management | Spécifique à l'accès campagne |
| Moteur de recherche (FTS) | Application / Infrastructure | PostgreSQL FTS — pas du domaine |
| Toute entité avec un ID propre | Son bounded context propriétaire | Le Core ne contient jamais d'entités |

---

## Règles de gouvernance

Avant d'ajouter un concept dans le Core :

1. Ce concept est-il utile à **au moins deux** bounded contexts ?
2. Son sens métier reste-t-il **identique** dans ces contextes ?
3. Peut-il exister **sans connaître** Campaign, Session, Content ou Identity ?
4. Peut-il être **testé seul** ?
5. Est-il **stable** — ne changera-t-il pas à chaque évolution d'un contexte spécifique ?
6. S'agit-il d'une **abstraction ou d'un type**, et non d'une entité avec un cycle de vie ?

Si une réponse est **non**, le concept reste dans son bounded context propriétaire.

---

## Diagramme

→ [Classes](diagrams/classes/core.md)

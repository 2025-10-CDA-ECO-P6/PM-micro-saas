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

Tous les identifiants sont des types forts encapsulant un identifiant unique global pour éviter les confusions entre IDs de natures différentes.

| Type | Utilisé dans |
|---|---|
| `UserId` | Identity & Access, Space Management, Content Library, Session Conduct |
| `SpaceId` | Space Management, Content Library, Session Conduct |
| `GuestAccessId` | Space Management, Content Library, Session Conduct |
| `SessionId` | Session Conduct, Space Management |
| `DocumentId` | Content Library, Session Conduct |
| `FolderId` | Content Library, Session Conduct |

### Value objects primitifs

| VO | Validation | Utilisé dans |
|---|---|---|
| `Email` | Format RFC 5321, normalisé en minuscules | Identity & Access, Space Management (invitations) |
| `Slug` | Alphanumérique + tirets, minuscules, 3–100 caractères | Content Library, Space Management |
| `Tag` | Chaîne non vide, max 50 caractères, normalisée | Content Library, Session Conduct |

### Primitives de traçabilité

| Concept | Champs | Utilisé dans |
|---|---|---|
| `AuditInfo` | `createdAt: DateTime`, `updatedAt: DateTime`, `createdById: UserId` | Tous les contextes |
| `SoftDelete` | `isDeleted: bool`, `deletedAt: DateTime?` | Content Library, Space Management — **exception RGPD** : les notes `PLAYER_PRIVATE` sous obligation d'effacement (fin d'accès invité, suppression de compte) sont supprimées **physiquement** et ne relèvent pas de cette primitive (voir ci-dessous). |

> **Exception RGPD à `SoftDelete`** : la suppression logique réversible (`SoftDelete`) ne s'applique
> pas aux notes `PLAYER_PRIVATE` qui tombent sous une obligation d'effacement RGPD (article 17 —
> droit à l'effacement). Deux populations concernées :
> - Notes créées par un utilisateur dont le compte est supprimé (fin de compte).
> - Notes créées par un joueur invité dont l'accès a pris fin définitivement (expiration ou révocation
>   sans conversion en compte).
>
> Pour ces populations, « supprimé » signifie « **effacé physiquement** », pas « masqué ». Le
> `SoftDelete` est réservé aux suppressions réversibles dans le cycle de vie normal du contenu
> (suppression MJ d'un document, archivage). Il n'est pas un substitut à l'effacement légal.
> Voir règle F-08 dans Session Conduct et Identity & Access.

### Enums partagés

| Enum | Valeurs | Utilisé dans |
|---|---|---|
| `Visibility` | `PUBLIC`, `GM_ONLY`, `PLAYER_PRIVATE` | Content Library (documents), Session Conduct (notes de session) |

---

## Ce qui n'est PAS dans le Core

| Concept | Appartient à | Raison |
|---|---|---|
| `AccessPolicy`, `GuestAccess` | Space Management | Spécifique à l'accès espace — seul le type `GuestAccessId` est partagé au Core (même patron que `Document`/`DocumentId` : l'entité vit dans son contexte, son ID typé est au Core) |
| `MemberRole`, `SpaceMembership` | Space Management | Rôle contextuel par espace |
| `DocumentType`, `DocumentBlock` | Content Library | Structure de contenu spécifique |
| Machine d'états de Session | Session Conduct | Logique LIVE→CLOSED→ARCHIVED |
| Documents de type `LIVE_NOTE`, documents épinglés de session | Session Conduct | Concepts de session uniquement |
| `Invitation` | Space Management | Spécifique à l'accès espace |
| Moteur de recherche | Application / Infrastructure | La recherche plein texte est une préoccupation d'infrastructure, pas du domaine |
| Toute entité avec un ID propre | Son bounded context propriétaire | Le Core ne contient jamais d'entités |

---

## Règles de gouvernance

Avant d'ajouter un concept dans le Core :

1. Ce concept est-il utile à **au moins deux** bounded contexts ?
2. Son sens métier reste-t-il **identique** dans ces contextes ?
3. Peut-il exister **sans connaître** Space, Session, Content ou Identity ?
4. Peut-il être **testé seul** ?
5. Est-il **stable** — ne changera-t-il pas à chaque évolution d'un contexte spécifique ?
6. S'agit-il d'une **abstraction ou d'un type**, et non d'une entité avec un cycle de vie ?

Si une réponse est **non**, le concept reste dans son bounded context propriétaire.

---

## Diagramme

→ [Classes](diagrams/classes/core.md)

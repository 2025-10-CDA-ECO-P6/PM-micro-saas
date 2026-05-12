# 02 — Bounded Contexts

> Comment le domaine est découpé, pourquoi ce découpage précis,
> et les règles d'isolation entre contextes.

---

## Qu'est-ce qu'un Bounded Context

Un Bounded Context est une **frontière explicite** dans laquelle un modèle donné
est valide et cohérent. Le même mot peut avoir des significations différentes
dans deux contextes distincts — c'est normal et attendu en DDD.

---

## Les 4 contextes de Haversack

```
┌─────────────────────────────────────────────────────────────┐
│                      Shared Kernel                          │
│          Id typés · AuditInfo · SoftDelete · ...            │
└─────────────────────────────────────────────────────────────┘
              consommé par tous les contextes

       ┌──────────────────┐
       │ Identity & Access│  ← Upstream de tous les autres
       │   User            │    Fournit UserId
       └──────────────────┘
                │ UserId
       ┌────────────────────────────┐
       │   Campaign Management      │  ← Contexte organisationnel
       │   Campaign · Membership    │    Fournit CampaignId
       │   Invitation · GuestAccess │
       │   GameSystem · AccessPolicy│
       └────────────────────────────┘
                │ CampaignId
    ┌───────────────────────────┐     ┌──────────────────────┐
    │   Content Library         │────▶│   Session Conduct    │
    │   Document · NPC          │     │   Session · LiveNote │
    │   PlayerCharacter         │     │   SessionSummary     │
    │   Scenario · Scene        │     └──────────────────────┘
    │   DocumentTemplate        │
    └───────────────────────────┘
         DocumentId · CharacterId · ScenarioId
```

---

## Pourquoi ce découpage

**Identity & Access en upstream** : l'authentification et les rôles globaux doivent
pouvoir évoluer indépendamment (ajout OAuth, SSO, authentification externe) sans
impacter la logique métier. Isoler ce contexte protège tous les autres d'une
refonte de la gestion des identités.

**Campaign Management séparé de Content Library** : une campagne est un espace
organisationnel — elle contient des membres, des invitations, des règles d'accès.
Le contenu éditorial (notes, PNJ, scénarios) a sa propre logique de composition
et de visibilité. Mélanger les deux créerait un contexte ingérable et des
dépendances croisées difficiles à démêler.

**Session Conduct séparé de Content Library** : une session consomme du contenu
mais ne le possède pas. Elle a sa propre logique opérationnelle — état LIVE unique
par campagne, transitions de statut strictes, notes en temps réel. Cette séparation
permet de faire évoluer le moteur de session sans risquer de casser le contenu.

---

## Le Shared Kernel

### Pourquoi il existe

Sans Shared Kernel, chaque contexte réinventerait les mêmes primitives.
`UserId` défini quatre fois, `AuditInfo` copié partout, `Visibility` redéclaré
dans chaque contexte. La duplication crée des incohérences inévitables.

Le Shared Kernel centralise les primitives **stables** sans créer de couplage
métier entre contextes.

### Règle de contenu

> Un concept rejoint le Shared Kernel si et seulement si :
> - il est utilisé par au moins deux contextes différents
> - il est stable — il change rarement
> - il n'a aucune règle métier propre à un contexte

**Ce qui est dans le Shared Kernel** :

- Les Id typés (`UserId`, `CampaignId`, `DocumentId`...)
- Les value objects fondations (`AuditInfo`, `SoftDelete`, `Email`, `Slug`, `Visibility`, `PinnedItem`)
- Les abstractions d'infrastructure (`IAggregateRoot`, `IEntity`, `IDomainEvent`, `IRepository`, `IUnitOfWork`)

**Ce qui n'est jamais dans le Shared Kernel** :

- Les entités métier — elles appartiennent à leur contexte
- Les règles de validation métier — elles appartiennent au contexte qui les définit
- Les services domaine — ils ont des comportements, donc un contexte propriétaire

---

## Règle d'isolation entre contextes

> Un bounded context ne peut jamais importer une entité domaine d'un autre contexte.
> Il ne consomme que les Id typés du Shared Kernel.

Cette règle est stricte et intentionnelle. Si `Session` importait `Scenario` de
Content Library, tout changement dans `Scenario` deviendrait un risque de régression
pour `Session`. Avec les Id typés uniquement, les contextes peuvent évoluer en parallèle.

Quand la couche Application a besoin d'une entité d'un autre contexte, elle appelle
le repository de ce contexte avec l'Id typé. Ce n'est pas une jointure SQL —
c'est une query cross-context applicative.

---

## Structure de dossiers cible

Cette isolation se traduit directement dans l'organisation du code :

```
src/
├── Core/                          ← Shared Kernel
│   ├── Domain/
│   │   ├── Primitives/            ← AuditInfo, SoftDelete, Slug, Email...
│   │   ├── Ids/                   ← UserId, CampaignId, DocumentId...
│   │   └── Abstractions/          ← IAggregateRoot, IRepository...
│
├── Identity/                      ← Bounded Context
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
│
├── CampaignManagement/            ← Bounded Context
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
│
├── ContentLibrary/                ← Bounded Context
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
│
└── SessionConduct/                ← Bounded Context
    ├── Domain/
    ├── Application/
    └── Infrastructure/
```

Un contexte ne référence jamais le dossier `Domain/` d'un autre contexte.
Seul `Core/` est partagé.

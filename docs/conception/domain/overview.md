# Vision du domaine et Bounded Contexts

## 1. Vision du domaine

Haversack est un outil d'assistance au Maître du Jeu (MJ) pour la préparation
et la conduite de campagnes de jeu de rôle.

Le domaine résout un problème central : **la dispersion des informations**.
Un MJ gère simultanément des scénarios, des PNJ, des notes privées, des fiches
personnages, des sessions en cours sur des outils non spécialisés et non connectés.
Haversack centralise ces informations dans un modèle structuré, modulaire et extensible.

### Utilisateurs du domaine

MJ et Joueur sont des **rôles contextuels au sein d'une campagne**, pas des attributs globaux d'un utilisateur.
Un même utilisateur peut être MJ d'une campagne et joueur dans une autre.
Tout utilisateur authentifié peut créer une campagne — il en devient automatiquement le MJ (OWNER).

| Rôle | Description |
|---|---|
| **MJ (Maître du Jeu)** | Rôle dans une campagne. Propriétaire de la campagne, droits complets : création de contenu, gestion des membres, conduite des sessions. Tout utilisateur authentifié peut endosser ce rôle en créant une campagne. |
| **Joueur** | Rôle dans une campagne. Accède à sa fiche personnage et aux informations partagées par le MJ dans les campagnes dont il est membre. |
| **Joueur invité** | Accès temporaire sans compte. Rejoint via token d'invitation. Traité comme un joueur à part entière du point de vue de la visibilité du contenu — la distinction est uniquement technique (pas d'identité persistante). |

---

## 3. Bounded Contexts et Context Map

### Découpage en Bounded Contexts

| Contexte | Responsabilité | Agrégats racines | Profils / entités avec repository |
|---|---|---|---|
| **Core** (Shared Kernel) | Primitives, Id typés, abstractions | — | — |
| **Identity & Access** | Utilisateurs authentifiés, rôles globaux | `User` | — |
| **Campaign Management** | Campagnes, membres, invitations, systèmes de jeu, accès aux ressources partageables | `Campaign`, `GameSystem` | `GuestAccess` |
| **Content Library** | Tout le contenu éditorial | `Document`, `Scenario`, `DocumentTemplate`, `Folder`, `Tag` | `NPC`, `PlayerCharacter` |
| **Session Conduct** | Préparation, conduite et clôture des sessions | `Session` | — |

### Context Map

```
Core (Shared Kernel)
  └── consommé par tous les contextes
      └── fournit : Id typés, AuditInfo, SoftDelete, abstractions, RequesterId

Identity & Access  [Upstream]
  └── fournit UserId à tous les autres contextes

Campaign Management
  ├── consomme Identity (UserId)
  ├── fournit CampaignId à Content Library et Session Conduct
  └── héberge AccessPolicy — service domaine de visibilité des ressources partageables

Content Library
  ├── consomme Campaign Management (CampaignId)
  ├── consomme Identity (UserId)
  └── fournit DocumentId, NpcId, CharacterId, ScenarioId, TagId à Session Conduct

Session Conduct
  ├── consomme Campaign Management (CampaignId)
  ├── consomme Content Library (DocumentId, CharacterId, ScenarioId)
  └── ne référence les entités des autres contextes que par leurs Id typés
```

### Règle de dépendance entre contextes

> Un bounded context ne peut jamais importer une entité domaine d'un autre contexte.
> Il ne consomme que les **Id typés** et les **primitives** du Shared Kernel.
> La résolution d'un Id vers une entité complète est faite par la couche Application
> via une query cross-context.

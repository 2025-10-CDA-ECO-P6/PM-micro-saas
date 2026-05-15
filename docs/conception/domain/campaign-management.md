# Campaign Management

> **Responsabilité** : gérer les espaces de jeu (campagnes et one-shots), leur configuration,
> leurs membres, leurs accès invités et leurs invitations.
> Ce contexte est le gardien de l'accès : il décide qui peut entrer dans quel espace et avec quel niveau d'accès.
>
> Il ne gère pas le contenu (documents, scénarios → Content Library),
> ni le déroulement de session (→ Session Conduct).
> Il fournit les identités d'accès que ces contextes consomment.

---

## Ce que ce contexte fait

- Créer et configurer une campagne ou un one-shot.
- Gérer les membres : inviter, associer à un personnage, retirer.
- Gérer les co-MJs (rôle GM).
- Créer et révoquer des invitations (par lien ou email).
- Gérer les accès invités sans compte (GuestAccess).
- Convertir un GuestAccess en CampaignMembership lors de la création de compte.
- Géler les campagnes excédentaires lors d'un downgrade de tier.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Contenu de la campagne (documents, scénarios, scènes) | Content Library |
| Déroulement de session (LIVE, LiveNotes, partage) | Session Conduct |
| Comptes utilisateurs, authentification | Identity & Access |
| Gestion des personnages joueurs (fiche complète) | Content Library |

---

## Agrégats

### Campaign (agrégat principal)

Frontière de cohérence pour les membres et les invitations. Représente indifféremment une campagne longue ou un one-shot — la différence est portée par `type`.

| Champ | Type | Description |
|---|---|---|
| `id` | `CampaignId` | |
| `ownerId` | `UserId` | Propriétaire technique unique — responsabilité billing et RGPD. FK réelle vers `users` (ADR-12). |
| `name` | `string` | Nom de la campagne |
| `slug` | `Slug` | Unique par propriétaire |
| `type` | `CampaignType` | `CAMPAIGN` \| `ONE_SHOT` |
| `status` | `CampaignStatus` | `ACTIVE` \| `ARCHIVED` \| `FROZEN` |
| `memberships` | `CampaignMembership[]` | Entités enfants |
| `invitations` | `Invitation[]` | Entités enfants |
| `createdAt` | `DateTime` | |
| `updatedAt` | `DateTime` | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(ownerId, name, type)` | `CampaignCreated` | Crée la campagne. L'owner est automatiquement ajouté comme membre OWNER. |
| `AddMember(userId, role)` | `MemberJoined` | Ajoute un membre (GM ou PLAYER). |
| `RemoveMember(userId)` | `MemberRemoved` | Retire un membre. Bloqué si userId == ownerId. |
| `CreateInvitation(type, scope, options)` | — | Crée une invitation enfant. |
| `RevokeInvitation(invitationId)` | — | Passe l'invitation en REVOKED. |
| `AssociateCharacter(userId, characterId)` | — | Associe un personnage (référence Content Library) à un membre. |
| `Archive()` | `CampaignArchived` | Archivage manuel par le MJ. Irréversible (MVP). |
| `Freeze()` | `CampaignFrozen` | Gel automatique lors d'un downgrade de tier. Passe en lecture seule. |
| `Unfreeze()` | `CampaignUnfrozen` | Dégel lors d'un upgrade de tier. |

---

### CampaignMembership (entité dans Campaign)

| Champ | Type | Description |
|---|---|---|
| `userId` | `UserId` | |
| `role` | `MemberRole` | `OWNER` \| `GM` \| `PLAYER` |
| `status` | `MembershipStatus` | `PENDING` \| `ACTIVE` \| `REMOVED` |
| `characterIds` | `CharacterId[]` | Références vers les personnages (Content Library) |
| `joinedAt` | `DateTime` | |

**Rôles**

| Rôle | Cardinalité | Droits |
|---|---|---|
| `OWNER` | Exactement 1 | Tout : supprimer la campagne, gérer les GMs, responsabilité billing/RGPD |
| `GM` | 0..* | Gérer le contenu, lancer/piloter des sessions, inviter des joueurs — ne peut pas supprimer la campagne ni gérer d'autres GMs |
| `PLAYER` | 0..* | Accès aux contenus partagés, consultation de sa fiche personnage |

> L'OWNER est aussi en membership avec `role = OWNER` pour la cohérence des requêtes.
> `ownerId` sur `Campaign` reste le champ de responsabilité technique (billing, RGPD).

---

### Invitation (entité dans Campaign)

| Champ | Type | Description |
|---|---|---|
| `id` | `InvitationId` | |
| `token` | `string` | UUID unique, utilisé dans l'URL d'invitation |
| `type` | `InvitationType` | `LINK` \| `EMAIL` |
| `scope` | `InvitationScope` | `CAMPAIGN` \| `SESSION` |
| `sessionId` | `SessionId?` | Renseigné si scope = SESSION |
| `expiresAt` | `DateTime?` | |
| `maxUses` | `int?` | |
| `usedCount` | `int` | |
| `status` | `InvitationStatus` | `ACTIVE` \| `REVOKED` \| `EXPIRED` |
| `createdAt` | `DateTime` | |

> Une invitation est le mécanisme d'entrée. Son utilisation crée soit un `CampaignMembership`
> (utilisateur connecté), soit un `GuestAccess` (utilisateur anonyme).
> Cette orchestration est applicative — la campagne ne crée pas directement ces objets.

---

### GuestAccess (agrégat séparé)

Représente l'accès d'un joueur sans compte. Agrégat indépendant car son cycle de vie
(expiration, conversion) est orthogonal à celui de la campagne, et il est référencé
depuis Session Conduct par son token.

| Champ | Type | Description |
|---|---|---|
| `id` | `GuestAccessId` | |
| `campaignId` | `CampaignId` | |
| `scope` | `GuestAccessScope` | `SESSION` \| `CAMPAIGN` |
| `sessionId` | `SessionId?` | Renseigné si scope = SESSION |
| `token` | `string` | UUID unique, utilisé dans l'URL |
| `displayName` | `string` | Saisi par le joueur à l'arrivée |
| `characterId` | `CharacterId?` | Associé par le MJ (référence Content Library) |
| `status` | `GuestAccessStatus` | `ACTIVE` \| `EXPIRED` \| `REVOKED` \| `CONVERTED` |
| `expiresAt` | `DateTime?` | Calculé depuis la fermeture de session + 24h pour scope SESSION |
| `createdAt` | `DateTime` | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(campaignId, scope, sessionId?)` | `GuestAccessCreated` | |
| `SetDisplayName(name)` | — | Saisi par le joueur à l'arrivée |
| `AssociateCharacter(characterId)` | — | Effectué par le MJ |
| `Expire()` | `GuestAccessExpired` | Déclenché automatiquement après session + 24h |
| `Revoke()` | `GuestAccessRevoked` | Révocation manuelle par le MJ |
| `Convert(userId)` | `GuestAccessConverted` | Quand l'invité crée un compte |

---

## Enums

| Enum | Valeurs |
|---|---|
| `CampaignType` | `CAMPAIGN` \| `ONE_SHOT` |
| `CampaignStatus` | `ACTIVE` \| `ARCHIVED` \| `FROZEN` |
| `MemberRole` | `OWNER` \| `GM` \| `PLAYER` |
| `MembershipStatus` | `PENDING` \| `ACTIVE` \| `REMOVED` |
| `InvitationType` | `LINK` \| `EMAIL` |
| `InvitationScope` | `CAMPAIGN` \| `SESSION` |
| `InvitationStatus` | `ACTIVE` \| `REVOKED` \| `EXPIRED` |
| `GuestAccessScope` | `SESSION` \| `CAMPAIGN` |
| `GuestAccessStatus` | `ACTIVE` \| `EXPIRED` \| `REVOKED` \| `CONVERTED` |

---

## One-shot — spécificités

`ONE_SHOT` est un `Campaign` avec `type = ONE_SHOT`. Les différences sont comportementales, pas structurelles.

| Comportement | `CAMPAIGN` | `ONE_SHOT` |
|---|---|---|
| Parcours de création | Configuration complète | Express — nom + scénario, aucun membre requis |
| Membres permanents | Attendus | Optionnels — GuestAccess typique, mais CampaignMembership possible |
| Sessions | Multiples | Une seule attendue (non forcée techniquement) |
| Archivage | Manuel par le MJ | Manuel par le MJ |

---

## Invariants métier

1. Une `Campaign` a toujours exactement un membre avec `role = OWNER`.
2. L'OWNER ne peut pas être retiré de sa campagne (MVP : blocage).
3. Un `UserId` ne peut avoir qu'un seul `CampaignMembership` actif par campagne.
4. Un token d'`Invitation` est globalement unique.
5. Un token de `GuestAccess` est globalement unique.
6. Un utilisateur FREE ne peut pas créer une 4e campagne active — la création est bloquée avec invitation à upgrader.
7. Une `Campaign` avec `status = FROZEN` refuse toute écriture (lecture seule). Seule `Unfreeze()` est autorisée.
8. Un `GuestAccess` avec `status = CONVERTED` ne peut plus être utilisé pour accéder à la campagne.
9. Un `GuestAccess` avec `status = EXPIRED` ou `REVOKED` ne donne plus accès.

---

## Règles métier

1. Seul l'OWNER peut inviter ou retirer des GMs.
2. Un GM peut inviter ou retirer des PLAYER.
3. Retirer un membre ne supprime pas ses données dans la campagne (personnages, notes partagées restent).
4. Un membre retiré peut être réinvité.
5. Un `GuestAccess SESSION` expire à la fermeture de la session + 24h de grâce.
6. Quand `AccountTierChanged` (PRO → FREE) et que l'owner a > 3 campagnes ACTIVE : les campagnes excédentaires sont gelées dans l'ordre de création (les plus récentes en premier).
7. L'archivage est manuel et définitif (MVP). Une campagne archivée est en lecture seule.
8. Un one-shot peut avoir simultanément des `CampaignMembership` (joueurs avec compte) et des `GuestAccess` (joueurs sans compte).

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `CampaignCreated` | `Campaign.Create()` | Content Library (créer les dossiers système), Application |
| `MemberJoined` | `Campaign.AddMember()` | Application (notification) |
| `MemberRemoved` | `Campaign.RemoveMember()` | Session Conduct (retirer l'accès actif si session en cours) |
| `CampaignFrozen` | `Campaign.Freeze()` | Application (notification au MJ) |
| `CampaignUnfrozen` | `Campaign.Unfreeze()` | Application (notification au MJ) |
| `CampaignArchived` | `Campaign.Archive()` | Content Library, Session Conduct |
| `GuestAccessCreated` | `GuestAccess.Create()` | Session Conduct (accès aux informations partagées) |
| `GuestAccessExpired` | `GuestAccess.Expire()` | Session Conduct |
| `GuestAccessConverted` | `GuestAccess.Convert(userId)` | Application (finalise la création de compte), Session Conduct |

---

## Intégration avec les autres contextes

### Ce que Campaign Management reçoit

| Événement / Requête | Source | Action |
|---|---|---|
| `AccountTierChanged` | Identity & Access | Geler les campagnes excédentaires si downgrade |
| `UserDeleted` | Identity & Access | Anonymiser les données nominatives des memberships |

### Ce que Campaign Management publie

- `CampaignId`, `GuestAccessId`, `MemberRole` comme identifiants de référence pour les autres contextes.
- Les événements listés ci-dessus.

### Utilisation par les autres contextes

| Contexte | Usage |
|---|---|
| Content Library | `CampaignId` pour scoper les documents ; `CampaignCreated` pour créer les dossiers système |
| Session Conduct | `CampaignId`, `GuestAccessId`, `MemberRole` pour les autorisations d'accès en session |

---

## Diagrammes

→ [Classes](diagrams/classes/campaign-management.md)
→ [MCD](diagrams/mcd/campaign-management.md)
→ [MLD](diagrams/mld/campaign-management.md)
→ [Flux](diagrams/flows/campaign-management.md)

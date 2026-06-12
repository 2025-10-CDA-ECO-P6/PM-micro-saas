> Fichier à renommer `space-management.md` — différé, voir plan W2

# Space Management

> **Responsabilité** : gérer les espaces de jeu (campagnes, one-shots et espace personnel), leur configuration,
> leurs membres, leurs accès invités et leurs invitations.
> Ce contexte est le gardien de l'accès : il décide qui peut entrer dans quel espace et avec quel niveau d'accès.
>
> Il ne gère pas le contenu (documents, scénarios → Content Library),
> ni le déroulement de session (→ Session Conduct).
> Il fournit les identités d'accès que ces contextes consomment.

---

## Ce que ce contexte fait

- Créer et configurer un espace (campagne, one-shot ou espace personnel).
- Gérer les membres : inviter, associer à un personnage, retirer.
- Gérer les co-MJs (rôle GM).
- Créer et révoquer des invitations (par lien ou email).
- Gérer les accès invités sans compte (GuestAccess).
- Convertir un GuestAccess en SpaceMembership lors de la création de compte.
- Géler les espaces excédentaires lors d'un downgrade de tier.

## Ce que ce contexte ne fait PAS

| Responsabilité | Contexte propriétaire |
|---|---|
| Contenu de l'espace (documents, scénarios, scènes) | Content Library |
| Déroulement de session (LIVE, notes de session, partage) | Session Conduct |
| Comptes utilisateurs, authentification | Identity & Access |
| Gestion des personnages joueurs (fiche complète) | Content Library |

---

## Agrégats

### Space (agrégat principal)

Frontière de cohérence pour les membres et les invitations. Représente indifféremment une campagne longue, un one-shot ou un espace personnel — la différence est portée par `type`.

| Champ | Type | Description |
|---|---|---|
| `id` | `SpaceId` | |
| `ownerId` | `UserId` | Propriétaire technique unique — responsabilité billing et RGPD. FK réelle vers `users` ([ADR-009](../../architecture/decisions/ADR-009-fk-campaign-owner.md)). |
| `name` | `string` | Nom de l'espace |
| `slug` | `Slug` | Unique par propriétaire |
| `type` | `SpaceType` | `CAMPAIGN` \| `ONE_SHOT` \| `PERSONAL` |
| `status` | `SpaceStatus` | `ACTIVE` \| `ARCHIVED` \| `FROZEN` |
| `memberships` | `SpaceMembership[]` | Entités enfants |
| `invitations` | `Invitation[]` | Entités enfants — non applicables pour `PERSONAL` (voir invariant 13) |
| `createdAt` | `DateTime` | |
| `updatedAt` | `DateTime` | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(ownerId, name, type)` | `SpaceCreated` | Crée l'espace. L'owner est automatiquement ajouté comme membre OWNER. |
| `AddMember(userId, role)` | `MemberJoined` | Ajoute un membre (GM ou PLAYER). Bloqué pour `type = PERSONAL` (voir invariant 13). |
| `RemoveMember(userId)` | `MemberRemoved` | Retire un membre. Bloqué si userId == ownerId. |
| `CreateInvitation(type, scope, options)` | `InvitationCreated` | Crée une invitation enfant. Bloqué pour `type = PERSONAL` (voir invariant 13). |
| `RevokeInvitation(invitationId)` | `InvitationRevoked` | Passe l'invitation en REVOKED. |
| `AssociateCharacter(userId, characterId)` | `CharacterAssociated` | Associe un personnage (référence Content Library) à un membre. Le paramètre `characterId` est un `DocumentId` pointant vers un `Document` de type `player_character`. |
| `Archive()` | `SpaceArchived` | Archivage manuel par le MJ. Irréversible (MVP). |
| `Freeze()` | `SpaceFrozen` | Gel automatique lors d'un downgrade de tier. Passe en lecture seule. |
| `Unfreeze()` | `SpaceUnfrozen` | Dégel lors d'un upgrade de tier. |
| `Delete()` | `SpaceDeleted` | Suppression d'un espace. Matérialise le contrat présupposé par ADR-010/011 (saga de suppression/purge) — absent du modèle domaine avant ADR-018. |

---

### SpaceMembership (entité dans Space)

> Dans l'Ubiquitous Language, un `SpaceMembership` avec `status = ACTIVE` est appelé **membre**
> dans tous les UC/US. `SpaceMembership` est le terme technique interne.

| Champ | Type | Description |
|---|---|---|
| `userId` | `UserId` | |
| `role` | `MemberRole` | `OWNER` \| `GM` \| `PLAYER` |
| `status` | `MembershipStatus` | `PENDING` \| `ACTIVE` \| `REMOVED` |
| `characterIds` | `DocumentId[]` | Références vers les personnages (Content Library) — alias sémantique vers des `Document` de type `player_character` ; la cohérence de type est garantie par validation runtime. |
| `joinedAt` | `DateTime` | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Activate()` | `MemberActivated` | Passe le statut de `PENDING` à `ACTIVE` lors de l'utilisation du lien d'invitation, une fois sa validité vérifiée. |

**Rôles**

| Rôle | Cardinalité | Droits |
|---|---|---|
| `OWNER` | Exactement 1 | Tout : supprimer l'espace, gérer les GMs, responsabilité billing/RGPD |
| `GM` | 0..* | Gérer le contenu, lancer/piloter des sessions, inviter des joueurs — ne peut pas supprimer l'espace ni gérer d'autres GMs |
| `PLAYER` | 0..* | Accès aux contenus partagés, consultation de sa fiche personnage |

> Le MJ propriétaire est aussi en membership avec `role = OWNER` pour la cohérence des requêtes.
> `ownerId` sur `Space` reste le champ de responsabilité technique (billing, RGPD).

---

### Invitation (entité dans Space)

| Champ | Type | Description |
|---|---|---|
| `id` | `InvitationId` | |
| `token` | `InvitationToken` | Identifiant du lien d'invitation, globalement unique, non prédictible. |
| `type` | `InvitationType` | `LINK` \| `EMAIL` |
| `scope` | `InvitationScope` | `CAMPAIGN` \| `SESSION` |
| `sessionId` | `SessionId?` | Renseigné si scope = SESSION |
| `expiresAt` | `DateTime?` | |
| `maxUses` | `int?` | |
| `usedCount` | `int` | |
| `status` | `InvitationStatus` | `ACTIVE` \| `REVOKED` \| `EXPIRED` |
| `createdAt` | `DateTime` | |

> Une invitation est le mécanisme d'entrée. Son utilisation crée soit un `SpaceMembership`
> (utilisateur connecté), soit un `GuestAccess` (utilisateur anonyme).
> Cette orchestration est applicative — l'espace ne crée pas directement ces objets.

---

### GuestAccess (agrégat séparé)

Représente l'accès d'un joueur sans compte. Agrégat indépendant car son cycle de vie
(expiration, conversion) est orthogonal à celui de l'espace, et il est référencé
depuis Session Conduct par son lien d'accès.

| Champ | Type | Description |
|---|---|---|
| `id` | `GuestAccessId` | |
| `spaceId` | `SpaceId` | |
| `scope` | `GuestAccessScope` | `SESSION` \| `CAMPAIGN` |
| `sessionId` | `SessionId?` | Renseigné si scope = SESSION |
| `token` | `GuestAccessToken` | Identifiant du lien d'accès invité, globalement unique, non prédictible. |
| `displayName` | `string` | Saisi par le joueur à l'arrivée |
| `characterId` | `DocumentId?` | Associé par le MJ (référence Content Library) — alias sémantique vers un `Document` de type `player_character` ; la cohérence de type est garantie par validation runtime. |
| `status` | `GuestAccessStatus` | `ACTIVE` \| `EXPIRED` \| `REVOKED` \| `CONVERTED` |
| `expiresAt` | `DateTime?` | Calculé depuis la fermeture de session + 24h pour scope SESSION |
| `createdAt` | `DateTime` | |

**Méthodes**

| Méthode | Événement produit | Description |
|---|---|---|
| `Create(spaceId, scope, sessionId?)` | `GuestAccessCreated` | |
| `SetDisplayName(name)` | — | Saisi par le joueur à l'arrivée |
| `AssociateCharacter(characterId)` | — | Effectué par le MJ |
| `Expire()` | `GuestAccessExpired` | Déclenché automatiquement après session + 24h |
| `Revoke()` | `GuestAccessRevoked` | Révocation manuelle par le MJ |
| `Convert(userId)` | `GuestAccessConvertedToMember` | Quand l'invité crée un compte |

---

### ScenarioLibrary et ScenarioLibraryEntry (post-MVP)

> **Statut : post-MVP — modélisé, non implémenté.** Ce concept est présent dans le modèle
> pour éviter une migration structurelle ultérieure, au même titre que la valeur `EMAIL` de
> `InvitationType` conservée inactive en MVP. Son implémentation est conditionnée à UC-13.

La `ScenarioLibrary` représente la bibliothèque de scénarios réutilisables au niveau du compte MJ,
transverse aux espaces. Elle appartient à **Space Management** car elle opère au niveau du
compte MJ (comme les quotas et l'`ownerId`), et non au niveau d'un espace donné —
ce qui dépasse les responsabilités de Content Library (toujours espace-scoped).

> **Point à confirmer (ADR-018)** : l'introduction de l'espace `PERSONAL` comme conteneur par défaut
> du contenu hors d'un espace de jeu partagé soulève la question de l'attribution de la `ScenarioLibrary` sous ce
> nouveau paradigme (ADR-018 la signale potentiellement subsumée). Ce point n'est pas résolu ici —
> la `ScenarioLibrary` est conservée dans ce contexte sans modification ; la question d'attribution
> est à trancher en W2.

#### ScenarioLibraryEntry (agrégat dans Space Management)

Chaque entrée représente la promotion d'un scénario (`Document` de type `SCENARIO`) vers la
bibliothèque personnelle du MJ propriétaire du compte.

| Champ | Type | Description |
|---|---|---|
| `id` | `ScenarioLibraryEntryId` | Identifiant unique de l'entrée |
| `ownerId` | `UserId` | Compte MJ propriétaire de la bibliothèque |
| `documentId` | `DocumentId` | Référence vers le Document de type `SCENARIO` promu (Content Library) |
| `promotedAt` | `DateTime` | Date de promotion dans la bibliothèque |

> **Point ouvert (W2)** : le champ `ownerId` de `ScenarioLibraryEntry` utilise `UserId` alors que
> certains documents dans le corpus utilisent `userId` pour le même concept — l'incohérence
> `ownerId`/`userId` est à résoudre en W2 (hors périmètre de cette réécriture).

**Règles associées (post-MVP)**

- Un `Document` ne peut être promu que s'il a `isReusable = true` et `documentTypeId = SCENARIO`.
- Une entrée de bibliothèque est liée à l'`ownerId` — elle n'est pas transférable.
- La suppression du document source retire l'entrée de bibliothèque correspondante.
- L'instanciation d'un scénario depuis la bibliothèque reste dans `Document.Instantiate()` (Content Library).

---

## Value Objects

| VO | Validation | Description |
|---|---|---|
| `InvitationToken` | Globalement unique, non prédictible | Identifiant porté par le lien d'invitation. Généré à la création. Non modifiable. |
| `GuestAccessToken` | Globalement unique, non prédictible | Identifiant porté par le lien d'accès invité. Généré à la création. Non modifiable. |

---

## Enums

| Enum | Valeurs |
|---|---|
| `SpaceType` | `CAMPAIGN` \| `ONE_SHOT` \| `PERSONAL` |
| `SpaceStatus` | `ACTIVE` \| `ARCHIVED` \| `FROZEN` |
| `MemberRole` | `OWNER` \| `GM` \| `PLAYER` |
| `MembershipStatus` | `PENDING` \| `ACTIVE` \| `REMOVED` |
| `InvitationType` | `LINK` \| `EMAIL` — `EMAIL` est hors périmètre MVP (US-UC-11 stories exclues) ; la valeur est conservée pour éviter une migration ultérieure. |
| `InvitationScope` | `CAMPAIGN` \| `SESSION` |
| `InvitationStatus` | `ACTIVE` \| `REVOKED` \| `EXPIRED` |
| `GuestAccessScope` | `SESSION` \| `CAMPAIGN` |
| `GuestAccessStatus` | `ACTIVE` \| `EXPIRED` \| `REVOKED` \| `CONVERTED` |

> **`SpaceType.PERSONAL`** : `PERSONAL` — espace personnel du propriétaire, conteneur par défaut du
> contenu hors d'un espace de jeu partagé. Mono-membre (le propriétaire `OWNER` uniquement). Non partageable.

---

## One-shot — spécificités

`ONE_SHOT` est un `Space` avec `type = ONE_SHOT`. Les différences sont comportementales, pas structurelles.

| Comportement | `CAMPAIGN` | `ONE_SHOT` | `PERSONAL` |
|---|---|---|---|
| Parcours de création | Configuration complète | Express — nom + scénario, aucun membre requis *(post-MVP)* | Automatique à la création du compte (voir invariant 14) |
| Membres permanents | Attendus | Optionnels — GuestAccess typique, mais SpaceMembership possible | Aucun — propriétaire uniquement (voir invariant 13) |
| Sessions | Multiples | Une seule attendue (non forcée techniquement) | Non applicable (conteneur de contenu, pas de session) |
| Archivage | Manuel par le MJ | Manuel par le MJ | À préciser — voir NOTE (invariant 14, point i) |
| Invitations | Oui | Oui | Non (voir invariant 13) |

**MVP** : Au MVP, aucune différence comportementale n'existe entre `CAMPAIGN` et `ONE_SHOT` — création, structure des dossiers, cycle de session et vue session sont identiques pour les deux types. Le parcours de création express est une caractéristique cible post-MVP (arbitrage UC-13, vision-produit §5bis). L'espace `PERSONAL` est créé automatiquement à la création du compte.

---

## Invariants métier

1. Un `Space` a toujours exactement un membre avec `role = OWNER`.
2. L'OWNER ne peut pas être retiré de son espace (MVP : blocage).
3. Un `UserId` ne peut avoir qu'un seul `SpaceMembership` actif par espace.
4. L'identifiant porté par le lien d'invitation est globalement unique.
5. L'identifiant porté par le lien d'accès invité est globalement unique.
6. Un utilisateur FREE ne peut pas créer un 4e espace actif de type `CAMPAIGN` ou `ONE_SHOT` — la création est bloquée avec invitation à upgrader. L'espace `PERSONAL` par défaut n'est pas décompté du quota.
7. Un `Space` avec `status = FROZEN` refuse toute écriture (lecture seule). Seule `Unfreeze()` est autorisée.
8. Un `GuestAccess` avec `status = CONVERTED` ne peut plus être utilisé pour accéder à l'espace.
9. Un `GuestAccess` avec `status = EXPIRED` ou `REVOKED` ne donne plus accès.
10. Un `DocumentId` référençant un personnage ne peut être associé qu'à un seul `SpaceMembership` actif à la fois dans un espace (RB-11-18). Le `DocumentId` associé doit référencer un `Document` de type `player_character` — cette validation est appliquée à l'écriture dans l'invariant de domaine de `SpaceMembership`. La même contrainte s'applique au champ `characterId` de `GuestAccess` : le `DocumentId` fourni doit également référencer un `Document` de type `player_character`.
11. Pour un `Space` dont l'`ownerId` référence un utilisateur FREE, le propriétaire ne peut pas accorder l'accès à une session à plus de **4 joueurs distincts**, MJ non compté. Tout octroi d'accès supplémentaire — quelle qu'en soit la forme (`SpaceMembership` PLAYER/GM ou `GuestAccess`) — est refusé au moment de l'octroi dès que cette limite est atteinte. L'intention est d'éviter tout contournement par composition entre les types d'accès existants et futurs. **À préciser à la modélisation** : la sémantique exacte de comptage inter-types (un membre permanent compte-t-il une fois par espace ou par session ? extensibilité aux types d'accès futurs) est à affiner ; la règle de besoin est portée par UC-09 / RB-09-21, qui fait foi.
12. *(post-MVP)* Une `ScenarioLibraryEntry` est unique par `(ownerId, documentId)` : un même document ne peut être promu qu'une seule fois dans la bibliothèque personnelle d'un propriétaire (RB-13-03 / US-13).
13. Un espace `PERSONAL` est mono-membre : seul le propriétaire (`role = OWNER`) y est membre. Les opérations `AddMember()`, `CreateInvitation()` et tout octroi de `GuestAccess` sont bloqués pour `type = PERSONAL`.
14. Un espace `PERSONAL` existe par défaut pour chaque propriétaire de compte. La règle de besoin est : à tout moment, un propriétaire dispose d'un et d'un seul espace `PERSONAL` actif. **Point d'articulation avec Identity & Access à confirmer** : le déclencheur exact de la création (événement ou étape du flux de création de compte) est à préciser avec le contexte Identity & Access — voir NOTE ci-dessous.

> **NOTE — points ouverts sur l'espace `PERSONAL`**
>
> (i) **Sémantique de `SpaceStatus` `ARCHIVED`/`FROZEN` pour un espace `PERSONAL`** — à préciser (voir tableau One-shot, colonne PERSONAL, ligne Archivage). Un espace `PERSONAL` est le conteneur par défaut du propriétaire ; les conséquences d'un archivage ou d'un gel sur ce cas particulier n'ont pas encore été arbitrées.
>
> (ii) **Déclencheur exact de la création de l'espace `PERSONAL`** — à préciser avec le contexte Identity & Access. Candidats : réaction à `UserRegistered`, étape synchrone dans le flux de création de compte, ou commande applicative dédiée. Ce point conditionne l'articulation entre les deux contextes (invariant 14).

---

## Règles métier

1. Seul l'OWNER peut inviter ou retirer des GMs.
2. Un GM peut inviter ou retirer des PLAYER.
3. Retirer un membre ne supprime pas ses données dans l'espace (personnages, notes partagées restent).
4. Un membre retiré peut être réinvité.
5. Un `GuestAccess SESSION` expire à la fermeture de la session + 24h de grâce.
6. Quand `AccountTierChanged` (PRO → FREE) et que le MJ propriétaire a > 3 espaces `CAMPAIGN` ou `ONE_SHOT` à l'état `ACTIVE` (l'espace `PERSONAL` n'est pas compté) : les espaces excédentaires sont gelés dans l'ordre de création (les plus récents en premier).
7. L'archivage est manuel et définitif (MVP). Un espace archivé est en lecture seule.
8. Un one-shot peut avoir simultanément des `SpaceMembership` (joueurs avec compte) et des `GuestAccess` (joueurs sans compte).
9. Lors de la fin définitive d'un `GuestAccess` (expiration après grâce ou révocation sans réactivation), les données personnelles qu'il porte (`displayName`, élément d'accès) cessent immédiatement d'être utilisées et affichées — plus aucune finalité produit. Leur effacement effectif intervient au plus tard 90 jours après la fin d'accès, fenêtre bornée dont la seule finalité est l'exercice des droits de l'invité et le traitement des contestations (RGPD Art. 5(1)(e) — limitation de la conservation). Si l'invité a été converti en compte, ses données suivent les règles du compte.
10. À la fin définitive d'un `GuestAccess` non converti, les notes `PLAYER_PRIVATE` créées par cet invité sont supprimées physiquement — uniquement les siennes, jamais celles d'autres participants. Cette suppression répond à la même obligation légale que l'effacement des notes à la suppression d'un compte (RGPD Art. 17 ; cohérence avec le domaine Identity & Access).

---

## Événements domaine

| Événement | Producteur | Consommateurs |
|---|---|---|
| `SpaceCreated` | `Space.Create()` | Content Library (créer les dossiers système), Application |
| `MemberJoined` | `Space.AddMember()` | Application (notification) |
| `MemberActivated` | `SpaceMembership.Activate()` | Session Conduct (autoriser l'accès en session), Application |
| `MemberRemoved` | `Space.RemoveMember()` | Session Conduct (retirer l'accès actif si session en cours) |
| `InvitationCreated` | `Space.CreateInvitation()` | Application (fourniture du lien au MJ) |
| `InvitationRevoked` | `Space.RevokeInvitation()` | Identity & Access (invalider le lien d'invitation immédiatement) |
| `CharacterAssociated` | `Space.AssociateCharacter()` | Session Conduct (accès aux notes PLAYER_PRIVATE du personnage) |
| `SpaceFrozen` | `Space.Freeze()` | Application (notification au MJ) |
| `SpaceUnfrozen` | `Space.Unfreeze()` | Application (notification au MJ) |
| `SpaceArchived` | `Space.Archive()` | Content Library, Session Conduct |
| `SpaceDeleted` | `Space.Delete()` | Content Library, Session Conduct, Application |
| `GuestAccessCreated` | `GuestAccess.Create()` | Session Conduct (accès aux informations partagées) |
| `GuestAccessExpired` | `GuestAccess.Expire()` | Session Conduct |
| `GuestAccessRevoked` | `GuestAccess.Revoke()` | Session Conduct (couper l'accès actif en session si présent) |
| `GuestAccessConvertedToMember` | `GuestAccess.Convert(userId)` | Application (finalise la création de compte), Session Conduct |

---

## Intégration avec les autres contextes

### Ce que Space Management reçoit

| Événement / Requête | Source | Action |
|---|---|---|
| `AccountTierChanged` | Identity & Access | Geler les espaces excédentaires si downgrade |
| `UserDeleted` | Identity & Access | Anonymiser les données nominatives des memberships |
| `DisplayNameUpdated` | Identity & Access | Mettre à jour le nom d'affichage dans les vues membres |

### Ce que Space Management publie

- `SpaceId`, `GuestAccessId`, `MemberRole` comme identifiants de référence pour les autres contextes.
- Les événements listés ci-dessus.

### Utilisation par les autres contextes

| Contexte | Usage |
|---|---|
| Content Library | `SpaceId` pour scoper les documents ; `SpaceCreated` pour créer les dossiers système |
| Session Conduct | `SpaceId`, `GuestAccessId`, `MemberRole` pour les autorisations d'accès en session |

---

## Diagrammes

→ [Classes](diagrams/classes/space-management.md)
→ [MCD](diagrams/mcd/space-management.md)
→ [MLD](diagrams/mld/space-management.md)
→ [Flux](diagrams/flows/space-management.md)

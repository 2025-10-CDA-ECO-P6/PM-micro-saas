# ADR-014 — Modèle d'autorisation API (appartenance ressource↔campagne)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (cadrage P1)
- **Findings liés** : F-05 (IDOR), F-07 (conversion invité→User), H-06 (anti-fuite SignalR)

---

## Périmètre de cet ADR

Cet ADR spécifie le modèle d'autorisation au niveau de la couche Application : comment déterminer qu'un appelant a le droit d'accéder à une ressource donnée, et comment ce gate est systématisé sur tous les endpoints et le canal SignalR.

**Hors périmètre de cet ADR** (renvois explicites) :
- Rate limiting token (F-06) → **[ADR-015](ADR-015-securite-authentification-mvp.md)** (résolu)
- Politique mot de passe, JWT, OAuth (F-02, F-10, F-11) → **[ADR-015](ADR-015-securite-authentification-mvp.md)** (résolu)
- Détail runtime SignalR (groupes, révocation en cours de session `GuestAccessRevoked`/`GuestAccessExpired`) → **B1.7**
- Contrat OpenAPI, codes 403/404 → **B1.10**
- Query filters EF Core globaux (soft-delete documents/campagnes) → **B5.1**
- Tests IDOR → **B5.2**
- Tests anti-fuite SignalR → **B8.2**

---

## Contexte

ADR-007 a posé le principe : « toute requête portant sur une ressource vérifie que cette ressource appartient à une campagne accessible à l'appelant ». Ce principe n'avait pas été développé en un modèle concret. Finding F-05 identifie l'IDOR (Insecure Direct Object Reference) comme risque réel dans un contexte solo : sans mécanisme centralisé, chaque handler doit individuellement vérifier l'appartenance — précisément la discipline que F-05 juge insuffisante à l'échelle.

Ce constat appelle deux décisions complémentaires : (1) un modèle complet du principal et de la composition des prédicats d'accès, (2) une systématisation centralisée qui rende la vérification non contournable par discipline.

---

## Frontière fondamentale — deux couches d'autorisation

ADR-014 repose sur une frontière stricte entre deux couches. Les confondre ou les dupliquer crée deux sources de vérité.

**Couche Application — appartenance ressource↔campagne** (objet de cet ADR)
La couche Application répond à la question : « L'appelant a-t-il accès à la *campagne* de cette ressource ? » C'est le gate antérieur, commun à toutes les ressources d'une campagne, indépendant du type de document.

**Couche Domaine — visibilité document** (hors périmètre de cet ADR)
Le domaine répond à la question : « Ce document précis est-il *visible* pour cet appelant ? » Les règles sont encodées dans `Document.CanBeReadBy(userId, memberRole, documentType)` (Content Library — signature domaine souveraine sur sa propre forme). ADR-014 délègue la visibilité à cette méthode ; il ne la ré-exprime pas.

Règles `CanBeReadBy` actées (décision opérateur, alignée Content Library) :
- `PUBLIC` : lisible par tous les membres actifs et les invités selon leur scope.
- `GM_ONLY` : lisible par `OWNER` et `GM` uniquement.
- `PLAYER_PRIVATE` : lisible par l'auteur seul (`createdById` / `guest_access_id`) — `OWNER` et `GM` exclus, **pour tout type de document** (il n'y a pas d'exception par type ; la règle est uniforme).

**Décision actée** : `PLAYER_PRIVATE` est auteur-seul, MJ exclu, uniformément. Cette règle est portée par le domaine `CanBeReadBy` (mis à jour en parallèle dans Content Library). Toute révision ultérieure des règles de visibilité est un changement **domaine**, hors périmètre de cet ADR.

---

## Décision

### 1. Modèle du principal

Un appelant authentifié peut être de deux natures.

**Membre (utilisateur avec compte)**

Un utilisateur est « membre actif » d'une campagne si et seulement si :

```
campaign_memberships.campaign_id = <campagne cible>
AND campaign_memberships.user_id = <userId appelant>
AND campaign_memberships.status = 'ACTIVE'
```

Son rôle (`OWNER` / `GM` / `PLAYER`) est lu depuis `campaign_memberships.role`. Ce rôle est transmis à `Document.CanBeReadBy` pour résoudre la visibilité.

**Invité (GuestAccess, sans compte)**

Un invité est « actif » sur une campagne si et seulement si :

```
guest_accesses.campaign_id = <campagne cible>
AND guest_accesses.id = <guestAccessId appelant>
AND guest_accesses.status = 'ACTIVE'
AND (guest_accesses.expires_at IS NULL OR guest_accesses.expires_at > now())
```

Son **scope** (`CAMPAIGN` ou `SESSION`) détermine quelles ressources entrent dans son périmètre d'appartenance (§4 — matrice ressource).

**Statut `CONVERTED`** : un `GuestAccess` avec `status = 'CONVERTED'` correspond à un invité dont le compte a été créé mais dont le `CampaignMembership` n'a pas encore été activé par un `OWNER` ou `GM`. Pendant cette fenêtre, l'appelant conserve le **niveau d'accès GuestAccess d'origine** (scope limité de l'accès invité d'origine), pas le niveau `PLAYER`. Le saut de privilège est interdit : le statut `CONVERTED` ne satisfait pas la condition `status = 'ACTIVE'` du prédicat membre. L'élévation au rôle `PLAYER` requiert l'activation explicite du `CampaignMembership` — cohérent avec §5 / F-07.

---

### 2. Contexte de lecture unifié — composition des quatre prédicats

Toute lecture d'une ressource de campagne compose, en ET (non en OU), les quatre prédicats suivants :

| # | Prédicat | Portée | Mécanisme |
|---|---|---|---|
| P1 | **Appartenance** : membre ACTIVE ou GuestAccess actif dont le scope inclut la ressource | Dépend de l'appelant | Prédicat Application (§1 + §4) |
| P2 | `campaigns.deleted_at IS NULL` ET `campaigns.purge_claimed_at IS NULL` | Indépendant de l'appelant | Query filter EF Core global (→ B5.1) |
| P3 | `documents.is_deleted = false` | Indépendant de l'appelant | Query filter EF Core global (→ B5.1) |
| P4 | **Visibilité** : `Document.CanBeReadBy(...)` | Dépend de l'appelant et du type | Délégation domaine |

Les prédicats P2 et P3 sont structurels et indépendants de l'appelant : ils sont portés par des **query filters EF Core globaux** (→ B5.1). Le prédicat P1 (appartenance) dépend de l'appelant et est un **prédicat Application** évalué par le service de décision centralisé (§3). Le prédicat P4 est délégué au domaine et co-évalué par ce même service.

**Cas restauration campagne en corbeille** : l'endpoint de restauration est réservé à l'`OWNER` et distinct du chemin de lecture normal. Il vérifie `deleted_at IS NOT NULL` et la borne de restauration définie en B1 (→ ADR-011 pour la valeur) — c'est le seul chemin autorisé à traverser P2. Le chemin de lecture normal ne laisse jamais passer une campagne en corbeille.

---

### 3. Systématisation centralisée — service de décision et pipeline behavior MediatR

Les prédicats P1 (appartenance) et P4 (visibilité domaine) sont **composés en un point unique** dans la couche Application : le service `IResourceAccessPolicy`.

**Service `IResourceAccessPolicy`**

```
IResourceAccessPolicy.CanAccess(principal, resourceRef) : AccessDecision
```

Ce service est un **service de décision pur de la couche Application**. Il compose :
- **P1** — appartenance : l'appelant est membre actif ou GuestAccess actif dont le scope inclut la ressource (§1 + §4).
- **P4** — visibilité domaine : délégation à `Document.CanBeReadBy(...)`.

`AccessDecision` est une valeur discriminée : `Allowed` / `Denied(reason)`. Le service ne produit pas d'effets secondaires.

**Motif** : F-05 identifie la discipline par-endpoint comme vecteur d'IDOR. En ne centralisant que P1 (comme dans la version précédente), le trou de discipline se déplace vers P4 — re-appliqué par chaque handler sur les documents résolus via session. En ne nommant qu'un « prédicat partagé » REST/SignalR sans le réifier, le risque est deux implémentations divergentes. `IResourceAccessPolicy` ferme les deux trous : P1+P4 ensemble, un seul type.

**Deux points d'appel, un seul service**

1. **Pipeline behavior MediatR (REST — pull)** : tout `IRequest` portant sur une ressource de campagne implémente `ICampaignScopedRequest` (déclarant son `CampaignId` direct ou résolu). Le behavior intercepte la requête, appelle `IResourceAccessPolicy.CanAccess(...)`, et retourne `403`/`404` avant d'atteindre le handler si la décision est `Denied`. Les handlers ne font pas cette vérification en doublon.

2. **Filtre de diffusion SignalR (push, par message, → B1.7)** : avant de pousser un message vers un abonné, le filtre appelle le **même** `IResourceAccessPolicy.CanAccess(...)`. Il n'y a pas de diffusion indifférenciée par groupe campagne. REST et SignalR partagent le même type — non-divergence structurelle garantie.

La résolution transitive du `campaign_id` (§4) fait partie du contrat d'implémentation de chaque requête / chaque message SignalR.

**Test d'archi CI** (→ **B3.2**) : le test d'architecture doit couvrir deux catégories de handlers :
- **Handlers scopés** (`ICampaignScopedRequest`) : le test vérifie qu'ils passent tous par le pipeline behavior / `IResourceAccessPolicy`. Tout handler scopé qui échapperait au pipeline est un échec CI.
- **Handlers non-scopés** (recherche globale, liste « mes campagnes », dashboard — sans `campaign_id` unique) : ces handlers n'implémentent pas `ICampaignScopedRequest` et filtrent directement sur `campaign_memberships.user_id = appelant`. Le test doit **catégoriser** les handlers et vérifier que les non-scopés filtrent bien sur l'appartenance membre — sinon angle mort. La liste des handlers non-scopés autorisés est renvoyée à B3.2 pour définition exhaustive.

---

### 4. Matrice ressource → règle d'appartenance

Pour chaque type de ressource accessible par l'API, le chemin de résolution vers `campaign_id` et les contraintes supplémentaires d'appartenance invité.

**Ressources avec `campaign_id` direct**

| Ressource | Chemin vers `campaign_id` | Invité scope CAMPAIGN | Invité scope SESSION |
|---|---|---|---|
| `campaigns` | `campaigns.id` | Oui (lecture métadonnées publiques) | Non |
| `documents` | `documents.campaign_id` | Oui + visibilité domaine | Restreint aux documents de sa session (via `session_pinned_documents`, `session_live_notes` ou `scenario_id` — y compris sur accès direct par ID) |
| `folders` | `folders.campaign_id` | Oui | Non (pas de navigation bibliothèque hors session) |
| `sessions` | `sessions.campaign_id` | Oui | Uniquement `guest_accesses.session_id` |
| `invitations` | `invitations.campaign_id` | Non (lecture MJ uniquement) | Non |
| `guest_accesses` | `guest_accesses.campaign_id` | Non (gestion MJ uniquement) | Non |
| `campaign_memberships` | `campaign_memberships.campaign_id` | Non (gestion MJ uniquement) | Non |
| `session_view_configs` | `session_view_configs.campaign_id` | Oui (si session accessible) | Oui (session propre) |
| `document_types` (custom) | `document_types.campaign_id` | Non (gestion MJ uniquement) | Non |
| `membership_characters` | `membership_characters.campaign_id` | Non (gestion MJ) | Non (gestion MJ) — lecture : joueur concerné uniquement |

**Ressources avec résolution transitive**

| Ressource | Chemin de résolution | Contrainte supplémentaire |
|---|---|---|
| `document_blocks` | `document_blocks.document_id` → `documents.campaign_id` | + visibilité domaine du document parent |
| `document_links` | `document_links.source_document_id` → `documents.campaign_id` **ET** `document_links.target_document_id` → `documents.campaign_id` | **Appartenance bilatérale + invariant domaine** : un lien cross-campagne est **rejeté au niveau domaine** — `Document.LinkDocument(...)` lève un invariant si `source` et `target` n'appartiennent pas à la même campagne. Il n'existe donc pas de lien dont `source` et `target` seraient dans des campagnes différentes. La couche Application n'a pas à gérer ce cas : l'invariant le ferme à la source (→ P4 implémentation en **B4**). |
| `document_tags` | `document_tags.document_id` → `documents.campaign_id` | + visibilité domaine du document parent |
| `session_pinned_documents` | `session_pinned_documents.session_id` → `sessions.campaign_id` | **Re-applique `Document.CanBeReadBy` via `IResourceAccessPolicy`** — un `GM_ONLY` épinglé ne transite pas vers un joueur/invité |
| `session_live_notes` | `session_live_notes.session_id` → `sessions.campaign_id` ET `session_live_notes.document_id` → `documents.campaign_id` | **Re-applique `Document.CanBeReadBy` via `IResourceAccessPolicy`** — un `PLAYER_PRIVATE` (auteur-seul) n'est jamais lisible que par son auteur, sur tous les chemins |
| `session_view_folders` | `session_view_folders.session_view_config_id` → `session_view_configs.campaign_id` ET `session_view_folders.folder_id` → `folders.campaign_id` | Les deux chemins doivent pointer vers la même campagne |

**Point dur — session_pinned_documents et session_live_notes** : la résolution d'un document via une session ne court-circuite pas la visibilité. Le gate d'appartenance (P1 — l'appelant est dans la session) est satisfait, mais P4 (`Document.CanBeReadBy`) est co-évalué par `IResourceAccessPolicy` sur le document lui-même sur tous les chemins. Un `GM_ONLY` épinglé en session reste invisible à un joueur ou à un invité. Un document `PLAYER_PRIVATE` (qu'il soit de type `LIVE_NOTE` ou autre) n'est jamais lisible que par son auteur — la règle est uniforme ; `IResourceAccessPolicy` la garantit sur tous les chemins, y compris la résolution via session.

---

### 5. Droits de lecture et d'écriture des invités

**Lecture**

Un invité accède aux ressources visibles selon son scope et la visibilité domaine :
- Invité scope `CAMPAIGN` : documents `PUBLIC` de la campagne + ses propres documents `PLAYER_PRIVATE` (`guest_access_id` = lui-même).
- Invité scope `SESSION` : documents `PUBLIC` accessibles dans sa session (via `session_pinned_documents`, `session_live_notes` ou `scenario_id`) + ses propres documents `PLAYER_PRIVATE`. Cette contrainte de session s'applique y compris sur un accès direct par ID (`GET /documents/{id}`) — P1 inclut la vérification que le document est rattaché à la session de l'invité, pas seulement que `campaign_id` correspond.
- `GM_ONLY` : jamais accessible à un invité.
- `PLAYER_PRIVATE` d'autrui : jamais accessible à un invité (règle auteur-seul uniforme — voir §Frontière fondamentale).

**Écriture**

Un invité peut créer et éditer exclusivement ses propres `LIVE_NOTE` : documents de type `live_note` avec `visibility = PLAYER_PRIVATE` et `guest_access_id` = son propre identifiant. Aucune autre écriture n'est autorisée.

**Conversion invité → User (F-07)**

Pendant la fenêtre entre la création de compte et l'activation du `CampaignMembership` par le MJ, le compte nouvellement créé hérite du niveau `GuestAccess` (scope limité de l'accès invité d'origine). Il n'obtient pas automatiquement le niveau `PLAYER`. Le saut de privilège est interdit : l'élévation au rôle `PLAYER` requiert l'activation explicite du `CampaignMembership` par un `OWNER` ou `GM`.

---

### 6. Canal SignalR — service partagé

La fonction de décision « cet appelant peut voir cette ressource » est portée par `IResourceAccessPolicy.CanAccess(principal, resourceRef)` (§3), consommé à deux points d'application :

1. **Filtre de requête REST (pull)** : le pipeline behavior MediatR (§3) appelle `IResourceAccessPolicy` — compose P1 + P4 en un seul point.
2. **Filtre de diffusion SignalR (push, par message, → B1.7)** : avant de pousser un message vers un abonné, le filtre SignalR appelle le **même** `IResourceAccessPolicy.CanAccess(...)`. Il n'y a pas de diffusion indifférenciée par groupe campagne.

REST et SignalR partagent le même service, le même type — non-divergence structurelle garantie (pas deux implémentations du prédicat P1+P4 qui pourraient diverger).

La propagation de révocation en cours de session (`GuestAccessRevoked`, `GuestAccessExpired`) et le détail du runtime SignalR (groupes, reconnexion, état de session) sont **renvoyés à B1.7**. ADR-014 pose le service partagé ; B1.7 configure le runtime.

**Conséquence** : un message `GM_ONLY` ne peut jamais être poussé vers un joueur ou un invité, même s'ils sont abonnés au même hub de campagne. Un document `PLAYER_PRIVATE` ne peut jamais être poussé vers un autre appelant que son auteur.

---

### 7. Visibilité MJ et PLAYER_PRIVATE

**Décision actée** : `PLAYER_PRIVATE` est auteur-seul, MJ (`OWNER`/`GM`) exclu, uniformément pour tout type de document. L'ancienne distinction `PLAYER_PRIVATE` régulier (lisible MJ) / `LIVE_NOTE` auteur-seul disparaît : la règle auteur-seul est la règle générale, portée par `Document.CanBeReadBy` (mis à jour en parallèle dans Content Library).

`IResourceAccessPolicy` garantit l'application de cette règle sur tous les chemins — REST (pipeline behavior) et SignalR (filtre de diffusion) — y compris pour les documents résolus via session (`session_pinned_documents`, `session_live_notes`). Un `PLAYER_PRIVATE` n'est jamais lisible que par son auteur, quel que soit le chemin d'accès.

Toute révision ultérieure de cette règle est un changement **domaine**, hors périmètre de cet ADR.

---

## Alternatives considérées

**Vérification d'appartenance par discipline de handler**
Chaque handler vérifie lui-même l'appartenance. Rejeté : F-05 identifie précisément ce modèle comme vecteur d'IDOR. Un handler oublié ou mal implémenté suffit à ouvrir une fuite. Aucun test d'archi ne peut garantir l'exhaustivité sans la contrainte structurelle du pipeline behavior.

**Middleware ASP.NET au niveau HTTP**
Intercepter au niveau du middleware HTTP plutôt que MediatR. Écarté au profit du pipeline behavior : le middleware HTTP n'a pas accès au contexte applicatif résolu (type de ressource, `campaign_id` résolu transitivement). La résolution transitive nécessite une requête DB contextualisée — ce que MediatR fournit proprement dans la couche Application.

**Autorisation basée sur les rôles (RBAC) seul**
RBAC vérifie le rôle de l'utilisateur mais pas l'appartenance à la campagne spécifique. Un `PLAYER` d'une campagne A pourrait lire une ressource de la campagne B s'il en connaît l'identifiant. Rejeté : la contrainte d'appartenance ressource↔campagne est précisément ce que RBAC seul ne couvre pas.

---

## Conséquences

### Critères d'acceptation — cas IDOR de référence (→ B5.2 / B8.2)

Les scénarios suivants constituent les cas de test que B5.2 (tests IDOR REST) et B8.2 (tests anti-fuite SignalR) devront implémenter :

| Scénario | Résultat attendu |
|---|---|
| Appelant non-membre → lecture d'une ressource d'une autre campagne | 404 (ne révèle pas l'existence) ou 403 |
| Invité scope `SESSION` → ressource hors de sa session | 403 |
| Lecture d'une campagne avec `deleted_at IS NOT NULL` | Invisible (filtre P2) |
| Lecture d'une campagne avec `purge_claimed_at IS NOT NULL` | Invisible (filtre P2) |
| Joueur → document `GM_ONLY` | 403 |
| Invité → document `GM_ONLY` (y compris épinglé en session) | 403 |
| Invité → document `PLAYER_PRIVATE` d'un autre joueur | 403 |
| Invité réassocié au même `character_id` → `LIVE_NOTE` d'un compte supprimé | 403 (règle auteur-seul domaine + F-08 / ADR-012) |
| SignalR — push d'un message `GM_ONLY` vers un joueur abonné | Interdit (filtré avant diffusion) |
| SignalR — push vers un invité après révocation du `GuestAccess` | Interdit (→ B1.7 pour la coupure runtime) |

### Impact sur la couche Application

- Chaque `IRequest` portant sur une ressource de campagne déclare son `CampaignId` résolu (interface `ICampaignScopedRequest` ou équivalent).
- Le pipeline behavior appelle `IResourceAccessPolicy.CanAccess(...)` — seul point d'évaluation de P1+P4 sur le chemin REST. Les handlers ne font pas cette vérification en doublon.
- Le filtre de diffusion SignalR appelle le même `IResourceAccessPolicy.CanAccess(...)` — seul point d'évaluation de P1+P4 sur le chemin push.
- La résolution transitive du `campaign_id` (ex. `document_blocks` → `documents`) fait partie du contrat de chaque requête / message et peut nécessiter une pré-requête légère ou une jointure.

### Test d'archi (→ B3.2)

Un test ArchUnit (ou équivalent .NET) catégorise les handlers en deux familles et vérifie :
- **Handlers scopés** (`ICampaignScopedRequest`) : tous passent par le pipeline behavior / `IResourceAccessPolicy`. Tout ajout non conforme échoue en CI.
- **Handlers non-scopés** (liste exhaustive définie en B3.2 — ex. recherche globale, liste « mes campagnes ») : chacun filtre sur `campaign_memberships.user_id = appelant`. Tout handler non-scopé qui n'appliquerait pas ce filtre est un échec CI.

Ce test tourne en CI et garantit la non-régression structurelle de l'invariant. Sa définition exhaustive est renvoyée à B3.2.

### Fuite d'existence par backlink

Les requêtes de **backlinks** (`document_links.target_document_id = <docId>`) — c'est-à-dire « quels documents pointent vers ce document ? » — doivent appliquer `IResourceAccessPolicy`/`CanBeReadBy` sur chaque document **source** retourné. Un auteur d'un document cible ne doit pas pouvoir inférer l'existence d'un document `PLAYER_PRIVATE` qui le référence : si le document source n'est pas lisible par l'appelant (auteur-seul, `PLAYER_PRIVATE` d'autrui), il est exclu de la liste des backlinks retournée.

### Conformité RGPD

L'invariant d'appartenance garantit qu'un utilisateur ne peut pas accéder aux données d'une campagne à laquelle il n'appartient pas — ce qui inclut les données à caractère personnel portées par les documents, profils de personnages et `LIVE_NOTE`. Cet invariant contribue à l'obligation de minimisation et de contrôle d'accès (Art. 5(1)(f) RGPD).

---

## Points à trancher

Les éléments suivants sont explicitement renvoyés sans être traités dans cet ADR :

- ~~**B1.5**~~ → **[ADR-015](ADR-015-securite-authentification-mvp.md) (résolu)** — Rate limiting sur les endpoints d'authentification et de validation de token (F-06) ; politique mot de passe, JWT, OAuth (F-02, F-10, F-11).
- **B1.7** — Runtime SignalR : gestion des groupes, révocation en cours de session, reconnexion, propagation des événements `GuestAccessRevoked` / `GuestAccessExpired`.
- **B1.10** — Contrat OpenAPI : sémantique 403 vs 404 par endpoint (ne-pas-révéler-l'existence vs accès refusé connu), annotations de sécurité.
- **B3.2** — Définition et implémentation du test d'archi CI couvrant l'exhaustivité du pipeline behavior.
- **B5.1** — Configuration des query filters EF Core globaux pour P2 (`campaigns.deleted_at`, `campaigns.purge_claimed_at`) et P3 (`documents.is_deleted`).
- **B5.2** — Implémentation des tests IDOR (cas de référence listés dans la section Conséquences).
- **B8.2** — Implémentation des tests anti-fuite SignalR (cas de référence listés dans la section Conséquences).

---

## Compléments post-revue

*(Section réservée aux clarifications post-implémentation — vide à la date de l'ADR.)*

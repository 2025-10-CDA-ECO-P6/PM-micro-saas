# Contrat API — codes de statut, non-révélation d'existence, annotations sécurité

> Spec pré-build, cluster sécurité & contrats API. Dérivée fidèlement de **ADR-014** (Modèle d'autorisation API, l.101, l.240-247, l.266-268, l.282) et **ADR-015** (Sécurité authentification MVP, l.101, l.178-187, l.295). Statut ADR-015 : décision pré-implémentation, à confirmer à l'entrée en build (ADR-015, en-tête).
>
> **Portée** : cette spec formalise la sémantique des codes HTTP et les annotations de sécurité observables au niveau du contrat API. Elle ne fixe **aucune valeur** que l'ADR source laisse ouverte — toute valeur non fixée est marquée `[À TRANCHER — <ticket>]`. Les exemples chiffrés éventuels des Annexes ADR (rate limiting 20/10 req/min, claim 1 h, etc.) sont **illustratifs dans l'ADR source, non repris ici comme autorité**.

---

## 1. Principe transversal : non-révélation d'existence

Principe posé par ADR-014 et confirmé par ADR-015 : un appelant qui n'a pas accès à une ressource ne doit pas pouvoir en déduire l'existence.

> ADR-015:117 — « La liaison est rejetée silencieusement si la condition n'est pas satisfaite (pas de message d'erreur révélant l'existence du compte — **cohérent avec ADR-014 §Conséquences, principe de non-révélation d'existence**). »

Ce principe gouverne trois familles de décisions distinctes dans ce document :
- le choix 403 vs 404 sur les lectures de ressources d'espace (§2, §4),
- l'exclusion silencieuse des backlinks non lisibles (§3),
- le rejet silencieux de la liaison OAuth (§5).

**Mécanisme porteur** : la décision d'accès est composée en un point unique par `IResourceAccessPolicy.CanAccess(principal, resourceRef)` (ADR-014 §3), qui retourne une valeur discriminée `Allowed` / `Denied(reason)`. C'est cette même décision qui alimente, côté REST, le code de statut renvoyé par le pipeline behavior MediatR — et, côté SignalR, le filtrage avant diffusion (hors périmètre HTTP de cette spec).

---

## 2. Cas 403 déjà tranchés (référence IDOR — ADR-014:238-245)

Le tableau suivant reproduit fidèlement les scénarios de référence d'ADR-014 §Conséquences (« Critères d'acceptation — cas IDOR de référence », destinés à B5.2/B8.2). Ce sont des **décisions actées**, pas des propositions : le code indiqué est celui de l'ADR.

| # | Scénario | Code HTTP | Source ADR | Statut |
|---|---|---|---|---|
| 1 | Appelant non-membre → lecture d'une ressource d'un autre espace | **404 ou 403** (l'ADR laisse le choix ouvert — voir §4) | ADR-014:238 | Ouvert par l'ADR lui-même — non tranché ici |
| 2 | Invité scope `SESSION` → ressource hors de sa session | **403** | ADR-014:239 | Tranché |
| 3 | Lecture d'un espace avec `deleted_at IS NOT NULL` | Invisible (filtre P2 — voir §3) | ADR-014:240 | Tranché — pas un code distinct |
| 4 | Lecture d'un espace avec `purge_claimed_at IS NOT NULL` | Invisible (filtre P2 — voir §3) | ADR-014:241 | Tranché — pas un code distinct |
| 5 | Joueur → document `GM_ONLY` | **403** | ADR-014:242 | Tranché |
| 6 | Invité → document `GM_ONLY` (y compris épinglé en session) | **403** | ADR-014:243 | Tranché |
| 7 | Invité → document `PLAYER_PRIVATE` d'un autre joueur | **403** | ADR-014:244 | Tranché |
| 8 | Invité réassocié au même `character_id` → `LIVE_NOTE` d'un compte supprimé | **403** (règle auteur-seul domaine + F-08/ADR-012) | ADR-014:245 | Tranché |

**Lecture** : les scénarios 2, 5, 6, 7, 8 sont des cas de **refus légitime connu** — l'appelant est un principal reconnu (membre ou invité actif de l'espace) qui se voit opposer une règle de visibilité ou de scope. Dans tous ces cas, l'ADR fixe **403**, jamais 404 : l'existence de la ressource n'est pas cachée à un appelant qui appartient déjà à l'espace ou à la session concernée.

Le scénario 1 (non-membre, cross-espace) est le seul des huit laissé ouvert par l'ADR lui-même (« ou » — l.240). Voir §4 pour la proposition dérivée.

---

## 3. Cas P2 et backlinks — exclusion sans code distinct

### 3.1 Espace en corbeille ou en purge (P2)

> ADR-014:100 — « Les prédicats P2 et P3 sont structurels et indépendants de l'appelant : ils sont portés par des **query filters EF Core globaux** (→ B5.1). »

Un espace avec `deleted_at IS NOT NULL` ou `purge_claimed_at IS NOT NULL` (P2), ou un document avec `is_deleted = true` (P3), n'est **pas un cas d'erreur distinct au niveau du contrat** : le filtre EF Core global rend la ressource invisible en amont de toute résolution applicative. Le comportement observable au niveau HTTP est le même que pour toute ressource non trouvée par les prédicats structurels — la spec ne lui attribue pas de code ou de corps de réponse dédié. Ce point est cohérent avec ADR-011:159 (« Invariant de visibilité du soft-delete » — appliqué à tous les chemins de lecture, y compris lecture directe par ID).

**Exception actée** : l'endpoint de restauration d'espace (réservé à l'`OWNER`) est le seul chemin autorisé à traverser P2 — il vérifie explicitement `deleted_at IS NOT NULL` (ADR-014:102).

### 3.2 Backlinks — exclusion silencieuse

> ADR-014:264-266 — « Les requêtes de backlinks (…) doivent appliquer `IResourceAccessPolicy`/`CanBeReadBy` sur chaque document **source** retourné. Un auteur d'un document cible ne doit pas pouvoir inférer l'existence d'un document `PLAYER_PRIVATE` qui le référence : si le document source n'est pas lisible par l'appelant (…), il est **exclu de la liste des backlinks retournée**. »

Une requête de backlinks (« quels documents pointent vers ce document ? ») ne renvoie donc jamais d'erreur pour les documents source non lisibles — ils sont simplement absents de la liste retournée. Aucun code d'erreur, aucun indicateur de filtrage partiel n'est prévu par l'ADR pour signaler l'exclusion.

---

## 4. `[À TRANCHER — B1.10]` — 403 vs 404 sur non-membre / cross-espace

ADR-014:238 laisse explicitement ouvert le choix entre 404 et 403 pour le scénario « appelant non-membre → lecture d'une ressource d'un autre espace », et renvoie ce point à **B1.10** (ADR-014:280 : « Contrat OpenAPI : sémantique 403 vs 404 par endpoint (ne-pas-révéler-l'existence vs accès refusé connu), annotations de sécurité »).

**Proposition dérivée du principe de non-révélation (§1) — à confirmer B1.10, non tranchée** :

| Situation | Code proposé | Justification dérivée |
|---|---|---|
| Appelant non-membre de l'espace (aucune appartenance, P1 non satisfait) | **404** | L'appelant ne doit pas apprendre l'existence de la ressource — cohérent avec le principe de non-révélation (§1) |
| Appelant membre ou invité actif, refus connu par une règle de visibilité ou de scope (cas §2, scénarios 2/5/6/7/8) | **403** | L'appartenance à l'espace/session est déjà établie ; le refus est un refus légitime connu, pas une dissimulation d'existence |

Cette proposition n'est **pas une décision** : elle est une lecture dérivée du principe transversal (§1) appliquée au seul point que l'ADR laisse en « ou ». Elle doit être confirmée à l'occasion de B1.10, endpoint par endpoint.

**Restent également `[À TRANCHER — B1.10]`**, sans proposition dérivée (hors périmètre du principe de non-révélation) :
- schémas de requête et de réponse exacts par endpoint,
- attribution fine des codes 400 (validation) et 401 (authentification manquante/invalide) par endpoint,
- liste exhaustive et littérale des endpoints (chemins REST), non fixée par ADR-014 ni ADR-015 — les tableaux de cette spec désignent des **ressources et scénarios**, pas des chemins d'URL arrêtés.

---

## 5. 429 — Rate limiting sur les endpoints d'authentification

> ADR-015:176-185 — périmètre des endpoints couverts par le rate limiting.

| Endpoint | Risque principal | Source |
|---|---|---|
| `POST /auth/login` | Brute-force, credential stuffing | ADR-015:224 |
| `POST /token/refresh` | Replay de refresh token compromis | ADR-015:225 |
| `POST /auth/validate-token` (GuestAccess) | Énumération de tokens (F-06 original) | ADR-015:226 |
| `POST /auth/password-reset/request` | Abus de la fonction de reset, spam | ADR-015:227 |
| `POST /auth/password-reset/confirm` | Brute-force du token de reset | ADR-015:228 |

Ces cinq endpoints renvoient **429** au-delà du seuil de rate limiting. Le mécanisme est hybride (par-IP ET par-compte, seuils indépendants cumulatifs) — détaillé dans `config-securite-migration.md` (hors périmètre codes HTTP de cette spec ; voir aussi ADR-015:348, B1.10 : « codes d'erreur 400/401/429, schémas de requête et réponse »).

**`[À TRANCHER — B1.10]`** : schéma exact du corps de réponse 429 (ex. `Retry-After`, quota restant) — non spécifié par l'ADR.

---

## 6. Liaison OAuth rejetée silencieusement

> ADR-015:117 (contexte complet §2.3 de l'ADR) — la liaison d'un compte OAuth à un compte préexistant dont l'email n'est pas prouvé vérifié est **rejetée silencieusement** : « pas de message d'erreur révélant l'existence du compte ».

Au niveau du contrat API, cela signifie qu'aucun code ou corps de réponse distinctif ne doit permettre à l'appelant de déduire que le rejet est dû à l'existence d'un compte non vérifié plutôt qu'à une autre cause. La spec n'attribue donc pas ici de code spécifique à ce cas — l'ADR pose le principe de silence, pas un code.

**`[À TRANCHER — B1.10]`** : code HTTP exact retourné dans ce cas (générique, indistinguable d'un autre refus) — non fixé par l'ADR source.

---

## 7. Annotations de sécurité — principe de traçabilité du contrat

Le contrat OpenAPI doit annoter, par endpoint scopé à une ressource d'espace, le fait qu'il passe par le pipeline behavior `IResourceAccessPolicy` (ADR-014 §3) — cette annotation est ce qui permet au test d'archi CI (ADR-014 → B3.2) de vérifier l'exhaustivité de la couverture. Cette spec ne fixe pas le format exact de l'annotation (attribut OpenAPI, tag, extension `x-*`) : ce choix relève de B1.10.

---

## Récapitulatif `[À TRANCHER]`

| Point ouvert | Ticket | Nature |
|---|---|---|
| 403 vs 404 non-membre/cross-espace (proposition dérivée fournie §4, à confirmer) | B1.10 | Décision |
| Schémas de requête/réponse exacts par endpoint | B1.10 | Implémentation |
| Attribution fine 400/401 par endpoint | B1.10 | Implémentation |
| Chemins REST littéraux (liste exhaustive) | B1.10 | Implémentation |
| Schéma exact du corps de réponse 429 | B1.10 | Implémentation |
| Code HTTP exact du rejet silencieux de liaison OAuth | B1.10 | Implémentation |
| Format de l'annotation de sécurité OpenAPI (`IResourceAccessPolicy`) | B1.10 | Implémentation |

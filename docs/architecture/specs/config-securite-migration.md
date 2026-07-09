# Registre des seuils et valeurs de configuration — sécurité & migration

> Spec pré-build, cluster sécurité & contrats API. Dérivée fidèlement de **ADR-015** (Sécurité authentification MVP, l.53-55, l.59, l.63, l.111-112, l.190, l.194, l.196, l.206, l.277, l.293), **ADR-016** (Sérialisation locale et contrat de migration, l.183, l.238, l.254) et **ADR-011** (Cascade & intégrité référentielle, l.166, l.277). Statut : ADR-015 et ADR-011 = décision pré-implémentation ; ADR-016 = mixte à dominante pré-implémentation (en-têtes des trois ADR) — à confirmer à l'entrée en build.
>
> **Portée** : ce registre reporte les **bornes normatives réellement fixées** par les ADR sources — pas les valeurs d'exemple de leurs Annexes. Là où l'ADR fixe une borne dure ou une plage, cette spec la reporte telle quelle. Là où l'ADR renvoie explicitement une valeur exacte à un ticket d'implémentation, la case « Valeur retenue » reste `[À TRANCHER — <ticket>]` — aucune valeur n'est inventée ici. Les exemples chiffrés des Annexes ADR (Argon2id 64 MiB / 3 itérations, 20/10 req/min, claim 1 h) sont **illustratifs dans l'ADR source, non repris ici comme valeur retenue**.

---

## Registre

| Paramètre | Borne normative ADR (fichier:ligne) | Valeur retenue / ouverte | Ticket |
|---|---|---|---|
| Longueur minimale du mot de passe | ADR-015:53-55 | **Retenue : ≥ 12 caractères** | — (tranché) |
| Complexité du mot de passe (chiffres/symboles/casse) | ADR-015:53-55 | **Retenue : désactivée explicitement** (`RequireDigit`/`RequireNonAlphanumeric`/`RequireUppercase`/`RequireLowercase` positionnés à `false` dans le code, pas laissés par défaut) | — (tranché) |
| Algorithme de hachage du mot de passe | ADR-015:59, 63 | **Retenue : Argon2id**, avec **repli bcrypt cost ≥ 12** si Argon2id non intégrable dans la contrainte de dépendances MVP | `[À TRANCHER — B1.5]` pour les paramètres exacts (mémoire, itérations, parallélisme) — les valeurs de l'Annexe ADR-015 (bloc 2) sont illustratives, pas autoritaires |
| Durée de vie de l'access token (JWT) | ADR-015:111 | **Retenue : ≤ 15 min**, aucune prolongation — renouvellement via refresh uniquement | — (tranché) |
| Durée de vie du refresh token | ADR-015:112 | **Retenue : ≤ 7 jours ABSOLUS**, borne dure, aucune prolongation glissante | — (tranché) |
| Dimension du rate limiting | ADR-015:190 | **Retenue : hybride par-IP ET par-compte**, seuils indépendants et cumulatifs (les deux dimensions combinées, pas l'une ou l'autre) | — (principe tranché) |
| Valeurs exactes des seuils rate limiting (par-IP, par-compte) | ADR-015:194 | `[À TRANCHER — B1.5]` | B1.5 |
| Borne supérieure de sécurité par seuil de rate limiting | ADR-015:196 | **Retenue : une borne supérieure de sécurité est obligatoire pour chaque seuil** (principe — une valeur arbitrairement élevée neutraliserait la protection) ; valeur exacte `[À TRANCHER — B1.5]` | B1.5 |
| TTL du token de reset de mot de passe | ADR-015:206 | **Retenue : borne dure ≤ 15 min**, valeur exacte dans la plage cible **10-15 min** | `[À TRANCHER — B1.5]` pour la valeur exacte dans la plage |
| `schemaVersion` minimale maintenue côté serveur | ADR-016:238, 254 | `[À TRANCHER — P7]` | P7 |
| Rétention du registre `migration_batch_id` | ADR-016:183 | `[À TRANCHER — P7]` | P7 |
| Timeout d'expiration du claim `purge_claimed_at` | ADR-011:166, 277 | `[À TRANCHER — B1]` — l'ADR mentionne « ex. 1 heure » à titre d'exemple, non retenu comme valeur | B1 |

---

## Détail par paramètre

### Politique mot de passe (ADR-015 §1.1)

> ADR-015:53-55 — « La longueur minimale est 12 caractères (…). Les règles de complexité (chiffres, symboles, casse) sont **désactivées explicitement**. (…) Les options `RequireDigit`, `RequireNonAlphanumeric`, `RequireUppercase`, `RequireLowercase` sont **positionnées à `false`** dans le code, pas simplement commentées. »

Ces deux valeurs (longueur ≥ 12, complexité désactivée) sont des bornes fixées par l'ADR, pas des valeurs d'exemple : elles sont reportées ici comme **retenues**.

### Algorithme de hachage (ADR-015 §1.2)

> ADR-015:59 — « L'implémentation `IPasswordHasher<User>` utilisée est **Argon2id** (…) »
> ADR-015:63 — « Si Argon2id n'est pas intégrable dans la contrainte de dépendances MVP, **bcrypt avec cost factor ≥ 12** constitue le repli acceptable. »

Le choix d'algorithme (Argon2id, ou repli bcrypt ≥ 12) est **tranché** par l'ADR. Les paramètres numériques d'Argon2id (mémoire, itérations, parallélisme) figurent dans l'Annexe ADR-015 (bloc 2) comme esquisse pré-implémentation — l'ADR précise explicitement (en-tête d'Annexe) que ces esquisses « illustrent les décisions d'architecture (…) ne font pas autorité sur le besoin ». Cette spec ne les reprend donc pas comme valeur retenue.

### Cycle de vie des tokens (ADR-015 §3.1)

> ADR-015:111-112 (table) :
> | Token | Durée | Mode de prolongation |
> |---|---|---|
> | Access token (JWT) | ≤ 15 min | Aucune — renouvellement via refresh uniquement |
> | Refresh token | ≤ 7 jours ABSOLUS | Aucune prolongation glissante (borne dure) |

Ces deux bornes sont des valeurs fixées, pas des illustrations — reportées telles quelles.

### Rate limiting (ADR-015 §4.2)

> ADR-015:190 — « Le rate limiting est **hybride : par-IP ET par-compte**. (…) Les deux dimensions doivent être combinées avec un seuil indépendant pour chaque. »
> ADR-015:194 — « Les valeurs exactes sont renvoyées à **B1.5** (implémentation). Le principe — deux dimensions, seuils indépendants, cumulatifs — est arrêté ici. »
> ADR-015:196 — « **Note sécurité** : B1.5 doit déclarer une borne supérieure de sécurité pour chaque seuil — des valeurs arbitrairement élevées (ex. 10 000 tentatives/min) neutraliseraient la protection. La justification des seuils retenus (…) est exigée à l'implémentation. »

Le principe hybride et l'obligation d'une borne supérieure de sécurité sont **tranchés**. Les valeurs numériques (seuils par-IP, par-compte, et la borne supérieure elle-même) sont explicitement renvoyées à B1.5. L'exemple « 20/10 tentatives/min » de l'Annexe ADR-015 (bloc 7) est illustratif et n'est pas repris comme valeur retenue.

### TTL du token de reset de mot de passe (ADR-015 §4.3)

> ADR-015:206 — « **TTL court** : durée de validité ≤ 15 min (borne dure normative) ; valeur exacte à fixer en B1.5 dans la plage 10-15 min. »

La borne dure (≤ 15 min) et la plage cible (10-15 min) sont fixées par l'ADR — ce ne sont pas des valeurs illustratives. La valeur exacte à l'intérieur de cette plage reste ouverte.

### `schemaVersion` minimale et rétention `migration_batch_id` (ADR-016)

> ADR-016:238 (table des dettes) — « Seuil de `schemaVersion` minimale maintenue | Configuration — à fixer à l'implémentation | **P7** »
> ADR-016:254 (points à trancher) — « **Seuil `schemaVersion` minimale** — à fixer à l'implémentation P7 en fonction des versions du client déployées. »
> ADR-016:183 — « **Dette — horizon de rétention du registre `migration_batch_id`** : la durée de conservation des `migration_batch_id` en base (pour garantir l'idempotence) n'est pas fixée. À trancher en **P7** lors de l'implémentation du handler d'import. »

Ces deux paramètres sont explicitement non fixés par l'ADR — reportés comme ouverts, sans valeur proposée.

### Timeout d'expiration du claim `purge_claimed_at` (ADR-011)

> ADR-011:166 — « **Expiration du claim** : un claim posé (`purge_claimed_at IS NOT NULL`) est considéré **expiré** si la purge n'est pas terminée dans un délai maximal (à définir en **B1** — ex. 1 heure). »
> ADR-011:277 (points à trancher) — « **Seuil d'expiration du claim** : valeur concrète du timeout de `purge_claimed_at` (ex. 1 heure) à fixer en B1 lors de l'implémentation du Hosted Service. »

Le mécanisme (claim expirable, purge reclaimable après expiration) est tranché ; la valeur numérique du délai est explicitement un exemple (« ex. 1 heure »), non une décision — reportée ici comme ouverte.

---

## Confirmation de fidélité

Aucun exemple illustratif d'Annexe (paramètres Argon2id, seuils rate limiting 20/10 req/min, timeout de claim « 1 heure ») n'est promu ici en valeur retenue. Chaque ligne du registre distingue explicitement la borne normative fixée par l'ADR (reportée telle quelle) de la valeur numérique exacte laissée à l'implémentation (`[À TRANCHER — <ticket>]`).

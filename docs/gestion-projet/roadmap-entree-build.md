# Roadmap d'entrée en build — Haversack

> **Statut** : artefact de planification dédié, référencé par [CdC §11](../context/cahier-des-charges.md#11-phasage--jalons) et [ADR-011 §Points à trancher](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) comme le document produit « en amont de l'entrée en construction ». Audience : équipe de build (développeur·se solo ou équipe restreinte prenant en charge l'implémentation) et responsable produit (arbitrages de point de décision).
> **Date de production** : 2026-07-15.

---

## Bandeau de cadrage — ce que ce document fait et ne fait pas

Ce document **ordonnance** un périmètre déjà arrêté. Il ne redéfinit :
- **ni le MoSCoW** ([CdC §3](../context/cahier-des-charges.md#3-périmètre-moscow) — Must/Should/Could/Won't, [ADR-006](../architecture/decisions/ADR-006-perimetre-mvp.md) pour la trace des arbitrages fusionnés) ;
- **ni le phasage macro** ([CdC §11](../context/cahier-des-charges.md#11-phasage--jalons) — axe J0-J3 nommé par contenu, indicatif et non figé).

Il ne tranche **aucun ADR** : chaque contrainte technique citée ci-dessous renvoie à sa décision d'origine, qui fait foi en cas de désaccord de lecture. Il n'invente **aucun seuil, aucune valeur chiffrée** différée par un ADR à l'implémentation — ces points sont reportés fidèlement sous la forme `[À TRANCHER — ticket <code>]`.

Ce que ce document **fait** : il prend la séquence de jalons déjà nommée dans [ADR-006 §Conséquences](../architecture/decisions/ADR-006-perimetre-mvp.md) (J1 → J2 → J3) et le préalable J0 documenté dans [ADR-008 §Compléments post-revue](../architecture/decisions/ADR-008-structure-solution.md), et les assemble en une séquence unique typée, avec pour chaque jalon : les préalables bloquants déjà actés, les critères de sortie factuels déjà déductibles du corpus, l'ordre interne C#-first déjà acté par [ADR-001 §Compléments](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) et [ADR-008 §Compléments](../architecture/decisions/ADR-008-structure-solution.md), et le rattachement des livrables/tests nommés par chaque ADR à leur jalon. Il porte enfin la réconciliation — ratifiée en décision produit (§5) — de la double lecture du symbole `J1` signalée comme incohérence ouverte par [CdC §11](../context/cahier-des-charges.md#11-phasage--jalons).

**En cas de conflit entre ce document et un ADR, une US ou le CdC** : la source citée fait foi ; ce document est un artefact d'ordonnancement, pas une nouvelle autorité de corpus (même posture que [`cadrage-validation-pre-lancement-eu.md`](../securite/conformite/cadrage-validation-pre-lancement-eu.md) vis-à-vis des ADR RGPD).

**Hors périmètre** : contenu précis de chaque lot B1.x/M1/L1/P… (renvoyé à chaque ADR source), valeurs de seuils et de configuration (toutes marquées `[À TRANCHER — ticket]` dans les ADR d'origine), assets d'identité et écrans (renvoyés à la conception d'interface, `docs/conception/interface/**`, non modifiée ici), procédure et calendrier de saisine juridique (hors artefact de planification technique).

---

## 1. Séquence macro typée

La séquence retenue par cette roadmap comme axe d'ordonnancement du build est :

```
J0 (socle)
   │
J1 (local-only)
   │
[DÉCISION MARCHÉ]  ── NON-VERIFIABLE-IN-BUILD ── go/no-go
   │
J2 (cloud + migration)
   │
[VALIDATION JURIDIQUE EU]  ── NON-VERIFIABLE-IN-BUILD ── go/no-go, avant lancement (pas avant build)
   │
J3 (partage + temps réel)
```

**Note de lecture du diagramme** : la chaîne à flèche unique ci-dessus reflète l'ordre de citation, pas une dépendance de blocage entre `[VALIDATION JURIDIQUE EU]` et J3. La validation juridique EU bloque le **lancement/release EU** (mise en production commerciale) — elle ne bloque **pas le build de J3**, qui peut être construit en parallèle de l'instruction juridique (voir §3.5, « Peut être instruit en parallèle du build »). Seule `[DÉCISION MARCHÉ]`, en amont, conditionne réellement l'entrée dans le jalon suivant (J2).

Trois natures de nœuds distinctes composent cette séquence — les confondre serait une erreur d'ordonnancement :

| Nature | Nœuds | Ce qui les caractérise |
|---|---|---|
| **(a) Jalon de build** | J0, J1, J2, J3 | Contenu vérifiable en CI ou par revue de code ; critères de sortie factuels (§3) ; ordonnancement interne C#-first (§4) |
| **(b) Point de décision go/no-go, non vérifiable en CI** | `[DÉCISION MARCHÉ]`, `[VALIDATION JURIDIQUE EU]` | **`NON-VERIFIABLE-IN-BUILD`** — aucun test automatisé, aucun linter, aucune build Haversack ne peut trancher ces deux points de décision ; la dépendance est externe (télémétrie interprétée par le responsable produit pour le premier, avis juriste pour le second) |
| **(c) Préalable bloquant** | rattachés à leur jalon (§3, par nœud) | Condition d'entrée qui doit être levée **avant** que le jalon correspondant ne débute ou ne se termine ; nommément listée par l'ADR source, jamais inventée ici |

**Sources de la séquence** :
- J1 → J2 → J3, enchaînement sans découplage : [ADR-006 §Conséquences](../architecture/decisions/ADR-006-perimetre-mvp.md) — les jalons de build s'enchaînent sans découplage produit entre eux.
- J0 : préalable de structure non nommé « J0 » explicitement dans ADR-006, mais ses livrables (squelette Domain/Application, test d'archi CI) sont documentés comme antérieurs à tout jalon applicatif dans [ADR-008 §Compléments post-revue](../architecture/decisions/ADR-008-structure-solution.md). Cette roadmap le nomme J0 pour compléter la séquence — voir §5 pour la discussion de nomenclature.
- `[DÉCISION MARCHÉ]` : [ADR-006 §Compléments post-revue](../architecture/decisions/ADR-006-perimetre-mvp.md) — une décision marché doit être prise avant d'engager le build lourd cloud + temps réel.
- `[VALIDATION JURIDIQUE EU]` : [`cadrage-validation-pre-lancement-eu.md`](../securite/conformite/cadrage-validation-pre-lancement-eu.md) (5 axes, tous « à valider juriste », non vérifiable en CI) ; positionné **avant lancement EU**, distinct d'un jalon de build — voir §3.5 sur cette distinction.

---

## 2. Vue de synthèse par jalon

| Jalon | Contenu | Dépend de | Livrable-signal |
|---|---|---|---|
| **J0** | Socle : confirmation des 8 ADR pré-implémentation, squelette Domain/Application C#, test d'archi CI, contrat d'autorisation Application | — (point de départ) | Test d'archi CI vert en pipeline |
| **J1** | Local-only : Domain/App → projection TS/IndexedDB → Angular minimal | J0 stabilisé (ADR-006 §Compléments : « CR-1 est un préalable bloquant ») | Export `schemaVersion` produit et rejouable |
| **[DÉCISION MARCHÉ]** | Go/no-go produit sur télémétrie J1 | J1 stabilisé | Décision produit tracée |
| **J2** | Cloud + migration : EF Core, auth, handler d'import, cascade RGPD | J1 stable + décision marché = go | Migration d'un jeu de données J1 réussie de bout en bout |
| **[VALIDATION JURIDIQUE EU]** | Go/no-go avis juriste, 5 axes RGPD | Peut être instruit en parallèle de J2/J3 ; bloque le **lancement EU**, pas le build | 5 axes statués par un juriste |
| **J3** | Partage + temps réel : SignalR, canal invité, anti-fuite | J2 | Test anti-fuite SignalR (B8.2) vert |

---

## 3. Détail par jalon

### 3.1 J0 — Socle

**Contenu** (source : [ADR-008](../architecture/decisions/ADR-008-structure-solution.md), [ADR-007 §Compléments](../architecture/decisions/ADR-007-rgpd-autorisation-api.md)) :
- Confirmation à l'entrée en build des **8 ADR de nature pré-implémentation ou mixte** : ADR-003 (stack front), ADR-004 (transport temps réel), ADR-008 (structure solution), ADR-011 (cascade RGPD), ADR-015 (sécurité authentification), ADR-016 (sérialisation locale/migration), ADR-017 (modèle IndexedDB), ADR-018 (généralisation Space). Chacun le signale dans son en-tête — la confirmation est un acte de build, pas de conception.
- Squelette Domain/Application C# — Clean Architecture, 4 bounded contexts en frontières logiques (namespaces, pas d'assembly séparée au MVP), granularité multi-projets conservée sur Infrastructure/Présentation ([ADR-008 §Décision](../architecture/decisions/ADR-008-structure-solution.md)).
- **Test d'architecture en CI = livrable J0** ([ADR-008 §Compléments post-revue](../architecture/decisions/ADR-008-structure-solution.md) l.56) : vérifie les frontières de bounded context, remplace la discipline de revue de code jugée insuffisante en contexte solo. Sa **définition exhaustive** (liste complète des handlers scopés/non-scopés, couverture `ITokenValidator`/`ITokenDenylist`) est un renvoi ultérieur (→ B3.2, voir Annexe A) ; le test lui-même, dans sa forme initiale (frontières BC), est bien un livrable J0.
- Contrat d'autorisation Application : positionnement de `IResourceAccessPolicy` en couche Application, avant tout jalon applicatif ([ADR-007 §Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) l.48 : « doit être intégré dans les contrats de la couche Application avant le début du jalon J1 » — la définition du contrat est donc un livrable J0, son câblage effectif un livrable J1, voir §3.2).

**Critères de sortie factuels** :
- Les 8 ADR listés portent une confirmation explicite (date + décideur) à l'entrée en build.
- Le squelette Domain/Application compile et respecte la structure décrite dans `structure-projets.md`.
- Le test d'archi CI existe, tourne en pipeline, et échoue si une référence traverse une frontière de bounded context non autorisée.
- L'interface `IResourceAccessPolicy` (ou son équivalent tranché à l'implémentation) est déclarée en couche Application — son câblage complet (pipeline behavior REST + filtre SignalR) reste un critère de J1/J2, pas de J0.

**Préalable bloquant rattaché** : aucun préalable n'est bloquant *pour entrer* dans J0 — J0 est le point de départ de la séquence.

---

### 3.2 J1 — Local-only

**Contenu** (source : [ADR-001](../architecture/decisions/ADR-001-execution-domaine-mode-local.md), [ADR-016](../architecture/decisions/ADR-016-serialisation-locale-migration.md), [ADR-017](../architecture/decisions/ADR-017-modele-indexeddb-local.md), [ADR-007 §Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md)) :
- Domaine/Application (C#) → projection TS/IndexedDB → Angular minimal, dans cet ordre (voir §4 — ordre C#-first à l'intérieur du jalon).
- **`navigator.storage.persist()` (G-08) — préalable bloquant, à lever avant J1** ([ADR-001 §Compléments](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) l.59 ; [ADR-017 §3.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md)). Traité comme un état de première classe (booléen lu et traité, bandeau de durabilité affiché si refusé/best-effort) — pas une case cochée en silence.
- **Invariant d'autorisation câblé** : le contrat `IResourceAccessPolicy` défini en J0 doit être intégré aux contrats Application « avant le début du jalon J1 » ([ADR-007 §Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) l.48).
- **Export au format `schemaVersion`** : enveloppe versionnée (`schemaVersion`, `exportedAt`, `appVersion`, `spaces`), champs gouvernés vs libres, couture de projection `store local → payload` ([ADR-016 §1](../architecture/decisions/ADR-016-serialisation-locale-migration.md)).
- **Invariant `validation locale ⊆ validation serveur`** ([ADR-001 §Compléments](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) l.55 ; [ADR-016 §Conséquences](../architecture/decisions/ADR-016-serialisation-locale-migration.md)) — discipline de conception, non garantie outillée (pas de test cross-langage TS/C#).
- **Pas d'EF Core cloud en J1.** Le mode local est un store IndexedDB aggregate-rooted, sans aucune synchronisation continue, sans persistance serveur ([ADR-017 §1-2](../architecture/decisions/ADR-017-modele-indexeddb-local.md)). Toute apparition d'un accès EF Core / cloud dans le périmètre livré à J1 signale une confusion d'ordonnancement avec J2.

**Critères de sortie factuels** :
- Le store IndexedDB local (`folders`/`documents`/`document_blocks`/`document_links`/`document_tags`/`document_types`, plus le store racine — **nom exact non figé par ce critère, voir Annexe B** : [ADR-017 §1.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md) nomme encore ce store `campaigns`, résidu de rédaction antérieur au renommage [ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) qui n'a pas encore été propagé à l'ADR technique) est opérationnel et couvre le périmètre défini par [ADR-017 §1.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md).
- `navigator.storage.persist()` est appelé à l'entrée en mode local ; le résultat (accordé / refusé-best-effort) est lu et déclenche le bandeau de durabilité si nécessaire.
- Un export au format `schemaVersion` peut être produit depuis le store local et est structurellement rejouable (dry-run de validation possible côté contrat, sans nécessiter de serveur cloud actif pour cette vérification structurelle).
- Aucun appel réseau vers l'API Haversack n'existe dans le périmètre mode local livré (observable via la CSP `connect-src 'self'`, [ADR-017 §4.2](../architecture/decisions/ADR-017-modele-indexeddb-local.md)).

**Préalable bloquant rattaché** : `navigator.storage.persist()` avant J1 (G-08) ; invariant d'autorisation API câblé dans les contrats Application avant J1 ; stabilisation de J0 (« CR-1 est un préalable bloquant à la partie cloud/migration du MVP », [ADR-006 §Compléments](../architecture/decisions/ADR-006-perimetre-mvp.md) l.65 — J1 ne peut débuter sans J0 stabilisé).

---

### 3.3 [DÉCISION MARCHÉ] — go/no-go avant le build lourd cloud + temps réel

**Nature** : `NON-VERIFIABLE-IN-BUILD`. Positionné **après J1, avant** d'engager le build lourd cloud + temps réel (J2/J3) — [ADR-006 §Compléments post-revue](../architecture/decisions/ADR-006-perimetre-mvp.md) l.67.

**Critère de décision** (non-CI) : télémétrie instrumentée dès le périmètre MVP par pilier — activation préparation (**espace créé** + N documents, le contenu d'un espace personnel sans campagne ne comptant que partiellement s'il traduit un geste structurant — [CdC §12.4](../context/cahier-des-charges.md#124-valeurs-de-référence-chiffrées)), activation vue session (session ouverte + usage réel), activation partage (document partagé + ≥ 1 joueur l'ayant ouvert) — croisée avec les hypothèses de validation **H1 à H4** ([CdC §2.3](../context/cahier-des-charges.md#23-hypothèses-de-validation-h1-à-h5)). Les seuils de décision eux-mêmes ne sont pas fixés par cette roadmap — ils relèvent de l'instrumentation produit, hors périmètre technique de cet artefact.

**Ce que cette décision n'est pas** : ce n'est pas un jalon de build, elle ne produit aucun livrable technique. C'est une décision produit qui conditionne l'entrée en J2 — au même titre qu'un point de décision produit go/no-go usuel. Elle ne réintroduit pas de découplage *produit* (MVP unique) — le MVP reste unique, J1 → J2 → J3 restent enchaînés sans découplage produit entre eux (§1). Elle arbitre un découplage d'investissement : engager ou non le capital du build lourd cloud + temps réel (J2/J3), pas une nouvelle scission du périmètre fonctionnel.

---

### 3.4 J2 — Cloud + migration

**Contenu** (source : [ADR-014](../architecture/decisions/ADR-014-modele-autorisation-api.md), [ADR-015](../architecture/decisions/ADR-015-securite-authentification-mvp.md), [ADR-016](../architecture/decisions/ADR-016-serialisation-locale-migration.md), [ADR-011](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md)) :
- **Dépendance d'entrée** : J1 stable **et** `[DÉCISION MARCHÉ]` = go. Les deux conditions sont cumulatives, pas alternatives ([ADR-006 §Conséquences](../architecture/decisions/ADR-006-perimetre-mvp.md)).
- **Préalable schéma — promotion ADR-002 (`characterId`/`guestAccessId` des `LIVE_NOTE`)** : ces deux champs passent de `documents.properties` (jsonb) à des **colonnes nullable indexées de premier niveau** sur `documents` ([ADR-002 §Compléments post-revue](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md) l.58). C'est un préalable au mapping EF Core (query filters ci-dessous, mêmes colonnes de premier niveau) et une **dépendance bloquante de l'invariant d'autorisation** : ADR-002 documente explicitement que « ce changement débloque l'invariant d'autorisation décrit dans ADR-007 ».
- **EF Core + query filters globaux** : `spaces.deleted_at IS NOT NULL` (soft-delete), `documents.is_deleted` — prédicats structurels P2/P3 du modèle d'autorisation, portés par des query filters EF Core globaux plutôt que par discipline de handler (→ B5.1, [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.284).
- **Auth F-02/F-06/F-10/F-11 + rotation clé JWT** : politique de mot de passe (Argon2id, longueur ≥ 12), rate limiting hybride par-IP/par-compte, cycle de vie des tokens (access ≤ 15 min, refresh ≤ 7 jours, rotation avec détection de réutilisation par famille), liaison OAuth conditionnée à l'email vérifié — le tout derrière des contrats applicatifs observables `ITokenValidator`/`ITokenDenylist`/`ITokenSigner`/`IEmailVerificationPolicy` ([ADR-015 §1-5](../architecture/decisions/ADR-015-securite-authentification-mvp.md)). Rotation de clé via `ITokenSigner.RotateKey(...)` sans redéploiement.
- **Handler d'import/migration** : dry-run de pré-validation par espace, rapport de rejets structuré, transaction par espace (pas tout-ou-rien global, pas document par document), ordre topologique d'import, idempotence par `migration_batch_id`, gate de confirmation anti-appropriation côté serveur (drapeau `confirmed: true` obligatoire) ([ADR-016 §2-4](../architecture/decisions/ADR-016-serialisation-locale-migration.md)).
- **`Space.Delete()` + sagas** : toutes les FK en `ON DELETE RESTRICT`, saga applicative `SpaceDeleted` (purge J+30, deux passes — déliaison puis DELETE topologique) et saga `UserAnonymized` (effacement de compte, réécriture ASP.NET Identity avant anonymisation) ([ADR-011](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md)). Purge inconditionnelle de l'espace `PERSONAL` à `UserDeleted`, sous le même invariant de claim/idempotence/reprise que la purge J+30.
- **`ReclaimViaFederatedProof` (B1.5)** : opération de domaine dédiée pour le cas « email OAuth = compte préexistant non vérifié » — reprise de la coquille non vérifiée par la preuve IdP (bascule `emailVerified` + neutralisation du credential préexistant + liaison fédérée) ([ADR-015 §2.3](../architecture/decisions/ADR-015-securite-authentification-mvp.md)). **Statut de la résolution elle-même** : `[À TRANCHER — à ratifier produit]` dans l'ADR source — cette roadmap ne préjuge pas de la ratification, elle rattache seulement l'opération à J2/B1.5 si la résolution proposée (Option A) est retenue.

**Critères de sortie factuels** :
- Les query filters EF Core P2/P3 sont actifs sur tous les chemins de lecture (énumération, lecture par ID, projections) — pas seulement les endpoints listés.
- Le pipeline behavior REST et le filtre de diffusion SignalR appellent le même `IResourceAccessPolicy.CanAccess(...)` (non-divergence structurelle, [ADR-014 §Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md)).
- Un export J1 peut être migré de bout en bout : dry-run, rapport de rejets le cas échéant, import transactionnel par espace, idempotence vérifiée sur rejeu du même `migration_batch_id`.
- `AccountSuspended` et `UserAnonymized` déclenchent effectivement `ITokenDenylist.RevokeFamilyAsync` (fenêtre résiduelle fermée au sens applicatif, [ADR-015 §3.6](../architecture/decisions/ADR-015-securite-authentification-mvp.md)).
- La saga `SpaceDeleted` s'exécute sans erreur de cycle FK sur un espace de test couvrant les deux cycles documentés (`documents ⇄ guest_accesses`, `documents ⇄ folders`).

**Préalable bloquant rattaché** : J1 stable + `[DÉCISION MARCHÉ]` = go (cumulatif) ; **promotion ADR-002 des colonnes `characterId`/`guestAccessId` (`LIVE_NOTE`)**, préalable schéma qui débloque l'invariant d'autorisation ADR-007 (voir ci-dessus) ; F-02/F-06/F-10/F-11 (auth) déjà reclassés bloquants MVP par [ADR-007 §Compléments](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) ; F-04 (base légale invités, [ADR-013](../architecture/decisions/ADR-013-rgpd-donnees-invites.md)) — bloquant **avant tout lancement EU**, donc au plus tard à la `[VALIDATION JURIDIQUE EU]`, pas nécessairement avant le début technique de J2.

---

### 3.5 [VALIDATION JURIDIQUE EU] — validation juridique pré-lancement

**Nature** : `NON-VERIFIABLE-IN-BUILD`. Bloque le **lancement EU** (mise en production commerciale), pas le build technique — distinction explicite à ne pas confondre avec un jalon de build ([`cadrage-validation-pre-lancement-eu.md` § Non vérifiable en CI](../securite/conformite/cadrage-validation-pre-lancement-eu.md)).

**Contenu — 5 axes, tous « à valider juriste »** (source unique de détail : [`docs/securite/conformite/cadrage-validation-pre-lancement-eu.md`](../securite/conformite/cadrage-validation-pre-lancement-eu.md), ce document n'en reproduit que la synthèse) :
1. Art. 8 — qualification « service destiné aux mineurs » (attestation 16+ suffisante ou non).
2. Art. 28 + périmètre DPA — posture sous-traitant pour le contenu MJ décrivant des tiers identifiables, y compris l'espace `PERSONAL`.
3. Mise en balance de l'intérêt légitime — conservation post-effacement des documents partagés, base légale du `display_name` invité.
4. Art. 17 — hard-delete inconditionnel de l'espace `PERSONAL`, y compris les deux points subsidiaires sur l'instant de référence.
5. Facette RGPD de l'Option A « reclaim-in-place » — sort du contenu éventuel d'une coquille non vérifiée reprise par preuve IdP.

**Peut être instruit en parallèle du build** : rien n'impose d'attendre la fin de J2/J3 pour engager la saisine juriste — les 5 axes portent sur des postures déjà actées en conception, consultables indépendamment de l'avancement technique. Ce document ne fixe ni délai ni procédure de saisine ([`cadrage-validation-pre-lancement-eu.md` § Ce que ce document ne fait pas](../securite/conformite/cadrage-validation-pre-lancement-eu.md)) — cette roadmap n'en fixe pas davantage.

**Critère de sortie** : les 5 axes portent chacun un statut autre que « à valider juriste » (validé, mitigé avec mesure actée, ou refusé avec plan de remédiation) — critère de nature binaire par axe, pas un seuil chiffré.

---

### 3.6 J3 — Partage + temps réel

**Contenu** (source : [ADR-004](../architecture/decisions/ADR-004-transport-temps-reel.md), [ADR-014](../architecture/decisions/ADR-014-modele-autorisation-api.md)) :
- **Dépendance d'entrée** : J2 (le canal SignalR filtre par le même `IResourceAccessPolicy` que REST — dépendance structurelle, pas seulement calendaire).
- **SignalR + repli polling** : transport forcé en SSE tant que la communication reste unidirectionnelle MJ→joueurs ; connexion fermée en fin de session LIVE ; heartbeat allongé. **Seuil de coût par session concurrente déclenchant le repli vers polling adaptatif = `[À TRANCHER — ticket]`** (critère de réversibilité, pas un commentaire de documentation, [ADR-004 §Compléments](../architecture/decisions/ADR-004-transport-temps-reel.md) l.47).
- **Auth canal invité, token hors query-string** : le token `GuestAccess` n'est pas transmis en query-string — cookie court-lived ou échange de token avant négociation WebSocket ([ADR-004 §Compléments](../architecture/decisions/ADR-004-transport-temps-reel.md) l.49). Déconnexion forcée du hub sur `GuestAccessRevoked`/`GuestAccessExpired` (→ B1.7).
- **Anti-fuite par visibilité** : chaque événement poussé est filtré selon `PUBLIC`/`PLAYER_PRIVATE`/`GM_ONLY` — pas de diffusion par groupe espace indifférenciée ([ADR-004 §Compléments](../architecture/decisions/ADR-004-transport-temps-reel.md) ; [ADR-014 §Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md)).

**Critères de sortie factuels** :
- Le filtre de diffusion SignalR appelle `IResourceAccessPolicy.CanAccess(...)` avant tout push — même service que REST, pas de logique dupliquée.
- Une déconnexion forcée du hub est observable sur `GuestAccessRevoked` et `GuestAccessExpired`.
- Le token `GuestAccess` n'apparaît dans aucune URL ni log de requête (vérifiable par revue/scan).
- Le test anti-fuite SignalR (B8.2) couvre au minimum le cas de référence « push vers un invité après révocation du `GuestAccess` » → attendu interdit ([ADR-014 §Conséquences](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.249).

**Préalable bloquant rattaché** : J2 achevé (autorisation câblée, migration opérationnelle) ; le seuil de repli SSE→polling reste un `[À TRANCHER — ticket]` non bloquant pour l'entrée en J3 mais bloquant pour sa clôture opérationnelle.

---

## 4. Ordre C#-first — interne à chaque jalon

**Principe acté** (cohérent avec [ADR-001 §Compléments](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) l.53, ordre C#-first ; formulation littérale en [ADR-008 §Compléments post-revue](../architecture/decisions/ADR-008-structure-solution.md) l.60) : *« La solution .NET est échafaudée en premier. La structure des projets .NET est donc le premier livrable de structure, avant tout projet Angular ou TypeScript. »*

Cet ordre s'applique **à l'intérieur de chaque jalon**, pas en travers de la séquence macro :

```
Domain / Application (C#)  →  EF Core (persistance)  →  TS mode local  →  Angular
```

**Note de lecture** : ce diagramme est un **gabarit intra-jalon**, pas une séquence macro — **aucun jalon ne contient les quatre étages** (J0 n'a ni EF Core ni TS/Angular ; J1 n'a pas d'EF Core cloud ; J2 n'a pas de nouveau TS/Angular local ; voir « Application par jalon » ci-dessous). Lire « EF Core avant TS » comme un ordre global qui contredirait « J1 local-only avant J2 cloud » (§1) serait une erreur de lecture : ce gabarit ne s'applique qu'à l'intérieur d'un même jalon, jamais en travers de la séquence macro.

**Ce que cet ordre interdit explicitement** : que l'EF Core cloud de J2 précède l'UI locale de J1. Le mode local (TS/IndexedDB, Angular minimal) de J1 est une **projection** du domaine C# défini en J0/J1 — il ne s'exécute pas en attendant que J2 (EF Core, cloud) soit disponible. Confondre les deux ordres reviendrait à bloquer J1 sur un livrable de J2, ce que la séquence macro (§1) exclut explicitement : J1 est local-only et ne dépend d'aucune brique cloud.

**Application par jalon** :
- **J0** : Domain/Application C# (squelette, test d'archi) — aucune brique EF Core cloud, TS ou Angular à ce stade.
- **J1** : Domain/Application (déjà posé en J0, consommé ici) → projection TS/IndexedDB (store local, ADR-017) → Angular minimal (CRUD local). Pas d'EF Core cloud.
- **J2** : Application (contrats déjà posés) → EF Core (mapping, query filters, migrations SQL) → pas de nouveau TS/Angular local — le mode local de J1 reste stable pendant que le cloud se construit dessous.
- **J3** : Application/Domain (événements `GuestAccessRevoked`/`Expired` déjà modélisés) → Haversack.Infrastructure.Notifications (SignalR, isolé dès J0 selon [ADR-004 §Conséquences](../architecture/decisions/ADR-004-transport-temps-reel.md)) → Angular (vue session temps réel).

---

## 5. Réconciliation de nomenclature

### 5.1 Le double référent constaté

Le corpus portait deux usages distincts du symbole « J1 » :
- **ADR-006** ([§Conséquences](../architecture/decisions/ADR-006-perimetre-mvp.md) l.46) : J1 = **local-only** (le jalon décrit en §3.2 de cette roadmap).
- **ADR-007** ([§Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) l.48) : dans sa rédaction d'origine, un « jalon J1 » assorti d'un repère de jalon numéroté distinct (nomenclature aujourd'hui retirée du corpus) — appliqué à l'intégration de l'invariant d'autorisation API, une préoccupation **cloud**.

[CdC §11](../context/cahier-des-charges.md#11-phasage--jalons) constatait lui-même cette « incohérence apparente » et la renvoyait explicitement à « l'artefact de planification dédié » — c'est-à-dire au présent document.

### 5.2 Axe canonique acté pour l'ensemble du corpus

L'axe **J0/J1/J2/J3**, avec **J1 = local-only** (au sens ADR-006), est acté comme **axe canonique unique de phasage pour l'ensemble du corpus** — ratifié en décision produit le **2026-09-01**. Raison retenue : c'est l'usage le plus densément documenté (ADR-001, ADR-006, ADR-016, ADR-017 s'y réfèrent tous de façon cohérente), et c'est celui qui structure la dépendance technique explicite « pas d'EF cloud en J1 » (§1, §4).

La formulation d'origine d'ADR-007 a été clarifiée en conséquence ([§Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) l.48) : l'invariant d'autorisation API est intégré aux contrats de la couche Application avant J1, son application effective relevant de J2 — sans changement de la décision, de sa justification ni de ses conséquences.

### 5.3 Rattachement des codes de renvoi à l'axe J0-J3

Le tableau suivant rattache chaque code de renvoi le plus fréquemment cité par les ADR (P0.5, B1.x, M1, L1, P6, P7, B3.2, B5.1, B5.2, B8.2) au jalon de l'axe canonique (J0-J3) dont le **contenu** correspond.

| Jalon (axe canonique) | Codes de renvoi rattachés | Nature de l'ancrage |
|---|---|---|
| *(préalable, avant J0)* | P0.5 | **Inféré, à confirmer** — CdC §11 ne rapproche P0.5 de la phase de conception que par antériorité déduite (« hors périmètre du lot P0.5 » implique antérieur au premier jalon de build), pas par ancrage littéral. |
| J0 | — | Non ancré littéralement dans le corpus documenté ; cette roadmap le nomme J0 pour compléter la séquence macro (§1). |
| J1 (local-only) | **M1** (ADR-016/017, format sérialisation + IndexedDB + sécurité mode local), **L1** (sous-lot de M1, ADR-017 — schéma IndexedDB détaillé), **P6** (implémentation Angular des services IndexedDB) | Ancrage par **contenu** (les codes M1/L1/P6 portent tous sur le mode local), pas par citation explicite d'un jalon dans les ADR sources. |
| J1 → J2 (bascule migration) | **P7** (handler d'import serveur, tests e2e multi-versions IndexedDB) | P7 porte à la fois sur du code serveur (J2) et sur la validation du contrat local (J1) — code de frontière, pas assignable à un seul jalon. |
| J2 (cloud + migration) | **B1.x** (B1.4 RGPD étendu, B1.5 auth + `ReclaimViaFederatedProof`, B1.6 données invité espace vivant, B1.7 runtime SignalR, B1.9 chaîne média, B1.10 contrat OpenAPI), **B3.2** (test archi CI, définition exhaustive), **B5.1/B5.2** (query filters, tests IDOR) | Ancrage par **contenu** (auth cloud, migration, cascade RGPD étendue). L'ancrage documenté dans [ADR-011 §Points à trancher](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) rattachait littéralement B1.x à un jalon nommé numériquement dans une nomenclature aujourd'hui retirée du corpus — le rattachement retenu ici suit le contenu de B1.x, conformément à la règle de résolution actée en §5.2. |
| J3 (partage + temps réel) | **B1.7** (runtime SignalR, également listé J2 — recoupement), **B8.2** (tests anti-fuite SignalR) | Ancrage par **contenu** (temps réel) ; non ancré littéralement dans le corpus documenté. |
| Lancement EU (`[VALIDATION JURIDIQUE EU]`) | cadrage juridique interne (cf. [`cadrage-validation-pre-lancement-eu.md`](../securite/conformite/cadrage-validation-pre-lancement-eu.md)) | Hors séquence de jalons techniques — le cadrage juridique conditionne le lancement, pas un jalon de build. |

**Ce que ce tableau acte** : le rattachement de B1.x à J2 par contenu (ligne ci-dessus) applique la même règle de résolution que celle retenue en §5.2 pour le double référent « J1 » — le contenu prime sur la citation littérale d'une nomenclature de jalon aujourd'hui retirée du corpus.

### 5.4 Règle ratifiée

L'axe **J0/J1/J2/J3**, nommé par contenu, est l'unique repère de phasage du corpus. La nomenclature de jalon antérieure, numérotée indépendamment du contenu, en est retirée : toute référence à un jalon de build se fait par son contenu (socle / local-only / cloud + migration / partage + temps réel), jamais par cette ancienne numérotation. La divergence historique d'ADR-007 est résolue (§5.2 ci-dessus ; voir [ADR-007 §Conséquences](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) l.48).

---

## Annexe A — Livrables & tests nommés par ADR, par jalon

| Jalon | Livrable / test | Code de renvoi | Source ADR |
|---|---|---|---|
| J0 | Test d'archi CI (frontières bounded context) | — (livrable direct) | [ADR-008 §Compléments](../architecture/decisions/ADR-008-structure-solution.md) l.56 |
| J0 | Définition exhaustive du test d'archi CI (handlers scopés/non-scopés, couverture token) | B3.2 | [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.283 ; [ADR-015 §Points à trancher](../architecture/decisions/ADR-015-securite-authentification-mvp.md) l.351 |
| J1 | e2e multi-versions IndexedDB (fonction de projection store→payload) | P7 | [ADR-016 §Points à trancher](../architecture/decisions/ADR-016-serialisation-locale-migration.md) l.253 ; [ADR-017 §Points à trancher](../architecture/decisions/ADR-017-modele-indexeddb-local.md) l.276 |
| J1 | Services Angular IndexedDB (wrappers, `DomSanitizer`, bandeaux durabilité/confidentialité, CSP complète) | P6 | [ADR-017 §Points à trancher](../architecture/decisions/ADR-017-modele-indexeddb-local.md) l.275 |
| J2 | Promotion colonnes `characterId`/`guestAccessId` (`LIVE_NOTE`), `documents.properties` (jsonb) → colonnes nullable indexées de premier niveau | — (livrable direct) | [ADR-002 §Compléments post-revue](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md) l.58 |
| J2 | Query filters EF Core globaux (`spaces.deleted_at`, `documents.is_deleted`) | B5.1 | [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.284 |
| J2 | Handler d'import/migration serveur (résolution références, création topologique, rapport de rejets, idempotence) | P7 | [ADR-016 §Points à trancher](../architecture/decisions/ADR-016-serialisation-locale-migration.md) l.253 |
| J2 | `Space.Delete()` + sagas `SpaceDeleted`/`UserAnonymized` | — (livrable direct) | [ADR-011 §Conséquences](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) |
| J2 | 4 volets auth (`PasswordOptions`/Argon2id, `IEmailVerificationPolicy`, `ITokenDenylist`/`ITokenValidator`, rate limiting) + rotation clé JWT | B1.5 | [ADR-015 §Points à trancher](../architecture/decisions/ADR-015-securite-authentification-mvp.md) l.348 |
| J2 | `ReclaimViaFederatedProof` (opération de domaine dédiée, si Option A ratifiée) | B1.5 | [ADR-015 §2.3](../architecture/decisions/ADR-015-securite-authentification-mvp.md) l.331, 144 |
| J2 | Données invité sur espace vivant (`guest_accesses.display_name` hors purge) | B1.6 | [ADR-011 §Points à trancher](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) l.291 |
| J2 | Contrat OpenAPI des endpoints d'auth (codes 400/401/403/429) | B1.10 | [ADR-015 §Points à trancher](../architecture/decisions/ADR-015-securite-authentification-mvp.md) l.350 |
| J2 | Test IDOR (cas de référence) | B5.2 | [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.285 |
| J2 | Tests de sécurité spécifiques auth (brute-force, replay, rotation) | B3.2 | [ADR-015 §Périmètre](../architecture/decisions/ADR-015-securite-authentification-mvp.md) l.23 |
| J3 | Runtime SignalR (groupes, révocation en session, reconnexion), auth canal invité hors query-string | B1.7 | [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.281 ; [ADR-004 §Compléments](../architecture/decisions/ADR-004-transport-temps-reel.md) |
| J3 | Test anti-fuite SignalR (cas de référence : push vers invité après révocation) | B8.2 | [ADR-014 §Points à trancher](../architecture/decisions/ADR-014-modele-autorisation-api.md) l.286 |
| Post-MVP (dette nommée, hors jalons J0-J3) | Chaîne média externalisée (câblage purge `SpaceDeleted`) | B1.9 | [ADR-011 §Contenu LIVE_NOTE](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) l.268-272 |

**Note de lecture** : cette annexe rattache chaque livrable/test au jalon de l'axe canonique (§5.2). Certains codes (P7, B1.7) apparaissent à cheval sur deux jalons — reflet direct de la tension documentée en §5.3, pas une imprécision de cette annexe.

---

## Annexe B — Prérequis hors jalon

- **Assets d'identité** (logo, charte graphique/typo/grille, ressources icônes/composants) — préalable porté par la conception d'interface. Mentionné pour visibilité de trajectoire ; le détail (production, validation) relève de `docs/conception/interface/**`, non modifié par ce document.
- **Résidu de nommage store `campaigns` → `spaces`** ([ADR-017 §1.1](../architecture/decisions/ADR-017-modele-indexeddb-local.md), tableau des object stores, encore nommé `campaigns` au moment de la rédaction de cet ADR, antérieur au renommage [ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md)) — **watch point de build** : le renommage ubiquitaire acté par ADR-018 §Compléments post-revue a été propagé sur le corpus de conception (domaine, glossaire, use cases), mais la dénomination technique de l'object store IndexedDB (`campaigns` vs `spaces`) doit être confirmée cohérente au moment de l'implémentation P6 — ce n'est pas un point tranché différemment, c'est un résidu de rédaction à vérifier avant de coder le store.

---

## Renvois

- [`docs/context/cahier-des-charges.md` §11](../context/cahier-des-charges.md#11-phasage--jalons) — macro-ordonnancement de référence (axe J0-J3 nommé par contenu), réconciliation de nomenclature ratifiée (§5).
- [`docs/securite/conformite/cadrage-validation-pre-lancement-eu.md`](../securite/conformite/cadrage-validation-pre-lancement-eu.md) — détail des 5 axes de la `[VALIDATION JURIDIQUE EU]`.
- ADR cités : [ADR-001](../architecture/decisions/ADR-001-execution-domaine-mode-local.md), [ADR-002](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md), [ADR-003](../architecture/decisions/ADR-003-stack-front.md), [ADR-004](../architecture/decisions/ADR-004-transport-temps-reel.md), [ADR-006](../architecture/decisions/ADR-006-perimetre-mvp.md), [ADR-007](../architecture/decisions/ADR-007-rgpd-autorisation-api.md), [ADR-008](../architecture/decisions/ADR-008-structure-solution.md), [ADR-011](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md), [ADR-014](../architecture/decisions/ADR-014-modele-autorisation-api.md), [ADR-015](../architecture/decisions/ADR-015-securite-authentification-mvp.md), [ADR-016](../architecture/decisions/ADR-016-serialisation-locale-migration.md), [ADR-017](../architecture/decisions/ADR-017-modele-indexeddb-local.md), [ADR-018](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md).
- [`docs/architecture/README.md`](../architecture/README.md) et [`docs/architecture/architecture-detaillee.md`](../architecture/architecture-detaillee.md) — vue transverse par préoccupation, non redondante avec l'ordonnancement temporel de ce document.

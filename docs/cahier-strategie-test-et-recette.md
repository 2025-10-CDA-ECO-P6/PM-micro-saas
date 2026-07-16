# Cahier de stratégie de test et de recette — Haversack

| Champ | Valeur |
|---|---|
| Version | 2.0 |
| Date | 2026-07-16 |
| Statut | Version de référence — consolidation du corpus |
| Périmètre | MVP (UC-01 à UC-12, UC-14) + post-MVP signalé où il apparaît (UC-13, UC-15, stories repoussées) |
| Audiences | équipe de build (développeur·se solo ou équipe restreinte), opérateur (arbitrage des points non tranchés), expert QA en revue |
| Sources | `docs/conception/usecases/**`, `docs/conception/user-stories/**` (Gherkin, source unique des critères d'acceptation), `docs/conception/nfr/**`, `docs/planning/roadmap-entree-build.md` |

> Document dérivé du corpus, zéro décision produit neuve.

---

## 1. Objet & cadre d'autorité

### Ce que ce document fait

Ce document produit deux artefacts consolidés : (1) une **stratégie de test** — niveaux de test retenus, ce que chacun couvre, dérivés du corpus (use cases, domaine, NFR, ADR, roadmap) ; (2) un **cahier de recette** par use case — une table de cas de recette par UC, transposant chaque scénario Gherkin des User Stories en précondition/étapes/résultat attendu, avec un verdict à cocher en phase de recette.

### Ce que ce document ne fait pas

Il ne crée aucun critère d'acceptation produit. Il ne réordonnance rien (`roadmap-entree-build.md` fait foi sur l'ordonnancement des jalons). Il n'invente aucun seuil chiffré. Il ne tranche aucun point ouvert du corpus — un point non tranché est renvoyé au registre des points ouverts (§12), jamais comblé par une valeur ou une hypothèse.

### Ordre d'autorité du corpus

L'ordre suivant n'est jamais renversé par ce document :

**personas → vision → use cases (source de vérité) → user stories / NFR → domaine → glossaire.**

Toute référence ajoutée par ce document pointe vers le haut de cette hiérarchie : une ligne de recette cite l'UC et l'US qui la fondent, jamais l'inverse ; ce document ne devient à aucun moment une source que le corpus amont citerait en retour.

### Posture consommateur Gherkin

La couche Gherkin des user stories (`docs/conception/user-stories/`, arbitrage T-07) est la **source unique** des critères d'acceptation produit, en langage métier (Given/When/Then, ou Étant donné/Quand/Alors selon le fichier source). Ce document se positionne en **consommateur** : chaque case de la table de recette **transpose** un scénario Gherkin en précondition/étapes/résultat attendu et **cite sa source** (`US-UCNN-KK §"<titre du scénario>"`). Il ne réécrit pas les Gherkin mot pour mot comme une paraphrase autonome — il les trace vers un format exécutable en recette.

**Clause de re-dérivation** : si un scénario Gherkin source est modifié, la ligne de recette correspondante doit être re-dérivée. Une divergence constatée entre une ligne de ce cahier et le scénario Gherkin qu'elle cite est un **défaut à corriger dans ce document**, jamais une variante à documenter comme telle — le Gherkin fait foi. Cette citation se fait par **titre du scénario** (le corpus amont ne porte pas d'identifiant stable de scénario) — un renommage silencieux du titre source rendrait une citation caduque sans le signaler mécaniquement. C'est pourquoi un contrôle périodique de conformité des citations (§13) est le mécanisme concret de cette clause, pas seulement une politique déclarée.

### Deux natures de trou, traitées différemment

- **Règle métier (RB) existante sans Gherkin porteur** : un cas de recette peut en être dérivé, à condition de marquer explicitement la source `RB-NN-xx (dérivé, pas de Gherkin)` — ce marquage rend visible que la preuve ne descend pas d'un scénario Gherkin validé, mais d'une règle métier déjà actée dans le corpus.
- **Point réellement non tranché** : aucun cas de recette n'est créé. Le trou est renvoyé au registre des points ouverts (§12). Aucune valeur chiffrée (seuil de performance, coût, délai) n'est jamais inventée pour combler un tel point.

### Convention d'identifiants des cas de recette

Chaque case de la table de recette porte un identifiant `CR-UCNN-KK` où `NN` est le numéro du use case sur 2 chiffres et `KK` un numéro de case séquentiel sur 2 chiffres au sein de cet UC (ex. `CR-UC01-03`). Cette numérotation est propre à ce document — elle ne se substitue pas à la numérotation `US-UCNN-KK` des user stories, qu'elle cite en colonne Source.

---

## 2. Approche de test

**Stratégie fondée sur le risque.** Les zones à fort enjeu du corpus reçoivent la couverture la plus dense : confidentialité MJ/joueur (séparation `PLAYER_PRIVATE` / `GM_ONLY` / `PUBLIC`, NFR-CONF-01), autorisation (chemins de lecture filtrés, IDOR), authentification (cycle de vie des tokens, réinitialisation), migration de données (mode local → cloud, irréversible pour le MJ en cas de perte), temps réel (anti-fuite SignalR après révocation). Une zone à plus faible enjeu (ex. réordonnancement de dossiers) reçoit une couverture proportionnée, sans sur-instrumentation.

**Principe de la pyramide de tests.** La majorité de la couverture est portée par le niveau unitaire, sur les invariants du modèle de domaine — c'est le niveau le moins coûteux à maintenir et le plus proche de la règle métier. L'intégration cible les chemins traversant plusieurs composants (autorisation, migration, sagas). L'e2e reste concentré sur les parcours critiques du MVP (hors connexion intégral, migration bout-en-bout, accès joueur sans compte) plutôt que sur une couverture exhaustive de l'interface.

**Invariant transverse : privé par défaut.** Un document nouvellement créé est privé par défaut (RB-04-01), une note de session MJ est `GM_ONLY` par défaut (RB-06-11), une note de session joueur est `PLAYER_PRIVATE` et hors d'atteinte du MJ y compris `OWNER`/`GM` (RB-06-25/26). Cet invariant est recetté systématiquement à chaque niveau de test pertinent (unitaire pour la règle, intégration pour le chemin de lecture filtré, sécurité pour l'absence de contournement) — il n'est pas un cas isolé mais une propriété vérifiée partout où une visibilité est en jeu.

---

## 3. Niveaux de test

Cinq niveaux de test et deux axes transverses dérivés des NFR. Pour chacun : Objet, Périmètre, Critères d'entrée, Critères de sortie.

### 3.1 Unitaire (domaine / invariants métier)

**Objet** — Vérifier les invariants du modèle de domaine indépendamment de toute infrastructure (base de données, réseau, interface).

**Périmètre** :
- Machine d'états `Session` unidirectionnelle `LIVE → CLOSED → ARCHIVED` : chaque transition est irréversible, `ARCHIVED` refuse toute modification (`docs/conception/domain/session-conduct.md`).
- `Document.CanBeReadBy(userId, memberRole, documentType)` : un document `visibility = PLAYER_PRIVATE` est inaccessible même au MJ (`OWNER`/`GM`), sans exception — y compris pour les `LIVE_NOTE` (`docs/conception/domain/content-library.md`, `docs/conception/domain/session-conduct.md`).
- Visibilité privée par défaut à la création d'un document (RB-04-01) et d'une note de session MJ (RB-06-11).
- Quota de 3 espaces `CAMPAIGN`/`ONE_SHOT` actifs pour un compte `FREE` ; l'espace `PERSONAL` est hors quota, pour tous les plans (RB-02-19, `docs/conception/domain/space-management.md`).
- `isSystem` est informatif : les dossiers système sont renommables et supprimables ; le dossier virtuel « Non classés » est protégé (non renommable, non supprimable).
- `Document.Instantiate()` : copie profonde indépendante d'un template — la source reste intacte, la modification d'une instance n'affecte pas la source.
- Exclusivité personnage ↔ membership actif.

**Critères d'entrée** — Le modèle de domaine (agrégats `Session`, `Document`, `Space`, `Folder`) est implémenté en C# et compile ; les règles métier ci-dessus sont traçables à une règle nommée du corpus.

**Critères de sortie** — Chaque règle métier listée ci-dessus dispose d'au moins un test unitaire vert ; aucune règle listée n'est couverte uniquement par un niveau supérieur (intégration/e2e).

### 3.2 Intégration

**Objet** — Vérifier les chemins traversant plusieurs composants applicatifs ou l'accès aux données.

**Périmètre** :
- Query filters EF Core globaux P2/P3 (`spaces.deleted_at`, `documents.is_deleted`) actifs sur **tous** les chemins de lecture (énumération, lecture par ID, projections) → test nommé **B5.1**.
- Non-divergence REST/SignalR : le pipeline behavior REST et le filtre de diffusion SignalR appellent le même `IResourceAccessPolicy.CanAccess(...)`.
- Handler d'import/migration : dry-run de pré-validation, rapport de rejets, transaction par espace, idempotence sur rejeu du même `migration_batch_id` → **P7**.
- Sagas `SpaceDeleted` / `UserAnonymized` (cascades d'intégrité référentielle).
- Contrats d'authentification `ITokenValidator` / `ITokenDenylist` / `ITokenSigner` / `IEmailVerificationPolicy` → **B1.5**.
- Contrat OpenAPI des endpoints d'authentification, codes de réponse 400/401/403/429 → **B1.10** — le détail de certains codes (403 vs 404 sur une ressource non autorisée, schéma exact du 429) reste un point ouvert (§12, PO-13).
- Données invité persistées sur un espace vivant (`guest_accesses.display_name` hors fenêtre de purge) → **B1.6**.

**Critères d'entrée** — Les contrats applicatifs cités (`IResourceAccessPolicy`, `ITokenValidator`, etc.) sont déclarés ; un environnement EF Core + PostgreSQL de test est disponible.

**Critères de sortie** — B5.1 couvre tous les chemins de lecture énumérés ; B1.5 couvre les 4 contrats d'authentification cités ; B1.10 couvre les codes de réponse contractuels des endpoints d'authentification ; B1.6 couvre la persistance des données invité sur espace vivant ; P7 couvre le dry-run et le rapport de rejets ; aucune divergence REST/SignalR détectée sur le jeu de cas testé.

### 3.3 e2e

**Objet** — Vérifier les parcours complets, y compris les transitions de contexte réseau.

**Périmètre** :
- Hors connexion intégral : préparation et session sans réseau, durabilité inter-sessions, aucune perte silencieuse de données locales accompagnée des bandeaux requis, résilience à une perte réseau cloud passagère.
- Migration bout-en-bout d'un export local vers le cloud.
- e2e multi-versions IndexedDB (fonction de projection store → payload) → **P7**.
- Services Angular IndexedDB (wrappers, `DomSanitizer`, bandeaux, CSP complète) → **P6**.
- Runtime SignalR (groupes de diffusion, révocation en session, reconnexion), authentification du canal invité → **B1.7** — le test anti-fuite associé (cas de référence révocation) relève du niveau Sécurité (§3.5, **B8.2**).
- Parcours joueur complet : accès sans compte via lien de session, puis bascule invité → compte sans perte d'accès.

**Critères d'entrée** — Le mode local (store IndexedDB) et le cloud (EF Core) sont tous deux opérationnels dans l'environnement de test ; un jeu de données d'export versionné est disponible comme fixture.

**Critères de sortie** — Le parcours hors connexion intégral s'exécute sans appel réseau observé ; la migration bout-en-bout produit un rapport de rejets exploitable sur au moins un cas d'échec ; le parcours joueur sans compte → compte ne perd aucun accès observé ; B1.7 couvre le runtime SignalR (groupes, révocation, reconnexion) sans divergence avec le pipeline REST.

### 3.4 Test d'architecture (CI)

**Objet** — Vérifier les frontières structurelles du code, en remplacement de la discipline de revue de code jugée insuffisante en contexte d'équipe restreinte.

**Périmètre** :
- Frontières des 4 bounded contexts — livrable du jalon « Socle », sous sa forme initiale (frontières BC uniquement).
- Définition exhaustive du test (liste complète des handlers scopés/non-scopés, couverture des contrats `ITokenValidator`/`ITokenDenylist`) → **B3.2**, renvoi ultérieur au jalon « Cloud + migration ».

**Critères d'entrée** — Le squelette Domain/Application (4 bounded contexts en frontières logiques) existe.

**Critères de sortie** — Le test échoue si une référence traverse une frontière de bounded context non autorisée ; il tourne en pipeline CI à chaque build.

**Précision de périmètre** : ce niveau n'apparaît pas dans le cahier de recette par use case (§10) — il est adossé au jalon « Socle » (frontière structurelle des bounded contexts), non rattachable à un use case particulier.

### 3.5 Sécurité (adversarial)

**Objet** — Vérifier les scénarios d'attaque et les fuites de confidentialité.

**Périmètre** :
- IDOR (accès à une ressource par identifiant sans autorisation) → **B5.2**.
- Anti-fuite temps réel : push SignalR vers un invité après révocation de son `GuestAccess` = interdit → **B8.2**.
- Authentification : brute-force, replay, rotation de clé JWT — volet **B3.2**.
- Isolation de confidentialité : séparation stricte privé-MJ / vue-joueur, isolation du mode local (aucune émission réseau observable), cloisonnement `PLAYER_PRIVATE` (aucun accès MJ, y compris `LIVE_NOTE`).
- Anti-hijacking sur connexion fédérée avec branche email non vérifiée (CWE-287).
- Anti-énumération (CWE-204) : messages génériques sur login, réinitialisation, lien invalide — ne jamais révéler si un compte existe.
- Tokens de réinitialisation de mot de passe : borne dure ≤ 15 min, usage unique, invalidation des tokens précédents.
- Token `GuestAccess` jamais transmis en query-string ni journalisé (source : roadmap §3.6 / ADR-004 — pas les US).

**Critères d'entrée** — Les contrats d'autorisation et d'authentification sont câblés (pas seulement déclarés) ; un jeu de comptes de test multi-rôles est disponible (MJ, joueur, invité).

**Critères de sortie** — Le test anti-fuite B8.2 couvre au minimum le cas de référence « push vers un invité après révocation » → attendu interdit ; aucun accès `PLAYER_PRIVATE` non-auteur n'est observé sur le jeu de cas testé, y compris pour `OWNER`/`GM`.

### 3.6 Axe transverse — Accessibilité (dérivé NFR-ACC-01→04)

**Objet** — Vérifier les exigences d'accessibilité dérivées des NFR-ACC-01 à 04.

**Périmètre** :
- Parcours clavier complet, MJ et joueur, sans dispositif de pointage (NFR-ACC-01).
- Focus visible en permanence, ordre de tabulation cohérent avec la logique visuelle de la page (NFR-ACC-01).
- Compatibilité avec les lecteurs d'écran + annonce des changements d'état sans action de l'utilisateur (NFR-ACC-02).
- Lisibilité en conditions de faible éclairage — contraste texte/arrière-plan (NFR-ACC-03).
- Tailles de texte adaptées à une lecture rapide en session (NFR-ACC-04).

**Critères d'entrée** — L'interface Angular des parcours MJ et joueur est implémentée pour le périmètre testé.

**Critères de sortie** — Chaque parcours clavier listé est exécutable sans dispositif de pointage ; chaque changement d'état pertinent déclenche une annonce assistive observable.

**Précision de calibrage** : le référentiel WCAG 2.1 niveau AA est cité par les NFR sources comme **calibrage d'implémentation** — il oriente la réalisation sans se substituer au besoin produit formulé dans les NFR. Ce document ne convertit pas WCAG 2.1 AA en critère de recette produit ; il reste un repère technique pour l'implémentation.

### 3.7 Axe transverse — Réactivité perçue (dérivé NFR-PERF-01→04)

**Objet** — Vérifier les exigences de réactivité perçue dérivées des NFR-PERF-01 à 04, en observation qualitative.

**Périmètre** :
- Fluidité de la navigation en session (NFR-PERF-01).
- Réactivité perçue à la première interaction (NFR-PERF-02).
- Continuité sans interruption perceptible pendant une session (NFR-PERF-03).
- Résultats de recherche perçus comme immédiats (NFR-PERF-04).

**Critères d'entrée** — Le parcours ou la fonctionnalité concernée est implémenté en environnement représentatif (pas nécessairement en charge).

**Critères de sortie** — Observation qualitative documentée (fluidité perçue, absence d'interruption observée, réactivité jugée immédiate) pour chacun des 4 NFR ; aucun seuil chiffré n'est exigé pour clore ce niveau.

**Marqueur explicite** : aucun seuil chiffré (ms, percentile p95) n'est dérivable du corpus pour ces quatre exigences — l'instrumentation produit (télémétrie) n'a pas fixé de valeur de décision. Tout critère de recette perf chiffré est un point ouvert (§12), jamais une valeur inventée ici.

---

## 4. Types de test

| Type | Se rattache à |
|---|---|
| Fonctionnel | Tous les UC du MVP (UC-01 à UC-12, UC-14) |
| Sécurité | Autorisation (B5.1/B5.2), authentification (B1.5/B3.2), anti-fuite temps réel (B8.2), confidentialité `PLAYER_PRIVATE`/`GM_ONLY` (UC-04, UC-06) |
| Confidentialité / RGPD | NFR-CONF-01→04, suppression effective des données personnelles, isolation mode local |
| Migration de données | P7, NFR-OFF-02 (durabilité), export/import versionné (UC-01 US-01-05/07) |
| Hors-connexion / résilience | NFR-OFF-01→04, e2e hors connexion intégral, résilience réseau UC-06 (E1) |
| Accessibilité | NFR-ACC-01→04 |
| Internationalisation | NFR-I18N-01→03 (contenu MJ toute langue, y compris scripts non-latins) |
| Réactivité perçue | NFR-PERF-01→04 |

---

## 5. Environnements & données de test

**Mode local** — Store IndexedDB navigateur sans serveur ; object stores `folders` / `documents` / `document_blocks` / `document_links` / `document_tags` / `document_types`, plus un store racine (**nom exact non figé — watch point de build** : résidu `campaigns` vs `spaces`, non encore propagé depuis le renommage `ADR-018` ; ne pas figer ce nom dans un critère de recette). `navigator.storage.persist()` accordé et refusé-best-effort — les deux branches sont à simuler. CSP `connect-src 'self'` observable (test « aucun appel réseau »).

**Migration** — Export en enveloppe versionnée (`schemaVersion`, `exportedAt`, `appVersion`, `spaces`) utilisée comme fixture ; dry-run réalisable sans serveur ; côté serveur : transaction par espace, idempotence sur `migration_batch_id`, gate de confirmation `confirmed: true` obligatoire.

**Cloud** — EF Core + PostgreSQL, query filters globaux, colonnes de premier niveau `characterId` / `guestAccessId` ; fenêtres de tokens (access ≤ 15 min, refresh ≤ 7 jours, rotation avec détection de réutilisation par famille).

**Temps réel** — Session `LIVE` avec au moins un invité (`GuestAccess`) ; SignalR forcé en SSE tant que le canal reste unidirectionnel MJ → joueurs ; connexion fermée à la fin de la session `LIVE` ; filtrage par visibilité `PUBLIC` / `PLAYER_PRIVATE` / `GM_ONLY` par événement diffusé.

**Comptes & jeux de données de test** — Compte gratuit (quota 3 espaces `CAMPAIGN`/`ONE_SHOT`, 4 joueurs distincts par session) ; compte supérieur (illimité au MVP) ; compte fédéré-only ; compte non vérifié ; jeux de scénarios multi-scripts (contenu MJ dans toute langue, y compris scripts non-latins, NFR-I18N-03).

**Gestion des données de test** — Données synthétiques uniquement, aucune donnée personnelle réelle ; isolation par environnement (local / test / cloud de recette).

**Limites de périmètre** — La synchronisation multi-appareils hors connexion est **exclue du MVP** : aucun cas de recette de ce document ne la couvre.

---

## 6. Organisation, rôles & responsabilités

Table calibrée pour une équipe restreinte (développeur·se solo ou équipe réduite prenant en charge implémentation et recette).

| Niveau de test | Conçoit | Exécute | Valide |
|---|---|---|---|
| Unitaire | dev | dev (automatisé, CI) | dev (revue de code ou auto-revue) |
| Intégration | dev | dev (automatisé, CI) | dev |
| e2e | dev | dev (automatisé ou manuel scénarisé) | dev, revue ponctuelle |
| Test d'architecture (CI) | dev | CI (automatisé) | CI (bloquant si rouge) |
| Sécurité (adversarial) | dev | dev (checklist B5.2/B8.2/B3.2) | dev, revue dédiée avant jalon Cloud/Partage |
| Accessibilité / Réactivité perçue | dev | dev (observation manuelle) | dev |
| Gates humains (`[GATE MARCHÉ]`, `[GATE JURISTE EU]`) | opérateur | opérateur (télémétrie) / juriste externe | opérateur |

Cette table reste factuelle : elle ne présuppose ni rôle QA dédié ni outillage de gestion de test spécifique — dev désigne la personne en charge de l'implémentation, seule ou en équipe réduite.

---

## 7. Processus de test & gestion des anomalies

**Cycle de vie d'une anomalie** : `Nouvelle → Confirmée → Corrigée → Vérifiée → Close`.

- **Nouvelle** : anomalie signalée, non encore reproduite.
- **Confirmée** : anomalie reproduite et qualifiée (sévérité, priorité assignées).
- **Corrigée** : correctif appliqué, en attente de vérification.
- **Vérifiée** : correctif vérifié sur l'environnement cible.
- **Close** : anomalie clôturée, aucune action supplémentaire requise.

**Échelle de sévérité** :

| Sévérité | Définition |
|---|---|
| Critique | Perte de données, faille de confidentialité (accès `PLAYER_PRIVATE` non autorisé, fuite temps réel), blocage total d'un parcours Must |
| Majeure | Fonctionnalité Must dégradée sans contournement praticable |
| Mineure | Fonctionnalité Should/Could dégradée, ou Must avec contournement praticable |
| Cosmétique | Écart visuel ou de formulation sans impact fonctionnel |

**Échelle de priorité** : ordonnance le traitement indépendamment de la sévérité (une anomalie mineure peut être prioritaire si elle bloque une autre recette en cours). L'échelle de priorité elle-même (ex. P1-P4, ou tout autre barème) reste à la main de l'équipe de build — aucun barème de priorité n'est dérivable du corpus, cohérent avec la discipline zéro-décision-neuve de ce document (§1).

**Critères de suspension des tests** : une anomalie Critique ouverte sur un parcours Must du jalon en cours de recette suspend la recette de ce parcours jusqu'à correction.

**Critères de reprise** : la recette reprend une fois l'anomalie Critique passée en statut Vérifiée sur l'environnement de recette.

**Journal des anomalies** : le suivi nominatif des anomalies (ouverture, statut, assignation) est un artefact de runtime tenu hors de ce document — ce cahier définit le cycle de vie et les échelles (ci-dessus), il ne tient pas lui-même de registre d'anomalies vivant.

---

## 8. Critères d'entrée/sortie globaux & Definition of Done de test

**Critères d'entrée en recette** :
- Le build du jalon concerné est déployable sur l'environnement de recette.
- Les données de test (comptes, jeux de scénarios, fixtures d'export) sont prêtes.
- L'environnement de recette est disponible et isolé des données réelles.

**Critères de sortie de recette** :
- Tous les cas de recette de priorité Must du périmètre concerné ont été exécutés.
- Zéro anomalie Critique ou Majeure ouverte sur ce périmètre.
- Les points ouverts identifiés sont tracés dans le registre (§12), pas laissés implicites.

**Definition of Done de test, par use case** : un UC est considéré comme recetté quand chaque critère d'acceptation tracé dans sa table de recette (§10) porte un cas exécuté avec un verdict — `OK`, `KO` ou `N/A` justifié —, sans case Must laissée à blanc.

---

## 9. Critères de sortie par jalon

Quatre jalons de build et deux gates humains hors-CI, nommés par leur contenu (Socle / Local-only / Cloud + migration / Partage + temps réel) — reprise fidèle des critères de sortie factuels de `docs/planning/roadmap-entree-build.md §3`.

### Jalon « Socle »

- Les 8 ADR de nature pré-implémentation portent une confirmation explicite (date + décideur) à l'entrée en build.
- Le squelette Domain/Application compile et respecte la structure documentée.
- Le test d'architecture CI existe, tourne en pipeline, et échoue si une référence traverse une frontière de bounded context non autorisée.
- L'interface `IResourceAccessPolicy` est déclarée en couche Application — son câblage complet (pipeline REST + filtre SignalR) reste un critère des jalons suivants.

### Jalon « Local-only »

- Le store IndexedDB local est opérationnel et couvre le périmètre défini.
- `navigator.storage.persist()` est appelé à l'entrée en mode local ; le résultat (accordé / refusé-best-effort) est lu et déclenche le bandeau de durabilité si nécessaire.
- Un export au format `schemaVersion` peut être produit depuis le store local et est structurellement rejouable (dry-run sans serveur cloud actif).
- Aucun appel réseau vers l'API n'existe dans le périmètre mode local livré — observable via la CSP `connect-src 'self'`.

### [GATE MARCHÉ] — hors-CI, ne pas convertir en dette de test

Décision go/no-go de l'opérateur sur la télémétrie des trois piliers d'activation (préparation, vue session, partage), croisée avec les hypothèses de validation H1-H4. Les seuils de décision ne sont pas fixés par la roadmap — ils relèvent de l'instrumentation produit. **Critère de sortie = décision opérateur tracée.** Aucun test automatisé, aucun linter, aucune build ne peut trancher ce gate.

### Jalon « Cloud + migration »

- Les query filters EF Core P2/P3 sont actifs sur tous les chemins de lecture (énumération, lecture par ID, projections).
- Le pipeline behavior REST et le filtre de diffusion SignalR appellent le même `IResourceAccessPolicy.CanAccess(...)`.
- Un export du jalon « Local-only » peut être migré de bout en bout : dry-run, rapport de rejets le cas échéant, import transactionnel par espace, idempotence vérifiée sur rejeu du même `migration_batch_id`.
- `AccountSuspended` et `UserAnonymized` déclenchent effectivement `ITokenDenylist.RevokeFamilyAsync`.
- La saga `SpaceDeleted` s'exécute sans erreur de cycle FK sur un espace de test couvrant les deux cycles documentés.

### [GATE JURISTE EU] — hors-CI, ne pas convertir en dette de test

Bloque le lancement commercial EU — pas le build technique, qui peut être instruit en parallèle. Cinq axes RGPD, chacun devant porter un statut distinct de « à valider juriste » (nature binaire par axe) : article 8 (mineurs) ; article 28 + DPA sous-traitant ; intérêt légitime post-effacement et `display_name` invité ; article 17 (hard-delete `PERSONAL`) ; facette RGPD « reclaim-in-place ». Ce gate n'est pas converti en dette de test.

### Jalon « Partage + temps réel »

- Le filtre SignalR appelle `IResourceAccessPolicy.CanAccess` avant tout push.
- Déconnexion forcée du hub sur `GuestAccessRevoked` / `GuestAccessExpired`.
- Le token `GuestAccess` est absent de toute URL et de tout journal.
- Le test anti-fuite B8.2 couvre au minimum le cas de référence « push vers un invité après révocation du `GuestAccess` » → attendu interdit.
- Le seuil de repli SSE → polling adaptatif est un point ouvert (§12) — bloquant pour la clôture opérationnelle du jalon, pas pour son entrée.

---

## 10. Cahier de recette par use case

Cette section produit une table de recette par use case du MVP. Chaque case de recette transpose un scénario Gherkin source (posture consommateur, §1) en précondition/étapes/résultat attendu, ou — pour les trous de nature (a) — une règle métier sans Gherkin porteur, marquée comme telle.

**Format de la table** :

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|

- **ID** : identifiant `CR-UCNN-KK` (§1).
- **Cas** : intitulé court du cas de recette, avec sa nature entre crochets (nominal / alternatif / erreur / limite / irréversible / sécurité).
- **Préconditions**, **Étapes**, **Résultat attendu** : transposition directe du scénario Gherkin source (Given/When/Then ou Étant donné/Quand/Alors).
- **Source** : `US-UCNN-KK §"<titre du scénario Gherkin>"`, ou `RB-NN-xx (dérivé, pas de Gherkin)` pour un cas de nature (a).
- **Verdict** : `☐ OK ☐ KO ☐ N/A`, renseigné en phase d'exécution de la recette.

---

### UC-01 — Mode local sans compte (Must / Should)

**Environnement(s)** : mode local (US-01-05 couvre la transition local → cloud).
**Sources US** : US-01-01, US-01-02, US-01-04, US-01-05, US-01-06, US-01-07, US-01-09.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC01-01 | [nominal] Commencer sans compte | Le MJ ouvre l'application pour la première fois | Il choisit « Commencer sans compte » | L'application affiche un message court sur le stockage local ; un lien FAQ est disponible ; le MJ est redirigé vers l'écran de création de campagne | US-01-01 §"Le MJ choisit de commencer sans compte" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-02 | [nominal] Message informatif au 1er démarrage | Le MJ ouvre l'application pour la première fois | Il choisit de commencer sans compte | Le MJ reçoit un message informatif court sur le stockage local lors du premier démarrage | US-01-01 §"Le MJ reçoit un message informatif au premier démarrage" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-03 | [nominal] Créer une campagne en mode local | Le MJ est en mode local sans compte | Il crée une campagne et y ajoute du contenu | Aucune donnée n'est envoyée au serveur ; le contenu est disponible dans la session courante | US-01-01 §"Le MJ crée une campagne en mode local" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-04 | [erreur] 4e campagne locale refusée | Le MJ est en mode local sans compte et a déjà 3 campagnes créées | Il tente de créer une 4e campagne | La création est bloquée ; un message invite à créer un compte pour un stockage cloud illimité | US-01-01 §"Le MJ atteint la limite de 3 campagnes en mode local" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-05 | [nominal] Retour après fermeture du navigateur | Le MJ a créé du contenu en mode local et a fermé le navigateur | Il rouvre l'application | Ses campagnes et contenus sont disponibles | US-01-02 §"Retour après fermeture du navigateur" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-06 | [alternatif] Partage visible mais désactivé | Le MJ est en mode local sans compte | Il accède à une fonctionnalité de partage (UC-08) ou d'accès joueur (UC-09) | La fonctionnalité est visible et désactivée ; un CTA invite à créer un compte | US-01-04 §"Fonctionnalité de partage visible mais désactivée" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-07 | [alternatif] Fonctions cloud distinguables, aucune masquée | Le MJ est en mode local sans compte | Il navigue dans l'application | Les fonctionnalités locales sont pleinement accessibles ; les fonctionnalités cloud sont distinguables visuellement (cadenas/label) ; aucune n'est masquée | US-01-04 §"Le MJ comprend ce qui est accessible sans compte" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-08 | [nominal] Compte créé depuis invite contextuelle, migration après gate | Le MJ est en mode local sans compte et a tenté d'accéder à une fonctionnalité cloud | Il crée un compte via le CTA | Il est redirigé vers UC-10 ; après création, le gate de reconnaissance est présenté puis la migration démarre après confirmation ; il retrouve ses campagnes et contenus en cloud | US-01-05 §"Le MJ crée un compte depuis une invite contextuelle" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-09 | [alternatif] Création de compte depuis les paramètres | Le MJ est en mode local sans compte | Il accède aux paramètres et choisit de créer un compte | Le même flow de migration automatique est déclenché | US-01-05 §"Le MJ crée un compte depuis les paramètres" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-10 | [alternatif] Gate de reconnaissance avant migration | Le MJ vient de créer un compte et des données locales existent | La création de compte est finalisée | Les campagnes locales détectées (titre, volume, date) sont présentées ; confirmation explicite demandée ; la migration ne démarre qu'après confirmation | US-01-05 §"Le gate de reconnaissance est présenté avant la migration" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-11 | [erreur] Échec de migration d'une campagne | Le MJ vient de créer un compte et a confirmé la migration | La migration d'une campagne échoue (slug collision, properties invalides, type inconnu, version non supportée) | Un rapport de rejets détaille la raison ; les données locales rejetées restent intactes ; les autres campagnes migrées restent accessibles | US-01-05 §"La migration échoue pour une campagne" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-12 | [alternatif] Données effacées (distinct de 1re visite) | Le MJ a déjà utilisé l'application en mode local et les données locales ont été supprimées | Le MJ rouvre l'application | Un message explicite indique que les données précédentes sont introuvables, distinct du message de première visite ; le MJ peut créer une nouvelle campagne ou se connecter | US-01-06 §"Données effacées (cache vidé)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-13 | [nominal] Première visite réelle | Le MJ ouvre l'application pour la première fois | L'application ne trouve aucune donnée locale | Le message affiché est celui d'un accueil, pas d'un avertissement de perte de données | US-01-06 §"Première visite réelle" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-14 | [erreur] Stockage navigateur plein | Le MJ est en mode local | Le stockage local du navigateur atteint sa limite | Un message indique que le stockage est plein ; l'application propose de migrer vers le cloud | US-01-06 §"Stockage navigateur plein" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-15 | [nominal] Export en mode local | Le MJ est en mode local sans compte et a au moins une campagne | Il accède aux paramètres et déclenche l'export | Un fichier de sauvegarde au format ouvert est généré et téléchargé, contenant l'intégralité des données de la campagne | US-01-07 §"Le MJ exporte sa campagne depuis les paramètres (mode local)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-16 | [nominal] Export en compte cloud | Le MJ est connecté avec un compte | Il accède aux paramètres et déclenche l'export d'une campagne | Un fichier de sauvegarde au format ouvert est généré et téléchargé, contenant l'intégralité des données | US-01-07 §"Le MJ exporte sa campagne depuis les paramètres (compte cloud)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-17 | [nominal] Créer un document sans campagne | Le MJ est en mode local sans compte et aucune campagne n'a été créée | Il crée un document (lieu, PNJ, scénario ou objet) | Le document est enregistré dans l'espace personnel par défaut ; aucune campagne requise ; aucune donnée envoyée au serveur | US-01-09 §"Le MJ crée un document sans avoir créé de campagne" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-18 | [nominal] Retrouver le contenu capturé | Le MJ a capturé du contenu dans l'espace personnel en mode local | Il navigue vers l'espace personnel | Tous les documents capturés sont visibles et accessibles | US-01-09 §"Le MJ retrouve le contenu capturé" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-19 | [alternatif] Rattacher à une campagne ultérieurement | Le MJ a du contenu dans l'espace personnel et a créé (ou crée) une campagne | Il décide de rattacher un document de l'espace personnel à cette campagne | Le document est déplacé ou instancié dans la campagne ; l'espace personnel ne contient plus ce document s'il a été déplacé | US-01-09 §"Le MJ range le contenu dans une campagne ultérieurement" | ☐ OK ☐ KO ☐ N/A |
| CR-UC01-20 | [sécurité, dérivé] Aucun secret stocké en mode local | Le MJ utilise l'application en mode local | Inspection du stockage local du navigateur | Aucun élément protégé (jeton d'accès, secret, identifiant de connexion à un compte) n'est conservé dans le stockage du navigateur — propriété distincte de l'absence de requête réseau (CR-UC01-03/17) | RB-01-15 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

**Trou traité (b) — non comblé** : US-01-08 (réimport de fichier de sauvegarde) est post-MVP — sa recette est différée, aucun cas n'est produit ici (§12).

---

### UC-02 — Créer un espace de jeu (Must / Should)

**Environnement(s)** : local + cloud (FREE/PRO). L'espace `PERSONAL` préexiste dans les deux cas.
**Sources US** : US-02-00, US-02-01, US-02-03. (US-02-02, US-02-04 : post-MVP, dépendent UC-13 — non recettées ici.)

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC02-01 | [nominal] Compte créé → PERSONAL immédiat | Un MJ vient de créer son compte | Il accède à son tableau de bord pour la première fois | Un espace `PERSONAL` est déjà présent, contenant le dossier virtuel « Non classés » ; aucune action de création requise | US-02-00 §"Le MJ crée un compte et dispose immédiatement d'un espace personnel" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-02 | [nominal] Mode local → PERSONAL conteneur par défaut | Le MJ utilise l'application en mode local sans compte | Il accède à l'application pour la première fois | Un espace `PERSONAL` est présent comme conteneur par défaut, avec le dossier virtuel « Non classés » | US-02-00 §"Le MJ en mode local dispose d'un espace personnel comme conteneur par défaut" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-03 | [limite] PERSONAL non décompté du quota FREE | Le MJ possède un compte gratuit et son espace PERSONAL est actif | Le système calcule le quota d'espaces actifs | L'espace PERSONAL n'est pas décompté ; le MJ dispose de 3 emplacements CAMPAIGN/ONE_SHOT disponibles | US-02-00 §"L'espace personnel n'apparaît pas dans le quota FREE" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-04 | [nominal] Campagne avec nom seul | Le MJ est connecté, quota non atteint | Il choisit « Nouvelle campagne », renseigne uniquement un nom, valide | Un espace de campagne est créé ; les 4 dossiers système + « Non classés » sont créés automatiquement ; redirection vers le nouvel espace | US-02-01 §"Le MJ crée une campagne avec le nom uniquement" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-05 | [nominal] Campagne avec tous les champs | Le MJ est connecté, quota non atteint | Il renseigne nom, description courte et système de jeu, valide | L'espace est créé avec les informations fournies ; dossiers système + « Non classés » créés | US-02-01 §"Le MJ crée une campagne avec tous les champs" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-06 | [erreur] Sans nom → refus | Le MJ accède au formulaire de création de campagne | Il valide sans renseigner de nom | Un message d'erreur indique que le nom est obligatoire ; aucun espace n'est créé | US-02-01 §"Le MJ tente de créer une campagne sans renseigner de nom" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-07 | [alternatif] Sans système de jeu (mode générique) | Le MJ accède au formulaire de création de campagne | Il valide sans renseigner de système de jeu | L'espace est créé en mode générique ; dossiers système et « Non classés » créés avec labels neutres | US-02-01 §"Le MJ crée une campagne sans système de jeu" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-08 | [nominal] Création en mode local | Le MJ utilise l'application en mode local et a moins de 3 espaces créés | Il crée une campagne avec un nom | L'espace est créé et pleinement fonctionnel ; dossiers système + « Non classés » créés | US-02-01 §"Le MJ est en mode local et crée une campagne" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-09 | [erreur] FREE 4e espace actif refusé | Le MJ possède un compte gratuit et a déjà 3 espaces CAMPAIGN/ONE_SHOT actifs | Il tente de créer une nouvelle campagne ou lancer un one-shot | La création est bloquée ; message de limite de 3 espaces ; CTA vers PRO ; PERSONAL non mentionné dans le décompte | US-02-03 §"Le MJ gratuit tente de créer un 4e espace CAMPAIGN ou ONE_SHOT actif" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-10 | [alternatif] FREE débloqué après archivage | Le MJ possède un compte gratuit avec 3 espaces actifs | Il archive l'un de ses espaces puis tente une création | La création est autorisée | US-02-03 §"Le MJ gratuit peut créer un nouvel espace après avoir archivé une campagne" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-11 | [erreur] Local 4e espace refusé | Le MJ utilise l'application en mode local et a déjà 3 espaces créés | Il tente de créer une nouvelle campagne ou lancer un one-shot | La création est bloquée ; message de limite en mode local ; CTA vers création de compte | US-02-03 §"Le MJ en mode local tente de créer un 4e espace CAMPAIGN ou ONE_SHOT" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-12 | [alternatif] PRO jamais bloqué | Le MJ possède un compte PRO avec 3 espaces actifs ou plus | Il tente de créer une nouvelle campagne | La création est autorisée sans restriction | US-02-03 §"Le MJ PRO n'est jamais bloqué" | ☐ OK ☐ KO ☐ N/A |
| CR-UC02-13 | [erreur, dérivé] Erreur de sauvegarde à la création | Le MJ soumet un formulaire de création d'espace valide | Une erreur survient lors de la sauvegarde | Le système affiche un message d'erreur et conserve les données saisies ; aucun espace partiel n'est créé | UC-02 fiche §Exceptions E2 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

**Trous traités** : US-02-02 et US-02-04 sont post-MVP (dépendent UC-13) — non recettées ici. UC-02 exception E2 (« erreur de création ») a été traitée en case dérivée (CR-UC02-13) plutôt qu'en point ouvert, la fiche UC-02 la spécifiant explicitement (message d'erreur + conservation des données saisies).

---

### UC-03 — Structurer un scénario (Must / Should)

**Environnement(s)** : contexte d'espace (local ou cloud).
**Sources US** : US-03-01, US-03-02, US-03-03, US-03-04, US-03-05, US-03-06, US-03-07.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC03-01 | [nominal] Créer un scénario avec titre uniquement | Le MJ est dans la vue de son espace, section Scénarios | Il crée un scénario en saisissant uniquement un titre | Le scénario est créé et apparaît dans la liste des scénarios de l'espace | US-03-01 §"Créer un scénario avec titre uniquement" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-02 | [nominal] Titre seul champ obligatoire | Le MJ est dans le formulaire de création de scénario | Il soumet sans remplir les champs optionnels (résumé, contexte, objectif) | Le scénario est créé sans erreur | US-03-01 §"Le titre est le seul champ obligatoire" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-03 | [nominal] Scénario improvisé minimal valide | Le MJ crée un scénario avec titre uniquement | La création est confirmée | Le scénario est fonctionnel et accessible dans l'espace sans configuration supplémentaire | US-03-01 §"Un scénario improvisé minimal est valide (A4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-04 | [nominal] Ajouter une scène | Le MJ est dans l'éditeur d'un scénario existant | Il ajoute une nouvelle scène | La scène apparaît dans le scénario à la position attendue | US-03-02 §"Ajouter une scène à un scénario existant" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-05 | [nominal] Ordre des scènes respecté | Un scénario avec plusieurs scènes ordonnées | Le MJ consulte le scénario | Les scènes s'affichent dans l'ordre défini | US-03-02 §"Les scènes respectent l'ordre défini" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-06 | [nominal] Modifier titre et contenu d'une scène | Le MJ est dans l'éditeur d'une scène | Il modifie le titre et le contenu | Les modifications sont sauvegardées et visibles dans le scénario | US-03-02 §"Modifier le titre et le contenu d'une scène" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-07 | [alternatif] Supprimer une scène du scénario | Un scénario contenant une scène | Le MJ supprime cette scène du scénario | Le lien scène↔scénario est supprimé ; le document de scène n'est pas supprimé automatiquement | US-03-02 §"Supprimer une scène du scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-08 | [limite] Scénario zéro scène valide | Un scénario sans aucune scène ajoutée | Le MJ consulte ce scénario | Le scénario est valide et fonctionnel | US-03-02 §"Un scénario peut contenir zéro scène" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-09 | [nominal] Scénario monobloc sans scènes | Le MJ a créé un scénario | Il rédige du contenu libre dans les blocs sans ajouter de scènes | Le scénario est valide et accessible dans la campagne | US-03-03 §"Écrire un scénario monobloc sans scènes" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-10 | [alternatif] Ajouter des scènes a posteriori | Un scénario existant sans aucune scène | Le MJ décide d'y ajouter des scènes | Les scènes sont ajoutées sans perte du contenu libre existant | US-03-03 §"Ajouter des scènes à un scénario libre a posteriori" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-11 | [nominal] Ajouter une révélation | Le MJ édite une scène de son scénario | Il clique sur « Ajouter une révélation » | Un document lié à la scène est créé automatiquement (visibility = privé MJ), affiché dans la section « Révélations joueurs » | US-03-04 §"Le MJ ajoute une révélation depuis l'éditeur de scène" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-12 | [alternatif] Contenu privé et partageable séparés | Le MJ a une scène avec notes privées et une révélation joueurs | — | Le document de scène principal est privé MJ ; le document de révélation est distinct ; un joueur ne voit pas le document de scène privé MJ | US-03-04 §"Le contenu privé et le contenu partageable sont bien séparés" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-13 | [alternatif] Lier un document existant à un scénario | Le MJ est dans l'éditeur d'un scénario | Il recherche et sélectionne un document existant à lier | Le document apparaît dans la liste des documents liés du scénario | US-03-05 §"Lier un document existant à un scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-14 | [alternatif] Lier un document existant à une scène | Le MJ est dans l'éditeur d'une scène | Il recherche et sélectionne un document existant à lier | Le document apparaît dans la liste des documents liés de la scène | US-03-05 §"Lier un document existant à une scène" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-15 | [alternatif] Supprimer un lien sans supprimer la cible | Un scénario avec un document lié (ex. PNJ) | Le MJ supprime ce lien depuis l'éditeur | Le lien est supprimé ; le document cible n'est pas supprimé et reste accessible | US-03-05 §"Supprimer un lien sans supprimer le document cible" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-16 | [alternatif] Backlinks depuis la fiche d'un PNJ | Un PNJ référencé dans un ou plusieurs scénarios | Le MJ consulte la fiche du PNJ | Il voit la liste des scénarios qui référencent ce PNJ | US-03-05 §"Voir les backlinks depuis la fiche d'un PNJ" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-17 | [nominal] Définir le statut d'un scénario | Le MJ est dans l'éditeur ou la fiche d'un scénario | Il choisit un statut (brouillon, prêt, joué, archivé) | Le statut est enregistré et visible dans la liste des scénarios | US-03-06 §"Définir le statut d'un scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-18 | [nominal] Statut par défaut brouillon | Le MJ crée un scénario sans choisir de statut explicite | Le scénario est créé | Son statut est « brouillon » par défaut | US-03-06 §"Statut par défaut à la création" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-19 | [alternatif] Filtrer les scénarios par statut | Le MJ consulte la liste des scénarios | Il applique un filtre sur un statut donné | Seuls les scénarios ayant ce statut sont affichés | US-03-06 §"Filtrer les scénarios par statut" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-20 | [nominal] Créer un document à la volée depuis l'éditeur de scénario | Le MJ est dans l'éditeur d'un scénario ou d'une scène | Il crée un nouveau document d'un type donné depuis l'éditeur | Le document est créé, automatiquement lié, et accessible depuis son dossier de type | US-03-07 §"Créer un document à la volée depuis l'éditeur de scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC03-21 | [erreur, dérivé] Titre manquant refusé | Le MJ est dans le formulaire de création de scénario | Il valide sans renseigner de titre | La création est refusée, le titre étant obligatoire | UC-03 fiche §Exceptions E1 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

**Trou traité (b) — non comblé** : UC-03 exception E2 (« perte de connexion ou erreur de sauvegarde ») n'est pas spécifiable en case de recette au-delà du principe général déjà couvert par NFR-OFF § résilience (voir §5, Limites de périmètre) — renvoyé au registre des points ouverts (§12), aucun cas produit ici.

---

### UC-04 — Gérer les documents d'un espace (Must / Should / Could)

**Environnement(s)** : contexte d'espace (campagne ou personnel).
**Sources US** : US-04-01, US-04-02, US-04-03, US-04-04, US-04-05, US-04-06.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC04-01 | [alternatif] Supprimer un document (masquage logique) | Le MJ dispose d'un document visible dans son espace | Il choisit de supprimer ce document | Le document est masqué de la bibliothèque active ; il n'apparaît plus dans les listes courantes | US-04-01 §"Le MJ supprime un document" | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-02 | [alternatif] Déplacer un document vers un autre dossier | Le MJ dispose d'un document dans un dossier | Il déplace ce document vers un autre dossier de l'espace | Le document apparaît dans le dossier cible et n'apparaît plus dans le dossier source | US-04-01 §"Le MJ déplace un document vers un autre dossier" | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-03 | [nominal, dérivé] Créer un document avec titre seul | Le MJ accède à la bibliothèque, un dossier ou un raccourci de création | Il crée un document en renseignant uniquement un titre | Le document est créé ; type, propriétés, tags et visibilité restent optionnels | RB-04-01..05 (dérivé, pas de Gherkin — critères checkbox US-04-01) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-04 | [alternatif, dérivé] Type optionnel non bloquant | Le MJ crée ou modifie un document | Il laisse le type non renseigné ou en change | Le document reste pleinement fonctionnel, libre ou typé, sans perte du contenu en blocs | RB-04-01..05 (dérivé, pas de Gherkin — critères checkbox US-04-01) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-05 | [nominal, dérivé] Modifier un document existant | Un document existe | Le MJ modifie titre, blocs, propriétés et type | Les modifications sont enregistrées sur le document existant | RB-04-01..05 (dérivé, pas de Gherkin — critères checkbox US-04-01) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-06 | [nominal, dérivé] Créer une note sans renseigner aucun champ | Le MJ déclenche la création rapide d'une note | Il valide sans renseigner aucun champ | La note est créée avec un titre généré automatiquement côté interface | dérivé de critères US-04-02 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-07 | [nominal, dérivé] Titre auto-généré non vide | Une note rapide est créée sans titre saisi | — | Un titre non vide est généré côté interface (date/heure ou début de contenu) | dérivé de critères US-04-02 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-08 | [nominal, dérivé] Dossier « Notes » par défaut | Une note rapide est créée sans dossier explicite | — | La note est placée dans le dossier « Notes » par défaut | dérivé de critères US-04-02 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-09 | [nominal, dérivé] Note retrouvable/modifiable/liable comme document standard | Une note rapide a été créée | Le MJ la recherche, la modifie ou la lie | La note se comporte comme n'importe quel autre document | dérivé de critères US-04-02 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-10 | [alternatif, dérivé] Note renommable après création | Une note rapide existe | Le MJ la renomme | Le nouveau titre est enregistré | dérivé de critères US-04-02 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-11 | [nominal] MJ partage un document | Le MJ dispose d'un document privé | Il choisit de partager ce document | Le document devient visible par les joueurs, accessible par tous les membres de la campagne | US-04-03 §"Le MJ partage un document" | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-12 | [alternatif] MJ retire le partage | Le MJ dispose d'un document partagé | Il choisit de retirer le partage | Le document redevient privé, n'est plus visible par les joueurs | US-04-03 §"Le MJ retire le partage d'un document" | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-13 | [erreur] Joueur ne voit pas un document privé | Un document est privé | Un joueur consulte la bibliothèque de la campagne | Le document n'apparaît pas dans sa vue | US-04-03 §"Un joueur ne voit pas un document privé" | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-14 | [alternatif, dérivé] Ajouter/retirer un tag | Un document existe | Le MJ ajoute ou retire un tag | Le tag est associé ou retiré ; le document sans tag reste pleinement fonctionnel | dérivé de critères US-04-04 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-15 | [limite, dérivé] Document sans tag pleinement fonctionnel | Un document n'a aucun tag | — | Le document reste pleinement utilisable | dérivé de critères US-04-04 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-16 | [alternatif, dérivé] Lier un document existant depuis sa fiche | Le MJ consulte la fiche d'un document | Il recherche et lie un document existant | Le document lié est visible depuis l'éditeur du document source | dérivé de critères US-04-05 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-17 | [alternatif, dérivé] Supprimer un lien sans supprimer le document cible | Deux documents sont liés | Le MJ supprime le lien | Le lien est supprimé ; le document cible n'est pas supprimé | dérivé de critères US-04-05 (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-18 | [alternatif, dérivé, priorité basse] Consulter les backlinks d'un document | Un document est référencé par d'autres documents | Le MJ consulte la fiche du document | La liste des documents référençant ce document est visible ; section vide sans erreur si aucun | dérivé de critères US-04-06 — Could Have, priorité basse (pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-19 | [erreur, dérivé] Contenu vide refusé sans titre | Le MJ soumet la création d'un document | Il valide sans titre | La création est refusée ; un document avec titre mais sans blocs peut être un brouillon valide | UC-04 fiche §Exceptions E1 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-20 | [erreur, sécurité, dérivé] Accès joueur non autorisé | Un joueur tente d'accéder à un document privé ou une note personnelle qui ne lui appartient pas | Le joueur tente l'accès direct | Le système refuse l'accès | UC-04 fiche §Exceptions E2 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC04-21 | [sécurité, dérivé] Anti-énumération : accès direct à un document non partagé ne révèle pas son existence | Un document existe mais n'est pas partagé (`visibility != PUBLIC`) | Un joueur tente d'accéder directement à l'adresse de ce document (identifiant direct) | L'accès est refusé ; la tentative ne révèle pas l'existence du document — aucune distinction observable entre « existe et privé » et « n'existe pas » | NFR-CONF-01 §Situation limite — tentative d'accès à un document non partagé (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-05 — Organiser par dossiers (Must / Should)

**Environnement(s)** : contexte d'espace (local ou cloud).
**Sources US** : US-05-01, US-05-02, US-05-03, US-05-04, US-05-05.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC05-01 | [nominal] Créer un dossier avec nom valide | Le MJ dispose d'un espace | Il crée un dossier avec un nom (ex. « Factions ») | Le dossier est créé et apparaît en navigation | US-05-01 §"Le MJ crée un dossier avec un nom valide" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-02 | [erreur] Nom vide refusé | Le MJ dispose d'un espace | Il soumet la création d'un dossier avec un nom vide | La création est refusée ; un message d'erreur est affiché | US-05-01 §"Le MJ tente de créer un dossier avec un nom vide" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-03 | [nominal] Renommer un dossier système, isSystem reste true | La campagne dispose du dossier système « Personnages » | Le MJ le renomme en « Factions » | Le dossier s'appelle « Factions » en navigation ; son flag `isSystem` reste `true` | US-05-02 §"Le MJ renomme un dossier système" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-04 | [erreur] Renommer avec un nom vide refusé | Le MJ dispose d'un dossier « Notes » | Il soumet le renommage avec un nom vide | Le renommage est refusé ; un message d'erreur est affiché | US-05-02 §"Le MJ tente de renommer avec un nom vide" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-05 | [nominal] Associer un template à un dossier | Le dossier « Factions » n'a pas de template ; le document « Fiche Faction » est marqué modèle réutilisable | Le MJ associe « Fiche Faction » comme template du dossier | `defaultTemplateDocumentId` du dossier pointe vers « Fiche Faction » | US-05-03 §"Le MJ associe un template à un dossier" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-06 | [nominal] Création dans un dossier avec template (copie profonde) | Le dossier « Factions » a un template associé | Le MJ crée un nouveau document dans ce dossier | `Document.Instantiate` est appelé sur le template ; le nouveau document est une copie profonde indépendante | US-05-03 §"Création d'un document dans un dossier avec template" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-07 | [erreur] Template supprimé après association | Le dossier « Factions » a un template associé | Le template est supprimé | `defaultTemplateDocumentId` passe à `null` ; les documents existants du dossier sont inchangés | US-05-03 §"Template supprimé après association" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-08 | [nominal] Supprimer un dossier vide | Le dossier « Indices » est vide | Le MJ le supprime | Le dossier est supprimé et disparaît de la navigation | US-05-04 §"Le MJ supprime un dossier vide" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-09 | [alternatif] Supprimer un dossier non vide — option Non classés | Le dossier « Indices » contient 3 documents | Le MJ le supprime en choisissant « Laisser dans Non classés » | Le dossier est supprimé ; les 3 documents sont rattachés au dossier virtuel « Non classés » | US-05-04 §"Le MJ supprime un dossier non vide — option Non classés" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-10 | [alternatif] Supprimer un dossier non vide — option déplacer | Le dossier « Indices » contient 3 documents | Le MJ le supprime en choisissant de déplacer vers « Notes » | Le dossier est supprimé ; les 3 documents se trouvent dans « Notes » | US-05-04 §"Le MJ supprime un dossier non vide — option déplacer" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-11 | [alternatif] Supprimer un dossier système, même logique | Le dossier système « Scénarios » existe (`isSystem = true`) | Le MJ le supprime | Le dossier est supprimé selon la même logique qu'un dossier standard | US-05-04 §"Le MJ supprime un dossier système" | ☐ OK ☐ KO ☐ N/A |
| CR-UC05-12 | [alternatif] Réordonner les dossiers, ordre persisté | La campagne dispose de dossiers dans un ordre initial | Le MJ modifie l'ordre des dossiers | Le nouvel ordre est persisté ; la navigation le reflète à la prochaine ouverture | US-05-05 §"Le MJ réordonne ses dossiers" | ☐ OK ☐ KO ☐ N/A |

---

### UC-06 — Vue session (Must / Should)

**Environnement(s)** : session `LIVE`/`CLOSED`/`ARCHIVED` ; MJ utilisable en mode local ; vue joueur nécessite compte ou accès invité (pas de mode local côté joueur).
**Sources US** : US-06-01 à US-06-10.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC06-01 | [nominal] Lancer avec un scénario | Le MJ dispose d'une campagne avec un scénario | Il lance une session avec ce scénario | La session est créée avec `status = LIVE` et `scenarioId` renseigné ; la vue session s'ouvre | US-06-01 §"Le MJ lance une session avec un scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-02 | [alternatif] Lancer sans scénario | Le MJ dispose d'une campagne | Il lance une session sans sélectionner de scénario | La session est créée avec `status = LIVE` et `scenarioId = null` ; la vue session s'ouvre | US-06-01 §"Le MJ lance une session sans scénario" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-03 | [erreur] Titre vide refusé au lancement | Le MJ est sur l'écran de lancement | Il soumet sans titre | La création est refusée ; un message d'erreur est affiché | US-06-01 §"Lancement refusé si titre vide" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-04 | [nominal] Configurer sans lancer de session | Le MJ est sur sa campagne | Il ouvre la vue session en mode édition | Il peut ajouter/retirer/réordonner les panneaux ; sauvegarde automatique ; aucune session n'est créée | US-06-02 §"Le MJ configure ses panneaux sans lancer de session" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-05 | [alternatif] Modifier les panneaux en direct pendant LIVE | Une session est en statut `LIVE` | Le MJ modifie les panneaux depuis la vue session | La config est mise à jour et sauvegardée immédiatement ; la session reste en LIVE sans interruption | US-06-02 §"Le MJ modifie les panneaux en direct pendant une session LIVE" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-06 | [nominal] Config persiste entre deux sessions | Le MJ a configuré ses panneaux | Il lance une nouvelle session | La même configuration est chargée automatiquement | US-06-02 §"La config persiste entre deux sessions" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-07 | [nominal] MJ consulte un dossier pendant la session | Une session est LIVE, le dossier « PNJ » est mis en avant | Le MJ ouvre le panneau « PNJ » | Les documents du dossier sont affichés en vue condensée | US-06-03 §"Le MJ consulte un dossier pendant la session" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-08 | [erreur] Joueur ne voit pas les documents non publics | Une session est LIVE, un document « Plan secret » a `visibility != visible par les joueurs` | Le joueur consulte la vue joueur | « Plan secret » n'est pas affiché | US-06-03 §"Le joueur ne voit pas les documents non publics" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-09 | [nominal] Note privée MJ (non visible joueur) | Une session est LIVE | Le MJ saisit une note « Ragnar ment sur son passé » | La note est créée avec `visibility = privé MJ` ; elle n'est pas visible dans la vue joueur | US-06-04 §"Le MJ crée une note de session privé MJ" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-10 | [alternatif] Rendre une note publique, visible immédiatement | Une note de session existe avec `visibility = privé MJ` | Le MJ bascule sa visibilité vers visible par les joueurs | La note est immédiatement visible dans la vue joueur | US-06-04 §"Le MJ rend une note de session publique" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-11 | [erreur, résilience] Perte de connexion → draft local + synchro | Le MJ perd la connexion pendant la saisie d'une note | Il continue de saisir sa note | Le contenu est conservé en draft local ; la note est synchronisée automatiquement au retour de connexion | US-06-04 §"Résilience réseau (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-12 | [nominal] Épingler un document, source inchangé | Une session est LIVE, le document « Carte du donjon » existe | Le MJ épingle « Carte du donjon » | Le document apparaît dans le panneau Épinglés ; il reste inchangé dans la bibliothèque | US-06-05 §"Le MJ épingle un document" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-13 | [alternatif] Désépingler, reste en bibliothèque | « Carte du donjon » est épinglé dans la session | Le MJ le désépingle | Il disparaît du panneau Épinglés ; reste accessible dans son dossier d'origine | US-06-05 §"Le MJ désépingle un document" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-14 | [alternatif] Épinglage auto à la création à la volée | Une session est LIVE | Le MJ crée un document à la volée via UC-07 | Le document est automatiquement ajouté aux documents épinglés | US-06-05 §"Épinglage automatique à la création à la volée" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-15 | [irréversible] Terminer LIVE → CLOSED | Une session est en statut LIVE | Le MJ clique « Terminer » | La session passe en statut CLOSED | US-06-06 §"Le MJ termine une session LIVE" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-16 | [alternatif] Note rétroactive en CLOSED | Une session est en statut CLOSED | Le MJ crée une note de session | La note est enregistrée avec la session CLOSED | US-06-06 §"Le MJ ajoute une note rétroactive en CLOSED" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-17 | [erreur] Joueur ne peut pas créer de note en CLOSED | Une session est en statut CLOSED | Un joueur tente de créer une note de session | La création est refusée | US-06-06 §"Un joueur ne peut pas créer de note de session en CLOSED" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-18 | [irréversible] Archiver CLOSED → ARCHIVED | Une session est en statut CLOSED | Le MJ l'archive | La session passe en statut ARCHIVED ; aucune modification n'est possible | US-06-06 §"Le MJ archive une session CLOSED" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-19 | [nominal] Joueur accède, docs partagés + ses notes | Une session est LIVE, le joueur a un compte valide | Le joueur accède à la vue session | Il voit les documents partagés et ses notes de session personnelle joueur | US-06-07 §"Le joueur accède à la vue session" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-20 | [erreur] Joueur ne voit pas les notes privé MJ | Une session est LIVE, une note privé MJ existe | Le joueur consulte la vue session | La note privé MJ n'est pas visible | US-06-07 §"Le joueur ne voit pas les notes privé MJ" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-21 | [erreur] Accès refusé sans compte/invité en mode local | La session est en mode local | Un joueur tente d'accéder à la vue session | L'accès est refusé | US-06-07 §"Accès refusé sans compte ni accès invité" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-22 | [nominal, sécurité] Joueur crée une note personnelle non visible MJ | Une session est LIVE, le joueur est connecté | Il crée une note « Je soupçonne Ragnar » | La note est créée avec `visibility = personnelle joueur` ; non visible en vue MJ | US-06-08 §"Le joueur crée une note personnelle" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-23 | [erreur, sécurité] Note PLAYER_PRIVATE inaccessible au MJ | Une note personnelle joueur existe | Le MJ consulte les notes de session | La note personnelle joueur n'est pas affichée | US-06-08 §"La note personnelle joueur est inaccessible au MJ" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-24 | [alternatif] Invité avec personnage associé crée une note | Un joueur en accès invité est associé au personnage « Kira » | Il crée une note de session pendant LIVE | La note est créée pour « Kira » avec une visibilité personnelle | US-06-08 §"Joueur accès invité avec personnage associé" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-25 | [alternatif] Reprendre après interruption, reste LIVE | Une session était LIVE lors de l'interruption | Le MJ rouvre la campagne | La session est toujours en statut LIVE ; les notes précédentes sont présentes | US-06-09 §"Le MJ reprend une session après interruption" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-26 | [alternatif] Note rétroactive CLOSED (US-06-10) | Une session est en statut CLOSED | Le MJ crée une note « Ragnar a menti — confirmé » | La note est enregistrée avec la session CLOSED | US-06-10 §"Le MJ ajoute une note rétroactive" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-27 | [erreur] Création impossible en ARCHIVED | Une session est en statut ARCHIVED | Le MJ tente de créer une note de session | La création est refusée | US-06-10 §"Création impossible en ARCHIVED" | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-28 | [erreur, sécurité, dérivé] Accès non autorisé à la vue MJ | Un joueur tente d'accéder à la vue session du MJ | Il accède directement à l'URL/route de la vue MJ | L'accès est refusé ; le joueur est redirigé vers sa propre vue joueur | UC-06 fiche §Exceptions E2 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC06-29 | [sécurité, dérivé] PLAYER_PRIVATE : ni lecture ni révélation d'existence (anti-énumération) | Un document ou une note de session a `visibility = PLAYER_PRIVATE`, dont le MJ n'est pas l'auteur | Le MJ tente d'y accéder directement (lecture ou énumération par identifiant) | L'accès est refusé — `Document.CanBeReadBy()` gate l'objet entier, sans exception ; la tentative ne révèle ni le contenu ni l'existence du document (aucune métadonnée exposée par ce chemin) | RB-06-25/26 (dérivé, pas de Gherkin) + NFR-CONF-01 §Situation limite — tentative d'accès à un document non partagé | ☐ OK ☐ KO ☐ N/A |

**Trou traité (b) — non comblé** : UC-06 exception E3 (« MJ consulte une session CLOSED/ARCHIVED ») est déjà substantiellement couverte par CR-UC06-15/16/18/26/27 (transitions et notes rétroactives) — la nuance additionnelle d'affichage en lecture seule côté MJ pour une session CLOSED n'est pas spécifiable au-delà de ce qui est déjà tracé ; renvoyée au registre des points ouverts (§12) plutôt que dupliquée en case redondante.

---

### UC-07 — Créer à la volée (Must / Should)

**Environnement(s)** : session LIVE (US-07-01) et CLOSED (US-07-02) ; impossible en ARCHIVED ; mode local possible ; réservé au MJ.
**Sources US** : US-07-01, US-07-02.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC07-01 | [nominal] Créer un PNJ à la volée | Une session est en statut LIVE | Le MJ ouvre le panneau de création rapide, choisit PNJ, saisit « Baronne Elara » | Un document PNJ est créé avec `visibility = GM_ONLY` et propriétés vides ; ajouté aux documents épinglés ; placé dans le dossier Personnages | US-07-01 §"Le MJ crée un PNJ à la volée (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-02 | [alternatif] Créer une note de session (jamais épinglée) | Une session est en statut LIVE | Le MJ choisit NOTE, saisit « Connexion faction Corbeau » | Le document NOTE est créé avec `visibility = GM_ONLY` ; ajouté aux notes de session via `AttachNote` (jamais épinglé) ; placé dans le dossier Notes | US-07-01 §"Le MJ crée une note de session à la volée (A3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-03 | [alternatif] Créer un document générique, GM_ONLY + épinglé | Une session est en statut LIVE | Le MJ crée un document de type « Lieu » intitulé « Auberge du Pont Brisé » | Le document est créé, lié à la campagne, `visibility = GM_ONLY`, épinglé dans la session | US-07-01 §"Le MJ crée un document générique à la volée (A4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-04 | [erreur] Titre vide refusé | Une session est en statut LIVE | Le MJ valide la création sans titre | La création est refusée ; un message d'erreur est affiché | US-07-01 §"Création refusée si titre vide (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-05 | [erreur] Création impossible en ARCHIVED | Une session est en statut ARCHIVED | Le MJ tente d'ouvrir le panneau de création rapide | L'action est désactivée ou refusée | US-07-01 §"Création impossible en ARCHIVED (E2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-06 | [alternatif] PNJ rétroactif en CLOSED | Une session est en statut CLOSED | Le MJ crée un PNJ « Capitaine Draven » depuis le panneau de création rapide | Le document est créé dans la bibliothèque de contenu avec `visibility = GM_ONLY`, lié à la campagne | US-07-02 §"Le MJ ajoute un PNJ rétroactivement en CLOSED (A5)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-07 | [alternatif] Document créé en CLOSED persiste après archivage | Un document a été créé à la volée depuis une session CLOSED | Le MJ archive la session (CLOSED → ARCHIVED) | Le document est toujours accessible dans la bibliothèque de contenu | US-07-02 §"Le document créé en CLOSED est persisté après archivage" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-08 | [erreur] Création impossible en ARCHIVED (US-07-02) | Une session est en statut ARCHIVED | Le MJ tente d'ouvrir le panneau de création rapide | L'action est désactivée ou refusée | US-07-02 §"Création impossible en ARCHIVED" | ☐ OK ☐ KO ☐ N/A |
| CR-UC07-09 | [alternatif, dérivé] Personnage joueur (PJ) à la volée | Une session est en statut LIVE ou CLOSED | Le MJ crée un document de type PJ avec un nom seul | Le document PJ est créé, `visibility = GM_ONLY`, lié à la campagne, épinglé par défaut en LIVE (désépinglable) ; associable à un joueur ultérieurement | RB-07-01..04 (dérivé, pas de Gherkin spécifique PJ) | ☐ OK ☐ KO ☐ N/A |

---

### Cas transverses (invariants recoupant UC-01 à UC-07)

Les six invariants suivants sont déjà exercés par au moins un cas de recette ci-dessus ; cette sous-section les rassemble comme grille de vérification transverse et ajoute la formulation adversariale la plus stricte quand elle dépasse le cas fonctionnel déjà tracé.

| ID | Invariant | Renvoi | Verdict |
|---|---|---|---|
| CR-TRANS-01 | `PLAYER_PRIVATE` invisible au MJ, y compris `OWNER`/`GM` — ni lecture, ni énumération, ni métadonnées (RB-06-25/26) | CR-UC06-23 (lecture refusée) ; CR-UC06-29 (anti-énumération, non-révélation d'existence) | ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-02 | Mode local : aucune requête réseau sortante + aucun secret stocké (RB-01-01/15) | CR-UC01-03, CR-UC01-17 (absence de requête réseau) ; CR-UC01-20 (aucun secret stocké) | ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-03 | Migration : gate de reconnaissance + confirmation explicite obligatoire ; échec → données locales intactes + rapport de rejets (RB-01-09/11, RB-10-04) — **potentiellement irréversible si confirmé par erreur** | CR-UC01-10, CR-UC01-11 | ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-04 | Machine d'états session unidirectionnelle `LIVE → CLOSED → ARCHIVED` (RB-06-18/20/21) | CR-UC06-15, CR-UC06-18, CR-UC06-27 | ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-05 | Privé par défaut (RB-04-01/RB-07-03/RB-06-11) | CR-UC04-03, CR-UC06-09, CR-UC07-01 | ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-06 | `PERSONAL` jamais décompté du quota (RB-02-19) | CR-UC02-03, CR-UC02-09 | ☐ OK ☐ KO ☐ N/A |

### UC-08 — Partager une information aux joueurs (Must)

**Environnement(s)** : cloud ; session LIVE requise pour les cas de propagation temps réel.
**Sources US** : US-08-01, US-08-02, US-08-03.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC08-01 | [nominal] Partager un document GM_ONLY | Un document « Carte du donjon » a `visibility = GM_ONLY` | Le MJ clique sur « Partager » | `Document.visibility` passe à `PUBLIC` ; l'événement `DocumentVisibilityChanged` est produit ; les joueurs voient le document | US-08-01 §"Le MJ partage un document GM_ONLY (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-02 | [nominal] Partager un REVEAL depuis une scène | Un document REVEAL « Inscription sur la porte » a `visibility = GM_ONLY` | Le MJ clique sur « Partager » | `Document.visibility` passe à `PUBLIC` ; les joueurs voient l'inscription | US-08-01 §"Partage d'un document REVEAL depuis une scène" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-03 | [erreur] Document déjà PUBLIC (no-op) | Un document a `visibility = PUBLIC` | Le MJ clique sur « Partager » | Aucune modification appliquée ; l'interface indique que le document est déjà visible par les joueurs | US-08-01 §"Document déjà PUBLIC (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-04 | [sécurité, dérivé] PLAYER_PRIVATE non partageable | Un document a `visibility = PLAYER_PRIVATE` | Le partage est tenté sur ce document | L'action est refusée ; `PLAYER_PRIVATE` ne peut pas passer à `PUBLIC` via cette action | RB-08-01/02 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-05 | [sécurité, dérivé] Un joueur ne peut pas déclencher le partage | Un joueur consulte un document `GM_ONLY` | Le joueur tente l'action de partage | L'action est refusée — seul `OWNER`/`GM` peut partager | RB-08-01/02 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-06 | [alternatif] Retirer le partage PUBLIC → GM_ONLY | Un document « Carte du donjon » a `visibility = PUBLIC` | Le MJ clique sur « Retirer le partage » | `Document.visibility` passe à `GM_ONLY` ; `DocumentVisibilityChanged` produit ; les joueurs ne voient plus le document | US-08-02 §"Le MJ retire le partage d'un document PUBLIC (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-07 | [temps réel] Retrait en LIVE propagé en temps réel | Une session est LIVE ; un document « Carte du donjon » a `visibility = PUBLIC` | Le MJ retire le partage | Le document disparaît de la vue joueur en temps réel | US-08-02 §"Retrait du partage en session LIVE" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-08 | [erreur] Document déjà GM_ONLY (no-op) | Un document a `visibility = GM_ONLY` | Le MJ tente de retirer le partage | Aucune modification n'est appliquée | US-08-02 §"Document déjà GM_ONLY" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-09 | [alternatif, dérivé] Document épinglé dont on retire le partage | Un document `PUBLIC` est épinglé dans une session LIVE | Le MJ retire le partage de ce document | Le document reste dans les documents épinglés ; il est visible pour le MJ seul | RB-08-08b (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-10 | [nominal] Partager un REVEAL depuis la vue session | Une session est LIVE ; un document REVEAL « Indice : inscription sur la porte » a `visibility = GM_ONLY` | Le MJ clique sur « Partager » depuis la vue session | `Document.visibility` passe à `PUBLIC` ; le document est ajouté aux documents épinglés de la session ; les joueurs voient l'indice | US-08-03 §"Le MJ partage un REVEAL depuis la vue session (A3, A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-11 | [nominal] Partager une note improvisée en session | Une session est LIVE ; une note « Connexion faction Corbeau » a `visibility = GM_ONLY` | Le MJ la partage depuis la vue session | Le document est `PUBLIC` et épinglé dans la session | US-08-03 §"Partage d'une note improvisée en session (A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC08-12 | [erreur] Partage depuis une session non LIVE | Une session est en statut CLOSED | Le MJ partage un document depuis cette vue de session | `Document.visibility` passe à `PUBLIC` ; le document n'est pas auto-épinglé (session non LIVE) | US-08-03 §"Partage depuis une session non LIVE" | ☐ OK ☐ KO ☐ N/A |

---

### UC-09 — Accéder à une session en tant que joueur (Must / Should)

**Environnement(s)** : cloud ; lien ponctuel (`GuestAccess`) ou permanent (`Member`) ; session LIVE ; comptes FREE/PRO.
**Sources US** : US-09-01, US-09-02, US-09-03, US-09-04.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC09-01 | [nominal] Rejoindre via lien ponctuel sans compte | Un MJ a généré un lien de session ponctuel valide | Le joueur clique sur le lien et saisit un nom d'affichage | Le `GuestAccess` est activé ; il voit les documents PUBLIC et les documents épinglés de la session ; son nom est visible par le MJ | US-09-01 §"Joueur rejoint via lien ponctuel sans compte (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-02 | [alternatif] Déjà connecté, saisie du nom sautée | Un MJ a généré un lien de session ponctuel ; le joueur est déjà connecté à son compte Haversack | Le joueur clique sur le lien | Il accède directement à la session avec son historique ; l'étape de saisie du nom est sautée | US-09-01 §"Joueur déjà connecté à son compte (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-03 | [erreur] Accès expiré après 24h de grâce | Une session est CLOSED depuis plus de 24 heures | Le joueur tente de rouvrir le lien ponctuel | Le `GuestAccess` est invalide ; le joueur voit le message « lien non actif » | US-09-01 §"Accès expire après 24h de grâce" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-04 | [erreur, dérivé] FREE : 5e joueur distinct refusé | Un compte FREE a déjà 4 joueurs distincts disposant d'un accès à une session | Un 5e joueur distinct tente d'obtenir un accès | L'accès est refusé avec un message sobre (sans révélation sur la campagne) ; le MJ reçoit un signalement avec invitation à passer PRO | RB-09-21 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-05 | [RGPD, dérivé] Information RGPD à la saisie du nom | Un joueur s'apprête à saisir son nom d'affichage | Il arrive sur l'écran de saisie | Il est informé simplement de ce qui est conservé (nom, notes privées), pour combien de temps, et du sort de ses données à la fin de l'accès | RB-09-20 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-06 | [erreur] Lien expiré | Une session est terminée depuis plus de 24 heures | Le joueur clique sur l'ancien lien de session | Il voit le message « Ce lien n'est plus actif », invitant à contacter le MJ ; aucune information sur la campagne n'est révélée | US-09-02 §"Lien de session expiré (A4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-07 | [erreur] Lien révoqué | Le MJ a révoqué le `GuestAccess` d'une session | Le joueur tente d'y accéder | Il voit le même message que pour un lien expiré | US-09-02 §"Lien révoqué par le MJ (A4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-08 | [erreur] Lien invalide ou malformé | Un lien est incorrect ou malicieux | Un joueur l'ouvre | Il voit une page d'erreur sobre ; aucune information sur l'existence de la campagne n'est révélée — message indistinguable des deux cas précédents | US-09-02 §"Lien invalide ou mal formé (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-09 | [nominal, Should] Compte rejoint via lien permanent | Un MJ a généré un lien permanent ; le joueur a déjà un compte Haversack | Le joueur clique sur le lien permanent | Il est lié comme `Member` de la campagne ; il accède à l'historique des sessions et aux documents de lore PUBLIC | US-09-03 §"Joueur avec compte rejoint via lien permanent (A3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-10 | [alternatif, Should] Sans compte → redirigé UC-10 puis Member | Un joueur clique sur un lien permanent de campagne sans avoir de compte | La page s'affiche | Il est invité à créer un compte via UC-10 ; après création, il est automatiquement lié comme `Member` | US-09-03 §"Joueur sans compte redirigé vers UC-10 (A3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-11 | [erreur, Should] MJ révoque l'accès permanent | Un `Member` a accès à la campagne via lien permanent | Le MJ révoque l'accès de ce membre | Le lien devient invalide pour ce membre ; il ne peut plus accéder à la campagne | US-09-03 §"MJ révoque l'accès permanent d'un membre" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-12 | [alternatif, Should] Invité crée un compte et migre | Un joueur invité a accès à plusieurs sessions et a pris des notes personnelles | Il clique sur « Créer un compte » depuis la vue invitée et complète la création via UC-10 | Ses notes personnelles sont migrées ; son historique de `GuestAccess` est rattaché au compte ; aucune note n'est perdue | US-09-04 §"Joueur invité crée un compte et migre ses données (A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-13 | [nominal, Should] MJ promeut le joueur migré en Member | Un joueur invité vient de créer un compte | Le MJ voit la notification dans le panneau membres | Il peut valider la promotion du joueur en `Member` — ce gate est indépendant de l'état `emailVerified` du compte | US-09-04 §"MJ peut promouvoir le joueur migré en Member" | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-14 | [sécurité/temps réel] Invité connecté en LIVE, GuestAccess révoqué en cours → déconnexion forcée + aucun push ultérieur | Un invité est connecté à une session LIVE via un `GuestAccess` actif | Le MJ révoque le `GuestAccess` de cet invité pendant que sa connexion est active, puis un document est partagé | La connexion du hub de cet invité est fermée de force immédiatement (déconnexion forcée) ; aucun événement poussé après la révocation n'est reçu par cet invité — cas de référence B8.2, distinct du retrait de partage (CR-UC08-07) et de la re-tentative d'accès (CR-UC09-07) | RB-09-06 (dérivé, pas de Gherkin) + roadmap-entree-build.md §3.6 (« Déconnexion forcée du hub sur GuestAccessRevoked/GuestAccessExpired ») + roadmap Annexe A B8.2 | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-15 | [sécurité, dérivé] Token GuestAccess hors query-string et hors log | Un `GuestAccess` est actif et utilisé pour une négociation temps réel | Revue/scan des requêtes et journaux applicatifs de la session | Le token `GuestAccess` n'apparaît dans aucune URL ni aucun journal de requête | roadmap-entree-build.md §3.6, ADR-004 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-16 | [RGPD, dérivé] Suppression physique des notes PLAYER_PRIVATE à la fin d'un GuestAccess non converti | Un `GuestAccess` non converti en compte arrive à sa fin définitive (expiration après grâce ou révocation sans réactivation) | La fin définitive de l'accès est atteinte | Les notes `PLAYER_PRIVATE` créées par cet invité sont supprimées physiquement ; seules les notes de cet invité sont concernées, jamais celles d'autres participants — distinct de l'information donnée à la saisie du nom (CR-UC09-05) | RB-09-19 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC09-17 | [RGPD, dérivé] Cessation d'usage et effacement du displayName sous 90 jours | Un `GuestAccess` arrive à sa fin définitive | — | Le `displayName` cesse immédiatement d'être utilisé/affiché ; son effacement effectif intervient au plus tard 90 jours après la fin d'accès | RB-09-18 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-10 — Créer un compte et synchroniser dans le cloud (Must — zone authentification sensible)

**Environnement(s)** : cloud ; local avec/sans données ; email + mot de passe ou connexion fédérée (Google/Discord) ; `emailVerified` vrai/faux ; statuts `ACTIVE`/`SUSPENDED`/`DELETED`.
**Sources US** : US-10-01 à US-10-06.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC10-01 | [nominal] Inscription depuis le local avec données | Un MJ utilise l'application en mode local avec des campagnes, documents et sessions locales | Il clique sur l'invite de sauvegarde cloud, saisit email/nom/mot de passe, soumet | Le compte est créé immédiatement ; le gate de reconnaissance présente les campagnes détectées et leur historique de session ; après confirmation explicite, tout est migré ; l'espace de travail est retrouvé intact et synchronisé | US-10-01 §"Inscription depuis le mode local avec données locales (nominal 1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-02 | [nominal] Inscription sans données locales | Un utilisateur accède directement à la page d'inscription, sans données locales | Il saisit email/nom/mot de passe et soumet | Le compte est créé immédiatement ; il est redirigé vers l'écran de création de campagne | US-10-01 §"Inscription sans données locales (nominal 2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-03 | [erreur] Email déjà utilisé | Un compte existe avec l'email « emilie@exemple.fr » | Un utilisateur tente de s'inscrire avec ce même email | Le système refuse la création ; un message indique que l'adresse est déjà associée à un compte | US-10-01 §"Email déjà utilisé lors de l'inscription (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-04 | [alternatif] Invité crée un compte (notes PLAYER_PRIVATE migrées) | Un joueur invité a un `GuestAccess` et a pris des notes personnelles | Il clique sur « Créer un compte » depuis la vue invitée et complète le formulaire | Son `GuestAccess` est rattaché au nouveau compte ; ses notes `PLAYER_PRIVATE` sont migrées sans perte | US-10-01 §"Joueur invité crée un compte depuis un lien d'invitation (A3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-05 | [nominal] Connexion avec identifiants valides | Un utilisateur a un compte actif | Il saisit email et mot de passe corrects, soumet | Il est authentifié et redirigé vers son tableau de bord | US-10-02 §"Connexion avec identifiants valides (nominal 3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-06 | [erreur, sécurité] Identifiants invalides, message générique | Un utilisateur tente de se connecter | Il saisit un mot de passe incorrect | Le système affiche un message d'erreur générique, sans préciser si l'email ou le mot de passe est incorrect | US-10-02 §"Identifiants invalides (E2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-07 | [erreur] Compte suspendu refusé | Un compte a le statut SUSPENDED | L'utilisateur tente de se connecter | La connexion est refusée ; un message indique que le compte est suspendu | US-10-02 §"Connexion avec un compte suspendu" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-08 | [nominal] 1re connexion fédérée sans données | Un utilisateur n'a pas de compte Haversack ni de données locales | Il se connecte via un fournisseur d'identité externe | Un compte `User` est créé avec son email ; il est redirigé vers l'écran de création de campagne | US-10-03 §"Première connexion fédérée sans données locales — création de compte (nominal 4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-09 | [nominal] Fédéré, email présent VÉRIFIÉ → liaison au compte existant | Un compte existe avec une adresse vérifiée | L'utilisateur se connecte via son fournisseur d'identité avec cette même adresse | La connexion fédérée est liée au compte existant ; aucun doublon n'est créé | US-10-03 §"Connexion fédérée avec adresse déjà présente — adresse vérifiée" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-10 | [limite, garanties fermes seulement] Fédéré, email présent NON vérifié (branche b) | Un compte existe avec une adresse NON vérifiée | L'utilisateur se connecte via son fournisseur d'identité avec cette même adresse | Garanties vérifiables : la liaison automatique est refusée (anti-hijacking CWE-287) ; aucun doublon n'est créé (invariant email unique) ; aucune indication ne révèle le compte préexistant (anti-énumération CWE-204). **L'issue exacte (Option A « reclaim-in-place » ou alternative) reste [À TRANCHER] — voir PO-03, ce cas ne préjuge pas de la résolution** | US-10-03 §"Connexion fédérée avec adresse déjà présente — adresse non vérifiée (branche b, RB-10-08 — résolution ouverte)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-11 | [nominal] Fédéré avec données locales existantes | Un utilisateur a des données locales | Il s'inscrit pour la première fois via son fournisseur d'identité | Le gate de reconnaissance présente les campagnes détectées ; après confirmation, les données sont migrées ; l'espace de travail est retrouvé intact | US-10-03 §"Connexion fédérée avec données locales existantes" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-12 | [nominal] Réinitialisation, message identique si email inconnu | Un utilisateur a oublié son mot de passe | Il clique sur « Mot de passe oublié » et saisit son email | Un email de réinitialisation est envoyé si le compte existe ; le message de confirmation est identique que l'email existe ou non | US-10-04 §"Réinitialisation du mot de passe (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-13 | [erreur] Lien de réinitialisation expiré (≤ 15 min) | Un utilisateur a reçu un lien de réinitialisation, et le lien a expiré | Il clique sur le lien | Le système l'informe que le lien n'est plus valide et propose d'en générer un nouveau | US-10-04 §"Lien de réinitialisation expiré (E3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-14 | [nominal] Réinitialisation réussie, ancien mdp invalidé | Un utilisateur utilise un lien de réinitialisation valide | Il saisit et confirme un nouveau mot de passe | Le mot de passe est mis à jour ; l'ancien est immédiatement invalide ; redirection vers la connexion | US-10-04 §"Réinitialisation réussie" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-15 | [nominal] Mise à jour du nom d'affichage | Un utilisateur est connecté à son compte | Il modifie son nom d'affichage et sauvegarde | Le nouveau nom est appliqué immédiatement et visible dans toutes ses campagnes | US-10-05 §"Mise à jour du nom d'affichage (A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-16 | [nominal] Modification du mot de passe (mdp actuel requis) | Un utilisateur est connecté à son compte | Il saisit son mot de passe actuel et un nouveau, sauvegarde | Le mot de passe est mis à jour ; confirmation visuelle reçue | US-10-05 §"Modification du mot de passe" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-17 | [irréversible] Suppression de compte | Un utilisateur est authentifié, son email est validé, aucune campagne à membres actifs | Il demande la suppression, prend connaissance des conséquences, confirme | Les notes de session personnelles sont supprimées physiquement ; les données nominatives sont anonymisées ; le compte passe au statut DELETED ; l'utilisateur est déconnecté | US-10-06 §"Suppression de compte (A4 nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-18 | [erreur] Bloquée si propriétaire de campagne à membres actifs | Un utilisateur est propriétaire d'une campagne avec des membres actifs | Il demande la suppression de son compte | Le système bloque la suppression et indique les campagnes concernées | US-10-06 §"Suppression bloquée — propriétaire de campagne active (E4)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-19 | [erreur] Bloquée si email non validé | Un utilisateur est authentifié mais son email n'est pas validé | Il tente de demander la suppression de son compte | Le système refuse la demande et indique que la validation de l'email est requise | US-10-06 §"Suppression bloquée — adresse de messagerie non validée" | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-20 | [sécurité, dérivé] SetInitialPassword fédéré refusé sans emailVerified + ré-authentification | Un compte est fédéré-only (aucun mot de passe existant) | L'utilisateur tente de définir un premier mot de passe complémentaire sans `emailVerified = true`, ou sans preuve de ré-authentification récente auprès du fournisseur d'identité | L'opération est refusée — `SetInitialPassword()` exige `emailVerified = true` ET une ré-authentification récente auprès du fournisseur d'identité fédéré (anti CWE-620) ; cette même exigence d'`emailVerified` s'applique à d'autres opérations sensibles (cf. suppression de compte, CR-UC10-19) | RB-10-10(b) (dérivé, pas de Gherkin), CWE-620 | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-21 | [sécurité, dérivé] Brute-force — rate limiting hybride bloque après N tentatives | Un utilisateur multiplie les tentatives de connexion incorrectes sur un même compte et/ou depuis une même IP | Il dépasse le seuil de tentatives autorisées | Le rate limiting hybride par-IP/par-compte bloque les tentatives suivantes (seuil N non chiffré — voir PO-12) | roadmap Annexe A B3.2 (dérivé, pas de Gherkin), ADR-015 (rate limiting hybride) | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-22 | [sécurité, dérivé] Replay — réutilisation d'un token consommé refusée | Un token access ou refresh a déjà été consommé ou roté | Le même token est rejoué | La réutilisation est refusée ; la rotation avec détection de réutilisation par famille invalide la famille entière de tokens concernée | roadmap Annexe A B3.2 (dérivé, pas de Gherkin), ADR-015 (rotation avec détection de réutilisation par famille, access ≤ 15 min / refresh ≤ 7 jours, §5) | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-23 | [sécurité, dérivé] Rotation de clé JWT sans invalider les sessions légitimes | Une rotation de clé de signature est déclenchée via `ITokenSigner.RotateKey(...)` | La clé est rotée | Les tokens signés avec l'ancienne clé, dans leur fenêtre de validité légitime, restent acceptés ; la rotation n'invalide pas les sessions en cours légitimement | roadmap Annexe A B3.2 (dérivé, pas de Gherkin), ADR-015 §Rotation de clé JWT | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-24 | [sécurité, dérivé] Réutilisation d'un lien de réinitialisation déjà consommé refusée | Un lien de réinitialisation a déjà été utilisé, qu'il ait abouti ou non | Ce même lien est réutilisé | La réutilisation est refusée — le lien est à usage unique | RB-10-12 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-25 | [sécurité, dérivé] Émission d'un nouveau lien invalide les liens précédents | Un utilisateur a déjà reçu un lien de réinitialisation actif | Il redemande un nouveau lien de réinitialisation | Tous les liens précédemment émis pour ce compte deviennent invalides | RB-10-12 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC10-26 | [erreur, sécurité, dérivé] Changement de mot de passe refusé sans le mot de passe actuel | Un utilisateur est connecté à son compte | Il tente de changer son mot de passe sans fournir le mot de passe actuel, ou en fournissant un mot de passe actuel incorrect | Le changement est refusé | RB-10-15 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-11 — Gérer les membres d'une campagne (Should)

**Environnement(s)** : cloud ; MJ propriétaire (`OWNER`) ; `GuestAccess` et `Member`.
**Sources US** : US-11-01, US-11-02, US-11-03, US-11-04.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC11-01 | [nominal] Générer un lien CAMPAIGN | Le MJ est propriétaire de la campagne | Il ouvre la section Membres, choisit le périmètre CAMPAIGN, génère le lien | Un lien d'invitation est généré ; le bouton « Copier » est disponible ; l'invitation apparaît avec le statut PENDING | US-11-01 §"MJ génère un lien d'invitation campagne (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-02 | [nominal] Générer un lien SESSION | Le MJ veut inviter un joueur pour une session donnée | Il choisit le périmètre SESSION et sélectionne la session | Le lien est associé à cette session ; un `GuestAccess` sera créé à l'utilisation du lien | US-11-01 §"MJ génère un lien d'invitation session" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-03 | [erreur] Joueur déjà membre, pas de doublon | Un joueur est déjà `Member` ACTIVE de la campagne | Le MJ tente de générer une nouvelle invitation pour ce joueur | Le système informe le MJ que ce joueur est déjà membre ; aucun doublon n'est créé | US-11-01 §"Joueur déjà membre — doublon détecté (E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-04 | [erreur] Invitation expirée, REVOKED non réactivable | Une invitation a dépassé sa date d'expiration | Le MJ consulte la liste des invitations | L'invitation est marquée REVOKED ; aucune option de réactivation n'est proposée ; le MJ peut créer un nouveau lien | US-11-01 §"Invitation expirée non réactivable (E2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-05 | [alternatif] Révoquer une invitation active | Le MJ a généré un lien dont le statut est PENDING | Il clique sur « Révoquer » | Le statut passe à REVOKED ; le token est invalidé immédiatement | US-11-02 §"MJ révoque une invitation active (A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-06 | [erreur] Joueur tente un lien révoqué, message générique | Une invitation est à l'état REVOKED | Un joueur clique sur le lien | Il voit le message « Ce lien n'est plus actif » ; aucune information sur la campagne n'est révélée | US-11-02 §"Joueur tente d'utiliser un lien révoqué" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-07 | [erreur] Déjà révoquée, pas de réactivation | Une invitation est à l'état REVOKED | Le MJ consulte la liste des invitations | Aucune option « Réactiver » n'est proposée ; une option « Nouveau lien » est disponible | US-11-02 §"Invitation déjà révoquée — pas de réactivation" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-08 | [alternatif] Retirer un membre, accès perdu immédiatement | La campagne a un `Member` ACTIVE nommé « Julien » | Le MJ clique sur « Retirer » pour Julien | Le statut de Julien passe à REMOVED ; il ne peut plus accéder à la campagne | US-11-03 §"MJ retire un membre de la campagne (A3)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-09 | [nominal] Données préservées après retrait | Julien avait un personnage et des notes dans la campagne | Le MJ retire Julien | Le personnage de Julien reste visible ; les notes partagées sont conservées | US-11-03 §"Données préservées après retrait" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-10 | [nominal] Membre retiré réinvitable | Julien est à l'état REMOVED | Le MJ génère un nouveau lien d'invitation pour Julien | Julien peut utiliser ce lien pour rejoindre à nouveau ; son ancien personnage peut lui être réassocié | US-11-03 §"Membre retiré peut être réinvité" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-11 | [erreur] Ancien lien refusé | Julien est à l'état REMOVED | Julien tente d'accéder à la campagne via un ancien lien | Il voit un message d'accès refusé | US-11-03 §"Ancien lien d'un membre retiré" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-12 | [nominal] Associer joueur ↔ personnage | Le MJ a un `Member` ACTIVE « Sophie » ; la campagne contient un personnage « Aelindra » | Le MJ associe Sophie à Aelindra | Sophie voit la fiche d'Aelindra et accède à ses notes `PLAYER_PRIVATE` dans sa vue joueur | US-11-04 §"MJ associe un joueur à un personnage (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-13 | [erreur] Personnage déjà associé, unicité | Aelindra est déjà associée à Sophie | Le MJ tente d'associer Aelindra à un autre membre | Le système indique qu'Aelindra est déjà associée à Sophie | US-11-04 §"Personnage déjà associé — unicité" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-14 | [confidentialité] Nouveau GuestAccess sur le même personnage, fiche seulement | Un personnage « Aelindra » a une fiche existante ; un nouveau lien GuestAccess pointant vers Aelindra est généré | Le joueur utilise ce lien sans compte | Il accède à la fiche d'Aelindra ; les notes `PLAYER_PRIVATE` d'un invité précédent ne lui sont jamais accessibles | US-11-04 §"GuestAccess récupère la fiche via lien personnage, pas les notes d'un invité précédent" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-15 | [nominal] Dissociation sans suppression | Sophie est associée à Aelindra | Le MJ dissocie Sophie d'Aelindra | Aelindra reste dans la campagne avec ses notes ; Sophie n'a plus accès à la fiche | US-11-04 §"Dissociation sans suppression" | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-16 | [sécurité/IDOR, dérivé] Non-OWNER ne peut pas révoquer une invitation | Un membre non-OWNER (ex. un joueur `PLAYER`) de la campagne | Il tente de révoquer une invitation de la campagne | L'action est refusée — seul le MJ propriétaire (`OWNER`) peut révoquer une invitation | RB-11-07 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-17 | [sécurité/IDOR, dérivé] Non-OWNER ne peut pas retirer un membre | Un membre non-OWNER | Il tente de retirer un autre membre de la campagne | L'action est refusée — seul `OWNER` peut retirer un membre | RB-11-11 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC11-18 | [sécurité/IDOR, dérivé] Non-OWNER ne peut pas associer un personnage | Un membre non-OWNER | Il tente d'associer un joueur à un personnage | L'action est refusée — seul `OWNER` peut associer ou dissocier un joueur et un personnage | RB-11-16 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-12 — Consulter sa campagne en tant que joueur, vue post-accès (Should — zone IDOR/isolation)

**Environnement(s)** : cloud ; `GuestAccess` ou `Member` ; périmètre SESSION ou CAMPAIGN ; 0/1/N personnages associés.
**Sources US** : US-12-01, US-12-02.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC12-01 | [nominal] Avec personnage : fiche + PUBLIC + notes liées éditables | Lucas a rejoint la session via un lien valide ; le MJ a associé le personnage « Aldric » à Lucas | La vue joueur s'affiche | Lucas voit la fiche d'Aldric, les documents PUBLIC, et peut consulter/modifier ses notes `PLAYER_PRIVATE` liées à Aldric | US-12-01 §"Joueur avec personnage associé accède à sa fiche et aux documents partagés (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-02 | [limite] Sans personnage : PUBLIC only, pas de note liée | Lucas a rejoint la session ; aucun personnage ne lui a été associé | La vue joueur s'affiche | Lucas voit les documents PUBLIC ; il ne voit pas de fiche de personnage ; il ne peut pas créer de note `PLAYER_PRIVATE` liée à un personnage | US-12-01 §"Joueur sans personnage associé — consultation uniquement" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-03 | [alternatif] Périmètre SESSION, pas d'historique/lore | Lucas a rejoint via un lien périmètre SESSION | La vue joueur s'affiche | Lucas voit les documents épinglés de la session et les PUBLIC ; il ne voit pas l'historique des sessions précédentes ni le lore complet | US-12-01 §"Accès périmètre SESSION — pas d'accès au lore campagne complet" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-04 | [confidentialité] Notes PLAYER_PRIVATE préservées pour le même auteur | Lucas a pris des notes `PLAYER_PRIVATE` sur Aldric lors d'une session précédente ; il rejoint une nouvelle session en tant que même auteur | La vue joueur s'affiche | Les notes écrites par Lucas sur Aldric sont disponibles | US-12-01 §"Notes PLAYER_PRIVATE préservées entre les sessions pour le même auteur" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-05 | [confidentialité] Documents GM_ONLY invisibles | La campagne contient un document `GM_ONLY` | Lucas consulte la vue joueur | Ce document n'apparaît pas dans sa liste de documents | US-12-01 §"Documents GM_ONLY invisibles pour le joueur" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-06 | [nominal] Choix du personnage actif à l'entrée (multi-perso) | Thomas est `Member` de la campagne ; le MJ l'a associé aux personnages « Veran » et « Kael » | Thomas accède à la vue joueur | Une interface de sélection lui propose de choisir entre « Veran » et « Kael » | US-12-02 §"Joueur avec plusieurs personnages — choix du personnage actif à l'entrée" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-07 | [nominal] Vue après sélection, notes de l'autre personnage non affichées | Thomas a sélectionné « Veran » comme personnage actif | La vue joueur s'affiche | La fiche de Veran est visible en priorité ; les notes `PLAYER_PRIVATE` de Veran sont accessibles ; celles de Kael ne sont pas affichées | US-12-02 §"Vue joueur après sélection du personnage actif" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-08 | [alternatif] Changement de personnage actif en cours de session | Thomas a « Veran » comme personnage actif | Il change le personnage actif pour « Kael » | La fiche de Kael s'affiche ; les notes `PLAYER_PRIVATE` de Kael remplacent celles de Veran dans la vue | US-12-02 §"Changement de personnage actif en cours de session" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-09 | [limite] Un seul personnage, pas de sélection | Lucas est `Member` ; le MJ l'a associé à un seul personnage « Aldric » | Lucas accède à la vue joueur | La fiche d'Aldric s'affiche directement, sans étape de sélection | US-12-02 §"Joueur avec un seul personnage — pas de sélection requise" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-10 | [nominal] Changement de focus ne modifie pas les associations MJ | Thomas a « Veran » comme personnage actif et change pour « Kael » | Le MJ consulte le panneau membres | L'association de Thomas avec Veran et Kael est inchangée dans la gestion de l'espace — le personnage actif est un focus de présentation, jamais un droit d'accès | US-12-02 §"Changement de personnage actif sans modifier les associations MJ" | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-11 | [sécurité/IDOR, dérivé] IDOR cross-tenant refusé | Un joueur/membre de l'espace A n'est membre d'aucune façon de l'espace B | Il tente d'accéder par identifiant direct à une ressource (document, session, personnage) de l'espace B | L'accès est refusé, sans révélation d'existence de la ressource ni de l'espace B | roadmap Annexe A B5.2, « Test IDOR — cas de référence » (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |
| CR-UC12-12 | [confidentialité, dérivé] Réassociation par Member : aucune note héritée du prédécesseur | Un personnage a été précédemment joué par un `Member` A dissocié ; le personnage est réassocié à un `Member` B | Member B accède à la fiche du personnage | Member B n'hérite jamais des notes `PLAYER_PRIVATE` créées par Member A — les notes restent liées à leur auteur, jamais au personnage ; complète le vecteur invité déjà couvert (CR-UC11-14) | RB-12-02 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-13 — Utiliser un scénario réutilisable (post-MVP — hors première livraison)

> **Recette différée** : ce use case est **Should Have, hors première livraison**. UC-13 conditionne le parcours express one-shot d'UC-02 §A1 (US-02-02/US-02-04), lui-même reporté tant que UC-13 n'est pas livré. Les cas suivants sont produits pour tracer la matière disponible ; ils ne sont pas exigibles à la livraison MVP.

**Environnement(s)** : cloud ou local ; espace `PERSONAL`.
**Sources US** : US-13-01, US-13-02, US-13-03.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC13-01 | [nominal, post-MVP] Marquer un scénario réutilisable | Sonia a un scénario « La Crypte de Malnoir » dans sa campagne, pas encore dans son espace personnel | Elle choisit « Marquer comme réutilisable » | Le scénario apparaît dans l'espace personnel de Sonia (`isReusable = true`) ; le scénario source dans la campagne reste intact | US-13-01 §"Marquer un scénario comme réutilisable depuis une campagne — nominal" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-02 | [erreur, post-MVP] Promotion impossible si déjà présent | « La Crypte de Malnoir » est déjà dans l'espace personnel de Sonia | Elle tente de la marquer réutilisable une deuxième fois | L'application indique que le scénario est déjà dans la bibliothèque ; aucune duplication n'est créée | US-13-01 §"Promotion impossible si le scénario est déjà dans l'espace personnel" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-03 | [confidentialité, post-MVP] Espace PERSONAL accessible au propriétaire seul | Sonia a un espace personnel avec 15 scénarios | Antoine accède au tableau de bord de son propre compte | Antoine ne voit pas les scénarios de Sonia dans son espace personnel | US-13-01 §"Espace personnel accessible uniquement par le MJ propriétaire" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-04 | [nominal, post-MVP] Rejouer en one-shot | Sonia a le scénario dans son espace personnel | Elle choisit « Rejouer » puis « One-shot ce soir », saisit un nom | Une campagne one-shot est créée ; une instance du scénario est créée via `Document.Instantiate` ; le scénario source reste intact | US-13-02 §"Rejouer un scénario en one-shot ce soir — nominal" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-05 | [nominal, post-MVP] Rejouer dans une campagne existante | Antoine a un scénario dans son espace personnel et une campagne en cours | Il choisit « Rejouer » puis « Ajouter à une campagne existante » | Une instance est créée dans la campagne cible ; le scénario source reste intact | US-13-02 §"Rejouer un scénario dans une campagne existante" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-06 | [nominal, post-MVP] Modifier l'instance sans impact sur la source | Sonia a créé une instance de son scénario | Elle renomme un PNJ dans l'instance | Le PNJ change de nom dans l'instance ; il garde son nom d'origine dans le scénario source | US-13-02 §"Modification de l'instance sans impact sur le source" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-07 | [nominal, post-MVP] Archivage manuel après session | Sonia a joué une session dans la campagne one-shot | La session se termine | La campagne reste à l'état CLOSED ; Sonia doit l'archiver manuellement si elle le souhaite | US-13-02 §"Archivage manuel de la campagne one-shot après la session" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-08 | [nominal, post-MVP] Consulter l'historique des runs | Sonia a joué un scénario trois fois | Elle ouvre la fiche du scénario dans son espace personnel | Elle voit trois entrées d'historique, chacune avec date, contexte et notes MJ | US-13-03 §"Consulter l'historique des runs d'un scénario — nominal" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-09 | [alternatif, post-MVP] Instance passée en lecture seule | Sonia a un historique avec un run donné | Elle clique sur l'entrée correspondante | Elle consulte le contenu de l'instance en lecture seule ; aucune modification n'est possible depuis cette vue | US-13-03 §"Consulter une instance passée en lecture seule" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |
| CR-UC13-10 | [limite, post-MVP] Historique vide | Antoine a ajouté un scénario sans instance créée | Il consulte l'historique | L'historique est vide ; un message invite à lancer le premier run | US-13-03 §"Historique vide pour un scénario jamais joué" — post-MVP, recette différée | ☐ OK ☐ KO ☐ N/A |

---

### UC-14 — Rechercher rapidement une information (Should)

**Environnement(s)** : contexte d'espace + vue session ; visibilité MJ (GM_ONLY + PUBLIC) vs joueur (PUBLIC seul).
**Sources US** : US-14-01, US-14-02, US-14-03.

**Note MVP** : la recherche full-text sur le contenu des blocs et les types personnalisés sont post-MVP ; au MVP, la recherche porte sur le titre uniquement, sur les 8 types built-in.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC14-01 | [nominal] Recherche par titre avec résultat | L'espace actif contient un document « Seigneur Varek » (PNJ, `GM_ONLY`) | Le MJ saisit « Varek » dans la barre de recherche | Le document apparaît dans les résultats et peut être ouvert | US-14-01 §"Recherche par titre avec résultat (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-02 | [nominal] Recherche partielle insensible à la casse | L'espace actif contient « Note de session 3 » (LIVE_NOTE) | Le MJ saisit « note de session » | Le document apparaît dans les résultats | US-14-01 §"Recherche partielle insensible à la casse" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-03 | [limite] Aucun résultat, état vide | Aucun document ne contient « Dragon rouge » dans son titre | Le MJ saisit « Dragon rouge » | Un état vide est affiché avec une suggestion de modifier la recherche | US-14-01 §"Aucun résultat (A1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-04 | [sécurité/confidentialité] Recherche joueur, GM_ONLY exclus | L'espace contient « Plan secret » (`GM_ONLY`) et « Carte publique » (`PUBLIC`) | Le joueur effectue une recherche sur « plan » | « Plan secret » n'est pas dans les résultats ; « Carte publique » n'est pas affectée par ce filtre | US-14-01 §"Recherche joueur — documents GM_ONLY exclus des résultats (A4, E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-05 | [nominal] LIVE_NOTE de session passée recherchable | Une session clôturée contient une LIVE_NOTE « Révélation faction Corbeau » | Le MJ saisit « Corbeau » | La note apparaît dans les résultats | US-14-01 §"LIVE_NOTE session passée recherchable" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-06 | [nominal] Filtrer par type PNJ | La recherche sur « Varek » retourne un PNJ et une scène | Le MJ sélectionne le filtre de type « PNJ » | Seul le PNJ apparaît dans les résultats | US-14-02 §"Filtrer les résultats par type PNJ (A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-07 | [limite] Aucun résultat après filtre, état vide | La recherche retourne des résultats ; le MJ applique un filtre de type sans correspondance | — | Un état vide est affiché avec une option pour retirer le filtre | US-14-02 §"Aucun résultat après filtre (A1 + A2)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-08 | [nominal] Retirer le filtre, tous types réapparaissent | Le filtre de type « PNJ » est appliqué | Le MJ retire ce filtre | Tous les types de documents réapparaissent dans les résultats | US-14-02 §"Retirer le filtre de type" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-09 | [nominal] Recherche depuis la vue session, pondération session active | Une session est LIVE ; « Seigneur Varek » est épinglé ; « Varek le marchand » ne l'est pas | Le MJ saisit « Varek » depuis la vue session | « Seigneur Varek » apparaît en tête des résultats ; « Varek le marchand » apparaît après | US-14-03 §"Recherche depuis la vue session avec pondération (nominal)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-10 | [nominal] Ouvrir un document dans le panneau latéral | Une session est LIVE ; le MJ a cherché « Corbeau » | Il sélectionne « Faction des Corbeaux » | Le document s'ouvre dans un panneau latéral ; la vue session reste active | US-14-03 §"Ouverture d'un document dans le panneau latéral" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-11 | [nominal] Fermer le panneau sans perte de contexte | Le panneau latéral de recherche est ouvert | Le MJ le ferme | La vue session est restaurée telle quelle, sans perte de contexte | US-14-03 §"Fermeture du panneau latéral sans perte de contexte" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-12 | [sécurité] Recherche joueur depuis la vue session, note privée MJ absente | Une session est LIVE ; un document « Note privée MJ » a `GM_ONLY` | Le joueur effectue une recherche depuis la vue session | « Note privée MJ » n'apparaît pas dans ses résultats | US-14-03 §"Recherche joueur depuis la vue session (A4, E1)" | ☐ OK ☐ KO ☐ N/A |
| CR-UC14-13 | [erreur, dérivé] Erreur d'indexation | La recherche échoue techniquement | Le MJ ou le joueur effectue une recherche | Le système affiche un message d'erreur et propose une navigation manuelle | UC-14 fiche §Exceptions E2 (dérivé, pas de Gherkin) | ☐ OK ☐ KO ☐ N/A |

---

### UC-15 — Gel de campagnes au downgrade de tier (post-MVP — aucune US ni Gherkin)

> **Tous les cas de cette section sont marqués « dérivé de RB-15-xx (pas de Gherkin) — post-MVP »**. UC-15 est l'amont source-de-vérité de ce comportement (le domaine `space-management.md` en est l'aval documenté) ; aucune user story ni scénario Gherkin ne porte ce use case.

**Environnement(s)** : cloud ; déclencheur = événement de facturation `AccountTierChanged` ; jamais d'action MJ directe.

| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |
|---|---|---|---|---|---|---|
| CR-UC15-01 | [nominal, dérivé, post-MVP] Downgrade → gel des espaces excédentaires | Un MJ downgrade vers un tier gratuit avec plus d'espaces `CAMPAIGN`/`ONE_SHOT` actifs que le quota | L'événement `AccountTierChanged` est reçu | Les espaces excédentaires sont gelés dans l'ordre de création, les plus récents en premier, jusqu'au retour dans le quota | RB-15-01 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |
| CR-UC15-02 | [limite, dérivé, post-MVP] PERSONAL jamais compté ni gelé | Un MJ downgrade | L'événement `AccountTierChanged` est traité | L'espace `PERSONAL` n'est jamais compté dans le quota et n'est jamais gelé | RB-15-02 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |
| CR-UC15-03 | [nominal, dérivé, post-MVP] Espace FROZEN = lecture seule | Un espace est passé à l'état FROZEN | Le MJ consulte ou tente de modifier cet espace | Le contenu reste consultable ; aucune écriture n'est possible ; aucune donnée n'est supprimée du fait du gel | RB-15-03 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |
| CR-UC15-04 | [alternatif, dérivé, post-MVP] Montée de tier → dégel automatique dans la limite du quota cible | Un MJ possède des espaces FROZEN et monte de tier | L'événement `AccountTierChanged` est reçu | Les espaces FROZEN sont dégelés automatiquement, dans la limite du quota du tier cible — **l'ordre de dégel pour un tier intermédiaire borné reste un point ouvert (PO-07)** | RB-15-04 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |
| CR-UC15-05 | [nominal, dérivé, post-MVP] Cycle gel/dégel réversible sans perte de données | Un espace a été gelé puis dégelé | — | Le contenu et la structure de l'espace dégelé sont identiques à ce qu'ils étaient avant le gel | RB-15-05 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |
| CR-UC15-06 | [nominal, dérivé, post-MVP] Propriétaire notifié à chaque gel et dégel | Un espace est gelé ou dégelé | — | Le propriétaire reçoit une notification à chaque événement `SpaceFrozen` et `SpaceUnfrozen` | RB-15-06 (dérivé, pas de Gherkin) — post-MVP | ☐ OK ☐ KO ☐ N/A |

---

### Cas transverses (lot UC-08 à UC-15, priorité sécurité/confidentialité)

Suite de la sous-section « Cas transverses » ci-dessus (CR-TRANS-01 à 06). Cas dédiés cross-surface pour le lot UC-08..15 :

| ID | Invariant | Renvoi / Verdict |
|---|---|---|
| CR-TRANS-07 | Isolation de visibilité cross-surface : un joueur ne voit jamais `GM_ONLY` ni `PLAYER_PRIVATE` d'autrui — vérifiée sur espace, vue session, recherche bibliothèque ET recherche session, indépendamment de l'épinglage | CR-UC04-21 (anti-énumération document), CR-UC06-08 (vue session), CR-UC12-05 (espace/vue joueur), CR-UC14-04/12 (recherche bibliothèque/session) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-08 | Isolation des notes PLAYER_PRIVATE par AUTEUR, jamais par personnage : réassociation d'un personnage à un nouvel invité ou à un nouveau membre → aucune note héritée | CR-UC11-14 (vecteur invité), CR-UC12-12 (vecteur membre) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-09 | Anti-fuite temps réel : retrait de partage en LIVE propagé à la vue joueur ; révocation d'un GuestAccess en session déconnecte de force et bloque tout push ultérieur ; l'épinglage n'ouvre pas la visibilité | CR-UC09-14 (cas de référence B8.2 — révocation GuestAccess en session), CR-UC08-07 (retrait de partage propagé), CR-UC08-09 (épinglage n'ouvre pas la visibilité) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-10 | Opacité / anti-énumération : lien invalide/expiré/révoqué + login échoué + réinitialisation produisent des réponses indistinguables | CR-UC09-06/07/08, CR-UC10-06, CR-UC10-12 — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-11 | Anti-hijacking fédéré (CWE-287) + anti-énumération (CWE-204) + unicité email : branche non vérifiée → ni liaison auto, ni doublon, ni révélation (issue au registre PO-03) | CR-UC10-10 — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-12 | Token de réinitialisation : usage unique + expiration ≤ 15 min + invalidation des liens précédents + ancien mdp invalidé | CR-UC10-13 (expiration ≤ 15 min), CR-UC10-24 (usage unique), CR-UC10-25 (invalidation des liens précédents), CR-UC10-14 (ancien mdp invalidé) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-13 | Gate `emailVerified` sur opérations sensibles (+ mdp actuel pour changement de mdp, + ré-authentification récente pour 1er mdp fédéré, CWE-620) | CR-UC10-19 (emailVerified — suppression de compte), CR-UC10-20 (emailVerified + ré-authentification — 1er mdp fédéré), CR-UC10-16 (mdp actuel — changement de mdp) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-14 | IDOR / contrôle OWNER-GM : aucune action MJ (partage, invitation, révocation, retrait, association) déclenchable par un acteur non autorisé → B5.2 | CR-UC08-05 (partage), CR-UC11-16 (révocation invitation), CR-UC11-17 (retrait membre), CR-UC11-18 (association personnage), CR-UC12-11 (IDOR cross-tenant) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-15 | Invalidation d'accès immédiate : ancien lien refusé après révocation/retrait | CR-UC09-07, CR-UC11-06, CR-UC11-11 — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-16 | Gate anti-appropriation à la migration (poste partagé) : migration sans confirmation explicite rejetée | CR-UC01-10, CR-UC10-01 — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-17 | RGPD — effacement/rétention bornée : `displayName` invité ≤ 90 jours ; notes PLAYER_PRIVATE supprimées sans délai ; séquence d'effacement compte (suppression physique des notes incarnées) | CR-UC09-16 (suppression physique notes invité), CR-UC09-17 (effacement displayName), CR-UC10-17 (séquence d'effacement compte) — ☐ OK ☐ KO ☐ N/A |
| CR-TRANS-18 | Isolation multi-tenant de l'espace PERSONAL (post-MVP) | CR-UC13-03 — ☐ OK ☐ KO ☐ N/A |

---

## 11. Matrice de traçabilité (les 15 use cases)

| UC | Intitulé | Statut MVP | US couvertes | Plage de cas de recette | Nb cas | NFR | Tests nommés | Niveaux |
|---|---|---|---|---|---|---|---|---|
| UC-01 | Mode local sans compte | Must / Should | US-01-01, 02, 04, 05, 06, 07, 09 | CR-UC01-01..20 | 20 | OFF-01/02/03, CONF-02/04 | P6, P7 | U, I, E2E, SEC |
| UC-02 | Créer un espace de jeu | Must / Should (post-MVP : US-02-02/04) | US-02-00, 01, 03 | CR-UC02-01..13 | 13 | I18N transverses | — | U, I |
| UC-03 | Structurer un scénario | Must / Should | US-03-01 à 07 | CR-UC03-01..21 | 21 | I18N-03 | — | U, I |
| UC-04 | Gérer les documents d'un espace | Must / Should / Could | US-04-01 à 06 | CR-UC04-01..21 | 21 | CONF-01, I18N-03, ACC-* | — | U, I, SEC |
| UC-05 | Organiser par dossiers | Must / Should | US-05-01 à 05 | CR-UC05-01..12 | 12 | ACC-*, PERF-01 | — | U, I |
| UC-06 | Vue session | Must / Should | US-06-01 à 10 | CR-UC06-01..29 | 29 | PERF-01/02/03, OFF-04, CONF-01, ACC-01/02/03/04 | B1.7 | U, I, E2E, SEC, ACC, PERF |
| UC-07 | Créer à la volée | Must / Should | US-07-01, 02 | CR-UC07-01..09 | 9 | PERF-03 | — | U, I |
| UC-08 | Partager une information | Must | US-08-01, 02, 03 | CR-UC08-01..12 | 12 | PERF-03, CONF-01 | B8.2 | U, I, SEC |
| UC-09 | Accéder à une session en tant que joueur | Must / Should | US-09-01 à 04 | CR-UC09-01..17 | 17 | CONF-01/03, PERF-02, ACC-01/02 | B1.6, B1.7, B8.2 | U, I, E2E, SEC |
| UC-10 | Compte cloud + migration | Must | US-10-01 à 06 | CR-UC10-01..26 | 26 | CONF-02/03, OFF-04 | B1.5, B1.10, B3.2, P7, sagas SpaceDeleted/UserAnonymized | U, I, E2E, SEC |
| UC-11 | Gérer les membres | Should | US-11-01 à 04 | CR-UC11-01..18 | 18 | CONF-03 | B1.6 | U, I, SEC |
| UC-12 | Vue joueur post-accès | Should | US-12-01, 02 | CR-UC12-01..12 | 12 | CONF-01, ACC-*, PERF-* | B5.2 | U, I, SEC |
| UC-13 | Scénario réutilisable | Should — hors première livraison (post-MVP) | US-13-01, 02, 03 | CR-UC13-01..10 | 10 | I18N-03 | — | U, I |
| UC-14 | Rechercher | Should | US-14-01, 02, 03 | CR-UC14-01..13 | 13 | PERF-04, CONF-01, I18N-03, ACC-* | B5.1 | U, I, SEC, PERF |
| UC-15 | Gel des campagnes au downgrade | Post-MVP spécifié (Should, hors première livraison) | Aucune US — dérivé RB-15-01..06 | CR-UC15-01..06 | 6 | — | — | U, I |

**Preuve de couverture** : chaque UC du périmètre MVP (UC-01 à UC-12, UC-14) porte au moins un cas de recette traçable vers sa source US/Gherkin ou, pour les trous de nature (a), vers une règle métier explicitement marquée. UC-13 et UC-15 sont hors périmètre de première livraison — leur recette est produite ici par anticipation (différée pour UC-13, intégralement dérivée pour UC-15 en l'absence d'US) mais n'est pas exigible à la livraison MVP.

**Rattachement hors matrice** : le test d'architecture CI (**B3.2**, frontières des 4 bounded contexts) est adossé au jalon « Socle » — il n'apparaît dans aucune ligne de la matrice ci-dessus car il n'est rattachable à aucun UC particulier (§3.4).

---

## 12. Registre des points ouverts

| ID | Point | Nature | Source | Propriétaire de décision | Impact |
|---|---|---|---|---|---|
| PO-01 | Seuils de réactivité perçue non chiffrés (NFR-PERF-01→04) | Seuil chiffré non dérivable | `NFR-PERF-01→04` (aucune valeur de décision fixée) | Opérateur / instrumentation produit | La recette perf reste une observation qualitative (§3.7) tant que ce point n'est pas tranché |
| PO-02 | Seuil de repli SSE → polling adaptatif | Seuil chiffré non dérivable | `roadmap-entree-build.md §3.6`, ADR-004 | Opérateur / build | Bloque la clôture opérationnelle du jalon « Partage + temps réel », pas son entrée (§9) |
| PO-03 | Branche fédérée email non vérifié (RB-10-08 branche b) | Résolution de sécurité/RGPD non ratifiée | ADR-015 §2.3, RB-10-08 | Opérateur (ratification sécurité) + Lot 14 juridique (facette RGPD) | Aucun cas de recette (CR-UC10-10) ne présume de l'issue ; Option A « reclaim-in-place » documentée mais non actée |
| PO-04 | Réconciliation de nomenclature « J1 » | Incohérence de corpus à ratifier | `roadmap-entree-build.md §5` | Opérateur | Toute référence de ce document à un jalon nomme le contenu (Socle/Local-only/Cloud+migration/Partage+temps réel), jamais une numérotation de vague |
| PO-05 | Délai de purge RGPD compte (« J+30 ») | Critère de recette à confirmer | `roadmap-entree-build.md`, corpus RGPD | Opérateur | Le délai est dérivable de la roadmap mais sa formulation en critère de recette produit reste ouverte |
| PO-06 | Nom de l'object store racine (`campaigns` vs `spaces`) | Watch point de build | ADR-017 §1.1, ADR-018 | Équipe de build | Ne pas figer ce nom dans un critère de recette (§5) |
| PO-07 | Ordre de dégel UC-15 pour un tier intermédiaire borné | Non fixé, post-MVP | UC-15, RB-15-04 | Opérateur / build (à l'introduction d'un tier intermédiaire) | Sans impact MVP — le tier binaire actuel implique un dégel total |
| PO-08 | Réimport de fichier de sauvegarde (UC-01, US-01-08) | Post-MVP, recette différée | US-01-08 | Opérateur (roadmap produit) | Aucun cas de recette MVP ; règles de validation tracées pour reprise ultérieure |
| PO-09 | UC-03 exception E2 — perte de connexion ou erreur de sauvegarde | Non dérivable d'un Gherkin ni d'une RB ferme | UC-03 fiche §Exceptions E2 | Équipe de build (spécification à affiner) | Non recetté ; couvert au mieux par le principe général de résilience réseau (NFR-OFF) |
| PO-10 | UC-06 exception E3 — MJ consulte une session CLOSED en lecture seule | Nuance non spécifiable au-delà de l'existant | UC-06 fiche §Exceptions E3 | Équipe de build | Substantiellement déjà couvert par CR-UC06-15/16/18/26/27 ; la nuance additionnelle n'est pas dupliquée en case redondante |
| PO-11 | Deux gates humains (`[GATE MARCHÉ]`, `[GATE JURISTE EU]`) | Rappel structurel | `roadmap-entree-build.md §3.3/3.5` | Opérateur | Hors-CI par nature — ne jamais convertir en dette de test (§9) |
| PO-12 | Seuil de tentatives (N) du rate limiting brute-force | Seuil chiffré non dérivable | ADR-015 (rate limiting hybride par-IP/par-compte, valeur non fixée) | Équipe de build | CR-UC10-21 ne chiffre pas N ; aucune valeur inventée dans ce document |
| PO-13 | Sous-points B1.10 non tranchés (403 vs 404 sur ressource non autorisée, schéma exact du code 429) | Détail de contrat à trancher | roadmap Annexe A, B1.10 | Équipe de build | Le contrat OpenAPI des endpoints d'authentification reste à compléter sur ces deux points avant clôture du jalon Cloud + migration |

---

## 13. Métriques & reporting

Les métriques ci-dessous sont définies **qualitativement** — tout seuil chiffré (taux cible, pourcentage plancher) est un point ouvert (§12), jamais une valeur inventée ici.

- **Couverture des cas Must exécutés** : proportion des cas de recette de priorité Must, par UC et globale, ayant reçu un verdict (`OK`/`KO`/`N/A` justifié) sur le total des cas Must tracés.
- **Taux de réussite** : proportion de cas exécutés avec verdict `OK` sur le total des cas exécutés, par niveau de test et par UC.
- **Densité d'anomalies par sévérité** : nombre d'anomalies ouvertes, réparties selon l'échelle de sévérité (§7), rapporté au périmètre recetté (par jalon).
- **Nombre de points ouverts résiduels** : décompte des entrées actives du registre (§12), à surveiller pour tendance décroissante à l'approche de chaque gate.
- **Contrôle d'intégrité des citations** : à chaque jalon, une passe de relecture bornée vérifie que chaque ligne `CR-` cite un scénario Gherkin (par titre) ou une règle métier (RB) réel et à jour dans le corpus source — mécanisme concret de la clause de re-dérivation (§1), pas seulement une politique déclarée.

**Cadence de reporting** : à chaque jalon de build (Socle, Local-only, Cloud + migration, Partage + temps réel) et avant chaque gate humain (`[GATE MARCHÉ]`, `[GATE JURISTE EU]`), un état des quatre métriques ci-dessus est produit pour éclairer la décision de sortie de jalon ou de gate.

---

## Annexe A — Glossaire

- **Visibilité** : `GM_ONLY` (visible du MJ — `OWNER`/`GM` — uniquement), `PUBLIC` (visible de tous les membres actifs et `GuestAccess` actifs de l'espace), `PLAYER_PRIVATE` (visible de son auteur seul, y compris pour `OWNER`/`GM` — RB-06-25/26).
- **Visibilité vs épinglage** : deux dimensions indépendantes. L'épinglage détermine la présence d'un document dans le panneau « Documents épinglés » d'une session ; la visibilité détermine qui peut le voir. Retirer un partage ne désépingle pas ; désépingler ne retire pas le partage (RB-08-08b/c).
- **Statuts de session** : `LIVE → CLOSED → ARCHIVED`, machine d'états unidirectionnelle, chaque transition irréversible (RB-06-18/20/21).
- **Acteurs d'accès** : `GuestAccess` (accès temporaire sans compte, lié à une session ponctuelle ou à une campagne selon le périmètre, expire après grâce ou révocation) ; `Member` (accès permanent lié à un compte, valide jusqu'à révocation explicite du MJ).
- **Types d'espace** : `PERSONAL` (conteneur personnel préexistant, hors quota, un seul dossier virtuel « Non classés ») ; `CAMPAIGN` (espace partagé durable, 4 dossiers système + « Non classés ») ; `ONE_SHOT` (même modèle que `CAMPAIGN`, parcours utilisateur différent).
- **Codes de test nommés** : `B3.2` (test d'architecture CI, frontières bounded context) ; `B5.1` (query filters EF Core sur tous les chemins de lecture) ; `B5.2` (test IDOR) ; `B1.5` (contrats d'authentification) ; `B1.6` (données invité sur espace vivant) ; `B1.7` (runtime SignalR) ; `B1.10` (contrat OpenAPI endpoints auth) ; `B8.2` (test anti-fuite temps réel) ; `P6` (services Angular IndexedDB) ; `P7` (handler d'import/migration, e2e multi-versions IndexedDB).

## Annexe B — Références du corpus

- `docs/conception/usecases/**` — use cases, source de vérité.
- `docs/conception/user-stories/**` — user stories, Gherkin source unique des critères d'acceptation.
- `docs/conception/nfr/**` — exigences non fonctionnelles.
- `docs/planning/roadmap-entree-build.md` — jalons, critères de sortie factuels, gates humains.

## Annexe C — Historique des versions

- **v1.0** — consolidation initiale (192 lignes) : stratégie de test synthétique et cahier de recette agrégé par UC (une ligne par UC, critères d'acceptation groupés).
- **v2.0** (ce document) — version de référence, produite en 2 passes : structure professionnelle complète (contrôle documentaire, 10 sections de stratégie, cahier de recette pas à pas), cas de recette dérivés un par un des scénarios Gherkin des User Stories avec traçabilité explicite, matrice de traçabilité des 15 UC, registre des points ouverts consolidé, métriques qualitatives et annexes.

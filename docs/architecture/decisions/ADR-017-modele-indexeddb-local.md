# ADR-017 — Modèle IndexedDB local et sécurité du mode local

- **Statut** : Accepté
- **Date** : 2026-06-10

> **Nature : mixte — à dominante pré-implémentation** — modèle de stockage et politique de sécurité client à confirmer à l'entrée en build ; parts de conception qui contraignent le modèle dès maintenant : l'invariant migration-only (aucune synchronisation continue au MVP), les bandeaux de durabilité et de confidentialité du mode local, et le non-chiffrement at-rest assumé. *(Annotation du 2026-06-10 — qualification postérieure à l'arbitrage T-03.)*

---

## Périmètre de cet ADR

Cet ADR est le dernier item du lot L1 (B1.2) du jalon M1. Il formalise ce qu'ADR-016 avait explicitement renvoyé à B1.2 :

- **Dans le périmètre** : structure des object stores IndexedDB, posture de synchronisation (migration-only), stratégie de versionnement et d'upgrade du store, `navigator.storage.persist()` et gestion du best-effort, sécurité du mode local (F-09) — sanitisation XSS côté client, CSP, import JSON, non-chiffrement at-rest.

**Hors périmètre, renvoyés explicitement** :
- Implémentation Angular des services IndexedDB (accès au store, wrappers) → **P6**
- Handler de migration côté serveur, tests e2e navigateur multi-versions IndexedDB → **P7** (renvoi ADR-016 §Points à trancher)
- Offline-first complète, synchronisation continue, édition multi-device hors ligne, résolution de conflits CRDT → **UC-F04, post-MVP**
- Directives CSP complètes (liste exhaustive des directives, valeurs de nonce) → **P6**
- Bibliothèque de sanitisation exacte, liste exhaustive des balises autorisées → **P6**

---

## Contexte

ADR-016 a formalisé le contrat de sérialisation et la frontière de confiance local→cloud. Il a renvoyé à B1.2 deux blocs non spécifiés : le schéma IndexedDB local (C-11 d'ADR-001) et le détail de la sécurité du mode local (F-09). Ces deux blocs sont indissociables : le modèle de stockage conditionne la surface d'attaque ; la sanitisation XSS et la politique CSP ne peuvent pas être conçues sans connaître la structure du store et les chemins de lecture.

Deux points de contexte méritent d'être nommés explicitement.

**Posture de synchronisation.** Au moment de cadrer ce document, une question restait ouverte : le store local doit-il prévoir un moteur de synchronisation continue (delta par entité, suivi de révision, journal de modifications) ? La réponse est non, et elle est décidée ici. Le mode local au MVP est un store CRUD hors-ligne mono-navigateur, sans aucune synchronisation cloud en cours de session. Le seul croisement local→cloud est la migration one-shot décrite dans ADR-016. Cette décision est posée comme invariant, pas comme simplification temporaire : toute infrastructure de sync continue serait architecturalement différente de ce qu'UC-01 spécifie (l.134 — aucune donnée envoyée au serveur en mode local) et relève de UC-F04, architecture offline-first complète, qui est un projet à part entière post-MVP.

**Résolution STRUCTURAL.** F-09 signale un risque XSS Angular exfiltrant le contenu local (CWE-79/312). En mode local pur, il n'existe pas de serveur pour absorber le vecteur : le plancher de sanitisation côté serveur (ADR-016 §2.4) ne couvre pas le rendu local. Ce point est classé STRUCTURAL : un payload XSS persisté en IndexedDB n'est pas seulement dangereux à la lecture locale, il traverse la frontière vers le cloud à la migration, ce qui annulerait partiellement la protection serveur. Cet ADR ferme ce vecteur en imposant une sanitisation équivalente côté client Angular.

---

## Décision

### 1. Modèle IndexedDB — structure aggregate-rooted

#### 1.1 Object stores

Le store local a son propre périmètre, dérivé de UC-01, UC-06 et UC-07 — un périmètre plus large que celui du payload de migration, décrit séparément par ADR-016 §1.2 § Périmètre sérialisé. Les object stores suivants sont créés :

| Object store | Clé primaire | Description |
|---|---|---|
| `spaces` | `id` (UUID local) | Racine de l'agrégat |
| `folders` | `id` (UUID local) | Arborescence rattachée à l'espace |
| `documents` | `id` (UUID local) | Documents avec leurs champs scalaires |
| `document_blocks` | `id` (UUID local) | Blocs de contenu rattachés à un document |
| `document_links` | `[source_id, target_id]` | Références inter-documents |
| `document_tags` | `[document_id, tag]` | Tags normalisés |
| `document_types` | `id` (UUID local) | Types custom uniquement (les types système sont seedés côté serveur) |
| `sessions` | `id` (UUID local) | Statut, titre, résumé, dates |
| `session_pinned_documents` | `[session_id, document_id]` | Références d'épinglage document↔session |
| `session_live_notes` | `[session_id, document_id]` | Références note↔session ; les notes de session elles-mêmes sont des documents de type `LIVE_NOTE`, déjà couverts par l'entrée `documents` |
| `session_view_configs` | `id` (UUID local) | Espace associé — configuration de la vue session |
| `session_view_folders` | `[session_view_config_id, folder_id]` | Liste ordonnée des dossiers mis en avant dans la vue session |

**Exclusions du store local** : entités d'identité (`users`, `space_memberships`, `membership_characters`), flags de cycle de vie serveur (`deleted_at`, `is_deleted`, `purge_claimed_at`). Ces exclusions ne sont pas des omissions de simplification — elles délimitent ce que le mode local UC-01 contient.

Le store local n'est pas identique au payload de migration : trois éléments qu'il contient en sont exclus pour des motifs propres à l'export — les sessions en statut `LIVE`, `session_view_configs` et `session_view_folders`. Ces motifs sont énoncés dans ADR-016 §1.2 § Exclusions explicites.

#### 1.2 Structure aggregate-rooted plutôt que miroir relationnel

Le store local n'est pas un miroir des tables serveur. Il est organisé autour de l'espace comme racine d'agrégat : son contenu (dossiers, documents, blocs, liens, tags, types custom) lui est rattaché par `space_id`. Cette organisation est délibérée.

Un miroir relationnel réimporterait dans le navigateur la logique d'intégrité référentielle serveur (contraintes FK, ordre d'insertion topologique, résolution de cycles), ce qu'ADR-001 refuse explicitement. L'aggregate-rooted conserve le store simple et interrogeable par le seul chemin qu'UC-01 requiert : accéder à tout le contenu d'un espace depuis sa racine.

Conséquence directe sur la couture de projection ADR-016 §1.4 : la fonction `store local → payload` applique un filtre nommé — elle écarte les sessions en statut `LIVE`, `session_view_configs` et `session_view_folders` (§1.1) — mais ne comporte aucun reformatage structurel complexe au-delà de ce filtre. Le reste du contenu transporté est une sérialisation directe du store vers le payload.

#### 1.3 Indexes locaux

Les indexes sont créés uniquement pour les chemins de lecture qu'UC-01 exige réellement. Aucun index spéculatif.

| Index | Object store | Champ | Justification |
|---|---|---|---|
| `by_space` | `documents`, `folders`, `document_types` | `space_id` | Navigation : lister tout le contenu d'un espace |
| `by_folder` | `documents` | `folder_id` | Navigation de l'arborescence d'un dossier |
| `by_document` | `document_blocks`, `document_links`, `document_tags` | `document_id` / `source_id` | Lecture du contenu d'un document |
| `by_title` | `documents` | `title` | Recherche par titre (UC-01 — fonctionnalité « recherche locale ») |

> **Note `by_document`** : le champ indexé diffère selon l'object store. `document_links` utilise `source_id` (sa clé primaire est la composite `[source_id, target_id]` — cf. §1.1) ; `document_blocks` et `document_tags` utilisent `document_id`.

La recherche full-text sur le contenu des blocs (`document_block.content`) est post-MVP — non indexée ici, cohérent avec la même décision côté serveur (content-library.md note GIN tsvector).

#### 1.4 Versionnement du store et stratégie d'upgrade

Le store local dispose d'un **numéro de version IndexedDB** (`IDBOpenDBRequest` via `indexedDB.open(name, version)`). Ce versionnement est **distinct du `schemaVersion` du payload ADR-016** : les deux évoluent indépendamment.

- Le `schemaVersion` du payload gouverne le contrat entre le store local et le serveur — il est défini dans ADR-016 et touche à la migration.
- La version IndexedDB gouverne la structure interne du store dans le navigateur — elle peut évoluer entre deux versions de l'application sans que le contrat de payload change.

Cette séparation est la matérialisation directe de la couture de projection ADR-016 §1.4 : si le schéma IndexedDB évolue (nouvel index, renommage d'object store), la fonction de projection absorbe la différence, et le `schemaVersion` du payload reste stable pour le serveur.

La migration de version du store (`onupgradeneeded`) est déclarée dans l'handler d'ouverture. Chaque version doit être associée à une procédure d'upgrade documentée dans le code (création des nouveaux object stores ou indexes, migration des données existantes si nécessaire). Un upgrade sans procédure documentée est interdit — une régression silencieuse de store IndexedDB en navigateur réel n'est pas détectable en CI.

#### 1.5 Validations locales TypeScript

Les validations locales sont minimales, conformément à ADR-001 §Décision et à l'invariant `validation locale ⊆ validation serveur` (ADR-001 §Compléments). En mode local, le serveur n'est pas disponible pour valider ; ces validations constituent un plancher de qualité, pas une garantie métier.

**Plancher actif** :
- Titre de document non vide (avant écriture dans le store).
- Structure de blocs : au moins un bloc par document, position (`order`) unique parmi les blocs d'un même document.

**Discipline, non garantie outillée** : l'invariant `validation locale ⊆ validation serveur` est une règle de conception que l'équipe applique lors de l'implémentation TypeScript. Il n'existe pas de test cross-langage automatique qui vérifie la cohérence entre les règles TS locales et les Value Objects C# serveur. La migration revalide systématiquement via les VO serveur — c'est ce mécanisme qui constitue le filet de sécurité.

#### 1.6 RB-01-03 — absence de plafond de création en mode local

*Décision initiale retirée le 2026-09-01 — trace et motif en § Compléments post-revue.*

Il n'existe aucun plafond de création d'espace en mode local. Le store ne compte ni ne borne le nombre d'espaces avant écriture dans l'object store `spaces` : la seule limite à la création est la capacité de stockage allouée au navigateur (UC-01 §Modèle d'accès et de monétisation), une contrainte physique et non une règle produit à faire appliquer par un compteur applicatif.

RB-01-03 porte exclusivement sur la synchronisation cloud d'un compte gratuit — un maximum de 3 espaces `CAMPAIGN`/`ONE_SHOT` synchronisables, l'espace `PERSONAL` étant exclu du décompte (UC-01). Cette règle est hors périmètre du store local décrit ici : elle conditionne l'écriture côté serveur à la migration (ADR-016), pas l'écriture dans IndexedDB.

---

### 2. Posture de synchronisation — migration-only, invariant MVP

Le mode local est un store CRUD hors-ligne mono-navigateur. Il n'y a pas de moteur de synchronisation continue, pas de notion de delta, pas de journal de modifications local destiné à être rejoué sur le serveur.

Trois régimes sont à distinguer explicitement pour éviter toute dérive de conception à l'implémentation :

**(a) Mode local pur** : toutes les opérations (création, lecture, modification, suppression) s'exécutent localement dans IndexedDB. Aucune donnée n'est envoyée au serveur (UC-01 §Règles métier). Il n'y a pas de session cloud active, pas de token JWT (ADR-015 §Périmètre).

**(b) Migration one-shot local→cloud** : le seul passage de données vers le serveur est la migration décrite dans ADR-016. Elle est déclenchée explicitement par l'utilisateur lors de la création de compte ou depuis les paramètres. Elle se déroule en lot, espace par espace, avec gate de confirmation anti-appropriation (ADR-016 §4). Ce n'est pas un canal de synchronisation : il n'y a pas de delta, pas de merge, pas d'idempotence de révision. La migration est one-shot ; elle transfère les données locales vers le cloud sous l'identité du compte créé.

**(c) Mode cloud** : après la migration (ou après une connexion directe sans données locales), l'application opère en mode cloud. Les écritures sont serveur-autoritaires — le serveur est la seule source de vérité (ADR-001 §Décision). UC-06 E1 (perte réseau en mode cloud) produit un brouillon transitoire côté client, pas un store local persistant : ce brouillon disparaît si la connexion n'est pas rétablie ; il n'y a pas de réconciliation asynchrone.

**Invariant anti-dérive** : il n'y a aucune synchronisation cloud→local au MVP. Un utilisateur qui a un compte ne peut pas modifier ses données cloud hors ligne et les re-synchroniser — cette capacité est UC-F04 (offline-first complète), post-MVP. La migration (ADR-016) est locale→cloud, one-shot, avec confirmation ; ce n'est pas un mécanisme de sync bidirectionnelle.

Nommer cet invariant explicitement sert à borner le scope de l'implémentation P6 : toute infrastructure TypeScript qui irait au-delà (suivi de révision local, journal de conflits, sync delta) serait hors périmètre MVP et devrait être soumise à une décision explicite.

---

### 3. `navigator.storage.persist()` — persistance et gestion du best-effort

#### 3.1 Demande de persistance

L'appel à `navigator.storage.persist()` est effectué dès l'entrée en mode local, conformément à ADR-001 §Compléments G-08 (préalable identifié avant J1). En l'absence de cet appel, les données IndexedDB sont soumises à l'éviction navigateur sous pression mémoire (comportement best-effort). La demande de persistance est sans garantie — le navigateur peut la refuser selon sa politique (absence d'interaction suffisante avec le site, site non ajouté en page d'accueil, etc.).

#### 3.2 La branche « refusé / best-effort » est un état de première classe

La valeur booléenne retournée par `persist()` est lue et traitée. L'état `navigator.storage.persisted()` est vérifié à chaque entrée en mode local pour détecter une réinitialisation entre sessions.

Si la persistance est accordée : aucune notification supplémentaire — comportement silencieux.

Si la persistance est refusée ou en best-effort : un **bandeau de durabilité non bloquant** est affiché, avec le message : *« Vos données sont en mode éphémère — elles peuvent être supprimées par le navigateur. Créez un compte pour les sécuriser. »* Ce bandeau est câblé à l'invite de création de compte (UC-10). Il ne bloque pas l'accès aux fonctionnalités — l'utilisateur peut continuer à travailler.

Ce comportement est cohérent avec UC-01 l.34 (risque communiqué clairement) et avec la philosophie du mode local : friction nulle à l'entrée, information honnête sur les limitations.

#### 3.3 Maillon NON VÉRIFIABLE IN BUILD

Le comportement de grant ou de refus de `navigator.storage.persist()` par le navigateur, et le comportement d'éviction IndexedDB sous pression mémoire, sont des comportements d'environnement d'exécution non assertables en CI. La logique applicative (lire le booléen retourné, déclencher le bandeau si false) est testable par mock de l'API `navigator.storage`. Le comportement réel du navigateur ne l'est pas.

Ce maillon non vérifiable est nommé pour éviter une fausse confiance dans la couverture CI sur ce point.

---

### 4. Sécurité du mode local (F-09)

#### 4.1 Sanitisation côté client — résolution du vecteur STRUCTURAL

En mode local pur, il n'y a pas de serveur. La sanitisation côté serveur (ADR-016 §2.4) ne s'applique pas aux données qui n'ont pas encore traversé la frontière locale→cloud.

Si un payload contenant du contenu XSS (balises `<script>`, attributs `on*`, vecteurs d'injection dans `document_block.content`) est persisté dans IndexedDB — que ce soit par saisie directe dans l'éditeur ou par import JSON (§4.3) —, il sera rendu tel quel dans le navigateur avant toute migration. Ce vecteur produit une exécution de code XSS dans l'application Angular avec accès au store local. De plus, ce contenu traverserait la frontière à la migration : bien que le serveur le sanitise à l'import (ADR-016 §2.4), il aurait déjà été exécuté côté client avant ce filtrage serveur.

**Décision** : le rendu de `document_block.content` (champ libre, ADR-016 §1.3) est sanitisé côté client Angular avant injection dans le DOM. La sanitisation utilise `DomSanitizer` d'Angular avec une politique de liste blanche positive : seules les balises explicitement autorisées sont conservées ; les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont supprimés sans exception.

Ce plancher est identique en posture à celui d'ADR-016 §2.4 (côté serveur) : même politique de liste blanche positive, mêmes interdictions absolues. La liste exacte des balises autorisées et la bibliothèque de sanitisation (ex. DOMPurify côté client ou `DomSanitizer` Angular natif) sont des points d'implémentation renvoyés à P6, mais la posture et les règles minimales sont actées ici.

Ce point ferme le vecteur classé STRUCTURAL (§Contexte). La conformité conçue est non certifiée jusqu'à l'implémentation P6 (§Conséquences).

#### 4.2 CSP stricte

Une politique CSP (Content Security Policy) stricte est définie pour l'application Haversack :

- `default-src 'self'` : toutes les ressources par défaut restreintes à l'origine de l'application.
- `script-src 'self'` : pas de script inline, pas de scripts depuis des sources tierces non explicitement autorisées.
- Sources restreintes : les assets, images, styles sont limités à l'origine et aux CDN explicitement approuvés.

Cette CSP sert deux fonctions complémentaires. D'abord, c'est une défense en profondeur anti-XSS : même si un vecteur échappe à la sanitisation applicative, l'exécution de scripts non autorisés est bloquée par le navigateur. Ensuite, elle rend **observable** la règle « aucune donnée envoyée au serveur en mode local » : en mode local, il ne doit y avoir aucun fetch vers l'API Haversack — une CSP stricte avec `connect-src 'self'` (ou restreinte à l'API) permettrait de détecter toute tentative d'envoi non autorisée.

La posture et les directives principales sont actées ici. Le détail complet (directives exhaustives, valeurs de nonce pour les scripts légitimes, ajustements pour les CDN de ressources) est renvoyé à P6.

#### 4.3 Import JSON local (UC-01 A4)

L'utilisateur peut importer un fichier JSON exporté précédemment (scénario UC-01 A4). Un fichier importé provient d'une source externe non contrôlée — il ne bénéficie pas de la frontière de confiance serveur d'ADR-016, laquelle s'applique uniquement lors de la migration.

Avant toute écriture dans IndexedDB, le fichier importé est soumis à deux vérifications :

1. **Validation structurelle** : le fichier est validé contre la structure attendue du payload (présence de `schemaVersion`, format des champs, types). Un fichier malformé est rejeté avec un message d'erreur.

2. **Sanitisation de contenu** : le contenu de chaque `document_block.content` est sanitisé selon le même plancher que §4.1 — liste blanche positive, interdiction absolue des balises et attributs dangereux — avant écriture dans le store.

L'ordre est impératif : valider la structure en premier (éviter d'exécuter de la sanitisation sur un document structurellement incohérent), sanitiser le contenu en second (avant persistance, pas après lecture). Un fichier JSON importé n'est jamais écrit tel quel dans IndexedDB.

#### 4.4 Données at-rest — chiffrement non implémenté et bandeau de confidentialité

IndexedDB n'est pas chiffré dans le navigateur. Un autre utilisateur du même poste peut lire les données via les DevTools du navigateur (onglet Application → IndexedDB). Ce fait est structurel : en mode sans-compte, il n'existe pas de secret à partir duquel dériver une clé de chiffrement — la même impossibilité que celle notée dans ADR-016 §2.1 pour la preuve d'appartenance. Sans secret, toute dérivation de clé est circulaire.

**Décision** : le non-chiffrement at-rest est une **limitation MVP conçue, non silencieuse**. Un **bandeau d'avertissement de confidentialité** est affiché en mode local, distinct du bandeau de durabilité (§3.2) :

- Bandeau durabilité (§3.2) : *risque de perte de données* par éviction navigateur → invite à créer un compte.
- Bandeau confidentialité (ce paragraphe) : *risque d'accès par un tiers* sur poste partagé via DevTools → invite à créer un compte.

Ces deux bandeaux sont distincts car leurs causes, leurs risques et leurs publics cibles sont différents. Les afficher séparément évite un message ambigu.

La relation avec le gate d'appropriation d'ADR-016 §4 est à noter : ce gate borne le risque d'**import accidentel** par un tiers sur poste partagé ; le bandeau de confidentialité adresse le risque de **lecture directe** des données au repos. Les deux mécanismes sont complémentaires et couvrent des vecteurs distincts.

**Dette assumée** : le chiffrement at-rest IndexedDB est reporté post-MVP. La condition de levée est l'existence d'un mécanisme de secret utilisateur (ex. mot de passe d'application, clé dérivée de l'identifiant d'appareil) — ce mécanisme n'existe pas dans le périmètre du mode local sans compte.

#### 4.5 Absence de tokens en mode local

En mode local, aucun JWT d'accès, aucun refresh token, aucun token de session ne vit dans le navigateur (ni en mémoire, ni en localStorage, ni dans un cookie). ADR-015 §Périmètre acte que le mode local n'a pas d'authentification. La session cloud et son stockage de token — gouvernés par ADR-015 — ne démarrent qu'à la création de compte ou à la connexion. Ce point est confirmé ici pour clore toute ambiguïté d'implémentation : un service Angular en mode local qui tenterait de lire ou écrire un token dans le store serait hors périmètre.

---

## Alternatives considérées

**Structure miroir des tables serveur**

Reproduire la structure relationnelle des tables serveur (spaces, folders, documents, etc., avec leurs FK et contraintes d'unicité) dans IndexedDB. Écarté : IndexedDB n'est pas une base relationnelle et n'implémente pas les contraintes FK. Réimporter ces contraintes côté client imposerait une logique d'intégrité référentielle TS sans outillage adapté, contraire à ADR-001 §Décision. L'aggregate-rooted est la structure cohérente avec la posture CRUD du mode local.

**Versionner le store à la même version que le payload**

Utiliser le `schemaVersion` du payload ADR-016 comme numéro de version du store IndexedDB — un seul compteur pour les deux. Écarté : les deux contrats évoluent à des rythmes différents. Le payload est un contrat inter-systèmes (local → serveur) dont la stabilité est une exigence ; le schéma IndexedDB est interne au client et peut évoluer librement. Les coupler forcerait une incrémentation du `schemaVersion` côté serveur pour chaque évolution interne du store, créant une pression sur le contrat de migration.

**Sanitisation post-lecture uniquement (lazy)**

Sanitiser `document_block.content` au moment du rendu seulement, sans sanitiser à l'écriture dans IndexedDB. Écarté pour le cas de l'import JSON (§4.3) : un fichier importé contenant du XSS serait persisté non sanitisé, et si un bug de rendu ou une future lecture non prévue bypasse la sanitisation au rendu, le payload brut reste en store. Sanitiser avant persistance (pour l'import) et au rendu (pour la saisie en session) constitue une défense en profondeur cohérente.

**Chiffrement at-rest avec clé dérivée du `fingerprint` d'appareil**

Dériver une clé de chiffrement à partir d'un fingerprint d'appareil (user agent, résolution, etc.) pour chiffrer IndexedDB sans secret explicite. Écarté : les fingerprints sont non secrets et partiellement stables — un tiers sur le même appareil obtiendrait le même fingerprint, annulant la protection. Cette approche donnerait une fausse impression de sécurité. Le bandeau d'avertissement est la réponse proportionnée : communiquer la limitation honnêtement plutôt que la masquer derrière une protection inefficace.

**Moteur de sync légère (delta par timestamp)**

Prévoir dès le MVP un mécanisme de synchronisation delta basé sur les `updated_at` locaux, pour permettre une future sync continue sans réécriture. Écarté : UC-01 est explicitement mono-navigateur sans sync cloud, et toute infrastructure de sync préparerait un chemin vers UC-F04 sans décision explicite. La couture de projection ADR-016 §1.4 est la seule interface entre le store local et le serveur au MVP ; la garder simple et sans journal est cohérent avec l'invariant anti-dérive posé en §2.

---

## Conséquences

### Couture de projection filtrée, sans reformatage structurel (ADR-016 §1.4 confirmé)

La structure aggregate-rooted du store local rend la fonction `store local → payload` sans reformatage structurel complexe au MVP, au filtre près qui écarte les sessions en statut `LIVE`, `session_view_configs` et `session_view_folders` (§1.1). L'enveloppe `schemaVersion`, `exportedAt`, `appVersion` est ajoutée par la fonction de projection ; les UUID locaux sont transportés tels quels (le serveur les remplace à l'import, ADR-016 §2.2).

### Maillon NON VÉRIFIABLE IN BUILD (cumulatif)

Deux maillons non vérifiables en CI sont introduits ou confirmés par cet ADR :

- **Persistance navigateur** (§3.3) : le comportement de grant/refus de `navigator.storage.persist()` et l'éviction IndexedDB sous pression mémoire ne peuvent pas être assertés en CI.
- **Multi-versions IndexedDB en navigateur réel** (héritage ADR-016) : la fonction de projection `store local → payload` sur différentes versions du schéma IndexedDB, avec des données persistées par des versions antérieures de l'application, n'est pas couvrable par les tests d'archi CI. Ces vérifications relèvent des tests e2e navigateur renvoyés à P7.

La logique applicative autour de ces maillons (lire le booléen `persist()`, déclencher le bandeau, déclencher la sanitisation) est elle testable par mock des APIs navigateur.

### Dettes nommées (non silencieuses)

| Dette | Nature | Ticket |
|---|---|---|
| Chiffrement at-rest IndexedDB | Sécurité — non implémentable sans secret utilisateur en mode sans-compte | post-MVP |
| Offline-first complète / synchronisation continue / résolution de conflits CRDT | Architecture — UC-F04, projet à part entière | post-MVP |
| Directives CSP complètes (liste exhaustive, nonces) | Sécurité — posture actée §4.2, détail à l'implémentation | P6 |
| Bibliothèque de sanitisation exacte et liste complète des balises autorisées | Sécurité — posture actée §4.1, choix d'implémentation | P6 |
| Implémentation Angular des services IndexedDB (wrappers, accès au store) | Implémentation — hors périmètre de cet ADR | P6 |

### Conformité conçue, non certifiée

La sanitisation côté client (§4.1) ferme le vecteur Stored XSS au sens applicatif pour le mode local. La bibliothèque et la liste de balises exactes sont des points d'implémentation P6 ; leur adéquation devra être vérifiée à cette occasion.

La CSP (§4.2) fournit une défense en profondeur. Les directives complètes sont à spécifier en P6 ; la posture est actée ici.

Le non-chiffrement at-rest (§4.4) est une limitation conçue, communiquée via bandeau. Elle n'est pas traitée comme une conformité — c'est une limitation assumée.

---

## Points à trancher

- **P6** — Implémentation Angular : services d'accès IndexedDB, wrappers, intégration `DomSanitizer`, bandeaux (durabilité + confidentialité), CSP complète avec directives exactes, bibliothèque de sanitisation côté client.
- **P7** — Tests e2e navigateur multi-versions IndexedDB (héritage ADR-016 §Points à trancher) ; handler d'import côté serveur.

---

## Croisements

| ADR / Document | Nature du croisement |
|---|---|
| **ADR-016** | Fondation directe — périmètre sérialisé (§1.2), couture de projection filtrée, sans reformatage structurel (§1.4), plancher de sanitisation serveur (§2.4) dont cet ADR est le pendant client, format et `schemaVersion` distincts du versionnement IndexedDB |
| **ADR-001** | Mode local CRUD + validations minimales (§1.5), invariant `validation locale ⊆ validation serveur`, G-08 (`navigator.storage.persist()`), question ouverte I-05 (sync) renvoyée à UC-F04 |
| **ADR-003** | Stack Angular — `DomSanitizer`, implémentation des services IndexedDB en P6 |
| **ADR-015** | Confirmation de l'absence de JWT/token en mode local (§4.5) ; session cloud et tokens gouvernés par ADR-015 démarrent à la création de compte |
| **UC-F04** | Offline-first complète, résolution de conflits CRDT — post-MVP, périmètre distinct et explicitement exclu de cet ADR |
| **UC-01** | Bandeaux (durabilité + confidentialité), invite cloud, import JSON (A4), absence de plafond de création en mode local, RB-01-03 (plafond cloud uniquement — 3 espaces `CAMPAIGN`/`ONE_SHOT`, `PERSONAL` exclu), risque communiqué clairement (l.34) |
| **UC-10** | Bandeaux câblés à l'invite de création de compte (UC-10) |

---

## Compléments post-revue

**§1.6 — absence de plafond de création en mode local (2026-09-01).** La décision initialement inscrite au §1.6 appliquait RB-01-03 comme plafond de *création* en mode local : blocage de l'écriture au-delà de 3 espaces dans l'object store `spaces`, avec un message orientant vers la création de compte. Cette décision portait un point ouvert `[À TRANCHER — PRODUIT]` signalant que cette application de RB-01-03 n'avait pas de source dans UC-01, lequel emploie cet identifiant pour le plafond de synchronisation cloud, espace personnel exclu.

Ce point est tranché : **il n'existe aucun plafond de création en mode local.** UC-01 a été amendé pour l'énoncer explicitement — la seule contrainte de création en mode local est la capacité de stockage du navigateur. RB-01-03 est confirmé comme portant exclusivement sur la synchronisation cloud d'un compte gratuit (3 espaces `CAMPAIGN`/`ONE_SHOT`, espace `PERSONAL` exclu) — pas sur la création locale. Le §1.6 est réécrit en conséquence : aucun compteur de plafond n'est implémenté dans le store local décrit par cet ADR.

Cette révision du 2026-09-01 ne touchait, à la date de son inscription, à aucune autre décision de cet ADR — object stores, versionnement du store, `navigator.storage.persist()`, posture migration-only et sécurité du mode local restaient inchangés à cette date.

**§1.1 — périmètre du store local (2026-09-02).** L'exclusion des entités de session (`sessions`, `session_view_configs`, `session_pinned_documents`, `session_live_notes`, `session_view_folders`) reposait sur la prémisse « exclusions identiques à ADR-016 ». Cette prémisse est fausse : la correction datée du 2026-06-10 apportée à ADR-016 §1.2 § Exclusions explicites qualifie de factuellement fausse l'énoncé selon lequel le mode local ne comprend pas de session, en citant UC-06 et UC-07.

Le §1.1 est réécrit en conséquence : le store local est désormais énoncé avec son périmètre propre, dérivé de UC-01, UC-06 et UC-07, plus large que le payload de migration décrit par ADR-016 §1.2 § Périmètre sérialisé. Il contient les sessions y compris en statut `LIVE`, ainsi que la configuration de vue (`session_view_configs`, `session_view_folders`). Trois de ces éléments restent exclus du payload de migration pour des motifs propres à l'export énoncés par ADR-016 §1.2 § Exclusions explicites : sessions en statut `LIVE`, `session_view_configs`, `session_view_folders`.

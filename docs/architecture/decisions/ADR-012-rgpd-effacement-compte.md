# ADR-012 — RGPD : effacement de compte (Art. 17 — droit à l'oubli)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session de cadrage P1)
- **Findings liés** : F-03, F-08, B1.4 (renvoi ADR-011 *Points à trancher*)

> *Annotation du 2026-06-12 — ADR-018 : ajout de la catégorie contenu d'espace `PERSONAL` (hard-delete inconditionnel) ; override du défaut de conservation pour les espaces partagés.*

---

## Périmètre de cet ADR

Cet ADR couvre exclusivement les traitements déclenchés par la **suppression d'un compte utilisateur** (saga `UserAnonymized`). Il définit la politique RGPD correspondante : procédure d'anonymisation, sort des données par catégorie, périmètre de l'effacement étendu (option Art. 17), règle anti-résidu F-08, purge des logs de corrélation et obligations procédurales Art. 12§3.

**Hors périmètre de cet ADR** :
- Données d'invité (GuestAccess, `display_name`, base légale, information Art. 13) → **ADR-013**
- Purge à la suppression d'espace (saga `SpaceDeleted`, J+30) → **ADR-010 + ADR-011**
- Modèle d'autorisation API → **ADR-007** (B1.3)
- Politique de mot de passe / validation email / JWT → **[ADR-015](ADR-015-securite-authentification-mvp.md)** (résolu)
- Portabilité Art. 20 → post-MVP

---

## Contexte

ADR-007 a acté le principe d'anonymisation par réécriture : l'`UserId` est conservé pour la continuité des espaces actifs, l'email et le `display_name` sont remplacés par des valeurs neutres. ADR-011 a défini la saga `UserAnonymized` et son ordre impératif d'exécution. Ces deux ADR définissent le **mécanisme** ; le présent ADR définit la **politique** : quelles données sont supprimées, conservées, ou purgées, sur quelle base légale, et selon quelle procédure.

Deux questions restaient ouvertes au terme d'ADR-011 :

1. **Périmètre de l'option Art. 17 (effacement étendu)** : quels documents attachés à un compte sont supprimables sur un espace vivant, et lesquels sont conservés et à quel titre ?
2. **Règle F-08** : que faire des notes `PLAYER_PRIVATE` d'un compte supprimé lorsqu'un nouvel invité est ultérieurement réassocié au même `character_id` ?

---

## Décision

### 1. Procédure d'anonymisation — rappel et précisions

La procédure est définie dans ADR-007 et ADR-011. Les points ci-dessous en rappellent les contraintes et en précisent les ambiguïtés.

**Ordre impératif** (non modifiable sans rouvrir ADR-011) :

1. Réécriture de `asp_net_users.password_hash` et `security_stamp` avant tout appel à `users.Anonymize()`. Cette étape est **obligatoire, pas optionnelle** : si un second effacement intervenait après une première `Anonymize()` sans cette réécriture, la contrainte `UNIQUE` sur `email` serait violée (valeur `deleted-{id}@haversack.invalid` déjà présente).
2. `users.email := 'deleted-{id}@haversack.invalid'` — format unique garantissant la non-violation de la contrainte UNIQUE.
3. `users.display_name := '[Compte supprimé]'`.
4. `users.status := 'DELETED'`.
5. L'`id` de l'utilisateur est **conservé** : les FK vers `users.id` (`spaces.owner_id`, `space_memberships.user_id`, `folders.created_by_id`, `documents.created_by_id`, `sessions.created_by_id`) restent intactes. L'identifiant survit anonymisé ; les contraintes NOT NULL sont satisfaites.

**Base légale de la pseudonymisation** : intérêt légitime (Art. 6§1(f)) — continuité des espaces actifs pour les membres tiers. Cette posture est documentée dans la politique de confidentialité.

---

### 2. Tableau des données du compte — sort à l'effacement

| Catégorie | Données | Sort à l'effacement | Base légale si conservée |
|---|---|---|---|
| Identité | `email`, `display_name`, `password_hash`, `security_stamp` | **Anonymisés** (réécriture) — voir procédure §1 | — |
| Identifiant | `users.id` (`UserId`) | **Conservé** | Intérêt légitime — continuité espaces tiers (Art. 6§1(f)) |
| Espaces possédés | `spaces.owner_id` (FK vers `users.id`) | **FK intacte**, `owner_id` pointe vers `UserId` anonymisé — **pour les espaces partagés (CAMPAIGN, ONE_SHOT) uniquement** *(exception PERSONAL — voir dernière ligne du tableau)* | Idem — l'espace reste lisible pour les membres actifs |
| Memberships | `space_memberships.user_id` | **FK intacte** (membership survit, référence le `UserId` anonymisé) | Idem |
| Documents créés | `documents.created_by_id`, `folders.created_by_id`, `sessions.created_by_id` | **FK intacte** — le contenu survit dans les espaces partagés ; l'auteur est anonymisé | Idem (continuité éditoriale pour les membres tiers) |
| Documents `PLAYER_PRIVATE` créés par l'utilisateur | `documents WHERE created_by_id = userId AND visibility = 'PLAYER_PRIVATE'` | **Supprimés** — voir §3(a) | Contrainte légale : donnée personnelle exclusive, aucun intérêt légitime tiers |
| Documents non partagés | `documents WHERE created_by_id = userId` et non partagés au sens §3(b) | **Supprimés** — voir §3(b) | Contrainte légale : aucun usage tiers démontrable |
| Documents partagés / `GM_ONLY` | Documents contribués à un espace partagé et visibles ou utilisés par d'autres membres | **Conservés** — voir §3(c) | Intérêt légitime — continuité pour les tiers (Art. 17§3(e)) |
| Notes liées aux personnages de l'utilisateur | `documents WHERE character_id IN (SELECT character_id FROM membership_characters WHERE user_id = userId) AND created_by_id = userId` (l'ensemble des `character_id` liés à ce `user_id` via `membership_characters`, restreint aux espaces vivants) | **Supprimés** avec les `PLAYER_PRIVATE` (§3(a)) | — |
| Notes live de session (`session_live_notes`) | `session_live_notes` sont des documents de type `LIVE_NOTE` — leur sort suit la règle `documents.created_by_id` : anonymisé si conservé (partagé, §3(c)) / supprimé si `PLAYER_PRIVATE` (§3(a)). Si une `session_live_notes` portait un chemin de donnée personnelle hors colonne `document`, ce chemin serait hors tableau (à vérifier au schéma lors de l'implémentation B1.4). | Intérêt légitime si conservée (§3(c)) / Contrainte légale si supprimée (§3(a)) |
| Logs de corrélation UUID↔email | Logs applicatifs et infra corrélant `UserId` ↔ email d'origine | **Purgés ou anonymisés** dans le périmètre de `UserAnonymized` — voir §5 | — |
| **Contenu de l'espace `PERSONAL`** | Tous les `Document`, `Folder`, `DocumentBlock`, `DocumentLink`, `DocumentTag`, `DocumentType` custom rattachés à l'espace personnel de l'utilisateur | **Hard-delete inconditionnel** déclenché par `UserDeleted`, via la saga `SpaceDeleted` sur l'espace personnel | **Aucune** — aucun tiers, aucun intérêt légitime de continuité Art. 17§3(e) |

---

### 3. Périmètre de l'option Art. 17 — politique de sélection de l'étape 5 de `UserAnonymized`

L'étape 5 de la saga `UserAnonymized` (ADR-011) exécute un effacement étendu sur les documents sélectionnés selon les trois règles ci-dessous, précédé de la déliaison nécessaire (NULL des FK nullable sur les documents concernés, puis DELETE).

**Périmètre d'application de §3(a/b/c)** : ces trois règles s'appliquent au contenu des **espaces partagés** (CAMPAIGN, ONE_SHOT avec membres). Le contenu de l'espace `PERSONAL` relève d'un traitement distinct — hard-delete inconditionnel décrit au tableau §2 — car il est par construction sans tiers. La sélection §3(a/b/c) n'est pas simplifiée par l'introduction de l'espace personnel : ces règles restent nécessaires pour distinguer les sous-catégories de contenu des espaces partagés.

#### (a) Supprimables obligatoirement — contrainte légale dure

Les documents suivants sont supprimés sans exception :

- `documents WHERE created_by_id = userId AND visibility = 'PLAYER_PRIVATE'`
- Les notes liées aux personnages de l'utilisateur : `documents WHERE character_id IN (SELECT character_id FROM membership_characters WHERE user_id = userId) AND created_by_id = userId` — l'ensemble des `character_id` liés à ce `user_id` via `membership_characters`, restreint aux espaces vivants (PK `membership_characters (space_id, user_id, character_id)`). `UserAnonymized` opère cross-espaces vivants ; ne pas restreindre à un seul `character_id` évite d'omettre des personnages dans d'autres espaces.

**Motif** : ces données ont une nature strictement personnelle (notes privées du joueur, non partagées avec le reste de l'espace). Aucun intérêt légitime tiers ne peut justifier leur conservation après la suppression du compte. La conservation constituerait un traitement sans base légale valide.

#### (b) Supprimables obligatoirement — document non partagé

Les documents créés par l'utilisateur (`created_by_id = userId`) qui ne sont visibles que par lui (non partagés avec d'autres membres, non utilisés comme modèle par d'autres documents) sont supprimés.

**Critère opérationnel** : un document est considéré « non partagé » s'il ne figure dans aucun `session_pinned_documents` (C1), aucun `document_links.source_document_id` ou `target_document_id` référencé depuis un document d'un autre utilisateur (C2), et n'a pas été instancié (`source_document_id`) par un autre document (C3).

**Clause d'exclusion — visibilité `GM_ONLY`** : un document `visibility = 'GM_ONLY'` est **exclu de la branche de suppression** du présent critère, quelle que soit la satisfaction de C1/C2/C3. Le test C1/C2/C3 porte sur le partage *effectif* (épinglage, lien, instanciation) mais ne teste jamais la visibilité déclarée du document ; un document `GM_ONLY` non épinglé/non lié/non instancié satisferait C1/C2/C3 et serait à tort sélectionné pour suppression, alors que §3(c) et le tableau §2 (ligne « Documents partagés / `GM_ONLY` ») affirment sa conservation inconditionnelle au titre de l'intérêt légitime tiers (Art. 17§3(e)). Cette clause referme la contradiction : la visibilité `GM_ONLY` prime sur le résultat de C1/C2/C3 et route le document vers §3(c), jamais vers §3(b).

**Instant de référence** : le critère de partage est évalué à `deletion_requested_at` (horodatage de la demande, enregistré conformément à §6), et non au moment de l'exécution effective de la saga. Cette précision prévient un risque de race TOCTOU de conformité : si l'état de partage d'un document évoluait entre la demande et l'exécution (ex. partage retiré par un tiers après la demande), l'évaluation à `deletion_requested_at` garantit la cohérence juridique de la sélection.

**Motif** : sans partage effectif, le document n'a pas d'utilité résiduelle pour des tiers. Sa conservation ne reposerait sur aucune base légale valide.

#### (c) Conservés sous intérêt légitime documenté — contrainte légale Art. 17§3(e)

Les documents créés par l'utilisateur (`created_by_id = userId`) qui sont partagés avec d'autres membres, visibles en `GM_ONLY`, ou utilisés comme modèle par d'autres documents dans l'espace, sont **conservés**.

**Base légale** : Art. 17§3(e) RGPD — intérêt légitime à des fins de continuité pour les membres tiers d'un espace vivant. La suppression de ces documents casserait l'expérience de jeu des tiers (liens brisés, scénarios orphelins, personnages sans fiche).

**Obligations** :
- La politique de confidentialité mentionne explicitement que des contenus créés par un compte supprimé peuvent être conservés dans le contexte d'un espace partagé.
- Le `created_by_id` du document continue de pointer vers le `UserId` anonymisé (valeur `'[Compte supprimé]'` affichée en lieu et place du nom réel).

> **Validation juridique requise** : la qualification de cet intérêt légitime au titre de l'Art. 17§3(e) doit être confirmée par un juriste avant le lancement EU. Voir section *Conformité conçue, non certifiée* en fin d'ADR.

---

### 4. Règle F-08 — anti-résidu sur les notes `PLAYER_PRIVATE`

**Contexte du risque** : un utilisateur peut supprimer son compte alors que son personnage (`character_id`) est toujours associé à un espace vivant. Un nouvel invité peut ultérieurement être réassocié à ce même `character_id` (parcours `CONVERTED` invité→compte puis re-invitation). Si les notes `PLAYER_PRIVATE` du compte supprimé avaient été conservées (dissociation seule, `character_id := NULL`), un accès indirect par une route de lecture non filtrée pourrait exposer ces notes au nouvel invité.

**Règle** : à la suppression du compte, les documents `PLAYER_PRIVATE` créés par l'utilisateur (y compris ceux issus d'un parcours `CONVERTED` invité→compte) sont **supprimés physiquement** (DELETE, précédé de la déliaison nécessaire). La dissociation seule (`character_id := NULL` sur la ligne document) n'est pas suffisante : si la note reste en base avec `character_id = NULL` mais `created_by_id = userId_anonymisé`, une requête portant sur `created_by_id` ou sur les documents visibles d'un `character_id` réassigné (selon implémentation de la couche Application) pourrait encore exposer la donnée.

**Implémentation** : l'étape 5 de `UserAnonymized` doit, pour les documents couverts par §3(a), exécuter :

1. Déliaison des FK nullable sur ces documents (ex. `document_links`, `session_pinned_documents` — NULL si nullable, DELETE si NOT NULL feuille).
2. DELETE des `document_blocks` correspondants.
3. DELETE des documents eux-mêmes.

Ce séquençage est conforme au mécanisme de saga de déliaison-puis-delete défini dans ADR-011.

**Trou nommé — réfs nullable entrantes, étendu à §3(a)** : l'étape 1 ci-dessus ne délie que les FK nullable *portées par* le document supprimé lui-même (`document_links`, `session_pinned_documents`). Elle ne couvre pas les **références nullable entrantes** — des colonnes portées par *d'autres* lignes et pointant vers le document supprimé : `documents.character_id` d'un autre document, `guest_accesses.character_id`, `folders.default_template_document_id`. Ce point est le même angle mort que celui déjà ticketisé pour §3(b) dans **[docs/architecture/specs/requete-effacement-non-partage.md §5](../specs/requete-effacement-non-partage.md)** (« Trou nommé — références nullable entrantes non couvertes par le critère §3(b) », `[À TRANCHER — ticket]`). Ce ticket est étendu explicitement ici : il couvre la classe « réfs nullable entrantes » pour **toute** branche de hard-delete de documents utilisateur, soit §3(a) (présente section) **et** §3(b). Le point n'est pas tranché par le présent ADR ; il relève de l'arbitrage déjà ouvert au spec §5.

---

### 5. Logs de corrélation UUID↔email

La pseudonymisation de l'email dans `asp_net_users` est sans effet si des logs applicatifs ou d'infrastructure (logs d'accès HTTP, logs d'authentification, logs d'audit) conservent des entrées corrélant le `UserId` à l'email d'origine de l'utilisateur.

**Règle** : dans le périmètre de la saga `UserAnonymized`, les entrées de logs corrélant `UserId` ↔ email d'origine sont purgées ou remplacées par la valeur anonymisée (`deleted-{id}@haversack.invalid`). Cette purge doit être planifiée et exécutée avant la fin de la fenêtre de traitement de la demande (Art. 12§3 — voir §6).

**Carve-out explicite — enregistrement de notification Art. 12§3** : l'enregistrement de notification dédié défini en §8 (chemin reprise après incident) est **exclu du périmètre de la présente purge**. Il répond à une finalité distincte (notifier la fin de traitement, et non tracer une corrélation d'accès/audit), à une base légale distincte (obligation légale Art. 6§1(c), non intérêt légitime), et suit sa propre règle de rétention (purge à l'envoi réussi ou en fin de fenêtre Art. 12§3, §8) indépendante du cycle décrit ici. Il ne réintroduit pas la corrélation `UserId` ↔ email visée par la présente règle.

**Backups** : les backups de base de données peuvent contenir des données pré-anonymisation. La politique de rétention des backups doit :
- Appliquer une durée maximale de rétention de **30 jours** (valeur retenue ; à confirmer selon la stratégie de backup P7 multi-instance).
- Documenter que les backups ne sont pas restaurés à des fins non conformes (ex. reconstituer un email supprimé).
- Ces conditions sont admises par le RGPD sous réserve que la durée de rétention soit définie et justifiée. En l'absence de définition, la rétention indéfinie constitue un manquement à l'Art. 5§1(e).

---

### 6. Délai de traitement Art. 12§3

Contrainte légale : toute demande d'effacement reçue au titre de l'Art. 17 doit être traitée dans un délai d'**1 mois** à compter de la réception. Ce délai est extensible à **2 mois** si la complexité de la demande le justifie, à condition d'en informer la personne dans le premier mois.

**Obligations d'implémentation** :
- Horodatage de la demande d'effacement au moment de sa réception (`deletion_requested_at`).
- Exécution de la saga `UserAnonymized` dans le délai imparti.
- Notification de la personne à l'issue du traitement — mécanisme détaillé en §8 (chemin nominal : notification avant réécriture de l'email ; chemin reprise après incident : enregistrement de notification dédié capturé à `deletion_requested_at`).

**Précision suite à ADR-018** : pour les espaces partagés (CAMPAIGN, ONE_SHOT), le comportement MVP est la conservation de l'espace sous l'`id` anonymisé (voir §Conséquences ci-dessous). Pour l'espace `PERSONAL`, la purge est inconditionnelle à `UserDeleted` via la saga `SpaceDeleted` — le délai Art. 12§3 s'applique à l'ensemble de la saga, purge personnelle incluse.

---

### 7. Mécanisme — matérialisation de la sélection à `deletion_requested_at` (appartenance d'espace figée)

**Constat** : ADR-011 définit les sagas `UserAnonymized` (étape 5, effacement étendu §3) et `SpaceDeleted`-PERSONAL (purge inconditionnelle du contenu personnel, §2 dernière ligne) comme des traitements qui, dans leur formulation actuelle, lisent le `space_id` / la `visibility` **courants** du document au moment de l'exécution de la saga. Or le §3(b) impose déjà que le critère de partage soit évalué à `deletion_requested_at`, et le point subsidiaire (b) de la section *Conformité conçue, non certifiée* (point 4) pose la question réciproque : un document déplacé entre espace personnel et espace partagé après la demande doit-il suivre la sélection figée à cet instant ? Faire lire aux sagas le `space_id` courant contredirait une évaluation figée. Le mécanisme ci-dessous referme cette contradiction au niveau technique — il ne tranche pas la question juridique sous-jacente (voir FLAG JURISTE ci-dessous).

**Mécanisme retenu** :

1. **Matérialisation à `deletion_requested_at`** : à l'horodatage de la demande d'effacement (§6), le système matérialise un **jeu de sélection** — un ensemble de lignes `(document_id → action)` où `action ∈ {hard-delete, conserve}` — déterminé par l'appartenance d'espace et la visibilité de chaque document **telles qu'elles sont à cet instant précis**, et non par leur état au moment de l'exécution effective de la saga.
2. **Consommation, pas recalcul** : les deux sagas concernées — `UserAnonymized` étape 5 (documents des espaces partagés, §3(a/b/c)) et `SpaceDeleted`-PERSONAL (purge du contenu de l'espace personnel, §2) — **consomment ce jeu matérialisé** comme source de vérité de leur périmètre d'action respectif. Elles ne recalculent pas la sélection depuis le `space_id` ou la `visibility` courants du document au moment où elles s'exécutent. Ce séquençage referme le risque TOCTOU par construction : un déplacement de document intervenant entre la demande et l'exécution de la saga, dans un sens comme dans l'autre, n'altère pas une sélection déjà figée.
3. **Step de relocation obligatoire** : un document sélectionné `conserve` (§3(c), intérêt légitime tiers) mais physiquement situé, au moment de l'exécution, dans un espace `PERSONAL` voué à la purge inconditionnelle — parce qu'il y a été déplacé après `deletion_requested_at` — constitue un conflit opérationnel : la purge PERSONAL est inconditionnelle et détruirait un document que le jeu matérialisé marque `conserve`. Ce cas est traité par un **step de relocation**, exécuté avant la purge `SpaceDeleted`-PERSONAL : tout document marqué `conserve` dans le jeu matérialisé et physiquement présent dans l'espace personnel au moment de l'exécution est **déplacé hors de cet espace** (vers l'espace partagé d'origine, ou un espace de rattachement défini par la couche Application) avant que la purge ne s'exécute. Ce déplacement ne se limite pas à la réaffectation de l'appartenance d'espace (`space_id`) du document : il **réaffecte aussi les FK scopées à l'espace** portées par le document — a minima `folder_id` (`documents.folder_id` pointe vers un `folders` de l'espace PERSONAL, lui-même voué à suppression, en `ON DELETE RESTRICT`), et, si applicables au document concerné, `guest_access_id` et `source_document_id`. Réaffecter la seule appartenance d'espace sans réassigner ces FK laisserait `folder_id` pointer vers un dossier de l'espace PERSONAL sur le point d'être purgé : la purge `SpaceDeleted`-PERSONAL échouerait alors sur la contrainte `RESTRICT` (le dossier ne pourrait être supprimé tant qu'un document déplacé le référence). Omettre ce step réaffecté laisserait donc, selon le cas, un blocage de purge par FK `RESTRICT`, une référence pendante, ou une purge silencieuse d'un document que la sélection avait marqué à conserver.

> **FLAG JURISTE** : quel instant fait foi *juridiquement* pour la sélection — la date de la **demande** (`deletion_requested_at`) ou celle de l'**exécution** de la saga — dans chacun des deux sens de déplacement (personnel → partagé : le document rejoint le régime hard-delete de §3(a/b) ; partagé → personnel : le document échapperait à la conservation §3(c) pour tomber dans le hard-delete PERSONAL). La suffisance de ce mécanisme au regard de l'Art. 17§3 **doit être validée par un juriste** avant implémentation — voir §Conformité conçue, non certifiée, point 4. Ce point n'est pas tranché ici.

---

### 8. Mécanisme — notification Art. 12§3 (chemin nominal et reprise après incident)

**Constat** : les canaux esquissés initialement (affichage en session, journal d'audit) sont incohérents avec le cas d'échec identifié : un compte déjà anonymisé (email réécrit en `deleted-{id}@haversack.invalid`, §1) n'est plus joignable à son adresse d'origine, et un canal « session » suppose une session active au moment de l'envoi — non garanti si la saga reprend après incident, potentiellement bien après la déconnexion de l'utilisateur.

**Chemin nominal** : la saga `UserAnonymized` est ordonnée de sorte que la **notification de fin de traitement** soit envoyée à l'adresse d'origine **avant** l'étape de réécriture de l'email. L'ordre impératif de la saga (§1) est ainsi complété explicitement : notification à l'adresse d'origine → puis réécriture `password_hash`/`security_stamp` → puis `users.email := 'deleted-{id}@haversack.invalid'`. Cet ordonnancement garantit que l'adresse d'origine est encore valide au moment de l'envoi lorsque la saga s'exécute d'un seul tenant, sans reprise.

**Chemin reprise après incident** : si l'exécution de `UserAnonymized` est interrompue puis reprise après que l'étape de réécriture de l'email a déjà eu lieu, l'adresse d'origine n'est plus disponible en base au moment de la reprise. Pour couvrir ce cas :

- À `deletion_requested_at`, la destination email est capturée dans un **enregistrement de notification dédié** — minimal, à finalité strictement limitée (purpose-limited) : la cible (adresse email), l'identifiant de la demande d'effacement, et un drapeau « effacement terminé » mis à jour par la saga à son achèvement.
- Cet enregistrement repose sur une **base légale distincte** de celle retenue pour la pseudonymisation (§1, intérêt légitime Art. 6§1(f)) : **obligation légale (Art. 6§1(c))**, au titre du respect de l'obligation de notification Art. 12§3 elle-même — base **proposée en conception, à valider juriste**.
- Il est **purgé** dès l'envoi réussi de la notification, ou au plus tard à la fin de la fenêtre de traitement Art. 12§3 (1 ou 2 mois, §6) — selon l'événement survenant en premier.
- Il est **explicitement découpé de la purge des logs de corrélation (§5)** : finalité distincte (notification de fin de traitement vs purge de logs de corrélation applicatifs/infra), rétention distincte, base légale distincte. Cet enregistrement ne doit **pas** réintroduire une corrélation `UserId` ↔ email dans un log persistant au sens de §5 — il est isolé, à finalité unique, purgé selon sa propre règle de rétention indépendamment du cycle de purge des logs de §5.

> **FLAG JURISTE** : (a) l'Art. 12§3 exige-t-il une notification de fin de traitement pour un effacement **auto-initié** par la personne concernée (par opposition à une décision prise par le responsable de traitement) ? (b) la rétention temporaire de la destination email sous base « obligation légale » est-elle compatible avec le principe de minimisation et l'objectif de non-résidu poursuivi par ailleurs (§5) ? Ces deux points **ne sont pas tranchés ici** et requièrent une validation juridique avant implémentation.

---

## Alternatives considérées

**Hard-delete du compte (suppression physique de la ligne `users`)**

Non applicable. Tant que des espaces partagés référencent le `UserId` via des FK NOT NULL (`spaces.owner_id`, `space_memberships.user_id`, etc.), le hard-delete de la ligne `users` déclenche une erreur `RESTRICT` en base. Ce blocage est documenté dans ADR-007 et ADR-009.

**Conservation de tous les documents créés, y compris `PLAYER_PRIVATE`**

Rejetée. Les documents `PLAYER_PRIVATE` ont une nature strictement personnelle. Leur conservation après la suppression du compte ne peut pas être fondée sur l'intérêt légitime (aucun usage tiers démontrable) ni sur l'exécution du contrat (le contrat est rompu). Cette posture exposerait l'éditeur à un traitement sans base légale.

**Suppression de tous les documents créés par l'utilisateur, y compris partagés**

Rejetée. Supprimer l'intégralité des contributions d'un utilisateur détruirait l'expérience de jeu des tiers d'un espace vivant (personnages orphelins, scénarios incomplets). L'Art. 17§3(e) autorise explicitement la conservation lorsque l'intérêt légitime de tiers le justifie.

**Anonymisation du `display_name` uniquement, sans purge des logs**

Rejetée. Une pseudonymisation sans purge des logs de corrélation UUID↔email est factice : un opérateur disposant des logs peut retrouver l'identité réelle. La pseudonymisation doit couvrir l'intégralité des vecteurs de corrélation.

---

## Conséquences

- L'étape 5 de la saga `UserAnonymized` (ADR-011) doit implémenter la politique de sélection §3(a)/(b)/(c) du présent ADR pour le contenu des espaces partagés.
- La saga `SpaceDeleted` doit être déclenchée sur l'espace `PERSONAL` à `UserDeleted` pour en réaliser la purge inconditionnelle.
- La saga `UserAnonymized` inclut la **révocation des tokens actifs** de l'utilisateur (`ITokenDenylist.RevokeFamilyAsync`) comme étape de la saga — conformément à [ADR-015](ADR-015-securite-authentification-mvp.md) §3.6. Cette révocation s'applique également sur `AccountSuspended`.
- La couche Application doit exposer une interface de demande d'effacement horodatée, déclenchant la saga et notifiant l'utilisateur à l'issue.
- La politique de confidentialité doit mentionner : (i) la conservation des documents partagés sous intérêt légitime, (ii) la purge inconditionnelle du contenu de l'espace personnel, (iii) la durée de rétention des backups, (iv) le délai de traitement des demandes.
- La purge des logs de corrélation doit être intégrée au pipeline d'exécution de `UserAnonymized`, pas traitée comme une opération indépendante asynchrone.
- Les `GuestAccess` (données invité) sur un espace vivant ne sont pas traités par `UserAnonymized` — leur base légale, rétention et sort sur demande individuelle sont définis dans **ADR-013**.
- Le cas UC-11 (MJ propriétaire effaçant son compte sur un espace partagé vivant sans désigner de successeur) : comportement MVP = espace partagé (CAMPAIGN, ONE_SHOT) conservé sous l'`id` anonymisé (`owner_id NOT NULL` satisfait) ; transfert de propriété forcé post-MVP. Comportement MVP défini dans ADR-011 (saga `UserAnonymized`) + ADR-013 §6. **Ce comportement ne s'applique pas à l'espace `PERSONAL`** : l'espace personnel ne survit pas sous identité anonymisée — il est purgé inconditionnellement à `UserDeleted` via la saga `SpaceDeleted`. Ce point est une surcharge explicite du défaut UC-11 / ADR-013 §6.

---

## Points à trancher

- **Critère opérationnel exact de « document non partagé »** (§3(b)) : la définition donnée ici est fonctionnelle ; la requête SQL précise est à écrire lors de l'implémentation.
- **Seuil d'extension du délai** (Art. 12§3) : dans quels cas l'extension à 2 mois est-elle invoquée ? À documenter dans la procédure de support.
- **Canal alternatif de notification Art. 12§3** : mécanisme désormais inscrit en §8 (chemin nominal — notification avant réécriture de l'email — et chemin reprise après incident — enregistrement de notification dédié, purpose-limited, base légale obligation légale Art. 6§1(c)). Reste ouvert : la qualification juridique de ce mécanisme (voir FLAG JURISTE §8 et *Conformité conçue, non certifiée* point 4).
- **Appartenance d'espace figée à `deletion_requested_at` — réciproque** : mécanisme désormais inscrit en §7 (matérialisation d'un jeu de sélection à `deletion_requested_at`, consommé sans recalcul par les deux sagas, step de relocation obligatoire). Reste ouvert : quel instant fait foi juridiquement dans chaque sens de déplacement — voir FLAG JURISTE §7 et point 4(b) réciproque dans *Conformité conçue, non certifiée* — à valider juridiquement avant implémentation.

---

## Conformité conçue, non certifiée

Les choix documentés dans cet ADR sont cohérents avec le RGPD tel que lu et interprété lors de la conception. Quatre points requièrent une **validation juridique avant tout lancement EU** :

1. **Qualification Art. 8** : Haversack est-il un service « destiné aux enfants » au sens du RGPD et des droits nationaux applicables ? Si oui, la case d'attestation 16+ au formulaire d'inscription est insuffisante et un mécanisme de vérification de l'âge est requis.
2. **Qualification sous-traitant / responsable F-13 + DPA** : pour les contenus créés par les MJ (données de tiers, personnages de joueurs réels), Haversack agit-il comme responsable de traitement ou comme sous-traitant au sens de l'Art. 28 ? Un Data Processing Agreement (DPA) entre Haversack et les utilisateurs responsables de traitement peut être requis.
3. **Test de mise en balance de l'intérêt légitime (§3(c))** : la conservation des documents partagés au titre de l'Art. 17§3(e) repose sur un intérêt légitime dont la mise en balance formelle (nécessité, proportionnalité, attentes raisonnables de la personne) n'a pas été conduite par un juriste.
4. **Qualification Art. 17 du hard-delete inconditionnel du contenu de l'espace `PERSONAL`** : la thèse hard-delete inconditionnel à `UserDeleted` repose sur l'absence de tiers — donc l'absence d'intérêt légitime Art. 17§3(e). Cette qualification est solide en principe mais doit être confirmée. Points subsidiaires : (a) l'instant de référence `deletion_requested_at` (cohérent avec §3(b)) s'applique-t-il au contenu personnel avec la même rigueur qu'au contenu partagé ? (b) si un document est déplacé d'un espace personnel vers un espace partagé après une demande d'effacement, la sélection est-elle figée à `deletion_requested_at` ou suivie dynamiquement ? — Réciproque de (b) : un document déplacé d'un espace **partagé vers l'espace `PERSONAL`** après `deletion_requested_at` échapperait-il à §3(c) (conservation sous intérêt légitime tiers) pour tomber dans le hard-delete `PERSONAL` ? Le **mécanisme technique** retenu (§7) matérialise l'appartenance d'espace figée à `deletion_requested_at` dans les deux sens (cohérent avec l'instant de référence de §3(b)) sous la forme d'un jeu de sélection consommé sans recalcul par les deux sagas, complété par un step de relocation obligatoire pour les documents `conserve` physiquement présents dans l'espace personnel au moment de la purge. **La suffisance de ce mécanisme au regard de l'Art. 17§3** — quel instant fait juridiquement foi, demande ou exécution, dans chacun des deux sens — n'est pas tranchée ici et requiert une validation juridique avant implémentation.

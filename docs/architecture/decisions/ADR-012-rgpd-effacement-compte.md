# ADR-012 — RGPD : effacement de compte (Art. 17 — droit à l'oubli)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session de cadrage P1)
- **Findings liés** : F-03, F-08, B1.4 (renvoi ADR-011 *Points à trancher*)

---

## Périmètre de cet ADR

Cet ADR couvre exclusivement les traitements déclenchés par la **suppression d'un compte utilisateur** (saga `UserAnonymized`). Il définit la politique RGPD correspondante : procédure d'anonymisation, sort des données par catégorie, périmètre de l'effacement étendu (option Art. 17), règle anti-résidu F-08, purge des logs de corrélation et obligations procédurales Art. 12§3.

**Hors périmètre de cet ADR** :
- Données d'invité (GuestAccess, `display_name`, base légale, information Art. 13) → **ADR-013**
- Purge à la suppression de campagne (saga `CampaignDeleted`, J+30) → **ADR-010 + ADR-011**
- Modèle d'autorisation API → **ADR-007** (B1.3)
- Politique de mot de passe / validation email / JWT → **[ADR-015](ADR-015-securite-authentification-mvp.md)** (résolu)
- Portabilité Art. 20 → post-MVP

---

## Contexte

ADR-007 a acté le principe d'anonymisation par réécriture : l'`UserId` est conservé pour la continuité des campagnes actives, l'email et le `display_name` sont remplacés par des valeurs neutres. ADR-011 a défini la saga `UserAnonymized` et son ordre impératif d'exécution. Ces deux ADR définissent le **mécanisme** ; le présent ADR définit la **politique** : quelles données sont supprimées, conservées, ou purgées, sur quelle base légale, et selon quelle procédure.

Deux questions restaient ouvertes au terme d'ADR-011 :

1. **Périmètre de l'option Art. 17 (effacement étendu)** : quels documents attachés à un compte sont supprimables sur une campagne vivante, et lesquels sont conservés et à quel titre ?
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
5. L'`id` de l'utilisateur est **conservé** : les FK vers `users.id` (`campaigns.owner_id`, `campaign_memberships.user_id`, `folders.created_by_id`, `documents.created_by_id`, `sessions.created_by_id`) restent intactes. L'identifiant survit anonymisé ; les contraintes NOT NULL sont satisfaites.

**Base légale de la pseudonymisation** : intérêt légitime (Art. 6§1(f)) — continuité des campagnes actives pour les membres tiers. Cette posture est documentée dans la politique de confidentialité.

---

### 2. Tableau des données du compte — sort à l'effacement

| Catégorie | Données | Sort à l'effacement | Base légale si conservée |
|---|---|---|---|
| Identité | `email`, `display_name`, `password_hash`, `security_stamp` | **Anonymisés** (réécriture) — voir procédure §1 | — |
| Identifiant | `users.id` (`UserId`) | **Conservé** | Intérêt légitime — continuité campagnes tierces (Art. 6§1(f)) |
| Campagnes possédées | `campaigns.owner_id` (FK vers `users.id`) | **FK intacte**, `owner_id` pointe vers `UserId` anonymisé | Idem — la campagne reste lisible pour les membres actifs |
| Memberships | `campaign_memberships.user_id` | **FK intacte** (membership survit, référence le `UserId` anonymisé) | Idem |
| Documents créés | `documents.created_by_id`, `folders.created_by_id`, `sessions.created_by_id` | **FK intacte** — le contenu survit ; l'auteur est anonymisé | Idem (continuité éditoriale pour les membres tiers) |
| Documents `PLAYER_PRIVATE` créés par l'utilisateur | `documents WHERE created_by_id = userId AND visibility = 'PLAYER_PRIVATE'` | **Supprimés** — voir §3(a) | Contrainte légale : donnée personnelle exclusive, aucun intérêt légitime tiers |
| Documents non partagés | `documents WHERE created_by_id = userId` et non partagés au sens §3(b) | **Supprimés** — voir §3(b) | Contrainte légale : aucun usage tiers démontrable |
| Documents partagés / `GM_ONLY` | Documents contribués à la campagne et visibles ou utilisés par d'autres membres | **Conservés** — voir §3(c) | Intérêt légitime — continuité de campagne pour les tiers (Art. 17§3(e)) |
| Notes liées aux personnages de l'utilisateur | `documents WHERE character_id IN (SELECT character_id FROM membership_characters WHERE user_id = userId) AND created_by_id = userId` (l'ensemble des `character_id` liés à ce `user_id` via `membership_characters`, restreint aux campagnes vivantes) | **Supprimés** avec les `PLAYER_PRIVATE` (§3(a)) | — |
| Notes live de session (`session_live_notes`) | `session_live_notes` sont des documents de type `LIVE_NOTE` — leur sort suit la règle `documents.created_by_id` : anonymisé si conservé (partagé, §3(c)) / supprimé si `PLAYER_PRIVATE` (§3(a)). Si une `session_live_notes` portait un chemin de donnée personnelle hors colonne `document`, ce chemin serait hors tableau (à vérifier au schéma lors de l'implémentation B1.4). | Intérêt légitime si conservée (§3(c)) / Contrainte légale si supprimée (§3(a)) |
| Logs de corrélation UUID↔email | Logs applicatifs et infra corrélant `UserId` ↔ email d'origine | **Purgés ou anonymisés** dans le périmètre de `UserAnonymized` — voir §5 | — |

---

### 3. Périmètre de l'option Art. 17 — politique de sélection de l'étape 5 de `UserAnonymized`

L'étape 5 de la saga `UserAnonymized` (ADR-011) exécute un effacement étendu sur les documents sélectionnés selon les trois règles ci-dessous, précédé de la déliaison nécessaire (NULL des FK nullable sur les documents concernés, puis DELETE).

#### (a) Supprimables obligatoirement — contrainte légale dure

Les documents suivants sont supprimés sans exception :

- `documents WHERE created_by_id = userId AND visibility = 'PLAYER_PRIVATE'`
- Les notes liées aux personnages de l'utilisateur : `documents WHERE character_id IN (SELECT character_id FROM membership_characters WHERE user_id = userId) AND created_by_id = userId` — l'ensemble des `character_id` liés à ce `user_id` via `membership_characters`, restreint aux campagnes vivantes (PK `membership_characters (campaign_id, user_id, character_id)`). `UserAnonymized` opère cross-campagnes vivantes ; ne pas restreindre à un seul `character_id` évite d'omettre des personnages dans d'autres campagnes.

**Motif** : ces données ont une nature strictement personnelle (notes privées du joueur, non partagées avec le reste de la campagne). Aucun intérêt légitime tiers ne peut justifier leur conservation après la suppression du compte. La conservation constituerait un traitement sans base légale valide.

#### (b) Supprimables obligatoirement — document non partagé

Les documents créés par l'utilisateur (`created_by_id = userId`) qui ne sont visibles que par lui (non partagés avec d'autres membres, non utilisés comme modèle par d'autres documents) sont supprimés.

**Critère opérationnel** : un document est considéré « non partagé » s'il ne figure dans aucun `session_pinned_documents`, aucun `document_links.source_document_id` ou `target_document_id` référencé depuis un document d'un autre utilisateur, et n'a pas été instancié (`source_document_id`) par un autre document.

**Instant de référence** : le critère de partage est évalué à `deletion_requested_at` (horodatage de la demande, enregistré conformément à §6), et non au moment de l'exécution effective de la saga. Cette précision prévient un risque de race TOCTOU de conformité : si l'état de partage d'un document évoluait entre la demande et l'exécution (ex. partage retiré par un tiers après la demande), l'évaluation à `deletion_requested_at` garantit la cohérence juridique de la sélection.

**Motif** : sans partage effectif, le document n'a pas d'utilité résiduelle pour des tiers. Sa conservation ne reposerait sur aucune base légale valide.

#### (c) Conservés sous intérêt légitime documenté — contrainte légale Art. 17§3(e)

Les documents créés par l'utilisateur (`created_by_id = userId`) qui sont partagés avec d'autres membres, visibles en `GM_ONLY`, ou utilisés comme modèle par d'autres documents dans la campagne, sont **conservés**.

**Base légale** : Art. 17§3(e) RGPD — intérêt légitime à des fins de continuité pour les membres tiers d'une campagne vivante. La suppression de ces documents casserait l'expérience de jeu des tiers (liens brisés, scénarios orphelins, personnages sans fiche).

**Obligations** :
- La politique de confidentialité mentionne explicitement que des contenus créés par un compte supprimé peuvent être conservés dans le contexte d'une campagne partagée.
- Le `created_by_id` du document continue de pointer vers le `UserId` anonymisé (valeur `'[Compte supprimé]'` affichée en lieu et place du nom réel).

> **Validation juridique requise** : la qualification de cet intérêt légitime au titre de l'Art. 17§3(e) doit être confirmée par un juriste avant le lancement EU. Voir section *Conformité conçue, non certifiée* en fin d'ADR.

---

### 4. Règle F-08 — anti-résidu sur les notes `PLAYER_PRIVATE`

**Contexte du risque** : un utilisateur peut supprimer son compte alors que son personnage (`character_id`) est toujours associé à une campagne vivante. Un nouvel invité peut ultérieurement être réassocié à ce même `character_id` (parcours `CONVERTED` invité→compte puis re-invitation). Si les notes `PLAYER_PRIVATE` du compte supprimé avaient été conservées (dissociation seule, `character_id := NULL`), un accès indirect par une route de lecture non filtrée pourrait exposer ces notes au nouvel invité.

**Règle** : à la suppression du compte, les documents `PLAYER_PRIVATE` créés par l'utilisateur (y compris ceux issus d'un parcours `CONVERTED` invité→compte) sont **supprimés physiquement** (DELETE, précédé de la déliaison nécessaire). La dissociation seule (`character_id := NULL` sur la ligne document) n'est pas suffisante : si la note reste en base avec `character_id = NULL` mais `created_by_id = userId_anonymisé`, une requête portant sur `created_by_id` ou sur les documents visibles d'un `character_id` réassigné (selon implémentation de la couche Application) pourrait encore exposer la donnée.

**Implémentation** : l'étape 5 de `UserAnonymized` doit, pour les documents couverts par §3(a), exécuter :

1. Déliaison des FK nullable sur ces documents (ex. `document_links`, `session_pinned_documents` — NULL si nullable, DELETE si NOT NULL feuille).
2. DELETE des `document_blocks` correspondants.
3. DELETE des documents eux-mêmes.

Ce séquençage est conforme au mécanisme de saga de déliaison-puis-delete défini dans ADR-011.

---

### 5. Logs de corrélation UUID↔email

La pseudonymisation de l'email dans `asp_net_users` est sans effet si des logs applicatifs ou d'infrastructure (logs d'accès HTTP, logs d'authentification, logs d'audit) conservent des entrées corrélant le `UserId` à l'email d'origine de l'utilisateur.

**Règle** : dans le périmètre de la saga `UserAnonymized`, les entrées de logs corrélant `UserId` ↔ email d'origine sont purgées ou remplacées par la valeur anonymisée (`deleted-{id}@haversack.invalid`). Cette purge doit être planifiée et exécutée avant la fin de la fenêtre de traitement de la demande (Art. 12§3 — voir §6).

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
- Notification de la personne à l'issue du traitement (email envoyé à l'adresse d'origine avant anonymisation, ou canal alternatif si l'email n'est plus disponible).

---

## Alternatives considérées

**Hard-delete du compte (suppression physique de la ligne `users`)**

Non applicable. Tant que des campagnes référencent le `UserId` via des FK NOT NULL (`campaigns.owner_id`, `campaign_memberships.user_id`, etc.), le hard-delete de la ligne `users` déclenche une erreur `RESTRICT` en base. Ce blocage est documenté dans ADR-007 et ADR-009.

**Conservation de tous les documents créés, y compris `PLAYER_PRIVATE`**

Rejetée. Les documents `PLAYER_PRIVATE` ont une nature strictement personnelle. Leur conservation après la suppression du compte ne peut pas être fondée sur l'intérêt légitime (aucun usage tiers démontrable) ni sur l'exécution du contrat (le contrat est rompu). Cette posture exposerait l'éditeur à un traitement sans base légale.

**Suppression de tous les documents créés par l'utilisateur, y compris partagés**

Rejetée. Supprimer l'intégralité des contributions d'un utilisateur détruirait l'expérience de jeu des tiers d'une campagne vivante (personnages orphelins, scénarios incomplets). L'Art. 17§3(e) autorise explicitement la conservation lorsque l'intérêt légitime de tiers le justifie.

**Anonymisation du `display_name` uniquement, sans purge des logs**

Rejetée. Une pseudonymisation sans purge des logs de corrélation UUID↔email est factice : un opérateur disposant des logs peut retrouver l'identité réelle. La pseudonymisation doit couvrir l'intégralité des vecteurs de corrélation.

---

## Conséquences

- L'étape 5 de la saga `UserAnonymized` (ADR-011) doit implémenter la politique de sélection §3(a)/(b)/(c) du présent ADR.
- La saga `UserAnonymized` inclut la **révocation des tokens actifs** de l'utilisateur (`ITokenDenylist.RevokeFamilyAsync`) comme étape de la saga — conformément à [ADR-015](ADR-015-securite-authentification-mvp.md) §3.6. Cette révocation s'applique également sur `AccountSuspended`.
- La couche Application doit exposer une interface de demande d'effacement horodatée, déclenchant la saga et notifiant l'utilisateur à l'issue.
- La politique de confidentialité doit mentionner : (i) la conservation des documents partagés sous intérêt légitime, (ii) la durée de rétention des backups, (iii) le délai de traitement des demandes.
- La purge des logs de corrélation doit être intégrée au pipeline d'exécution de `UserAnonymized`, pas traitée comme une opération indépendante asynchrone.
- Les `GuestAccess` (données invité) sur une campagne vivante ne sont pas traités par `UserAnonymized` — leur base légale, rétention et sort sur demande individuelle sont définis dans **ADR-013**.
- Le cas UC-11 (MJ propriétaire effaçant son compte sur une campagne vivante sans désigner de successeur) : comportement MVP = campagne conservée sous l'`id` anonymisé ; transfert de propriété forcé post-MVP. Comportement MVP défini dans ADR-011 (saga `UserAnonymized`) + ADR-013 §6.

---

## Points à trancher

- **Critère opérationnel exact de « document non partagé »** (§3(b)) : la définition donnée ici est fonctionnelle ; la requête SQL précise est à écrire lors de l'implémentation.
- **Seuil d'extension du délai** (Art. 12§3) : dans quels cas l'extension à 2 mois est-elle invoquée ? À documenter dans la procédure de support.
- **Canal alternatif de notification Art. 12§3** : si la saga `UserAnonymized` s'exécute après que l'email de l'utilisateur a été anonymisé (ex. saga reprise après incident, email déjà réécrit en `deleted-{id}@haversack.invalid`), la notification de fin de traitement ne peut plus être envoyée à l'adresse d'origine. Un canal alternatif (ex. affichage en session avant déconnexion, notification enregistrée dans un journal d'audit consultable) doit être défini dans la procédure de support.

---

## Conformité conçue, non certifiée

Les choix documentés dans cet ADR sont cohérents avec le RGPD tel que lu et interprété lors de la conception. Trois points requièrent une **validation juridique avant tout lancement EU** :

1. **Qualification Art. 8** : Haversack est-il un service « destiné aux enfants » au sens du RGPD et des droits nationaux applicables ? Si oui, la case d'attestation 16+ au formulaire d'inscription est insuffisante et un mécanisme de vérification de l'âge est requis.
2. **Qualification sous-traitant / responsable F-13 + DPA** : pour les contenus créés par les MJ (données de tiers, personnages de joueurs réels), Haversack agit-il comme responsable de traitement ou comme sous-traitant au sens de l'Art. 28 ? Un Data Processing Agreement (DPA) entre Haversack et les utilisateurs responsables de traitement peut être requis.
3. **Test de mise en balance de l'intérêt légitime (§3(c))** : la conservation des documents partagés au titre de l'Art. 17§3(e) repose sur un intérêt légitime dont la mise en balance formelle (nécessité, proportionnalité, attentes raisonnables de la personne) n'a pas été conduite par un juriste.

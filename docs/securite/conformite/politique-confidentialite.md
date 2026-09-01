# Politique de confidentialité

> ## ⚠ BROUILLON — À VALIDER JURISTE, NON OPPOSABLE
>
> Ce document est un **projet de politique de confidentialité**, rédigé à partir des décisions de conception déjà actées pour Haversack (ADR, spécifications techniques). Il **n'a pas été validé par un juriste** et **n'est opposable à personne** en l'état. Il ne doit pas être publié ni présenté aux utilisateurs avant relecture et validation par un professionnel du droit.
>
> Chaque point qui relève d'une qualification légale (base légale d'un traitement, durée de conservation non fixée par la conception, transfert hors UE, qualification de responsable ou de sous-traitant) est balisé `[À VALIDER — JURISTE]`. Chaque point que le corpus de conception ne permet pas de renseigner est balisé `[À COMPLÉTER — …]`. Aucune affirmation de ce document ne doit être lue comme une position juridique arrêtée.

---

## 0. Portée de ce document

Haversack est un outil web d'aide à la préparation et à la conduite de parties de jeu de rôle, destiné à un Maître du Jeu (MJ). Il fonctionne selon deux régimes distincts, dont les implications en matière de données personnelles diffèrent fortement l'une de l'autre :

- un **mode local sans compte**, où les données restent dans le navigateur du MJ ;
- un **mode cloud avec compte**, où les données sont synchronisées sur un serveur pour permettre le partage avec les joueurs.

Un joueur peut également rejoindre une partie **sans créer de compte**, par un lien d'invitation temporaire. Cette politique couvre ces trois situations séparément lorsque leurs traitements diffèrent.

---

## 1. Qui traite vos données

**Identité et coordonnées du responsable de traitement** : `[À COMPLÉTER — entité juridique]`. La structure juridique éditrice de Haversack n'est pas encore constituée ou nommée dans le corpus de conception à la date de rédaction de ce brouillon ; cette section doit être complétée dès que l'entité est arrêtée, avant toute publication.

**Contact pour toute question relative à vos données** : `support@haversack.io`. Cette adresse est un **placeholder opérationnel**, retenu en conception (ADR-013 §2) et déjà acté comme canal de saisine pour l'exercice des droits d'un joueur invité (voir [section 7](#7-vos-droits-et-comment-les-exercer)) ; elle reste **à confirmer au provisionnement effectif** du service.

**Délégué à la protection des données (DPO)** : `[À COMPLÉTER]` — la désignation éventuelle d'un DPO, et son caractère obligatoire ou non au regard de la taille et de l'activité de l'éditeur, n'est pas déterminée en conception. `[À VALIDER — JURISTE]`.

---

## 2. Les trois catégories d'utilisateurs et leurs données

Haversack distingue nettement trois populations, dont le traitement de données diffère structurellement.

### 2.1 Le Maître du Jeu avec compte (mode cloud)

Un MJ qui crée un compte pour synchroniser ses données et partager du contenu avec des joueurs voit traiter :

| Donnée | Description | Source |
|---|---|---|
| Adresse de messagerie | Identifiant de connexion, canal de communication (validation, notifications) | ADR-015 §2.1 |
| Mot de passe | Jamais stocké en clair : haché par un algorithme Argon2id (ou repli bcrypt), jamais transmis ni conservé lisible | ADR-015 §1.2 |
| Nom d'affichage (`display_name`) | Affiché aux autres membres d'un espace partagé | `identity-access.md` |
| Jetons d'authentification (accès, rafraîchissement) | Permettent de rester connecté sans ressaisir le mot de passe ; durées bornées (accès ≤ 15 min, rafraîchissement ≤ 7 jours, sans prolongation glissante) | ADR-015 §3.1 |
| Identifiant de fournisseur fédéré (Google, Discord), si liaison OAuth | Utilisé pour authentifier le compte via un fournisseur tiers | ADR-015 §2.2 |
| Contenus créés (documents, notes, dossiers, scénarios) | Rattachés à l'espace personnel du MJ ou aux espaces partagés qu'il possède ou rejoint | `content-library.md` |

### 2.2 Le Maître du Jeu en mode local (sans compte)

**Aucune donnée du MJ en mode local ne quitte son navigateur.** C'est une caractéristique du produit, pas seulement une limitation : Haversack n'a, dans ce mode, **aucune authentification, aucun compte, aucun jeton** — ni JWT, ni jeton de rafraîchissement, ni cookie de session (ADR-017 §4.5). Tant que le MJ n'a pas créé de compte ni déclenché d'export manuel, aucune de ses données de préparation ou de session (campagnes, documents, notes) n'est transmise à un service extérieur (NFR-CONF-02).

Les données sont stockées localement dans le navigateur (IndexedDB). Cette persistance locale **n'est pas chiffrée au repos** — c'est une limitation documentée du produit, assortie d'une information explicite affichée à l'utilisateur (bandeau de confidentialité), en particulier pertinente sur un poste partagé (ADR-017 §4.4).

Le seul moment où des données quittent le navigateur en mode local est une action explicite du MJ : la création d'un compte (migration ponctuelle, avec confirmation préalable) ou un export manuel de ses données. Ces deux actions sont consenties, pas automatiques (NFR-CONF-02).

### 2.3 Le joueur invité (accès sans compte)

Un joueur qui rejoint une partie par un lien d'invitation, sans créer de compte, voit traiter :

| Donnée | Description | Durée retenue en conception | Source |
|---|---|---|---|
| Nom d'affichage (`display_name`) | Saisi librement au moment de rejoindre la session ; sert à l'identifier pendant la partie et dans les contenus partagés | La ligne d'accès est purgée 90 jours après l'expiration de l'accès ; les occurrences du nom dans les contenus de l'espace suivent le cycle de vie de cet espace | ADR-013 §1, §3 |
| Personnage associé (`character_id`), le cas échéant | Rattache l'invité à un personnage joué | Suit le sort de l'accès invité | ADR-013 |
| Notes créées en session (`PLAYER_PRIVATE`) | Notes strictement personnelles créées par l'invité pendant la partie | Suit le sort de l'accès invité et de l'espace | `space-management.md` |
| Adresse IP, horodatages, journaux techniques | Finalité sécurité et débogage | 30 jours maximum après la fin de l'accès | ADR-013 §1 |

L'information relative à ce traitement est affichée **au moment même de la saisie du nom d'affichage**, avant l'entrée en session, avec un lien vers la présente politique (ADR-013 §2) — l'invité ne créant pas de compte, cette information ne peut pas être différée à une page de paramètres.

---

## 3. Finalités des traitements

| Traitement | Finalité | Population concernée |
|---|---|---|
| Compte email/mot de passe et jetons d'authentification | Permettre la connexion et le maintien de session | MJ avec compte |
| Nom d'affichage (MJ ou invité) | Identification pendant la session, attribution des contributions dans l'espace partagé | MJ avec compte, joueur invité |
| Liaison à un fournisseur OAuth (Google, Discord) | Authentification alternative sans mot de passe dédié | MJ avec compte |
| Journaux techniques et adresse IP (invité) | Sécurité du service, détection d'abus, débogage d'incidents | Joueur invité |
| Contenus narratifs (documents, notes, scénarios) | Fourniture du service — préparation et conduite de parties | MJ avec compte, MJ en mode local |
| Mesure d'usage anonyme (instrumentation par pilier) | Mesurer l'occurrence d'un usage (activation préparation / vue de session / partage), **sans capter le contenu narratif créé ou partagé** | Tous, dès le mode local — voir [§8](#8-mesure-daudience-anonyme) |

Le détail des traceurs et de leur durée liés à la mesure d'audience et à l'authentification technique est renvoyé à la [politique de cookies et traceurs](./politique-cookies.md), qui doit être lue en complément de la présente politique.

---

## 4. Base légale de chaque traitement

**Aucune base légale de ce document n'est arrêtée juridiquement.** Le tableau ci-dessous reporte la base légale **pressentie en conception**, lorsque le corpus en propose une, sans la trancher.

| Traitement | Base légale pressentie | Statut |
|---|---|---|
| Nom d'affichage de l'invité (`display_name`) | Intérêt légitime (Art. 6§1(f)) — identification en session, attribution des contributions | `[À VALIDER — JURISTE]` — la mise en balance formelle (nécessité, proportionnalité, attentes raisonnables) n'a pas été conduite par un juriste (ADR-013 §1) |
| Journaux techniques de l'invité (IP, horodatages) | Intérêt légitime (Art. 6§1(f)) — sécurité et débogage | `[À VALIDER — JURISTE]` (ADR-013 §1) |
| Création et gestion du compte MJ (email, mot de passe) | Non qualifiée en conception (probablement exécution du contrat, Art. 6§1(b), ou consentement, selon la structure retenue) | `[À COMPLÉTER]` — le corpus consolidé ne qualifie pas explicitement cette base ; `[À VALIDER — JURISTE]` |
| Pseudonymisation du compte à l'effacement, conservation de l'identifiant technique pour la continuité des espaces partagés | Intérêt légitime (Art. 6§1(f)) — continuité des espaces actifs pour les membres tiers | `[À VALIDER — JURISTE]` (ADR-012 §1) |
| Conservation de documents partagés créés par un compte supprimé (visibles ou utilisés par des tiers) | Intérêt légitime (Art. 17§3(e)) — continuité pour les membres tiers d'un espace vivant | `[À VALIDER — JURISTE]` — mise en balance formelle non conduite (ADR-012 §3(c)) |
| Enregistrement de notification dédié (chemin de reprise après incident, Art. 12§3) | Obligation légale (Art. 6§1(c)) — respect du délai de notification | `[À VALIDER — JURISTE]` — base proposée en conception, non tranchée (ADR-012 §8) |
| Contenus narratifs du MJ décrivant des tiers identifiables (personnages inspirés de joueurs réels) | Qualification de Haversack comme sous-traitant (Art. 28) vis-à-vis du MJ responsable de traitement, posture retenue en conception | `[À VALIDER — JURISTE]` — voir [§6](#6-destinataires-et-sous-traitants) et le [squelette de DPA](./dpa-skeleton.md) (ADR-013 §5) |
| Mesure d'audience anonyme | Non qualifiée en conception — l'exigence porte sur le résultat (« anonyme RGPD »), pas sur le mécanisme ni la base légale | `[À COMPLÉTER]` / `[À VALIDER — JURISTE]` — voir [§8](#8-mesure-daudience-anonyme) |

---

## 5. Durées de conservation

Les durées ci-dessous sont celles que la conception technique fixe explicitement. Toute durée non fixée par le corpus est balisée `[À COMPLÉTER]` — elle ne doit pas être déduite ou estimée par extrapolation.

| Donnée | Durée retenue | Source |
|---|---|---|
| Accès invité (`GuestAccess`) expiré | Purge physique 90 jours après expiration, indépendamment du cycle de vie de l'espace | ADR-013 §3 |
| Journaux techniques de l'invité (IP, horodatages) | 30 jours maximum après la fin de l'accès | ADR-013 §1, §3 |
| Notes strictement personnelles (`PLAYER_PRIVATE`) d'un compte supprimé | Suppression physique immédiate dans le cadre de la suppression du compte, sans exception | ADR-012 §3(a) |
| Documents créés par un compte supprimé, non partagés | Suppression physique dans le cadre de la suppression du compte | ADR-012 §3(b) |
| Documents créés par un compte supprimé, partagés ou utilisés par des tiers (visibilité `GM_ONLY` incluse) | Conservés tant que l'espace partagé existe, sous l'identifiant anonymisé — durée non bornée dans le temps par la conception au-delà de la vie de l'espace | ADR-012 §2, §3(c) |
| Contenu de l'espace personnel d'un compte supprimé | Suppression physique inconditionnelle, déclenchée par la suppression du compte | ADR-012 tableau §2, ADR-018 |
| Contenu d'un espace partagé après sa suppression | Purge définitive J+30 après la demande de suppression de l'espace | ADR-011 (renvoi) |
| Journaux de corrélation identifiant technique ↔ email d'origine | Purgés ou anonymisés dans le cadre de la suppression du compte | ADR-012 §5 |
| Sauvegardes de base de données (backups) | Durée maximale retenue de 30 jours — valeur à confirmer selon la stratégie de sauvegarde définitive | ADR-012 §5 |
| Enregistrement de notification dédié (chemin de reprise après incident, Art. 12§3) | Purgé dès l'envoi réussi de la notification, ou au plus tard à la fin du délai de traitement (1 à 2 mois) | ADR-012 §8 |
| Compte utilisateur lui-même (email, mot de passe, contenus) | Conservé tant que le compte n'a pas fait l'objet d'une demande de suppression | — |
| Toute autre donnée non listée ci-dessus (registre de traitements, données de facturation le cas échéant, etc.) | `[À COMPLÉTER]` | — |

---

## 6. Destinataires et sous-traitants

| Destinataire | Rôle | Statut |
|---|---|---|
| Fournisseurs d'authentification fédérée — Google, Discord | Authentification alternative (OAuth) si le MJ choisit de lier son compte | Retenus pour le MVP (ADR-015 §2.2) ; le partage de données avec ces fournisseurs se limite à la vérification d'identité au moment de la connexion |
| Hébergeur de l'infrastructure cloud | Hébergement du serveur, de la base de données | `[non tranché]` — aucun hébergeur n'est arrêté à ce jour dans le corpus d'architecture (`docs/deploiement/README.md`, `docs/context/dossier-architecture.md`) |
| Service de messagerie transactionnelle (email support, notifications) | Envoi d'emails (validation de compte, notifications de traitement de demandes) | Pressenti en conception à titre indicatif, non contractuel — `[À COMPLÉTER]` (squelette de DPA, §5) |
| CDN (réseau de diffusion de contenu), le cas échéant | Diffusion des ressources statiques de l'application | Pressenti à titre indicatif, non contractuel — `[À COMPLÉTER]` |

**Qualification sous-traitant / responsable de traitement pour les contenus créés par le MJ** : lorsqu'un MJ saisit des contenus décrivant des personnages inspirés de joueurs réels (données de tiers identifiables), la conception retient par défaut la posture « Haversack agit comme sous-traitant » (Art. 28) vis-à-vis du MJ, responsable de traitement pour ces contenus. Cette qualification est `[À VALIDER — JURISTE]` (ADR-013 §5). Un squelette de sections destiné à guider la rédaction d'un Data Processing Agreement (DPA) existe : [`dpa-skeleton.md`](./dpa-skeleton.md) — il ne tranche aucune clause. Le périmètre exact de cette qualification (espaces partagés seulement, ou également le contenu de l'espace personnel décrivant un tiers identifiable) est lui-même `[À VALIDER — JURISTE]` (ADR-018).

---

## 7. Vos droits et comment les exercer

Toute personne dont les données sont traitées par Haversack dispose des droits suivants, dans les conditions et limites du RGPD :

| Droit | Article | Applicable à |
|---|---|---|
| Accès | Art. 15 | MJ avec compte, joueur invité |
| Rectification | Art. 16 | MJ avec compte, joueur invité |
| Effacement | Art. 17 | MJ avec compte, joueur invité — voir [§9](#9-effacement--ce-qui-se-passe-réellement) |
| Opposition | Art. 21 | Applicable dès lors qu'un traitement repose sur l'intérêt légitime (nom d'affichage, journaux techniques de l'invité, conservation de documents partagés) |
| Portabilité | Art. 20 | **Non implémentée à ce stade** — différée après la première version du produit (ADR-012, hors périmètre). Une fonctionnalité d'export d'un espace existe par ailleurs (format de sérialisation versionné réutilisable, cahier des charges §7.4) ; elle est distincte de l'exercice formel du droit à la portabilité |
| Limitation du traitement | Art. 18 | Non instruite par la procédure actuelle — applicabilité `[À VALIDER — JURISTE]` |

**Pour un Maître du Jeu avec compte** : les demandes s'exercent via le contact indiqué en [§1](#1-qui-traite-vos-données). La suppression du compte suit la procédure décrite en [§9](#9-effacement--ce-qui-se-passe-réellement).

**Pour un joueur invité sans compte** : une procédure dédiée existe, décrite dans [`procedure-droits-invites.md`](./procedure-droits-invites.md). En résumé :
- la demande s'adresse par email à `support@haversack.io`, en mentionnant le nom d'affichage utilisé en session et l'espace concerné ;
- une vérification d'identité proportionnée est effectuée avant toute action (la méthode exacte n'est pas encore arrêtée — `[À VALIDER — JURISTE/SÉCURITÉ]`) ;
- le délai de traitement est d'1 mois, extensible à 2 mois selon la complexité, avec information du demandeur dans le premier mois (Art. 12§3) ;
- la portée exacte de l'effacement ou de l'opposition lorsque la donnée de l'invité est imbriquée dans un contenu partagé conservé sous intérêt légitime tiers reste `[À VALIDER — JURISTE]`.

**Réclamation auprès d'une autorité de contrôle** : toute personne peut introduire une réclamation auprès de l'autorité de protection des données compétente (en France, la CNIL). `[À COMPLÉTER]` — mention à finaliser selon le pays de résidence de l'utilisateur et l'entité juridique retenue en [§1](#1-qui-traite-vos-données).

---

## 8. Mesure d'audience anonyme

Une instrumentation existe pour mesurer l'occurrence de trois usages du produit — activation préparation, activation vue de session, activation partage — **sans capter le contenu narratif** créé ou partagé par le MJ, y compris en mode local (cahier des charges §7.4 ; NFR-CONF-02). Cette mesure est décrite en détail, avec ses points laissés ouverts, dans la [politique de cookies et traceurs](./politique-cookies.md#4-mesure-daudience--instrumentation-par-pilier), qu'il convient de lire en complément de la présente section.

Une capture d'adresse email **non bloquante** est également prévue dès le mode local (le MJ n'est pas empêché de progresser s'il ne la fournit pas) ; ni le mécanisme exact de sollicitation ni le traitement de cette adresse ne sont fixés par la conception à ce jour — `[À COMPLÉTER]` (spécification télémétrie §4).

---

## 9. Effacement — ce qui se passe réellement

Lorsqu'un utilisateur avec compte demande la suppression de son compte, la procédure suivante est déclenchée (résumé fidèle à ADR-011, ADR-012 ; description complète du mécanisme dans ces ADR) :

1. **Anonymisation du compte** : l'adresse de messagerie et le nom d'affichage sont réécrits par des valeurs neutres ; le mot de passe est neutralisé. L'identifiant technique du compte est **conservé** pour assurer la continuité des espaces partagés auxquels l'utilisateur appartenait.
2. **Suppression physique et inconditionnelle du contenu de l'espace personnel** de l'utilisateur (documents, dossiers, notes qui n'appartiennent qu'à lui) — aucune exception, aucun tiers n'y ayant accès par construction.
3. **Suppression physique des notes strictement personnelles** (`PLAYER_PRIVATE`) que l'utilisateur avait créées dans des espaces partagés, ainsi que des documents qu'il avait créés sans les avoir partagés.
4. **Conservation, sous l'identité anonymisée**, des documents que l'utilisateur avait créés dans un espace partagé et qui sont visibles ou utilisés par d'autres membres (y compris en visibilité réservée au meneur de jeu) — pour préserver l'expérience de jeu de ces tiers. Cette conservation repose sur l'intérêt légitime des membres tiers ; sa qualification juridique précise reste `[À VALIDER — JURISTE]` (voir [§4](#4-base-légale-de-chaque-traitement)).
5. **Révocation immédiate** de tous les jetons d'authentification actifs du compte.
6. **Purge des journaux techniques** corrélant l'identifiant du compte à son adresse de messagerie d'origine.
7. **Notification** de la fin du traitement à l'adresse de messagerie utilisée au moment de la demande — ou, si cette adresse n'est plus joignable après une reprise suite à incident, via un enregistrement de notification dédié capturé au moment de la demande et purgé après envoi (ADR-012 §8).

Le délai de traitement de cette demande est d'1 mois, extensible à 2 mois selon la complexité, avec information de l'utilisateur dans le premier mois (Art. 12§3).

**Un cas particulier** : si le Maître du Jeu propriétaire d'un espace partagé actif supprime son compte sans avoir désigné de successeur, l'espace est conservé sous l'identité anonymisée du propriétaire d'origine — aucun transfert de propriété automatique n'est effectué à ce stade (ADR-012, ADR-013 §6). Un mécanisme de transfert de propriété est envisagé pour une version ultérieure du produit.

Pour un **joueur invité sans compte**, l'accès et les données qui lui sont propres sont purgés automatiquement 90 jours après l'expiration de l'accès (30 jours pour les journaux techniques), indépendamment de toute demande, ou sur demande individuelle selon la [procédure dédiée](./procedure-droits-invites.md).

---

## 10. Transferts de données hors Union européenne

`[À VALIDER — JURISTE]` / `[À COMPLÉTER]` — l'hébergeur de l'infrastructure n'étant pas encore arrêté ([§6](#6-destinataires-et-sous-traitants)), la localisation effective des données et l'existence éventuelle d'un transfert hors de l'Union européenne ne peuvent pas être qualifiées à ce stade. Les fournisseurs d'authentification fédérée retenus (Google, Discord) peuvent impliquer un traitement de données hors UE selon leurs propres politiques — ce point doit être vérifié et, le cas échéant, encadré par les garanties appropriées (clauses contractuelles types ou équivalent) avant tout lancement dans l'Union européenne. Aucune garantie de transfert n'est actée dans le corpus de conception.

---

## 11. Mineurs

Haversack n'est pas conçu comme un service destiné aux enfants. Une case d'attestation d'âge (« j'atteste avoir au moins 16 ans ») est présentée à la fois au formulaire de création de compte et au formulaire de saisie du nom d'affichage pour un joueur invité. **La qualification du service au regard de l'article 8 du RGPD et des droits nationaux applicables — et donc la suffisance de cette simple attestation — n'est pas tranchée** et doit être validée par un juriste avant tout lancement dans l'Union européenne (ADR-012 §Conformité conçue, non certifiée ; ADR-013 §4). `[À VALIDER — JURISTE]`.

---

## 12. Modifications de cette politique

`[À COMPLÉTER]` — le mécanisme d'information des utilisateurs en cas de modification de cette politique (notification, date de mise à jour affichée, nouvelle acceptation requise ou non) n'est pas défini dans le corpus de conception consulté.

---

## Traçabilité — sources de ce document

Chaque affirmation technique de ce document est tirée d'une des sources suivantes. Aucune finalité, durée, ou catégorie de donnée n'est introduite sans ancrage dans l'une d'elles.

| Source | Contenu utilisé |
|---|---|
| `docs/architecture/decisions/ADR-012-rgpd-effacement-compte.md` | Procédure d'anonymisation, tableau des données du compte, périmètre de l'effacement étendu, purge des logs de corrélation, délai Art. 12§3, mécanisme de notification, points de validation juridique |
| `docs/architecture/decisions/ADR-013-rgpd-donnees-invites.md` | Base légale du `display_name` invité, information Art. 13, rétention autonome des accès invités, posture mineurs, posture sous-traitant |
| `docs/architecture/decisions/ADR-015-securite-authentification-mvp.md` | Catégories de données du compte cloud, cycle de vie des jetons, fournisseurs OAuth retenus, périmètre exclusivement cloud de l'authentification |
| `docs/architecture/decisions/ADR-017-modele-indexeddb-local.md` | Absence de compte, de jeton et de cookie en mode local ; absence de chiffrement au repos d'IndexedDB |
| `docs/architecture/decisions/ADR-018-espace-personnel-generalisation-space.md` | Purge inconditionnelle de l'espace `PERSONAL`, extension du périmètre DPA |
| `docs/architecture/specs/telemetrie.md` | Piliers d'activation, capture email non bloquante, principe de mesure d'occurrence sans contenu narratif |
| `docs/architecture/specs/requete-effacement-non-partage.md` | Critère opérationnel de document « non partagé », instant de référence de la sélection |
| `docs/context/cahier-des-charges.md` §7.4 | Synthèse de la posture RGPD, export d'espace |
| `docs/conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md` | Portée de l'isolation des données en mode local |
| `docs/conception/besoin/nfr/NFR-CONF-03-suppression-effective-donnees-personnelles.md` | Droit à l'effacement, confirmation de suppression |
| `docs/securite/conformite/cadrage-validation-pre-lancement-eu.md` | Cinq axes de validation juridique consolidés |
| `docs/securite/conformite/procedure-droits-invites.md` | Procédure d'exercice des droits d'un invité |
| `docs/securite/conformite/dpa-skeleton.md` | Squelette de DPA, sous-traitants ultérieurs pressentis |
| `docs/securite/dossier-securite.md` | Consolidation des actifs sensibles et contre-mesures RGPD |
| `docs/deploiement/README.md`, `docs/context/dossier-architecture.md` | Statut `[non tranché]` de l'hébergeur |

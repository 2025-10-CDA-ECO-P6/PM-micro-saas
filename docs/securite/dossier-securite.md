# Dossier de sécurité — Haversack

## Statut du document

**Nature.** Ce dossier consolide, dans un format unique destiné à une lecture externe (RSSI, auditeur, conseil juridique, équipe de construction), la posture de sécurité issue du corpus de conception d'Haversack — décisions d'architecture (ADR), spécifications fonctionnelles et non fonctionnelles, contrats d'API. **Ce dossier ne crée aucune décision de sécurité** : il consolide, structure et met en relation des décisions actées ailleurs. Chaque contre-mesure, chaque menace et chaque point ouvert porte l'ancre exacte du document source qui l'établit.

**Portée temporelle.** Ce document est une consolidation pré-construction (« pré-build »). Il devra être confirmé, complété et daté à l'entrée en phase de construction, en particulier sur les points marqués `[À TRANCHER]` et sur les validations juridiques identifiées en section 5.3.

**Portée fonctionnelle.** Haversack est une plateforme de jeu de rôle narratif multi-joueurs. Le présent dossier couvre l'ensemble des mécanismes de sécurité applicative et de conformité RGPD documentés à ce stade : authentification, autorisation, canal temps réel, stockage local, migration, effacement de données et télémétrie. Il ne couvre pas les aspects d'infrastructure (hébergement, réseau, durcissement système) qui ne font pas partie du corpus consolidé ici.

---

## 1. Périmètre et actifs sensibles

### 1.1 Deux régimes d'exécution distincts

Haversack fonctionne selon deux régimes d'exécution dont la frontière de sécurité est structurante :

- **Régime local** : exécution dans le navigateur, persistance IndexedDB, **sans compte, sans entité `User`, sans jeton** (JWT ou autre). Aucune authentification n'existe dans ce régime (`identity-access.md`, agrégat / périmètre ; ADR-017 §4.5). La protection repose exclusivement sur la sanitisation côté client, la politique CSP et une information explicite de l'utilisateur (bandeaux), **sans chiffrement au repos**.
- **Régime cloud** : serveur autoritaire, authentification, autorisation par jeton, base de données partagée (ADR-015).

Le seul point de croisement entre les deux régimes est une **migration ponctuelle (« one-shot »)** du local vers le cloud (ADR-016) : il ne s'agit pas d'une synchronisation continue, ce qui borne strictement la surface d'attaque de la frontière de confiance à un seul mécanisme d'import.

### 1.2 Périmètre de sécurité

Le régime local n'a **aucune authentification** (ADR-015 §Périmètre) : les mécanismes d'authentification, d'autorisation et de gestion de jeton documentés dans ce dossier portent exclusivement sur le régime cloud. Le régime local est protégé par un ensemble distinct de mesures (sanitisation, CSP, information utilisateur) décrites en section 4.4.

### 1.3 Actifs sensibles

| Actif | Description | Ancre |
|---|---|---|
| Comptes utilisateurs | Email, hachage de mot de passe, `security_stamp`, jetons d'accès et de rafraîchissement | `identity-access.md` (agrégat `User`) ; ADR-015 |
| Données joueur | Documents `PLAYER_PRIVATE`, personnages, notes | `content-library` / `session-conduct.md` |
| Données invité | `GuestAccess.display_name`, `character_id`, adresse IP et journaux techniques | `space-management.md` (agrégat `GuestAccess`) ; ADR-013 |
| Visibilité et partage | Niveaux `PUBLIC` / `GM_ONLY` / `PLAYER_PRIVATE` | ADR-014 |
| Canal temps réel | Flux SignalR entre le meneur de jeu et les joueurs invités | ADR-004 |
| Contenu narratif décrivant des tiers | Contenu pouvant qualifier Haversack de sous-traitant au sens de l'article 28 du RGPD | ADR-013 §5 |

---

## 2. Surface d'attaque

Neuf surfaces d'attaque sont identifiées dans le corpus de conception.

| Surface | Description | Ancres |
|---|---|---|
| **S1** — Authentification cloud | Identifiants, jetons JWT / rafraîchissement, réinitialisation de mot de passe | ADR-015 ; `config-securite-migration.md` |
| **S2** — Liaison OAuth / fournisseurs fédérés | Google, Discord ; risque de préemption de compte (« pre-account-hijacking ») | ADR-015 §2 |
| **S3** — API / autorisation | Référence directe non protégée (IDOR), appartenance ressource ↔ espace, fuite par lien retour | ADR-014 ; `contrat-openapi.md` |
| **S4** — Canal temps réel invité | Anti-fuite SignalR, révocation en session | ADR-004 §Compléments post-revue ; ADR-014 §3 / §6 ; `session-conduct.md` (invariant 4, garde-fou n°3) |
| **S5** — Stockage local | IndexedDB, XSS persistant, absence de chiffrement au repos, poste partagé | ADR-017 §4 ; `sanitisation-csp.md` |
| **S6** — Migration et import local → cloud | Frontière de confiance, appropriation de poste partagé, revalidation | ADR-016 §2 / §3 / §4 |
| **S7** — Import JSON local | Fichier externe non fiable importé en régime local | ADR-017 §4.3 ; `sanitisation-csp.md` §2 |
| **S8** — Télémétrie / analytique | Minimisation, non-captation du contenu narratif | `telemetrie.md` ; ADR-006 §Périmètre MVP ; NFR-CONF-02 |
| **S9** — Effacement et cascade RGPD | Résidus de données, accès résiduel, intégrité référentielle | ADR-011 ; ADR-012 ; ADR-013 ; `requete-effacement-non-partage.md` |

---

## 3. Modèle de menaces (STRIDE) par surface

Cette section ne présente, par surface, que les catégories STRIDE effectivement couvertes par une contre-mesure documentée dans le corpus. Une catégorie non traitée positivement par une mesure identifiée est signalée comme telle et renvoyée à la section 5.

### Matrice de synthèse — surface × STRIDE

| Surface | S | T | R | I | D | E |
|---|---|---|---|---|---|---|
| S1 — Authentification cloud | ✓ (→ #1, #4, #10) | ✓ (→ #4, #11) | lacune (→ L2) | ✓ (→ #5) | ✓ (→ #9, #10) | ✓ (→ #6, #42) |
| S2 — Liaison OAuth | ✓ (→ #12, #13, #14) | — | lacune (→ L2) | ✓ (→ #15) | — | ✓ (→ #16, non ratifié) |
| S3 — API / autorisation | — | ✓ (→ #17) | lacune (→ L2) | ✓ (→ #20, #21, #22, #24) | — | ✓ (→ #17, #23) |
| S4 — Canal temps réel invité | ✓ (→ #25) | — | lacune (→ L2) | ✓ (→ #19, #20) | — | ✓ (→ #26) |
| S5 — Stockage local | — | ✓ (→ #27, #30) | lacune (→ L2) | ✓ (→ #30, #36) | — | — |
| S6 — Migration et import local → cloud | ✓ (→ #33) | ✓ (→ #28, #31, #32, #34) | lacune (→ L2) | — | — | — |
| S7 — Import JSON local | — | ✓ (→ #29) | lacune (→ L2) | — | — | — |
| S8 — Télémétrie / analytique | — | — | lacune (→ L2) | ✓ (→ #45) | — | — |
| S9 — Effacement et cascade RGPD | — | ✓ (→ #37, #38, #43) | lacune (→ L2) | ✓ (→ #39, #41, #42) | — | — |

Les cases « — » signifient hors-périmètre du corpus consolidé pour cette surface, non une absence de risque non évaluée.

### S1 — Authentification cloud

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Spoofing (usurpation) | Politique de mot de passe (longueur minimale, complexité désactivée), signature JWT asymétrique, refus explicite de l'algorithme `none` | ADR-015 §1 / §3.2 |
| Tampering (altération) | Refus de l'algorithme `none`, validation par `ITokenValidator` | ADR-015 §3.2 / §5 |
| Information disclosure | Gestion de clé hors dépôt, via gestionnaire de secrets | ADR-015 §3.2 |
| Denial of Service | Limitation de débit hybride (par IP et par compte) | ADR-015 §4 |
| Elevation of Privilege | Rotation des jetons de rafraîchissement par famille, détection de réutilisation → révocation de la famille ; révocation sur suspension de compte ou anonymisation | ADR-015 §3.3 / §3.6 |
| Repudiation | Non traitée positivement par une mesure de traçabilité dédiée — voir la lacune signalée L2 (section 5.5) | — |

### S2 — Liaison OAuth / fournisseurs fédérés

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Spoofing | Verrou anti-préemption de compte : liaison uniquement si l'email préexistant est prouvé vérifié, avec correspondance canonique ; preuve d'identité alternative exigée pour la définition d'un premier mot de passe sur un compte fédéré uniquement | ADR-015 §2.3 ; §1.4 |
| Information disclosure | Rejet silencieux de la liaison, non-révélation de l'existence d'un compte | ADR-015 §2.3 ; `contrat-openapi.md` §6 |
| Elevation of Privilege | Mécanisme de reprise en place (« reclaim-in-place »), **statut non ratifié** — voir section 5.2 | ADR-015 §2.3 |

### S3 — API / autorisation

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Tampering / Elevation of Privilege | Verrou d'appartenance non contournable (`IResourceAccessPolicy`) intégré au pipeline de traitement des requêtes ; suppression en `RESTRICT` pilotée par saga applicative, jamais de cascade SQL silencieuse | ADR-014 §3 ; ADR-011 §1 |
| Information disclosure | Non-révélation d'existence (réponses 403 / 404 différenciées) ; exclusion silencieuse des liens retour non lisibles ; filtres de requête appliqués systématiquement | ADR-014 §Conséquences, §Fuite d'existence par backlink, §2 ; `contrat-openapi.md` §1-4 |
| Elevation of Privilege | Verrou anti-saut de privilège invité → utilisateur enregistré | ADR-014 §1 / §5 |

### S4 — Canal temps réel invité

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Information disclosure | Filtrage par message via la même politique d'accès aux ressources que l'API ; aucune diffusion de groupe indifférenciée | ADR-004 §Compléments post-revue ; ADR-014 §6 |
| Spoofing | Jeton d'accès invité transmis hors chaîne de requête (« query-string ») | ADR-004 §Compléments post-revue |
| Elevation of Privilege (révocation) | Déconnexion forcée du canal sur révocation ou expiration de l'accès invité | ADR-004 §Compléments post-revue ; `session-conduct.md` |

### S5 — Stockage local

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Tampering (XSS persistant) | Sanitisation côté client par liste blanche positive, appliquée avant injection dans le DOM et avant persistance ; seconde couche : politique CSP stricte comme défense en profondeur anti-XSS | ADR-017 §4.1 / §4.2 ; `sanitisation-csp.md` §1 / §3 |
| Information disclosure | Politique CSP stricte ; absence de chiffrement au repos assumée comme limitation documentée | ADR-017 §4.2 / §4.4 |

Sanitisation applicative (#27) et CSP (#30) forment une défense en profondeur anti-XSS à deux couches complémentaires, non substituables l'une à l'autre. Ancres : ADR-017 §4.2 ; `sanitisation-csp.md` §3 / §4.

### S6 — Migration et import local → cloud

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Spoofing (appropriation) | Verrou de confirmation explicite, drapeau serveur `confirmed:true` | ADR-016 §4 |
| Tampering (frontière de confiance) | Champs d'autorité jamais honorés depuis la charge utile importée, identifiants régénérés côté serveur ; revalidation par les objets-valeur du domaine | ADR-016 §2.2 / §2.3 |
| Tampering (XSS persistant) | Sanitisation côté serveur à l'import | ADR-016 §2.4 |
| Intégrité transactionnelle | Transaction par espace, exécution à blanc préalable, rapport de rejets, idempotence par identifiant de lot de migration | ADR-016 §3 |

### S7 — Import JSON local

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Tampering / XSS | Ordre impératif : validation structurelle puis sanitisation, avant toute écriture en base locale | ADR-017 §4.3 ; `sanitisation-csp.md` §2 |

### S8 — Télémétrie / analytique

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Information disclosure | Minimisation : mesure d'occurrence sans captation du contenu narratif | NFR-CONF-02 ; `telemetrie.md` §5 |

### S9 — Effacement et cascade RGPD

| Catégorie STRIDE | Traitement | Ancre |
|---|---|---|
| Information disclosure (résidu) | Suppression physique définitive des documents `PLAYER_PRIVATE` ; purge des journaux de corrélation identifiant technique ↔ email ; révocation des jetons actifs au sein de la saga d'anonymisation | ADR-012 §3(a) / §4 / §5 ; ADR-015 §3.6 |
| Intégrité référentielle | Saga applicative en ordre topologique, mécanisme de réservation (« claim ») et reprise idempotents | ADR-011 |
| Tampering (cohérence temporelle / TOCTOU) | Figement de la sélection des données à l'instant de la demande d'effacement, jeu de données matérialisé, étape de relocation obligatoire | ADR-012 §7 ; `requete-effacement-non-partage.md` §2 |

---

## 4. Contre-mesures

Les quarante-cinq contre-mesures documentées dans le corpus sont regroupées ci-dessous par thème. Chaque ligne indique la surface et la catégorie de menace adressée, ainsi que l'ancre exacte de la décision source. Les valeurs numériques précises non tranchées par le corpus sont reportées à la section 5.1 et ne sont pas reproduites ici.

### 4.1 Authentification et gestion des identifiants (surface S1)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 1 | Politique de mot de passe : longueur minimale de 12 caractères, exigence de complexité désactivée explicitement | S1 / Spoofing | ADR-015 §1.1 ; `config-securite-migration.md` |
| 2 | Hachage Argon2id, avec repli sur bcrypt (coût ≥ 12), via un port `IPasswordHasher<User>` | S1 / Spoofing | ADR-015 §1.2 |
| 3 | Cycle de vie des jetons : accès ≤ 15 minutes, rafraîchissement ≤ 7 jours en borne absolue (non glissante) | S1 / Elevation of Privilege | ADR-015 §3.1 |
| 4 | Signature JWT asymétrique (RS256 / ES256) ; interdiction explicite de l'algorithme `none` dans les paramètres de validation | S1 / Tampering | ADR-015 §3.2 |
| 5 | Rotation de la clé de signature sans redéploiement ; clé jamais versionnée, gestion via gestionnaire de secrets obligatoire | S1 / Information disclosure | ADR-015 §3.2 |
| 6 | Rotation des jetons de rafraîchissement par famille, avec détection de réutilisation entraînant la révocation de la famille entière | S1 / Elevation of Privilege | ADR-015 §3.3 |
| 7 | Liste de révocation de jetons (JTI) externe et partagée : conséquence de la topologie scale-out multi-instances déjà actée par ADR-004 (sticky sessions) — une conservation en mémoire mono-instance y est incompatible, pas seulement en cas de montée en charge future | S1 / Elevation of Privilege | ADR-015 §3.4 ; ADR-004 §Compléments post-revue |
| 8 | Purge périodique des identifiants de jeton (JTI) expirés, tâche idempotente | S1 / hygiène opérationnelle | ADR-015 §3.5 |
| 9 | Limitation de débit hybride, par adresse IP et par compte, seuils indépendants et cumulatifs, borne supérieure de sécurité obligatoire, sur les points d'entrée de connexion, rafraîchissement, validation de jeton et réinitialisation de mot de passe | S1 / Denial of Service | ADR-015 §4.1 / §4.2 ; `contrat-openapi.md` §5 |
| 10 | Jeton de réinitialisation de mot de passe : durée de vie ≤ 15 minutes, usage unique, invalidation des jetons antérieurs | S1 / Denial of Service, Spoofing | ADR-015 §4.3 |
| 11 | Contrats applicatifs observables (`ITokenDenylist`, `ITokenValidator`, `IEmailVerificationPolicy`, `ITokenSigner`) vérifiables par test d'architecture | S1 / non-régression structurelle | ADR-015 §5 |

### 4.2 Liaison OAuth (surface S2)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 12 | Verrou anti-préemption de compte : liaison OAuth autorisée uniquement si l'email préexistant est prouvé vérifié, avec correspondance canonique | S2 / Spoofing | ADR-015 §2.3 |
| 13 | Confiance accordée par fournisseur, conditionnée à la déclaration de vérification d'email (`email_verified` Google, `verified` Discord) et à la correspondance canonique | S2 / Spoofing | ADR-015 §2.2 |
| 14 | Preuve d'identité alternative (ré-authentification récente auprès du fournisseur d'identité) exigée pour la définition d'un premier mot de passe sur un compte fédéré uniquement | S2 / Elevation of Privilege | ADR-015 §1.4 ; `identity-access.md` (règle métier n°6) |
| 15 | Rejet silencieux d'une liaison déjà existante, sans révélation d'existence | S2 / Information disclosure | ADR-015 §2.3 ; `contrat-openapi.md` §6 |
| 16 | Mécanisme de reprise en place (« reclaim-in-place ») : bascule de l'indicateur de vérification d'email et neutralisation obligatoire du mot de passe préexistant avant liaison fédérée — **statut `[À TRANCHER — à ratifier opérateur]`, non acquis** | S2 / Elevation of Privilege | ADR-015 §2.3 |

### 4.3 Autorisation API et anti-fuite par visibilité (surfaces S3, S4)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 17 | Point d'autorisation unique (`IResourceAccessPolicy.CanAccess`) composant appartenance et visibilité (`Document.CanBeReadBy`) | S3 / Elevation of Privilege | ADR-014 §3 |
| 18 | Comportement de pipeline appliqué systématiquement sur toute requête portée par un espace, court-circuit 403/404 avant exécution du gestionnaire métier, sans vérification dupliquée | S3 / Elevation of Privilege | ADR-014 §3 |
| 19 | Anti-fuite par visibilité sur le canal temps réel : le filtre de diffusion SignalR invoque la même politique d'autorisation par message ; un contenu `GM_ONLY` ou `PLAYER_PRIVATE` n'est jamais transmis à un tiers ; tests anti-fuite SignalR référencés sous le code de test **B8.2** | S4 / Information disclosure | ADR-014 §6 ; §Points à trancher ; ADR-004 §Compléments post-revue |
| 20 | Règle uniforme de confidentialité `PLAYER_PRIVATE` réservée à l'auteur seul (meneur de jeu exclu, tous types de contenu confondus), appliquée sur tous les chemins y compris la résolution via une session | S3 / S4 Information disclosure | ADR-014 §7 ; `session-conduct.md` |
| 21 | Exclusion silencieuse des liens retour non lisibles par l'appelant | S3 / Information disclosure | ADR-014 §Fuite d'existence par backlink ; `contrat-openapi.md` §3.2 |
| 22 | Filtres de requête globaux (dates de suppression et de purge, indicateur de suppression) appliqués sur tous les chemins de lecture, y compris la lecture par identifiant direct | S3 / Information disclosure | ADR-014 §2 ; ADR-011 §Invariant de visibilité du soft-delete |
| 23 | Verrou anti-saut de privilège invité → utilisateur enregistré : la conversion conserve le périmètre de l'accès invité d'origine | S3 / Elevation of Privilege | ADR-014 §1 / §5 |
| 24 | Non-révélation d'existence par différenciation des codes de réponse (403 pour un refus connu, 404 pour un non-membre) | S3 / Information disclosure | ADR-014 §Conséquences ; `contrat-openapi.md` §2 / §4 |
| 25 | Transmission du jeton d'accès invité hors chaîne de requête sur le canal temps réel | S4 / Spoofing | ADR-004 §Compléments post-revue |
| 26 | Déconnexion forcée du canal temps réel sur révocation ou expiration de l'accès invité | S4 / révocation en session | ADR-004 §Compléments post-revue |

### 4.4 Sanitisation et politique de sécurité de contenu (surfaces S5, S6, S7)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 27 | Sanitisation côté client par liste blanche positive : balises `<script>` et attributs gestionnaires d'événement supprimés sans exception, appliquée avant injection dans le DOM et avant persistance | S5 / S7 Tampering | ADR-017 §4.1 ; `sanitisation-csp.md` §1 / §3 |
| 28 | Sanitisation côté serveur par liste blanche positive à l'import de migration, avec le même plancher de règles | S6 / Tampering | ADR-016 §2.4 ; `sanitisation-csp.md` §1 |
| 29 | Ordre impératif de traitement de l'import JSON local : validation structurelle, puis sanitisation, puis écriture | S7 / Tampering | ADR-017 §4.3 ; `sanitisation-csp.md` §2 |
| 30 | Politique de sécurité de contenu stricte (`default-src 'self'`, `script-src 'self'` sans exécution en ligne, `connect-src 'self'`) | S5 / Information disclosure | ADR-017 §4.2 ; `sanitisation-csp.md` §4 |

### 4.5 Frontière de confiance de la migration (surface S6)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 31 | Champs d'autorité (propriétaire, créateur) jamais honorés depuis la charge utile importée ; tous les identifiants sont régénérés côté serveur | S6 / Tampering | ADR-016 §2.1 / §2.2 |
| 32 | Revalidation systématique par les objets-valeur du domaine, sur le chemin d'écriture normal, sans second validateur dédié | S6 / Tampering | ADR-016 §2.3 |
| 33 | Verrou anti-appropriation de poste partagé : rejet de l'import en l'absence d'un drapeau serveur explicite de confirmation | S6 / Spoofing | ADR-016 §4 |
| 34 | Transaction par espace, exécution à blanc préalable, rapport de rejets, idempotence par identifiant de lot de migration | S6 / Intégrité | ADR-016 §3 |
| 35 | Absence de tout jeton en régime local (aucun jeton d'accès, de rafraîchissement ou de session dans le navigateur) | S5 / réduction de surface | ADR-017 §4.5 ; ADR-015 §Périmètre |

### 4.6 Stockage au repos — limitation assumée (surface S5)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 36 | Absence de chiffrement au repos d'IndexedDB, traitée comme une limitation conçue pour le périmètre initial, assortie d'une information explicite de confidentialité distincte de l'information de durabilité | S5 / Information disclosure (poste partagé, outils de développement) | ADR-017 §4.4 |

### 4.7 Cascade et effacement RGPD (surface S9)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 37 | Toutes les clés étrangères en suppression restreinte (`RESTRICT`) ; la cascade est pilotée exclusivement par saga applicative, jamais par une cascade SQL silencieuse | S9 / Intégrité | ADR-011 §1 |
| 38 | Sagas de suppression d'espace et d'anonymisation de compte : réservation exclusive, expiration récupérable, transaction unique, idempotence | S9 / Intégrité, garantie article 17 | ADR-011 §Séquences de saga |
| 39 | Suppression physique définitive des documents `PLAYER_PRIVATE` (créés par l'utilisateur et rattachés à ses personnages incarnés) | S9 / Information disclosure (résidu) | ADR-012 §3(a) / §4 ; `identity-access.md` (garde-fou n°4) ; `session-conduct.md` (garde-fou n°8) |
| 40 | Suppression physique inconditionnelle du contenu de l'espace personnel à la suppression de compte | S9 / Information disclosure | ADR-012 §2 / §Conséquences ; ADR-018 §Effacement de compte ; `identity-access.md` (invariant 3) |
| 41 | Purge des journaux de corrélation identifiant technique ↔ email au sein de la saga d'anonymisation, y compris sur la rétention de sauvegardes à 30 jours | S9 / Information disclosure | ADR-012 §5 |
| 42 | Révocation des jetons actifs comme étape de la saga, sur anonymisation et sur suspension de compte | S9 / Elevation of Privilege (accès résiduel) | ADR-015 §3.6 ; ADR-012 §Conséquences |
| 43 | Figement de la sélection des données à l'instant de la demande d'effacement, jeu de données matérialisé consommé sans recalcul, étape de relocation obligatoire | S9 / Tampering (cohérence temporelle) | ADR-012 §7 ; `requete-effacement-non-partage.md` §2 |
| 44 | Purge autonome des accès invités expirés (90 jours après expiration, journaux techniques conservés 30 jours), tâche idempotente | S9 / minimisation | ADR-013 §3 |

### 4.8 Télémétrie (surface S8)

| # | Contre-mesure | Surface / menace | Ancre |
|---|---|---|---|
| 45 | Minimisation : mesure d'occurrence sans captation du contenu narratif ; exigence de résultat d'anonymisation actée, moyen technique non tranché — cohérent avec NFR-CONF-02 | S8 / Information disclosure | `telemetrie.md` §5 ; `cahier-des-charges.md` |

### 4.9 Chaînes structurantes

La topologie scale-out multi-instances actée par ADR-004 (§Compléments post-revue — sticky sessions, connexions persistantes) est la racine commune de plusieurs mesures de ce dossier, présentées ailleurs comme si elles étaient indépendantes : au minimum la denylist JTI externe et partagée (#7) et le comportement d'exécution du canal SignalR / backplane, renvoyé en section 5.1. Le risque évité : sans magasin de révocation partagé hors-process, une révocation opérée sur une instance resterait invisible des autres jusqu'à l'expiration naturelle du jeton. Ancres : ADR-015 §3.4 ; ADR-004 §Compléments post-revue.

---

## 5. Points ouverts et dette de sécurité

### 5.1 Dettes nommées et valeurs renvoyées à l'implémentation

| Point ouvert | Ancre | Référence |
|---|---|---|
| Vérification de mot de passe compromis (HaveIBeenPwned, k-anonymat) — post-MVP | ADR-015 §Conséquences | — |
| Jeton de rafraîchissement glissant — post-MVP | ADR-015 §Conséquences | — |
| Paramètres exacts d'Argon2id (mémoire, itérations, parallélisme) | `config-securite-migration.md` ; ADR-015 Annexe (illustratif, non retenu tel quel) | `[À TRANCHER — B1.5]` |
| Valeurs exactes des seuils de limitation de débit et borne supérieure de sécurité | ADR-015 §4.2 ; `config-securite-migration.md` | `[À TRANCHER — B1.5]` |
| Durée de vie exacte du jeton de réinitialisation (dans la plage 10 à 15 minutes) | ADR-015 §4.3 | `[À TRANCHER — B1.5]` |
| Fraîcheur maximale et déclenchement de la ré-authentification auprès du fournisseur d'identité | ADR-015 §1.4 | `[À TRANCHER — B1.5]` |
| Liste exhaustive des balises autorisées et bibliothèque de sanitisation (serveur et client) | `sanitisation-csp.md` §5 | `[À TRANCHER — B1.2 / P6]` |
| Directives CSP complètes et valeurs de nonce | ADR-017 §4.2 ; `sanitisation-csp.md` §5 | `[À TRANCHER — P6]` |
| Chiffrement au repos d'IndexedDB — impossible sans secret utilisateur en régime sans compte | ADR-017 §4.4 | post-MVP |
| Sémantique 403 / 404 pour un non-membre inter-espaces, schémas de requête et de réponse, corps de la réponse 429, code du rejet OAuth silencieux, format d'annotation de sécurité OpenAPI | `contrat-openapi.md` §Récapitulatif | `[À TRANCHER — B1.10]` |
| Comportement d'exécution SignalR (gestion de groupes, reconnexion, propagation des révocations et expirations d'accès invité) et vérification du jeton en connexion persistante | ADR-014 §Périmètre ; ADR-015 §Points à trancher | `[À TRANCHER — B1.7]` |
| Codes de test dédiés : **B3.2** (test d'architecture), **B5.2** (test IDOR), **B8.2** (test anti-fuite SignalR), **B5.1** (filtres de requête) | ADR-014 §Points à trancher | renvois de test |
| Seuil de coût de session concurrente, métrique associée, cadence de sondage adaptatif, mécanisme de détection | `repli-temps-reel.md` §3 / §5 ; ADR-004 §Compléments post-revue | `[À TRANCHER — ticket]` |
| Télémétrie : valeur de seuil d'usage réel constaté, opérationnalisation de ce seuil, outil analytique, schéma d'événements, technique d'anonymisation RGPD, mécanisme de capture d'email non bloquant | `telemetrie.md` §6 | `[À TRANCHER — ticket]` |
| Durée de la réservation de purge, seuil minimal de version de schéma, durée de rétention de l'identifiant de lot de migration | `config-securite-migration.md` ; ADR-011 ; ADR-016 | `[À TRANCHER — B1 / P7]` |
| Références nullables entrantes non couvertes (`documents.character_id`, `guest_accesses.character_id`, `folders.default_template_document_id` entrants) | `requete-effacement-non-partage.md` §5 ; ADR-012 §4 | `[À TRANCHER — ticket]` |
| Câblage de la purge des médias externalisés sur les deux chemins d'effacement (suppression d'espace et purge de l'espace personnel) | ADR-011 §Contenu LIVE_NOTE et médias externalisés | `[À TRANCHER — B1.9]` |

### 5.2 Postures non ratifiées et arbitrages assumés

- **Reprise en place (« reclaim-in-place »)** — mécanisme proposé pour résoudre le cas d'une liaison OAuth sur un compte préexistant non vérifié. **Statut : `[À TRANCHER — à ratifier opérateur]`. Ce mécanisme n'est pas une mesure acquise** tant qu'il n'a pas été ratifié. Ancre : ADR-015 §2.3.
- **Résidu d'énumération d'email (CWE-204)** — le message d'erreur « adresse déjà associée » constitue un résidu d'énumération assumé. Il s'agit d'une décision opérateur tracée, **acceptée** et mitigée par la limitation de débit — ce n'est pas un point ouvert mais un arbitrage clos. Ancre : ADR-015 §Résidu CWE-204.

### 5.3 Gates humains hors CI — validations juridiques préalables au lancement dans l'Union européenne

Cinq axes de validation juridique conditionnent le lancement dans l'Union européenne. Ces validations ne sont, par nature, pas vérifiables par un dispositif d'intégration continue.

| Axe | Objet | Ancres |
|---|---|---|
| Axe 1 | Article 8 RGPD — mineurs, attestation d'âge (16 ans et plus) | `cadrage-validation-pre-lancement-eu.md` (axe 1) ; ADR-012 ; ADR-013 §4 |
| Axe 2 | Article 28 RGPD et périmètre du contrat de sous-traitance (y compris le contenu de l'espace personnel décrivant des tiers) | `cadrage-validation-pre-lancement-eu.md` (axe 2) ; ADR-013 §5 ; ADR-018 ; `dpa-skeleton.md` |
| Axe 3 | Mise en balance de l'intérêt légitime (article 6.1(f) / §3(c) et nom d'affichage de l'invité) | `cadrage-validation-pre-lancement-eu.md` (axe 3) ; ADR-012 ; ADR-013 |
| Axe 4 | Article 17 RGPD — suppression physique inconditionnelle du contenu de l'espace personnel, instant de référence de la demande d'effacement, déplacements de contenu | `cadrage-validation-pre-lancement-eu.md` (axe 4) ; ADR-012 §7 ; ADR-018 |
| Axe 5 | Facette RGPD du mécanisme de reprise en place | `cadrage-validation-pre-lancement-eu.md` (axe 5) ; ADR-015 §2.3 |

Un point de validation supplémentaire est signalé hors des cinq axes : le canal de notification prévu à l'article 12§3 pour un compte anonymisé, les critères de l'extension de délai de deux mois, et la méthode de vérification d'identité d'un invité exerçant ses droits — **signalement juridique explicite (FLAG JURISTE)**. Ancres : ADR-012 §8 ; `procedure-droits-invites.md` §2 / §4 / §5.

### 5.4 Maillons non vérifiables en intégration continue

Les éléments suivants sont des **limites structurelles nommées**, non des dettes de test à corriger : leur nature (dépendance externe, comportement de navigateur réel, charge réelle) exclut par construction une vérification automatisée en intégration continue.

| Maillon | Nature de la limite | Ancre |
|---|---|---|
| Fiabilité de la déclaration de vérification d'email par le fournisseur d'identité (`email_verified` / `verified`) | Dépendance externe | ADR-015 §2.2 |
| Octroi ou refus de `navigator.storage.persist()` et éviction réelle d'IndexedDB par le navigateur | Comportement navigateur réel | ADR-017 §3.3 ; `sanitisation-csp.md` §6 |
| Migration IndexedDB multi-versions en navigateur réel | Comportement navigateur réel (test de bout en bout) | ADR-016 §Maillon ; `sanitisation-csp.md` §6 |
| Coût et repli réels du canal temps réel sous charge | Charge réelle | `repli-temps-reel.md` §4 |
| Les cinq axes de validation juridique | Jugement humain, hors CI par nature | `cadrage-validation-pre-lancement-eu.md` §Gate humain hors CI |

### 5.5 Lacunes signalées

Cette sous-section distingue deux natures de lacunes STRIDE : l'une nommée comme point ouvert par le corpus lui-même, l'autre observée à partir d'une absence de traitement — et signalée comme telle, sans lui prêter le statut d'un point ouvert acté par le corpus.

**L1 — Notification de violation de données (articles 33 et 34 du RGPD).** Le corpus nomme lui-même ce sujet comme point ouvert : aucune procédure de notification de violation n'est encore arrêtée. Ancre : `dpa-skeleton.md §4(f)`.

**L2 — Journalisation d'audit de sécurité à visée positive (catégorie STRIDE-Repudiation).** *Observation* : le corpus n'aborde les journaux d'événements que sous l'angle de leur purge (ADR-012 §5) ; aucune contre-mesure positive de traçabilité des événements de sécurité n'est spécifiée par ailleurs. **Le corpus ne pose pas cette question — aucune posture n'est actée sur ce point.** Cette observation est à confirmer par l'opérateur avant l'ouverture d'un ticket ; elle n'est ni un `[À TRANCHER]` du corpus, ni une exigence de ce dossier.

---

## 6. Tensions résolues

Trois tensions ont été explicitement tranchées dans le corpus de conception. Elles sont présentées ici comme des arbitrages clos, non comme des conflits ouverts.

| Tension | Résolution | Statut | Ancre |
|---|---|---|---|
| Unicité de l'email (invariant du modèle de compte) contre protection anti-préemption (CWE-287) | Résolue par le mécanisme de reprise en place ; l'option d'un « nouveau compte à côté » a été déclarée non implémentable | Résolution retenue, **mise en œuvre non ratifiée** (cf. 5.2) | ADR-015 §2.3 |
| Ergonomie du message d'erreur (résidu d'énumération CWE-204) contre non-révélation stricte | Arbitrée en faveur de l'ergonomie ; le résidu est accepté et mitigé par la limitation de débit | Arbitrage clos | ADR-015 §Résidu CWE-204 |
| Conservation au titre de l'intérêt légitime (§3(c)) contre suppression physique inconditionnelle du contenu de l'espace personnel sur document déplacé | Fermée techniquement par l'étape de relocation obligatoire | Fermée techniquement ; suffisance juridique en attente (FLAG JURISTE, cf. 5.3) | ADR-012 §7 |

---

## 7. Note de fidélité

Ce dossier est une consolidation : il ne substitue son jugement à aucune décision actée dans le corpus de conception et n'introduit aucune qualification de sécurité qui n'y figure pas explicitement.

Trois précisions ferment ce document :

1. Les valeurs numériques figurant en annexe des décisions sources (paramètres Argon2id « 64 Mio / 3 itérations », seuils de limitation de débit « 20 requêtes pour 10 minutes », durée de réservation de purge « 1 heure ») sont **illustratives dans leurs documents d'origine** et n'ont pas été reprises comme valeurs retenues dans ce dossier. Seule la borne normative décidée est reproduite (par exemple : jeton d'accès ≤ 15 minutes) ; la valeur numérique fine reste renvoyée à son ticket d'implémentation (section 5.1).
2. Le mécanisme de reprise en place (« reclaim-in-place ») est présenté partout dans ce dossier avec son statut **non ratifié** ; il ne doit pas être lu comme une mesure acquise.
3. Les lacunes signalées en section 5.5 ne sont pas des mesures acquises : L1 est un point ouvert que le corpus nomme lui-même ; L2 est une observation formulée à partir d'une absence de traitement, non une décision du corpus, et reste à confirmer par l'opérateur avant toute ouverture de ticket.

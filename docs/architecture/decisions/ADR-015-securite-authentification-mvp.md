# ADR-015 — Sécurité authentification MVP

- **Statut** : Accepté
- **Date** : 2026-06-10
- **Décideur** : opérateur (cadrage P1 — findings reclassés bloquants MVP par ADR-007 §Compléments)
- **Findings liés** : F-02 (CWE-521), F-06 (CWE-307), F-10 (CWE-613/294/347), F-11 (CWE-287)

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03, audit conception pure 2026-06.)*

---

## Périmètre de cet ADR

Cet ADR couvre les quatre findings sécurité reclassés bloquants-MVP par ADR-007 §Compléments : politique de mot de passe, cycle de vie des tokens JWT, rate limiting des endpoints d'auth, et sécurisation de la liaison OAuth. Ces décisions s'appliquent au **mode cloud uniquement** : ADR-001 acte que le mode local n'a aucune authentification — pas de compte, pas de JWT, pas de mot de passe côté navigateur.

Contrairement aux ADR de la flotte RGPD (ADR-007, ADR-012, ADR-013), cet ADR ne définit pas de nouvelles politiques de fond : les seuils ont été actés dans ADR-007 §Compléments. Il formalise ces seuils, précise les mécanismes de raccordement qui manquaient, et pose les contrats applicatifs observables nécessaires au test d'archi CI.

**Hors périmètre de cet ADR** (renvois explicites) :
- Token SignalR (transport, transmission hors query-string) et vérification JTI sur connexion persistante → **B1.7**
- Contrat OpenAPI des endpoints d'auth (codes 400/401/403, schémas) → **B1.10**
- Vérification des mots de passe contre les listes de compromission (breach-check HaveIBeenPwned k-anonymity) → **post-MVP** (dette nommée, §Conséquences)
- Refresh glissant (prolongation de la durée du refresh token à chaque usage) → **post-MVP** (dette nommée, §Conséquences)
- Tests de sécurité spécifiques auth (brute-force, replay, rotation) → **B3.2**

---

## Contexte

ADR-007 §Compléments (2026-06-09) a reclassé les findings F-02, F-06, F-10, F-11 de « Vague 2 » en bloquants-MVP, sans définir les mécanismes précis. Ces quatre findings forment un ensemble cohérent : ils couvrent toutes les surfaces d'attaque de la couche d'authentification (credentials, tokens, accès OAuth, énumération).

**F-02** (CWE-521) identifie l'absence de toute politique de mot de passe : aucune longueur minimale, aucune règle de hachage documentée, validation email absente de facto.

**F-06** (CWE-307) identifie l'absence de rate limiting sur l'endpoint de validation de token GuestAccess. L'audit se limitait à cet endpoint, mais le risque s'étend à tous les endpoints d'auth (login, refresh, reset de mot de passe).

**F-10** (CWE-613/294/347) identifie des tokens de durée indéfinie et l'absence de mécanisme de révocation : un `AccountSuspended` n'a pas de prise sur un token existant. Une fenêtre d'exploitation longue durée et un accès résiduel post-effacement RGPD sont les conséquences directes.

**F-11** (CWE-287) identifie la liaison automatique email→OAuth sans vérification de l'email préexistant, vecteur de prise de contrôle de compte (pre-account-hijacking).

Ces quatre dimensions interagissent. La configuration par défaut d'ASP.NET Identity couvre partiellement F-02, mais les valeurs par défaut ne sont ni documentées ni opposables par un test CI. L'absence d'une interface de contrat applicatif sur la validation des tokens (`ITokenValidator`, `ITokenDenylist`) produit une sécurité opaque : les garanties ne sont pas observables, pas testables, et leur dérive n'est pas détectable par le test d'archi CI (→ B3.2). Ce constat est le même que celui d'ADR-014 pour `IResourceAccessPolicy` — la réponse est symétrique.

---

## Décision

### 1. Politique de mot de passe (ferme F-02, CWE-521)

#### 1.1 Configuration `PasswordOptions`

La configuration ASP.NET Identity est définie explicitement dans le code applicatif, non laissée à ses valeurs par défaut. Les options suivantes sont positionnées :

*Esquisse pré-implémentation : voir Annexe, bloc 1.*

La longueur minimale est 12 caractères (valeur « gratuite » côté `PasswordOptions` qui satisfait le minimum de 8 exigé par F-02 avec marge). Les règles de complexité (chiffres, symboles, casse) sont **désactivées explicitement**. Ce choix est délibéré : le NIST SP 800-63B §5.1.1 démontre que ces règles poussent les utilisateurs vers des patterns prévisibles (substitutions triviales, mots de passe en fin de liste imposée) et réduisent l'espace d'entropie effectif. La longueur seule est le facteur de résistance pertinent.

Les options `RequireDigit`, `RequireNonAlphanumeric`, `RequireUppercase`, `RequireLowercase` sont **positionnées à `false`** dans le code, pas simplement commentées. Une valeur par défaut non explicite est invisible au test d'archi et vulnérable à une régression par upgrade de package.

#### 1.2 Algorithme de hachage

L'implémentation `IPasswordHasher<User>` utilisée est **Argon2id** (via une librairie .NET compatible, ex. `Konscious.Security.Cryptography` ou `Isopoh.Cryptography.Argon2`) avec les paramètres suivants :

*Esquisse pré-implémentation : voir Annexe, bloc 2.*

Si Argon2id n'est pas intégrable dans la contrainte de dépendances MVP, **bcrypt avec cost factor ≥ 12** constitue le repli acceptable. La valeur par défaut de `PasswordHasherCompatibilityMode.IdentityV3` (PBKDF2-SHA256, 100 000 itérations) est insuffisante selon les recommandations actuelles du NIST et de l'OWASP. Le remplacement doit être opéré via l'interface `IPasswordHasher<TUser>` exposée par ASP.NET Identity — pas en surchargeant directement les méthodes internes.

#### 1.3 Vérification de compromission (breach-check)

La vérification du mot de passe contre une liste de mots de passe compromis (API HaveIBeenPwned k-anonymity, dite « pwned passwords ») est **une dette nommée, reportée post-MVP**. Elle n'est pas un trou silencieux : son absence est documentée ici et visible dans §Conséquences. La mise en œuvre post-MVP utilise le protocole k-anonymity (envoi du prefixe de 5 caractères du hash SHA-1, jamais le hash complet ni le mot de passe en clair).

#### 1.4 Mot de passe complémentaire sur compte fédéré (compte hybride)

> **Note** : CWE-620 (voir ci-dessous) n'a pas de finding F-xx d'origine dans l'audit — cette section est une décision PROACTIVE, au même titre que §4.3 (CWE-640), motivée par la décision opérateur (5) autorisant le mot de passe complémentaire sur compte fédéré.

Un compte créé et authentifié uniquement via un fournisseur fédéré (Google, Discord — §2.2) peut définir un **mot de passe complémentaire**, devenant ainsi un compte **hybride** (authentifiable par le fournisseur fédéré OU par email/mot de passe). Ce mécanisme est une mesure **préventive** : il permet d'anticiper un verrouillage permanent du compte (lockout) si l'accès au fournisseur d'identité venait à être perdu ultérieurement (compte fournisseur suspendu, supprimé, ou temporairement inaccessible).

**Portée préventive, pas récupératrice.** Cette mesure doit être activée **tant que l'accès au fournisseur d'identité subsiste** : sa définition exige elle-même une ré-authentification IdP récente (gap CWE-620, ci-dessous). Si l'accès IdP est **déjà perdu** au moment où l'utilisateur en aurait besoin, cette ré-authentification est impossible et le mot de passe complémentaire ne peut plus être créé — le mécanisme n'offre donc **aucune capacité de récupération** une fois l'accès IdP effectivement perdu. Une récupération post-perte (flux distinct de type *account-recovery*, ex. vérification d'identité alternative) constituerait un flux séparé, **hors périmètre MVP** (dette nommée, §Conséquences).

**Définition d'un premier mot de passe — opération sensible.** La définition de ce premier mot de passe est une **opération sensible** au sens du §2.1 : elle exige `emailVerified = true` (condition déjà satisfaite pour un compte fédéré, cf. §2.2) et le respect intégral de la politique de mot de passe des §1.1/§1.2 (Argon2id, longueur ≥ 12).

**CWE-620 — gap de preuve d'identité.** Le mécanisme standard d'une opération sensible sur le mot de passe (« saisissez votre mot de passe actuel ») **ne s'applique pas ici** : un compte fédéré-only n'a, par construction, aucun mot de passe existant à faire saisir. S'appuyer sur la seule session applicative Haversack active pour autoriser la définition ouvrirait un gap CWE-620 (preuve d'identité insuffisante avant une opération sensible sur les credentials du compte). La définition du premier mot de passe **exige donc une preuve d'identité alternative : une ré-authentification récente et complète auprès du fournisseur d'identité fédéré** (nouveau flux OAuth avec le fournisseur, pas la seule présence d'un cookie de session Haversack), immédiatement avant l'opération. La fraîcheur maximale tolérée de cette ré-authentification et son mécanisme de déclenchement exact sont renvoyés à B1.5.

**Amendement RB-10-10 — Résolu (US-UC-10 amendée).** RB-10-10, définie dans US-UC-10 (`docs/conception/user-stories/US-UC-10-compte-cloud.md`), énonçait qu'un `User` authentifié uniquement via connexion fédérée n'a pas de mot de passe et que la réinitialisation de mot de passe ne s'applique pas à ce compte. Cette règle reste exacte **au sens strict de la réinitialisation** (il n'existe rien à réinitialiser) mais ne couvrait pas — et ne devait pas être lue comme excluant — la **définition d'un premier mot de passe**, opération distincte autorisée par le présent ADR sous les conditions ci-dessus. La distinction est désormais portée dans le texte de RB-10-10 : « réinitialisation d'un mot de passe inexistant » reste **N/A** (inchangé) ; « définition d'un premier mot de passe sur compte fédéré » est **autorisée**, opération sensible avec ré-authentification IdP (ce paragraphe) — mesure restant **préventive**, sans capacité de récupération si l'accès IdP est déjà perdu (précision ci-dessus). Ce point est résolu sur le même modèle que le renvoi ADR-012 (§Conséquences, également résolu) : ADR-015 définit la règle, US-UC-10 §RB-10-10 porte désormais le texte amendé.

---

### 2. Validation email et liaison OAuth (ferme F-11 + F-02, CWE-287)

Deux parcours de création de compte doivent être distingués avec des règles différentes.

#### 2.1 Parcours email/mot de passe

À la création d'un compte email/mot de passe, un email de validation est envoyé. La validation Haversack est **requise pour les opérations sensibles** (modification de l'email, modification du mot de passe, liaison d'un fournisseur OAuth, définition d'un premier mot de passe sur un compte fédéré — §1.4, demande d'effacement RGPD Art. 17) mais **non bloquante au login initial**. Ce choix est cohérent avec la vision « friction d'entrée nulle » : un utilisateur peut explorer l'outil sans valider son email, mais ne peut pas modifier son identité ni lier un compte OAuth sans validation préalable.

La contrainte « opérations sensibles requièrent email vérifié » est portée par un prédicat applicatif exposé via une interface :

*Esquisse pré-implémentation : voir Annexe, bloc 3.*

Ce contrat est positionné dans la couche Application (même positionnement qu'`IResourceAccessPolicy` dans ADR-014), vérifiable par le test d'archi CI. La signature normative complète est en §5.

#### 2.2 Parcours fournisseurs OAuth fédérés — Google, Discord (email réputé vérifié)

Les **fournisseurs d'identité retenus pour le MVP sont Google et Discord**. Lors d'une connexion ou création de compte via l'un de ces fournisseurs, Haversack lit le claim d'email vérifié qu'il expose — `email_verified` pour Google (token d'identité / ID token), flag `verified` porté par l'objet `user` de l'API OAuth2 pour Discord. Si ce claim vaut vrai, l'email est réputé vérifié par l'IdP : Haversack n'envoie pas de second email de validation, ce qui est conforme à la posture « friction nulle ».

**Règle de confiance, généralisée par fournisseur** (ex-règle « en forme de Google », désormais posée par fournisseur) : un fournisseur OAuth n'est éligible à la liaison de compte (§2.3) que s'il satisfait deux conditions cumulatives — (1) il expose un claim (ou équivalent) d'email vérifié jugé **fiable**, c'est-à-dire que le fournisseur ne le délivre à vrai qu'après avoir lui-même vérifié la maîtrise de l'adresse par son détenteur ; (2) il expose une **adresse email canonique** (non un alias de relais), seule base valable pour le matching de liaison (§2.3, RB-10-08). Google et Discord satisfont ces deux conditions pour le périmètre MVP.

**Dépendance externe — maillon NON VÉRIFIABLE IN BUILD** : la confiance dans le claim d'email vérifié de chaque fournisseur retenu (`email_verified` Google, `verified` Discord) est une dépendance au comportement de l'IdP — elle ne se prouve pas par la CI Haversack, quel que soit le fournisseur. Le gate applicatif (§2.3 ci-dessous) reste, lui, testable en CI via un provider OAuth mocké, pour chaque fournisseur retenu.

**Dette nommée — fournisseur à email de relais / non canonique** : l'ajout futur d'un fournisseur exposant un email de relais ou non canonique (ex. Apple « Hide My Email », qui masque l'adresse réelle derrière un alias généré) ne satisfait pas la condition (2) ci-dessus telle quelle : le matching de liaison par email canonique (RB-10-08) perdrait son ancrage, un alias de relais n'identifiant pas de façon stable le même détenteur d'une session à l'autre selon la politique du fournisseur. L'ajout d'un tel fournisseur impose de **revisiter la règle de liaison par email (RB-10-08)** avant activation. Cette dette est nommée ici et non résolue — elle n'est pas déclenchée par le périmètre MVP (Google + Discord, tous deux à email canonique).

#### 2.3 Liaison OAuth à un compte préexistant

La liaison d'un compte OAuth (Google ou Discord) à un compte email/mot de passe **préexistant** est autorisée uniquement si l'email du compte préexistant est **prouvé vérifié** au sens du §2.1, le matching s'effectuant sur l'**email canonique** (RB-10-08). La liaison est interdite si l'email du compte préexistant n'a pas été validé — cette interdiction est **indépendante du fournisseur** (Google, Discord, ou tout fournisseur futur satisfaisant §2.2).

**Motif** : un email non vérifié est un email que n'importe qui peut avoir renseigné (spam d'inscription, usurpation). Autoriser la liaison OAuth vers ce compte revient à permettre à un tiers — disposant d'un compte chez le fournisseur fédéré sur cet email — de prendre le contrôle du compte Haversack sans que l'usurpateur ait prouvé sa maîtrise de l'email. Ce vecteur est documenté sous le nom « pre-account-hijacking » dans la littérature OAuth.

**Règle** :

*Esquisse pré-implémentation : voir Annexe, bloc 4.*

La liaison est rejetée silencieusement si la condition n'est pas satisfaite (pas de message d'erreur révélant l'existence du compte — cohérent avec ADR-014 §Conséquences, principe de non-révélation d'existence).

#### Résolution — email OAuth face à un compte préexistant NON vérifié (reclaim-in-place)

Le rejet silencieux ci-dessus répond à « la liaison automatique est-elle autorisée ? » — non. Il laisse ouverte la question suivante : que devient alors la tentative de connexion OAuth ? Le traitement envisagé ailleurs dans le corpus pour ce cas — créer un nouveau compte à côté de la coquille non vérifiée (US-UC-10, RB-10-08 (b), point actuellement ouvert dans cette user story) — n'est **pas implémentable** : il ferait coexister deux comptes sur la même adresse email, en violation de l'invariant 1 (`email` unique dans le système, `identity-access.md`).

**Constat.** Deux exigences tirent en sens contraire sur la même adresse : ne pas lier automatiquement une identité fédérée à une coquille non vérifiée (anti-hijacking, CWE-287, règle ci-dessus), et ne pas dupliquer un email (invariant 1). Le « nouveau compte » évoqué par ailleurs satisfait la première exigence mais viole la seconde — il est écarté ici comme **non implémentable**, pas comme option concurrente à arbitrer.

**Résolution proposée — Option A, *reclaim-in-place*.** La preuve de possession apportée par le fournisseur OAuth (email réputé vérifié par l'IdP, §2.2) reprend la coquille non vérifiée existante plutôt que de créer un compte concurrent :

- `emailVerified` de la coquille passe à `true` ;
- l'identité fédérée est liée à ce compte (`LinkFederatedIdentity()`, invariant 7 de `identity-access.md`) ;
- le credential mot de passe préexistant de la coquille est **neutralisé de façon obligatoire** — hash de mot de passe et `security_stamp` réécrits — selon le précédent déjà acté pour l'anonymisation dans **ADR-007 §Compléments** (« la neutralisation de `asp_net_users` … est obligatoire, pas optionnelle »). Sans cette neutralisation, un mot de passe antérieur resterait valide sur un compte dont la preuve de possession vient de changer de main.

**Justification.** Une coquille non vérifiée n'a, par construction, aucun propriétaire prouvé — elle n'a jamais franchi la validation d'email (§2.1). L'anti-hijacking ci-dessus protège un compte **vérifié**, c'est-à-dire des données dont la possession est prouvée ; il ne protège pas une coquille dont personne n'a établi la possession. Reprendre une coquille non prouvée au bénéfice d'une preuve IdP fraîche n'ouvre donc pas de vecteur de prise de contrôle : le résultat observable (compte à email vérifié, lié au fournisseur, mot de passe antérieur neutralisé) est **identique à celui d'une création OAuth normale (§2.2)** sur la même adresse. Aucune information supplémentaire n'est révélée à l'appelant OAuth par rapport au cas « aucun compte existant » — le canal d'énumération CWE-204 (§Résidu CWE-204) n'est pas rouvert par cette résolution.

**Alternatives écartées.**
- *Refus non-silencieux* (message d'erreur explicite à la tentative OAuth) — réouvre le canal d'énumération CWE-204 que le rejet silencieux ferme.
- *Email synthétique* (sur le modèle `deleted-{id}@haversack.invalid`, ADR-007 §Compléments) — ce motif répond à un effacement terminal (compte mort, email libéré) ; une coquille reprise par reclaim reste une identité **vivante**, l'usage d'un email synthétique est inadapté ici.
- *Suppression ou déplacement de la coquille* (cascade `UserDeleted`, ADR-012) — écartée comme résolution par défaut : cette cascade est dimensionnée pour un effacement RGPD délibéré, pas pour un conflit d'email au moment d'une connexion OAuth. Conservée uniquement comme repli (*fallback*) si le reclaim-in-place s'avère techniquement impossible en build.

**Statut** : `[À TRANCHER — à ratifier opérateur]`. Résolution de posture sécurité issue de l'audit sécurité et de la revue critic, **non encore validée par l'opérateur** — même registre de traçabilité que le §Résidu CWE-204 ci-dessous, sans en partager le statut : celui-ci est déjà tranché, celui-ci reste à ratifier.

**Routage.**
- **Facette RGPD** (sort du contenu éventuel — notes, documents — rattaché à la coquille non vérifiée évincée par le reclaim) : hors périmètre sécurité de cet ADR → **Lot 14 juridique**.
- **Opération de domaine dédiée** (bascule `emailVerified` + neutralisation du credential + liaison fédérée — `ReclaimViaFederatedProof()` ou équivalent, distincte de `LinkFederatedIdentity()` seule) : **B1.5**, même jalon que les autres volets d'implémentation de cette section.
- **Dépendance non vérifiable** : la fiabilité du claim d'email vérifié de l'IdP reste **NON VÉRIFIABLE IN BUILD**, au même titre que pour §2.2/§2.3 ci-dessus. Le gate applicatif (bascule `emailVerified` + neutralisation + liaison, conditionné à la preuve IdP) reste, lui, testable en CI via provider OAuth mocké.

---

### 3. Cycle de vie des tokens (ferme F-10, CWE-613/294/347)

#### 3.1 Durées

| Token | Durée | Mode de prolongation |
|---|---|---|
| Access token (JWT) | ≤ 15 min | Aucune — renouvellement via refresh uniquement |
| Refresh token | ≤ 7 jours ABSOLUS | Aucune prolongation glissante (borne dure) |

La borne de 7 jours est absolue, non glissante. Un refresh token émis à T expire à T+7j quels que soient ses usages entre-temps. Le refresh glissant (réinitialisation de la durée à chaque usage) est reporté post-MVP — dette nommée (§Conséquences). Le choix d'une borne dure borne la fenêtre d'exploitation d'un refresh token compromis sans nécessiter une infrastructure de révocation immédiate du côté client.

#### 3.2 Algorithme de signature JWT

L'algorithme de signature est **asymétrique** : **RS256** (RSA-PKCS1v1.5 avec SHA-256) ou **ES256** (ECDSA avec P-256 et SHA-256). ES256 est préférable si la contrainte de performance de signature est sensible (opérations plus rapides). RS256 est le repli compatible si des contraintes d'interopérabilité l'imposent.

L'algorithme `alg: none` est **explicitement interdit** dans la configuration du validateur JWT. Cette interdiction est portée par la configuration de `TokenValidationParameters` :

*Esquisse pré-implémentation : voir Annexe, bloc 5.*

Une tentative de validation d'un token signé avec `alg: none` doit échouer avec une exception, pas retourner un résultat valide.

La clé de signature (clé privée RSA ou EC) est gérée en infrastructure (secret manager ou certificat). La rotation de clé est décrite comme un livrable de configuration B1.5, pas un point d'implémentation de cet ADR — mais le contrat applicatif (`ITokenSigner`) doit permettre la rotation sans modification de code.

**Contrainte de gestion de clé** : la clé privée de signature NE DOIT PAS être committée au dépôt (CWE-321/798). Elle NE DOIT PAS résider en variable d'environnement en clair exposée aux logs. La gestion est obligatoirement via un secret manager ou un magasin de certificats (ex. Azure Key Vault, AWS Secrets Manager, Kubernetes Secrets chiffrés). La rotation s'effectue via `ITokenSigner.RotateKey(...)` sans redéploiement applicatif.

#### 3.3 Rotation des refresh tokens avec détection de réutilisation

Chaque refresh token appartient à une **famille** (`family_id`, UUID v4 généré à l'émission du premier token de la famille). À chaque appel `/token/refresh` :

1. Le serveur valide le refresh token présenté.
2. Si valide : émet un nouveau access token + un nouveau refresh token (même `family_id`, nouveau JTI), révoque l'ancien refresh token (inscrit son JTI en denylist).
3. Si le JTI présenté est **déjà en denylist** : le token a été réutilisé après rotation — probable vol du token précédent. Action : **révoquer toute la famille** (`family_id`). Tous les refresh tokens de la famille passent en denylist.

Ce mécanisme est aligné sur RFC 9700 (OAuth 2.0 Security Best Current Practice) et la détection de réutilisation recommandée dans OAuth 2.0 Security BCP §4.14. Une rotation sans détection de réutilisation est un demi-mécanisme : elle ne protège pas contre un attaquant qui rejoue l'ancien token avant que la victime ne l'utilise.

**Contrat applicatif** :

*Esquisse pré-implémentation : voir Annexe, bloc 6.*

Ce contrat est positionné dans la couche Application (même positionnement qu'`IResourceAccessPolicy` et `IEmailVerificationPolicy`), vérifiable par le test d'archi CI. Son implémentation en mémoire est explicitement marquée **mono-instance uniquement, à remplacer avant scale-out** (cf. §3.4). L'implémentation de référence MVP utilise un store partagé/externe (cache Redis ou table SQL dédiée) conformément à la topologie scale-out actée dans ADR-004 (sticky sessions, connexions persistantes — contexte qui impose un state externe).

#### 3.4 Denylist JTI et topologie scale-out

ADR-004 acte une topologie avec connexions persistantes et sticky sessions. Une denylist JTI en mémoire par instance est incompatible avec cette topologie : une révocation opérée sur l'instance A ne serait pas visible de l'instance B avant l'expiration naturelle du token. La denylist doit donc être **externe et partagée** dès le départ.

L'implémentation de référence utilise un store externe (Redis ou table SQL `token_denylist(jti, family_id, expires_at, revoked_at)`). Toute implémentation en mémoire de `ITokenDenylist` doit être annotée avec un attribut ou un commentaire structuré identifiable par le test d'archi CI, signalant son caractère mono-instance.

#### 3.5 Purge des JTI expirés

Les entrées de denylist dont `expires_at < now()` sont inutiles (un token expiré est rejeté par sa date d'expiration avant d'atteindre la vérification en denylist). Leur accumulation dégrade les performances de lookup.

Un **Hosted Service .NET** assure la purge périodique des JTI expirés. Ce service est **introduit par cet ADR, sur le pattern du Hosted Service idempotent défini dans ADR-011** (saga `SpaceDeleted` — même structure : sélection par critère temporel, claim exclusif si applicable, transaction atomique, idempotence). ADR-011 ne couvre pas ce service : il couvre les sagas de purge domaine (`SpaceDeleted`, `UserAnonymized`) ; la purge JTI est une opération d'infrastructure d'authentification, distincte du domaine.

Paramètres de configuration minimaux : fréquence d'exécution (ex. toutes les heures), délai de grâce optionnel (purger les JTI expirés depuis au moins N minutes pour absorber les skews d'horloge).

#### 3.6 Raccordement aux événements domaine

La denylist JTI doit être alimentée sur **deux événements domaine** (cf. `identity-access.md` §Événements domaine) :

**`AccountSuspended`** : un compte suspendu ne doit plus avoir de tokens actifs. Sans révocation, la suspension effective est différée jusqu'à l'expiration naturelle des tokens existants (au plus 15 min pour l'access token, au plus 7 jours pour le refresh token). Le handler de `AccountSuspended` appelle `ITokenDenylist.RevokeFamilyAsync(...)` sur toutes les familles actives de l'utilisateur.

**`UserAnonymized`** : un compte effacé ne doit plus avoir de tokens actifs. Un access token survivant à l'effacement constitue un accès résiduel RGPD — la fenêtre est courte (≤ 15 min) mais réelle. Le handler de `UserAnonymized` appelle `ITokenDenylist.RevokeFamilyAsync(...)` sur toutes les familles actives de l'utilisateur. **Ce raccordement doit être opéré dans la saga `UserAnonymized`** (ADR-011 §Saga `UserAnonymized`, ADR-012 §Conformité RGPD) : la révocation des tokens actifs est une étape de la saga, pas une opération asynchrone indépendante. Sans ce câblage, l'effacement RGPD laisse subsister un accès résiduel dont la durée maximale est la durée résiduelle de l'access token.

**Croisement ADR-012** : la révocation des tokens actifs à l'effacement de compte doit figurer comme étape de la saga `UserAnonymized` dans ADR-012. Ce point est un renvoi de cohérence : ADR-015 définit le mécanisme ; ADR-012 §Conséquences doit référencer l'appel à `ITokenDenylist` dans la saga.

---

### 4. Rate limiting (ferme F-06, CWE-307)

F-06 identifiait l'absence de rate limiting sur l'endpoint de validation de token GuestAccess. Le risque s'étend à tous les endpoints d'authentification, dont les vecteurs les plus critiques sont le login (credential stuffing) et le reset de mot de passe (abus de la fonction de réinitialisation).

#### 4.1 Périmètre des endpoints couverts

Le rate limiting s'applique aux endpoints suivants :

| Endpoint | Risque principal |
|---|---|
| `POST /auth/login` | Brute-force, credential stuffing |
| `POST /token/refresh` | Replay de refresh token compromis |
| `POST /auth/validate-token` (GuestAccess) | Énumération de tokens (F-06 original) |
| `POST /auth/password-reset/request` | Abus de la fonction de reset, spam |
| `POST /auth/password-reset/confirm` | Brute-force du token de reset |

#### 4.2 Dimension hybride

Le rate limiting est **hybride : par-IP ET par-compte**. La dimension par-IP seule est contournable par rotation d'IP (proxies, botnets, credential stuffing distribué). La dimension par-compte seule est insuffisante sur les endpoints publics (avant authentification, l'identifiant compte peut ne pas être connu). Les deux dimensions doivent être combinées avec un seuil indépendant pour chaque :

*Esquisse pré-implémentation : voir Annexe, bloc 7.*

Les valeurs exactes sont renvoyées à B1.5 (implémentation). Le principe — deux dimensions, seuils indépendants, cumulatifs — est arrêté ici.

> **Note sécurité** : B1.5 doit déclarer une borne supérieure de sécurité pour chaque seuil — des valeurs arbitrairement élevées (ex. 10 000 tentatives/min) neutraliseraient la protection. La justification des seuils retenus (équilibre protection/faux positifs) est exigée à l'implémentation.

L'implémentation utilise le middleware `RateLimiter` d'ASP.NET Core (disponible depuis .NET 7, politiques `FixedWindowRateLimiter` ou `SlidingWindowRateLimiter`) ou un composant équivalent en couche Infrastructure. Le choix d'implémentation est renvoyé à B1.5.

#### 4.3 Reset de mot de passe (CWE-640)

En complément du rate limiting, les tokens de reset de mot de passe respectent les contraintes suivantes.

> **Note** : CWE-640 n'a pas de finding F-xx d'origine dans l'audit — cette section est une décision PROACTIVE au-delà des quatre findings listés dans le header (F-02/F-06/F-10/F-11). Elle est incluse ici pour clore un vecteur d'attaque adjacent au périmètre audité.

- **TTL court** : durée de validité ≤ 15 min (borne dure normative) ; valeur exacte à fixer en B1.5 dans la plage 10–15 min.
- **Usage unique** : un token de reset est invalidé après son premier usage, qu'il ait abouti ou non.
- **Invalidation des tokens précédents** : une nouvelle demande de reset invalide tous les tokens de reset précédemment émis pour cet utilisateur. L'accumulation de tokens de reset actifs est un vecteur de fenêtre d'exploitation élargie.

---

### 5. Contrat applicatif observable

Les garanties de sécurité JWT/token sont exprimées derrière des **contrats applicatifs observables** (interfaces dans la couche Application), vérifiables par le test d'archi CI (→ B3.2), et non comme configuration d'infrastructure ASP.NET Identity opaque. Ce positionnement est identique à celui d'`IResourceAccessPolicy` dans ADR-014.

Les interfaces définies par cet ADR sont :

*Esquisse pré-implémentation : voir Annexe, bloc 8.*

Ces interfaces sont dans la couche Application (assemblage `Haversack.Application`). Leurs implémentations sont en couche Infrastructure (`Haversack.Infrastructure`). Une implémentation en mémoire de `ITokenDenylist` est tolérée en développement local uniquement et doit être annotée de manière structurée.

Le test d'archi CI doit vérifier :
- Que toute validation de token passe par `ITokenValidator` (aucun appel direct à `JwtSecurityTokenHandler.ValidateToken` dans les handlers).
- Que tout handler traitant un token refresh appelle `ITokenDenylist.IsRevokedAsync` avant d'émettre un nouveau token.
- Que les handlers de `AccountSuspended` et `UserAnonymized` appellent `ITokenDenylist.RevokeFamilyAsync`.

Ces vérifications sont renvoyées à B3.2 pour définition exhaustive.

---

## Alternatives considérées

**Configurer ASP.NET Identity sans contrat applicatif**

La configuration `PasswordOptions` et `TokenValidationParameters` dans `Program.cs` sans interfaces dédiées est la voie la moins coûteuse à court terme. Rejetée pour la même raison qu'ADR-014 a rejeté la vérification d'appartenance par discipline de handler : les garanties ne sont pas observables, pas testables par le test d'archi CI, et une régression par upgrade de package ou modification de configuration ne serait pas détectée. Le coût marginal d'un contrat d'interface est faible ; le bénéfice en observabilité et non-régression est structurel.

**Refresh token glissant**

La prolongation glissante de la durée du refresh token à chaque usage offre une expérience utilisateur plus fluide (reconnexion automatique tant que l'utilisateur est actif). Reporté post-MVP : la borne absolue de 7 jours est plus simple à implémenter et borne durement la fenêtre d'exploitation d'un token compromis. Le glissant est une optimisation d'UX, pas une exigence de sécurité.

**HS256 (algorithme symétrique) pour la signature JWT**

HS256 avec un secret partagé est plus simple à configurer qu'une paire de clés asymétrique. Rejeté : dans une topologie scale-out (ADR-004), tous les nœuds doivent partager le secret de signature — le secret devient un secret distribué dont la gestion est plus risquée qu'une clé publique diffusée. RS256/ES256 permet de diffuser la clé publique de validation sans exposer la clé privée de signature.

**Denylist en mémoire**

Une denylist en mémoire par instance est suffisante dans un déploiement mono-instance. Écartée comme implémentation de référence MVP : ADR-004 acte des sticky sessions impliquant plusieurs instances possibles. Une denylist mono-instance est tolérée en développement local, sous annotation structurée.

**Rate limiting par-IP seul**

Un rate limiting par-IP est plus simple à implémenter (pas de résolution de l'identifiant compte sur les endpoints pré-auth). Insuffisant : le credential stuffing distribué contourne le rate limiting par-IP en distribuant les tentatives sur de nombreuses adresses IP. La dimension par-compte borne les tentatives indépendamment de l'IP source.

---

## Conséquences

### Impact sur la couche Application

- `ITokenDenylist`, `ITokenValidator`, `IEmailVerificationPolicy` et `ITokenSigner` sont définis dans `Haversack.Application` (couche Application, domaine I&A).
- Les implémentations correspondantes sont dans `Haversack.Infrastructure`. L'implémentation de `ITokenDenylist` référence un store externe (Redis ou table SQL) conformément à ADR-004.
- Le Hosted Service de purge JTI est introduit dans `Haversack.Infrastructure` sur le pattern du Hosted Service de ADR-011 (idempotence, critère temporel, fréquence configurable).
- Les handlers de `AccountSuspended` et `UserAnonymized` (couche Application) doivent appeler `ITokenDenylist.RevokeFamilyAsync` dans leur traitement.
- La configuration `PasswordOptions` (longueur ≥ 12, complexité désactivée) et l'enregistrement de `IPasswordHasher<User>` (Argon2id) sont dans la registration DI de `Haversack.Infrastructure`.
- La politique OAuth (§2.3) est portée par le handler de liaison de compte, dans la couche Application.
- Le handler de définition d'un premier mot de passe sur compte fédéré (§1.4) est porté par la couche Application ; il vérifie `emailVerified = true`, déclenche la ré-authentification IdP requise (gap CWE-620), puis délègue l'écriture du hash à `IPasswordHasher<User>` (Argon2id).

### Croisement ADR-012 (révocation tokens dans saga UserAnonymized)

La révocation des tokens actifs à l'effacement de compte doit figurer comme étape de la saga `UserAnonymized`. ADR-012 §Conséquences devra être mis à jour pour référencer cet appel. Ce point est une dette de cohérence documentaire entre les deux ADR : ADR-015 définit le mécanisme, ADR-012 doit le référencer dans la saga.

### Croisement US-UC-10 (RB-10-10 — définition d'un premier mot de passe sur compte fédéré) — Résolu

Le §1.4 amende la portée de RB-10-10 : la réinitialisation d'un mot de passe inexistant reste N/A (inchangé), mais la définition d'un premier mot de passe sur compte fédéré est désormais autorisée, sous condition de ré-authentification IdP — mesure **préventive**, sans capacité de récupération si l'accès IdP est déjà perdu (§1.4). Le texte de RB-10-10 dans US-UC-10 (`docs/conception/user-stories/US-UC-10-compte-cloud.md`) a été mis à jour pour porter cette distinction. Ce point, du même ordre que le croisement ADR-012 ci-dessus, est **résolu** : ADR-015 définit la règle, US-UC-10 §RB-10-10 la référence désormais.

### Dettes nommées (non silencieuses)

| Dette | Nature | Ticket |
|---|---|---|
| Breach-check mot de passe (HaveIBeenPwned k-anonymity) | Fonctionnelle — complète la politique mdp | post-MVP |
| Refresh token glissant | UX — prolonge la session utilisateur active | post-MVP si souhaité |
| Valeurs de seuil rate limiting | Configuration — à fixer en implémentation | B1.5 |
| TTL exact du token de reset | Configuration — à fixer dans la plage 10–15 min (borne dure ≤ 15 min) | B1.5 |
| Fournisseur OAuth à email de relais/non canonique (ex. Apple Hide My Email) | Architecture — imposerait de revisiter la règle de liaison par email (RB-10-08, §2.2) | post-MVP, si fournisseur ajouté |
| Fraîcheur maximale et déclenchement exact de la ré-authentification IdP (§1.4, CWE-620) | Configuration — à fixer en implémentation | B1.5 |
| ~~Mise à jour du texte de RB-10-10 dans US-UC-10 (distinction réinitialisation N/A / premier mot de passe autorisé)~~ | Cohérence documentaire | **Résolu — US-UC-10 amendée** |
| Récupération post-perte d'accès IdP (flux *account-recovery* pour compte fédéré-only ayant perdu l'accès IdP sans mot de passe complémentaire déjà défini — §1.4) | Fonctionnelle — flux distinct de la mesure préventive du mot de passe complémentaire | hors MVP |
| Reclaim-in-place (§2.3) — opération de domaine dédiée (bascule `emailVerified`, neutralisation credential préexistant, liaison fédérée) | Fonctionnelle — complète §2.3, résolution `[À TRANCHER — à ratifier opérateur]` | B1.5 |
| Reclaim-in-place (§2.3) — facette RGPD (sort du contenu éventuel de la coquille non vérifiée évincée) | RGPD | Lot 14 |

### Résidu CWE-204 — Énumération de comptes (décision opérateur tracée)

Le message explicite d'inscription « adresse déjà associée à un compte » (US-UC-10 E1 — scénario E1 de US-10-01) est **conservé par choix produit** : il améliore l'UX en orientant l'utilisateur vers la connexion ou la réinitialisation de mot de passe. Ce choix crée un résidu d'énumération de comptes (CWE-204) : un observateur peut confirmer l'existence d'un email dans le système via ce message. Ce résidu est accepté et mitigé par le rate limiting (§4.1 — double dimension par-IP et par-compte) qui borne le volume d'énumération possible. Décision d'opérateur tracée, pas un oubli.

### Conformité conçue, non certifiée

Le raccordement de `ITokenDenylist.RevokeFamilyAsync` à `UserAnonymized` garantit qu'aucun token actif ne subsiste après l'effacement d'un compte. La fenêtre résiduelle maximale, sans ce raccordement, est la durée de l'access token (≤ 15 min). Ce raccordement ferme cette fenêtre au sens applicatif.

La fiabilité du claim d'email vérifié exposé par chaque fournisseur retenu (Google `email_verified`, Discord `verified` — §2.2) est une dépendance externe non vérifiable par les tests applicatifs Haversack, quel que soit le fournisseur. Le gate applicatif côté Haversack (§2.3 — interdiction de liaison OAuth vers un compte à email non vérifié) est, lui, testable en CI via provider OAuth mocké.

---

## Points à trancher

- **B1.5** — Implémentation des quatre volets (configuration `PasswordOptions` + `IPasswordHasher`, validation email + `IEmailVerificationPolicy`, cycle de vie des tokens + `ITokenDenylist` + `ITokenValidator`, rate limiting). Valeurs exactes des seuils de rate limiting et TTL du token de reset à arrêter à cette occasion. Inclut également la fraîcheur maximale et le mécanisme de déclenchement de la ré-authentification IdP requise pour la définition d'un premier mot de passe sur compte fédéré (§1.4, CWE-620).
- **B1.7** — Token SignalR (mode de transmission hors query-string) et vérification JTI sur connexion persistante (hors périmètre de cet ADR).
- **B1.10** — Contrat OpenAPI des endpoints d'auth (codes d'erreur 400/401/429, schémas de requête et réponse).
- **B3.2** — Définition des tests d'archi CI couvrant `ITokenDenylist`, `ITokenValidator`, `IEmailVerificationPolicy`, et les handlers `AccountSuspended` / `UserAnonymized`.
- ~~**ADR-012 §Conséquences** — Renvoi de cohérence : ajouter la révocation des tokens actifs (`ITokenDenylist.RevokeFamilyAsync`) comme étape de la saga `UserAnonymized`.~~ *(Résolu — ADR-012 §Conséquences contient déjà ce câblage.)*
- ~~**US-UC-10 §RB-10-10** — Renvoi de cohérence : mettre à jour le texte de RB-10-10 pour distinguer réinitialisation d'un mot de passe inexistant (N/A, inchangé) et définition d'un premier mot de passe sur compte fédéré (autorisée, §1.4).~~ *(Résolu — US-UC-10 §RB-10-10 porte désormais cette distinction.)*
- **Ajout futur d'un fournisseur OAuth à email de relais/non canonique** (ex. Apple Hide My Email) — imposerait de revisiter la règle de liaison par email (RB-10-08, §2.2). Dette nommée, non déclenchée par le périmètre MVP.
- **[À TRANCHER — à ratifier opérateur]** — Résolution du cas « email OAuth = compte préexistant non vérifié » (§2.3, *reclaim-in-place*) : reprise de la coquille non vérifiée par la preuve IdP (bascule `emailVerified` + neutralisation du credential préexistant + liaison fédérée) plutôt que création d'un doublon — le doublon n'est pas implémentable (invariant 1, email unique). Alternatives écartées : refus non-silencieux (réouvre CWE-204), email synthétique (motif ADR-007, inadapté à une identité vivante), suppression/déplacement de la coquille (cascade `UserDeleted` surdimensionnée, repli seulement). Facette RGPD → Lot 14 ; opération de domaine dédiée → B1.5.

---

## Croisements

| ADR | Nature du croisement |
|---|---|
| **ADR-007** | ADR parent RGPD/autorisation — §Compléments acte les seuils que cet ADR formalise |
| **ADR-001** | Frontière cloud/local — le mode local (IndexedDB, sans compte) est hors périmètre de tout ce qui est défini ici |
| **ADR-004** | Topologie scale-out (sticky sessions) — motive la denylist JTI externe et partagée, et le choix RS256/ES256 plutôt que HS256 |
| **ADR-011** | Pattern Hosted Service idempotent — réutilisé pour le service de purge JTI (structure, not scope) |
| **ADR-012** | Saga `UserAnonymized` — la révocation des tokens actifs doit être cadrée dans la saga ; renvoi de cohérence documentaire |
| **ADR-014** | Positionnement symétrique : contrats applicatifs observables (`IResourceAccessPolicy` ↔ `ITokenDenylist`/`ITokenValidator`/`IEmailVerificationPolicy`/`ITokenSigner`), test d'archi CI, non-régression structurelle |
| **US-UC-10** (RB-10-08, RB-10-10) | Source de la règle de matching par email canonique (RB-10-08) que §2.2/§2.3 généralisent par fournisseur ; RB-10-10 amendée pour distinguer réinitialisation (N/A) et définition d'un premier mot de passe (autorisée, §1.4, mesure préventive) — **résolu** |

---

## Compléments post-revue

*(Section réservée aux clarifications post-implémentation — vide à la date de l'ADR.)*

---

## Annexe — Esquisses pré-implémentation *(hors corps décisionnel)*

Les esquisses suivantes sont extraites du corps décisionnel conformément au finding CP-03 de l'audit conception (2026-06). Elles illustrent les décisions d'architecture, à confirmer à l'entrée en build. Elles ne font pas autorité sur le besoin — elles sont des illustrations pré-implémentation.

### Bloc 1 — §1.1 Configuration `PasswordOptions`

```
PasswordOptions.RequiredLength          = 12
PasswordOptions.RequireDigit            = false
PasswordOptions.RequireNonAlphanumeric  = false
PasswordOptions.RequireUppercase        = false
PasswordOptions.RequireLowercase        = false
```

### Bloc 2 — §1.2 Paramètres Argon2id

```
DegreeOfParallelism = 1
MemorySize          = 65536  (64 MiB)
Iterations          = 3
HashLength          = 32
```

### Bloc 3 — §2.1 Parcours email/mot de passe

```csharp
interface IEmailVerificationPolicy
{
    bool RequiresVerification(SensitiveOperation operation);
    Task<bool> IsVerifiedAsync(UserId userId);
}
```

### Bloc 4 — §2.3 Liaison OAuth à compte préexistant

```
liaison OAuth → compte préexistant : autorisée si et seulement si
  compte_préexistant.email_verified = true
  ET email(token_oauth) = compte_préexistant.email
```

### Bloc 5 — §3.2 Algorithme de signature JWT

```csharp
TokenValidationParameters.ValidAlgorithms = new[] { "RS256" }; // ou "ES256"
```

### Bloc 6 — §3.3 Rotation des refresh tokens

```csharp
interface ITokenDenylist
{
    Task RevokeAsync(Jti jti, DateTimeOffset expiresAt);
    Task RevokeFamilyAsync(FamilyId familyId);
    Task<bool> IsRevokedAsync(Jti jti);
}
```

### Bloc 7 — §4.2 Dimension hybride rate limiting

```
par-IP    : [seuil à définir en B1.5 — ex. 20 tentatives/min]
par-compte : [seuil à définir en B1.5 — ex. 10 tentatives/min]
```

### Bloc 8 — §5 Contrat applicatif complet

```csharp
// Couche Application — I&A
interface ITokenDenylist
{
    Task RevokeAsync(Jti jti, DateTimeOffset expiresAt);
    Task RevokeFamilyAsync(FamilyId familyId);
    Task<bool> IsRevokedAsync(Jti jti);
}

interface ITokenValidator
{
    Task<TokenValidationResult> ValidateAccessTokenAsync(string rawToken);
    Task<TokenValidationResult> ValidateRefreshTokenAsync(string rawToken);
}

interface IEmailVerificationPolicy
{
    bool RequiresVerification(SensitiveOperation operation);
    Task<bool> IsVerifiedAsync(UserId userId);
}

interface ITokenSigner
{
    string Sign(IDictionary<string, object> claims);
    void RotateKey(SigningKeyMaterial newKey);
    string ActiveKeyId { get; }
}
```

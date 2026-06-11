# ADR-013 — RGPD : données des joueurs invités (GuestAccess, Art. 6/13)

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session de cadrage P1)
- **Findings liés** : F-04, F-12, F-13, B1.6 (renvoi ADR-011 *Points à trancher*)

---

## Périmètre de cet ADR

Cet ADR couvre exclusivement les traitements liés au **cycle de vie des joueurs invités** (entités `GuestAccess`) : base légale des données collectées, obligation d'information Art. 13, rétention/purge autonome, posture sur les mineurs et la qualification sous-traitant/responsable.

**Hors périmètre de cet ADR** :
- Procédure d'anonymisation d'un compte utilisateur → **ADR-012**
- Règle F-08 (notes `PLAYER_PRIVATE` d'un compte supprimé accessible à un invité réassigné) → **ADR-012 §4** (déclencheur = suppression de compte)
- Mécanique de pseudonymisation `deleted-{id}@haversack.invalid` → **ADR-012 §1** (référencée ici, non redéfinie)
- Purge à la suppression de campagne (saga `CampaignDeleted`) → **ADR-011**
- Modèle d'autorisation API → **ADR-007**

---

## Contexte

Un joueur invité participe à une campagne sans créer de compte. Son seul identifiant collecté au point d'entrée est un `display_name` saisi librement. À ce `GuestAccess` peuvent être associés : un `character_id` (personnage joué), des notes `PLAYER_PRIVATE` créées pendant la session, et des métadonnées techniques (IP de connexion, timestamps, logs d'accès).

ADR-007 avait identifié la base légale et la durée de conservation des données invitées comme un item bloquant avant lancement EU (finding F-04, reclassé bloquant dans les Compléments post-revue ADR-007). ADR-011 a défini que les `guest_accesses` sont hard-deletés en passe 2 de la saga `CampaignDeleted` — mais cette purge ne suffit pas pour une campagne vivante de longue durée.

Deux questions restaient ouvertes :

1. Sur quelle base légale le `display_name` et les métadonnées techniques sont-ils traités ?
2. Quelle est la durée de rétention autonome des `guest_accesses` expirés, indépendamment de la purge de campagne ?

---

## Décision

### 1. Base légale (Art. 6 RGPD)

#### `display_name`

**Base légale retenue : intérêt légitime (Art. 6§1(f)).**

Le `display_name` est nécessaire à l'identification du joueur invité pendant la session et dans les contenus partagés de la campagne (personnages, notes, attribution des contributions). Sans cet identifiant, la coordination entre participants n'est pas fonctionnelle.

**Test de mise en balance** (résumé) :
- *Finalité* : identification en session, attribution des contributions dans le contexte de la campagne.
- *Nécessité* : le `display_name` est le seul identifiant collecté ; aucune donnée supplémentaire n'est exigée pour participer.
- *Proportionnalité* : la donnée est fournie volontairement par l'invité au moment de rejoindre la session ; aucun profilage n'en est tiré.
- *Attentes raisonnables* : un joueur rejoignant une session de jeu de rôle s'attend à être identifié par un nom pendant la partie.

> **Validation juridique requise** : la mise en balance formelle doit être conduite et documentée par un juriste avant le lancement EU. Voir section *Conformité conçue, non certifiée*.

#### Logs et métadonnées techniques (IP, timestamps, logs d'accès)

**Base légale retenue : intérêt légitime (Art. 6§1(f)) — finalité sécurité et débogage.**

Ces données sont nécessaires à la détection d'abus, au débogage des incidents, et à la sécurité du service. Durée de conservation : **30 jours maximum** à compter de la fin de la session (`GuestAccess.expires_at`). Passé ce délai, suppression ou anonymisation.

---

### 2. Information Art. 13 — au point de saisie du `display_name`

L'obligation d'information de l'Art. 13 RGPD s'applique au moment de la collecte de données, c'est-à-dire **au formulaire de saisie du `display_name`**, avant que l'invité entre en session. L'invité ne s'inscrivant pas, la mention ne peut pas être reportée à une page de paramètres de compte.

**Implémentation UI requise** :

- Un lien vers la politique de confidentialité est affiché au formulaire invité, en proximité immédiate du champ `display_name`.
- Une mention courte est affichée (exemple de formulation non normative : « Votre nom est utilisé pour vous identifier pendant la partie. [Politique de confidentialité] »).

**Contenu minimum de la mention** (conforme Art. 13§1 et §2) :

| Élément | Contenu |
|---|---|
| Identité du responsable de traitement | Nom et coordonnées de l'éditeur de Haversack |
| Finalité du traitement | Identification en session, attribution des contributions dans la campagne |
| Base légale | Intérêt légitime |
| Durée de conservation | (i) La ligne `guest_accesses` (dont le `display_name` de référence) est purgée 90 jours après `expires_at`. (ii) Les occurrences du `display_name` dans les contenus de campagne (notes, attributions) survivent tant que la campagne existe — leur sort suit le cycle de vie de la campagne (purge J+30 après soft-delete, saga `CampaignDeleted`). Les logs techniques (IP, timestamps) sont purgés à 30 jours après `expires_at`. |
| Droits et modalités d'exercice (Art. 13§2(b)) | Accès, rectification, effacement, opposition — exercice par email à l'adresse support dédiée ([adresse à définir en implémentation]). Un invité sans compte peut exercer ses droits en contactant ce canal en mentionnant son `display_name` et la campagne concernée. |
| Durée de conservation — logs techniques (IP, timestamps) | 30 jours maximum après `expires_at` du `GuestAccess` (Art. 13§2(a)) |
| Droit à l'opposition | L'invité peut s'opposer au traitement basé sur l'intérêt légitime |

---

### 3. Rétention autonome et purge des `guest_accesses` expirés

**Règle** : les `guest_accesses` dont `status = EXPIRED` ou dont `expires_at` est dépassé sont purgés (DELETE physique) **90 jours après `expires_at`**, indépendamment du cycle de vie de la campagne.

**Motif** : la purge de la saga `CampaignDeleted` (ADR-011, J+30 après soft-delete) ne suffit pas pour une campagne vivante de longue durée. Une campagne peut rester active plusieurs années ; des `guest_accesses` expirés depuis 18 mois conserveraient le `display_name` sans aucune finalité résiduelle. L'Art. 5§1(e) (limitation de la conservation) impose une purge fondée sur la durée, pas uniquement sur la suppression de la campagne.

**Implémentation** : un job de purge (Hosted Service .NET, distinct ou mutualisé avec le job de purge de campagnes) sélectionne les `guest_accesses WHERE expires_at ≤ now() - 90 jours` et les supprime. Les métadonnées techniques associées (logs d'accès, IP) sont supprimées ou anonymisées à **30 jours** après `expires_at` (délai plus court — voir §1), indépendamment de la purge de la ligne `guest_accesses` à 90 j. Un log d'accès contenant à la fois `guest_access_id` et `display_name` est purgé à 30 jours : la survie de la ligne `guest_accesses` jusqu'à 90 j ne justifie pas de conserver le log au-delà de 30 j.

**Interaction avec la saga `CampaignDeleted`** : si la campagne est purgée avant que le délai de 90 jours soit atteint, les `guest_accesses` sont supprimés en passe 2 de `CampaignDeleted` (ADR-011). Les deux mécanismes coexistent sans conflit : le premier à s'exécuter purge la ligne.

**Idempotence** : le job de purge des `guest_accesses` (Hosted Service) doit être idempotent (claim/reprise), par cohérence avec le mécanisme de claim du job `CampaignDeleted` défini dans ADR-011. Un crash entre la sélection et la suppression physique doit être récupérable sans double effet de bord.

---

### 4. Mineurs — posture MVP (Art. 8 RGPD)

**Posture retenue : service non destiné aux mineurs.**

Haversack n'est pas conçu comme un service destiné aux enfants. Les CGU mentionnent explicitement que le service est réservé aux personnes âgées d'au moins 16 ans.

**Implémentation au formulaire invité** : une case à cocher ou une mention explicite confirmant l'âge (« J'atteste avoir au moins 16 ans ») est affichée au formulaire de saisie du `display_name`. La case d'inscription au compte ne couvre pas les invités — un invité peut rejoindre une session sans s'inscrire et sans avoir coché la case d'inscription.

> **Validation juridique requise** : la qualification du service comme « non destiné aux enfants » au sens du RGPD et des droits nationaux applicables (notamment en ce qui concerne la définition d'un service « susceptible d'être utilisé par des enfants ») doit être vérifiée par un juriste avant le lancement EU. Si le service est requalifié comme « destiné aux enfants », l'attestation 16+ est insuffisante et un mécanisme de vérification de l'âge ou de consentement parental est requis. Voir section *Conformité conçue, non certifiée*.

---

### 5. Contenu créé par le MJ — posture sous-traitant (Art. 28 RGPD)

**Contexte** : le MJ (Game Master) saisit dans Haversack des contenus qui peuvent décrire des personnages référençant des personnes réelles (joueurs identifiables, attributs personnels fictifs mais projetés sur des individus réels). Ces données sont des données de tiers pour lesquelles le MJ — en tant qu'utilisateur de la plateforme — détermine les finalités et moyens du traitement.

**Posture retenue : sous-traitant (Art. 28).**

Haversack agit en qualité de sous-traitant vis-à-vis des MJ responsables de traitement pour les données saisies concernant des tiers identifiables. Cette qualification impose la mise en place d'un **Data Processing Agreement (DPA)** entre Haversack (sous-traitant) et les utilisateurs responsables de traitement (MJ).

**Ce que cet ADR ne fait pas** : le DPA est un document contractuel, non un ADR technique. Son contenu (obligations du sous-traitant, mesures de sécurité, sous-traitants ultérieurs, droits d'audit) est à définir par l'équipe juridique en lien avec la politique de confidentialité. Une référence à ce besoin figure dans la roadmap juridique.

> **Validation juridique requise** : la qualification sous-traitant/responsable doit être confirmée par un juriste. Dans certains cas (données de personnages entièrement fictifs sans lien avec des individus réels), la qualification peut être différente. Voir section *Conformité conçue, non certifiée*.

---

### 6. UC-11 — MJ propriétaire anonymisant son compte sur une campagne vivante

**Comportement MVP** : si le MJ propriétaire d'une campagne active supprime son compte (saga `UserAnonymized`), la campagne est conservée sous l'`id` anonymisé (ADR-011 — `campaigns.owner_id` pointe vers le `UserId` anonymisé, contrainte NOT NULL satisfaite). Aucun transfert de propriété automatique n'est effectué.

**Post-MVP** : un mécanisme de transfert de propriété forcé (désignation d'un successeur parmi les membres actifs, ou dissolution de la campagne) est prévu mais hors périmètre MVP.

**Impact sur les invités** : les `guest_accesses` de la campagne survivante restent actifs jusqu'à leur expiration naturelle. Leur purge suit la règle §3 (90 jours après `expires_at`). La suppression du compte du MJ ne déclenche pas la purge immédiate des `guest_accesses` de ses campagnes.

---

### 7. Renvois croisés

| Sujet | ADR de référence |
|---|---|
| Mécanique de pseudonymisation (`deleted-{id}@haversack.invalid`, ordre impératif) | **ADR-012 §1** |
| Règle F-08 (notes `PLAYER_PRIVATE` accessibles à un invité réassigné) | **ADR-012 §4** |
| Purge physique des `guest_accesses` à la suppression de campagne | **ADR-011** (saga `CampaignDeleted`, passe 2) |
| `display_name` invité : neutralisation éventuelle à la suppression de compte | Non applicable — les `guest_accesses` d'une campagne vivante ne sont pas traités par `UserAnonymized` (ADR-011, saga `UserAnonymized`) |
| UC-11 — MJ propriétaire anonymisant son compte sur une campagne vivante | **ADR-011** (saga `UserAnonymized`) + **ADR-013 §6** |

---

## Alternatives considérées

**Consentement explicite (Art. 6§1(a)) comme base légale pour le `display_name`**

Rejeté. Le consentement est révocable à tout moment. Si un invité retire son consentement, le `display_name` doit être supprimé immédiatement — ce qui détruirait rétrospectivement les attributions dans les contenus de campagne. L'intérêt légitime, sous réserve du test de mise en balance, est une base plus stable pour ce cas d'usage.

**Exécution du contrat (Art. 6§1(b)) comme base légale**

Non applicable. L'invité ne signe pas de contrat avec Haversack — il rejoint une session sans s'inscrire. La relation contractuelle s'établit entre le MJ (abonné) et Haversack, pas entre l'invité et Haversack.

**Purge des `guest_accesses` expirés uniquement à la suppression de campagne**

Rejetée. Insuffisant au regard de l'Art. 5§1(e) pour les campagnes de longue durée. Une campagne active depuis 2 ans peut contenir des `guest_accesses` expirés depuis 18 mois dont la finalité de conservation est nulle.

**Délai de rétention de 7 jours ou 30 jours après `expires_at`**

Écarté. Un délai trop court impose une contrainte opérationnelle forte sur le job de purge et peut poser des problèmes en cas de litige ou de débogage d'incidents récents. 90 jours est le délai retenu comme équilibre entre minimisation des données et opérabilité.

---

## Conséquences

- Le formulaire de saisie du `display_name` (front Angular) doit intégrer la mention Art. 13 et la case d'attestation d'âge avant le lancement EU.
- Un job de purge autonome des `guest_accesses` expirés (délai 90 jours après `expires_at`) doit être implémenté, distinct de la saga `CampaignDeleted`.
- La politique de confidentialité doit couvrir : base légale `display_name`, durée de rétention, droits des invités, posture sous-traitant pour les contenus MJ.
- La roadmap juridique doit inclure la rédaction du DPA (Data Processing Agreement) avant le lancement EU.
- Les métadonnées techniques (IP, logs d'accès) suivent une durée de rétention de 30 jours après `expires_at`, gérée par le même job de purge ou un job dédié.

---

## Points à trancher

- **Délai de purge des logs techniques** : valeur actée à 30 jours (§1 et §3) ; à valider en implémentation selon les besoins de débogage réels.
- **Interface d'exercice des droits pour les invités** : canal email support dédié retenu (mentionné dans la mention Art. 13 §2) ; l'adresse exacte et la procédure de traitement sont à définir lors de l'implémentation.
- **Périmètre exact du DPA** : contenu, sous-traitants ultérieurs (hébergeur, CDN), droits d'audit — à définir avec l'équipe juridique.

---

## Conformité conçue, non certifiée

Les choix documentés dans cet ADR sont cohérents avec le RGPD tel que lu et interprété lors de la conception. Trois points requièrent une **validation juridique avant tout lancement EU** :

1. **Qualification Art. 8** : Haversack est-il un service « destiné aux enfants » au sens du RGPD et des droits nationaux applicables ? Si oui, la case d'attestation 16+ au formulaire invité est insuffisante et un mécanisme de vérification de l'âge ou de consentement parental est requis.
2. **Qualification sous-traitant / responsable F-13 + DPA** : pour les contenus créés par les MJ décrivant des tiers identifiables, la qualification sous-traitant doit être vérifiée. Un DPA entre Haversack et les MJ responsables de traitement est nécessaire si cette qualification est confirmée.
3. **Test de mise en balance de l'intérêt légitime** : la mise en balance formelle pour le traitement du `display_name` invité (Art. 6§1(f)) n'a pas été conduite par un juriste. Ce test doit être documenté avant le lancement EU.

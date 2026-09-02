# Conditions générales d'utilisation — Haversack

> ## ⚠ BROUILLON — NON VALIDÉ JURIDIQUEMENT
>
> Ce document est un **projet de conditions générales d'utilisation**. Il n'est **pas opposable** et ne produit aucun effet juridique en l'état. Il consolide fidèlement les règles de fonctionnement du service telles que décidées dans le corpus de conception (cas d'usage, décisions d'architecture), en **langage utilisateur**. Chaque point relevant d'une qualification juridique — responsabilité, garanties, droit applicable, titularité de droits, disponibilité du service — est marqué `[À VALIDER — JURISTE]` et ne doit être ni publié ni invoqué avant révision par un professionnel du droit. Les points non couverts par le corpus de conception sont marqués `[À COMPLÉTER]`.

- **Statut** : Brouillon — à valider juriste
- **Portée** : Utilisateurs du service Haversack — Maîtres du Jeu (MJ) et joueurs, avec ou sans compte
- **Document lié** : [`politique-confidentialite.md`](./politique-confidentialite.md) — traitement des données personnelles

---

## 1. Objet du service

Haversack est un outil web destiné à assister le Maître du Jeu (MJ) de jeu de rôle sur table dans la préparation de ses campagnes et le pilotage de ses sessions de jeu. Il permet de structurer des scénarios, de centraliser des notes et documents de campagne, et de piloter une session en temps réel via une vue dédiée. Il permet également au MJ de partager sélectivement certaines informations avec ses joueurs, qui peuvent y accéder avec ou sans création de compte.

Haversack ne gère ni jets de dés, ni mécaniques de combat, ni règles de système de jeu : le service est agnostique au système pratiqué à la table.

## 2. Accès au service

Le service est accessible selon trois situations distinctes, dont les droits et les limites diffèrent.

### 2.1 Mode local, sans compte (MJ)

- Accès immédiat, sans inscription. Le MJ peut créer du contenu (lieux, personnages non-joueurs, scénarios, objets, notes) dès l'ouverture de l'application.
- Les données sont stockées exclusivement dans le stockage local du navigateur utilisé — aucune donnée n'est transmise au serveur en mode local.
- Fonctionnalités disponibles : création et organisation de contenu, documents, dossiers, vue de session, création de contenu à la volée, recherche locale, export d'un espace dans un format ouvert depuis les paramètres.
- Fonctionnalités indisponibles en mode local : partage avec des joueurs, synchronisation entre appareils, accès multi-appareil. Il n'existe pas de vue joueur ni d'accès invité en mode local — aucun joueur ne peut rejoindre une session ni y prendre de notes ; la vue de session sert au MJ seul.
- Limite de stockage : la capacité du navigateur de l'utilisateur (de l'ordre de 50 à 100 Mo en pratique) — cette limite n'est pas fixée par le service mais par le navigateur lui-même.
- Le service informe l'utilisateur, par deux bandeaux distincts et non bloquants, que (i) le navigateur ne garantit pas la conservation permanente des données stockées localement, et (ii) ces données ne sont pas chiffrées au repos — toute personne ayant accès au navigateur sur le même poste peut les consulter.
- L'utilisateur en mode local reste propriétaire de ses données locales et peut les exporter dans un format ouvert à tout moment depuis les paramètres, indépendamment de toute création de compte.

### 2.2 Compte cloud (MJ ou joueur)

Décrit en détail à la section 3 ci-dessous. La création d'un compte active la synchronisation cloud, le partage avec les joueurs et l'accès multi-appareil, dans les limites du palier souscrit (section 6).

### 2.3 Accès joueur invité, par lien temporaire, sans compte

Un joueur peut accéder à une session ou à une campagne sans créer de compte, via un lien partagé par le MJ.

- **Lien de session ponctuel** : le joueur saisit uniquement un nom d'affichage — aucune adresse de messagerie, aucun mot de passe. Il consulte les informations partagées par le MJ pendant la session. Son accès expire à la fin de la session, après une fenêtre de grâce de 24 heures.
- **Lien de campagne permanent** : ce lien nécessite que le joueur se connecte à un compte ou en crée un (section 3) ; l'accès obtenu est alors persistant, jusqu'à révocation par le MJ.
- Le joueur invité sans compte dispose des mêmes droits fonctionnels qu'un joueur authentifié dans le périmètre de son lien : il peut consulter les contenus qui lui sont partagés, sa fiche de personnage le cas échéant, et créer des notes personnelles.
- Le joueur invité peut créer un compte à tout moment pour transformer son accès ponctuel en accès permanent, sans perdre les notes déjà prises pendant la session en cours.
- Le nombre de joueurs pouvant accéder à une même session dépend du palier du compte du MJ (section 6).
- Le service informe l'invité, au moment où il saisit son nom d'affichage, de ce qui est conservé (nom d'affichage, notes personnelles), pour quelle durée, et de ce qu'il advient de ces données à la fin de son accès (voir [`politique-confidentialite.md`](./politique-confidentialite.md)).
- Le service est destiné aux personnes âgées d'au moins 16 ans. Une attestation d'âge est demandée à la création de compte et au formulaire d'accès invité. La qualification du service au regard des règles applicables aux services susceptibles d'être utilisés par des mineurs n'est pas tranchée à ce stade `[À VALIDER — JURISTE]` ; si elle devait l'être en ce sens, l'attestation d'âge actuelle serait insuffisante et un mécanisme de vérification ou de consentement parental serait requis.

## 3. Compte utilisateur

### 3.1 Création et authentification

Un compte peut être créé de deux façons :
- par adresse de messagerie et mot de passe ;
- via un fournisseur d'identité tiers — **Google** ou **Discord** — sans mot de passe défini à la création (un mot de passe complémentaire peut être ajouté ultérieurement pour ouvrir aussi la connexion par email/mot de passe).

L'adresse de messagerie est unique dans le système : une même adresse ne peut être associée qu'à un seul compte.

La validation de l'adresse de messagerie n'est pas requise pour accéder au service après inscription, mais elle est requise avant toute opération dite sensible : modification de l'adresse de messagerie, modification du mot de passe, liaison d'un fournisseur d'identité tiers, définition d'un premier mot de passe sur un compte créé via un fournisseur tiers, ou demande de suppression du compte. Un compte créé via un fournisseur d'identité tiers dont l'adresse est déjà réputée vérifiée par ce fournisseur n'a pas à la revalider.

### 3.2 Responsabilité de l'utilisateur sur ses identifiants

L'utilisateur est responsable de la confidentialité de son mot de passe et de l'accès à son compte. Le mot de passe n'est jamais détenu en clair par le service.

### 3.3 Mise à jour et suppression du compte

L'utilisateur peut mettre à jour son nom d'affichage et son mot de passe depuis sa page de profil, et demander la suppression de son compte. Le sort des données à la suppression du compte est décrit à la section 7 et détaillé dans [`politique-confidentialite.md`](./politique-confidentialite.md).

## 4. Contenu produit par le MJ

### 4.1 Fonctionnement technique

Le contenu créé par le MJ (documents, notes, scénarios, personnages non-joueurs, lieux, objets) est rattaché à l'espace dans lequel il est créé — un espace personnel privé par défaut, ou un espace de campagne / one-shot. Ce contenu n'est jamais visible d'un joueur tant que le MJ ne l'a pas explicitement partagé.

Le partage vers les joueurs se fait document par document : le MJ décide, pour chaque document, s'il est visible par les joueurs de l'espace ou s'il reste privé. Les notes marquées comme personnelles à un joueur (notes de session privées) appartiennent à leur auteur — MJ ou joueur — et ne sont jamais visibles par un autre participant, y compris le MJ pour les notes personnelles d'un joueur.

### 4.2 Titularité des droits sur le contenu

La question de la titularité des droits sur le contenu créé par l'utilisateur (droits d'auteur, droits voisins, ou toute autre qualification) n'est pas tranchée par ce document `[À VALIDER — JURISTE]`.

### 4.3 Contenu décrivant des tiers identifiables

Un contenu créé par un MJ peut décrire un personnage inspiré d'un joueur réel ou faire référence à des informations personnelles d'un participant. Dans ce cas, la qualification du service comme sous-traitant ou comme responsable de traitement au sens de l'article 28 du RGPD n'est pas tranchée `[À VALIDER — JURISTE]`. Si la qualification de sous-traitant est retenue, un contrat de sous-traitance (Data Processing Agreement) encadrera ce traitement — ce document n'en fixe aucune clause.

## 5. Contenu partagé aux joueurs et statut du joueur invité

Le MJ contrôle ce qui est visible par les joueurs de son espace, document par document. Un joueur, authentifié ou invité, ne voit que les documents que le MJ a explicitement partagés, et sa propre fiche de personnage le cas échéant.

Un joueur invité sans compte (`display_name` seul) a le même périmètre de consultation qu'un joueur authentifié dans les limites de son lien d'accès. Le sort de ses données personnelles à la fin de son accès (nom d'affichage, notes personnelles) est décrit dans [`politique-confidentialite.md`](./politique-confidentialite.md).

La fiche de personnage associée à un joueur — invité ou membre — appartient à l'espace de campagne : elle survit au retrait ou à l'expiration de l'accès du joueur et peut être réassociée à un autre participant. Les notes personnelles écrites par ce joueur ne sont, elles, jamais transférées à un autre participant.

## 6. Paliers et limites

Le service est proposé selon trois paliers.

| Palier | Compte requis | Ce que le palier permet | Limites |
|---|---|---|---|
| **Local** | Aucun | Préparation complète, vue de session, création de contenu à la volée, export d'un espace | Stockage limité à la capacité du navigateur (de l'ordre de 50 à 100 Mo en pratique) ; pas de partage aux joueurs ; un seul appareil |
| **Gratuit** | Email et mot de passe, ou fournisseur d'identité tiers | Synchronisation cloud, partage aux joueurs, accès multi-appareil | **3 espaces** de type campagne ou one-shot synchronisés en cloud (l'espace personnel n'est pas décompté de cette limite) ; **4 joueurs** disposant d'un accès par session ; **500 Mo** de stockage cloud |
| **Pro** | Abonnement payant | L'ensemble des fonctionnalités du palier gratuit, sans les limites de volume ou d'espaces | Espaces illimités ; joueurs illimités par session ; **5 Go et plus** de stockage cloud ; tarif cible d'environ **7 €/mois** ou **60 €/an** |

Ces valeurs sont celles consolidées dans le corpus de conception à la date du 2026-07-02 (`vision-produit.md` §3, `cahier-des-charges.md` §12.4, pour le tarif mensuel ; `UC-01-mode-local-sans-compte.md` §Modèle d'accès et de monétisation, pour la valeur annuelle) ; le tarif du palier Pro y est désigné comme un tarif cible, non encore contractualisé. Toute modification de ces valeurs avant publication doit être vérifiée auprès de ces mêmes sources.

Le passage du palier local au palier gratuit est déclenché par le besoin de partager avec des joueurs ou de sécuriser les données locales. Le passage du palier gratuit au palier Pro est déclenché par le dépassement de la limite de 3 espaces. Aucune fonctionnalité de préparation n'est restreinte ou dégradée en mode local pour inciter à la conversion.

### Gel des espaces en cas de retour au palier gratuit

Le corpus de conception documente un mécanisme applicable lorsqu'un utilisateur passant du palier Pro au palier gratuit se retrouve avec plus d'espaces de type campagne ou one-shot que ne le permet le palier gratuit : les espaces excédentaires (les plus récemment créés en premier) passent alors en lecture seule — leur contenu reste consultable mais ne peut plus être modifié, et aucune donnée n'est supprimée. Ce gel est automatique et notifié à l'utilisateur. Il est intégralement réversible : si l'utilisateur revient au palier Pro, les espaces gelés redeviennent modifiables sans perte de contenu.

**Ce mécanisme est documenté dans le corpus de conception comme fonctionnalité Post-MVP (spécifiés), hors périmètre de la première livraison du service (UC-15, classement `moscow.md` §UC-15).** Il n'est donc pas activé dans la version actuellement disponible du service. Cette section décrit le comportement prévu pour informer l'utilisateur de ce qui adviendra de son contenu si ce mécanisme est activé dans une version ultérieure ; elle sera revue lors de son activation effective.

## 7. Suppression de compte et sort des données

Un utilisateur peut demander la suppression de son compte depuis sa page de profil, après validation de son adresse de messagerie. Le service affiche préalablement les conséquences de cette suppression (sort des espaces dont il est propriétaire, sort de ses notes personnelles). La suppression est irréversible.

Le détail des traitements appliqués aux données personnelles à la suppression du compte — anonymisation, effacement physique, délais — est décrit dans [`politique-confidentialite.md`](./politique-confidentialite.md).

## 8. Disponibilité, évolutions, suspension du service

`[À VALIDER — JURISTE]` — Aucun engagement de disponibilité, de continuité, de préavis d'évolution ou de condition de suspension du service n'est énoncé par ce document. Ces rubriques doivent être rédigées par un juriste ; le corpus de conception ne fixe aucun engagement de service opposable à un utilisateur.

## 9. Responsabilité et garanties

`[À VALIDER — JURISTE]`

## 10. Droit applicable et juridiction compétente

`[À VALIDER — JURISTE]`

## 11. Traçabilité

Ce document consolide fidèlement les sources suivantes du corpus de conception. Il n'invente aucune règle qui n'y figure pas.

- [`docs/conception/besoin/usecases/UC-01-mode-local-sans-compte.md`](../../conception/besoin/usecases/UC-01-mode-local-sans-compte.md) — mode local, bandeaux durabilité/confidentialité, export.
- [`docs/conception/besoin/usecases/UC-09-acces-session-joueur.md`](../../conception/besoin/usecases/UC-09-acces-session-joueur.md) — accès joueur invité, statut invité, sort des données invité.
- [`docs/conception/besoin/usecases/UC-10-compte-cloud.md`](../../conception/besoin/usecases/UC-10-compte-cloud.md) — création de compte, authentification, suppression de compte.
- [`docs/conception/besoin/usecases/UC-11-gerer-membres-espace-partage.md`](../../conception/besoin/usecases/UC-11-gerer-membres-espace-partage.md) — invitations, membres, personnages, retrait.
- [`docs/conception/besoin/usecases/UC-15-gel-espaces-downgrade-tier.md`](../../conception/besoin/usecases/UC-15-gel-espaces-downgrade-tier.md) — gel/dégel des espaces au changement de palier (Post-MVP (spécifiés), classement `moscow.md` §UC-15).
- [`docs/conception/besoin/vision/vision-produit.md`](../../conception/besoin/vision/vision-produit.md) §3 — modèle de monétisation.
- [`docs/context/cahier-des-charges.md`](../../context/cahier-des-charges.md) §12.4 — valeurs chiffrées consolidées des paliers.
- [`docs/architecture/decisions/ADR-005-modele-monetisation.md`](../../architecture/decisions/ADR-005-modele-monetisation.md) — trace historique du modèle de paliers.
- [`docs/architecture/decisions/ADR-013-rgpd-donnees-invites.md`](../../architecture/decisions/ADR-013-rgpd-donnees-invites.md) — statut du joueur invité, information Art. 13, posture sous-traitant Art. 28, attestation d'âge 16+.
- [`docs/architecture/decisions/ADR-015-securite-authentification-mvp.md`](../../architecture/decisions/ADR-015-securite-authentification-mvp.md) — fournisseurs d'identité tiers (Google, Discord), validation de l'adresse de messagerie.
- [`docs/securite/conformite/cadrage-validation-pre-lancement-eu.md`](./cadrage-validation-pre-lancement-eu.md) — liste consolidée des points de qualification juridique en attente (Art. 8, Art. 28, intérêt légitime).

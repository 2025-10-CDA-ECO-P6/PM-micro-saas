# UC-10 — Créer un compte et synchroniser dans le cloud

> **MoSCoW : Must Have** — UC-08 (partage joueurs) nécessite un compte ; UC-10 est
> le prérequis du second pilier produit (raisonnement de priorisation : voir MoSCoW).
> Le point d'entrée reste UC-01 (mode local sans compte) : UC-10 couvre le passage au cloud
> — sauvegarde, partage avec les joueurs, accès multi-device.

## Acteur principal

MJ ou Joueur

## Acteurs secondaires

Aucun.

## Objectif

Permettre à un utilisateur de créer un compte pour activer la synchronisation cloud, le partage avec les joueurs et l'accès multi-device.

## Contexte

Un MJ qui a commencé à utiliser l'application en mode local (UC-01) veut soit sauvegarder ses données dans le cloud, soit partager des informations avec ses joueurs (UC-08), soit accéder à ses campagnes depuis un autre appareil. Ces trois besoins déclenchent naturellement la création de compte.

Un joueur invité sans compte (UC-09) peut créer un compte pour rejoindre une campagne de façon permanente et conserver ses notes entre sessions.

La création de compte ne doit pas être imposée au démarrage — elle est proposée en réponse à un besoin concret.

Un parcours attendu est celui d'un MJ qui a conduit des sessions en mode local, crée son compte, migre sa campagne avec tout son historique de session (sessions passées, notes, documents épinglés, résumés), puis invite ses joueurs pour des sessions futures dans cette même campagne. La continuité entre ses sessions solo passées et ses sessions futures avec joueurs est ainsi garantie.

## Besoin utilisateur

Le MJ veut activer la sauvegarde cloud ou le partage joueurs. Le joueur veut accéder à l'historique de campagne entre les sessions. Les deux veulent créer un compte rapidement, sans friction.

## Déclencheur

- Le MJ tente de partager une information avec ses joueurs (UC-08) depuis le mode local.
- Le MJ veut accéder à ses campagnes depuis un autre appareil.
- Le bandeau "données locales" invite le MJ à sauvegarder dans le cloud.
- Un joueur invité sans compte veut rejoindre une campagne de façon permanente.

## Préconditions

- Aucune pour l'inscription.
- Un compte existant pour la connexion.
- Des données locales peuvent exister (UC-01) ; si c'est le cas, un gate de reconnaissance est présenté et la migration est déclenchée uniquement après confirmation explicite (ADR-016 §4).

## Scénario nominal — Inscription depuis le mode local

1. Le MJ en mode local déclenche une action nécessitant un compte (partage, multi-device) ou clique sur l'invite de sauvegarde cloud.
2. L'application propose la création de compte en context (sans rediriger vers une page dédiée).
3. Le MJ renseigne :
   - adresse email ;
   - nom d'affichage ;
   - mot de passe.
4. Le système crée le compte (tier gratuit).
5. L'application présente les données locales détectées (titres des campagnes, volume, date) et affiche pour chaque campagne son historique de session : les sessions terminées, les notes de session attachées à chaque session, les documents épinglés, les résumés. Si une campagne contient une session en cours (statut LIVE), le gate signale au MJ qu'elle doit être clôturée avant que la campagne puisse migrer. L'application demande une confirmation explicite avant l'import (gate de reconnaissance anti-appropriation — voir [ADR-016](../../architecture/decisions/ADR-016-serialisation-locale-migration.md) §4).
6. Après confirmation, les données locales (campagnes, documents, dossiers et tout leur historique de session — sessions terminées, notes de session, documents épinglés, résumés) sont importées vers le cloud ; la migration est traitée campagne par campagne, tout-ou-rien par campagne.
7. Le MJ retrouve son espace de travail intact pour les campagnes importées avec succès, maintenant synchronisé. Son historique de session est retrouvé complet : les sessions passées sont consultables, les notes et les documents épinglés sont en place. Seule la configuration des panneaux de la vue session doit être recréée par le MJ.

## Scénario nominal — Inscription sans données locales

1. L'utilisateur accède à la page d'inscription.
2. Il renseigne email, nom d'affichage, mot de passe.
3. Le système crée le compte.
4. L'utilisateur est redirigé vers l'écran de création de campagne.

## Scénario nominal — Connexion

1. L'utilisateur accède à la page de connexion.
2. Il saisit son email et son mot de passe.
3. Le système valide les identifiants.
4. L'utilisateur est redirigé vers son tableau de bord.

## Scénario nominal — Connexion via Google OAuth

1. L'utilisateur clique sur "Continuer avec Google" depuis la page d'inscription ou de connexion.
2. Il s'authentifie via le flux OAuth Google.
3. Si c'est un premier accès : le système crée automatiquement un compte (tier gratuit) avec l'email et le nom d'affichage Google. Aucun mot de passe n'est défini.
4. Si un compte existe déjà avec cet email : le système connecte l'utilisateur à ce compte existant.
5. Si des données locales existent, le gate de reconnaissance est présenté (titres, volume, date) et la migration ne démarre qu'après confirmation explicite — ADR-016 §4.
6. L'utilisateur est redirigé vers son tableau de bord.

## Scénarios alternatifs

### A1 — Réinitialisation du mot de passe

L'utilisateur demande un lien de réinitialisation. Le système envoie un email avec un token temporaire. L'utilisateur choisit un nouveau mot de passe.

### A2 — Mise à jour du profil

L'utilisateur modifie son nom d'affichage ou son mot de passe depuis la page profil.

### A3 — Joueur créant un compte depuis un lien d'invitation

Un joueur invité sans compte clique sur un lien d'invitation, crée un compte et rejoint la campagne en une seule action. Son accès invité est transformé en accès membre.

### A4 — Suppression du compte (droit à l'effacement RGPD)

1. L'utilisateur accède à la page profil et demande la suppression de son compte.
2. Le système affiche les conséquences :
   - les campagnes dont l'utilisateur est propriétaire (`ownerId`) seront orphelines — il doit d'abord transférer leur propriété ou accepter leur suppression en cascade ;
   - ses notes privées (personnelle joueur) seront supprimées physiquement, ainsi que les notes privées qu'il a créées rattachées aux personnages qu'il incarnait dans les campagnes vivantes ;
   - les autres données liées (participations, memberships) seront anonymisées.
3. L'utilisateur confirme la suppression.
4. Le système exécute la séquence :
   a. Suppression physique des `note de session` avec `visibility = personnelle joueur` créées par l'utilisateur supprimé.
      Cette suppression emporte également les notes privées rattachées aux personnages que cet utilisateur incarnait
      dans les campagnes vivantes, afin d'éviter qu'une note résiduelle ne soit exposée à un futur joueur réassocié au personnage.
      La fiche du personnage elle-même survit et reste ré-associable à un autre joueur pour préserver la continuité de campagne.
   b. Anonymisation des données nominatives dans les autres tables (nom d'affichage remplacé par `[Compte supprimé]`).
   c. Suppression ou transfert des campagnes dont l'utilisateur est propriétaire.
   d. Désactivation du compte (`status = DELETED`).
5. L'utilisateur est déconnecté et redirigé vers la page d'accueil.

**Note MVP** : si l'utilisateur est propriétaire de campagnes avec des membres actifs, le transfert de propriété est hors MVP. Dans le MVP, la suppression est bloquée tant que l'utilisateur a des campagnes avec d'autres membres — un message explicite lui demande de les gérer d'abord.

## Exceptions

### E1 — Email déjà utilisé

Le système refuse la création et informe l'utilisateur que l'adresse est déjà associée à un compte.

### E2 — Identifiants invalides

Le système affiche un message d'erreur générique sans préciser si c'est l'email ou le mot de passe qui est incorrect (sécurité).

### E3 — Lien de réinitialisation expiré

Le système informe l'utilisateur que le lien n'est plus valide et lui propose d'en générer un nouveau.

### E4 — Suppression bloquée (propriétaire de campagnes actives)

L'utilisateur tente de supprimer son compte mais est propriétaire de campagnes avec des membres actifs. Le système bloque la suppression et indique les campagnes concernées. L'utilisateur doit d'abord exclure tous les membres ou transférer la propriété (post-MVP) avant de pouvoir supprimer son compte.

### E5 — Échec ou interruption de la migration local→cloud

Si la migration des données locales vers le cloud échoue ou est interrompue (erreur réseau, le système ne répond pas dans le délai attendu, fermeture du navigateur en cours de migration), le traitement est tout-ou-rien **par campagne** : une campagne importée avec succès est confirmée ; une campagne en échec est rejetée. Les données locales des campagnes rejetées — campagnes, documents, dossiers et tout leur historique de session (sessions terminées, notes de session, documents épinglés, résumés) — sont conservées intégralement dans le navigateur. Un rapport de rejets est présenté au MJ, indiquant les raisons par campagne (l'identifiant de campagne cible est déjà occupé, propriétés de document invalides, type inconnu, version du format de données non reconnue). Le compte est créé mais reste en état « migration en attente » pour les campagnes non importées : le MJ peut reprendre la migration depuis son espace de travail sans perte de données. Aucune donnée locale n'est supprimée avant que la migration ne soit confirmée pour la campagne concernée. — Voir [ADR-016](../../architecture/decisions/ADR-016-serialisation-locale-migration.md) §4.

## Postconditions

### Inscription / Connexion
- L'utilisateur dispose d'un compte actif.
- L'utilisateur est authentifié.
- L'utilisateur peut accéder à son tableau de bord.

### Suppression de compte
- Le compte est marqué `status = DELETED`.
- Les données nominatives sont anonymisées.
- Les notes privées (personnelle joueur) créées par l'utilisateur supprimé sont supprimées physiquement, y compris celles rattachées aux personnages qu'il incarnait.
- Les fiches de personnages survivent et restent ré-associables à d'autres joueurs.
- L'utilisateur est déconnecté.

## Données manipulées

### Compte utilisateur

- Email (unique)
- Nom d'affichage
- Mot de passe (jamais détenu en clair — sa protection est assurée par l'infrastructure)
- Statut du compte

## Règles métier

- L'email est unique dans le système.
- Le mot de passe est hashé en infrastructure — le compte utilisateur ne le connaît pas.
- Un utilisateur ne porte aucun rôle global. Le rôle MJ ou Joueur est défini dans chaque campagne. Tout utilisateur authentifié peut créer une campagne et en devenir le MJ.
- Un compte suspendu ou supprimé ne peut pas se connecter.
- **Migration des données locales vers le cloud** :
  - La migration d'une campagne emporte tout son historique de session : sessions terminées, notes de session, documents épinglés et résumés. Rien de cet historique n'est perdu à la migration.
  - Une session en cours (statut LIVE) ne migre pas en l'état : elle doit être clôturée avant la migration. Le gate de reconnaissance signale au MJ toute session en cours et indique qu'elle doit être clôturée pour que la campagne puisse migrer.
  - La configuration de la vue session (choix des panneaux affichés) n'est pas reprise : elle est recréée et le MJ la reconfigure.
- **RGPD — droit à l'effacement** :
  - La suppression d'un compte déclenche l'anonymisation des données nominatives dans toutes les tables.
  - Les `note de session` avec `visibility = personnelle joueur` créées par l'utilisateur supprimé sont supprimées physiquement.
    Cela emporte également les notes privées rattachées aux personnages que cet utilisateur incarnait dans les campagnes vivantes,
    afin d'éviter qu'une note résiduelle ne soit exposée à un futur joueur réassocié au personnage.
    La fiche du personnage elle-même survit et reste ré-associable à un autre joueur pour préserver la continuité de campagne.
  - Les autres contenus créés (documents, notes MJ, PNJ) restent attachés à la campagne sous identité anonymisée — ils appartiennent à la campagne, pas à l'individu.
  - La suppression est irréversible.
  - Un utilisateur propriétaire de campagnes avec des membres actifs ne peut pas supprimer son compte tant qu'il n'a pas géré ces campagnes (MVP : exclusion des membres ; post-MVP : transfert de propriété).

## Critères d'acceptation

- Un utilisateur peut créer un compte avec email et mot de passe.
- Un utilisateur peut se connecter avec ses identifiants.
- Un utilisateur peut réinitialiser son mot de passe par email.
- Un email déjà utilisé est refusé à l'inscription.
- Un utilisateur peut mettre à jour son nom d'affichage.
- Un joueur invité peut créer un compte et rejoindre la campagne en une action.
- Un utilisateur nouvellement inscrit peut immédiatement créer une campagne ou rejoindre une campagne existante via invitation.
- Le gate de reconnaissance présente l'historique de session détecté par campagne (sessions terminées, notes de session, documents épinglés, résumés) ; toute session en cours (LIVE) est signalée avec indication qu'elle doit être clôturée avant que la campagne puisse migrer.
- Après migration réussie, l'historique de session des campagnes migrées est retrouvé intact dans l'espace de travail cloud.
- Un utilisateur peut demander la suppression de son compte depuis sa page profil.
- La suppression est bloquée si l'utilisateur est propriétaire de campagnes avec des membres actifs.
- Après suppression : le compte est désactivé, les données nominatives sont anonymisées, les notes privées de l'utilisateur sont supprimées physiquement.

## Questions à valider en interview

- Les MJ sont-ils à l'aise avec un compte email classique ou préfèrent-ils une connexion sociale (Google) ?
- La création de compte est-elle un frein perçu pour les joueurs ?
- Faut-il une validation par email à l'inscription pour le MVP ?
- Quelle politique d'anonymisation est suffisante pour le RGPD dans ce contexte (remplacer le nom, ou supprimer les enregistrements) ?

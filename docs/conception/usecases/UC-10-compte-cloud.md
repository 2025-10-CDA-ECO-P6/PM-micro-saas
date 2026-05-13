# UC-10 — Créer un compte et synchroniser dans le cloud

> **MoSCoW : Should Have** — Le point d'entrée de l'application est désormais le mode local sans compte (UC-01).
> UC-10 couvre le passage au cloud : sauvegarde, partage avec les joueurs, accès multi-device.

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
- Des données locales peuvent exister (UC-01) et doivent être migrées automatiquement.

## Scénario nominal — Inscription depuis le mode local

1. Le MJ en mode local déclenche une action nécessitant un compte (partage, multi-device) ou clique sur l'invite de sauvegarde cloud.
2. L'application propose la création de compte en context (sans rediriger vers une page dédiée).
3. Le MJ renseigne :
   - adresse email ;
   - nom d'affichage ;
   - mot de passe.
4. Le système crée le compte (tier gratuit).
5. Les données locales (campagnes, documents, dossiers) sont migrées vers le cloud.
6. Le MJ retrouve son espace de travail intact, maintenant synchronisé.

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

## Scénarios alternatifs

### A1 — Réinitialisation du mot de passe

L'utilisateur demande un lien de réinitialisation. Le système envoie un email avec un token temporaire. L'utilisateur choisit un nouveau mot de passe.

### A2 — Mise à jour du profil

L'utilisateur modifie son nom d'affichage ou son mot de passe depuis la page profil.

### A3 — Joueur créant un compte depuis un lien d'invitation

Un joueur invité sans compte clique sur un lien d'invitation, crée un compte et rejoint la campagne en une seule action. Son accès GuestAccess est migré en CampaignMembership.

### A4 — Suppression du compte (droit à l'effacement RGPD)

1. L'utilisateur accède à la page profil et demande la suppression de son compte.
2. Le système affiche les conséquences :
   - les campagnes dont l'utilisateur est propriétaire (`ownerId`) seront orphelines — il doit d'abord transférer leur propriété ou accepter leur suppression en cascade ;
   - ses accès aux notes personnelles (`PLAYER_PRIVATE`) seront retirés ;
   - les autres données liées (participations, memberships) seront anonymisées.
3. L'utilisateur confirme la suppression.
4. Le système exécute la séquence :
   a. Conservation des `LiveNote` avec `visibility = PLAYER_PRIVATE` liées à un `ownerCharacterId`.
      Le compte supprimé perd l'accès, mais les notes restent attachées au personnage pour préserver
      la continuité de campagne.
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

## Postconditions

### Inscription / Connexion
- L'utilisateur dispose d'un compte actif.
- L'utilisateur est authentifié.
- L'utilisateur peut accéder à son tableau de bord.

### Suppression de compte
- Le compte est marqué `status = DELETED`.
- Les données nominatives sont anonymisées.
- Le compte supprimé ne peut plus accéder aux notes `PLAYER_PRIVATE`.
- Les notes `PLAYER_PRIVATE` restent attachées à leur `ownerCharacterId`.
- L'utilisateur est déconnecté.

## Données manipulées

### Compte utilisateur

- Email (unique)
- Nom d'affichage
- Mot de passe (hashé, géré par ASP.NET Identity en infrastructure)
- Statut du compte

## Règles métier

- L'email est unique dans le système.
- Le mot de passe est hashé en infrastructure — l'entité domaine `User` ne le connaît pas.
- `User` ne porte aucun rôle global. Le rôle MJ ou Joueur est défini par `CampaignMembership.role` dans chaque campagne. Tout utilisateur authentifié peut créer une campagne et en devenir le MJ.
- Un compte suspendu ou supprimé ne peut pas se connecter.
- **RGPD — droit à l'effacement** :
  - La suppression d'un compte déclenche l'anonymisation des données nominatives dans toutes les tables.
  - Les `LiveNote` avec `visibility = PLAYER_PRIVATE` sont liées au `CharacterId`, pas au compte.
    La suppression du compte retire l'accès de l'utilisateur mais ne supprime pas automatiquement
    ces notes de personnage.
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
- Un utilisateur peut demander la suppression de son compte depuis sa page profil.
- La suppression est bloquée si l'utilisateur est propriétaire de campagnes avec des membres actifs.
- Après suppression : le compte est désactivé, les données nominatives sont anonymisées, l'accès aux notes `PLAYER_PRIVATE` est retiré.

## Questions à valider en interview

- Les MJ sont-ils à l'aise avec un compte email classique ou préfèrent-ils une connexion sociale (Google) ?
- La création de compte est-elle un frein perçu pour les joueurs ?
- Faut-il une validation par email à l'inscription pour le MVP ?
- Quelle politique d'anonymisation est suffisante pour le RGPD dans ce contexte (remplacer le nom, ou supprimer les enregistrements) ?

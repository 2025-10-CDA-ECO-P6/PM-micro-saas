# UC-00 — S'inscrire et gérer son compte

## Acteur principal

MJ ou Joueur

## Acteurs secondaires

Aucun.

## Objectif

Permettre à un utilisateur de créer un compte, de s'authentifier et de gérer son profil pour accéder à l'application.

## Contexte

Haversack est un SaaS nécessitant une authentification pour accéder aux campagnes. Le MJ a besoin d'un compte persistant pour créer et gérer ses campagnes. Le joueur peut accéder sans compte (GuestAccess) mais un compte lui permet un accès durable et la gestion de ses notes personnelles.

## Besoin utilisateur

L'utilisateur veut créer un compte rapidement et s'y connecter sans friction. Il veut également pouvoir réinitialiser son mot de passe et mettre à jour ses informations de profil.

## Déclencheur

L'utilisateur découvre l'application et souhaite l'utiliser, ou tente d'accéder à une ressource protégée.

## Préconditions

- Aucune pour l'inscription.
- Un compte existant pour la connexion.

## Scénario nominal — Inscription

1. L'utilisateur accède à la page d'inscription.
2. Il renseigne :
   - adresse email ;
   - nom d'affichage ;
   - mot de passe.
3. Il valide l'inscription.
4. Le système crée le compte avec `role = GM` par défaut.
5. L'utilisateur est redirigé vers son tableau de bord.

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
   - les notes personnelles (`PLAYER_PRIVATE`) seront définitivement supprimées ;
   - les autres données liées (participations, memberships) seront anonymisées.
3. L'utilisateur confirme la suppression.
4. Le système exécute la séquence :
   a. Suppression physique des `LiveNote` avec `visibility = PLAYER_PRIVATE` et `authorId = userId`.
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
- Les notes `PLAYER_PRIVATE` sont définitivement supprimées.
- L'utilisateur est déconnecté.

## Données manipulées

### Compte utilisateur

- Email (unique)
- Nom d'affichage
- Mot de passe (hashé, géré par ASP.NET Identity en infrastructure)
- Rôle (`GM` par défaut)
- Statut du compte

## Règles métier

- L'email est unique dans le système.
- Le mot de passe est hashé en infrastructure — l'entité domaine `User` ne le connaît pas.
- Un utilisateur nouvellement inscrit a le rôle `GM` par défaut.
- Le rôle dans une campagne précise est distinct du rôle global (`MemberRole` dans Campaign Management).
- Un compte suspendu ou supprimé ne peut pas se connecter.
- **RGPD — droit à l'effacement** :
  - La suppression d'un compte déclenche l'anonymisation des données nominatives dans toutes les tables.
  - Les `LiveNote` avec `visibility = PLAYER_PRIVATE` créées par l'utilisateur sont supprimées physiquement (données personnelles non accessibles à des tiers, suppression sans ambiguïté).
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
- Un utilisateur peut demander la suppression de son compte depuis sa page profil.
- La suppression est bloquée si l'utilisateur est propriétaire de campagnes avec des membres actifs.
- Après suppression : le compte est désactivé, les données nominatives sont anonymisées, les notes `PLAYER_PRIVATE` sont supprimées physiquement.

## Questions à valider en interview

- Les MJ sont-ils à l'aise avec un compte email classique ou préfèrent-ils une connexion sociale (Google) ?
- La création de compte est-elle un frein perçu pour les joueurs ?
- Faut-il une validation par email à l'inscription pour le MVP ?
- Quelle politique d'anonymisation est suffisante pour le RGPD dans ce contexte (remplacer le nom, ou supprimer les enregistrements) ?

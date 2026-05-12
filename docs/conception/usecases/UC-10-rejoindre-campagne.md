# UC-10 — Rejoindre une campagne ou une session

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre à un joueur de rejoindre facilement une campagne ou une session, avec ou sans compte.

## Contexte

Le produit étant choisi par le MJ, l'expérience joueur doit être peu contraignante. Une obligation de création de compte peut devenir un frein à l'adoption.

## Besoin utilisateur

Le joueur veut accéder rapidement à la campagne, à sa fiche et aux informations utiles sans configuration complexe.

## Déclencheur

Le MJ invite un joueur à rejoindre une campagne ou une session.

## Préconditions

- Une campagne existe.
- Le MJ a généré un lien ou un code d'invitation.

## Scénario nominal sans compte

1. Le MJ génère un lien d'invitation.
2. Le joueur ouvre le lien.
3. Le système affiche une page de rejoindre.
4. Le joueur saisit un pseudo.
5. Le joueur rejoint la campagne ou la session comme invité.
6. Le MJ associe l'accès invité à un personnage existant ou en crée un.
7. Le joueur accède à sa fiche et aux informations partagées.

## Scénario nominal avec compte

1. Le joueur ouvre le lien d'invitation.
2. Il se connecte ou crée un compte.
3. Il rejoint la campagne.
4. Le MJ l'associe à un personnage.
5. L'accès est conservé durablement sur son compte.

## Scénarios alternatifs

### A1 — Lien expiré

Le joueur ouvre un lien expiré. Le système affiche un message d'erreur et demande un nouveau lien.

### A2 — Joueur en attente de validation

Le joueur demande à rejoindre la campagne, mais le MJ doit valider son accès.

### A3 — Accès direct à une session

Le joueur rejoint uniquement la session en cours, sans accès complet à la campagne.

## Exceptions

### E1 — Campagne introuvable

Le lien ne correspond à aucune campagne active.

### E2 — Accès refusé par le MJ

Le MJ refuse ou retire l'accès du joueur.

## Postconditions

- Le joueur est associé à la campagne ou à la session.
- Le joueur peut accéder aux informations autorisées.
- Le MJ peut gérer son association à un personnage.

## Données manipulées

### Invitation

- Identifiant unique
- Code ou token
- Campagne associée
- Date d'expiration
- Type d'accès
- Statut

### Participant

- Pseudo
- Compte éventuel
- Rôle
- Personnage associé

## Règles métier

- Le MJ contrôle les invitations.
- Un joueur invité sans compte a un accès limité ou temporaire, sécurisé par token.
- Pour agir comme un joueur complet, l'accès invité doit être associé à un `CharacterId`.
- Les données personnelles joueur sont liées au personnage, pas au GuestAccess temporaire.
- Un compte joueur permet un accès persistant.
- Le joueur ne voit que les informations partagées avec lui.

## Critères d'acceptation

- Le MJ peut générer un lien d'invitation.
- Un joueur peut rejoindre avec un pseudo sans compte.
- Le MJ peut associer un joueur à un personnage.
- Un joueur invité peut consulter sa fiche et les notes partagées.
- Un lien expiré ne permet pas l'accès.

## Questions à valider en interview

- La création de compte est-elle un frein pour les joueurs ?
- Les MJ préfèrent-ils inviter par lien, code ou email ?
- Les joueurs doivent-ils accéder à toute la campagne ou seulement à la session ?
- Le MJ veut-il valider les entrées manuellement ?

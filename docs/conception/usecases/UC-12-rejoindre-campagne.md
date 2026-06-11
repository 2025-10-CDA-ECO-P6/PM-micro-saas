# UC-12 — Rejoindre une campagne (membre permanent avec compte)

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre à un joueur de rejoindre une campagne de façon durable, en tant que membre permanent, en utilisant son compte.

## Contexte

UC-12 couvre l'adhésion permanente à une campagne — accès à l'historique des sessions partagées, aux documents de lore visibles entre les parties, et persistance du lien joueur/campagne. Ce cas nécessite un compte joueur.

L'accès ponctuel sans compte (session one-shot, invité temporaire) est couvert par UC-09 — Accès joueur sans compte.

## Besoin utilisateur

Le joueur régulier veut accéder à l'historique de la campagne entre les sessions et retrouver les informations que le MJ lui a partagées, sans perdre ce contexte d'une partie à l'autre.

## Déclencheur

Le MJ invite un joueur à rejoindre la campagne de façon permanente via un lien d'invitation.

## Préconditions

- Une campagne existe.
- Le MJ a généré un lien ou un code d'invitation permanent.
- Le joueur dispose d'un compte (ou le crée lors du parcours d'invitation — voir UC-10 A3).

## Scénario nominal — Rejoindre avec un compte existant

1. Le joueur ouvre le lien d'invitation.
2. Il se connecte à son compte.
3. Il rejoint la campagne.
4. Le MJ l'associe à un personnage.
5. L'accès est conservé durablement sur son compte.

## Scénarios alternatifs

### A1 — Lien expiré

Le joueur ouvre un lien expiré. Le système affiche un message d'erreur et demande au MJ de générer un nouveau lien.

### A2 — Joueur en attente de validation

Le joueur demande à rejoindre la campagne, mais le MJ doit valider son accès avant que l'adhésion soit effective.

### A3 — Joueur sans compte créant un compte pendant l'invitation

Un joueur sans compte suit le lien d'invitation et crée son compte dans le même parcours (voir UC-10 A3). À l'issue, il rejoint la campagne comme membre permanent.

## Exceptions

### E1 — Campagne introuvable

Le lien ne correspond à aucune campagne active.

### E2 — Accès refusé par le MJ

Le MJ refuse ou retire l'accès du joueur.

## Postconditions

- Le joueur est membre permanent de la campagne.
- Le joueur peut accéder aux informations autorisées, y compris l'historique des sessions partagées.
- Le MJ peut gérer son association à un personnage.

## Données manipulées

### Invitation

- Identifiant unique
- Code ou token
- Campagne associée
- Date d'expiration
- Type d'accès (permanent)
- Statut

### Membre de campagne

- Compte joueur associé
- Rôle
- Personnage associé

## Règles métier

- UC-12 = adhésion permanente avec compte uniquement. L'accès sans compte relève de UC-09.
- Le MJ contrôle les invitations.
- Un compte joueur est requis pour un accès persistant et l'accès à l'historique de campagne.
- Les données personnelles joueur sont liées au personnage, pas à un accès invité temporaire.
- Un joueur peut être associé à plusieurs personnages dans une même campagne ; il choisit le personnage actif quand l'action ou la consultation dépend d'une fiche précise.
- Le joueur ne voit que les informations partagées avec lui par le MJ.

## Critères d'acceptation

- Le MJ peut générer un lien d'invitation permanent.
- Un joueur avec un compte peut rejoindre la campagne via le lien d'invitation.
- Le MJ peut associer un joueur membre à un personnage.
- Un joueur membre peut consulter sa fiche et les notes partagées, y compris l'historique.
- Un lien expiré ne permet pas l'accès.

## Questions à valider en interview

- Les MJ préfèrent-ils inviter par lien, code ou email ?
- Les joueurs doivent-ils accéder à toute la campagne ou seulement aux contenus partagés ?
- Le MJ veut-il valider les entrées manuellement ?

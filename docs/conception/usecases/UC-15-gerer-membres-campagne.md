# UC-15 — Gérer les membres d'une campagne

## Acteur principal

MJ

## Acteurs secondaires

Joueurs invités.

## Objectif

Permettre au MJ de contrôler qui accède à sa campagne : inviter des joueurs, leur associer un personnage, retirer un accès, gérer les invitations actives.

## Contexte

UC-10 couvre le point de vue du joueur rejoignant une campagne. UC-15 couvre le point de vue du MJ qui administre les accès. Une campagne est un espace contrôlé : le MJ décide qui peut y accéder et avec quel niveau d'accès.

## Besoin utilisateur

Le MJ veut inviter ses joueurs facilement, leur associer leur personnage, et retirer l'accès d'un joueur qui ne participe plus.

## Déclencheur

Le MJ souhaite intégrer un nouveau joueur, ou gérer les accès existants.

## Préconditions

- Une campagne existe.
- Le MJ est propriétaire de la campagne.

## Scénario nominal — Inviter un joueur

1. Le MJ ouvre la campagne.
2. Il accède à la section "Membres".
3. Il clique sur "Inviter un joueur".
4. Il choisit le type d'invitation :
   - lien partageable (copie dans le presse-papiers) ;
   - invitation par email.
5. Il configure optionnellement :
   - date d'expiration du lien ;
   - nombre d'utilisations maximum.
6. Le système génère l'invitation.
7. Le MJ partage le lien ou envoie l'email.

## Scénarios alternatifs

### A1 — Associer un joueur à un personnage

Après qu'un joueur a rejoint, le MJ lui associe un personnage existant ou en crée un nouveau.

### A2 — Révoquer une invitation

Le MJ invalide un lien avant qu'il ne soit utilisé.

### A3 — Retirer un membre

Le MJ retire un joueur de la campagne. Le joueur perd l'accès mais ses données (personnage, notes partagées) restent dans la campagne.

### A4 — Accès invité sans compte

Le MJ génère un lien pour un joueur qui rejoindra sans créer de compte (GuestAccess). Voir UC-10.

## Exceptions

### E1 — Joueur déjà membre

Le système informe le MJ que l'email ciblé est déjà membre de la campagne.

### E2 — Invitation expirée

Le système ne permet pas de réactiver une invitation expirée — il faut en créer une nouvelle.

## Postconditions

- L'invitation est active et utilisable par le destinataire.
- Ou : le membre est retiré et n'a plus accès à la campagne.
- Ou : le joueur est associé à un personnage.

## Données manipulées

### Invitation

- Type (`LINK` ou `EMAIL`)
- Token
- Date d'expiration (optionnel)
- Nombre d'utilisations maximum (optionnel)
- Statut

### Membre de campagne

- Utilisateur associé
- Rôle dans la campagne (`PLAYER`)
- Personnage associé (optionnel)
- Statut (`PENDING`, `ACTIVE`, `REMOVED`)

## Règles métier

- Seul le MJ propriétaire peut gérer les membres de sa campagne.
- Un lien d'invitation peut être limité en durée ou en nombre d'utilisations.
- La révocation d'une invitation est une suppression physique dans le domaine (`Invitation` → statut `REVOKED`).
- Retirer un membre ne supprime pas ses données dans la campagne.
- Un membre retiré peut être réinvité.

## Critères d'acceptation

- Le MJ peut générer un lien d'invitation partageable.
- Le MJ peut inviter par email.
- Le MJ peut configurer expiration et nombre d'usages du lien.
- Le MJ peut révoquer une invitation active.
- Le MJ peut associer un membre à un personnage.
- Le MJ peut retirer un membre de la campagne.
- La liste des membres est visible depuis la campagne.

## Questions à valider en interview

- Les MJ invitent-ils principalement par lien, email ou les deux ?
- Ont-ils besoin de voir qui a ouvert une invitation sans encore rejoindre ?
- Le retrait d'un joueur doit-il être visible par les autres joueurs ?
- Les MJ veulent-ils une validation manuelle avant que le joueur accède à la campagne ?

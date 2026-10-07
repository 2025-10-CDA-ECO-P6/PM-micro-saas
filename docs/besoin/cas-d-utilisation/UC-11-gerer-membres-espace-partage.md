# UC-11 — Gérer les membres d'un espace partagé

## Acteur principal

MJ

## Acteurs secondaires

Joueurs invités.

## Objectif

Permettre au MJ de contrôler qui accède à son espace : inviter des joueurs, leur associer un personnage, retirer un accès, gérer les invitations actives.

## Contexte

UC-12 couvre la **vue joueur post-accès** (ce que le joueur consulte une fois entré : fiche, documents `PUBLIC`, personnage actif). UC-11 couvre l'**administration des accès et des membres côté MJ** : génération des invitations, association joueur-personnage, révocation, retrait. Un espace partagé (`CAMPAIGN` ou `ONE_SHOT`) est un espace contrôlé : le MJ décide qui peut y accéder et avec quel niveau d'accès. L'espace `PERSONAL` est mono-membre (invariant 13 du domaine) et n'a ni joueur ni membre — il est hors périmètre d'UC-11.

**Note** : le périmètre d'invitation `SPACE` (`InvitationScope`), défini plus bas, désigne un régime d'accès durable, pas le type d'espace — il s'applique aussi bien à un espace `CAMPAIGN` qu'à un `ONE_SHOT`. Le glossaire fait autorité sur cette distinction.

## Besoin utilisateur

Le MJ veut inviter ses joueurs facilement, leur associer leur personnage, et retirer l'accès d'un joueur qui ne participe plus.

## Déclencheur

Le MJ souhaite intégrer un nouveau joueur, ou gérer les accès existants.

## Préconditions

- Un espace partagé (`CAMPAIGN` ou `ONE_SHOT`) existe.
- Le MJ est propriétaire de l'espace.
- L'adhésion d'un membre permanent présuppose que le joueur dispose d'un compte ou en crée un au fil du parcours d'invitation (UC-10). L'octroi d'accès côté joueur (via lien) est couvert par **UC-09** ; la vue obtenue après entrée est couverte par **UC-12**. L'accès invité sans compte (A4) reste couvert par UC-09.

## Scénario nominal — Inviter un joueur

1. Le MJ ouvre l'espace.
2. Il accède à la section "Membres".
3. Il clique sur "Inviter un joueur".
4. Il choisit le mode d'invitation :
   - lien partageable (copie dans le presse-papiers). *(MVP)*
   - ~~invitation par email~~ *(hors MVP — voir Questions ouvertes)*
5. Il choisit le périmètre d'accès :
   - accès durable à l'espace (périmètre `SPACE`) ;
   - accès session temporaire (périmètre `SESSION`).
6. Il configure optionnellement :
   - date d'expiration du lien ;
   - nombre d'utilisations maximum.
7. Le système génère l'invitation.
8. Le MJ partage le lien (Discord, WhatsApp, canal de son choix). *(L'envoi d'email par la plateforme est hors MVP.)*

## Scénarios alternatifs

### A1 — Associer un joueur à un personnage

Après qu'un joueur a rejoint, le MJ lui associe un personnage existant ou en crée un nouveau.

### A2 — Révoquer une invitation

Le MJ invalide un lien avant qu'il ne soit utilisé.

### A3 — Retirer un membre

Le MJ retire un joueur de l'espace. Le joueur perd l'accès mais ses données (personnage, notes partagées) restent dans l'espace.

### A4 — Accès invité sans compte

Le MJ génère un lien pour un joueur qui rejoindra sans créer de compte (accès invité).
Le lien est associé à un personnage ou permet au MJ de faire cette association au moment
de l'arrivée du joueur. L'octroi d'accès côté joueur (clic sur le lien, validation du token, création du `GuestAccess`) relève d'**UC-09** (octroi d'accès joueur via lien) ; la vue obtenue après entrée relève d'**UC-12** (vue joueur post-accès).

## Exceptions

### E1 — Joueur déjà membre

Le système informe le MJ que le joueur est déjà membre de l'espace (détecté sur lien MVP ; sur email — hors MVP).

### E2 — Invitation expirée

Le système ne permet pas de réactiver une invitation expirée — il faut en créer une nouvelle.

## Postconditions

- L'invitation est active et utilisable par le destinataire.
- Ou : le membre est retiré et n'a plus accès à l'espace.
- Ou : le joueur est associé à un personnage.

## Données manipulées

UC-11 est le **propriétaire unique** des données `Invitation` et `Membre d'espace` (`SpaceMembership`).

### Invitation

- Type de lien (`LINK` — MVP ; `EMAIL` hors MVP)
- Périmètre (`SPACE` ou `SESSION`) — détermine la durée et l'étendue de l'accès octroyé : `SPACE` correspond à un accès durable (membre permanent) ; `SESSION` correspond à un accès temporaire (invité de session). Ces deux valeurs remplacent et subsument l'ancienne notion de « type d'accès (permanent / temporaire) ».
- Session associée si périmètre `SESSION`
- Token
- Date d'expiration (optionnel)
- Nombre d'utilisations maximum (optionnel)
- Statut (`ACTIVE`, `REVOKED`, `EXPIRED`)

### Membre d'espace (SpaceMembership)

- Utilisateur associé (compte joueur)
- Rôle dans l'espace (`PLAYER`)
- Personnages associés via les `Document` de type `player_character` (optionnel, zéro à plusieurs)
- Statut (`PENDING`, `ACTIVE`, `REMOVED`)

## Règles métier

- Seul le MJ propriétaire peut gérer les membres de son espace.
- L'octroi d'un accès durable (`SpaceMembership`) à un joueur est soumis à la limite du tier gratuit : sur un compte FREE, tout octroi d'accès supplémentaire — quelle qu'en soit la forme (membre permanent ou accès invité) — est refusé si la limite est déjà atteinte. Règle définie dans UC-09 (RB-09-21) ; UC-11 ne la redéfinit pas.
- Un lien d'invitation peut être limité en durée ou en nombre d'utilisations.
- La révocation d'une invitation passe son statut à `REVOKED`. Elle ne peut plus créer de nouvel accès.
- Retirer un membre ne supprime pas ses données dans l'espace.
- Un membre retiré peut être réinvité.
- Le personnage associé à un membre retiré est un `Document` de type `player_character` ; il appartient à l'espace, persiste après le retrait et peut être réassocié à un autre joueur.
- Un accès invité est temporaire. La **fiche de personnage** (`Document` de type `player_character`) appartient à l'espace, persiste entre les accès et est ré-associable via un nouveau lien invité vers le même personnage.
- Les **notes personnelles** (`PLAYER_PRIVATE`) appartiennent à leur auteur, pas au personnage. Un invité non converti en compte voit ses notes supprimées à la fin définitive de son accès (RB-09-19) — elles ne sont **jamais** récupérées par un nouvel invité réassocié au même personnage. Elles ne survivent que si l'invité crée un compte avant la fin de son accès (UC-09 A2, RB-09-14). L'invité est averti de ce sort en temps utile pour agir avant la fin de son accès (RB-09-22).

## Critères d'acceptation

- Le MJ peut générer un lien d'invitation partageable.
- Le MJ peut configurer expiration et nombre d'usages du lien.
- Le MJ peut révoquer une invitation active.
- Le MJ peut associer un membre à un personnage.
- Le MJ peut retirer un membre de l'espace.
- La liste des membres est visible depuis l'espace.

## Questions à valider en interview

- **[Hors MVP — à valider post-MVP]** L'invitation par email : les MJ en ont-ils besoin en complément du lien ? Quelle est la valeur ajoutée par rapport à copier-coller le lien dans un email manuel ?
- Les MJ invitent-ils principalement par lien, email ou les deux ?
- Ont-ils besoin de voir qui a ouvert une invitation sans encore rejoindre ? *(distinct du compteur d'usages restants — résolu, dérivable du domaine ; voir US-UC-11, Questions ouvertes Q1. Ce point-ci porte sur le suivi d'ouverture, non tranché.)*
- Le retrait d'un joueur doit-il être visible par les autres joueurs ? *(le log d'activité associé reste Could Have, déjà exclu ; voir US-UC-11, Questions ouvertes Q2. Seule cette visibilité-ci reste ouverte.)*
- Les MJ veulent-ils une validation manuelle avant que le joueur accède à l'espace ?

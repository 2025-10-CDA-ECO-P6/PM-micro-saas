# UC-08 — Partager une information aux joueurs

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, en consultation des documents rendus publics.

## Objectif

Permettre au MJ de partager certains documents avec les joueurs tout en conservant ses documents
privés.

## Contexte

Dans un espace, certaines informations doivent être transmises aux joueurs : indices, aides
de jeu, lore, rappels, révélations ou résumés. D'autres informations doivent rester secrètes.
Dans le MVP, le partage durable d'un document passe par sa visibilité : privé pour le MJ
ou visible par les joueurs.

## Besoin utilisateur

Le MJ veut contrôler clairement ce qui est visible ou non par les joueurs, sans exposer
accidentellement sa préparation privée.

## Déclencheur

Le MJ souhaite transmettre une information au groupe.

## Préconditions

- Un espace existe.
- Des joueurs ou des accès invités peuvent accéder à l'espace ou à la session.
- Le contenu à partager existe ou est créé par le MJ.

## Scénario nominal

1. Le MJ ouvre un document partageable.
2. Il choisit l'action "Partager".
3. Le système indique que le document deviendra visible pour tous les membres de l'espace
   et les accès invités actifs.
4. Le MJ valide le partage.
5. Le système rend le document visible par les joueurs.
6. Les joueurs voient l'information dans leur espace ou leur vue session.

## Scénarios alternatifs

### A1 — Retirer le partage

Le MJ rend à nouveau une information privée via l'action de retrait du partage.

### A2 — Partage d'une note de session

Le MJ rend publique une note créée pendant une session. Elle devient visible par les joueurs
comme les autres documents partagés.

### A3 — Partage depuis la vue session

Le MJ partage une information pendant une partie. Lorsque la session est en statut `LIVE`, le document partagé est automatiquement ajouté aux documents épinglés de la session (auto-épinglage). L'auto-épinglage ne s'applique qu'aux sessions `LIVE` — un partage depuis une vue de session `CLOSED` rend le document visible par les joueurs sans l'épingler.

### A4 — Partage d'un récapitulatif post-session

Le MJ crée ou ouvre un document de récapitulatif post-session, puis le partage comme n'importe quel document.

## Exceptions

### E1 — Document déjà public

Le MJ partage un document déjà visible par les joueurs. Le système n'applique aucun changement destructeur
et indique que le document est déjà visible par les joueurs.

### E2 — Accès joueur expiré ou révoqué

Un joueur dont l'accès invité a expiré ou été révoqué ne peut plus consulter les documents partagés.

## Postconditions

- L'information est visible par les joueurs autorisés à accéder aux documents partagés.
- Les notes privées restent protégées.
- Le MJ peut modifier la visibilité ultérieurement.

## Données manipulées

### Information partagée

- Identifiant du contenu
- Type de contenu
- Visibilité
- Date de partage
- Auteur du partage

## Règles métier

- Tout document créé par le MJ est privé par défaut.
- Seul le MJ peut partager une information durable de l'espace.
- Un joueur ne peut consulter que les documents partagés et ses propres notes de session personnelles joueur.
- Le MJ peut retirer un partage via l'action de retrait du partage.
- Le partage est durable : le document reste accessible entre les sessions jusqu'à retrait explicite.
- Le partage sélectif par joueur ou personnage n'est pas dans le périmètre MVP — voir l'arbitrage ci-dessous.
- Un document partagé depuis la vue session pendant une session `LIVE` est automatiquement ajouté aux documents épinglés de cette session. Ce comportement (auto-épinglage) est limité aux sessions en statut `LIVE`.
- L'épinglage et la visibilité sont indépendants : retirer le partage d'un document épinglé ne le supprime pas des documents épinglés — il y reste mais n'est plus visible que du MJ. Désépingler et retirer le partage sont deux opérations distinctes.

### Arbitrage — Granularité du partage (2026-06-10)

Le MVP livre un partage par document (le MJ rend un document visible pour l'ensemble des joueurs autorisés, ou le garde privé). Le partage sélectif par joueur ou personnage est reporté post-MVP.

**Raison d'être** : valider en priorité que le partage au groupe (sans secret intra-groupe) améliore effectivement la fluidité de transmission des informations comparé aux solutions actuelles, avant d'investir dans la complexité d'une granularité fine.

**Condition de retour** : si des tables réelles se heurtent à un blocage — révélations destinées à un seul joueur, gestion des secrets entre joueurs trop rigide — cette granularité sera réexaminée.

## Critères d'acceptation

- Le MJ peut partager un document avec tous les joueurs autorisés.
- Le MJ peut retirer un partage.
- Un joueur ne voit pas les documents privés.
- Les joueurs voient les informations partagées dans leur espace.

## Questions à valider en interview

- Les MJ partagent-ils souvent des documents ou notes aux joueurs ?
- Le partage par groupe suffit-il pour le MVP ?
- Ont-ils déjà eu des problèmes de spoilers ou d'informations révélées trop tôt ?
- Quels types d'informations sont généralement partagés ?

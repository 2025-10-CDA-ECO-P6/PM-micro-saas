# UC-12 — Consulter sa campagne en tant que joueur (vue post-accès)

## Acteur principal

Joueur

## Acteurs secondaires

Aucun (vue en lecture/consultation ; le MJ agit via UC-11).

## Objectif

Offrir au joueur déjà entré dans une campagne une vue cohérente : fiche du personnage actif, documents `PUBLIC`, et choix du personnage actif lorsque le joueur est associé à plusieurs personnages, selon le périmètre `SESSION` ou `CAMPAIGN`.

## Contexte

UC-12 couvre la perspective joueur **post-accès** : le joueur est déjà entré, son `GuestAccess` ou son `CampaignMembership` est actif.

L'octroi de cet accès est couvert par **UC-09** (côté joueur, via lien) et **UC-11** (génération et administration côté MJ) ; UC-12 commence une fois le joueur entré.

## Besoin utilisateur

Le joueur veut accéder sans friction à sa fiche de personnage et aux informations que le MJ lui a partagées, dès qu'il est entré dans la campagne ou la session.

## Déclencheur

Le joueur accède à la vue de campagne ou de session après que son accès a été activé.

## Préconditions

- Le joueur dispose d'un `GuestAccess` ou d'un `CampaignMembership` actif.
- L'octroi de cet accès est couvert par **UC-09** (octroi d'accès joueur via lien) et **UC-11** (génération et administration côté MJ) ; UC-12 commence une fois le joueur entré.

## Scénario nominal — Vue joueur post-accès

1. Le joueur accède à la vue de campagne ou de session.
2. Le système vérifie que son `GuestAccess` ou son `CampaignMembership` est actif.
3. Si un personnage lui a été associé par le MJ, la fiche du personnage actif s'affiche.
4. Les documents `PUBLIC` sont affichés selon le périmètre d'accès (`SESSION` ou `CAMPAIGN`).
5. Le joueur peut consulter sa fiche et ses notes `PLAYER_PRIVATE` liées au personnage actif.

## Scénarios alternatifs

### A1 — Joueur associé à plusieurs personnages (choix du personnage actif)

Le joueur est associé à plusieurs personnages dans la même campagne. À l'entrée en vue, une interface de sélection lui propose de choisir le personnage actif pour la session en cours. Après sélection, la fiche et les notes `PLAYER_PRIVATE` du personnage actif s'affichent. Le choix ne modifie pas les associations définies par le MJ (RB-12-07). Le joueur peut changer de personnage actif à tout moment (RB-12-09).

### A2 — Accès périmètre SESSION uniquement

Le joueur dispose d'un accès périmètre `SESSION`. Il voit les documents épinglés de la session et les documents `PUBLIC` de la campagne, mais n'a pas accès à l'historique complet des sessions ni au lore complet de la campagne (RB-12-04).

### A3 — Joueur sans personnage associé

Aucun personnage n'a été associé au joueur par le MJ. Le joueur consulte les documents `PUBLIC` mais ne peut pas créer de notes `PLAYER_PRIVATE` liées à un personnage (RB-12-03).

## Exceptions

### E1 — GuestAccess ou CampaignMembership inactif ou expiré

L'accès n'est plus actif. La vue post-accès ne s'affiche pas. Le joueur est renvoyé vers le flux d'accès — voir **UC-09** (octroi d'accès joueur via lien) et **UC-11** (administration côté MJ).

## Postconditions

- Le joueur consulte sa fiche de personnage actif (si personnage associé) et les documents `PUBLIC` selon son périmètre.
- Les notes `PLAYER_PRIVATE` de l'auteur sont accessibles sur le personnage actif.
- Si multi-personnages, le joueur a sélectionné un personnage actif pour la session en cours.

## Règles métier

- **RB-12-01** : Le joueur ne voit que les documents dont la visibilité est `PUBLIC`. Les documents `GM_ONLY` et les documents `PLAYER_PRIVATE` d'autres personnages sont invisibles.
- **RB-12-02** : Les notes `PLAYER_PRIVATE` sont liées à leur **auteur**, rattachées au personnage pour l'affichage. Elles persistent entre les sessions pour le **même auteur** (compte stable). Un joueur accédant à un personnage déjà joué par quelqu'un d'autre ne récupère jamais les notes de son prédécesseur (RB-09-19).
- **RB-12-03** : Un joueur sans personnage associé peut consulter les documents `PUBLIC` mais ne peut pas créer de notes `PLAYER_PRIVATE` liées à un personnage.
- **RB-12-04** : Un accès périmètre `SESSION` ne donne pas accès à l'historique complet des sessions ni au lore de la campagne. Seuls les documents `PUBLIC` et les documents épinglés de la session sont visibles.
- **RB-12-05** : Un accès périmètre `CAMPAIGN` (`CampaignMembership`) donne accès à l'ensemble des documents `PUBLIC` et à l'historique des sessions passées.
- **RB-12-06** : Un joueur associé à plusieurs personnages doit choisir un personnage actif pour les actions dépendant d'une fiche précise (notes `PLAYER_PRIVATE`, affichage de fiche).
- **RB-12-07** : Le choix du personnage actif est propre à la session en cours. Il ne modifie pas les associations définies par le MJ.
- **RB-12-08** : Un joueur avec un seul personnage associé n'a pas à effectuer de sélection — son personnage est actif par défaut.
- **RB-12-09** : Le joueur peut changer de personnage actif à tout moment pendant la session.
- **RB-12-10** : Les notes `PLAYER_PRIVATE` et la fiche affichées correspondent toujours au personnage actif sélectionné.

## Critères d'acceptation

- Le joueur avec un personnage associé voit la fiche du personnage actif dès l'accès à la vue.
- Le joueur voit les documents `PUBLIC` filtrés selon son périmètre (`SESSION` ou `CAMPAIGN`).
- Les notes `PLAYER_PRIVATE` du personnage associé sont accessibles et éditables.
- Un joueur sans personnage associé voit les documents `PUBLIC` mais ne peut pas créer de notes liées à un personnage.
- Un accès périmètre `SESSION` ne donne pas accès à l'historique des sessions précédentes ni au lore complet.
- Un accès périmètre `CAMPAIGN` donne accès à l'ensemble des documents `PUBLIC` et à l'historique.
- Un joueur associé à plusieurs personnages voit une interface de sélection du personnage actif.
- Le changement de personnage actif ne modifie pas les associations définies par le MJ.

## Frontière avec UC-06

UC-06 enrichit cette vue **pendant une session `LIVE`** en autorisant la prise de notes de session ; UC-12 couvre la consultation post-accès (fiche, documents `PUBLIC`, historique selon périmètre) et le choix du personnage actif.

## Relations

- **UC-09** — Octroi d'accès joueur via lien (précondition : `GuestAccess` ou `CampaignMembership` créé ici).
- **UC-11** — Administration côté MJ : génération des invitations, association joueur-personnage, gestion des membres.
- **UC-06** — Vue en session LIVE : enrichit UC-12 pendant une session active.

## Questions à valider en interview

- Le personnage actif doit-il être mémorisé entre les sessions ou remis à zéro à chaque connexion ?
- Quand un joueur a deux personnages et que le MJ épingle un document lié à un seul, ce document apparaît-il quelle que soit la sélection active ?
- Un joueur sans personnage associé peut-il prendre des notes « libres » non liées à un personnage ?
- La sélection du personnage actif doit-elle être visible par le MJ dans sa vue session ?
- Si un joueur cumule un accès `SESSION` et un accès `CAMPAIGN` simultanément, quelle vue s'affiche en priorité ?

# UC-12 — Consulter son espace en tant que joueur (vue post-accès)

## Acteur principal

Joueur

## Acteurs secondaires

Aucun (vue en lecture/consultation ; le MJ agit via UC-11).

## Objectif

Offrir au joueur déjà entré dans un espace partagé (`CAMPAIGN` ou `ONE_SHOT`) une vue cohérente : fiche du personnage actif, documents `PUBLIC`, et choix du personnage actif lorsque le joueur est associé à plusieurs personnages, selon le périmètre `SESSION` ou `SPACE`.

## Contexte

UC-12 couvre la perspective joueur **post-accès** : le joueur est déjà entré, son `GuestAccess` ou son `SpaceMembership` est actif.

L'octroi de cet accès est couvert par **UC-09** (côté joueur, via lien) et **UC-11** (génération et administration côté MJ) ; UC-12 commence une fois le joueur entré.

## Besoin utilisateur

Le joueur veut accéder sans friction à sa fiche de personnage et aux informations que le MJ lui a partagées, dès qu'il est entré dans l'espace ou la session.

## Déclencheur

Le joueur accède à la vue d'espace ou de session après que son accès a été activé.

## Préconditions

- Le joueur dispose d'un `GuestAccess` ou d'un `SpaceMembership` actif.
- L'accès porte sur un espace partagé (`CAMPAIGN` ou `ONE_SHOT`) — un espace `PERSONAL` est mono-membre et n'a ni joueur ni membre autre que son propriétaire (invariant 13 du domaine), il est hors périmètre d'UC-12.
- L'octroi de cet accès est couvert par **UC-09** (octroi d'accès joueur via lien) et **UC-11** (génération et administration côté MJ) ; UC-12 commence une fois le joueur entré.

## Scénario nominal — Vue joueur post-accès

1. Le joueur accède à la vue d'espace ou de session.
2. Le système vérifie que son `GuestAccess` ou son `SpaceMembership` est actif.
3. Si un personnage lui a été associé par le MJ, la fiche du personnage actif s'affiche, qu'elle soit `GM_ONLY` ou `PUBLIC` (RB-12-01).
4. Les documents `PUBLIC` sont affichés selon le périmètre d'accès (`SESSION` ou `SPACE`).

## Scénarios alternatifs

### A1 — Joueur associé à plusieurs personnages (choix du personnage actif)

Le joueur est associé à plusieurs personnages dans le même espace. À l'entrée en vue, une interface de sélection lui propose de choisir le personnage actif pour la session en cours. Après sélection, la fiche du personnage actif s'affiche. Le choix ne modifie pas les associations définies par le MJ (RB-12-07). Le joueur peut changer de personnage actif à tout moment (RB-12-09).

### A2 — Accès périmètre SESSION uniquement

Le joueur dispose d'un accès périmètre `SESSION`. Il voit les documents épinglés de la session et tous les documents `PUBLIC` de l'espace, mais n'a pas accès à l'historique des sessions (RB-12-04).

### A3 — Joueur sans personnage associé

Aucun personnage n'a été associé au joueur par le MJ. Le joueur consulte les documents `PUBLIC` ; il n'a aucune fiche à afficher.

## Exceptions

### E1 — GuestAccess ou SpaceMembership inactif ou expiré

L'accès n'est plus actif. La vue post-accès ne s'affiche pas. Le joueur est renvoyé vers le flux d'accès — voir **UC-09** (octroi d'accès joueur via lien) et **UC-11** (administration côté MJ).

## Postconditions

- Le joueur consulte sa fiche de personnage actif (si personnage associé) et les documents `PUBLIC` selon son périmètre.
- Si multi-personnages, le joueur a sélectionné un personnage actif pour la session en cours.

## Règles métier

- RB-12-01 à RB-12-05 : définies dans [US-12-01](../stories/US-UC-12-consulter-espace-joueur.md#us-12-01--accéder-à-sa-fiche-de-personnage-et-aux-informations-partagées).
- RB-12-06 à RB-12-10 : définies dans [US-12-02](../stories/US-UC-12-consulter-espace-joueur.md#us-12-02--choisir-son-personnage-actif-quand-associé-à-plusieurs-personnages).

## Critères d'acceptation

- Le joueur avec un personnage associé voit la fiche du personnage actif dès l'accès à la vue, qu'elle soit `GM_ONLY` ou `PUBLIC`.
- Le joueur voit les documents `PUBLIC` filtrés selon son périmètre (`SESSION` ou `SPACE`).
- Un joueur sans personnage associé voit les documents `PUBLIC`.
- Un accès périmètre `SESSION` donne accès aux documents épinglés de la session et à tous les documents `PUBLIC` de l'espace, sans l'historique des sessions précédentes.
- Un accès périmètre `SPACE` donne accès à l'ensemble des documents `PUBLIC` et à l'historique.
- Un joueur associé à plusieurs personnages voit une interface de sélection du personnage actif.
- Le changement de personnage actif ne modifie pas les associations définies par le MJ.

## Frontière avec UC-06

UC-06 décrit la vue joueur **pendant une session `LIVE`** (documents partagés en direct par le MJ) ; UC-12 couvre la consultation post-accès (fiche, documents `PUBLIC`, historique selon périmètre) et le choix du personnage actif. La prise de notes de session par le joueur est hors MVP (voir la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md#notes-personnelles-joueur)).

## Relations

- **UC-09** — Octroi d'accès joueur via lien (précondition : `GuestAccess` ou `SpaceMembership` créé ici).
- **UC-11** — Administration côté MJ : génération des invitations, association joueur-personnage, gestion des membres.
- **UC-06** — Vue en session LIVE : enrichit UC-12 pendant une session active.

## Décisions (issues des questions ouvertes — détail dans US-12-02)

**Principe unificateur** : le personnage actif est un état de **focus / présentation éphémère**, jamais un droit d'accès ; les droits sur les documents dérivent de l'**ensemble des personnages associés** au joueur, pas du seul personnage actif.

- **Personnage actif mémorisé entre sessions** : **session-local au MVP** (RB-12-07) — préférence client, non persistée au MVP ; persistance inter-sessions hors MVP, sa priorité est attribuée par la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md) (§ Persistance du personnage actif entre sessions (UC-12)).
- **Document épinglé lié à un personnage** : **toujours visible**, quel que soit le personnage actif (RB-12-01, RB-08-04, RB-12-10) — le personnage actif ne gouverne que le focus de présentation.
- **Notes de session du joueur** : **hors MVP**, toutes (Could Have). Quand elles existeront, toute note `PLAYER_PRIVATE` requerra un `player_character` (RB-12-02, RB-12-03, RB-12-10) ; la note libre non rattachée est hors MVP au même titre. Le cycle de vie éphémère des notes d'un invité sans compte (règle métier 10 de *space-management*, RGPD Art. 17) est inchangé. Sa priorité est attribuée par la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md) (§ Notes libres du joueur, sans personnage associé (UC-12)). Le cycle de vie éphémère des notes d'un invité sans compte (règle métier 10 de *space-management*, RGPD Art. 17) est inchangé.
- **Priorité `SESSION` + `SPACE` simultanés** : **`SPACE` prioritaire** (superset — RB-12-05 ⊃ RB-12-04) ; cas largement précludé par les invariants 3 et 8 du domaine.

## Questions à valider en interview

- La sélection du personnage actif doit-elle être visible par le MJ dans sa vue session ? Différée — hors MVP. Si retenue, relèverait d'un **état runtime de Session Conduct (UC-06)**, pas d'une donnée `SpaceMembership` (le personnage actif restant préférence client au MVP).

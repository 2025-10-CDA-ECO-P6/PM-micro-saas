# UC-07 — Créer un élément à la volée pendant la session

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre au MJ de créer instantanément n'importe quel type d'élément (note, PNJ, personnage joueur, document) depuis la vue session, pour s'adapter en temps réel aux imprévus des joueurs.

## Contexte

Pendant une session, les joueurs prennent des décisions imprévues : ils interagissent avec un PNJ non préparé, créent un nouveau personnage spontanément, ou le MJ veut noter immédiatement une information. Le MJ doit pouvoir réagir sans quitter le contexte de session ni bloquer le rythme de jeu.

## Besoin utilisateur

Le MJ veut créer n'importe quel type d'élément de jeu en quelques secondes depuis la vue session, avec un minimum d'informations requises, pour maintenir la fluidité de la partie.

## Déclencheur

Le MJ clique sur "Créer" ou utilise un raccourci depuis la vue session.

## Préconditions

- Une session est en statut LIVE ou CLOSED.
- Le MJ est propriétaire de la campagne (compte cloud) ou en mode local (UC-01).

## Scénario nominal

1. Le MJ ouvre le panneau de création rapide depuis la vue session.
2. Il sélectionne le type d'élément à créer :
   - Note (LiveNote ou Document NOTE) ;
   - PNJ ;
   - Personnage joueur ;
   - Document (lore, lieu, objet, etc.).

3. Il saisit un titre minimal (seul champ obligatoire).
4. Il valide.
5. Le système crée l'élément avec :
   - `campaignId` de la session en cours ;
   - `sessionId` de la session en cours (lien auto) ;
   - champs requis à leurs valeurs par défaut.

6. L'élément est immédiatement disponible dans la vue session.
7. Le MJ peut l'enrichir plus tard en dehors de la session.

## Scénarios alternatifs

### A1 — Création d'un PNJ à la volée

Le MJ renseigne uniquement le nom. Le PNJ est créé comme un `Document` standard typé PNJ
avec des propriétés vides, puis ajouté à `Session.selectedDocumentIds`.

### A2 — Création d'un personnage joueur à la volée

Le MJ crée un `PlayerCharacter` minimal (nom seul). Il pourra l'associer à un joueur et le compléter plus tard.

### A3 — Création d'une note LiveNote

La note est automatiquement liée à la session (`sessionId`) et horodatée.

### A4 — Création d'un document générique

Le MJ crée un Document (lieu, faction, objet, lore) avec un titre. Le document est lié à la campagne et optionnellement à la session.

### A5 — Session CLOSED (ajout rétroactif)

Le MJ peut créer des éléments à la volée depuis une session CLOSED (ajout rétroactif d'informations oubliées pendant la partie).

## Exceptions

### E1 — Titre vide

Le système refuse la création si le titre est vide ou uniquement composé d'espaces.

### E2 — Session ARCHIVED

La création à la volée est impossible depuis une session ARCHIVED (lecture seule complète).

## Postconditions

- L'élément créé est lié à la campagne et à la session.
- Il est immédiatement consultable dans la vue session.
- Il peut être enrichi ultérieurement.
- Pour un PNJ : il est ajouté à `Session.selectedDocumentIds`.

## Données manipulées

- Note / LiveNote
- Document typé PNJ (avec lien dans `Session.selectedDocumentIds`)
- PlayerCharacter
- Document (tout type)
- Session (`selectedDocumentIds` mis à jour si PNJ, lieu, objet ou document utile à la session)

## Règles métier

- Le titre est le seul champ obligatoire pour toute création à la volée.
- L'élément créé est automatiquement lié à la `campaignId` et au `sessionId` de la session en cours.
- Un PNJ créé à la volée est automatiquement ajouté à `Session.selectedDocumentIds`.
- La création à la volée est possible sur une session LIVE ou CLOSED, mais pas ARCHIVED.
- Les éléments créés à la volée sont privés par défaut (`visibility = PRIVATE`).

## Critères d'acceptation

- Le MJ peut créer une note, un PNJ, un personnage joueur et un document depuis la vue session.
- Le titre seul suffit à valider la création.
- L'élément est immédiatement lié à la session et à la campagne.
- Un PNJ créé à la volée apparaît dans les documents de session, filtrable comme PNJ.
- La création est impossible depuis une session ARCHIVED.
- Le MJ peut compléter l'élément créé à la volée ultérieurement.

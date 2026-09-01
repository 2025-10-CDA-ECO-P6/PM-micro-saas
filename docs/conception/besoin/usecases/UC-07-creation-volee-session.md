# UC-07 — Créer un élément à la volée pendant la session

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre au MJ de créer instantanément un document de campagne depuis la vue session
(note, PNJ, personnage joueur, lieu, faction, objet, lore ou autre contenu), pour s'adapter
en temps réel aux imprévus des joueurs.

## Contexte

Pendant une session, les joueurs prennent des décisions imprévues : ils interagissent avec un PNJ
non préparé, découvrent un lieu inattendu, font émerger une faction ou obligent le MJ à noter
immédiatement une information. Le MJ doit pouvoir créer un document durable ou une note de session
sans quitter le contexte de session ni bloquer le rythme de jeu.

## Besoin utilisateur

Le MJ veut créer un document en quelques secondes depuis la vue session, avec un minimum
d'informations requises, pour maintenir la fluidité de la partie et enrichir la campagne après coup.

## Déclencheur

Le MJ clique sur "Créer" ou utilise un raccourci depuis la vue session.

## Préconditions

- Une session est en statut LIVE ou CLOSED.
- Le MJ est propriétaire de la campagne (compte cloud) ou en mode local (UC-01).

## Scénario nominal

1. Le MJ ouvre le panneau de création rapide depuis la vue session.
2. Il sélectionne le mode de création :
   - note de session ;
   - document libre ;
   - document typé optionnel (note, PNJ, lieu, personnage joueur ou autre type disponible).

3. Il saisit un titre minimal (seul champ obligatoire).
4. Il valide.
5. Le système crée le document avec :
   - la campagne de la session en cours ;
   - le type choisi, le cas échéant ;
   - une visibilité privée par défaut ;
   - dossier d'accueil selon le point d'entrée ou le type ;
   - champs requis à leurs valeurs par défaut.

6. Si la session est en statut `LIVE`, le système épingle automatiquement le document créé dans la session
   (épinglage par défaut — même paradigme qu'AR-12/UC-08 pour le partage). Le MJ peut le désépingler
   d'un geste si le document ne concerne pas la table. En session `CLOSED`, l'épinglage n'est pas
   automatique : le MJ épingle manuellement s'il le souhaite.
7. Le MJ peut l'enrichir plus tard en dehors de la session.

## Scénarios alternatifs

### A1 — Création d'un PNJ à la volée

Le MJ renseigne uniquement le nom. Le PNJ est créé comme un document typé PNJ
avec des propriétés vides. En session `LIVE`, il est épinglé automatiquement par défaut dans
la session (le MJ peut le désépingler d'un geste) ; en session `CLOSED`, l'épinglage reste
optionnel et non automatique.

### A2 — Création d'un personnage joueur à la volée

Le MJ crée une fiche de personnage joueur minimale (nom seul). Il pourra l'associer
à un joueur et le compléter plus tard.

### A3 — Création d'une note de session

La note est créée comme note de session, puis rattachée à la session en cours.

### A4 — Création d'un document générique

Le MJ crée un document (lieu, faction, objet, lore) avec un titre. Le document est lié à la campagne.
En session `LIVE`, il est épinglé automatiquement par défaut dans la session (le MJ peut le désépingler
d'un geste s'il ne doit pas rester sous la main) ; en session `CLOSED`, l'épinglage reste optionnel et
non automatique, à la main du MJ s'il doit rester accessible.

### A5 — Session CLOSED (ajout rétroactif)

Le MJ peut créer des éléments à la volée depuis une session CLOSED (ajout rétroactif d'informations oubliées pendant la partie).

## Exceptions

### E1 — Titre vide

Le système refuse la création si le titre est vide ou uniquement composé d'espaces.

### E2 — Session ARCHIVED

La création à la volée est impossible depuis une session ARCHIVED (lecture seule complète).

## Postconditions

- Le document créé est lié à la campagne.
- Il est immédiatement consultable dans la vue session.
- Il peut être enrichi ultérieurement.
- En session `LIVE` : le document créé est épinglé par défaut dans la session (désépinglable par le MJ).
  En session `CLOSED` : l'épinglage reste optionnel, non automatique.

## Données manipulées

- Document libre ou typé
- Document typé PNJ
- Document typé personnage joueur
- Note de session
- Session (documents épinglés ou notes de session mis à jour selon le cas)

## Règles métier

- Le titre est le seul champ obligatoire pour toute création à la volée.
- Le document créé est automatiquement lié à la campagne de la session en cours.
- Une note de session est rattachée à la session.
- En session `LIVE`, un document durable créé à la volée est automatiquement épinglé par défaut (même
  paradigme qu'AR-12/UC-08 pour le partage) ; le MJ peut le désépingler d'un geste si le document n'est
  pas pertinent pour la table. En session `CLOSED`, l'épinglage n'est pas automatique : il reste une
  action manuelle optionnelle du MJ.
- La création à la volée est possible sur une session LIVE ou CLOSED, mais pas ARCHIVED.
- Les documents créés à la volée sont privés par défaut.

## Critères d'acceptation

- Le MJ peut créer une note, un PNJ, un personnage joueur et un document depuis la vue session.
- Le titre seul suffit à valider la création.
- Le document est immédiatement lié à la campagne et, si nécessaire, référencé par la session.
- Un PNJ créé à la volée apparaît dans les documents épinglés ou dans son dossier, filtrable comme PNJ.
- La création est impossible depuis une session ARCHIVED.
- Le MJ peut compléter le document créé à la volée ultérieurement.

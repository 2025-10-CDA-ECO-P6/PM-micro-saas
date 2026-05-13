# UC-04 — Gérer les documents de campagne et notes MJ

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, uniquement si certaines notes sont partagées.

## Objectif

Permettre au MJ de créer, organiser, retrouver et éventuellement partager des documents de campagne :
notes libres, lore, lieux, PNJ, objets, rappels ou idées.

## Contexte

La capture d'information est un besoin central pour un MJ. Une "note" est ici un document libre
ou typé selon le besoin : idée rapide, événement, secret, décision de joueurs, élément de lore,
lieu, PNJ, objet ou rappel pour une prochaine session.

## Besoin utilisateur

Le MJ veut centraliser ses documents et les relier au contexte approprié pour éviter de les perdre ou de les oublier.

## Déclencheur

Le MJ prépare une campagne, rédige un scénario, improvise une idée ou prend une note pendant une session.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section "Notes".
3. Il clique sur "Créer un document" ou "Créer une note rapide".
4. Le système affiche l'éditeur de document.
5. Le MJ renseigne :
   - titre ;
   - contenu ;
   - type de document optionnel ;
   - visibilité ;
   - tags ;
   - éléments liés.

6. Par défaut, le document est privé.
7. Le MJ peut lier le document à un scénario, une scène, un PNJ, un personnage ou une session.
8. Le MJ sauvegarde le document.
9. Le document devient accessible depuis son dossier, la recherche et les entités liées.

## Scénarios alternatifs

### A1 — Note rapide

Le MJ crée une note rapide sans titre. Le système génère un titre temporaire à partir de la date ou du début du contenu.
Cette note reste un `Document` standard.

### A2 — Note partagée

Le MJ définit le document comme visible par tous les joueurs ou par certains joueurs uniquement.

### A3 — Changement de visibilité

Le MJ transforme un document privé en document partagé ou inversement.

### A4 — Note prise pendant une session

Le MJ crée une LiveNote depuis la vue session, ou un Document durable ajouté à `Session.selectedDocumentIds`
s'il veut le conserver comme élément de campagne.

## Exceptions

### E1 — Contenu vide

Le système peut empêcher la création d'une note vide ou autoriser une note vide comme brouillon.

### E2 — Accès joueur non autorisé

Un joueur tente d'accéder à une note privée. Le système refuse l'accès.

## Postconditions

- Le document est enregistré.
- Une note rapide est placée dans le dossier système **Notes** de la campagne par défaut.
- Le document respecte sa configuration de visibilité.

## Données manipulées

### Document de campagne

- Identifiant unique
- Titre
- Contenu
- Type de document optionnel
- Visibilité
- Tags
- Entités liées
- Auteur
- Date de création
- Date de modification

## Règles métier

- Tout document créé par le MJ est privé par défaut.
- Seul le MJ peut partager un document de campagne.
- Un document peut être lié à plusieurs entités.
- Un joueur ne peut consulter et voir que les documents qui lui sont explicitement accessibles.

## Critères d'acceptation

- Le MJ peut créer un document privé.
- Le MJ peut créer un document partagé.
- Le MJ peut modifier la visibilité d'un document.
- Un document privé n'est pas visible par les joueurs.
- Un document peut être retrouvé via la recherche.

## Questions à valider en interview

- Comment les MJ prennent-ils leurs notes aujourd'hui ?
- Les notes sont-elles plutôt longues, courtes, structurées ou libres ?
- Les MJ ont-ils besoin de partager certaines notes ?
- Comment gèrent-ils les secrets et les informations connues des joueurs ?
- Quelles notes doivent être retrouvées rapidement en session ?

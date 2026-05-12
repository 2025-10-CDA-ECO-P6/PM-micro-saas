# UC-04 — Gérer les notes MJ

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, uniquement si certaines notes sont partagées.

## Objectif

Permettre au MJ de créer, organiser, retrouver et éventuellement partager ses notes de campagne.

## Contexte

La prise de notes est un besoin central pour un MJ. Les notes peuvent concerner des idées, des événements, des secrets, des décisions de joueurs, des éléments de lore ou des rappels pour une prochaine session.

## Besoin utilisateur

Le MJ veut centraliser ses notes et les relier au contexte approprié pour éviter de les perdre ou de les oublier.

## Déclencheur

Le MJ prépare une campagne, rédige un scénario, improvise une idée ou prend une note pendant une session.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section "Notes".
3. Il clique sur "Créer une note".
4. Le système affiche un éditeur de note.
5. Le MJ renseigne :
   - titre ;
   - contenu ;
   - type de note ;
   - visibilité ;
   - tags ;
   - éléments liés.

6. Par défaut, la note est privée.
7. Le MJ peut lier la note à un scénario, une scène, un PNJ, un personnage ou une session.
8. Le MJ sauvegarde la note.
9. La note devient accessible depuis la liste des notes, la recherche et les entités liées.

## Scénarios alternatifs

### A1 — Note rapide

Le MJ crée une note rapide sans titre. Le système génère un titre temporaire à partir de la date ou du début du contenu.

### A2 — Note partagée

Le MJ définit la note comme visible par tous les joueurs ou par certains joueurs uniquement.

### A3 — Changement de visibilité

Le MJ transforme une note privée en note partagée ou inversement.

### A4 — Note prise pendant une session

Le MJ crée une note depuis la vue session. Elle est automatiquement liée à la session en cours.

## Exceptions

### E1 — Contenu vide

Le système peut empêcher la création d'une note vide ou autoriser une note vide comme brouillon.

### E2 — Accès joueur non autorisé

Un joueur tente d'accéder à une note privée. Le système refuse l'accès.

## Postconditions

- La note est enregistrée.
- La note est placée dans le dossier système **Notes** de la campagne par défaut.
- La note respecte sa configuration de visibilité.

## Données manipulées

### Note

- Identifiant unique
- Titre
- Contenu
- Type
- Visibilité
- Tags
- Entités liées
- Auteur
- Date de création
- Date de modification

## Règles métier

- Toute note est privée par défaut.
- Seul le MJ peut partager une note.
- Une note peut être liée à plusieurs entités.
- Un joueur ne peut consulter et voire que les notes qui lui sont explicitement accessibles.

## Critères d'acceptation

- Le MJ peut créer une note privée.
- Le MJ peut créer une note partagée.
- Le MJ peut modifier la visibilité d'une note.
- Une note privée n'est pas visible par les joueurs.
- Une note peut être retrouvée via la recherche.

## Questions à valider en interview

- Comment les MJ prennent-ils leurs notes aujourd'hui ?
- Les notes sont-elles plutôt longues, courtes, structurées ou libres ?
- Les MJ ont-ils besoin de partager certaines notes ?
- Comment gèrent-ils les secrets et les informations connues des joueurs ?
- Quelles notes doivent être retrouvées rapidement en session ?

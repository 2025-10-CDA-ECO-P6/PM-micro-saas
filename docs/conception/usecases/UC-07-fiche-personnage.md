# UC-07 — Créer et gérer une fiche personnage

## Acteurs principaux

MJ et joueur

## Acteurs secondaires

Aucun.

## Objectif

Centraliser les fiches personnages pour permettre au joueur et au MJ d'y accéder facilement.

## Contexte

Les fiches personnages sont parfois conservées sur papier, en PDF, en image ou dans des documents séparés. Elles peuvent être oubliées, perdues ou difficiles à consulter rapidement par le MJ.

## Besoin utilisateur

Le joueur veut accéder facilement à sa fiche. Le MJ veut pouvoir consulter rapidement les informations utiles des personnages pendant la session.

## Déclencheur

Un joueur rejoint une campagne ou un personnage doit être créé.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal côté MJ

1. Le MJ ouvre une campagne.
2. Il accède à la section "Personnages".
3. Il clique sur "Créer un personnage".
4. Il renseigne :
   - nom du personnage ;
   - joueur associé ;
   - description ;
   - caractéristiques principales ;
   - notes MJ éventuelles.

5. Le MJ sauvegarde la fiche.
6. La fiche devient consultable depuis la campagne et la vue session.

## Scénario nominal côté joueur

1. Le joueur accède à son espace campagne ou à un lien fourni par le MJ.
2. Il ouvre sa fiche personnage.
3. Il consulte les informations.
4. Il modifie les champs autorisés :
   - notes personnelles ;
   - inventaire ;
   - informations libres selon permissions.

5. Les modifications sont sauvegardées.

## Scénarios alternatifs

### A1 — Personnage sans joueur associé

Le MJ crée une fiche personnage avant que le joueur ne rejoigne.

### A2 — Joueur invité sans compte

Le joueur accède à sa fiche via un lien temporaire ou une session.

### A3 — Fiche générique

Le personnage est créé avec une structure générique non dépendante d'un système de jeu précis.

### A4 — Verrouillage de certains champs *(hors MVP — modélisé)*

Le MJ verrouille des champs que le joueur ne peut pas modifier. Le champ `DocumentBlock.isLocked: Boolean` est modélisé dans le domaine (valeur par défaut `false`) mais cette fonctionnalité n'est pas activée dans le MVP.

## Exceptions

### E1 — Accès non autorisé

Un joueur tente d'accéder à la fiche d'un autre joueur sans autorisation. Le système refuse l'accès.

### E2 — Modification interdite

Le joueur tente de modifier un champ verrouillé. Le système bloque la modification.

## Postconditions

- Une fiche personnage existe dans la campagne.
- La fiche est placée dans le dossier système **Personnages joueurs** de la campagne.
- Le MJ peut la consulter.
- Le joueur associé peut y accéder selon les permissions.

## Données manipulées

### Personnage

- Identifiant unique
- Nom
- Description
- Joueur associé
- Caractéristiques génériques
- Ressources simples
- Inventaire
- Notes joueur
- Notes privées MJ
- Campagne associée

## Règles métier

- Un personnage appartient à une campagne.
- Un personnage peut être associé à zéro ou un joueur.
- Le MJ peut consulter toutes les fiches personnages de sa campagne.
- Un joueur ne peut consulter que les fiches auxquelles il a accès.
- Certains champs peuvent être modifiables uniquement par le MJ.

## Critères d'acceptation

- Le MJ peut créer une fiche personnage.
- Le MJ peut associer une fiche à un joueur.
- Le joueur peut consulter sa fiche.
- Le joueur peut modifier les champs autorisés.
- Le MJ peut consulter la fiche depuis la vue session.

## Questions à valider en interview

- Les MJ ont-ils besoin de consulter les fiches joueurs pendant la partie ?
- Les joueurs accepteraient-ils d'utiliser une fiche dans l'outil ?
- Une fiche générique suffit-elle pour un MVP ?
- Quelles informations sont indispensables sur une fiche ?

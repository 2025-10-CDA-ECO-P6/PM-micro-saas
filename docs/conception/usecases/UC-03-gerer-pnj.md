# UC-03 — Créer et gérer des PNJ

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Centraliser les informations des personnages non-joueurs afin de les retrouver rapidement pendant la préparation ou la session.

## Contexte

Les PNJ sont nombreux dans une campagne. Le MJ peut devoir retenir leur nom, leur rôle, leur personnalité, leurs motivations, leurs relations avec les joueurs et leurs secrets. Ces informations sont souvent dispersées dans les notes du MJ.

## Besoin utilisateur

Le MJ veut éviter d'oublier des informations importantes sur ses PNJ et pouvoir les consulter rapidement.

## Déclencheur

Le MJ prépare un scénario, enrichit sa campagne ou crée un PNJ improvisé pendant une session.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section "PNJ".
3. Il clique sur "Créer un PNJ".
4. Le système affiche une fiche PNJ.
5. Le MJ renseigne les informations principales :
   - nom ;
   - rôle ;
   - description ;
   - personnalité ;
   - motivation ;
   - statut ;
   - informations connues des joueurs ;
   - notes privées MJ.

6. Le MJ ajoute éventuellement des relations avec d'autres entités :
   - personnages joueurs ;
   - autres PNJ ;
   - lieux ;
   - scénarios ;
   - factions.

7. Le MJ sauvegarde la fiche PNJ.
8. Le PNJ devient accessible dans la campagne, la recherche et les scénarios liés.

## Scénarios alternatifs

### A1 — Création rapide

Le MJ crée un PNJ avec seulement un nom et une note courte, par exemple pendant une session.

### A2 — Transformation d'une note en PNJ

Le MJ transforme une note existante en fiche PNJ.

### A3 — Ajout d'un PNJ depuis un scénario

Le MJ crée un PNJ directement depuis une scène de scénario.

### A4 — Archivage d'un PNJ

Le MJ archive un PNJ qui n'est plus utilisé sans le supprimer définitivement.

## Exceptions

### E1 — Nom manquant

Le système empêche la création d'un PNJ sans nom ou génère un nom temporaire selon le comportement choisi.

### E2 — Suppression accidentelle

Le système demande confirmation avant suppression définitive ou privilégie l'archivage.

## Postconditions

- Le PNJ est enregistré dans la campagne.
- Le PNJ est placé dans le dossier système **PNJ** de la campagne.
- Le PNJ peut être retrouvé par recherche.
- Le PNJ peut être lié à un scénario, une scène ou une note.

## Données manipulées

### PNJ

- Identifiant unique
- Nom
- Alias
- Rôle
- Description
- Personnalité
- Motivation
- Statut
- Informations publiques
- Notes privées
- Relations
- Tags
- Campagne associée

## Règles métier

- Un PNJ appartient à une campagne.
- Un PNJ peut être lié à plusieurs scénarios ou scènes.
- Les notes privées d'un PNJ ne sont visibles que par le MJ.
- Les informations publiques peuvent être partagées aux joueurs si le MJ le décide.

## Critères d'acceptation

- Le MJ peut créer une fiche PNJ.
- Le MJ peut modifier une fiche PNJ.
- Le MJ peut retrouver un PNJ via la recherche.
- Le MJ peut lier un PNJ à un scénario ou une scène.
- Le MJ peut distinguer les informations privées des informations partageables.

## Questions à valider en interview

- Combien de PNJ les MJ suivent-ils en moyenne ?
- Quelles informations sont réellement utiles sur une fiche PNJ ?
- Les MJ ont-ils besoin de relations entre PNJ et personnages ?
- Ont-ils besoin de fiches très détaillées ou de fiches rapides ?
- Créent-ils souvent des PNJ improvisés ?

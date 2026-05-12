# UC-08 — Gérer l'inventaire d'un personnage

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre au joueur de suivre les objets de son personnage et au MJ de les consulter si nécessaire.

## Contexte

L'inventaire est une information fréquente en jeu de rôle. Il peut être oublié ou mal synchronisé entre le joueur et le MJ, surtout quand la campagne dure longtemps.

## Besoin utilisateur

Le joueur veut suivre ses objets facilement. Le MJ veut pouvoir vérifier rapidement l'inventaire d'un personnage.

## Déclencheur

Le personnage obtient, perd, utilise ou modifie un objet.

## Préconditions

- Une fiche personnage existe.
- Le joueur a accès à la fiche ou le MJ administre la campagne.

## Scénario nominal

1. Le joueur ouvre sa fiche personnage.
2. Il accède à la section "Inventaire".
3. Il ajoute un objet avec :
   - nom ;
   - quantité ;
   - description courte ;
   - note éventuelle.

4. Il sauvegarde l'objet.
5. L'objet apparaît dans son inventaire.
6. Le MJ peut consulter l'inventaire depuis la fiche du personnage.

## Scénarios alternatifs

### A1 — Modification d'un objet

Le joueur modifie la quantité ou la description d'un objet.

### A2 — Suppression d'un objet

Le joueur retire un objet de son inventaire.

### A3 — Ajout par le MJ

Le MJ ajoute directement un objet dans l'inventaire d'un personnage.

### A4 — Objet secret

Le MJ crée un objet non visible par le joueur. Cette option est plutôt hors MVP ou à traiter simplement.

## Exceptions

### E1 — Quantité invalide

Le système refuse une quantité négative si la quantité est renseignée.

### E2 — Accès interdit

Un joueur tente de modifier l'inventaire d'un autre joueur. Le système refuse.

## Postconditions

- L'inventaire du personnage est mis à jour.
- Les modifications sont visibles par les utilisateurs autorisés.

## Données manipulées

### Objet d'inventaire

- Identifiant unique
- Nom
- Quantité
- Description
- Note
- Personnage associé

## Règles métier

- Un objet d'inventaire appartient à un personnage.
- Le joueur peut modifier son propre inventaire si le MJ l'autorise.
- Le MJ peut consulter et modifier les inventaires des personnages de sa campagne.

## Critères d'acceptation

- Le joueur peut ajouter un objet à son inventaire.
- Le joueur peut modifier un objet existant.
- Le joueur peut supprimer un objet.
- Le MJ peut consulter l'inventaire depuis la fiche personnage.

## Questions à valider en interview

- L'inventaire est-il souvent source de friction ?
- Les MJ veulent-ils valider les modifications d'inventaire ?
- Les joueurs gèrent-ils leur inventaire sérieusement ?
- L'inventaire est-il indispensable au MVP ?

# UC-08 — Partager une information aux joueurs

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, notamment quand ils partagent leurs propres LiveNotes personnelles.

## Objectif

Permettre au MJ de partager certaines informations avec les joueurs tout en conservant ses notes privées.

## Contexte

Dans une campagne, certaines informations doivent être transmises aux joueurs : indices, documents, lore, rappels ou notes communes. D'autres informations doivent rester secrètes.

## Besoin utilisateur

Le MJ veut contrôler précisément ce qui est visible ou non par les joueurs.

## Déclencheur

Le MJ souhaite transmettre une information au groupe ou à certains joueurs.

## Préconditions

- Une campagne existe.
- Des joueurs ou personnages sont associés à la campagne.
- Le contenu à partager existe ou est créé par le MJ.

## Scénario nominal

1. Le MJ ouvre une ressource partageable : document ou LiveNote.
2. Il choisit l'action "Partager".
3. Le système affiche les options de visibilité.
4. Le MJ choisit la cible :
   - tous les joueurs ;
   - certains joueurs ;
   - certains personnages ;
   - participants de la session actuelle.

5. Le MJ valide le partage.
6. Le système met la ressource en visibilité `SHARED` et crée les `ContentAccessRule` correspondant aux cibles.
7. Les joueurs voient l'information dans leur espace.

## Scénarios alternatifs

### A1 — Retirer le partage

Le MJ rend à nouveau une information privée.

### A2 — Partage à un seul joueur

Le MJ partage une information uniquement à un joueur précis.

### A3 — Partage depuis la vue session

Le MJ partage une information pendant une partie.

### A4 — Partage d'un récapitulatif post-session

Le MJ crée ou ouvre un document de récapitulatif post-session, puis le partage comme n'importe quel document.

## Exceptions

### E1 — Aucun destinataire sélectionné

Le système empêche la validation si aucune cible n'est sélectionnée.

### E2 — Joueur supprimé ou non disponible

Si un joueur n'est plus associé à la campagne, il n'apparaît plus comme cible de partage.

## Postconditions

- L'information est visible par les joueurs ciblés.
- Les notes privées restent protégées.
- Le MJ peut modifier la visibilité ultérieurement.

## Données manipulées

### Information partagée

- Identifiant du contenu
- Type de contenu
- Visibilité
- Cibles
- Date de partage
- Auteur du partage

## Règles métier

- Tout contenu est privé par défaut.
- Seul le MJ peut partager une information de campagne durable.
- Un joueur peut partager une LiveNote personnelle qu'il a créée pendant une session,
  sans obtenir de droits sur les documents de campagne.
- Un joueur ne peut consulter que les informations explicitement partagées avec lui.
- Le MJ peut retirer un partage.
- Les règles de partage sont unifiées par `AccessPolicy` et `ContentAccessRule`.
- Les cibles possibles sont : tous les membres, un membre authentifié, un personnage joueur,
  ou les participants de la session actuelle.

## Critères d'acceptation

- Le MJ peut partager une note avec tous les joueurs.
- Le MJ peut partager une note avec un joueur spécifique.
- Le MJ peut retirer un partage.
- Un joueur ne voit pas les notes privées.
- Les joueurs voient les informations partagées dans leur espace.

## Questions à valider en interview

- Les MJ partagent-ils souvent des documents ou notes aux joueurs ?
- Ont-ils besoin d'un partage par joueur ou seulement par groupe ?
- Ont-ils déjà eu des problèmes de spoilers ou d'informations révélées trop tôt ?
- Quels types d'informations sont généralement partagés ?

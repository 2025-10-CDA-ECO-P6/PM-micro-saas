# UC-13 — Gérer ses notes personnelles (joueur)

## Acteur principal

Joueur

## Acteurs secondaires

Aucun.

## Objectif

Permettre au joueur de consigner des notes personnelles sur la campagne, visibles uniquement par lui-même.

## Contexte

Un joueur peut avoir besoin de noter des informations qu'il ne souhaite pas partager avec le MJ : théories, mémos personnels, objectifs secrets, rappels. Ces notes doivent être strictement privées, invisibles au MJ et aux autres joueurs.

## Besoin utilisateur

Le joueur veut pouvoir prendre des notes personnelles sans qu'elles soient accessibles au MJ ou aux autres participants.

## Déclencheur

Le joueur souhaite noter une information personnelle pendant ou en dehors d'une session.

## Préconditions

- Le joueur est associé à une campagne.
- Le joueur dispose d'un compte ou d'un accès GuestAccess.

## Scénario nominal

1. Le joueur accède à son espace dans la campagne.
2. Il crée une nouvelle note.
3. Il choisit la visibilité "Personnel" (`PLAYER_PRIVATE`).
4. Il saisit le contenu de la note.
5. Il sauvegarde.
6. La note est stockée avec `visibility = PLAYER_PRIVATE` et `createdById = userId`.
7. La note n'apparaît pas dans les vues MJ ni dans les vues des autres joueurs.

## Scénarios alternatifs

### A1 — Note créée pendant la session

Le joueur prend une note personnelle directement depuis la vue de session active.

### A2 — Modification d'une note existante

Le joueur revient sur une note personnelle pour la compléter ou la corriger.

### A3 — Suppression

Le joueur supprime une note personnelle. Le MJ n'est pas notifié.

## Exceptions

### E1 — Tentative d'accès par le MJ

Le système refuse l'accès : une note `PLAYER_PRIVATE` n'est accessible qu'au joueur `createdById`, quelle que soit la requête.

### E2 — Joueur sans accès actif

Si l'accès du joueur a expiré (GuestAccess), ses notes `PLAYER_PRIVATE` restent en base mais inaccessibles jusqu'à réactivation.

## Postconditions

- La note est sauvegardée avec `visibility = PLAYER_PRIVATE`.
- Aucun autre utilisateur (MJ inclus) ne peut consulter cette note.
- La note est accessible au joueur dans les sessions suivantes.

## Données manipulées

- Document / Note avec `visibility = PLAYER_PRIVATE`
- `createdById: UserId` (discriminant d'accès)
- Campagne associée
- Session associée (optionnel)

## Règles métier

- Une note `PLAYER_PRIVATE` est visible uniquement par son créateur (`createdById`).
- Le MJ n'a pas accès aux notes `PLAYER_PRIVATE`, même s'il est propriétaire de la campagne.
- La règle d'accès est appliquée par le domaine (`ContentAccessPolicy`) et non par la couche applicative seule.
- Les notes `PLAYER_PRIVATE` n'apparaissent pas dans les résultats de recherche du MJ.

## Critères d'acceptation

- Le joueur peut créer une note avec visibilité `PLAYER_PRIVATE`.
- La note n'est pas visible dans la vue MJ de la campagne.
- La note n'est pas retournée par la recherche du MJ.
- Le joueur peut modifier et supprimer ses propres notes personnelles.
- La note est accessible au joueur lors des sessions suivantes.

# UC-12 — Clôturer une session et préparer la suite

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si un résumé est partagé.

## Objectif

Conserver une trace de la session jouée et préparer la continuité narrative.

## Contexte

Après une session, le MJ doit souvent se souvenir des décisions prises, des PNJ rencontrés, des objets obtenus, des conséquences narratives et des éléments à préparer pour la prochaine partie.

## Besoin utilisateur

Le MJ veut éviter de perdre les informations importantes entre deux sessions.

## Déclencheur

Une session se termine.

## Préconditions

- Une session existe.
- Le MJ est dans la campagne concernée.

## Scénario nominal

1. Le MJ termine une session depuis la vue session.
2. Le système propose une étape de clôture.
3. Le MJ rédige un résumé de session.
4. Il ajoute les éléments importants :
   - décisions des joueurs ;
   - PNJ rencontrés ;
   - objets obtenus ;
   - conséquences ;
   - pistes pour la suite.

5. Le MJ choisit si le résumé reste privé ou devient partagé.
6. Le MJ valide la clôture.
7. La session passe au statut "terminée".
8. Le résumé est sauvegardé dans la campagne.

## Scénarios alternatifs

### A1 — Clôture sans résumé

Le MJ termine la session sans rédiger de résumé.

### A2 — Résumé ajouté plus tard

Le MJ revient sur une session terminée pour compléter le résumé.

### A3 — Version privée et version partagée *(hors MVP)*

Le MJ garde une version privée détaillée et partage une version simplifiée aux joueurs. Dans le MVP, un seul résumé existe avec une visibilité paramétrable (privée ou partagée).

### A4 — Création automatique de notes de suivi

Certaines informations du résumé peuvent être transformées en notes ou tâches de préparation. Cette fonctionnalité est plutôt hors MVP.

## Exceptions

### E1 — Clôture accidentelle

Le MJ peut modifier le contenu d'une session CLOSED (résumé, LiveNotes rétroactives) sans changer son statut. La machine d'états est unidirectionnelle : PLANNED → LIVE → CLOSED → ARCHIVED. Une session CLOSED reste éditable ; une session ARCHIVED est en lecture seule complète.

### E2 — Erreur de sauvegarde

Le système avertit le MJ et conserve le contenu saisi si possible.

## Postconditions

- La session est marquée comme terminée.
- Un résumé peut être consulté dans l'historique de campagne.
- Le résumé peut être partagé aux joueurs selon la visibilité choisie.

## Données manipulées

### Résumé de session

- Identifiant unique
- Session associée
- Contenu
- Éléments liés
- Visibilité
- Date de création
- Date de modification

## Règles métier

- La machine d'états Session est unidirectionnelle : PLANNED → LIVE → CLOSED → ARCHIVED.
- Une session CLOSED est consultable et son contenu reste éditable (résumé, LiveNotes rétroactives).
- Une session ARCHIVED est en lecture seule complète.
- Le résumé est privé par défaut.
- Le MJ choisit explicitement ce qui est partagé.
- "Rouvrir" une session CLOSED signifie modifier son contenu, pas revenir au statut LIVE.

## Critères d'acceptation

- Le MJ peut clôturer une session (LIVE → CLOSED).
- Le MJ peut rédiger ou modifier le résumé d'une session CLOSED.
- Le MJ peut ajouter des LiveNotes rétroactives sur une session CLOSED.
- Le résumé est sauvegardé.
- Le MJ peut choisir de partager ou non le résumé.
- Une session terminée apparaît dans l'historique.
- Une session ARCHIVED ne peut plus être éditée.

## Questions à valider en interview

- Les MJ font-ils des résumés après session ?
- Les résumés sont-ils destinés au MJ, aux joueurs ou aux deux ?
- Quelles informations sont importantes à conserver ?
- Les MJ veulent-ils préparer la prochaine session à partir du résumé ?

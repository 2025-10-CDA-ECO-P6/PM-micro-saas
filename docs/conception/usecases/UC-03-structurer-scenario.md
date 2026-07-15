# UC-03 — Structurer un scénario

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Permettre au MJ de préparer et structurer un scénario exploitable pendant une session,
en s'appuyant sur le modèle documentaire modulaire.

## Contexte

Le scénario est un élément central de la préparation du MJ. Dans Haversack, un scénario est
un document d'espace : il peut être rédigé librement en blocs ou structuré en scènes
liées. Il peut référencer des PNJ, lieux, indices, objets, révélations ou notes.

UC-03 décrit l'organisation narrative d'un scénario. Cette activité de préparation s'applique
à tout espace : le MJ peut structurer un scénario dans son espace personnel (hors campagne,
pour une préparation autonome ou en vue d'une future instanciation — cohérent avec UC-13),
comme dans un espace de type campagne ou one-shot. Le modèle général des documents,
blocs, types, dossiers, templates, recherche et visibilité est décrit par UC-04.

## Besoin utilisateur

Le MJ veut préparer un scénario de manière structurée tout en gardant une certaine liberté d'improvisation.

## Déclencheur

Le MJ prépare une future session ou un arc narratif.

## Préconditions

- Un espace existe.
- Le MJ a les droits d'administration sur l'espace.

## Scénario nominal

1. Le MJ ouvre un espace.
2. Il accède à la section "Scénarios" (dossier système dans un espace `CAMPAIGN` ou `ONE_SHOT`, ou dossier créé librement dans un espace `PERSONAL`).
3. Il clique sur "Créer un scénario".
4. Le système affiche un éditeur de document scénario.
5. Le MJ renseigne les informations générales :
   - titre ;
   - résumé ;
   - contexte ;
   - objectif narratif ;
   - statut du scénario.

6. Le MJ ajoute une ou plusieurs scènes, représentées par des documents `SCENE` liés.
7. Pour chaque scène, le MJ peut renseigner :
   - titre ;
   - description ;
   - objectif ;
   - informations à révéler aux joueurs ;
   - notes privées ;
   - PNJ liés ;
   - lieux ou objets liés.

8. Le MJ sauvegarde le scénario.
9. Le scénario devient disponible dans l'espace et peut être utilisé dans une session.

## Scénarios alternatifs

### A1 — Scénario libre sans découpage en scènes

Le MJ peut créer un scénario sous forme de document monobloc sans utiliser le découpage en scènes.

### A2 — Ajout d'éléments liés existants

Le MJ lie au scénario des documents déjà existants dans l'espace : PNJ, lieux, notes,
révélations, personnages joueurs, aides de jeu ou autres contenus.

### A3 — Création d'un élément depuis le scénario

Le MJ crée un nouveau document directement depuis l'éditeur de scénario. Il est automatiquement
lié au scénario ou à la scène courante.

### A4 — Scénario improvisé

Le MJ crée un scénario minimal pendant ou juste avant une session, avec uniquement un titre
et quelques blocs libres.

## Exceptions

### E1 — Titre manquant

Le système empêche la création si le scénario n'a pas de titre.

### E2 — Perte de connexion ou erreur de sauvegarde

Le système conserve les données saisies localement si possible et affiche un message d'erreur.

## Postconditions

- Le scénario est sauvegardé comme document de l'espace.
- Le scénario est placé dans le dossier **Scénarios** de l'espace (dossier système dans un espace `CAMPAIGN` ou `ONE_SHOT` ; dossier créé librement par le MJ dans un espace `PERSONAL`).
- Le scénario peut être consulté, modifié ou lié à une session.
- Les éléments liés au scénario sont accessibles depuis celui-ci.

## Données manipulées

### Scénario

- Identifiant unique
- Type de document `SCENARIO`
- Titre
- Résumé
- Contexte
- Objectif narratif
- Statut
- Espace associé
- Scènes
- Documents liés

### Scène

- Identifiant unique
- Type de document `SCENE`
- Titre
- Description
- Objectif
- Ordre d'affichage
- Notes
- Informations partageables
- Documents liés : PNJ, lieux, objets, révélations ou autres contenus

## Règles métier

- Un scénario est un document d'espace spécialisé pour la préparation narrative.
- Une scène est un document d'espace lié au scénario.
- Un scénario peut référencer zéro, une ou plusieurs scènes.
- Un document `SCENE` peut être lié depuis plusieurs scénarios (cardinalité n↔n via `DocumentLink`, aucune contrainte d'unicité sur `targetDocumentId`) — le même mécanisme qu'un PNJ partagé entre plusieurs scènes. « Ajouter une scène » crée toujours une scène fraîche possédée (create+link) ; le partage multi-parent n'est atteignable que via « lier un document existant » (SCENE est un type cible éligible de ce chemin). Retirer une scène d'un scénario supprime le lien, pas le document — la scène survit pour les autres scénarios qui la référencent ; pas de suppression en cascade. Le backlink calculé « référencée par N scénarios » est surfacé pour signaler le partage. Une scène partagée au niveau template n'affecte pas les instances (`Document.Instantiate`, UC-13) — chaque instance reste indépendante.
- Une scène peut référencer plusieurs documents : PNJ, lieux, objets, révélations, notes ou aides de jeu.
- Les notes privées d'un scénario restent dans des documents privés pour le MJ.
- Un scénario peut porter un statut de préparation dans ses propriétés structurées.

## Critères d'acceptation

- Le MJ peut créer un scénario dans un espace.
- Le MJ peut ajouter, modifier et supprimer des scènes.
- Le MJ peut lier un PNJ à une scène.
- Le MJ peut sauvegarder un scénario comme brouillon.
- Le scénario peut être retrouvé depuis l'espace.

## Questions à valider en interview

- Les MJ structurent-ils leurs scénarios en scènes, actes, lieux ou événements ?
- Ont-ils besoin d'un éditeur très libre ou très guidé ?
- Quelles informations doivent absolument apparaître dans un scénario ?
- À quel moment préparent-ils leurs scénarios ?
- Utilisent-ils des templates ou des structures récurrentes ?

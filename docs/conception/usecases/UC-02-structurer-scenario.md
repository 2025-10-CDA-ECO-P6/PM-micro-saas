# UC-02 — Structurer un scénario

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Permettre au MJ de préparer et structurer un scénario exploitable pendant une session.

## Contexte

Le scénario est un élément central de la préparation du MJ. Il peut contenir des scènes, des objectifs narratifs, des PNJ, des lieux, des indices, des objets et des notes privées. Si ces informations sont mal organisées, le MJ peut perdre du temps ou oublier des éléments importants pendant la partie.

## Besoin utilisateur

Le MJ veut préparer un scénario de manière structurée tout en gardant une certaine liberté d'improvisation.

## Déclencheur

Le MJ prépare une future session ou un arc narratif.

## Préconditions

- Une campagne existe.
- Le MJ a les droits d'administration sur la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section "Scénarios".
3. Il clique sur "Créer un scénario".
4. Le système affiche un éditeur de scénario.
5. Le MJ renseigne les informations générales :
   - titre ;
   - résumé ;
   - contexte ;
   - objectif narratif ;
   - statut du scénario.

6. Le MJ ajoute une ou plusieurs scènes.
7. Pour chaque scène, le MJ peut renseigner :
   - titre ;
   - description ;
   - objectif ;
   - informations à révéler aux joueurs ;
   - notes privées ;
   - PNJ liés ;
   - lieux ou objets liés.

8. Le MJ sauvegarde le scénario.
9. Le scénario devient disponible dans la campagne et peut être utilisé dans une session.

## Scénarios alternatifs

### A1 — Scénario libre sans découpage en scènes

Le MJ peut créer un scénario sous forme de note structurée sans utiliser le découpage en scènes.

### A2 — Ajout d'éléments liés existants

Le MJ lie au scénario des PNJ, notes ou personnages déjà existants dans la campagne.

### A3 — Création d'un élément depuis le scénario

Le MJ crée un nouveau PNJ ou une nouvelle note directement depuis l'éditeur de scénario.

### A4 — Scénario improvisé

Le MJ crée un scénario minimal pendant ou juste avant une session, avec uniquement un titre et quelques notes.

## Exceptions

### E1 — Titre manquant

Le système empêche la création si le scénario n'a pas de titre.

### E2 — Perte de connexion ou erreur de sauvegarde

Le système conserve les données saisies localement si possible et affiche un message d'erreur.

## Postconditions

- Le scénario est sauvegardé dans la campagne.
- Le scénario est placé dans le dossier système **Scénarios** de la campagne.
- Le scénario peut être consulté, modifié ou lié à une session.
- Les éléments liés au scénario sont accessibles depuis celui-ci.

## Données manipulées

### Scénario

- Identifiant unique
- Titre
- Résumé
- Contexte
- Objectif narratif
- Statut
- Campagne associée
- Scènes
- Éléments liés

### Scène

- Identifiant unique
- Titre
- Description
- Objectif
- Ordre d'affichage
- Notes
- Informations partageables
- PNJ liés
- Lieux liés
- Objets liés

## Règles métier

- Un scénario appartient à une campagne.
- Un scénario peut contenir zéro, une ou plusieurs scènes.
- Une scène peut être liée à plusieurs PNJ.
- Les notes privées d'un scénario ne sont visibles que par le MJ.
- Un scénario peut être dans l'un des statuts suivants : brouillon, prêt, joué, archivé.

## Critères d'acceptation

- Le MJ peut créer un scénario dans une campagne.
- Le MJ peut ajouter, modifier et supprimer des scènes.
- Le MJ peut lier un PNJ à une scène.
- Le MJ peut sauvegarder un scénario comme brouillon.
- Le scénario peut être retrouvé depuis la campagne.

## Questions à valider en interview

- Les MJ structurent-ils leurs scénarios en scènes, actes, lieux ou événements ?
- Ont-ils besoin d'un éditeur très libre ou très guidé ?
- Quelles informations doivent absolument apparaître dans un scénario ?
- À quel moment préparent-ils leurs scénarios ?
- Utilisent-ils des templates ou des structures récurrentes ?

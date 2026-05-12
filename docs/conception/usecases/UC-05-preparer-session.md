# UC-05 — Préparer une session de jeu

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si le MJ associe des participants à la session.

## Objectif

Préparer une session précise en sélectionnant les éléments de campagne utiles pour cette partie.

## Contexte

Une campagne peut contenir beaucoup d'informations. Pour une session donnée, le MJ a besoin d'un espace focalisé sur ce qui sera utilisé : scénario prévu, scènes importantes, PNJ présents, personnages joueurs, notes critiques.

## Besoin utilisateur

Le MJ veut préparer une session sans devoir parcourir toute sa campagne au moment de jouer.

## Déclencheur

Une partie est prévue à une date donnée.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.
- Des éléments de campagne peuvent déjà exister, mais ce n'est pas obligatoire.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la section "Sessions".
3. Il clique sur "Créer une session".
4. Il renseigne :
   - titre ;
   - date prévue ;
   - scénario associé ;
   - résumé d'intention.

5. Il sélectionne les éléments utiles :
   - scènes prévues ;
   - personnages participants ;
   - notes nécessaires.

6. Si un scénario est associé, le système déduit automatiquement les PNJ importants
   depuis les `linkedNpcIds` de toutes les scènes du scénario et les propose au MJ.
7. Le MJ peut ajouter ou retirer des PNJ de la sélection.
8. Le système prépare une vue session à partir de ces éléments.
9. Le MJ sauvegarde la session.

## Scénarios alternatifs

### A1 — Session sans scénario

Le MJ crée une session libre, sans scénario associé.

### A2 — Session improvisée

Le MJ crée une session au dernier moment avec très peu d'informations.

### A3 — Ajout d'éléments après création

Le MJ complète la session progressivement avant la partie.

### A4 — Duplication d'une session précédente

Le MJ duplique une session passée comme base de préparation.

## Exceptions

### E1 — Session sans titre

Le système demande un titre ou génère un titre automatique basé sur la date.

### E2 — Scénario supprimé

Si le scénario lié est supprimé ou archivé, le système informe le MJ et conserve la session sans scénario actif.

## Postconditions

- Une session est créée.
- La session est liée à une campagne.
- La session peut être lancée en mode session.

## Données manipulées

### Session

- Identifiant unique
- Titre
- Date prévue
- Statut
- Scénario associé
- Participants
- PNJ sélectionnés
- Notes sélectionnées
- Résumé d'intention

## Règles métier

- Une session appartient à une campagne.
- Une session peut être liée à zéro ou un scénario principal.
- Une session peut avoir plusieurs participants.
- Une session peut passer par les statuts : prévue, en cours, terminée, archivée.
- Si un scénario est associé, les PNJ importants (`selectedNpcIds`) sont initialisés
  automatiquement depuis l'union des `linkedNpcIds` de toutes les scènes du scénario.
- Le MJ peut modifier manuellement la liste des PNJ sélectionnés à tout moment.

## Critères d'acceptation

- Le MJ peut créer une session depuis une campagne.
- Le MJ peut associer un scénario à une session.
- Si un scénario est associé, les PNJ des scènes sont proposés automatiquement.
- Le MJ peut ajouter ou retirer des PNJ de la sélection.
- Le MJ peut sélectionner des notes utiles.
- Une session créée peut être lancée en vue session.

## Questions à valider en interview

- Les MJ préparent-ils session par session ?
- Ont-ils besoin d'une checklist avant partie ?
- Préfèrent-ils une préparation très structurée ou libre ?
- Quels éléments veulent-ils avoir sous les yeux au moment de lancer une partie ?

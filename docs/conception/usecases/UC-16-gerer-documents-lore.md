# UC-16 — Gérer les documents de lore

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, si certains documents sont partagés.

## Objectif

Permettre au MJ de créer et organiser des documents de référence pour sa campagne : lieux, factions, objets importants, lore, chronologie — tout contenu qui n'est pas un PNJ, une note ou un scénario.

## Contexte

Une campagne riche nécessite des fiches de référence variées : une description de la ville principale, la liste des factions politiques, la chronologie des événements passés, les propriétés d'un artefact clé. Ces éléments ne rentrent pas dans les catégories PNJ, personnage ou scénario, mais constituent l'ossature narrative de la campagne.

## Besoin utilisateur

Le MJ veut créer des fiches de référence organisées par type, les lier aux scénarios et PNJ concernés, et les retrouver rapidement en session.

## Déclencheur

Le MJ enrichit sa campagne avec des informations de contexte ou crée un élément narratif réutilisable.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il navigue vers un dossier existant (ex. un dossier personnalisé "Lore", "Lieux", "Factions") ou crée un nouveau dossier via UC-17.
3. Il clique sur "Créer un document".
4. Il choisit le type :
   - Lieu (`LOCATION`) ;
   - Type personnalisé (`CUSTOM`) avec un libellé libre (ex. "Faction", "Artefact", "Événement").
5. Il renseigne :
   - titre ;
   - contenu via les blocs du modèle Document (texte, champs, relations, images...) ;
   - tags ;
   - visibilité (privée par défaut).
6. Il lie optionnellement le document à :
   - des scénarios ;
   - des scènes ;
   - des PNJ ;
   - d'autres documents (via blocs `RELATION` — backlinks automatiques).
7. Il sauvegarde.

## Scénarios alternatifs

### A1 — Document partagé aux joueurs

Le MJ définit la visibilité du document sur `SHARED` avec cibles spécifiques. Les joueurs ciblés peuvent le consulter.

### A2 — Document depuis un template

Le MJ crée un document à partir d'un template (ex. "Fiche de lieu standard"). Le document hérite de la structure de blocs du template.

### A3 — Lien depuis un scénario

Le MJ crée un document de lore directement depuis l'éditeur de scénario via un bloc `RELATION`.

### A4 — Archivage

Le MJ archive un document de lore obsolète sans le supprimer.

## Exceptions

### E1 — Titre manquant

Le système refuse la création si le titre est vide.

### E2 — Type CUSTOM sans libellé

Si le type est `CUSTOM`, le champ `customType` est obligatoire.

## Postconditions

- Le document est enregistré dans la campagne.
- Il est accessible via la recherche.
- Il peut être lié à d'autres entités.
- Il respecte sa configuration de visibilité.
- Si le document contient des blocs `RELATION`, les backlinks correspondants sont automatiquement visibles depuis les documents cibles.

## Données manipulées

### Document de lore

- Type (`LOCATION` ou `CUSTOM`)
- Type personnalisé (obligatoire si `CUSTOM`)
- Titre
- Slug (unique par `campaignId` et `type`)
- Blocs de contenu (modèle DocumentBlock)
- Visibilité
- Tags
- Entités liées (scénarios, PNJ, autres documents)
- Template d'origine (snapshot)

## Règles métier

- Le slug est unique par `(campaignId, type)`.
- Un document `CUSTOM` doit avoir un `customType` non vide.
- La visibilité par défaut est `PRIVATE`.
- Une ressource partagée (`SHARED`) nécessite au moins une `ContentAccessRule` pour définir les destinataires.
- Le lien avec d'autres entités est non destructeur : supprimer un lien ne supprime ni le document ni l'entité liée.
- La protection de `GameSystem.isBuiltIn` s'applique aussi aux templates : un template `BUILTIN` ne peut pas être modifié.

## Critères d'acceptation

- Le MJ peut créer un document de type `LOCATION` ou `CUSTOM`.
- Le MJ peut composer le contenu avec des blocs libres.
- Le MJ peut lier le document à un scénario, une scène ou un PNJ.
- Le MJ peut partager un document à certains joueurs.
- Le document est retrouvable via la recherche.
- Un document `CUSTOM` sans `customType` est refusé.

## Questions à valider en interview

- Les MJ créent-ils des fiches de lieux, de factions, d'artefacts ?
- Ces fiches sont-elles souvent partagées aux joueurs ?
- Ont-ils besoin de templates pour ce type de contenu ?
- Quelle est la structure de fiche la plus utilisée en pratique ?

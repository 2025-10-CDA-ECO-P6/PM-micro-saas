# UC-05 — Organiser par dossiers

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre au MJ de structurer librement le contenu de son espace en créant ses propres dossiers,
d'associer un template par défaut à chaque dossier, et de déplacer des documents entre dossiers.

## Contexte

Chaque espace a une organisation qui lui est propre. Un MJ de D&D va vouloir des dossiers
"Factions", "Lieux", "Artefacts". Un MJ de Call of Cthulhu préférera "Suspects", "Indices",
"Lieux du crime". Imposer une structure fixe bride la créativité et force le MJ à contourner
l'outil. Le système de dossiers donne la structure à l'utilisateur, pas à l'application.

Pour un espace **`CAMPAIGN` ou `ONE_SHOT`**, quatre **dossiers système nommés** sont créés
automatiquement à la création de l'espace pour donner un point de départ neutre
(Personnages, Joueurs, Scénarios, Notes). Ces dossiers présupposent un groupe de jeu : les concepts
« Joueurs » et « Personnages » n'ont pas de sens dans un espace solo.
Ils peuvent être renommés ou supprimés comme n'importe quel autre dossier — `isSystem = true` indique
l'origine automatique, pas une contrainte de non-suppression.

Pour un espace **`PERSONAL`**, seul le dossier virtuel « Non classés » est créé automatiquement.
Le propriétaire organise son espace avec ses propres dossiers (organisation libre).

### Deux niveaux de structure documentaire

Le système distingue deux niveaux optionnels au-dessus du document libre :

**1. Template de contenu** — initialise les blocs du document à la création (markdown pré-rempli,
sections suggérées). Associé à un dossier. N'impose rien au schéma de données.

**2. Type de document** — ajoute un schéma de propriétés structurées à un document, indépendamment
de son contenu en blocs. Un document typé a des champs nommés (ex. : Nom, Rôle, PV, Secret) qui
sont accessibles en dehors du corps du document : dans la vue session, dans la recherche, en filtrage.

Ces deux niveaux sont indépendants et tous deux optionnels. Un document peut avoir :
- ni template ni type → document libre pur
- un template de contenu → démarrage guidé, liberté totale ensuite
- un type de document → propriétés structurées + contenu libre en blocs
- les deux → propriétés structurées + initialisation du contenu

### Types de document fournis par défaut

Les types suivants sont proposés nativement. Aucun n'est imposé — le MJ peut créer un document
libre dans n'importe quel dossier, y compris les dossiers système :

| Type | Propriétés principales |
|---|---|
| Scénario | Résumé, contexte, statut de préparation |
| Scène | Objectif, ordre via les liens, informations de préparation |
| PNJ | Nom, rôle, affiliation, description publique, secret MJ |
| Personnage joueur | Nom, joueur, description |
| Lieu | Nom, ambiance, connexions |
| Note | Contenu libre, rappel, idée |
| note de session | Métadonnées de session, auteur ou personnage associé |
| Révélation | Contenu préparé pour partage explicite aux joueurs |

Ces types sont un point de départ. Un objet, une faction, une aide de jeu ou tout autre contenu
peut rester un document libre, partir d'un template, ou devenir un type personnalisé dans une
évolution ultérieure. Le MJ qui n'utilise pas les types n'en voit pas la complexité. Celui qui
les utilise bénéficie d'un affichage condensé en vue session et d'un filtrage par propriété.

### Vision long terme — Relations et règles entre types

Les propriétés structurées des types sont la fondation d'une évolution future : les **relations
typées** entre documents (ce PNJ appartient à cette faction, cet objet est porté par ce personnage).

Ces relations ouvrent ensuite la voie à un moteur de règles léger : si un personnage porte un objet
avec une propriété "dégâts", une règle peut calculer un effet. C'est l'émulation de système de jeu
sans en être un moteur complet — voir [UC-HORS-MVP](UC-HORS-MVP.md).

## Besoin utilisateur

Le MJ veut organiser son contenu selon sa propre logique, retrouver rapidement un document,
et configurer un template par défaut pour que la création dans un dossier soit immédiatement
productive.

## Déclencheur

Le MJ commence à remplir son espace et veut l'organiser, ou a besoin d'une nouvelle catégorie
de contenu non prévue par défaut.

## Préconditions

- Un espace existe.
- Le MJ est propriétaire de l'espace.

## Scénario nominal — Créer un dossier

1. Le MJ accède à la section "Dossiers" ou à la barre latérale de l'espace.
2. Il clique sur "Nouveau dossier".
3. Il renseigne :
   - nom du dossier ;
   - template par défaut (optionnel) — le template appliqué à la création de tout document dans ce dossier.
4. Il valide.
5. Le dossier apparaît dans la navigation de l'espace.
6. Le MJ peut immédiatement créer des documents dans ce dossier.

## Scénario nominal — Associer un template à un dossier

1. Le MJ ouvre les paramètres d'un dossier existant.
2. Il sélectionne ou change le template par défaut parmi les templates disponibles.
3. Il valide.
4. Tout nouveau document créé dans ce dossier est initialisé avec la structure de ce template.
5. Les documents existants dans le dossier ne sont pas modifiés automatiquement.

## Scénario nominal — Déplacer un document

1. Le MJ sélectionne un ou plusieurs documents.
2. Il choisit "Déplacer vers..." et sélectionne le dossier cible.
3. Les documents sont déplacés. Leur contenu est inchangé.

## Scénarios alternatifs

### A1 — Renommer un dossier

Le MJ renomme un dossier, y compris un dossier système. Le contenu n'est pas affecté.

### A2 — Réordonner les dossiers

Le MJ réorganise l'ordre d'affichage des dossiers dans la navigation par glisser-déposer
ou via une interface d'ordre numérique.

### A3 — Création immédiate depuis un dossier

Le MJ clique sur "+" directement depuis un dossier. Le document créé est automatiquement
placé dans ce dossier et initialisé avec son template par défaut s'il en a un.

### A4 — Document sans dossier

Un document "non classé" est automatiquement placé dans le dossier virtuel "Non classés" de l'espace.
Ce dossier est invisible dans la navigation MJ mais les documents qu'il contient sont accessibles
via la recherche et une vue "Non classés" dédiée. Le dossier associé d'un document n'est jamais nul : un document sans dossier explicite pointe vers le dossier virtuel "Non classés".

### A5 — Dossier sans template

Un dossier peut ne pas avoir de template par défaut. Dans ce cas, le document est créé
avec un contenu vide.

### A6 — Dossiers créés à la création d'un espace `CAMPAIGN` ou `ONE_SHOT`

À la création d'un espace `CAMPAIGN` ou `ONE_SHOT`, les dossiers système suivants sont créés automatiquement :
- **Personnages** — type par défaut "PNJ", renommable (ex. : "Suspects", "Contacts", "Factions")
- **Joueurs** — type par défaut "Personnage joueur", renommable
- **Scénarios** — type par défaut "Scénario", structure narrative via UC-03
- **Notes** — sans type par défaut, document libre

Ces dossiers sont renommables et supprimables. Les noms proposés sont neutres et
système-agnostiques — "Personnages" couvre PNJ, suspects, contacts, factions selon le système.
Le MJ de Blades in the Dark peut renommer "Personnages" en "Factions" et changer le type par défaut.

À la création d'un espace **`PERSONAL`**, seul le dossier virtuel « Non classés » est créé.
Le MJ organise son espace personnel avec ses propres dossiers créés librement.

## Exceptions

### E1 — Nom de dossier vide

Le système refuse la création si le nom est vide.

### E2 — Suppression d'un dossier non vide

Le MJ tente de supprimer un dossier qui contient des documents. Le système demande
ce qu'il faut faire :
- Déplacer les documents vers un autre dossier (sélection de la cible).
- Garder les documents sans dossier ("non classés").

La suppression est impossible si aucune option n'est choisie.

### E3 — Suppression d'un dossier système

Les dossiers système peuvent être supprimés comme n'importe quel autre dossier. Si le dossier
contient des documents, le système applique la même logique qu'en E2 (déplacer ou laisser non classé).
`isSystem = true` indique l'origine automatique du dossier, pas une contrainte de non-suppression.

### E4 — Template supprimé après association

Si le template par défaut d'un dossier est supprimé, le dossier perd son association
sans erreur. Les documents déjà créés conservent leur contenu (ils sont des snapshots indépendants).

## Postconditions

- Le dossier existe dans l'espace et est navigable.
- Les documents qu'il contient sont accessibles depuis le dossier.
- Le template par défaut est appliqué à tout nouveau document créé dans ce dossier.
- Les documents existants ne sont pas modifiés.

## Données manipulées

### Dossier

- Identifiant unique
- Nom
- Espace associé
- Template par défaut (optionnel)
- Indicateur système (`isSystem`)
- Ordre d'affichage
- Audit

### Document (mise à jour)

- Dossier associé (dossier associé, non-nullable — pointe vers le dossier virtuel "Non classés" si aucun dossier explicite)

## Règles métier

- Un dossier appartient à un espace.
- Un dossier peut avoir zéro ou un template par défaut.
- Les dossiers système peuvent être supprimés. `isSystem = true` est informatif.
- Tous les dossiers (y compris système) peuvent être renommés.
- Un document appartient toujours à exactement un dossier (dossier associé non-nullable). Si son dossier est supprimé, il est automatiquement déplacé vers le dossier virtuel "Non classés".
- Changer le template par défaut d'un dossier n'affecte pas les documents existants.
- La suppression d'un dossier nécessite de traiter les documents qu'il contient.
- L'ordre des dossiers est persisté et géré par un service (mécanisme de classement des dossiers).

## Critères d'acceptation

- Le MJ peut créer un dossier avec un nom.
- Le MJ peut associer un template par défaut à un dossier.
- Le MJ peut renommer n'importe quel dossier, y compris les dossiers système.
- Le MJ peut supprimer un dossier système comme n'importe quel autre dossier.
- Le MJ peut supprimer un dossier en traitant son contenu (déplacer ou laisser non classé).
- Le MJ peut déplacer un document d'un dossier à un autre.
- À la création d'un espace `CAMPAIGN` ou `ONE_SHOT`, les 4 dossiers système existent avec leurs templates. À la création d'un espace `PERSONAL`, seul le dossier virtuel « Non classés » existe.
- Un document créé depuis un dossier avec template est initialisé avec ce template.
- Les documents existants ne sont jamais modifiés lors d'un changement de template par défaut.
- Le MJ peut réordonner ses dossiers.

## Questions à valider en interview

- Comment les MJ organisent-ils aujourd'hui leur contenu dans Notion ou équivalent ?
- Ont-ils des catégories récurrentes au-delà de PNJ/personnages ?
- Le fait de "choisir" une structure est-il vécu comme une liberté ou une charge ?
- Les MJ ont-ils besoin de sous-dossiers, ou un niveau est-il suffisant ?
- Un template automatique à la création dans un dossier est-il intuitif ou perturbant ?

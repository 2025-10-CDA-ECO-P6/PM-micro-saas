# UC-04 — Gérer les documents d'une campagne

## Acteur principal

MJ

## Acteurs secondaires

Joueurs, uniquement si certains documents sont partagés.

## Objectif

Permettre au MJ de créer, modifier, structurer, organiser, retrouver et éventuellement partager
des documents de campagne modulaires : notes libres, scénarios, scènes, fiches de PNJ, lieux,
factions, objets, lore, aides de jeu, résumés ou idées.

## Contexte

La capture et la structuration d'information durable sont un besoin central pour un MJ.
Dans Haversack, le document de campagne est l'unité fonctionnelle utilisée pour conserver
ce que le MJ prépare, improvise, relie et partage.

Une note ne disparaît pas : elle devient un cas simple de document, généralement libre
ou associée au type "note". Le même modèle permet aussi de représenter un scénario, une scène, un PNJ,
un lieu, une faction, un objet, un élément de lore ou un récapitulatif de session.

Le document reste utilisable avec très peu de structure : un titre et quelques blocs suffisent.
Les types, propriétés structurées, templates, tags et liens enrichissent le document sans imposer
une structure rigide ni une logique centrée sur un système de jeu particulier.

## Besoin utilisateur

Le MJ veut centraliser son contenu durable, le structurer à son niveau de besoin, le relier
aux autres éléments de campagne et le retrouver rapidement sans devoir attendre que l'application
fournisse un écran spécialisé pour chaque cas.

## Déclencheur

Le MJ prépare une campagne, rédige un scénario, crée une fiche, improvise une idée,
réorganise son contenu ou transforme une capture de session en document durable.

## Préconditions

- Une campagne existe.
- Le MJ a accès à la campagne.

## Scénario nominal

1. Le MJ ouvre une campagne.
2. Il accède à la bibliothèque de documents, à un dossier ou à une zone de création rapide.
3. Il clique sur "Créer un document" ou "Créer une note rapide".
4. Le système affiche l'éditeur de document.
5. Le MJ renseigne :
   - titre ;
   - type de document optionnel ;
   - propriétés structurées dépendantes du type, si un type est choisi ;
   - contenu libre en blocs ;
   - visibilité ;
   - tags ;
   - documents liés.

6. Par défaut, le document est privé pour le MJ.
7. Le MJ peut ajouter des blocs de contenu : texte libre, descriptions, listes, checklists,
   tableaux, images, séparateurs ou autres blocs disponibles.
8. Le MJ peut lier le document à d'autres documents : scénario, scène, PNJ, lieu, aide de jeu,
   résumé, révélation ou tout autre contenu de campagne.
9. Le MJ sauvegarde le document.
10. Le document devient accessible depuis son dossier, la recherche, ses liens et les vues
    qui consomment la bibliothèque de contenu.

## Clarification du modèle documentaire

### Document libre

Un document libre n'a pas de type particulier. Il repose sur son titre, ses blocs,
ses tags, sa visibilité, son dossier et ses liens. C'est le mode le plus léger : idéal pour
Nadia, Rémi ou Émilie quand la valeur attendue est une page immédiatement utilisable.

### Document typé

Un document typé utilise une catégorie fonctionnelle, par exemple note, PNJ, lieu, personnage
joueur, scénario, scène, note de session ou révélation. Le type peut proposer des propriétés
structurées utiles pour l'affichage condensé, le filtrage ou la recherche, mais le corps
du document reste libre et composé de blocs.

### Blocs documentaires

Les blocs forment le contenu libre du document. Ils permettent de représenter des formats variés
sans créer un objet dédié pour chaque besoin : texte, description, liste, checklist, tableau,
image, séparation, résumé, indices, secrets, informations joueurs ou préparation MJ.

Un bloc ne porte pas la visibilité dans le MVP. Si le MJ veut séparer un contenu privé d'un
contenu destiné aux joueurs, il crée ou utilise un document lié avec sa propre visibilité
(privé MJ ou visible par les joueurs). Cette règle évite de transformer le bloc en fourre-tout
de droits.

### Templates

Un document peut servir de modèle réutilisable. Lorsqu'un dossier définit un modèle par défaut,
la création depuis ce dossier part d'une copie indépendante. Le modèle accélère la création,
mais n'impose pas une structure durable : le MJ peut modifier les blocs et le type après création.

### Notes

Une note est un document, généralement libre ou associée au type "note". Une note rapide peut recevoir un
titre généré par l'interface et être complétée plus tard. Elle reste durable, recherchable,
déplaçable dans un dossier, partageable selon sa visibilité et liée à d'autres documents.

Les notes de session relèvent de UC-06/UC-07 : ce sont des notes prises dans le contexte
d'une session, puis conservées si elles doivent rester utiles après la partie.

### Fiches PNJ, lieux, factions et objets

Une fiche de PNJ ou de lieu peut être un document typé avec des propriétés structurées
et des blocs libres. Une faction, un objet, une aide de jeu ou un élément de lore
peut être représenté par un document libre, par un document typé si un type existe, par un template,
par des propriétés structurées ou par des liens vers d'autres documents. Un objet métier dédié
n'est nécessaire que si un comportement métier spécifique le justifie.

## Scénarios alternatifs

### A1 — Note rapide

Le MJ crée une note rapide sans titre. Le système génère un titre temporaire à partir de la date ou du début du contenu.
Cette note reste un document standard.

### A2 — Document typé

Le MJ choisit un type optionnel. Le système affiche les propriétés structurées associées,
sans bloquer l'édition libre en blocs.

### A3 — Document créé depuis un template

Le MJ crée un document depuis un dossier avec template par défaut. Le système instancie une copie
indépendante du template. Les modifications ultérieures n'affectent pas le template source.

### A4 — Changement de visibilité

Le MJ rend un document visible par les joueurs ou retire ce partage depuis les actions prévues
dans l'interface. Les règles détaillées de partage restent dans UC-08.

### A5 — Document utilisé pendant une session

Un document durable peut être sélectionné, consulté ou épinglé dans la vue session. UC-04 décrit
le modèle documentaire ; UC-06 décrit l'usage pendant la session : documents épinglés, notes
de session et vue joueur.

### A6 — Document créé à la volée

Le MJ crée rapidement un document depuis la vue session. UC-04 définit le document créé ;
UC-07 définit le workflow de création rapide et son éventuel épinglage dans la session.

## Exceptions

### E1 — Contenu vide

Le système peut empêcher la création d'un document sans titre. Un document avec titre mais sans
blocs de contenu peut être autorisé comme brouillon.

### E2 — Accès joueur non autorisé

Un joueur tente d'accéder à un document privé ou à une note personnelle qui ne lui appartient pas.
Le système refuse l'accès.

## Postconditions

- Le document est enregistré.
- Un document créé sans dossier explicite est placé dans le dossier virtuel **Non classés** ou,
  pour une note rapide, dans le dossier système **Notes** selon le point d'entrée choisi.
- Le document respecte sa configuration de visibilité.
- Le document est indexable par la recherche.

## Données manipulées

### Document de campagne

- Identifiant
- Campagne associée
- Dossier associé
- Titre
- Type optionnel
- Propriétés structurées optionnelles selon le type
- Contenu en blocs
- Documents liés
- Visibilité
- Tags
- Statut de modèle réutilisable
- Modèle source éventuel
- Auteur
- Dates de création et de modification
- État de suppression logique

## Règles métier

- Tout document créé par le MJ est privé par défaut.
- Un document peut rester libre, sans type.
- Un type de document ajoute des propriétés structurées sans supprimer le contenu libre.
- Un document appartient toujours à exactement un dossier.
- Un document peut être lié à plusieurs autres documents.
- Un document peut être instancié depuis un template réutilisable.
- Un document partagé reste visible par les joueurs jusqu'à retrait explicite du partage.
- Seul le MJ peut partager un document de campagne durable.
- Un joueur ne peut consulter et voir que les documents qui lui sont explicitement accessibles.
- UC-03 possède la structure narrative des scénarios et scènes, même s'ils sont représentés par des documents.
- UC-05 possède l'organisation en dossiers et les templates par défaut de dossier.
- UC-06 possède l'usage en session : documents épinglés, notes de session et vue joueur.
- UC-07 possède le workflow de création rapide pendant une session.
- UC-08 possède les règles de partage et d'accès côté joueurs.

## Critères d'acceptation

- Le MJ peut créer un document libre avec un titre et du contenu en blocs.
- Le MJ peut créer un document typé sans perdre la liberté du contenu en blocs.
- Le MJ peut créer une note rapide comme document standard.
- Le MJ peut utiliser un template pour initialiser un document.
- Le MJ peut lier un document à d'autres documents.
- Le MJ peut organiser un document dans un dossier.
- Le MJ peut créer un document privé.
- Le MJ peut partager un document avec les joueurs.
- Le MJ peut modifier la visibilité d'un document.
- Un document privé n'est pas visible par les joueurs.
- Un document peut être retrouvé via la recherche.

## Questions à valider en interview

- Comment les MJ prennent-ils leurs notes aujourd'hui ?
- Les documents sont-ils plutôt longs, courts, structurés ou libres ?
- Quels contenus les MJ veulent-ils typer, et lesquels doivent rester libres ?
- Quels blocs sont utiles sans transformer l'outil en moteur de règles ?
- Les MJ ont-ils besoin de partager certains documents ?
- Comment gèrent-ils les secrets et les informations connues des joueurs ?
- Quels documents doivent être retrouvés rapidement en session ?

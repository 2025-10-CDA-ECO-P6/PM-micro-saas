# UC-18 — Synchroniser un document avec son template

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre au MJ d'appliquer manuellement les évolutions de structure d'un template
à un document existant ou à l'ensemble des documents d'un dossier, sans écraser
le contenu déjà saisi.

## Contexte

Les documents sont créés comme des **snapshots indépendants** de leur template (ADR-09).
Modifier un template n'affecte pas les documents existants — c'est intentionnel.

Mais un MJ peut avoir créé 15 fiches PNJ et souhaiter ensuite ajouter un nouveau champ
"Faction" à toutes ses fiches. Sans mécanisme de synchronisation, il doit ouvrir chaque
document et ajouter le bloc manuellement. Ce use case couvre ce besoin sans renoncer
au principe du snapshot.

**La règle fondamentale** : la synchronisation n'écrase jamais le contenu existant.
Elle ajoute les blocs manquants depuis le template et ignore les blocs déjà présents.
Le MJ reste maître de ses données.

## Besoin utilisateur

Le MJ veut faire évoluer la structure de ses fiches sans repartir de zéro
et sans perdre le contenu qu'il a déjà saisi.

## Déclencheur

Le MJ a modifié un template et veut répercuter les changements structurels
sur des documents existants.

## Préconditions

- Un template existe et a été modifié depuis la création du document.
- Le document cible a été créé depuis ce template (ou un template compatible).
- Le MJ a accès à la campagne.

## Scénario nominal — Synchroniser un document

1. Le MJ ouvre un document.
2. Il constate qu'une version plus récente du template est disponible
   (indicateur dans l'interface : "Nouvelle version du template disponible").
3. Il clique sur "Synchroniser avec le template".
4. Le système affiche une prévisualisation des changements :
   - blocs qui seraient ajoutés (présents dans le template, absents du document) ;
   - blocs ignorés (présents dans le document, absents du template — conservés tels quels) ;
   - aucun bloc existant n'est modifié ni supprimé.
5. Le MJ confirme.
6. Les blocs manquants sont ajoutés à la fin du document (ou à leur position dans le template
   si le document n'a pas été réorganisé).
7. Le contenu existant est inchangé.

## Scénario nominal — Synchroniser tous les documents d'un dossier

1. Le MJ accède aux paramètres d'un dossier.
2. Il clique sur "Synchroniser tous les documents avec le template".
3. Le système affiche un résumé : nombre de documents concernés, blocs qui seront ajoutés.
4. Le MJ confirme.
5. La synchronisation est appliquée à tous les documents du dossier qui ont été créés
   depuis ce template.
6. Les documents créés depuis un autre template ou sans template ne sont pas affectés.

## Scénarios alternatifs

### A1 — Document modifié manuellement (structure divergente)

Le MJ a supprimé ou réorganisé des blocs dans le document depuis sa création.
La synchronisation ajoute uniquement les blocs du template qui n'existent pas encore
dans le document, quelle que soit leur position. Elle ne restaure pas les blocs supprimés.

### A2 — Aucun changement à appliquer

Si le document est déjà à jour avec la version actuelle du template, le système
l'indique et ne propose pas de synchronisation.

### A3 — Synchronisation depuis la vue dossier sur une sélection

Le MJ sélectionne plusieurs documents (pas nécessairement tout le dossier) et applique
la synchronisation à la sélection.

## Exceptions

### E1 — Template supprimé

Si le template d'origine du document a été supprimé, la synchronisation est impossible.
Le document conserve son contenu actuel.

### E2 — Erreur pendant la synchronisation en masse

Si la synchronisation échoue sur un ou plusieurs documents (ex. : problème de sauvegarde),
le système liste les documents en erreur. Les documents traités avec succès ne sont pas annulés.

## Postconditions

- Les blocs manquants ont été ajoutés au(x) document(s).
- Aucun contenu existant n'a été écrasé ni supprimé.
- La version de template appliquée est enregistrée sur le document.

## Données manipulées

- Document (blocs ajoutés)
- Template (structure de référence)
- `Document.appliedTemplateVersion` — version du template appliquée au document (pour détecter si une mise à jour est disponible)

## Règles métier

- La synchronisation n'écrase jamais le contenu existant d'un bloc.
- La synchronisation n'ajoute que les blocs présents dans le template et absents du document.
- La synchronisation ne supprime jamais un bloc du document, même s'il a été retiré du template.
- La synchronisation est une action manuelle et explicite — jamais automatique.
- Un document peut être synchronisé plusieurs fois sans effet de bord.
- La synchronisation en masse s'applique uniquement aux documents liés au template du dossier.

## Critères d'acceptation

- Le MJ peut synchroniser un document individuel avec son template.
- Le MJ voit une prévisualisation des blocs qui seront ajoutés avant de confirmer.
- Aucun contenu existant n'est modifié après synchronisation.
- Le MJ peut synchroniser tous les documents d'un dossier en une action.
- Un document déjà à jour indique qu'aucune synchronisation n'est nécessaire.
- La synchronisation fonctionne même si le document a été modifié manuellement depuis sa création.

## Questions à valider en interview

- Les MJ font-ils souvent évoluer leurs templates après avoir créé des documents ?
- Préfèrent-ils voir un diff avant d'appliquer, ou juste confirmer ?
- La synchronisation en masse est-elle plus utile que document par document ?
- Serait-il utile d'indiquer visuellement qu'un document est "derrière" la version du template ?

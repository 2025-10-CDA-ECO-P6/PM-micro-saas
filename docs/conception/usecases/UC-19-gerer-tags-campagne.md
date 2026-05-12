# UC-19 — Gérer les tags d'une campagne

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre au MJ de créer, modifier et supprimer des tags au niveau de la campagne, afin de les utiliser pour organiser et filtrer ses documents.

## Contexte

Les tags sont des étiquettes réutilisables créées au niveau d'une campagne. Un MJ peut créer des tags comme "Important", "Révélé aux joueurs", "À développer" ou toute catégorie propre à sa narration. Ces tags sont ensuite appliqués aux documents (PNJ, notes, scénarios, etc.) pour faciliter la navigation et le filtrage.

Un tag appartient à une campagne — il n'est pas partagé entre campagnes.

## Besoin utilisateur

Le MJ veut organiser ses documents avec des étiquettes personnalisées, pouvoir les renommer et les supprimer sans impacter l'intégrité de ses données.

## Déclencheur

Le MJ accède à la gestion des tags depuis les paramètres de campagne ou tente d'appliquer un tag inexistant depuis un document.

## Préconditions

- Une campagne existe.
- Le MJ est le propriétaire ou un membre avec les droits de gestion de la campagne.
- Le MJ est authentifié.

---

## Scénario nominal — Créer un tag

1. Le MJ accède à la section "Tags" dans les paramètres de la campagne.
2. Il clique sur "Nouveau tag".
3. Il saisit :
   - un libellé (obligatoire) ;
   - une couleur (optionnelle).
4. Le système valide l'unicité du libellé dans la campagne (case-insensitive).
5. Le système crée le tag avec `TagId`, `campaignId`, `label`, `color?`.
6. Le tag apparaît dans la liste des tags de la campagne.
7. Le tag est disponible pour être appliqué à n'importe quel document de la campagne.

---

## Scénario nominal — Modifier un tag

1. Le MJ sélectionne un tag existant dans la liste.
2. Il modifie le libellé et/ou la couleur.
3. Le système valide l'unicité du nouveau libellé (si changé).
4. Le système met à jour le tag.
5. Tous les documents portant ce tag voient automatiquement la modification reflétée (le libellé est résolu depuis l'entité `Tag`, pas dénormalisé dans `DOCUMENT_TAG`).

---

## Scénario nominal — Supprimer un tag

1. Le MJ sélectionne un tag et clique sur "Supprimer".
2. Le système affiche une confirmation : "Ce tag est appliqué à N documents. La suppression le retirera de tous ces documents."
3. Le MJ confirme.
4. Le système émet l'événement `TagDeleted`.
5. Un handler purge toutes les entrées de `DOCUMENT_TAG` pour ce `TagId`.
6. Le tag est supprimé physiquement.

---

## Scénario nominal — Appliquer un tag à un document

1. Depuis l'éditeur d'un document, le MJ clique sur le sélecteur de tags.
2. Le système affiche la liste des tags de la campagne.
3. Le MJ sélectionne un ou plusieurs tags.
4. Le système crée les entrées dans `DOCUMENT_TAG` pour chaque `(documentId, tagId)` sélectionné.
5. Les tags apparaissent sur le document.

---

## Scénario nominal — Retirer un tag d'un document

1. Depuis l'éditeur d'un document, le MJ clique sur un tag appliqué.
2. Il le retire.
3. Le système supprime l'entrée `DOCUMENT_TAG` correspondante.
4. Le tag disparaît du document mais reste dans la liste de la campagne.

---

## Scénarios alternatifs

### A1 — Création d'un tag depuis le sélecteur de tags d'un document

Le MJ commence à taper un libellé dans le sélecteur de tags d'un document. Si aucun tag correspondant n'existe, l'interface propose "Créer le tag « [libellé] »". Le MJ confirme et le tag est créé à la volée, puis immédiatement appliqué au document.

### A2 — Filtrage des documents par tag

Le MJ clique sur un tag depuis la liste des tags de la campagne. L'interface filtre la vue des documents pour n'afficher que ceux portant ce tag.

### A3 — Filtrage multi-tags

Le MJ sélectionne plusieurs tags simultanément. L'interface filtre les documents qui portent **tous** les tags sélectionnés (logique ET).

---

## Exceptions

### E1 — Libellé déjà utilisé

Le MJ tente de créer ou de renommer un tag avec un libellé qui existe déjà dans la campagne (comparaison case-insensitive). Le système refuse et affiche : "Un tag avec ce nom existe déjà dans cette campagne."

### E2 — Libellé vide

Le MJ tente de créer un tag sans libellé. Le système refuse et indique que le libellé est obligatoire.

### E3 — Tag appliqué à de nombreux documents

Avant la suppression, le système affiche le nombre de documents affectés. Si le nombre est élevé, un avertissement supplémentaire est affiché mais la suppression reste possible.

---

## Postconditions

### Création
- Le tag est disponible dans la campagne.
- Il peut être appliqué à n'importe quel document.

### Modification
- Le libellé et/ou la couleur sont mis à jour.
- Tous les documents portant ce tag reflètent le changement sans action supplémentaire.

### Suppression
- Le tag est supprimé de la liste de la campagne.
- Toutes les associations `DOCUMENT_TAG` pour ce tag sont supprimées.
- Les documents précédemment taggés ne sont pas affectés autrement.

---

## Données manipulées

### Tag

- `id: TagId`
- `campaignId: CampaignId`
- `label: String` (unique par campagne, case-insensitive, max 50 caractères)
- `color: String?` (code couleur hexadécimal, ex. `#FF5733`)
- `createdAt: DateTime`
- `updatedAt: DateTime`

### Association document-tag

- Table de liaison `DOCUMENT_TAG` : clé composite `(documentId, tagId)`
- Pas de colonne supplémentaire — l'association est binaire

---

## Règles métier

- Un tag appartient à une campagne — il n'est pas partagé entre campagnes.
- Le libellé est unique par campagne (unicité case-insensitive).
- La suppression d'un tag est une suppression physique (pas de soft-delete — le concept de tag supprimé n'a pas de sens métier).
- La suppression d'un tag retire le tag de tous les documents sans modifier le contenu des documents.
- Un tag sans couleur est valide — la couleur est un attribut d'affichage optionnel.
- Les tags ne sont pas hiérarchiques — pas de tag parent/enfant dans le MVP.
- Les tags sont gérés uniquement par le MJ — les joueurs ne peuvent pas créer, modifier ni supprimer des tags.
- Les joueurs peuvent voir les tags appliqués à des documents qui leur sont accessibles (les tags ne sont pas un mécanisme de contrôle d'accès).

---

## Critères d'acceptation

- Le MJ peut créer un tag avec un libellé et une couleur optionnelle.
- La création est refusée si le libellé existe déjà dans la campagne (case-insensitive).
- Le MJ peut modifier le libellé et la couleur d'un tag existant.
- La modification est refusée si le nouveau libellé est déjà pris.
- Le MJ peut supprimer un tag avec confirmation préalable indiquant le nombre de documents affectés.
- La suppression retire le tag de tous les documents associés.
- Le MJ peut appliquer un ou plusieurs tags à un document depuis l'éditeur.
- Le MJ peut créer un nouveau tag directement depuis le sélecteur de tags d'un document (A1).
- Le MJ peut filtrer les documents par tag depuis la vue campagne.
- Les joueurs voient les tags appliqués aux documents accessibles mais ne peuvent pas les gérer.

---

## Relations avec d'autres use cases

| Use Case | Relation |
|---|---|
| [UC-01](UC-01-creer-campagne.md) — Créer une campagne | Les tags sont créés dans le contexte d'une campagne existante |
| [UC-04](UC-04-gerer-notes-mj.md) — Gérer les notes MJ | Application de tags sur les notes |
| [UC-03](UC-03-gerer-pnj.md) — Gérer les PNJ | Application de tags sur les fiches PNJ |
| [UC-16](UC-16-gerer-documents-lore.md) — Gérer les documents de lore | Application de tags sur les documents de lore |
| [UC-11](UC-11-recherche.md) — Rechercher | Le filtrage par tag est une modalité de recherche |

---

## Questions à valider en interview

- Les MJ utilisent-ils des tags principalement pour organiser ou pour filtrer rapidement pendant une session ?
- Un tag partagé entre campagnes (tag "global") serait-il utile, ou la portée campagne est-elle toujours suffisante ?
- Faut-il une limite au nombre de tags par campagne ou par document ?
- La logique ET pour le filtrage multi-tags est-elle intuitive, ou les MJ s'attendraient-ils à une logique OU ?

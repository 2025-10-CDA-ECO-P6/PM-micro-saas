# UC-02 — Créer un espace de jeu (campagne ou one-shot)

## Acteur principal

MJ

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Créer un espace de travail pour organiser une campagne longue ou lancer un one-shot,
avec un niveau de configuration adapté au contexte.

## Contexte

Un espace de jeu peut prendre deux formes selon le MJ et la situation :

- **Campagne** : plusieurs sessions, groupe stable, continuité narrative.
  Antoine gère trois campagnes simultanées. Thomas a une campagne qui dure depuis deux ans.

- **One-shot** : une seule session, joueurs potentiellement différents à chaque fois,
  pas de continuité narrative. Sonia anime 3 à 5 one-shots par mois avec des groupes
  changeants. Nadia joue une session par mois. Un MJ de convention joue cinq parties
  en un week-end.

Ces deux contextes n'ont pas les mêmes besoins : une campagne se configure, un one-shot
se lance. L'application doit présenter deux points d'entrée distincts, même si le modèle
de données sous-jacent est identique.

Le MJ peut créer un espace de jeu en mode local (sans compte) ou depuis un compte cloud.
Les préconditions sont identiques dans les deux cas.

## Déclencheur

- Le MJ veut commencer à préparer une nouvelle campagne ou lancer un one-shot.
- Le MJ accède à l'application pour la première fois et veut créer son premier espace de jeu.

## Préconditions

- L'application est accessible (mode local ou compte cloud).

## Scénario nominal — Créer une campagne

1. Le MJ accède à son tableau de bord.
2. Il choisit "Nouvelle campagne".
3. Il renseigne :
   - nom de la campagne (obligatoire) ;
   - description courte (optionnel) ;
   - système de jeu (optionnel).
4. Il valide.
5. L'application crée l'espace de campagne et redirige vers son tableau de bord.
6. Quatre dossiers système sont créés automatiquement :
   **Personnages**, **Joueurs**, **Scénarios**, **Notes**.

## Scénario alternatif A1 — Lancer un one-shot (parcours express)

Le one-shot est un parcours distinct et plus court que la création de campagne.

1. Le MJ accède à son tableau de bord.
2. Il choisit "Lancer un one-shot".
3. L'application propose deux options :
   - **Scénario existant** : le MJ choisit un scénario depuis sa bibliothèque (→ UC-13).
   - **Nouveau scénario** : le MJ saisit uniquement un titre et commence directement.
4. L'application crée l'espace one-shot en une action — aucune configuration supplémentaire
   n'est demandée.
5. L'application redirige directement vers la vue session si le MJ a choisi un scénario
   existant, ou vers l'éditeur de scénario si le scénario est nouveau.

> L'espace one-shot est techniquement une campagne de type `ONE_SHOT` — même modèle de données,
> parcours utilisateur différent. Le MJ ne voit jamais ce détail d'implémentation.

### Ce qui est omis dans le one-shot :
- Pas de description ni de configuration initiale.
- Les membres joueurs rejoignent via un lien de session temporaire (UC-09 et UC-11) — pas d'invitation permanente.

## Scénario alternatif A2 — Création en mode brouillon

Le MJ commence à remplir le formulaire de campagne mais ne dispose pas encore de toutes les
informations. Il sauvegarde comme brouillon et complète plus tard.

## Scénario alternatif A3 — Campagne sans système de jeu

Le MJ ne choisit aucun système de jeu. L'espace est créé en mode générique, ce qui est le
cas par défaut recommandé pour les systèmes narratifs, les systèmes maison ou les MJ
multi-système (Antoine).

## Exceptions

### E1 — Nom manquant

Le système empêche la validation et indique que le nom est obligatoire.
Exception pour le one-shot : si le MJ a choisi un scénario existant depuis sa bibliothèque,
le nom est pré-rempli avec le titre du scénario.

### E2 — Erreur de création

Si une erreur survient lors de la sauvegarde, le système affiche un message d'erreur
et conserve les données saisies.

## Postconditions

### Campagne
- L'espace de campagne est créé.
- Quatre dossiers système existent : **Personnages**, **Joueurs**, **Scénarios**, **Notes**.
- Le MJ peut commencer à préparer son contenu.

### One-shot
- L'espace one-shot est créé, avec le scénario choisi ou un scénario vide.
- Si un scénario existant a été sélectionné, une instance est créée (→ UC-13).
- La vue session est accessible immédiatement.

## Données manipulées

### Espace de jeu (Campagne)

- Identifiant unique
- Nom
- Description (optionnel)
- Système de jeu (optionnel)
- Type : `CAMPAIGN` ou `ONE_SHOT`
- Statut : actif, brouillon, archivé
- Propriétaire

## Règles métier

- Un espace de jeu appartient à un seul propriétaire (`MemberRole.OWNER`).
- Le même utilisateur peut être MJ de plusieurs espaces et joueur dans d'autres.
- Le nom est obligatoire pour les campagnes. Pour les one-shots avec scénario existant,
  il est pré-rempli.
- Les dossiers système sont créés automatiquement et sont non-supprimables mais renommables.
- Un espace one-shot peut être archivé manuellement par le MJ, comme une campagne.
- Un espace peut être archivé sans être supprimé définitivement.

## Critères d'acceptation

- Un MJ peut créer une campagne depuis son tableau de bord avec juste un nom.
- Un MJ peut lancer un one-shot en moins de 30 secondes depuis le tableau de bord.
- Un one-shot lancé depuis un scénario existant redirige directement vers la vue session.
- Les quatre dossiers système existent et portent les noms neutres définis.
- Un espace créé en mode local (sans compte) est pleinement fonctionnel.

## Questions à valider en interview

- Les MJ font-ils une séparation mentale claire entre "campagne" et "one-shot" ?
- Sonia crée-t-elle une campagne pour chaque one-shot, ou pense-t-elle en termes de scénarios ?
- L'archivage automatique du one-shot est-il perçu comme une aide ou une perte de contrôle ?
- Le tableau de bord doit-il séparer "campagnes" et "one-shots", ou afficher tous les espaces ensemble ?

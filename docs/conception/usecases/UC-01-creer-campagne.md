# UC-01 — Créer et configurer une campagne

## Acteur principal

Utilisateur authentifié (futur MJ de la campagne)

## Acteurs secondaires

Aucun dans le scénario principal.

## Objectif

Créer un espace centralisé pour organiser une campagne de jeu de rôle.

## Contexte

Une campagne de jeu de rôle regroupe de nombreuses informations : scénarios, sessions, PNJ, personnages joueurs, notes, documents et éléments de lore. Sans espace centralisé, les informations sont dispersées sur plusieurs supports.

Tout utilisateur authentifié peut créer une campagne. En la créant, il en devient le MJ (`MemberRole.OWNER`) — ce rôle est contextuel à cette campagne. Le même utilisateur peut être joueur dans d'autres campagnes.

## Besoin utilisateur

L'utilisateur veut disposer d'un espace unique pour organiser une campagne sans mélanger ses informations avec d'autres campagnes ou d'autres projets.

## Déclencheur

L'utilisateur souhaite commencer une nouvelle campagne ou migrer une campagne existante dans l'outil.

## Préconditions

- L'utilisateur dispose d'un compte.
- L'utilisateur est authentifié.

## Scénario nominal

1. L'utilisateur accède à son tableau de bord.
2. Il clique sur l'action "Créer une campagne".
3. Le système affiche un formulaire de création.
4. L'utilisateur renseigne les informations principales :
   - nom de la campagne ;
   - description courte ;
   - système de jeu utilisé ;
   - statut initial de la campagne.

5. L'utilisateur valide la création.
6. Le système crée la campagne et enregistre l'utilisateur comme MJ (`MemberRole.OWNER`) via une `CampaignMembership`.
7. Le système redirige l'utilisateur vers le tableau de bord de la campagne.
8. Le tableau de bord affiche les sections principales : scénarios, sessions, PNJ, joueurs, notes et informations partagées.

## Scénarios alternatifs

### A1 — Création en mode brouillon

Le MJ commence à remplir le formulaire mais ne dispose pas encore de toutes les informations. Il peut sauvegarder la campagne comme brouillon.

### A2 — Campagne sans système de jeu défini

Le MJ ne choisit aucun système de jeu. Le système crée alors la campagne en mode générique.

### A3 — Annulation de la création

Le MJ quitte le formulaire sans valider. Aucune campagne n'est créée, sauf si un brouillon automatique est prévu.

## Exceptions

### E1 — Nom de campagne manquant

Le système empêche la validation et indique que le nom est obligatoire.

### E2 — Erreur de création

Si une erreur survient lors de la sauvegarde, le système affiche un message d'erreur et conserve les données saisies.

## Postconditions

- Une campagne est créée.
- L'utilisateur est enregistré comme MJ (`MemberRole.OWNER`) de cette campagne via une `CampaignMembership`.
- Quatre dossiers système sont créés automatiquement : **PNJ**, **Personnages joueurs**, **Scénarios**, **Notes**.
- Le MJ peut commencer à ajouter des scénarios, PNJ, notes et personnages.

## Données manipulées

### Campagne

- Identifiant unique
- Nom
- Description
- Système de jeu
- Statut
- Propriétaire
- Date de création

## Règles métier

- Tout utilisateur authentifié peut créer une campagne — aucune restriction de rôle global.
- En créant une campagne, l'utilisateur obtient le rôle `MemberRole.OWNER` dans cette campagne.
- Une campagne a exactement un `OWNER` — invariant garanti par l'agrégat `Campaign`.
- Un utilisateur peut être `OWNER` de plusieurs campagnes simultanément.
- Le nom de campagne est obligatoire.
- Le système de jeu est facultatif dans le MVP.
- Une campagne peut être archivée sans être supprimée définitivement.
- Les dossiers système (`isSystem = true`) sont créés automatiquement et non supprimables, mais renommables.

## Critères d'acceptation

- Tout utilisateur authentifié peut créer une campagne depuis son tableau de bord.
- Une campagne créée apparaît dans la liste des campagnes dont l'utilisateur est MJ.
- L'utilisateur accède à un tableau de bord dédié à la campagne après création.
- Une campagne ne peut pas être créée sans nom.

## Questions à valider en interview

- Comment les MJ organisent-ils leurs campagnes ?
- Font-ils une séparation claire entre campagne, scénario et session ?
- Ont-ils plusieurs campagnes en parallèle ?
- Le choix du système de jeu est-il important dès la création ?

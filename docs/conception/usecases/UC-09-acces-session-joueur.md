# UC-09 — Accéder à une session en tant que joueur (sans compte)

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre à un joueur d'accéder aux informations partagées par le MJ via un lien, sans créer de compte, avec une friction minimale.

## Contexte

Le MJ choisit Haversack — pas le joueur. Si rejoindre la session impose au joueur de créer un compte, de confirmer un email et de configurer un profil, une partie du groupe ne passera pas cette étape. Lucas (22 ans, étudiant) représente ce profil : il a déjà D&D Beyond, Discord et un carnet. Un outil de plus n'est pas bienvenu. Si le lien d'invitation mène à une page d'inscription, il ferme l'onglet.

L'accès joueur sans compte est la condition d'adoption du groupe entier, ce qui détermine la rétention du MJ.

## Déclencheur

Le MJ partage un lien d'accès à une session ou à sa campagne avec ses joueurs.

## Préconditions

- Le MJ a un compte (au minimum gratuit) — le partage nécessite un backend.
- Le MJ a généré un lien de session depuis UC-11 ou UC-08.

## Scénario nominal — Accès session ponctuel (sans compte)

1. Le MJ génère un lien de session depuis la vue session ou le panneau membres.
2. Il partage le lien (Discord, WhatsApp, email, peu importe).
3. Le joueur clique sur le lien.
4. L'application affiche immédiatement les informations partagées par le MJ pour cette session.
5. Le joueur saisit uniquement un **nom d'affichage** (pas d'email, pas de mot de passe).
6. Il peut consulter les documents partagés, les informations révélées en temps réel pendant la session.
7. À la fin de la session, son accès expire.

## Scénarios alternatifs

### A1 — Joueur avec un compte existant

1. Le joueur clique sur le lien.
2. Il est déjà connecté à son compte Haversack.
3. Il accède directement aux informations partagées, avec son historique de session.

### A2 — Joueur qui veut créer un compte depuis l'accès invité

1. Le joueur a accédé comme invité à plusieurs sessions.
2. Il veut conserver ses notes personnelles entre sessions.
3. Il crée un compte depuis la page invité.
4. Ses notes et accès existants sont migrés vers son compte.
5. Il devient membre permanent de la campagne si le MJ valide.

### A3 — Lien de campagne permanent (membres réguliers)

1. Le MJ invite un joueur régulier avec un lien de campagne (et non un lien de session).
2. Le joueur peut accéder à l'historique des sessions passées et aux documents de lore partagés.
3. Ce mode nécessite un compte joueur (accès persistant → UC-10).

### A4 — Lien expiré ou révoqué

1. Le joueur clique sur un lien de session expiré ou que le MJ a révoqué.
2. L'application affiche un message clair : "Ce lien n'est plus actif."
3. Elle invite le joueur à contacter le MJ pour un nouveau lien.

## Exceptions

### E1 — Lien invalide

Le lien est mal formé ou ne correspond à aucune session. L'application affiche une page d'erreur sobre, sans révéler si la campagne existe.

## Postconditions

- Le joueur accède aux informations partagées sans avoir créé de compte.
- Son nom d'affichage est visible dans la session pour le MJ.
- Son accès expire à la fin de la session (pour les liens ponctuels).

## Règles métier

- Un lien de session ponctuel est valable le temps de la session + une fenêtre de grâce (ex. : 24 h).
- Un lien de campagne permanent est valable jusqu'à révocation par le MJ.
- Le joueur invité (sans compte) a les mêmes droits fonctionnels qu'un joueur authentifié
  dans le périmètre de son lien : il peut voir les contenus publics ou partagés avec lui,
  consulter sa fiche si un personnage lui est associé, et créer des notes personnelles.
  La seule différence est technique : il n'a pas de compte persistant.
- La création d'un compte depuis l'accès invité migre l'accès sans perdre les notes déjà prises.
- Le MJ avec un compte gratuit peut inviter jusqu'à 4 joueurs par session. Le compte Pro lève cette limite.

## Critères d'acceptation

- Un joueur peut rejoindre une session en cliquant sur un lien, en saisissant uniquement un nom, sans inscription.
- Le joueur voit en temps réel les informations que le MJ partage pendant la session.
- Le lien expire correctement après la session.
- Le joueur peut créer un compte depuis la page invité sans perdre ses données de session.
- Un lien révoqué affiche un message clair sans révéler d'information sur la campagne.

## Questions à valider en interview

- Le nom d'affichage seul est-il suffisant, ou faut-il un identifiant léger (pseudo ou code) pour éviter les collisions ?
- Quelle est la durée de grâce acceptable après la fin de session pour un lien ponctuel ?
- Les joueurs veulent-ils recevoir un récapitulatif post-session sans avoir de compte ?

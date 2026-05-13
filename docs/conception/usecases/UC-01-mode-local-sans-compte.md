# UC-01 — Utiliser l'application sans compte (mode local MJ)

## Acteur principal

MJ

## Acteurs secondaires

Aucun.

## Objectif

Permettre à un MJ de commencer à utiliser Haversack immédiatement, sans créer de compte, en stockant les données localement dans le navigateur.

## Contexte

L'obligation de créer un compte avant de pouvoir utiliser l'application est une friction qui filtre les utilisateurs avant même qu'ils aient vu la valeur du produit. Le mode local supprime cette friction : le MJ ouvre l'app, crée une campagne, prépare son contenu et pilote une session — sans email, sans mot de passe.

Ce mode cible particulièrement les profils qui veulent évaluer l'outil avant de s'engager (Nadia, Rémi), qui ne veulent pas multiplier les comptes (Thomas), ou qui ont été déçus par des outils trop lourds à la configuration (Nadia abandonnant Notion).

Le mode local est aussi la base du modèle de monétisation non-agressif : la valeur est perçue d'abord, l'engagement vient ensuite.

## Modèle d'accès et de monétisation

### Mode local (sans compte)

- Accès immédiat, aucun formulaire.
- Données stockées dans le navigateur (IndexedDB).
- Fonctionnalités disponibles : création de campagne, documents, dossiers, vue session, création à la volée, recherche locale.
- Fonctionnalités indisponibles : partage avec les joueurs, synchronisation cloud, accès multi-device.
- Limite de stockage : capacité du navigateur (~50–100 Mo en pratique).
- **Risque communiqué clairement** : les données sont liées au navigateur. Vider le cache ou changer de navigateur les efface. L'application doit afficher un bandeau de rappel non intrusif.

### Compte gratuit (après inscription)

- Synchronisation cloud de **3 campagnes maximum**.
- Partage avec les joueurs (jusqu'à **4 joueurs par session**).
- Stockage cloud : **500 Mo**.
- Accès multi-device.
- Migration automatique des données locales lors de la création du compte.

### Compte Pro (abonnement payant)

- Campagnes en cloud **illimitées**.
- Joueurs **illimités** par session.
- Stockage cloud : **5 Go+**.
- Fonctionnalités avancées futures (templates communautaires, historique enrichi, etc.).
- Tarif cible : ~7 €/mois ou ~60 €/an.

> Ce modèle est inspiré d'Obsidian (local gratuit, sync payant) : la valeur est réelle avant le paiement, l'upgrade est une décision rationnelle, pas une contrainte imposée.

## Déclencheur

Le MJ accède à l'application pour la première fois sans être authentifié.

## Préconditions

Aucune.

## Scénario nominal

1. Le MJ ouvre l'application.
2. L'application propose deux options :
   - **"Commencer sans compte"** (mode local)
   - **"Créer un compte"** ou **"Se connecter"**
3. Le MJ choisit "Commencer sans compte".
4. L'application affiche un message court expliquant que les données seront stockées localement dans ce navigateur, avec un lien vers la FAQ.
5. Le MJ est redirigé vers l'écran de création de campagne.
6. Il utilise l'application normalement (UC-02 à UC-06, UC-14, UC-12).

## Scénarios alternatifs

### A1 — Conversion vers un compte

1. Le MJ tente une action nécessitant un compte (partage joueurs, accès depuis un autre device).
2. L'application affiche une invite contextuelle : *"Cette fonctionnalité nécessite un compte. Vos données locales seront migrées automatiquement."*
3. Le MJ crée un compte (UC-10).
4. Les données locales (campagnes, documents, dossiers) sont uploadées vers le cloud.
5. Le mode local est désactivé pour cet utilisateur — il bascule en compte gratuit.

### A2 — Retour après fermeture du navigateur

1. Le MJ ferme le navigateur puis revient sur l'application.
2. L'application récupère les données depuis IndexedDB.
3. Le MJ retrouve ses campagnes et documents intacts.
4. Le bandeau de rappel "données locales" est affiché si le MJ n'a pas de compte.

### A3 — Données introuvables (cache vidé)

1. Le MJ revient mais le cache a été vidé.
2. L'application détecte l'absence de données locales et affiche un message explicatif.
3. Elle propose de créer une nouvelle campagne ou de se connecter à un compte existant.

### A4 — Export manuel des données locales

1. Le MJ (sans compte) souhaite sauvegarder ses données.
2. Depuis les paramètres, il exporte ses campagnes en JSON.
3. Il peut les réimporter ultérieurement (même navigateur, autre navigateur, ou lors d'une migration vers un compte).

## Exceptions

### E1 — Stockage navigateur plein

Le navigateur refuse l'écriture dans IndexedDB. L'application affiche un message et propose de créer un compte pour migrer les données vers le cloud.

## Postconditions

- Le MJ peut utiliser toutes les fonctionnalités de préparation et de session sans compte.
- Les données sont persistantes entre les sessions navigateur (jusqu'à vidage du cache).
- L'invitation à créer un compte est présente mais non intrusive.

## Règles métier

- En mode local, aucune donnée n'est envoyée au serveur.
- La création de compte depuis le mode local déclenche obligatoirement une migration des données locales.
- Le bandeau "données locales" est affiché à chaque visite tant que le MJ n'a pas de compte, sans bloquer l'usage.
- Les fonctionnalités de partage (UC-08) et d'accès joueur (UC-09) nécessitent au minimum un compte gratuit.

## Critères d'acceptation

- Un MJ peut créer une campagne et préparer du contenu sans créer de compte.
- Les données persistent après fermeture et réouverture du navigateur.
- L'application affiche un rappel non bloquant sur la nature locale des données.
- Le MJ peut créer un compte depuis n'importe quelle page de l'app, avec migration automatique.
- L'export JSON des données locales fonctionne et produit un fichier réimportable.
- Les fonctionnalités de partage sont visibles mais désactivées avec une invite claire en mode local.

## Questions à valider en interview

- Est-ce que la contrainte "données liées au navigateur" est acceptable pour un MJ qui veut évaluer l'outil ?
- Le message de risque sur la perte de données est-il suffisant ou bloquant ?
- À quel moment le MJ est-il prêt à créer un compte (après quelle action ou quel délai) ?
- L'export JSON est-il suffisant comme filet de sécurité, ou faut-il un export plus lisible (Markdown, PDF) ?

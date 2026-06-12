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
- Données stockées dans le stockage local du navigateur.
- Fonctionnalités disponibles : création de campagne, documents, dossiers, vue session, création à la volée, recherche locale.
- Fonctionnalités indisponibles : partage avec les joueurs, synchronisation cloud, accès multi-device, participation de joueurs. En mode local, il n'existe ni vue joueur ni accès invité — aucun joueur ne peut rejoindre une session ni y prendre de notes ; la vue session sert au MJ seul.
- Limite de stockage : capacité du navigateur (~50–100 Mo en pratique).
- **Risques communiqués clairement** (deux bandeaux distincts) :
  - **Bandeau de durabilité** : le navigateur n'a pas garanti la conservation permanente des données. Sous pression mémoire, elles peuvent être supprimées. L'application affiche un bandeau non bloquant invitant à créer un compte pour sécuriser les données.
  - **Bandeau de confidentialité** : les données ne sont pas chiffrées au repos. Sur un poste partagé, toute personne ayant accès à ce navigateur sur ce poste peut les lire. L'application affiche un bandeau distinct invitant à créer un compte.

### Compte gratuit (après inscription)

- Synchronisation cloud de **3 campagnes maximum**.
- Partage avec les joueurs (jusqu'à **4 joueurs par session**).
- Stockage cloud : **500 Mo**.
- Accès multi-device.
- Migration des données locales — avec gate de reconnaissance (confirmation explicite) — lors de la création du compte.

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
4. L'application affiche un message court expliquant que les données seront stockées dans le stockage local du navigateur, avec un lien vers la FAQ.
5. Le MJ est redirigé vers l'écran de création de campagne.
6. Il utilise l'application normalement (UC-02 à UC-06, UC-14).

## Scénarios alternatifs

### A1 — Conversion vers un compte

1. Le MJ tente une action nécessitant un compte (partage joueurs, accès depuis un autre device).
2. L'application affiche une invite contextuelle : *"Cette fonctionnalité nécessite un compte. Vos données locales pourront être migrées vers le cloud."*
3. Le MJ crée un compte (UC-10).
4. L'application présente les données locales détectées — titres des campagnes, historique de session (sessions terminées, notes de session, documents épinglés, résumés), volume estimé, date de création — et demande une confirmation explicite avant la migration (gate de reconnaissance). Les données sont migrées campagne par campagne après confirmation. Une session en cours (statut `LIVE`) doit être clôturée avant migration et le gate de reconnaissance le signale. En cas de rejet d'une campagne, un rapport détaille la raison du refus et les données locales correspondantes restent intactes.
5. Le mode local est désactivé pour cet utilisateur — il bascule en compte gratuit.

### A2 — Retour après fermeture du navigateur

1. Le MJ ferme le navigateur puis revient sur l'application.
2. L'application récupère les données depuis le stockage local du navigateur.
3. Le MJ retrouve ses campagnes et documents intacts.

### A3 — Données introuvables (cache vidé ou supprimées par le navigateur)

1. Le MJ revient mais les données locales ont disparu : le cache a été vidé manuellement ou le navigateur les a supprimées faute de garantie de conservation permanente.
2. L'application détecte l'absence de données locales et affiche un message explicatif distinguant ce cas d'une première visite.
3. Elle propose de créer une nouvelle campagne ou de se connecter à un compte existant.

### A4 — Export de campagne et réimport manuel d'un fichier de sauvegarde

#### A4a — Export de campagne (Should Have — disponible en mode local comme avec un compte)

1. Le MJ (sans compte) souhaite sauvegarder ses données ou récupérer ses campagnes.
2. Depuis les paramètres, il exporte sa campagne dans un fichier de sauvegarde au format ouvert.
3. Le fichier est téléchargé sur son poste. Il en est propriétaire.

#### A4b — Réimport d'un fichier de sauvegarde (post-MVP)

> **Post-MVP.** Le filet de sécurité du mode local repose sur la persistance navigateur, l'export et la migration vers un compte (NFR-OFF-02/03) — il ne dépend pas du réimport. Ce scénario est conservé pour tracer le raisonnement et les règles de validation à reprendre lors de l'implémentation ultérieure.

1. Le MJ souhaite réimporter un fichier de sauvegarde (même navigateur, autre navigateur, ou lors d'une migration vers un compte).
2. Lors du réimport, le fichier de sauvegarde est soumis avant toute écriture dans le stockage local à deux vérifications dans cet ordre :
   - **Validation structurelle** : le fichier est vérifié (version reconnue, format des champs, types attendus). Un fichier malformé ou d'une version non reconnue est rejeté avec un message d'erreur.
   - **Nettoyage de contenu** : le contenu est nettoyé de tout élément susceptible de déclencher l'exécution de code avant d'être enregistré. Un fichier importé n'est jamais enregistré tel quel dans le stockage local.

## Exceptions

### E1 — Stockage navigateur plein

Le navigateur refuse l'enregistrement de nouvelles données. L'application affiche un message et propose de créer un compte pour migrer les données vers le cloud. Ce cas est distinct du bandeau de durabilité : E1 signale un refus d'enregistrement immédiat (capacité pleine), tandis que le bandeau de durabilité signale un risque de suppression future (faute de garantie de conservation permanente).

## Postconditions

- Le MJ peut utiliser toutes les fonctionnalités de préparation et de session sans compte.
- Les données sont persistantes entre les sessions navigateur (jusqu'à vidage du cache).
- L'invitation à créer un compte est présente mais non intrusive.

## Règles métier

- En mode local, aucune donnée n'est envoyée au serveur.
- En mode local, la session est mono-utilisateur MJ : aucun joueur ne peut y accéder ni y prendre de notes. La vue session sert au MJ seul. L'accès joueur (vue joueur, notes de session de joueur, accès invité) présuppose un compte MJ.
- La création de compte depuis le mode local déclenche une migration des données locales après confirmation explicite. L'application présente les campagnes détectées (titre, historique de session, volume estimé, date de création) et exige une confirmation explicite avant de débuter. La migration ne commence qu'après cette confirmation. Les campagnes sont importées une à une ; en cas de rejet d'une campagne (contenu invalide, intitulé en conflit, type ou format non reconnu), un rapport détaille la raison du refus pour chaque campagne et les données locales correspondantes restent intactes dans le navigateur ; les campagnes acceptées sont migrées et accessibles. Une session en cours (statut `LIVE`) doit être clôturée avant migration.
- Les fonctionnalités de partage (UC-08) et d'accès joueur (UC-09) nécessitent au minimum un compte gratuit.
- L'application affiche deux bandeaux distincts et non bloquants en mode local :
  - **Bandeau de durabilité** : affiché si le navigateur n'a pas garanti la conservation permanente des données — risque qu'elles soient supprimées sous pression mémoire.
  - **Bandeau de confidentialité** : affiché systématiquement en mode local — risque qu'une personne ayant accès à ce navigateur sur ce poste puisse lire les données. Les données ne sont pas chiffrées au repos, ce qui est une limitation du MVP assumée.
- Tout fichier de sauvegarde réimporté est validé en structure (version reconnue, format des champs, types attendus) et nettoyé de tout élément susceptible de déclencher l'exécution de code avant d'être enregistré dans le stockage local.
- Aucun contenu, importé ou saisi, ne peut déclencher l'exécution de code lors de son affichage.

## Critères d'acceptation

- Un MJ peut créer une campagne et préparer du contenu sans créer de compte.
- Les données persistent après fermeture et réouverture du navigateur.
- Le MJ peut créer un compte depuis n'importe quelle page de l'app ; si des données locales existent, la migration est proposée avec confirmation explicite.
- Le MJ peut exporter sa campagne dans un format ouvert depuis les paramètres (Should Have — disponible en mode local comme avec un compte).
- Le réimport d'un fichier de sauvegarde (post-MVP) : validation de structure (version reconnue, formats, types) puis nettoyage avant enregistrement dans le stockage local.
- Les fonctionnalités de partage sont visibles mais désactivées avec une invite claire en mode local.
- Si le navigateur n'a pas garanti la conservation permanente des données, le bandeau de durabilité est affiché (non bloquant).
- Le bandeau de confidentialité est affiché en mode local (risque d'accès par une personne sur un poste partagé).
- Un fichier de sauvegarde réimporté est rejeté ou nettoyé avant enregistrement ; un fichier malformé ou d'une version non reconnue produit un message d'erreur.

## Décisions liées

- [ADR-016](../../architecture/decisions/ADR-016-serialisation-locale-migration.md) — trace du raisonnement sur la sérialisation locale, le contrat de migration local→cloud et la mécanique du gate de reconnaissance.
- [ADR-017](../../architecture/decisions/ADR-017-modele-indexeddb-local.md) — trace du raisonnement sur le modèle de stockage local et la sécurité du mode local (bandeaux durabilité/confidentialité, vérification et nettoyage des fichiers réimportés, garantie de non-exécution de code).

## Questions à valider en interview

- Est-ce que la contrainte "données liées au navigateur" est acceptable pour un MJ qui veut évaluer l'outil ?
- Le message de risque sur la perte de données est-il suffisant ou bloquant ?
- À quel moment le MJ est-il prêt à créer un compte (après quelle action ou quel délai) ?
- Le format ouvert retenu pour l'export de campagne est-il suffisant comme filet de sécurité, ou faut-il un format plus lisible (document texte, PDF) ?

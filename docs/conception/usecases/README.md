# Use Cases — Index

> Contexte produit et positionnement → [vision-produit.md](../vision/vision-produit.md)
> Priorisation détaillée → [moscow.md](../vision/moscow.md)

---

## Deux piliers produit

- **Pilier 1 — Document générique** : le MJ crée, organise et enrichit son contenu librement, sans structure imposée par l'application.
- **Pilier 2 — Vue session** : le MJ pilote sa partie en temps réel depuis une interface qui expose le contenu préparé et permet la création à la volée.

---

## Must Have — UC-01 à UC-09

| ID | Use Case | Acteur | Pilier | Résumé |
|---|---|---|---|---|
| [UC-01](UC-01-mode-local-sans-compte.md) | Mode local sans compte | MJ | — | Le MJ ouvre l'app et commence à préparer sans s'inscrire. Les données sont stockées localement dans le navigateur, avec un chemin naturel vers le cloud. |
| [UC-02](UC-02-creer-campagne.md) | Créer et configurer une campagne | MJ | 1 | Le MJ crée un espace de travail nommé — conteneur de tout le contenu, configuré en moins de deux minutes, qu'il s'agisse d'une campagne longue ou d'un one-shot. |
| [UC-03](UC-03-structurer-scenario.md) | Structurer un scénario | MJ | 1 | Le MJ prépare son scénario en blocs libres ou en scènes structurées, de cinq bullet points (Émilie) à une architecture détaillée (Antoine), sans forme imposée. |
| [UC-04](UC-04-gerer-notes-mj.md) | Gérer les documents et notes MJ | MJ | 1 + 2 | Le MJ capture rapidement des informations à tout moment — documents libres, notes rapides, lore ou éléments typés — sans contrainte de structure ni délai. |
| [UC-05](UC-05-organiser-dossiers.md) | Organiser le contenu en dossiers | MJ | 1 | Le MJ structure sa campagne en dossiers librement nommés et associe un type de document optionnel (PNJ, Lieu, Objet…) à chaque dossier pour enrichir les entrées sans les contraindre. |
| [UC-06](UC-06-vue-session.md) | Utiliser la vue session | MJ | 2 | Interface de pilotage en temps réel : le MJ accède aux scènes, PNJ et notes préparés, épingle le contenu clé et prend des notes rapides sans jamais quitter le contexte de la partie. |
| [UC-07](UC-07-creation-volee-session.md) | Créer un élément à la volée | MJ | 2 | En quelques secondes, le MJ crée un PNJ, un lieu ou une note depuis la vue session pour répondre aux imprévus des joueurs sans casser le rythme. |
| [UC-08](UC-08-partager-information.md) | Partager une information | MJ | 2 | Depuis la vue session, le MJ révèle un document ou une note — les joueurs le voient instantanément sur leur écran via un lien, sans avoir de compte. |
| [UC-09](UC-09-acces-session-joueur.md) | Accès joueur sans compte | Joueur | 2 | Un joueur clique sur un lien de session, saisit un nom d'affichage, et voit en temps réel les informations partagées par le MJ — sans créer de compte. |

---

## Should Have — UC-10 à UC-14

| ID | Use Case | Acteur | Pilier | Résumé |
|---|---|---|---|---|
| [UC-10](UC-10-compte-cloud.md) | Créer un compte et synchroniser | MJ / Joueur | — | Le MJ crée un compte pour sauvegarder ses campagnes dans le cloud, activer le partage joueurs et accéder depuis n'importe quel appareil. Ses données locales migrent automatiquement. |
| [UC-11](UC-11-gerer-membres-campagne.md) | Gérer les membres d'une campagne | MJ | — | Le MJ invite des membres permanents à sa campagne et génère des liens de session temporaires pour les groupes ponctuels (one-shots, conventions). |
| [UC-12](UC-12-rejoindre-campagne.md) | Rejoindre une campagne | Joueur | — | Un joueur avec un compte rejoint une campagne de façon durable et accède à l'historique des sessions partagées et aux documents que le MJ rend visibles. |
| [UC-13](UC-13-scenario-reutilisable.md) | Scénario réutilisable | MJ | 1 | Le MJ marque un scénario comme réutilisable et crée des instances indépendantes pour le rejouer avec des groupes différents — le scénario source reste intact entre les runs. |
| [UC-14](UC-14-recherche.md) | Rechercher et filtrer | MJ | 1 + 2 | Le MJ retrouve n'importe quel document de la campagne par mot-clé ou par tag depuis n'importe quelle vue, y compris en pleine session. |

---

## Vision long terme

- [Use Cases hors MVP](UC-HORS-MVP.md) — fonctionnalités exclues, deprioritisées, et vision produit

# Use Cases — Index

> Contexte produit et positionnement → [vision-produit.md](../vision/vision-produit.md)
> Priorisation détaillée → [moscow.md](../vision/moscow.md)

---

## Deux piliers produit

- **Pilier 1 — Document générique** : le MJ crée, organise et enrichit son contenu librement, sans structure imposée par l'application.
- **Pilier 2 — Vue session** : le MJ pilote sa partie en temps réel depuis une interface qui expose le contenu préparé et permet la création à la volée.

---

## Must Have — UC-01 à UC-10

| ID | Use Case | Acteur | Pilier | Résumé |
|---|---|---|---|---|
| [UC-01](UC-01-mode-local-sans-compte.md) | Mode local sans compte | MJ | — | Le MJ ouvre l'app et commence à préparer sans s'inscrire. Les données sont stockées localement dans le navigateur, avec un chemin naturel vers le cloud. |
| [UC-02](UC-02-creer-espace-jeu.md) | Créer un espace de jeu | MJ | 1 | Le MJ crée un espace de travail nommé — conteneur de tout le contenu, configuré en moins de deux minutes, qu'il s'agisse d'une campagne longue ou d'un one-shot. |
| [UC-03](UC-03-structurer-scenario.md) | Structurer un scénario | MJ | 1 | Le MJ prépare son scénario en blocs libres ou en scènes structurées, de cinq bullet points (Émilie) à une architecture détaillée (Antoine), sans forme imposée. |
| [UC-04](UC-04-gerer-documents-campagne.md) | Gérer les documents d'une campagne | MJ | 1 + 2 | Le MJ capture rapidement des informations à tout moment — documents libres, notes rapides, lore ou éléments typés — sans contrainte de structure ni délai. |
| [UC-05](UC-05-organiser-dossiers.md) | Organiser par dossiers *(base — dossiers libres)* | MJ | 1 | Le MJ structure sa campagne en dossiers librement nommés. La couche « riche » (types de document élaborés) est Should Have — voir ci-dessous. |
| [UC-06](UC-06-vue-session.md) | Utiliser la vue session | MJ | 2 | Interface de pilotage en temps réel : le MJ accède aux scènes, PNJ et notes préparés, épingle le contenu clé et prend des notes rapides sans jamais quitter le contexte de la partie. |
| [UC-07](UC-07-creation-volee-session.md) | Créer un élément à la volée | MJ | 2 | En quelques secondes, le MJ crée un PNJ, un lieu ou une note depuis la vue session pour répondre aux imprévus des joueurs sans casser le rythme. |
| [UC-08](UC-08-partager-information.md) | Partager une information aux joueurs | MJ | 2 | Depuis la vue session, le MJ révèle un document ou une note — les joueurs le voient instantanément sur leur écran via un lien, sans avoir de compte. |
| [UC-09](UC-09-acces-session-joueur.md) | Accès joueur sans compte | Joueur | 2 | Un joueur clique sur un lien de session, saisit un nom d'affichage, et voit en temps réel les informations partagées par le MJ — sans créer de compte. |
| [UC-10](UC-10-compte-cloud.md) | Créer un compte et synchroniser dans le cloud | MJ / Joueur | — | Le MJ crée un compte pour activer le partage joueurs (UC-08) et sauvegarder dans le cloud. Ses données locales migrent automatiquement. *(Must Have — raisonnement de priorisation dans la section MoSCoW.)* |

---

## Should Have — UC-05 riche + UC-11 à UC-14 *(UC-13 hors première livraison)*

| ID | Use Case | Acteur | Pilier | Résumé |
|---|---|---|---|---|
| [UC-05](UC-05-organiser-dossiers.md) | Organiser par dossiers *(riche — types élaborés)* | MJ | 1 | Le MJ associe un type de document optionnel (PNJ, Lieu, Objet…) à un dossier ou un document pour enrichir ses entrées sans contraindre la structure. *(La base UC-05 est Must Have.)* |
| [UC-11](UC-11-gerer-membres-campagne.md) | Gérer les membres d'une campagne | MJ | — | Le MJ invite des membres permanents à sa campagne et génère des liens de session temporaires pour les groupes ponctuels (one-shots, conventions). |
| [UC-12](UC-12-rejoindre-campagne.md) | Consulter sa campagne en tant que joueur (vue post-accès) | Joueur | — | Après avoir obtenu l'accès (via UC-09 ou UC-11), le joueur consulte sa campagne : fiche, documents `PUBLIC`, périmètre SESSION/CAMPAIGN. |
| [UC-13](UC-13-scenario-reutilisable.md) | Scénario réutilisable *(Should Have — hors première livraison)* | MJ | 1 | Le MJ marque un scénario comme réutilisable et crée des instances indépendantes pour le rejouer avec des groupes différents — le scénario source reste intact entre les runs. |
| [UC-14](UC-14-recherche.md) | Rechercher et filtrer | MJ | 1 + 2 | Le MJ retrouve n'importe quel document de la campagne par mot-clé ou par tag depuis n'importe quelle vue, y compris en pleine session. |

---

## Post-MVP (spécifiés) — UC-15

| ID | Use Case | Résumé |
|---|---|---|
| [UC-15](UC-15-gel-campagnes-downgrade-tier.md) | Gel de campagnes au downgrade de tier | Espaces excédentaires gelés automatiquement (lecture seule) au downgrade ; dégel automatique au ré-upgrade, sans perte de données. |

---

## Vision long terme

- [Use Cases hors MVP](UC-HORS-MVP.md) — fonctionnalités exclues, deprioritisées, et vision produit

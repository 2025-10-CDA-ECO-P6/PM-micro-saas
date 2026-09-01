# NFR-OFF-05 — Continuité d'édition en préparation cloud lors d'une perte de réseau

Famille **Hors connexion** — [Index de conception](../../README.md)

---

## Énoncé normatif

Lorsqu'un MJ utilisant un compte perd sa connexion pendant la préparation — édition d'un document ou d'un scénario, hors session — il ne perd pas son travail en cours. Les modifications sont conservées et synchronisées au retour de la connexion, sans action de sa part.

---

## Raison d'être

La douleur adressée ici n'est pas sociale mais celle du travail perdu. La préparation est un investissement long et solitaire : un MJ peut passer plusieurs heures à structurer un scénario ou enrichir une fiche, en dehors de toute table de jeu et de tout regard extérieur. Perdre une saisie sur une coupure réseau passagère est un préjudice direct sur ce temps investi seul, distinct de la rupture de rythme collectif que NFR-OFF-04 adresse en session active.

Le persona Thomas (persona-01) illustre cet investissement : il a construit son organisation documentaire sur plusieurs années et la connaît par cœur, ce qui présuppose un temps de préparation conséquent qu'une perte de contenu rendrait d'autant plus coûteuse à refaire. Le persona Antoine (persona-05), qui mène trois campagnes en parallèle avec des sessions fréquentes, prépare dans des fenêtres de temps contraintes entre ses différentes tables ; une perte de saisie sur l'une de ces fenêtres n'est pas rattrapable au même moment que la suivante.

UC-03 documente déjà ce risque à la marge, dans son exception E2 (perte de connexion ou erreur de sauvegarde) : « le système conserve les données saisies localement si possible et affiche un message d'erreur ». Cette exigence formalise et généralise ce comportement à l'échelle du modèle documentaire porté par UC-04, au-delà du seul cas du scénario.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'édition de documents et de scénarios en préparation (hors session), en mode cloud, sur un seul appareil, lors d'une interruption réseau passagère.
- La conservation locale du contenu en cours d'édition pendant l'interruption.
- La synchronisation automatique des modifications en attente au retour de la connexion, sans intervention du MJ.
- L'information non bloquante sur l'état de synchronisation pendant l'interruption et à la reconnexion.

**Ce que cette exigence ne couvre pas :**

- Le chargement de contenu non encore consulté avant l'interruption : ce contenu nécessite une connexion pour être récupéré.
- La synchronisation entre plusieurs appareils hors connexion : explicitement exclue du MVP (vision §2.4).
- Le mode local sans compte : en mode local, les données ne sont jamais synchronisées vers un cloud — il n'y a pas de synchronisation à conserver en attente. NFR-OFF-01, NFR-OFF-02 et NFR-OFF-03 couvrent la durabilité locale pour ce mode.
- La session active : ce cas reste couvert par NFR-OFF-04, dont la raison d'être — le coût social et immédiat de l'interruption collective — ne s'applique pas à la préparation solitaire et asynchrone couverte ici. Réciproquement, NFR-OFF-04 ne couvre pas la préparation hors session : c'est précisément le manque que la présente exigence referme.

**Périmètre MVP assumé :** cette exigence couvre uniquement le cas d'une interruption passagère sur un seul appareil, en préparation (hors session), avec un compte existant.

---

## Critères d'acceptation produit

**Situation nominale — perte de connexion pendant l'édition :**

Un MJ qui perd sa connexion en cours d'édition d'un document ou d'un scénario voit son contenu conservé dans l'interface. Une notification non bloquante indique que la synchronisation est en attente. Le MJ peut continuer à saisir et à modifier le contenu en cours.

**Situation nominale — reconnexion automatique :**

Dès que la connexion est rétablie, les modifications conservées en attente sont synchronisées sans que le MJ ait à effectuer une action. La notification de synchronisation disparaît ou se met à jour pour indiquer que la synchronisation est terminée. Aucun contenu n'est perdu.

**Situation limite — interruption longue :**

Un MJ dont la connexion est interrompue pendant une durée prolongée continue à pouvoir éditer le document ou le scénario en cours. À la reconnexion, toutes les modifications sont synchronisées dans l'ordre de leur saisie.

**Ce que le MJ ne doit jamais constater :**

Le MJ ne perd aucune modification saisie pendant une interruption réseau passagère en préparation. À la reconnexion, l'état qu'il voit dans l'interface correspond à l'état synchronisé.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-03 — Structurer un scénario](../usecases/UC-03-structurer-scenario.md) | Exception E2 (perte de connexion ou erreur de sauvegarde) : le système conserve les données saisies localement si possible et affiche un message d'erreur. Cette exigence généralise ce comportement. |
| [UC-04 — Gérer les documents d'un espace](../usecases/UC-04-gerer-documents-campagne.md) | Porte le modèle documentaire général d'édition dont cette exigence couvre la continuité en cas de perte de réseau. Traçabilité montante à l'UC ; aucune règle métier dédiée à ce cas n'est portée par les user stories associées (US-UC-03, US-UC-04) à ce jour. |

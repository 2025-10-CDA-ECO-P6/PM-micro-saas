# NFR-OFF-04 — Fonctionnement partiel en cas de perte de réseau passagère en mode cloud

Famille **Hors connexion** — [Index de conception](../README.md)

---

## Énoncé normatif

Lorsqu'un MJ utilisant un compte perd sa connexion en cours de session, il peut continuer à saisir des notes et à consulter les documents déjà chargés. L'application conserve les modifications en attente et les synchronise dès que la connexion est rétablie, sans demander d'action supplémentaire au MJ.

---

## Raison d'être

Une session de jeu de rôle est un moment collectif, en temps réel, où l'interruption a un coût social immédiat : les joueurs attendent, le rythme se brise, la tension narrative retombe. La douleur prioritaire identifiée par la vision (§1.1) est précisément « le rythme de jeu est cassé ». Une perte de réseau passagère — réseau instable, coupure Wi-Fi momentanée, transition entre zones de couverture en déplacement — ne doit pas déclencher une interruption de la session.

Ce scénario est documenté directement dans UC-06 (Exception E1 : perte de réseau en session LIVE) et dans US-06-04 (RB-06-14 : résilience réseau). Ce n'est pas un cas théorique : les tables de jeu se tiennent dans des appartements avec des connexions domestiques variables, dans des cafés, des bibliothèques, des salles associatives. La fiabilité du réseau n'est pas garantie en dehors d'un environnement de bureau contrôlé.

La vue session est l'interface centrale pendant la partie (UC-06 contexte). Sa promesse est de réduire le délai entre « le MJ cherche une information » et « le MJ la trouve ». Si une coupure réseau de deux minutes efface une note de session ou bloque la saisie, cette promesse n'est pas tenue.

Les personas qui bénéficient de cette exigence sont principalement les MJ avec compte — Thomas, Émilie — qui ont fait le choix du cloud pour accéder aux fonctionnalités de partage. Ils ont accepté une dépendance réseau pour certaines fonctionnalités ; ils ne doivent pas, en contrepartie, voir leurs notes de session perdues à la moindre instabilité.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La saisie de notes de session pendant une interruption réseau, avec conservation locale du contenu en attente de synchronisation.
- La consultation des documents déjà chargés dans la vue session avant l'interruption.
- La synchronisation automatique des modifications en attente au retour de la connexion, sans intervention du MJ.
- L'information non bloquante indiquant au MJ l'état de synchronisation pendant l'interruption et à la reconnexion.

**Ce que cette exigence ne couvre pas :**

- Le chargement de nouveaux documents non encore consultés pendant l'interruption : ces documents nécessitent une connexion pour être récupérés.
- Le partage en direct avec les joueurs pendant l'interruption : le partage (UC-08) et l'accès joueur (UC-09) nécessitent une connexion active.
- La synchronisation entre plusieurs appareils hors connexion : explicitement exclue du MVP. La vision §2.4 identifie cette capacité comme une complexité disproportionnée au regard du périmètre initial.
- Le mode local sans compte : en mode local, les données ne sont jamais synchronisées vers un cloud — il n'y a pas de synchronisation à conserver en attente. NFR-OFF-02 et NFR-OFF-03 couvrent la durabilité locale pour ce mode.

**Périmètre MVP assumé :** la synchronisation hors connexion entre plusieurs appareils est exclue. Cette exigence couvre uniquement le cas d'une interruption passagère sur un seul appareil, en session active, avec un compte existant.

---

## Critères d'acceptation produit

**Situation nominale — perte de connexion pendant la saisie :**

Un MJ qui perd sa connexion en cours de saisie d'une note de session voit son contenu conservé dans l'interface. Une notification non bloquante indique que la synchronisation est en attente. Le MJ peut continuer à saisir de nouvelles notes, à consulter les documents déjà chargés et à naviguer dans les panneaux de la vue session.

**Situation nominale — reconnexion automatique :**

Dès que la connexion est rétablie, les notes conservées en attente sont synchronisées sans que le MJ ait à effectuer une action. La notification de synchronisation disparaît ou se met à jour pour indiquer que la synchronisation est terminée. Aucun contenu n'est perdu.

**Situation limite — interruption longue :**

Un MJ dont la connexion est interrompue pendant une durée prolongée (supérieure à quelques secondes) continue à disposer des documents déjà chargés dans sa vue session. Il peut prendre des notes. À la reconnexion, toutes les modifications sont synchronisées dans l'ordre de leur saisie.

**Situation limite — notification non bloquante :**

La notification de synchronisation en attente ne couvre pas la vue session, ne suspend pas l'interface et ne demande pas d'action au MJ. Elle est visible mais ne perturbe pas le pilotage de la session.

**Ce que le MJ ne doit jamais constater :**

Le MJ ne perd aucune note saisie pendant une interruption réseau passagère. À la reconnexion, l'état qu'il voit dans l'interface correspond à l'état synchronisé.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-06 — Vue session](../usecases/UC-06-vue-session.md) | Exception E1 (erreur de sauvegarde d'une note de session) : le système conserve le contenu localement, une notification non bloquante indique la synchronisation en attente, la note est sauvegardée automatiquement au retour de la connexion. Scénario alternatif A5 (reprise d'une session LIVE interrompue). |
| [US-UC-06 — Vue session](../user-stories/US-UC-06-vue-session.md) | US-06-04 (prendre des notes de session MJ), RB-06-14 (en cas de perte de connexion, le contenu est conservé localement et synchronisé au retour), US-06-09 (reprendre une session interrompue), RB-06-29 (une interruption technique ne modifie pas le statut de la session). |

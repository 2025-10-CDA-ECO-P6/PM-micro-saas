# NFR-OFF-01 — Utilisation intégrale sans connexion en mode local

Famille **Hors connexion** — [Index de conception](../../README.md)

---

## Énoncé normatif

Un MJ en mode local peut préparer une campagne, capturer du contenu dans son espace personnel, structurer des scénarios, organiser ses documents, lancer et piloter une vue session — le tout sans aucune connexion réseau, y compris en déplacement ou dans un lieu sans accès à Internet.

---

## Raison d'être

Le mode local est le point d'entrée sans compte du produit. Sa valeur repose entièrement sur l'absence de prérequis : pas d'inscription, pas de connexion, pas d'engagement. Dès lors, conditionner l'accès à un réseau contredirait la promesse fondamentale de « friction d'entrée nulle » que la vision identifie comme premier différenciant (vision §5).

**[Nadia](../persona/persona-04-nadia.md)** illustre le premier cas critique : elle prépare ses sessions le dimanche soir, tard, dans des conditions variables. Elle a abandonné Notion précisément parce que l'outil demandait un investissement continu. Une dépendance réseau ajouterait une contrainte supplémentaire à un profil déjà peu disponible. Sa question fondamentale — *« l'app reste-t-elle utile si on ne la met à jour qu'une fois par mois ? »* — s'applique aussi à la connexion.

**[Thomas](../persona/persona-01-thomas.md)** pose une exigence de confiance différente : la portabilité et la possession des données doivent être réelles, pas théoriques. Un outil local qui nécessite un réseau pour fonctionner ne possède pas ses données : il les emprunte.

Le différenciant « données toujours possédées par l'utilisateur » (vision §5) n'est pas négociable pour ces deux profils. Si le mode local nécessite un réseau, la promesse de possession devient une affirmation sans contenu.

L'hypothèse de monétisation du produit (vision §2.3, H5) repose sur la conversion naturelle du mode local vers le compte cloud, déclenchée par un besoin de partage ou de sauvegarde — et non par une limitation artificielle du mode local. Limiter les fonctionnalités de préparation ou de session au prétexte d'absence de réseau saborderait cette conversion naturelle.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La préparation dans tout espace local du MJ (espace personnel, campagnes et one-shots) : structuration de scénarios, organisation de documents, capture à la volée — en totalité, sans connexion réseau.
- Le lancement et le pilotage d'une vue session complète, sans connexion réseau.
- La création d'éléments à la volée en cours de session, sans connexion réseau.
- La recherche dans le contenu local du MJ, sans connexion réseau.

**Ce que cette exigence ne couvre pas :**

- Le partage d'informations avec les joueurs : cette fonctionnalité nécessite une connexion — c'est un choix de périmètre explicite, pas un oubli.
- La sauvegarde dans le cloud : nécessite également une connexion, par définition.
- La synchronisation entre plusieurs appareils hors connexion : explicitement exclue du MVP. La vision §2.4 identifie cette capacité comme une complexité disproportionnée au regard du périmètre initial.
- La vue joueur et l'accès invité : ces fonctionnalités présupposent un compte MJ et une connexion (UC-01, règles métier).

---

## Critères d'acceptation produit

**Situation nominale — préparation sans connexion :**

Un MJ qui coupe la connexion réseau de son appareil avant d'ouvrir l'application peut créer une campagne, y ajouter des documents, les organiser en dossiers et naviguer dans son contenu. Aucun message d'erreur ne s'affiche, aucune fonctionnalité de préparation n'est désactivée ou grisée.

**Situation nominale — session sans connexion :**

Un MJ en mode local qui lance une session depuis une campagne déjà préparée, sans connexion réseau, accède à la vue session complète : panneaux configurés, documents de la campagne consultables, prise de notes, création à la volée. Aucun gel de l'interface n'est imputable à l'absence de réseau.

**Situation limite — lieu sans accès réseau :**

Un MJ qui utilise l'application dans un lieu sans réseau (zone blanche, transport sans Wi-Fi, sous-sol) constate le même comportement qu'avec une connexion disponible, pour toutes les fonctionnalités de préparation et de session.

**Situation limite — fonctionnalités cloud visibles sans connexion :**

Les fonctionnalités qui nécessitent un compte (partage, accès joueur) restent visibles dans l'interface mais s'affichent comme indisponibles en mode local, avec une invite à créer un compte. Elles ne produisent pas de message d'erreur réseau.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) | Use case fondateur. Le scénario nominal et les fonctionnalités listées (UC-01) définissent le périmètre que cette exigence garantit utilisable sans connexion. |
| [UC-06 — Vue session](../usecases/UC-06-vue-session.md) | La précondition « Le MJ est authentifié ou en mode local sans compte » et le périmètre de la vue session MJ en mode local sont directement servis par cette exigence. |
| [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md) | RB-01-01 (aucune donnée envoyée au serveur), RB-01-04 (données durables), RB-01-06 (fonctionnalités de partage nécessitent un compte). |

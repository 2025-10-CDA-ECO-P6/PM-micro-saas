# UC-06 — Utiliser la vue session

## Acteur principal

MJ

## Acteurs secondaires

Joueurs (accès en lecture/notes de session depuis leur propre vue pendant une session LIVE)

## Objectif

Permettre au MJ de piloter une session de jeu en accédant rapidement aux informations critiques, en prenant des notes à la volée, en gérant l'exposition des informations aux joueurs et en assurant la traçabilité des événements narratifs.

## Contexte

La vue session est l'interface centrale pendant une partie de jeu de rôle. Contrairement au mode préparation, elle est optimisée pour la rapidité : le MJ n'a pas le temps de naviguer dans des arborescences profondes. L'enjeu est de réduire la latence entre "le MJ cherche une information" et "le MJ la trouve", sans jamais casser le rythme de la table.

En parallèle, les joueurs disposent de leur propre vue restreinte depuis laquelle ils peuvent consulter ce que le MJ leur a partagé et prendre leurs notes personnelles (personnelle joueur).

## Besoin utilisateur

- **MJ** : piloter la session avec un accès immédiat aux dossiers et documents qu'il a configurés, tout en pouvant créer de nouveaux éléments ou partager des informations sans quitter la vue.
- **Joueurs** : accéder aux informations qui leur ont été partagées et consigner leurs propres notes pendant la session.

## Déclencheur

Le MJ clique sur "Lancer la session" pour démarrer une nouvelle session (qui s'ouvre directement en `LIVE`) ou pour accéder à une session déjà en cours (`LIVE`).

## Préconditions

- Une campagne existe et le MJ y a accès.
- Le MJ est membre `OWNER` ou `GM` de la campagne.
- Le MJ est authentifié **ou** en mode local sans compte. La vue session MJ est disponible dans les deux cas. La vue joueur (partage, accès invité) nécessite un compte.

## Scénario nominal — Lancement et navigation MJ

### Phase 1 — Lancement de la session

1. Le MJ clique sur "Lancer une session" depuis la vue campagne.
2. Il saisit un titre (et optionnellement sélectionne un scénario).
3. Le système crée la session directement en statut `LIVE` (`Session.Start()`).
4. La vue session s'ouvre avec les panneaux configurés dans configuration de la vue session.

### Phase 2 — Vue session MJ (interface)

La vue session MJ est un **tableau de bord configurable**. Les panneaux affichent le contenu
des dossiers que le MJ a configurés dans sa configuration de la vue session pour cette campagne.
Le MJ choisit quels dossiers il met en avant — certains privilégient leurs PNJ, d'autres
leurs lieux ou leurs scènes. Aucune structure n'est imposée par l'application.

**Panneaux de dossiers configurés** *(colonnes principales)*
- Chaque panneau correspond à un dossier de la campagne sélectionné dans la configuration de la vue session.
- Le MJ voit les documents du dossier avec leurs informations résumées (titre, type, propriétés utiles si renseignées).
- L'ordre et la sélection des panneaux sont configurables hors session (paramètres campagne).
- Si le MJ a associé un scénario à la session (`Session.scenarioId`), le document scénario apparaît dans le panneau de son dossier.

- Zone de saisie libre pour créer des notes de session liés à la session en cours.
- Chaque note est créée comme note de session avec `visibility = privé MJ` (par défaut),
  puis référencée dans notes rattachées à la session.
- Le MJ peut changer la visibilité d'une note vers visible par les joueurs pour la rendre visible aux joueurs.
- Les notes s'affichent en ordre inverse chronologique.

**Panneau Documents épinglés** *(favoris de session)*
- Affiche les documents explicitement épinglés pour cette session (via documents épinglés de la session).
- Le MJ peut ajouter un document épinglé depuis les résultats de recherche ou la navigation dans les dossiers.
- Clic sur un document ouvre un panneau de consultation en lecture rapide.

**Barre de recherche globale**
- Recherche full-text dans tous les documents de la campagne accessibles au MJ.
- Résultats pondérés : éléments de la session active en premier.
- Permet d'ouvrir un document dans un panneau latéral sans quitter la vue session.

### Phase 3 — Actions en cours de session

5. Le MJ navigue dans les panneaux de dossiers configurés.
6. Le MJ consulte les documents selon les besoins (PNJ, lieux, scénario, notes…).
7. Le MJ crée des notes de session au fil de la partie (interactions, décisions narratives, informations révélées).
8. Le MJ épingle des documents pour y accéder rapidement.
9. Le MJ utilise la recherche pour retrouver un élément absent des panneaux configurés.
10. Le MJ peut créer un PNJ ou une note à la volée → **voir UC-07**.
11. Le MJ peut partager une information aux joueurs → **voir UC-08**.

### Phase 4 — Fermeture de la session

12. Le MJ clique sur "Terminer la session".
13. Le système passe la session en `CLOSED`. Le MJ peut ajouter des notes rétroactives ou créer un document de récapitulatif s'il le souhaite.

---

## Scénario nominal — Vue joueur pendant la session LIVE

1. Le joueur accède à la campagne pendant une session `LIVE`.
2. Le système lui présente une vue joueur simplifiée.
3. Cette vue affiche :
   - les documents partagés par le MJ (`visibility = visible par les joueurs`) ;
   - les notes de session personnelles du joueur ;
   - une zone de création de notes de session personnelles.
4. Le joueur peut créer une `note de session` personnelle :
   - `visibility = personnelle joueur`, personnage associé par défaut si associé à un personnage.
   - La note n'est accessible qu'à son auteur.
5. Le joueur ne voit pas les notes de session privé MJ du MJ.
6. Le joueur ne voit pas les notes de session personnelle joueur des autres joueurs.

---

## Scénarios alternatifs

### A1 — Lancement sans scénario associé

Le MJ souhaite démarrer une session improvisée sans scénario préparé.

1. Depuis la vue campagne, le MJ clique sur "Lancer une session" et laisse le champ scénario vide.
2. Le système crée une session avec : `status = LIVE`, `scenarioId = null`, `documents épinglés = []`.
3. La vue session s'ouvre avec les panneaux de dossiers configurés — le scénario n'est pas mis en avant.
4. Le MJ navigue directement dans ses dossiers et peut épingler des documents au fil de la session.

### A2 — Création d'un élément à la volée

Le MJ a besoin d'un PNJ ou d'une note qui n'a pas été préparée.

1. Le MJ clique sur "Créer" dans le panneau Documents de session ou notes de session.
2. Le flux de création rapide s'ouvre → **voir UC-07**.
3. L'élément créé est automatiquement référencé par la session active si nécessaire.
4. Un document créé est automatiquement épinglé dans documents épinglés de la session.

### A3 — Recherche d'un élément non prévu

Le MJ cherche un document de lore, une note ancienne ou un PNJ absent des panneaux.

1. Le MJ utilise la barre de recherche.
2. Le flux de recherche s'exécute → **voir UC-14**.
3. Le MJ peut épingler le résultat dans le panneau Documents épinglés pour y revenir facilement.

### A4 — Partage en direct d'un document

Le MJ décide de rendre visible un document aux joueurs.

1. Le MJ clique sur "Partager" sur un document.
2. Le flux de partage s'exécute → **voir UC-08**.
3. La visibilité du document passe à visible par les joueurs (opération l’action de partage dans la bibliothèque de contenu).
4. Le document est automatiquement épinglé dans documents épinglés de la session pour un accès rapide.
5. Les joueurs voient immédiatement le document dans leur vue.

### A5 — Reprise d'une session LIVE interrompue

La session a été interrompue (perte de connexion, pause) et le MJ la reprend.

1. Le MJ accède à la session depuis la vue campagne.
2. La session est déjà au statut `LIVE`.
3. La vue session s'ouvre avec l'état précédent intégralement restauré : notes de session, documents épinglés, panneaux de dossiers configurés.

### A6 — Épingler ou désépingler un document en cours de session

1. Le MJ clique sur "Épingler" depuis un document visible dans un panneau ou depuis les résultats de recherche.
2. Le système appelle épinglage du document.
3. Le document apparaît dans le panneau Documents épinglés.
4. Pour désépingler : le MJ clique sur "Retirer" — l'entrée est supprimée de documents épinglés. Le document reste intact dans la campagne.

---

## Exceptions

### E1 — Erreur de sauvegarde d'une note de session

Le réseau est indisponible lors de la création d'une note.

- Le système conserve le contenu localement (draft).
- Une notification non bloquante indique que la synchronisation est en attente.
- Dès que la connexion est rétablie, la note est sauvegardée automatiquement.

### E2 — Accès non autorisé à la vue MJ

Un joueur tente d'accéder à l'URL de la vue session MJ.

- Le système refuse l'accès et retourne une erreur 403.
- Le joueur est redirigé vers sa propre vue joueur pour la session active.

### E3 — Session non LIVE

Le MJ tente d'accéder à la vue session d'une session `CLOSED` ou `ARCHIVED`.

- `CLOSED` : le système affiche la vue lecture seule de la session clôturée côté joueurs ; le MJ peut encore ajouter des notes de session rétroactives et modifier le résumé.
- `ARCHIVED` : lecture seule intégrale, aucune modification possible.

### E4 — Session sans scénario associé

La session a été créée sans `scenarioId` (session improvisée).

- Le scénario n'est pas mis en avant dans les panneaux de la vue session.
- Le MJ peut créer un document à la volée → **voir UC-07**.

---

## Postconditions

- Toutes les notes de session créées pendant la session sont sauvegardées comme notes de session
  et référencées par notes rattachées à la session.
- Les modifications de documents épinglés de la session sont persistées.
- Les documents partagés aux joueurs ont leur visibilité mise à jour.
- La session reste au statut `LIVE` jusqu'à clôture explicite par le MJ.

---

## Données manipulées

### Session

- `id: SessionId`
- Campagne associée
- `scenarioId: DocumentId?` (nullable — scénario joué, document la bibliothèque de contenu)
- `status: SessionStatus` — `LIVE | CLOSED | ARCHIVED`
- `documents épinglés: DocumentId[]`
- `startedAt: DateTime`

### configuration de la vue session

- Campagne associée
- Liste ordonnée des dossiers mis en avant dans la vue session

### note de session (documents créés pendant la session)

- `id: DocumentId`
- `type optionnel: LIVE_NOTE`
- dossier associé — dossier "Notes" de la campagne par défaut
- blocs
- Visibilité : visible par les joueurs, privé MJ ou personnelle joueur
- Personnage associé, pour les notes personnelles joueur
- `propriétés structurées.guestAccessId: string?` — renseigné si auteur = accès invité
- audit : auteur, date de création, date de modification
- référence depuis la session : notes rattachées à la session

### Documents consultés / épinglés

- Documents de la campagne filtrés selon la `visibility` (le MJ voit tout, les joueurs voient visible par les joueurs)
- documents épinglés de la session mis à jour

### Documents par dossier

- Contenu des dossiers configurés dans `configuration de la vue session.dossiers mis en avant`
- Données affichées selon le type de document si renseigné : propriétés utiles en résumé
- Le contenu des dossiers appartient à la bibliothèque de contenu — la conduite de session ne le possède pas

---

## Règles métier

- La vue session MJ est accessible uniquement au MJ de la campagne.
- Les joueurs disposent d'une vue distincte : ils ne voient que les informations partagées et leurs propres notes personnelle joueur.
- **notes de session en session LIVE** :
  - MJ : peut créer des notes de session à tout moment pendant `LIVE`. Visibilité par défaut : privé MJ. Peut basculer en visible par les joueurs.
  - Joueur : peut créer des notes de session uniquement pendant une session `LIVE`. Visibilité par défaut : personnelle joueur, liée au personnage associé si renseigné.
- **notes de session après session** :
  - MJ : peut créer et modifier des notes de session rétroactives sur une session `CLOSED` (pour compléter ses notes après la partie).
  - Joueur : ne peut créer des notes de session que pendant `LIVE`.
- Une `note de session` avec `visibility = personnelle joueur` est inaccessible au MJ, quelles que soient ses permissions de campagne.
- Un joueur invité sans compte peut créer une note de session personnelle si son accès invité est associé à un personnage associé. La note reste récupérable lors d'une séance suivante via un nouveau lien sécurisé associé au même personnage.
- Partager un document change sa `visibility` à visible par les joueurs dans la bibliothèque de contenu (opération permanente) — ce n'est pas un partage temporaire de session.
- documents épinglés de la session est une liste modifiable manuellement à tout moment pendant une session `LIVE`.
- La machine d'états `Session` est unidirectionnelle : `LIVE → CLOSED → ARCHIVED`.
- Un document sans `visibility = visible par les joueurs` n'est pas visible dans la vue joueur, même s'il est épinglé dans la session.

---

## Critères d'acceptation

### Vue MJ

- Le MJ peut lancer une session (créée directement en `LIVE`).
- Le MJ peut lancer une session sans scénario associé.
- Le MJ voit les panneaux de dossiers configurés dans configuration de la vue session.
- Le MJ peut consulter les documents de ses dossiers configurés avec leurs informations résumées.
- Le MJ peut créer une note de session (visibilité privé MJ par défaut).
- Le MJ peut changer la visibilité d'une note de session vers visible par les joueurs.
- Le MJ peut épingler un document dans le panneau Documents épinglés.
- Le MJ peut rechercher dans tous les documents de la campagne sans quitter la vue session.
- Le MJ peut créer un élément à la volée → UC-07.
- Le MJ peut partager une information → UC-08.
- Le MJ peut terminer la session → UC-12.

### Vue Joueur

- Le joueur voit les informations partagées (visible par les joueurs).
- Le joueur peut créer une note de session personnelle joueur pendant une session `LIVE`.
- Le joueur ne voit pas les notes de session privé MJ du MJ.
- Le joueur ne voit pas les notes de session personnelle joueur des autres joueurs.

### Persistance

- Les notes de session sont sauvegardées avec leur session associée.
- documents épinglés de la session est persisté.

---

## Relations avec d'autres use cases

| Use Case | Relation |
|---|---|
| [UC-05](UC-05-organiser-dossiers.md) — Organiser le contenu | Fournit documents, types, dossiers et tags utilisés en session |
| [UC-08](UC-08-partager-information.md) — Partager une information | Appelé depuis UC-06 A4 |
| [UC-14](UC-14-recherche.md) — Rechercher une information | Appelé depuis UC-06 A3 |
| [UC-12](UC-12-rejoindre-campagne.md) — Rejoindre une campagne ou session | Alimente la vue joueur et les accès invités |
| [UC-07](UC-07-creation-volee-session.md) — Créer à la volée | Appelé depuis UC-06 A2 |

---

## Questions à valider en interview

- Les MJ veulent-ils voir les notes de session des sessions précédentes dans la vue session (historique récent) ?
- Quel niveau de détail sur un PNJ est utile dans la vue session : traits seulement, ou accès à la fiche complète en panneau latéral ?
- Les joueurs accèdent-ils depuis un appareil séparé (smartphone) ou partagent-ils l'écran ?
- Faut-il une notification pour les joueurs quand le MJ partage une nouvelle information en direct ?
- Le mode "offline partiel" (draft local en cas de perte réseau) est-il critique pour le MVP ?

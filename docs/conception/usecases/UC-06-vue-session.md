# UC-06 — Utiliser la vue session

## Acteur principal

MJ

## Acteurs secondaires

Joueurs (accès en lecture/LiveNotes depuis leur propre vue pendant une session LIVE)

## Objectif

Permettre au MJ de piloter une session de jeu en accédant rapidement aux informations critiques, en prenant des notes à la volée, en gérant l'exposition des informations aux joueurs et en assurant la traçabilité des événements narratifs.

## Contexte

La vue session est l'interface centrale pendant une partie de jeu de rôle. Contrairement au mode préparation, elle est optimisée pour la rapidité : le MJ n'a pas le temps de naviguer dans des arborescences profondes. L'enjeu est de réduire la latence entre "le MJ cherche une information" et "le MJ la trouve", sans jamais casser le rythme de la table.

En parallèle, les joueurs disposent de leur propre vue restreinte depuis laquelle ils peuvent consulter ce que le MJ leur a partagé et prendre leurs notes personnelles (`PLAYER_PRIVATE`).

## Besoin utilisateur

- **MJ** : piloter la session avec un accès immédiat aux scènes, PNJ, personnages et notes, tout en pouvant créer de nouveaux éléments ou partager des informations sans quitter la vue.
- **Joueurs** : accéder aux informations qui leur ont été partagées et consigner leurs propres notes pendant la session.

## Déclencheur

Le MJ clique sur "Lancer la session" depuis la page de préparation d'une session (`PLANNED → LIVE`) ou depuis une session déjà en cours (`LIVE`).

## Préconditions

- Une campagne existe et le MJ y a accès.
- Une session existe (status `PLANNED` ou `LIVE`), ou le MJ lance une session rapide sans session préparée.
- Le MJ est authentifié.

## Scénario nominal — Lancement et navigation MJ

### Phase 1 — Lancement de la session

1. Le MJ sélectionne une session `PLANNED` depuis la vue campagne.
2. Il clique sur "Lancer la session".
3. Le système transite la session vers le statut `LIVE`.
4. La vue session s'ouvre.

### Phase 2 — Vue session MJ (interface)

La vue session MJ est composée de plusieurs panneaux :

**Panneau Scénario & Scènes** *(colonne centrale)*
- Affiche le scénario actif associé à la session (via `Session.scenarioId`, nullable).
- Liste les scènes du scénario dans leur ordre (`Scenario.order`).
- Chaque scène indique son statut (non jouée / en cours / jouée).
- Le MJ peut marquer une scène comme jouée d'un clic.

**Panneau PNJ** *(colonne latérale)*
- Affiche les PNJ présents dans `Session.selectedNpcIds`.
- Cette liste est auto-déduite depuis les scènes du scénario actif (union des `Scene.linkedNpcIds`) lors de la préparation (UC-05), puis modifiable manuellement par le MJ en cours de session.
- Le MJ peut ajouter un PNJ non prévu ou en retirer un.
- Chaque PNJ affiche : nom, traits résumés, statut (actif/neutralisé/fui).

**Panneau Personnages Joueurs** *(colonne latérale)*
- Affiche les fiches `PlayerCharacter` associées à la campagne.
- Le MJ peut consulter une fiche complète en un clic.
- Les fiches sans propriétaire (`ownerId = null`) sont affichées avec la mention "sans joueur associé".

**Panneau LiveNotes MJ** *(zone de notes rapides)*
- Zone de saisie libre pour créer des `LiveNote` liés à la session en cours.
- Chaque note est créée avec : `sessionId`, `authorUserId = MJ.userId`, `authorRole = GM`, `visibility = PRIVATE` (par défaut).
- Le MJ peut changer la visibilité d'une note vers `SHARED` pour la rendre visible aux joueurs.
- Les notes s'affichent en ordre inverse chronologique.

**Panneau Documents épinglés** *(favoris de session)*
- Affiche les documents explicitement épinglés pour cette session (via `Session.pinnedItems`).
- Le MJ peut ajouter un document épinglé depuis les résultats de recherche ou la navigation.
- Clic sur un document ouvre un panneau de consultation en lecture rapide.

**Barre de recherche globale**
- Recherche full-text dans tous les documents de la campagne accessibles au MJ.
- Résultats pondérés : éléments de la session active en premier.
- Permet d'ouvrir un document dans un panneau latéral sans quitter la vue session.

### Phase 3 — Actions en cours de session

5. Le MJ consulte les scènes et navigue dans le scénario.
6. Le MJ consulte les fiches PNJ et personnages selon les besoins.
7. Le MJ crée des LiveNotes au fil de la partie (interactions, décisions narratives, informations révélées).
8. Le MJ marque des scènes comme jouées.
9. Le MJ utilise la recherche pour retrouver un élément non prévu.
10. Le MJ peut créer un PNJ ou une note à la volée → **voir UC-14**.
11. Le MJ peut partager une information aux joueurs → **voir UC-09**.

### Phase 4 — Fermeture de la session

12. Le MJ clique sur "Terminer la session".
13. Le système affiche l'étape de clôture → **voir UC-12**.

---

## Scénario nominal — Vue joueur pendant la session LIVE

1. Le joueur accède à la campagne pendant une session `LIVE`.
2. Le système lui présente une vue joueur simplifiée.
3. Cette vue affiche :
   - les informations partagées par le MJ (`visibility = SHARED` ou `PUBLIC`) ;
   - les `LiveNote` du joueur avec `visibility = PLAYER_PRIVATE` et `ownerCharacterId = personnage associé au requester` ;
   - une zone de création de LiveNotes personnelles.
4. Le joueur peut créer une `LiveNote` personnelle :
   - `authorRole = PLAYER`, `visibility = PLAYER_PRIVATE`, `ownerCharacterId = characterId` (par défaut, non modifiable).
   - La note n'est accessible qu'au requester associé au même personnage.
5. Le joueur ne voit pas les LiveNotes `PRIVATE` du MJ.
6. Le joueur ne voit pas les LiveNotes `PLAYER_PRIVATE` des autres joueurs.

---

## Scénarios alternatifs

### A1 — Lancement sans session préparée

Le MJ souhaite démarrer une session improvisée sans session préparée.

1. Depuis la vue campagne, le MJ clique sur "Lancer une session rapide".
2. Le système crée une session avec : `status = LIVE`, `scenarioId = null`, `selectedNpcIds = []`, `pinnedItems = []`.
3. La vue session s'ouvre avec les panneaux Scénario et PNJ vides.
4. Le MJ peut ajouter manuellement des PNJ et des documents épinglés.

### A2 — Création d'un élément à la volée

Le MJ a besoin d'un PNJ ou d'une note qui n'a pas été préparée.

1. Le MJ clique sur "Créer" dans le panneau PNJ ou LiveNotes.
2. Le flux de création rapide s'ouvre → **voir UC-14**.
3. L'élément créé est automatiquement lié à la session active.
4. Un PNJ créé est ajouté à `Session.selectedNpcIds`.

### A3 — Recherche d'un élément non prévu

Le MJ cherche un document de lore, une note ancienne ou un PNJ absent des panneaux.

1. Le MJ utilise la barre de recherche.
2. Le flux de recherche s'exécute → **voir UC-11**.
3. Le MJ peut épingler le résultat dans le panneau Documents épinglés pour y revenir facilement.

### A4 — Partage en direct d'une information

Le MJ décide de rendre visible un document ou une LiveNote aux joueurs.

1. Le MJ clique sur "Partager" sur un document ou une LiveNote.
2. Le flux de partage s'exécute → **voir UC-09**.
3. La visibilité de la ressource est mise à jour (`SHARED` ou `PUBLIC`).
4. Si la ressource devient `SHARED`, des `ContentAccessRule` ciblent les membres ou personnages concernés.
5. Les joueurs voient immédiatement l'information dans leur vue.

### A5 — Reprise d'une session LIVE interrompue

La session a été interrompue (perte de connexion, pause) et le MJ la reprend.

1. Le MJ accède à la session depuis la vue campagne.
2. La session est déjà au statut `LIVE`.
3. La vue session s'ouvre avec l'état précédent intégralement restauré : scènes marquées, LiveNotes, PNJ sélectionnés.

### A6 — Ajout ou retrait de PNJ en cours de session

1. Le MJ clique sur "Ajouter un PNJ" dans le panneau PNJ.
2. Une recherche s'ouvre dans les PNJ de la campagne.
3. Le MJ sélectionne un PNJ.
4. Le système l'ajoute à `Session.selectedNpcIds`.
5. Le PNJ apparaît dans le panneau.
6. Pour retirer un PNJ : le MJ clique sur "Retirer" — l'entrée est supprimée de `selectedNpcIds`. Le document PNJ reste intact dans la campagne.

---

## Exceptions

### E1 — Erreur de sauvegarde d'une LiveNote

Le réseau est indisponible lors de la création d'une note.

- Le système conserve le contenu localement (draft).
- Une notification non bloquante indique que la synchronisation est en attente.
- Dès que la connexion est rétablie, la note est sauvegardée automatiquement.

### E2 — Accès non autorisé à la vue MJ

Un joueur tente d'accéder à l'URL de la vue session MJ.

- Le système refuse l'accès et retourne une erreur 403.
- Le joueur est redirigé vers sa propre vue joueur pour la session active.

### E3 — Session non LIVE

Le MJ tente d'accéder à la vue session d'une session `PLANNED`, `CLOSED` ou `ARCHIVED`.

- `PLANNED` : le bouton "Lancer" est proposé, la vue session n'est pas accessible directement.
- `CLOSED` : le système affiche la vue lecture seule de la session clôturée (consultation des LiveNotes et résumé uniquement).
- `ARCHIVED` : lecture seule intégrale, aucune modification possible.

### E4 — Scénario sans scènes

La session est liée à un scénario existant mais sans scènes définies.

- Le panneau Scénario affiche le scénario avec un message "Aucune scène définie".
- Le MJ peut créer une scène à la volée → **voir UC-14**.

---

## Postconditions

- Toutes les `LiveNote` créées pendant la session sont sauvegardées avec leur `sessionId`.
- Les scènes marquées comme jouées conservent leur statut.
- Les modifications de `Session.selectedNpcIds` et `Session.pinnedItems` sont persistées.
- Les documents partagés aux joueurs ont leur visibilité mise à jour.
- La session reste au statut `LIVE` jusqu'à clôture explicite par le MJ.

---

## Données manipulées

### Session

- `id: SessionId`
- `campaignId: CampaignId`
- `scenarioId: ScenarioId?` (nullable)
- `status: SessionStatus` — `PLANNED | LIVE | CLOSED | ARCHIVED`
- `selectedNpcIds: NpcId[]`
- `pinnedItems: PinnedItem[]`
- `startedAt: DateTime?`

### LiveNote (créées pendant la session)

- `id: LiveNoteId`
- `sessionId: SessionId`
- `content: String`
- `authorUserId: UserId?`
- `authorGuestAccessId: GuestAccessId?`
- `ownerCharacterId: CharacterId?` — obligatoire pour `PLAYER_PRIVATE`
- `authorRole: LiveNoteAuthorRole` — `GM | PLAYER`
- `visibility: Visibility` — `PRIVATE` (GM) | `PLAYER_PRIVATE` (joueur)
- `createdAt: DateTime`

### Documents consultés / épinglés

- Documents de la campagne avec leur `AccessPolicy` appliqué
- `Session.pinnedItems` mis à jour

### PNJ

- `Session.selectedNpcIds` mis à jour
- Données affichées : nom, traits résumés, statut

---

## Règles métier

- La vue session MJ est accessible uniquement au MJ de la campagne.
- Les joueurs disposent d'une vue distincte : ils ne voient que les informations partagées et leurs propres notes `PLAYER_PRIVATE`.
- **LiveNotes en session LIVE** :
  - MJ : peut créer des LiveNotes à tout moment pendant `LIVE`. Visibilité par défaut : `PRIVATE`. Peut basculer en `SHARED`.
  - Joueur : peut créer des LiveNotes uniquement pendant une session `LIVE`. Visibilité fixe : `PLAYER_PRIVATE` (non modifiable par le joueur) et liée au `CharacterId`.
- **LiveNotes après session** :
  - MJ : peut créer et modifier des LiveNotes rétroactives sur une session `CLOSED` (pour compléter ses notes après la partie).
  - Joueur : ne peut créer des LiveNotes que pendant `LIVE`.
- Une `LiveNote` avec `visibility = PLAYER_PRIVATE` est inaccessible au MJ, quelles que soient ses permissions de campagne.
- Un joueur invité sans compte peut créer une LiveNote personnelle si son `GuestAccess` est associé à un `CharacterId`. La note reste récupérable lors d'une séance suivante via un nouveau lien sécurisé associé au même personnage.
- Les partages de `Document`, `LiveNote` et `SessionSummary` utilisent tous `AccessPolicy` et `ContentAccessRule`.
- `Session.selectedNpcIds` est une liste modifiable manuellement. La valeur initiale est déduite des scènes du scénario actif lors de la préparation (UC-05).
- Marquer une scène comme jouée ne supprime pas le scénario et ne déclenche aucune transition d'état automatique sur la session.
- La machine d'états `Session` est unidirectionnelle : `PLANNED → LIVE → CLOSED → ARCHIVED`.
- Un document sans accès explicite pour un joueur n'est pas visible dans la vue joueur, même s'il est épinglé dans la session.

---

## Critères d'acceptation

### Vue MJ

- Le MJ peut lancer une session `PLANNED` (transition `PLANNED → LIVE`).
- Le MJ peut lancer une session rapide sans session préparée.
- Le MJ peut consulter le scénario actif et ses scènes dans leur ordre.
- Le MJ peut marquer une scène comme jouée.
- Le MJ peut consulter les PNJ de `selectedNpcIds` avec leurs informations résumées.
- Le MJ peut ajouter et retirer des PNJ de `selectedNpcIds` en cours de session.
- Le MJ peut consulter les fiches personnages joueurs.
- Le MJ peut créer une LiveNote (visibilité `PRIVATE` par défaut).
- Le MJ peut changer la visibilité d'une LiveNote vers `SHARED`.
- Le MJ peut épingler un document dans le panneau Documents épinglés.
- Le MJ peut rechercher dans tous les documents de la campagne sans quitter la vue session.
- Le MJ peut créer un élément à la volée → UC-14.
- Le MJ peut partager une information → UC-09.
- Le MJ peut terminer la session → UC-12.

### Vue Joueur

- Le joueur voit les informations partagées (`SHARED`, `PUBLIC`).
- Le joueur peut créer une LiveNote `PLAYER_PRIVATE` pendant une session `LIVE`.
- Le joueur ne voit pas les LiveNotes `PRIVATE` du MJ.
- Le joueur ne voit pas les LiveNotes `PLAYER_PRIVATE` des autres joueurs.

### Persistance

- Les LiveNotes sont sauvegardées avec leur `sessionId`.
- `Session.selectedNpcIds` et `Session.pinnedItems` sont persistés.
- Les scènes marquées comme jouées conservent leur statut entre les accès.

---

## Relations avec d'autres use cases

| Use Case | Relation |
|---|---|
| [UC-05](UC-05-preparer-session.md) — Préparer une session | Précède UC-06 : prépare `selectedNpcIds`, scénario, scènes |
| [UC-09](UC-09-partager-information.md) — Partager une information | Appelé depuis UC-06 A4 |
| [UC-11](UC-11-recherche.md) — Rechercher une information | Appelé depuis UC-06 A3 |
| [UC-12](UC-12-cloturer-session.md) — Clôturer une session | Appelé depuis UC-06 Phase 4 |
| [UC-13](UC-13-notes-personnelles-joueur.md) — Notes personnelles joueur | Sous-cas de la vue joueur (A5) |
| [UC-14](UC-14-creation-volee-session.md) — Créer à la volée | Appelé depuis UC-06 A2 |

---

## Questions à valider en interview

- Les MJ veulent-ils voir les LiveNotes des sessions précédentes dans la vue session (historique récent) ?
- Quel niveau de détail sur un PNJ est utile dans la vue session : traits seulement, ou accès à la fiche complète en panneau latéral ?
- Les joueurs accèdent-ils depuis un appareil séparé (smartphone) ou partagent-ils l'écran ?
- Faut-il une notification pour les joueurs quand le MJ partage une nouvelle information en direct ?
- Le mode "offline partiel" (draft local en cas de perte réseau) est-il critique pour le MVP ?

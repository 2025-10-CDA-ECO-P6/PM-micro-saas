# Parcours bout-en-bout — Émilie, MJ improvisatrice, capture en séance

> Objet : vérifier les coutures inter-UC sur le parcours propre à Émilie.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict).
> Aucun nom de technologie, aucun ADR cité comme source d'autorité.
> Renvoi : [README](README.md) · [Fiche persona](../persona/persona-02-emilie.md)

---

## Présentation du parcours

Émilie ne prépare presque rien avant une session. Sa philosophie — et celle des systèmes qu'elle joue — veut que le contenu émerge à la table avec ses joueurs. Son douleur principale n'est pas la préparation : c'est la capture pendant la séance. Un PNJ inventé à la volée, noté sur un index card, perdu six mois plus tard.

Ce parcours couvre son entrée dans l'outil, son moment central d'usage (la séance comme lieu de création), et le rangement a posteriori. Il ne commence pas par une phase de préparation dense — c'est délibéré. L'outil doit lui prouver sa valeur sur la capture et la session avant tout.

Le parcours suppose qu'Émilie démarre en mode local (sans compte), et crée son compte lorsque le besoin de partager avec ses joueurs apparaît — ce qui correspond au déclencheur naturel documenté dans UC-10.

---

## Étape 1 — Démarrage sans compte et création d'une campagne minimale

**UC porteur :** [UC-01](../usecases/UC-01-mode-local-sans-compte.md) | **UJ porteur :** [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md)

### Ce qu'Émilie cherche à faire

Ouvrir l'outil, commencer immédiatement, sans remplir de formulaire. Elle a une session dans deux heures.

### Comportements observables

- L'application propose deux options équivalentes. Émilie choisit « Commencer sans compte ».
- Un message court indique que les données sont stockées dans ce navigateur — non bloquant.
- Elle est redirigée vers la création de campagne.
- Elle saisit un nom (« Ironsworn — Fer et Cendres »), laisse le reste vide, valide.
- La campagne est créée avec quatre dossiers système : Personnages, Joueurs, Scénarios, Notes.
- Les fonctionnalités de partage avec les joueurs sont visibles mais désactivées.

### État laissé

- Mode local actif : aucun `User` instancié, données dans le stockage local du navigateur.
- Une `Campagne` existe, avec ses quatre dossiers système et son dossier virtuel « Non classés ».
- Une `SessionViewConfig` est créée automatiquement à `CampaignCreated` (glossaire §5).

### Couture vers l'étape 2

UC-05 précondition : « Une campagne existe. Le MJ est propriétaire de la campagne. » Les deux conditions sont satisfaites. UC-05 scénario nominal A6 confirme que les dossiers système existent à la création de la campagne.

**Couture continue.**

---

## Étape 2 — Configuration minimale des dossiers (ou absence de configuration)

**UC porteur :** [UC-05](../usecases/UC-05-organiser-dossiers.md) | **UJ porteur :** [UJ-UC-05](../user-journeys/UJ-UC-05-organiser-contenu-dossiers.md)

### Ce qu'Émilie cherche à faire

Émilie joue Ironsworn : les entités centrales sont les serments, les liens et les personnages. Les dossiers par défaut (Personnages, Joueurs, Scénarios, Notes) sont approximativement utiles. Elle ne reconfigure pas à fond — au mieux, elle renomme « Scénarios » en « Serments » ou crée un dossier « Liens ». Plus probablement, elle démarre sans configuration et improvise.

Son attente est que ça marche sans rien configurer. Le scénario documenté dans UJ-UC-05 (profil Nadia) couvre ce cas exactement.

### Comportements observables

- Émilie peut renommer un dossier système si elle le souhaite (ex. : « Personnages » → « Liens »). L'opération est en ligne, sans confirmation (UC-05 A1).
- Si elle ne fait rien, les dossiers par défaut sont disponibles.
- La `SessionViewConfig` existe mais ses `focusedFolders` ne sont pas encore configurés — la vue session affichera un message d'invitation à configurer les panneaux à la première session (UC-06 §préconditions).

### État laissé

- La structure de dossiers est en place — modifiée ou non.
- La `SessionViewConfig` existe, potentiellement sans panneaux configurés.
- Aucun `Document` n'a encore été créé (Émilie ne prépare pas).

### Couture vers l'étape 3

UC-06 précondition : « Une campagne existe et le MJ y a accès. Le MJ est authentifié ou en mode local sans compte. La vue session MJ est disponible dans les deux cas. » Toutes les conditions sont satisfaites.

**Couture continue.**

---

## Étape 3 — Lancement de session improvisée

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) | **UJ porteur :** [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md)

### Ce qu'Émilie cherche à faire

Lancer rapidement. Elle ne sélectionne pas de scénario — il n'y en a pas. Elle veut ouvrir un tableau de bord et commencer à jouer.

### Comportements observables

- Émilie clique « Lancer une session » depuis la vue campagne.
- Elle saisit un titre (ex. : « Séance 1 »), laisse le champ scénario vide (`scenarioId = null`).
- La `Session` est créée directement en `LIVE` (UC-06 scénario nominal phase 1).
- La vue session s'ouvre. Les panneaux sont ceux configurés dans `SessionViewConfig` — s'ils ne sont pas configurés, les dossiers par défaut s'affichent avec un message d'invitation à configurer (UJ-UC-06 §flux fonctionnel).
- Émilie ne reconfigure pas les panneaux — elle s'en désintéresse et cherche immédiatement la zone de notes de session.

### État laissé

- Une `Session` existe en état `LIVE` dans la campagne.
- `pinnedDocumentIds` est vide.
- `sessionNoteIds` est vide.

### Couture vers l'étape 4

UC-07 précondition : « Une session est en statut LIVE ou CLOSED. Le MJ est propriétaire de la campagne (compte cloud) ou en mode local (UC-01). » Les deux conditions sont satisfaites — la session est `LIVE`, Émilie est en mode local.

**Couture continue.**

---

## Étape 4 — Capture à la volée pendant la séance

**UC porteur :** [UC-07](../usecases/UC-07-creation-volee-session.md) | **UJ porteur :** [UJ-UC-07](../user-journeys/UJ-UC-07-creation-volee-session.md)

### Ce qu'Émilie cherche à faire

C'est le moment central de son usage. Ses joueurs décident d'interroger un passant anonyme qu'elle n'a pas préparé. Elle a deux secondes. Elle crée le PNJ maintenant.

### Comportements observables

- Émilie clique sur « Créer » dans la barre de la vue session.
- Le panneau de création rapide s'ouvre. Un seul champ visible : le titre.
- Elle choisit le type PNJ, saisit « Marchand de soie », valide.
- Le `Document` est créé avec `visibility = GM_ONLY`, lié à la campagne, placé dans le dossier Personnages.
- Il est automatiquement épinglé dans `pinnedDocumentIds` de la session (UC-07 règle métier).
- Émilie continue la partie sans avoir quitté le contexte de session.
- Elle répète cette opération autant de fois que nécessaire — PNJ, lieux, notes.
- Elle crée aussi des notes de session via la zone de saisie libre de la vue session (UC-06), avec `visibility = GM_ONLY` par défaut.

### Comportement de la note de session (UC-06)

La zone de notes de session est distincte du panneau de création rapide UC-07. Une note de session est un `Document` de type `LIVE_NOTE`, référencée dans `sessionNoteIds`. Visibilité par défaut : `GM_ONLY`. Émilie peut basculer certaines notes en `PUBLIC` pour les rendre visibles aux joueurs — mais en mode local, aucun joueur ne peut accéder à la session (UC-06 règle métier « Mode local et vue joueur »). Ce basculement est donc sans effet pratique pour l'instant.

### État laissé

- Plusieurs `Document` créés, liés à la campagne, dans leurs dossiers respectifs.
- Des `LIVE_NOTE` créées, référencées dans `sessionNoteIds` de la session.
- La session est toujours en `LIVE`.
- Émilie est toujours en mode local.

### Couture vers l'étape 5

UC-06 scénario nominal phase 4 : le MJ clique « Terminer la session ». La `Session` passe en `CLOSED`. Le MJ peut ajouter des notes rétroactives sur une session `CLOSED` (UC-06 règle métier).

**Couture continue.**

---

## Étape 5 — Fermeture de session et notes rétroactives

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) | **UJ porteur :** [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md)

### Ce qu'Émilie cherche à faire

La session vient de se terminer. Elle clôt la session. Elle peut ajouter des notes rétroactives — ce qu'elle a oublié de capturer pendant la partie. Émilie peut aussi créer un élément à la volée depuis une session `CLOSED` (UC-07 A5).

### Comportements observables

- Émilie clique « Terminer la session ». La `Session` passe en `CLOSED`.
- Elle peut ajouter des notes rétroactives sur la session `CLOSED` (UC-06 scénario nominal phase 4).
- Elle peut créer des documents à la volée depuis la session `CLOSED` (UC-07 A5 : « La création à la volée est possible depuis une session CLOSED »).
- Elle peut ajouter un résumé narratif libre (`summary`), modifiable uniquement en `CLOSED`.
- La transition vers `ARCHIVED` est irréversible — Émilie ne l'effectue pas forcément immédiatement.

### État laissé

- La `Session` est en état `CLOSED`.
- Des `LIVE_NOTE` supplémentaires ont pu être créées rétroactivement.
- Des `Document` supplémentaires ont pu être créés à la volée depuis la session `CLOSED`.
- Le `summary` a pu être renseigné.

### Couture vers l'étape 6

UC-04 précondition : « Une campagne existe. Le MJ a accès à la campagne. » Satisfait. UC-04 scénario nominal : le MJ peut accéder à la bibliothèque de documents et enrichir les documents créés à la volée.

**Couture continue.**

---

## Étape 6 — Rangement et enrichissement a posteriori

**UC porteur :** [UC-04](../usecases/UC-04-gerer-documents-campagne.md) | **UJ porteur :** [UJ-UC-04](../user-journeys/UJ-UC-04-gerer-documents-campagne.md) (si besoin)

### Ce qu'Émilie cherche à faire

Après la session, Émilie retrouve les documents créés à la volée. Certains ne sont que des titres. Elle peut les enrichir, les déplacer, les lier entre eux. C'est le rangement après coup — distinct de la préparation en amont.

### Comportements observables

- Les `Document` créés à la volée pendant la session sont accessibles depuis la bibliothèque de contenu, dans leurs dossiers (ou dans « Non classés » si le dossier d'accueil n'a pas été résolu).
- Émilie peut enrichir le corps d'un document en ajoutant des blocs libres.
- Elle peut lier des documents entre eux (`DocumentLink`) — ex. : relier le PNJ « Marchand de soie » à un lieu.
- Elle peut déplacer des documents vers un autre dossier (UC-05 scénario nominal « Déplacer un document »).
- Elle peut modifier la visibilité d'un document.

### État laissé

- Les `Document` sont enrichis, classés, liés.
- La campagne dispose d'un corpus de contenu constitué de la session, réorganisé selon les besoins d'Émilie.

### Couture vers l'étape 7

Émilie souhaite partager certains documents ou informations avec ses joueurs lors de la prochaine session. UC-08 précondition : « Le MJ est authentifié et propriétaire de la campagne. » Émilie est en mode local — elle n'est pas authentifiée et aucun `User` n'est instancié (glossaire §2, Mode local). La précondition n'est pas satisfaite.

**Couture conditionnelle documentée : UC-08 n'est pas accessible en mode local, UC-10 s'intercale.** Ce parcours d'intercalation est formulé dans UC-10 (Contexte : le MJ qui a conduit des sessions en mode local crée son compte, migre sa campagne avec tout son historique, puis invite ses joueurs) et porté par UJ-UC-01 (tentative de partage comme point de conversion). Voir couture C7 dans la table récapitulative.

---

## Étape 7 — Création de compte pour partager avec les joueurs

**UC porteur :** [UC-10](../usecases/UC-10-compte-cloud.md) | **UJ porteur :** [UJ-UC-10](../user-journeys/UJ-UC-10-compte-cloud.md)

### Ce qu'Émilie cherche à faire

Elle veut inviter ses joueurs pour la prochaine session. Elle tente de cliquer sur « Partager » ou « Inviter des joueurs » — l'action est désactivée avec un appel à l'action vers la création de compte (UC-01 règle métier).

### Comportements observables

- Émilie clique sur le CTA « Créer un compte ».
- Elle renseigne email, nom d'affichage, mot de passe. Son compte est créé (tier `FREE`).
- Le **gate de reconnaissance** lui présente les campagnes locales détectées (titres, volume, date) avec leur historique de session (sessions terminées, notes de session, documents épinglés, résumés).
- Si une session est encore en `LIVE`, le gate lui signale qu'elle doit être clôturée avant migration — Émilie a déjà clôturé la session à l'étape 5, cette condition est satisfaite.
- Émilie confirme. La migration démarre, campagne par campagne, tout-ou-rien.
- Après migration : ses campagnes et tout leur historique de session sont disponibles en cloud. La `SessionViewConfig` n'est pas reprise — elle sera reconfigurée.

### État laissé

- Émilie est `User` authentifiée, `AccountTier = FREE`.
- Ses campagnes sont synchronisées en cloud.
- Tout l'historique de session (sessions terminées, notes de session, documents épinglés, résumés) a migré intégralement.
- Partage avec les joueurs (UC-08) et accès joueur (UC-09) sont désormais accessibles.

### Couture vers l'étape 8

UC-08 précondition : le MJ est authentifié et propriétaire de la campagne. Satisfait. UC-09 précondition : compte MJ `FREE` minimum + lien de session généré. Le compte `FREE` existe. Le lien de session sera généré dans l'étape suivante (UC-06 règle métier « Lien de session ponctuel »).

**Couture continue.**

---

## Étape 8 — Session suivante avec partage joueurs

**UC porteurs :** [UC-06](../usecases/UC-06-vue-session.md), [UC-07](../usecases/UC-07-creation-volee-session.md), [UC-08](../usecases/UC-08-partager-information.md), [UC-09](../usecases/UC-09-acces-session-joueur.md)

### Ce qu'Émilie cherche à faire

Même pratique que l'étape 3-4, mais maintenant avec ses joueurs connectés. Elle lance une nouvelle session sans scénario, capture à la volée, et partage certaines informations avec ses joueurs en direct.

### Comportements observables

- Émilie lance une nouvelle session (`Session` créée en `LIVE`).
- Depuis la vue session, elle génère un lien de session ponctuel (UC-06 règle métier « Lien de session ponctuel ») — `GuestAccess` de portée `SESSION`.
- Elle partage ce lien avec ses joueurs (Discord, message).
- Ses joueurs cliquent sur le lien, saisissent un nom d'affichage, accèdent à la vue session joueur (UC-09).
- En session, Émilie capture à la volée comme à l'étape 4.
- Elle peut basculer certaines notes de session en `PUBLIC` pour les rendre visibles aux joueurs — cette fois, les joueurs les voient immédiatement dans leur vue joueur.
- Limite compte `FREE` : 4 joueurs par session maximum (UC-01 modèle d'accès, UC-09 règle métier).

### État laissé

- Une `Session` `LIVE` avec des `GuestAccess` actifs.
- Des `Document` créés à la volée, épinglés dans la session.
- Des `LIVE_NOTE` avec `visibility = PUBLIC` visibles par les joueurs.

---

## Points de friction propres à Émilie

Ces points sont fondés sur le corpus (fiche persona, UJ-UC-06, UJ-UC-07) — aucun n'est inventé.

1. **Zone de saisie des notes de session non visible** : si les panneaux de dossiers occupent tout l'espace visible, la zone de saisie des notes de session est masquée. Émilie n'utilise pas les panneaux — elle cherche directement la zone de notes. Friction documentée dans UJ-UC-06 §Points de friction.

2. **Formulaire de création trop chargé** : si le panneau de création rapide (UC-07) affiche plusieurs champs en dehors du titre, Émilie l'abandonne. Score 2/5 si plusieurs champs sont visibles. Documenté dans UJ-UC-07 §Scénarios par persona — Émilie.

3. **Rangement a posteriori difficile** : les documents créés à la volée atterrissent dans leur dossier d'accueil ou dans « Non classés ». Émilie les retrouve en cherchant, mais sans vue « Récemment créés » ou filtre « Créés en session », le rangement est fastidieux. Documenté dans UJ-UC-07 §Points de friction.

4. **Rejet de l'outil si perçu comme outil de préparation** : la fiche persona indique explicitement qu'Émilie lira « préparer une campagne » et aura l'impression que l'outil n'est pas fait pour elle. La valeur doit lui être prouvée sur la capture en séance.

5. **Sessions en cours au moment de la migration** : si Émilie crée son compte depuis une session ouverte, le gate de reconnaissance lui signale qu'elle doit la clôturer. La clarté de ce message est critique pour éviter une friction à la conversion.

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Mode local → création de campagne | 1 → 2 | **Continue** | UC-02 précondition = application accessible en mode local ou cloud. UC-05 A6 confirme les dossiers système à la création. |
| C2 | Campagne créée → configuration dossiers | 1 → 2 | **Continue** | UC-05 précondition = campagne existante + MJ propriétaire. Satisfait dès la création. |
| C3 | Dossiers configurés → lancement de session | 2 → 3 | **Continue** | UC-06 précondition = campagne existante + MJ authentifié ou en mode local. Les deux chemins sont couverts explicitement. |
| C4 | Session LIVE → création à la volée | 3 → 4 | **Continue** | UC-07 précondition = session en LIVE ou CLOSED + MJ propriétaire ou en mode local. Satisfait. |
| C5 | Session LIVE → clôture | 4 → 5 | **Continue** | UC-06 phase 4 : le MJ clique « Terminer ». La session passe en CLOSED. Règle métier : le MJ peut ajouter des notes rétroactives et créer des éléments à la volée en CLOSED. |
| C6 | Session CLOSED → enrichissement bibliothèque | 5 → 6 | **Continue** | UC-04 précondition = campagne existante + MJ y ayant accès. Satisfait en mode local. Les documents créés à la volée sont accessibles depuis la bibliothèque de contenu. |
| C7 | Enrichissement bibliothèque → partage joueurs | 6 → 7 | **Continue (documentée)** | UC-08 précondition = MJ authentifié (compte). En mode local, aucun `User` n'est instancié. UC-10 s'intercale. L'intercalation est formalisée : UC-10 (déclencheur « Le MJ tente de partager depuis le mode local » + Contexte : parcours du MJ qui a conduit des sessions en mode local, crée son compte, migre avec tout l'historique puis invite ses joueurs), UJ-UC-01 (flux fonctionnel : tentative de partage → appel à la création de compte) et UJ-UC-10 (« Après la migration — Continuité vers le partage »). |
| C8 | Migration cloud → session avec joueurs | 7 → 8 | **Continue** | UC-08 et UC-09 préconditions satisfaites après UC-10. L'historique de session migre intégralement (UC-10 règle métier). La `SessionViewConfig` est recréée. |
| C9 | Capture à la volée → vue joueur (partage) | 4 (mode local) | **Continue (limitation)** | En mode local, les notes basculées en `PUBLIC` ne sont pas visibles par les joueurs — aucun `GuestAccess` ne peut exister (UC-06 règle métier « Mode local et vue joueur », UC-01 règle métier). Cette limitation est documentée dans les deux UC. Elle n'est pas une rupture de spécification mais une limitation portée. |

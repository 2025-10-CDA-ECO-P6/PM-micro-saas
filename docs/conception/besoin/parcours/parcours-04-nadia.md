# Parcours bout-en-bout — Nadia, MJ occasionnelle qui veut lancer vite et retrouver ses affaires

> Objet : vérifier les coutures inter-UC sur le parcours de Nadia, du démarrage sans configuration jusqu'au retour après une longue absence.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict). Termes retenus : `Session`, `LIVE_NOTE`, `SessionViewConfig`, `GM_ONLY`, `GuestAccess`, `Campagne`, Mode local, Gate de reconnaissance.
> → [Index des parcours](README.md)
> → [Fiche persona](../persona/persona-04-nadia.md)

---

## Présentation du parcours

Nadia, 42 ans, infirmière, MJ depuis 15 ans. Elle joue une session par mois quand tout va bien. Sa préparation se fait le dimanche soir, en 30 à 45 minutes. Elle a abandonné Notion parce que l'outil demandait un entretien régulier qu'elle ne pouvait pas maintenir.

Son parcours Haversack se déroule sur deux moments distants dans le temps :

1. **Premier contact** : Nadia découvre l'outil, décide de ne pas créer de compte tout de suite, crée une campagne en mode local, prépare un minimum de contenu, lance une session avec la vue session sans avoir configuré de panneaux.
2. **Retour après six semaines** : Nadia revient. Elle ne se souvient plus de l'emplacement de ses notes. Elle utilise la recherche pour se réorienter, puis relance une session.

Le parcours s'arrête au moment où Nadia décide — ou non — de créer un compte pour partager avec ses joueurs. Cette décision n'est pas l'objet du présent parcours ; elle est documentée dans le parcours Thomas (migration locale→cloud).

---

## Étape 1 — Démarrage sans compte, création d'une campagne en mode local

**UC porteur :** [UC-01](../usecases/UC-01-mode-local-sans-compte.md) | **UJ porteur :** [UJ-UC-01](../user-journeys/UJ-UC-01-mode-local-sans-compte.md)

### Ce que Nadia cherche à faire

Ouvrir l'application, ne pas remplir de formulaire d'inscription, créer une campagne utilisable immédiatement. Sa question est : "combien de temps pour que ça soit utilisable ?"

### Comportements observables

- L'écran d'accueil présente deux options équivalentes sans hiérarchie culpabilisante : "Commencer sans compte" et "Créer un compte / Se connecter".
- Nadia choisit "Commencer sans compte". Un message court — non bloquant — explique que les données seront stockées dans ce navigateur. Elle est redirigée vers la création de campagne.
- Elle saisit un nom, la campagne est créée. Les quatre dossiers système sont présents (`Personnages`, `Joueurs`, `Scénarios`, `Notes`) — point de départ neutre, aucun contenu à remplir pour que la campagne soit créée.
- Deux bandeaux distincts et non bloquants apparaissent selon les conditions : bandeau de durabilité (si le navigateur ne garantit pas la conservation permanente des données) et bandeau de confidentialité (systématique — données lisibles par toute personne ayant accès à ce navigateur).
- Les fonctionnalités de partage joueurs sont visibles mais désactivées, avec un appel à l'action discret.

### État laissé par l'étape 1

- Nadia est en mode local : aucun `User` instancié, aucune donnée envoyée au serveur.
- Une `Campagne` existe avec ses dossiers système, persistée dans le stockage local du navigateur.
- Une `SessionViewConfig` a été créée automatiquement à `CampaignCreated` — ses `focusedFolders` ne sont pas encore configurés.

### Couture vers l'étape 2

UC-01 postcondition : "Le MJ peut utiliser toutes les fonctionnalités de préparation et de session sans compte." UC-01 scénario nominal (step 6) : "Il utilise l'application normalement (UC-02 à UC-06, UC-14)." La précondition de UC-06 est : "Une campagne existe et le MJ y a accès" et "Le MJ est authentifié **ou** en mode local sans compte." Les deux conditions sont satisfaites. **Couture continue.** Note : UC-12 (vue joueur) n'est pas disponible en mode local — il requiert un `GuestAccess` ou un `SpaceMembership` actif, qui présupposent un compte.

---

## Étape 2 — Préparation minimale : quelques documents, sans structure elaborate

**UC porteurs :** [UC-04](../usecases/UC-04-gerer-documents-campagne.md), [UC-05](../usecases/UC-05-organiser-dossiers.md)

### Ce que Nadia cherche à faire

Créer quelques fiches — ses PNJ importants, une ou deux notes de contexte — sans investir du temps dans une arborescence complexe. Elle sait que l'outil doit lui être utile dès ce soir, avec très peu de contenu.

### Comportements observables

- Nadia crée des `Document` dans les dossiers par défaut. Chaque document est créé avec `visibility = GM_ONLY` par défaut.
- Elle peut renommer un dossier si elle le souhaite, ou laisser la structure par défaut intacte. Les dossiers système sont renommables et supprimables librement.
- Elle n'est pas obligée de créer un scénario structuré : une note libre de préparation dans le dossier Notes suffit.
- Tout le contenu est accessible via la recherche dès sa création (UC-14).

### État laissé par l'étape 2

- La `Campagne` contient des `Document` en `GM_ONLY`, organisés dans les dossiers par défaut ou renommés.
- La `SessionViewConfig` existe toujours sans `focusedFolders` configurés.

### Couture vers l'étape 3

UC-06 précondition : "Une campagne existe et le MJ y a accès." Satisfaite. UC-06 précise également que si la `SessionViewConfig` n'a pas de dossiers configurés, "les dossiers par défaut s'affichent et un message l'invite à configurer les panneaux" (UJ-UC-06, scénario Nadia). **Couture continue.**

---

## Étape 3 — Lancement de la session avec configuration minimale

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) | **UJ porteur :** [UJ-UC-06](../user-journeys/UJ-UC-06-vue-session.md)

### Ce que Nadia cherche à faire

Lancer une session en moins de 30 secondes, sans avoir configuré de panneaux à l'avance, et trouver ses PNJ rapidement pendant la partie.

### Comportements observables

- Nadia clique "Lancer une session", saisit un titre, ignore ou sélectionne un scénario. La `Session` est créée directement en `LIVE`.
- La vue session s'ouvre avec les dossiers par défaut de la campagne dans les panneaux — correspondant à ce que la `SessionViewConfig` expose sans configuration préalable. Un message invite à configurer les panneaux.
- Nadia navigue dans ses dossiers pour retrouver un PNJ, l'épingle dans `pinnedDocumentIds` pour un accès rapide le reste de la session.
- Elle crée des notes de session au fil de la partie (`LIVE_NOTE`, `visibility = GM_ONLY` par défaut).
- En fin de soirée, elle clique "Terminer la session". La `Session` passe en `CLOSED`.

### Comportement important pour Nadia

La vue session MJ est disponible en mode local, sans compte, pour l'intégralité des fonctionnalités MJ (notes, épingles, recherche, création à la volée). Les fonctionnalités joueurs (vue session joueur, partage, `GuestAccess`) sont indisponibles en mode local — la session sert à Nadia seule. Nadia ne peut pas partager avec ses joueurs sans créer un compte.

### État laissé par l'étape 3

- Une `Session` en état `CLOSED` existe dans la campagne.
- Des `LIVE_NOTE` ont été créées, toutes `GM_ONLY`, référencées par `sessionNoteIds`.
- Des documents sont dans `pinnedDocumentIds` de cette session.
- Nadia ferme le navigateur.

### Couture vers l'étape 4

L'étape 4 se produit six semaines plus tard. La précondition pour retrouver ses données est que le stockage local n'ait pas été vidé. UC-01 A2 : "Le MJ ferme le navigateur puis revient sur l'application. L'application récupère les données depuis le stockage local du navigateur. Le MJ retrouve ses campagnes et documents intacts." Les données sont "persistantes entre les sessions navigateur (jusqu'à vidage du cache)" (UC-01 postconditions). **Couture continue sous la condition que les données du navigateur n'aient pas été effacées.** Si elles l'ont été, UC-01 A3 s'applique (message distinct d'une première visite). Cette condition est explicitement communiquée à Nadia via le bandeau de durabilité de l'étape 1.

---

## Étape 4 — Retour après six semaines : retrouver où on en était

**UC porteurs :** [UC-01](../usecases/UC-01-mode-local-sans-compte.md) (A2), [UC-14](../usecases/UC-14-recherche.md) | **UJ porteur :** [UJ-UC-14](../user-journeys/UJ-UC-14-recherche.md)

### Ce que Nadia cherche à faire

Rouvrir l'application six semaines après sa dernière session et retrouver ses informations sans relire 40 pages de notes. Elle ne se souvient plus de l'emplacement exact de ses documents.

### Comportements observables

- L'application récupère les données du stockage local. Nadia retrouve ses campagnes intactes, dont la campagne avec sa session `CLOSED` et ses `LIVE_NOTE`.
- UC-14 persona central (documentation) : "Après plusieurs semaines d'absence, elle ne se souvient plus où est rangée une information. La recherche est son point d'entrée principal dans le contenu."
- Nadia utilise la barre de recherche. Dans le MVP, la recherche porte sur le titre des documents. Elle peut filtrer par type de document.
- Les résultats sont regroupés par type (PNJ, notes, scénarios, notes de session). Elle retrouve ses PNJ et ses notes sans naviguer dans l'arborescence des dossiers.
- Elle peut aussi accéder à la session `CLOSED` de la séance précédente pour consulter ses `LIVE_NOTE` MJ et le résumé si elle en a rédigé un.

### Limite de la recherche MVP

La recherche porte sur le titre des documents uniquement — aucune indexation du contenu des blocs (UC-14 scénario nominal, step 2). Pour Nadia, cela signifie que si ses notes ont des titres peu descriptifs ("Note 1", "PNJ mystère"), la recherche est peu efficace. Ce point est un risque de friction identifié dans UJ-UC-14 : "état vide peu informatif" et "résultats trop nombreux sans filtre".

### État laissé par l'étape 4

- Nadia a retrouvé ses documents. Elle est prête à relancer une session.
- Elle est toujours en mode local.

### Couture vers l'étape 5

La précondition de UC-06 pour relancer une session est identique à celle de l'étape 3 : "Une campagne existe, le MJ y a accès, en mode local ou authentifié." Satisfaite. **Couture continue.**

---

## Étape 5 — Deuxième session : lancement depuis l'état précédent

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md)

### Ce que Nadia cherche à faire

Relancer une session depuis la campagne déjà constituée, bénéficier cette fois de sa préparation du mois précédent.

### Comportements observables

- Nadia clique "Lancer une session". La nouvelle `Session` est créée directement en `LIVE`.
- La `SessionViewConfig` est toujours celle qu'elle a éventuellement ajustée lors de la première session. Si elle n'a pas configuré les panneaux, les dossiers par défaut s'affichent à nouveau.
- Ses `Document` préparés lors de l'étape 2 sont disponibles. Ses `LIVE_NOTE` de la session précédente sont accessibles depuis la session `CLOSED` dans la liste des sessions de la campagne.
- Il ne peut y avoir qu'une seule `Session` en `LIVE` par campagne à la fois.

### État laissé par l'étape 5

- Une nouvelle `Session` en `LIVE` existe.
- La campagne contient désormais deux sessions : une `CLOSED` (séance 1) et une `LIVE` (séance 2).

---

## Points de friction propres à Nadia

Les points suivants sont fondés sur le corpus lu (UC, UJ, persona).

**Note de frontière (hors profil Nadia) :** Le cadrage one-shot — parcours express, type `ONE_SHOT`, campagne à session unique — concerne le persona Sonia (persona-07), dont c'est le cœur de pratique. Nadia est une MJ de campagne dont les sessions sont espacées sur la même continuité narrative ; la problématique one-shot ne lui appartient pas.

**1. Configuration des panneaux : travail différé ou friction en session**
Si Nadia ne configure pas la `SessionViewConfig` avant sa session, les panneaux affichent les dossiers par défaut et un message d'invitation à configurer. Pour Nadia qui veut "lancer vite", ce message est une friction non bloquante mais réelle. La configuration des panneaux est faisable en direct pendant la session `LIVE` sans l'interrompre (UC-06 A2, UJ-UC-06 : "Modifier `SessionViewConfig` en direct"), mais cela coûte du temps qu'elle n'a pas.

**2. Recherche MVP limitée aux titres**
La recherche du MVP porte uniquement sur les titres des documents (UC-14 scénario nominal). Nadia qui revient après six semaines est documentée comme le persona central de UC-14. Si ses documents ont des titres peu mémorables, la recherche ne l'aide pas. Ce point est un risque identifié dans UJ-UC-14 ("état vide peu informatif") et non résolu dans le périmètre MVP.

**3. Risque de perte de données en mode local**
Nadia n'est pas sensible au risque de perte de données immédiate (elle crée une session par mois). Mais après six semaines sans ouvrir le navigateur, le stockage local peut être vidé par le navigateur. Le bandeau de durabilité l'a prévenue. UC-01 A3 couvre le cas ("données introuvables — cache vidé") mais la récupération est impossible sans export préalable (et l'export de paramètres est lui-même Should Have).

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Mode local → création de campagne | 1 → 2 | **Continue** | UC-01 postcondition couvre explicitement l'accès à UC-02 à UC-06 en mode local. Aucune précondition de compte requise pour la création de contenu. |
| C2 | Campagne locale → lancement de session sans panneaux configurés | 2 → 3 | **Continue** | UC-06 précondition : campagne existante + MJ en mode local. Deux conditions satisfaites. L'absence de `focusedFolders` est un cas couvert : la vue session s'ouvre avec les dossiers par défaut et un message d'invitation à configurer (UC-06 scénario nominal phase 4, UJ-UC-06 scénario Nadia). |
| C3 | Session CLOSED → retour après six semaines | 3 → 4 | **Continue (conditionnelle)** | UC-01 A2 couvre le retour après fermeture du navigateur. La continuité est conditionnée par la conservation des données du navigateur. Cette condition est communiquée via le bandeau de durabilité. Si les données ont été effacées, UC-01 A3 s'applique (rupture de données, non de parcours). Le parcours reste continu tant que les données sont présentes. |
| C4 | Retrouvabilité post-absence → recherche par titre | 4 | **Continue (limitation documentée)** | UC-14 précondition : campagne existante avec données recherchables. Satisfaite. La limitation MVP (recherche sur titre uniquement) est documentée dans UC-14 scénario nominal et constitue un risque pour Nadia si ses titres sont peu descriptifs. Ce n'est pas une rupture mais une zone de friction avérée. |
| C5 | Données retrouvées → relancement de session | 4 → 5 | **Continue** | Même précondition que C2. La `SessionViewConfig` est persistée entre sessions (UJ-UC-06, opportunité UX "persistée entre deux sessions"). La nouvelle `Session` est créée en `LIVE` depuis la campagne existante. |
| — | *(Hors profil Nadia)* Le cadrage one-shot et le type `ONE_SHOT` sont documentés dans le parcours Sonia (parcours-07-sonia.md), dont c'est le cœur. Ce parcours ne couvre pas cette frontière. | — | — | — |

# Parcours bout-en-bout — Rémi, le MJ papier (hors cible MVP)

> Objet : vérifier la frontière du périmètre produit sur le parcours de Rémi — jusqu'où le MVP
> peut l'atteindre, et à quel point exact il s'arrête.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict).
> Index des parcours → [README](README.md)
> Fiche persona → [persona-06-remi.md](../persona/persona-06-remi.md)

---

## Présentation

Rémi, 48 ans, mène ses sessions depuis 25 ans avec un carnet Leuchtturm, des fiches Bristol
et des stylos à encre. Son refus du numérique est un choix philosophique — la mouvance OSR
qu'il pratique (Old School Essentials, Pendragon) valorise la légèreté des outils et le rejet
des couches d'abstraction. Il utilise un ordinateur tous les jours ; ce n'est pas une question
de compétence.

Son groupe ne sort pas les téléphones à table. Il admet un point de friction concret : retrouver
une information dans ses anciens carnets prend du temps, et ses joueurs aimeraient parfois un
résumé de session. Mais ces frustrations ne le motivent pas à changer.

Ce parcours est documenté bien que Rémi soit hors cible MVP. La frontière du périmètre se
vérifie ici : le mode local sans compte supprime la friction d'entrée numérique, ce qui constitue
la seule fenêtre par laquelle le produit peut l'atteindre. La limite au-delà de cette fenêtre
est tracée explicitement dans le corpus et mérite d'être lue sur pièces.

---

## Étape 1 — Ouvrir l'application sans compte

**UC porteur :** UC-01 | **UJ porteur :** UJ-UC-01

### Ce que Rémi fait

Rémi entend parler de Haversack — peut-être par un joueur, peut-être par curiosité. Il ouvre
l'application. L'écran d'accueil lui présente deux options équivalentes : « Commencer sans
compte » et « Créer un compte / Se connecter ». Il choisit la première.

### Ce que Rémi obtient (comportements observables)

- Aucun formulaire d'inscription, aucun email demandé.
- Un message court lui explique que les données seront stockées dans le navigateur. Non bloquant.
- Deux bandeaux non bloquants apparaissent : un bandeau de durabilité et un bandeau de
  confidentialité (UC-01 règles métier).
- Les fonctionnalités de partage (UC-08) et d'accès joueur (UC-09) sont visibles mais
  désactivées, avec un appel à l'action vers la création de compte (UC-01 A1, RB-01-06).
- Il atterrit dans son **espace personnel** — le conteneur par défaut disponible dès le mode
  local (UC-01 scénario nominal étape 5), sans qu'aucune campagne n'existe encore. Il pourrait y
  écrire directement, sans structure de campagne imposée — ce qui rejoint directement sa
  question sur la page vierge (fiche persona §Ce qu'il demande à l'app : « En quoi c'est mieux
  qu'une page vierge ? »). Il choisit ensuite de créer un espace de jeu pour explorer la
  structure documentaire, un acte distinct et optionnel (UC-01 §Contexte, UC-02).

### Ce que révèle la fiche persona sur cette étape

UJ-UC-01 évalue l'ouverture de l'application à 2/5 pour Rémi (« méfiance initiale, interface
perçue comme complexe »). La friction est présente dès l'entrée. Le choix « Commencer sans
compte » est une condition nécessaire mais pas suffisante pour capter un MJ résistant au
numérique par conviction.

### État laissé par l'étape 1

- Rémi est en mode local : aucun `User` instancié, aucune donnée envoyée au serveur.
- Un **espace personnel** (`SpaceType.PERSONAL`) existe déjà comme conteneur par défaut, avec
  son seul dossier virtuel « Non classés » — Rémi en est le propriétaire.
- Une session navigateur locale existe.

### Couture vers l'étape 2

UC-02 précondition : « Le MJ peut créer un espace de jeu en mode local (sans compte) ou depuis
un compte cloud. Les préconditions sont identiques dans les deux cas. » **Couture continue.**

---

## Étape 2 — Explorer la structure documentaire

**UC porteurs :** UC-02, UC-04, UC-05 | **UJ porteurs :** UJ-UC-02, UJ-UC-04, UJ-UC-05

### Ce que Rémi fait (hypothèse d'exploration)

Rémi crée une campagne. Il découvre les quatre dossiers système générés automatiquement
(« Personnages », « Joueurs », « Scénarios », « Notes »). Il constate que ces dossiers sont
renommables et supprimables librement — `isSystem` est informatif, non restrictif (UC-05 base,
glossaire §3 `Folder`).

Il crée un ou deux documents libres. Le modèle de blocs génériques lui permet de saisir du texte
libre sans structure imposée. Il peut lier des documents entre eux (`DocumentLink`).

### Ce que Rémi obtient (comportements observables)

- Une campagne avec une arborescence modifiable, persistée dans le navigateur.
- Des `Document` créés avec `visibility = GM_ONLY` par défaut.
- Un espace de travail dont la structure n'est pas plus contrainte qu'une page vierge — si tant
  est que l'interface elle-même ne crée pas d'obstacle perceptuel.

### Le point de friction central de Rémi

La fiche persona pose une question directe au produit : « Le modèle de blocs est-il aussi libre
qu'un carnet, ou y a-t-il une structure implicite qui contraint la pensée ? » Cette question
ne reçoit pas de réponse dans le corpus — elle devra être vérifiée en entretien utilisateur.
Le corpus ne contient pas de spécification de l'interface de saisie au niveau où Rémi formule
sa résistance.

Par ailleurs, la fiche persona pose : « L'app peut-elle être utile en usage très partiel,
seulement pour le partage joueurs, sans reconstruire toute son organisation de campagne dedans ? »
Cette question reçoit une réponse dans le corpus (UC-08, section suivante) — mais la réponse
inclut un prérequis qui constitue la limite tracée.

### État laissé par l'étape 2

- Une `Campagne` existe en mode local avec ses `Folder` et ses `Document`.
- Rémi est toujours en mode local, sans `User`.

### Couture vers l'étape 3

UC-06 précondition : « Une campagne existe et le MJ y a accès. Le MJ est authentifié ou en
mode local sans compte. La vue session MJ est disponible dans les deux cas. »
**Couture continue.**

---

## Étape 3 — Utiliser la vue session en mode local

**UC porteur :** UC-06 | **UJ porteur :** UJ-UC-06

### Ce que Rémi peut faire

Rémi lance une session depuis sa campagne. La `Session` est créée directement en `LIVE`. La vue
session MJ s'ouvre — tableau de bord configurable, panneaux dossiers, prise de notes de session.

Il peut naviguer dans ses documents, épingler des éléments (`pinnedDocumentIds`), prendre des
notes de session avec `visibility = GM_ONLY` par défaut. En fin de session, il ferme (`CLOSED`),
peut ajouter un résumé (`summary`), puis archiver (`ARCHIVED`).

### Comportements observables

- Vue session MJ fonctionnelle en mode local — panneaux configurables, notes de session privées
  MJ, épingles, recherche globale (UC-06 précondition + UC-14 Should Have).
- Cycle de vie `Session` : `LIVE → CLOSED → ARCHIVED`.
- En mode local : pas de vue joueur, pas de partage temps réel — UC-01 règles métier le précise
  explicitement.

### L'arrêt : ce que Rémi veut et ne peut pas obtenir en mode local

Ce que Rémi a identifié lui-même comme seul usage envisageable du numérique : « un résumé de
session consultable par les joueurs, si c'est moins de 5 minutes de saisie après la partie »
(fiche persona §Ce qu'il utiliserait).

La vue session en mode local permet de saisir ce résumé (`summary` en `CLOSED`). Mais la
consultation par les joueurs — l'acte de partage — est bloquée en mode local. UC-01 règles
métier : « Les fonctionnalités de partage (UC-08) et d'accès joueur (UC-09) nécessitent au
minimum un compte gratuit. » UJ-UC-01 représente explicitement ce moment : « Tenter de partager
avec les joueurs → Bouton visible mais désactivé — CTA compte ».

Rémi voit le bouton. Il ne peut pas l'utiliser sans créer un compte. C'est ici que le produit
s'arrête pour lui dans le scénario nominal.

### Couture vers l'étape suivante (limite atteinte)

Le corpus ne documente pas de parcours Rémi au-delà de ce point dans le périmètre MVP. UJ-UC-01
positionne Rémi dans la section « Conversion » avec l'entrée : « Quitter sans créer de compte :
3/5 ». La décision la plus probable selon le corpus est donc le départ sans compte.

---

## L'arrêt — Rémi face à la création de compte

Rémi atteint le bouton de partage et constate qu'il est désactivé. Le CTA lui propose de créer
un compte. C'est le point de décision terminal de son parcours MVP.

La fiche persona éclaire ce qui se passe ici : Rémi n'est pas résistant par incompétence, mais
par conviction. Créer un compte pour partager un résumé représente exactement le type d'engagement
numérique qu'il rejette. Son groupe ne sort pas les téléphones à table — demander à ses joueurs
de suivre un lien de session, saisir un nom d'affichage, accéder à une vue joueur numérique,
c'est un changement de pratique de table qu'il n'a pas signalé vouloir.

La moscow.md le formule sans détour : « Rémi — résistant au numérique par philosophie — ne
considère Haversack que si le partage est simple » (UC-08 §Pourquoi Must Have). Le partage est
simple dans le produit — mais il nécessite un compte, ce qui constitue la barrière.

---

## Limite tracée et condition de retour

### Limite tracée

**Rémi est hors cible MVP.** La fiche persona conclut sans ambiguïté : « Rémi n'est probablement
pas la cible de Haversack. C'est un choix à assumer clairement. »

La moscow.md §UC-01 nomme Rémi parmi les profils que le mode local vise (« Rémi ne créera pas
de compte sans raison concrète ») — ce qui positionne le mode local sans compte comme fenêtre
d'évaluation, pas comme périmètre d'usage suffisant. La vision produit §2.2 le cite également
comme profil bénéficiant de la réduction de friction d'onboarding : « pour les profils les plus
résistants (Nadia, Rémi) ». Cette réduction de friction est réelle ; elle ne suffit pas si la
valeur perçue avant le prérequis compte ne justifie pas la création de compte.

### Condition de retour

Aucune condition de retour explicite n'est tracée dans le corpus pour Rémi spécifiquement.

La vision produit §5bis cite une condition générique pour l'export : « l'export matérialise la
promesse de possession des données » et répond à la « douleur de confiance de Thomas (crainte
de l'enfermement propriétaire) et de Rémi (réassurance face au numérique) ». L'export de campagne
(Should Have) est donc tracé comme réassurance pour Rémi, mais dans le registre de la confiance
— pas comme déclencheur de conversion.

Aucun texte du corpus ne définit une condition d'événement (signal d'usage, signal d'entretien)
qui ferait de Rémi une cible réexaminée. Il n'existe pas d'équivalent de la condition de retour
Sonia (§5bis vision produit, libellé exact cité dans le parcours Sonia).

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Démarrage sans compte → exploration de l'interface | Entrée → étape 1 | Continue | UC-01 : aucune précondition ; option « Commencer sans compte » disponible à l'écran d'accueil. Friction perceptuelle documentée dans UJ-UC-01 (score 2/5 à l'ouverture). |
| C2 | Mode local → création de campagne | Étape 1 → étape 2 | Continue | UC-02 : précondition identique en mode local et mode cloud. Aucune restriction. |
| C3 | Campagne locale → vue session MJ | Étape 2 → étape 3 | Continue | UC-06 précondition : campagne existante + MJ authentifié ou en mode local. Explicitement couvert. |
| C4 | Vue session MJ → partage du résumé aux joueurs | Étape 3 → partage | Limite assumée | UC-01 règles métier + UJ-UC-01 flux fonctionnel : partage joueurs désactivé en mode local. Bouton visible, non actionnable sans compte. C'est le seul usage envisagé par Rémi — c'est la frontière du périmètre. |
| C5 | Création de compte → accès au partage | Hors parcours nominal Rémi | Limite assumée | Rémi n'a pas de parcours post-création de compte spécifique documenté. Si Rémi crée malgré tout un compte, il suit le parcours GÉNÉRIQUE de migration locale→cloud (UC-10, déjà instancié de bout en bout par le parcours Thomas). Aucun besoin Rémi-spécifique n'est requis au-delà de ce chemin partagé. Le scénario nominal de Rémi reste le départ sans compte (UJ-UC-01 : « Quitter sans créer de compte » 3/5). **Justification** : Rémi est explicitement hors cible MVP (persona et moscow.md sans ambiguïté). La limite assumée nomme : si Rémi crée un compte, le comportement est défini (UC-10 générique) ; aucun parcours spécialisé Rémi n'est requis, par conception produit, pas par trou de spécification. |

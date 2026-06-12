# Glossaire — Langage ubiquitaire Haversack

> Version : 2026-06-10
> Nature : outil de nommage dérivé — **autorité de forme**, pas d'autorité de fond.
> Ordre d'autorité (du plus faisant foi au moins) : personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire.
> En cas de conflit sur le fond (le besoin), l'artefact le plus en amont fait foi. Le glossaire n'arbitre jamais un conflit de besoin. Une entrée de glossaire qui contredit un use case est l'entrée à corriger, pas le use case.

---

## Préambule

Ce glossaire est la **source de référence terminologique** de la conception Haversack. Son rôle est double :

1. **Nommer sans ambiguïté** : chaque concept du domaine a un terme retenu, une définition en langage besoin et, quand il existe, son identifiant de domaine (`CamelCase` ou `SCREAMING_SNAKE`). Tout nouvel artefact (UC, US, UJ, diagramme) doit employer ces termes.

2. **Détecter les dérives** : un terme absent du glossaire employé dans un artefact est un signal d'enrichissement à instruire. Un terme divergent (synonyme non tracé, reformulation silencieuse) est un signal de consolidation à traiter.

**Comment enrichir le glossaire** : ouvrir une issue ou une PR ciblée sur ce fichier. Proposer le terme retenu, sa définition et, si applicable, la ligne de l'artefact de besoin qui l'introduit. La décision terminologique est arrêtée au niveau du besoin (use cases, user stories) et se reflète ensuite dans le domaine puis dans le glossaire.

**Périmètre** : vocabulaire de conception pure. Aucun nom de technologie, d'infrastructure, d'API ou d'outil n'a sa place dans ce glossaire. Les identifiants de domaine (`PLAYER_PRIVATE`, `GuestAccess`, `LIVE_NOTE`…) sont du vocabulaire de domaine légitime — ils ne désignent pas des choix techniques.

---

## 1. Termes transverses (Core / Shared Kernel)

### `Document`

Unité fondamentale de contenu dans Haversack. Tout contenu créé dans le système — note, scénario, scène, PNJ, lieu, objet, personnage joueur, règle maison, révélation — est un `Document`. Un `Document` est composé de blocs de contenu libre (`DocumentBlock[]`) et de références ordonnées vers d'autres `Document` (`DocumentLink[]`). Le type du document (`documentTypeId`) est optionnel et spécialise sans contraindre.

- Propriétaire : **Content Library** (creation, stockage, visibilité).
- Consommé par : **Session Conduct** (épinglage, notes de session).
- Relation clé : un `Document` appartient toujours à exactement un `Folder` dans exactement une `Campagne`.

> Gouvernance « tout est Document » : il n'existe pas de type « Scénario » ou « Scène » séparé du système documentaire. La hiérarchie Scénario → Scènes → PNJ est un cas d'usage parmi d'autres, pas une structure imposée.

---

### `Visibility` (niveau de visibilité)

Propriété portée par chaque `Document`. Détermine qui peut lire le document. Trois valeurs exhaustives :

| Valeur domaine | Sémantique |
|---|---|
| `PUBLIC` | Visible par tous les membres de la campagne et tous les `GuestAccess` actifs. |
| `GM_ONLY` | Visible uniquement par les membres avec le rôle `OWNER` ou `GM`. C'est la valeur par défaut à la création. |
| `PLAYER_PRIVATE` | Lisible uniquement par l'auteur du document : le membre identifié par `createdById`, ou l'invité identifié par `guestAccessId`. Les membres `OWNER` et `GM` n'y ont aucun accès — ni lecture, ni énumération, ni métadonnées. |

La confidentialité `PLAYER_PRIVATE` est absolue quel que soit le chemin d'accès (direct, via personnage associé, ou via accès invité).

- Défini dans le **Core** comme enum partagé `Visibility`.
- Utilisé dans : **Content Library** (tous les documents), **Session Conduct** (notes de session).

---

### `AuditInfo`

Primitive de traçabilité portée par tous les agrégats : `createdAt`, `updatedAt`, `createdById: UserId`. Défini dans le **Core**.

---

### `SoftDelete`

Suppression logique : `isDeleted: bool`, `deletedAt: DateTime?`. Un document soft-deleted disparaît des vues mais n'est pas effacé. Défini dans le **Core**, utilisé dans **Content Library** et **Campaign Management**.

---

## 2. Identity & Access

### `User`

Compte utilisateur authentifié. Porte l'identité (`email`, `displayName`), le statut du compte (`AccountStatus`) et le tier d'abonnement (`AccountTier`). `User` ne porte aucun rôle métier global — le rôle MJ ou Joueur est défini dans chaque campagne par **Campaign Management**.

- Agrégat unique de **Identity & Access**.
- Relation clé : le `UserId` est l'identifiant partagé avec tous les autres contextes ; ils le consomment sans importer l'entité `User`.

---

### `AccountStatus`

État du compte : `ACTIVE` (connexion autorisée), `SUSPENDED` (connexion bloquée temporairement), `DELETED` (compte supprimé, données nominatives anonymisées, connexion bloquée).

---

### `AccountTier`

Niveau d'abonnement du compte :

| Valeur | Description |
|---|---|
| `FREE` | Compte gratuit — 3 campagnes cloud, 4 joueurs par session, 500 Mo. Tier initial à la création. |
| `PRO` | Abonnement payant — campagnes illimitées, joueurs illimités, 5 Go+. |

Le **mode local** (sans compte) n'est pas un tier. Il n'y a pas de `User` en mode local.

---

### `displayName`

Nom d'affichage choisi par l'utilisateur lors de son inscription. Visible par les autres membres de la campagne. Modifiable à tout moment. Maximum 100 caractères, ne peut pas être vide.

---

### Mode local (sans compte)

Mode d'utilisation de l'application sans création de compte. Les données sont stockées localement dans le navigateur. Aucune donnée n'est envoyée au serveur. Les fonctionnalités de partage avec les joueurs et de synchronisation multi-device sont indisponibles en mode local. Ce mode n'instancie aucun `User`.

- Distinct du tier `FREE` : le mode local précède la création de compte.
- Défini dans : **UC-01**, **US-UC-01**.

---

### Gate de reconnaissance

Mécanisme de confirmation explicite présenté lors de la migration locale→cloud. Lorsqu'un utilisateur crée un compte depuis le mode local, le système présente les données locales détectées (titres des campagnes, volume, date) et exige une confirmation avant d'importer. La migration ne commence qu'après cette confirmation. Ce gate protège contre l'appropriation accidentelle de données d'un tiers.

- Défini dans : **UC-01 A1**, **UC-10**, **US-UC-01**, **US-UC-10**.

---

### Migration locale→cloud

Opération applicative qui importe les campagnes et documents du mode local vers le compte cloud nouvellement créé. La migration est traitée campagne par campagne, tout-ou-rien par campagne. En cas d'échec d'une campagne, les données locales de cette campagne sont conservées intégralement.

---

## 3. Campaign Management

### `Campagne`

Espace de jeu partagé entre un MJ et ses joueurs. Représente indifféremment une campagne longue ou un one-shot — la différence est portée par `CampaignType`. Une `Campagne` a toujours exactement un propriétaire (`OWNER`) et peut avoir plusieurs `CampaignMembership` actifs. Elle est le conteneur de tous les `Document` et `Folder` de jeu.

- Agrégat principal de **Campaign Management**.
- Relation clé : un `Folder` et un `Document` appartiennent toujours à exactement une `Campagne`.

---

### `CampaignType`

Type de l'espace de jeu : `CAMPAIGN` (campagne longue, plusieurs sessions attendues) ou `ONE_SHOT` (session unique attendue, membres permanents optionnels). Les différences entre les deux types sont comportementales, pas structurelles — `ONE_SHOT` est un `Campaign` avec `type = ONE_SHOT`.

En première livraison (MVP), aucune différence de comportement n'est implémentée : création, structure des dossiers, cycle de session et vue session sont identiques pour les deux types. Le tag `type` distingue les deux cas, qui convergent structurellement. Les différences comportementales cibles — parcours de création simplifié, point d'entrée distinct — sont des caractéristiques post-MVP couvertes par l'arbitrage UC-13 (vision-produit §5bis).

---

### `CampaignStatus`

État de la campagne : `ACTIVE` (opérationnelle), `ARCHIVED` (archivée manuellement par le MJ, lecture seule, irréversible dans le MVP), `FROZEN` (gelée automatiquement lors d'un downgrade de tier, lecture seule jusqu'à `Unfreeze()`).

> La valeur `FROZEN` et le mécanisme de downgrade de tier ne sont pas spécifiés dans le MVP. Voir **UC-HORS-MVP — Gel de campagnes au downgrade de tier (post-MVP)** pour la question ouverte délimitée.

---

### MJ (Maître du Jeu)

Rôle dans une campagne. Dans le modèle de domaine, le MJ est le membre avec le rôle `OWNER` ou `GM` dans la campagne. Ce rôle est contextuel : un même `User` peut être MJ dans une campagne et Joueur dans une autre. Le terme « MJ » est employé dans les UC et US comme raccourci de la combinaison `OWNER | GM`.

- Voir aussi : `MemberRole`.

---

### Joueur

Rôle dans une campagne. Membre avec le rôle `PLAYER`. Accède aux contenus partagés (`PUBLIC`) et à sa propre fiche de personnage. Peut créer des notes de session personnelles pendant une session. Peut être un `User` authentifié (via `CampaignMembership`) ou un invité sans compte (via `GuestAccess`).

---

### `MemberRole`

Rôle d'un membre dans une campagne : `OWNER` (propriétaire unique, responsabilité billing et RGPD), `GM` (co-maître du jeu, sans pouvoir de suppression de la campagne ni de gestion des autres GMs), `PLAYER` (joueur).

---

### Membre

`CampaignMembership` avec `status = ACTIVE`. Dans tous les UC et US, « membre » désigne cette entité dans cet état. `CampaignMembership` est le terme technique interne ; « membre » est le terme du langage ubiquitaire.

---

### `CampaignMembership`

Entité (enfant de `Campaign`) représentant la participation d'un `User` à une campagne. Porte le `MemberRole`, le statut (`PENDING`, `ACTIVE`, `REMOVED`) et les références aux personnages associés.

---

### `Invitation`

Mécanisme d'entrée dans une campagne. Peut être de type `LINK` (lien partageable) ou `EMAIL` (hors MVP). De portée `CAMPAIGN` (accès permanent) ou `SESSION` (accès ponctuel). L'utilisation d'une invitation par un utilisateur authentifié crée un `CampaignMembership` ; par un utilisateur anonyme, crée un `GuestAccess`.

---

### `GuestAccess`

Accès d'un joueur sans compte à une campagne ou une session. Agrégat indépendant de `Campaign`. Le joueur y accède par un lien d'accès unique partagé par le MJ, et saisit uniquement un nom d'affichage (`displayName`) à l'arrivée. Un `GuestAccess` de portée `SESSION` expire à la fermeture de la session + 24 heures. Il peut être converti en `CampaignMembership` lors de la création de compte par l'invité.

- Statuts : `ACTIVE`, `EXPIRED`, `REVOKED`, `CONVERTED`.
- Portée (`GuestAccessScope`) : `SESSION` ou `CAMPAIGN`.
- Propriétaire : **Campaign Management**.
- Consommé par : **Session Conduct** (autorisation d'accès), **Content Library** (auteur invité d'une `LIVE_NOTE` via `guestAccessId`).

---

### `ScenarioLibrary`

Bibliothèque personnelle de scénarios réutilisables au niveau du compte MJ, cross-campagne. Agrégat de **Campaign Management** (décision retenue dans `content-library.md` §Concepts en attente). Entrée de type `ScenarioLibraryEntry(userId, documentId, promotedAt)`. Couvert par **UC-13** (Should Have — post-MVP).

---

## 4. Content Library

### `Folder` (dossier)

Conteneur organisationnel. Structure l'arborescence du contenu dans une campagne. Un `Folder` appartient toujours à exactement une `Campagne`. Les dossiers système (`isSystem = true`) sont créés automatiquement à la création d'une campagne ; ils sont renommables et supprimables librement — `isSystem` est informatif, pas restrictif.

Le dossier virtuel « Non classés » (`isVirtual = true`) est non supprimable et invisible dans la navigation. Il reçoit automatiquement tout document dont le dossier explicite a été supprimé. Il en existe exactement un par campagne.

---

### `DocumentType`

Spécialisation optionnelle d'un `Document`. Les types système built-in sont : `SCENARIO`, `SCENE`, `NPC`, `LOCATION`, `NOTE`, `PLAYER_CHARACTER`, `LIVE_NOTE`, `REVEAL`. Les types personnalisés (MJ-définis) sont Could Have post-MVP. Le type est indicatif — il n'impose pas la structure des blocs.

---

### Personnage joueur

`Document` de type `PLAYER_CHARACTER` (`documentTypeId = PLAYER_CHARACTER`). Il n'existe pas d'entité dédiée `PlayerCharacter` dans le modèle — un personnage joueur est un document ordinaire spécialisé par son type. Le personnage est un point d'**affichage** et d'organisation : les notes `PLAYER_PRIVATE` sont liées à leur **auteur** (`createdById` ou `guestAccessId`), pas au personnage. L'association `characterId` sur une `LIVE_NOTE` sert à l'affichage groupé, pas à définir un droit de propriété.

- Créé et géré par le joueur dans sa section dédiée (UC-06, UC-12).
- Propriétaire domaine : **Content Library**.
- Voir aussi : `DocumentType`, `LIVE_NOTE`, `PLAYER_PRIVATE`.

---

### `LIVE_NOTE` (note de session)

Type de document (`documentTypeId = LIVE_NOTE`) représentant une note prise pendant ou juste après une session. Une note de session est un `Document` ordinaire stocké dans **Content Library** ; **Session Conduct** la référence via `sessionNoteIds`. Deux champs de premier niveau spécifiques : `characterId` (personnage associé, pour les notes `PLAYER_PRIVATE` joueur) et `guestAccessId` (auteur invité sans compte).

- Visibilité par défaut à la création par le MJ : `GM_ONLY` (terme de besoin : « privé MJ »).
- Visibilité d'une note créée par un joueur : `PLAYER_PRIVATE` (terme de besoin : « note personnelle joueur »).

> Cartographie visibilité ↔ langage de besoin (source : `US-UC-06-vue-session.md` §Mapping de visibilité) :
> `GM_ONLY` = « privé MJ » ; `PLAYER_PRIVATE` = « personnelle joueur » / « note personnelle joueur » ; `PUBLIC` = « visible par les joueurs ».

---

### `REVEAL` (document de révélation)

Type de document créé lié à une scène pour représenter une information à révéler aux joueurs. Créé avec `visibility = GM_ONLY` par défaut. Passe à `PUBLIC` uniquement via une action explicite du MJ en session (UC-08). Ce passage est permanent jusqu'à `Unshare()` explicite.

---

### Document réutilisable (template)

`Document` avec `isReusable = true`. Peut être instancié : l'instanciation crée une copie profonde indépendante. Les modifications du source après instanciation n'affectent pas les instances. Une instance est identifiée par `sourceDocumentId` renseigné.

- Relation clé : le `ScenarioLibrary` est construit sur des `Document` réutilisables marqués `isReusable = true`.

---

### Instance (d'un document réutilisable)

Copie profonde et indépendante d'un document réutilisable, créée par `Document.Instantiate()`. Après création, l'instance n'a aucun lien vivant avec son source. Elle porte ses propres blocs, liens et modifications.

---

### `DocumentLink`

Value object représentant une référence ordonnée d'un `Document` vers un autre. Permet à un scénario de lister ses scènes, à une scène de lister ses PNJ, etc. Les backlinks (documents qui pointent vers un document donné) ne sont pas stockés — ils sont calculés à la lecture.

---

### Dossiers système

Quatre dossiers créés automatiquement à la création d'une campagne : `Personnages`, `Joueurs`, `Scénarios`, `Notes`. Plus le dossier virtuel « Non classés ». Ces noms sont des points de départ renommables et supprimables librement.

---

### Export de campagne

Capacité permettant au MJ d'exporter l'ensemble d'une campagne — documents, notes, structure de dossiers — dans un format ouvert, lisible et réutilisable hors de l'application. Disponible en mode local comme avec un compte. Matérialise la promesse de possession des données : la possession n'est actionnable que si elle est exportable.

- Priorité : **Should Have** (post-MVP, UC-HORS-MVP à créer — voir `vision/moscow.md` §Should Have et `vision/vision-produit.md` §5).
- Distinct de la migration locale→cloud (qui importe des données vers un compte) : l'export produit un fichier autonome indépendant du compte.
- Défini dans : **vision-produit.md** §5, **moscow.md** §Should Have, **UC-01 A4**.

---

## 5. Session Conduct

### `Session`

Agrégat représentant une séance de jeu. Cycle de vie unidirectionnel : `LIVE → CLOSED → ARCHIVED`. Créée directement en état `LIVE`. Porte la liste des documents épinglés (`pinnedDocumentIds`), les références aux notes de session (`sessionNoteIds`) et le résumé (`summary`). Il ne peut y avoir qu'une seule session `LIVE` par campagne simultanément.

---

### `SessionStatus`

État de la session : `LIVE` (session en cours, tableau de bord actif, partage temps réel), `CLOSED` (session terminée, contenu encore éditable — résumé et notes rétroactives MJ), `ARCHIVED` (lecture seule complète, aucune modification possible).

---

### `SessionViewConfig`

Configuration du tableau de bord session au niveau de la campagne. Définit quels dossiers le MJ met en avant dans sa vue session (`focusedFolders`). Il en existe exactement un par campagne, créé automatiquement à `CampaignCreated`.

---

### Documents épinglés (`pinnedDocumentIds`)

Liste des documents explicitement épinglés par le MJ pendant une session. L'épinglage est local à la session : il n'affecte pas la visibilité du document dans la bibliothèque. Un document épinglé reste accessible rapidement dans le panneau dédié.

---

### Résumé de session (`summary`)

Texte narratif libre rédigé par le MJ après la séance. Modifiable uniquement en état `CLOSED`. Distinct des notes de session (`sessionNoteIds`) qui sont des `Document` de type `LIVE_NOTE`.

---

### Vue session MJ

Interface de pilotage pendant une session `LIVE`. Tableau de bord configurable affichant les dossiers sélectionnés dans `SessionViewConfig`, le panneau des documents épinglés, la zone de prise de notes de session et la barre de recherche globale. Disponible en mode local (sans compte) pour le MJ.

---

### Vue session joueur

Vue restreinte accessible aux joueurs (authentifiés ou invités) pendant une session `LIVE`. Affiche uniquement les documents `PUBLIC` et les notes de session `PLAYER_PRIVATE` propres au joueur connecté.

---

## 6. Termes écartés

| Terme écarté | Terme retenu | Raison / source |
|---|---|---|
| `propriétés structurées.guestAccessId` | `guestAccessId` (champ de premier niveau) | Promu depuis les propriétés structurées vers un champ de premier niveau du `Document` — `content-library.md` §DocumentType (note `live_note`) et `session-conduct.md` §Notes de session. L'ancien chemin `propriétés structurées.guestAccessId` est observable dans `UC-06-vue-session.md` l.229 (foyer de dérive signalé par l'audit CP-20). |
| « note privée MJ » | « privé MJ » / `GM_ONLY` | Forme longue non normalisée. Le terme retenu dans la prose de besoin est « privé MJ » ; le terme domaine est `GM_ONLY`. Les deux formes coexistaient dans les UC et US (observable dans `UC-06-vue-session.md` et `US-UC-06-vue-session.md`). Cartographie explicite dans `US-UC-06-vue-session.md` §Mapping de visibilité. |
| « note privée » (sans qualificatif) | « note de session privé MJ » ou « note de session personnelle joueur » | Terme ambigu : désignait tantôt `GM_ONLY` (privé MJ), tantôt `PLAYER_PRIVATE` (personnelle joueur). Les deux niveaux sont distincts et non interchangeables. Observable dans `UC-06-vue-session.md` l.256 (« note de session privé MJ ») et `identity-access.md` l.94 (« note privée résiduelle »). |
| « note personnelle » | « note de session personnelle joueur » | Raccourci non qualifié ambiguisant avec les notes du MJ. Le terme complet est requis dans les artefacts de besoin. Observable dans `US-UC-06-vue-session.md` titre de l'US-06-08. |
| « visible par les joueurs » | `PUBLIC` | Terme de langage besoin ; l'équivalent domaine est `visibility = PUBLIC`. Les deux formes sont légitimes dans leurs couches respectives. La dérive consistait à employer « visible par les joueurs » dans des artefacts de domaine au lieu de `PUBLIC` — observable dans `UC-06-vue-session.md` l.199-261. Cartographie dans `US-UC-06-vue-session.md` §Mapping. |
| `personnelle joueur` (seul) | `PLAYER_PRIVATE` (domaine) / « note de session personnelle joueur » (besoin) | Forme contractée non qualifiée. Employée comme valeur de visibilité dans `UC-06-vue-session.md` l.228 (« Visibilité : visible par les joueurs, privé MJ ou personnelle joueur »). Le terme domaine est `PLAYER_PRIVATE` ; le terme de besoin qualifié est « note de session personnelle joueur ». |
| « accès invité » (sans `Guest`) | `GuestAccess` | Traduction française partielle de l'identifiant de domaine. Les UC emploient parfois « accès invité » ou « joueur invité sans compte » ; l'identifiant canonique du domaine reste `GuestAccess`. La coexistence est observable dans `UC-09` et `UC-06`. |
| « membres permanents » | `CampaignMembership` actifs / Membres | Terme employé dans `campaign-management.md` §One-shot pour distinguer les joueurs avec compte des invités. Remplacé par la formulation explicite (`CampaignMembership` vs `GuestAccess`). |

---

## 7. Conventions transverses

### Casse des identifiants de domaine

- **`PascalCase`** pour les agrégats, entités et value objects : `Document`, `GuestAccess`, `SessionViewConfig`, `CampaignMembership`, `DocumentLink`.
- **`SCREAMING_SNAKE_CASE`** pour les valeurs d'enum : `GM_ONLY`, `PLAYER_PRIVATE`, `PUBLIC`, `LIVE_NOTE`, `PLAYER_CHARACTER`, `ONE_SHOT`, `ACTIVE`, `FROZEN`.
- Les identifiants de domaine sont légitimes dans les artefacts de besoin quand ils désignent un concept précis du modèle. Ils ne sont pas de la technologie.

### Français vs anglais

L'usage du corpus est le suivant : les noms de domaine structurants restent en anglais quand ils sont des identifiants (`GuestAccess`, `Document`, `Session`). Les descriptions et règles sont en français. Cette convention reflète l'usage réel du corpus — elle n'est pas une règle imposée.

### Pluriels

Les pluriels suivent la morphologie française : `Documents`, `Campagnes`, `Sessions`, `Membres`. Les identifiants de domaine en `PascalCase` ne se pluralisent pas dans le code de domaine, mais peuvent se pluraliser dans la prose française de conception.

### Abréviations admises

| Abréviation | Développé |
|---|---|
| MJ | Maître du Jeu (rôle `OWNER` ou `GM` dans une campagne) |
| UC | Use Case |
| US | User Story |
| UJ | User Journey |
| RB | Règle métier (préfixe de codification des règles dans les US, sections « Règles métier ») |
| PNJ | Personnage Non Joueur (`Document` de type `NPC`) |

### Identifiants de documents et artefacts

Format de référence : `UC-NN` (use case), `US-UC-NN` (epic user story), `UJ-UC-NN` (user journey), `US-NN-NN` (user story individuelle), `RB-NN-NN` (règle de besoin).

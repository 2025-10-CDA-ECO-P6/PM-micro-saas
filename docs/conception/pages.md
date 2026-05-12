# Pages de l'application — Haversack

Référence complète des pages et des composants UI majeurs.
Sert de base pour les wireframes.

---

## Principe de distinction page / composant

Une **page** a sa propre URL et peut être bookmarkée ou partagée.
Un **modal / panel / drawer** est contextuel — il s'ouvre par-dessus la page courante.
Un **onglet** est une section d'une page, pas une page indépendante.

---

## Conventions

| Notation | Signification |
|---|---|
| `[MJ]` | Accessible au MJ uniquement |
| `[Joueur]` | Accessible au joueur (compte requis) |
| `[MJ+Joueur]` | Accessible aux deux |
| `[Guest]` | Accessible sans compte (GuestAccess) |

---

## Pages

### AUTH

---

#### P-01 — Connexion
**URL :** `/login`
**Acteurs :** tous (non authentifié)
**Éléments :** champ email, champ mot de passe, bouton connexion, lien "créer un compte", lien "mot de passe oublié".

---

#### P-02 — Inscription
**URL :** `/register`
**Acteurs :** tous (non authentifié)
**Éléments :** champ nom d'affichage, email, mot de passe, confirmation, bouton créer.

---

#### P-03 — Réinitialisation du mot de passe
**URL :** `/reset-password?token=...`
**Acteurs :** tous
**Éléments :** nouveau mot de passe, confirmation, bouton valider.
**Note :** `/forgot-password` est un modal ou un formulaire inline sur P-01, pas une page dédiée.

---

### DASHBOARD

---

#### P-04 — Dashboard
**URL :** `/`
**Acteurs :** `[MJ+Joueur]`
**Description :** liste de toutes les campagnes de l'utilisateur. Premier écran après connexion.
**Éléments :**
- Cartes campagne (nom, système de jeu, rôle de l'utilisateur dans cette campagne, dernière activité)
- Distinction visuelle MJ / Joueur sur chaque carte
- Bouton "Nouvelle campagne" → ouvre **M-01**
- Section campagnes archivées (réduite par défaut)

---

### CAMPAGNE

---

#### P-05 — Hub de campagne
**URL :** `/campaigns/{campaignId}`
**Acteurs :** `[MJ+Joueur]`
**Description :** page centrale de la campagne, organisée en onglets. Point de navigation principal.

**Onglets :**

| Onglet | Contenu | Acteur |
|---|---|---|
| Aperçu | Résumé, dernière session, activité récente | MJ+Joueur |
| Contenu | Bibliothèque (dossiers + documents) | MJ |
| Scénarios | Liste ordonnée des scénarios | MJ |
| Sessions | Historique des sessions + statuts | MJ |
| Membres | Liste membres, invitations, associations joueur↔perso | MJ |
| Paramètres | Nom, système de jeu, archivage, suppression | MJ |

**Éléments globaux :**
- En-tête campagne (nom, système de jeu)
- Navigation onglets
- Barre de recherche globale → ouvre **C-01**
- Bouton "Nouvelle session" (depuis l'onglet Sessions)

---

#### P-06 — Bibliothèque (onglet Contenu de P-05)
**URL :** `/campaigns/{campaignId}` (onglet Contenu)
**Acteurs :** `[MJ]`
**Description :** navigation dans les dossiers et documents de la campagne.
**Éléments :**
- Arborescence de dossiers (panneau gauche)
- Grille / liste de documents dans le dossier sélectionné
- Filtres par type (NPC, Scénario, Note, Document libre)
- Barre de tags (filtre par tag)
- Bouton "Nouveau" contextuel selon le dossier → ouvre **M-03**
- Gestion des tags → ouvre **M-04**

---

### DOCUMENTS ET FICHES

---

#### P-07 — Éditeur de document
**URL :** `/campaigns/{campaignId}/documents/{documentId}`
**Acteurs :** `[MJ+Joueur]` (selon visibilité du document)
**Description :** éditeur de contenu par blocs. Utilisé directement pour les notes et documents libres. Embarqué dans les fiches PNJ, scénarios, scènes et personnages.
**Éléments :**
- Titre éditable
- Corps : blocs empilés (texte, titre, liste, relation, image...) — `/` pour ajouter un bloc
- Panneau latéral droit : tags, dossier, visibilité, backlinks vers d'autres éléments
- Indicateur de visibilité (GM_ONLY, PUBLIC, PLAYER_PRIVATE)
- Bouton "Partager" → ouvre **M-05**

---

#### P-08 — Fiche PNJ
**URL :** `/campaigns/{campaignId}/npcs/{npcId}`
**Acteurs :** `[MJ]`
**Description :** fiche complète d'un PNJ. Métadonnées en en-tête + éditeur de document associé.
**Éléments :**
- En-tête : nom, badge statut (vivant / mort / inconnu), tags
- Corps : éditeur de document (P-07 embarqué)
- Panneau latéral : backlinks, sessions où ce PNJ apparaît
- Bouton "Épingler à une session"

---

#### P-09 — Détail scénario
**URL :** `/campaigns/{campaignId}/scenarios/{scenarioId}`
**Acteurs :** `[MJ]`
**Description :** document du scénario + liste ordonnée des scènes.
**Éléments :**
- En-tête : titre, position dans la campagne (ordre)
- Document du scénario (P-07 embarqué)
- Section scènes : liste ordonnée drag-and-drop, bouton "Nouvelle scène"
- Panneau : PNJ associés à ce scénario

---

#### P-10 — Détail scène
**URL :** `/campaigns/{campaignId}/scenarios/{scenarioId}/scenes/{sceneId}`
**Acteurs :** `[MJ]`
**Description :** document de scène éditable.
**Éléments :**
- Fil d'Ariane : campagne → scénario → scène
- Titre + éditeur document (P-07 embarqué)
- Navigation scène précédente / suivante

---

### SESSIONS

---

#### P-11 — Préparation de session
**URL :** `/campaigns/{campaignId}/sessions/{sessionId}`  *(statut PLANNED)*
**Acteurs :** `[MJ]`
**Description :** checklist de préparation avant de lancer la session.
**Éléments :**
- Sélecteur scénario actif
- Liste PNJ pour cette session (suggestions auto depuis le scénario + ajout manuel)
- Notes épinglées
- Bouton "Lancer la session" → passe en LIVE, redirige vers P-12

---

#### P-12 — Vue session (conduite)
**URL :** `/campaigns/{campaignId}/sessions/{sessionId}` *(statut LIVE)*
**Acteurs :** `[MJ]`
**Description :** tableau de bord de conduite. Page principale du produit.
**Éléments :**
- **Panneau scénario** : scènes avec case à cocher "jouée", navigation entre scènes
- **Panneau PNJ** : liste des PNJ de la session, clic → drawer fiche PNJ
- **Panneau joueurs** : fiches personnages des participants, clic → drawer fiche
- **Zone LiveNotes** : saisie rapide, horodatage automatique, liste des notes de la session
- **Barre de recherche** (persistante) → ouvre **C-01**
- **Bouton "Créer à la volée"** → ouvre **M-06**
- **Bouton "Épingler"** : épingle n'importe quel élément dans la vue
- **Bouton "Clôturer"** → ouvre **M-07**

---

#### P-13 — Résumé de session
**URL :** `/campaigns/{campaignId}/sessions/{sessionId}` *(statut CLOSED)*
**Acteurs :** `[MJ]`
**Description :** relecture et rédaction du résumé post-session. Même URL que P-11/P-12, UI différente selon le statut.
**Éléments :**
- Liste des LiveNotes de la session (chronologique, lecture)
- Éditeur de résumé (SessionSummary, blocs)
- Toggle "Partager le résumé aux joueurs"
- Bouton "Archiver" → passe en ARCHIVED (lecture seule définitive)

---

### CÔTÉ JOUEUR

---

#### P-14 — Rejoindre une campagne
**URL :** `/join/{invitationToken}`
**Acteurs :** redirige vers login si non authentifié
**Description :** page d'acceptation d'une invitation.
**Éléments :** nom de la campagne, nom du MJ, bouton "Rejoindre", message d'erreur si lien expiré.

---

#### P-15 — Accès invité
**URL :** `/guest/{guestToken}`
**Acteurs :** `[Guest]`
**Description :** accès temporaire sans compte.
**Éléments :** champ pseudo (si premier accès), bouton "Accéder". Redirige vers la fiche personnage associée (P-16).

---

#### P-16 — Fiche personnage (vue joueur)
**URL :** `/campaigns/{campaignId}/characters/{characterId}`
**Acteurs :** `[Joueur]` `[Guest]` (lecture seule pour guest)
**Description :** fiche personnage avec onglets.

**Onglets :**

| Onglet | Contenu | Guest |
|---|---|---|
| Fiche | Éditeur document associé (P-07 embarqué) | Lecture seule |
| Inventaire | Liste objets, quantités, descriptions | Lecture seule |
| Notes privées | Notes PLAYER_PRIVATE, invisibles au MJ | Non accessible |

---

#### P-17 — Informations partagées
**URL :** `/campaigns/{campaignId}/shared`
**Acteurs :** `[Joueur]` `[Guest]`
**Description :** feed chronologique des éléments partagés par le MJ (documents, résumés de session, notes publiques).
**Éléments :** liste chronologique, filtre par type, accès en lecture au document partagé.

---

### PARAMÈTRES UTILISATEUR

---

#### P-18 — Profil
**URL :** `/settings/profile`
**Acteurs :** `[MJ+Joueur]`
**Éléments :** nom d'affichage, avatar, email, mot de passe (lien vers P-03).

---

## Composants UI majeurs (non-pages)

Ces éléments n'ont pas d'URL propre. Ils s'ouvrent par-dessus la page courante.

---

### Modals

#### M-01 — Créer une campagne
**Déclencheur :** bouton "Nouvelle campagne" sur P-04
**Éléments :** champ nom, sélecteur système de jeu, description courte, bouton créer.

---

#### M-02 — Inviter un joueur
**Déclencheur :** bouton "Inviter" dans l'onglet Membres de P-05
**Éléments :** toggle Lien / Email, champ email (si Email), bouton générer/envoyer, affichage du lien avec copie.

---

#### M-03 — Créer un élément
**Déclencheur :** bouton "Nouveau" dans la bibliothèque (P-06)
**Éléments :** sélecteur de type (Note, PNJ, Document libre...), champ titre, bouton créer.

---

#### M-04 — Gérer les tags
**Déclencheur :** icône tags dans P-06
**Éléments :** liste tags (pastille couleur, nom), bouton modifier, bouton supprimer, formulaire création.

---

#### M-05 — Partager un élément
**Déclencheur :** bouton "Partager" sur P-07
**Éléments :** sélecteur de visibilité (PUBLIC, GM_ONLY, PLAYER_PRIVATE), sélecteur de cible (tous les membres, membre spécifique, personnage spécifique).

---

#### M-06 — Créer à la volée (depuis session)
**Déclencheur :** bouton "Créer à la volée" sur P-12
**Éléments :** sélecteur de type (Note, PNJ, Personnage, Document), champ titre seul, bouton créer. Création minimale — auto-lié à la session.

---

#### M-07 — Clôturer une session
**Déclencheur :** bouton "Clôturer" sur P-12
**Éléments :** confirmation, aperçu du nombre de LiveNotes capturées, bouton "Clôturer et rédiger le résumé" → redirige vers P-13.

---

### Command palette

#### C-01 — Recherche globale
**Déclencheur :** `Cmd+K` / `Ctrl+K` ou clic sur la barre de recherche
**Scope :** dans le contexte d'une campagne
**Éléments :**
- Champ de saisie (focus automatique)
- Résultats en temps réel groupés par type (Documents, PNJ, Scénarios, Notes)
- Navigation clavier (↑↓ + Entrée)
- Extraits avec termes surlignés
- Clic ou Entrée → navigue vers l'élément

---

### Drawers (panneaux latéraux glissants)

#### D-01 — Aperçu rapide d'un PNJ
**Déclencheur :** clic sur un PNJ dans P-12 (vue session)
**Éléments :** fiche PNJ en lecture, bouton "Ouvrir la fiche complète" → P-08.

#### D-02 — Aperçu rapide d'une fiche personnage
**Déclencheur :** clic sur un joueur dans P-12 (vue session)
**Éléments :** fiche personnage en lecture, inventaire.

---

## Récapitulatif par acteur

### MJ
P-01, P-02, P-03, P-04, P-05, P-06, P-07, P-08, P-09, P-10, P-11, P-12, P-13, P-18

### Joueur (avec compte)
P-01, P-02, P-04, P-05 (onglet Aperçu), P-14, P-16, P-17, P-18

### Joueur invité (sans compte)
P-15, P-16 (lecture seule), P-17


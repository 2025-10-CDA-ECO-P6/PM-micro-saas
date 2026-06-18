# Vue campagne (espace de travail)

> Fiche de description d'écran basse-fidélité — Vague 3, sous-vague 3 (préparation espace partagé), cluster A.
> Hub de préparation d'un espace partagé — ancre la grammaire de préparation des autres fiches de la sous-vague.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Vue campagne (espace de travail)
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT (PERSONAL : absent par nature — voir AR-14 ; la version réduite
               de cette surface pour l'espace personnel n'est pas wireframée ici)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-02, UC-04, UC-05, UC-06, UC-08, UC-11 ; AR-01, AR-09, AR-10, AR-13
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ accède depuis un point d'entrée unique à l'ensemble des ressources de son espace
            partagé — dossiers et documents, vue session, paramètres, invitation de joueurs —
            afin de préparer sa campagne ou son one-shot sans avoir à naviguer hors de l'espace.
```

---

### Zones et hiérarchie

```
[ ZONE DE NAVIGATION PAR DOSSIERS ]
  type     : principal
  rôle     : affiche l'arborescence des dossiers de l'espace et permet au MJ de parcourir
             son contenu ; sert de point d'entrée vers la fiche navigation-dossiers
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-05 ; AR-11

[ ZONE DE CONTENU / LISTE DES DOCUMENTS ]
  type     : principal
  rôle     : liste les documents du dossier courant ou de l'espace en vue condensée
             (titre et repère de type) ; permet la création d'un document dans le
             contexte courant ; accès à l'éditeur pour tout document existant
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-04 ; UC-05 A3 ; AR-11

[ ZONE D'ACCÈS À LA VUE SESSION ]
  type     : principal
  rôle     : point d'entrée vers la vue session MJ — permet de lancer une session
             (basculement en mode LIVE) ou d'accéder à la vue session en mode
             configuration ; présente un repère « session en cours » si une session
             est déjà au statut en cours dans cet espace
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-06 §Déclencheur ; AR-09 (lancement depuis la vue campagne)

[ ZONE D'ACCÈS AUX PARAMÈTRES ]
  type     : latéral
  rôle     : accès à la configuration de la vue session (dossiers mis en avant, ordre),
             à l'archivage de l'espace et à la génération du lien d'invitation
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-09 (configuration de la vue session) ; UC-11 (lien d'invitation)

[ AFFORDANCE DE GÉNÉRATION DE LIEN D'INVITATION ]
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement non figé :
    l'affordance est logée dans la vue session OU dans la vue campagne — non tranché.
    Elle figure ici à titre de traçabilité en cohérence avec vue-session-mj.md (D2 :
    pas de fiche dédiée) ; son appartenance à cet écran n'est pas une décision.
  rôle     : permet au MJ de générer le lien d'invitation à partager aux joueurs ;
             le lien donne accès à la session en cours (accès temporaire) ou à la
             campagne de façon durable selon le périmètre choisi (UC-11)
  visibilité : MJ seul
  ancrage  : UC-06 (lien ponctuel) ; UC-11 §Scénario nominal ; UC-11 A4 (fraction Must — AR-10)

[ BARRE DE RECHERCHE ]
  type     : châssis
  rôle     : accélérateur de navigation omniprésent dans l'espace — titre seul au MVP ;
             les résultats s'ouvrent sans interrompre le contexte courant de la vue
             campagne (AR-11)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-14 ; AR-11
```

---

### Ce que l'utilisateur peut faire

```
- [UC-06 §Déclencheur ; AR-09] lancer la session → la vue session s'ouvre en mode LIVE ;
  la session passe au statut en cours ; les joueurs peuvent rejoindre via lien

- [AR-09 ; UC-06 A5] accéder à la vue session en mode configuration → la vue session
  s'ouvre sans lancer de session ; le MJ peut configurer les panneaux et préparer
  la session

- [AR-09 ; UC-06 A5] rejoindre une session en cours → si une session est déjà au
  statut en cours dans cet espace, la vue s'ouvre directement en mode LIVE ;
  pas de reconfiguration nécessaire

- [UC-05 ; AR-11] naviguer dans les dossiers → la zone de navigation par dossiers
  déroule l'arborescence de l'espace ; voir fiche navigation-dossiers.md

- [UC-04 ; UC-05 A3] créer un document dans le dossier courant → l'éditeur de document
  s'ouvre ; le document est automatiquement placé dans le dossier actif

- [UC-04] ouvrir un document existant → l'éditeur de document s'ouvre sur le contenu
  du document sélectionné

- [UC-14 ; AR-11] rechercher dans le contenu de l'espace → titre seul au MVP ;
  résultats affichés sans interrompre le contexte de la vue campagne

- [UC-06 ; UC-11 A4] générer un lien d'invitation →
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] l'affordance est logée dans
  la vue session ou la vue campagne — non tranché ; le lien donne accès à la
  session en cours (accès temporaire) ou à la campagne de façon durable selon
  le périmètre choisi (UC-11)

- [AR-09] accéder aux paramètres de l'espace → configuration de la vue session
  (dossiers mis en avant, ordre), archivage de l'espace
```

---

### États

```
état vide (espace nouvellement créé, aucun document hors dossiers système) :
  les quatre dossiers système (Personnages, Joueurs, Scénarios, Notes) sont présents
  dans la zone de navigation ; la zone de contenu invite à créer un premier document ;
  aucune session n'est en cours ; l'invite de lancement de session est visible (AR-09)

état chargé (espace avec documents) :
  la zone de navigation affiche l'arborescence complète des dossiers ;
  la zone de contenu liste les documents du dossier courant en vue condensée ;
  le repère « session en cours » apparaît dans la zone d'accès à la vue session
  si une session est au statut en cours dans cet espace (AR-09)

état erreur (perte de connexion en mode cloud) :
  la notification d'état de synchronisation apparaît de façon non bloquante,
  sans couvrir la surface (châssis S7 §Notification d'état de synchronisation ;
  NFR-OFF-04) ; la navigation dans les dossiers et la lecture des documents
  restent disponibles depuis le cache local
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la vue campagne.

```
annonce sans action :
  - apparition du repère « session en cours » (basculement d'une session au statut en cours
    dans cet espace) : annoncé assistivement sans déplacement de focus — NFR-ACC-02
  - changement d'état de synchronisation (reconnexion, stockage sous pression) :
    annoncé assistivement — châssis S7 §Notification ; NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone d'accès à la vue session (repère « session en cours » lisible
               sans approche de l'écran — NFR-ACC-04)
  priorité 2 — zone de navigation par dossiers (arborescence de l'espace)
  priorité 3 — zone de contenu / liste des documents
  priorité 4 — barre de recherche
  source : NFR-ACC-04 (usage à distance normale de l'écran pendant la préparation)
```

---

### Sources

```
Sources : UC-02 ; UC-04 ; UC-05 ; UC-06 ; UC-08 ; UC-11 ;
          AR-01 ; AR-09 ; AR-10 ; AR-11 ; AR-13 ; AR-14 ;
          NFR-ACC-02 ; NFR-ACC-04 ; NFR-OFF-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Mode local (déclenché — surface MJ)

```
Bandeaux présents (châssis standard — voir zoning.md §S7 §Châssis mode local) :
  - bandeau de durabilité : non bloquant ; signale que les données locales ne
    bénéficient pas d'une garantie de conservation permanente ;
    propose la création d'un compte
  - bandeau de confidentialité : non bloquant ; signale l'absence de protection
    par identifiants ; propose la création d'un compte

Fonctions cloud désactivées sur la vue campagne en mode local :
  - [AR-13 ; UC-01 A1] génération du lien d'invitation → invite contextuelle :
    « Inviter des joueurs nécessite un compte — créer un compte en un geste »
  - [AR-13 ; UC-01 A1] lancement de session avec partage vers des joueurs →
    invite contextuelle : « Partager avec des joueurs nécessite un compte »

Fonctions disponibles localement (non désactivées) :
  La navigation dans les dossiers, la création et la modification de documents,
  la recherche et l'accès à la vue session (modes configuration et consultation
  CLOSED) restent disponibles sans compte.
  Le lancement d'une session en mode LIVE local est disponible — seul le partage
  vers des joueurs et la génération du lien d'invitation sont cloud-dépendants.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-11 scénario nominal] invitation par email (le lien d'invitation
    est la fraction Must couverte — AR-10)
  [HORS-MVP — UC-11 scénario nominal] écran Membres complet (révocations,
    associations joueur-personnage) — différé (AR-10)
  [HORS-MVP — moscow.md §Could Have] export d'espace

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement de l'affordance de
    génération de lien d'invitation : vue session OU vue campagne — non tranché
  [SOUS-SPÉCIFIÉ — S9 §Interface de l'espace personnel sous-spécifiée] la
    déclinaison réduite de cette surface pour l'espace personnel (sans vue session,
    sans invitation de joueurs, sans dossiers système nommés) n'est pas wireframée
    séparément au MVP — son interface relève d'un point d'interview (UC-02
    §Questions à valider en interview)
```

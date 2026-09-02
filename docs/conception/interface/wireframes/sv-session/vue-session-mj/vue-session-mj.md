# Vue session MJ

> Fiche de description d'écran basse-fidélité — écran prioritaire.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md` sur l'écran le plus riche du MVP.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.

---

## En-tête de fiche

```
Nom          : Vue session MJ
Surface      : MJ
Contexte     : session
Type d'espace: CAMPAIGN | ONE_SHOT (PERSONAL : absent par nature — pas de vue session, AR-01 ; AR-14)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-06, UC-07, UC-08, UC-14 ; US-06-01 à US-06-10 ; UJ-UC-06
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ pilote sa session depuis un tableau de bord configurable qui lui donne accès
            à ses dossiers, à ses notes de session et au partage de documents vers les joueurs
            — sans quitter la surface et sans interrompre le rythme de la table.
```

---

### Zones et hiérarchie

```
[ INDICATEUR DE STATUT DE SESSION ]
  type     : châssis
  rôle     : matérialise le mode courant de la vue session
             (configuration / LIVE / consultation CLOSED) de façon omniprésente,
             quelle que soit la position de défilement
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : AR-09 ; UJ-UC-06 §friction (« Statut CLOSED peu visible »)

[ INDICATEUR DE PARTAGE ]
  type     : châssis
  rôle     : récapitule en permanence ce que les joueurs voient en ce moment —
             distinct du panneau des documents épinglés (un document peut être
             épinglé sans être partagé, et réciproquement)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : AR-12 ; châssis S7 §Indicateur de partage

[ ZONE DE NOTES ]
  type     : principal
  rôle     : zone de saisie des notes de session — co-présente et jamais masquée
             par défilement, quelle que soit la configuration des panneaux ;
             notes créées privé MJ par défaut
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul (GM_ONLY par défaut)
  ancrage  : AR-04 ; UJ-UC-06 §friction (« Zone de saisie non visible sans scroll
             si les panneaux occupent tout l'espace »)

[ ZONE DE RÉSUMÉ DE SESSION ]
  type     : formulaire
  rôle     : permet au MJ de rédiger ou modifier le résumé narratif libre de la séance
             (`Session.summary`) ; distinct des notes de session (rattachées via
             `AttachNote`) — porté par un champ dédié de la session ; éditable
             uniquement quand la session est terminée (CLOSED)
  priorité : secondaire-configurable (accessible uniquement en mode consultation CLOSED)
  visibilité : MJ seul
  ancrage  : UC-06 E3 (« le MJ peut encore ajouter des notes de session rétroactives
             et modifier le résumé ») ; domaine session-conduct.md (`Session.summary` ;
             `UpdateSummary(text)`, autorisé uniquement en état CLOSED)

[ PANNEAUX DE DOSSIERS ]
  type     : principal
  rôle     : affiche le contenu des dossiers mis en avant par le MJ dans sa
             configuration ; chaque panneau présente titre et repère de type
             pour permettre l'accès rapide même dans un dossier dense
             (défaut épuré mais informatif — AR-04)
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-04 ; UC-06 ; US-06-01

[ PANNEAU DES DOCUMENTS ÉPINGLÉS ]
  type     : favoris
  rôle     : liste les documents explicitement épinglés par le MJ pendant la
             session ; position stable sur la surface ; l'épinglage est local
             à la session et n'affecte pas la visibilité côté joueur
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-12 ; UC-06 ; UC-08

[ BARRE DE RECHERCHE ]
  type     : châssis
  rôle     : accélérateur de navigation omniprésent — titre seul au MVP ;
             les résultats s'ouvrent dans un panneau latéral sans interrompre
             le contexte courant de la vue session
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-14 ; AR-11 ; UJ-UC-06 §Opportunités UX

[ PANNEAU LATÉRAL DE RÉSULTATS DE RECHERCHE ]
  type     : latéral
  rôle     : affiche les résultats de recherche sans remplacer le contexte
             courant de la vue session ; apparaît à la saisie, disparaît à
             la fermeture
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-11 ; NFR-PERF-04

[ RÈGLES DE DENSITÉ — MODE LIVE (AR-19) ]
  type     : règle de zoning
  rôle     : définit le plancher garanti, le plafond d'éviction et la priorité
             d'éviction quand les panneaux saturent la surface en mode LIVE

  Plancher garanti (jamais masqué quelle que soit la configuration) :
    - indicateur de statut de session
    - indicateur de partage
    - zone de notes
    - barre de recherche
    Ces quatre zones ne peuvent pas être évincées par l'ouverture d'un panneau
    de dossier, de résultats de recherche ou du panneau des épinglés.

  Plafond d'éviction — panneaux de dossiers :
    Le nombre de panneaux de dossiers affichables simultanément est borné
    (nombre maximal défini en configuration, non libre) ; au-delà du plafond,
    les panneaux excédentaires se replient automatiquement dans le rail de
    bascule (AR-07) — le MJ accède aux dossiers repliés via ce rail sans
    perdre l'accès aux zones du plancher.

  Priorité d'éviction quand le panneau latéral de résultats de recherche s'ouvre :
    Le panneau de dossier le moins prioritaire (dernier ajouté dans la
    configuration, ou panneau de priorité la plus basse déclarée par le MJ)
    cède la place en premier ; le panneau des documents épinglés ne cède jamais
    la place au panneau de résultats (les épinglés restent visibles pendant
    une recherche).

  Divulgation progressive — zones secondaires :
    Les zones secondaires (panneau des documents épinglés, panneaux de dossiers
    supplémentaires) s'affichent selon l'espace disponible après placement du
    plancher ; elles n'apparaissent pas si la surface est pleine après le plancher.

  ancrage  : AR-19 ; AR-07 ; AR-04

[ AFFORDANCE DE GÉNÉRATION DE LIEN D'INVITATION ]
  type     : action
  rôle     : permet au MJ de générer le lien de SESSION PONCTUEL (GuestAccess SESSION —
             AR-10) à partager aux joueurs pour rejoindre la session en cours ;
             ce lien est temporaire et limité à la session active — il n'ouvre pas
             un accès durable à la campagne ; l'affordance est logée dans la vue session :
             c'est un acte de pilotage en séance, déclenché en mode LIVE (AR-10 ; AR-18)
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-06 (lien ponctuel) ; UC-11 §Scénario nominal ; UC-11 A4 (fraction Must — AR-10)
```

---

### Ce que l'utilisateur peut faire

```
- [AR-09 ; UC-06 §Déclencheur] lancer la session → la vue bascule en mode LIVE ;
  la session passe au statut en cours ; les joueurs peuvent rejoindre via lien

- [AR-09 ; UC-06 A5] reprendre une session en cours → la vue s'ouvre directement
  en mode LIVE si une session est déjà au statut en cours ;
  pas de reconfiguration nécessaire

- [AR-04 ; US-06-02] configurer les panneaux de dossiers → le MJ sélectionne et
  ordonne les dossiers mis en avant, sans lancer de session ;
  la configuration persiste entre les modes

- [AR-12 ; UC-08] partager un document → le document devient visible par les joueurs
  (PUBLIC) ; en mode LIVE, il s'auto-épingle dans le panneau des documents épinglés
  (UC-08 A3) ; l'indicateur de partage se met à jour sans délai perceptible

- [AR-12 ; UC-08] retirer le partage d'un document → le document disparaît de la vue
  joueur en temps réel (disparition symétrique — AR-06) ;
  il reste épinglé dans le panneau du MJ

- [AR-12 ; UC-06] épingler un document sans le partager → le document est accessible
  rapidement dans le panneau des épinglés ; il reste invisible côté joueur

- [AR-12 ; UC-06] désépingler un document → le document quitte le panneau des épinglés ;
  son état de partage n'est pas modifié

- [UC-07] créer un document ou une note à la volée → le panneau de création rapide
  s'ouvre (titre seul obligatoire) ; voir fiche dédiée
  `docs/conception/interface/wireframes/sv-session/panneau-creation-rapide/panneau-creation-rapide.md`

- [UC-14 ; AR-11] rechercher dans le contenu de l'espace → titre seul au MVP ;
  résultats affichés dans le panneau latéral sans interrompre le contexte
  de la vue session

- [UC-06 ; RB-06-21] terminer la session → la session passe au statut terminée ;
  la machine d'états est unidirectionnelle (pas de retour en LIVE)

- [UC-06 ; UC-11 A4 ; AR-10] générer le lien de SESSION PONCTUEL →
  le MJ génère depuis la vue session le GuestAccess SESSION (AR-10) :
  lien temporaire donnant accès à la session en cours uniquement ;
  acte de pilotage en séance, logé dans cet écran (AR-18) ;
  le lien permanent de campagne relève de la vue campagne — hors périmètre de cet écran
```

---

### États

```
état vide (mode configuration, aucune session active, aucune configuration enregistrée) :
  la vue s'affiche avec le défaut épuré — zone de notes visible, panneaux de dossiers
  absents ou réduits au minimum ; invite à configurer ou à lancer directement (AR-04)

état chargé (configuration enregistrée, mode quelconque) :
  les panneaux de dossiers sélectionnés s'affichent selon la configuration persistée
  (AR-09 §disposition persistée) ; la zone de notes est co-présente ; l'indicateur
  de statut reflète le mode courant

état erreur (perte de connexion en mode cloud) :
  la notification d'état de synchronisation apparaît de façon non bloquante, sans
  couvrir la vue session (châssis S7 §Notification d'état de synchronisation ;
  NFR-OFF-04 ; RB-06-14) ; le MJ peut continuer à saisir des notes — elles sont
  conservées en brouillon local jusqu'à la reconnexion ; la navigation dans les
  dossiers reste disponible
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la vue session MJ.

```
annonce sans action :
  - changement de mode (configuration → LIVE → consultation CLOSED) : annoncé
    assistivement sans que le MJ déplace son focus — NFR-ACC-02
  - note créée via le panneau de création rapide : annoncé assistivement — NFR-ACC-02
  - basculement de visibilité d'un document (partagé / non partagé) :
    annoncé assistivement — NFR-ACC-02 ;
    note : l'annonce côté vue joueur de l'apparition du document partagé est
    distincte (AR-06 ; NFR-ACC-02) — cette fiche ne rend pas la vue joueur
    (NFR-CONF-01)
  - apparition de résultats de recherche dans le panneau latéral :
    annoncé assistivement — NFR-ACC-02
  - changement d'état de synchronisation (reconnexion, stockage sous pression) :
    annoncé assistivement — châssis S7 §Notification ; NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — indicateur de statut de session (mode courant lisible à distance
               de la table — NFR-ACC-04 : tailles de texte adaptées à la lecture
               rapide en session)
  priorité 2 — zone de notes (co-présente, accès immédiat)
  priorité 3 — panneaux de dossiers (contenu mis en avant par le MJ)
  priorité 4 — panneau des documents épinglés
  priorité 5 — barre de recherche
  source : NFR-ACC-04 (usage à distance normale de l'écran pendant une session à table)
```

---

### Sources

```
Sources : UC-06 ; UC-06 E3 (résumé et notes rétroactives en CLOSED) ; UC-07 ; UC-08 ;
          UC-11 ; UC-14 ;
          US-06-01 à US-06-10 ; UJ-UC-06 ;
          AR-04 ; AR-06 ; AR-07 ; AR-09 ; AR-10 ; AR-11 ; AR-12 ; AR-18 ; AR-19 ;
          NFR-ACC-02 ; NFR-ACC-04 ; NFR-OFF-04 ; NFR-PERF-04 ;
          RB-06-14 ; RB-06-21 ;
          domaine session-conduct.md (`Session.summary` ; `UpdateSummary(text)`) ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Modes (déclenché — AR-09 : l'écran a 3 modes)

---

#### Mode : configuration

```
Déclencheur : aucune session en cours dans cet espace ;
              le MJ arrive sur la vue session depuis la vue campagne
              (ou depuis la vue session déjà configurée sans avoir lancé)

Zones actives :
  - indicateur de statut de session (affiche : « aucune session active »)
  - zone de notes (accessible pour prise de notes préparatoires)
  - panneaux de dossiers (configurables — AR-04 ; US-06-02 :
    configuration accessible sans lancer de session)
  - barre de recherche

Zones désactivées :
  [DÉSACTIVÉ] indicateur de partage — condition de réactivation : passage en mode LIVE
  [DÉSACTIVÉ] affordance de partage de document — condition : passage en mode LIVE

Zones absentes par nature :
  [ABSENT PAR NATURE] affordance « Terminer la session » — il n'y a pas de session
    en cours à terminer
  [ABSENT PAR NATURE] zone de résumé de session — `Session.summary` n'est modifiable
    qu'en état CLOSED (`UpdateSummary(text)`, domaine session-conduct.md)

Affordances spécifiques :
  - [AR-09 ; UC-06 §Déclencheur] lancer la session → basculement en mode LIVE
  - [AR-04 ; US-06-02] configurer les panneaux → sélection et ordonnancement des
    dossiers mis en avant ; configuration persistée entre les modes (AR-09)
```

---

#### Mode : LIVE

```
Déclencheur : le MJ a lancé la session depuis le mode configuration ou depuis
              la vue campagne ; la session est au statut en cours

Zones actives : toutes les zones listées dans « Zones et hiérarchie »
  (y compris indicateur de partage et toutes affordances de partage) ;
  les règles de densité AR-19 s'appliquent (plancher garanti, plafond d'éviction,
  priorité d'éviction — voir zone RÈGLES DE DENSITÉ ci-dessus)

Zones absentes par nature :
  [ABSENT PAR NATURE] zone de résumé de session — `Session.summary` n'est modifiable
    qu'en état CLOSED (`UpdateSummary(text)`, domaine session-conduct.md)

Affordances spécifiques :
  - [AR-12 ; UC-08] partager un document → visible par les joueurs (PUBLIC) +
    auto-épinglage déclenché (UC-08 A3 — sens unique : partage → épingle,
    jamais l'inverse) ; l'indicateur de partage se met à jour sans délai
  - [AR-12 ; UC-08] retirer le partage → disparition symétrique côté joueur
    en temps réel (AR-06) ; le document reste épinglé dans le panneau du MJ
  - [UC-07] créer à la volée → panneau de création rapide (titre seul obligatoire) ;
    voir fiche `panneau-creation-rapide.md`
  - [UC-06 ; RB-06-21] terminer la session → basculement en mode consultation CLOSED ;
    unidirectionnel (pas de retour en LIVE — machine d'états RB-06-21)
  - [UC-06 ; UC-11 A4 ; AR-10 ; AR-18] générer le lien de SESSION PONCTUEL →
    GuestAccess SESSION (lien temporaire, limité à la session en cours) ;
    la configuration de la vue session se fait dans le mode configuration de CET écran
    (AR-18) — paramètres-campagne pointe vers cet écran, ne duplique pas

Note sur la frontière de confidentialité :
  L'annonce de l'apparition d'un document partagé côté vue joueur est gérée par
  la vue joueur (AR-06 ; NFR-ACC-02) — cette fiche ne dessine pas la vue joueur
  (NFR-CONF-01).
```

---

#### Mode : consultation CLOSED

```
Déclencheur : le MJ a terminé la session ; la session est au statut terminée ;
              la vue reste accessible pour annotations rétroactives

Zones actives :
  - indicateur de statut de session (affiche : « session terminée »)
  - zone de notes (annotations rétroactives MJ possibles en CLOSED)
  - zone de résumé de session (modification du résumé narratif de la séance —
    `Session.summary`, `UpdateSummary(text)` — UC-06 E3)
  - panneaux de dossiers (navigation possible, consultation uniquement)
  - panneau des documents épinglés (consultation uniquement)
  - barre de recherche

Zones désactivées :
  [DÉSACTIVÉ] affordance de partage — condition de réactivation : aucune
    (une session terminée ne repasse pas en LIVE — RB-06-21)

Zones absentes par nature :
  [ABSENT PAR NATURE] affordance « Lancer la session » — la machine d'états est
    unidirectionnelle ; une session terminée ne retourne pas en cours (RB-06-21)
  [ABSENT PAR NATURE] auto-épinglage au partage — l'auto-épinglage (UC-08 A3)
    est réservé au mode LIVE
  [ABSENT PAR NATURE] auto-épinglage à la création rétroactive — en mode CLOSED,
    le document créé à la volée est rangé dans l'espace sans être épinglé
    automatiquement ; cohérent avec « partage rétroactif sans auto-épinglage » ;
    voir fiche `panneau-creation-rapide.md` (tranché)

Affordances spécifiques :
  - [UC-07 ; S4 §Vue session MJ] créer un document ou une note à la volée de façon
    rétroactive → le panneau de création rapide s'ouvre (titre seul obligatoire) ;
    voir fiche `panneau-creation-rapide.md` (UC-07 préconditions A5)
  - [UC-08 A3 ; AR-12] partager un document rétroactivement → le document devient
    visible par les joueurs (PUBLIC) SANS auto-épinglage (l'auto-épinglage est
    réservé au mode LIVE — UC-08 A3)
  - [UC-06 E3 ; domaine session-conduct.md] modifier le résumé de la session →
    le MJ rédige ou modifie le résumé narratif libre de la séance (`Session.summary`,
    via `UpdateSummary(text)`) ; distinct des notes de session rétroactives
  - [UC-06 ; glossaire §Session] archiver la session → la session passe au statut
    archivée, lecture seule complète (aucune modification possible)

Éléments différés (S8) :
  [HORS-MVP — moscow.md §Won't Have] clôture formelle de session (procédure guidée)
```

---

### Partage / visibilité (déclenché — AR-03 et AR-12 : la vue session MJ touche la frontière joueur)

```
Règle de séparation : la vue session MJ n'expose aucune structure MJ à la vue joueur ;
  les documents non partagés (GM_ONLY, PLAYER_PRIVATE) sont invisibles depuis la
  vue joueur, quelle que soit la tentative d'accès — NFR-CONF-01 ; AR-03.
  Cette fiche ne rend jamais la vue joueur — la frontière de confidentialité est
  une garantie de conception, pas un filtre.

Indicateur de partage :
  Récapitule en permanence la liste des documents qui sont en ce moment visibles
  par les joueurs (PUBLIC). Il est distinct du panneau des documents épinglés :
  un document épinglé peut ne pas être partagé, et un document partagé s'auto-épingle
  en LIVE sans qu'un épinglage manuel ait eu lieu.
  Source : AR-12 ; châssis S7 §Indicateur de partage.

Affordances de partage et d'épinglage :
  - [AR-12 ; UC-08] partager → le document devient visible par les joueurs (PUBLIC) ;
    en mode LIVE, l'auto-épinglage est déclenché (sens unique : partage → épingle,
    jamais l'inverse — UC-08 A3)
  - [AR-12 ; UC-06] épingler → le document est ajouté au panneau des épinglés ;
    l'épinglage seul ne rend pas le document visible côté joueur
  - [AR-12] distinction partager ≠ épingler : deux affordances visuellement distinctes
    sur chaque document ; l'indicateur de partage reflète uniquement ce qui est PUBLIC ;
    le panneau des épinglés reflète uniquement ce qui est épinglé — les deux états
    sont indépendants et co-visibles sur la surface MJ
```

---

### Mode local (déclenché — surface MJ)

```
Bandeaux présents (châssis standard — voir zoning.md §S7 §Châssis mode local) :
  - bandeau de durabilité : non bloquant ; signale que les données locales ne
    bénéficient pas d'une garantie de conservation permanente ;
    propose la création d'un compte
  - bandeau de confidentialité : non bloquant ; signale l'absence de protection
    par identifiants ; propose la création d'un compte

Fonctions cloud désactivées sur la vue session MJ en mode local :
  - [AR-13 ; UC-01 A1] partager des documents avec les joueurs → invite contextuelle :
    « Partager avec des joueurs nécessite un compte — créer un compte en un geste »
  - [AR-13] génération du lien d'invitation → même invite contextuelle

Fonctions disponibles localement (non désactivées) :
  La prise de notes (zone de notes), la navigation dans les dossiers locaux,
  la recherche, l'épinglage et la création à la volée restent disponibles
  sans compte — elles ne nécessitent pas de connexion au service cloud.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-11 scénario nominal] écran Membres complet
    (révocations, associations joueur-personnage)
  [HORS-MVP — UC-08 §Règles métier ; vision §5bis] partage sélectif par joueur
    ou personnage
  [HORS-MVP — moscow.md §Won't Have] clôture formelle de session
  [HORS-MVP — moscow.md §Won't Have] table visuelle

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur] la notification active
    (signalement sonore ou visuel du partage côté joueur) n'est pas décidée
    pour le MVP — angle d'interview (US-06 §Questions ouvertes)
  [SOUS-SPÉCIFIÉ — S9 §Persistance des notes invité inter-sessions] la mécanique
    de récupération des notes personnelles joueur d'un invité via un nouveau lien
    est évoquée dans UC-06 §Règles métier mais non entièrement spécifiée —
    trou de corpus
```

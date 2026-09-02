# Tableau de bord des espaces de jeu

> Fiche de description d'écran basse-fidélité — entrée espace MJ.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..22`.

---

## En-tête de fiche

```
Nom          : Tableau de bord des espaces de jeu
Surface      : MJ
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type — il liste tous les types)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-02 postcondition ; UC-06 A5 ; UC-01 (espace personnel hors quota — RB-02-10) ;
               UC-02 §Règles métier RB-02-22, RB-02-23 (désarchivage) ;
               AR-17 ; AR-15 ; AR-05 ; AR-20 ; AR-22 (accès aux espaces archivés)
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ retrouve l'ensemble de ses espaces de jeu en un seul endroit et passe
            de l'un à l'autre sans rupture — l'espace personnel, toujours accessible
            hors quota, est positionné en tête et visuellement distinct des espaces
            partagés ; une session en cours dans un espace partagé se reprend en un clic.
```

---

### Zones et hiérarchie

```
[ GRILLE DE CARTES-ESPACES ]
  type     : principal
  rôle     : affiche tous les espaces de jeu du MJ sous forme de grille de cartes ;
             la carte de l'espace personnel figure EN TÊTE, visuellement distincte
             des cartes d'espaces partagés (traitement graphique différencié — badge,
             séparateur, ou section dédiée selon le rendu) ; les cartes d'espaces
             partagés suivent, ordonnées selon la date de dernier accès ;
             chaque carte présente au minimum le nom de l'espace et, pour les espaces
             partagés, le type (CAMPAIGN / ONE_SHOT)
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-02 §Postconditions ; AR-17 ; AR-05

[ CARTE ESPACE PERSONNEL ]
  type     : principal
  rôle     : carte toujours présente en tête de grille, visuellement distincte des
             cartes d'espaces partagés ; porte le badge « hors quota » pour signifier
             qu'elle n'est jamais comptabilisée dans le quota FREE ; jamais bloquée
             ni grisée — même si le quota d'espaces partagés est atteint ;
             micro-copy d'intention : « Vos notes et contenus, hors campagne »
             (neutre — sert la capture-first et le foyer de contenus réutilisables
             sans nommer les deux modèles mentaux ; ne présente pas l'espace personnel
             comme une campagne ; se découvre par l'usage, non par un texte d'onboarding)
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-17 (hors quota, jamais bloqué) ; AR-05 (libellé « Espace personnel ») ;
             AR-15 (capture-first — micro-copy compatible) ;
             UC-01 RB-02-10 ; UC-02 §Postconditions

[ REPÈRE "SESSION EN COURS" ]
  type     : principal
  rôle     : signale, sur la carte d'un espace partagé, qu'une session est actuellement
             au statut en cours dans cet espace ; visuellement saillant — priorité 1
             de lecture parmi les cartes d'espaces partagés ; permet la reprise en un
             clic vers le mode LIVE de la vue session, sans reconfiguration
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-09 ; UC-06 A5 (reprise depuis le tableau de bord)

[ COMPTEUR D'ESPACES PARTAGÉS ]
  type     : principal
  rôle     : indique au MJ combien d'espaces partagés (campagnes et one-shots) il a créés
             sur le quota disponible à son niveau de compte (ex. « 2 / 3 ») ;
             ce compteur ne porte QUE sur les espaces partagés ACTIFS — l'espace
             personnel n'entre jamais dans ce décompte (AR-17), et un espace archivé
             n'y entre pas davantage : le quota et sa garde ne portent que sur les
             espaces actifs de type campagne ou one-shot (`space-management.md`
             invariant 6 ; règle métier 12 — même décompte pour la création et pour
             le désarchivage)
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-17 ; UC-01 RB-02-10

[ CARTE "+ CRÉER UNE CAMPAGNE" ]
  type     : principal
  rôle     : carte d'action positionnée parmi les espaces partagés ; point d'entrée
             vers la création d'un espace partagé (campagne ou one-shot) ;
             désactivée si le quota d'espaces partagés est atteint, avec une invite
             contextuelle expliquant le blocage et proposant une action
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-02 scénario nominal ; AR-17 (condition de blocage quota)

[ ACCÈS AUX ESPACES ARCHIVÉS ]
  type     : latéral
  rôle     : point d'entrée séparé, distinct de la grille de cartes-espaces, vers la
             liste des espaces archivés du MJ (campagnes et one-shots archivés) ;
             un espace archivé n'apparaît PAS dans la grille de cartes-espaces
             (§Grille de cartes-espaces) — il ne se confond ni avec un espace actif
             ni avec l'espace personnel, et n'entre pas dans le compteur d'espaces
             partagés (§Compteur d'espaces partagés) ; l'espace personnel n'y figure
             jamais — il n'est pas archivable (invariant 15 de `space-management.md` ;
             AR-14)
  priorité : secondaire-configurable (registre replié — action rare — AR-19)
  visibilité : MJ seul
  ancrage  : AR-22 ; UC-02 §Règles métier RB-02-22

[ LISTE DES ESPACES ARCHIVÉS ]
  rôle     : révélée depuis l'accès aux espaces archivés ; présente chaque espace
             archivé du MJ (nom, type) ; un clic sur une entrée ouvre l'espace archivé
             en lecture seule, dans l'état où il était au moment de l'archivage
             (`space-management.md` règle métier 7 — lecture seule tant qu'`ARCHIVED` ;
             `content-library.md` — `SpaceArchived` passe les documents en lecture
             seule) ; le MJ y retrouve ses paramètres de campagne, où vit le geste de
             désarchivage (AR-22 ; `parametres-campagne.md` §Modes §Mode : espace
             archivé)
  type     : latéral
  priorité : secondaire-configurable (repliée par défaut — divulgation progressive,
             action rare — AR-19)
  visibilité : MJ seul
  ancrage  : AR-22 ; UC-02 §Règles métier RB-02-22
```

---

### Ce que l'utilisateur peut faire

```
- [AR-17 ; AR-05 ; UC-01 RB-02-10] accéder à l'espace personnel → clic sur la carte
  espace personnel (en tête de grille) ; la préparation de l'espace personnel s'ouvre ;
  toujours disponible, jamais bloquée ni grisée, indépendamment du quota d'espaces
  partagés

- [UC-02 §Postconditions] accéder à un espace partagé → clic sur la carte de l'espace ;
  la vue campagne de l'espace s'ouvre ; le MJ entre dans l'espace de travail sans rupture

- [AR-09 ; UC-06 A5] reprendre une session en cours → clic sur le repère « session en
  cours » d'une carte d'espace partagé ; la vue session de l'espace concerné s'ouvre
  directement en mode LIVE ; aucune reconfiguration nécessaire

- [UC-02 scénario nominal] créer un espace de jeu → clic sur la carte
  « + Créer une campagne » ; redirige vers l'écran de création d'espace ;
  action disponible uniquement si le quota d'espaces partagés n'est pas atteint

- [AR-17 ; UC-01 §Scénarios alternatifs] tenter de créer un espace alors que le quota
  est atteint → invite contextuelle non bloquante expliquant la limite et proposant
  la création d'un compte (si mode local) ou le passage à l'offre supérieure
  (si compte gratuit saturé)

- [AR-22 ; UC-02 §Règles métier RB-02-22] consulter la liste des espaces archivés →
  ouvre la liste des espaces archivés du MJ (campagnes et one-shots) ; distincte de
  la grille de cartes-espaces

- [AR-22 ; UC-02 §Règles métier RB-02-22] ouvrir un espace archivé → l'espace s'ouvre
  en lecture seule, dans l'état où il était au moment de l'archivage ; le geste de
  désarchivage lui-même vit sur les paramètres de campagne de cet espace, pas ici
  (AR-22 ; `parametres-campagne.md` §Modes §Mode : espace archivé)
```

---

### États

```
état vide (aucun espace partagé créé) :
  la grille affiche la carte espace personnel en tête (badge « hors quota »,
  micro-copy « Vos notes et contenus, hors campagne ») et la carte
  « + Créer une campagne » ; le compteur d'espaces partagés affiche « 0 / 3 »
  (ou selon le niveau de compte) ; aucun repère « session en cours » présent ;
  l'accès aux espaces archivés n'est pas présenté s'il n'existe aucun espace archivé
  (Famille 7 — absent par nature, pas désactivé : rien à consulter)
  Note AR-15 : cet état vide est un état de navigation (le MJ a un compte ou revient
  après une première session) — il ne constitue PAS l'écran d'atterrissage de la
  première connexion, qui est la surface de capture de l'espace personnel (AR-15) ;
  le tableau de bord vide et l'atterrissage capture-first sont deux surfaces distinctes

état chargé (au moins un espace partagé créé) :
  la grille affiche la carte espace personnel en tête, puis les cartes d'espaces partagés
  avec leur nom, leur type et, le cas échéant, le repère « session en cours » ;
  le compteur d'espaces partagés reflète le nombre réel ; la carte
  « + Créer une campagne » est présente et active si le quota n'est pas atteint,
  désactivée avec invite contextuelle si le quota est atteint ; l'accès aux espaces
  archivés apparaît dès qu'au moins un espace archivé existe (AR-22)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique au tableau de bord.

```
annonce sans action :
  - apparition du repère "session en cours" sur un espace (si une session passe au
    statut en cours pendant que le MJ est sur le tableau de bord) : annoncé
    assistivement sans que le MJ déplace son focus — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — repère « session en cours » (état actif immédiatement perceptible
               sur la carte de l'espace concerné)
  priorité 2 — carte espace personnel en tête de grille (présence permanente,
               badge hors quota, visuellement distinct)
  priorité 3 — grille des espaces partagés
  priorité 4 — compteur d'espaces partagés et carte création
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-02 §Postconditions ; UC-06 A5 ; UC-01 ; RB-02-10 ;
          UC-02 §Règles métier RB-02-22, RB-02-23 ;
          AR-01 ; AR-05 ; AR-09 ; AR-15 ; AR-17 ; AR-20 ; AR-22 ;
          domaine `space-management.md` (règle métier 7, invariant 6, règle métier 12) ;
          NFR-ACC-02 ;
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

Fonctions cloud désactivées sur le tableau de bord en mode local :
  Châssis standard — voir S7 §Châssis mode local.
  Aucune fonction spécifique au tableau de bord n'est cloud-dépendante au-delà
  du standard du châssis : la liste des espaces locaux, l'accès à l'espace
  personnel et la création d'un espace partagé local restent disponibles
  sans compte.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-02 A1 §périmètre post-MVP] point d'entrée one-shot dédié
    (« Lancer un one-shot ») — au MVP le one-shot se crée via le parcours campagne
    nominal avec type = ONE_SHOT

Éléments sous-spécifiés (S9) :
  aucun — les deux points précédemment sous-spécifiés sont résolus :
  - agencement général du tableau de bord : tranché — grille de cartes-espaces,
    espace personnel en tête visuellement distinct, espaces partagés avec compteur
    et repère session en cours (résolution intégrée dans les zones et états)
  - présence et présentation de l'espace personnel au tableau de bord : tranché —
    carte en tête de grille, badge hors quota, micro-copy neutre
    « Vos notes et contenus, hors campagne » ; n'est pas présenté comme une campagne ;
    se découvre par l'usage (AR-17 ; AR-05)
```

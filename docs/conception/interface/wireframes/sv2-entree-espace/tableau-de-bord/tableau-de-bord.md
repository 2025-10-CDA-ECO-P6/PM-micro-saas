# Tableau de bord des espaces de jeu

> Fiche de description d'écran basse-fidélité — entrée espace MJ.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.

---

## En-tête de fiche

```
Nom          : Tableau de bord des espaces de jeu
Surface      : MJ
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type — il liste tous les types)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-02 postcondition ; UC-06 A5 ; UC-01 (espace personnel hors quota — RB-01-03) ;
               AR-17 ; AR-15 ; AR-05 ; AR-20
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
             UC-01 RB-01-03 ; UC-02 §Postconditions

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
             ce compteur ne porte QUE sur les espaces partagés — l'espace personnel
             n'entre jamais dans ce décompte
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-17 ; UC-01 RB-01-03

[ CARTE "+ CRÉER UNE CAMPAGNE" ]
  type     : principal
  rôle     : carte d'action positionnée parmi les espaces partagés ; point d'entrée
             vers la création d'un espace partagé (campagne ou one-shot) ;
             désactivée si le quota d'espaces partagés est atteint, avec une invite
             contextuelle expliquant le blocage et proposant une action
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-02 scénario nominal ; AR-17 (condition de blocage quota)
```

---

### Ce que l'utilisateur peut faire

```
- [AR-17 ; AR-05 ; UC-01 RB-01-03] accéder à l'espace personnel → clic sur la carte
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
```

---

### États

```
état vide (aucun espace partagé créé) :
  la grille affiche la carte espace personnel en tête (badge « hors quota »,
  micro-copy « Vos notes et contenus, hors campagne ») et la carte
  « + Créer une campagne » ; le compteur d'espaces partagés affiche « 0 / 3 »
  (ou selon le niveau de compte) ; aucun repère « session en cours » présent
  Note AR-15 : cet état vide est un état de navigation (le MJ a un compte ou revient
  après une première session) — il ne constitue PAS l'écran d'atterrissage de la
  première connexion, qui est la surface de capture de l'espace personnel (AR-15) ;
  le tableau de bord vide et l'atterrissage capture-first sont deux surfaces distinctes

état chargé (au moins un espace partagé créé) :
  la grille affiche la carte espace personnel en tête, puis les cartes d'espaces partagés
  avec leur nom, leur type et, le cas échéant, le repère « session en cours » ;
  le compteur d'espaces partagés reflète le nombre réel ; la carte
  « + Créer une campagne » est présente et active si le quota n'est pas atteint,
  désactivée avec invite contextuelle si le quota est atteint
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
  source : NFR-ACC-04 (lecture rapide en contexte de session)
```

---

### Sources

```
Sources : UC-02 §Postconditions ; UC-06 A5 ; UC-01 ; RB-01-03 ;
          AR-01 ; AR-05 ; AR-09 ; AR-15 ; AR-17 ; AR-20 ;
          NFR-ACC-02 ; NFR-ACC-04 ;
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

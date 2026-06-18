# Tableau de bord des espaces de jeu

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 2 (entrée espace MJ).
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Tableau de bord des espaces de jeu
Surface      : MJ
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type — il liste tous les types)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-02 postcondition ; UC-06 A5 ; UC-01 (espace personnel hors quota — RB-01-03)
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ retrouve l'ensemble de ses espaces de jeu en un seul endroit et passe
            de l'un à l'autre sans rupture — l'espace personnel est toujours accessible,
            hors quota, et une session en cours dans un espace partagé se reprend en un clic.
```

---

### Zones et hiérarchie

```
[ LISTE DES ESPACES DE JEU ]
  type     : principal
  rôle     : affiche tous les espaces dont le MJ est propriétaire ou membre (espaces partagés
             et espace personnel) ; chaque entrée présente au minimum le nom de l'espace
             et son type (campagne / one-shot) pour permettre l'identification immédiate
  priorité : principal
  visibilité : MJ seul

[ ESPACE PERSONNEL ]
  type     : principal
  rôle     : entrée permanente de la liste, visuellement distincte des espaces partagés ;
             n'est jamais comptabilisée dans le quota FREE ni dans un compteur d'espaces ;
             toujours accessible, jamais bloquée ni grisée — même si le quota d'espaces
             partagés est atteint
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-17 ; UC-01 RB-01-03 ; UC-02 §Postconditions (l'espace personnel figure
             dans la liste) ; AR-01 (espace personnel hors quota, listé au tableau de bord)

[ REPÈRE "SESSION EN COURS" ]
  type     : principal
  rôle     : signale, sur l'entrée d'un espace partagé, qu'une session est actuellement
             au statut en cours dans cet espace ; permet la reprise en un clic vers
             le mode LIVE de la vue session, sans reconfiguration
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-09 ; UC-06 A5 (reprise depuis le tableau de bord)

[ COMPTEUR D'ESPACES PARTAGÉS ]
  type     : principal
  rôle     : indique au MJ combien d'espaces partagés (campagnes et one-shots) il a créés
             sur le quota disponible à son niveau de compte (ex. « 2 / 3 ») ;
             l'espace personnel n'entre jamais dans ce décompte
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-17 ; UC-01 RB-01-03

[ ACCÈS CRÉATION D'ESPACE ]
  type     : principal
  rôle     : point d'entrée vers la création d'un espace partagé (campagne ou one-shot) ;
             désactivé si le quota d'espaces partagés est atteint, avec une invite
             contextuelle expliquant le blocage et proposant une action
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-02 scénario nominal ; AR-17 (condition de blocage quota)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-02 §Postconditions] accéder à un espace partagé → la vue campagne de l'espace
  s'ouvre ; le MJ entre dans l'espace de travail sans rupture

- [AR-01 ; UC-01 RB-01-03] accéder à l'espace personnel → la préparation de l'espace
  personnel s'ouvre ; toujours disponible, indépendamment du quota d'espaces partagés

- [AR-09 ; UC-06 A5] reprendre une session en cours → la vue session de l'espace concerné
  s'ouvre directement en mode LIVE ; aucune reconfiguration nécessaire

- [UC-02 scénario nominal] créer un espace de jeu → redirige vers l'écran de création
  d'espace ; action disponible uniquement si le quota d'espaces partagés n'est pas atteint

- [AR-17 ; UC-01 §Scénarios alternatifs] tenter de créer un espace alors que le quota
  est atteint → invite contextuelle non bloquante expliquant la limite et proposant
  la création d'un compte (si mode local) ou le passage à l'offre supérieure
  (si compte gratuit saturé)
```

---

### États

```
état vide (aucun espace partagé créé) :
  le tableau de bord affiche l'espace personnel (toujours présent) et l'accès à la
  création d'un espace partagé ; aucun repère "session en cours" ; le compteur
  d'espaces partagés affiche « 0 / 3 » (ou selon le niveau de compte)
  [SOUS-SPÉCIFIÉ — S4 §Tableau de bord des espaces de jeu] l'agencement exact de
  cet état vide n'est pas décrit dans le corpus — UC-02 pose la postcondition
  sans décrire l'interface initiale
  Note AR-15 : cet état vide est un état de navigation (le MJ a un compte ou revient
  après une première session) — il ne constitue pas l'écran d'atterrissage de la
  première connexion, qui est la surface de capture de l'espace personnel (AR-15)

état chargé (au moins un espace partagé ou espace personnel avec contenu) :
  la liste des espaces s'affiche avec leur nom, leur type et, le cas échéant, le repère
  "session en cours" ; le compteur d'espaces partagés reflète le nombre réel ;
  l'espace personnel est présent et visuellement distinct
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
  priorité 1 — repère "session en cours" (état actif immédiatement perceptible)
  priorité 2 — liste des espaces partagés (accès principal)
  priorité 3 — espace personnel (présence permanente)
  priorité 4 — compteur d'espaces partagés et accès création
  source : NFR-ACC-04 (lecture rapide en contexte de session)
```

---

### Sources

```
Sources : UC-02 §Postconditions ; UC-06 A5 ; UC-01 ; RB-01-03 ;
          AR-01 ; AR-09 ; AR-15 ; AR-17 ;
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
  [SOUS-SPÉCIFIÉ — S4 §Tableau de bord des espaces de jeu] agencement général
    du tableau — UC-02 pose la postcondition (« depuis le tableau de bord le MJ
    retrouve l'ensemble des espaces… ») sans décrire l'interface ; S4 l'annote
    explicitement « Sous-spécifié dans le corpus »
  [SOUS-SPÉCIFIÉ — S9 §Interface de l'espace personnel sous-spécifiée]
    la présence de l'espace personnel au tableau de bord (perçue comme naturelle
    ou à expliquer ?) est un point d'interview — UC-02 §Questions à valider
    en interview
```

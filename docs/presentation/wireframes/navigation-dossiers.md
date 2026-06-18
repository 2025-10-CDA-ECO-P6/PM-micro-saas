# Navigation par dossiers

> Fiche de description d'écran basse-fidélité — Vague 3, sous-vague 3 (préparation espace partagé), cluster A.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Navigation par dossiers
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT (PERSONAL : présent avec structure réduite — dossier virtuel
               « Non classés » uniquement à la création, sans dossiers système nommés — AR-16)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-05 scénario nominal ; UC-04 ; AR-11 ; AR-16
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ parcourt l'arborescence des dossiers de son espace, visualise les documents
            de chaque dossier en vue condensée, et crée de nouveaux documents directement dans
            le dossier courant — en respectant la règle qu'un document appartient à exactement
            un dossier.
```

---

### Zones et hiérarchie

```
[ ARBORESCENCE DES DOSSIERS ]
  type     : principal
  rôle     : liste tous les dossiers de l'espace selon leur ordre d'affichage persisté ;
             pour un espace CAMPAIGN ou ONE_SHOT, inclut les dossiers système
             (Personnages, Joueurs, Scénarios, Notes) et les dossiers créés librement ;
             pour un espace PERSONAL, inclut les dossiers créés librement par le
             propriétaire (aucun dossier système nommé — AR-16) ;
             le dossier virtuel « Non classés » n'apparaît pas dans l'arborescence
             (UC-05 A4 évoque une vue dédiée — non spécifiée ;
             voir [SOUS-SPÉCIFIÉ] ci-dessous)
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-05 scénario nominal ; AR-11 ; AR-16

[ LISTE CONDENSÉE DES DOCUMENTS ]
  type     : principal
  rôle     : affiche les documents du dossier courant en vue condensée (titre et
             repère de type) ; l'affichage condensé permet de retrouver un document
             même dans un dossier dense ; chaque document est accessible par sélection
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-05 ; UC-04 ; AR-11 (épine dorsale + défaut informatif)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-05 scénario nominal] ouvrir un dossier → la liste condensée des documents
  du dossier sélectionné s'affiche dans la zone adjacente

- [UC-05 A3 ; UC-04] créer un document dans le dossier courant → l'éditeur de
  document s'ouvre ; le document est automatiquement rattaché au dossier actif ;
  s'il existe un template par défaut pour ce dossier, le document est initialisé
  avec ce template

- [UC-04] ouvrir un document existant → l'éditeur de document s'ouvre sur le
  contenu du document sélectionné

- [UC-05 A1] renommer un dossier → le dossier prend le nouveau nom ; les documents
  qu'il contient ne sont pas affectés ; renommage disponible pour tous les dossiers,
  y compris les dossiers système

- [UC-05 A2] réordonner les dossiers → le MJ modifie l'ordre d'affichage des
  dossiers dans l'arborescence ; l'ordre est persisté

- [UC-05 scénario nominal] créer un nouveau dossier → le MJ saisit un nom ;
  le dossier apparaît dans l'arborescence ; un template par défaut peut y être associé

- [UC-05 E2 ; UC-05 E3] supprimer un dossier → le MJ doit traiter les documents
  du dossier avant la suppression : les déplacer vers un autre dossier ou les
  laisser dans « Non classés » ; aucune suppression silencieuse de documents

- [UC-05 scénario nominal — déplacer un document] déplacer un document vers un
  autre dossier → le document quitte son dossier d'origine et rejoint le dossier
  cible ; son contenu n'est pas affecté (unicité d'appartenance — un document
  appartient à exactement un dossier — UC-05 §Règles métier ; AR-11)
```

---

### États

```
état vide (dossier courant sans document) :
  la liste condensée des documents s'affiche vide ; invite à créer un premier
  document dans ce dossier ; l'arborescence reste accessible

état chargé (dossier courant avec documents) :
  la liste condensée affiche les documents avec titre et repère de type ;
  l'arborescence reflète l'ordre d'affichage persisté des dossiers

état erreur (perte de connexion en mode cloud) :
  la notification d'état de synchronisation apparaît de façon non bloquante
  (châssis S7 §Notification d'état de synchronisation ; NFR-OFF-04) ;
  la navigation dans les dossiers et la lecture des documents restent
  disponibles depuis le cache local
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la navigation par dossiers.

```
annonce sans action :
  - ajout d'un document dans la liste condensée (création dans le dossier courant) :
    annoncé assistivement — NFR-ACC-02
  - déplacement d'un document vers ou depuis le dossier courant : annoncé
    assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — arborescence des dossiers (structure de l'espace)
  priorité 2 — liste condensée des documents du dossier courant
  source : NFR-ACC-04 (usage à distance normale de l'écran pendant la préparation)
```

---

### Sources

```
Sources : UC-04 ; UC-05 scénario nominal, A1, A2, A3, A4, E2, E3 ;
          AR-11 ; AR-16 ;
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

Châssis standard — voir S7 §Châssis mode local.
Aucune fonction de navigation par dossiers n'est cloud-dépendante :
  la création, la navigation, la lecture et la réorganisation des dossiers et
  documents sont disponibles sans compte.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-05 A2] réordonnancement par glisser-déposer — l'ordre est
    configurable au MVP ; le mécanisme exact (glisser-déposer vs interface
    numérique) n'est pas figé dans le wireframe

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Vue « Non classés »] la vue dédiée aux documents non classés
    est évoquée dans UC-05 A4 (*« vue 'Non classés' dédiée »*) mais son interface
    n'est pas décrite. Le dossier virtuel « Non classés » est invisible dans
    l'arborescence de navigation (content-library invariant 4) ; l'entrée UC-05 A4
    et S9 §Vue « Non classés » évoquent une vue dédiée accessible en dehors de
    l'arborescence — tension non tranchée. Ce point n'est pas décidé ici.
```

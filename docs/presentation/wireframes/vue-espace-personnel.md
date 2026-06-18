# Espace personnel

> Fiche de description d'écran basse-fidélité — Vague 3, sous-vague 4 (espace personnel).
> Démontre la règle d'absence au niveau section : les sections conditionnelles « Modes » et
> « Partage / visibilité » ne se déclenchent pas pour un espace PERSONAL — elles n'apparaissent
> pas dans cette fiche (AR-14 ; Famille 7 de conventions-wireframe.md).
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Espace personnel
Surface      : MJ
Contexte     : préparation
Type d'espace: PERSONAL
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-01, UC-04 ; AR-14, AR-15, AR-16, AR-17
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ accède à son foyer de contenu capturé hors campagne — il y crée, organise
            et retrouve des documents immédiatement, sans avoir à créer un espace partagé,
            dès la première ouverture de l'application (capture-first — UC-01 ; AR-15).
```

---

### Zones et hiérarchie

```
[ ZONE DE NAVIGATION PAR DOSSIERS ]
  type     : principal
  rôle     : affiche l'arborescence des dossiers de l'espace personnel — débutant avec
             le seul dossier virtuel « Non classés » à la création, sans aucun dossier
             système nommé (AR-16 — raison principale : « Non classés » seul pour
             PERSONAL, les dossiers Personnages/Joueurs/Scénarios/Notes présupposent
             un groupe absent d'un espace mono-membre) ; le propriétaire crée
             librement ses propres dossiers par la suite ; tout document appartient
             à exactement un dossier (AR-11 — unicité d'appartenance, transverse)
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-16 (raison principale — espace personnel sans dossiers système) ;
             AR-11 (unicité d'appartenance — transverse à tous les espaces) ;
             UC-04 ; UC-05 §espace personnel

[ ZONE DE CONTENU / LISTE DES DOCUMENTS ]
  type     : principal
  rôle     : liste les documents du dossier courant en vue condensée (titre et repère
             de type) ; permet la création d'un document dans le contexte courant ;
             accès à l'éditeur pour tout document existant
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-04 ; AR-11

[ ACCÈS À L'ÉDITEUR DE DOCUMENT ]
  type     : principal
  rôle     : point d'entrée vers l'éditeur pour créer ou modifier un document ;
             titre seul obligatoire à la création — le document est rattaché au
             dossier courant ; voir fiche dédiée `editeur-document.md`
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-04 ; AR-15 (capture immédiate — titre seul obligatoire)

[ BARRE DE RECHERCHE ]
  type     : châssis
  rôle     : accélérateur de navigation omniprésent dans l'espace personnel —
             titre seul au MVP ; les résultats s'ouvrent sans interrompre
             le contexte courant (AR-11)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-14 ; AR-11
```

---

### Ce que l'utilisateur peut faire

```
- [UC-01 ; AR-15] capturer un document immédiatement → l'éditeur s'ouvre
  avec le titre seul obligatoire ; le document est placé dans « Non classés »
  si aucun dossier n'est sélectionné ; aucune étape préalable requise

- [UC-04] créer un document dans le dossier courant → l'éditeur s'ouvre ;
  le document est automatiquement rattaché au dossier actif

- [UC-04] ouvrir un document existant → l'éditeur s'ouvre sur le contenu
  du document sélectionné

- [UC-05 §espace personnel ; AR-16] créer un dossier → le nouveau dossier
  est ajouté à l'arborescence ; le propriétaire choisit librement le nom
  et l'organisation (pas de contrainte de structure)

- [UC-05 §espace personnel] déplacer un document dans un dossier →
  le document est rattaché au dossier cible ; il quitte son dossier
  d'origine (unicité d'appartenance — AR-11)

- [UC-14 ; AR-11] rechercher dans le contenu de l'espace personnel →
  titre seul au MVP ; résultats affichés sans interrompre le contexte
  de navigation courant
```

---

### États

```
état vide (espace personnel fraîchement créé) :
  la zone de navigation affiche uniquement le dossier virtuel « Non classés »
  (AR-16 — aucun dossier système nommé) ; la zone de contenu invite à créer
  un premier document ; l'accès à l'éditeur est disponible immédiatement
  pour une capture sans friction (AR-15)

état chargé (espace personnel avec documents) :
  la zone de navigation affiche l'arborescence complète des dossiers créés par
  le propriétaire, plus « Non classés » ; la zone de contenu liste les documents
  du dossier courant en vue condensée ; l'éditeur est accessible pour tout
  document sélectionné
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'espace personnel.

```
annonce sans action :
  - document créé ou déplacé dans l'arborescence : annoncé assistivement
    sans déplacement de focus — NFR-ACC-02 (changements d'état dynamiques)
  - changement d'état de synchronisation (reconnexion, stockage sous pression) :
    annoncé assistivement — châssis S7 §Notification ; NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de navigation par dossiers (arborescence de l'espace personnel,
               repère du dossier courant — NFR-ACC-04)
  priorité 2 — zone de contenu / liste des documents
  priorité 3 — barre de recherche
  source : NFR-ACC-04 (usage à distance normale de l'écran pendant la préparation)
```

---

### Sources

```
Sources : UC-01 ; UC-04 ; UC-05 ;
          AR-11 ; AR-14 ; AR-15 ; AR-16 ; AR-17 ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          domaine space-management.md (invariant 13 : mono-membre, pas d'AddMember,
            pas de CreateInvitation) ;
          domaine session-conduct.md (pas de SessionViewConfig pour PERSONAL) ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Mode local (déclenché — surface MJ ; AR-15 : l'espace personnel est le conteneur par défaut en mode local)

```
Bandeaux présents (châssis standard — voir zoning.md §S7 §Châssis mode local) :
  - bandeau de durabilité : non bloquant ; signale que les données locales ne
    bénéficient pas d'une garantie de conservation permanente ;
    propose la création d'un compte
  - bandeau de confidentialité : non bloquant ; signale l'absence de protection
    par identifiants ; propose la création d'un compte

Fonctions cloud désactivées sur l'espace personnel en mode local :
  Châssis standard — voir S7 §Châssis mode local.
  Aucune fonction cloud spécifique à l'espace personnel n'est désactivée
  au-delà du standard châssis : la capture, la navigation, l'organisation
  par dossiers, la recherche et l'édition sont entièrement disponibles
  sans compte (AR-13 ; UC-01 §Règles métier).
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — AR-08 ; UC-13 §post-MVP] bibliothèque « Mes scénarios » (vue filtrée
    des documents isReusable = true, parcours « Rejouer », instanciation cross-espace,
    historique des runs) — se logera dans l'espace personnel sous forme de raccourci
    vers une vue filtrée interne, sans nouveau nœud d'arborescence

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9] présence de l'espace personnel au tableau de bord et libellé
    de surface exacts — points d'interview (UC-02 §Questions à valider en interview ;
    AR-05 §Condition de retour)
  [SOUS-SPÉCIFIÉ — S9] vue « Non classés » : interface de la vue dédiée au dossier
    virtuel de repli — non tranchée dans le corpus
  [SOUS-SPÉCIFIÉ — S9] sémantique ARCHIVED / FROZEN d'un espace PERSONAL — non
    tranchée à ce stade (AR-14 §Condition de retour)
```

# Création d'un espace de jeu

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 2 (entrée espace MJ).
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..20`.

---

## En-tête de fiche

```
Nom          : Création d'un espace de jeu
Surface      : MJ
Contexte     : transversal
Type d'espace: CAMPAIGN | ONE_SHOT (l'espace personnel préexiste — non créé via ce parcours)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-02 scénario nominal ; UC-02 §Postconditions ; UC-02 §Règles métier
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ crée un espace de jeu partagé (campagne ou one-shot) en saisissant
            a minima un nom, puis est redirigé vers son tableau de bord où l'espace
            et ses quatre dossiers système sont immédiatement disponibles.
```

---

### Zones et hiérarchie

```
[ FORMULAIRE DE CRÉATION D'ESPACE ]
  type     : formulaire
  rôle     : recueille les informations nécessaires à la création d'un espace partagé —
             le nom est le seul champ obligatoire ; la description et le système de jeu
             sont optionnels ; le type de l'espace (CAMPAIGN ou ONE_SHOT) est déterminé
             par le parcours d'entrée (au MVP : parcours campagne nominal pour les deux)
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-02 §Scénario nominal ; UC-02 §Données manipulées ; UC-02 §Règles métier
             (le nom est obligatoire pour les campagnes)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-02 §Scénario nominal] saisir le nom de l'espace → champ obligatoire ; la validation
  est bloquée si le nom est absent, avec un retour non bloquant sous le champ
  (UC-02 §Exceptions E1)

- [UC-02 §Scénario nominal] saisir la description → champ optionnel ; la validation
  n'est pas conditionnée à son remplissage

- [UC-02 §Scénario nominal] saisir le système de jeu → champ optionnel ; la validation
  n'est pas conditionnée à son remplissage

- [UC-02 §Scénario nominal] valider la création → l'espace est créé (CAMPAIGN ou ONE_SHOT)
  avec les quatre dossiers système (Personnages, Joueurs, Scénarios, Notes) créés
  automatiquement ; le MJ est redirigé vers son tableau de bord
  (UC-02 §Postconditions §Campagne)

- [UC-02 §Exceptions E2] erreur à la création → le formulaire reste affiché avec les
  données saisies intactes ; un retour d'erreur est affiché sans rechargement
```

---

### États

```
état vide (formulaire ouvert, aucune saisie) :
  le formulaire s'affiche avec le champ nom vide et actif ; les champs optionnels
  (description, système) sont présents mais non remplis ; la validation est inactive
  tant que le nom est absent

état chargé (nom saisi, formulaire prêt à valider) :
  le champ nom est rempli ; la validation est disponible ; les champs optionnels
  peuvent être remplis ou laissés vides sans conséquence sur la validation

état erreur (erreur de création — E2) :
  le formulaire reste affiché avec les données saisies intactes ;
  un retour d'erreur signale la cause sans bloquer la possibilité de re-soumettre
  (UC-02 §Exceptions E2)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran de création.

```
annonce sans action :
  - retour d'erreur sur le champ nom (validation bloquée si nom absent) : annoncé
    assistivement sans que le MJ déplace son focus — NFR-ACC-02
  - retour d'erreur de création (E2) : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — champ nom (seul champ obligatoire, naturellement en tête)
  priorité 2 — champs optionnels (description, système)
  priorité 3 — affordance de validation
  source : NFR-ACC-04 (lecture rapide, formulaire minimaliste)
```

---

### Sources

```
Sources : UC-02 §Scénario nominal ; UC-02 §Postconditions ; UC-02 §Règles métier ;
          UC-02 §Exceptions E1, E2 ; UC-02 §Données manipulées ;
          AR-01 ; AR-16 (dossiers système réservés à CAMPAIGN et ONE_SHOT) ;
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

Fonctions cloud désactivées sur cet écran :
  Châssis standard — voir S7 §Châssis mode local.
  La création d'un espace partagé est disponible en mode local ; aucune fonction
  spécifique à cet écran n'est cloud-dépendante.
  Source : AR-13 ; UC-01 §Règles métier (mode local : création d'un espace CAMPAIGN
  ou ONE_SHOT disponible sans compte) ; UC-01 §Scénario nominal.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8)

```
Éléments différés (S8) :
  [HORS-MVP — UC-02 A1 §périmètre post-MVP] parcours express one-shot dédié
    (point d'entrée « Lancer un one-shot » distinct, branche Sonia) —
    au MVP le one-shot se crée via ce même formulaire avec type = ONE_SHOT ;
    le parcours express et les deux points d'entrée distincts sont reportés post-MVP
    (arbitrage UC-13)
```

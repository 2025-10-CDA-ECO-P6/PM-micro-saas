# Gate de migration local→cloud

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 1 (transversaux), lot B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Gate de migration local→cloud
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran intervient lors de la création de compte — indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-01 A1 ; UC-10 §Scénario nominal (inscription depuis le mode local) ;
               UC-10 §Règles métier (migration des données locales)
```

---

## Noyau obligatoire

### Intention

```
Intention : au moment de la création de compte depuis le mode local, l'utilisateur
            prend connaissance des espaces locaux détectés — avec leur titre, leur type,
            leur historique de session, leur volume estimé et leur date — et donne sa
            confirmation explicite avant que la migration vers le cloud soit déclenchée ;
            le gate signale toute session en cours qui doit être clôturée avant migration,
            et avertit que la configuration de la vue session n'est pas reprise et devra
            être recréée.
```

---

### Zones et hiérarchie

```
[ ZONE DE PRÉSENTATION DES ESPACES LOCAUX DÉTECTÉS ]
  type     : principal
  rôle     : liste les espaces locaux détectés, espace par espace ; pour chaque espace :
             titre, type (campagne ou espace personnel), historique de session
             (sessions terminées, notes de session, documents épinglés, résumés),
             volume estimé, date de création ou de dernière modification ;
             chaque espace est présenté de façon individuelle car la migration
             est traitée espace par espace (UC-10 §Règles métier) ;
             si une session est au statut en cours dans un espace, cet espace
             est signalé avec une indication que la session doit être clôturée
             avant que cet espace puisse migrer
  priorité : co-présent-jamais-masqué
  visibilité : utilisateur en cours de création de compte depuis le mode local
  ancrage  : UC-01 A1 ; UC-10 §Scénario nominal étape 5 ; UC-10 §Règles métier

[ ZONE D'AVERTISSEMENT SUR LA CONFIGURATION DE VUE SESSION ]
  type     : bandeau
  rôle     : informe l'utilisateur que la configuration de la vue session
             (choix des panneaux affichés) n'est pas reprise lors de la migration ;
             cette configuration est recréée — le MJ la reconfigure après migration ;
             l'avertissement est non bloquant et présent dès l'affichage du gate
  priorité : co-présent-jamais-masqué
  visibilité : utilisateur en cours de création de compte depuis le mode local
  ancrage  : UC-10 §Règles métier (migration — config vue session recréée) ;
             S4 §Gate de migration local→cloud

[ ZONE DE CONFIRMATION EXPLICITE ]
  type     : formulaire
  rôle     : recueille la confirmation explicite de l'utilisateur avant le déclenchement
             de la migration ; la confirmation porte sur l'ensemble des espaces listés ;
             aucune migration ne commence sans cette confirmation (UC-10 §Préconditions ;
             ADR-016 §4 — gate de reconnaissance anti-appropriation) ;
             la zone de confirmation n'est accessible que si aucune session en cours
             ne subsiste dans les espaces listés, ou si les espaces concernés ont été
             exclus du lot à migrer
  priorité : principal
  visibilité : utilisateur en cours de création de compte depuis le mode local
```

---

### Ce que l'utilisateur peut faire

```
- [UC-01 A1 ; UC-10 §Scénario nominal étape 5] consulter les espaces locaux détectés
  → l'utilisateur prend connaissance des espaces, de leur historique de session, du
  volume estimé et de la date, espace par espace, avant toute action

- [UC-10 §Règles métier] identifier les sessions en cours qui bloquent la migration
  d'un espace → le gate signale clairement quel espace contient une session en cours
  (statut en cours) et indique que cette session doit être clôturée avant que cet espace
  puisse migrer ; l'utilisateur peut choisir de clôturer la session, puis revenir

- [UC-01 A1 ; UC-10 §Scénario nominal étape 6] confirmer la migration → après lecture
  des espaces détectés et de l'avertissement sur la configuration de vue session,
  l'utilisateur confirme ; la migration démarre espace par espace, tout-ou-rien par
  espace ; les espaces migrés avec succès sont accessibles dans l'espace de travail
  cloud ; les espaces rejetés font l'objet d'un rapport de rejets et leurs données
  locales restent intactes (UC-10 E5)
```

---

### États

```
état nominal (espaces locaux détectés, aucune session en cours) :
  la zone de présentation des espaces locaux s'affiche avec l'ensemble des espaces
  détectés et leur historique ; la zone d'avertissement sur la configuration de vue
  session est visible ; la zone de confirmation est accessible ; l'utilisateur peut
  confirmer la migration

état bloquant partiel (au moins un espace contient une session en cours) :
  la zone de présentation des espaces signale le ou les espaces bloqués avec indication
  que la session doit être clôturée avant migration ; la zone de confirmation est
  accessible uniquement pour les espaces sans session en cours, ou après clôture de
  toutes les sessions en cours — selon la granularité de sélection ;
  [SOUS-SPÉCIFIÉ — S4 §Gate de migration local→cloud] agencement et granularité de
  la sélection des espaces à migrer : non tranché dans le corpus

état post-migration (rapport de rejets) :
  si des espaces ont été rejetés lors de la migration (UC-10 E5), un rapport de rejets
  est présenté, indiquant la raison par espace ; les données locales des espaces rejetés
  restent intactes ; les espaces migrés avec succès sont accessibles dans le tableau
  de bord cloud
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique au gate de migration local→cloud.

```
annonce sans action :
  - apparition du rapport de rejets en fin de migration : annoncée assistivement sans
    que l'utilisateur déplace son focus — NFR-ACC-02
  - signalement d'un espace bloqué par une session en cours : annoncé assistivement
    lors de l'affichage initial — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de présentation des espaces locaux détectés
               (contenu principal du gate — décision à prendre)
  priorité 2 — zone d'avertissement sur la configuration de vue session
               (information non bloquante mais structurante)
  priorité 3 — zone de confirmation explicite
  source : NFR-ACC-04
```

---

### Sources

```
Sources : UC-01 A1 ; UC-10 §Scénario nominal (inscription depuis le mode local, étapes 5–6) ;
          UC-10 E5 (échec ou interruption de migration) ;
          UC-10 §Règles métier (migration des données locales — espace par espace,
          tout-ou-rien par espace, session LIVE à clôturer, config vue session recréée) ;
          S4 §Gate de migration local→cloud ; S3 §Gate de migration local→cloud ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-01 A4b §Post-MVP] réimport manuel d'un fichier de sauvegarde :
    le réimport de sauvegarde est post-MVP ; le gate de migration ne couvre que le
    chemin local→cloud par création de compte

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S4 §Gate de migration local→cloud]
    agencement des espaces détectés sur l'écran : non tranché dans le corpus
  [SOUS-SPÉCIFIÉ — S4 §Gate de migration local→cloud]
    granularité de la sélection des espaces à migrer (tout sélectionner par défaut,
    ou sélection individuelle) : non tranché dans le corpus ; la règle espace par espace
    (UC-10 §Règles métier) pose le traitement unitaire, pas la granularité de la sélection
```

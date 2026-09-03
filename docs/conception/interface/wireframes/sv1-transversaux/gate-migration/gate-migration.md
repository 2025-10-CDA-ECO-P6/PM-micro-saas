# Gate de migration local→cloud

> Fiche de description d'écran basse-fidélité — écrans transversaux.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

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
[ ZONE DE PRÉSENTATION ET SÉLECTION DES ESPACES LOCAUX DÉTECTÉS ]
  type     : principal
  rôle     : liste les espaces locaux détectés sous forme de liste à sélection
             individuelle — chaque espace est coché par défaut (tout coché à
             l'ouverture du gate) ; l'utilisateur peut décocher des espaces
             individuellement pour les exclure du lot à migrer ;
             la granularité est par espace (UC-10 §Règles métier : migration
             espace par espace, tout-ou-rien par espace — la sélection est
             globale par espace, pas par document) ;
             la modalité de sélection individuelle (liste à cases, tout coché
             par défaut) est un choix de maquette — le corpus ne prescrit que
             le tout-ou-rien par espace, pas la modalité de sélection ;
             la granularité globale de la migration est « lot unique »
             (décision produit du 2026-07-09 — tous les espaces éligibles
             migrent en un seul passage, confirmation unique) ; l'exclusion
             ponctuelle d'un espace n'est pas un parcours nominal — elle
             relève du chemin d'échec/reprise E5 ;
             pour chaque espace : titre, type (campagne ou espace personnel),
             historique de session (sessions terminées, notes de session,
             documents épinglés, résumés), volume estimé, date de création
             ou de dernière modification ;
             un espace contenant une session au statut en cours est affiché
             DÉSACTIVÉ (case non cochable) avec le motif inline
             « session à clôturer » ; l'utilisateur doit clôturer la session
             depuis la vue session avant de pouvoir inclure cet espace
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

[ ZONE DE CONFIRMATION EXPLICITE GLOBALE ]
  type     : formulaire
  rôle     : recueille la confirmation explicite de l'utilisateur avant le déclenchement
             de la migration ; la confirmation porte sur l'ensemble des espaces cochés
             dans la sélection ; aucune migration ne commence sans cette confirmation
             (UC-10 §Préconditions ; ADR-016 §4 — gate de reconnaissance
             anti-appropriation) ;
             la zone de confirmation est accessible uniquement si aucun des espaces
             cochés ne contient de session en cours (les espaces désactivés avec
             motif « session à clôturer » ne bloquent pas la confirmation si
             l'utilisateur les a décochés)
  priorité : principal
  visibilité : utilisateur en cours de création de compte depuis le mode local
```

---

### Ce que l'utilisateur peut faire

```
- [UC-01 A1 ; UC-10 §Scénario nominal étape 5] consulter et sélectionner les espaces
  locaux détectés → l'utilisateur prend connaissance des espaces, de leur historique
  de session, du volume estimé et de la date, espace par espace, avant toute action ;
  tous les espaces sont cochés par défaut ; l'utilisateur peut décocher des espaces
  individuellement pour les exclure de la migration

- [UC-10 §Règles métier] identifier les sessions en cours qui bloquent la migration
  d'un espace → les espaces contenant une session en cours (statut en cours) sont
  affichés DÉSACTIVÉS avec le motif inline « session à clôturer » ; l'utilisateur
  peut choisir de clôturer la session depuis la vue session puis revenir,
  ou décocher l'espace pour l'exclure du lot et poursuivre la migration des autres

- [UC-01 A1 ; UC-10 §Scénario nominal étape 6] confirmer la migration → après lecture
  des espaces cochés et de l'avertissement sur la configuration de vue session,
  l'utilisateur confirme ; la confirmation porte sur l'ensemble du lot coché ;
  la migration démarre espace par espace, tout-ou-rien par espace ; les espaces
  migrés avec succès sont accessibles dans l'espace de travail cloud ; les espaces
  rejetés font l'objet d'un rapport de rejets et leurs données locales restent
  intactes (UC-10 E5)
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
  le ou les espaces bloqués s'affichent DÉSACTIVÉS dans la liste avec le motif inline
  « session à clôturer » ; ils ne peuvent pas être cochés tant que la session n'est
  pas clôturée ; la zone de confirmation reste accessible si l'utilisateur a décoché
  tous les espaces bloqués (ou si les espaces bloqués ont été clôturés) — la migration
  peut démarrer sur le sous-lot coché sans attendre les espaces exclus

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
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-01 A1 ; UC-10 §Scénario nominal (inscription depuis le mode local, étapes 5–6) ;
          UC-10 E5 (échec ou interruption de migration) ;
          UC-10 §Règles métier (migration des données locales — espace par espace,
          tout-ou-rien par espace, session LIVE à clôturer, config vue session recréée) ;
          S4 §Gate de migration local→cloud ;
          S3 (nœud « Gate de migration local→cloud » du graphe de navigation) ;
          NFR-ACC-02 ;
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
  — (aucun élément sous-spécifié restant dans le périmètre de cet écran)
```

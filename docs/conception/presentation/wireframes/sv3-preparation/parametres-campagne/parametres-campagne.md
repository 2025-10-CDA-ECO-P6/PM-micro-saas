# Paramètres de campagne

> Fiche de description d'écran basse-fidélité — Vague 3, cluster B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.
> Cet écran agrège des fonctions de plusieurs UC sans UC propre (S5 §Zones sans UC propre).

---

## En-tête de fiche

```
Nom          : Paramètres de campagne
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT (PERSONAL : absent par nature — pas de vue session,
               pas de membres, AR-01 ; AR-14)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-06 Phase 2 (config vue session — accès depuis paramètres) ; UC-02 (archivage) ;
               UC-01 A4a (export — MVP version minimale) ; UC-11 A4 (lien d'invitation de campagne)
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ accède à la configuration de la vue session (depuis un renvoi vers le mode
            configuration de la surface session), gère le cycle de vie de l'espace (archivage),
            génère le lien d'invitation permanent de campagne pour ses joueurs et déclenche
            l'export de l'espace — le tout depuis un point d'accès unique réservé à la préparation.
```

---

### Zones et hiérarchie

```
[ RENVOI VERS LA CONFIGURATION DE LA VUE SESSION ]
  rôle     : point d'accès vers le mode configuration de la surface session (AR-18) ;
             cet écran ne contient pas de panneau de configuration propre — la configuration
             elle-même (sélection et ordre des dossiers mis en avant) s'effectue dans la
             surface session en mode configuration ; le renvoi permet au MJ d'y accéder
             depuis les paramètres de campagne sans quitter le contexte de préparation
  ancrage  : AR-18 (configuration de la vue session logée dans la surface session, mode
             configuration — paramètres-campagne pointe, ne configure pas en propre) ;
             UC-06 §Phase 2 — Vue session MJ (interface) §Panneaux de dossiers configurés ;
             UC-06 §Données manipulées §configuration de la vue session ;
             AR-09 (disposition persistée entre modes)
  type     : lien / affordance de navigation
  priorité : principal (premier plan — AR-19)
  visibilité : MJ seul

[ LIEN D'INVITATION DE CAMPAGNE ]
  rôle     : permet au MJ de générer ou régénérer le lien d'invitation PERMANENT de campagne
             (SpaceMembership) — acte de préparation ; le lien donne accès à la campagne
             de façon durable (pas le lien de session ponctuel, logé dans la vue session) ;
             le MJ peut configurer le périmètre, la date d'expiration et le nombre
             d'utilisations maximum ; le lien est copié pour être partagé (Discord, WhatsApp,
             canal du MJ)
  ancrage  : AR-10 révisé (lien de campagne permanent = SpaceMembership, logé ici ;
             lien de session ponctuel logé dans la vue session — distinction explicite) ;
             UC-11 §Scénario nominal (lien partageable — MVP) ; UC-11 A4 (accès invité sans compte)
  type     : formulaire + affordance de copie
  priorité : principal (premier plan — AR-19)
  visibilité : MJ seul
  note     : le lien de campagne est un acte de préparation (SpaceMembership) ; il n'est
             pas à confondre avec le lien de session ponctuel (accès temporaire) qui relève
             de la surface session

[ ZONE D'ARCHIVAGE DE L'ESPACE ]
  rôle     : permet au MJ d'archiver l'espace (campagne ou one-shot) via un flux à deux temps :
             1. affordance « Archiver l'espace » ;
             2. panneau des conséquences (l'espace passe en lecture seule, plus modifiable,
                réversible côté produit) ;
             3. confirmation explicite du MJ.
             Si une session est en cours (statut en cours) : l'affordance est BLOQUÉE avec
             le message « Terminez la session en cours avant d'archiver » — pas d'archivage
             silencieux (UC-01 A1 §Règles métier).
             Archivage disponible aussi en mode local (ne dépend pas du cloud) — même flux,
             sans invite compte.
  ancrage  : UC-02 §Règles métier (« Un espace peut être archivé sans être supprimé
             définitivement ») ; UC-01 A1 §Règles métier (session en cours → blocage) ;
             AR-13 (mode local) ; S4 §Paramètres de campagne
  type     : formulaire (flux à deux temps : conséquences → confirmation)
  priorité : secondaire-configurable (replié — divulgation progressive, action rare — AR-19)
  visibilité : MJ seul
  note sémantique : la sémantique ARCHIVED d'un espace PERSONAL reste un point d'interview
             non tranché (S9) — cet écran est réservé aux espaces partagés, ce point ne
             le concerne pas directement

[ ZONE D'EXPORT DE L'ESPACE ]
  rôle     : permet au MJ d'exporter l'espace dans un format ouvert téléchargeable
             sur son poste ; disponible en mode local comme avec un compte ;
             présent au MVP en version minimale
  ancrage  : UC-01 A4a (export d'espace — MVP version minimale) ; S8 §Exclusions nommées
             (export réimport)
  type     : formulaire
  priorité : secondaire-configurable (registre secondaire — AR-19 ; non au premier plan)
  visibilité : MJ seul
```

---

### Ce que l'utilisateur peut faire

```
- [UC-06 §Phase 2 ; AR-18] accéder à la configuration de la vue session →
  le renvoi ouvre la surface session en mode configuration ; la sélection et l'ordonnancement
  des dossiers mis en avant s'effectuent dans ce mode (AR-18 — paramètres-campagne ne
  configure pas en propre) ; la configuration est persistée entre les modes de la vue
  session (AR-09)

- [UC-11 §Scénario nominal ; AR-10 révisé] générer le lien d'invitation de campagne →
  le lien de CAMPAGNE PERMANENT (SpaceMembership) est copié pour être partagé
  (Discord, WhatsApp, canal du MJ) ; le MJ peut configurer le périmètre, la date
  d'expiration et le nombre d'utilisations maximum ; ce lien est distinct du lien de
  session ponctuel (logé dans la vue session, pas ici)

- [UC-02 §Règles métier] archiver l'espace →
  flux à deux temps : affordance « Archiver l'espace » → panneau des conséquences
  (lecture seule, plus modifiable, réversible côté produit) → confirmation explicite ;
  si une session est en cours : affordance bloquée, message « Terminez la session en
  cours avant d'archiver » (UC-01 A1) ; l'espace passe en lecture seule après archivage ;
  disponible aussi en mode local (même flux, sans invite compte)

- [UC-01 A4a — MVP version minimale] exporter l'espace →
  export dans un format ouvert téléchargeable sur le poste ; disponible en mode local
  comme avec un compte ; présent au MVP en version minimale (registre secondaire)
```

---

### États

```
état vide (espace vient d'être créé, aucune configuration enregistrée) :
  le renvoi vers la configuration de la vue session est accessible ;
  le MJ est invité à configurer les dossiers à mettre en avant avant la première session
  (via la surface session en mode configuration — AR-18)

état chargé (configuration enregistrée) :
  le renvoi vers la configuration de la vue session est accessible ;
  les autres zones (lien d'invitation de campagne, archivage) sont accessibles ;
  l'état courant de la configuration (dossiers sélectionnés, ordre) est visible
  dans la surface session en mode configuration, pas dans cet écran

état erreur :
  erreur de sauvegarde de configuration (si applicable lors d'un accès lié à la session) :
  notification non bloquante conforme au châssis S7 §Notification d'état de synchronisation
  — le brouillon local est conservé ; le MJ peut reprendre sans perte
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique aux paramètres de campagne.

```
annonce sans action :
  - génération du lien d'invitation de campagne (lien disponible et copiable) :
    annoncé assistivement sans que le MJ déplace son focus — NFR-ACC-02
  - confirmation d'archivage : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance (AR-19 — divulgation progressive) :
  priorité 1 — renvoi vers la configuration de la vue session (accès à la
               préparation de la session — premier plan)
  priorité 2 — lien d'invitation de campagne (acte de préparation — premier plan)
  priorité 3 — zone d'archivage de l'espace (replié — action rare)
  priorité 4 — zone d'export (registre secondaire)
  source : NFR-ACC-04 ; AR-19
```

---

### Sources

```
Sources : UC-06 Phase 2 (accès à la configuration de la vue session via renvoi — AR-18) ;
          UC-02 §Règles métier (archivage) ;
          UC-01 A1 §Règles métier (blocage archivage si session en cours) ;
          UC-01 A4a (export — MVP version minimale) ;
          UC-11 §Scénario nominal (lien d'invitation de campagne — MVP) ; UC-11 A4 ;
          AR-09 (persistance de la configuration entre modes) ;
          AR-10 révisé (lien de campagne permanent = SpaceMembership ; lien de session ponctuel
                        dans la vue session) ;
          AR-13 (mode local) ;
          AR-18 (configuration de la vue session logée dans la surface session, mode
                 configuration — paramètres-campagne pointe, ne configure pas en propre) ;
          AR-19 (densité : premier plan / replié / registre secondaire) ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 §Notification d'état de synchronisation (zoning.md §S7)
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

Fonctions cloud désactivées sur les paramètres de campagne en mode local :
  - [AR-13 ; UC-01 A1] génération du lien d'invitation de campagne → invite contextuelle :
    « Inviter des joueurs nécessite un compte — créer un compte en un geste »

Fonctions disponibles localement (non désactivées) :
  La configuration de la vue session (via renvoi vers le mode configuration — AR-18)
  reste disponible sans compte — elle ne nécessite pas de connexion au service cloud.
  Source : AR-13 ; UC-01 §Règles métier.

  L'archivage de l'espace est disponible en mode local — même flux à deux temps
  (conséquences → confirmation), sans invite compte.
  Source : AR-13 ; résolution archivage.

  L'export de l'espace est disponible en mode local (version minimale MVP).
  Source : AR-13 ; UC-01 A4a.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — S8 §Exclusions nommées] réimport d'un fichier de sauvegarde —
    post-MVP
  [HORS-MVP — UC-11 scénario nominal] invitation par email — hors MVP ;
    seul le lien partageable est disponible au MVP

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Sémantique ARCHIVED/FROZEN d'un espace PERSONAL non tranchée]
    la sémantique de l'archivage pour un espace PERSONAL (non applicable ici,
    cet écran est réservé aux espaces partagés) reste non tranchée — AR-14 ;
    zoning.md §S9
```

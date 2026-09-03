# Paramètres de campagne

> Fiche de description d'écran basse-fidélité — préparation espace partagé.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.
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
Traçabilité  : UC-06 Phase 2 (config vue session — accès depuis paramètres) ;
               UC-02 (archivage, désarchivage — RB-02-21..23) ;
               UC-01 A4a (export — MVP version minimale) ; UC-11 A4 (lien d'invitation de campagne) ;
               AR-22 (désarchivage)
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
             2. panneau des conséquences (l'espace passe en lecture seule, plus modifiable ;
                archivage réversible — le MJ pourra désarchiver l'espace, voir ci-dessous) ;
             3. confirmation explicite du MJ.
             [DÉSACTIVÉ] affordance « Archiver l'espace » — condition de réactivation : la
             session en cours portée par l'espace doit d'abord être clôturée (RB-02-21). Le
             blocage est visible et expliqué au MJ : l'affordance reste présente mais non
             actionnable, accompagnée d'une explication indiquant la condition de déblocage —
             une affordance bloquée sans explication serait un défaut d'interface. Raisonnement
             du blocage : le domaine route l'événement d'archivage vers le contexte de conduite
             de session, mais celui-ci ne définit aucune réaction — sa machine d'états est
             unidirectionnelle et pilotée par ses propres commandes ; ne pas bloquer laisserait
             une session en cours dans un espace passé en lecture seule, dont les notes de
             séance — documents de cet espace — ne pourraient plus être écrites.
             L'archivage est réversible : le MJ peut désarchiver l'espace pour le remettre en
             état actif, avec le même contenu qu'au moment de l'archivage (UC-02 §Règles
             métier RB-02-22 ; domaine, méthode `Unarchive()`). Cette fiche annonce donc au MJ,
             dans le panneau des conséquences, que l'archivage n'est pas un geste définitif.
             L'affordance de désarchivage — symétrique de celle-ci — vit sur cette même fiche,
             dans le mode « espace archivé » (voir §Modes ci-dessous — AR-22). Le point
             d'entrée qui conduit le MJ vers un espace archivé (liste des espaces archivés)
             vit au tableau de bord (AR-22 ; `tableau-de-bord.md` §Zones et hiérarchie §Accès
             aux espaces archivés) — décision de zoning désormais figée ; cette fiche ne porte
             que le geste de désarchivage lui-même, pas le point d'entrée.
             Archivage disponible aussi en mode local (ne dépend pas du cloud) — même flux,
             sans invite compte, même condition de blocage.
  ancrage  : UC-02 §Règles métier RB-02-21 (blocage de l'archivage tant qu'une session est en
             cours) ; UC-02 §Règles métier RB-02-22 (réversibilité de l'archivage) ; domaine
             `space-management.md` invariant 16 (garde d'agrégat sur `Archive()` si l'espace
             porte une session en cours) ; domaine `space-management.md` §Méthodes (`Archive()`
             réversible, `Unarchive()`) ; AR-13 (mode local) ; S4 §Paramètres de campagne
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

- [UC-02 §Règles métier RB-02-21, RB-02-22] archiver l'espace →
  flux à deux temps : affordance « Archiver l'espace » → panneau des conséquences
  (lecture seule, plus modifiable ; archivage réversible — le MJ pourra désarchiver
  l'espace, voir §Zones et hiérarchie §ZONE D'ARCHIVAGE DE L'ESPACE) → confirmation
  explicite ;
  l'affordance est bloquée tant que l'espace porte une session en cours — le MJ doit
  d'abord clôturer cette session (RB-02-21 ; domaine `space-management.md` invariant 16,
  voir §Zones et hiérarchie §ZONE D'ARCHIVAGE DE L'ESPACE) ;
  l'espace passe en lecture seule après archivage, jusqu'à un désarchivage ultérieur —
  le geste de désarchivage vit sur cette même fiche, en mode « espace archivé »
  (voir §Modes ci-dessous) ; le point d'entrée qui y conduit vit au tableau de bord
  (AR-22) ;
  disponible aussi en mode local (même flux, sans invite compte, même condition de blocage)

- [AR-22 ; UC-02 §Règles métier RB-02-22, RB-02-23] désarchiver l'espace (mode espace
  archivé uniquement) →
  l'espace repasse ACTIVE, avec le même contenu qu'au moment de l'archivage ; refusé
  si le MJ est au palier gratuit et que le désarchivage porterait ses espaces actifs
  de type campagne ou one-shot au-delà de la limite de son palier — voir §Modes
  §Mode : espace archivé pour le traitement complet du geste et du refus

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
  - passage en mode « espace archivé » / retour en mode « espace actif » (désarchivage) :
    annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance (AR-19 — divulgation progressive) :
  priorité 1 — renvoi vers la configuration de la vue session (accès à la
               préparation de la session — premier plan)
  priorité 2 — lien d'invitation de campagne (acte de préparation — premier plan)
  priorité 3 — zone d'archivage de l'espace (replié — action rare)
  priorité 4 — zone d'export (registre secondaire)
  source : l'ordre de priorité ci-dessus est posé par AR-19 (divulgation progressive).
           [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-06 Phase 2 (accès à la configuration de la vue session via renvoi — AR-18) ;
          UC-02 §Règles métier (archivage, désarchivage — RB-02-21, RB-02-22, RB-02-23) ;
          domaine `space-management.md` (`Archive()`, `Unarchive()`, règle métier 7,
                   invariants 15 et 16, règle métier 12) ;
          UC-01 A4a (export — MVP version minimale) ;
          UC-11 §Scénario nominal (lien d'invitation de campagne — MVP) ; UC-11 A4 ;
          AR-09 (persistance de la configuration entre modes) ;
          AR-10 révisé (lien de campagne permanent = SpaceMembership ; lien de session ponctuel
                        dans la vue session) ;
          AR-13 (mode local) ;
          AR-18 (configuration de la vue session logée dans la surface session, mode
                 configuration — paramètres-campagne pointe, ne configure pas en propre) ;
          AR-19 (densité : premier plan / replié / registre secondaire) ;
          AR-22 (accès aux espaces archivés au tableau de bord, désarchivage ici) ;
          NFR-ACC-02 ;
          châssis S7 §Notification d'état de synchronisation (zoning.md §S7)
```

---

## Sections conditionnelles

### Modes (déclenché — AR-22 : l'écran a désormais deux modes selon le statut de l'espace)

---

#### Mode : espace actif

```
Déclencheur : l'espace est au statut ACTIVE (`space-management.md` — SpaceStatus) — c'est
              l'état par défaut de cette fiche, entièrement décrit ci-dessus dans le
              Noyau obligatoire.

Zones actives : toutes les zones décrites en « Zones et hiérarchie » — renvoi vers la
                configuration de la vue session, lien d'invitation de campagne, zone
                d'archivage de l'espace (affordance « Archiver l'espace »), zone d'export

Affordances spécifiques : voir §Ce que l'utilisateur peut faire (Noyau obligatoire) —
                aucune affordance propre à ce mode au-delà de celles déjà décrites
```

---

#### Mode : espace archivé

```
Déclencheur : l'espace est au statut ARCHIVED (`space-management.md` — SpaceStatus ;
              UC-02 §Règles métier RB-02-22)

Zones actives :
  - zone d'archivage de l'espace — remplacée par l'affordance symétrique de
    désarchivage (voir Affordances spécifiques ci-dessous)
  - renvoi vers la configuration de la vue session — reste visible, en lecture seule :
    l'espace archivé est en lecture seule (`space-management.md` règle métier 7 ;
    `content-library.md` — `SpaceArchived` passe tous les documents en lecture seule,
    soft-lock) ; le MJ consulte la disposition enregistrée, il ne la modifie pas
  - zone d'export de l'espace — reste disponible ; exporter un espace archivé n'écrit
    rien sur l'espace (UC-01 A4a)
  - bandeau d'état d'archivage — signale que l'espace est archivé, en lecture seule ;
    non bloquant, cohérent avec le registre des bandeaux non bloquants du châssis
    (zoning.md §S7) ; porte l'affordance de désarchivage

Zones désactivées :
  [DÉSACTIVÉ] lien d'invitation de campagne — condition de réactivation : désarchivage
    de l'espace ; un espace en lecture seule ne peut pas admettre de nouveaux membres
    tant qu'il reste ARCHIVED (`space-management.md` règle métier 7)

Affordances spécifiques :
  - [AR-22 ; UC-02 §Règles métier RB-02-22 ; domaine `Unarchive()`] désarchiver
    l'espace → le MJ retrouve son espace dans l'état où il était au moment de
    l'archivage (même contenu, mêmes dossiers) ; l'espace repasse ACTIVE ; le mode
    « espace archivé » cède la place au mode « espace actif »
  - [AR-22 ; UC-02 §Règles métier RB-02-23 ; domaine `space-management.md`
    règle métier 12] refus du désarchivage au quota → si le MJ est au palier gratuit
    et que le désarchivage porterait son nombre d'espaces actifs de type campagne ou
    one-shot au-delà de la limite de son palier, le geste est bloqué ; l'invite
    contextuelle reprend le même registre que celui déjà en place sur la création
    d'un espace bloquée au quota (`tableau-de-bord.md` §Ce que l'utilisateur peut
    faire — « tenter de créer un espace alors que le quota est atteint ») : message
    non bloquant expliquant la limite atteinte, proposant le passage à l'offre
    supérieure ; en mode local, le même registre propose la création d'un compte,
    par symétrie avec le précédent de blocage à la création ; l'espace reste
    ARCHIVED, aucune donnée n'est perdue ni modifiée
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

Fonctions cloud désactivées sur les paramètres de campagne en mode local :
  - [AR-13 ; UC-01 A1] génération du lien d'invitation de campagne → invite contextuelle :
    « Inviter des joueurs nécessite un compte — créer un compte en un geste »

Fonctions disponibles localement (non désactivées) :
  La configuration de la vue session (via renvoi vers le mode configuration — AR-18)
  reste disponible sans compte — elle ne nécessite pas de connexion au service cloud.
  Source : AR-13 ; UC-01 §Règles métier.

  L'archivage de l'espace est disponible en mode local — même flux à deux temps
  (conséquences → confirmation), sans invite compte ; même condition de blocage si
  l'espace porte une session en cours (RB-02-21).
  Source : AR-13 ; UC-02 §Règles métier RB-02-21.

  Le désarchivage (mode « espace archivé ») est disponible en mode local — même
  geste, sans invite compte ; par symétrie avec le précédent de blocage à la
  création (§Mode : espace archivé ci-dessus), le refus au quota y propose la
  création d'un compte plutôt que le passage à l'offre supérieure.
  Source : AR-13 ; AR-22 ; UC-02 §Règles métier RB-02-22.

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
  aucun — le point précédemment sous-spécifié est résolu : l'affordance de
  désarchivage vit désormais sur cette fiche (mode « espace archivé », §Modes) ;
  le point d'entrée vers la liste des espaces archivés vit au tableau de bord
  (AR-22 ; `tableau-de-bord.md` §Zones et hiérarchie §Accès aux espaces archivés)
```

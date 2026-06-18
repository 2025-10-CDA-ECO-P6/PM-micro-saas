# Paramètres de campagne

> Fiche de description d'écran basse-fidélité — Vague 3, cluster B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.
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
Traçabilité  : UC-06 Phase 2 (config vue session) ; UC-02 (archivage) ;
               UC-01 A4a (export — Should Have) ; UC-11 A4 (lien d'invitation)
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ configure la vue session de son espace partagé (dossiers mis en avant,
            ordre), gère le cycle de vie de l'espace (archivage), génère un lien d'invitation
            pour ses joueurs et déclenche l'export de l'espace — le tout depuis un point
            d'accès unique réservé à la préparation.
```

---

### Zones et hiérarchie

```
[ ZONE DE CONFIGURATION DE LA VUE SESSION ]
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne]
    la disposition et les affordances précises de cette zone ne sont pas décrites
    dans le corpus — UC-06 §Données manipulées décrit la configuration (liste ordonnée
    des dossiers mis en avant) sans décrire l'interface de configuration.
  type     : formulaire
  rôle     : permet au MJ de sélectionner les dossiers mis en avant dans la vue session
             et de définir leur ordre d'affichage ; la configuration est persistée
             entre les modes de la vue session (AR-09)
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-06 §Phase 2 — Vue session MJ (interface) §Panneaux de dossiers configurés ;
             UC-06 §Données manipulées §configuration de la vue session ;
             AR-09 (disposition persistée entre modes)

[ AFFORDANCE DE GÉNÉRATION DE LIEN D'INVITATION ]
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement non figé :
    l'affordance de génération du lien d'invitation est logée dans la vue session
    OU dans la vue campagne — non tranché (S4 ; AR-10). Elle figure ici à titre de
    traçabilité ; son appartenance à cet écran n'est pas une décision figée.
  type     : formulaire
  rôle     : permet au MJ de générer le lien de session ponctuel à partager aux joueurs ;
             le lien donne accès à la session en cours (accès temporaire) ou à la campagne
             de façon durable selon le périmètre choisi
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-11 §Scénario nominal (lien partageable — MVP) ; UC-11 A4 (accès invité sans compte) ;
             AR-10 (fraction Must wireframée)

[ ZONE D'ARCHIVAGE DE L'ESPACE ]
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne]
    l'interface d'archivage de l'espace (confirmation, présentation des conséquences,
    état après archivage) n'est pas décrite en détail dans le corpus.
  type     : formulaire
  rôle     : permet au MJ d'archiver l'espace (campagne ou one-shot) — l'espace passe
             en lecture seule et n'est plus modifiable ; l'archivage est une opération
             réversible dans la cible produit (UC-02 §Règles métier) mais la procédure
             de réactivation n'est pas décrite au MVP
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-02 §Règles métier (« Un espace peut être archivé sans être supprimé
             définitivement ») ; S4 §Paramètres de campagne

[ ZONE D'EXPORT DE L'ESPACE ]
  [HORS-MVP / Should — UC-01 A4a §Should Have] cette zone n'est pas présentée
    dans l'interface MVP ; elle est décrite ici à titre de traçabilité de périmètre.
  type     : formulaire
  rôle     : permet au MJ d'exporter l'espace dans un format ouvert téléchargeable
             sur son poste ; disponible en mode local comme avec un compte
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-01 A4a (Should Have — export d'espace) ; S8 §Exclusions nommées
             (export réimport)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-06 §Phase 2] configurer les dossiers mis en avant dans la vue session →
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne] les affordances de sélection et
  d'ordonnancement des dossiers ne sont pas décrites dans le corpus ;
  le résultat observable est que les panneaux de la vue session reflètent
  la configuration enregistrée (AR-09)

- [UC-11 §Scénario nominal] générer un lien d'invitation →
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement dans la vue session
  ou dans la vue campagne non tranché (S4 ; AR-10) ; le lien est copié pour être partagé
  (Discord, WhatsApp, canal du MJ) ; le MJ peut configurer le périmètre
  (accès durable ou accès de session temporaire), la date d'expiration et
  le nombre d'utilisations maximum (UC-11 §Scénario nominal)

- [UC-02 §Règles métier] archiver l'espace →
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne]
  l'interface de confirmation et la présentation des conséquences avant archivage
  ne sont pas décrites dans le corpus ; l'espace passe en lecture seule après
  archivage ; une session en cours (statut en cours) doit être clôturée avant
  archivage (UC-01 A1 §Règles métier)

- [UC-01 A4a — Should Have] exporter l'espace →
  [HORS-MVP / Should] non disponible au MVP ; tracé à titre de périmètre futur
```

---

### États

```
état vide (espace vient d'être créé, aucune configuration enregistrée) :
  la zone de configuration de la vue session s'affiche sans dossiers sélectionnés ;
  le MJ est invité à choisir les dossiers à mettre en avant avant la première session

état chargé (configuration enregistrée) :
  les dossiers sélectionnés et leur ordre d'affichage sont visibles ;
  les autres zones (lien d'invitation, archivage) sont accessibles

état erreur :
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne] le comportement de cet écran
  en cas d'erreur de sauvegarde de la configuration n'est pas décrit dans le corpus
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique aux paramètres de campagne.

```
annonce sans action :
  - génération du lien d'invitation (lien disponible et copiable) :
    annoncé assistivement sans que le MJ déplace son focus — NFR-ACC-02
  - confirmation d'archivage : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de configuration de la vue session (fonction principale
               de préparation à la session)
  priorité 2 — zone de génération du lien d'invitation
  priorité 3 — zone d'archivage de l'espace
  source : NFR-ACC-04
```

---

### Sources

```
Sources : UC-06 Phase 2 (configuration de la vue session) ;
          UC-02 §Règles métier (archivage) ;
          UC-01 A4a (export — Should Have) ;
          UC-11 §Scénario nominal (lien d'invitation — MVP) ; UC-11 A4 ;
          AR-09 (persistance de la configuration entre modes) ;
          AR-10 (fraction Must wireframée — placement lien d'invitation non figé) ;
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

Fonctions cloud désactivées sur les paramètres de campagne en mode local :
  - [AR-13 ; UC-01 A1] génération du lien d'invitation → invite contextuelle :
    « Inviter des joueurs nécessite un compte — créer un compte en un geste »
  - [AR-13] archivage d'un espace partagé (si l'espace est en mode local) →
    [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne] le comportement de l'archivage
    en mode local n'est pas décrit dans le corpus

Fonctions disponibles localement (non désactivées) :
  La configuration de la vue session (sélection et ordre des dossiers mis en avant)
  reste disponible sans compte — elle ne nécessite pas de connexion au service cloud.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP / Should — UC-01 A4a] export de l'espace dans un format ouvert —
    Should Have, non disponible au MVP
  [HORS-MVP — S8 §Exclusions nommées] réimport d'un fichier de sauvegarde —
    post-MVP
  [HORS-MVP — UC-11 scénario nominal] invitation par email — hors MVP ;
    seul le lien partageable est disponible au MVP

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne]
    cet écran est fortement sous-spécifié dans le corpus — aucun UC n'est
    dédié aux paramètres de campagne (S5 §Zones sans UC propre) ; les fonctions
    qu'il agrège sont décrites dans leurs UC respectifs sans que l'interface
    de la page paramètres soit définie
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement de l'affordance de
    génération du lien d'invitation : vue session OU vue campagne —
    non tranché (S4 ; AR-10) ; le lien figure dans les deux fiches à titre de traçabilité
  [SOUS-SPÉCIFIÉ — S9 §Sémantique ARCHIVED/FROZEN d'un espace PERSONAL non tranchée]
    la sémantique de l'archivage pour un espace PERSONAL (non applicable ici,
    cet écran est réservé aux espaces partagés) reste non tranchée — AR-14 ;
    zoning.md §S9
  [SOUS-SPÉCIFIÉ — S5 §Zones sans UC propre ; zoning.md §S4 §Paramètres de campagne] interface de confirmation avant
    archivage (présentation des conséquences, blocage si session en cours) —
    non décrite dans le corpus
```

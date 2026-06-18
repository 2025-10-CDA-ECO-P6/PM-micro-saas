# Vue joueur (post-accès, base)

> Fiche de description d'écran basse-fidélité — Vague 3, surface joueur.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md` sur la vue joueur de base.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-03, AR-06, AR-10, AR-14`.

---

## En-tête de fiche

```
Nom          : Vue joueur post-accès
Surface      : joueur
Contexte     : session
Type d'espace: n.a. (l'écran est indépendant du type d'espace — le joueur n'a pas accès
               à la structure de l'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté
               au MVP — AR-07)
Traçabilité  : UC-12 ; UC-09 scénario A ; UC-06 §vue joueur ; US-06-07 ; US-06-08 ; RB-12-09
```

---

## Noyau obligatoire

### Intention

```
Intention : le joueur, une fois son accès actif, consulte les documents PUBLIC partagés
            par le MJ, prend des notes personnelles pendant la session LIVE,
            et accède à la fiche du personnage qui lui a été associé si disponible.
```

---

### Zones et hiérarchie

```
[ ZONE DE DOCUMENTS PARTAGÉS ]
  type     : principal
  rôle     : présente la liste des documents PUBLIC visibles par le joueur selon
             son périmètre d'accès (SESSION ou CAMPAIGN) ; le contenu se met à jour
             en temps réel pendant une session LIVE lorsque le MJ partage ou retire
             le partage d'un document (AR-06 — apparition ambiante, disparition
             symétrique) ; le joueur ne voit jamais les documents GM_ONLY ni
             les notes PLAYER_PRIVATE d'autres joueurs (RB-12-01 ; NFR-CONF-01 ; AR-03)
  priorité : principal
  visibilité : joueur seul (PUBLIC visible par les joueurs — RB-12-01)
  ancrage  : UC-12 scénario nominal §4 ; UC-12 A2 ; RB-12-01 ; RB-12-04 ; AR-06

[ ZONE DE NOTES PERSONNELLES ]
  type     : principal
  rôle     : permet au joueur de créer et consulter ses notes personnelles
             (PLAYER_PRIVATE) pendant la session LIVE ; les notes sont liées à
             l'auteur — elles ne sont accessibles à aucun autre participant,
             y compris le MJ ; disponible uniquement si un personnage est associé
             au joueur (RB-12-03)
  priorité : principal
  visibilité : joueur seul (PLAYER_PRIVATE)
  ancrage  : UC-12 scénario nominal §5 ; US-06-07 ; US-06-08 ; RB-12-02 ; RB-12-03

[ ZONE DE FICHE DE PERSONNAGE ]
  type     : latéral
  rôle     : affiche la fiche du personnage associé au joueur par le MJ, s'il en
             a un ; la fiche correspond au personnage actif sélectionné (RB-12-10) ;
             la fiche appartient à la campagne et n'est pas modifiable par le joueur
             depuis cet écran
  priorité : secondaire-configurable
  visibilité : joueur seul
  ancrage  : UC-12 scénario nominal §3 ; UC-12 A3 (absent si aucun personnage associé)
              ; RB-12-02 ; RB-12-06 ; RB-12-08
```

---

### Ce que l'utilisateur peut faire

```
- [UC-12 scénario nominal §4 ; RB-12-01] consulter les documents PUBLIC de la session
  → le joueur lit les documents que le MJ a partagés selon son périmètre d'accès
  (SESSION ou CAMPAIGN) ; il ne peut pas modifier ces documents

- [UC-12 scénario nominal §5 ; RB-12-02 ; RB-12-03] créer une note personnelle →
  la note est liée à l'auteur et au personnage actif ; elle est PLAYER_PRIVATE
  et invisible pour les autres participants ; nécessite un personnage associé

- [UC-12 scénario nominal §5 ; RB-12-10] consulter ses notes personnelles existantes
  → les notes PLAYER_PRIVATE du joueur, liées au personnage actif, sont accessibles
  et éditables pendant la session LIVE

- [UC-12 scénario nominal §3 ; RB-12-06 ; RB-12-08] consulter la fiche du personnage
  actif → la fiche s'affiche si le MJ a associé un personnage au joueur ;
  le joueur ne peut pas modifier les associations définies par le MJ (RB-12-07)

- [UC-09 A2] créer un compte depuis cette vue → le joueur engage le parcours de
  création de compte (UC-10) pour conserver ses notes entre sessions
```

---

### États

```
état vide (accès actif, aucun document encore partagé, aucun personnage associé) :
  la zone de documents partagés affiche un état vide indiquant qu'aucun document
  n'a encore été partagé pour cette session ; la zone de notes personnelles est
  absente par nature si aucun personnage n'est associé (RB-12-03)
  [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur]

état chargé (accès actif, documents partagés présents, personnage associé) :
  les documents PUBLIC s'affichent ; la fiche du personnage actif est visible ;
  les notes PLAYER_PRIVATE du joueur sur ce personnage sont accessibles

état erreur (accès expiré ou révoqué pendant la session) :
  l'accès du joueur devient inactif (UC-12 E1) ; le joueur est renvoyé vers
  le flux d'accès UC-09 — voir fiche `docs/presentation/wireframes/erreur-acces.md`
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la vue joueur post-accès.

```
annonce sans action :
  - apparition d'un nouveau document partagé par le MJ en temps réel : annoncé
    assistivement sans que le joueur déplace son focus — NFR-ACC-02 ; AR-06
    (le changement de visibilité côté joueur est annoncé aux outils de lecture
    d'écran indépendamment de l'absence de notification UX visuelle)
  - disparition d'un document dont le partage a été retiré par le MJ : annoncé
    assistivement — NFR-ACC-02 ; AR-06 (disparition symétrique)
  - création d'une note personnelle confirmée : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de documents partagés (contenu de la session, valeur principale)
  priorité 2 — zone de notes personnelles (prise de notes active en LIVE)
  priorité 3 — zone de fiche de personnage (référence de la session)
  source : NFR-ACC-04 (usage à distance normale de l'écran, contexte de session à table)
```

---

### Sources

```
Sources : UC-12 scénario nominal ; UC-12 A1 ; UC-12 A2 ; UC-12 A3 ; UC-12 E1 ;
          UC-09 scénario A ; UC-09 A2 ;
          UC-06 §vue joueur ;
          US-06-07 ; US-06-08 ;
          RB-12-01 ; RB-12-02 ; RB-12-03 ; RB-12-04 ; RB-12-05 ;
          RB-12-06 ; RB-12-07 ; RB-12-08 ; RB-12-09 ; RB-12-10 ;
          AR-03 ; AR-06 ; AR-07 ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          NFR-CONF-01 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Partage / visibilité (déclenché — AR-03 et NFR-CONF-01 : cet écran est la surface joueur séparée)

```
Règle de séparation : le joueur ne voit que les documents PUBLIC selon son périmètre
  d'accès ; aucun document GM_ONLY, aucune note PLAYER_PRIVATE d'un autre joueur,
  aucune structure de l'espace n'est visible ni révélée — NFR-CONF-01 ; AR-03 ;
  RB-12-01. La séparation est une garantie de conception, pas un filtre.

Conditionnelle Partage/visibilité côté joueur :
  - Le joueur voit uniquement PUBLIC + ses propres PLAYER_PRIVATE.
  - L'apparition d'un document partagé est ambiante (repère de nouveauté discret)
    et immédiate — AR-06.
  - Le retrait d'un partage fait disparaître le document de la zone de documents
    partagés en temps réel — AR-06 (disparition symétrique).
  - Aucune notification active (toast, badge) au MVP — AR-06 ; US-06 §Questions
    ouvertes (point d'interview non tranché).

Affordances de partage et d'épinglage :
  [ABSENT PAR NATURE] — le joueur ne dispose d'aucune affordance de partage ni
  d'épinglage sur sa surface ; il consulte et note, il ne pilote pas la visibilité.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — AR-10 ; S8] enrichissement de la vue joueur au niveau campagne
    (UC-12 complet — historique de sessions, sélection multi-personnages intégrée,
    accès au lore complet CAMPAIGN) : non wireframé au MVP ; seule la fraction de
    base (session courante, périmètre SESSION ou CAMPAIGN simplifié) est couverte
  [HORS-MVP — UC-12 A1 ; AR-10] interface de sélection du personnage actif pour
    un joueur associé à plusieurs personnages dans la même campagne — différé avec
    l'enrichissement UC-12 complet
  [HORS-MVP — moscow.md §Could Have] notes personnelles joueur persistantes
    inter-sessions sans compte (RB-09-19 : les notes PLAYER_PRIVATE d'un invité
    non converti sont supprimées à la fin de l'accès)

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur] la notification active
    (signalement sonore ou visuel de l'apparition d'un nouveau document partagé)
    n'est pas décidée pour le MVP — angle d'interview (US-06 §Questions ouvertes)
  [SOUS-SPÉCIFIÉ — S9 §Persistance des notes invité inter-sessions sans compte]
    la mécanique de récupération des notes PLAYER_PRIVATE d'un invité via un nouveau
    lien vers le même personnage n'est pas entièrement spécifiée — relève de
    remédiation corpus
  [SOUS-SPÉCIFIÉ — UC-12 §Questions à valider en interview] le personnage actif
    doit-il être mémorisé entre sessions ou remis à zéro à chaque connexion —
    point d'interview
  [SOUS-SPÉCIFIÉ — UC-12 §Questions à valider en interview] un joueur sans personnage
    associé peut-il prendre des notes libres non liées à un personnage — point
    d'interview ; RB-12-03 interdit actuellement les notes PLAYER_PRIVATE sans
    personnage associé
```

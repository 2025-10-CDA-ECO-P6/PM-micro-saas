# Accès par lien + saisie du nom d'affichage

> Fiche de description d'écran basse-fidélité — surface joueur.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md` sur le point d'entrée joueur.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01, AR-03, AR-06`.

---

## En-tête de fiche

```
Nom          : Accès par lien + saisie du nom d'affichage
Surface      : joueur
Contexte     : session
Type d'espace: n.a. (l'écran est indépendant du type d'espace — le joueur n'a pas accès
               à la structure de l'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté
               au MVP — AR-07)
Traçabilité  : UC-09 scénario A ; RB-09-20 ; RB-09-21
```

---

## Noyau obligatoire

### Intention

```
Intention : le joueur, à l'ouverture du lien reçu du MJ, accède immédiatement aux
            documents partagés pour la session et saisit uniquement un nom d'affichage
            pour participer — sans créer de compte, sans mot de passe.
```

---

### Zones et hiérarchie

```
[ ZONE DE CONTENU PARTAGÉ ]
  type     : principal
  rôle     : présente les documents que le MJ a partagés pour cette session ;
             le contenu est visible dès l'ouverture du lien, avant toute saisie
             du nom d'affichage — le joueur perçoit la valeur avant tout engagement
  priorité : principal
  visibilité : PUBLIC (visible par les joueurs)
  ancrage  : UC-09 scénario A §4 (« L'application affiche immédiatement les informations
             partagées par le MJ pour cette session »)

[ FORMULAIRE DE SAISIE DU NOM D'AFFICHAGE ]
  type     : formulaire
  rôle     : recueille uniquement le nom d'affichage du joueur — seul champ requis
             pour participer à la session ; aucun email, aucun mot de passe demandé
  priorité : principal
  visibilité : joueur seul
  ancrage  : UC-09 scénario A §5 ; UC-09 §Règles métier (friction minimale)

[ ZONE D'INFORMATION RGPD ]
  type     : bandeau
  rôle     : informe le joueur de manière simple de ce qui est conservé (son nom
             d'affichage, ses notes privées éventuelles), pour combien de temps
             (durée de l'accès + grâce), et du sort de ses données à la fin de
             l'accès (notes privées supprimées sans délai si pas de compte créé ;
             nom d'affichage effacé au plus tard 90 jours après la fin d'accès) ;
             présente l'option de créer un compte pour conserver ses données
  priorité : co-présent-jamais-masqué
  visibilité : joueur seul
  ancrage  : RB-09-20 (information RGPD Art. 13 au moment de la saisie du nom)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-09 scénario A §5] saisir un nom d'affichage → le joueur entre dans la session
  avec ce nom visible du MJ ; son accès est actif pour la durée de la session
  (lien ponctuel — UC-09 §Règles métier)

- [UC-09 scénario A §4 ; RB-12-01] consulter les documents partagés avant de saisir
  son nom → le contenu PUBLIC visible pour cette session est accessible dès l'ouverture
  du lien ; la saisie du nom ne conditionne pas la consultation du contenu initial

- [UC-09 A2] créer un compte depuis cet écran → le joueur engage le parcours de
  création de compte (UC-10) pour conserver ses notes entre sessions ;
  l'accès invité en cours migre vers le compte sans perte de données
```

---

### États

```
état vide (lien valide, aucun document encore partagé par le MJ) :
  la zone de contenu partagé s'affiche avec un état vide explicite indiquant qu'aucun
  document n'a encore été partagé pour cette session ; le formulaire de saisie du nom
  reste accessible [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur]

état chargé (lien valide, documents partagés présents) :
  les documents PUBLIC partagés pour la session sont visibles ; le formulaire de saisie
  du nom est accessible ; la zone d'information RGPD est présente

état erreur (lien expiré, révoqué, invalide ou quota atteint) :
  voir fiche dédiée `docs/conception/interface/wireframes/sv5-joueur/erreur-acces/erreur-acces.md` —
  la page d'erreur est un écran distinct ; cet écran n'affiche pas d'état d'erreur
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à cet écran.

```
annonce sans action :
  - apparition d'un nouveau document partagé pendant que le joueur est sur cet écran :
    annoncé assistivement sans que le joueur déplace son focus — NFR-ACC-02 ;
    AR-06 (apparition ambiante ; changement de visibilité annoncé aux outils de lecture
    d'écran indépendamment de l'absence de notification UX visuelle)
  - validation du nom d'affichage et passage à la vue joueur : annoncé assistivement
    — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de contenu partagé (valeur immédiate de la session)
  priorité 2 — formulaire de saisie du nom d'affichage (seul geste requis)
  priorité 3 — zone d'information RGPD (information obligatoire — RB-09-20)
  source : NFR-ACC-04 (usage pendant une session, contexte de lecture rapide)
```

---

### Sources

```
Sources : UC-09 scénario A ; UC-09 A2 ; UC-09 A3 ;
          UC-09 E1 ; UC-09 E2 ;
          RB-09-18 ; RB-09-19 ; RB-09-20 ; RB-09-21 ;
          RB-12-01 ;
          AR-03 ; AR-06 ; AR-07 ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Partage / visibilité (déclenché — AR-03 : cet écran touche la frontière joueur)

```
Règle de séparation : le joueur ne voit que les documents PUBLIC partagés par le MJ
  pour cette session ; aucune structure MJ, aucun document GM_ONLY, aucun document
  PLAYER_PRIVATE d'un autre joueur ne sont visibles ni révélés — NFR-CONF-01 ; AR-03.
  Le joueur n'a pas accès à la connaissance de l'existence d'un contenu non partagé.

Indicateur de partage :
  Absent par nature sur cet écran — l'indicateur de partage est un composant de la
  surface MJ (châssis S7 §Indicateur de partage) ; cet écran est une surface joueur
  distincte qui ne l'expose pas.

Affordances de partage et d'épinglage :
  [ABSENT PAR NATURE] — le joueur ne dispose d'aucune affordance de partage ni
  d'épinglage sur cet écran ; il consulte le contenu PUBLIC, il ne le pilote pas.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — moscow.md §Could Have] notes personnelles joueur persistantes
    inter-sessions sans compte — à ce stade, les notes PLAYER_PRIVATE d'un invité
    non converti sont supprimées à la fin de l'accès (RB-09-19)
  [HORS-MVP — UC-09 A2 ; moscow.md §Could Have] récupération automatique de notes
    invité via un second lien sans création de compte

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur] la notification active
    (signalement sonore ou visuel de l'apparition d'un nouveau document partagé)
    n'est pas décidée pour le MVP — angle d'interview (US-06 §Questions ouvertes)
  [SOUS-SPÉCIFIÉ — S9 §Persistance des notes invité inter-sessions sans compte]
    la mécanique de récupération des notes PLAYER_PRIVATE d'un invité via un nouveau
    lien vers le même personnage est évoquée dans UC-06 §Règles métier mais non
    entièrement spécifiée — trou de corpus
  [SOUS-SPÉCIFIÉ — UC-09 §Questions à valider en interview] le nom d'affichage seul
    est-il suffisant ou faut-il un identifiant léger pour éviter les collisions —
    point d'interview
```

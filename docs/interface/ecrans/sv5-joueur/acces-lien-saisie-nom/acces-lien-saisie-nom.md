# Fiche d'écran — Accès par lien + saisie du nom d'affichage

> **Nature** : Fiche de description d'écran basse-fidélité — surface joueur.
> **Gabarit** : Instancie le gabarit `docs/interface/gabarit-ecran.md` sur le point d'entrée joueur.
> **Notation et nommage** : `docs/interface/conventions.md`.
> **Arbitrages figés** : `docs/interface/zonage.md §S6 AR-01, AR-03, AR-06`.

---

## En-tête de fiche

- **Nom** : Accès par lien + saisie du nom d'affichage
- **Surface** : joueur
- **Contexte** : session
- **Type d'espace** : n.a. (l'écran est indépendant du type d'espace — le joueur n'a pas accès
  à la structure de l'espace)
- **Forme cible** : grand écran + tablette (mobile : pensé dans la structure, non implémenté
  au MVP — AR-07)
- **Traçabilité** : UC-09 scénario A ; RB-09-20 ; RB-09-21

---

## Noyau obligatoire

### Intention

```
Intention : le joueur, à l'ouverture du lien reçu du MJ, voit de quel espace ou de
            quelle session il s'agit et qui est le MJ, puis saisit uniquement un nom
            d'affichage pour participer — sans créer de compte, sans mot de passe ;
            les documents partagés ne lui sont montrés qu'une fois son accès créé
            (vue joueur post-accès).
```

---

### Zones et hiérarchie

```
[ ZONE D'INFORMATION DU LIEN ]
  type     : principal
  rôle     : présente les seules métadonnées du lien — nom de l'espace ou de la session,
             MJ — avant toute saisie du nom d'affichage ; aucun document n'est affiché
             à ce stade : l'accès n'existe pas encore (il est créé à la validation du
             nom d'affichage) ; l'information RGPD vit dans sa zone dédiée ci-dessous
  priorité : principal
  visibilité : joueur seul
  ancrage  : UC-09 scénario A §4 ; domaine `space-management` flows.md Flux 4
             (page de saisie du nom d'abord, `GuestAccess.Create` ensuite) ; AR-03

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

- [UC-09 scénario A §4 ; domaine `space-management` Flux 4] prendre connaissance du lien avant
  de saisir son nom → le joueur lit le nom de l'espace ou de la session, le MJ et
  l'information RGPD ; aucun document n'est visible avant la création de l'accès ;
  les documents partagés s'affichent dans la vue joueur post-accès

- [UC-09 A2] créer un compte depuis cet écran → le joueur engage le parcours de
  création de compte (UC-10) pour conserver ses notes entre sessions ;
  l'accès invité en cours migre vers le compte sans perte de données
```

---

### États

```
état chargé (lien valide) :
  la zone d'information du lien, le formulaire de saisie du nom et la zone d'information
  RGPD sont présents ; aucun document n'est visible, qu'il y en ait ou non de partagés
  par le MJ (l'état vide des documents partagés relève de la vue joueur post-accès)

état erreur (lien expiré, révoqué, invalide ou quota atteint) :
  voir fiche dédiée `docs/interface/ecrans/sv5-joueur/erreur-acces/erreur-acces.md` —
  la page d'erreur est un écran distinct ; cet écran n'affiche pas d'état d'erreur
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à cet écran.

```
annonce sans action :
  - validation du nom d'affichage et passage à la vue joueur : annoncé assistivement
    — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone d'information du lien (de quelle session il s'agit)
  priorité 2 — formulaire de saisie du nom d'affichage (seul geste requis)
  priorité 3 — zone d'information RGPD (information obligatoire — RB-09-20)
  source : NFR-ACC-04 (usage pendant une session, contexte de lecture rapide)
```

---

### Sources

```
Sources : UC-09 scénario A §4 ; UC-09 scénario A §5 ; UC-09 A2 ; UC-09 A3 ;
          domaine `space-management` flows.md Flux 4 ;
          UC-09 E1 ; UC-09 E2 ;
          RB-09-18 ; RB-09-19 ; RB-09-20 ; RB-09-21 ;
          RB-12-01 ;
          AR-03 ; AR-06 ; AR-07 ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Partage et visibilité

Déclenché — AR-03 : cet écran touche la frontière joueur.

```
Règle de séparation : avant la création de son accès, le joueur ne voit aucun document,
  seulement les métadonnées du lien (décision 0045) ; aucune structure MJ, aucun document
  GM_ONLY, aucun document PLAYER_PRIVATE d'un autre joueur ne sont visibles ni révélés —
  NFR-CONF-01 ; AR-03. Le joueur n'a pas accès à la connaissance de l'existence d'un
  contenu non partagé ; les documents PUBLIC lui sont montrés par la vue joueur post-accès.

Indicateur de partage :
  Absent par nature sur cet écran — l'indicateur de partage est un composant de la
  surface MJ (châssis S7 §Indicateur de partage) ; cet écran est une surface joueur
  distincte qui ne l'expose pas.

Affordances de partage et d'épinglage :
  [ABSENT PAR NATURE] — le joueur ne dispose d'aucune affordance de partage ni
  d'épinglage sur cet écran ; il consulte le contenu PUBLIC, il ne le pilote pas.
```

---

### Hors périmètre, différé, sous-spécifié

Déclenché — éléments S8 et S9.

```
Éléments différés (S8) :
  [HORS-MVP — moscow.md §Could Have] notes personnelles joueur persistantes
    inter-sessions sans compte — à ce stade, les notes PLAYER_PRIVATE d'un invité
    non converti sont supprimées à la fin de l'accès (RB-09-19)
  [HORS-MVP — UC-09 A2 ; moscow.md §Could Have] récupération automatique de notes
    invité via un second lien sans création de compte

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Persistance des notes invité inter-sessions sans compte]
    la mécanique de récupération des notes PLAYER_PRIVATE d'un invité via un nouveau
    lien vers le même personnage est évoquée dans UC-06 §Règles métier mais non
    entièrement spécifiée — trou de corpus
  [SOUS-SPÉCIFIÉ — UC-09 §Questions à valider en interview] le nom d'affichage seul
    est-il suffisant ou faut-il un identifiant léger pour éviter les collisions —
    point d'interview
```

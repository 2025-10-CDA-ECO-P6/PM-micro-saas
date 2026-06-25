# Panneau de création rapide à la volée

> Fiche de description d'écran basse-fidélité — Vague 3.
> Sur-couche de la vue session MJ, invoquée sans quitter la surface.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S4 §S6`.

---

## En-tête de fiche

```
Nom          : Panneau de création rapide à la volée
Surface      : MJ
Contexte     : session
Type d'espace: CAMPAIGN | ONE_SHOT (PERSONAL : absent par nature — pas de vue session, AR-01 ; AR-14)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-07 ; S4 §Vue session MJ
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ crée un document ou une note sans quitter la vue session,
            depuis n'importe quel mode actif (LIVE ou consultation CLOSED),
            en saisissant uniquement un titre — le contenu peut être complété ensuite.
```

---

### Zones et hiérarchie

```
[ FORMULAIRE DE CRÉATION RAPIDE ]
  type     : formulaire
  rôle     : surface de saisie minimale permettant de créer un document ou une note ;
             seul le titre est obligatoire — tous les autres champs sont optionnels
             ou complétables après création ; la surface s'affiche par-dessus la vue
             session sans la remplacer
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-07 ; S4 §Panneau de création rapide à la volée
```

---

### Ce que l'utilisateur peut faire

```
- [UC-07 ; S4 §Vue session MJ] créer un document ou une note à la volée →
  le MJ saisit un titre (seul champ obligatoire) et valide ;
  le document est créé dans l'espace courant ;
  résultat : document disponible dans l'espace, accessible ensuite pour complétion

- [UC-07 §Préconditions] fermer le panneau sans créer → retour à la vue session
  dans l'état où elle était avant l'ouverture du panneau ; aucun document créé

Comportement de l'auto-épinglage selon le mode :
  En mode LIVE : le document créé est automatiquement épinglé dans le panneau
  des documents épinglés (UC-08 A3 — l'auto-épinglage est réservé à la session
  active).
  En mode consultation CLOSED : PAS d'auto-épinglage à la création rétroactive.
  Le document est créé et rangé dans l'espace ; il n'est pas épinglé
  automatiquement. Le MJ peut l'épingler manuellement ensuite.
  Cohérent avec « partage rétroactif sans auto-épinglage » (vue-session-mj.md
  §Mode consultation CLOSED).

Invocation depuis la vue session :
  - en mode LIVE : [UC-07 ; zoning.md §S3] le panneau est accessible depuis
    la vue session en mode LIVE (graphe de navigation S3)
  - en mode consultation CLOSED : [UC-07 A5] la création à la volée rétroactive
    est disponible depuis la vue session en mode consultation CLOSED
    (UC-07 préconditions A5 — annotations rétroactives MJ)
  Le panneau n'est pas invoqué depuis le mode configuration (aucune session active —
  la création à la volée est une affordance de session).
```

---

### États

```
état vide (panneau ouvert, aucune saisie) :
  le champ titre est vide ; la validation est indisponible jusqu'à la saisie
  d'au moins un caractère de titre

état en cours de saisie (titre saisi) :
  la validation devient accessible ; le MJ peut déclencher la création

état créé (après validation) :
  le panneau se referme ; la vue session retrouve son état précédent ;
  le document nouvellement créé est disponible dans l'espace ;
  auto-épinglage en mode LIVE uniquement (UC-08 A3) ;
  en mode CLOSED : document rangé sans épinglage automatique
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à ce panneau.

```
annonce sans action :
  - création d'un document ou d'une note confirmée : annoncée assistivement
    sans que le MJ déplace son focus — NFR-ACC-02 ;
    l'annonce inclut le titre du document créé
  - fermeture du panneau sans création : annoncée assistivement (retour au
    contexte de la vue session) — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — champ titre (seule saisie obligatoire)
  priorité 2 — affordance de validation
  priorité 3 — affordance de fermeture sans création
  source : NFR-ACC-04 (lisibilité à distance en session à table)
```

---

### Sources

```
Sources : UC-07 ; S4 §Vue session MJ ; S4 §Panneau de création rapide
          à la volée ; NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Mode local

Mode local hérité de la vue session hôte — voir `docs/presentation/wireframes/vue-session-mj.md`.

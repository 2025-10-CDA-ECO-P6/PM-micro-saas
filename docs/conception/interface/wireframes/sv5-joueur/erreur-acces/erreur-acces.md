# Page d'erreur d'accès

> Fiche de description d'écran basse-fidélité — surface joueur.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md` sur la page d'erreur d'accès.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-03`.

---

## En-tête de fiche

```
Nom          : Page d'erreur d'accès
Surface      : joueur
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace — aucune information
               sur l'espace n'est révélée)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté
               au MVP — AR-07)
Traçabilité  : UC-09 A3 ; UC-09 E1 ; UC-09 E2
```

---

## Noyau obligatoire

### Intention

```
Intention : le joueur dont l'accès ne peut pas être accordé (lien expiré, révoqué,
            invalide ou quota de joueurs atteint) reçoit un message sobre qui ne
            révèle pas l'existence de la campagne ni la raison précise du refus,
            et est invité à contacter le MJ pour obtenir un nouveau lien.
```

---

### Zones et hiérarchie

```
[ ZONE DE MESSAGE D'ERREUR ]
  type     : principal
  rôle     : affiche un message sobre indiquant que le lien n'est plus actif et
             invitant le joueur à contacter le MJ pour un nouveau lien ;
             le message est identique quel que soit le motif précis (lien expiré,
             révoqué, invalide, quota atteint) — même registre pour tous les cas
             afin de ne pas révéler l'existence de la campagne (RB-09-21 ; AR-03)
  priorité : principal
  visibilité : joueur seul
  ancrage  : UC-09 A3 (« Ce lien n'est plus actif — contacter le MJ pour un nouveau
             lien ») ; UC-09 E1 (lien invalide — message sobre sans révéler si la
             campagne existe) ; UC-09 E2 (quota FREE atteint — même registre)
             ; RB-09-21 (le joueur surnuméraire voit un message sobre sans information
             sur la campagne)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-09 A3] prendre note de l'invitation à contacter le MJ → le message oriente le
  joueur vers la seule action possible : solliciter un nouveau lien auprès du MJ ;
  aucune action produit n'est disponible sur cet écran
```

---

### États

```
état chargé (cas nominal de cet écran — un des motifs d'erreur est présent) :
  le message sobre s'affiche ; l'invitation à contacter le MJ est présente ;
  aucun détail sur le motif précis (lien expiré, révoqué, invalide ou quota atteint)
  n'est exposé — même registre pour tous les cas (UC-09 A3 ; UC-09 E1 ; UC-09 E2 ;
  RB-09-21)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à cet écran.

```
annonce sans action :
  - aucun changement d'état dynamique sur cet écran : l'écran est statique une fois
    affiché ; pas d'annonce assistive dynamique requise — NFR-ACC-02 (non applicable)

hiérarchie de lecture à distance :
  priorité 1 — zone de message d'erreur (seul contenu de l'écran)
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-09 A3 ; UC-09 E1 ; UC-09 E2 ;
          RB-09-21 ;
          AR-03 ;
          NFR-ACC-02 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Partage / visibilité (déclenché — AR-03 : cet écran est sur la surface joueur et ne révèle rien de la campagne)

```
Règle de séparation : cet écran ne révèle pas l'existence de la campagne, ni la
  visibilité d'aucun document, ni aucune structure MJ — le message sobre est
  identique quel que soit le motif précis du refus d'accès — NFR-CONF-01 ; AR-03.
  Un lien invalide (E1) et un quota atteint (E2) produisent le même message
  qu'un lien révoqué (A3) : l'uniformité du registre est la garantie
  de non-révélation.

Affordances de partage et d'épinglage :
  [ABSENT PAR NATURE] — aucun contenu n'est visible sur cet écran ;
  aucune affordance de partage ou d'épinglage n'a de sens ici.
```

# Suppression de compte (RGPD)

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 1 (transversaux), lot B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.

---

## En-tête de fiche

```
Nom          : Suppression de compte (RGPD)
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-10 A4 ; NFR-CONF-03
```

---

## Noyau obligatoire

### Intention

```
Intention : l'utilisateur authentifié déclenche la suppression définitive de son compte
            en prenant connaissance des conséquences irréversibles avant toute confirmation ;
            la procédure est bloquée si l'utilisateur est propriétaire de campagnes actives
            avec des membres, et elle expose explicitement les raisons du blocage.
```

---

### Zones et hiérarchie

```
[ ZONE DE PRÉSENTATION DES CONSÉQUENCES ]
  type     : principal
  rôle     : expose de façon exhaustive et lisible les effets irréversibles de la
             suppression — suppression intégrale et inconditionnelle de l'espace
             personnel (documents, dossiers, blocs, sans aucune exception), suppression
             physique des documents privés de joueur créés par l'utilisateur (y compris
             ceux rattachés aux personnages qu'il incarnait dans les campagnes vivantes),
             conservation des campagnes et one-shots partagés sous identité anonymisée
             (campagnes orphelines), et déconnexion du compte ;
             cette zone est visible avant toute action de confirmation
  priorité : co-présent-jamais-masqué
  visibilité : utilisateur authentifié

[ ZONE DE CONFIRMATION DE SUPPRESSION ]
  type     : formulaire
  rôle     : recueille la confirmation explicite de l'utilisateur après qu'il a pris
             connaissance des conséquences ; accessible uniquement en état confirmable
             (adresse de messagerie validée — RB-10-05 — et aucune campagne active
             avec membres) ; la confirmation est une action distincte et délibérée,
             non déclenchable par erreur
  priorité : principal
  visibilité : utilisateur authentifié

[ ZONE DE BLOCAGE ]
  type     : principal
  rôle     : remplace la zone de confirmation quand la suppression est impossible ;
             indique les campagnes actives avec membres qui font obstacle, et guide
             l'utilisateur vers les actions requises avant de pouvoir supprimer son
             compte (exclure les membres au MVP) ; visible uniquement en état bloqué
  priorité : principal
  visibilité : utilisateur authentifié (état bloqué uniquement)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-10 A4] prendre connaissance des conséquences de la suppression → la zone de
  présentation des conséquences est visible dès l'arrivée sur l'écran, avant toute
  action ; l'utilisateur lit les effets irréversibles avant de décider

- [UC-10 A4] confirmer la suppression de son compte → le compte est marqué supprimé ;
  l'espace personnel est supprimé intégralement et inconditionnellement (documents,
  dossiers, blocs, sans exception) ; les documents privés de joueur créés par
  l'utilisateur, y compris ceux rattachés aux personnages qu'il incarnait, sont
  supprimés physiquement (NFR-CONF-03) ; les campagnes et one-shots partagés sont
  conservés sous identité anonymisée ; l'utilisateur est déconnecté et redirigé
  vers la page d'accueil ;
  action disponible uniquement si l'adresse de messagerie est validée (RB-10-05)
  et si aucune campagne active avec membres ne fait obstacle (UC-10 E4)
```

---

### États

```
état confirmable (suppression possible) :
  l'utilisateur est propriétaire d'aucune campagne active avec membres,
  et son adresse de messagerie est validée ;
  la zone de présentation des conséquences s'affiche avec la zone de confirmation ;
  l'utilisateur peut confirmer la suppression

état bloqué (campagnes actives avec membres) :
  l'utilisateur est propriétaire d'au moins une campagne avec des membres actifs ;
  la zone de présentation des conséquences s'affiche ;
  la zone de confirmation est remplacée par la zone de blocage qui indique
  les campagnes concernées ;
  l'utilisateur ne peut pas déclencher la suppression avant d'avoir géré ces campagnes
  (UC-10 E4 ; UC-10 §Règles métier RGPD) ;
  note : la suppression est bloquée uniquement par la présence de campagnes actives
  avec membres — un espace personnel mono-membre ne bloque jamais la suppression

[SOUS-SPÉCIFIÉ — S4 §Suppression de compte (RGPD)]
  wording exact des conséquences présentées à l'utilisateur : non tranché dans le corpus.
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran Suppression de compte.

```
annonce sans action :
  - passage de l'état confirmable à l'état bloqué (ou inversement si l'utilisateur
    a géré une campagne depuis un autre onglet) : annoncé assistivement sans que
    l'utilisateur déplace son focus — NFR-ACC-02
  - message d'erreur de précondition (adresse de messagerie non validée) :
    annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone de présentation des conséquences (information critique avant action)
  priorité 2 — zone de confirmation ou zone de blocage selon l'état courant
  source : NFR-ACC-04
```

---

### Sources

```
Sources : UC-10 A4 ; UC-10 E4 ; UC-10 §Règles métier (RGPD, RB-10-05) ;
          NFR-CONF-03 (suppression effective des données personnelles) ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-10 A4 §Note MVP] transfert de propriété de campagne :
    le déblocage de la suppression passe au MVP par l'exclusion des membres ;
    le transfert de propriété est hors MVP

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S4 §Suppression de compte (RGPD)]
    wording exact des conséquences présentées à l'utilisateur : non tranché dans le corpus
```

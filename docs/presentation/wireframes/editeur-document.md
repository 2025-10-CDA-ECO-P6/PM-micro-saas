# Éditeur de document

> Fiche de description d'écran basse-fidélité — Vague 3, cluster B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.

---

## En-tête de fiche

```
Nom          : Éditeur de document
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-04, UC-07 ; AR-11
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ crée ou modifie un document dans son espace — en lui donnant un titre,
            en choisissant optionnellement un type, en rédigeant du contenu libre en blocs
            et en configurant sa visibilité — afin que ce document soit disponible pour
            la préparation, la session et le partage.
```

---

### Zones et hiérarchie

```
[ ZONE D'EN-TÊTE DU DOCUMENT ]
  type     : principal
  rôle     : saisie du titre du document (obligatoire) et sélection optionnelle du type ;
             le titre est le seul champ obligatoire — le document peut exister avec un titre seul
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-04 §Scénario nominal (étape 5 — titre, type optionnel) ; UC-04 §Règles métier

[ ZONE DE PROPRIÉTÉS STRUCTURÉES ]
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition et affordances de type :
    la zone existe si un type est sélectionné (UC-04 A2), mais la présentation des
    propriétés structurées selon le type n'est pas décrite dans le corpus.
  type     : formulaire
  rôle     : affiche les propriétés structurées associées au type sélectionné,
             sans remplacer ni bloquer la zone de contenu libre
  priorité : secondaire-configurable (présente uniquement si un type est choisi — UC-04 A2)
  visibilité : MJ seul
  ancrage  : UC-04 A2 (document typé) ; UC-04 §Clarification du modèle documentaire

[ ZONE DE CONTENU LIBRE ]
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition des blocs et affordances d'édition :
    UC-04 décrit les types de blocs disponibles (texte, description, liste, checklist,
    tableau, image, séparateur…) mais ne précise pas la disposition de l'interface d'édition
    ni les affordances permettant d'ajouter, réordonner ou supprimer des blocs.
  type     : principal
  rôle     : saisie du contenu du document en blocs libres — les blocs représentent des
             formats variés sans objet dédié pour chaque besoin ; le corps reste libre
             quelle que soit la présence d'un type
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-04 §Clarification du modèle documentaire §Blocs documentaires

[ ZONE DE VISIBILITÉ ET MÉTADONNÉES ]
  type     : formulaire
  rôle     : configuration de la visibilité du document (privé MJ par défaut,
             visible par les joueurs, ou personnelle joueur) ; dossier de rattachement ;
             tags optionnels
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-04 §Scénario nominal (étape 5 — visibilité, tags) ; UC-04 §Règles métier
             (« Tout document créé par le MJ est privé par défaut »)

[ ZONE DES DOCUMENTS LIÉS ]
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] gestion des liens :
    UC-04 décrit le modèle de liens entre documents (un document peut être lié à plusieurs
    autres) mais ne précise pas l'interface de saisie et d'affichage de ces liens.
  type     : latéral
  rôle     : liste les documents explicitement liés à ce document depuis l'espace ;
             permet d'ajouter ou de retirer des liens vers d'autres documents
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-04 §Scénario nominal (étape 8 — lier à d'autres documents) ; UC-04 §Règles métier

[ ZONE « RÉFÉRENCÉ PAR » (BACKLINKS) ]
  [SOUS-SPÉCIFIÉ — S9 §Écran de consultation des backlinks non décrit ; AR-11] :
    la vision §2.2 mentionne les backlinks ; UJ-UC-04 utilise le label « Référencé par » ;
    mais aucun UC ni US ne décrit l'interface de consultation des backlinks —
    AR-11 traite ce placement comme une recommandation révisable, non une décision figée.
  type     : latéral
  rôle     : liste les documents de l'espace qui font référence à ce document ;
             permet de naviguer vers ces documents sans quitter l'éditeur
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-11 (recommandation — placement révisable) ; vision §2.2 (backlinks mentionnés)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-04 §Scénario nominal] saisir le titre du document → le titre devient le repère
  de navigation dans les dossiers, la recherche et les listes

- [UC-04 A2] choisir un type optionnel → les propriétés structurées associées au type
  s'affichent ; le contenu libre en blocs reste accessible et modifiable

- [UC-04 §Clarification du modèle documentaire §Blocs documentaires] composer le contenu
  en blocs libres → le MJ ajoute des blocs de texte, description, liste, checklist,
  tableau, image ou séparateur selon son besoin ; un bloc ne porte pas la visibilité
  dans le MVP

- [UC-04 §Scénario nominal] configurer la visibilité → le document est privé MJ par défaut ;
  le MJ peut le rendre visible par les joueurs ou le garder privé (UC-04 A4 — les règles
  de partage détaillées relèvent d'UC-08)

- [UC-04 §Scénario nominal] rattacher à un dossier → le document appartient à exactement
  un dossier ; s'il n'est pas rattaché explicitement, il est placé dans « Non classés »
  (UC-04 §Postconditions ; UC-05)

- [UC-04 §Scénario nominal] lier à d'autres documents →
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] l'interface de saisie des liens n'est pas
  décrite dans le corpus ; UC-04 établit que le lien est possible, pas comment il se saisit

- [UC-04 A3] créer depuis un template → si le dossier définit un modèle par défaut,
  le document s'ouvre avec une copie indépendante ; les modifications n'affectent pas
  le template source

- [UC-04 §Scénario nominal] sauvegarder le document → le document est enregistré,
  indexé par la recherche et accessible depuis son dossier, ses liens et les vues
  qui consomment la bibliothèque de contenu
```

---

### États

```
état vide (document créé sans contenu) :
  seul le titre est affiché ; la zone de contenu libre est présente mais vide ;
  le document peut être sauvegardé avec un titre seul comme brouillon (UC-04 §E1 —
  un titre suffit à éviter le refus de création)

état chargé (document existant ouvert en modification) :
  le titre, le type éventuel, les propriétés structurées, les blocs de contenu,
  la visibilité et les documents liés s'affichent selon ce qui a été enregistré

état erreur (perte de connexion en mode cloud) :
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document ; NFR-OFF-04] le comportement de l'éditeur en cas de
  perte de connexion (sauvegarde automatique locale, indicateur de brouillon, reprise)
  n'est pas décrit dans le corpus au-delà de UC-03 E2 (conserve les données localement
  si possible) ; cette fiche ne le précise pas davantage
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'éditeur de document.

```
annonce sans action :
  - ajout ou suppression d'un bloc de contenu : annoncé assistivement
    sans que le MJ déplace son focus — NFR-ACC-02
  - changement de visibilité du document (privé MJ → visible par les joueurs) :
    annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone d'en-tête du document (titre — identifiant principal du document)
  priorité 2 — zone de contenu libre (corps du document)
  priorité 3 — zone de propriétés structurées (si un type est sélectionné)
  priorité 4 — zone de visibilité et métadonnées
  source : NFR-ACC-04
```

---

### Sources

```
Sources : UC-04 ; UC-07 (création à la volée depuis la vue session) ;
          AR-11 (backlinks — recommandation révisable) ;
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

Fonctions cloud désactivées sur l'éditeur de document en mode local :
  Châssis standard — voir zoning.md §S7 §Châssis mode local.
  L'éditeur en lui-même (titre, type, blocs, liens, dossier) est disponible sans compte.
  La visibilité « visible par les joueurs » est configurable localement mais le partage
  effectif avec des joueurs nécessite un compte (AR-13 ; UC-01 §Règles métier).
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — moscow.md §Could Have] types de document personnalisés
  [HORS-MVP — UC-13 §Statut] instanciation d'un scénario depuis la bibliothèque
    « Mes scénarios » (post-MVP — AR-08 révisé)

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition des blocs de contenu
    dans l'interface d'édition et affordances permettant d'ajouter, réordonner
    ou supprimer des blocs — UC-04 décrit le modèle, pas l'interface
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] affordances de sélection du type :
    comment le type est présenté et sélectionné (liste, icônes, champ libre…)
    — non décrit dans le corpus
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] interface de saisie et d'affichage
    des liens entre documents — UC-04 établit que le lien est possible (un document
    peut être lié à plusieurs autres) sans décrire comment ce lien se saisit
    dans l'interface
  [SOUS-SPÉCIFIÉ — S9 §Écran de consultation des backlinks non décrit] zone
    « Référencé par » (backlinks) — placement et format non décidés (AR-11 :
    recommandation révisable ; aucun UC ni US ne décrit l'interface)
```

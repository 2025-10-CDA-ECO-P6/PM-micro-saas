# Éditeur de document

> Fiche de description d'écran basse-fidélité — Vague 3, cluster B.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.

---

## En-tête de fiche

```
Nom          : Éditeur de document
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-04, UC-07 ; AR-11 ; AR-19
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
  type     : formulaire
  rôle     : affiche les propriétés structurées associées au type sélectionné,
             sans remplacer ni bloquer la zone de contenu libre ; la zone
             apparaît quand le MJ sélectionne un type ; elle n'est jamais
             imposée et ne conditionne pas l'accès au contenu libre
  présentation du type : le type est un champ discret dans la zone d'en-tête du
             document, vide par défaut (valeur « Aucun type ») ; la sélection
             d'un type révèle cette zone de propriétés structurées ; ne pas
             sélectionner de type est un chemin de premier rang — la non-sélection
             n'est pas un état dégradé (UC-04 A2)
  priorité : secondaire-configurable (présente uniquement si un type est choisi — UC-04 A2)
  densité  : repliée par défaut (divulgation progressive — AR-19) ; accessible
             à la demande
  visibilité : MJ seul
  ancrage  : UC-04 A2 (document typé) ; UC-04 §Clarification du modèle documentaire ;
             AR-19 §Divulgation progressive

[ ZONE DE CONTENU LIBRE ]
  type     : principal
  rôle     : saisie du contenu du document en blocs libres — les blocs représentent des
             formats variés sans objet dédié pour chaque besoin ; le corps reste libre
             quelle que soit la présence d'un type (UC-04 §Clarification)
  disposition : édition en flux de blocs verticaux ; chaque bloc dispose d'une poignée
             apparaissant au survol ou au focus, exposant les actions : ajouter un bloc
             sous celui-ci, supprimer, déplacer vers le haut ou vers le bas ;
             une affordance « + » persistante en fin de document permet d'ajouter
             un bloc après le dernier ; au survol d'un interstice entre deux blocs,
             une affordance « + » contextuelle permet d'insérer un bloc entre les deux ;
             le type de bloc est choisi au moment de la création du bloc via un menu
             inline (texte, description, liste, checklist, tableau, image, séparateur…)
  réordonnancement : le déplacement d'un bloc est possible par déplacement explicite
             (haut/bas via la poignée) et par interaction clavier, de façon à ne pas
             exclure les utilisateurs ne pouvant pas utiliser le glisser-déposer
             (NFR-ACC-01) ; tout réordonnancement est annoncé assistivement (NFR-ACC-02)
  plancher garanti : cette zone est au premier plan en permanence (AR-19 §Plancher garanti) ;
             elle n'est jamais masquée ni reléguée, quelle que soit la configuration
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-04 §Clarification du modèle documentaire §Blocs documentaires ;
             NFR-ACC-01 ; NFR-ACC-02 ; AR-19 §Plancher garanti

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
  type     : latéral
  rôle     : liste les documents explicitement liés à ce document depuis l'espace ;
             permet d'ajouter ou de retirer des liens vers d'autres documents
  interface de saisie : un champ « + lier un document » ouvre une recherche par titre ;
             la sélection d'un résultat crée le lien ; la recherche porte sur le titre
             seul au MVP (cohérent avec UC-14 — recherche titre seul) ;
             les liens existants s'affichent sous forme de liste de titres cliquables
             permettant de naviguer vers le document lié
  priorité : secondaire-configurable
  densité  : repliée par défaut (divulgation progressive — AR-19) ; accessible à la demande ;
             tant que l'interface de liaison n'est pas figée, cette zone ne s'affiche pas
             au premier plan
  visibilité : MJ seul
  ancrage  : UC-04 §Scénario nominal (étape 8 — lier à d'autres documents) ;
             UC-04 §Règles métier ; UC-14 (recherche titre seul) ; AR-19 §Divulgation progressive

[ ZONE « RÉFÉRENCÉ PAR » (BACKLINKS) ]
  type     : latéral secondaire
  rôle     : liste les documents de l'espace qui font référence à ce document ;
             lecture seule — un backlink se crée automatiquement quand un autre
             document crée un lien vers celui-ci (cohérent AR-11) ;
             permet de naviguer vers ces documents sans quitter l'éditeur
  placement : zone latérale secondaire, sous la zone des documents liés ;
             jamais imposée au premier plan
  densité  : repliée par défaut (divulgation progressive — AR-19) ; accessible à la demande ;
             tant que l'interface de backlinks n'est pas figée, cette zone ne s'affiche pas
             au premier plan
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-11 (placement révisable — décision retenue : zone latérale secondaire,
             lecture seule) ; vision §2.2 (backlinks mentionnés) ; AR-19 §Divulgation progressive
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

- [UC-04 §Scénario nominal] lier à d'autres documents → depuis la zone latérale
  « Documents liés », le MJ utilise le champ « + lier un document » pour rechercher
  par titre ; la sélection d'un résultat crée le lien ; les documents liés s'affichent
  sous forme de liste de titres cliquables (UC-14 — recherche titre seul au MVP)

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
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document ; NFR-OFF-04] comportement de l'éditeur
    en cas de perte de connexion (sauvegarde automatique locale, indicateur de brouillon,
    reprise) — non décrit dans le corpus au-delà de UC-03 E2 ; point d'interview
    produit non couvert par les décisions actuelles
```

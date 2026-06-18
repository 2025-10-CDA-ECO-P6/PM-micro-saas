# Éditeur de scénario

> Fiche de description d'écran basse-fidélité — Vague 3, cluster B.
> Cas spécialisé de l'éditeur de document — instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..17`.
> Cas général : `docs/presentation/wireframes/editeur-document.md`.

---

## En-tête de fiche

```
Nom          : Éditeur de scénario
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-03
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ structure un scénario en lui donnant ses informations générales
            (titre, résumé, contexte, objectif narratif, statut) et en lui associant
            des scènes et des documents liés — afin que le scénario soit exploitable
            pendant une session.
```

---

### Zones et hiérarchie

L'éditeur de scénario est un cas spécialisé de l'éditeur de document. Il hérite de toutes les zones décrites dans `editeur-document.md` et les enrichit de zones spécifiques à la structure narrative. Les zones communes (en-tête, contenu libre, visibilité, documents liés, backlinks) ne sont pas répétées ici ; seules les zones propres au scénario sont décrites.

```
[ ZONE D'INFORMATIONS GÉNÉRALES DU SCÉNARIO ]
  type     : formulaire
  rôle     : saisie des métadonnées narratives du scénario — titre (obligatoire),
             résumé, contexte, objectif narratif, statut de préparation ;
             ces champs sont des propriétés structurées du type SCENARIO
             (UC-03 §Données manipulées §Scénario)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-03 §Scénario nominal (étape 5)

[ ZONE DES SCÈNES ]
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition de la liste de scènes
    et affordances de navigation entre scènes : UC-03 décrit le modèle (une scène
    est un document de type SCENE lié au scénario, avec titre, description, objectif,
    ordre d'affichage, notes privées, informations partageables, documents liés) mais
    ne précise pas comment ces scènes sont présentées et naviguées dans l'interface.
  type     : principal
  rôle     : liste les scènes associées au scénario dans leur ordre d'affichage ;
             chaque scène est un document lié ; le MJ peut ajouter, modifier ou
             supprimer une scène depuis cette zone ; une scène peut référencer
             des documents liés (PNJ, lieux, objets, révélations, notes)
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-03 §Scénario nominal (étapes 6-7) ; UC-03 §Données manipulées §Scène ;
             UC-03 §Règles métier

[ ZONE D'ÉDITION DE SCÈNE (focus) ]
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] présentation et disposition de la zone
    d'édition d'une scène sélectionnée — comment l'édition d'une scène s'articule
    avec la liste des scènes et le contenu du scénario parent n'est pas décrit
    dans le corpus.
  type     : principal
  rôle     : édition du contenu d'une scène sélectionnée — titre, description,
             objectif, notes privées MJ, informations partageables, documents liés
             à la scène ; contenu libre en blocs accessible en parallèle
  priorité : principal (présente si une scène est sélectionnée ou créée)
  visibilité : MJ seul
  ancrage  : UC-03 §Scénario nominal (étape 7) ; UC-03 §Données manipulées §Scène
```

---

### Ce que l'utilisateur peut faire

```
- [UC-03 §Scénario nominal] saisir les informations générales du scénario (titre
  obligatoire, résumé, contexte, objectif narratif, statut) → les métadonnées
  narratives du scénario sont enregistrées comme propriétés structurées du document

- [UC-03 §Scénario nominal] ajouter une scène → une nouvelle scène (document de
  type SCENE lié au scénario) est créée avec son titre ; elle peut être ordonnancée
  et enrichie de documents liés (UC-03 §Règles métier)

- [UC-03 A2] lier des documents existants au scénario ou à une scène → des documents
  déjà présents dans l'espace (PNJ, lieux, notes, révélations, aides de jeu)
  sont associés au scénario ou à la scène courante

- [UC-03 A3] créer un document depuis l'éditeur de scénario → un nouveau document
  est créé directement et automatiquement lié au scénario ou à la scène courante

- [UC-03 A1] rédiger le scénario sans découpage en scènes → le MJ compose un
  document monobloc en utilisant uniquement la zone de contenu libre, sans utiliser
  la zone des scènes

- [UC-03 A4] créer un scénario minimal pendant ou juste avant une session → titre
  seul obligatoire ; quelques blocs libres suffisent à un scénario improvisé

- [UC-03 §Scénario nominal] sauvegarder le scénario → le scénario est enregistré
  comme document de l'espace, placé dans le dossier Scénarios (dossier système dans
  les espaces CAMPAIGN et ONE_SHOT ; dossier créé librement dans l'espace PERSONAL),
  consultable, modifiable et associable à une session
```

---

### États

```
état vide (scénario créé sans contenu) :
  seul le titre est affiché dans la zone d'informations générales ;
  la zone des scènes est vide ; le scénario peut être sauvegardé avec
  un titre seul (UC-03 §E1 — le titre est obligatoire, le reste est facultatif)

état chargé (scénario existant ouvert en modification) :
  les informations générales, les scènes dans leur ordre d'affichage,
  et les documents liés s'affichent selon ce qui a été enregistré

état erreur (perte de connexion ou erreur de sauvegarde) :
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document ; NFR-OFF-04] le comportement de l'éditeur en cas
  de perte de connexion n'est pas décrit en détail dans le corpus pour ce cas ;
  UC-03 E2 mentionne que le système conserve les données saisies localement
  si possible et affiche un message d'erreur
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'éditeur de scénario.

```
annonce sans action :
  - ajout ou suppression d'une scène dans la liste des scènes :
    annoncé assistivement sans que le MJ déplace son focus — NFR-ACC-02
  - changement d'ordre des scènes : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone d'informations générales (titre du scénario)
  priorité 2 — zone des scènes (liste des scènes dans leur ordre narratif)
  priorité 3 — zone d'édition de scène sélectionnée
  source : NFR-ACC-04
```

---

### Sources

```
Sources : UC-03 ;
          AR-11 (backlinks — hérité de editeur-document.md) ;
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

Fonctions cloud désactivées sur l'éditeur de scénario en mode local :
  Châssis standard — voir zoning.md §S7 §Châssis mode local.
  La création et la modification d'un scénario et de ses scènes sont disponibles
  sans compte. Le partage du scénario avec des joueurs (visibilité visible par
  les joueurs) nécessite un compte (AR-13 ; UC-01 §Règles métier).
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-13 §Statut] instanciation d'un scénario depuis la bibliothèque
    « Mes scénarios » — l'interface de bibliothèque est post-MVP (AR-08 révisé)

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition de la liste de scènes
    et affordances de navigation entre scènes dans l'interface — UC-03 décrit
    le modèle (scènes liées, ordre d'affichage, documents liés par scène),
    pas la disposition de l'interface
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] articulation entre la zone d'édition
    du scénario parent et la zone d'édition de la scène sélectionnée —
    comment ces deux niveaux d'édition coexistent sur la même surface
    n'est pas décrit dans le corpus
  [SOUS-SPÉCIFIÉ — S9 §Éditeur de document] affordances de l'interface de saisie
    des liens entre documents (liens vers PNJ, lieux, objets, révélations) —
    hérité du cas général ; voir editeur-document.md §Hors-périmètre
```

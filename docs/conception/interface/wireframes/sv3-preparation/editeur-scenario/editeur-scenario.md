# Éditeur de scénario

> Fiche de description d'écran basse-fidélité — préparation espace partagé.
> Cas spécialisé de l'éditeur de document — instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6 AR-01..21`.
> Cas général : `docs/conception/interface/wireframes/sv3-preparation/editeur-document/editeur-document.md`.

---

## En-tête de fiche

```
Nom          : Éditeur de scénario
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-03 ; AR-19
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

Héritage des décisions de conception (résolutions UX) : la disposition en flux de blocs verticaux, les affordances d'ajout/suppression/réordonnancement de blocs (NFR-ACC-01), la sélection optionnelle du type via champ discret, l'interface de saisie des liens par recherche-titre inline (UC-14), la zone « Référencé par » en lecture seule, et le principe de densité AR-19 (plancher garanti, plafond borné, divulgation progressive) s'appliquent à l'éditeur de scénario selon les mêmes termes que l'éditeur de document — voir `editeur-document.md §Zones et hiérarchie`.

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
  type     : principal — rail latéral gauche (liste-détail)
  rôle     : liste les scènes associées au scénario dans leur ordre d'affichage ;
             chaque scène est un document lié ; le MJ peut ajouter, modifier ou
             supprimer une scène depuis cette zone ; une scène peut référencer
             des documents liés (PNJ, lieux, objets, révélations, notes)
  disposition : colonne/rail de scènes ordonnée à gauche ; la sélection d'une scène
             ouvre la scène en zone d'édition focus (zone centrale) ;
             le réordonnancement des scènes est possible par déplacement explicite
             (annoncé assistivement — NFR-ACC-02) ;
             si le scénario est monobloc (UC-03 A1 — aucune scène associée),
             le rail n'est pas affiché : seule la zone de contenu libre est présente
  présence : conditionnelle — absente si le scénario ne contient aucune scène
             (monobloc, UC-03 A1) ; présente dès qu'au moins une scène existe
  priorité : principal
  visibilité : MJ seul
  ancrage  : UC-03 §Scénario nominal (étapes 6-7) ; UC-03 §Données manipulées §Scène ;
             UC-03 §Règles métier ; UC-03 A1 (scénario monobloc) ; NFR-ACC-02

[ ZONE D'ÉDITION DE SCÈNE (focus) ]
  type     : principal — zone centrale
  rôle     : édition du contenu d'une scène sélectionnée — titre, description,
             objectif, notes privées MJ, informations partageables, documents liés
             à la scène ; contenu libre en blocs accessible en parallèle
  articulation deux niveaux : les deux niveaux (scénario parent et scène) coexistent
             sur une seule surface sans changement d'écran (principe S1) ;
             le panneau gauche porte les informations générales du scénario et le
             rail de scènes ; la zone centrale affiche l'édition de la scène active ;
             si aucune scène n'est sélectionnée, la zone centrale affiche le contenu
             libre du scénario parent ; un fil d'Ariane « Scénario › Scène N »
             signale le niveau en cours d'édition et permet de remonter au niveau
             scénario sans quitter l'éditeur
  présence : conditionnelle — présente si une scène est sélectionnée ou créée ;
             remplacée par la zone de contenu libre du scénario si aucune scène
             n'est sélectionnée
  priorité : principal (présente si une scène est sélectionnée ou créée)
  visibilité : MJ seul
  ancrage  : UC-03 §Scénario nominal (étape 7) ; UC-03 §Données manipulées §Scène ;
             principe S1 (surface unique)
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
  le contenu en cours d'édition est conservé dans l'interface ; une notification non
  bloquante indique que la synchronisation est en attente ; le MJ peut continuer à
  saisir et à modifier le contenu ; dès le retour de la connexion, les modifications
  en attente sont synchronisées sans action du MJ, dans l'ordre de leur saisie
  (NFR-OFF-05 — continuité d'édition en préparation cloud ; hérité de editeur-document.md,
  cf. UC-03 E2 pour la formulation d'origine limitée à la sauvegarde locale)
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
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-03 ;
          AR-11 (backlinks — hérité de editeur-document.md) ;
          NFR-ACC-02 ; NFR-OFF-05 ;
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
  [HORS-MVP — UC-13 en-tête] instanciation d'un scénario depuis la bibliothèque
    « Mes scénarios » — l'interface de bibliothèque est post-MVP (AR-08 révisé)

Éléments sous-spécifiés (S9) :
  RÉSOLU — le comportement de l'éditeur en cas de perte de connexion en préparation
    cloud est désormais couvert par NFR-OFF-05 (continuité d'édition en préparation
    cloud), hérité de `editeur-document.md` ; voir §États §état erreur ci-dessus.
```

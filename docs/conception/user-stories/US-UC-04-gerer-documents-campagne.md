# Epic — Créer et gérer des documents de campagne modulaires

## Objectif utilisateur

Permettre au MJ de créer, modifier, organiser, retrouver et éventuellement partager des documents
de campagne modulaires : notes libres, scénarios, scènes, lore, lieux, PNJ, personnages joueurs,
objets, factions, aides de jeu, rappels ou idées. Les notes sont un usage particulier du document
de campagne : le type est une aide optionnelle, la structure reste libre en blocs.

---

## Personas concernés

| Persona | Motivation principale |
|---|---|
| Émilie | Capturer vite, note rapide en 2 touches, réorganiser après |
| Antoine | Créer des documents détaillés avec types et propriétés, lier ses documents |
| Nadia | Peu de création, mais retrouver ses documents après 6 semaines d'absence |
| Thomas | Backlinks et organisation libre style Obsidian |
| Rémi | Usage partiel, léger, utile sans reconstruire toute sa campagne |
| Sonia | Documents ponctuels réutilisables ou liés à un scénario one-shot |

---

## Use cases couverts

- **UC-04** — Créer et gérer des documents de campagne modulaires
  - Nominal : création complète avec blocs, type optionnel, propriétés, visibilité, tags et liens
  - A1 : note rapide sans titre (titre généré côté interface)
  - A2 : document typé
  - A3 : document créé depuis un template
  - A4 : changement de visibilité après création
  - A5 : document durable utilisé pendant une session

---

## Priorité MoSCoW

| Priorité | Stories |
|---|---|
| Must Have | US-04-01, US-04-02, US-04-03 |
| Should Have | US-04-04, US-04-05 |
| Could Have | US-04-06 |
| Won't Have (MVP) | Partage sélectif par joueur |

---

## Bounded contexts pressentis

- Création et modification des documents de campagne
- Classement dans les dossiers
- Liens entre documents
- Tags, recherche et visibilité

---

## Vue d'ensemble Mermaid

```mermaid
flowchart TD
    A[MJ ouvre sa campagne] --> B[Accède à la bibliothèque\nou à un dossier]
    B --> C{Mode de création}
    C -->|Création complète| D[Renseigne titre, type optionnel,\npropriétés, visibilité, tags]
    C -->|Note rapide| E[Titre généré automatiquement\ncôté interface]
    D --> F[Rédige le contenu libre blocs]
    E --> F
    F --> G{Liens ?}
    G -->|Oui| H[Lie des documents existants]
    G -->|Non| I[Définit la visibilité]
    H --> I
    I -->|Privé par défaut| J[Sauvegarde]
    I -->|Visible joueurs| J
    J --> K[Document accessible depuis dossier,\nrecherche et documents liés]
    K --> L{Action ultérieure}
    L -->|Changer la visibilité| M[Partage ou retrait]
    L -->|Consulter les backlinks| N[Voir les documents\nqui référencent ce doc]
```

---

## Dépendances Mermaid

```mermaid
flowchart LR
    US0401[US-04-01\nCréer et modifier\nun document]
    US0402[US-04-02\nNote rapide]
    US0403[US-04-03\nVisibilité]
    US0404[US-04-04\nTaguer]
    US0405[US-04-05\nLier des documents]
    US0406[US-04-06\nBacklinks]

    UC03[UC-03\nStructurer un scénario]
    UC05[UC-05\nGérer les dossiers]
    UC14[UC-14\nRecherche]

    US0401 --> US0402
    US0401 --> US0403
    US0401 --> US0404
    US0401 --> US0405
    US0405 --> US0406
    US0404 --> UC14
    US0405 --> UC03
    US0401 --> UC05
```

---

## User stories

### US-04-01 — Créer et modifier un document de campagne

**Priorité** : Must Have

**En tant que** MJ,  
**je veux** créer un document modulaire dans ma campagne, y rédiger un contenu libre en blocs
et le modifier à tout moment,  
**afin de** disposer d'un espace durable pour représenter mes éléments de campagne sans structure imposée.

**Notes de conception** :
- Le type de document est optionnel. Les types proposés peuvent inclure note, PNJ, lieu,
  personnage joueur, scénario, scène, note de session ou révélation.
- UC-04 porte le modèle documentaire général. Les workflows narratifs des scénarios et scènes
  restent dans UC-03 ; les notes de session restent dans UC-06/UC-07.
- Le dossier d'accueil dépend du point d'entrée : dossier courant, dossier système adapté, dossier "Notes" pour une note rapide, ou dossier virtuel "Non classés" si aucun dossier explicite n'est choisi.
- Un document créé est privé par défaut.
- La suppression MVP masque le document de la campagne active sans imposer de suppression physique immédiate.
- Le contenu est structuré en blocs libres. Aucune contrainte de format n'est imposée.
- Les propriétés structurées dépendent du type et ne remplacent jamais les blocs.

**Règles métier** :
- RB-04-01 : Un document créé est privé par défaut.
- RB-04-02 : Le MJ peut supprimer un document durable de sa campagne active.
- RB-04-03 : Un document peut être déplacé dans n'importe quel dossier de la campagne après création.
- RB-04-04 : Un document peut rester libre, sans type.
- RB-04-05 : Un document typé conserve un corps libre en blocs.

**Critères d'acceptation** :
- [ ] Le MJ peut créer un document depuis la bibliothèque, un dossier ou un raccourci de création.
- [ ] Le titre est le seul champ obligatoire à la création.
- [ ] Le type, les propriétés, les tags et la visibilité sont optionnels à la création.
- [ ] Un document créé sans configuration explicite est privé.
- [ ] Le MJ peut modifier le titre, les blocs, les propriétés et le type d'un document existant.
- [ ] Un document supprimé n'est plus visible dans la campagne active (suppression logique).

```gherkin
Scénario : Le MJ supprime un document
  Étant donné que le MJ dispose d'un document visible dans sa campagne
  Quand il choisit de supprimer ce document
  Alors le document est masqué de la bibliothèque active
  Et il n'apparaît plus dans les listes courantes

Scénario : Le MJ déplace un document vers un autre dossier
  Étant donné que le MJ dispose d'un document dans un dossier
  Quand il déplace ce document vers un autre dossier de la campagne
  Alors le document apparaît dans le dossier cible
  Et le document n'apparaît plus dans le dossier source
```

---

### US-04-02 — Créer une note rapide sans configuration

**Priorité** : Must Have

**En tant que** MJ,  
**je veux** créer une note immédiatement sans remplir de formulaire,  
**afin de** capturer une idée ou une information en session sans interrompre le jeu.

**Notes de conception** :
- La logique de titre généré est **interface uniquement** : le système conserve toujours un titre non vide.
- Format de titre suggéré : "Note — {date} {heure}" (ex. "Note — 15 mai 2026 21:34"), ou les N premiers caractères du contenu si l'utilisateur a saisi du texte avant de déclencher la sauvegarde.
- La note rapide est un document standard, pas un objet fonctionnel séparé. Elle est créée dans le dossier "Notes" par défaut.
- Le MJ peut renommer la note après création (couvert par US-04-01).
- Cible principale : Émilie (capture rapide) et Nadia/Rémi (faible configuration). L'objectif UX est d'atteindre l'éditeur en 2 interactions depuis la vue campagne ou depuis la vue session.

**Note de conception — intégration UC-06/UC-07** : L'accès depuis la vue session est un point
d'intégration avec le parcours de session. La note rapide durable reste un document de campagne.
Une note de session est créée dans le contexte d'une session et traitée par UC-06/UC-07.

**Critères d'acceptation** :
- [ ] Le MJ peut créer une note sans renseigner aucun champ.
- [ ] Un titre est généré automatiquement côté interface si aucun titre n'est fourni.
- [ ] La note est placée dans le dossier "Notes" par défaut.
- [ ] La note est un document standard : elle peut être retrouvée, modifiée et liée comme n'importe quel autre document.
- [ ] Le MJ peut renommer la note après création.
- [ ] La note rapide est accessible depuis la bibliothèque de campagne ET depuis la vue session.

---

### US-04-03 — Contrôler la visibilité d'un document

**Priorité** : Must Have

**En tant que** MJ,  
**je veux** définir si un document est privé ou visible par tous les membres de ma campagne,  
**afin de** partager certains éléments avec mes joueurs tout en gardant mes notes pour moi.

**Notes de conception** :
- Le MJ peut rendre un document visible par les joueurs.
- Le MJ peut retirer ce partage et rendre le document à nouveau privé.
- Partage = tous les membres de la campagne et accès invités actifs. Pas de granularité par joueur en MVP — décision actée.
- Un document partagé reste accessible entre les sessions (pas un partage temporaire).
- Seul le MJ peut modifier la visibilité d'un document de campagne.
- Les notes personnelles joueur ne sont pas un flux actif du MJ dans UC-04.

**Critères d'acceptation** :

```gherkin
Scénario : Le MJ partage un document
  Étant donné que le MJ dispose d'un document privé
  Quand il choisit de partager ce document
  Alors le document devient visible par les joueurs
  Et le document est accessible par tous les membres de la campagne

Scénario : Le MJ retire le partage d'un document
  Étant donné que le MJ dispose d'un document partagé
  Quand il choisit de retirer le partage
  Alors le document redevient privé
  Et le document n'est plus visible par les joueurs

Scénario : Un joueur ne voit pas un document privé
  Étant donné qu'un document est privé
  Quand un joueur consulte la bibliothèque de la campagne
  Alors le document n'apparaît pas dans sa vue
```

- [ ] Le MJ peut définir la visibilité à la création.
- [ ] Le MJ peut modifier la visibilité d'un document existant.
- [ ] Un document privé n'est pas visible par les joueurs.
- [ ] Un document partagé est accessible par tous les membres actifs de la campagne.
- [ ] Seul le MJ peut modifier la visibilité d'un document de campagne.

---

### US-04-04 — Taguer un document

**Priorité** : Should Have

**En tant que** MJ,  
**je veux** associer des tags à mes documents,  
**afin de** les retrouver rapidement par thème lors de la recherche.

**Notes de conception** :
- Les tags sont associés directement au document.
- Les tags servent principalement à la recherche et au filtrage (UC-14 Should Have). Un document sans tags reste pleinement utilisable.
- Dépendance légère avec UC-14 : les tags n'ont de valeur significative que lorsque la recherche est disponible.
- La gestion du référentiel de tags (création, suppression, renommage) est hors périmètre UC-04.

**Critères d'acceptation** :
- [ ] Le MJ peut ajouter un ou plusieurs tags à un document à la création ou après.
- [ ] Le MJ peut retirer un tag d'un document.
- [ ] Les tags d'un document sont utilisables comme critères de filtrage dans la recherche (UC-14).
- [ ] Un document sans tags est pleinement fonctionnel.

---

### US-04-05 — Lier un document à d'autres éléments de campagne

**Priorité** : Should Have

**En tant que** MJ,  
**je veux** lier mes documents entre eux,  
**afin de** naviguer entre un PNJ, ses scènes, ses notes associées, sans dupliquer le contenu.

**Notes de conception** :
- Le lien part d'un document vers un ou plusieurs autres documents.
- Exemples : PNJ vers scène, note vers scénario, lieu vers PNJ.
- Les documents liés peuvent être de n'importe quel type disponible dans la campagne.
- Complémentaire à US-03-05 qui lie depuis un scénario ou une scène vers d'autres documents.
- Dépend de UC-05 pour la navigation dans les dossiers lors de la sélection d'un document à lier.

**Critères d'acceptation** :
- [ ] Le MJ peut rechercher et lier un document existant de la campagne depuis la fiche d'un document.
- [ ] Les documents liés sont visibles depuis l'éditeur du document source.
- [ ] Le MJ peut supprimer un lien sans supprimer le document cible.
- [ ] Un document peut être lié à plusieurs autres documents simultanément.

---

### US-04-06 — Consulter les backlinks d'un document

**Priorité** : Could Have

**En tant que** MJ,  
**je veux** voir quels documents référencent le document que je consulte,  
**afin de** naviguer dans le graphe de ma campagne et comprendre les dépendances entre éléments.

**Notes de conception** :
- Les backlinks sont la liste des documents qui pointent vers le document courant.
- Performance à surveiller sur les PNJ très référencés.
- Valeur principale pour Thomas (graphe de relations style Obsidian).
- Dépend de US-04-05 : sans liens créés, il n'y a pas de backlinks à afficher.

**Critères d'acceptation** :
- [ ] Le MJ peut voir la liste des documents qui référencent le document courant.
- [ ] Les backlinks sont accessibles depuis la fiche du document (section dédiée ou panneau latéral).
- [ ] Un clic sur un backlink navigue vers le document source.
- [ ] Si aucun document ne référence le document courant, la section est vide (pas d'erreur).

---

## Stories exclues ou repoussées

| Story / Feature | Raison |
|---|---|
| Partage sélectif par joueur | Hors périmètre MVP. Le partage vise le groupe. Décision actée. |
| Workflow des notes de session | Couvert par UC-06/07. UC-04 garde le modèle général des documents de campagne. |
| Suppression physique définitive d'un document | Hors périmètre MVP. Le besoin métier décrit surtout le retrait de la bibliothèque active. |
| Déplacement d'un document entre dossiers | DÉCIDÉ : le déplacement de dossier est possible. Couvert par US-04-01 (RB-04-03). |
| Graphe de relations visuel | UX non définie. Les backlinks (US-04-06) posent le socle technique, la visualisation est une étape ultérieure. |
| Visibilité par bloc | Hors périmètre MVP. La visibilité est au niveau document ; un contenu joueur doit être un document lié. |

---

## Ordre de livraison recommandé

1. **US-04-01** — Créer et modifier un document (socle, bloque tout le reste)
2. **US-04-02** — Note rapide (valeur immédiate pour Émilie, coût faible)
3. **US-04-03** — Visibilité (nécessaire avant tout partage avec les joueurs)
4. **US-04-05** — Lier des documents (dépend US-04-01, valeur pour Antoine et Thomas)
5. **US-04-04** — Taguer (Should Have, dépend UC-14 pour sa valeur pleine)
6. **US-04-06** — Backlinks (Could Have, dépend US-04-05)

---

## Vérification de couverture

| Cas UC-04 | Story couvrant |
|---|---|
| Nominal — création complète | US-04-01 + US-04-03 + US-04-04 + US-04-05 |
| A1 — note rapide sans titre | US-04-02 |
| A2 — document typé | US-04-01 |
| A3 — document depuis template | UC-05 + US-04-01 |
| A4 — changement de visibilité | US-04-03 |
| A5 — document utilisé en session | US-04-01 + UC-06/07 |
| Backlinks | US-04-06 |

---

## Questions ouvertes

1. **Suppression physique** : non retenue pour le MVP. Le besoin couvert est le retrait de la bibliothèque active.
2. **Déplacement de dossier** : DÉCIDÉ : le déplacement de dossier est possible.
3. **Note rapide depuis la vue session** : DÉCIDÉ : note rapide accessible depuis la bibliothèque et depuis la vue session.

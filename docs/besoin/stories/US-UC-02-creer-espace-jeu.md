# Epic — Créer un espace de jeu (UC-02)

- **Gherkin** : Scénario, Étant donné que, Quand, Alors, Et, Mais

---

## Objectif utilisateur

Permettre à un MJ de créer un espace de travail pour organiser une campagne longue ou lancer un one-shot, avec un niveau de configuration adapté au contexte. L'objectif est de minimiser la friction à la création, d'offrir un parcours express pour les one-shots (< 30 secondes), et de permettre une configuration progressive pour les campagnes.

Les espaces de type `CAMPAIGN` et `ONE_SHOT` sont créés par ces parcours. L'espace de type `PERSONAL` préexiste : il est créé automatiquement à la création du compte (ou fourni comme conteneur par défaut en mode local) et n'est pas accessible via ces parcours de création.

---

## Personas concernés

| Persona | Profil | Douleur principale | Motivation principale |
|---|---|---|---|
| **Nadia** | MJ infirmière, 42 ans, session mensuelle | Configuration initiale trop longue | Bénéficiaire directe — campagne créable avec juste un nom |
| **Antoine** | MJ multi-campagnes, 3 systèmes différents | Structure imposée et non agnostique | Espace agnostique sans système de jeu obligatoire |
| **Sonia** | MJ one-shot exclusif, 3 à 5 one-shots/mois | Lancer un one-shot prend trop de temps | Parcours express one-shot < 30 secondes |
| **Émilie** | MJ improvisatrice | Devoir tout configurer avant de commencer | Création minimaliste, enrichissement en cours de route |
| **Thomas** | MJ Obsidian | Migration de contenu forcée | Espace créé sans importer de contenu |

---

## Use cases couverts

- **UC-02** — Créer un espace de jeu (intégralité du périmètre)
- **UC-13** — Scénario réutilisable (dépendance externe pour US-02-04)

---

## Priorité MoSCoW

La priorité MoSCoW se lit exclusivement dans la matrice de priorités, quel qu'en soit le grain : ici dans [`moscow.md`, § UC-02 — Créer et configurer un espace (campagne ou one-shot)](../vision/perimetre/matrice-de-priorites.md) — seule autorité du corpus pour l'attribuer (voir [`docs/reference/ordre-d-autorite.md`, § Ordre d'autorité entre artefacts](../../reference/ordre-d-autorite.md), point 1 : les use cases sont la source de vérité du besoin, les user stories en dérivent et s'y conforment). Cette fiche ne répartit donc pas de priorité propre par story : une répartition au grain story recopierait une valeur que seul `moscow.md` a l'autorité d'accorder, et qu'une révision ultérieure de ce document laisserait alors périmée ici sans le savoir.

**Source de la priorité** : Matrice de priorités

---

## Vue d'ensemble

```mermaid
flowchart TD
    A[MJ accède au tableau de bord] --> B{Action choisie}
    B -- Nouvelle campagne --> C[Formulaire de création\nnom obligatoire\ndescription et système optionnels]
    B -- Lancer un one-shot --> D{Scénario existant ?}
    C --> E{Formulaire valide ?}
    E -- Non, nom manquant --> F[Message d'erreur\nchamp nom requis]
    F --> C
    E -- Oui --> G{Quota atteint ?}
    G -- Oui --> H[Blocage avec message contextuel\net CTA upgrade — US-02-03]
    G -- Non --> I[Espace créé]
    I --> J[la bibliothèque de contenu crée\n4 dossiers nommés + dossier virtuel Non classés]
    J --> K[Redirection vers l'espace créé]
    D -- Non, nouveau scénario --> L[Saisie du titre uniquement]
    L --> M{Quota atteint ?}
    M -- Oui --> H
    M -- Non --> N[Espace créé]
    N --> O[Redirection vers l'éditeur de scénario]
    D -- Oui, scénario existant --> P[Sélection dans la bibliothèque\nUS-02-04]
    P --> Q{Quota atteint ?}
    Q -- Oui --> H
    Q -- Non --> R[Espace créé]
    R --> S[Redirection vers la vue session]
```

### Bounded contexts pressentis

- **Space Management** — création de l'espace, application des règles métier (quota, ownership, type), publication de création d'espace
- **la bibliothèque de contenu** — réception de création d'espace, création des dossiers système (Personnages, Joueurs, Scénarios, Notes, Non classés)
- **Identity & Access** — vérification de l'id du MJ propriétaire, plan d'abonnement (gratuit/PRO)
- **la conduite de session** — redirection vers la vue session à l'issue du parcours one-shot depuis scénario existant (US-02-04)

La création des dossiers système n'appartient pas à Space Management. Ce périmètre fonctionnel publie uniquement l'événement création d'espace. C'est la bibliothèque de contenu qui réagit à cet événement pour créer les dossiers système.

---

## Dépendances

```mermaid
flowchart LR
    US0200[US-02-00\nEspace personnel\npar défaut]
    US0201[US-02-01\nCréer une campagne]
    US0202[US-02-02\nOne-shot express\npost-MVP]
    US0203[US-02-03\nBlocage quota]
    US0204[US-02-04\nOne-shot depuis bibliothèque\npost-MVP]
    UC13[UC-13\nScénario réutilisable\npost-MVP]

    US0201 --> US0203
    US0202 --> US0203
    UC13 --> US0204
    US0202 --> US0204
```

---

## User stories

---

### US-02-00 — Disposer d'un espace personnel par défaut

**En tant que** : MJ,  
**je veux** : je dispose d'un espace personnel (`PERSONAL`) sans avoir à le créer,  
**afin de** : avoir un espace de travail privé disponible dès l'ouverture du compte ou de l'application.

**Notes de conception** :

**Métadonnées**

| Champ | Valeur |
|---|---|
| Source | UC-02 — ontologie des types d'espace, décision ADR-018 |
| Bounded context | Space Management, Identity & Access |

- La création automatique de l'espace `PERSONAL` est déclenchée par Space Management au moment de la création du compte (ou de l'initialisation locale). Ce n'est pas un événement déclenché par le MJ.
- L'espace `PERSONAL` ne doit pas apparaître dans les parcours de création d'espace (US-02-01, US-02-02). Il est visible dans le tableau de bord comme espace distinct.

**Règles métier** :

- RB-02-18 : L'espace `PERSONAL` est créé automatiquement à la création du compte. En mode local, il est fourni comme conteneur par défaut. Le MJ ne passe jamais par un parcours de création pour cet espace.
- RB-02-19 : L'espace `PERSONAL` n'est pas décompté dans le quota d'espaces actifs (`CAMPAIGN`/`ONE_SHOT`). Il est hors quota pour tous les plans (gratuit, PRO, local).
- RB-02-20 : L'espace `PERSONAL` reçoit uniquement le dossier virtuel « Non classés ». Il ne reçoit pas les 4 dossiers nommés (Personnages, Joueurs, Scénarios, Notes).

**Critères d'acceptation** :

```gherkin
# Fonctionnalité : Espace personnel par défaut

  Scénario: Le MJ crée un compte et dispose immédiatement d'un espace personnel
    Étant donné qu'un MJ vient de créer son compte
    Quand il accède à son tableau de bord pour la première fois
    Alors un espace de type PERSONAL est déjà présent
    Et cet espace contient le dossier virtuel "Non classés"
    Et aucune action de création n'a été nécessaire de la part du MJ

  Scénario: Le MJ en mode local dispose d'un espace personnel comme conteneur par défaut
    Étant donné que le MJ utilise l'application en mode local sans compte
    Quand il accède à l'application pour la première fois
    Alors un espace PERSONAL est présent comme conteneur par défaut
    Et cet espace contient le dossier virtuel "Non classés"

  Scénario: L'espace personnel n'apparaît pas dans le quota FREE
    Étant donné que le MJ possède un compte gratuit
    Et son espace personnel PERSONAL est actif
    Quand le système calcule le quota d'espaces actifs
    Alors l'espace PERSONAL n'est pas décompté dans le quota
    Et le MJ dispose bien de 3 emplacements CAMPAIGN/ONE_SHOT disponibles
```

---

### US-02-01 — Créer une campagne depuis le tableau de bord

**En tant que** : MJ,  
**je veux** : créer un espace de campagne depuis mon tableau de bord en renseignant uniquement un nom,  
**afin de** : commencer à préparer ma campagne sans configuration longue.

**Notes de conception** :

**Métadonnées**

| Champ | Valeur |
|---|---|
| Source | UC-02 — scénario nominal |
| Bounded context | Space Management, la bibliothèque de contenu |

- La création d'un espace de jeu publie l'événement création d'espace. Space Management ne crée pas les dossiers système directement. C'est la bibliothèque de contenu qui consomme création d'espace et déclenche la création des 4 dossiers nommés et du dossier virtuel. Ce point d'architecture transverse doit être documenté dans les contrats d'intégration.
- Le `type` est positionné à `CAMPAIGN` pour ce scénario. L'utilisateur ne voit jamais ce détail technique.
- L'absence de système de jeu est le comportement par défaut. Aucune valeur pré-sélectionnée ne doit orienter le MJ vers un système spécifique.

**Règles métier** :

- RB-02-01 : Le nom est obligatoire pour créer une campagne. Description et système de jeu sont facultatifs.
- RB-02-02 : L'espace créé appartient à un seul propriétaire avec le rôle `MemberRole.OWNER`.
- RB-02-03 : Les 4 dossiers système nommés (Personnages, Joueurs, Scénarios, Notes) et le dossier virtuel « Non classés » sont créés automatiquement pour un espace partagé (`CAMPAIGN` ou `ONE_SHOT`). Les dossiers nommés sont renommables et supprimables (`isSystem` est informatif, non restrictif).
- RB-02-04 : Un espace créé sans système de jeu est en mode générique — comportement recommandé par défaut.
- RB-02-05 : Un espace créé en mode local est pleinement fonctionnel, au même titre qu'un espace cloud.

**Critères d'acceptation** :

```gherkin
# Fonctionnalité : Création d'une campagne

  Scénario: Le MJ crée une campagne avec le nom uniquement
    Étant donné que le MJ est connecté et accède à son tableau de bord
    Et son quota d'espaces CAMPAIGN/ONE_SHOT actifs n'est pas atteint
    Quand il choisit "Nouvelle campagne"
    Et il renseigne uniquement un nom
    Et il valide
    Alors un espace de campagne est créé
    Et les 4 dossiers système nommés sont créés automatiquement : Personnages, Joueurs, Scénarios, Notes
    Et le dossier virtuel "Non classés" est créé automatiquement
    Et le MJ est redirigé vers le nouvel espace

  Scénario: Le MJ crée une campagne avec tous les champs
    Étant donné que le MJ est connecté et accède à son tableau de bord
    Et son quota d'espaces CAMPAIGN/ONE_SHOT actifs n'est pas atteint
    Quand il choisit "Nouvelle campagne"
    Et il renseigne un nom, une description courte et un système de jeu
    Et il valide
    Alors un espace de campagne est créé avec les informations fournies
    Et les 4 dossiers système nommés et le dossier virtuel "Non classés" sont créés automatiquement

  Scénario: Le MJ tente de créer une campagne sans renseigner de nom
    Étant donné que le MJ accède au formulaire de création de campagne
    Quand il valide sans renseigner de nom
    Alors un message d'erreur indique que le nom est obligatoire
    Et aucun espace n'est créé

  Scénario: Le MJ crée une campagne sans système de jeu
    Étant donné que le MJ accède au formulaire de création de campagne
    Quand il valide sans renseigner de système de jeu
    Alors l'espace est créé en mode générique
    Et les 4 dossiers système nommés et le dossier virtuel "Non classés" sont créés avec leurs labels neutres

  Scénario: Le MJ est en mode local et crée une campagne
    Étant donné que le MJ utilise l'application en mode local sans compte
    Quand il crée une campagne avec un nom
    Alors l'espace est créé et pleinement fonctionnel
    Et les 4 dossiers système nommés et le dossier virtuel "Non classés" sont créés automatiquement
```

---

### US-02-02 — Lancer un one-shot en parcours express

> **Post-MVP** — Cette story décrit le parcours express one-shot (point d'entrée dédié « Lancer un one-shot »). En première livraison, un one-shot se crée via le parcours campagne nominal avec `type = ONE_SHOT` (US-02-01). La livraison de cette story est conditionnée à celle de UC-13 — voir vision §5bis et UC-02 §A1.

**En tant que** : MJ,  
**je veux** : lancer un one-shot depuis le tableau de bord en moins de 30 secondes avec un titre uniquement,  
**afin de** : ne pas perdre de temps en configuration avant une partie.

**Notes de conception** :

**Métadonnées**

| Champ | Valeur |
|---|---|
| Source | UC-02 — scénario alternatif A1 |
| Bounded context | Space Management |

- Le parcours "Lancer un one-shot" doit être mesuré de bout en bout (depuis le clic sur l'action jusqu'à la redirection finale) pour vérifier le critère < 30 secondes. Il s'agit d'un critère d'acceptation mesurable.

**Règles métier** :

- RB-02-06 : Un one-shot est techniquement un espace avec type `ONE_SHOT`. Cette distinction n'est pas exposée à l'utilisateur.
- RB-02-07 : Pour un one-shot avec nouveau scénario, le titre est le seul champ obligatoire.
- RB-02-08 : L'archivage d'un one-shot est manuel, comme pour une campagne.
- RB-02-09 : Un espace archivé n'est pas supprimé définitivement.

**Critères d'acceptation** :

```gherkin
# Fonctionnalité : Lancement d'un one-shot en parcours express

  Scénario: Le MJ lance un one-shot avec un nouveau scénario
    Étant donné que le MJ est connecté et accède à son tableau de bord
    Et son quota d'espaces CAMPAIGN/ONE_SHOT actifs n'est pas atteint
    Quand il choisit "Lancer un one-shot"
    Et il saisit uniquement un titre de scénario
    Et il valide
    Alors un espace one-shot est créé
    Et le MJ est redirigé vers l'éditeur de scénario

  Scénario: Le parcours one-shot est complété en moins de 30 secondes
    Étant donné que le MJ est connecté et accède à son tableau de bord
    Quand il choisit "Lancer un one-shot" et saisit un titre
    Et il valide
    Alors l'espace est créé et le MJ est redirigé en moins de 30 secondes depuis l'action initiale
```

---

### US-02-03 — Être bloqué et guidé quand la limite d'espaces est atteinte

**En tant que** : MJ,  
**je veux** : recevoir un message clair et contextuel quand je ne peux pas créer de nouvel espace,  
**afin de** : comprendre pourquoi je suis bloqué et de savoir comment débloquer la situation.

**Notes de conception** :

**Métadonnées**

| Champ | Valeur |
|---|---|
| Source | UC-02 — règles métier, règle stable gratuit |
| Bounded context | Space Management |

- La limite du compte gratuit (3 espaces `CAMPAIGN`/`ONE_SHOT` actifs maximum) est vérifiée à la création côté serveur. Le rejet est une règle fonctionnelle, pas seulement une validation d'écran.
- Le mode local n'est soumis à aucun quota d'espaces : il n'y a pas de compte utilisateur côté serveur en mode local, et aucun plafond de création n'y existe (arbitrage produit, RB-01-03 retirée dans US-UC-01). Cette story ne couvre donc que le blocage du compte gratuit face au plan PRO.

**Règles métier** :

- RB-02-10 : Un utilisateur gratuit ne peut pas avoir plus de 3 espaces `CAMPAIGN`/`ONE_SHOT` actifs simultanément. L'espace `PERSONAL` n'est pas décompté. Cette règle stable est vérifiée côté serveur dans Space Management.
- RB-02-11 : **Retirée** (arbitrage produit, cf. US-UC-01 RB-01-03). Cette règle prétendait qu'un cap à 3 espaces `CAMPAIGN`/`ONE_SHOT` s'appliquait aussi en mode local, comme règle d'interface. UC-01 ne porte aucun plafond de comptage en mode local : la seule contrainte du mode local est la capacité de stockage du navigateur. L'identifiant `RB-02-11` n'est pas réattribué.
- RB-02-12 : Le message de blocage informe le MJ gratuit qu'il a atteint la limite de 3 espaces `CAMPAIGN`/`ONE_SHOT` actifs et l'invite à passer en PRO. Ce blocage ne concerne que le compte gratuit — le mode local n'est jamais bloqué par un quota d'espaces (RB-01-03 retirée, US-UC-01).
- RB-02-13 : Les espaces archivés ne comptent pas dans le quota actif.
- RB-02-14 : Un utilisateur PRO ne rencontre jamais ce blocage.

**Critères d'acceptation** :

```gherkin
# Fonctionnalité : Blocage à la limite d'espaces CAMPAIGN/ONE_SHOT actifs

  Scénario: Le MJ gratuit tente de créer un 4e espace CAMPAIGN ou ONE_SHOT actif
    Étant donné que le MJ possède un compte gratuit
    Et il a déjà 3 espaces CAMPAIGN/ONE_SHOT actifs
    Quand il tente de créer une nouvelle campagne ou de lancer un one-shot
    Alors la création est bloquée
    Et un message indique qu'il a atteint la limite de 3 espaces CAMPAIGN/ONE_SHOT actifs pour un compte gratuit
    Et un CTA l'invite à passer en PRO pour bénéficier d'un nombre illimité d'espaces
    Et son espace PERSONAL n'est pas mentionné dans ce décompte

  Scénario: Le MJ gratuit peut créer un nouvel espace après avoir archivé un espace
    Étant donné que le MJ possède un compte gratuit
    Et il a 3 espaces CAMPAIGN/ONE_SHOT actifs
    Quand il archive l'un de ses espaces CAMPAIGN ou ONE_SHOT
    Et il tente de créer une nouvelle campagne
    Alors la création est autorisée

  Scénario: Le MJ PRO n'est jamais bloqué
    Étant donné que le MJ possède un compte PRO
    Et il a déjà 3 espaces CAMPAIGN/ONE_SHOT actifs ou plus
    Quand il tente de créer une nouvelle campagne
    Alors la création est autorisée sans restriction

  Scénario: Le MJ en mode local n'est jamais bloqué par un quota d'espaces
    Étant donné que le MJ utilise l'application en mode local sans compte
    Et il a déjà créé 3 espaces CAMPAIGN/ONE_SHOT
    Quand il crée un 4e espace CAMPAIGN ou ONE_SHOT
    Alors la création est autorisée sans restriction
    Et aucun message de blocage n'est affiché
```

---

### US-02-04 — Lancer un one-shot depuis un scénario de bibliothèque

**En tant que** : MJ,  
**je veux** : lancer un one-shot directement depuis un scénario de ma bibliothèque,  
**afin de** : démarrer une session sans re-saisir le titre ni naviguer manuellement vers l'espace.

**Notes de conception** :

**Métadonnées**

| Champ | Valeur |
|---|---|
| Source | UC-02 — scénario alternatif A1, option "scénario existant" |
| Bounded context | Space Management, la bibliothèque de contenu, la conduite de session |

- Cette story dépend de UC-13 (scénario réutilisable). Elle ne peut être livrée qu'après stabilisation d'UC-13. Marquer en bloquée si UC-13 n'est pas livré.
- La redirection vers la vue session implique la conduite de session. L'intégration transverse (Space Management → la conduite de session) doit être définie dans les contrats d'intégration.
- Le scénario sélectionné est associé à l'espace one-shot nouvellement créé. La bibliothèque de contenu doit exposer la liste des scénarios disponibles pour la sélection dans ce parcours.

**Règles métier** :

- RB-02-15 : Lors d'un one-shot depuis scénario existant, le nom de l'espace est pré-rempli avec le titre du scénario.
- RB-02-16 : La redirection est directe vers la vue session (UC-06) — aucune étape intermédiaire.
- RB-02-17 : Le quota d'espaces `CAMPAIGN`/`ONE_SHOT` actifs est vérifié de la même façon que pour les autres créations.

**Critères d'acceptation** :

```gherkin
# Fonctionnalité : One-shot depuis un scénario de bibliothèque

  Scénario: Le MJ lance un one-shot depuis un scénario existant
    Étant donné que le MJ est connecté et accède à son tableau de bord
    Et son quota d'espaces CAMPAIGN/ONE_SHOT actifs n'est pas atteint
    Quand il choisit "Lancer un one-shot"
    Et il sélectionne un scénario existant dans sa bibliothèque
    Et il valide
    Alors un espace one-shot est créé
    Et le nom du scénario est pré-rempli comme nom de l'espace
    Et le MJ est redirigé directement vers la vue session

  Scénario: Le nom de l'espace est pré-rempli depuis le titre du scénario
    Étant donné que le MJ sélectionne un scénario existant pour un one-shot
    Quand l'espace est créé
    Alors le nom de l'espace correspond au titre du scénario sélectionné
    Et le MJ peut modifier ce nom avant de valider

  Scénario: Le MJ est redirigé vers la vue session sans étape intermédiaire
    Étant donné que le MJ a sélectionné un scénario existant
    Quand il valide la création du one-shot
    Alors il est redirigé directement vers la vue session (UC-06)
    Et aucune étape de configuration supplémentaire n'est présentée
```

---

## Stories exclues ou repoussées

| Story / Feature | Raison |
|---|---|
| **US-02-02** | Post-MVP. Le parcours express one-shot (point d'entrée dédié) est reporté après la première livraison. En MVP, le one-shot se crée via le parcours campagne nominal avec `type = ONE_SHOT` (US-02-01). Conditionné à la livraison de UC-13. |
| **US-02-04** | Non bloquante pour le MVP, dépend de UC-13. |
| **US-02-05 — Sauvegarder une campagne en brouillon** | Exclue. Hors MVP. Le formulaire de création est minimal (1 champ obligatoire). Si implémenté, ce sera un état purement interface (brouillon local) sans statut DRAFT dans le modèle fonctionnel. |

---

## Ordre de livraison recommandé

**MVP (première livraison) :**

1. **US-02-00** — Espace personnel par défaut (socle ontologique, aucune action requise du MJ)
2. **US-02-01** — Créer une campagne (fondation de l'epic, couvre aussi la création de one-shot via `type = ONE_SHOT`)
3. **US-02-03** — Blocage quota (corollaire obligatoire de US-02-01, la création sans garde est risquée)

**Post-MVP (après livraison de UC-13) :**

4. **US-02-02** — One-shot express (parcours express dédié, conditionné à UC-13)
5. **US-02-04** — One-shot depuis bibliothèque (conditionné à UC-13 et US-02-02)

---

## Vérification de couverture

| Cas | Story couvrant |
|---|---|
| A1 — One-shot express (nouveau scénario) | US-02-02 |
| A1 — One-shot depuis scénario existant | US-02-04 |
| A2 — Brouillon | US-02-05 (exclue, hors MVP) |
| A3 — Sans système de jeu | US-02-01 |

| Règle métier UC-02 | Story couvrant la règle |
|---|---|
| Un espace appartient à un seul propriétaire (MemberRole.OWNER) | US-02-01 |
| Nom obligatoire pour les campagnes | US-02-01 |
| Nom pré-rempli pour un one-shot depuis scénario existant | US-02-04 |
| Dossiers système créés automatiquement, renommables et supprimables (`isSystem` informatif) | US-02-01 |
| Espace PERSONAL : uniquement dossier virtuel "Non classés" | US-02-00, US-02-01 (RB-02-03, RB-02-20) |
| One-shot archivable manuellement par le MJ, comme une campagne | US-02-02 |
| Un espace peut être archivé sans suppression définitive | US-02-02, US-02-03 |
| Espace PERSONAL préexiste, non créé par le MJ | US-02-00 |
| Quota FREE : 3 espaces CAMPAIGN/ONE_SHOT, PERSONAL non décompté | US-02-00, US-02-03 |
| Aucun plafond de création en mode local (RB-01-03 retirée, US-UC-01) | US-02-01, US-02-03 |

| Critère d'acceptation UC-02 | Story couvrant le critère |
|---|---|
| MJ peut créer une campagne avec juste un nom | US-02-01 |
| MJ peut lancer un one-shot en moins de 30 secondes | US-02-02 |
| One-shot depuis scénario existant redirige vers la vue session | US-02-04 |
| Les 4 dossiers nommés + dossier virtuel "Non classés" existent | US-02-01 |
| Un espace créé en mode local est pleinement fonctionnel | US-02-01 |
| One-shot archivable manuellement (comme une campagne) | US-02-02 |
| MJ dispose d'un espace PERSONAL sans le créer | US-02-00 |

**Absence de RB-02-21, RB-02-22 et RB-02-23 dans les tables ci-dessus** : SOUS-SPÉCIFIÉ. Ces trois règles (archivage refusé tant qu'une session `LIVE` est en cours, désarchivage, garde de quota au désarchivage) sont définies dans UC-02. Elles y ont été ajoutées le 2026-09-02 (commit f4214d1), le même jour où cette fiche était retouchée, sans qu'aucune story, ni aucune ligne de couverture, ni la matrice de priorités ne les reprenne. Aucun commit n'enregistre une coupe volontaire : c'est un trou connu, pas une coupe volontaire.

---

## Questions ouvertes — CLOSES

1. **Séparation visuelle campagnes / one-shots dans le tableau de bord**

**Résolution** : Liste/grille unifiée de cartes-espaces avec **indicateur de type par carte** (CAMPAIGN/ONE_SHOT). L'espace personnel préexiste en tête, distinct du quota (hors quota). Cohérent avec la persona Sonia qui pense en scénarios, pas en catégories d'espaces.

**Renvoi** : wireframe `tableau-de-bord.md` §S9 ; UC-02 Postconditions pour les règles métier (RB-02-19 : PERSONAL hors quota).

**Validation terrain (DIFFÉRÉE)** : La validation du terrain — le MJ perçoit-il bien la distinction visuelle entre campagnes et one-shots ? — reste reportée en interview utilisateur (UC-02 §Questions à valider). Non bloquante MVP.

---

2. **Renommage des dossiers système à la création**

**Résolution** : Les dossiers système ne sont renommables que **après création de l'espace**, non dès le formulaire.

**Justification** : Les dossiers système (Personnages, Joueurs, Scénarios, Notes) naissent sur l'événement `SpaceCreated` (ils n'existent pas au moment du formulaire). Le formulaire de création reste minimal — 1 champ obligatoire (nom) — pour friction nulle et parcours express. Les renommer dès le formulaire impliquerait une couche de configuration prématurée, contraire au design. Le renommage intervient après création, dans l'espace créé (par ailleurs autorisé : RB-02-03, `isSystem` informatif).

**Règle métier afférente** : RB-02-03 — Dossiers système renommables et supprimables (post-création).

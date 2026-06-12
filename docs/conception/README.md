# Haversack — Conception

Index de toute la documentation de conception produit et domaine.

---

## Ordre d'autorité entre artefacts

En cas de conflit entre deux artefacts de conception, l'artefact le plus en amont de la hiérarchie suivante fait foi :

**personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire**

### Trois conséquences directes

1. Les **use cases** sont la source de vérité du besoin. Les user journeys, user stories et NFR en dérivent et s'y conforment.
2. Le **domaine** modélise la résolution du besoin — il s'y conforme. Il ne dicte pas le besoin.
3. Le **glossaire n'est pas une autorité de fond** — c'est un outil de nommage dérivé. Une entrée de glossaire qui contredit un use case est l'entrée à corriger.

### Distinction autorité de forme / autorité de fond

Ces deux notions sont distinctes et ne se confondent pas :

- **Autorité de forme (nommage)** — légitime pour le glossaire : une fois un terme décidé au niveau du besoin, le glossaire est la référence de forme (terme retenu, identifiant, orthographe). « Nommer » n'est pas « arbitrer ». Tout artefact (UC, US, UJ, diagramme) doit employer les termes du glossaire.
- **Autorité de fond (le besoin lui-même)** — réservée à la hiérarchie ci-dessus. Le glossaire ne tranche jamais un conflit de besoin. Si une définition de glossaire semble contredire un use case, c'est la définition de glossaire qui est à corriger.

---

## Sens de dépendance besoin ↔ décision

### Filtre de nature : conception vs implémentation

Avant d'ajouter une règle métier, un critère d'acceptation ou un invariant de domaine dans ces artefacts, appliquer le test suivant :

**Cette décision contraint-elle le modèle de domaine ou une règle métier autoportante dès maintenant ?**

- **Oui** → la règle appartient à la conception. L'ADR peut la motiver, jamais la remplacer. Elle est exprimée en langage de besoin, sans référence à un choix technologique ou d'implémentation.
- **Non** → c'est un choix d'implémentation. Il n'a pas sa place dans cette couche.

### Test d'autoportance : dépistage des inversions

Masquer mentalement la mention d'un ADR ou d'une technologie dans une règle métier.

- **La règle reste complète et compréhensible** → le lien est informatif. C'est l'usage sain : une règle de besoin renvoie vers un ADR pour justifier son arbitrage, pas pour la définir.
- **La règle devient vide ou orpheline** → l'inversion est consommée. La substance vit dans l'ADR, pas dans le besoin. À corriger : rapatrier la règle en langage besoin, conserver l'ADR comme trace du raisonnement.

### Motifs interdits — détection par balayage

Deux motifs, détectables à la lecture dans les sections de règles métier, critères d'acceptation et invariants de domaine :

1. **ADR cité comme source d'autorité** : formulations du type « voir ADR-XX », « résolu par ADR-XX », « conformément à ADR-XX » à l'intérieur d'une règle métier, quand masquer cette mention rend la règle incomplète. Le lien sain va dans le sens inverse : une règle métier autonome peut renvoyer vers un ADR qui l'a motivée.

2. **Nom de technologie, infrastructure, API ou outil** : références à des noms d'interface applicative, de format de stockage, de protocole, de bibliothèque ou de mécanisme système, dans les sections de règles ou de critères. Exemple interdit : nommer la technologie de persistance du navigateur dans une règle. Exemple correct : « les données sont durables après fermeture du navigateur » (comportement observable).

### Responsable et fréquence d'application

Chaque contributeur applique ce balayage au fichier qu'il vient de modifier, **avant toute intégration**, pour vérifier qu'il n'a pas introduit l'un des deux motifs. Pas d'outillage imposé — la détection est manuelle et rapide (relecture guidée par les motifs).

---

## Vision & Produit

| Fichier | Contenu |
|---|---|
| [vision/vision-produit.md](vision/vision-produit.md) | Positionnement produit, acteurs, choix assumés, modèle de monétisation |
| [vision/moscow.md](vision/moscow.md) | Matrice MoSCoW complète (Must / Should / Could / Won't Have) |
| [nfr/](nfr/README.md) | Exigences non fonctionnelles produit, en langage besoin — un fichier détaillé par exigence (performance perçue, hors connexion, confidentialité, accessibilité, internationalisation) |

---

## Personas

7 profils utilisateurs de référence utilisés pour valider les choix fonctionnels.

| Fichier | Persona |
|---|---|
| [persona/persona-01-thomas.md](persona/persona-01-thomas.md) | Thomas — MJ préparateur |
| [persona/persona-02-emilie.md](persona/persona-02-emilie.md) | Émilie — MJ impro |
| [persona/persona-03-lucas.md](persona/persona-03-lucas.md) | Lucas — joueur sans compte |
| [persona/persona-04-nadia.md](persona/persona-04-nadia.md) | Nadia — MJ occasionnelle |
| [persona/persona-05-antoine.md](persona/persona-05-antoine.md) | Antoine — MJ avancé |
| [persona/persona-06-remi.md](persona/persona-06-remi.md) | Rémi — MJ débutant |
| [persona/persona-07-sonia.md](persona/persona-07-sonia.md) | Sonia — MJ one-shot |

→ [Vue d'ensemble des personas](persona/README.md)

---

## Use Cases (UC)

14 use cases MVP couvrant l'ensemble des fonctionnalités de la première version.

| UC | Titre |
|---|---|
| [UC-01](usecases/UC-01-mode-local-sans-compte.md) | Mode local sans compte |
| [UC-02](usecases/UC-02-creer-espace-jeu.md) | Créer un espace de jeu |
| [UC-03](usecases/UC-03-structurer-scenario.md) | Structurer un scénario |
| [UC-04](usecases/UC-04-gerer-documents-campagne.md) | Gérer les documents d'une campagne |
| [UC-05](usecases/UC-05-organiser-dossiers.md) | Organiser par dossiers |
| [UC-06](usecases/UC-06-vue-session.md) | Vue session |
| [UC-07](usecases/UC-07-creation-volee-session.md) | Création à la volée en session |
| [UC-08](usecases/UC-08-partager-information.md) | Partager une information aux joueurs |
| [UC-09](usecases/UC-09-acces-session-joueur.md) | Accès session joueur |
| [UC-10](usecases/UC-10-compte-cloud.md) | Compte cloud |
| [UC-11](usecases/UC-11-gerer-membres-campagne.md) | Gérer les membres d'une campagne |
| [UC-12](usecases/UC-12-rejoindre-campagne.md) | Consulter sa campagne en tant que joueur (vue post-accès) |
| [UC-13](usecases/UC-13-scenario-reutilisable.md) | Scénario réutilisable |
| [UC-14](usecases/UC-14-recherche.md) | Recherche |
| [UC-HORS-MVP](usecases/UC-HORS-MVP.md) | Fonctionnalités exclues du MVP |

→ [Index use cases](usecases/README.md) · [Diagrammes de cas d'utilisation](usecases/use-cases.md)

---

## User Stories (US)

User stories détaillées avec critères d'acceptation et règles métier (RB-XX) pour chaque UC.

→ [Index user stories](user-stories/README.md)

---

## User Journeys (UJ)

Parcours utilisateur pas à pas pour chaque use case, du point de vue des personas.

→ [Index user journeys](user-journeys/README.md)

---

## Parcours bout-en-bout

Couture transverse des use cases du point de vue de chaque persona (les 7) : fil narratif de bout en bout, coutures inter-UC vérifiées, points de friction. Les UJ restent la source de vérité par use case ; les parcours les tissent.

→ [Index des parcours](parcours/README.md)

---

## Domaine DDD

Modélisation Domain-Driven Design — 4 bounded contexts + shared kernel.

| Fichier | Bounded context | Responsabilité |
|---|---|---|
| [domain/core.md](domain/core.md) | Core (Shared Kernel) | Abstractions DDD, IDs typés, value objects transverses |
| [domain/identity-access.md](domain/identity-access.md) | Identity & Access | Comptes utilisateurs, authentification, tiers, suppression RGPD |
| [domain/campaign-management.md](domain/campaign-management.md) | Campaign Management | Campagnes, one-shots, membres, invitations, accès invités |
| [domain/content-library.md](domain/content-library.md) | Content Library | Documents, dossiers, types de documents, références entre documents |
| [domain/session-conduct.md](domain/session-conduct.md) | Session Conduct | Cycle de vie de session, tableau de bord configurable, notes de session |

→ [Index domaine](domain/README.md)

---

## Stack technique

→ [docs/architecture/stack.md](../architecture/stack.md) — Choix technologiques, justifications, points forts/faibles, roadmap clients. Document de couche décision, déplacé vers `docs/architecture/` (CP-02).

---

## Autres

- [glossaire.md](glossaire.md) — Glossaire du langage ubiquitaire (4 bounded contexts, termes écartés et équivalents retenus)
- [INTERVIEW_GUIDE.md](INTERVIEW_GUIDE.md) — Guide d'entretiens utilisateurs

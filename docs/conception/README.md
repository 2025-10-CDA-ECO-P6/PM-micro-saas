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

## besoin/

Tout ce qui exprime le besoin produit : personas, vision, use cases, user stories, user journeys, NFR, parcours bout-en-bout — dans l'ordre d'autorité posé ci-dessus.

→ [Index du besoin](besoin/README.md)

---

## domain/

Modélisation Domain-Driven Design du besoin — 4 bounded contexts et un shared kernel. En aval du besoin : le domaine s'y conforme, il ne le dicte pas.

→ [Index domaine](domain/README.md)

---

## interface/

Conception de l'interface du MVP : couche réflexive (arbitrages, conventions, rationale) et artefacts dérivés (wireframes). En aval du besoin : elle le traduit en surfaces décrites, elle ne l'arbitre pas.

→ [Index interface](interface/README.md)

---

## Autres fichiers de ce dossier

- [glossaire.md](glossaire.md) — Glossaire du langage ubiquitaire (4 bounded contexts, termes écartés et équivalents retenus)
- [INTERVIEW_GUIDE.md](INTERVIEW_GUIDE.md) — Guide d'entretiens utilisateurs

---

## Stack technique

→ [docs/architecture/stack.md](../architecture/stack.md) — Choix technologiques, justifications, points forts/faibles, roadmap clients.

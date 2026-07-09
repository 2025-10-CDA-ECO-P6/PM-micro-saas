# 01 — Fondations DDD

> Pourquoi DDD a été choisi pour Haversack, et ce que les concepts clés
> signifient concrètement dans ce projet.

---

## Pourquoi DDD

Haversack gère un domaine riche et évolutif : espaces, scénarios, PNJ,
personnages, sessions, partage de contenu, systèmes de jeu différents.
Sans structure explicite, ce type de domaine dérive vers une "big ball of mud" —
un monolithe sans frontières claires où tout dépend de tout, impossible à faire
évoluer sans régression.

Le Domain-Driven Design (DDD) est une approche de conception qui place le
**modèle métier** au centre de l'architecture. L'objectif n'est pas d'utiliser
des patterns pour leur beauté technique, mais de créer un modèle qui reflète
fidèlement les règles et concepts du domaine métier.

DDD impose trois disciplines concrètes :

**Un langage ubiquitaire** — les mêmes termes dans le code, la documentation
et les conversations. `Space`, `Session`, `Document` dans le code, pas `Project`,
`Event`, `Record`. Quand le code parle le même langage que le métier,
les malentendus disparaissent et le code se documente lui-même.

**Des frontières explicites** — le domaine est découpé en Bounded Contexts
indépendants. Chaque contexte a sa propre logique, ses propres règles, son propre
modèle. Une modification dans un contexte n'affecte pas les autres.

**Des règles métier dans le domaine** — les invariants (un espace a exactement
un OWNER, une session LIVE est unique par espace) sont dans les entités et agrégats,
pas dans les services applicatifs ou les contrôleurs.

### Ce que DDD n'est pas

DDD n'est pas un dogme. Sur ce projet, l'objectif est de créer un domaine mature
et maintenable, pas de cocher des cases. Certains compromis sont assumés et documentés
dans les ADR. Voir le [registre des décisions](decisions/README.md).

---

## Les concepts clés appliqués au projet

### Entité

Une entité est un objet du domaine qui a une **identité propre et un cycle de vie**.
Deux entités avec les mêmes données restent deux entités distinctes parce qu'elles
ont des identifiants différents.

Sur ce projet, toutes les entités ont un **Id typé** (jamais un UUID nu) et embarquent
un `AuditInfo` (qui a créé, qui a modifié, quand).

Exemples : `Space`, `Session`, `Document`, `DocumentBlock`.

### Value Object

Un value object est un objet défini uniquement par ses **valeurs**.
Deux value objects avec les mêmes données sont identiques.
Un value object est **immuable** — on ne le modifie pas, on le remplace.

L'intérêt : encapsuler la validation et la logique dans le type lui-même.
Un `Email` invalide ne peut pas être instancié. Un `Slug` est toujours lowercase
avec des tirets. Cette logique n'est pas dans un service ou un validateur externe —
elle est dans le type, une fois, testée une fois.

Exemples : `Email`, `Slug`, `AuditInfo`, `SoftDelete`.

### Agrégat

Un agrégat est un **groupe d'entités traitées comme une unité de cohérence**.
Il a une racine (l'agrégat racine) qui garantit les invariants de tout le groupe.
On ne modifie jamais une entité enfant directement — on passe toujours par la racine.

Exemples : `Space` garantit qu'il y a exactement un OWNER parmi ses membres.
`Document` garantit l'ordre de ses `DocumentBlock` et la cohérence de leur visibilité.

La règle de décision pour savoir si une entité mérite d'être agrégat racine
s'appuie sur la cohérence transactionnelle : toutes les modifications doivent pouvoir
être traitées comme une unité logique et persistées atomiquement.

### Domain Event

Un domain event est un fait métier passé, immuable.
"La session a démarré", "le titre du document a changé", "un membre a rejoint".

Les domain events servent à deux choses sur ce projet :

**Découpler les effets de bord** — quand la visibilité d'un `Document` change, le domain event
`DocumentVisibilityChanged` est émis. Un handler dans Session Conduct met à jour la vue
joueur en temps réel. Le Document ne sait pas qui écoute.

**Tracer l'historique** — les events peuvent alimenter un log d'audit ou,
plus tard, un bus d'événements pour le temps réel.

### Repository

Un repository est l'interface entre le domaine et la persistance.
Le domaine définit l'interface (`IDocumentRepository`), l'infrastructure l'implémente.
Le domaine ne sait pas comment ses entités sont persistées.

### Les Id typés — pourquoi c'est important

```
// Mauvais — rien n'empêche de confondre les Id à la compilation
void AssignOwner(Guid spaceId, Guid userId) { ... }
AssignOwner(userId, spaceId); // compile, bug en production

// Bon — erreur de compilation si on inverse
void AssignOwner(SpaceId spaceId, UserId userId) { ... }
AssignOwner(userId, spaceId); // erreur de compilation
```

Chaque agrégat et entité avec repository a son propre type d'identifiant wrappant
un UUID. C'est une ligne de code par type, mais elle élimine toute une classe de bugs.
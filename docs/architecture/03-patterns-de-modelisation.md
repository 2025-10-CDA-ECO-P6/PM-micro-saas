# 03 — Patterns de modélisation

> Comment décider si une entité est un agrégat racine ou non,
> le modèle Document modulaire, et comment les références cross-context fonctionnent.

---

## Règle de décision pour les agrégats

La question à poser pour chaque entité candidate :

> Est-ce que cette entité protège des invariants sur une collection d'entités enfants,
> ou a-t-elle un cycle de vie indépendant avec des règles qui lui sont propres ?

Si la réponse est non aux deux, c'est une **entité avec repository**, pas un agrégat racine.

### Tableau de décision appliqué au projet

| Entité | Entités enfants | Invariants propres forts | Décision |
|---|---|---|---|
| `Campaign` | `CampaignMembership`, `Invitation` | Exactement un OWNER | Agrégat racine |
| `Document` | `DocumentBlock` | Ordre des blocs, cohérence visibilité | Agrégat racine |
| `Scenario` | `Scene` (cardinalité 0..*) | Ordre des scènes, order persisté | Agrégat racine |
| `Session` | `LiveNote`, `SessionSummary` | Session LIVE unique par campagne | Agrégat racine |
| `DocumentTemplate` | `BlockSchema` | Combinaisons scope/owner valides | Agrégat racine |
| `GameSystem` | Aucune | Immuabilité si BUILTIN | Agrégat racine |
| `NPC` | Aucune | Statut uniquement | Entité avec repository |
| `PlayerCharacter` | Aucune | Ownership uniquement | Entité avec repository |

### Pourquoi NPC et PlayerCharacter ne sont pas des agrégats racines

On pourrait penser que `NPC` et `PlayerCharacter` méritent le statut d'agrégat
racine parce qu'ils ont un rôle important dans le domaine. Mais la question
n'est pas "est-ce que cette entité est importante" — c'est "est-ce qu'elle protège
des invariants transactionnels sur ses enfants".

`NPC` n'a pas d'entités enfants. Son contenu vit dans `Document`.
Son seul invariant propre est son statut (`ALIVE`, `DEAD`...) et son lien
optionnel vers un `PlayerCharacter`. Ces invariants sont locaux à l'entité
elle-même — ils ne nécessitent pas de frontière transactionnelle étendue.

En pratique, les deux ont un `Id` propre et un `IRepository`.
La différence est dans la sémantique : un agrégat racine garantit la cohérence
d'un groupe d'entités dans une transaction, une entité avec repository est
sauvegardée et chargée indépendamment.

---

## Le modèle Document modulaire

### Le problème qu'il résout

Un MJ de D&D 5e a des fiches personnage avec Force, Dextérité, Constitution.
Un MJ de Call of Cthulhu a des fiches avec Santé Mentale et compétences en %.
Un MJ de Fate a des Aspects et des approches nommées par le jeu.

Si on modélise une fiche avec des colonnes fixes en base, on est bloqué sur
un système de jeu. Si on crée une table par système, la complexité explose.

La solution choisie : **le modèle Document modulaire inspiré de Notion**.

### Le principe

Tout contenu éditorial est un `Document` composé de `DocumentBlock` typés.
Chaque bloc a un `kind` et une `value` structurée selon ce kind.

```
Document (type: NPC, title: "Aragorn")
├── Block (kind: TEXT,     label: "Description")
│   └── value: { content: "Rôdeur du Nord..." }
├── Block (kind: FIELD,    label: "Race")
│   └── value: { value: "Homme du Nord" }
├── Block (kind: STAT_BAR, label: "Points de vie")
│   └── value: { current: 45, max: 45 }
└── Block (kind: RELATION, label: "Allié de")
    └── value: { targetId: "uuid-gandalf", targetType: "NPC" }
```

Pour D&D 5e, on ajoute des blocs `FIELD` pour Force, Dextérité, etc.
Pour Call of Cthulhu, un bloc `STAT_BAR` pour la Santé Mentale.
Pour Fate, des blocs `TEXT` pour les Aspects.
**Aucune migration de schéma** pour ajouter un nouveau type de bloc ou supporter
un nouveau système de jeu.

### Les enveloppes métier

`Document` n'est pas utilisé directement pour les PNJ, personnages et scénarios.
Ces entités ont une **enveloppe relationnelle** qui porte leurs règles métier propres :

```
NPC (enveloppe)                    Document (contenu)
├── id: NpcId                      ├── id: DocumentId
├── campaignId: CampaignId  ───▶   ├── type: NPC
├── documentId: DocumentId         ├── blocks: [...]
├── name: String (dénorm.)         └── visibility: PRIVATE
├── status: NpcStatus
└── linkedCharacterId?
```

L'enveloppe sert aux requêtes efficaces (lister tous les PNJ vivants d'une campagne
sans charger tous leurs blocs) et aux règles métier spécifiques (un NPC DEAD reste
consultable, les blocs privés ne sont jamais exposés aux joueurs).

### Pourquoi BlockValue est une hiérarchie de types, pas un objet avec champs optionnels

Version naïve à éviter :

```csharp
class BlockValue {
    string? Content;    // pour TEXT
    string? Value;      // pour FIELD
    int? Current;       // pour STAT_BAR
    int? Max;           // pour STAT_BAR
    Guid? TargetId;     // pour RELATION
    // etc.
}
```

Ce modèle crée des états invalides non détectables : un bloc `FIELD` avec
`Current = 14` compile et s'insère en base sans erreur, mais n'a aucun sens métier.

Version correcte — hiérarchie de types scellés :

```csharp
abstract record BlockValue;
record TextBlockValue(string Content) : BlockValue;
record StatBarBlockValue(int Current, int Max) : BlockValue;
record FieldBlockValue(string Value) : BlockValue;
// etc.
```

Chaque sous-type ne contient que les champs qui lui sont pertinents.
Un `StatBarBlockValue` ne peut pas avoir un `Content`. Erreur de compilation.

### Les templates — snapshot, pas héritage dynamique

Un `DocumentTemplate` définit un schéma de blocs attendus.
Quand un MJ crée un PNJ depuis un template "D&D 5e — PNJ", le document créé
est une **copie indépendante** du schéma.

Si le template change après coup, les documents déjà créés ne changent pas.
Ce choix est intentionnel : le MJ ne doit pas voir ses fiches se modifier
à cause d'un changement de template qu'il n'a pas demandé.

---

## Les références cross-context

### Le problème

`Session` (Session Conduct) a des participants qui sont des `PlayerCharacter`
(Content Library). Comment modéliser cette relation sans coupler les deux contextes ?

### La solution : références légères par Id

`Session` ne stocke pas d'entités `PlayerCharacter`. Elle stocke une liste
de `CharacterId` — des Id typés du Shared Kernel.

```csharp
public class Session : IAggregateRoot<SessionId> {
    public IReadOnlyList<CharacterId> ParticipantIds { get; private set; }
    public IReadOnlyList<NpcId> SelectedNpcIds { get; private set; }  // auto-déduit + surcharge manuelle MJ
    // Jamais : public IReadOnlyList<PlayerCharacter> Participants
}
```

Quand la couche Application a besoin des fiches complètes, elle appelle
le repository de Content Library avec les `CharacterId`. Ce n'est pas une
jointure SQL — c'est une query cross-context explicite.

### Pourquoi pas de FK en base sur ces références

Les colonnes cross-context (ex. `SESSION_PARTICIPANT.characterId`) ne portent
pas de contrainte `FOREIGN KEY` en base pour trois raisons :

1. Les tables peuvent être dans des schémas distincts ou des bases différentes
   si on migre vers des services séparés à terme.
2. Une FK en base créerait une dépendance de déploiement entre contextes —
   impossible de faire une migration dans un contexte sans bloquer l'autre.
3. La cohérence est garantie applicativement via les domain events.

### Nettoyage des références via domain events

Si une entité référencée est soft-deleted, les références dans les autres
contextes doivent être nettoyées. Ce nettoyage est déclenché par les domain events :

```
NPC soft-deleted
    → NpcDeleted event émis
    → Handler dans Session Conduct : retire les NpcId des sessions concernées
    → Handler dans Content Library : supprime les SCENE_NPC correspondants
```

Ce mécanisme est plus explicite qu'une cascade SQL et plus maintenable
parce que chaque contexte gère son propre nettoyage.

### CLOSED vs ARCHIVED — une distinction critique

La machine d'états de `Session` est unidirectionnelle : `PLANNED → LIVE → CLOSED → ARCHIVED`.
Ces deux derniers statuts ont des sémantiques différentes que les handlers doivent respecter :

- **CLOSED** : la session est terminée mais son contenu reste **éditable**. Le MJ peut compléter
  le résumé, ajouter des LiveNotes rétroactives, corriger les participants. C'est le statut
  post-session normal.
- **ARCHIVED** : la session est en **lecture seule complète**. Plus aucune modification n'est possible.
  Ce statut est irréversible.

"Rouvrir" une session CLOSED signifie modifier son contenu — **pas changer son statut**.
Aucun handler ne doit implémenter un retour en arrière dans la machine d'états.

```csharp
// Session Conduct domain — invariant dans l'agrégat, jamais dans Application
public void Transition(SessionStatus next) {
    var allowed = (_status, next) switch {
        (PLANNED, LIVE)   => true,
        (LIVE,    CLOSED) => true,
        (CLOSED,  ARCHIVED) => true,
        _ => false
    };
    if (!allowed) throw new DomainException($"Transition {_status} → {next} interdite.");
    _status = next;
}
```

### Tableau des références cross-context du projet

| Table source | Colonne | Cible | Raison |
|---|---|---|---|
| `GUEST_ACCESS` | `characterId` | `PLAYER_CHARACTER` | Associe l'invité à son personnage |
| `CONTENT_ACCESS_RULE` | `documentId` | `DOCUMENT` | Le contenu partagé |
| `SESSION` | `scenarioId` | `SCENARIO` | Le scénario joué |
| `SESSION_PARTICIPANT` | `characterId` | `PLAYER_CHARACTER` | Les participants (PK composite) |
| `SESSION_NPC` | `npcId` | `NPC` | Les PNJ sélectionnés pour la session |
| `PINNED_ITEM` | `documentId` | `DOCUMENT` | Les docs épinglés |
| `LIVE_NOTE` | `linkedDocumentId` | `DOCUMENT` | Lien narratif optionnel |
| `NPC` | `linkedCharacterId` | `PLAYER_CHARACTER` | Association narrative |
| `PLAYER_CHARACTER` | `linkedNpcId` | `NPC` | Association narrative |

---

## Core — deux niveaux distincts

### Niveau 1 — Primitives (Shared Kernel)

C'est ce que le Core contient aujourd'hui : Id typés, AuditInfo, SoftDelete,
Email, Slug, Visibility, PinnedItem et abstractions. Pas de comportement métier.
Consommé par tous les bounded contexts.

### Niveau 2 — Moteurs métier stables

Un moteur métier est un concept qui a des règles et un comportement, mais qui est
assez générique pour servir de base à plusieurs bounded contexts.

Sur Haversack, le modèle `Document` + `DocumentBlock` est un moteur métier.
Il fournit une mécanique de contenu modulaire que `NPC`, `PlayerCharacter`,
`Scenario` et `Scene` utilisent tous. Si une future feature `WorldBuilding`
a besoin de fiches de lieux ou de factions, elle consomme le même moteur
sans rien réécrire.

```
Core
├── Primitives (niveau 1)
│   ├── Id typés, AuditInfo, SoftDelete
│   ├── Email, Slug, Tag, Visibility, PinnedItem
│   └── IAggregateRoot, IRepository...
│
└── Moteurs métier stables (niveau 2)
    └── Candidats actuels :
        - Document + DocumentBlock (dans ContentLibrary, candidat à la promotion)
        - Modèle de progression d'états (dans Session, candidat à l'extraction)
```

---

## Règle de promotion dans Core

Un concept est promu dans Core quand **les trois conditions** sont vraies :

1. **Stable** — sa définition ne change pas souvent
2. **Réutilisé** — utilisé par au moins deux bounded contexts distincts
3. **Métier réel** — exprime un concept du domaine, pas une abstraction technique

Aujourd'hui, `Document` est candidat mais ne remplit pas encore la condition 2
(utilisé uniquement par ContentLibrary). Il reste dans ContentLibrary jusqu'à ce
qu'un second contexte en ait besoin.

Ne pas promouvoir par anticipation. Un concept promu prématurément devient
une dépendance pesante pour tous les contextes.

---

## Règle d'extension — ordre de préférence

Inspirée du principe "extend before invent" appliqué à l'architecture Haversack.

Avant de créer un nouveau concept dans un bounded context, parcourir cet ordre :

**1. Étendre un concept Core existant**

Ajouter un `DocumentType.CUSTOM` avec un `customType: String` plutôt que
de créer une nouvelle table. Créer un nouveau `BlockKind` plutôt qu'un
nouveau modèle de contenu. Spécialiser un moteur existant.

**2. Créer dans le bounded context concerné**

Si le concept est trop spécifique pour être partagé, il appartient au contexte
qui en a besoin. Ne pas forcer la généralisation.

**3. Promouvoir dans Core**

Seulement quand le concept s'avère stable, réutilisé par 2+ contextes,
et exprime un concept métier réel. Voir règle de promotion ci-dessus.

Exemple appliqué sur Haversack :

```
Besoin : une future feature "Combat" a des fiches de créatures

Étape 1 — peut-on étendre l'existant ?
  → Document (type: CUSTOM, customType: "CREATURE") avec des blocs STAT_BAR
  → Oui. Pas besoin d'un nouveau modèle.

Besoin : la feature "Combat" a une progression de rounds (round 1 → 2 → fin)

Étape 1 — peut-on étendre l'existant ?
  → Session a déjà un modèle de progression PLANNED → LIVE → CLOSED
  → Similaire mais pas identique. Évaluer l'extraction.

Étape 2 — créer dans le bounded context Combat
  → RoundProgression dans Combat, inspiré du pattern Session
  → Si d'autres features ont le même besoin → candidat Core (étape 3)
```

---

## Comment les features consomment Core

Un bounded context consomme Core de deux façons :

**Consommation des primitives** — toujours, par tous les contextes.
Id typés, AuditInfo, abstractions. Aucune logique métier, juste des fondations.

**Consommation des moteurs métier** — via des références cross-context par Id.
Une feature qui a besoin de contenu flexible référence `DocumentId` du moteur
Document, sans importer les entités de ContentLibrary.

```
SessionConduct
└── LiveNote.linkedDocumentId : DocumentId
    → référence le moteur Document de ContentLibrary
    → pas d'import de Document.cs dans SessionConduct
    → la résolution se fait en couche Application

Future feature WorldBuilding
└── LocationSheet.documentId : DocumentId
    → même moteur, même pattern
    → aucun code dupliqué
```

Ce pattern garantit que les moteurs métier restent dans leur contexte d'origine
jusqu'à ce que la promotion dans Core soit justifiée.
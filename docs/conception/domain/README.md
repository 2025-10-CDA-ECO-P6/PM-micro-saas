# Modèle de domaine — Haversack

> Ce dossier contient la documentation du modèle de domaine DDD de Haversack.
> Chaque bounded context est documenté séparément avec ses règles métier, ses agrégats,
> ses événements domaine et ses diagrammes.
>
> Sources de vérité produit → [vision-produit.md](../vision/vision-produit.md)
> Use cases détaillés → [usecases/README.md](../usecases/README.md)

---

## Architecture générale

Haversack est un monolithe modulaire organisé en quatre bounded contexts plus un Core partagé.

```
Core (Shared Kernel)
  ├── Identity & Access
  ├── Space Management
  ├── Content Library
  └── Session Conduct
```

Les bounded contexts dépendent du Core. Ils ne dépendent pas les uns des autres.
Les échanges inter-contextes passent par des IDs, des événements domaine ou des contrats applicatifs.

---

## Bounded contexts

| Contexte | Responsabilité |
|---|---|
| [Core](core.md) | Abstractions DDD, IDs typés, value objects transverses |
| [Identity & Access](identity-access.md) | Comptes authentifiés, tiers, suppression RGPD |
| [Space Management](space-management.md) | Espaces de jeu (campagnes et one-shots), membres, invitations, accès invités |
| [Content Library](content-library.md) | Documents, dossiers, types de documents, références entre documents |
| [Session Conduct](session-conduct.md) | Session LIVE, tableau de bord configurable, notes de session, accès joueurs |

---

## Règles de gouvernance du Core

Avant d'ajouter un concept dans le Core, toutes les réponses doivent être **oui** :

1. Ce concept est-il utile à au moins deux bounded contexts ?
2. Son sens métier reste-t-il identique dans ces contextes ?
3. Peut-il exister sans connaître Campaign, Session, Content ou Identity ?
4. Peut-il être testé seul ?
5. Est-il stable, ou risque-t-il de changer à chaque évolution d'un contexte spécifique ?
6. **Le Core ne contient jamais d'entités.** Tout concept avec un identifiant propre et un cycle de vie appartient à un bounded context.

---

## Sens de dépendance besoin ↔ décision — pureté du domaine

Les invariants de domaine et la prose des contextes sont la couche la plus stable du modèle. Deux garde-fous la protègent :

1. **Aucun nom de technologie ou d'infrastructure** n'y figure directement — API navigateur, format de stockage, protocole, bibliothèque. Exemple interdit : citer le nom d'une interface logicielle dans la table des événements de domaine ; correct : « la suppression de compte déclenche la révocation des sessions actives ». Pour plus de détail et le test d'autoportance, voir la section « Sens de dépendance besoin ↔ décision » dans le [README parent](../README.md#sens-de-dépendance-besoin--décision).

2. **Aucun renvoi à une ADR comme source d'autorité d'une règle de domaine** — un ADR peut motiver un invariant, jamais le définir. Si masquer le renvoi ADR rend l'invariant vide, l'inversion est consommée : la substance doit vivre dans le domaine, pas dans la décision d'architecture.

---

## Diagrammes

Les diagrammes sont dans le dossier [diagrams/](diagrams/) et utilisent la syntaxe Mermaid.

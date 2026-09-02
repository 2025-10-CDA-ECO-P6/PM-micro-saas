# Structure des projets .NET et périmètre du mode local TypeScript

> Ce document est la source de vérité de la structure concrète des projets .NET (granularité, noms, responsabilités)
> et du périmètre du mode local TypeScript dans le navigateur.
> Il s'appuie sur [ADR-008](decisions/ADR-008-structure-solution.md), [ADR-001](decisions/ADR-001-execution-domaine-mode-local.md) et [ADR-003](decisions/ADR-003-stack-front.md).

---

## 1 — Objet et sources

Ce document décrit comment les limites architecturales DDD se concrétisent dans la structure physique des projets .NET
et comment le mode local TypeScript s'inscrit dans l'architecture globale.

La source de vérité des décisions structurelles est le [registre ADR](decisions/README.md) —
en particulier les trois ADR cités ci-dessus. Ce document synthétise leurs conséquences pour le développeur
qui échafaude ou navigue la solution.

---

## 2 — Principe structurant : Clean Architecture et inversion des dépendances

L'architecture suit le principe de **Clean Architecture** (Robert C. Martin) avec **inversion des dépendances** :

- Le **domaine définit les interfaces** — les contrats entre la logique métier et le monde extérieur (persistance, notifications).
- L'**infrastructure les implémente** — les repositories, les clients SignalR, les accès à la base de données.
- La **présentation dépend du domaine et de l'application**, jamais l'inverse.

Ce principe s'applique **indépendamment de la granularité physique** des projets. Même si plusieurs bounded contexts
coexistent dans un même assembly, les dépendances vont toujours domaine → application → infrastructure, jamais inverse.

*Source : [ADR-008, § Décision](decisions/ADR-008-structure-solution.md)*

---

## 3 — Granularité des projets .NET (MVP)

### Domaine et Application : projets uniques

Les quatre bounded contexts (**Identity & Access**, **Space Management**, **Content Library**, **Session Conduct**)
sont des **frontières logiques** — organisées en namespaces distincts et contrats internes clairs — pas une assembly par contexte.

```
Haversack.Domain/
  ├── IdentityAccess/       [À TRANCHER — J0]
  ├── SpaceManagement/      [À TRANCHER — J0]
  ├── ContentLibrary/       [À TRANCHER — J0]
  ├── SessionConduct/       [À TRANCHER — J0]
  └── SharedKernel/         [À TRANCHER — J0]

Haversack.Application/      [À TRANCHER — J0]
```

ADR-008 — la source citée pour cette section — décide seulement les deux projets (`Haversack.Domain`,
`Haversack.Application`) et les quatre bounded contexts comme namespaces distincts ; le nommage `SharedKernel`
est décidé par le §4 du présent document, qui clôt un report explicite d'ADR-008 (§ Conséquences). Le contenu
de chacun des cinq namespaces — fichiers, classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune
autre section du présent document ; il relève du build (`[À TRANCHER — J0]`, convention de balisage reprise de
[guide-conventions-et-dod.md, § Convention de balisage](../gestion-projet/guide-conventions-et-dod.md)).

Cette approche permet une montée en équipe sans cérémonie de configuration dès J0 (ADR-008 Contexte).
Si un contexte doit être isolé pour des raisons réelles (équipe, dépendances incompatibles, performance de build),
il peut être extrait en projet `.Domain.<ContextName>` sans rupture architecturale.

*Source : [ADR-008, § Décision](decisions/ADR-008-structure-solution.md)*

### Infrastructure et Présentation : multi-projets conservé

Les préoccupations techniques transversales restent isolées dès J0 :

```
Infrastructure.Persistence/    [À TRANCHER — J0]

Infrastructure.Notifications/  [À TRANCHER — J0] (ADR-004, § Compléments)

Presentation.Api/              [À TRANCHER — J0]

Presentation.Landing/          [À TRANCHER — J0] (Angular SSR/prerender, voir § 7)
```

ADR-008 — la source citée pour cette section — décide seulement les quatre noms de projets. Le contenu de
chacun — dossiers, fichiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document ;
il relève du build (`[À TRANCHER — J0]`).

L'isolation de `Infrastructure.Notifications` est explicitement validée pour supporter le transport SignalR temps réel (ADR-004).

*Source : [ADR-008, § Décision](decisions/ADR-008-structure-solution.md)*

---

## 4 — Nommage du noyau partagé : `SharedKernel`

Le dossier/namespace partagé s'appelle **`SharedKernel`** — convention DDD classique, déjà adoptée dans `stack.md`
(`Haversack.SharedKernel`). Ce choix clôt le report ADR-008.

Le `SharedKernel` héberge les abstractions métier communes :
- classes de base (`Entity`, `AggregateRoot`, `DomainEvent`)
- value objects transversaux (`AuditInfo`, `SoftDelete`)
- interfaces transversales (`IRepository<T>`)
- exceptions métier
- spécifications (si Query Object Pattern)

*Source : [ADR-008, § Conséquences](decisions/ADR-008-structure-solution.md)*

---

## 5 — Promotion ultérieure des bounded contexts

Un bounded context peut être extrait en projet physique séparé à tout moment si un besoin réel émerge :
- montée en équipe (deux développeurs travaillent isolément sur le même contexte)
- dépendances incompatibles (deux contextes ont besoin de versions différentes d'une librairie)
- performance de build (un contexte devient très volumineux)
- réutilisation externe (un contexte mérite son propre nuget)

**Tant qu'ils coexistent dans le même assembly**, les frontières logiques reposent sur la discipline de revue de code.
Il n'y a pas de garantie du compilateur sur le respect des dépendances entre namespaces — ce risque est mitigé
par un test d'architecture automatisé (voir § 6).

*Source : [ADR-008, § Décision et Conséquences](decisions/ADR-008-structure-solution.md)*

---

## 6 — Frontières outillées en CI : test d'architecture obligatoire

La séparation logique entre bounded contexts au sein d'un même assembly est garantie par un **test d'architecture automatisé**
exécuté en CI — typiquement avec [NetArchTest](https://www.nuget.org/packages/NetArchTest.Rules/) ou une convention de namespace
vérifiée par script.

Ce test remplace la « discipline de revue de code » (insuffisante en contexte solo) par une **contrainte outillée vérifiable**
à chaque commit. Les règles à mettre en place incluent :
- le Domaine n'importe aucune assembly d'Infrastructure ou de Présentation (il reste au centre, indépendant)
- chaque contexte respecte les frontières logiques définies par les namespaces
- l'Application et l'Infrastructure dépendent uniquement du Domaine et de ses interfaces

Ce test est un **livrable de J0** (avant la première livraison à production).

*Source : [ADR-008, § Compléments post-revue](decisions/ADR-008-structure-solution.md)*

---

## 7 — Périmètre du mode local TypeScript

### Le mode local : persistance navigateur + validations minimales

Le mode local permet l'utilisation de l'application sans compte ni connexion serveur. Les données sont persistées
dans **IndexedDB** côté navigateur.

**Point fondamental** : le mode local n'exécute **pas le domaine C# complet**. C'est une couche de persistance CRUD
avec des validations TypeScript minimales :
- titre de document non vide
- structure de blocs valide (au moins un bloc, position unique)
- aucune validation métier riche (propriétés, accès cross-context)

Le domaine C#/.NET côté serveur reste la **source de vérité unique**.

### Projection dérivée du serveur

Lorsqu'un utilisateur migre ses données locales vers le cloud, il s'agit d'une **importation**, pas d'une simple copie de confiance :
- les données sont revalidées par le domaine serveur
- les invariants métier complexes sont appliqués (un espace a exactement un OWNER, une session LIVE est unique)
- toute donnée qui ne satisfait pas les règles est rejetée ou mise en quarantaine

**Invariant clé** : `validation locale ⊆ validation serveur`. Le mode local peut accepter un état que le serveur refuserait,
mais jamais l'inverse.

### Format de sérialisation unique

L'export des données du navigateur utilise le même format JSON que le payload de migration vers le serveur.
Un seul format, deux usages :
1. Export pour sauvegarde locale / déplacement manuel
2. Payload de migration lors de la synchronisation cloud

Le format inclut un champ `schemaVersion` obligatoire ; les versions inconnues sont rejetées proprement.
Le parcours de migration est tout-ou-rien par espace, avec un gate de reconnaissance anti-appropriation
(présentation des données détectées + confirmation explicite) et un rapport de rejets.

*Sources : [ADR-001, § Décision](decisions/ADR-001-execution-domaine-mode-local.md) ; [ADR-016 — Sérialisation locale et contrat de migration local→cloud](decisions/ADR-016-serialisation-locale-migration.md) ; [ADR-017 — Modèle IndexedDB local et sécurité du mode local](decisions/ADR-017-modele-indexeddb-local.md) (object stores, posture migration-only, `navigator.storage.persist()`, sécurité F-09)*

### Frontend : Angular SPA + Angular SSR/prerender

Le frontend est construit en **Angular** — un seul écosystème, partagé par deux surfaces :
- **Application principale** : SPA interactive (Space Management, Content Library, Session Conduct)
- **Landing page** : SSR/prerender statique, déployable sur CDN pour SEO et performance au premier chargement

**Blazor WASM est écarté** — bien que le backend soit .NET/C#, partager le domaine C# dans le navigateur
aurait introduit une double implémentation métier (C# serveur, TypeScript local) — exactement la dualité
que DDD cherche à éviter. Angular+TypeScript minimal offre une surface stable et maintenable.

Conséquence : le mode local reste plus pauvre que le domaine serveur (invariants métier riches non garantis hors ligne).
C'est un choix accepté, documenté dans la vision produit et dans les use cases (UC-01).

*Source : [ADR-003, § Alternatives et Conséquences](decisions/ADR-003-stack-front.md) ; [ADR-001, § Décision](decisions/ADR-001-execution-domaine-mode-local.md)*

---

## 8 — Ordre de construction : C#-first

La solution .NET est échafaudée **en premier** — elle est la source de vérité.

L'ordre est :
1. **C# / .NET** : structure Domain/Application, entités, agrégats, interfaces du domaine
2. **Persistance** : configuration EF Core, repositories, migrations
3. **TypeScript mode local** : projection du schéma IndexedDB, validations minimales (dérivées du domaine serveur)
4. **Frontend Angular** : composants, pages, état local, connexion API

Cette séquence reflète le principe de Clean Architecture : le domaine ne dépend de rien (il existe d'abord),
l'infrastructure et le front dépendent du domaine.

Le « walking skeleton local-only » (une première expérience utilisateur hors ligne) n'est pas un jalon isolé ;
il est construit après que la structure .NET et le modèle IndexedDB soient stables.

*Source : [ADR-001, § Compléments post-revue](decisions/ADR-001-execution-domaine-mode-local.md) ; [ADR-008, § Compléments post-revue](decisions/ADR-008-structure-solution.md)*

---

## Prochaines étapes

- **Statué (ADR-008)** : nommage `SharedKernel` acté et documenté (§4) — clôt le report explicitement laissé à J0 par [ADR-008, § Conséquences](decisions/ADR-008-structure-solution.md)
- **J0** : test d'architecture en CI (NetArchTest ou convention namespace)
- **Post-MVP** : extraction de bounded contexts en projets séparés si besoin réel (équipe, dépendances)
- **Statué (ADR-016)** : migration locale → cloud — format de sérialisation, gate de reconnaissance, parcours d'échec par espace (voir [ADR-016](decisions/ADR-016-serialisation-locale-migration.md))
- **Statué (ADR-017)** : modèle IndexedDB local — object stores, versionnement du store, `navigator.storage.persist()`, sécurité mode local (bandeaux durabilité/confidentialité, import JSON, sanitisation client) (voir [ADR-017](decisions/ADR-017-modele-indexeddb-local.md))
- **J2** : configuration EF Core (voir [ADR-008, § Conséquences](decisions/ADR-008-structure-solution.md))

Voir le [registre ADR](decisions/README.md) pour le détail complet de chaque décision.

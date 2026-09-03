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

**Portée de cette section.** Sont tranchés, et ne se rouvrent pas à J0 : les principes de granularité par couche
(Domaine et Application en projets uniques, Infrastructure et Présentation en multi-projets), les namespaces de
bounded context du Domaine, le noyau partagé (§ 4), l'isolation du module de notifications temps réel, et la
Clean Architecture (§ 2). Relèvent de l'ouverture de J0 : la séparation en projets à l'intérieur de chaque couche
— combien, et lesquels —, les noms de projet, et le contenu de chacun — dossiers, fichiers, classes, sous-dossiers
(`[À TRANCHER — J0]`, convention de balisage reprise de
[guide-conventions-et-dod.md, § Convention de balisage](../gestion-projet/guide-conventions-et-dod.md)).

**Convention de désignation.** Un projet dont le nom n'est pas tranché se désigne par son rôle — « le projet
Domaine unique », « le projet Application unique » ; un namespace se désigne par son nom, tranché. Tout document
qui dérive de la présente section emploie cette convention plutôt qu'un nom de projet non arrêté.

### Domaine et Application : projets uniques

Les quatre bounded contexts (**Identity & Access**, **Space Management**, **Content Library**, **Session Conduct**)
sont des **frontières logiques** — organisées en namespaces distincts et contrats internes clairs — pas une assembly par contexte.

```
Projet Domaine unique               [À TRANCHER — J0] (nom du projet)
  ├── IdentityAccess/                [À TRANCHER — J0] (contenu)
  ├── SpaceManagement/               [À TRANCHER — J0] (contenu)
  ├── ContentLibrary/                [À TRANCHER — J0] (contenu)
  ├── SessionConduct/                [À TRANCHER — J0] (contenu)
  └── SharedKernel/                  [À TRANCHER — J0] (contenu)

Projet Application unique           [À TRANCHER — J0] (nom du projet)
```

ADR-008 — la source citée pour cette section — décide que le Domaine et l'Application sont chacun un **projet
unique** — pas les noms qui les désignent ci-dessus, qui restent une illustration — et que les quatre bounded
contexts y sont des namespaces distincts. Le nommage `SharedKernel` est décidé par le § 4 du présent document,
qui clôt un report explicite d'ADR-008 (§ Conséquences). Le contenu de chacun des cinq namespaces — fichiers,
classes, sous-dossiers — n'est fixé ni par ADR-008 ni par aucune autre section du présent document ; il relève
du build (`[À TRANCHER — J0]`).

Cette approche permet une montée en équipe sans cérémonie de configuration dès J0 (ADR-008 Contexte).
Si un contexte doit être isolé pour des raisons réelles (équipe, dépendances incompatibles, performance de build),
il peut être extrait en projet `.Domain.<ContextName>` sans rupture architecturale.

*Source : [ADR-008, § Décision](decisions/ADR-008-structure-solution.md)*

### Infrastructure et Présentation : multi-projets conservé

La granularité reste multi-projets sur l'Infrastructure et la Présentation, pour des préoccupations techniques
distinctes du découpage DDD :

```
Haversack.Infrastructure.Persistence/    [À TRANCHER — J0] (illustration ; nom et contenu)

Haversack.Infrastructure.Notifications/  (tranché — ADR-008 § Conséquences ; ADR-004, § Compléments)

Haversack.Presentation.Api/              [À TRANCHER — J0] (illustration ; nom et contenu)

Haversack.Presentation.Landing/          [À TRANCHER — J0] (illustration ; nom et contenu ; Angular SSR/prerender, voir § 7)
```

ADR-008 — la source citée pour cette section — décide cette granularité multi-projets et donne les noms ci-dessus
« Par exemple » (ADR-008 § Décision) : ils restent une illustration, pas un nom arrêté. Il isole nommément
`Haversack.Infrastructure.Notifications` dès J0 (ADR-008 § Conséquences), cohérent avec le transport SignalR
temps réel (ADR-004). Combien de projets composent chaque couche au-delà de ce module, lesquels, et le contenu
de chacun — dossiers, fichiers — ne sont fixés ni par ADR-008 ni par aucune autre section du présent document ;
ils relèvent du build (`[À TRANCHER — J0]`).

*Source : [ADR-008, § Décision](decisions/ADR-008-structure-solution.md)*

---

### Points de composition : un par agrégat

**Tranché** (décision d'entrée en build du 2026-09-03). Les points où les couches se composent — enregistrement des cas d'usage de la couche Application, table de routage, déclaration du schéma du store local — sont portés **par agrégat**, et non rassemblés dans un fichier commun.

**Pourquoi ce point est structurel.** La maille de désignation du périmètre d'écriture du [plan de travail](../gestion-projet/plan-de-travail.md) distingue les travaux par agrégat. Un fichier de composition commun serait écrit par tous et vu par aucun : deux travaux déclarés sans recouvrement s'y rencontreraient. Un point d'enregistrement par agrégat supprime ce fichier partagé, et rend vraie la garantie que la maille énonce.

*Source : cette décision clôt un point que le § 3 laissait `[À TRANCHER — J0]` sur le contenu des projets.*

---

## 4 — Nommage du noyau partagé : `SharedKernel`

Le dossier/namespace partagé s'appelle **`SharedKernel`** — convention DDD classique. Ce choix clôt le report ADR-008.

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

**Outillage tranché** (décision d'entrée en build du 2026-09-03) : **les deux mécanismes, chacun sur les règles qu'il vérifie honnêtement**. La règle 2 — les frontières entre namespaces d'un même projet — relève de **NetArchTest**, qui parcourt le graphe des types de l'assemblage compilé ; un contrôle sur le texte source raterait en silence les noms pleinement qualifiés, les `global using`, les alias et les dépendances portées par une signature. Les règles 1 et 3 — les références entre projets — relèvent d'un **contrôle du graphe des fichiers de projet**, sans ambiguïté de syntaxe et exécutable avant la compilation. *Détail et limites de chacun : [`guide-conventions-et-dod.md §6`](../gestion-projet/guide-conventions-et-dod.md).*

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

### Accès au store : un service par agrégat

**Tranché** (décision d'entrée en build du 2026-09-03). Aucun composant de la couche cliente n'accède au store local directement : tout accès passe par un **service d'accès dédié à l'agrégat concerné**, un par agrégat persisté côté client — `Space`, `Folder`, `Document`, `DocumentType`, `Session`, `SessionViewConfig` ([`plan-de-travail.md §2`](../gestion-projet/plan-de-travail.md) porte le rattachement des object stores aux agrégats).

**Deux conséquences, et la seconde est structurelle.** D'abord, la couche cliente se construit contre ce contrat sans attendre l'implémentation du domaine ([ADR-001, § Compléments post-revue (2026-09-03)](decisions/ADR-001-execution-domaine-mode-local.md)) : une doublure qui rend des jeux de données le satisfait. Ensuite, **deux travaux portant sur deux agrégats distincts n'écrivent jamais le même fichier d'accès au store** — il n'existe pas de service de façade partagé.

**Contrôle opposable** : une règle de style interdit tout import direct de l'API du store hors de ces services ([`guide-conventions-et-dod.md §3`](../gestion-projet/guide-conventions-et-dod.md)).

### Projection dérivée du serveur

Lorsqu'un utilisateur migre ses données locales vers le cloud, il s'agit d'une **importation**, pas d'une simple copie de confiance :
- les données sont revalidées par le domaine serveur
- les invariants métier complexes sont appliqués (un espace a exactement un OWNER, une session LIVE est unique)
- toute donnée qui ne satisfait pas les règles est rejetée ou mise en quarantaine

**Invariant clé** : `validation locale ⊆ validation serveur`, **au sens des règles** — aucune règle de validation côté
client que le domaine serveur ne porte pas. Le mode local peut donc accepter un état que le serveur refuserait ; le filet
est la revalidation par les value objects à l'import, avec rapport de rejets. Réciproquement, une validation côté client
qui refuse ce que le domaine serveur accepte est un **défaut à retirer du client**, non une protection supplémentaire.

*La formulation antérieure de cet invariant, incompatible avec la § Décision d'ADR-001, est requalifiée en visée dirigée
par [ADR-001, § Compléments post-revue (2026-09-03)](decisions/ADR-001-execution-domaine-mode-local.md).*

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

L'emplacement de la couche Angular et TypeScript du mode local — répertoires et découpage interne, pour la persistance locale comme pour les deux surfaces frontend décrites ci-dessus — doit être nommé à l'ouverture de `J0`. Ce nommage porte le marqueur `[À TRANCHER — J0]` et ne relève d'aucune section du présent document.

*Source : [ADR-003, § Alternatives et Conséquences](decisions/ADR-003-stack-front.md) ; [ADR-001, § Décision](decisions/ADR-001-execution-domaine-mode-local.md)*

---

## 8 — Ordre de construction : C#-first, au sens de l'autorité du contrat

La structure des projets .NET est échafaudée **en premier** : elle demeure le premier livrable de structure, avant tout
projet Angular ou TypeScript ([ADR-008, § Compléments post-revue](decisions/ADR-008-structure-solution.md), non amendé).

**C#-first désigne l'autorité du contrat, non l'ordre de construction de la couche cliente**
([ADR-001, § Compléments post-revue (2026-09-03)](decisions/ADR-001-execution-domaine-mode-local.md)). Le domaine C# est
la source de vérité des règles métier et du contrat de données ; la couche cliente se construit contre le contrat du
service d'accès au store local, sans attendre l'implémentation du domaine.

L'ordre de **dépendance** — celui que la Clean Architecture impose et que le § 2 pose — est inchangé : le domaine ne
dépend de rien, l'infrastructure et la présentation dépendent du domaine. C'est l'ordre de **construction** de la couche
cliente qui cesse d'être dérivé de cet ordre de dépendance.

Ce que cette révision ne change pas :
- la persistance EF Core cloud de J2 ne précède jamais l'interface locale de J1 ;
- une doublure de service côté client ne porte **aucune** règle métier — les seules validations autorisées sont celles
  que le § 7 énumère, et l'alternative écartée par [ADR-001, § Alternatives considérées](decisions/ADR-001-execution-domaine-mode-local.md)
  — réimplémenter le domaine en TypeScript — le reste écartée.

Cet ordre s'applique à l'intérieur d'un même jalon ; il ne s'applique jamais en travers de la séquence des jalons.
L'application de cet ordre jalon par jalon relève de la [Roadmap d'entrée en build, § 4. Ordre C#-first — interne à chaque jalon](../gestion-projet/roadmap-entree-build.md).

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

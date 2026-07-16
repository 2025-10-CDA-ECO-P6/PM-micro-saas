# Guide de conventions et Definition of Done — Haversack

| Champ | Valeur |
|---|---|
| Statut | Guide de conventions dérivé du corpus de conception, à confirmer à l'entrée en build |
| Audience | Développeur·se de build (y compris externe), à l'entrée en build (J0) |
| Sources | `docs/architecture/**` (stack, ADR, structure des projets, spécifications), `docs/cahier-strategie-test-et-recette.md`, `docs/planning/roadmap-entree-build.md` |

---

## 0. Objet, audience, autorité

### Objet

Ce guide rassemble, en un seul document, les conventions de développement et la Definition of Done (DoD) applicables à Haversack. Il n'énonce aucune règle nouvelle : chaque point impératif est une décision déjà actée en conception (ADR, document de structure, spécification), citée avec sa source précise. Là où le corpus de conception n'a rien tranché, ce guide le signale explicitement plutôt que d'inventer une valeur.

### Audience

Développeur·se prenant en charge le build de Haversack — solo ou en équipe restreinte, y compris une personne externe découvrant le projet à J0. Ce guide est le point d'entrée pour comprendre les conventions de code et les critères de complétude d'une tâche, avant de devoir naviguer l'ensemble du corpus de conception.

### Bandeau d'autorité

> **En cas de conflit entre ce guide et une source citée (ADR, document de structure, cahier de stratégie de test), la source citée fait foi.** Ce guide est un artefact d'agrégation et de renvoi, pas une nouvelle autorité de corpus — il ne redéfinit aucune décision, il les rassemble et y renvoie fidèlement.

### Convention de balisage

Deux marqueurs distinguent ce qui est tranché de ce qui ne l'est pas :

- **`[À TRANCHER — J0]`** : point non couvert par le corpus de conception. La décision appartient à l'équipe de build, sur la base du code réel — pas au présent document. L'intention est documentée quand elle est connue (par exemple : « un seuil de couverture sera fixé par l'équipe de build sur le code réel ») ; aucune valeur n'est inventée ici.
- **`[À TRANCHER — <code>]`** (par exemple `B1.2`, `P6`, `B3.2`) : dette déjà nommée explicitement par une décision ou une spécification de conception, identifiée par son code de renvoi. Ce guide reprend le balisage existant tel quel — il ne le résout pas et n'ajoute aucun code de renvoi qui ne serait pas déjà dans le corpus source.

### Note de portée

Ce guide est un document projet autonome, dérivé du corpus de conception. Il pourra servir de point de départ pour outiller les vérifications automatiques (formatage, linters) au moment de l'ouverture du dépôt de code — cette instrumentation reste hors du périmètre du présent document, qui décrit des conventions, pas une configuration d'outillage.

---

## 1. Stack et principes d'architecture

**Frontend — un seul écosystème Angular.** L'application (SPA) et la landing page (SSR/prerender, SSG au MVP) partagent le même écosystème Angular ; il n'y a pas de second framework front. Blazor WASM et Next.js ont été évalués et écartés.
*Source : [ADR-003 — Stack front, § Décision](architecture/decisions/ADR-003-stack-front.md) (l.22-24), § Compléments post-revue (l.51, landing SSG/prerender statique au MVP).*

**Backend — ASP.NET Core.** ASP.NET Core (.NET / C#), Entity Framework Core, PostgreSQL, ASP.NET Identity.
*Source : [Stack technique](architecture/stack.md) (l.7-15, tableau de synthèse).*

**Architecture — Clean Architecture + monolithe modulaire + DDD, inversion des dépendances.** Le domaine définit les interfaces (contrats vers la persistance, les notifications, etc.) ; l'infrastructure les implémente. La présentation dépend du domaine et de l'application, jamais l'inverse. Le sens des dépendances est Domaine → Application → Infrastructure, quelle que soit la granularité physique des projets — jamais l'inverse.
*Source : [06-structure-projets.md, § 2 — Principe structurant](architecture/06-structure-projets.md) (l.22-29) ; [ADR-008 — Structure physique de la solution, § Décision](architecture/decisions/ADR-008-structure-solution.md) (l.28, « Clean Architecture conservée »).*

**Quatre bounded contexts en frontières logiques.** Identity & Access, Space Management, Content Library, Session Conduct sont des frontières logiques — organisées en namespaces distincts et contrats internes clairs — pas une assembly séparée par contexte au MVP. Un bounded context peut être promu en projet physique séparé à tout moment si un besoin réel émerge (montée en équipe, dépendances incompatibles, performance de build).
*Source : [ADR-008, § Décision](architecture/decisions/ADR-008-structure-solution.md) (l.22-26) ; [06-structure-projets.md, § 3](architecture/06-structure-projets.md) (l.37-40, l.137-149).*

---

## 2. Conventions de code C#

### Dérivable du corpus

- **Id typés obligatoires.** Jamais un `Guid`/UUID nu dans une signature métier — une classe par type d'identifiant (`SpaceId`, `UserId`, etc.). Un identifiant typé rend une inversion de paramètres détectable à la compilation plutôt qu'en production.
  *Source : [01-ddd-fondations.md, § Les Id typés](architecture/01-ddd-fondations.md) (l.52-53, l.104-116).*

- **Value objects immuables, validation encapsulée dans le type.** Un value object est défini uniquement par ses valeurs, ne se modifie pas (il se remplace), et n'est jamais instanciable dans un état invalide — par exemple un `Email` invalide ne s'instancie pas.
  *Source : [01-ddd-fondations.md, § Value Object](architecture/01-ddd-fondations.md) (l.59-68).*

- **Agrégats modifiés via la racine uniquement.** On ne modifie jamais une entité enfant directement — on passe toujours par la racine de l'agrégat, qui garantit les invariants de tout le groupe (par exemple : `Space` garantit qu'il y a exactement un `OWNER` parmi ses membres).
  *Source : [01-ddd-fondations.md, § Agrégat](architecture/01-ddd-fondations.md) (l.71-81).*

- **`AuditInfo` sur toute entité.** Toute entité embarque un `AuditInfo` (qui a créé, qui a modifié, quand).
  *Source : [01-ddd-fondations.md](architecture/01-ddd-fondations.md) (l.53).*

- **Langage ubiquitaire imposé.** Les mêmes termes dans le code, la documentation et les échanges : `Space`, `Session`, `Document`, `DocumentBlock` — jamais de synonymes techniques (`Project`, `Event`, `Record`).
  *Source : [01-ddd-fondations.md, § Pourquoi DDD](architecture/01-ddd-fondations.md) (l.23-26).*

- **Organisation par namespaces par bounded context.** Le code du Domaine s'organise en un sous-namespace par bounded context (`IdentityAccess`, `SpaceManagement`, `ContentLibrary`, `SessionConduct`), plus un `SharedKernel` transversal.
  *Source : [06-structure-projets.md, § 3](architecture/06-structure-projets.md) (l.42-68).*

- **Style C# aligné DDD.** Records immuables, sealed hierarchies, pattern matching — un style cohérent avec les value objects et la modélisation du domaine.
  *Source : [Stack technique, § Backend](architecture/stack.md) (l.101, choix natif value objects/records/sealed classes ; l.106, « C# adapté au DDD : records immuables, pattern matching, sealed hierarchies »).*

### Non couvert par le corpus

- **[À TRANCHER — J0]** : règles de format et de style fines (`.editorconfig`, analyzers Roslyn activés), conventions de casse détaillées au-delà du PascalCase standard .NET. Aucune source du corpus de conception ne fixe ces points ; ils sont laissés à l'équipe de build.

---

## 3. Conventions de code TypeScript / Angular

### Dérivable du corpus

- **TypeScript strict par défaut.**
  *Source : [Stack technique, § Application web](architecture/stack.md) (l.73).*

- **Écosystème Angular unique, partagé landing + application.** Formulaires réactifs, injection de dépendances native — cohérente avec la culture .NET du projet.
  *Source : [Stack technique, § Application web](architecture/stack.md) (l.70-76) ; [ADR-003](architecture/decisions/ADR-003-stack-front.md).*

- **Mode local TypeScript = persistance CRUD + validations minimales.** Le mode local (navigateur, sans compte) n'exécute pas le domaine C# complet : c'est une couche de persistance avec des validations TypeScript minimales (par exemple : titre de document non vide, structure de blocs valide). Le domaine serveur reste la source de vérité unique. Invariant impératif : `validation locale ⊆ validation serveur` — le mode local peut accepter un état que le serveur refuserait, jamais l'inverse.
  *Source : [06-structure-projets.md, § 7 — Périmètre du mode local TypeScript](architecture/06-structure-projets.md) (l.178-194).*

### Non couvert par le corpus

- **[À TRANCHER — J0]** : configuration ESLint/Prettier, ruleset exact, versions précises d'Angular et de Node. Aucune source du corpus de conception ne fixe ces points.

---

## 4. Structure des projets

La structure concrète des projets .NET — granularité, noms de projets, responsabilités de chacun, organisation des namespaces — est intégralement décrite dans **[06-structure-projets.md](architecture/06-structure-projets.md)**, qui en est la source de vérité. Ce guide n'en reproduit pas le contenu ; il y renvoie.

Un seul point est énoncé ici en propre, parce qu'il clôt un report explicite d'ADR-008 : le noyau partagé de types et abstractions transversales (classes de base, value objects communs, interfaces transversales) est nommé **`SharedKernel`**. Ce choix est acté et le report est clos.
*Source : [06-structure-projets.md, § 4 — Nommage du noyau partagé](architecture/06-structure-projets.md) (l.121-124).*

Convention consommatrice qui en découle : tout nouveau code se place dans le namespace de son bounded context ; aucune référence ne traverse une frontière de bounded context non autorisée. Cette convention est garantie par le gate outillé en intégration continue décrit en section 6.

---

## 5. Conventions de commit et de branche

**[À TRANCHER — J0]** — Le corpus de conception ne définit aucun format de message de commit ni de convention de nommage de branche. Ce point est intégralement laissé à l'équipe de build : une convention de commit et de branche sera arrêtée à l'entrée en build ; un candidat courant est Conventional Commits, à ratifier — ce guide ne le tranche pas et n'énonce aucune règle de commit ou de branche comme décidée.

---

## 6. Gates d'intégration continue

### Dérivable du corpus

**Le test d'architecture en intégration continue est un gate obligatoire, livrable de J0.** Il vérifie les frontières de bounded context et remplace la discipline de revue de code, jugée insuffisante en contexte d'équipe restreinte, par une contrainte outillée vérifiable à chaque commit.
*Source : [06-structure-projets.md, § 6 — Frontières outillées en CI](architecture/06-structure-projets.md) (l.153-165) ; [ADR-008, § Compléments post-revue](architecture/decisions/ADR-008-structure-solution.md) (l.56).*

Les trois règles du test d'architecture, dans sa forme initiale :
1. le Domaine n'importe aucune assembly d'Infrastructure ni de Présentation — il reste au centre, indépendant ;
2. chaque contexte respecte les frontières logiques définies par les namespaces ;
3. l'Application et l'Infrastructure ne dépendent que du Domaine et de ses interfaces.

*Source : [06-structure-projets.md, § 6](architecture/06-structure-projets.md) (l.161-163).*

**Ordre C#-first, à l'intérieur de chaque incrément.** La solution .NET est échafaudée en premier. L'ordre à l'intérieur d'un même incrément est : Domaine / Application (C#) → EF Core (persistance) → TypeScript mode local → Angular. Cet ordre s'applique à l'intérieur d'un incrément, jamais en travers de la séquence macro des jalons.
*Source : [06-structure-projets.md, § 8 — Ordre de construction](architecture/06-structure-projets.md) (l.230-234) ; [Roadmap d'entrée en build, § 4](planning/roadmap-entree-build.md).*

**Renvoi** : le détail du test d'architecture dans le dispositif de test global — son objet, son périmètre, ses critères d'entrée et de sortie — est décrit dans [le cahier de stratégie de test et de recette, § 3.4](cahier-strategie-test-et-recette.md) (l.115).

### Non couvert par le corpus

- **[À TRANCHER — J0]** : l'outil exact du test d'architecture. La source présente deux options de façon alternative, sans trancher entre elles — NetArchTest ou une convention de namespace vérifiée par script. Ce guide reprend cette alternative telle quelle, sans la clore.
- **[À TRANCHER — B3.2]** : la définition exhaustive du test d'architecture (liste complète des handlers scopés/non-scopés, couverture des contrats `ITokenValidator`/`ITokenDenylist`) est une dette déjà nommée par le corpus sous ce code de renvoi. Elle n'est pas résolue ici.

---

## 7. Conventions de sécurité de code

Les points suivants sont des **critères d'acceptation non négociables**, dérivés fidèlement de la spécification de sanitisation et de politique CSP.

- **Liste blanche positive de sanitisation HTML, identique côté serveur et côté client.** Jamais de liste noire.
  *Source : [Politique de sanitisation HTML et CSP, § 1](architecture/specs/sanitisation-csp.md) (l.9-21).*

- **Interdictions absolues, sans exception, quelle que soit l'implémentation retenue :** la balise `<script>` et les attributs gestionnaires d'événements (`on*`) sont interdits et supprimés des deux côtés.
  *Source : [sanitisation-csp.md, § 1](architecture/specs/sanitisation-csp.md) (l.22-27).*

- **Ordre impératif à l'import : valider la structure, puis sanitiser le contenu — avant toute écriture ou persistance.** Un fichier importé n'est jamais écrit tel quel dans le stockage local.
  *Source : [sanitisation-csp.md, § 2](architecture/specs/sanitisation-csp.md) (l.31-41).*

- **Posture CSP actée :** `default-src 'self'` (restriction par défaut à l'origine de l'application) ; `script-src 'self'`, sans script inline ni tiers non approuvé ; `connect-src 'self'` (ou restreint à l'API), qui rend observable la règle « aucun envoi serveur en mode local ».
  *Source : [sanitisation-csp.md, § 4 — Posture CSP actée](architecture/specs/sanitisation-csp.md) (l.58-73).*

- **Le token d'accès invité n'est jamais transmis en query-string.** Cookie court-lived ou échange de token avant la négociation WebSocket.
  *Source : [Roadmap d'entrée en build, § 3.6](planning/roadmap-entree-build.md) (l.167).*

### Non couvert par le corpus

- **[À TRANCHER — B1.2 / P6]** (le corpus nomme déjà cette dette sous ces deux codes — repris tel quel, non résolu ici) : la liste exhaustive des balises HTML autorisées, la bibliothèque de sanitisation exacte, et les directives CSP complètes (liste exhaustive des directives restantes, valeurs de nonce).
  *Source : [sanitisation-csp.md, § 5](architecture/specs/sanitisation-csp.md) (l.77-86).*

---

## 8. Definition of Done

### DoD de code

Une tâche de développement est **terminée** quand, cumulativement, les 5 critères suivants sont satisfaits. Deux registres distincts s'y mêlent, distingués ici pour respecter le bandeau d'autorité de la section 0 : les items 2 et 5 sont **ancrés dans le corpus de conception**, cités avec leur source (sections 6 et 7 de ce guide) ; les items 1, 3 et 4 forment un **socle d'ingénierie assumé par ce guide lui-même**, non dérivé d'une décision de conception actée — ce guide ne prétend pas qu'ils proviennent d'une source de conception.

1. le build compile ;
2. le gate de test d'architecture en intégration continue (section 6) est vert ;
3. la revue de code est passée ;
4. la documentation impactée par la tâche est à jour ;
5. les critères de sécurité opposables de la section 7 sont satisfaits, là où la tâche y touche.

**Réconciliation avec la section 6** : l'item 3 (revue de code) désigne une pratique complémentaire et résiduelle, non substituable au gate de test d'architecture — c'est ce gate outillé, et non la revue manuelle, qui remplace la discipline de revue jugée insuffisante en contexte d'équipe restreinte (section 6).

Cette DoD de code s'applique à chaque incrément livré. Son accumulation, incrément après incrément, nourrit les critères de sortie de jalon dont le cahier de stratégie de test et la roadmap d'entrée en build sont les auteurs — elle ne s'y substitue pas.

### Renvoi — DoD de test et critères de sortie par jalon

Ce guide n'énonce pas de DoD de test ni de critères de sortie de jalon en propre : ce sont des artefacts déjà produits, dont ce guide n'est pas l'auteur.

- **DoD de test, par use case** : [cahier de stratégie de test et de recette, § 8](cahier-strategie-test-et-recette.md) (l.274).
- **Critères de sortie, par jalon de build** : [cahier de stratégie de test et de recette, § 9](cahier-strategie-test-et-recette.md) (l.278) et [Roadmap d'entrée en build, § 3](planning/roadmap-entree-build.md) (critères de sortie factuels, par jalon).

**Frontière explicite** : la DoD de code de ce guide s'applique à chaque incrément livré, quelle que soit sa taille. Les critères de sortie de jalon opèrent à une granularité supérieure — ils agrègent plusieurs incréments et des vérifications propres au jalon (recette fonctionnelle, gates humains hors intégration continue). Un incrément peut satisfaire la DoD de code de ce guide sans que le jalon auquel il appartient satisfasse encore ses propres critères de sortie.

# Guide de conventions et Definition of Done — Haversack

| Champ | Valeur |
|---|---|
| Statut | Guide de conventions dérivé du corpus de conception, à confirmer à l'entrée en build |
| Audience | Développeur·se de build (y compris externe), à l'entrée en build (J0) |
| Sources | `docs/architecture/**` (stack, ADR, structure des projets, spécifications), `docs/test/cahier-strategie-test-et-recette.md`, `docs/gestion-projet/roadmap-entree-build.md` |

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
*Source : [ADR-003 — Stack front, § Décision](../architecture/decisions/ADR-003-stack-front.md), § Compléments post-revue (l.51, landing SSG/prerender statique au MVP).*

**Backend — ASP.NET Core.** ASP.NET Core (.NET / C#), Entity Framework Core, PostgreSQL, ASP.NET Identity.
*Source : [Stack technique, § Vue d'ensemble](../architecture/stack.md) (tableau de synthèse).*

**Architecture — Clean Architecture + monolithe modulaire + DDD, inversion des dépendances.** Le domaine définit les interfaces (contrats vers la persistance, les notifications, etc.) ; l'infrastructure les implémente. La présentation dépend du domaine et de l'application, jamais l'inverse. Le sens des dépendances est Domaine → Application → Infrastructure, quelle que soit la granularité physique des projets — jamais l'inverse.
*Source : [structure-projets.md, § 2 — Principe structurant](../architecture/structure-projets.md) ; [ADR-008 — Structure physique de la solution, § Décision](../architecture/decisions/ADR-008-structure-solution.md) (l.28, « Clean Architecture conservée »).*

**Quatre bounded contexts en frontières logiques.** Identity & Access, Space Management, Content Library, Session Conduct sont des frontières logiques — organisées en namespaces distincts et contrats internes clairs — pas une assembly séparée par contexte au MVP. Un bounded context peut être promu en projet physique séparé à tout moment si un besoin réel émerge (montée en équipe, dépendances incompatibles, performance de build).
*Source : [ADR-008, § Décision](../architecture/decisions/ADR-008-structure-solution.md) ; [structure-projets.md, § Domaine et Application : projets uniques](../architecture/structure-projets.md) et [§ 5 — Promotion ultérieure des bounded contexts](../architecture/structure-projets.md).*

---

## 2. Conventions de code C#

### Dérivable du corpus

- **Id typés obligatoires.** Jamais un `Guid`/UUID nu dans une signature métier — une classe par type d'identifiant (`SpaceId`, `UserId`, etc.). Un identifiant typé rend une inversion de paramètres détectable à la compilation plutôt qu'en production.
  *Source : [ddd-fondations.md, § Entité](../architecture/ddd-fondations.md) (l.52-53, mention des Id typés) ; [§ Les Id typés — pourquoi c'est important](../architecture/ddd-fondations.md).*

- **Value objects immuables, validation encapsulée dans le type.** Un value object est défini uniquement par ses valeurs, ne se modifie pas (il se remplace), et n'est jamais instanciable dans un état invalide — par exemple un `Email` invalide ne s'instancie pas.
  *Source : [ddd-fondations.md, § Value Object](../architecture/ddd-fondations.md).*

- **Agrégats modifiés via la racine uniquement.** On ne modifie jamais une entité enfant directement — on passe toujours par la racine de l'agrégat, qui garantit les invariants de tout le groupe (par exemple : `Space` garantit qu'il y a exactement un `OWNER` parmi ses membres).
  *Source : [ddd-fondations.md, § Agrégat](../architecture/ddd-fondations.md).*

- **`AuditInfo` sur toute entité.** Toute entité embarque un `AuditInfo` (qui a créé, qui a modifié, quand).
  *Source : [ddd-fondations.md, § Entité](../architecture/ddd-fondations.md) (l.53).*

- **Langage ubiquitaire imposé.** Les mêmes termes dans le code, la documentation et les échanges : `Space`, `Session`, `Document`, `DocumentBlock` — jamais de synonymes techniques (`Project`, `Event`, `Record`).
  *Source : [ddd-fondations.md, § Pourquoi DDD](../architecture/ddd-fondations.md).*

- **Organisation par namespaces par bounded context.** Le code du Domaine s'organise en un sous-namespace par bounded context (`IdentityAccess`, `SpaceManagement`, `ContentLibrary`, `SessionConduct`), plus un `SharedKernel` transversal.
  *Source : [structure-projets.md, § Domaine et Application : projets uniques](../architecture/structure-projets.md).*

- **Style C# aligné DDD.** Records immuables, sealed hierarchies, pattern matching — un style cohérent avec les value objects et la modélisation du domaine.
  *Source : [Stack technique, § Backend › Pourquoi](../architecture/stack.md) (l.101, choix natif value objects/records/sealed classes) ; [§ Backend › Points forts](../architecture/stack.md) (l.106, « C# adapté au DDD : records immuables, pattern matching, sealed hierarchies »).*

### Non couvert par le corpus

- **Tranché** (décision d'entrée en build du 2026-09-03) : le format et le style .NET sont portés par un fichier `.editorconfig` à la racine de la solution et par les **analyzers Roslyn livrés avec le SDK**, sans dépendance d'analyse supplémentaire. Les règles y sont déclarées, et le build les applique.
- **Version du SDK** : la version en support à long terme en cours à l'ouverture du build. Ce guide énonce la règle, jamais un numéro — un numéro vieillit, la règle non (§8).
- **Reste ouvert** : les conventions de casse détaillées au-delà du PascalCase standard .NET, que la configuration ci-dessus fixe au cas par cas plutôt que ce guide.

---

## 3. Conventions de code TypeScript / Angular

### Dérivable du corpus

- **TypeScript strict par défaut.**
  *Source : [Stack technique, § Application web › Points forts](../architecture/stack.md) (l.73).*

- **Écosystème Angular unique, partagé landing + application.** Formulaires réactifs, injection de dépendances native — cohérente avec la culture .NET du projet.
  *Source : [Stack technique, § Application web › Points forts](../architecture/stack.md) ; [ADR-003](../architecture/decisions/ADR-003-stack-front.md).*

- **Mode local TypeScript = persistance CRUD + validations minimales.** Le mode local (navigateur, sans compte) n'exécute pas le domaine C# complet : c'est une couche de persistance avec des validations TypeScript minimales (par exemple : titre de document non vide, structure de blocs valide). Le domaine serveur reste la source de vérité unique. Invariant impératif : `validation locale ⊆ validation serveur` — le mode local peut accepter un état que le serveur refuserait, jamais l'inverse.
  *Source : [structure-projets.md, § 7 — Périmètre du mode local TypeScript](../architecture/structure-projets.md).*

### Non couvert par le corpus

- **Tranché** (décision d'entrée en build du 2026-09-03) : le format et le style de la couche cliente sont portés par **ESLint avec le préréglage `angular-eslint`** et par **Prettier**. Le build applique les deux.
- **Versions d'Angular et de Node** : la version en support à long terme en cours à l'ouverture du build — règle et non numéro, pour la raison donnée au §8.
- **Une règle de style opposable, tranchée avec le contrat d'accès au store** : aucun composant n'importe directement l'API du store local ; tout accès passe par le service d'accès de l'agrégat concerné ([`structure-projets.md §7`](../architecture/structure-projets.md)).

---

## 4. Structure des projets

La structure concrète des projets .NET — granularité, noms de projets, responsabilités de chacun, organisation des namespaces — est intégralement décrite dans **[structure-projets.md](../architecture/structure-projets.md)**, qui en est la source de vérité. Ce guide n'en reproduit pas le contenu ; il y renvoie.

Un seul point est énoncé ici en propre, parce qu'il clôt un report explicite d'ADR-008 : le noyau partagé de types et abstractions transversales (classes de base, value objects communs, interfaces transversales) est nommé **`SharedKernel`**. Ce choix est acté et le report est clos.
*Source : [structure-projets.md, § 4 — Nommage du noyau partagé](../architecture/structure-projets.md).*

Convention consommatrice qui en découle : tout nouveau code se place dans le namespace de son bounded context ; aucune référence ne traverse une frontière de bounded context non autorisée. Cette convention est garantie par le gate outillé en intégration continue décrit en section 6.

---

## 5. Conventions de commit et de branche

**Tranché** (décision d'entrée en build du 2026-09-03) — la proposition que le corpus signalait « à ratifier » est ratifiée.

**Message de commit — Conventional Commits, avec une portée entre parenthèses.** La forme est `type(portée): sujet`, le sujet à l'impératif présent, sans point final. La portée nomme la zone touchée. Un commit qui ne s'y conforme pas est refusé en revue.

**Nommage de branche — `<type>/<TB-nnn>-<slug>`.** Le type reprend celui du commit ; `TB-nnn` est l'identifiant de la tranche du [plan de travail](plan-de-travail.md) que la branche sert ; le slug est un rappel lisible de son intitulé.

**Pourquoi la branche porte l'identifiant de la tranche.** Il rend le lien branche ↔ tranche ↔ ticket retrouvable dans les trois sens, par simple recherche, **sans qu'aucun numéro ne soit recopié dans le plan** — le plan continue de ne porter aucun numéro de branche ni d'issue, pour la raison qu'expose [`methode-de-ticket.md §6`](methode-de-ticket.md).

---

## 6. Gates d'intégration continue

### Dérivable du corpus

**Le test d'architecture en intégration continue est un gate obligatoire, livrable de J0.** Il vérifie les frontières de bounded context et remplace la discipline de revue de code, jugée insuffisante en contexte d'équipe restreinte, par une contrainte outillée vérifiable à chaque commit.
*Source : [structure-projets.md, § 6 — Frontières outillées en CI](../architecture/structure-projets.md) ; [ADR-008, § Compléments post-revue](../architecture/decisions/ADR-008-structure-solution.md) (l.56).*

Les trois règles du test d'architecture, dans sa forme initiale :
1. le Domaine n'importe aucune assembly d'Infrastructure ni de Présentation — il reste au centre, indépendant ;
2. chaque contexte respecte les frontières logiques définies par les namespaces ;
3. l'Application et l'Infrastructure ne dépendent que du Domaine et de ses interfaces.

*Source : [structure-projets.md, § 6](../architecture/structure-projets.md) (l.130-132).*

**Ordre C#-first — autorité du contrat, non ordre de construction de la couche cliente.** La structure des projets .NET est échafaudée en premier : elle reste le premier livrable de structure. Mais le domaine C# est l'**autorité** du contrat de données et des règles métier, pas un préalable de construction pour la couche cliente : celle-ci se construit contre le contrat du service d'accès au store local, sans attendre l'implémentation du domaine. L'ordre de **dépendance** de la Clean Architecture est, lui, inchangé. Reste interdit : que la persistance EF Core cloud de J2 précède l'interface locale de J1.
*Source : [ADR-001, § Compléments post-revue (2026-09-03)](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) ; [structure-projets.md, § 8 — Ordre de construction : C#-first, au sens de l'autorité du contrat](../architecture/structure-projets.md) ; [Roadmap d'entrée en build, § 4](roadmap-entree-build.md).*

**Renvoi** : le détail du test d'architecture dans le dispositif de test global — son objet, son périmètre, ses critères d'entrée et de sortie — est décrit dans [le cahier de stratégie de test et de recette, § 3.4 — Test d'architecture (CI)](../test/cahier-strategie-test-et-recette.md).

### Non couvert par le corpus

- **Tranché** (décision d'entrée en build du 2026-09-03) : les contrôles d'intégration continue s'exécutent sur **GitHub Actions**, la plateforme du dépôt où vit déjà le code. Le déclencheur est chaque commit. Cette décision porte sur l'intégration continue seule — **l'hébergement de l'application reste non tranché** ([`docs/deploiement/README.md`](../deploiement/README.md)).
- **Tranché** (décision d'entrée en build du 2026-09-03) : l'outil du test d'architecture est **NetArchTest**. Les règles énumérées ci-dessus s'y écrivent en assertions, dans le projet de test d'architecture, et s'exécutent avec les autres tests. L'alternative que la source présentait — une convention de namespace vérifiée par script — est écartée : elle demanderait d'écrire et de maintenir soi-même l'outillage que la bibliothèque fournit.
- **[À TRANCHER — B3.2]** : la définition exhaustive du test d'architecture (liste complète des handlers scopés/non-scopés, couverture des contrats `ITokenValidator`/`ITokenDenylist`) est une dette déjà nommée par le corpus sous ce code de renvoi. Elle n'est pas résolue ici.

---

## 7. Conventions de sécurité de code

Les points suivants sont des **critères d'acceptation non négociables**, dérivés fidèlement de la spécification de sanitisation et de politique CSP.

- **Liste blanche positive de sanitisation HTML, identique côté serveur et côté client.** Jamais de liste noire.
  *Source : [Politique de sanitisation HTML et CSP, § 1](../architecture/specs/sanitisation-csp.md) (l.9-21).*

- **Interdictions absolues, sans exception, quelle que soit l'implémentation retenue :** la balise `<script>` et les attributs gestionnaires d'événements (`on*`) sont interdits et supprimés des deux côtés.
  *Source : [sanitisation-csp.md, § Interdictions absolues](../architecture/specs/sanitisation-csp.md).*

- **Ordre impératif à l'import : valider la structure, puis sanitiser le contenu — avant toute écriture ou persistance.** Un fichier importé n'est jamais écrit tel quel dans le stockage local.
  *Source : [sanitisation-csp.md, § 2 — Ordre de traitement à l'import JSON](../architecture/specs/sanitisation-csp.md).*

- **Posture CSP actée :** `default-src 'self'` (restriction par défaut à l'origine de l'application) ; `script-src 'self'`, sans script inline ni tiers non approuvé ; `connect-src 'self'` (ou restreint à l'API), qui rend observable la règle « aucun envoi serveur en mode local ».
  *Source : [sanitisation-csp.md, § 4 — Posture CSP actée](../architecture/specs/sanitisation-csp.md).*

- **Le token d'accès invité n'est jamais transmis en query-string.** Cookie court-lived ou échange de token avant la négociation WebSocket.
  *Source : [Roadmap d'entrée en build, § 3.6](roadmap-entree-build.md) (l.167).*

### Non couvert par le corpus

- **Tranché** (décision d'entrée en build du 2026-09-03) — **bibliothèque de sanitisation, côté client** : `DomSanitizer` d'Angular, déjà acté par ADR-017, complété de **DOMPurify** sur le seul chemin d'import de fichier JSON, où le contenu vient de l'extérieur.
- **Tranché** (décision d'entrée en build du 2026-09-03) — **directives de la politique de sécurité de contenu** : aux trois directives déjà actées (`default-src 'self'`, `script-src 'self'`, `connect-src 'self'`) s'ajoutent `object-src 'none'`, `base-uri 'self'`, `form-action 'self'` et `frame-ancestors 'none'`. Aucune valeur de nonce n'est employée : la posture `'self'` sans script en ligne la rend inutile.
- **Tranché** (décision d'entrée en build du 2026-09-03) — **le contenu d'un bloc n'est jamais du balisage**. Il est un **arbre de nœuds typés**, conforme à ce que le modèle de domaine écrit déjà (« contenu structuré selon le type de bloc ») et au stockage `jsonb`. L'interface le rend en construisant ses éléments, **jamais en injectant une chaîne de balisage**.
  *Source : [`conception/domain/content-library.md § DocumentBlock`](../conception/domain/content-library.md).*
- **Conséquence sur la liste blanche.** La « liste blanche positive » que le plancher exige porte donc sur les **types de nœuds admis**, énumération fermée portée par le modèle, et non sur une liste de balises à maintenir des deux côtés. Le plancher lui-même est inchangé : rien d'exécutable n'est admis, et les deux interdictions absolues restent des critères d'acceptation non négociables.
  *Source : [sanitisation-csp.md, § 5 — Points laissés ouverts](../architecture/specs/sanitisation-csp.md).*

---

## 8. Conventions de maintenance du corpus documentaire

Les points qui suivent ne relèvent pas du même registre que les sections 1 à 7 : ils ne dérivent pas d'une décision de conception déjà actée (ADR, document de structure, spécification), mais de défauts effectivement constatés dans le corpus documentaire. Cette section ne fait donc pas exception au bandeau d'autorité de la section 0 : elle en respecte l'esprit en le disant explicitement, plutôt qu'en habillant une pratique de méthode d'une fausse source de conception. Elle porte sur la maintenance du corpus `docs/conception/**` et des autres documents normatifs du dépôt, que l'équipe de build continuera d'éditer pendant le build — amender un use case, une décision d'architecture, une spécification. Elle ne concerne ni le code C#, ni le TypeScript, ni la CI.

### 8.1 Vérifier la chaîne citée au moment où l'on réécrit

Un document peut citer entre guillemets une phrase d'un autre document, exactement au moment où la citation est écrite. Puis la source est réécrite, et la citation devient une attribution à un texte qui n'existe plus. Ce défaut est invisible à tout outillage de vérification de liens ou d'ancres, et invisible à la relecture du document fautif lui-même, qui reste parfaitement cohérent avec lui-même : il faut ouvrir la source pour voir le manque. Les guillemets sont précisément la marque typographique par laquelle un lecteur s'autorise à ne pas vérifier.

**Pratique** : à chaque réécriture d'un énoncé normatif, chercher dans tout le corpus la chaîne distinctive que l'on vient de supprimer ou de modifier. C'est celui qui réécrit qui doit le faire — lui seul sait ce qu'il vient d'invalider, et il travaille sur une chaîne précise, pas sur un balayage général.

Un balayage automatique global de cette famille de défaut a été tenté et a échoué (précision de l'ordre de 20 %) : attribuer une citation entre guillemets au fichier nommé sur la même ligne est une heuristique fausse le plus souvent — une ligne nomme très souvent un fichier pour une raison sans rapport avec ce qu'elle met entre guillemets. Il est inutile de retenter cet outil sous cette forme : la garantie tient au geste de celui qui réécrit, pas à un contrôle périodique.

Note pour qui ferait une comparaison littérale automatisée : ajouter du gras à l'intérieur d'une citation la rend non littérale au sens strict — le corpus le fait par endroits, sans conséquence pratique, mais une comparaison automatisée doit normaliser l'emphase avant de comparer.

### 8.2 Renvoyer par nom de section plutôt que par numéro de ligne

Un numéro de ligne désigne une position, pas une identité : il se périme à chaque édition de sa cible, y compris une édition qui ne touche pas à la substance visée par le renvoi. La réécriture d'une seule section d'un ADR a ainsi périmé une dizaine de renvois pointant dans ce fichier. La même propriété rend le défaut difficile à vérifier automatiquement sans bruit — un vérificateur écrit pour cette famille de renvoi s'est trompé dans les deux sens et n'a pas pu établir avec certitude combien de renvois étaient réellement faux.

**Pratique** : quand la substance visée par un renvoi est une section, renvoyer par son nom plutôt que par son numéro de ligne. Un nom de section ne se périme que si la section disparaît ou change de nom. Le numéro de ligne reste légitime quand il désigne une ligne précise, à l'intérieur d'une section, dont le nom ne rendrait pas compte.

**Dérogation structurelle** : quand une section nommée contient elle-même une sous-section nommée qui est aussi la cible de renvois distincts, le nom seul de la section mère ne désambiguïse plus entre un renvoi visant son corps propre et un renvoi visant sa sous-section — les deux sont nominalement « dans » la section mère. Dans cette configuration, porter le numéro de ligne en plus du nom n'est pas une préférence de style : c'est la seule façon de distinguer les deux renvois. Par exemple, `sanitisation-csp.md, § 1` contient la sous-section nommée `§ Interdictions absolues` ; le renvoi vers le corps du § 1 (la politique de liste blanche elle-même, sans sous-titre propre) et celui vers `§ Interdictions absolues` sont tous deux nominalement « dans § 1 » — seul le numéro de ligne du premier les distingue encore.

Le corpus applique déjà ce principe à ses index : un renvoi qui pointe ne dérive pas quand sa cible bouge, un contenu recopié doit être resynchronisé à chaque changement de la source — voir le principe posé en tête de [besoin/README.md](../conception/besoin/README.md) et de [docs/README.md](../README.md) sur le rôle d'un index. Cette pratique en est l'extension à la famille de renvois qui ne l'avait pas encore reçue.

**Bilan de la résorption sur les sections 1 à 7** : les 34 renvois par numéro de ligne que portaient les sections 1 à 7 ont été rouverts un par un contre leur cible. 22 ont été convertis en renvoi par nom de section — le renvoi désignait une plage ou une section entière, dont le nom rend compte aussi bien que le numéro. 12 conservent leur numéro de ligne, chacun parce qu'il désigne un fait précis à l'intérieur d'une section qui porte plusieurs faits distincts (par exemple une clause parmi plusieurs dans une même liste, ou un paragraphe sans sous-titre propre au milieu d'une section plus large) — c'est exactement la dérogation posée ci-dessus, pas un oubli. Cette réouverture a aussi révélé une attribution de section imprécise dans un renvoi vers `ddd-fondations.md` (une citation étiquetée § Les Id typés désignait en réalité deux lignes vivant dans § Entité) ; elle a été corrigée en scindant le renvoi vers ses deux sections réelles. Aucun des 34 renvois initiaux n'est resté non examiné.

### 8.3 Chercher par revendication, pas par périmètre de lecture

Une vérification du corpus découpée par dossier, avec lecture intégrale de chaque fichier, peut manquer un défaut qui traverse les périmètres de lecture — un use case entier resté non généralisé pendant qu'une décision voisine l'était, une décision d'architecture actée qui contredit le modèle de domaine sur un point précis. Le lecteur de chaque dossier peut avoir lu son fichier en entier sans rien y voir d'anormal : une lecture intégrale par périmètre ne peut structurellement pas détecter une contradiction qui se noue entre deux périmètres.

**Pratique** : poser la question sous forme de revendication et la mesurer sur l'ensemble du corpus, du type — quels fichiers énoncent X sans jamais nommer Y. Ce recensement est rapide et exhaustif sur ses candidats, ce qu'aucune relecture ne garantit.

Deux contreparties, mesurées elles aussi :

- **Le détecteur doit être validé, pas pris au mot.** Un premier filtre qui ne retient que les fichiers ne mentionnant jamais Y laisse passer les fichiers partiellement corrigés — ceux qui mentionnent Y quelque part tout en énonçant ailleurs une règle qui reste fausse. Un défaut a ainsi survécu une itération de plus.
- **Le motif de mesure doit couvrir au moins toutes les formes du défaut.** Un renommage a été fait à moitié parce que le périmètre de recherche avait été établi sur un seul motif technique, qui ne trouvait ni les formulations en prose ni les libellés de diagrammes portant le même défaut.

Il en découle une règle simple : un défaut uniforme sur N fichiers ne se corrige pas sur un sous-ensemble — un corpus à demi corrigé est cohérent avec rien, ni avec son ancien état ni avec le nouveau.

### 8.4 Ouvrir la cible au moment d'écrire un renvoi, pas seulement au moment de le relire

Un renvoi peut être faux sans avoir jamais été juste. Ce défaut est d'une autre nature que ceux de 8.1 et 8.2, qui raisonnent tous deux sur la **dérive** : une citation ou un numéro de ligne justes à l'écriture, rendus faux par une édition ultérieure de leur cible. Ici, rien n'a bougé dans la cible — c'est le renvoi lui-même qui, dès son écriture, désignait mal sa cible. Comme la citation fabriquée de 8.1, ce défaut est invisible à la relecture du document citant : celui-ci reste parfaitement cohérent avec lui-même, il faut ouvrir la cible pour voir l'écart. Il concerne aussi bien un renvoi par numéro de ligne qu'un renvoi par nom de section — le nom peut être aussi mal attribué que le numéro.

En rouvrant un par un, dans ce guide, les 34 renvois par numéro de ligne mentionnés en 8.2 contre leur cible, deux cas sont apparus, tous deux dans ce document. Ce constat ne porte que sur ce guide — il ne dit rien du reste du corpus, dont les renvois n'ont pas été rouverts un par un.
- Un renvoi groupait deux plages de lignes vivant dans deux sections différentes de la même cible sous un seul nom de section, exact pour une seule des deux plages.
- Un renvoi bornait sa plage jusqu'à une ligne tombée déjà dans la sous-section suivante de sa cible, sans rapport avec la substance annoncée.

**Pratique** : au moment d'écrire un renvoi — pas seulement au moment de le relire — ouvrir la cible et vérifier que le nom de section (ou la ligne) annoncé est bien celui sous lequel la substance visée vit réellement, pas celui sous lequel on croit qu'elle vit. Le bornage d'une plage de lignes fait partie de cette vérification : une plage qui déborde dans la section suivante annonce une cible plus large que celle réellement visée — un renvoi juste s'arrête où s'arrête la substance qu'il désigne, pas où s'arrête la lecture de celui qui l'écrit.

---

## 9. Definition of Done

### DoD de code

Une tâche de développement est **terminée** quand, cumulativement, les 5 critères suivants sont satisfaits. Deux registres distincts s'y mêlent, distingués ici pour respecter le bandeau d'autorité de la section 0 : les items 2 et 5 sont **ancrés dans le corpus de conception**, cités avec leur source (sections 6 et 7 de ce guide) ; les items 1, 3 et 4 forment un **socle d'ingénierie assumé par ce guide lui-même**, non dérivé d'une décision de conception actée — ce guide ne prétend pas qu'ils proviennent d'une source de conception.

1. le build compile ;
2. le gate de test d'architecture en intégration continue (section 6) est vert ;
3. la revue de code est passée ;
4. la documentation impactée par la tâche est à jour ;
5. les critères de sécurité opposables de la section 7 sont satisfaits, là où la tâche y touche.

**Réconciliation avec la section 6** : l'item 3 (revue de code) désigne une pratique complémentaire et résiduelle, non substituable au gate de test d'architecture — c'est ce gate outillé, et non la revue manuelle, qui remplace la discipline de revue jugée insuffisante en contexte d'équipe restreinte (section 6).

**Réconciliation avec la section 8** : quand une tâche réécrit un énoncé normatif du corpus documentaire cité ailleurs, l'item 4 (documentation à jour) se vérifie par la pratique de la section 8.1 — chercher dans le corpus la chaîne distinctive que la réécriture vient de supprimer. Ce n'est pas un sixième critère : c'est la façon dont l'item 4 se satisfait dans ce cas précis.

Cette DoD de code s'applique à chaque incrément livré. Son accumulation, incrément après incrément, nourrit les critères de sortie de jalon dont le cahier de stratégie de test et la roadmap d'entrée en build sont les auteurs — elle ne s'y substitue pas.

### Renvoi — DoD de test et critères de sortie par jalon

Ce guide n'énonce pas de DoD de test ni de critères de sortie de jalon en propre : ce sont des artefacts déjà produits, dont ce guide n'est pas l'auteur.

- **DoD de test, par use case** : [cahier de stratégie de test et de recette, § 8](../test/cahier-strategie-test-et-recette.md) (l.274).
- **Critères de sortie, par jalon de build** : [cahier de stratégie de test et de recette, § 9 — Critères de sortie par jalon](../test/cahier-strategie-test-et-recette.md) et [Roadmap d'entrée en build, § 3](roadmap-entree-build.md) (critères de sortie factuels, par jalon).

**Frontière explicite** : la DoD de code de ce guide s'applique à chaque incrément livré, quelle que soit sa taille. Les critères de sortie de jalon opèrent à une granularité supérieure — ils agrègent plusieurs incréments et des vérifications propres au jalon (recette fonctionnelle, points de décision hors intégration continue). Un incrément peut satisfaire la DoD de code de ce guide sans que le jalon auquel il appartient satisfasse encore ses propres critères de sortie.

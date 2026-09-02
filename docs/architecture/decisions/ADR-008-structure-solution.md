# ADR-008 — Structure physique de la solution

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : G-05, B-06, B-07, C-15

> **Nature : décision pré-implémentation** — décision d'architecture actée en phase conception, à confirmer à l'entrée en build. Le raisonnement et les alternatives écartées restent la référence. *(Annotation du 2026-06-10 — arbitrage T-03, audit conception pure 2026-06.)*

---

## Contexte

La conception décrivait une structure cible avec 6 assemblies isolées dès le démarrage : un projet par bounded context, plus des projets d'infrastructure séparés. L'audit a identifié cette granularité comme sur-dimensionnée pour une phase MVP en validation d'hypothèses avec une équipe solo. La cérémonie de configuration (références inter-projets, contrats d'interface explicites dès J0, overhead de build) est disproportionnée au stade de conception actuel.

La tension à résoudre était entre la rigueur architecturale DDD (isolation des bounded contexts) et la praticité d'un démarrage solo en validation.

---

## Décision

**Domaine et Application en projets uniques.** Les 4 bounded contexts sont des frontières logiques — namespaces et contrats internes — pas une assembly par contexte. La séparation physique (un projet .NET par contexte) est différée jusqu'à ce qu'un besoin réel émerge.

**Granularité multi-projets conservée sur Infrastructure et Présentation.** Par exemple : `Infrastructure.Persistence`, `Infrastructure.Notifications` (SignalR, ADR-004), `Presentation.Api`, `Presentation.Landing` (SSR Angular, ADR-003). Ces séparations reflètent des préoccupations techniques distinctes, pas le découpage DDD.

**Promotion autorisée à tout moment.** Un bounded context peut être extrait en projet dédié dans n'importe quelle couche si un besoin réel émerge (extraction pour montée en équipe, dépendances incompatibles, performance de build).

**Clean Architecture conservée.** L'inversion de dépendances reste le principe structurant : le domaine définit les interfaces, l'infrastructure les implémente. Ce principe s'applique indépendamment de la granularité des projets.

---

## Alternatives considérées

**6 assemblies isolées dès J0 (un projet par bounded context + infrastructure + présentation).**
Écartée. Cérémonie et coût de changement élevés sur du code encore en validation d'hypothèses. Les bounded contexts partagent une seule base de données (PostgreSQL) et un seul processus : l'isolation physique n'apporte pas de bénéfice d'isolation réelle au stade MVP.

**Tout dans un seul projet (.NET monorepo plat).**
Non retenu. Perd la séparation Clean Architecture et les garanties de sens de dépendance que le compilateur peut vérifier entre couches.

---

## Conséquences

- Un fichier `structure-projets.md` est à produire comme source de vérité de la structure concrète des projets .NET (noms, responsabilités, références inter-projets). Ce fichier est un livrable de J0.
- Le nommage du projet noyau partagé (`SharedKernel` vs `Domain.Kernel`) est à trancher et à uniformiser dans toute la documentation. Ce point est délibérément laissé à J0.
- La frontière entre bounded contexts repose sur la discipline de revue de code tant qu'ils ne sont pas extraits en projets séparés. Il n'y a pas de garantie du compilateur sur le respect des frontières logiques.
- Le module `Infrastructure.Notifications` est isolé dès J0 (cohérent avec ADR-004 — SignalR).
- Une note de mapping EF Core est à produire en J2 : converters d'IDs typés, owned types, `HasColumnType("jsonb")` pour `properties`, discriminant de bloc, query filters.

---

## Compléments post-revue (2026-06-09)

Suite à une revue critique postérieure à cette décision, celle-ci est complétée comme suit, sans changer sa direction.

- **Test d'architecture en CI = livrable J0.** Un test d'architecture automatisé (ex. NetArchTest, ou vérification de convention de namespace en CI) garantissant les frontières de bounded context est un livrable J0. Il remplace la « discipline de revue de code » — mécanisme insuffisant en contexte solo — par une contrainte outillée vérifiable à chaque commit.

- **Périmètre de `structure-projets.md` étendu.** Le fichier de structure projets doit couvrir le périmètre TypeScript/front du mode local (ADR-001), pas seulement la solution .NET.

- **Ordre C#-first (ADR-001).** La solution .NET est échafaudée en premier. La structure des projets .NET est donc le premier livrable de structure, avant tout projet Angular ou TypeScript.

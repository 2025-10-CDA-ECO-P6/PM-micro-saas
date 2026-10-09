<!-- corpus:generated:begin -->
Écrit par `corpus.sh render` à partir de la sélection ci-dessous : modifier la sélection, pas cette zone.

## Ordre d'autorité

Quand deux documents divergent, celui qui est listé en premier fait foi.

1. `conventions`
2. `traceability`
3. `corpus-changelog`
4. `personas`
5. `vision`
6. `use-cases`
7. `priority-matrix`
8. `use-case-diagrams`
9. `exclusions`
10. `user-stories`
11. `user-journeys`
12. `cross-journeys`
13. `nfr`
14. `interview-guide`
15. `interview-reports`
16. `domain-contexts`
17. `shared-kernel`
18. `domain-classes`
19. `domain-flows`
20. `domain-mcd`
21. `domain-mld`
22. `event-catalogue`
23. `glossary`
24. `interface-zoning`
25. `interface-conventions`
26. `screen-sheets`
27. `wireframe-mockups`
28. `ux-rationale`
29. `content-guide`
30. `adr`
31. `stack`
32. `solution-structure`
33. `architecture-foundations`
34. `architecture-crosscut`
35. `prebuild-specs`
36. `security-dossier`
37. `compliance-frameworks`
38. `compliance-public-drafts`
39. `test-strategy`
40. `processing-register`
41. `dpia`
42. `functional-spec`
43. `technical-spec`
44. `architecture-dossier`
45. `detailed-design`
46. `business-note`
47. `project-charter`
48. `conventions-dod`
49. `build-roadmap`
50. `product-roadmap`
51. `work-plan`
52. `ticket-method`
53. `risk-register`
54. `pending-decisions`
55. `reading-guide`
56. `plan-review`
57. `release-plan`
58. `operating-model`

## Modules sélectionnés

| Module | Statut | Chemin | Porte |
|---|---|---|---|
| `conventions` | cœur | `docs/conventions.md` | corpus.rules, authority.order, identifier.registry, marker.registry, reference.convention |
| `traceability` | optionnel | `docs/tracabilite.md` | - |
| `corpus-changelog` | optionnel | `docs/journal-du-corpus.md` | - |
| `personas` | optionnel | `docs/besoin/decouverte/personas` | persona.reference |
| `vision` | cœur | `docs/besoin/vision/vision.md` | product.positioning, hypotheses, actors |
| `use-cases` | cœur | `docs/besoin/cas-d-utilisation` | need.source-of-truth, interview.questions |
| `priority-matrix` | optionnel | `docs/besoin/vision/perimetre/matrice-de-priorites.md` | priority.authority |
| `use-case-diagrams` | optionnel | `docs/besoin/cas-d-utilisation/diagrammes.md` | - |
| `exclusions` | optionnel | `docs/besoin/vision/perimetre/exclusions.md` | exclusion.register |
| `user-stories` | optionnel | `docs/besoin/stories` | acceptance.criteria, business.rules |
| `user-journeys` | optionnel | `docs/besoin/parcours-utilisateur` | journey.per-uc |
| `cross-journeys` | optionnel | `docs/besoin/parcours-utilisateur/bout-en-bout` | cross-uc.seams |
| `nfr` | optionnel | `docs/besoin/nfr` | nfr |
| `interview-guide` | optionnel | `docs/besoin/decouverte/guide-d-entretien.md` | interview.protocol |
| `interview-reports` | optionnel | par défaut | hypotheses.evidence |
| `domain-contexts` | optionnel | `docs/domaine/<contexte>` | domain.model, aggregate.list |
| `shared-kernel` | optionnel | `docs/domaine/core/kernel.md` | shared.kernel |
| `domain-classes` | optionnel | `docs/domaine/<contexte>` | - |
| `domain-flows` | optionnel | `docs/domaine/<contexte>` | - |
| `domain-mcd` | optionnel | `docs/domaine/<contexte>` | data.conceptual |
| `domain-mld` | optionnel | `docs/domaine/<contexte>` | - |
| `event-catalogue` | optionnel | `docs/domaine/evenements.md` | - |
| `glossary` | cœur | `docs/glossaire.md` | terminology.form |
| `interface-zoning` | optionnel | `docs/interface/zonage.md` | screen.inventory, interface.arbitrations, navigation.model |
| `interface-conventions` | optionnel | `docs/interface/conventions.md` | interface.notation |
| `screen-sheets` | optionnel | `docs/interface/ecrans` | screen.spec |
| `wireframe-mockups` | optionnel | `docs/interface/ecrans` | - |
| `ux-rationale` | optionnel | `docs/interface/raisonnement-ux.md` | - |
| `content-guide` | optionnel | par défaut | - |
| `adr` | cœur | `docs/decisions` | decision.record |
| `stack` | optionnel | `docs/architecture/stack.md` | - |
| `solution-structure` | optionnel | `docs/architecture/structure-projets.md` | - |
| `architecture-foundations` | optionnel | `docs/architecture/fondations.md` | - |
| `architecture-crosscut` | optionnel | `docs/architecture/transverse.md` | - |
| `prebuild-specs` | optionnel | `docs/architecture/specs` | - |
| `security-dossier` | optionnel | `docs/assurance/securite/dossier-securite.md` | shared-threat-model |
| `compliance-frameworks` | optionnel | `docs/assurance/conformite/cadres` | - |
| `compliance-public-drafts` | optionnel | `docs/assurance/conformite/public` | - |
| `test-strategy` | optionnel | `docs/assurance/strategie-de-test.md` | acceptance.cases, test.levels |
| `processing-register` | optionnel | par défaut | - |
| `dpia` | optionnel | par défaut | - |
| `functional-spec` | optionnel | `docs/reference/cahier-des-charges.md` | - |
| `technical-spec` | optionnel | `docs/reference/cahier-specifications-techniques.md` | - |
| `architecture-dossier` | optionnel | `docs/reference/dossier-architecture.md` | - |
| `detailed-design` | optionnel | `docs/reference/conception-detaillee.md` | - |
| `business-note` | optionnel | `docs/reference/note-business.md` | - |
| `project-charter` | optionnel | `docs/projet/note-de-cadrage.md` | - |
| `conventions-dod` | optionnel | `docs/projet/conventions-dod.md` | dod |
| `build-roadmap` | optionnel | `docs/projet/roadmap-de-build.md` | milestones, decision-gates |
| `product-roadmap` | optionnel | `docs/projet/roadmap-produit.md` | value.tiers |
| `work-plan` | optionnel | `docs/projet/plan-de-travail.md` | work.items |
| `ticket-method` | optionnel | `docs/projet/methode-de-ticket.md` | ticket.lifecycle, definition-of-ready |
| `risk-register` | optionnel | `docs/projet/registre-des-risques.md` | risks |
| `pending-decisions` | optionnel | `docs/projet/decisions-en-attente.md` | pending.decisions |
| `reading-guide` | optionnel | `docs/projet/guide-de-lecture.md` | - |
| `plan-review` | optionnel | `docs/projet/revues` | - |
| `release-plan` | optionnel | par défaut | - |
| `operating-model` | optionnel | par défaut | - |
<!-- corpus:generated:end -->

```corpus
kind|corpus
module|glossary|docs/glossaire.md
module|vision|docs/besoin/vision/vision.md
module|personas|docs/besoin/decouverte/personas
module|use-cases|docs/besoin/cas-d-utilisation
module|exclusions|docs/besoin/vision/perimetre/exclusions.md
module|user-stories|docs/besoin/stories
module|user-journeys|docs/besoin/parcours-utilisateur
module|cross-journeys|docs/besoin/parcours-utilisateur/bout-en-bout
module|nfr|docs/besoin/nfr
module|interview-guide|docs/besoin/decouverte/guide-d-entretien.md
module|domain-contexts|docs/domaine/<contexte>
module|domain-classes|docs/domaine/<contexte>
module|domain-flows|docs/domaine/<contexte>
module|domain-mcd|docs/domaine/<contexte>
module|domain-mld|docs/domaine/<contexte>
module|interface-zoning|docs/interface/zonage.md
module|interface-conventions|docs/interface/conventions.md
module|adr|docs/decisions
module|stack|docs/architecture/stack.md
module|security-dossier|docs/assurance/securite/dossier-securite.md
module|test-strategy|docs/assurance/strategie-de-test.md
module|functional-spec|docs/reference/cahier-des-charges.md
module|technical-spec|docs/reference/cahier-specifications-techniques.md
module|detailed-design|docs/reference/conception-detaillee.md
module|project-charter|docs/projet/note-de-cadrage.md
module|conventions-dod|docs/projet/conventions-dod.md
module|build-roadmap|docs/projet/roadmap-de-build.md
module|work-plan|docs/projet/plan-de-travail.md
module|ticket-method|docs/projet/methode-de-ticket.md
module|risk-register|docs/projet/registre-des-risques.md
module|conventions|docs/conventions.md
module|priority-matrix|docs/besoin/vision/perimetre/matrice-de-priorites.md
module|screen-sheets|docs/interface/ecrans
module|use-case-diagrams|docs/besoin/cas-d-utilisation/diagrammes.md
module|ux-rationale|docs/interface/raisonnement-ux.md
module|shared-kernel|docs/domaine/core/kernel.md
module|prebuild-specs|docs/architecture/specs
module|architecture-foundations|docs/architecture/fondations.md
module|architecture-crosscut|docs/architecture/transverse.md
module|solution-structure|docs/architecture/structure-projets.md
module|compliance-frameworks|docs/assurance/conformite/cadres
module|compliance-public-drafts|docs/assurance/conformite/public
module|architecture-dossier|docs/reference/dossier-architecture.md
module|business-note|docs/reference/note-business.md
module|product-roadmap|docs/projet/roadmap-produit.md
module|plan-review|docs/projet/revues
module|pending-decisions|docs/projet/decisions-en-attente.md
module|reading-guide|docs/projet/guide-de-lecture.md
module|event-catalogue|docs/domaine/evenements.md
module|traceability|docs/tracabilite.md
module|corpus-changelog|docs/journal-du-corpus.md
module|wireframe-mockups|docs/interface/ecrans
module|processing-register|docs/assurance/securite/registre-des-traitements.md
module|dpia|docs/assurance/securite/analyse-d-impact.md
module|interview-reports|-
module|release-plan|-
module|operating-model|-
module|content-guide|-
```

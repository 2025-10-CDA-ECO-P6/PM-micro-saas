# Cadrage — Mise en conformité du corpus

| Champ | Valeur |
|---|---|
| Chantier | `conformite-corpus` |
| Date | 2026-10-07 |
| Profondeur | 3 — complet |
| Décisions | ADR-020 (arborescence cible), ADR-021 (ordre du chantier), ADR-022 (conditions d'exécution), ADR-023 (branche dédiée et fusion finale) |
| Tâches | [TACHES.md](TACHES.md) — 24 tâches, une spec chacune |
| Branche | `chantier/conformite-corpus`, fusionnée en un seul commit à la fin (ADR-023) |
| Études | [etudes/](etudes/README.md) |
| Exécution | une tâche par session (`claude -n conformite-corpus-<tâche>`), jamais dans la session de cadrage |

## 1. Intention et utilisateurs

Le corpus de conception doit pouvoir être repris par une autre personne que son auteur, et tenu par l'outillage `/write` et `/refresh` sans exception. Aujourd'hui, il range le besoin, le domaine et l'interface sous `docs/conception/` (`docs/corpus.md:12`), les décisions sous `docs/architecture/decisions/` (`docs/corpus.md:29`), et mêle méthode, plans et registres dans `docs/gestion-projet/` (`docs/gestion-projet/README.md:1`).

L'onboarding du 2026-10-07 a cartographié le corpus sans rien déplacer, et a laissé 92 entrées d'alignement (`docs/project/backlog/README.md:1`). Ce chantier les traite et amène le corpus à la forme validée le 2026-10-05 (D11, D12), arrêtée par l'ADR-020 (`docs/architecture/decisions/ADR-020-le-corpus-est-reorganise-selon-l-arborescence-un-dossier-par.md:13`).

Utilisateurs : le porteur du projet, un repreneur éventuel, et les sessions d'assistance qui écrivent dans le corpus.

## 2. Périmètre fonctionnel

**Dans le chantier :**

- la réorganisation en un dossier par couche (F1 : déplacements, liens recalculés, outillage C# mis à jour) ;
- les identifiants portés deux fois (F4) ;
- les gabarits de chaque type de document (F2) ;
- les doublons repliés (F3) ;
- les documents générés (F5) ;
- les 92 entrées du backlog d'onboarding ;
- l'écriture des modules sélectionnés qui n'ont encore aucun document ;
- la mise en CI des quatre contrôles du corpus.

**Hors du chantier :**

- tout changement de fond qui ne vient ni d'une entrée du backlog, ni d'une décision de l'opérateur (ADR-019, `docs/architecture/decisions/ADR-019-conventions-de-tenue-du-corpus.md:21`) ;
- la renumérotation d'un identifiant ;
- le code produit ;
- les sujets du plan de travail sans lien avec la tenue du corpus.

## 3. Organisation cible

L'arborescence est celle de l'ADR-020 : `docs/besoin/`, `docs/domaine/<contexte>/`, `docs/interface/`, `docs/decisions/`, `docs/architecture/`, `docs/assurance/`, `docs/reference/` et `docs/projet/`, plus `docs/conventions.md` et `docs/glossaire.md` à la racine.

Le plan calculé par l'outil ne sert pas d'oracle (ADR-022, condition 1). Il envoie un README sur un autre, aplatit les fiches écran et ne déplace pas le domaine ([étude 01](etudes/01-revue-architecte-chemin.md)). Les déplacements suivent donc une table ancien → nouveau écrite et relue (tâche 2).

## 4. Outillage et données touchés

- **Outillage C# qui code des chemins en dur**, mis à jour dans le même commit que les déplacements : `tools/VerifMailleAgregat`, `tools/VerifCouvertureRecette`, `tools/CreerIssues`, et le workflow `.github/workflows/recalcul-suivi-projet.yml:1`.
- **Golden masters sur le corpus réel** (`tools/Tests/VerifCorpusTests.cs:36`) : après les déplacements, leur différence doit être exactement la substitution des chemins (ADR-022, condition 4).
- **Règles d'assistance** `.claude/rules/corpus-*.md` : elles citent les anciens chemins et sont régénérées.
- **Le dossier `docs/project/backlog/`** est consommé puis retiré (ADR-022, condition 5).
- **Le workflow de suivi GitHub** est suspendu pendant le chantier, puis réactivé, et les issues sont mises à jour (ADR-022, condition 6).

## 5. Revue du chemin

L'architecte a validé le chemin à trois conditions, reprises dans l'ADR-022 ([étude 01](etudes/01-revue-architecte-chemin.md)). Il propose aussi une alternative qui n'a pas été retenue : traiter le backlog d'abord, sur les chemins actuels, pour que la conformité soit entièrement prouvable sans perte. Elle est écartée par l'ADR-021, car chaque document serait repris deux fois.

## 6. Invariants de toute tâche

- Un commit par tâche sur la branche dédiée, vérifié par :
  - `bash .claude/skills/onboard/conform.sh verify <commit de départ>` ;
  - les quatre contrôles du guide de conventions (`docs/gestion-projet/guide-conventions-et-dod.md:298`).
- Le commit de départ est épinglé à la tâche 1, puis réépinglé après chaque tâche « backlog » (ADR-022, condition 3).
- Aucun identifiant n'est renuméroté ni réutilisé (ADR-019, § Identifiants).

## 7. Questions escaladées

Neuf questions ont été posées à l'opérateur pendant ce cadrage :

1. la profondeur ;
2. le `CLAUDE.md` ;
3. l'arborescence (ratification de D11/D12) ;
4. l'intégration du backlog ;
5. les modules à écrire maintenant ;
6. le sort de `docs/project/backlog` ;
7. le suivi GitHub ;
8. le choix des candidats ;
9. le choix des modules absents.

Avant de poser la question de l'arborescence, la proposition du 2026-10-05 (§ 5.10, § 9.9) a été relue : elle a été posée comme une ratification, pas comme une décision ouverte. Les conditions techniques de l'architecte n'ont pas été escaladées : elles ont été prises par l'orchestrateur et consignées dans l'ADR-022.

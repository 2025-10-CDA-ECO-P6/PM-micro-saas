# SPEC — gabarits-besoin

## Contrat

### Pourquoi

Chaque type de document a un gabarit déclaré (ADR-019, § Gabarits). Les gabarits passent avant le contenu, dans un commit séparé (ADR-022, condition 3).

### État actuel

Le plan calculé relève, sur tout le corpus, 247 sections manquantes ou mal placées, 358 champs manquants, 372 étapes Gherkin hors de la langue du projet, 92 champs d'en-tête et 36 titres (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`). Index de départ : `docs/besoin/cas-d-utilisation/README.md:1`.

### Ce qui doit être vrai à la fin

- Chaque document de la couche besoin suit le gabarit de son module : titre, en-tête, sections dans l'ordre déclaré.
- Le contenu de chaque section est conservé. Ce qui n'a pas de place dans le gabarit va dans la section optionnelle la plus proche, ou est listé dans le rapport, jamais supprimé.
- Les mots-clés Gherkin sont en français ; les titres de scénario ne changent pas.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Passer par `/write <module> --amend <instance>`, une instance à la fois.
- Un titre renommé garde son ancre joignable : les liens vers l'ancien titre sont réécrits dans le même commit.
- Ne traiter aucune entrée du backlog : elles font l'objet de la tâche suivante.

### Hors contrat

- Les entrées du backlog de la couche.
- Tout changement de fond.

### À lire avant de commencer

- ADR-019 et ADR-022
- les recettes `.claude/corpus/modules/<module>.md` des modules de la couche
- le plan calculé (lignes F2)

## Périmètre

- `docs/besoin/**`

## Revue

Le diff contre le gabarit : forme seulement, aucune perte de fond ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check` sur chaque document de la couche besoin, aucun finding `template` ne subsiste.
2. Quand on lance `conform.sh verify <commit de départ>`, il sort en code 0, et chaque ligne déclarée est un titre renommé ou un mot-clé Gherkin traduit.
3. Quand on lance VerifCorpus, on obtient 0 ancre morte.
4. Quand on lance VerifCouvertureRecette, aucune citation de titre de scénario n'est devenue orpheline.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `bash .claude/skills/write/write.sh check <module> <fichiers>` pour chaque module de la couche

## Notes de dev

- Point de départ : 872 écarts de gabarit dans la couche besoin, mesurés par le contrôle `template` global (11 min par passage). Quatre lots en parallèle dans des worktrees détachés (stories 01-05, stories 06-14, NFR, reste), puis un lot de contenu absent.
- Décisions de l'opérateur (2026-10-07), valables pour toutes les tâches de gabarits : l'élision « Étant donné qu'un… » est gardée (85 étapes que le contrôle refuse à tort, défaut de l'outillage) ; un élément requis et absent reçoit le seul marqueur `SOUS-SPÉCIFIÉ` (161 marqueurs posés) ; un qualificatif nominal/limite posé par jugement sur un texte intact est accepté ; les conversions de forme sont déclarées à verify.
- Décisions de l'orchestrateur : `Feature: X` devient `# Fonctionnalité : X` ; « Bounded contexts pressentis » devient une sous-section `###` de « Vue d'ensemble ».
- Écarts `template` restants dans la couche : les 85 étapes d'élision, et rien d'autre.
- `conform.sh verify 282e2b6 --declared docs/workstreams/conformite-corpus/declares-gabarits-besoin.txt` : code 0 (fichier cumulé depuis le commit de départ).
- Restent à reprendre sur le fond (backlog) : les « Transitions inter-UC » des UJ-04 à 06, à migrer vers les parcours bout-en-bout ; les opportunités des UJ-08 à 14, à rapprocher ligne à ligne de leurs frictions.

## Notes de revue

Revue « forme seulement, aucune perte de fond ? », 2026-10-07, sur le diff au grain du mot : dix constats.

- `fix` : `exclusions.md` UC-F07, « Pourquoi Won't Have » ajoutait une classification absente de la source. Le libellé d'origine est rétabli, et le champ du gabarit porte `SOUS-SPÉCIFIÉ`.
- `fix` : les titres raccourcis de `exclusions.md` et du guide d'entretien (§ 4) gardent leur intitulé d'origine en phrase.
- Rejeté : la prose de « Personas concernés » des UJ-08 à 14 reprend mot pour mot la parenthèse du Périmètre du même fichier. UC-12 garde « Différée — hors MVP » dans son contenu. Les ancres sont vérifiées par VerifCorpus (0 morte).
- Optionnels gardés : les en-têtes de colonnes renommés (Motivation principale, Mode de travail…), « Source de la priorité » (la section Priorité de chaque epic renvoie déjà à la matrice), le lien d'index des NFR vers leur README, la structure du § 8 du guide, `# Fonctionnalité`.

Répartition : 2 `fix`, 3 rejetés, 5 optionnels gardés.


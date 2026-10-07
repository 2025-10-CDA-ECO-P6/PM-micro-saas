# SPEC — gabarits-interface

## Contrat

### Pourquoi

Chaque type de document a un gabarit déclaré (ADR-019, § Gabarits). Les gabarits passent avant le contenu, dans un commit séparé (ADR-022, condition 3).

### État actuel

Le plan calculé relève, sur tout le corpus, 247 sections manquantes ou mal placées, 358 champs manquants, 372 étapes Gherkin hors de la langue du projet, 92 champs d'en-tête et 36 titres (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`). Index de départ : `docs/interface/README.md:1`.

### Ce qui doit être vrai à la fin

- Chaque document de la couche interface suit le gabarit de son module : titre, en-tête, sections dans l'ordre déclaré.
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

- `docs/interface/**`

## Revue

Le diff contre le gabarit : forme seulement, aucune perte de fond ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check` sur chaque document de la couche interface, aucun finding `template` ne subsiste.
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



## Notes de revue




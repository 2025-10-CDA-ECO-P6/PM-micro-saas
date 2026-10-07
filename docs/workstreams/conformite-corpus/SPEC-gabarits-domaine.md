# SPEC — gabarits-domaine

## Contrat

### Pourquoi

Chaque type de document a un gabarit déclaré (ADR-019, § Gabarits). Les gabarits passent avant le contenu, dans un commit séparé (ADR-022, condition 3).

### État actuel

Le plan calculé relève, sur tout le corpus, 247 sections manquantes ou mal placées, 358 champs manquants, 372 étapes Gherkin hors de la langue du projet, 92 champs d'en-tête et 36 titres (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`). Index de départ : `docs/domaine/README.md:1`.

### Ce qui doit être vrai à la fin

- Chaque document de la couche domaine suit le gabarit de son module : titre, en-tête, sections dans l'ordre déclaré.
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

- `docs/domaine/**`

## Revue

Le diff contre le gabarit : forme seulement, aucune perte de fond ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check` sur chaque document de la couche domaine, aucun finding `template` ne subsiste.
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

- 80 écarts de gabarit corrigés dans les 22 fichiers du domaine. Le contrôle `template` sur racine minimale sort à 0. VerifMailleAgregat donne une sortie identique ; les titres `## Agrégats` et `### Nom` sont lus comme avant.
- 20 marqueurs `SOUS-SPÉCIFIÉ` posés (méthodes de DocumentBlock, DocumentType et Invitation ; « Utilisé dans » du noyau ; descriptions de colonnes sans source). Les 5 descriptions de colonnes remplies reprennent le `context.md` du même contexte.
- Citations des titres renommés réécrites hors de la couche : `docs/reference/conception-detaillee.md:157` (§ Ce qui est dans le noyau) et `docs/projet/plan-de-travail.md:77` (`## Agrégats` dans les quatre contextes). La revue datée du 2026-09-03 n'est pas touchée.
- `conform.sh verify e99aa88 --declared docs/workstreams/conformite-corpus/declares-gabarits-domaine.txt` : code 0.
- Laissés hors liste : les 35 descriptions vides des autres tableaux des MLD (le contrôle ne lit que le premier), et la classe `SpaceUnarchived` absente du diagramme de space-management.

## Notes de revue

Revue « forme seulement, aucune perte de fond ? », 2026-10-07 : six constats.

- `fix` : « Agrégat unique » rétabli en phrase sous `## Agrégats` (identity-access).
- `fix` : `plan-de-travail.md:77` décrivait l'ancien titre ; aligné.
- Rejeté : les descriptions de colonnes des MLD reprennent mot pour mot le `context.md` du même contexte, par exemple « Unique par propriétaire » (`space-management/context.md:45`).
- Rejeté : aucun lien entrant vers les ancres renommées (VerifCorpus, 0 ancre morte).
- Optionnels gardés : colonne du noyau, stéréotypes `<<domainEvent>>` (exigés par le gabarit), fichier des lignes déclarées.

Répartition : 2 `fix`, 2 rejetés, 2 optionnels gardés.


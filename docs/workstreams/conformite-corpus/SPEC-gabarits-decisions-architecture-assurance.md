# SPEC — gabarits-decisions-architecture-assurance

## Contrat

### Pourquoi

Chaque type de document a un gabarit déclaré (ADR-019, § Gabarits). Les gabarits passent avant le contenu, dans un commit séparé (ADR-022, condition 3).

### État actuel

Le plan calculé relève, sur tout le corpus, 247 sections manquantes ou mal placées, 358 champs manquants, 372 étapes Gherkin hors de la langue du projet, 92 champs d'en-tête et 36 titres (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`). Index de départ : `docs/decisions/README.md:46`.

### Ce qui doit être vrai à la fin

- Chaque document de les couches décisions, architecture et assurance suit le gabarit de son module : titre, en-tête, sections dans l'ordre déclaré.
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

## Lots

- Lot 1 — fichiers: docs/decisions/**, docs/architecture/** — vérités: 1, 2, 3
- Lot 2 — fichiers: docs/assurance/** — vérités: 1, 2, 4

## Périmètre

- `docs/decisions/**`
- `docs/architecture/**`
- `docs/assurance/**`
- hors couche, pour la seule réécriture des citations littérales de titres renommés (contrainte : « les liens vers l'ancien titre sont réécrits dans le même commit ») : `docs/projet/conventions-dod.md`, `docs/projet/plan-de-travail.md`

## Revue

Le diff contre le gabarit : forme seulement, aucune perte de fond ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check` sur chaque document de les couches décisions, architecture et assurance, aucun finding `template` ne subsiste.
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

- Trois lots parallèles : les ADR et les documents d'architecture générale ; les specs d'architecture et les cadres de conformité ; la stratégie de test et le dossier de sécurité. 111 écarts de gabarit, dont 109 corrigés.
- `conform-verify-complet.sh d6bbafd --declared docs/workstreams/conformite-corpus/declares-gabarits-decisions-architecture-assurance.txt` : code 0.
- Natures des ADR : prises de la colonne Nature de `docs/decisions/README.md` et des bandeaux. ADR-016, 017 et 018 valent « mixte », la valeur fermée la plus proche de « mixte (dominante pré-implémentation) ». Aucun statut d'ADR ne change : le complément du statut d'ADR-018 passe dans un bandeau.
- Citations littérales des titres renommés, réécrites hors couche dans `docs/projet/conventions-dod.md` et `docs/projet/plan-de-travail.md`.
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **§9 « Critères de sortie par jalon » de la stratégie de test gardé en place** ; le contrôle le signale, car la section revient au propriétaire des jalons (`build-roadmap`). **Justification** : la roadmap de build porte déjà des critères de sortie par jalon. Déplacer le §9 fusionnerait deux définitions, ce qui relève du fond et sort du contrat, et renumeroterait les §10 à §13, cités partout. Le doublon va à la tâche 15 (doublons) ;
  - **UC-15, champ Sources** : « User stories (aucune pour UC-15 ; dérivé RB-15-01..06) », que le contrôle signale encore. **Justification** : « User stories » nu affirmerait des stories qui n'existent pas, et `SOUS-SPÉCIFIÉ` cacherait un fait connu ;
  - **sections « Traçabilité » des specs** : une phrase de renvoi vers les sources citées au fil du corps, et non `SOUS-SPÉCIFIÉ`. **Justification** : la décision 12 de l'opérateur ne vaut que pour un contenu écrit nulle part.
- Restes, différés : les sections 3 à 8 de `structure-projets.md` gardent leurs titres hors gabarit (une trentaine de citations littérales) ; l'index des ADR garde d'anciens libellés (« campagne ») ; les lignes « Source » de `structure-projets.md`, que la recette demande, ne sont pas contrôlées.

## Notes de revue

Revue « forme seulement, aucune perte de fond ? », 2026-10-07 : six constats.

- `fix` : `plan-de-travail.md:1161` citait l'ancien titre du §8 de la stratégie de test ; la citation est réécrite.
- `fix` : le bandeau d'ADR-018 portait un libellé « Nature » qui contredisait l'en-tête ; il devient « Statut d'origine : … ».
- Gardé : « **Sources** : User stories » au-dessus de « **Sources US** ». **Justification** : le contrôle exige cette valeur exacte, et renommer la ligne existante ferait revenir l'écart sur les 14 UC.
- Optionnels gardés : les regroupements « Pourquoi ce document existe » et « Ce que ce document ne fait pas » des cadres de conformité ; la numérotation mixte de `structure-projets.md` ; la description de `architecture/README.md`.
- `deferred` : `decisions-en-attente.md:167` décrit l'ancien titre du §5 du dossier de sécurité.

Répartition : 2 `fix`, 4 gardés, 1 `deferred`.

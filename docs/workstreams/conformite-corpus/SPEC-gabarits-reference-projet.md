# SPEC — gabarits-reference-projet

## Contrat

### Pourquoi

Chaque type de document a un gabarit déclaré (ADR-019, § Gabarits). Les gabarits passent avant le contenu, dans un commit séparé (ADR-022, condition 3).

### État actuel

Le plan calculé relève, sur tout le corpus, 247 sections manquantes ou mal placées, 358 champs manquants, 372 étapes Gherkin hors de la langue du projet, 92 champs d'en-tête et 36 titres (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`). Index de départ : `docs/projet/README.md:1`.

### Ce qui doit être vrai à la fin

- Chaque document de les couches référence et projet suit le gabarit de son module : titre, en-tête, sections dans l'ordre déclaré.
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

- `docs/reference/**`
- `docs/projet/**`
- `docs/README.md`
- `README.md`
- `docs/glossaire.md` : aucune tâche de gabarits ne le couvrait ; ajouté à la dernière par l'orchestrateur, sur délégation de l'opérateur (2026-10-07)

## Revue

Le diff contre le gabarit : forme seulement, aucune perte de fond ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check` sur chaque document de les couches référence et projet, aucun finding `template` ne subsiste.
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

- Trois lots parallèles : les deux cahiers et le glossaire ; le dossier d'architecture, la note business et la conception détaillée ; la couche projet. 144 écarts de gabarit au départ.
- `conform-verify-complet.sh 3037a00 --declared docs/workstreams/conformite-corpus/declares-gabarits-reference-projet.txt` : code 0. Les lignes déclarées sont des titres, des conversions de forme et des citations réécrites.
- Citations réécrites hors couche, comme l'exige la contrainte « les liens vers l'ancien titre sont réécrits dans le même commit » :
  - les ancres du cahier des charges et du guide de conventions ;
  - les § renumérotés du dossier d'architecture (transverse, mentions légales, déploiement) ;
  - les 36 « §12.4 » du cahier des charges, devenus « annexe « Valeurs de référence chiffrées » » (vision, UC-15, ADR-005, domaine, CGU, note de cadrage, roadmap, conception détaillée, note business) ;
  - l'ancien titre « Dossier d'Architecture Technique ».
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **le glossaire est ajouté à cette tâche**. **Justification** : aucune tâche de gabarits ne le couvrait, et celle-ci est la dernière ;
  - **les champs Date sans valeur écrite prennent la date du dernier commit avant le chantier** (`git log --follow -1 --format=%ad --date=short 282e2b6 -- <chemin d'origine>`). **Justification** : c'est un fait de l'historique. Une Version sans source reste `SOUS-SPÉCIFIÉ` : le contrôle refuse le marqueur dans un champ à format imposé, et l'écart est assumé ;
  - **le plan de travail garde ses niveaux de titres**, sans section « Fiches de travail ». **Justification** : `VerifCouvertureRecette` borne sa lecture sur « ## 5. ». Passer les jalons en H3 lui faisait lire jusqu'à la fin du fichier sans que rien le signale, et l'opérateur a exclu tout chantier d'outillage. L'écart reste visible (1 section absente). Il en cache un autre, que je consigne ici : les 61 fiches TB sont des tableaux, lus par `VerifMailleAgregat` et `CreerIssues`, alors que la recette attend des champs « **Libellé** : », soit 549 champs ;
  - **le registre des risques garde ses tableaux d'échelles et sa matrice** (7 colonnes signalées). **Justification** : les lever obligerait à fusionner des échelles de cotation, ce qui touche la méthode, donc le fond ;
  - **les nœuds §3.3 [DÉCISION MARCHÉ] et §3.5 [VALIDATION JURIDIQUE EU] de la roadmap ne reçoivent pas les libellés de jalon** (3 champs signalés). **Justification** : la roadmap les dit elle-même « pas un jalon de build » ;
  - **la section « Jalons » de la note de cadrage est gardée**, comme le §9 de la stratégie de test : le doublon avec la roadmap de build va à la tâche 15 ;
  - **les conventions 8.x sont rangées sous « Non couvert par le corpus »**, car le §8 dit lui-même ne pas dériver d'une décision actée.
- Restes, différés : les sous-titres « ### §N — … » du §11 du cahier des spécifications techniques reprennent les anciens noms de sections ; « Source de la priorité » est un nom sans lien ; l'entrée 0005 cite « CdC §12.1 ».

## Notes de revue

Revue « forme seulement, aucune perte de fond ? », 2026-10-07 : huit constats. Aucune perte de fond ; les chiffres de la note business sont intacts.

- `fix` : un paragraphe de la méthode de ticket, collé à `---`, devenait un titre setext.
- `fix` : « §9 — Definition of Done » et « Definition of ready (§2) » citaient d'anciens titres ; ils deviennent « Définition de fini » et « Définition de prêt ».
- `fix` (optionnel pris, risque sur l'outillage) : les niveaux de titres du plan de travail sont rétablis (voir les notes de dev).
- `fix` (optionnel pris) : les libellés de jalon posés sur les nœuds de décision de la roadmap sont retirés, et « factuels » n'est plus ajouté au §3.5.
- Gardés : le titre de la revue datée (mots d'origine) et sa clause Nature (libellé exact exigé) ; l'ordre §1, §3, §2, §4 du registre (aucune renumérotation) ; les sous-titres du §11 du cahier des spécifications (différés en bloc, pour ne pas en corriger un seul).
- `deferred` : l'entrée 0005 cite « CdC §12.1 ».

Répartition : 4 `fix`, 3 gardés, 1 `deferred`.

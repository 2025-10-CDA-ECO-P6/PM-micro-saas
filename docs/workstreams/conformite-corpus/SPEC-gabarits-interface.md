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

- Trois lots parallèles : le zonage, les conventions et le raisonnement UX ; les fiches d'écran 1 à 10 ; les fiches 11 à 20, avec le README des écrans et `gabarit-ecran.md`. Les 16 écarts du contrôle `template` sont corrigés, et le contrôle sort à 0 sur une racine minimale de la couche.
- Le module `screen-sheets` n'a pas de lecteur dans le contrôle. Les 20 fiches ont donc été comparées à la recette à la main : titre « Fiche d'écran — X », bandeau à quatre labels, en-tête de fiche en six puces, titres conditionnels exacts, avec leur déclenchement en première ligne. VerifMailleAgregat indexe toujours 20 fiches.
- `conform-verify-complet.sh dca4540 --declared docs/workstreams/conformite-corpus/declares-gabarits-interface.txt` : code 0. Les lignes déclarées sont des titres, des bandeaux, des en-têtes de fiche, des en-têtes de colonnes, plus la ligne de `reference.md` qui reçoit le hash de la tâche 8.
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **Repères S1 à S10 du zonage.** La recette numérote les titres de 1 à 10, et le contrôle n'accepte que la forme « 6. Titre ». Une phrase du préambule pose que le repère S*n* désigne la section *n*. **Justification** : le numéro ne change pas, donc aucun identifiant n'est renuméroté (ADR-019). Les quelque 400 citations « §S6 » restent justes, sans réécriture de masse risquée (S1 à S9 désignent aussi les domaines du dossier de sécurité). C'est la finalité de la numérotation selon la recette, qui veut une référence stable. Même règle pour C1 à C3 dans les conventions ;
  - **« Mode local »** est gardée telle quelle dans 11 fiches. **Justification** : `gabarit-ecran.md` la déclare comme section conditionnelle distincte (châssis sans compte), qui n'est pas un mode de l'écran. La ranger sous « Modes » changerait le fond. C'est un manque de la recette ;
  - **README des écrans** : les colonnes Surface et UC couverts sont copiées de l'inventaire S4. Pour l'espace personnel, absent de S4, elles sont reprises de l'en-tête de la fiche, et la provenance est notée.
- Restes, différés :
  - les anciens noms de fichiers cités en prose (`zoning.md`, `conventions-wireframe.md`, `reflexion-ux-mvp.md`, une centaine d'occurrences sur plusieurs couches) ;
  - les surfaces SV1 à SV5 sont déclarées dans le README des écrans, et non dans le zonage comme le veut la recette ;
  - trois marqueurs hors liste dans le zonage ;
  - des titres de niveau 3 de `raisonnement-ux.md` portent un compte ;
  - le H1 de `vue-joueur-base` diffère de son champ Nom ;
  - UC-03 est dans S4 mais pas dans la traçabilité de `vue-campagne`.

## Notes de revue

Revue « forme seulement, aucune perte de fond ? », 2026-10-07 : neuf constats, plus deux hors lentille.

- `fix` : la phrase des repères S était rangée sous « Principe directeur » ; elle passe dans l'introduction du préambule.
- `fix` : la provenance des valeurs de l'espace personnel dans le README des écrans est notée.
- `fix` : « Partage/visibilité » restait en prose dans `vue-joueur-base`.
- `fix` : le modèle de `gabarit-ecran.md` figeait « Arbitrages figés : §S6 » ; la ligne devient un emplacement à remplir.
- Gardés, car la recette impose ces libellés et cet ordre : l'en-tête « UC couverts » (`col.screen-sheets.covers`) ; Résolutions placées après la dernière réflexion ; « Question | Portée par » ; « 1. Statut » ; les qualificatifs perdus avec les titres renommés (décision 14).
- `deferred` :
  - `docs/projet/decisions-en-attente.md:146` cite le titre exact « S10 — Points ouverts sur le modèle de navigation (graphe S3) ». Le repère reste joignable, le libellé ne l'est plus, et le fichier est hors périmètre ;
  - les entrées 0018 et 0065 citent « Partage / visibilité », ce qui revient à la tâche `backlog-interface`.

Répartition : 4 `fix`, 5 gardés, 2 `deferred`.



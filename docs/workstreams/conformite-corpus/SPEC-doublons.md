# SPEC — doublons

## Contrat

### Pourquoi

Le glossaire fait référence pour les noms de termes, et un renommage est propagé partout (ADR-019, § Glossaire).

### État actuel

Le plan calculé relève 11 termes rejetés : « scenariolibrary » dans l'ADR-018 et la conception détaillée, « visible par les joueurs » dans NFR-CONF-01 et le zoning, « note privée mj » dans la stratégie de test (`docs/workstreams/conformite-corpus/plan-conformite-calcule-2026-10-07.txt:1`).

### Ce qui doit être vrai à la fin

- Chaque terme rejeté est remplacé par le terme du glossaire.
- Chaque définition présente à deux endroits n'en garde qu'un, avec `origin: <ancien chemin>:<ligne> @ <commit>`, et l'autre garde un renvoi.
- Deux versions qui diffèrent ne sont pas repliées : elles deviennent une question pour l'opérateur.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).

### Hors contrat

- Les gabarits et le backlog.

### À lire avant de commencer

- `docs/glossaire.md`
- les lignes F3 du plan calculé

## Périmètre

- `docs/**`

## Revue

Le diff : un seul propriétaire par définition, aucun fond changé ?

- general-purpose

## Vérités

1. Quand on cherche chaque terme rejeté dans le corpus, on ne le trouve plus qu'au glossaire, comme alias.
2. Quand on lance `conform.sh verify`, il sort en code 0, et chaque ligne manquante est déclarée comme repliée avec renvoi.
3. Quand on lance les quatre contrôles, ils donnent les compteurs de la référence.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.

## Notes de dev

- Termes rejetés : 12 emplois relevés par le contrôle `terms` au départ, 4 restants, assumés.
  - `ScenarioLibrary` : ADR-018, Content Library et la conception détaillée parlent de « bibliothèque personnelle ». Là où la phrase désigne l'ancien agrégat de pont, elle dit « l'agrégat `ScenarioLibrary` », et le nom reste en code comme identifiant historique. Les titres renommés et leurs citations (glossaire, UC-13, exclusions, conception détaillée) sont réécrits.
  - « note privée MJ » : CR-UC06-09 s'aligne sur sa source (US-06-04, « note de session privé MJ ») ; la donnée de test de CR-UC14-12 et d'US-UC-14 devient « Document privé MJ », et aucun titre de scénario ne change.
  - « visible par les joueurs » : **gardé** dans NFR-CONF-01 et le zonage, si bien que le contrôle `terms` signale encore 4 emplois. **Justification** (décision de l'orchestrateur, par délégation de l'opérateur) : la ligne du glossaire qui écarte ce terme dit elle-même que « les deux formes sont légitimes dans leurs couches respectives » ; la dérive visée est son emploi dans un artefact du domaine. NFR-CONF-01 relève de la couche besoin et le zonage de l'interface. Le contrôle ignore les couches : c'est un faux positif, à reporter au futur cadrage de l'outillage.
- RB-06-25 et RB-06-26 disaient la même chose. RB-06-25 garde la définition, reçoit « encodée dans `Document.CanBeReadBy()`, sans exception » et la ligne `origin` ; RB-06-26 garde son identifiant et devient un renvoi.
- Critères de sortie par jalon : la roadmap de build (§ 3) en devient propriétaire, avec une ligne `origin`. Le § 9 de la stratégie de test renvoie, jalon par jalon, aux sous-sections de la roadmap, et le § 8 de la note de cadrage renvoie à la roadmap. **Justification** (décision de l'orchestrateur, par délégation) : le § 9 se disait lui-même « reprise fidèle » de la roadmap. Ses deux dérives (« ou mixte » omis à J0, seuil SSE promu en critère de J3) étaient des erreurs de copie, et le texte de la roadmap prévaut. Le § 9 garde en propre les seules phrases que la roadmap ne porte pas (décision produit tracée, validation non convertie en dette de test).
- Migration : « par campagne » devient « par espace » dans parcours-01, parcours-02 et RB-01-11, comme UC-01 et UC-10.
- `conform-verify-complet.sh 5b5190a --declared docs/workstreams/conformite-corpus/declares-doublons.txt` : code 0.
- Les écarts de gabarit « milestone-exit » (stratégie de test) et « Jalons » (note de cadrage) restent signalés : les deux sections existent encore, comme renvois, pour garder les ancres et la numérotation.

## Notes de revue

Revue « un seul propriétaire par définition, aucun fond changé ? », 2026-10-07 : six constats.

- `fix` : trois citations de l'ancien titre « §ScenarioLibrary » d'ADR-018 (glossaire, UC-13, exclusions).
- `fix` : deux phrases d'ADR-018 attribuaient à la bibliothèque personnelle ce que disait l'ancien agrégat (« entité de pont ») ; elles disent « l'agrégat `ScenarioLibrary` ».
- `intent`, tranché par délégation : « visible par les joueurs » est rétabli en couche besoin et interface (voir les notes de dev).
- `fix` : le § 9 de la stratégie de test s'annonçait encore « reprise fidèle ».
- `fix` : le libellé de CR-UC06-09 prend le terme de sa source.
- `fix` : parcours-01 était à moitié converti, et sa citation d'UC-10 (« tout-ou-rien par campagne ») n'existait plus. `deferred` : le reste de ce constat, qui dit que le sort des sessions migrées n'est pas précisé alors qu'UC-10 le précise aujourd'hui.

Répartition : 5 `fix`, 1 `intent` tranché, 1 `deferred`.

# SPEC — write-release-plan

## Contrat

### Pourquoi

Le module est sélectionné comme attendu (`docs/corpus.md`), et l'opérateur a demandé qu'il soit écrit dans ce chantier.

### État actuel

Aucune instance n'existe. Source disponible : la roadmap d'entrée en build et le plan de travail (`docs/projet/roadmap-de-build.md:117`).

### Ce qui doit être vrai à la fin

- Chaque module a au moins une instance écrite par `/write <module>`, qui ne reprend que ce que ses sources établissent.
- L'index du module existe, et la carte du README le liste.
- Une section sans source n'est pas écrite (aucun gabarit vide).

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Aucune valeur inventée : chaque nombre, seuil ou délai est copié avec sa référence de source.

### Hors contrat

- Toute décision nouvelle, sauf réponse de l'opérateur pendant `/write`.

### À lire avant de commencer

- la recette du module sous `.claude/corpus/modules/`
- la roadmap d'entrée en build et le plan de travail

## Périmètre

- `docs/**`

## Revue

Le contrat et les sources : aucune valeur inventée ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check <module>` sur chaque nouvelle instance, il sort `CLEAN`.
2. Quand on cherche chaque nombre de l'instance, on trouve à côté sa référence de source.
3. Quand on lance VerifCorpus, on obtient 0 orphelin et 0 lien mort.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `bash .claude/skills/write/write.sh check <module> <fichier>`

## Notes de dev



## Notes de revue




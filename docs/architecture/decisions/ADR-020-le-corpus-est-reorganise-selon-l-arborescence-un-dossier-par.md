# ADR-020 — Le corpus est réorganisé selon l'arborescence « un dossier par couche » validée

- **Statut** : Accepté
- **Date** : 2026-10-07
- **Nature** : méthode

## Contexte

L'onboarding du 2026-10-07 a cartographié le corpus sans rien y déplacer. La forme cible d'un corpus a été validée le 2026-10-05 (D11, D12 de la proposition condor, § 5.10) : un dossier par couche. Le corpus actuel range le besoin, le domaine et l'interface sous docs/conception/, les ADR sous docs/architecture/, et mêle méthode, plans et registres dans docs/gestion-projet/.

## Décision

Le corpus est réorganisé selon l'arborescence « un dossier par couche » validée le 2026-10-05 (D11, D12), avec les noms de dossiers dans la langue du projet : docs/besoin/, docs/domaine/<contexte>/, docs/interface/, docs/decisions/, docs/architecture/, docs/assurance/, docs/reference/, docs/projet/, plus docs/conventions.md et docs/glossaire.md à la racine. La sélection docs/corpus.md est réécrite vers ces chemins cibles avant tout déplacement.

**Raison.** Cette forme regroupe chaque couche dans un seul dossier : les ADR quittent architecture/ car 8 des 18 ADR antérieures à l’ADR-019 sont de nature conception, le domaine est groupé par contexte, et projet/ est séparé par nature (méthode, plans, registres, revues). Elle a été validée comme forme canonique le 2026-10-05, et un corpus conforme peut être repris par l'outillage /write et /refresh sans exception.

## Alternatives considérées

- Garder l'arborescence actuelle : écarté, car la conformité se limiterait aux gabarits ; les ADR resteraient sous architecture/ alors que le besoin et le domaine les citent, et les cinq vues d'un contexte du domaine resteraient dans cinq dossiers.
- Noms de dossiers anglais du § 5.10 (need/, domain/, project/…) : écarté, car la documentation se lit en français (ADR-019, langue) et le système produit les noms dans la langue du projet.

## Conséquences

Ce qui devient vrai : chaque couche a un seul dossier, le registre des décisions est à la racine de docs/, et les cinq vues d'un contexte du domaine sont dans un même dossier.

Ce qui coûte : environ 150 déplacements, le recalcul de tous les liens relatifs, la mise à jour de l'outillage C# qui code des chemins en dur, et le réenregistrement des golden masters. Les conditions d'exécution sont fixées par l'ADR-022.

Dette : les chemins inscrits dans la sélection s'écartent des valeurs par défaut du catalogue condor (docs/pilotage/), ce qui est à signaler en amont.

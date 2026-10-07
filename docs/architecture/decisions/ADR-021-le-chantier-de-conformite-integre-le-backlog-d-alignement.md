# ADR-021 — Le chantier de conformité intègre le backlog d'alignement

- **Statut** : Accepté
- **Date** : 2026-10-07
- **Nature** : méthode

## Contexte

Le backlog d'onboarding (docs/project/backlog/, 92 entrées) et la mise en conformité touchent les mêmes documents : une correction faite avant un déplacement doit être redéplacée, et un gabarit appliqué avant une correction de contenu doit être repris.

## Décision

Le chantier de conformité intègre le backlog d'alignement : la sélection est d'abord réécrite vers les chemins cibles, puis viennent les déplacements (F1, avec relink et mise à jour de l'outillage C#) et les identifiants (F4). Ensuite, pour chaque couche, une seule tâche applique les gabarits (F2) et les entrées de docs/project/backlog/ qui portent sur ses documents. Les doublons (F3), les documents générés (F5) et la mise en CI des contrôles viennent en dernier. Chaque tâche est un commit vérifié par conform-verify et par les quatre contrôles du corpus.

**Raison.** Les déplacements d'abord : toute opération suivante s'adresse aux chemins définitifs. Une seule passe par document pour les gabarits et le backlog évite de reprendre deux fois les mêmes fichiers.

## Alternatives considérées

- Traiter le backlog après toute la conformité : écarté, car chaque document serait repris deux fois, d'abord pour son gabarit puis pour son contenu.

## Conséquences

Ce qui devient vrai : chaque document est repris une fois pour son gabarit et une fois pour son contenu, dans deux commits distincts (ADR-022), et toujours à son emplacement définitif.

Ce qui coûte : le chantier est long, avec une tâche par couche après les déplacements, et la couche besoin (45 entrées) est découpée en lots.

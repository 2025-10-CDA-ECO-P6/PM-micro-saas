# SPEC — fusion-finale

## Contrat

### Pourquoi

Le chantier s'exécute sur une branche dédiée, avec un commit par tâche, puis il est fusionné en un seul commit sur la branche de travail (ADR-023).

### État actuel

Les tâches 1 à 23 ont été commitées une à une sur `chantier/conformite-corpus`. La liste des tâches est dans `docs/workstreams/conformite-corpus/TACHES.md:1`, et la référence de départ dans `docs/workstreams/conformite-corpus/reference.md`.

### Ce qui doit être vrai à la fin

- La branche de travail porte un seul commit de plus, dont l'arbre est identique à celui de la dernière tâche de `chantier/conformite-corpus`.
- Le message de ce commit liste les 23 tâches, avec, pour chacune, le hash de son commit sur la branche dédiée et le résultat de ses contrôles.
- La branche `chantier/conformite-corpus` est conservée jusqu'à la relecture par l'opérateur.

### Contraintes et pièges

- Ne supprimer la branche dédiée qu'avec l'accord de l'opérateur.
- Vérifier, avant de fusionner, que la branche de travail n'a pas avancé ; si elle a avancé, s'arrêter et demander.
- Pas de trailer Co-Authored-By.

### Hors contrat

- Toute modification de contenu : la fusion ne change rien à l'arbre.

### À lire avant de commencer

- `docs/architecture/decisions/ADR-023-le-chantier-conformite-corpus-s-execute-sur-une-branche-dedi.md`
- `docs/workstreams/conformite-corpus/TACHES.md`

## Lots

- Lot 1 — fichiers: docs/**, tools/** — vérités: 1, 2, 3
- Lot 2 — fichiers: .github/workflows/**, README.md — vérités: 1

## Périmètre

- `docs/**`
- `README.md`
- `tools/**`
- `.github/workflows/**`

## Revue

Le contrat : l'arbre fusionné est-il exactement celui de la branche dédiée ?

## Vérités

1. Quand on compare l'arbre du commit fusionné à celui du dernier commit de la branche dédiée (`git diff <fusion> <dernier>`), le diff est vide.
2. Quand on lit `git log` de la branche de travail, le chantier n'y occupe qu'un commit depuis la référence de départ.
3. Quand on lit le message du commit fusionné, chacune des 23 tâches y figure avec son hash.

## Vérification

- `git diff <fusion> chantier/conformite-corpus` : vide
- les quatre contrôles du corpus, sur le commit fusionné

## Notes de dev



## Notes de revue




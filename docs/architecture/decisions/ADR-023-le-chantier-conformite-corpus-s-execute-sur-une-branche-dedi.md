# ADR-023 — Le chantier conformite-corpus s'exécute sur une branche dédiée,

- **Statut** : Accepté
- **Date** : 2026-10-07
- **Nature** : méthode

## Contexte

Le chantier compte 24 tâches, chacune vérifiée par conform-verify contre un commit de départ, réépinglé après chaque tâche « backlog » (ADR-022). L'opérateur veut un seul commit dans l'historique pour tout le chantier.

## Décision

Le chantier conformite-corpus s'exécute sur une branche dédiée, chantier/conformite-corpus, avec un commit par tâche. Ces commits servent de références à conform-verify (ADR-022). À la fin, la branche est fusionnée en un seul commit (squash) sur la branche de travail.

**Raison.** L'opérateur ne veut pas d'une multiplication de commits dans l'historique. Un commit par tâche reste nécessaire pour que conform-verify distingue une perte d'une correction voulue (ADR-022, condition 3).

## Alternatives considérées

- Un seul commit à la fin, sans commit intermédiaire : écarté, car conform-verify n'aurait plus de commit de départ entre les tâches, et le travail non commité risque d'être perdu entre deux sessions.
- Un commit par tâche directement sur la branche de travail : écarté par l'opérateur, pour garder un historique lisible.

## Conséquences

Ce qui devient vrai : l'historique de la branche de travail ne reçoit qu'un commit pour tout le chantier, et chaque étape a été vérifiée sur la branche dédiée. Ce qui coûte : le détail par tâche ne survit pas au squash. Le message du commit final liste les tâches et leurs vérifications, et la branche dédiée est conservée jusqu'à la relecture.

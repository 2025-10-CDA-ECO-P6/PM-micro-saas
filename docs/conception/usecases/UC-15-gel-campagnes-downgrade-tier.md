# UC-15 — Gel de campagnes au downgrade de tier

> **Should Have — hors première livraison (post-MVP)** — Ce cas d'utilisation couvre le gel automatique des espaces excédentaires lors d'un downgrade de tier, ainsi que le dégel réversible au ré-upgrade. En MVP, le gel n'est pas déclenché (le downgrade suppose un tier payant, hors périmètre première livraison).

> **Note de positionnement d'autorité** — Ce use case est l'**amont source-de-vérité** du comportement de gel/dégel ; le domaine ([`space-management.md`](../domain/space-management.md) — règles 6 et 11, méthodes `Freeze()`/`Unfreeze()`) en est l'aval documenté, conformément à l'ordre d'autorité du corpus (use case → domaine). Il ratifie et détaille ce comportement plutôt que de le réinventer. Ce UC est issu de la question ouverte posée dans [UC-HORS-MVP §Gel de campagnes au downgrade de tier](UC-HORS-MVP.md).

## Acteur principal

Le système — réaction automatique à un événement de facturation (`AccountTierChanged`). Aucune action manuelle du MJ ne déclenche le gel ou le dégel.

## Acteurs secondaires

Aucun. Le MJ propriétaire de l'espace est l'acteur **concerné** (il subit le gel ou bénéficie du dégel, et en est notifié) mais n'est l'acteur déclencheur d'aucune étape de ce use case — le déclencheur est un événement de facturation, pas une action du MJ dans l'application.

## Objectif

Définir ce qu'il advient des espaces d'un MJ lorsqu'il repasse au tier gratuit en étant au-delà du quota d'espaces autorisé, et la situation inverse lorsqu'il ré-upgrade vers un tier payant.

## Contexte

Un MJ perd son tier premium — fin d'abonnement ou non-renouvellement — et se retrouve au-delà de la limite d'espaces `CAMPAIGN`/`ONE_SHOT` du tier gratuit (quota consolidé, valeur volatile — voir `vision-produit.md §3` / `cahier-des-charges.md §12.4`), qui n'est pas figée par ce use case.

Ce scénario existe car le modèle de monétisation (voir [ADR-005](../../architecture/decisions/ADR-005-modele-monetisation.md), trace historique fusionnée dans la vision) autorise un MJ à créer des espaces au-delà du quota gratuit tant qu'il reste sur un tier payant ; le retour au tier gratuit peut donc laisser un MJ dans un état où son nombre d'espaces actifs dépasse ce que le tier gratuit autorise.

## Déclencheur

L'événement `AccountTierChanged`, émis par Identity & Access et reçu par Space Management :
- **Sens downgrade** (tier premium → gratuit) : déclenche le scénario de gel.
- **Sens montée de tier** (p. ex. gratuit → premium) : déclenche le scénario de dégel.

## Préconditions

- Le MJ possède un compte cloud — le gel ne concerne pas le mode local (le mode local n'a pas de notion de tier facturé).
- Pour le scénario de gel : le MJ possède, au moment de l'événement, un nombre d'espaces `CAMPAIGN`/`ONE_SHOT` à l'état `ACTIVE` strictement supérieur au quota du tier gratuit.
- Pour le scénario de dégel : le MJ possède au moins un espace à l'état `FROZEN`.

## Scénario nominal — Gel au downgrade

*(ratifie la règle 6 de `space-management.md`)*

1. Le système reçoit `AccountTierChanged` pour le compte du MJ, dans le sens downgrade (tier premium → gratuit).
2. Il compte les espaces `CAMPAIGN`/`ONE_SHOT` à l'état `ACTIVE` dont le MJ est propriétaire. L'espace `PERSONAL` du MJ n'est **jamais compté** dans ce total : il est structurellement hors quota (invariant 15 de `space-management.md` — un espace `PERSONAL` est toujours `ACTIVE`, jamais gelé).
3. Si ce compte dépasse le quota du tier gratuit, le système identifie les espaces **excédentaires** — ceux qui dépassent le quota — dans l'ordre de création, **les plus récents en premier**.
4. Le système gèle chacun de ces espaces excédentaires via `Space.Freeze()`, qui produit l'événement `SpaceFrozen`, jusqu'à ce que le nombre d'espaces `ACTIVE` du propriétaire revienne dans la limite du quota.
5. Un espace passé à l'état `FROZEN` devient en **lecture seule** (invariant 7 de `space-management.md`) : son contenu reste consultable normalement, mais aucune écriture n'y est plus possible. Aucune donnée n'est supprimée à cette étape.
6. Le MJ est notifié du gel (l'Application consomme `SpaceFrozen` pour émettre la notification).

## Scénario de réversibilité — Dégel au ré-upgrade

*(comble la lacune laissée ouverte dans UC-HORS-MVP, ratifie la nouvelle règle 11 de `space-management.md`)*

1. Le système reçoit `AccountTierChanged` pour le compte du MJ, dans un sens de montée de tier.
2. Il identifie les espaces à l'état `FROZEN` dont le MJ est propriétaire.
3. Il dégèle automatiquement ces espaces via `Space.Unfreeze()`, qui produit l'événement `SpaceUnfrozen`, **dans la limite du quota d'espaces du tier cible**.
4. Au MVP, le modèle de tier est binaire (le tier premium/`PRO` est illimité) : tous les espaces `FROZEN` du propriétaire sont donc dégelés dans ce scénario. La garde de `Unfreeze()` est néanmoins exprimée **relativement au quota du tier cible**, et non de façon inconditionnelle, afin de rester correcte si un tier intermédiaire (quota borné mais non nul) est introduit ultérieurement. L'ordre de dégel lorsque le quota du tier cible est borné et inférieur au nombre d'espaces `FROZEN` (futur tier intermédiaire) n'est pas fixé au MVP (tier binaire ⇒ dégel total) ; il sera arrêté à l'introduction éventuelle d'un tier intermédiaire.
5. Le dégel ne perd aucune donnée — le gel était un état de lecture seule, jamais une suppression ; le contenu et la structure de l'espace dégelé sont identiques à ce qu'ils étaient avant le gel.
6. Le MJ est notifié du dégel (l'Application consomme `SpaceUnfrozen` pour émettre la notification).

## Règles métier

- **RB-15-01** — Lors d'un downgrade laissant le propriétaire au-delà du quota du tier gratuit, les espaces excédentaires `CAMPAIGN`/`ONE_SHOT` sont gelés dans l'ordre de création, les plus récents en premier, jusqu'à ce que le nombre d'espaces `ACTIVE` revienne dans le quota.
- **RB-15-02** — L'espace `PERSONAL` du propriétaire n'est jamais compté dans le quota et n'est jamais gelé, quel que soit l'état du tier (invariant 15 de `space-management.md`).
- **RB-15-03** — Un espace `FROZEN` est en lecture seule : son contenu reste consultable, aucune écriture n'y est possible, et aucune donnée n'y est supprimée du seul fait du gel (invariant 7 de `space-management.md`).
- **RB-15-04** — Lors d'une montée de tier, les espaces `FROZEN` du propriétaire sont dégelés automatiquement, dans la limite du quota du tier cible — la garde est relative au quota cible, pas inconditionnelle, pour rester correcte si un tier intermédiaire apparaît.
- **RB-15-05** — Le dégel ne perd aucune donnée : le gel et le dégel sont un cycle réversible sans suppression, dans les deux sens.
- **RB-15-06** — Le propriétaire est notifié à chaque gel (`SpaceFrozen`) et à chaque dégel (`SpaceUnfrozen`) le concernant.

## Renvois

- [`space-management.md`](../domain/space-management.md) — `SpaceStatus.FROZEN`, méthodes `Freeze()`/`Unfreeze()`, invariants 7 et 15, règles métier 6 et 11.
- [Glossaire — `SpaceStatus`](../glossaire.md) — définition de la valeur `FROZEN`.
- [ADR-005 — Modèle de monétisation](../../architecture/decisions/ADR-005-modele-monetisation.md) — trace historique du modèle de tiers (substance fusionnée dans la vision).
- [UC-HORS-MVP — Gel de campagnes au downgrade de tier](UC-HORS-MVP.md) — question ouverte dont ce UC est issu.
- `vision-produit.md §3` / `cahier-des-charges.md §12.4` — valeurs chiffrées du quota et des tiers (volatiles, non figées par ce use case).

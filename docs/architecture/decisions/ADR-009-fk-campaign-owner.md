# ADR-009 — FK SPACES.ownerId → USER

> **Annotation 2026-06-12 (ADR-018)** — Renommage `campaigns.owner_id` → `spaces.owner_id` suite à la généralisation de l'espace personnel. Le slug du fichier (`fk-campaign-owner`) est conservé pour la traçabilité historique.

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : CR-3, A-01, A-02
- **Reformalisation de** : décision antérieure connue sous « CRIT-04 » / « ADR-12 » (fichier supprimé lors des remises à zéro)

---

## Contexte

Cette décision avait été validée lors d'une itération précédente de la conception, sous les références internes « CRIT-04 » et « ADR-12 ». Elle est référencée dans deux fichiers de conception actuels (`space-management.md`, `identity-access.md`) mais le fichier ADR d'origine a été supprimé lors des remises à zéro successives. Ce fichier reformalise fidèlement la décision pour rétablir la traçabilité.

**Le problème de fond.** Le champ `Space.ownerId` référence un `UserId` défini dans le bounded context Identity & Access. Cette référence traverse deux bounded contexts. Dans un monolithe modulaire strict, les contextes ne partagent pas de tables — chaque contexte est isolé et communique par événements. Une FK physique entre les tables de deux contextes est une entorse apparente à cette isolation.

---

## Décision

**FK physique réelle `SPACES.ownerId → USER` acceptée dans le contexte d'un monolithe modulaire à base de données unique partagée.** La table `spaces` a une contrainte de clé étrangère réelle vers la table `users`. À la date de cette décision, c'était la seule FK cross-module identifiée et formalisée ; la revue de cadrage P1 (2026-06-09) recense **17 FK cross-module** dans le graphe complet (voir ADR-011). La présente décision reste un précédent de gouvernance valide. La stratégie `ON DELETE` et le mécanisme de cascade sont définis dans **[ADR-011](ADR-011-cascade-integrite-referentielle.md)**.

Suite à ADR-018 (2026-06-12), cette FK s'applique à **tout espace** (`spaces.owner_id`), quel que soit son type : `CAMPAIGN`, `ONE_SHOT` et `PERSONAL`. Un espace `PERSONAL` est possédé par son utilisateur (`owner_id`) au même titre qu'un espace `CAMPAIGN` ou `ONE_SHOT`. La responsabilité billing et RGPD du propriétaire vaut pour l'ensemble de ses espaces.

**Pas d'entité de liaison dans le Shared Kernel / Core.** L'idée d'introduire une entité `SpaceOwner` ou similaire dans le Core pour éviter la FK directe est rejetée. Le Core ne contient jamais d'entités — il contient des abstractions (interfaces, value objects, primitives partagées). Introduire une entité de liaison dans le Core violerait la gouvernance du Shared Kernel.

**En cas d'extraction future.** Si le bounded context Space Management ou Identity & Access est extrait en service séparé, la FK physique sera remplacée par une `UserProjection` locale maintenue par événements (lecture depuis `UserRegistered`, `UserAnonymized`).

---

## Alternatives considérées

**Référence logique par `UserId` nu, sans FK physique.**
Non retenue. Dans une base de données unique partagée, l'absence de FK physique abandonne l'intégrité référentielle sans apporter de bénéfice d'isolation réelle (les tables sont dans le même schéma). Cette option est pertinente uniquement dans une architecture à bases de données séparées par contexte.

**Entité de liaison dans le Shared Kernel / Core.**
Rejetée. Le Core ne contient jamais d'entités — introduire une entité de liaison pour représenter un propriétaire d'espace dans le noyau partagé crée une abstraction qui n'a pas de sens hors du contexte Space Management.

---

## Conséquences

- À la date de cette décision, cette FK était la seule FK cross-module identifiée et formalisée. La revue de cadrage P1 (2026-06-09) recense **17 FK cross-module** ; voir la matrice exhaustive dans [ADR-011](ADR-011-cascade-integrite-referentielle.md). Toute nouvelle FK cross-module doit être traitée comme une décision architecturale explicite.
- **Contrainte sur l'effacement RGPD (ADR-007).** Un `USER` ne peut pas être hard-deleted tant qu'un espace référence son `id` via la FK — qu'il s'agisse d'un espace `CAMPAIGN`, `ONE_SHOT` ou `PERSONAL`. L'anonymisation par réécriture (`email`/`display_name` remplacés par `[Compte supprimé]`, `id` conservé) est la seule option conforme à la fois à l'intégrité référentielle et au droit à l'effacement RGPD.
- Les deux références « (ADR-12) » dans `space-management.md` et `identity-access.md` sont à remplacer par des références à « ADR-009 » avec un lien vers ce fichier.

---

## Compléments post-revue (2026-06-09)

Suite à une revue critique postérieure à cette décision, celle-ci est complétée comme suit, sans changer sa direction.

- **Statué (cadrage P1, 2026-06-09).** Toutes les FK cross-module sont réelles (option a). Le recensement complet (17 FK cross-module) et la stratégie `ON DELETE`/cascade sont dans [ADR-011](ADR-011-cascade-integrite-referentielle.md). L'affirmation « la seule FK cross-module documentée et assumée » dans les sections Décision et Conséquences ci-dessus est corrigée en conséquence.

- **Corriger la FK `membership_characters`.** La FK composite `(space_id, user_id)` définie dans `membership_characters` est invalide car elle cible une colonne non-unique. Correction mécanique C-13 : cibler la clé primaire de `space_memberships`.

- **Annoter les blocs « Note FK inter-modules » des 4 MLD.** Chaque MLD doit indiquer explicitement les FK cross-module retenues et leur exception assumée (J2).

- **Cohérence avec ADR-007 — pseudonymisation.** La FK `ownerId NOT NULL` sur `spaces` implique qu'on ne peut jamais supprimer la ligne `users` tant qu'un espace la référence. Ce comportement est cohérent avec la posture de pseudonymisation actée dans ADR-007 (pas de hard-delete, anonymisation par réécriture).

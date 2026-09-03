# ADR-001 — Exécution du domaine en mode local

- **Statut** : Accepté
- **Date** : 2026-06-09

---

## Contexte

Le mode local — démarrer et travailler sans compte, avec persistance dans le navigateur (IndexedDB) — est un pilier non négociable du produit (UC-01, statut Must). Or le domaine métier est implémenté en C#/.NET côté serveur. Cette question restait non tranchée dans la documentation existante : où s'exécute la logique métier lorsque l'utilisateur travaille hors ligne, sans compte ?

Sans décision explicite, deux chemins implicites coexistaient dans la documentation : soit réimplémenter le domaine côté navigateur en TypeScript, soit traiter le mode local comme une simple couche de persistance sans validation métier. La documentation ne tranchait pas, ce qui rendait la conception du modèle local IndexedDB et la procédure de migration impossibles à spécifier.

---

## Décision

**Le mode local est une couche de persistance et de CRUD côté navigateur avec des validations TypeScript minimales (titre non vide, structure de blocs). Il ne contient pas le domaine C# complet.**

Le domaine C#/.NET côté serveur est la source de vérité unique. Lorsqu'un utilisateur migre ses données locales vers le cloud, la migration est un import revalidé par le domaine serveur — pas une copie de confiance.

Le format de sérialisation utilisé pour l'export local est le même que le payload de migration : un seul format, deux usages.

---

## Alternatives considérées

**(a) Réimplémenter le domaine en TypeScript côté navigateur.**
Écartée. Cela produit deux codebases métier en langages différents qui divergent inévitablement, exactement la « big ball of mud » que le DDD cherche à éviter — à l'échelle inter-langage. La maintenance de deux implémentations cohérentes d'invariants métier complexes n'est pas viable pour une équipe solo.

**(c) Cloud-first, mode local reporté post-MVP.**
Écartée. Cette option renie le pilier local-first, qui est un Must explicite (UC-01). Elle n'est pas compatible avec la vision produit.

---

## Conséquences

- Le mode local est volontairement plus pauvre que le domaine serveur : les invariants métier riches ne sont pas garantis hors ligne. Ce point doit être documenté honnêtement dans UC-01 et dans la vision produit.
- Le modèle de données local (object stores IndexedDB) est à concevoir comme une projection du schéma serveur, avec un sous-ensemble des validations. Ce travail est en J1.
- La migration locale→cloud doit inclure une étape de confirmation anti-appropriation : l'utilisateur doit prouver que les données lui appartiennent avant import. **Statué dans [ADR-016](ADR-016-serialisation-locale-migration.md)** : gate de reconnaissance par présentation des données détectées, pas de preuve formelle d'appartenance possible.
- Le format de sérialisation de l'export local devient un livrable de conception à part entière — il doit être spécifié explicitement. **Statué dans [ADR-016](ADR-016-serialisation-locale-migration.md)** : format JSON avec `schemaVersion` obligatoire, rejet propre des versions inconnues.
- La stratégie de synchronisation (granularité lot ou delta) est à décider en J2.
- La capture d'audience (analytics) est structurellement limitée par le mode local — point à adresser en J2.

---

## Compléments post-revue (2026-06-09)

Suite à une revue critique postérieure à cette décision, celle-ci est complétée comme suit, sans changer sa direction.

- **Ordre de build acté : C#-first.** Le domaine C# serveur est construit en premier (source de vérité). Le modèle local est une projection dérivée du domaine serveur. Le « walking skeleton local-only » n'est plus un jalon isolé.

- **Invariant ajouté : « validation locale ⊆ validation serveur ».** Le mode local peut être plus permissif en lecture mais ne doit jamais accepter en écriture un état que le serveur rejettera à l'import.

- **Préalables bloquants (non différés) :** spécification du format de sérialisation (payload de migration) et parcours d'échec de revalidation à la migration (quarantaine / correction guidée). Le payload est traité comme non fiable et revalidé par les Value Objects à l'import. **Ces deux points sont désormais statués dans [ADR-016 — Sérialisation locale et contrat de migration local→cloud](ADR-016-serialisation-locale-migration.md)** : format `schemaVersion`, gate de reconnaissance anti-appropriation, parcours tout-ou-rien par campagne, rapport de rejets et conservation des données locales en cas de rejet.

- **G-08 :** appeler `navigator.storage.persist()` dès l'entrée en mode local, à traiter avant J1 — risque de perte des données du premier contact utilisateur.

---

## Compléments post-revue (2026-09-03)

Suite à une revue de la décomposition du plan de travail, deux points des Compléments du 2026-06-09 sont révisés.
**La direction de la décision est inchangée** : le domaine C#/.NET côté serveur reste la source de vérité unique, et
le mode local reste une couche de persistance CRUD sans le domaine complet.

- **Ordre C#-first — révisé : autorité du contrat, et non ordre de construction de la couche cliente.** La puce
  « Ordre de build acté : C#-first » des Compléments du 2026-06-09 portait sur l'ordre de construction — domaine C#
  construit en premier, modèle local projeté ensuite. Elle est révisée : **C#-first désigne l'autorité du contrat** —
  le domaine C# est la source de vérité des règles métier et du contrat de données — et ne gouverne plus l'ordre dans
  lequel la couche cliente est construite. La couche Angular et TypeScript du mode local se construit contre le
  contrat du service d'accès au store local, sans attendre l'implémentation du domaine.

  **Ce qui reste interdit** : que la persistance EF Core cloud de J2 précède l'interface locale de J1
  ([ADR-006, § Conséquences](ADR-006-perimetre-mvp.md) — J1 → J2 → J3 s'enchaînent sans découplage).

  **Ce qui reste inchangé** : [ADR-008, § Compléments post-revue](ADR-008-structure-solution.md) — la structure des
  projets .NET demeure le **premier livrable de structure**, avant tout projet Angular ou TypeScript. La présente
  révision ne porte pas sur l'ordre des livrables de structure et n'amende pas ADR-008.

  **Ce qu'une doublure de service ne peut pas porter** : aucune règle métier. L'alternative (a) écartée par la
  § Décision — réimplémenter le domaine en TypeScript côté navigateur — le reste écartée. Une doublure rend des jeux
  de données ; les seules validations autorisées côté client sont celles que
  [structure-projets.md, § 7](../structure-projets.md) énumère.

- **Invariant « validation locale ⊆ validation serveur » — requalifié : visée dirigée, non garantie.** La puce
  d'invariant des Compléments du 2026-06-09 écrit que le mode local « ne doit jamais accepter en écriture un état que
  le serveur rejettera à l'import ». Cette formulation est incompatible avec la § Décision et la § Conséquences de la
  présente décision, qui posent un mode local **volontairement plus pauvre** dont « les invariants métier riches ne
  sont pas garantis hors ligne ». **Ce sont la § Décision et la § Conséquences qui font foi.**

  L'invariant se lit donc **au sens des règles** : aucune règle de validation côté client que le domaine serveur ne
  porte pas. Il en découle que le mode local **peut** accepter un état que le serveur refusera, et que le filet est la
  revalidation systématique par les value objects à l'import, avec rapport de rejets — déjà spécifié par
  [ADR-016, § Conséquences](ADR-016-serialisation-locale-migration.md), qui qualifie cet invariant de « discipline de
  conception dirigée, pas une propriété vérifiée par outillage ».

  **Conséquence opposable** : une validation côté client qui refuse ce que le domaine serveur accepte est un **défaut
  à retirer du client**, non une protection supplémentaire — le domaine serveur fait foi.

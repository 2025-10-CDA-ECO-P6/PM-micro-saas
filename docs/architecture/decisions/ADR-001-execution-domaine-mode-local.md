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

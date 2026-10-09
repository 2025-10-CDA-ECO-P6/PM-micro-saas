# Plan de livraison

> **Nature** : Document dérivé : il dit ce qui est livré à qui à chaque jalon. Il n'ajoute aucun jalon, aucune porte ni aucune date que ses sources ne portent.

## Objet et sources

Ce plan livre le périmètre que la [roadmap de build](roadmap-de-build.md) ordonnance et que la
[roadmap produit](roadmap-produit.md) classe en paliers de valeur, pour le produit Haversack. Il n'en
restate ni les critères de sortie, dont la roadmap de build est propriétaire, ni le contenu : il y renvoie.

Le [MVP est unique](../decisions/ADR-006-perimetre-mvp.md) : ADR-006 (§ Conséquences) enchaîne J1, J2 et J3
« sans découplage produit entre eux », ce qui porte sur le périmètre, non sur la mise à disposition d'une
cohorte. J0 est un socle de build et J2 un jalon enchaîné : aucune source ne les nomme comme mise à
disposition d'un public, ce plan n'en fait pas des livraisons. J1 l'est pour la cohorte pilote, que la
vision nomme et que la roadmap de build mesure après J2 ; le socle complet est livré comme « un bloc
unique et indivisible » ([roadmap produit § 2](roadmap-produit.md#2-le-socle)). Les extensions déclenchées
par signal et la vision long terme sont, pour la roadmap produit, conditionnelles ou d'horizon : elles ne
figurent pas ici comme livraisons.

## Livraisons

### Local-only (J1) à la cohorte pilote

- **Contenu** : le jalon [J1](roadmap-de-build.md#32-j1--local-only) de la roadmap de build.
- **Public visé** : la cohorte pilote restreinte de MJ (early adopters recrutés) que la
  [vision § 2.3](../besoin/vision/vision.md#23-ce-que-le-mvp-doit-démontrer) nomme pour les hypothèses H1 et H2.
- **Porte franchie** : les critères de sortie de [J1](roadmap-de-build.md#32-j1--local-only), propriété de la
  roadmap de build ; la mesure qui en résulte ne quitte le navigateur qu'à la création d'un compte, possible à partir
  de J2 ; elle sert ensuite `[DÉCISION MARCHÉ]`, qui « ne mesure que ce que J1 et J2 livrent »
  ([§ 3.3](roadmap-de-build.md#33-décision-marché--gono-go-avant-le-build-lourd-de-partage--temps-réel)).
- **Condition de retrait** : [SOUS-SPÉCIFIÉ — aucune source ne parle de retrait]

### Le socle

- **Contenu** : les jalons de la roadmap de build, [J0](roadmap-de-build.md#31-j0--socle),
  [J1](roadmap-de-build.md#32-j1--local-only), [J2](roadmap-de-build.md#34-j2--cloud--migration) et
  [J3](roadmap-de-build.md#36-j3--partage--temps-réel) ; le palier [Le socle](roadmap-produit.md#2-le-socle)
  de la roadmap produit ; les épiques et tâches du [plan de travail](plan-de-travail.md#4-vue-des-épiques).
- **Public visé** : les maîtres du jeu et les joueurs, dont les joueurs invités, que la
  [vision](../besoin/vision/vision.md#4-acteurs) nomme ; la roadmap produit désigne « MJ et joueur » comme
  bénéficiaires du palier.
- **Porte franchie** : `[DÉCISION MARCHÉ]` puis `[VALIDATION JURIDIQUE EU]`, les deux points go/no-go de la
  roadmap de build. [§ 3.3](roadmap-de-build.md#33-décision-marché--gono-go-avant-le-build-lourd-de-partage--temps-réel)
  conditionne l'entrée en J3 ; la décision est le fait du responsable produit, qui interprète la mesure d'usage
  des comptes et les entretiens ([§ 1](roadmap-de-build.md#1-séquence-macro-typée)), et son état est porté par
  [decisions-en-attente.md](decisions-en-attente.md#autres-décisions-produit).
  [§ 3.5](roadmap-de-build.md#35-validation-juridique-eu--validation-juridique-pré-lancement)
  conditionne le lancement commercial en Europe sans bloquer le build ; ses axes relèvent de l'avis d'un
  juriste [À VALIDER — JURISTE].
- **Condition de retrait** : [SOUS-SPÉCIFIÉ — aucune source ne dit quel fait observable fait retirer le socle, ni ce qui est retiré]

### Approfondissement du cœur

- **Contenu** : le palier [Approfondissement du cœur](roadmap-produit.md#4-approfondissement-du-cœur) de la
  roadmap produit, avec la correspondance de son [annexe A](roadmap-produit.md#annexe-a--correspondance-avec-le-périmètre-de-conception) ;
  aucun jalon de build ne le porte.
- **Public visé** : les maîtres du jeu et les joueurs, selon la table des [paliers de valeur](roadmap-produit.md#paliers-de-valeur) de la roadmap produit ;
  [vision § 4](../besoin/vision/vision.md#4-acteurs) pour les acteurs.
- **Porte franchie** : le [jalon de validation](roadmap-produit.md#3-le-jalon-de-validation) de la roadmap
  produit, qui précède ce palier (« une fois le socle validé ») [SOUS-SPÉCIFIÉ — aucune source
  ne nomme qui passe ce point go/no-go, ni son rapport à `[DÉCISION MARCHÉ]` de la roadmap de build]
- **Condition de retrait** : [SOUS-SPÉCIFIÉ — aucune source ne dit quel fait observable fait retirer ce palier, ni ce qui est retiré]

## Points ouverts

- `[VALIDATION JURIDIQUE EU]` : le lancement commercial en Europe est conditionné à l'avis d'un juriste sur les axes de la
  [roadmap de build § 3.5](roadmap-de-build.md#35-validation-juridique-eu--validation-juridique-pré-lancement) ;
  l'état de ces axes est porté par [decisions-en-attente.md](decisions-en-attente.md#groupe-4--cadrage-juridique-interne-bloque-le-lancement-dans-lunion-européenne-pas-le-build-de-j3) [À VALIDER — JURISTE]
- le jalon de validation de la roadmap produit n'a pas de décideur nommé [SOUS-SPÉCIFIÉ — la roadmap produit ne dit pas qui passe le go/no-go de son § 3]

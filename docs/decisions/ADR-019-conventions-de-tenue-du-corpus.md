# ADR-019 — Conventions de tenue du corpus

- **Statut** : Accepté
- **Date** : 2026-10-07
- **Nature** : méthode

## Contexte

Le 2026-10-07, une passe d'onboarding a relu l'ensemble du corpus, soit 206 documents. Elle a relevé 118 contradictions entre documents et 75 incertitudes.

La plupart ne viennent pas d'un désaccord sur le fond. Elles tiennent à des règles de tenue que le corpus applique sans les écrire :
- des décisions datées du 2026-09-03 ne sont pas reportées dans les documents de consolidation ;
- deux gabarits de user stories coexistent ;
- cinq mots différents marquent un point clos ;
- aucun document ne dit qui attribue le numéro suivant ;
- la légende des ADR prescrit le remplacement alors que la pratique est l'amendement sur place ;
- les commandes qui vérifient le corpus ne sont déclarées nulle part.

Sans règles écrites, chaque correction réintroduit l'écart qu'elle répare. Ces règles sont aussi la condition pour que le corpus puisse être repris par une autre personne.

## Décision

Le corpus se tient selon douze conventions, retenues par le porteur du projet le 2026-10-07. Chacune est tranchée ci-dessous ; le guide de conventions (§ 8) les résume et renvoie ici.

### 1. Ordre d'autorité

L'ordre est : personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire (glossaire, en-tête). Les documents de consolidation et de pilotage (`docs/reference/`, `docs/projet/`) ne font jamais foi : en cas d'écart, ce sont eux qu'on corrige. Les ADR et les spécifications servent le besoin sans le redéfinir.

### 2. Identifiants

Une seule table recense toutes les familles d'identifiants : UC, US-UC, UJ-UC, US-NN-NN, RB-NN-NN, NFR-XXX-NN, ADR-NNN, AR-NN, TB / EP / LOT, persona-NN. Le numéro suivant est le plus grand numéro de la famille plus un. Un identifiant n'est jamais réutilisé, même retiré.

### 3. Gabarits

Chaque type de document (UC, US, UJ, NFR, fiche écran, ADR) a un gabarit déclaré, dérivé de sa forme majoritaire. Les user stories ont une seule forme, dans laquelle le Gherkin est l'unique porteur des critères d'acceptation. Les documents qui s'écartent de leur gabarit sont alignés.

### 4. Renvois

Un renvoi est un lien relatif : un chemin, plus une ancre si besoin. Il est mort si le fichier, l'ancre ou la section « § » citée n'existe pas. Un nom de fichier nu, ambigu, n'est pas un renvoi.

### 5. Marqueurs

La liste des marqueurs est fermée :
- point ouvert : `À TRANCHER`, `SOUS-SPÉCIFIÉ` ;
- point clos : `DÉCIDÉ`, avec la date et un renvoi vers la décision ;
- fin de vie : `Retiré`, `Remplacé par <id>`.

La légende des statuts d'ADR est inchangée. Les formes `RÉSOLU`, `FERMÉE`, `Clos` et `TRANCHÉ` sont converties, et un point tranché ne garde pas `À TRANCHER`.

*Ajustement du 2026-10-09 : la liste compte deux formes de plus, que le corpus emploie sans que cette section les prévoie. Qualification juridique : `[À VALIDER — JURISTE]` (102 emplois), close par un juriste ; ce n'est pas un point ouvert ordinaire, puisque seul un juriste peut le clore, et la convertir en `À TRANCHER` lui ferait perdre ce sens. Dette reportée à un jalon nommé : `dette nommée, <jalon> (<renvoi vers la décision>)`, par exemple « dette nommée, post-MVP (ADR-016, § 1.4) » ; c'est la tournure des 18 emplois de « dette nommée » du corpus (ADR-015, ADR-016, cahier des spécifications techniques, registre des risques, roadmap de build), et une dette reportée n'est pas non plus un point ouvert. La forme `REPORTÉ` du registre généré de `docs/conventions.md` n'est pas retenue : le corpus ne l'emploie nulle part. En cas d'écart avec ce registre, le présent § 5 fait foi.*

### 6. Registre des décisions

Les ADR sont le registre unique des décisions. La vision (§ 5bis), les arbitrages AR-NN du zoning et `decisions-en-attente.md` y renvoient. Un ajustement se fait sur place, dans une section datée. Un changement de sens fait l'objet d'une nouvelle ADR, qui remplace l'ancienne.

### 7. Reste à faire

`plan-de-travail.md` (TB-nnn) est la source de vérité des tâches. `decisions-en-attente.md` est l'index unique des questions ouvertes : les sections locales y renvoient. Le backlog qui a reçu les constats de l'onboarding (2026-10-07) a été appliqué en entier, puis retiré le 2026-10-09 : `docs/project/backlog/README.md @ 51a7cdc`.

### 8. Vérification

Quatre contrôles vérifient le corpus, lancés depuis la racine :
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : les golden masters de l'outillage ;
- `dotnet run --project tools/VerifCorpus` : liens, ancres et orphelins ;
- `dotnet run --project tools/VerifMailleAgregat` : la maille du plan ;
- `dotnet run --project tools/VerifCouvertureRecette` : la couverture de recette (la sortie en code 1 est attendue tant que des TROU sont assumés).

Ils sont déclarés dans le guide de conventions et lancés en CI sur toute modification de `docs/`.

### 9. Langue

La documentation se lit en français. Le code est en anglais, ainsi que tout ce qui touche au code : identifiants de domaine, noms de types, d'événements, de champs, de tables et d'endpoints, extraits de code. Une prose française cite les noms anglais tels qu'ils seront dans le code.

### 10. Règles métier

Chaque règle RB-NN-NN est définie une seule fois, dans l'épique de user stories qui la porte. Les autres documents la citent sans la redéfinir.

### 11. Glossaire

Le glossaire est la référence des noms de termes ; le sens d'une règle reste porté par sa source. Un terme renommé est propagé dans tout le corpus, et l'ancien nom n'est gardé que comme alias dans le glossaire.

### 12. Flux

Chaque niveau de flux a un seul propriétaire :
- les user journeys portent le parcours de chaque UC ;
- les parcours transverses n'ajoutent que l'enchaînement entre UC, et citent l'UC au lieu de le paraphraser ;
- le zoning porte la navigation ;
- les flux du domaine portent le comportement du système.

**Raison.** Chacune de ces douze règles reprend la pratique majoritaire du corpus quand elle était saine, et la corrige quand elle produisait les écarts relevés. Le porteur du projet les a choisies axe par axe le 2026-10-07, sur proposition et après avoir vu, pour chaque axe, la pratique observée et ses défauts.

**Références.** Entretien d'onboarding du 2026-10-07 ; docs/corpus.md ; docs/project/backlog/README.md @ 51a7cdc

## Alternatives considérées

- Garder chaque pratique telle qu'observée, sans règle écrite : écarté, c'est précisément ce qui a produit les 118 contradictions relevées.
- Registre des décisions avec remplacement strict (toute modification d'une ADR acceptée passe par une nouvelle ADR) : écarté, car le corpus amende déjà ses ADR par des compléments datés, et une nouvelle ADR pour chaque ajustement disperserait une même décision.
- Reste à faire tenu entièrement dans le backlog (`docs/project/backlog/README.md @ 51a7cdc`), le plan n'en étant que la projection : écarté, car plan-de-travail.md porte déjà les dépendances et la maille des tâches, et l'outillage (VerifMailleAgregat, CreerIssues) s'appuie sur lui.
- Contrôles déclarés sans CI : écarté, car un contrôle qu'on ne lance pas à chaque changement ne signale l'écart qu'après coup.

## Conséquences

Ce qui devient vrai :
- un écart entre deux documents a toujours un côté qui fait foi ;
- un identifiant a une seule origine ;
- un point ouvert se reconnaît à un seul mot ;
- un repreneur sait quelles commandes vérifient le corpus.

Ce qui coûte : l'alignement du corpus existant sur ces règles. Il a été inscrit dans le backlog (`docs/project/backlog/README.md @ 51a7cdc`, entrées 0001 à 0092) : gabarits des US et des UJ, marqueurs à convertir, doublon RB-05-01, renvois « § » non contrôlés.

Deux contrôles automatiques restent à écrire :
- l'extension de VerifCorpus aux renvois « § » ;
- la vérification des marqueurs hors liste fermée.

La mise en CI des quatre contrôles est aussi à faire.

*Ajustement du 2026-10-09 : la mise en CI est faite (`.github/workflows/controles-corpus.yml`). Elle lance cinq contrôles : les quatre du § 8 et `tools/VerifMarqueursIdentifiants` (marqueurs hors de la liste fermée du § 5, identifiants définis deux fois). Les deux contrôles automatiques annoncés ci-dessus sont écrits : `tools/VerifCorpus` couvre les renvois « § », `tools/VerifMarqueursIdentifiants` les marqueurs hors liste fermée. Le § 8 reste tel que décidé le 2026-10-07.*

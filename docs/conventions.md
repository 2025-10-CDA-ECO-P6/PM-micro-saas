# Conventions du corpus

- **Statut** : En vigueur, tranché par l'ADR-019

## Ordre d'autorité entre artefacts

Quand deux documents divergent, celui qui est listé le premier est celui qu'on croit. L'ordre est tranché par [ADR-019, § 1. Ordre d'autorité](decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité).

<!-- corpus:generated:authority-order:begin -->
1. Conventions
2. Traçabilité
3. Journal du corpus
4. Personas
5. Vision
6. Cas d'utilisation
7. Matrice de priorités
8. Diagrammes des cas d'utilisation
9. Exclusions
10. User stories
11. Parcours utilisateur
12. Parcours de bout en bout
13. Exigences non fonctionnelles
14. Guide d'entretien
15. Comptes rendus d'entretien
16. Contextes du domaine
17. Noyau partagé
18. Diagrammes de classes du domaine
19. Diagrammes de flux
20. Modèle conceptuel de données
21. Modèle logique de données
22. Catalogue des événements
23. Glossaire
24. Zonage de l'interface
25. Conventions d'interface
26. Fiches d'écran
27. Maquettes filaires
28. Raisonnement UX
29. Guide de contenu
30. Décisions
31. Stack technologique
32. Structure de la solution
33. Fondations architecturales
34. Architecture transverse
35. Spécifications préalables
36. Dossier de sécurité
37. Cadres de conformité
38. Brouillons publics de conformité
39. Stratégie de test
40. Registre des traitements
41. Analyse d'impact relative à la protection des données
42. Cahier des charges fonctionnel
43. Cahier des spécifications techniques
44. Dossier d'architecture
45. Conception détaillée
46. Note business
47. Note de cadrage du projet
48. Conventions et définition de fini
49. Roadmap de build
50. Roadmap produit
51. Plan de travail
52. Méthode de ticket
53. Registre des risques
54. Décisions en attente
55. Guide de lecture
56. Revues du plan
57. Plan de livraison
58. Modèle de fonctionnement
<!-- corpus:generated:authority-order:end -->

Les documents de consolidation et de pilotage ne font jamais foi : en cas d'écart, ce sont eux qu'on corrige. Les ADR et les spécifications servent le besoin sans le redéfinir.

Trois conséquences découlent de cet ordre :

- les use cases précèdent le domaine : en cas d'écart entre les deux, le use case fait foi ;
- dans la chaîne du besoin (personas → vision → use cases → parcours, stories, exigences non fonctionnelles → domaine → glossaire), le glossaire vient en dernier : il fait référence pour le nom des termes, jamais pour le sens d'une règle ;
- le sens d'une règle reste porté par sa source ([ADR-019, § 11. Glossaire](decisions/ADR-019-conventions-de-tenue-du-corpus.md#11-glossaire)).

## Autorité de forme et autorité de fond

Le glossaire fait référence pour le nom des termes ; le sens d'une règle reste porté par sa source. Un terme renommé est propagé dans tout le corpus, et l'ancien nom n'est gardé que comme alias dans le glossaire ([ADR-019, § 11. Glossaire](decisions/ADR-019-conventions-de-tenue-du-corpus.md#11-glossaire)).

Quand le glossaire et un use case divergent sur le fond, l'ordre d'autorité tranche : le use case précède le glossaire, il fait foi.

## Sens de dépendance entre besoin et décision

Les ADR et les spécifications servent le besoin sans le redéfinir ([ADR-019, § 1. Ordre d'autorité](decisions/ADR-019-conventions-de-tenue-du-corpus.md#1-ordre-dautorité)).

Le filtre de nature, le test d'autoportance, les motifs interdits et leur responsable sont portés par l'[ordre d'autorité, § Sens de dépendance besoin ↔ décision](reference/ordre-d-autorite.md#sens-de-dépendance-besoin--décision).

## Registre des identifiants

Une seule table recense toutes les familles d'identifiants. Le numéro suivant est le plus grand numéro de la famille plus un. Un identifiant n'est jamais réutilisé, même retiré ([ADR-019, § 2. Identifiants](decisions/ADR-019-conventions-de-tenue-du-corpus.md#2-identifiants)).

<!-- corpus:generated:identifier-registry:begin -->
| Unité | Format | Motif | Module | Retrait |
|---|---|---|---|---|
| instance | `persona-NN` | `persona-[0-9][0-9]` | Personas | ligne de tableau marquée retirée |
| hypothesis | `H<n>` | `H[0-9]+` | Vision | ligne de tableau marquée retirée |
| arbitration | `ARB-NN` | `ARB-[0-9][0-9]` | Vision | ligne marquée retirée |
| instance | `UC-NN` | `UC-[0-9]{2}` | Cas d'utilisation | - |
| alternative | `A<n>` | `A[0-9]+` | Cas d'utilisation | - |
| exception | `E<n>` | `E[0-9]+` | Cas d'utilisation | - |
| exclusion | `UC-F<n>` | `UC-F[0-9]+` | Exclusions | statut Spécifiée |
| instance | `US-UC-NN` | `US-UC-[0-9]{2}` | User stories | - |
| story | `US-NN-KK` | `US-[0-9]{2}-[0-9]{2}` | User stories | section Stories exclues ou repoussées |
| business-rule | `RB-NN-KK` | `RB-[0-9]{2}-[0-9]{2}` | User stories | ligne marquée retirée |
| instance | `UJ-UC-NN` | `UJ-UC-[0-9]{2}` | Parcours utilisateur | - |
| instance | `journey-NN` | `journey-[0-9]{2}` | Parcours de bout en bout | - |
| seam | `C<n>` | `C[0-9]+` | Parcours de bout en bout | - |
| rupture | `RUPTURE-<n>` | `RUPTURE-[0-9]+` | Parcours de bout en bout | - |
| instance | `NFR-<FAM>-NN` | `NFR-[A-Z0-9]+-[0-9][0-9]` | Exigences non fonctionnelles | - |
| surface | `SV<n>` | `SV[0-9]+` | Zonage de l'interface | - |
| arbitration | `AR-NN` | `AR-[0-9]{2}` | Zonage de l'interface | - |
| instance | `ADR-NNN` | `ADR-[0-9]{3}` | Décisions | statut Remplacé |
| surface | `S<n>` | `S[0-9]+` | Dossier de sécurité | ligne de tableau marquée retirée |
| countermeasure | `#<n>` | `#[0-9]+` | Dossier de sécurité | ligne de tableau marquée retirée |
| gap | `L<n>` | `L[0-9]+` | Dossier de sécurité | ligne marquée retirée |
| case | `CR-UCNN-KK` | `CR-UC[0-9]{2}-[0-9]{2}` | Stratégie de test | ligne de tableau marquée retiré |
| transverse-case | `CR-TRANS-NN` | `CR-TRANS-[0-9]{2}` | Stratégie de test | ligne de tableau marquée retiré |
| open-point | `PO-NN` | `PO-[0-9]{2}` | Stratégie de test | - |
| processing | `PR-NN` | `PR-[0-9]{2}` | Registre des traitements | ligne de tableau marquée retirée |
| risk | `IR-NN` | `IR-[0-9]{2}` | Analyse d'impact relative à la protection des données | ligne de tableau marquée retirée |
| item | `TB-NNN` | `TB-[0-9]{3}` | Plan de travail | section Fiches retirées |
| epic | `EP-NN` | `EP-[0-9]{2}` | Plan de travail | - |
| batch | `LOT-NN` | `LOT-[0-9]{2}` | Plan de travail | - |
| risk | `<FAM>-NN` | `[TPJD]-[0-9]{2}` | Registre des risques | ligne marquée retirée |
<!-- corpus:generated:identifier-registry:end -->

## Registre des marqueurs

La liste des marqueurs est fermée ([ADR-019, § 5. Marqueurs](decisions/ADR-019-conventions-de-tenue-du-corpus.md#5-marqueurs)). La légende des statuts d'ADR est inchangée. Les formes `RÉSOLU`, `FERMÉE`, `Clos` et `TRANCHÉ` sont converties, et un point tranché ne garde pas `À TRANCHER`. L'ADR-019 § 5 fait foi : en cas d'écart avec le registre ci-dessous, c'est lui qu'on suit.

<!-- corpus:generated:marker-registry:begin -->
| Marqueur | Sens | Clos par |
|---|---|---|
| `[À DÉCIDER — <axe>]` | une décision non prise ; l'axe nomme qui ou quoi la tranche | le décideur que l'axe nomme |
| `[HORS PÉRIMÈTRE — <source>]` | une exclusion décidée ; la source nomme la décision | personne : elle est terminale |
| `REPORTÉ` | un point reporté à un jalon nommé | ce jalon |
| `[RÉSOLU — <source>]` | un point qui était ouvert et qui est tranché ; la source dit où | personne : il est terminal |
| `[SOUS-SPÉCIFIÉ — <motif>]` | un élément que le besoin ne tranche pas assez pour être dessiné ; le motif dit ce qui manque | le document ou la personne qui tranche ce qui manque |
| `[ABSENT PAR NATURE — <motif>]` | un élément qui n'existe pas sur cet écran à cause de ce qu'est l'écran, et non parce qu'il est désactivé | personne : il est terminal |
| `[DÉSACTIVÉ — <condition>]` | un élément qui existe sur l'écran et ne peut être utilisé tant que la condition n'est pas remplie | la condition qu'il nomme |
| `[À VALIDER — JURISTE]` | une qualification juridique | un juriste |
| `[À COMPLÉTER — <responsable>]` | une valeur que le responsable doit fournir | le responsable qu'il nomme |
| `TROU` | une fiche de travail ou un critère qui pointe vers ce que le corpus ne nomme pas | le module amont qui le nomme |
| `HORS MAILLE` | une fiche de travail dont le périmètre ne vise aucun module de code | personne : il est terminal |
<!-- corpus:generated:marker-registry:end -->

## Renvois

Un renvoi est un lien relatif : un chemin, plus une ancre si besoin. Il est mort si le fichier, l'ancre ou la section « § » citée n'existe pas. Un nom de fichier nu, ambigu, n'est pas un renvoi ([ADR-019, § 4. Renvois](decisions/ADR-019-conventions-de-tenue-du-corpus.md#4-renvois)).

Les pratiques de rédaction d'un renvoi (rechercher la chaîne citée après une réécriture, renvoyer par section, chercher par revendication, ouvrir la cible au moment d'écrire) sont portées par le [guide de conventions, § 8.1 à 8.4](projet/conventions-dod.md#81-vérifier-la-chaîne-citée-au-moment-où-lon-réécrit).

## Index

Chaque dossier est propriétaire de son propre index ; un index de niveau supérieur n'en recopie pas le contenu ([index de `docs/`](README.md)).

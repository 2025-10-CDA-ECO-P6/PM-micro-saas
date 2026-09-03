# Méthode de ticket — Haversack

| Champ | Valeur |
|---|---|
| Statut | Méthode dérivée du plan de travail et du guide de conventions, à confirmer à l'entrée en build |
| Audience | la ou les personnes qui prennent en charge une tâche du plan de travail, seules ou en petit collectif — y compris une personne découvrant le projet sans pouvoir en joindre l'auteur |
| Sources | `docs/gestion-projet/plan-de-travail.md`, `docs/gestion-projet/guide-conventions-et-dod.md`, `docs/test/cahier-strategie-test-et-recette.md`, `docs/gestion-projet/roadmap-entree-build.md`, `docs/architecture/structure-projets.md` |

---

## Bandeau de cadrage — frontière et autorité

**Ce document n'énonce pas de Definition of Done.** Elle existe déjà, complète, dans [`guide-conventions-et-dod.md §9 — Definition of Done`](guide-conventions-et-dod.md) ; elle n'est pas réécrite ici. Ce que ce document énonce est une **condition d'entrée** — ce qui doit être vrai avant de prendre une tâche — que rien dans le corpus ne porte encore. Condition d'entrée et Definition of Done décrivent deux moments distincts et complémentaires d'une même tâche : l'un avant qu'elle ne soit prise, l'autre avant qu'elle ne se ferme.

> **En cas de conflit entre ce document et une source citée** (plan de travail, guide de conventions et DoD, cahier de stratégie de test et de recette, roadmap d'entrée en build), **la source citée fait foi.** Ce document est un artefact de méthode, pas une nouvelle autorité de corpus — il ne redéfinit aucun but, aucune dépendance, aucun critère d'acceptation, il décrit comment les faire transiter par un ticket.

---

## 1. Modèle de ticket

**Principe structurant.** Le plan de travail porte ce qui est invariant à l'effectif qui l'exécute — but, dépendances, périmètre d'écriture, taille, critères d'acceptation ([`plan-de-travail.md §1`](plan-de-travail.md), règle (a)). Le ticket porte ce qui varie avec cet effectif — l'état d'avancement, qui l'a pris, ce qui a été constaté en le faisant. C'est ce qui rend vérifiable la règle selon laquelle aucune tâche du plan ne porte de « qui » : le seul champ de tout le dispositif — plan et tickets ensemble — qui nomme une personne vit dans le ticket, donc dans un autre fichier que le plan, jamais dans `plan-de-travail.md` lui-même.

Champs attendus au minimum :

| Champ | Obligatoire | Pourquoi il n'est pas déjà porté par le plan |
|---|---|---|
| **Titre** | oui | Le plan porte un intitulé de tâche, pas un titre d'issue — c'est un artefact distinct, qui doit rester comparable littéralement au premier pour détecter une divergence. |
| **Tâche de plan** | oui | C'est l'unique lien vers le but, les dépendances, le périmètre d'écriture, la taille et les critères d'acceptation de la tâche — des champs déjà portés par `plan-de-travail.md`, qu'aucun ticket ne recopie. |
| **Libellés** | oui | Le plan ne classe pas ses tâches par jalon, nature de tranche ou nature de vérification — c'est une lecture transverse au plan, propre au suivi d'exécution, absente de sa structure (`plan-de-travail.md §4`, vue des épiques par jalon uniquement). |
| **État** | oui | Le plan ne porte aucune notion d'avancement — une tâche y est décrite une fois, indépendamment du moment où elle est prise en charge. |
| **Pris par** | non, renseigné à la prise | C'est le seul champ du dispositif entier qui nomme une personne — le plan ne peut structurellement pas le porter (`plan-de-travail.md §1`, règle (a)). |
| **Notes d'exécution** | non | Texte libre sans valeur normative, propre au déroulé d'une prise en charge donnée — rien de comparable n'existe dans le plan. |
| **Écart constaté** | non, mais important | C'est le chemin de retour vers le corpus : ce que le corpus dit et ce que le code impose, avec un renvoi vers le point à amender. Sans lui, une tâche menée jusqu'à sa Definition of Done s'écarte silencieusement de la conception, y compris sur un point encore `[À TRANCHER — J0]` que le ticket est chargé de clore. |

**Titre.** L'intitulé de la tâche `TB-nnn` du plan, recopié verbatim, précédé de son identifiant. Une divergence entre le titre du ticket et l'intitulé du plan est alors détectable par simple comparaison littérale des deux chaînes, sans avoir à ouvrir le plan pour la constater.

**Tâche de plan.** Le champ porte l'identifiant `TB-nnn` seul. Le ticket ne recopie aucun des champs qu'il pointe (but, dépendances, périmètre d'écriture, taille, critères d'acceptation) : un champ recopié à deux endroits est un champ qui dérive dès que l'un des deux change sans l'autre — exactement la famille de défaut que [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md) a nommée pour le reste du corpus documentaire, et que ce modèle de ticket étend au report plan → ticket.

**Un fait à porter, mesuré sur le plan.** Des tranches de `plan-de-travail.md` portent `TROU` en critères d'acceptation : le corpus ne fournit pas de critère vérifiable pour le point qu'elles couvrent. Ce document ne recopie aucun décompte de ces tranches — il renvoie au plan pour les identifier. Ce qu'un ticket ouvert sur l'une d'elles doit faire : refléter cette absence plutôt que la masquer. Une tranche dont les critères portent `TROU` n'est pas prête au sens du §2 — le ticket qui lui correspond reste à l'état non prêt (§5) jusqu'à ce que le corpus source referme le trou, indépendamment de toute autre condition par ailleurs satisfaite.

**À distinguer du marqueur `HORS MAILLE`** en périmètre d'écriture, qui ne signale aucun manque : la tranche ne vise simplement aucun module de code, et cela ne l'empêche ni d'être prête, ni d'être close.

---

## 2. Definition of ready

Une tâche du plan est **prête à être prise** quand, cumulativement, les conditions suivantes sont satisfaites. Chacune se vérifie en ouvrant un document, sans avoir à demander l'avis de quiconque — c'est le critère qui fixe la liste : une condition qu'on ne pourrait pas trancher seul n'a pas sa place ici.

1. **La tâche existe dans le plan.** L'identifiant `TB-nnn` désigne une fiche de tâche réelle dans [`plan-de-travail.md`](plan-de-travail.md) — vérifiable en l'y ouvrant.
2. **Ses dépendances sont closes.** Chaque tâche nommée dans le champ `Dépend de` de `TB-nnn` correspond à un ticket à l'état terminal du cycle de vie décrit au §5 — vérifiable en ouvrant les tickets liés.
3. **Aucun conflit n'est en cours.** Aucune tâche nommée dans le champ `En conflit avec` de `TB-nnn` ne correspond à un ticket aux états « Pris » ou « En cours » (§5) — vérifiable en ouvrant les tickets liés.
4. **Un critère d'acceptation existe et sa cible aussi.** Le champ `Critères d'acceptation` de `TB-nnn` porte au moins un renvoi dont la cible existe réellement — vérifiable en ouvrant cette cible. Une valeur `TROU` ne constitue pas un renvoi et ne satisfait donc pas cette condition.
5. **Un périmètre d'écriture est renseigné.** Le champ `Périmètre d'écriture` de `TB-nnn` porte au moins un module au sens de la maille de désignation ([`plan-de-travail.md §2`](plan-de-travail.md)) — vérifiable en ouvrant cette section. Deux marqueurs s'y distinguent, et un seul fait échouer la condition :
   - **`HORS MAILLE`** — la tâche ne vise aucun module de code (confirmer une décision, retenir un outil, arrêter une convention, renseigner une colonne de verdict). Elle **satisfait** cette condition : il n'y a rien à nommer, et la maille ne parle que de code.
   - **`TROU`** — la tâche vise un module de code que le corpus ne nomme pas. Elle **ne satisfait pas** cette condition.

**Aucune de ces cinq conditions ne recouvre la Definition of Done** ([`guide-conventions-et-dod.md §9`](guide-conventions-et-dod.md)), qui décrit une condition de **sortie**. Celles-ci décrivent une condition d'**entrée** : une tâche peut être prête au sens de cette section sans qu'aucun code n'ait encore été écrit, exactement comme elle peut, à l'inverse, avoir tout son code écrit et rester loin de sa Definition of Done.

---

## 3. Taxonomie de libellés

Trois axes orthogonaux, à valeurs fermées, chacun ancré sur une source du corpus.

**Par jalon** — les jalons de build nommés par [`roadmap-entree-build.md §1`](roadmap-entree-build.md), ligne « (a) Jalon de build » de son tableau des natures de nœuds :

| Valeur | Ancrage |
|---|---|
| J0 | `roadmap-entree-build.md §1` |
| J1 | `roadmap-entree-build.md §1` |
| J2 | `roadmap-entree-build.md §1` |
| J3 | `roadmap-entree-build.md §1` |

Les deux points de décision qui séparent ces jalons — `[DÉCISION MARCHÉ]` et `[VALIDATION JURIDIQUE EU]` — ne sont pas des valeurs de cet axe : `roadmap-entree-build.md §1` les qualifie de `NON-VERIFIABLE-IN-BUILD`, donc par construction non vérifiables par une tâche de code, donc par aucun ticket de cette taxonomie.

**Par nature de tranche** — dérivée des natures de module que pose la maille de désignation du plan ([`plan-de-travail.md §2`](plan-de-travail.md)). La valeur se **déduit du champ `Périmètre d'écriture`** de la tranche ; elle n'est jamais saisie à la main :

| Valeur | Déduite quand le périmètre porte… | Ancrage |
|---|---|---|
| Comportement | un agrégat du modèle de domaine | `plan-de-travail.md §2`, première nature |
| Surface | une fiche d'écran | `plan-de-travail.md §2`, deuxième nature |
| Socle | un projet .NET, le noyau partagé, ou un namespace de bounded context | `plan-de-travail.md §2`, troisième nature |
| Couche cliente | un des six modules de la couche cliente | `plan-de-travail.md §2`, quatrième nature |
| Outillage | un module d'outillage | `plan-de-travail.md §2`, cinquième nature |
| Hors maille | le marqueur `HORS MAILLE` | `plan-de-travail.md §2` |

**Pourquoi cet axe a remplacé un axe « par couche ».** La version antérieure de cette taxonomie classait par couche technique, ce qui était classifiant tant qu'une tâche *était* une couche. Depuis que les tranches les traversent, une tranche de comportement écrit le Domaine, l'Application et la persistance locale : elle recevrait trois libellés de couche, et un libellé posé trois fois sur la même issue ne filtre plus rien. La nature de tranche, elle, reste une valeur unique dans la quasi-totalité des cas.

**Cet axe ne recopie pas le périmètre.** Il en donne la **catégorie**, pas le contenu : le module exact vit dans le champ `Périmètre d'écriture` de la tranche, que le ticket pointe déjà par son champ `Tâche de plan`. Reproduire ici les noms d'agrégats ou de fiches d'écran serait la duplication que [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md) proscrit ; en donner la nature est une lecture transverse que le plan ne porte pas en champ.

**Par nature de vérification** — dérivée des niveaux et des axes transverses de [`cahier-strategie-test-et-recette.md §3`](../test/cahier-strategie-test-et-recette.md) :

| Valeur | Ancrage |
|---|---|
| Unitaire | `cahier-strategie-test-et-recette.md §3.1` |
| Intégration | `cahier-strategie-test-et-recette.md §3.2` |
| e2e | `cahier-strategie-test-et-recette.md §3.3` |
| Test d'architecture (CI) | `cahier-strategie-test-et-recette.md §3.4` |
| Sécurité (adversarial) | `cahier-strategie-test-et-recette.md §3.5` |
| Accessibilité (axe transverse) | `cahier-strategie-test-et-recette.md §3.6` |
| Réactivité perçue (axe transverse) | `cahier-strategie-test-et-recette.md §3.7` |
| Anomalie | `cahier-strategie-test-et-recette.md §7` |

La valeur `Anomalie` n'est pas un niveau de test : c'est le type de ticket que présuppose déjà le cycle de vie et l'échelle de sévérité décrits par `cahier-strategie-test-et-recette.md §7` (renvoi seul — voir §5 ci-dessous). Sans cette valeur, un ticket signalant un défaut constaté en cours de build n'aurait aucune nature à porter dans cette taxonomie.

**Interdiction nommée : aucun axe « priorité ».** [`cahier-strategie-test-et-recette.md §7`](../test/cahier-strategie-test-et-recette.md) écrit littéralement que « l'échelle de priorité elle-même (ex. P1-P4, ou tout autre barème) reste à la main de l'équipe de build — aucun barème de priorité n'est dérivable du corpus ». Poser un tel axe ici contredirait cette non-décision explicite. `[À TRANCHER — J0]` : si un axe de priorité s'avère nécessaire à l'usage, il reste à définir par l'équipe de build — ce document n'en propose aucune valeur. Noter que l'espace `P-nn` est par ailleurs déjà pris par le registre des risques du dossier.

---

## 4. Chemin d'entrée d'un nouvel arrivant

Le [`README.md`](../../README.md) racine porte déjà, en sa section [« Par où commencer »](../../README.md#par-où-commencer), un ordre de lecture pour une personne qui reprend le projet en vue d'écrire la première ligne de code. Ce document ne le refait pas — il le prolonge, jusqu'à la prise du premier ticket :

1. **Les étapes de [« Par où commencer »](../../README.md#par-où-commencer)** — renvoi seul, non recopié ici : elles apportent la carte du corpus documentaire, la séquence de jalons et ses conditions de passage, les conventions de code et la Definition of Done, puis la structure concrète des projets .NET.
2. **[`guide-lecture-par-fonctionnalite.md`](guide-lecture-par-fonctionnalite.md)** — apporte l'entrée par fonctionnalité produit : pour toute capacité du périmètre, le renvoi vers son besoin, ses règles métier, sa maquette, les décisions d'architecture qui la contraignent, sa spécification technique et ses cas de recette.
3. **[`plan-de-travail.md`](plan-de-travail.md)** — apporte la décomposition en épiques et en tâches exécutables du périmètre engagé, avec dépendances techniques, périmètre d'écriture et conflits calculés depuis ce périmètre.
4. **Ce document** — apporte le modèle de ticket, la condition d'entrée à vérifier avant de prendre une tâche (§2), et le cycle de vie à suivre jusqu'à sa clôture (§5).
5. **La prise du premier ticket** — choisir, dans le plan, une tâche dont la Definition of ready (§2) est satisfaite ; ouvrir un ticket dans le dépôt du projet en suivant le modèle du §1 ; renseigner `Pris par` au moment de la prise.

---

## 5. Cycle de vie d'un ticket

**États et transitions**, pour un ticket portant sur une tâche du plan :

```
Ouvert, non prêt  →  Prêt  →  Pris  →  En cours  →  En revue  →  Terminé
                                            ↑____________|
                                    (la revue renvoie le ticket
                                     en cours si elle ne le clôt pas)
```

- **Ouvert, non prêt** — le ticket existe, mais la Definition of ready (§2) n'est pas encore satisfaite pour la tâche qu'il porte.
- **Prêt** — la Definition of ready est satisfaite ; personne ne l'a encore pris.
- **Pris** — une personne l'a pris en charge ; le champ `Pris par` est renseigné.
- **En cours** — l'implémentation est engagée.
- **En revue** — l'implémentation est proposée, en attente de la revue prévue par la Definition of Done ([`guide-conventions-et-dod.md §9`](guide-conventions-et-dod.md), renvoi seul).
- **Terminé** — la Definition of Done est satisfaite pour cette tâche ; le ticket se ferme. Ce qui ferme un ticket est ce renvoi, jamais une liste de critères réécrite ici.

**Pour les tickets de nature « Anomalie »** (§3), le cycle de vie et l'échelle de sévérité sont ceux de [`cahier-strategie-test-et-recette.md §7`](../test/cahier-strategie-test-et-recette.md) — renvoi seul, non redit ici. Ils ne suivent pas le cycle ci-dessus, propre à une tâche de développement.

---

## 6. Où naissent les tickets, et ce qui fait foi

Les tickets sont ouverts dans les issues du dépôt du projet — le dépôt distant dont ce clone est issu.

**Le plan versionné est la source de vérité ; les issues en sont le report — jamais l'inverse.** Sans cette hiérarchie, le découpage en tâches disparaîtrait du clone dès lors qu'une issue serait fermée ou recréée, et une personne reprenant le projet depuis ce dépôt ne le retrouverait plus.

**Le plan ne porte aucun numéro d'issue.** Le lien entre une tâche et son ticket se fait par l'identifiant `TB-nnn` placé en tête du titre du ticket (§1, champ Titre), ce qui le rend retrouvable par recherche dans les deux sens. Un numéro d'issue inscrit dans le plan serait une donnée que le plan ne possède pas nativement — à resynchroniser à chaque création, fermeture ou recréation de ticket. C'est exactement la famille de dérive que [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md) a déjà nommée et corrigée pour le reste du corpus documentaire : un renvoi qui pointe ne dérive pas quand sa cible bouge, une donnée recopiée doit être resynchronisée à chaque changement de la source.

---

## 7. Ce que cette méthode ne tranche pas

- **La convention de message de commit et de nommage de branche.** [`guide-conventions-et-dod.md §5`](guide-conventions-et-dod.md) la laisse explicitement `[À TRANCHER — J0]` : « Le corpus de conception ne définit aucun format de message de commit ni de convention de nommage de branche. […] un candidat courant est Conventional Commits, à ratifier — ce guide ne le tranche pas. » Le plan porte la tâche qui clôt ce point : **TB-012 — Arrêter la convention de commit et de branche** (`plan-de-travail.md §5`). Ce document ne tranche rien à sa place.
- **Le barème de priorité des tickets.** Interdit à cette méthode par une non-décision explicite du corpus (§3 ci-dessus) ; marqué `[À TRANCHER — J0]`, sans valeur proposée.
- **L'outillage concret de la taxonomie et du modèle dans le dépôt du projet** — libellés réels, modèle d'issue, champs personnalisés. Aucune source du corpus de conception ne fixe ce point ; `[À TRANCHER — J0]`, à la charge de l'équipe de build au moment d'ouvrir le dépôt de code, dans le même esprit que la note de portée de [`guide-conventions-et-dod.md §0`](guide-conventions-et-dod.md) sur l'instrumentation outillée.

---

## 8. Renvois

- Décomposition en tâches, dépendances, périmètre d'écriture, taille : [`plan-de-travail.md`](plan-de-travail.md)
- Entrée par fonctionnalité produit : [`guide-lecture-par-fonctionnalite.md`](guide-lecture-par-fonctionnalite.md)
- Conventions de code et Definition of Done : [`guide-conventions-et-dod.md`](guide-conventions-et-dod.md)
- Conventions de maintenance du corpus documentaire (famille de dérive par recopie) : [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md)
- Séquence de jalons, préalables bloquants, critères de sortie : [`roadmap-entree-build.md`](roadmap-entree-build.md)
- Niveaux de test, axes transverses, cycle de vie d'anomalie et sévérité : [`cahier-strategie-test-et-recette.md`](../test/cahier-strategie-test-et-recette.md)
- Granularité des projets .NET et périmètre du mode local TypeScript : [`structure-projets.md`](../architecture/structure-projets.md)
- Ordre de lecture pour une personne qui reprend le projet : [`README.md`, § Par où commencer](../../README.md#par-où-commencer)

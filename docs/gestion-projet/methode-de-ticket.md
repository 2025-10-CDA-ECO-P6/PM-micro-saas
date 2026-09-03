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

**Deux natures, par exception nommée.** Ce dossier porte déjà un document qui mêle règle et fait sans se contredire : `plan-de-travail.md` pose des règles de lecture, puis porte la décomposition en tranches et en lots qui en résulte — un contenu constaté, pas une règle nouvelle. **Ce précédent établit seulement qu'un même document peut mêler les deux registres sans se contredire — pas de régime d'autorité à deux vitesses** : la clause de conflit du bandeau du plan s'applique à ce document entier, sans distinction de section. Ce document fonde son propre régime d'autorité, par exception nommée plutôt que par rang de section : toute section de ce document est **normative** — chacune pose une règle ou une condition, opposable au même titre que le reste de ce bandeau, et la clause de conflit ci-dessus s'y applique pleinement —, à l'exception de la section **Registre de trajectoire**, qui est **factuelle** : elle n'énonce aucune règle, elle enregistre ce qui a été exécuté ; la clause de conflit ci-dessus ne s'y applique pas de la même façon, et c'est cette section elle-même, à son ouverture, qui fonde et précise sa propre frontière d'autorité.

---

## 1. Modèle de ticket

**Principe structurant.** Le plan de travail porte ce qui est invariant à l'effectif qui l'exécute — but, dépendances, périmètre d'écriture, taille, critères d'acceptation ([`plan-de-travail.md §1`](plan-de-travail.md), règle (a)). Le ticket porte ce qui varie avec cet effectif — l'état d'avancement, qui l'a pris, ce qui a été constaté en le faisant. C'est ce qui rend vérifiable la règle selon laquelle aucune tâche du plan ne porte de « qui » : le seul champ de tout le dispositif — plan et tickets ensemble — qui nomme une personne vit dans le ticket, donc dans un autre fichier que le plan, jamais dans `plan-de-travail.md` lui-même.

**Le critère qui gouverne ce tableau : la décidabilité, pas l'absence de duplication.** Le champ `Titre`, plus bas, en est la preuve littérale : il **recopie** verbatim l'intitulé de la tâche, et cette recopie est licite précisément parce qu'« une divergence entre le titre du ticket et l'intitulé du plan est alors détectable par simple comparaison littérale des deux chaînes, sans avoir à ouvrir le plan pour la constater » (§1, champ `Titre`). Un champ n'est donc pas exclu de ce ticket parce qu'il duplique une donnée du plan — il l'est quand sa duplication rendrait une divergence indécidable sans rouvrir le plan pour la constater. Ce critère gouverne chacun des champs qui suivent, y compris ceux qu'une projection engendrée rend dérivables plus bas dans cette section : ce qu'il faut préserver est la décidabilité, jamais la non-duplication prise pour elle-même.

Champs attendus au minimum :

| Champ | Obligatoire | Pourquoi il n'est pas déjà porté par le plan |
|---|---|---|
| **Titre** | oui | Le plan porte un intitulé de tâche, pas un titre d'issue — c'est un artefact distinct, qui doit rester comparable littéralement au premier pour détecter une divergence. |
| **Tâche de plan** | oui | C'est l'unique lien vers le but, les dépendances, le périmètre d'écriture, la taille et les critères d'acceptation de la tâche — des champs déjà portés par `plan-de-travail.md`, qu'aucun ticket ne **recopie à la main** : ceux d'entre eux qui gagnent en actionnabilité à être projetés le sont par dérivation engendrée (corps engendré, plus bas), jamais par recopie. |
| **Estampille de provenance** | oui | `plan-de-travail.md` ne peut structurellement pas porter sa propre révision — un fichier ne connaît pas le commit qui le modifiera après lui. C'est ce qui rend décidable la péremption de tout ce que la zone engendrée projette. |
| **Libellés** | oui | Le plan ne classe pas ses tâches par jalon, nature de tranche ou nature de vérification — c'est une lecture transverse au plan, propre au suivi d'exécution, absente de sa structure (`plan-de-travail.md §4`, vue des épiques par jalon uniquement). |
| **État** | oui | Le plan ne porte aucune notion d'avancement — une tâche y est décrite une fois, indépendamment du moment où elle est prise en charge. |
| **Pris par** | non, renseigné à la prise | C'est le seul champ du dispositif entier qui nomme une personne — le plan ne peut structurellement pas le porter (`plan-de-travail.md §1`, règle (a)). |
| **Notes d'exécution** | non | Texte libre sans valeur normative, propre au déroulé d'une prise en charge donnée — rien de comparable n'existe dans le plan. |
| **Écart constaté** | non, mais important | C'est le chemin de retour vers le corpus : ce que le corpus dit et ce que le code impose, avec un renvoi vers le point à amender. Sans lui, une tâche menée jusqu'à sa Definition of Done s'écarte silencieusement de la conception, y compris sur un point encore `[À TRANCHER — J0]` que le ticket est chargé de clore. |

**Titre.** L'intitulé de la tâche `TB-nnn` du plan, recopié verbatim, précédé de son identifiant. Une divergence entre le titre du ticket et l'intitulé du plan est alors détectable par simple comparaison littérale des deux chaînes, sans avoir à ouvrir le plan pour la constater.

**Tâche de plan.** Le champ porte l'identifiant `TB-nnn` seul. Le ticket ne **recopie à la main** aucun des champs qu'il pointe (but, dépendances, périmètre d'écriture, taille, critères d'acceptation) : un champ recopié à deux endroits est un champ qui dérive dès que l'un des deux change sans l'autre — exactement la famille de défaut que [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md) a nommée pour le reste du corpus documentaire, et que ce modèle de ticket étend au report plan → ticket.

Recopier et dériver ne sont pas le même geste, et seul le premier est proscrit. **Recopier**, c'est écrire à la main, dans le ticket, une valeur qui vit déjà dans le plan : la copie a deux auteurs distincts dans le temps, sans rien qui la relie mécaniquement à sa source une fois écrite — elle dérive au sens défavorable du terme. **Dériver**, c'est engendrer une projection depuis la source, avec régénération : la projection garde une source unique, jamais une main qui la retape à côté. Ce document porte déjà cette distinction pour le lien plan → ticket, au §6 : « un renvoi qui pointe ne dérive pas quand sa cible bouge, une donnée recopiée doit être resynchronisée à chaque changement de la source ». Le corpus l'applique par ailleurs déjà au couple Gherkin → recette : [`cahier-strategie-test-et-recette.md §1 § Posture consommateur Gherkin`](../test/cahier-strategie-test-et-recette.md) pose une **clause de re-dérivation** — si le scénario source change, la ligne de recette qui en dérive doit être re-dérivée, et une divergence constatée y est un défaut à corriger dans le document dérivé, jamais une variante à documenter. La zone engendrée décrite ci-dessous applique la même clause au couple plan → ticket.

**Corps engendré, et ce qui ne l'est pas.** Un ticket porte, à la suite des champs ci-dessus, une zone **engendrée** : une projection régénérée depuis `plan-de-travail.md`, jamais retapée à la main. Un ticket qui ne la porte pas est à régénérer ; le §7 renvoie vers l'outillage concret de cette régénération. Une ligne de séparation textuelle marque la frontière dans le corps du ticket — au-dessus, du contenu engendré, régénérable, écrasé à chaque exécution ; en dessous, du contenu écrit à la main (notamment `Notes d'exécution` et `Écart constaté`), que la régénération ne touche jamais :

```
── engendré — régénérable, écrasé à chaque exécution — sous cette ligne : écrit à la main ──
```

**Ce que la zone engendrée porte.** Un champ y entre s'il **gagne en actionnabilité** par la projection — pas simplement en confort de lecture, ce qui rouvrirait la porte à la recopie. Projeté, `Dépend de TB-002, TB-003` devient une liste de renvois cliquables vers les tickets de ces tâches, dont l'état (§5) se lit d'un coup d'œil : c'est un accès de lecture rapide à l'état que la deuxième condition d'entrée du §2 (« ses dépendances sont closes ») exige de vérifier sur les tickets liés eux-mêmes (§2, autorité de vérification) — jamais la vérification elle-même —, et un accès que le plan, texte statique, ne donnera jamais aussi vite. `En conflit avec` gagne la même actionnabilité, pour la même raison : c'est le même accès de lecture à l'état que la troisième condition d'entrée du §2 (« aucun conflit n'est en cours ») exige de vérifier sur les tickets liés, par la même projection en renvois cliquables vers l'état des tickets en conflit.

Les autres champs de la fiche de tranche n'y entrent pas, faute du même gain. `But`, `Épique`, `Jalon`, `Périmètre d'écriture`, `Taille` et `Code de renvoi` sont des textes statiques du plan, sans état de ticket à consulter : les projeter retaperait leur valeur sans rien y gagner que le renvoi du champ `Tâche de plan` n'apporte déjà. `Critères d'acceptation` reste hors zone engendrée pour une raison symétrique : ce que sa quatrième condition d'entrée (§2) exige de vérifier est une propriété de sa cible dans le corpus de conception — la présence ou l'absence d'un marqueur de décision non prise — jamais un état de ticket ; l'ouvrir depuis le renvoi du plan ou depuis une projection revient au même geste, et la projection n'y ajoute rien.

La zone engendrée porte donc, au minimum : les renvois dérivés vers les tickets de `Dépend de` et de `En conflit avec`, et l'estampille de provenance.

**Estampille de provenance.** Porte, dans un seul champ, la révision de `plan-de-travail.md` dont dérive la zone engendrée et l'identifiant `TB-nnn` de la tranche — les deux valeurs ensemble, pour qu'un contrôle de fraîcheur se fasse sans rouvrir un autre champ du ticket. La révision se lit sur le suivi de versions que porte déjà le dépôt : `git log -1 --format=%H -- docs/gestion-projet/plan-de-travail.md` donne le commit courant du fichier ; l'estampille est ce commit au moment où la zone engendrée a été produite. Un écart entre l'estampille et ce commit courant est le signal de péremption — ce que le §6 précise en clause du gel assumé.

**Notes d'exécution et Écart constaté, répétés : un signal, jamais une rétrospective à part.** Un seul ticket portant l'un de ces champs ne dit rien au-delà de sa propre tâche. Plusieurs tickets qui en portent un du même ordre changent sa portée sans changer sa nature : le champ devient une **trace d'apprentissage** — un motif qui dépasse la tâche qui l'a écrit. Ce document ne crée aucun artefact pour la capter : la remontée suit une destination que le corpus porte déjà, selon ce que le motif concerne. Un point qui appelle un arbitrage remonte vers [`decisions-en-attente.md`](decisions-en-attente.md), qui les regroupe par décideur. Une dette ou un point ouvert propre à une source du corpus remonte vers le registre propriétaire de cette source, que [`decisions-en-attente.md § Registres propriétaires`](decisions-en-attente.md) énumère déjà. Le champ ne change pas de nature en se répétant sur plusieurs tickets — sa répétition désigne seulement où le regarder.

**Ce qu'un ticket fait du marqueur `TROU`.** Une tranche dont les critères d'acceptation portent ce marqueur n'est pas prête au sens de la quatrième condition du §2 — le ticket qui lui correspond reste à l'état non prêt (§5) jusqu'à ce que le corpus source referme le trou, indépendamment de toute autre condition par ailleurs satisfaite. Ce qu'un ticket ouvert sur une telle tranche doit faire : refléter cette absence plutôt que la masquer.

**À distinguer du marqueur `HORS MAILLE`** en périmètre d'écriture, qui ne signale aucun manque : la tranche ne vise simplement aucun module de code, et cela ne l'empêche ni d'être prête, ni d'être close.

---

## 2. Definition of ready

Une tâche du plan est **prête à être prise** quand, cumulativement, les conditions suivantes sont satisfaites. Chacune se vérifie en ouvrant un document, sans avoir à demander l'avis de quiconque — c'est le critère qui fixe la liste : une condition qu'on ne pourrait pas trancher seul n'a pas sa place ici.

**Autorité de vérification.** Ces cinq conditions se vérifient sur `plan-de-travail.md` et sur les tickets qu'il désigne, jamais sur la projection qu'un corps de ticket en porte (§1, zone engendrée) : cette projection est un accès de lecture, pas la source de la vérification. Une projection périmée — au sens de l'estampille de provenance et de sa clause du gel assumé (§6) — ne dispense donc jamais d'ouvrir la source qu'elle projette. Le geste de régénération lui-même a un propriétaire : un ticket dont l'estampille de provenance ne porte pas la révision courante de `plan-de-travail.md` est régénéré avant d'être pris — l'outillage concret de ce geste reste à trancher à J0 (§7).

1. **La tâche existe dans le plan.** L'identifiant `TB-nnn` désigne une fiche de tâche réelle dans [`plan-de-travail.md`](plan-de-travail.md) — vérifiable en l'y ouvrant.
2. **Ses dépendances sont closes.** Chaque tâche nommée dans le champ `Dépend de` de `TB-nnn` correspond à un ticket à l'état terminal du cycle de vie décrit au §5 — vérifiable en ouvrant les tickets liés.
3. **Aucun conflit n'est en cours.** Aucune tâche nommée dans le champ `En conflit avec` de `TB-nnn` ne correspond à un ticket aux états « Pris » ou « En cours » (§5) — vérifiable en ouvrant les tickets liés.
4. **Un critère d'acceptation existe, sa cible aussi, et cette cible tranche.** Le champ `Critères d'acceptation` de `TB-nnn` porte au moins un renvoi dont la cible existe réellement **et dont la section citée ne porte pas, pour le point en cause, un marqueur de décision non prise** — vérifiable en ouvrant cette cible et en y cherchant le marqueur, dont [`guide-conventions-et-dod.md § Convention de balisage`](guide-conventions-et-dod.md) fixe la forme. Une valeur `TROU` ne constitue pas un renvoi et ne satisfait donc pas cette condition.
   *Sans cette seconde exigence, un renvoi vers un registre de dettes passerait l'épreuve d'existence : la cible existe, mais elle dit que le point reste à décider.*
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
- **Terminé** — la Definition of Done est satisfaite pour cette tâche ; le ticket se ferme. Ce qui ferme un ticket est ce renvoi, jamais une liste de critères réécrite ici. **Le champ `Écart constaté` (§1), s'il porte un contenu, est remonté vers sa destination avant que le ticket ne passe à cet état — jamais après.** Le §6 refuse que le découpage vive dans la seule issue, au motif « qu'une issue serait fermée ou recréée, et une personne reprenant le projet depuis ce dépôt ne le retrouverait plus » ; le même raisonnement s'applique ici. Un écart constaté qui ne vivrait que dans un ticket refermé serait perdu de la même façon — et il porte, à la différence du découpage, la substance la plus précieuse du dispositif : le point où la conception et le code ont divergé.

**Pour les tickets de nature « Anomalie »** (§3), le cycle de vie et l'échelle de sévérité sont ceux de [`cahier-strategie-test-et-recette.md §7`](../test/cahier-strategie-test-et-recette.md) — renvoi seul, non redit ici. Ils ne suivent pas le cycle ci-dessus, propre à une tâche de développement.

---

## 6. Où naissent les tickets, et ce qui fait foi

Les tickets sont ouverts dans les issues du dépôt du projet — le dépôt distant dont ce clone est issu.

**Le plan versionné est la source de vérité ; les issues en sont le report — jamais l'inverse.** Sans cette hiérarchie, le découpage en tâches disparaîtrait du clone dès lors qu'une issue serait fermée ou recréée, et une personne reprenant le projet depuis ce dépôt ne le retrouverait plus.

**Le plan ne porte aucun numéro d'issue.** Le lien entre une tâche et son ticket se fait par l'identifiant `TB-nnn` placé en tête du titre du ticket (§1, champ Titre), ce qui le rend retrouvable par recherche dans les deux sens. Un numéro d'issue inscrit dans le plan serait une donnée que le plan ne possède pas nativement — à resynchroniser à chaque création, fermeture ou recréation de ticket. C'est exactement la famille de dérive que [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md) a déjà nommée et corrigée pour le reste du corpus documentaire : un renvoi qui pointe ne dérive pas quand sa cible bouge, une donnée recopiée doit être resynchronisée à chaque changement de la source.

**La clause du gel assumé.** L'estampille de provenance (§1) est, pour la première fois dans tout ce dispositif, une donnée délibérément **gelée** : elle ne se resynchronise jamais toute seule. C'est une inversion assumée du raisonnement tenu au paragraphe précédent, qui refuse un numéro d'issue dans le plan précisément parce qu'il « serait une donnée que le plan ne possède pas nativement — à resynchroniser à chaque création, fermeture ou recréation de ticket ». Une estampille qui se resynchroniserait à chaque changement du plan ne dirait plus rien : elle porterait toujours la révision courante, jamais celle dont la zone engendrée dérive réellement. **Le gel est le point, et l'écart entre l'estampille et la révision courante du plan est le signal** — son absence signifiant que la zone engendrée reste à jour, sa présence qu'elle est à régénérer. Sans cette clause, §1 et §6 se contrediraient : une règle qui refuse ici la donnée non nativement portée et qui prescrit là de la geler serait la même règle qui se dément elle-même, et un relecteur aurait raison de déclarer l'estampille non conforme.

**Limite assumée de la granularité.** Le signal porte sur la révision du fichier `plan-de-travail.md` entier, pas sur celle de la seule tranche `TB-nnn` référencée : toute modification touchant une autre tranche du même fichier fait apparaître le même écart, sans rapport avec la zone engendrée du ticket. Ce sur-déclenchement est sans danger — il pousse à régénérer une zone engendrée qui n'en avait pas besoin, jamais à manquer une péremption réelle — et il est assumé comme tel, faute d'un suivi de révision plus fin que le fichier.

**La clause d'asymétrie de direction.** Aucun document versionné de ce corpus ne porte de renvoi sortant vers le tracker du dépôt — ni URL d'issue, ni URL de commit, ni URL de branche ; c'est déjà la règle posée au paragraphe précédent pour le numéro d'issue, qui n'en est qu'un cas particulier. Cette clôture rend le corpus vérifiable dans un clone : le vérificateur de liens et d'ancres du corpus (outillage concret, §7) ne peut prétendre à l'exhaustivité sur les liens et les ancres que parce qu'aucun renvoi versionné ne sort du périmètre qu'il balaie — un lien externe mort n'est, pour cet outil, pas un lien cassé. Rien d'écrit ne l'imposait jusqu'ici : un contributeur ajoutant demain une URL d'issue dans un document versionné n'enfreindrait aucune règle explicite. Cette clause l'énonce : le corpus ne porte, et ne doit porter, aucun renvoi sortant vers le tracker. Le renvoi va dans l'autre sens — du ticket vers le plan, jamais du plan vers le ticket — et c'est cette asymétrie qui met la charge de la fraîcheur sur le ticket : lui seul pointe vers une source susceptible de bouger sans lui, et c'est exactement ce que l'estampille de provenance (§1) rend décidable.

---

## 7. Ce que cette méthode ne tranche pas

- **La convention de message de commit et de nommage de branche.** [`guide-conventions-et-dod.md §5`](guide-conventions-et-dod.md) la laisse explicitement `[À TRANCHER — J0]` : « Le corpus de conception ne définit aucun format de message de commit ni de convention de nommage de branche. […] un candidat courant est Conventional Commits, à ratifier — ce guide ne le tranche pas. » Le plan porte la tâche qui clôt ce point : **TB-012 — Arrêter la convention de commit et de branche** (`plan-de-travail.md §5`). Ce document ne tranche rien à sa place.
- **Le barème de priorité des tickets.** Interdit à cette méthode par une non-décision explicite du corpus (§3 ci-dessus) ; marqué `[À TRANCHER — J0]`, sans valeur proposée.
- **L'outillage concret de la taxonomie et du modèle dans le dépôt du projet** — libellés réels, modèle d'issue, champs personnalisés. Aucune source du corpus de conception ne fixe ce point ; `[À TRANCHER — J0]`, à la charge de l'équipe de build au moment d'ouvrir le dépôt de code, dans le même esprit que la note de portée de [`guide-conventions-et-dod.md §0`](guide-conventions-et-dod.md) sur l'instrumentation outillée. **Produire** l'estampille de provenance (§1), au moment où un outil engendre ou régénère le corps d'un ticket, n'entre pas dans ce point : la révision s'y lit localement, sans aucun accès au tracker. Ce qui y entre est le **contrôle** d'un ticket déjà ouvert — comparer l'estampille qu'il porte à la révision courante de `plan-de-travail.md` — puisque ce ticket vit dans le tracker, pas dans ce dépôt : lire l'estampille déjà écrite exige alors l'API du tracker, quand la révision courante du plan, elle, se lit sans elle.
- **L'outillage qui lit et vérifie le corpus documentaire.** `plan-de-travail.md` déclare en bandeau ce qu'il fait : « il prend la séquence de jalons J0-J3 et […] la décompose jusqu'à la tranche ». Un outil qui lit et vérifie le corpus documentaire ne relève pas de cette séquence : il ne reçoit donc pas de tranche, et la maille de désignation (`plan-de-travail.md §2`) n'a pas à le désigner.

---

## 8. Itération

**Le problème que cette section résout.** Le corpus interdit toute notion temporelle : [`plan-de-travail.md`](plan-de-travail.md), en bandeau, classe hors périmètre « durée, effectif, affectation nominative — aucun de ces éléments n'est un fait dérivable du corpus de conception, qui ne porte ni charge ni ordonnancement temporel propre ». Une itération semblerait donc incompatible avec ce corpus. Elle ne l'est pas : la couture existe déjà, au §1 de ce document — « Le plan de travail porte ce qui est invariant à l'effectif qui l'exécute — but, dépendances, périmètre d'écriture, taille, critères d'acceptation […] Le ticket porte ce qui varie avec cet effectif — l'état d'avancement, qui l'a pris, ce qui a été constaté en le faisant. » Une itération varie avec l'effectif qui l'exécute et le moment où il l'exécute : elle appartient donc à ce que porte le ticket, jamais à ce que porte le plan.

**Ce qu'une itération est.** Un sous-ensemble **choisi** des tranches ouvrables du palier courant ([`plan-de-travail.md §9`](plan-de-travail.md)) : sa largeur est un choix de l'effectif, jamais une conséquence automatique de celle du palier. Sur un palier ouvrant plusieurs tranches à la fois, une itération qui épouserait systématiquement le palier entier n'aurait aucune fonction de cadencement propre — ce serait une seule et même traite, avec un unique événement de clôture, sous un autre nom. Sur un palier n'ouvrant qu'une seule tranche, l'itération et le palier coïncident nécessairement ; ce n'est pas une exception à la règle, puisque le choix reste entier — il n'a simplement rien d'autre à choisir.

**Ce qui la ferme.** Une itération est close quand chacune des tranches qu'elle contient est à l'état terminal du cycle de vie décrit au §5 — Cycle de vie d'un ticket — de ce document, jamais avant.

**Pourquoi ce critère n'en énonce pas un en propre.** Le corpus porte déjà trois autorités de complétude, chacune propriétaire d'un grain distinct : la Definition of Done de code ([`guide-conventions-et-dod.md §9`](guide-conventions-et-dod.md)), la Definition of Done de test ([`cahier-strategie-test-et-recette.md §8`](../test/cahier-strategie-test-et-recette.md)) et les critères de sortie de jalon — propriétaires conjoints [`roadmap-entree-build.md §3`](roadmap-entree-build.md) et [`cahier-strategie-test-et-recette.md §9 — Critères de sortie par jalon`](../test/cahier-strategie-test-et-recette.md). Énoncer ici un critère de clôture propre en ferait une quatrième, concurrente des trois autres sur le sort d'une même tranche. Ce n'est pas le cas : fermer une itération ne fait que constater, tranche par tranche, un état déjà décidé ailleurs. Le corpus déclare d'ailleurs déjà cette frontière entre grains : « la DoD de code de ce guide s'applique à chaque incrément livré, quelle que soit sa taille. Les critères de sortie de jalon opèrent à une granularité supérieure — ils agrègent plusieurs incréments et des vérifications propres au jalon (recette fonctionnelle, points de décision hors intégration continue). Un incrément peut satisfaire la DoD de code de ce guide sans que le jalon auquel il appartient satisfasse encore ses propres critères de sortie » ([`guide-conventions-et-dod.md §9 § Frontière explicite`](guide-conventions-et-dod.md)). Une itération se loge à un grain intermédiaire, entre l'incrément et le jalon, sans ajouter de critère au-dessus de ceux qui existent déjà.

**Ce qui fixe la largeur du sous-ensemble.** Combien de tranches entrent dans une itération est une propriété de l'effectif qui l'exécute, pas du corpus : le plan ne porte ni charge ni durée, et ne peut donc rien dériver sur ce point. C'est, par construction, la seule grandeur de tout ce dispositif qu'une personne seule peut légitimement fixer par simple observation de ce qu'elle peut effectivement mener de front — et elle vit côté ticket, hors corpus, par la partition même que pose le §1. C'est précisément ce qui rend l'itération admissible sans introduire de notion temporelle : elle ne mesure rien que le corpus devrait mesurer à sa place.

**Ce qui reste dehors, nommément.** Aucune durée, aucune date, aucune cadence, aucune vélocité, aucune estimation. Le calendrier d'une itération — ses dates, son état courant — vit dans le support de suivi, jamais dans ce document.

**Cérémonies.** Deux traces écrites par itération, aucune réunion :
- une trace d'**ouverture** : les tranches prises, et la source qui les a choisies au sens du dispositif de départage de [`plan-de-travail.md §1`](plan-de-travail.md) — y compris quand ce dispositif ne départage plus, auquel cas c'est cette absence, déclarée comme telle, qui est inscrite — renvoi seul, jamais recopié ici ;
- une trace de **clôture** : ce qui a été appris, et tout écart constaté à remonter au corpus.

Ces deux traces s'écrivent dans le registre de trajectoire (§9) : l'ouverture y pose la ligne de l'itération, la clôture la complète.

**Ce qu'une revue de soi par soi ne produit pas.** Une revue produit un second regard ; se relire seul n'en produit pas. Mais la fonction de porte que tiendrait une telle revue ne manque pas ici : elle est déjà tenue, le §5 routant la clôture d'une tranche par sa Definition of Done, jamais par cette section. Ce qui serait réellement perdu sans trace de clôture est un autre moment : celui où quelqu'un se demande si ce qui a été fait est bien ce que le corpus demandait. Le corpus a déjà un repreneur nommé pour ce moment — le champ `Écart constaté` (§1), que ce document qualifie de « chemin de retour vers le corpus ». La trace de clôture n'invente rien : elle rappelle d'y écrire.

**Désambiguïsation.** Le mot « itération » porte, ailleurs dans le corpus versionné, des sens étrangers à celui posé ici — notamment une itération antérieure de la conception elle-même (`ADR-002-tout-est-document-gouvernance.md`, `ADR-009-fk-campaign-owner.md`) et le paramètre cryptographique d'un algorithme de hachage de mot de passe (`dossier-securite.md`, `config-securite-migration.md`, `ADR-015-securite-authentification-mvp.md`, `cahier-specifications-techniques.md`), sans risque de confusion avec celui posé ici. Un sens expose en revanche à une confusion réelle, dans un document normatif de la même famille que celui-ci : [`guide-conventions-et-dod.md`](guide-conventions-et-dod.md) écrit qu'« un défaut a ainsi survécu une itération de plus » — une passe d'une boucle de correction, jamais un conteneur de tranches. Une itération au sens de cette section ne se répète pas sur un défaut : elle regroupe des tranches, se clôt une fois, et n'est jamais rejouée.

---

## 9. Registre de trajectoire

**Pourquoi il existe.** La composition d'une itération (§8) est une jointure de deux sources invariantes à l'effectif — le palier courant et le dispositif de départage de [`plan-de-travail.md §1`](plan-de-travail.md) — et d'une troisième qui ne l'est pas : le choix effectivement fait par l'effectif qui exécute. Cette troisième source ne vit dans aucun fichier versionné ; son conteneur est le support de suivi (§8). Si rien n'est écrit ici, une personne qui reprend le projet depuis le seul clone dispose du découpage — et de rien d'autre : ni quelles tranches ont été prises, dans quel ordre, ni pourquoi.

**Le corpus traite déjà cette classe de perte comme inacceptable.** [`plan-de-travail.md §11 — Identifiants retirés`](plan-de-travail.md) tient un tableau des identifiants retirés précisément pour qu'une personne reprenant le projet ne doive jamais deviner ce qu'un identifiant disparu recouvrait. Ce registre applique le même principe à la trajectoire d'exécution : ce qui disparaît du support de suivi survit dans ce dépôt.

**Ce que ce registre est.** Un registre factuel, en ajout seul : sa seule propriété est de ne jamais réécrire une ligne déjà posée. Une ligne par itération, portant sa composition, la source qui l'a choisie — y compris l'absence de source, déclarée comme telle, quand le dispositif de départage de [`plan-de-travail.md §1`](plan-de-travail.md) ne départage plus —, et ce qui a été appris. Aucune règle, aucune norme.

**Son autorité.** Chacune des autres sections de ce document se referme sur la clause posée en bandeau : en cas de conflit, la source citée fait foi, ce document n'étant pas une nouvelle autorité de corpus. Cette section ne peut pas porter la même clause telle quelle : elle est la seule source du fait qu'elle enregistre — nulle autre section de ce document, ni aucune autre source du corpus, ne dit quelles tranches ont été prises, dans quel ordre. Sa clause tient donc en deux moitiés distinctes : ce registre fait foi, seul, pour le fait d'exécution qu'il porte ; il ne fait foi pour aucune règle. En cas de conflit entre une de ses lignes et [`plan-de-travail.md`](plan-de-travail.md), le plan fait foi pour la règle — la ligne, elle, reste le fait constaté.

---

## 10. Renvois

- Décomposition en tâches, dépendances, périmètre d'écriture, taille : [`plan-de-travail.md`](plan-de-travail.md)
- Entrée par fonctionnalité produit : [`guide-lecture-par-fonctionnalite.md`](guide-lecture-par-fonctionnalite.md)
- Conventions de code et Definition of Done : [`guide-conventions-et-dod.md`](guide-conventions-et-dod.md)
- Conventions de maintenance du corpus documentaire (famille de dérive par recopie) : [`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md)
- Séquence de jalons, préalables bloquants, critères de sortie : [`roadmap-entree-build.md`](roadmap-entree-build.md)
- Niveaux de test, axes transverses, cycle de vie d'anomalie et sévérité : [`cahier-strategie-test-et-recette.md`](../test/cahier-strategie-test-et-recette.md)
- Granularité des projets .NET et périmètre du mode local TypeScript : [`structure-projets.md`](../architecture/structure-projets.md)
- Ordre de lecture pour une personne qui reprend le projet : [`README.md`, § Par où commencer](../../README.md#par-où-commencer)

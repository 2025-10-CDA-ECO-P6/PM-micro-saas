# Revue produit du plan de travail — Haversack

| Champ | Valeur |
|---|---|
| Statut | Revue datée du 2026-09-03 — **non normative**, périmée dès que ses recommandations sont arbitrées |
| Autorité | **aucune** — cette revue ne redéfinit ni jalon, ni épique, ni tâche, ni priorité, ni critère d'acceptation |
| Audience | la personne qui décide de changer ou de ne pas changer le découpage du plan avant l'entrée en construction |
| Objet | juger la **pertinence** de la décomposition du plan de travail, et instruire son redécoupage en tranches verticales |
| Sources | `docs/gestion-projet/plan-de-travail.md`, `docs/gestion-projet/methode-de-ticket.md`, `docs/gestion-projet/guide-lecture-par-fonctionnalite.md`, `docs/gestion-projet/roadmap-entree-build.md`, `docs/conception/besoin/vision/vision-produit.md`, `docs/conception/besoin/vision/moscow.md`, `docs/conception/domain/*.md`, `docs/test/cahier-strategie-test-et-recette.md`, `docs/architecture/structure-projets.md` |

---

## Bandeau de cadrage — ce que cette revue fait et ne fait pas

Ce document **juge** un découpage déjà écrit et **propose** son remplacement. Il ne modifie rien : le plan de travail
reste, à la date de cette revue, inchangé et seul opposable. Il ne redéfinit ni le périmètre MoSCoW
([`vision/moscow.md`](../conception/besoin/vision/moscow.md), seule autorité de priorisation), ni le phasage J0-J3
([`roadmap-entree-build.md §1-2`](roadmap-entree-build.md)), ni les critères de sortie de jalon, ni l'ordre des
user stories à l'intérieur d'une epic.

**En cas de conflit entre cette revue et une source citée, la source citée fait foi.**

**Statut particulier des nombres portés par ce document.** Le corpus s'interdit d'écrire un décompte dont la donnée
est possédée ailleurs, parce qu'un nombre recopié dérive dès que sa source bouge
([`guide-conventions-et-dod.md §8`](guide-conventions-et-dod.md)). Cette revue porte pourtant des nombres, et
l'exception est assumée : ce ne sont pas des données recopiées d'une source, ce sont des **mesures prises sur le plan
à une date**, chacune accompagnée de la commande littérale qui l'établit. Elles ne sont pas à maintenir — elles sont
à **remesurer** en relançant la commande. C'est aussi pourquoi ce document est daté et périssable : passé l'arbitrage,
ses mesures décrivent un état qui n'existe plus.

**Ce que cette revue n'est pas** : un troisième artefact d'ordonnancement. Le dossier en porte déjà deux à des grains
distincts — [`roadmap-entree-build.md`](roadmap-entree-build.md) ordonnance les jalons,
[`plan-de-travail.md`](plan-de-travail.md) décompose à l'intérieur — plus deux documents nommés « roadmap »
([README du dossier](README.md) les désambiguïse). Cette revue n'ordonnance rien et ne décompose rien : elle mesure,
conclut et recommande.

---

## 1. Comment cette revue a été établie

Le dépôt n'a ni build, ni suite de tests, ni analyseur de code : toute vérification y est mécanique et construite
pour l'occasion. Chaque constat de la §2 porte la commande qui l'établit, exécutable telle quelle depuis la racine
du dépôt.

**État de référence du corpus, mesuré au début et à la fin de cette revue** : zéro lien relatif à cible inexistante,
zéro renvoi à ancre inexistante, zéro fichier orphelin, zéro bloc de code déséquilibré, sur 205 fichiers markdown et
1522 liens relatifs. L'ajout de ce document ne dégrade aucun de ces quatre compteurs.

**Deux instruments ont dû être corrigés en cours de route, et le dire fait partie du résultat.**

- Un premier relevé des tâches livrant une surface visible par un utilisateur cherchait le mot `fiche` dans le champ
  `Périmètre d'écriture`. Il en trouvait dans le **texte explicatif** des périmètres marqués `TROU`, qui écrivent
  « ni fiche d'écran de wireframe » pour dire précisément l'inverse. La mesure fausse annonçait la première surface
  visible au maillon 15 ; la mesure corrigée l'annonce au maillon 18. La correction consiste à exclure d'abord tout
  périmètre portant `TROU`.
- Un relevé de la partition des cas de recette par user story rattachait `CR-UC02-11` à `US-01-01`. Vérification faite,
  ce cas est marqué `*(retiré)*` dans le cahier et sa colonne `Source` porte une règle métier elle-même retirée. Ce
  n'était pas une anomalie du corpus mais une lecture trop naïve de l'instrument — le corpus, lui, dit exactement ce
  qu'il en est.

---

## 2. Ce que la mesure établit

### 2.1 Le graphe sature à trois — au-delà, une personne de plus n'ouvre rien

**Mesure.** Simulation d'ordonnancement respectant à la fois les dépendances techniques déclarées et les conflits
d'écriture déclarés, à effectif croissant. Les deux ensembles de dépendances énoncés en prose plutôt qu'en liste
(TB-024, TB-055) sont reconstitués depuis leur énoncé.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
dep, conf = {}, {}
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    dep[i] = set(re.findall(r'TB-\d{3}', ch.get('Dépend de', '')))
    conf[i] = set(re.findall(r'TB-\d{3}', ch.get('En conflit avec', '')))
dep['TB-024'] |= {f'TB-{n:03d}' for n in (13,14,15,16,17,18,19,20,21,22,23)}
dep['TB-055'] = {k for k in dep if k != 'TB-055' and int(k[3:]) >= 13}
def simule(effectif, avec_conflits=True):
    reste, clos, rondes, occ = set(dep), set(), 0, []
    while reste:
        rondes += 1
        pris = []
        for k in sorted(k for k in reste if dep[k] <= clos):
            if len(pris) >= effectif: break
            if not avec_conflits or all(k not in conf[p] and p not in conf[k] for p in pris):
                pris.append(k)
        occ.append(len(pris)); clos |= set(pris); reste -= set(pris)
    return rondes, sum(occ) / (rondes * effectif)
for e in (1, 2, 3, 5, 10):
    r, o = simule(e)
    print(f"effectif {e:2} : {r:2} etapes, occupation moyenne {o*100:5.1f} %")
PY
```

Sortie brute :

```
effectif  1 : 55 etapes, occupation moyenne 100.0 %
effectif  2 : 32 etapes, occupation moyenne  85.9 %
effectif  3 : 27 etapes, occupation moyenne  67.9 %
effectif  5 : 27 etapes, occupation moyenne  40.7 %
effectif 10 : 27 etapes, occupation moyenne  20.4 %
```

**Conclusion.** La propriété que le plan s'impose — « une personne de plus qu'il n'y a de tâches ouvrables attend la
levée d'une dépendance ; elle ne redécoupe pas le lot » ([`plan-de-travail.md §1`](plan-de-travail.md), règle de
lecture pour un effectif variable) — est **formellement tenue** : rien dans le plan n'oblige à le redécouper quand
l'effectif change. Elle est en revanche **pratiquement vide au-delà de trois** : de 3 à 10, le nombre d'étapes ne
bouge plus du tout, et l'occupation moyenne tombe de 67,9 % à 20,4 %. La quatrième personne n'ouvre aucune tâche que
les trois premières n'auraient pas ouverte.

**Coût de l'inaction.** Le plan reste juste, mais il ne dit pas ce qu'il ne peut pas absorber. Une décision d'engager
plus de trois personnes sur ce périmètre serait prise en croyant que le plan la supporte, alors qu'il la sature.

**Recommandation.** Ne pas corriger la règle, qui est bonne : lui ajouter le fait mesuré. Le plan gagnerait, dans sa
section 1, une phrase disant que **la largeur du graphe est une propriété mesurable du document et non une promesse**,
avec la commande pour la remesurer après tout redécoupage. C'est la seule façon de rendre la propriété falsifiable
plutôt que déclarative.

---

### 2.2 La séquentialité ne vient pas de la maille d'écriture — elle vient de l'empilement des couches

**Mesure.** Même simulation, en supprimant **tous** les conflits d'écriture (dépendances seules), puis mesure de la
chaîne série la plus longue restreinte aux seules tâches de socle et d'agrégat de domaine.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
dep, per = {}, {}
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    dep[i] = set(re.findall(r'TB-\d{3}', ch.get('Dépend de', '')))
    per[i] = ch.get("Périmètre d'écriture", '')
memo = {}
def prof(n, ens):
    c = (n, id(ens))
    if c not in memo: memo[c] = 1 + max((prof(d, ens) for d in dep.get(n, ()) if d in ens), default=0)
    return memo[c]
tout = set(dep)
print("chaine serie la plus longue, plan entier :", max(prof(k, tout) for k in tout))
NS = {'IdentityAccess','SpaceManagement','ContentLibrary','SessionConduct','SharedKernel'}
socle = {'TB-001','TB-002','TB-003','TB-004','TB-005'}
dom = {k for k in dep if 'TROU' not in per[k]
       and {x.strip() for x in per[k].split(';') if x.strip()} <= NS} | socle
print("chaine restreinte au socle et aux agregats de domaine :", max(prof(k, dom) for k in dom))
PY
```

Sortie brute :

```
chaine serie la plus longue, plan entier : 22
chaine restreinte au socle et aux agregats de domaine : 11
```

Et, en supprimant tous les conflits d'écriture dans la simulation de la §2.1 (paramètre `avec_conflits=False`) :
27 étapes deviennent 23 à effectif 5.

**Conclusion.** Trois faits se déduisent, et ils commandent tout le reste de cette revue.

1. **La maille d'écriture coûte peu.** Supprimer *tous* les conflits — hypothèse maximaliste, une maille infiniment
   fine — ne fait passer que de 27 à 23 étapes. Affiner la maille sans rien changer d'autre aurait donc rapporté
   moins de 15 %. Le plan a raison d'écrire que sa maille conservatrice est « un coût de délai, jamais un risque de
   collision » ([`plan-de-travail.md §2`](plan-de-travail.md)) : ce coût est réel, et il est petit.
2. **La chaîne du domaine est irréductible et courte.** Espace, puis dossier, puis document, puis type de document,
   puis scénario et session, puis configuration de vue : onze maillons, dont cinq de socle. Ce sont de vraies
   dépendances métier — un dossier appartient à un espace, un document à un dossier. Aucun découpage ne les supprime.
3. **L'autre moitié de la chaîne est produite par le découpage lui-même.** Onze maillons sur vingt-deux ne viennent
   ni du socle, ni du métier : ils viennent de l'empilement contrats Application → object stores → services d'accès →
   châssis → écrans, où chaque étage attend l'achèvement de l'étage entier sous lui.

La réponse à la question « la cause est-elle la décomposition ou le produit lui-même ? » est donc : **moitié-moitié,
et c'est la moitié imputable à la décomposition qui est attaquable.**

**Coût de l'inaction.** Continuer avec le découpage par couche revient à porter une chaîne deux fois plus longue que
celle du domaine, sans que ce doublement serve à rien d'autre qu'à la commodité de désignation du périmètre d'écriture.

**Recommandation.** C'est exactement ce que le redécoupage en tranches verticales attaque (§3) : une tranche ne
dépend plus de l'achèvement de l'étage sous elle, mais seulement du chemin mince qu'elle traverse. La cible n'est pas
une maille plus fine, c'est une **chaîne plus courte**.

---

### 2.3 Huit tâches de domaine ne sont pas clôturables sur leurs propres critères d'acceptation

C'est le constat le plus grave de cette revue.

**Mesure.** Tâches dont le périmètre d'écriture ne contient que des namespaces du Domaine — donc ni écran, ni object
store, ni couche Application — et dont les critères d'acceptation citent malgré tout un scénario Gherkin ou un cas
de recette, c'est-à-dire un comportement observable par un utilisateur.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
NS = {'IdentityAccess','SpaceManagement','ContentLibrary','SessionConduct','SharedKernel'}
n = 0
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    mods = {x.strip() for x in ch.get("Périmètre d'écriture", '').split(';') if x.strip()}
    if not mods or not mods <= NS: continue
    acc = ch.get("Critères d'acceptation", '')
    g = re.findall(r'US-\d{2}-\d{2}\s*§"', acc)
    cr = re.findall(r'CR-UC\d{2}-\d{2}', acc)
    if g or cr:
        n += 1
        print(f"  {i} {sorted(mods)} : {len(g)} scenario(s) Gherkin, {len(cr)} borne(s) de cas de recette")
print("total :", n)
PY
```

Sortie brute :

```
  TB-013 ['SpaceManagement'] : 2 scenario(s) Gherkin, 2 borne(s) de cas de recette
  TB-014 ['ContentLibrary'] : 3 scenario(s) Gherkin, 2 borne(s) de cas de recette
  TB-015 ['ContentLibrary'] : 4 scenario(s) Gherkin, 2 borne(s) de cas de recette
  TB-017 ['ContentLibrary'] : 1 scenario(s) Gherkin, 0 borne(s) de cas de recette
  TB-018 ['ContentLibrary'] : 4 scenario(s) Gherkin, 2 borne(s) de cas de recette
  TB-019 ['SessionConduct'] : 3 scenario(s) Gherkin, 2 borne(s) de cas de recette
  TB-020 ['SessionConduct'] : 1 scenario(s) Gherkin, 0 borne(s) de cas de recette
  TB-021 ['ContentLibrary', 'SessionConduct'] : 2 scenario(s) Gherkin, 0 borne(s) de cas de recette
total : 8
```

**Conclusion.** Les critères d'acceptation produit vivent dans les scénarios Gherkin des user stories, et un scénario
Gherkin décrit un comportement **observable par un utilisateur**. Or ces huit tâches n'écrivent que dans le Domaine :
aucun écran, aucun store, aucun chemin d'exécution atteignable. Prenons TB-014, dont le périmètre est le seul
namespace `ContentLibrary` : elle cite `US-05-01 §"Le MJ crée un dossier avec un nom valide"` et la plage
`CR-UC05-01 → CR-UC05-12`. Le cas `CR-UC05-01` décrit un MJ qui crée un dossier depuis une interface. Personne ne
peut prouver ce cas en ayant seulement écrit l'agrégat `Folder`.

Ces huit tâches sont donc **faisables mais non clôturables**. C'est précisément le défaut que la contrainte
« chaque tâche est fermable indépendamment, ses critères d'acceptation sont dans le corpus » vise à interdire — et il
est présent, non parce que les critères manquent, mais parce qu'ils sont **d'une autre nature que le périmètre**.

Il ne s'agit pas d'un défaut du corpus : les scénarios Gherkin sont à leur place et sont la bonne source. Il s'agit
d'un défaut du découpage, qui attache un critère vertical à une tâche horizontale.

**Coût de l'inaction.** Une personne qui prend TB-014 la termine sans pouvoir la clore, et n'a que deux issues :
appeler quelqu'un pour arbitrer ce qui compte comme « fait » — ce que la condition d'entrée interdit expressément —
ou clore sur une preuve qu'elle se donne elle-même, c'est-à-dire un test unitaire de l'agrégat, qui n'est pas le
critère écrit. Dans les deux cas la Definition of Done devient déclarative. Sur les huit tâches concernées, ce sont
toutes les fondations métier de J1.

**Recommandation.** C'est la justification première du redécoupage vertical : une tranche livre le comportement
complet que son critère décrit, donc son critère devient atteignable. Aucun autre correctif ne referme ce défaut —
adoucir les critères des tâches de domaine reviendrait à réénoncer une substance dont les user stories sont l'autorité.

---

### 2.4 Quatre plages de critères sont engagées deux fois, et une englobe un cas retiré

**Mesure.** Pour chaque plage de cas de recette citée dans le plan, nombre de tâches qui la citent, avec la nature de
leur périmètre. Puis contrôle du nombre réel de cas dans le cahier.

```bash
python3 - <<'PY'
import re, collections
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
cite = collections.defaultdict(list)
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    per, acc = ch.get("Périmètre d'écriture", ''), ch.get("Critères d'acceptation", '')
    nat = 'ecran' if (re.search(r'fiche\s+`', per) and 'TROU' not in per) else 'domaine ou autre'
    for uc, fin in re.findall(r'CR-UC(\d{2})-\d{2}\s*→\s*CR-UC\d{2}-(\d{2})', acc):
        cite[f'CR-UC{uc} -> {fin}'].append((i, nat))
for k, v in sorted(cite.items()):
    print(f"  {k} : {len(v)} tache(s) {v}")
PY
```

```bash
python3 -c "
import re, collections
t = open('docs/test/cahier-strategie-test-et-recette.md', encoding='utf-8').read()
tot, act = collections.Counter(), collections.Counter()
for l in t.splitlines():
    m = re.match(r'^\|\s*CR-UC(\d{2})-\d{2}\s*\|(.*)', l)
    if m:
        tot[m.group(1)] += 1
        if 'retiré' not in m.group(2)[:40]: act[m.group(1)] += 1
for k in sorted(tot):
    if tot[k] != act[k]: print(f'CR-UC{k} : etendue {tot[k]}, actifs {act[k]}')
"
```

Sortie brute :

```
  CR-UC02 -> 13 : 1 tache(s) [('TB-013', 'domaine ou autre')]
  CR-UC03 -> 21 : 1 tache(s) [('TB-018', 'domaine ou autre')]
  CR-UC04 -> 21 : 2 tache(s) [('TB-015', 'domaine ou autre'), ('TB-047', 'ecran')]
  CR-UC05 -> 12 : 2 tache(s) [('TB-014', 'domaine ou autre'), ('TB-046', 'ecran')]
  CR-UC06 -> 29 : 2 tache(s) [('TB-019', 'domaine ou autre'), ('TB-051', 'ecran')]
  CR-UC07 -> 09 : 2 tache(s) [('TB-022', 'domaine ou autre'), ('TB-052', 'ecran')]
  CR-UC14 -> 13 : 1 tache(s) [('TB-049', 'ecran')]
```

```
CR-UC01 : etendue 20, actifs 19
CR-UC02 : etendue 13, actifs 12
CR-UC11 : etendue 18, actifs 17
```

**Conclusion.** Deux défauts, tous deux mécaniques.

Le premier : quatre plages sur sept sont citées par exactement **deux** tâches, une de domaine et une d'écran. Or la
règle de comptage du plan est explicite — « citer `RB-02-01 → RB-02-20` engage réellement vingt preuves distinctes à
la clôture de la tâche, pas une » ([`plan-de-travail.md §3`](plan-de-travail.md)). La même preuve est donc engagée
deux fois, et **aucune des deux tâches ne peut la produire seule** : c'est la §2.3 vue depuis l'échelle de taille.
L'échelle est ainsi gonflée aux deux extrémités de chaque paire, ce qui explique une part de la proportion de tâches
`L` — 19 sur 55, mesurable par la commande de la §2.5 du plan.

Le second : la plage `CR-UC02-01 → CR-UC02-13` citée par TB-013 englobe `CR-UC02-11`, marqué `*(retiré)*` dans le
cahier. Par la règle de comptage, cette citation engage treize preuves quand douze existent. C'est le défaut
structurel de la citation par plage numérique : elle compte des bornes, pas des cas.

**Coût de l'inaction.** L'échelle de taille reste recalculable — sa mécanique est bonne — mais elle mesure une surface
de vérification surévaluée sur quatre paires de tâches, et fausse d'une unité sur une cinquième. Une décision de
scinder une tâche `L` serait prise sur un nombre qui compte deux fois la même preuve.

**Recommandation.** Citer les critères **par user story** plutôt que par plage numérique. Le cahier de recette porte
une colonne `Source` qui nomme, pour chaque cas, la user story et le scénario Gherkin dont il dérive : la sous-plage
d'une user story est donc déjà dans le corpus et se lit par une commande (§3.9). Une citation par user story ne peut
englober un cas retiré, et ne peut pas être portée par deux tâches puisque la tranche verticale qui porte cette user
story est unique.

---

### 2.5 Rien d'observable par un utilisateur avant le maillon 18 d'une chaîne de 22

**Mesure.** Profondeur, dans le graphe de dépendances, des tâches dont le périmètre d'écriture nomme une fiche
d'écran — seules surfaces qu'un utilisateur peut voir.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
dep, per = {}, {}
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    dep[i] = set(re.findall(r'TB-\d{3}', ch.get('Dépend de', '')))
    per[i] = ch.get("Périmètre d'écriture", '')
memo = {}
def prof(n):
    if n not in memo: memo[n] = 1 + max((prof(d) for d in dep.get(n, ()) if d in dep), default=0)
    return memo[n]
def visible(p): return 'TROU' not in p and re.search(r'fiche\s+`', p) is not None
vis = sorted((prof(k), k) for k in dep if visible(per[k]))
print("chaine la plus longue :", max(prof(k) for k in dep))
print("premiere surface visible :", vis[0])
print("taches livrant une surface visible :", len(vis), "sur", len(dep))
for p, k in vis[:5]: print("   ", p, k)
PY
```

Sortie brute :

```
chaine la plus longue : 22
premiere surface visible : (18, 'TB-042')
taches livrant une surface visible : 12 sur 55
    18 TB-042
    18 TB-043
    18 TB-051
    19 TB-044
    19 TB-045
```

**Conclusion.** Dix-sept maillons de chaîne se succèdent avant qu'un utilisateur puisse voir quoi que ce soit. Or ce
que ce produit cherche à prouver est écrit, et c'est de l'observable : les cinq hypothèses de
[`vision-produit.md §2.3`](../conception/besoin/vision/vision-produit.md) portent sur des comportements d'utilisateurs
réels, mesurés par activation. La tâche d'instrumentation qui alimente ces mesures, TB-053, est au maillon 19 ; le hub
de conduite de session, sujet direct de l'hypothèse H2, est au maillon 18.

Le plan construit donc dans l'ordre exact que la mission de cette revue qualifiait de confortable et dangereux :
tout ce qui est certain d'abord, tout ce qui est incertain à la fin. Et il a raison de le faire, parce que la
contrainte le lui impose : **l'ordre d'une tâche est celui de ses dépendances techniques, rien d'autre.** L'agrégat
`Space` précède réellement l'écran qui le manipule.

**Ce n'est donc pas une erreur du plan, c'est une conséquence du découpage par couche** — et c'est le second effet du
redécoupage vertical : une tranche verticale, par construction, livre une surface observable dès qu'elle se ferme.
Le premier signal produit ne se déplace pas parce qu'on aurait réordonné par la valeur, ce que la contrainte interdit
et que cette revue ne fait pas ; il se déplace parce que la tranche qui le porte cesse de dépendre de l'achèvement de
quatre étages.

**Coût de l'inaction.** Tant que la première surface observable est au maillon 18, aucune des cinq hypothèses de
validation ne reçoit le moindre commencement de preuve avant que J1 soit presque entièrement construit. Le risque
produit — celui de construire correctement quelque chose dont personne ne veut — reste entier jusqu'à la toute fin
du jalon.

**Recommandation.** Ne rien réordonner. Redécouper, et **remesurer cette même profondeur après redécoupage** : c'est
l'indicateur qui dit si le redécoupage a produit son effet. Si la première surface visible ne remonte pas nettement,
la proposition de la §3 a échoué et doit être rejetée.

---

### 2.6 Vingt-quatre tâches sur cinquante-cinq ne satisfont pas la condition d'entrée du plan

**Mesure.** La [méthode de ticket §2](methode-de-ticket.md) pose cinq conditions cumulatives, dont la quatrième exige
un renvoi de critère dont la cible existe — une valeur `TROU` ne compte pas — et la cinquième un périmètre d'écriture
portant au moins un module — une valeur `TROU` ne compte pas non plus.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
blocs = re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]
tp, ta = [], []
for b in blocs:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    if 'TROU' in ch.get("Périmètre d'écriture", ''): tp.append(i)
    if 'TROU' in ch.get("Critères d'acceptation", ''): ta.append(i)
print("condition 5 non satisfaite (perimetre TROU)   :", len(tp))
print("condition 4 non satisfaite (acceptation TROU) :", len(ta))
print("union, sur un total de", len(blocs), ":", len(set(tp) | set(ta)))
PY
```

Sortie brute :

```
condition 5 non satisfaite (perimetre TROU)   : 20
condition 4 non satisfaite (acceptation TROU) : 9
union, sur un total de 55 : 24
```

**Conclusion.** Vingt-quatre tâches sur cinquante-cinq sont, dès aujourd'hui et par les règles du plan lui-même,
**non prenables**. Le plan et la méthode de ticket ne le cachent pas — la méthode écrit explicitement qu'une tâche
`TROU` « reste à l'état non prêt jusqu'à ce que le corpus source referme le trou ». Le fait notable n'est pas que la
règle existe, c'est sa **proportion** : à l'ouverture du build, un peu moins de la moitié du plan est inerte.

Ces vingt-quatre tâches ne sont pas de même nature. Trois familles se distinguent, et elles ne se traitent pas
pareil :

- **Hors maille par nature, et légitimement** — TB-001 (confirmation d'ADR), TB-012 (convention de commit),
  TB-055 (verdicts de recette) ne visent aucun module de code. Leur `TROU` dit une vérité : la maille de désignation
  du plan ne parle que de code. Ce n'est pas à refermer, c'est à reconnaître.
- **En attente d'une décision de structure** — TB-006, TB-008, TB-009, TB-011, TB-040 attendent que soient nommés le
  projet de test d'architecture, la plateforme d'intégration continue, l'emplacement des configurations, le projet de
  test de bout en bout. C'est le second trou ouvert, traité en §4.2.
- **Non nommables faute de maille adaptée** — TB-029, TB-031 à TB-037, TB-039, TB-041, TB-054 sont des composants de
  la couche client dont le corpus décrit la fonction sans nommer le module. C'est la famille que le redécoupage peut
  réduire, et la §3.3 mesure de combien.

**Coût de l'inaction.** Une personne qui applique honnêtement la condition d'entrée à l'ouverture du build trouve
moins de la moitié du plan prenable, sans qu'aucun document ne l'ait prévenue de l'ampleur. Elle conclura soit que le
plan n'est pas prêt, soit qu'il faut ignorer la condition d'entrée — et la seconde issue est la plus probable, parce
que c'est la seule qui permet de commencer.

**Recommandation.** Porter le fait dans la méthode de ticket, non comme un décompte figé mais comme la commande qui
l'établit, et **distinguer les trois familles** ci-dessus : la première n'est pas un trou, la deuxième est un préalable
de structure, la troisième est un défaut de maille. Aujourd'hui les trois portent le même marqueur `TROU`, ce qui les
rend indiscernables et laisse croire qu'elles se referment de la même façon.

---

### 2.7 La condition d'entrée a un trou : un renvoi vers un point à trancher passe l'épreuve

**Mesure.** Application de la condition d'entrée, point par point et renvoi par renvoi, à trois tâches : une de
domaine (TB-014), une d'écran (TB-047), une marquée `TROU` (TB-029).

```bash
# TB-014 : les douze regles metier, les cinq scenarios Gherkin, la citation du
# cahier, les deux arbitrages de zoning et les douze cas de recette existent-ils ?
grep -oE 'RB-05-(0[1-9]|1[0-2])\b' docs/conception/besoin/user-stories/US-UC-05-*.md | sed 's/.*://' | sort -u | wc -l
grep -oF 'Le MJ supprime un dossier non vide — option Non classés' docs/conception/besoin/user-stories/US-UC-05-*.md | wc -l
grep -oF 'isSystem` est informatif : les dossiers système sont renommables et supprimables' docs/test/cahier-strategie-test-et-recette.md | wc -l
grep -oE '\bAR-(16|20)\b' docs/conception/interface/zoning.md | sort -u | wc -l
grep -oE 'CR-UC05-(0[1-9]|1[0-2])\b' docs/test/cahier-strategie-test-et-recette.md | sort -u | wc -l
# TB-047 : la fiche d'ecran, l'arbitrage de zoning, les vingt-et-un cas
ls docs/conception/interface/wireframes/sv3-preparation/editeur-document/
grep -oE 'CR-UC04-(0[1-9]|1[0-9]|2[01])\b' docs/test/cahier-strategie-test-et-recette.md | sort -u | wc -l
# TB-029 : que dit sa cible ?
grep -n 'Services Angular IndexedDB' docs/gestion-projet/roadmap-entree-build.md
```

Sortie brute :

```
12
1
1
2
12
editeur-document.dc.html
editeur-document.md
21
247:| J1 | Services Angular IndexedDB (wrappers, `DomSanitizer`, bandeaux durabilité/confidentialité, CSP complète) | P6 | [ADR-017 §Points à trancher](...) l.275 |
```

**Conclusion.** Sur TB-014 et TB-047, **tous les renvois résolvent, jusqu'au bout et au caractère près** : les douze
règles métier existent, les cinq titres de scénario se retrouvent littéralement, la citation du cahier de recette est
verbatim, les arbitrages de zoning existent, les plages de cas de recette sont complètes, la fiche d'écran est sur le
disque. La discipline de renvoi du plan est saine et son contrôle par citation littérale fonctionne. Une personne
découvrant le projet **peut faire** ces deux tâches : tout ce dont elle a besoin est écrit et atteignable.

Elle ne peut pas **clore** TB-014, pour la raison mesurée en §2.3 — ses critères décrivent un comportement que son
périmètre ne rend pas atteignable. Elle peut clore TB-047, parce que ses dépendances lui apportent le domaine dont
l'écran a besoin. Le défaut est donc bien dans le découpage, pas dans le corpus référencé.

Sur TB-029, la condition 4 est **formellement satisfaite et substantiellement vide**. Le renvoi existe : c'est une
ligne de l'Annexe A de la roadmap. Mais cette ligne est une **entrée de registre de points à trancher** — elle porte
un code de renvoi, `P6`, et pointe vers la section « Points à trancher » d'un ADR. Le renvoi résout vers un document
qui dit « ceci reste à décider ». La condition 4 teste l'**existence** de la cible, pas le fait qu'elle **énonce un
critère vérifiable**.

**Coût de l'inaction.** Une tâche dont le seul critère renvoie à un point non tranché peut être déclarée prête. Sur
TB-029 le risque est nul, parce que son périmètre `TROU` la fait échouer à la condition 5. Mais rien ne garantit
qu'une future tâche à périmètre nommable et à critère non tranché soit attrapée : la condition 5 ne la rattraperait pas.

**Recommandation.** Renforcer la condition 4 d'une clause : un renvoi dont la cible porte, pour le point en cause, un
marqueur de décision non prise ne satisfait pas la condition. C'est vérifiable par une commande, puisque le corpus
balise ces points par une convention nommée
([`guide-conventions-et-dod.md § Convention de balisage`](guide-conventions-et-dod.md)) — il suffit de constater
qu'aucun marqueur ne se trouve dans la section citée.

---

### 2.8 Ce qui est sain, mesuré, et ne doit pas bouger

Cette revue conclut au redécoupage. Elle doit dire aussi, avec la même exigence de preuve, ce que le plan fait juste
et ce que le redécoupage doit préserver intact.

**Les conflits sont réellement calculés, pas affirmés.** Le plan pose en règle (c) que `En conflit avec` est
l'intersection des périmètres d'écriture. Recalcul de cette intersection sur toutes les paires de tâches à périmètre
nommable, comparé au champ déclaré : **zéro écart**.

```bash
python3 - <<'PY'
import re
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
T = {}
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    i = re.match(r'(TB-\d{3})', b).group(1)
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    per = ch.get("Périmètre d'écriture", '')
    T[i] = ({} if 'TROU' in per else
            {x.strip() for x in per.split(';') if x.strip() not in ('', '—', '-')},
            set(re.findall(r'TB-\d{3}', ch.get('En conflit avec', ''))))
ecarts = 0
for a in sorted(T):
    for b_ in sorted(T):
        if a >= b_ or not T[a][0] or not T[b_][0]: continue
        partage = bool(set(T[a][0]) & set(T[b_][0]))
        declare = b_ in T[a][1] or a in T[b_][1]
        if partage != declare:
            ecarts += 1; print(f"  ECART {a} <-> {b_}")
print("ecarts :", ecarts)
PY
```

Sortie brute : `ecarts : 0`

**Les décomptes que le plan écrit sont exacts.** Les justifications de taille des tâches `L` citent des nombres de
preuves — treize, vingt-et-un, vingt-neuf, neuf, douze. Contrôlés contre le cahier de recette, ils correspondent tous
(la seule exception est le cas retiré de la §2.4, qui est un défaut de méthode de citation, pas d'arithmétique).

**Chaque tâche `L` porte sa raison de non-scission, et ces raisons tiennent.** Les dix-neuf ont été ouvertes une par
une. Deux familles s'y distinguent, toutes deux légitimes : les gestes solidaires — la solution .NET s'échafaude d'un
bloc, les cinq namespaces se posent d'un bloc, les object stores s'initialisent d'un `onupgradeneeded` unique — et
les surfaces de vérification indivisibles d'un agrégat. Une seule justification signale d'elle-même sa faiblesse et
propose sa propre coupe : TB-051, qui écrit être « candidate à un redécoupage par mode » sans trancher. C'est
exactement la bonne posture, et le redécoupage vertical lui donne la coupe qu'elle attendait (§3.8).

**La séparation épique / lot parallélisable est juste**, et la règle qui interdit de créer un identifiant neuf là où
un use case porte déjà le regroupement est à conserver telle quelle — pour la raison que le plan donne en §1 : deux
objets homonymes couvrant le même périmètre, avec deux ordres internes divergents, sont pires qu'un découpage
imparfait.

**Recommandation.** Ces quatre propriétés sont ce qui fait la valeur du plan, et elles sont indépendantes du
découpage par couche : périmètre déclaré, conflit calculé, taille recalculable, critère par renvoi. La proposition de
la §3 les préserve toutes les quatre, et la §3.9 donne la commande qui le vérifie.

---

## 3. Proposition instruite — le redécoupage en tranches verticales

Le redécoupage est décidé ; ce qui suit l'instruit. La difficulté est nommée : le découpage par couche rendait le
recouvrement d'écriture **calculable**, et une tranche verticale traverse par construction plusieurs couches. Appliqué
naïvement, le redécoupage sérialiserait tout — à la maille actuelle, presque toutes les tranches partageraient « le
projet Application unique ». Il faut donc une autre maille, et elle doit s'appuyer sur ce que le corpus nomme déjà.

### 3.1 L'unité de la tranche : la user story

Avant de choisir une maille de périmètre, il faut choisir **ce qui fait une tranche**. Trois candidats existaient : le
use case, l'agrégat, la user story. C'est la user story, pour trois raisons dont deux sont mesurées.

1. **Elle porte ses critères d'acceptation en propre.** Les scénarios Gherkin vivent dans la section « User stories »
   du fichier `US-UC-NN-*.md`, un bloc par `US-NN-KK`. Une tranche adossée à une user story cite les scénarios de
   cette story seule — donc des critères qu'elle rend elle-même atteignables. C'est la réponse directe au défaut de
   la §2.3.
2. **Sa sous-plage de cas de recette est déjà écrite dans le corpus.** Le cahier de recette porte, pour chaque cas,
   une colonne `Source` qui nomme la user story et le titre du scénario. La partition des cas par story est donc
   dérivable par commande, avec un résidu nommé et expliqué par le cahier lui-même (§3.9).
3. **Son nombre est du bon ordre de grandeur.** Le périmètre J1 compte 48 identifiants `US-NN-KK` sur les use cases
   01 à 07 et 14, dont une partie est hors MVP (parcours express, quota cloud). Le plan y porte aujourd'hui 43 tâches.
   Le redécoupage ne multiplie donc pas le nombre d'objets — il le déplace.

```bash
git ls-files -z 'docs/conception/besoin/user-stories/US-UC-*.md' \
  | xargs -0 grep -hoE 'US-(0[1-7]|14)-[0-9]{2}' | sort -u | wc -l
```

Sortie brute : `48`

Le use case était trop gros — une tranche par use case reproduirait les tâches `L` non clôturables. L'agrégat était
tentant mais c'est un objet de structure, pas de comportement : une tranche « agrégat `Document` » retomberait dans
le défaut de la §2.3. L'agrégat trouve sa place ailleurs, comme maille de périmètre — c'est le point suivant.

### 3.2 La maille de désignation du périmètre d'écriture : l'agrégat

**La maille retenue est l'agrégat de domaine**, nommé par le modèle de domaine, en remplacement du namespace de
bounded context et du projet .NET. Les trois autres natures de la maille actuelle sont conservées inchangées :
l'object store local, la fiche d'écran, et le module transverse nommé (`SharedKernel`,
`Haversack.Infrastructure.Notifications`).

**Sur quoi du corpus elle s'appuie.** [`docs/conception/domain/`](../conception/domain/README.md) nomme ses agrégats
en sections propres — `## Agrégats` dans trois fichiers de contexte, `## Agrégat unique : User` dans le quatrième —
et distingue typographiquement l'agrégat de ce qui vit dedans : `### Document (agrégat principal)` face à
`### DocumentBlock (entité dans Document)`. La commande suivante montre ce nommage — elle ramène tous les titres de
la zone, agrégats et éléments internes confondus ; c'est la commande d'extraction plus bas qui applique la règle
d'exclusion et produit la liste fermée :

```bash
grep -hnE '^(## Agrégat unique|### [A-Za-z]+( \((agrégat|entité|value object)[^)]*\))?$)' \
  docs/conception/domain/*.md
```

Huit agrégats en résultent, et l'exclusion des entités et value objects internes n'est pas une commodité : **un
agrégat est la frontière transactionnelle du modèle**. Deux tranches qui écrivent deux agrégats distincts écrivent
deux racines distinctes ; deux tranches qui écrivent le même agrégat écrivent la même racine et doivent être
sérialisées. La maille coïncide avec la propriété qu'elle sert, ce qui n'était pas le cas du namespace — un namespace
n'est pas une unité d'écriture, c'est un tiroir.

**Pourquoi cette maille ne dépend pas du trou d'arborescence.** C'est le point qui décide. Une maille par chemin de
fichier exigerait que soit tranchée l'arborescence interne des projets, marquée `[À TRANCHER — J0]` par
[`structure-projets.md §3`](../architecture/structure-projets.md) — et le redécoupage deviendrait alors bloqué par ce
trou. L'agrégat, lui, est un objet du modèle de domaine, nommé et stable **indépendamment de sa future disposition en
fichiers**. Le redécoupage vertical ne rend donc pas le trou d'arborescence bloquant. C'est un résultat de première
importance, et il est l'inverse de ce que l'on pouvait craindre : la maille par agrégat est la seule qui permette de
verticaliser **sans** attendre une décision de structure (§4.2).

**Mesure de l'écart à combler.** Vérification, item par item, de la résolution des périmètres actuels dans la maille
proposée :

```bash
python3 - <<'PY'
import re, glob
AG = set()
for f in sorted(glob.glob('docs/conception/domain/*.md')):
    sous = False
    for l in open(f, encoding='utf-8'):
        h2 = re.match(r'^##\s+(.*?)\s*$', l)
        if h2:
            sous = h2.group(1).startswith('Agrégats')
            m = re.match(r'^Agrégat unique\s*:\s*`?([A-Za-z]+)`?', h2.group(1))
            if m: AG.add(m.group(1))
            continue
        h3 = re.match(r'^###\s+([A-Za-z]+)\s*(?:\(([^)]*)\))?\s*$', l)
        if h3 and sous:
            q = h3.group(2) or ''
            if ' dans ' in q: continue
            if q == '' or 'agrégat' in q or 'référence' in q: AG.add(h3.group(1))
NS = {'IdentityAccess','SpaceManagement','ContentLibrary','SessionConduct'}
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
trou = couche = agrege = 0
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
    p = ch.get("Périmètre d'écriture", '')
    if 'TROU' in p: trou += 1; continue
    items = {x.strip() for x in p.split(';') if x.strip() not in ('', '—', '-')}
    if any(i in NS or 'projet' in i for i in items): couche += 1
    else: agrege += 1
print("agregats extraits :", len(AG), sorted(AG))
print("taches a perimetre TROU                        :", trou)
print("taches designees par une couche (a verticaliser):", couche)
print("taches deja designees hors couche (inchangees) :", agrege)
PY
```

Sortie brute :

```
agregats extraits : 8 ['Document', 'DocumentType', 'Folder', 'GuestAccess', 'Session', 'SessionViewConfig', 'Space', 'User']
taches a perimetre TROU                        : 20
taches designees par une couche (a verticaliser): 16
taches deja designees hors couche (inchangees) : 19
```

**Un point de lecture, et non de règle, à signaler.** `DocumentType` porte le qualificatif « entité de référence » et
non « agrégat ». Le motif d'extraction l'admet parce qu'un catalogue de référence s'écrit indépendamment de toute
instance de `Document` — mais c'est une **lecture du corpus**, pas une correspondance littérale. C'est le seul des
huit items où le motif ne suffit pas seul, et il doit être ratifié plutôt que présumé.

### 3.3 Ce que la maille par agrégat ne nomme toujours pas

Une proposition qui prétendrait refermer les vingt `TROU` serait fausse. Elle en referme une partie, et il faut dire
laquelle.

| Famille | Tâches concernées | Effet de la maille par agrégat |
|---|---|---|
| Services d'accès au store local | TB-029 | **Refermée** — un service d'accès sert des agrégats nommés ; son périmètre devient la liste de ces agrégats, et il se dissout dans les tranches qui les portent |
| Fonctions de projection et d'enveloppe d'export | TB-036, TB-037, TB-039 | **Refermée** — la projection lit des agrégats nommés ; c'est d'ailleurs ce que le plan écrit déjà en clair (« fonction de projection transverse aux object stores ») |
| Bandeaux et châssis transversaux | TB-032, TB-033, TB-041, TB-054 | **Non refermée** — un bandeau de durabilité n'appartient à aucun agrégat ; il reste `TROU`, et c'est exact |
| Appel d'API navigateur, sanitisation, politique de sécurité de la page | TB-031, TB-034, TB-035 | **Non refermée** — ces objets vivent au niveau de la page ou de la base entière |
| Projets de test, configuration d'intégration continue, conventions | TB-006, TB-008, TB-009, TB-011, TB-040 | **Non refermée** — relève de la décision de structure (§4.2) |
| Actes documentaires et verdicts de recette | TB-001, TB-012, TB-055 | **Hors maille par nature** — ne visent aucun module de code, et ne doivent pas y entrer |

Sur vingt tâches à périmètre `TROU`, la maille par agrégat en referme quatre et laisse seize ouvertes, dont trois
légitimement hors maille. **La proposition ne résout pas le manque de nom pour la couche cliente transversale** — le
corpus ne nomme aucun répertoire pour cette couche ([`structure-projets.md §7`](../architecture/structure-projets.md)
en décrit la fonction, pas l'emplacement), et cette revue n'a pas à le nommer à sa place.

### 3.4 La forme d'une tranche verticale — ce qui change, ce qui ne change pas

**Ne changent pas** — et c'est ce qui préserve la valeur du plan (§2.8) :

| Champ | Pourquoi il ne change pas |
|---|---|
| `But` | reste un énoncé observable ; il devient simplement un comportement complet au lieu d'un artefact de couche |
| `Épique` | reste l'epic du corpus quand un use case porte le regroupement, un `EP-nn` sinon (§3.6) |
| `Jalon` | inchangé — le phasage n'est pas rouvert |
| `Dépend de` | sémantique inchangée : dépendances techniques seules, jamais une hypothèse d'effectif |
| `En conflit avec` | toujours **calculé** par intersection des périmètres, jamais affirmé ; seule la maille de l'intersection change |
| `Taille` | même échelle, mêmes seuils, même règle de comptage des renvois, même recalculabilité |
| `Critères d'acceptation` | toujours des renvois seuls, jamais de substance réénoncée |
| `Code de renvoi`, `Point d'attention`, `Trou balisé` | inchangés |

**Changent** :

| Champ | Ce qui change |
|---|---|
| `Périmètre d'écriture` | la maille : l'agrégat remplace le namespace et le projet .NET (§3.2). Les trois autres natures — object store, fiche d'écran, module transverse nommé — sont conservées à l'identique |
| `Critères d'acceptation` | la **méthode de citation** : par user story et scénario nommé, plutôt que par plage numérique de cas de recette (§2.4). La plage reste dérivable, elle cesse d'être recopiée |

**Aucun champ nouveau.** La trace d'absorption des identifiants ne prend pas la forme d'un champ (§3.7).

### 3.5 La règle qui décide quand le front se détache

Le front se détache **quand la fiche d'écran qu'il porte sert plus d'un use case** ; il reste dans la tranche sinon.
La règle n'est pas une convenance : elle se lit sur la table de couverture UC → fiche(s) de
[`wireframes/README.md`](../conception/interface/wireframes/README.md), et se calcule.

```bash
python3 - <<'PY'
import re, collections
t = open('docs/conception/interface/wireframes/README.md', encoding='utf-8').read()
bloc = t.split('## Table de couverture UC → fiche(s)')[1].split('\n---')[0]
par_fiche = collections.defaultdict(set)
for l in bloc.splitlines():
    m = re.match(r'^\|\s*\*\*(UC-\d{2})\*\*.*?\|(.*)\|\s*$', l)
    if not m: continue
    for f in re.findall(r'`([a-z0-9-]+)/\1\.md`', m.group(2)):
        par_fiche[f].add(m.group(1))
for f, u in sorted(par_fiche.items()):
    print(("SE DETACHE " if len(u) > 1 else "DANS LA TRANCHE ") + f"{f:26} {sorted(u)}")
PY
```

Six fiches se détachent — `vue-session-mj` (4 use cases), `parametres-campagne` (3), `inscription`,
`navigation-dossiers`, `panneau-creation-rapide`, `vue-joueur-base` (2 chacune) — et quatorze restent dans la tranche
du comportement qu'elles servent.

**Le fondement de la règle.** Une fiche mono-use-case est la surface d'un comportement : la détacher recréerait le
défaut de la §2.3, une moitié de comportement dont le critère n'est pas atteignable. Une fiche composite est au
contraire un **lieu de composition** — le hub de session accueille la conduite, le partage, la gestion de membres et
la recherche — et l'attacher à une seule tranche la placerait en conflit d'écriture avec toutes les autres tranches
qui doivent y loger quelque chose. Une tranche détachée sur une fiche composite est donc l'inverse d'un retour au
découpage par couche : c'est la reconnaissance d'un module réellement partagé, exactement ce que la maille est faite
pour désigner.

**Conséquence à assumer** : une tranche détachée sur une fiche composite n'est pas clôturable sur un scénario Gherkin
complet, puisqu'elle ne livre qu'une surface. Son critère d'acceptation est celui de la fiche — zones, hiérarchie,
arbitrages de zoning — que le corpus porte bel et bien, et non un comportement. C'est une exception nommée à la
propriété de la §2.3, bornée à six fiches sur vingt, et elle doit être écrite comme telle plutôt que subie.

### 3.6 Ce que deviennent les épiques

**Mesure de l'état actuel** : nombre de tâches par épique.

```bash
python3 -c "
import re, collections
t = open('docs/gestion-projet/plan-de-travail.md', encoding='utf-8').read()
e = collections.Counter()
for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
    ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*\$', b, re.M)}
    e[ch.get('Épique', '')] += 1
for k, v in sorted(e.items()): print(f'{k:12} : {v}')
"
```

Sortie brute (extrait) : `EP-01 : 1`, `EP-04 : 1`, `EP-10 : 1`, `EP-12 : 1`, `US-UC-01 : 1`, `US-UC-14 : 1`,
`EP-06 : 2`, `EP-07 : 5`, `EP-08 : 5`, `EP-09 : 5`, `US-UC-06 : 6`.

**Trois épiques nomment une couche et non une capacité**, ce qui était cohérent dans un découpage par couche et
cesse de l'être : **EP-06 — Contrats Application du périmètre local**, **EP-07 — Socle technique du store local
IndexedDB**, et pour partie **EP-02 — Échafaudage de la solution .NET**.

**Recommandation, épique par épique** :

- **EP-06 se dissout.** « Chaque geste métier passe par un cas d'usage Application unique » est un invariant
  d'architecture, pas un lot de travail : c'est une clause de Definition of Done, et
  [`guide-conventions-et-dod.md §1 § Architecture`](guide-conventions-et-dod.md) la porte déjà. Chaque tranche
  traverse la couche Application pour son propre geste ; aucune épique n'a besoin de les regrouper. TB-023, qui câble
  l'invariant d'autorisation et est un **préalable bloquant d'entrée en J1** nommé par la roadmap, rejoint EP-04 dont
  il est la suite directe.
- **EP-07 se dissout aussi, sauf son premier maillon.** Les object stores, les index et les validations locales sont
  la moitié cliente de chaque tranche. Reste indivisible l'ouverture de la base et son versionnement — un
  `onupgradeneeded` porte sur la base entière, le plan le dit et c'est exact. Cette part rejoint EP-02, dont la nature
  est déjà celle d'un socle indivisible.
- **EP-02, EP-03, EP-05 restent, et c'est justifié.** Un socle n'est pas un découpage par couche déguisé : ce sont
  des gestes solidaires, sans comportement utilisateur, dont aucun use case ne peut porter le regroupement. Les
  garder est cohérent avec la règle qui réserve `EP-nn` à ce que nul use case ne porte.
- **EP-08 à EP-12 restent.** Durabilité et sécurité du mode local, export, châssis, instrumentation, recette : cinq
  regroupements que le corpus reconnaît comme des unités fonctionnelles sans use case — trois d'entre elles sont
  d'ailleurs les trois Must Have sans use case identifiés par
  [`moscow.md § Vue d'ensemble`](../conception/besoin/vision/moscow.md).
- **Les épiques adossées à un use case accueillent tout le reste**, et suffisent : les tranches étant adossées aux
  user stories, et chaque user story vivant dans un fichier `US-UC-NN-*.md`, le rattachement est immédiat et sans
  ambiguïté. C'est un gain propre du redécoupage : le rattachement d'une tranche à son épique cesse d'être un
  jugement pour devenir une lecture.
- **Une épique réduite à une seule tranche reste légitime** si son but est une capacité — US-UC-01, US-UC-14, EP-04,
  EP-10 sont dans ce cas. Le nombre de tâches n'est pas un critère d'existence d'une épique ; le fait qu'un but
  fonctionnel soit nommé l'est.

**La règle qui ne bouge pas** : là où un use case porte le regroupement, l'épique **est** l'epic du corpus et ne
reçoit aucun identifiant neuf. Aucune des recommandations ci-dessus ne crée d'homonyme.

### 3.7 Les identifiants : absorption tracée, aucun renumérotage

Les tranches prennent des `TB-nnn` **neufs à la suite** — le plan s'arrête à TB-055, les tranches commencent à TB-056.
Aucun identifiant existant n'est renuméroté, réattribué ni réemployé pour désigner un autre périmètre.

Un identifiant absorbé est **retiré**, avec la tranche qui le reprend. La trace vit dans une section dédiée du plan,
sous forme de tableau à deux colonnes — et **délibérément pas sous forme de fiche de tâche** : le plan est déjà lu par
un outil qui repère ses tâches à leur titre de niveau 4 à 6 commençant par `TB-`, et une fiche de tâche retirée
produirait un ticket pour une tâche qui n'existe plus. Un tableau ne porte pas ce titre, donc ne produit rien.

```
| Identifiant retiré | Repris par |
|---|---|
| TB-013 | TB-056, TB-057 |
| TB-043 | TB-056 |
```

### 3.8 Trois tâches actuelles redécoupées — exemples travaillés

Les trois exemples sont donnés avec le champ `Dépend de` que la tranche porterait, et la profondeur de graphe qui en
résulte, à comparer à la profondeur actuelle mesurée en §2.5. Les critères d'acceptation citent la sous-plage de cas
de recette **dérivée de la colonne `Source` du cahier** (§3.9) — ils ne l'inventent pas.

#### Exemple A — l'épique US-UC-02, quatre tâches de couche vers trois tranches

État actuel : TB-013 (agrégat `Space`, profondeur 6, taille `L`), TB-043 (tableau de bord, profondeur 18),
TB-044 (écran de création, profondeur 19), TB-045 (vue campagne et vue espace personnel, profondeur 19, taille `L`).

**TB-056 — L'espace personnel existe et se voit, sans geste du MJ**

| Champ | Valeur |
|---|---|
| But | à la première ouverture, un espace `PERSONAL` est présent avec son dossier virtuel « Non classés », visible au tableau de bord et hors quota |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-005 |
| Périmètre d'écriture | `Space` ; store local `spaces` ; fiche `tableau-de-bord` |
| En conflit avec | TB-057 — module partagé : `Space` |
| Taille | L — trois modules ; le geste est indivisible : l'espace `PERSONAL` n'existe pas sans être persisté ni sans être visible, c'est ce qui en fait un comportement |
| Critères d'acceptation | US-02-00, ses scénarios nommés ; CR-UC02-01 → CR-UC02-03 (dérivés de la colonne `Source`) ; les règles métier que la table de couverture de `US-UC-02-creer-espace-jeu.md` rattache à US-02-00 |

**TB-057 — Créer une campagne nommée depuis le tableau de bord**

| Champ | Valeur |
|---|---|
| But | le MJ crée un espace `CAMPAIGN` ou `ONE_SHOT` avec un nom seul, jamais bloqué par un quota en mode local ; sans nom, le refus est explicite |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | `Space` ; fiche `creation-espace` |
| En conflit avec | TB-056 — module partagé : `Space` |
| Taille | L — deux modules et 6 renvois (la plage compte pour son étendue, §3 du plan) ; le geste de création est indivisible de l'écran qui le porte |
| Critères d'acceptation | US-02-01, ses scénarios nommés ; CR-UC02-04 → CR-UC02-08 (dérivés de la colonne `Source`) |

**TB-058 — Le hub de travail d'un espace, et sa déclinaison personnelle**

| Champ | Valeur |
|---|---|
| But | le MJ accède au hub de travail d'un espace `CAMPAIGN`/`ONE_SHOT` et à la déclinaison sans vue session ni partage de l'espace `PERSONAL` |
| Épique | US-UC-02 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | fiche `vue-campagne` ; fiche `vue-espace-personnel` |
| En conflit avec | — |
| Taille | L — deux fiches et 4 renvois ; la déclinaison `PERSONAL` n'étant « pas une surface-UC distincte », les deux fiches sont livrées ensemble |
| Critères d'acceptation | les deux fiches de wireframe ; `zoning.md §S6 AR-14`, `AR-15` ; US-01-09, son scénario nommé |

**Ce que l'exemple montre.** La profondeur de la première surface visible de cette épique passe de **18 à 6**. La
tâche `L` non clôturable TB-013 disparaît au profit de deux tranches dont les critères sont atteignables et d'une
tranche de surface. TB-058 conserve deux fiches ensemble pour la raison que TB-045 donnait déjà, et qui reste valable :
la déclinaison `PERSONAL` n'est « pas une surface-UC distincte ». Le total passe de quatre objets à trois.

#### Exemple B — TB-014 et TB-046, la paire qui comptait deux fois la même preuve

État actuel : TB-014 (agrégat `Folder`, profondeur 7, taille `L`, cite `CR-UC05-01 → CR-UC05-12`) et TB-046
(navigation par dossiers, profondeur 20, taille `L`, cite **la même plage**).

`navigation-dossiers` sert deux use cases : la fiche **se détache** (§3.5). Le redécoupage donne donc une tranche de
surface et des tranches de comportement.

**TB-059 — Chaque espace naît avec son arborescence de dossiers**

| Champ | Valeur |
|---|---|
| But | un espace naît avec son gabarit de dossiers ; « Non classés » est présent et protégé ; les dossiers système sont renommables et supprimables |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-056 |
| Périmètre d'écriture | `Folder` ; store local `folders` |
| En conflit avec | TB-060 — module partagé : `Folder` |
| Taille | L — deux modules et 6 renvois ; l'arborescence par défaut naît avec l'espace, elle ne se scinde pas de l'agrégat qui la porte |
| Critères d'acceptation | US-05-01 et US-05-02, leurs scénarios nommés ; CR-UC05-01 → CR-UC05-04 (dérivés de la colonne `Source`) ; `cahier-strategie-test-et-recette.md §3.1`, sur le caractère informatif de `isSystem` |

**TB-060 — Déplacer et supprimer un dossier sans perdre son contenu**

| Champ | Valeur |
|---|---|
| But | un dossier non vide se supprime en reversant son contenu dans « Non classés » ; un dossier système se supprime comme un autre |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-059 |
| Périmètre d'écriture | `Folder` |
| En conflit avec | TB-059 — module partagé : `Folder` |
| Taille | L — un module et 9 renvois ; les trois stories partagent le cycle de vie du dossier et ses huit cas de recette |
| Critères d'acceptation | US-05-03, US-05-04 et US-05-05, leurs scénarios nommés ; CR-UC05-05 → CR-UC05-12 (dérivés de la colonne `Source`) |

**TB-061 — Surface de navigation par dossiers** *(fiche composite détachée)*

| Champ | Valeur |
|---|---|
| But | l'arborescence d'un espace se parcourt, et la création d'un document dans le dossier courant y est accessible |
| Épique | US-UC-05 |
| Jalon | J1 |
| Dépend de | TB-059, TB-058 |
| Périmètre d'écriture | fiche `navigation-dossiers` |
| En conflit avec | — |
| Taille | S — un module, 2 renvois |
| Critères d'acceptation | la fiche de wireframe ; `zoning.md §S6 AR-11`, `AR-16`, `AR-20` |

**Ce que l'exemple montre.** La plage `CR-UC05-01 → CR-UC05-12` n'est plus citée deux fois : elle est **partitionnée**
entre TB-059 et TB-060, chacune ne portant que les cas que son comportement produit. Le double comptage disparaît sans que
l'échelle ait été touchée. La profondeur de la surface passe de 20 à 8. En revanche les tailles **ne baissent pas** :
appliquée telle quelle, la règle du plan met TB-059 et TB-060 en `L` et seule la tranche de surface TB-061 descend
en `S`. C'est un effet mesuré du redécoupage, traité en §3.10, point 5.

#### Exemple C — TB-051, la tâche `L` qui demandait elle-même sa coupe

État actuel : TB-051, une fiche unique, profondeur 18, taille `L` avec cette justification — « 10 US, 30 RB et 29 CR
distincts s'y rattachent […] candidate à un redécoupage par mode ». `vue-session-mj` sert quatre use cases : la fiche
**se détache** en une tranche de surface, et les dix user stories deviennent autant de tranches de comportement, dont
trois données ici.

| Tranche | But | Périmètre d'écriture | Critères (sous-plage dérivée) |
|---|---|---|---|
| **TB-062** | une session se lance depuis un espace, refusée sans titre ; une seule session `LIVE` par espace | `Session` ; store local `sessions` | US-06-01 ; CR-UC06-01 → CR-UC06-03 |
| **TB-063** | le MJ configure ses panneaux hors session et les modifie en direct ; la configuration persiste | `SessionViewConfig` ; store local `session_view_configs` ; store local `session_view_folders` | US-06-02 ; CR-UC06-04 → CR-UC06-06 |
| **TB-064** | une note de session MJ naît `GM_ONLY`, une note joueur `PLAYER_PRIVATE` et reste hors d'atteinte du MJ | `Session` ; `Document` ; store local `session_live_notes` | US-06-04 ; CR-UC06-09 → CR-UC06-11 |
| **TB-065** | *(surface détachée)* le hub de conduite de session, ses trois modes | fiche `vue-session-mj` | la fiche ; `zoning.md §S6 AR-02`, `AR-04`, `AR-09` ; NFR-PERF-01 → NFR-PERF-03 |

**Ce que l'exemple montre.** La coupe que TB-051 réclamait « par mode » se révèle mieux servie **par user story** :
le corpus porte déjà la partition des vingt-neuf cas de recette par story, alors qu'aucune source ne partitionne par
mode. La tâche `L` de vingt-neuf preuves devient une tranche de surface et dix tranches de comportement, chacune
clôturable — mais **pas plus petites au sens de l'échelle** : appliquée telle quelle, la règle du plan les met en `L`
elles aussi (§3.10, point 5). TB-062 et TB-063 n'ont **aucun module partagé** — `Session` et `SessionViewConfig` sont deux agrégats
distincts — donc elles sont parallélisables, ce que la maille par namespace `SessionConduct` interdisait.

#### Contre-exemple — ce que le redécoupage ne répare pas

TB-032 (bandeau de durabilité), TB-033 (bandeau de confidentialité), TB-041 (châssis) et TB-054 (capture de contact)
restent à périmètre `TROU` après redécoupage : aucun agrégat ne les porte, aucune fiche d'écran ne les héberge, et
le corpus ne nomme pas leur couche. Elles restent donc non prenables au sens de la condition d'entrée. Le redécoupage
ne referme pas ce trou et ne doit pas prétendre le faire.

### 3.9 Le contrôle mécanique de la proposition

Une garantie qui ne se contrôle plus par une commande n'est qu'une intention. Trois contrôles couvrent la proposition,
tous exécutables sans build.

**Contrôle 1 — chaque item de périmètre se résout dans la maille.** La commande de la §3.2 extrait la liste fermée des
agrégats depuis le modèle de domaine, et signale tout item de `Périmètre d'écriture` qui ne se résout ni en agrégat,
ni en object store d'ADR-017, ni en fiche de wireframe présente sur le disque, ni en module transverse nommé. Validé
en injectant une fiche inexistante dans un périmètre : l'item est signalé.

**Contrôle 2 — `En conflit avec` reste l'intersection des périmètres.** La commande de la §2.8 s'applique inchangée à
la nouvelle maille — elle ne connaît pas la nature des items, seulement leur égalité. Elle donne aujourd'hui zéro
écart et doit continuer de le donner. Validée en déclarant un conflit sans module partagé : l'écart est signalé.

**Contrôle 3 — la sous-plage de cas de recette d'une tranche est dérivée, jamais recopiée.** La colonne `Source` du
cahier de recette nomme, pour chaque cas, sa user story et son scénario.

```bash
python3 - <<'PY'
import re, collections
t = open('docs/test/cahier-strategie-test-et-recette.md', encoding='utf-8').read()
par_us, sans = collections.defaultdict(list), []
for l in t.splitlines():
    m = re.match(r'^\|\s*(CR-UC(?:0[1-7]|14)-\d{2})\s*\|', l)
    if not m: continue
    cols = [c.strip() for c in l.strip('|').split('|')]
    src = cols[5] if len(cols) > 5 else ''
    us = sorted(set(re.findall(r'US-\d{2}-\d{2}', src)))
    if us:
        for u in us: par_us[u].append(m.group(1))
    else: sans.append((m.group(1), src[:55]))
print("user stories portant leur sous-plage :", len(par_us))
print("cas sans user story en colonne Source :", len(sans))
for c, s in sans: print("   ", c, "->", s)
PY
```

Sortie brute :

```
user stories portant leur sous-plage : 43
cas sans user story en colonne Source : 10
    CR-UC01-20 -> RB-01-15 (dérivé, pas de Gherkin)
    CR-UC02-13 -> UC-02 fiche §Exceptions E2 (dérivé, pas de Gherkin)
    CR-UC03-21 -> UC-03 fiche §Exceptions E1 (dérivé, pas de Gherkin)
    CR-UC04-19 -> UC-04 fiche §Exceptions E1 (dérivé, pas de Gherkin)
    CR-UC04-20 -> UC-04 fiche §Exceptions E2 (dérivé, pas de Gherkin)
    CR-UC04-21 -> NFR-CONF-01 §Situation limite — tentative d'accès à un 
    CR-UC06-28 -> UC-06 fiche §Exceptions E2 (dérivé, pas de Gherkin)
    CR-UC06-29 -> RB-06-25/26 (dérivé, pas de Gherkin) + NFR-CONF-01 §Sit
    CR-UC07-09 -> RB-07-01..04 (dérivé, pas de Gherkin spécifique PJ)
    CR-UC14-13 -> UC-14 fiche §Exceptions E2 (dérivé, pas de Gherkin)
```

Quarante-trois user stories portent leur sous-plage, et **dix cas du périmètre J1 n'ont pas de user story en source** :
ce sont des cas dérivés d'une exception de fiche de use case, d'une règle métier ou d'une exigence non fonctionnelle,
et le cahier le dit lui-même en clair. Ces dix cas ne se rattachent donc pas mécaniquement à une tranche : ils
appartiennent aux tranches par lecture de leur source, ce qui est un acte de l'application du redécoupage, pas de
cette revue.

**Un contrôle que la proposition ne fournit pas** : la partition des **règles métier** par user story. Les quatorze
fichiers de user story portent bien une table « Vérification de couverture » qui rattache chaque règle à sa story,
mais elle le fait par **énoncé de règle** et non systématiquement par identifiant `RB-nn-kk`. La sous-plage de règles
d'une tranche se lit donc, elle ne se calcule pas. C'est une différence de nature avec les cas de recette, et elle
doit être dite.

### 3.10 Ce que le redécoupage coûte

Une proposition qui ne nomme aucune contrepartie n'a pas été instruite. Il y en a quatre, et la première est sérieuse.

**1. L'erreur de la maille cesse d'aller dans le sens sûr.** C'est la contrepartie majeure. Le plan écrit aujourd'hui,
et c'est vrai, que sa maille conservatrice produit « un coût de délai, jamais un risque de collision — l'erreur va
dans le sens sûr ». La maille par agrégat **inverse ce sens**. Deux tranches sur deux agrégats distincts sont
déclarées sans conflit, alors qu'elles écriront toutes deux dans les points de composition de la couche
Application — enregistrement des cas d'usage, table de routage, déclaration du schéma du store. La maille par
agrégat ne voit pas ces points, parce qu'ils n'appartiennent à aucun agrégat. Le résultat n'est plus un retard, c'est
une collision possible.

La mitigation existe et n'est pas la mienne : une convention plaçant les points de composition **par agrégat** plutôt
qu'en fichier unique — un enregistrement par agrégat, ou une découverte par balayage — supprime le partage. C'est une
décision de structure, à prendre à l'ouverture de J0 avec l'arborescence (§4.2), et **le redécoupage devrait être
conditionné à elle**. Sans elle, le gain de parallélisme est acquis contre un risque de collision non couvert.

**2. Le libellé « par couche » perd son sens pour une tranche.** La taxonomie de la
[méthode de ticket §3](methode-de-ticket.md) porte un axe « par couche » à huit valeurs fermées, ancré sur
`structure-projets.md`. Une tranche verticale traverse trois ou quatre de ces valeurs : lui en attribuer quatre n'est
plus une classification. L'axe doit être remplacé — pour les tranches — par un axe **« par agrégat »**, ancré sur le
modèle de domaine avec la même discipline que les trois axes existants. Les tranches de surface détachées et les
tâches de socle continuent de porter un libellé de couche, qui garde son sens pour elles.

**3. Un outil qui dérive des tickets du plan lit ce champ.** Le champ `Périmètre d'écriture` n'est pas seulement lu
par des personnes : il sert à déduire le libellé de couche. Un nom d'agrégat n'est ni un nom de projet, ni un des
cinq namespaces, ni un préfixe `store local`, ni un préfixe `fiche` — il tomberait donc en résidu non reconnu.
Changer la maille impose de mettre à jour cette correspondance, et la mise à jour est mécanique puisque la liste des
agrégats s'extrait du modèle de domaine (§3.2).

**4. Le nombre de tranches augmente là où une tâche `L` en portait dix.** TB-051 devient une tranche de surface et
dix tranches de comportement. Ce n'est pas une inflation gratuite — c'était déjà vingt-neuf preuves à produire, et le
plan le signalait — mais cela déplace du volume du champ `Taille` vers le nombre de fiches. Le plan sera plus long à
lire. Le contrepoids est que chaque fiche devient clôturable, ce qui n'était pas le cas de TB-051 prise en bloc.

**5. L'échelle de taille sature en `L` sur les tranches, et c'est mesuré.** En appliquant à la lettre la règle de
comptage du plan aux dix tranches des trois exemples travaillés — un segment de critère compte pour un, une plage
compte pour son étendue —, **huit sur dix tombent en `L`**. Deux causes se cumulent : une tranche déclare presque
toujours l'agrégat **et** son object store local, donc deux modules, ce qui abaisse le plafond `M` à deux renvois ; et
toute citation d'une sous-plage de cas de recette apporte à elle seule trois à huit renvois. Une échelle où presque
tout vaut `L` ne discrimine plus rien.

```bash
python3 -c "
def taille(m, r):
    if m == 1 and r <= 2: return 'S'
    if m == 1 and 3 <= r <= 6: return 'M'
    if m == 2 and r <= 2: return 'M'
    return 'L'
ex = [('TB-056',3,['US','plage:3','regles']), ('TB-057',2,['US','plage:5']),
      ('TB-058',2,['fiche','fiche','zoning','US']), ('TB-059',2,['US','plage:4','cahier']),
      ('TB-060',1,['US','plage:8']), ('TB-061',1,['fiche','zoning']),
      ('TB-062',2,['US','plage:3']), ('TB-063',3,['US','plage:3']),
      ('TB-064',3,['US','plage:3']), ('TB-065',1,['fiche','zoning','NFR'])]
for i, m, segs in ex:
    r = sum(int(s.split(':')[1]) if s.startswith('plage:') else 1 for s in segs)
    print(f'{i} : {m} module(s), {r} renvoi(s) -> {taille(m, r)}')
"
```

Sortie brute :

```
TB-056 : 3 module(s), 5 renvoi(s) -> L
TB-057 : 2 module(s), 6 renvoi(s) -> L
TB-058 : 2 module(s), 4 renvoi(s) -> L
TB-059 : 2 module(s), 6 renvoi(s) -> L
TB-060 : 1 module(s), 9 renvoi(s) -> L
TB-061 : 1 module(s), 2 renvoi(s) -> S
TB-062 : 2 module(s), 4 renvoi(s) -> L
TB-063 : 3 module(s), 4 renvoi(s) -> L
TB-064 : 3 module(s), 4 renvoi(s) -> L
TB-065 : 1 module(s), 3 renvoi(s) -> M
```

**Un ajustement d'une ligne suffit, et il est cohérent avec la maille elle-même** : ne pas compter l'object store local
d'un agrégat comme un module distinct de cet agrégat. Ce n'est pas un assouplissement de complaisance — c'est la
conséquence de la maille : le store local `spaces` **est** l'agrégat `Space` persisté côté client, pas un second objet.
Recalculées sous cette lecture, TB-059, TB-062 et TB-063 redescendent en `M`. L'échelle retrouve sa capacité à
discriminer, sans qu'aucun seuil ne bouge.

**C'est un ajustement de règle, donc un arbitrage** : cette revue le recommande et ne le décide pas. Le refuser est
tenable — mais il faut alors assumer que la taille cesse d'informer sur les tranches, et le dire dans le plan plutôt
que de laisser croire à une échelle qui discrimine.

**Ce que la version par couche garantissait mieux, en une phrase** : elle garantissait qu'aucune collision d'écriture
n'était possible, au prix d'un délai. La version verticale garantit qu'aucune tâche n'est non clôturable, au prix
d'une collision possible sur les points de composition — et cette collision doit être fermée par une décision de
structure avant que le redécoupage ne soit appliqué.

---

## 4. Les deux trous encore ouverts, sous l'angle produit

### 4.1 L'instrumentation de validation — un préalable, pas une dette

**Mesure.** Recherche de tout porteur de l'instrumentation dans la couche besoin, puis état de sa spécification.

```bash
for d in usecases user-stories user-journeys; do
  printf "%s : " "$d"
  git ls-files -z "docs/conception/besoin/$d/*.md" \
    | xargs -0 grep -hoiE 'instrumentation|t[ée]l[ée]m[ée]trie|analytics' | wc -l
done
grep -o '\[À TRANCHER' docs/architecture/specs/telemetrie.md | wc -l
```

Sortie brute :

```
usecases : 0
user-stories : 0
user-journeys : 0
15
```

Le motif a été élargi d'un cran avant de conclure sur ces zéros — pluriel, casse insensible, accents optionnels,
trois termes alternatifs. Les zéros tiennent.

**La chaîne, établie en lisant la roadmap.** Elle se compose en quatre maillons, et chacun est écrit dans le corpus :

1. [`moscow.md § Instrumentation de validation du MVP`](../conception/besoin/vision/moscow.md) classe l'instrumentation
   **Must Have** et nomme trois piliers à constater : activation préparation, activation vue session, **activation
   partage** — cette dernière définie comme « un document a été partagé aux joueurs ET au moins un joueur l'a consulté ».
2. Les cinq hypothèses de [`vision-produit.md §2.3`](../conception/besoin/vision/vision-produit.md) désignent la mesure
   d'usage anonyme comme leur instrument de constat — pour les cinq, sans exception.
3. [`roadmap-entree-build.md §3.3`](roadmap-entree-build.md) fait de cette mesure le **critère de la décision marché**,
   croisée avec les hypothèses H1 à H4.
4. [`roadmap-entree-build.md §3.4 § Dépendance d'entrée`](roadmap-entree-build.md) conditionne l'entrée en J2,
   cumulativement, à J1 stable **et** à cette décision marché.

L'instrumentation est donc **le seul intrant** du point de décision qui ouvre le jalon suivant. Or elle n'a aucun
porteur dans la couche besoin — ni use case, ni user story, ni scénario Gherkin, ni règle métier, ni cas de recette —
et sa spécification porte quinze points non tranchés en se déclarant elle-même « non implémentable en l'état ». La
tâche qui la porte, TB-053, a en conséquence un champ `Critères d'acceptation` intégralement marqué `TROU`.

**Conclusion : c'est un préalable, pas une dette portée jusqu'à son échéance.** L'argument est de séquence, non de
préférence. Une dette acceptable est une dette dont l'échéance tombe après le travail qui en dépend. Ici l'échéance
tombe **avant** : l'instrumentation doit avoir produit des mesures interprétables pour que la décision marché puisse
être prise, et cette décision précède l'engagement du build cloud. Un trou dont l'échéance précède ce qu'il
conditionne n'est pas une dette, c'est un blocage différé.

**Coût de ne rien faire maintenant.** J1 se construirait en entier, TB-053 resterait non prenable jusqu'au bout, et à
la fin du jalon la décision marché se présenterait sans son intrant. Deux issues alors, toutes deux mauvaises : la
décision se prend sans mesure — et les cinq hypothèses, que le corpus a pris soin de rendre falsifiables avec seuil et
délai, redeviennent des opinions — ou bien le build s'arrête le temps de spécifier puis d'implémenter
l'instrumentation, à l'endroit exact du plan où l'arrêt coûte le plus cher, tout étant déjà construit.

**Coût de le fermer maintenant.** Trancher les quinze points de la spécification demande des décisions produit
— dont le seuil du geste structurant de H1, l'opérationnalisation d'« usage réel constaté », la technique
d'anonymisation, le mécanisme de capture de contact — et n'exige aucune ligne de code. Ce travail est du cadrage, il
se fait sans que le build attende, et il produit les critères d'acceptation qui manquent à TB-053 et TB-054.

**Recommandation.** Fermer la spécification de télémétrie **avant l'entrée en construction**, et lui donner un porteur
dans la couche besoin — les scénarios Gherkin d'une user story, faute de quoi TB-053 restera non clôturable même une
fois son périmètre nommé. Ces deux actes sont des décisions produit : cette revue ne les prend pas.

**Un troisième point, distinct, à remonter séparément** : voir §5.1 — le critère de la décision marché cite un signal
qui, par la séquence de jalons, n'existe qu'après le jalon d'après.

### 4.2 L'arborescence interne des projets — une dette acceptable, sauf sur un point

**Mesure.**

```bash
grep -o '\[À TRANCHER' docs/architecture/structure-projets.md | wc -l
```

Sortie brute : `14`

**Conclusion en deux temps, et c'est le résultat le plus contre-intuitif de cette revue.**

**L'arborescence en elle-même n'est pas rendue bloquante par le redécoupage — au contraire.** On pouvait craindre
l'inverse : si la maille devait descendre au fichier pour distinguer deux tranches, alors l'arborescence
`[À TRANCHER — J0]` deviendrait un préalable dur. Ce n'est pas le cas, parce que la maille retenue est l'agrégat,
objet du modèle de domaine nommé et stable **indépendamment de sa disposition en fichiers** (§3.2). Le redécoupage
vertical se conçoit, s'écrit et se contrôle sans que l'arborescence soit tranchée. L'arborescence reste ce que le plan
en dit déjà : un plancher de maille qui pourra être affiné quand J0 l'aura tranchée, sans qu'aucun autre champ des
tâches ne change.

**Un point précis, en revanche, devient bloquant : la disposition des points de composition.** C'est la contrepartie
majeure de la §3.10. La maille par agrégat ne peut pas voir un fichier partagé par toutes les tranches — enregistrement
des cas d'usage Application, table de routage, déclaration du schéma du store local. Tant que rien ne dit que ces
points sont disposés **par agrégat**, deux tranches déclarées sans conflit peuvent se marcher dessus, et la propriété
que le plan garantit aujourd'hui est perdue.

**Recommandation.** Ne pas faire de l'arborescence complète un préalable — elle peut rester une dette portée jusqu'à
J0, comme le plan le prévoit. Mais **extraire un seul point de cette dette et le trancher avant d'appliquer le
redécoupage** : la disposition des points de composition, un par agrégat ou par découverte au chargement plutôt qu'en
fichier unique. C'est une décision de structure, étroite, qui ne rouvre pas les treize autres points, et sans laquelle
la garantie de non-recouvrement du plan verticalisé n'est qu'une intention. Cette décision n'est pas la mienne.

---

## 5. Arbitrages à remonter — constatés, non résolus

### 5.1 La décision marché est conditionnée à un signal qui n'existe qu'après le jalon d'après

**Mesure.** Jalon de rattachement du partage aux joueurs dans le plan, face au critère de décision de la roadmap.

```bash
grep -nE '^\| (US-UC-08|US-UC-09) ' docs/gestion-projet/plan-de-travail.md | cut -c1-95
grep -o 'H1 à H4' docs/gestion-projet/roadmap-entree-build.md | wc -l
```

Sortie brute :

```
146:| US-UC-08 — Partager une information aux joueurs | diffuser un contenu choisi vers les j
147:| US-UC-09 — Accès joueur sans compte | entrer dans une session par un simple lien, sans
1
```

**Le constat.** Les deux epics qui livrent le partage aux joueurs sont rattachées à **J3**. Or le pilier « activation
partage » — un document partagé **et consulté par au moins un joueur** — ne peut pas se constater avant elles, et deux
des quatre hypothèses que le critère de décision croise portent précisément sur le partage : H3 sur la fluidité du
partage, H4 sur l'accès joueur sans compte. Le critère de la décision marché, placé entre J1 et J2, cite donc un
signal que la séquence de jalons ne rend disponible qu'après J3.

Le même constat se lit sur une tâche : TB-053 est rattachée à **J1** et son but est que « trois compteurs anonymes
attestent l'usage réel de chacun des piliers ». Le troisième pilier porte sur un comportement dont l'interface arrive
deux jalons plus tard.

**Ce que cela coûte de ne rien faire.** La décision qui ouvre l'engagement du build cloud est adossée à un critère
partiellement inatteignable au moment où elle se prend. Elle se prendra donc de fait sur H1 et H2 seules, sans que
personne n'ait écrit que c'était la règle — et un critère qu'on applique à moitié sans le dire est un critère qui a
cessé d'être opposable.

**Ce que cette revue recommande, sans le décider.** Trois issues existent, et le choix entre elles est une décision
produit.

- **Restreindre explicitement le critère à H1 et H2**, et écrire que H3 et H4 sont constatées après J3, hors décision
  marché. C'est l'issue la moins coûteuse et elle ne change aucun jalon.
- **Avancer une fraction minimale du partage en J1**, ce qui déplace du périmètre entre jalons — donc rouvre le
  phasage, que cette revue n'a pas autorité pour rouvrir.
- **Assumer que la décision se prend sur un signal partiel** et l'écrire comme telle.

Cette revue recommande la première, pour une raison de méthode : c'est la seule qui rende le critère à nouveau
opposable sans toucher au périmètre ni au phasage. **La décision reste à prendre.**

### 5.2 Où l'ordre de valeur et l'ordre de dépendance divergent — et pourquoi ils peuvent coexister

**Mesure.** Ordre de livraison recommandé de chaque epic du périmètre J1, tel qu'écrit dans son fichier de user
stories.

```bash
for f in $(git ls-files 'docs/conception/besoin/user-stories/US-UC-0[1-7]-*.md' \
                        'docs/conception/besoin/user-stories/US-UC-14-*.md'); do
  echo "### $(basename $f)"
  awk '/^## Ordre de livraison recommandé/{f=1;next} /^## /{f=0} f' "$f" \
    | grep -oE 'US-[0-9]{2}-[0-9]{2}' | tr '\n' ' '; echo
done
```

Deux epics recommandent un ordre qui n'est pas celui de leurs identifiants, et leur justification est explicitement
de la valeur : `US-UC-03` place US-03-03 en deuxième position — « valeur immédiate pour Émilie et Nadia, coût
faible » — avant US-03-02 ; `US-UC-05` place US-05-04 en troisième — « complète le cycle de vie du dossier » — avant
US-05-03.

**Le constat, et il est utile.** Cette divergence est aujourd'hui **invisible**, parce que le grain du plan est plus
gros que la user story : une seule tâche de domaine couvre plusieurs stories, et le plan n'a donc aucun ordre à
l'échelle où la divergence existe. Le redécoupage vertical la rend **actuelle** : chaque tranche étant une user story,
le plan devra ordonner là où le corpus recommande déjà.

**Ce qui rend la coexistence possible.** Sur les deux paires mesurées, les deux stories concernées écrivent le **même
agrégat** — `Document` pour US-UC-03, `Folder` pour US-UC-05. Elles sont donc en **conflit d'écriture** et doivent
être sérialisées ; mais aucune ne **dépend** techniquement de l'autre. Or le plan ordonne par dépendance, et un
conflit n'est pas une dépendance : deux tranches en conflit sans dépendance doivent être sérialisées **sans que le
plan dise dans quel ordre**.

C'est là que l'ordre recommandé par la valeur trouve sa place, et il la trouve sans que le plan ordonne jamais par la
valeur : il départage deux tranches que la dépendance laisse indifférentes.

**Recommandation, à ratifier.** Inscrire dans le plan une règle de départage : lorsque deux tranches sont en conflit
d'écriture sans dépendance mutuelle, l'ordre de sérialisation suit la section « Ordre de livraison recommandé » de
l'epic qui les porte, citée en renvoi. La règle ne réordonne rien par la valeur — elle nomme la source qui départage
ce que la dépendance n'ordonne pas. **C'est un ajout de règle : il relève d'une décision, pas de cette revue.**

### 5.3 L'axe « par couche » de la taxonomie de libellés perd son sens pour une tranche

Constat détaillé en §3.10, points 2 et 3. À remonter parce qu'il touche un document que cette revue ne modifie pas
— la [méthode de ticket §3](methode-de-ticket.md) — et parce que la valeur de remplacement, un axe « par agrégat »
ancré sur le modèle de domaine, est un ajout à une taxonomie à valeurs fermées. La substitution est mécanique, le
choix de la faire ne l'est pas.

---

## 6. Ce que cette revue n'a pas tranché

**Décisions produit — hors de la compétence de cette revue.**

- Les quinze points non tranchés de la spécification de télémétrie, dont le seuil du geste structurant de H1
  (§4.1). Recommandation : les fermer avant l'entrée en construction, en préalable et non en dette.
- Le sort du critère de la décision marché face aux hypothèses H3 et H4 (§5.1). Recommandation : restreindre le
  critère à H1 et H2 et l'écrire.
- L'attribution d'un porteur à l'instrumentation dans la couche besoin (§4.1). Sans porteur, TB-053 reste non
  clôturable même une fois son périmètre nommé.

**Ajustements de règle — recommandés, non décidés.**

- Ne pas compter l'object store local d'un agrégat comme un module distinct de cet agrégat, faute de quoi l'échelle
  de taille sature en `L` sur les tranches — huit sur dix, mesuré (§3.10, point 5).
- Départager par la section « Ordre de livraison recommandé » deux tranches en conflit d'écriture sans dépendance
  mutuelle, que la dépendance laisse indifférentes (§5.2).
- Renforcer la quatrième condition d'entrée : un renvoi dont la cible porte un marqueur de décision non prise, pour
  le point en cause, ne satisfait pas la condition (§2.7).
- Remplacer l'axe « par couche » de la taxonomie de libellés par un axe « par agrégat » pour les tranches (§5.3).

**Décisions de structure — hors de la compétence de cette revue.**

- La disposition des points de composition de la couche Application et du store local (§4.2). Recommandation :
  la trancher avant d'appliquer le redécoupage, faute de quoi la garantie de non-recouvrement devient une intention.
  C'est le seul point de l'arborescence qui soit bloquant.
- Le nom du projet de test d'architecture, celui du projet de test de bout en bout, la plateforme d'intégration
  continue, l'emplacement des configurations de style — cinq tâches en dépendent (§2.6). Aucune n'est refermée par
  le redécoupage.
- Le nom d'un répertoire pour la couche cliente transversale, sans lequel le châssis et les bandeaux restent
  non nommables (§3.3, contre-exemple de la §3.8).

**Trous du corpus, signalés et non comblés.**

- `DocumentType` porte le qualificatif « entité de référence » et non « agrégat ». Son admission dans la maille est
  une lecture, pas une correspondance littérale, et elle doit être ratifiée (§3.2).
- La partition des règles métier par user story existe dans les tables « Vérification de couverture » mais par
  énoncé de règle, non par identifiant : elle se lit, elle ne se calcule pas (§3.9). C'est une asymétrie avec les cas
  de recette, dont la colonne `Source` porte l'identifiant.
- `CR-UC02-11` est marqué retiré et se trouve englobé par une plage que TB-013 cite, laquelle engage donc treize
  preuves quand douze existent (§2.4). Le corpus est juste ; c'est la méthode de citation par plage qui produit
  l'écart.

**Ce que cette revue recommande de laisser tel quel.**

- Les quatre propriétés mécaniques du plan — périmètre déclaré, conflit calculé, taille recalculable, critère par
  renvoi — mesurées saines et à préserver intactes (§2.8).
- La règle qui interdit de créer un identifiant d'épique neuf là où un use case porte déjà le regroupement, et sa
  raison (§3.6).
- Les épiques EP-02, EP-03, EP-05 et EP-08 à EP-12, y compris celles réduites à une seule tâche (§3.6).
- Les justifications de non-scission des tâches `L` de socle : la solution .NET, les namespaces, les object stores
  s'échafaudent chacun d'un geste solidaire, et les scinder serait un faux gain (§2.8).
- L'interdiction d'un axe de priorité dans la taxonomie de libellés, qui découle d'une non-décision explicite du
  corpus et n'a pas à être rouverte.

**Ce que cette revue n'a pas fait, par construction.**

- Aucune modification du plan de travail, de la méthode de ticket ni du guide d'entrée par fonctionnalité. Le
  redécoupage se produit en deux temps : cette revue propose, l'arbitrage tranche, l'application suit.
- Aucun réordonnancement par la valeur. Les divergences sont constatées et remontées (§5.2).
- Aucune notion de durée, de date, d'affectation, de capacité ni de vélocité. Les nombres de cette revue mesurent
  des étapes de graphe et des surfaces de vérification, jamais du temps.

---

## 7. Renvois

- Décomposition en tâches, maille de désignation, échelle de taille, lots parallélisables :
  [`plan-de-travail.md`](plan-de-travail.md)
- Condition d'entrée, taxonomie de libellés, cycle de vie d'un ticket : [`methode-de-ticket.md`](methode-de-ticket.md)
- Entrée par fonctionnalité produit : [`guide-lecture-par-fonctionnalite.md`](guide-lecture-par-fonctionnalite.md)
- Séquence de jalons, points de décision, critères de sortie : [`roadmap-entree-build.md`](roadmap-entree-build.md)
- Conventions de code, Definition of Done, conventions de maintenance du corpus :
  [`guide-conventions-et-dod.md`](guide-conventions-et-dod.md)
- Hypothèses de validation et seuils : [`vision-produit.md`](../conception/besoin/vision/vision-produit.md)
- Priorisation MoSCoW, Must Have sans use case : [`moscow.md`](../conception/besoin/vision/moscow.md)
- Agrégats, entités et value objects par bounded context : [`domain/README.md`](../conception/domain/README.md)
- Granularité des projets, périmètre du mode local, points à trancher à J0 :
  [`structure-projets.md`](../architecture/structure-projets.md)
- Cas de recette, colonne `Source`, niveaux de test : [`cahier-strategie-test-et-recette.md`](../test/cahier-strategie-test-et-recette.md)
- Table de couverture UC → fiche(s) : [`wireframes/README.md`](../conception/interface/wireframes/README.md)
- Spécification de télémétrie et ses points non tranchés : [`specs/telemetrie.md`](../architecture/specs/telemetrie.md)

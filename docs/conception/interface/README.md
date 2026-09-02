# Sous-couche interface — Haversack MVP

> Ce dossier porte la conception de l'interface du MVP : la couche réflexive (arbitrages,
> conventions, rationale) et les artefacts qui en dérivent (wireframes).
> Il ne dicte pas le besoin — les use cases font autorité. Il traduit ce besoin en surfaces décrites.

---

## Ordre d'autorité

L'ordre d'autorité général de la conception Haversack s'applique ici sans exception :

**personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire**

La sous-couche interface est en aval. Elle traduit le besoin en descriptions de surfaces — elle ne le crée pas et ne l'arbitre pas.

---

## Contenu du dossier

### Couche réflexive

| Fichier | Rôle | Statut |
|---|---|---|
| [`zoning.md`](zoning.md) | Ossature de navigation, inventaire des écrans (S4), couverture UC (S5), 21 arbitrages AR-01..AR-21 (S6), châssis applicatif (S7), exclusions (S8), trous de corpus (S9) | Produit — source de vérité des décisions d'interface |
| [`conventions-wireframe.md`](conventions-wireframe.md) | Conventions de notation et de nommage pour les fiches d'écran : deux registres (domaine / région d'interface), légende de 10 familles de marqueurs | Produit |
| [`gabarit-ecran.md`](gabarit-ecran.md) | Structure de toute fiche de description d'écran basse-fidélité + exemple-pilote sur la vue session MJ | Produit |
| [`reflexion-ux-mvp.md`](reflexion-ux-mvp.md) | Rationale de conception d'interface : 4 lentilles d'analyse appliquées à l'ensemble des écrans du MVP (liste propriétaire : [`wireframes/README.md`](wireframes/README.md)), points ouverts et décisions actées | Produit |

### Artefact dérivé

Une **fiche de wireframe** décrit *un écran* — son contenu, ses régions, ses règles. Toutes ne
décrivent pas une surface pleine : certaines portent une sur-couche invoquée sans quitter l'écran
hôte, ou une région transversale du châssis. Aucune ne fait autorité sur la navigation : la source
de vérité du graphe reste [`zoning.md`](zoning.md), dont les fiches sont une instanciation.

| Dossier | Rôle | Statut |
|---|---|---|
| [`wireframes/`](wireframes/README.md) | Une fiche basse-fidélité par écran du périmètre MVP, instanciant le gabarit ; index et table de couverture UC→écran | Produit — 20 écrans |

### Assets d'identité (hors périmètre de ce dossier)

Logo, charte graphique (couleurs, typographie, grille) et ressources d'interface (icônes, composants visuels) relèvent d'une phase de design postérieure à la conception. Aucun de ces assets n'est produit à ce stade.

---

## Séquence de lecture

1. **`zoning.md`** — source de vérité des décisions d'interface (arbitrages, inventaire, châssis). Tout ce qui est décidé y est tracé.
2. **`conventions-wireframe.md`** — à lire avant de produire ou de lire une fiche d'écran. Définit les deux registres de nommage et la légende de notation.
3. **`gabarit-ecran.md`** — structure à respecter pour chaque fiche. L'exemple-pilote sur la vue session MJ illustre l'utilisation complète du gabarit.
4. **`reflexion-ux-mvp.md`** — rationale des décisions consignées dans `zoning.md` et les fiches ; n'introduit aucune règle nouvelle.
5. **[`wireframes/`](wireframes/README.md)** — les fiches d'écran, dans l'ordre de priorité de l'inventaire S4 (vue session MJ en premier). La table de couverture UC→écran vit dans l'index de ce dossier.

---

## Ce que ce dossier ne fait pas

- Il ne re-prescrit pas les arbitrages AR-01..AR-21 — il les référence.
- Il n'invente pas de règles métier — celles-ci viennent des use cases.
- Il ne contient aucun asset d'identité visuelle final (logo, charte graphique, ressources d'interface) — ceux-ci relèvent d'une phase de design postérieure à la conception.

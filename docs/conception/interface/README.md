# Sous-couche interface — Haversack MVP

> Couche réflexive. Ce dossier porte la conception de l'interface du MVP avant la production des wireframes.
> Il ne dicte pas le besoin — les use cases font autorité. Il documente la forme que prend la traduction de ce besoin en surfaces décrites.

---

## Ordre d'autorité

L'ordre d'autorité général de la conception Haversack s'applique ici sans exception :

**personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire**

La sous-couche interface est en aval. Elle traduit le besoin en descriptions de surfaces — elle ne le crée pas et ne l'arbitre pas.

---

## Contenu du dossier

| Fichier | Rôle | Statut |
|---|---|---|
| [`zoning.md`](zoning.md) | Ossature de navigation, inventaire des écrans (S4), couverture UC (S5), 21 arbitrages AR-01..AR-21 (S6), châssis applicatif (S7), exclusions (S8), trous de corpus (S9) | Validé — à lire en premier |
| [`conventions-wireframe.md`](conventions-wireframe.md) | Conventions de notation et de nommage pour les fiches d'écran : deux registres (domaine / région d'interface), légende de 10 familles de marqueurs | Produit — vague 2 |
| [`gabarit-ecran.md`](gabarit-ecran.md) | Structure de toute fiche de description d'écran basse-fidélité + exemple-pilote sur la vue session MJ | Produit — vague 2 |
| Wireframes (à venir) | Une fiche par écran de l'inventaire S4, instanciant le gabarit | Vague 3 |

---

## Séquence de lecture

1. **`zoning.md`** — source de vérité des décisions d'interface (arbitrages, inventaire, châssis). Tout ce qui est décidé y est tracé.
2. **`conventions-wireframe.md`** — à lire avant de produire ou de lire une fiche d'écran. Définit les deux registres de nommage et la légende de notation.
3. **`gabarit-ecran.md`** — structure à respecter pour chaque fiche. L'exemple-pilote sur la vue session MJ illustre l'utilisation complète du gabarit.
4. **Wireframes (vague 3)** — une fiche par écran, dans l'ordre de priorité de l'inventaire S4 (vue session MJ en premier).

---

## Ce que ce dossier ne fait pas

- Il ne re-prescrit pas les arbitrages AR-01..AR-21 — il les référence.
- Il n'invente pas de règles métier — celles-ci viennent des use cases.
- Il ne contient pas de wireframes finaux ni de maquettes visuelles.
- Il ne modifie pas `docs/conception/README.md` (index racine — réservé à une mise à jour ultérieure).

# Roadmap juridique — préalables au lancement EU

> **CADRE À VALIDER JURISTE — non contractuel, séquencement de conception.**
> Ce document liste et ordonne les livrables juridiques identifiés par le corpus de conception comme préalables au lancement EU. Il ne tranche aucune base légale, aucune qualification Art. 28, aucune clause. Chaque jalon est marqué **à produire** ou **à valider juriste** et renvoie à sa source de conception. Le statut et le contenu final de chaque jalon relèvent de l'équipe juridique.

---

## Pourquoi cette roadmap existe

Le corpus de conception (ADR-012, ADR-013, ADR-018) a identifié, au fil des décisions RGPD, une série de points qui **ne peuvent pas être tranchés en conception** — soit parce qu'ils relèvent d'une qualification juridique (Art. 8, Art. 28, mise en balance Art. 6§1(f), Art. 17§3(e)), soit parce qu'ils nécessitent un document contractuel (DPA) ou un document d'information (politique de confidentialité, mentions Art. 13/14) que la conception ne produit pas.

Ces points sont dispersés dans plusieurs ADR. Cette roadmap les consolide en une liste ordonnée, pour donner une vue unique de ce qui doit être produit ou validé avant tout lancement EU, et de l'ordre dans lequel ces livrables s'enchaînent logiquement.

---

## Vue d'ensemble — séquencement

L'ordre proposé ci-dessous suit une logique de dépendance : un jalon en amont fournit la matière ou la qualification nécessaire au jalon suivant. Le séquencement est une **proposition de conception**, à confirmer par l'équipe juridique qui peut avoir un ordre de préférence différent (par exemple mener certaines validations en parallèle).

| # | Jalon | Statut | Dépend de |
|---|---|---|---|
| 1 | Base légale confirmée (par traitement) | à valider juriste | — (point d'entrée) |
| 2 | Cadrage 5 axes validé juriste | à valider juriste | Jalon 1 (partiellement) |
| 3 | DPA finalisé (Art. 28) | à produire | Jalon 2 (qualification sous-traitant) |
| 4 | Politique de confidentialité | à produire | Jalons 1, 2, 3 |
| 5 | Mentions Art. 13/14 (points de collecte) | à produire | Jalon 4 |
| 6 | Registre des traitements (Art. 30), si applicable | à produire / à valider juriste | Jalons 1-4 |

---

## Jalon 1 — Base légale confirmée (par traitement)

**Statut : à valider juriste.**

**Contenu attendu** : confirmation juridique, traitement par traitement, de la base légale retenue en conception. Deux bases légales sont proposées par la conception, sous réserve de validation :

- `display_name` des invités : intérêt légitime (Art. 6§1(f)) — la mise en balance formelle (nécessité, proportionnalité, attentes raisonnables) **n'a pas été conduite par un juriste** ; ADR-013 §1 en propose une esquisse résumée, non normative.
- Logs et métadonnées techniques (IP, timestamps) : intérêt légitime (Art. 6§1(f)), finalité sécurité et débogage — même réserve.
- Conservation des documents partagés au titre de l'Art. 17§3(e) (ADR-012 §3(c)) : intérêt légitime, mise en balance formelle non conduite par un juriste.

**Dépendances** : aucune — c'est le point d'entrée de la roadmap. Certains volets de ce jalon peuvent être menés en parallèle du jalon 2.

**Sources conception** : ADR-013 §1 ; ADR-013 *Conformité conçue, non certifiée* point 3 ; ADR-012 §3(c) et *Conformité conçue, non certifiée* point 3.

---

## Jalon 2 — Cadrage 5 axes validé juriste

**Statut : à valider juriste.**

**Contenu attendu** : ADR-018 consolide explicitement 5 axes de validation juridique pré-lancement EU dans un document dédié produit sous `docs/securite/conformite/cadrage-validation-pre-lancement-eu.md`. Les 5 axes identifiés par la conception sont :

1. **Art. 8 (mineurs)** — Haversack est-il un service « destiné aux enfants » au sens du RGPD et des droits nationaux applicables ? Si oui, l'attestation d'âge 16+ actuellement retenue est insuffisante.
2. **Art. 28 + périmètre DPA** — la qualification sous-traitant est-elle confirmée pour les contenus MJ décrivant des tiers identifiables, et pour quel périmètre d'espace (partagé, personnel, ou les deux) ?
3. **Mise en balance de l'intérêt légitime** — `display_name` des invités (ADR-013 §1) et conservation des documents partagés (ADR-012 §3(c)).
4. **Art. 17 du hard-delete inconditionnel de l'espace `PERSONAL`** — la thèse d'absence de tiers justifiant un hard-delete inconditionnel est-elle confirmée, y compris pour les cas de document déplacé entre espace personnel et espace partagé autour de `deletion_requested_at` ?
5. **Facette RGPD de l'Option A « reclaim-in-place »** — cadrage de la conformité de la mécanique de récupération d'un espace (transférer de facto la responsabilité à un tiers candidat, sans qu'il ait le droit d'accès initial) pour les données à titre personnel du détenteur sortant, y compris les contenus personnels décrivant d'autres tiers.

**Dépendances** : recoupe partiellement le jalon 1 (l'axe 3 est aussi un volet du jalon 1) ; conditionne le jalon 3 (le DPA ne peut être finalisé tant que l'axe 2 n'est pas tranché).

**Sources conception** : ADR-018, *Conformité conçue, non certifiée* et section *Consolidation — cadrage de validation pré-lancement EU* ; ADR-012 *Conformité conçue, non certifiée* (4 points, numérotation parallèle mais non strictement identique aux 5 axes ADR-018) ; ADR-013 *Conformité conçue, non certifiée* (3 points) ; les travaux d'identité et d'accès joueur (Option A reclaim-in-place).

---

## Jalon 3 — DPA finalisé (Art. 28)

**Statut : à produire.**

**Contenu attendu** : rédaction et validation juriste du Data Processing Agreement dont le squelette de sections est fourni par [`dpa-skeleton.md`](./dpa-skeleton.md) (objet et durée, nature et finalités, catégories de données et de personnes, obligations Art. 28§3 a-h, sous-traitants ultérieurs, mesures de sécurité, assistance aux droits, sort des données en fin de contrat, droits d'audit).

**Dépendances** : nécessite que le jalon 2 (axe 2 — qualification sous-traitant/périmètre) soit tranché, faute de quoi le périmètre exact du DPA (espaces partagés seuls, ou aussi espace `PERSONAL`) reste incertain.

**Sources conception** : ADR-013 §5 ; ADR-013 Conséquences (« la roadmap juridique doit inclure la rédaction du DPA ») ; ADR-018 *Conformité conçue, non certifiée* point 2.

---

## Jalon 4 — Politique de confidentialité

**Statut : à produire.**

**Contenu attendu** : document d'information générale couvrant, a minima, les éléments déjà identifiés par la conception comme devant y figurer :

- Base légale et durée de rétention du `display_name` des invités et des métadonnées techniques (ADR-013 §1, §3).
- Droits des invités et modalités d'exercice (ADR-013 §2).
- Posture sous-traitant pour les contenus MJ (renvoi au DPA, jalon 3).
- Sort des données à la suppression de compte et d'espace (ADR-011, ADR-012 §2).

**Dépendances** : s'appuie sur les jalons 1 (bases légales confirmées) et 3 (DPA finalisé, pour la cohérence de la mention sous-traitant) — une politique de confidentialité rédigée avant confirmation de ces jalons devra être révisée si les qualifications changent.

**Sources conception** : ADR-013 Conséquences (« la politique de confidentialité doit couvrir... ») ; ADR-013 §2 (table de mention Art. 13, colonne « lien vers la politique de confidentialité »).

---

## Jalon 5 — Mentions Art. 13/14 (points de collecte)

**Statut : à produire.**

**Contenu attendu** : mise en œuvre des mentions d'information au(x) point(s) de collecte identifiés en conception, en particulier :

- Formulaire de saisie du `display_name` invité (ADR-013 §2) : identité du responsable de traitement, finalité, base légale, durée de conservation, droits et modalités d'exercice — contenu minimum déjà esquissé en ADR-013 §2 (table), formulation finale à valider juriste.
- Case d'attestation d'âge 16+ au même formulaire (ADR-013 §4) — dépend de l'issue du jalon 2, axe 1 (Art. 8).

**Dépendances** : s'appuie sur le jalon 4 (la mention renvoie à la politique de confidentialité) ; le contenu de l'attestation d'âge dépend de l'issue de l'axe 1 du jalon 2.

**Sources conception** : ADR-013 §2 (table de mention Art. 13) ; ADR-013 §4 (attestation d'âge) ; ADR-013 Conséquences (« le formulaire de saisie du `display_name` doit intégrer la mention Art. 13 et la case d'attestation d'âge avant le lancement EU »).

---

## Jalon 6 — Registre des traitements (Art. 30), si applicable

**Statut : à produire / à valider juriste.**

**Contenu attendu** : détermination préalable par le juriste de l'applicabilité de l'obligation de tenue d'un registre Art. 30 (selon la taille de l'organisation et la nature des traitements), puis, si applicable, production du registre couvrant a minima les traitements déjà cartographiés par la conception (données invités, données de compte, contenus narratifs des espaces, logs techniques).

**Dépendances** : s'appuie sur les jalons 1 et 2 pour la description exacte des finalités et bases légales à y faire figurer. Peut être mené en parallèle des jalons 4-5 une fois l'applicabilité confirmée.

**Sources conception** : aucune source ADR ne mentionne explicitement le registre Art. 30 — ce jalon est ajouté par cette roadmap en cohérence avec les obligations RGPD générales listées par le mandat de cette tâche ; **son applicabilité et son contenu ne sont pas tranchés en conception et doivent être déterminés par le juriste**.

---

## Points connexes non repris comme jalons

Au-delà des jalons 1-6 ci-dessus, plusieurs points connexes de conformité RGPD, déjà identifiés par le corpus de conception, ne sont pas repris comme jalons distincts de cette roadmap (hors périmètre du DPA/politique de confidentialité au sens strict), mais leur issue peut affecter la rédaction des jalons 3-4 :

- Adresse email support + procédure d'exercice des droits des invités (ADR-013 §2, §189).
- Sort des blobs/médias externalisés dans les deux chemins de purge (ADR-011).
- Extension de délai Art. 12§3 et canal de notification alternatif (ADR-012).
- Facette RGPD de l'Option A « reclaim-in-place » (cf. ADR-015 §2.3 et [`cadrage-validation-pre-lancement-eu.md`](./cadrage-validation-pre-lancement-eu.md), axe 5).

Cette roadmap ne reprend pas ces points en jalons distincts, mais signale leur dépendance potentielle sur les jalons 3 et 4 ci-dessus : une procédure de droits non stabilisée peut faire évoluer la politique de confidentialité après une première rédaction.

---

## Renvois croisés

| Jalon | Source(s) conception |
|---|---|
| 1 — Base légale | ADR-013 §1 ; ADR-012 §3(c) |
| 2 — Cadrage 5 axes | ADR-018 (consolidation) ; ADR-012 ; ADR-013 |
| 3 — DPA | ADR-013 §5 ; ADR-018 point 2 ; [`dpa-skeleton.md`](./dpa-skeleton.md) |
| 4 — Politique de confidentialité | ADR-013 Conséquences |
| 5 — Mentions Art. 13/14 | ADR-013 §2, §4 |
| 6 — Registre Art. 30 | ajouté par cette roadmap, applicabilité non tranchée |
| Points connexes non repris comme jalons | ADR-013 §2, §189 ; ADR-011 ; ADR-012 ; ADR-015 §2.3 |

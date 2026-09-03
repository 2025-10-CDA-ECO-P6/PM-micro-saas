# VO `DocumentProperties` & `propertiesSchema` par type système — cluster données & persistance

- **Statut** : Spec pré-build — à confirmer à l'entrée en build.
- **Sources** : [ADR-002](../decisions/ADR-002-tout-est-document-gouvernance.md) (l.24, 26, 46, 60, 62).
- **Companion domaine (lecture seule)** : [content-library.md](../../conception/domain/content-library.md), [diagrams/mld/content-library.md](../../conception/domain/diagrams/mld/content-library.md).
- **Périmètre** : le contrat du value object `DocumentProperties`, le régime de validation par catégorie de type, et l'énumération des 8 types système seedés. Pour les champs concrets de chaque `propertiesSchema`, cette note dérive fidèlement ce que le domaine modélise déjà et marque `[À TRANCHER — modélisation domaine]` ce qu'il ne modélise pas encore. **ADR-002 ne fixe pas les champs — ils ne sont pas inventés ici.**

---

## 1. Contrat du value object `DocumentProperties`

**Décision reportée** (ADR-002:22) :

> `Document.SetProperties()` accepte uniquement un `DocumentProperties` validé contre le `propertiesSchema` du type de document concerné. Les écritures directes dans le JSON sans passer par ce VO sont interdites.

Conséquences directes du contrat, reportées fidèlement :

- `DocumentProperties` est le point de passage **obligatoire** de toute écriture dans `documents.properties`. Il n'existe pas de chemin d'écriture alternatif (pas de setter direct sur la colonne, pas de patch JSON brut).
- La validation se fait **contre le `propertiesSchema` du type de document concerné** — c'est-à-dire le `propertiesSchema` porté par l'entité `DocumentType` associée au `documentTypeId` du `Document` (content-library.md:181, diagrams/mld/content-library.md:31 : `document_types.properties_schema jsonb, nullable`).
- Un `Document` sans `documentTypeId` (le champ est optionnel, content-library.md:67) n'a pas de `propertiesSchema` à valider contre. **Tranché** (décision d'entrée en build du 2026-09-03) : **`properties` n'existe pas sans type** — `SetProperties()` sur un document sans `documentTypeId` est refusé. Un `properties` accepté sans schéma serait un chemin d'écriture non gouverné, ce que le caractère obligatoire du point de passage interdit. Le contenu libre du document n'est pas concerné : il vit dans ses blocs.

---

## 2. Deux régimes de validation

**Décision reportée** (ADR-002:24, 62) :

- **Types système** (MVP) : « schéma figé seedé en base ». Pas de `properties` libre non validé au démarrage — chaque type système dispose d'un schéma déclaré qui contraint les valeurs acceptables. Le schéma figé s'applique **aux types système non modifiés**.
- **Types custom ou modifiés** : « la validation est permissive par défaut — choix délibéré pour ne pas bloquer l'extensibilité » (ADR-002:60).

Ces deux régimes sont des décisions distinctes et déjà tranchées — il n'y a pas de point ouvert sur *le principe* de la distinction. Le contenu exact de la validation « permissive par défaut » (ex. accepte tout JSON valide sans contrainte de forme, ou applique un schéma minimal générique) n'est pas détaillé par ADR-002 : `[À TRANCHER — modélisation domaine]`.

---

## 3. `DocumentBlock.content` : hors périmètre du VO

**Décision reportée** (ADR-002:58) :

> `DocumentBlock.content` est du contenu éditeur, sans enjeu d'intégrité référentielle. Ce choix est explicite et distinct de `properties` (gouverné par `DocumentProperties`).

`DocumentBlock.content` n'est **pas** soumis au contrat de validation de `DocumentProperties` — c'est une décision explicite de non-gouvernance, pas un oubli. Ce champ reste hors périmètre de cette spec.

---

## 4. Énumération des 8 types système seedés

**Modélisation domaine trouvée** (content-library.md:179, 185 ; diagrams/mld/content-library.md:40) — les deux sources convergent sur la même liste de 8 types système, seedés en base à `is_system = true`, `space_id = NULL` :

| # | Slug | Nom (source) |
|---|---|---|
| 1 | `scenario` | SCÉNARIO |
| 2 | `scene` | SCÈNE |
| 3 | `npc` | PNJ |
| 4 | `location` | LIEU |
| 5 | `note` | NOTE |
| 6 | `player_character` | PERSONNAGE JOUEUR |
| 7 | `live_note` | NOTE LIVE |
| 8 | `reveal` | RÉVÉLATION |

Contraintes d'unicité du `slug` pour ces types système : `UNIQUE (slug) WHERE space_id IS NULL` (diagrams/mld/content-library.md:37).

---

## 5. Champs de `propertiesSchema` par type — dérivés du domaine ou marqués à trancher

### `scenario` — dérivé du domaine

Le domaine modélise explicitement le champ structuré de ce type (content-library.md:198-210) :

| Propriété structurée | Type | Valeurs | Description |
|---|---|---|---|
| `scenarioStatus` | `enum` | `DRAFT` \| `READY` \| `PLAYED` \| `ARCHIVED` | État éditorial du scénario, géré par le MJ. Valeur par défaut à la création : `DRAFT`. Distinct du `status` d'espace (Space Management) et du `status` de session (Session Conduct). |

Ce champ est porté par `Document.properties` et n'est accessible qu'en présence de `documentTypeId = SCENARIO` (content-library.md:208-209).

### `live_note` — dérivé du domaine (absence explicite)

Le domaine précise explicitement (content-library.md:190-194) :

> Les champs domaine `characterId` et `guestAccessId` du Document sont promus au niveau des champs de premier niveau (ADR-002) — ils ne font plus partie des propriétés structurées. Aucune propriété structurée supplémentaire spécifique au type `live_note` n'est définie pour le MVP.

`propertiesSchema` de `live_note` : aucun champ structuré au MVP — pas un trou, une absence actée.

### `scene`, `npc`, `location`, `note`, `player_character`, `reveal` — aucun champ structuré au MVP

Ces 6 types système ne font l'objet d'aucune section « Propriétés structurées du type `X` » dans `content-library.md`, contrairement à `scenario` et `live_note`.

**Tranché** (décision d'entrée en build du 2026-09-03) : `propertiesSchema` de ces 6 types est **vide au MVP** — aucun champ structuré. C'est une **absence actée, non un trou**, sur le précédent que `live_note` porte déjà dans cette même spécification. Leur contenu vit dans les blocs du document, ce que le principe « tout est document » rend possible sans propriété structurée.

Cette décision n'interdit rien pour la suite : un type custom peut porter un `propertiesSchema` dès le MVP, sous le régime permissif du § 2, et ces 6 types pourront en recevoir un après le MVP sans rupture de contrat.

---

## Synthèse des trous nommés

| # | Type système | `propertiesSchema` |
|---|---|---|
| 1 | `scenario` | Dérivé — `scenarioStatus` (enum 4 valeurs) |
| 2 | `scene` | `[À TRANCHER — modélisation domaine]` |
| 3 | `npc` | `[À TRANCHER — modélisation domaine]` |
| 4 | `location` | `[À TRANCHER — modélisation domaine]` |
| 5 | `note` | `[À TRANCHER — modélisation domaine]` |
| 6 | `player_character` | `[À TRANCHER — modélisation domaine]` |
| 7 | `live_note` | Dérivé — aucun champ structuré au MVP (absence actée, `characterId`/`guestAccessId` promus hors `properties`) |
| 8 | `reveal` | `[À TRANCHER — modélisation domaine]` |

Points ouverts additionnels :

- Comportement de `SetProperties()` sur un `Document` sans `documentTypeId` — non fixé par ADR-002.
- Contenu exact de la validation « permissive par défaut » pour les types custom/modifiés — le principe est acté (ADR-002:60), sa forme précise ne l'est pas.

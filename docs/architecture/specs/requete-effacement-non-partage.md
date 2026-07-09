# Requête « document non partagé » — RGPD effacement de compte, §3(b)

- **Statut** : Spec pré-build — illustrative, non normative. ADR-012 renvoie explicitement l'écriture précise de cette requête à l'implémentation (« la définition donnée ici est fonctionnelle ; la requête SQL précise est à écrire lors de l'implémentation », ADR-012:189, section *Points à trancher*). Cette note produit une première formulation fidèle au critère fixé, sans le clore.
- **Sources** : [ADR-012](../decisions/ADR-012-rgpd-effacement-compte.md) (l.92, 94, 118-124, 189) ; matrice FK [ADR-011](../decisions/ADR-011-cascade-integrite-referentielle.md) (§Schéma et MLD).
- **Périmètre** : la sélection des documents « non partagés » au sens de l'étape 5 de la saga `UserAnonymized` (ADR-012 §3(b)), l'instant de référence de l'évaluation, et le séquençage déliaison-puis-DELETE aligné sur le mécanisme de saga ADR-011.

---

## 1. Critère métier — document non partagé (ADR-012 §3(b))

**Décision reportée** (ADR-012:88-92) :

> Les documents créés par l'utilisateur (`created_by_id = userId`) qui ne sont visibles que par lui (non partagés avec d'autres membres, non utilisés comme modèle par d'autres documents) sont supprimés.
>
> **Critère opérationnel** : un document est considéré « non partagé » s'il ne figure dans aucun `session_pinned_documents`, aucun `document_links.source_document_id` ou `target_document_id` référencé depuis un document d'un autre utilisateur, et n'a pas été instancié (`source_document_id`) par un autre document.

Ce critère se décompose en trois conditions cumulatives (ET), toutes évaluées sur un document `d` tel que `d.created_by_id = userId` :

| # | Condition | Table(s) impliquée(s) |
|---|---|---|
| C1 | `d` ne figure dans aucun `session_pinned_documents` | `session_pinned_documents.document_id` |
| C2 | `d` n'est référencé — comme source ou cible — par aucun `document_links` **depuis un document d'un autre utilisateur** (`created_by_id != userId`) | `document_links.source_document_id` / `.target_document_id`, jointure sur `documents.created_by_id` |
| C3 | `d` n'a pas été instancié par un autre document (`documents.source_document_id` d'un autre document ne pointe pas vers `d`) | `documents.source_document_id` |

**Périmètre d'application** : ce critère s'applique au contenu des **espaces partagés vivants** (CAMPAIGN, ONE_SHOT), par distinction explicite avec le contenu de l'espace `PERSONAL` qui relève d'un hard-delete inconditionnel séparé (ADR-012 §3, l.77 — hors périmètre de cette note).

---

## 2. Instant de référence

**Décision reportée** (ADR-012:94) :

> Le critère de partage est évalué à `deletion_requested_at` (horodatage de la demande, enregistré conformément à §6), et non au moment de l'exécution effective de la saga. Cette précision prévient un risque de race TOCTOU de conformité : si l'état de partage d'un document évoluait entre la demande et l'exécution (ex. partage retiré par un tiers après la demande), l'évaluation à `deletion_requested_at` garantit la cohérence juridique de la sélection.

**Conséquence pour la requête** : la sélection ne doit pas s'appuyer sur l'état courant des tables au moment de l'exécution de la saga, mais sur un instantané figé à `deletion_requested_at`. ADR-012 §6 (l.145-146) impose l'horodatage de la demande (`deletion_requested_at`) mais **ne fixe pas le mécanisme technique de figement** (snapshot des tables, table d'audit historisée, ou requête différée exécutée immédiatement à la réception de la demande plutôt qu'au traitement effectif).

`[À TRANCHER — ticket]` : le mécanisme de figement de l'état à `deletion_requested_at` (snapshot vs exécution immédiate de la sélection à la réception de la demande) n'est pas arbitré par ADR-012 — la requête ci-dessous illustre le critère métier, pas ce mécanisme de figement.

---

## 3. Requête SQL — illustrative, balisée

```sql
-- ILLUSTRATIF — non normatif (ADR-012:189 : la requête précise reste un point à trancher à l'implémentation).
-- Sélection des documents "non partagés" au sens ADR-012 §3(b), pour un espace partagé vivant donné.
-- Le mécanisme de figement à `deletion_requested_at` (§2 ci-dessus) n'est pas représenté ici : cette
-- requête illustre le critère métier des trois conditions C1/C2/C3, pas l'instantané temporel.

SELECT d.id
FROM documents d
WHERE d.created_by_id = :userId
  -- C1 : absent de tout session_pinned_documents
  AND NOT EXISTS (
        SELECT 1
        FROM session_pinned_documents spd
        WHERE spd.document_id = d.id
      )
  -- C2 : non référencé (source ou cible) par un document_links d'un document d'un autre utilisateur
  AND NOT EXISTS (
        SELECT 1
        FROM document_links dl
        JOIN documents other
          ON other.id = CASE
                           WHEN dl.source_document_id = d.id THEN dl.target_document_id
                           WHEN dl.target_document_id = d.id THEN dl.source_document_id
                         END
        WHERE (dl.source_document_id = d.id OR dl.target_document_id = d.id)
          AND other.created_by_id <> :userId
      )
  -- C3 : non instancié (source_document_id) par un autre document
  AND NOT EXISTS (
        SELECT 1
        FROM documents instances
        WHERE instances.source_document_id = d.id
      );
```

---

## 4. Séquençage — déliaison puis DELETE, aligné saga ADR-011

**Décision reportée** — ADR-012 §4 (l.118-124) décrit ce séquençage pour la sélection §3(a) (`PLAYER_PRIVATE`) et le qualifie de « conforme au mécanisme de saga de déliaison-puis-delete défini dans ADR-011 ». Le mandat de cette note applique le même mécanisme à la sélection §3(b) :

1. **Déliaison** des FK nullable portées par les documents sélectionnés eux-mêmes : `source_document_id`, `character_id`, `guest_access_id` (colonnes nullable de `documents`, ADR-011 matrice l.129-131).
2. **DELETE des tables feuilles NOT NULL** qui référencent les documents sélectionnés : `document_links` (source ou cible), `document_tags`, `document_blocks` — conformément à l'ordre topologique passe 2 d'ADR-011 (feuilles avant racines).
3. **DELETE des documents** eux-mêmes.

```sql
-- ILLUSTRATIF — non normatif (ADR-012:189 : la requête précise reste un point à trancher à l'implémentation).
-- Étape 1 — déliaison des FK nullable portées par les documents sélectionnés
UPDATE documents SET source_document_id = NULL WHERE id = ANY(:selectedIds);
UPDATE documents SET character_id       = NULL WHERE id = ANY(:selectedIds);
UPDATE documents SET guest_access_id    = NULL WHERE id = ANY(:selectedIds);

-- Étape 2 — DELETE des tables feuilles NOT NULL référençant les documents sélectionnés
DELETE FROM document_links  WHERE source_document_id = ANY(:selectedIds) OR target_document_id = ANY(:selectedIds);
DELETE FROM document_tags   WHERE document_id = ANY(:selectedIds);
DELETE FROM document_blocks WHERE document_id = ANY(:selectedIds);

-- Étape 3 — DELETE des documents eux-mêmes
DELETE FROM documents WHERE id = ANY(:selectedIds);
```

Cette séquence respecte le principe général d'ADR-011 (§2, l.36-42) : seules les FK nullable sont NULL-ées ; les FK NOT NULL imposent l'ordre de DELETE (enfant avant parent), jamais une déliaison.

---

## 5. Trou nommé — références nullable entrantes non couvertes par le critère §3(b)

Le critère opérationnel d'ADR-012 §3(b) couvre trois chemins précis (C1/C2/C3, §1 ci-dessus). Il ne couvre **pas** explicitement d'autres colonnes nullable du graphe ADR-011 qui peuvent référencer un document sélectionné :

- `documents.character_id` d'un **autre** document pointant vers le document sélectionné (si celui-ci est de type `player_character`).
- `guest_accesses.character_id` pointant vers le document sélectionné.
- `folders.default_template_document_id` pointant vers le document sélectionné.

Si l'une de ces références existe sur un document par ailleurs qualifié « non partagé » par C1/C2/C3, la séquence de l'étape 2 ci-dessus (qui ne NULL-e que les colonnes portées par le document sélectionné lui-même) laisserait une FK `RESTRICT` non résolue, bloquant le DELETE de l'étape 3.

`[À TRANCHER — ticket]` : ADR-012 §3(b) ne tranche pas si ces trois chemins de référence entrante doivent (a) exclure le document du périmètre « non partagé » (élargissement du critère C2), ou (b) être déliés en étape 1 au même titre que les colonnes propres au document. Ce point n'est pas arbitré dans le corpus source consulté et n'est pas inventé ici.

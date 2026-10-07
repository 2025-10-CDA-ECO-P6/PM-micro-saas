# 0073 — Aligner cahier-specifications-techniques.md sur ses sources (6 écarts)

- **Id** : 0073
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-reference-projet (chantier conformite-corpus)
- **Appliqué dans** : `docs/reference/cahier-specifications-techniques.md` (§ 3 à § 9, § 11 et § 12), `docs/architecture/specs/document-properties-schemas.md` (consommateur, « Synthèse des trous nommés »), `docs/projet/registre-des-risques.md` (consommateur, T-14 et T-15)
- **Conclusion** : Les six écarts sont appliqués. Alignés dans le même document, par cohérence : le corps du § 5 (sanitisation), du § 4 (types système), du § 7 (méthode d'écriture) et du § 9 (télémétrie), qui répétaient les mêmes points ; l'attribut de nature d'ADR-001 est corrigé dans les quatre tableaux qui le portent, celui d'ADR-018 dans les trois. Le § 9 ne garde ouvert que le schéma d'événements formalisé, conformément à `docs/architecture/specs/telemetrie.md` § 6. Consommateur aligné : registre des risques, T-14 et T-15 clos (décision de l'orchestrateur, précédent T-06).

- [technical-spec:contradicted-1 — stale] Marquer tranchés dans docs/context/cahier-specifications-techniques.md:590 et suivantes N et l'usage réel, avec renvoi à moscow.md
- [technical-spec:contradicted-2 — stale] Marquer docs/context/cahier-specifications-techniques.md:550 tranché : propertiesSchema vide au MVP, renvoi à document-properties-schemas.md §5
- [technical-spec:contradicted-3 — stale] Marquer docs/context/cahier-specifications-techniques.md:580 tranché : écriture refusée sans type, renvoi à document-properties-schemas.md §1
- [technical-spec:contradicted-4 — stale] Réduire docs/context/cahier-specifications-techniques.md:560 au seul point encore ouvert côté serveur, avec renvoi à sanitisation-csp.md §5
- [technical-spec:contradicted-5 — stale] Remplacer pré-implémentation par conception pour ADR-001 dans docs/context/cahier-specifications-techniques.md:631
- [technical-spec:contradicted-6 — stale] Remplacer conception par mixte (dominante pré-implémentation) pour ADR-018 dans docs/context/cahier-specifications-techniques.md:648

**Raison.** Contradictions triées stale/gap, tri validé en bloc par l'opérateur le 2026-10-07, regroupées par document à corriger

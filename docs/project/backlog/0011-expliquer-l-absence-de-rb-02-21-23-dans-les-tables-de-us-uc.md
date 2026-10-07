# 0011 — Expliquer l'absence de RB-02-21..23 dans les tables de US-UC-02

- **Id** : 0011
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-besoin (chantier conformite-corpus)
- **Appliqué dans** : docs/besoin/stories/US-UC-02-creer-espace-jeu.md
- **Conclusion** : trou connu, pas une coupe volontaire. RB-02-21..23 ont été ajoutées à UC-02 le 2026-09-02 (commit f4214d1), qui retouchait aussi US-UC-02 et la matrice de priorités sans les y reprendre ; aucun commit ne décide leur retrait. Consignée dans US-UC-02, section Vérification de couverture (marqueur SOUS-SPÉCIFIÉ).

Expliquer l'absence de RB-02-21..23 dans les tables de US-UC-02. Elles sont définies dans UC-02 mais absentes des tables de couverture (US-UC-02:428-439) : trou connu ou coupe volontaire ? (git log -p sur UC-02 et moscow)

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

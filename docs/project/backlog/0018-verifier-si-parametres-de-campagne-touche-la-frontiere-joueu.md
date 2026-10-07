# 0018 — Vérifier si Paramètres de campagne touche la frontière joueur

- **Id** : 0018
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : docs/interface/ecrans/sv3-preparation/parametres-campagne/parametres-campagne.md
- **Conclusion** : la fiche touche la frontière joueur (AR-03) : le lien d'invitation de campagne décide qui entre dans l'espace (AR-10 révisé ; UC-11 §Scénario nominal) et l'archivage change l'état d'un espace dont les joueurs sont membres. La section conditionnelle « Partage et visibilité » est ajoutée (règle de séparation, indicateur de partage et affordances de partage et d'épinglage absents par nature, lien d'invitation, archivage). Un point n'est pas dit par les sources et reste marqué SOUS-SPÉCIFIÉ dans la fiche : ce que voit un joueur déjà membre d'un espace ARCHIVED, et la validité du lien d'invitation en cours (ni UC-02, UC-09, UC-11, UC-12, ni le domaine ne le disent ; `content-library` ne fait que passer les documents en lecture seule).

Vérifier si Paramètres de campagne touche la frontière joueur. Archivage et lien d'invitation permanent, sans section Partage / visibilité (gabarit-ecran.md:156) ; relire parametres-campagne.md:129-310 contre AR-03.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

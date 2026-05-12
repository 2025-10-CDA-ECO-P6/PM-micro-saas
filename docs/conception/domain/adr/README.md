# Décisions d'architecture domaine

Index des 26 décisions d'architecture domaine du projet Haversack.

- [AD-01.md](AD-01.md) — Séparation entités domaine / modèles infrastructure
- [AD-02.md](AD-02.md) — Shared Kernel pour les primitives partagées
- [AD-03.md](AD-03.md) — Id typés — jamais de UUID nu dans le domaine
- [AD-04.md](AD-04.md) — SoftDelete séparé de AuditInfo
- [AD-05.md](AD-05.md) — BlockValue comme hiérarchie de value objects scellés
- [AD-06.md](AD-06.md) — NPC et PlayerCharacter comme profils spécialisés de Document
- [AD-07.md](AD-07.md) — Lien NPC ↔ PlayerCharacter optionnel et non structurant
- [AD-08.md](AD-08.md) — Séparation User authentifié et accès invité
- [AD-09.md](AD-09.md) — AccessPolicy comme service domaine
- [AD-10.md](AD-10.md) — DocumentTemplate — combinaisons scope/ownerId/campaignId protégées
- [AD-11.md](AD-11.md) — Ordre des scénarios persisté via `Scenario.order`
- [AD-12.md](AD-12.md) — PinnedItem value object pour les documents épinglés
- [AD-13.md](AD-13.md) — LiveNote avec visibilité contrôlée et auteur typé
- [AD-14.md](AD-14.md) — Références légères entre contextes — Id uniquement
- [AD-15.md](AD-15.md) — Synchronisation des champs dénormalisés via domain event synchrone
- [AD-16.md](AD-16.md) — Dossiers utilisateur — structure libre, non imposée
- [AD-17.md](AD-17.md) — Synchronisation template — action manuelle, jamais automatique
- [AD-18.md](AD-18.md) — RequesterId — type union pour l'autorisation GuestAccess
- [AD-19.md](AD-19.md) — Tags comme entités de campagne dans Content Library
- [AD-20.md](AD-20.md) — Dispatch synchrone des domain events dans le monolithe MVP
- [AD-21.md](AD-21.md) — Pattern factory pour la co-création Document + entité métier
- [AD-22.md](AD-22.md) — Backlinks orphelins — non affichés, jamais d'erreur
- [AD-23.md](AD-23.md) — Stratégie URL — Id UUID, slug pour l'affichage uniquement
- [AD-24.md](AD-24.md) — Visibilité par défaut des personnages sans association joueur
- [AD-25.md](AD-25.md) — AccessPolicy unifiée pour Document, LiveNote et SessionSummary
- [AD-26.md](AD-26.md) — Suppression de UserRole global — rôle uniquement contextuel (campaign-level)

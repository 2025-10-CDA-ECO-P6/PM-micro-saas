# Haversack — Documentation

Outil d'assistance au Maître du Jeu pour la préparation et la conduite de parties de jeu de rôle.

---

## Documents maîtres

Livrables autoportants de premier rang, à la racine de `docs/` (qualité professionnelle, présentables en entreprise).

- [note-cadrage-projet.md](note-cadrage-projet.md) — Note de cadrage du projet : intention, périmètre, jalons
- [cahier-des-charges.md](cahier-des-charges.md) — Cahier des charges (MVP) : le **quoi fonctionnel**
- [cahier-specifications-techniques.md](cahier-specifications-techniques.md) — Cahier de spécifications techniques : le **quoi technique** (stack, contraintes, NFR, sécurité, modèle de données, contrats/API, migration)
- [dossier-architecture.md](dossier-architecture.md) — Dossier d'Architecture Technique (DAT) : la **carte du système** (vues C4, choix de structure)
- [dossier-conception-detaillee.md](dossier-conception-detaillee.md) — Dossier de conception détaillée (SDD) : le **comment** (séquences, règles, relations entre décisions)
- [dossier-securite.md](dossier-securite.md) — Dossier de sécurité
- [cahier-strategie-test-et-recette.md](cahier-strategie-test-et-recette.md) — Cahier de stratégie de test et de recette
- [guide-conventions-et-dod.md](guide-conventions-et-dod.md) — Guide de conventions et Definition of Done

---

## Conception

Documentation produit et fonctionnelle.

- [conception/README.md](conception/README.md) — Index de toute la documentation de conception

### Vision & Produit

- [conception/vision/vision-produit.md](conception/vision/vision-produit.md) — Positionnement produit, acteurs, choix assumés, modèle de monétisation
- [conception/vision/moscow.md](conception/vision/moscow.md) — Matrice MoSCoW complète (Must / Should / Could / Won't Have)

### Use Cases

- [conception/usecases/README.md](conception/usecases/README.md) — Index des 14 use cases MVP avec résumés
- [conception/usecases/use-cases.md](conception/usecases/use-cases.md) — Diagrammes de cas d'utilisation (5 vues)
- [conception/usecases/UC-HORS-MVP.md](conception/usecases/UC-HORS-MVP.md) — Fonctionnalités exclues et vision long terme

### User Stories

Epics de user stories avec critères d'acceptation et règles métier (RB-XX) pour chaque use case.

→ [Index user stories](conception/user-stories/README.md)

### User Journeys

Parcours utilisateur pas à pas pour chaque use case, du point de vue des personas.

→ [Index user journeys](conception/user-journeys/README.md)

### Personas

- [conception/persona/README.md](conception/persona/README.md) — 7 personas : Thomas, Émilie, Lucas, Nadia, Antoine, Rémi, Sonia

### Exigences non fonctionnelles (NFR)

Performance perçue, hors connexion, confidentialité, accessibilité, internationalisation — exigences produit en langage besoin.

→ [Index NFR](conception/nfr/README.md)

### Parcours bout-en-bout

Fil narratif de bout en bout pour chaque persona : couture transverse des use cases, vérification des transitions inter-UC.

→ [Index parcours](conception/parcours/README.md)

### Domaine DDD

- [conception/domain/README.md](conception/domain/README.md) — Index des 4 bounded contexts
- [conception/domain/core.md](conception/domain/core.md) — Shared Kernel : abstractions, IDs typés, value objects transverses
- [conception/domain/identity-access.md](conception/domain/identity-access.md) — Comptes, authentification, tiers, RGPD
- [conception/domain/space-management.md](conception/domain/space-management.md) — Campagnes, membres, invitations, accès invités
- [conception/domain/content-library.md](conception/domain/content-library.md) — Documents, dossiers, types, références entre documents
- [conception/domain/session-conduct.md](conception/domain/session-conduct.md) — Cycle de vie de session, tableau de bord, notes de session


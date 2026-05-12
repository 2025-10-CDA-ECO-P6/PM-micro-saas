# Conception — Haversack

Documentation de conception du projet Haversack. Organisée par thème.

---

## Vision

Positionnement produit, vocabulaire partagé et priorisation.

- [vision/vision-produit.md](vision/vision-produit.md) — Positionnement produit, acteurs, choix assumés et comparatif concurrentiel
- [vision/ubiquitous-language.md](vision/ubiquitous-language.md) — Vocabulaire partagé du domaine
- [vision/moscow.md](vision/moscow.md) — Matrice MoSCoW des use cases MVP

---

## Domain

Modèle de domaine DDD — bounded contexts, agrégats, règles métier, décisions d'architecture.

- [domain/overview.md](domain/overview.md) — Vision du domaine et Context Map
- [domain/shared-kernel.md](domain/shared-kernel.md) — Shared Kernel (Id typés, value objects, RequesterId)
- [domain/identity-access.md](domain/identity-access.md) — Bounded Context Identity & Access
- [domain/campaign-management.md](domain/campaign-management.md) — Bounded Context Campaign Management
- [domain/content-library.md](domain/content-library.md) — Bounded Context Content Library
- [domain/session-conduct.md](domain/session-conduct.md) — Bounded Context Session Conduct
- [domain/domain-events.md](domain/domain-events.md) — Vue globale des domain events
- [domain/out-of-scope.md](domain/out-of-scope.md) — Hors périmètre MVP
- [domain/adr/README.md](domain/adr/README.md) — Index des 26 décisions d'architecture (AD-01 à AD-26)

---

## Classes

Diagrammes de classes UML par bounded context.

- [classes/identity-access.md](classes/identity-access.md) — Agrégat User
- [classes/shared-kernel.md](classes/shared-kernel.md) — RequesterId (type union)
- [classes/campaign-management.md](classes/campaign-management.md) — Campaign, GuestAccess, AccessPolicy
- [classes/content-library.md](classes/content-library.md) — Document, NPC, PlayerCharacter, Scenario, Folder
- [classes/session-conduct.md](classes/session-conduct.md) — Session, LiveNote, SessionSummary
- [classes/context-map.md](classes/context-map.md) — Vue globale des dépendances entre contextes

---

## Data

Modèle conceptuel de données (MCD / ERD) par bounded context.

- [data/overview.md](data/overview.md) — Conventions et vue globale
- [data/identity-access.md](data/identity-access.md) — Table USER
- [data/campaign-management.md](data/campaign-management.md) — Tables Campaign Management
- [data/content-library.md](data/content-library.md) — Tables Content Library
- [data/session-conduct.md](data/session-conduct.md) — Tables Session Conduct
- [data/references.md](data/references.md) — Récapitulatif des tables et références cross-context

---

## Diagrams

- [diagrams/use-cases.md](diagrams/use-cases.md) — Diagrammes de cas d'utilisation (5 vues)

---

## Stack technique

- [stack.md](stack.md) — Choix technologiques, justifications, points forts/faibles et évolutions futures

---

## Pages de l'application

- [pages.md](pages.md) — Référence complète de toutes les pages (URL, acteurs, éléments clés, priorité wireframes)

---

## Autres fichiers à la racine

- [Interview.md](Interview.md) — Notes d'entretien utilisateur
- [usecases/](usecases/) — Use cases détaillés

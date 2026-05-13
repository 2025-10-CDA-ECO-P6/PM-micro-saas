# Conception — Haversack

Documentation de conception du projet Haversack. Organisée par thème.

---

## Vision

Positionnement produit, vocabulaire partagé et priorisation.

- [vision/vision-produit.md](vision/vision-produit.md) — Positionnement produit, acteurs, choix assumés et comparatif concurrentiel
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

---

## Classes

Diagrammes de classes UML par bounded context.

- [classes/identity-access.md](classes/identity-access.md) — Agrégat User
- [classes/shared-kernel.md](classes/shared-kernel.md) — RequesterId (type union)
- [classes/campaign-management.md](classes/campaign-management.md) — Campaign, GuestAccess, AccessPolicy
- [classes/content-library.md](classes/content-library.md) — Document, PlayerCharacter, Scenario, DocumentType, Folder
- [classes/session-conduct.md](classes/session-conduct.md) — Session, LiveNote
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

## Use Cases

Périmètre fonctionnel MVP, diagrammes et fiches détaillées.

- [usecases/README.md](usecases/README.md) — Index complet des 14 use cases avec résumés et MoSCoW
- [usecases/use-cases.md](usecases/use-cases.md) — Diagrammes de cas d'utilisation (5 vues Mermaid)
- [usecases/UC-HORS-MVP.md](usecases/UC-HORS-MVP.md) — Fonctionnalités exclues du MVP et vision long terme

---

## Stack technique

- [stack.md](stack.md) — Choix technologiques, justifications, points forts/faibles et évolutions futures

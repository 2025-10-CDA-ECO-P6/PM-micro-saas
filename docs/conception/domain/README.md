# Domain

Modèle de domaine complet du projet Haversack — bounded contexts, agrégats, règles métier et décisions d'architecture.

- [overview.md](overview.md) — Vision du domaine, utilisateurs et découpage en Bounded Contexts (Context Map)
- [shared-kernel.md](shared-kernel.md) — Shared Kernel : Id typés, value objects fondations, RequesterId, enumerations partagées et abstractions
- [identity-access.md](identity-access.md) — Bounded Context Identity & Access : agrégat User, statuts et domain events
- [campaign-management.md](campaign-management.md) — Bounded Context Campaign Management : Campaign, GuestAccess, GameSystem, AccessPolicy
- [content-library.md](content-library.md) — Bounded Context Content Library : Document, NPC, PlayerCharacter, Scenario, DocumentTemplate, Folder, Tag
- [session-conduct.md](session-conduct.md) — Bounded Context Session Conduct : Session, LiveNote, SessionSummary
- [domain-events.md](domain-events.md) — Vue globale de tous les domain events par contexte
- [out-of-scope.md](out-of-scope.md) — Fonctionnalités hors périmètre MVP avec points d'extension documentés
- [adr/README.md](adr/README.md) — Index des 26 décisions d'architecture domaine (AD-01 à AD-26)

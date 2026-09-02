# Architecture détaillée : vue transverse par préoccupation

> Ce document est la **vue par préoccupation technique transverse**, complémentaire de la carte structurelle du système : il regroupe les décisions d'architecture déjà actées, organisées par axe technique (persistance, sécurité, RGPD, temps réel, mode local) plutôt que par ordre chronologique ou par ordre de lecture. Il **consolide et renvoie** — la source de vérité de chaque décision reste l'ADR ou le fichier de fondations cité en regard de chaque affirmation. Pour la carte structurelle du système (vues C4, choix de structure), voir le [Dossier d'Architecture Technique](../context/dossier-architecture.md).
>
> Ce document n'est **ni une nouvelle décision d'architecture, ni la conception détaillée en avant du build** : il ne tranche rien qui ne soit déjà tranché ailleurs, et il ne prépare pas de travail d'implémentation — cela reste un travail dédié différé, propre à l'entrée en build. Pour l'historique et le statut de chaque décision, voir le [registre ADR](decisions/README.md) ; pour une découverte dans l'ordre pédagogique, voir l'[index d'architecture](README.md).

---

## 1 — Fondations & découpage

La structure du système (Clean Architecture, découpage en bounded contexts DDD, architecture applicative front) est cartographiée par le [Dossier d'Architecture Technique](../context/dossier-architecture.md) — voir [§1 Choix structurants](../context/dossier-architecture.md) pour l'argumentaire de structure et [§4 Vue composants](../context/dossier-architecture.md) pour la vue par bounded context. Les sections qui suivent s'appuient sur deux concepts de domaine que la carte structurelle nomme sans les développer ici : le pattern « tout est Document » ([ADR-002](decisions/ADR-002-tout-est-document-gouvernance.md)) et la racine applicative généralisée `Space` ([ADR-018](decisions/ADR-018-espace-personnel-generalisation-space.md)).

---

## 2 — Persistance & mapping

La persistance serveur repose sur EF Core. Les conventions de mapping (converters d'Id typés, owned types, query filters globaux) sont un livrable de conception rattaché à la structure des projets .NET et à la couche `Haversack.Infrastructure.Persistence` — voir [structure-projets.md §3](structure-projets.md) et [ADR-008 § Conséquences](decisions/ADR-008-structure-solution.md), qui identifie ce mapping comme point de conception à part entière.

Côté navigateur, le mode local persiste dans un store IndexedDB structuré en aggregate-rooted (racine `Space`, plutôt qu'un miroir relationnel des tables serveur), avec un jeu d'index restreint aux chemins de lecture effectivement requis et un versionnement de store découplé du contrat de sérialisation. Ce modèle est spécifié dans [ADR-017](decisions/ADR-017-modele-indexeddb-local.md), qui s'articule avec le contrat de sérialisation défini dans [ADR-016](decisions/ADR-016-serialisation-locale-migration.md).

---

## 3 — Sécurité & autorisation

L'authentification cloud (politique de mot de passe, hachage, cycle de vie des tokens JWT — durées, rotation, dénylist par famille —, rate limiting, liaison OAuth) est portée par des contrats applicatifs observables : `ITokenValidator`, `ITokenDenylist`, `ITokenSigner`, `IEmailVerificationPolicy`. Ces interfaces, leurs garanties et leur vérifiabilité par test d'architecture CI sont définies dans [ADR-015](decisions/ADR-015-securite-authentification-mvp.md).

L'autorisation au niveau des ressources repose sur un service de décision centralisé, `IResourceAccessPolicy`, qui compose le prédicat d'appartenance (ressource ↔ espace) et le prédicat de visibilité délégué au domaine, évalués en un point unique via un pipeline behavior MediatR côté REST et un filtre de diffusion côté SignalR — un seul type de service pour les deux canaux, non-divergence garantie. Le modèle du principal (membre actif, invité `GuestAccess`, statut `CONVERTED`), la matrice ressource → règle d'appartenance et les query filters globaux structurels (espace non supprimé, document non supprimé) sont spécifiés dans [ADR-014](decisions/ADR-014-modele-autorisation-api.md).

---

## 4 — RGPD & cascade

L'intégrité référentielle à la suppression d'un espace repose sur une saga applicative (`SpaceDeleted`) — toutes les FK sont déclarées `ON DELETE RESTRICT`, le séquençage (déliaison des cycles, ordre topologique de suppression) est piloté explicitement plutôt que délégué à une cascade SQL. Le mécanisme, la matrice des FK et l'ordre des passes sont définis dans [ADR-011](decisions/ADR-011-cascade-integrite-referentielle.md), qui pose également la saga symétrique `UserAnonymized` pour l'effacement de compte.

La politique RGPD de l'effacement de compte (Art. 17) — catégorisation des données conservées, anonymisées ou supprimées, périmètre de l'effacement étendu, règle anti-résidu sur les notes privées, purge des logs de corrélation, délai de traitement Art. 12§3 — est actée dans [ADR-012](decisions/ADR-012-rgpd-effacement-compte.md). Le traitement spécifique des données des joueurs invités (base légale, information Art. 13, rétention autonome, posture sur les mineurs et sur la qualification sous-traitant) est un périmètre distinct, spécifié dans [ADR-013](decisions/ADR-013-rgpd-donnees-invites.md). Ces deux ADR se croisent explicitement avec la cascade d'[ADR-011](decisions/ADR-011-cascade-integrite-referentielle.md) sans la redéfinir.

---

## 5 — Temps réel

Le transport temps réel (partage MJ → joueurs) retient SignalR dès le départ, avec repli automatique (WebSocket → Server-Sent Events → long-polling) et isolation dans le module `Haversack.Infrastructure.Notifications`. Le choix, les alternatives écartées (polling, différé post-MVP) et les compléments de configuration (configuration sobre, sécurité du canal invité, filtrage par visibilité de ressource) sont actés dans [ADR-004](decisions/ADR-004-transport-temps-reel.md).

---

## 6 — Mode local & frontière local↔cloud

Le mode local est une couche de persistance et de validations TypeScript minimales côté navigateur — il n'exécute pas le domaine C# complet, qui reste la source de vérité unique. Ce principe fondateur, l'invariant « validation locale ⊆ validation serveur » et le choix C#-first pour l'ordre de construction sont actés dans [ADR-001](decisions/ADR-001-execution-domaine-mode-local.md).

La frontière entre le mode local et le cloud est traversée exclusivement par une migration one-shot, jamais par une synchronisation continue. Le contrat de sérialisation (enveloppe versionnée, champs gouvernés vs libres, champs jamais honorés depuis le payload, parcours d'échec transactionnel par espace, gate de confirmation anti-appropriation) est spécifié dans [ADR-016](decisions/ADR-016-serialisation-locale-migration.md). Le pendant client de cette frontière — modèle IndexedDB, posture de synchronisation migration-only, persistance best-effort (`navigator.storage.persist()`), sanitisation XSS côté client et non-chiffrement at-rest assumé — est spécifié dans [ADR-017](decisions/ADR-017-modele-indexeddb-local.md).

---

*Ce document est une consolidation ; en cas de divergence apparente avec un ADR cité, l'ADR fait autorité.*

# 0003 — Recouper les 45 contre-mesures du dossier sécurité avec leurs ADR

- **Id** : 0003
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-decisions-architecture-assurance (chantier conformite-corpus)
- **Appliqué dans** : docs/assurance/securite/dossier-securite.md, docs/architecture/specs/sanitisation-csp.md ; `docs/decisions/ADR-016-serialisation-locale-migration.md`, `docs/decisions/ADR-017-modele-indexeddb-local.md` (compléments post-revue du 2026-09-03 : l'amont rattrape l'aval, décision de l'orchestrateur par délégation de l'opérateur)

Recouper les 45 contre-mesures du dossier sécurité avec leurs ADR. Seuls les points ouverts, les tensions et les lignes de télémétrie ont été lus ; la matrice STRIDE et §1.3 à §4.9 n'ont pas été recoupées (docs/securite/dossier-securite.md).

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur

**Conclusion de la vérification (tâche backlog-decisions-architecture-assurance).** Les 45 contre-mesures, les neuf tables STRIDE par surface (§3), la matrice de synthèse, le tableau des surfaces (§2) et les tableaux §1.3 à §4.9 ont été recoupés avec leur cible ouverte : ADR-004, 006, 011, 012, 013, 014, 015, 016, 017, 018, les huit specs d'architecture citées, le cadrage juridique, le squelette de DPA, la procédure de droits des invités et les contextes de domaine. 45 contre-mesures recoupées sur 45 ; 41 sont conformes à leur source. Quatre contredisaient leur source et ont été corrigées dans le dossier, sans décision :

- n° 7 et §4.9 : l'ancre « ADR-004 §Compléments post-revue » portait des sticky sessions et des connexions persistantes qui figurent dans §Contexte et §Conséquences ; la mention « backplane » n'existe dans aucune source et a été retirée ;
- n° 24 et table S3 : le 404 pour un non-membre était présenté comme acquis alors que `contrat-openapi.md` §4 le donne pour une proposition non tranchée (B1.10), que §5.1 du dossier listait déjà comme ouvert ;
- n° 45 : « moyen technique non tranché » contredisait `telemetrie.md` §6 (transport décidé le 2026-09-03, `fd9124f`) ;
- n° 30 : `connect-src 'self'` omettait « ou restreint à l'API » (ADR-017 §4.2).

Trois lignes de §5.1 hors numérotation ont aussi été corrigées : la ligne des balises et bibliothèque de sanitisation et la ligne des directives CSP (décidées le 2026-09-03, `conventions-dod.md` §7, vérifié par `git log -S` : `04f16eb`), remplacées par une ligne ouverte (bibliothèque côté serveur, B1.2) et une ligne DÉCIDÉ ; la ligne de télémétrie réduite par l'entrée 0068. L'ancre « ADR-006 §Périmètre MVP » du tableau des surfaces (S8) pointait une section inexistante : elle devient « §Compléments post-revue ».

Arbitrés ensuite par l'orchestrateur, par délégation de l'opérateur, et appliqués :

- n° 14 et n° 42 : la matrice et les tables S2 et S9 sont alignées sur le classement du §4 (Elevation of Privilege) ;
- §4.9 : « comportement d'exécution SignalR » devient « choix de signature asymétrique (n° 4) », ce que porte ADR-015 §Croisements ;
- `sanitisation-csp.md` §4 : « reste ouvert (§5) » est aligné sur le §5 (renvoi à `conventions-dod.md` §7).

Restent, sans être tranchés :

- le marqueur `[À TRANCHER — à ratifier produit]` du reclaim-in-place diffère de `[À TRANCHER — PRODUIT]` dans ADR-015 ; la variante est reprise dans `cahier-specifications-techniques.md`, `conception-detaillee.md` et `roadmap-de-build.md` (hors périmètre de cette couche), différé à la normalisation des marqueurs ;
- amont à aligner : ADR-016 §2.4, ADR-017 §4.1 / §4.2 (liste de balises, bibliothèque, directives renvoyées à B1.2 / P6) n'ont pas suivi les décisions du 2026-09-03 de `conventions-dod.md` §7 ;
- consommateurs hors couche qui citent encore la télémétrie comme ouverte : `cahier-specifications-techniques.md` (l.456, l.592) et `conception-detaillee.md` (l.1065).

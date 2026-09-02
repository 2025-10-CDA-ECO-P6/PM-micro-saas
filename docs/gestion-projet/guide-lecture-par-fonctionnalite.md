# Guide d'entrée par fonctionnalité — Haversack

- **Statut** — Guide de renvoi dérivé du corpus de conception, d'architecture et de test.
- **Audience** — Toute personne reprenant le projet sans connaître son auteur et sans pouvoir le joindre.
- **Sources** — `docs/conception/**`, `docs/architecture/decisions/**`, `docs/architecture/specs/**`, `docs/test/cahier-strategie-test-et-recette.md`, `docs/conception/interface/wireframes/README.md`.

### Bandeau d'autorité

> **En cas de conflit entre ce guide et une source citée, la source citée fait foi.** Ce guide est un artefact de renvoi, jamais une autorité de corpus — il ne redéfinit aucune décision, aucune règle et aucun statut, il les rassemble par fonctionnalité et y renvoie fidèlement.

---

## Comment lire une entrée

Chaque fonctionnalité du produit est une sous-section, avec une table à deux colonnes fixes : un axe de lecture, et où le lire. **Toute cellule « Où lire » est un renvoi, jamais un énoncé.** Aucune cellule ne porte de règle métier, de critère d'acceptation, de seuil, de décompte ou de priorité MoSCoW en clair — ce guide ne réénonce rien de ce que le corpus porte déjà.

Deux axes du corpus possèdent déjà une partie de cette information et **sont délégués** — ce guide y renvoie sans la recopier :

- **Les user stories, leurs scénarios Gherkin et leurs règles métier** vivent dans la section « User stories » du fichier `US-UC-NN-*.md` correspondant. Ce guide n'en recopie aucun identifiant `RB-` ni aucun titre de scénario.
- **Les exigences non fonctionnelles et les cas de recette** sont déjà rattachés par use case dans [le cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — intitulé, statut MVP, user stories couvertes, plage de cas de recette, NFR et tests nommés y vivent en une seule ligne par use case. Ce guide y renvoie plutôt que de reproduire un décompte qui dériverait à chaque cas ajouté ou retiré.
- **Le rattachement écran** vit dans [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md). Ce guide n'énumère aucune fiche individuellement — ajouter un écran à une fonctionnalité existante ne doit jamais obliger à rouvrir ce guide.
- **Le statut dans le périmètre** (Must / Should / Could / Post-MVP) n'est jamais réénoncé en clair : chaque entrée renvoie à sa sous-section de `moscow.md`, seule autorité du corpus pour cet axe.

Un seul axe n'est délégué à aucune table existante et **c'est ce que ce guide produit en propre** : le rattachement d'une fonctionnalité à la décision d'architecture (ADR) qui la contraint et à la spécification technique qui la détaille. Aucun document du corpus ne portait ce lien avant ce guide ; il nomme la décision et la spécification, sans rien redire de ce qu'elles décident.

**Clause opposable** : une cellule qui commence par `aucun` ou `aucune` documente une absence réelle du corpus, jamais une case oubliée. Deux natures d'absence s'y distinguent. Une absence qui se suffit — la valeur dit tout ce qu'il y a à dire, comme `aucune spécification dédiée` : toute fonctionnalité n'appelle pas une spécification dédiée dans [`docs/architecture/specs/`](../architecture/specs/) — reste nue. Une absence qui surprend — typiquement une unité Must Have sans récit, sans parcours ou sans cas de recette — porte impérativement sa raison et un renvoi vers ce qui l'atteste : le lecteur doit pouvoir vérifier que ce n'est pas un oubli.

---

## Périmètre couvert

La liste des fonctionnalités ci-dessous suit [`moscow.md`, § Vue d'ensemble](../conception/besoin/vision/moscow.md), seule autorité du corpus pour la priorisation produit. **Cette liste n'est pas la source du périmètre** — elle en est une entrée de lecture ; un désaccord entre cette liste et `moscow.md` se résout toujours en faveur de `moscow.md`.

Le périmètre couvert est celui que le corpus spécifie dans son intégralité — Must Have, Should Have et post-MVP spécifié compris. Une fonctionnalité classée Should Have hors première livraison ou Post-MVP (spécifiés) a, comme toute autre, sa fiche de use case, ses règles métier et ses cas de recette : elle a donc son entrée dans ce guide, au même titre que les fonctionnalités du MVP.

Une fonctionnalité n'est pas toujours un use case : trois éléments Must Have (l'espace personnel, l'export d'espace, l'instrumentation de validation du MVP) n'ont aucun use case porteur, et un même use case peut porter plusieurs fonctionnalités distinctes. Les entrées ci-dessous suivent la fonctionnalité, pas le numéro d'use case : celles qui ont un use case le nomment et délèguent à l'aval déjà indexé par use case ; celles qui n'en ont pas ont leur entrée propre.

---

## Les entrées

### Démarrage sans compte et persistance locale

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-01 — Utiliser l'application sans compte (mode local MJ), § Objectif](../conception/besoin/usecases/UC-01-mode-local-sans-compte.md) |
| Les récits et les règles métier | [US-UC-01 — Epic mode local sans compte, § User stories](../conception/besoin/user-stories/US-UC-01-mode-local-sans-compte.md) |
| Le parcours d'usage | [UJ-UC-01 — Mode local sans compte](../conception/besoin/user-journeys/UJ-UC-01-mode-local-sans-compte.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-01 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-01 |
| Les décisions qui la contraignent | [ADR-001 — Exécution du domaine en mode local, § Décision](../architecture/decisions/ADR-001-execution-domaine-mode-local.md) ; [ADR-016 — Sérialisation locale et contrat de migration local→cloud, § Décision](../architecture/decisions/ADR-016-serialisation-locale-migration.md) ; [ADR-017 — Modèle IndexedDB local et sécurité du mode local, § Décision](../architecture/decisions/ADR-017-modele-indexeddb-local.md) ; [ADR-018 — Contenu personnel de premier ordre, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) |
| La spécification technique | [Politique de sanitisation HTML et CSP](../architecture/specs/sanitisation-csp.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-01 |
| Le statut dans le périmètre | [moscow.md, § UC-01 — Mode local sans compte](../conception/besoin/vision/moscow.md) |

### Espace personnel — provisionnement, capture-first, hors quota

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [moscow.md, § Espace personnel — conteneur par défaut et capture-first](../conception/besoin/vision/moscow.md) |
| Les récits et les règles métier | [US-UC-01 — Epic mode local sans compte, § User stories](../conception/besoin/user-stories/US-UC-01-mode-local-sans-compte.md) ; [US-UC-02 — Epic créer un espace de jeu, § User stories](../conception/besoin/user-stories/US-UC-02-creer-espace-jeu.md) |
| Le parcours d'usage | [UJ-UC-01 — Mode local sans compte](../conception/besoin/user-journeys/UJ-UC-01-mode-local-sans-compte.md) ; [UJ-UC-02 — Créer un espace de jeu](../conception/besoin/user-journeys/UJ-UC-02-creer-espace-jeu.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — logée dans la ligne UC-01 (vue de l'espace personnel) |
| Les exigences non fonctionnelles | [NFR-OFF-01 — Utilisation intégrale sans connexion en mode local](../conception/besoin/nfr/NFR-OFF-01-utilisation-integrale-sans-connexion.md) |
| Les décisions qui la contraignent | [ADR-018 — Contenu personnel de premier ordre : généralisation de `Campaign` en `Space`, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | aucune plage dédiée ; les cas pertinents sont dispersés dans [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — lignes UC-01 et UC-02 |
| Le statut dans le périmètre | [moscow.md, § Espace personnel — conteneur par défaut et capture-first](../conception/besoin/vision/moscow.md) |

### Export d'espace

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [moscow.md, § Export d'espace](../conception/besoin/vision/moscow.md) |
| Les récits et les règles métier | [US-UC-01 — Epic mode local sans compte, § User stories](../conception/besoin/user-stories/US-UC-01-mode-local-sans-compte.md) |
| Le parcours d'usage | [UJ-UC-01 — Mode local sans compte, § Scénarios alternatifs et d'erreur](../conception/besoin/user-journeys/UJ-UC-01-mode-local-sans-compte.md) |
| La maquette | aucune fiche dédiée nommée ; [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) rattache l'ensemble d'UC-01 sans isoler l'affordance d'export |
| Les exigences non fonctionnelles | [NFR-OFF-02 — Durabilité des données locales entre les sessions](../conception/besoin/nfr/NFR-OFF-02-durabilite-donnees-locales.md) ; [NFR-OFF-03 — Aucune perte silencieuse de données locales](../conception/besoin/nfr/NFR-OFF-03-aucune-perte-silencieuse-donnees-locales.md) |
| Les décisions qui la contraignent | [ADR-016 — Sérialisation locale et contrat de migration local→cloud, § Enveloppe du payload](../architecture/decisions/ADR-016-serialisation-locale-migration.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | aucune plage dédiée ; dispersée dans [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-01 |
| Le statut dans le périmètre | [moscow.md, § Export d'espace](../conception/besoin/vision/moscow.md) |

### Conversion du mode local vers un compte et migration

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-01 — Utiliser l'application sans compte (mode local MJ), § A1 — Conversion vers un compte](../conception/besoin/usecases/UC-01-mode-local-sans-compte.md) |
| Les récits et les règles métier | [US-UC-01 — Epic mode local sans compte, § User stories](../conception/besoin/user-stories/US-UC-01-mode-local-sans-compte.md) ; [US-UC-10 — Epic créer un compte et synchroniser dans le cloud, § User stories](../conception/besoin/user-stories/US-UC-10-compte-cloud.md) |
| Le parcours d'usage | [UJ-UC-01 — Mode local sans compte, § Scénarios alternatifs et d'erreur](../conception/besoin/user-journeys/UJ-UC-01-mode-local-sans-compte.md) ; [UJ-UC-10 — Créer un compte et synchroniser dans le cloud](../conception/besoin/user-journeys/UJ-UC-10-compte-cloud.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — lignes UC-01 et UC-10 (gate de migration) |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — lignes UC-01 et UC-10 |
| Les décisions qui la contraignent | [ADR-016 — Sérialisation locale et contrat de migration local→cloud, § Décision](../architecture/decisions/ADR-016-serialisation-locale-migration.md) ; [ADR-017 — Modèle IndexedDB local et sécurité du mode local, § Décision](../architecture/decisions/ADR-017-modele-indexeddb-local.md) |
| La spécification technique | [Registre des seuils et valeurs de configuration — sécurité & migration](../architecture/specs/config-securite-migration.md) |
| Les cas de recette | aucune plage dédiée ; dispersée dans [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — lignes UC-01 et UC-10 |
| Le statut dans le périmètre | [moscow.md, § UC-01 — Mode local sans compte](../conception/besoin/vision/moscow.md) ; [moscow.md, § UC-10 — Créer un compte et synchroniser dans le cloud](../conception/besoin/vision/moscow.md) |

### Création d'un espace partagé et quota FREE

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-02 — Créer un espace de jeu, § Objectif](../conception/besoin/usecases/UC-02-creer-espace-jeu.md) |
| Les récits et les règles métier | [US-UC-02 — Epic créer un espace de jeu, § User stories](../conception/besoin/user-stories/US-UC-02-creer-espace-jeu.md) |
| Le parcours d'usage | [UJ-UC-02 — Créer un espace de jeu](../conception/besoin/user-journeys/UJ-UC-02-creer-espace-jeu.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-02 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-02 |
| Les décisions qui la contraignent | [ADR-005 — Modèle de monétisation, § Décision](../architecture/decisions/ADR-005-modele-monetisation.md) ; [ADR-018 — Contenu personnel de premier ordre, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) ; [ADR-010 — Suppression d'espace, § Décision](../architecture/decisions/ADR-010-suppression-espace.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-02 |
| Le statut dans le périmètre | [moscow.md, § UC-02 — Créer et configurer un espace (campagne ou one-shot)](../conception/besoin/vision/moscow.md) |

### One-shot express et one-shot depuis bibliothèque

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-02 — Créer un espace de jeu, § Scénario alternatif A1 — Lancer un one-shot (parcours express)](../conception/besoin/usecases/UC-02-creer-espace-jeu.md) |
| Les récits et les règles métier | [US-UC-02 — Epic créer un espace de jeu, § User stories](../conception/besoin/user-stories/US-UC-02-creer-espace-jeu.md) |
| Le parcours d'usage | [UJ-UC-02 — Créer un espace de jeu](../conception/besoin/user-journeys/UJ-UC-02-creer-espace-jeu.md) |
| La maquette | aucune fiche dédiée ; [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) rattache l'ensemble d'UC-02 sans isoler ce parcours |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-02 |
| Les décisions qui la contraignent | aucune — aucun ADR ne nomme spécifiquement cette variante ; elle hérite des décisions citées à l'entrée « Création d'un espace partagé et quota FREE » |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | aucune plage dédiée ; dispersée dans [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-02 |
| Le statut dans le périmètre | aucune sous-section MoSCoW nommée pour cette variante ; son rattachement se lit par dépendance dans [moscow.md, § UC-13 — Utiliser un scénario réutilisable](../conception/besoin/vision/moscow.md) |

### Organisation en dossiers — base

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-05 — Organiser par dossiers, § Objectif](../conception/besoin/usecases/UC-05-organiser-dossiers.md) |
| Les récits et les règles métier | [US-UC-05 — Epic organiser le contenu en dossiers, § User stories](../conception/besoin/user-stories/US-UC-05-organiser-contenu-dossiers.md) |
| Le parcours d'usage | [UJ-UC-05 — Organiser le contenu en dossiers](../conception/besoin/user-journeys/UJ-UC-05-organiser-contenu-dossiers.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-05 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-05 |
| Les décisions qui la contraignent | aucune — aucun ADR ne nomme cette unité ; les invariants vivent dans le modèle de domaine ([content-library.md](../conception/domain/content-library.md)) et dans [zoning.md, § Arbitrages tracés](../conception/interface/zoning.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-05 |
| Le statut dans le périmètre | [moscow.md, § UC-05 base — Organiser le contenu en dossiers (Must Have)](../conception/besoin/vision/moscow.md) |

### Types de document built-in — riche

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-05 — Organiser par dossiers, § Types de document fournis par défaut](../conception/besoin/usecases/UC-05-organiser-dossiers.md) |
| Les récits et les règles métier | [US-UC-05 — Epic organiser le contenu en dossiers, § User stories](../conception/besoin/user-stories/US-UC-05-organiser-contenu-dossiers.md) |
| Le parcours d'usage | [UJ-UC-05 — Organiser le contenu en dossiers](../conception/besoin/user-journeys/UJ-UC-05-organiser-contenu-dossiers.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-05 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-05 |
| Les décisions qui la contraignent | aucune — aucun ADR ne nomme cette unité ; les invariants vivent dans le modèle de domaine ([content-library.md](../conception/domain/content-library.md)) |
| La spécification technique | [VO `DocumentProperties` & `propertiesSchema` par type système](../architecture/specs/document-properties-schemas.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-05 |
| Le statut dans le périmètre | [moscow.md, § UC-05 riche — Dossiers et types de document élaborés (Should Have)](../conception/besoin/vision/moscow.md) |

### Gestion des documents d'un espace

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-04 — Gérer les documents d'un espace, § Objectif](../conception/besoin/usecases/UC-04-gerer-documents-espace.md) |
| Les récits et les règles métier | [US-UC-04 — Epic créer et gérer des documents d'espace modulaires, § User stories](../conception/besoin/user-stories/US-UC-04-gerer-documents-espace.md) |
| Le parcours d'usage | [UJ-UC-04 — Gérer les documents d'un espace](../conception/besoin/user-journeys/UJ-UC-04-gerer-documents-espace.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-04 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-04 |
| Les décisions qui la contraignent | [ADR-002 — « Tout est Document » strict et gouvernance des identifiants et de `properties`, § Décision](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md) ; [ADR-018 — Contenu personnel de premier ordre, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) |
| La spécification technique | [VO `DocumentProperties` & `propertiesSchema` par type système](../architecture/specs/document-properties-schemas.md) ; [Politique de sanitisation HTML et CSP](../architecture/specs/sanitisation-csp.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-04 |
| Le statut dans le périmètre | [moscow.md, § UC-04 — Gérer les documents d'un espace](../conception/besoin/vision/moscow.md) |

### Structuration d'un scénario

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-03 — Structurer un scénario, § Objectif](../conception/besoin/usecases/UC-03-structurer-scenario.md) |
| Les récits et les règles métier | [US-UC-03 — Epic structurer un scénario, § User stories](../conception/besoin/user-stories/US-UC-03-structurer-scenario.md) |
| Le parcours d'usage | [UJ-UC-03 — Structurer un scénario](../conception/besoin/user-journeys/UJ-UC-03-structurer-scenario.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-03 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-03 |
| Les décisions qui la contraignent | [ADR-002 — « Tout est Document » strict et gouvernance des identifiants et de `properties`, § Décision](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md) |
| La spécification technique | [VO `DocumentProperties` & `propertiesSchema` par type système](../architecture/specs/document-properties-schemas.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-03 |
| Le statut dans le périmètre | [moscow.md, § UC-03 — Structurer un scénario](../conception/besoin/vision/moscow.md) |

### Vue session MJ

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-06 — Utiliser la vue session, § Objectif](../conception/besoin/usecases/UC-06-vue-session.md) |
| Les récits et les règles métier | [US-UC-06 — Epic vue session, § User stories](../conception/besoin/user-stories/US-UC-06-vue-session.md) |
| Le parcours d'usage | [UJ-UC-06 — Vue session](../conception/besoin/user-journeys/UJ-UC-06-vue-session.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-06 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-06 |
| Les décisions qui la contraignent | [ADR-004 — Transport temps réel : SignalR, § Décision](../architecture/decisions/ADR-004-transport-temps-reel.md) ; [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) ; [ADR-016 — Sérialisation locale et contrat de migration local→cloud, § Périmètre sérialisé](../architecture/decisions/ADR-016-serialisation-locale-migration.md) ; [ADR-017 — Modèle IndexedDB local et sécurité du mode local, § Object stores](../architecture/decisions/ADR-017-modele-indexeddb-local.md) |
| La spécification technique | [Seuil de coût session concurrente → repli SSE → polling adaptatif](../architecture/specs/repli-temps-reel.md) ; [Spec — Instrumentation par pilier, § Pilier Vue session](../architecture/specs/telemetrie.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-06 |
| Le statut dans le périmètre | [moscow.md, § UC-06 — Utiliser la vue session](../conception/besoin/vision/moscow.md) |

### Création à la volée en session

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-07 — Créer un élément à la volée pendant la session, § Objectif](../conception/besoin/usecases/UC-07-creation-volee-session.md) |
| Les récits et les règles métier | [US-UC-07 — Epic créer un élément à la volée pendant la session, § User stories](../conception/besoin/user-stories/US-UC-07-creation-volee-session.md) |
| Le parcours d'usage | [UJ-UC-07 — Créer un élément à la volée pendant la session](../conception/besoin/user-journeys/UJ-UC-07-creation-volee-session.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-07 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-07 |
| Les décisions qui la contraignent | [ADR-002 — « Tout est Document » strict et gouvernance des identifiants et de `properties`, § Décision](../architecture/decisions/ADR-002-tout-est-document-gouvernance.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-07 |
| Le statut dans le périmètre | [moscow.md, § UC-07 — Créer un élément à la volée en session](../conception/besoin/vision/moscow.md) |

### Partage d'information aux joueurs

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-08 — Partager une information aux joueurs, § Objectif](../conception/besoin/usecases/UC-08-partager-information.md) |
| Les récits et les règles métier | [US-UC-08 — Epic partager une information aux joueurs, § User stories](../conception/besoin/user-stories/US-UC-08-partager-information.md) |
| Le parcours d'usage | [UJ-UC-08 — Partager une information aux joueurs](../conception/besoin/user-journeys/UJ-UC-08-partager-information.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-08 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-08 |
| Les décisions qui la contraignent | [ADR-004 — Transport temps réel : SignalR, § Décision](../architecture/decisions/ADR-004-transport-temps-reel.md) ; [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) |
| La spécification technique | [Seuil de coût session concurrente → repli SSE → polling adaptatif](../architecture/specs/repli-temps-reel.md) ; [Spec — Instrumentation par pilier, § Pilier Partage](../architecture/specs/telemetrie.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-08 |
| Le statut dans le périmètre | [moscow.md, § UC-08 — Partager une information aux joueurs](../conception/besoin/vision/moscow.md) |

### Accès joueur sans compte

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-09 — Accès joueur via lien, § Objectif](../conception/besoin/usecases/UC-09-acces-session-joueur.md) |
| Les récits et les règles métier | [US-UC-09 — Epic accéder à une session en tant que joueur, § User stories](../conception/besoin/user-stories/US-UC-09-acces-session-joueur.md) |
| Le parcours d'usage | [UJ-UC-09 — Accéder à une session en tant que joueur](../conception/besoin/user-journeys/UJ-UC-09-acces-session-joueur.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-09 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-09 |
| Les décisions qui la contraignent | [ADR-013 — RGPD : données des joueurs invités, § Décision](../architecture/decisions/ADR-013-rgpd-donnees-invites.md) ; [ADR-004 — Transport temps réel : SignalR, § Décision](../architecture/decisions/ADR-004-transport-temps-reel.md) ; [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) |
| La spécification technique | [Contrat API — codes de statut, non-révélation d'existence, annotations sécurité, § Principe transversal : non-révélation d'existence](../architecture/specs/contrat-openapi.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-09 |
| Le statut dans le périmètre | [moscow.md, § UC-09 — Accès joueur sans compte](../conception/besoin/vision/moscow.md) |

### Compte cloud

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-10 — Créer un compte et synchroniser dans le cloud, § Objectif](../conception/besoin/usecases/UC-10-compte-cloud.md) |
| Les récits et les règles métier | [US-UC-10 — Epic créer un compte et synchroniser dans le cloud, § User stories](../conception/besoin/user-stories/US-UC-10-compte-cloud.md) |
| Le parcours d'usage | [UJ-UC-10 — Créer un compte et synchroniser dans le cloud](../conception/besoin/user-journeys/UJ-UC-10-compte-cloud.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-10 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-10 |
| Les décisions qui la contraignent | [ADR-015 — Sécurité authentification MVP, § Décision](../architecture/decisions/ADR-015-securite-authentification-mvp.md) ; [ADR-007 — Conformité RGPD et modèle d'autorisation API, § Décision](../architecture/decisions/ADR-007-rgpd-autorisation-api.md) ; [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) ; [ADR-012 — RGPD : effacement de compte, § Décision](../architecture/decisions/ADR-012-rgpd-effacement-compte.md) ; [ADR-011 — Cascade & intégrité référentielle, § Décision](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) ; [ADR-009 — FK SPACES.ownerId → USER, § Décision](../architecture/decisions/ADR-009-fk-campaign-owner.md) ; [ADR-016 — Sérialisation locale et contrat de migration local→cloud, § Décision](../architecture/decisions/ADR-016-serialisation-locale-migration.md) |
| La spécification technique | [Registre des seuils et valeurs de configuration — sécurité & migration](../architecture/specs/config-securite-migration.md) ; [Contrat API — codes de statut, non-révélation d'existence, annotations sécurité](../architecture/specs/contrat-openapi.md) ; [Requête « document non partagé »](../architecture/specs/requete-effacement-non-partage.md) ; [Note de mapping EF Core](../architecture/specs/mapping-ef-core.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-10 |
| Le statut dans le périmètre | [moscow.md, § UC-10 — Créer un compte et synchroniser dans le cloud](../conception/besoin/vision/moscow.md) |

### Gestion des membres et invitations

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-11 — Gérer les membres d'un espace partagé, § Objectif](../conception/besoin/usecases/UC-11-gerer-membres-espace-partage.md) |
| Les récits et les règles métier | [US-UC-11 — Epic gérer les membres d'un espace partagé, § User stories](../conception/besoin/user-stories/US-UC-11-gerer-membres-espace-partage.md) |
| Le parcours d'usage | [UJ-UC-11 — Gérer les membres d'un espace partagé](../conception/besoin/user-journeys/UJ-UC-11-gerer-membres-espace-partage.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-11, fraction Must wireframée ; [wireframes/README.md, § Éléments différés](../conception/interface/wireframes/README.md) — écran Membres complet |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-11 |
| Les décisions qui la contraignent | [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) ; [ADR-013 — RGPD : données des joueurs invités, § Décision](../architecture/decisions/ADR-013-rgpd-donnees-invites.md) ; [ADR-011 — Cascade & intégrité référentielle, § Décision](../architecture/decisions/ADR-011-cascade-integrite-referentielle.md) |
| La spécification technique | [Contrat API — codes de statut, non-révélation d'existence, annotations sécurité](../architecture/specs/contrat-openapi.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-11 |
| Le statut dans le périmètre | [moscow.md, § UC-11 — Gérer les membres d'un espace](../conception/besoin/vision/moscow.md) |

### Vue joueur post-accès

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-12 — Consulter son espace en tant que joueur (vue post-accès), § Objectif](../conception/besoin/usecases/UC-12-consulter-espace-joueur.md) |
| Les récits et les règles métier | [US-UC-12 — Epic consulter son espace en tant que joueur, § User stories](../conception/besoin/user-stories/US-UC-12-consulter-espace-joueur.md) |
| Le parcours d'usage | [UJ-UC-12 — Consulter son espace en tant que joueur](../conception/besoin/user-journeys/UJ-UC-12-consulter-espace-joueur.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-12, fraction de base ; [wireframes/README.md, § Éléments différés](../conception/interface/wireframes/README.md) — enrichissement niveau campagne |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-12 |
| Les décisions qui la contraignent | [ADR-014 — Modèle d'autorisation API, § Décision](../architecture/decisions/ADR-014-modele-autorisation-api.md) ; [ADR-013 — RGPD : données des joueurs invités, § Décision](../architecture/decisions/ADR-013-rgpd-donnees-invites.md) |
| La spécification technique | [Contrat API — codes de statut, non-révélation d'existence, annotations sécurité](../architecture/specs/contrat-openapi.md) |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-12 |
| Le statut dans le périmètre | [moscow.md, § UC-12 — Consulter son espace en tant que joueur (vue post-accès)](../conception/besoin/vision/moscow.md) |

### Recherche et filtrage

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-14 — Rechercher rapidement une information, § Objectif](../conception/besoin/usecases/UC-14-recherche.md) |
| Les récits et les règles métier | [US-UC-14 — Epic rechercher rapidement une information, § User stories](../conception/besoin/user-stories/US-UC-14-recherche.md) |
| Le parcours d'usage | [UJ-UC-14 — Rechercher rapidement une information](../conception/besoin/user-journeys/UJ-UC-14-recherche.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — ligne UC-14 |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-14 |
| Les décisions qui la contraignent | [ADR-017 — Modèle IndexedDB local et sécurité du mode local, § Indexes locaux](../architecture/decisions/ADR-017-modele-indexeddb-local.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-14 |
| Le statut dans le périmètre | [moscow.md, § UC-14 — Rechercher et filtrer l'information](../conception/besoin/vision/moscow.md) |

### Instrumentation de validation du MVP

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [moscow.md, § Instrumentation de validation du MVP](../conception/besoin/vision/moscow.md) |
| Les récits et les règles métier | aucune — aucune user story ni scénario Gherkin ; [moscow.md, § Instrumentation de validation du MVP](../conception/besoin/vision/moscow.md) décrit le comportement attendu sans nommer de porteur aval |
| Le parcours d'usage | aucun — aucun parcours utilisateur nommé ; [Index des parcours utilisateur, § Tableau de correspondance](../conception/besoin/user-journeys/README.md) s'arrête à UC-14 |
| La maquette | aucune — cette unité n'a aucun UC porteur et ne figure dans aucune ligne de [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) |
| Les exigences non fonctionnelles | aucune — aucune exigence non fonctionnelle n'est nommée pour cette unité dans le besoin |
| Les décisions qui la contraignent | [ADR-006 — Périmètre MVP, § Compléments post-revue](../architecture/decisions/ADR-006-perimetre-mvp.md) |
| La spécification technique | [Spec — Instrumentation par pilier, capture email, analytics RGPD](../architecture/specs/telemetrie.md) — déclarée non implémentable en l'état (voir son en-tête) |
| Les cas de recette | aucun — [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) est structurée par use case et cette unité n'en a aucun |
| Le statut dans le périmètre | [moscow.md, § Instrumentation de validation du MVP](../conception/besoin/vision/moscow.md) |

### Scénario réutilisable / bibliothèque

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-13 — Utiliser un scénario réutilisable (one-shot), § Objectif](../conception/besoin/usecases/UC-13-scenario-reutilisable.md) |
| Les récits et les règles métier | [US-UC-13 — Epic utiliser un scénario réutilisable, § User stories](../conception/besoin/user-stories/US-UC-13-scenario-reutilisable.md) |
| Le parcours d'usage | [UJ-UC-13 — Utiliser un scénario réutilisable](../conception/besoin/user-journeys/UJ-UC-13-scenario-reutilisable.md) |
| La maquette | [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) — post-MVP, pas de fiche |
| Les exigences non fonctionnelles | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-13 |
| Les décisions qui la contraignent | [ADR-018 — Contenu personnel de premier ordre, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-13, recette produite par anticipation et non exigible à la livraison MVP |
| Le statut dans le périmètre | [moscow.md, § UC-13 — Utiliser un scénario réutilisable](../conception/besoin/vision/moscow.md) |

### Gel d'espaces au downgrade de tier

| Axe de lecture | Où lire |
|---|---|
| Le besoin | [UC-15 — Gel d'espaces au downgrade de tier, § Objectif](../conception/besoin/usecases/UC-15-gel-espaces-downgrade-tier.md) |
| Les récits et les règles métier | aucune user story ni scénario Gherkin ; les règles métier vivent dans [UC-15 — Gel d'espaces au downgrade de tier, § Règles métier](../conception/besoin/usecases/UC-15-gel-espaces-downgrade-tier.md) elle-même |
| Le parcours d'usage | aucun — [Index des parcours utilisateur, § Tableau de correspondance](../conception/besoin/user-journeys/README.md) s'arrête à UC-14 ; UC-15 n'y figure pas |
| La maquette | aucune — UC-15 n'apparaît dans aucune ligne de [wireframes/README.md, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md) |
| Les exigences non fonctionnelles | aucune — [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) ne cite aucune NFR pour la ligne UC-15 |
| Les décisions qui la contraignent | [ADR-005 — Modèle de monétisation, § Décision](../architecture/decisions/ADR-005-modele-monetisation.md) ; [ADR-018 — Contenu personnel de premier ordre, § Décision](../architecture/decisions/ADR-018-espace-personnel-generalisation-space.md) |
| La spécification technique | aucune spécification dédiée |
| Les cas de recette | [cahier de stratégie de test et de recette, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md) — ligne UC-15, intégralement dérivée des règles métier en l'absence d'US |
| Le statut dans le périmètre | [moscow.md, § UC-15 — Classement arbitré : Post-MVP (spécifiés)](../conception/besoin/vision/moscow.md) |

---

## Ce que ce guide ne couvre pas

**Les exigences non fonctionnelles transverses**, communes à plusieurs fonctionnalités et non rattachées à une seule d'entre elles, vivent dans [`docs/conception/besoin/nfr/README.md`](../conception/besoin/nfr/README.md) — ce guide n'en produit pas d'index séparé.

**Les points ouverts** du corpus, hors ceux nommés ci-dessous, sont recensés dans [le cahier de stratégie de test et de recette, § Registre des points ouverts](../test/cahier-strategie-test-et-recette.md).

Trois points restent ouverts et le demeurent à l'issue de ce guide — aucune opinion n'est exprimée ici sur leur issue :

- **L'identité graphique** (logo, charte graphique, ressources visuelles), axe de conception d'interface — [`roadmap-entree-build.md`, § Annexe B — Prérequis hors jalon](roadmap-entree-build.md).
- **Le seuil chiffré du geste structurant de l'hypothèse H1**, axe métrique produit — [`vision-produit.md`, § Ce que le MVP doit démontrer](../conception/besoin/vision/vision-produit.md) en est propriétaire ; [`moscow.md`, § Instrumentation de validation du MVP](../conception/besoin/vision/moscow.md) y renvoie.
- **L'entrée J-12 du registre des risques** (méthode de vérification d'identité des invités), axe juridique — [`registre-risques.md`, § Axe juridique](registre-risques.md).

---

## Renvois

- Bandeau d'autorité et conventions de renvoi du corpus documentaire : [`guide-conventions-et-dod.md`, § Conventions de maintenance du corpus documentaire](guide-conventions-et-dod.md)
- Ordre d'autorité du corpus de conception : [`docs/conception/README.md`, § Ordre d'autorité entre artefacts](../conception/README.md)
- Priorisation produit, seule autorité MoSCoW : [`moscow.md`](../conception/besoin/vision/moscow.md)
- Matrice de traçabilité par use case : [`cahier-strategie-test-et-recette.md`, § Matrice de traçabilité](../test/cahier-strategie-test-et-recette.md)
- Rattachement écran par use case : [`wireframes/README.md`, § Table de couverture UC → fiche(s)](../conception/interface/wireframes/README.md)
- Index des décisions d'architecture : [`docs/architecture/decisions/README.md`](../architecture/decisions/README.md)

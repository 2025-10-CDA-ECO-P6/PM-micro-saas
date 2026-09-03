# Décisions en attente — Haversack

| Champ | Valeur |
|---|---|
| Statut | Index de consolidation par décideur — mis à jour le 2026-09-03, les lignes barrées portent une décision prise |
| Audience | équipe de build, responsable produit, cadrage juridique interne, comité des risques, conception d'interface |
| Sources | `docs/context/cahier-specifications-techniques.md` §11, `docs/test/cahier-strategie-test-et-recette.md` §12, `docs/context/dossier-conception-detaillee.md` §8, `docs/securite/dossier-securite.md` §5, `docs/conception/interface/zoning.md` §S10, `docs/gestion-projet/registre-risques.md`, chaque décision d'architecture portant une section § Points à trancher, `docs/architecture/specs/telemetrie.md` §6, `docs/architecture/specs/repli-temps-reel.md` §5, `docs/gestion-projet/plan-de-travail.md`, `docs/gestion-projet/roadmap-entree-build.md` |

---

## Bandeau de cadrage — ce que ce document fait et ne fait pas

Regroupe par décideur, tous axes confondus, les points que le corpus balise `[À TRANCHER — …]` ou équivalent, et ordonne les groupes par ce qu'ils bloquent dans la séquence J0 → J1 → J2 → J3 → lancement. C'est le seul regroupement de ce type dans le corpus — chaque registre cité reste organisé par section de son document hôte, jamais par décideur.

Pointe vers le registre qui porte le détail de chaque point ; ne recopie ni sa description longue, ni sa cotation, ni aucun décompte. Un renvoi qui pointe ne dérive pas quand sa cible bouge ; un contenu recopié devrait être resynchronisé à chaque changement de sa source — ce document choisit le renvoi.

Ne tranche aucune décision. Nomme qui décide, ce que la décision débloque, et ce qu'il faut avoir sous les yeux pour décider — sans proposer d'issue, sans recommander d'option, sans inventer de seuil.

En cas de conflit entre ce document et un registre cité, le registre cité fait foi.

---

## Groupe 1 — Équipe de build : structure (condition d'entrée en J0)

Les décisions de ce groupe sont déclarées bloquantes avant l'ouverture de J0.

### 1.a — Arborescence interne des couches .NET et TypeScript / Angular du mode local

**Où le manque vit, côté .NET.** `docs/architecture/structure-projets.md § 3 — Granularité des projets .NET (MVP)` marque `[À TRANCHER — J0]` la séparation en projets à l'intérieur de chaque couche — combien, et lesquels —, les noms de projet, et le contenu de chacun. Restent tranchés, et ne se rouvrent pas à J0 : les principes de granularité par couche (Domaine et Application en projets uniques, Infrastructure et Présentation en multi-projets), les namespaces de bounded context du Domaine, le noyau partagé `SharedKernel` (§ 4), l'isolation du module `Haversack.Infrastructure.Notifications` dès J0 (ADR-008 § Conséquences), et la Clean Architecture.

**Où le manque vit, côté TypeScript / Angular.** `structure-projets.md § 7 — Périmètre du mode local TypeScript` décrit le périmètre fonctionnel de cette couche (persistance navigateur, validations minimales, format de sérialisation, frontend Angular SPA et landing) sans nommer aucun répertoire, et porte désormais le même marqueur `[À TRANCHER — J0]` pour son emplacement.

**Ce que ça bloque.** Les tâches de `docs/gestion-projet/plan-de-travail.md` dont le champ `Périmètre d'écriture` porte `TROU — non nommable dans la maille` ne peuvent pas déclarer de module. Le champ `En conflit avec` garantit l'absence de recouvrement d'écriture entre deux tâches parallèles en comparant leurs modules déclarés — il ne protège donc pas ces tâches-là.

**Ce qu'il faut savoir pour répondre.** `plan-de-travail.md § 2 — Maille de désignation du périmètre d'écriture` énonce, dans son paragraphe sur le plancher de la maille, que dès que J0 tranche cette arborescence, le champ `Périmètre d'écriture` des tâches concernées pourra être affiné à un grain plus fin — sans qu'aucun autre champ de la tâche (but, dépendances, acceptation) n'ait à changer. La décision est donc localisée : elle ne rouvre pas le plan. `structure-projets.md § 3` fixe par ailleurs une convention de désignation qui permet aux documents dérivés de nommer un module avant que son nom de projet soit tranché.

**Portage.** Cette décision est portée par une tâche dédiée, et reste un préalable à l'ouverture de J0.

**Registre propriétaire.** `structure-projets.md § 3` pour la couche .NET ; `structure-projets.md § 7` pour la couche TypeScript / Angular ; `plan-de-travail.md § 2` pour l'effet sur le plan de travail.

### 1.b — Nom du projet de test d'architecture, et plateforme d'intégration continue

Un des `Critères de sortie factuels` de `roadmap-entree-build.md § 3.1 — J0 — Socle` exige que « Le test d'archi CI existe, tourne en pipeline, et échoue si une référence traverse une frontière de bounded context non autorisée. ». Deux tâches du plan de travail (`TB-006`, `TB-009`) déclarent un recouvrement d'écriture « à confirmer à J0 » précisément parce que le projet de test d'architecture qu'elles écrivent toutes deux n'est nommé par aucun document du corpus.

**Ce qu'il faut savoir pour répondre.** `docs/deploiement/README.md` déclare lui-même qu'aucun document de déploiement n'est produit à ce jour, et marque l'hébergeur `[non tranché]`. Aucun ADR ni spécification ne décrit la plateforme d'intégration continue, ses environnements ni son déclencheur.

**Registre propriétaire.** `docs/deploiement/README.md` ; `roadmap-entree-build.md § 3.1` pour le critère de sortie ; `plan-de-travail.md`, tâches `TB-006` et `TB-009`, pour le recouvrement en attente.

### Autres décisions de structure et de détail technique

Sans hiérarchie entre elles ; chacune est déjà nommée par un registre du corpus.

| Décision | Bloque | À savoir | Registre propriétaire |
|---|---|---|---|
| ~~Outil exact du test d'architecture~~ | **Tranché le 2026-09-03** — NetArchTest, l'alternative par script écartée | ne bloque plus TB-007 | `guide-conventions-et-dod.md § 6 § Non couvert par le corpus` |
| Nom du projet de test de bout en bout | — (non signalé comme bloquant par le corpus) | Distinct du projet de test d'architecture (1.b) | `cahier-specifications-techniques.md § 11` |
| Paramètres de hachage de mot de passe (Argon2id), seuils de limitation de débit, TTL du jeton de réinitialisation, fraîcheur de ré-authentification IdP | La définition des contrats `IPasswordHasher` / rate limiting / `ITokenValidator` en J2 | Regroupés sous le renvoi `B1.5` | `cahier-specifications-techniques.md § 11 § 5` |
| Code de refus (403/404) sur ressource d'un autre espace, schémas de requête/réponse par point d'entrée, attribution des codes 400/401, schéma du 429 | La clôture du contrat OpenAPI avant fin de J2 | Regroupés sous le renvoi `B1.10` | `cahier-specifications-techniques.md § 11 § 7` ; `cahier-strategie-test-et-recette.md § 12`, entrée `PO-13` |
| ~~Directives de sécurité de contenu et bibliothèque de sanitisation~~ | **Tranché le 2026-09-03** — directives au complet et `DomSanitizer` complété de DOMPurify à l'import. La « liste blanche » porte désormais sur l'**énumération fermée des types de nœuds**, le contenu d'un bloc étant un arbre typé et non du balisage | reste ouverte côté serveur la seule énumération, dérivée du même modèle (`B1.2`) | `cahier-specifications-techniques.md § 11 § 5` |
| Seuil de `schemaVersion` minimale maintenue côté serveur, horizon de rétention du registre des lots de migration | Le handler d'import/migration (`P7`) | — | `cahier-specifications-techniques.md § 11 § 8` |
| Seuil de tentatives (N) de la limitation anti-force-brute | Le chiffrage du rate limiting hybride par-IP/par-compte | Aucun cas de recette ne chiffre N | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-12` |
| UC-03 exception E2 — perte de connexion ou erreur de sauvegarde | La recette de cette exception | Non dérivable d'un scénario Gherkin ni d'une règle métier ferme ; couvert au mieux par le principe général de résilience réseau | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-09` |
| UC-06 exception E3 — MJ consulte une session `CLOSED` en lecture seule | La recette de la nuance additionnelle | Substantiellement déjà couvert par les cas de recette existants ; la nuance n'est pas dupliquée en cas redondant | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-10` |

---

## Groupe 2 — Responsable produit (condition d'entrée en J2, et une décision bloquante avant J0)

### 2.a — Instrumentation de validation du MVP : aucun critère d'acceptation

**Déclarée bloquante avant l'ouverture de J0.**

**Ce qui existe.** `docs/conception/besoin/vision/moscow.md § Instrumentation de validation du MVP` classe cette instrumentation `Must Have` et décrit en comportement observable les trois constats attendus — activation préparation, activation vue session, activation partage — ainsi que la mesure anonyme dès le mode local et la capture de contact non bloquante. `docs/conception/besoin/vision/vision-produit.md § 2.3 — Ce que le MVP doit démontrer` en fait l'instrument de constat exclusif de ses cinq hypothèses de validation (H1 à H5).

**Ce qui manque, et la nature exacte du trou.** Cette substance vit au-dessus du niveau use case et n'est jamais redescendue. L'ordre d'autorité du corpus (`docs/conception/README.md § Ordre d'autorité entre artefacts` : personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire) place les use cases comme source de vérité du besoin, dont dérivent user stories, parcours et exigences non fonctionnelles. Vérification faite sur les sous-dossiers `usecases`, `user-stories`, `user-journeys`, `parcours` et `nfr` de `docs/conception/besoin/` : aucun use case, aucune user story, aucun scénario Gherkin, aucune règle métier, aucun parcours et aucune fiche d'écran ne porte cette unité fonctionnelle. La seule occurrence trouvée côté NFR est une mise en garde de `NFR-CONF-02` contre une fuite de contenu local via télémétrie — pas une exigence d'instrumentation.

**Ce que ça bloque.** `docs/architecture/specs/telemetrie.md` est la seule spécification de cette instrumentation et se déclare, dans son bandeau `Statut`, « cadre à compléter — non implémentable en l'état, les trous nommés ci-dessous bloquent le passage en développement ». Cette instrumentation est le seul intrant du point de décision `[DÉCISION MARCHÉ]` qui conditionne l'entrée en J2 : `roadmap-entree-build.md § 3.3` fonde son critère de décision sur la télémétrie par pilier, et `§ 3.4` pose que l'entrée en J2 est conditionnée, cumulativement, par J1 stable et `[DÉCISION MARCHÉ] = go`. `plan-de-travail.md § 10 — Ce que ce plan n'ordonne pas` reformule la même dépendance dans son premier paragraphe : ce plan hérite les effets de ce point de décision sans le décomposer en tâches.

**Ce qu'il faut savoir pour répondre.** Combler ce trou suppose d'écrire du besoin — un use case ou une user story, avec ses propres critères d'acceptation. C'est une décision produit, pas une correction de cohérence : le corpus ne porte nulle part la substance qui permettrait de la dériver mécaniquement.

**Registre propriétaire.** `moscow.md § Instrumentation de validation du MVP` ; `vision-produit.md § 2.3` ; `telemetrie.md` (bandeau `Statut` et § 6) ; `roadmap-entree-build.md § 3.3` et `§ 3.4` ; `plan-de-travail.md § 10`.

### Autres décisions produit

| Décision | Bloque | À savoir | Registre propriétaire |
|---|---|---|---|
| Seuils de réactivité perçue (exigences non fonctionnelles de performance) | La recette de performance reste une observation qualitative tant que non tranché | Seuil chiffré non dérivable du corpus | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-01` |
| Seuil de repli du transport temps réel (SSE) vers l'interrogation périodique | La clôture opérationnelle du jalon Partage + temps réel, pas son entrée | Seuil chiffré non dérivable du corpus | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-02` |
| Résolution de la branche fédérée à adresse non vérifiée (`ReclaimViaFederatedProof`) | Le rattachement de l'opération à J2/`B1.5` | Deux décideurs : responsable produit pour la ratification sécurité, cadrage juridique interne pour la facette protection des données | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-03` ; `ADR-015 § 2.3` |
| Formulation du délai de purge (« J+30 ») en critère de recette | La clôture du cas de recette correspondant | Le délai lui-même est dérivable de la roadmap ; sa formulation en critère de recette reste ouverte | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-05` |
| Activation de l'espace personnel comme zone d'atterrissage par défaut | L'inscription de cette posture dans les user stories `UC-01`/`UC-02` | `ADR-018 § Geste capture-first — espace personnel comme zone d'atterrissage par défaut — RECOMMANDÉ EN MVP` la porte comme une recommandation dont le changement de posture « doit être validé explicitement avant d'être inscrit dans les user stories » | `ADR-018`, section précitée |
| Point de décision `[DÉCISION MARCHÉ]` lui-même | L'entrée en J2 (cumulé avec J1 stable) | Non vérifiable en intégration continue | `roadmap-entree-build.md § 3.3` |
| Ordre de dégel `UC-15` pour un tier intermédiaire borné (post-MVP) | Sans impact MVP — le tier binaire actuel implique un dégel total | Non fixé | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-07` |
| Réimport de fichier de sauvegarde (`UC-01`, `US-01-08`, post-MVP) | Aucun cas de recette MVP | Règles de validation tracées pour reprise ultérieure | `cahier-strategie-test-et-recette.md § 12`, entrée `PO-08` |

**Ce point l'était et ne l'est plus.** Le critère exact du « geste structurant » de l'hypothèse H1 est **tranché le 2026-09-03** : un dossier créé, ou un document déplacé hors de « Non classés ». Le seuil de documents de l'activation préparation est fixé à trois, l'opérationnalisation de l'« usage réel constaté » à au moins une action du MJ en session, et le transport de la mesure à des compteurs locaux transmis à la création de compte — avec le biais que ce transport induit, nommé dans [`vision-produit.md §2.3`](../conception/besoin/vision/vision-produit.md). Registre propriétaire de ces décisions : [`moscow.md § Instrumentation de validation du MVP`](../conception/besoin/vision/moscow.md).

---

## Groupe 3 — Modélisation domaine

| Décision | Bloque | À savoir | Registre propriétaire |
|---|---|---|---|
| ~~Champs des schémas `propertiesSchema` des six types système~~ | **Tranché le 2026-09-03** — **vide au MVP**, absence actée sur le précédent de `live_note` ; leur contenu vit dans les blocs | ne bloque plus TB-066 | `docs/architecture/specs/document-properties-schemas.md § 5 — Champs de propertiesSchema par type — dérivés du domaine ou marqués à trancher` |
| ~~Comportement de `SetProperties()` sans type déclaré~~ | **Tranché le 2026-09-03** — `properties` **n'existe pas sans type**, l'écriture est refusée | reste ouvert le contenu exact de la validation « permissive par défaut » des types custom | `document-properties-schemas.md § 1` |
| Réconciliation d'un désaccord de décompte de clés étrangères inter-modules entre `ADR-009` (en renvoi vers `ADR-011`) et la matrice propre d'`ADR-011` | La fiabilité du modèle de cascade référentielle documenté | Désaccord non arbitré, signalé tel quel par le cahier de spécifications techniques | `cahier-specifications-techniques.md § 11 § 4 — Modèle de données & domaine` |

---

## Groupe 4 — Cadrage juridique interne (bloque le lancement dans l'Union européenne, pas le build de J3)

**Axes du dossier juridique marqués `[À TRANCHER — FLAG JURISTE]`.** `cahier-specifications-techniques.md § 11 § 6 — Conformité RGPD technique` les énumère sous sa section consacrée à la conformité au règlement sur la protection des données (qualification mineurs, qualification sous-traitant et périmètre du contrat associé, mise en balance de l'intérêt légitime, suffisance du hard-delete de l'espace personnel, sort du contenu d'une coquille reprise par reclaim-in-place). Registre propriétaire : cette même section.

**Informations à fournir, distinctes des axes précédents.** Les champs d'identité de l'entité éditrice ne sont pas des arbitrages mais des informations à fournir dès qu'elles existent : dénomination, forme juridique, siège social, immatriculation dans `docs/securite/conformite/mentions-legales.md § 1 — Éditeur du service` ; identité et coordonnées du responsable de traitement dans `docs/securite/conformite/politique-confidentialite.md`, section correspondante ; les points d'identité, d'hébergeur et de sous-traitants équivalents portés par `docs/securite/conformite/politique-cookies.md`, `docs/securite/conformite/cgu.md` et le squelette `docs/securite/conformite/dpa-skeleton.md`. Chacun de ces documents se déclare lui-même brouillon non opposable, non validé juriste.

**Le point de validation juridique lui-même.** `roadmap-entree-build.md § 3.5 — [VALIDATION JURIDIQUE EU] — validation juridique pré-lancement` bloque le lancement dans l'Union européenne — mise en production commerciale — et non le build technique de J3 ; `plan-de-travail.md § 10` reformule la même distinction dans son second point.

---

## Groupe 5 — Comité des risques

**Statut de l'entrée `J-12` du registre des risques — non arbitrable aujourd'hui, axe juridique.** L'entrée `J-12` (méthode de vérification d'identité des invités, article 12 §6 du RGPD) est marquée `[À TRANCHER — COMITÉ DES RISQUES]` : le registre signale lui-même que la frontière entre « lacune réelle » et « posture existante » n'est pas tranchée par lui, et attend un statut en comité des risques.

**Registre propriétaire.** `registre-risques.md § 2.5 — Points à ratifier, dettes majeures et questions ouvertes à lacune réelle` pour le signalement de l'arbitrage attendu ; `§ 3.3 — Axe juridique`, entrée `J-12`, pour le détail.

---

## Groupe 6 — Conception d'interface (préalable hors jalon)

**Identité graphique — non arbitrable aujourd'hui, axe conception d'interface.** Logo, charte graphique/typo/grille et ressources visuelles constituent un préalable hors jalon porté par `docs/conception/interface/**`. Registre propriétaire : `roadmap-entree-build.md § Annexe B — Prérequis hors jalon`.

**Points ouverts du modèle de navigation.** Transitions non modélisées dans le graphe de navigation, granularité de deux arêtes à préciser, arête `Tableau → SuppressionRGPD` directe ou par composition, `NavDossiers`/`NavDossiersPerso` en écran distinct ou interaction in-page, libellés à harmoniser entre le graphe et les wireframes, second point de passage joueur → MJ — chacun marqué `[À TRANCHER — graphe S3]`. Registre propriétaire : `zoning.md § S10 — Points ouverts sur le modèle de navigation (graphe S3)`.

---

## Points déjà tranchés ailleurs

Un registre peut porter une entrée qu'une décision postérieure a déjà close ; recensé ici pour qu'aucune tâche ne s'ouvre sans objet.

- **`PO-06` — Nom de l'object store racine.** Déjà marquée **RÉSOLU** dans `cahier-strategie-test-et-recette.md § 12` : le store racine est nommé `spaces` (`ADR-017 § 1.1`, `ADR-018`).
- **`PO-04` — Réconciliation de nomenclature de jalon (« J1 »).** Déjà marquée **RÉSOLU** dans `cahier-strategie-test-et-recette.md § 12`, décision produit ratifiée, renvoi seul à `roadmap-entree-build.md § 5`.
- **Contradiction entre `ADR-016 § 1.2` et `ADR-017 § 1.1` sur la persistance locale des entités de session.** Tranchée : `ADR-017 § 1.1 § Object stores` porte désormais le périmètre propre du store local, dérivé de `UC-01`, `UC-06` et `UC-07`, plus large que le payload de migration décrit par `ADR-016 § 1.2 § Périmètre sérialisé`.
- **`T-06` du registre des risques — « Résidu de nommage de l'espace de stockage local ».** Décrit un résidu qui n'existe plus : `ADR-017 § 1.1` nomme la racine `spaces`. Cette entrée reste ouverte dans `registre-risques.md § 3.1 — Axe technique` au moment de la rédaction du présent document — **à clore**. Sa clôture touche la matrice de criticité inhérente (`§ 2.1`) et le décompte des entrées à criticité résiduelle non abaissée (`§ 2.2`) : ce document ne la clôt pas, il la signale.

---

## Registres propriétaires

- `docs/context/cahier-specifications-techniques.md § 11` — points ouverts et confirmations attendues à l'entrée en build, groupés par section du cahier (contraintes techniques, modèle de données, sécurité, RGPD, contrats d'API, migration, observabilité, exigences non fonctionnelles).
- `docs/test/cahier-strategie-test-et-recette.md § 12` — registre des points ouverts, identifiants `PO-nn`, avec propriétaire de décision par entrée.
- `docs/context/dossier-conception-detaillee.md § 8` — points ouverts et limites de vérifiabilité de la conception détaillée.
- `docs/securite/dossier-securite.md § 5` — points ouverts et dette de sécurité.
- `docs/conception/interface/zoning.md § S10` — points ouverts sur le modèle de navigation (graphe S3).
- `docs/gestion-projet/registre-risques.md` — registre d'évaluation des risques, entrées `T-nn` (technique), `P-nn` (produit), `J-nn` (juridique), `D-nn` (délai-coût).
- `docs/architecture/decisions/` — chaque décision d'architecture portant une section `## Points à trancher` y nomme sa propre dette technique, rattachée à son jalon ou à son code de renvoi.
- `docs/architecture/specs/telemetrie.md § 6` — récapitulatif des points à trancher de l'instrumentation de validation du MVP.
- `docs/architecture/specs/repli-temps-reel.md § 5` — récapitulatif des points à trancher du repli du transport temps réel.

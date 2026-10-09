# Décisions en attente

| Champ | Valeur |
|---|---|
| Statut | Index de consolidation par décideur — mis à jour le 2026-09-03, les lignes barrées portent une décision prise |
| Audience | équipe de build, responsable produit, cadrage juridique interne, comité des risques, conception d'interface |
| Sources | `docs/reference/cahier-specifications-techniques.md` §11, `docs/assurance/strategie-de-test.md` §12, `docs/reference/conception-detaillee.md` §11, `docs/assurance/securite/dossier-securite.md` §5, `docs/interface/zonage.md` §S10, `docs/projet/registre-des-risques.md`, chaque décision d'architecture portant une section § Points à trancher, `docs/architecture/specs/telemetrie.md` §6, `docs/architecture/specs/repli-temps-reel.md` §5, `docs/projet/plan-de-travail.md`, `docs/projet/roadmap-de-build.md` |

---

## Bandeau de cadrage — ce que ce document fait et ne fait pas

Regroupe par décideur, tous axes confondus, les points que le corpus balise `[À TRANCHER — …]` ou équivalent, et ordonne les groupes par ce qu'ils bloquent dans la séquence J0 → J1 → J2 → J3 → lancement. C'est le seul regroupement de ce type dans le corpus — chaque registre cité reste organisé par section de son document hôte, jamais par décideur.

Pointe vers le registre qui porte le détail de chaque point ; ne recopie ni sa description longue, ni sa cotation, ni aucun décompte. Un renvoi qui pointe ne dérive pas quand sa cible bouge ; un contenu recopié devrait être resynchronisé à chaque changement de sa source — ce document choisit le renvoi.

Ne tranche aucune décision. Nomme qui décide, ce que la décision débloque, et ce qu'il faut avoir sous les yeux pour décider — sans proposer d'issue, sans recommander d'option, sans inventer de seuil.

En cas de conflit entre ce document et un registre cité, le registre cité fait foi.

---

## Décisions ouvertes

### Groupe 1 — Équipe de build : structure (condition d'entrée en J0)

Les décisions de ce groupe sont déclarées bloquantes avant l'ouverture de J0.

#### 1.a — Arborescence interne des couches .NET et TypeScript / Angular du mode local

**Où le manque vit, côté .NET.** `docs/architecture/structure-projets.md § 3 — Granularité des projets .NET (MVP)` marque `[À TRANCHER — J0]` la séparation en projets à l'intérieur de chaque couche — combien, et lesquels —, les noms de projet, et le contenu de chacun. Restent tranchés, et ne se rouvrent pas à J0 : les principes de granularité par couche (Domaine et Application en projets uniques, Infrastructure et Présentation en multi-projets), les namespaces de bounded context du Domaine, le noyau partagé `SharedKernel` (§ 4), l'isolation du module `Haversack.Infrastructure.Notifications` dès J0 (ADR-008 § Conséquences), et la Clean Architecture.

**Où le manque vit, côté TypeScript / Angular.** `structure-projets.md § 7 — Périmètre du mode local TypeScript` décrit le périmètre fonctionnel de cette couche (persistance navigateur, validations minimales, format de sérialisation, frontend Angular SPA et landing) sans nommer aucun répertoire, et porte désormais le même marqueur `[À TRANCHER — J0]` pour son emplacement.

**Ce que ça bloque.** Aujourd'hui, une seule tâche de `docs/projet/plan-de-travail.md` porte le marqueur `TROU` : [`TB-104`](plan-de-travail.md), l'échafaudage du projet Angular (`tools/VerifMailleAgregat` la compte). Les autres tâches de la couche cliente sont nommées par la **préoccupation** que le corpus donne à leurs modules ailleurs, plutôt que par un chemin de fichier (`plan-de-travail.md § 2`) — ce qui contourne le manque pour elles, pas pour `TB-104`. Une tranche `TROU` n'est pas protégée par le champ `En conflit avec`, qui compare des modules déclarés : le recouvrement de `TB-104` avec une autre tranche reste non décidable tant que `structure-projets.md § 7` n'a pas nommé l'emplacement de la couche cliente.

**Ce qu'il faut savoir pour répondre.** `plan-de-travail.md § 2` énonce que la maille par agrégat **ne dépend pas** de cette décision : un agrégat est nommé et stable indépendamment de sa future disposition en fichiers, et les six modules de la couche cliente sont nommés par leur préoccupation plutôt que par un répertoire. La décision d'arborescence ne rouvre donc pas le plan — elle conditionne la disposition en fichiers du futur code, pas la désignation des modules que ce plan porte déjà. `structure-projets.md § 3` fixe par ailleurs une convention de désignation qui permet aux documents dérivés de nommer un module avant que son nom de projet soit tranché.

**Portage.** Cette décision est portée par une tâche dédiée, et reste un préalable à l'ouverture de J0.

**Registre propriétaire.** `structure-projets.md § 3` pour la couche .NET ; `structure-projets.md § 7` pour la couche TypeScript / Angular ; `plan-de-travail.md § 2` pour l'effet sur le plan de travail.

#### 1.b — Nom du projet de test d'architecture, et plateforme d'intégration continue

Un des `Critères de sortie factuels` de `roadmap-de-build.md § 3.1 — J0 — Socle` exige que « Le test d'archi CI existe, tourne en pipeline, et échoue si une référence traverse une frontière de bounded context non autorisée. ». Deux tâches du plan de travail (`TB-006`, `TB-009`) déclarent un recouvrement d'écriture « à confirmer à J0 » précisément parce que le projet de test d'architecture qu'elles écrivent toutes deux n'est nommé par aucun document du corpus.

**Ce qu'il faut savoir pour répondre.** `docs/architecture/deploiement/README.md` déclare lui-même qu'aucun document de déploiement n'est produit à ce jour, et marque l'hébergeur `[À TRANCHER — hébergement]`. Aucun ADR ni spécification ne décrit la plateforme d'intégration continue, ses environnements ni son déclencheur.

**Registre propriétaire.** `docs/architecture/deploiement/README.md` ; `roadmap-de-build.md § 3.1` pour le critère de sortie ; `plan-de-travail.md`, tâches `TB-006` et `TB-009`, pour le recouvrement en attente.

#### 1.c — Redécoupage du module `châssis` dans la maille de désignation

**Où le manque vit.** `plan-de-travail.md § 2 — Natures du périmètre d'écriture` nomme `châssis` comme un seul module transverse. `zonage.md § S7 — Châssis applicatif` y range pourtant des préoccupations distinctes : recherche (composant de châssis omniprésent), bandeaux (durabilité, confidentialité), fonctions cloud visibles mais désactivées en mode local, accès compte.

**Ce que ça bloque.** La grappe de tranches déclarées en conflit sur ce seul module est passée de deux (`TB-058`, `TB-093`) à cinq (`TB-058`, `TB-093`, `TB-078`, `TB-097`, `TB-103`) au fil des dernières vagues de ce plan — mesuré en `plan-de-travail.md § 9`. Chacune est sérialisée avec les quatre autres faute d'un découpage plus fin, alors que leurs préoccupations réelles (recherche, instrumentation de validation, châssis mode local) ne se recouvrent pas fonctionnellement. Le levier de largeur de graphe qu'un découpage par préoccupation ouvrirait n'est pas exploité.

**Ce qu'il faut savoir pour répondre.** Un redécoupage nommerait, à la place d'un seul `châssis`, les préoccupations distinctes que `zonage.md § S7` porte déjà en prose — sans inventer de substance neuve, seulement une maille plus fine. C'est une décision sur la maille elle-même (`plan-de-travail.md § 2`), qui s'appliquerait à toute tranche future de la couche cliente citant `châssis`, pas seulement aux cinq déjà écrites.

**Registre propriétaire.** `plan-de-travail.md § 2` pour la maille ; `zonage.md § S7` pour les préoccupations distinctes qu'un découpage nommerait ; `plan-de-travail.md § 9` pour la grappe de conflit mesurée.

#### Autres décisions de structure et de détail technique

Sans hiérarchie entre elles ; chacune est déjà nommée par un registre du corpus.

| Où le manque vit | Ce que cela bloque | Ce qu'il faut savoir | Registre propriétaire |
|---|---|---|---|
| Contrôle de l'appariement `But` d'épique / paragraphe `Critère de sortie` du use case porté (`plan-de-travail.md § 4`) | Aucun test, aucun linter ne le vérifie aujourd'hui | Défaut trouvé et corrigé à la main trois fois dans ce plan (`US-UC-14`, `US-UC-01`, deux fois) ; candidat naturel pour un vérificateur, sur le modèle de `tools/VerifCouvertureRecette` | `plan-de-travail.md § 4` ; `matrice-de-priorites.md`, paragraphes `Critère de sortie` par use case |
| Épique départageant un couple de tranches inter-épiques (`plan-de-travail.md § 1`, règle (b bis)) | — (aucun usage actuel ne l'invoque) | La règle ne dit pas quelle épique départage un couple dont les deux tranches appartiennent à deux épiques distinctes ; une invocation fautive de ce cas a été retirée de `TB-069` le 2026-09-04 (le couple était en réalité départagé par dépendance directe, la règle ne s'y appliquait pas) | `plan-de-travail.md § 1`, règle (b bis) |
| ~~Outil exact du test d'architecture~~ | **Tranché le 2026-09-03** — les deux mécanismes envisagés sont retenus, chacun sur ses règles : NetArchTest sur la règle 2, un contrôle du graphe des références de projet sur les règles 1 et 3 | ne bloque plus TB-007 | `conventions-dod.md § 6 § Non couvert par le corpus` |
| Nom du projet de test de bout en bout | — (non signalé comme bloquant par le corpus) | Distinct du projet de test d'architecture (1.b) | `plan-de-travail.md § 2` pour sa désignation par le rôle ; `structure-projets.md § 3` pour la convention de désignation d'un projet dont le nom n'est pas tranché |
| Paramètres de hachage de mot de passe (Argon2id), seuils de limitation de débit, TTL du jeton de réinitialisation, fraîcheur de ré-authentification IdP | La définition des contrats `IPasswordHasher` / rate limiting / `ITokenValidator` en J2 | Regroupés sous le renvoi `B1.5` | `cahier-specifications-techniques.md § 11 § 5` |
| Code de refus (403/404) sur ressource d'un autre espace, schémas de requête/réponse par point d'entrée, attribution des codes 400/401, schéma du 429 | La clôture du contrat OpenAPI avant fin de J2 | Regroupés sous le renvoi `B1.10` | `cahier-specifications-techniques.md § 11 § 7` ; `strategie-de-test.md § 12`, entrée `PO-13` |
| ~~Directives de sécurité de contenu et bibliothèque de sanitisation~~ | **Tranché le 2026-09-03** — directives au complet et `DomSanitizer` complété de DOMPurify à l'import. La « liste blanche » porte désormais sur l'**énumération fermée des types de nœuds**, le contenu d'un bloc étant un arbre typé et non du balisage | reste ouverte côté serveur la seule bibliothèque de sanitisation (`B1.2`) ; l'énumération des types de nœuds est tranchée (`sanitisation-csp.md § 5`) | `cahier-specifications-techniques.md § 11 § 5` |
| Seuil de `schemaVersion` minimale maintenue côté serveur, horizon de rétention du registre des lots de migration | Le handler d'import/migration (`P7`) | — | `cahier-specifications-techniques.md § 11 § 8` |
| Seuil de tentatives (N) de la limitation anti-force-brute | Le chiffrage du rate limiting hybride par-IP/par-compte | Aucun cas de recette ne chiffre N | `strategie-de-test.md § 12`, entrée `PO-12` |
| UC-03 exception E2 — perte de connexion ou erreur de sauvegarde | La recette de cette exception | Non dérivable d'un scénario Gherkin ni d'une règle métier ferme ; couvert au mieux par le principe général de résilience réseau | `strategie-de-test.md § 12`, entrée `PO-09` |
| UC-06 exception E3 — MJ consulte une session `CLOSED` en lecture seule | La recette de la nuance additionnelle | Substantiellement déjà couvert par les cas de recette existants ; la nuance n'est pas dupliquée en cas redondant | `strategie-de-test.md § 12`, entrée `PO-10` |
| Hébergeur de l'application, marqué `[À TRANCHER — hébergement]` | — (non signalé comme bloquant par le corpus) | `stack.md` recense des options (Railway, Supabase, Render, auto-hébergé) sans arbitrer. Le dossier d'architecture ne cartographie que la contrainte structurelle, les sticky sessions imposées par le transport temps réel, et n'invente aucune topologie : aucun choix de fournisseur n'est acté dans le corpus | `docs/reference/dossier-architecture.md § 7 — Déploiement` et `§ 8 — Limites cartographiques` ; `docs/architecture/stack.md` |

---

### Groupe 2 — Responsable produit (condition d'entrée en J3, et une décision bloquante avant J0)

#### 2.a — Instrumentation de validation du MVP : aucun critère d'acceptation au niveau use case

**Déclarée bloquante avant l'ouverture de J0.**

**Ce qui existe.** `docs/besoin/vision/perimetre/matrice-de-priorites.md § Instrumentation de validation du MVP` classe cette instrumentation `Must Have` et décrit en comportement observable les trois constats attendus — activation préparation, activation vue session, activation partage — ainsi que la mesure anonyme dès le mode local et la capture de contact non bloquante. `docs/besoin/vision/vision.md § 2.3 — Ce que le MVP doit démontrer` en fait l'instrument de constat exclusif de ses cinq hypothèses de validation (H1 à H5).

**Ce qui manque, et la nature exacte du trou.** Les seuils, le transport de la mesure et la capture de contact sont tranchés (2026-09-03, [`matrice-de-priorites.md § Instrumentation de validation du MVP`](../besoin/vision/perimetre/matrice-de-priorites.md)). Seul reste ouvert le critère d'acceptation au niveau use case : cette substance vit au-dessus du niveau use case et n'est jamais redescendue. L'ordre d'autorité du corpus (`docs/reference/ordre-d-autorite.md § Ordre d'autorité entre artefacts` : personas → vision produit → use cases → user journeys / user stories / NFR → domaine → glossaire) place les use cases comme source de vérité du besoin, dont dérivent user stories, parcours et exigences non fonctionnelles. Vérification faite sur les sous-dossiers `usecases`, `user-stories`, `user-journeys`, `parcours` et `nfr` de `docs/besoin/` : aucun use case, aucune user story, aucun scénario Gherkin, aucune règle métier, aucun parcours et aucune fiche d'écran ne porte cette unité fonctionnelle. La seule occurrence trouvée côté NFR est une mise en garde de `NFR-CONF-02` contre une fuite de contenu local via télémétrie — pas une exigence d'instrumentation.

**Ce que ça bloque.** `docs/architecture/specs/telemetrie.md` est la seule spécification de cette instrumentation. Son bandeau `Statut` déclare tranchés (2026-09-03) cinq des six points qui bloquaient le passage en développement ; seul le schéma d'événements formalisé reste ouvert, il se dérive de ces décisions au moment de l'implémentation. Cette instrumentation est le seul intrant de mesure d'usage du point de décision `[DÉCISION MARCHÉ]` qui conditionne l'entrée en J3 (les entretiens en sont l'autre intrant) : `roadmap-de-build.md § 3.4` fonde son critère de décision sur la télémétrie par pilier, mesurée chez les comptes, et `§ 3.6` pose que l'entrée en J3 est conditionnée, cumulativement, par J2 achevé et `[DÉCISION MARCHÉ] = go`. `plan-de-travail.md § 10 — Ce que ce plan n'ordonne pas` reformule la même dépendance dans son premier paragraphe : ce plan hérite les effets de ce point de décision sans le décomposer en tâches.

**Ce qu'il faut savoir pour répondre.** Combler ce trou suppose d'écrire du besoin — un use case ou une user story, avec ses propres critères d'acceptation. C'est une décision produit, pas une correction de cohérence : le corpus ne porte nulle part la substance qui permettrait de la dériver mécaniquement.

**Registre propriétaire.** `matrice-de-priorites.md § Instrumentation de validation du MVP` ; `vision.md § 2.3` ; `telemetrie.md` (bandeau `Statut` et § 6) ; `roadmap-de-build.md § 3.4` et `§ 3.6` ; `plan-de-travail.md § 10`.

#### Autres décisions produit

| Où le manque vit | Ce que cela bloque | Ce qu'il faut savoir | Registre propriétaire |
|---|---|---|---|
| Seuils de réactivité perçue (exigences non fonctionnelles de performance) | La recette de performance reste une observation qualitative tant que non tranché | Seuil chiffré non dérivable du corpus | `strategie-de-test.md § 12`, entrée `PO-01` |
| Seuil de repli du transport temps réel (SSE) vers l'interrogation périodique | La clôture opérationnelle du jalon Partage + temps réel, pas son entrée | Seuil chiffré non dérivable du corpus | `strategie-de-test.md § 12`, entrée `PO-02` |
| Résolution de la branche fédérée à adresse non vérifiée (`ReclaimViaFederatedProof`) | Le rattachement de l'opération à J2/`B1.5` | Deux décideurs : responsable produit pour la ratification sécurité, cadrage juridique interne pour la facette protection des données | `strategie-de-test.md § 12`, entrée `PO-03` ; `ADR-015 § 2.3` |
| Formulation du délai d'effacement de compte (1 mois, extensible à 2 mois) en critère de recette | La clôture du cas de recette correspondant | Le délai lui-même est tranché par `ADR-012 § 6` ; sa formulation en critère de recette reste ouverte | `strategie-de-test.md § 12`, entrée `PO-05` |
| ~~Activation de l'espace personnel comme zone d'atterrissage par défaut~~ | **DÉCIDÉ 2026-06-12** — comportement opérationnel en MVP, arbitré explicitement | ne bloque plus l'inscription de cette posture dans les user stories `UC-01`/`UC-02` | [`matrice-de-priorites.md § Espace personnel — conteneur par défaut et capture-first`](../besoin/vision/perimetre/matrice-de-priorites.md#espace-personnel--conteneur-par-défaut-et-capture-first-must-have--adr-018) |
| Point de décision `[DÉCISION MARCHÉ]` lui-même | L'entrée en J3 (cumulé avec J2 achevé) | Non vérifiable en intégration continue | `roadmap-de-build.md § 3.4` |
| Ordre de dégel `UC-15` pour un tier intermédiaire borné (post-MVP) | Sans impact MVP — le tier binaire actuel implique un dégel total | Non fixé | `strategie-de-test.md § 12`, entrée `PO-07` |
| Réimport de fichier de sauvegarde (`UC-01`, `US-01-08`, post-MVP) | Aucun cas de recette MVP | Règles de validation tracées pour reprise ultérieure | `strategie-de-test.md § 12`, entrée `PO-08` |
| Priorité `Could Have` de capacités absentes de `matrice-de-priorites.md`, dans les sections `Stories exclues ou repoussées` des fiches `US-UC-*` (mesuré : 9 lignes, hors la seule tracée à `matrice-de-priorites.md § UC-14` — filtre par tag) | Aucune tranche ; ne bloque rien | Même famille que le sous-grain MoSCoW retiré des fiches le 2026-09-04, mais au grain de la **capacité** plutôt que de la story ; retirer la valeur coûterait une exclusion argumentée pour gagner une étiquette — non rouvert lors de cette mission. Mise à jour du 2026-10-07 : la [matrice de priorités](../besoin/vision/perimetre/matrice-de-priorites.md) attribue désormais seule ces priorités, à tout grain (entrée de backlog 0032, `docs/project/backlog/0032-moscow-attribue-seul-les-priorites-a-tout-grain.md @ 51a7cdc`) ; les fiches y renvoient | `docs/besoin/stories/US-UC-*.md`, sections `Stories exclues ou repoussées` |
| Création directe d'un scénario réutilisable dans l'espace personnel, sans passer par un espace partagé | Le chemin de création des scénarios réutilisables décrit par `US-13-01` | Question produit non arbitrée, à trancher par le responsable produit ; `US-UC-13` ne décrit que le marquage d'un scénario existant | `docs/besoin/stories/US-UC-13-scenario-reutilisable.md`, § Questions ouvertes, point 3 |
| Visibilité, pour les autres membres, du retrait d'un membre (`UC-11`, `US-UC-11` § Questions ouvertes, Q2) | Le message et l'affichage du retrait côté joueurs, pas le retrait lui-même | Non tranchable sur pièces : la question est portée au guide d'entretien (`UC-11`), à trancher par le responsable produit | [`US-UC-11`](../besoin/stories/US-UC-11-gerer-membres-espace-partage.md) |

**Ce point l'était et ne l'est plus.** Le critère exact du « geste structurant » de l'hypothèse H1 est **tranché le 2026-09-03** : un dossier créé, ou un document déplacé hors de « Non classés ». Le seuil de documents de l'activation préparation est fixé à trois, l'opérationnalisation de l'« usage réel constaté » à au moins une action du MJ en session, et le transport de la mesure à des compteurs locaux transmis à la création de compte — avec le biais que ce transport induit, nommé dans [`vision.md §2.3`](../besoin/vision/vision.md). Registre propriétaire de ces décisions : [`matrice-de-priorites.md § Instrumentation de validation du MVP`](../besoin/vision/perimetre/matrice-de-priorites.md).

---

### Groupe 3 — Modélisation domaine

| Où le manque vit | Ce que cela bloque | Ce qu'il faut savoir | Registre propriétaire |
|---|---|---|---|
| ~~Champs des schémas `propertiesSchema` des six types système~~ | **Tranché le 2026-09-03** — **vide au MVP**, absence actée sur le précédent de `live_note` ; leur contenu vit dans les blocs | ne bloque plus TB-066 | `docs/architecture/specs/document-properties-schemas.md § 5 — Champs de propertiesSchema par type — dérivés du domaine ou marqués à trancher` |
| ~~Comportement de `SetProperties()` sans type déclaré~~ | **Tranché le 2026-09-03** — `properties` **n'existe pas sans type**, l'écriture est refusée | reste ouvert le contenu exact de la validation « permissive par défaut » des types custom | `document-properties-schemas.md § 1` |
| ~~Réconciliation d'un désaccord de décompte de clés étrangères inter-modules entre `ADR-009` (en renvoi vers `ADR-011`) et la matrice propre d'`ADR-011`~~ | **DÉCIDÉ 2026-09-02** — [`ADR-009`](../decisions/ADR-009-fk-campaign-owner.md), corrigé le 2026-09-02 (commit `f4214d1`), annonce 17 FK cross-module et renvoie à la matrice d'[`ADR-011`](../decisions/ADR-011-cascade-integrite-referentielle.md), qui en recense 17 : les deux concordent | ne bloque plus la fiabilité du modèle de cascade référentielle documenté | `cahier-specifications-techniques.md § 4 — Données` |

---

### Groupe 4 — Cadrage juridique interne (bloque le lancement dans l'Union européenne, pas le build de J3)

**Axes du dossier juridique marqués `[À VALIDER — JURISTE]`.** `cahier-specifications-techniques.md § 11 § 6 — Conformité` les énumère sous sa section consacrée à la conformité au règlement sur la protection des données (qualification mineurs, qualification sous-traitant et périmètre du contrat associé, mise en balance de l'intérêt légitime, suffisance du hard-delete de l'espace personnel, sort du contenu d'une coquille reprise par reclaim-in-place). Registre propriétaire : cette même section.

**Informations à fournir, distinctes des axes précédents.** Les champs d'identité de l'entité éditrice ne sont pas des arbitrages mais des informations à fournir dès qu'elles existent : dénomination, forme juridique, siège social, immatriculation dans `docs/assurance/conformite/public/mentions-legales.md § 1 — Éditeur du service` ; identité et coordonnées du responsable de traitement dans `docs/assurance/conformite/public/politique-confidentialite.md`, section correspondante ; les points d'identité, d'hébergeur et de sous-traitants équivalents portés par `docs/assurance/conformite/public/politique-cookies.md`, `docs/assurance/conformite/public/cgu.md` et le squelette `docs/assurance/conformite/cadres/dpa-skeleton.md`. Chacun de ces documents se déclare lui-même brouillon non opposable, non validé juriste.

**Le point de validation juridique lui-même.** `roadmap-de-build.md § 3.5 — [VALIDATION JURIDIQUE EU] — validation juridique pré-lancement` bloque le lancement dans l'Union européenne — mise en production commerciale — et non le build technique de J3 ; `plan-de-travail.md § 10` reformule la même distinction dans son second point.

---

### Groupe 5 — Comité des risques

**Statut de l'entrée `J-12` du registre des risques — non arbitrable aujourd'hui, axe juridique.** L'entrée `J-12` (méthode de vérification d'identité des invités, article 12 §6 du RGPD) est marquée `[À TRANCHER — COMITÉ DES RISQUES]` : le registre signale lui-même que la frontière entre « lacune réelle » et « posture existante » n'est pas tranchée par lui, et attend un statut en comité des risques.

**Registre propriétaire.** `registre-des-risques.md § 2.5 — Points à ratifier, dettes majeures et questions ouvertes à lacune réelle` pour le signalement de l'arbitrage attendu ; `§ 3.3 — Axe juridique`, entrée `J-12`, pour le détail.

---

### Groupe 6 — Conception d'interface (préalable hors jalon)

**Identité graphique — non arbitrable aujourd'hui, axe conception d'interface.** Logo, charte graphique/typo/grille et ressources visuelles constituent un préalable hors jalon porté par `docs/interface/**`. Registre propriétaire : `roadmap-de-build.md § Annexe B — Prérequis hors jalon`.

**Points ouverts du modèle de navigation.** Transitions non modélisées dans le graphe de navigation, granularité de deux arêtes à préciser, arête `Tableau → SuppressionRGPD` directe ou par composition, `NavDossiers`/`NavDossiersPerso` en écran distinct ou interaction in-page, libellés à harmoniser entre le graphe et les wireframes, second point de passage joueur → MJ — chacun marqué `[À TRANCHER — graphe S3]`. Registre propriétaire : `zonage.md § S10 — Points ouverts sur le modèle de navigation (graphe S3)`.

---

## Points déjà tranchés ailleurs

Un registre peut porter une entrée qu'une décision postérieure a déjà close ; recensé ici pour qu'aucune tâche ne s'ouvre sans objet.

- **`PO-06` — Nom de l'object store racine.** Déjà marquée **DÉCIDÉ 2026-09-03** dans `strategie-de-test.md § 12` : le store racine est nommé `spaces` (`ADR-017 § 1.1`, `ADR-018`).
- **`PO-04` — Réconciliation de nomenclature de jalon (« J1 »).** Déjà marquée **DÉCIDÉ 2026-09-01** dans `strategie-de-test.md § 12`, décision produit ratifiée, renvoi seul à `roadmap-de-build.md § 5`.
- **Contradiction entre `ADR-016 § 1.2` et `ADR-017 § 1.1` sur la persistance locale des entités de session.** Tranchée : `ADR-017 § 1.1 § Object stores` porte désormais le périmètre propre du store local, dérivé de `UC-01`, `UC-06` et `UC-07`, plus large que le payload de migration décrit par `ADR-016 § 1.2 § Périmètre sérialisé`.
- **`T-06` du registre des risques — « Résidu de nommage de l'espace de stockage local ».** Décrit un résidu qui n'existe plus : `ADR-017 § 1.1` nomme la racine `spaces`. DÉCIDÉ 2026-09-03 (commit `8a4bbec`, `ADR-017 § 1.1`) : la dénomination est confirmée cohérente, le risque est résolu. Son retrait de la matrice de criticité inhérente (`§ 2.1`) et du décompte des entrées à criticité résiduelle non abaissée (`§ 2.2`) a été fait le 2026-10-07 par le chantier conformite-corpus ; l'entrée reste au registre, marquée close ([`registre-des-risques.md § 3.1 — Axe technique`](registre-des-risques.md)).
- **Sous-grain MoSCoW des fiches `US-UC-*`.** Retiré le 2026-09-04 : les quatorze fiches `docs/besoin/stories/US-UC-*.md` ne portent plus aucune priorité Must/Should/Could/Won't Have au grain de la story — seul [`matrice-de-priorites.md`](../besoin/vision/perimetre/matrice-de-priorites.md) a l'autorité du corpus pour cette attribution, sur le précédent de `matrice-de-priorites.md § UC-15` (décision du 2026-09-01, qui posait déjà ce principe pour un cas voisin). La priorité se lit désormais exclusivement dans la matrice de priorités, quel qu'en soit le grain (`matrice-de-priorites.md`, que chaque fiche cite en renvoi).

---

## Registres propriétaires

- `docs/reference/cahier-specifications-techniques.md § 11` — points ouverts et confirmations attendues à l'entrée en build, groupés par section du cahier (contraintes techniques, modèle de données, sécurité, RGPD, contrats d'API, migration, observabilité, exigences non fonctionnelles).
- `docs/assurance/strategie-de-test.md § 12` — registre des points ouverts, identifiants `PO-nn`, avec propriétaire de décision par entrée.
- `docs/reference/conception-detaillee.md § 11` — points ouverts et limites de vérifiabilité de la conception détaillée.
- `docs/assurance/securite/dossier-securite.md § 5` — points ouverts et dette de sécurité.
- `docs/interface/zonage.md § S10` — points ouverts sur le modèle de navigation (graphe S3).
- `docs/projet/registre-des-risques.md` — registre d'évaluation des risques, entrées `T-nn` (technique), `P-nn` (produit), `J-nn` (juridique), `D-nn` (délai-coût).
- `docs/decisions/` — chaque décision d'architecture portant une section `## Points à trancher` y nomme sa propre dette technique, rattachée à son jalon ou à son code de renvoi.
- `docs/architecture/specs/telemetrie.md § 6` — récapitulatif des points à trancher de l'instrumentation de validation du MVP.
- `docs/architecture/specs/repli-temps-reel.md § 5` — récapitulatif des points à trancher du repli du transport temps réel.

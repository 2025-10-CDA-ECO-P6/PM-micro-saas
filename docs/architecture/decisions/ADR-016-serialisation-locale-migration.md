# ADR-016 — Sérialisation locale et contrat de migration local→cloud

- **Statut** : Accepté
- **Date** : 2026-06-10
- **Décideur** : opérateur (cadrage P1 — items B1.1a et B1.1b reclassés bloquants M1)
- **Findings liés** : F-01 (CWE-284 — reformulé §Contexte), F-09 (XSS/poste partagé — délégué B1.2 pour le détail)

> **Nature : mixte — à dominante pré-implémentation** — contrat de format et mécanique d'import à confirmer à l'entrée en build ; parts de conception qui contraignent le modèle dès maintenant : la frontière de confiance, le gate de reconnaissance, la transactionnalité par campagne et le périmètre de séance du contrat de migration. *(Annotation du 2026-06-10 — qualification postérieure à l'arbitrage T-03, audit conception pure 2026-06.)*

---

## Périmètre de cet ADR

Cet ADR formalise deux items bloquants du jalon M1 qui constituent ensemble le **contrat de données et le contrat d'échec de la frontière local↔cloud** :

- **B1.1a — Format de sérialisation** : structure du payload, champs gouvernés, enveloppe versionnée, couture de projection locale.
- **B1.1b — Parcours d'échec de revalidation** : stratégie de validation à l'import, rapport de rejets, idempotence, préservation des données locales.

Les fondations de ces deux items sont actées dans ADR-001 §Compléments (format = payload, invariant `validation locale ⊆ validation serveur`, payload non fiable revalidé par les VO) et dans 06-structure-projets.md §7. Cet ADR formalise et détaille ces fondations sans les re-décider.

**Dans le périmètre de cet ADR** : format du payload versionné, enveloppe et métadonnées, champs gouvernés vs libres, frontière de confiance, liste des champs jamais honorés depuis le payload, parcours d'échec transactionnel par campagne, ordre topologique à l'import, idempotence, gate de confirmation anti-appropriation.

**Hors périmètre, renvoyés explicitement** :
- Schéma IndexedDB détaillé, sync lot/delta, gestion de `navigator.storage.persist()` → **B1.2**
- Détail XSS/CSP du mode local (F-09) → **B1.2**
- Mécanisme complet de migration, implémentation du handler d'import → **P7**
- Tests e2e navigateur multi-versions IndexedDB → **P7**

---

## Contexte

ADR-001 §Compléments a acté le cadre structurant : le mode local est une couche de persistance CRUD avec validations TypeScript minimales ; le domaine C# serveur est la source de vérité ; la migration est un import revalidé, pas une copie de confiance. Il a identifié le format de sérialisation et le parcours d'échec de revalidation comme préalables bloquants, sans les spécifier.

Trois points restaient ouverts au terme d'ADR-001 :

1. **Format** : la structure concrète du payload (enveloppe, versionnement, périmètre des entités sérialisées, séparation des champs gouvernés et libres) n'était pas définie.

2. **Frontière de confiance** : le finding F-01 de l'audit avait identifié le risque sous le libellé « migration silencieuse sans preuve d'appartenance (CWE-284) ». Ce cadrage est **partiellement inexact**. Le mode local n'a pas de compte : l'utilisateur qui migre ne peut pas prouver que les données lui « appartiennent » au sens de la possession — elles n'ont pas de propriétaire avant la migration. Ce que le finding signale en réalité est un risque d'**appropriation sur poste partagé** : si la migration est silencieuse, un second utilisateur du même poste peut importer sous son compte les données créées par quelqu'un d'autre. La réponse correcte n'est pas une preuve d'appartenance (impossible sans compte préalable) mais un **gate de confirmation** présentant les données détectées. Ce recadrage est acté dans cet ADR (§4 ci-dessous) ; le libellé CWE-284 est conservé comme référence d'audit mais son interprétation est précisée.

3. **Parcours d'échec** : la stratégie de granularité (tout-ou-rien par payload, par campagne, par document), le rapport de rejets, et la préservation des données locales n'avaient pas été spécifiés.

---

## Décision

### 1. Format de sérialisation (B1.1a)

#### 1.1 Enveloppe du payload

Le payload de migration est un document JSON avec une enveloppe obligatoire en tête :

```
{
  "schemaVersion": <entier, obligatoire>,
  "exportedAt": "<ISO 8601, date d'export, obligatoire>",
  "appVersion": "<string, version de l'application au moment de l'export, obligatoire>",
  "campaigns": [ ... ]
}
```

`schemaVersion` est un entier incrémental qui identifie la version du **contrat de format**, indépendant de la version de l'application. Sa présence est obligatoire ; un payload sans `schemaVersion` est rejeté comme malformé. `exportedAt` et `appVersion` sont des métadonnées d'origine destinées au diagnostic — elles ne sont pas utilisées comme critères de décision à l'import.

#### 1.2 Périmètre sérialisé

L'agrégat `Document` et ses dépendances directes forment le périmètre sérialisé :

- `campaigns` (racine, une ou plusieurs)
- `folders` (arborescence rattachée à la campagne)
- `documents` (avec `title`, `slug`, `visibility`, `properties`, `document_type` — voir §1.3)
- `document_blocks` (contenu structuré)
- `document_links` (références inter-documents)
- `document_tags` (tags normalisés)
- `document_types` custom uniquement (types créés par l'utilisateur dans la campagne ; les types système sont seedés côté serveur et non transportés)
- `sessions` (sessions terminées uniquement — statut, titre, résumé, dates)
- `session_pinned_documents` (références d'épinglage document↔session)
- `session_live_notes` (références note↔session ; les notes de session elles-mêmes sont des documents de type `LIVE_NOTE`, déjà couverts par l'entrée `documents` ci-dessus)

**Exclusions explicites** :
- Sessions en cours (`status = LIVE`) : une session active ne peut pas être migrée ; elle doit être clôturée avant export. **Correction — Annotation du 2026-06-10** : l'exclusion antérieure reposait sur la prémisse « le mode local UC-01 ne comprend pas de session — ces entités n'existent pas dans le store local ». Cette prémisse est factuellement fausse : UC-06 et UC-07 spécifient que la vue session du MJ est disponible en mode local sans compte, et que des sessions terminées, notes de session, épinglages et résumés y persistent. L'exclusion est inversée le 2026-06-10 — l'historique de session fait partie du périmètre migré avec les données de campagne. Restent explicitement exclues : (i) les sessions en statut `LIVE` (elles ne sont pas migrables en l'état ; voir §3), (ii) `session_view_configs` (préférence d'affichage, recréée à l'import comme à la création de campagne), (iii) `session_view_folders` (contenu de la configuration de vue — même traitement).
- `users` et memberships (`campaign_memberships`, `membership_characters`) : absents du mode local sans compte.
- Tout flag de cycle de vie serveur (`deleted_at` sur campagnes, `purge_claimed_at`, `is_deleted` sur documents) : ces états sont gouvernés par le serveur et ne doivent pas être transportés.

#### 1.3 Champs gouvernés et champs libres

Deux catégories de champs se distinguent selon la nature de leur revalidation à l'import.

**Champs gouvernés** — revalidés par les Value Objects du domaine à l'import :
- `document.properties` : revalidé contre le `propertiesSchema` du type de document concerné (ADR-002 — même chemin que `Document.SetProperties()`).
- `document.visibility` : revalidé contre l'énumération `PUBLIC | GM_ONLY | PLAYER_PRIVATE`.
- `document.slug` : revalidé (unicité par campagne et type, format) ; en cas de collision de slug, l'item est rejeté avec la raison correspondante dans le rapport (§3).
- `document_type.slug` (pour les types custom) : résolu et validé comme appartenant à la campagne créée à l'import (voir §2 — frontière de confiance).
- `session.status` : revalidé contre l'énumération `CLOSED | ARCHIVED` ; une session en statut `LIVE` dans le payload entraîne le rejet de la campagne concernée (raison `LIVE_SESSION_IN_PAYLOAD` dans le rapport, cohérent avec §3). Le titre et le résumé de session sont traités comme contenu libre, sanitisés comme le contenu des blocs (§2.4).

**Champs libres** — transportés sans revalidation sémantique :
- `document_block.content` : contenu éditeur, sans enjeu d'intégrité référentielle (ADR-002 §Compléments). Le contenu est néanmoins sanitisé côté serveur avant persistance (§2 — sanitisation HTML anti-XSS).

#### 1.4 Format comme contrat versionné stable

Au MVP, le payload est une sérialisation directe du store IndexedDB local — l'esprit « un seul format, deux usages » acté dans ADR-001 est préservé. Cependant, le format est nommé ici comme **contrat versionné stable** et matérialisé par une **fonction de projection** `store local → payload`.

Cette couture de projection est délibérée : si le schéma IndexedDB évolue (B1.2), la projection absorbe la différence sans modifier le contrat de payload. Le payload reste stable pour le serveur même quand le store local change. Sans cette couture, toute évolution du store IndexedDB imposerait une migration côté serveur.

**Politique de version** : le serveur rejette proprement un payload dont la `schemaVersion` est inconnue ou antérieure au seuil de compatibilité maintenu. Ce rejet est traité comme un cas d'échec de revalidation (§3 — rapport de rejets, raison `UNSUPPORTED_SCHEMA_VERSION`). La migration ascendante du payload côté serveur (compatibilité avec les versions antérieures de `schemaVersion`) est une **dette nommée, reportée post-MVP**.

---

### 2. Frontière de confiance (reformulation de F-01)

#### 2.1 Cadrage : assignation, non preuve

Le mode local n'a pas de compte. La migration n'est pas une « preuve d'appartenance » — c'est une **assignation de propriété** : l'utilisateur qui migre devient propriétaire (`owner_id`) des données importées au moment de l'import (ADR-014 — `owner_id` naît à l'import). Il n'y a rien à prouver, parce qu'il n'y avait pas de propriétaire avant.

Le risque réel identifié par F-01 est différent : sur un poste partagé, un second utilisateur peut lancer le flux de migration et importer sous son compte les données créées par quelqu'un d'autre. La réponse est un **gate de confirmation** (§4), pas un mécanisme d'authentification de l'origine des données.

En revanche, le payload traverse une **frontière de confiance** réelle : il est produit par le navigateur, potentiellement modifié, sans signature. Le serveur ne fait confiance à **aucun champ d'autorité** contenu dans le payload.

#### 2.2 Champs jamais honorés depuis le payload

Les champs suivants sont **toujours réémis ou réassignés côté serveur**, jamais lus depuis le payload :

| Champ | Traitement côté serveur |
|---|---|
| `document_id`, `campaign_id`, `folder_id`, `document_block_id`, `document_link_id`, `document_tag_id`, `session_id`, `session_pinned_document_id`, `session_live_note_id` | Nouveaux UUID générés à l'import ; les ID locaux servent uniquement à résoudre les références internes au payload (ex. `folder_id` d'un document → ID du dossier créé dans la même transaction ; `session_id` des épinglages et notes rattachées → ID de session créé dans la même transaction) |
| `owner_id` | Assigné au compte authentifié qui déclenche la migration (ADR-014) |
| `created_by_id` | Assigné au compte authentifié |
| `created_at`, `updated_at` | Horodatages serveur au moment de l'import |
| `deleted_at`, `purge_claimed_at`, `is_deleted` | Non transportés — voir §1.2 exclusions |

`document_type_id` n'est pas transporté comme ID serveur : les types custom sont résolus par `slug` à l'import. La résolution vérifie que le slug appartient à un type créé dans la campagne importée (ou à un type système). Un `slug` de type custom inconnu est rejeté dans le rapport (raison `UNKNOWN_DOCUMENT_TYPE_SLUG`).

**Règle de précédence des slugs de type** : les slugs de `document_types` **système** sont réservés. Un type custom dont le slug entre en collision avec un slug système est rejeté à l'import (raison `DOCUMENT_TYPE_SLUG_RESERVED`) — le payload n'est jamais l'autorité sur l'espace de noms des types système (§2 — frontière de confiance). L'unicité des slugs de types custom au sein du payload (par campagne) est elle-même un contrôle de champ gouverné, appliqué lors du dry-run de pré-validation (§3).

#### 2.3 Revalidation via les Value Objects du domaine

Le serveur revalide tous les champs gouvernés (§1.3) en utilisant le **chemin d'écriture normal du domaine** — les mêmes Value Objects qui valident une écriture API ordinaire (ADR-002). Il n'y a pas de second validateur spécifique à l'import : l'import emprunte le chemin de création de campagne et de document existant, avec en plus la résolution des références internes du payload.

Ce principe garantit que toute règle métier introduite côté serveur s'applique automatiquement à l'import, sans modification du code d'import.

#### 2.4 Sanitisation HTML du contenu

Le contenu des `DocumentBlock` (`content`) et le résumé de session (si présent) sont sanitisés côté serveur avant persistance, afin de neutraliser les vecteurs Stored XSS (CWE-79). Cette sanitisation est réalisée à l'import au même titre qu'elle l'est sur les écritures API ordinaires.

**Plancher minimal opposable** : la sanitisation est réalisée par **liste blanche positive** — seules les balises explicitement autorisées sont conservées. Les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont **interdits et supprimés sans exception**. Ce plancher constitue un critère d'acceptation non négociable, indépendamment de la bibliothèque ou de la liste exacte de balises choisie. Le détail de la politique de sanitisation (liste complète des balises autorisées, bibliothèque côté serveur) et la politique CSP du mode local sont délégués à **B1.2** (F-09).

---

### 3. Parcours d'échec de revalidation (B1.1b)

#### 3.1 Stratégie transactionnelle par campagne

La granularité de la transaction d'import est **la campagne**. Le serveur ne fait pas de tout-ou-rien sur le payload entier (toutes les campagnes ou aucune), ni document par document. La campagne est l'unité cohérente : une campagne et toutes ses entités dépendantes — documents, sessions, références d'épinglage et notes rattachées — sont importées en transaction ou rejetées en bloc.

**Séquence** :

1. Le serveur effectue un **dry-run de pré-validation** sur chaque campagne du payload : il applique les Value Objects sur tous les champs gouvernés (incluant `session.status`) et collecte les rejets sans écrire en base.
2. Il produit un **rapport de rejets** indiquant, pour chaque campagne invalide, les raisons du rejet (ex. `SLUG_COLLISION`, `INVALID_PROPERTIES`, `UNKNOWN_DOCUMENT_TYPE_SLUG`, `UNSUPPORTED_SCHEMA_VERSION`, `LIVE_SESSION_IN_PAYLOAD`).
3. Les campagnes valides sont importées dans des transactions séparées. Les campagnes invalides ne sont pas importées.
4. Les données locales correspondant aux campagnes rejetées restent **intactes dans le navigateur** — conformément à l'acquis E5 d'UC-10 : aucune donnée locale n'est supprimée avant confirmation explicite de la migration côté serveur.

#### 3.2 Ordre d'import topologique

L'import respecte un ordre topologique qui reflète les dépendances internes au payload :

- Les `folders` sont créés avant les `documents` (un document référence son dossier par son ID local, résolu en ID serveur créé à l'étape précédente).
- Les `document_types` custom sont créés avant les `documents` qui les référencent.
- Les `sessions` sont créées après tous les `documents` de la campagne.
- Les `session_pinned_documents` et `session_live_notes` sont créées après la création des sessions et des documents/notes qu'elles référencent (résolution des références internes).
- Les `document_links` sont créés après tous les documents (source et cible doivent exister).

En cas de **références orphelines** dans le payload — un document référençant un dossier parent dont la campagne a été rejetée, ou un document référençant une cible de lien absente du payload — le comportement est le suivant : l'entité orpheline est rejetée de manière cohérente avec la sous-arborescence concernée, et le rejet est consigné dans le rapport.

Les **arêtes de cycle nullable** dans le payload (ex. `folders.default_template_document_id`) sont traitées en deux passes à l'import, par analogie avec la passe 1 de la saga `CampaignDeleted` d'ADR-011 : les références cycliques sont d'abord résolues à NULL, puis rétablies après la création des entités cibles.

#### 3.3 Idempotence

Chaque opération de migration est associée à un **`migration_batch_id`** (UUID v4 généré côté client au déclenchement de la migration). Le serveur **valide le format UUID v4** du `migration_batch_id` et **rejette la requête si le format est invalide** (raison `INVALID_MIGRATION_BATCH_ID`). Le serveur utilise cet identifiant pour garantir l'idempotence : une campagne déjà importée dans le cadre d'un même `migration_batch_id` n'est pas importée une seconde fois. L'idempotence est **scopée par utilisateur authentifié** — deux utilisateurs différents peuvent soumettre le même `migration_batch_id` sans collision. Ce mécanisme couvre le cas E5 de UC-10 (migration en attente, reprise sur réseau instable) sans créer de doublons.

> **Dette — horizon de rétention du registre `migration_batch_id`** : la durée de conservation des `migration_batch_id` en base (pour garantir l'idempotence) n'est pas fixée. À trancher en **P7** lors de l'implémentation du handler d'import.

---

### 4. Gate de confirmation anti-appropriation

Lorsqu'une migration est déclenchée sur un compte pour lequel le navigateur contient des données locales, le serveur ou le client **présente à l'utilisateur les données détectées** (titres des campagnes locales, volume estimé, date d'export), y compris l'historique de session de chaque campagne (sessions terminées, notes de session, documents épinglés, résumés), et requiert une **confirmation explicite** avant de lancer l'import.

Cette confirmation joue le rôle de **reconnaissance des données** : l'utilisateur voit ce qui va être importé — incluant l'historique de session — et peut identifier si ces données lui appartiennent ou correspondent à des données d'un autre utilisateur du même poste. Sur un poste partagé, ce gate borne l'appropriation silencieuse : un utilisateur qui déclenche par inadvertance le flux de migration voit les données détectées et peut interrompre avant import. Le gate signale aussi toute session en cours (statut `LIVE`) qui devra être clôturée avant migration — elle n'est pas importée, mais l'utilisateur en est informé.

**Le gate est un check applicatif côté serveur, pas seulement une barrière UX client** : la requête d'import porte un **drapeau de consentement explicite** (ex. champ `confirmed: true`). Le serveur **rejette l'import si ce drapeau est absent**, indépendamment de ce que le client a présenté. Le gate n'est donc pas contournable par un appel direct à l'API d'import sans ce drapeau.

**Ce que ce gate garantit et ce qu'il ne garantit pas** : le gate empêche une appropriation silencieuse — il ne constitue pas une preuve d'appartenance (§2.1). Un utilisateur peut confirmer l'import de données qui ne lui appartiennent pas s'il est assis devant le poste. Le gate borne le risque d'accident ou d'inattention, pas l'acte délibéré.

**Réalignement de RB-10-04** : ~~la règle RB-10-04 dans US-UC-10 stipule actuellement « la migration est silencieuse, aucune confirmation n'est demandée ». Cette règle est **reformulée par la présente décision** : « silencieuse » signifiait « sans cérémonie superflue », mais le gate de confirmation des données détectées est un point de reconnaissance nécessaire, distinct d'une cérémonie. La formulation de RB-10-04 devra être réalignée en cohérence dans une passe séparée.~~ — **Réalignement effectué** : US-UC-10 RB-10-04 est conforme à la présente décision (gate de reconnaissance anti-appropriation, confirmation explicite requise avant migration).

---

## Alternatives considérées

**Tout-ou-rien sur le payload entier**
Rejeter l'ensemble du payload si une seule campagne est invalide. Écarté : une erreur de slug dans une campagne ne doit pas bloquer l'import de campagnes entièrement valides. La granularité par campagne offre la récupérabilité maximale sans sacrifier la cohérence.

**Document par document**
Importer les documents valides et rejeter les documents invalides au sein d'une même campagne. Écarté : la cohérence d'une campagne dépend de l'intégralité de ses entités (dossiers, types, documents, liens). Un import partiel produirait une campagne incohérente côté serveur (document sans dossier, lien orphelin). La campagne est la bonne unité transactionnelle.

**Aucune enveloppe `schemaVersion`**
Inférer la version du payload depuis son contenu (duck typing). Écarté : sans versionnement explicite, toute évolution du format oblige le serveur à deviner la version — logique fragile et non testable. La `schemaVersion` explicite garantit un rejet propre et un message d'erreur intelligible.

**Conserver le libellé F-01 « preuve d'appartenance »**
Implémenter un mécanisme de preuve (ex. signature HMAC, challenge-réponse local) pour démontrer que les données ont été créées sur ce navigateur. Écarté : le mode local n'a pas de secret partagé avec le serveur ; toute signature côté client est forgeable par définition. Le gate de confirmation (§4) est la réponse proportionnée au risque réel.

---

## Conséquences

### Invariant `validation locale ⊆ validation serveur` — discipline, non garantie outillée

ADR-001 §Compléments pose l'invariant : le mode local peut être plus permissif que le serveur, mais ne doit pas accepter en écriture ce que le serveur rejette. Cet invariant est une **discipline de conception dirigée**, pas une propriété vérifiée par outillage.

Le mode local est implémenté en TypeScript, le domaine serveur en C# : aucun test cross-langage automatique ne vérifie la cohérence des règles de validation entre les deux. Le filet de sécurité est la **revalidation systématique par les VO serveur à l'import** : même si le local a accepté un état que le serveur refuse, la migration le détecte et le consigne dans le rapport de rejets. Présenter cet invariant comme une propriété prouvée serait inexact.

### Maillon NON VÉRIFIABLE IN BUILD

**Le contrat de format est testable** : les tests d'import peuvent porter sur des payloads synthétiques, valider la revalidation par les VO, vérifier le rapport de rejets sur des cas invalides, et tester l'idempotence par `migration_batch_id` (tests unitaires et intégration côté serveur, CI).

**La migration réelle depuis un IndexedDB multi-versions en navigateur n'est pas prouvable en CI** : le comportement de la fonction de projection `store local → payload` sur différentes versions de l'IndexedDB, dans des navigateurs réels, avec des données persistées par des versions antérieures de l'application, ne peut pas être couvert par les tests d'archi CI. Cette vérification relève de tests e2e navigateur renvoyés à **P7**.

Ce maillon non vérifiable en build est nommé explicitement pour éviter une fausse confiance dans la couverture CI.

### Dettes nommées (non silencieuses)

| Dette | Nature | Ticket |
|---|---|---|
| Migration ascendante du payload (compatibilité `schemaVersion` antérieures) | Évolutivité — permet de migrer des exports anciens après évolution du format | post-MVP |
| Seuil de `schemaVersion` minimale maintenue | Configuration — à fixer à l'implémentation | P7 |
| Détail de la politique de sanitisation HTML / liste des balises autorisées | Sécurité — complète F-09 | B1.2 |
| ~~Réalignement de RB-10-04 dans US-UC-10~~ | ~~Cohérence documentaire~~ — **Résolu** : US-UC-10 RB-10-04 conforme (gate de reconnaissance acté) | ✓ |

### Conformité conçue, non certifiée

La sanitisation HTML du contenu (§2.4) ferme le vecteur Stored XSS au sens applicatif. La bibliothèque de sanitisation et la configuration exacte sont des points d'implémentation renvoyés à B1.2 ; leur adéquation devra être vérifiée à cette occasion.

Le gate de confirmation (§4) borne le risque d'appropriation sur poste partagé. Il ne constitue pas une preuve cryptographique d'appartenance des données — ce qui est cohérent avec le modèle sans compte du mode local (§2.1).

---

## Points à trancher

- **B1.2** — Schéma IndexedDB détaillé (object stores, indexes, migrations de version), politique de sanitisation HTML (F-09), politique CSP mode local. Ce jalon précise aussi l'implémentation de la fonction de projection `store local → payload`.
- **P7** — Implémentation complète du handler d'import côté serveur : résolution des références internes du payload, création topologique des entités, rapport de rejets structuré, `migration_batch_id` idempotent. Tests e2e navigateur multi-versions IndexedDB.
- **Seuil `schemaVersion` minimale** — à fixer à l'implémentation P7 en fonction des versions du client déployées.
- ~~**Réalignement RB-10-04**~~ — **Effectué** : US-UC-10 RB-10-04 est conforme à la décision §4 (gate de confirmation, non silencieux).
- ~~**F-01 fermé comme finding d'audit**~~ — le cadrage est reformulé (§Contexte + §2), le gate de confirmation (§4) est acté. Le libellé original « preuve d'appartenance » est archivé avec la note de reformulation ci-dessus.

---

## Croisements

| ADR / Document | Nature du croisement |
|---|---|
| **ADR-001** | Fondation — format = payload, invariant `validation locale ⊆ validation serveur`, payload non fiable revalidé par les VO. Préalables bloquants actés dans §Compléments, formalisés ici. |
| **ADR-002** | Modèle Document — `DocumentProperties` et `propertiesSchema` sont le mécanisme de revalidation des champs gouvernés à l'import ; `DocumentBlock.content` est libre (décision ADR-002 §Compléments). |
| **ADR-011** | Ordre topologique — la passe 1 de déliaison des arêtes de cycle de la saga `CampaignDeleted` est le miroir de la résolution des cycles à l'import (§3.2). |
| **ADR-013** | Données invité — si une campagne rejetée contient des données marquées comme appartenant à un tiers (F-13 / données invité), le rejet de la campagne protège ces données ; renvoi léger vers ADR-013 pour la politique de traitement de ces cas. |
| **ADR-014** | Assignation de propriété — `owner_id` et `created_by_id` naissent à l'import assignés au compte authentifié. La frontière de confiance (§2) est cohérente avec le modèle d'autorisation d'ADR-014. |

---

## Compléments post-revue

*(Section réservée aux clarifications post-implémentation — vide à la date de l'ADR.)*

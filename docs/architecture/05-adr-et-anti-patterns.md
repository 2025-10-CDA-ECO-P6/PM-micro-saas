# 05 — ADR et anti-patterns

> Les décisions d'architecture importantes, pourquoi elles ont été prises,
> et les pièges à éviter.

---

## Architecture Decision Records

Un ADR (Architecture Decision Record) documente une décision importante,
son contexte et les alternatives écartées. Il permet de comprendre "pourquoi"
sans devoir reconstituer l'historique des discussions.

---

### ADR-01 — DDD comme approche de conception

**Contexte** : le domaine est riche, évolutif et multi-utilisateurs.
Il doit supporter différents systèmes de jeu et évoluer sans réécriture majeure.

**Décision** : utiliser DDD avec des Bounded Contexts, Agrégats, Value Objects
et Domain Events.

**Alternatives écartées** :
- Architecture CRUD classique : les règles métier finissent dispersées dans
  les contrôleurs et les services — impossible à maintenir sur un domaine riche.
- Architecture anémique (entités sans comportement) : même problème,
  logique dispersée dans des services sans cohérence.

---

### ADR-02 — Shared Kernel pour les primitives

**Contexte** : plusieurs contextes partagent les mêmes primitives.
Deux approches possibles : duplication ou partage.

**Décision** : Shared Kernel contenant uniquement Id typés, value objects
fondations et abstractions d'infrastructure.

**Alternatives écartées** :
- Duplication dans chaque contexte : incohérences inévitables à terme.
- Un contexte "Common" avec des entités métier : crée du couplage entre contextes.

**Règle** : le Shared Kernel ne contient jamais de règles métier.
Dès qu'un concept a un comportement métier, il appartient à un contexte précis.

---

### ADR-03 — Modèle Document modulaire

**Contexte** : Haversack doit supporter plusieurs systèmes de jeu avec des structures
de fiches radicalement différentes, sans schéma fixe.

**Décision** : modèle `Document` avec des `DocumentBlock` typés et une `BlockValue`
polymorphe. Inspiré du modèle de blocs de Notion.

**Alternatives écartées** :
- Colonnes fixes par type de document : impossible à étendre sans migration.
- Table `key_value` générique : flexible mais non typé, validation impossible.
- Héritage de tables (TPH/TPT) : combinatoire explosive avec les systèmes de jeu.

**Conséquences** : ajouter un nouveau type de bloc ou supporter un nouveau système
de jeu ne nécessite aucune migration de schéma. Le trade-off est une complexité
de requête légèrement plus élevée pour les champs structurés.

---

### ADR-04 — NPC et PlayerCharacter comme profils spécialisés de Document

**Contexte** : un PNJ ou un personnage joueur peut aller d'une simple description
à une fiche complète selon le système de jeu. Le contenu doit rester flexible,
mais certaines métadonnées doivent être requêtables efficacement.

**Décision** : `NPC` et `PlayerCharacter` sont des profils spécialisés d'un `Document`
typé. Leur contenu vit dans `Document`; leurs tables dédiées portent les métadonnées
transverses comme statut, nom dénormalisé et associations narratives.

**Alternatives écartées** :
- Entités enfants de `Campaign` : l'agrégat `Campaign` deviendrait ingérable,
  chargé de centaines d'entités à chaque accès.
- Fiches rigides par système de jeu : migrations et combinatoire trop coûteuses.
- Tout mettre dans `Document` sans profil : listes, statuts et associations deviendraient
  coûteux et trop implicites.

---

### ADR-05 — GuestAccess séparé de User

**Contexte** : l'application supporte des joueurs invités sans compte.
Option A : un `User` avec `role = GUEST`. Option B : une entité `GuestAccess` distincte.

**Décision** : `GuestAccess` dans Campaign Management.
`UserRole` ne contient que `GM` et `PLAYER`.

**Alternatives écartées** :
- `User` avec `role = GUEST` : un User représente une identité persistante
  avec email et authentification. Un invité n'a ni l'un ni l'autre.
  Unifier les deux crée un concept incohérent avec des champs
  conditionnellement valides (`passwordHash` null, `email` null...).

---

### ADR-06 — AccessPolicy comme service domaine

**Contexte** : la visibilité des contenus est transverse à plusieurs contextes.
Elle concerne les documents, mais aussi les LiveNotes et les résumés de session.

**Décision** : service domaine `AccessPolicy` dans Campaign Management,
persistant des entités légères `ContentAccessRule` immuables. Une règle cible
une `ShareableResourceRef` (`DOCUMENT`, `LIVE_NOTE`, `SESSION_SUMMARY`) plutôt
qu'un `DocumentId` seul.

**Alternatives écartées** :
- Agrégat `ShareGrant` : pas d'entités enfants, pas d'invariants transactionnels.
  Un agrégat racine serait surdimensionné.
- Champ `visibility` uniquement sur `Document` : insuffisant pour cibler
  un membre ou un personnage spécifique.
- Policies séparées par type de ressource : duplication et risque de divergence.
- Contexte autonome `AccessControl` : surcomplexité pour le MVP,
  extractible plus tard si la complexité le justifie.

**Note** : `ContentAccessRule` est immuable — créée ou révoquée (suppression
physique), jamais modifiée. Ce pattern reflète la nature binaire d'une autorisation.

---

### ADR-07 — Séparation entités domaine / modèles infrastructure

**Contexte** : stack .NET avec EF Core et ASP.NET Identity.

**Décision** : entités domaine pures dans la couche Domain,
modèles de persistance séparés dans Infrastructure, mappeurs bidirectionnels.

**Alternatives écartées** :
- Entités domaine héritant de `IdentityUser` ou portant des attributs EF :
  couplage fort au framework, tests unitaires impossibles sans base de données.
- Approche "Database First" : le schéma dicte le modèle, pas le domaine.

---

### ADR-08 — BlockValue comme hiérarchie de types scellés

**Contexte** : `DocumentBlock.value` est polymorphe selon le `kind`.

**Décision** : hiérarchie de value objects scellés, un sous-type par `BlockKind`.

**Alternatives écartées** :
- Objet unique avec tous les champs optionnels : états invalides non détectables,
  bugs silencieux en production.
- Sérialisation JSON brute sans type : perd la validation à la compilation.

---

### ADR-09 — DocumentTemplate comme snapshot

**Contexte** : comportement des templates à la création d'un document.

**Décision** : snapshot — un document créé depuis un template est une copie
indépendante. Modifier le template ne modifie pas les documents existants.

**Alternatives écartées** :
- Héritage dynamique : imprévisible pour le MJ. Une modification de template
  ne doit pas altérer rétrospectivement des fiches déjà remplies.

---

### ADR-10 — Champs dénormalisés synchronisés par domain events

**Contexte** : `NPC.name`, `Scenario.title` sont dénormalisés depuis `Document.title`
pour les requêtes de liste efficaces.

**Décision** : synchronisation via le domain event `DocumentTitleUpdated`.
Un handler met à jour les champs dénormalisés quand le titre change.

**Alternatives écartées** :
- Jointure systématique avec `Document` : coût en performance sur les listes.
- Synchronisation dans la couche Application : logique dispersée, risque d'oubli.
- Pas de dénormalisation : performance insuffisante pour les listes de campagne.

**Conséquences** : risque d'incohérence limité à la fenêtre entre l'émission
de l'event et son traitement — acceptable pour ce domaine.

---

### ADR-11 — Monolithe modulaire comme cible d'architecture

**Contexte** : choix de la topologie physique de déploiement.
Options : microservices, monolithe classique, monolithe modulaire.

**Décision** : monolithe modulaire. Un seul processus, un seul dépôt, mais une isolation stricte du code entre les bounded contexts (pas d'import d'entités domaine d'un contexte vers un autre).

**Alternatives écartées** :
- Microservices d'emblée : complexité opérationnelle (déploiement distribué, réseau, cohérence éventuelle) injustifiée à ce stade sans validation produit.
- Monolithe classique (sans isolation) : conduit à un Big Ball of Mud — les frontières de contexte disparaissent sous la pression du développement.

**Conséquences** : les bounded contexts peuvent être extraits en services indépendants si la charge ou les équipes le justifient. L'isolation au niveau code est une précondition à cette extraction.

---

### ADR-12 — FK réelle CAMPAIGN.ownerId → USER (exception documentée)

**Contexte** : la règle d'isolation stricte des bounded contexts interdit les FK en base entre contextes. `CAMPAIGN.ownerId` référence `USER.id` — deux contextes différents.

**Décision** : exception documentée. Une FK réelle est posée sur `CAMPAIGN.ownerId → USER.id` dans le monolithe modulaire. L'isolation reste au niveau du code, pas de la base de données.

**Alternatives écartées** :
- Entité de liaison dans le Shared Kernel (Core) : **rejetée**. Le Shared Kernel ne contient jamais d'entités. Toute entité appartient à un contexte précis avec ses règles métier.
- Référence cross-context sans FK (comme les autres) : acceptable mais perd l'intégrité référentielle pour le lien le plus critique du système.

**Plan de migration** : si `Campaign Management` est extrait en service, `CAMPAIGN.ownerId` devient une référence cross-context et une `UserProjection` locale est introduite, maintenue par événements.

---

### ADR-13 — Invitation par email via interface applicative

**Contexte** : `InvitationType.EMAIL` est dans le périmètre MVP. L'envoi d'email dépend d'un provider externe (SendGrid, SMTP, etc.) non encore choisi.

**Décision** : interface `IEmailInvitationSender` dans la couche Application. L'implémentation concrète (Infrastructure) est découplée du domaine. Le provider peut être branché ou remplacé sans toucher au domaine.

**Alternatives écartées** :
- Appel direct au provider dans un handler Application : couplage fort à un SDK tiers dans la couche Application.
- Différer EMAIL après le MVP : accepté initialement, mais le besoin est avéré — l'interface prépare l'implémentation future sans complexifier le domaine.

---

### ADR-14 — JSONB pour DocumentBlock.value

**Contexte** : `DocumentBlock.value` est polymorphe selon le `kind`. Sa structure varie selon le type de bloc.

**Décision** : colonne `value JSONB` dans `DOCUMENT_BLOCK`. La structure attendue est documentée par `kind` et validée par les value objects domaine lors de la désérialisation.

**Alternatives écartées** :
- Colonne par type de valeur : explosion combinatoire, migrations à chaque nouveau `BlockKind`.
- Table séparée par `BlockKind` (TPT) : jointures coûteuses pour reconstituer un document, complexité d'ORM élevée.
- `value TEXT` sérialisé : perd la capacité d'indexation et de requêtes sur le contenu.

**Conséquences** : un index GIN sur `DOCUMENT_BLOCK.value` permet des requêtes dans le JSON (recherche FTS). La validation de la structure est applicative, pas en base.

**Convention de migration** : toute modification de la structure JSON d'un `BlockKind` doit être rétrocompatible ou accompagnée d'une migration de données JSONB.

---

### ADR-15 — PostgreSQL FTS pour la recherche

**Contexte** : le MJ doit pouvoir retrouver rapidement toute information de sa campagne (UC-11). La recherche doit couvrir les titres et contenus des documents.

**Décision** : PostgreSQL Full-Text Search (FTS) avec index `GIN` sur les colonnes `DOCUMENT.title` et `DOCUMENT_BLOCK.value`. Pas de moteur de recherche externe dans le MVP.

**Alternatives écartées** :
- Elasticsearch / OpenSearch : surcharge opérationnelle (service séparé, synchronisation, coûts) injustifiée pour un MVP avec une seule instance PostgreSQL.
- Recherche `ILIKE '%term%'` : pas d'index, dégradation de performance linéaire avec le volume.

**Limites acceptées** : pas de recherche phonétique, pas de suggestion, pas d'analyse linguistique avancée. Suffisant pour le MVP. Extractible vers un moteur dédié si le volume ou les besoins l'exigent.

---

### ADR-16 — PLAYER_PRIVATE : notes joueur invisibles au MJ

**Contexte** : question ouverte sur la visibilité des notes personnelles des joueurs. Est-ce que le MJ peut voir les notes marquées comme privées par un joueur ?

**Décision** : `Visibility.PLAYER_PRIVATE` — une note créée avec cette visibilité
n'est accessible qu'au demandeur associé au `ownerCharacterId` de la ressource.
Le MJ est explicitement exclu, même s'il est propriétaire de la campagne.

**Rationale** : respecte l'intention du joueur. Un joueur doit pouvoir noter des théories, mémos ou informations personnelles sans risquer que le MJ les lise accidentellement. Cela renforce la confiance dans l'outil côté joueurs.

**Alternatives écartées** :
- MJ voit tout par défaut : casse la promesse de confidentialité pour les joueurs, frein à l'adoption.
- Option de configuration par campagne : surcharge inutile — la règle est invariante.

**Implémentation** : la règle est appliquée dans `AccessPolicy` (domaine), pas seulement
dans les handlers Application. Un MJ qui charge les ressources d'une campagne ne voit
jamais les `PLAYER_PRIVATE` d'un personnage joueur.

**Conséquence GuestAccess** : un joueur sans compte peut récupérer ses notes privées
lors d'une séance suivante si le MJ lui redonne un `GuestAccess` actif associé au même
`CharacterId`. La persistance est portée par le personnage, pas par le guest temporaire.

---

### ADR-17 — Dossiers comme structure libre gérée par l'utilisateur

**Contexte** : le contenu d'une campagne (PNJ, scénarios, notes, etc.) doit être organisé. Deux approches : des catégories fixes imposées par l'application (DocumentType seul), ou une structure de dossiers gérée par le MJ.

**Décision** : agrégat `Folder` dans Content Library. Le MJ crée, renomme et réordonne ses dossiers librement. Quatre dossiers système (`PNJ`, `Personnages joueurs`, `Scénarios`, `Notes`) sont créés automatiquement à l'initialisation de la campagne — ils sont renommables mais non supprimables (`isSystem = true`).

**Alternatives écartées** :
- Catégories fixes uniquement (DocumentType) : ferme la porte à la personnalisation. Un MJ qui veut un dossier "Lieux", "Factions" ou "Secrets" ne peut pas le créer.
- Dossiers imbriqués (arborescence) : complexité de navigation et d'implémentation disproportionnée pour le MVP. Reporté à une itération post-MVP.

**Relation avec DocumentType** : les deux coexistent. `DocumentType` qualifie la nature métier d'un document (NPC, CHARACTER, SCENARIO…). Le `Folder` est la couche d'organisation UX. Un NPC peut être dans n'importe quel dossier — le type reste NPC.

**Conséquences** : `Document.folderId` est nullable — un document sans dossier est simplement "non classé". `FolderOrderService` gère l'ordre des dossiers dans la navigation, sur le même modèle que `ScenarioOrderService`.

---

### ADR-18 — Backlinks via index GIN sur DOCUMENT_BLOCK.value

**Contexte** : les blocs `RELATION` (kind = RELATION) permettent de lier un document à un autre via `{ "targetId": "uuid", "targetType": "..." }`. Pour afficher les backlinks (tous les documents qui pointent vers X), il faut requêter cette structure JSONB efficacement.

**Décision** : index GIN sur `DOCUMENT_BLOCK.value->>'targetId'`. La requête de backlinks est `SELECT documentId FROM DOCUMENT_BLOCK WHERE kind = 'RELATION' AND value->>'targetId' = :targetId`. Pas de table de liaison dédiée — l'index rend la requête performante.

**Alternatives écartées** :
- Table `DOCUMENT_LINK (sourceId, targetId)` dénormalisée : double source de vérité. À chaque modification d'un bloc RELATION, il faudrait maintenir la table. Risque d'incohérence en cas d'échec partiel.
- Requête séquentielle sans index : O(n) sur tous les blocs de la campagne — inacceptable à l'échelle.

**Conséquences** : les backlinks sont calculés à la lecture, pas stockés. Cohérence garantie sans double maintenance. L'index GIN est posé sur la colonne `value` existante — pas de nouvelle colonne.

**Règle d'affichage** : les backlinks orphelins (document source soft-deleted) sont filtrés à la requête via `AND d.isDeleted = false` jointure sur `DOCUMENT`. Ils restent en base mais ne sont jamais affichés. Voir ADR-23.

---

### ADR-19 — RequesterId comme union type pour l'autorisation unifiée

**Contexte** : `AccessPolicy.CanAccess` doit évaluer les droits d'accès pour deux types de demandeurs radicalement différents : un `User` authentifié (avec `UserId`) et un joueur invité via `GuestAccess` (sans `UserId`). Une signature limitée à un `DocumentId` et un `UserId` exclurait les invités et ne permettrait pas de lier les droits privés au personnage.

**Décision** : introduction de `RequesterId` comme union type scellée dans le Shared Kernel :

```csharp
abstract record RequesterId;
record AuthenticatedRequesterId(UserId UserId, CharacterId? CharacterId) : RequesterId;
record GuestRequesterId(GuestAccessId GuestAccessId, CharacterId? CharacterId) : RequesterId;
```

La signature devient `CanAccess(resource: ShareableResourceRef, requester: RequesterId) → bool`. L'algorithme résout le type de demandeur par pattern matching avant d'évaluer les règles.

**Alternatives écartées** :
- `CanAccess` avec deux surcharges séparées : duplication de la logique d'évaluation, risque d'incohérence entre les deux chemins.
- `UserId?` nullable : un `null` n'est pas un invité — c'est une absence de valeur. Crée des états ambigus.
- Traiter les invités comme des `User` avec un rôle `GUEST` : contredit ADR-05. Un `User` est une identité persistante avec email et authentification.

**Conséquences** : le Shared Kernel gagne deux nouveaux types record et une référence de ressource partageable. Tous les call sites de `CanAccess` doivent être mis à jour. L'avantage est que le compilateur force la gestion des deux cas sans possibilité d'ignorer silencieusement les invités.

---

### ADR-20 — Tags comme entités de campagne, pas comme value objects

**Contexte** : les documents peuvent être taggés. Deux modèles possibles : tags comme value objects embarqués dans `Document` (liste de strings) ou tags comme entités avec leur propre cycle de vie au niveau campagne.

**Décision** : `Tag` est une entité de premier niveau dans Content Library, avec `TagId`, `campaignId`, `label` (unique par campagne, case-insensitive), `color?` et `SoftDelete`. Les documents référencent des `TagId[]`. Une table de liaison `DOCUMENT_TAG` gère l'association.

**Alternatives écartées** :
- Tags comme strings embarqués dans `Document` : impossible de renommer un tag sur tous les documents, impossible de changer sa couleur, pas de liste canonique de tags. Évolutivité bloquée dès la première itération.
- Tags dans le Shared Kernel : les tags sont spécifiques à une campagne et ont des règles métier (unicité, couleur). Ce n'est pas une primitive — c'est un concept de Content Library.

**Conséquences** :
- UC-19 `GererTagsCampagne` est nécessaire pour le cycle de vie des tags.
- Le soft delete d'un tag émet `TagDeleted` — un handler purge `DOCUMENT_TAG` pour ce `TagId`.
- Les tags sont chargés une fois par contexte de campagne et mis en cache côté client pour les suggestions.

---

### ADR-21 — Dispatch des domain events synchrone en-process pour le MVP

**Contexte** : les domain events sont émis par les agrégats. Question de l'architecture de dispatch : synchrone (in-process, MediatR) ou asynchrone (message broker, outbox pattern).

**Décision** : dispatch synchrone en-process via MediatR pour le MVP. Les handlers sont appelés dans la même transaction applicative que la commande qui a émis l'événement.

**Alternatives écartées** :
- Message broker (RabbitMQ, Azure Service Bus) : complexité opérationnelle injustifiée pour un monolithe MVP. Introduit la latence, la gestion des dead letters, la supervision d'un service supplémentaire.
- Outbox pattern : pertinent pour la cohérence éventuelle cross-service. Inutile dans un monolithe où tous les handlers s'exécutent dans le même processus.

**Conséquences** :
- Les handlers `DocumentTitleUpdated`, `NpcDeleted`, `TagDeleted`, etc. s'exécutent dans la même transaction que la commande source.
- Risque : un handler défaillant fait échouer la commande entière — acceptable pour le MVP, préférable à des incohérences silencieuses.
- **Point d'extension** : si le monolithe est extrait en services, les domain events deviennent des messages inter-services. L'interface `IDomainEventHandler<T>` est préservée — seule l'implémentation de dispatch change.

---

### ADR-22 — Co-création par factory method pour Document + enveloppe métier

**Contexte** : `NPC`, `PlayerCharacter`, `Scenario` et `Scene` sont des enveloppes métier qui possèdent chacune un `Document` associé. Ces deux entités doivent être créées atomiquement — un `NPC` sans `Document` est un état invalide, et vice-versa.

**Décision** : chaque enveloppe expose une factory method statique pour la création. La factory crée l'enveloppe et son `Document` ensemble, les associe, et retourne les deux en une seule opération. La création directe d'un `Document` de type `NPC/CHARACTER/SCENARIO/SCENE` sans passer par la factory est interdite.

```csharp
// Factory — seul point d'entrée autorisé
var (npc, document) = NPC.Create(campaignId, name, templateId);

// Interdit — crée un Document orphelin sans enveloppe
var doc = Document.Create(campaignId, DocumentType.NPC, ...);
```

**Alternatives écartées** :
- Création séparée dans le handler Application : risque de transaction partielle si la deuxième création échoue. L'application porterait la responsabilité d'un invariant domaine.
- Un seul agrégat racine `NPC` contenant `Document` : trop couplé — `Document` a son propre agrégat racine avec des règles d'accès et de blocs indépendantes.

**Conséquences** : les handlers Application pour la création d'un NPC/Character/Scenario/Scene appellent uniquement les factories, jamais `Document.Create` directement pour ces types.

---

### ADR-23 — Backlinks orphelins filtrés à la requête

**Contexte** : un document peut avoir des blocs `RELATION` pointant vers d'autres documents. Si le document cible est soft-deleted, le backlink devient orphelin. Question : que faire des backlinks orphelins ?

**Décision** : les backlinks orphelins ne sont pas affichés. La requête de backlinks filtre systématiquement les documents sources avec `isDeleted = false`. Aucun nettoyage des blocs RELATION en base — le filtrage est applicatif, à la requête.

**Alternatives écartées** :
- Afficher les backlinks orphelins avec un état "document supprimé" : complexité UX pour un cas marginal. Le MJ ne veut pas voir des liens vers des documents qui n'existent plus.
- Purger les blocs RELATION lors du soft-delete du document cible : double propagation complexe — il faudrait retrouver tous les documents qui référencent la cible et modifier leurs blocs. Coûteux, fragile.
- Hard delete des blocs RELATION : suppression irréversible d'une information structurelle. Incompatible avec la stratégie soft-delete.

**Conséquences** : si un document est restauré (soft-delete annulé), ses backlinks réapparaissent automatiquement sans aucune migration. La stratégie est cohérente avec le reste du système.

---

### ADR-24 — UUID dans les URL, slugs pour l'affichage uniquement

**Contexte** : les URL des campagnes, documents, PNJ, etc. doivent identifier les ressources de façon stable. Deux approches courantes : slugs lisibles (`/campaigns/ma-campagne/documents/mon-pnj`) ou UUID (`/campaigns/550e8400-.../documents/6ba7b810-...`).

**Décision** : UUID dans les URL. Pattern : `/campaigns/{campaignId}/documents/{documentId}`. Les slugs (`Slug`) sont stockés comme champ de recherche et d'affichage, mais ne servent jamais à l'identification en URL.

**Alternatives écartées** :
- Slugs dans les URL : fragiles — un renommage change l'URL et casse les bookmarks. Nécessite une gestion de redirections. La contrainte d'unicité des slugs par campagne est plus complexe à maintenir qu'un UUID.
- Slugs avec redirection (canonical URL) : complexité inutile pour un SaaS B2C à ce stade.

**Conséquences** : les URL sont stables et opaques. Le `Slug` du `Shared Kernel` reste utile pour la recherche, les filtres et l'affichage dans les fils d'Ariane, mais n'a aucun impact sur le routage.

---

## Anti-patterns à éviter

### Ne pas mettre de règles métier dans les repositories

Un repository persiste et charge des entités. Il ne prend pas de décisions métier.

```csharp
// Mauvais — règle métier dans le repository
public class NpcRepository {
    public void Save(NPC npc) {
        if (npc.Status == NpcStatus.Dead)
            throw new Exception("Ne peut pas sauvegarder un NPC mort");
    }
}

// Bon — la règle est dans l'entité domaine
public class NPC {
    public void ChangeStatus(NpcStatus newStatus) {
        if (_status == NpcStatus.Dead)
            throw new DomainException("Un NPC mort ne peut pas changer de statut.");
        _status = newStatus;
        AddDomainEvent(new NpcStatusChanged(Id, _status, newStatus));
    }
}
```

### Ne pas contourner la racine d'agrégat

Pour modifier une `Scene`, on passe toujours par son `Scenario` parent.
On ne charge pas une `Scene` isolément pour la modifier directement.

```csharp
// Mauvais — contourne l'agrégat
var scene = sceneRepository.FindById(sceneId);
scene.MarkAsPlayed();
sceneRepository.Save(scene);

// Bon — passe par la racine
var scenario = scenarioRepository.FindById(scenarioId);
scenario.MarkSceneAsPlayed(sceneId);
scenarioRepository.Save(scenario);
```

### Ne pas exposer les entités domaine à la couche de présentation

Les entités domaine ne sont pas des DTOs. On ne les sérialise pas directement en JSON.
La couche Application crée des DTOs adaptés à chaque cas d'usage.

### Ne pas importer des entités d'un autre bounded context

```csharp
// Mauvais — Session Conduct importe une entité de Content Library
using ContentLibrary.Domain;
public class Session {
    public List<PlayerCharacter> Participants { get; set; }
}

// Bon — référence légère par Id typé du Shared Kernel
public class Session {
    public IReadOnlyList<CharacterId> ParticipantIds { get; private set; }
}
```

### Ne pas créer d'agrégat sans justification réelle

Un agrégat racine se justifie par des invariants transactionnels réels sur des
entités enfants. Créer des agrégats racines pour `NPC` ou `PlayerCharacter`
parce que "ça semble important" serait une erreur — ça ajoute de la complexité
sans valeur métier. Voir ADR-04.

### Ne pas déplacer la protection de GameSystem.isBuiltIn vers l'Application

La règle "un `GameSystem` built-in ne peut pas être modifié ou supprimé" est une règle métier de domaine. Elle appartient à l'entité `GameSystem`, pas à un handler Application.

```csharp
// Mauvais — règle métier dans le handler Application
public class UpdateGameSystemHandler {
    public async Task Handle(UpdateGameSystemCommand cmd) {
        var gs = await _repo.FindByIdAsync(cmd.Id);
        if (gs.IsBuiltIn)
            throw new InvalidOperationException("Impossible de modifier un système built-in.");
        // ...
    }
}

// Bon — règle dans l'entité domaine
public class GameSystem {
    public void Update(string name, string description) {
        if (_isBuiltIn)
            throw new DomainException("Un GameSystem built-in est immuable.");
        _name = name;
        _description = description;
    }
}
```

Si la protection est dans un handler, elle peut être contournée par un autre handler ou un event handler qui appelle directement le repository.

### Ne pas ignorer les champs dénormalisés

`NPC.name`, `Scenario.title` et d'autres sont des copies de `Document.title`.
Toute modification du titre d'un document doit déclencher `DocumentTitleUpdated`
pour que la synchronisation s'effectue. Ne pas émettre cet event crée des
incohérences entre l'affichage en liste et le contenu réel du document.

---

## Pour aller plus loin

### Lectures recommandées

- **Domain-Driven Design** — Eric Evans (le livre fondateur)
- **Implementing Domain-Driven Design** — Vaughn Vernon (plus pratique, exemples concrets)
- **Architecture Patterns with Python** — Harry Percival & Bob Gregory (accessible, patterns similaires)

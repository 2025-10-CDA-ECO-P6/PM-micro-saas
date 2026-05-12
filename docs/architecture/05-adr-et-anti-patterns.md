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

### ADR-04 — NPC et PlayerCharacter comme entités avec repository

**Contexte** : `NPC` et `PlayerCharacter` ont un cycle de vie important
mais pas d'entités enfants à protéger transactionnellement.

**Décision** : entités de premier niveau avec repository, pas d'agrégats racines.

**Alternatives écartées** :
- Agrégats racines : justification insuffisante — pas d'entités enfants,
  pas d'invariants transactionnels complexes.
- Entités enfants de `Campaign` : l'agrégat `Campaign` deviendrait ingérable,
  chargé de centaines d'entités à chaque accès.
- Sous-types de `Document` : mélange du contenu et de la logique métier.

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

**Décision** : service domaine `AccessPolicy` dans Campaign Management,
persistant des entités légères `ContentAccessRule` immuables.

**Alternatives écartées** :
- Agrégat `ShareGrant` : pas d'entités enfants, pas d'invariants transactionnels.
  Un agrégat racine serait surdimensionné.
- Champ `visibility` uniquement sur `Document` : insuffisant pour cibler
  un membre ou un personnage spécifique.
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

**Décision** : `Visibility.PLAYER_PRIVATE` — une note créée avec cette visibilité n'est accessible qu'au joueur `createdById`. Le MJ est explicitement exclu, même s'il est propriétaire de la campagne.

**Rationale** : respecte l'intention du joueur. Un joueur doit pouvoir noter des théories, mémos ou informations personnelles sans risquer que le MJ les lise accidentellement. Cela renforce la confiance dans l'outil côté joueurs.

**Alternatives écartées** :
- MJ voit tout par défaut : casse la promesse de confidentialité pour les joueurs, frein à l'adoption.
- Option de configuration par campagne : surcharge inutile — la règle est invariante.

**Implémentation** : la règle est appliquée dans `ContentAccessPolicy` (domaine), pas seulement dans les handlers Application. Un MJ qui charge les documents d'une campagne ne voit jamais les `PLAYER_PRIVATE` d'un autre joueur.

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

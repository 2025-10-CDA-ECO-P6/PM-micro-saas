# Content Library — Diagrammes de flux

## 1. Création des dossiers système à la création de campagne

```mermaid
sequenceDiagram
    participant App as Application Layer
    participant CM as Campaign Management
    participant CL as Content Library

    CM-->>App: CampaignCreated(campaignId, ownerId)
    App->>CL: Folder.Create(campaignId, "Personnages", isSystem=true)
    App->>CL: Folder.Create(campaignId, "Joueurs", isSystem=true)
    App->>CL: Folder.Create(campaignId, "Scénarios", isSystem=true)
    App->>CL: Folder.Create(campaignId, "Notes", isSystem=true)
    CL-->>App: 4 dossiers créés
```

## 2. Créer un document simple (PNJ, note…)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library

    MJ->>App: Créer un document (title, folderId, typeId?)
    App->>CL: Document.Create(campaignId, folderId, title, typeId?)
    CL-->>App: DocumentCreated event
    App-->>MJ: Document créé — éditeur ouvert
```

## 3. Créer un scénario avec scènes

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library

    MJ->>App: Créer un scénario
    App->>CL: Document.Create(campaignId, folderId, title, typeId=SCENARIO)
    CL-->>App: Document scénario créé (docId)

    loop Pour chaque scène
        MJ->>App: Ajouter une scène (title)
        App->>CL: Document.Create(campaignId, folderId, title, typeId=SCENE)
        CL-->>App: Document scène créé (sceneId)
        App->>CL: Document.LinkDocument(docId, targetId=sceneId, order)
        CL-->>App: Lien ajouté
    end

    App-->>MJ: Scénario structuré
```

## 4. Référencer un PNJ depuis une scène

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library

    Note over MJ: Le PNJ existe déjà dans la bibliothèque
    MJ->>App: Lier le PNJ à la scène (sceneId, npcId)
    App->>CL: Document.LinkDocument(sourceId=sceneId, targetId=npcId, order)
    CL-->>App: Lien ajouté
    App-->>MJ: PNJ référencé depuis la scène
```

## 5. Instancier un scénario réutilisable (UC-13)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library

    MJ->>App: Utiliser ce scénario pour un nouveau groupe (sourceDocId, campaignId)
    App->>CL: Document.Instantiate(campaignId, folderId)
    Note over CL: Copie profonde :<br/>blocs + liens + propriétés<br/>sourceDocumentId = sourceDocId<br/>isReusable = false sur l'instance
    CL-->>App: DocumentInstantiated (newDocId)
    App-->>MJ: Instance indépendante créée dans la campagne
```

## 6. Partager un document avec les joueurs

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library
    participant SC as Session Conduct

    MJ->>App: Partager ce document (documentId)
    App->>CL: Document.Share()
    CL-->>App: DocumentVisibilityChanged(GM_ONLY → PUBLIC)
    App->>SC: Notifier les joueurs connectés (temps réel)
    App-->>MJ: Document visible par les joueurs
    Note over App: Permanent — le joueur peut y accéder<br/>entre les sessions
```

## 7. Création à la volée en session (UC-07)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library
    participant SC as Session Conduct

    Note over MJ: En pleine session — un PNJ imprévu apparaît
    MJ->>App: Créer un PNJ à la volée (title, sessionId)
    App->>CL: Document.Create(campaignId, folderId, title, typeId=NPC)
    CL-->>App: DocumentCreated (docId)
    App->>SC: Ajouter le document à la session active (sessionId, docId)
    App-->>MJ: PNJ créé et visible dans la vue session (< 10 secondes)
```

## 8. Consulter les backlinks d'un document

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library

    MJ->>App: Voir les documents qui référencent ce PNJ (documentId)
    App->>CL: GetBacklinks(targetDocumentId)
    Note over CL: SELECT source_document_id FROM document_links<br/>WHERE target_document_id = :id
    CL-->>App: Liste des documents source
    App-->>MJ: "Ce PNJ est référencé par 3 scènes"
```

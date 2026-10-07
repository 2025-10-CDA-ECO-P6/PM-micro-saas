# Session Conduct — Diagrammes de flux

## 1. Initialisation de SessionViewConfig à la création d'espace

```mermaid
sequenceDiagram
    participant App as Application Layer
    participant SM as Space Management
    participant CL as Content Library
    participant SC as Session Conduct

    SM-->>App: SpaceCreated(spaceId)
    App->>CL: Créer les 4 dossiers système visibles + 1 dossier virtuel technique
    CL-->>App: [folderId1, folderId2, folderId3, folderId4, folderIdVirtual]
    App->>SC: SessionViewConfig.Create(spaceId, folderIds)
    SC-->>App: SessionViewConfig initialisé avec les dossiers système
```

## 2. Configurer les panneaux de la vue session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SC as Session Conduct

    MJ->>App: Ajouter un dossier à la vue session (folderId)
    App->>SC: SessionViewConfig.AddFolder(folderId, order)
    SC-->>App: OK
    App-->>MJ: Panneau ajouté

    MJ->>App: Retirer un dossier de la vue session (folderId)
    App->>SC: SessionViewConfig.RemoveFolder(folderId)
    SC-->>App: OK
    App-->>MJ: Panneau retiré
```

## 3. Démarrer une session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SC as Session Conduct

    MJ->>App: Démarrer une session (title, scenarioId?)
    App->>SC: Session.Start(spaceId, title, scenarioId?)
    SC-->>App: SessionStarted event
    App->>SC: Charger SessionViewConfig(spaceId)
    SC-->>App: Liste des dossiers en focus
    App-->>MJ: Vue session ouverte avec les panneaux configurés
```

## 4. Épingler un document en session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SC as Session Conduct

    MJ->>App: Épingler ce document (documentId)
    App->>SC: Session.PinDocument(documentId)
    SC-->>App: DocumentPinned event
    App-->>MJ: Document accessible depuis la vue session
    Note over App: Épingler ne change pas la visibility du document.<br/>Partager (rendre visible aux joueurs) est une opération séparée<br/>déclenchée sur Content Library.
```

## 5. Partager un document depuis la vue session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SC as Session Conduct
    participant CL as Content Library

    MJ->>App: Partager ce document avec les joueurs (documentId)
    App->>CL: Document.Share()
    CL-->>App: DocumentVisibilityChanged (→ PUBLIC)
    App->>SC: Session.PinDocument(documentId)
    SC-->>App: DocumentPinned event
    App-->>MJ: Document partagé et épinglé
    Note over App: Le partage est permanent (Content Library).<br/>L'épingle est locale à la session.
```

## 6. Créer une note de session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library
    participant SC as Session Conduct

    MJ->>App: Nouvelle note (content, visibility)
    App->>CL: Document.Create(spaceId, folderId="Notes", typeId=LIVE_NOTE)
    CL-->>App: DocumentCreated (docId)
    App->>CL: Document.UpdateContent([TextBlock(content)])
    App->>SC: Session.AttachNote(docId)
    SC-->>App: OK
    App-->>MJ: Note enregistrée
    Note over App: La note est un Document — elle peut être liée<br/>à d'autres documents via linkedDocuments.
```

## 7. Création à la volée en session (UC-07)

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant CL as Content Library
    participant SC as Session Conduct

    MJ->>App: Créer un PNJ à la volée (title, sessionId)
    App->>CL: Document.Create(spaceId, folderId, title, typeId=NPC)
    CL-->>App: DocumentCreated (docId)
    App->>SC: Session.PinDocument(docId)
    SC-->>App: DocumentPinned event
    App-->>MJ: PNJ créé et épinglé dans la vue session
```

## 8. Clôturer une session

```mermaid
sequenceDiagram
    actor MJ
    participant App as Application Layer
    participant SC as Session Conduct
    participant SM as Space Management

    MJ->>App: Clôturer la session
    App->>SC: Session.Close()
    SC-->>App: SessionClosed event
    App->>SM: Déclencher expiration GuestAccess SESSION (spaceId, sessionId)
    SM-->>App: OK
    App-->>MJ: Session clôturée — résumé éditable
```

## 9. Accès joueur à la session (sans compte)

```mermaid
sequenceDiagram
    actor Joueur
    participant App as Application Layer
    participant SM as Space Management
    participant SC as Session Conduct

    Joueur->>App: Accéder à la session (guestToken)
    App->>SM: Valider GuestAccess(token)
    alt GuestAccess invalide / expiré
        SM-->>App: Refusé
        App-->>Joueur: "Ce lien n'est plus actif"
    else GuestAccess valide
        SM-->>App: GuestAccessId + spaceId + characterId?
        App->>SC: Charger vue session (documents PUBLIC, notes de session PUBLIC)
        SC-->>App: Contenu autorisé
        App-->>Joueur: Vue joueur — documents partagés en temps réel
    end
```

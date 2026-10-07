# Session Conduct — Diagramme de classes

```mermaid
classDiagram
    class Session {
        +identifiant id
        +identifiant spaceId
        +texte title
        +SessionStatus status
        +identifiant scenarioId
        +liste de identifiant pinnedDocumentIds
        +liste de identifiant sessionNoteIds
        +texte summary
        +horodatage startedAt
        +horodatage closedAt
        +AuditInfo auditInfo
        +Start(spaceId, title, scenarioId?)$
        +Close()
        +Archive()
        +PinDocument(docId)
        +UnpinDocument(docId)
        +AttachNote(documentId)
        +UpdateSummary(text)
    }

    class SessionViewConfig {
        +identifiant id
        +identifiant spaceId
        +liste de SessionViewFolder focusedFolders
        +horodatage updatedAt
        +AddFolder(folderId, order)
        +RemoveFolder(folderId)
        +ReorderFolders(orderedFolderIds)
    }

    class SessionViewFolder {
        <<valueObject>>
        +identifiant folderId
        +entier order
    }

    class SessionStatus {
        <<enumeration>>
        LIVE
        CLOSED
        ARCHIVED
    }

    class SessionStarted {
        +identifiant sessionId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class SessionClosed {
        +identifiant sessionId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class SessionArchived {
        +identifiant sessionId
        +horodatage occurredAt
    }

    class DocumentPinned {
        +identifiant sessionId
        +identifiant documentId
        +horodatage occurredAt
    }

    class DocumentUnpinned {
        +identifiant sessionId
        +identifiant documentId
        +horodatage occurredAt
    }

    Session --> SessionStatus
    Session ..> SessionStarted : produces
    Session ..> SessionClosed : produces
    Session ..> SessionArchived : produces
    Session ..> DocumentPinned : produces
    Session ..> DocumentUnpinned : produces
    SessionViewConfig "1" *-- "0..*" SessionViewFolder : focusedFolders
```

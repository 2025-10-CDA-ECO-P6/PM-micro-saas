# Content Library — Diagramme de classes

```mermaid
classDiagram
    class Document {
        +identifiant id
        +identifiant spaceId
        +identifiant folderId
        +texte title
        +identifiant documentTypeId
        +structure properties
        +liste de DocumentBlock blocks
        +liste de DocumentLink linkedDocuments
        +Visibility visibility
        +liste de Tag tags
        +Slug slug
        +booléen isReusable
        +identifiant sourceDocumentId
        +identifiant characterId
        +identifiant guestAccessId
        +AuditInfo auditInfo
        +SoftDelete softDelete
        +Create(spaceId, folderId, title, typeId)$
        +UpdateContent(blocks)
        +LinkDocument(targetId, order)
        +UnlinkDocument(targetId)
        +Share()
        +Unshare()
        +Delete()
        +Instantiate(spaceId, folderId)
    }

    class DocumentBlock {
        +identifiant id
        +entier order
        +BlockType type
        +structure content
        +booléen isLocked
    }

    class DocumentLink {
        <<valueObject>>
        +identifiant targetDocumentId
        +entier order
    }

    class Folder {
        +identifiant id
        +identifiant spaceId
        +identifiant parentFolderId
        +texte name
        +booléen isSystem
        +booléen isVirtual
        +identifiant defaultDocumentTypeId
        +identifiant defaultTemplateDocumentId
        +entier order
        +AuditInfo auditInfo
        +Create(spaceId, name, parentId?)$
        +Rename(name)
        +Delete()
    }

    class DocumentType {
        +identifiant id
        +texte slug
        +texte name
        +structure propertiesSchema
        +booléen isSystem
        +identifiant spaceId
    }

    class BlockType {
        <<enumeration>>
        TEXT
        TABLE
        IMAGE
        DIVIDER
    }

    class DocumentCreated {
        +identifiant documentId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class DocumentVisibilityChanged {
        +identifiant documentId
        +Visibility previousVisibility
        +Visibility newVisibility
        +horodatage occurredAt
    }

    class DocumentDeleted {
        +identifiant documentId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class DocumentInstantiated {
        +identifiant sourceDocumentId
        +identifiant newDocumentId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class DocumentLinked {
        +identifiant sourceDocumentId
        +identifiant targetDocumentId
        +entier order
        +horodatage occurredAt
    }

    class DocumentUnlinked {
        +identifiant sourceDocumentId
        +identifiant targetDocumentId
        +horodatage occurredAt
    }

    class FolderCreated {
        +identifiant folderId
        +identifiant spaceId
        +horodatage occurredAt
    }

    class FolderDeleted {
        +identifiant folderId
        +identifiant spaceId
        +horodatage occurredAt
    }

    Document "1" *-- "0..*" DocumentBlock : blocks
    Document "1" *-- "0..*" DocumentLink : linkedDocuments
    Document --> DocumentType : typed by
    Document ..> DocumentCreated : produces
    Document ..> DocumentVisibilityChanged : produces
    Document ..> DocumentDeleted : produces
    Document ..> DocumentInstantiated : produces
    Document ..> DocumentLinked : produces
    Document ..> DocumentUnlinked : produces
    Folder ..> FolderCreated : produces
    Folder ..> FolderDeleted : produces
    DocumentBlock --> BlockType
    Folder "1" o-- "0..*" Document : contains
    Folder "0..1" o-- "0..*" Folder : parent
```

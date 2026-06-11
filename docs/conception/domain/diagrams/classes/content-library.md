# Content Library — Diagramme de classes

```mermaid
classDiagram
    class Document {
        +identifiant id
        +identifiant campaignId
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
        +Create(campaignId, folderId, title, typeId)$
        +UpdateContent(blocks)
        +LinkDocument(targetId, order)
        +UnlinkDocument(targetId)
        +Share()
        +Unshare()
        +Delete()
        +Instantiate(campaignId, folderId)
    }

    class DocumentBlock {
        +identifiant id
        +entier order
        +BlockType type
        +structure content
        +booléen isLocked
    }

    class DocumentLink {
        +identifiant targetDocumentId
        +entier order
    }

    class Folder {
        +identifiant id
        +identifiant campaignId
        +identifiant parentFolderId
        +texte name
        +booléen isSystem
        +booléen isVirtual
        +identifiant defaultDocumentTypeId
        +identifiant defaultTemplateDocumentId
        +entier order
        +AuditInfo auditInfo
        +Create(campaignId, name, parentId?)$
        +Rename(name)
        +Delete()
    }

    class DocumentType {
        +identifiant id
        +texte slug
        +texte name
        +structure propertiesSchema
        +booléen isSystem
        +identifiant campaignId
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
        +identifiant campaignId
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
        +identifiant campaignId
        +horodatage occurredAt
    }

    class DocumentInstantiated {
        +identifiant sourceDocumentId
        +identifiant newDocumentId
        +identifiant campaignId
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

    class FolderDeleted {
        +identifiant folderId
        +identifiant campaignId
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
    Folder ..> FolderDeleted : produces
    DocumentBlock --> BlockType
    Folder "1" o-- "0..*" Document : contains
    Folder "0..1" o-- "0..*" Folder : parent
```

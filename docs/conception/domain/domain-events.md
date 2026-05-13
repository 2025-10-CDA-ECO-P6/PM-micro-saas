# Domain Events — vue globale

### Identity & Access
```
UserRegistered, UserDeactivated
```

### Campaign Management
```
CampaignCreated, CampaignStatusChanged, CampaignOwnerTransferred
MemberJoined, MemberRemoved
InvitationCreated, InvitationRevoked
GuestAccessCreated, GuestAccessExpired, GuestAccessCharacterAssigned
ContentAccessGranted, ContentAccessRevoked
```

### Content Library
```
DocumentCreated, DocumentTitleUpdated, DocumentVisibilityChanged
DocumentMovedToFolder, DocumentDeleted
BlockAdded, BlockUpdated, BlockRemoved, BlockReordered
TagCreated, TagUpdated, TagDeleted
DocumentTypeCreated, DocumentTypeUpdated, DocumentTypeDeleted
TemplateCreated, TemplateUpdated
FolderCreated, FolderRenamed, FolderDeleted
CharacterCreated, CharacterOwnerAssigned, CharacterStatusChanged
CharacterLinkedToDocument, CharacterUnlinkedFromDocument
ScenarioCreated, ScenarioStatusChanged, ScenarioReordered
ScenarioMarkedAsTemplate, ScenarioInstanceCreated
SceneAdded, SceneRemoved, SceneStatusChanged, SceneReordered
SceneDocumentLinked, SceneDocumentUnlinked
```

### Session Conduct
```
SessionPlanned, SessionStarted, SessionClosed, SessionArchived
SessionDocumentSelected, SessionDocumentDeselected
LiveNoteAdded, LiveNoteAddedPostSession, LiveNoteRemoved, LiveNoteVisibilityChanged
DocumentPinnedToSession, DocumentUnpinnedFromSession
```

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
TemplateCreated, TemplateSchemaUpdated, TemplateAppliedToDocument
FolderCreated, FolderRenamed, FolderDeleted
NpcCreated, NpcStatusChanged, NpcLinkedToCharacter, NpcUnlinkedFromCharacter
CharacterCreated, CharacterOwnerAssigned, CharacterStatusChanged
CharacterLinkedToNpc, CharacterUnlinkedFromNpc
ScenarioCreated, ScenarioStatusChanged, ScenarioReordered
SceneAdded, SceneRemoved, SceneStatusChanged, SceneReordered
SceneNpcLinked, SceneNpcUnlinked
```

### Session Conduct
```
SessionPlanned, SessionStarted, SessionClosed, SessionArchived
SessionNpcSelected, SessionNpcDeselected
LiveNoteAdded, LiveNoteAddedPostSession, LiveNoteRemoved, LiveNoteVisibilityChanged
DocumentPinnedToSession, DocumentUnpinnedFromSession
SessionSummaryCreated, SessionSummaryUpdated, SessionSummaryVisibilityChanged
```

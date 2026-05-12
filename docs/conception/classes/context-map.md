# Diagramme de classes — Context Map

Vue globale des dépendances entre bounded contexts. Les flèches indiquent
la direction de consommation — il n'y a pas de dépendance circulaire.

Le **Shared Kernel** est la fondation commune consommée par tous les contextes.
Il contient uniquement des primitives stables sans règle métier, ainsi que
`RequesterId` (type union pour l'autorisation des GuestAccess).

**Identity & Access** est le contexte upstream — il fournit `UserId` à tous les autres.

**Campaign Management** fournit `CampaignId` aux deux contextes en aval. Il héberge
`AccessPolicy` dont la `ContentAccessRule` référence une `ShareableResourceRef`
(`Document`, `LiveNote` ou `SessionSummary`) par Id uniquement.

**Content Library** fournit ses Id (`DocumentId`, `CharacterId`, `ScenarioId`, `TagId`) à Session Conduct.

```mermaid
graph TB
    subgraph CORE[Shared Kernel]
        IDS[Id types incl. AccessRuleId TagId]
        PRIM[AuditInfo SoftDelete Email Slug Visibility PinnedItem]
        REQ[RequesterId AuthenticatedRequesterId GuestRequesterId]
        ABS[IAggregateRoot IEntity IDomainEvent IRepository IUnitOfWork]
    end
    subgraph IA[Identity and Access]
        U[User]
    end
    subgraph CM[Campaign Management]
        CA[Campaign]
        MB[CampaignMembership]
        INV[Invitation]
        GA[GuestAccess]
        GS[GameSystem]
        AP[AccessPolicy]
        CR[ContentAccessRule]
    end
    subgraph CL[Content Library]
        DOC[Document et DocumentBlock]
        NPC[NPC]
        PC[PlayerCharacter]
        SCE[Scenario et Scene]
        TPL[DocumentTemplate]
        FLD[Folder]
        TAG[Tag]
    end
    subgraph SE[Session Conduct]
        SS[Session]
        LN[LiveNote]
        SUM[SessionSummary]
        PI[PinnedItem]
    end
    CORE -.->|primitives Id types RequesterId| IA
    CORE -.->|primitives Id types RequesterId| CM
    CORE -.->|primitives Id types RequesterId| CL
    CORE -.->|primitives Id types RequesterId| SE
    IA -->|UserId| CM
    IA -->|UserId| CL
    IA -->|UserId| SE
    CM -->|CampaignId| CL
    CM -->|CampaignId| SE
    CR -.->|DocumentId ref cross-context| DOC
    CL -->|DocumentId CharacterId ScenarioId TagId| SE
    FLD -.->|contient par folderId FK| DOC
    CA --- MB
    CA --- INV
    INV -.->|cree| GA
    AP --- CR
    CA -.->|ref| GS
```

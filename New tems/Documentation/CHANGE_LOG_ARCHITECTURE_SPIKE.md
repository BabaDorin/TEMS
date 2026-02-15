# Change Log / Audit Trail Architecture Spike

**Author:** Senior Software Architect  
**Date:** February 15, 2026  
**Status:** Architectural Design Proposal

---

## Executive Summary

This document proposes a comprehensive architecture for implementing change log tracking (audit trail) across the TEMS application. After analyzing the existing codebase, I recommend **creating a dedicated ChangeLog module** that integrates seamlessly with the current vertical slice architecture using MediatR domain events.

**Key Decision:** Standalone Module + Event-Driven Integration  
**Rationale:** Maintains loose coupling, follows existing patterns, enables independent scaling and querying

---

## Current Architecture Analysis

### Modular Architecture (Vertical Slices)
TEMS follows a clean modular architecture with the following characteristics:

```
Backend/Tems/
├── Tems.Common/              # Shared contracts, notifications, tenant context
├── Tems.Host/                # ASP.NET Core host, DI registration, middleware
└── Modules/
    ├── AssetManagement/      # Assets, Types, Definitions, Properties
    ├── LocationManagement/   # Sites, Buildings, Rooms
    ├── UserManagement/       # Users, profiles, authentication bridge
    ├── TicketManagement/     # Tickets, ticket types
    └── EquipmentManagement/  # (Future module)
```

Each module follows a **4-layer structure**:
1. **API** - FastEndpoints, HTTP routing, authentication policies
2. **Application** - MediatR command/query handlers, business logic
3. **Contract** - Commands, queries, DTOs, responses (shared contracts)
4. **Infrastructure** - MongoDB repositories, external service clients

### Key Architectural Patterns Identified

#### 1. **MediatR-Centric Communication**
- All business operations are commands (`IRequest<TResponse>`)
- Cross-module communication via `INotification` published events
- Examples:
  - `UserDeletedNotification` → `UserDeletedNotificationHandler` (in AssetManagement)
  - `LocationDeletedNotification` → `LocationDeletedNotificationHandler` (in AssetManagement)

#### 2. **MongoDB Document Store**
- Each module has its own collections
- No shared schemas between modules
- Repositories use `IMongoDatabase` injected from host
- Primary constructors for all services (C# 12+, .NET 9)

#### 3. **Domain-Centric Design**
- Rich domain models in `Application/Domain` namespace
- Domain models map to/from database entities via explicit mappers
- No shared base entity classes - each module defines its own entities
- Examples:
  - `AssetManagement.Application.Domain.Asset`
  - `UserManagement.Infrastructure.Entities.User`

#### 4. **Tenant Isolation**
- `ITenantContext` injected into handlers
- All data scoped by `TenantId` (multi-tenancy ready)
- Middleware extracts tenant from JWT claims

#### 5. **Event Sourcing Lite (Current State)**
- **NOT using full event sourcing** - no event store
- **State-based persistence** - only current state is stored
- **Change tracking** - limited to `CreatedAt`, `UpdatedAt`, `CreatedBy` fields
- **No audit trail** - historical changes are not preserved

---

## Requirements Analysis

### Functional Requirements
1. **Entity Coverage:** Track changes for Assets, Locations (Sites/Buildings/Rooms), Users, Tickets
2. **Event Types:**
   - **Created** - Entity first created
   - **Updated** - Field-level changes (what changed, from → to)
   - **Deleted** - Entity marked for deletion or archived
   - **Assigned** - Asset assigned to user/location
   - **Unassigned** - Asset unassigned
   - **Status Changed** - Workflow state transitions
   - **Custom Events** - Module-specific events (e.g., Maintenance Performed, Ticket Resolved)
3. **Timeline View:** Chronological display of changes per entity
4. **Audit Metadata:**
   - Who (userId, username)
   - When (timestamp)
   - What (entity type, entity ID, action)
   - Changes (field-level diffs)
   - Context (IP address, user agent, tenant)

### Non-Functional Requirements
1. **Performance:** Change log writes must not slow down primary operations
2. **Scalability:** Support high-volume change tracking (thousands of changes/day)
3. **Query Performance:** Fast retrieval of entity timelines
4. **Storage Efficiency:** Compact representation of changes
5. **Consistency:** Changes logged atomically with business operations
6. **Compliance:** Immutable audit trail (no deletion/editing)

---

## Proposed Architecture

### Option A: **Dedicated ChangeLog Module** ⭐ **RECOMMENDED**

#### Structure
```
Modules/
└── ChangeLog/
    ├── ChangeLog.API/
    │   ├── Endpoints/
    │   │   └── GetEntityTimelineEndpoint.cs       # GET /changelog/{entityType}/{entityId}
    │   └── ChangeLogServiceRegistration.cs
    ├── ChangeLog.Application/
    │   ├── Domain/
    │   │   ├── ChangeLogEntry.cs                  # Rich domain model
    │   │   └── FieldChange.cs                     # Before/After value pair
    │   ├── Handlers/
    │   │   ├── AssetCreatedNotificationHandler.cs # Subscribes to MediatR notifications
    │   │   ├── AssetUpdatedNotificationHandler.cs
    │   │   ├── UserDeletedNotificationHandler.cs  # Already exists pattern
    │   │   └── ...
    │   └── Interfaces/
    │       └── IChangeLogRepository.cs
    ├── ChangeLog.Contract/
    │   ├── Commands/
    │   │   └── GetEntityTimelineCommand.cs
    │   ├── DTOs/
    │   │   └── ChangeLogEntryDto.cs
    │   └── Responses/
    │       └── GetEntityTimelineResponse.cs
    └── ChangeLog.Infrastructure/
        ├── Entities/
        │   └── ChangeLogEntryDb.cs                # MongoDB document
        └── Repositories/
            └── ChangeLogRepository.cs
```

#### Domain Model

```csharp
namespace ChangeLog.Application.Domain;

public class ChangeLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    
    // What changed
    public string EntityType { get; set; } = string.Empty;  // "Asset", "User", "Location"
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;  // Display name (AssetTag, Username, etc.)
    
    // Action
    public ChangeAction Action { get; set; }  // Created, Updated, Deleted, Assigned, etc.
    public string ActionDescription { get; set; } = string.Empty;  // Human-readable
    
    // Changes (field-level diffs)
    public List<FieldChange> Changes { get; set; } = [];
    
    // Metadata
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    
    // Contextual data
    public Dictionary<string, string> Metadata { get; set; } = new();  // Flexible key-value pairs
}

public class FieldChange
{
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string FieldType { get; set; } = "string";  // For display formatting
}

public enum ChangeAction
{
    Created,
    Updated,
    Deleted,
    Assigned,
    Unassigned,
    StatusChanged,
    Archived,
    Restored,
    Custom
}
```

#### Integration Pattern: Domain Events via Tems.Common

**Step 1:** Define new notifications in `Tems.Common/Notifications/`

```csharp
// Tems.Common/Notifications/AssetChangeNotifications.cs
namespace Tems.Common.Notifications;

public record AssetCreatedNotification(
    string AssetId,
    string AssetTag,
    string DefinitionName,
    string UserId,
    string Username
) : INotification;

public record AssetUpdatedNotification(
    string AssetId,
    string AssetTag,
    Dictionary<string, (object? OldValue, object? NewValue)> Changes,
    string UserId,
    string Username
) : INotification;

public record AssetAssignedNotification(
    string AssetId,
    string AssetTag,
    string AssignedToUserId,
    string AssignedToName,
    string? PreviousUserId,
    string UserId,
    string Username
) : INotification;

// Similar for LocationChangeNotifications, UserChangeNotifications, etc.
```

**Step 2:** Publish notifications from command handlers

```csharp
// AssetManagement.Application/Commands/CreateAssetCommandHandler.cs
public class CreateAssetCommandHandler(
    IAssetRepository assetRepository,
    IPublisher publisher  // ← Inject MediatR publisher
) : IRequestHandler<CreateAssetCommand, CreateAssetResponse>
{
    public async Task<CreateAssetResponse> Handle(CreateAssetCommand request, CancellationToken ct)
    {
        // ... existing creation logic ...
        var asset = await assetRepository.CreateAsync(domainEntity, ct);
        
        // Publish change event
        await publisher.Publish(new AssetCreatedNotification(
            AssetId: asset.Id,
            AssetTag: asset.AssetTag,
            DefinitionName: asset.Definition.Name,
            UserId: request.CreatedBy,  // From authenticated context
            Username: "john.doe"  // Resolved from user service or JWT
        ), ct);
        
        return new CreateAssetResponse(true, "Asset created", assetDto);
    }
}
```

**Step 3:** ChangeLog module subscribes to notifications

```csharp
// ChangeLog.Application/Handlers/AssetCreatedNotificationHandler.cs
public class AssetCreatedNotificationHandler(
    IChangeLogRepository changeLogRepository
) : INotificationHandler<AssetCreatedNotification>
{
    public async Task Handle(AssetCreatedNotification notification, CancellationToken ct)
    {
        var entry = new ChangeLogEntry
        {
            EntityType = "Asset",
            EntityId = notification.AssetId,
            EntityName = notification.AssetTag,
            Action = ChangeAction.Created,
            ActionDescription = $"Asset '{notification.AssetTag}' created with definition '{notification.DefinitionName}'",
            UserId = notification.UserId,
            Username = notification.Username,
            Timestamp = DateTime.UtcNow,
            Changes = [],
            Metadata = new()
            {
                ["DefinitionName"] = notification.DefinitionName
            }
        };
        
        await changeLogRepository.CreateAsync(entry, ct);
    }
}
```

#### Database Schema (MongoDB)

**Collection:** `changelogs`

```json
{
  "_id": "65abc123...",
  "tenant_id": "default-tenant",
  "entity_type": "Asset",
  "entity_id": "asset-123",
  "entity_name": "LAP-001",
  "action": "Updated",
  "action_description": "Asset details updated",
  "changes": [
    {
      "field_name": "Status",
      "old_value": "Available",
      "new_value": "In Use",
      "field_type": "enum"
    },
    {
      "field_name": "SerialNumber",
      "old_value": "SN123",
      "new_value": "SN123-UPDATED",
      "field_type": "string"
    }
  ],
  "user_id": "user-456",
  "username": "john.doe",
  "timestamp": "2026-02-15T10:30:00Z",
  "ip_address": "192.168.1.100",
  "user_agent": "Mozilla/5.0...",
  "metadata": {
    "module": "AssetManagement",
    "source": "Web UI"
  }
}
```

**Indexes:**
- `{ entity_type: 1, entity_id: 1, timestamp: -1 }` - Timeline queries
- `{ tenant_id: 1, timestamp: -1 }` - Global audit log
- `{ user_id: 1, timestamp: -1 }` - User activity
- `{ timestamp: -1 }` - Chronological queries

#### API Endpoint

```csharp
// ChangeLog.API/Endpoints/GetEntityTimelineEndpoint.cs
public class GetEntityTimelineEndpoint(IMediator mediator) 
    : EndpointWithoutRequest<GetEntityTimelineResponse>
{
    public override void Configure()
    {
        Get("/changelog/{entityType}/{entityId}");
        Policies("CanViewChangeLogs");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var entityType = Route<string>("entityType")!;
        var entityId = Route<string>("entityId")!;
        var pageNumber = Query<int>("pageNumber", false) ?? 1;
        var pageSize = Query<int>("pageSize", false) ?? 50;
        
        var command = new GetEntityTimelineCommand(entityType, entityId, pageNumber, pageSize);
        var response = await mediator.Send(command, ct);
        
        await SendOkAsync(response, ct);
    }
}
```

---

### Option B: Embedded in Each Module (NOT Recommended)

Each module maintains its own change log collection and logic.

**Pros:**
- Fully isolated per module
- No cross-module dependencies

**Cons:**
- **Code duplication** - Same change tracking logic in every module
- **Inconsistent schemas** - Each module defines its own change format
- **Complex querying** - Cross-entity timelines require federated queries
- **Violates DRY** - Maintenance nightmare

**Verdict:** ❌ Rejected - Does not align with TEMS architecture principles

---

### Option C: Tems.Common Shared Library

Put change log logic in `Tems.Common` as shared utilities.

**Pros:**
- Reusable across modules
- Central configuration

**Cons:**
- **Coupling** - Every module depends on Tems.Common implementation details
- **No clear ownership** - Who maintains the change log storage?
- **Mixed concerns** - Tems.Common should only contain contracts, not business logic
- **Difficult versioning** - Changes affect all modules simultaneously

**Verdict:** ❌ Rejected - Violates separation of concerns

---

## Recommended Approach: Option A with Enhancements

### Implementation Phases

#### Phase 1: Foundation (Week 1-2)
1. Create ChangeLog module structure
2. Define core domain models (`ChangeLogEntry`, `FieldChange`)
3. Implement MongoDB repository with proper indexes
4. Create basic API endpoint (`GET /changelog/{entityType}/{entityId}`)
5. Add to `Tems.Host` DI registration

#### Phase 2: Asset Module Integration (Week 2-3)
1. Define `AssetChangeNotifications` in `Tems.Common`
2. Update `CreateAssetCommandHandler` to publish `AssetCreatedNotification`
3. Update `UpdateAssetCommandHandler` to publish `AssetUpdatedNotification`
4. Implement notification handlers in ChangeLog module
5. Build frontend timeline component for asset detail page

#### Phase 3: Extended Coverage (Week 4-5)
1. Add notifications for LocationManagement module
2. Add notifications for UserManagement module
3. Add notifications for TicketManagement module
4. Implement field-level change detection utility

#### Phase 4: Advanced Features (Week 6+)
1. Add filtering/search capabilities
2. Export timeline to PDF/Excel
3. Real-time timeline updates (SignalR)
4. Compliance reporting dashboard
5. Retention policies and archival

---

## Technical Considerations

### Change Detection Strategy

**For Updates:** Compare old vs new state

```csharp
public static List<FieldChange> DetectChanges<T>(T oldEntity, T newEntity)
{
    var changes = new List<FieldChange>();
    var properties = typeof(T).GetProperties();
    
    foreach (var prop in properties)
    {
        if (ShouldTrack(prop))
        {
            var oldValue = prop.GetValue(oldEntity);
            var newValue = prop.GetValue(newEntity);
            
            if (!Equals(oldValue, newValue))
            {
                changes.Add(new FieldChange
                {
                    FieldName = prop.Name,
                    OldValue = oldValue?.ToString(),
                    NewValue = newValue?.ToString(),
                    FieldType = GetFieldType(prop)
                });
            }
        }
    }
    
    return changes;
}
```

### Performance Optimization

1. **Async Fire-and-Forget:** Change log writes don't block main operation
   - MediatR `INotification` already supports this
   - Handlers run in background after command completes

2. **Batching:** Group multiple changes into single DB write
   ```csharp
   services.AddMediatR(cfg => 
   {
       cfg.RegisterServicesFromAssembly(typeof(ChangeLogModule).Assembly);
       cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);  // Parallel execution
   });
   ```

3. **Indexing:** Proper MongoDB indexes on query patterns
   ```csharp
   await collection.Indexes.CreateManyAsync([
       new CreateIndexModel<ChangeLogEntryDb>(
           Builders<ChangeLogEntryDb>.IndexKeys
               .Ascending(x => x.EntityType)
               .Ascending(x => x.EntityId)
               .Descending(x => x.Timestamp)),
       // ... other indexes
   ]);
   ```

4. **Pagination:** Always paginate timeline queries

### Security & Privacy

1. **Authentication:** Only authenticated users can view change logs
2. **Authorization:** Policy-based access (`CanViewChangeLogs`)
3. **Tenant Isolation:** All queries filtered by `TenantId`
4. **PII Masking:** Sensitive fields (e.g., password hashes) never logged
5. **Immutability:** Change log entries cannot be modified or deleted (audit compliance)

### Monitoring & Observability

1. **Logging:** Log all change log operations with correlation IDs
2. **Metrics:** Track change log write volume, latency
3. **Alerts:** Alert on write failures or excessive volume
4. **Health Checks:** Verify MongoDB connectivity

---

## Migration Strategy

### Backward Compatibility
- No changes to existing functionality
- ChangeLog module is **additive only**
- Existing modules continue to work without modification
- Gradual rollout per module

### Data Backfill
- Historical data **cannot be reconstructed** (no event store)
- Change log starts from implementation date forward
- Optionally seed with "Entity Created" entries for existing assets
  ```csharp
  var existingAssets = await assetRepository.GetAllAsync();
  foreach (var asset in existingAssets)
  {
      await changeLogRepository.CreateAsync(new ChangeLogEntry
      {
          EntityType = "Asset",
          EntityId = asset.Id,
          EntityName = asset.AssetTag,
          Action = ChangeAction.Created,
          ActionDescription = "Asset migrated to change log system",
          UserId = "system",
          Username = "System Migration",
          Timestamp = asset.CreatedAt,
          Changes = []
      });
  }
  ```

---

## Alternative: Full Event Sourcing (Future Consideration)

If TEMS evolves to require:
- **Complete audit trail reconstruction**
- **Point-in-time state queries** ("What did asset LAP-001 look like on Jan 15?")
- **Undo/Replay capabilities**

Consider migrating to **Event Sourcing** with **EventStoreDB** or **Marten**:
- All state changes captured as immutable events
- Current state derived from event stream
- Snapshots for performance optimization
- CQRS pattern for read models

**Trade-offs:**
- ✅ Complete history
- ✅ Audit compliance
- ✅ Temporal queries
- ❌ Complexity (2-3x development effort)
- ❌ Learning curve
- ❌ Operational overhead

**Recommendation:** Start with Option A (ChangeLog module), evaluate Event Sourcing if requirements justify it.

---

## Decision Matrix

| Criteria | Option A (ChangeLog Module) | Option B (Per-Module) | Option C (Tems.Common) |
|----------|----------------------------|----------------------|----------------------|
| **Alignment with Architecture** | ✅ Perfect fit | ❌ Poor fit | ⚠️ Misaligned |
| **Code Reusability** | ✅ Central implementation | ❌ Duplicated 4x | ✅ Shared |
| **Maintainability** | ✅ Single source of truth | ❌ 4 sources of truth | ⚠️ Tight coupling |
| **Query Performance** | ✅ Single collection | ❌ Federated queries | ✅ Single collection |
| **Scalability** | ✅ Independent scaling | ⚠️ Coupled to modules | ⚠️ Coupled to all |
| **Implementation Effort** | 2-3 weeks | 6-8 weeks (4x work) | 3-4 weeks |
| **Testing** | ✅ Isolated tests | ❌ 4x test suites | ⚠️ Integration tests |
| **Future Extensibility** | ✅ Easy to add features | ❌ Change 4 places | ⚠️ Risky changes |

---

## Conclusion & Recommendation

**Create a dedicated ChangeLog module** following the existing TEMS modular architecture.

### Why This Approach Wins:
1. ✅ **Architectural Consistency:** Follows existing module patterns
2. ✅ **Loose Coupling:** MediatR notifications for integration
3. ✅ **Single Responsibility:** ChangeLog owns audit trail concerns
4. ✅ **Performance:** Optimized indexes, async writes
5. ✅ **Scalability:** Independent MongoDB collection, can scale separately
6. ✅ **Maintainability:** One place to update change log logic
7. ✅ **Extensibility:** Easy to add new entity types and event types
8. ✅ **Compliance:** Immutable audit trail meets regulatory requirements

### Next Steps:
1. **Review & Approve:** Validate this architecture with team
2. **Spike Implementation:** Build proof-of-concept for Asset module (3-5 days)
3. **Iteration:** Refine based on learnings
4. **Full Implementation:** Follow phased rollout plan

---

## Appendix A: Example Timeline UI (React/Angular)

```typescript
interface TimelineEntry {
  id: string;
  timestamp: Date;
  action: string;
  description: string;
  user: { id: string; name: string };
  changes: { field: string; oldValue: string; newValue: string }[];
}

// Component renders chronological timeline with visual indicators
<Timeline entries={changeLogEntries}>
  <TimelineItem entry={entry}>
    <Icon type={entry.action} />  // Created, Updated, Assigned icons
    <Timestamp>{entry.timestamp}</Timestamp>
    <Description>{entry.description}</Description>
    <User>{entry.user.name}</User>
    <ChangesList changes={entry.changes} />  // Expandable field diffs
  </TimelineItem>
</Timeline>
```

---

## Appendix B: Sample Queries

**Get asset timeline:**
```csharp
GET /api/changelog/Asset/asset-123?pageNumber=1&pageSize=20
```

**Get user activity:**
```csharp
GET /api/changelog/user-activity/user-456?from=2026-01-01&to=2026-02-15
```

**Global audit log:**
```csharp
GET /api/changelog/all?tenantId=default&pageNumber=1&pageSize=100
```

---

**END OF SPIKE**

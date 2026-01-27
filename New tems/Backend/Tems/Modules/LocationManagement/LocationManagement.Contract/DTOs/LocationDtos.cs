namespace LocationManagement.Contract.DTOs;

public record SiteDto(
    string Id,
    string Name,
    string Code,
    string Timezone,
    bool IsActive,
    string TenantId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record BuildingDto(
    string Id,
    string SiteId,
    string Name,
    string AddressLine,
    string ManagerContact,
    string TenantId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record RoomDto(
    string Id,
    string BuildingId,
    string Name,
    string? RoomNumber,
    string? FloorLabel,
    string Type,
    int? Capacity,
    double? Area,
    string Status,
    string? Description,
    string TenantId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? SiteName,
    string? SiteId,
    string? BuildingName,
    Dictionary<string, int>? AssetCounts = null
);

public record LocationHierarchyDto(
    string Id,
    string Name,
    string Type,
    string? ParentId,
    string FullPath,
    string? SiteId,
    string? BuildingId
);

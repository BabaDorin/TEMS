namespace AssetManagement.Contract.DTOs;

public record AssetFilterDto(
    string? AssetTag = null,
    List<string>? AssetTypeIds = null,
    List<string>? DefinitionIds = null,
    bool IncludeArchived = false
);

namespace UserManagement.Contract.DTOs;

public record UserDto(
    string Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    List<string> TenantIds,
    string? KeycloakId,
    List<string> Roles,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Dictionary<string, int>? AssetCounts = null
);

public record RoleDto(
    string Id,
    string Name,
    string? Description
);

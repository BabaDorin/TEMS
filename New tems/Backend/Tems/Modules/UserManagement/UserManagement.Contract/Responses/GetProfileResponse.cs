namespace UserManagement.Contract.Responses;

public record GetProfileResponse(
    string Id,
    string Name,
    string Email,
    string? AvatarUrl,
    string IdentityProviderId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

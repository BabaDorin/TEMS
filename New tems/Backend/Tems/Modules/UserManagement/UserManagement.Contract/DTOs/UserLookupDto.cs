namespace UserManagement.Contract.DTOs;

public record UserLookupDto(
    string Id,
    string Name,
    string Email,
    string DisplayName
);

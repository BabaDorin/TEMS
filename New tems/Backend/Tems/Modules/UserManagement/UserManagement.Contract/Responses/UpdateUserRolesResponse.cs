using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Responses;

public record UpdateUserRolesResponse(
    bool Success,
    string? Message,
    UserDto? User
);

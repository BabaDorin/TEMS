using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Responses;

public record CreateUserResponse(
    bool Success,
    string? Message,
    UserDto? User
);

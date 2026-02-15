namespace UserManagement.Contract.Responses;

public record DeleteUserResponse(
    bool Success,
    string? Message
);

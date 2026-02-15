using MediatR;

namespace Tems.Common.Notifications;

/// <summary>
/// Published after a user has been removed from Keycloak, Duende, and the TEMS database.
/// Interested modules should handle this to clean up any references to the deleted user.
/// </summary>
public record UserDeletedNotification(string UserId, string? UserName) : INotification;

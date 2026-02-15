using MediatR;

namespace Tems.Common.Notifications;

public record UserCreatedNotification(
    string UserId,
    string UserName,
    string Email,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record UserRolesUpdatedNotification(
    string UserId,
    string UserName,
    List<string> AddedRoles,
    List<string> RemovedRoles,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

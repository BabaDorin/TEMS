using MediatR;

namespace Tems.Common.Notifications;

public record LocationCreatedNotification(
    string LocationId,
    string LocationName,
    string LocationType,
    string? ParentId,
    string? ParentName,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

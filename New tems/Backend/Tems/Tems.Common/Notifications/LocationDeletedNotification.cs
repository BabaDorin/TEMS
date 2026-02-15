using MediatR;

namespace Tems.Common.Notifications;

/// <summary>
/// Published after a location (room, building, or site) has been deleted.
/// Interested modules should handle this to clean up any references to the deleted location.
/// </summary>
public record LocationDeletedNotification(string LocationId, string LocationType) : INotification;

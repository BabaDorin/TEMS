using MediatR;

namespace Tems.Common.Notifications;

public record AssetCreatedNotification(
    string AssetId,
    string AssetTag,
    string DefinitionName,
    string? AssetTypeName,
    string Status,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetUpdatedNotification(
    string AssetId,
    string AssetTag,
    List<AssetFieldChange> Changes,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetFieldChange(
    string FieldName,
    string? OldValue,
    string? NewValue
);

public record AssetDeletedNotification(
    string AssetId,
    string AssetTag,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetAssignedToUserNotification(
    string AssetId,
    string AssetTag,
    string UserId,
    string UserName,
    string? PreviousUserId,
    string? PreviousUserName,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetUnassignedFromUserNotification(
    string AssetId,
    string AssetTag,
    string UserId,
    string UserName,
    string? Reason,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetAssignedToLocationNotification(
    string AssetId,
    string AssetTag,
    string LocationId,
    string LocationName,
    string? PreviousLocationId,
    string? PreviousLocationName,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetUnassignedFromLocationNotification(
    string AssetId,
    string AssetTag,
    string LocationId,
    string LocationName,
    string? Reason,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetManualLogAddedNotification(
    string AssetId,
    string AssetTag,
    string LogType,
    string LogDescription,
    decimal? CostAmount,
    string? CostCurrency,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

public record AssetMentionedInTicketNotification(
    string AssetId,
    string AssetTag,
    string TicketId,
    string TicketHumanReadableId,
    string TicketTitle,
    string? PerformedByUserId,
    string? PerformedByUserName
) : INotification;

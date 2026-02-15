using ChangeLog.Contract.Enums;

namespace ChangeLog.Contract.DTOs;

public record ChangeLogEntryDto(
    string Id,
    ChangeLogAction Action,
    string Description,
    DateTime Timestamp,
    string? PerformedByUserId,
    string? PerformedByUserName,
    Dictionary<string, string?> References,
    Dictionary<string, object?>? Details
);

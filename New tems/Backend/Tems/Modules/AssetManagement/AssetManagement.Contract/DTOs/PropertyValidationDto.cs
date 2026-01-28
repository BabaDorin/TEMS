namespace AssetManagement.Contract.DTOs;

public record PropertyValidationDto(
    string Type,
    int? MaxLength,
    string? Pattern,
    int? Min,
    int? Max,
    string? Unit,
    List<string>? EnumValues);

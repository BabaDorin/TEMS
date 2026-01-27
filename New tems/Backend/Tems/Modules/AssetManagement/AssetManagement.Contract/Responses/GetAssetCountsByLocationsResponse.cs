namespace AssetManagement.Contract.Responses;

public record GetAssetCountsByLocationsResponse(
    bool Success,
    string? Message,
    Dictionary<string, Dictionary<string, int>> Data
);

public record AssetTypeCountDto(string AssetTypeName, int Count);

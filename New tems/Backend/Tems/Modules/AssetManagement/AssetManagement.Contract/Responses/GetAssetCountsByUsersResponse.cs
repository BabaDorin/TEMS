namespace AssetManagement.Contract.Responses;

public record GetAssetCountsByUsersResponse(
    bool Success,
    string? Message,
    Dictionary<string, Dictionary<string, int>> Data
);

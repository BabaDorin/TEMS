using System.Text.Json;

namespace AssetManagement.Application.Commands;

internal static class AssetSpecificationValueConverter
{
    public static object Convert(object? value)
    {
        if (value is not JsonElement jsonElement)
        {
            return value ?? string.Empty;
        }

        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
            JsonValueKind.Number => jsonElement.TryGetInt32(out var intValue) ? intValue :
                jsonElement.TryGetInt64(out var longValue) ? longValue :
                jsonElement.TryGetDecimal(out var decimalValue) ? decimalValue :
                jsonElement.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Undefined => string.Empty,
            _ => jsonElement.GetRawText()
        };
    }
}

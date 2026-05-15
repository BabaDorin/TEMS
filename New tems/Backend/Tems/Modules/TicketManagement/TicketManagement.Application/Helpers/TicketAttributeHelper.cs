using System.Text.Json;
using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Helpers;

public static class TicketAttributeHelper
{
    public static Dictionary<string, object> NormalizeAttributes(Dictionary<string, object>? attributes)
    {
        var converted = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        if (attributes == null)
        {
            return converted;
        }

        foreach (var kvp in attributes)
        {
            converted[kvp.Key] = ConvertValue(kvp.Value);
        }

        return converted;
    }

    public static void ValidateRequiredAttributes(TicketType ticketType, IDictionary<string, object> attributes)
    {
        var missingFields = ticketType.AttributeDefinitions
            .Where(attribute => attribute.IsRequired && !HasValue(attributes, attribute.Key))
            .Select(attribute => attribute.Label)
            .ToList();

        if (missingFields.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException($"Missing required ticket fields: {string.Join(", ", missingFields)}");
    }

    public static string GetRequiredString(IDictionary<string, object> attributes, string key, string label)
    {
        var value = GetString(attributes, key);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        throw new InvalidOperationException($"{label} is required");
    }

    public static string? GetString(IDictionary<string, object> attributes, string key)
    {
        if (!attributes.TryGetValue(key, out var value) || value == null)
        {
            return null;
        }

        return value switch
        {
            string text => text.Trim(),
            JsonElement json when json.ValueKind == JsonValueKind.String => json.GetString()?.Trim(),
            _ => value.ToString()?.Trim()
        };
    }

    public static decimal GetRequiredDecimal(IDictionary<string, object> attributes, string key, string label)
    {
        if (TryGetDecimal(attributes, key, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"{label} is required");
    }

    public static bool TryGetDecimal(IDictionary<string, object> attributes, string key, out decimal value)
    {
        value = 0m;
        if (!attributes.TryGetValue(key, out var rawValue) || rawValue == null)
        {
            return false;
        }

        switch (rawValue)
        {
            case decimal decimalValue:
                value = decimalValue;
                return true;
            case int intValue:
                value = intValue;
                return true;
            case long longValue:
                value = longValue;
                return true;
            case double doubleValue:
                value = Convert.ToDecimal(doubleValue);
                return true;
            case float floatValue:
                value = Convert.ToDecimal(floatValue);
                return true;
            case JsonElement json when json.ValueKind == JsonValueKind.Number:
                if (json.TryGetDecimal(out var jsonDecimal))
                {
                    value = jsonDecimal;
                    return true;
                }
                return false;
            default:
                return decimal.TryParse(rawValue.ToString(), out value);
        }
    }

    private static bool HasValue(IDictionary<string, object> attributes, string key)
    {
        if (!attributes.TryGetValue(key, out var value) || value == null)
        {
            return false;
        }

        return value switch
        {
            string text => !string.IsNullOrWhiteSpace(text),
            JsonElement json => json.ValueKind switch
            {
                JsonValueKind.Null => false,
                JsonValueKind.Undefined => false,
                JsonValueKind.String => !string.IsNullOrWhiteSpace(json.GetString()),
                JsonValueKind.Array => json.GetArrayLength() > 0,
                _ => true
            },
            _ => true
        };
    }

    private static object ConvertValue(object value)
    {
        if (value is not JsonElement jsonElement)
        {
            return value;
        }

        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
            JsonValueKind.Number => jsonElement.TryGetInt64(out var longValue) ? longValue :
                jsonElement.TryGetDecimal(out var decimalValue) ? decimalValue :
                jsonElement.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => string.Empty,
            _ => jsonElement.GetRawText()
        };
    }
}

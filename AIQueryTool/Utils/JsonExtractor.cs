using System.Text.Json;

namespace AIToolbox.Utils;

public class JsonExtractor
{
    public static string GetMessageFromJson(string json)
    {
        return ExtractValuesOnly(json);
    }
    
    private static string ExtractValuesOnly(string jsonLine)
    {
        using var doc = JsonDocument.Parse(jsonLine);
        return ExtractValuesRecursive(doc.RootElement);
    }
    private static string ExtractValuesRecursive(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => string.Join(" ", element.EnumerateObject().Select(p => ExtractValuesRecursive(p.Value))),
            JsonValueKind.Array => string.Join(" ", element.EnumerateArray().Select(ExtractValuesRecursive)),
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => element.ToString(),
            _ => string.Empty
        };
    }
}
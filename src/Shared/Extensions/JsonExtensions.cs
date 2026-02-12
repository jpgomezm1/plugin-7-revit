using System.Text.Json;

namespace DocumentationGeneratorAI.Shared.Extensions;

/// <summary>
/// Extension methods for JSON serialization and deserialization using System.Text.Json.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Default serializer options: camelCase naming, not indented, case-insensitive property matching.
    /// </summary>
    public static JsonSerializerOptions DefaultOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Serializes the object to a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="indented">If true, the JSON output is formatted with indentation.</param>
    public static string ToJson<T>(this T obj, bool indented = false)
    {
        if (indented)
        {
            var options = new JsonSerializerOptions(DefaultOptions)
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(obj, options);
        }

        return JsonSerializer.Serialize(obj, DefaultOptions);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object, or default if deserialization fails.</returns>
    public static T? FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }
}

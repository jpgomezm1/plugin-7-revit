using System.Reflection;
using System.Text.Json;

namespace DocumentationGeneratorAI.AiClient.Models;

public static class TextFormatSchema
{
    private static object? _cachedSchema;

    public static object GetSchema()
    {
        if (_cachedSchema != null) return _cachedSchema;

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("construction_document_schema.json"))
            ?? throw new InvalidOperationException("Embedded schema resource not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        _cachedSchema = JsonSerializer.Deserialize<object>(json)
            ?? throw new InvalidOperationException("Failed to deserialize schema.");
        return _cachedSchema;
    }
}

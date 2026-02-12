using System.Text.Json.Serialization;

namespace DocumentationGeneratorAI.AiClient.Models;

public sealed class ResponsesApiRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public List<InputMessage> Input { get; set; } = new();

    [JsonPropertyName("text")]
    public TextFormat? Text { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("max_output_tokens")]
    public int MaxOutputTokens { get; set; }

    [JsonPropertyName("store")]
    public bool Store { get; set; }
}

public sealed class InputMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public sealed class TextFormat
{
    [JsonPropertyName("format")]
    public FormatSpec Format { get; set; } = new();
}

public sealed class FormatSpec
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "json_schema";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "construction_document";

    [JsonPropertyName("strict")]
    public bool Strict { get; set; } = true;

    [JsonPropertyName("schema")]
    public object? Schema { get; set; }
}

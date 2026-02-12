using System.Text.Json.Serialization;

namespace DocumentationGeneratorAI.AiClient.Models;

public sealed class ResponsesApiResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("output")]
    public List<OutputItem> Output { get; set; } = new();

    [JsonPropertyName("error")]
    public ApiError? Error { get; set; }

    public string? GetOutputText()
    {
        var message = Output.FirstOrDefault(o => o.Type == "message");
        var textContent = message?.Content?.FirstOrDefault(c => c.Type == "output_text");
        return textContent?.Text;
    }
}

public sealed class OutputItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<ContentItem>? Content { get; set; }
}

public sealed class ContentItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public sealed class ApiError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

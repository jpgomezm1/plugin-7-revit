using System.Text.Json.Serialization;

namespace DocumentationGeneratorAI.AiClient.Models;

public sealed class AiDocumentResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonPropertyName("generatedDate")]
    public string GeneratedDate { get; set; } = string.Empty;

    [JsonPropertyName("sections")]
    public List<AiDocumentSection> Sections { get; set; } = new();

    [JsonPropertyName("metadata")]
    public AiDocumentMetadata Metadata { get; set; } = new();
}

public sealed class AiDocumentSection
{
    [JsonPropertyName("heading")]
    public string Heading { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}

public sealed class AiDocumentMetadata
{
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("phase")]
    public string Phase { get; set; } = string.Empty;

    [JsonPropertyName("audience")]
    public string Audience { get; set; } = string.Empty;

    [JsonPropertyName("detailLevel")]
    public string DetailLevel { get; set; } = string.Empty;

    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();
}

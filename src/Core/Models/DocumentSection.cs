namespace DocumentationGeneratorAI.Core.Models;

public sealed class DocumentSection
{
    public string Heading { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
}

namespace DocumentationGeneratorAI.Core.Models;

public sealed class ExtractionWarning
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ExtractionWarning() { }

    public ExtractionWarning(string code, string message)
    {
        Code = code;
        Message = message;
    }
}

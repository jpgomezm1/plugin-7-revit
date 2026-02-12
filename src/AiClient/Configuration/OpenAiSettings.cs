namespace DocumentationGeneratorAI.AiClient.Configuration;

public sealed class OpenAiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1";
    public double Temperature { get; set; } = 0.3;
    public int MaxOutputTokens { get; set; } = 16000;
    public int TimeoutSeconds { get; set; } = 120;
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
}

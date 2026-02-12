using System.Text.Json;
using DocumentationGeneratorAI.AiClient.Configuration;
using DocumentationGeneratorAI.AiClient.Models;
using DocumentationGeneratorAI.AiClient.Validation;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Configuration;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.AiClient;

public sealed class OpenAiDocumentGenerator : IAiDocumentGenerator, IDisposable
{
    private readonly ResponsesApiClient _apiClient;
    private readonly PromptBuilder _promptBuilder;
    private readonly ResponseValidator _validator;
    private readonly OpenAiSettings _settings;
    private readonly IPluginLogger _logger;

    public OpenAiDocumentGenerator(OpenAiSettings settings, IPluginLogger logger)
    {
        _settings = settings;
        _logger = logger;
        _apiClient = new ResponsesApiClient(settings, logger);
        _promptBuilder = new PromptBuilder();
        _validator = new ResponseValidator();
    }

    public OpenAiDocumentGenerator(OpenAiSettings settings, IPluginLogger logger, HttpClient httpClient)
    {
        _settings = settings;
        _logger = logger;
        _apiClient = new ResponsesApiClient(settings, logger, httpClient);
        _promptBuilder = new PromptBuilder();
        _validator = new ResponseValidator();
    }

    public async Task<GeneratedDocument> GenerateAsync(
        ModelContext context,
        DocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = _promptBuilder.BuildSystemPrompt();
        var userPrompt = _promptBuilder.BuildUserPrompt(context, request);

        var apiRequest = new ResponsesApiRequest
        {
            Model = _settings.Model,
            Instructions = systemPrompt,
            Input = new List<InputMessage>
            {
                new() { Role = "user", Content = userPrompt }
            },
            Text = new TextFormat
            {
                Format = new FormatSpec
                {
                    Type = "json_schema",
                    Name = "construction_document",
                    Strict = true,
                    Schema = TextFormatSchema.GetSchema()
                }
            },
            Temperature = _settings.Temperature,
            MaxOutputTokens = _settings.MaxOutputTokens,
            Store = false
        };

        for (int attempt = 1; attempt <= PluginConstants.MaxRetryAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.Info($"AI generation attempt {attempt}/{PluginConstants.MaxRetryAttempts}");

                var response = await _apiClient.SendAsync(apiRequest, cancellationToken);
                var outputText = response.GetOutputText();

                if (string.IsNullOrWhiteSpace(outputText))
                {
                    _logger.Warning($"Attempt {attempt}: Empty response from API.");
                    if (attempt == PluginConstants.MaxRetryAttempts)
                        throw new InvalidOperationException("AI returned empty response after all retry attempts.");
                    continue;
                }

                var aiResponse = JsonSerializer.Deserialize<AiDocumentResponse>(outputText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var (isValid, error) = _validator.Validate(aiResponse);
                if (!isValid)
                {
                    _logger.Warning($"Attempt {attempt}: Invalid response structure - {error}");
                    if (attempt == PluginConstants.MaxRetryAttempts)
                        throw new InvalidOperationException($"AI returned invalid response after all retry attempts: {error}");
                    continue;
                }

                return MapToGeneratedDocument(aiResponse!);
            }
            catch (JsonException ex) when (attempt < PluginConstants.MaxRetryAttempts)
            {
                _logger.Warning($"Attempt {attempt}: Malformed JSON response - {ex.Message}. Retrying...");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
            catch (JsonException ex)
            {
                _logger.Error($"Attempt {attempt}: Malformed JSON response after all retries - {ex.Message}");
                throw new InvalidOperationException($"AI returned malformed JSON after all retry attempts: {ex.Message}", ex);
            }
            catch (HttpRequestException ex) when (attempt < PluginConstants.MaxRetryAttempts)
            {
                _logger.Warning($"Attempt {attempt}: HTTP error - {ex.Message}. Retrying...");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
        }

        throw new InvalidOperationException("AI generation failed after all retry attempts.");
    }

    private static GeneratedDocument MapToGeneratedDocument(AiDocumentResponse response)
    {
        return new GeneratedDocument
        {
            Title = response.Title,
            DocumentType = response.DocumentType,
            GeneratedDate = response.GeneratedDate,
            Sections = response.Sections.Select(s => new DocumentSection
            {
                Heading = s.Heading,
                Content = s.Content,
                Order = s.Order
            }).ToList(),
            ProjectName = response.Metadata.ProjectName,
            Phase = response.Metadata.Phase,
            Audience = response.Metadata.Audience,
            DetailLevel = response.Metadata.DetailLevel,
            Warnings = response.Metadata.Warnings
        };
    }

    public void Dispose()
    {
        _apiClient.Dispose();
    }
}

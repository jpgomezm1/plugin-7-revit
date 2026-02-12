using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DocumentationGeneratorAI.AiClient.Configuration;
using DocumentationGeneratorAI.AiClient.Models;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.AiClient;

public sealed class ResponsesApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;
    private readonly IPluginLogger _logger;

    public ResponsesApiClient(OpenAiSettings settings, IPluginLogger logger)
        : this(settings, logger, new HttpClient())
    {
    }

    public ResponsesApiClient(OpenAiSettings settings, IPluginLogger logger, HttpClient httpClient)
    {
        _settings = settings;
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<ResponsesApiResponse> SendAsync(ResponsesApiRequest request, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        _logger.Debug($"Sending request to OpenAI Responses API. Model: {request.Model}");

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/v1/responses", content, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error($"OpenAI API error: {(int)response.StatusCode} - {responseBody}");
            throw new HttpRequestException(
                $"OpenAI API returned {(int)response.StatusCode}: {responseBody}",
                null,
                response.StatusCode);
        }

        var apiResponse = JsonSerializer.Deserialize<ResponsesApiResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return apiResponse ?? throw new InvalidOperationException("Failed to deserialize API response.");
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

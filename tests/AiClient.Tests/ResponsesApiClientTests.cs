using System.Net;
using DocumentationGeneratorAI.AiClient.Configuration;
using DocumentationGeneratorAI.AiClient.Models;
using DocumentationGeneratorAI.Shared.Logging;
using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace DocumentationGeneratorAI.AiClient.Tests;

public class ResponsesApiClientTests
{
    private readonly Mock<IPluginLogger> _mockLogger = new();
    private readonly OpenAiSettings _settings = new()
    {
        ApiKey = "test-key",
        Model = "gpt-4.1",
        BaseUrl = "https://api.openai.com/v1",
        TimeoutSeconds = 30
    };

    [Fact]
    public async Task SendAsync_SuccessfulResponse_ShouldReturnParsedResponse()
    {
        var mockHttp = new MockHttpMessageHandler();
        var responseJson = """
        {
            "id": "resp_123",
            "output": [
                {
                    "type": "message",
                    "content": [
                        {
                            "type": "output_text",
                            "text": "{\"title\":\"Test\"}"
                        }
                    ]
                }
            ]
        }
        """;

        mockHttp.When("https://api.openai.com/v1/responses")
            .Respond("application/json", responseJson);

        var httpClient = mockHttp.ToHttpClient();
        using var client = new ResponsesApiClient(_settings, _mockLogger.Object, httpClient);

        var request = new ResponsesApiRequest { Model = "gpt-4.1" };
        var result = await client.SendAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be("resp_123");
        result.GetOutputText().Should().Contain("Test");
    }

    [Fact]
    public async Task SendAsync_ErrorResponse_ShouldThrowHttpRequestException()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openai.com/v1/responses")
            .Respond(HttpStatusCode.Unauthorized, "application/json", "{\"error\":{\"message\":\"Invalid key\"}}");

        var httpClient = mockHttp.ToHttpClient();
        using var client = new ResponsesApiClient(_settings, _mockLogger.Object, httpClient);

        var request = new ResponsesApiRequest { Model = "gpt-4.1" };

        var act = () => client.SendAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public void GetOutputText_NoMessage_ShouldReturnNull()
    {
        var response = new ResponsesApiResponse
        {
            Output = new List<OutputItem>()
        };

        response.GetOutputText().Should().BeNull();
    }

    [Fact]
    public void GetOutputText_WithMessage_ShouldReturnText()
    {
        var response = new ResponsesApiResponse
        {
            Output = new List<OutputItem>
            {
                new()
                {
                    Type = "message",
                    Content = new List<ContentItem>
                    {
                        new() { Type = "output_text", Text = "hello world" }
                    }
                }
            }
        };

        response.GetOutputText().Should().Be("hello world");
    }
}

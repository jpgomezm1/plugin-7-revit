using DocumentationGeneratorAI.AiClient.Models;
using DocumentationGeneratorAI.AiClient.Validation;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.AiClient.Tests;

public class ResponseValidatorEdgeCaseTests
{
    private readonly ResponseValidator _validator = new();

    [Fact]
    public void Validate_EmptyProjectName_ShouldReturnInvalid()
    {
        var response = CreateValidResponse();
        response.Metadata.ProjectName = "";

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("projectName");
    }

    [Fact]
    public void Validate_EmptyPhase_ShouldReturnInvalid()
    {
        var response = CreateValidResponse();
        response.Metadata.Phase = "   ";

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("phase");
    }

    [Fact]
    public void Validate_EmptyAudience_ShouldReturnInvalid()
    {
        var response = CreateValidResponse();
        response.Metadata.Audience = "";

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("audience");
    }

    [Fact]
    public void Validate_EmptyDetailLevel_ShouldReturnInvalid()
    {
        var response = CreateValidResponse();
        response.Metadata.DetailLevel = "";

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("detailLevel");
    }

    [Fact]
    public void Validate_WhitespaceOnlyTitle_ShouldReturnInvalid()
    {
        var response = CreateValidResponse();
        response.Title = "   \t\n  ";

        var (isValid, _) = _validator.Validate(response);
        isValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_MultipleSections_AllValid_ShouldPass()
    {
        var response = CreateValidResponse();
        response.Sections = Enumerable.Range(1, 10).Select(i => new AiDocumentSection
        {
            Heading = $"Section {i}",
            Content = $"Content {i}",
            Order = i
        }).ToList();

        var (isValid, error) = _validator.Validate(response);
        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void Validate_OneInvalidSectionAmongMany_ShouldCatchIt()
    {
        var response = CreateValidResponse();
        response.Sections = new List<AiDocumentSection>
        {
            new() { Heading = "Valid 1", Content = "OK", Order = 1 },
            new() { Heading = "Valid 2", Content = "OK", Order = 2 },
            new() { Heading = "", Content = "Bad heading", Order = 3 },
            new() { Heading = "Valid 4", Content = "OK", Order = 4 }
        };

        var (isValid, error) = _validator.Validate(response);
        isValid.Should().BeFalse();
        error.Should().Contain("order 3");
    }

    [Fact]
    public void Validate_EmptyWarningsArray_ShouldBeValid()
    {
        var response = CreateValidResponse();
        response.Metadata.Warnings = new List<string>();

        var (isValid, _) = _validator.Validate(response);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WarningsWithContent_ShouldBeValid()
    {
        var response = CreateValidResponse();
        response.Metadata.Warnings = new List<string> { "Warning 1", "Warning 2" };

        var (isValid, _) = _validator.Validate(response);
        isValid.Should().BeTrue();
    }

    private static AiDocumentResponse CreateValidResponse()
    {
        return new AiDocumentResponse
        {
            Title = "Test Document",
            DocumentType = "Descriptive Report",
            GeneratedDate = "2025-01-01",
            Sections = new List<AiDocumentSection>
            {
                new() { Heading = "Overview", Content = "Some content", Order = 1 }
            },
            Metadata = new AiDocumentMetadata
            {
                ProjectName = "Test Project",
                Phase = "Design",
                Audience = "Client",
                DetailLevel = "Standard",
                Warnings = new List<string>()
            }
        };
    }
}

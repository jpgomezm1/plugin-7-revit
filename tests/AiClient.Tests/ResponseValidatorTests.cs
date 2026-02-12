using DocumentationGeneratorAI.AiClient.Models;
using DocumentationGeneratorAI.AiClient.Validation;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.AiClient.Tests;

public class ResponseValidatorTests
{
    private readonly ResponseValidator _validator = new();

    [Fact]
    public void Validate_NullResponse_ShouldReturnInvalid()
    {
        var (isValid, error) = _validator.Validate(null);

        isValid.Should().BeFalse();
        error.Should().Contain("null");
    }

    [Fact]
    public void Validate_EmptyTitle_ShouldReturnInvalid()
    {
        var response = new AiDocumentResponse { Title = "" };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("title");
    }

    [Fact]
    public void Validate_NoSections_ShouldReturnInvalid()
    {
        var response = new AiDocumentResponse
        {
            Title = "Test",
            DocumentType = "Report",
            Sections = new List<AiDocumentSection>()
        };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("no sections");
    }

    [Fact]
    public void Validate_EmptySectionHeading_ShouldReturnInvalid()
    {
        var response = new AiDocumentResponse
        {
            Title = "Test",
            DocumentType = "Report",
            Sections = new List<AiDocumentSection>
            {
                new() { Heading = "", Content = "Content", Order = 1 }
            },
            Metadata = new AiDocumentMetadata()
        };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("empty heading");
    }

    [Fact]
    public void Validate_EmptySectionContent_ShouldReturnInvalid()
    {
        var response = new AiDocumentResponse
        {
            Title = "Test",
            DocumentType = "Report",
            Sections = new List<AiDocumentSection>
            {
                new() { Heading = "Heading", Content = " ", Order = 1 }
            },
            Metadata = new AiDocumentMetadata()
        };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("empty content");
    }

    [Fact]
    public void Validate_ValidResponse_ShouldReturnValid()
    {
        var response = new AiDocumentResponse
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
                ProjectName = "Test",
                Phase = "Design",
                Audience = "Client",
                DetailLevel = "Standard"
            }
        };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void Validate_NullMetadata_ShouldReturnInvalid()
    {
        var response = new AiDocumentResponse
        {
            Title = "Test",
            DocumentType = "Report",
            Sections = new List<AiDocumentSection>
            {
                new() { Heading = "H", Content = "C", Order = 1 }
            },
            Metadata = null!
        };

        var (isValid, error) = _validator.Validate(response);

        isValid.Should().BeFalse();
        error.Should().Contain("metadata");
    }
}

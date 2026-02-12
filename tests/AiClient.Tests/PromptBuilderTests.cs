using DocumentationGeneratorAI.Core.Enums;
using DocumentationGeneratorAI.Core.Models;
using FluentAssertions;
using Xunit;

namespace DocumentationGeneratorAI.AiClient.Tests;

public class PromptBuilderTests
{
    private readonly PromptBuilder _builder = new();

    [Fact]
    public void BuildSystemPrompt_ShouldContainKeyInstructions()
    {
        var prompt = _builder.BuildSystemPrompt();

        prompt.Should().Contain("construction documentation writer");
        prompt.Should().Contain("STRICT RULES");
        prompt.Should().Contain("ModelContext");
        prompt.Should().Contain("Do not invent");
    }

    [Fact]
    public void BuildUserPrompt_ShouldContainDocumentType()
    {
        var context = CreateTestContext();
        var request = new DocumentRequest
        {
            DocumentType = DocumentType.DescriptiveReport,
            Phase = ProjectPhase.DetailedDesign,
            Audience = AudienceType.Client,
            DetailLevel = DetailLevel.Standard,
            IncludeQuantitiesSummary = true
        };

        var prompt = _builder.BuildUserPrompt(context, request);

        prompt.Should().Contain("Descriptive Report");
        prompt.Should().Contain("Detailed Design");
        prompt.Should().Contain("Client");
        prompt.Should().Contain("Standard");
        prompt.Should().Contain("Include Quantities Summary: Yes");
    }

    [Fact]
    public void BuildUserPrompt_ShouldContainModelContextJson()
    {
        var context = CreateTestContext();
        context.ProjectInfo.Name = "TestProject";

        var request = new DocumentRequest
        {
            DocumentType = DocumentType.TechnicalSpecification,
            Phase = ProjectPhase.Construction,
            Audience = AudienceType.Supervision,
            DetailLevel = DetailLevel.Extended
        };

        var prompt = _builder.BuildUserPrompt(context, request);

        prompt.Should().Contain("TestProject");
        prompt.Should().Contain("```json");
    }

    [Theory]
    [InlineData(DocumentType.DescriptiveReport, "Descriptive Report")]
    [InlineData(DocumentType.TechnicalSpecification, "Technical Specification")]
    [InlineData(DocumentType.ProgressReport, "Progress Report")]
    [InlineData(DocumentType.CoordinationReport, "Coordination Report")]
    [InlineData(DocumentType.FinalDeliveryDocument, "Final Delivery Document")]
    public void BuildUserPrompt_ShouldIncludeDocTypeSpecificInstructions(DocumentType type, string expectedText)
    {
        var context = CreateTestContext();
        var request = new DocumentRequest
        {
            DocumentType = type,
            Phase = ProjectPhase.DetailedDesign,
            Audience = AudienceType.Client,
            DetailLevel = DetailLevel.Standard
        };

        var prompt = _builder.BuildUserPrompt(context, request);

        prompt.Should().Contain(expectedText);
    }

    [Fact]
    public void BuildUserPrompt_CompanyTemplateNo_ShouldSayNo()
    {
        var context = CreateTestContext();
        var request = new DocumentRequest
        {
            DocumentType = DocumentType.DescriptiveReport,
            Phase = ProjectPhase.DetailedDesign,
            Audience = AudienceType.Client,
            DetailLevel = DetailLevel.Standard,
            UseCompanyTemplate = false
        };

        var prompt = _builder.BuildUserPrompt(context, request);
        prompt.Should().Contain("Apply Company Template Style: No");
    }

    private static ModelContext CreateTestContext()
    {
        return new ModelContext
        {
            ProjectInfo = new ProjectInfoDto { Name = "Test Project", Number = "001" },
            Levels = new List<LevelDto>
            {
                new() { Name = "Level 1", Elevation = 0 },
                new() { Name = "Level 2", Elevation = 3.5 }
            },
            CategoryCounts = new List<CategoryCountDto>
            {
                new() { CategoryName = "Walls", Count = 42 }
            }
        };
    }
}

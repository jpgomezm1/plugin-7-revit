using DocumentationGeneratorAI.Core.Enums;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Core.Orchestration;
using DocumentationGeneratorAI.Shared.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace DocumentationGeneratorAI.Core.Tests.Orchestration;

public class DocumentGenerationOrchestratorTests
{
    private readonly Mock<IAiDocumentGenerator> _mockGenerator;
    private readonly Mock<ITemplateProvider> _mockTemplateProvider;
    private readonly Mock<IPluginLogger> _mockLogger;
    private readonly DocumentGenerationOrchestrator _orchestrator;

    public DocumentGenerationOrchestratorTests()
    {
        _mockGenerator = new Mock<IAiDocumentGenerator>();
        _mockTemplateProvider = new Mock<ITemplateProvider>();
        _mockLogger = new Mock<IPluginLogger>();
        _orchestrator = new DocumentGenerationOrchestrator(
            _mockGenerator.Object,
            _mockTemplateProvider.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateDocumentAsync_ShouldCallAiGenerator()
    {
        var context = new ModelContext();
        var request = new DocumentRequest
        {
            DocumentType = DocumentType.DescriptiveReport,
            Phase = ProjectPhase.DetailedDesign,
            Audience = AudienceType.Client,
            DetailLevel = DetailLevel.Standard
        };
        var expected = new GeneratedDocument { Title = "Test Doc" };

        _mockGenerator
            .Setup(g => g.GenerateAsync(context, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _orchestrator.GenerateDocumentAsync(context, request);

        result.Should().Be(expected);
        result.Title.Should().Be("Test Doc");
        _mockGenerator.Verify(g => g.GenerateAsync(context, request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateDocumentAsync_ShouldLogWarnings()
    {
        var context = new ModelContext();
        context.Warnings.Add(new ExtractionWarning("W1", "Warning 1"));

        var request = new DocumentRequest
        {
            DocumentType = DocumentType.ProgressReport,
            Phase = ProjectPhase.Construction,
            Audience = AudienceType.Management,
            DetailLevel = DetailLevel.Short
        };

        _mockGenerator
            .Setup(g => g.GenerateAsync(context, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeneratedDocument());

        await _orchestrator.GenerateDocumentAsync(context, request);

        _mockLogger.Verify(l => l.Warning(It.Is<string>(s => s.Contains("1 extraction warning"))), Times.Once);
    }

    [Fact]
    public async Task GenerateDocumentAsync_WhenCancelled_ShouldThrowAndLog()
    {
        var context = new ModelContext();
        var request = new DocumentRequest
        {
            DocumentType = DocumentType.DescriptiveReport,
            Phase = ProjectPhase.DetailedDesign,
            Audience = AudienceType.Client,
            DetailLevel = DetailLevel.Standard
        };

        _mockGenerator
            .Setup(g => g.GenerateAsync(context, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _orchestrator.GenerateDocumentAsync(context, request));

        _mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("cancelled"))), Times.Once);
    }

    [Fact]
    public async Task GenerateDocumentAsync_WhenFails_ShouldThrowAndLogError()
    {
        var context = new ModelContext();
        var request = new DocumentRequest
        {
            DocumentType = DocumentType.TechnicalSpecification,
            Phase = ProjectPhase.Handover,
            Audience = AudienceType.PublicAuthority,
            DetailLevel = DetailLevel.Extended
        };

        _mockGenerator
            .Setup(g => g.GenerateAsync(context, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI error"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orchestrator.GenerateDocumentAsync(context, request));

        _mockLogger.Verify(l => l.Error(It.Is<string>(s => s.Contains("failed")), It.IsAny<Exception>()), Times.Once);
    }
}

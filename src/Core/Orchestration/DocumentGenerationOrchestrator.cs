using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.Core.Orchestration;

public sealed class DocumentGenerationOrchestrator
{
    private readonly IAiDocumentGenerator _aiGenerator;
    private readonly ITemplateProvider _templateProvider;
    private readonly IPluginLogger _logger;

    public DocumentGenerationOrchestrator(
        IAiDocumentGenerator aiGenerator,
        ITemplateProvider templateProvider,
        IPluginLogger logger)
    {
        _aiGenerator = aiGenerator;
        _templateProvider = templateProvider;
        _logger = logger;
    }

    public async Task<GeneratedDocument> GenerateDocumentAsync(
        ModelContext modelContext,
        DocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.Info($"Starting document generation: {request.DocumentType}, Phase: {request.Phase}, Audience: {request.Audience}");

        if (modelContext.Warnings.Count > 0)
        {
            _logger.Warning($"Model has {modelContext.Warnings.Count} extraction warning(s).");
        }

        try
        {
            var document = await _aiGenerator.GenerateAsync(modelContext, request, cancellationToken);
            _logger.Info($"Document generated successfully: {document.Title}");
            return document;
        }
        catch (OperationCanceledException)
        {
            _logger.Info("Document generation was cancelled by the user.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error("Document generation failed.", ex);
            throw;
        }
    }
}

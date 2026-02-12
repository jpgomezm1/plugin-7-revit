namespace DocumentationGeneratorAI.Core.Interfaces;

using DocumentationGeneratorAI.Core.Models;

public interface IAiDocumentGenerator
{
    Task<GeneratedDocument> GenerateAsync(ModelContext context, DocumentRequest request, CancellationToken cancellationToken = default);
}

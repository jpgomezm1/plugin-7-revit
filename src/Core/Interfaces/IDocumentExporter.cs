namespace DocumentationGeneratorAI.Core.Interfaces;

using DocumentationGeneratorAI.Core.Models;

public interface IDocumentExporter
{
    Task ExportAsync(GeneratedDocument document, string filePath, CancellationToken cancellationToken = default);
    string FileExtension { get; }
    string FileFilter { get; }
}

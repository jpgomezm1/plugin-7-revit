using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.DocExport;

public sealed class DocxExporter : IDocumentExporter
{
    public string FileExtension => ".docx";
    public string FileFilter => "Word documents (*.docx)|*.docx";

    public Task ExportAsync(GeneratedDocument document, string filePath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("DOCX export will be available in Phase 2.");
    }
}

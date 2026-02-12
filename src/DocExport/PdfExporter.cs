using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.DocExport;

public sealed class PdfExporter : IDocumentExporter
{
    public string FileExtension => ".pdf";
    public string FileFilter => "PDF files (*.pdf)|*.pdf";

    public Task ExportAsync(GeneratedDocument document, string filePath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("PDF export will be available in Phase 3.");
    }
}

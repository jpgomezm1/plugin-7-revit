using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.DocExport;

public sealed class MarkdownExporter : IDocumentExporter
{
    public string FileExtension => ".md";
    public string FileFilter => "Markdown files (*.md)|*.md";

    public async Task ExportAsync(GeneratedDocument document, string filePath, CancellationToken cancellationToken = default)
    {
        var markdown = document.ToMarkdown();
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, markdown, cancellationToken);
    }
}

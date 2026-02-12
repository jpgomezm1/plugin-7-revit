using System.Reflection;
using DocumentationGeneratorAI.Core.Enums;
using DocumentationGeneratorAI.Core.Interfaces;

namespace DocumentationGeneratorAI.DocTemplates;

public sealed class TemplateProvider : ITemplateProvider
{
    private readonly Dictionary<DocumentType, string> _templateCache = new();
    private readonly Assembly _assembly;

    public TemplateProvider()
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    public string GetTemplate(DocumentType documentType)
    {
        if (_templateCache.TryGetValue(documentType, out var cached))
            return cached;

        var resourceName = GetResourceName(documentType);
        var names = _assembly.GetManifestResourceNames();
        var match = names.FirstOrDefault(n => n.EndsWith(resourceName))
            ?? throw new InvalidOperationException($"Template not found for document type: {documentType}");

        using var stream = _assembly.GetManifestResourceStream(match)!;
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        _templateCache[documentType] = template;
        return template;
    }

    public bool HasTemplate(DocumentType documentType)
    {
        var resourceName = GetResourceName(documentType);
        return _assembly.GetManifestResourceNames().Any(n => n.EndsWith(resourceName));
    }

    private static string GetResourceName(DocumentType type) => type switch
    {
        DocumentType.DescriptiveReport => "DescriptiveReport.md",
        DocumentType.TechnicalSpecification => "TechnicalSpecification.md",
        DocumentType.ProgressReport => "ProgressReport.md",
        DocumentType.CoordinationReport => "CoordinationReport.md",
        DocumentType.FinalDeliveryDocument => "FinalDeliveryDocument.md",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}

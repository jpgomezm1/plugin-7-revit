using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class MaterialExtractor
{
    public static List<MaterialDto> Extract(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(Material))
            .Cast<Material>()
            .Where(m => !string.IsNullOrWhiteSpace(m.Name))
            .Take(PluginConstants.MaxListItems)
            .Select(m => new MaterialDto
            {
                Name = m.Name,
                MaterialClass = m.MaterialClass ?? string.Empty
            })
            .ToList();
    }
}

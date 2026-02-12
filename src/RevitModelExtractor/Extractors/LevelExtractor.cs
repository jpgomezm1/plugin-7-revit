using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class LevelExtractor
{
    public static List<LevelDto> Extract(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(l => l.Elevation)
            .Select(l => new LevelDto
            {
                Name = l.Name,
                Elevation = Math.Round(l.Elevation, 2)
            })
            .ToList();
    }
}

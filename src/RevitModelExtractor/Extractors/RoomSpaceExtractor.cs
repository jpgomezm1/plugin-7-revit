using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class RoomSpaceExtractor
{
    public static List<RoomDto> Extract(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType()
            .Cast<Room>()
            .Where(r => r.Area > 0)
            .Take(PluginConstants.MaxListItems)
            .Select(r => new RoomDto
            {
                Name = r.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? string.Empty,
                Number = r.Number ?? string.Empty,
                Level = r.Level?.Name ?? string.Empty,
                Area = Math.Round(r.Area, 2),
                Volume = Math.Round(r.Volume, 2)
            })
            .ToList();
    }
}

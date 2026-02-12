using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class QuantityExtractor
{
    public static QuantitySummaryDto Extract(Document doc, List<LevelDto> levels, List<RoomDto> rooms)
    {
        var totalFloorArea = GetTotalAreaForCategory(doc, BuiltInCategory.OST_Floors);
        var totalWallArea = GetTotalAreaForCategory(doc, BuiltInCategory.OST_Walls);

        return new QuantitySummaryDto
        {
            TotalFloorArea = Math.Round(totalFloorArea, 2),
            TotalWallArea = Math.Round(totalWallArea, 2),
            TotalRoomArea = Math.Round(rooms.Sum(r => r.Area), 2),
            TotalRoomVolume = Math.Round(rooms.Sum(r => r.Volume), 2),
            LevelCount = levels.Count,
            RoomCount = rooms.Count
        };
    }

    private static double GetTotalAreaForCategory(Document doc, BuiltInCategory category)
    {
        var elements = new FilteredElementCollector(doc)
            .OfCategory(category)
            .WhereElementIsNotElementType()
            .ToElements();

        double total = 0;
        foreach (var elem in elements)
        {
            var areaParam = elem.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
            if (areaParam != null)
                total += areaParam.AsDouble();
        }
        return total;
    }
}

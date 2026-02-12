using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class CategoryElementCounter
{
    private static readonly BuiltInCategory[] TrackedCategories = new[]
    {
        BuiltInCategory.OST_Walls,
        BuiltInCategory.OST_Floors,
        BuiltInCategory.OST_Roofs,
        BuiltInCategory.OST_Doors,
        BuiltInCategory.OST_Windows,
        BuiltInCategory.OST_StructuralColumns,
        BuiltInCategory.OST_StructuralFraming,
        BuiltInCategory.OST_StructuralFoundation,
        BuiltInCategory.OST_Ceilings,
        BuiltInCategory.OST_Stairs,
        BuiltInCategory.OST_Ramps,
        BuiltInCategory.OST_CurtainWallPanels,
        BuiltInCategory.OST_Railings,
        BuiltInCategory.OST_GenericModel,
        BuiltInCategory.OST_Furniture,
        BuiltInCategory.OST_Casework,
        BuiltInCategory.OST_PlumbingFixtures,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_ElectricalEquipment,
        BuiltInCategory.OST_ElectricalFixtures,
        BuiltInCategory.OST_DuctCurves,
        BuiltInCategory.OST_PipeCurves
    };

    public static List<CategoryCountDto> Extract(Document doc)
    {
        var results = new List<CategoryCountDto>();

        foreach (var cat in TrackedCategories)
        {
            var count = new FilteredElementCollector(doc)
                .OfCategory(cat)
                .WhereElementIsNotElementType()
                .GetElementCount();

            if (count > 0)
            {
                results.Add(new CategoryCountDto
                {
                    CategoryName = FormatCategoryName(cat),
                    Count = count
                });
            }
        }

        return results;
    }

    private static string FormatCategoryName(BuiltInCategory cat)
    {
        var name = cat.ToString();
        if (name.StartsWith("OST_"))
            name = name.Substring(4);

        // Insert spaces before capitals: "StructuralColumns" -> "Structural Columns"
        return System.Text.RegularExpressions.Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", " ");
    }
}

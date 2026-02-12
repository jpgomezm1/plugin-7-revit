using DocumentationGeneratorAI.Core.Models;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class WarningCollector
{
    public static List<ExtractionWarning> Collect(ModelContext context)
    {
        var warnings = new List<ExtractionWarning>();

        if (string.IsNullOrWhiteSpace(context.ProjectInfo.Name))
            warnings.Add(new ExtractionWarning("MISSING_PROJECT_NAME", "Project name is not set in Project Information."));

        if (context.Levels.Count == 0)
            warnings.Add(new ExtractionWarning("NO_LEVELS", "No levels found in the model."));

        if (context.Rooms.Count == 0)
            warnings.Add(new ExtractionWarning("NO_ROOMS", "No rooms with area > 0 found in the model."));

        if (context.QuantitySummary.TotalFloorArea == 0)
            warnings.Add(new ExtractionWarning("NO_FLOOR_AREA", "Total floor area is zero. No floor elements found or areas not computed."));

        if (context.MepSystems.Count == 0)
            warnings.Add(new ExtractionWarning("NO_MEP_SYSTEMS", "No MEP systems found in the model."));

        if (context.Materials.Count == 0)
            warnings.Add(new ExtractionWarning("NO_MATERIALS", "No materials found in the model."));

        if (context.CategoryCounts.Count == 0)
            warnings.Add(new ExtractionWarning("NO_ELEMENTS", "No categorized elements found in the model."));

        return warnings;
    }
}

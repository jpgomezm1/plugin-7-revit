using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.RevitModelExtractor.Extractors;

internal static class MepSystemExtractor
{
    public static List<MepSystemDto> Extract(Document doc)
    {
        var systems = new List<MepSystemDto>();

        // Mechanical systems
        var mechSystems = new FilteredElementCollector(doc)
            .OfClass(typeof(MechanicalSystem))
            .Cast<MechanicalSystem>()
            .Take(PluginConstants.MaxListItems);

        foreach (var sys in mechSystems)
        {
            systems.Add(new MepSystemDto
            {
                Name = sys.Name ?? string.Empty,
                SystemType = "Mechanical",
                ElementCount = sys.DuctNetwork?.Count ?? 0
            });
        }

        // Piping systems
        var pipeSystems = new FilteredElementCollector(doc)
            .OfClass(typeof(PipingSystem))
            .Cast<PipingSystem>()
            .Take(PluginConstants.MaxListItems);

        foreach (var sys in pipeSystems)
        {
            systems.Add(new MepSystemDto
            {
                Name = sys.Name ?? string.Empty,
                SystemType = "Piping",
                ElementCount = sys.PipingNetwork?.Count ?? 0
            });
        }

        // Electrical systems
        var elecSystems = new FilteredElementCollector(doc)
            .OfClass(typeof(ElectricalSystem))
            .Cast<ElectricalSystem>()
            .Take(PluginConstants.MaxListItems);

        foreach (var sys in elecSystems)
        {
            systems.Add(new MepSystemDto
            {
                Name = sys.Name ?? string.Empty,
                SystemType = "Electrical",
                ElementCount = sys.Elements?.Size ?? 0
            });
        }

        return systems;
    }
}

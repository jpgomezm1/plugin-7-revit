using Autodesk.Revit.DB;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.RevitModelExtractor.Extractors;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.RevitModelExtractor;

public sealed class RevitModelExtractorService : IModelExtractor
{
    private readonly IPluginLogger _logger;

    public RevitModelExtractorService(IPluginLogger logger)
    {
        _logger = logger;
    }

    public ModelContext Extract(object document)
    {
        if (document is not Document doc)
            throw new ArgumentException("Expected Autodesk.Revit.DB.Document", nameof(document));

        _logger.Info("Starting model extraction...");

        var context = new ModelContext();

        try
        {
            context.ProjectInfo = ProjectInfoExtractor.Extract(doc);
            _logger.Debug($"Extracted project info: {context.ProjectInfo.Name}");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract project info.", ex);
        }

        try
        {
            context.Levels = LevelExtractor.Extract(doc);
            _logger.Debug($"Extracted {context.Levels.Count} levels.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract levels.", ex);
        }

        try
        {
            context.CategoryCounts = CategoryElementCounter.Extract(doc);
            _logger.Debug($"Extracted {context.CategoryCounts.Count} category counts.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract category counts.", ex);
        }

        try
        {
            context.Materials = MaterialExtractor.Extract(doc);
            _logger.Debug($"Extracted {context.Materials.Count} materials.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract materials.", ex);
        }

        try
        {
            context.MepSystems = MepSystemExtractor.Extract(doc);
            _logger.Debug($"Extracted {context.MepSystems.Count} MEP systems.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract MEP systems.", ex);
        }

        try
        {
            context.Rooms = RoomSpaceExtractor.Extract(doc);
            _logger.Debug($"Extracted {context.Rooms.Count} rooms.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract rooms.", ex);
        }

        try
        {
            context.QuantitySummary = QuantityExtractor.Extract(doc, context.Levels, context.Rooms);
            _logger.Debug("Extracted quantity summary.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to extract quantities.", ex);
        }

        context.Warnings = WarningCollector.Collect(context);
        context.ExtractionTimestamp = DateTime.UtcNow;

        _logger.Info($"Model extraction complete. {context.Warnings.Count} warning(s).");
        return context;
    }
}

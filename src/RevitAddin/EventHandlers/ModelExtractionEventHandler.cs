using Autodesk.Revit.UI;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.RevitAddin.EventHandlers;

public sealed class ModelExtractionEventHandler : IExternalEventHandler
{
    private readonly IModelExtractor _extractor;
    private readonly IPluginLogger _logger;
    private TaskCompletionSource<ModelContext>? _tcs;

    public ModelExtractionEventHandler(IModelExtractor extractor, IPluginLogger logger)
    {
        _extractor = extractor;
        _logger = logger;
    }

    public void SetCompletionSource(TaskCompletionSource<ModelContext> tcs)
    {
        _tcs = tcs;
    }

    public void Execute(UIApplication app)
    {
        try
        {
            _logger.Info("ExternalEventHandler: Executing model extraction in Revit API context.");
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
            {
                _tcs?.TrySetException(new InvalidOperationException("No active document."));
                return;
            }

            var context = _extractor.Extract(doc);
            _tcs?.TrySetResult(context);
        }
        catch (Exception ex)
        {
            _logger.Error("ExternalEventHandler: Extraction failed.", ex);
            _tcs?.TrySetException(ex);
        }
    }

    public string GetName() => "DocumentationGeneratorAI.ModelExtraction";
}

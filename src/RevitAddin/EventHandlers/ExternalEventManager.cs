using Autodesk.Revit.UI;
using DocumentationGeneratorAI.Core.Models;
using DocumentationGeneratorAI.Core.Testing;
using DocumentationGeneratorAI.RevitModelExtractor;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.RevitAddin.EventHandlers;

public sealed class ExternalEventManager : IDisposable
{
    private readonly ModelExtractionEventHandler _handler;
    private readonly ExternalEvent _externalEvent;
    private readonly IPluginLogger _logger;
    private bool _isDemoMode;

    public ExternalEventManager(IPluginLogger logger)
    {
        _logger = logger;
        var extractor = new RevitModelExtractorService(logger);
        _handler = new ModelExtractionEventHandler(extractor, logger);
        _externalEvent = ExternalEvent.Create(_handler);
    }

    public void EnableDemoMode()
    {
        _isDemoMode = true;
        _logger.Info("ExternalEventManager: Demo mode enabled — will return sample ModelContext.");
    }

    public Task<ModelContext> RequestExtractionAsync()
    {
        if (_isDemoMode)
        {
            _logger.Info("Demo mode: Returning sample ModelContext (no Revit API call).");
            return Task.FromResult(DemoModelContextFactory.Create());
        }

        var tcs = new TaskCompletionSource<ModelContext>();
        _handler.SetCompletionSource(tcs);

        _logger.Info("Raising external event for model extraction.");
        var result = _externalEvent.Raise();

        if (result != ExternalEventRequest.Accepted)
        {
            tcs.TrySetException(new InvalidOperationException($"External event was not accepted: {result}"));
        }

        return tcs.Task;
    }

    public void Dispose()
    {
        _externalEvent.Dispose();
    }
}

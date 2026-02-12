using DocumentationGeneratorAI.AiClient;
using DocumentationGeneratorAI.AiClient.Configuration;
using DocumentationGeneratorAI.Core.Interfaces;
using DocumentationGeneratorAI.Core.Orchestration;
using DocumentationGeneratorAI.DocExport;
using DocumentationGeneratorAI.DocTemplates;
using DocumentationGeneratorAI.RevitAddin.EventHandlers;
using DocumentationGeneratorAI.Shared.Configuration;
using DocumentationGeneratorAI.Shared.Logging;

namespace DocumentationGeneratorAI.RevitAddin.Services;

public static class ServiceLocator
{
    public static ExternalEventManager ExternalEventManager { get; private set; } = null!;
    public static DocumentGenerationOrchestrator Orchestrator { get; private set; } = null!;
    public static IDocumentExporter MarkdownExporter { get; private set; } = null!;
    public static IPluginLogger Logger { get; private set; } = null!;

    public static void Initialize(ExternalEventManager eventManager, IPluginLogger logger)
    {
        ExternalEventManager = eventManager;
        Logger = logger;

        var settings = new OpenAiSettings
        {
            ApiKey = EnvironmentConfig.IsApiKeyConfigured() ? EnvironmentConfig.ApiKey : string.Empty,
            Model = EnvironmentConfig.ModelName,
            Temperature = EnvironmentConfig.Temperature,
            MaxOutputTokens = EnvironmentConfig.MaxOutputTokens,
            TimeoutSeconds = EnvironmentConfig.TimeoutSeconds
        };

        var aiGenerator = new OpenAiDocumentGenerator(settings, logger);
        var templateProvider = new TemplateProvider();

        Orchestrator = new DocumentGenerationOrchestrator(aiGenerator, templateProvider, logger);
        MarkdownExporter = new MarkdownExporter();

        logger.Info("ServiceLocator initialized.");
    }
}

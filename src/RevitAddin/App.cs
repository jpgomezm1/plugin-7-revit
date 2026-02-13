using Autodesk.Revit.UI;
using DocumentationGeneratorAI.RevitAddin.EventHandlers;
using DocumentationGeneratorAI.RevitAddin.Services;
using DocumentationGeneratorAI.Shared.Configuration;
using DocumentationGeneratorAI.Shared.Logging;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace DocumentationGeneratorAI.RevitAddin;

public class App : IExternalApplication
{
    private ExternalEventManager? _externalEventManager;

    public Result OnStartup(UIControlledApplication application)
    {
        try
        {
            var logger = PluginLogger.Instance;
            logger.Info("Documentation Generator AI starting up...");

            // Create ribbon tab
            try
            {
                application.CreateRibbonTab(PluginConstants.TabName);
            }
            catch
            {
                // Tab may already exist from another addin
            }

            // Create panel
            var panel = application.CreateRibbonPanel(PluginConstants.TabName, PluginConstants.PanelName);

            // Create button
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var buttonData = new PushButtonData(
                "GenerateDocumentation",
                PluginConstants.ButtonName,
                assemblyPath,
                typeof(Commands.GenerateDocumentationCommand).FullName)
            {
                ToolTip = PluginConstants.ButtonDescription
            };

            // Try to set icons from embedded resources
            try
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                buttonData.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/{assemblyName};component/Resources/icon-32.png"));
                buttonData.Image = new BitmapImage(new Uri($"pack://application:,,,/{assemblyName};component/Resources/icon-16.png"));
            }
            catch
            {
                // Icons are optional
            }

            panel.AddItem(buttonData);

            // Initialize external event manager
            _externalEventManager = new ExternalEventManager(logger);

            // Initialize service locator
            ServiceLocator.Initialize(_externalEventManager, logger);

            logger.Info("Documentation Generator AI started successfully.");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Documentation Generator AI", $"Failed to start: {ex.Message}");
            return Result.Failed;
        }
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        try
        {
            ServiceLocator.Cleanup();
            _externalEventManager?.Dispose();
            PluginLogger.Instance.Info("Documentation Generator AI shut down.");
        }
        catch
        {
            // Ignore shutdown errors
        }
        return Result.Succeeded;
    }
}

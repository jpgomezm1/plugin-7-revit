using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DocumentationGeneratorAI.RevitAddin.Services;
using DocumentationGeneratorAI.RevitAddin.ViewModels;
using DocumentationGeneratorAI.RevitAddin.Views;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.RevitAddin.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class GenerateDocumentationCommand : IExternalCommand
{
    private static System.Windows.Window? _activeWindow;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            // If a window is already open, bring it to front
            if (_activeWindow is not null)
            {
                _activeWindow.Activate();
                return Result.Succeeded;
            }

            // Check API key
            if (!EnvironmentConfig.IsApiKeyConfigured())
            {
                TaskDialog.Show(PluginConstants.ApplicationName,
                    "OPENAI_API_KEY environment variable is not set.\n\nPlease set it and restart Revit.\n\nPowerShell:\n[System.Environment]::SetEnvironmentVariable(\"OPENAI_API_KEY\", \"sk-your-key\", \"User\")");
                return Result.Failed;
            }

            // Check active document
            var uiDoc = commandData.Application.ActiveUIDocument;
            if (uiDoc == null)
            {
                TaskDialog.Show(PluginConstants.ApplicationName, "No Revit document is currently open.");
                return Result.Failed;
            }

            // Create ViewModel and Window
            var viewModel = new MainViewModel(
                ServiceLocator.Orchestrator,
                ServiceLocator.ExternalEventManager,
                ServiceLocator.MarkdownExporter,
                ServiceLocator.Logger);

            var window = new MainWindow
            {
                DataContext = viewModel
            };

            window.Closed += (_, _) => _activeWindow = null;
            _activeWindow = window;
            window.Show(); // Modeless — does not block Revit

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            TaskDialog.Show(PluginConstants.ApplicationName, $"Error: {ex.Message}");
            return Result.Failed;
        }
    }
}

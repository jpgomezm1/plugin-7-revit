using System;
using System.IO;

namespace DocumentationGeneratorAI.Shared.Configuration;

/// <summary>
/// Application-wide constants for the Documentation Generator AI plugin.
/// </summary>
public static class PluginConstants
{
    // Revit ribbon UI
    public const string TabName = "Irrelevant";
    public const string PanelName = "Documentation";
    public const string ButtonName = "Generate Documentation";
    public const string ButtonDescription =
        "Generate AI-powered construction documentation from Revit model data";

    // Logging
    public static string LogDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DocumentationGeneratorAI",
            "logs");

    public const string LogFileName = "plugin.log";

    // Limits
    public const int MaxListItems = 200;
    public const int MaxJsonSizeBytes = 30 * 1024;
    public const int MaxRetryAttempts = 3;

    // Application
    public const string ApplicationName = "Documentation Generator AI";
}

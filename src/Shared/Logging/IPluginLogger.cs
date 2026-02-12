using System;

namespace DocumentationGeneratorAI.Shared.Logging;

/// <summary>
/// Abstraction for plugin logging.
/// </summary>
public interface IPluginLogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? ex = null);
    void Debug(string message);
}

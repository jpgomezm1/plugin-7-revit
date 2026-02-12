using System;
using System.IO;
using DocumentationGeneratorAI.Shared.Configuration;

namespace DocumentationGeneratorAI.Shared.Logging;

/// <summary>
/// Thread-safe file logger that writes to the plugin log directory.
/// Implements a lazy singleton via <see cref="Instance"/>.
/// </summary>
public sealed class PluginLogger : IPluginLogger, IDisposable
{
    private static readonly Lazy<PluginLogger> _lazy =
        new(() => new PluginLogger());

    /// <summary>
    /// Gets the singleton logger instance.
    /// </summary>
    public static PluginLogger Instance => _lazy.Value;

    private readonly object _lock = new();
    private readonly StreamWriter _writer;
    private bool _disposed;

    private PluginLogger()
    {
        string logDirectory = PluginConstants.LogDirectory;
        Directory.CreateDirectory(logDirectory);

        string logPath = Path.Combine(logDirectory, PluginConstants.LogFileName);
        _writer = new StreamWriter(logPath, append: true) { AutoFlush = true };
    }

    public void Info(string message) => Write("INFO", message);

    public void Warning(string message) => Write("WARNING", message);

    public void Error(string message, Exception? ex = null)
    {
        if (ex is not null)
        {
            Write("ERROR", $"{message}{Environment.NewLine}{ex}");
        }
        else
        {
            Write("ERROR", message);
        }
    }

    public void Debug(string message) => Write("DEBUG", message);

    private void Write(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string entry = $"[{timestamp}] [{level}] {message}";

        lock (_lock)
        {
            if (!_disposed)
            {
                _writer.WriteLine(entry);
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (!_disposed)
            {
                _disposed = true;
                _writer.Dispose();
            }
        }
    }
}

using System;

namespace DocumentationGeneratorAI.Shared.Configuration;

/// <summary>
/// Reads plugin configuration from environment variables.
/// </summary>
public static class EnvironmentConfig
{
    private const string DefaultModel = "gpt-4.1";
    private const double DefaultTemperature = 0.3;
    private const int DefaultMaxOutputTokens = 16000;
    private const int DefaultTimeoutSeconds = 120;

    /// <summary>
    /// Gets the OpenAI API key from the OPENAI_API_KEY environment variable.
    /// Throws <see cref="InvalidOperationException"/> if the variable is not set.
    /// </summary>
    public static string ApiKey
    {
        get
        {
            string? key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "The OPENAI_API_KEY environment variable is not set. " +
                    "Please set it to your OpenAI API key before using this plugin.");
            }
            return key;
        }
    }

    /// <summary>
    /// Gets the model name from the DOCGEN_MODEL environment variable.
    /// Defaults to "gpt-4.1".
    /// </summary>
    public static string ModelName =>
        Environment.GetEnvironmentVariable("DOCGEN_MODEL") is { Length: > 0 } model
            ? model
            : DefaultModel;

    /// <summary>
    /// Gets the temperature from the DOCGEN_TEMPERATURE environment variable.
    /// Defaults to 0.3.
    /// </summary>
    public static double Temperature =>
        double.TryParse(Environment.GetEnvironmentVariable("DOCGEN_TEMPERATURE"), out double temp)
            ? temp
            : DefaultTemperature;

    /// <summary>
    /// Gets the max output tokens from the DOCGEN_MAX_TOKENS environment variable.
    /// Defaults to 16000.
    /// </summary>
    public static int MaxOutputTokens =>
        int.TryParse(Environment.GetEnvironmentVariable("DOCGEN_MAX_TOKENS"), out int tokens)
            ? tokens
            : DefaultMaxOutputTokens;

    /// <summary>
    /// Gets the timeout in seconds from the DOCGEN_TIMEOUT_SECONDS environment variable.
    /// Defaults to 120.
    /// </summary>
    public static int TimeoutSeconds =>
        int.TryParse(Environment.GetEnvironmentVariable("DOCGEN_TIMEOUT_SECONDS"), out int timeout)
            ? timeout
            : DefaultTimeoutSeconds;

    /// <summary>
    /// Checks whether the OPENAI_API_KEY environment variable is configured without throwing.
    /// </summary>
    /// <returns>True if the API key is set and non-empty; otherwise false.</returns>
    public static bool IsApiKeyConfigured()
    {
        string? key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        return !string.IsNullOrWhiteSpace(key);
    }
}

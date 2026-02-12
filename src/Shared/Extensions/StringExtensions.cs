using System;
using System.Globalization;

namespace DocumentationGeneratorAI.Shared.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates the string to <paramref name="maxLength"/> characters, appending "..." if truncated.
    /// </summary>
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (maxLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Maximum length must be non-negative.");

        if (value.Length <= maxLength)
            return value;

        if (maxLength <= 3)
            return value[..maxLength];

        return string.Concat(value.AsSpan(0, maxLength - 3), "...");
    }

    /// <summary>
    /// Converts the string to title case using the current culture.
    /// </summary>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(value.ToLower(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Returns true if the string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}

using System.Diagnostics.CodeAnalysis;

namespace GamaLearn.Extensions;

/// <summary>
/// Extension methods for <see cref="Color"/> manipulation and transformation.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Lightens the color by the specified percentage.
    /// </summary>
    /// <param name="color">The color to lighten.</param>
    /// <param name="amount">The amount to lighten (0.0 to 1.0). Default is 0.2 (20%).</param>
    /// <returns>A new lightened color.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when amount is not between 0 and 1.</exception>
    public static Color Lighten(this Color color, float amount = 0.2f)
    {
        ArgumentNullException.ThrowIfNull(color);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, 1f);

        float r = Math.Min(1f, color.Red + (1f - color.Red) * amount);
        float g = Math.Min(1f, color.Green + (1f - color.Green) * amount);
        float b = Math.Min(1f, color.Blue + (1f - color.Blue) * amount);

        return new Color(r, g, b, color.Alpha);
    }

    /// <summary>
    /// Darkens the color by the specified percentage.
    /// </summary>
    /// <param name="color">The color to darken.</param>
    /// <param name="amount">The amount to darken (0.0 to 1.0). Default is 0.2 (20%).</param>
    /// <returns>A new darkened color.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when amount is not between 0 and 1.</exception>
    public static Color Darken(this Color color, float amount = 0.2f)
    {
        ArgumentNullException.ThrowIfNull(color);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, 1f);

        float r = Math.Max(0f, color.Red * (1f - amount));
        float g = Math.Max(0f, color.Green * (1f - amount));
        float b = Math.Max(0f, color.Blue * (1f - amount));

        return new Color(r, g, b, color.Alpha);
    }

    /// <summary>
    /// Returns a new color with the specified alpha (opacity) value.
    /// </summary>
    /// <param name="color">The base color.</param>
    /// <param name="alpha">The alpha value (0.0 to 1.0). 0 is fully transparent, 1 is fully opaque.</param>
    /// <returns>A new color with the specified alpha.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when alpha is not between 0 and 1.</exception>
    public static Color WithAlpha(this Color color, float alpha)
    {
        ArgumentNullException.ThrowIfNull(color);
        ArgumentOutOfRangeException.ThrowIfLessThan(alpha, 0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(alpha, 1f);

        return new Color(color.Red, color.Green, color.Blue, alpha);
    }

    /// <summary>
    /// Returns a new color with the alpha value multiplied by the specified factor.
    /// </summary>
    /// <param name="color">The base color.</param>
    /// <param name="factor">The factor to multiply alpha by (0.0 to 1.0).</param>
    /// <returns>A new color with adjusted alpha.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when factor is not between 0 and 1.</exception>
    public static Color MultiplyAlpha(this Color color, float factor)
    {
        ArgumentNullException.ThrowIfNull(color);
        ArgumentOutOfRangeException.ThrowIfLessThan(factor, 0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, 1f);

        float newAlpha = Math.Clamp(color.Alpha * factor, 0f, 1f);
        return new Color(color.Red, color.Green, color.Blue, newAlpha);
    }

    /// <summary>
    /// Converts the color to grayscale using the luminosity method.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A grayscale version of the color.</returns>
    public static Color ToGrayscale(this Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        // Use luminosity method: 0.299R + 0.587G + 0.114B
        float gray = (color.Red * 0.299f) + (color.Green * 0.587f) + (color.Blue * 0.114f);
        return new Color(gray, gray, gray, color.Alpha);
    }

    /// <summary>
    /// Inverts the color (creates a negative).
    /// </summary>
    /// <param name="color">The color to invert.</param>
    /// <returns>The inverted color.</returns>
    public static Color Invert(this Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        return new Color(1f - color.Red, 1f - color.Green, 1f - color.Blue, color.Alpha);
    }

    /// <summary>
    /// Gets the luminance (perceived brightness) of the color.
    /// </summary>
    /// <param name="color">The color to analyze.</param>
    /// <returns>Luminance value between 0 (black) and 1 (white).</returns>
    public static float GetLuminance(this Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        // Use relative luminance formula: 0.2126R + 0.7152G + 0.0722B
        return (color.Red * 0.2126f) + (color.Green * 0.7152f) + (color.Blue * 0.0722f);
    }

    /// <summary>
    /// Determines if the color is considered light (luminance > 0.5).
    /// Useful for determining whether to use dark or light text on this background color.
    /// </summary>
    /// <param name="color">The color to check.</param>
    /// <returns>True if the color is light, false if dark.</returns>
    public static bool IsLight(this Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        return GetLuminance(color) > 0.5f;
    }

    /// <summary>
    /// Determines if the color is considered dark (luminance less than or equal to 0.5).
    /// Useful for determining whether to use dark or light text on this background color.
    /// </summary>
    /// <param name="color">The color to check.</param>
    /// <returns>True if the color is dark, false if light.</returns>
    public static bool IsDark(this Color color)
    {
        ArgumentNullException.ThrowIfNull(color);

        return GetLuminance(color) <= 0.5f;
    }

    /// <summary>
    /// Gets a contrasting color (black or white) that will be readable on this background color.
    /// </summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <returns>Black if the background is light, white if the background is dark.</returns>
    public static Color GetContrastingTextColor(this Color backgroundColor)
    {
        ArgumentNullException.ThrowIfNull(backgroundColor);

        return backgroundColor.IsLight() ? Colors.Black : Colors.White;
    }

    /// <summary>
    /// Blends two colors together using linear interpolation.
    /// </summary>
    /// <param name="color1">The first color.</param>
    /// <param name="color2">The second color.</param>
    /// <param name="amount">The blend amount (0.0 = color1, 1.0 = color2).</param>
    /// <returns>The blended color.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when amount is not between 0 and 1.</exception>
    public static Color Blend(this Color color1, Color color2, float amount = 0.5f)
    {
        ArgumentNullException.ThrowIfNull(color1);
        ArgumentNullException.ThrowIfNull(color2);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0f);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, 1f);

        float r = color1.Red + ((color2.Red - color1.Red) * amount);
        float g = color1.Green + ((color2.Green - color1.Green) * amount);
        float b = color1.Blue + ((color2.Blue - color1.Blue) * amount);
        float a = color1.Alpha + ((color2.Alpha - color1.Alpha) * amount);

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Converts the color to a hex string representation.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <param name="includeAlpha">Whether to include the alpha channel in the output.</param>
    /// <returns>A hex string in the format #RRGGBB or #AARRGGBB.</returns>
    public static string ToHex(this Color color, bool includeAlpha = false)
    {
        ArgumentNullException.ThrowIfNull(color);

        int r = (int)(color.Red * 255);
        int g = (int)(color.Green * 255);
        int b = (int)(color.Blue * 255);
        int a = (int)(color.Alpha * 255);

        return includeAlpha
            ? $"#{a:X2}{r:X2}{g:X2}{b:X2}"
            : $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Parses a hex color string to a Color object.
    /// Supports formats: #RGB, #ARGB, #RRGGBB, #AARRGGBB.
    /// </summary>
    /// <param name="hex">The hex string to parse.</param>
    /// <returns>The parsed color.</returns>
    /// <exception cref="ArgumentException">Thrown when the hex string is invalid.</exception>
    public static Color FromHex([NotNull] string hex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hex);

        hex = hex.Trim();

        if (!hex.StartsWith('#'))
        {
            throw new ArgumentException("Hex string must start with #", nameof(hex));
        }

        hex = hex[1..]; // Remove the #

        return hex.Length switch
        {
            3 => ParseRgb(hex),           // #RGB
            4 => ParseArgb(hex),          // #ARGB
            6 => ParseRrGgBb(hex),        // #RRGGBB
            8 => ParseAaRrGgBb(hex),      // #AARRGGBB
            _ => throw new ArgumentException($"Invalid hex color format: #{hex}", nameof(hex))
        };
    }

    /// <summary>
    /// Tries to parse a hex color string to a Color object.
    /// </summary>
    /// <param name="hex">The hex string to parse.</param>
    /// <param name="color">The parsed color, or null if parsing failed.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryFromHex(string? hex, [NotNullWhen(true)] out Color? color)
    {
        color = null;

        if (string.IsNullOrWhiteSpace(hex))
        {
            return false;
        }

        try
        {
            color = FromHex(hex);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #region Private Helper Methods
    private static Color ParseRgb(string hex)
    {
        // #RGB format
        int r = Convert.ToInt32(new string(hex[0], 2), 16);
        int g = Convert.ToInt32(new string(hex[1], 2), 16);
        int b = Convert.ToInt32(new string(hex[2], 2), 16);

        return Color.FromRgb(r, g, b);
    }

    private static Color ParseArgb(string hex)
    {
        // #ARGB format
        int a = Convert.ToInt32(new string(hex[0], 2), 16);
        int r = Convert.ToInt32(new string(hex[1], 2), 16);
        int g = Convert.ToInt32(new string(hex[2], 2), 16);
        int b = Convert.ToInt32(new string(hex[3], 2), 16);

        return Color.FromRgba(r, g, b, a);
    }

    private static Color ParseRrGgBb(string hex)
    {
        // #RRGGBB format
        int r = Convert.ToInt32(hex.Substring(0, 2), 16);
        int g = Convert.ToInt32(hex.Substring(2, 2), 16);
        int b = Convert.ToInt32(hex.Substring(4, 2), 16);

        return Color.FromRgb(r, g, b);
    }

    private static Color ParseAaRrGgBb(string hex)
    {
        // #AARRGGBB format
        int a = Convert.ToInt32(hex.Substring(0, 2), 16);
        int r = Convert.ToInt32(hex.Substring(2, 2), 16);
        int g = Convert.ToInt32(hex.Substring(4, 2), 16);
        int b = Convert.ToInt32(hex.Substring(6, 2), 16);

        return Color.FromRgba(r, g, b, a);
    }
    #endregion
}

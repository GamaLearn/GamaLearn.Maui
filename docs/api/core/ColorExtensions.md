# ColorExtensions

Extension methods for manipulating and transforming colors in .NET MAUI applications.

**Namespace:** `GamaLearn.Extensions`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

ColorExtensions provides a comprehensive set of extension methods for the `Color` class, enabling common color operations like lightening, darkening, adjusting opacity, blending, and more. All methods are optimized for performance and maintain the original color's characteristics where appropriate.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Methods

### Lighten(float amount = 0.2f)

Lightens the color by moving it toward white.

**Parameters:**
- `amount` - Amount to lighten (0.0 to 1.0). Default is 0.2 (20%)

**Returns:** A new lightened color

```csharp
Color blue = Colors.Blue;
Color lighterBlue = blue.Lighten(0.3f); // 30% lighter
```

### Darken(float amount = 0.2f)

Darkens the color by moving it toward black.

**Parameters:**
- `amount` - Amount to darken (0.0 to 1.0). Default is 0.2 (20%)

**Returns:** A new darkened color

```csharp
Color red = Colors.Red;
Color darkerRed = red.Darken(0.4f); // 40% darker
```

### WithAlpha(float alpha)

Returns a new color with the specified alpha (opacity) value.

**Parameters:**
- `alpha` - Alpha value (0.0 to 1.0). 0 is fully transparent, 1 is fully opaque

**Returns:** A new color with the specified alpha

```csharp
Color semiTransparentRed = Colors.Red.WithAlpha(0.5f); // 50% opacity
```

### MultiplyAlpha(float factor)

Returns a new color with the alpha value multiplied by the specified factor.

**Parameters:**
- `factor` - Factor to multiply alpha by (0.0 to 1.0)

**Returns:** A new color with adjusted alpha

```csharp
Color color = Colors.Red.WithAlpha(0.8f);
Color faded = color.MultiplyAlpha(0.5f); // Alpha becomes 0.4 (0.8 * 0.5)
```

### ToGrayscale()

Converts the color to grayscale using the luminosity method.

**Returns:** A grayscale version of the color

```csharp
Color gray = Colors.Blue.ToGrayscale();
```

### Invert()

Inverts the color (creates a negative).

**Returns:** The inverted color

```csharp
Color inverted = Colors.Blue.Invert();
```

### GetLuminance()

Gets the perceived brightness of the color.

**Returns:** Luminance value between 0 (black) and 1 (white)

```csharp
float luminance = Colors.Blue.GetLuminance(); // ~0.2126
```

### IsLight()

Determines if the color is considered light (luminance > 0.5).

**Returns:** True if the color is light, false if dark

```csharp
if (backgroundColor.IsLight())
{
    // Use dark text
}
```

### IsDark()

Determines if the color is considered dark (luminance <= 0.5).

**Returns:** True if the color is dark, false if light

```csharp
if (backgroundColor.IsDark())
{
    // Use light text
}
```

### GetContrastingTextColor()

Gets a contrasting color (black or white) for readable text.

**Returns:** Black if background is light, white if background is dark

```csharp
Color textColor = backgroundColor.GetContrastingTextColor();
```

### Blend(Color color2, float amount = 0.5f)

Blends two colors using linear interpolation.

**Parameters:**
- `color2` - The second color to blend with
- `amount` - Blend amount (0.0 = color1, 1.0 = color2)

**Returns:** The blended color

```csharp
Color purple = Colors.Red.Blend(Colors.Blue, 0.5f); // 50/50 blend
```

### ToHex(bool includeAlpha = false)

Converts the color to a hex string representation.

**Parameters:**
- `includeAlpha` - Whether to include the alpha channel

**Returns:** A hex string in format #RRGGBB or #AARRGGBB

```csharp
string hex = Colors.Red.ToHex(); // "#FF0000"
string hexWithAlpha = Colors.Red.ToHex(includeAlpha: true); // "#FFFF0000"
```

### FromHex(string hex)

Parses a hex color string to a Color object.

**Parameters:**
- `hex` - Hex string (#RGB, #ARGB, #RRGGBB, or #AARRGGBB)

**Returns:** The parsed color

```csharp
Color red = ColorExtensions.FromHex("#FF0000");
Color blue = ColorExtensions.FromHex("#00F");
Color semiRed = ColorExtensions.FromHex("#80FF0000");
```

### TryFromHex(string? hex, out Color? color)

Tries to parse a hex color string.

**Parameters:**
- `hex` - Hex string to parse
- `color` - The parsed color, or null if parsing failed

**Returns:** True if parsing succeeded

```csharp
if (ColorExtensions.TryFromHex(userInput, out Color? color) && color is not null)
{
    // Use the color
}
```

---

## Usage Examples

### Theme-Adaptive Colors

```csharp
public class ThemeService
{
    public Color GetButtonColor(Color baseColor, bool isDarkMode)
    {
        if (isDarkMode)
        {
            return baseColor.Lighten(0.2f);
        }
        else
        {
            return baseColor.Darken(0.1f);
        }
    }

    public Color GetTextColor(Color backgroundColor)
    {
        // Automatically choose black or white text
        return backgroundColor.GetContrastingTextColor();
    }
}
```

### Hover Effects

```csharp
public class ButtonViewModel : ObservableObject
{
    public Color NormalColor { get; } = Colors.Blue;
    public Color HoverColor => NormalColor.Lighten(0.15f);
    public Color PressedColor => NormalColor.Darken(0.1f);
    public Color DisabledColor => NormalColor.MultiplyAlpha(0.5f);
}
```

### Color Transitions

```csharp
public async Task AnimateColorTransitionAsync(View view, Color fromColor, Color toColor)
{
    const int steps = 30;
    const int delayMs = 16; // ~60 FPS

    for (int i = 0; i <= steps; i++)
    {
        float progress = i / (float)steps;
        Color currentColor = fromColor.Blend(toColor, progress);
        view.BackgroundColor = currentColor;

        await Task.Delay(delayMs);
    }
}
```

### Dynamic Color Palette

```csharp
public class ColorPaletteGenerator
{
    public List<Color> GenerateMonochromaticPalette(Color baseColor, int count = 5)
    {
        var palette = new List<Color>();
        float step = 0.8f / count;

        for (int i = 0; i < count; i++)
        {
            float factor = 0.1f + (step * i);
            palette.Add(baseColor.Lighten(factor));
        }

        return palette;
    }

    public List<Color> GenerateComplementaryColors(Color baseColor)
    {
        return new List<Color>
        {
            baseColor,
            baseColor.Invert(),
            baseColor.Lighten(0.2f),
            baseColor.Darken(0.2f)
        };
    }
}
```

### Accessibility Helpers

```csharp
public class AccessibilityHelper
{
    private const float MinimumContrastRatio = 4.5f; // WCAG AA standard

    public bool HasSufficientContrast(Color foreground, Color background)
    {
        float luminance1 = foreground.GetLuminance();
        float luminance2 = background.GetLuminance();

        float lighter = Math.Max(luminance1, luminance2);
        float darker = Math.Min(luminance1, luminance2);

        float contrastRatio = (lighter + 0.05f) / (darker + 0.05f);

        return contrastRatio >= MinimumContrastRatio;
    }

    public Color AdjustForContrast(Color foreground, Color background)
    {
        if (HasSufficientContrast(foreground, background))
        {
            return foreground;
        }

        // Adjust the foreground color
        if (background.IsLight())
        {
            return foreground.Darken(0.3f);
        }
        else
        {
            return foreground.Lighten(0.3f);
        }
    }
}
```

### Color Picker

```csharp
public class ColorPickerViewModel : ObservableObject
{
    [ObservableProperty]
    private Color selectedColor = Colors.Blue;

    [ObservableProperty]
    private string hexValue = "#0000FF";

    partial void OnSelectedColorChanged(Color value)
    {
        HexValue = value.ToHex();
        OnPropertyChanged(nameof(PreviewColors));
    }

    partial void OnHexValueChanged(string value)
    {
        if (ColorExtensions.TryFromHex(value, out Color? color) && color is not null)
        {
            SelectedColor = color;
        }
    }

    public List<Color> PreviewColors => new()
    {
        SelectedColor.Lighten(0.4f),
        SelectedColor.Lighten(0.2f),
        SelectedColor,
        SelectedColor.Darken(0.2f),
        SelectedColor.Darken(0.4f)
    };
}
```

### Status Indicators

```csharp
public class StatusIndicatorViewModel : ObservableObject
{
    public Color GetStatusColor(string status) => status switch
    {
        "success" => Colors.Green,
        "warning" => Colors.Orange,
        "error" => Colors.Red,
        "info" => Colors.Blue,
        _ => Colors.Gray
    };

    public Color GetStatusBackgroundColor(string status)
    {
        Color baseColor = GetStatusColor(status);
        return baseColor.Lighten(0.7f).WithAlpha(0.3f);
    }

    public Color GetStatusBorderColor(string status)
    {
        Color baseColor = GetStatusColor(status);
        return baseColor.Darken(0.1f);
    }
}
```

### Gradient Generation

```csharp
public class GradientHelper
{
    public List<Color> GenerateGradient(Color start, Color end, int steps)
    {
        var gradient = new List<Color>();

        for (int i = 0; i < steps; i++)
        {
            float progress = i / (float)(steps - 1);
            gradient.Add(start.Blend(end, progress));
        }

        return gradient;
    }

    public Brush CreateLinearGradient(Color start, Color end)
    {
        return new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(start, 0.0f),
                new GradientStop(start.Blend(end, 0.5f), 0.5f),
                new GradientStop(end, 1.0f)
            },
            new Point(0, 0),
            new Point(1, 1)
        );
    }
}
```

---

## Best Practices

### 1. Use Named Colors

```csharp
✅ Good:
public static class AppColors
{
    public static Color Primary => Colors.Blue;
    public static Color PrimaryLight => Primary.Lighten(0.2f);
    public static Color PrimaryDark => Primary.Darken(0.2f);
}

❌ Avoid:
Color button = Color.FromRgb(0, 0, 255).Lighten(0.2f);
```

### 2. Cache Computed Colors

```csharp
✅ Good:
private readonly Lazy<Color> hoverColor;

public MyViewModel()
{
    hoverColor = new Lazy<Color>(() => BaseColor.Lighten(0.15f));
}

public Color HoverColor => hoverColor.Value;

❌ Avoid:
public Color HoverColor => BaseColor.Lighten(0.15f); // Computed every access
```

### 3. Validate User Input

```csharp
✅ Good:
if (ColorExtensions.TryFromHex(userInput, out Color? color) && color is not null)
{
    SelectedColor = color;
}
else
{
    await DisplayAlert("Invalid Color", "Please enter a valid hex color.", "OK");
}

❌ Avoid:
Color color = ColorExtensions.FromHex(userInput); // May throw exception
```

### 4. Consider Accessibility

```csharp
✅ Good:
Color textColor = backgroundColor.GetContrastingTextColor();

if (!HasSufficientContrast(textColor, backgroundColor))
{
    textColor = backgroundColor.IsDark() ? Colors.White : Colors.Black;
}

❌ Avoid:
Color textColor = Colors.Gray; // May not have sufficient contrast
```

---

## See Also

- [Guard](Guard.md) - Argument validation
- [BatteryService](BatteryService.md) - Battery monitoring
- [DeviceInfoService](DeviceInfoService.md) - Device information

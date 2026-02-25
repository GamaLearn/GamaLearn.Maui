# Coding Standards

This document defines the coding standards and best practices for the GamaLearn.Maui library project.

---

## Table of Contents

- [General Principles](#general-principles)
- [File Organization](#file-organization)
- [Naming Conventions](#naming-conventions)
- [Code Formatting](#code-formatting)
- [Documentation](#documentation)
- [C# Language Features](#c-language-features)
- [.NET MAUI Specific](#net-maui-specific)
- [Error Handling](#error-handling)
- [Platform-Specific Code](#platform-specific-code)
- [Testing](#testing)
- [Performance](#performance)

---

## General Principles

### Code Quality

- **Readability First**: Write code that is easy to read and understand. Code is read far more often than it is written.
- **KISS (Keep It Simple, Stupid)**: Favor simple, straightforward solutions over clever or complex ones.
- **DRY (Don't Repeat Yourself)**: Avoid code duplication. Extract common functionality into reusable methods or classes.
- **YAGNI (You Aren't Gonna Need It)**: Don't add functionality until it's necessary.
- **Single Responsibility**: Each class and method should have one clear purpose.

### Modern C# Standards

- Target **.NET 9.0** or later
- Enable **nullable reference types** in all projects
- Use modern C# language features (pattern matching, records, init-only setters, etc.)
- Follow the latest C# coding conventions from Microsoft

---

## File Organization

### Project Structure

```
source/
  GamaLearn.Maui.Core/              # Core utilities library
    Collections/                     # Collection types
    Extensions/                      # Extension methods
    Guards/                          # Guard clauses
    Services/                        # Service interfaces and implementations
    Threading/                       # Threading utilities
    Platforms/                       # Platform-specific implementations
      Android/
      iOS/
      MacCatalyst/
      Windows/
      Tizen/
  GamaLearn.Maui.UIKit/             # UI controls library
    Controls/                        # UI control definitions
      GLButton/
      GLSegmentedControl/
    Handlers/                        # Platform handlers
    Platforms/                       # Platform-specific views
    Enums/                           # Enumerations
tests/
  GamaLearn.Maui.Core.Tests/
  GamaLearn.Maui.UIKit.Tests/
samples/
  GamaLearn.Maui.UIKit.SampleApp/
```

### File Naming

- **One class per file** (except for nested types)
- **File name matches class name**: `GLButton.cs` for `GLButton` class
- **Platform-specific files** use suffixes:
  - `.android.cs` for Android
  - `.ios.cs` for iOS
  - `.mac.cs` for MacCatalyst
  - `.windows.cs` for Windows
  - `.shared.cs` for shared cross-platform code
  - Example: `GLButtonHandler.windows.cs`, `GLButtonHandler.shared.cs`

### Namespace Organization

- **Namespaces by feature**, not folder structure (per [.editorconfig](.editorconfig))
- Use consistent, logical namespace hierarchies:
  - `GamaLearn.Guards` for guard clauses
  - `GamaLearn.Controls` for UI controls
  - `GamaLearn.Platforms.Windows` for Windows-specific code
  - `GamaLearn.Extensions` for extension methods

### Using Directives

- Place all `using` directives at the top of the file
- Remove unused using directives
- Order using directives:
  1. System namespaces
  2. Microsoft namespaces
  3. Third-party namespaces
  4. Project namespaces
- Use `global using` for commonly used namespaces (in `GlobalUsings.cs`)

Example:
```csharp
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;

namespace GamaLearn.Controls;
```

---

## Naming Conventions

### General Rules

| Identifier | Casing | Example |
|------------|--------|---------|
| Namespace | PascalCase | `GamaLearn.Controls` |
| Class | PascalCase | `GLButton` |
| Interface | PascalCase with `I` prefix | `IGLButton` |
| Method | PascalCase | `UpdateBackgroundColor()` |
| Property | PascalCase | `IconSize` |
| Event | PascalCase | `Clicked` |
| Field (public/protected) | PascalCase | `VisualStateNormal` |
| Field (private) | camelCase | `virtualView`, `isPressed` |
| Parameter | camelCase | `imageSource`, `mauiContext` |
| Local variable | camelCase | `cacheKey`, `focusColor` |
| Constant | PascalCase | `VisualStateNormal` |
| Enum type | PascalCase | `GLButtonBadgeMode` |
| Enum value | PascalCase | `IconOnly`, `AspectFit` |

### Naming Guidelines

**Classes and Interfaces**
- Use **nouns** or **noun phrases**
- Be specific and descriptive: `GLButtonPlatformView` not `View`
- Interfaces should describe capability: `IAppRatingService`
- Prefix custom controls with library identifier: `GL` in `GLButton`

**Methods**
- Use **verbs** or **verb phrases**
- Clearly describe what the method does:
  - `UpdateBackgroundColor()` - updates the background color
  - `LoadIcon()` - loads an icon
  - `SendClicked()` - sends a clicked event
- Boolean-returning methods should ask a question:
  - `ShouldShowBadge()` not `ShowBadge()`
  - `IsInRange()` not `InRange()`

**Properties**
- Use **nouns** or **noun phrases**
- Boolean properties should be affirmative phrases:
  - `IsEnabled` not `IsDisabled`
  - `IsBusy` not `NotBusy`
- Avoid prefixes like `Get` or `Set` (use properties, not methods)

**Events**
- Use **past tense** for post-action events: `Clicked`, `Pressed`, `Released`
- Use **-ing** form for pre-action events: `Clicking`, `Pressing`
- Event handler delegates should end with `EventHandler`
- Event args should end with `EventArgs`: `RatingResponseEventArgs`

**Fields**
- Private fields use camelCase: `virtualView`, `isPressed`, `iconCache`
- Avoid Hungarian notation (`strName`, `intCount`)
- Static readonly fields use PascalCase: `VisualStateNormal`
- Constants use PascalCase: `DefaultIconSize`

**Parameters**
- Use descriptive names that indicate purpose: `imageSource`, `mauiContext`
- Avoid single-letter names except for:
  - Loop counters: `i`, `j`, `k`
  - Common conventions: `e` for event args, `T` for generic type parameters

---

## Code Formatting

### Indentation and Spacing

- Use **4 spaces** for indentation (no tabs)
- One statement per line
- One declaration per line
- Add blank line between methods and logical code blocks
- No trailing whitespace

### Braces

- **Allman style** (braces on new line):
```csharp
if (condition)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```

- Always use braces, even for single-line blocks:
```csharp
// ✓ Good
if (isPressed)
{
    return;
}

// ✗ Bad
if (isPressed)
    return;
```

### Line Length

- Keep lines under **120 characters** when practical
- Break long lines at natural boundaries (parameters, operators)
- Align continuation lines for readability

### Regions

Use `#region` / `#endregion` to organize code sections in large files:

```csharp
public class GLButton
{
    #region Visual State Constants
    public const string VisualStateNormal = "Normal";
    #endregion

    #region Bindable Properties
    public static readonly BindableProperty TextProperty = ...;
    #endregion

    #region Public Methods
    public void UpdateAll() { }
    #endregion

    #region Private Methods
    private void Initialize() { }
    #endregion

    #region Event Handlers
    private void OnPointerPressed(object sender, EventArgs e) { }
    #endregion
}
```

Common region names:
- `Fields`, `Properties`, `Events`, `Constructors`
- `Public Methods`, `Private Methods`, `Protected Methods`
- `Event Handlers`, `Helper Methods`, `Static Methods`
- `Nested Types`, `Interface Implementation`
- `Platform-specific Methods`

---

## Documentation

### XML Documentation

**All public APIs must have XML documentation comments** including:
- Classes
- Interfaces
- Public/protected methods
- Public/protected properties
- Events
- Enum types and values

**Required tags:**
- `<summary>` - Brief description of the member
- `<param>` - For each method parameter
- `<returns>` - For methods that return values
- `<exception>` - For documented exceptions
- `<remarks>` - Additional details (optional)
- `<example>` - Usage examples (optional)
- `<seealso>` or `<see>` - References to related members

**Example:**
```csharp
/// <summary>
/// Updates the background color or brush of the button based on the current visual state and virtual view properties.
/// </summary>
public void UpdateBackgroundColor()
{
    // Implementation
}

/// <summary>
/// Throws <see cref="ArgumentNullException"/> if the value is null.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="value">The value to check.</param>
/// <param name="parameterName">The name of the parameter (auto-populated).</param>
/// <returns>The non-null value.</returns>
/// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
public static T IsNotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
{
    return value is null ? throw new ArgumentNullException(parameterName) : value;
}
```

### Inline Comments

- Use `//` for single-line comments
- Place comments **above** the code they describe, not on the same line
- Explain **why**, not **what** (the code shows what)
- Avoid obvious comments

```csharp
// ✓ Good - explains WHY
// Temporarily reserve layout space for the focus ring to avoid parent clipping
Padding = new Thickness(padLeft, padTop, padRight, padBottom);

// ✗ Bad - states the obvious
// Set padding
Padding = new Thickness(padLeft, padTop, padRight, padBottom);
```

### File Headers

No file headers or copyright notices are required. Focus on code documentation instead.

---

## C# Language Features

### Nullable Reference Types

- Enable nullable reference types in all projects
- Use `?` to indicate nullable reference types: `IGLButton?`
- Use `[NotNull]` attribute for guard clause parameters
- Avoid `!` (null-forgiving operator) unless absolutely necessary

```csharp
// ✓ Good
public void SetVirtualView(IGLButton? virtualView, IMauiContext? mauiContext)
{
    this.virtualView = virtualView;
    this.mauiContext = mauiContext;
}

public static T IsNotNull<T>([NotNull] T? value, string? parameterName = null)
{
    return value ?? throw new ArgumentNullException(parameterName);
}
```

### Modern C# Features

**Pattern Matching**
```csharp
// ✓ Good - use switch expressions
public Color GetCurrentIconColor() => CurrentVisualState switch
{
    VisualStateHover => HoverIconColor ?? IconColor,
    VisualStatePressed => PressedIconColor ?? IconColor,
    VisualStateDisabled => DisabledIconColor ?? IconColor,
    _ => IconColor
};

// ✓ Good - use type patterns
if (imageSource is FileImageSource fileSource)
{
    var filePath = fileSource.File;
}
```

**Expression-Bodied Members**
```csharp
// ✓ Good - for simple getters, setters, and methods
public string Text
{
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
}

private static Stretch GetWindowsStretch(GLIconAspect aspect) => aspect switch
{
    GLIconAspect.AspectFit => Stretch.Uniform,
    GLIconAspect.AspectFill => Stretch.UniformToFill,
    _ => Stretch.Uniform
};
```

**CallerArgumentExpression**
```csharp
// ✓ Good - use for guard clauses
public static T IsNotNull<T>(
    [NotNull] T? value,
    [CallerArgumentExpression(nameof(value))] string? parameterName = null)
{
    return value is null ? throw new ArgumentNullException(parameterName) : value;
}
```

**Conditional Access and Null-Coalescing**
```csharp
// ✓ Good
virtualView?.UpdateVisualState(GLButton.VisualStateNormal);

var color = HoverIconColor ?? IconColor ?? Colors.Transparent;
```

### Async/Await

- Use `async`/`await` for asynchronous operations
- Methods returning `Task` should end with `Async` suffix: `LoadIconAsync()`
- Avoid `async void` except for event handlers
- Always use `ConfigureAwait(false)` in library code (not UI code)

```csharp
// ✓ Good
private async Task LoadIconAsync(ImageSource imageSource)
{
    var result = await imageSource.GetPlatformImageAsync(mauiContext).ConfigureAwait(false);
    // Process result
}
```

---

## .NET MAUI Specific

### Bindable Properties

**Define bindable properties for all bindable members:**

```csharp
public static readonly BindableProperty TextProperty =
    BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(GLButton),
        string.Empty);

public string Text
{
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
}
```

**Guidelines:**
- Property name + `Property` suffix for the backing field
- Use `nameof()` for property name
- Specify correct default values
- Add property changed callbacks when needed

### Handlers

**Follow the handler pattern for custom controls:**
- Implement `IViewHandler` or derive from appropriate handler base class
- Use platform-specific partial classes:
  - `GLButtonHandler.shared.cs` - cross-platform logic
  - `GLButtonHandler.windows.cs` - Windows implementation
  - `GLButtonHandler.android.cs` - Android implementation
  - `GLButtonHandler.ios.cs` - iOS implementation
  - `GLButtonHandler.mac.cs` - MacCatalyst implementation
- Use `PropertyMapper` for property updates

### Platform-Specific Views

**Create native platform views that implement the control interface:**
- `GLButtonPlatformView` (Windows) - derives from native WinUI controls
- Follow platform design guidelines (Fluent Design, Material Design, etc.)
- Implement proper lifecycle management (initialize, update, cleanup)
- Handle platform-specific events and map to cross-platform events

---

## Error Handling

### Guard Clauses

**Use guard clauses at method entry to validate parameters:**

```csharp
using GamaLearn.Guards;

public void ProcessData(string data, int count)
{
    Guard.IsNotNullOrWhiteSpace(data);
    Guard.IsPositive(count);

    // Method implementation
}
```

**Available Guard methods:**
- `IsNotNull<T>` - Check for null
- `IsNotNullOrEmpty` - Check for null or empty string
- `IsNotNullOrWhiteSpace` - Check for null or whitespace string
- `IsInRange<T>` - Check if value in range
- `IsPositive` - Check for positive integers
- `IsNotNegative` - Check for non-negative integers
- `IsTrue` / `IsFalse` - Check boolean conditions

### Exception Handling

**When to catch exceptions:**
- Only catch exceptions you can meaningfully handle
- Don't catch and ignore exceptions
- Log exceptions when appropriate

```csharp
// ✓ Good - specific exception handling
try
{
    await LoadIconAsync(imageSource);
}
catch (FileNotFoundException ex)
{
    // Gracefully handle missing file
    iconImage.Source = defaultIcon;
    logger.LogWarning(ex, "Icon file not found: {Path}", imageSource);
}

// ✗ Bad - swallowing all exceptions
try
{
    await LoadIconAsync(imageSource);
}
catch
{
    // Silent failure
}
```

**Exception types:**
- `ArgumentNullException` - for null arguments
- `ArgumentException` - for invalid arguments
- `ArgumentOutOfRangeException` - for out-of-range values
- `InvalidOperationException` - for invalid state
- `NotSupportedException` - for unsupported operations

### Null Checking

**Prefer pattern matching over null checks:**

```csharp
// ✓ Good
if (virtualView is null)
    return;

// ✓ Also good
if (virtualView == null)
    return;

// ✗ Avoid
if (virtualView != null)
{
    // Large block of code
}
```

---

## Platform-Specific Code

### Conditional Compilation

Use platform-specific symbols for conditional compilation:

```csharp
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#elif ANDROID
using Android.Widget;
using Android.Views;
#elif IOS || MACCATALYST
using UIKit;
#endif
```

### Platform Abstraction

**Create interfaces for platform-specific functionality:**

```csharp
// Shared interface
public interface IAppRatingService
{
    Task RequestReviewAsync();
}

// Platform-specific implementation
// Platforms/Android/AppRatingService.cs
public class AppRatingService : IAppRatingService
{
    public async Task RequestReviewAsync()
    {
        // Android-specific implementation
    }
}
```

### Native API Access

**Wrap native APIs in a clean, idiomatic C# interface:**
- Use meaningful method names
- Convert native types to .NET types when appropriate
- Handle platform differences gracefully
- Document platform-specific behaviors

---

## Testing

### Unit Tests

- Place tests in separate test projects: `GamaLearn.Maui.Core.Tests`
- Test class name should be `{ClassUnderTest}Tests`
- Test method names should describe the scenario: `UpdateBackgroundColor_WhenHovered_AppliesHoverColor`
- Use Arrange-Act-Assert pattern

```csharp
[Fact]
public void IsNotNull_WhenValueIsNull_ThrowsArgumentNullException()
{
    // Arrange
    string? value = null;

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => Guard.IsNotNull(value));
}

[Fact]
public void IsNotNull_WhenValueIsNotNull_ReturnsValue()
{
    // Arrange
    string value = "test";

    // Act
    var result = Guard.IsNotNull(value);

    // Assert
    Assert.Equal("test", result);
}
```

### Test Coverage

- Aim for high test coverage of public APIs
- Focus on business logic and complex algorithms
- Test edge cases and error conditions
- Don't test framework code or trivial properties

---

## Performance

### General Guidelines

- **Measure before optimizing** - use profiling tools
- **Avoid premature optimization** - focus on clean code first
- **Cache expensive operations** - like image loading, calculations
- **Dispose resources properly** - implement `IDisposable` when needed
- **Avoid allocations in hot paths** - reuse objects, use object pooling

### MAUI-Specific Performance

**Property Updates**
- Minimize property changed notifications
- Batch related updates when possible
- Use `BeginBatchUpdate()` / `EndBatchUpdate()` for multiple property changes

**Image Loading**
- Cache loaded platform images (see `IconStateCache` in `GLButtonPlatformView`)
- Reuse image controls instead of recreating
- Use appropriate image formats and sizes

**Layout**
- Avoid deep view hierarchies
- Use `Grid` efficiently with spans
- Minimize layout passes

**Animations**
- Respect `UISettings.AnimationsEnabled` for accessibility
- Use platform-native animations when available
- Clean up animation resources

**Memory Management**
- Unsubscribe from events in cleanup/disposal
- Clear caches when appropriate
- Avoid memory leaks from event handlers

```csharp
public void Cleanup()
{
    // Unsubscribe from events
    PointerPressed -= OnPointerPressed;
    PointerReleased -= OnPointerReleased;

    // Clear references
    virtualView = null;
    iconCache.Clear();
}
```

---

## Code Review Checklist

Before submitting code for review, ensure:

- [ ] Code follows naming conventions
- [ ] All public APIs have XML documentation
- [ ] Guard clauses validate parameters
- [ ] Nullable reference types are used correctly
- [ ] Platform-specific code is properly isolated
- [ ] No compiler warnings
- [ ] Code is formatted consistently
- [ ] Complex logic has explanatory comments
- [ ] Resources are properly disposed
- [ ] Tests are added for new functionality
- [ ] No hardcoded strings or magic numbers
- [ ] Error cases are handled appropriately

---

## Additional Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET API Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [MAUI Handler Architecture](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/)

---

## Questions or Feedback?

If you have questions about these coding standards or suggestions for improvements, please open an issue or discussion in the repository.

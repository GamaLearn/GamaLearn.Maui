# SafeAreaHelper

Static helper for getting safe area insets on devices with notches, rounded corners, or system bars.

**Namespace:** `GamaLearn.Helpers`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

SafeAreaHelper provides platform-specific methods to retrieve safe area insets, ensuring your UI doesn't overlap with system UI elements like notches, status bars, navigation bars, and rounded corners. Critical for modern devices like iPhone 14 Pro, Pixel 7, and other edge-to-edge displays.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Methods

### GetSafeAreaInsets()

Gets the safe area insets for the current window.

**Returns:** `Thickness` (Left, Top, Right, Bottom) in device-independent units

```csharp
using GamaLearn.Helpers;

Thickness insets = SafeAreaHelper.GetSafeAreaInsets();
// Example: { Left=0, Top=59, Right=0, Bottom=34 } on iPhone 14 Pro
```

### GetSafeAreaInsets(Page page)

Gets the safe area insets for a specific page.

**Parameters:**
- `page` - The page to get insets for

**Returns:** `Thickness` representing safe area insets

```csharp
Thickness insets = SafeAreaHelper.GetSafeAreaInsets(this);
```

### ApplySafeAreaPadding(Layout layout, SafeAreaEdges edges = SafeAreaEdges.All)

Applies safe area padding to a layout by adding insets to current padding.

**Parameters:**
- `layout` - The layout to apply padding to
- `edges` - Which edges to apply padding to (default: all)

```csharp
SafeAreaHelper.ApplySafeAreaPadding(myStackLayout);

// Only apply to top and bottom
SafeAreaHelper.ApplySafeAreaPadding(myStackLayout, SafeAreaEdges.Top | SafeAreaEdges.Bottom);
```

### ApplySafeAreaPadding(Page page, SafeAreaEdges edges = SafeAreaEdges.All)

Applies safe area padding to a page.

```csharp
SafeAreaHelper.ApplySafeAreaPadding(this);
```

### GetStatusBarHeight()

Gets the status bar height.

**Returns:** Status bar height in device-independent units

```csharp
double statusBarHeight = SafeAreaHelper.GetStatusBarHeight();
// Example: 59 on iPhone 14 Pro, 24 on most Android devices
```

### GetNavigationBarHeight()

Gets the navigation bar height (bottom system UI on Android, home indicator on iOS).

**Returns:** Navigation bar height in device-independent units

```csharp
double navBarHeight = SafeAreaHelper.GetNavigationBarHeight();
// Example: 34 on iPhone 14 Pro, 48 on Android with gesture nav
```

---

## SafeAreaEdges Enum

```csharp
[Flags]
public enum SafeAreaEdges
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    All = Left | Top | Right | Bottom
}
```

---

## Usage Examples

### Basic Page Setup

```csharp
public partial class MyPage : ContentPage
{
    public MyPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Apply safe area padding to the page
        SafeAreaHelper.ApplySafeAreaPadding(this);
    }
}
```

### Full-Screen Content with Safe Areas

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.FullScreenPage"
             BackgroundColor="Black">

    <Grid>
        <!-- Background image extends to edges -->
        <Image Source="background.jpg" Aspect="AspectFill" />

        <!-- Content respects safe areas -->
        <VerticalStackLayout x:Name="ContentLayout"
                             VerticalOptions="Fill">
            <Label Text="Title" FontSize="32" TextColor="White" />
            <Label Text="Content" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();
    SafeAreaHelper.ApplySafeAreaPadding(ContentLayout);
}
```

### Selective Safe Area Application

```csharp
public class CustomHeaderPage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        var header = this.FindByName<StackLayout>("Header");
        var footer = this.FindByName<StackLayout>("Footer");

        // Only apply to top and bottom
        SafeAreaHelper.ApplySafeAreaPadding(header, SafeAreaEdges.Top);
        SafeAreaHelper.ApplySafeAreaPadding(footer, SafeAreaEdges.Bottom);
    }
}
```

### Dynamic Layout Adjustments

```csharp
public class AdaptiveViewModel : ObservableObject
{
    private double topPadding;
    public double TopPadding
    {
        get => topPadding;
        set => SetProperty(ref topPadding, value);
    }

    private double bottomPadding;
    public double BottomPadding
    {
        get => bottomPadding;
        set => SetProperty(ref bottomPadding, value);
    }

    public void UpdateSafeAreaPadding(Page page)
    {
        Thickness insets = SafeAreaHelper.GetSafeAreaInsets(page);
        TopPadding = insets.Top;
        BottomPadding = insets.Bottom;
    }
}
```

```xml
<ContentPage>
    <VerticalStackLayout Padding="{Binding TopPadding, 0, BottomPadding, 0}">
        <Label Text="Content" />
    </VerticalStackLayout>
</ContentPage>
```

### Status Bar Overlay

```csharp
public class TransparentStatusBarPage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Get status bar height
        double statusBarHeight = SafeAreaHelper.GetStatusBarHeight();

        // Adjust header to account for transparent status bar
        var header = this.FindByName<Grid>("Header");
        header.Padding = new Thickness(0, statusBarHeight, 0, 0);
    }
}
```

### Floating Action Button

```csharp
<ContentPage>
    <AbsoluteLayout>
        <VerticalStackLayout AbsoluteLayout.LayoutBounds="0,0,1,1"
                             AbsoluteLayout.LayoutFlags="All">
            <!-- Main content -->
        </VerticalStackLayout>

        <!-- FAB positioned with safe area consideration -->
        <Button x:Name="Fab"
                Text="+"
                AbsoluteLayout.LayoutBounds="0.95,0.95,60,60"
                AbsoluteLayout.LayoutFlags="PositionProportional"
                CornerRadius="30" />
    </AbsoluteLayout>
</ContentPage>
```

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();

    double navBarHeight = SafeAreaHelper.GetNavigationBarHeight();

    // Adjust FAB position to avoid navigation bar
    AbsoluteLayout.SetLayoutBounds(Fab,
        new Rect(0.95, 0.95, 60, 60 + navBarHeight));
}
```

### Modal with Safe Areas

```csharp
public class ModalPage : ContentPage
{
    public ModalPage()
    {
        InitializeComponent();

        // Full screen modal
        Shell.SetPresentationMode(this, PresentationMode.FullScreen);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Apply safe area to ensure close button is visible
        SafeAreaHelper.ApplySafeAreaPadding(this);
    }
}
```

### Landscape Orientation

```csharp
public class VideoPlayerPage : ContentPage
{
    private bool isPortrait = true;

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        bool wasPortrait = isPortrait;
        isPortrait = height > width;

        // Reapply safe area when orientation changes
        if (wasPortrait != isPortrait)
        {
            UpdateSafeAreas();
        }
    }

    private void UpdateSafeAreas()
    {
        Thickness insets = SafeAreaHelper.GetSafeAreaInsets(this);

        // In landscape, notches may be on left/right
        var playerControls = this.FindByName<Grid>("PlayerControls");
        playerControls.Padding = insets;
    }
}
```

---

## Best Practices

### 1. Apply on OnAppearing

```csharp
✅ Good:
protected override void OnAppearing()
{
    base.OnAppearing();
    SafeAreaHelper.ApplySafeAreaPadding(this);
}

❌ Avoid:
public MyPage()
{
    InitializeComponent();
    SafeAreaHelper.ApplySafeAreaPadding(this); // Too early, may not have handler
}
```

### 2. Reapply on Orientation Changes

```csharp
✅ Good:
protected override void OnSizeAllocated(double width, double height)
{
    base.OnSizeAllocated(width, height);
    SafeAreaHelper.ApplySafeAreaPadding(this);
}

❌ Avoid:
// Only applying once, doesn't adapt to rotation
```

### 3. Combine with Existing Padding

```csharp
✅ Good:
// ApplySafeAreaPadding adds to existing padding
myLayout.Padding = new Thickness(16); // Design padding
SafeAreaHelper.ApplySafeAreaPadding(myLayout); // Adds safe area

❌ Avoid:
// This overwrites design padding
Thickness insets = SafeAreaHelper.GetSafeAreaInsets();
myLayout.Padding = insets;
```

---

## Platform Behavior

### iOS / MacCatalyst
- Returns insets from `UIWindow.SafeAreaInsets`
- Includes notch, Dynamic Island, status bar, home indicator
- Example iPhone 14 Pro: Top=59, Bottom=34

### Android
- Returns combined system bars + display cutout insets
- Includes status bar, navigation bar, notches, punch holes
- Adapts to gesture navigation vs. button navigation

### Windows
- Returns title bar height at top
- No insets on other edges (no notches on Windows devices)

### Tizen
- Returns zero (no system UI overlays)

---

## Troubleshooting

### Insets Always Zero

Call `ApplySafeAreaPadding` in `OnAppearing`, not in the constructor:

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();
    SafeAreaHelper.ApplySafeAreaPadding(this);
}
```

### Content Still Overlaps System UI

Ensure you're applying to the correct layout. The layout must be at the root level of your page content.

### Excessive Padding

Don't apply safe area padding multiple times:

```csharp
❌ Avoid:
SafeAreaHelper.ApplySafeAreaPadding(myLayout);
SafeAreaHelper.ApplySafeAreaPadding(myLayout); // Applied twice!
```

---

## See Also

- [KeyboardHelper](KeyboardHelper.md) - Keyboard management
- [DeviceInfoService](DeviceInfoService.md) - Device information
- [BatteryService](BatteryService.md) - Battery monitoring

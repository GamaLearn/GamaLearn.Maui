# Sample Page Template

A guide and template for creating new control sample pages in the GamaLearn.Maui.UIKit sample app. Follow these conventions to ensure consistency across all demo pages.

---

## Quality Standards

- **7-8 curated sections** per page (not exhaustive test grids)
- **Enterprise demo quality** — each section tells a story about a real use case
- **Both themes** must look correct (light and dark)
- **Responsive** — works on Desktop, Tablet, and Phone
- **No hardcoded colors** — all colors from CommonTheme.xaml via `toolkit:AppThemeResource` or `StaticResource`
- **No color logic in ViewModels** — keep all visual definitions in XAML
- **No MAUI `Button`** — always use `gl:GLButton` instead. MAUI's `Button` triggers `MaterialButton` warnings on Android about custom backgrounds conflicting with Material elevation, shape, color, and state management. Use `Style="{StaticResource ActionButton}"` for small utility buttons.
- **Material Symbols icons only** — use `x:Static constants:MaterialOutlined.*`

---

## File Structure

Each control page consists of 3 files:

```
Pages/
  GL{ControlName}Page.xaml          ← XAML layout
  GL{ControlName}Page.xaml.cs       ← Code-behind (minimal)
  GL{ControlName}PageViewModel.cs   ← ViewModel (only for dynamic behavior)
```

---

## XAML Template

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:gl="clr-namespace:GamaLearn.Controls;assembly=GamaLearn.Maui.UIKit"
             xmlns:constants="clr-namespace:GamaLearn.Maui.UIKit.SampleApp.Resources.Constants"
             xmlns:local="clr-namespace:GamaLearn.Maui.UIKit.SampleApp.Pages"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="GamaLearn.Maui.UIKit.SampleApp.Pages.GL{ControlName}Page"
             x:DataType="local:GL{ControlName}PageViewModel"
             Title="GL{ControlName}"
             BackgroundColor="{toolkit:AppThemeResource PageBg}">

    <ContentPage.BindingContext>
        <local:GL{ControlName}PageViewModel />
    </ContentPage.BindingContext>

    <ScrollView Padding="16">
        <VerticalStackLayout Spacing="20"
                             MaximumWidthRequest="960"
                             HorizontalOptions="Center">

            <!--  Page Header  -->
            <VerticalStackLayout Spacing="4"
                                 Margin="0,8,0,0">
                <HorizontalStackLayout Spacing="10">
                    <Label Text="{x:Static constants:MaterialOutlined.{PageIcon}}"
                           FontFamily="MaterialOutlined"
                           FontSize="28"
                           TextColor="{StaticResource Primary}"
                           VerticalOptions="Center" />
                    <Label Text="GL{ControlName}"
                           FontSize="26"
                           FontAttributes="Bold"
                           TextColor="{toolkit:AppThemeResource TextColor}"
                           VerticalOptions="Center" />
                </HorizontalStackLayout>
                <Label Text="Brief description of what this control does"
                       Style="{StaticResource SectionDescription}" />
            </VerticalStackLayout>

            <!--  Section 1: {SectionName}  -->
            <Border Style="{StaticResource SectionBorder}">
                <VerticalStackLayout Spacing="12">
                    <HorizontalStackLayout Spacing="8">
                        <Label Text="{x:Static constants:MaterialOutlined.{SectionIcon}}"
                               FontFamily="MaterialOutlined"
                               FontSize="20"
                               TextColor="{toolkit:AppThemeResource PrimaryButtonBg}"
                               VerticalOptions="Center" />
                        <Label Text="{SectionTitle}"
                               Style="{StaticResource SectionTitle}"
                               VerticalOptions="Center" />
                    </HorizontalStackLayout>
                    <Label Text="{SectionDescription}"
                           Style="{StaticResource SectionDescription}" />

                    <!-- Demo content here -->
                    <FlexLayout Wrap="Wrap"
                                JustifyContent="Start"
                                AlignItems="Center">
                        <!-- Control instances -->
                    </FlexLayout>
                </VerticalStackLayout>
            </Border>

            <!-- Repeat sections 2-8... -->

            <!--  Footer  -->
            <Label Text="GL{ControlName} — GamaLearn.Maui.UIKit"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondary}"
                   HorizontalOptions="Center"
                   Margin="0,16,0,40" />

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

---

## Code-Behind Template

Keep the code-behind minimal. Avoid event handlers — use commands in XAML instead.

```csharp
namespace GamaLearn.Maui.UIKit.SampleApp.Pages;

public partial class GL{ControlName}Page : ContentPage
{
    public GL{ControlName}Page()
    {
        InitializeComponent();
    }
}
```

---

## ViewModel Template

Only include properties and commands needed for **dynamic behavior** (async operations, add/remove items, toggle states). Static demo content belongs in XAML.

```csharp
namespace GamaLearn.Maui.UIKit.SampleApp.Pages;

public partial class GL{ControlName}PageViewModel : ObservableObject
{
    #region Properties
    [ObservableProperty]
    public partial string ResultMessage { get; set; } = string.Empty;
    #endregion

    #region Commands
    [RelayCommand]
    private async Task DoSomethingAsync()
    {
        ResultMessage = "Processing...";
        await Task.Delay(1000);
        ResultMessage = $"Done at {DateTime.Now:HH:mm:ss}";
    }
    #endregion
}
```

**Rules for ViewModels:**
- Use `[ObservableProperty]` with `public partial` property syntax
- Use `[RelayCommand]` for all commands
- Use `object?` parameter type for commands bound to control selection events (avoids XamlC compiled binding issues with cross-assembly types)
- No `Color.FromArgb()` or any color definitions — all colors belong in XAML/CommonTheme.xaml
- Only create `ObservableCollection` for sections that demo add/remove/dynamic changes
- Static collections belong as `x:Array` in XAML

---

## Color Usage Reference

### Semantic Tokens (via `toolkit:AppThemeResource`)

These adapt automatically to light/dark mode:

| Token | Purpose |
|-------|---------|
| `PageBg` | Page background |
| `CardBg` | Section card/border fill |
| `CardBorder` | Section card/border stroke |
| `TextColor` | Primary text |
| `InvertedTextColor` | Text on colored backgrounds |
| `PrimaryButtonBg` | Primary action button |
| `PrimaryButtonHoverBg` | Primary hover state |
| `PrimaryButtonPressedBg` | Primary pressed state |
| `PrimaryButtonText` | Primary button text |
| `SecondaryButtonBg/HoverBg/PressedBg/Text` | Secondary button states |
| `OutlineButtonBorder/HoverBg/Text` | Outline button states |
| `GhostButtonHoverBg/PressedBg/Text` | Ghost button states |
| `DangerButtonBg/HoverBg/PressedBg` | Danger/destructive states |
| `SuccessButtonBg/HoverBg/PressedBg` | Success states |
| `WarningButtonBg/HoverBg/PressedBg` | Warning states |
| `DisabledButtonBg/Text/Border` | Disabled states |
| `DisabledIconColor` | Disabled icon tint |
| `SegmentFg/HoverFg/HoverBg` | Segmented control states |
| `SegmentPressedFg/PressedBg` | Segmented pressed |
| `SegmentSelectedFg/SelectedBg` | Segmented selected |
| `SegmentDisabledFg/DisabledBg` | Segmented disabled |
| `SegmentBorderColor` | Segmented border |
| `FlyoutBg/HeaderBg/ItemText/ItemSelectedBg` | Shell flyout |
| `BlueFocusColor` | Focus ring color |

### Palette Colors (via `StaticResource`)

Theme-invariant colors for decorative use:

- `Primary`, `PrimaryDark` — brand colors
- `{Color}{Shade}` — e.g., `Blue4`, `Red1`, `Green9` (1=lightest, 9=darkest)
- Available color families: Blue, Red, Green, Orange, Purple, Pink, Amber, Cyan, Teal, Gray
- `TextSecondary` — secondary/muted text
- `White`, `Black` — absolutes (use sparingly)

### When to Use What

| Scenario | Approach |
|----------|----------|
| Page background | `{toolkit:AppThemeResource PageBg}` |
| Section container bg/stroke | `{StaticResource SectionBorder}` style |
| Text that must adapt to theme | `{toolkit:AppThemeResource TextColor}` |
| Button variant colors | `{toolkit:AppThemeResource PrimaryButtonBg}` etc. |
| Decorative tinted background | `{AppThemeBinding Light={StaticResource Blue1}, Dark={StaticResource Blue9}}` |
| Color that never changes (e.g., white text on blue) | `{StaticResource White}` |

---

## Icon Usage

All icons use Material Symbols via the `MaterialOutlined` font:

```xml
<!-- As standalone icon label -->
<Label Text="{x:Static constants:MaterialOutlined.Home}"
       FontFamily="MaterialOutlined"
       FontSize="20" />

<!-- As button icon -->
<gl:GLButton.Icon>
    <FontImageSource Glyph="{x:Static constants:MaterialOutlined.Save}"
                     FontFamily="MaterialOutlined"
                     Size="20" />
</gl:GLButton.Icon>

<!-- As segmented control item icon -->
<gl:GLSegmentedItem Icon="{FontImageSource FontFamily=MaterialOutlined,
                                           Glyph={x:Static constants:MaterialOutlined.Home},
                                           Size=20}" />
```

Find available icons in `Resources/Constants/MaterialIcons.cs`. The file contains both `MaterialOutlined` and `MaterialOutlineFilled` classes.

---

## Responsive Layout Patterns

### Max-width centering (all pages)
```xml
<VerticalStackLayout MaximumWidthRequest="960"
                     HorizontalOptions="Center">
```

### Wrapped button/control groups
```xml
<FlexLayout Wrap="Wrap"
            JustifyContent="Start"
            AlignItems="Center">
    <!-- Items with Margin="0,0,10,10" for spacing -->
</FlexLayout>
```

### Responsive grid columns
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Default='*,*'}"
      ColumnSpacing="12"
      RowSpacing="12">
```

### Static data in XAML (preferred over ViewModel collections)
```xml
<gl:GLSegmentedControl.ItemsSource>
    <x:Array Type="{x:Type gl:GLSegmentedItem}">
        <gl:GLSegmentedItem Text="Option A"
                            ForegroundColor="{toolkit:AppThemeResource SegmentFg}"
                            BackgroundColor="Transparent"
                            HoverForegroundColor="{toolkit:AppThemeResource SegmentHoverFg}"
                            HoverBackgroundColor="{toolkit:AppThemeResource SegmentHoverBg}"
                            SelectedForegroundColor="{toolkit:AppThemeResource SegmentSelectedFg}"
                            SelectedBackgroundColor="{toolkit:AppThemeResource SegmentSelectedBg}" />
        <!-- More items... -->
    </x:Array>
</gl:GLSegmentedControl.ItemsSource>
```

---

## Registration Checklist

When adding a new sample page:

1. **Create the 3 files** in `Pages/` following the templates above
2. **Register in MauiProgram.cs** — no DI registration needed if BindingContext is set in XAML
3. **Add to AppShell.xaml** as a new `FlyoutItem`:
   ```xml
   <FlyoutItem Title="GL{ControlName}"
               Route="GL{ControlName}Page">
       <FlyoutItem.Icon>
           <FontImageSource FontFamily="MaterialOutlined"
                            Glyph="{x:Static constants:MaterialOutlined.{Icon}}"
                            Color="{toolkit:AppThemeResource FlyoutItemText}" />
       </FlyoutItem.Icon>
       <ShellContent ContentTemplate="{DataTemplate local:GL{ControlName}Page}" />
   </FlyoutItem>
   ```
4. **Add navigation card to MainPage.xaml** in the Controls Catalog grid
5. **Add navigation command to MainViewModel.cs**:
   ```csharp
   [RelayCommand]
   private async Task NavigateToGL{ControlName}()
   {
       await Shell.Current.GoToAsync("///GL{ControlName}Page");
   }
   ```

---

## Pre-Submission Checklist

- [ ] 7-8 sections maximum, each with a clear purpose
- [ ] Page header with Material icon + title + description
- [ ] Each section wrapped in `Border Style="{StaticResource SectionBorder}"`
- [ ] Each section header has Material icon + title via `HorizontalStackLayout`
- [ ] Zero hardcoded hex color values in XAML
- [ ] Zero `Color.FromArgb()` calls in ViewModel
- [ ] Zero MAUI `<Button>` usage — use `<gl:GLButton>` with `Style="{StaticResource ActionButton}"` for utility buttons
- [ ] All theme-adaptive colors use `toolkit:AppThemeResource {Token}`
- [ ] All icons use `x:Static constants:MaterialOutlined.*`
- [ ] `MaximumWidthRequest="960"` on content wrapper
- [ ] `BackgroundColor="{toolkit:AppThemeResource PageBg}"` on ContentPage
- [ ] `x:DataType` set on ContentPage for compiled bindings
- [ ] Looks correct in both Light and Dark mode
- [ ] Responsive layout works on phone (single column) and desktop (multi-column)
- [ ] Static demo data defined in XAML `x:Array`, not ViewModel
- [ ] ViewModel only contains properties/commands for dynamic behavior
- [ ] Registered in AppShell.xaml, MainPage.xaml, and MainViewModel.cs

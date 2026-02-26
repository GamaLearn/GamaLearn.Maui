# KeyboardHelper

Static helper for controlling and monitoring the soft keyboard on mobile devices.

**Namespace:** `GamaLearn.Helpers`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

KeyboardHelper provides platform-specific methods to show/hide the soft keyboard, get its height, and monitor keyboard visibility changes. Essential for creating responsive UIs that adapt to keyboard appearance, especially for forms and chat applications.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Methods

### ShowKeyboard(View view)

Shows the soft keyboard for the specified view.

**Parameters:**
- `view` - The view to show the keyboard for (typically an Entry or Editor)

**Returns:** `bool` - True if the keyboard was shown successfully

```csharp
using GamaLearn.Helpers;

bool shown = KeyboardHelper.ShowKeyboard(myEntry);
```

### HideKeyboard(View? view = null)

Hides the soft keyboard.

**Parameters:**
- `view` - The view that currently has the keyboard (optional)

**Returns:** `bool` - True if the keyboard was hidden successfully

```csharp
KeyboardHelper.HideKeyboard();

// Or specify the focused view
KeyboardHelper.HideKeyboard(myEntry);
```

### GetKeyboardHeight()

Gets the current height of the soft keyboard.

**Returns:** `double` - Keyboard height in device-independent units, or 0 if not visible

```csharp
double height = KeyboardHelper.GetKeyboardHeight();
```

### IsKeyboardVisible()

Gets whether the soft keyboard is currently visible.

**Returns:** `bool` - True if keyboard is visible

```csharp
if (KeyboardHelper.IsKeyboardVisible())
{
    // Keyboard is showing
}
```

---

## Events

### KeyboardShown

Raised when the keyboard is shown.

**Event Args:** `KeyboardEventArgs`
- `Height` - The keyboard height in device-independent units

```csharp
KeyboardHelper.KeyboardShown += (sender, e) =>
{
    Debug.WriteLine($"Keyboard shown with height: {e.Height}");
    AdjustLayoutForKeyboard(e.Height);
};
```

### KeyboardHidden

Raised when the keyboard is hidden.

```csharp
KeyboardHelper.KeyboardHidden += (sender, e) =>
{
    Debug.WriteLine("Keyboard hidden");
    RestoreLayout();
};
```

---

## Platform-Specific Setup

### Android

**IMPORTANT:** On Android, you must call `StartMonitoring` to enable keyboard height detection and events:

```csharp
using GamaLearn.Helpers;
using Microsoft.Maui.Platform;

public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Start monitoring keyboard on Android
        var rootView = Window?.DecorView?.RootView;
        if (rootView is not null)
        {
            KeyboardHelper.StartMonitoring(rootView);
        }
    }

    protected override void OnDestroy()
    {
        KeyboardHelper.StopMonitoring();
        base.OnDestroy();
    }
}
```

### iOS / MacCatalyst

Keyboard events are automatically monitored. No additional setup required.

### Windows

Keyboard management on Windows uses focus management. The on-screen keyboard is controlled by the OS.

---

## Usage Examples

### Auto-Scroll Chat Input

```csharp
public partial class ChatPage : ContentPage
{
    public ChatPage()
    {
        InitializeComponent();

        KeyboardHelper.KeyboardShown += OnKeyboardShown;
        KeyboardHelper.KeyboardHidden += OnKeyboardHidden;
    }

    private void OnKeyboardShown(object? sender, KeyboardEventArgs e)
    {
        // Scroll to bottom when keyboard appears
        var messagesList = this.FindByName<CollectionView>("MessagesList");
        messagesList.ScrollTo(messagesList.ItemsSource.Cast<object>().LastOrDefault());

        // Add padding to avoid keyboard overlap
        var inputLayout = this.FindByName<Grid>("InputLayout");
        inputLayout.TranslateTo(0, -e.Height, 250, Easing.CubicOut);
    }

    private void OnKeyboardHidden(object? sender, EventArgs e)
    {
        // Restore layout
        var inputLayout = this.FindByName<Grid>("InputLayout");
        inputLayout.TranslateTo(0, 0, 250, Easing.CubicOut);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        KeyboardHelper.KeyboardShown -= OnKeyboardShown;
        KeyboardHelper.KeyboardHidden -= OnKeyboardHidden;
    }
}
```

### Dismiss on Tap Outside

```csharp
public partial class FormPage : ContentPage
{
    public FormPage()
    {
        InitializeComponent();

        // Add tap gesture to background
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnBackgroundTapped;

        var background = this.FindByName<Grid>("BackgroundGrid");
        background.GestureRecognizers.Add(tapGesture);
    }

    private void OnBackgroundTapped(object? sender, EventArgs e)
    {
        // Hide keyboard when tapping outside input fields
        KeyboardHelper.HideKeyboard();
    }
}
```

### Form Navigation

```csharp
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void OnUsernameCompleted(object sender, EventArgs e)
    {
        // Move to password field
        PasswordEntry.Focus();
    }

    private void OnPasswordCompleted(object sender, EventArgs e)
    {
        // Hide keyboard and submit
        KeyboardHelper.HideKeyboard();
        LoginCommand.Execute(null);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        KeyboardHelper.HideKeyboard();
        Navigation.PopAsync();
    }
}
```

### Adaptive Layout

```csharp
public class AdaptiveFormViewModel : ObservableObject
{
    private double contentMargin;
    public double ContentMargin
    {
        get => contentMargin;
        set => SetProperty(ref contentMargin, value);
    }

    public AdaptiveFormViewModel()
    {
        KeyboardHelper.KeyboardShown += OnKeyboardShown;
        KeyboardHelper.KeyboardHidden += OnKeyboardHidden;
    }

    private void OnKeyboardShown(object? sender, KeyboardEventArgs e)
    {
        // Push content up to avoid keyboard
        ContentMargin = -e.Height / 2;
    }

    private void OnKeyboardHidden(object? sender, EventArgs e)
    {
        ContentMargin = 0;
    }
}
```

```xml
<ContentPage>
    <ScrollView Margin="0,{Binding ContentMargin},0,0">
        <VerticalStackLayout>
            <Entry Placeholder="Username" />
            <Entry Placeholder="Password" IsPassword="True" />
            <Button Text="Login" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

### Search Bar Focus

```csharp
public class SearchViewModel : ObservableObject
{
    [RelayCommand]
    private void OpenSearch()
    {
        // Show search bar and keyboard
        IsSearchVisible = true;

        // Wait for UI to render, then show keyboard
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100);
            var searchEntry = GetSearchEntry();
            if (searchEntry is not null)
            {
                KeyboardHelper.ShowKeyboard(searchEntry);
            }
        });
    }

    [RelayCommand]
    private void CloseSearch()
    {
        KeyboardHelper.HideKeyboard();
        IsSearchVisible = false;
    }
}
```

### Comment Input

```csharp
public partial class PostDetailPage : ContentPage
{
    public PostDetailPage()
    {
        InitializeComponent();
    }

    private void OnAddCommentClicked(object sender, EventArgs e)
    {
        // Show comment input and keyboard
        CommentInput.IsVisible = true;
        CommentInput.Focus();
        KeyboardHelper.ShowKeyboard(CommentInput);
    }

    private async void OnSubmitCommentClicked(object sender, EventArgs e)
    {
        // Submit comment
        await SubmitCommentAsync(CommentInput.Text);

        // Hide keyboard and input
        KeyboardHelper.HideKeyboard();
        CommentInput.Text = string.Empty;
        CommentInput.IsVisible = false;
    }
}
```

### Modal Input Dialog

```csharp
public class InputDialogPage : ContentPage
{
    private readonly TaskCompletionSource<string?> tcs = new();

    public Task<string?> GetInputAsync() => tcs.Task;

    public InputDialogPage(string title, string placeholder)
    {
        Title = title;

        var entry = new Entry { Placeholder = placeholder };
        var submitButton = new Button { Text = "Submit" };
        var cancelButton = new Button { Text = "Cancel" };

        submitButton.Clicked += (s, e) =>
        {
            KeyboardHelper.HideKeyboard();
            tcs.TrySetResult(entry.Text);
        };

        cancelButton.Clicked += (s, e) =>
        {
            KeyboardHelper.HideKeyboard();
            tcs.TrySetResult(null);
        };

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children = { entry, submitButton, cancelButton }
        };

        // Show keyboard when page appears
        Appearing += (s, e) =>
        {
            entry.Focus();
            KeyboardHelper.ShowKeyboard(entry);
        };
    }
}

// Usage:
var dialog = new InputDialogPage("Enter Name", "Your name");
await Navigation.PushModalAsync(dialog);
string? result = await dialog.GetInputAsync();
await Navigation.PopModalAsync();
```

---

## Best Practices

### 1. Unsubscribe from Events

```csharp
✅ Good:
protected override void OnAppearing()
{
    base.OnAppearing();
    KeyboardHelper.KeyboardShown += OnKeyboardShown;
}

protected override void OnDisappearing()
{
    base.OnDisappearing();
    KeyboardHelper.KeyboardShown -= OnKeyboardShown;
}

❌ Avoid:
// Subscribing without unsubscribing (memory leak)
KeyboardHelper.KeyboardShown += OnKeyboardShown;
```

### 2. Android Setup Required

```csharp
✅ Good (Android):
// In MainActivity
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    var rootView = Window?.DecorView?.RootView;
    if (rootView is not null)
    {
        KeyboardHelper.StartMonitoring(rootView);
    }
}

❌ Avoid:
// Not calling StartMonitoring on Android
// Keyboard events won't work!
```

### 3. Wait for View to Render

```csharp
✅ Good:
protected override async void OnAppearing()
{
    base.OnAppearing();
    await Task.Delay(100); // Let UI render
    KeyboardHelper.ShowKeyboard(myEntry);
}

❌ Avoid:
public MyPage()
{
    InitializeComponent();
    KeyboardHelper.ShowKeyboard(myEntry); // Too early, view not ready
}
```

---

## Platform Behavior

### iOS / MacCatalyst
- Automatic keyboard monitoring via `NSNotification`
- Keyboard height includes safe area insets
- Events fire on `UIKeyboard.WillShow/WillHide`

### Android
- Requires `StartMonitoring(rootView)` call in MainActivity
- Uses `WindowInsetsCompat` to detect keyboard
- Height excludes safe area insets

### Windows
- Focus management only
- Keyboard height always returns 0
- On-screen keyboard managed by Windows OS

### Tizen
- Placeholder implementation
- Keyboard control not currently supported

---

## Troubleshooting

### Events Not Firing (Android)

Make sure you called `StartMonitoring`:

```csharp
// In MainActivity.OnCreate
var rootView = Window?.DecorView?.RootView;
if (rootView is not null)
{
    KeyboardHelper.StartMonitoring(rootView);
}
```

### ShowKeyboard Not Working

Ensure the view has focus and is rendered:

```csharp
protected override async void OnAppearing()
{
    base.OnAppearing();
    myEntry.Focus();
    await Task.Delay(100);
    KeyboardHelper.ShowKeyboard(myEntry);
}
```

### Height Always Zero

On iOS, keyboard height is only available after `KeyboardShown` event fires. On Windows, keyboard height is not available.

---

## See Also

- [SafeAreaHelper](SafeAreaHelper.md) - Safe area insets
- [DeviceInfoService](DeviceInfoService.md) - Device information
- [DispatchHelper](DispatchHelper.md) - UI thread helpers

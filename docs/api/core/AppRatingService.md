# AppRatingService

Cross-platform app rating service for prompting users to rate your app on their respective app stores. Intelligently manages when and how often to show rating prompts based on configurable conditions.

**Namespace:** `GamaLearn.Services`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

AppRatingService provides a non-intrusive way to ask users to rate your app. It automatically tracks:
- Days since first launch
- Number of app launches
- Days between prompts
- Total prompt count
- User preferences (declined permanently)

The service uses platform-native APIs for the best user experience:
- **iOS**: `SKStoreReviewController` (system-controlled dialog)
- **Android**: Google Play In-App Review API
- **Windows**: Opens Microsoft Store rating page
- **MacCatalyst**: Opens App Store rating page

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Setup

### Registration

Register the service in your `MauiProgram.cs`:

```csharp
using GamaLearn.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        // Register service
        builder.Services.AddSingleton<IAppRatingService, AppRatingService>();

        // Or use extension method with options
        builder.Services.AddGamaLearnCore(options =>
        {
            options.AppRatingOptions = new AppRatingOptions
            {
                MinimumDaysSinceFirstLaunch = 7,
                MinimumDaysBetweenPrompts = 30,
                MaximumPromptCount = 3,
                MinimumLaunchCount = 10
            };
        });

        return builder.Build();
    }
}
```

### Configuration

```csharp
public class AppRatingOptions
{
    /// <summary>
    /// Minimum days since first app launch before prompting.
    /// Default: 7 days
    /// </summary>
    public int MinimumDaysSinceFirstLaunch { get; set; } = 7;

    /// <summary>
    /// Minimum days between prompts.
    /// Default: 30 days
    /// </summary>
    public int MinimumDaysBetweenPrompts { get; set; } = 30;

    /// <summary>
    /// Maximum number of times to prompt the user.
    /// Default: 3
    /// </summary>
    public int MaximumPromptCount { get; set; } = 3;

    /// <summary>
    /// Whether to track app launches.
    /// Default: true
    /// </summary>
    public bool TrackAppLaunches { get; set; } = true;

    /// <summary>
    /// Minimum number of launches before prompting.
    /// Default: 10
    /// </summary>
    public int MinimumLaunchCount { get; set; } = 10;
}
```

---

## Methods

### TryPromptForRatingAsync(bool isSignificantEvent = false)

Attempts to prompt the user for a rating based on configured conditions.

**Parameters:**
- `isSignificantEvent` - If true, bypasses the time-based check (but still respects prompt limits and user preferences)

**Returns:** `Task<bool>` - True if the rating prompt was shown, false otherwise

**Conditions Checked:**
1. User hasn't declined permanently
2. Maximum prompt count not exceeded
3. Minimum days since first launch (unless significant event)
4. Minimum days between prompts
5. Minimum launch count met

```csharp
// Regular prompt (checks all conditions)
bool shown = await ratingService.TryPromptForRatingAsync();

// After significant event (bypasses time check)
bool shown = await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
```

### OpenStoreRatingAsync()

Opens the app store rating page directly without any checks or dialogs. Use this for manual "Rate Us" buttons in settings.

**Returns:** `Task<bool>` - True if the store was opened successfully

```csharp
await ratingService.OpenStoreRatingAsync();
```

### ResetRatingData()

Resets all rating tracking data (prompt count, dates, user preferences). Useful for testing or when user requests to be asked again.

```csharp
ratingService.ResetRatingData();
```

---

## Properties

### HasUserDeclinedPermanently

**Type:** `bool` (read-only)

Gets whether the user has permanently declined rating prompts.

```csharp
if (ratingService.HasUserDeclinedPermanently)
{
    // Hide "Rate Us" button
}
```

### PromptCount

**Type:** `int` (read-only)

Gets the number of times the user has been prompted.

```csharp
int count = ratingService.PromptCount;
```

### LastPromptDate

**Type:** `DateTime?` (read-only)

Gets the date of the last rating prompt, or null if never prompted.

```csharp
DateTime? lastPrompt = ratingService.LastPromptDate;
```

---

## Usage Examples

### Basic Usage

```csharp
public class MainViewModel
{
    private readonly IAppRatingService ratingService;

    public MainViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public async Task OnAppStarted()
    {
        // Try to show rating prompt on app start
        bool shown = await ratingService.TryPromptForRatingAsync();

        if (shown)
        {
            Debug.WriteLine("Rating prompt shown");
        }
    }
}
```

### After Significant Event

```csharp
public class TaskViewModel
{
    private readonly IAppRatingService ratingService;

    public TaskViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public async Task OnTaskCompleted()
    {
        // Save task...
        await SaveTaskAsync();

        // Prompt for rating after completing a significant task
        await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
    }
}
```

### Manual "Rate Us" Button

```csharp
public class SettingsViewModel
{
    private readonly IAppRatingService ratingService;

    public SettingsViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public ICommand RateAppCommand => new Command(async () =>
    {
        // Open store directly
        bool success = await ratingService.OpenStoreRatingAsync();

        if (!success)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Unable to open app store",
                "OK");
        }
    });

    // Hide button if user permanently declined
    public bool ShowRateButton => !ratingService.HasUserDeclinedPermanently;
}
```

```xml
<Button Text="Rate This App"
        Command="{Binding RateAppCommand}"
        IsVisible="{Binding ShowRateButton}" />
```

### Strategic Timing

```csharp
public class GameViewModel
{
    private readonly IAppRatingService ratingService;
    private int levelsCompleted;

    public GameViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public async Task OnLevelCompleted(int level)
    {
        levelsCompleted++;

        // Prompt after every 5 levels
        if (levelsCompleted % 5 == 0)
        {
            await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
        }
    }

    public async Task OnHighScoreAchieved()
    {
        // Prompt when user achieves personal best
        await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
    }
}
```

### App Lifecycle Integration

```csharp
public class App : Application
{
    private readonly IAppRatingService ratingService;

    public App(IAppRatingService ratingService)
    {
        InitializeComponent();
        this.ratingService = ratingService;

        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Prompt after app has been used for a while
        await Task.Delay(TimeSpan.FromSeconds(5));
        await ratingService.TryPromptForRatingAsync();
    }

    protected override void OnResume()
    {
        base.OnResume();

        // Could also prompt on resume (but be careful not to be annoying)
    }
}
```

### Purchase Completion

```csharp
public class CheckoutViewModel
{
    private readonly IAppRatingService ratingService;

    public CheckoutViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public async Task OnPurchaseCompleted()
    {
        // Show success message
        await Shell.Current.DisplayAlert(
            "Success",
            "Your order has been placed!",
            "OK");

        // Prompt for rating after successful purchase
        await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
    }
}
```

### Achievement Unlocked

```csharp
public class AchievementService
{
    private readonly IAppRatingService ratingService;

    public AchievementService(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public async Task UnlockAchievement(Achievement achievement)
    {
        // Save achievement...
        await SaveAchievementAsync(achievement);

        // Show achievement notification...
        ShowNotification(achievement);

        // Prompt for rating if it's a major achievement
        if (achievement.IsMajor)
        {
            await Task.Delay(TimeSpan.FromSeconds(2)); // Wait for notification to dismiss
            await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
        }
    }
}
```

### Testing and Debugging

```csharp
#if DEBUG
public class DebugSettingsViewModel
{
    private readonly IAppRatingService ratingService;

    public DebugSettingsViewModel(IAppRatingService ratingService)
    {
        this.ratingService = ratingService;
    }

    public ICommand ResetRatingDataCommand => new Command(() =>
    {
        ratingService.ResetRatingData();
        Debug.WriteLine("Rating data reset");
    });

    public ICommand ForcePromptCommand => new Command(async () =>
    {
        await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
    });

    public string PromptInfo =>
        $"Prompted {ratingService.PromptCount} times\n" +
        $"Last prompt: {ratingService.LastPromptDate?.ToString() ?? "Never"}\n" +
        $"Declined: {ratingService.HasUserDeclinedPermanently}";
}
#endif
```

---

## Platform-Specific Behavior

### iOS

Uses `SKStoreReviewController.RequestReview()`:
- System-controlled dialog (respects user's App Store settings)
- Limited to 3 prompts per year per device (enforced by iOS)
- Dialog may not appear if user recently saw it in another app
- No way to detect if user actually rated

```csharp
// iOS automatically manages prompt frequency
await ratingService.TryPromptForRatingAsync();
```

### Android

Uses Google Play In-App Review API:
- Shows in-app review dialog
- User can rate without leaving the app
- Respects Google Play's quotas
- Works only if app is installed from Google Play

```csharp
// Android shows in-app review flow
await ratingService.TryPromptForRatingAsync();
```

### Windows

Opens Microsoft Store rating page:
- Launches Microsoft Store app
- Navigates to app's rating page
- User leaves the app to rate

```csharp
// Opens store app
await ratingService.OpenStoreRatingAsync();
```

### MacCatalyst

Opens App Store rating page:
- Opens App Store app or web page
- Navigates to app's rating page

```csharp
// Opens App Store
await ratingService.OpenStoreRatingAsync();
```

---

## Best Practices

### 1. Don't Be Annoying

✅ Good timing:
- After completing a meaningful task
- After achieving something significant
- When user has used app for a while
- At natural pause points

❌ Bad timing:
- Immediately on app start
- During active use
- After errors or failures
- Too frequently

### 2. Use Significant Events

Mark truly meaningful moments as significant events:

```csharp
✅ Good:
await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
// After: Purchase, Level completion, Achievement unlocked

❌ Bad:
await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
// After: Button click, Page view, Minor interaction
```

### 3. Configure Appropriately

Adjust settings based on your app's usage patterns:

```csharp
// For frequently-used apps
new AppRatingOptions
{
    MinimumDaysSinceFirstLaunch = 3,
    MinimumLaunchCount = 5
};

// For occasionally-used apps
new AppRatingOptions
{
    MinimumDaysSinceFirstLaunch = 14,
    MinimumLaunchCount = 3
};
```

### 4. Provide Manual Option

Always offer a "Rate Us" button in settings:

```csharp
public ICommand RateCommand => new Command(async () =>
{
    await ratingService.OpenStoreRatingAsync();
});
```

### 5. Respect User Preferences

Check if user declined before showing rating reminders:

```csharp
if (!ratingService.HasUserDeclinedPermanently)
{
    // Show subtle reminder in UI
}
```

---

## Common Scenarios

### 1. E-commerce App

```csharp
// Prompt after successful purchase
public async Task OnOrderCompleted()
{
    await SaveOrderAsync();
    await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
}
```

### 2. Game

```csharp
// Prompt after completing levels
public async Task OnLevelComplete()
{
    if (currentLevel % 10 == 0)
    {
        await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
    }
}
```

### 3. Productivity App

```csharp
// Prompt after using app for a week
public async Task CheckRatingPrompt()
{
    // Regular check, respects all conditions
    await ratingService.TryPromptForRatingAsync();
}
```

### 4. Social App

```csharp
// Prompt after positive interactions
public async Task OnContentShared()
{
    await ShareContentAsync();
    await ratingService.TryPromptForRatingAsync(isSignificantEvent: true);
}
```

---

## Troubleshooting

### Prompt Not Showing

Check these conditions:
1. User hasn't declined permanently
2. Maximum prompt count not exceeded (check `PromptCount`)
3. Enough days since first launch
4. Enough days since last prompt
5. Minimum launch count met

```csharp
// Debug information
Debug.WriteLine($"Prompt count: {ratingService.PromptCount}");
Debug.WriteLine($"Last prompt: {ratingService.LastPromptDate}");
Debug.WriteLine($"Declined: {ratingService.HasUserDeclinedPermanently}");
```

### iOS Prompt Not Appearing

iOS strictly limits prompts (3 per year). The system may suppress the prompt even if your app requests it.

### Testing

Reset data between tests:

```csharp
#if DEBUG
ratingService.ResetRatingData();
#endif
```

---

## See Also

- [Guard](Guard.md) - Argument validation
- [Debouncer](Debouncer.md) - Debounce operations
- [ObservableRangeCollection](ObservableRangeCollection.md) - Efficient collections

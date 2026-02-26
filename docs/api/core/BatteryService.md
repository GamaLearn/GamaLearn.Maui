# BatteryService

Cross-platform service for monitoring device battery information and power state.

**Namespace:** `GamaLearn.Services`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

BatteryService provides a clean, testable wrapper around .NET MAUI's battery APIs. It monitors battery charge level, charging state, power source, and energy saver mode with automatic event notifications.

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
using GamaLearn;
using GamaLearn.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        // Register Battery Service
        builder.Services.AddBatteryService();

        // Or register all core services at once
        builder.Services.AddGamaLearnCoreServices();

        return builder.Build();
    }
}
```

---

## Properties

### ChargeLevel

**Type:** `double` (read-only)

Gets the current battery charge level as a percentage (0.0 to 1.0).

```csharp
double charge = batteryService.ChargeLevel; // 0.75 = 75%
```

### State

**Type:** `BatteryState` (read-only)

Gets the current battery state.

**Possible Values:**
- `BatteryState.Charging` - Battery is charging
- `BatteryState.Discharging` - Battery is discharging (on battery power)
- `BatteryState.Full` - Battery is fully charged
- `BatteryState.NotCharging` - Not charging but plugged in
- `BatteryState.Unknown` - State cannot be determined

```csharp
if (batteryService.State == BatteryState.Charging)
{
    // Device is charging
}
```

### PowerSource

**Type:** `BatteryPowerSource` (read-only)

Gets the power source being used.

**Possible Values:**
- `BatteryPowerSource.Battery` - Running on battery
- `BatteryPowerSource.AC` - Plugged into AC power
- `BatteryPowerSource.Usb` - Plugged into USB
- `BatteryPowerSource.Wireless` - Wireless charging
- `BatteryPowerSource.Unknown` - Unknown power source

```csharp
if (batteryService.PowerSource == BatteryPowerSource.Battery)
{
    // Running on battery, consider power-saving measures
}
```

### EnergySaverStatus

**Type:** `bool` (read-only)

Gets whether the device is in energy saver (low-power) mode.

```csharp
if (batteryService.EnergySaverStatus)
{
    // Reduce background operations
}
```

---

## Methods

### StartMonitoring()

Starts monitoring battery changes. Call this to begin receiving battery change events.

```csharp
batteryService.StartMonitoring();
```

### StopMonitoring()

Stops monitoring battery changes. Call this when you no longer need updates to save resources.

```csharp
batteryService.StopMonitoring();
```

---

## Events

### BatteryInfoChanged

Occurs when the battery charge level, state, or power source changes.

**Event Args:** `BatteryInfoChangedEventArgs`
- `ChargeLevel` - New charge level (0.0 to 1.0)
- `State` - New battery state
- `PowerSource` - New power source

```csharp
batteryService.BatteryInfoChanged += (sender, e) =>
{
    Debug.WriteLine($"Battery: {e.ChargeLevel:P0}, State: {e.State}, Power: {e.PowerSource}");
};
```

### EnergySaverStatusChanged

Occurs when energy saver mode is toggled.

**Event Args:** `EnergySaverStatusChangedEventArgs`
- `IsEnergySaverOn` - Whether energy saver is now enabled

```csharp
batteryService.EnergySaverStatusChanged += (sender, e) =>
{
    if (e.IsEnergySaverOn)
    {
        // Reduce animations, background tasks, etc.
    }
};
```

---

## Usage Examples

### Basic Usage

```csharp
public class MainViewModel
{
    private readonly IBatteryService batteryService;

    public MainViewModel(IBatteryService batteryService)
    {
        this.batteryService = batteryService;

        // Start monitoring
        batteryService.StartMonitoring();

        // Subscribe to events
        batteryService.BatteryInfoChanged += OnBatteryInfoChanged;
        batteryService.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
    }

    private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        Debug.WriteLine($"Battery level: {e.ChargeLevel:P0}");

        if (e.ChargeLevel < 0.2)
        {
            ShowLowBatteryWarning();
        }
    }

    private void OnEnergySaverStatusChanged(object? sender, EnergySaverStatusChangedEventArgs e)
    {
        if (e.IsEnergySaverOn)
        {
            ReduceBackgroundActivity();
        }
    }
}
```

### Battery Indicator

```csharp
public class BatteryIndicatorViewModel : ObservableObject
{
    private readonly IBatteryService batteryService;

    [ObservableProperty]
    private double chargeLevel;

    [ObservableProperty]
    private string batteryIcon = "battery_full";

    [ObservableProperty]
    private Color batteryColor = Colors.Green;

    public BatteryIndicatorViewModel(IBatteryService batteryService)
    {
        this.batteryService = batteryService;

        batteryService.StartMonitoring();
        batteryService.BatteryInfoChanged += OnBatteryInfoChanged;

        UpdateBatteryUI();
    }

    private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        ChargeLevel = e.ChargeLevel;
        UpdateBatteryUI();
    }

    private void UpdateBatteryUI()
    {
        BatteryIcon = ChargeLevel switch
        {
            >= 0.9 => "battery_full",
            >= 0.6 => "battery_6_bar",
            >= 0.4 => "battery_4_bar",
            >= 0.2 => "battery_2_bar",
            _ => "battery_alert"
        };

        BatteryColor = ChargeLevel switch
        {
            >= 0.3 => Colors.Green,
            >= 0.2 => Colors.Orange,
            _ => Colors.Red
        };
    }
}
```

### Power-Aware Operations

```csharp
public class SyncService
{
    private readonly IBatteryService batteryService;
    private readonly ILogger<SyncService> logger;

    public SyncService(IBatteryService batteryService, ILogger<SyncService> logger)
    {
        this.batteryService = batteryService;
        this.logger = logger;
    }

    public async Task<bool> SyncDataAsync()
    {
        // Check battery conditions before syncing
        if (!ShouldSync())
        {
            logger.LogInformation("Skipping sync due to battery conditions");
            return false;
        }

        logger.LogInformation("Starting data sync");
        await PerformSyncAsync();
        return true;
    }

    private bool ShouldSync()
    {
        // Don't sync if battery is very low
        if (batteryService.ChargeLevel < 0.15)
        {
            logger.LogInformation("Battery too low for sync: {ChargeLevel:P0}",
                batteryService.ChargeLevel);
            return false;
        }

        // Don't sync if in energy saver mode (unless charging)
        if (batteryService.EnergySaverStatus &&
            batteryService.State != BatteryState.Charging)
        {
            logger.LogInformation("Energy saver mode enabled, skipping sync");
            return false;
        }

        // Prefer to sync when plugged in
        if (batteryService.PowerSource != BatteryPowerSource.Battery)
        {
            logger.LogInformation("Device is plugged in, safe to sync");
            return true;
        }

        // Only sync on battery if charge is adequate
        return batteryService.ChargeLevel >= 0.5;
    }

    private async Task PerformSyncAsync()
    {
        // Your sync logic here
        await Task.Delay(1000);
    }
}
```

### Download Manager

```csharp
public class DownloadManager
{
    private readonly IBatteryService batteryService;
    private bool isDownloading;

    public DownloadManager(IBatteryService batteryService)
    {
        this.batteryService = batteryService;

        batteryService.StartMonitoring();
        batteryService.BatteryInfoChanged += OnBatteryInfoChanged;
    }

    public async Task StartDownloadAsync(string url)
    {
        if (!CanDownload())
        {
            throw new InvalidOperationException("Cannot download: battery conditions not met");
        }

        isDownloading = true;
        await PerformDownloadAsync(url);
        isDownloading = false;
    }

    private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        // Pause downloads if battery gets too low
        if (isDownloading && e.ChargeLevel < 0.1)
        {
            PauseDownload();
        }
    }

    private bool CanDownload()
    {
        // Large downloads only when charging or battery > 30%
        return batteryService.State == BatteryState.Charging ||
               batteryService.ChargeLevel > 0.3;
    }

    private async Task PerformDownloadAsync(string url)
    {
        // Download logic
        await Task.Delay(1000);
    }

    private void PauseDownload()
    {
        // Pause logic
    }
}
```

### Settings Page

```csharp
public class SettingsViewModel : ObservableObject
{
    private readonly IBatteryService batteryService;

    public string BatteryInfo => $"{batteryService.ChargeLevel:P0} - {batteryService.State}";
    public string PowerSource => batteryService.PowerSource.ToString();
    public bool IsEnergySaverOn => batteryService.EnergySaverStatus;

    public SettingsViewModel(IBatteryService batteryService)
    {
        this.batteryService = batteryService;

        batteryService.StartMonitoring();
        batteryService.BatteryInfoChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(BatteryInfo));
            OnPropertyChanged(nameof(PowerSource));
        };

        batteryService.EnergySaverStatusChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsEnergySaverOn));
        };
    }
}
```

```xml
<VerticalStackLayout Padding="16">
    <Label Text="Device Status" FontSize="20" FontAttributes="Bold" />

    <Label Text="Battery Level:" />
    <Label Text="{Binding BatteryInfo}" FontAttributes="Bold" />

    <Label Text="Power Source:" />
    <Label Text="{Binding PowerSource}" FontAttributes="Bold" />

    <Label Text="Energy Saver:" />
    <Label Text="{Binding IsEnergySaverOn}" FontAttributes="Bold" />
</VerticalStackLayout>
```

### Background Task Scheduler

```csharp
public class BackgroundTaskScheduler
{
    private readonly IBatteryService batteryService;
    private Timer? scheduledTask;

    public BackgroundTaskScheduler(IBatteryService batteryService)
    {
        this.batteryService = batteryService;
        batteryService.StartMonitoring();
    }

    public void ScheduleTask(Func<Task> task, TimeSpan interval)
    {
        scheduledTask?.Dispose();

        scheduledTask = new Timer(async _ =>
        {
            if (ShouldRunTask())
            {
                await task();
            }
        }, null, interval, interval);
    }

    private bool ShouldRunTask()
    {
        // Don't run background tasks in low battery or energy saver mode
        if (batteryService.ChargeLevel < 0.2 || batteryService.EnergySaverStatus)
        {
            return false;
        }

        return true;
    }

    public void Dispose()
    {
        scheduledTask?.Dispose();
        batteryService.StopMonitoring();
    }
}
```

---

## Best Practices

### 1. Always Stop Monitoring

```csharp
✅ Good:
public class MyViewModel : IDisposable
{
    public MyViewModel(IBatteryService batteryService)
    {
        this.batteryService = batteryService;
        batteryService.StartMonitoring();
    }

    public void Dispose()
    {
        batteryService.StopMonitoring();
    }
}

❌ Avoid:
// Starting monitoring without ever stopping (memory leak)
batteryService.StartMonitoring();
```

### 2. Respect Energy Saver Mode

```csharp
✅ Good:
if (batteryService.EnergySaverStatus)
{
    // Reduce animations, polling, background tasks
    DisableNonEssentialFeatures();
}

❌ Avoid:
// Ignoring energy saver mode and draining battery
PerformExpensiveOperations();
```

### 3. Use Dependency Injection

```csharp
✅ Good:
public class MyService
{
    private readonly IBatteryService batteryService;

    public MyService(IBatteryService batteryService)
    {
        this.batteryService = batteryService;
    }
}

❌ Avoid:
// Direct instantiation, not testable
var batteryService = new BatteryService();
```

---

## Platform Notes

### Android

- Requires `BatteryStats` permission (automatically included)
- Battery state updates immediately
- Energy saver mode reflects system power saving settings

### iOS

- Battery monitoring is enabled automatically
- Low power mode is reflected in `EnergySaverStatus`
- Battery level updates every 1% change

### Windows

- Desktop Windows may always report `ChargeLevel = 1.0` on devices without batteries
- Laptops report accurate battery information

### MacCatalyst

- Similar to iOS behavior
- Battery information available on MacBooks

---

## Troubleshooting

### Battery Level Always 1.0

This is expected on desktop devices without batteries (Windows desktops, some development machines).

### Events Not Firing

Make sure you called `StartMonitoring()`:

```csharp
batteryService.StartMonitoring();
batteryService.BatteryInfoChanged += OnBatteryInfoChanged;
```

### Memory Leaks

Always call `StopMonitoring()` or implement `IDisposable`:

```csharp
public void Dispose()
{
    batteryService.BatteryInfoChanged -= OnBatteryInfoChanged;
    batteryService.StopMonitoring();
}
```

---

## See Also

- [DeviceInfoService](DeviceInfoService.md) - Device information
- [AppRatingService](AppRatingService.md) - In-app ratings
- [Guard](Guard.md) - Argument validation

# DeviceInfoService

Cross-platform service for accessing device information and characteristics.

**Namespace:** `GamaLearn.Services`
**Assembly:** GamaLearn.Maui.Core

---

## Overview

DeviceInfoService provides a clean, testable wrapper around .NET MAUI's device information APIs. It exposes device model, manufacturer, platform, idiom, and other characteristics useful for analytics, debugging, and feature detection.

---

## Installation

```bash
dotnet add package GamaLearn.Maui.Core
```

---

## Setup

### Registration

```csharp
using GamaLearn;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Register Device Info Service
        builder.Services.AddDeviceInfoService();

        // Or register all core services
        builder.Services.AddGamaLearnCoreServices();

        return builder.Build();
    }
}
```

---

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Model` | `string` | Device model (e.g., "iPhone 14 Pro", "Pixel 7") |
| `Manufacturer` | `string` | Device manufacturer (e.g., "Apple", "Samsung") |
| `Name` | `string` | User-set device name (e.g., "John's iPhone") |
| `VersionString` | `string` | OS version (e.g., "17.0", "13.0") |
| `Platform` | `DevicePlatform` | Platform (iOS, Android, WinUI, MacCatalyst, Tizen) |
| `Idiom` | `DeviceIdiom` | Device type (Phone, Tablet, Desktop, TV, Watch) |
| `DeviceType` | `DeviceType` | Physical or Virtual (emulator/simulator) |
| `IsPhysicalDevice` | `bool` | True if running on real hardware |
| `IsVirtualDevice` | `bool` | True if running in emulator/simulator |
| `IsPhone` | `bool` | True if device is a phone |
| `IsTablet` | `bool` | True if device is a tablet |
| `IsDesktop` | `bool` | True if device is a desktop |
| `DeviceId` | `string` | Generated device identifier (not stable across reinstalls) |

---

## Usage Examples

### Basic Usage

```csharp
public class MainViewModel
{
    private readonly IDeviceInfoService deviceInfo;
    private readonly ILogger logger;

    public MainViewModel(IDeviceInfoService deviceInfo, ILogger logger)
    {
        this.deviceInfo = deviceInfo;
        this.logger = logger;

        logger.LogInformation("Running on: {Manufacturer} {Model} ({Platform} {Version})",
            deviceInfo.Manufacturer,
            deviceInfo.Model,
            deviceInfo.Platform,
            deviceInfo.VersionString);
    }
}
```

### Platform-Specific Features

```csharp
public class FeatureManager
{
    private readonly IDeviceInfoService deviceInfo;

    public FeatureManager(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }

    public bool SupportsFaceID()
    {
        return deviceInfo.Platform == DevicePlatform.iOS &&
               deviceInfo.Model.Contains("iPhone") &&
               !deviceInfo.Model.Contains("iPhone 8");
    }

    public bool SupportsInAppReview()
    {
        return deviceInfo.Platform switch
        {
            DevicePlatform.iOS => true,
            DevicePlatform.Android => true,
            DevicePlatform.WinUI => true,
            _ => false
        };
    }

    public int GetRecommendedImageQuality()
    {
        // Lower quality on older/slower devices
        if (deviceInfo.Platform == DevicePlatform.Android)
        {
            return deviceInfo.Model.Contains("Galaxy S2") ? 60 : 85;
        }

        return 90;
    }
}
```

### Responsive Layout

```csharp
public class LayoutService
{
    private readonly IDeviceInfoService deviceInfo;

    public LayoutService(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }

    public int GetColumnCount()
    {
        return deviceInfo.Idiom switch
        {
            DeviceIdiom.Phone => 1,
            DeviceIdiom.Tablet => 2,
            DeviceIdiom.Desktop => 3,
            _ => 1
        };
    }

    public bool ShouldUseLargeControls()
    {
        return deviceInfo.IsTablet || deviceInfo.IsDesktop;
    }
}
```

### Analytics

```csharp
public class AnalyticsService
{
    private readonly IDeviceInfoService deviceInfo;

    public AnalyticsService(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }

    public void TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        properties ??= new();

        // Add device context
        properties["device_manufacturer"] = deviceInfo.Manufacturer;
        properties["device_model"] = deviceInfo.Model;
        properties["os_platform"] = deviceInfo.Platform.ToString();
        properties["os_version"] = deviceInfo.VersionString;
        properties["device_idiom"] = deviceInfo.Idiom.ToString();
        properties["is_physical_device"] = deviceInfo.IsPhysicalDevice;

        // Send to analytics service
        SendToAnalytics(eventName, properties);
    }

    private void SendToAnalytics(string eventName, Dictionary<string, object> properties)
    {
        // Your analytics implementation
    }
}
```

### Debug Information

```csharp
public class AboutViewModel : ObservableObject
{
    private readonly IDeviceInfoService deviceInfo;

    public string DeviceModel => deviceInfo.Model;
    public string Manufacturer => deviceInfo.Manufacturer;
    public string Platform => $"{deviceInfo.Platform} {deviceInfo.VersionString}";
    public string DeviceType => $"{deviceInfo.Idiom} ({deviceInfo.DeviceType})";
    public string DeviceId => deviceInfo.DeviceId;

    public AboutViewModel(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }
}
```

### Feature Flags

```csharp
public class FeatureFlagService
{
    private readonly IDeviceInfoService deviceInfo;

    public FeatureFlagService(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }

    public bool IsFeatureEnabled(string featureName)
    {
        return featureName switch
        {
            "advanced_camera" => deviceInfo.IsPhysicalDevice && !IsOldDevice(),
            "offline_mode" => true,
            "premium_animations" => !deviceInfo.IsVirtualDevice,
            "haptic_feedback" => deviceInfo.IsPhone,
            _ => false
        };
    }

    private bool IsOldDevice()
    {
        // Parse version and check if it's old
        if (double.TryParse(deviceInfo.VersionString.Split('.')[0], out double majorVersion))
        {
            return deviceInfo.Platform switch
            {
                DevicePlatform.iOS => majorVersion < 14,
                DevicePlatform.Android => majorVersion < 10,
                _ => false
            };
        }

        return false;
    }
}
```

---

## Best Practices

### 1. Use Dependency Injection

```csharp
✅ Good:
public class MyService
{
    private readonly IDeviceInfoService deviceInfo;

    public MyService(IDeviceInfoService deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }
}

❌ Avoid:
var deviceInfo = new DeviceInfoService(); // Not testable
```

### 2. Cache Device Info

```csharp
✅ Good:
// Service is registered as singleton, properties are cached
builder.Services.AddDeviceInfoService();

❌ Avoid:
// Repeatedly creating new instances
var info1 = new DeviceInfoService();
var info2 = new DeviceInfoService();
```

### 3. Don't Rely on DeviceId for Security

```csharp
✅ Good:
// Use DeviceId for analytics, debugging
logger.LogInformation("Device: {DeviceId}", deviceInfo.DeviceId);

❌ Avoid:
// Don't use for security, licensing, or permanent identification
string licenseKey = GenerateLicense(deviceInfo.DeviceId);
```

---

## Platform Notes

### iOS / MacCatalyst
- `Model` returns device model (e.g., "iPhone15,2")
- `Name` returns user-set device name
- `IsVirtualDevice` detects iOS Simulator

### Android
- `Model` returns manufacturer model name
- `IsVirtualDevice` detects Android Emulator
- `DeviceId` is generated, not the Android ID

### Windows
- `Idiom` is always `Desktop`
- `Manufacturer` may be PC manufacturer

---

## See Also

- [BatteryService](BatteryService.md) - Battery monitoring
- [SafeAreaHelper](SafeAreaHelper.md) - Safe area insets
- [KeyboardHelper](KeyboardHelper.md) - Keyboard management

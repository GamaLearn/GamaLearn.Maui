namespace GamaLearn.Services;

/// <summary>
/// Service for accessing device information and characteristics.
/// </summary>
public interface IDeviceInfoService
{
    /// <summary>
    /// Gets the model of the device (e.g., "iPhone 14 Pro", "Pixel 7").
    /// </summary>
    string Model { get; }

    /// <summary>
    /// Gets the manufacturer of the device (e.g., "Apple", "Samsung", "Google").
    /// </summary>
    string Manufacturer { get; }

    /// <summary>
    /// Gets the name of the device as set by the user (e.g., "John's iPhone").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the version string of the operating system (e.g., "17.0", "13.0").
    /// </summary>
    string VersionString { get; }

    /// <summary>
    /// Gets the platform/operating system (iOS, Android, WinUI, MacCatalyst, Tizen).
    /// </summary>
    DevicePlatform Platform { get; }

    /// <summary>
    /// Gets the device idiom (phone, tablet, desktop, TV, watch).
    /// </summary>
    DeviceIdiom Idiom { get; }

    /// <summary>
    /// Gets the device type (physical or virtual/emulator).
    /// </summary>
    DeviceType DeviceType { get; }

    /// <summary>
    /// Gets whether the app is running on a physical device.
    /// </summary>
    bool IsPhysicalDevice { get; }

    /// <summary>
    /// Gets whether the app is running in an emulator or simulator.
    /// </summary>
    bool IsVirtualDevice { get; }

    /// <summary>
    /// Gets whether the app is running on a phone.
    /// </summary>
    bool IsPhone { get; }

    /// <summary>
    /// Gets whether the app is running on a tablet.
    /// </summary>
    bool IsTablet { get; }

    /// <summary>
    /// Gets whether the app is running on a desktop.
    /// </summary>
    bool IsDesktop { get; }

    /// <summary>
    /// Gets a unique identifier for the device.
    /// Note: This is not a stable identifier and may change across app reinstalls.
    /// </summary>
    string DeviceId { get; }
}

using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

/// <summary>
/// Cross-platform service for accessing device information.
/// </summary>
public class DeviceInfoService : IDeviceInfoService
{
    #region Fields
    private readonly ILogger<DeviceInfoService>? logger;
    private readonly IDeviceInfo deviceInfo;
    private readonly Lazy<string> deviceId;
    #endregion

    /// <summary>
    /// Creates a new instance of the DeviceInfoService.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public DeviceInfoService(ILogger<DeviceInfoService>? logger = null)
    {
        this.logger = logger;
        this.deviceInfo = DeviceInfo.Current;
        this.deviceId = new Lazy<string>(GenerateDeviceId);

        logger?.LogInformation("Device: {Manufacturer} {Model} ({Platform} {Version}), Idiom: {Idiom}, Type: {DeviceType}",
            Manufacturer, Model, Platform, VersionString, Idiom, DeviceType);
    }

    #region IDeviceInfoService Implementation
    /// <inheritdoc />
    public string Model => deviceInfo.Model;

    /// <inheritdoc />
    public string Manufacturer => deviceInfo.Manufacturer;

    /// <inheritdoc />
    public string Name => deviceInfo.Name;

    /// <inheritdoc />
    public string VersionString => deviceInfo.VersionString;

    /// <inheritdoc />
    public DevicePlatform Platform => deviceInfo.Platform;

    /// <inheritdoc />
    public DeviceIdiom Idiom => deviceInfo.Idiom;

    /// <inheritdoc />
    public DeviceType DeviceType => deviceInfo.DeviceType;

    /// <inheritdoc />
    public bool IsPhysicalDevice => DeviceType == DeviceType.Physical;

    /// <inheritdoc />
    public bool IsVirtualDevice => DeviceType == DeviceType.Virtual;

    /// <inheritdoc />
    public bool IsPhone => Idiom == DeviceIdiom.Phone;

    /// <inheritdoc />
    public bool IsTablet => Idiom == DeviceIdiom.Tablet;

    /// <inheritdoc />
    public bool IsDesktop => Idiom == DeviceIdiom.Desktop;

    /// <inheritdoc />
    public string DeviceId => deviceId.Value;
    #endregion

    #region Private Methods
    private string GenerateDeviceId()
    {
        try
        {
            // Generate a stable but not truly unique ID based on device characteristics
            // Note: This is not suitable for sensitive operations or advertising IDs
            string uniqueString = $"{Manufacturer}|{Model}|{Platform}|{VersionString}";

            // Hash the string to create a more compact ID
            byte[] hashBytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(uniqueString));

            // Convert to hex string
            string hash = Convert.ToHexStringLower(hashBytes);

            // Take first 16 characters for readability
            return hash[..16];
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error generating device ID");
            return Guid.NewGuid().ToString("N")[..16];
        }
    }
    #endregion
}
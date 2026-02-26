namespace GamaLearn.Services;

/// <summary>
/// Service for monitoring device battery information and power state.
/// </summary>
public interface IBatteryService
{
    /// <summary>
    /// Gets the current battery charge level as a percentage (0.0 to 1.0).
    /// Returns 1.0 if battery information is not available.
    /// </summary>
    double ChargeLevel { get; }

    /// <summary>
    /// Gets the current battery state (charging, discharging, full, etc.).
    /// </summary>
    BatteryState State { get; }

    /// <summary>
    /// Gets the power source being used (battery, AC, USB, wireless).
    /// </summary>
    BatteryPowerSource PowerSource { get; }

    /// <summary>
    /// Gets whether the device is running in energy saver mode (low-power mode).
    /// </summary>
    bool EnergySaverStatus { get; }

    /// <summary>
    /// Occurs when the battery charge level changes.
    /// </summary>
    event EventHandler<BatteryInfoChangedEventArgs> BatteryInfoChanged;

    /// <summary>
    /// Occurs when the energy saver status changes.
    /// </summary>
    event EventHandler<EnergySaverStatusChangedEventArgs> EnergySaverStatusChanged;

    /// <summary>
    /// Starts monitoring battery changes.
    /// Call this to begin receiving battery change events.
    /// </summary>
    void StartMonitoring();

    /// <summary>
    /// Stops monitoring battery changes.
    /// Call this when you no longer need battery updates to save resources.
    /// </summary>
    void StopMonitoring();
}

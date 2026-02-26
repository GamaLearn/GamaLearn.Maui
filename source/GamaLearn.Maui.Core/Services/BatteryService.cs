using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

/// <summary>
/// Cross-platform service for monitoring device battery information.
/// </summary>
public partial class BatteryService : IBatteryService, IDisposable
{
    #region Fields
    private readonly ILogger<BatteryService>? logger;
    private readonly IBattery battery;
    private bool isMonitoring;
    private bool disposed;
    #endregion

    /// <summary>
    /// Creates a new instance of the BatteryService.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public BatteryService(ILogger<BatteryService>? logger = null)
    {
        this.logger = logger;
        this.battery = Battery.Default;
    }

    #region IBatteryService Implementation
    /// <inheritdoc />
    public double ChargeLevel => battery.ChargeLevel;

    /// <inheritdoc />
    public BatteryState State => battery.State;

    /// <inheritdoc />
    public BatteryPowerSource PowerSource => battery.PowerSource;

    /// <inheritdoc />
    public bool EnergySaverStatus => battery.EnergySaverStatus == Microsoft.Maui.Devices.EnergySaverStatus.On;

    /// <inheritdoc />
    public event EventHandler<BatteryInfoChangedEventArgs>? BatteryInfoChanged;

    /// <inheritdoc />
    public event EventHandler<EnergySaverStatusChangedEventArgs>? EnergySaverStatusChanged;

    /// <inheritdoc />
    public void StartMonitoring()
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (isMonitoring)
        {
            logger?.LogDebug("Battery monitoring already started");
            return;
        }

        battery.BatteryInfoChanged += OnBatteryInfoChanged;
        battery.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
        isMonitoring = true;

        logger?.LogInformation("Battery monitoring started. Current level: {ChargeLevel:P0}, State: {State}, Power: {PowerSource}",
            ChargeLevel, State, PowerSource);
    }

    /// <inheritdoc />
    public void StopMonitoring()
    {
        if (!isMonitoring)
        {
            logger?.LogDebug("Battery monitoring already stopped");
            return;
        }

        battery.BatteryInfoChanged -= OnBatteryInfoChanged;
        battery.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;
        isMonitoring = false;

        logger?.LogInformation("Battery monitoring stopped");
    }
    #endregion

    #region Event Handlers
    private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        logger?.LogDebug("Battery info changed. Level: {ChargeLevel:P0}, State: {State}, Power: {PowerSource}",
            e.ChargeLevel, e.State, e.PowerSource);

        BatteryInfoChanged?.Invoke(this, e);
    }

    private void OnEnergySaverStatusChanged(object? sender, EnergySaverStatusChangedEventArgs e)
    {
        logger?.LogInformation("Energy saver status changed: {EnergySaverStatus}", battery.EnergySaverStatus);

        EnergySaverStatusChanged?.Invoke(this, e);
    }
    #endregion

    #region IDisposable Implementation
    /// <summary>
    /// Disposes the service and stops monitoring.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;

        StopMonitoring();
        disposed = true;

        GC.SuppressFinalize(this);
    }
    #endregion
}
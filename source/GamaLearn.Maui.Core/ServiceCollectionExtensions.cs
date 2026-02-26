using GamaLearn.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GamaLearn;

/// <summary>
/// Extension methods for registering GamaLearn.Maui.Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    #region App Rating Service
    /// <summary>
    /// Adds the App Rating Service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure the rating options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppRatingService(this IServiceCollection services, Action<AppRatingOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        AppRatingOptions options = new();
        configure(options);

        ValidateOptions(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<IAppRatingService>(sp =>
        {
            ILogger<AppRatingService>? logger = sp.GetService<ILoggerFactory>()?.CreateLogger<AppRatingService>();

            return new AppRatingService(options, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds the App Rating Service to the service collection with default options.
    /// You must still configure the store identifiers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="windowsProductId">Windows Store Product ID.</param>
    /// <param name="iOSAppId">iOS App Store ID.</param>
    /// <param name="androidPackageName">Android package name (optional, will use current app's package if not set).</param>
    /// <param name="macAppId">Mac App Store ID (optional, will use iOS ID if not set).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppRatingService(this IServiceCollection services, string? windowsProductId = null, string? iOSAppId = null, string? androidPackageName = null, string? macAppId = null)
    {
        return services.AddAppRatingService(options =>
        {
            options.WindowsProductId = windowsProductId;
            options.IOSAppId = iOSAppId;
            options.AndroidPackageName = androidPackageName;
            options.MacAppId = macAppId;
        });
    }

    private static void ValidateOptions(AppRatingOptions options)
    {
        if (options.DaysBetweenPrompts < 0)
        {
            throw new ArgumentException("DaysBetweenPrompts must be non-negative", nameof(options));
        }

        if (options.MaxPromptCount < 1)
        {
            throw new ArgumentException("MaxPromptCount must be at least 1", nameof(options));
        }

        if (options.MinLaunchesBeforeFirstPrompt < 0)
        {
            throw new ArgumentException("MinLaunchesBeforeFirstPrompt must be non-negative", nameof(options));
        }

        if (options.MinDaysAfterInstall < 0)
        {
            throw new ArgumentException("MinDaysAfterInstall must be non-negative", nameof(options));
        }

#if WINDOWS
        if (string.IsNullOrWhiteSpace(options.WindowsProductId))
        {
            System.Diagnostics.Trace.WriteLine("[AppRatingService] Warning: WindowsProductId is not configured. Rating prompts will fail on Windows.");
        }
#elif IOS
        if (string.IsNullOrWhiteSpace(options.IOSAppId))
        {
            System.Diagnostics.Trace.WriteLine("[AppRatingService] Warning: iOSAppId is not configured. Store fallback will fail on iOS.");
        }
#elif MACCATALYST
        if (string.IsNullOrWhiteSpace(options.MacAppId) && string.IsNullOrWhiteSpace(options.IOSAppId))
        {
            System.Diagnostics.Trace.WriteLine("[AppRatingService] Warning: MacAppId and iOSAppId are not configured. Store fallback will fail on Mac.");
        }
#endif
    }
    #endregion

    #region Battery Service
    /// <summary>
    /// Adds the Battery Service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBatteryService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IBatteryService>(sp =>
        {
            ILogger<BatteryService>? logger = sp.GetService<ILoggerFactory>()?.CreateLogger<BatteryService>();
            return new BatteryService(logger);
        });

        return services;
    }
    #endregion

    #region Device Info Service
    /// <summary>
    /// Adds the Device Info Service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDeviceInfoService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IDeviceInfoService>(sp =>
        {
            ILogger<DeviceInfoService>? logger = sp.GetService<ILoggerFactory>()?.CreateLogger<DeviceInfoService>();
            return new DeviceInfoService(logger);
        });

        return services;
    }
    #endregion

    #region All Core Services
    /// <summary>
    /// Adds all GamaLearn.Maui.Core services to the service collection.
    /// Includes: BatteryService, DeviceInfoService.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGamaLearnCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddBatteryService();
        services.AddDeviceInfoService();

        return services;
    }
    #endregion
}
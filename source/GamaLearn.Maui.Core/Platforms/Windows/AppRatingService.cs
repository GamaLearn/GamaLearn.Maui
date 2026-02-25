#if WINDOWS
using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

public partial class AppRatingService
{
    /// <summary>
    /// Opens the Microsoft Store rating page.
    /// Note: We intentionally avoid RequestRateAndReviewAppAsync due to known access violation crashes in WinUI 3.
    /// </summary>
    private async partial Task<bool> PlatformOpenStoreAsync()
    {
        string? productId = options.WindowsProductId;

        if (string.IsNullOrWhiteSpace(productId))
        {
            logger?.LogWarning("Windows Product ID is not configured");
            return false;
        }

        try
        {
            // Primary method: Use ms-windows-store protocol
            Uri storeUri = new($"ms-windows-store://review/?ProductId={productId}");

            Windows.System.LauncherOptions options = new()
            {
                DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMore
            };

            bool launched = await Windows.System.Launcher.LaunchUriAsync(storeUri, options);

            if (launched)
            {
                logger?.LogInformation("Opened Microsoft Store for product {ProductId}", productId);
                return true;
            }

            logger?.LogWarning("LaunchUriAsync returned false");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to open Microsoft Store via protocol");
        }
        return false;
    }

    /// <summary>
    /// Windows does not support native in-app review due to known crashes with RequestRateAndReviewAppAsync.
    /// Always returns false to fall back to opening the store.
    /// </summary>
    private partial Task<bool> TryNativeInAppReviewAsync()
    {
        // IMPORTANT: We intentionally skip StoreContext.RequestRateAndReviewAppAsync() 
        // because it causes access violation crashes in WinUI 3 / MAUI Windows apps.
        // See: https://github.com/microsoft/WindowsAppSDK/discussions/3292
        logger?.LogDebug("Native in-app review not supported on Windows, will open store instead");
        return Task.FromResult(false);
    }
}
#endif
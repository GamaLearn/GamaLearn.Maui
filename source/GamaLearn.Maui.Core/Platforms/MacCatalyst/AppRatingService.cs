#if MACCATALYST
using UIKit;
using StoreKit;
using Foundation;
using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

public partial class AppRatingService
{
    /// <summary>
    /// Opens the Mac App Store rating page for the app.
    /// </summary>
    private async partial Task<bool> PlatformOpenStoreAsync()
    {
        string? appId = options.MacAppId ?? options.IOSAppId;

        if (string.IsNullOrWhiteSpace(appId))
        {
            logger?.LogWarning("Mac App ID is not configured");
            return false;
        }

        try
        {
            // Open Mac App Store directly to the reviews section
            string storeUrl = $"macappstore://apps.apple.com/app/id{appId}?action=write-review";
            NSUrl url = new(storeUrl);

            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url, new NSDictionary(), null);
                logger?.LogInformation("Opened Mac App Store for app {AppId}", appId);
                return true;
            }

            logger?.LogWarning("Cannot open Mac App Store URL, trying web fallback");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to open Mac App Store, trying web fallback");
        }

        // Fallback: Open web browser
        try
        {
            Uri webUri = new($"https://apps.apple.com/app/id{appId}?action=write-review");
            await Browser.OpenAsync(webUri, BrowserLaunchMode.External);

            logger?.LogInformation("Opened Mac App Store web page for app {AppId}", appId);
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to open Mac App Store web fallback");
            return false;
        }
    }

    /// <summary>
    /// Attempts to show Apple's native in-app review dialog on Mac Catalyst.
    /// </summary>
    private partial Task<bool> TryNativeInAppReviewAsync()
    {
        try
        {
            UIWindowScene? windowScene = GetCurrentWindowScene();

            if (windowScene is null)
            {
                logger?.LogWarning("Could not get current window scene for in-app review");
                return Task.FromResult(false);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (OperatingSystem.IsMacCatalystVersionAtLeast(16, 0))
                {
                    // Mac 16+ → StoreKit 2
                    // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable APL0004
                    AppStore.RequestReview(windowScene);
#pragma warning restore APL0004
                }
                else if (OperatingSystem.IsMacCatalystVersionAtLeast(14, 0))
                {
                    // Mac 14–15 → Scene-based SKStoreReviewController
                    SKStoreReviewController.RequestReview(windowScene);
                }
                else if (OperatingSystem.IsMacCatalystVersionAtLeast(10, 3))
                {
                    // Mac 10.3–13 → Legacy SKStoreReviewController
                    SKStoreReviewController.RequestReview();
                }
            });

            logger?.LogInformation("Requested in-app review (actual display is controlled by Apple)");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error requesting in-app review");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Gets the current UIWindowScene for macOS Catalyst APIs.
    /// </summary>
    private static UIWindowScene? GetCurrentWindowScene()
    {
        try
        {
            UIScene? scene = UIApplication.SharedApplication.ConnectedScenes
                .ToArray()
                .FirstOrDefault(x => x.ActivationState == UISceneActivationState.ForegroundActive);

            if (scene == null || scene is not UIWindowScene windowScene)
                return null;

            return windowScene;
        }
        catch
        {
            return null;
        }
    }
}
#endif
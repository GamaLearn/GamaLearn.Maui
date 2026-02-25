#if IOS
using UIKit;
using StoreKit;
using Foundation;
using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

public partial class AppRatingService
{
    /// <summary>
    /// Opens the App Store rating page for the app.
    /// </summary>
    private async partial Task<bool> PlatformOpenStoreAsync()
    {
        string? appId = options.IOSAppId;

        if (string.IsNullOrWhiteSpace(appId))
        {
            logger?.LogWarning("iOS App ID is not configured");
            return false;
        }

        try
        {
            // Open App Store directly to the reviews section
            string storeUrl = $"itms-apps://itunes.apple.com/app/id{appId}?action=write-review";
            NSUrl url = new(storeUrl);

            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url, new NSDictionary(), null);
                logger?.LogInformation("Opened App Store for app {AppId}", appId);
                return true;
            }

            logger?.LogWarning("Cannot open App Store URL, trying web fallback");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to open App Store, trying web fallback");
        }

        // Fallback: Open web browser
        try
        {
            Uri webUri = new($"https://apps.apple.com/app/id{appId}?action=write-review");
            await Browser.OpenAsync(webUri, BrowserLaunchMode.External);

            logger?.LogInformation("Opened App Store web page for app {AppId}", appId);
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to open App Store web fallback");
            return false;
        }
    }

    /// <summary>
    /// Attempts to show Apple's native in-app review dialog.
    /// Note: Apple limits how often this is shown, even if called.
    /// </summary>
    private partial Task<bool> TryNativeInAppReviewAsync()
    {
        try
        {
            // iOS 14+ uses SKStoreReviewController.RequestReview(windowScene:)
            // But for MAUI, we use the simpler API that works across versions

            UIWindowScene? windowScene = GetCurrentWindowScene();

            if (windowScene is null)
            {
                logger?.LogWarning("Could not get current window scene for in-app review");
                return Task.FromResult(false);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (OperatingSystem.IsIOSVersionAtLeast(16, 0))
                {
                    // iOS 16+ → StoreKit 2
                    // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable APL0004
                    AppStore.RequestReview(windowScene);
#pragma warning restore APL0004
                }
                else if (OperatingSystem.IsIOSVersionAtLeast(14, 0))
                {
                    // iOS 14–15 → Scene-based SKStoreReviewController
                    SKStoreReviewController.RequestReview(windowScene);
                }
                else if (OperatingSystem.IsIOSVersionAtLeast(10, 3))
                {
                    // iOS 10.3–13 → Legacy SKStoreReviewController
                    SKStoreReviewController.RequestReview();
                }
            });

            logger?.LogInformation("Requested in-app review (actual display is controlled by Apple)");
            // Note: We can't know if Apple actually showed the dialog or if user submitted a review
            // Apple may silently ignore the request based on their own rate-limiting logic
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error requesting in-app review");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Gets the current UIWindowScene for iOS 14+ APIs.
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
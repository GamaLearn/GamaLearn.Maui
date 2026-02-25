#if ANDROID
using Android.Content;
using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.Review;
using Xamarin.Google.Android.Play.Core.Review.Testing;
using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

public partial class AppRatingService
{
    /// <summary>
    /// Opens Google Play Store rating page for the app.
    /// </summary>
    private partial Task<bool> PlatformOpenStoreAsync()
    {
        string? packageName = options.AndroidPackageName;

        // If not configured, use the current app's package name
        if (string.IsNullOrWhiteSpace(packageName))
        {
            packageName = Platform.CurrentActivity?.PackageName;
        }

        if (string.IsNullOrWhiteSpace(packageName))
        {
            logger?.LogWarning("Android Package Name is not configured and could not be determined");
            return System.Threading.Tasks.Task.FromResult(false);
        }

        try
        {
            // Try to open in Google Play app
            Intent intent = new(Intent.ActionView, Android.Net.Uri.Parse($"market://details?id={packageName}"));
            intent.SetPackage("com.android.vending");
            intent.AddFlags(ActivityFlags.NewTask);

            if (Platform.CurrentActivity != null &&
                intent.ResolveActivity(Platform.CurrentActivity.PackageManager!) != null)
            {
                Platform.CurrentActivity.StartActivity(intent);
                logger?.LogInformation("Opened Google Play Store for package {PackageName}", packageName);
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to open Google Play app");
        }
        return System.Threading.Tasks.Task.FromResult(false);
    }

    /// <summary>
    /// Attempts to show Google Play's native in-app review dialog.
    /// </summary>
    private async partial Task<bool> TryNativeInAppReviewAsync()
    {
        if (Platform.CurrentActivity is null)
        {
            logger?.LogWarning("CurrentActivity is null, cannot show in-app review");
            return false;
        }

        try
        {
            // Use FakeReviewManager in debug builds for testing
            IReviewManager reviewManager;
#if DEBUG
            logger?.LogDebug("Using FakeReviewManager for debug build");
            reviewManager = new FakeReviewManager(Platform.CurrentActivity);
#else
            reviewManager = ReviewManagerFactory.Create(Platform.CurrentActivity);
#endif

            // Request review info
            Android.Gms.Tasks.Task requestTask = reviewManager.RequestReviewFlow();

            TaskCompletionSource<Java.Lang.Object?> requestTcs = new();

            requestTask.AddOnCompleteListener(new OnCompleteListener(task =>
            {
                if (task.IsSuccessful)
                {
                    requestTcs.TrySetResult(task.GetResult(Java.Lang.Class.FromType(typeof(ReviewInfo))));
                }
                else
                {
                    requestTcs.TrySetException(task.Exception ?? new Exception("Failed to request review flow"));
                }
            }));

            Java.Lang.Object? result = await requestTcs.Task;

            if (result is not ReviewInfo reviewInfo)
            {
                logger?.LogWarning("Failed to get ReviewInfo from Play Store");
                return false;
            }

            // Launch review flow
            Android.Gms.Tasks.Task launchTask = reviewManager.LaunchReviewFlow(Platform.CurrentActivity, reviewInfo);

            TaskCompletionSource<bool> launchTcs = new();

            launchTask.AddOnCompleteListener(new OnCompleteListener(task =>
            {
                // Note: We can't know if the user actually submitted a review
                launchTcs.TrySetResult(task.IsSuccessful);
            }));

            bool success = await launchTcs.Task;

            if (success)
            {
                logger?.LogInformation("In-app review flow completed");
            }
            else
            {
                logger?.LogWarning("In-app review flow failed");
            }

            return success;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error showing in-app review");
            return false;
        }
    }

    /// <summary>
    /// Helper class for Google Play Core task completion.
    /// </summary>
    private sealed class OnCompleteListener(Action<Android.Gms.Tasks.Task> onComplete) : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Android.Gms.Tasks.Task> onComplete = onComplete;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            onComplete(task);
        }
    }
}
#endif
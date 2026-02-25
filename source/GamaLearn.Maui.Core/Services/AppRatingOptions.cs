using GamaLearn.Events;

namespace GamaLearn.Services;

/// <summary>
/// Configuration options for the App Rating Service.
/// </summary>
public sealed class AppRatingOptions
{
    /// <summary>
    /// Minimum number of days between rating prompts.
    /// Default: 7 days.
    /// </summary>
    public int DaysBetweenPrompts { get; set; } = 7;

    /// <summary>
    /// Maximum number of times to prompt the user before giving up.
    /// Default: 3 prompts.
    /// </summary>
    public int MaxPromptCount { get; set; } = 3;

    /// <summary>
    /// Minimum number of app launches before showing the first prompt.
    /// Default: 3 launches.
    /// </summary>
    public int MinLaunchesBeforeFirstPrompt { get; set; } = 3;

    /// <summary>
    /// Minimum number of days after install before showing the first prompt.
    /// Default: 3 days.
    /// </summary>
    public int MinDaysAfterInstall { get; set; } = 3;

    /// <summary>
    /// Windows Store Product ID (from Partner Center).
    /// Example: "9NBLGGH5L9XT"
    /// </summary>
    public string? WindowsProductId { get; set; }

    /// <summary>
    /// iOS App Store ID (numeric ID from App Store Connect).
    /// Example: "389801252" (without the "id" prefix)
    /// </summary>
    public string? IOSAppId { get; set; }

    /// <summary>
    /// Mac App Store ID (numeric ID from App Store Connect).
    /// Example: "389801252"
    /// </summary>
    public string? MacAppId { get; set; }

    /// <summary>
    /// Android Package Name (from Google Play Console).
    /// Example: "com.gamalearn.grademate"
    /// </summary>
    public string? AndroidPackageName { get; set; }

    /// <summary>
    /// Title for the rating request dialog.
    /// Default: "Enjoying the app?"
    /// </summary>
    public string DialogTitle { get; set; } = "Enjoying the app?";

    /// <summary>
    /// Message for the rating request dialog.
    /// Default: "Would you like to leave a review? It helps us improve and reach more users!"
    /// </summary>
    public string DialogMessage { get; set; } = "Would you like to leave a review? It helps us improve and reach more users!";

    /// <summary>
    /// Text for the positive action button.
    /// Default: "Yes, rate it!"
    /// </summary>
    public string DialogPositiveButton { get; set; } = "Yes, rate it!";

    /// <summary>
    /// Text for the negative/later action button.
    /// Default: "Not now"
    /// </summary>
    public string DialogNegativeButton { get; set; } = "Not now";

    /// <summary>
    /// Text for the permanent decline button.
    /// Default: "Never ask again"
    /// </summary>
    public string DialogNeverButton { get; set; } = "Never ask again";

    /// <summary>
    /// Whether to show a custom dialog before opening the store.
    /// If false, will attempt to use native in-app review (Android/iOS only).
    /// Default: true (recommended for better user experience and tracking).
    /// </summary>
    public bool ShowCustomDialog { get; set; } = true;

    /// <summary>
    /// Whether to use in-app review APIs when available (Android/iOS).
    /// If false, always opens the store page.
    /// Default: true.
    /// Note: Windows always opens the store page due to known issues with in-app review.
    /// </summary>
    public bool PreferInAppReview { get; set; } = true;

    /// <summary>
    /// Whether to assume the user rated after opening the store.
    /// Since stores don't confirm actual reviews, we can only assume.
    /// If true, will stop prompting after user agrees to rate.
    /// Default: true.
    /// </summary>
    public bool AssumeRatedAfterStoreOpen { get; set; } = true;

    /// <summary>
    /// Callback invoked when a rating prompt is shown.
    /// Useful for analytics.
    /// </summary>
    public Action<RatingPromptEventArgs>? OnPromptShown { get; set; }

    /// <summary>
    /// Callback invoked when user responds to the rating prompt.
    /// Useful for analytics.
    /// </summary>
    public Action<RatingResponseEventArgs>? OnUserResponse { get; set; }
}
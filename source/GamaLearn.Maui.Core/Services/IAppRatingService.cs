namespace GamaLearn.Services;

/// <summary>
/// Service for prompting users to rate the application on their respective app stores.
/// </summary>
public interface IAppRatingService
{
    /// <summary>
    /// Attempts to prompt the user for a rating based on configured conditions.
    /// Will check if enough time has passed, prompt count limits, and user preferences.
    /// </summary>
    /// <param name="isSignificantEvent">
    /// If true, bypasses the time-based check (but still respects prompt limits and user preferences).
    /// Use this after significant user achievements like completing a major task.
    /// </param>
    /// <returns>True if the rating prompt was shown, false otherwise.</returns>
    Task<bool> TryPromptForRatingAsync(bool isSignificantEvent = false);

    /// <summary>
    /// Opens the app store rating page directly without any checks or dialogs.
    /// Use this for manual "Rate Us" buttons in settings.
    /// </summary>
    /// <returns>True if the store was opened successfully.</returns>
    Task<bool> OpenStoreRatingAsync();

    /// <summary>
    /// Resets all rating tracking data (prompt count, dates, user preferences).
    /// Useful for testing or when user requests to be asked again.
    /// </summary>
    void ResetRatingData();

    /// <summary>
    /// Gets whether the user has permanently declined rating prompts.
    /// </summary>
    bool HasUserDeclinedPermanently { get; }

    /// <summary>
    /// Gets the number of times the user has been prompted.
    /// </summary>
    int PromptCount { get; }

    /// <summary>
    /// Gets the date of the last rating prompt, or null if never prompted.
    /// </summary>
    DateTime? LastPromptDate { get; }
}
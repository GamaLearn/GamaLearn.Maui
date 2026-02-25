using GamaLearn.Enums;
using GamaLearn.Events;
using Microsoft.Extensions.Logging;

namespace GamaLearn.Services;

/// <summary>
/// Cross-platform service for prompting users to rate the application.
/// </summary>
public partial class AppRatingService : IAppRatingService
{
    #region Constants
    private const string PreferencePrefix = "gamalearn_apprating_";
    private const string LastPromptDateKey = PreferencePrefix + "last_prompt_date";
    private const string PromptCountKey = PreferencePrefix + "prompt_count";
    private const string UserDeclinedKey = PreferencePrefix + "user_declined";
    private const string UserRatedKey = PreferencePrefix + "user_rated";
    private const string FirstLaunchDateKey = PreferencePrefix + "first_launch_date";
    private const string LaunchCountKey = PreferencePrefix + "launch_count";
    #endregion

    #region Fields
    private readonly AppRatingOptions options;
    private readonly ILogger<AppRatingService>? logger;
    #endregion

    /// <summary>
    /// Creates a new instance of the AppRatingService.
    /// </summary>
    /// <param name="options">Configuration options for the service.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public AppRatingService(AppRatingOptions options, ILogger<AppRatingService>? logger = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger;

        // Track first launch and launch count
        InitializeLaunchTracking();
    }

    #region IAppRatingService Implementation
    /// <inheritdoc />
    public bool HasUserDeclinedPermanently => Preferences.Get(UserDeclinedKey, false);

    /// <inheritdoc />
    public int PromptCount => Preferences.Get(PromptCountKey, 0);

    /// <inheritdoc />
    public DateTime? LastPromptDate
    {
        get
        {
            long ticks = Preferences.Get(LastPromptDateKey, 0L);
            return ticks > 0 ? new DateTime(ticks, DateTimeKind.Utc) : null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryPromptForRatingAsync(bool isSignificantEvent = false)
    {
        try
        {
            // Check if we should prompt
            if (!ShouldPromptUser(isSignificantEvent, out string reason))
            {
                logger?.LogDebug("Skipping rating prompt: {Reason}", reason);
                return false;
            }

            // Show the prompt
            RatingResponse response = await ShowRatingPromptAsync();

            // Track the prompt
            int newPromptCount = PromptCount + 1;
            Preferences.Set(PromptCountKey, newPromptCount);
            Preferences.Set(LastPromptDateKey, DateTime.UtcNow.Ticks);

            // Invoke callback
            options.OnPromptShown?.Invoke(new RatingPromptEventArgs
            {
                PromptNumber = newPromptCount,
                WasSignificantEvent = isSignificantEvent,
                DaysSinceLastPrompt = LastPromptDate.HasValue
                    ? (int)(DateTime.UtcNow - LastPromptDate.Value).TotalDays
                    : null
            });

            // Handle response
            await HandleUserResponseAsync(response, newPromptCount);

            return response == RatingResponse.Accepted;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error during rating prompt");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> OpenStoreRatingAsync()
    {
        try
        {
            logger?.LogInformation("Opening store rating page");

            bool success = await PlatformOpenStoreAsync();

            if (success && options.AssumeRatedAfterStoreOpen)
            {
                Preferences.Set(UserRatedKey, true);
            }

            return success;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error opening store rating page");
            return false;
        }
    }

    /// <inheritdoc />
    public void ResetRatingData()
    {
        logger?.LogInformation("Resetting all rating data");

        Preferences.Remove(LastPromptDateKey);
        Preferences.Remove(PromptCountKey);
        Preferences.Remove(UserDeclinedKey);
        Preferences.Remove(UserRatedKey);
        Preferences.Remove(FirstLaunchDateKey);
        Preferences.Remove(LaunchCountKey);

        // Re-initialize launch tracking
        InitializeLaunchTracking();
    }
    #endregion

    #region Private Methods
    private void InitializeLaunchTracking()
    {
        // Track first launch date
        if (Preferences.Get(FirstLaunchDateKey, 0L) == 0)
        {
            Preferences.Set(FirstLaunchDateKey, DateTime.UtcNow.Ticks);
        }

        // Increment launch count
        int launchCount = Preferences.Get(LaunchCountKey, 0);
        Preferences.Set(LaunchCountKey, launchCount + 1);

        logger?.LogDebug("App launch #{LaunchCount}", launchCount + 1);
    }

    private bool ShouldPromptUser(bool isSignificantEvent, out string reason)
    {
        // Already rated?
        if (Preferences.Get(UserRatedKey, false))
        {
            reason = "User already rated";
            return false;
        }

        // User declined permanently?
        if (HasUserDeclinedPermanently)
        {
            reason = "User declined permanently";
            return false;
        }

        // Exceeded max prompts?
        if (PromptCount >= options.MaxPromptCount)
        {
            reason = $"Max prompt count ({options.MaxPromptCount}) reached";
            return false;
        }

        // Check minimum launches before first prompt
        int launchCount = Preferences.Get(LaunchCountKey, 0);
        if (launchCount < options.MinLaunchesBeforeFirstPrompt)
        {
            reason = $"Not enough launches ({launchCount}/{options.MinLaunchesBeforeFirstPrompt})";
            return false;
        }

        // Check minimum days after install
        long firstLaunchTicks = Preferences.Get(FirstLaunchDateKey, 0L);
        if (firstLaunchTicks > 0)
        {
            DateTime firstLaunch = new(firstLaunchTicks, DateTimeKind.Utc);
            int daysSinceInstall = (int)(DateTime.UtcNow - firstLaunch).TotalDays;

            if (daysSinceInstall < options.MinDaysAfterInstall)
            {
                reason = $"Not enough days since install ({daysSinceInstall}/{options.MinDaysAfterInstall})";
                return false;
            }
        }

        // Significant events bypass time check (but not other checks)
        if (isSignificantEvent)
        {
            reason = string.Empty;
            return true;
        }

        // Check time since last prompt
        if (LastPromptDate.HasValue)
        {
            int daysSinceLastPrompt = (int)(DateTime.UtcNow - LastPromptDate.Value).TotalDays;
            if (daysSinceLastPrompt < options.DaysBetweenPrompts)
            {
                reason = $"Not enough days since last prompt ({daysSinceLastPrompt}/{options.DaysBetweenPrompts})";
                return false;
            }
        }

        reason = string.Empty;
        return true;
    }

    private async Task<RatingResponse> ShowRatingPromptAsync()
    {
        if (!options.ShowCustomDialog)
        {
            // Try native in-app review directly
            bool opened = await TryNativeInAppReviewAsync();
            return opened ? RatingResponse.Accepted : RatingResponse.Dismissed;
        }

        // Show custom dialog
        return await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            string? result = await Application.Current!.Windows[0].Page!.DisplayActionSheet(
                options.DialogTitle + "\n" + options.DialogMessage,
                options.DialogNegativeButton,
                null,
                options.DialogPositiveButton,
                options.DialogNeverButton);

            if (result == options.DialogPositiveButton)
            {
                return RatingResponse.Accepted;
            }
            else if (result == options.DialogNeverButton)
            {
                return RatingResponse.DeclinedPermanently;
            }
            else if (result == options.DialogNegativeButton)
            {
                return RatingResponse.DeclinedForNow;
            }
            else
            {
                return RatingResponse.Dismissed;
            }
        });
    }

    private async Task HandleUserResponseAsync(RatingResponse response, int promptNumber)
    {
        // Invoke callback
        options.OnUserResponse?.Invoke(new RatingResponseEventArgs
        {
            Response = response,
            PromptNumber = promptNumber
        });

        switch (response)
        {
            case RatingResponse.Accepted:
                logger?.LogInformation("User accepted rating prompt #{PromptNumber}", promptNumber);

                // Open the store
                if (options.PreferInAppReview && await TryNativeInAppReviewAsync())
                {
                    // Native in-app review shown
                    if (options.AssumeRatedAfterStoreOpen)
                    {
                        Preferences.Set(UserRatedKey, true);
                    }
                }
                else
                {
                    // Fall back to opening store
                    await OpenStoreRatingAsync();
                }
                break;

            case RatingResponse.DeclinedPermanently:
                logger?.LogInformation("User permanently declined rating prompt");
                Preferences.Set(UserDeclinedKey, true);
                break;

            case RatingResponse.DeclinedForNow:
                logger?.LogInformation("User declined rating prompt for now");
                break;

            case RatingResponse.Dismissed:
                logger?.LogInformation("User dismissed rating prompt");
                break;
        }
    }
    #endregion

    #region Platform-Specific Methods (Partial)
    /// <summary>
    /// Platform-specific implementation to open the store rating page.
    /// </summary>
    private partial Task<bool> PlatformOpenStoreAsync();

    /// <summary>
    /// Platform-specific implementation to try native in-app review.
    /// Returns false if not supported or failed.
    /// </summary>
    private partial Task<bool> TryNativeInAppReviewAsync();
    #endregion
}
namespace GamaLearn.Events;

/// <summary>
/// Event arguments for when a rating prompt is shown.
/// </summary>
public sealed class RatingPromptEventArgs
{
    /// <summary>
    /// The prompt number (1-based).
    /// </summary>
    public int PromptNumber { get; init; }

    /// <summary>
    /// Whether this was triggered by a significant event.
    /// </summary>
    public bool WasSignificantEvent { get; init; }

    /// <summary>
    /// Days since last prompt (null if first prompt).
    /// </summary>
    public int? DaysSinceLastPrompt { get; init; }
}
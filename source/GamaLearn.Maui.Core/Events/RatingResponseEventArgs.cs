using GamaLearn.Enums;

namespace GamaLearn.Events;

/// <summary>
/// Event arguments for when user responds to the rating prompt.
/// </summary>
public sealed class RatingResponseEventArgs
{
    /// <summary>
    /// The user's response to the rating prompt.
    /// </summary>
    public required RatingResponse Response { get; init; }

    /// <summary>
    /// The prompt number when this response was given.
    /// </summary>
    public int PromptNumber { get; init; }
}
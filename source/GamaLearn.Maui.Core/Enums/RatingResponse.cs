namespace GamaLearn.Enums;

/// <summary>
/// Possible user responses to the rating prompt.
/// </summary>
public enum RatingResponse
{
    /// <summary>
    /// User agreed to rate the app.
    /// </summary>
    Accepted,

    /// <summary>
    /// User declined for now (may be asked later).
    /// </summary>
    DeclinedForNow,

    /// <summary>
    /// User declined permanently (will not be asked again).
    /// </summary>
    DeclinedPermanently,

    /// <summary>
    /// User dismissed the dialog without choosing.
    /// </summary>
    Dismissed
}
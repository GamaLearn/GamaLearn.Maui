namespace GamaLearn.Helpers;

/// <summary>
/// Helper for controlling and monitoring the soft keyboard.
/// </summary>
public static partial class KeyboardHelper
{
    /// <summary>
    /// Event raised when the keyboard is shown.
    /// </summary>
    public static event EventHandler<KeyboardEventArgs>? KeyboardShown;

    /// <summary>
    /// Event raised when the keyboard is hidden.
    /// </summary>
    public static event EventHandler<EventArgs>? KeyboardHidden;

    /// <summary>
    /// Shows the soft keyboard for the specified view.
    /// </summary>
    /// <param name="view">The view to show the keyboard for.</param>
    /// <returns>True if the keyboard was shown successfully.</returns>
    public static bool ShowKeyboard(View view)
    {
        ArgumentNullException.ThrowIfNull(view);

        return PlatformShowKeyboard(view);
    }

    /// <summary>
    /// Hides the soft keyboard.
    /// </summary>
    /// <param name="view">The view that currently has the keyboard (optional).</param>
    /// <returns>True if the keyboard was hidden successfully.</returns>
    public static bool HideKeyboard(View? view = null)
    {
        return PlatformHideKeyboard(view);
    }

    /// <summary>
    /// Gets the current height of the soft keyboard.
    /// </summary>
    /// <returns>The keyboard height in device-independent units, or 0 if not visible.</returns>
    public static double GetKeyboardHeight()
    {
        return PlatformGetKeyboardHeight();
    }

    /// <summary>
    /// Gets whether the soft keyboard is currently visible.
    /// </summary>
    /// <returns>True if the keyboard is visible.</returns>
    public static bool IsKeyboardVisible()
    {
        return GetKeyboardHeight() > 0;
    }

    /// <summary>
    /// Raises the KeyboardShown event.
    /// </summary>
    internal static void OnKeyboardShown(double height)
    {
        KeyboardShown?.Invoke(null, new KeyboardEventArgs(height));
    }

    /// <summary>
    /// Raises the KeyboardHidden event.
    /// </summary>
    internal static void OnKeyboardHidden()
    {
        KeyboardHidden?.Invoke(null, EventArgs.Empty);
    }

#if ANDROID || IOS || MACCATALYST || WINDOWS || TIZEN
    /// <summary>
    /// Platform-specific implementation to show the keyboard.
    /// </summary>
    private static partial bool PlatformShowKeyboard(View view);

    /// <summary>
    /// Platform-specific implementation to hide the keyboard.
    /// </summary>
    private static partial bool PlatformHideKeyboard(View? view);

    /// <summary>
    /// Platform-specific implementation to get the keyboard height.
    /// </summary>
    private static partial double PlatformGetKeyboardHeight();
#else
    /// <summary>
    /// Platform-specific implementation to show the keyboard.
    /// Not supported on standard .NET targets.
    /// </summary>
    private static bool PlatformShowKeyboard(View view)
    {
        return false;
    }

    /// <summary>
    /// Platform-specific implementation to hide the keyboard.
    /// Not supported on standard .NET targets.
    /// </summary>
    private static bool PlatformHideKeyboard(View? view)
    {
        return false;
    }

    /// <summary>
    /// Platform-specific implementation to get the keyboard height.
    /// Not supported on standard .NET targets.
    /// </summary>
    private static double PlatformGetKeyboardHeight()
    {
        return 0;
    }
#endif
}

/// <summary>
/// Event arguments for keyboard events.
/// </summary>
public class KeyboardEventArgs : EventArgs
{
    /// <summary>
    /// Gets the height of the keyboard in device-independent units.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Creates a new instance of KeyboardEventArgs.
    /// </summary>
    /// <param name="height">The keyboard height.</param>
    public KeyboardEventArgs(double height)
    {
        Height = height;
    }
}

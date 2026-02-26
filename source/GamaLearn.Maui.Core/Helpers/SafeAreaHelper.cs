namespace GamaLearn.Helpers;

/// <summary>
/// Helper for getting safe area insets on devices with notches, rounded corners, or system bars.
/// </summary>
public static partial class SafeAreaHelper
{
    /// <summary>
    /// Gets the safe area insets for the current window/view.
    /// Returns insets for notches, status bar, navigation bar, and rounded corners.
    /// </summary>
    /// <returns>Thickness representing the safe area insets (Left, Top, Right, Bottom).</returns>
    public static Thickness GetSafeAreaInsets()
    {
        return PlatformGetSafeAreaInsets();
    }

    /// <summary>
    /// Gets the safe area insets for a specific page.
    /// </summary>
    /// <param name="page">The page to get safe area insets for.</param>
    /// <returns>Thickness representing the safe area insets (Left, Top, Right, Bottom).</returns>
    public static Thickness GetSafeAreaInsets(Page page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return PlatformGetSafeAreaInsets(page);
    }

    /// <summary>
    /// Applies safe area padding to a layout.
    /// Adds the safe area insets to the current padding.
    /// </summary>
    /// <param name="layout">The layout to apply safe area padding to.</param>
    /// <param name="edges">Which edges to apply safe area padding to. Default is all edges.</param>
    public static void ApplySafeAreaPadding(Layout layout, SafeAreaEdges edges = SafeAreaEdges.All)
    {
        ArgumentNullException.ThrowIfNull(layout);

        Thickness insets = GetSafeAreaInsets();
        Thickness current = layout.Padding;

        double left = edges.HasFlag(SafeAreaEdges.Left) ? current.Left + insets.Left : current.Left;
        double top = edges.HasFlag(SafeAreaEdges.Top) ? current.Top + insets.Top : current.Top;
        double right = edges.HasFlag(SafeAreaEdges.Right) ? current.Right + insets.Right : current.Right;
        double bottom = edges.HasFlag(SafeAreaEdges.Bottom) ? current.Bottom + insets.Bottom : current.Bottom;

        layout.Padding = new Thickness(left, top, right, bottom);
    }

    /// <summary>
    /// Applies safe area padding to a page.
    /// Adds the safe area insets to the current padding.
    /// </summary>
    /// <param name="page">The page to apply safe area padding to.</param>
    /// <param name="edges">Which edges to apply safe area padding to. Default is all edges.</param>
    public static void ApplySafeAreaPadding(Page page, SafeAreaEdges edges = SafeAreaEdges.All)
    {
        ArgumentNullException.ThrowIfNull(page);

        Thickness insets = GetSafeAreaInsets(page);
        Thickness current = page.Padding;

        double left = edges.HasFlag(SafeAreaEdges.Left) ? current.Left + insets.Left : current.Left;
        double top = edges.HasFlag(SafeAreaEdges.Top) ? current.Top + insets.Top : current.Top;
        double right = edges.HasFlag(SafeAreaEdges.Right) ? current.Right + insets.Right : current.Right;
        double bottom = edges.HasFlag(SafeAreaEdges.Bottom) ? current.Bottom + insets.Bottom : current.Bottom;

        page.Padding = new Thickness(left, top, right, bottom);
    }

    /// <summary>
    /// Gets the status bar height.
    /// </summary>
    /// <returns>The status bar height in device-independent units.</returns>
    public static double GetStatusBarHeight()
    {
        Thickness insets = GetSafeAreaInsets();
        return insets.Top;
    }

    /// <summary>
    /// Gets the navigation bar height (bottom system UI on Android, home indicator on iOS).
    /// </summary>
    /// <returns>The navigation bar height in device-independent units.</returns>
    public static double GetNavigationBarHeight()
    {
        Thickness insets = GetSafeAreaInsets();
        return insets.Bottom;
    }

#if ANDROID || IOS || MACCATALYST || WINDOWS || TIZEN
    /// <summary>
    /// Platform-specific implementation to get safe area insets.
    /// </summary>
    private static partial Thickness PlatformGetSafeAreaInsets();

    /// <summary>
    /// Platform-specific implementation to get safe area insets for a specific page.
    /// </summary>
    private static partial Thickness PlatformGetSafeAreaInsets(Page page);
#else
    /// <summary>
    /// Platform-specific implementation to get safe area insets.
    /// Not supported on standard .NET targets.
    /// </summary>
    private static Thickness PlatformGetSafeAreaInsets()
    {
        return Thickness.Zero;
    }

    /// <summary>
    /// Platform-specific implementation to get safe area insets for a specific page.
    /// Not supported on standard .NET targets.
    /// </summary>
    private static Thickness PlatformGetSafeAreaInsets(Page page)
    {
        return Thickness.Zero;
    }
#endif
}

/// <summary>
/// Specifies which edges should have safe area padding applied.
/// </summary>
[Flags]
public enum SafeAreaEdges
{
    /// <summary>
    /// No edges.
    /// </summary>
    None = 0,

    /// <summary>
    /// Left edge.
    /// </summary>
    Left = 1,

    /// <summary>
    /// Top edge.
    /// </summary>
    Top = 2,

    /// <summary>
    /// Right edge.
    /// </summary>
    Right = 4,

    /// <summary>
    /// Bottom edge.
    /// </summary>
    Bottom = 8,

    /// <summary>
    /// All edges.
    /// </summary>
    All = Left | Top | Right | Bottom
}
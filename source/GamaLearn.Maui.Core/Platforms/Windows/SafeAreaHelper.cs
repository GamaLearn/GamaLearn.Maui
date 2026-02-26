#if WINDOWS
namespace GamaLearn.Helpers;

public static partial class SafeAreaHelper
{
    private static partial Thickness PlatformGetSafeAreaInsets()
    {
        try
        {
            // Get the current window
            if (Application.Current?.Windows[0] is not Window window)
                return Thickness.Zero;

            // Get title bar height if custom title bar is used
            Microsoft.UI.Xaml.Window? mauiWindow = (Microsoft.UI.Xaml.Window?)Application.Current?.Windows[0]?.Handler?.PlatformView;

            if (mauiWindow?.AppWindow != null)
            {
                double titleBarHeight = mauiWindow.AppWindow.TitleBar.Height;
                // Windows doesn't typically have notches or rounded corners like mobile devices
                // Safe area is primarily the title bar on top
                return new Thickness(0, titleBarHeight, 0, 0);
            }
            return Thickness.Zero;
        }
        catch
        {
            // If anything fails, return zero insets
            return Thickness.Zero;
        }
    }

    private static partial Thickness PlatformGetSafeAreaInsets(Page page)
    {
        // Windows doesn't have per-page safe areas
        // Use the window-level insets
        return PlatformGetSafeAreaInsets();
    }
}
#endif
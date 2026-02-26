#if ANDROID
using Android.Content;
using AndroidX.Core.View;
using AView = Android.Views.View;
using Window = Android.Views.Window;
using Insets = AndroidX.Core.Graphics.Insets;

namespace GamaLearn.Helpers;

public static partial class SafeAreaHelper
{
    private static partial Thickness PlatformGetSafeAreaInsets()
    {
        Context? context = Platform.CurrentActivity;

        if (context == null)
            return Thickness.Zero;

        Window? window = Platform.CurrentActivity?.Window;

        if (window == null)
            return Thickness.Zero;

        AView? decorView = window.DecorView;

        if (decorView == null)
            return Thickness.Zero;

        // Get window insets
        WindowInsetsCompat? insets = ViewCompat.GetRootWindowInsets(decorView);

        if (insets == null)
            return Thickness.Zero;

        // Get system bars insets (status bar + navigation bar)
        // Typically returns Insets with all zeros if no system bars, not null
        Insets? systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());

        // Get display cutout insets (notches)
        // Typically returns Insets with all zeros if no cutouts, not null
        Insets? displayCutoutInsets = insets.GetInsets(WindowInsetsCompat.Type.DisplayCutout());

        // Both being null is extremely rare, but if so, return zero
        if (systemBarsInsets == null && displayCutoutInsets == null)
            return Thickness.Zero;

        // Combine both to get total safe area
        double density = context.Resources?.DisplayMetrics?.Density ?? 1.0;

        int left = Math.Max(systemBarsInsets?.Left ?? 0, displayCutoutInsets?.Left ?? 0);
        int top = Math.Max(systemBarsInsets?.Top ?? 0, displayCutoutInsets?.Top ?? 0);
        int right = Math.Max(systemBarsInsets?.Right ?? 0, displayCutoutInsets?.Right ?? 0);
        int bottom = Math.Max(systemBarsInsets?.Bottom ?? 0, displayCutoutInsets?.Bottom ?? 0);

        return new Thickness(left / density, top / density, right / density, bottom / density);
    }

    private static partial Thickness PlatformGetSafeAreaInsets(Page page)
    {
        // Try to get insets from page's platform view
        if (page.Handler?.PlatformView is AView view)
        {
            WindowInsetsCompat? insets = ViewCompat.GetRootWindowInsets(view);

            if (insets != null && view.Context != null)
            {
                // Get system bars insets (status bar + navigation bar)
                // Typically returns Insets with all zeros if no system bars, not null
                Insets? systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());

                // Get display cutout insets (notches)
                // Typically returns Insets with all zeros if no cutouts, not null
                Insets? displayCutoutInsets = insets.GetInsets(WindowInsetsCompat.Type.DisplayCutout());

                // Both being null is extremely rare, but if so, fall back
                if (systemBarsInsets == null && displayCutoutInsets == null)
                    return PlatformGetSafeAreaInsets();

                // Combine both to get total safe area
                double density = view.Context.Resources?.DisplayMetrics?.Density ?? 1.0;

                int left = Math.Max(systemBarsInsets?.Left ?? 0, displayCutoutInsets?.Left ?? 0);
                int top = Math.Max(systemBarsInsets?.Top ?? 0, displayCutoutInsets?.Top ?? 0);
                int right = Math.Max(systemBarsInsets?.Right ?? 0, displayCutoutInsets?.Right ?? 0);
                int bottom = Math.Max(systemBarsInsets?.Bottom ?? 0, displayCutoutInsets?.Bottom ?? 0);

                return new Thickness(left / density, top / density, right / density, bottom / density);
            }
        }

        // Fall back to activity-based method
        return PlatformGetSafeAreaInsets();
    }
}
#endif

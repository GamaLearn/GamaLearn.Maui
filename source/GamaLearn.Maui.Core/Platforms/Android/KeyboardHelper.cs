#if ANDROID
using Android.Content;
using Android.Views.InputMethods;
using AndroidX.Core.View;
using AView = Android.Views.View;
using Insets = AndroidX.Core.Graphics.Insets;

namespace GamaLearn.Helpers;

public static partial class KeyboardHelper
{
    private static double currentKeyboardHeight;
    private static readonly WeakReference<AView?> rootViewReference = new(null);

    private static partial bool PlatformShowKeyboard(View view)
    {
        if (view.Handler?.PlatformView is not AView platformView)
            return false;

        Context? context = platformView.Context;

        if (context == null)
            return false;


        if (context.GetSystemService(Context.InputMethodService) is not InputMethodManager imm)
            return false;

        // Request focus on the view
        platformView.RequestFocus();

        // Show the keyboard
        return imm.ShowSoftInput(platformView, ShowFlags.Implicit);
    }

    private static partial bool PlatformHideKeyboard(View? view)
    {
        Context? context = Platform.CurrentActivity;

        if (context == null)
            return false;

        if (context.GetSystemService(Context.InputMethodService) is not InputMethodManager imm)
            return false;

        if (view != null && view.Handler?.PlatformView is AView platformView)
            return imm.HideSoftInputFromWindow(platformView.WindowToken, HideSoftInputFlags.None);

        // Hide for the current activity's window
        if (Platform.CurrentActivity?.CurrentFocus is AView currentFocus)
            return imm.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);

        return false;
    }

    private static partial double PlatformGetKeyboardHeight()
    {
        return currentKeyboardHeight;
    }

    /// <summary>
    /// Call this method to start monitoring keyboard height changes on Android.
    /// Should be called from your MainActivity or when setting up your page/view.
    /// </summary>
    /// <param name="rootView">The root view of your activity or page.</param>
    public static void StartMonitoring(AView rootView)
    {
        ArgumentNullException.ThrowIfNull(rootView);

        rootViewReference.SetTarget(rootView);

        ViewCompat.SetOnApplyWindowInsetsListener(rootView, new WindowInsetsListener());
    }

    /// <summary>
    /// Stops monitoring keyboard height changes.
    /// </summary>
    public static void StopMonitoring()
    {
        if (rootViewReference.TryGetTarget(out AView? rootView) && rootView != null)
            ViewCompat.SetOnApplyWindowInsetsListener(rootView, null);

        rootViewReference.SetTarget(null);
    }

    private class WindowInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(AView? v, WindowInsetsCompat? insets)
        {
            if (v == null || insets == null)
                return insets;

            // Get IME (keyboard) insets
            Insets? imeInsets = insets.GetInsets(WindowInsetsCompat.Type.Ime());

            if (imeInsets == null)
                return insets;

            // Convert to device-independent units
            double density = v.Context?.Resources?.DisplayMetrics?.Density ?? 1.0;
            double newHeight = imeInsets.Bottom / density;

            if (newHeight != currentKeyboardHeight)
            {
                double previousHeight = currentKeyboardHeight;
                currentKeyboardHeight = newHeight;

                if (newHeight > 0 && previousHeight == 0)
                {
                    // Keyboard was shown
                    OnKeyboardShown(newHeight);
                }
                else if (newHeight == 0 && previousHeight > 0)
                {
                    // Keyboard was hidden
                    OnKeyboardHidden();
                }
            }

            return insets;
        }
    }
}
#endif

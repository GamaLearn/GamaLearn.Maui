#if IOS
using UIKit;

namespace GamaLearn.Helpers;

public static partial class SafeAreaHelper
{
    private static partial Thickness PlatformGetSafeAreaInsets()
    {
        UIWindow? window = GetKeyWindow();

        if (window == null)
            return Thickness.Zero;

        UIEdgeInsets safeAreaInsets = window.SafeAreaInsets;

        return new Thickness(safeAreaInsets.Left, safeAreaInsets.Top, safeAreaInsets.Right, safeAreaInsets.Bottom);
    }

    private static partial Thickness PlatformGetSafeAreaInsets(Page page)
    {
        // Try to get window from page handler
        if (page.Handler?.PlatformView is UIView view)
        {
            UIWindow? window = view.Window;
            if (window != null)
            {
                UIEdgeInsets safeAreaInsets = window.SafeAreaInsets;
                return new Thickness(safeAreaInsets.Left, safeAreaInsets.Top, safeAreaInsets.Right, safeAreaInsets.Bottom);
            }
        }
        // Fall back to key window
        return PlatformGetSafeAreaInsets();
    }

    private static UIWindow? GetKeyWindow()
    {
        // iOS 15+ is minimum, so we always use the modern API
        UIWindowScene? windowScene = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);

        return windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow) ?? windowScene?.Windows.FirstOrDefault();
    }
}
#endif

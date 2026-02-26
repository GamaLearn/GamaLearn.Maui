#if WINDOWS
using Microsoft.UI.Xaml.Controls;

namespace GamaLearn.Helpers;

public static partial class KeyboardHelper
{
    private static partial bool PlatformShowKeyboard(View view)
    {
        if (view.Handler?.PlatformView is not Control platformControl)
            return false;

        // Try to focus the control
        return platformControl.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    private static partial bool PlatformHideKeyboard(View? view)
    {
        // Windows doesn't have a soft keyboard in the same way mobile does
        // The on-screen keyboard is managed by the OS
        if (view is not null && view.Handler?.PlatformView is Control)
        {
            // Remove focus from the control
            Microsoft.UI.Xaml.Input.FocusManager.TryMoveFocus(Microsoft.UI.Xaml.Input.FocusNavigationDirection.Next);
            return true;
        }
        return false;
    }

    private static partial double PlatformGetKeyboardHeight()
    {
        // Windows on-screen keyboard height is not easily accessible
        // and is managed by the OS
        return 0;
    }
}
#endif
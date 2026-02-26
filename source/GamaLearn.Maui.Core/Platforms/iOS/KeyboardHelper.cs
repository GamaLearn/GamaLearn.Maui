#if IOS
using UIKit;
using Foundation;

namespace GamaLearn.Helpers;

public static partial class KeyboardHelper
{
    private static NSObject? keyboardShowObserver;
    private static NSObject? keyboardHideObserver;
    private static double currentKeyboardHeight;

    static KeyboardHelper()
    {
        InitializeKeyboardObservers();
    }

    private static partial bool PlatformShowKeyboard(View view)
    {
        if (view.Handler?.PlatformView is not UIView platformView)
            return false;

        // Find the first responder (text input)
        UIView? responder = FindFirstResponder(platformView);

        if (responder is UITextField textField)
        {
            textField.BecomeFirstResponder();
            return true;
        }

        if (responder is UITextView textView)
        {
            textView.BecomeFirstResponder();
            return true;
        }

        // Try to make the platform view itself the first responder
        if (platformView.CanBecomeFirstResponder)
        {
            platformView.BecomeFirstResponder();
            return true;
        }

        return false;
    }

    private static partial bool PlatformHideKeyboard(View? view)
    {
        if (view != null && view.Handler?.PlatformView is UIView platformView)
        {
            platformView.ResignFirstResponder();
            return true;
        }

        // Hide keyboard for the entire window
        UIWindow? keyWindow = GetKeyWindow();
        keyWindow?.EndEditing(true);

        return true;
    }

    private static partial double PlatformGetKeyboardHeight()
    {
        return currentKeyboardHeight;
    }

    private static void InitializeKeyboardObservers()
    {
        // Remove existing observers first if any
        CleanupKeyboardObservers();

        keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardWillShow);
        keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardWillHide);
    }

    /// <summary>
    /// Cleans up keyboard observers. Called automatically, but can be called manually if needed.
    /// </summary>
    public static void CleanupKeyboardObservers()
    {
        if (keyboardShowObserver is not null)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardShowObserver);
            keyboardShowObserver.Dispose();
            keyboardShowObserver = null;
        }

        if (keyboardHideObserver is not null)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardHideObserver);
            keyboardHideObserver.Dispose();
            keyboardHideObserver = null;
        }
    }

    private static void OnKeyboardWillShow(NSNotification notification)
    {
        if (notification.UserInfo == null)
            return;

        NSValue? keyboardFrame = notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;

        if (keyboardFrame is not null)
        {
            double height = keyboardFrame.CGRectValue.Height;

            // Convert to device-independent units
            double scale = UIScreen.MainScreen.Scale;
            currentKeyboardHeight = height / scale;

            OnKeyboardShown(currentKeyboardHeight);
        }
    }

    private static void OnKeyboardWillHide(NSNotification notification)
    {
        currentKeyboardHeight = 0;
        OnKeyboardHidden();
    }

    private static UIView? FindFirstResponder(UIView view)
    {
        if (view.IsFirstResponder)
            return view;

        foreach (UIView subview in view.Subviews)
        {
            UIView? responder = FindFirstResponder(subview);
            if (responder != null)
                return responder;
        }

        return null;
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
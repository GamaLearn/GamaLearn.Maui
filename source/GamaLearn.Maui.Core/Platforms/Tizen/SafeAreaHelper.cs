#if TIZEN
namespace GamaLearn.Helpers;

public static partial class SafeAreaHelper
{
    private static partial Thickness PlatformGetSafeAreaInsets()
    {
        // Tizen doesn't typically have notches or system UI overlays
        return Thickness.Zero;
    }

    private static partial Thickness PlatformGetSafeAreaInsets(Page page)
    {
        // Tizen doesn't typically have notches or system UI overlays
        return Thickness.Zero;
    }
}
#endif

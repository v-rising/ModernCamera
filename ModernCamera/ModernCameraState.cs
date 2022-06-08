using ModernCamera.Structs;
using System;

namespace ModernCamera;

internal static class ModernCameraState
{
    internal static IntPtr gamehandle;
    internal static POINT mousePosition;
    internal static bool isInitialized;
    internal static bool isMenuOpen;
    internal static bool isPopupOpen;
    internal static bool isMouseDown;
    internal static bool isFirstPerson;
    internal static bool isMouseLocked;
}

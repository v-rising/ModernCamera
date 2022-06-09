using ModernCamera.Structs;
using System;
using Unity.Entities;
using UnityEngine;

namespace ModernCamera;

internal static class ModernCameraState
{
    internal static IntPtr gamehandle;
    internal static POINT  mousePosition;
    internal static bool   isInitialized;
    internal static bool   isMenuOpen;
    internal static bool   isPopupOpen;
    internal static bool   isMouseDown;
    internal static bool   isMouseDownExternal;
    internal static bool   isFirstPerson;
    internal static bool   isMouseLocked;
    internal static bool isCrosshairShown;

    internal static GameObject cross_hair;

    internal static bool hasWorld;
    internal static World clientWorld;
}

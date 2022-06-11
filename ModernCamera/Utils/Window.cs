using ModernCamera.Structs;
using System;
using System.Runtime.InteropServices;

namespace ModernCamera.Utils;

internal static class Window
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hwnd, ref RECT rectangle);

    public static IntPtr GetWindow(string title)
    {
        return FindWindow(null, title);
    }

    public static RECT GetWindowRect()
    {
        var rect = new RECT();
        GetWindowRect(ModernCameraState.gamehandle, ref rect);
        return rect;
    }

    public static RECT GetClientRect()
    {
        var rect = new RECT();
        GetClientRect(ModernCameraState.gamehandle, ref rect);
        return rect;
    }
}
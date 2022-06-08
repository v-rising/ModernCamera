using ModernCamera.Structs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ModernCamera.Utils;

internal static class Window
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

    public static IntPtr GetWindow(string title)
    {
        return FindWindow(null, title);
    }

    public static RECT GetWindowRect(IntPtr intPtr)
    {
        var rect = new RECT();
        GetWindowRect(intPtr, ref rect);
        return rect;
    }
}
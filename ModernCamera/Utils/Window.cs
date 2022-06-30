using ModernCamera.Structs;
using System;
using System.Runtime.InteropServices;

namespace ModernCamera.Utils;

internal static class Window
{
    internal static IntPtr Handle;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hwnd, ref RECT rectangle);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hwnd, ref POINT point);

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hwnd, ref POINT point);

    static Window()
    {
        Handle = GetWindow("VRising");
    }

    public static IntPtr GetWindow(string title)
    {
        return FindWindow(null, title);
    }

    public static RECT GetWindowRect()
    {
        var rect = new RECT();
        GetWindowRect(Handle, ref rect);
        return rect;
    }

    public static RECT GetClientRect()
    {
        var rect = new RECT();
        GetClientRect(Handle, ref rect);
        return rect;
    }

    public static POINT ClientToScreen(int x, int y)
    {
        var point = new POINT(x, y);
        ClientToScreen(Handle, ref point);
        return point;
    }

    public static POINT ClientToScreen(POINT point)
    {
        return ClientToScreen(point.X, point.Y);
    }

    public static POINT ScreenToClient(int x, int y)
    {
        var point = new POINT(x, y);
        ClientToScreen(Handle, ref point);
        return point;
    }

    public static POINT ScreenToClient(POINT point)
    {
        return ScreenToClient(point.X, point.Y);
    }
}
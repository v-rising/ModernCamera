using ModernCamera.Enums;
using ModernCamera.Structs;
using System.Runtime.InteropServices;

namespace ModernCamera.Utils;

internal static class Mouse
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT point);

    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    internal static bool SetCursorPosition(POINT point)
    {
        return SetCursorPos(point.X, point.Y);
    }

    internal static bool SetCursorPosition(int x, int y)
    {
        return SetCursorPos(x, y);
    }

    internal static POINT GetCursorPosition()
    {
        POINT point;
        GetCursorPos(out point);
        return point;
    }

    internal static void Click(MouseEvent mouseEvent, POINT point)
    {
        mouse_event((int)mouseEvent, point.X, point.Y, 0, 0);
    }

    internal static void Click(MouseEvent mouseEvent, int x, int y)
    {
        mouse_event((int)mouseEvent, x, y, 0, 0);
    }
}
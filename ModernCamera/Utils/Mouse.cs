using ModernCamera.Enums;
using ModernCamera.Structs;
using System.Runtime.InteropServices;
using UnityEngine;

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
        var point = Window.ClientToScreen(x, y);
        return SetCursorPos(point.X, point.Y);
    }

    internal static void CenterCursorPosition()
    {
        var rect = Window.GetClientRect();
        SetCursorPosition((rect.Right - rect.Left) / 2, (rect.Bottom - rect.Top) / 2);
    }

    internal static POINT GetCursorPosition()
    {
        GetCursorPos(out var point);
        return point;
    }

    internal static void Click(MouseEvent mouseEvent)
    {
        Click(mouseEvent, GetCursorPosition());
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
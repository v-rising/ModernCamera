using System.Runtime.InteropServices;

namespace ModernCamera.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    internal int Left;
    internal int Top;
    internal int Right;
    internal int Bottom;

    internal RECT(int Left, int Top, int Right, int Bottom)
    {
        this.Left = Left;
        this.Top = Top;
        this.Right = Right;
        this.Bottom = Bottom;
    }
}

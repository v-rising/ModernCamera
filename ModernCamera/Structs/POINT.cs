using System.Runtime.InteropServices;

namespace ModernCamera.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    internal int X;
    internal int Y;

    internal POINT(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }
}

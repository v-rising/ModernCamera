using System;

namespace ModernCamera.Enums;

[Flags]
internal enum MouseEvent
{
    Absolute = 0x8000, // Use absolute coordinates 
    LeftDown = 0x0002, // The left button is down
    LeftUp = 0x0004, // The left button is up
    MiddleDown = 0x0020, // The middle button is down
    MiddleUp = 0x0040, // The middle button is up
    Move = 0x0001, // Movement occurred
    RightDown = 0x0008, // The right button is down
    RightUp = 0x0010, // The right button is up
}

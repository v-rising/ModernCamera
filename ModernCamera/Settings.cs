using BepInEx.Configuration;
using ModernCamera.Enums;

namespace ModernCamera;

internal static class Settings
{
    private static ConfigEntry<string> _cameraRotateMode;
    internal static CameraRotateMode cameraRotateMode
    {
        get {
            switch (_cameraRotateMode.Value)
            {
                case "toggle":
                case "Toggle":
                    return CameraRotateMode.Toggle;
                case "held":
                case "Held":
                default:
                    return CameraRotateMode.Held;
            }
        }
    }

    private static ConfigEntry<string> _aimMode;
    internal static CameraAimMode aimMode
    {
        get
        {
            switch (_cameraRotateMode.Value)
            {
                case "forward":
                case "Forward":
                    return CameraAimMode.Forward;
                case "default":
                case "Default":
                default:
                    return CameraAimMode.Default;
            }
        }
    }


    internal static float maxZoom = 18f;
    internal static float minZoom = 2f;

    internal static void Load(ConfigFile config)
    {
        _cameraRotateMode = config.Bind("Default", "CameraRotateMode", "Held", "[Toggle|Held] Hold button or toggle camera rotation");
        _cameraRotateMode = config.Bind("Default", "CameraAimMode", "Normal", "[Default|Forward] Aim abilities towards the mouse (default) or in the direction you are looking (forward) when rotating the camera");
    }
}

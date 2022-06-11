using BepInEx.Configuration;
using ModernCamera.Enums;
using UnityEngine;

namespace ModernCamera;

internal static class Settings
{
    private static ConfigEntry<string> _cameraRotateMode;
    internal static CameraRotateMode cameraRotateMode
    {
        get
        {
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
    internal static CameraAimMode cameraAimMode
    {
        get
        {
            switch (_aimMode.Value)
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


    private static ConfigEntry<bool> _invertY;
    internal static bool invertY { get { return _invertY.Value; } }


    private static ConfigEntry<bool> _thirdPersonRoof;
    internal static bool thirdPersonRoof { get { return _thirdPersonRoof.Value; } }


    private static ConfigEntry<bool> _overTheShoulder;
    internal static bool overTheShoulder { get { return _overTheShoulder.Value; } }


    private static ConfigEntry<float> _maxZoom;
    internal static float maxZoom { get { return Mathf.Clamp(_maxZoom.Value, 4, 20); } }


    internal static float firstPersonForwardOffset = 1.65f;
    internal static float headHeightOffset = 0.9f;
    internal static float shoulderRightOffset = 0.8f;
    internal static float minZoom = 2f;

    internal static void Load(ConfigFile config)
    {
        _invertY = config.Bind("Default", "InvertY", false, "[true|false] Invert the Y axis");
        _maxZoom = config.Bind("Default", "MaxZoom", 18f, "[4 - 20] How far the camera can zoom out");
        _cameraRotateMode = config.Bind("Default", "CameraRotateMode", "Held", "[Toggle|Held] Hold button or toggle camera rotation");
        _aimMode = config.Bind("Default", "CameraAimMode", "Normal", "[Default|Forward] Aim abilities towards the mouse (default) or in the direction you are looking (forward) when rotating the camera");
        _thirdPersonRoof = config.Bind("Default", "ThirdPersonRoof", false, "[true|false] Should roofs be visible when inside in third person");
        _overTheShoulder = config.Bind("Default", "OverTheShoulder", false, "[true|false] Should third person view use an over the shoulder offset");
    }
}

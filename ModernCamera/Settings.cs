using BepInEx.Configuration;

namespace ModernCamera;

internal static class Settings
{
    private static ConfigEntry<float> _thirdPersonMaxPitch;
    private static ConfigEntry<float> _thirdPersonMinPitch;
    private static ConfigEntry<float> _thirdPersonMaxZoom;
    private static ConfigEntry<float> _thirdPersonMinZoom;

    private static ConfigEntry<float> _firstPersonMaxPitch;
    private static ConfigEntry<float> _firstPersonMinPitch;

    internal static float thirdPersonMaxPitch { get { return _thirdPersonMaxPitch.Value; } }
    internal static float thirdPersonMinPitch { get { return _thirdPersonMinPitch.Value; } }
    internal static float thirdPersonMaxZoom { get { return _thirdPersonMaxZoom.Value; } }
    internal static float thirdPersonMinZoom { get { return _thirdPersonMinZoom.Value; } }

    internal static float firstPersonMaxPitch { get { return _firstPersonMaxPitch.Value; } }
    internal static float firstPersonMinPitch { get { return _firstPersonMinPitch.Value; } }

    internal static void Load(ConfigFile config)
    {
        _thirdPersonMaxPitch = config.Bind("Third Person", "MaxPitch", 1.57f, "(Radians) 1.57 = looking directly down at the character");
        _thirdPersonMinPitch = config.Bind("Third Person", "MinPitch", .17f, "(Radians) Below .17 may cause shadow flickering, 0 = looking forward");
        _thirdPersonMaxZoom = config.Bind("Third Person", "MaxZoom", 15f, "(Unity Units) How far the camera can zoom out");
        _thirdPersonMinZoom = config.Bind("Third Person", "MinZoom", 2f, "(Unity Units) How far the camera can zoom in while in third person view");

        _firstPersonMaxPitch = config.Bind("First Person", "MaxPitch", 1.57f, "(Radians) 1.57 = looking directly down");
        _firstPersonMinPitch = config.Bind("First Person", "MinPitch", -1.57f, "(Radians) -1.57 = looking directly up");
    }
}

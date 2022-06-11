using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using ModernCamera.Hooks;
using UnhollowerRuntimeLib;

namespace ModernCamera;

[BepInProcess("VRising.exe")]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static ManualLogSource Logger;

    private static Harmony harmony;

    public override void Load()
    {
        Logger = Log;

        Settings.Load(Config);

        ClassInjector.RegisterTypeInIl2Cpp<ModernCamera>();
        AddComponent<ModernCamera>();

        TopdownCameraSystem_Hook.CreateAndApply();

        harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Log.LogInfo($"Plugin Travanti - Inc - Modern - Camera is loaded!");
    }

    public override bool Unload()
    {
        harmony.UnpatchSelf();

        TopdownCameraSystem_Hook.Dispose();

        return true;
    }
}

using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using ModernCamera.Hooks;
using UnhollowerRuntimeLib;

namespace ModernCamera;

[BepInProcess("VRising.exe")]
[BepInDependency("iZastic.Silkworm")]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static ManualLogSource Logger;

    private static Harmony Harmony;

    public override void Load()
    {
        Logger = Log;

        Settings.Init();

        ClassInjector.RegisterTypeInIl2Cpp<ModernCamera>();
        AddComponent<ModernCamera>();

        TopdownCameraSystem_Hook.CreateAndApply();

        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} is loaded!");
    }

    public override bool Unload()
    {
        Harmony.UnpatchSelf();

        TopdownCameraSystem_Hook.Dispose();

        return true;
    }
}

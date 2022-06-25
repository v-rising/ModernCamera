using HarmonyLib;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class TopdownCameraSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(TopdownCameraSystem), nameof(TopdownCameraSystem.OnUpdate))]
    private static void OnUpdate(TopdownCameraSystem __instance)
    {
        if (Settings.Enabled)
            __instance._ZoomModifierSystem._ZoomModifiers.Clear();
    }
}

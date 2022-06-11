using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class ActionWheelSystem_Patch
{
    private static bool wheelVisible;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.OnUpdate))]
    private static void OnUpdate(ActionWheelSystem __instance)
    {
        if (!wheelVisible && (__instance._ActionWheel.IsVisible() || __instance._EmoteWheel.IsVisible()))
        {
            ModernCameraState.isMenuOpen = true;
            wheelVisible = true;
        }
        else if (wheelVisible && !__instance._ActionWheel.IsVisible() && !__instance._EmoteWheel.IsVisible())
        {
            ModernCameraState.isMenuOpen = false;
            wheelVisible = false;
        }
    }
}

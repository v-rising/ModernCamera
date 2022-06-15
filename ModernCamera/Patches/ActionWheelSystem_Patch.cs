using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class ActionWheelSystem_Patch
{
    private static bool WheelVisible;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.OnUpdate))]
    private static void OnUpdate(ActionWheelSystem __instance)
    {
        if (!WheelVisible && (__instance._ActionWheel.IsVisible() || __instance._EmoteWheel.IsVisible()))
        {
            ModernCameraState.IsMenuOpen = true;
            WheelVisible = true;
        }
        else if (WheelVisible && !__instance._ActionWheel.IsVisible() && !__instance._EmoteWheel.IsVisible())
        {
            ModernCameraState.IsMenuOpen = false;
            WheelVisible = false;
        }
    }
}

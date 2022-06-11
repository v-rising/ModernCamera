using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class HUDMenu_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnEnable))]
    private static void OnEnable() => ModernCameraState.isMenuOpen = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnDisable))]
    private static void OnDisable() => ModernCameraState.isMenuOpen = false;
}

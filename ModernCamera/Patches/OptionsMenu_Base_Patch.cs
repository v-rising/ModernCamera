using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class OptionsMenu_Base_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.Start))]
    private static void Start() => ModernCameraState.isMenuOpen = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.OnDestroy))]
    private static void OnDestroy() => ModernCameraState.isMenuOpen = false;
}

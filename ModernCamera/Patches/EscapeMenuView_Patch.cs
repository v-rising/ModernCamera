using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal class EscapeMenuView_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnEnable))]
    private static void OnEnable(EscapeMenuView __instance) => ModernCameraState.isMenuOpen = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnDestroy))]
    private static void OnDestroy(EscapeMenuView __instance) => ModernCameraState.isMenuOpen = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_LeaveGame))]
    private static void OnButtonClick_LeaveGame()
    {
        ModernCameraState.isMenuOpen = false;
    }
}

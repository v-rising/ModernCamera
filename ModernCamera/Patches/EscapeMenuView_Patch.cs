using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal class EscapeMenuView_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnEnable))]
    private static void OnEnable() => ModernCameraState.IsMenuOpen = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnDestroy))]
    private static void OnDestroy() => ModernCameraState.IsMenuOpen = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_LeaveGame))]
    private static void OnButtonClick_LeaveGame()
    {
        ModernCameraState.IsMouseLocked = false;
        ModernCameraState.IsMenuOpen = false;
    }
}

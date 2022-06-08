using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class OpenHUDMenuSystem_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(OpenHUDMenuSystem), nameof(OpenHUDMenuSystem.OnUpdate))]
    private static void OnUpdate(OpenHUDMenuSystem __instance) => ModernCameraState.isMenuOpen = __instance.CurrentMenuType != HUDMenuType.None;
}

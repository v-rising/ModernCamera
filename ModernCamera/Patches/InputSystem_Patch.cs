using HarmonyLib;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class InputSystem_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InputSystem), nameof(InputSystem.SetupMergedSettings))]
    private static void SetupMergedSettings(InputSystem __instance)
    {
        ModernCameraState.UpdateInputSettings(__instance.MergedInputSettings);
    }
}

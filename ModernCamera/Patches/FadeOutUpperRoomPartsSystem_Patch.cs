using HarmonyLib;
using ProjectM.Presentation;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class FadeOutUpperRoomPartsSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FadeOutUpperRoomPartsSystem), nameof(FadeOutUpperRoomPartsSystem.OnUpdate))]
    private static void OnUpdate(FadeOutUpperRoomPartsSystem __instance)
    {
        if (ModernCameraState.IsFirstPerson || Settings.ThirdPersonRoof)
            __instance.SetFadeOutMode(FadeOutModeEnum.None);
        else
            __instance.SetFadeOutMode(FadeOutModeEnum.Full);
    }
}

using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches
{
    [HarmonyPatch]
    internal static class SetHUDElementVisibilitySystem_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SetHUDElementVisibilitySystem), nameof(SetHUDElementVisibilitySystem.OnUpdate))]
        private static void OnUpdatePostfix(SetHUDElementVisibilitySystem __instance)
        {
            if (__instance._UIDataSystem == null ||
                __instance._UIDataSystem.UI == null ||
                __instance._UIDataSystem.UI.CanvasBase == null) return;

            if (ModernCameraState.IsUIHidden)
                __instance._UIDataSystem.UI.CanvasBase.gameObject.SetActive(false);
        }
    }
}

using HarmonyLib;
using ProjectM.UI;
using UnityEngine;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class HUDElementComponent_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDElementComponent), nameof(HUDElementComponent.UpdateVisibility))]
    private static void UpdateVisibility(HUDElementComponent __instance)
    {
        var canvasGroup = __instance.GetComponentInChildren<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }
}

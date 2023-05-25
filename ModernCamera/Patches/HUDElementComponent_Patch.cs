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
        if (__instance.gameObject.name.Equals("InteractorEntry(Clone)"))
        {
            foreach (var canvasGroup in __instance.GetComponentsInChildren<CanvasGroup>())
                canvasGroup.alpha = 1f;
        }
    }
}

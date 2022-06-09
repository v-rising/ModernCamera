using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class PlayJoinMenuView_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayJoinMenuView), nameof(PlayJoinMenuView.Update))]
    private static void Update(PlayJoinMenuView __instance)
    {
        if (__instance._SelectedSession != null)
        {
            // Disables the join button if the selected server is an official server
            var official = __instance._SelectedEntry.Value.Data.IsOfficial;
            if (official && __instance.JoinButton.enabled) __instance.JoinButton.enabled = false;
            else if (!official && !__instance.JoinButton.enabled) __instance.JoinButton.enabled = true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayJoinMenuView), nameof(PlayJoinMenuView.OnButtonClick_Join))]
    // Prevent joining an official server if some how the player was able to click join
    private static bool OnButtonClick_Join(PlayJoinMenuView __instance) => !__instance._SelectedEntry.Value.Data.IsOfficial;
}

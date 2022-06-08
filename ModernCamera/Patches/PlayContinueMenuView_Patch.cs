using HarmonyLib;
using ProjectM.UI;

namespace ModernCamera.Patches
{
    [HarmonyPatch]
    internal static class PlayContinueMenuView_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayContinueMenuView), nameof(PlayContinueMenuView.Update))]
        private static void Update(PlayContinueMenuView __instance)
        {
            if (__instance._SelectedSession != null)
            {
                // Disables the join button if the selected server is an official server
                var official = __instance._SelectedServerEntry.Value.Data.IsOfficial;
                if (official && __instance.JoinButton.enabled) __instance.JoinButton.enabled = false;
                else if (!official && !__instance.JoinButton.enabled) __instance.JoinButton.enabled = true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayContinueMenuView), nameof(PlayContinueMenuView.OnButtonClick_Join))]
        // Prevent joining an official server if some how the player was able to click join
        private static bool OnButtonClick_Join(PlayContinueMenuView __instance) => !__instance._SelectedServerEntry.Value.Data.IsOfficial;
    }
}

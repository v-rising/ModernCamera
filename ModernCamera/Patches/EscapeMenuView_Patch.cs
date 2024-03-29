﻿using HarmonyLib;
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
}

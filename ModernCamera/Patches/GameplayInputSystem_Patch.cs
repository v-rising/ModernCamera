using HarmonyLib;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class GameplayInputSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
    private static void HandleInputPrefix(ref InputState inputState)
    {
        ModernCameraState.GameplayInputState = inputState;

        if (!Settings.Enabled) return;
        if (ModernCameraState.IsMouseLocked && !ModernCameraState.IsMenuOpen && !inputState.IsInputPressed(InputFlag.RotateCamera))
        {
            inputState.InputsPressed |= InputFlag.RotateCamera;
        }
    }
}

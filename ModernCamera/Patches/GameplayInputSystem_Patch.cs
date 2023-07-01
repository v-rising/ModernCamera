using HarmonyLib;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class GameplayInputSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
    private static unsafe void HandleInputPrefix(ref InputState inputState)
    {
        ModernCameraState.ValidGameplayInputState = true;
        ModernCameraState.GameplayInputState = inputState;

        if (!Settings.Enabled) return;
        if (ModernCameraState.IsMouseLocked && !ModernCameraState.IsMenuOpen && !inputState.IsInputPressed(InputFlag.RotateCamera))
        {
            inputState.InputsPressed.m_ListData->Add(InputFlag.RotateCamera);
        }
    }
}

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
        if (ModernCameraState.isMouseLocked && !ModernCameraState.isMenuOpen && !inputState.IsInputPressed(InputFlag.RotateCamera))
        {
            inputState.InputsPressed |= InputFlag.RotateCamera;
        }
    }
}

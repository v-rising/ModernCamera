using BepInEx.IL2CPP.Hook;
using ProjectM;
using Silkworm.Utils;
using System;
using System.Runtime.InteropServices;

namespace ModernCamera.Hooks;

#nullable enable
internal static class TopdownCameraSystem_Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void UpdateCameraInputs(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData);
    private static UpdateCameraInputs? UpdateCameraInputsOriginal;
    private static FastNativeDetour? UpdateCameraInputsDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void HandleInput(IntPtr _this, InputState* inputState);
    private static HandleInput? HandleInputOriginal;
    private static FastNativeDetour? HandleInputDetour;

    internal static unsafe void CreateAndApply()
    {
        if (UpdateCameraInputsDetour == null)
        {
            UpdateCameraInputsDetour = DetourUtils.Create(typeof(TopdownCameraSystem), "UpdateCameraInputs", "OriginalLambdaBody", UpdateCameraInputsHook, out UpdateCameraInputsOriginal);
        }

        if (HandleInputDetour == null)
        {
            HandleInputDetour = DetourUtils.Create(typeof(TopdownCameraSystem), "HandleInput", HandleInputHook, out HandleInputOriginal);
        }
    }

    internal static void Dispose()
    {
        UpdateCameraInputsDetour?.Dispose();
        HandleInputDetour?.Dispose();
    }

    private static unsafe void HandleInputHook(IntPtr _this, InputState* inputState)
    {
        ModernCameraState.CurrentCameraBehaviour!.HandleInput(ref *inputState);

        HandleInputOriginal!(_this, inputState);
    }

    private static unsafe void UpdateCameraInputsHook(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData)
    {
        ModernCameraState.CurrentCameraBehaviour!.ProbablyShapeshiftedOrMounted = cameraState->ZoomSettings.MinZoom > 0;

        // Set zoom settings
        cameraState->ZoomSettings.MaxZoom = Settings.MaxZoom;
        cameraState->ZoomSettings.MinZoom = 0f;

        // Check camera behaviours for activation
        foreach (var behaviour in ModernCameraState.CameraBehaviours.Values)
        {
            if (behaviour.ShouldActivate(ref *cameraState))
            {
                ModernCameraState.CurrentCameraBehaviour!.Deactivate();
                behaviour.CameraSystem = new TopdownCameraSystem(_this);
                behaviour.Activate(ref *cameraState);
                break;
            }
        }

        // Update current camera behaviour
        if (ModernCameraState.CurrentCameraBehaviour != null)
        {
            ModernCameraState.CurrentCameraBehaviour.CameraSystem = new TopdownCameraSystem(_this);

            if (!ModernCameraState.CurrentCameraBehaviour.Active)
                ModernCameraState.CurrentCameraBehaviour.Activate(ref *cameraState);
            
            ModernCameraState.CurrentCameraBehaviour.UpdateCameraInputs(ref *cameraState, ref *cameraData);
        }

        cameraData->StandardZoomSettings = cameraState->ZoomSettings;

        UpdateCameraInputsOriginal!(_this, cameraState, cameraData);
    }
}

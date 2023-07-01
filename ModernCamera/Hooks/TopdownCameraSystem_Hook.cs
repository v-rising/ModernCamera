using BepInEx.Unity.IL2CPP.Hook;
using ProjectM;
using Silkworm.Utils;
using System;
using System.Runtime.InteropServices;

namespace ModernCamera.Hooks;

#nullable enable
internal static class TopdownCameraSystem_Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void HandleInput(IntPtr _this, ref InputState inputState);
    private static HandleInput? HandleInputOriginal;
    private static INativeDetour? HandleInputDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void UpdateCameraInputs(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData);
    private static UpdateCameraInputs? UpdateCameraInputsOriginal;
    private static INativeDetour? UpdateCameraInputsDetour;

    private static bool DefaultZoomSettingsSaved;
    private static bool UsingDefaultZoomSettings;
    private static ZoomSettings DefaultZoomSettings;
    private static ZoomSettings DefaultStandardZoomSettings;
    private static ZoomSettings DefaultBuildModeZoomSettings;

    internal static unsafe void CreateAndApply()
    {
        if (HandleInputDetour == null)
        {
            HandleInputDetour = DetourUtils.Create(typeof(TopdownCameraSystem), "HandleInput", HandleInputHook, out HandleInputOriginal);
        }

        if (UpdateCameraInputsDetour == null)
        {
            UpdateCameraInputsDetour = DetourUtils.Create(typeof(TopdownCameraSystem), "UpdateCameraInputs", "OriginalLambdaBody", UpdateCameraInputsHook, out UpdateCameraInputsOriginal);
        }
    }

    internal static void Dispose()
    {
        UpdateCameraInputsDetour?.Dispose();
        HandleInputDetour?.Dispose();
    }

    private static unsafe void HandleInputHook(IntPtr _this, ref InputState inputState)
    {
        if (Settings.Enabled)
        {
            ModernCameraState.CurrentCameraBehaviour?.HandleInput(ref inputState);
        }

        HandleInputOriginal!(_this, ref inputState);
    }

    private static unsafe void UpdateCameraInputsHook(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData)
    {
        if (Settings.Enabled)
        {
            if (!DefaultZoomSettingsSaved)
            {
                DefaultZoomSettings = cameraState.ZoomSettings;
                DefaultStandardZoomSettings = cameraData.StandardZoomSettings;
                DefaultBuildModeZoomSettings = cameraData.BuildModeZoomSettings;
                DefaultZoomSettingsSaved = true;
            }
            UsingDefaultZoomSettings = false;

            // Set zoom settings
            cameraState.ZoomSettings.MaxZoom = Settings.MaxZoom;
            cameraState.ZoomSettings.MinZoom = 0f;

            // Check camera behaviours for activation
            foreach (var behaviour in ModernCameraState.CameraBehaviours.Values)
            {
                if (behaviour.ShouldActivate(ref cameraState))
                {
                    ModernCameraState.CurrentCameraBehaviour?.Deactivate();
                    behaviour.Activate(ref cameraState);
                    break;
                }
            }

            // Update current camera behaviour
            if (!ModernCameraState.CurrentCameraBehaviour!.Active)
                ModernCameraState.CurrentCameraBehaviour!.Activate(ref cameraState);

            ModernCameraState.CurrentCameraBehaviour!.UpdateCameraInputs(ref cameraState, ref cameraData);

            cameraData.StandardZoomSettings = cameraState.ZoomSettings;
        }
        else if (DefaultZoomSettingsSaved && !UsingDefaultZoomSettings)
        {
            cameraState.ZoomSettings = DefaultZoomSettings;
            cameraData.StandardZoomSettings = DefaultStandardZoomSettings;
            cameraData.BuildModeZoomSettings = DefaultBuildModeZoomSettings;
            UsingDefaultZoomSettings = true;
        }

        UpdateCameraInputsOriginal!(_this, ref cameraState, ref cameraData);
    }
}

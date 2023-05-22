﻿using BepInEx.Unity.IL2CPP.Hook;
using MonoMod.RuntimeDetour;
using ProjectM;
using Silkworm.Utils;
using System;
using System.Runtime.InteropServices;

namespace ModernCamera.Hooks;

#nullable enable
internal static class TopdownCameraSystem_Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void HandleInput(IntPtr _this, InputState* inputState);
    private static HandleInput? HandleInputOriginal;
    private static NativeDetour? HandleInputDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void UpdateCameraInputs(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData);
    private static UpdateCameraInputs? UpdateCameraInputsOriginal;
    private static NativeDetour? UpdateCameraInputsDetour;

    private static bool DefaultZoomSettingsSaved;
    private static bool UsingDefaultZoomSettings;
    private static ZoomSettings DefaultZoomSettings;
    private static ZoomSettings DefaultStandardZoomSettings;

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

    private static unsafe void HandleInputHook(IntPtr _this, InputState* inputState)
    {
        if (Settings.Enabled)
        {
            ModernCameraState.CurrentCameraBehaviour!.HandleInput(ref *inputState);
        }

        HandleInputOriginal!(_this, inputState);
    }

    private static unsafe void UpdateCameraInputsHook(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData)
    {
        if (Settings.Enabled)
        {
            if (!DefaultZoomSettingsSaved)
            {
                DefaultZoomSettings = cameraState->ZoomSettings;
                DefaultStandardZoomSettings = cameraData->StandardZoomSettings;
                DefaultZoomSettingsSaved = true;
            }
            UsingDefaultZoomSettings = false;

            // Set zoom settings
            cameraState->ZoomSettings.MaxZoom = Settings.MaxZoom;
            cameraState->ZoomSettings.MinZoom = 0f;

            // Check camera behaviours for activation
            foreach (var behaviour in ModernCameraState.CameraBehaviours.Values)
            {
                if (behaviour.ShouldActivate(ref *cameraState))
                {
                    ModernCameraState.CurrentCameraBehaviour!.Deactivate();
                    behaviour.Activate(ref *cameraState);
                    break;
                }
            }

            // Update current camera behaviour
            if (!ModernCameraState.CurrentCameraBehaviour!.Active)
                ModernCameraState.CurrentCameraBehaviour!.Activate(ref *cameraState);

            ModernCameraState.CurrentCameraBehaviour!.UpdateCameraInputs(ref *cameraState, ref *cameraData);

            cameraData->StandardZoomSettings = cameraState->ZoomSettings;
        }
        else if (DefaultZoomSettingsSaved && !UsingDefaultZoomSettings)
        {
            cameraState->ZoomSettings = DefaultZoomSettings;
            cameraData->StandardZoomSettings = DefaultStandardZoomSettings;
            UsingDefaultZoomSettings = true;
        }

        UpdateCameraInputsOriginal!(_this, cameraState, cameraData);
    }
}

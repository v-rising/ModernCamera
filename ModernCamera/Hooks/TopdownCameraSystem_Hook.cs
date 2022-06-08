using BepInEx.IL2CPP.Hook;
using ModernCamera.Structs;
using ModernCamera.Utils;
using ProjectM;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ModernCamera.Hooks;

#nullable enable
internal static class TopdownCameraSystem_Hook
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void UpdateCameraInputs(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData);
    private static UpdateCameraInputs? UpdateCameraInputsOriginal;
    private static FastNativeDetour? UpdateCameraInputsDetour;

    internal static unsafe void CreateAndApply()
    {
        if (UpdateCameraInputsDetour == null)
        {
            UpdateCameraInputsDetour = NativeDetour.Create(typeof(TopdownCameraSystem), "UpdateCameraInputs", "OriginalLambdaBody", UpdateCameraInputsHook, out UpdateCameraInputsOriginal);
        }
    }

    internal static void Dispose()
    {
        UpdateCameraInputsDetour?.Dispose();
    }

    private static unsafe void UpdateCameraInputsHook(IntPtr _this, TopdownCameraState* cameraState, TopdownCamera* cameraData)
    {
        cameraState->ZoomSettings.MaxPitch = Settings.thirdPersonMaxPitch;
        cameraState->ZoomSettings.MinPitch = Settings.thirdPersonMinPitch;
        cameraState->ZoomSettings.MaxZoom = Settings.thirdPersonMaxZoom;
        cameraState->ZoomSettings.MinZoom = -1.0f;

        var flag = cameraState->Current.Zoom > 0.0;
        var num = flag ? cameraState->Current.Zoom / cameraState->ZoomSettings.MaxZoom : 0.0f;
        var lookat = cameraState->Current.LookAtRootPos;
        var lmod = 0.085f;
        var pc = cameraState->Current.Pitch / (cameraState->ZoomSettings.MaxPitch / 100);
        if (pc <= 0) pc = 0;

        var yx = Mathf.SmoothStep(lookat.y, lookat.y + lmod, (100 - pc) * 0.01f);
        var yz = Mathf.SmoothStep(1.24f, 1.80f, (100 - pc) * 0.01f);

        lookat.y = (float)Math.Round(yx, 2);
        if (cameraState->Current.Zoom < 0.8f)
        {
            if (!ModernCameraState.isInitialized)
            {
                ModernCameraState.isInitialized = true;
                ModernCameraState.isMenuOpen = false;
            }

            ModernCameraState.isFirstPerson = true;
            cameraState->ZoomSettings.MinPitch = -1.5f;
            cameraState->Current.Zoom = -1.0f;
            cameraState->Current.NormalizedLookAtOffset.y = flag ? Mathf.Lerp(1f, 0.0f, num) : 0.0f;
        }
        else
        {
            ModernCameraState.isFirstPerson = false;
            cameraState->Current.NormalizedLookAtOffset.y = 0.0f;
            cameraData->LookAtHeight = (float)Math.Round(yz, 2);
        }

        cameraData->StandardZoomSettings = cameraState->ZoomSettings;

        UpdateCameraInputsOriginal!(_this, cameraState, cameraData);
    }
}

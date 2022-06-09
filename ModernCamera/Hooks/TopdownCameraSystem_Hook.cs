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
        float OrigZoomLambda = 12f;
        cameraState->ZoomSettings.MaxPitch = 1.5f; // temporary for debugging
        cameraState->ZoomSettings.MinPitch = 0.0f;
        cameraState->ZoomSettings.MaxZoom = cameraState->ZoomSettings.MinZoom > 0.0 ? 14f : 30f;
        cameraState->ZoomSettings.MinZoom = -1.0f;

        var flag = cameraState->Current.Zoom > 0.0;
        var num = flag ? cameraState->Current.Zoom / cameraState->ZoomSettings.MaxZoom : 0.0f;
        var lookat = cameraState->Current.LookAtRootPos;
        var lmod = 0.085f;
        var pc = cameraState->Current.Pitch / (cameraState->ZoomSettings.MaxPitch / 100);
        if (pc <= 0) pc = 0;

        var yx = Mathf.SmoothStep(lookat.y, lookat.y + lmod, (100 - pc) * 0.01f);
        var yz = Mathf.SmoothStep(1.24f, 1.80f, (100 - pc) * 0.01f);
        var zoomlamsmo = Mathf.Lerp(12f, 5f, (100 - pc) * 0.01f);
      //  Plugin.Logger.LogError($"{zoomlamsmo}");
        if (cameraState->InstantJump)
        {
            Plugin.Logger.LogError("insta jump");
           // UpdateCameraInputsOriginal!(_this, cameraState, cameraData);
        }
       
       
        lookat.y = (float)Math.Round(yx, 2);
        if (cameraState->Current.Zoom < .8f)
        {
            if (!ModernCameraState.isInitialized)
            {
                ModernCameraState.isInitialized = true;
                ModernCameraState.isMenuOpen = false;
            }

            if (!ModernCameraState.isFirstPerson)
            {
                ModernCameraState.isFirstPerson = true;
               
                cameraState->Current.Pitch = 0.0f;
                cameraState->Current.Zoom = -1.1f;
            }
            else
            {
                
                cameraData->LerpLambdas.ZoomLambda = OrigZoomLambda;
            }
            cameraState->ZoomSettings.MinPitch = -1.5f;
            cameraState->ZoomSettings.MinZoom = -1.1f;
            cameraState->Current.NormalizedLookAtOffset.y = flag ? Mathf.Lerp(1f, 0.0f, num) : 0.0f;
        }
        else
        {
            
            if (ModernCameraState.isFirstPerson)
            {
                cameraState->ZoomSettings.MinPitch = Settings.thirdPersonMinPitch;
                cameraState->Current.Zoom = 1.5f;
                ModernCameraState.isFirstPerson = false;
            }
           // Plugin.Logger.LogError($"{cameraData->LerpLambdas.ZoomLambda}");
           // 
            cameraState->Current.NormalizedLookAtOffset.y = 0.0f;
            cameraData->LookAtHeight = (float)Math.Round(yz, 2);
            cameraData->LerpLambdas.ZoomLambda =zoomlamsmo;
        }
        if (cameraState->InBuildMode)
        {
            Plugin.Logger.LogError("In build");
            

        }
       
        cameraData->BuildModeZoomSettings = cameraState->ZoomSettings;
        cameraData->StandardZoomSettings = cameraState->ZoomSettings;
        
        
        UpdateCameraInputsOriginal!(_this, cameraState, cameraData);
    }
}

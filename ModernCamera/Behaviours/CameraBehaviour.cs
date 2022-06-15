using ModernCamera.Enums;
using ProjectM;
using UnityEngine;

namespace ModernCamera.Behaviours;

internal abstract class CameraBehaviour
{
    internal TopdownCameraSystem CameraSystem;
    internal BehaviourType BehaviourType;
    internal ZoomSettings BuildModeZoomSettings;
    internal float DefaultMaxPitch;
    internal float DefaultMinPitch;
    internal bool ProbablyShapeshiftedOrMounted;
    internal bool Active;

    protected bool InBuildMode;
    protected bool IsBuildSettingsSet;
    protected float TargetZoom = Settings.MaxZoom / 2;

    internal virtual void Activate(ref TopdownCameraState state)
    {
        Active = true;
    }

    internal virtual void Deactivate()
    {
        Active = false;
    }

    internal virtual bool ShouldActivate(ref TopdownCameraState state) => false;

    internal virtual void HandleInput(ref InputState inputState)
    {
        if (ModernCameraState.IsMouseLocked && !ModernCameraState.IsMenuOpen && !inputState.IsInputPressed(InputFlag.RotateCamera))
        {
            inputState.InputsPressed |= InputFlag.RotateCamera;
        }

        // Manually manage camera zoom
        var zoomVal = inputState.GetAnalogValue(AnalogInput.ZoomCamera);
        if (zoomVal != 0 && (!InBuildMode || !Settings.DefaultBuildMode))
        {
            // Consume zoom input for the camera
            var zoomChange = inputState.GetAnalogValue(AnalogInput.ZoomCamera) > 0 ? 2 : -2;
            if ((TargetZoom > Settings.MinZoom && TargetZoom + zoomChange < Settings.MinZoom) || (ModernCameraState.IsFirstPerson && zoomChange > 0))
                TargetZoom = Settings.MinZoom;
            else
                TargetZoom = Mathf.Clamp(TargetZoom + zoomChange, Settings.FirstPersonEnabled ? 0 : Settings.MinZoom, Settings.MaxZoom);
            inputState.SetAnalogValue(AnalogInput.ZoomCamera, 0);
        }

        if (Settings.InvertY)
            inputState.SetAnalogValue(AnalogInput.RotateCameraY, -inputState.GetAnalogValue(AnalogInput.RotateCameraY));
    }

    internal virtual void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        InBuildMode = state.InBuildMode;
        if (!IsBuildSettingsSet)
        {
            BuildModeZoomSettings = data.BuildModeZoomSettings;
            IsBuildSettingsSet = true;
        }

        // Set camera behaviour pitch settings
        state.ZoomSettings.MaxPitch = DefaultMaxPitch;
        state.ZoomSettings.MinPitch = DefaultMinPitch;

        // Manually set zoom if not in build mode
        if (!state.InBuildMode || !Settings.DefaultBuildMode)
        {
            data.BuildModeZoomSettings.MaxPitch = DefaultMaxPitch;
            data.BuildModeZoomSettings.MinPitch = DefaultMinPitch;
            state.LastTarget.Zoom = TargetZoom;
            state.Target.Zoom = TargetZoom;
        }

        if (state.InBuildMode && Settings.DefaultBuildMode)
        {
            data.BuildModeZoomSettings = BuildModeZoomSettings;
            state.LastTarget.Zoom = data.BuildModeZoomDistance;
            state.Target.Zoom = data.BuildModeZoomDistance;
        }
    }
}

using ModernCamera.Enums;
using ProjectM;
using ProjectM.Sequencer;
using UnityEngine;

namespace ModernCamera.Behaviours;

internal abstract class CameraBehaviour
{
    internal TopdownCameraSystem cameraSystem;
    internal BehaviourType type;
    internal float defaultMaxPitch;
    internal float defaultMinPitch;
    internal bool probablyShapeshiftedOrMounted;
    internal bool active;

    protected float targetZoom = Settings.maxZoom / 2;

    internal virtual void Activate(ref TopdownCameraState state)
    {
        active = true;
    }

    internal virtual void Deactivate()
    {
        active = false;
    }

    internal virtual bool ShouldActivate(ref TopdownCameraState state) => false;

    internal virtual void HandleInput(ref InputState inputState)
    {
        if (ModernCameraState.isMouseLocked && !ModernCameraState.isMenuOpen && !inputState.IsInputPressed(InputFlag.RotateCamera))
        {
            inputState.InputsPressed |= InputFlag.RotateCamera;
        }

        // Manually manage camera zoom
        var zoomVal = inputState.GetAnalogValue(AnalogInput.ZoomCamera);
        if (zoomVal != 0)
        {
            // Consume zoom input for the camera
            var zoomChange = inputState.GetAnalogValue(AnalogInput.ZoomCamera) > 0 ? 2 : -2;
            if (targetZoom > Settings.minZoom && targetZoom + zoomChange < Settings.minZoom)
                targetZoom = Settings.minZoom;
            else
                targetZoom = Mathf.Clamp(targetZoom + zoomChange, 0, Settings.maxZoom);
            inputState.SetAnalogValue(AnalogInput.ZoomCamera, 0);
        }

        if (Settings.invertY)
            inputState.SetAnalogValue(AnalogInput.RotateCameraY, -inputState.GetAnalogValue(AnalogInput.RotateCameraY));
    }

    internal virtual void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        // Set camera behaviour pitch settings
        state.ZoomSettings.MaxPitch = defaultMaxPitch;
        state.ZoomSettings.MinPitch = defaultMinPitch;

        // Manually set zoom if not in build mode
        if (!state.InBuildMode)
        {
            state.LastTarget.Zoom = targetZoom;
            state.Target.Zoom = targetZoom;
        }
    }
}

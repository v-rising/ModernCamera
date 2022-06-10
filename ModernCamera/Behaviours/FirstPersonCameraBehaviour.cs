using ModernCamera.Enums;
using ProjectM;

namespace ModernCamera.Behaviours;

internal class FirstPersonCameraBehaviour : CameraBehaviour
{
    internal FirstPersonCameraBehaviour()
    {
        type = BehaviourType.FirstPerson;
        defaultMaxPitch = 1.57f;
        defaultMinPitch = -1.57f;
    }

    internal override void Activate(ref TopdownCameraState state)
    {
        base.Activate(ref state);

        ModernCameraState.isMouseLocked = true;
        ModernCameraState.isFirstPerson = true;
        ModernCameraState.currentBehaviourType = type;
        state.PitchPercent = 0.5f;
        targetZoom = 0;
    }

    internal override void Deactivate()
    {
        base.Deactivate();

        ModernCameraState.isMouseLocked = false;
        ModernCameraState.isFirstPerson = false;
    }

    internal override bool ShouldActivate(ref TopdownCameraState state)
    {
        return ModernCameraState.currentBehaviourType != type && state.Target.Zoom < Settings.minZoom;
    }

    internal override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        state.LastTarget.NormalizedLookAtOffset.z = 1.65f;
        state.LastTarget.NormalizedLookAtOffset.y = 0.9f;
    }
}

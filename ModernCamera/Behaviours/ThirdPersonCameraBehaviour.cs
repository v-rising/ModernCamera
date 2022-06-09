using ModernCamera.Enums;
using ProjectM;

namespace ModernCamera.Behaviours;

internal class ThirdPersonCameraBehaviour : CameraBehaviour
{
    internal ThirdPersonCameraBehaviour()
    {
        type = BehaviourType.ThirdPerson;
        defaultMaxPitch = 1.57f;
        defaultMinPitch = 0.0f;
    }

    internal override void Activate(ref TopdownCameraState state)
    {
        base.Activate(ref state);

        ModernCameraState.currentBehaviourType = type;
        state.PitchPercent = 0.5f;
        targetZoom = Settings.minZoom;
    }

    internal override bool ShouldActivate(ref TopdownCameraState state)
    {
        return ModernCameraState.currentBehaviourType != type && state.Target.Zoom > state.ZoomSettings.MinZoom;
    }

    internal override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        state.LastTarget.NormalizedLookAtOffset.y = 0.9f;
    }
}

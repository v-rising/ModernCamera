using ModernCamera.Enums;
using ProjectM;

namespace ModernCamera.Behaviours;

internal class FirstPersonCameraBehaviour : CameraBehaviour
{
    internal FirstPersonCameraBehaviour()
    {
        BehaviourType = BehaviourType.FirstPerson;
        DefaultMaxPitch = 1.57f;
        DefaultMinPitch = -1.57f;
    }

    internal override void Activate(ref TopdownCameraState state)
    {
        base.Activate(ref state);

        ModernCameraState.IsMouseLocked = true;
        ModernCameraState.IsFirstPerson = true;
        ModernCameraState.CurrentBehaviourType = BehaviourType;
        state.PitchPercent = 0.5f;
        TargetZoom = 0;
    }

    internal override void Deactivate()
    {
        base.Deactivate();

        ModernCameraState.IsMouseLocked = false;
        ModernCameraState.IsFirstPerson = false;
    }

    internal override bool ShouldActivate(ref TopdownCameraState state)
    {
        return Settings.FirstPersonEnabled && ModernCameraState.CurrentBehaviourType != BehaviourType && state.Target.Zoom < Settings.MinZoom;
    }

    internal override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        state.LastTarget.NormalizedLookAtOffset.z = Settings.FirstPersonForwardOffset;
        state.LastTarget.NormalizedLookAtOffset.y = Settings.HeadHeightOffset;
    }
}

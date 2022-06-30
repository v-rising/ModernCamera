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
        state.PitchPercent = 0.51f;
        TargetZoom = 0;
    }

    internal override void Deactivate()
    {
        base.Deactivate();

        if (!ModernCameraState.IsActionMode)
            ModernCameraState.IsMouseLocked = false;
        ModernCameraState.IsFirstPerson = false;
    }

    internal override bool ShouldActivate(ref TopdownCameraState state)
    {
        return Settings.FirstPersonEnabled && ModernCameraState.CurrentBehaviourType != BehaviourType && TargetZoom < Settings.MinZoom;
    }

    internal override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        var forwardOffset = Settings.FirstPersonForwardOffset;
        var headHeight = Settings.HeadHeightOffset;

        if (Settings.FirstPersonShapeshiftOffsets.ContainsKey(ModernCameraState.ShapeshiftName))
        {
            forwardOffset = Settings.FirstPersonShapeshiftOffsets[ModernCameraState.ShapeshiftName].y;
            headHeight = Settings.FirstPersonShapeshiftOffsets[ModernCameraState.ShapeshiftName].x;
        }

        state.LastTarget.NormalizedLookAtOffset.z = forwardOffset;
        state.LastTarget.NormalizedLookAtOffset.y = ModernCameraState.IsMounted ? headHeight + Settings.MountedOffset : headHeight;
    }
}

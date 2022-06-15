using ModernCamera.Enums;
using ProjectM;
using UnityEngine;

namespace ModernCamera.Behaviours;

internal class ThirdPersonCameraBehaviour : CameraBehaviour
{
    internal ThirdPersonCameraBehaviour()
    {
        BehaviourType = BehaviourType.ThirdPerson;
        DefaultMaxPitch = Settings.MaxPitch;
        DefaultMinPitch = Settings.MinPitch;
    }

    internal override void Activate(ref TopdownCameraState state)
    {
        base.Activate(ref state);

        if (ModernCameraState.CurrentBehaviourType == BehaviourType)
            TargetZoom = Settings.MaxZoom / 2;
        else
            TargetZoom = Settings.MinZoom;

        ModernCameraState.CurrentBehaviourType = BehaviourType;
        state.PitchPercent = 0.5f;
    }

    internal override bool ShouldActivate(ref TopdownCameraState state)
    {
        return ModernCameraState.CurrentBehaviourType != BehaviourType && state.Target.Zoom > state.ZoomSettings.MinZoom;
    }

    internal override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        state.LastTarget.NormalizedLookAtOffset.y = Settings.HeadHeightOffset;
        if (Settings.OverTheShoulder && !ProbablyShapeshiftedOrMounted)
            state.LastTarget.NormalizedLookAtOffset.x = Mathf.Lerp(Settings.ShoulderRightOffset, 0, state.Current.Zoom / state.ZoomSettings.MaxZoom);
    
        if (Settings.LockPitch && (!state.InBuildMode || !Settings.DefaultBuildMode))
        {
            state.ZoomSettings.MaxPitch = Settings.LockPitchAngle;
            state.ZoomSettings.MinPitch = Settings.LockPitchAngle;
            data.BuildModeZoomSettings.MaxPitch = Settings.LockPitchAngle;
            data.BuildModeZoomSettings.MinPitch = Settings.LockPitchAngle;
        }
    }
}

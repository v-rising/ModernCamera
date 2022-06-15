using ModernCamera.Enums;
using Silkworm.API;
using Silkworm.Core.KeyBinding;
using Silkworm.Core.Options;
using System;
using UnityEngine;

namespace ModernCamera;

internal static class Settings
{
    internal static bool InvertY { get => InvertYOption.Value; }
    internal static bool ActionModeCrosshair { get => ActionModeCrosshairOption.Value; }

    internal static bool LockZoom { get => LockCameraZoomOption.Value; }
    internal static float LockZoomDistance { get => LockCameraZoomDistanceOption.Value; }
    internal static float MinZoom { get => MinZoomOption.Value; }
    internal static float MaxZoom { get => MaxZoomOption.Value; }

    internal static bool LockPitch { get => LockCameraPitchOption.Value; }
    internal static float LockPitchAngle { get => LockCameraPitchAngleOption.Value * Mathf.Deg2Rad; }
    internal static float MinPitch { get => MinPitchOption.Value * Mathf.Deg2Rad; }
    internal static float MaxPitch { get => MaxPitchOption.Value * Mathf.Deg2Rad; }

    internal static bool FirstPersonEnabled { get => FirstPersonEnabledOption.Value; }
    internal static bool DefaultBuildMode { get => DefaultBuildModeOption.Value; }
    internal static bool ThirdPersonRoof { get => ThirdPersonRoofOption.Value; }

    internal static bool OverTheShoulder { get => OverTheShoulderOption.Value; }
    internal static float OverTheShoulderX { get => OverTheShoulderXOption.Value; }
    internal static float OverTheShoulderY { get => OverTheShoulderYOption.Value; }

    internal static CameraAimMode CameraAimMode { get => CameraAimModeOption.GetEnumValue<CameraAimMode>(); }

    internal static float FirstPersonForwardOffset = 1.65f;
    internal static float HeadHeightOffset = 0.9f;
    internal static float ShoulderRightOffset = 0.8f;

    private static float ZoomOffset = 2;

    private static ToggleOption InvertYOption;
    private static ToggleOption ActionModeCrosshairOption;

    private static ToggleOption LockCameraZoomOption;
    private static SliderOption LockCameraZoomDistanceOption;
    private static SliderOption MinZoomOption;
    private static SliderOption MaxZoomOption;

    private static ToggleOption LockCameraPitchOption;
    private static SliderOption LockCameraPitchAngleOption;
    private static SliderOption MinPitchOption;
    private static SliderOption MaxPitchOption;

    private static ToggleOption FirstPersonEnabledOption;
    private static ToggleOption DefaultBuildModeOption;
    private static ToggleOption ThirdPersonRoofOption;

    private static ToggleOption OverTheShoulderOption;
    private static SliderOption OverTheShoulderXOption;
    private static SliderOption OverTheShoulderYOption;

    private static DropdownOption CameraAimModeOption;

    private static Keybinding ActionModeKeybind;

    internal static void Init()
    {
        SetupOptions();
        SetupKeybinds();
    }

    private static void SetupOptions()
    {
        var category = OptionsManager.AddCategory("Modern Camera");
        InvertYOption = category.AddToggle("moderncamera.inverty", "Invert Y", false);
        ActionModeCrosshairOption = category.AddToggle("moderncamera.actionmodecrosshair", "Action Mode Crosshair", false);

        MinZoomOption = category.AddSlider("moderncamera.minzoom", "Third Person Min Zoom", 1, 18, 2);
        MaxZoomOption = category.AddSlider("moderncamera.maxzoom", "Third Person Max Zoom", 3, 20, 18);
        LockCameraZoomOption = category.AddToggle("moderncamera.lockzoom", "Lock Third Person Camera Zoom", false);
        LockCameraZoomDistanceOption = category.AddSlider("moderncamera.lockzoomdistance", "Lock Third Person Camera Zoom Distance", 1, 20, 15);

        LockCameraPitchOption = category.AddToggle("moderncamera.lockpitch", "Lock Camera Pitch", false);
        LockCameraPitchAngleOption = category.AddSlider("moderncamera.lockpitchangle", "Lock Camera Pitch Angle", 0, 90, 60);
        MinPitchOption = category.AddSlider("moderncamera.minpitch", "Third Person Min Pitch", 0, 90, 9);
        MaxPitchOption = category.AddSlider("moderncamera.maxpitch", "Third Person Max Pitch", 0, 90, 90);

        FirstPersonEnabledOption = category.AddToggle("moderncamera.firstperson", "Enable First Person", true);
        DefaultBuildModeOption = category.AddToggle("moderncamera.defaultbuildmode", "Default Build Mode Camera", true);
        ThirdPersonRoofOption = category.AddToggle("moderncamera.thirdpersonroof", "Castle Roof in Third Person", false);

        OverTheShoulderOption = category.AddToggle("moderncamera.overtheshoulder", "Use Over the Shoulder Offset", false);
        OverTheShoulderXOption = category.AddSlider("moderncamera.overtheshoulderx", "Over the Shoulder X Offset", 0.5f, 2, 0.8f);
        OverTheShoulderYOption = category.AddSlider("moderncamera.overtheshouldery", "Over the Shoulder Y Offset", 0.9f, 2, 0.9f);

        CameraAimModeOption = category.AddDropdown("moderncamera.aimmode", "Aim Mode", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));

        MinZoomOption.AddListener(value =>
        {
            if (value + ZoomOffset > MaxZoom && value + ZoomOffset < MaxZoomOption.MaxValue)
                MaxZoomOption.SetValue(value + ZoomOffset);
            else if (value + ZoomOffset > MaxZoomOption.MaxValue)
                MinZoomOption.SetValue(MaxZoomOption.MaxValue - ZoomOffset);
        });

        MaxZoomOption.AddListener(value =>
        {
            if (value - ZoomOffset < MinZoom && value - ZoomOffset > MinZoomOption.MinValue)
                MinZoomOption.SetValue(value - ZoomOffset);
            else if (value - ZoomOffset < MinZoomOption.MinValue)
                MaxZoomOption.SetValue(MinZoomOption.MinValue + ZoomOffset);
        });

        MinPitchOption.AddListener(value =>
        {
            if (value > MaxPitchOption.Value && value < MaxPitchOption.MaxValue)
                MaxPitchOption.SetValue(value);
            else if (value > MaxPitchOption.MaxValue)
                MinPitchOption.SetValue(MaxPitchOption.MaxValue);
        });

        MaxPitchOption.AddListener(value =>
        {
            if (value < MinPitchOption.Value && value > MinPitchOption.MinValue)
                MinPitchOption.SetValue(value);
            else if (value < MinPitchOption.MinValue)
                MaxPitchOption.SetValue(MinPitchOption.MinValue);
        });
    }

    private static void SetupKeybinds()
    {
        var category = KeybindingsManager.AddCategory("Modern Camera");
        ActionModeKeybind = category.AddKeyBinding("moderncamera.actionmode", "Action Mode");
        ActionModeKeybind.AddKeyDownListener(() =>
        {
            if (!ModernCameraState.IsFirstPerson)
            {
                ModernCameraState.IsMouseLocked = !ModernCameraState.IsMouseLocked;
                ModernCameraState.IsActionMode = !ModernCameraState.IsActionMode;
            }
        });
    }
}

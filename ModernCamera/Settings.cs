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
    internal static float MinZoom { get => MinZoomOption.Value; }
    internal static float MaxZoom { get => MaxZoomOption.Value; }
    internal static float MinPitch { get => MinPitchOption.Value * Mathf.Deg2Rad; }
    internal static float MaxPitch { get => MaxPitchOption.Value * Mathf.Deg2Rad; }
    internal static bool LockPitch { get => LockCameraPitchOption.Value; }
    internal static float LockPitchAngle { get => LockCameraPitchAngleOption.Value * Mathf.Deg2Rad; }
    internal static bool FirstPersonEnabled { get => FirstPersonEnabledOption.Value; }
    internal static bool DefaultBuildMode { get => DefaultBuildModeOption.Value; }
    internal static bool ThirdPersonRoof { get => ThirdPersonRoofOption.Value; }
    internal static bool OverTheShoulder { get => OverTheShoulderOption.Value; }
    internal static CameraAimMode CameraAimMode { get => CameraAimModeOption.GetEnumValue<CameraAimMode>(); }

    internal static float FirstPersonForwardOffset = 1.65f;
    internal static float HeadHeightOffset = 0.9f;
    internal static float ShoulderRightOffset = 0.8f;

    private static float ZoomOffset = 2;

    private static ToggleOption InvertYOption;
    private static SliderOption MinZoomOption;
    private static SliderOption MaxZoomOption;
    private static SliderOption MinPitchOption;
    private static SliderOption MaxPitchOption;
    private static ToggleOption LockCameraPitchOption;
    private static SliderOption LockCameraPitchAngleOption;
    private static ToggleOption FirstPersonEnabledOption;
    private static ToggleOption DefaultBuildModeOption;
    private static DropdownOption CameraAimModeOption;
    private static ToggleOption ThirdPersonRoofOption;
    private static ToggleOption OverTheShoulderOption;

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
        MinZoomOption = category.AddSlider("moderncamera.minzoom", "Third Person Min Zoom", 1, 18, 2);
        MaxZoomOption = category.AddSlider("moderncamera.maxzoom", "Third Person Max Zoom", 3, 20, 18);
        MinPitchOption = category.AddSlider("moderncamera.minpitch", "Third Person Min Pitch", 0, 90, 9);
        MaxPitchOption = category.AddSlider("moderncamera.maxpitch", "Third Person Max Pitch", 0, 90, 90);
        LockCameraPitchOption = category.AddToggle("moderncamera.lockpitch", "Lock Camera Pitch", false);
        LockCameraPitchAngleOption = category.AddSlider("moderncamera.lockpitchangle", "Lock Camera Pitch Angle", 0, 90, 60);
        FirstPersonEnabledOption = category.AddToggle("moderncamera.firstperson", "Enable First Person", true);
        DefaultBuildModeOption = category.AddToggle("moderncamera.defaultbuildmode", "Default Build Mode Camera", true);
        ThirdPersonRoofOption = category.AddToggle("moderncamera.thirdpersonroot", "Castle Roof in Third Person", false);
        OverTheShoulderOption = category.AddToggle("moderncamera.overtheshoulder", "Use Over the Shoulder Offset", false);
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
            ModernCameraState.IsMouseLocked = !ModernCameraState.IsMouseLocked;
            ModernCameraState.IsActionMode = !ModernCameraState.IsActionMode;
        });
    }
}

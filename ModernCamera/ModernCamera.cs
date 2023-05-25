using BepInEx;
using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using Silkworm.Utils;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    private static GameObject CrosshairPrefab;
    private static GameObject Crosshair;
    private static CanvasScaler CanvasScaler;

    private static ZoomModifierSystem ZoomModifierSystem;
    private static PrefabCollectionSystem PrefabCollectionSystem;
    private static UIDataSystem UIDataSystem;

    private static GameObject hudCanvas;
    private static Camera GameCamera;

    private static bool GameFocused;

    public static void Enabled(bool enabled)
    {
        Settings.Enabled = enabled;
        UpdateEnabled(enabled);
    }

    public static void ActionMode(bool enabled)
    {
        ModernCameraState.IsMouseLocked = enabled;
        ModernCameraState.IsActionMode = enabled;
    }

    private static void UpdateEnabled(bool enabled)
    {
        if (ZoomModifierSystem != null)
            ZoomModifierSystem.Enabled = !enabled;

        if (Crosshair != null)
            Crosshair.active = enabled && Settings.AlwaysShowCrosshair && !ModernCameraState.InBuildMode;

        if (!enabled)
        {
            Cursor.visible = true;
            ActionMode(false);
        }
    }

    private static void UpdateFieldOfView(float fov)
    {
        if (GameCamera != null) GameCamera.fieldOfView = fov;
    }

    private static void ToggleUI()
    {
        if (hudCanvas != null) hudCanvas.SetActive(!hudCanvas.activeInHierarchy);
    }

    private void Awake()
    {
        ModernCameraState.RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        ModernCameraState.RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
        ModernCameraState.CurrentBehaviourType = BehaviourType.ThirdPerson;

        Settings.AddEnabledListener(UpdateEnabled);
        Settings.AddFieldOfViewListener(UpdateFieldOfView);
        Settings.AddHideUIListener(ToggleUI);
    }

    private void Update()
    {
        if (!GameFocused || !Settings.Enabled) return;

        if (CrosshairPrefab == null)
            BuildCrosshair();

        if (WorldUtils.ClientWorldExists)
        {
            if (GameCamera == null)
            {
                var cameraObject = GameObject.Find("Main_GameToolCamera(Clone)");
                if (cameraObject != null)
                    GameCamera = cameraObject.GetComponent<Camera>();
            }

            if (hudCanvas == null)
                hudCanvas = GameObject.Find("HUDCanvas(Clone)");

            GatherSystems();
            UpdateSystems();
            UpdateCrosshair();
        }
        else
        {
            Cursor.visible = true;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        GameFocused = hasFocus;
    }

    private void BuildCrosshair()
    {
        try
        {
            if (CursorController._CursorDatas == null) return;
            var cursorData = CursorController._CursorDatas.First(x => x.CursorType == CursorType.Game_Normal);
            if (cursorData == null) return;

            CrosshairPrefab = new GameObject("Crosshair");
            CrosshairPrefab.active = false;
            CrosshairPrefab.AddComponent<CanvasRenderer>();
            var rectTransform = CrosshairPrefab.AddComponent<RectTransform>();
            rectTransform.transform.SetSiblingIndex(1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            rectTransform.localPosition = new Vector3(0, 0, 0);
            var image = CrosshairPrefab.AddComponent<Image>();
            image.sprite = Sprite.Create(cursorData.Texture, new Rect(0, 0, cursorData.Texture.width, cursorData.Texture.height), new Vector2(0.5f, 0.5f), 100f);
            CrosshairPrefab.active = false;
        }
        catch (Exception ex)
        {
            LogUtils.LogDebugError(ex);
        }
    }

    private void GatherSystems()
    {
        if (ZoomModifierSystem == null)
        {
            ZoomModifierSystem = WorldUtils.ClientWorld.GetExistingSystem<ZoomModifierSystem>();

            if (ZoomModifierSystem != null)
                ZoomModifierSystem.Enabled = false;
        }

        if (PrefabCollectionSystem == null)
        {
            PrefabCollectionSystem = WorldUtils.ClientWorld.GetExistingSystem<PrefabCollectionSystem>();
        }

        if (UIDataSystem == null)
        {
            UIDataSystem = WorldUtils.ClientWorld.GetExistingSystem<UIDataSystem>();
        }
    }

    private void UpdateSystems()
    {
        if (UIDataSystem == null || PrefabCollectionSystem == null) return;

        try
        {
            if (UIDataSystem.UI.BuffBarParent != null)
            {
                ModernCameraState.IsShapeshifted = false;
                ModernCameraState.ShapeshiftName = "";
                foreach (var buff in UIDataSystem.UI.BuffBarParent.BuffsSelectionGroup.Entries)
                {
                    if (PrefabCollectionSystem.PrefabGuidToNameDictionary.ContainsKey(buff.PrefabGUID))
                    {
                        ModernCameraState.IsShapeshifted = PrefabCollectionSystem.PrefabGuidToNameDictionary[buff.PrefabGUID].ToString().Contains("shapeshift", System.StringComparison.OrdinalIgnoreCase);
                        if (ModernCameraState.IsShapeshifted)
                        {
                            ModernCameraState.ShapeshiftName = PrefabCollectionSystem.PrefabGuidToNameDictionary[buff.PrefabGUID].ToString().Trim();
                            break;
                        }
                    }
                }
            }

            if (UIDataSystem.UI.AbilityBar != null)
            {
                ModernCameraState.IsMounted = false;
                foreach (var ability in UIDataSystem.UI.AbilityBar.Entries)
                {
                    if (PrefabCollectionSystem.PrefabGuidToNameDictionary.ContainsKey(ability.AbilityId))
                    {
                        ModernCameraState.IsMounted = PrefabCollectionSystem.PrefabGuidToNameDictionary[ability.AbilityId].ToString().Contains("mounted", System.StringComparison.OrdinalIgnoreCase);
                        if (ModernCameraState.IsMounted)
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogUtils.LogDebugError(ex);
        }
    }

    private void UpdateCrosshair()
    {
        try
        {
            if (Crosshair == null && CrosshairPrefab != null)
            {
                var uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");
                if (uiCanvas != null)
                {
                    CanvasScaler = uiCanvas.GetComponent<CanvasScaler>();
                    Crosshair = Instantiate(CrosshairPrefab, uiCanvas.transform);
                    Crosshair.active = true;
                }
            }

            var cursorVisible = true;
            var crosshairVisible = false;
            // Locks the mouse to center of screen if mouse should be locked or camera rotate button is pressed
            if ((ModernCameraState.IsMouseLocked || ModernCameraState.GameplayInputState.IsInputPressed(InputFlag.RotateCamera)) && !ModernCameraState.IsMenuOpen)
            {
                if (ModernCameraState.IsActionMode || ModernCameraState.IsFirstPerson || Settings.CameraAimMode == CameraAimMode.Forward)
                {
                    Mouse.SetCursorPosition((Screen.width / 2) + Settings.AimOffsetX, (Screen.height / 2) - Settings.AimOffsetY);
                }

                crosshairVisible = ModernCameraState.IsFirstPerson || (ModernCameraState.IsActionMode && Settings.ActionModeCrosshair);
                cursorVisible = false;
            }

            if (Crosshair != null)
            {
                Crosshair.active = (crosshairVisible || Settings.AlwaysShowCrosshair) && !ModernCameraState.InBuildMode;

                if (ModernCameraState.IsFirstPerson)
                {
                    Crosshair.transform.localPosition = Vector3.zero;
                }
                else
                {
                    if (CanvasScaler != null)
                    {
                        Crosshair.transform.localPosition = new Vector3(
                            Settings.AimOffsetX * (CanvasScaler.referenceResolution.x / Screen.width),
                            Settings.AimOffsetY * (CanvasScaler.referenceResolution.y / Screen.height),
                            0
                        );
                    }
                }
            }

            Cursor.visible = cursorVisible;
        }
        catch (Exception ex)
        {
            LogUtils.LogDebugError(ex);
        }
    }
}

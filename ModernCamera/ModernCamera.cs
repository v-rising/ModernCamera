using BepInEx;
using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using Silkworm.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    private static GameObject Crosshair;
    private static CanvasScaler CanvasScaler;

    private static ZoomModifierSystem ZoomModifierSystem;
    private static bool ZoomModifierSystemSet;

    private static PrefabCollectionSystem PrefabCollectionSystem;
    private static bool PrefabCollectionSystemSet;

    private static UIDataSystem UIDataSystem;
    private static bool UIDataSystemSet;

    private static bool CrosshairCreated;
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
        if (ZoomModifierSystemSet)
            ZoomModifierSystem.Enabled = !enabled;

        if (CrosshairCreated)
            Crosshair.active = enabled && Settings.AlwaysShowCrosshair && !ModernCameraState.InBuildMode;
    }

    private void Awake()
    {
        ModernCameraState.RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        ModernCameraState.RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
        ModernCameraState.CurrentBehaviourType = BehaviourType.ThirdPerson;

        Settings.AddEnabledListener(UpdateEnabled);
    }

    private void Update()
    {
        if (!GameFocused || !Settings.Enabled) return;

        if (WorldUtils.ClientWorldExists)
        {
            if (!ZoomModifierSystemSet)
            {
                ZoomModifierSystem = WorldUtils.ClientWorld.GetExistingSystem<ZoomModifierSystem>();
                ZoomModifierSystemSet = ZoomModifierSystem != null;

                if (ZoomModifierSystemSet)
                    ZoomModifierSystem.Enabled = false;
            }

            if (!PrefabCollectionSystemSet)
            {
                PrefabCollectionSystem = WorldUtils.ClientWorld.GetExistingSystem<PrefabCollectionSystem>();
                PrefabCollectionSystemSet = PrefabCollectionSystem != null;
            }

            if (!UIDataSystemSet)
            {
                UIDataSystem = WorldUtils.ClientWorld.GetExistingSystem<UIDataSystem>();
                UIDataSystemSet = UIDataSystem != null;
            }

            if (UIDataSystemSet)
            {
                if (UIDataSystem.UI != null && UIDataSystem.UI.BuffBarParent != null)
                {
                    ModernCameraState.IsShapeshifted = false;
                    ModernCameraState.ShapeshiftName = "";
                    foreach (var buff in UIDataSystem.UI.BuffBarParent.BuffsSelectionGroup.Entries)
                    {
                        if (PrefabCollectionSystem.PrefabNameLookupMap.ContainsKey(buff.PrefabGUID))
                        {
                            ModernCameraState.IsShapeshifted = PrefabCollectionSystem.PrefabNameLookupMap[buff.PrefabGUID].ToString().Contains("shapeshift", System.StringComparison.OrdinalIgnoreCase);
                            if (ModernCameraState.IsShapeshifted)
                            {
                                ModernCameraState.ShapeshiftName = PrefabCollectionSystem.PrefabNameLookupMap[buff.PrefabGUID].ToString().Trim();
                                break;
                            }
                        }
                    }
                }

                if (UIDataSystem.UI != null && UIDataSystem.UI.AbilityBar != null)
                {
                    ModernCameraState.IsMounted = false;
                    foreach (var ability in UIDataSystem.UI.AbilityBar.Entries)
                    {
                        if (PrefabCollectionSystem.PrefabNameLookupMap.ContainsKey(ability.AbilityId))
                        {
                            ModernCameraState.IsMounted = PrefabCollectionSystem.PrefabNameLookupMap[ability.AbilityId].ToString().Contains("mounted", System.StringComparison.OrdinalIgnoreCase);
                            if (ModernCameraState.IsMounted)
                                break;
                        }
                    }
                }
            }
        }

        if (!CrosshairCreated)
        {
            var uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");
            if (uiCanvas != null)
            {
                CanvasScaler = uiCanvas.GetComponent<CanvasScaler>();
                Crosshair = BuildCrosshair(uiCanvas.transform);
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

        if (CrosshairCreated)
        {
            Crosshair.active = (crosshairVisible || Settings.AlwaysShowCrosshair) && !ModernCameraState.InBuildMode;

            if (ModernCameraState.IsFirstPerson)
                Crosshair.transform.localPosition = Vector3.zero;
            else
                UpdateCrosshairPosition();
        }

        Cursor.visible = cursorVisible;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        GameFocused = hasFocus;
    }

    private GameObject BuildCrosshair(Transform transform)
    {
        var cursorData = CursorController._CursorDatas.First(x => x.CursorType == CursorType.Game_Normal);
        var gameObject = new GameObject("Crosshair");
        gameObject.active = false;
        gameObject.AddComponent<CanvasRenderer>();
        var rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.transform.SetSiblingIndex(1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(32, 32);
        rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        rectTransform.localPosition = new Vector3(0, 0, 0);
        var image = gameObject.AddComponent<Image>();
        image.overrideSprite = Sprite.Create(cursorData.Texture, new Rect(0, 0, cursorData.Texture.width, cursorData.Texture.height), new Vector2(0.5f, 0.5f), 100f);

        CrosshairCreated = true;

        return gameObject;
    }

    private void UpdateCrosshairPosition()
    {
        var widthScale = CanvasScaler.referenceResolution.x / Screen.width;
        var heightScale = CanvasScaler.referenceResolution.y / Screen.height;
        Crosshair.transform.localPosition = new Vector3(Settings.AimOffsetX * widthScale, Settings.AimOffsetY * heightScale, 0);
    }
}
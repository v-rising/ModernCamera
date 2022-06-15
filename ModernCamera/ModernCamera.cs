using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    private GameObject Crosshair;
    private bool GameFocused;

    private void Awake()
    {
        ModernCameraState.RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        ModernCameraState.RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
        ModernCameraState.CurrentBehaviourType = BehaviourType.ThirdPerson;
        ModernCameraState.Gamehandle = Window.GetWindow("VRising");
    }

    private void Update()
    {
        if (!GameFocused) return;

        if (Crosshair == null)
        {
            var uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");
            if (uiCanvas != null)
                Crosshair = BuildCrosshair(uiCanvas.transform);
        }

        var cursorVisible = true;
        var crosshairVisible = false;
        // Locks the mouse to center of screen if mouse should be locked or camera rotate button is pressed
        if ((ModernCameraState.IsMouseLocked || ModernCameraState.GameplayInputState.IsInputPressed(InputFlag.RotateCamera)) && !ModernCameraState.IsMenuOpen)
        {
            if (ModernCameraState.IsActionMode || ModernCameraState.IsFirstPerson || Settings.CameraAimMode == CameraAimMode.Forward)
                Mouse.CenterCursorPosition();

            crosshairVisible = ModernCameraState.IsFirstPerson || (ModernCameraState.IsActionMode && Settings.ActionModeCrosshair);
            cursorVisible = false;
        }

        if (Crosshair != null)
            Crosshair.active = crosshairVisible;
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
        rectTransform.parent = transform;
        rectTransform.transform.SetSiblingIndex(1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(32, 32);
        rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        rectTransform.localPosition = new Vector3(0, 0, 0);
        var image = gameObject.AddComponent<Image>();
        image.overrideSprite = Sprite.Create(cursorData.Texture, new Rect(0, 0, cursorData.Texture.width, cursorData.Texture.height), new Vector2(0.5f, 0.5f), 100f);
        return gameObject;
    }
}
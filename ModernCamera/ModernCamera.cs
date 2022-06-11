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
    private GameObject crosshair;
    private bool gameFocused;

    private void Awake()
    {
        ModernCameraState.RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        ModernCameraState.RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
        ModernCameraState.currentBehaviourType = BehaviourType.ThirdPerson;
        ModernCameraState.gamehandle = Window.GetWindow("VRising");
    }

    private void Update()
    {
        if (!gameFocused) return;

        if (crosshair == null)
        {
            var uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");
            if (uiCanvas != null)
                crosshair = BuildCrosshair(uiCanvas.transform);
        }


        // Toggles camera rotate lock if Toggle mode is set in settings
        if (!ModernCameraState.isMenuOpen && Settings.cameraRotateMode == CameraRotateMode.Toggle && !ModernCameraState.isFirstPerson && ModernCameraState.gameplayInputState.IsInputDown(InputFlag.RotateCamera))
            ModernCameraState.isMouseLocked = !ModernCameraState.isMouseLocked;

        var cursorVisible = true;
        var crosshairVisible = false;
        // Locks the mouse to center of screen if mouse should be locked or camera rotate button is pressed
        if ((ModernCameraState.isMouseLocked || ModernCameraState.gameplayInputState.IsInputPressed(InputFlag.RotateCamera)) && !ModernCameraState.isMenuOpen)
        {
            if (ModernCameraState.isFirstPerson || Settings.cameraAimMode == CameraAimMode.Forward)
                Mouse.CenterCursorPosition();

            crosshairVisible = ModernCameraState.isFirstPerson;
            cursorVisible = false;
        }

        if (crosshair != null)
            crosshair.active = crosshairVisible;
        Cursor.visible = cursorVisible;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        gameFocused = hasFocus;
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
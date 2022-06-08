using HarmonyLib;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using UnityEngine;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    private readonly Harmony _harmony = new Harmony("Travanti.ModernCamera");

    public static void SetRMouse(bool down)
    {
        if (down)
        {
            if (!ModernCameraState.isMouseDown)
            {
                UpdateCursorPosition();
                Mouse.Click(MouseEvent.RightDown, Mouse.GetCursorPosition());
                ModernCameraState.isMouseDown = true;
            }
        }
        else
        {
            if (ModernCameraState.isMouseDown)
            {
                UpdateCursorPosition();
                Mouse.Click(MouseEvent.RightUp, Mouse.GetCursorPosition());
                ModernCameraState.isMouseDown = false;
            }
        }
    }

    public static void UpdateCursorPosition()
    {
        var rect = Window.GetWindowRect(ModernCameraState.gamehandle);
        var a = new CursorController.CursorPosition
        {
            X = rect.Left + Screen.width / 2,
            Y = rect.Top + Screen.height / 2 - (int)(Screen.height * 0.100000001490116)
        };
        CursorController.SetCursorPos(a.X, a.Y);
        Mouse.SetCursorPosition(a.X, a.Y);
    }

    public void Awake()
    {
        return;
        ModernCameraState.isMenuOpen = true;
        Cursor.visible = true;

        ModernCameraState.gamehandle = Window.GetWindow("VRising");
    }

    public void Update()
    {
        return;
        if (!ModernCameraState.isInitialized) return;

        if (ModernCameraState.isFirstPerson)
        {
            if (Input.GetMouseButtonUp(1))
            {
                ModernCameraState.isMouseDown = false;
                ModernCameraState.isMouseLocked = false;
                return;

            }
            if (Input.GetMouseButtonDown(1))
            {
                ModernCameraState.isMouseDown = true;
                ModernCameraState.isMouseLocked = true;
                return;

            }

            if (ModernCameraState.isMenuOpen)
            {
                ModernCameraState.isMouseLocked = false;
                CursorController.Set(CursorType.Menu_Normal);
                SetRMouse(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) && Event.current.type == EventType.keyDown)
            {
                ModernCameraState.isMenuOpen = true;
                ModernCameraState.isPopupOpen = false;
                ModernCameraState.isMouseLocked = false;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt) ||
                 Input.GetKeyDown(KeyCode.Escape)) && Event.current.type == EventType.keyDown)
            {
                CursorController.Set(CursorType.Game_Normal);
                ModernCameraState.isMouseLocked = false;
                ModernCameraState.isPopupOpen = true;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftAlt)) &&
                Event.current.type == EventType.keyUp)
            {
                if (ModernCameraState.isPopupOpen) ModernCameraState.isPopupOpen = false;
                if (!ModernCameraState.isMouseDown) SetRMouse(true);
                return;
            }

            if (ModernCameraState.isMenuOpen)
            {
                if (ModernCameraState.isMouseLocked)
                {
                    Plugin.Logger.LogError("Setting unlock");
                    ModernCameraState.isMouseLocked = false;
                    CursorController.Set(CursorType.Menu_Normal);
                    SetRMouse(false);
                    return;
                }

                return;
            }

            if (ModernCameraState.isPopupOpen)
            {
                if (ModernCameraState.isMouseLocked)
                {
                    Plugin.Logger.LogError("Setting unlock");
                    ModernCameraState.isMouseLocked = false;
                    CursorController.Set(CursorType.Game_Normal);
                    SetRMouse(false);
                    return;
                }

                return;
            }

            if (!ModernCameraState.isMouseLocked)
            {
                SetRMouse(true);
                CursorController.Set(CursorType.None_LockedToCenter);
                UpdateCursorPosition();
                ModernCameraState.isMouseLocked = true;
                ModernCameraState.isMenuOpen = false;
                ModernCameraState.isPopupOpen = false;
            }

            return;
        }

        if (ModernCameraState.isMouseDown) SetRMouse(false);

        if (ModernCameraState.isMenuOpen)
        {
            ModernCameraState.isMouseLocked = false;
            CursorController.Set(CursorType.Menu_Normal);

            SetRMouse(false);
            return;
        }

        ModernCameraState.isMouseLocked = false;
        CursorController.Set(CursorType.Game_Normal);
        SetRMouse(false);
    }
}
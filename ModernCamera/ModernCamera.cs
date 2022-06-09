using HarmonyLib;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using UnityEngine;
using UnhollowerRuntimeLib;
using Unity.Entities;
using static ModernCamera.ModernCameraState;
using System;

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
      
        ModernCameraState.isMenuOpen = true;
        Cursor.visible = true;

        ModernCameraState.gamehandle = Window.GetWindow("VRising");
    }

    public void createCrosshair()
    {
        var txt = Texture2D.blackTexture;
        foreach (var i in Resources.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<Texture2D>()))
        {
            if (i.name == "Cursor_Crosshair")
            {
                txt = i.TryCast<Texture2D>();
            }
        }


        var HC         = GameObject.Find("HUDCanvas(Clone)/Canvas");
        ModernCameraState.cross_hair                  = new GameObject("Crosshair");
        ModernCameraState.cross_hair.transform.parent = HC.transform;
        ModernCameraState.cross_hair.transform.SetSiblingIndex(1);
        ModernCameraState.cross_hair.AddComponent<CanvasRenderer>();
        var cross_hairRectTransform = ModernCameraState.cross_hair.AddComponent<RectTransform>();
        cross_hairRectTransform.pivot         = new Vector2(0,  0);
        cross_hairRectTransform.anchorMin     = new Vector2(0,  0);
        cross_hairRectTransform.anchorMax     = new Vector2(0,  0);
        cross_hairRectTransform.sizeDelta     = new Vector2(32, 32);
        cross_hairRectTransform.localScale    = new Vector3(1, 1, 1);
        cross_hairRectTransform.localPosition = new Vector3(0, 0, 0);
        var c = ModernCameraState.cross_hair.AddComponent<UnityEngine.UI.Image>();
        c.overrideSprite = Sprite.Create(txt, new Rect(0, 0, 32, 32), new Vector2(0, 0), 32);
        Plugin.Logger.LogError("cURSORcREATED!!");
        isCrosshairShown = true;
    }

    public void Update()
    {
        if (!isInitialized) return;

        if (!hasWorld)
        {
            foreach (var a in World.All)
            {
                if (a.Name == "Client_0")
                {
                    hasWorld = true;
                    clientWorld = a;
                    continue;

                }
            }
            if (!hasWorld)
            {
                throw new Exception("cant get world");
            }
            return;
        }

        if (cross_hair == null)
        {
            createCrosshair();
        }
        if (isFirstPerson)
        {
            if (!isCrosshairShown)
            {
                cross_hair.active = true;
            }
            // Plugin.Logger.LogError($"{isMouseDown}");
            if (Input.GetMouseButtonUp(1) && isMouseDownExternal)
            {
                Plugin.Logger.LogError($"-----------------MOUSE UP FROM EXTERNAL----------------");
                isMouseDown = false;
                isMouseDownExternal = false;
                isMouseLocked = false;
                return;

            }
            if (Input.GetMouseButtonDown(1) && !isMouseDownExternal)
            {
                Plugin.Logger.LogError($"-----------------MOUSE DOWN FROM EXTERNAL----------------");
                isMouseDown = true;
                isMouseDownExternal = true;
                isMouseLocked = true;
                return;

            }

            if (Input.GetKeyDown(KeyCode.Escape) && Event.current.type == EventType.keyDown)
            {
                Plugin.Logger.LogError("Setting inmenu");
                isMenuOpen = true;
                isPopupOpen = false;
                isMouseLocked = false;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt) ||
                 Input.GetKeyDown(KeyCode.Escape)) && Event.current.type == EventType.keyDown)
            {
                CursorController.Set(CursorType.Game_Normal);
                Plugin.Logger.LogError("Setting inpopup");
                isMouseLocked = false;
                isPopupOpen = true;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftAlt)) &&
                Event.current.type == EventType.keyUp)
            {
                Plugin.Logger.LogError("Setting NOT inpopup");
                if (isPopupOpen) isPopupOpen = false;
                if (!isMouseDown) SetRMouse(true);
                return;

            }
            if (isMenuOpen)
            {
                if (isCrosshairShown)
                {
                    cross_hair.active = false;
                }
                if (isMouseLocked)
                {
                    Plugin.Logger.LogError("Setting unlock");
                    isMouseLocked = false;
                    CursorController.Set(CursorType.Menu_Normal);
                    SetRMouse(false);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (isPopupOpen)
            {
                if (isCrosshairShown)
                {
                    cross_hair.active = false;
                }
                if (isMouseLocked)
                {
                    Plugin.Logger.LogError("Setting unlock");
                    isMouseLocked = false;
                    CursorController.Set(CursorType.Game_Normal);
                    SetRMouse(false);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (!isMouseLocked)
            {
                Plugin.Logger.LogError("Setting lock");
                UpdateCursorPosition();
                SetRMouse(true);
                CursorController.Set(CursorType.None_LockedToCenter);
                UpdateCursorPosition();
                isMouseLocked = true;
                isMenuOpen = false;
                isPopupOpen = false;
                if (!isCrosshairShown)
                {
                    cross_hair.active = true;
                }
            }

            return;
        }
        if (isCrosshairShown)
        {
            cross_hair.active = false;
        }
        if (isMouseLocked)
        {
            isMouseLocked = false;
            CursorController.Set(CursorType.Game_Normal);
            SetRMouse(false);
        }


        if (isMouseDown) SetRMouse(false);

        if (isMenuOpen)
        {
            isMouseLocked = false;
            CursorController.Set(CursorType.Menu_Normal);
            SetRMouse(false);

            return;
        }


        // SetRMouse(false);
    }

}
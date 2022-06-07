using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BepInEx.IL2CPP.Hook;
using HarmonyLib;
using ProjectM;
using ProjectM.UI;
using TMPro;
using UnityEngine;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown   = 0x00000002,
        LeftUp     = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp   = 0x00000040,
        Move       = 0x00000001,
        Absolute   = 0x00008000,
        RightDown  = 0x00000008,
        RightUp    = 0x00000010
    }

    private static   IntPtr  _gameHandle;
    private static   bool    _isInFirst;
    private static   bool    _isLocked;
    private static   bool    _isMenuOpen = true;
    private static   bool    _isInPopup;
    private static   bool    _mouseIsDown;
    private static   bool    _hasInit;
    private readonly Harmony _harmony = new("Travanti.ModernCamera");

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out MousePoint lpMousePoint);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);


    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


    public static void SetRMouse(bool down)
    {
        if (down)
        {
            if (!_mouseIsDown)
            {
                UpdateCursorPosition();
                MouseEvent(MouseEventFlags.RightDown);
                _mouseIsDown = true;
            }
        }
        else
        {
            if (_mouseIsDown)
            {
                UpdateCursorPosition();
                MouseEvent(MouseEventFlags.RightUp);
                _mouseIsDown = false;
            }
        }
    }

    public static MousePoint GetCursorPosition()
    {
        var gotPoint                     = GetCursorPos(out var currentMousePoint);
        if (!gotPoint) currentMousePoint = new MousePoint(0, 0);

        return currentMousePoint;
    }

    public static void UpdateCursorPosition()
    {
        var rectangle = new Rect();
        GetWindowRect(_gameHandle, ref rectangle);
        var a = new CursorController.CursorPosition
                {
                    X = (int)rectangle.left                    + Screen.width / 2,
                    Y = (int)rectangle.top + Screen.height / 2 - (int)(Screen.height * 0.100000001490116)
                };
        CursorController.SetCursorPos(a.X, a.Y);
        SetCursorPos(a.X, a.Y);
    }

    public static void UpdateCursorVisible(bool vis)
    {
    }

    public static void MouseEvent(MouseEventFlags value)
    {
        var position = GetCursorPosition();

        mouse_event
            ((int)value,
                position.X,
                position.Y,
                0,
                0)
            ;
    }

    // ReSharper disable once UnusedMember.Global
    public void Awake()
    {
        Cursor.visible = true;
        _harmony.PatchAll();

        TopdownCameraUpdateCameraInputs.ApplyDetour();
        PlayMenuViewP.ApplyDetour();
        PlayContinueMenuViewP.ApplyDetour();
        _gameHandle = FindWindow(null, "VRising");
    }

    // ReSharper disable once UnusedMember.Global
    public void Update()
    {
        if (!_hasInit) return;

        if (_isInFirst)
        {
            if (_isMenuOpen)
            {
                _isLocked = false;
                CursorController.Set(CursorType.Menu_Normal);
                SetRMouse(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) && Event.current.type == EventType.keyDown)
            {
                _isMenuOpen = true;
                _isInPopup  = false;
                _isLocked   = false;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt) ||
                 Input.GetKeyDown(KeyCode.Escape)) && Event.current.type == EventType.keyDown)
            {
                CursorController.Set(CursorType.Game_Normal);
                _isLocked  = false;
                _isInPopup = true;
                SetRMouse(false);
                return;
            }

            if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftAlt)) &&
                Event.current.type == EventType.keyUp)
            {
                if (_isInPopup) _isInPopup = false;
                if (!_mouseIsDown) SetRMouse(true);
                return;

            }

            if (!_isLocked)
            {

                UpdateCursorPosition();
                SetRMouse(true);
                CursorController.Set(CursorType.None_LockedToCenter);
                UpdateCursorPosition();
            }
            SetRMouse(true);
            UpdateCursorPosition();
            return;
        }

        if (_mouseIsDown) SetRMouse(false);

        if (_isMenuOpen)
        {
            _isLocked = false;
            CursorController.Set(CursorType.Menu_Normal);

            SetRMouse(false);
            return;
        }

        _isLocked = false;
        CursorController.Set(CursorType.Game_Normal);
        SetRMouse(false);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static class PlayMenuViewP
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void d_OnButtonClick_Join(IntPtr _this);
        private static d_OnButtonClick_Join OnButtonClick_JoinOriginal;
        public static unsafe void hkOnButtonClick_Join(IntPtr _this)
        {
            var a = new PlayJoinMenuView(_this);
            Plugin.Logger.LogWarning("HOOKED IN");
            Plugin.Logger.LogWarning($"Name: {a._SelectedEntry.Value.Data.Name}");
            Plugin.Logger.LogWarning($"Official: {a._SelectedEntry.Value.Data.IsOfficial}");
            if (a._SelectedEntry.Value.Data.IsOfficial)
            {

                Plugin.Logger.LogWarning("NOT JOINNING IN");


            }
            else
            {
                OnButtonClick_JoinOriginal(_this);
            }

        }
        public static unsafe void ApplyDetour()
        {

            var ty = typeof(PlayJoinMenuView);
            var original = ty.GetMethod("OnButtonClick_Join", AccessTools.all);
            FastNativeDetour.CreateAndApply(Il2CppMethodResolver.ResolveFromMethodInfo(original!), hkOnButtonClick_Join, out OnButtonClick_JoinOriginal);
        }
    }
    public static class PlayContinueMenuViewP
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void d_OnButtonClick_Join1(IntPtr _this);
        private static d_OnButtonClick_Join1 OnButtonClick_JoinOriginal1;
        public static unsafe void hkOnButtonClick_Join(IntPtr _this)
        {
            var a = new PlayContinueMenuView(_this);
            Plugin.Logger.LogWarning("HOOKED IN pcm");
            Plugin.Logger.LogWarning($"Name: {a._SelectedServerEntry.Value.Data.Name}");
            Plugin.Logger.LogWarning($"Official: {a._SelectedServerEntry.Value.Data.IsOfficial}");
            if (a._SelectedServerEntry.Value.Data.IsOfficial)
            {

                Plugin.Logger.LogWarning("NOT JOINNING IN");


            }
            else
            {
                OnButtonClick_JoinOriginal1(_this);
            }

        }
        public static unsafe void ApplyDetour()
        {

            var ty1 = typeof(PlayContinueMenuView);
            var original1 = ty1.GetMethod("OnButtonClick_Join", AccessTools.all);
            FastNativeDetour.CreateAndApply(Il2CppMethodResolver.ResolveFromMethodInfo(original1!), hkOnButtonClick_Join, out OnButtonClick_JoinOriginal1);
        }
    }

    public static class TopdownCameraUpdateCameraInputs
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void DOriginalLambdaBody(
            IntPtr              @this,
            TopdownCameraState* cameraState,
            TopdownCamera*      cameraData);

        private static DOriginalLambdaBody _originalLambdaBodyOriginal;

        public static void PassCamera(
            ref TopdownCameraState cameraState,
            ref TopdownCamera      cameraData)
        {
            cameraState.ZoomSettings.MaxPitch = 1.5f;
            cameraState.ZoomSettings.MinPitch = 0.0f;
            cameraState.ZoomSettings.MaxZoom  = cameraState.ZoomSettings.MinZoom > 0.0 ? 14f : 30f;
            cameraState.ZoomSettings.MinZoom  = -1.0f;
            var flag        = cameraState.Current.Zoom > 0.0;
            var num         = flag ? cameraState.Current.Zoom / cameraState.ZoomSettings.MaxZoom : 0.0f;
            var lookat      = cameraState.Current.LookAtRootPos;
            var lmod        = 0.085f;
            var pc          = cameraState.Current.Pitch / (cameraState.ZoomSettings.MaxPitch / 100);
            if (pc <= 0) pc = 0;


            var yx = Mathf.SmoothStep(lookat.y, lookat.y + lmod, (100 - pc) * 0.01f);
            var yz = Mathf.SmoothStep(1.24f,    1.80f,           (100 - pc) * 0.01f);

            lookat.y = (float)Math.Round(yx, 2);
            if (cameraState.Current.Zoom < 0.8f)
            {
                if (!_hasInit)
                {
                    _hasInit    = true;
                    _isMenuOpen = false;
                }

                _isInFirst                                   = true;
                cameraState.ZoomSettings.MinPitch            = -1.5f;
                cameraState.Current.Zoom                     = -1.0f;
                cameraState.Current.NormalizedLookAtOffset.y = flag ? Mathf.Lerp(1f, 0.0f, num) : 0.0f;
            }
            else
            {
                _isInFirst                                   = false;
                cameraState.Current.NormalizedLookAtOffset.y = 0.0f;
                cameraData.LookAtHeight                      = (float)Math.Round(yz, 2);
            }
        }


        public static unsafe void HkOriginalLambdaBody(
            IntPtr              @this,
            TopdownCameraState* cameraState,
            TopdownCamera*      cameraData)
        {
            PassCamera(ref *cameraState, ref *cameraData);
            cameraData->StandardZoomSettings = cameraState->ZoomSettings;
            _originalLambdaBodyOriginal(@this, cameraState, cameraData);
        }

        public static unsafe void ApplyDetour()
        {
            var ty = typeof(TopdownCameraSystem).GetNestedTypes().First(x => x.Name.Contains("UpdateCameraInputs"));
            var original = ty.GetMethod("OriginalLambdaBody", AccessTools.all);
            FastNativeDetour.CreateAndApply(Il2CppMethodResolver.ResolveFromMethodInfo(original!), HkOriginalLambdaBody,
                out _originalLambdaBodyOriginal);
        }
    }
    [HarmonyPatch]
    public class EscapeMenuView_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscapeMenuView), "OnEnable")]
        public static void OnEnable(EscapeMenuView __instance) => _isMenuOpen = true;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscapeMenuView), "OnDestroy")]
        public static void OnDestroy(EscapeMenuView __instance) => _isMenuOpen = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EscapeMenuView), "OnButtonClick_LeaveGame")]
        public static void OnButtonClick_LeaveGame()
        {
            _isMenuOpen                                            = false;
         
            Cursor.lockState                                       = (CursorLockMode)0;
            Cursor.visible                                         = true;
        }
    }

    [HarmonyPatch]
    public class OpenHUDMenuSystem_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OpenHUDMenuSystem), "OnUpdate")]
        public static void OnUpdate(OpenHUDMenuSystem __instance) => _isMenuOpen = __instance.CurrentMenuType > 0;
    }

    [HarmonyPatch(typeof(MainMenuNewView), "Start")]
    public class Bpacher
    {
        static void Postfix(ProjectM.UI.MainMenuNewView __instance)
        {
            try
            {
                //BetaNotice

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--------------------");
                sb.AppendLine("void ProjectM.UI.MainMenuNewView::Start()");
                sb.Append("- __instance: ").AppendLine(__instance.ToString());
                Plugin.Logger.LogWarning(sb.ToString());

                var a = __instance.GetComponent<Transform>().FindChild("Content").FindChild("BetaNotice");
                a.gameObject.SetActive(true);
                var txt = a.FindChild("Text").GetComponent<TextMeshProUGUI>();
                txt.horizontalAlignment = HorizontalAlignmentOptions.Center;
                var newText = @"<b>Mods Are Active!</b>
<size=24><b>Official</b> servers are <b>DISABLED</b>
<b>Private</b>/<b>Hosted</b>/<b>Local</b>
<b>ENABLED</b>
Discord: https://dev.il.gy</size>";
                var v = new Vector3(0f, 0f, 0f);
                a.set_localPosition_Injected(ref v);
                txt.text = newText;
                Plugin.Logger.LogWarning(a.ToString());
                //Content/NewsPanelParent/NewsPanel/OuterContent/


            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError(ex.ToString());
            }
        }

    }
}
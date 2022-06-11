using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ProjectM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModernCamera;

internal static class ModernCameraState
{
    internal static IntPtr gamehandle;
    internal static bool isFirstPerson;
    internal static bool isMouseLocked;
    internal static InputState gameplayInputState;
    internal static BehaviourType currentBehaviourType = BehaviourType.Default;
    internal static Dictionary<BehaviourType, CameraBehaviour> cameraBehaviours = new();

    internal static bool isMenuOpen
    {
        get { return _menusOpen > 0; }
        set { _menusOpen = value ? _menusOpen + 1 : Mathf.Max(0, _menusOpen - 1); }
    }
    private static int _menusOpen;

    internal static CameraBehaviour currentCameraBehaviour
    {
        get
        {
            if (cameraBehaviours.ContainsKey(currentBehaviourType))
                return cameraBehaviours[currentBehaviourType];
            return null;
        }
    }

    internal static void RegisterCameraBehaviour(CameraBehaviour behaviour)
    {
        cameraBehaviours.Add(behaviour.type, behaviour);
    }
}

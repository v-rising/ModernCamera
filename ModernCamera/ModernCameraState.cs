using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ProjectM;
using System;
using System.Collections.Generic;

namespace ModernCamera;

internal static class ModernCameraState
{
    internal static IntPtr gamehandle;
    internal static bool isMenuOpen;
    internal static bool isFirstPerson;
    internal static bool isMouseLocked;
    internal static BehaviourType currentBehaviourType = BehaviourType.Default;
    internal static Dictionary<BehaviourType, CameraBehaviour> cameraBehaviours = new();
    internal static Dictionary<InputFlag, KeyInputMapping> keyMappings = new();

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

    internal static void UpdateInputSettings(InputSettings inputSettings)
    {
        keyMappings.Clear();
        foreach (var mapping in inputSettings.KeyInputMappings)
            keyMappings.Add(mapping.InputFlag, mapping);
    }
}

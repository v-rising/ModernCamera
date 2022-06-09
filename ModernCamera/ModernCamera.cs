using HarmonyLib;
using ModernCamera.Behaviours;
using ModernCamera.Enums;
using ModernCamera.Utils;
using ProjectM;
using UnityEngine;

namespace ModernCamera;

public class ModernCamera : MonoBehaviour
{
    public void Awake()
    {
        ModernCameraState.RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        ModernCameraState.RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
        ModernCameraState.currentBehaviourType = BehaviourType.ThirdPerson;
        ModernCameraState.gamehandle = Window.GetWindow("VRising");
    }

    public void Update()
    {
        ModernCameraState.keyMappings.TryGetValue(InputFlag.RotateCamera, out var rotateCameraMapping);

        if (ModernCameraState.keyMappings.TryGetValue(InputFlag.ToggleActionWheel, out var actionMapping))
            if (Input.GetKey(actionMapping.PrimaryKeyCode) || Input.GetKey(actionMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = true;
            else if (Input.GetKeyUp(actionMapping.PrimaryKeyCode) || Input.GetKeyUp(actionMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = false;

        if (ModernCameraState.keyMappings.TryGetValue(InputFlag.ToggleEmoteWheel, out var emoteMapping))
            if (Input.GetKey(emoteMapping.PrimaryKeyCode) || Input.GetKey(emoteMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = true;
            else if (Input.GetKeyUp(emoteMapping.PrimaryKeyCode) || Input.GetKeyUp(emoteMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = false;

        if (Settings.cameraRotateMode == CameraRotateMode.Toggle && !ModernCameraState.isFirstPerson &&
            (Input.GetKeyDown(rotateCameraMapping.PrimaryKeyCode) || Input.GetKeyDown(rotateCameraMapping.SecondaryKeyCode)))
            ModernCameraState.isMouseLocked = !ModernCameraState.isMouseLocked;


        var lockState = CursorLockMode.None;

        if ((ModernCameraState.isMouseLocked || Input.GetKey(rotateCameraMapping.PrimaryKeyCode) || Input.GetKey(rotateCameraMapping.SecondaryKeyCode)) && !ModernCameraState.isMenuOpen)
        {
            lockState = CursorLockMode.Locked;

            if (ModernCameraState.isFirstPerson)
                Cursor.visible = true;
        }

        Cursor.lockState = lockState;
    }
}
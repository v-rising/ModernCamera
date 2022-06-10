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

        // Toggles isMenuOpen for the action wheel
        if (ModernCameraState.keyMappings.TryGetValue(InputFlag.ToggleActionWheel, out var actionMapping))
            if (Input.GetKey(actionMapping.PrimaryKeyCode) || Input.GetKey(actionMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = true;
            else if (Input.GetKeyUp(actionMapping.PrimaryKeyCode) || Input.GetKeyUp(actionMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = false;

        // Toggles isMenuOpen for the emote wheel
        if (ModernCameraState.keyMappings.TryGetValue(InputFlag.ToggleEmoteWheel, out var emoteMapping))
            if (Input.GetKey(emoteMapping.PrimaryKeyCode) || Input.GetKey(emoteMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = true;
            else if (Input.GetKeyUp(emoteMapping.PrimaryKeyCode) || Input.GetKeyUp(emoteMapping.SecondaryKeyCode))
                ModernCameraState.isMenuOpen = false;

        // Toggles camera rotate lock if Toggle mode is set in settings
        if (Settings.cameraRotateMode == CameraRotateMode.Toggle && !ModernCameraState.isFirstPerson &&
            (Input.GetKeyDown(rotateCameraMapping.PrimaryKeyCode) || Input.GetKeyDown(rotateCameraMapping.SecondaryKeyCode)))
            ModernCameraState.isMouseLocked = !ModernCameraState.isMouseLocked;


        var lockState = CursorLockMode.None;
        // Locks the mouse to center of screen if mouse should be locked or camera rotate button is pressed
        if ((ModernCameraState.isMouseLocked || Input.GetKey(rotateCameraMapping.PrimaryKeyCode) || Input.GetKey(rotateCameraMapping.SecondaryKeyCode)) && !ModernCameraState.isMenuOpen)
        {
            if (ModernCameraState.isFirstPerson || Settings.aimMode == CameraAimMode.Forward)
                lockState = CursorLockMode.Locked;
            else
                lockState = CursorLockMode.Confined;

            // Forces cursor visible in first person
            if (ModernCameraState.isFirstPerson)
                Cursor.visible = true;
        }

        Cursor.lockState = lockState;
    }
}
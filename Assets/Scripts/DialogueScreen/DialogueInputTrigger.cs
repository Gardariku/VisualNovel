using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

// This script acts as a single point for all other scripts to get
// the current input from. It uses Unity's new Input System and
// functions should be mapped to their corresponding controls
// using a PlayerInput component with Unity Events.

namespace DialogueScreen
{
    public class DialogueInputTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueSystem _dialogueSystem;
        [SerializeField] private PlayerInput _inputSystem;

        private bool interactPressed = false;
        private bool submitPressed = false;
        private bool cancelPressed = false;

        private void OnEnable()
        {
            _inputSystem.SwitchCurrentActionMap($"{UIDirectory.DialogueRoot}");
        }

        public void InteractButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
                interactPressed = true;
            else if (context.canceled)
                interactPressed = false;
        }

        public void SubmitPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                submitPressed = true;
                if (UIRouter.State == UIState.Root)
                    _dialogueSystem.TryContinue();
            }
            else if (context.canceled)
                submitPressed = false;
        }

        public void CancelPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                cancelPressed = true;
                if (UIRouter.State == UIState.Root)
                    UIRouter.SwitchUrl($"{UIDirectory.DialogueRoot}/{UIDirectory.DialogueMenu}");
                else
                    UIRouter.ProceedBack();
            }
            else if (context.canceled)
                cancelPressed = false;
        }

        public bool GetInteractPressed()
        {
            bool result = interactPressed;
            interactPressed = false;
            return result;
        }

        public bool GetSubmitPressed()
        {
            bool result = submitPressed;
            submitPressed = false;
            return result;
        }

        public bool GetCancelPressed()
        {
            bool result = cancelPressed;
            cancelPressed = false;
            return result;
        }

        public void RegisterSubmitPressed()
        {
            submitPressed = false;
        }
    }
}

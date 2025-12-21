using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.System
{
    [CreateAssetMenu(fileName = "Input", menuName = "SO/Input")]
    public class InputSO : ScriptableObject, Controls.IPlayerActions
    {
        Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }

            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 mouseVec = context.ReadValue<Vector2>();
        }
    }
}


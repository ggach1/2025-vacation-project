using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.System
{
    [CreateAssetMenu(fileName = "Input", menuName = "SO/Input")]
    public class InputSO : ScriptableObject, Controls.IPlayerActions
    {
        Controls _controls;

        public event Action LeftClickPressed;
        public event Action RightClickPressed;

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

        #region 마우스 조작

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                LeftClickPressed?.Invoke();
        }

        public void OnShowInformation(InputAction.CallbackContext context)
        {
            if (context.performed)
                RightClickPressed?.Invoke();
        }

        #endregion

        #region 버튼 조작

        public void OnLvUp(InputAction.CallbackContext context)
        {
            throw new global::System.NotImplementedException();
        }

        public void OnReroll(InputAction.CallbackContext context)
        {
            throw new global::System.NotImplementedException();
        }

        public void OnSell(InputAction.CallbackContext context)
        {
            throw new global::System.NotImplementedException();
        }

        #endregion
    }
}


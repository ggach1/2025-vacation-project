using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.System
{
    [CreateAssetMenu(fileName = "Input", menuName = "SO/Input")]
    public class InputSO : ScriptableObject, Controls.IPlayerActions
    {
        public Vector2 MovementKey { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        public Vector2 MousePosition => MouseDelta;
        public bool SprintHeld { get; private set; }

        Controls _controls;
        int _lastPolledFrame = -1;
        bool _jumpPressed;
        bool _interactPressed;
        bool _cursorTogglePressed;

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
            MovementKey = context.ReadValue<Vector2>();
        }

        public void OnMouse(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                MouseDelta = Vector2.zero;
                return;
            }

            MouseDelta += context.ReadValue<Vector2>();
        }

        public void Refresh()
        {
            if (_lastPolledFrame == Time.frameCount)
            {
                return;
            }

            _lastPolledFrame = Time.frameCount;

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                SprintHeld = false;
                return;
            }

            SprintHeld = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            _jumpPressed |= keyboard.spaceKey.wasPressedThisFrame;
            _interactPressed |= keyboard.eKey.wasPressedThisFrame;
            _cursorTogglePressed |= keyboard.escapeKey.wasPressedThisFrame;
        }

        public Vector2 ConsumeMouseDelta()
        {
            Refresh();

            Vector2 delta = MouseDelta;
            MouseDelta = Vector2.zero;
            return delta;
        }

        public bool ConsumeJumpPressed()
        {
            Refresh();

            bool pressed = _jumpPressed;
            _jumpPressed = false;
            return pressed;
        }

        public bool ConsumeInteractPressed()
        {
            Refresh();

            bool pressed = _interactPressed;
            _interactPressed = false;
            return pressed;
        }

        public bool ConsumeCursorTogglePressed()
        {
            Refresh();

            bool pressed = _cursorTogglePressed;
            _cursorTogglePressed = false;
            return pressed;
        }
    }
}

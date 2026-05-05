using UnityEngine;
using UnityEngine.InputSystem;
using Gameplay.Input;

namespace Framework.Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerInputActions _input;

        public InputState State { get; private set; } = new InputState();

        private void Awake()
        {
            _input = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _input.Enable();

            _input.Player.Attack.performed += OnAttack;
            _input.Player.Move.performed   += OnMove;
            _input.Player.Move.canceled    += OnMoveCanceled;
            _input.Camera.Look.performed   += OnLook;
            // NOTE: No canceled handler for Look —
            // Mouse delta is naturally zero when not moving.
            // Zeroing on canceled kills the value before
            // PlayerLook and FPSCameraMode can read it.
        }

        private void OnDisable()
        {
            _input.Player.Attack.performed -= OnAttack;
            _input.Player.Move.performed   -= OnMove;
            _input.Player.Move.canceled    -= OnMoveCanceled;
            _input.Camera.Look.performed   -= OnLook;

            _input.Disable();
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            State.Move = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            State.Move = Vector2.zero;
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            State.AttackPressed = true;
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            State.Look = ctx.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            // Reset one-frame inputs after everything has read them
            State.AttackPressed = false;

            // Reset Look each frame — it is repopulated by OnLook
            // only when the mouse actually moves
            State.Look = Vector2.zero;
        }
    }
}
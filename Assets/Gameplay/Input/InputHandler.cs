using UnityEngine;
using UnityEngine.InputSystem;
using Framework.Input;

namespace Gameplay.Input
{
    // =========================================
    // InputHandler
    // Execution order set to 100 so it runs
    // AFTER CameraModeController (default 0).
    // This ensures Look is not zeroed before
    // camera reads it in LateUpdate.
    // Set in Edit → Project Settings →
    // Script Execution Order if needed.
    // =========================================
    [DefaultExecutionOrder(100)]
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
            _input.Player.Pause.performed  += OnPause;
            _input.Camera.Look.performed   += OnLook;
        }

        private void OnDisable()
        {
            _input.Player.Attack.performed -= OnAttack;
            _input.Player.Move.performed   -= OnMove;
            _input.Player.Move.canceled    -= OnMoveCanceled;
            _input.Player.Pause.performed  -= OnPause;
            _input.Camera.Look.performed   -= OnLook;

            _input.Disable();
        }

        private void OnMove(InputAction.CallbackContext ctx)
            => State.Move = ctx.ReadValue<Vector2>();

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
            => State.Move = Vector2.zero;

        private void OnAttack(InputAction.CallbackContext ctx)
            => State.AttackPressed = true;

        private void OnPause(InputAction.CallbackContext ctx)
            => State.PausePressed = true;

        private void OnLook(InputAction.CallbackContext ctx)
            => State.Look = ctx.ReadValue<Vector2>();

        private void LateUpdate()
        {
            // Runs after all other LateUpdates due to DefaultExecutionOrder(100)
            // Camera has already read Look by the time we zero it
            State.AttackPressed = false;
            State.PausePressed  = false;
            State.Look          = Vector2.zero;
        }
    }
}
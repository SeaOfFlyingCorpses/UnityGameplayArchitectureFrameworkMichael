using UnityEngine;
using UnityEngine.InputSystem;

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
            _input.Player.Move.performed += OnMove;
            _input.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _input.Player.Attack.performed -= OnAttack;
            _input.Player.Move.performed -= OnMove;
            _input.Player.Move.canceled -= OnMoveCanceled;

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

        private void LateUpdate()
        {
            // reset one-frame inputs
            State.AttackPressed = false;
        }
    }
}
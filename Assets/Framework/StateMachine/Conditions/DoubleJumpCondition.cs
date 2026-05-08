using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    // True when jump pressed while airborne
    // and a delegate says extra jump is available
    public class DoubleJumpCondition : ITransitionCondition
    {
        private readonly System.Func<bool> _canDoubleJump;

        public DoubleJumpCondition(
            System.Func<bool> canDoubleJump)
        {
            _canDoubleJump = canDoubleJump;
        }

        public bool Evaluate(StateContext context)
            => context.Input.JumpPressed &&
               !context.IsGrounded &&
               (_canDoubleJump?.Invoke() ?? false);
    }
}

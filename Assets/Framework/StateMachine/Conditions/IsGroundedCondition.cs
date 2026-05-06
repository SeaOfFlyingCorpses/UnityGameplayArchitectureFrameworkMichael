using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    public class IsGroundedCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
            => context.IsGrounded;
    }
}

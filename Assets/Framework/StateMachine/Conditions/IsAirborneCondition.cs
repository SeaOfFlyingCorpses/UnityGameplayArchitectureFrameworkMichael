using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    public class IsAirborneCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
            => !context.IsGrounded;
    }
}

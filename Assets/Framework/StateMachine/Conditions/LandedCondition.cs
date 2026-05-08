using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    // True when agent just landed (IsGrounded)
    // Optional callback on land
    public class LandedCondition : ITransitionCondition
    {
        private readonly System.Action<StateContext> _onLanded;

        public LandedCondition(
            System.Action<StateContext> onLanded = null)
        {
            _onLanded = onLanded;
        }

        public bool Evaluate(StateContext context)
        {
            if (!context.IsGrounded) return false;
            _onLanded?.Invoke(context);
            return true;
        }
    }
}

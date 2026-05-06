using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    public class JumpPressedCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
            => context.Input.JumpPressed;
    }
}

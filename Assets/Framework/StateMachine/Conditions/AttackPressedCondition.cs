namespace Framework.StateMachine.Conditions
{
    public class AttackPressedCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.Input.AttackPressed;
        }
    }
}
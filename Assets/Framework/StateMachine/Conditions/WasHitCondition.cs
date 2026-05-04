namespace Framework.StateMachine.Conditions
{
    public class WasHitCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.WasHit;
        }
    }
}
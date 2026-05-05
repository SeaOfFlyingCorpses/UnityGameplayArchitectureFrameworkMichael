namespace Framework.StateMachine.Conditions
{
    // =========================================
    // TargetLostCondition
    // True when agent had a target but can no
    // longer see it. Triggers transition to
    // ChaseState or SearchState.
    // =========================================
    public class TargetLostCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            // Has a target but cannot see it
            return context.Target != null &&
                   context.Perception != null &&
                   !context.Perception.CanSeeTarget;
        }
    }
}

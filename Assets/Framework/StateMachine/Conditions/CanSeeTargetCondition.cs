namespace Framework.StateMachine.Conditions
{
    // =========================================
    // CanSeeTargetCondition
    // True when the agent currently has at least
    // one visible target in its perception context.
    // Used to transition into CombatState / ChaseState.
    // =========================================
    public class CanSeeTargetCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.Perception != null &&
                   context.Perception.CanSeeTarget;
        }
    }
}

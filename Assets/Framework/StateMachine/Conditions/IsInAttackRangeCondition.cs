namespace Framework.StateMachine.Conditions
{
    // =========================================
    // IsInAttackRangeCondition
    // True when agent's target is within
    // attack range per PerceptionState.
    // Used to transition from ChaseState
    // into CombatState.
    // =========================================
    public class IsInAttackRangeCondition : ITransitionCondition
    {
        public bool Evaluate(StateContext context)
        {
            return context.Perception != null &&
                   context.Perception.IsTargetInAttackRange;
        }
    }
}

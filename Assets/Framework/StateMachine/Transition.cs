using Framework.StateMachine.Conditions;

namespace Framework.StateMachine
{
    public class Transition
    {
        public ITransitionCondition Condition;
        public IState TargetState;

        public Transition(ITransitionCondition condition, IState targetState)
        {
            Condition = condition;
            TargetState = targetState;
        }

        public bool Evaluate(StateContext context)
        {
            return Condition.Evaluate(context);
        }
    }
}
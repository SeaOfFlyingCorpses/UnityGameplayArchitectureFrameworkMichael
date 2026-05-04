namespace Framework.StateMachine.Conditions
{
    public interface ITransitionCondition
    {
        bool Evaluate(StateContext context);
    }
}
using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    // Checks a delegate for dash completion
    // e.g. () => dashState.IsFinished
    public class DashFinishedCondition : ITransitionCondition
    {
        private readonly System.Func<bool> _isFinished;
        private readonly System.Action<StateContext> _onFinished;

        public DashFinishedCondition(
            System.Func<bool>              isFinished,
            System.Action<StateContext>    onFinished = null)
        {
            _isFinished = isFinished;
            _onFinished = onFinished;
        }

        public bool Evaluate(StateContext context)
        {
            if (!_isFinished()) return false;
            _onFinished?.Invoke(context);
            return true;
        }
    }
}

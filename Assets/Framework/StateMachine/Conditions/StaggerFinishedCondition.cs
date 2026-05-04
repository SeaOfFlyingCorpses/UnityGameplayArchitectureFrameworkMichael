using Framework.StateMachine;
using Framework.StateMachine.States;

namespace Framework.StateMachine.Conditions
{
    public class StaggerFinishedCondition : ITransitionCondition
    {
        private readonly StaggerState _state;
        private readonly float _duration;
        public StaggerFinishedCondition(StaggerState state)
        {
            _state = state;
        }

        public bool Evaluate(StateContext context)
        {
            return _state.IsFinished();
        }
    }
}
using Framework.StateMachine;
using Framework.StateMachine.Conditions;

namespace Gameplay.States.Conditions
{
    // Lives in Gameplay — references LandState which is Gameplay
    public class LandFinishedCondition : ITransitionCondition
    {
        private readonly LandState _state;

        public LandFinishedCondition(LandState state)
        {
            _state = state;
        }

        public bool Evaluate(StateContext context)
            => _state.IsFinished();
    }
}

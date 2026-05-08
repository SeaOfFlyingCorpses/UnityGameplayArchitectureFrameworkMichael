using Unity.Profiling;
using Framework.Core;
namespace Framework.StateMachine
{
    public class StateMachine
    {
        private IState       _currentState;
        private StateContext _context;

        public StateMachine(StateContext context)
        {
            _context = context;
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(_context);

            // Write state name for debug overlay
            _context.CurrentStateName =
                _currentState?.GetType().Name ?? "—";
        }

        public void Update()
        {
            if (_currentState == null) return;

            using var marker = FrameworkProfiler.AIStateUpdate.Auto();

            foreach (var transition in
                     _currentState.GetTransitions())
            {
                if (transition.Evaluate(_context))
                {
                    ChangeState(transition.TargetState);
                    return;
                }
            }

            _currentState.Update(_context);
        }
    }
}
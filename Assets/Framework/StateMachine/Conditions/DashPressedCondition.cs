using Framework.StateMachine;

namespace Framework.StateMachine.Conditions
{
    // Requires DashPressed input and no active cooldown
    // Cooldown tracked externally by the caller
    public class DashPressedCondition : ITransitionCondition
    {
        private readonly System.Func<bool> _onCooldown;

        // Pass a delegate to check cooldown
        // e.g. () => dashState.OnCooldown
        public DashPressedCondition(
            System.Func<bool> onCooldown = null)
        {
            _onCooldown = onCooldown;
        }

        public bool Evaluate(StateContext context)
        {
            if (_onCooldown != null && _onCooldown()) return false;
            return context.Input.DashPressed;
        }
    }
}

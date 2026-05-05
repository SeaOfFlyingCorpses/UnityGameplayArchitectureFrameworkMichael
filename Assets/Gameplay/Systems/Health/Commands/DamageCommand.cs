using Framework.Commands;
using Gameplay.Systems.Health;

namespace Gameplay.Systems.Health.Commands
{
    // =========================================
    // DamageCommand
    // Deals damage to any IHealth implementation.
    // Works with Health, ShieldedHealth,
    // RegenHealth, or any future type.
    // =========================================
    public class DamageCommand : ICommand
    {
        private readonly IHealth _health;
        private readonly int     _amount;

        public DamageCommand(IHealth health, int amount)
        {
            _health = health;
            _amount = amount;
        }

        public void Execute()
        {
            _health?.Damage(_amount);
        }
    }
}
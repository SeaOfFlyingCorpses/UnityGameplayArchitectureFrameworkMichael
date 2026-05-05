using Framework.Commands;
using Framework.Systems.Health;

namespace Gameplay.Systems.Health.Commands
{
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
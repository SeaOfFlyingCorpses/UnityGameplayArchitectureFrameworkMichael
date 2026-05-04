using Framework.Commands;
using UnityEngine;
using Gameplay.Systems.Health;

namespace Gameplay.Systems.Health.Commands
{
    public class DamageCommand : ICommand
    {
        private readonly Health _health;
        private readonly int _amount;

        public DamageCommand(Health health, int amount)
        {
            _health = health;
            _amount = amount;
        }

        public void Execute()
        {
            _health.Damage(_amount);
        }
    }
}

using Framework.Systems.Health;
using System;

namespace Gameplay.Systems.Health
{
    public class Health : IHealth
    {
        public int  Value    { get; private set; }
        public int  MaxValue { get; private set; }
        public bool IsDead   => Value <= 0;

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        public Health(int maxValue)
        {
            MaxValue = maxValue;
            Value    = maxValue;
        }

        public void Damage(int amount)
        {
            if (IsDead) return;
            Value = Math.Max(0, Value - amount);
            OnChanged?.Invoke(Value);
            if (IsDead) OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            Value = Math.Min(MaxValue, Value + amount);
            OnChanged?.Invoke(Value);
        }

        public void Reset()
        {
            Value = MaxValue;
            OnChanged?.Invoke(Value);
        }
    }
}
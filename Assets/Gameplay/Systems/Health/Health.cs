using System;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Systems.Health
{
    public class Health
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<int> OnChanged;
        public event Action OnDeath;

        public Health(int maxValue = 100)
        {
            MaxValue = maxValue;
            Value = maxValue;
        }

        public void Set(int value)
        {
            if (IsDead) return;

            Value = Mathf.Clamp(value, 0, MaxValue);

            OnChanged?.Invoke(Value);
            EventBus.Publish(new HealthChangedEvent(Value));

            if (Value <= 0)
                Die();
        }

        public void Damage(int amount)
        {
            Set(Value - amount);
        }

        private void Die()
        {
            if (IsDead) return;

            IsDead = true;

            OnDeath?.Invoke();
        }
    }
}
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    public class HealthComponent : MonoBehaviour
    {
        private Health _health;

        public event Action<Vector3> OnHit;
        public event Action OnDeath;

        [SerializeField] private int maxHealth = 100;

        private void Awake()
        {
            _health = new Health(maxHealth);

            _health.OnChanged += OnHealthChanged;
            _health.OnDeath += HandleDeath;
        }

        private void HandleDeath()
        {
            Debug.Log($"{gameObject.name} DIED");

            OnDeath?.Invoke();

            // simple test death behaviour
            Destroy(gameObject, 1f);
        }

        private void OnHealthChanged(int value)
        {
            // UI / VFX hook
        }

        public Health GetHealth()
        {
            return _health;
        }

        public void Damage(int amount, Vector3 hitPoint)
        {
            _health.Damage(amount);
            OnHit?.Invoke(hitPoint);
        }
    }
}
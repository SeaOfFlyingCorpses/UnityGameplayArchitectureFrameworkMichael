using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // HealthComponent
    // MonoBehaviour wrapper around the pure
    // Health data object.
    //
    // Responsibilities:
    //   - owns the Health instance
    //   - surfaces OnHit and OnDeath events
    //   - routes Damage calls with hit point
    //
    // Does NOT destroy the GameObject on death —
    // that is DeathSystem's responsibility.
    // =========================================
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        private Health _health;

        public event Action<Vector3> OnHit;
        public event Action          OnDeath;

        private void Awake()
        {
            _health = new Health(maxHealth);

            _health.OnChanged += OnHealthChanged;
            _health.OnDeath   += HandleDeath;
        }

        private void HandleDeath()
        {
            Debug.Log($"{gameObject.name} DIED");

            // Notify all listeners (DeathSystem, AIController, UI, etc.)
            OnDeath?.Invoke();

            // Intentionally no Destroy() here —
            // DeathSystem owns the destroy timing
        }

        private void OnHealthChanged(int value)
        {
            // UI / VFX hook — wired externally
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
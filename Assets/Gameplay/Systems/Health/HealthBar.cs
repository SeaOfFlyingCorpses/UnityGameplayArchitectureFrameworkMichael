using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Systems.Health
{
    // =========================================
    // HealthBar
    // Subscribes to Health.OnChanged once and
    // updates the fill only when health actually
    // changes — zero per-frame polling.
    // =========================================
    public class HealthBar : MonoBehaviour
    {
        public HealthComponent health;
        public Image           fill;

        private Health _health;

        private void Start()
        {
            if (health == null || fill == null)
                return;

            _health = health.GetHealth();

            if (_health == null)
                return;

            // Subscribe — update fill whenever health changes
            _health.OnChanged += OnHealthChanged;

            // Set initial fill immediately
            Refresh(_health.Value, _health.MaxValue);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnChanged -= OnHealthChanged;
        }

        // =========================================
        // EVENT HANDLER
        // Called only when health actually changes
        // =========================================
        private void OnHealthChanged(int current)
        {
            Refresh(current, _health.MaxValue);
        }

        private void Refresh(int current, int max)
        {
            if (fill == null || max <= 0)
                return;

            float ratio = Mathf.Clamp01((float)current / max);
            fill.transform.localScale = new Vector3(ratio, 1f, 1f);
        }
    }
}
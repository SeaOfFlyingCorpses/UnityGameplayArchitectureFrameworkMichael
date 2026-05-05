using UnityEngine;
using UnityEngine.UI;
using Framework.Systems.Health;

namespace Gameplay.Systems.Health
{
    public class HealthBar : MonoBehaviour
    {
        public HealthComponent health;
        public Image           fill;

        private IHealth _health;

        private void Start()
        {
            if (health == null || fill == null)
                return;

            _health = health.GetHealth();

            if (_health == null)
                return;

            _health.OnChanged += OnHealthChanged;
            Refresh(_health.Value, _health.MaxValue);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnChanged -= OnHealthChanged;
        }

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
using System.Collections.Generic;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Combat
{
    // =========================================
    // ComboSystem
    // Tracks attack chains and applies
    // escalating damage multipliers.
    //
    // Each successful hit increments combo.
    // Combo resets after timeout.
    // At combo thresholds, bonus damage fires.
    //
    // Subscribes to HealthChangedEvent to
    // detect successful hits.
    //
    // Usage:
    //   Add to Player or any attacking agent.
    //   Read CurrentCombo in AttackState to
    //   choose which animation/ability to play.
    // =========================================
    public class ComboSystem : MonoBehaviour
    {
        [Header("Combo Settings")]
        public float  comboWindow    = 1.5f; // seconds before reset
        public int    maxCombo       = 10;

        [Header("Damage Multipliers")]
        [Tooltip("Multiplier at each combo threshold")]
        public List<ComboThreshold> thresholds = new List<ComboThreshold>
        {
            new ComboThreshold { comboCount = 3,  multiplier = 1.2f },
            new ComboThreshold { comboCount = 5,  multiplier = 1.5f },
            new ComboThreshold { comboCount = 10, multiplier = 2.0f },
        };

        public int   CurrentCombo   { get; private set; }
        public float Multiplier     { get; private set; } = 1f;

        private float _lastHitTime;

        private void OnEnable()
        {
            EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
        }

        private void Update()
        {
            // Reset combo after window expires
            if (CurrentCombo > 0 &&
                Time.time > _lastHitTime + comboWindow)
            {
                ResetCombo();
            }
        }

        private void OnHealthChanged(HealthChangedEvent e)
        {
            // Only count damage dealt to others — not self
            if (e.Source == gameObject) return;
            if (e.Delta >= 0) return; // heal not damage

            RegisterHit();
        }

        public void RegisterHit()
        {
            _lastHitTime = Time.time;
            CurrentCombo = Mathf.Min(CurrentCombo + 1, maxCombo);

            UpdateMultiplier();

            EventBus.Publish(new ComboUpdatedEvent(
                CurrentCombo, Multiplier));
        }

        public void ResetCombo()
        {
            if (CurrentCombo == 0) return;

            CurrentCombo = 0;
            Multiplier   = 1f;

            EventBus.Publish(new ComboResetEvent());
        }

        private void UpdateMultiplier()
        {
            Multiplier = 1f;

            foreach (var threshold in thresholds)
                if (CurrentCombo >= threshold.comboCount)
                    Multiplier = threshold.multiplier;
        }

        [System.Serializable]
        public class ComboThreshold
        {
            public int   comboCount;
            public float multiplier;
        }
    }
}

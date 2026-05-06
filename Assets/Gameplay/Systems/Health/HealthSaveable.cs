using System;
using UnityEngine;
using Framework.Persistence;
using Framework.Core;

namespace Gameplay.Systems.Health
{
    // =========================================
    // HealthSaveable
    // Add alongside HealthComponent to make
    // health persist across sessions.
    //
    // SaveId should be unique per agent —
    // set it in the Inspector.
    // =========================================
    public class HealthSaveable : MonoBehaviour, ISaveable
    {
        [Tooltip("Must be unique per agent — e.g. 'Player', 'Boss', 'Guard_01'")]
        public string saveId = "Agent";

        private HealthComponent _health;

        public string SaveId => saveId;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            ServiceLocator.Get<SaveSystem>()?.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<SaveSystem>()?.Unregister(this);
        }

        public object CaptureState()
        {
            return new HealthSaveData
            {
                currentValue = _health.GetHealth().Value,
                maxValue     = _health.GetHealth().MaxValue
            };
        }

        public void RestoreState(object state)
        {
            var data = state as HealthSaveData;
            if (data == null) return;

            var health = _health.GetHealth();

            // Reset then apply damage to reach saved value
            health.Reset();
            int diff = health.MaxValue - data.currentValue;
            if (diff > 0)
                health.Damage(diff);
        }

        [Serializable]
        public class HealthSaveData
        {
            public int currentValue;
            public int maxValue;
        }
    }
}

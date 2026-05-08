using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Progression;
using Gameplay.Systems.Health;

namespace Gameplay.Progression
{
    // =========================================
    // XPSource
    // Attach to any enemy or object that grants
    // experience when destroyed or interacted with.
    //
    // Subscribes to HealthChangedEvent for this
    // agent — grants XP to player on death.
    //
    // Setup:
    //   1. Add component to enemy prefab
    //   2. Set XP Amount
    //   3. Done — player gets XP when enemy dies
    // =========================================
    public class XPSource : MonoBehaviour
    {
        [Header("Experience")]
        public int xpAmount = 50;

        [Tooltip("Scale XP by enemy level if it has a LevelSystem")]
        public bool scaleWithLevel = true;

        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            int amount = xpAmount;

            // Scale with own level if present
            if (scaleWithLevel)
            {
                var ownLevel = GetComponent<LevelSystem>();
                if (ownLevel != null)
                    amount = Mathf.RoundToInt(
                        xpAmount * (1f + (ownLevel.Level - 1) * 0.1f));
            }

            // Grant to player via ServiceLocator
            var playerLevel = ServiceLocator.Get<ILevelSystem>();
            if (playerLevel == null) return;

            playerLevel.AddExperience(amount);

            Debug.Log($"[XPSource] {gameObject.name} granted " +
                      $"{amount} XP to player.");
        }
    }
}

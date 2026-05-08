using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Progression;

namespace Gameplay.Progression
{
    // =========================================
    // LevelSystem
    // MonoBehaviour implementation of ILevelSystem.
    //
    // Attach to Player for player leveling.
    // Attach to any agent for per-agent levels.
    // Register player instance as service for
    // global access:
    //   ServiceLocator.Register<ILevelSystem>(this)
    //
    // Fires LevelUpEvent via EventBus on level up.
    // Works with SaveSystem via ISaveable.
    // =========================================
    public class LevelSystem : MonoBehaviour,
        ILevelSystem,
        Framework.Persistence.ISaveable
    {
        [Header("Settings")]
        public int maxLevel = 99;

        [Header("XP Curve")]
        public ExperienceCurve curve = new ExperienceCurve();

        [Header("Register as Service")]
        [Tooltip("Register with ServiceLocator for global access")]
        public bool registerAsService = true;

        // =========================================
        // ILevelSystem
        // =========================================
        public int   Level      { get; private set; } = 1;
        public int   Experience { get; private set; } = 0;
        public bool  IsMaxLevel => Level >= maxLevel;

        public int NextLevel =>
            IsMaxLevel ? 0 : curve.GetRequired(Level + 1);

        public float Progress =>
            IsMaxLevel ? 1f :
            NextLevel  > 0
                ? (float)Experience / NextLevel
                : 0f;

        private void Awake()
        {
            if (registerAsService)
                ServiceLocator.Register<ILevelSystem>(this);
        }

        private void OnDestroy()
        {
            if (registerAsService)
                ServiceLocator.Unregister<ILevelSystem>();
        }

        // =========================================
        // ILevelSystem
        // =========================================
        public void AddExperience(int amount)
        {
            if (IsMaxLevel || amount <= 0) return;

            Experience += amount;

            EventBus.Publish(new ExperienceGainedEvent(
                amount, Experience, NextLevel));

            // Check for level up — may level multiple times
            while (!IsMaxLevel && Experience >= NextLevel)
            {
                Experience -= NextLevel;
                LevelUp();
            }

            // Cap at max
            if (IsMaxLevel) Experience = 0;
        }

        public void SetLevel(int level)
        {
            Level      = Mathf.Clamp(level, 1, maxLevel);
            Experience = 0;
        }

        private void LevelUp()
        {
            int old = Level;
            Level = Mathf.Min(Level + 1, maxLevel);

            Debug.Log($"[LevelSystem] {gameObject.name} " +
                      $"leveled up: {old} → {Level}");

            EventBus.Publish(new LevelUpEvent(old, Level));
        }

        // =========================================
        // ISaveable
        // =========================================
        public string SaveId => $"LevelSystem_{gameObject.name}";

        public object CaptureState() => new SaveData
        {
            level      = Level,
            experience = Experience
        };

        public void RestoreState(object state)
        {
            if (state is not SaveData data) return;
            Level      = data.level;
            Experience = data.experience;
        }

        [System.Serializable]
        public class SaveData
        {
            public int level;
            public int experience;
        }
    }
}
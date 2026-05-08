using System.Collections.Generic;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Progression
{
    // =========================================
    // StatSystem
    // Tracks base stats and bonuses.
    // Subscribes to LevelUpEvent to apply
    // stat gains per level automatically.
    //
    // Stats are read by abilities, health builds,
    // and movement — via ServiceLocator or direct
    // component reference.
    //
    // Built-in stats:
    //   Strength    — physical damage
    //   Agility     — speed, crit chance
    //   Vitality    — max health
    //   Intelligence — magic damage, mana
    //   Defence     — damage reduction
    // =========================================
    public class StatSystem : MonoBehaviour
    {
        [System.Serializable]
        public class StatEntry
        {
            public string name;
            public float  baseValue;
            public float  perLevel;  // added each level up
        }

        [Header("Stats")]
        public List<StatEntry> stats = new List<StatEntry>
        {
            new StatEntry { name = "Strength",     baseValue = 10, perLevel = 2 },
            new StatEntry { name = "Agility",      baseValue = 10, perLevel = 2 },
            new StatEntry { name = "Vitality",     baseValue = 10, perLevel = 3 },
            new StatEntry { name = "Intelligence", baseValue = 10, perLevel = 2 },
            new StatEntry { name = "Defence",      baseValue = 5,  perLevel = 1 },
        };

        // Runtime values — base + all bonuses
        private readonly Dictionary<string, float> _values  = new();
        private readonly Dictionary<string, float> _bonuses = new();

        private void Awake()
        {
            foreach (var stat in stats)
                _values[stat.name] = stat.baseValue;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelUpEvent>(OnLevelUp);
        }

        // =========================================
        // READ
        // =========================================
        public float Get(string statName)
        {
            float value   = 0f;
            float bonus   = 0f;

            _values .TryGetValue(statName, out value);
            _bonuses.TryGetValue(statName, out bonus);

            return value + bonus;
        }

        public float GetBase(string statName)
        {
            _values.TryGetValue(statName, out var v);
            return v;
        }

        // =========================================
        // WRITE — for equipment, buffs, debuffs
        // =========================================
        public void AddBonus(string statName, float amount)
        {
            _bonuses.TryGetValue(statName, out var current);
            _bonuses[statName] = current + amount;
        }

        public void RemoveBonus(string statName, float amount)
        {
            _bonuses.TryGetValue(statName, out var current);
            _bonuses[statName] = current - amount;
        }

        public void SetBase(string statName, float value)
        {
            _values[statName] = value;
        }

        // =========================================
        // LEVEL UP — apply per-level gains
        // =========================================
        private void OnLevelUp(LevelUpEvent e)
        {
            int levelsGained = e.NewLevel - e.OldLevel;

            foreach (var stat in stats)
            {
                if (!_values.ContainsKey(stat.name))
                    _values[stat.name] = stat.baseValue;

                _values[stat.name] += stat.perLevel * levelsGained;
            }

            Debug.Log($"[StatSystem] {gameObject.name} " +
                      $"stats updated for level {e.NewLevel}");
        }
    }
}

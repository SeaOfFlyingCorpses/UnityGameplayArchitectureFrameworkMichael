using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Progression;

namespace Gameplay.Progression
{
    // =========================================
    // ClassSystem
    // MonoBehaviour implementation of IClassSystem.
    // Attach to Player.
    //
    // Works with StatSystem — applies stat bonuses
    // when a class is selected or evolved.
    //
    // Works with LevelSystem — checks required
    // level before allowing evolution.
    //
    // To integrate your friend's class system:
    //   Create an adapter class:
    //   public class FriendClassAdapter : IClassSystem
    //   {
    //       private FriendClassSystem _friend;
    //       // wrap his methods here
    //   }
    //   Register it instead of this component.
    // =========================================
    public class ClassSystem : MonoBehaviour, IClassSystem
    {
        [Header("Starting Classes")]
        [Tooltip("All classes available at game start (Tier 1)")]
        public List<ClassAsset> startingClasses = new();

        [Header("All Classes")]
        [Tooltip("Every class in the game — needed for evolution lookup")]
        public List<ClassAsset> allClasses = new();

        public ICharacterClass           Current   { get; private set; }
        public IReadOnlyList<ICharacterClass> Available => _available;

        private readonly List<ICharacterClass>        _available = new();
        private readonly Dictionary<string, ICharacterClass> _registry  = new();

        private StatSystem   _stats;
        private LevelSystem  _levels;

        private void Awake()
        {
            ServiceLocator.Register<IClassSystem>(this);

            _stats  = GetComponent<StatSystem>();
            _levels = GetComponent<LevelSystem>();

            // Register all classes
            foreach (var asset in allClasses)
            {
                if (asset == null) continue;
                var cls = asset.Build();
                _registry[cls.Id] = cls;
            }

            // Build available list from starting classes
            foreach (var asset in startingClasses)
            {
                if (asset == null) continue;
                var id = string.IsNullOrEmpty(asset.classId)
                    ? asset.name : asset.classId;
                if (_registry.TryGetValue(id, out var cls))
                    _available.Add(cls);
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IClassSystem>();
        }

        // =========================================
        // IClassSystem
        // =========================================
        public void SelectClass(string classId)
        {
            if (!_registry.TryGetValue(classId, out var cls))
            {
                Debug.LogWarning(
                    $"[ClassSystem] Class '{classId}' not found.");
                return;
            }

            if (Current != null)
            {
                // Remove old class bonuses
                RemoveBonuses(Current);
            }

            Current = cls;
            ApplyBonuses(cls);

            // Unlock evolution paths as available
            UpdateAvailable();

            EventBus.Publish(new ClassSelectedEvent(
                cls.Id, cls.DisplayName, cls.Tier));
        }

        public bool CanEvolve(string classId)
        {
            if (Current == null) return false;
            if (!Current.EvolutionIds.Contains(classId)) return false;
            if (!_registry.TryGetValue(classId, out var target)) return false;

            int currentLevel = _levels != null ? _levels.Level : 1;
            return currentLevel >= target.RequiredLevel;
        }

        public void EvolveClass(string classId)
        {
            if (!CanEvolve(classId))
            {
                Debug.LogWarning(
                    $"[ClassSystem] Cannot evolve to '{classId}'. " +
                    $"Check level requirement or evolution path.");
                return;
            }

            string oldId = Current.Id;

            // Remove old bonuses, apply new
            RemoveBonuses(Current);

            Current = _registry[classId];
            ApplyBonuses(Current);

            UpdateAvailable();

            EventBus.Publish(new ClassEvolvedEvent(
                oldId,
                Current.Id,
                Current.DisplayName,
                Current.Tier));
        }

        // =========================================
        // INTERNAL
        // =========================================
        private void ApplyBonuses(ICharacterClass cls)
        {
            if (_stats == null) return;

            foreach (var bonus in cls.StatBonuses)
                _stats.AddBonus(bonus.StatName, bonus.Amount);
        }

        private void RemoveBonuses(ICharacterClass cls)
        {
            if (_stats == null) return;

            foreach (var bonus in cls.StatBonuses)
                _stats.RemoveBonus(bonus.StatName, bonus.Amount);
        }

        private void UpdateAvailable()
        {
            _available.Clear();

            if (Current == null) return;

            // Show all evolution paths from current class
            foreach (var evoId in Current.EvolutionIds)
                if (_registry.TryGetValue(evoId, out var evo))
                    _available.Add(evo);
        }
    }
}

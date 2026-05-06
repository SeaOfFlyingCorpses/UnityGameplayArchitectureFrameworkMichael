using System.Collections.Generic;
using System.Linq;
using Framework.Abilities;

namespace Gameplay.Abilities
{
    public class AbilitySystem : IAbilitySystem
    {
        private readonly List<Ability> _abilities = new();

        // IAbilitySystem.Register
        public void Register(object ability)
        {
            if (ability is Ability a) Register(a);
        }

        // Typed Register
        public void Register(Ability ability)
        {
            if (ability == null) return;
            _abilities.Add(ability);
        }

        // IAbilitySystem.Use — use by id
        public void Use(string abilityId, object context)
        {
            var ability = _abilities.FirstOrDefault(a => a.Id == abilityId);
            ability?.Use(context as AbilityContext);
        }

        // Typed Use — use by id
        public void Use(string abilityId, AbilityContext context)
        {
            var ability = _abilities.FirstOrDefault(a => a.Id == abilityId);
            ability?.Use(context);
        }

        // =========================================
        // USE BEST AVAILABLE
        // Tries abilities in priority order.
        // Uses first one that is off cooldown.
        // Returns true if an ability fired.
        // =========================================
        public bool UseBestAvailable(AbilityContext context)
        {
            if (_abilities.Count == 0) return false;

            // Sort by priority descending — highest priority first
            var sorted = _abilities.OrderByDescending(a => a.Priority);

            foreach (var ability in sorted)
            {
                if (!ability.CanUse()) continue;

                ability.Use(context);
                return true;
            }

            return false;
        }

        // =========================================
        // CAN USE ANY — check before attempting
        // =========================================
        public bool CanUseAny()
        {
            return _abilities.Any(a => a.CanUse());
        }
    }
}
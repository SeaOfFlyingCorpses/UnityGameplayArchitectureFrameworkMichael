using System.Collections.Generic;
using Framework.Abilities;

namespace Gameplay.Abilities
{
    public class AbilitySystem : IAbilitySystem
    {
        private readonly Dictionary<string, Ability> _abilities = new();

        // IAbilitySystem.Register — accepts object for interface compatibility
        public void Register(object ability)
        {
            if (ability is Ability a)
                Register(a);
        }

        // Typed Register for Gameplay code
        public void Register(Ability ability)
        {
            if (ability == null) return;
            _abilities[ability.Id] = ability;
        }

        // IAbilitySystem.Use — object context for interface compatibility
        public void Use(string abilityId, object context)
        {
            if (_abilities.TryGetValue(abilityId, out var ability))
                ability.Use(context as AbilityContext);
        }

        // Typed Use for Gameplay code
        public void Use(string abilityId, AbilityContext context)
        {
            if (_abilities.TryGetValue(abilityId, out var ability))
                ability.Use(context);
        }
    }
}
using System.Collections.Generic;

namespace Gameplay.Abilities
{
    public class AbilitySystem
    {
        private Dictionary<string, Ability> _abilities = new();

        public void Register(Ability ability)
        {
            _abilities[ability.Id] = ability;
        }

        public void Use(string id, AbilityContext context)
        {
            if (_abilities.TryGetValue(id, out var ability))
            {
                ability.Use(context);
            }
        }
    }
}
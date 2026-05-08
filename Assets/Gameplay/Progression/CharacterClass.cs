using System.Collections.Generic;
using Framework.Progression;

namespace Gameplay.Progression
{
    // =========================================
    // CharacterClass
    // Plain C# — no MonoBehaviour.
    // Built via code or ClassAsset ScriptableObject.
    //
    // Usage:
    //   new CharacterClass("warrior", "Warrior", tier: 1)
    //       .AddStatBonus("Strength",  5)
    //       .AddStatBonus("Vitality",  3)
    //       .AddAbility("basic_slash")
    //       .AddEvolution("knight")
    //       .AddEvolution("berserker")
    //       .RequiresLevel(1)
    // =========================================
    public class CharacterClass : ICharacterClass
    {
        public string Id           { get; }
        public string DisplayName  { get; }
        public string Description  { get; private set; }
        public int    Tier         { get; }
        public int    RequiredLevel{ get; private set; }

        private readonly List<StatBonus> _statBonuses  = new();
        private readonly List<string>    _abilityIds   = new();
        private readonly List<string>    _evolutionIds = new();

        public IReadOnlyList<StatBonus> StatBonuses  => _statBonuses;
        public IReadOnlyList<string>    AbilityIds   => _abilityIds;
        public IReadOnlyList<string>    EvolutionIds => _evolutionIds;

        public CharacterClass(string id, string displayName, int tier = 1)
        {
            Id          = id;
            DisplayName = displayName;
            Tier        = tier;
        }

        // =========================================
        // BUILDER API
        // =========================================
        public CharacterClass SetDescription(string desc)
        {
            Description = desc;
            return this;
        }

        public CharacterClass RequiresLevel(int level)
        {
            RequiredLevel = level;
            return this;
        }

        public CharacterClass AddStatBonus(
            string statName,
            float  amount,
            bool   isPercentage = false)
        {
            _statBonuses.Add(new StatBonus(statName, amount, isPercentage));
            return this;
        }

        public CharacterClass AddAbility(string abilityId)
        {
            if (!_abilityIds.Contains(abilityId))
                _abilityIds.Add(abilityId);
            return this;
        }

        public CharacterClass AddEvolution(string classId)
        {
            if (!_evolutionIds.Contains(classId))
                _evolutionIds.Add(classId);
            return this;
        }
    }
}

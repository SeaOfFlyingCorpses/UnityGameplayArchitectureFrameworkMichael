using System.Collections.Generic;

namespace Framework.Progression
{
    // =========================================
    // ICharacterClass
    // Defines a playable class.
    //
    // Examples:
    //   Tier 1: Warrior, Mage, Rogue
    //   Tier 2: Knight/Berserker, Wizard/Sorcerer
    //   Tier 3: Paladin/DarkKnight, Archmage/Necromancer
    // =========================================
    public interface ICharacterClass
    {
        string              Id           { get; }
        string              DisplayName  { get; }
        string              Description  { get; }
        int                 Tier         { get; } // 1=base, 2=evolved, 3=ascended
        int                 RequiredLevel{ get; } // min level to unlock/evolve

        // Stat bonuses this class provides
        IReadOnlyList<StatBonus>      StatBonuses  { get; }

        // Abilities this class unlocks
        IReadOnlyList<string>         AbilityIds   { get; }

        // Classes this evolves into
        IReadOnlyList<string>         EvolutionIds { get; }
    }

    // =========================================
    // StatBonus
    // A flat or percentage bonus to a stat.
    // Applied by StatSystem on class selection.
    // =========================================
    public struct StatBonus
    {
        public string StatName;
        public float  Amount;
        public bool   IsPercentage; // true = 20% bonus, false = +20 flat

        public StatBonus(string statName, float amount, bool isPercentage = false)
        {
            StatName     = statName;
            Amount       = amount;
            IsPercentage = isPercentage;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Framework.Progression;

namespace Gameplay.Progression
{
    // =========================================
    // ClassAsset
    // ScriptableObject — create classes in
    // the Inspector without writing code.
    //
    // Create:
    //   Right click → Create → Gameplay → Class
    //
    // Example hierarchy:
    //   Tier 1: Warrior
    //   Tier 2: warrior_evolutions = [knight, berserker]
    //   Tier 3: knight_evolutions  = [paladin, dark_knight]
    // =========================================
    [CreateAssetMenu(
        fileName = "NewClass",
        menuName  = "Gameplay/Class")]
    public class ClassAsset : ScriptableObject
    {
        [System.Serializable]
        public class StatBonusData
        {
            public string statName;
            public float  amount;
            public bool   isPercentage;
        }

        [Header("Identity")]
        public string classId;
        public string displayName;
        [TextArea(2, 4)]
        public string description;

        [Header("Progression")]
        public int tier          = 1;
        public int requiredLevel = 1;

        [Header("Stat Bonuses")]
        public List<StatBonusData> statBonuses = new();

        [Header("Abilities Unlocked")]
        public List<string> abilityIds = new();

        [Header("Evolution Paths")]
        [Tooltip("Classes this evolves into at next tier")]
        public List<ClassAsset> evolutions = new();

        public ICharacterClass Build()
        {
            var cls = new CharacterClass(
                string.IsNullOrEmpty(classId) ? name : classId,
                displayName,
                tier)
                .SetDescription(description)
                .RequiresLevel(requiredLevel);

            foreach (var bonus in statBonuses)
                cls.AddStatBonus(
                    bonus.statName,
                    bonus.amount,
                    bonus.isPercentage);

            foreach (var id in abilityIds)
                cls.AddAbility(id);

            foreach (var evo in evolutions)
                if (evo != null)
                    cls.AddEvolution(
                        string.IsNullOrEmpty(evo.classId)
                            ? evo.name : evo.classId);

            return cls;
        }
    }
}

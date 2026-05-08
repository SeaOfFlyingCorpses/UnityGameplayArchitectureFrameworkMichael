using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Items
{
    // =========================================
    // LootEntry
    // One possible drop.
    // =========================================
    [System.Serializable]
    public class LootEntry
    {
        [Tooltip("Item to drop")]
        public ItemDefinition item;

        [Tooltip("Drop chance 0-1 (0.5 = 50%)")]
        [Range(0f, 1f)]
        public float dropChance = 0.5f;

        [Tooltip("How many to drop")]
        public int minCount = 1;
        public int maxCount = 1;
    }

    // =========================================
    // LootTable
    // ScriptableObject defining what an enemy
    // drops on death.
    //
    // Create:
    //   Right click → Create → Gameplay → Loot Table
    //
    // Usage:
    //   Drag onto LootSystem component on enemy.
    // =========================================
    [CreateAssetMenu(
        fileName = "NewLootTable",
        menuName  = "Gameplay/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [Header("Drops")]
        public List<LootEntry> entries = new();

        [Header("Guaranteed Drops")]
        [Tooltip("Always drop these regardless of chance")]
        public List<LootEntry> guaranteed = new();

        // =========================================
        // Roll — returns what to drop this death
        // =========================================
        public List<(ItemDefinition item, int count)> Roll()
        {
            var result = new List<(ItemDefinition, int)>();

            // Guaranteed drops
            foreach (var entry in guaranteed)
            {
                if (entry.item == null) continue;
                int count = Random.Range(entry.minCount, entry.maxCount + 1);
                result.Add((entry.item, count));
            }

            // Chance drops
            foreach (var entry in entries)
            {
                if (entry.item == null) continue;
                if (Random.value > entry.dropChance) continue;
                int count = Random.Range(entry.minCount, entry.maxCount + 1);
                result.Add((entry.item, count));
            }

            return result;
        }
    }
}

using UnityEngine;
using Framework.Items;

namespace Gameplay.Items
{
    // =========================================
    // ItemDefinition
    // ScriptableObject — one asset per item type.
    //
    // Create:
    //   Right click → Create → Gameplay → Item Definition
    //
    // Examples:
    //   HealthPotion.asset   — stackable, consumable
    //   IronSword.asset      — not stackable, weapon
    //   GoldCoin.asset       — stackable, currency
    //   QuestKey.asset       — not stackable, quest
    // =========================================
    [CreateAssetMenu(
        fileName = "ItemDefinition",
        menuName = "Gameplay/Item Definition")]
    public class ItemDefinition : ScriptableObject, IItem
    {
        [Header("Identity")]
        public string itemId      = "item";
        public string displayName = "Item";

        [Header("Stacking")]
        [Tooltip("Max stack size. 1 = not stackable.")]
        public int stackSize = 1;

        [Header("Display")]
        public Sprite icon;

        [TextArea]
        public string description;

        // IItem
        public string Id          => itemId;
        public string DisplayName => displayName;
        public int    StackSize   => stackSize;
    }
}

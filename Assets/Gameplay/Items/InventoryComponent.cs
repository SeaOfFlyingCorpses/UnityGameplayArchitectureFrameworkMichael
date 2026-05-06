using UnityEngine;
using Framework.Items;
using Framework.Core;

namespace Gameplay.Items
{
    // =========================================
    // InventoryComponent
    // Add to any GameObject to give it an
    // inventory. Player, chest, shop, enemy loot.
    //
    // Access from anywhere:
    //   var inv = GetComponent<InventoryComponent>();
    //   inv.Add(itemDef, 1);
    //
    // Or via ServiceLocator if registered:
    //   ServiceLocator.Get<InventoryComponent>();
    // =========================================
    public class InventoryComponent : MonoBehaviour
    {
        [Header("Settings")]
        public int    capacity = 20;
        public string ownerId  = "Player";

        public Inventory Inventory { get; private set; }

        private void Awake()
        {
            Inventory = new Inventory(capacity, ownerId);
        }

        // =========================================
        // CONVENIENCE METHODS
        // =========================================
        public bool Add(ItemDefinition item, int amount = 1)
            => Inventory.Add(item, amount);

        public bool Remove(string itemId, int amount = 1)
            => Inventory.Remove(itemId, amount);

        public bool Has(string itemId, int amount = 1)
            => Inventory.Has(itemId, amount);

        public int GetCount(string itemId)
            => Inventory.GetCount(itemId);
    }
}

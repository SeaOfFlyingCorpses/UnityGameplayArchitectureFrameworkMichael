using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Items;
using Framework.Core;
using Framework.Persistence;

namespace Gameplay.Items
{
    // =========================================
    // InventoryComponent
    // Add to any GameObject to give it an
    // inventory. Player, chest, shop, enemy loot.
    //
    // Now implements ISaveable — inventory
    // persists across sessions via SaveSystem.
    // Requires ItemDatabase to reconstruct items.
    //
    // Access from anywhere:
    //   var inv = GetComponent<InventoryComponent>();
    //   inv.Add(itemDef, 1);
    // =========================================
    public class InventoryComponent : MonoBehaviour, ISaveable
    {
        [Header("Settings")]
        public int    capacity = 20;
        public string ownerId  = "Player";

        public Inventory Inventory { get; private set; }

        private void Awake()
        {
            Inventory = new Inventory(capacity, ownerId);
        }

        private void Start()
        {
            ServiceLocator.Get<Framework.Persistence.SaveSystem>()
                ?.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<Framework.Persistence.SaveSystem>()
                ?.Unregister(this);
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

        // =========================================
        // ISaveable
        // =========================================
        public string SaveId => $"Inventory_{ownerId}";

        public object CaptureState()
        {
            var data = new SaveData();
            data.slots = new List<SlotData>();

            foreach (var slot in Inventory.Slots)
            {
                if (slot?.Item == null) continue;
                data.slots.Add(new SlotData
                {
                    itemId = slot.Item.Id,
                    count  = slot.Count
                });
            }

            return data;
        }

        public void RestoreState(object state)
        {
            if (state is not SaveData data) return;

            var db = ServiceLocator.Get<IItemDatabase>();
            if (db == null)
            {
                Debug.LogWarning(
                    "[InventoryComponent] No IItemDatabase found. " +
                    "Cannot restore inventory. Add ItemDatabaseLoader " +
                    "to _GameSystems.");
                return;
            }

            Inventory = new Inventory(capacity, ownerId);

            foreach (var slot in data.slots)
            {
                var item = db.Get(slot.itemId);
                if (item is ItemDefinition def)
                    Inventory.Add(def, slot.count);
            }
        }

        [Serializable]
        public class SaveData
        {
            public List<SlotData> slots = new();
        }

        [Serializable]
        public class SlotData
        {
            public string itemId;
            public int    count;
        }
    }
}
using System;
using System.Collections.Generic;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Framework.Items
{
    // =========================================
    // Inventory
    // Standard inventory with capacity and
    // item stacking. Fires InventoryChangedEvent
    // via EventBus on any change.
    //
    // Usage:
    //   var inv = new Inventory(capacity: 20);
    //   inv.Add(sword, 1);
    //   inv.Remove("sword", 1);
    //   inv.Has("potion", 3);
    // =========================================
    public class Inventory : IInventory
    {
        private readonly List<InventorySlot> _slots = new();
        private readonly int                 _capacity;

        public IReadOnlyList<InventorySlot> Slots => _slots;

        public string OwnerId { get; }

        public Inventory(int capacity = 20, string ownerId = "")
        {
            _capacity = capacity;
            OwnerId   = ownerId;
        }

        // =========================================
        // ADD
        // =========================================
        public bool Add(IItem item, int amount = 1)
        {
            if (item == null || amount <= 0) return false;

            // Try to stack into existing slot
            if (item.IsStackable)
            {
                var existing = FindSlot(item.Id);
                if (existing != null)
                {
                    existing.Add(amount);
                    PublishChanged(item.Id, amount);
                    return true;
                }
            }

            // Need new slot
            if (_slots.Count >= _capacity)
            {
                return false; // inventory full
            }

            _slots.Add(new InventorySlot(item, amount));
            PublishChanged(item.Id, amount);
            return true;
        }

        // =========================================
        // REMOVE
        // =========================================
        public bool Remove(string itemId, int amount = 1)
        {
            var slot = FindSlot(itemId);
            if (slot == null || slot.Count < amount)
                return false;

            slot.Remove(amount);

            if (slot.IsEmpty)
                _slots.Remove(slot);

            PublishChanged(itemId, -amount);
            return true;
        }

        // =========================================
        // QUERY
        // =========================================
        public bool Has(string itemId, int amount = 1)
            => GetCount(itemId) >= amount;

        public int GetCount(string itemId)
        {
            var slot = FindSlot(itemId);
            return slot?.Count ?? 0;
        }

        private InventorySlot FindSlot(string itemId)
        {
            foreach (var slot in _slots)
                if (slot.Item.Id == itemId)
                    return slot;
            return null;
        }

        private void PublishChanged(string itemId, int delta)
        {
            EventBus.Publish(new InventoryChangedEvent(OwnerId, itemId, delta));
        }
    }
}

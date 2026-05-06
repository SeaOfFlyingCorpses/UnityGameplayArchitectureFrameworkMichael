using System.Collections.Generic;

namespace Framework.Items
{
    // =========================================
    // IInventory
    // Contract for any inventory container.
    // Works for player bag, chest, shop, hotbar.
    // =========================================
    public interface IInventory
    {
        bool Add(IItem item, int amount = 1);
        bool Remove(string itemId, int amount = 1);
        bool Has(string itemId, int amount = 1);
        int  GetCount(string itemId);

        IReadOnlyList<InventorySlot> Slots { get; }
    }
}

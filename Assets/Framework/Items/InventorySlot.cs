namespace Framework.Items
{
    // =========================================
    // InventorySlot
    // One slot in an inventory.
    // Holds a reference to an item and a count.
    // =========================================
    public class InventorySlot
    {
        public IItem Item   { get; }
        public int   Count  { get; private set; }
        public bool  IsEmpty => Count <= 0;

        public InventorySlot(IItem item, int count = 1)
        {
            Item  = item;
            Count = count;
        }

        public void Add(int amount)
        {
            Count += amount;
        }

        public void Remove(int amount)
        {
            Count = System.Math.Max(0, Count - amount);
        }
    }
}

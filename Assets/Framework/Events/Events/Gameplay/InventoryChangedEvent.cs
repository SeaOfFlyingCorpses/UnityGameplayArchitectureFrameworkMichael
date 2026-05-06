namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // InventoryChangedEvent
    // Fired via EventBus when any inventory
    // gains or loses items.
    //
    // Delta > 0 = items added
    // Delta < 0 = items removed
    //
    // Subscribe to update UI, trigger quests,
    // play pickup sounds etc.
    // =========================================
    public struct InventoryChangedEvent
    {
        public string OwnerId;   // which inventory changed
        public string ItemId;    // which item
        public int    Delta;     // how many added/removed

        public InventoryChangedEvent(
            string ownerId,
            string itemId,
            int    delta)
        {
            OwnerId = ownerId;
            ItemId  = itemId;
            Delta   = delta;
        }
    }
}

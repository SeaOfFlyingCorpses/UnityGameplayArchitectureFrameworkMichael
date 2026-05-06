namespace Framework.Items
{
    // =========================================
    // IItem
    // Contract for any item in the game.
    // Implement to create weapons, consumables,
    // equipment, quest items, crafting materials.
    // =========================================
    public interface IItem
    {
        string Id          { get; }
        string DisplayName { get; }
        int    StackSize   { get; }   // 1 = not stackable
        bool   IsStackable => StackSize > 1;
    }
}

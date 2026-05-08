namespace Framework.Items
{
    // =========================================
    // IItemDatabase
    // Look up any item by its id from anywhere.
    // Required for inventory save/load.
    //
    // Usage:
    //   ServiceLocator.Get<IItemDatabase>()
    //       ?.Get("health_potion");
    // =========================================
    public interface IItemDatabase
    {
        IItem      Get       (string itemId);
        bool       Contains  (string itemId);
        IItem[]    GetAll    ();
    }
}

using UnityEngine;
using Framework.Core;
using Framework.Items;

namespace Gameplay.Items
{
    // =========================================
    // ItemDatabaseLoader
    // Registers the ItemDatabase with
    // ServiceLocator at game start.
    // Place on _GameSystems.
    //
    // Setup:
    //   1. Create ItemDatabase asset
    //      (Right click → Create → Gameplay → Item Database)
    //   2. Drag all ItemDefinition assets into it
    //   3. Add ItemDatabaseLoader to _GameSystems
    //   4. Drag ItemDatabase asset into slot
    // =========================================
    public class ItemDatabaseLoader : MonoBehaviour
    {
        [Header("Database")]
        public ItemDatabase database;

        private void Awake()
        {
            if (database == null)
            {
                Debug.LogWarning(
                    "[ItemDatabaseLoader] No ItemDatabase assigned. " +
                    "Inventory save/load will not work.");
                return;
            }

            ServiceLocator.Register<IItemDatabase>(database);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IItemDatabase>();
        }
    }
}

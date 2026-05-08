using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Items;

namespace Gameplay.Items
{
    // =========================================
    // ItemDatabase
    // ScriptableObject registry of all items.
    // Create ONE instance and put it in Resources
    // or drag into ItemDatabaseLoader on _GameSystems.
    //
    // Create:
    //   Right click → Create → Gameplay → Item Database
    //
    // Usage from code:
    //   ServiceLocator.Get<IItemDatabase>()
    //       ?.Get("health_potion");
    //
    // Usage for save/load:
    //   var db = ServiceLocator.Get<IItemDatabase>();
    //   var item = db.Get(savedItemId);
    //   inventory.Add(item, savedCount);
    // =========================================
    [CreateAssetMenu(
        fileName = "ItemDatabase",
        menuName  = "Gameplay/Item Database")]
    public class ItemDatabase : ScriptableObject, IItemDatabase
    {
        [Header("Items")]
        [Tooltip("Drag every ItemDefinition asset here")]
        public List<ItemDefinition> items = new();

        private Dictionary<string, IItem> _lookup;

        // =========================================
        // Build lookup on first access
        // =========================================
        private void BuildLookup()
        {
            if (_lookup != null) return;

            _lookup = new Dictionary<string, IItem>();

            foreach (var item in items)
            {
                if (item == null) continue;

                if (_lookup.ContainsKey(item.Id))
                {
                    Debug.LogWarning(
                        $"[ItemDatabase] Duplicate item id '{item.Id}'. " +
                        $"Check your ItemDefinition assets.");
                    continue;
                }

                _lookup[item.Id] = item;
            }
        }

        // =========================================
        // IItemDatabase
        // =========================================
        public IItem Get(string itemId)
        {
            BuildLookup();

            if (string.IsNullOrEmpty(itemId)) return null;

            _lookup.TryGetValue(itemId, out var item);

            if (item == null)
                Debug.LogWarning(
                    $"[ItemDatabase] Item '{itemId}' not found. " +
                    $"Make sure it is added to the database.");

            return item;
        }

        public bool Contains(string itemId)
        {
            BuildLookup();
            return !string.IsNullOrEmpty(itemId) &&
                   _lookup.ContainsKey(itemId);
        }

        public IItem[] GetAll()
        {
            BuildLookup();
            var all = new IItem[_lookup.Count];
            _lookup.Values.CopyTo(all, 0);
            return all;
        }

        // =========================================
        // Editor helper — rebuild lookup in editor
        // =========================================
        private void OnValidate()
        {
            _lookup = null; // force rebuild
        }
    }
}

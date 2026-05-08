using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Items
{
    // =========================================
    // LootSystem
    // Attach to any enemy to drop items on death.
    // Spawns item pickups in the world.
    //
    // Setup:
    //   1. Add LootSystem to enemy prefab
    //   2. Create a LootTable asset
    //   3. Drag LootTable into slot
    //   4. Optionally assign a pickup prefab
    //      (must have InteractableItem component)
    //      or items go directly to nearest player
    // =========================================
    public class LootSystem : MonoBehaviour
    {
        [Header("Loot")]
        public LootTable lootTable;

        [Header("Drop Style")]
        [Tooltip("True = spawn pickup in world. " +
                 "False = auto-add to nearest player inventory.")]
        public bool spawnInWorld = true;

        [Tooltip("Prefab with InteractableItem component")]
        public GameObject itemPickupPrefab;

        [Tooltip("Scatter radius around death position")]
        public float scatterRadius = 1f;

        private Gameplay.Systems.Health.HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<Gameplay.Systems.Health.HealthComponent>();
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            if (lootTable == null) return;

            var drops = lootTable.Roll();

            if (drops.Count == 0) return;

            if (spawnInWorld)
                SpawnDrops(drops);
            else
                GiveToNearestPlayer(drops);
        }

        private void SpawnDrops(
            System.Collections.Generic.List<
                (ItemDefinition item, int count)> drops)
        {
            if (itemPickupPrefab == null)
            {
                // No prefab — fall back to giving directly
                GiveToNearestPlayer(drops);
                return;
            }

            var registry = ServiceLocator.Get<PoolRegistry>();

            foreach (var (item, count) in drops)
            {
                // Scatter around death position
                Vector2 scatter = Random.insideUnitCircle * scatterRadius;
                Vector3 pos = transform.position +
                              new Vector3(scatter.x, 0.5f, scatter.y);

                GameObject pickup;

                if (registry != null &&
                    registry.HasPool(itemPickupPrefab.name))
                    pickup = registry.Get(
                        itemPickupPrefab.name, pos, Quaternion.identity);
                else
                    pickup = Instantiate(itemPickupPrefab, pos,
                                         Quaternion.identity);

                var interactable = pickup
                    .GetComponent<Gameplay.Interaction.InteractableItem>();

                if (interactable != null)
                {
                    interactable.itemDefinition = item;
                    interactable.quantity       = count;
                }
            }
        }

        private void GiveToNearestPlayer(
            System.Collections.Generic.List<
                (ItemDefinition item, int count)> drops)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) return;

            var inventory = playerObj
                .GetComponent<Gameplay.Items.InventoryComponent>();
            if (inventory == null) return;

            foreach (var (item, count) in drops)
            {
                inventory.Add(item, count);

                EventBus.Publish(new InventoryChangedEvent(
                    inventory.ownerId, item.Id, count));
            }
        }
    }
}

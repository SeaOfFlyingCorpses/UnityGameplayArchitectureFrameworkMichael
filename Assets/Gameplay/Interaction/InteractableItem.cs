using UnityEngine;
using Framework.Core;
using Framework.Interaction;
using Framework.Items;

namespace Gameplay.Interaction
{
    // =========================================
    // InteractableItem
    // A pickup in the world.
    // Player walks near it and presses E.
    // Adds item to inventory then disables self.
    //
    // Setup:
    //   1. Add to any pickup GameObject
    //   2. Add a Collider (trigger or solid)
    //   3. Set Item Definition and Quantity
    // =========================================
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        [Header("Item")]
        public Gameplay.Items.ItemDefinition itemDefinition;
        public int quantity = 1;

        [Header("Prompt")]
        public string promptText = "Pick up";

        public string PromptText => string.IsNullOrEmpty(promptText)
            ? $"Pick up {itemDefinition?.DisplayName}"
            : promptText;

        public bool CanInteract(Transform interactor) =>
            gameObject.activeSelf && itemDefinition != null;

        public void Interact(Transform interactor)
        {
            var inventory = interactor
                .GetComponent<Gameplay.Items.InventoryComponent>();

            if (inventory == null) return;

            inventory.Add(itemDefinition, quantity);

            // Return to pool or disable
            var registry = ServiceLocator.Get<Framework.Core.PoolRegistry>();
            if (registry != null && registry.HasPool(gameObject.name))
                registry.Return(gameObject.name, gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}

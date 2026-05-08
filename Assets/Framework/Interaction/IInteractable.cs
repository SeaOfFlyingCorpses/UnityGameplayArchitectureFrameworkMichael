using UnityEngine;

namespace Framework.Interaction
{
    // =========================================
    // IInteractable
    // Any object the player can interact with.
    // Implement on doors, NPCs, items, switches,
    // chests, levers, terminals — anything.
    //
    // Usage:
    //   public class Door : MonoBehaviour,
    //       IInteractable
    //   {
    //       public string PromptText => "Open Door";
    //       public bool   CanInteract(Transform t)
    //           => !_isOpen;
    //       public void   Interact(Transform t)
    //           => Open();
    //   }
    // =========================================
    public interface IInteractable
    {
        // Text shown to player — "Open", "Talk", "Pick up"
        string PromptText { get; }

        // Return false to disable interaction
        // e.g. locked door, dead NPC
        bool CanInteract(Transform interactor);

        // Called when player interacts
        void Interact(Transform interactor);
    }
}

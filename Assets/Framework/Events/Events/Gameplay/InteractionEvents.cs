using UnityEngine;

namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // InteractionFocusedEvent
    // Fired when player aims at an interactable.
    // UI subscribes to show prompt ("Press E").
    // =========================================
    public struct InteractionFocusedEvent
    {
        public string    PromptText;
        public Transform Target;

        public InteractionFocusedEvent(string promptText, Transform target)
        {
            PromptText = promptText;
            Target     = target;
        }
    }

    // =========================================
    // InteractionUnfocusedEvent
    // Fired when player looks away.
    // UI subscribes to hide prompt.
    // =========================================
    public struct InteractionUnfocusedEvent { }

    // =========================================
    // InteractedEvent
    // Fired when interaction completes.
    // Quest system subscribes to track objectives.
    // =========================================
    public struct InteractedEvent
    {
        public string    InteractableId;
        public Transform Interactor;
        public Transform Target;

        public InteractedEvent(
            string    interactableId,
            Transform interactor,
            Transform target)
        {
            InteractableId = interactableId;
            Interactor     = interactor;
            Target         = target;
        }
    }
}

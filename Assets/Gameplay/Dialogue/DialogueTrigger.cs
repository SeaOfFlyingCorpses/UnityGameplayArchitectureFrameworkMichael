using UnityEngine;
using Framework.Core;
using Framework.Dialogue;

namespace Gameplay.Dialogue
{
    // =========================================
    // DialogueTrigger
    // Attach to any NPC or interactable object.
    // Call Trigger() to start the dialogue.
    // Works with the Interaction system (Step 60).
    //
    // Setup:
    //   1. Add component to NPC
    //   2. Drag a DialogueAsset into the slot
    //   3. Call trigger.Trigger() from your
    //      interaction system or on collision
    // =========================================
    public class DialogueTrigger : MonoBehaviour, Framework.Interaction.IInteractable
    {
        [Header("Dialogue")]
        public DialogueAsset dialogueAsset;

        [Header("Settings")]
        [Tooltip("Only trigger once per play session")]
        public bool onlyOnce = false;

        private bool _triggered;

        // =========================================
        // IInteractable — works with InteractionSystem
        // =========================================
        [Header("Interaction")]
        public string interactionPrompt = "Talk";

        public string PromptText => interactionPrompt;

        public bool CanInteract(UnityEngine.Transform interactor)
            => dialogueAsset != null && !(onlyOnce && _triggered);

        public void Interact(UnityEngine.Transform interactor)
            => Trigger();

        public void Trigger()
        {
            if (dialogueAsset == null) return;
            if (onlyOnce && _triggered) return;

            var system = ServiceLocator.Get<IDialogueSystem>();
            if (system == null)
            {
                Debug.LogWarning(
                    "[DialogueTrigger] No IDialogueSystem found. " +
                    "Add DialogueSystem to _GameSystems.");
                return;
            }

            system.Play(dialogueAsset.Build());
            _triggered = true;
        }

        // =========================================
        // Optional — trigger on player proximity
        // =========================================
        [Header("Auto Trigger")]
        [Tooltip("Trigger automatically when player enters range")]
        public bool  autoTrigger    = false;
        public float triggerRadius  = 2f;
        public string playerTag     = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (!autoTrigger) return;
            if (!other.CompareTag(playerTag)) return;
            Trigger();
        }

        private void OnDrawGizmosSelected()
        {
            if (!autoTrigger) return;
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, triggerRadius);
        }
    }
}
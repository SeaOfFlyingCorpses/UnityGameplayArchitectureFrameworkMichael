using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Interaction;

namespace Gameplay.Interaction
{
    // =========================================
    // InteractionSystem
    // Attach to the Player GameObject.
    //
    // Detects IInteractable objects in range.
    // Publishes focus/unfocus events for UI.
    // Calls Interact() when player presses E.
    //
    // Connects to DialogueTrigger automatically
    // — DialogueTrigger.Trigger() is called
    // via IInteractable.Interact().
    // =========================================
    public class InteractionSystem : MonoBehaviour
    {
        [Header("Detection")]
        public float        interactRange = 2f;
        public LayerMask    interactLayer;
        public string       interactTag   = "";

        [Header("Input")]
        [Tooltip("Read from InputState automatically " +
                 "if InputHandler is assigned")]
        public Gameplay.Input.InputHandler inputHandler;

        private IInteractable _focused;
        private Transform     _focusedTransform;

        private void Update()
        {
            DetectInteractable();
            HandleInput();
        }

        // =========================================
        // DETECTION
        // Finds the closest IInteractable in range
        // =========================================
        private void DetectInteractable()
        {
            var hits = Physics.OverlapSphere(
                transform.position,
                interactRange,
                interactLayer);

            IInteractable  closest          = null;
            Transform      closestTransform = null;
            float          closestDist      = float.MaxValue;

            foreach (var hit in hits)
            {
                // Tag filter — skip if tag set and doesn't match
                if (!string.IsNullOrEmpty(interactTag) &&
                    !hit.CompareTag(interactTag))
                    continue;

                var interactable = hit.GetComponent<IInteractable>()
                                ?? hit.GetComponentInParent<IInteractable>();

                if (interactable == null) continue;
                if (!interactable.CanInteract(transform)) continue;

                float dist = Vector3.Distance(
                    transform.position, hit.transform.position);

                if (dist >= closestDist) continue;

                closest          = interactable;
                closestTransform = hit.transform;
                closestDist      = dist;
            }

            // Focus changed
            if (closest != _focused)
            {
                _focused          = closest;
                _focusedTransform = closestTransform;

                if (_focused != null)
                    EventBus.Publish(new InteractionFocusedEvent(
                        _focused.PromptText, _focusedTransform));
                else
                    EventBus.Publish(new InteractionUnfocusedEvent());
            }
        }

        // =========================================
        // INPUT
        // =========================================
        private void HandleInput()
        {
            if (_focused == null) return;

            bool pressed = inputHandler != null
                ? inputHandler.State.InteractPressed
                : UnityEngine.Input.GetKeyDown(KeyCode.E);

            if (!pressed) return;

            Interact();
        }

        public void Interact()
        {
            if (_focused == null) return;
            if (!_focused.CanInteract(transform)) return;

            _focused.Interact(transform);

            EventBus.Publish(new InteractedEvent(
                _focusedTransform.name,
                transform,
                _focusedTransform));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.2f);
            Gizmos.DrawSphere(transform.position, interactRange);
        }
    }
}

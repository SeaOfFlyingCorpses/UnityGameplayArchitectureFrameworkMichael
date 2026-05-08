using UnityEngine;
using Framework.Interaction;

namespace Gameplay.Interaction
{
    // =========================================
    // InteractableDoor
    // A door the player can open and close.
    // Animates via Animator if present.
    // Blocks movement via Collider toggle.
    //
    // Setup:
    //   1. Add to Door GameObject
    //   2. Add a Collider for blocking
    //   3. Optional: add Animator with
    //      "IsOpen" bool parameter
    // =========================================
    public class InteractableDoor : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        public bool   startsOpen   = false;
        public bool   lockedByDefault = false;
        public string lockedPrompt = "Locked";
        public string openPrompt   = "Open Door";
        public string closePrompt  = "Close Door";

        private bool      _isOpen;
        private bool      _isLocked;
        private Animator  _animator;
        private Collider  _collider;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            _isLocked = lockedByDefault;
            _isOpen   = startsOpen;

            ApplyState();
        }

        public string PromptText
        {
            get
            {
                if (_isLocked)  return lockedPrompt;
                return _isOpen  ? closePrompt : openPrompt;
            }
        }

        public bool CanInteract(Transform interactor)
            => !_isLocked;

        public void Interact(Transform interactor)
        {
            if (_isLocked) return;

            _isOpen = !_isOpen;
            ApplyState();
        }

        // =========================================
        // Call from key/puzzle systems to unlock
        // =========================================
        public void Unlock()
        {
            _isLocked = false;
        }

        public void Lock()
        {
            _isLocked = true;
            if (_isOpen) { _isOpen = false; ApplyState(); }
        }

        private void ApplyState()
        {
            _animator?.SetBool("IsOpen", _isOpen);

            // Disable collider when open so player can pass
            if (_collider != null)
                _collider.enabled = !_isOpen;
        }
    }
}

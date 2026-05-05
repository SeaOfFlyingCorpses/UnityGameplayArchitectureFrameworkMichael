using Framework.StateMachine;
using UnityEngine;

namespace Framework.Animation
{
    // =========================================
    // AnimationSystem
    // Reads AnimationRequest from StateContext
    // each LateUpdate and plays it via
    // AnimationController.
    //
    // StateContext is injected via SetContext()
    // from AIController.BindSystems() —
    // never assign it manually in the Inspector.
    //
    // AnimationController is assigned in Inspector
    // since the Animator may be on a child object.
    //
    // Attach to the same GameObject as AIController.
    // =========================================
    public class AnimationSystem : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the AnimationController — " +
                 "may be on a child GameObject if Animator is separate")]
        public AnimationController controller;

        private StateContext _context;

        // =========================================
        // INJECT — called by AIController
        // =========================================
        public void SetContext(StateContext context)
        {
            _context = context;
        }

        private void LateUpdate()
        {
            if (_context == null || controller == null)
                return;

            if (!_context.AnimationRequest.HasValue)
                return;

            controller.Play(_context.AnimationRequest.Value);
            _context.AnimationRequest = null;
        }
    }
}
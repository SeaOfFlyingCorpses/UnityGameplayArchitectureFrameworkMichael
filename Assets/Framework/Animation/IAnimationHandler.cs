using UnityEngine;

namespace Framework.Animation
{
    // =========================================
    // IAnimationHandler
    // Handles one AnimationType on an Animator.
    //
    // One class per animation type.
    // Register into AnimationController via
    // RegisterHandler() — no switch needed.
    //
    // Example — add a Dodge animation:
    //   1. Add Dodge to AnimationType enum
    //   2. Create DodgeAnimationHandler : IAnimationHandler
    //   3. Register it in AnimationController.RegisterDefaults()
    //   Zero changes to existing code.
    // =========================================
    public interface IAnimationHandler
    {
        AnimationType Type { get; }
        void Play(Animator animator, AnimationRequest request);
    }
}

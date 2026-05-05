using UnityEngine;

namespace Framework.Animation
{
    // =========================================
    // Default animation handlers.
    // One class per AnimationType.
    // All Animator parameter names live here —
    // no magic strings anywhere else.
    //
    // To add a new animation type:
    //   1. Add to AnimationType enum
    //   2. Create a new handler class here
    //   3. Register in AnimationController.RegisterDefaults()
    // =========================================

    public class IdleAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Idle;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Attack");
        }
    }

    public class MoveAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Move;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetFloat("Speed", 1f);
        }
    }

    public class AttackAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Attack;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetTrigger("Attack");
        }
    }

    public class HitAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Hit;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetTrigger("Hit");
        }
    }

    public class DeathAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Death;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetTrigger("Death");
            animator.SetBool("IsDead", true);
        }
    }

    public class DodgeAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Dodge;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetTrigger("Dodge");
        }
    }

    public class StaggerAnimationHandler : IAnimationHandler
    {
        public AnimationType Type => AnimationType.Stagger;

        public void Play(Animator animator, AnimationRequest request)
        {
            animator.SetTrigger("Stagger");
        }
    }
}

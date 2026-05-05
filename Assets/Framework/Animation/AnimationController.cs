using System.Collections.Generic;
using UnityEngine;

namespace Framework.Animation
{
    // =========================================
    // AnimationController
    // Plays animations by looking up the correct
    // IAnimationHandler from a dictionary.
    // Zero switch statements — fully data-driven.
    //
    // To add a new animation type:
    //   1. Add to AnimationType enum
    //   2. Create handler in AnimationHandlers.cs
    //   3. Register in RegisterDefaults() below
    //   No other files change.
    //
    // To override a handler at runtime:
    //   animationController.RegisterHandler(new MyCustomAttackHandler());
    // =========================================
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;

        private readonly Dictionary<AnimationType, IAnimationHandler> _handlers = new();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            RegisterDefaults();
        }

        // =========================================
        // REGISTER ALL DEFAULT HANDLERS
        // =========================================
        private void RegisterDefaults()
        {
            RegisterHandler(new IdleAnimationHandler());
            RegisterHandler(new MoveAnimationHandler());
            RegisterHandler(new AttackAnimationHandler());
            RegisterHandler(new HitAnimationHandler());
            RegisterHandler(new DeathAnimationHandler());
            RegisterHandler(new DodgeAnimationHandler());
            RegisterHandler(new StaggerAnimationHandler());
        }

        // =========================================
        // REGISTER — adds or replaces a handler
        // =========================================
        public void RegisterHandler(IAnimationHandler handler)
        {
            if (handler == null)
                return;

            _handlers[handler.Type] = handler;
        }

        // =========================================
        // PLAY — looks up handler and calls it
        // =========================================
        public void Play(AnimationRequest request)
        {
            if (_animator == null)
                return;

            if (_handlers.TryGetValue(request.Type, out var handler))
                handler.Play(_animator, request);
            else
                Debug.LogWarning($"[AnimationController] No handler registered for {request.Type}");
        }
    }
}
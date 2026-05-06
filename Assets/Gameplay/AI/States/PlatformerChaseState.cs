using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;
using Gameplay.Systems.Movement;

namespace Gameplay.AI.States
{
    // =========================================
    // PlatformerChaseState
    // 2D platformer AI chase behaviour.
    // Moves horizontally toward target.
    // Jumps when target is above and grounded.
    // Stops when in attack range.
    //
    // Requires PlatformerMovementStrategy.
    // Works with PatrolAIStateFactory or any
    // factory that targets a 2D platformer agent.
    //
    // Usage:
    //   var chase = new PlatformerChaseState(
    //       attackState, searchState,
    //       speed: 3f, jumpThreshold: 1.5f);
    // =========================================
    public class PlatformerChaseState : IState
    {
        private readonly List<Transition> _transitions = new();
        private readonly IState           _attackState;
        private readonly IState           _lostState;
        private readonly float            _speed;
        private readonly float            _jumpThreshold;
        private readonly float            _attackRange;

        // =========================================
        // jumpThreshold — how many units above the
        // target the agent must be before jumping
        // attackRange — stop chasing when this close
        // =========================================
        public PlatformerChaseState(
            IState attackState,
            IState lostState,
            float  speed          = 3f,
            float  jumpThreshold  = 1.5f,
            float  attackRange    = 1.5f)
        {
            _attackState   = attackState;
            _lostState     = lostState;
            _speed         = speed;
            _jumpThreshold = jumpThreshold;
            _attackRange   = attackRange;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Update(StateContext context)
        {
            var target = context.Target;

            if (target == null) return;

            Vector2 toTarget = new Vector2(
                target.position.x - context.Self.position.x,
                target.position.y - context.Self.position.y);

            float distance = toTarget.magnitude;

            // Stop chasing — transition to attack handled
            // by transition conditions, not here
            if (distance <= _attackRange)
            {
                context.Movement?.Stop(context.Self);
                return;
            }

            // Move horizontally toward target
            Vector3 dir = new Vector3(
                Mathf.Sign(toTarget.x), 0f, 0f);

            context.Movement?.MoveInDirection(
                context.Self, dir, _speed);

            // Jump if target is significantly above
            // and agent is grounded
            var platformer = context.Movement
                as PlatformerMovementStrategy;

            if (platformer != null &&
                platformer.IsGrounded &&
                toTarget.y > _jumpThreshold)
            {
                platformer.Jump();
            }

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Move);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}

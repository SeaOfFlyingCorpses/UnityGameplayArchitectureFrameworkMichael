using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;
using Gameplay.Systems.Movement;

namespace Gameplay.States
{
    // =========================================
    // DoubleJumpState
    // Second jump while airborne.
    // Resets when agent lands.
    // Supports N jumps (triple jump etc.)
    // via maxExtraJumps.
    //
    // Works with PlatformerMovementStrategy.
    //
    // Wire in factory:
    //   fall.AddTransition(new Transition(
    //       new DoubleJumpCondition(dj),
    //       dj));
    //   jump.AddTransition(new Transition(
    //       new DoubleJumpCondition(dj),
    //       dj));
    // =========================================
    public class DoubleJumpState : IState
    {
        private readonly List<Transition> _transitions = new();

        private readonly float _jumpForce;
        private readonly int   _maxExtraJumps;
        private int            _jumpsUsed;

        public bool CanDoubleJump => _jumpsUsed < _maxExtraJumps;

        public DoubleJumpState(
            float jumpForce      = 12f,
            int   maxExtraJumps  = 1)
        {
            _jumpForce     = jumpForce;
            _maxExtraJumps = maxExtraJumps;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            _jumpsUsed++;

            context.AnimationRequest =
                new AnimationRequest(AnimationType.Jump);

            // Force an upward velocity
            var platformer = context.Movement
                as PlatformerMovementStrategy;

            if (platformer != null)
            {
                platformer.SetVelocityY(0f);
                platformer.Jump();
            }
        }

        public void Update(StateContext context)
        {
            // Horizontal control during double jump
            Vector2 input = context.Input.Move;
            if (Mathf.Abs(input.x) > 0.01f)
                context.Movement?.MoveInDirection(
                    context.Self,
                    new Vector3(input.x, 0f, 0f),
                    5f);
        }

        public void Exit() { }

        // Called when agent lands — resets counter
        public void ResetJumps() => _jumpsUsed = 0;

        public List<Transition> GetTransitions() => _transitions;
    }
}

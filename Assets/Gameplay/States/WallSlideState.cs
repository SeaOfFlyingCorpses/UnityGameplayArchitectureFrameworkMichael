using System.Collections.Generic;
using UnityEngine;
using Framework.Animation;
using Framework.StateMachine;

namespace Gameplay.States
{
    // =========================================
    // WallSlideState
    // Agent is pressing against a wall while
    // airborne — slides down slowly.
    // Can jump off wall (wall jump).
    // =========================================
    public class WallSlideState : IState
    {
        private readonly List<Transition> _transitions = new();
        private readonly float _slideSpeed;

        public WallSlideState(float slideSpeed = 1.5f)
        {
            _slideSpeed = slideSpeed;
        }

        public void AddTransition(Transition t) => _transitions.Add(t);

        public void Enter(StateContext context)
        {
            context.AnimationRequest =
                new AnimationRequest(AnimationType.WallSlide);
        }

        public void Update(StateContext context)
        {
            // Slow fall while wall sliding
            var platformer = context.Movement
                as Gameplay.Systems.Movement.PlatformerMovementStrategy;
            platformer?.SetVelocityY(-_slideSpeed);
        }

        public void Exit() { }
        public List<Transition> GetTransitions() => _transitions;
    }
}
